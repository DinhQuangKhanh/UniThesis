using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Aggregates.SemesterAggregate.Events
{
    public sealed record PhaseUpcomingEvent(int SemesterId, int PhaseId, SemesterPhaseType PhaseType) : DomainEventBase;
}
