using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch
{
    // CONTROLLER
    class SearchAgent
    {
        /// <summary>
        /// Search steps
        /// </summary>
        private List<Path> stepByStep = new List<Path>();

        /// <summary>
        /// Gets search steps
        /// </summary>
        public List<Path> StepByStep { get { return stepByStep; } }

        /// <summary>
        /// Stores graph path data
        /// </summary>
        public class Path
        {
            List<int> vertices = new List<int>();
            double distance = double.MaxValue;
            bool reversed = false;

            public List<int> Vertices { get { return vertices; } }

            public double Distance { get { return distance; } }

            public int First { get { return vertices[0]; } }

            public int Last { get { return vertices[vertices.Count - 1]; } }

            public bool Reversed { get { return reversed; } }

            public Path() { }

            public Path(int firstVertex, bool reversed)
            {
                vertices.Add(firstVertex);
                this.reversed = reversed;
            }

            public Path(int begin, int end, double distance, bool reversed)
            {
                vertices.Add(begin);
                vertices.Add(end);
                this.distance = distance;
                this.reversed = reversed;
            }

            public Path(Path another, int newVertex, double newDistancePart)
            {
                vertices = new List<int>(another.vertices);
                vertices.Add(newVertex);
                distance = another.distance + newDistancePart;
                reversed = another.reversed;
            }

            public Path(Path another)
            {
                vertices = new List<int>(another.vertices);
                distance = another.distance;
                reversed = another.reversed;
            }

            public int Count { get { return vertices.Count; } }

            public int this[int index] { get { return vertices[index]; } }

            public void Add(int vertex, double distance)
            {
                vertices.Add(vertex);
                if (this.distance == double.MaxValue) this.distance = 0;
                this.distance += distance;
            }

            public bool Cycled { get { return vertices.Distinct().Count() != vertices.Count; } }
        }

        /// <summary>
        /// Stores front and back pathes to some common vertex
        /// </summary>
        private class FrontAndBackPath
        {
            int commonVertex;
            Path front = new Path();
            Path back = new Path();

            public FrontAndBackPath() { }

            public FrontAndBackPath(Path path)
            {
                commonVertex = path.Last;
                if (path.Reversed) back = path;
                else front = path;
            }

            public int CommonVertex { get { return commonVertex; } }

            public bool Connected { get { return front.Vertices.Intersect(back.Vertices).Count() != 0; } }

            public Path ConnectedPath(WeightedGraph graph)
            {
                if (!Connected) return null;
                Path result = new Path(front[0], false);
                for (int i = 0; i + 1 < front.Vertices.Count; i++)
                    if (front.Vertices[i] != commonVertex)
                        result.Add(front.Vertices[i + 1], graph.GetDistance(front[i], front[i + 1]));
                for (int i = back.Vertices.IndexOf(commonVertex); i > 0; i--)
                    result.Add(back.Vertices[i - 1], graph.GetDistance(back[i - 1], back[i]));
                return result;
            }

            public double FrontDistance { get { return front.Distance; } }

            public double BackDistance { get { return back.Distance; } }

            public void SetIfBetter(Path path)
            {
                if (path.Reversed && back.Distance > path.Distance) back = path;
                else if (!path.Reversed && front.Distance > path.Distance) front = path;
            }

            public double ConnectedDistance(WeightedGraph graph)
            {
                if (!Connected) return double.MaxValue;
                double result = 0;
                for (int i = 0; i < front.Vertices.Count; i++)
                    if (front.Vertices[i] != commonVertex)
                        result += graph.GetDistance(front.Vertices[i], front.Vertices[i + 1]);
                for (int i = 0; i < back.Vertices.Count; i++)
                    if (back.Vertices[i] != commonVertex)
                        result += graph.GetDistance(back.Vertices[i], back.Vertices[i + 1]);
                return result;
            }
        }

        /// <summary>
        /// Searches for the shortest path starting at begin and finishing at end vertex
        /// </summary>
        /// <param name="graph">Weights graph</param>
        /// <param name="begin">Begin vertex index</param>
        /// <param name="end">End vertex index</param>
        /// <returns>The shortest path</returns>
        public Path BidirectionalSearch(WeightedGraph graph, int begin, int end)
        {
            // search steps
            stepByStep.Clear();
            // trivial: begin = end
            if (begin == end) return new Path(begin, end, 0, false);

            List<FrontAndBackPath> tableOfShortest = new List<FrontAndBackPath>();  // table of the shortest pathes
            List<Path> process = new List<Path>();                                  // main processing list
            FrontAndBackPath result = new FrontAndBackPath();                       // result path

            // add first and last adjacent vertices to the processing list
            foreach (int adjacent in graph.AdjacentFrom(begin))
                process.Add(new Path(begin, adjacent, graph.GetDistance(begin, adjacent), false));
            foreach (int adjacent in graph.AdjacentTo(end))
                process.Add(new Path(end, adjacent, graph.GetDistance(adjacent, end), true));

            // check if process contains wanted path
            int index = PathIndex(process, begin, end);
            if (index != -1) return process[index];

            while (process.Count != 0)
            {

                // get the shortest path and try to add it to the table
                Path theShortest = Shortest(process);
                tableOfShortest = AddToTableOfShortestPathes(tableOfShortest, theShortest);
                process.Remove(theShortest);
                stepByStep.Add(new Path(theShortest));

                // get, filter and add the next level vertices
                List<Path> nextLevel = NextLevel(graph, theShortest);
                nextLevel = FilterPathes(process, tableOfShortest, nextLevel);
                process.AddRange(nextLevel);

                // check if process contains wanted path
                index = PathIndex(nextLevel, begin, end);
                if (index != -1) return nextLevel[index];

                // try to find the shortest closed path from begin to end vertex
                // and exclude longer pathes from the processing list
                result = FindTheShortestClosedPath(graph, tableOfShortest, result);
                //if (result.Connected) process = RemoveLongerFrontAndBack(process, result);
                if (result.Connected) return CheckRemainder(result.ConnectedPath(graph), process);
            }

            return result.ConnectedPath(graph);
        }

        /// <summary>
        /// Searches for the index of the path from begin to end
        /// </summary>
        /// <param name="list">List to search in</param>
        /// <param name="begin">Begin node</param>
        /// <param name="end">End node</param>
        /// <returns>Founded index, -1 otherwise</returns>
        private int PathIndex(List<Path> list, int begin, int end)
        {
            int result = -1;
            for (int i = 0; i < list.Count; i++)
                if (list[i].Vertices.Contains(begin) && list[i].Vertices.Contains(end))
                    if (result == -1 || (list[i].Distance < list[result].Distance)) result = i;
            return result;
        }

        /// <summary>
        /// Checks process list for the better path than found
        /// </summary>
        /// <param name="result">Found path</param>
        /// <param name="process">Process list to search in</param>
        /// <returns>Given path or new founded, if better</returns>
        private Path CheckRemainder(Path result, List<Path> process)
        {
            for (int i = 0; i < process.Count; i++)
            {
                if (process[i].First == result.First &&
                    process[i].Last == result.Last &&
                    process[i].Distance < result.Distance)
                {
                    result = process[i];
                    continue;
                }
                for (int j = i + 1; j < process.Count; j++)
                {
                    if (process[i].Last == process[j].Last &&
                        process[i].First != process[j].First &&
                        process[i].Distance + process[j].Distance < result.Distance)
                    {
                        result = Connect(process[i], process[j]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Connects two pathes
        /// </summary>
        /// <param name="first">First path</param>
        /// <param name="second">Second path</param>
        /// <returns>Connected path</returns>
        private Path Connect(Path first, Path second)
        {
            Path front = (!first.Reversed) ? first : second;
            Path back = (second.Reversed) ? second : first;
            front.Add(second.Last, second.Distance);
            for (int i = second.Vertices.Count - 1; i >= 0; i--)
                front.Add(second.Vertices[i], 0);
            return front;
        }

        /// <summary>
        /// Removes pathes longer than result front and back path respectively from processing list
        /// </summary>
        /// <param name="process">Processing path list</param>
        /// <param name="result">Closed path to compare</param>
        /// <returns>Filtered processing list</returns>
        private List<Path> RemoveLongerFrontAndBack(List<Path> process, FrontAndBackPath result)
        {
            for (int i = 0; i < process.Count; i++)
            {
                if (process[i].Reversed && process[i].Distance > result.BackDistance)
                    process.RemoveAt(i--);
                else if (!process[i].Reversed && process[i].Distance > result.FrontDistance)
                    process.RemoveAt(i--);
            }
            return process;
        }

        /// <summary>
        /// Compares every closed path from table with the currently shortest path and returns the shortest
        /// </summary>
        /// <param name="graph">Weights graph</param>
        /// <param name="tableOfShortest">Path table</param>
        /// <param name="theShortest">The currently shortest path</param>
        /// <returns>The shortest path</returns>
        private FrontAndBackPath FindTheShortestClosedPath(WeightedGraph graph, List<FrontAndBackPath> tableOfShortest, FrontAndBackPath theShortest)
        {
            foreach (var current in tableOfShortest)
                if (current.ConnectedDistance(graph) < theShortest.ConnectedDistance(graph))
                    theShortest = current;
            return theShortest;
        }

        /// <summary>
        /// Removes cycled and not optimal pathes from new pathes list
        /// </summary>
        /// <param name="process">Process list</param>
        /// <param name="tableOfShortest">The shortest found pathes</param>
        /// <param name="newPathes">New pathes</param>
        /// <returns>Filtered new pathes list</returns>
        private List<Path> FilterPathes(List<Path> process, List<FrontAndBackPath> tableOfShortest, List<Path> newPathes)
        {
            for (int i = 0; i < newPathes.Count; i++)
            {
                // check for cycle
                if (newPathes[i].Cycled)
                {
                    newPathes.RemoveAt(i--);
                    continue;
                }

                // check if better path already exists in process list
                for (int j = 0; j < process.Count; j++)
                {
                    if (process[j].First == newPathes[i].First &&
                        process[j].Last == newPathes[i].Last &&
                        process[j].Distance <= newPathes[i].Distance)
                    {
                        newPathes.RemoveAt(i--);
                        break;
                    }
                }
                if (i == -1) continue;

                // check for better path in the shortest table
                for (int j = 0; j < tableOfShortest.Count; j++)
                {
                    if (tableOfShortest[j].CommonVertex == newPathes[i].Last)
                    {
                        if ((newPathes[i].Reversed && tableOfShortest[j].BackDistance <= newPathes[i].Distance) ||
                            (!newPathes[i].Reversed && tableOfShortest[j].FrontDistance <= newPathes[i].Distance))
                        {
                            newPathes.RemoveAt(i--);
                            break;
                        }
                    }
                }
                if (i == -1) continue;

                
            }

            // filter process list
            for (int i = 0; i < process.Count; i++)
            {
                for (int j = 0; j < newPathes.Count; j++)
                {
                    if (process[i].First == newPathes[j].First &&
                        process[i].Last == newPathes[j].Last &&
                        process[i].Distance >= newPathes[j].Distance)
                    {
                        process.RemoveAt(i--);
                        break;
                    }
                }
            }

            return newPathes;
        }

        /// <summary>
        /// Adds new or updates an existing path in the shortest pathes table if the path is better
        /// </summary>
        /// <param name="table">Table of the shortest pathes</param>
        /// <param name="path">Path to process</param>
        /// <returns>Processed table</returns>
        private List<FrontAndBackPath> AddToTableOfShortestPathes(List<FrontAndBackPath> table, Path path)
        {
            // update if found
            foreach (var current in table)
            {
                if (current.CommonVertex == path.Last)
                {
                    current.SetIfBetter(path);
                    return table;
                }
            }
            // add otherwise
            table.Add(new FrontAndBackPath(path));
            return table;
        }

        /// <summary>
        /// Gets pathes with all adjacent vertices to the "path" last vertex
        /// </summary>
        /// <param name="graph">Graph to get data from</param>
        /// <param name="path">Base path to be continued</param>
        /// <returns>Pathes with one more last vertex</returns>
        private List<Path> NextLevel(WeightedGraph graph, Path path)
        {
            List<Path> nextLevel = new List<Path>();
            if (path.Reversed)
            {
                foreach (int next in graph.AdjacentTo(path.Last))
                    nextLevel.Add(new Path(path, next, graph.GetDistance(next, path.Last)));
            }
            else
            {
                foreach (int next in graph.AdjacentFrom(path.Last))
                    nextLevel.Add(new Path(path, next, graph.GetDistance(path.Last, next)));
            }
            return nextLevel;
        }

        /// <summary>
        /// Finds the shortest path from list
        /// </summary>
        /// <param name="list">Path list</param>
        /// <returns>The shortest path</returns>
        private Path Shortest(List<Path> list)
        {
            Path theShortest = new Path(0, 0, double.MaxValue, false);
            foreach (Path current in list)
                if (current.Distance < theShortest.Distance)
                    theShortest = current;
            return theShortest;
        }
    }
}
