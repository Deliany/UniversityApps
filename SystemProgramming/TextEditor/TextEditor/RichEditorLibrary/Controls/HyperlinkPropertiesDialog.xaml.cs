using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Adastra.RichEditorLibrary.Properties;
using System.Diagnostics;

namespace Adastra.RichEditorLibrary
{
    /// <summary>
    /// Interaction logic for HyperlinkPropertiesDialog.xaml
    /// </summary>
    public partial class HyperlinkPropertiesDialog : Window
    {
        #region | Instance variables |

        private Hyperlink m_hyperlink = null;
        private string m_strHyperlinkDesc = String.Empty;
        private string m_strNavigateUri = String.Empty;

        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Image to bind with</param>
        public HyperlinkPropertiesDialog(Hyperlink hyperlink)
        {
            InitializeComponent();
            this.BindHyperlink(hyperlink);
            this.Loaded += new RoutedEventHandler(HyperlinkPropertiesDialog_Loaded);
        }

        void HyperlinkPropertiesDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavigateToURL();
        }

        #endregion //| Constructor stuff |

        #region | HyperlinkPropertiesDialog interface |
        /// <summary>
        /// Get bind image
        /// </summary>
        public Hyperlink Hyperlink
        {
            get { return this.m_hyperlink; }
            set 
            {
                try
                {
                    this.m_hyperlink = value;
                    if (null != this.m_hyperlink)
                    {
                        this.HyperlinkDesc = (null == this.m_hyperlink.ToolTip ? String.Empty : ((string)((ToolTip)this.m_hyperlink.ToolTip).Content));
                        this.NavigateUri = this.m_hyperlink.NavigateUri.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, "Set Hyperlink failed: " + ex);
                }
            }
        }

        public string HyperlinkDesc
        {
            get { return this.m_strHyperlinkDesc; }
            set { this.m_strHyperlinkDesc = value; }
        }

        public string NavigateUri
        {
            get { return ((this.m_strNavigateUri.StartsWith("http://") || this.m_strNavigateUri.StartsWith("mailto:")) ? this.m_strNavigateUri : ("http://" + this.m_strNavigateUri)); }
            set { this.m_strNavigateUri = value; }
        }


        #endregion | HyperlinkPropertiesDialog interface |

        #region | Private stuff |
        /// <summary>
        /// Bind DataContext
        /// </summary>
        private void BindHyperlink(Hyperlink hyperlink)
        {
            this.DataContext = this;
            this.Hyperlink = hyperlink;

        }

        /// <summary>
        /// Cancell button click handler
        /// </summary>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// OK button click handler
        /// </summary>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.m_strNavigateUri))
            {
                try
                {
                    new Uri(this.m_strNavigateUri);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} ({1})", Res.txtNotValidUrlAddress, this.m_strNavigateUri), Res.txtEditorName);
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void OnNavigateUriLostFocus(object sender, RoutedEventArgs e)
        {
            this.NavigateToURL();
        }

        private void NavigateToURL()
        {
            if (!String.IsNullOrEmpty(this.m_strNavigateUri) && this.m_strNavigateUri.StartsWith("http://"))
            {
                try
                {
                    Uri uri = new Uri(this.NavigateUri);
                    this.m_webBrowser.Navigate(uri);
                }
                catch(Exception){}
            }
        }

        private void OnPathGotFocus(object sender, RoutedEventArgs e)
        {
            if (0 == this.m_txbxNavigateUri.Text.Length)
            {
                this.m_txbxNavigateUri.Text = "http://";
            }
        }

        #endregion //| Private stuff |


    }
}
