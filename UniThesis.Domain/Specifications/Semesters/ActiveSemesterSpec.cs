using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Specifications.Semesters
{
    public class ActiveSemesterSpec : BaseSpecification<Semester>
    {
        public ActiveSemesterSpec()
            : base(s => s.Status == SemesterStatus.Active)
        {
            AddInclude(s => s.Phases);
        }
    }
}
