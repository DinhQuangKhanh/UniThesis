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

        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var idList = ids.ToList();
            if (!idList.Any()) return Enumerable.Empty<User>();
            return await _context.Users
                .Where(u => idList.Contains(u.Id))
                .ToListAsync(ct);
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
    }
}
