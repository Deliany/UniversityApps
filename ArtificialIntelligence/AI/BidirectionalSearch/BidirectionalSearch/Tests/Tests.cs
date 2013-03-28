using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BidirectionalSearch.Model;
using NUnit.Framework;

namespace BidirectionalSearch
{
    [TestFixture]
    class Tests
    {
        private Graph g;
        [SetUp]
        public void Setup()
        {
            g = new Graph("graph_matrix.txt");
        }

        [Test]
        public void TestShortDistance()
        {
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).TotalCost;
            Assert.AreEqual(totalCost, 151);
        }

        [Test]
        public void TestShortDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("b"), new Vertex("a"), 150),
                };
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("b")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestAnotherShortDistance()
        {
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).TotalCost;
            Assert.AreEqual(totalCost, 270);
        }

        [Test]
        public void TestAnotherShortDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("f"), new Vertex("a"), 150),
                };
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("f")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestLongDistance()
        {
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).TotalCost;
            Assert.AreEqual(totalCost, 1163);
        }

        [Test]
        public void TestLongDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("d"), 134),
                    new Edge(new Vertex("d"), new Vertex("h"), 249),
                    new Edge(new Vertex("h"), new Vertex("j"), 370),
                    new Edge(new Vertex("r"), new Vertex("j"), 793),
                    new Edge(new Vertex("w"), new Vertex("r"), 364),
                    new Edge(new Vertex("z"), new Vertex("w"), 278),
                };
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("z")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }

        [Test]
        public void TestAnotherLongDistance()
        {
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            Double totalCost = br.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).TotalCost;
            Assert.AreEqual(totalCost, 1404);
        }

        [Test]
        public void TestAnotherLongDistancePath()
        {
            List<Edge> expectedEdges = new List<Edge>
                {
                    new Edge(new Vertex("a"), new Vertex("c"), 210),
                    new Edge(new Vertex("c"), new Vertex("i"), 400),
                    new Edge(new Vertex("i"), new Vertex("k"), 542),
                    new Edge(new Vertex("m"), new Vertex("k"), 862),
                    new Edge(new Vertex("s"), new Vertex("m"), 526),
                    new Edge(new Vertex("x"), new Vertex("s"), 337),
                };
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            List<Edge> shortestPath = br.SynchronousSearch(g, new Vertex("a"), new Vertex("x")).GetShortestPath();
            Assert.That(expectedEdges, Is.EquivalentTo(shortestPath));
        }
    }
}
