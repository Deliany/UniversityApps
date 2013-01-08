using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using Adastra.RichEditorLibrary.Properties;


namespace Adastra.RichEditorLibrary
{
    public class AcmBitmapImageHolder
    {
        #region | Instance variables |

        public Image Image = null;
        public BitmapImage BitmapImage { get; set; }

        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// Constructor
        /// </summary>
        public AcmBitmapImageHolder(Uri uriImgSource)
            :this(new Image(),uriImgSource)
        {
        }

        public AcmBitmapImageHolder(Image image,Uri uriImgSource)
        {
            this.Image = image;
            this.BitmapImage = new BitmapImage(uriImgSource);
            this.Image.Width = (1.0 == this.BitmapImage.Width ?  50 : this.BitmapImage.Width);
            this.Image.Height = (1.0 == this.BitmapImage.Height ?  50 : this.BitmapImage.Height);
            //this.BitmapImage.UriSource = uriImgSource;
            this.BitmapImage.DownloadCompleted += new EventHandler(BitmapImage_DownloadCompleted);
            this.BitmapImage.DownloadFailed += new EventHandler<System.Windows.Media.ExceptionEventArgs>(BitmapImage_DownloadFailed);

            this.Image.Source = this.BitmapImage;
            this.Image.Tag = uriImgSource.AbsoluteUri;
        }

        #endregion //| Constructor stuff |

        #region | Private stuff |

        void BitmapImage_DownloadCompleted(object sender, EventArgs e)
        {
            if (sender is BitmapImage)
            {
                this.Image.Width = this.BitmapImage.Width;
                this.Image.Height = this.BitmapImage.Height;
            }
        }

        void BitmapImage_DownloadFailed(object sender, System.Windows.Media.ExceptionEventArgs e)
        {
            MessageBox.Show(Res.txtLoadPictureFailed + "\n\r" + e.ErrorException, Res.txtEditorName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Debug.WriteLine("BitmapImage_DownloadFailed: " + e.ErrorException);
            // TODO: dodelat download failed...Res.imgNotFound;
            //this.Image.Source = System.Windows.Media.Imaging.this.BitmapImage.StreamSource = Res.imgNotFound.GetHbitmap;
        }
        
        #endregion //| Private stuff |
    }
}
