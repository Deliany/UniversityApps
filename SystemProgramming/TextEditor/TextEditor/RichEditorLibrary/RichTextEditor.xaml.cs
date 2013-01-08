using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using Microsoft.Windows.Controls.Ribbon;
using HTMLConverter;
using Adastra.RichEditorLibrary.Properties;
using System.Collections;
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Printing;
using System.Windows.Xps;

namespace Adastra.RichEditorLibrary
{       
    /// <summary>
    /// Interaction logic for RichTextEditor.xaml
    /// </summary>
    public partial class RichTextEditor : UserControl, IRichTextEditor, INotifyPropertyChanged
    {
        #region | Constants |

        private const string mc_strHtmlHeader = "<!DOCTYPE HTML PUBLIC";

        #endregion //| Constants |

        #region | Static members |

        public static RoutedCommand HotKeySaveAs = new RoutedCommand();
        public static RoutedCommand HotKeySave = new RoutedCommand();
        public static RoutedCommand HotKeyOpen = new RoutedCommand();
        public static RoutedCommand HotKeyUndo = new RoutedCommand();
        public static RoutedCommand HotKeyRedo = new RoutedCommand();
        public static RoutedCommand HotKeyNew = new RoutedCommand();

        #endregion //| Static members |

        #region | Events |

        public event TextChangedEventHandler TextChanged;

        #endregion //| Events |

        #region | Instance variables |

        private bool                                m_updateSelectionPropertiesPending = false;
        private ResizingAdorner                     m_resizingAdorner = null;
        private Dictionary<string,string>           m_dictInsertTextItems = null;     
        private bool                                m_bDocumentChanged = false;
        private bool                                m_bDocumentOnDiscChanged = false;

        private bool                                m_bIgnoreTextChanges = false;
        private string                              m_strFileName = String.Empty;


        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// Static constructor
        /// </summary>
        static RichTextEditor()
        {
            try
            {
                AuthenticationManager.CredentialPolicy = new ProxyCredentials();
                HttpWebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
                HttpWebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RichTextEditor static constructor ex: " + ex);
                Debug.Assert(false, ex.ToString());
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();

            foreach (FontFamily family in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                RibbonComboBoxItem rci = new RibbonComboBoxItem();
                rci.Content = family.Source;
                rci.FontFamily = family;
                rci.FontSize = 13;
                m_rcbxFontName.Items.Add(rci);
            }

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeySave, this.SaveDocCommandExecuted));
            RichTextEditor.HotKeySave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeySaveAs, this.SaveAsDocCommandExecuted));
            RichTextEditor.HotKeySaveAs.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeyOpen, this.OpenDocCommandExecuted));
            RichTextEditor.HotKeyOpen.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeyUndo, this.UndoCommandExecuted));
            RichTextEditor.HotKeyUndo.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeyRedo, this.RedoCommandExecuted));
            RichTextEditor.HotKeyRedo.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));

            this.CommandBindings.Add(new CommandBinding(RichTextEditor.HotKeyNew, this.NewDocCommandExecuted));
            RichTextEditor.HotKeyNew.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));

            this.BindRibbonCommands();

            /**/
            //### Attach background handler to update edit control's UI when RichTextBox's selection changes.
            this.RichTextBox.Selection.Changed += new EventHandler(OnRichTextBox_SelectionChanged);
            this.RichTextBox.ContextMenuOpening += new ContextMenuEventHandler(OnRichTextBox_ContextMenuOpening);
            this.RichTextBox.TextChanged += new TextChangedEventHandler(OnRichTextBox_TextChanged);

            this.RichTextBox.Loaded += new RoutedEventHandler(OnRichTextBox_Loaded);

            //### Enforce the first update. MiscCommands.InstallTableCallbacks(this.m_RTB);
            this.OnRichTextBox_SelectionChanged(this, null);
            this.Document.FontFamily = new FontFamily("Tahoma");
            this.m_RTB.FontFamily = new FontFamily("Tahoma");
            this.DataContext = this;

            this.m_colorPickerFont.ColorAutomatic = Brushes.Black.Color;
            this.m_colorBackgroundPickerFont.ColorAutomatic = Brushes.White.Color;

            this.m_colorPickerFont.ColorChanged += new ColorChangedEventHandler(OnFontColorChanged);
            this.m_colorBackgroundPickerFont.ColorChanged += new ColorChangedEventHandler(OnFontBackgroundColorChanged);
        }

        private void BindRibbonCommands()
        {
            #region | Command rebinding |
            //### Workaround CAB control hosting problem
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtOpenDoc.Command, OpenDocCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtSaveDoc.Command, SaveDocCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtSaveAsDoc.Command, SaveAsDocCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtNewDoc.Command, NewDocCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtUndo.Command, UndoCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtRedo.Command, RedoCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtPaste.Command, PasteCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtCut.Command, CutCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtCopy.Command, CopyCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtDecrease.Command, DecreaseCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtIncrease.Command, IncreaseCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbLeft.Command, LeftCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbRight.Command, RightCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbCenter.Command, CenterCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbJustify.Command, JustifyCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertPicture.Command, InsertPictureCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertHyperlink.Command, InsertHyperlinkCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertActiveTextButt.Command, InsertActiveTextCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertTable.Command, InsertTableCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertRowDown.Command, InsertRowDownCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertRowUp.Command, InsertRowUpCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertColumnLeft.Command, InsertColumnLeftCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertColumnRight.Command, InsertColumnRightCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtDelRow.Command, DelRowCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtDelCol.Command, DelColCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtDelTable.Command, DelTableCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtInsertLine.Command, InsertLineCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbBold.Command, BoldCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbItalic.Command, ItalicCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbUnderline.Command, UnderlineCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbSpellCheck.Command, SpellCheckCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbNumbering.Command, NumberingCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbBullets.Command, BulletsCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbbtPrint.Command, PrintCommandExecuted));

            this.CommandBindings.Add(new CommandBinding(this.m_rbtbFontColor.Command, FontColorCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbtbFontBackgroundColor.Command, FontBackgroundColorCommandExecuted));

            this.CommandBindings.Add(new CommandBinding(this.m_rbmnThemes.Command, NullRibonCommand));
            this.CommandBindings.Add(new CommandBinding(this.m_rbmn2007Black.Command, Mn2007BlackCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbmn2007Blue.Command, Mn2007BlueCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(this.m_rbmn2007Silver.Command, Mn2007SilverCommandExecuted));
            
            #endregion //| Command rebinding |
        }

        public void OnRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.m_bIgnoreTextChanges)
            {
                this.DocumentChanged = true;
                this.DocumentOnDiscChanged = true;
                this.OnRichTextEditorTextChanged(e);
            }
        }

        private void OnRichTextEditorTextChanged(TextChangedEventArgs e)
        {
            if (null != this.TextChanged)
            {
                this.TextChanged(this,e);
            }
        }

        #endregion //| Constructor stuff |

        #region | RichTextEditor interface |
        
        #region | Dependency properties |
        /// <summary>
        /// DependencyProperty IsSpellCheckEnabled
        /// </summary>
        private static readonly DependencyProperty DocTitleProperty = DependencyProperty.Register(
                                                                "DocTitle", typeof(string), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata((string)"Document", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnDocTitlePropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property IsSpellCheckEnabled
        /// </summary>
        [Bindable(true), Browsable(true), Category("Appearance")]
        public string DocTitle
        {
            get { return (string)GetValue(DocTitleProperty) + this.DocFileName; }
            set 
            {
                SetValue(DocTitleProperty, (value + " - " + Res.txtEditation)); 
            }
        }

        public string DocTitle2
        {
            get { return this.DocFileName + " - " +  Res.txtEditation; }
        }

        public string DocFileName
        {
            get
            {
                try
                {
                    if (String.IsNullOrEmpty(this.FileName))
                    {
                        return String.Empty;
                    }
                    int iBackSlash = this.FileName.LastIndexOf('\\');
                    iBackSlash += (-1 == iBackSlash ? 0 : 1);
                    string strRetVal = this.FileName.Substring((-1 == iBackSlash ? 0 : iBackSlash), this.FileName.Length - iBackSlash); ;
                    return strRetVal;
                }
                catch (Exception)
                {
                    return this.FileName;
                }
            }
        }

        public string FileName
        {
            get { return this.m_strFileName; }
            set 
            {
                this.m_strFileName = value;
                this.NotifyPropertyChanged("DocTitle2");
            }
        }

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
        /// Handler for PropertyChangedCallback of DependencyProperty DocTitle
        /// </summary>
        private static void OnDocTitlePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// DependencyProperty IsSpellCheckEnabled
        /// </summary>
        private static readonly DependencyProperty IsSpellCheckEnabledProperty = DependencyProperty.Register(
                                                                "IsSpellCheckEnabled", typeof(bool), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata((bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnIsSpellCheckEnabledPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property IsSpellCheckEnabled
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        private bool IsSpellCheckEnabled
        {
            get { return (bool)GetValue(IsSpellCheckEnabledProperty); }
            set { SetValue(IsSpellCheckEnabledProperty, value); }
        }
        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty IsSpellCheckEnabled
        /// </summary>
        private static void OnIsSpellCheckEnabledPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)o;
            bool value = (bool)e.NewValue;
            richEditor.RichTextBox.SpellCheck.IsEnabled = value;
        }

        private void OnFontSizeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.m_rcbxFontSize.SelectedItem)
            {
                this.SelectionFontSize = ((RibbonComboBoxItem)m_rcbxFontSize.SelectedItem).Content as string;
            }
            this.m_rbbtPaste.IsEnabled = true;
            //this.m_rbbtPaste.Command.CanExecute(
        }

        private void OnFontNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (null != this.m_rcbxFontName.SelectedItem)
            {
                string strFontFamily = ((RibbonComboBoxItem)m_rcbxFontName.SelectedItem).Content as string;
                this.m_rcbxFontName.FontFamily = new FontFamily(strFontFamily);
                this.SelectionFontFamily = strFontFamily;
            }
        }

        private void SelectRibbonCbxItem(RibbonComboBox ribComboBox, string strFontName)
        {
            if (String.IsNullOrEmpty(strFontName))
            {
                return;
            }

            foreach (RibbonComboBoxItem rcbxItem in ribComboBox.Items)
            {
                if (((string)rcbxItem.Content).ToLower() == strFontName.ToLower())
                {
                    ribComboBox.SelectedItem = rcbxItem;
                    ribComboBox.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// DependencyProperty SelectionFontFamily
        /// </summary>
        private static readonly DependencyProperty SelectionFontFamilyProperty = DependencyProperty.Register(
                                                                "SelectionFontFamily", typeof(string), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnSelectionFontFamilyPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionFontFamily
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        private string SelectionFontFamily
        {
            get { return (string)GetValue(SelectionFontFamilyProperty); }
            set 
            {
                SetValue(SelectionFontFamilyProperty, value);
                this.SelectRibbonCbxItem(this.m_rcbxFontName, value);
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionFontFamily
        /// </summary>
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
                richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value.ToLower());
                richEditor.OnRichTextBox_SelectionChanged(null, null);
            }
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        private static readonly DependencyProperty SelectionFontSizeProperty = DependencyProperty.Register(
                                                                "SelectionFontSize", typeof(string), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnSelectionFontSizePropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionFontSize
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        private string SelectionFontSize
        {
            get { return (string)GetValue(SelectionFontSizeProperty); }
            set 
            {
                SetValue(SelectionFontSizeProperty, value);
                this.SelectRibbonCbxItem(this.m_rcbxFontSize, value);
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionFontSize
        /// </summary>
        private static void OnSelectionFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            string value = (string)e.NewValue;
            if (value != null)
            {
                richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value);
                richEditor.OnRichTextBox_SelectionChanged(null, null);
            }
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        public static readonly DependencyProperty SelectionIsBoldProperty = DependencyProperty.Register(
                                                            "SelectionIsBold", typeof(bool), typeof(RichTextEditor),
                                                            new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                          new PropertyChangedCallback(OnSelectionIsBoldPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsBold
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsBold
        {
            get { return (bool)GetValue(SelectionIsBoldProperty); }
            set 
            {
                SetValue(SelectionIsBoldProperty, value); 
                this.m_rbtbBold.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsBold
        /// </summary>
        private static void OnSelectionIsBoldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, (value == true) ? FontWeights.Bold : FontWeights.Normal);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        public static readonly DependencyProperty SelectionIsItalicProperty = DependencyProperty.Register(
                                                                "SelectionIsItalic", typeof(bool), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnSelectionIsItalicPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsItalic
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsItalic
        {
            get { return (bool)GetValue(SelectionIsItalicProperty); }
            set 
            {
                SetValue(SelectionIsItalicProperty, value);
                this.m_rbtbItalic.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsItalic
        /// </summary>
        private static void OnSelectionIsItalicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, (value == true) ? FontStyles.Italic : FontStyles.Normal);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        public static readonly DependencyProperty SelectionIsUnderlineProperty = DependencyProperty.Register(
                                                                    "SelectionIsUnderline", typeof(bool), typeof(RichTextEditor),
                                                                    new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  new PropertyChangedCallback(OnSelectionIsUnderlinePropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsUnderline
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsUnderline
        {
            get { return (bool)GetValue(SelectionIsUnderlineProperty); }
            set 
            {
                SetValue(SelectionIsUnderlineProperty, value);
                this.m_rbtbUnderline.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsUnderline
        /// </summary>
        private static void OnSelectionIsUnderlinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, (value == true) ? System.Windows.TextDecorations.Underline : null);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        public static readonly DependencyProperty SelectionIsAlignLeftProperty = DependencyProperty.Register(
                                                                    "SelectionIsAlignLeft", typeof(bool?), typeof(RichTextEditor),
                                                                    new FrameworkPropertyMetadata((bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  new PropertyChangedCallback(OnSelectionIsAlignLeftPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsAlignLeft
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignLeft
        {
            get { return (bool)GetValue(SelectionIsAlignLeftProperty); }
            set { SetValue(SelectionIsAlignLeftProperty, value); }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsAlignLeft
        /// </summary>
        private static void OnSelectionIsAlignLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Left : TextAlignment.Left);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionFontSize
        /// </summary>
        public static readonly DependencyProperty SelectionIsAlignCenterProperty = DependencyProperty.Register(
                                                                    "SelectionIsAlignCenter", typeof(bool), typeof(RichTextEditor),
                                                                    new FrameworkPropertyMetadata((bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  new PropertyChangedCallback(OnSelectionIsAlignCenterPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsAlignCenter
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignCenter
        {
            get { return (bool)GetValue(SelectionIsAlignCenterProperty); }
            set 
            {
                SetValue(SelectionIsAlignCenterProperty, value);
                this.m_rbtbCenter.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsAlignCenter
        /// </summary>
        private static void OnSelectionIsAlignCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Center : TextAlignment.Left);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionIsAlignRight
        /// </summary>
        public static readonly DependencyProperty SelectionIsAlignRightProperty = DependencyProperty.Register(
                                                                    "SelectionIsAlignRight", typeof(bool), typeof(RichTextEditor),
                                                                    new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  new PropertyChangedCallback(OnSelectionIsAlignRightPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsAlignRight
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignRight
        {
            get { return (bool)GetValue(SelectionIsAlignRightProperty); }
            set 
            {
                SetValue(SelectionIsAlignRightProperty, value); 
                
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsAlignRight
        /// </summary>
        private static void OnSelectionIsAlignRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Right : TextAlignment.Left);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionIsAlignJustify
        /// </summary>
        public static readonly DependencyProperty SelectionIsAlignJustifyProperty = DependencyProperty.Register(
                                                                    "SelectionIsAlignJustify", typeof(bool?), typeof(RichTextEditor),
                                                                    new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                  new PropertyChangedCallback(OnSelectionIsAlignJustifyPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsAlignJustify
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsAlignJustify
        {
            get { return (bool)GetValue(SelectionIsAlignJustifyProperty); }
            set 
            {
                SetValue(SelectionIsAlignJustifyProperty, value);
                this.m_rbtbJustify.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsAlignJustify
        /// </summary>
        private static void OnSelectionIsAlignJustifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, (value == true) ? TextAlignment.Justify : TextAlignment.Left);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionParagraphIsLeftToRight
        /// </summary>
        public static readonly DependencyProperty SelectionParagraphIsLeftToRightProperty = DependencyProperty.Register(
                                                                                "SelectionParagraphIsLeftToRight", typeof(bool), typeof(RichTextEditor),
                                                                                new FrameworkPropertyMetadata((bool)true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                              new PropertyChangedCallback(OnSelectionParagraphIsLeftToRightPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionParagraphIsLeftToRight
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionParagraphIsLeftToRight
        {
            get { return (bool)GetValue(SelectionParagraphIsLeftToRightProperty); }
            set 
            {
                SetValue(SelectionParagraphIsLeftToRightProperty, value);
                this.m_rbtbLeft.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionParagraphIsLeftToRight
        /// </summary>
        private static void OnSelectionParagraphIsLeftToRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.LeftToRight : FlowDirection.RightToLeft);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionParagraphIsRightToLeft
        /// </summary>
        public static readonly DependencyProperty SelectionParagraphIsRightToLeftProperty = DependencyProperty.Register(
                                                                            "SelectionParagraphIsRightToLeft", typeof(bool), typeof(RichTextEditor),
                                                                            new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                          new PropertyChangedCallback(OnSelectionParagraphIsRightToLeftPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionParagraphIsRightToLeft
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionParagraphIsRightToLeft
        {
            get { return (bool)GetValue(SelectionParagraphIsRightToLeftProperty); }
            set 
            {
                SetValue(SelectionParagraphIsRightToLeftProperty, value);
                this.m_rbtbRight.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionParagraphIsRightToLeft
        /// </summary>
        private static void OnSelectionParagraphIsRightToLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            richEditor.RichTextBox.Selection.ApplyPropertyValue(Paragraph.FlowDirectionProperty, (value == true) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionIsBullets
        /// </summary>
        public static readonly DependencyProperty SelectionIsBulletsProperty = DependencyProperty.Register(
                                                                "SelectionIsBullets", typeof(bool?), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnSelectionIsBulletsPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsBullets
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsBullets
        {
            get { return (bool)GetValue(SelectionIsBulletsProperty); }
            set 
            {
                SetValue(SelectionIsBulletsProperty, value);
                this.m_rbtbBullets.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsBullets
        /// </summary>
        private static void OnSelectionIsBulletsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleBullets.Execute(null, richEditor.RichTextBox);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }

        /// <summary>
        /// DependencyProperty SelectionIsNumbering
        /// </summary>
        public static readonly DependencyProperty SelectionIsNumberingProperty = DependencyProperty.Register(
                                                                "SelectionIsNumbering", typeof(bool?), typeof(RichTextEditor),
                                                                new FrameworkPropertyMetadata((bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                              new PropertyChangedCallback(OnSelectionIsNumberingPropertyChanged), null));

        /// <summary>
        /// Get/Set dependency property SelectionIsNumbering
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool SelectionIsNumbering
        {
            get { return (bool)GetValue(SelectionIsNumberingProperty); }
            set 
            {
                SetValue(SelectionIsNumberingProperty, value);
                this.m_rbtbNumbering.IsChecked = value;
            }
        }

        /// <summary>
        /// Handler for PropertyChangedCallback of DependencyProperty SelectionIsNumbering
        /// </summary>
        private static void OnSelectionIsNumberingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextEditor richEditor = (RichTextEditor)d;
            if (richEditor.m_updateSelectionPropertiesPending) return;
            bool value = (bool)e.NewValue;

            EditingCommands.ToggleNumbering.Execute(null, richEditor.RichTextBox);
            richEditor.OnRichTextBox_SelectionChanged(null, null);
        }
        
        #endregion //| Dependency properties |
        /// <summary>
        /// Get editor content in HTML form
        /// </summary>
        public string HTML
        {
            get { return HtmlFromXamlConverter.ConvertXamlToHtml(this.Document); }
            set
            {
                this.Document = HtmlToXamlConverter.ConvertHtmlToXaml(value);
            }
        }

        public string DocumentBody
        {
            get { return HtmlFromXamlConverter.SerializeFlowDocument(this.Document); }
            set 
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                if (value.StartsWith(mc_strHtmlHeader,true,CultureInfo.CurrentCulture))
                {
                    //### DOcument type is HTML (backward compatibility)
                    this.HTML = value;
                    return;
                }
                
                this.Document = HtmlToXamlConverter.DeserializeFlowDocument(value); 
            }
        }

        /// <summary>
        /// Loads RTF format into RichTextBox
        /// </summary>
        /// <param name="rtf"></param>
        private void LoadRTF(string rtf)
        {
            if (string.IsNullOrEmpty(rtf))
            {
                throw new ArgumentNullException();
            }

            TextRange textRange = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);

            //### Create a MemoryStream of the Rtf content
            using (MemoryStream rtfMemoryStream = new MemoryStream())
            {
                using (StreamWriter rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                {
                    rtfStreamWriter.Write(rtf);
                    rtfStreamWriter.Flush();
                    rtfMemoryStream.Seek(0, SeekOrigin.Begin);

                    //### Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                    textRange.Load(rtfMemoryStream, DataFormats.Rtf);
                }
            }
        }

        private FlowDocument Document
        {
            get {return this.RichTextBox.Document;}
            set 
            {
                this.RichTextBox.Selection.Changed -= new EventHandler(OnRichTextBox_SelectionChanged);
                try
                {
                    this.m_bIgnoreTextChanges = true;
                    this.RichTextBox.Document = value;

                    foreach (Block block in this.RichTextBox.Document.Blocks)
                    {
                        if (block is Table)
                        {
                            MiscCommands.InstallTableCallbacks(block as Table);
                        }

                        //foreach (Block inline in block.SiblingBlocks)
                        //{
                        //    bool bIsTable = inline is Table;
                        //    int i = 9;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Res.txtLoadFailed + " :\n\r" +ex,this.DocTitle);
                    //Clipboard.SetData(DataFormats.Text, value);
                }
                finally
                {
                    this.m_bIgnoreTextChanges = false;
                    this.RichTextBox.Selection.Changed += new EventHandler(OnRichTextBox_SelectionChanged);
                }
            }
        }

        public Dictionary<string, string> InsertCustomTextItems
        {
            get { return this.m_dictInsertTextItems; }
            set { this.m_dictInsertTextItems = value; }
        }

        public bool DocumentChanged
        {
            get { return this.m_bDocumentChanged; }
            set 
            {
                this.m_bDocumentChanged = value; 
            }
        }

        public bool DocumentOnDiscChanged
        {
            get { return this.m_bDocumentOnDiscChanged; }
            set 
            {
                m_bDocumentOnDiscChanged = value; 
                this.m_rbbtSaveDoc.IsEnabled = value;
            }
        }

        #endregion //| RichTextEditor interface |

        #region | Private members |
        #region | Properties |
        /// <summary>
        /// Get/Set main RichTextBox object
        /// </summary>
        public RichTextBox RichTextBox
        {
            get { return this.m_RTB; }
        }

        #endregion //| Properties |

        #region | Methods |
        /// <summary>
        /// Handler for Load event of main RichTextBox object
        /// </summary>
        void OnRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.Resources.MergedDictionaries.Add(Microsoft.Windows.Controls.Ribbon.PopularApplicationSkins.Office2007Black);

            FocusManager.SetIsFocusScope(this, true);
            FocusManager.SetIsFocusScope(this.m_Crl, true);
            FocusManager.SetIsFocusScope(this.m_rbbtPaste, true);
            this.m_rbbtPaste.Focus();
            //### FontFamily
            //object value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            //this.SelectRibbonCbxItem(this.m_rcbxFontName,(value == DependencyProperty.UnsetValue) ? null : value.ToString());

            ////### FontSize
            //value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            //this.SelectRibbonCbxItem(this.m_rcbxFontSize,(value == DependencyProperty.UnsetValue) ? null : String.Format("{0:0}",value.ToString()));

            this.IsSpellCheckEnabled = false;

            //### Fill insert custom text menu
            if (null != this.InsertCustomTextItems)
            {
                foreach (string strTextItem in this.InsertCustomTextItems.Keys)
                {
                    //MenuItem menuItem = new MenuItem();
                    //menuItem.Tag = strTextItem;
                    //menuItem.Header = this.InsertCustomTextItems[strTextItem];
                    //menuItem.Click += new RoutedEventHandler(InsertCustomTextMenuItem_Click);
                    //menuItem.Command = this.m_rbtbBold.Command;

                    //this.m_rbInsertActiveText.Items.Add(menuItem);
                    RibbonComboBoxItem ribbonComboBoxItem = new RibbonComboBoxItem();
                    ribbonComboBoxItem.Content = this.InsertCustomTextItems[strTextItem];
                    ribbonComboBoxItem.Tag = strTextItem;
                    this.m_rbInsertActiveText.Items.Add(ribbonComboBoxItem);
                }
            }
            this.m_rbbtPaste.IsEnabled = true;
        }

        void InsertCustomTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                string strCustomText = (string)((MenuItem)sender).Tag;
                TextRange textRange = this.m_RTB.Selection;
                textRange.Text = strCustomText;
            }
        }

        private const int mc_iPropertiesIdx = 4;
        /// <summary>
        /// Handler for ContextMenuOpening event of main RichTextBox object
        /// </summary>
        void OnRichTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MenuItem menuProperties = this.m_RTB.ContextMenu.Items[mc_iPropertiesIdx] as MenuItem;
            if (null != menuProperties)
            {
                string strHeaderText = Res.txtProperties + " ";

                if (MiscCommands.IsImageSelected(this.m_RTB))
                {
                    strHeaderText += Res.txtPicture;
                }
                else if (MiscCommands.IsTableSelected(this.m_RTB))
                {
                    strHeaderText += Res.txtTable;
                }
                menuProperties.Header = strHeaderText;
            }
        }

        /// <summary>
        /// Handler for SelectionChanged event of main RichTextBox object
        /// </summary>
        private void OnRichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (!this.m_updateSelectionPropertiesPending)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                         new DispatcherOperationCallback(UpdateSelectionProperties),
                                                         null);
                this.m_updateSelectionPropertiesPending = true;
            }
        }

        /// <summary>
        /// Update UI components of the editor
        /// </summary>
        /// <param name="arg">Parameters</param>
        /// <returns></returns>
        private object UpdateSelectionProperties(object arg)
        {
            object value;

            try
            {
                //### FontFamily
                value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
                this.SelectionFontFamily = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

                //### FontSize
                value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
                this.SelectionFontSize = (value == DependencyProperty.UnsetValue) ? null : value.ToString();

                //### FontWeight
                value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
                this.SelectionIsBold = (value == DependencyProperty.UnsetValue) ? false : ((FontWeight)value == FontWeights.Bold);

                //### FontStyle
                value = this.RichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
                this.SelectionIsItalic = (value == DependencyProperty.UnsetValue) ? false : ((FontStyle)value == FontStyles.Italic);

                //### Underline
                value = this.RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                this.SelectionIsUnderline = (value == DependencyProperty.UnsetValue) ? false : value != null && System.Windows.TextDecorations.Underline.Equals(value);

                //### TextAlignment
                value = this.RichTextBox.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
                this.SelectionIsAlignLeft = (value == DependencyProperty.UnsetValue) ? true : (TextAlignment)value == TextAlignment.Left;
                this.SelectionIsAlignCenter = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Center;
                this.SelectionIsAlignRight = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Right;
                this.SelectionIsAlignJustify = (value == DependencyProperty.UnsetValue) ? false : (TextAlignment)value == TextAlignment.Justify;

                //### FlowDirection
                if (this.RichTextBox.Selection.Start.Paragraph != null)
                {
                    FlowDirection flowDirection = this.RichTextBox.Selection.Start.Paragraph.FlowDirection;
                    this.SelectionParagraphIsLeftToRight = flowDirection == FlowDirection.LeftToRight;
                    this.SelectionParagraphIsRightToLeft = flowDirection == FlowDirection.RightToLeft;
                }

                //### Bullets and Numbering
                Paragraph startParagraph = this.RichTextBox.Selection.Start.Paragraph;
                Paragraph endParagraph = this.RichTextBox.Selection.End.Paragraph;
                if (startParagraph != null && endParagraph != null &&
                    (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) &&
                    ((ListItem)startParagraph.Parent).List == ((ListItem)endParagraph.Parent).List)
                {
                    TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
                    this.SelectionIsBullets = (markerStyle == TextMarkerStyle.Disc ||
                                               markerStyle == TextMarkerStyle.Circle ||
                                               markerStyle == TextMarkerStyle.Square ||
                                               markerStyle == TextMarkerStyle.Box);
                    this.SelectionIsNumbering = (markerStyle == TextMarkerStyle.LowerRoman ||
                                                 markerStyle == TextMarkerStyle.UpperRoman ||
                                                 markerStyle == TextMarkerStyle.LowerLatin ||
                                                 markerStyle == TextMarkerStyle.UpperLatin ||
                                                 markerStyle == TextMarkerStyle.Decimal);
                }

                //### Update status bar line info
                this.m_StatusBarLineInfo.Text = String.Format("{0} : {1} {2}: {3}",Res.strLine, Helper.GetLineNumberFromSelection(this.RichTextBox.Selection.Start),
                                                                                   Res.strColumn, Helper.GetColumnNumberFromSelection(this.RichTextBox.Selection.Start));

                //### Adorner
                this.SetAdorner();
                if (null == this.m_RTB.ContextMenu || !this.m_RTB.ContextMenu.IsOpen)
                {
                    this.m_RTB.Focus();
                }
            }
            finally
            {
                this.m_updateSelectionPropertiesPending = false;
            }
            return null;
        }
        

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

        /// <summary>
        /// Handler for riboon button Copy operation
        /// </summary>
        private void CopyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, this.RichTextBox);
        }

        /// <summary>
        /// Handler for riboon button Paste operation
        /// </summary>
        private void PasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Paste.Execute(null, this.RichTextBox);
        }

        /// <summary>
        /// Handler for riboon button Cut operation
        /// </summary>
        private void CutCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Cut.Execute(null, this.RichTextBox);
        }

        
        private void SaveDocCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveAsDoc(this.FileName); 
        }

        private void SaveAsDocCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveAsDoc(String.Empty); 
        }

        private void SaveAsDoc(string strFileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string strDocument = String.Empty;

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Title = Res.txtEnterFileName;
            saveFileDialog.Filter = Res.txtSupportedDocuments;
            saveFileDialog.FileName = strFileName;

            if (!String.IsNullOrEmpty(strFileName) || true == saveFileDialog.ShowDialog())
            {
                Stream fileStream = null;
                this.FileName = saveFileDialog.FileName;

                if ((fileStream = saveFileDialog.OpenFile()) != null)
                {
                    using (fileStream)
                    {
                        StreamWriter streamReader = new StreamWriter(fileStream);
                        string strDocumentData = (saveFileDialog.FileName.EndsWith(".htm") || saveFileDialog.FileName.EndsWith(".html") ? this.HTML : this.DocumentBody);

                        streamReader.BaseStream.Position = 0;
                        streamReader.Write(strDocumentData);
                        streamReader.Flush();
                        this.DocumentOnDiscChanged = false;
                    }
                }
            }        
        }

        private void NewDocCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Document = new FlowDocument();
            this.FileName = String.Empty;
            this.DocumentOnDiscChanged = false;
        }

        private void OpenDocCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string strDocument = String.Empty;

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false;
            openFileDialog.Title = Res.txtEnterFileName;
            openFileDialog.Filter = Res.txtSupportedDocuments;

            if (true == openFileDialog.ShowDialog())
            {
                Stream fileStream = null;
                try
                {
                    if ((fileStream = openFileDialog.OpenFile()) != null)
                    {
                        using (fileStream)
                        {
                            StreamReader streamReader = new StreamReader(fileStream);

                            streamReader.BaseStream.Position = 0;
                            strDocument = streamReader.ReadToEnd();
                            //this.HTML = strDocument;

                            if (strDocument.StartsWith("<FlowDocument", true, CultureInfo.CurrentCulture))
                            {
                                this.Document = HtmlToXamlConverter.DeserializeFlowDocument(strDocument);
                            }
                            else if (strDocument.StartsWith("{\\rtf", true, CultureInfo.CurrentCulture))
                            {
                                this.LoadRTF(strDocument);
                            }
                            else
                            {
                                this.Document = HtmlToXamlConverter.ConvertHtmlToXaml(strDocument);
                            }
                            this.DocumentChanged = true;
                            this.DocumentOnDiscChanged = false;
                            this.FileName = openFileDialog.FileName;
                            this.OnRichTextEditorTextChanged(null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Res.txtOpenFileFailed + " \n\r\n\r" + ex, Res.txtEditorName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void IncreaseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            EditingCommands.IncreaseIndentation.Execute(null, this.RichTextBox);
        }

        private void DecreaseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            EditingCommands.DecreaseIndentation.Execute(null, this.RichTextBox);
        }

        private void LeftCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsAlignLeft = (true == this.m_rbtbLeft.IsChecked);
            this.m_rbtbJustify.IsChecked = this.m_rbtbCenter.IsChecked = this.m_rbtbRight.IsChecked = false;
        }

        private void RightCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsAlignRight = (true == this.m_rbtbRight.IsChecked);
            this.m_rbtbJustify.IsChecked = this.m_rbtbCenter.IsChecked = this.m_rbtbLeft.IsChecked = false;
        }

        private void CenterCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsAlignCenter = (true == this.m_rbtbCenter.IsChecked);
            this.m_rbtbJustify.IsChecked = this.m_rbtbRight.IsChecked = this.m_rbtbLeft.IsChecked = false;
        }

        private void JustifyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsAlignJustify = (true == this.m_rbtbJustify.IsChecked);
            this.m_rbtbCenter.IsChecked = this.m_rbtbRight.IsChecked = this.m_rbtbLeft.IsChecked = false;
        }

        private void UndoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Undo.Execute(null, this.RichTextBox);
        }

        private void RedoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Redo.Execute(null, this.RichTextBox);
        }

        private void InsertTableCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnInsertTable(this,e);
        }

        private void InsertRowDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnInsertRowsBelow(this, e);
        }

        private void InsertRowUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnInsertRowsAbove(this, e);
        }

        private void InsertColumnLeftCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnInsertColumnsToLeft(this, e);
        }

        private void InsertColumnRightCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnInsertColumnsToRight(this, e);
        }

        private void DelRowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnDeleteRows(this, e);
        }

        private void DelTableCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnDeleteTable(this, e);
        }

        private void TablePropertiesCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnEditTableProperties(this, e);
        }

        private void DelColCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableCommands.OnDeleteColumns(this, e);
        }

        private void InsertPictureCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PictureCommands.OnInsertPicture(this, e);
        }

        private void InsertLineCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PictureCommands.OnInsertLine(this, e);
        }

        private void InsertHyperlinkCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PictureCommands.OnInsertHyperlink(this,e);
        }

        private void NumberingCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsNumbering = (true == this.m_rbtbNumbering.IsChecked);
        }

        private void BulletsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsBullets = (true == this.m_rbtbBullets.IsChecked);
        }

        private void PrintCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //PrintDialog printDlg = new PrintDialog();
            //if (true == printDlg.ShowDialog())
            //{
            //    this.Document.PageHeight = printDlg.PrintableAreaHeight;
            //    this.Document.PageWidth = printDlg.PrintableAreaWidth;
            //    printDlg.PrintDocument(((IDocumentPaginatorSource)this.Document).DocumentPaginator, "Tisk sablony");
            //}
            //return;
            //Printing with Paginator
            try
            {
                //set pagination with printer's margins and size
                PrintDocumentImageableArea area = null;
                XpsDocumentWriter xdwriter = PrintQueue.CreateXpsDocumentWriter(Res.txtTemplatePrintDesc,ref area);

                //make a copy to print with pagginator w/o crashing
                FlowDocument sourceDocumentCopy = HtmlToXamlConverter.DeserializeFlowDocument(this.DocumentBody);
                //TextPointer position1 = sourceDocumentCopy.ContentStart;
                //TextPointer position2 = sourceDocumentCopy.ContentEnd;
                //TextRange sourceDocumentRange = new TextRange(position1, position2);

                //MemoryStream tempstream = new MemoryStream();
                //sourceDocumentRange.Save(tempstream, System.Windows.DataFormats.Xaml);

                //FlowDocument sourceDocumentCopy = new FlowDocument();
                TextPointer position3 = sourceDocumentCopy.ContentStart;
                TextPointer position4 = sourceDocumentCopy.ContentEnd;
                TextRange copyDocumentRange = new TextRange(position3, position4);
                //copyDocumentRange.Load(tempstream, System.Windows.DataFormats.Xaml);

                if ((xdwriter != null) && (area != null))
                {
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)sourceDocumentCopy).DocumentPaginator;
                    paginator.PageSize = new Size(area.MediaSizeWidth, area.MediaSizeHeight);

                    Thickness pageBounds = sourceDocumentCopy.PagePadding;

                    double leftmargin, topmargin, rightmargin, bottommargin;
                    if (area.OriginWidth > pageBounds.Left)
                        leftmargin = area.OriginWidth;
                    else
                        leftmargin = pageBounds.Left;

                    if (area.OriginHeight > pageBounds.Top)
                        topmargin = area.OriginHeight;
                    else
                        topmargin = pageBounds.Top;

                    double printerRightMargin = area.MediaSizeWidth - (area.OriginWidth + area.ExtentWidth);
                    if (printerRightMargin > pageBounds.Right)
                        rightmargin = printerRightMargin;
                    else
                        rightmargin = pageBounds.Right;

                    double printerBottomMargin = area.MediaSizeHeight - (area.OriginHeight + area.ExtentHeight);
                    if (printerBottomMargin > pageBounds.Bottom)
                        bottommargin = printerBottomMargin;
                    else
                        bottommargin = pageBounds.Bottom;

                    sourceDocumentCopy.PagePadding = new Thickness(leftmargin, topmargin, rightmargin, bottommargin);

                    //can be used to set columns
                    sourceDocumentCopy.ColumnWidth = double.PositiveInfinity;

                    xdwriter.Write(paginator);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Res.txtPrintFailed + " \n\r\n\r" + ex.Message, Res.txtEditorName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoldCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsBold = (true == this.m_rbtbBold.IsChecked);
            //this.m_RTB.Focus();
        }

        private void ItalicCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsItalic = (true == this.m_rbtbItalic.IsChecked);
        }

        private void UnderlineCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SelectionIsUnderline = (true == this.m_rbtbUnderline.IsChecked);
        }

        private void SpellCheckCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.IsSpellCheckEnabled = (true == this.m_rbtbSpellCheck.IsChecked);
        }

        private void Mn2007BlackCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Resources.MergedDictionaries
               .Add(Microsoft.Windows.Controls.Ribbon.PopularApplicationSkins.Office2007Black);

        }

        private void Mn2007BlueCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Resources.MergedDictionaries
               .Add(Microsoft.Windows.Controls.Ribbon.PopularApplicationSkins.Office2007Blue);

        }

        private void Mn2007SilverCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Resources.MergedDictionaries
               .Add(Microsoft.Windows.Controls.Ribbon.PopularApplicationSkins.Office2007Silver);

        }

        private void FontColorCommandExecuted(object sender, ExecutedRoutedEventArgs e) 
        {
            this.OnFontColorChanged(this.m_colorPickerFont.Color);
            //this.m_RTB.Focus();
        }

        private void FontBackgroundColorCommandExecuted(object sender, ExecutedRoutedEventArgs e) 
        {
            this.OnFontBackgroundColorChanged(this.m_colorBackgroundPickerFont.Color);
            //this.m_RTB.Focus();
        }

        void OnFontColorChanged(Color color)
        {
            if(null == color)
            {
                return;
            }

            TextRange textRange = this.m_RTB.Selection;
            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, solidColorBrush);
        }

        void OnFontBackgroundColorChanged(Color color)
        {
            if(null == color)
            {
                return;
            }

            TextRange textRange = this.m_RTB.Selection;
            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, solidColorBrush);
        }

        private void NullRibonCommand(object sender, ExecutedRoutedEventArgs e) 
        {
            Debug.WriteLine("NullRibonCommand - " + sender);
        }
        #endregion //| Methods |

        #endregion //| Private members | 

        private void InsertActiveTextCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.InsertCustomData();
        }

        private void InsertCustomData()
        {
            RibbonComboBoxItem ribbonComboBoxItem = this.m_rbInsertActiveText.SelectedItem as RibbonComboBoxItem;
            if (null != ribbonComboBoxItem && !String.IsNullOrEmpty((string)ribbonComboBoxItem.Content))
            {
                string strCustomText = (string)ribbonComboBoxItem.Tag;
                TextRange textRange = this.m_RTB.Selection;
                textRange.Text = strCustomText;

                textRange.Select(textRange.End,textRange.End);

                this.m_RTB.Focus();
            }
        }
        
        private void OnInsertActiveTextSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.InsertCustomData();
        }



    }
}
