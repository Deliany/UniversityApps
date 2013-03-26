using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Model
{
    public class Vertex : IEquatable<Vertex>
    {
        public string Name { get; set; }

        public Vertex(Vertex v)
        {
            this.Name = v.Name;
        }

        public Vertex(string name)
        {
            this.Name = name;
        }

        public Vertex() { }

        public static bool operator==(Vertex v1, Vertex v2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(v1, v2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)v1 == null) || ((object)v2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return v1.Name == v2.Name;
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }

        public bool Equals(Vertex other)
        {
            return other.Name == this.Name;
        }
    }
}
