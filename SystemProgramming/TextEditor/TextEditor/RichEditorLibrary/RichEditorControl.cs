using System;
using System.ComponentModel;
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

namespace RichEditorLibrary
{
    public class RichEditorControl : Control
    {
        // -----------------------------------------------------------
        //
        // Constructors
        //
        // -----------------------------------------------------------

        #region Constructors

        static RichEditorControl()
        {
            InitializeCommands();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichEditorControl), new FrameworkPropertyMetadata(typeof(RichEditorControl)));
        }

        #endregion

        // -----------------------------------------------------------
        //
        // Public Properties
        //
        // -----------------------------------------------------------

        #region Public Properties

        public RichTextBox RichTextBox
        {
            get
            {
                return (RichTextBox)GetTemplateChild("RTB");
            }
        }

        public ComboBox FindComboBox
        {
            get
            {
                return (ComboBox)GetTemplateChild("FindComboBox");
            }
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

        public static RoutedCommand FindCommand
        {
            get
            {
                return _findCommand;
            }
        }

        #endregion

        //------------------------------------------------------
        //
        // Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Attach background handler to update edit control's UI when RichTextBox's selection changes.
            this.RichTextBox.Selection.Changed += new EventHandler(OnSelectionChanged);

            // Enforce the first update.
            this.OnSelectionChanged(null, null);
        }

        #endregion

        //------------------------------------------------------
        //
        // Selection binding properties
        //
        //------------------------------------------------------

        #region Selection Binding Properties

        // Handler for selection change notification - schedules a job to update editor UI controls.
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!_updateSelectionPropertiesPending)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(UpdateSelectionProperties),
                    null);
                _updateSelectionPropertiesPending = true;
            }
        }

        // Worker that updates editor UI when selection properties change.
        private object UpdateSelectionProperties(object arg)
        {
            object value;

            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            this.SelectionFontFamily = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            this.SelectionFontSize = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            this.SelectionIsBold = (value == DependencyProperty.UnsetValue) ? false : ((FontWeight)value == FontWeights.Bold);

            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            this.SelectionIsItalic = (value == DependencyProperty.UnsetValue) ? false : ((FontStyle)value == FontStyles.Italic);

            value = this.RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            this.SelectionIsUnderline = (value == DependencyProperty.UnsetValue) ? false : value != null && System.Windows.TextDecorations.Underline.Equals(value);

            value = this.RichTextBox.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
            this.SelectionIsAlignLeft = (value == DependencyProperty.UnsetValue) ? true : (TextAlignment)value == TextAlignment.Left;
            this.SelectionIsAlignCenter = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Center;
            this.SelectionIsAlignRight = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Right;
            this.SelectionIsAlignJustify = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Justify;

            FlowDirection flowDirection = this.RichTextBox.Selection.Start.Paragraph.FlowDirection;
            this.SelectionParagraphIsLeftToRight = flowDirection == FlowDirection.LeftToRight;
            this.SelectionParagraphIsRightToLeft = flowDirection == FlowDirection.RightToLeft;

            // Bullets and Numbering
            Paragraph startParagraph = this.RichTextBox.Selection.Start.Paragraph;
            Paragraph endParagraph = this.RichTextBox.Selection.End.Paragraph;
            if (startParagraph != null && endParagraph != null &&
                (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) &&
                ((ListItem)startParagraph.Parent).List == ((ListItem)endParagraph.Parent).List)
            {
                TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
                this.SelectionIsBullets = (
                    markerStyle == TextMarkerStyle.Disc ||
                    markerStyle == TextMarkerStyle.Circle ||
                    markerStyle == TextMarkerStyle.Square ||
                    markerStyle == TextMarkerStyle.Box);
                this.SelectionIsNumbering = (
                    markerStyle == TextMarkerStyle.LowerRoman ||
                    markerStyle == TextMarkerStyle.UpperRoman ||
                    markerStyle == TextMarkerStyle.LowerLatin ||
                    markerStyle == TextMarkerStyle.UpperLatin ||
                    markerStyle == TextMarkerStyle.Decimal);
            }

            _updateSelectionPropertiesPending = false;
            return null;
        }

        // IsSpellCheckEnabled
        // -------------------
        private static readonly DependencyProperty IsSpellCheckEnabledProperty = DependencyProperty.Register(
            "IsSpellCheckEnabled", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnIsSpellCheckEnabledPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        private bool IsSpellCheckEnabled
        {
            get { return (bool)GetValue(IsSpellCheckEnabledProperty); }
            set { SetValue(IsSpellCheckEnabledProperty, value); }
        }

        private static void OnIsSpellCheckEnabledPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)o;
            if (richEditorControl._updateSelectionPropertiesPending)
            {
                return;
            }
            bool value = (bool)e.NewValue;
            if (value != null)
            {
                richEditorControl.RichTextBox.SpellCheck.IsEnabled = value;
            }
        }

        // SelectionFontFamily
        // -------------------
        private static readonly DependencyProperty SelectionFontFamilyProperty = DependencyProperty.Register(
            "SelectionFontFamily", typeof(string), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionFontFamilyPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        private string SelectionFontFamily 
        { 
            get { return (string)GetValue(SelectionFontFamilyProperty); } 
            set { SetValue(SelectionFontFamilyProperty, value); } 
        }

        private static void OnSelectionFontFamilyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)o;
            if (richEditorControl._updateSelectionPropertiesPending)
            {
                return;
            }
            string value = (string)e.NewValue;
            if (value != null)
            {
                richEditorControl.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value);
                richEditorControl.OnSelectionChanged(null, null);
            }
        }

        // SelectionFontSize
        // -----------------
        private static readonly DependencyProperty SelectionFontSizeProperty = DependencyProperty.Register(
            "SelectionFontSize", typeof(string), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionFontSizePropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        private string SelectionFontSize 
        { 
            get { return (string)GetValue(SelectionFontSizeProperty); } 
            set { SetValue(SelectionFontSizeProperty, value); } 
        }

        private static void OnSelectionFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            string value = (string)e.NewValue;
            if (value != null)
            {
                richEditorControl.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value);
                richEditorControl.OnSelectionChanged(null, null);
            }
        }

        // SelectionIsBold
        // ---------------
        public static readonly DependencyProperty SelectionIsBoldProperty = DependencyProperty.Register(
            "SelectionIsBold", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsBoldPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsBold 
        { 
            get { return (bool)GetValue(SelectionIsBoldProperty); } 
            set { SetValue(SelectionIsBoldProperty, value); } 
        }

        private static void OnSelectionIsBoldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, (value == true) ? FontWeights.Bold : FontWeights.Normal);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsItalic
        // -----------------
        public static readonly DependencyProperty SelectionIsItalicProperty = DependencyProperty.Register(
            "SelectionIsItalic", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsItalicPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsItalic 
        { 
            get { return (bool)GetValue(SelectionIsItalicProperty); } 
            set { SetValue(SelectionIsItalicProperty, value); } 
        }

        private static void OnSelectionIsItalicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, (value == true) ? FontStyles.Italic : FontStyles.Normal);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsUnderline
        // --------------------
        public static readonly DependencyProperty SelectionIsUnderlineProperty = DependencyProperty.Register(
            "SelectionIsUnderline", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsUnderlinePropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsUnderline 
        { 
            get { return (bool)GetValue(SelectionIsUnderlineProperty); } 
            set { SetValue(SelectionIsUnderlineProperty, value); } 
        }

        private static void OnSelectionIsUnderlinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, (value == true) ? System.Windows.TextDecorations.Underline : null);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignLeft
        // --------------------
        public static readonly DependencyProperty SelectionIsAlignLeftProperty = DependencyProperty.Register(
            "SelectionIsAlignLeft", typeof(bool?), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsAlignLeftPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignLeft 
        { 
            get { return (bool)GetValue(SelectionIsAlignLeftProperty); } 
            set { SetValue(SelectionIsAlignLeftProperty, value); } 
        }

        private static void OnSelectionIsAlignLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Left : TextAlignment.Left);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignCenter
        // ----------------------
        public static readonly DependencyProperty SelectionIsAlignCenterProperty = DependencyProperty.Register(
            "SelectionIsAlignCenter", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsAlignCenterPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignCenter 
        { 
            get { return (bool)GetValue(SelectionIsAlignCenterProperty); } 
            set { SetValue(SelectionIsAlignCenterProperty, value); } 
        }

        private static void OnSelectionIsAlignCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Center : TextAlignment.Left);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignRight
        // ---------------------
        public static readonly DependencyProperty SelectionIsAlignRightProperty = DependencyProperty.Register(
            "SelectionIsAlignRight", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsAlignRightPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignRight 
        { 
            get { return (bool)GetValue(SelectionIsAlignRightProperty); } 
            set { SetValue(SelectionIsAlignRightProperty, value); } 
        }

        private static void OnSelectionIsAlignRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Right : TextAlignment.Left);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignJustify
        // -----------------------
        public static readonly DependencyProperty SelectionIsAlignJustifyProperty = DependencyProperty.Register(
            "SelectionIsAlignJustify", typeof(bool?), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsAlignJustifyPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignJustify 
        { 
            get { return (bool)GetValue(SelectionIsAlignJustifyProperty); } 
            set { SetValue(SelectionIsAlignJustifyProperty, value); } 
        }

        private static void OnSelectionIsAlignJustifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Justify : TextAlignment.Left);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionParagraphIsLeftToRight
        // -------------------------------
        public static readonly DependencyProperty SelectionParagraphIsLeftToRightProperty = DependencyProperty.Register(
            "SelectionParagraphIsLeftToRight", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionParagraphIsLeftToRightPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionParagraphIsLeftToRight
        {
            get { return (bool)GetValue(SelectionParagraphIsLeftToRightProperty); }
            set { SetValue(SelectionParagraphIsLeftToRightProperty, value); }
        }

        private static void OnSelectionParagraphIsLeftToRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.LeftToRight : FlowDirection.RightToLeft);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionParagraphIsRightToLeft
        // -------------------------------
        public static readonly DependencyProperty SelectionParagraphIsRightToLeftProperty = DependencyProperty.Register(
            "SelectionParagraphIsRightToLeft", typeof(bool), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionParagraphIsRightToLeftPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionParagraphIsRightToLeft
        {
            get { return (bool)GetValue(SelectionParagraphIsRightToLeftProperty); }
            set { SetValue(SelectionParagraphIsRightToLeftProperty, value); }
        }

        private static void OnSelectionParagraphIsRightToLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditorControl.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsBullets
        // ------------------
        public static readonly DependencyProperty SelectionIsBulletsProperty = DependencyProperty.Register(
            "SelectionIsBullets", typeof(bool?), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsBulletsPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsBullets 
        { 
            get { return (bool)GetValue(SelectionIsBulletsProperty); } 
            set { SetValue(SelectionIsBulletsProperty, value); } 
        }

        private static void OnSelectionIsBulletsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleBullets.Execute(null, richEditorControl.RichTextBox);
            richEditorControl.OnSelectionChanged(null, null);
        }

        // SelectionIsNumbering
        // --------------------
        public static readonly DependencyProperty SelectionIsNumberingProperty = DependencyProperty.Register(
            "SelectionIsNumbering", typeof(bool?), typeof(RichEditorControl),
            new FrameworkPropertyMetadata(
                (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSelectionIsNumberingPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsNumbering 
        { 
            get { return (bool)GetValue(SelectionIsNumberingProperty); } 
            set { SetValue(SelectionIsNumberingProperty, value); } 
        }

        private static void OnSelectionIsNumberingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichEditorControl richEditorControl = (RichEditorControl)d;
            if (richEditorControl._updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleNumbering.Execute(null, richEditorControl.RichTextBox);
            richEditorControl.OnSelectionChanged(null, null);
        }

        #endregion

        //------------------------------------------------------
        //
        // Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private static void InitializeCommands()
        {
            _applyNormalStyleCommand = new RoutedCommand("ApplyNormalStyle", typeof(RichEditorControl));
            CommandManager.RegisterClassCommandBinding(typeof(RichEditorControl), new CommandBinding(_applyNormalStyleCommand, OnApplyNormalStyle, OnCanExecuteTrue));

            _applyHeading1StyleCommand = new RoutedCommand("ApplyHeading1Style", typeof(RichEditorControl));
            CommandManager.RegisterClassCommandBinding(typeof(RichEditorControl), new CommandBinding(_applyHeading1StyleCommand, OnApplyHeading1Style, OnCanExecuteTrue));

            _applyHeading2StyleCommand = new RoutedCommand("ApplyHeading2Style", typeof(RichEditorControl));
            CommandManager.RegisterClassCommandBinding(typeof(RichEditorControl), new CommandBinding(_applyHeading2StyleCommand, OnApplyHeading2Style, OnCanExecuteTrue));

            _applyHeading3StyleCommand = new RoutedCommand("ApplyHeading3Style", typeof(RichEditorControl));
            CommandManager.RegisterClassCommandBinding(typeof(RichEditorControl), new CommandBinding(_applyHeading3StyleCommand, OnApplyHeading3Style, OnCanExecuteTrue));

            _findCommand = new RoutedCommand("Find", typeof(RichEditorControl));
            CommandManager.RegisterClassCommandBinding(typeof(RichEditorControl), new CommandBinding(_findCommand, OnFind, OnCanExecuteTrue));
        }

        private static void OnCanExecuteTrue(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void OnApplyNormalStyle(object sender, ExecutedRoutedEventArgs e)
        {
            RichEditorControl control = (RichEditorControl)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            paragraph.FontFamily = new FontFamily("Verdana");
            paragraph.FontSize = 11;
        }

        private static void OnApplyHeading1Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichEditorControl control = (RichEditorControl)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            paragraph.FontFamily = new FontFamily("Arial");
            paragraph.FontSize = 16;
            paragraph.FontWeight = FontWeights.Bold;
        }

        private static void OnApplyHeading2Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichEditorControl control = (RichEditorControl)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            paragraph.FontFamily = new FontFamily("Arial");
            paragraph.FontSize = 14;
            paragraph.FontWeight = FontWeights.Bold;
            paragraph.FontStyle = FontStyles.Italic;
        }

        private static void OnApplyHeading3Style(object sender, ExecutedRoutedEventArgs e)
        {
            RichEditorControl control = (RichEditorControl)sender;
            Paragraph paragraph = control.RichTextBox.Selection.Start.Paragraph;
            paragraph.FontFamily = new FontFamily("Arial");
            paragraph.FontSize = 13;
            paragraph.FontWeight = FontWeights.Bold;
        }

        private static void OnFind(object sender, ExecutedRoutedEventArgs e)
        {
            RichEditorControl control = (RichEditorControl)sender;
            string findText = (string)control.FindComboBox.Text;

            TextPointer navigator = control.RichTextBox.Selection.IsEmpty ?
                control.RichTextBox.Document.ContentStart :
                control.RichTextBox.Selection.End.GetNextInsertionPosition(LogicalDirection.Forward);

            while (navigator != null && navigator.CompareTo(control.RichTextBox.Document.ContentEnd) < 0)
            {
                TextRange wordRange = WordBreaker.GetWordRange(navigator);

                if (wordRange == null)
                {
                    break;
                }

                string wordText = wordRange.Text;
                if (wordText == findText)
                {
                    control.RichTextBox.Selection.Select(wordRange.Start, wordRange.End);
                    return;
                }

                navigator = wordRange.End.GetNextInsertionPosition(LogicalDirection.Forward);
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
        private static RoutedCommand _findCommand;
        private bool _updateSelectionPropertiesPending;

        #endregion
    }
}