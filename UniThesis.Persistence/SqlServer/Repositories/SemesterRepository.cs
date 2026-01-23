using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Aggregates.SemesterAggregate.ValueObjects;
using UniThesis.Domain.Specifications.Semesters;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for Semester aggregate using specifications.
    /// </summary>
    public class SemesterRepository : BaseRepository<Semester, int>, ISemesterRepository
    {
        public SemesterRepository(AppDbContext context) : base(context) { }

        public override async Task<Semester?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Phases)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Semester?> GetByCodeAsync(SemesterCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Phases)
                .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
        }

        public async Task<Semester?> GetWithPhasesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Phases.OrderBy(p => p.Order))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Semester?> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            var spec = new ActiveSemesterSpec();
            return await FirstOrDefaultAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Semester>> GetByAcademicYearAsync(string academicYear, CancellationToken cancellationToken = default)
        {
            var spec = new SemesterByAcademicYearSpec(academicYear);
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<IEnumerable<Semester>> GetUpcomingAsync(CancellationToken cancellationToken = default)
        {
            var spec = new UpcomingSemestersSpec();
            return await ListAsync(spec, cancellationToken);
        }

        public async Task<bool> ExistsCodeAsync(SemesterCode code, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(s => s.Code == code, cancellationToken);
        }

        public async Task<int> GetNextIdAsync(CancellationToken cancellationToken = default)
        {
            var maxId = await _dbSet.MaxAsync(s => (int?)s.Id, cancellationToken);
            return (maxId ?? 0) + 1;
        }

        /// <summary>
        /// Gets all semesters with their phases.
        /// </summary>
        public override async Task<IEnumerable<Semester>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Phases)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the semester that is N semesters after the given semester.
        /// </summary>
        public async Task<Semester?> GetSemesterAfterAsync(int semesterId, int count, CancellationToken cancellationToken = default)
        {
            var currentSemester = await _dbSet.FindAsync(new object[] { semesterId }, cancellationToken);
            if (currentSemester == null) return null;

            return await _dbSet
                .Where(s => s.StartDate > currentSemester.EndDate)
                .OrderBy(s => s.StartDate)
                .Skip(count - 1)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
