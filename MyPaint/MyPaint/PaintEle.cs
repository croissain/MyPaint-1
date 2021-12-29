using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyPaint
{
    enum Fills
    {
        NoFill, Solid, Linear, Radial, Image
    }

    class PaintEle
    {
        public Fills FillType = Fills.NoFill;
        public DoubleCollection OutLineType = new DoubleCollection() { 1, 0 };
        public int currSize = 2;
        public bool mainColorSelected = true;
        public bool subColorSelected = false;
        public Brush ColorOutLineBrush = Brushes.Black;
        public Brush ColorFillBrush = Brushes.Transparent;
        public FontFamily currFont = new FontFamily("Arial");
        public Brush getFillBrush()
        {
            Color cl1 = ((System.Windows.Media.SolidColorBrush)(ColorOutLineBrush)).Color;
            Color cl2 = ((System.Windows.Media.SolidColorBrush)(ColorFillBrush)).Color;
            switch (FillType)
            {
                case Fills.NoFill:
                    return Brushes.Transparent;
                case Fills.Solid:
                    if (mainColorSelected)
                        return new SolidColorBrush(cl1);
                    else
                        return new SolidColorBrush(cl2);
                case Fills.Linear:
                    return new LinearGradientBrush(cl1, cl2, 1);
                case Fills.Radial:
                    return new RadialGradientBrush(cl1, cl2);
                default:
                    return new SolidColorBrush(cl1);
            }
        }
    }
}
