using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ellipse2D
{
    public class Ellipse2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Ellipse";

        public int IconKind => (int)PackIconKind.EllipseOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }

        public void Draw(Canvas canvas)
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;
            var ellipse = new Ellipse()
            {
                Width = witdh > 0 ? witdh : -witdh,
                Height = height > 0 ? height : -height,
                Stroke = s_mColor,
                StrokeThickness = s_mThickness
            };

            if (witdh > 0 && height > 0)
            {
                Canvas.SetLeft(ellipse, _leftTop.X);
                Canvas.SetTop(ellipse, _leftTop.Y);
            }
            else if (witdh > 0 && height < 0)
            {
                Canvas.SetLeft(ellipse, _leftTop.X);
                Canvas.SetTop(ellipse, _rightBottom.Y);
            }
            else if (witdh < 0 && height > 0)
            {
                Canvas.SetLeft(ellipse, _rightBottom.X);
                Canvas.SetTop(ellipse, _leftTop.Y);
            }
            else
            {
                Canvas.SetLeft(ellipse, _rightBottom.X);
                Canvas.SetTop(ellipse, _rightBottom.Y);
            }

            canvas.Children.Add(ellipse);
        }

        public void HandleStart(double x, double y)
        {
            _leftTop.X = x;
            _leftTop.Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom.X = x;
            _rightBottom.Y = y;
        }

        public IShape Clone()
        {
            return new Ellipse2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}
