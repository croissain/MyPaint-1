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

namespace Diamond2D
{
    public class Diamond2D : IShape
    {

        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private PointCollection diamond_point = new PointCollection();

        private Polygon _diamond = null;
        private Polygon _diamondFinal = new Polygon();
        private Canvas _canvas;

        public string Name => "Diamond";
        public int IconKind => (int)PackIconKind.CardsDiamondOutline;
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
            diamond_point.Add(new Point(2, 1));
            diamond_point.Add(new Point(1, 2));
            diamond_point.Add(new Point(2, 3));
            diamond_point.Add(new Point(3, 2));
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _diamond = new Polygon();
        }

        public void HandleEnd(double x, double y)
        {
            if (_diamond != null)
            {
                _diamond.Focusable = true;
                _diamond.Focus();
                currAdnr = new RectangleAdorner(_diamond);
                adnrLayer.Add(currAdnr);
            }
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_diamond != null)
            {
                rotateTransform = _diamondFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                //_diamond = new Polygon();
                _diamond.Width = Math.Abs(_width);
                _diamond.Height = Math.Abs(_height);
                _diamond.Stroke = s_mColor;
                _diamond.StrokeThickness = s_mThickness;
                _diamond.StrokeDashArray = s_Outline;
                _diamond.Fill = s_Fill;
                _diamond.RenderTransformOrigin = new Point(0.5, 0.5);
                _diamond.RenderTransform = new RotateTransform(angle);
                _diamond.LostFocus += Diamond_LostFocus;
                _diamond.Stretch = Stretch.Fill;
                _diamond.Points = diamond_point;

                SetPosition(_diamond, _width, _height);
                canvas.Children.Add(_diamond);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _diamondFinal.Fill = s_Fill;

            canvas.Children.Add(_diamondFinal);
        }

        private void Diamond_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_diamond != null)
            {
                _diamondFinal.Width = _diamond.Width;
                _diamondFinal.Height = _diamond.Height;
                _diamondFinal.Stroke = _diamond.Stroke;
                _diamondFinal.StrokeThickness = _diamond.StrokeThickness;
                _diamondFinal.StrokeDashArray = _diamond.StrokeDashArray;
                _diamondFinal.Fill = _diamond.Fill;
                _diamondFinal.RenderTransformOrigin = _diamond.RenderTransformOrigin;
                _diamondFinal.RenderTransform = _diamond.RenderTransform;
                _diamondFinal.Stretch = Stretch.Fill;
                _diamondFinal.Points = diamond_point;

                Canvas.SetLeft(_diamondFinal, Canvas.GetLeft(_diamond));
                Canvas.SetTop(_diamondFinal, Canvas.GetTop(_diamond));
            }

            _canvas.Children.Remove(_diamond);
            _diamond = null;
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
            return new Diamond2D();
        }
    }
}