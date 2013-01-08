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
using System.ComponentModel;
using System.Diagnostics;

namespace Adastra.RichEditorLibrary
{
    public enum TableType
    {
        Undefined,
        NoneBorder,
        BoxBorder,
        FullBorder
    }

    /// <summary>
    /// Interaction logic for TablePropertiesDialog.xaml
    /// </summary>
    public partial class TablePropertiesDialog : Window, INotifyPropertyChanged
    {
        #region | Static stuff |
        private static double[] mst_doubleLineWidths = { 0,0.1,0.2,0.3,0.4,0.5 /*default*/,1,1.3,1.5,2,3,4,5} ;
        private static int mst_iDefaultLineWidthIdx = 5;
                     
        private static int mst_iDefaultWhiteColorIdx = 0;
        private static int mst_iDefaultBlackColorIdx = 1;
        private static int mst_iDefaultYelowColorIdx = 5;
        private static Color[] mst_colorArray = {                
                Colors.White,                                                       
                Colors.Black,  
                Colors.Blue,  
                Colors.Red, 
                Colors.Green,
                Colors.Yellow, 
                Colors.Purple,                
                Colors.Brown,        
                Colors.Gray,  
                Colors.DarkGray,
                Colors.DarkBlue,                                
                Colors.DarkGreen,                
                Colors.DarkMagenta,
                Colors.DarkOliveGreen,
                Colors.DarkOrange,
                Colors.DarkOrchid,
                Colors.DarkRed,                
                Colors.DarkTurquoise,                
                Colors.Gold,                
                Colors.Cyan,
                Colors.Violet,                             
                Colors.Aqua,                
                Colors.Beige,               
                Colors.GreenYellow,                
                Colors.Indigo,
                Colors.Ivory,                
                Colors.LightBlue,                
                Colors.Lime,
                Colors.Magenta,
                Colors.Maroon,
                Colors.MediumBlue,
                Colors.Navy, 
                Colors.Olive,                
                Colors.Orange,
                Colors.OrangeRed,                
                Colors.Pink,                                                            
                Colors.Tan,
                Colors.Teal,                
                Colors.Turquoise,                             
                Colors.YellowGreen
                };
        #endregion //| Static stuff |

        #region | Instance variables |

        private Table m_tableBackup;

        private Table m_table = null;
        private int m_iRows = 2;
        private int m_iColumns = 5;
        private Brush m_borderBrush = System.Windows.Media.Brushes.Black;
        private Thickness m_borderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);

        private bool m_bIsNoBorder = true;
        private bool m_bIsBoxBorder = false;
        private bool m_bIsFullBorder = false;

        private double m_dCellWidth = 100;
        private bool m_bIsCellWidthAutomatic = false;

        public TableType TableType { get; set; }

        #endregion //| Instance variables |

        #region | Constructor stuff |
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Image to bind with</param>
        public TablePropertiesDialog(Table table)
        {
            InitializeComponent();
            this.BindTable(table);
            this.Loaded += new RoutedEventHandler(TablePropertiesDialog_Loaded);
        }

        void TablePropertiesDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitUIControls();
            if (null != this.Table)
            {
                this.m_txBxColumns.IsEnabled = this.m_txBxRows.IsEnabled = false;
            }
        }

        #endregion //| Constructor stuff |

        #region | TablePropertiesDialog interface |
        #region | INotifyPropertyChanged Events |

        /// <summary>
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="strInfo"></param>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string strInfo)
        {
            //PropertyChangedEventHandler handler = PropertyChanged;

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(strInfo));
            }
        }

        #endregion //| INotifyPropertyChanged Events |

        public double CellWidth
        {
            get { return this.m_dCellWidth; }
            set 
            {
                this.m_dCellWidth = value;
                this.NotifyPropertyChanged("CellWidth");
            }
        }

        public double TableCellWidth
        {
            get { return (this.IsCellWidthAutomatic ? double.NaN : this.m_dCellWidth); }
        }

        public bool IsCellWidthAutomatic
        {
            get { return this.m_bIsCellWidthAutomatic; }
            set 
            {
                this.m_bIsCellWidthAutomatic = value; 
                if(this.m_bIsCellWidthAutomatic)
                {
                    //this.CellWidth = double.NaN;
                }
                this.m_txtCellWidth.IsEnabled = !this.m_bIsCellWidthAutomatic;
            }
        }

        public bool IsNoBorder
        {
            get { return this.m_bIsNoBorder; }
            set 
            { 
                this.m_bIsNoBorder = value; 
                this.m_bIsBoxBorder = !value; 
                this.m_bIsFullBorder = !value;
                this.NotifyPropertyChangedBorders();
                if(value)
                {
                    this.TableType = TableType.FullBorder;
                }
            }
        }

        public bool IsBoxBorder
        {
            get { return this.m_bIsBoxBorder; }
            set 
            {
                this.m_bIsNoBorder = !value;
                this.m_bIsBoxBorder = value;
                this.m_bIsFullBorder = !value;
                this.NotifyPropertyChangedBorders();
                if (value)
                {
                    this.TableType = TableType.BoxBorder;
                }
            }
        }

        public bool IsFullBorder
        {
            get { return this.m_bIsFullBorder; }
            set 
            {
                this.m_bIsNoBorder = !value;
                this.m_bIsBoxBorder = !value;
                this.m_bIsFullBorder = value;
                this.NotifyPropertyChangedBorders();
                if (value)
                {
                    this.TableType = TableType.FullBorder;
                }
            }
        }

        /// <summary>
        /// Get bind image
        /// </summary>
        public Table Table
        {
            get { return this.m_table; }
            set { this.m_table = value; }
        }

        public int Rows
        {
            get { return this.m_iRows; }
            set 
            { 
                this.m_iRows = value;
                this.NotifyPropertyChanged("Rows");
            }
        }

        public int Columns
        {
            get { return this.m_iColumns; }
            set 
            { 
                this.m_iColumns = value;
                this.NotifyPropertyChanged("Columns");
            }
        }

        public Brush TableBorderBrush
        {
            get { return m_borderBrush; }
            set { m_borderBrush = value; }
        }

        public Thickness TableBorderThickness
        {
            get { return this.m_borderThickness; }
            set { this.m_borderThickness = value; }
        }

        #endregion //| TablePropertiesDialog interface |

        #region | Private stuff |
        private void NotifyPropertyChangedBorders()
        {
            this.NotifyPropertyChanged("IsFullBorder");
            this.NotifyPropertyChanged("IsBoxBorder");
            this.NotifyPropertyChanged("IsNoBorder");
            this.m_comboBorderColor.IsEnabled = this.m_comboBorderWidth.IsEnabled = !this.m_bIsNoBorder;
        }

        /// <summary>
        /// Bind DataContext
        /// </summary>
        private void BindTable(Table table)
        {
            this.DataContext = this;
            this.m_tableBackup = table;
            this.m_table = table;

            if (null == table)
            {
                return;
            }

            if (0 >= table.Columns.Count)
            {
                this.IsCellWidthAutomatic = true;
            }
            else
            {
                bool bIsAutomatic = false;
                foreach (TableColumn tableColumn in table.Columns)
                {
                    if (tableColumn.Width == GridLength.Auto)
                    {
                        bIsAutomatic = true;
                        break;
                    }
                }
                this.IsCellWidthAutomatic = bIsAutomatic;
            }
        }

        /// <summary>
        /// Cancell button click handler
        /// </summary>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// OK button click handler
        /// </summary>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //### Set last selected border width
                ComboBoxShapeInfo comboBoxShapeInfo = this.m_comboBorderWidth.SelectedItem as ComboBoxShapeInfo;
                if (null != comboBoxShapeInfo)
                {
                    double dLeft = comboBoxShapeInfo.LineWidth;
                    double dTop = comboBoxShapeInfo.LineWidth;
                    double dRight = comboBoxShapeInfo.LineWidth;
                    double dBottom = comboBoxShapeInfo.LineWidth;

                    if (this.IsNoBorder)
                    {
                        dLeft = dTop = dRight = dBottom = 0.0;
                    }
                    else if (this.IsBoxBorder)
                    {
                    }

                    this.TableBorderThickness = new Thickness(dLeft, dTop, dRight, dBottom);
                }

                //### Set last selected border color
                int iSelIndex = this.m_comboBorderColor.SelectedIndex;
                this.TableBorderBrush = new SolidColorBrush(mst_colorArray[-1 == iSelIndex ? 0 : iSelIndex]);

                if (null != this.Table)
                {
                    this.Table.BorderBrush = this.TableBorderBrush;
                    this.Table.BorderThickness = this.TableBorderThickness;
                    this.Table.Tag = this.TableType;

                    foreach (TableRowGroup rowGroup in this.Table.RowGroups)
                    {
                        foreach (TableRow row in rowGroup.Rows)
                        {
                            foreach (TableCell cell in row.Cells)
                            {
                                cell.BorderBrush = this.TableBorderBrush;
                                cell.BorderThickness = this.TableBorderThickness;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex);
                Debug.Assert(false,"Exception: " + ex);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void AddComboCollor(Color color)
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            Rectangle colorRect = new Rectangle();
            colorRect.Margin = new Thickness(1, 1, 1, 1);
            colorRect.Width = 80;
            colorRect.Height = 10;
            colorRect.Fill = solidColorBrush;
            colorRect.Stroke = Brushes.DarkGray;
            colorRect.StrokeThickness = 1;
            colorRect.Tag = color;
            this.m_comboBackColor.Items.Add(colorRect);

            Rectangle colorItemRect = colorRect.Clone();
            colorItemRect.Width = 80;
            colorItemRect.Height = 10;
            this.m_comboBorderColor.Items.Add(colorItemRect);
        }

        private void AddComboBorderWidth(double dLineWidth)
        {
            ComboBoxShapeInfo comboBoxShapeInfo = new ComboBoxShapeInfo(String.Format("{0} ", dLineWidth), dLineWidth);

            this.m_comboBorderWidth.Items.Add(comboBoxShapeInfo);
        }

        public void InitUIControls()
        {
            //### Border width
            for (int i = 0; i < mst_doubleLineWidths.Length; i++)
            {
                this.AddComboBorderWidth(mst_doubleLineWidths[i]);
            }

            //### Border/background collor
            for (int i = 0; i < mst_colorArray.Length; i++)
            {
                this.AddComboCollor(mst_colorArray[i]);
            }

            if (null != this.Table)
            {
                this.Rows = this.Table.RowGroups[0].Rows.Count;
                this.Columns = this.Table.RowGroups[0].Rows[0].Cells.Count;
                this.m_txBxRows.IsEnabled = this.m_txBxColumns.IsEnabled = false;

                bool bFound = false;

                if (null != this.Table.BorderBrush)
                {
                    for (int i = 0; i < mst_colorArray.Length; i++)
                    {
                        if (null != this.Table.BorderBrush && mst_colorArray[i] == ((SolidColorBrush)this.Table.BorderBrush).Color)
                        {
                            this.m_comboBorderColor.SelectedIndex = i;
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        this.AddComboCollor(((SolidColorBrush)this.Table.BorderBrush).Color);
                    }
                }

                if (null != this.Table.BorderThickness && 0 < this.Table.BorderThickness.Left)
                {
                    bFound = false;
                    for (int i = 0; i < mst_doubleLineWidths.Length; i++)
                    {
                        if (mst_doubleLineWidths[i] == this.Table.BorderThickness.Left)
                        {
                            this.m_comboBorderWidth.SelectedIndex = i;
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        this.AddComboBorderWidth(this.Table.BorderThickness.Left);
                    }
                }

                TableType tableType = (null != this.Table.Tag ? (TableType)this.Table.Tag : TableType.BoxBorder);
                switch(tableType)
                {
                    case TableType.BoxBorder:
                        this.IsBoxBorder = true;
                        break;

                    case TableType.FullBorder:
                        this.IsFullBorder = true;
                        break;

                    case TableType.NoneBorder:
                        this.IsNoBorder = true;
                        break;

                    default:
                        Debug.Assert(false, "Unexpected table type!");
                        break;
                }
                
                bool bIsAuto = false;
                foreach (TableColumn tableColumn in this.Table.Columns)
                {
                    if (GridLength.Auto == tableColumn.Width)
                    {
                        bIsAuto = true;
                        break;
                    }
                }
                if (!bIsAuto && 0 < this.Table.Columns.Count)
                {
                    this.CellWidth = this.Table.Columns[0].Width.Value;
                    this.IsCellWidthAutomatic = false;
                }
            }
            else
            {
                this.m_comboBorderWidth.SelectedIndex = mst_iDefaultLineWidthIdx;
                this.m_comboBorderColor.SelectedIndex = mst_iDefaultBlackColorIdx;
                this.IsFullBorder = true;
            }
        }


        #endregion //| Private stuff |

    }
    public class ComboBoxShapeInfo
    {
        public string WidthInfo { get; set; }
        public Line Shape { get; set; }
        public double LineWidth { get; set; }

        public ComboBoxShapeInfo(string strInfo,double dLineWidth)
        {
            this.WidthInfo = strInfo;
            this.LineWidth = dLineWidth;

            this.Shape = new Line();
            this.Shape.Stroke = System.Windows.Media.Brushes.Black;
            this.Shape.X1 = 1;
            this.Shape.X2 = 80;
            this.Shape.Y1 = 1;
            this.Shape.Y2 = 1;
            this.Shape.HorizontalAlignment = HorizontalAlignment.Left;
            this.Shape.VerticalAlignment = VerticalAlignment.Center;
            this.Shape.StrokeThickness = dLineWidth;
        }
    }

    public static class Extensions
    {
        public static Rectangle Clone(this Rectangle rectVal)
        {
            Rectangle rectCloned = new Rectangle();
            rectCloned.Margin = rectVal.Margin;
            rectCloned.Width = rectVal.Width;
            rectCloned.Height = rectVal.Height;
            rectCloned.Fill = rectVal.Fill;
            rectCloned.Stroke = rectVal.Stroke;
            rectCloned.StrokeThickness = rectVal.StrokeThickness;
            return rectCloned;
        }
    }
}
