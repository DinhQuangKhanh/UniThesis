using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.DefenseAggregate;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.Defense
{
    /// <summary>
    /// EF Core configuration for DefenseSchedule aggregate root.
    /// </summary>
    public class DefenseScheduleConfiguration : IEntityTypeConfiguration<DefenseSchedule>
    {
        public void Configure(EntityTypeBuilder<DefenseSchedule> builder)
        {
            builder.ToTable("DefenseSchedules");

            builder.HasKey(d => d.Id);

            // Value Object conversions
            builder.Property(d => d.Location)
                .HasConversion<DefenseLocationConverter>()
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion<int>();

            builder.Property(d => d.Notes)
                .HasMaxLength(2000);

            // Relationships
            builder.HasOne(d => d.Council)
                .WithMany()
                .HasForeignKey(d => d.CouncilId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(d => d.GroupId);
            builder.HasIndex(d => d.CouncilId);
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.ScheduledDate);

            // Ignore domain events
            builder.Ignore(d => d.DomainEvents);
        }
    }
}
