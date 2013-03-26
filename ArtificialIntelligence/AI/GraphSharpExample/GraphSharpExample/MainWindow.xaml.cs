using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BidirectionalSearch.Model;
using GraphSharp;
using GraphSharp.Controls;
using GraphSharpExample.Annotations;
using QuickGraph;

namespace GraphSharpExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BidirectionalGraph<object, TaggedEdge<object, object>> Graph { get; set; }

        public MainWindow()
        {
            
           
            //var g = new BidirectionalGraph<object, IEdge<object>>();

            //IList<Object> vertices = new List<Object>();
            //for (int i = 0; i < 6; i++)
            //{
            //    vertices.Add(i.ToString());
            //}

            //for (int i = 0; i < 5; i++)
            //{
            //    Color edgeColor = (i%2 == 0) ? Colors.Black : Colors.Red;
            //    Console.WriteLine(edgeColor);

            //    g.AddVerticesAndEdge(new MyEdge(vertices[i], vertices[i + 1])
            //        {
            //            Id = i.ToString(),
            //            EdgeColor = edgeColor
            //        });
            //}

            //Graph = g;

            //Console.WriteLine(Graph.VertexCount);
            //Console.WriteLine(Graph.EdgeCount);

            

            var g = new BidirectionalGraph<object, TaggedEdge<object, object>>();

            Graph gr = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
            List<Edge> pss = new List<Edge>();
            foreach (var vert in gr.vertices)
            {
                g.AddVertex(vert.Name);
            }
            foreach (var edge in gr.edges)
            {
                //if (!pss.Contains(new Edge{VerticeFrom = edge.VerticeTo,VerticeTo = edge.VerticeFrom,Weight = edge.Weight}))
                {
                    g.AddEdge(new TaggedEdge<object, object>(edge.VerticeFrom.Name, edge.VerticeTo.Name, edge.Weight.ToString()));
                    pss.Add(edge);
                }
            }

            //add the vertices to the graph
            //Person[] vertices = new Person[26];
            //for (int i = 0; i < 26; i++)
            //{
            //    char letter = (char)(65 + i);
            //    vertices[i] = new Person { FirstName = letter.ToString(), LastName = "" };
            //    g.AddVertex(vertices[i]);
            //}

            //Random r = new Random();
            //for (int i = 0; i < 26; i++)
            //{
            //    g.AddEdge(new TaggedEdge<object, object>(vertices[i], vertices[r.Next(25)], (300+r.Next(300)).ToString()));
            //}
            //add some edges to the graph
            //g.AddEdge(new TaggedEdge<object, object>(vertices[0], vertices[1], "479"));
            //g.AddEdge(new TaggedEdge<object, object>(vertices[1], vertices[2], "123"));
            //g.AddEdge(new TaggedEdge<object, object>(vertices[2], vertices[3], "325"));
            //g.AddEdge(new TaggedEdge<object, object>(vertices[3], vertices[1], "556"));
            //g.AddEdge(new TaggedEdge<object, object>(vertices[1], vertices[4], "238"));
            this.Graph = g;

            
            // 
            InitializeComponent();
            GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters overlapRemoval = new GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters();
            overlapRemoval.HorizontalGap = 50;
            overlapRemoval.VerticalGap = 50;
            graphLayout.OverlapRemovalParameters = overlapRemoval;
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MyEdge edge = ((MyEdge) Graph.Edges.First());
            //edge.EdgeColor = Colors.GreenYellow;
            //graphLayout.InvalidateVisual();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            graphLayout.LayoutAlgorithmType =  (((sender as ComboBox).SelectedItem) as ComboBoxItem).Content.ToString();
        }
    }

    public class TaggedGraphLayout : GraphLayout<object, TaggedEdge<object, object>, IBidirectionalGraph<object, TaggedEdge<object, object>>>
    {
    }

    public class EdgeContentPresenter : ContentPresenter
    {
        public EdgeContentPresenter()
        {
            LayoutUpdated += new EventHandler(EdgeContentPresenter_LayoutUpdated);
        }

        private EdgeControl GetEdgeControl(DependencyObject parent)
        {
            while (parent != null)
                if (parent is EdgeControl)
                    return (EdgeControl)parent;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            return null;
        }

        private static double GetAngleBetweenPoints(Point point1, Point point2)
        {
            return Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);
        }

        private static double GetDistanceBetweenPoints(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        private static double GetLabelDistance(double edgeLength)
        {
            return edgeLength / 2;  // set the label halfway the length of the edge
        }

        void EdgeContentPresenter_LayoutUpdated(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;
            var edgeControl = GetEdgeControl(VisualParent);
            if (edgeControl == null)
                return;
            var source = edgeControl.Source;
            var p1 = new Point(GraphCanvas.GetX(source), GraphCanvas.GetY(source));
            var target = edgeControl.Target;
            var p2 = new Point(GraphCanvas.GetX(target), GraphCanvas.GetY(target));

            double edgeLength;
            var routePoints = edgeControl.RoutePoints;
            if (routePoints == null)
                // the edge is a single segment (p1,p2)
                edgeLength = GetLabelDistance(GetDistanceBetweenPoints(p1, p2));
            else
            {
                // the edge has one or more segments
                // compute the total length of all the segments
                edgeLength = 0;
                for (int i = 0; i <= routePoints.Length; ++i)
                    if (i == 0)
                        edgeLength += GetDistanceBetweenPoints(p1, routePoints[0]);
                    else if (i == routePoints.Length)
                        edgeLength += GetDistanceBetweenPoints(routePoints[routePoints.Length - 1], p2);
                    else
                        edgeLength += GetDistanceBetweenPoints(routePoints[i - 1], routePoints[i]);
                // find the line segment where the half distance is located
                edgeLength = GetLabelDistance(edgeLength);
                Point newp1 = p1;
                Point newp2 = p2;
                for (int i = 0; i <= routePoints.Length; ++i)
                {
                    double lengthOfSegment;
                    if (i == 0)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = p1, newp2 = routePoints[0]);
                    else if (i == routePoints.Length)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[routePoints.Length - 1], newp2 = p2);
                    else
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[i - 1], newp2 = routePoints[i]);
                    if (lengthOfSegment >= edgeLength)
                        break;
                    edgeLength -= lengthOfSegment;
                }
                // redefine our edge points
                p1 = newp1;
                p2 = newp2;
            }
            // align the point so that it  passes through the center of the label content
            var p = p1;
            var desiredSize = DesiredSize;
            p.Offset(-desiredSize.Width / 2, -desiredSize.Height / 2);

            // move it "edgLength" on the segment
            var angleBetweenPoints = GetAngleBetweenPoints(p1, p2);
            //p.Offset(edgeLength * Math.Cos(angleBetweenPoints), -edgeLength * Math.Sin(angleBetweenPoints));
            float x = 12.5f, y = 12.5f;
            double sin = Math.Sin(angleBetweenPoints);
            double cos = Math.Cos(angleBetweenPoints);
            double sign = sin * cos / Math.Abs(sin * cos);
            p.Offset(x * sin * sign + edgeLength * cos, y * cos * sign - edgeLength * sin);
            Arrange(new Rect(p, desiredSize));
        }

    }

    public class MyEdge : TypedEdge<Object>,INotifyPropertyChanged
    {
        public String Id { get; set; }

        private Color color;

        public Color EdgeColor { get { return color; } set { color = value; OnPropertyChanged("EdgeColor"); } }

        public MyEdge(Object source, Object target) : base(source, target, EdgeTypes.General)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class EdgeColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
