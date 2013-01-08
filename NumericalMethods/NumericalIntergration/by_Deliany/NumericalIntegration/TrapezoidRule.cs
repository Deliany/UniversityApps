using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    class TrapezoidRule:NumericalIntegration
    {
        protected override double Calculate(double a, double b, int n, string integral)
        {
            double res = 0;
            double h = (b - a) / n;

            for (int i = 1; i < n; ++i)
            {
                res += MyParser.calculate(integral, (a + i * h));
            }
            res += (MyParser.calculate(integral, a) + MyParser.calculate(integral, b))/2;
            res *= h;
            return res;
        }
        public override string ToString()
        {
            return "Trapezoid";
        }
    }
}
