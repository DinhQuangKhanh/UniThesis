using UniThesis.Domain.Common.Primitives;
using UniThesis.Domain.Enums.System;

namespace UniThesis.Domain.Entities
{
    public class Report : Entity<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public ReportType Type { get; private set; }
        public ReportFormat Format { get; private set; }
        public string FilePath { get; private set; } = string.Empty;
        public long FileSize { get; private set; }
        public int? SemesterId { get; private set; }
        public int? DepartmentId { get; private set; }
        public string? Parameters { get; private set; }
        public Guid GeneratedBy { get; private set; }
        public DateTime GeneratedAt { get; private set; }

        private Report() { }

        public static Report Create(
            string name,
            ReportType type,
            ReportFormat format,
            string filePath,
            long fileSize,
            Guid generatedBy,
            int? semesterId = null,
            int? departmentId = null,
            string? parameters = null)
        {
            return new Report
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                Format = format,
                FilePath = filePath,
                FileSize = fileSize,
                SemesterId = semesterId,
                DepartmentId = departmentId,
                Parameters = parameters,
                GeneratedBy = generatedBy,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
