using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record PhaseStartedEvent(int SemesterId, int PhaseId, SemesterPhaseType PhaseType) : DomainEventBase;
}
