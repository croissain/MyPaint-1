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

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private Rectangle _rectangle = null;
        private Rectangle _rectangleFinal = new Rectangle();
        private Canvas _canvas;

        public string Name => "Rectangle";

        public int IconKind => (int)PackIconKind.RectangleOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public int s_Style { get; set; }
        public Brush s_Fill { get; set; }
        public Adorner currAdnr { get; set; }
        public AdornerLayer adnrLayer { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            if (_rectangle != null)
            {
                _rectangle.Focusable = true;
                _rectangle.Focus();
                currAdnr = new RectangleAdorner(_rectangle);
                adnrLayer.Add(currAdnr);
            }
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _rectangle = new Rectangle();
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_rectangle != null)
            {
                rotateTransform = _rectangleFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;
                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _rectangle = new Rectangle();
                _rectangle.Width = width;
                _rectangle.Height = height;
                _rectangle.Stroke = s_mColor;
                _rectangle.StrokeThickness = s_mThickness;
                _rectangle.StrokeDashArray = s_Outline;
                _rectangle.Fill = s_Fill;
                _rectangle.RenderTransformOrigin = new Point(0.5, 0.5);
                _rectangle.RenderTransform = new RotateTransform(angle);
                _rectangle.LostFocus += Rectangle_LostFocus;

                SetPosition(_rectangle, _width, _height);
                canvas.Children.Add(_rectangle);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _rectangleFinal.Fill = s_Fill;

            canvas.Children.Add(_rectangleFinal);
        }

        private void Rectangle_LostFocus(object sender, RoutedEventArgs e)
        {
            _rectangleFinal.Width = _rectangle.Width;
            _rectangleFinal.Height = _rectangle.Height;
            _rectangleFinal.Stroke = _rectangle.Stroke;
            _rectangleFinal.StrokeThickness = _rectangle.StrokeThickness;
            _rectangleFinal.StrokeDashArray = _rectangle.StrokeDashArray;
            _rectangleFinal.Fill = _rectangle.Fill;
            _rectangleFinal.RenderTransformOrigin = _rectangle.RenderTransformOrigin;
            _rectangleFinal.RenderTransform = _rectangle.RenderTransform;

            Canvas.SetLeft(_rectangleFinal, Canvas.GetLeft(_rectangle));
            Canvas.SetTop(_rectangleFinal, Canvas.GetTop(_rectangle));

            _canvas.Children.Remove(_rectangle);
            _rectangle = null;
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
            return new Rectangle2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}