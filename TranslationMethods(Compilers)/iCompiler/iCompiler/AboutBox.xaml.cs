using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iCompiler
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
            LoadHelp();
        }

        private void LoadHelp()
        {
            try
            {

                using (
                    Stream s = typeof (MainWindow).Assembly.GetManifestResourceStream("iCompiler.Samples.grammar.txt"))
                {
                    if (s == null)
                        throw new InvalidOperationException("Could not find embedded resource");
                    using (TextReader reader = new StreamReader(s))
                    {
                        richTextBox.AppendText(reader.ReadToEnd() + Environment.NewLine);
                    }
                }

                richTextBox.AppendText("========Example code:=============" + Environment.NewLine);

                using (
                    Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("iCompiler.Samples.sample_app.mnl"))
                {
                    if (s == null)
                        throw new InvalidOperationException("Could not find embedded resource");
                    using (TextReader reader = new StreamReader(s))
                    {
                        richTextBox.AppendText(reader.ReadToEnd() + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} >>> {1}", ex.GetType(), ex.Message),
                                "Error while loading resources", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
