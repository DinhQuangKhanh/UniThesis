using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Specifications.Semesters
{
    public class ActiveSemesterSpec : BaseSpecification<Semester>
    {
        public ActiveSemesterSpec()
            : base(s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
        {
            AddInclude(s => s.Phases);
        }
    }
}
