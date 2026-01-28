using Microsoft.AspNetCore.Identity;

namespace UniThesis.Persistence.SqlServer.Identity;

/// <summary>
/// Custom Identity Role extending IdentityRole with permission management.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the permissions as a JSON string.
    /// Example: ["users.read", "users.write", "projects.manage"]
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether this role is a system role (cannot be deleted).
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Gets or sets whether this role is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

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

    /// <summary>
    /// Constructor with role name, description, and permissions.
    /// </summary>
    public ApplicationRole(string roleName, string description, string? permissions) : base(roleName)
    {
        Description = description;
        Permissions = permissions;
    }

    /// <summary>
    /// Updates the role information.
    /// </summary>
    public void Update(string name, string? description, string? permissions)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        Permissions = permissions;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the permissions for this role.
    /// </summary>
    public void SetPermissions(string? permissions)
    {
        Permissions = permissions;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the role.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the role.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
