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
<<<<<<< Updated upstream
        public Brush _Brush { get; set; }
        public int Thickness { get; set; }
=======
        public Brush s_Color { get; set; }
        public int s_Thickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
>>>>>>> Stashed changes

        public UIElement Draw()
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;
            var rect = new Rectangle()
            {
                Width = witdh > 0 ? witdh : -witdh,
                Height = height > 0 ? height : -height,
<<<<<<< Updated upstream
                Stroke = _Brush,
                StrokeThickness = Thickness
=======
                StrokeDashArray = s_Outline,
                Stroke = s_Color,
                StrokeThickness = s_Thickness
>>>>>>> Stashed changes
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

            return rect;
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
            return new Rectangle2D() { _Brush = new SolidColorBrush(Colors.Red), Thickness = 2 };
        }
    }
}
