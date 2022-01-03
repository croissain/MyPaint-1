using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Line2D;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyPaint
{
    class Eraser : IShape
    {
        private List<Line> _lines = new List<Line>();
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();

        public string Name => "Eraser";
        public int IconKind => (int)PackIconKind.Pencil;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public Brush s_Fill { get; set; }
        public Adorner currAdnr
        {
            get; set;
        }
        public AdornerLayer adnrLayer
        {
            get; set;
        }

        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
            var line = new Line()
            {
                X1 = x,
                Y1 = y,
                X2 = x,
                Y2 = y,
                StrokeThickness = s_mThickness,
                Stroke = s_sColor,
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
                StrokeThickness = s_mThickness,
                Stroke = s_sColor,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };

            _lines.Add(line);

            _start = _end;
        }

        public void HandleMove(double x, double y)
        {
            HandleEnd(x, y);
        }

        public void Draw(Canvas canvas)
        {
            foreach (var line in _lines)
            {
                canvas.Children.Add(line);
            }
        }

        public IShape Clone()
        {
            return new Eraser() { s_sColor = new SolidColorBrush(Colors.White), s_mThickness = 2 };
        }
    }
}
