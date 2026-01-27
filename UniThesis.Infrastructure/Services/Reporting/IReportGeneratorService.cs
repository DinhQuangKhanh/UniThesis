namespace UniThesis.Infrastructure.Services.Reporting
{
    public interface IReportGeneratorService
    {
        Task<byte[]> GenerateExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken ct = default);
        Task<byte[]> GenerateExcelAsync(ReportDefinition definition, CancellationToken ct = default);
        Task<byte[]> GeneratePdfAsync(PdfReportDefinition definition, CancellationToken ct = default);
    }

    public record ReportDefinition(string Title, string SheetName, IEnumerable<ReportColumn> Columns, IEnumerable<IDictionary<string, object?>> Data);
    public record ReportColumn(string Name, string Header, int Width = 20);
    public record PdfReportDefinition(string Title, string? Subtitle, IEnumerable<PdfSection> Sections, PdfReportMetadata? Metadata = null);
    public record PdfSection(string Title, PdfSectionType Type, object Content);
    public record PdfReportMetadata(string? Author, string? Department, DateTime? GeneratedAt);

    public enum PdfSectionType { Text, Table, KeyValue, List }
}
