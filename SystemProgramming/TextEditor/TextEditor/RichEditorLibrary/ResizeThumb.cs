using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;

namespace Adastra.RichEditorLibrary
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            base.DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }

        void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //DesignerItem designerItem = this.DataContext as DesignerItem;
            //DesignerCanvas designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;

            //if (designerItem != null && designer != null && designerItem.IsSelected)
            //{
            //    double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
            //    double dragDeltaVertical, dragDeltaHorizontal;

            //    // only resize DesignerItems
            //    IEnumerable<DesignerItem> selectedDesignerItems = designer.SelectionService.CurrentSelection.OfType<DesignerItem>();
            //    //var selectedDesignerItems = from item in designer.SelectedItems
            //    //                            where item is DesignerItem
            //    //                            select item;

            //    CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop,
            //                        out minDeltaHorizontal, out minDeltaVertical);

            //    foreach (DesignerItem item in selectedDesignerItems)
            //    {
            //        if (item != null)
            //        {
            //            switch (base.VerticalAlignment)
            //            {
            //                case VerticalAlignment.Bottom:
            //                    dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
            //                    item.Height = item.ActualHeight - dragDeltaVertical;
            //                    break;
            //                case VerticalAlignment.Top:
            //                    double top = Canvas.GetTop(item);
            //                    dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
            //                    Canvas.SetTop(item, top + dragDeltaVertical);
            //                    item.Height = item.ActualHeight - dragDeltaVertical;
            //                    break;
            //                default:
            //                    break;
            //            }

            //            switch (base.HorizontalAlignment)
            //            {
            //                case HorizontalAlignment.Left:
            //                    double left = Canvas.GetLeft(item);
            //                    dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
            //                    Canvas.SetLeft(item, left + dragDeltaHorizontal);
            //                    item.Width = item.ActualWidth - dragDeltaHorizontal;
            //                    break;
            //                case HorizontalAlignment.Right:
            //                    dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
            //                    item.Width = item.ActualWidth - dragDeltaHorizontal;
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }
            //    }
            //    e.Handled = true;
            //}
        }

        //private void CalculateDragLimits(IEnumerable<DesignerItem> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        //{
            //minLeft = double.MaxValue;
            //minTop = double.MaxValue;
            //minDeltaHorizontal = double.MaxValue;
            //minDeltaVertical = double.MaxValue;

            //// drag limits are set by these parameters: canvas top, canvas left, minHeight, minWidth
            //// calculate min value for each parameter for each item
            //foreach (DesignerItem item in selectedItems)
            //{
            //    double left = Canvas.GetLeft(item);
            //    double top = Canvas.GetTop(item);

            //    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
            //    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

            //    minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
            //    minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
            //}
        //}
    }
}
