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
    /// <summary>
    /// TODO
    /// </summary>
    public partial class RichTextEditor : UserControl
    {
        #region Private Fields

        private bool            m_updateSelectionPropertiesPending;

        #endregion

        // -----------------------------------------------------------
        //
        // Constructors
        //
        // -----------------------------------------------------------

        #region Constructors

        static RichTextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextEditor), new FrameworkPropertyMetadata(typeof(RichTextEditor)));
        }

        public RichTextEditor()
        {
            this.InitializeComponent();
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
            this.RichTextBox.MouseDoubleClick += new MouseButtonEventHandler(RichTextBox_MouseDoubleClick);
            this.RichTextBox.ContextMenuOpening += new ContextMenuEventHandler(RichTextBox_ContextMenuOpening);

            this.RichTextBox.Loaded += new RoutedEventHandler(RichTextBox_Loaded);

            // Enforce the first update.
            this.OnSelectionChanged(null, null);

            //RichTextBox.IsVisibleChanged += new DependencyPropertyChangedEventHandler(RichTextBox_IsVisibleChanged);

        }

        void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void RichTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        }

        void RichTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MiscCommands.PropertiesCommand.Execute(sender, null);
        }

        //public void RichTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (RichTextBox.Visibility == Visibility.Collapsed)
        //    {
        //        ((TextBox)GetTemplateChild("TB")).Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //    }

        //}

        #endregion

        // -----------------------------------------------------------
        //
        // Public Properties
        //
        // -----------------------------------------------------------

        #region Public Properties

        // Template elements
        // -----------------
        
        public RichTextBox RichTextBox
        {
            get
            {
                return m_RTB;
            }
        }

        //public Menu MainMenu
        //{
        //    get
        //    {
        //        return (Menu)GetTemplateChild("FirstMenu");
        //    }
        //}
        public void RibbonCommand_Executed_7(object sender, ExecutedRoutedEventArgs e)
        {

        }

        //public ToolBarTray MainToolBar
        //{
        //    get
        //    {
        //        return (ToolBarTray)GetTemplateChild("FirstToolBar");
        //    }
        //}

        //public MenuItem MainTableMenu
        //{
        //    get
        //    {
        //        return (MenuItem)GetTemplateChild("TableMenu");
        //    }
        //}


        //public TextBlock StatusBarMessage
        //{
        //    get
        //    {
        //        return (TextBlock)GetTemplateChild("StatusBarMessage");
        //    }
        //}

        //public ComboBox FindComboBox
        //{
        //    get
        //    {
        //        return (ComboBox)GetTemplateChild("FindComboBox");
        //    }
        //}

        // Paragraph commands
        // ------------------
        public static RoutedCommand ApplyNormalStyleCommand
        {
            get
            {
                return ParagraphCommands.ApplyNormalStyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading1StyleCommand
        {
            get
            {
                return ParagraphCommands.ApplyHeading1StyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading2StyleCommand
        {
            get
            {
                return ParagraphCommands.ApplyHeading2StyleCommand;
            }
        }

        public static RoutedCommand ApplyHeading3StyleCommand
        {
            get
            {
                return ParagraphCommands.ApplyHeading3StyleCommand;
            }
        }

        public static RoutedCommand InsertPictureCommand
        {
            get
            {
                return PictureCommands.InsertPictureCommand;
            }
        }

        public static RoutedCommand InsertHyperlinkCommand
        {
            get
            {
                return PictureCommands.InsertHyperlinkCommand;
            }
        }

        // Table commands
        // --------------
        public static RoutedCommand InsertTableCommand
        {
            get
            {
                return TableCommands.InsertTableCommand;
            }
        }

        public static RoutedCommand InsertRowsAboveCommand
        {
            get
            {
                return TableCommands.InsertRowsAboveCommand;
            }
        }

        public static RoutedCommand InsertRowsBelowCommand
        {
            get
            {
                return TableCommands.InsertRowsBelowCommand;
            }
        }

        public static RoutedCommand InsertColumnsToRightCommand
        {
            get
            {
                return TableCommands.InsertColumnsToRightCommand;
            }
        }

        public static RoutedCommand InsertColumnsToLeftCommand
        {
            get
            {
                return TableCommands.InsertColumnsToLeftCommand;
            }
        }

        public static RoutedCommand DeleteTableCommand
        {
            get
            {
                return TableCommands.DeleteTableCommand;
            }
        }

        public static RoutedCommand DeleteRowsCommand
        {
            get
            {
                return TableCommands.DeleteRowsCommand;
            }
        }

        public static RoutedCommand DeleteColumnsCommand
        {
            get
            {
                return TableCommands.DeleteColumnsCommand;
            }
        }

        // Misc commands
        // -------------
        public static RoutedCommand PrintCommand
        {
            get
            {
                return MiscCommands.PrintCommand;
            }
        }

        public static RoutedCommand PropertiesCommand
        {
            get
            {
                return MiscCommands.PropertiesCommand;
            }
        }

        public static RoutedCommand FindCommand
        {
            get
            {
                return MiscCommands.FindCommand;
            }
        }

        public static RoutedCommand ClearFormattingCommand
        {
            get
            {
                return MiscCommands.ClearFormattingCommand;
            }
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
            if (!m_updateSelectionPropertiesPending)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                         new DispatcherOperationCallback(UpdateSelectionProperties),
                                                         null);
                m_updateSelectionPropertiesPending = true;
            }
        }

        // Worker that updates editor UI when selection properties change.
        private object UpdateSelectionProperties(object arg)
        {
            object value;

            // FontFamily
            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            this.SelectionFontFamily = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

            // FontSize
            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            this.SelectionFontSize = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

            // FontWeight
            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            this.SelectionIsBold = (value == DependencyProperty.UnsetValue) ? false : ((FontWeight)value == FontWeights.Bold);

            // FontStyle
            value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            this.SelectionIsItalic = (value == DependencyProperty.UnsetValue) ? false : ((FontStyle)value == FontStyles.Italic);

            // Underline
            value = this.RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            this.SelectionIsUnderline = (value == DependencyProperty.UnsetValue) ? false : value != null && System.Windows.TextDecorations.Underline.Equals(value);

            // TextAlignment
            value = this.RichTextBox.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
            this.SelectionIsAlignLeft = (value == DependencyProperty.UnsetValue) ? true : (TextAlignment)value == TextAlignment.Left;
            this.SelectionIsAlignCenter = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Center;
            this.SelectionIsAlignRight = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Right;
            this.SelectionIsAlignJustify = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Justify;

            // FlowDirection
            if (this.RichTextBox.Selection.Start.Paragraph != null)
            {
                FlowDirection flowDirection = this.RichTextBox.Selection.Start.Paragraph.FlowDirection;
                this.SelectionParagraphIsLeftToRight = flowDirection == FlowDirection.LeftToRight;
                this.SelectionParagraphIsRightToLeft = flowDirection == FlowDirection.RightToLeft;
            }

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

            // Update status bar line info
            this.StatusBarLineInfo.Text =   "At Line :" + Helper.GetLineNumberFromSelection(this.RichTextBox.Selection.Start) + 
                                            " Column:" + Helper.GetColumnNumberFromSelection(this.RichTextBox.Selection.Start);

            m_updateSelectionPropertiesPending = false;

            //### Adorner
            this.SetAdorner();

            return null;
        }
        private ResizingAdorner m_resizingAdorner = null;

        private void SetAdorner()
        {
            TextPointer textPosition = this.RichTextBox.Selection.Start;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.RichTextBox);
            if (null != adornerLayer && null != this.m_resizingAdorner)
            {
                adornerLayer.Remove(this.m_resizingAdorner);
                this.m_resizingAdorner.Visibility = Visibility.Hidden;
                this.m_resizingAdorner = null;
            }
            UIElement uiElement = null;
            InlineUIContainer inlineUIContainer = Helper.GetInlineUIContainer(textPosition);

            if (null != inlineUIContainer && null != inlineUIContainer.Child)
            {
                uiElement = inlineUIContainer.Child;
            }
            //else
            //{
            //    Table table = Helper.GetTableAncestor(textPosition);
            //    uiElement = ((FlowDocument)table.Parent);
            //}

            if (null != uiElement)
            {
                this.m_resizingAdorner = new ResizingAdorner(uiElement);
                adornerLayer.Add(this.m_resizingAdorner);
            }
        }

        // IsSpellCheckEnabled
        // -------------------
        private static readonly DependencyProperty IsSpellCheckEnabledProperty = DependencyProperty.Register(
            "IsSpellCheckEnabled", typeof(bool), typeof(RichTextEditor),
            new FrameworkPropertyMetadata(
                (bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnIsSpellCheckEnabledPropertyChanged), null));

        [Bindable(true), Browsable(false), Category("Appearance")]
        private bool IsSpellCheckEnabled
        {
            get { return (bool)GetValue(IsSpellCheckEnabledProperty); }
            set { SetValue(IsSpellCheckEnabledProperty, value); }
        }

        private static void OnIsSpellCheckEnabledPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)o;
            bool value = (bool)e.NewValue;
            richEditor.RichTextBox.SpellCheck.IsEnabled = value;
        }

        // SelectionFontFamily
        // -------------------
        private static readonly DependencyProperty SelectionFontFamilyProperty = DependencyProperty.Register(
            "SelectionFontFamily", typeof(string), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)o;
            if (richEditor.m_updateSelectionPropertiesPending)
            {
                return;
            }
            string value = (string)e.NewValue;
            if (value != null)
            {
                richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value);
                richEditor.OnSelectionChanged(null, null);
            }
        }

        // SelectionFontSize
        // -----------------
        private static readonly DependencyProperty SelectionFontSizeProperty = DependencyProperty.Register(
            "SelectionFontSize", typeof(string), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            string value = (string)e.NewValue;
            if (value != null)
            {
                richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value);
                richEditor.OnSelectionChanged(null, null);
            }
        }

        // SelectionIsBold
        // ---------------
        public static readonly DependencyProperty SelectionIsBoldProperty = DependencyProperty.Register(
            "SelectionIsBold", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, (value == true) ? FontWeights.Bold : FontWeights.Normal);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsItalic
        // -----------------
        public static readonly DependencyProperty SelectionIsItalicProperty = DependencyProperty.Register(
            "SelectionIsItalic", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, (value == true) ? FontStyles.Italic : FontStyles.Normal);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsUnderline
        // --------------------
        public static readonly DependencyProperty SelectionIsUnderlineProperty = DependencyProperty.Register(
            "SelectionIsUnderline", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, (value == true) ? System.Windows.TextDecorations.Underline : null);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignLeft
        // --------------------
        public static readonly DependencyProperty SelectionIsAlignLeftProperty = DependencyProperty.Register(
            "SelectionIsAlignLeft", typeof(bool?), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Left : TextAlignment.Left);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignCenter
        // ----------------------
        public static readonly DependencyProperty SelectionIsAlignCenterProperty = DependencyProperty.Register(
            "SelectionIsAlignCenter", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Center : TextAlignment.Left);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignRight
        // ---------------------
        public static readonly DependencyProperty SelectionIsAlignRightProperty = DependencyProperty.Register(
            "SelectionIsAlignRight", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Right : TextAlignment.Left);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsAlignJustify
        // -----------------------
        public static readonly DependencyProperty SelectionIsAlignJustifyProperty = DependencyProperty.Register(
            "SelectionIsAlignJustify", typeof(bool?), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Justify : TextAlignment.Left);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionParagraphIsLeftToRight
        // -------------------------------
        public static readonly DependencyProperty SelectionParagraphIsLeftToRightProperty = DependencyProperty.Register(
            "SelectionParagraphIsLeftToRight", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.LeftToRight : FlowDirection.RightToLeft);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionParagraphIsRightToLeft
        // -------------------------------
        public static readonly DependencyProperty SelectionParagraphIsRightToLeftProperty = DependencyProperty.Register(
            "SelectionParagraphIsRightToLeft", typeof(bool), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsBullets
        // ------------------
        public static readonly DependencyProperty SelectionIsBulletsProperty = DependencyProperty.Register(
            "SelectionIsBullets", typeof(bool?), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleBullets.Execute(null, richEditor.RichTextBox);
            richEditor.OnSelectionChanged(null, null);
        }

        // SelectionIsNumbering
        // --------------------
        public static readonly DependencyProperty SelectionIsNumberingProperty = DependencyProperty.Register(
            "SelectionIsNumbering", typeof(bool?), typeof(RichTextEditor),
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
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleNumbering.Execute(null, richEditor.RichTextBox);
            richEditor.OnSelectionChanged(null, null);
        }

        #endregion
    }
}