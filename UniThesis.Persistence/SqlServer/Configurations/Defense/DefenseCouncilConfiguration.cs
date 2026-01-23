using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.DefenseAggregate.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Defense
{
    /// <summary>
    /// EF Core configuration for DefenseCouncil entity.
    /// </summary>
    public class DefenseCouncilConfiguration : IEntityTypeConfiguration<DefenseCouncil>
    {
        public void Configure(EntityTypeBuilder<DefenseCouncil> builder)
        {
            builder.ToTable("DefenseCouncils");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            // Relationships
            builder.HasMany(c => c.Members)
                .WithOne()
                .HasForeignKey("DefenseCouncilId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.SemesterId);
            builder.HasIndex(c => c.ChairmanId);
        }
    }
}
