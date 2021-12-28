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
        public Brush _Brush { get; set; }
        public int Thickness { get; set; }

        public UIElement Draw()
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;
            var ellipse = new Ellipse()
            {
                Width = witdh > 0 ? witdh : -witdh,
                Height = height > 0 ? height : -height,
                Stroke = _Brush,
                StrokeThickness = Thickness
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

            return ellipse;
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
            return new Ellipse2D() { _Brush = new SolidColorBrush(Colors.Red), Thickness = 2 };
        }
    }
}
