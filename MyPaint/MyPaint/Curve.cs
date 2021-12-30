using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Line2D;
using System.Windows.Controls;

namespace MyPaint
{
    class Curve : IShape
    {
        private List<Line> _lines = new List<Line>();
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();

        public string Name => "Curve";
        public int IconKind => (int)PackIconKind.Pencil;
        public Brush s_Color { get; set; }
        public int s_Thickness { get; set; }
        public DoubleCollection s_Outline { get; set; }

        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
            var line = new Line()
            {
                X1 = x,
                Y1 = y,
                X2 = x,
                Y2 = y,
                StrokeThickness = s_Thickness,
                Stroke = s_Color,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            _lines.Add(line);
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
            var line = new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = x,
                Y2 = y,
                StrokeThickness = s_Thickness,
                Stroke = s_Color,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            _lines.Add(line);

            _start = _end;
        }

        public void Draw(Canvas canvas)
        {
            foreach(var line in _lines)
            {
                canvas.Children.Add(line);
            }
        }

        public IShape Clone()
        {
            return new Curve() { s_Color = new SolidColorBrush(Colors.Red), s_Thickness = 2 };
        }
    }
}
