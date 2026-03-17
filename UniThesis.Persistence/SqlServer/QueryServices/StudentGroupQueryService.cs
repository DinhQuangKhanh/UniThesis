using Microsoft.EntityFrameworkCore;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.StudentGroups.DTOs;
using UniThesis.Domain.Enums.Mentor;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Persistence.SqlServer.QueryServices;

/// <summary>
/// EF Core implementation of complex StudentGroup read queries.
/// Uses direct DbContext access with AsNoTracking and Select projections for optimal performance.
/// </summary>
public class StudentGroupQueryService : IStudentGroupQueryService
{
    private readonly AppDbContext _context;

    public StudentGroupQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MentorGroupDto>> GetMentorGroupsAsync(
        Guid mentorId,
        int? semesterId,
        CancellationToken cancellationToken = default)
    {
        // If no semesterId provided, find the active semester
        var targetSemesterId = semesterId
            ?? await _context.Semesters
                .AsNoTracking()
                .Where(s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (targetSemesterId == 0)
            return [];

        // Single optimized query: ProjectMentors → Projects → Groups → Members → Users
        var groups = await (
            from pm in _context.ProjectMentors.AsNoTracking()
            where pm.MentorId == mentorId && pm.Status == ProjectMentorStatus.Active
            join p in _context.Projects on pm.ProjectId equals p.Id
            where p.SemesterId == targetSemesterId && p.GroupId != null
            join g in _context.Groups on p.GroupId equals g.Id
            select new MentorGroupDto
            {
                GroupId = g.Id,
                GroupCode = g.Code,
                GroupName = g.Name,
                GroupStatus = g.Status.ToString(),
                MaxMembers = g.MaxMembers,
                ProjectId = p.Id,
                ProjectName = p.NameVi,
                ProjectCode = p.Code,
                ProjectStatus = p.Status.ToString(),
                CreatedAt = g.CreatedAt,
                Members = (
                    from gm in _context.GroupMembers
                    where gm.GroupId == g.Id
                    join u in _context.Users on gm.StudentId equals u.Id
                    select new GroupMemberDto
                    {
                        StudentId = u.Id,
                        FullName = u.FullName,
                        StudentCode = u.StudentCode,
                        Email = u.Email,
                        Role = gm.Role.ToString(),
                        Status = gm.Status.ToString(),
                        JoinedAt = gm.JoinedAt
                    }
                ).ToList()
            }
        ).ToListAsync(cancellationToken);

        return groups;
    }
}
