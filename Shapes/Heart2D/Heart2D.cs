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

namespace Heart2D
{
    public class Heart2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private Path _heart = null;
        private Path _heartFinal = new Path();
        private Canvas _canvas;

        public string Name => "Heart";

        public int IconKind => (int)PackIconKind.HeartOutline;
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
        public Brush s_Fill
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
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            if (_heart != null)
            {
                _heart.Focusable = true;
                _heart.Focus();
                currAdnr = new RectangleAdorner(_heart);
                adnrLayer.Add(currAdnr);
            }
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _heart = new Path();
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;
            if (_heart != null)
            {
                rotateTransform = _heartFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                //_heart = new Heart();
                _heart.Width = Math.Abs(_width);
                _heart.Height = Math.Abs(_height);
                _heart.Stroke = s_mColor;
                _heart.StrokeThickness = s_mThickness;
                _heart.StrokeDashArray = s_Outline;
                _heart.Data = Geometry.Parse(@"M 241,200 
                                            A 20,20 0 0 0 200,240
                                            C 210,250 240,270 240,270
                                            C 240,270 260,260 280,240
                                            A 20,20 0 0 0 239,200");
                _heart.Fill = s_Fill;
                _heart.RenderTransformOrigin = new Point(0.5, 0.5);
                _heart.RenderTransform = new RotateTransform(angle);
                _heart.LostFocus += Rectangle_LostFocus;

                SetPosition(_heart, _width, _height);
                canvas.Children.Add(_heart);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _heartFinal.Fill = s_Fill;

            canvas.Children.Add(_heartFinal);
        }

        private void Rectangle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_heart != null)
            {
                _heartFinal.Width = _heart.Width;
                _heartFinal.Height = _heart.Height;
                _heartFinal.Stroke = _heart.Stroke;
                _heartFinal.StrokeThickness = _heart.StrokeThickness;
                _heartFinal.StrokeDashArray = _heart.StrokeDashArray;
                _heartFinal.Data = _heart.Data;
                _heartFinal.Fill = _heart.Fill;
                _heartFinal.RenderTransformOrigin = _heart.RenderTransformOrigin;
                _heartFinal.RenderTransform = _heart.RenderTransform;

                Canvas.SetLeft(_heartFinal, Canvas.GetLeft(_heart));
                Canvas.SetTop(_heartFinal, Canvas.GetTop(_heart));
            }

            _canvas.Children.Remove(_heart);
            _heart = null;
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
            return new Heart2D();
        }
    }
}
