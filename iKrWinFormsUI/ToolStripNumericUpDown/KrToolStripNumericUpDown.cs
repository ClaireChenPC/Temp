using System.Windows.Forms.Design;
using Krypton.Toolkit;

// ReSharper disable once CheckNamespace
namespace iKrWinFormsUI;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
// ReSharper disable once UnusedMember.Global
public class KrToolStripNumericUpDown : ToolStripControlHost
{
    public KrToolStripNumericUpDown() : base(new KryptonNumericUpDown())
    {
    }

    public KryptonNumericUpDown? Instance => this.Control as KryptonNumericUpDown;
}