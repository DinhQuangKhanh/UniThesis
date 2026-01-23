using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.Defense;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Entities
{
    public class CouncilMember : Entity<int>
    {
        public Guid MemberId { get; private set; }
        public CouncilMemberRole Role { get; private set; }

        private CouncilMember() { }

        internal static CouncilMember Create(Guid memberId, CouncilMemberRole role)
        {
            return new CouncilMember { MemberId = memberId, Role = role };
        }
    }
}
