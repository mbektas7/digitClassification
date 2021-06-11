using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassification.Helpers
{
    public static class Misc
    {
        public static Color[] centroidColors;

        static Misc()
        {
            centroidColors = new Color[10];
            centroidColors[0] = Color.Red;
            centroidColors[1] = Color.Blue;
            centroidColors[2] = Color.Green;
            centroidColors[3] = Color.Yellow;
            centroidColors[4] = Color.Orange;
            centroidColors[5] = Color.LightBlue;
            centroidColors[6] = Color.LightCyan;
            centroidColors[7] = Color.DarkGoldenrod;
            centroidColors[8] = Color.DarkMagenta;
            centroidColors[9] = Color.Black;


        }

        public static List<double[]> Clone(List<double[]> array)
        {
            List<double[]> resultList = new List<double[]>();
            foreach (double[] tempArray in array)
            {
                double[] newArray = new double[tempArray.Length];
                for (int i = 0; i < tempArray.Length; i++)
                    newArray[i] = tempArray[i];
                resultList.Add(newArray);
            }
            return resultList;
        }

        public static List<Tuple<double, double>> GetMinMaxPoints(double[][] dataset)
        {
            List<Tuple<double, double>> result = new List<Tuple<double, double>>();

            for (int j = 0; j < dataset[0].GetLength(0); j++)
            {
                double min = Double.MaxValue;
                double max = Double.MinValue;
                for (int i = 0; i < dataset.Length; i++)
                {
                    double element = dataset[i][j];
                    if (element < min)
                        min = element;
                    if (element > max)
                        max = element;
                }
                result.Add(new Tuple<double, double>(min, max));
            }
            return result;
        }

        public static double GenerateRandomDouble(Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
