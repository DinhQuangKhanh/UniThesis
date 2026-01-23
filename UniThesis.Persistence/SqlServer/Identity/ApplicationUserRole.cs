using Microsoft.AspNetCore.Identity;

namespace UniThesis.Persistence.SqlServer.Identity
{
    /// <summary>
    /// Custom Identity UserRole for many-to-many relationship with navigation properties.
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        /// <summary>
        /// Gets or sets the assignment timestamp.
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user who assigned this role.
        /// </summary>
        public Guid? AssignedBy { get; set; }

        /// <summary>
        /// Navigation property for the user.
        /// </summary>
        public virtual ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Navigation property for the role.
        /// </summary>
        public virtual ApplicationRole Role { get; set; } = null!;
    }
}
