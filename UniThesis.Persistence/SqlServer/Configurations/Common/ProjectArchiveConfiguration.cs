using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Common
{
    /// <summary>
    /// EF Core configuration for ProjectArchive entity.
    /// </summary>
    public class ProjectArchiveConfiguration : IEntityTypeConfiguration<ProjectArchive>
    {
        public void Configure(EntityTypeBuilder<ProjectArchive> builder)
        {
            builder.ToTable("ProjectArchives");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ProjectName)
                .HasMaxLength(350)
                .IsRequired();

            builder.Property(a => a.StudentNames)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.AcademicYear)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Summary)
                .HasMaxLength(2000);

            builder.Property(a => a.DocumentUrl)
                .HasMaxLength(500);

            builder.Property(a => a.Tags)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(a => a.MajorId);
            builder.HasIndex(a => a.AcademicYear);

            // Full-text search index (if using SQL Server full-text)
            // builder.HasIndex(a => new { a.ProjectName, a.Tags });
        }
    }
}
