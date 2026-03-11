using Microsoft.EntityFrameworkCore;
using UniThesis.Domain.Aggregates.UserAggregate;
using UniThesis.Persistence.Common;

namespace UniThesis.Persistence.SqlServer.Repositories
{
    /// <summary>
    /// Repository implementation for User aggregate.
    /// </summary>
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _dbSet
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => EF.Property<string>(u, "Email") == normalizedEmail, ct);
        }

        /// <inheritdoc/>
        public async Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, ct);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetByRoleAsync(string roleName, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.RoleName == roleName && r.IsActive))
                .ToListAsync(ct);
        }

        /// <inheritdoc/>
        public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Synchronous EF operation. Returns completed task without async state machine overhead.
        /// </remarks>
        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _dbSet.Update(user);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _dbSet.AnyAsync(u => EF.Property<string>(u, "Email") == normalizedEmail, ct);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(u => u.FirebaseUid == firebaseUid, ct);
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            string? role, string? search, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _dbSet.Include(u => u.Roles).AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Roles.Any(r => r.RoleName == role && r.IsActive));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLowerInvariant();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(term) ||
                    EF.Property<string>(u, "Email").Contains(term) ||
                    (u.StudentCode != null && u.StudentCode.ToLower().Contains(term)) ||
                    (u.EmployeeCode != null && u.EmployeeCode.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
