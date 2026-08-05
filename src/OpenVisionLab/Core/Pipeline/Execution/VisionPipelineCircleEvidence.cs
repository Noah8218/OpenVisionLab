using OpenVisionLab.Vision2D.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    internal sealed class VisionPipelineCircleSampleEvidence
    {
        public int Number { get; set; }
        public double AngleDeg { get; set; }
        public double ScanStartX { get; set; }
        public double ScanStartY { get; set; }
        public double ScanEndX { get; set; }
        public double ScanEndY { get; set; }
        public bool HasEdgePoint { get; set; }
        public double EdgeX { get; set; }
        public double EdgeY { get; set; }
        public double EdgeRadiusPx { get; set; }
        public double EdgeStrengthGv { get; set; }
        public double EdgeSignedResponseGv { get; set; }
        public bool ContrastAccepted { get; set; }
        public bool FitInlier { get; set; }
        public bool HasFitResidual { get; set; }
        public double FitResidualPx { get; set; }
        public double ProfileRadiusStartPx { get; set; }
        public double ProfileRadiusStepPx { get; set; } = 1D;
        public IReadOnlyList<double> IntensityValues { get; set; } = Array.Empty<double>();
        public IReadOnlyList<double> SignedResponseValues { get; set; } = Array.Empty<double>();
        public string RejectReason { get; set; } = string.Empty;

        public string StateText => !HasEdgePoint
            ? "No sample"
            : !ContrastAccepted
                ? "Contrast reject"
                : FitInlier
                    ? "Inlier"
                    : HasFitResidual ? "Fit outlier" : "Edge candidate";

        public string PointText => HasEdgePoint
            ? $"({EdgeX:0.##}, {EdgeY:0.##})"
            : "-";

        public string ResidualText => HasFitResidual
            ? FitResidualPx.ToString("+0.###;-0.###;0")
            : "-";

        public VisionPipelineCircleSampleEvidence Clone()
        {
            return new VisionPipelineCircleSampleEvidence
            {
                Number = Number,
                AngleDeg = AngleDeg,
                ScanStartX = ScanStartX,
                ScanStartY = ScanStartY,
                ScanEndX = ScanEndX,
                ScanEndY = ScanEndY,
                HasEdgePoint = HasEdgePoint,
                EdgeX = EdgeX,
                EdgeY = EdgeY,
                EdgeRadiusPx = EdgeRadiusPx,
                EdgeStrengthGv = EdgeStrengthGv,
                EdgeSignedResponseGv = EdgeSignedResponseGv,
                ContrastAccepted = ContrastAccepted,
                FitInlier = FitInlier,
                HasFitResidual = HasFitResidual,
                FitResidualPx = FitResidualPx,
                ProfileRadiusStartPx = ProfileRadiusStartPx,
                ProfileRadiusStepPx = ProfileRadiusStepPx,
                IntensityValues = Array.AsReadOnly((IntensityValues ?? Array.Empty<double>()).ToArray()),
                SignedResponseValues = Array.AsReadOnly((SignedResponseValues ?? Array.Empty<double>()).ToArray()),
                RejectReason = RejectReason ?? string.Empty
            };
        }
    }

    internal sealed class VisionPipelineCircleEvidence
    {
        public string StepName { get; set; } = string.Empty;
        public string InputLayer { get; set; } = string.Empty;
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public double TaughtCenterX { get; set; }
        public double TaughtCenterY { get; set; }
        public double RadiusMinPx { get; set; }
        public double RadiusMaxPx { get; set; }
        public double StartAngleDeg { get; set; }
        public double SweepAngleDeg { get; set; }
        public int ScanCount { get; set; }
        public string EdgePolarity { get; set; } = string.Empty;
        public double MinimumContrastGv { get; set; }
        public double MinimumSupportRatio { get; set; }
        public double MaximumFitResidualPx { get; set; }
        public bool HasFit { get; set; }
        public double FitCenterX { get; set; }
        public double FitCenterY { get; set; }
        public double FitRadiusPx { get; set; }
        public double FitResidualPx { get; set; }
        public double RobustRejectionPx { get; set; }
        public int EdgeCandidateCount { get; set; }
        public int InlierCount { get; set; }
        public double SupportRatio { get; set; }
        public double CoverageDeg { get; set; }
        public IReadOnlyList<VisionPipelineCircleSampleEvidence> Samples { get; set; } =
            Array.Empty<VisionPipelineCircleSampleEvidence>();

        public string SummaryText => HasFit
            ? $"{ScanCount} scans | edge {EdgeCandidateCount} | inlier {InlierCount} | "
                + $"S {SupportRatio:0.###} >= {MinimumSupportRatio:0.###} | "
                + $"R {FitRadiusPx:0.###} px | RMS {FitResidualPx:0.###} <= {MaximumFitResidualPx:0.###} px"
            : $"{ScanCount} scans | edge {EdgeCandidateCount} | "
                + $"S gate >= {MinimumSupportRatio:0.###} | no fitted circle";

        public VisionPipelineCircleEvidence Clone()
        {
            return new VisionPipelineCircleEvidence
            {
                StepName = StepName,
                InputLayer = InputLayer,
                ImageWidth = ImageWidth,
                ImageHeight = ImageHeight,
                TaughtCenterX = TaughtCenterX,
                TaughtCenterY = TaughtCenterY,
                RadiusMinPx = RadiusMinPx,
                RadiusMaxPx = RadiusMaxPx,
                StartAngleDeg = StartAngleDeg,
                SweepAngleDeg = SweepAngleDeg,
                ScanCount = ScanCount,
                EdgePolarity = EdgePolarity,
                MinimumContrastGv = MinimumContrastGv,
                MinimumSupportRatio = MinimumSupportRatio,
                MaximumFitResidualPx = MaximumFitResidualPx,
                HasFit = HasFit,
                FitCenterX = FitCenterX,
                FitCenterY = FitCenterY,
                FitRadiusPx = FitRadiusPx,
                FitResidualPx = FitResidualPx,
                RobustRejectionPx = RobustRejectionPx,
                EdgeCandidateCount = EdgeCandidateCount,
                InlierCount = InlierCount,
                SupportRatio = SupportRatio,
                CoverageDeg = CoverageDeg,
                Samples = Array.AsReadOnly(
                    (Samples ?? Array.Empty<VisionPipelineCircleSampleEvidence>())
                        .Where(item => item != null)
                        .Select(item => item.Clone())
                        .ToArray())
            };
        }
    }

    internal static class VisionPipelineCircleEvidenceStore
    {
        private static readonly ConditionalWeakTable<VisionToolResult, Holder> Values =
            new ConditionalWeakTable<VisionToolResult, Holder>();

        public static void Set(VisionToolResult result, VisionPipelineCircleEvidence evidence)
        {
            if (result == null || evidence == null)
            {
                return;
            }

            Values.Remove(result);
            Values.Add(result, new Holder(evidence.Clone()));
        }

        public static VisionPipelineCircleEvidence Get(VisionToolResult result)
        {
            return result != null && Values.TryGetValue(result, out Holder holder)
                ? holder.Value.Clone()
                : null;
        }

        private sealed class Holder
        {
            public Holder(VisionPipelineCircleEvidence value)
            {
                Value = value;
            }

            public VisionPipelineCircleEvidence Value { get; }
        }
    }
}
