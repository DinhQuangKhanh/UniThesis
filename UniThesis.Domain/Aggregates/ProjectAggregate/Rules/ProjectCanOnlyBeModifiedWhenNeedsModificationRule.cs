using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.Project;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Rules
{
    public class ProjectCanOnlyBeModifiedWhenNeedsModificationRule : IBusinessRule
    {
        private readonly ProjectStatus _currentStatus;

        public ProjectCanOnlyBeModifiedWhenNeedsModificationRule(ProjectStatus currentStatus)
        {
            _currentStatus = currentStatus;
        }

        public string Message => "Project can only be modified when in NeedsModification status.";

        public bool IsBroken() => _currentStatus != ProjectStatus.NeedsModification;
    }
}
