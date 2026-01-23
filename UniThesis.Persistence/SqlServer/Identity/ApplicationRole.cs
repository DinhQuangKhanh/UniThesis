using Microsoft.AspNetCore.Identity;

namespace UniThesis.Persistence.SqlServer.Identity
{
    /// <summary>
    /// Custom Identity Role extending IdentityRole.
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets whether this role is a system role (cannot be deleted).
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Navigation property for user roles.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ApplicationRole() : base() { }

        /// <summary>
        /// Constructor with role name.
        /// </summary>
        public ApplicationRole(string roleName) : base(roleName) { }

        /// <summary>
        /// Constructor with role name and description.
        /// </summary>
        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            Description = description;
        }
    }
}
