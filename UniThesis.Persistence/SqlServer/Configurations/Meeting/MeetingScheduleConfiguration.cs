using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.MeetingAggregate;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.Meeting
{
    /// <summary>
    /// EF Core configuration for MeetingSchedule aggregate root.
    /// </summary>
    public class MeetingScheduleConfiguration : IEntityTypeConfiguration<MeetingSchedule>
    {
        public void Configure(EntityTypeBuilder<MeetingSchedule> builder)
        {
            builder.ToTable("MeetingSchedules");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            // Value Object conversions
            builder.Property(m => m.Location)
                .HasConversion<MeetingLocationConverter>()
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(m => m.Status)
                .HasConversion<int>();

            builder.Property(m => m.Notes)
                .HasMaxLength(2000);

            builder.Property(m => m.RejectionReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(m => m.GroupId);
            builder.HasIndex(m => m.MentorId);
            builder.HasIndex(m => m.Status);
            builder.HasIndex(m => m.ScheduledDate);
            builder.HasIndex(m => m.RequestedBy);

            // Ignore domain events
            builder.Ignore(m => m.DomainEvents);
        }
    }
}
