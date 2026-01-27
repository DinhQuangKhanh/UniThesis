using UniThesis.Domain.Aggregates.GroupAggregate.Entities;
using UniThesis.Domain.Aggregates.GroupAggregate.Events;
using UniThesis.Domain.Aggregates.GroupAggregate.Rules;
using UniThesis.Domain.Aggregates.GroupAggregate.ValueObjects;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.Group;

namespace UniThesis.Domain.Aggregates.GroupAggregate
{
    public class Group : AggregateRoot<Guid>
    {
        private readonly List<GroupMember> _members = new();
        private const int DefaultMaxMembers = 5;

        public GroupCode Code { get; private set; } = null!;
        public string? Name { get; private set; }
        public Guid? ProjectId { get; private set; }
        public int SemesterId { get; private set; }
        public Guid? LeaderId { get; private set; }
        public GroupStatus Status { get; private set; }
        public int MaxMembers { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();
        public IReadOnlyCollection<GroupMember> ActiveMembers => _members.Where(m => m.IsActive).ToList().AsReadOnly();
        public GroupMember? Leader => _members.FirstOrDefault(m => m.IsLeader);

        private Group() { }

        public static Group Create(GroupCode code, int semesterId, Guid leaderId, string? name = null, int maxMembers = DefaultMaxMembers)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                SemesterId = semesterId,
                LeaderId = leaderId,
                Status = GroupStatus.Active,
                MaxMembers = maxMembers,
                CreatedAt = DateTime.UtcNow
            };

            var leaderMember = GroupMember.Create(group.Id, leaderId, GroupMemberRole.Leader);
            group._members.Add(leaderMember);

            group.RaiseDomainEvent(new GroupCreatedEvent(group.Id, group.Code.Value, semesterId));
            return group;
        }

        public void AddMember(Guid studentId)
        {
            CheckRule(new GroupCannotExceedMaxMembersRule(ActiveMembers.Count, MaxMembers));
            CheckRule(new StudentCannotJoinMultipleGroupsRule(_members.Any(m => m.StudentId == studentId && m.IsActive)));

            var member = GroupMember.Create(Id, studentId, GroupMemberRole.Member);
            _members.Add(member);
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MemberAddedEvent(Id, studentId, GroupMemberRole.Member));
        }

        public void RemoveMember(Guid studentId, Guid removedBy)
        {
            var member = _members.FirstOrDefault(m => m.StudentId == studentId && m.IsActive)
                ?? throw new EntityNotFoundException(nameof(GroupMember), studentId);

            if (member.IsLeader)
                throw new BusinessRuleValidationException("Cannot remove the leader. Assign a new leader first.");

            member.Leave();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new MemberRemovedEvent(Id, studentId, removedBy));
        }

        public void ChangeLeader(Guid newLeaderId)
        {
            var newLeader = _members.FirstOrDefault(m => m.StudentId == newLeaderId && m.IsActive)
                ?? throw new BusinessRuleValidationException("New leader must be an active member of the group.");

            var oldLeader = Leader;
            oldLeader?.DemoteToMember();
            newLeader.PromoteToLeader();
            LeaderId = newLeaderId;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new LeaderChangedEvent(Id, oldLeader?.StudentId, newLeaderId));
        }

        public void AssignProject(Guid projectId)
        {
            ProjectId = projectId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Disband()
        {
            if (Status != GroupStatus.Active)
                throw new BusinessRuleValidationException("Only active groups can be disbanded.");
            Status = GroupStatus.Disbanded;
            foreach (var member in ActiveMembers) member.Leave();
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new GroupDisbandedEvent(Id));
        }

        public void Complete()
        {
            if (Status != GroupStatus.Active)
                throw new BusinessRuleValidationException("Only active groups can be completed.");
            Status = GroupStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new GroupCompletedEvent(Id));
        }

        public void SetName(string? name) { Name = name; UpdatedAt = DateTime.UtcNow; }
        public void SetMaxMembers(int maxMembers)
        {
            if (maxMembers < ActiveMembers.Count)
                throw new BusinessRuleValidationException($"Cannot set max members to {maxMembers} when group has {ActiveMembers.Count} active members.");
            MaxMembers = maxMembers;
            UpdatedAt = DateTime.UtcNow;
        }

        private void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }
    }
}
