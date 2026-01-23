using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Persistence.SqlServer.Configurations.Identity
{
    /// <summary>
    /// EF Core configuration for ApplicationUser.
    /// </summary>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(u => u.StudentCode)
                .HasMaxLength(20);

            builder.Property(u => u.EmployeeCode)
                .HasMaxLength(20);

            builder.Property(u => u.AcademicTitle)
                .HasMaxLength(50);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(u => u.StudentCode)
                .IsUnique()
                .HasFilter("[StudentCode] IS NOT NULL");

            builder.HasIndex(u => u.EmployeeCode)
                .IsUnique()
                .HasFilter("[EmployeeCode] IS NOT NULL");

            builder.HasIndex(u => u.Status);

            builder.HasIndex(u => u.DepartmentId);

            // Relationships
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
