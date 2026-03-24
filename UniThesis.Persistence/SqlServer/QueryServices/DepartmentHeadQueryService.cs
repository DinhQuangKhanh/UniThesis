using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Departments.DTOs;
using UniThesis.Domain.Constants;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Persistence.SqlServer.QueryServices
{
    public class DepartmentHeadQueryService : IDepartmentHeadQueryService
    {
        private readonly AppDbContext _context;

        public DepartmentHeadQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DepartmentProjectsResponse> GetDepartmentProjectsAsync(
            int departmentId,
            CancellationToken cancellationToken = default)
        {
            var majorIds = await _context.Majors
                .Where(m => m.DepartmentId == departmentId && m.IsActive)
                .Select(m => m.Id)
                .ToListAsync(cancellationToken);

            // Get projects that are pending evaluation or have been evaluated
            var projects = await _context.Projects
                .Include(p => p.Mentors)
                .Where(p => majorIds.Contains(p.MajorId) &&
                           (p.Status == ProjectStatus.PendingEvaluation ||
                            p.Status == ProjectStatus.Approved ||
                            p.Status == ProjectStatus.NeedsModification ||
                            p.Status == ProjectStatus.Rejected))
                .OrderByDescending(p => p.SubmittedAt)
                .ToListAsync(cancellationToken);

            var projectIds = projects.Select(p => p.Id).ToList();

            // Get all assignments for these projects
            var assignments = await _context.ProjectEvaluatorAssignments
                .Where(a => projectIds.Contains(a.ProjectId) && a.IsActive)
                .ToListAsync(cancellationToken);

            // Get evaluator names
            var evaluatorIds = assignments.Select(a => a.EvaluatorId).Distinct().ToList();
            var evaluatorNames = await _context.Users
                .Where(u => evaluatorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            // Get mentor names
            var mentorIds = projects.SelectMany(p => p.Mentors.Where(m => m.IsActive).Select(m => m.MentorId)).Distinct().ToList();
            var mentorNames = await _context.Users
                .Where(u => mentorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            // Get major and semester names
            var majorNames = await _context.Majors
                .Where(m => majorIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Name, cancellationToken);

            var semesterIds = projects.Select(p => p.SemesterId).Distinct().ToList();
            var semesterNames = await _context.Semesters
                .Where(s => semesterIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name, cancellationToken);

            var items = projects.Select(p =>
            {
                var projectAssignments = assignments
                    .Where(a => a.ProjectId == p.Id)
                    .OrderBy(a => a.EvaluatorOrder)
                    .ToList();

                var submittedCount = projectAssignments.Count(a => a.HasSubmittedEvaluation);
                var distinctResults = projectAssignments
                    .Where(a => a.HasSubmittedEvaluation)
                    .Select(a => a.IndividualResult)
                    .Distinct()
                    .ToList();

                var hasConflict = submittedCount >= 2 && distinctResults.Count > 1;
                var needsDecision = hasConflict && p.Status == ProjectStatus.PendingEvaluation;

                return new DepartmentProjectDto
                {
                    ProjectId = p.Id,
                    ProjectCode = p.Code.Value,
                    NameVi = p.NameVi.Value,
                    NameEn = p.NameEn.Value,
                    MajorName = majorNames.GetValueOrDefault(p.MajorId, ""),
                    SemesterName = semesterNames.GetValueOrDefault(p.SemesterId, ""),
                    Status = p.Status.ToString(),
                    StatusValue = (int)p.Status,
                    SubmittedAt = p.SubmittedAt?.ToString("o"),
                    AssignedEvaluatorCount = projectAssignments.Count,
                    HasConflict = hasConflict,
                    NeedsFinalDecision = needsDecision,
                    Mentors = p.Mentors
                        .Where(m => m.IsActive)
                        .Select(m => new MentorSummaryDto
                        {
                            MentorId = m.MentorId,
                            MentorName = mentorNames.GetValueOrDefault(m.MentorId, "")
                        }).ToList(),
                    Evaluators = projectAssignments.Select(a => new EvaluatorAssignmentDto
                    {
                        AssignmentId = a.Id,
                        EvaluatorId = a.EvaluatorId,
                        EvaluatorName = evaluatorNames.GetValueOrDefault(a.EvaluatorId, ""),
                        EvaluatorOrder = a.EvaluatorOrder,
                        IndividualResult = a.IndividualResult?.ToString(),
                        IndividualResultValue = a.IndividualResult.HasValue ? (int?)a.IndividualResult.Value : null,
                        Feedback = a.Feedback,
                        EvaluatedAt = a.EvaluatedAt?.ToString("o"),
                        HasSubmitted = a.HasSubmittedEvaluation
                    }).ToList()
                };
            }).ToList();

            return new DepartmentProjectsResponse
            {
                Items = items,
                TotalCount = items.Count,
                PendingAssignmentCount = items.Count(i => i.AssignedEvaluatorCount < 2 && i.StatusValue == (int)ProjectStatus.PendingEvaluation),
                InEvaluationCount = items.Count(i => i.AssignedEvaluatorCount >= 2 && !i.NeedsFinalDecision && i.StatusValue == (int)ProjectStatus.PendingEvaluation),
                NeedsFinalDecisionCount = items.Count(i => i.NeedsFinalDecision),
                CompletedCount = items.Count(i => i.StatusValue != (int)ProjectStatus.PendingEvaluation)
            };
        }

        public async Task<List<DepartmentEvaluatorDto>> GetDepartmentEvaluatorsAsync(
            int departmentId,
            CancellationToken cancellationToken = default)
        {
            // Get users with Evaluator role in this department
            var evaluators = await _context.Users
                .Where(u => u.DepartmentId.HasValue &&
                           u.DepartmentId.Value == departmentId &&
                           u.Roles.Any(r => r.RoleName == DomainRoleNames.Evaluator && r.IsActive))
                .ToListAsync(cancellationToken);

            var evaluatorIds = evaluators.Select(e => e.Id).ToList();

            // Get active assignment counts
            var assignmentCounts = await _context.ProjectEvaluatorAssignments
                .Where(a => evaluatorIds.Contains(a.EvaluatorId) && a.IsActive &&
                            (!a.IndividualResult.HasValue || a.IndividualResult.Value == EvaluationResult.Pending))
                .GroupBy(a => a.EvaluatorId)
                .Select(g => new { EvaluatorId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EvaluatorId, x => x.Count, cancellationToken);

            return evaluators.Select(e => new DepartmentEvaluatorDto
            {
                UserId = e.Id,
                FullName = e.FullName,
                Email = e.Email.Value,
                AcademicTitle = e.AcademicTitle,
                ActiveAssignmentCount = assignmentCounts.GetValueOrDefault(e.Id, 0)
            }).OrderBy(e => e.FullName).ToList();
        }
    }
}
