using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Data> DataSet { get; set; } 
        private int AttrCount { get; set; }
        KMeans kMeans = new KMeans();
        List<Brush> br = new List<Brush>(); 

        public MainWindow()
        {
            InitializeComponent();
            ReadFromFile();
        }

        private void FillGrid(IEnumerable<Data> data)
        {
            DataTable dataBase = new DataTable();

            dataBase.Columns.Add(new DataColumn("ID", typeof(string)));
            for (int i = 1; i <= AttrCount; i++)
            {
                dataBase.Columns.Add(new DataColumn("A" + i, typeof(double)));
            }

            foreach (var r in data)
            {
                var row = dataBase.NewRow();

                row["ID"] = r.ID;
                for (int i = 1; i <= AttrCount; i++)
                {
                    row["A" + i] = r.Attributes[i - 1];
                }

                dataBase.Rows.Add(row);
            }

            dataGridData.ItemsSource = dataBase.DefaultView;
        }

        private void ReadFromFile(string path = "birth.data")
        {
            try
            {
                var dataset = new List<Data>();
                var info = new StringBuilder();

                foreach (var data in File.ReadAllLines(path))
                {
                    if (data.StartsWith("#"))
                    {
                        info.Append(data + Environment.NewLine);
                        continue;
                    }

                    var values = new List<double>();
                    string[] trainingExample = data.Split(' ');

                    string Id = trainingExample.First();
                    for(int i = 1; i < trainingExample.Length; i++)
                    {
                        values.Add(double.Parse(trainingExample[i], CultureInfo.InvariantCulture));
                    }

                    dataset.Add(new Data {ID = Id, Attributes = values});
                }

                AttrCount = dataset.First().Attributes.Count;
                labelAttrCount.Content = "Attributes Count: " + AttrCount;
                labelExamplesCount.Content = "Examples Count: " + dataset.Count;
                DataSet = dataset;

                FillGrid(dataset);

                for(int i = 0; i < 10; i++)
                {
                    comboBoxClusters.Items.Add(i + 1);
                }
                comboBoxClusters.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Tabify(List<Cluster> clusters)
        {
            tabControlClusters.Items.Clear();

            int index = 1;
            foreach (var cluster in clusters)
            {
                TabItem tabItem = new TabItem();
                tabItem.Background = br[index];
                Grid grid = new Grid();
                DataGrid dataGrid = new DataGrid
                                        {
                                            AutoGenerateColumns = true,
                                            RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed
                                        };
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add(new DataColumn("ID", typeof(string)));
                for (int i = 1; i <= AttrCount; i++)
                {
                    dataTable.Columns.Add(new DataColumn("A" + i, typeof(double)));
                }

                tabItem.Header = "Cluster #" + index++;
                foreach (var point in cluster.Points)
                {
                    var row = dataTable.NewRow();
                    row["ID"] = point.ID;
                    for (int i = 0; i < point.Attributes.Count;i++ )
                    {
                        row["A" + (i + 1)] = point.Attributes[i];
                    }
                    dataTable.Rows.Add(row);
                }

                dataGrid.ItemsSource = dataTable.DefaultView;
                grid.Children.Add(dataGrid);
                tabItem.Content = grid;
                tabControlClusters.Items.Add(tabItem);
                tabControlClusters.SelectedIndex = 0;
            }
        }

        private void Draw(List<Cluster> clusters)
        {
            canvasGraph.Children.Clear();
            // colors of clusters
            var brushes = new List<Brush>();
            brushes.Add(Brushes.Red);
            brushes.Add(Brushes.LightGreen);
            brushes.Add(Brushes.Blue);
            brushes.Add(Brushes.Violet);
            Random r = new Random();
            byte red = Convert.ToByte(r.Next(0, byte.MaxValue + 1));
            byte green = Convert.ToByte(r.Next(0, byte.MaxValue + 1));
            byte blue = Convert.ToByte(r.Next(0, byte.MaxValue + 1));
            for(int i = 0; i < 6; i++)
            {
                brushes.Add(new SolidColorBrush(Color.FromRgb(red, green, blue)));
            }
            br = brushes;


            //---------------------------------------
            List<double> avg = kMeans.AverageMean(DataSet);
            Point center = new Point(canvasGraph.Width / 2, canvasGraph.Height / 2);


            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Brush axis = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A7A7"));

                dc.DrawLine(new Pen(axis, 3), new Point(20, canvasGraph.Height - 10), null,
                            new Point(20, 20), null);
                dc.DrawLine(new Pen(axis, 3), new Point(10, canvasGraph.Height - 20), null,
                            new Point(canvasGraph.Width - 10, canvasGraph.Height - 20), null);
                dc.DrawText(new FormattedText(
                                "Birth rate %",
                                CultureInfo.GetCultureInfo("en-us"),
                                FlowDirection.LeftToRight,
                                new Typeface("Verdana"),
                                12,
                                Brushes.Black), new Point(canvasGraph.Width/2 - 40, canvasGraph.Height - 15));

                RotateTransform rt = new RotateTransform(270);
                rt.CenterX = canvasGraph.Width/6 - 5;
                rt.CenterY = canvasGraph.Height/4;
                dc.PushTransform(rt);

                dc.DrawText(new FormattedText(
                                "Death rate %",
                                CultureInfo.GetCultureInfo("en-us"),
                                FlowDirection.LeftToRight,
                                new Typeface("Verdana"),
                                12,
                                Brushes.Black), new Point(-30, 5));

                dc.Pop();

                

                int i = 0;
                foreach (var cluster in clusters)
                {
                    Brush brush = brushes[i++];
                    Point p1 = new Point(center.X + (cluster.Centroid[0] - avg[0])*7,
                                         center.Y + (cluster.Centroid[1] - avg[1])*7);
                    dc.DrawLine(new Pen(brush,3), p1 - new Vector(7, 7), null, p1 + new Vector(7, 7), null);
                    dc.DrawLine(new Pen(brush, 3), p1 - new Vector(7, -7), null, p1 + new Vector(7, -7), null);

                    foreach (var point in cluster.Points)
                    {
                        Point p2 = new Point(center.X + (point.Attributes[0] - avg[0])*7,
                                             center.Y + (point.Attributes[1] - avg[1])*7);
                        dc.DrawEllipse(brush, null, p2, null, 3, null, 3, null);
                    }
                }

                dc.Close();
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(Convert.ToInt32(canvasGraph.Width),
                                                            Convert.ToInt32(canvasGraph.Height), 96, 96,
                                                            PixelFormats.Pbgra32);
            rtb.Render(dv);
            Image img = new Image();
            img.Source = rtb;

            //-----------------------------------------

            canvasGraph.Children.Add(img);
        }

        private void comboBoxClusters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Cluster> clusters = kMeans.SplitToClusters(DataSet, comboBoxClusters.SelectedIndex + 1);
            Draw(clusters);
            Tabify(clusters);
        }
    }
}
