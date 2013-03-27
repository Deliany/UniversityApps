using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class Edge
    {
        public Vertex VerticeFrom { get; set; }
        public Vertex VerticeTo { get; set; }

        public Double Weight { get; set; }

        public Edge(Vertex vertFrom, Vertex vertTo, Double weight)
        {
            this.VerticeFrom = vertFrom;
            this.VerticeTo = vertTo;
            this.Weight = weight;
        }

        public Edge(Edge edge, Double weight)
        {
            this.VerticeFrom = edge.VerticeFrom;
            this.VerticeTo = edge.VerticeTo;
            this.Weight = weight;
        }

        public static bool operator ==(Edge e1, Edge e2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(e1, e2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)e1 == null) || ((object)e2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return e1.VerticeFrom == e2.VerticeFrom && e1.VerticeTo == e2.VerticeTo;
        }

        public static bool operator !=(Edge e1, Edge e2)
        {
            return !(e1 == e2);
        }

        public bool Equals(Edge other)
        {
            return this.VerticeFrom == other.VerticeFrom && this.VerticeTo == other.VerticeTo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (VerticeFrom != null ? VerticeFrom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VerticeTo != null ? VerticeTo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Weight;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})={2}", this.VerticeFrom, this.VerticeTo,this.Weight);
        }
    }
}
