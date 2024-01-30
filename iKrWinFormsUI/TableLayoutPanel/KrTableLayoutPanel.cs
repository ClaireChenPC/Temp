﻿// ReSharper disable UnusedMember.Global

using Krypton.Toolkit;

// ReSharper disable once CheckNamespace
namespace iKrWinFormsUI;

public class KrTableLayoutPanel : KryptonTableLayoutPanel
{
    public KrTableLayoutPanel()
    {
        this.SetCustomizedStyle();
    }

    private void SetCustomizedStyle()
    {
        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint,
            //ControlStyles.SupportsTransparentBackColor,
            true);
    }
}