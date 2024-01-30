using DevExpress.Spreadsheet;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;

namespace iDxWinFormsUI.Models
{
    internal class CustomOpenFileCommand : SaveDocumentAsCommand
    {
        public CustomOpenFileCommand(ISpreadsheetControl control)
            : base(control)
        {
        }

        public string InitialDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public override void Execute()
        {
            var dialog = new XtraOpenFileDialog();
            var formats = new Dictionary<string, DocumentFormat>
            {
                { "Excel Workbook (*.xlsx)|*.xlsx", DocumentFormat.OpenXml },
                { "|Excel 97-2003 Workbook (*.xls)|*.xls", DocumentFormat.Xls },
                { "|CSV (comma delimited) (*.csv)|*.csv", DocumentFormat.Csv },
                { "|Text (*.txt)|*.txt", DocumentFormat.Text }
            };

            foreach (var item in formats.Keys) dialog.Filter += item;
            dialog.InitialDirectory = this.InitialDirectory;
            dialog.FileName = "";

            if (dialog.ShowDialog() == DialogResult.OK)
                ((SpreadsheetControl)this.Control).SaveDocument(dialog.FileName,
                    formats[formats.Keys.ToList()[dialog.FilterIndex - 1]]);
        }
    }
}
