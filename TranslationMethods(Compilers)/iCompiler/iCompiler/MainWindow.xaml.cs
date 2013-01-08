using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using Newtonsoft.Json;
using iCompiler.Helpers;
using Formatting = System.Xml.Formatting;

namespace iCompiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string defaultFileName = "application.mnl";

        public MainWindow()
        {
            InitializeComponent();

            textEditor.ShowLineNumbers = true;
            textEditor.Options.ShowSpaces = true;
            textEditor.Options.ShowTabs = true;

            ClearAndFillDefaultValuesAnalysersRichTextBoxes();

            LoadSyntaxHighlight();
            LoadDefaultFile();
        }

        #region UI Interaction

        private void LoadDefaultFile()
        {
            try
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                textEditor.Load(appPath + defaultFileName);
            }
            catch { }
        }

        private void LoadSyntaxHighlight()
        {
            try
            {
                // Load our custom highlighting definition
                IHighlightingDefinition customHighlighting;
                using (
                    Stream s =
                        typeof(MainWindow).Assembly.GetManifestResourceStream("iCompiler.Resources.MiniSyntaxHighlighting.xshd"))
                {
                    if (s == null)
                        throw new InvalidOperationException("Could not find embedded resource");
                    using (XmlReader reader = new XmlTextReader(s))
                    {
                        customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                                                         HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
                // and register it in the HighlightingManager
                HighlightingManager.Instance.RegisterHighlighting("Mini Syntax Highlighting", new string[] { ".mnl" },
                                                                  customHighlighting);

                textEditor.SyntaxHighlighting = customHighlighting;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} >>> {1}", ex.GetType(), ex.Message),
                                "Error while loading syntax highlight", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        string currentFileName;

        private void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = ".mnl";
            dlg.Filter = "Mini language (.mnl)|*.mnl";
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        private void saveFileClick(object sender, RoutedEventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".mnl";
                dlg.Filter = "Mini language (.mnl)|*.mnl";
                dlg.FileName = currentFileName ?? defaultFileName;
                if (dlg.ShowDialog() == true)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        private void newFileClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("If you create new file, all data will be lost.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                textEditor.Text = "";
                currentFileName = null;
            }
            else
            {
                return;
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                textEditor.Save(appPath + "application.mnl");
            }
            catch
            { }
        }

        #endregion

        private void buildClick(object sender, RoutedEventArgs e)
        {
            Build();
        }

        private void runClick(object sender, RoutedEventArgs e)
        {
            if (Build())
            {
                string fileName = currentFileName ?? defaultFileName;

                var proc = new System.Diagnostics.Process
                {
                    StartInfo = { FileName = Path.GetFileNameWithoutExtension(fileName) + ".exe" }
                };

                proc.Start();
            }
            else
            {
                MessageBox.Show("There were errors during compiling process");
            }
        }

        private bool Build()
        {
            try
            {
                LexicalAnalysis scanner;
                ClearAnalysersRichTextBoxes();

                string fileName = currentFileName ?? defaultFileName;

                var writeToFile = new StreamWriter(fileName, false, Encoding.Unicode);
                writeToFile.Write(textEditor.Text);
                writeToFile.Close();

                using (TextReader input = File.OpenText(fileName))
                {
                    scanner = new LexicalAnalysis(input);
                }

                FillLexicalAnalysisTab(scanner.Tokens);

                var parser = new SyntaxAnalysis(scanner.Tokens);
                FillSyntaxAnalysisTab(parser.Result);

                var generator = new Generator(parser.Result, Path.GetFileNameWithoutExtension(fileName) + ".exe");

                TextRange tr = new TextRange(outputRichTextBlock.Document.ContentStart, outputRichTextBlock.Document.ContentStart);
                tr.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " | >>> " +
                          Path.GetFileNameWithoutExtension(fileName) + ".exe successfully build. \r\n";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            }
            catch (Exception ex)
            {
                TextRange tr = new TextRange(outputRichTextBlock.Document.ContentStart, outputRichTextBlock.Document.ContentStart);
                tr.Text = DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " | " + ex.GetType().Name + " >>> Error: " +
                          ex.Message + "\r\n";

                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

                ClearAndFillDefaultValuesAnalysersRichTextBoxes();
                return false;
            }

            return true;
        }

        private void FillSyntaxAnalysisTab(Statement statement)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            
            string json = JsonConvert.SerializeObject(statement, Newtonsoft.Json.Formatting.Indented, settings);
            TextRange tr = new TextRange(syntaxAnalysisRichTextBlock.Document.ContentStart, syntaxAnalysisRichTextBlock.Document.ContentStart);

            tr.Text = json;

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

        }

        private void FillLexicalAnalysisTab(System.Collections.Generic.List<Tuple<TokenType, LexemeClass>> list)
        {
            TextRange tr = new TextRange(lexicalAnalysisRichTextBlock.Document.ContentStart, lexicalAnalysisRichTextBlock.Document.ContentStart);

            StringBuilder text = new StringBuilder();
            text.AppendLine("List of lexemes, pattern: <TokenClass, Lexeme>");
            text.AppendLine("===========================================");
            foreach (var tuple in list)
            {
                text.Append("<");
                text.Append(tuple.Item1.ToString());
                text.Append(", ");
                text.Append(tuple.Item2.Lexeme);
                text.Append(">");
                text.AppendLine();
            }

            tr.Text = text.ToString();

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
        }

        private void ClearAnalysersRichTextBoxes()
        {
            lexicalAnalysisRichTextBlock.Document.Blocks.Clear();
            syntaxAnalysisRichTextBlock.Document.Blocks.Clear();
        }

        private void ClearAndFillDefaultValuesAnalysersRichTextBoxes()
        {
            lexicalAnalysisRichTextBlock.Document.Blocks.Clear();
            syntaxAnalysisRichTextBlock.Document.Blocks.Clear();

            TextRange tr = new TextRange(lexicalAnalysisRichTextBlock.Document.ContentStart, lexicalAnalysisRichTextBlock.Document.ContentStart);
            tr.Text = "Results shows only for last successfull build. Please correct errors and build again.";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange tr2 = new TextRange(syntaxAnalysisRichTextBlock.Document.ContentStart, syntaxAnalysisRichTextBlock.Document.ContentStart);
            tr2.Text = "Results shows only for last successfull build. Please correct errors and build again.";
            tr2.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
        }

        private void helpClick(object sender, RoutedEventArgs e)
        {
            AboutBox window = new AboutBox();
            window.Show();
        }
    }
}
