using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class Path
    {
        private List<Edge> edges = new List<Edge>();

        public List<Edge> Edges { get { return edges; } } 

        public void Add(Edge edge)
        {
            edges.Add(edge);
        }

        public void Remove(Vertex vert)
        {
            edges.Remove(edges.Single(e => e.VerticeTo == vert));
        }
    }
}
