using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchAlgorithms.Model
{
    public enum SearchEventType
    {
        AddedEdgeToTraveled,
        AddedEdgeToExplored,
        RemovedEdgeFromExplored
    }

    public class SearchEvent
    {
        public SearchEventType Type { get; private set; }
        public Edge ParticipantEdge { get; private set; }

        public string EventMessage { get; private set; }

        public SearchEvent(SearchEventType type, Edge edge)
        {
            this.Type = type;
            this.ParticipantEdge = edge;

            switch (type)
            {
                case SearchEventType.AddedEdgeToExplored:
                    this.EventMessage = string.Format("Added to explored: {0}", edge);
                    break;
                case SearchEventType.AddedEdgeToTraveled:
                    this.EventMessage = string.Format("Added to TRAVELED: {0}", edge);
                    break;
                case SearchEventType.RemovedEdgeFromExplored:
                    this.EventMessage = string.Format("Removed from explored: {0}", edge);
                    break;
            }
        }
    }
}
