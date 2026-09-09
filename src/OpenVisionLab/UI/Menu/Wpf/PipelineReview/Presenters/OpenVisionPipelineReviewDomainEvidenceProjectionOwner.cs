using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Result;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    // Owns selected-step evidence-family policy without owning View or image lifetime.
    internal sealed class OpenVisionPipelineReviewDomainEvidenceProjectionOwner
    {
        internal OpenVisionPipelineReviewDomainEvidenceProjection Project(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary)
        {
            return new OpenVisionPipelineReviewDomainEvidenceProjection(
                SupportsObjectResults(step),
                summary?.ObjectResults,
                SupportsInstanceResults(step),
                summary?.InstanceResults,
                SupportsGeometryResults(step),
                summary?.GeometryFeatures,
                SupportsCircleEvidence(step),
                summary?.CircleEvidence,
                SupportsMatcherDiagnostics(step),
                summary?.EdgeBasedMatchingDiagnostics,
                summary?.Metrics);
        }

        private static bool SupportsObjectResults(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty).Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            toolType = toolType.Replace(" ", string.Empty).Replace("_", string.Empty);
            return string.Equals(toolType, "Blob", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "Contour", StringComparison.OrdinalIgnoreCase);
        }

        private static bool SupportsInstanceResults(VisionPipelineStep step)
        {
            return VisionPipelineMultiMatchMeanService.IsMultiMatchMean(
                step?.ToolType);
        }

        private static bool SupportsGeometryResults(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(toolType, "Line", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "LineGauge", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "CircleGauge", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "GeometryMeasure", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "GeometricMeasurement", StringComparison.OrdinalIgnoreCase);
        }

        private static bool SupportsCircleEvidence(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            return string.Equals(toolType, "CircleGauge", StringComparison.OrdinalIgnoreCase);
        }

        private static bool SupportsMatcherDiagnostics(VisionPipelineStep step)
        {
            string toolType = (step?.ToolType ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            if (toolType.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                toolType = toolType.Substring(0, toolType.Length - 4);
            }

            return string.Equals(toolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "EdgeBasedTemplateMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(toolType, "EdgeTemplateMatching", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class OpenVisionPipelineReviewDomainEvidenceProjection
    {
        internal OpenVisionPipelineReviewDomainEvidenceProjection(
            bool supportsObjectResults,
            IReadOnlyList<VisionPipelineObjectResult> objectResults,
            bool supportsInstanceResults,
            IReadOnlyList<VisionPipelineInstanceResult> instanceResults,
            bool supportsGeometryResults,
            IReadOnlyList<VisionPipelineGeometryFeatureResult> geometryResults,
            bool supportsCircleEvidence,
            VisionPipelineCircleEvidence circleEvidence,
            bool supportsMatcherDiagnostics,
            EdgeBasedMatchingDiagnosticEvidence matcherDiagnostics,
            IReadOnlyDictionary<string, double> metrics)
        {
            SupportsObjectResults = supportsObjectResults;
            ObjectResults = objectResults;
            SupportsInstanceResults = supportsInstanceResults;
            InstanceResults = instanceResults;
            SupportsGeometryResults = supportsGeometryResults;
            GeometryResults = geometryResults;
            SupportsCircleEvidence = supportsCircleEvidence;
            CircleEvidence = circleEvidence;
            SupportsMatcherDiagnostics = supportsMatcherDiagnostics;
            MatcherDiagnostics = matcherDiagnostics;
            Metrics = metrics;
        }

        internal bool SupportsObjectResults { get; }

        internal IReadOnlyList<VisionPipelineObjectResult> ObjectResults { get; }

        internal bool SupportsInstanceResults { get; }

        internal IReadOnlyList<VisionPipelineInstanceResult> InstanceResults { get; }

        internal bool SupportsGeometryResults { get; }

        internal IReadOnlyList<VisionPipelineGeometryFeatureResult> GeometryResults { get; }

        internal bool SupportsCircleEvidence { get; }

        internal VisionPipelineCircleEvidence CircleEvidence { get; }

        internal bool SupportsMatcherDiagnostics { get; }

        internal EdgeBasedMatchingDiagnosticEvidence MatcherDiagnostics { get; }

        internal IReadOnlyDictionary<string, double> Metrics { get; }
    }
}
