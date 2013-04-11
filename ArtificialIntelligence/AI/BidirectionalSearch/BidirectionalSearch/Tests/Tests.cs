using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchAlgorithms.Model;
using NUnit.Framework;

namespace SearchAlgorithms
{
    [TestFixture]
    class Tests
    {
        private Graph g;
        private BidirectionalSearch br;
        private GreedySearch gs;
        [SetUp]
        public void Setup()
        {
            g = new Graph("graph_matrix.txt", "heuristic_data.txt");
            br = new Model.BidirectionalSearch();
            gs = new GreedySearch();
        }

        [Test]
        public void TestShortDistance()
        {
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).TotalCost;
            Assert.AreEqual(totalCost, 151);

            totalCost = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).TotalCost;
            Assert.AreEqual(totalCost, 151);
        }

        [Test]
        public void TestShortDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("b"), new Vertex("a")),
                };
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));

            expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("b")),
                };

            shortestPath = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestAnotherShortDistance()
        {
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).TotalCost;
            Assert.AreEqual(totalCost, 270);

            totalCost = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).TotalCost;
            Assert.AreEqual(totalCost, 270);
        }

        [Test]
        public void TestAnotherShortDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("f"), new Vertex("a")),
                };
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));

            expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("f")),
                };

            shortestPath = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestLongDistance()
        {
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).TotalCost;
            Assert.AreEqual(totalCost, 1163);

            totalCost = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).TotalCost;
            Assert.AreEqual(totalCost, 1411);
        }

        [Test]
        public void TestLongDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("d")),
                    new Edge(new Vertex("d"), new Vertex("h")),
                    new Edge(new Vertex("h"), new Vertex("j")),
                    new Edge(new Vertex("r"), new Vertex("j")),
                    new Edge(new Vertex("w"), new Vertex("r")),
                    new Edge(new Vertex("z"), new Vertex("w")),
                };
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));

            /*
             * (a,d)=134=>(d,g)=174=>(g,h)=189=>(h,j)=121=>(j,r)=429=>(r,w)=86=>(w,z)=278=>
             */
            expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("d")),
                    new Edge(new Vertex("d"), new Vertex("g")),
                    new Edge(new Vertex("g"), new Vertex("h")),
                    new Edge(new Vertex("h"), new Vertex("j")),
                    new Edge(new Vertex("j"), new Vertex("r")),
                    new Edge(new Vertex("r"), new Vertex("w")),
                    new Edge(new Vertex("w"), new Vertex("z"))
                };

            shortestPath = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestAnotherLongDistance()
        {
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).TotalCost;
            Assert.AreEqual(totalCost, 1404);

            totalCost = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).TotalCost;
            Assert.AreEqual(totalCost, 1404);
        }

        [Test]
        public void TestAnotherLongDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("c")),
                    new Edge(new Vertex("c"), new Vertex("i")),
                    new Edge(new Vertex("i"), new Vertex("k")),
                    new Edge(new Vertex("m"), new Vertex("k")),
                    new Edge(new Vertex("s"), new Vertex("m")),
                    new Edge(new Vertex("x"), new Vertex("s")),
                };
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));

            expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("c")),
                    new Edge(new Vertex("c"), new Vertex("i")),
                    new Edge(new Vertex("i"), new Vertex("k")),
                    new Edge(new Vertex("k"), new Vertex("m")),
                    new Edge(new Vertex("m"), new Vertex("s")),
                    new Edge(new Vertex("s"), new Vertex("x")),
                };

            shortestPath = gs.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }
    }
}
