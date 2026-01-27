using UniThesis.Domain.Common.Rules;
using UniThesis.Domain.Enums.TopicPool;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Rules
{
    /// <summary>
    /// Rule: Topic pool must be active to accept new proposals.
    /// </summary>
    public class TopicPoolMustBeActiveRule : IBusinessRule
    {
        private readonly TopicPoolStatus _status;

        public TopicPoolMustBeActiveRule(TopicPoolStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status != TopicPoolStatus.Active;

        public string Message => "Topic pool is not active. Cannot accept new proposals or registrations.";
    }
}
