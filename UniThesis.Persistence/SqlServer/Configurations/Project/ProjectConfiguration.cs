using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Persistence.ValueConverters;

namespace UniThesis.Persistence.SqlServer.Configurations.Project
{
    /// <summary>
    /// EF Core configuration for Project aggregate root.
    /// </summary>
    public class ProjectConfiguration : IEntityTypeConfiguration<Domain.Aggregates.ProjectAggregate.Project>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregates.ProjectAggregate.Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            // Value Object conversions
            builder.Property(p => p.Code)
                .HasConversion<ProjectCodeConverter>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.NameVi)
                .HasConversion<ProjectNameConverter>()
                .HasMaxLength(350)
                .IsRequired();

            builder.Property(p => p.NameEn)
                .HasConversion<ProjectNameConverter>()
                .HasMaxLength(350);

            builder.Property(p => p.NameAbbr)
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(p => p.Objectives)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(p => p.Scope)
                .HasMaxLength(2000);

            builder.Property(p => p.Technologies)
                .HasConversion<TechnologyStackConverter>()
                .HasMaxLength(500);

            builder.Property(p => p.ExpectedResults)
                .HasMaxLength(2000);

            // Enums stored as integers
            builder.Property(p => p.Status)
                .HasConversion<int>();

            builder.Property(p => p.Priority)
                .HasConversion<int>();

            builder.Property(p => p.SourceType)
                .HasConversion<int>();

            builder.Property(p => p.RegistrationType)
                .HasConversion<int>();

            builder.Property(p => p.LastEvaluationResult)
                .HasConversion<int?>();

            // Relationships
            builder.HasMany(p => p.Mentors)
                .WithOne()
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Documents)
                .WithOne()
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.Code).IsUnique();
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.SemesterId);
            builder.HasIndex(p => p.MajorId);
            builder.HasIndex(p => p.GroupId);
            builder.HasIndex(p => p.TopicPoolId);
            builder.HasIndex(p => p.CreatedAt);
            builder.HasIndex(p => p.SubmittedAt);

            // Ignore domain events collection
            builder.Ignore(p => p.DomainEvents);
            builder.Ignore(p => p.ActiveMentors);
            builder.Ignore(p => p.PrimaryMentor);
            builder.Ignore(p => p.SecondaryMentor);
        }
    }
}
