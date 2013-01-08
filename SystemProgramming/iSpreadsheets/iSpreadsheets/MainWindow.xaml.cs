using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ExtendedGrid.Classes;
using iSpreadsheets.Helpers;
using ExtendedGrid.ExtendedGridControl;
using DataGridAutoGeneratingColumnEventArgs = ExtendedGrid.Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs;
using SelectedCellsChangedEventArgs = ExtendedGrid.Microsoft.Windows.Controls.SelectedCellsChangedEventArgs;

namespace iSpreadsheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Total number of sheets, also counts deleted sheets
        /// </summary>
        private int NumberOfSheets { get; set; }

        private List<DataTable> DataTables { get; set; }

        private int CurrentSheetNumber { get { return WorkspaceTabControl.SelectedIndex; } }

        public MainWindow()
        {
            InitializeComponent();
            Logger.LogBox = this.logRichTextBox;
            this.SetDefaultCulture();
            this.ResizeTableDialog.SetParent(this.spreadSheetGrid);

            this.DataTables = new List<DataTable>();
            this.CreateSheet(this.WorkDataTable0, this.Sheet0TabItem);
        }

        #region Useful methods

        /// <summary>
        /// Adds event handlers for datagrid and binds table to datagrid
        /// Also increases number of sheets
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="sheetTabItem"></param>
        /// <param name="bindTable"></param>
        private void CreateSheet(ExtendedDataGrid dataGrid, TabItem sheetTabItem, DataTable bindTable = null)
        {
            dataGrid.FillAttributesForSpreedsheetView();
            dataGrid.SelectedCellsChanged += CoordinatesOfSelectedCells_OnSelectedCellsChanged;
            dataGrid.LoadingRow += NumerateRows_OnLoadRows;
            dataGrid.AutoGeneratingColumn += SpreadsheetCell_AutoGeneratingColumn;

            if (bindTable == null)
            {
                FillDefaultData(dataGrid);
            }
            else
            {
                dataGrid.ItemsSource = bindTable.DefaultView;
            }

            sheetTabItem.Content = dataGrid;

            this.NumberOfSheets++;
        }

        private void FillDefaultData(ExtendedDataGrid exGrid)
        {
            DataTable dataTable = new DataTable("DataTable" + this.NumberOfSheets);
            int rowsNum = 17;
            int colsNum = 13;

            for (int i = 0; i < colsNum; i++)
            {
                dataTable.Columns.Add(new DataColumn(SSColumns.ToString(i), typeof(SpreadsheetCell)));
            }

            for (int i = 0; i < rowsNum; i++)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
                for (int j = 0; j < colsNum; j++)
                {
                    string cellName = SSColumns.ToString(j) + i;
                    row[j] = new SpreadsheetCell(string.Empty, dataTable, new SpreadsheetCellCustomInfo(new CellName(cellName, i, j)));
                }
            }

            this.DataTables.Add(dataTable);
            exGrid.ItemsSource = dataTable.DefaultView;
        }

        /// <summary>
        /// Generates tabitem, datagrid, and binds table to datagrid
        /// </summary>
        /// <param name="bindTable"></param>
        private void GenerateSheet(DataTable bindTable = null)
        {
            TabItem sheetTabItem = new TabItem { Header = "Sheet" + this.NumberOfSheets, Style = (Style)FindResource("BlueAndOrange") };
            this.WorkspaceTabControl.Items.Add(sheetTabItem);

            ExtendedDataGrid dataGrid = new ExtendedDataGrid { Name = "WorkDataTable" + this.NumberOfSheets };

            this.CreateSheet(dataGrid, sheetTabItem, bindTable);
        }

        /// <summary>
        /// Resize table by creating new and copying old values
        /// </summary>
        /// <param name="exGrid"></param>
        /// <param name="oldDataTable"></param>
        /// <param name="rowsNum"></param>
        /// <param name="colsNum"></param>
        private void ResizeTable(ExtendedDataGrid exGrid, DataTable oldDataTable, int rowsNum, int colsNum)
        {
            int boundRows = oldDataTable.Rows.Count < rowsNum ? oldDataTable.Rows.Count : rowsNum;
            int boundCols = oldDataTable.Columns.Count < colsNum ? oldDataTable.Columns.Count : colsNum;

            DataTable newDataTable = new DataTable(oldDataTable.TableName);

            for (int i = 0; i < colsNum; i++)
            {
                newDataTable.Columns.Add(new DataColumn(SSColumns.ToString(i), typeof(SpreadsheetCell)));
            }

            for (int i = 0; i < rowsNum; i++)
            {
                var row = newDataTable.NewRow();
                newDataTable.Rows.Add(row);
                for (int j = 0; j < colsNum; j++)
                {
                    string cellName = SSColumns.ToString(j) + i;
                    row[j] = new SpreadsheetCell(string.Empty, newDataTable, new SpreadsheetCellCustomInfo(new CellName(cellName, i, j)));
                }
            }

            // copy data
            for (int i = 0; i < boundRows; i++)
            {
                for (int j = 0; j < boundCols; j++)
                {
                    SpreadsheetCell cell = newDataTable.GetSpreadsheetCell(i, j);
                    SpreadsheetCell oldCell = oldDataTable.GetSpreadsheetCell(i, j);
                    cell.Content = oldCell.Content;
                    cell.Tag = oldCell.Tag;
                }
            }
            this.DataTables[this.CurrentSheetNumber] = newDataTable;
            
            // VERY VERY DIRTY HACK because nulling throws exception, dunno why
            try
            {
                exGrid.ItemsSource = null;
            }
            catch { }
            
            exGrid.ItemsSource = newDataTable.DefaultView;
        }

        /// <summary>
        /// Displays additional info: current cell name and current cell formula
        /// </summary>
        /// <param name="dataGrid"></param>
        private void DisplayAdditionalInfo(ExtendedDataGrid dataGrid)
        {
            var currentCellInfo = dataGrid.CurrentCell;
            var currentCell = dataGrid.TryToFindGridCell(currentCellInfo);

            var contentPresenter = currentCell.Content as ContentPresenter;
            if (contentPresenter != null)
            {
                var spreadsheetCell = contentPresenter.Content as SpreadsheetCell;
                if (spreadsheetCell != null)
                {
                    var cellInfo = spreadsheetCell.Tag;

                    this.CurrentCellFormulaTextBox.Text = cellInfo.Formula;
                    this.CurrentCellCoordinatesTextBox.Text = cellInfo.CellName.FullName;
                }
            }
        }

        /// <summary>
        /// Set default culture as en-US 
        /// </summary>
        private void SetDefaultCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
            XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        /// <summary>
        /// Gets sum of selected cells content
        /// </summary>
        /// <param name="chartBy"></param>
        /// <returns></returns>
        private Dictionary<string, double> GetChartDataOfSelectedCells(ChartBy chartBy)
        {
            Dictionary<string, double> drawChart = new Dictionary<string, double>();

            var dataGrid = (ExtendedDataGrid)((TabItem)WorkspaceTabControl.Items[this.CurrentSheetNumber]).Content;

            foreach (var cellInfo in dataGrid.SelectedCells)
            {
                // this changes the cell's content not the data item behind it
                var gridCell = dataGrid.TryToFindGridCell(cellInfo);
                if (gridCell != null)
                {
                    string columnHeader = gridCell.Column.Header.ToString();
                    int rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);

                    string coord = chartBy == ChartBy.Rows ? rowIndex.ToString() : columnHeader;

                    var cell = this.DataTables[this.CurrentSheetNumber].GetSpreadsheetCell(rowIndex, SSColumns.Parse(columnHeader));
                    if (!drawChart.ContainsKey(coord))
                    {
                        drawChart.Add(coord, 0);
                    }

                    if (cell.Content == string.Empty)
                    {
                        continue;
                    }

                    double doubleVal;
                    if (double.TryParse(cell.Content, out doubleVal))
                    {
                        drawChart[coord] += doubleVal;
                    }
                    else
                    {
                        throw new Exception("Some of selected cells contains non-numeric values!");
                    }
                }
            }

            if (drawChart.Count == 0)
            {
                throw new Exception("Please select cells to graph from!");
            }

            return drawChart;
        }

        #endregion

        #region Events

        /// <summary>
        /// Numerate rows in ascending order, from 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumerateRows_OnLoadRows(object sender, ExtendedGrid.Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex();
        }

        /// <summary>
        /// Displays selected sells coordinates on status bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedCellsChangedEventArgs"></param>
        private void CoordinatesOfSelectedCells_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs selectedCellsChangedEventArgs)
        {
            try
            {
                var dataGrid = (ExtendedDataGrid)sender;

                StringBuilder selectedCellsCoordinates = new StringBuilder();
                selectedCellsCoordinates.Append("Selected cells: ");

                foreach (var cellInfo in dataGrid.SelectedCells)
                {
                    // this changes the cell's content not the data item behind it
                    var gridCell = dataGrid.TryToFindGridCell(cellInfo);
                    if (gridCell != null)
                    {
                        string columnHeader = gridCell.Column.Header.ToString();
                        int rowIndex = dataGrid.Items.IndexOf(cellInfo.Item);

                        selectedCellsCoordinates.Append(string.Format("[{0},{1}] ", columnHeader, rowIndex));
                    }
                }

                this.DisplayAdditionalInfo(dataGrid);

                StatusBarTextBox.Text = selectedCellsCoordinates.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name + ": " + ex.Message, "Error occured during selecting cells", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Replaces standard method of auto generating columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetCell_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                if (e.PropertyType == typeof(SpreadsheetCell))
                {
                    var col = new SpreadsheetTemplateColumn
                        {
                            ColumnName = e.PropertyName,
                            CellTemplate = (DataTemplate)FindResource("CellTemplate"),
                            CellEditingTemplate = (DataTemplate)FindResource("CellEditingTemplate")
                        };
                    e.Column = col;
                    e.Column.Header = e.PropertyName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during autogenerating column", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }


        /// <summary>
        /// Clears all data and creates one default sheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewDocumentButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("If you create new file, all sheets will be deleted and data will be lost.",
                                "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    // remove all current sheets
                    this.WorkspaceTabControl.Items.Clear();
                    this.DataTables.Clear();
                    this.NumberOfSheets = 0;
                    this.GenerateSheet();
                    Logger.WriteLogInfo(DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " | >>> " + "All data cleared, new document created! Please wait for UI loading!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error occured during creating new document", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Saves all current data tables using binary serializer to *.ispf file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileButton_OnClick(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".ispdf";
            dlg.Filter = "iSpreadsheet document format (.ispf)|*.ispf";
            dlg.FileName = "iDocument";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        formatter.Serialize(stream, this.DataTables);
                    }
                    Logger.WriteLogInfo(DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff") + " | >>> " + "File \"" + filename + "\" was saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error occured during saving file", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
                }
            }

        }

        /// <summary>
        /// Loads data tables from file *.ispf using binary deserializer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".ispf";
            dlg.Filter = "iSpreadsheet document format (.ispf)|*.ispf";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        this.DataTables = (List<DataTable>)formatter.Deserialize(stream);
                        this.WorkspaceTabControl.Items.Clear();
                        this.NumberOfSheets = 0;

                        // Update cell parent table referencing, cause it's not serializable
                        foreach (var dataTable in this.DataTables)
                        {
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                for (int j = 0; j < dataTable.Columns.Count; j++)
                                {
                                    SpreadsheetCell cell = dataTable.GetSpreadsheetCell(i, j);
                                    cell.Table = dataTable;
                                }
                            }

                            this.GenerateSheet(dataTable);
                        }

                    }
                    Logger.WriteLogInfo("File \"" + filename + "\" was loaded successfully! Please wait for UI loading!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error occured during saving file", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
                }
            }

        }

        /// <summary>
        /// Generates new sheet: new tabitem, new datatable, new datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSheetButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.GenerateSheet();
                Logger.WriteLogInfo("Successfully added new Sheet" + (this.NumberOfSheets - 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during adding new sheet", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Removes currently selected sheet: removes tabitem with its content and removes corresponding data table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveCurrentSheetButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("If you remove this sheet, all stored data will be lost.",
                                                      "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    if (WorkspaceTabControl.Items.Count > 0)
                    {
                        int indexOfSheet = this.CurrentSheetNumber;
                        this.WorkspaceTabControl.Items.RemoveAt(indexOfSheet);
                        this.DataTables.RemoveAt(indexOfSheet);
                        Logger.WriteLogInfo("Successfully deleted Sheet" + indexOfSheet);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error occured during removing current sheet", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Resize current table: create new with new dimensions and copy old values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeTableButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentDataGrid = (ExtendedDataGrid)((TabItem)WorkspaceTabControl.Items[this.CurrentSheetNumber]).Content;
                var currentDataTable = this.DataTables[this.CurrentSheetNumber];

                bool dialogResult = this.ResizeTableDialog.ShowHandlerDialog(currentDataTable.Rows.Count, currentDataTable.Columns.Count);
                if (dialogResult == true)
                {
                    int newRows = int.Parse(this.ResizeTableDialog.RowsTextBox.Text);
                    int newCols = int.Parse(this.ResizeTableDialog.ColsTextBox.Text);

                    this.ResizeTable(currentDataGrid, currentDataTable, newRows, newCols);
                    Logger.WriteLogInfo("Successfully resized Data Table. Please wait for UI loading!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during resizing table", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Shows column chart in separate window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnChartButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ChartBy chartBy = this.ByRows.IsChecked == true ? ChartBy.Rows : ChartBy.Cols;
                Dictionary<string, double> chartData = this.GetChartDataOfSelectedCells(chartBy);

                ColumnChart window = new ColumnChart();
                window.SetDataSumValuesList(chartData, chartBy);
                window.Show();

                Logger.WriteLogInfo(string.Format("Successfully drawed Column chart by {0} of selected cells",
                                                  (chartBy == ChartBy.Rows ? "rows" : "columns")));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name + ": " + ex.Message, "Error occured during drawing graph", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Shows bar chart in separate window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarChart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ChartBy chartBy = this.ByRows.IsChecked == true ? ChartBy.Rows : ChartBy.Cols;
                Dictionary<string, double> chartData = this.GetChartDataOfSelectedCells(chartBy);

                BarChart window = new BarChart();
                window.SetDataSumValuesList(chartData, chartBy);
                window.Show();

                Logger.WriteLogInfo(string.Format("Successfully drawed Bar chart by {0} of selected cells",
                                                  (chartBy == ChartBy.Rows ? "rows" : "columns")));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name + ": " + ex.Message, "Error occured during drawing graph", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }


        private void ExportToCsv_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".csv";
                dlg.Filter = "Comma separated values (.csv)|*.csv";
                dlg.FileName = "WorkSheet" + this.CurrentSheetNumber;
                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;

                    // DUMB way to select current grid
                    var currentDataGrid = (ExtendedDataGrid)((TabItem)WorkspaceTabControl.Items[this.CurrentSheetNumber]).Content;
                    currentDataGrid.ExportToCsv("WorkSheet" + this.CurrentSheetNumber, filename, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during exporting to csv", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToXls_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".xls";
                dlg.Filter = "Excel format (.xls)|*.xls";
                dlg.FileName = "WorkSheet" + this.CurrentSheetNumber;
                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;

                    // DUMB way to select current grid
                    var currentDataGrid = (ExtendedDataGrid)((TabItem)WorkspaceTabControl.Items[this.CurrentSheetNumber]).Content;
                    currentDataGrid.ExportToExcel("WorkSheet" + this.CurrentSheetNumber, filename, ExcelTableStyle.Medium14, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during exporting to excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToPdf_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "Portable document format (.pdf)|*.pdf";
                dlg.FileName = "WorkSheet" + this.CurrentSheetNumber;
                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;

                    // DUMB way to select current grid
                    var currentDataGrid = (ExtendedDataGrid)((TabItem)WorkspaceTabControl.Items[this.CurrentSheetNumber]).Content;
                    currentDataGrid.ExportToPdf("WorkSheet" + this.CurrentSheetNumber, filename, ExcelTableStyle.Medium14, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error occured during exporting to pdf", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Simple exit dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure want to exit, all sheets will be deleted and data will be lost.",
                               "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        #endregion

        private void PieChart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ChartBy chartBy = this.ByRows.IsChecked == true ? ChartBy.Rows : ChartBy.Cols;
                Dictionary<string, double> chartData = this.GetChartDataOfSelectedCells(chartBy);

                bool allZeros = true;
                foreach (var d in chartData)
                {
                    if (d.Value != 0)
                    {
                        allZeros = false;
                    }
                }
                if (allZeros)
                {
                    throw new Exception("Pie chart cannot be drawn if all cell values are equal to 0 or is empty");
                }

                PieChart window = new PieChart();
                window.GenerateDataSeries(chartData, chartBy);
                window.Show();

                string chartType = chartBy == ChartBy.Rows ? "rows" : "columns";
                Logger.WriteLogInfo(string.Format("Successfully drawed pie chart by {0} of selected cells", chartType));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name + ": " + ex.Message, "Error occured during drawing graph", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
            }
        }
    }
}
