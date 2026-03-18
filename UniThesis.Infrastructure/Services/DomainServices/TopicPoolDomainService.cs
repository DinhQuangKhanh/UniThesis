using Microsoft.Extensions.Logging;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Entities;
using UniThesis.Domain.Enums.Project;
using UniThesis.Domain.Enums.TopicPool;
using UniThesis.Domain.Services;

namespace UniThesis.Infrastructure.Services.DomainServices;

/// <summary>
/// Domain service implementation for topic pool-related business logic.
/// </summary>
public class TopicPoolDomainService : ITopicPoolDomainService
{
    private readonly ITopicPoolRepository _topicPoolRepository;
    private readonly ITopicRegistrationRepository _registrationRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMajorReadRepository _majorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TopicPoolDomainService> _logger;

    public TopicPoolDomainService(
        ITopicPoolRepository topicPoolRepository,
        ITopicRegistrationRepository registrationRepository,
        IProjectRepository projectRepository,
        IMajorReadRepository majorRepository,
        IUnitOfWork unitOfWork,
        ILogger<TopicPoolDomainService> logger)
    {
        _topicPoolRepository = topicPoolRepository;
        _registrationRepository = registrationRepository;
        _projectRepository = projectRepository;
        _majorRepository = majorRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> GeneratePoolCodeAsync(int majorId, CancellationToken cancellationToken = default)
    {
        var major = await _majorRepository.GetByIdAsync(majorId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Major), majorId);

        return TopicPool.GenerateCode(major.Code);
    }

    public async Task<TopicPool> GetOrCreatePoolAsync(int majorId, Guid createdBy, CancellationToken cancellationToken = default)
    {
        // Try to get existing pool
        var existingPool = await _topicPoolRepository.GetByMajorIdAsync(majorId, cancellationToken);
        if (existingPool is not null)
            return existingPool;

        // Create new pool
        var major = await _majorRepository.GetByIdAsync(majorId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Major), majorId);

        var code = TopicPool.GenerateCode(major.Code);
        var name = $"Kho đề tài {major.Name}";

        var pool = TopicPool.Create(code, name, null, majorId, createdBy: createdBy);

        await _topicPoolRepository.AddAsync(pool, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created topic pool {PoolCode} for major {MajorId}", code, majorId);

        return pool;
    }

    public async Task<int> GetMentorActiveTopicCountAsync(Guid mentorId, Guid topicPoolId, CancellationToken cancellationToken = default)
    {
        // Active = Available or Reserved (not Assigned or Expired)
        return await _projectRepository.CountActivePoolTopicsByMentorAsync(topicPoolId, mentorId, cancellationToken);
    }

    public async Task<(bool CanPropose, string? Reason)> CanMentorProposeTopicAsync(
        Guid mentorId,
        Guid topicPoolId,
        CancellationToken cancellationToken = default)
    {
        var pool = await _topicPoolRepository.GetByIdAsync(topicPoolId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(TopicPool), topicPoolId);

        if (!pool.IsAcceptingProposals())
            return (false, "Topic pool is not currently accepting proposals.");

        var currentCount = await GetMentorActiveTopicCountAsync(mentorId, topicPoolId, cancellationToken);
        if (currentCount >= pool.MaxActiveTopicsPerMentor)
            return (false, $"You have reached the maximum of {pool.MaxActiveTopicsPerMentor} active topics in this pool.");

        return (true, null);
    }

    public async Task<TopicRegistration> RequestRegistrationAsync(
        Guid projectId,
        Guid groupId,
        Guid registeredBy,
        int priority = 1,
        string? note = null,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Project), projectId);

        // Validate project
        if (project.SourceType != ProjectSourceType.FromPool)
            throw new BusinessRuleValidationException("This project is not from the topic pool.");

        if (project.PoolStatus != PoolTopicStatus.Available)
            throw new BusinessRuleValidationException("This topic is not available for registration.");

        if (project.Status != ProjectStatus.Approved)
            throw new BusinessRuleValidationException("Only approved topics can be registered for.");

        if (!project.TopicPoolId.HasValue)
            throw new BusinessRuleValidationException("Project is not associated with a topic pool.");

        var pool = await _topicPoolRepository.GetByIdAsync(project.TopicPoolId.Value, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(TopicPool), project.TopicPoolId.Value);

        if (!pool.IsAcceptingRegistrations())
            throw new BusinessRuleValidationException("Topic pool is not currently accepting registrations.");

        // Check duplicate
        var hasPending = await _registrationRepository.HasPendingRegistrationAsync(groupId, projectId, cancellationToken);
        if (hasPending)
            throw new BusinessRuleValidationException("Your group already has a pending registration for this topic.");

        // Create registration
        var registration = TopicRegistration.Create(projectId, groupId, registeredBy, priority, note);
        await _registrationRepository.AddAsync(registration, cancellationToken);

        // Update project status to Reserved
        project.SetPoolStatus(PoolTopicStatus.Reserved);
        _projectRepository.Update(project);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration requested: Project {ProjectId}, Group {GroupId}", projectId, groupId);

        return registration;
    }

    public async Task ConfirmRegistrationAsync(
        Guid registrationId,
        Guid confirmedBy,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetByIdAsync(registrationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(TopicRegistration), registrationId);

        if (registration.Status != TopicRegistrationStatus.Pending)
            throw new BusinessRuleValidationException("Only pending registrations can be confirmed.");

        var project = await _projectRepository.GetByIdAsync(registration.ProjectId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Project), registration.ProjectId);

        // Confirm the registration
        registration.Confirm(confirmedBy);
        _registrationRepository.Update(registration);

        // Assign group to project and update status
        project.AssignGroup(registration.GroupId);
        project.SetPoolStatus(PoolTopicStatus.Assigned);
        _projectRepository.Update(project);

        // Cancel all other pending registrations for this project
        var otherPendingRegistrations = await _registrationRepository.GetPendingByProjectIdAsync(registration.ProjectId, cancellationToken);
        foreach (var otherReg in otherPendingRegistrations.Where(r => r.Id != registrationId))
        {
            otherReg.Cancel("Another group was selected for this topic.");
            _registrationRepository.Update(otherReg);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration confirmed: {RegistrationId}, Group {GroupId} assigned to Project {ProjectId}",
            registrationId, registration.GroupId, registration.ProjectId);
    }

    public async Task RejectRegistrationAsync(
        Guid registrationId,
        Guid rejectedBy,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetByIdAsync(registrationId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(TopicRegistration), registrationId);

        if (registration.Status != TopicRegistrationStatus.Pending)
            throw new BusinessRuleValidationException("Only pending registrations can be rejected.");

        registration.Reject(rejectedBy, reason);
        _registrationRepository.Update(registration);

        // Check if there are any other pending registrations for this project
        var otherPendingCount = await _registrationRepository.CountPendingByProjectIdExcludingAsync(
            registration.ProjectId, registrationId, cancellationToken);

        // If no other pending registrations, set project back to Available
        if (otherPendingCount == 0)
        {
            var project = await _projectRepository.GetByIdAsync(registration.ProjectId, cancellationToken);
            if (project is not null && project.PoolStatus == PoolTopicStatus.Reserved)
            {
                project.SetPoolStatus(PoolTopicStatus.Available);
                _projectRepository.Update(project);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration rejected: {RegistrationId}, Reason: {Reason}", registrationId, reason);
    }

    public async Task<TopicPoolStatistics> GetPoolStatisticsAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
    {
        var pool = await _topicPoolRepository.GetByIdAsync(topicPoolId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(TopicPool), topicPoolId);

        var statusCounts = await _projectRepository.GetPoolStatusCountsAsync(topicPoolId, cancellationToken);
        var totalTopics = statusCounts.Values.Sum();

        var projectIds = await _projectRepository.GetPoolProjectIdsAsync(topicPoolId, cancellationToken);

        var registrationCounts = await _registrationRepository.GetRegistrationStatusCountsByProjectIdsAsync(projectIds, cancellationToken);

        var mentorCounts = await _projectRepository.GetMentorTopicCountsInPoolAsync(topicPoolId, cancellationToken);

        return new TopicPoolStatistics(
            TotalTopics: totalTopics,
            AvailableTopics: statusCounts.GetValueOrDefault(PoolTopicStatus.Available),
            ReservedTopics: statusCounts.GetValueOrDefault(PoolTopicStatus.Reserved),
            AssignedTopics: statusCounts.GetValueOrDefault(PoolTopicStatus.Assigned),
            ExpiredTopics: statusCounts.GetValueOrDefault(PoolTopicStatus.Expired),
            TotalRegistrations: registrationCounts.Values.Sum(),
            PendingRegistrations: registrationCounts.GetValueOrDefault(TopicRegistrationStatus.Pending),
            ConfirmedRegistrations: registrationCounts.GetValueOrDefault(TopicRegistrationStatus.Confirmed),
            TopMentorTopicCount: mentorCounts.Count > 0 ? mentorCounts.Max() : 0,
            AverageTopicsPerMentor: mentorCounts.Count > 0 ? mentorCounts.Average() : 0
        );
    }

    public async Task<int> ExpireOldTopicsAsync(int currentSemesterId, CancellationToken cancellationToken = default)
    {
        // Find all projects that should be expired
        var expiredProjects = await _projectRepository.GetExpirablePoolTopicsAsync(currentSemesterId, cancellationToken);

        foreach (var project in expiredProjects)
        {
            project.MarkAsExpired();
            _projectRepository.Update(project);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Expired {Count} topics in semester {SemesterId}", expiredProjects.Count, currentSemesterId);

        return expiredProjects.Count;
    }

    public async Task<IEnumerable<Guid>> GetAvailableTopicsInPoolAsync(Guid topicPoolId, CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetAvailableApprovedPoolTopicIdsAsync(topicPoolId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetExpiringTopicsAsync(int currentSemesterId, CancellationToken cancellationToken = default)
    {
        return await _projectRepository.GetExpiringPoolTopicsWithMentorsAsync(currentSemesterId, cancellationToken);
    }

    public async Task<int> CalculateExpirationSemesterAsync(int createdSemesterId, int expirationSemesters, CancellationToken cancellationToken = default)
    {
        // Simple calculation: add N to semester ID
        // In real implementation, you might need to look up actual semester sequence
        return createdSemesterId + expirationSemesters;
    }
}