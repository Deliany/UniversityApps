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
using System.Diagnostics;

namespace Adastra.RichEditorLibrary
{
    public class MiscCommands
    {
        #region Private Fields

        private static RoutedCommand m_propertiesCommand;
        private static RoutedCommand m_printCommand;
        private static RoutedCommand m_findCommand;
        private static RoutedCommand m_clearFormattingCommand;

        #endregion

        static MiscCommands()
        {
            // Misc commands
            // ----------------------------
            m_propertiesCommand = new RoutedCommand("Properties", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_propertiesCommand, OnProperties, OnCanExecuteProps));

            m_printCommand = new RoutedCommand("Print", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_printCommand, OnPrint, OnCanExecuteTrue));

            //m_findCommand = new RoutedCommand("Find", typeof(RichTextEditor));
            //CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
            //    new CommandBinding(m_findCommand, OnFind, OnCanExecuteTrue));

            m_clearFormattingCommand = new RoutedCommand("ClearFormatting", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor),
                new CommandBinding(m_clearFormattingCommand, OnClearFormatting, OnCanExecuteTrue));
        }

        public static RoutedCommand PrintCommand
        {
            get
            {
                return m_printCommand;
            }
        }

        public static RoutedCommand PropertiesCommand
        {
            get
            {
                return m_propertiesCommand;
            }
        }

        //public static RoutedCommand FindCommand
        //{
        //    get
        //    {
        //        return m_findCommand;
        //    }
        //}

        public static RoutedCommand ClearFormattingCommand
        {
            get
            {
                return m_clearFormattingCommand;
            }
        }

        #region Private Methods

        private static void OnCanExecuteTrue(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void OnCanExecuteProps(object target, CanExecuteRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)target;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer textPosition = richTextBox.Selection.Start;
            //TextRange txR = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
            //int iRes = richTextBox.Selection.Start.CompareTo(richTextBox.Selection.End);
            //bool bIsSelectedText = !String.IsNullOrEmpty((new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End)).Text);

            if ((null != Helper.GetImageAncestor(textPosition) || null != Helper.GetTableAncestor(textPosition)))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public static void OnProperties(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer textPosition = richTextBox.Selection.Start;

            if (null != Helper.GetImageAncestor(textPosition))
            {
                PictureCommands.EditPictureCommand.Execute(sender, null);
            }
            else if(null != Helper.GetTableAncestor(textPosition))
            {
                TableCommands.EditTablePropertiesCommand.Execute(sender, null);
                
            }
        }

        public static bool IsImageSelected(RichTextBox richTextBox)
        {
            TextPointer textPosition = richTextBox.Selection.Start;

            if (null != Helper.GetImageAncestor(textPosition))
            {
                return true;
            }
            return false;
        }

        public static bool IsTableSelected(RichTextBox richTextBox)
        {
            TextPointer textPosition = richTextBox.Selection.Start;

            if(null != Helper.GetTableAncestor(textPosition))
            {
                return true;
            }
            return false;
        }

        public static void InstallTableCallbacks(Table table)
        {
            try
            {
                if (null != table)
                {
                    //MouseEventHandler mouseEventEnter = new MouseEventHandler(Helper.table_MouseEnter);
                    //MouseEventHandler mouseEventLeave = new MouseEventHandler(Helper.table_MouseLeave);
                    //table.MouseEnter -= mouseEventEnter;
                    //table.MouseLeave -= mouseEventLeave;

                    //table.MouseEnter += mouseEventEnter;
                    //table.MouseLeave += mouseEventLeave;

                    table.MouseEnter += new MouseEventHandler(Helper.table_MouseEnter);
                    table.MouseLeave += new MouseEventHandler(Helper.table_MouseLeave);

                    foreach (TableRowGroup rowGroup in table.RowGroups)
                    {
                        foreach (TableRow row in rowGroup.Rows)
                        {
                            foreach (TableCell cell in row.Cells)
                            {
                                cell.MouseLeftButtonDown += new MouseButtonEventHandler(Helper.cell_MouseLeftButtonDown);
                                cell.MouseLeftButtonUp += new MouseButtonEventHandler(Helper.cell_MouseLeftButtonUp);
                                cell.MouseMove += new MouseEventHandler(Helper.cell_MouseMove);
                                cell.MouseEnter += new MouseEventHandler(Helper.cell_MouseEnter);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("### Exception: " + ex);
            }
        }
        
         
        private static void OnPrint(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            // Serialize RichTextBox content into a stream in Xaml format. Note: XamlPackage format isn't supported in partial trust.
            TextRange sourceDocument = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            sourceDocument.Save(stream, DataFormats.Xaml);

            // Clone the source document's content into a new FlowDocument.
            FlowDocument flowDocumentCopy = new FlowDocument();
            TextRange copyDocumentRange = new TextRange(flowDocumentCopy.ContentStart, flowDocumentCopy.ContentEnd);
            copyDocumentRange.Load(stream, DataFormats.Xaml);

            // Creates a XpsDocumentWriter object, opens a Windows common print dialog and 
            // returns a ref parameter that represents information about the dimensions of the media. 
            PrintDocumentImageableArea ia = null;
            XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocumentCopy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness pagePadding = flowDocumentCopy.PagePadding;
                flowDocumentCopy.PagePadding = new Thickness(
                        Math.Max(ia.OriginWidth, pagePadding.Left),
                        Math.Max(ia.OriginHeight, pagePadding.Top),
                        Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), pagePadding.Right),
                        Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), pagePadding.Bottom));
                flowDocumentCopy.ColumnWidth = double.PositiveInfinity;

                // Send DocumentPaginator to the printer.
                docWriter.Write(paginator);
            }
        }

        private static void OnClearFormatting(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            richTextBox.Selection.ClearAllProperties();
        }

        #endregion

    }
}