using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchAlgorithms.Model
{
    public class ProblemSolvingAgent
    {
        public List<Edge> seq { get; private set; }
        public State state { get; private set; }
        public Vertex goal { get; private set; }

        private bool formulateGoalsIndefinitely = true;
        private int maxGoalsToFormulate = 1;
        private int goalsFormulated = 0;

        public ProblemSolvingAgent()
        {
            this.formulateGoalsIndefinitely = true;

            seq = new List<Edge>();
            state = new State();
        }

        public ProblemSolvingAgent(int maxGoalsToFormulate)
        {
            this.formulateGoalsIndefinitely = false;
            this.maxGoalsToFormulate = maxGoalsToFormulate;

            seq = new List<Edge>();
            state = new State();
        }

        public class State
        {
            public Graph Graph { get; set; }
            public Vertex Current { get; set; }
        }

        public class Problem
        {
            public ProblemType Type { get; set; }
            public Graph Graph { get; set; }
            public Vertex Root { get; set; }
            public Vertex Goal { get; set; }
        }

        public enum ProblemType
        {
            SearchShortestPathProblem
        }

        public class Percept
        {
            public Graph Graph { get; set; }
            public Vertex Current { get; set; }
        }





        public Edge execute(Percept p)
        {
            Edge action = null;

            // state <- UPDATE-STATE(state, percept)
            updateState(p);
            // if seq is empty then do
            if (seq.Count == 0)
            {
                if (formulateGoalsIndefinitely || goalsFormulated < maxGoalsToFormulate)
                {
                    // goal <- FORMULATE-GOAL(state)
                    Vertex goal = formulateGoal();
                    goalsFormulated++;
                    // problem <- FORMULATE-PROBLEM(state, goal)
                    Problem problem = formulateProblem(state, goal);
                    // seq <- SEARCH(problem)
                    seq.AddRange(search(problem));
                    if (seq.All(e => e == null))
                    {
                        // Unable to identify a path
                    }
                }
                else
                {
                    // Agent no longer wishes to
                    // achieve any more goals
                }
            }

            if (seq.Count > 0)
            {
                // action <- FIRST(seq)
                action = seq.First();
                // seq <- REST(seq)
                seq = seq.Skip(1).ToList();
            }

            return action;
        }

        public void updateState(Percept c)
        {
            this.state.Graph = c.Graph;
            this.state.Current = c.Current;
        }

        public Vertex formulateGoal()
        {
            Random r = new Random();
            return this.state.Graph.Vertices[r.Next(this.state.Graph.Vertices.Count)];
        }

        public Problem formulateProblem(State state, Vertex goal)
        {
            return new Problem
                {
                    Type = ProblemType.SearchShortestPathProblem,
                    Root = state.Current,
                    Goal = goal,
                    Graph = state.Graph
                };
        }

        public List<Edge> search(Problem problem)
        {
            switch (problem.Type)
            {
                case ProblemType.SearchShortestPathProblem:
                    {
                        BidirectionalSearch bs = new BidirectionalSearch();
                        return bs.SynchronousSearch(problem.Graph, problem.Root, problem.Goal).GetShortestPath();
                    }
            }
            return null;
        }
    }
}
