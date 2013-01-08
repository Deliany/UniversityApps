using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MatrixLibrary;

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Matrix theta;
        private int AttrCount { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ReadFromFile();
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

        private void ReadFromFile(string path = "HousesPrices.txt")
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
                    foreach (var value in data.Split(' '))
                    {
                        values.Add(double.Parse(value, CultureInfo.InvariantCulture));
                    }

                    rows.Add(values);
                }

                AttrCount = rows.First().Count;
                textBoxInfo.Text = info.ToString();

                FillGrid(rows);
                NormalEquation(rows);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NormalEquation(List<List<double>> rows)
        {
            int m = rows.Count;

            Matrix X = new Matrix(m, AttrCount);

            for (int i = 0; i < m; i++)
            {
                X[i, 0] = 1;
                for (int j = 0; j < AttrCount - 1; j++)
                {
                    X[i, j + 1] = rows[i][j];
                }
            }

            Matrix Y = new Matrix(m, 1);
            for (int i = 0; i < m; i++)
            {
                Y[i, 0] = rows[i].Last();
            }

            StringBuilder formulaH0 = new StringBuilder();
            formulaH0.Append("hθ(x)=θ"+0.ToString().Subscript());
            for(int i = 1; i < AttrCount; i++)
            {
                formulaH0.Append("+θ" + i.ToString().Subscript() + "x" + i.ToString().Subscript());
            }
            formulaH0.Append("=θᵀx");
            labelH0.Content = formulaH0.ToString();

            string[][] MatrixX = new string[m][];
            for (int i = 0; i < m; i++)
            {
                MatrixX[i] = new string[AttrCount];
                for (int j = 0; j < AttrCount; j++)
                {
                    MatrixX[i][j] = X[i, j].ToString();
                }
            }
            dataGridMatrixX.ItemsSource2D = MatrixX;

            string[][] VectorY = new string[m][];
            for (int i = 0; i < m; i++)
            {
                VectorY[i] = new string[1];
                VectorY[i][0] = Y[i, 0].ToString();
            }
            dataGridVectorY.ItemsSource2D = VectorY;

            Matrix XT = Matrix.Transpose(X);
            Matrix A = Matrix.Multiply(XT, X);
            Matrix A1 = Matrix.Inverse(A);
            Matrix temp1 = Matrix.Multiply(A1, XT);
            theta = Matrix.Multiply(temp1, Y);

            string[][] VectorTheta = new string[theta.NoRows][];
            for (int i = 0; i < theta.NoRows; i++)
            {
                VectorTheta[i] = new string[1];
                VectorTheta[i][0] = Math.Round(theta[i, 0],2).ToString();
            }
            dataGridVectorTheta.ItemsSource2D = VectorTheta;

            StringBuilder formulaH0Solved = new StringBuilder();
            formulaH0Solved.Append("hθ(x)=" + Math.Round(theta[0, 0],2));
            for (int i = 1; i < AttrCount; i++)
            {
                formulaH0Solved.Append((theta[i, 0] > 0
                                            ? "+"
                                            : "") + Math.Round(theta[i, 0], 2) + "x" + i.ToString().Subscript());
            }
            labelH0Solved.Content = formulaH0Solved.ToString();
        }

        #region Gradient Descent

        List<double> thetas = new List<double>();
        private void GradientDescent(List<List<double>> rows)
        {

            int m = rows.Count;
            int AttrCount = rows.First().Count;

            List<List<double>> x = new List<List<double>>();
            for (int i = 0; i < m; i++)
            {
                x.Add(rows[i]);
                x[x.Count - 1].RemoveAt(x[x.Count - 1].Count - 1);
                x[x.Count - 1].Insert(0, 1);
            }
            List<double> y = new List<double>();
            foreach (var row in rows)
            {
                y.Add(row.Last());
            }

            double[] theta = new double[AttrCount];
            double alpha = 0.001;

            double[] temp = new double[AttrCount];
            while (true)
            {
                for (int i = 0; i < AttrCount; i++)
                {
                    temp[i] = theta[i];
                }

                for (int i = 0; i < AttrCount; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < m; j++)
                    {
                        sum += (h0(theta, x[j]) - y[j]) * x[j][i];
                    }
                    temp[i] = theta[i] - (alpha / m) * sum;
                }


                bool flag = true;
                for (int i = 0; i < AttrCount; i++)
                {
                    if (Math.Abs(theta[i] - temp[i]) > 0.00001)
                    {
                        flag = false;
                        break;
                    }
                }

                for (int i = 0; i < AttrCount; i++)
                {
                    theta[i] = temp[i];
                }

                if (flag == true)
                {
                    break;
                }
            }
            thetas = theta.ToList();
        }
        private double h0(double[] theta, List<double> x)
        {
            double sum = 0;
            for (int i = 0; i < theta.Length; i++)
            {
                sum += theta[i] * x[i];
            }
            return sum;
        }

        #endregion

        private void buttonExample1_Click(object sender, RoutedEventArgs e)
        {
            ReadFromFile("HousesPrices.txt");
        }

        private void buttonExample2_Click(object sender, RoutedEventArgs e)
        {
            ReadFromFile("DeathRate.txt");
        }

        private void buttonExample3_Click(object sender, RoutedEventArgs e)
        {
            ReadFromFile("Housing.txt");
        }

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

            List<double> values = new List<double>();
            for (int i = 0; i < dataGridPredict.Columns.Count; i++)
            {
                TextBlock cellContent = dataGridPredict.Columns[i].GetCellContent(row) as TextBlock;
                if (cellContent != null)
                {
                    values.Add(double.Parse(cellContent.Text, CultureInfo.InvariantCulture));
                }
            }

            if (values.Count != AttrCount - 1)
            {
                throw new Exception("Entered incorrect values");
            }

            Matrix x = new Matrix(AttrCount, 1);
            x[0, 0] = 1;
            for (int i = 0; i < values.Count; i++)
            {
                x[i + 1, 0] = values[i];
            }

            Matrix transp = Matrix.Transpose(theta);
            Matrix h0 = transp * x;
            labelResult.Content = "Results: " + h0[0, 0].ToString();
        }
    }
}
