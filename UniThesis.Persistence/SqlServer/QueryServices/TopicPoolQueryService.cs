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

        var totalTopics = await poolProjects.CountAsync(cancellationToken);

        var activeTopics = await poolProjects
            .CountAsync(p => p.PoolStatus == PoolTopicStatus.Available, cancellationToken);

        var registeredTopics = await poolProjects
            .CountAsync(p => p.PoolStatus == PoolTopicStatus.Reserved || p.PoolStatus == PoolTopicStatus.Assigned, cancellationToken);

        var expiredTopics = await poolProjects
            .CountAsync(p => p.PoolStatus == PoolTopicStatus.Expired, cancellationToken);

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
}
