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
using System.Globalization;
using Microsoft.Windows.Controls.Ribbon;
using System.Reflection;

namespace Adastra.RichEditorLibrary
{
    public delegate void ColorChangedEventHandler(Color color);

    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPick : UserControl, IRibbonControl
    {
        #region | Events |

        public event ColorChangedEventHandler ColorChanged;

        #endregion //| Events |

        public static readonly DependencyProperty ColorProperty;

        public ColorPick()
        {
            InitializeComponent();

            KeepPreviewColor = false;
            this.m_cbxColorsBox.ItemsSource = typeof(Colors).GetProperties();

            for (int i = 0; i < this.m_cbxColorsBox.Items.Count;i++)
            {
                var varObj = this.m_cbxColorsBox.Items[i];
                if ("Red" == ((PropertyInfo)varObj).Name)
                {
                    this.m_cbxColorsBox.SelectedIndex = i;
                    break;
                }
            }

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

        static ColorPick()
        {
            PropertyMetadata metaData;

            metaData = new PropertyMetadata(Colors.Red);
            ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorPick),metaData);
        }

        internal static bool KeepPreviewColor { get; set; }
        internal static double BoxWidth { get; set; }
        internal static double BoxHeight { get; set; }

        public Color ColorAutomatic { get; set; }

        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
                if (null != this.ColorChanged)
                {
                    this.ColorChanged(value);
                }
            }
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            BoxHeight = BWLayer.ActualHeight;
            BoxWidth = BWLayer.ActualWidth;
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
            ColorPick.KeepPreviewColor = true;
        }

        private void BWLayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!BWLayer.IsMouseCaptured)
            {
                return;
            }

            SetColorFromBWLayer(e);
        }

        private void BWLayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            BWLayer.ReleaseMouseCapture();
            ColorPick.KeepPreviewColor = false;
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

        private void ColorsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Color = (Color)ColorConverter.ConvertFromString(((System.Reflection.PropertyInfo)m_cbxColorsBox.SelectedItem).Name);

        }

        private void OnAutomaticButtonClick(object sender, RoutedEventArgs e)
        {
            Color = this.ColorAutomatic;
        }
    }

    [ValueConversion(typeof(double), typeof(byte))]
    public class DoubleToByteConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            return (byte)(double)value;
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            return (double)value;
        }
    }

    [ValueConversion(typeof(double), typeof(byte))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }

    [ValueConversion(typeof(Color), typeof(string))]
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            Color c;
            if (value is Color)
            {
                c = (Color)value;
            }
            else
            {
                c = (Color)ColorConverter.ConvertFromString(value.ToString());
            }
            string s = string.Empty;

            if (c.R < 16) { s += '0'; }
            s += c.R.ToString("X");
            if (c.G < 16) { s += '0'; }
            s += c.G.ToString("X");
            if (c.B < 16) { s += '0'; }
            s += c.B.ToString("X");

            return s;
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            string s = (string)value;
            if (s.Length != 6)
            {
                return Binding.DoNothing;
            }

            byte[] d = new byte[3];
            try
            {
                d[0] = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
                d[1] = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
                d[2] = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber);
            }
            catch
            {
                return Binding.DoNothing;
            }

            return Color.FromRgb(d[0], d[1], d[2]);
        }
    }

    [ValueConversion(typeof(Color), typeof(Color))]
    public class PreviewColorConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            // Do not update PreviewColor, when picking color from BWLayer.
            if (ColorPick.KeepPreviewColor)
            {
                return Binding.DoNothing;
            }

            Color c = (Color)value;
            byte[] b = new byte[] { c.R, c.G, c.B };

            byte max = (byte)Math.Max(b[0], Math.Max(b[1], b[2]));
            byte min = (byte)Math.Min(b[0], Math.Min(b[1], b[2]));
            byte[] pixels = new byte[3];

            if (max == min)
            {
                return Binding.DoNothing;
            }

            for (int i = 0; i < 3; i++)
            {
                if (b[i] == max)
                {
                    pixels[i] = 255;
                }
                else
                {
                    if (b[i] == min)
                    {
                        pixels[i] = 0;
                    }
                    else
                    {
                        pixels[i] = (byte)(255 * (double)(b[i] - min) / (max - min));
                    }
                }
            }

            return Color.FromRgb(pixels[0], pixels[1], pixels[2]);
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            throw new Exception("Conversion not implemented.");
        }
    }

    public class RgbToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type typeTarget, object param, CultureInfo culture)
        {
            Color clr = Color.FromRgb((byte)(double)value[0], (byte)(double)value[1], (byte)(double)value[2]);

            if (typeTarget == typeof(Color))
            {
                return clr;
            }

            if (typeTarget == typeof(Brush))
            {
                return new SolidColorBrush(clr);
            }

            return Binding.DoNothing;
        }
        public object[] ConvertBack(object value, Type[] typeTarget, object param, CultureInfo culture)
        {
            Color clr;
            object[] primaries = new object[3];

            if (value is Color)
            {
                clr = (Color)value;
            }
            else
            {
                if (value is SolidColorBrush)
                {
                    clr = (value as SolidColorBrush).Color;
                }
                else
                {
                    return null;
                }
            }

            primaries[0] = (double)clr.R;
            primaries[1] = (double)clr.G;
            primaries[2] = (double)clr.B;
            return primaries;
        }
    }

    [ValueConversion(typeof(Color), typeof(Thickness))]
    public class ColorToPointerConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            Color c = (Color)value;
            Thickness t = new Thickness();

            byte max = (byte)Math.Max(c.R, Math.Max(c.G, c.B));
            byte min = (byte)Math.Min(c.R, Math.Min(c.G, c.B));
            if (max == 0)
            {
                return Binding.DoNothing;
            }

            t.Left = ColorPick.BoxWidth - ((double)min / max) * ColorPick.BoxWidth - 4;
            t.Top = ColorPick.BoxHeight - ((double)max / 255) * ColorPick.BoxHeight - 4;

            return t;
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            throw new Exception("Conversion not implemented.");
        }
    }

    [ValueConversion(typeof(Color), typeof(Thickness))]
    public class ColorToRainbowPointerConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            // Do not update RainbowPointer, when picking color from Gradient.
            if (ColorPick.KeepPreviewColor)
            {
                return Binding.DoNothing;
            }

            Color c = (Color)value;
            Thickness t = new Thickness();
            t.Left = -1;

            byte max = (byte)Math.Max(c.R, Math.Max(c.G, c.B));
            byte min = (byte)Math.Min(c.R, Math.Min(c.G, c.B));
            if (max == min)
            {
                return Binding.DoNothing;
            }

            if (c.R == max)
            {
                if (c.B > min)
                {
                    t.Top = ColorPick.BoxHeight * (0.833 + ((double)(max - c.B) / (max - min)) * 0.167) - 2;
                }
                else
                {
                    t.Top = ColorPick.BoxHeight * ((double)(c.G - min) / (max - min)) * 0.167 - 2;
                }
            }
            if (c.G == max)
            {
                if (c.R > min)
                {
                    t.Top = ColorPick.BoxHeight * (0.167 + ((double)(max - c.R) / (max - min)) * 0.166) - 2;
                }
                else
                {
                    t.Top = ColorPick.BoxHeight * (0.333 + ((double)(c.B - min) / (max - min)) * 0.167) - 2;
                }
            }

            if (c.B == max)
            {
                if (c.G > min)
                {
                    t.Top = ColorPick.BoxHeight * (0.5 + ((double)(max - c.G) / (max - min)) * 0.167) - 2;
                }
                else
                {
                    t.Top = ColorPick.BoxHeight * (0.667 + ((double)(c.R - min) / (max - min)) * 0.166) - 2;
                }
            }

            return t;
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            throw new Exception("Conversion not implemented.");
        }
    }
}
