using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.Support
{
    /// <summary>
    /// EF Core configuration for SupportTicket aggregate root.
    /// </summary>
    public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("SupportTickets");

            builder.HasKey(t => t.Id);

            // Value Object conversions
            builder.Property(t => t.Code)
                .HasConversion<TicketCodeConverter>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(t => t.Category)
                .HasConversion<int>();

            builder.Property(t => t.Priority)
                .HasConversion<int>();

            builder.Property(t => t.Status)
                .HasConversion<int>();

            // Indexes
            builder.HasIndex(t => t.Code).IsUnique();
            builder.HasIndex(t => t.ReporterId);
            builder.HasIndex(t => t.AssigneeId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.Category);
            builder.HasIndex(t => t.Priority);
            builder.HasIndex(t => t.CreatedAt);

            // Foreign key to User (Reporter)
            builder.HasOne<Domain.Aggregates.UserAggregate.User>()
                .WithMany()
                .HasForeignKey(t => t.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key to User (Assignee)
            builder.HasOne<Domain.Aggregates.UserAggregate.User>()
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Ignore domain events
            builder.Ignore(t => t.DomainEvents);
        }
    }
}
