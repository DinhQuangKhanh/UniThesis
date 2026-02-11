using UniThesis.Domain.Common.Rules;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Rules
{
    /// <summary>
    /// Business rule that ensures a defense schedule has a council assigned before starting.
    /// </summary>
    public class DefenseMustHaveCouncilRule : IBusinessRule
    {
        private readonly int? _councilId;

        public DefenseMustHaveCouncilRule(int? councilId)
        {
            _councilId = councilId;
        }

        public string Message => "Defense must have a council assigned.";

        public bool IsBroken() => !_councilId.HasValue;
    }
}
