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
using System.Windows.Shapes;
using Visiblox.Charts;

namespace iSpreadsheets
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : Window
    {
        public PieChart()
        {
            InitializeComponent();
        }

        public  void GenerateDataSeries(Dictionary<string,double> data, ChartBy chartBy)
        {
            var series = new DataSeries<string, double>();

            foreach (var d in data)
            {
                series.Add(new DataPoint<string, double>((chartBy == ChartBy.Cols ? "Column " : "Row ") + d.Key, d.Value));
            }

            MainChart.DataSeries = series;
        }
    }
}
