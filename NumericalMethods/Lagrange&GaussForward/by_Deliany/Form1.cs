using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathPolynom;

namespace by_Deliany
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridViewNodes.Columns.Add("1", "Node 1");
            dataGridViewNodes.Rows.Add(1);
            dataGridViewNodes.Rows.Add(1);
            dataGridViewNodes.AllowUserToAddRows = false;

            dataGridViewGaussNodes.Columns.Add("1", "X");
            dataGridViewGaussNodes.Columns[0].Width = 75;
            dataGridViewGaussNodes.Columns.Add("2", "Y");
            dataGridViewGaussNodes.Columns[1].Width = 75;
            dataGridViewGaussNodes.Rows.Add(1);
            dataGridViewGaussNodes.AllowUserToAddRows = false;

            dataGridViewDifferenceTable.AllowUserToAddRows = false;
        }

        public int nodesCount = 2;
        List<double> nodes = new List<double>();
        List<double> values = new List<double>();
        private Lagrange lagrangePolynomial;
        
        public string MakeNormalForm(Polynomial polynom)
        {
            string result = "";
            for (int i = 0; i < polynom.Order; ++i )
            {
                if(polynom[i] != 0)
                {
                    int deg = (polynom.Order - i - 1);

                    //result += string.Format("{0}{1}", ((polynom[i] > 0 && result != string.Empty) ? "+" : "") + polynom[i],
                    //                        (deg > 0
                    //                             ? ("*x" +
                    //                                (deg > 1
                    //                                     ? ("^" + deg)
                    //                                     : ""
                    //                                ))
                    //                             : ""));

                    /* write coefficient if necessary                    */

                    // force plus sign if necessary
                    // plus sign isn't needed if coefficient is on first place of polynom
                    if (polynom[i] > 0 && result != string.Empty)
                    {
                        result += "+";
                    }

                    // no necessary to write coefficient equal to '1' or '-1' behind our x's
                    // force minus sign if there's coefficient equal to -1
                    if (Math.Round(polynom[i], 5) == -1 && deg !=0)
                    {
                        result += "-";
                    }
                    if(Math.Abs(Math.Round(polynom[i],5)) != 1  || deg == 0)
                    {
                        result += Math.Round(polynom[i],3);
                    }

                    // force '*x' if degree of x is bigger than 0
                    // force '^degree' if degree is bigger than 1
                    if(deg > 0)
                    {
                        // force * if previous coefficient wasn't equal to '1'
                        if (Math.Abs(Math.Round(polynom[i], 5)) != 1)
                        {
                            result += "*";
                        }
                        result += "x";
                        if(deg > 1)
                        {
                            result += "^" + deg;
                        }
                    }
                }
            }
            return result;
        }

        public void PolynomFunction()
        {
            lagrangePolynomial = new Lagrange(nodes.ToArray(), values.ToArray());
            string result = MakeNormalForm(lagrangePolynomial.LagrangePolynomial());
            label1.Text = "Polynom: "+ result;
        }

        private void buttonAddCol_Click(object sender, EventArgs e)
        {
            if (dataGridViewNodes.Columns.Count < 20)
            {
                dataGridViewNodes.Columns.Add(nodesCount.ToString(), "Node " + nodesCount);
                nodesCount++;
            }
        }

        private void buttonDelCol_Click(object sender, EventArgs e)
        {
            if (dataGridViewNodes.Columns.Count > 1)
            {
                nodesCount--;
                dataGridViewNodes.Columns.RemoveAt(dataGridViewNodes.Columns.Count - 1);
            }
        }

        private void buttonLagrange_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            values.Clear();
            try
            {
                foreach (DataGridViewCell x in dataGridViewNodes.Rows[0].Cells)
                {
                    nodes.Add(double.Parse(x.Value.ToString()));
                }
                foreach (DataGridViewCell y in dataGridViewNodes.Rows[1].Cells)
                {
                    values.Add(double.Parse(y.Value.ToString()));
                }

                PolynomFunction();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void buttonAddNodeGauss_Click(object sender, EventArgs e)
        {
            if(dataGridViewGaussNodes.Rows.Count < 20)
            {
                dataGridViewGaussNodes.Rows.Add(1);
            }
        }

        private void buttonDelNodeGauss_Click(object sender, EventArgs e)
        {
            if(dataGridViewGaussNodes.Rows.Count > 1)
            {
                this.dataGridViewGaussNodes.AllowUserToAddRows = false;
                dataGridViewGaussNodes.Rows.RemoveAt(dataGridViewGaussNodes.Rows.Count - 1);
                this.dataGridViewGaussNodes.AllowUserToAddRows = true;
            }
        }

        private void buttonGauss_Click(object sender, EventArgs e)
        {
            // organizing difference table
            dataGridViewDifferenceTable.Columns.Clear();
            string[] nums = new string[] { "¹","²","³","⁴","⁵","⁶","⁷","⁸","⁹" };
            for(int i = 0; i < dataGridViewGaussNodes.Rows.Count - 1; ++i)
            {
                dataGridViewDifferenceTable.Columns.Add(i.ToString(), "Δ" + (i == 0 ? "" : nums[i]) + "y");
            }
            dataGridViewDifferenceTable.Rows.Add(dataGridViewGaussNodes.Rows.Count - 1);

            nodes.Clear();
            values.Clear();

            try
            {
                foreach (DataGridViewRow row in dataGridViewGaussNodes.Rows)
                {
                    nodes.Add(double.Parse(row.Cells[0].Value.ToString()));
                    values.Add(double.Parse(row.Cells[1].Value.ToString()));
                }

                GaussForwardInterpolation f = new GaussForwardInterpolation(nodes.ToArray(),values.ToArray());

                label2.Text = "Result: " + Math.Round(f.GaussInterpolation( double.Parse(textBox1.Text) ),3);
                for (int i = 1; i < f.Diff_table.GetLength(0); i++)
                {
                    for (int j = 0; j < f.Diff_table[i].Length; j++)
                    {
                        dataGridViewDifferenceTable.Rows[j].Cells[i - 1].Value = Math.Round(f.Diff_table[i][j], 4);
                    }
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
      }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if(lagrangePolynomial != null)
                {
                    double checkResult =
                        lagrangePolynomial.LagrangePolynomial().Calculate(double.Parse(textBoxCheck.Text));
                    labelCheck.Text = "Check result: " + Math.Round(checkResult, 3);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
