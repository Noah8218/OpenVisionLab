using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionLearnLineSimulationModel
    {
        private const int EdgeGridWidth = 5;
        private const int EdgeGridHeight = 5;
        private const double MillimetersPerPixel = 0.006D;

        private static readonly int[] edgeSampleValues =
        {
            42, 48, 54, 186, 193,
            45, 50, 57, 190, 198,
            43, 49, 55, 188, 196,
            47, 53, 60, 194, 201,
            44, 51, 58, 191, 199
        };

        private static readonly int[] distanceLeftEdges = { 2, 2, 2, 2, 2 };

        private static readonly int[] distanceRightEdges = { 6, 6, 7, 6, 6 };

        public static IReadOnlyList<int> EdgeSampleValues => edgeSampleValues;

        public static IReadOnlyList<int> DistanceLeftEdges => distanceLeftEdges;

        public static IReadOnlyList<int> DistanceRightEdges => distanceRightEdges;

        public static EdgeLineEvaluation EvaluateEdgeLine(double thresholdValue)
        {
            int threshold = Math.Max(10, (int)Math.Round(thresholdValue));
            bool[] edges = new bool[edgeSampleValues.Length];
            int[] strengths = new int[edgeSampleValues.Length];

            for (int y = 0; y < EdgeGridHeight; y++)
            {
                for (int x = 0; x < EdgeGridWidth; x++)
                {
                    int index = y * EdgeGridWidth + x;
                    int strength = x < EdgeGridWidth - 1
                        ? Math.Abs(edgeSampleValues[index + 1] - edgeSampleValues[index])
                        : 0;
                    strengths[index] = strength;
                    edges[index] = strength >= threshold;
                }
            }

            int bestColumn = 0;
            int bestRun = 0;
            for (int x = 0; x < EdgeGridWidth; x++)
            {
                int run = 0;
                for (int y = 0; y < EdgeGridHeight; y++)
                {
                    int index = y * EdgeGridWidth + x;
                    run = edges[index] ? run + 1 : 0;
                    if (run > bestRun)
                    {
                        bestRun = run;
                        bestColumn = x;
                    }
                }
            }

            return new EdgeLineEvaluation(
                threshold,
                strengths,
                edges,
                bestColumn,
                bestRun);
        }

        public static LineDistanceEvaluation EvaluateLineDistance(double rangeMaximum)
        {
            int[] distances = distanceRightEdges
                .Select((right, index) => right - distanceLeftEdges[index])
                .ToArray();
            double average = distances.Average();
            int minimum = distances.Min();
            int maximum = distances.Max();
            int range = maximum - minimum;

            return new LineDistanceEvaluation(
                distances,
                average,
                minimum,
                maximum,
                range,
                average * MillimetersPerPixel,
                range * MillimetersPerPixel,
                maximum * MillimetersPerPixel,
                range <= rangeMaximum);
        }

        internal readonly record struct EdgeLineEvaluation(
            int Threshold,
            int[] Strengths,
            bool[] Edges,
            int BestColumn,
            int BestRun);

        internal readonly record struct LineDistanceEvaluation(
            int[] Distances,
            double Average,
            int Minimum,
            int Maximum,
            int Range,
            double AverageMillimeters,
            double RangeMillimeters,
            double MaximumMillimeters,
            bool RangePass);
    }
}
