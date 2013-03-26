using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using BidirectionalSearch.Annotations;
using BidirectionalSearch.Helper;
using BidirectionalSearch.Model;
using QuickGraph;

namespace BidirectionalSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[,] Matrix { get; set; }
        public Graph Graph { get; set; }
        public BidirectionalGraph<object, IEdge<object>> VisioGraph { get; set; }
        public BidirectionalGraph<object, IEdge<object>> VisioTree { get; set; }
        Model.BidirectionalSearch br = new Model.BidirectionalSearch();
        public ObservableCollection<Vertex> Vertices { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            br.searchDidFinishedWithEvents = SearchDidFinishedWithEvents;
            br.setPathCost = SetPathCost;

            this.Graph = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
            Logger.LogBox = this.logRTF;

            this.FillVisioGraphWithGraph(this.Graph);
            this.FillComboboxesWithVertices(this.Graph.vertices);
            this.FillDataTableWithMatrix(this.Graph);
        }

        private void SetPathCost(int cost)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Logger.WriteLogInfo(string.Format("Total path cost: {0}",cost)); 
                    this.pathCostTextBox.Text = cost.ToString(); 
                }));
        }

        private void setupVisioTree()
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
        
            foreach (var edge in graph.edges)
            {
                this.VisioGraph.AddVerticesAndEdge(new MyEdge(edge.VerticeFrom, edge.VerticeTo, edge.Weight.ToString(),
                                                              Colors.Silver));
            }
            this.graphLayout.Graph = this.VisioGraph;
            

            // get some extra space between vertices
            var overlapRemoval = new GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters();
            overlapRemoval.HorizontalGap = 50;
            overlapRemoval.VerticalGap = 50;
            this.graphLayout.OverlapRemovalParameters = overlapRemoval;
        }

        private void FillComboboxesWithVertices(List<Vertex> vertices)
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

            c_dataGrid2D.ItemsSource = BindingHelper.GetBindable2DArray<int>(this.Matrix);
        }

        private void SearchDidFinishedWithEvents(SearchEventManager sem)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { this.setupVisioTree(); }));
            Application.Current.Dispatcher.Invoke(new Action(() => { this.FillVisioGraphWithGraph(this.Graph); }));

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Logger.ClearLog();
                foreach (var searchEvent in sem.Events)
                {
                    Logger.WriteLogInfo(
                        searchEvent.EventMessage);
                }
            }));

            //Application.Current.Dispatcher.Invoke(new Action(() => { Logger.WriteLogInfo("Shortest path:"); }));
            //var shortestPath = sem.GetShortestPath();
            //foreach (var edge in shortestPath)
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => { Logger.WriteLogInfo(edge.ToString()); }));
            //}
            

            foreach (var searchEvent in sem.Events)
            {
                var edges1 = this.VisioGraph.Edges.Where(
                                e =>
                                e.Source == searchEvent.ParticipantEdge.VerticeFrom &&
                                e.Target == searchEvent.ParticipantEdge.VerticeTo);

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

            //foreach (var edge in shortestPath)
            //{
            //    var edgeFromGraph = this.VisioGraph.Edges.Single(e => e.Source == edge.VerticeFrom && e.Target == edge.VerticeTo);
            //    if (edgeFromGraph is MyEdge)
            //    {
            //        var ed = edgeFromGraph as MyEdge;
            //        ed.EdgeColor = Colors.SpringGreen;
            //    }
            //}

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                br.Search(this.Graph, new Vertex(rootVertexChkbx.Text), new Vertex(goalVertexChkbx.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            br.OutsideThread.Abort();
            this.FillVisioGraphWithGraph(this.Graph);
            this.graphLayout.Relayout();
        }

        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            this.graphLayout.Relayout();
            this.treeLayout.Relayout();
        }
    }
}
