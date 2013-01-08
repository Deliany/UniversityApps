using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathPolynom
{
    public class Lagrange
    {
        private double[] nodes;
        private double[] values;

        public Lagrange(double[] nodes, double[] values)
        {
            this.nodes = nodes;
            this.values = values;
        }

        public Polynomial LagrangePolynomial()
        {
            Polynomial result = new Polynomial(0);
            for(int i = 0; i < values.Length; ++i)
            {
                result = result + values[i]*BasicPolynomial(i);
            }

            return result;
        }

        public Polynomial BasicPolynomial(int k)
        {
            Polynomial result = new Polynomial(1);
            for (int i = 0; i < nodes.Length; i++)
            {
                if (i != k)
                {
                    result *= new Polynomial(1, -nodes[i]) / (nodes[k] - nodes[i]);
                }
            }
            return result;
        }
    }
}
