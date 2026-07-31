using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionLearnMatchingSimulationModel
    {
        private const int SearchWidth = 5;
        private const int TemplateWidth = 2;

        private static readonly int[] searchValues =
        {
            0, 0, 0, 0, 0,
            0, 1, 1, 0, 0,
            0, 1, 0, 0, 1,
            1, 1, 1, 0, 1,
            1, 0, 0, 0, 0
        };

        private static readonly int[] templateValues =
        {
            1, 1,
            1, 0
        };

        private static readonly (int X, int Y)[] candidatePositions =
        {
            (0, 0),
            (1, 1),
            (3, 1),
            (2, 2),
            (3, 3)
        };

        private static readonly (int X, int Y)[] featureReferencePoints =
        {
            (1, 1),
            (3, 1),
            (2, 2),
            (1, 3),
            (3, 3),
            (0, 4)
        };

        private static readonly (int X, int Y)[] featureScenePoints =
        {
            (1, 1),
            (3, 1),
            (2, 2),
            (1, 3),
            (3, 3),
            (4, 4)
        };

        private static readonly double[] featureScores =
        {
            0.92D,
            0.88D,
            0.81D,
            0.74D,
            0.67D,
            0.42D
        };

        private static readonly bool[] featureRansacInliers =
        {
            true,
            true,
            true,
            true,
            false,
            false
        };

        public static IReadOnlyList<int> SearchValues => searchValues;

        public static IReadOnlyList<int> TemplateValues => templateValues;

        public static IReadOnlyList<(int X, int Y)> CandidatePositions => candidatePositions;

        public static IReadOnlyList<(int X, int Y)> FeatureReferencePoints => featureReferencePoints;

        public static IReadOnlyList<(int X, int Y)> FeatureScenePoints => featureScenePoints;

        public static IReadOnlyList<double> FeatureScores => featureScores;

        public static IReadOnlyList<bool> FeatureRansacInliers => featureRansacInliers;

        public static TemplateEvaluation EvaluateTemplate(double threshold)
        {
            double[] scores = candidatePositions
                .Select(position => CalculateTemplateScore(position.X, position.Y))
                .ToArray();
            double bestScore = scores.Max();
            int bestIndex = Array.IndexOf(scores, bestScore);
            return new TemplateEvaluation(
                scores,
                bestIndex,
                bestScore,
                bestScore >= threshold);
        }

        public static FeatureEvaluation EvaluateFeatures(double requiredValue)
        {
            int required = Math.Max(1, (int)Math.Round(requiredValue));
            bool[] goodMatches = featureScores
                .Select(score => score >= FeatureScoreThreshold)
                .ToArray();
            int goodCount = goodMatches.Count(item => item);
            return new FeatureEvaluation(
                goodMatches,
                goodCount,
                required,
                goodCount >= required);
        }

        public const double FeatureScoreThreshold = 0.65D;

        private static double CalculateTemplateScore(int startX, int startY)
        {
            int matches = 0;
            for (int y = 0; y < TemplateWidth; y++)
            {
                for (int x = 0; x < TemplateWidth; x++)
                {
                    int search = searchValues[(startY + y) * SearchWidth + startX + x];
                    int template = templateValues[y * TemplateWidth + x];
                    if (search == template)
                    {
                        matches++;
                    }
                }
            }

            return matches / (double)templateValues.Length;
        }

        internal readonly record struct TemplateEvaluation(
            double[] Scores,
            int BestIndex,
            double BestScore,
            bool Pass);

        internal readonly record struct FeatureEvaluation(
            bool[] GoodMatches,
            int GoodCount,
            int Required,
            bool Pass);
    }
}
