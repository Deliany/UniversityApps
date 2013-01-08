using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace by_Deliany
{
    public class KMeans
    {
        /// <summary>
        /// Return list of clusters of given data and clusters count
        /// </summary>
        public List<Cluster> SplitToClusters(List<Data> data, int clustersCount = 1)
        {
            if(clustersCount < 1)
            {
                throw new Exception("Cluster count cannot be less than 1");
            }
            
            // Result list of clusters
            var resClusters = new List<Cluster>();

            // Distortion function J=(1/m)*Σ||x⁽ⁱ⁾-μ⁽ⁱ⁾||²
            // Our goal: minimize this function
            double DistortFunc = double.MaxValue;

            Random random = new Random();

            // Counter of random initialization
            for (int g = 0; g < 100; g++ )
            {
                #region Random initialization of K-means
                //----------------------------------------

                // Counter of filled centroids
                int index = 0;

                // Temporary vector of clusters that contains centroids and points that belongs to them
                var clusters = new List<Cluster>();

                // Pick random indexes until we pickup all different centroids
                while (index < clustersCount)
                {
                    int randomIndex = random.Next(0, data.Count - 1);
                    var cluster = new Cluster { Centroid = data[randomIndex].Attributes };

                    bool contains = false;

                    foreach (var clusterInList in clusters)
                    {
                        if (clusterInList.Centroid.Equals(cluster.Centroid))
                        {
                            contains = true;
                        }
                    }

                    if (!contains)
                    {
                        clusters.Add(cluster);
                        index++;
                    }
                }

                //----------------------------------------
                #endregion

                // Array of Indexes (from 1 to K) of cluster centroid closest to x⁽ⁱ⁾
                var c = new List<int>(new int[data.Count]);

                for(int i = 0; i < data.Count; i++)
                {
                    c[i] = clusters.IndexOf(ClosestCentroid(clusters, data[i]));
                }
                
                // link cluster centroid to closest points
                for(int i = 0; i < data.Count; i++)
                {
                    clusters[c[i]].Points.Add(data[i]);
                }

                // average(mean) of points assisned to cluster K
                for (int i = 0; i < clusters.Count; i++)
                {
                    clusters[i].Centroid = AverageMean(clusters[i].Points);
                }

                // calculate updated value of distortion function
                double newDistortFuncValue = data.Select((t, i) => EuclideanDistance(clusters[c[i]].Centroid, t.Attributes)).Sum();
                newDistortFuncValue /= data.Count;

                if(newDistortFuncValue < DistortFunc)
                {
                    DistortFunc = newDistortFuncValue;
                    resClusters = clusters;
                }

            }
            return resClusters;
        }

        /// <summary>
        /// Return closes centroid of given data
        /// </summary>
        private Cluster ClosestCentroid(List<Cluster> centroids, Data data)
        {
            double minVal = double.MaxValue;
            Cluster closestCentroid = centroids[0];

            for(int i = 0; i < centroids.Count; i++)
            {
                double measure = EuclideanDistance(centroids[i].Centroid, data.Attributes);
                if(measure < minVal)
                {
                    minVal = measure;
                    closestCentroid = centroids[i];
                }
            }

            return closestCentroid;
        }

        /// <summary>
        /// Return vector of average values from given attributes values
        /// </summary>
        public List<double> AverageMean(List<Data> points)
        {
            // vector, that will store average value of our data attributes
            var average = new List<double>();

            // dimension of our dataset
            int attrCount = points.First().Attributes.Count;

            for (int i = 0; i < attrCount; i++)
            {
                double sum = points.Sum(point => point.Attributes[i]);
                average.Add(sum / points.Count);
            }
            return average;
        } 

        #region Distance measure
        //----------------------------------------

        /// <summary>
        /// Euclidian distance measure, how close are two vectors in Rⁿ 
        /// </summary>
        private double EuclideanDistance(List<double> centroid, List<double> data)
        {
            double sum = centroid.Select((value, i) => Math.Pow(data[i] - value, 2)).Sum();

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Returns value between 0 and 1, where value of 1 means two vectors are very similar
        /// </summary>
        private double EuclideanSimilarity(List<double> centroid, List<double> data)
        {
            if (centroid.Count != data.Count)
            {
                throw new Exception("Dimensions isn't equal");
            }
            return 1 / (1 + EuclideanDistance(centroid, data));
        }

        //----------------------------------------
        #endregion
    }
}
