using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        int IconKind { get; }
        Brush s_Color { get; set; }
        int s_Thickness { get; set; }
        DoubleCollection s_Outline { get; set; }

        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);

        void Draw(Canvas canvas);
        IShape Clone();
    }
}
