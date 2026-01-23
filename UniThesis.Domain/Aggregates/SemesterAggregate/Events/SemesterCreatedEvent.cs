using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record SemesterCreatedEvent(int SemesterId, string SemesterCode) : DomainEventBase;
}
