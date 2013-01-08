using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace RichEditorLibrary
{
    internal static class HyperlinkHelper
    {
        // Helper that returns true when passed property applies to Hyperlink only.
        public static bool IsHyperlinkProperty(DependencyProperty dp)
        {
            return dp == Hyperlink.CommandProperty ||
                dp == Hyperlink.CommandParameterProperty ||
                dp == Hyperlink.CommandTargetProperty ||
                dp == Hyperlink.NavigateUriProperty ||
                dp == Hyperlink.TargetNameProperty;
        }

        // Helper that returns true if passed caretPosition and backspacePosition cross a hyperlink end boundary
        // (under the assumption that caretPosition and backSpacePosition are adjacent insertion positions).
        public static bool IsHyperlinkBoundaryCrossed(TextPointer caretPosition, TextPointer backspacePosition, out Hyperlink backspacePositionHyperlink)
        {
            Hyperlink caretPositionHyperlink = GetHyperlinkAncestor(caretPosition);
            backspacePositionHyperlink = GetHyperlinkAncestor(backspacePosition);

            return (caretPositionHyperlink == null && backspacePositionHyperlink != null) ||
                (caretPositionHyperlink != null && backspacePositionHyperlink != null && caretPositionHyperlink != backspacePositionHyperlink);
        }

        // Helper that returns a Hyperlink ancestor of passed TextPointer.
        private static Hyperlink GetHyperlinkAncestor(TextPointer position)
        {
            Inline parent = position.Parent as Inline;
            while (parent != null && !(parent is Hyperlink))
            {
                parent = parent.Parent as Inline;
            }

            return parent as Hyperlink;
        }

        // Helper that returns true when passed TextPointer is within the scope of a Hyperlink element.
        public static bool IsInHyperlinkScope(TextPointer position)
        {
            return GetHyperlinkAncestor(position) != null;
        }
    }
}
