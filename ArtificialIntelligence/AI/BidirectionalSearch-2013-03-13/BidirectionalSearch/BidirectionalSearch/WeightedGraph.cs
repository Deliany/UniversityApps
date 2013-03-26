using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace BidirectionalSearch
{
    // MODEL
    class WeightedGraph
    {
        /// <summary>
        /// If starts the line, detects double directed edge
        /// </summary>
        private const string doubleDirectedSign = "+";
        /// <summary>
        /// Square symmetric positive matrix
        /// </summary>
        private List<double> elements = new List<double>();
        /// <summary>
        /// Rows/columns names
        /// </summary>
        private List<string> names = new List<string>();

        /// <summary>
        /// Gets nodes count
        /// </summary>
        public int Count { get { return (int)Math.Sqrt(elements.Count); } }

        /// <summary>
        /// Loads graph data from text file
        /// </summary>
        /// <param name="fileName">File with "dds node1 node2 weight" line structure ("dds" is optional and indicates double directed edge)</param>
        /// <returns>Empty if succeded, otherwise exception message</returns>
        public string Load(string fileName)
        {
            try
            {
                // read and iterate file lines
                string[] lines = File.ReadAllLines(fileName, Encoding.Default);
                foreach (string current in lines)
                {
                    string[] components = current.Split(' ');
                    // check for optional double directed sign
                    int afterSign = components[0] == doubleDirectedSign ? 1 : 0;
                    double value; // get and write edge weight
                    if (double.TryParse(components[afterSign + 2], out value))
                    {
                        //matrix.AddValue(components[afterSign], components[afterSign + 1], value);
                        SetDistance(components[afterSign], components[afterSign + 1], value);
                        // add symmetric edge if double directed
                        if (afterSign == 1) SetDistance(components[afterSign + 1], components[afterSign], value);
                    }
                }

                // data loaded
                return "";
            }
            catch (Exception ex)
            {
                // any exception causes exit
                elements = null;
                return ex.Message;
            }
        }

        private void SetDistance(string from, string to, double value)
        {
            // grow if needed
            if (!names.Contains(from)) Add(from);
            if (!names.Contains(to)) Add(to);
            // find location and set value
            int rowIndex = names.IndexOf(from);
            int columnIndex = names.IndexOf(to);
            elements[rowIndex * Count + columnIndex] = value;
        }

        /// <summary>
        /// Direct distance between "first" and "second" vertices
        /// </summary>
        /// <param name="from">First vertex index</param>
        /// <param name="to">Second vertex index</param>
        /// <returns>Vertices distance or 0 if not exists</returns>
        public double GetDistance(int from, int to)
        {
            if (from >= 0 && from < Count && to >= 0 && to < Count)
                return elements[from * Count + to];
            return 0;
        }

        /// <summary>
        /// Adds row and column to the end
        /// </summary>
        /// <param name="title">Row/column name</param>
        private void Add(string name)
        {
            names.Add(name);
            // insert last value in every row
            for (int i = 0; i < Count; i++)
                elements.Insert((i + 1) * Count + i, 0);
            // add last row
            elements.AddRange(new double[Count + 1]);
        }

        /// <summary>
        /// Gets adjacent nodes from given node
        /// </summary>
        /// <param name="node">Node index</param>
        /// <returns>List of adjacent nodes indices</returns>
        public List<int> AdjacentFrom(int node)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < Count; i++)
                if (GetDistance(node, i) != 0) result.Add(i);
            return result;
        }

        /// <summary>
        /// Gets adjacent nodes to given node
        /// </summary>
        /// <param name="node">Node index</param>
        /// <returns>List of adjacent nodes indices</returns>
        public List<int> AdjacentTo(int node)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < Count; i++)
                if (GetDistance(i, node) != 0) result.Add(i);
            return result;
        }

        /// <summary>
        /// Gets node name with specified index
        /// </summary>
        /// <param name="nodeIndex">Node index</param>
        /// <returns>Node name</returns>
        public string Name(int nodeIndex)
        {
            return names[nodeIndex];
        }

        /// <summary>
        /// Gets node index with specified name
        /// </summary>
        /// <param name="p">Node name</param>
        /// <returns>Node index</returns>
        public int Index(string nodeName)
        {
            return names.IndexOf(nodeName);
        }
    }
}
