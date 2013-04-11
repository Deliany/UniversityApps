using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchAlgorithms.Model
{
    public class Graph : HeuristicData
    {
        public HeuristicData HeuristicData { get; protected set; }

        /// <summary>
        /// Initializes instance of Graph from file
        /// File must contain data in specific format, read README for more
        /// Example:  a b c
        ///         a 0 1 2
        ///         b 1 0 3
        ///         c 2 3 0
        /// </summary>
        /// <param name="filePath"></param>
        public Graph(string graphMatrixPath, string heuristicDataPath=null) : this()
        {
            this.LoadGraphFromFile(graphMatrixPath);
            if (heuristicDataPath != null)
            {
                this.HeuristicData.LoadHeuristicDataFromFile(heuristicDataPath, true);   
            }
        }

        public Graph()
        {
            this.Edges = new List<Edge>();
            this.Vertices = new List<Vertex>();
            this.HeuristicData = new HeuristicData();
        }

        public void LoadGraphFromFile(string filePath)
        {
            this.LoadHeuristicDataFromFile(filePath);
        }
    }
}
