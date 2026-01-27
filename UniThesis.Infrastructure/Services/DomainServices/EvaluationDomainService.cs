using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Domain.Services;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Infrastructure.Services.DomainServices
{
    public class EvaluationDomainService : IEvaluationDomainService
    {
        private readonly IEvaluationSubmissionRepository _submissionRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly AppDbContext _context;
        private const int MaxResubmissions = 3;
        private const int ModificationDeadlineDays = 14;

        public EvaluationDomainService(
            IEvaluationSubmissionRepository submissionRepository,
            IProjectRepository projectRepository,
            IDateTimeService dateTimeService,
            AppDbContext context)
        {
            _submissionRepository = submissionRepository;
            _projectRepository = projectRepository;
            _dateTimeService = dateTimeService;
            _context = context;
        }

        public async Task<bool> CanResubmitAsync(Guid projectId, CancellationToken ct = default)
        {
            var remaining = await GetRemainingResubmissionsAsync(projectId, ct);
            var deadlinePassed = await IsModificationDeadlinePassedAsync(projectId, ct);
            return remaining > 0 && !deadlinePassed;
        }

        public async Task<int> GetRemainingResubmissionsAsync(Guid projectId, CancellationToken ct = default)
        {
            var submissionCount = await _submissionRepository.GetSubmissionCountByProjectIdAsync(projectId, ct);
            return Math.Max(0, MaxResubmissions - submissionCount + 1); // +1 because first submission is not a resubmission
        }

        public async Task<bool> IsModificationDeadlinePassedAsync(Guid projectId, CancellationToken ct = default)
        {
            var latestSubmission = await _submissionRepository.GetLatestByProjectIdAsync(projectId, ct);
            if (latestSubmission is null) return false;

            if (latestSubmission.Result != EvaluationResult.NeedsModification) return false;

            var deadline = latestSubmission.EvaluatedAt?.AddDays(ModificationDeadlineDays);
            return deadline.HasValue && _dateTimeService.UtcNow > deadline.Value;
        }

        public async Task<EvaluationStatistics> GetStatisticsAsync(int semesterId, CancellationToken ct = default)
        {
            var submissions = await _context.EvaluationSubmissions
                .Include(e => e.Snapshot)
                .Where(e => _context.Projects.Any(p => p.Id == e.ProjectId && p.SemesterId == semesterId))
                .ToListAsync(ct);

            var evaluatorWorkload = submissions
                .Where(s => s.AssignedEvaluatorId.HasValue)
                .GroupBy(s => s.AssignedEvaluatorId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            var completedSubmissions = submissions.Where(s => s.Status == SubmissionStatus.Completed).ToList();
            var avgDays = completedSubmissions.Any()
                ? completedSubmissions.Average(s => (s.EvaluatedAt!.Value - s.SubmittedAt).TotalDays)
                : 0;

            return new EvaluationStatistics(
                TotalSubmissions: submissions.Count,
                PendingSubmissions: submissions.Count(s => s.Status == SubmissionStatus.Pending),
                InReviewSubmissions: submissions.Count(s => s.Status == SubmissionStatus.InReview),
                CompletedSubmissions: completedSubmissions.Count,
                ApprovedCount: submissions.Count(s => s.Result == EvaluationResult.Approved),
                NeedsModificationCount: submissions.Count(s => s.Result == EvaluationResult.NeedsModification),
                RejectedCount: submissions.Count(s => s.Result == EvaluationResult.Rejected),
                AverageEvaluationDays: avgDays,
                EvaluatorWorkload: evaluatorWorkload
            );
        }

        public async Task<Guid?> SuggestEvaluatorAsync(Guid submissionId, CancellationToken ct = default)
        {
            // Simple workload-based suggestion
            var submission = await _submissionRepository.GetByIdAsync(submissionId, ct);
            if (submission is null) return null;

            var project = await _projectRepository.GetByIdAsync(submission.ProjectId, ct);
            if (project is null) return null;

            // Get evaluators with least workload in the same department
            var evaluatorWorkloads = await _context.EvaluationSubmissions
                .Where(e => e.Status == SubmissionStatus.Pending || e.Status == SubmissionStatus.InReview)
                .Where(e => e.AssignedEvaluatorId.HasValue)
                .GroupBy(e => e.AssignedEvaluatorId!.Value)
                .Select(g => new { EvaluatorId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            // This is simplified - in real implementation, filter by department and expertise
            var leastLoaded = evaluatorWorkloads.OrderBy(e => e.Count).FirstOrDefault();
            return leastLoaded?.EvaluatorId;
        }
    }
}
