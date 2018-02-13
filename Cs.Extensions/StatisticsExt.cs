using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    public class StatisticsException : Exception
    {
        public StatisticsException()
            : base()
        {
        }

        public StatisticsException(string message)
            : base(message)
        {
        }

        public StatisticsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class StatisticsExt
    {
        #region [ Histogram Methods ]

        public static IEnumerable<HistogramBin> HistogramBins(this IEnumerable<double> values, bool useFreedmanDiaconis)
        {
            List<double> list = values.ToList();

            return list.Count == 0 ? null : list.HistogramBins(0, list.Count, useFreedmanDiaconis);
        }

        public static IEnumerable<HistogramBin> HistogramBins(this IEnumerable<double> values, int start, int end, bool useFreedmanDiaconis)
        {
            List<double> list = values.ToList();
            List<double> orderedList = list.OrderBy(num => num).ToList();

            double h = 0;
            int n = orderedList.GetRange(start, end - start).Count;
            if (useFreedmanDiaconis)
            {
                double IQRx = orderedList.IQR(); //Freedman-Diaconis' Choice
                h = (2 * IQRx) / (Math.Pow(n, (1.0 / 3))); //Freedman-Diaconis' Choice
            }
            else
            {
                double s = StandardDeviation(orderedList, start, end); //Scott's Normal Reference Rule
                h = (3.49 * s) / (Math.Pow(n, (1.0 / 3))); //Scott's Normal Reference Rule
            }
            double min = orderedList.Min();
            double max = orderedList.Max();
            double dataRange = max - min;
            int k = (int)Math.Ceiling(dataRange / h);

            List<HistogramBin> binList = new List<HistogramBin>();
            double binOffset = ((h * k) - dataRange) / 2;
            double binStart = orderedList[0] - binOffset;
            for (int i = 0; i < k; i++)
            {
                HistogramBin histogramBin = new HistogramBin();
                histogramBin.binStart = binStart;
                histogramBin.binStop = binStart + h;
                histogramBin.xValue = (histogramBin.binStop + histogramBin.binStart) / 2;
                histogramBin.binCount = orderedList.Count(v => histogramBin.binStart < v && v < histogramBin.binStop);
                binList.Add(histogramBin);

                binStart = histogramBin.binStop;
            }

            return binList;
        }

        public static double IQR(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();
            List<double> orderedList = list.OrderBy(num => num).ToList();

            int listSize = orderedList.Count;
            double lowerQuartile;
            double upperQuartile;
            if (listSize.IsEven())
            {
                int midIndex = (listSize / 2);
                lowerQuartile = values.Median(0, midIndex);
                upperQuartile = values.Median(midIndex, listSize);
            }
            else
            {
                int midIndex = (int)Math.Ceiling((double)(listSize / 2));
                lowerQuartile = values.Median(0, midIndex - 1);
                upperQuartile = values.Median(midIndex + 1, listSize);
            }
            return upperQuartile - lowerQuartile;
        }

        #endregion

        #region [ StandardDeviation Methods ]

        public static double StandardDeviation(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();

            return list.Count == 0 ? 0 : list.StandardDeviation(0, list.Count);
        }

        public static double StandardDeviation(this IEnumerable<double> values, int start, int end)
        {
            List<double> list = values.ToList();

            double mean = list.Mean(start, end);
            double variance = list.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }

        #endregion

        #region [ Variance Methods ]

        public static double Variance(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();

            return list.Variance(list.Mean(), 0, list.Count);
        }

        public static double Variance(this IEnumerable<double> values, double mean)
        {
            List<double> list = values.ToList();

            return list.Variance(mean, 0, list.Count);
        }

        public static double Variance(this IEnumerable<double> values, double mean, int start, int end)
        {
            List<double> list = values.ToList();
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((list[i] - mean), 2);
            }

            int count = end - start;
            if (start > 0) count -= 1;

            return variance / (count);
        }

        #endregion

        #region [ Mean Methods ]

        public static double Mean(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();

            return list.Count == 0 ? 0 : list.Mean(0, list.Count);
        }

        public static double Mean(this IEnumerable<double> values, int start, int end)
        {
            List<double> list = values.ToList();
            double sum = 0;

            for (int i = start; i < end; i++)
            {
                sum += list[i];
            }

            return sum / (end - start);
        }

        #endregion

        #region [ Median Methods ]

        public static double Median(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();

            return list.Count == 0 ? 0 : list.Median(0, list.Count);
        }

        public static double Median(this IEnumerable<double> values, int start, int end)
        {
            double medianVal;
            int midIndex;

            List<double> list = values.ToList();
            List<double> subList = list.GetRange(start, end - start);
            List<double> orderedList = subList.OrderBy(num => num).ToList();

            int listSize = orderedList.Count;

            if (listSize.IsEven())
            {
                midIndex = listSize / 2;
                medianVal = ((orderedList.ElementAt(midIndex - 1) + orderedList.ElementAt(midIndex)) / 2);
            }
            else
            {
                midIndex = (int)Math.Round((double)(listSize / 2), MidpointRounding.AwayFromZero);
                medianVal = orderedList.ElementAt(midIndex - 1);
            }

            return medianVal;
        }

        #endregion

        #region [ Modes Methods ]

        public static IEnumerable<double> Modes(this IEnumerable<double> values)
        {
            List<double> list = values.ToList();

            return list.Count == 0 ? null : list.Modes(0, list.Count);
        }

        public static IEnumerable<double> Modes(this IEnumerable<double> values, int start, int end)
        {
            List<double> list = values.ToList();
            List<double> subList = new List<double>();

            for (int i = start; i < end; i++)
            {
                subList.Add(list.ElementAt(i));
            }

            var modesList = subList
                .GroupBy(num => num)
                .Select(valueCluster =>
                        new
                        {
                            Value = valueCluster.Key,
                            Occurrence = valueCluster.Count(),
                        })
                .ToList();

            int maxOccurrence = modesList.Max(g => g.Occurrence);

            return modesList
                .Where(x => x.Occurrence == maxOccurrence && maxOccurrence > 1)
                .Select(x => x.Value);
        }

        #endregion

        public static bool IsEven(this int number)
        {
            return number % 2 == 0 ? true : false;
        }
    }

    public struct HistogramBin
    {
        public double xValue;
        public double binStart;
        public double binStop;
        public int binCount;
    }
}
