using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace iDraw
{
    public class DrawingArea
    {
        #region Private Variables

        private Canvas _canvas;
        ToolHelper toolHelper = new ToolHelper();
        List<Shape> tempHolder = new List<Shape>();
        Line _virtualLine = new Line() { Visibility = Visibility.Collapsed, Stroke = new SolidColorBrush(Colors.LightGray), StrokeThickness = 2 };
        CurrentTool tool = CurrentTool.Pencil;

        private List<Shape> redoList;
        private List<Shape> UndoList;
        private Dictionary<Shape, List<Shape>> UnShapeDrawingItems = new Dictionary<Shape, List<Shape>>();
        private int redoTop;
        private int undoTop;

        #endregion

        #region Properties


        /// <summary>
        /// Current tool to perform draw
        /// </summary>
        public CurrentTool Tool
        {
            get { return tool; }
            set { tool = value; }
        }


        /// <summary>
        /// Temporary holder to hold list of objects
        /// </summary>
        public List<Shape> TempHolder
        {
            get { return tempHolder; }
            set { tempHolder = value; }
        }


        /// <summary>
        /// Previous point
        /// </summary>
        public Point PrevPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Starting Point
        /// </summary>
        public Point StartPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Stroke color
        /// </summary>
        public SolidColorBrush StrokeColor { get; set; }

        /// <summary>
        /// Stroke width
        /// </summary>
        public double StrokeWidth { get; set; }

        /// <summary>
        /// Filling color
        /// </summary>
        public SolidColorBrush FillColor { get; set; }

        #endregion

        #region Ctor and Methods

        /// <summary>
        /// Drawing area constructor
        /// </summary>
        /// <param name="c"></param>
        public DrawingArea(Canvas c)
        {
            _canvas = c;
            _canvas.Children.Add(_virtualLine);
            redoList = new List<Shape>();
            UndoList = new List<Shape>();

        }


        /// <summary>
        /// Draws item on complete
        /// </summary>
        public Point DrawOnComplete(Point cupt)
        {
            switch (tool)
            {
                case CurrentTool.Line:
                    {
                        var line = toolHelper.CreateLine(PrevPoint, cupt);
                        ApplyAttributes(line as Shape);
                        _canvas.Children.Add(line);
                        PrevPoint = cupt;
                        AddToUndoShape(line as Shape);
                        break;

                    }

                case CurrentTool.Rectangle:
                    {
                        var shp = toolHelper.CreateShape<Rectangle>(PrevPoint, cupt);
                        ApplyAttributes(shp as Shape);
                        _canvas.Children.Add(shp);
                        AddToUndoShape(shp as Shape);
                        PrevPoint = cupt;

                        break;
                    }
                case CurrentTool.CustomStar:
                    {
                        Polygon figure = new Polygon();
                        figure.Points = new PointCollection { new Point(100 + PrevPoint.X, 0 + PrevPoint.Y), new Point(75 + PrevPoint.X, 75 + PrevPoint.Y), new Point(100 + PrevPoint.X, 100 + PrevPoint.Y), new Point(125 + PrevPoint.X, 75 + PrevPoint.Y) };
                        figure.Stroke = Brushes.Black;
                        figure.StrokeThickness = 2;
                        figure.Fill = Brushes.Yellow;

                        Polygon figure2 = new Polygon();
                        figure2.Points = new PointCollection { new Point(100 + PrevPoint.X, 100 + PrevPoint.Y), new Point(125 + PrevPoint.X, 125 + PrevPoint.Y), new Point(100 + PrevPoint.X, 200 + PrevPoint.Y), new Point(75 + PrevPoint.X, 125 + PrevPoint.Y) };
                        figure2.Stroke = Brushes.Yellow;
                        figure2.StrokeThickness = 2;
                        figure2.Fill = Brushes.Black;

                        Polygon figure3 = new Polygon();
                        figure3.Points = new PointCollection { new Point(100 + PrevPoint.X, 100 + PrevPoint.Y), new Point(125 + PrevPoint.X, 75 + PrevPoint.Y), new Point(200 + PrevPoint.X, 100 + PrevPoint.Y), new Point(125 + PrevPoint.X, 125 + PrevPoint.Y) };
                        figure3.Stroke = Brushes.Red;
                        figure3.StrokeThickness = 2;
                        figure3.Fill = Brushes.Blue;

                        Polygon figure4 = new Polygon();
                        figure4.Points = new PointCollection { new Point(100 + PrevPoint.X, 100 + PrevPoint.Y), new Point(75 + PrevPoint.X, 125 + PrevPoint.Y), new Point(0 + PrevPoint.X, 100 + PrevPoint.Y), new Point(75 + PrevPoint.X, 75 + PrevPoint.Y) };
                        figure4.Stroke = Brushes.Blue;
                        figure4.StrokeThickness = 2;
                        figure4.Fill = Brushes.Red;

                        _canvas.Children.Add(figure);
                        _canvas.Children.Add(figure2);
                        _canvas.Children.Add(figure3);
                        _canvas.Children.Add(figure4);
                        AddToUndoShape(figure as Shape);
                        AddToUndoShape(figure2 as Shape);
                        AddToUndoShape(figure3 as Shape);
                        AddToUndoShape(figure4 as Shape);

                        PrevPoint = cupt;
                    }
                    break;
                case CurrentTool.Triangle:
                    {
                        var shp = toolHelper.CreateShape<Triangle>(PrevPoint, cupt);
                        ApplyAttributes(shp as Shape);
                        _canvas.Children.Add(shp);
                        AddToUndoShape(shp as Shape);
                        PrevPoint = cupt;
                    }
                    break;
                case CurrentTool.Ellipse:
                    {
                        var shp = toolHelper.CreateShape<Ellipse>(PrevPoint, cupt);
                        ApplyAttributes(shp as Shape);
                        _canvas.Children.Add(shp);
                        //tempHolder.Add(shp as Shape);
                        AddToUndoShape(shp as Shape);


                        PrevPoint = cupt;
                        break;
                    }
                case CurrentTool.Brush:
                case CurrentTool.EraseBrush:
                case CurrentTool.Pencil:
                    {
                        var shp = toolHelper.CreateLine(PrevPoint, cupt);
                        List<Shape> UnshapeList = new List<Shape>();
                        tempHolder.ForEach(p => UnshapeList.Add(p));
                        UnShapeDrawingItems.Add(shp as Shape, UnshapeList);
                        tempHolder.Clear();
                        AddToUndoShape(shp as Shape);
                    }
                    break;
            }



            return cupt;
        }

        /// <summary>
        /// Draws the item on move
        /// </summary>
        public Point DrawOnMove(Point cupt)
        {
            switch (tool)
            {
                case CurrentTool.Brush:
                    {
                        HideVirtualLine();
                        var spot = toolHelper.CreateBrush(PrevPoint, cupt, StrokeWidth);
                        (spot as Shape).StrokeThickness = 0;
                        (spot as Shape).Fill = FillColor;
                        _canvas.Children.Add(spot);
                        tempHolder.Add(spot as Shape);
                        PrevPoint = cupt;
                        break;

                    }

                case CurrentTool.EraseBrush:
                    {
                        HideVirtualLine();
                        var spot = toolHelper.CreateBrush(PrevPoint, cupt, StrokeWidth);
                        (spot as Shape).StrokeThickness = 0;
                        (spot as Shape).Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _canvas.Children.Add(spot);
                        tempHolder.Add(spot as Shape);
                        PrevPoint = cupt;
                        break;

                    }
                case CurrentTool.Pencil:
                    {
                        var pen = toolHelper.CreateLine(PrevPoint, cupt);
                        ApplyAttributes(pen as Shape);
                        (pen as Shape).StrokeThickness = 3;
                        _canvas.Children.Add(pen);
                        tempHolder.Add(pen as Shape);
                        PrevPoint = cupt;
                        break;
                    }
                default:
                    {
                        ShowVirtualLine(StartPoint.X, StartPoint.Y, cupt.X, cupt.Y);
                        break;
                    }

            }
            return cupt;
        }

        /// <summary>
        /// Apply shape attribute
        /// </summary>
        public void ApplyAttributes(Shape e)
        {
            e.Stroke = StrokeColor;
            e.Fill = FillColor;
            e.StrokeThickness = 2;
            e.StrokeThickness = StrokeWidth;
        }


        /// <summary>
        /// Hide virtual line
        /// </summary>
        public void HideVirtualLine()
        {
            _virtualLine.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows the virtual line
        /// </summary>
        public void ShowVirtualLine(double x1, double y1, double x2, double y2)
        {
            if (!_canvas.Children.Contains(_virtualLine))
                _canvas.Children.Add(_virtualLine);
            _virtualLine.Visibility = Visibility.Visible;
            Canvas.SetZIndex(_virtualLine, 0);
            _virtualLine.X1 = x1;
            _virtualLine.X2 = x2;
            _virtualLine.Y1 = y1;
            _virtualLine.Y2 = y2;
        }


        public void UndoShape()
        {
            if (undoTop > 0)
            {
                undoTop--;
                Shape shape = UndoList[undoTop];
                if (shape is Line && UnShapeDrawingItems.ContainsKey(shape))
                {
                    foreach (var unShapeDrawingItem in UnShapeDrawingItems[shape])
                    {
                        _canvas.Children.Remove(unShapeDrawingItem);
                    }
                }
                else
                    _canvas.Children.Remove(shape);
                UndoList.RemoveAt(undoTop);

                AddToRedoList(shape);
            }
        }
        public void RedoShape()
        {
            if (redoTop > 0)
            {
                redoTop--;
                Shape shape = redoList[redoTop];
                if (shape is Line && UnShapeDrawingItems.ContainsKey(shape))
                {
                    foreach (var unShapeDrawingItem in UnShapeDrawingItems[shape])
                    {
                        _canvas.Children.Add(unShapeDrawingItem);
                    }
                }
                else
                    _canvas.Children.Add(shape);
                redoList.RemoveAt(redoTop);
                AddToUndoShape(shape);
            }
        }

        private void AddToRedoList(Shape shape)
        {
            if (redoTop >= 100)
            {
                RemoveRedoBottom();
            }
            redoTop++;
            redoList.Add(shape);
        }

        private void RemoveRedoBottom()
        {
            redoTop--;
            redoList.RemoveAt(0);
        }

        public void AddToUndoShape(Shape shape)
        {
            if (undoTop >= 100)
            {
                RemoveUndoBottom();
            }
            undoTop++;
            UndoList.Add(shape);

        }

        private void RemoveUndoBottom()
        {
            undoTop--;
            UndoList.RemoveAt(0);
        }


        #endregion

        public void Clear()
        {
            _canvas.Children.Clear();
            UndoList.Clear();
            redoList.Clear();
            UnShapeDrawingItems.Clear();
            tempHolder.Clear();
            undoTop = 0;
            redoTop = 0;
        }


    }

    #region Enum
    public enum CurrentTool
    {
        Line,
        Pencil,
        Brush,
        EraseBrush,
        Ellipse,
        Rectangle,
        Triangle,
        CustomStar
    }
    #endregion
}
