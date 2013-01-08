using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using EducationalTrainer.Classes;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace EducationalTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Course course;
        private string absolutePath;
        private bool resultsReady = false;

        public MainWindow()
        {
            InitializeComponent();

            var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
            startupPath += "resources/";
            webBrowser.Navigate(new Uri(startupPath + "welcome.html"));

            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
            absolutePath = fileName + "course-informatics/";
            fileName += "course-informatics/structure.xml";

            try
            {
                course = Serializer.Deserialize<Course>(fileName);
                BindTree(course);

                resultsReady = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void BindTree(Course course)
        {
            ObservableCollection<CourseTreeViewItem> collection = new ObservableCollection<CourseTreeViewItem>();
            var theories = new CourseTreeViewItem { Items = new ObservableCollection<object>( course.Theories ), ImageUrl = "../Images/book.jpg" ,Title = "Теорія"};
            var tests = new CourseTreeViewItem { Items = new ObservableCollection<object>(course.Tests), ImageUrl = "../Images/pencil.jpg", Title = "Тести" };
            var results = new CourseTreeViewItem {  Items = new ObservableCollection<object>(), ImageUrl = "../Images/trophy.jpg", Title = "Результати" };
            collection.Add(theories);
            collection.Add(tests);
            collection.Add(results);

            tvMain.ItemTemplate = GetTemplate();
            tvMain.ItemsSource = collection;
            tvMain.SelectedItemChanged += tvMain_SelectedItemChanged;
        }

        void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Test)
            {
                foreach (var theory in course.Theories)
                {
                    if (theory.Visited == false)
                    {
                        var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\","/") + "/";
                        startupPath += "resources/";
                        // dumb way to hidden unnessary browser
                        webBrowser.Visibility = Visibility.Visible;
                        testSystem.Visibility = Visibility.Hidden;

                        webBrowser.Navigate(new Uri(startupPath+"pass_theory_first.html"));
                        return;
                    }
                }
                var test = (Test) e.NewValue;
                test.Visited = true;
                AddTests(test);

                // dumb way to hidden unnessary browser
                webBrowser.Visibility = Visibility.Hidden;
                testSystem.Visibility = Visibility.Visible;

                testSystem.webBrowser.Navigate(new Uri(absolutePath + test.Url));
            }
            else if (e.NewValue is Theory)
            {
                if(course.Tests.Any(test => test.Visited == true) && course.Tests.Count(test => test.Visited == true) != course.Tests.Count)
                {
                    var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
                    startupPath += "resources/";
                    // dumb way to hidden unnessary browser
                    webBrowser.Visibility = Visibility.Visible;
                    testSystem.Visibility = Visibility.Hidden;

                    webBrowser.Navigate(new Uri(startupPath + "test_is_already_started.html"));
                    return;
                }

                var theory = (Theory)e.NewValue;
                theory.Visited = true;

                // dumb way to hidden unnessary browser
                webBrowser.Visibility = Visibility.Visible;
                testSystem.Visibility = Visibility.Hidden;

                webBrowser.Navigate(new Uri(absolutePath + theory.Url));
            }
            else if (e.NewValue is CourseTreeViewItem)
            {
                var item = e.NewValue as CourseTreeViewItem;
                if (item.Title == "Результати")
                {
                     if (course.Tests.Any(test => test.Visited == true) &&
                         course.Tests.Count(test => test.Visited == true) == course.Tests.Count)
                     {
                         DataTable dataTable = GenerateDataTable(course.Tests);
                         string htmlTable = ConvertDataTableToHtml(dataTable);
                         File.WriteAllText("resources/results.html", htmlTable, UnicodeEncoding.Unicode);

                         var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
                         startupPath += "resources/";
                         // dumb way to hidden unnessary browser
                         webBrowser.Visibility = Visibility.Visible;
                         testSystem.Visibility = Visibility.Hidden;

                         webBrowser.Navigate(new Uri(startupPath + "results.html"));
                         resultsReady = true;
                     }
                     else
                     {
                         var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
                         startupPath += "resources/";
                         // dumb way to hidden unnessary browser
                         webBrowser.Visibility = Visibility.Visible;
                         testSystem.Visibility = Visibility.Hidden;

                         webBrowser.Navigate(new Uri(startupPath + "test_is_not_passed.html"));
                         return;
                     }
                }
            }
        }

        private DataTable GenerateDataTable(List<Test> tests)
        {
            DataTable table = new DataTable("Результати");
            table.Columns.Add("Номер питання");
            table.Columns.Add("Назва питання");
            table.Columns.Add("Відповідь користувача");
            table.Columns.Add("Правильна відповідь");
            table.Columns.Add("Бали");


            int sumPts = 0;
            for (int i = 0; i < tests.Count; ++i)
            {
                DataRow row = table.NewRow();
                row["Номер питання"] = i + 1;
                row["Назва питання"] = tests[i].Title;
                row["Відповідь користувача"] = tests[i].GivenAnswer;
                row["Правильна відповідь"] = tests[i].CorrectStringAnswer;
                row["Бали"] = tests[i].ResultPoints;
                sumPts += tests[i].ResultPoints;
                table.Rows.Add(row);
            }

            DataRow rowSummary = table.NewRow();
            rowSummary["Номер питання"] = "";
            rowSummary["Назва питання"] = "";
            rowSummary["Відповідь користувача"] = "";
            rowSummary["Правильна відповідь"] = "Сума балів";
            rowSummary["Бали"] = sumPts;
            table.Rows.Add(rowSummary);

            return table;
        }

        private HierarchicalDataTemplate GetTemplate()
        {
            //create the data template
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate();

            //create stack pane;
            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            grid.Name = "parentStackpanel";
            grid.SetValue(Grid.WidthProperty, Convert.ToDouble(200));
            grid.SetValue(Grid.HeightProperty, Convert.ToDouble(24));
            grid.SetValue(Grid.MarginProperty, new Thickness(2));

            // Create Image 
            FrameworkElementFactory image = new FrameworkElementFactory(typeof(Image));
            image.SetValue(Image.MarginProperty, new Thickness(2));
            image.SetValue(Image.VerticalAlignmentProperty, VerticalAlignment.Center);
            image.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            image.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("ImageUrl") });

            grid.AppendChild(image);

            // create text
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(TextBlock));
            label.SetBinding(TextBlock.TextProperty,
                new Binding() { Path = new PropertyPath("Title") });
            label.SetValue(TextBlock.MarginProperty, new Thickness(25,2,2,2));
            label.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            label.SetValue(TextBlock.ToolTipProperty, new Binding("Title"));

            grid.AppendChild(label);

            dataTemplate.ItemsSource = new Binding("Items");

            //set the visual tree of the data template
            dataTemplate.VisualTree = grid;

            return dataTemplate;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
                          {
                              DefaultExt = ".xml",
                              Filter =
                                  "Educational course structure (.xml)|*.xml",
                              FileName = "structure.xml"
                          };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                absolutePath = Path.GetDirectoryName(fileName).Replace("\\","/") + "/";

                try
                {
                    course = Serializer.Deserialize<Course>(fileName);
                    BindTree(course);

                    resultsReady = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddTests(Test test)
        {
            testSystem.stackPanel.Children.Clear();
            foreach (var radioButton in test.RadioButtons)
            {
                testSystem.stackPanel.Children.Add(radioButton);
            }
            
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help helpWindow = new Help();
            helpWindow.Show();
        }

        public string ConvertDataTableToHtml(DataTable targetTable)
        {
            string htmlString = "";

            if (targetTable == null)
            {
                throw new System.ArgumentNullException("targetTable");
            }

            StringBuilder htmlBuilder = new StringBuilder();

            //Create Top Portion of HTML Document
            htmlBuilder.Append("<html>");
            htmlBuilder.Append("<head>");
            htmlBuilder.Append("<title>");
            htmlBuilder.Append("Page-");
            htmlBuilder.Append(Guid.NewGuid().ToString());
            htmlBuilder.Append("</title>");
            htmlBuilder.Append("<link href='results.css' rel='stylesheet' type='text/css'>");
            htmlBuilder.Append("</head>");
            htmlBuilder.Append("<body>");
            htmlBuilder.Append("<table id='results'");
            htmlBuilder.Append("style='border: solid 1px Black; font-size: small;'>");

            //Create Header Row
            htmlBuilder.Append("<tr align='left' valign='top'>");

            foreach (DataColumn targetColumn in targetTable.Columns)
            {
                htmlBuilder.Append("<th align='left' valign='top'>");
                htmlBuilder.Append(targetColumn.ColumnName);
                htmlBuilder.Append("</th>");
            }

            htmlBuilder.Append("</tr>");

            //Create Data Rows
            bool parity = false;
            foreach (DataRow myRow in targetTable.Rows)
            {
                parity = !parity;
                htmlBuilder.Append("<tr align='left' valign='top' " + (parity? "class='alt'" : "") + ">");

                foreach (DataColumn targetColumn in targetTable.Columns)
                {
                    htmlBuilder.Append("<td align='left' valign='top'>");
                    htmlBuilder.Append(myRow[targetColumn.ColumnName].ToString());
                    htmlBuilder.Append("</td>");
                }

                htmlBuilder.Append("</tr>");
            }

            //Create Bottom Portion of HTML Document
            htmlBuilder.Append("</table>");
            htmlBuilder.Append("</body>");
            htmlBuilder.Append("</html>");

            //Create String to be Returned
            htmlString = htmlBuilder.ToString();

            return htmlString;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (resultsReady)
            {
                var dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    DefaultExt = ".html",
                    Filter =
                        "Test results (.html)|*.html",
                    FileName = "Results.html"
                };

                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        string fileName = dlg.FileName;
                        var startupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace("\\", "/") + "/";
                        startupPath += "resources/";
                        startupPath += "results.css";
                        var destPath = Path.GetDirectoryName(fileName).Replace("\\", "/") + "/";
                        destPath += "results.css";
                        if (File.Exists(destPath))
                        {
                            File.Replace(destPath, startupPath, destPath + "_backup");
                        }
                        else
                        {
                            File.Copy(startupPath, destPath);
                        }

                        DataTable dataTable = GenerateDataTable(course.Tests);
                        var htmlTable = ConvertDataTableToHtml(dataTable);
                        File.WriteAllText(fileName, htmlTable, UnicodeEncoding.Unicode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Результати ще не готові. Пройдіть тест!", "Інформація", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
