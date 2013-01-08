using System;
using System.Windows;
using Visiblox.Charts;

namespace iGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Graph.SolveDiffEquation(15, 20, 14, 21, 20);
        }

        private void SolveButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                double T = double.Parse(T_TextBox.Text);
                double b = double.Parse(b_TextBox.Text);
                double sigma = double.Parse(Sigma_TextBox.Text);
                double f = double.Parse(f_TextBox.Text);
                int n = int.Parse(n_TextBox.Text);

                //if (n > 100) Graph.exampleLine.ShowPoints = false;
                //else Graph.exampleLine.ShowPoints = true;

                Graph.SolveDiffEquation(T, b, sigma, f, n);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
