using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Aggregates.DefenseAggregate.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Defense
{
    /// <summary>
    /// EF Core configuration for CouncilMember entity.
    /// </summary>
    public class CouncilMemberConfiguration : IEntityTypeConfiguration<CouncilMember>
    {
        public void Configure(EntityTypeBuilder<CouncilMember> builder)
        {
            builder.ToTable("CouncilMembers");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Role)
                .HasConversion<int>();

            // Shadow property for foreign key
            builder.Property<int>("DefenseCouncilId");

            // Indexes
            builder.HasIndex("DefenseCouncilId");
            builder.HasIndex(m => m.MemberId);
            builder.HasIndex(m => m.Role);

            // Foreign key to User (Member)
            builder.HasOne<Domain.Aggregates.UserAggregate.User>()
                .WithMany()
                .HasForeignKey(m => m.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
