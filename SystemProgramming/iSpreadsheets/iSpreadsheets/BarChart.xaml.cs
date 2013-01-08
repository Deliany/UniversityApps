using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Visiblox.Charts;
using SelectionMode = Visiblox.Charts.SelectionMode;

namespace iSpreadsheets
{
    /// <summary>
    /// Interaction logic for BarChart.xaml
    /// </summary>
    public partial class BarChart : Window
    {
        private ObservableCollection<DataSumValuesCollection> SumValuesCollection = new ObservableCollection<DataSumValuesCollection>();

        public BarChart()
        {
            InitializeComponent();
            AddMouseEventHandlers();
        }

        private void AddMouseEventHandlers()
        {
            //Set HighlightedStyle to Normal style and add mouse enter and leave events on series
            foreach (BarSeries series in this.MainChart.Series)
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

            // copy columns data into our observable collection
            foreach (var column in data)
            {
                this.SumValuesCollection.Add(new DataSumValuesCollection
                    {
                        new DataSumValues
                            {
                                Header = (chartBy == ChartBy.Cols ? "Column " : "Row ") + column.Key,
                                SumValues = column.Value
                            }
                    });
            }

            foreach (var item in this.SumValuesCollection)
            {
                BarSeries columnSeries = new BarSeries { SelectionMode = SelectionMode.Series };

                BindableDataSeries bindableData = new BindableDataSeries
                {
                    Title = item[0].Header,
                    ItemsSource = item,
                    XValueBinding = new Binding("SumValues"),
                    YValueBinding = new Binding("Header"),
                };
                columnSeries.DataSeries = bindableData;

                this.MainChart.Series.Add(columnSeries);
            }
        }
    }
}
