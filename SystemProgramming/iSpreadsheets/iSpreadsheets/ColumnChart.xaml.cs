using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using Visiblox.Charts;
using System.Linq;
using SelectionMode = Visiblox.Charts.SelectionMode;

namespace iSpreadsheets
{
    /// <summary>
    /// Interaction logic for Diagram.xaml
    /// </summary>
    public partial class ColumnChart : Window
    {
        private ObservableCollection<DataSumValuesCollection> SumValuesCollection = new ObservableCollection<DataSumValuesCollection>();

        public ColumnChart()
        {
            InitializeComponent();

            AddMouseEventHandlers();
        }

        private void AddMouseEventHandlers()
        {
            //Set HighlightedStyle to Normal style and add mouse enter and leave events on series
            foreach (ColumnSeries series in this.MainChart.Series)
            {
                series.MouseEnter += (s, e) => this.Cursor = Cursors.Hand;
                series.MouseLeave += (s, e) => this.Cursor = Cursors.Arrow;
            }
        }

        public void SetDataSumValuesList(Dictionary<string, double> data, ChartBy chartBy)
        {
            // lets clear previous chart
            this.SumValuesCollection.Clear();
            this.MainChart.Series.Clear();
            this.CustomLegend.Children.Clear();

            // copy columns data into our observable collection
            foreach (var d in data)
            {
                this.SumValuesCollection.Add(new DataSumValuesCollection
                    {
                        new DataSumValues
                            {
                                Header = (chartBy == ChartBy.Cols ? "Column " : "Row ") + d.Key,
                                SumValues = d.Value
                            }
                    });
            }

            foreach (var item in this.SumValuesCollection)
            {
                ColumnSeries columnSeries = new ColumnSeries { SelectionMode = SelectionMode.Series };

                BindableDataSeries bindableData = new BindableDataSeries
                {
                    Title = item[0].Header,
                    ItemsSource = item,
                    XValueBinding = new Binding("Header"),
                    YValueBinding = new Binding("SumValues")
                };
                columnSeries.DataSeries = bindableData;

                this.MainChart.Series.Add(columnSeries);

                Rectangle rect = new Rectangle
                {
                    Margin = new Thickness(5, 0, 2, 0),
                    Height = 10,
                    Width = 10,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Binding fillBind = new Binding();
                fillBind.ElementName = "MainChart";
                fillBind.Path = new PropertyPath("Series[" + (this.MainChart.Series.Count - 1) + "].PointFill");
                rect.SetBinding(Rectangle.FillProperty, fillBind);

                TextBlock txtblock = new TextBlock
                {
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Binding textBind = new Binding();
                textBind.ElementName = "MainChart";
                textBind.Path = new PropertyPath("Series[" + (this.MainChart.Series.Count - 1) + "].DataSeries.Title");
                txtblock.SetBinding(TextBlock.TextProperty, textBind);

                this.CustomLegend.Children.Add(rect);
                this.CustomLegend.Children.Add(txtblock);
            }
            
        }
    }

    // Data model

    public class DataSumValuesCollection : ObservableCollection<DataSumValues> { }

    public class DataSumValues
    {
        public string Header { get; set; }
        public double SumValues { get; set; }
    }

    public enum ChartBy
    {
        Rows,
        Cols
    }
}
