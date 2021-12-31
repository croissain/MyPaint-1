using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyPaint
{
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
            if(_textbox != null)
            {
                _textbox.Focus();
            }
        }

        public void Draw(Canvas canvas)
        {
            _canvas = canvas;
            var _width = _rightBottom.X - _leftTop.X;
            var _height = _rightBottom.Y - _leftTop.Y;

            if (_rect != null && _textbox != null)
            {
                var width = Math.Abs(_width);
                var height = Math.Abs(_height);

                _rect.Width = width;
                _rect.Height = height;
                _rect.Stroke = new SolidColorBrush(Colors.Blue);
                _rect.StrokeThickness = 1;
                _rect.StrokeDashArray = new DoubleCollection() { 3, 2 };

                _textbox = new TextBox();
                _textbox.Width = width;
                _textbox.Height = height;
                _textbox.TextWrapping = TextWrapping.Wrap;
                _textbox.Foreground = s_mColor;
                _textbox.Background = s_sColor;
                _textbox.BorderBrush = Brushes.Transparent;
                _textbox.LostFocus += TextBox_LostFocus;

                SetPosition(_rect, _width, _height);
                SetPosition(_textbox, _width, _height);
                canvas.Children.Add(_textbox);
                canvas.Children.Add(_rect);
            }

            _text.Foreground = s_mColor;
            _text.FontSize = 14;
            SetPosition(_text, _width, _height);
            canvas.Children.Add(_text);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _text.Text = _textbox.Text;

            _canvas.Children.Remove(_rect);
            _canvas.Children.Remove(_textbox);
            _rect = null;
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
            return new Textbox2D() { s_mColor = new SolidColorBrush(Colors.Red), s_mThickness = 2 };
        }
    }
}
