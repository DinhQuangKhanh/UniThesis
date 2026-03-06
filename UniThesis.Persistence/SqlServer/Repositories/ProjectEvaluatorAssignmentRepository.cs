using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Aggregates.EvaluationAggregate.Entities;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository for ProjectEvaluatorAssignment entity.
    /// </summary>
    public class ProjectEvaluatorAssignmentRepository : IProjectEvaluatorAssignmentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<ProjectEvaluatorAssignment> _dbSet;

        public ProjectEvaluatorAssignmentRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<ProjectEvaluatorAssignment>();
        }

        public async Task AddAsync(ProjectEvaluatorAssignment assignment, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(assignment, cancellationToken);
        }

        public async Task<int> GetActiveCountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(a => a.ProjectId == projectId && a.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<ProjectEvaluatorAssignment>> GetActiveByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(a => a.ProjectId == projectId && a.IsActive)
                .OrderBy(a => a.EvaluatorOrder)
                .ToListAsync(cancellationToken);
        }
    }
}
