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
        public Image image = null;
        public Image imageFinal = new Image();
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
        public FontWeight s_FontWeight { get; set; }
        public FontStyle s_FontStyle { get; set; }
        public int s_TextDecoration { get; set; }
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
            if (image != null)
            {
                _canvas.Children.Remove(image);
            }
            _rect = new Rectangle();
            image = CanvasUltilities.Crop(_canvas, _leftTop.X, _leftTop.Y, Math.Abs(x - _leftTop.X), Math.Abs(y - _leftTop.Y));
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            if (image != null)
            {
                image.Focusable = true;
                image.Focus();
                currAdnr = new RectangleAdorner(image);
                adnrLayer.Add(currAdnr);
            }
            if (_rect != null)
            {
                _rect.StrokeThickness = 0;
                _rect.Fill = Brushes.White;
            }
        }

        public void HandleHoldShift(double x, double y)
        {
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;

            if (_rect != null && image != null)
            {
                double angle = 0;
                if (imageFinal != null)
                {
                    rotateTransform = imageFinal.RenderTransform as RotateTransform;
                    angle = (rotateTransform != null) ? rotateTransform.Angle : 0;
                }

                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _rect.Width = width + 2;
                _rect.Height = height + 2;
                _rect.Stroke = new SolidColorBrush(Colors.Blue);
                _rect.StrokeThickness = 1;
                _rect.Fill = Brushes.Transparent;
                _rect.StrokeDashArray = new DoubleCollection() { 3, 2 };
                if (_width > 0 && _height > 0)
                {
                    Canvas.SetLeft(_rect, _leftTop.X - 1);
                    Canvas.SetTop(_rect, _leftTop.Y - 1);
                }
                else if (_width > 0 && _height < 0)
                {
                    Canvas.SetLeft(_rect, _leftTop.X - 1);
                    Canvas.SetTop(_rect, _rightBottom.Y - 1);
                }
                else if (_width < 0 && _height > 0)
                {
                    Canvas.SetLeft(_rect, _rightBottom.X - 1);
                    Canvas.SetTop(_rect, _leftTop.Y - 1);
                }
                else
                {
                    Canvas.SetLeft(_rect, _rightBottom.X - 1);
                    Canvas.SetTop(_rect, _rightBottom.Y - 1);
                }
                canvas.Children.Add(_rect);

                image.Width = width;
                image.Height = height;
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = new RotateTransform(angle);
                image.LostFocus += Image_LostFocus;
                SetPosition(image, _width, _height);
                canvas.Children.Add(image);

                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            if (_rect != null && _rect.Fill == Brushes.White)
            {
                canvas.Children.Add(_rect);
            }
            if (imageFinal != null)
            {
                //SetPosition(_imageFinal, _width, _height);
                canvas.Children.Add(imageFinal);
            }
        }

        private void Image_LostFocus(object sender, RoutedEventArgs e)
        {
            if(image != null && imageFinal != null)
            {
                imageFinal.Source = image.Source;
                imageFinal.Width = image.Width;
                imageFinal.Height = image.Height;
                imageFinal.RenderTransformOrigin = image.RenderTransformOrigin;
                imageFinal.RenderTransform = image.RenderTransform;
                Canvas.SetLeft(imageFinal, Canvas.GetLeft(image));
                Canvas.SetTop(imageFinal, Canvas.GetTop(image));
            }

            //_canvas.Children.Remove(_rect);
            _canvas.Children.Remove(image);
            //_rect = null;
            image = null;
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
