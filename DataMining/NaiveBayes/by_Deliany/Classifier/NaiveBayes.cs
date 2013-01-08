using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Classifiers;
using SerializableDictionary;

namespace Bayes
{
    public class NaiveBayes : Classifier
    {
        /// <summary>
        /// Calculate the probability for each category,
        /// and will determine which one is the largest
        /// and whether it exceeds the next largest by more than its threshold
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="defaultCat">Default category</param>
        /// <returns>Category which item mostly belongs to</returns>
        public string Classify(string item, string defaultCat = "unknown")
        {
            SerializableDictionary<string, double> probs = new SerializableDictionary<string, double>();
            string best = "";
            string possible = "";

            // Find the category with highest probability
            double max = 0.0;
            foreach (var category in Categories())
            {
                probs.Add(category, Probability(item, category));
                if (probs[category] > max)
                {
                    max = probs[category];
                    best = category;
                }
            }

                // Find the second suitable category
                if (probs.ContainsKey(best))
                {
                    probs.Remove(best);
                }
                max = 0.0;
                foreach (var category in probs)
                {
                    if (category.Value > max)
                    {
                        max = category.Value;
                        possible = category.Key;
                    }
                }
            probs.Add(best, Probability(item, best));
            

            // Make sure the probability exceeds threshould*next best
            foreach (var cat in probs)
            {
                if (cat.Key == best)
                {
                    continue;
                }
                if (cat.Value * GetThreshold(best) > probs[best])
                {
                    return defaultCat;
                }
            }
            return best + (possible.Length > 0 ? (" or " + possible) : "");
        }

        /// <summary>
        /// Calculate the entire document probability
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="category">Category</param>
        /// <returns>Probability</returns>
        protected double DocumentProbability(string item, string category)
        {
            SerializableDictionary<string, int> features = GetFeatures(item);

            // Multiply the probabilites of all the features together
            double p = 1;
            foreach (var feature in features)
            {
                p *= WeightedProbability(feature.Key, category);
            }
            return p;
        }

        /// <summary>
        /// Calculates the probability of the category
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="category">Category</param>
        /// <returns>Probability</returns>
        public double Probability(string item, string category)
        {
            double categoryProbability = (double)CategoryCount(category) / TotalCount();
            double documentProbability = DocumentProbability(item, category);

            return documentProbability * categoryProbability;
        }

        public NaiveBayes()
            : base()
        {

        }
    }
}
