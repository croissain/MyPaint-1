using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        int IconKind { get; }
        Brush s_mColor { get; set; }    // main color
        Brush s_sColor { get; set; }    // sub color (for eraser)
        int s_mThickness { get; set; }  // main thickness
        //int s_sThickness { get; set; }  // sub thickness (for eraser)
        DoubleCollection s_Outline { get; set; }
        Style s_Style { get; set; }

        void HandleStart(double x, double y);
        void HandleMove(double x, double y);
        void HandleEnd(double x, double y);

        void Draw(Canvas canvas);
        IShape Clone();
    }
}
