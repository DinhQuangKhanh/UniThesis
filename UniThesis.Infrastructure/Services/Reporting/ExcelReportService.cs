using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace UniThesis.Infrastructure.Services.Reporting
{
    public class ExcelReportService : IReportGeneratorService
    {
        private readonly ILogger<ExcelReportService> _logger;

        public ExcelReportService(ILogger<ExcelReportService> logger)
        {
            _logger = logger;
        }

        public Task<byte[]> GenerateExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken ct = default)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            var properties = typeof(T).GetProperties();

            // Header
            for (var i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Data
            var row = 2;
            foreach (var item in data)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item);
                    worksheet.Cell(row, i + 1).Value = value?.ToString() ?? string.Empty;
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            _logger.LogInformation("Excel report generated: {SheetName}", sheetName);
            return Task.FromResult(stream.ToArray());
        }

        public Task<byte[]> GenerateExcelAsync(ReportDefinition definition, CancellationToken ct = default)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(definition.SheetName);

            // Title
            worksheet.Cell(1, 1).Value = definition.Title;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, definition.Columns.Count()).Merge();

            // Header
            var col = 1;
            foreach (var column in definition.Columns)
            {
                worksheet.Cell(3, col).Value = column.Header;
                worksheet.Cell(3, col).Style.Font.Bold = true;
                worksheet.Cell(3, col).Style.Fill.BackgroundColor = XLColor.Orange;
                worksheet.Cell(3, col).Style.Font.FontColor = XLColor.White;
                worksheet.Column(col).Width = column.Width;
                col++;
            }

            // Data
            var row = 4;
            foreach (var item in definition.Data)
            {
                col = 1;
                foreach (var column in definition.Columns)
                {
                    var value = item.TryGetValue(column.Name, out var v) ? v : null;
                    worksheet.Cell(row, col).Value = value?.ToString() ?? string.Empty;
                    col++;
                }
                row++;
            }

            // Borders
            var dataRange = worksheet.Range(3, 1, row - 1, definition.Columns.Count());
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            _logger.LogInformation("Excel report generated: {Title}", definition.Title);
            return Task.FromResult(stream.ToArray());
        }

        public Task<byte[]> GeneratePdfAsync(PdfReportDefinition definition, CancellationToken ct = default)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(c => ComposeHeader(c, definition));
                    page.Content().Element(c => ComposeContent(c, definition));
                    page.Footer().Element(ComposeFooter);
                });
            });

            var bytes = document.GeneratePdf();
            _logger.LogInformation("PDF report generated: {Title}", definition.Title);
            return Task.FromResult(bytes);
        }

        private static void ComposeHeader(IContainer container, PdfReportDefinition definition)
        {
            container.Column(column =>
            {
                column.Item().Text(definition.Title).FontSize(20).Bold().FontColor(Colors.Orange.Darken2);
                if (!string.IsNullOrEmpty(definition.Subtitle))
                    column.Item().Text(definition.Subtitle).FontSize(12).Italic();
                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        private static void ComposeContent(IContainer container, PdfReportDefinition definition)
        {
            container.Column(column =>
            {
                foreach (var section in definition.Sections)
                {
                    column.Item().PaddingTop(10).Text(section.Title).FontSize(14).Bold();
                    column.Item().PaddingTop(5).Element(c => ComposeSectionContent(c, section));
                }
            });
        }

        private static void ComposeSectionContent(IContainer container, PdfSection section)
        {
            switch (section.Type)
            {
                case PdfSectionType.Text:
                    container.Text(section.Content?.ToString() ?? string.Empty);
                    break;
                case PdfSectionType.Table when section.Content is PdfTableData tableData:
                    ComposeTable(container, tableData);
                    break;
                case PdfSectionType.KeyValue when section.Content is IEnumerable<KeyValuePair<string, string>> keyValues:
                    container.Column(col =>
                    {
                        foreach (var kv in keyValues)
                            col.Item().Row(row =>
                            {
                                row.RelativeItem(1).Text(kv.Key + ":").Bold();
                                row.RelativeItem(2).Text(kv.Value);
                            });
                    });
                    break;
                case PdfSectionType.List when section.Content is IEnumerable<string> items:
                    container.Column(col =>
                    {
                        foreach (var item in items)
                            col.Item().Text($"• {item}");
                    });
                    break;
            }
        }

        private static void ComposeTable(IContainer container, PdfTableData tableData)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    foreach (var _ in tableData.Headers)
                        columns.RelativeColumn();
                });

                // Header
                foreach (var header in tableData.Headers)
                    table.Cell().Background(Colors.Orange.Lighten1).Padding(5).Text(header).Bold().FontColor(Colors.White);

                // Data
                foreach (var row in tableData.Rows)
                {
                    foreach (var cell in row)
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(cell?.ToString() ?? string.Empty);
                }
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text($"Ngày tạo: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Trang ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9);
                    x.Span(" / ").FontSize(9);
                    x.TotalPages().FontSize(9);
                });
            });
        }
    }
    public record PdfTableData(IEnumerable<string> Headers, IEnumerable<IEnumerable<object?>> Rows);

}
