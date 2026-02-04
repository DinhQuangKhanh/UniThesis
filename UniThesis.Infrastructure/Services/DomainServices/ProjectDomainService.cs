using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.EvaluationAggregate.ValueObjects;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Project;
using UniThesis.Domain.Services;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Infrastructure.Services.DomainServices
{
    public class ProjectDomainService : IProjectDomainService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly AppDbContext _context;

        public ProjectDomainService(
            IProjectRepository projectRepository,
            IGroupRepository groupRepository,
            IDateTimeService dateTimeService,
            AppDbContext context)
        {
            _projectRepository = projectRepository;
            _groupRepository = groupRepository;
            _dateTimeService = dateTimeService;
            _context = context;
        }

        public async Task<string> GenerateProjectCodeAsync(int year, CancellationToken ct = default)
        {
            var sequence = await _projectRepository.GetNextSequenceAsync(year, ct);
            return $"PROJ-{year}-{sequence:D4}";
        }

        public async Task<(bool IsValid, string[] Errors)> ValidateForSubmissionAsync(Guid projectId, CancellationToken ct = default)
        {
            var errors = new List<string>();
            var project = await _projectRepository.GetWithMentorsAsync(projectId, ct);

            if (project is null)
                return (false, new[] { "Đề tài không tồn tại." });

            if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.NeedsModification)
                errors.Add("Đề tài chỉ có thể nộp khi ở trạng thái Nháp hoặc Cần chỉnh sửa.");

            if (!project.Mentors.Any(m => m.IsActive))
                errors.Add("Đề tài phải có ít nhất một giảng viên hướng dẫn.");

            if (string.IsNullOrWhiteSpace(project.Description))
                errors.Add("Mô tả đề tài không được để trống.");

            if (string.IsNullOrWhiteSpace(project.Objectives))
                errors.Add("Mục tiêu đề tài không được để trống.");

            if (project.NameVi is null || string.IsNullOrWhiteSpace(project.NameVi.Value))
                errors.Add("Tên đề tài tiếng Việt không được để trống.");

            if (project.NameEn is null || string.IsNullOrWhiteSpace(project.NameEn.Value))
                errors.Add("Tên đề tài tiếng Anh không được để trống.");

            return (errors.Count == 0, errors.ToArray());
        }

        public Task<ProjectSnapshot> CreateSnapshotAsync(Project project, CancellationToken ct = default)
        {
            var snapshot = ProjectSnapshot.Capture(
                project.NameVi.Value,
                project.NameEn.Value,
                project.NameAbbr,
                project.Description,
                project.Objectives,
                project.Scope ?? string.Empty,
                project.Technologies?.Value ?? string.Empty,
                project.ExpectedResults ?? string.Empty,
                _dateTimeService.UtcNow
            );

            return Task.FromResult(snapshot);
        }

        public async Task<ProjectStatistics> GetStatisticsAsync(int semesterId, CancellationToken ct = default)
        {
            var projects = await _context.Projects
                .Where(p => p.SemesterId == semesterId)
                .ToListAsync(ct);

            return new ProjectStatistics(
                TotalProjects: projects.Count,
                DraftProjects: projects.Count(p => p.Status == ProjectStatus.Draft),
                PendingEvaluationProjects: projects.Count(p => p.Status == ProjectStatus.PendingEvaluation),
                ApprovedProjects: projects.Count(p => p.Status == ProjectStatus.Approved),
                RejectedProjects: projects.Count(p => p.Status == ProjectStatus.Rejected),
                InProgressProjects: projects.Count(p => p.Status == ProjectStatus.InProgress),
                CompletedProjects: projects.Count(p => p.Status == ProjectStatus.Completed),
                CancelledProjects: projects.Count(p => p.Status == ProjectStatus.Cancelled),
                FromPoolCount: projects.Count(p => p.SourceType == ProjectSourceType.FromPool),
                DirectRegistrationCount: projects.Count(p => p.SourceType == ProjectSourceType.DirectRegistration)
            );
        }
    }
}
