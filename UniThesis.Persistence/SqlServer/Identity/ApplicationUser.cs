using Microsoft.AspNetCore.Identity;
using UniThesis.Domain.Enums.User;

namespace UniThesis.Persistence.SqlServer.Identity
{
    /// <summary>
    /// Custom Identity User extending IdentityUser with additional properties.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the student code (for students).
        /// </summary>
        public string? StudentCode { get; set; }

        /// <summary>
        /// Gets or sets the employee code (for staff/mentors).
        /// </summary>
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the academic title (e.g., ThS., TS., PGS.TS.).
        /// </summary>
        public string? AcademicTitle { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the user status.
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Active;

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the last update timestamp.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last login timestamp.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for JWT authentication.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiry time.
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Navigation property for user roles.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        /// <summary>
        /// Checks if the user is a student.
        /// </summary>
        public bool IsStudent => !string.IsNullOrEmpty(StudentCode);

        /// <summary>
        /// Checks if the user is a staff member (mentor/admin).
        /// </summary>
        public bool IsStaff => !string.IsNullOrEmpty(EmployeeCode);

        /// <summary>
        /// Gets the display name (with academic title if available).
        /// </summary>
        public string DisplayName => string.IsNullOrEmpty(AcademicTitle)
            ? FullName
            : $"{AcademicTitle} {FullName}";

        /// <summary>
        /// Updates the last login timestamp.
        /// </summary>
        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets a new refresh token.
        /// </summary>
        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes the refresh token.
        /// </summary>
        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
