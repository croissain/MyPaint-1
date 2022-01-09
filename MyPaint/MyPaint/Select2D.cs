using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using Contract;

namespace MyPaint
{
    class Select2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private Rectangle _rect = null;
        private Image _image = null;
        private Image _imageFinal = new Image();
        private Canvas _canvas;

        public string Name => "Select";

        public int IconKind => (int)PackIconKind.RectangleOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public Brush s_Fill { get; set; }
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public int s_Style { get; set; }
        public Adorner currAdnr { get; set; }
        public AdornerLayer adnrLayer { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            if (_rect != null)
            {
                _canvas.Children.Remove(_rect);
            }
            if (_image != null)
            {
                _canvas.Children.Remove(_image);
            }
            _rect = new Rectangle();
            _image = CanvasUltilities.Crop(_canvas, _leftTop.X, _leftTop.Y, Math.Abs(x - _leftTop.X), Math.Abs(y - _leftTop.Y));
        }

        public void HandleEnd(double x, double y)
        {
            if (_image != null)
            {
                _image.Focusable = true;
                _image.Focus();
                currAdnr = new RectangleAdorner(_image);
                adnrLayer.Add(currAdnr);
            }
            if (_rect != null)
            {
                _rect.StrokeThickness = 0;
                _rect.Fill = Brushes.White;
            }
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;

            if (_rect != null && _image != null)
            {
                rotateTransform = _imageFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _rect.Width = width;
                _rect.Height = height;
                _rect.Stroke = new SolidColorBrush(Colors.Blue);
                _rect.StrokeThickness = 1;
                _rect.Fill = Brushes.Transparent;
                _rect.StrokeDashArray = new DoubleCollection() { 3, 2 };
                SetPosition(_rect, _width, _height);
                canvas.Children.Add(_rect);

                _image.Width = width;
                _image.Height = height;
                _image.RenderTransformOrigin = new Point(0.5, 0.5);
                _image.RenderTransform = new RotateTransform(angle);
                _image.LostFocus += Image_LostFocus;
                SetPosition(_image, _width, _height);
                canvas.Children.Add(_image);

                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            //SetPosition(_imageFinal, _width, _height);
            if (_rect.Fill == Brushes.White)
            {
                canvas.Children.Add(_rect);
            }
            canvas.Children.Add(_imageFinal);
        }

        private void Image_LostFocus(object sender, RoutedEventArgs e)
        {
            if(_image != null)
            {
                _imageFinal.Source = _image.Source;
                _imageFinal.Width = _image.Width;
                _imageFinal.Height = _image.Height;
                _imageFinal.RenderTransformOrigin = _image.RenderTransformOrigin;
                _imageFinal.RenderTransform = _image.RenderTransform;
                Canvas.SetLeft(_imageFinal, Canvas.GetLeft(_image));
                Canvas.SetTop(_imageFinal, Canvas.GetTop(_image));
            }

            //_canvas.Children.Remove(_rect);
            _canvas.Children.Remove(_image);
            //_rect = null;
            _image = null;
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
            return new Select2D();
        }
    }
}
