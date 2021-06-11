using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassification.Helpers
{
    public class KMeans
    {
        public int K;  
        public double[][] data; 
        public int N; 
        public int dim;  
        public string initMethod; 
        public int maxIter; 
        public int[] clustering;  
        public double[][] means;  
        public double wcss; 
        public int[] counts; 
        public Random rnd; 

        public KMeans(int K, double[][] data, string initMethod, int maxIter, int seed)
        {
            this.K = K;
            this.data = data;  
            this.initMethod = initMethod;
            this.maxIter = maxIter;

            this.N = data.Length;
            this.dim = data[0].Length;

            this.means = new double[K][];  
            for (int k = 0; k < K; ++k)
                this.means[k] = new double[this.dim];
            this.clustering = new int[N];  
            this.counts = new int[K];  
            this.wcss = double.MaxValue; 

            this.rnd = new Random(seed);
        } 

        public void Cluster(int trials)
        {
            for (int trial = 0; trial < trials; ++trial)
                Cluster();  
        }

        public void Cluster()
        {
            

            int[] currClustering = new int[this.N];  

            double[][] currMeans = new double[this.K][];
            for (int k = 0; k < this.K; ++k)
                currMeans[k] = new double[this.dim];

            if (this.initMethod == "plusplus")
                InitPlusPlus(this.data, currClustering, currMeans, this.rnd);
            else
                throw new Exception("not supported");

            bool changed;  
            int iter = 0;
            while (iter < this.maxIter)
            {
                UpdateMeans(currMeans, this.data, currClustering);
                changed = UpdateClustering(currClustering,
                  this.data, currMeans);
                if (changed == false)
                    break;  
                ++iter;
            }

            double currWCSS = ComputeWithinClusterSS(this.data,
              currMeans, currClustering);
            if (currWCSS < this.wcss) 
            {
                for (int i = 0; i < this.N; ++i)
                    this.clustering[i] = currClustering[i];

                for (int k = 0; k < this.K; ++k)
                    for (int j = 0; j < this.dim; ++j)
                        this.means[k][j] = currMeans[k][j];

                this.counts = ComputeCounts(this.K, currClustering);
                this.wcss = currWCSS;
            }

        } 

        private static void InitPlusPlus(double[][] data, int[] clustering, double[][] means, Random rnd)
        {
          
            int N = data.Length;
            int dim = data[0].Length;
            int K = means.Length;

            int idx = rnd.Next(0, N);
            for (int j = 0; j < dim; ++j)
                means[0][j] = data[idx][j];

            for (int k = 1; k < K; ++k) 
            {
                double[] dSquareds = new double[N]; 

                for (int i = 0; i < N; ++i) 
                {
                    double[] distances = new double[k];

                    for (int ki = 0; ki < k; ++ki)
                        distances[ki] = EucDistance(data[i], means[ki]);

                    int mi = ArgMin(distances);  
                                                
                    dSquareds[i] = distances[mi] * distances[mi];  
                } 

                int newMeanIdx = ProporSelect(dSquareds, rnd);
                for (int j = 0; j < dim; ++j)
                    means[k][j] = data[newMeanIdx][j];
            } 

            UpdateClustering(clustering, data, means);
        } 

        static int ProporSelect(double[] vals, Random rnd)
        {
      
            int n = vals.Length;

            double sum = 0.0;
            for (int i = 0; i < n; ++i)
                sum += vals[i];

            double cumP = 0.0;  
            double p = rnd.NextDouble();

            for (int i = 0; i < n; ++i)
            {
                cumP += (vals[i] / sum);
                if (cumP > p) return i;
            }
            return n - 1;  
        }

        private static int[] ComputeCounts(int K, int[] clustering)
        {
            int[] result = new int[K];
            for (int i = 0; i < clustering.Length; ++i)
            {
                int cid = clustering[i];
                ++result[cid];
            }
            return result;
        }

        private static void UpdateMeans(double[][] means, double[][] data, int[] clustering)
        {
         

            int K = means.Length;
            int N = data.Length;
            int dim = data[0].Length;

            int[] counts = ComputeCounts(K, clustering); 

            for (int k = 0; k < K; ++k)  
                if (counts[k] == 0)
                    throw new Exception("empty cluster passed to UpdateMeans()");

            double[][] result = new double[K][];  
            for (int k = 0; k < K; ++k)
                result[k] = new double[dim];

            for (int i = 0; i < N; ++i)  
            {
                int cid = clustering[i];  
                for (int j = 0; j < dim; ++j)
                    result[cid][j] += data[i][j];  
            }

            for (int k = 0; k < K; ++k)
                for (int j = 0; j < dim; ++j)
                    result[k][j] /= counts[k];

            for (int k = 0; k < K; ++k)
                for (int j = 0; j < dim; ++j)
                    means[k][j] = result[k][j];
        }

        private static bool UpdateClustering(int[] clustering, double[][] data, double[][] means)
        {
          

            int K = means.Length;
            int N = data.Length;

            int[] result = new int[N];  
            bool change = false;  
            int[] counts = new int[K]; 

            for (int i = 0; i < N; ++i)  
                result[i] = clustering[i];

            for (int i = 0; i < data.Length; ++i)  
            {
                double[] dists = new double[K]; 
                for (int k = 0; k < K; ++k)
                    dists[k] = EucDistance(data[i], means[k]);

                int cid = ArgMin(dists); 
                result[i] = cid;
                if (result[i] != clustering[i])
                    change = true;  
                ++counts[cid];
            }

            if (change == false)
                return false;  

            for (int k = 0; k < K; ++k)
                if (counts[k] == 0)
                    return false;  
            for (int i = 0; i < N; ++i)
                clustering[i] = result[i];

            return true; 
        }

        private static double EucDistance(double[] item, double[] mean)
        {
       
            double sum = 0.0;
            for (int j = 0; j < item.Length; ++j)
                sum += (item[j] - mean[j]) * (item[j] - mean[j]);
            return Math.Sqrt(sum);
        }

        private static int ArgMin(double[] v)
        {
            int minIdx = 0;
            double minVal = v[0];
            for (int i = 0; i < v.Length; ++i)
            {
                if (v[i] < minVal)
                {
                    minVal = v[i];
                    minIdx = i;
                }
            }
            return minIdx;
        }

        private static double ComputeWithinClusterSS(double[][] data, double[][] means, int[] clustering)
        {

            double sum = 0.0;
            for (int i = 0; i < data.Length; ++i)
            {
                int cid = clustering[i]; 
                sum += SumSquared(data[i], means[cid]);
            }
            return sum;
        }

        private static double SumSquared(double[] item, double[] mean)
        {

            double sum = 0.0;
            for (int j = 0; j < item.Length; ++j)
                sum += (item[j] - mean[j]) * (item[j] - mean[j]);
            return sum;
        }

   

    } 
}
