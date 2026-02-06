namespace UniThesis.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Repository interface for User aggregate.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Gets a user by their Firebase UID.
        /// </summary>
        Task<User?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default);

        /// <summary>
        /// Gets all users with a specific role.
        /// </summary>
        Task<IEnumerable<User>> GetByRoleAsync(string roleName, CancellationToken ct = default);

        /// <summary>
        /// Adds a new user.
        /// </summary>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        Task UpdateAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Checks if a user with the given email exists.
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Checks if a user with the given Firebase UID exists.
        /// </summary>
        Task<bool> ExistsByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default);
    }
}
