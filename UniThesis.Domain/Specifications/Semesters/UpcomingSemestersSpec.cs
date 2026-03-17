using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Domain.Specifications.Semesters
{
    public class UpcomingSemestersSpec : BaseSpecification<Semester>
    {
        public UpcomingSemestersSpec()
            : base(s => s.StartDate > DateTime.UtcNow)
        {
            ApplyOrderBy(s => s.StartDate);
        }
    }
}
