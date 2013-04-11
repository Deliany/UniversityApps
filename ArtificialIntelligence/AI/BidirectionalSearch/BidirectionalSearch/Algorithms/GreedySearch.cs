using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SearchAlgorithms.Model
{
    /*
     * currentNode = startNode;
        loop do
            L = NEIGHBORS(currentNode);
            nextCost = INF;
            nextNode = NULL;
            for all x in L
                if (HOPCOST(x) < nextCost)
                    nextNode = x;
                    nextCost = HOPCOST(x);
                end if
            end for
            if nextNode == targetNode
                return “computed path from startNode to nextNode”;
            end if
            currentNode = nextNode;
        end do
     */
    public class GreedySearch : SearchAlgorithm
    {
        private TraveledPathData pathData;

        public override void AsynchronousSearch(Graph graph, Vertex root, Vertex goal)
        {
            this.graph = graph;
            this.root = root;
            this.goal = goal;
            new Thread(AsyncSearch).Start();
        }

        public TraveledPathData SynchronousSearch(Graph graph, Vertex root, Vertex goal)
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

        private TraveledPathData SyncSearch()
        {
            return this.Search();
        }

        private TraveledPathData Search()
        {
            this.pathData = new TraveledPathData(root, goal);

            Vertex currentNode = root;
            do
            {
                List<Vertex> neighbours = this.graph.AdjacentVertices(currentNode);
                Double nextCost = double.PositiveInfinity;
                Vertex nextNode = null;
                Edge nextEdge = null;
                foreach (var x in neighbours)
                {
                    Edge edgeFromHeuristic = this.graph.HeuristicData.GetEdge(x, goal);

                    Edge possibleNextEdge = this.graph.GetEdge(currentNode, x);
                    double HOPCOST = edgeFromHeuristic.Weight;
                    if (HOPCOST < nextCost )//&& !this.pathData.TraveledEdges.Contains(possibleNextEdge))
                    {
                        nextNode = x;
                        nextEdge = possibleNextEdge;
                        nextCost = HOPCOST;
                    }
                }
                this.pathData.AddTraveledEdge(this.graph.GetEdge(nextEdge.VerticeFrom, nextEdge.VerticeTo));
                
                if (nextNode == this.goal)
                {
                    return this.pathData;
                }
                currentNode = nextNode;

            } while (true);
        }
    }
}
