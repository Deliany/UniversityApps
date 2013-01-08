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
using Microsoft.Win32;
using System.ComponentModel;

namespace Adastra.RichEditorLibrary
{
    /// <summary>
    /// Interaction logic for ImagePropertiesDialog.xaml
    /// </summary>
    public partial class ImagePropertiesDialog : Window, INotifyPropertyChanged
    {
        #region | Instance variables |

        Image m_imageBackup = new Image();
        Image m_image = null;

        //private double m_dWidth = double.NaN;
        //private double m_dHeight = double.NaN;
        private string m_strHyperlink = String.Empty;
        private string m_strImagePath = String.Empty;

        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Image to bind with</param>
        public ImagePropertiesDialog(Image image)
        {
            InitializeComponent();
            this.BindImage(image);
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        #endregion //| Constructor stuff |

        #region | ImagePropertiesDialog interface |
        #region | INotifyPropertyChanged Events |

        /// <summary>
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="strInfo"></param>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string strInfo)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(strInfo));
            }
        }

        #endregion //| INotifyPropertyChanged Events |

        /// <summary>
        /// Get bind image
        /// </summary>
        public Image Image
        {
            get { return this.m_image; }
            set { this.m_image = value; }
        }

        /// <summary>
        /// Get/Set image hyperlink
        /// </summary>
        public string ImageHyperlink
        {
            get { return this.m_strHyperlink; }
            set { this.m_strHyperlink = value; }
        }

        /// <summary>
        /// Get/Set image path
        /// </summary>
        public string ImagePath
        {
            get { return this.m_strImagePath; }
            set 
            {
                this.m_strImagePath = value;
                this.NotifyPropertyChanged("ImagePath");
            }
        }

        /// <summary>
        /// Get/Set image width
        /// </summary>
        public double ImageWidth
        {
            get { return (null == this.Image ? double.NaN : this.Image.Width); }
            set 
            {
                if (null != this.Image)
                {
                    this.Image.Width = value;
                }
            }
        }

        /// <summary>
        /// Get/Set image height
        /// </summary>
        public double ImageHeight
        {
            get { return (null == this.Image ? double.NaN : this.Image.Height); }
            set 
            {
                if (null != this.Image)
                {
                    this.Image.Height = value;
                }
            }
        }

        #endregion | ImagePropertiesDialog interface |

        #region | Private stuff |
        /// <summary>
        /// Bind DataContext
        /// </summary>
        private void BindImage(Image image)
        {
            this.DataContext = this;
            this.Image = image;

            if(null == this.Image)
            {
                return;
            }

            this.m_imageBackup.Width = image.Width;
            this.m_imageBackup.Height = image.Height;
            
            //System.Windows.Media.Imaging.BitmapFrame bm = new  
            //BitmapImage bitmapImage = this.Image.Source as BitmapImage;
            BitmapSource bitmapSource = this.Image.Source as BitmapSource;
            //string strUr = bitmapSource.ToString();

            if (null != bitmapSource)
            {
                this.SetImageInfo(bitmapSource);
            }

            if(String.IsNullOrEmpty(this.ImagePath) && !String.IsNullOrEmpty(this.Image.Tag as string))
            {
                this.ImagePath = this.Image.Tag as string;
            }
            //this.SetImageInfo(((BitmapImage)this.Image.Source));
        }

        private void SetImageInfo(BitmapSource bitmapSource)
        {
            this.m_lblPictureInfoName.Content = this.m_lblPictureInfoSize.Content = String.Empty;

            if (null == bitmapSource)
            {
                return;
            }
            string strImagePath = bitmapSource.ToString();
            int iImageName = strImagePath.LastIndexOf('/');
            if (-1 != iImageName)
            {
                this.m_lblPictureInfoName.Content = strImagePath.Substring(iImageName+1);
            }

            this.ImagePath = strImagePath;
            
            this.m_lblPictureInfoSize.Content = String.Format("{0:0.##} x {1:0.##} (šířka x výška v pixelech)", bitmapSource.Width, bitmapSource.Height);
            this.m_strHyperlink = (null != this.Image.Tag ? (string)this.Image.Tag : "");
        }

        public bool PictureSourceChanged
        {
            get
            {
                BitmapSource bitmapSource = this.Image.Source as BitmapSource;
                string strUrl = (null != bitmapSource ? bitmapSource.ToString() : String.Empty);
                return (strUrl.ToLower() != this.ImagePath.ToLower());
            }
        }

        //private void SetImageInfo(BitmapImage bitmapImage)
        //{
        //    this.m_lblPictureInfoName.Content = this.m_lblPictureInfoSize.Content = String.Empty;

        //    if (null == bitmapImage)
        //    {
        //        return;
        //    }
        //    string strImagePath = bitmapImage.UriSource.ToString();
        //    int iImageName = strImagePath.LastIndexOf('/');
        //    if (-1 != iImageName)
        //    {
        //        this.m_lblPictureInfoName.Content = strImagePath.Substring(iImageName);
        //    }

        //    this.m_txbxPath.Text = strImagePath;
        //    this.m_lblPictureInfoSize.Content = String.Format("{0:0.##} x {1:0.##} (šířka x výška v pixelech)", bitmapImage.Width, bitmapImage.Height);
        //    this.m_strHyperlink = (null != this.Image.Tag ? (string)this.Image.Tag : "");
        //}

        /// <summary>
        /// Cancell button click handler
        /// </summary>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            if (null != this.Image && null != this.m_imageBackup)
            {
                this.Image.Width = this.m_imageBackup.Width;
                this.Image.Height = this.m_imageBackup.Height;
            }
            this.Close();
        }

        /// <summary>
        /// OK button click handler
        /// </summary>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.ImagePath))
            {
                MessageBox.Show(String.Format("{0}", Res.txtNotValidPictureAddress), Res.txtEditorName);
                return ;
            }

            try
            {
                new Uri(this.ImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0} ({1})", Res.txtNotValidPictureAddress, this.ImagePath), Res.txtEditorName);
                return;
            }

            int iDot = this.ImagePath.Length - this.ImagePath.LastIndexOf('.');
            iDot += (-1 == iDot ? 4 : -1);

            string strExt = this.ImagePath.ToLower().Substring(this.ImagePath.Length - iDot, iDot);

            if(strExt != "jpg" &&
               strExt != "bmp" &&
               strExt != "png" &&
               strExt != "ico" &&
               strExt != "gif" &&
               strExt != "tif")
            {
                if (MessageBoxResult.Yes != MessageBox.Show(String.Format("{0} (.{1})", Res.txtNotValidPictureType, strExt), Res.txtEditorName, MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    return;
                }
            }

            if (!String.IsNullOrEmpty(this.ImageHyperlink))
            {
                try
                {
                    new Uri(this.ImageHyperlink);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} ({1})", Res.txtNotValidUrlAddress, this.ImageHyperlink), Res.txtEditorName);
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// SetDefaults button click handler
        /// </summary>
        private void OnSetDefaultsClick(object sender, RoutedEventArgs e)
        {
            //AcmBitmapImageHolder acmBitmapImageHolder = new AcmBitmapImageHolder(this.Image,((BitmapImage)this.Image.Source).UriSource);
            try
            {
                BitmapSource bitmapSource = this.Image.Source as BitmapSource;
                if (null != bitmapSource)
                {
                    AcmBitmapImageHolder acmBitmapImageHolder = new AcmBitmapImageHolder(this.Image, new Uri(bitmapSource.ToString()));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnSetDefaultsClick ex: "+ex);
            }
            //this.BindImage(acmBitmapImageHolder.Image);
        }

        private void OnPathGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(null == textBox)
            {
                return;
            }

            if (0 == textBox.Text.Length)
            {
                textBox.Text = "http://";
            }
        }

        private void OnNavigateUriLostFocus(object sender, RoutedEventArgs e)
        {
            this.NavigateToURL();
        }

        private void NavigateToURL()
        {
            if (!String.IsNullOrEmpty(this.m_strImagePath) && this.m_strImagePath.StartsWith("http://"))
            {
                try
                {
                    Uri uri = new Uri(this.m_strImagePath);
                    this.m_webBrowser.Source = new BitmapImage(new Uri(this.m_strImagePath)); 
                }
                catch (Exception) { }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.NavigateToURL();
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string strDocument = String.Empty;

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false;
            openFileDialog.Title = Res.txtEnterPicturePath;
            openFileDialog.Filter = Res.txtPictures;

            if (true == openFileDialog.ShowDialog())
            {
                this.ImagePath = openFileDialog.FileName;
                this.NavigateToURL();
            }
        }

        #endregion //| Private stuff |
    }
}
