using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class TraveledPathData
    {
        private List<Edge> exploredEdges = new List<Edge>();
        private List<Edge> traveledEdges = new List<Edge>();

        private SearchEventManager eventManager;

        public SearchEventManager EventManager
        {
            get { return eventManager; }
            set { eventManager = value; }
        }

        public List<Edge> ExploredEdges
        {
            get { return exploredEdges; }
        }

        public List<Edge> TraveledEdges
        {
            get { return traveledEdges; }
        }

        public TraveledPathData(Vertex root, Vertex goal)
        {
            eventManager = new SearchEventManager(root, goal);
        }

        public void AddExploredEdge(Edge edge)
        {
            exploredEdges.Add(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToExplored, edge);
            eventManager.AddEvent(ev);
        }

        public void AddTraveledEdge(Edge edge)
        {
            traveledEdges.Add(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToTraveled, edge);
            eventManager.AddEvent(ev);
        }

        public void RemoveExplored(Vertex vert)
        {
            Edge edgeToRemove = exploredEdges.Single(e => e.VerticeTo == vert);
            exploredEdges.Remove(edgeToRemove);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edgeToRemove);
            eventManager.AddEvent(ev);
        }

        public void RemoveExplored(Edge edge)
        {
            exploredEdges.Remove(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edge);
            eventManager.AddEvent(ev);
        }

        public List<Edge> GetShortestPath()
        {
            List<Edge> shortestPath = new List<Edge>();
            var lastEdge = this.traveledEdges.Last();
            shortestPath.Insert(0,lastEdge);


            while (this.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeFrom))
            {
                var edge = this.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeFrom);
                shortestPath.Insert(0, edge);
            }

            return shortestPath;
        }
    }
}
