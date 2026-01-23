using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Rules
{
    public class TopicMustBeAvailableForSelectionRule : IBusinessRule
    {
        private readonly TopicPoolStatus _status;
        public TopicMustBeAvailableForSelectionRule(TopicPoolStatus status) => _status = status;
        public string Message => "Topic is not available for selection.";
        public bool IsBroken() => _status != TopicPoolStatus.Available;
    }
}
