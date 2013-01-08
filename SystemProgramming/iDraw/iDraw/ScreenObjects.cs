using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace iDraw
{
    public enum ScreenObjectType
    {
        Line,
        Ellipse,
        Text,
        Shape,
        Rectangle
    }

    public class ScreenObject
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double Thickness { get; set; }
        public string StrokeColor { get; set; }
        public string FillColor { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ScreenObjectType Type { get; set; }
        public string Text;
    }

    public class Triangle : Shape
    {
        public Triangle()
        {
        }

        protected override Geometry DefiningGeometry
        {
            get
            {

                Point p1 = new Point(0.0d, 0.0d);
                Point p2 = new Point(this.Width, 0.0d);
                Point p3 = new Point(this.Width / 2, this.Height);

                List<PathSegment> segments = new List<PathSegment>(3);
                segments.Add(new LineSegment(p1, true));
                segments.Add(new LineSegment(p2, true));
                segments.Add(new LineSegment(p3, true));

                List<PathFigure> figures = new List<PathFigure>(1);
                PathFigure pf = new PathFigure(p1, segments, true);
                figures.Add(pf);

                Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);

                return g;
            }
        }

    }
}
