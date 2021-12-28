using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        int IconKind { get; }
        Brush _Brush { get; set; }
        int Thickness { get; set; }


        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);

        UIElement Draw();
        IShape Clone();
    }
}
