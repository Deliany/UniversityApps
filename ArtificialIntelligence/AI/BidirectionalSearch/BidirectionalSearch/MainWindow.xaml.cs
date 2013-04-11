using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SearchAlgorithms.Annotations;
using SearchAlgorithms.Helper;
using SearchAlgorithms.Model;
using QuickGraph;

namespace SearchAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Double[,] Matrix { get; set; }
        public Graph Graph { get; set; }
        public BidirectionalGraph<object, IEdge<object>> VisioGraph { get; set; }
        public BidirectionalGraph<object, IEdge<object>> VisioTree { get; set; }

        public SearchAlgorithm sa;

        public ObservableCollection<Vertex> Vertices { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Graph = new Graph("graph_matrix.txt", "heuristic_data.txt");
            Logger.LogBox = this.logRTF;

            this.FillVisioGraphWithGraph(this.Graph);
            this.FillComboboxesWithVertices(this.Graph.Vertices);
            this.FillDataTableWithMatrix(this.Graph);
        }

        

        private void WritePathCostToTextBox(Double cost)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Logger.WriteLogInfo(string.Format("Total path cost: {0}",cost)); 
                    this.pathCostTextBox.Text = cost.ToString(CultureInfo.InvariantCulture); 
                }));
        }

        private void WriteLogWithTwoWayTraveledData(TwoWayTraveledPathData data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SearchEventManager sem = data.SEM;

                Logger.ClearLog();

                foreach (var searchEvent in sem.Events)
                {
                    Logger.WriteLogInfo(
                        searchEvent.EventMessage);
                }
                StringBuilder msg = new StringBuilder();

                Logger.WriteLogInfo(string.Format("Total edges EXPLORED in PATH1: {0}", data.Path1.ExploredEdges.Count));
                foreach (var edge in data.Path1.ExploredEdges)
                {
                    msg.Append(edge + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();

                Logger.WriteLogInfo(string.Format("Total edges EXPLORED in PATH2: {0}", data.Path2.ExploredEdges.Count));
                foreach (var edge in data.Path2.ExploredEdges)
                {
                    msg.Append(edge + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();

                Logger.WriteLogInfo(string.Format("Total edges TRAVELED in PATH1: {0}", data.Path1.TraveledEdges.Count));
                foreach (var vert in data.Path1.TraveledEdges)
                {
                    msg.Append(vert + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();

                Logger.WriteLogInfo(string.Format("Total edges TRAVELED in PATH2: {0}", data.Path2.TraveledEdges.Count));
                foreach (var vert in data.Path2.TraveledEdges)
                {
                    msg.Append(vert + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();

                Logger.WriteLogInfo("SHORTEST PATH:");
                var shortestPath = data.GetShortestPath();
                foreach (var edge in shortestPath)
                {
                    msg.Append(edge + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
            }));
        }

        private void WriteLogWithTraveledData(TraveledPathData data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SearchEventManager sem = data.EventManager;

                Logger.ClearLog();

                foreach (var searchEvent in sem.Events)
                {
                    Logger.WriteLogInfo(
                        searchEvent.EventMessage);
                }
                StringBuilder msg = new StringBuilder();

                Logger.WriteLogInfo(string.Format("Total edges EXPLORED in PATH: {0}", data.ExploredEdges.Count));
                foreach (var edge in data.ExploredEdges)
                {
                    msg.Append(edge + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();


                Logger.WriteLogInfo(string.Format("Total edges TRAVELED in PATH: {0}", data.TraveledEdges.Count));
                foreach (var vert in data.TraveledEdges)
                {
                    msg.Append(vert + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
                msg.Clear();

                Logger.WriteLogInfo("SHORTEST PATH:");

                var shortestPath = data.GetShortestPath();
                foreach (var edge in shortestPath)
                {
                    msg.Append(edge + "=>");
                }
                Logger.WriteLogInfo(msg.ToString());
            }));
        }

        private void SetupVisioTree()
        {
            this.VisioTree = new BidirectionalGraph<object, IEdge<object>>();
            var overlapRemoval = new GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters();
            overlapRemoval.HorizontalGap = 50;
            overlapRemoval.VerticalGap = 50;
            this.treeLayout.OverlapRemovalParameters = overlapRemoval;
            this.treeLayout.Graph = null;
        }

        private void FillVisioGraphWithGraph(Graph graph)
        {
            this.VisioGraph = new BidirectionalGraph<object, IEdge<object>>();
        
            foreach (var edge in graph.Edges)
            {
                this.VisioGraph.AddVerticesAndEdge(new MyEdge(edge.VerticeFrom, edge.VerticeTo, edge.Weight.ToString(CultureInfo.InvariantCulture),
                                                              Colors.Silver));
            }
            this.graphLayout.Graph = this.VisioGraph;
            

            // get some extra space between vertices
            var overlapRemoval = new GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters();
            overlapRemoval.HorizontalGap = 50;
            overlapRemoval.VerticalGap = 50;
            this.graphLayout.OverlapRemovalParameters = overlapRemoval;
        }

        private void FillComboboxesWithVertices(IEnumerable<Vertex> vertices)
        {
            this.Vertices = new ObservableCollection<Vertex>();
            foreach (var vertex in vertices)
            {
                this.Vertices.Add(vertex);
            }
            rootVertexChkbx.ItemsSource = this.Vertices;
            goalVertexChkbx.ItemsSource = this.Vertices;
        }

        private void FillDataTableWithMatrix(Graph graph)
        {
            this.Matrix = graph.AsMatrix();
            int n = this.Matrix.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (this.Matrix[i, j] != this.Matrix[j, i])
                    {
                        MessageBox.Show(string.Format("Error at [{0},{1}]={2} and [{3},{4}]={5}", i, j, this.Matrix[i, j], j, i, this.Matrix[j, i]));
                    }
                }
            }

            c_dataGrid2D.ItemsSource = BindingHelper.GetBindable2DArray<Double>(this.Matrix);
        }

        private void SearchDidFinishedWithData(object data)
        {
            SearchEventManager sem;
            List<Edge> shortestPath;
            if (data is TwoWayTraveledPathData)
            {
                TwoWayTraveledPathData twtpd = data as TwoWayTraveledPathData;
                sem = twtpd.SEM;
                shortestPath = twtpd.GetShortestPath();
                this.WritePathCostToTextBox(twtpd.TotalCost);
                this.WriteLogWithTwoWayTraveledData(twtpd);
            }
            else if (data is TraveledPathData)
            {
                TraveledPathData tpd = data as TraveledPathData;
                sem = tpd.EventManager;
                shortestPath = tpd.TraveledEdges;
                this.WritePathCostToTextBox(tpd.TotalCost);
                this.WriteLogWithTraveledData(tpd);
            }
            else
            {
                return;
            }


            Application.Current.Dispatcher.Invoke(new Action(this.SetupVisioTree));
            Application.Current.Dispatcher.Invoke(new Action(() => this.FillVisioGraphWithGraph(this.Graph)));
            
            foreach (var searchEvent in sem.Events)
            {
                var edges1 = this.VisioGraph.Edges.Where(
                                e =>
                                (Vertex) e.Source == searchEvent.ParticipantEdge.VerticeFrom &&
                                (Vertex) e.Target == searchEvent.ParticipantEdge.VerticeTo);

                switch (searchEvent.Type)
                {
                    //case SearchEventType.AddedEdgeToExplored:
                    //    foreach (var edge in edges1)
                    //    {
                    //        if (edge is MyEdge)
                    //        {
                    //            var ed = edge as MyEdge;
                    //            ed.EdgeColor = Colors.Blue;

                    //            Application.Current.Dispatcher.Invoke(new Action(() => { this.VisioTree.AddVerticesAndEdge(edge); this.treeLayout.Graph = this.VisioTree; }));
                    //            Thread.Sleep(1000);
                    //        }
                    //    }
                    //    break;
                    case SearchEventType.AddedEdgeToTraveled:
                        foreach (var edge in edges1)
                        {

                            if (edge is MyEdge)
                            {
                                var ed = edge as MyEdge;
                                ed.EdgeColor = Colors.Green;

                                Application.Current.Dispatcher.Invoke(new Action(() => { this.VisioTree.AddVerticesAndEdge(edge); this.treeLayout.Graph = this.VisioTree; }));
                                Thread.Sleep(1000);
                            }
                        }
                        break;
                    //case SearchEventType.RemovedEdgeFromExplored:
                    //    foreach (var edge in edges1)
                    //    {
                    //        if (edge is MyEdge)
                    //        {
                    //            var ed = edge as MyEdge;
                    //            ed.EdgeColor = Colors.Red;

                    //            Application.Current.Dispatcher.Invoke(new Action(() => { this.VisioTree.AddVerticesAndEdge(edge); this.treeLayout.Graph = this.VisioTree; }));
                    //            Thread.Sleep(1000);
                    //        }
                    //    }
                    //    break;
                }
            }

            foreach (var edge in shortestPath)
            {
                var edgeFromGraph = this.VisioGraph.Edges.Single(
                    e => (Vertex) e.Source == edge.VerticeFrom &&
                         (Vertex) e.Target == edge.VerticeTo);
                if (edgeFromGraph is MyEdge)
                {
                    var ed = edgeFromGraph as MyEdge;
                    ed.EdgeColor = Colors.OrangeRed;
                }
            }

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sa = this.SearchAlgorithmComboBox.SelectedIndex == 0 ? (SearchAlgorithm) new BidirectionalSearch() : new GreedySearch();
                sa.searchDidFinishedWithData = SearchDidFinishedWithData;
                sa.AsynchronousSearch(this.Graph, new Vertex(rootVertexChkbx.Text), new Vertex(goalVertexChkbx.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            sa.ActionThread.Abort();
            this.FillVisioGraphWithGraph(this.Graph);
            this.graphLayout.Relayout();
        }

        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            this.graphLayout.Relayout();
            this.treeLayout.Relayout();
        }

        private void PSA_OnClick(object sender, RoutedEventArgs e)
        {
            this.SetupVisioTree();
            

            string rootName = rootVertexChkbx.Text;
            new Thread(() =>
                {
                    ProblemSolvingAgent psa = new ProblemSolvingAgent(5);
                    Edge action;
                    try
                    {
                        while (
                            (action =
                             psa.execute(new ProblemSolvingAgent.Percept
                                 {
                                     Current = new Vertex(rootName),
                                     Graph = this.Graph
                                 })) != null)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    this.goalVertexChkbx.Text = action.VerticeTo.Name;

                                    this.VisioTree.AddVerticesAndEdge(new MyEdge(action.VerticeFrom, action.VerticeTo,
                                                                                 action.Weight.ToString(
                                                                                     CultureInfo.InvariantCulture),
                                                                                 Colors.Green));
                                    this.treeLayout.Graph = this.VisioTree;
                                }));

                            Thread.Sleep(1000);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }).Start();
        }
    }
}
