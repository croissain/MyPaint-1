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

namespace Hexagon2D
{
    public class Hexagon2D : IShape
    {

        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private PointCollection hexagon_point = new PointCollection();

        private Polygon _hexagon = null;
        private Polygon _hexagonFinal = new Polygon();
        private Canvas _canvas;

        public string Name => "Hexagon";
        public int IconKind => (int)PackIconKind.HexagonOutline;
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

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            hexagon_point.Add(new Point(2, 1));
            hexagon_point.Add(new Point(1, 2));
            hexagon_point.Add(new Point(1, 3.41421));
            hexagon_point.Add(new Point(2, 4.41421));
            hexagon_point.Add(new Point(3, 3.41421));
            hexagon_point.Add(new Point(3, 2));
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _hexagon = new Polygon();
        }

        public void HandleEnd(double x, double y)
        {
            if (_hexagon != null)
            {
                _hexagon.Focusable = true;
                _hexagon.Focus();
                currAdnr = new RectangleAdorner(_hexagon);
                adnrLayer.Add(currAdnr);
            }
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_hexagon != null)
            {
                rotateTransform = _hexagonFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                //_hexagon = new Polygon();
                _hexagon.Width = Math.Abs(_width);
                _hexagon.Height = Math.Abs(_height);
                _hexagon.Stroke = s_mColor;
                _hexagon.StrokeThickness = s_mThickness;
                _hexagon.StrokeDashArray = s_Outline;
                _hexagon.Fill = s_Fill;
                _hexagon.RenderTransformOrigin = new Point(0.5, 0.5);
                _hexagon.RenderTransform = new RotateTransform(angle);
                _hexagon.LostFocus += Hexagon_LostFocus;
                _hexagon.Stretch = Stretch.Fill;
                _hexagon.Points = hexagon_point;

                SetPosition(_hexagon, _width, _height);
                canvas.Children.Add(_hexagon);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _hexagonFinal.Fill = s_Fill;

            canvas.Children.Add(_hexagonFinal);
        }

        private void Hexagon_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_hexagon != null)
            {
                _hexagonFinal.Width = _hexagon.Width;
                _hexagonFinal.Height = _hexagon.Height;
                _hexagonFinal.Stroke = _hexagon.Stroke;
                _hexagonFinal.StrokeThickness = _hexagon.StrokeThickness;
                _hexagonFinal.StrokeDashArray = _hexagon.StrokeDashArray;
                _hexagonFinal.Fill = _hexagon.Fill;
                _hexagonFinal.RenderTransformOrigin = _hexagon.RenderTransformOrigin;
                _hexagonFinal.RenderTransform = _hexagon.RenderTransform;
                _hexagonFinal.Stretch = Stretch.Fill;
                _hexagonFinal.Points = hexagon_point;

                Canvas.SetLeft(_hexagonFinal, Canvas.GetLeft(_hexagon));
                Canvas.SetTop(_hexagonFinal, Canvas.GetTop(_hexagon));
            }

            _canvas.Children.Remove(_hexagon);
            _hexagon = null;
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
            return new Hexagon2D();
        }
    }
}