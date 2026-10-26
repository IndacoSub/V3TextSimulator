using System.Drawing.Drawing2D;

namespace DGRV3TS;

/// <summary>Fits the canvas to the window, keeping its lower edge at the bottom.</summary>
internal sealed class PreviewImageBox : Control
{
    private Image? image;
    public Image? Image
    {
        get => image;
        set { image = value; Invalidate(); }
    }

    public PreviewImageBox()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (image is null || ClientSize.Width <= 0 || ClientSize.Height <= 0) return;
        var scale = Math.Min((double)ClientSize.Width / image.Width, (double)ClientSize.Height / image.Height);
        var width = Math.Clamp((int)Math.Round(image.Width * scale), 1, ClientSize.Width);
        var height = Math.Clamp((int)Math.Round(image.Height * scale), 1, ClientSize.Height);
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        e.Graphics.DrawImage(image, new Rectangle((ClientSize.Width - width) / 2, ClientSize.Height - height, width, height));
    }
}
