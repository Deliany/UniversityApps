using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    class GaussForwardInterpolation
    {
        private double[] nodes;
        private double[] values;
        private double[][] diff_table;
        private double h;
        private double u;

        public double[][] Diff_table
        {
            get { return diff_table; }
        }

        public GaussForwardInterpolation(double[] nodes, double[] values)
        {
            this.nodes = nodes;
            this.values = values;

            CalcDifferenceTable();
        }

        private void CalcDifferenceTable()
        {
            int len = values.Length;
            diff_table = new double[len][];

            for (int i = 0; i < len; i++)
            {
                diff_table[i] = new double[len - i];
            }

            for (int i = 0; i < len; i++)
            {
                diff_table[0][i] = values[i];
            }

            for (int i = 1; i < diff_table.GetLength(0); i++)
            {
                for (int j = 0; j < diff_table[i].Length; j++)
                {
                    diff_table[i][j] = diff_table[i - 1][j + 1] - diff_table[i - 1][j];
                }
            }
        }
        private double fact(double x)
        {
            double res = 1;
            for (int i = 2; i <= x; i++)
            {
                res *= i;
            }
            return res;
        }

        private double G(double u, int k)
        {
            if (k == 0)
            {
                return 1;
            }

            double result = 1;
            double factorial = fact(k);
            int l = 0;
            for (int i = 0; i < k; i++)
            {
                if(i%2==1)
                {
                    result *= (u-l);
                }
                else
                {
                    result *= (u + i - l);
                    l++;
                }
            }
            return result/factorial;
        }
        public double GaussInterpolation(double x)
        {
            h = nodes[1] - nodes[0];

            int i0 = nodes.Length /2;

            u = (x - nodes[i0]) / h;

            double F = 0;
            int m = 0;
            for (int i = 0; i < diff_table.GetLength(0); i++)
            {
                 m+= ((i%2 == 0 && i!= 0)? 1 : 0);
                 double y = diff_table[i][(int)(i0 - m) < 0 ? 0 : (int)(i0 - m)];
                F +=  y* G(u, i);

                //y += diff_table[0][i0] + (u/fact(1))*diff_table[1][i0] + (u*(u - 1)/fact(2))*diff_table[2][i0 - 1] +
                //     ((u + 1)*u*(u - 1)/fact(3))*diff_table[3][i0 - 1] +
                //     ((u + 1)*u*(u - 1)*(u - 2)/fact(4))*diff_table[4][i0 - 2];
            }




            return F;
        }
    }
}
