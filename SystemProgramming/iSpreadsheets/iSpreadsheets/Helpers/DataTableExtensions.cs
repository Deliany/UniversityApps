using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iSpreadsheets.Helpers
{
    public static class DataTableExtensions
    {
        public static SpreadsheetCell GetSpreadsheetCell(this DataTable dataTable, int rowNum, int colNum)
        {
            int rowsCount = dataTable.Rows.Count;
            int colsCount = dataTable.Columns.Count;

            // Instead of throwing IndexOutOfRange exception, just return null
            if ((colNum < 0 || colNum > colsCount) || (rowNum < 0 || rowNum > rowsCount))
            {
                return null;
            }

            SpreadsheetCell cell = dataTable.Rows[rowNum][colNum] as SpreadsheetCell;
            return cell;
        }
    }
}
