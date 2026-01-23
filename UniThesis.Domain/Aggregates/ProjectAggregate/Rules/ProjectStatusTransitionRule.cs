using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Rules
{
    public class ProjectStatusTransitionRule : IBusinessRule
    {
        private readonly ProjectStatus _currentStatus;
        private readonly ProjectStatus _targetStatus;
        private readonly Dictionary<ProjectStatus, ProjectStatus[]> _validTransitions;

        public ProjectStatusTransitionRule(ProjectStatus currentStatus, ProjectStatus targetStatus)
        {
            _currentStatus = currentStatus;
            _targetStatus = targetStatus;
            _validTransitions = new Dictionary<ProjectStatus, ProjectStatus[]>
            {
                { ProjectStatus.Draft, new[] { ProjectStatus.PendingEvaluation, ProjectStatus.Cancelled } },
                { ProjectStatus.PendingEvaluation, new[] { ProjectStatus.Approved, ProjectStatus.NeedsModification, ProjectStatus.Rejected, ProjectStatus.Cancelled } },
                { ProjectStatus.NeedsModification, new[] { ProjectStatus.PendingEvaluation, ProjectStatus.Cancelled } },
                { ProjectStatus.Approved, new[] { ProjectStatus.InProgress, ProjectStatus.Cancelled } },
                { ProjectStatus.Rejected, new[] { ProjectStatus.Draft } },
                { ProjectStatus.InProgress, new[] { ProjectStatus.Completed, ProjectStatus.Cancelled } },
                { ProjectStatus.Completed, Array.Empty<ProjectStatus>() },
                { ProjectStatus.Cancelled, Array.Empty<ProjectStatus>() }
            };
        }

        public string Message => $"Cannot transition from {_currentStatus} to {_targetStatus}.";

        public bool IsBroken()
        {
            if (!_validTransitions.TryGetValue(_currentStatus, out var validTargets))
                return true;

            return !validTargets.Contains(_targetStatus);
        }
    }
}
