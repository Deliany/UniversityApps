using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewtonModified_Method
{
    class Matrix
    {
        public double[][] arr;

        private int n;
        private int m;

        public Matrix(int rows, int columns)
        {
            n = rows;
            m = columns;
            arr = new double[n][];
            for (int i = 0; i < n; i++)
            {
                arr[i] = new double[m];
            }
        }

        public double Determinant()
        {
            if (n != m)
                return 0;
            if (n == 1)
                return arr[0][0];
            if (n == 2)
                return arr[0][0] * arr[1][1] - arr[1][0] * arr[0][1];

            double result = 0;
            int k = 1;
            for (int i = 0; i < n; i++)
            {

                result += k * arr[0][i] * Minor(0, i).Determinant();
                k *= -1;
            }
            return result;
        }
        private Matrix Minor(int rowIndex, int colIndex)
        {
            Matrix mat = new Matrix(n - 1, n - 1);
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    if (col < colIndex)
                    {
                        if (row < rowIndex)
                        {
                            mat.arr[row][col] = arr[row][col];
                        }
                        if (row > rowIndex)
                        {
                            mat.arr[row - 1][col] = arr[row][col];
                        }
                    }
                    if (col > colIndex)
                    {
                        if (row < rowIndex)
                        {
                            mat.arr[row][col - 1] = arr[row][col];
                        }
                        if (row > rowIndex)
                        {
                            mat.arr[row - 1][col - 1] = arr[row][col];
                        }
                    }
                }
            }
            return mat;

        }
        public Matrix Inverse()
        {
            Matrix mat = new Matrix(n, m);
            if (n != m)
                return mat;

            double det = Determinant();
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    int k = 1;
                    if ((row + col) % 2 == 1)
                        k = -1;

                    mat.arr[col][row] = k * Minor(row, col).Determinant() / det;
                }
            }
            return mat;
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.m != m2.n)
                new Exception("Incorrect size of matrices");
            Matrix result = new Matrix(m1.n, m2.m);
            for (int i = 0; i < result.n; i++)
                for (int j = 0; j < result.m; j++)
                {
                    double tmp = 0;
                    for (int k = 0; k < m1.m; k++)
                        tmp += m1.arr[i][k] * m2.arr[k][j];
                    result.arr[i][j] = tmp;
                }
            return result;
        }

    }
}
