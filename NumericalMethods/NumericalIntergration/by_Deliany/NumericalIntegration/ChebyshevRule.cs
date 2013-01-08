using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    class ChebyshevRule:NumericalIntegration
    {
        //public override KeyValuePair<double, int> CalculateIntegral(double a, double b, double eps, string integral)
        //{
        //    int n = 3;
        //    double i1 = Calculate(a, b, n, integral);
        //    double i2 = 0;
        //    int k = 2;
        //    do
        //    {
        //        i2 = 0;
        //        for (int i = 0; i < k; i++)
        //        {
        //            i2 += Calculate(a + i * (b - a) / k, a + (i + 1) * (b - a) / k, n, integral);
        //        }

        //        double tmp = i2;
        //        i2 = i1;
        //        i1 = tmp;
        //        k++;
        //    } while (Math.Abs(i1 - i2) >= eps);

        //    return new KeyValuePair<double, int>(i1, k);
        //}

        protected override double Calculate(double a, double b, int n, string integral)
        {
            if (n == 8 || n > 9)
            {
                n = 9;
            }
            double res = 0;
            double A = (b - a) / 2;
            double B = (b + a) / 2;
            double[] x = ChebyshevCoefs.arr[7].x;
            for (int i = 0; i < ChebyshevCoefs.arr.Length; i++)
            {
                if (ChebyshevCoefs.arr[i].n == n)
                {
                    x = ChebyshevCoefs.arr[i].x;
                }
            }
            for (int i = 0; i < x.Length; i++)
            {
                res += MyParser.calculate(integral, A * x[i] + B);
            }
            return 2 * res * A / n;

        }
        public override string ToString()
        {
            return "Chebyshev";
        }
    }
    class ChebyshevCoefs
    {
        /* n = 1 */
        static double[] x1 = { 0 };

        /* n = 2 */
        static double[] x2 = { -0.577350, 0.577350 };

        /* n = 3 */
        static double[] x3 = { -0.707107, 0, 0.707107 };

        /* n = 4 */
        static double[] x4 = { -0.794654, -0.187592, 0.187592, 0.794654 };

        /* n = 5 */
        static double[] x5 = { -0.832498, -0.374541, 0, 0.374541, 0.832498 };

        /* n = 6 */
        static double[] x6 = { -0.866247, -0.422519, -0.266635, 0.266635, 0.422519, 0.866247 };

        /* n = 7 */
        static double[] x7 = { -0.883862, -0.529657, -0.321912, 0, 0.321912, 0.529657, 0.883862 };

        /* n = 9 */
        static double[] x9 = { -0.911589, -0.601019, -0.528762, -0.167906, 0, 0.167906, 0.528762, 0.601019, 0.911589 };

        public struct CC
        {
            public int n;
            public double[] x;
            public CC(int _n, double[] _x)
            {
                n = _n;
                x = _x;
            }
        };

        public static CC[] arr = {
                                     new CC(1, x1),
                                     new CC(2, x2),
                                     new CC(3, x3),
                                     new CC(4, x4),
                                     new CC(5, x5),
                                     new CC(6, x6),
                                     new CC(7, x7),
                                     new CC(9, x9),

                                  };
    }
}
