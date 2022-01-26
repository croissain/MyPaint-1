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

namespace Triangle2D
{
    public class Triangle2D : IShape
    {

        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private PointCollection triangle_point = new PointCollection();

        private Polygon _triangle = null;
        private Polygon _triangleFinal = new Polygon();
        private Canvas _canvas;

        public string Name => "Triangle";
        public int IconKind => (int)PackIconKind.TriangleOutline;
        public Brush s_mColor
        {
            get; set;
        }
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
        public FontFamily s_FontFamily
        {
            get; set;
        }
        public double s_FontSize
        {
            get; set;
        }
        public int s_Style
        {
            get; set;
        }
        public Adorner currAdnr
        {
            get; set;
        }
        public AdornerLayer adnrLayer
        {
            get; set;
        }
        public FontWeight s_FontWeight { get; set; }
        public FontStyle s_FontStyle { get; set; }
        public int s_TextDecoration { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            triangle_point.Add(new Point(2, 1));
            triangle_point.Add(new Point(1, 2));
            triangle_point.Add(new Point(3, 2));
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _triangle = new Polygon();
        }

        public void HandleHoldShift(double x, double y)
        {
            double _width = Math.Abs(x - _leftTop.X);
            double _height = Math.Abs(y - _leftTop.Y);
            double diff = _width < _height ? _width : _height;

            if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                if (_width > _height)
                {
                    _rightBottom = new Point2D() { X = _leftTop.X + diff, Y = y };
                }
                else
                {
                    _rightBottom = new Point2D() { X = x, Y = _leftTop.Y + diff };
                }
            }
            else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
            {
                if (_width > _height)
                {
                    _rightBottom = new Point2D() { X = _leftTop.X + diff, Y = y };
                }
                else
                {
                    _rightBottom = new Point2D() { X = x, Y = _leftTop.Y - diff };
                }
            }
            else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                if (_width > _height)
                {
                    _rightBottom = new Point2D() { X = _leftTop.X - diff, Y = y };
                }
                else
                {
                    _rightBottom = new Point2D() { X = x, Y = _leftTop.Y + diff };
                }
            }
            else
            {
                if (_width > _height)
                {
                    _rightBottom = new Point2D() { X = _leftTop.X - diff, Y = y };
                }
                else
                {
                    _rightBottom = new Point2D() { X = x, Y = _leftTop.Y - diff };
                }
            }
            _triangle = new Polygon();
        }

        public void HandleEnd(double x, double y)
        {
            if (_triangle != null)
            {
                _triangle.Focusable = true;
                _triangle.Focus();
                currAdnr = new RectangleAdorner(_triangle);
                adnrLayer.Add(currAdnr);
            }
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_triangle != null)
            {
                rotateTransform = _triangleFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                //_triangle = new Polygon();
                _triangle.Width = Math.Abs(_width);
                _triangle.Height = Math.Abs(_height);
                _triangle.Stroke = s_mColor;
                _triangle.StrokeThickness = s_mThickness;
                _triangle.StrokeDashArray = s_Outline;
                _triangle.Fill = s_Fill;
                _triangle.RenderTransformOrigin = new Point(0.5, 0.5);
                _triangle.RenderTransform = new RotateTransform(angle);
                _triangle.LostFocus += Triangle_LostFocus;
                _triangle.Stretch = Stretch.Fill;
                _triangle.Points = triangle_point;

                SetPosition(_triangle, _width, _height);
                canvas.Children.Add(_triangle);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _triangleFinal.Fill = s_Fill;

            canvas.Children.Add(_triangleFinal);
        }

        private void Triangle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_triangle != null)
            {
                _triangleFinal.Width = _triangle.Width;
                _triangleFinal.Height = _triangle.Height;
                _triangleFinal.Stroke = _triangle.Stroke;
                _triangleFinal.StrokeThickness = _triangle.StrokeThickness;
                _triangleFinal.StrokeDashArray = _triangle.StrokeDashArray;
                _triangleFinal.Fill = _triangle.Fill;
                _triangleFinal.RenderTransformOrigin = _triangle.RenderTransformOrigin;
                _triangleFinal.RenderTransform = _triangle.RenderTransform;
                _triangleFinal.Stretch = Stretch.Fill;
                _triangleFinal.Points = triangle_point;

                Canvas.SetLeft(_triangleFinal, Canvas.GetLeft(_triangle));
                Canvas.SetTop(_triangleFinal, Canvas.GetTop(_triangle));
            }

            _canvas.Children.Remove(_triangle);
            _triangle = null;
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
            return new Triangle2D();
        }

    }
}