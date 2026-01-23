using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Persistence.SqlServer.Identity;

namespace UniThesis.Persistence.SqlServer.Configurations.Identity
{
    /// <summary>
    /// EF Core configuration for ApplicationUserRole.
    /// </summary>
    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasIndex(ur => ur.AssignedAt);
        }
    }
}
