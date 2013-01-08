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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visiblox.Charts;

namespace iGraph
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : UserControl
    {
        public Graph()
        {
            InitializeComponent();
            TestTridiagonalAlgorithm();
        }

        public void SolveDiffEquation(double T, double b, double sigma, double f, int n)
        {
            try
            {
                double[] u = new double[n + 1];
                u[0] = 0;
                u[n] = 0;
                var coefficients = FindCoefficients(T, b, sigma, f, n).Reverse().ToArray();
                for (int i = 1; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 1; j < n; j++)
                    {
                        double x = (double)i / n;
                        sum += coefficients[j - 1]*CourantFun(x, n, j);
                    }
                    u[i] = sum;
                }

                DataSeries<double, double> fun = new DataSeries<double, double>("u(x)");
                for (int i = 0; i < n + 1; i++)
                {
                    fun.Add(new DataPoint<double, double>() { X = (double)i / n, Y = u[i] });
                }
                exampleChart.Series[0].DataSeries = fun;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Calculate value of courant function, default segment is [0; 1]
        /// </summary>
        /// <param name="x">Argument x</param>
        /// <param name="n">Number of segments partitioning</param>
        /// <param name="i">Index of courant function</param>
        /// <param name="startSegment">Start segment</param>
        /// <param name="endSegment">End of segment</param>
        /// <returns></returns>
        public double CourantFun(double x, int n, int i, double startSegment = 0, double endSegment = 1)
        {
            if (startSegment > endSegment)
            {
                throw new Exception("Start of the segment is greater than end!");
            }


            //             { 0,                        x0    <= x < xi_minus_1
            // phi_i (x) = { (x - x_(i-1))/h,     xi_minus_1 <= x < xi
            //             { (xi_plus_1 - x)/h,        xi    <= x < xi_plus_1
            //             { 0,                   xi_plus_1  <= x <= xn

            double h = 1.0/n;
            double xi = i*h;
            double xi_minus_1 = (i - 1)*h;
            double xi_plus_1 = (i + 1)*h;

            if (x < xi_minus_1 || x > xi_plus_1)
            {
                return 0;
            }
            else if (xi_minus_1 <= x && x < xi)
            {
                return (x - xi_minus_1)/h;
            }
            else if (xi <= x && x < xi_plus_1)
            {
                return (xi_plus_1 - x)/h;
            }

            return 0;
        }

        /// <summary>
        /// Simple examples to test tridiagonal algorithm.
        /// </summary>
        public void TestTridiagonalAlgorithm()
        {
            // Test 1:
            // ---------------------------------
            double[,] A1 = { { 3, -1, 0, 0 }, { 2, -3, 2, 0 }, { 0, 1, 2, 5 }, { 0, 0, 1, -1 } };
            double[] d1 = { 5, 5, 10, 1 };

            // answer: x1=2, x2=1, x3=2, x4=1

            // Test 2:
            // ---------------------------------
            double[,] A2 = { { 1, 4, 0, 0 }, { 2, 10, -4, 0 }, { 0, 1, 8, -1 }, { 0, 0, 1, -6 } };
            double[] d2 = { 10, 7, 6, 4 };

            // answer: x1=26.376, x2=-4.094, x3=1.203, x4=-0.466

            try
            {
                double[] first_solution_x = SolveTridiagonalLinearSystem(A1, d1);
                double x1 = first_solution_x[0];

                double[] second_solution_x = SolveTridiagonalLinearSystem(A2, d2);
                double x2 = second_solution_x[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Builds matrix and solve the linear system.
        /// Aq=l.
        /// Based on Lagrange-linear equation.
        /// Case when u(0)=0 and u(1)=0.
        /// Method: Finite Elements Method.
        /// </summary>
        /// <param name="T">Constant T</param>
        /// <param name="b">Constant b</param>
        /// <param name="sigma">Constant sigma</param>
        /// <param name="f">Constant f</param>
        /// <param name="n">Number of segments partitioning</param>
        /// <returns></returns>
        public double[] FindCoefficients(double T, double b, double sigma, double f, int n)
        {
            double h = 1.0 / n;
            
            // Aq=l
            double[,] A = new double[n - 1, n - 1];
            double[] l = new double[n - 1];

            int N = A.GetLength(0);
            for (int i = 0; i < N; i++)
            {
                // Fill tridiagonal matrix with elements
                A[i, i] = 2.0*T/h + 2.0*h*sigma/3.0;
                if (i != N - 1) A[i, i + 1] = -T / h - b / 2.0 + h * sigma / 6.0;
                if (i != 0) A[i, i - 1] = -T / h + b / 2.0 + h * sigma / 6.0;
            }

            for (int i = 0; i < N; i++)
            {
                // Fill right-hand vector
                l[i] = h*f;
            }

            // Lets solve linear system!
            double[] q = SolveTridiagonalLinearSystem(A, l);
            return q;
        }

        /// <summary>
        /// Convert matrix to readable string.
        /// </summary>
        /// <param name="A">Matrix</param>
        /// <returns>Returns string with matrix elements</returns>
        public static string MatrixToString(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);

            StringBuilder matrixToString = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                matrixToString.Append("[ ");
                for (int j = 0; j < m; j++)
                {
                    matrixToString.Append(Math.Round(A[i, j],3) + " ");
                }
                matrixToString.Append("]\n");
            }

            return matrixToString.ToString();
        }

        /// <summary>
        /// Check whether matrix is tridiagonal or not
        /// </summary>
        /// <param name="A">Matrix to check</param>
        /// <returns>Returns true if matrix is tridiagonal
        /// False otherwise</returns>
        public bool IsTridiagonalMatrix(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            if (n != m)
            {
                throw new Exception("Matrix is not square!");
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i < n && i + 1 < j && A[i, j] != 0) || (j < n && j + 1 < i && A[i, j] != 0))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Tridiagonal Matrix Algorithm.
        /// Solves Tridiagonal Linear System of Equations.
        /// Ax=d.
        /// </summary>
        /// <param name="A">Tridiagonal matrix</param>
        /// <param name="d">Right-hand vector</param>
        /// <returns>Solution vector</returns>
        public double[] SolveTridiagonalLinearSystem(double[,] A, double[] d)
        {
            if (!IsTridiagonalMatrix(A))
            {
                throw new Exception("Matrix is not tridiagonal!");
            }


            // [b1 c1               ][x1]    [d1]
            // [a2 b2 c2            ][x2]    [d2]
            // [   a3 b3            ][x3]  = [d3]
            // [        ...         ][...]   [...]
            // [               c_n-1][...]   [...]
            // [            an   bn ][xn]    [dn]

            int N = d.Length;
            double[] _c = new double[N - 1];
            double[] _d = new double[N];
            double[] x = new double[N];

            // c'_1 = c_1 / b_1
            _c[0] = A[0, 1]/A[0, 0];

            // d'_1 = d_1 / b_1
            _d[0] = d[0]/A[0, 0];


            for (int i = 1; i < N - 1; ++i)
            {
                // c'_i = c_i / (b_i - c'_(i-1)*a_i) i = 2,n-1
                _c[i] = A[i, i + 1]/(A[i, i] - _c[i - 1]*A[i, i - 1]);

                // d'_i = (d_i - d'_(i-1)*a_i) / (b_i - c'_(i-1)*a_i) i = 2,n
                _d[i] = (d[i] - _d[i - 1]*A[i, i - 1])/(A[i, i] - _c[i - 1]*A[i, i - 1]);
            }
            _d[N - 1] = (d[N - 1] - _d[N - 2] * A[N - 1, N - 2]) / (A[N - 1, N - 1] - _c[N - 2] * A[N - 1, N - 2]);

            // xn = d'_n
            x[N - 1] = _d[N - 1];

            // xi = d'_i - c'_i*x_(i+1) i = n-1,n-2,...,1
            for (int i = N - 2; i >= 0; --i)
            {
                x[i] = _d[i] - _c[i]*x[i + 1];
            }

            return x;
        }
    }
}
