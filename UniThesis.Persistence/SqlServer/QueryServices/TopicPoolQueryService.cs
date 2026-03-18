using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;
using UniThesis.Domain.Enums.Mentor;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Persistence.SqlServer.QueryServices;

/// <summary>
/// EF Core implementation of complex TopicPool read queries.
/// Uses direct DbContext access with AsNoTracking and Select projections for optimal performance.
/// </summary>
public class TopicPoolQueryService : ITopicPoolQueryService
{
    private readonly AppDbContext _context;

    public TopicPoolQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TopicPoolDto>> GetTopicPoolsAsync(int? majorId, CancellationToken cancellationToken = default)
    {
        var query = _context.TopicPools.AsNoTracking();

        if (majorId.HasValue)
        {
            query = query.Where(tp => tp.MajorId == majorId.Value);
        }

        return await query
            .OrderBy(tp => tp.MajorId)
            .Select(tp => new TopicPoolDto
            {
                Id = tp.Id,
                Code = tp.Code,
                Name = tp.Name,
                Description = tp.Description,
                MajorId = tp.MajorId,
                Status = tp.Status,
                MaxActiveTopicsPerMentor = tp.MaxActiveTopicsPerMentor,
                ExpirationSemesters = tp.ExpirationSemesters,
                CreatedAt = tp.CreatedAt,
                UpdatedAt = tp.UpdatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DepartmentWithPoolsDto>> GetPoolsByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new
            {
                d.Id,
                d.Code,
                d.Name,
                Majors = _context.Majors
                    .Where(m => m.DepartmentId == d.Id && m.IsActive)
                    .OrderBy(m => m.Name)
                    .Select(m => new
                    {
                        m.Id,
                        m.Code,
                        m.Name,
                        Pool = _context.TopicPools
                            .Where(tp => tp.MajorId == m.Id)
                            .OrderByDescending(tp => tp.UpdatedAt ?? tp.CreatedAt)
                            .Select(tp => new
                            {
                                tp.Id,
                                tp.Code,
                                tp.Name,
                                tp.Status,
                                TotalTopics = _context.Projects.Count(p =>
                                    p.TopicPoolId == tp.Id &&
                                    p.SourceType == ProjectSourceType.FromPool)
                            })
                            .FirstOrDefault()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return departments.Select(d => new DepartmentWithPoolsDto
        {
            DepartmentId = d.Id,
            DepartmentCode = d.Code,
            DepartmentName = d.Name,
            Majors = d.Majors.Select(m => new MajorWithPoolDto
            {
                MajorId = m.Id,
                MajorCode = m.Code,
                MajorName = m.Name,
                Pool = m.Pool is null
                    ? null
                    : new TopicPoolSummaryDto
                    {
                        Id = m.Pool.Id,
                        Code = m.Pool.Code,
                        Name = m.Pool.Name,
                        StatusName = m.Pool.Status.ToString(),
                        TotalTopics = m.Pool.TotalTopics,
                    }
            }).ToList()
        }).ToList();
    }

    public async Task<TopicPoolDto?> GetTopicPoolByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TopicPools
            .AsNoTracking()
            .Where(tp => tp.Id == id)
            .Select(tp => new TopicPoolDto
            {
                Id = tp.Id,
                Code = tp.Code,
                Name = tp.Name,
                Description = tp.Description,
                MajorId = tp.MajorId,
                Status = tp.Status,
                MaxActiveTopicsPerMentor = tp.MaxActiveTopicsPerMentor,
                ExpirationSemesters = tp.ExpirationSemesters,
                CreatedAt = tp.CreatedAt,
                UpdatedAt = tp.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TopicPoolStatisticsDto> GetTopicPoolStatisticsAsync(Guid poolId, CancellationToken cancellationToken = default)
    {
        // Get pool info
        var pool = await _context.TopicPools
            .AsNoTracking()
            .Where(tp => tp.Id == poolId)
            .Select(tp => new { tp.Id, tp.Code, tp.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (pool is null)
        {
            return new TopicPoolStatisticsDto
            {
                PoolId = poolId,
                PoolCode = string.Empty,
                PoolName = string.Empty,
            };
        }

        // Query all projects in this pool
        var poolProjects = _context.Projects
            .AsNoTracking()
            .Where(p => p.TopicPoolId == poolId && p.SourceType == ProjectSourceType.FromPool);

        var statusCounts = await poolProjects
            .Where(p => p.PoolStatus.HasValue)
            .GroupBy(p => p.PoolStatus!.Value)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        var totalTopics = statusCounts.Values.Sum();
        var activeTopics = statusCounts.GetValueOrDefault(PoolTopicStatus.Available);
        var registeredTopics = statusCounts.GetValueOrDefault(PoolTopicStatus.Reserved)
                             + statusCounts.GetValueOrDefault(PoolTopicStatus.Assigned);
        var expiredTopics = statusCounts.GetValueOrDefault(PoolTopicStatus.Expired);

        var totalMentors = await poolProjects
            .SelectMany(p => p.Mentors.Where(m => m.Status == ProjectMentorStatus.Active))
            .Select(m => m.MentorId)
            .Distinct()
            .CountAsync(cancellationToken);

        return new TopicPoolStatisticsDto
        {
            PoolId = pool.Id,
            PoolCode = pool.Code,
            PoolName = pool.Name,
            TotalMentors = totalMentors,
            TotalTopicsCount = totalTopics,
            ActiveTopicsCount = activeTopics,
            RegisteredTopicsCount = registeredTopics,
            ExpiredTopicsCount = expiredTopics,
        };
    }

    public async Task<GetPoolTopicsResult> GetPoolTopicsAsync(
        int? majorId, string? search, int? poolStatus, string? sortBy,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Base query: projects from pool with major info (no 'let' subquery — avoids EF translation issues)
        var query = from p in _context.Projects.AsNoTracking()
                    where p.SourceType == ProjectSourceType.FromPool
                    join m in _context.Set<Domain.Entities.Major>() on p.MajorId equals m.Id
                    select new { Project = p, MajorName = m.Name, MajorCode = m.Code };

        // Filter by major
        if (majorId.HasValue)
            query = query.Where(x => x.Project.MajorId == majorId.Value);

        // Filter by pool status — enum comparison; EF handles string↔enum conversion
        if (poolStatus.HasValue)
        {
            var status = (PoolTopicStatus)poolStatus.Value;
            query = query.Where(x => x.Project.PoolStatus == status);
        }

        // Search in title and mentor name
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                EF.Property<string>(x.Project, "NameVi").Contains(term) ||
                EF.Property<string>(x.Project, "NameEn").Contains(term) ||
                x.Project.Mentors.Any(pm =>
                    pm.Status == ProjectMentorStatus.Active &&
                    _context.Users.Any(u => u.Id == pm.MentorId && u.FullName.Contains(term))));
        }

        // Sort — use EF.Property<string>() for value objects to avoid translation failures
        query = sortBy switch
        {
            "name" => query.OrderBy(x => EF.Property<string>(x.Project, "NameVi")),
            "mentor" => query.OrderBy(x =>
                x.Project.Mentors
                    .Where(pm => pm.Status == ProjectMentorStatus.Active)
                    .Join(_context.Users, pm => pm.MentorId, u => u.Id, (pm, u) => u.FullName)
                    .FirstOrDefault() ?? ""),
            _ => query.OrderByDescending(x => x.Project.CreatedAt) // "newest" default
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // 2-step projection: anonymous first to avoid (int) cast on nvarchar PoolStatus in SQL.
        // Mentor subquery is resolved here (only for the current page) instead of via 'let' clause.
        var rawItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Project.Id,
                Code = x.Project.Code.Value,
                NameVi = x.Project.NameVi.Value,
                NameEn = x.Project.NameEn.Value,
                x.Project.Description,
                Technologies = x.Project.Technologies != null ? x.Project.Technologies.Value : null,
                x.Project.MajorId,
                x.MajorName,
                x.MajorCode,
                x.Project.PoolStatus,
                x.Project.MaxStudents,
                MentorName = x.Project.Mentors
                    .Where(pm => pm.Status == ProjectMentorStatus.Active)
                    .Join(_context.Users, pm => pm.MentorId, u => u.Id, (pm, u) => u.FullName)
                    .FirstOrDefault() ?? "Chưa có mentor",
                MentorId = x.Project.Mentors
                    .Where(pm => pm.Status == ProjectMentorStatus.Active)
                    .Select(pm => pm.MentorId)
                    .FirstOrDefault(),
                x.Project.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        // Client-side: safe enum → int conversion after EF materialization
        var items = rawItems.Select(x => new PoolTopicItemDto
        {
            Id = x.Id,
            Code = x.Code,
            NameVi = x.NameVi,
            NameEn = x.NameEn,
            Description = x.Description,
            Technologies = x.Technologies,
            MajorId = x.MajorId,
            MajorName = x.MajorName,
            MajorCode = x.MajorCode,
            PoolStatus = x.PoolStatus.HasValue ? (int)x.PoolStatus.Value : 0,
            PoolStatusName = x.PoolStatus.HasValue ? x.PoolStatus.Value.ToString() : "Unknown",
            MaxStudents = x.MaxStudents,
            MentorName = x.MentorName,
            MentorId = x.MentorId,
            CreatedAt = x.CreatedAt,
        }).ToList();

        return new GetPoolTopicsResult(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<PoolTopicDetailDto?> GetPoolTopicDetailAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        // 2-step projection to avoid (int) cast on nvarchar PoolStatus in SQL
        var raw = await (
            from p in _context.Projects.AsNoTracking()
            where p.Id == projectId && p.SourceType == ProjectSourceType.FromPool
            join m in _context.Set<Domain.Entities.Major>() on p.MajorId equals m.Id
            select new
            {
                p.Id,
                Code = p.Code.Value,
                NameVi = p.NameVi.Value,
                NameEn = p.NameEn.Value,
                p.NameAbbr,
                p.Description,
                p.Objectives,
                p.Scope,
                Technologies = p.Technologies != null ? p.Technologies.Value : null,
                p.ExpectedResults,
                p.MajorId,
                MajorName = m.Name,
                MajorCode = m.Code,
                p.PoolStatus,
                p.MaxStudents,
                Mentors = p.Mentors
                    .Where(pm => pm.Status == ProjectMentorStatus.Active)
                    .Join(_context.Users, pm => pm.MentorId, u => u.Id, (pm, u) => new MentorSummaryDto
                    {
                        MentorId = pm.MentorId,
                        FullName = u.FullName,
                    })
                    .ToList(),
                p.CreatedAt,
                p.UpdatedAt,
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (raw is null) return null;

        // Client-side: safe enum → int conversion
        return new PoolTopicDetailDto
        {
            Id = raw.Id,
            Code = raw.Code,
            NameVi = raw.NameVi,
            NameEn = raw.NameEn,
            NameAbbr = raw.NameAbbr,
            Description = raw.Description,
            Objectives = raw.Objectives,
            Scope = raw.Scope,
            Technologies = raw.Technologies,
            ExpectedResults = raw.ExpectedResults,
            MajorId = raw.MajorId,
            MajorName = raw.MajorName,
            MajorCode = raw.MajorCode,
            PoolStatus = raw.PoolStatus.HasValue ? (int)raw.PoolStatus.Value : 0,
            PoolStatusName = raw.PoolStatus.HasValue ? raw.PoolStatus.Value.ToString() : "Unknown",
            MaxStudents = raw.MaxStudents,
            Mentors = raw.Mentors,
            CreatedAt = raw.CreatedAt,
            UpdatedAt = raw.UpdatedAt,
        };
    }
}
