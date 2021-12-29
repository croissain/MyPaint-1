using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name => "Point";      
        public int IconKind => (int)PackIconKind.ChartLineVariant;
        public Brush s_Color{ get; set; }
        public int s_Thickness { get; set; }

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Draw(Canvas canvas)
        {
            Line l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = s_Thickness,
                Stroke = s_Color,
            };

            canvas.Children.Add(l);
        }

        public IShape Clone()
        {
            return new Point2D() { s_Color = new SolidColorBrush(Colors.Red), s_Thickness = 2 };
        }
    }
}
