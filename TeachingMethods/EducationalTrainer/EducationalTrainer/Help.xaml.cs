using System.Windows;
using System.Windows.Documents;


namespace EducationalTrainer
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            var fileName = "resources/help.rtf";
            LoadHelp(fileName);
        }

        private void LoadHelp(string fileName)
        {
            TextRange textRange;
            System.IO.FileStream fileStream;

            if (System.IO.File.Exists(fileName))
            {
                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                using (fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate))
                {
                    textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
                }
            }
        }
    }
}
