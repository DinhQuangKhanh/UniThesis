using UniThesis.Domain.Common.Rules;

namespace UniThesis.Domain.Aggregates.DefenseAggregate.Rules
{
    /// <summary>
    /// Business rule that ensures a defense council has a chairman assigned.
    /// </summary>
    public class CouncilMustHaveChairmanRule : IBusinessRule
    {
        private readonly Guid? _chairmanId;

        public CouncilMustHaveChairmanRule(Guid? chairmanId)
        {
            _chairmanId = chairmanId;
        }

        public string Message => "Defense council must have a chairman assigned.";

        public bool IsBroken() => !_chairmanId.HasValue;
    }
}
