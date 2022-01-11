using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MyPaint
{
    public partial class MainWindow : Fluent.RibbonWindow
    {
        private CheckBox checkboxUnderline = new CheckBox();
        private CheckBox checkboxStrike = new CheckBox();
        private void GroupBoxTextStyle()
        {
            var fontList = Fonts.SystemFontFamilies;

            //Combobox chọn font
            var fontCbb = new Fluent.ComboBox();
            fontCbb.Width = 170;
            fontCbb.Margin = new Thickness() { Bottom = 5, Top = 5 };
            fontCbb.Size = Fluent.RibbonControlSize.Middle;
            fontCbb.SelectedIndex = 0;
            fontCbb.SelectionChanged += FontCbb_SelectionChanged;
            fontCbb.Focusable = false;
            fontCbb.ItemsSource = fontList;

            //Combobox chọn fontsize
            var fontSizeCbb = new Fluent.ComboBox();
            fontSizeCbb.Width = 170;
            fontSizeCbb.Size = Fluent.RibbonControlSize.Middle;
            fontSizeCbb.SelectedIndex = 4;
            fontSizeCbb.SelectionChanged += FontSizeCbb_SelectionChanged;
            fontSizeCbb.Focusable = false;
            fontSizeCbb.ItemsSource = new List<Double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 32, 36, 48, 72 };

            //Checkbox chọn style
            CheckBox checkboxBold = new CheckBox()
            {
                Name = "Bold",
                Content = "B",
                Tag = (int)TextStyle.BOLD,
                FontWeight = FontWeights.DemiBold,
                FontStyle = FontStyles.Normal,
                FontSize = 18,
                Margin = new Thickness() { Right = 5 },
            };
            checkboxBold.Checked += Checkbox_Checked;
            checkboxBold.Unchecked += Checkbox_Unchecked;
            checkboxBold.Focusable = false;

            CheckBox checkboxItalic = new CheckBox()
            {
                Name = "Italic",
                Content = "i",
                Tag = (int)TextStyle.ITALIC,
                FontStyle = FontStyles.Oblique,
                FontWeight = FontWeights.Normal,
                Margin = new Thickness() { Right = 15 },
                FontSize = 18,
            };
            checkboxItalic.Checked += Checkbox_Checked;
            checkboxItalic.Unchecked += Checkbox_Unchecked;
            checkboxItalic.Focusable = false;

            checkboxUnderline = new CheckBox()
            {
                Name = "Underline",
                Content = new TextBlock() { Text = "U", TextDecorations = TextDecorations.Underline, FontSize = 18 },
                Tag = (int)TextStyle.UNDERLINE,
                Margin = new Thickness() { Right = 5 },
            };
            checkboxUnderline.Checked += Checkbox_Checked;
            checkboxUnderline.Unchecked += Checkbox_Unchecked;
            checkboxUnderline.Focusable = false;

            checkboxStrike = new CheckBox()
            {
                Name = "Strike",
                Content = new TextBlock() { Text = "abc", TextDecorations = TextDecorations.Strikethrough, FontSize = 18 },
                Tag = (int)TextStyle.STRIKE,
            };
            checkboxStrike.Checked += Checkbox_Checked;
            checkboxStrike.Unchecked += Checkbox_Unchecked;
            checkboxStrike.Focusable = false;

            // 
            StackPanel subStack = new StackPanel() { Orientation = Orientation.Horizontal };
            subStack.Children.Add(checkboxBold);
            subStack.Children.Add(checkboxItalic);
            subStack.Children.Add(checkboxUnderline);
            subStack.Children.Add(checkboxStrike);

            ChooseStyleStack.Orientation = Orientation.Vertical;
            ChooseStyleStack.Children.Add(fontCbb);
            ChooseStyleStack.Children.Add(fontSizeCbb);
            ChooseStyleStack.Children.Add(subStack);
        }

        private void FontCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var font = (sender as ComboBox).SelectedItem as FontFamily;
            _selectedFontFamily = font;
            _preview.s_FontFamily = _selectedFontFamily;

            int lastIndex = _shapes.Count - 1;
            if(lastIndex >= 0)
            {
                _shapes[lastIndex].s_FontFamily = _selectedFontFamily;
                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void FontSizeCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = (sender as ComboBox).SelectedItem;
            _selectedFontSize = (double)size;
            _preview.s_FontSize = _selectedFontSize;

            int lastIndex = _shapes.Count - 1;
            if(lastIndex >= 0)
            {
                _shapes[lastIndex].s_FontSize = _selectedFontSize;

                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            var style = (sender as CheckBox).Tag;
            if ((int)style == (int)TextStyle.BOLD)
            {
                _selectedFontWeight = FontWeights.DemiBold;
            }
            if ((int)style == (int)TextStyle.ITALIC)
            {
                _selectedFontStyle = FontStyles.Oblique;
            }

            if((int)style == (int)TextStyle.UNDERLINE)
            {
                _selectedTextDecoration = (int)TextStyle.UNDERLINE;
            }

            if ((int)style == (int)TextStyle.STRIKE)
            {
                _selectedTextDecoration = (int)TextStyle.STRIKE;
            }

            _preview.s_FontWeight = _selectedFontWeight;
            _preview.s_FontStyle = _selectedFontStyle;
            _preview.s_TextDecoration = _selectedTextDecoration;

            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                _shapes[lastIndex].s_FontWeight = _selectedFontWeight;
                _shapes[lastIndex].s_FontStyle = _selectedFontStyle;
                _shapes[lastIndex].s_TextDecoration = _selectedTextDecoration;

                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            var style = (sender as CheckBox).Tag;
            if ((int)style == (int)TextStyle.BOLD)
            {
                _selectedFontWeight = FontWeights.Normal;
            }
            if ((int)style == (int)TextStyle.ITALIC)
            {
                _selectedFontStyle = FontStyles.Normal;
            }

            if ((int)style == (int)TextStyle.UNDERLINE)
            {
                if(checkboxStrike.IsChecked == true)
                {
                    checkboxStrike.IsChecked = false;
                }
                _selectedTextDecoration = -1;
            }

            if ((int)style == (int)TextStyle.STRIKE)
            {
                if (checkboxUnderline.IsChecked == true)
                {
                    checkboxUnderline.IsChecked = false;
                }
                _selectedTextDecoration = -1;
            }

            _preview.s_FontWeight = _selectedFontWeight;
            _preview.s_FontStyle = _selectedFontStyle;
            _preview.s_TextDecoration = _selectedTextDecoration;

            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                _shapes[lastIndex].s_FontWeight = _selectedFontWeight;
                _shapes[lastIndex].s_FontStyle = _selectedFontStyle;
                _shapes[lastIndex].s_TextDecoration = _selectedTextDecoration;

                //Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                //Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }
    }
}