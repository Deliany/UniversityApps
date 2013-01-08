using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using iDraw.Helpers;

namespace iDraw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool drawing = false;
        DrawingArea drawArea;

        public MainWindow()
        {
            InitializeComponent();

            ModalDialog.SetParent(this.DrawingCanvas);

            drawArea = new DrawingArea(this.DrawingCanvas);
            drawArea.Tool = CurrentTool.Pencil;
            drawArea.StrokeColor = new SolidColorBrush(Colors.YellowGreen);
            drawArea.FillColor = new SolidColorBrush(Colors.White);
            drawArea.StrokeWidth = 5;
            this.DataContext = drawArea;
        }


        #region Events

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            this.ColorPanel.Visibility = Visibility.Collapsed;
        }

        private void ButtonTool_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null && btn.Tag is string)
            {
                drawArea.Tool = (CurrentTool)Enum.Parse(typeof(CurrentTool), btn.Tag as string, true);
            }
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawArea.PrevPoint = e.GetPosition(this.DrawingCanvas);
            drawArea.StartPoint = drawArea.PrevPoint;
            drawArea.TempHolder.Clear();

            if (drawing)
                drawing = false;
            else
            {
                drawing = true;
                e.Handled = true;
            }
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drawing = false;
            var cupt = e.GetPosition(this.DrawingCanvas);
            drawArea.HideVirtualLine();

            cupt = drawArea.DrawOnComplete(cupt);
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing && drawArea.PrevPoint != null)
            {
                var cupt = e.GetPosition(this.DrawingCanvas);
                cupt = drawArea.DrawOnMove(cupt);
            }

        }

        private void ButtonColor_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas)
            {
                this.ColorPanel.Visibility = Visibility.Visible;
                ColorPanel.Tag = sender;
                ColorPicker.ColorChanged += (s, c) =>
                {
                    (ColorPanel.Tag as Canvas).Background = c.newColor;
                };
            }
        }


        #endregion

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            drawArea.UndoShape();
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            drawArea.RedoShape();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            var date = DateTime.Now.ToString().Replace(":", "").Replace(".", "").Replace(" ", "");
            dlg.FileName = "sketch_" + date;

            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dlg.Title = "Save your image";
            dlg.DefaultExt = "png";
            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                string savePath = dlg.FileName;
                try
                {
                    var uriPath = new Uri(savePath);
                    ExportToPng(uriPath,this.DrawingCanvas);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error while saving file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }


        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                string imageFile = dlg.FileName;
                try
                {
                    Uri imgsource = new Uri(imageFile);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = imgsource;
                    bitmapImage.EndInit();
                    Image img = new Image();
                    img.Source = bitmapImage;
                    drawArea.Clear();
                    this.DrawingCanvas.Width = bitmapImage.Width;
                    this.DrawingCanvas.Height = bitmapImage.Height;
                    this.DrawingCanvas.Children.Add(img);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error while opening file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void ExportToPng(Uri path, Canvas surface)
        {
            if (path == null) return;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var handled = ModalDialog.ShowHandlerDialog();
                if (handled)
                {
                    // ----
                    drawArea.Clear();
                    this.DrawingCanvas.Width = int.Parse(ModalDialog.WidthTextBox.Text);
                    this.DrawingCanvas.Height = int.Parse(ModalDialog.HeightTextBox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while creating file", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyUIElementToClipboard(this.DrawingCanvas);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while copying to clipboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ImageSource ImageFromClipboardDib()
        {
            MemoryStream ms = Clipboard.GetData("DeviceIndependentBitmap") as MemoryStream;
            if (ms != null)
            {
                byte[] dibBuffer = new byte[ms.Length];
                ms.Read(dibBuffer, 0, dibBuffer.Length);

                BITMAPINFOHEADER infoHeader =
                    BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

                int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
                int infoHeaderSize = infoHeader.biSize;
                int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

                BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();
                fileHeader.bfType = BITMAPFILEHEADER.BM;
                fileHeader.bfSize = fileSize;
                fileHeader.bfReserved1 = 0;
                fileHeader.bfReserved2 = 0;
                fileHeader.bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4;

                byte[] fileHeaderBytes =
                    BinaryStructConverter.ToByteArray<BITMAPFILEHEADER>(fileHeader);

                MemoryStream msBitmap = new MemoryStream();
                msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
                msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
                msBitmap.Seek(0, SeekOrigin.Begin);

                return BitmapFrame.Create(msBitmap);
            }
            return null;
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var img = new Image();
                img.Source = ImageFromClipboardDib();
                this.DrawingCanvas.Children.Add(img);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while pasting from clipboard", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
