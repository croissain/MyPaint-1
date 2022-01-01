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
        Style _selectedStyle = new Style();
        FontWeight _fontWeightSetter;

        private void GroupBoxTextStyle()
        {
            var fontList = Fonts.SystemFontFamilies;

            //Combobox chọn font
            var fontCbb = new Fluent.ComboBox();
            fontCbb.Width = 120;
            fontCbb.Margin = new Thickness() { Bottom = 5 };
            fontCbb.Size = Fluent.RibbonControlSize.Middle;
            fontCbb.SelectedIndex = 0;
            fontCbb.SelectionChanged += FontCbb_SelectionChanged;
            fontCbb.ItemsSource = fontList;

            //Combobox chọn fontsize
            var fontSizeCbb = new Fluent.ComboBox();
            fontSizeCbb.Width = 120;
            fontCbb.Margin = new Thickness() { Bottom = 5 };
            fontSizeCbb.Size = Fluent.RibbonControlSize.Middle;
            fontSizeCbb.SelectedIndex = 4;
            fontSizeCbb.SelectionChanged += FontSizeCbb_SelectionChanged;
            fontSizeCbb.ItemsSource = new List<Double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 32, 36, 48, 72 };

            //Radio button chọn style
            RadioButton radioBold = new Fluent.RadioButton() 
            {
                Name = "BoldRadio",
                FontWeight = FontWeights.DemiBold,
                FontStyle = FontStyles.Normal,
                Header = "Bold",
                GroupName = "Style",
            };
            radioBold.Checked += RadioButton_Checked;

            RadioButton radioItalic = new Fluent.RadioButton()
            {
                Name = "ItalicRadio",
                FontStyle = FontStyles.Oblique,
                FontWeight = FontWeights.Normal,
                Header = "Italic",
                GroupName = "Style",
            };
            radioItalic.Checked += RadioButton_Checked;

            RadioButton radioUnderline = new Fluent.RadioButton()
            {
                Name = "UnderlineRadio",
                Header = "Underline",
                GroupName = "Style",
            };
            radioUnderline.Checked += RadioButton_Checked;

            StackPanel subStack = new StackPanel() { Orientation = Orientation.Horizontal};
            subStack.Children.Add(radioBold);
            subStack.Children.Add(radioItalic);
            subStack.Children.Add(radioUnderline);
            //subStack.Children.Add(new TextBlock() { Text = "Underline", TextDecorations = TextDecorations.Underline });

            _selectedStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, _fontWeightSetter));
           // _selectedStyle.Setters.Add(new Setter(TextBlock.FontStyleProperty, FontStyles.Normal));

            ChooseStyleStack.Orientation = Orientation.Vertical;
            ChooseStyleStack.Children.Add(fontCbb);
            ChooseStyleStack.Children.Add(fontSizeCbb);
            ChooseStyleStack.Children.Add(subStack);
        }

        private void FontCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var font = (sender as ComboBox).SelectedItem as FontFamily;
            _selectedStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, font));
        }

        private void FontSizeCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = (sender as ComboBox).SelectedItem;
            _selectedStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, size));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var style = (sender as RadioButton);
            _fontWeightSetter = style.FontWeight;

            //_selectedStyle.Setters.OfType<Setter>().FirstOrDefault(X => X.Property == TextBlock.FontWeightProperty).Value = style.FontWeight;
            //_selectedStyle.Setters.OfType<Setter>().FirstOrDefault(X => X.Property == TextBlock.FontStyleProperty).Value = style.FontStyle;

            //try
            //{
            //    //_selectedStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, style.FontWeight));
            //    _selectedStyle.Setters.Remove(_fontWeightSetter);
            //    _fontWeightSetter = new Setter(TextBlock.FontWeightProperty, style.FontWeight);
            //    _selectedStyle.Setters.Add(_fontWeightSetter);
            //}
            //catch
            //{
            //    _fontWeightSetter = new Setter(TextBlock.FontWeightProperty, style.FontWeight);
            //    _selectedStyle.Setters.Add(_fontWeightSetter);
            //}

            //try
            //{
            //    _selectedStyle.Setters.Add(new Setter(TextBlock.FontStyleProperty, style.FontStyle));
            //}
            //catch
            //{
            //    _selectedStyle.Setters.OfType<Setter>().FirstOrDefault(X => X.Property == TextBlock.FontStyleProperty).Value = fontStyle;
            //}
        }
    }
}