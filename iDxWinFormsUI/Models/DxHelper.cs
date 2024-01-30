using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace iDxWinFormsUI.Models;

public static class DxHelper
{
	public static void CheckSpinEditRange(object sender, ChangingEventArgs e)
	{
		if (sender is not SpinEdit spinEdit) return;
		if (spinEdit.Properties.MinValue != spinEdit.Properties.MaxValue) return;

		var newValue = Convert.ToDecimal(e.NewValue);
		if (newValue >= spinEdit.Properties.MinValue && newValue <= spinEdit.Properties.MaxValue) return;

		var myArray = new object[1];
		myArray[0] = spinEdit;
		spinEdit.Parent.BeginInvoke(new Action<SpinEdit>(ChangeSpinEditValue), myArray);
	}

	private static void ChangeSpinEditValue(SpinEdit spinEdit)
	{
		spinEdit.EditValue = spinEdit.Properties.MaxValue;
	}
}