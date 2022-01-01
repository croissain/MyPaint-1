using Contract;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
        Brush _selectedmColor; //m là main color
        Brush _selectedsColor; //s là sub color
        int _selectedSize;
        bool mainColorSelected = true;
        PaintEle pe = new PaintEle();
        List<Outline> _outlines = new List<Outline>();
        DoubleCollection _selectedOutline;

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
                    if (type.IsClass && typeof(IShape).IsAssignableFrom(type)) // && typeof(IShape).Namespace != type.Namespace
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
                button.Margin = new Thickness(2);
                button.Background = new SolidColorBrush((Color)color.GetValue(null, null));

                button.Click += colorButton_Click;
                colors.Children.Add(button);
            }

            //Thêm curve vào danh sách
            Curve curve = new Curve();
            _prototypes.Add(curve.Name, curve);

            //Thêm eraser vào danh sách
            Eraser eraser = new Eraser();
            _prototypes.Add(eraser.Name, eraser);

            //Thêm textbox vào danh sách
            Textbox2D txb = new Textbox2D();
            _prototypes.Add(txb.Name, txb);

            //Cấu hình thông số ban đầu
            _selectedShapeName = curve.Name;
            _selectedmColor = new SolidColorBrush(Colors.Black);
            _selectedsColor = new SolidColorBrush(Colors.White);
            _selectedSize = 2;
            _selectedOutline = null;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;

            //Thêm outline vào danh sách
            _outlines.Add(new Outline() { Name = "Solid", Value = null });
            _outlines.Add(new Outline() { Name = "Dash", Value = new DoubleCollection() { 3, 4 } });
            _outlines.Add(new Outline() { Name = "Dot", Value = new DoubleCollection() { 1, 1 } });
            _outlines.Add(new Outline() { Name = "Dash Dot", Value = new DoubleCollection() { 4, 1, 1, 1 } });
            OutlineCbbox.ItemsSource = _outlines;
        }

        private void _mainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChooseSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = ChooseSize.SelectedValue as Fluent.GalleryItem;
            _selectedSize = Int32.Parse(size.Tag as string);
            _preview.s_mThickness = _selectedSize;
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
            _preview.s_mColor = _selectedmColor;
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;
            _preview.s_Outline = _selectedOutline;
        }

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = (sender as Button).Background;

            if (mainColorSelected)
            {
                mainColor.Background = color;
                //pe.ColorOutLineBrush = color;
                _preview.s_mColor = mainColor.Background;
                _preview.s_sColor = subColor.Background;
                _selectedmColor = _preview.s_mColor;
                _selectedsColor = _preview.s_sColor;
            }
            else
            {
                subColor.Background = color;
                //pe.ColorFillBrush = color;
                _preview.s_mColor = mainColor.Background;
                _preview.s_sColor = subColor.Background;
                _selectedmColor = mainColor.Background;
                _selectedsColor = subColor.Background;
                mainColorSelected = true;
            }
        }

        private void Outline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var outline = (OutlineCbbox.SelectedValue as Outline);
            _selectedOutline = outline.Value;
            _preview.s_Outline = _selectedOutline;
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
                _preview.HandleMove(pos.X, pos.Y);

                // Xoá hết các hình vẽ cũ
                paintCanvas.Children.Clear();

                // Vẽ lại các hình trước đó
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }

                // Vẽ hình preview đè lên
                _preview.Draw(paintCanvas);

                //Title = $"{pos.X} {pos.Y}";
            }
        }

        private void paint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            // Thêm đối tượng cuối cùng vào mảng quản lí        
            _shapes.Add(_preview);

            // Ve lai Xoa toan bo
            paintCanvas.Children.Clear();

            // Ve lai tat ca cac hinh
            foreach (var shape in _shapes)
            {
                shape.Draw(paintCanvas);
            }

            //Gọi hàm xử lý kết thúc cho đối tượng cuối cùng
            Point pos = e.GetPosition(paintCanvas);
            _preview.HandleEnd(pos.X, pos.Y);

            // Sinh ra đối tượng mẫu kế
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;
            _preview.s_Outline = _selectedOutline;
        }

        private void buttonEraser_Click(object sender, RoutedEventArgs e)
        {
            Eraser eraser = new Eraser();
            _selectedShapeName = eraser.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;
        }

        private void buttonPencil_Click(object sender, RoutedEventArgs e)
        {
            Curve curve = new Curve();
            _selectedShapeName = curve.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;
            _preview.s_mThickness = _selectedSize;
        }

        private void mainColor_Click(object sender, RoutedEventArgs e)
        {
            mainColorSelected = true;
        }

        private void subColor_Click(object sender, RoutedEventArgs e)
        {
            mainColorSelected = false;

        }

        private void buttonBucket_Click(object sender, RoutedEventArgs e)
        {

        }
        private void moreColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color cl = new Color();
                cl.A = dialog.Color.A;
                cl.R = dialog.Color.R;
                cl.G = dialog.Color.G;
                cl.B = dialog.Color.B;

                Brush br = new SolidColorBrush(cl);
                if (mainColorSelected)
                    mainColor.Background = br;
                else
                    subColor.Background = br;

                _selectedmColor = mainColor.Background;
                _selectedsColor = subColor.Background;

            }
        }

        #region newFile
        private void newButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region saveFile
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        string save_filename;
        private void CreateSaveDialog()
        {
            dlg.FileName = "mypaint"; // Default file name
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Jpg Files (*.jpg)|*.jpg|Png Files (*.png)|*.png|Canvas (.cvs)|*.cvs|Bitmap (.bmp)|*.bmp"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename_tmp = dlg.FileName;
            }
            save_filename = dlg.FileName.ToString();
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSaveDialog();
            util.SaveGridCanvas(this, fullCanvas, 96, save_filename);
        }
        #endregion

        #region loadFile
        Microsoft.Win32.OpenFileDialog dlg_open = new Microsoft.Win32.OpenFileDialog();
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogue = new OpenFileDialog()
            {
                Filter = "JPG Image (.jpg)|*.jpg|Png Image (.png)|*.png|Gif Image (.gif)|*.gif|Bitmap Image (.bmp)|*.bmp"
            };

            if (openFileDialogue.ShowDialog() == true)
            {
                Stream imageStreamSource = new FileStream(openFileDialogue.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                BitmapDecoder decoder;
                #region Decoding
                string extension = System.IO.Path.GetExtension(openFileDialogue.FileName);
                switch (extension.ToLower())
                {
                    case ".jpeg":
                        decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    case ".png":
                        decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    case ".gif":
                        decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    case ".tiff":
                        decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    case ".wmf":
                        decoder = new WmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    case ".bmp":
                        decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        break;
                    default:
                        return;
                }
                #endregion
                BitmapSource LoadedBitmap = decoder.Frames[0];
                pasteCanvas.Width = LoadedBitmap.Width;
                pasteCanvas.Height = LoadedBitmap.Height;
                pasteCanvas.Children.Add(new Image() { Source = LoadedBitmap });
            }
        }
        #endregion

        public static class util
        {
            public static void SaveWindow(Window window, int dpi, string filename)
            {
                var rtb = new RenderTargetBitmap(
                    (int)window.Width, //width 
                    (int)window.Width, //height 
                    dpi, //dpi x 
                    dpi, //dpi y 
                    PixelFormats.Pbgra32// pixelformat 
                    );
                rtb.Render(window);
                SaveRTBAsPNG(rtb, filename);

            }

            //Lưu paintCanvas
            public static void SaveCanvas(Window window, Canvas canvas, int dpi, string filename)
            {
                Size size = new Size(window.Width, window.Height);
                canvas.Measure(size);

                var rtb = new RenderTargetBitmap(
                    (int)window.Width, //width 
                    (int)window.Height, //height 
                    dpi, //dpi x 
                    dpi, //dpi y 
                    PixelFormats.Pbgra32 // pixelformat 
                    );
                rtb.Render(canvas);
                SaveRTBAsPNG(rtb, filename);
            }

            //Lưu fullCanvas
            public static void SaveGridCanvas(Window window, Grid canvas, int dpi, string filename)
            {
                Size size = new Size(window.Width, window.Height);
                canvas.Measure(size);

                var rtb = new RenderTargetBitmap(
                    (int)window.Width, //width 
                    (int)window.Height, //height 
                    dpi, //dpi x 
                    dpi, //dpi y 
                    PixelFormats.Pbgra32 // pixelformat 
                    );
                rtb.Render(canvas);
                SaveRTBAsPNG(rtb, filename);
            }
            private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
            {
                var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

                using (var stm = System.IO.File.Create(filename))
                {
                    enc.Save(stm);
                }
            }

        }

        private void buttonText_Click(object sender, RoutedEventArgs e)
        {
            Textbox2D txb = new Textbox2D();
            _selectedShapeName = txb.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;

            //Xóa đi bảng chọn size, outline và thêm vào bảng chọn font, size, style cho text
            ChooseStyleStack.Children.Clear();

            GroupBoxTextStyle();
            _preview.s_Style = _selectedStyle;
        }

        
    }
}
