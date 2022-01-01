using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Line2D;
using System.Collections.Generic;

namespace Star2D
{
    public class Star2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private PointCollection star_point = new PointCollection();

        public string Name => "Star";
        public int IconKind => (int)PackIconKind.StarOutline;
        public Brush s_mColor{get; set;}
        public Brush s_sColor
        {
            get; set;
        }
        public int s_mThickness
        {
            get; set;
        }
        public DoubleCollection s_Outline
        {
            get; set;
        }
        public Brush s_Fill
        {
            get; set;
        }
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public int s_Style { get; set; }

        public void Draw(Canvas canvas)
        {
            var witdh = _rightBottom.X - _leftTop.X;
            var height = _rightBottom.Y - _leftTop.Y;

            var star = new Polygon()
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
                Canvas.SetLeft(star, _leftTop.X);
                Canvas.SetTop(star, _leftTop.Y);
            }
            else if (witdh > 0 && height < 0)
            {
                Canvas.SetLeft(star, _leftTop.X);
                Canvas.SetTop(star, _rightBottom.Y);
            }
            else if (witdh < 0 && height > 0)
            {
                Canvas.SetLeft(star, _rightBottom.X);
                Canvas.SetTop(star, _leftTop.Y);
            }
            else
            {
                Canvas.SetLeft(star, _rightBottom.X);
                Canvas.SetTop(star, _rightBottom.Y);
            }
            star.Stretch = Stretch.Fill;

            star_point.Add(new Point(0, 0));
            star_point.Add(new Point(-0.11226, 0.34549));
            star_point.Add(new Point(-0.47552, 0.34549));
            star_point.Add(new Point(-0.18163, 0.55901));
            star_point.Add(new Point(-0.29389, 0.90451));
            star_point.Add(new Point(0, 0.69097));
            star_point.Add(new Point(0.29389, 0.90451));
            star_point.Add(new Point(0.18163, 0.55901));
            star_point.Add(new Point(0.47552, 0.34549));
            star_point.Add(new Point(0.11226, 0.34549));
            star.Points = star_point;


            canvas.Children.Add(star);
        }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            HandleEnd(x, y);
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public IShape Clone()
        {
            return new Star2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}
