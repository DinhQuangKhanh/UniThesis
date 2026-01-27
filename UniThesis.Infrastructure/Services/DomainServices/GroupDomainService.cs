using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.GroupAggregate;
using UniThesis.Domain.Aggregates.ProjectAggregate;
using UniThesis.Domain.Enums.Group;
using UniThesis.Domain.Services;
using UniThesis.Persistence.SqlServer;

namespace UniThesis.Infrastructure.Services.DomainServices
{
    public class GroupDomainService : IGroupDomainService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly AppDbContext _context;

        public GroupDomainService(
            IGroupRepository groupRepository,
            IProjectRepository projectRepository,
            AppDbContext context)
        {
            _groupRepository = groupRepository;
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task<string> GenerateGroupCodeAsync(int year, CancellationToken ct = default)
        {
            var sequence = await _groupRepository.GetNextSequenceAsync(year, ct);
            return $"G-{year}-{sequence:D4}";
        }

        public async Task<(bool CanJoin, string? Reason)> CanStudentJoinGroupAsync(Guid studentId, int semesterId, CancellationToken ct = default)
        {
            var isInActiveGroup = await _groupRepository.IsStudentInActiveGroupAsync(studentId, semesterId, ct);
            if (isInActiveGroup)
                return (false, "Sinh viên đã tham gia một nhóm khác trong học kỳ này.");

            return (true, null);
        }

        public async Task<IEnumerable<Guid>> GetGroupsWithoutProjectAsync(int semesterId, CancellationToken ct = default)
        {
            var groups = await _context.Groups
                .Where(g => g.SemesterId == semesterId &&
                           g.Status == GroupStatus.Active &&
                           g.ProjectId == null)
                .Select(g => g.Id)
                .ToListAsync(ct);

            return groups;
        }
    }
}
