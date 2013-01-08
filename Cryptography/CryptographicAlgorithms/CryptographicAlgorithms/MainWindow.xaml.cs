using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CryptographicAlgorithms.Analyzer;
using CryptographicAlgorithms.Cyphers;
using Microsoft.Win32;

namespace CryptographicAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Alphabet alphabet = Alphabet.Latin;
        private Cypher cypher = Cypher.ShiftCypher;
        private FrequencyAnalysis analyzer = new FrequencyAnalysis();
        private FrequencyAnalysis hacker = new FrequencyAnalysis();
        private Cyphers.RSA rsa;
        private Cyphers.Diffi_Hellman diffiHellman;
        UTF8Encoding encoding = new UTF8Encoding();

        public MainWindow()
        {
            InitializeComponent();
            ShiftCypherPreparation();
        }

        #region Analyzer

        class FreqTable
        {
            public char Letter { get; set; }
            public string Frequency { get; set; }
        }

        private void Analyze()
        {
            string text = textBoxAnalyzer.Text.ToLower();
            string correctText = Regex.Replace(text, @"\s+", " ");

            analyzer.Alphabet = alphabet;
            analyzer.Analyze(correctText);

            var table = new Dictionary<char, double>();
            switch (alphabet)
            {
                case Alphabet.Latin:
                    table = analyzer.LatinTable;
                    break;
                case Alphabet.Ukrainian:
                    table = analyzer.UkrainianTable;
                    break;
            }

            var query = from x in table
                        orderby x.Value descending
                        select new { Буква = x.Key, Частота = Math.Round(x.Value, 3).ToString() + "%" };

            ObservableCollection<FreqTable> collection = new ObservableCollection<FreqTable>();
            foreach (var q in query)
            {
                collection.Add(new FreqTable { Letter = q.Буква, Frequency = q.Частота });
            }
            
            dataGridFrequencyTable.ItemsSource = collection;
        }

        private void Hack()
        {
            string text = textBoxEncryptedText.Text;
            hacker.Alphabet = alphabet;
            hacker.Analyze(text);

            var table1 = new Dictionary<char, double>();
            var table2 = new Dictionary<char, double>();
            switch (alphabet)
            {
                case Alphabet.Latin:
                    table1 = analyzer.LatinTable;
                    table2 = hacker.LatinTable;
                    break;
                case Alphabet.Ukrainian:
                    table1 = analyzer.UkrainianTable;
                    table2 = hacker.UkrainianTable;
                    break;
            }

            var query = from x in table1
                        orderby x.Value descending
                        select x.Key;
            var query2 = from x in table2
                         orderby x.Value descending
                         select x.Key;

            var substitutions = new Dictionary<char, char>();
            for (int i = 0; i < query.Count(); i++)
            {
                substitutions.Add(query.ElementAt(i), query2.ElementAt(i));
            }

            ObservableCollection<SubTable> collection = new ObservableCollection<SubTable>();

            foreach (var s in substitutions)
            {
                collection.Add(new SubTable { Letter = s.Key, SubLetter = s.Value });
            }
            dataGridHackerTable.ItemsSource = collection;
            SubstitutionCypher cipher = new SubstitutionCypher { Alphabet = alphabet };
            textBoxDecryptedText.Text = cipher.Decrypt(text, substitutions);
        }

        #endregion

        #region Cyphers Encryption
        //----------------------------------------------------

        private void ShiftCypherEncrypt()
        {
            ShiftCypher cipher = new ShiftCypher { Alphabet = alphabet };
            string text = Regex.Replace(textBoxOpenText.Text.ToLower(), "[^" + alphabet.GetStringValue() + "]", "");
            string M = Regex.Replace(text, @"\s+", " ");
            DoEvents();
            int Key = (gridKey.Children[1] as ComboBox).SelectedIndex;

            textBoxEncryptedText.Text = cipher.Encrypt(M, Key);
        }

        private void SubstitutionCypherEncrypt()
        {
            SubstitutionCypher cipher = new SubstitutionCypher { Alphabet = alphabet };
            string text = Regex.Replace(textBoxOpenText.Text.ToLower(), "[^" + alphabet.GetStringValue() + "]", "");
            string M = Regex.Replace(text, @"\s+", " ");
            DoEvents();

            Dictionary<char, char> substitutions = new Dictionary<char, char>();
            DataGrid dataGrid = gridKey.Children[1] as DataGrid;

            for (int i = 0; i < alphabet.GetLength(); ++i)
            {
                var letter = (dataGrid.GetCell(i, 0).Content as TextBlock).Text[0];
                var subletter = (dataGrid.GetCell(i, 1).Content as TextBlock).Text[0];
                substitutions.Add(letter, subletter);
            }

            textBoxEncryptedText.Text = cipher.Encrypt(M, substitutions);
        }

        private void VigenereCypherEncrypt()
        {
            VigenereCypher cipher = new VigenereCypher { Alphabet = alphabet };
            string text = Regex.Replace(textBoxOpenText.Text.ToLower(), "[^" + alphabet.GetStringValue() + "]", "");
            string M = Regex.Replace(text, @"\s+", " ");
            DoEvents();

            TextBox textBox = gridKey.Children[1] as TextBox;
            string Key = Regex.Replace(textBox.Text.ToLower(), "[^" + alphabet.GetStringValue() + "]", "");
            if (Key == string.Empty)
            {
                throw new Exception("Key cannot be empty!");
            }

            textBoxEncryptedText.Text = cipher.Encrypt(M, Key);
        }

        //----------------------------------------------------
        #endregion

        #region Cyphers Decryption
        //----------------------------------------------------

        private void ShiftCypherDecrypt()
        {
            ShiftCypher cypher = new ShiftCypher { Alphabet = alphabet };
            string C = textBoxEncryptedText.Text;
            DoEvents();
            int Key = (gridKey.Children[1] as ComboBox).SelectedIndex;

            textBoxDecryptedText.Text = cypher.Decrypt(C, Key);
        }

        private void SubstitutionCypherDecrypt()
        {
            SubstitutionCypher cypher = new SubstitutionCypher { Alphabet = alphabet };
            string C = textBoxEncryptedText.Text;
            DoEvents();
            Dictionary<char, char> substitutions = new Dictionary<char, char>();
            DataGrid dataGrid = gridKey.Children[1] as DataGrid;

            for (int i = 0; i < alphabet.GetLength(); ++i)
            {
                var letter = (dataGrid.GetCell(i, 0).Content as TextBlock).Text[0];
                var subletter = (dataGrid.GetCell(i, 1).Content as TextBlock).Text[0];
                substitutions.Add(letter, subletter);
            }

            textBoxDecryptedText.Text = cypher.Decrypt(C, substitutions);
        }

        private void VigenereCypherDecrypt()
        {
            VigenereCypher cypher = new VigenereCypher { Alphabet = alphabet };
            string C = textBoxEncryptedText.Text;
            DoEvents();

            TextBox textBox = gridKey.Children[1] as TextBox;
            string Key = Regex.Replace(textBox.Text.ToLower(), "[^" + alphabet.GetStringValue() + "]", "");
            if (Key == string.Empty)
            {
                throw new Exception("Key cannot be empty!");
            }

            textBoxDecryptedText.Text = cypher.Decrypt(C, Key);
        }

        //----------------------------------------------------
        #endregion

        #region Cypher Key Preparation

        private void ShiftCypherPreparation()
        {
            Label label = new Label();
            label.Content = "Зсув букв на таке число:";
            label.Foreground = Brushes.White;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;

            ComboBox comboBox = new ComboBox();
            comboBox.Height = 20;
            comboBox.Width = 50;
            comboBox.Margin = new Thickness(10, 25, 0, 0);
            comboBox.HorizontalAlignment = HorizontalAlignment.Left;
            comboBox.VerticalAlignment = VerticalAlignment.Top;
            for (int i = 0; i < alphabet.GetLength(); ++i)
            {
                comboBox.Items.Add(i);
            }
            comboBox.SelectedIndex = 0;

            groupBoxKey.Height = 100;
            gridKey.Children.Clear();
            gridKey.Children.Add(label);
            gridKey.Children.Add(comboBox);
        }

        public class SubTable
        {
            public char Letter { get; set; }
            public char SubLetter { get; set; }
        }
        private void SubstitutionCypherPreparation()
        {
            Label label = new Label();
            label.Content = "Таблиця підстановок:";
            label.Foreground = Brushes.White;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;


            DataGrid dataGrid = new DataGrid();
            dataGrid.AutoGenerateColumns = true;
            dataGrid.Margin = new Thickness(10, 25, 0, 0);
            dataGrid.HorizontalAlignment = HorizontalAlignment.Left;
            dataGrid.VerticalAlignment = VerticalAlignment.Top;
            dataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            dataGrid.IsReadOnly = false;
            dataGrid.CanUserAddRows = false;

            ObservableCollection<SubTable> collection = new ObservableCollection<SubTable>();
            string characters = alphabet.GetStringValue();
            string substitutions = " nopqrstuvwxyzabcdefghijklm";
            for (int i = 0; i < characters.Length; ++i)
            {
                collection.Add(new SubTable { Letter = characters[i], SubLetter = substitutions[i] });
            }
            dataGrid.ItemsSource = collection;

            groupBoxKey.Height = 250;
            gridKey.Children.Clear();
            gridKey.Children.Add(label);
            gridKey.Children.Add(dataGrid);
        }

        private void VigenereCypherPreparation()
        {
            Label label = new Label();
            label.Content = "Введіть ключ(мовою вище):";
            label.Foreground = Brushes.White;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;

            TextBox textBox = new TextBox();
            textBox.Height = 25;
            textBox.Width = 100;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.Margin = new Thickness(5, 25, 0, 0);

            groupBoxKey.Height = 100;
            gridKey.Children.Clear();
            gridKey.Children.Add(label);
            gridKey.Children.Add(textBox);
        }

        #endregion

        #region Events

        private void comboBoxCypher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxCypher.SelectedIndex)
            {
                case 0:
                    cypher = Cypher.ShiftCypher;
                    ShiftCypherPreparation();

                    if (checkBoxAnalysisMode != null)
                    {
                        checkBoxAnalysisMode.Visibility = Visibility.Visible;
                    }
                    break;
                case 1:
                    cypher = Cypher.SubstitutionCypher;
                    SubstitutionCypherPreparation();

                    if (checkBoxAnalysisMode != null)
                    {
                        checkBoxAnalysisMode.Visibility = Visibility.Visible;
                    }
                    break;
                case 2:
                    cypher = Cypher.VigenereCypher;
                    VigenereCypherPreparation();

                    if (checkBoxAnalysisMode != null)
                    {
                        checkBoxAnalysisMode.Visibility = Visibility.Hidden;
                        checkBoxAnalysisMode.IsChecked = false;
                    }
                    break;
            }
        }

        private void comboBoxAlphabet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxAlphabet.SelectedIndex)
            {
                case 0:
                    alphabet = Alphabet.Latin;
                    if (comboBoxCypher != null)
                    {
                        comboBoxCypher_SelectionChanged(null, null);
                    }
                    break;
                case 1:
                    alphabet = Alphabet.Ukrainian;
                    if (comboBoxCypher != null)
                    {
                        comboBoxCypher_SelectionChanged(null, null);
                    }
                    break;
            }
        }

        private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (cypher)
                {
                    case Cypher.ShiftCypher:
                        ShiftCypherEncrypt();
                        break;
                    case Cypher.SubstitutionCypher:
                        SubstitutionCypherEncrypt();
                        break;
                    case Cypher.VigenereCypher:
                        VigenereCypherEncrypt();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (cypher)
                {
                    case Cypher.ShiftCypher:
                        ShiftCypherDecrypt();
                        break;
                    case Cypher.SubstitutionCypher:
                        SubstitutionCypherDecrypt();
                        break;
                    case Cypher.VigenereCypher:
                        VigenereCypherDecrypt();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                textBoxOpenText.Text = File.ReadAllText(dlg.FileName).ToLower();
                DoEvents();
            }
        }

        private void buttonReadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                textBoxAnalyzer.Text = File.ReadAllText(dlg.FileName).ToLower();
                DoEvents();
            }
        }

        private void buttonAnalyze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Analyze();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonHack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBoxAnalysisMode_Checked(object sender, RoutedEventArgs e)
        {
            if (groupBoxAnalyzer != null)
            {
                groupBoxAnalyzer.Visibility = Visibility.Visible;
                groupBoxFrequencyTable.Visibility = Visibility.Visible;
                buttonAnalyze.Visibility = Visibility.Visible;
                buttonReadFromFile.Visibility = Visibility.Visible;
                buttonHack.Visibility = Visibility.Visible;
                groupBoxHacker.Visibility = Visibility.Visible;
            }
        }

        private void checkBoxAnalysisMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (groupBoxAnalyzer != null)
            {
                groupBoxAnalyzer.Visibility = Visibility.Hidden;
                groupBoxFrequencyTable.Visibility = Visibility.Hidden;
                buttonAnalyze.Visibility = Visibility.Hidden;
                buttonReadFromFile.Visibility = Visibility.Hidden;
                buttonHack.Visibility = Visibility.Hidden;
                groupBoxHacker.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region DoEvents(unfreeze application)

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        #endregion

        #region RSA

        private void buttonGeneratePQ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rsa = new Cyphers.RSA(int.Parse(textBoxKeySize.Text));
                rsa.GeneratePQ();
                textBoxP.Text = rsa.P.ToString();
                textBoxQ.Text = rsa.Q.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCalcRSA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rsa == null)
                {
                    throw new Exception("P and Q values are not initialized!");
                }

                rsa.CalculateNPhi();
                textBoxN.Text = rsa.N.ToString();
                textBoxPhi.Text = rsa.Phi.ToString();

                rsa.CalculateDE();
                textBoxPublicKey.Text = rsa.E.ToString();
                textBoxSecretKey.Text = rsa.D.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSetPQ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger p = BigInteger.Parse(textBoxP.Text);
                BigInteger q = BigInteger.Parse(textBoxQ.Text);
                rsa = new Cyphers.RSA(p, q);
                textBoxKeySize.Text = rsa.KeySize.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonEncryptRSA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = encoding.GetBytes(textBoxMessage.Text);
                textBoxEncryptedMessage.Text = new BigInteger(rsa.EncryptMessage(message)).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDecryptRSA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var encMessage = BigInteger.Parse(textBoxEncryptedMessage.Text).ToByteArray();
                textBoxMessage.Text = @encoding.GetString(rsa.DecryptMessage(encMessage));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSetNED_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger N = BigInteger.Parse(textBoxN.Text);
                BigInteger E = BigInteger.Parse(textBoxPublicKey.Text);
                BigInteger D = BigInteger.Parse(textBoxSecretKey.Text);

                rsa.SetNED(N, E, D);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion



        #region DiffiHellman

        private void buttonSetP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger p = BigInteger.Parse(textBoxPDiffiHellman.Text);
                BigInteger g = BigInteger.Parse(textBoxG.Text);
                diffiHellman = new Diffi_Hellman(p, g);

                textBoxSizeInBits.Text = diffiHellman.KeySize.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonGenerateP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman = new Diffi_Hellman(int.Parse(textBoxSizeInBits.Text));
                diffiHellman.GeneratePG();

                textBoxPDiffiHellman.Text = diffiHellman.P.ToString();
                textBoxG.Text = diffiHellman.G.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSet_a_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger a = BigInteger.Parse(textBox_a.Text);

                diffiHellman.Set_a(a);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSet_b_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger b = BigInteger.Parse(textBox_b.Text);

                diffiHellman.Set_b(b);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonGenerate_ab_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman.Generate_ab();

                textBox_a.Text = diffiHellman.a.ToString();
                textBox_b.Text = diffiHellman.b.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSetA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger a = BigInteger.Parse(textBoxA.Text);
                diffiHellman.SetA(a);
                textBox_a.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSetB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BigInteger b = BigInteger.Parse(textBoxB.Text);
                diffiHellman.SetB(b);
                textBox_a.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCalcA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman.CalculateA();

                textBoxA.Text = diffiHellman.A.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCalcB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman.CalculateB();

                textBoxB.Text = diffiHellman.B.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCalculateSharedKey1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman.CalculateKey1();

                textBoxSharedKey1.Text = diffiHellman.Key1.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCalculateSharedKey2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                diffiHellman.CalculateKey2();

                textBoxSharedKey2.Text = diffiHellman.Key2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxDecryptedText.Text = "";
        }

        private void textBoxMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                BigInteger numMsg = new BigInteger(encoding.GetBytes(textBoxMessage.Text));
                textBoxNumericMessage.Text = numMsg.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
