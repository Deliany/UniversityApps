using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace IterativeMethods
{
    public static class MyExtension
    {
        public delegate double Function(double x);

        public static void Graph(this ZedGraphControl zed, Function f,
            double a, double b, double h, Color c)
        {
            PointPairList list = new PointPairList();
            for (double x = a; x <= b; x += h)
            {
                list.Add(x, f(x));

                zed.GraphPane.AddCurve("", list, c, SymbolType.None);

                zed.AxisChange();
            }
        }
        public static void Graph(this ZedGraphControl zed, Function f,
            double a, double b, double h)
        {
            zed.Graph(f, a, b, h, Color.Black);
        }
    }
}
