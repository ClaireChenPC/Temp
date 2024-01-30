// ReSharper disable once UnusedMember.Global

using Krypton.Toolkit;

// ReSharper disable once CheckNamespace
namespace iKrWinFormsUI;

public class KrPanel : KryptonPanel
{
    public KrPanel()
    {
        this.SetCustomizedStyle();
    }

    private void SetCustomizedStyle()
    {
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);
    }
}