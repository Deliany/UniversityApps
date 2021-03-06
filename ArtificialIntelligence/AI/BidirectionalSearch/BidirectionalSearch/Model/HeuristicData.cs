﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchAlgorithms.Model
{
    public class HeuristicData
    {
        public List<Vertex> Vertices { get; protected set; }
        public List<Edge> Edges { get; protected set; }

        /// <summary>
        /// Initializes instance of HeuristicData from file
        /// File must contain data in specific format, read README for more
        /// Example:  a b c
        ///         a 0 1 2
        ///         b 1 0 3
        ///         c 2 3 0
        /// </summary>
        /// <param name="filePath"></param>
        public HeuristicData(string filePath) : this()
        {
            this.LoadHeuristicDataFromFile(filePath);
        }

        public HeuristicData()
        {
            this.Edges = new List<Edge>();
            this.Vertices = new List<Vertex>();
        }

        public void LoadHeuristicDataFromFile(string filePath, bool forceZeros = false)
        {
            Vertices.Clear();
            Edges.Clear();

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; ++i)
            {
                // replace all spaces, tabs and other whitespace to single space
                lines[i] = Regex.Replace(lines[i], @"\s+", " ").Trim();

                // first line in file must be vertices names separated by whitespace
                if (i == 0)
                {
                    string[] verticeNames = lines[i].Split(' ');
                    foreach (var vertName in verticeNames)
                    {
                        Vertex v = new Vertex { Name = vertName };
                        Vertices.Add(v);
                    }
                }
                else
                {
                    string[] data = lines[i].Split(' ');
                    Vertex vertFrom = new Vertex { Name = data[0] };

                    for (int j = 1; j < data.Length; j++)
                    {
                        Double weight;
                        if (!Double.TryParse(data[j], out weight))
                        {
                            throw new Exception(
                                string.Format("Number at [{0},{1}] position is in bad format, number expected", i, j));
                        }

                        if (weight != 0 || forceZeros)
                        {
                            Edge edge = new Edge(new Vertex(vertFrom), Vertices[j - 1], weight);
                            Edges.Add(edge);
                        }
                    }
                }
            }
        }

        public Double[,] AsMatrix()
        {
            int dimension = Vertices.Count;
            Double[,] matrix = new Double[dimension,dimension];
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (EdgeExists(Vertices[i], Vertices[j]))
                    {
                        matrix[i, j] =
                            Edges.Single(edge => edge.VerticeFrom == Vertices[i] && edge.VerticeTo == Vertices[j]).Weight;
                    }
                    
                }
            }

            return matrix;
        }

        public Double GetEdgeWeight(Vertex vertFrom, Vertex vertTo)
        {
            return Edges.Single(edge => edge.VerticeFrom == vertFrom && edge.VerticeTo == vertTo).Weight;
        }

        public bool EdgeExists(Vertex vertFrom, Vertex vertTo)
        {
            return Edges.Any(edge => edge.VerticeFrom == vertFrom && edge.VerticeTo == vertTo);
        }

        public Edge GetEdge(Vertex vertFrom, Vertex vertTo)
        {
            return !this.EdgeExists(vertFrom,vertTo) ? null : Edges.Single(edge => edge.VerticeFrom == vertFrom && edge.VerticeTo == vertTo);
        }

        public List<Edge> AdjacentEdges(Vertex v)
        {
            return new List<Edge>(Edges.Where(edge => edge.VerticeFrom == v));
        } 

        public List<Vertex> AdjacentVertices(Vertex v)
        {
            List<Edge> adjacentEdges = this.AdjacentEdges(v);

            return adjacentEdges.Select(edge => edge.VerticeTo).ToList();
        } 

    }
}
