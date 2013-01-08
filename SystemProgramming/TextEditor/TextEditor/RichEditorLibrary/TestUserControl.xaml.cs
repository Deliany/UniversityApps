using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;

namespace Adastra.RichEditorLibrary
{
    /// <summary>
    /// Interaction logic for TestUserControl.xaml
    /// </summary>
    public partial class TestUserControl : UserControl, IRibbonControl
    {
        public static readonly DependencyProperty ColorProperty;
        internal static bool KeepPreviewColor { get; set; }
        internal static double BoxWidth { get; set; }
        internal static double BoxHeight { get; set; }

        static TestUserControl()
        {
            PropertyMetadata metaData;

            metaData = new PropertyMetadata(Colors.Red);
            ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(TestUserControl),metaData);
        }

        public TestUserControl()
        {
            InitializeComponent();

            return;
            KeepPreviewColor = false;
            ColorsBox.ItemsSource = typeof(Colors).GetProperties();

            #region Binding Color to sliders

            MultiBinding mb = new MultiBinding();
            mb.Converter = new RgbToColorConverter();
            mb.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            mb.Mode = BindingMode.TwoWay;

            Binding bb = new Binding("Value");
            bb.ElementName = "R";
            bb.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(bb);

            bb = new Binding("Value");
            bb.ElementName = "G";
            bb.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(bb);

            bb = new Binding("Value");
            bb.ElementName = "B";
            bb.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(bb);

            this.SetBinding(ColorProperty, mb);

            #endregion
        }

        private void ColorsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Color = (Color)ColorConverter.ConvertFromString(((System.Reflection.PropertyInfo)ColorsBox.SelectedItem).Name);

        }

        #region BWLayer Mouse handlers

        private void BWLayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            BWLayer.CaptureMouse();

            SetColorFromBWLayer(e);
            // Do not update Preview Color when clicking on BWLayer
            TestUserControl.KeepPreviewColor = true;
        }

        private void BWLayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!BWLayer.IsMouseCaptured)
            {
                return;
            }

            SetColorFromBWLayer(e);
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            return;
            BoxHeight = BWLayer.ActualHeight;
            BoxWidth = BWLayer.ActualWidth;
        }

        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        private void BWLayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            BWLayer.ReleaseMouseCapture();
            TestUserControl.KeepPreviewColor = false;
        }

        /// <summary>
        /// Approximate selected color under mouse pointer on the BWLayer.
        /// </summary>
        /// <param name="e"></param>
        private void SetColorFromBWLayer(MouseEventArgs e)
        {
            LinearGradientBrush br = (LinearGradientBrush)ColorLayer.Fill;
            GradientStop Co0 = br.GradientStops[1];
            GradientStop Co1 = br.GradientStops[0];

            float y = 1 - (float)(e.GetPosition(BWLayer).Y / BWLayer.ActualHeight);
            float x = 1 - (float)(e.GetPosition(BWLayer).X / BWLayer.ActualWidth);
            if (y > 1) { y = 1; }
            if (y < 0) { y = 0; }
            if (x > 1) { x = 1; }
            if (x < 0) { x = 0; }

            float rVal = y * ((Co1.Color.R - Co0.Color.R) * x + Co0.Color.R);
            float gVal = y * ((Co1.Color.G - Co0.Color.G) * x + Co0.Color.G);
            float bVal = y * ((Co1.Color.B - Co0.Color.B) * x + Co0.Color.B);

            Color = Color.FromRgb((byte)rVal, (byte)gVal, (byte)bVal);
        }

        #endregion

        #region Rainbow Mouse handlers

        private void Rainbow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            Rainbow.CaptureMouse();

            SetColorFromRainbow(e);
        }

        private void Rainbow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Rainbow.IsMouseCaptured)
            {
                return;
            }

            SetColorFromRainbow(e);
        }

        private void Rainbow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            if (!Rainbow.IsMouseCaptured)
            {
                return;
            }
            Rainbow.ReleaseMouseCapture();
        }

        /// <summary>
        /// Approximate Color under mouse pointer on the Rainbow rectangle.
        /// </summary>
        /// <param name="e"></param>
        private void SetColorFromRainbow(MouseEventArgs e)
        {
            LinearGradientBrush br = (LinearGradientBrush)Rainbow.Fill;
            GradientStopCollection gs = br.GradientStops;

            float y = (float)(e.GetPosition(Rainbow).Y / Rainbow.ActualHeight);
            if (y > 1) { y = 1; }
            if (y < 0) { y = 0; }

            float rVal = 0;
            float gVal = 0;
            float bVal = 0;
            for (int i = 1; i < gs.Count; i++)
            {
                if (y <= gs[i].Offset)
                {
                    y = (float)((y - gs[i - 1].Offset) / Math.Abs(gs[i].Offset - gs[i - 1].Offset));
                    rVal = (gs[i].Color.R - gs[i - 1].Color.R) * y + gs[i - 1].Color.R;
                    gVal = (gs[i].Color.G - gs[i - 1].Color.G) * y + gs[i - 1].Color.G;
                    bVal = (gs[i].Color.B - gs[i - 1].Color.B) * y + gs[i - 1].Color.B;
                    break;
                }
            }

            Color c = Color.FromRgb((byte)rVal, (byte)gVal, (byte)bVal);
            byte[] pixels = new byte[3] { c.R, c.G, c.B };

            // fix for setting PreviewColor, when all parts of Color are equal
            if ((Color.R == Color.G) && (Color.G == Color.B))
            {
                PreviewColor.Color = c;
                return;
            }

            byte max = (byte)Math.Max(Color.R, Math.Max(Color.G, Color.B));
            byte min = (byte)Math.Min(Color.R, Math.Min(Color.G, Color.B));

            for (int i = 0; i < 3; i++)
            {
                if (pixels[i] == 255)
                {
                    pixels[i] = max;
                }
                else
                {
                    if (pixels[i] == 0)
                    {
                        pixels[i] = min;
                    }
                    else
                    {
                        double coef = (double)pixels[i] / 255;
                        pixels[i] = (byte)(min + (max - min) * coef);
                    }
                }
            }

            Color = Color.FromRgb(pixels[0], pixels[1], pixels[2]);
        }

        #endregion

    }
}
