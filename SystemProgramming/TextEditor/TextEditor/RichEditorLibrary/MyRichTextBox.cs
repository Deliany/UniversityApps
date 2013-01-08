using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RichEditorLibrary
{
    public class MyRichTextBox : RichTextBox
    {
        // Ctor.
        static MyRichTextBox()
        {
            EventManager.RegisterClassHandler(typeof(MyRichTextBox), KeyDownEvent, new KeyEventHandler(OnKeyDown), /*handledEventsToo*/true);
        }

        // Ctor.
        public MyRichTextBox()
        {
            DataObject.AddPastingHandler(this, this.DataObjectPastingEventHandler);
            this.TextChanged += this.TextChangedEventHandler;
        }

        /// <summary>
        /// Event handler for KeyDown event to auto-detect hyperlinks on space, enter and backspace keys. 
        /// </summary>
        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            MyRichTextBox myRichTextBox = (MyRichTextBox)sender;

            if (e.Key != Key.Back && e.Key != Key.Space && e.Key != Key.Return)
            {
                return;
            }

            if (!myRichTextBox.Selection.IsEmpty)
            {
                myRichTextBox.Selection.Text = String.Empty;
            }

            TextPointer caretPosition = myRichTextBox.Selection.Start;

            if (e.Key == Key.Space || e.Key == Key.Return)
            {
                TextRange wordRange = WordBreaker.GetWordRange(caretPosition);
                string wordText = wordRange.Text;

                if (wordText == "www.microsoft.com")
                {
                    // Insert hyperlink element at word boundaries.
                    new Hyperlink(wordRange.Start, wordRange.End);

                    // No need to update RichTextBox caret position, 
                    // since we only inserted a Hyperlink ElementEnd following current caretPosition.
                    // Subsequent handling of space input by base RichTextBox will update selection.
                }
            }
            else // Key.Back
            {
                TextPointer backspacePosition = caretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
                Hyperlink hyperlink;
                if (backspacePosition != null && HyperlinkHelper.IsHyperlinkBoundaryCrossed(caretPosition, backspacePosition, out hyperlink))
                {
                    // Remember caretPosition with forward gravity. This is necessary since we are going to delete 
                    // the hyperlink element preceeding caretPosition and after deletion current caretPosition 
                    // (with backward gravity) will follow content preceeding the hyperlink. 
                    // We want to remember content following the hyperlink to set new caret position at.

                    TextPointer newCaretPosition = caretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);

                    // Deleting the hyperlink is done using logic below.

                    // 1. Copy its children Inline to a temporary array.
                    InlineCollection hyperlinkChildren = hyperlink.Inlines;
                    Inline[] inlines = new Inline[hyperlinkChildren.Count];
                    hyperlinkChildren.CopyTo(inlines, 0);

                    // 2. Remove each child from parent hyperlink element and insert it after the hyperlink.
                    for (int i = inlines.Length - 1; i >= 0; i--)
                    {
                        hyperlinkChildren.Remove(inlines[i]);
                        hyperlink.SiblingInlines.InsertAfter(hyperlink, inlines[i]);
                    }

                    // 3. Apply hyperlink's local formatting properties to inlines (which are now outside hyperlink scope).
                    LocalValueEnumerator localProperties = hyperlink.GetLocalValueEnumerator();
                    TextRange inlineRange = new TextRange(inlines[0].ContentStart, inlines[inlines.Length - 1].ContentEnd);

                    while (localProperties.MoveNext())
                    {
                        LocalValueEntry property = localProperties.Current;
                        DependencyProperty dp = property.Property;
                        object value = property.Value;

                        if (!dp.ReadOnly &&
                            dp != Inline.TextDecorationsProperty && // Ignore hyperlink defaults.
                            dp != TextElement.ForegroundProperty &&
                            dp != BaseUriHelper.BaseUriProperty &&
                            !HyperlinkHelper.IsHyperlinkProperty(dp))
                        {
                            inlineRange.ApplyPropertyValue(dp, value);
                        }
                    }

                    // 4. Delete the (empty) hyperlink element.
                    hyperlink.SiblingInlines.Remove(hyperlink);

                    // 5. Update selection, since we deleted Hyperlink element and caretPosition was at that Hyperlink's end boundary.
                    myRichTextBox.Selection.Select(newCaretPosition, newCaretPosition);
                }
            }
        }

        /// <summary>
        /// Event handler for DataObject.Pasting event on this RichTextBox.
        /// </summary>
        private void DataObjectPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            this.pasteFlag = true;

            // We don't handle the event here. Let the base RTB handle the paste operation.
            // This will raise a TextChanged event, which we handle below to scan for any matching hyperlinks.
        }

        /// <summary>
        /// Event handler for RichTextBox.TextChanged event.
        /// </summary>
        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (!this.pasteFlag || this.Document == null)
            {
                return;
            }

            // Temporarily disable TextChanged event handler, since following code might insert Hyperlinks,
            // which will raise another TextChanged event.
            this.TextChanged -= this.TextChangedEventHandler;

            TextPointer navigator = this.Document.ContentStart;
            while (navigator != null && navigator.CompareTo(this.Document.ContentEnd) < 0)
            {
                TextRange wordRange = WordBreaker.GetWordRange(navigator);
                if (wordRange == null || wordRange.IsEmpty)
                {
                    // No more words in the document.
                    break;
                }

                string wordText = wordRange.Text;
                if (wordText == "www.microsoft.com" &&
                    !HyperlinkHelper.IsInHyperlinkScope(wordRange.Start) &&
                    !HyperlinkHelper.IsInHyperlinkScope(wordRange.End))
                {
                    Hyperlink hyperlink = new Hyperlink(wordRange.Start, wordRange.End);
                    navigator = hyperlink.ElementEnd.GetNextInsertionPosition(LogicalDirection.Forward);
                }
                else
                {
                    navigator = wordRange.End.GetNextInsertionPosition(LogicalDirection.Forward);
                }
            }

            this.TextChanged += this.TextChangedEventHandler;
            this.pasteFlag = false;
        }

        // Private flag which is true during a paste operation on this RichTextBox.
        private bool pasteFlag;
    }
}