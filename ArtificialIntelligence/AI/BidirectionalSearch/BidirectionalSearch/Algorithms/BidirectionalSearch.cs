using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using SearchAlgorithms.Helper;

namespace SearchAlgorithms.Model
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
    /// Bidirectional search that uses uniform-cost search in two directions - one from start and one from goal
    /// </summary>
    public class BidirectionalSearch : SearchAlgorithm
    {
        private TwoWayTraveledPathData pathData;

        public override void AsynchronousSearch(Graph graph, Vertex root, Vertex goal)
        {
            this.graph = graph;
            this.root = root;
            this.goal = goal;
            new Thread(AsyncSearch).Start();
        }

        public TwoWayTraveledPathData SynchronousSearch(Graph graph, Vertex root, Vertex goal)
        {
            this.graph = graph;
            this.root = root;
            this.goal = goal;
            return this.SyncSearch();
        }

        private void AsyncSearch()
        {
            var result = this.Search();
            this.ExecuteInBiggerStackThread(searchDidFinishedWithData, result);
        }

        private TwoWayTraveledPathData SyncSearch()
        {
            return this.Search();
        }

        private TwoWayTraveledPathData Search()
        {
            pathData = new TwoWayTraveledPathData(root, goal);

            // enqueue root node in frontier
            pathData.Frontier1.Enqueue(root);
            pathData.Frontier2.Enqueue(goal);

            // if frontier is empty - we finished searching
            while (!pathData.Frontier1.Empty || !pathData.Frontier2.Empty)
            {
                // by priority at top I mean the smallest total cost path to vertex in frontier
                pathData.UpdateCost1(pathData.Frontier1.PriorityAtTop());
                pathData.UpdateCost2(pathData.Frontier2.PriorityAtTop());

                // dequeue vertex with the least total cost
                Vertex node1 = pathData.Frontier1.Dequeue();
                Vertex node2 = pathData.Frontier2.Dequeue();

                // search in explored for adjacent edge for added node
                foreach (var edge in pathData.Path1.ExploredEdges)
                {
                    if (edge.VerticeTo == node1)
                    {
                        pathData.AddTraveledEdgeToPath1(new Edge(edge, pathData.Cost1));
                    }
                }
                foreach (var edge in pathData.Path2.ExploredEdges)
                {
                    if (edge.VerticeTo == node2)
                    {
                        pathData.AddTraveledEdgeToPath2(new Edge(edge, pathData.Cost2));
                    }
                }

                // if our dequeued node is goal node, then we got path with shortest cost to it
                if (node1 == goal || node2 == root)
                {
                    return pathData;
                }

                // add dequeued node to explored list
                pathData.Explored1.Add(node1);
                pathData.Explored2.Add(node2);

                if (pathData.Path2.TraveledEdges.Any(e => e.VerticeTo == node1) || pathData.Path1.TraveledEdges.Any(e => e.VerticeTo == node2))
                {
                    return pathData;
                }


                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node1))
                {
                    // if node is not in explored
                    if (!pathData.Explored1.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node1, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!pathData.Frontier1.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            pathData.AddExploredEdgeToPath1(new Edge(edge, pathData.Cost1 + edge.Weight));

                            pathData.Frontier1.Enqueue(adjacentVertex, pathData.Cost1 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((pathData.Cost1 + edge.Weight) < pathData.Frontier1.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            pathData.RemoveExploredFromPath1(adjacentVertex);
                            pathData.AddExploredEdgeToPath1(new Edge(edge, pathData.Cost1 + edge.Weight));

                            // update vertex priority in frontier
                            pathData.Frontier1.Remove(adjacentVertex);
                            pathData.Frontier1.Enqueue(adjacentVertex, pathData.Cost1 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, pathData.Cost1 + edge.Weight);
                            pathData.AddExploredEdgeToPath1(tempEdge);
                            pathData.RemoveExploredFromPath1(tempEdge);
                        }
                    }
                }
                // foreach neighbour's node
                foreach (var adjacentVertex in graph.AdjacentVertices(node2))
                {
                    // if node is not in explored
                    if (!pathData.Explored2.Contains(adjacentVertex))
                    {
                        Edge edge = graph.GetEdge(node2, adjacentVertex);
                        // and if neighbour node is not in frontier
                        if (!pathData.Frontier2.Contains(adjacentVertex))
                        {
                            // add edge NODE-ADJACENTVERTEX
                            pathData.AddExploredEdgeToPath2(new Edge(edge, pathData.Cost2 + edge.Weight));

                            pathData.Frontier2.Enqueue(adjacentVertex, pathData.Cost2 + edge.Weight);
                        }
                        // else if neighbour node is in frontier and its cost is higher then current
                        // -> replace existing node with node with lower path cost
                        else if ((pathData.Cost2 + edge.Weight) < pathData.Frontier2.GetPriority(adjacentVertex))
                        {
                            // remove edge PREVIOUSNODE-ADJACENTVERTEX
                            // add edge NODE-ADJACENTVERTEX
                            pathData.RemoveExploredFromPath2(adjacentVertex);
                            pathData.AddExploredEdgeToPath2(new Edge(edge, pathData.Cost2 + edge.Weight));

                            // update vertex priority in frontier
                            pathData.Frontier2.Remove(adjacentVertex);
                            pathData.Frontier2.Enqueue(adjacentVertex, pathData.Cost2 + edge.Weight);
                        }
                        else
                        {
                            // add and delete it cause it is failure, longer path
                            Edge tempEdge = new Edge(edge, pathData.Cost2 + edge.Weight);
                            pathData.AddExploredEdgeToPath2(tempEdge);
                            pathData.RemoveExploredFromPath2(tempEdge);
                        }
                    }
                }
            }
            return null;
        }
    }
}
