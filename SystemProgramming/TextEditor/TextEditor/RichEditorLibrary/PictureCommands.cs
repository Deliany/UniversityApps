using System;
using System.IO;
using System.ComponentModel;
using System.Printing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Xps;
using Adastra.RichEditorLibrary.Properties;
using System.Runtime.Serialization;

namespace Adastra.RichEditorLibrary
{
    public class TextPointerPaar : ISerializable
    {
        private TextPointer m_Start;
        private TextPointer m_End;

        public TextPointerPaar()
        {
            //this.Start = (TextPointer)TextPointer.Missing;
            //this.End = (TextPointer)TextPointer.Missing;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public override string ToString()
        {
           return String.Empty;
        }

        public TextPointerPaar(TextPointer start,TextPointer end)
        {
            this.Start = start;
            this.End = end;
        }
        public TextPointer Start 
        {
            get { return m_Start; }
            set { m_Start = value; } 
        }
        public TextPointer End 
        {
            get { return m_End; }
            set { m_End = value; }
        }
    }

    /// <summary>
    /// Helper class for table commands exposed by RichTextEditor.
    /// </summary>
    internal class PictureCommands
    {
        #region | Instance variables |

        private static RoutedUICommand m_insertPictureCommand;
        private static RoutedUICommand m_insertHyperlinkCommand;
        private static RoutedUICommand m_editPictureCommand;

        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// static constructor
        /// </summary>
        static PictureCommands()
        {
            m_insertPictureCommand = new RoutedUICommand("InsertPicture","InsertPicture", typeof(RichTextEditor));
                                                        CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                                                        new CommandBinding(m_insertPictureCommand, OnInsertPicture, OnCanExecuteInsertPicture));

            m_insertHyperlinkCommand = new RoutedUICommand("InsertHyperlink", "InsertHyperlink",typeof(RichTextEditor));
                                                        CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor),
                                                        new CommandBinding(m_insertHyperlinkCommand, OnInsertHyperlink, OnCanExecuteInsertPicture));

            m_editPictureCommand = new RoutedUICommand("EditPicture", "EditPicture",typeof(RichTextEditor));
                                                        CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor),
                                                        new CommandBinding(m_editPictureCommand, OnEditPictureProperties, OnCanExecuteInsertPicture));

        }

        #endregion //| Constructor stuff |

        #region | PictureCommands interface |
        /// <summary>
        /// Command for inserting picture
        /// </summary>
        public static RoutedUICommand InsertPictureCommand
        {
            get{return m_insertPictureCommand;}
        }

        /// <summary>
        /// Command for inserting hyperlink
        /// </summary>
        public static RoutedUICommand InsertHyperlinkCommand
        {
            get {return m_insertHyperlinkCommand;}
        }

        /// <summary>
        /// Command for editing picture
        /// </summary>
        internal static RoutedUICommand EditPictureCommand
        {
            get {return m_editPictureCommand;}
        }

        #endregion //| PictureCommands interface |

        #region | Private methods |
        /// <summary>
        /// Handler for "CanExecute" question for commands
        /// </summary>
        private static void OnCanExecuteInsertPicture(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            RichTextEditor control = (RichTextEditor)target;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;

            // Disable pictures inside lists and hyperlinks
            if (Helper.HasAncestor(insertionPosition, typeof(List)) || Helper.HasAncestor(insertionPosition, typeof(Hyperlink)))
            {
                e.CanExecute = false;
            }
        }

        private static Hyperlink GetHyperlinkAncestor(TextPointer position)
        {
            Inline parent = position.Parent as Inline;

            while (parent != null && !(parent is Hyperlink))
            {
                parent = parent.Parent as Inline;
            }
            return parent as Hyperlink;
        }

        /// <summary>
        /// Handler for inserting of the hyperlink
        /// </summary>
        public static void OnInsertHyperlink(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;


            if (richTextBox.Selection.IsEmpty)
            {
                return;
            }

            TextPointer insertionPosition = richTextBox.Selection.Start;
            Paragraph paragraph = insertionPosition.Paragraph;
            Hyperlink hyperlink = null;

            Image image = Helper.GetImageAncestor(insertionPosition);
            InlineUIContainer inlineUIContainer = Helper.GetInlineUIContainer(insertionPosition);
            if (image != null)
            {
                OnEditPictureProperties(sender, e);
                return;
            }

            //textRange.Text
            //### Detect existing hyperlink
            //hyperlink = GetHyperlinkAncestor(insertionPosition);
            TextRange textRange = richTextBox.Selection;
            foreach (Inline inline in paragraph.Inlines)
            {
                //if (inline is Hyperlink && ((Hyperlink)inline).Tag is TextPointerPaar)
                if (inline is Hyperlink)
                {
                    hyperlink = (Hyperlink)inline;
                    TextRange textRangeHyper = new TextRange(hyperlink.ElementStart,hyperlink.ElementEnd);

                    //if (0 == richTextBox.Selection.Start.CompareTo(((TextPointerPaar)hyperlink.Tag).Start) && 0 == richTextBox.Selection.End.CompareTo(((TextPointerPaar)hyperlink.Tag).End))
                    if (textRange.Text == textRangeHyper.Text)
                    {
                        break;
                    }
                    else
                    {
                        hyperlink = null;
                    }
                }
            }

            HyperlinkPropertiesDialog hyperlinkPropertiesDlg = new HyperlinkPropertiesDialog(hyperlink);
            hyperlinkPropertiesDlg.ShowDialog();

            if (true == hyperlinkPropertiesDlg.DialogResult)
            {
                try
                {
                    Uri uri = new Uri(hyperlinkPropertiesDlg.NavigateUri); 
                    if (null == hyperlink)
                    {
                        hyperlink = new Hyperlink(richTextBox.Selection.Start, richTextBox.Selection.End);
                        //hyperlink.Tag = new TextPointerPaar(richTextBox.Selection.Start,richTextBox.Selection.End);
                    }
                    hyperlink.NavigateUri = uri;
                    /*
                    if (String.IsNullOrEmpty(hyperlinkPropertiesDlg.HyperlinkDesc))
                    {
                        hyperlink.ToolTip = null;
                    }
                    else
                    {
                        ToolTip toolTip = new ToolTip();
                        toolTip.Content = hyperlinkPropertiesDlg.HyperlinkDesc;
                        hyperlink.ToolTip = toolTip;
                    }*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} ({1}) :\n\r{2}",Res.txtLoadHyperlinkFailed,hyperlinkPropertiesDlg.NavigateUri,ex), Res.txtEditorName);
                }
            }

            //paragraph.Inlines.Add(hyperlink);
       }

        /// <summary>
        /// Handler for inserting of the picture
        /// </summary>
        public static void OnInsertLine(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.Text = String.Empty;
            }

            TextPointer insertionPosition = richTextBox.Selection.Start;
            Paragraph paragraph = insertionPosition.Paragraph;
            
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(7, 1);

            LineSegment myLineSegment = new LineSegment();
            myLineSegment.Point = new Point(800, 1);

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myLineSegment);

            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
           // myPath.Height = 1;
            myPath.Data = myPathGeometry;


            //<Path  Height="10" Fill="Black" Stretch="Fill" Stroke="Black" StrokeThickness="1" Visibility="Visible" Data="M7,34 L390,34" />
            paragraph.Inlines.Add(myPath);
        }

        public static void OnInsertPicture(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            //###
            TextPointer textPosition = richTextBox.Selection.Start;

            if (null != Helper.GetImageAncestor(textPosition))
            {
                PictureCommands.OnEditPictureProperties(sender, e);
                return;
            }
            //###

            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.Text = String.Empty;
            }

            TextPointer insertionPosition = richTextBox.Selection.Start;


            ImagePropertiesDialog imageProperties = new ImagePropertiesDialog(null);
            imageProperties.ShowDialog();

            if (true == imageProperties.DialogResult)
            {
                try
                {
                    Paragraph paragraph = insertionPosition.Paragraph;

                    // Split current paragraph at insertion position
                    insertionPosition = insertionPosition.InsertParagraphBreak();
                    paragraph = insertionPosition.Paragraph;
                    //paragraph.Inlines.Add("Some ");

                    //AcmBitmapImageHolder acmBitmapImageHolder = new AcmBitmapImageHolder(new Uri(@"http://www.cetelem.cz/images/cetelem2/cetelem_logo_cl_small.gif"));
                    AcmBitmapImageHolder acmBitmapImageHolder = new AcmBitmapImageHolder(new Uri(imageProperties.ImagePath));

                    //acmBitmapImageHolder.Image.Style= sender.
                    paragraph.Inlines.Add(acmBitmapImageHolder.Image);

                    if (!String.IsNullOrEmpty(imageProperties.ImageHyperlink))
                    {
                        //### Set hyperlink
                        acmBitmapImageHolder.Image.Tag = imageProperties.ImageHyperlink;
                        Hyperlink hyperlink = new Hyperlink(paragraph.Inlines.LastInline, insertionPosition);
                        hyperlink.NavigateUri = new Uri(imageProperties.ImageHyperlink);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} ({1}) :\n\r{2}",Res.txtLoadPictureFailed,imageProperties.ImageHyperlink,ex), Res.txtEditorName);
                }
            }
        }

        /// <summary>
        /// Handler for editing of the picture
        /// </summary>
        public static void OnEditPictureProperties(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;

            Image image = Helper.GetImageAncestor(insertionPosition);
            InlineUIContainer inlineUIContainer = Helper.GetInlineUIContainer(insertionPosition);
            if (image != null)
            {
                ImagePropertiesDialog imageProperties = new ImagePropertiesDialog(image);
                imageProperties.ShowDialog();
                if (true == imageProperties.DialogResult)
                {
                    try
                    {
                        if (imageProperties.PictureSourceChanged)
                        {
                            AcmBitmapImageHolder acmBitmapImageHolder = new AcmBitmapImageHolder(image,new Uri(imageProperties.ImagePath));
                        }
                        if (!String.IsNullOrEmpty(imageProperties.ImageHyperlink))
                        {
                            //### Set hyperlink
                            image.Tag = imageProperties.ImageHyperlink;
                            Hyperlink hyperlink = new Hyperlink(inlineUIContainer,insertionPosition);
                            hyperlink.NavigateUri = new Uri(imageProperties.ImageHyperlink);
                        }
                        control.OnRichTextBox_TextChanged(control, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0} ({1}) :\n\r{2}",Res.txtLoadPictureFailed,imageProperties.ImageHyperlink,ex), Res.txtEditorName);
                    }
                }
            }
        }
        #endregion //| Private methods |
    }
}