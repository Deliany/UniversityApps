using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Diagnostics;

namespace Adastra.RichEditorLibrary
{
    internal static class TableDragHandle
    {
        private static Table m_tblLastEntered = null;
        private static Point? m_dragStartPoint = null;
        private static double m_dInitialWidth = double.NaN;
        private static int m_iRowIdx = -1;

        public static void Reset()
        {
            TableDragHandle.LastEnteredObjectTable = null;
            TableDragHandle.DragStartPoint = null;
            TableDragHandle.InitialCellWidth = double.NaN;
            TableDragHandle.RowIdx = -1;
        }

        public static bool IsHandleValid
        {
            get
            {
                bool bRetVal =
                (null != TableDragHandle.LastEnteredObjectTable &&
                 null != TableDragHandle.DragStartPoint &&
                 !double.IsNaN(TableDragHandle.InitialCellWidth) &&
                 -1 != TableDragHandle.RowIdx);
                //Debug.WriteLine("IsHandleValid: " + bRetVal);
                return bRetVal;
            }
        }

        public static int RowIdx
        {
            get { return TableDragHandle.m_iRowIdx; }
            set { TableDragHandle.m_iRowIdx = value; }
        }

        public static double InitialCellWidth
        {
            get { return TableDragHandle.m_dInitialWidth; }
            set { TableDragHandle.m_dInitialWidth = value; }
        }

        public static Table LastEnteredObjectTable
        {
            get { return TableDragHandle.m_tblLastEntered; }
            set { TableDragHandle.m_tblLastEntered = value; }
        }

        public static Point? DragStartPoint
        {
            get { return TableDragHandle.m_dragStartPoint; }
            set { TableDragHandle.m_dragStartPoint = value; }
        }
    }
}
