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
        public Brush s_Color { get; set; }
        public int s_Thickness { get; set; }

        public void Draw(Canvas canvas)
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;
            var rect = new Rectangle()
            {
                Width = witdh > 0 ? witdh : -witdh,
                Height = height > 0 ? height : -height,
                Stroke = s_Color,
                StrokeThickness = s_Thickness
            };

            if (witdh > 0 && height > 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _leftTop.Y);
            }
            else if(witdh > 0 && height < 0)
            {
                Canvas.SetLeft(rect, _leftTop.X);
                Canvas.SetTop(rect, _rightBottom.Y);
            }
            else if(witdh < 0 && height > 0)
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

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public IShape Clone()
        {
            return new Rectangle2D() { s_Color = new SolidColorBrush(Colors.Red), s_Thickness = 2 };
        }
    }
}
