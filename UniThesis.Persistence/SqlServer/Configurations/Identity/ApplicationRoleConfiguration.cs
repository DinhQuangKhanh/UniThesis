using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Persistence.SqlServer.Constants;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Persistence.SqlServer.Configurations.Identity
{
    /// <summary>
    /// EF Core configuration for ApplicationRole.
    /// </summary>
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.Property(r => r.Description)
                .HasMaxLength(500);

            // Seed default roles
            builder.HasData(
                new ApplicationRole
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = RoleNames.Admin,
                    NormalizedName = RoleNames.Admin.ToUpperInvariant(),
                    Description = "System Administrator",
                    IsSystemRole = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = RoleNames.Mentor,
                    NormalizedName = RoleNames.Mentor.ToUpperInvariant(),
                    Description = "Thesis Mentor/Supervisor",
                    IsSystemRole = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = RoleNames.Student,
                    NormalizedName = RoleNames.Student.ToUpperInvariant(),
                    Description = "Student",
                    IsSystemRole = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = RoleNames.Evaluator,
                    NormalizedName = RoleNames.Evaluator.ToUpperInvariant(),
                    Description = "Project Evaluator",
                    IsSystemRole = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = RoleNames.DepartmentHead,
                    NormalizedName = RoleNames.DepartmentHead.ToUpperInvariant(),
                    Description = "Department Head",
                    IsSystemRole = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Relationships
            builder.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
