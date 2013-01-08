using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    class GaussRule:NumericalIntegration
    {

        protected override double Calculate(double a, double b, int n, string integral)
        {
            double res = 0;
            double A = (b - a) / 2;
            double B = (b + a) / 2;
            double[] x = GaussLegendre.arr[25].x;
            double[] w = GaussLegendre.arr[25].w;
            for (int i = 0; i < GaussLegendre.arr.Length; i++)
            {
                if (GaussLegendre.arr[i].n == n)
                {
                    x = GaussLegendre.arr[i].x;
                    w = GaussLegendre.arr[i].w;
                }
            }

            if (n % 2 == 1)
            {
                res = w[0] * MyParser.calculate(integral, B);
                for (int i = 1; i < x.Length; i++)
                {
                    res += w[i] * (MyParser.calculate(integral, B + A * x[i]) + MyParser.calculate(integral, B - A * x[i]));
                }
            }
            else
            {
                for (int i = 0; i < x.Length; i++)
                {
                    res += w[i] * (MyParser.calculate(integral, B + A * x[i]) + MyParser.calculate(integral, B - A * x[i]));
                }
            }
            return A * res;
        }
        public override string ToString()
        {
            return "Gauss";
        }
    }
}
