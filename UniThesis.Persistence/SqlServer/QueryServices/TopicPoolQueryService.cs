using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;
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

        // Count by PoolStatus
        var statusCounts = await poolProjects
            .GroupBy(p => p.PoolStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalTopics = statusCounts.Sum(x => x.Count);
        var activeCount = statusCounts
            .Where(x => x.Status == PoolTopicStatus.Available)
            .Sum(x => x.Count);
        var registeredCount = statusCounts
            .Where(x => x.Status == PoolTopicStatus.Reserved || x.Status == PoolTopicStatus.Assigned)
            .Sum(x => x.Count);
        var expiredCount = statusCounts
            .Where(x => x.Status == PoolTopicStatus.Expired)
            .Sum(x => x.Count);

        // Count distinct active mentors across all projects in this pool
        var totalMentors = await poolProjects
            .SelectMany(p => p.Mentors.Where(m => m.IsActive))
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
            ActiveTopicsCount = activeCount,
            RegisteredTopicsCount = registeredCount,
            ExpiredTopicsCount = expiredCount,
        };
    }
}
