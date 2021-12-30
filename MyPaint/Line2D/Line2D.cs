using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    public class Line2D : IShape
    {
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();

        public string Name => "Line";
        public int IconKind => (int)PackIconKind.ChartLineVariant;
        public Brush s_Color { get; set; }
        public int s_Thickness { get; set; }
<<<<<<< HEAD
        public DoubleCollection s_Outline { get; set; }
=======
>>>>>>> 19120575

        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public void Draw(Canvas canvas)
        {
            Line l = new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = _end.X,
                Y2 = _end.Y,
                StrokeThickness = s_Thickness,
<<<<<<< HEAD
                StrokeDashArray = s_Outline,
=======
>>>>>>> 19120575
                Stroke = s_Color,
            };

            canvas.Children.Add(l);
        }

        public IShape Clone()
        {
            return new Line2D() { s_Color = new SolidColorBrush(Colors.Red), s_Thickness = 2 };
        }
    }
}
