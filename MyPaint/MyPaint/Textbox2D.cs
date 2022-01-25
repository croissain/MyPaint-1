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
    enum TextStyle:int
    {
        BOLD = 1,
        ITALIC = 2,
        UNDERLINE = 3,
        STRIKE = 4
    }
    class Textbox2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private TextBox _textbox = null;
        private Rectangle _rect = null;
        private TextBlock _text = new TextBlock();
        private Canvas _canvas;

        public string Name => "Textbox";

        public int IconKind => (int)PackIconKind.RectangleOutline;
        public Brush s_mColor { get; set; }
        public Brush s_sColor { get; set; }
        public int s_mThickness { get; set; }
        public DoubleCollection s_Outline { get; set; }
        public Brush s_Fill { get; set; }
        public Adorner currAdnr { get; set; }
        public AdornerLayer adnrLayer { get; set; }
        //Text style
        public FontFamily s_FontFamily { get; set; }
        public double s_FontSize { get; set; }
        public FontWeight s_FontWeight { get; set; }
        public FontStyle s_FontStyle { get; set; }
        public int s_TextDecoration { get; set; }

        RotateTransform rotateTransform = new RotateTransform();

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleMove(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            _rect = new Rectangle();
            _textbox = new TextBox();
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            if (_textbox != null)
            {
                _textbox.Focus();
                _rect.Stroke = Brushes.Transparent;
                currAdnr = new RectangleAdorner(_textbox);
                adnrLayer.Add(currAdnr);
            }
            if(_rect != null)
            {
                _canvas.Children.Remove(_rect);
                _rect = null;
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

            rotateTransform = _text.RenderTransform as RotateTransform;
            double angle = (rotateTransform != null) ? rotateTransform.Angle : 0;
            var width = Math.Abs(_width);
            var height = Math.Abs(_height);
            if (_rect != null)
            {
                _rect.Width = width;
                _rect.Height = height;
                _rect.Stroke = new SolidColorBrush(Colors.Blue);
                _rect.StrokeThickness = 1;
                _rect.StrokeDashArray = new DoubleCollection() { 3, 2 };

                SetPosition(_rect, _width, _height);
                canvas.Children.Add(_rect);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            if(_textbox != null)
            {
                _textbox.Width = width;
                _textbox.Height = height;
                _textbox.TextWrapping = TextWrapping.Wrap;
                _textbox.Foreground = s_mColor;
                _textbox.FontFamily = s_FontFamily;
                _textbox.FontSize = s_FontSize;
                _textbox.Background = Brushes.Transparent;
                _textbox.BorderBrush = Brushes.Transparent;
                _textbox.RenderTransformOrigin = new Point(0.5, 0.5);
                _textbox.RenderTransform = new RotateTransform(angle);
                _textbox.LostFocus += TextBox_LostFocus;

                SetPosition(_textbox, _width, _height);
                canvas.Children.Add(_textbox);
                adnrLayer = AdornerLayer.GetAdornerLayer(canvas);
            }

            _text.Foreground = s_mColor;
            _text.FontSize = s_FontSize;
            _text.FontFamily = s_FontFamily;
            _text.FontWeight = s_FontWeight;
            _text.FontStyle = s_FontStyle;
            if (s_TextDecoration == (int)TextStyle.UNDERLINE)
            {
                _text.TextDecorations = TextDecorations.Underline;
            }
            else if(s_TextDecoration == (int)TextStyle.STRIKE)
            {
                _text.TextDecorations = TextDecorations.Strikethrough;
            }
            else
            {
                _text.TextDecorations = null;
            }

            //SetPosition(_text, _width, _height);
            canvas.Children.Add(_text);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(_textbox != null)
            {
                _text.Text = _textbox.Text;
                _text.Width = _textbox.Width;
                _text.Height = _textbox.Height;
                _text.RenderTransformOrigin = _textbox.RenderTransformOrigin;
                _text.RenderTransform = _textbox.RenderTransform;
                _text.TextWrapping = TextWrapping.Wrap;

                Canvas.SetLeft(_text, Canvas.GetLeft(_textbox));
                Canvas.SetTop(_text, Canvas.GetTop(_textbox));
            }

            _canvas.Children.Remove(_textbox);
            _textbox = null;
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
            return new Textbox2D();
        }
    }
}
