using Contract;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();         
        }

        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        Dictionary<string, int> packIconKind = new Dictionary<string, int>();
        bool _isDrawing = false;
        List<IShape> _shapes = new List<IShape>();
        IShape _preview;
        string _selectedShapeName = "";
        Brush _selectedColor = new SolidColorBrush(Colors.Red);
        int _selectedSize = 2;

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Tải tất cả các shape từ file .dll
            string folder = AppDomain.CurrentDomain.BaseDirectory + "ShapesDLL";
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (FileInfo f in fis)
            {
                Assembly assembly = Assembly.LoadFile(f.FullName);
                //Assembly assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(f.FullName));

                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IShape).IsAssignableFrom(type) && typeof(IShape).Namespace != type.Namespace)
                    {
                        var shape = Activator.CreateInstance(type) as IShape;
                        _prototypes.Add(shape.Name, shape);
                        packIconKind.Add(shape.Name, shape.IconKind);
                    }

                }
            }

            // Tạo ra các nút bấm hàng mẫu
            foreach (var item in _prototypes)
            {
                var shape = item.Value as IShape;
                var button = new Fluent.Button();

                button.Tag = shape.Name;
                button.SizeDefinition = "Small";
                PackIcon packIcon = new PackIcon();
                packIcon.Kind = (PackIconKind)shape.IconKind;
                button.LargeIcon = packIcon;

                button.Click += ChooseShapeButton_Click;
                ChooseShapeWrapPanel.Children.Add(button);
            }

            //Tạo ra các button colors
            foreach (var color in typeof(Colors).GetProperties())
            {
                var button = new Button();
                button.Name = color.Name;
                //button.Style = StaticResource.MaterialDesignFloatingActionMiniLightButton;
                button.Height = 20;
                button.Width = 20;
                button.Margin = new Thickness(_selectedSize);
                button.Background = new SolidColorBrush((Color)color.GetValue(null, null));

                button.Click += colorButton_Click;
                colors.Children.Add(button);
            }

            _selectedShapeName = _prototypes.First().Value.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview._Brush = _selectedColor;
            _preview.Thickness = _selectedSize;
        }

        private void _mainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ChooseSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = ChooseSize.SelectedValue as Fluent.GalleryItem;
            _selectedSize = Int32.Parse(size.Tag as string);
            _preview.Thickness = _selectedSize;
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChooseShapeButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedShapeName = (sender as Button).Tag as string;

            _preview = _prototypes[_selectedShapeName].Clone();
            _preview._Brush = _selectedColor;
        }

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = (sender as Button).Background;
            _selectedColor = color;
            _preview._Brush = color;
        }

        private void paint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;

            Point pos = e.GetPosition(paintCanvas);

            _preview.HandleStart(pos.X, pos.Y);
        }

        private void paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                Point pos = e.GetPosition(paintCanvas);
                _preview.HandleEnd(pos.X, pos.Y);

                // Xoá hết các hình vẽ cũ
                paintCanvas.Children.Clear();

                // Vẽ lại các hình trước đó
                foreach (var shape in _shapes)
                {
                    UIElement element = shape.Draw();
                    paintCanvas.Children.Add(element);
                }

                // Vẽ hình preview đè lên
                paintCanvas.Children.Add(_preview.Draw());

                //Title = $"{pos.X} {pos.Y}";
            }
        }

        private void paint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            // Thêm đối tượng cuối cùng vào mảng quản lí
            Point pos = e.GetPosition(paintCanvas);
            _preview.HandleEnd(pos.X, pos.Y);
            _shapes.Add(_preview);

            // Sinh ra đối tượng mẫu kế
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview._Brush = _selectedColor;
            _preview.Thickness = _selectedSize;

            // Ve lai Xoa toan bo
            paintCanvas.Children.Clear();

            // Ve lai tat ca cac hinh
            foreach (var shape in _shapes)
            {
                var element = shape.Draw();
                paintCanvas.Children.Add(element);
            }

        }

    }
}
