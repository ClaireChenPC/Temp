using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;

namespace iDxWinFormsUI.Models;

internal class SpreadsheetCustomService : SpreadsheetCommandFactoryServiceWrapper
{
    public SpreadsheetCustomService(ISpreadsheetCommandFactoryService service)
        : base(service)
    {
    }

    public SpreadsheetControl? Control { get; set; }

    public string OpenFileInitialDirectory { get; set; } =
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public string SaveFileAsInitialDirectory { get; set; } =
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public override SpreadsheetCommand CreateCommand(SpreadsheetCommandId id)
    {
        if (this.Control is null) return base.CreateCommand(id);

        if (id == SpreadsheetCommandId.FileSaveAs)
        {
            var command = new CustomSaveFileAsCommand(this.Control)
            {
                InitialDirectory = this.SaveFileAsInitialDirectory
            };

            return command;
        }

        if (id != SpreadsheetCommandId.FileOpen) return base.CreateCommand(id);

        {
            var command = new CustomOpenFileCommand(this.Control)
            {
                InitialDirectory = this.OpenFileInitialDirectory
            };

            return command;
        }
    }
}