using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int AttrCount { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ReadFromFile();
            //Predict10();
        }

        private void FillGrid(List<List<double>> rows)
        {
            DataTable dataBase = new DataTable();
            DataTable predictionRow = new DataTable();

            for (int i = 1; i < AttrCount; i++)
            {
                dataBase.Columns.Add(new DataColumn("A" + i, typeof (double)));
                predictionRow.Columns.Add(new DataColumn("A" + i, typeof(double)));
            }
            dataBase.Columns.Add(new DataColumn("B", typeof (double)));

            
            foreach (var r in rows)
            {
                var row = dataBase.NewRow();

                for (int i = 1; i < AttrCount; i++)
                {
                    row["A" + i] = r[i - 1];
                }
                row["B"] = r.Last();

                dataBase.Rows.Add(row);
            }

            predictionRow.Rows.Add(predictionRow.NewRow());
            dataGridPredict.CanUserAddRows = false;

            dataGridPredict.ItemsSource = predictionRow.DefaultView;
            dataGridMain.ItemsSource = dataBase.DefaultView;
        }

        private void ReadFromFile(string path = "wine.txt")
        {
            try
            {
                var rows = new List<List<double>>();
                var info = new StringBuilder();

                foreach (var data in File.ReadAllLines(path))
                {
                    if (data.StartsWith("#"))
                    {
                        info.Append(data + Environment.NewLine);
                        continue;
                    }

                    List<double> values = new List<double>();
                    foreach (var value in data.Split(','))
                    {
                        values.Add(double.Parse(value, CultureInfo.InvariantCulture));
                    }

                    rows.Add(values);
                }

                AttrCount = rows.First().Count;
                textBoxInfo.Text = info.ToString();
                labelAttrCount.Content = "Attributes Count: " + AttrCount;
                labelExamplesCount.Content = "Examples Count: " + rows.Count;

                FillGrid(rows);
                GradientDescent(rows, 0.0001);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Gradient Descent

        List<double> thetas = new List<double>();
        private void GradientDescent(List<List<double>> rows, double alpha)
        {
            // m - number of examples
            int m = rows.Count;

            // AttrCount - number of variables
            AttrCount = rows.First().Count;

            // Matrix of independent variables values
            // e.g. |1 2.5 3.3 4.9|
            //      |1 9.3 4.5 0.3|
            //      |1 3.4 1.9 6.6|
            var x = new List<List<double>>();
            foreach (var row in rows)
            {
                x.Add(row.ToList());

                // remove unnecessary last feature
                x.Last().RemoveAt(x.Last().Count - 1);
                // insert '1' as x(0) in the beginning of vector
                // because Hypotesis function looks like h0(x) = θ₁+θ₂x₁+θ₃x₂+..., where x₀=1
                x.Last().Insert(0, 1);
            }

            // Vector of dependent attribute values
            var y = rows.Select(row => row.Last()).ToList();

            // Vector of thetas, start values = 1 
            var theta = new List<double>();
            for (int i = 0; i < AttrCount; i++ )
            {
                theta.Add(1);
            }

            // Variable, to know whether Cost Function increases or decreases over time
            double valueJ0 = 0;

            for (int g = 0; g < 5000; g++ )
            {
                // Save previous value of Cost Function
                double previosValueJ0 = valueJ0;

                // Save previous values of theta, to do the simultaneous update
                var tempTheta = theta.Select(t => t).ToList();

                // Calculating θj
                for (int j = 0; j < AttrCount; j++)
                {
                    double sum = 0;
                    for (int i = 0; i < m; i++)
                    {
                        sum += (h0(theta, x[i]) - y[i]) * x[i][j];
                    }
                    tempTheta[j] = theta[j] - (alpha / m) * sum;
                }

                // Simultaneous update all θj
                theta = tempTheta.Select(t => t).ToList();

                valueJ0 = CostFun(theta, x, y);
                if (previosValueJ0 > valueJ0)
                {
                    throw new Exception("Cost Function INCREASED => BAD learning rate!!!");
                }
            }


            thetas = theta.ToList();
            StringBuilder formulaH0Solved = new StringBuilder();
            formulaH0Solved.Append("θᵀx=" + Math.Round(theta[0], 2));
            for (int i = 1; i < AttrCount; i++)
            {
                formulaH0Solved.Append((theta[i] > 0
                                            ? "+"
                                            : "") + Math.Round(theta[i], 2) + "x" + i.ToString().Subscript());
            }
            labelTheta.Content = formulaH0Solved.ToString();
        }

        private double h0(List<double> theta, List<double> x)
        {
            // h0(x) = g(θᵀx), θᵀx = θ₁+θ₂x₁+θ₃x₂+..., where x₀=1
            // g(z) = 1/(1+e^(-z))
            double sum = 0;
            for (int i = 0; i < theta.Count; i++)
            {
                sum += theta[i] * x[i];
            }
            sum = 1 / (1 + Math.Pow(Math.E, -sum));
            return sum;
        }

        private double CostFun(List<double> theta, List<List<double>> x, List<double> y)
        {
            double sum = 0;
            int m = x.Count;

            for (int i = 0; i < m; ++i)
            {
                double H0 = h0(theta, x[i]);
                sum += y[i] * Math.Log(H0) + (1 - y[i]) * Math.Log(1 - H0);
            }

            return sum/m;
        }

        #endregion

        private void buttonPredict_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Predict();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Predict()
        {
            DataGridRow row = DataGridExtension.GetRow(dataGridPredict, 0);

            List<double> x = new List<double>();
            x.Add(1);
            for (int i = 0; i < dataGridPredict.Columns.Count; i++)
            {
                TextBlock cellContent = dataGridPredict.Columns[i].GetCellContent(row) as TextBlock;
                if (cellContent != null)
                {
                    x.Add(double.Parse(cellContent.Text, CultureInfo.InvariantCulture));
                }
            }

            if (x.Count != AttrCount)
            {
                throw new Exception("Entered incorrect values");
            }

            double result = h0(thetas, x);
            if(result >= 0.5)
            {
                labelResult.Content = "Result: 1 = Good quality of wine";
            }
            else
            {
                labelResult.Content = "Result: 0 = Bad quality of wine";
            }
        }

        private void Predict10()
        {
            string path = @"C:\Users\SiLenT\Desktop\task.txt";
            DataTable predictionRow = new DataTable();

            for (int i = 1; i < AttrCount; i++)
            {
                predictionRow.Columns.Add(new DataColumn("A" + i, typeof(double)));
            }
            predictionRow.Columns.Add(new DataColumn("Result", typeof(string)));
            

            foreach (var data in File.ReadAllLines(path))
            {
                List<double> x = new List<double>();
                x.Add(1);
                foreach (var value in data.Split(','))
                {
                    x.Add(double.Parse(value, CultureInfo.InvariantCulture));
                }
                x.Remove(x.Count - 1);


                var row = predictionRow.NewRow();
                double result = h0(thetas, x);

                for (int i = 1; i < AttrCount; i++)
                {
                    row["A" + i] = x[i - 1];
                }


                if (result >= 0.5)
                {
                    row["Result"] = "Result: 1 = Good quality of wine";
                }
                else
                {
                    row["Result"] = "Result: 0 = Bad quality of wine";
                }
                predictionRow.Rows.Add(row);
            }

            dataGridPredict.ItemsSource = predictionRow.DefaultView;
        }
    }
}
