using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record SemesterClosedEvent(int SemesterId) : DomainEventBase;
}
