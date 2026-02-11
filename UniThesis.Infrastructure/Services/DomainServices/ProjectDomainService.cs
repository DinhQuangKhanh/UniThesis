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
                return (false, ["Đề tài không tồn tại."]);

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
            var statusCounts = await _context.Projects
                .Where(p => p.SemesterId == semesterId)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, ct);

            var sourceCounts = await _context.Projects
                .Where(p => p.SemesterId == semesterId)
                .GroupBy(p => p.SourceType)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Source, x => x.Count, ct);

            int Count(ProjectStatus s) => statusCounts.GetValueOrDefault(s);

            return new ProjectStatistics(
                TotalProjects: statusCounts.Values.Sum(),
                DraftProjects: Count(ProjectStatus.Draft),
                PendingEvaluationProjects: Count(ProjectStatus.PendingEvaluation),
                ApprovedProjects: Count(ProjectStatus.Approved),
                RejectedProjects: Count(ProjectStatus.Rejected),
                InProgressProjects: Count(ProjectStatus.InProgress),
                CompletedProjects: Count(ProjectStatus.Completed),
                CancelledProjects: Count(ProjectStatus.Cancelled),
                FromPoolCount: sourceCounts.GetValueOrDefault(ProjectSourceType.FromPool),
                DirectRegistrationCount: sourceCounts.GetValueOrDefault(ProjectSourceType.DirectRegistration)
            );
        }
    }
}
