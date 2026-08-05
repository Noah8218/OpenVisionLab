using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenVisionLab
{
    public sealed class VisionPipelineMatchResultEvidence
    {
        public int NativeIndex { get; set; }
        public string SourceStep { get; set; } = string.Empty;
        public string CoordinateLayer { get; set; } = string.Empty;
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public double Score { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double BoundsX { get; set; }
        public double BoundsY { get; set; }
        public double BoundsWidth { get; set; }
        public double BoundsHeight { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; } = 1D;

        public VisionPipelineMatchResultEvidence Clone()
        {
            return (VisionPipelineMatchResultEvidence)MemberwiseClone();
        }
    }

    public sealed class VisionPipelineInstanceResult
    {
        public int Number { get; set; }
        public string InstanceId { get; set; } = string.Empty;
        public string SourceStep { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public double Score { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; } = 1D;
        public double RoiCenterX { get; set; }
        public double RoiCenterY { get; set; }
        public double RoiWidth { get; set; }
        public double RoiHeight { get; set; }
        public double RoiAngle { get; set; }
        public double MeanValue { get; set; }
        public double ValidPixelRatio { get; set; }
        public string RejectReason { get; set; } = string.Empty;

        public string StateText => Accepted ? "OK" : "NG";

        public VisionPipelineInstanceResult Clone()
        {
            return (VisionPipelineInstanceResult)MemberwiseClone();
        }
    }

    internal static class VisionPipelineMatchResultStore
    {
        private sealed class Holder
        {
            public IReadOnlyList<VisionPipelineMatchResultEvidence> Items { get; set; }
                = Array.Empty<VisionPipelineMatchResultEvidence>();
        }

        private static readonly ConditionalWeakTable<VisionToolResult, Holder> results
            = new ConditionalWeakTable<VisionToolResult, Holder>();

        public static void Set(
            VisionToolResult result,
            IEnumerable<VisionPipelineMatchResultEvidence> items)
        {
            if (result == null)
            {
                return;
            }

            IReadOnlyList<VisionPipelineMatchResultEvidence> stable = (items
                    ?? Enumerable.Empty<VisionPipelineMatchResultEvidence>())
                .Where(item => item != null)
                .Select(item => item.Clone())
                .ToList();
            results.Remove(result);
            results.Add(result, new Holder { Items = stable });
        }

        public static IReadOnlyList<VisionPipelineMatchResultEvidence> Get(
            VisionToolResult result)
        {
            return result != null && results.TryGetValue(result, out Holder holder)
                ? holder.Items
                : Array.Empty<VisionPipelineMatchResultEvidence>();
        }
    }

    internal static class VisionPipelineInstanceResultStore
    {
        private sealed class Holder
        {
            public IReadOnlyList<VisionPipelineInstanceResult> Items { get; set; }
                = Array.Empty<VisionPipelineInstanceResult>();
        }

        private static readonly ConditionalWeakTable<VisionToolResult, Holder> results
            = new ConditionalWeakTable<VisionToolResult, Holder>();

        public static void Set(
            VisionToolResult result,
            IEnumerable<VisionPipelineInstanceResult> items)
        {
            if (result == null)
            {
                return;
            }

            IReadOnlyList<VisionPipelineInstanceResult> stable = (items
                    ?? Enumerable.Empty<VisionPipelineInstanceResult>())
                .Where(item => item != null)
                .Select(item => item.Clone())
                .OrderBy(item => item.Number)
                .ToList();
            results.Remove(result);
            results.Add(result, new Holder { Items = stable });
        }

        public static IReadOnlyList<VisionPipelineInstanceResult> Get(
            VisionToolResult result)
        {
            return result != null && results.TryGetValue(result, out Holder holder)
                ? holder.Items
                : Array.Empty<VisionPipelineInstanceResult>();
        }
    }

    internal static class VisionPipelineMatchResultCaptureService
    {
        public static void Capture(
            VisionPipelineStep step,
            Mat input,
            IVisionTool executedTool,
            VisionToolResult result)
        {
            if (step == null
                || input == null
                || input.Empty()
                || result?.Success != true)
            {
                return;
            }

            IEnumerable<MatchingResult> matches = executedTool is MatchingTool matching
                ? matching.results
                : executedTool is EdgeBasedTemplateMatchingTool edgeMatching
                    ? edgeMatching.results
                    : null;
            if (matches == null)
            {
                return;
            }

            List<VisionPipelineMatchResultEvidence> captured = matches
                .Where(IsUsable)
                .Select(item => new VisionPipelineMatchResultEvidence
                {
                    NativeIndex = item.Index,
                    SourceStep = step.Name ?? string.Empty,
                    CoordinateLayer = step.InputLayer ?? string.Empty,
                    ImageWidth = input.Width,
                    ImageHeight = input.Height,
                    Score = item.Score,
                    CenterX = item.Center.X,
                    CenterY = item.Center.Y,
                    BoundsX = item.Bounding.X,
                    BoundsY = item.Bounding.Y,
                    BoundsWidth = item.Bounding.Width,
                    BoundsHeight = item.Bounding.Height,
                    Angle = item.Angle,
                    Scale = item.Scale > 0D ? item.Scale : 1D
                })
                .ToList();
            VisionPipelineMatchResultStore.Set(result, captured);
        }

        private static bool IsUsable(MatchingResult item)
        {
            return item != null
                && IsFinite(item.Score)
                && IsFinite(item.Center.X)
                && IsFinite(item.Center.Y)
                && IsFinite(item.Bounding.X)
                && IsFinite(item.Bounding.Y)
                && IsFinite(item.Bounding.Width)
                && IsFinite(item.Bounding.Height)
                && IsFinite(item.Angle)
                && IsFinite(item.Scale)
                && item.Bounding.Width > 0D
                && item.Bounding.Height > 0D
                && item.Scale > 0D;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
