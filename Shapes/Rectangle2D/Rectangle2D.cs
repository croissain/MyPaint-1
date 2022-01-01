using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Rectangle";

        public int IconKind => (int)PackIconKind.RectangleOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public Brush s_Fill
        {
            get; set;
        }
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public int s_Style { get; set; }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            HandleEnd(x, y);
        }

        public void Draw(Canvas canvas)
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;
            var rect = new Rectangle()
            {
                Width = witdh > 0 ? witdh : -witdh,
                Height = height > 0 ? height : -height,
                Stroke = s_mColor,
                StrokeThickness = s_mThickness,
                StrokeDashArray = s_Outline,
                Fill = s_Fill,
            };

            if (witdh > 0 && height > 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _leftTop.Y);
            }
            else if (witdh > 0 && height < 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _rightBottom.Y);
            }
            else if (witdh < 0 && height > 0)
            {
                Canvas.SetLeft(rect, _rightBottom.X);
                Canvas.SetTop(rect, _leftTop.Y);
            }
            else
            {
                Canvas.SetLeft(rect, _rightBottom.X);
                Canvas.SetTop(rect, _rightBottom.Y);
            }

            canvas.Children.Add(rect);
        }

        public IShape Clone()
        {
            return new Rectangle2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}