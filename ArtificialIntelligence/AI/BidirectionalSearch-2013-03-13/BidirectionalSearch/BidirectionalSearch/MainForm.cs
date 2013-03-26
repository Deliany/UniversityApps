using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BidirectionalSearch
{
    // VIEW
    public partial class MainForm : Form
    {
        /// <summary>
        /// Graph object
        /// </summary>
        private WeightedGraph graph = new WeightedGraph();
        /// <summary>
        /// Nodes points
        /// </summary>
        private List<Point> nodesLocation = new List<Point>();
        /// <summary>
        /// Indicates will the node be moved on MouseMove event
        /// </summary>
        private bool moveNode = false;
        /// <summary>
        /// Clicked node index
        /// </summary>
        private int selectedNode = -1;
        /// <summary>
        /// Graph search agent
        /// </summary>
        private SearchAgent agent = new SearchAgent();
        /// <summary>
        /// Found pathes to visualize
        /// </summary>
        private List<SearchAgent.Path> pathes = new List<SearchAgent.Path>();
        /// <summary>
        /// Current step index to visualize
        /// </summary>
        private int stepToShow = 0;
        /// <summary>
        /// The shortest path between two vertices
        /// </summary>
        private SearchAgent.Path result = new SearchAgent.Path();
        /// <summary>
        /// Indicates whether to show result path
        /// </summary>
        bool showResult = false;

        /// <summary>
        /// Constructs a new form
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws one-sided edge arrow
        /// </summary>
        /// <param name="g">Graphics object to draw</param>
        /// <param name="first">Arrow begin point</param>
        /// <param name="second">Arrow end point</param>
        public void DrawArrow(Graphics g, Point first, Point second)
        {
            g.DrawLine(new Pen(Color.Black), first, second);

            Point arrow = new Point(second.X, second.Y);
            if (first.X < second.X) arrow.X -= (second.X - first.X) / 8;
            else arrow.X += (first.X - second.X) / 8;
            if (first.Y < second.Y) arrow.Y -= (second.Y - first.Y) / 8;
            else arrow.Y += (first.Y - second.Y) / 8;

            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(arrow.X - 2, arrow.Y - 2, 5, 5)); 
        }

        /// <summary>
        /// Redraw image on resizing
        /// </summary>
        private void pictureBoxGraph_SizeChanged(object sender, EventArgs e)
        {
            nodesLocation.Clear();
            selectedNode = -1;
            (sender as PictureBox).Invalidate();
        }

        /// <summary>
        /// Drag graph node
        /// </summary>
        private void pictureBoxGraph_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < nodesLocation.Count; i++)
            {
                if (Math.Abs(nodesLocation[i].X - e.X) < 20 && Math.Abs(nodesLocation[i].Y - e.Y) < 20)
                {
                    selectedNode = i;
                    if (e.Button == MouseButtons.Left) toolStripTextBoxFrom.Text = graph.Name(selectedNode);
                    else if (e.Button == MouseButtons.Right) toolStripTextBoxTo.Text = graph.Name(selectedNode);
                    moveNode = true;
                    (sender as PictureBox).Invalidate();
                }
            }
        }

        /// <summary>
        /// Move graph node
        /// </summary>
        private void pictureBoxGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveNode)
            {
                nodesLocation[selectedNode] = e.Location;
                (sender as PictureBox).Invalidate();
            }
        }

        /// <summary>
        /// Drop graph node
        /// </summary>
        private void pictureBoxGraph_MouseUp(object sender, MouseEventArgs e)
        {
            moveNode = false;
            (sender as PictureBox).Invalidate();
        }

        /// <summary>
        /// Redraw graph
        /// </summary>
        private void pictureBoxGraph_Paint(object sender, PaintEventArgs e)
        {
            if (graph.Count == 0) return;
            Graphics g = e.Graphics;
            int nodeSize = 20;

            #region form nodes in levels
            List<List<int>> levels = new List<List<int>>();
            levels.Add(new List<int>() { 0 });
            List<int> used = new List<int>() { 0 };
            while (true)
            {
                List<int> adjacent = new List<int>();
                foreach (var current in levels[levels.Count - 1])
                    adjacent.AddRange(graph.AdjacentFrom(current));
                adjacent = adjacent.Except(used).ToList();
                if (adjacent.Count == 0) break;
                used.AddRange(adjacent);
                levels.Add(adjacent);
            }
            #endregion

            #region set nodes location
            List<List<LocatedNode>> located = new List<List<LocatedNode>>();
            if (nodesLocation.Count == 0)
            {
                int xStep = (sender as PictureBox).Width / (levels.Count + 1);
                for (int i = 0, xBegin = xStep; i < levels.Count; i++, xBegin += xStep)
                {
                    located.Add(new List<LocatedNode>());
                    int yStep = (sender as PictureBox).Height / (levels[i].Count + 1);
                    for (int j = 0, yBegin = yStep; j < levels[i].Count; j++, yBegin += yStep)
                    {
                        int xBegin1 = (j % 2 == 0) ? (xBegin - xStep / 4) : (xBegin + xStep / 8);
                        located[i].Add(new LocatedNode(levels[i][j], new Point(xBegin1, yBegin)));
                        nodesLocation.Add(new Point(xBegin1, yBegin));
                    }
                }
            }
            else
            {
                for (int i = 0, curr = 0; i < levels.Count; i++)
                {
                    located.Add(new List<LocatedNode>());
                    for (int j = 0; j < levels[i].Count; j++, curr++)
                    {
                        located[i].Add(new LocatedNode(levels[i][j], nodesLocation[curr]));
                    }
                }
            }
            #endregion

            #region draw edges, nodes and pathes

            // draw edges and pathes
            foreach (var levelOuter in located)
            {
                foreach (var first in levelOuter)
                {
                    foreach (var adjacent in graph.AdjacentFrom(first.node))
                    {
                        foreach (var levelInner in located)
                        {
                            foreach (var second in levelInner)
                            {
                                if (second.node == adjacent)
                                {
                                    // draw edge
                                    DrawArrow(g, first.location, second.location);

                                    // draw path through edge if exists
                                    for (int i = 0; i < pathes.Count; i++)
                                    {
                                        List<int> nodes = pathes[i].Vertices;
                                        for (int j = 1; j < nodes.Count; j++)
                                        {
                                            if ((first.node == nodes[j - 1] && second.node == nodes[j]) ||
                                                (first.node == nodes[j] && second.node == nodes[j - 1]))
                                            {
                                                g.DrawLine(new Pen(Color.Orange, 5), first.location, second.location);
                                            }
                                        }
                                    }

                                    // draw the shortest path through edge if exists
                                    if (showResult)
                                    {
                                        List<int> final = result.Vertices;
                                        for (int j = 1; j < final.Count; j++)
                                        {
                                            if ((first.node == final[j - 1] && second.node == final[j]) ||
                                                (first.node == final[j] && second.node == final[j - 1]))
                                            {
                                                g.DrawLine(new Pen(Color.Blue, 5), first.location, second.location);
                                            }
                                        }
                                    }

                                    // draw weight
                                    Point weightPosition = new Point(second.location.X, second.location.Y);
                                    if (first.location.X < second.location.X) weightPosition.X -= (second.location.X - first.location.X) / 4 + 20;
                                    else weightPosition.X += (first.location.X - second.location.X) / 4 - 20;
                                    if (first.location.Y < second.location.Y) weightPosition.Y -= (second.location.Y - first.location.Y) / 4 + 10;
                                    else weightPosition.Y += (first.location.Y - second.location.Y) / 4 - 10;

                                    g.DrawString(graph.GetDistance(first.node, second.node).ToString(),
                                        new Font("Verdana", 10), new SolidBrush(Color.DarkGreen), weightPosition);
                                }
                            }
                        }
                    }
                }
            }

            // draw nodes
            foreach (var level in located)
            {
                foreach (var node in level)
                {
                    int x = node.location.X - nodeSize / 2;
                    int y = node.location.Y - nodeSize / 2;
                    Color nodeColor = (node.node == selectedNode) ? Color.Gold : Color.Purple;
                    g.FillEllipse(new SolidBrush(nodeColor), new Rectangle(x, y, nodeSize, nodeSize));
                    g.DrawString(graph.Name(node.node), new Font("Verdana", 10), new SolidBrush(Color.Red), new Point(x + nodeSize, y));
                }
            }

            #endregion
        }

        /// <summary>
        /// Load graph from file
        /// </summary>
        private void toolStripButtonLoadGraph_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                graph = new WeightedGraph();
                showResult = false;
                string result = graph.Load(dialog.FileName);
                if (result != "") MessageBox.Show(result);
                else
                {
                    nodesLocation = new List<Point>();
                    pictureBoxGraph.Invalidate();
                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Perform bidirectional search
        /// </summary>
        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            // find the shortest path with bidirectional search
            pathes.Clear();
            showResult = false;
            pictureBoxGraph.Invalidate();
            result = agent.BidirectionalSearch(graph, graph.Index(toolStripTextBoxFrom.Text), graph.Index(toolStripTextBoxTo.Text));
            toolStripTextBoxPathDistance.Text = result.Distance.ToString();

            // start timer to visualize results
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (pathes.Count == agent.StepByStep.Count)
            {
                showResult = true;
                (sender as Timer).Stop();
            }
            else pathes.Add(agent.StepByStep[pathes.Count]);
            pictureBoxGraph.Invalidate();
        }

        /// <summary>
        /// Stores node's index and location
        /// </summary>
        private struct LocatedNode
        {
            public int node;
            public Point location;

            public LocatedNode(int node, Point location)
            {
                this.node = node;
                this.location = location;
            }
        }
    }
}
