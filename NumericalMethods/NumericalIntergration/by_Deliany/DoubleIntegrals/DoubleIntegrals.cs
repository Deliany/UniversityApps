using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;

namespace by_Deliany
{
    public class DoubleIntegrals
    {
        private string Integral { get; set; }

        public DoubleIntegrals(string integral)
        {
            Integral = integral;
        }
        private double f(double x, double y)
        {
            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(Integral);

            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);

            List<VariableValue> variables = new List<VariableValue>();
            variables.Add(new VariableValue(x, "x"));
            variables.Add(new VariableValue(y, "y"));
            double res = ToolsHelper.Calculator.Calculate(compiledExpression, variables);

            return res;
        }
        public delegate double MyDelegate(double a, double b, double c, double d, int eps);

        public KeyValuePair<double,int> Calculate(double a, double b, double c, double d, MyDelegate function, double eps )
        {

            int n = 1;
            double result = function(a, b, c, d, n);
            double tempResult;

            do
            {
                tempResult = result;
                n *= 2;
                result = function(a, b, c, d, n);
            } while (Math.Abs(tempResult - result) > eps);

            return new KeyValuePair<double, int>(result, n);
        }

        public double IntegralSimpson(double a, double b, double c, double d, int n)
        {
            double h1 = (b - a) / n;
            double h2 = (d - c) / n;
            double result = 0.0;
            int N = 2 * n - 1;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    result += IntegralSimpsonSmall(a + h1 * (i - 1), a + h1 * i, c + h2 * (j - 1), c + h2 * j);
                }
            }
            return result;
        }

        public double IntegralSimpsonSmall(double a, double b, double c, double d)
        {
            double AB = (a + b) / 2;
            double CD = (c + d) / 2;
            double result = f(a, c) + f(b, c) + f(a, d) + f(b, d);
            result += 4 * (f(AB, c) + f(AB, d) + f(a, CD) + f(b, CD));
            result += 16 * f(AB, CD);
            return (result * (b - a) * (d - c) / 36);
        }

        public double IntegralMonteCarlo(double a, double b, double c, double d, int n)
        {
            double x, y;
            double result = 0.0;
            RandomMonteCarlo rand = new RandomMonteCarlo();
            for (int i = 0; i < n; i++)
            {
                x = rand.Next();
                y = rand.Next();
                result += (b - a) * (d - c) * f(a + (b - a) * x, c + (d - c) * y);
            }
            return (result / n);
        }
    }
}
