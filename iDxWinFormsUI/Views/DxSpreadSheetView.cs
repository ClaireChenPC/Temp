using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Services;
using iDxWinFormsUI.Models;

namespace iDxWinFormsUI.Views;

public partial class DxSpreadsheetView : XtraUserControl
{
	public DxSpreadsheetView()
	{
		this.InitializeComponent();
	}

	public void SubstituteService()
	{
		var service = this.Spreadsheet.GetService(typeof(ISpreadsheetCommandFactoryService));
		if (service is null) return;
		var factoryService = (ISpreadsheetCommandFactoryService)service;
		var customService = new SpreadsheetCustomService(factoryService);
		this.Spreadsheet.ReplaceService<ISpreadsheetCommandFactoryService>(customService);
		customService.Control = this.Spreadsheet;
	}
}