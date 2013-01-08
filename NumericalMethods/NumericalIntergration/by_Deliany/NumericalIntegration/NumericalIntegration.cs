using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    abstract class NumericalIntegration
    {
        public virtual KeyValuePair<double, int> CalculateIntegral(double a, double b, double eps, string integral)
        {
            int n = 1;
            double i1 = Calculate(a, b, n, integral);
            double i2;
            do
            {
                n *= 2;
                i2 = Calculate(a, b, n, integral);
                
                double tmp = i2;
                i2 = i1;
                i1 = tmp;
            } while (Math.Abs(i1 - i2) >= eps);

            return new KeyValuePair<double, int>(i1, n);
        }
        protected abstract double Calculate(double a, double b, int n, string integral);


    }
}
