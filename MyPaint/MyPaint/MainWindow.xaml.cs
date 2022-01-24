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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            //paintCanvas.MouseEnter += new MouseEventHandler(paint_MouseEnter);
            //paintCanvas.MouseWheel += new MouseWheelEventHandler(paint_MouseWheel);
        }

        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        Dictionary<string, int> packIconKind = new Dictionary<string, int>();
        bool _isDrawing = false;
        bool _isPicker = false;
        List<IShape> _shapes = new List<IShape>();
        IShape _preview;
        string _selectedShapeName = "";
        Brush _selectedmColor; //m là main color
        Brush _selectedsColor; //s là sub color
        int _selectedSize;
        bool mainColorSelected = true;
        List<Outline> _outlines = new List<Outline>();
        DoubleCollection _selectedOutline;
        List<FillColor> _fill = new List<FillColor>();
        Brush _selectedFill;
        FontFamily _selectedFontFamily;
        double _selectedFontSize;
        FontWeight _selectedFontWeight;
        FontStyle _selectedFontStyle;
        int _selectedTextDecoration;
        FillColor fillcolor = new FillColor();
        AdornerLayer _adnrLayer;
        Adorner _adCanvas;
        bool _isWriting = false;
        List<IShape> _bufferShapes = new List<IShape>();
        Point _startPoint;
        private double _zoomValue = 1.0;

        private void PaintCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _adnrLayer = AdornerLayer.GetAdornerLayer(paintCanvas);
            _adCanvas = new CanvasAdorner(paintCanvas);
            _adnrLayer.Add(_adCanvas);
        }

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
                button.Height = 20;
                button.Width = 20;
                button.Margin = new Thickness(2);
                button.Background = new SolidColorBrush((Color)color.GetValue(null, null));
                button.Focusable = false;

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

            //Thêm select vào danh sách
            Select2D select = new Select2D();
            _prototypes.Add(select.Name, select);

            // Thêm Image vào danh sách
            Image2D image = new Image2D();
            _prototypes.Add(image.Name, image);

            //Cấu hình thông số ban đầu
            _selectedShapeName = curve.Name;
            _selectedmColor = new SolidColorBrush(Colors.Black);
            _selectedsColor = new SolidColorBrush(Colors.White);
            _selectedSize = 2;
            _selectedOutline = null;
            _selectedFill = null;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;

            //Thêm outline vào danh sách
            _outlines.Add(new Outline() { Name = "Solid", Value = null });
            _outlines.Add(new Outline() { Name = "Dash", Value = new DoubleCollection() { 3, 4 } });
            _outlines.Add(new Outline() { Name = "Dot", Value = new DoubleCollection() { 1, 2 } });
            _outlines.Add(new Outline() { Name = "Dash Dot", Value = new DoubleCollection() { 4, 1, 1, 1 } });
            OutlineCbbox.ItemsSource = _outlines;

            //AdornerLayer adnrLayer = AdornerLayer.GetAdornerLayer(fullCanvas);
            //adnrLayer.Add(new CanvasAdorner(paintCanvas));
            //paintCanvas.Focus();
        }


        private void _mainRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChooseFill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fill = ChooseFill.SelectedValue as Fluent.GalleryItem;

            fillcolor.Name = fill.Tag as string;
            switch (fillcolor.Name)
            {
                case "Solid":
                    fillcolor.Value = new SolidColorBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color);
                    break;
                case "Linear":
                    fillcolor.Value = new LinearGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color, 1);
                    break;
                case "Radial":
                    fillcolor.Value = new RadialGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color);
                    break;
                default:
                    fillcolor.Value = Brushes.Transparent;
                    break;
            }
            _selectedFill = fillcolor.Value;
            _preview.s_Fill = _selectedFill;

            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                _shapes[lastIndex].s_Fill = _selectedFill;

                //Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                //Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
            //var outline = (OutlineCbbox.SelectedValue as Outline);
            //_selectedOutline = outline.Value;
            //_preview.s_Outline = _selectedOutline;
        }

        private void ChooseSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = ChooseSize.SelectedValue as Fluent.GalleryItem;
            _selectedSize = Int32.Parse(size.Tag as string);
            _preview.s_mThickness = _selectedSize;

            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                _shapes[lastIndex].s_mThickness = _selectedSize;

                //Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                //Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.Clipboard.ContainsImage())
            {
                System.Windows.Forms.IDataObject clipboardData = System.Windows.Forms.Clipboard.GetDataObject();
                if (clipboardData != null)
                {
                    if (clipboardData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                    {
                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)clipboardData.GetData(System.Windows.Forms.DataFormats.Bitmap);
                        
                        if(bitmap.Width > paintCanvas.Width) paintCanvas.Width = bitmap.Width + 10;
                  
                        if(bitmap.Height > paintCanvas.Height) paintCanvas.Height = bitmap.Height + 10;
                        
                        var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        var pasteImg = new Image() { Source = source };

                        if (pasteImg != null)
                        {
                            Image2D image2D = new Image2D();
                            image2D.image = pasteImg;
                            image2D.adnrLayer = _adnrLayer;
                            _shapes.Add(image2D);

                            image2D.HandleStart(10, 10);
                            image2D.HandleMove(10 + bitmap.Width, 10 + bitmap.Height);
                            image2D.Draw(paintCanvas);
                            image2D.HandleEnd(10 + bitmap.Width, 10 + bitmap.Height);
                        }
                    }
                }
            }
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            Select2D select = new Select2D();
            _selectedShapeName = select.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;
        }

        private void ChooseShapeButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedShapeName = (sender as Button).Tag as string;

            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;
            _preview.s_sColor = _selectedsColor;
            _preview.s_mThickness = _selectedSize;
            _preview.s_Outline = _selectedOutline;
            _preview.s_Fill = _selectedFill;
            _isPicker = false;

            if(_isWriting == true)
            {
                //Trả lại bảng chọn size và outline
                ChooseStyleStack.Children.Clear();
                ChooseStyleStack.Orientation = Orientation.Horizontal;
                ChooseStyleStack.Children.Add(OutlineCbbox);
                ChooseStyleStack.Children.Add(ChooseSizeButton);

                _isWriting = false;
            }
        }

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = (sender as Button).Background;

            if (mainColorSelected)
            {
                mainColor.Background = color;
                _preview.s_mColor = mainColor.Background;
                _preview.s_sColor = subColor.Background;
                _selectedmColor = _preview.s_mColor;
                _selectedsColor = _preview.s_sColor;
                switch (fillcolor.Name)
                {
                    case "Solid":
                        fillcolor.Value = new SolidColorBrush(((System.Windows.Media.SolidColorBrush)_selectedsColor).Color);
                        break;
                    case "Linear":
                        fillcolor.Value = new LinearGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color, 1);
                        break;
                    case "Radial":
                        fillcolor.Value = new RadialGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color);
                        break;
                    default:
                        fillcolor.Value = Brushes.Transparent;
                        break;
                }
                _preview.s_Fill = fillcolor.Value;
                _selectedFill = fillcolor.Value;

                int lastIndex = _shapes.Count - 1;
                if (lastIndex >= 0)
                {
                    _shapes[lastIndex].s_Fill = _selectedFill;
                    _shapes[lastIndex].s_mColor = _selectedmColor;
                    _shapes[lastIndex].s_sColor = _selectedsColor;

                    //Ve lai Xoa toan bo
                    paintCanvas.Children.Clear();

                    //Ve lai tat ca cac hinh
                    foreach (var shape in _shapes)
                    {
                        shape.Draw(paintCanvas);
                    }
                }
            }
            else
            {
                subColor.Background = color;
                _preview.s_mColor = mainColor.Background;
                _preview.s_sColor = subColor.Background;
                _selectedmColor = mainColor.Background;
                _selectedsColor = subColor.Background;
                switch (fillcolor.Name)
                {
                    case "Solid":
                        fillcolor.Value = new SolidColorBrush(((System.Windows.Media.SolidColorBrush)_selectedsColor).Color);
                        break;
                    case "Linear":
                        fillcolor.Value = new LinearGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color, 1);
                        break;
                    case "Radial":
                        fillcolor.Value = new RadialGradientBrush(((System.Windows.Media.SolidColorBrush)_selectedmColor).Color, ((System.Windows.Media.SolidColorBrush)_selectedsColor).Color);
                        break;
                    default:
                        fillcolor.Value = Brushes.Transparent;
                        break;
                }
                _preview.s_Fill = fillcolor.Value;
                _selectedFill = fillcolor.Value;

                int lastIndex = _shapes.Count - 1;
                if (lastIndex >= 0)
                {
                    _shapes[lastIndex].s_Fill = _selectedFill;
                    _shapes[lastIndex].s_mColor = _selectedmColor;
                    _shapes[lastIndex].s_sColor = _selectedsColor;

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

        private void Outline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var outline = (OutlineCbbox.SelectedValue as Outline);
            _selectedOutline = outline.Value;
            _preview.s_Outline = _selectedOutline;

            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                _shapes[lastIndex].s_Outline = _selectedOutline;

                //Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                //Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void paint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.Space))
            {
                if (_isPicker)
                {
                    _isDrawing = false;
                    Point ptClicked = e.GetPosition(paintCanvas);

                    if (e.LeftButton.Equals(MouseButtonState.Pressed))
                    {
                        Color pxlColor = GetPixelColor(paintCanvas, ptClicked);
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = (Brush)converter.ConvertFromString(pxlColor.ToString());
                        MessageBox.Show("HEX: " + brush.ToString());
                        mainColor.Background = brush;
                        _selectedmColor = brush;
                        _preview.s_mColor = brush;
                    }
                } else {
                    _isDrawing = true;
                    _isPicker = false;
                    Point pos = e.GetPosition(paintCanvas);
                    _preview.HandleStart(pos.X, pos.Y);
                    _startPoint = new Point(pos.X, pos.Y);

                    if(_selectedShapeName == "Select")
                    {
                        cutButton.IsEnabled = true;
                        copyButton.IsEnabled = true;
                        Crop.IsEnabled = true;
                    }
                }
            }
        }

        private void paint_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(paintCanvas);
            if (!Keyboard.IsKeyDown(Key.Space))
            {
                 if (_isDrawing) {
                    _isPicker = false;
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
                }
            }
            
            Coordinates.Text = $"{Math.Round(pos.X)}, {Math.Round(pos.Y)}";
        }

        private void paint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            Point pos = e.GetPosition(paintCanvas);
            if(pos.X != _startPoint.X && pos.Y != _startPoint.Y)
            {
                // Thêm đối tượng cuối cùng vào mảng quản lí        
                _shapes.Add(_preview);
                //Shape currShape = (Shape)_preview;

                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }

                //Gọi hàm xử lý kết thúc cho đối tượng cuối cùng
                _preview.HandleEnd(pos.X, pos.Y);

                // Sinh ra đối tượng mẫu kế
                _preview = _prototypes[_selectedShapeName].Clone();
                _preview.s_mColor = _selectedmColor;
                _preview.s_sColor = _selectedsColor;
                _preview.s_mThickness = _selectedSize;
                _preview.s_Outline = _selectedOutline;
                _preview.s_Fill = _selectedFill;
                _preview.s_FontFamily = _selectedFontFamily;
                _preview.s_FontSize = _selectedFontSize;
                _preview.s_FontWeight = _selectedFontWeight;
                _preview.s_FontStyle = _selectedFontStyle;
                _preview.s_TextDecoration = _selectedTextDecoration;

                //Enable undo
                if (_shapes.Count > 0)
                {
                    undoButton.IsEnabled = true;
                }
            }
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
            _isPicker = false;
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

                int lastIndex = _shapes.Count - 1;
                if (lastIndex >= 0)
                {
                    _shapes[lastIndex].s_mColor = _selectedmColor;
                    _shapes[lastIndex].s_sColor = _selectedsColor;

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

        #region newFile
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            var ans = MessageBox.Show("Save this masterpiece?", "MyPaint", MessageBoxButton.YesNoCancel);

            if (ans == MessageBoxResult.Yes)
            {
                dlg.FileName = "mypaint"; // Default file name
                dlg.DefaultExt = ".jpeg";
                dlg.Filter = "Jpeg Files (*.jpeg)|*.jpeg|Png Files (*.png)|*.png|Canvas (.cvs)|*.cvs|Bitmap (.bmp)|*.bmp"; // Filter files by extension
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename_tmp = dlg.FileName;
                }
                save_filename = dlg.FileName.ToString();
            }
            else if (ans == MessageBoxResult.No)
            {
                _shapes.Clear();
                //pasteCanvas.Children.Clear();
                paintCanvas.Children.Clear();
            }

        }
        #endregion

        #region saveFile
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        string save_filename;
        private void CreateSaveDialog()
        {
            dlg.FileName = "mypaint"; // Default file name
            dlg.DefaultExt = ".jpeg";
            dlg.Filter = "Jpeg Files (*.jpeg)|*.jpeg|Png Files (*.png)|*.png|Canvas (.cvs)|*.cvs|Bitmap (.bmp)|*.bmp"; // Filter files by extension
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
            //util.SaveGridCanvas(this, fullCanvas, 96, save_filename);
            util.SaveCanvas(this, paintCanvas, 96, save_filename);
        }
        #endregion

        #region loadFile
        Microsoft.Win32.OpenFileDialog dlg_open = new Microsoft.Win32.OpenFileDialog();
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogue = new OpenFileDialog()
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Gif Image (.gif)|*.gif|Bitmap Image (.bmp)|*.bmp"
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
                        //decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        decoder = BitmapDecoder.Create(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default); //nếu lỗi thì dùng dòng trên
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
                paintCanvas.Width = LoadedBitmap.Width;
                paintCanvas.Height = LoadedBitmap.Height;
                Image pasteImg = new Image() { Source = LoadedBitmap };
                if (pasteImg != null)
                {
                    Image2D image2D = new Image2D();
                    image2D.image = pasteImg;
                    image2D.adnrLayer = _adnrLayer;
                    _shapes.Add(image2D);

                    image2D.HandleStart(10, 10);
                    image2D.HandleMove(10 + LoadedBitmap.Width, 10 + LoadedBitmap.Height);
                    image2D.Draw(paintCanvas);
                    //image2D.HandleEnd(10 + LoadedBitmap.Width, 10 + LoadedBitmap.Height);
                }
            }
        }
        #endregion

        private void buttonText_Click(object sender, RoutedEventArgs e)
        {
            _isWriting = true;
            Textbox2D txb = new Textbox2D();
            _selectedShapeName = txb.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.s_mColor = _selectedmColor;

            //Xóa đi bảng chọn size, outline và thêm vào bảng chọn font, size, style cho text
            ChooseStyleStack.Children.Clear();

            //_selectedStyle = new List<int>();

            GroupBoxTextStyle();
            _preview.s_FontFamily = _selectedFontFamily;
            _preview.s_FontSize = _selectedFontSize;
            _preview.s_FontWeight = _selectedFontWeight;
            _preview.s_FontStyle = _selectedFontStyle;
            _preview.s_TextDecoration = _selectedTextDecoration;
        }

        // new
        public static Color GetPixelColor(Visual visual, Point pt)
        {
            Point ptDpi = getScreenDPI(visual);

            Size srcSize = VisualTreeHelper.GetDescendantBounds(visual).Size;

            //Viewbox uses values between 0 & 1 so normalize the Rect with respect to the visual's Height & Width
            Rect percentSrcRec = new Rect(pt.X / srcSize.Width, pt.Y / srcSize.Height,
                                          1 / srcSize.Width, 1 / srcSize.Height);

            var bmpOut = new RenderTargetBitmap(1, 1, 96d, 96d, PixelFormats.Pbgra32); //assumes 96 dpi
            //var bmpOut = new RenderTargetBitmap((int)(ptDpi.X / 96d),
            //                                    (int)(ptDpi.Y / 96d),
            //                                    ptDpi.X, ptDpi.Y, PixelFormats.Default); //generalized for monitors with different dpi

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrcRec },
                                 null, //no Pen
                                 new Rect(0, 0, 1d, 1d));
            }
            bmpOut.Render(dv);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);

            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static Point getScreenDPI(Visual v)
        {
            //System.Windows.SystemParameters
            PresentationSource source = PresentationSource.FromVisual(v);
            Point ptDpi;
            if (source != null)
            {
                ptDpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11,
                                   96.0 * source.CompositionTarget.TransformToDevice.M22);
            }
            else
                ptDpi = new Point(96d, 96d); //default value.

            return ptDpi;
        }

        private void buttonEyedrop_Click(object sender, RoutedEventArgs e)
        {
            _isPicker = true;
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            redoButton.IsEnabled = true;
            int lastIndex = _shapes.Count - 1;
            if(lastIndex >= 0)
            {
                _bufferShapes.Add(_shapes[lastIndex]);
                _shapes.RemoveAt(lastIndex);

                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }

                if (lastIndex == 0) undoButton.IsEnabled = false;
            }
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            undoButton.IsEnabled = true;
            int lastIndex = _bufferShapes.Count - 1;
            if(lastIndex >= 0)
            {
                _shapes.Add(_bufferShapes[lastIndex]);
                _bufferShapes.RemoveAt(lastIndex);

                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }

                if (lastIndex == 0) redoButton.IsEnabled = false;
            }
        }

        private void DockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                undoButton_Click(sender, e);
            }
            else if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                redoButton_Click(sender, e);
            }
            else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && copyButton.IsEnabled == true)
            {
                copyButton_Click(sender, e);
            }
            else if (e.Key == Key.X && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && cutButton.IsEnabled == true)
            {
                cutButton_Click(sender, e);
            }
            else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && pasteButton.IsEnabled == true)
            {
                pasteButton_Click(sender, e);
            }
            else if (e.Key == Key.I)
            {
                buttonEyedrop_Click(sender, e);
            }
        }

        private void cutButton_Click(object sender, RoutedEventArgs e)
        {
            undoButton.IsEnabled = true;
            cutButton.IsEnabled = false;
            copyButton.IsEnabled = false;
            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                var _copyShape = _shapes[lastIndex];
                _shapes.RemoveAt(lastIndex);

                if (_copyShape.Name == "Select")
                {
                    var currentSelect = _copyShape as Select2D;

                    //MemoryStream ms = new MemoryStream();
                    //System.Windows.Media.Imaging.BmpBitmapEncoder bbe = new BmpBitmapEncoder();
                    //bbe.Frames.Add(currentSelect._image.Source as BitmapFrame);
                    //bbe.Save(ms);
                    //System.Drawing.Image img2 = System.Drawing.Image.FromStream(ms);
                    //System.Windows.Forms.Clipboard.SetImage(img2);

                    CanvasUltilities.CopyUIElementToClipboard(currentSelect.image as FrameworkElement);

                    //currentSelect._imageFinal = null;
                    currentSelect.image = null;
                    _shapes.Add(currentSelect);
                }
                
                // Ve lai Xoa toan bo
                paintCanvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in _shapes)
                {
                    shape.Draw(paintCanvas);
                }
            }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            undoButton.IsEnabled = true;
            copyButton.IsEnabled = false;
            cutButton.IsEnabled = false;
            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                var _copyShape = _shapes[lastIndex];
                _shapes.RemoveAt(lastIndex);

                if (_copyShape.Name == "Select")
                {
                    var currentSelect = _copyShape as Select2D;

                    CanvasUltilities.CopyUIElementToClipboard(currentSelect.imageFinal as FrameworkElement);

                    //currentSelect._imageFinal = null;
                    //currentSelect._image = null;
                    _shapes.Add(currentSelect);
                }
                
            }
        }

        private void saveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Binary File (*.dat) | *.dat";

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName;
                FileStream stream = new FileStream(path, FileMode.Create);

                foreach (var shape in _shapes)
                {
                    //formatter.Serialize(stream, shape);
                }

                stream.Close();
            }
        }

        private void fullCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
                var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);
                foreach (var ext in lst.Select((f) => System.IO.Path.GetExtension(f)))
                {
                    if (!validExtensions.Contains(ext))
                        System.Windows.MessageBox.Show("Can't drop this file!! Try with another file.", "File not true", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                foreach (var file in files)
                {
                    BitmapImage tmpImage = new BitmapImage((new Uri(file)));
                    var pasteImg = new Image() { Source = tmpImage };

                    if (pasteImg != null)
                    {
                        Image2D image2D = new Image2D();
                        image2D.image = pasteImg;
                        image2D.adnrLayer = _adnrLayer;
                        _shapes.Add(image2D);

                        if(tmpImage.Width > paintCanvas.Width) paintCanvas.Width = tmpImage.Width + 10;
                        if(tmpImage.Height > paintCanvas.Height) paintCanvas.Height = tmpImage.Height + 10;

                        image2D.HandleStart(10, 10);
                        image2D.HandleMove(tmpImage.Width, tmpImage.Height);
                        image2D.Draw(paintCanvas);
                        //image2D.HandleEnd(tmpImage.Width, tmpImage.Height);
                    }
                }               
            }
        }

        private void paintBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) {
                if (e.Delta > 0)
                {
                    _zoomValue += 0.1;
                }
                else
                {
                    _zoomValue -= 0.1;
                }

                ScaleTransform scale = new ScaleTransform(_zoomValue, _zoomValue);
                if(scale.ScaleX >= 0.1)
                    paintBorder.LayoutTransform = scale;
                e.Handled = true;
            }
        }

        private void paintCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = paintCanvas.Width;
            double height = paintCanvas.Height;
            CanvasSize.Text = $"{Math.Round(width)} x {Math.Round(height)} px";

            fullCanvas.Width = width;
            fullCanvas.Height = height;
            CanvasBorder.Width = width;
            CanvasBorder.Height = height;
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            undoButton.IsEnabled = true;
            copyButton.IsEnabled = false;
            cutButton.IsEnabled = false;
            Crop.IsEnabled = false;
            int lastIndex = _shapes.Count - 1;
            if (lastIndex >= 0)
            {
                var _copyShape = _shapes[lastIndex];
                _shapes.Clear();
                paintCanvas.Children.Clear();

                if (_copyShape.Name == "Select")
                {
                    var currentSelect = _copyShape as Select2D;
                    paintCanvas.Width = currentSelect.imageFinal.Width;
                    paintCanvas.Height = currentSelect.imageFinal.Height;

                    _shapes.Add(currentSelect);
                    _preview = currentSelect;
                    Canvas.SetLeft(currentSelect.imageFinal, 0);
                    Canvas.SetTop(currentSelect.imageFinal, 0);
                    paintCanvas.Children.Add(currentSelect.imageFinal);
                }

            }
        }
    }

    public static class util
    {
        public static void SaveWindow(Window window, int dpi, string filename)
        {
            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width 
                (int)window.Height, //height 
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
            //Size size = new Size(window.Width, window.Height);
            //canvas.Measure(size);

            var rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth, //width 
                (int)canvas.ActualHeight, //height 
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
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);

            var rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth, //width 
                (int)canvas.ActualHeight, //height 
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
}
