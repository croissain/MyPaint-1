using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyPaint
{
    public static class CanvasUltilities
    {
        public static Image Crop(Panel displayer, double x, double y, double Width, double Height)
        {
            if (displayer == null) return null;
            if (Width < 1 || Height < 1) return null;

            Rect rect = new Rect(displayer.RenderSize);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
              (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(displayer);

            var crop = new CroppedBitmap(rtb, new Int32Rect((int)x, (int)y, (int)Width, (int)Height));

            return new Image() { Source = BitmapFrame.Create(crop) };
        }

        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96d, 96d, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));

            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }
    }
}
