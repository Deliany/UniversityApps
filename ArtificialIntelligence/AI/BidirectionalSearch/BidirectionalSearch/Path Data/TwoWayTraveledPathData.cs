using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchAlgorithms.Helper;

namespace SearchAlgorithms.Model
{
    public class TwoWayTraveledPathData
    {
        public TraveledPathData Path1 { get; private set; }
        public TraveledPathData Path2 { get; private set; }

        public PriorityQueue<Vertex> Frontier1 { get; private set; }
        public PriorityQueue<Vertex> Frontier2 { get; private set; }

        public List<Vertex> Explored1 { get; private set; }
        public List<Vertex> Explored2 { get; private set; }

        public Double Cost1 { get; private set; }
        public Double Cost2 { get; private set; }

        public Double TotalCost
        {
            get
            {
                Edge commonEdge = this.SEM.Events.Last().ParticipantEdge;
                Edge linkedEdge = null;

                if (this.Path1.TraveledEdges.Contains(commonEdge))
                {
                    if (this.Path2.TraveledEdges.Any(e => e.VerticeTo == commonEdge.VerticeTo))
                    {
                        linkedEdge = this.Path2.TraveledEdges.Single(e => e.VerticeTo == commonEdge.VerticeTo);
                    }
                }
                else if (this.Path2.TraveledEdges.Contains(commonEdge))
                {
                    if (this.Path1.TraveledEdges.Any(e => e.VerticeTo == commonEdge.VerticeTo))
                    {
                        linkedEdge = this.Path1.TraveledEdges.Single(e => e.VerticeTo == commonEdge.VerticeTo);
                    }
                }

                return commonEdge.Weight + (linkedEdge != null ? linkedEdge.Weight : 0.0);
            }
        }

        public Vertex Root { get; private set; }
        public Vertex Goal { get; private set; }

        public SearchEventManager SEM { get; private set; }

        public TwoWayTraveledPathData(Vertex root, Vertex goal)
        {
            this.Frontier1 = new PriorityQueue<Vertex>();
            this.Frontier2 = new PriorityQueue<Vertex>();

            this.Explored1 = new List<Vertex>();
            this.Explored2 = new List<Vertex>();

            this.Root = root;
            this.Goal = goal;

            this.Path1 = new TraveledPathData(root, goal);
            this.Path2 = new TraveledPathData(goal, root);

            this.SEM = new SearchEventManager(root, goal);
        }

        public void UpdateCost1(Double cost)
        {
            this.Cost1 = cost;
        }

        public void UpdateCost2(Double cost)
        {
            this.Cost2 = cost;
        }

        public void AddExploredEdgeToPath1(Edge edge)
        {
            this.Path1.AddExploredEdge(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToExplored, edge);
            this.SEM.AddEvent(ev);
        }

        public void AddExploredEdgeToPath2(Edge edge)
        {
            this.Path2.AddExploredEdge(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToExplored, edge);
            this.SEM.AddEvent(ev);
        }

        public void AddTraveledEdgeToPath1(Edge edge)
        {
            this.Path1.AddTraveledEdge(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToTraveled, edge);
            this.SEM.AddEvent(ev);
        }

        public void AddTraveledEdgeToPath2(Edge edge)
        {
            this.Path2.AddTraveledEdge(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.AddedEdgeToTraveled, edge);
            this.SEM.AddEvent(ev);
        }

        public void RemoveExploredFromPath1(Vertex vert)
        {
            Edge edgeToRemove = this.Path1.ExploredEdges.Single(e => e.VerticeTo == vert);
            this.Path1.RemoveExplored(vert);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edgeToRemove);
            this.SEM.AddEvent(ev);
        }

        public void RemoveExploredFromPath2(Vertex vert)
        {
            Edge edgeToRemove = this.Path2.ExploredEdges.Single(e => e.VerticeTo == vert);
            this.Path2.RemoveExplored(vert);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edgeToRemove);
            this.SEM.AddEvent(ev);
        }

        public void RemoveExploredFromPath1(Edge edge)
        {
            this.Path1.RemoveExplored(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edge);
            this.SEM.AddEvent(ev);
        }

        public void RemoveExploredFromPath2(Edge edge)
        {
            this.Path2.RemoveExplored(edge);

            SearchEvent ev = new SearchEvent(SearchEventType.RemovedEdgeFromExplored, edge);
            this.SEM.AddEvent(ev);
        }

        public List<Edge> GetShortestPath()
        {
            List<Edge> shortestPath = new List<Edge>();
            Edge commonEdge = this.SEM.Events.Last().ParticipantEdge;
            shortestPath.Add(commonEdge);

            if (this.Path1.TraveledEdges.Contains(commonEdge))
            {
                while (this.Path1.TraveledEdges.Any(e => e.VerticeTo == shortestPath.Last().VerticeFrom))
                {
                    var edge = this.Path1.TraveledEdges.Single(e => e.VerticeTo == shortestPath.Last().VerticeFrom);
                    shortestPath.Add(edge);
                }

                if (this.Path2.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeTo))
                {
                    var edge = this.Path2.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeTo);
                    shortestPath.Insert(0, edge);
                }

                while (this.Path2.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeFrom))
                {
                    var edge = this.Path2.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeFrom);
                    shortestPath.Insert(0, edge);
                }
            }
            else if (this.Path2.TraveledEdges.Contains(commonEdge))
            {
                while (this.Path2.TraveledEdges.Any(e => e.VerticeTo == shortestPath.Last().VerticeFrom))
                {
                    var edge = this.Path2.TraveledEdges.Single(e => e.VerticeTo == shortestPath.Last().VerticeFrom);
                    shortestPath.Add(edge);
                }

                if (this.Path1.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeTo))
                {
                    var edge = this.Path1.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeTo);
                    shortestPath.Insert(0, edge);
                }

                while (this.Path1.TraveledEdges.Any(e => e.VerticeTo == shortestPath.First().VerticeFrom))
                {
                    var edge = this.Path1.TraveledEdges.Single(e => e.VerticeTo == shortestPath.First().VerticeFrom);
                    shortestPath.Insert(0, edge);
                }
            }

            

            return shortestPath;
        }
    }
}
