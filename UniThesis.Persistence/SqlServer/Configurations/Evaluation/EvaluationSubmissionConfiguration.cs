using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.Evaluation
{
    /// <summary>
    /// EF Core configuration for EvaluationSubmission aggregate root.
    /// </summary>
    public class EvaluationSubmissionConfiguration : IEntityTypeConfiguration<EvaluationSubmission>
    {
        public void Configure(EntityTypeBuilder<EvaluationSubmission> builder)
        {
            builder.ToTable("EvaluationSubmissions");

            builder.HasKey(e => e.Id);

            // Value Object conversions
            builder.Property(e => e.SubmissionNumber)
                .HasConversion<SubmissionNumberConverter>()
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>();

            builder.Property(e => e.Result)
                .HasConversion<int>();

            builder.Property(e => e.Notes)
                .HasMaxLength(2000);

            // ProjectSnapshot stored as JSON
            builder.Property(e => e.Snapshot)
                .HasConversion<ProjectSnapshotConverter>()
                .HasColumnType("nvarchar(max)");

            // Indexes
            builder.HasIndex(e => e.ProjectId);
            builder.HasIndex(e => e.AssignedEvaluatorId);
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.Result);
            builder.HasIndex(e => e.SubmittedAt);
            builder.HasIndex(e => new { e.ProjectId, e.SubmissionNumber }).IsUnique();

            // Ignore domain events
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
