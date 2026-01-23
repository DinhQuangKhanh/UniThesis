using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Common
{
    /// <summary>
    /// EF Core configuration for Report entity.
    /// </summary>
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(r => r.FilePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(r => r.Parameters)
                .HasMaxLength(4000);

            builder.Property(r => r.Type)
                .HasConversion<int>();

            builder.Property(r => r.Format)
                .HasConversion<int>();

            // Indexes
            builder.HasIndex(r => r.Type);
            builder.HasIndex(r => r.GeneratedBy);
            builder.HasIndex(r => r.GeneratedAt);
            builder.HasIndex(r => r.SemesterId);
        }
    }
}
