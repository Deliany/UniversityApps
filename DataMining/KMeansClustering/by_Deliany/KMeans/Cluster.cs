using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    public class Cluster
    {
        public List<double> Centroid { get; set; }
        public List<Data> Points { get; set; } 

        public Cluster()
        {
            Centroid = new List<double>();
            Points = new List<Data>();
        }
    }
}
