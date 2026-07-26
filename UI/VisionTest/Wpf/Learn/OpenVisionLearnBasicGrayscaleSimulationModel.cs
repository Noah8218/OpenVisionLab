using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionLearnBasicGrayscaleSimulationModel
    {
        private static readonly int[] thresholdSamples =
        {
            24,
            60,
            96,
            119,
            128,
            151,
            190,
            230
        };

        private static readonly int[] brightnessSamples =
        {
            22,
            48,
            75,
            103,
            126,
            152,
            184,
            218
        };

        private static readonly int[] arithmeticInputA =
        {
            20,
            45,
            90,
            120,
            150,
            180,
            210,
            240
        };

        private static readonly int[] arithmeticInputB =
        {
            10,
            60,
            40,
            140,
            110,
            200,
            30,
            220
        };

        private static readonly int[] filterSamples =
        {
            42,
            58,
            54,
            60,
            220,
            65,
            57,
            62,
            59
        };

        public static IReadOnlyList<int> ThresholdSamples => thresholdSamples;

        public static IReadOnlyList<int> BrightnessSamples => brightnessSamples;

        public static IReadOnlyList<int> ArithmeticInputA => arithmeticInputA;

        public static IReadOnlyList<int> ArithmeticInputB => arithmeticInputB;

        public static IReadOnlyList<int> FilterSamples => filterSamples;

        public static ThresholdEvaluation EvaluateThreshold(
            double thresholdValue,
            bool invert,
            int maximumValue)
        {
            int threshold = ClampToByte(thresholdValue);
            int maximum = ClampToByte(maximumValue);
            int[] results = thresholdSamples
                .Select(source =>
                {
                    bool high = source >= threshold;
                    return invert
                        ? high ? 0 : maximum
                        : high ? maximum : 0;
                })
                .ToArray();
            return new ThresholdEvaluation(threshold, results);
        }

        public static BrightnessEvaluation EvaluateBrightness(double offsetValue)
        {
            int offset = (int)Math.Round(offsetValue);
            int[] results = brightnessSamples
                .Select(value => ClampToByte(value + offset))
                .ToArray();
            int[] bins = new int[8];
            foreach (int result in results)
            {
                bins[Math.Min(7, result / 32)]++;
            }

            return new BrightnessEvaluation(
                offset,
                results,
                bins,
                (int)Math.Round(brightnessSamples.Average()),
                (int)Math.Round(results.Average()));
        }

        public static ArithmeticEvaluation EvaluateArithmetic(string mode)
        {
            int[] results = arithmeticInputA
                .Select((inputA, index) => CalculateArithmeticResult(
                    mode,
                    inputA,
                    arithmeticInputB[index]))
                .ToArray();
            return new ArithmeticEvaluation(results);
        }

        public static FilterEvaluation EvaluateFilter(string mode)
        {
            int center = filterSamples[4];
            int[] sortedValues = filterSamples.OrderBy(value => value).ToArray();
            int sum = filterSamples.Sum();
            int result = mode switch
            {
                "Median" => sortedValues[4],
                "Sharpen" => ClampToByte(
                    center * 5
                    - filterSamples[1]
                    - filterSamples[3]
                    - filterSamples[5]
                    - filterSamples[7]),
                _ => ClampToByte(filterSamples.Average())
            };

            return new FilterEvaluation(result, sortedValues, sum);
        }

        private static int CalculateArithmeticResult(
            string mode,
            int inputA,
            int inputB)
        {
            return mode switch
            {
                "Add" => ClampToByte(inputA + inputB),
                "Subtract" => ClampToByte(inputA - inputB),
                "Bitwise AND" => inputA & inputB,
                "Bitwise OR" => inputA | inputB,
                _ => Math.Abs(inputA - inputB)
            };
        }

        private static int ClampToByte(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return 0;
            }

            return Math.Max(0, Math.Min(255, (int)Math.Round(value)));
        }

        internal readonly record struct ThresholdEvaluation(
            int Threshold,
            int[] Results);

        internal readonly record struct BrightnessEvaluation(
            int Offset,
            int[] Results,
            int[] HistogramBins,
            int SourceAverage,
            int ResultAverage);

        internal readonly record struct ArithmeticEvaluation(int[] Results);

        internal readonly record struct FilterEvaluation(
            int Result,
            int[] SortedValues,
            int Sum);
    }
}
