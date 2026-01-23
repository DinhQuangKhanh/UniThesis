using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Specifications.Semesters
{
    public class UpcomingSemestersSpec : BaseSpecification<Semester>
    {
        public UpcomingSemestersSpec()
            : base(s => s.Status == SemesterStatus.Upcoming)
        {
            ApplyOrderBy(s => s.StartDate);
        }
    }
}
