using System.Windows;
using System.Windows.Media;
using ExtendedGrid.ExtendedGridControl;
using ExtendedGrid.Microsoft.Windows.Controls.Primitives;
using DataGrid = ExtendedGrid.Microsoft.Windows.Controls.DataGrid;
using DataGridCell = ExtendedGrid.Microsoft.Windows.Controls.DataGridCell;
using DataGridCellInfo = ExtendedGrid.Microsoft.Windows.Controls.DataGridCellInfo;
using DataGridRow = ExtendedGrid.Microsoft.Windows.Controls.DataGridRow;
using DataGridSelectionUnit = ExtendedGrid.Microsoft.Windows.Controls.DataGridSelectionUnit;

namespace iSpreadsheets.Helpers
{
    public static class ExtendedDataGridExtensions
    {
        public static void FillAttributesForSpreedsheetView(this ExtendedDataGrid dataGrid)
        {
            // MUST set EnableRowVirtualization to false for correct row numbers showing
            // and EnableColumnVirtualization to false for correct work
            dataGrid.EnableRowVirtualization = false;
            dataGrid.EnableColumnVirtualization = false;


            // Prevent unnecessary user actions
            dataGrid.CanUserReorderColumns = false;
            dataGrid.CanUserReorderRows = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserDeleteRows = false;
            dataGrid.AllowUserToCopy = false;

            // Some dark theming
            dataGrid.Theme = ExtendedDataGrid.Themes.LiveExplorer;


            // Select only one cell per click, or row if clicked on row header
            dataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            //dataGrid.CanUserSortColumns = false;

            // Columns and rows default size
            dataGrid.MinColumnWidth = 50;
            dataGrid.RowHeaderWidth = 30;
            dataGrid.MinRowHeight = 25;

            // Hide annoying label
            dataGrid.GroupByControlVisibility = Visibility.Collapsed;
        }

        #region Find cell by its cellinfo

        public static DataGridCell TryToFindGridCell(this ExtendedDataGrid dataGrid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = dataGrid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return result;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        #endregion

        #region Select cell by row and column index

        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }

        #endregion
        
    }
}
