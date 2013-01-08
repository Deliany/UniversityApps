using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    class SimpsonRule:NumericalIntegration
    {
        protected override double Calculate(double a, double b, int n, string integral)
        {
            double res = 0;
            double h = (b - a) / n;
            bool even = false;
            for (int i = 1; i < n; ++i)
            {
                res += (even ? 2 : 4) * MyParser.calculate(integral, (a + i * h));
                even = !even;
            }
            res += MyParser.calculate(integral, a) + MyParser.calculate(integral, b);
            res *= h/3;
            return res;
        }
        public override string ToString()
        {
            return "Simpson";
        }
    }
}
