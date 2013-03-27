using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class TraveledPathData
    {
        public List<Edge> ExploredEdges { get; private set; }
        public List<Edge> TraveledEdges { get; private set; }
        public Double PathCost { get; private set; }

        public SearchEventManager EventManager { get; set; }

        public TraveledPathData(Vertex root, Vertex goal)
        {
            this.EventManager = new SearchEventManager(root, goal);
            this.ExploredEdges = new List<Edge>();
            this.TraveledEdges = new List<Edge>();
        }

        public void AddExploredEdge(Edge edge)
        {
            this.ExploredEdges.Add(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToExplored, edge);
            this.EventManager.AddEvent(ev);
        }

        public void AddTraveledEdge(Edge edge)
        {
            this.TraveledEdges.Add(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToTraveled, edge);
            this.EventManager.AddEvent(ev);
        }

        public void RemoveExplored(Vertex vert)
        {
            Edge edgeToRemove = ExploredEdges.Single(e => e.VerticeTo == vert);
            this.ExploredEdges.Remove(edgeToRemove);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edgeToRemove);
            this.EventManager.AddEvent(ev);
        }

        public void RemoveExplored(Edge edge)
        {
            this.ExploredEdges.Remove(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edge);
            this.EventManager.AddEvent(ev);
        }

        public List<Edge> GetShortestPath()
        {
            List<Edge> shortestPath = new List<Edge>();
            var lastEdge = this.TraveledEdges.Last();
            shortestPath.Insert(0,lastEdge);


            while (this.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeFrom))
            {
                var edge = this.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeFrom);
                shortestPath.Insert(0, edge);
            }

            return shortestPath;
        }

        public void UpdatePathCost(Double pathCost)
        {
            this.PathCost = pathCost;
        }
    }
}
