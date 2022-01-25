using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;

namespace MyPaint
{
    class Image2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public Image image = null;
        public Image _imageFinal = new Image();
        private Canvas _canvas;

        public string Name => "Image";

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
            //image = new Image();
        }

        public void HandleEnd(double x, double y)
        {
            if (image != null)
            {
                image.Focusable = true;
                image.Focus();
                currAdnr = new RectangleAdorner(image);
                adnrLayer.Add(currAdnr);
            }   
        }

        public void HandleHoldShift(double x, double y)
        {
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;

            if (image != null)
            {
                rotateTransform = _imageFinal.RenderTransform as RotateTransform;
                double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;

                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = new RotateTransform(angle);
                image.LostFocus += Image_LostFocus;

                Canvas.SetLeft(image, _leftTop.X);
                Canvas.SetTop(image, _leftTop.Y);
                canvas.Children.Add(image);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            if(_imageFinal != null)
            {   
                canvas.Children.Add(_imageFinal);
            }
        }

        private void Image_LostFocus(object sender, RoutedEventArgs e)
        {
            if (image != null)
            {
                _imageFinal.Source = image.Source;
                _imageFinal.RenderTransformOrigin = image.RenderTransformOrigin;
                _imageFinal.RenderTransform = image.RenderTransform;

                Canvas.SetLeft(_imageFinal, Canvas.GetLeft(image));
                Canvas.SetTop(_imageFinal, Canvas.GetTop(image));

            }

            _canvas.Children.Remove(image);
            image = null;
        }

        public IShape Clone()
        {
            return new Image2D();
        }
    }
}
