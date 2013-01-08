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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Dictionary<string, string> dictInsertTextItems = new Dictionary<string, string>();

            for (int i = 0; i < 10; i++)
            {
                dictInsertTextItems.Add("_itemKey_" + i, "Item Label " + i);
            }
            this.m_richTextEditor.InsertCustomTextItems = dictInsertTextItems;
        }
    }
}
