using UniThesis.Domain.Common.Rules;

namespace UniThesis.Domain.Aggregates.TopicPoolAggregate.Rules
{
    public class TopicCannotBeSelectedAfterExpirationRule : IBusinessRule
    {
        private readonly int _currentSemesterId;
        private readonly int _expirationSemesterId;
        public TopicCannotBeSelectedAfterExpirationRule(int currentSemesterId, int expirationSemesterId)
        {
            _currentSemesterId = currentSemesterId;
            _expirationSemesterId = expirationSemesterId;
        }
        public string Message => "Topic has expired and cannot be selected.";
        public bool IsBroken() => _currentSemesterId > _expirationSemesterId;
    }
}
