using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;

namespace NewtonModified_Method
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Parser parser = new Parser();
        private int iterCount = 0;
        private Hashtable vars = new Hashtable();
        private Matrix fx;
        private Matrix fx1;
        private string variable = "xyz";
        private bool flagEnd = false;
        double Epsilon = 1e-3;

        private double calculate(string input, Hashtable args)
        {
            int n = (int)NewtonUpDown.Value;

            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(input);

            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);

            List<VariableValue> variables = new List<VariableValue>();
            for(int i = 0; i < n; ++i)
            {
                variables.Add(new VariableValue((double)args[variable[i].ToString()], variable[i].ToString()));
            }
            double res = ToolsHelper.Calculator.Calculate(compiledExpression, variables);

            return res;
        }
        //private double calculateFormula(string input, Hashtable args)
        //{
        //    string expression = input;
        //    for (int i = 0; i < variable.Length; ++i)
        //    {
        //        Regex regEx = new Regex(variable[i].ToString(), RegexOptions.IgnoreCase);
        //        expression = regEx.Replace(expression, args[variable[i].ToString()].ToString());
        //    }
        //    parser.Evaluate(expression);
        //    return parser.Result;
        //}

        private double Differencial(string formula, Hashtable args, string variable, double eps)
        {
            double f0 = calculate(formula, args);
            args[variable] = (double)args[variable] + eps;
            double f1 = calculate(formula, args);
            return (f1 - f0) / eps;
        }

        private void NewtonUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (NewtonUpDown.Value == 2)
            {
                NewtonZ.Enabled = false;
            }
            if (NewtonUpDown.Value == 3)
            {
                NewtonZ.Enabled = true;
            }
        }

        private void nextNewtonButton_Click(object sender, EventArgs e)
        {
            if(!flagEnd)
            {
                NewtonMethod();
            }
        }

        private void NewtonMethod()
        {
            int n = (int)NewtonUpDown.Value;
            if (iterCount == 0)
            {
                vars["x"] = double.Parse(NewtonX.Text);
                vars["y"] = double.Parse(NewtonY.Text);
                vars["z"] = double.Parse(NewtonZ.Text);
            }


            double eps = 0.00001;
            Matrix f = new Matrix(n, 1);



            #region Jacobian Matrix

            if (iterCount == 0)
            {



                fx = new Matrix(n, n);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        fx.arr[i][j] = Differencial(NewtonEquationTextBox.Lines[i], vars, variable[j].ToString(), eps);
                    }
                }
                fx1 = fx.Inverse();
            }

            #endregion



            for (int i = 0; i < n; i++)
            {
                f.arr[i][0] = calculate(NewtonEquationTextBox.Lines[i], vars);
            }




            string resJacobian = "Det J = " + string.Format("{0:0.#}", fx.Determinant()) + "\r\n";


            for (int i = 0; i < n; i++)
            {
                resJacobian += "| ";
                for (int j = 0; j < n; j++)
                {
                    resJacobian += string.Format("{0:0.###}", fx.arr[i][j]) + "  ";
                }
                resJacobian += " |\r\n\n";
            }


            resJacobian += "Inversed J: \r\n";
            for (int i = 0; i < n; i++)
            {
                resJacobian += "| ";
                for (int j = 0; j < n; j++)
                {
                    resJacobian += string.Format("{0:0.###}", fx1.arr[i][j]) + "  ";
                }
                resJacobian += " |\r\n";
            }


            Matrix res = fx1 * f;


            bool[] flags = new bool[3];
            
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(Convert.ToDouble(vars[variable[i].ToString()]) - (double)vars[variable[i].ToString()] - res.arr[i][0]) < Epsilon)
                    flags[i] = true;
                vars[variable[i].ToString()] = (double)vars[variable[i].ToString()] - res.arr[i][0];

            }



            string resValues = ""; if (flags[0] == true && flags[1] == true && flags[2] == true)
            {
                flagEnd = true;
                resValues += "Last result: ";
            }



            for (int i = 0; i < n; i++)
            {
                resValues += variable[i] + "=" + string.Format("{0}", vars[variable[i].ToString()]) + " \n";
            }
            resValues += "--------------------------------\n";
            NewtonResultTextBox.Text = resValues + NewtonResultTextBox.Text;
            ResultJacobianTextBox.Text = resJacobian;

            iterCount++;
            iterNewtonCount.Text = "iterations: " + iterCount.ToString();
        }

        private void clearNewtonButton_Click(object sender, EventArgs e)
        {
            NewtonResultTextBox.Text = "";
            ResultJacobianTextBox.Text = "";
            iterCount = 0;
            flagEnd = false;
            iterNewtonCount.Text = "iterations: 0";
        }

        private void EpsilonTextBox_Validated(object sender, EventArgs e)
        {
            double a;
            if(!double.TryParse(EpsilonTextBox.Text, out a))
            {
                EpsilonTextBox.Text = "1e-2";
            }
            else if (Convert.ToDouble(EpsilonTextBox.Text) > 1e-2)
            {
                EpsilonTextBox.Text = "1e-2";
            }

            Epsilon = Convert.ToDouble(EpsilonTextBox.Text);
        }

        private void calcNewtonButton_Click(object sender, EventArgs e)
        {
            while(!flagEnd)
            {
                NewtonMethod();
            }
        }
    }
}
