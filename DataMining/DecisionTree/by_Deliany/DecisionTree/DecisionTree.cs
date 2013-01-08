using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DividedSet = System.Collections.Generic.KeyValuePair<System.Collections.Generic.List<string[]>, System.Collections.Generic.List<string[]>>;
using Set = System.Collections.Generic.List<string[]>;
using Pair = System.Collections.Generic.KeyValuePair<int, string>;

namespace by_Deliany
{
    public class DecisionNode
    {
        /// <summary>
        /// Column index of the criteria to be tested
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The value that the column must match to get a true result
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Stores a dictionary of results for this branch.
        /// This is null for everything except endpoints
        /// </summary>
        public Dictionary<string, double> Results { get; set; }

        /// <summary>
        /// The next nodes in the tree if the result is true or false, respectively
        /// </summary>
        public DecisionNode NextTrueNode { get; set; }

        public DecisionNode NextFalseNode { get; set; }

        /// <summary>
        /// Creates instance of DecisionNode class 
        /// </summary>
        public DecisionNode(int column = -1, string value = null, Dictionary<string, double> results = null,
                            DecisionNode nextTrueNode = null, DecisionNode nextFalseNode = null)
        {
            Column = column;
            Value = value;
            Results = results;
            NextTrueNode = nextTrueNode;
            NextFalseNode = nextFalseNode;
        }  
    }

    public class DecisionTreeTools
    {
        /// <summary>
        ///  metrics for measuring how mixed a set is
        /// </summary>
        public enum MeasuringMetric
        {
            GiniImpurity,
            Entropy,
            Variance
        };

        private delegate bool Function(string[] data);

        /// <summary>
        /// Divides a set on a specific column.
        /// Can handle numeric or nominal values
        /// </summary>
        public static DividedSet DivideSet(Set rows, int column, string value)
        {
            // Make a function that tells us if a row is in
            // the first group (true) or the second group (false)
            Function split_function;

            #region determine split_function

            double res;
            if (double.TryParse(value, out res))
            {
                // split function will compare numerical data
                split_function = data =>
                {
                    double result;
                    if (double.TryParse(data[column], out result))
                    {
                        return result >= Convert.ToDouble(value);
                    }

                    return false;
                };
            }
            else
            {
                // split function will compare text data
                split_function = data => data[column] == value;
            }


            #endregion

            // Divide the rows into two sets and return them
            Set set1 = (from row in rows
                        where split_function(row)
                        select row).ToList();
            Set set2 = (from row in rows
                        where !split_function(row)
                        select row).ToList();

            return new DividedSet(set1, set2);
        }

        /// <summary>
        /// Create counts of possible results (the last column of each row is the result)
        /// </summary>
        public static Dictionary<string, double> UniqueCounts(Set rows)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();

            // finds all the different possible outcomes and returns them as a dictionary
            // of how many times they each appear
            foreach (var row in rows)
            {
                string data = row[row.Length - 1];
                if (!results.ContainsKey(data))
                {
                    results.Add(data, 0);
                }
                results[data] += 1;
            }
            return results;
        }

        /// <summary>
        /// Gini impurity is the expected error rate if one of the results from a set is randomly
        /// applied to one of the items in the set.
        /// Returns probability that a randomly placed item will be in the wrong category
        /// </summary>
        public static double GiniImpurity(Set rows)
        {
            int Total = rows.Count;
            var counts = UniqueCounts(rows);
            double imp = 0;

            // calculates the probability of each possible outcome by dividing the
            // number of times that outcome occurs by the total number of rows in the set
            foreach (var k1 in counts)
            {
                double p1 = (double)k1.Value / Total;

                foreach (var k2 in counts)
                {
                    if (k1.Key == k2.Key && k1.Value == k2.Value)
                    {
                        continue;
                    }
                    double p2 = (double)k2.Value / Total;

                    imp += p1 * p2;
                }
            }

            // The higher this probability, the worse the split
            // A probability of zero is great because it tells you that everything is
            // already in the right set
            return imp;
        }

        /// <summary>
        /// Entropy, in information theory, is the amount of disorder in a set—basically, how mixed a set is
        /// Entropy is the sum of p(x)log(p(x)) across all the different possible results
        /// </summary>
        public static double Entropy(Set rows)
        {
            var results = UniqueCounts(rows);

            // Now calculate the entropy
            double entropy = 0;

            // applies these formulas:
            // p(i) = frequency(outcome) = count(outcome) / count(total rows)
            // Entropy = sum of p(i) x log(p(i)) for all outcomes
            foreach (var r in results)
            {
                double p = (double)r.Value / rows.Count;
                entropy = entropy - p * Math.Log(p, 2);
            }

            // This is a measurement of how different the outcomes are from each other. If they’re
            // all the same (e.g., if you were really lucky and everyone became a premium subscriber),
            // then the entropy is 0

            //The more mixed up the groups are, the higher their entropy
            return entropy;
        }

        private delegate double Fun(Set rows);

        /// <summary>
        /// Builds the tree by choosing the best dividing criteria for the current set
        /// </summary>
        public static DecisionNode BuildTree(Set rows, MeasuringMetric mode = MeasuringMetric.Entropy)
        {
            Fun scoreCalc = Entropy;
            switch (mode)
            {
                case MeasuringMetric.Entropy:
                    scoreCalc = Entropy;
                    break;
                case MeasuringMetric.GiniImpurity:
                    scoreCalc = GiniImpurity;
                    break;
                case MeasuringMetric.Variance:
                    scoreCalc = Variance;
                    break;
            }

            if (rows.Count == 0)
            {
                return new DecisionNode();
            }
            double currentScore = scoreCalc(rows);


            // Set up some variables to track the best criteria
            double bestGain = 0;
            Pair bestCriteria = new Pair();
            DividedSet bestSets = new DividedSet();

            int columnCount = rows[0].Length - 1;
            for (int col = 0; col < columnCount; ++col)
            {
                // Generate the list of different values in this column
                List<string> columnValues = new List<string>();

                foreach (var row in rows)
                {
                    if (!columnValues.Contains(row[col]))
                    {
                        columnValues.Add(row[col]);
                    }
                }

                // Now try dividing the rows up for each value in this column
                foreach (var value in columnValues)
                {
                    DividedSet divSet = DivideSet(rows, col, value);

                    // Information gain
                    double p = (double)divSet.Key.Count / rows.Count;
                    double gain = currentScore - p * scoreCalc(divSet.Key) - (1 - p) * scoreCalc(divSet.Value);


                    if (gain > bestGain && divSet.Key.Count > 0 && divSet.Value.Count > 0)
                    {
                        bestGain = gain;
                        bestCriteria = new Pair(col, value);
                        bestSets = divSet;
                    }
                }
            }

            // Create the subbranches
            if (bestGain > 0)
            {
                DecisionNode trueBranch = BuildTree(bestSets.Key);
                DecisionNode falseBranch = BuildTree(bestSets.Value);
                return new DecisionNode(column: bestCriteria.Key, value: bestCriteria.Value, nextTrueNode: trueBranch,
                                        nextFalseNode: falseBranch);
            }
            return new DecisionNode(results: UniqueCounts(rows));
        }

        public static StringBuilder PrintTree(DecisionNode tree, string indent = "")
        {
            StringBuilder str = new StringBuilder();
            // Is this a leaf node?
            if (tree.Results != null)
            {
                foreach (var res in tree.Results)
                {
                    str.Append("{'" + res.Key + "': " + res.Value + "}");
                }
                str.Append("\n");
            }
            else
            {
                // Print the criteria
                str.Append(tree.Column + ":" + tree.Value + "?" + "\n");

                // Print the branches
                str.Append(indent + "T->");
                str.Append(PrintTree(tree.NextTrueNode, indent + "\t"));
                str.Append(indent + "F->");
                str.Append(PrintTree(tree.NextFalseNode, indent + "\t"));
            }
            return str;
        }

        /// <summary>
        /// takes a new observation and classifies it according to the decision tree
        /// </summary>
        public static Dictionary<string, double> Classify(string[] observation, DecisionNode tree)
        {
            if (tree.Results != null)
            {
                return tree.Results;
            }
            else
            {
                string data = observation[tree.Column];
                DecisionNode branch;

                double res;
                if (double.TryParse(data, out res))
                {
                    branch = res >= Convert.ToDouble(tree.Value) ? tree.NextTrueNode : tree.NextFalseNode;
                }
                else
                {
                    branch = data == tree.Value ? tree.NextTrueNode : tree.NextFalseNode;
                }
                return Classify(observation, branch);
            }
        }

        /// <summary>
        /// When this function is called on the root node, it will traverse all the way down the
        /// tree to the nodes that only have leaf nodes as children. It will create a combined list
        /// of results from both of the leaves and will test the entropy. If the change in entropy is
        /// less than the mingain parameter, the leaves will be deleted and all their results moved
        /// to their parent node. The combined node then becomes a possible candidate for
        /// deletion and merging with another node.
        /// </summary>
        public static void Prune(DecisionNode tree, double mingain)
        {
            // If the branches aren't leaves, then prune them
            if (tree.NextTrueNode.Results == null)
            {
                Prune(tree.NextTrueNode, mingain);
            }
            if (tree.NextFalseNode.Results == null)
            {
                Prune(tree.NextFalseNode, mingain);
            }

            // If both the subbranches are now leaves, see if they should merged
            if (tree.NextTrueNode.Results != null && tree.NextFalseNode.Results != null)
            {
                // Build a combined dataset
                Set tb = new Set(), fb = new Set();
                foreach (var s in tree.NextTrueNode.Results)
                {
                    for (int i = 0; i < s.Value; i++)
                    {
                        tb.Add(new string[] { s.Key });
                    }
                }

                foreach (var s in tree.NextFalseNode.Results)
                {
                    for (int i = 0; i < s.Value; i++)
                    {
                        fb.Add(new string[] { s.Key });
                    }
                }

                // Test the reduction in entropy
                double delta = Entropy(tb.Union(fb).ToList()) - (Entropy(tb) + Entropy(fb) / 2);
                if (delta < mingain)
                {
                    // Merge the branches
                    tree.NextTrueNode = null;
                    tree.NextFalseNode = null;
                    tree.Results = UniqueCounts(tb.Union(fb).ToList());
                }
            }
        }

        /// <summary>
        /// The only difference is at the end where, if the important piece of data is missing, the
        /// results for each branch are calculated and then combined with their respective
        /// weightings.
        /// </summary>
        public static Dictionary<string, double> MissingDataClassify(string[] observation, DecisionNode tree)
        {
            if (tree.Results != null)
            {
                return tree.Results;
            }
            else
            {
                string v = observation[tree.Column];

                if (v == null)
                {
                    var tr = MissingDataClassify(observation, tree.NextTrueNode);
                    var fr = MissingDataClassify(observation, tree.NextFalseNode);
                    double tcount = tr.Values.Sum();
                    double fcount = fr.Values.Sum();
                    double tw = (double)tcount / (tcount + fcount);
                    double fw = (double)fcount / (tcount + fcount);

                    Dictionary<string, double> result = new Dictionary<string, double>();
                    foreach (var i in tr)
                    {
                        if (!result.ContainsKey(i.Key))
                        {
                            result.Add(i.Key, i.Value * tw);
                        }
                        else
                        {
                            result[i.Key] = i.Value * tw;
                        }
                    }
                    foreach (var i in fr)
                    {
                        if (!result.ContainsKey(i.Key))
                        {
                            result.Add(i.Key, i.Value * fw);
                        }
                        else
                        {
                            result[i.Key] = i.Value * fw;
                        }
                    }
                    return result;
                }
                else
                {
                    DecisionNode branch;

                    double res;
                    if (double.TryParse(v, out res))
                    {
                        branch = res >= Convert.ToDouble(tree.Value) ? tree.NextTrueNode : tree.NextFalseNode;
                    }
                    else
                    {
                        branch = v == tree.Value ? tree.NextTrueNode : tree.NextFalseNode;
                    }
                    return MissingDataClassify(observation, branch);
                }
            }
        }

        public static double Variance(Set rows)
        {
            if (rows.Count == 0)
            {
                return 0;
            }
            List<double> data = new List<double>();
            foreach (var row in rows)
            {
                data.Add(Convert.ToDouble(row[row.Length - 1]));
            }
            double mean = data.Sum() / data.Count;
            double variance = data.Sum(d => Math.Pow(d - mean, 2)) / data.Count;
            return variance;
        }
    }
}
