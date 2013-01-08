using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;


namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<NumericalIntegration> methods = new List<NumericalIntegration>();
        ObservableCollection<IntegrateResult> _dummyCollection = new ObservableCollection<IntegrateResult>();
        private bool mode = false;

        public MainWindow()
        {
            InitializeComponent();
            function.Focus();
            
            //methods.Add(new LeftRectangleRule());
            //methods.Add(new RightRectangleRule());
            methods.Add(new MiddleRectangleRule());
            methods.Add(new TrapezoidRule());
            methods.Add(new SimpsonRule());
            methods.Add(new GaussRule());
            methods.Add(new ChebyshevRule());
        }
        
        public class IntegrateResult
        {
            public string Method { get; set; }
            public double Value { get; set; }
            public int Intervals { get; set; }
            public string Error { get; set; }
        }

        // cos(x/2) ^ 2 
        // (1/2)*(x+sin(x))

        //
        //
        private void integrate()
        {
            double a = double.Parse(intervalA.Text);
            double b = double.Parse(intervalB.Text);

            double eps = double.Parse(epsilon.Text);
            if (eps >= 1 || eps < 0)
            {
                throw new Exception("Epsilon must me between 0 and 1");
            }

            double exactValue = MyParser.calculate(original.Text, b) - MyParser.calculate(original.Text, a);
            _dummyCollection.Clear();
            foreach (NumericalIntegration method in methods)
            {
                KeyValuePair<double, int> res = method.CalculateIntegral(a, b, eps, function.Text);
                double error = Math.Abs(res.Key - exactValue);
                _dummyCollection.Add(new IntegrateResult { Method = method.ToString(), Value = res.Key, Intervals = res.Value, Error = string.Format("{0:0.########}", error) });
            }
            _dummyCollection.Add(new IntegrateResult { Method = "Exact value", Value = exactValue });
            dataGrid1.ItemsSource = _dummyCollection;
        }

        private void integrateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             if(mode == false)
             {
                 integrate();
             }
             else
             {
                 integrateDoubles();
             }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void integrateDoubles()
        {
            double a = double.Parse(intervalA.Text);
            double b = double.Parse(intervalB.Text);
            double c = double.Parse(intervalC.Text);
            double d = double.Parse(intervalD.Text);

            double eps = double.Parse(epsilon.Text);
            if (eps >= 1 || eps < 0)
            {
                throw new Exception("Epsilon must me between 0 and 1");
            }

            DoubleIntegrals p = new DoubleIntegrals(function.Text);




            KeyValuePair<double, int> res;
            _dummyCollection.Clear();
            _dummyCollection.Add(new IntegrateResult
                                     {Method = "SimpsonSmall", Value = p.IntegralSimpsonSmall(a, b, c, d)});
            res = p.Calculate(a, b, c, d, p.IntegralSimpson, eps);
            _dummyCollection.Add(new IntegrateResult
                                     {Method = "SimsponBig", Value = res.Key, Intervals = res.Value });
            res = p.Calculate(a, b, c, d, p.IntegralMonteCarlo, eps);
            _dummyCollection.Add(new IntegrateResult
                                     {Method = "Monte Carlo", Value = res.Key, Intervals = res.Value});
            dataGrid1.ItemsSource = _dummyCollection;
        }

        private void changeMode()
        {
            label9.Visibility = mode ? Visibility.Hidden : Visibility.Visible;
            label10.Visibility = mode ? Visibility.Hidden : Visibility.Visible;
            intervalC.Visibility = mode ? Visibility.Hidden : Visibility.Visible;
            intervalD.Visibility = mode ? Visibility.Hidden : Visibility.Visible;
            label1.Visibility = !mode ? Visibility.Hidden : Visibility.Visible;
            original.Visibility = !mode ? Visibility.Hidden : Visibility.Visible;
            integrateButton.Margin = !mode ? new Thickness(614, 190, 0, 0) : new Thickness(477, 190, 0, 0);
            mode = !mode;
        }
        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            changeMode();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            changeMode();
        }
    }
}
