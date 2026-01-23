using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record PhaseCompletedEvent(int SemesterId, int PhaseId, SemesterPhaseType PhaseType) : DomainEventBase;
}
