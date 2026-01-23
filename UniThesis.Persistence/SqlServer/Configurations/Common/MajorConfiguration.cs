using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniThesis.Domain.Entities;

namespace UniThesis.Persistence.SqlServer.Configurations.Common
{
    /// <summary>
    /// EF Core configuration for Major entity.
    /// </summary>
    public class MajorConfiguration : IEntityTypeConfiguration<Major>
    {
        public void Configure(EntityTypeBuilder<Major> builder)
        {
            builder.ToTable("Majors");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Code)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(m => m.Code).IsUnique();
            builder.HasIndex(m => m.DepartmentId);
            builder.HasIndex(m => m.IsActive);

            // Seed data
            builder.HasData(
                new { Id = 1, DepartmentId = 1, Name = "Kỹ thuật phần mềm", Code = "SE", Description = "Chuyên ngành Kỹ thuật phần mềm", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 2, DepartmentId = 1, Name = "An toàn thông tin", Code = "IA", Description = "Chuyên ngành An toàn thông tin", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 3, DepartmentId = 1, Name = "Trí tuệ nhân tạo", Code = "AI", Description = "Chuyên ngành Trí tuệ nhân tạo", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 4, DepartmentId = 2, Name = "Quản trị kinh doanh", Code = "BA", Description = "Chuyên ngành Quản trị kinh doanh", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new { Id = 5, DepartmentId = 3, Name = "Ngôn ngữ Anh", Code = "ENG", Description = "Chuyên ngành Ngôn ngữ Anh", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
