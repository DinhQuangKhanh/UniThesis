using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.TopicPool
{
    /// <summary>
    /// EF Core configuration for TopicPool aggregate root.
    /// </summary>
    public class TopicPoolConfiguration : IEntityTypeConfiguration<Domain.Aggregates.TopicPoolAggregate.TopicPool>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregates.TopicPoolAggregate.TopicPool> builder)
        {
            builder.ToTable("TopicPools");

            builder.HasKey(t => t.Id);

            // Value Object conversions
            builder.Property(t => t.Code)
                .HasConversion<TopicCodeConverter>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.NameVi)
                .HasMaxLength(350)
                .IsRequired();

            builder.Property(t => t.NameEn)
                .HasMaxLength(350);

            builder.Property(t => t.NameAbbr)
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(t => t.Objectives)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(t => t.Scope)
                .HasMaxLength(2000);

            builder.Property(t => t.Technologies)
                .HasMaxLength(500);

            builder.Property(t => t.ExpectedResults)
                .HasMaxLength(2000);

            builder.Property(t => t.Status)
                .HasConversion<int>();

            // Relationships
            builder.HasMany(t => t.Registrations)
                .WithOne()
                .HasForeignKey(r => r.TopicPoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(t => t.Code).IsUnique();
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.MajorId);
            builder.HasIndex(t => t.ProposedBy);
            builder.HasIndex(t => t.CreatedSemesterId);
            builder.HasIndex(t => t.ExpirationSemesterId);
            builder.HasIndex(t => t.SelectedByGroupId);

            // Ignore domain events
            builder.Ignore(t => t.DomainEvents);
        }
    }
}
