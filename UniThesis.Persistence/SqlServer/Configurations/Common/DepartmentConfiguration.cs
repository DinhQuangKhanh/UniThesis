using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Common
{
    /// <summary>
    /// EF Core configuration for Department entity.
    /// </summary>
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.Code)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(d => d.Code).IsUnique();
            builder.HasIndex(d => d.IsActive);

            builder.HasIndex(d => d.HeadOfDepartmentId);

            // Foreign key to User (HeadOfDepartment) - nullable
            builder.HasOne<Domain.Aggregates.UserAggregate.User>()
                .WithMany()
                .HasForeignKey(d => d.HeadOfDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed data
            builder.HasData(
                new { Id = 1, Name = "Công nghệ thông tin", Code = "CNTT", Description = "Khoa Công nghệ thông tin", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 2, Name = "Kinh tế", Code = "KT", Description = "Khoa Kinh tế", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 3, Name = "Ngôn ngữ Anh", Code = "NNA", Description = "Khoa Ngôn ngữ Anh", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
