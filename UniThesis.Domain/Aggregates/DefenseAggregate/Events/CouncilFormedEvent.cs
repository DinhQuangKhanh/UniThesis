using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Events
{
    public sealed record CouncilFormedEvent(int CouncilId, string CouncilName) : DomainEventBase;
}
