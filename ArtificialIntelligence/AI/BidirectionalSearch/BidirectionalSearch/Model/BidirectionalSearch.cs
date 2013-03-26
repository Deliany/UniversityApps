using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using BidirectionalSearch.Helper;

namespace BidirectionalSearch.Model
{
    /*
      procedure UniformCostSearch(Graph, root, goal)
        node := root, cost = 0
        frontier := priority queue containing node only
        explored := empty set
        do 
            if frontier is empty 
                return failure
            node := frontier.pop()
            if node is goal 
                return solution
            explored.add(node)
            for each of node's neighbors n 
                if n is not in explored 
                    if n is not in frontier 
                        frontier.add(n)
                    else if n is in frontier with higher cost 
                        replace existing node with n
     */


    /// <summary>
    /// Bidirectional search that uses uniform-cost search in two directions
    /// </summary>
    public class BidirectionalSearch
    {
        private PriorityQueue<Vertex> frontier1 = new PriorityQueue<Vertex>();
        private PriorityQueue<Vertex> frontier2 = new PriorityQueue<Vertex>();
        private List<Vertex> explored1 = new List<Vertex>();
        private List<Vertex> explored2 = new List<Vertex>();
        private Thread search1Thread;
        private Thread search2Thread;
        private int cost1;
        private int cost2;
        private TraveledPathData path1;
        private TraveledPathData path2;

        private Graph graph { get; set; }
        private Vertex root { get; set; }
        private Vertex goal { get; set; }

        public Action<SearchEventManager> searchDidFinishedWithEvents;
        public Action<int> setPathCost;

        public Thread OutsideThread { get; set; }


        public void Search(Graph graph, Vertex root, Vertex goal)
        {
            this.graph = graph;
            this.root = root;
            this.goal = goal;
            new Thread(TrueSearch).Start();
            //search1Thread = new Thread(Search1);
            //search2Thread = new Thread(Search2);
            //search1Thread.Start();
            //search2Thread.Start();
        }

        protected void ExecuteInBiggerStackThread<T>(Action<T> action, T parameterObject)
        {
            OutsideThread = new Thread(() => action(parameterObject), 1024 * 1024);
            OutsideThread.Start();
            OutsideThread.Join();
        }

        public void TrueSearch()
        {
            frontier1 = new PriorityQueue<Vertex>();
            explored1 = new List<Vertex>();
            path1 = new TraveledPathData(root, goal);

            frontier2 = new PriorityQueue<Vertex>();
            explored2 = new List<Vertex>();
            path2 = new TraveledPathData(goal, root);

            SearchEventManager man = new SearchEventManager(root, goal);
            path1.EventManager = man;
            path2.EventManager = man;

            // enqueue root node in frontier
            frontier1.Enqueue(root);
            frontier2.Enqueue(goal);

            // if frontier is empty - we finished searching
            while (!frontier1.Empty || !frontier2.Empty)
            {
                // by priority at top I mean the smallest total cost path to vertex in frontier
                cost1 = frontier1.PriorityAtTop();
                cost2 = frontier2.PriorityAtTop();

                // dequeue vertex with the least total cost
                Vertex node1 = frontier1.Dequeue();
                Vertex node2 = frontier2.Dequeue();

                // search in explored for adjacent edge for added node
                foreach (var edge in path1.ExploredEdges)
                {
                    if (edge.VerticeTo == node1)
                    {
                        path1.AddTraveledEdge(new Edge(edge, cost1));
                    }
                }
                foreach (var edge in path2.ExploredEdges)
                {
                    if (edge.VerticeTo == node2)
                    {
                        path2.AddTraveledEdge(new Edge(edge, cost2));
                    }
                }

                // if our dequeued node is goal node, then we got path with shortest cost to it
                if (node1 == goal)
                {
                    string msg = string.Empty;
                    foreach (var e in path1.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    //MessageBox.Show(msg + "\nTotal cost reached1: " + cost1);

                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path1.EventManager);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path2.EventManager);
                    return;
                }
                if (node2 == root)
                {
                    string msg = string.Empty;
                    foreach (var e in path2.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    //MessageBox.Show(msg + "\nTotal cost reached2: " + cost2);

                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path1.EventManager);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path2.EventManager);
                    return;
                }

                // add dequeued node to explored list
                explored1.Add(node1);
                explored2.Add(node2);

                if (path2.TraveledEdges.Any(e => e.VerticeTo == node1) || path1.TraveledEdges.Any(e => e.VerticeTo == node2))
                {
                    string msg = string.Empty;
                    foreach (var e in man.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";


                    //List<Vertex> shortestPath1 = path1.GenerateShortestPath();
                    //List<Vertex> shortestPath2 = path2.GenerateShortestPath();
                    //shortestPath2.Reverse();
                    //List<Vertex> allPath = new List<Vertex>();
                    //allPath.AddRange(shortestPath1);
                    //allPath.AddRange(shortestPath2);
                    //string strin = "";
                    //foreach (var vertex in allPath)
                    //{
                    //    strin += vertex + "->";
                    //}
                    //MessageBox.Show(strin);

                    // MessageBox.Show(string.Format("Total cost intersect1: {0}+{1}={2}", cost1, cost2, cost1 + cost2));

                    int totalCost = cost1 + cost2;
                    this.ExecuteInBiggerStackThread(this.setPathCost, totalCost);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, man);
                    return;
                }


                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node1))
                {
                    // if node is not in explored
                    if (!explored1.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node1, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier1.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost1 + edge.Weight) < frontier1.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path1.RemoveExplored(adjacentVertex);
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            // update vertex priority in frontier
                            frontier1.Remove(adjacentVertex);
                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost1 + edge.Weight);
                            path1.AddExploredEdge(tempEdge);
                            path1.RemoveExplored(tempEdge);
                        }
                    }
                }
                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node2))
                {
                    // if node is not in explored
                    if (!explored2.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node2, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier2.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost2 + edge.Weight) < frontier2.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path2.RemoveExplored(adjacentVertex);
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            // update vertex priority in frontier
                            frontier2.Remove(adjacentVertex);
                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost2 + edge.Weight);
                            path2.AddExploredEdge(tempEdge);
                            path2.RemoveExplored(tempEdge);
                        }
                    }
                }
            }
        }

        public int TrueSearche(Graph graph, Vertex root, Vertex goal)
        {
            this.graph = graph;
            this.root = root;
            this.goal = goal;
            frontier1 = new PriorityQueue<Vertex>();
            explored1 = new List<Vertex>();
            path1 = new TraveledPathData(root, goal);

            frontier2 = new PriorityQueue<Vertex>();
            explored2 = new List<Vertex>();
            path2 = new TraveledPathData(goal, root);

            SearchEventManager man = new SearchEventManager(root, goal);
            path1.EventManager = man;
            path2.EventManager = man;

            // enqueue root node in frontier
            frontier1.Enqueue(root);
            frontier2.Enqueue(goal);

            // if frontier is empty - we finished searching
            while (!frontier1.Empty || !frontier2.Empty)
            {
                // by priority at top I mean the smallest total cost path to vertex in frontier
                cost1 = frontier1.PriorityAtTop();
                cost2 = frontier2.PriorityAtTop();

                // dequeue vertex with the least total cost
                Vertex node1 = frontier1.Dequeue();
                Vertex node2 = frontier2.Dequeue();

                // search in explored for adjacent edge for added node
                foreach (var edge in path1.ExploredEdges)
                {
                    if (edge.VerticeTo == node1)
                    {
                        path1.AddTraveledEdge(new Edge(edge, cost1));
                    }
                }
                foreach (var edge in path2.ExploredEdges)
                {
                    if (edge.VerticeTo == node2)
                    {
                        path2.AddTraveledEdge(new Edge(edge, cost2));
                    }
                }

                // if our dequeued node is goal node, then we got path with shortest cost to it
                if (node1 == goal)
                {
                    string msg = string.Empty;
                    foreach (var e in path1.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    //MessageBox.Show(msg + "\nTotal cost reached1: " + cost1);

                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path1.EventManager);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path2.EventManager);
                    return cost1;
                }
                if (node2 == root)
                {
                    string msg = string.Empty;
                    foreach (var e in path2.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    //MessageBox.Show(msg + "\nTotal cost reached2: " + cost2);

                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path1.EventManager);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, path2.EventManager);
                    return cost2;
                }

                // add dequeued node to explored list
                explored1.Add(node1);
                explored2.Add(node2);

                if (path2.TraveledEdges.Any(e => e.VerticeTo == node1) || path1.TraveledEdges.Any(e => e.VerticeTo == node2))
                {
                    string msg = string.Empty;
                    foreach (var e in man.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";


                    //List<Vertex> shortestPath1 = path1.GenerateShortestPath();
                    //List<Vertex> shortestPath2 = path2.GenerateShortestPath();
                    //shortestPath2.Reverse();
                    //List<Vertex> allPath = new List<Vertex>();
                    //allPath.AddRange(shortestPath1);
                    //allPath.AddRange(shortestPath2);
                    //string strin = "";
                    //foreach (var vertex in allPath)
                    //{
                    //    strin += vertex + "->";
                    //}
                    //MessageBox.Show(strin);

                    // MessageBox.Show(string.Format("Total cost intersect1: {0}+{1}={2}", cost1, cost2, cost1 + cost2));

                    int totalCost = cost1 + cost2;
                    this.ExecuteInBiggerStackThread(this.setPathCost, totalCost);
                    this.ExecuteInBiggerStackThread(searchDidFinishedWithEvents, man);
                    return totalCost;
                }


                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node1))
                {
                    // if node is not in explored
                    if (!explored1.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node1, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier1.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost1 + edge.Weight) < frontier1.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path1.RemoveExplored(adjacentVertex);
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            // update vertex priority in frontier
                            frontier1.Remove(adjacentVertex);
                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost1 + edge.Weight);
                            path1.AddExploredEdge(tempEdge);
                            path1.RemoveExplored(tempEdge);
                        }
                    }
                }
                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node2))
                {
                    // if node is not in explored
                    if (!explored2.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node2, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier2.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost2 + edge.Weight) < frontier2.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path2.RemoveExplored(adjacentVertex);
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            // update vertex priority in frontier
                            frontier2.Remove(adjacentVertex);
                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost2 + edge.Weight);
                            path2.AddExploredEdge(tempEdge);
                            path2.RemoveExplored(tempEdge);
                        }
                    }
                }
            }
            return 0;
        }

        public void Search1()
        {
            frontier1 = new PriorityQueue<Vertex>();
            explored1 = new List<Vertex>();
            path1 = new TraveledPathData(root, goal);

            // enqueue root node in frontier
            frontier1.Enqueue(root);

            // if frontier is empty - we finished searching
            while (!frontier1.Empty)
            {
                // by priority at top I mean the smallest total cost path to vertex in frontier
                cost1 = frontier1.PriorityAtTop();

                // dequeue vertex with the least total cost
                Vertex node = frontier1.Dequeue();

                // search in explored for adjacent edge for added node
                foreach (var edge in path1.ExploredEdges)
                {
                    if (edge.VerticeTo == node)
                    {
                        path1.AddTraveledEdge(new Edge(edge, cost1));
                    }
                }

                // if our dequeued node is goal node, then we got path with shortest cost to it
                if (node == goal)
                {
                    string msg = string.Empty;
                    foreach (var e in path1.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    MessageBox.Show(msg + "\nTotal cost reached1: " + cost1);

                    this.searchDidFinishedWithEvents(path1.EventManager);
                    return;
                }

                // add dequeued node to explored list
                explored1.Add(node);

                // THREADED SYNC CODE
                //if (path2.TraveledEdges.Any(e => e.VerticeTo == node))
                //{
                //    search2Thread.Abort();
                //    MessageBox.Show(string.Format("Total cost intersect1: {0}+{1}={2}", cost1, cost2, cost1 + cost2));
                //    this.searchDidFinishedWithEvents(path1.EventManager);
                //    this.searchDidFinishedWithEvents(path2.EventManager);
                //    return;
                //}

                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node))
                {
                    // if node is not in explored
                    if (!explored1.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier1.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost1 + edge.Weight) < frontier1.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path1.RemoveExplored(adjacentVertex);
                            path1.AddExploredEdge(new Edge(edge, cost1 + edge.Weight));

                            // update vertex priority in frontier
                            frontier1.Remove(adjacentVertex);
                            frontier1.Enqueue(adjacentVertex, cost1 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost1 + edge.Weight);
                            path1.AddExploredEdge(tempEdge);
                            path1.RemoveExplored(tempEdge);
                        }
                    }
                }
            }
        }

        public void Search2()
        {
            frontier2 = new PriorityQueue<Vertex>();
            explored2 = new List<Vertex>();
            path2 = new TraveledPathData(goal, root);

            // enqueue root node in frontier
            frontier2.Enqueue(goal);

            // if frontier is empty - we finished searching
            while (!frontier2.Empty)
            {
                // by priority at top I mean the smallest total cost path to vertex in frontier
                cost2 = frontier2.PriorityAtTop();

                // dequeue vertex with the least total cost
                Vertex node = frontier2.Dequeue();

                // search in explored for adjacent edge for added node
                foreach (var edge in path2.ExploredEdges)
                {
                    if (edge.VerticeTo == node)
                    {
                        path2.AddTraveledEdge(new Edge(edge, cost2));
                    }
                }

                // if our dequeued node is goal node, then we got path with shortest cost to it
                if (node == root)
                {
                    string msg = string.Empty;
                    foreach (var e in path2.EventManager.Events)
                    {
                        msg += e.EventMessage + "\n";
                    }
                    msg += "!!!Finished!!!\n";
                    MessageBox.Show(msg + "\nTotal cost reached2: " + cost2);

                    this.searchDidFinishedWithEvents(path2.EventManager);
                    return;
                }

                // add dequeued node to explored list
                explored2.Add(node);

                // THREADED SYNC CODE
                //if (path1.TraveledEdges.Any(e => e.VerticeTo == node))
                //{
                //    search1Thread.Abort();
                //    MessageBox.Show(string.Format("Total cost intersect1: {0}+{1}={2}", cost1, cost2, cost1 + cost2));
                //    this.searchDidFinishedWithEvents(path1.EventManager);
                //    this.searchDidFinishedWithEvents(path2.EventManager);
                //    return;
                //}

                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node))
                {
                    // if node is not in explored
                    if (!explored2.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!frontier2.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((cost2 + edge.Weight) < frontier2.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            path2.RemoveExplored(adjacentVertex);
                            path2.AddExploredEdge(new Edge(edge, cost2 + edge.Weight));

                            // update vertex priority in frontier
                            frontier2.Remove(adjacentVertex);
                            frontier2.Enqueue(adjacentVertex, cost2 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, cost2 + edge.Weight);

                            path2.AddExploredEdge(tempEdge);
                            path2.RemoveExplored(tempEdge);
                        }
                    }
                }
            }
        }
    }
}
