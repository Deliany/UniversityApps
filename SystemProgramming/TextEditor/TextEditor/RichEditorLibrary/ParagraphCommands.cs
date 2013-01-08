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

namespace Adastra.RichEditorLibrary
{
    /// <summary>
    /// This file contains code for style commands exposed by the edit control.
    /// </summary>
    internal class ParagraphCommands
    {
        static ParagraphCommands()
        {
            // Paragraph style commands
            // ------------------------
            _applyNormalStyleCommand = new RoutedCommand("ApplyNormalStyle", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), new CommandBinding(_applyNormalStyleCommand, OnApplyNormalStyle, OnCanExecuteTrue));

            _applyHeading1StyleCommand = new RoutedCommand("ApplyHeading1Style", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), new CommandBinding(_applyHeading1StyleCommand, OnApplyHeading1Style, OnCanExecuteTrue));

            _applyHeading2StyleCommand = new RoutedCommand("ApplyHeading2Style", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), new CommandBinding(_applyHeading2StyleCommand, OnApplyHeading2Style, OnCanExecuteTrue));

            _applyHeading3StyleCommand = new RoutedCommand("ApplyHeading3Style", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), new CommandBinding(_applyHeading3StyleCommand, OnApplyHeading3Style, OnCanExecuteTrue));

            //CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
            //    new CommandBinding(EditingCommands.EnterParagraphBreak, OnEnterParagraphBreak, OnCanExecuteTrue));
        }

        public static RoutedCommand ApplyNormalStyleCommand
        {
            get
            {
                return _applyNormalStyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading1StyleCommand
        {
            get
            {
                return _applyHeading1StyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading2StyleCommand
        {
            get
            {
                return _applyHeading2StyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading3StyleCommand
        {
            get
            {
                return _applyHeading3StyleCommand;
            }
        }

        #region Private Methods

        private static void OnCanExecuteTrue(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void OnApplyNormalStyle(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            if (paragraph != null)
            {
                paragraph.FontFamily = new FontFamily("Verdana");
                paragraph.FontSize = 11;
            }
        }

        private static void OnApplyHeading1Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            if (paragraph != null)
            {
                paragraph.FontFamily = new FontFamily("Arial");
                paragraph.FontSize = 16;
                paragraph.FontWeight = FontWeights.Bold;
            }
        }

        private static void OnApplyHeading2Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            if (paragraph != null)
            {
                paragraph.FontFamily = new FontFamily("Arial");
                paragraph.FontSize = 14;
                paragraph.FontWeight = FontWeights.Bold;
                paragraph.FontStyle = FontStyles.Italic;
            }
        }

        private static void OnApplyHeading3Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            if (paragraph != null)
            {
                paragraph.FontFamily = new FontFamily("Arial");
                paragraph.FontSize = 13;
                paragraph.FontWeight = FontWeights.Bold;
            }
        }

        private static void OnEnterParagraphBreak(object sender, KeyEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)sender;
            RichTextBox richTextBox = richEditor.RichTextBox;

            if (richTextBox.Selection.Start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text &&
                richTextBox.Selection.Start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
            {
                return; // Default handling of enter break will split parent Paragraph...
            }

            if (richTextBox.Selection.Start.Paragraph != null &&
                richTextBox.Selection.Start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
            {
                // Caret is at end of paragraph content...

                Paragraph paragraph = richTextBox.Selection.Start.Paragraph;
                FontFamily fontFamily = paragraph.FontFamily;
                double fontSize = paragraph.FontSize;
                FontStyle fontStyle = paragraph.FontStyle;
                FontWeight fontWeight = paragraph.FontWeight;

                FontFamily arialFont = new FontFamily("Arial");

                if ((fontFamily.Equals(arialFont) && fontWeight == FontWeights.Bold) &&
                    (fontSize == 16 || (fontSize == 14 && fontStyle == FontStyles.Italic) || fontSize == 13))
                {
                    Paragraph newParagraph = new Paragraph(new Run());
                    paragraph.SiblingBlocks.InsertAfter(paragraph, newParagraph);
                    richTextBox.CaretPosition = newParagraph.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
                    e.Handled = true;
                }
            }
        }
        #endregion

        //------------------------------------------------------
        //
        // Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static RoutedCommand _applyNormalStyleCommand;
        private static RoutedCommand _applyHeading1StyleCommand;
        private static RoutedCommand _applyHeading2StyleCommand;
        private static RoutedCommand _applyHeading3StyleCommand;

        #endregion
    }
}