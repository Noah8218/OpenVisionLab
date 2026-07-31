using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal sealed class VisionToolThresholdSuggestion
    {
        public bool Accepted { get; init; }
        public int Threshold { get; init; }
        public double SeparationRatio { get; init; }
        public double LowerPopulationRatio { get; init; }
        public double UpperPopulationRatio { get; init; }
        public string Reason { get; init; } = string.Empty;
        public string EvidenceId { get; init; } = string.Empty;
    }

    internal static class VisionToolThresholdSuggestionAnalyzer
    {
        private const double MinimumClassPopulationRatio = 0.01D;

        public static VisionToolThresholdSuggestion Analyze(
            VisionToolSignalEvidence evidence,
            bool selectBright = true)
        {
            if (evidence == null)
            {
                return Rejected("No current Preview histogram is available.");
            }

            VisionToolSignalSeries population = evidence.Series.FirstOrDefault(series =>
                string.Equals(series.Name, "Gray population", StringComparison.Ordinal));
            if (population == null || population.Values.Count != 256)
            {
                return Rejected(
                    "The current evidence is not a 256-bin full-image gray histogram.",
                    evidence.EvidenceId);
            }

            double[] histogram = population.Values.ToArray();
            double total = histogram.Sum();
            if (!double.IsFinite(total) || total <= 0D)
            {
                return Rejected("The current histogram has no finite population.", evidence.EvidenceId);
            }

            IReadOnlyList<int> significantModes = FindSignificantModes(histogram);
            if (significantModes.Count < 2)
            {
                return Rejected(
                    "Fewer than two significant gray modes were found; keep manual teaching.",
                    evidence.EvidenceId);
            }

            int targetMode = selectBright
                ? significantModes[significantModes.Count - 1]
                : significantModes[0];
            int adjacentMode = selectBright
                ? significantModes[significantModes.Count - 2]
                : significantModes[1];
            int suggestedThreshold = (int)Math.Round(
                (targetMode + adjacentMode) / 2D,
                MidpointRounding.AwayFromZero);
            double lowerPopulation = histogram.Take(suggestedThreshold + 1).Sum() / total;
            double upperPopulation = 1D - lowerPopulation;
            double separationRatio = CalculateSeparationRatio(
                histogram,
                total,
                suggestedThreshold);
            bool accepted = lowerPopulation >= MinimumClassPopulationRatio
                && upperPopulation >= MinimumClassPopulationRatio;
            string reason = accepted
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    "Suggested T={0}: deterministic {1} target split at the midpoint between significant gray modes {2} and {3}; separation {4:P1}, lower class {5:P1}, upper class {6:P1}. Review the exact orange candidate marker before Use.",
                    suggestedThreshold,
                    selectBright ? "bright Binary" : "dark BinaryInv",
                    adjacentMode,
                    targetMode,
                    separationRatio,
                    lowerPopulation,
                    upperPopulation)
                : string.Format(
                    CultureInfo.InvariantCulture,
                    "Rejected T={0}: one significant-mode class is smaller than {1:P0} of the current full-image population (lower {2:P1}, upper {3:P1}). Keep manual teaching.",
                    suggestedThreshold,
                    MinimumClassPopulationRatio,
                    lowerPopulation,
                    upperPopulation);

            return new VisionToolThresholdSuggestion
            {
                Accepted = accepted,
                Threshold = suggestedThreshold,
                SeparationRatio = separationRatio,
                LowerPopulationRatio = lowerPopulation,
                UpperPopulationRatio = upperPopulation,
                Reason = reason,
                EvidenceId = CreateEvidenceId(
                    evidence,
                    accepted,
                    selectBright,
                    suggestedThreshold,
                    separationRatio,
                    lowerPopulation,
                    upperPopulation)
            };
        }

        private static IReadOnlyList<int> FindSignificantModes(IReadOnlyList<double> histogram)
        {
            double[] smoothed = new double[histogram.Count];
            for (int index = 0; index < histogram.Count; index++)
            {
                int start = Math.Max(0, index - 2);
                int end = Math.Min(histogram.Count - 1, index + 2);
                double sum = 0D;
                for (int sample = start; sample <= end; sample++)
                {
                    sum += histogram[sample];
                }

                smoothed[index] = sum / (end - start + 1);
            }

            double minimumPeak = smoothed.Max() * 0.02D;
            List<int> rankedPeaks = Enumerable.Range(2, histogram.Count - 4)
                .Where(index =>
                    smoothed[index] >= minimumPeak
                    && smoothed[index] >= smoothed[index - 1]
                    && smoothed[index] >= smoothed[index + 1])
                .OrderByDescending(index => smoothed[index])
                .ThenBy(index => index)
                .ToList();
            List<int> retained = new List<int>();
            foreach (int peak in rankedPeaks)
            {
                if (retained.All(existing => Math.Abs(existing - peak) >= 10))
                {
                    retained.Add(peak);
                }
            }

            List<int> centered = retained
                .Select(peak =>
                {
                    int start = Math.Max(0, peak - 5);
                    int end = Math.Min(histogram.Count - 1, peak + 5);
                    double weight = 0D;
                    double moment = 0D;
                    for (int sample = start; sample <= end; sample++)
                    {
                        weight += histogram[sample];
                        moment += sample * histogram[sample];
                    }

                    return weight <= 0D
                        ? peak
                        : (int)Math.Round(moment / weight, MidpointRounding.AwayFromZero);
                })
                .Distinct()
                .OrderBy(value => value)
                .ToList();
            return centered;
        }

        private static double CalculateSeparationRatio(
            IReadOnlyList<double> histogram,
            double total,
            int threshold)
        {
            double totalMoment = 0D;
            for (int gray = 0; gray < histogram.Count; gray++)
            {
                totalMoment += gray * histogram[gray];
            }

            double mean = totalMoment / total;
            double totalVariance = 0D;
            double lowerWeightRaw = 0D;
            double lowerMoment = 0D;
            for (int gray = 0; gray < histogram.Count; gray++)
            {
                double delta = gray - mean;
                totalVariance += histogram[gray] * delta * delta;
                if (gray <= threshold)
                {
                    lowerWeightRaw += histogram[gray];
                    lowerMoment += gray * histogram[gray];
                }
            }

            totalVariance /= total;
            double upperWeightRaw = total - lowerWeightRaw;
            if (totalVariance <= 1e-12D
                || lowerWeightRaw <= 0D
                || upperWeightRaw <= 0D)
            {
                return 0D;
            }

            double lowerWeight = lowerWeightRaw / total;
            double upperWeight = upperWeightRaw / total;
            double lowerMean = lowerMoment / lowerWeightRaw;
            double upperMean = (totalMoment - lowerMoment) / upperWeightRaw;
            double deltaMean = lowerMean - upperMean;
            return Math.Clamp(
                lowerWeight * upperWeight * deltaMean * deltaMean / totalVariance,
                0D,
                1D);
        }

        private static VisionToolThresholdSuggestion Rejected(
            string reason,
            string sourceEvidenceId = "")
        {
            string canonical = string.Join(
                "|",
                "ThresholdSuggestion",
                sourceEvidenceId ?? string.Empty,
                "Rejected",
                reason ?? string.Empty);
            return new VisionToolThresholdSuggestion
            {
                Accepted = false,
                Reason = reason ?? string.Empty,
                EvidenceId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            };
        }

        private static string CreateEvidenceId(
            VisionToolSignalEvidence evidence,
            bool accepted,
            bool selectBright,
            int threshold,
            double separationRatio,
            double lowerPopulation,
            double upperPopulation)
        {
            string canonical = string.Join(
                "|",
                "ThresholdSuggestion",
                evidence.EvidenceId,
                evidence.SourceSha256,
                evidence.RegionDescription,
                accepted,
                selectBright ? "BrightBinary" : "DarkBinaryInv",
                threshold.ToString(CultureInfo.InvariantCulture),
                separationRatio.ToString("0.###############", CultureInfo.InvariantCulture),
                lowerPopulation.ToString("0.###############", CultureInfo.InvariantCulture),
                upperPopulation.ToString("0.###############", CultureInfo.InvariantCulture));
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        }
    }
}
