using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Evaluations.DTOs;
using UniThesis.Domain.Enums.Evaluation;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Persistence.SqlServer.QueryServices;

public class EvaluatorQueryService : IEvaluatorQueryService
{
    private readonly AppDbContext _context;

    public EvaluatorQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EvaluatorDashboardDto> GetDashboardAsync(Guid evaluatorId, CancellationToken cancellationToken = default)
    {
        // Get all active assignments for this evaluator
        var assignments = await _context.ProjectEvaluatorAssignments
            .AsNoTracking()
            .Where(a => a.EvaluatorId == evaluatorId && a.IsActive)
            .ToListAsync(cancellationToken);

        var totalAssigned = assignments.Count;
        var pendingAssignments = assignments.Where(a => a.IndividualResult == null || a.IndividualResult == EvaluationResult.Pending).ToList();
        var completedAssignments = assignments.Where(a => a.IndividualResult.HasValue && a.IndividualResult != EvaluationResult.Pending).ToList();

        var approvedCount = completedAssignments.Count(a => a.IndividualResult == EvaluationResult.Approved);
        var rejectedCount = completedAssignments.Count(a => a.IndividualResult == EvaluationResult.Rejected);
        var needsModCount = completedAssignments.Count(a => a.IndividualResult == EvaluationResult.NeedsModification);

        double? avgReviewDays = null;
        var withDuration = completedAssignments
            .Where(a => a.EvaluatedAt.HasValue)
            .Select(a => (a.EvaluatedAt!.Value - a.AssignedAt).TotalDays)
            .ToList();
        if (withDuration.Count > 0)
            avgReviewDays = Math.Round(withDuration.Average(), 1);

        var stats = new EvaluatorStatsDto
        {
            TotalAssigned = totalAssigned,
            PendingCount = pendingAssignments.Count,
            ApprovedCount = approvedCount,
            RejectedCount = rejectedCount,
            NeedsModificationCount = needsModCount,
            ReviewedCount = completedAssignments.Count,
            AvgReviewDays = avgReviewDays
        };

        // Get pending evaluations with project details
        var pendingProjectIds = pendingAssignments.Select(a => a.ProjectId).Distinct().ToList();

        var pendingEvaluations = await (
            from a in _context.ProjectEvaluatorAssignments.AsNoTracking()
            where a.EvaluatorId == evaluatorId && a.IsActive
                  && (a.IndividualResult == null || a.IndividualResult == EvaluationResult.Pending)
            join p in _context.Projects.AsNoTracking() on a.ProjectId equals p.Id
            join m in _context.Set<UniThesis.Domain.Entities.Major>().AsNoTracking() on p.MajorId equals m.Id into majors
            from m in majors.DefaultIfEmpty()
            select new
            {
                a.Id,
                a.ProjectId,
                ProjectCode = p.Code,
                ProjectNameVi = p.NameVi,
                MajorName = m != null ? m.Name : "",
                p.GroupId,
                a.AssignedAt
            }
        ).ToListAsync(cancellationToken);

        // Get student names for each project's group
        var groupIds = pendingEvaluations.Where(pe => pe.GroupId != null).Select(pe => pe.GroupId!.Value).Distinct().ToList();

        var groupLeaders = await (
            from gm in _context.GroupMembers.AsNoTracking()
            where groupIds.Contains(gm.GroupId) && gm.Role == Domain.Enums.Group.GroupMemberRole.Leader
            join u in _context.Users.AsNoTracking() on gm.StudentId equals u.Id
            select new { gm.GroupId, u.FullName, u.AvatarUrl }
        ).ToDictionaryAsync(x => x.GroupId, x => new { x.FullName, x.AvatarUrl }, cancellationToken);

        var now = DateTime.UtcNow;
        var pendingDtos = pendingEvaluations.Select(pe =>
        {
            var studentName = "";
            string? studentAvatar = null;
            if (pe.GroupId != null && groupLeaders.TryGetValue(pe.GroupId.Value, out var leader))
            {
                studentName = leader.FullName;
                studentAvatar = leader.AvatarUrl;
            }

            return new PendingEvaluationDto
            {
                AssignmentId = pe.Id,
                ProjectId = pe.ProjectId,
                ProjectCode = pe.ProjectCode,
                ProjectNameVi = pe.ProjectNameVi,
                MajorName = pe.MajorName,
                StudentName = studentName,
                StudentAvatar = studentAvatar,
                AssignedAt = pe.AssignedAt,
                IsUrgent = (now - pe.AssignedAt).TotalDays > 5
            };
        }).OrderByDescending(pe => pe.IsUrgent).ThenBy(pe => pe.AssignedAt).ToList();

        // Get recently reviewed projects
        var recentReviewed = await (
            from a in _context.ProjectEvaluatorAssignments.AsNoTracking()
            where a.EvaluatorId == evaluatorId && a.IsActive
                  && a.IndividualResult.HasValue && a.IndividualResult != EvaluationResult.Pending
                  && a.EvaluatedAt.HasValue
            join p in _context.Projects.AsNoTracking() on a.ProjectId equals p.Id
            orderby a.EvaluatedAt descending
            select new RecentReviewedDto
            {
                ProjectId = a.ProjectId,
                ProjectNameVi = p.NameVi,
                Result = a.IndividualResult.ToString()!,
                EvaluatedAt = a.EvaluatedAt!.Value
            }
        ).Take(5).ToListAsync(cancellationToken);

        return new EvaluatorDashboardDto
        {
            Stats = stats,
            PendingEvaluations = pendingDtos,
            RecentReviewed = recentReviewed
        };
    }

    public async Task<EvaluatorHistoryDto> GetHistoryAsync(
        Guid evaluatorId,
        int page,
        int pageSize,
        string? search,
        string? result,
        string? dateRange,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query =
            from a in _context.ProjectEvaluatorAssignments.AsNoTracking()
            where a.EvaluatorId == evaluatorId
                  && a.IsActive
                  && a.IndividualResult.HasValue
                  && a.IndividualResult != EvaluationResult.Pending
                  && a.EvaluatedAt.HasValue
            join p in _context.Projects.AsNoTracking() on a.ProjectId equals p.Id
            select new { a, p };

        if (!string.IsNullOrEmpty(dateRange))
        {
            var startOfWeek = now.AddDays(-((int)now.DayOfWeek == 0 ? 6 : (int)now.DayOfWeek - 1)).Date;
            var yesterday = now.AddDays(-1).Date;

            query = dateRange switch
            {
                "thisMonth" => query.Where(x =>
                    x.a.EvaluatedAt!.Value.Month == now.Month &&
                    x.a.EvaluatedAt.Value.Year == now.Year),
                "thisWeek" => query.Where(x => x.a.EvaluatedAt!.Value >= startOfWeek),
                "yesterday" => query.Where(x =>
                    x.a.EvaluatedAt!.Value >= yesterday &&
                    x.a.EvaluatedAt.Value < yesterday.AddDays(1)),
                _ => query
            };
        }

        if (!string.IsNullOrEmpty(result) && Enum.TryParse<EvaluationResult>(result, true, out var resultEnum))
        {
            query = query.Where(x => x.a.IndividualResult == resultEnum);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                x.p.NameVi.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                x.p.Code.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rawItems = await query
            .OrderByDescending(x => x.a.EvaluatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.a.ProjectId,
                ProjectCode = x.p.Code,
                ProjectNameVi = x.p.NameVi,
                x.p.GroupId,
                EvaluatedAt = x.a.EvaluatedAt!.Value,
                Result = x.a.IndividualResult!.Value.ToString(),
                x.a.Feedback
            })
            .ToListAsync(cancellationToken);

        var groupIds = rawItems
            .Where(x => x.GroupId != null)
            .Select(x => x.GroupId!.Value)
            .Distinct()
            .ToList();

        var groupLeaderMap = new Dictionary<Guid, (string FullName, string? AvatarUrl)>();
        if (groupIds.Count > 0)
        {
            var leaders = await (
                from gm in _context.GroupMembers.AsNoTracking()
                where groupIds.Contains(gm.GroupId) && gm.Role == Domain.Enums.Group.GroupMemberRole.Leader
                join u in _context.Users.AsNoTracking() on gm.StudentId equals u.Id
                select new { gm.GroupId, u.FullName, u.AvatarUrl }
            ).ToListAsync(cancellationToken);

            groupLeaderMap = leaders.ToDictionary(x => x.GroupId, x => (x.FullName, x.AvatarUrl));
        }

        var items = rawItems.Select(x =>
        {
            var studentName = "";
            string? studentAvatar = null;
            if (x.GroupId != null && groupLeaderMap.TryGetValue(x.GroupId.Value, out var leader))
            {
                studentName = leader.FullName;
                studentAvatar = leader.AvatarUrl;
            }

            return new EvaluatorHistoryItemDto
            {
                ProjectId = x.ProjectId,
                ProjectCode = x.ProjectCode,
                ProjectNameVi = x.ProjectNameVi,
                StudentName = studentName,
                StudentAvatar = studentAvatar,
                EvaluatedAt = x.EvaluatedAt,
                Result = x.Result,
                Feedback = x.Feedback
            };
        }).ToList();

        // Overall stats (not filtered)
        var statsBase = _context.ProjectEvaluatorAssignments
            .AsNoTracking()
            .Where(a => a.EvaluatorId == evaluatorId
                        && a.IsActive
                        && a.IndividualResult.HasValue
                        && a.IndividualResult != EvaluationResult.Pending);

        var totalReviewed = await statsBase.CountAsync(cancellationToken);
        var approvedCount = await statsBase.CountAsync(a => a.IndividualResult == EvaluationResult.Approved, cancellationToken);
        var needsModCount = await statsBase.CountAsync(a => a.IndividualResult == EvaluationResult.NeedsModification, cancellationToken);
        var rejectedCount = await statsBase.CountAsync(a => a.IndividualResult == EvaluationResult.Rejected, cancellationToken);

        var historyStats = new EvaluatorHistoryStatsDto
        {
            TotalReviewed = totalReviewed,
            ApprovedCount = approvedCount,
            NeedsModificationCount = needsModCount,
            RejectedCount = rejectedCount,
        };

        return new EvaluatorHistoryDto
        {
            Stats = historyStats,
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
