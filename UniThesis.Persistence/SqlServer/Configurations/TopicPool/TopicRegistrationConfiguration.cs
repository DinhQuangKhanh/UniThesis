using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.TopicPoolAggregate.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.TopicPool
{
    /// <summary>
    /// EF Core configuration for TopicRegistration entity.
    /// </summary>
    public class TopicRegistrationConfiguration : IEntityTypeConfiguration<TopicRegistration>
    {
        public void Configure(EntityTypeBuilder<TopicRegistration> builder)
        {
            builder.ToTable("TopicRegistrations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .HasConversion<int>();

            builder.Property(r => r.CancelledReason)
                .HasMaxLength(500);

            builder.Property(r => r.Notes)
                .HasMaxLength(1000);

            // Indexes
            builder.HasIndex(r => r.TopicPoolId);
            builder.HasIndex(r => r.GroupId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => new { r.TopicPoolId, r.GroupId, r.Status });
        }
    }
}
