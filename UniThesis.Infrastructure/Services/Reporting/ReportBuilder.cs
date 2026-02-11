namespace UniThesis.Infrastructure.Services.Reporting
{
    public class ReportBuilder
    {
        private string _title = "Report";
        private string _sheetName = "Sheet1";
        private readonly List<ReportColumn> _columns = [];
        private readonly List<IDictionary<string, object?>> _data = [];

        public ReportBuilder WithTitle(string title) { _title = title; return this; }
        public ReportBuilder WithSheetName(string sheetName) { _sheetName = sheetName; return this; }
        public ReportBuilder AddColumn(string name, string header, int width = 20) { _columns.Add(new ReportColumn(name, header, width)); return this; }
        public ReportBuilder AddRow(IDictionary<string, object?> row) { _data.Add(row); return this; }
        public ReportBuilder AddRows(IEnumerable<IDictionary<string, object?>> rows) { _data.AddRange(rows); return this; }

        public ReportDefinition Build() => new(_title, _sheetName, _columns, _data);
    }
}
