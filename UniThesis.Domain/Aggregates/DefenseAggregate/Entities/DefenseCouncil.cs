using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Defense;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Entities
{
    public class DefenseCouncil : Entity<int>
    {
        private readonly List<CouncilMember> _members = [];

        public string Name { get; private set; } = string.Empty;
        public Guid ChairmanId { get; private set; }
        public Guid SecretaryId { get; private set; }
        public int SemesterId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public IReadOnlyCollection<CouncilMember> Members => _members.AsReadOnly();

        private DefenseCouncil() { }

        internal static DefenseCouncil Create(string name, Guid chairmanId, Guid secretaryId, int semesterId)
        {
            var council = new DefenseCouncil
            {
                Name = name,
                ChairmanId = chairmanId,
                SecretaryId = secretaryId,
                SemesterId = semesterId,
                CreatedAt = DateTime.UtcNow
            };

            council._members.Add(CouncilMember.Create(chairmanId, CouncilMemberRole.Chairman));
            council._members.Add(CouncilMember.Create(secretaryId, CouncilMemberRole.Secretary));

            return council;
        }

        public void AddMember(Guid memberId)
        {
            if (_members.Any(m => m.MemberId == memberId))
                throw new BusinessRuleValidationException("Member already exists in council.");
            _members.Add(CouncilMember.Create(memberId, CouncilMemberRole.Member));
        }
    }
}
