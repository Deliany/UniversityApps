using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class SearchEventManager
    {
        private readonly List<SearchEvent> events = new List<SearchEvent>();

        public Vertex RootVertex { get; private set; }
        public Vertex GoalVertex { get; private set; }

        public SearchEventManager(Vertex root, Vertex goal)
        {
            this.RootVertex = root;
            this.GoalVertex = goal;
        }

        public List<SearchEvent> Events
        {
            get { return events; }
        }

        public void AddEvent(SearchEvent e)
        {
            events.Add(e);
        }

        //public List<Edge> GetShortestPath()
        //{
        //    List<Edge> shortestPath = new List<Edge>();
        //    var lastEdge = this.events.Last().ParticipantEdge;
        //    shortestPath.Insert(0, lastEdge);


        //    while (this.events.Any(e => e.ParticipantEdge.VerticeTo == shortestPath.First().VerticeFrom && e.Type == SearchEventType.AddedEdgeToTraveled))
        //    {
        //        var edge = this.events.Single(e => e.ParticipantEdge.VerticeTo == shortestPath.First().VerticeFrom && e.Type == SearchEventType.AddedEdgeToTraveled).ParticipantEdge;
        //        shortestPath.Insert(0, edge);
        //    }

        //    while (this.events.Any(e => e.ParticipantEdge.VerticeTo == shortestPath.Last().VerticeTo && e.Type == SearchEventType.AddedEdgeToTraveled))
        //    {
        //        var edge = this.events.Where(e => e.ParticipantEdge.VerticeTo == shortestPath.Last().VerticeTo && e.Type == SearchEventType.AddedEdgeToTraveled).First().ParticipantEdge;
        //        shortestPath.Insert(0, edge);
        //    }
        //    return shortestPath;
        //}

        public void Clear()
        {
            this.Events.Clear();
        }
    }
}
