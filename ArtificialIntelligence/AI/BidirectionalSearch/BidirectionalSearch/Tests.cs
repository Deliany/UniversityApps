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
            g = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
        }
        [Test]
        public void TestShortDistance()
        {
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            int totalCost = br.TrueSearche(g, new Vertex("a"), new Vertex("b"));
            Assert.AreEqual(totalCost, 151);
        }

        [Test]
        public void TestAnotherShortDistance()
        {
            Graph g = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            int totalCost = br.TrueSearche(g, new Vertex("a"), new Vertex("f"));
            Assert.AreEqual(totalCost, 270);
        }

        [Test]
        public void TestLongDistance()
        {
            Graph g = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            int totalCost = br.TrueSearche(g, new Vertex("a"), new Vertex("z"));
            Assert.AreEqual(totalCost, 1335);
        }

        [Test]
        public void TestAnotherLongDistance()
        {
            Graph g = new Graph("C:\\Users\\Deliany\\Desktop\\AI\\graph_matrix.txt");
            Model.BidirectionalSearch br = new Model.BidirectionalSearch();
            int totalCost = br.TrueSearche(g, new Vertex("a"), new Vertex("x"));
            Assert.AreEqual(totalCost, 1546);
        }
    }
}
