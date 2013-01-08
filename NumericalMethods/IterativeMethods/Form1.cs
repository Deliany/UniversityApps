using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ZedGraph;



namespace IterativeMethods
{
    public partial class Form1 : Form
    {
        const double Eps = 1e-6;

        public Form1()
        {
            InitializeComponent();
            zedIterative.GraphPane.Title.Text = "x = |cos(x)|^(1/2)";
            zedChord.GraphPane.Title.Text = "x^3 + 18x - 83 = 0";
            IterativePrepare(10);
            ChordPrepare(3, 8);
            NewtonPrepare(0.5);

        }
        private void IterativePrepare(double xs)
        {

            zedIterative.GraphPane.CurveList.Clear();
            MyExtension.Function f = (x =>Math.Pow(Math.Abs(Math.Cos(x)),0.5));

            zedIterative.Graph(f, -10, 10, 0.1, Color.Red);
            zedIterative.Graph(x => x, -10, 10, 1, Color.Blue);


            PointPairList list = new PointPairList();

            double x0,x1 = xs;
            iterativeLogger.Items.Clear();
            list.Add(x1, f(x1));
            do
            {
                x0 = x1;
                x1 = f(x0);
                list.Add(x0, x1);
                iterativeLogger.Items.Add(
                    String.Format("X =  + {0:0.#####} + ; f(x) =  + {1:0.########}", x1, f(x1)));
            } while (Math.Abs(x1 - x0) > Eps);

            
            LineItem line = zedIterative.GraphPane.AddCurve("", list, Color.Green, SymbolType.Diamond);
            line.Line.IsVisible = false;

            zedIterative.AxisChange();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var d = double.Parse(iterativeInput.Text);
                IterativePrepare(d);
                this.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something bad happened:" + ex.Message);
            }
        }

        private void ChordPrepare(double a, double b)
        {
            zedChord.GraphPane.CurveList.Clear();

            MyExtension.Function f = (x => x*x*x - 18*x - 83);
            
            zedChord.Graph(f, 2, 10, 0.1, Color.Blue);

           double x0 = a, x1 = b, x2;

            PointPairList list = new PointPairList();
            ChordLogger.Items.Clear();

            do
            {
                x2 = x1 - f(x1)*(x1 - x0)/(f(x1) - f(x0));
                x0 = x1;
                x1 = x2;
                list.Add(x2, f(x2));

                ChordLogger.Items.Add(
                    String.Format("x = {0:0.#####}, f(x) = {1:0.##########}", x2, f(x2)));

            } while (Math.Abs(f(x2)) > Eps);


            LineItem line = zedChord.GraphPane.AddCurve("", list, Color.Green, SymbolType.Diamond);
            line.Line.IsVisible = false;

            zedChord.AxisChange();
        }

        private void NewtonPrepare(double xs)
        {
            zedNewton.GraphPane.CurveList.Clear();

            MyExtension.Function f = (x => - Math.Pow(x, 3) + Math.Cos(x));
            MyExtension.Function fs = (x => -3*x*x - Math.Sin(x));

            zedNewton.Graph(f, -2, 2, 0.05, Color.Red);

            double x0, x1 = xs;

            PointPairList list = new PointPairList();
            list.Add(x1, f(x1));

            NewtonLogger.Items.Clear();

            do
            {
                x0 = x1;
                x1 = x0 - f(x0)/fs(x0);
                list.Add(x1, f(x1));
                NewtonLogger.Items.Add(
                    String.Format("x={0:0.###},f(x)={1:0.############}", x1, f(x1)));
            } while (Math.Abs(f(x1)) > Eps);

            LineItem line = zedNewton.GraphPane.AddCurve("", list, Color.Green, SymbolType.Diamond);
            line.Line.IsVisible = false;

            zedNewton.AxisChange();
        }
    }
}

