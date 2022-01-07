using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Line2D;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using MyPaint;


namespace Star2D
{
    public class Star2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private PointCollection star_point = new PointCollection();

        private Polygon _star = null;
        private Polygon _starFinal = new Polygon();
        private Canvas _canvas;

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
        public Adorner currAdnr { get; set; }
        public AdornerLayer adnrLayer { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
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
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _star = new Polygon();
        }

        public void HandleEnd(double x, double y)
        {
            if (_star != null)
            {
                _star.Focusable = true;
                _star.Focus();
                currAdnr = new RectangleAdorner(_star);
                adnrLayer.Add(currAdnr);
            }
        }

        public void Draw(Canvas canvas)
        {
            //var witdh = _rightBottom.X - _leftTop.X;
            //var height = _rightBottom.Y - _leftTop.Y;

            //var star = new Polygon()
            //{
            //    Width = witdh > 0 ? witdh : -witdh,
            //    Height = height > 0 ? height : -height,
            //    Stroke = s_mColor,
            //    StrokeThickness = s_mThickness,
            //    StrokeDashArray = s_Outline,
            //    Fill = s_Fill,
            //};

            //if (witdh > 0 && height > 0)
            //{
            //    Canvas.SetLeft(star, _leftTop.X);
            //    Canvas.SetTop(star, _leftTop.Y);
            //}
            //else if (witdh > 0 && height < 0)
            //{
            //    Canvas.SetLeft(star, _leftTop.X);
            //    Canvas.SetTop(star, _rightBottom.Y);
            //}
            //else if (witdh < 0 && height > 0)
            //{
            //    Canvas.SetLeft(star, _rightBottom.X);
            //    Canvas.SetTop(star, _leftTop.Y);
            //}
            //else
            //{
            //    Canvas.SetLeft(star, _rightBottom.X);
            //    Canvas.SetTop(star, _rightBottom.Y);
            //}
            //star.Stretch = Stretch.Fill;


            //star.Points = star_point;


            //canvas.Children.Add(star);

            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_star != null)
            {
                rotateTransform = _starFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;
                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _star = new Polygon();
                _star.Width = width;
                _star.Height = height;
                _star.Stroke = s_mColor;
                _star.StrokeThickness = s_mThickness;
                _star.StrokeDashArray = s_Outline;
                _star.Fill = s_Fill;
                _star.RenderTransformOrigin = new Point(0.5, 0.5);
                _star.RenderTransform = new RotateTransform(angle);
                _star.LostFocus += Star_LostFocus;
                _star.Stretch = Stretch.Fill;
                _star.Points = star_point;

                SetPosition(_star, _width, _height);
                canvas.Children.Add(_star);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _starFinal.Fill = s_Fill;

            canvas.Children.Add(_starFinal);
        }

        private void Star_LostFocus(object sender, RoutedEventArgs e)
        {
            _starFinal.Width = _star.Width;
            _starFinal.Height = _star.Height;
            _starFinal.Stroke = _star.Stroke;
            _starFinal.StrokeThickness = _star.StrokeThickness;
            _starFinal.StrokeDashArray = _star.StrokeDashArray;
            _starFinal.Fill = _star.Fill;
            _starFinal.RenderTransformOrigin = _star.RenderTransformOrigin;
            _starFinal.RenderTransform = _star.RenderTransform;
            _starFinal.Stretch = Stretch.Fill;
            _starFinal.Points = star_point;

            Canvas.SetLeft(_starFinal, Canvas.GetLeft(_star));
            Canvas.SetTop(_starFinal, Canvas.GetTop(_star));

            _canvas.Children.Remove(_star);
            _star = null;
        }

        private void SetPosition(UIElement shape, double width, double height)
        {
            if (width > 0 && height > 0)
            {
                Canvas.SetLeft(shape, _leftTop.X);
                Canvas.SetTop(shape, _leftTop.Y);
            }
            else if (width > 0 && height < 0)
            {
                Canvas.SetLeft(shape, _leftTop.X);
                Canvas.SetTop(shape, _rightBottom.Y);
            }
            else if (width < 0 && height > 0)
            {
                Canvas.SetLeft(shape, _rightBottom.X);
                Canvas.SetTop(shape, _leftTop.Y);
            }
            else
            {
                Canvas.SetLeft(shape, _rightBottom.X);
                Canvas.SetTop(shape, _rightBottom.Y);
            }
        }

        public IShape Clone()
        {
            return new Star2D();
        }
    }
}