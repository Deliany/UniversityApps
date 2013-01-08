using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace iDraw
{
    /// <summary>
    /// Interaction logic for ModalDialog.xaml
    /// </summary>
    public partial class ModalDialog : UserControl
    {
        public ModalDialog()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        private bool _hideRequest = false;
        private bool _result = false;
        private UIElement _parent;

        public void SetParent(UIElement parent)
        {
            _parent = parent;
        }

        public bool ShowHandlerDialog()
        {
            Visibility = Visibility.Visible;

            _parent.IsEnabled = false;

            _hideRequest = false;
            while (!_hideRequest)
            {
                // HACK: Stop the thread if the application is about to close
                if (this.Dispatcher.HasShutdownStarted ||
                    this.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                // HACK: Simulate "DoEvents"
                this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }

            return _result;
        }

        private void HideHandlerDialog()
        {
            _hideRequest = true;
            Visibility = Visibility.Hidden;
            _parent.IsEnabled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = true;
            HideHandlerDialog();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _result = false;
            HideHandlerDialog();
        }
    }
}
