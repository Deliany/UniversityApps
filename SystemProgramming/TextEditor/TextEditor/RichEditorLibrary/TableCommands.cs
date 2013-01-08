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
    /// Helper class for table commands exposed by RichTextEditor.
    /// </summary>
    internal class TableCommands
    {
        #region Private Fields

        private static RoutedCommand m_editTablePropertiesCommand;
        private static RoutedCommand m_insertTableCommand;
        private static RoutedCommand m_insertRowsAboveCommand;
        private static RoutedCommand m_insertRowsBelowCommand;
        private static RoutedCommand m_insertColumnsToRightCommand;
        private static RoutedCommand m_insertColumnsToLeftCommand;
        private static RoutedCommand m_deleteTableCommand;
        private static RoutedCommand m_deleteRowsCommand;
        private static RoutedCommand m_deleteColumnsCommand;

        #endregion

        static TableCommands()
        {
            m_editTablePropertiesCommand = new RoutedCommand("EditTableProperties", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor),
                new CommandBinding(m_editTablePropertiesCommand, OnEditTableProperties, OnCanExecuteTableCommand));

            m_insertTableCommand = new RoutedCommand("InsertTable", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_insertTableCommand, OnInsertTable, OnCanExecuteInsertTable));

            m_insertRowsAboveCommand = new RoutedCommand("InsertRowsAbove", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_insertRowsAboveCommand, OnInsertRowsAbove, OnCanExecuteTableCommand));

            m_insertRowsBelowCommand = new RoutedCommand("InsertRowsBelow", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_insertRowsBelowCommand, OnInsertRowsBelow, OnCanExecuteTableCommand));

            m_insertColumnsToRightCommand = new RoutedCommand("InsertColumnsToRight", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_insertColumnsToRightCommand, OnInsertColumnsToRight, OnCanExecuteTableCommand));

            m_insertColumnsToLeftCommand = new RoutedCommand("InsertColumnsToLeft", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_insertColumnsToLeftCommand, OnInsertColumnsToLeft, OnCanExecuteTableCommand));

            m_deleteTableCommand = new RoutedCommand("DeleteTable", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_deleteTableCommand, OnDeleteTable, OnCanExecuteTableCommand));

            m_deleteRowsCommand = new RoutedCommand("DeleteRows", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_deleteRowsCommand, OnDeleteRows, OnCanExecuteTableCommand));

            m_deleteColumnsCommand = new RoutedCommand("DeleteColumns", typeof(RichTextEditor));
            CommandManager.RegisterClassCommandBinding(typeof(RichTextEditor), 
                new CommandBinding(m_deleteColumnsCommand, OnDeleteColumns, OnCanExecuteTableCommand));
        }

        internal static RoutedCommand EditTablePropertiesCommand
        {
            get
            {
                return m_editTablePropertiesCommand;
            }
        }

        internal static RoutedCommand InsertTableCommand
        {
            get
            {
                return m_insertTableCommand;
            }
        }

        internal static RoutedCommand InsertRowsAboveCommand
        {
            get
            {
                return m_insertRowsAboveCommand;
            }
        }

        internal static RoutedCommand InsertRowsBelowCommand
        {
            get
            {
                return m_insertRowsBelowCommand;
            }
        }

        internal static RoutedCommand InsertColumnsToRightCommand
        {
            get
            {
                return m_insertColumnsToRightCommand;
            }
        }

        internal static RoutedCommand InsertColumnsToLeftCommand
        {
            get
            {
                return m_insertColumnsToLeftCommand;
            }
        }

        internal static RoutedCommand DeleteTableCommand
        {
            get
            {
                return m_deleteTableCommand;
            }
        }

        internal static RoutedCommand DeleteRowsCommand
        {
            get
            {
                return m_deleteRowsCommand;
            }
        }

        internal static RoutedCommand DeleteColumnsCommand
        {
            get
            {
                return m_deleteColumnsCommand;
            }
        }

        #region | TableCommands interface |

        public static void OnCanExecuteInsertTable(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            RichTextEditor control = (RichTextEditor)target;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;

            // Disable tables inside lists and hyperlinks
            if (Helper.HasAncestor(insertionPosition, typeof(List)) || Helper.HasAncestor(insertionPosition, typeof(Hyperlink)))
            {
                e.CanExecute = false;
            }
        }

        public static void OnCanExecuteTableCommand(object target, CanExecuteRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)target;
            RichTextBox richTextBox = control.RichTextBox;

            e.CanExecute = false;
            if (Helper.HasAncestor(richTextBox.Selection.Start, typeof(TableCell)))
            {
                e.CanExecute = true;
            }
        }

        public static void OnEditTableProperties(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer textPosition = richTextBox.Selection.Start;

            Table table = Helper.GetTableAncestor(textPosition);
            if (null != table)
            {
                TablePropertiesDialog tablePropertiesDialog = new TablePropertiesDialog(table);
                tablePropertiesDialog.ShowDialog();
                if (true == tablePropertiesDialog.DialogResult)
                {
                    Helper.UpdateTable(table,
                                        tablePropertiesDialog.Rows,
                                        tablePropertiesDialog.Columns,
                                        tablePropertiesDialog.TableBorderBrush,
                                        tablePropertiesDialog.TableBorderThickness,
                                        tablePropertiesDialog.TableCellWidth,
                                        tablePropertiesDialog.TableType);
                }
            }
        }

        public static void OnInsertTable(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.Text = String.Empty;
            }

            TextPointer insertionPosition = richTextBox.Selection.Start;
            Paragraph paragraph = insertionPosition.Paragraph;

            // Split current paragraph at insertion position
            insertionPosition = insertionPosition.InsertParagraphBreak();
            paragraph = insertionPosition.Paragraph;

            TablePropertiesDialog tablePropertiesDialog = new TablePropertiesDialog(null);
            tablePropertiesDialog.ShowDialog();
            if (true == tablePropertiesDialog.DialogResult)
            {
                //Table table = Helper.BuildTable(/*rows*/2, /*columns*/5);
                Table table = Helper.BuildTable(tablePropertiesDialog.Rows, 
                    tablePropertiesDialog.Columns, 
                    tablePropertiesDialog.TableBorderBrush, 
                    tablePropertiesDialog.TableBorderThickness,
                    tablePropertiesDialog.TableCellWidth,
                    tablePropertiesDialog.TableType);
                //table.LineHeight = tablePropertiesDialog.CellWidth;
                //table.Margin = new Thickness(30,30,30,30);
                paragraph.SiblingBlocks.InsertBefore(paragraph, table);
            }
        }

        public static void OnInsertRowsAbove(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer insertionPosition = richTextBox.Selection.Start;
            TableRow tableRow = Helper.GetTableRowAncestor(insertionPosition);
            if (tableRow == null)
            {
                return;
            }

            TableRowGroup tableRowGroup = tableRow.Parent as TableRowGroup;
            if (tableRowGroup == null)
            {
                return;
            }
            Table table = (Table)tableRowGroup.Parent;

            int rowIndex = tableRowGroup.Rows.IndexOf(tableRow);
            TableRow newTableRow = Helper.BuildTableRow(tableRow.Cells.Count,
                                                (null != table ? table.BorderBrush : System.Windows.Media.Brushes.Black),
                                                (null != table ? table.BorderThickness : new Thickness(0.5, 0.5, 0.5, 0.5)),
                                                double.NaN);
            tableRowGroup.Rows.Insert(rowIndex, newTableRow);
        }

        public static void OnInsertRowsBelow(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;
            
            TableRow tableRow = Helper.GetTableRowAncestor(insertionPosition);
            if (tableRow == null)
            {
                return;
            }

            TableRowGroup tableRowGroup = tableRow.Parent as TableRowGroup;
            if (tableRowGroup == null)
            {
                return;
            }
            Table table = (Table)tableRowGroup.Parent;

            int rowIndex = tableRowGroup.Rows.IndexOf(tableRow);
            TableRow newTableRow = Helper.BuildTableRow(tableRow.Cells.Count, 
                                                (null != table ? table.BorderBrush : System.Windows.Media.Brushes.Black),
                                                (null != table ? table.BorderThickness : new Thickness(0.5, 0.5, 0.5, 0.5)), 
                                                double.NaN);
            tableRowGroup.Rows.Insert(rowIndex + 1, newTableRow);
        }

        public static void OnInsertColumnsToRight(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;
            
            TableCell tableCell = Helper.GetTableCellAncestor(insertionPosition);
            if (tableCell == null)
            {
                return;
            }

            TableRow tableRow = tableCell.Parent as TableRow;
            if (tableRow == null)
            {
                return;
            }

            TableRowGroup tableRowGroup = tableRow.Parent as TableRowGroup;
            if (tableRowGroup == null)
            {
                return;
            }

            int columnIndex = tableRow.Cells.IndexOf(tableCell);
            using (richTextBox.DeclareChangeBlock())
            {
                foreach (TableRow row in tableRowGroup.Rows)
                {
                    TableCell newTableCell = Helper.BuildTableCell(tableCell.BorderBrush, tableCell.BorderThickness, double.NaN);
                    row.Cells.Insert(columnIndex + 1, newTableCell);
                }
            }
        }

        public static void OnInsertColumnsToLeft(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;

            TableCell tableCell = Helper.GetTableCellAncestor(insertionPosition);
            if (tableCell == null)
            {
                return;
            }

            TableRow tableRow = tableCell.Parent as TableRow;
            if (tableRow == null)
            {
                return;
            }

            TableRowGroup tableRowGroup = tableRow.Parent as TableRowGroup;
            if (tableRowGroup == null)
            {
                return;
            }

            int columnIndex = tableRow.Cells.IndexOf(tableCell);
            using (richTextBox.DeclareChangeBlock())
            {
                foreach (TableRow row in tableRowGroup.Rows)
                {
                    TableCell newTableCell = Helper.BuildTableCell(tableCell.BorderBrush, tableCell.BorderThickness, double.NaN);
                    row.Cells.Insert(columnIndex, newTableCell);
                }
            }
        }

        public static void OnDeleteTable(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;
            TextPointer insertionPosition = richTextBox.Selection.Start;

            Table table = Helper.GetTableAncestor(insertionPosition);
            if (table != null)
            {
                table.SiblingBlocks.Remove(table);
            }
        }

        public static void OnDeleteRows(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer selectionStartPosition = richTextBox.Selection.Start;
            TableRow startTableRow = Helper.GetTableRowAncestor(selectionStartPosition);
            if (startTableRow == null)
            {
                return;
            }

            TableRowGroup startTableRowGroup = startTableRow.Parent as TableRowGroup;
            if (startTableRowGroup == null)
            {
                return;
            }

            int startRowIndex = startTableRowGroup.Rows.IndexOf(startTableRow);
            int endRowIndex = startRowIndex;

            if (!richTextBox.Selection.IsEmpty)
            {
                TextPointer selectionEndPosition = richTextBox.Selection.End.GetNextInsertionPosition(LogicalDirection.Backward);
                TableRow endTableRow = Helper.GetTableRowAncestor(selectionEndPosition);
                if (endTableRow == null)
                {
                    return;
                }
                TableRowGroup endTableRowGroup = endTableRow.Parent as TableRowGroup;
                if (startTableRowGroup != endTableRowGroup)
                {
                    return;
                }
                endRowIndex = endTableRowGroup.Rows.IndexOf(endTableRow);
            }

            using (richTextBox.DeclareChangeBlock())
            {
                for (int i = startRowIndex; i <= endRowIndex; i++)
                {
                    startTableRowGroup.Rows.RemoveAt(i);
                }

                if (startTableRowGroup.Rows.Count == 0)
                {
                    Table table = startTableRowGroup.Parent as Table;
                    table.SiblingBlocks.Remove(table);
                }
            }
        }

        public static void OnDeleteColumns(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextEditor control = (RichTextEditor)sender;
            RichTextBox richTextBox = control.RichTextBox;

            TextPointer selectionStartPosition = richTextBox.Selection.Start;
            TableCell startTableCell = Helper.GetTableCellAncestor(selectionStartPosition);
            if (startTableCell == null)
            {
                return;
            }

            TableRow startTableRow = startTableCell.Parent as TableRow;
            if (startTableRow == null)
            {
                return;
            }

            TableRowGroup startTableRowGroup = startTableRow.Parent as TableRowGroup;
            if (startTableRowGroup == null)
            {
                return;
            }

            int startColumnIndex = startTableRow.Cells.IndexOf(startTableCell);
            int endColumnIndex = startColumnIndex;

            if (!richTextBox.Selection.IsEmpty)
            {
                TextPointer selectionEndPosition = richTextBox.Selection.End.GetNextInsertionPosition(LogicalDirection.Backward);
                TableCell endTableCell = Helper.GetTableCellAncestor(selectionEndPosition);
                if (endTableCell == null)
                {
                    return;
                }
                TableRow endTableRow = Helper.GetTableRowAncestor(selectionEndPosition);
                if (endTableRow == null)
                {
                    return;
                }
                TableRowGroup endTableRowGroup = endTableRow.Parent as TableRowGroup;
                if (startTableRowGroup != endTableRowGroup)
                {
                    return;
                }
                endColumnIndex = endTableRow.Cells.IndexOf(endTableCell);
            }

            using (richTextBox.DeclareChangeBlock())
            {
                for (int i = 0; i < startTableRowGroup.Rows.Count; i++)
                {
                    for (int j = startColumnIndex; j <= endColumnIndex; j++)
                    {
                        TableCellCollection cells = startTableRowGroup.Rows[i].Cells;
                        TableCell cellToDelete = cells[startColumnIndex];
                        cells.Remove(cellToDelete);
                    }
                }

                if (startTableRow.Cells.Count == 0)
                {
                    Table table = startTableRowGroup.Parent as Table;
                    table.SiblingBlocks.Remove(table);
                }
            }
        }

        #endregion

    }
}