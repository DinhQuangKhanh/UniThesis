using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record SemesterActivatedEvent(int SemesterId) : DomainEventBase;
}
