using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate.Events;
using UniThesis.Domain.Enums.Notification;

namespace UniThesis.Application.Features.DirectRegistration.EventHandlers;

public class ProjectMentorApprovedEventHandler : INotificationHandler<ProjectMentorApprovedEvent>
{
    private readonly ILogger<ProjectMentorApprovedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IProjectRepository _projectRepository;
    private readonly IGroupRepository _groupRepository;

    public ProjectMentorApprovedEventHandler(
        ILogger<ProjectMentorApprovedEventHandler> logger,
        INotificationService notificationService,
        IProjectRepository projectRepository,
        IGroupRepository groupRepository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _projectRepository = projectRepository;
        _groupRepository = groupRepository;
    }

    public async Task Handle(ProjectMentorApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Project {ProjectId} approved by mentor {MentorId}",
            notification.ProjectId, notification.MentorId);

        var project = await _projectRepository.GetByIdAsync(notification.ProjectId, cancellationToken);
        if (project?.GroupId is null) return;

        var group = await _groupRepository.GetWithMembersAsync(project.GroupId.Value, cancellationToken);
        if (group?.LeaderId is null) return;

        await _notificationService.SendAsync(
            group.LeaderId.Value,
            "Giảng viên đã duyệt đề tài",
            $"Đề tài \"{project.NameVi}\" đã được giảng viên duyệt và gửi đi thẩm định.",
            NotificationType.Success,
            NotificationCategory.Project,
            $"/student/my-topic",
            cancellationToken);
    }
}
