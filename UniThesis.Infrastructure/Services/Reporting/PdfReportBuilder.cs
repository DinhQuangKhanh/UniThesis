namespace UniThesis.Infrastructure.Services.Reporting
{
    public class PdfReportBuilder
    {
        private string _title = "Report";
        private string? _subtitle;
        private PdfReportMetadata? _metadata;
        private readonly List<PdfSection> _sections = new();

        public PdfReportBuilder WithTitle(string title) { _title = title; return this; }
        public PdfReportBuilder WithSubtitle(string subtitle) { _subtitle = subtitle; return this; }
        public PdfReportBuilder WithMetadata(PdfReportMetadata metadata) { _metadata = metadata; return this; }
        public PdfReportBuilder AddTextSection(string title, string content) { _sections.Add(new PdfSection(title, PdfSectionType.Text, content)); return this; }
        public PdfReportBuilder AddTableSection(string title, PdfTableData tableData) { _sections.Add(new PdfSection(title, PdfSectionType.Table, tableData)); return this; }
        public PdfReportBuilder AddKeyValueSection(string title, IEnumerable<KeyValuePair<string, string>> data) { _sections.Add(new PdfSection(title, PdfSectionType.KeyValue, data)); return this; }
        public PdfReportBuilder AddListSection(string title, IEnumerable<string> items) { _sections.Add(new PdfSection(title, PdfSectionType.List, items)); return this; }

        public PdfReportDefinition Build() => new(_title, _subtitle, _sections, _metadata);
    }
}
