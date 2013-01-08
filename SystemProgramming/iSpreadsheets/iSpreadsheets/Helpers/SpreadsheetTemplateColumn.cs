using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DataGridCell = ExtendedGrid.Microsoft.Windows.Controls.DataGridCell;
using DataGridTemplateColumn = ExtendedGrid.Microsoft.Windows.Controls.DataGridTemplateColumn;

namespace iSpreadsheets.Helpers
{
    public class SpreadsheetTemplateColumn : DataGridTemplateColumn
    {
        public string ColumnName { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // The DataGridTemplateColumn uses ContentPresenter with your DataTemplate.
            ContentPresenter cp = (ContentPresenter)base.GenerateElement(cell, dataItem);
            // Reset the Binding to the specific column. The default binding is to the DataRowView.
            BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding(this.ColumnName));

            return cp;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter cp = (ContentPresenter)base.GenerateEditingElement(cell, dataItem);
            BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding(this.ColumnName));
            return cp;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            SpreadsheetCell data = editingElement.DataContext as SpreadsheetCell;
            string uneditedText = string.Empty;
            if (data != null )
            {
                uneditedText = data.Content;
                data.Content = data.Tag.Formula == string.Empty ? uneditedText : data.Tag.Formula;
            }

            // move focus to text field and select all text
            editingElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            var contentPresenter = editingElement as ContentPresenter;
            var editingControl = ExtendedDataGridExtensions.GetVisualChild<TextBox>(contentPresenter);
            editingControl.SelectAll();

            return uneditedText;
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            SpreadsheetCell data = editingElement.DataContext as SpreadsheetCell;
            if (data != null)
            {
                data.Content = data.Tag.Value;
            }
        }

        protected override bool CommitCellEdit(FrameworkElement editingElement)
        {
            SpreadsheetCell data = editingElement.DataContext as SpreadsheetCell;
            if (data != null)
            {
                data.UpdateAllDependentCells();

                var formula = data.Content;
                data.Tag.Calculate(formula, data.Table);
                data.Content = data.Tag.Value;

                data.UpdateAllConsequentialCells();
            }

            return base.CommitCellEdit(editingElement);
        }

        public override object OnCopyingCellClipboardContent(object item)
        {
            return base.OnCopyingCellClipboardContent(item);
        }

        public override void OnPastingCellClipboardContent(object item, object cellContent)
        {
            base.OnPastingCellClipboardContent(item, cellContent);
        }
    }
}
