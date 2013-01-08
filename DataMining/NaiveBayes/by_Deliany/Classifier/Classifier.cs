using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using SerializableDictionary;

namespace Classifiers
{
    [XmlRoot("Features")]
    public class SerializableKeyValuePair<TKey, TValue> : IComparer<SerializableKeyValuePair<TKey, TValue>>
        where TKey : IComparable
        where TValue : IComparable
    {
        private TKey _key;
        private TValue _value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
        public SerializableKeyValuePair()
        {
            Key = default(TKey);
            Value = default(TValue);
        }
        [XmlAttribute("Word")]
        public TKey Key
        {
            get { return _key; }
            set { _key = value; }
        }
        [XmlAttribute("Category")]
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public int Compare(SerializableKeyValuePair<TKey, TValue> x, SerializableKeyValuePair<TKey, TValue> y)
        {
            int val1 = x.Key.CompareTo(y.Key);
            int val2 = x.Value.CompareTo(y.Value);
            return val1.CompareTo(val2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (this.GetType() != obj.GetType()) return false;

            // safe because of the GetType check
            SerializableKeyValuePair<TKey, TValue> pair = (SerializableKeyValuePair<TKey, TValue>)obj;

            // use this pattern to compare reference members
            if (!Object.Equals(Key, pair.Key)) return false;

            // use this pattern to compare value members
            if (!Value.Equals(pair.Value)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode() ^ this.Value.GetHashCode();
        }
    }
    /// <summary>
    /// This is simple implementation of Naive Bayes classifier.
    /// Classifier needs features to use for classifying different items.
    /// Feature - is anything that you can determine as being present or absent in the item.
    /// In this implementation, items = documents, features = words in document
    /// </summary>
    [Serializable]
    public class Classifier
    {
        #region common words
        // words that don't make sense in our goal
        private static string[] MostCommonWords =
            {
                "the",
                "of",
                "and",
                "a",
                "to",
                "in",
                "is",
                "you",
                "that",
                "was",
                "for",
                "on",
                "are",
                "as",
                "with",
                "his",
                "they",
                "i", "at",
                "be",
                "this",
                "have",
                "from",
                "or",
                "one",
                "had",
                "by",
                "but",
                "not",
                "what",
                "all",
                "were",
                "we",
                "when",
                "your",
                "said", "there",
                "use",
                "an",
                "each",
                "which",
                "do",
                "how",
                "their",
                "if",
                "out",
                "then",
                "them",
                "these",
                "so", "some",
                "would",
                "into",
                "has",
                "two",
                "more",
                "no",
                "could",
                "my",
                "than",
                "been",
                "who",
                "its",
                "it's",
                "now",
                "did",
                "made",
                "may",
                "about",
                "also",
                "either"
            };
        #endregion

        /// <summary>
        /// Extract features(words) from the text
        /// </summary>
        /// <param name="text">Some text</param>
        /// <returns>Array of pairs: word-frequency</returns>
        protected SerializableDictionary<string, int> GetFeatures(string text)
        {
            // Preprocessing

            List<string> words = new List<string>();
            Regex re = new Regex(@"[a-zA-Z]\w+", RegexOptions.Compiled);

            Match m = re.Match(text);
            while (m.Success)
            {
                words.Add(m.Groups[0].Value.ToLower());
                m = m.NextMatch();
            }

            // array of unique words from text
            string[] uniqueWords = words.Distinct().ToArray();

            // associative array, where key - word, value - frequency of appearence in text
            var features = new SerializableDictionary<string, int>();

            foreach (var word in uniqueWords)
            {
                // check for semantic words (rejects letters or conjunctors)
                if (word.Length > 2 && !MostCommonWords.Contains(word))
                {
                    // count the number of occurrences
                    int count = new Regex(word, RegexOptions.IgnoreCase).Matches(text).Count;
                    features.Add(word, count);
                }
            }

            return features;
        }




        /// <summary>
        /// Counts of feature/category combination
        /// </summary>
        [XmlElement("FeaturesCountInEachCategory")]
        public SerializableDictionary<SerializableKeyValuePair<string, string>, int> FeatureCategoryCombinations;

        /// <summary>
        ///  Counts of documents in each category
        /// </summary>
        [XmlElement("ItemsCountInEachCategory")]
        public SerializableDictionary<string, int> ItemsCountInCategory;

        /// <summary>
        /// Minimum threshold for each category
        /// </summary>
        [XmlElement("ThresholdsForEachCategory")]
        public SerializableDictionary<string, double> Thresholds;


        /// <summary>
        /// Creates an empty instance of classifier
        /// </summary>
        public Classifier()
        {
            FeatureCategoryCombinations = new SerializableDictionary<SerializableKeyValuePair<string, string>, int>();
            ItemsCountInCategory = new SerializableDictionary<string, int>();
            Thresholds = new SerializableDictionary<string, double>();
        }

        /// <summary>
        /// Increase the count of a feature/category pair
        /// </summary>
        /// <param name="feature">Pair: feature and its frequence</param>
        /// <param name="category">Category</param>
        protected void IncreaseFeatures(SerializableKeyValuePair<string, int> feature, string category)
        {
            SerializableKeyValuePair<string, string> pair = new SerializableKeyValuePair<string, string>(feature.Key, category);

            if (!FeatureCategoryCombinations.ContainsKey(pair))
            {
                FeatureCategoryCombinations.Add(pair, feature.Value);
            }
            else
            {
                FeatureCategoryCombinations[pair] += feature.Value;
            }
        }

        /// <summary>
        /// Increase the count of a category
        /// </summary>
        /// <param name="category">Category</param>
        protected void IncreaseCategories(string category)
        {
            if (!ItemsCountInCategory.ContainsKey(category))
            {
                ItemsCountInCategory.Add(category, 1);
                Thresholds.Add(category, 1);
                return;
            }

            // just increment items count in category
            ItemsCountInCategory[category] += 1;
        }

        /// <summary>
        /// The number of times a feature has appeared in a category
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <param name="category">Category</param>
        /// <returns>Count of specified feature in specified category</returns>
        protected int FeaturesCount(string feature, string category)
        {
            SerializableKeyValuePair<string, string> pair = new SerializableKeyValuePair<string, string>(feature, category);
            if (FeatureCategoryCombinations.ContainsKey(pair))
            {
                return FeatureCategoryCombinations[pair];
            }
            return 0;
        }

        /// <summary>
        /// The number of items in a category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Number of items in category</returns>
        protected int CategoryCount(string category)
        {
            if (ItemsCountInCategory.ContainsKey(category))
            {
                return ItemsCountInCategory[category];
            }
            return 0;
        }

        /// <summary>
        /// The total number of items
        /// </summary>
        /// <returns>Number of items in all categories</returns>
        protected int TotalCount()
        {
            int count = 0;
            foreach (var i in ItemsCountInCategory)
            {
                count += i.Value;
            }
            return count;
        }

        /// <summary>
        /// The list of all categories
        /// </summary>
        /// <returns>List of categories</returns>
        public List<string> Categories()
        {
            List<string> categories = new List<string>();
            foreach (var i in ItemsCountInCategory)
            {
                categories.Add(i.Key);
            }
            return categories;
        }

        /// <summary>
        /// Train classifier
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        public void Train(string item, string category)
        {
            // break the item into separate features
            SerializableDictionary<string, int> features = GetFeatures(item);
            foreach (var feature in features)
            {
                // increase the counts for this classification for every feature
                IncreaseFeatures(new SerializableKeyValuePair<string, int>(feature.Key, feature.Value), category);
            }

            // increase the total count for this classification
            IncreaseCategories(category);
        }

        /// <summary>
        /// Probability that a word is in a particular category
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <param name="category">Category</param>
        /// <returns>Probability</returns>
        protected double FeatureProbability(string feature, string category)
        {
            if (CategoryCount(category) == 0)
            {
                return 0;
            }

            // the total number of times this feature appeared in this
            // category divided by the total number of items in this category
            return (double)FeaturesCount(feature, category) / CategoryCount(category);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <param name="category">Category</param>
        /// <returns>A weighted average of getprobability and the assumed probability</returns>
        protected double WeightedProbability(string feature, string category, double weight = 1.0, double ap = 0.5)
        {
            // calculate the current probability
            double basicProb = FeatureProbability(feature, category);


            // Count the number of times this feature has appeared in
            // all categories
            int totals = 0;
            foreach (var cat in Categories())
            {
                totals += FeaturesCount(feature, cat);
            }

            // Calculate the weighted average
            double weighedProbability = ((weight * ap) + (totals * basicProb)) / (weight + totals);
            return weighedProbability;
        }


        public void SetThreshold(string category, double threshold)
        {
            if (Thresholds.ContainsKey(category))
            {
                Thresholds[category] = threshold;
            }
        }

        public double GetThreshold(string category)
        {
            if (!Thresholds.ContainsKey(category))
            {
                return 1.0;
            }
            return Thresholds[category];
        }

        public bool DeleteFeatureCategoryCombination(string feature, string category)
        {
            SerializableKeyValuePair<string, string> pair =
                        new SerializableKeyValuePair<string, string>(feature, category);
            if (FeatureCategoryCombinations.ContainsKey(pair))
            {
                int count = FeatureCategoryCombinations[pair];
                FeatureCategoryCombinations.Remove(pair);
                if(ItemsCountInCategory[category] >= count)
                {
                    ItemsCountInCategory[category] -= count;
                }
                else
                {
                    ItemsCountInCategory.Remove(category);
                    Thresholds.Remove(category);
                }
                
                return true;
            }
            return false;
        }

    }
}
