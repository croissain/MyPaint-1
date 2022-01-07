using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Input;
using MyPaint;

namespace Ellipse2D
{
    public class Ellipse2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private Ellipse _ellipse = null;
        private Ellipse _ellipseFinal = new Ellipse();
        private Canvas _canvas;

        public string Name => "Ellipse";

        public int IconKind => (int)PackIconKind.EllipseOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public Brush s_Fill{ get; set; }
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public int s_Style { get; set; }
        public Adorner currAdnr { get; set; }
        public AdornerLayer adnrLayer { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            //_leftTop.X = x;
            //_leftTop.Y = y;
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            //_rightBottom.X = x;
            //_rightBottom.Y = y;
            if (_ellipse != null)
            {
                _ellipse.Focusable = true;
                _ellipse.Focus();
                currAdnr = new RectangleAdorner(_ellipse);
                adnrLayer.Add(currAdnr);
            }
        }

        public void HandleMove(double x, double y)
        {
            //HandleEnd(x, y);
            _rightBottom = new Point2D { X = x, Y = y };
            _ellipse = new Ellipse();
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;

            if (_ellipse != null)
            {
                rotateTransform = _ellipseFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;
                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _ellipse = new Ellipse();
                _ellipse.Width = width;
                _ellipse.Height = height;
                _ellipse.Stroke = s_mColor;
                _ellipse.StrokeThickness = s_mThickness;
                _ellipse.StrokeDashArray = s_Outline;
                _ellipse.Fill = s_Fill;
                _ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
                _ellipse.RenderTransform = new RotateTransform(angle);
                _ellipse.LostFocus += Ellipse_LostFocus;

                SetPosition(_ellipse, _width, _height);
                canvas.Children.Add(_ellipse);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _ellipseFinal.Fill = s_Fill;

            canvas.Children.Add(_ellipseFinal);

            //var ellipse = new Ellipse()
            //{
            //    Width = width > 0 ? width : -width,
            //    Height = height > 0 ? height : -height,
            //    Stroke = s_mColor,
            //    StrokeThickness = s_mThickness,
            //    StrokeDashArray = s_Outline,
            //    Fill = s_Fill,
            //};

            //if (width > 0 && height > 0)
            //{
            //    Canvas.SetLeft(ellipse, _leftTop.X);
            //    Canvas.SetTop(ellipse, _leftTop.Y);
            //}
            //else if (width > 0 && height < 0)
            //{
            //    Canvas.SetLeft(ellipse, _leftTop.X);
            //    Canvas.SetTop(ellipse, _rightBottom.Y);
            //}
            //else if (width < 0 && height > 0)
            //{
            //    Canvas.SetLeft(ellipse, _rightBottom.X);
            //    Canvas.SetTop(ellipse, _leftTop.Y);
            //}
            //else
            //{
            //    Canvas.SetLeft(ellipse, _rightBottom.X);
            //    Canvas.SetTop(ellipse, _rightBottom.Y);
            //}
            //
            //canvas.Children.Add(ellipse);
        }

        private void Ellipse_LostFocus(object sender, RoutedEventArgs e)
        {
            _ellipseFinal.Width = _ellipse.Width;
            _ellipseFinal.Height = _ellipse.Height;
            _ellipseFinal.Stroke = _ellipse.Stroke;
            _ellipseFinal.StrokeThickness = _ellipse.StrokeThickness;
            _ellipseFinal.StrokeDashArray = _ellipse.StrokeDashArray;
            _ellipseFinal.Fill = _ellipse.Fill;
            _ellipseFinal.RenderTransformOrigin = _ellipse.RenderTransformOrigin;
            _ellipseFinal.RenderTransform = _ellipse.RenderTransform;

            Canvas.SetLeft(_ellipseFinal, Canvas.GetLeft(_ellipse));
            Canvas.SetTop(_ellipseFinal, Canvas.GetTop(_ellipse));

            _canvas.Children.Remove(_ellipse);
            _ellipse = null;
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
            return new Ellipse2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}

// # bỏ cmt để bỏ Adorner.