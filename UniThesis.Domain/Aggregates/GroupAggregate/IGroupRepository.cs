using UniThesis.Domain.Aggregates.GroupAggregate.ValueObjects;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.GroupAggregate
{
    public interface IGroupRepository : IRepository<Group, Guid>
    {
        Task<Group?> GetByCodeAsync(GroupCode code, CancellationToken cancellationToken = default);
        Task<Group?> GetWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Group>> GetBySemesterIdAsync(int semesterId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Group>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<Group?> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<bool> ExistsCodeAsync(GroupCode code, CancellationToken cancellationToken = default);
        Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default);
        Task<bool> IsStudentInActiveGroupAsync(Guid studentId, int semesterId, CancellationToken cancellationToken = default);
        Task<bool> IsLeaderOfGroupAsync(Guid leaderId, Guid groupId, CancellationToken cancellationToken = default);
        Task<List<Guid>> GetActiveGroupIdsWithoutProjectAsync(int semesterId, CancellationToken cancellationToken = default);
        Task<Group?> GetWithInvitationsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Group?> GetWithJoinRequestsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Group?> GetWithAllRelationsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
