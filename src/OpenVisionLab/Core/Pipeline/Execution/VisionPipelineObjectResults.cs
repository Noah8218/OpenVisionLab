using OpenVisionLab.Vision2D.Blob;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Result;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using DrawingPointF = System.Drawing.PointF;
using DrawingRectangleF = System.Drawing.RectangleF;

namespace OpenVisionLab
{
    public sealed class VisionPipelineObjectResult
    {
        public int Number { get; set; }
        public bool Accepted { get; set; }
        public double Area { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public int BoundsX { get; set; }
        public int BoundsY { get; set; }
        public int BoundsWidth { get; set; }
        public int BoundsHeight { get; set; }
        public double Angle { get; set; }
        public string RejectReason { get; set; } = string.Empty;

        public string StateText => Accepted ? "OK" : "REJECT";
    }

    internal static class VisionPipelineObjectResultStore
    {
        private sealed class Holder
        {
            public IReadOnlyList<VisionPipelineObjectResult> Items { get; set; } = Array.Empty<VisionPipelineObjectResult>();
        }

        private static readonly ConditionalWeakTable<VisionToolResult, Holder> results = new ConditionalWeakTable<VisionToolResult, Holder>();

        public static void Set(VisionToolResult result, IReadOnlyList<VisionPipelineObjectResult> items)
        {
            if (result == null)
            {
                return;
            }

            results.Remove(result);
            results.Add(result, new Holder { Items = items ?? Array.Empty<VisionPipelineObjectResult>() });
        }

        public static IReadOnlyList<VisionPipelineObjectResult> Get(VisionToolResult result)
        {
            return result != null && results.TryGetValue(result, out Holder holder)
                ? holder.Items
                : Array.Empty<VisionPipelineObjectResult>();
        }
    }

    internal static class VisionPipelineObjectResultCaptureService
    {
        private const int UnboundedMaximum = 1000000;

        public static void Capture(
            VisionPipelineStep step,
            Mat input,
            IVisionTool executedTool,
            VisionToolResult toolResult)
        {
            if (step == null || input == null || input.Empty() || executedTool == null || toolResult == null)
            {
                return;
            }

            if (!(executedTool is BlobTool) && !(executedTool is ContourTool))
            {
                return;
            }

            ObjectFilterCriteria criteria = ObjectFilterCriteria.From(step.Parameters);
            ApplyFilter(executedTool, toolResult, criteria);

            int auditMinimumArea = executedTool is ContourTool
                ? Math.Max(1, criteria.MinimumArea / 4)
                : 0;
            List<VisionPipelineObjectResult> rows = TryCaptureUnfiltered(
                step,
                input,
                criteria,
                auditMinimumArea);
            if (rows.Count == 0)
            {
                rows = CaptureAccepted(executedTool, criteria);
            }

            VisionPipelineObjectResultStore.Set(toolResult, Stabilize(rows));
        }

        public static void ApplyNativeFilter(
            BlobProperty property,
            BlobTool tool,
            VisionToolResult toolResult)
        {
            ApplyFilter(tool, toolResult, ObjectFilterCriteria.From(property));
        }

        public static void ApplyNativeFilter(
            ContourProperty property,
            ContourTool tool,
            VisionToolResult toolResult)
        {
            ApplyFilter(tool, toolResult, ObjectFilterCriteria.From(property));
        }

        private static List<VisionPipelineObjectResult> TryCaptureUnfiltered(
            VisionPipelineStep step,
            Mat input,
            ObjectFilterCriteria criteria,
            int auditMinimumArea)
        {
            VisionToolResult auditResult = null;
            try
            {
                VisionPipelineStep auditStep = CloneForAreaAudit(step, auditMinimumArea);
                IVisionTool auditTool = VisionPipelineAppToolFactory.Create(auditStep);
                using Mat auditInput = input.Clone();
                auditResult = auditTool.Execute(auditInput);
                if (auditResult?.Success != true)
                {
                    return new List<VisionPipelineObjectResult>();
                }

                return CaptureAll(auditTool, criteria);
            }
            catch
            {
                return new List<VisionPipelineObjectResult>();
            }
            finally
            {
                auditResult?.ResultImage?.Dispose();
            }
        }

        private static VisionPipelineStep CloneForAreaAudit(VisionPipelineStep source, int auditMinimumArea)
        {
            VisionPipelineStep clone = new VisionPipelineStep
            {
                Name = source.Name,
                ToolType = source.ToolType,
                Enabled = source.Enabled,
                InputLayer = source.InputLayer,
                OutputLayer = source.OutputLayer
            };
            foreach (KeyValuePair<string, string> parameter in source.Parameters ?? new Dictionary<string, string>())
            {
                clone.Parameters[parameter.Key] = parameter.Value;
            }

            clone.Parameters["MIN_AREA"] = auditMinimumArea.ToString(CultureInfo.InvariantCulture);
            clone.Parameters["MAX_AREA"] = int.MaxValue.ToString(CultureInfo.InvariantCulture);
            return clone;
        }

        private static List<VisionPipelineObjectResult> CaptureAll(
            IVisionTool tool,
            ObjectFilterCriteria criteria)
        {
            if (tool is BlobTool blob)
            {
                return (blob.results ?? new List<BlobResult>())
                    .Where(item => item != null)
                    .Select(item => Create(
                        item.Area,
                        item.Center.X,
                        item.Center.Y,
                        item.Bounding.X,
                        item.Bounding.Y,
                        item.Bounding.Width,
                        item.Bounding.Height,
                        item.Angle,
                        criteria))
                    .ToList();
            }

            if (tool is ContourTool contour)
            {
                return (contour.results ?? new List<ContourResult>())
                    .Where(item => item != null)
                    .Select(item => Create(
                        item.Area,
                        item.Center.X,
                        item.Center.Y,
                        item.Bounding.X,
                        item.Bounding.Y,
                        item.Bounding.Width,
                        item.Bounding.Height,
                        item.Angle,
                        criteria))
                    .ToList();
            }

            return new List<VisionPipelineObjectResult>();
        }

        private static List<VisionPipelineObjectResult> CaptureAccepted(
            IVisionTool tool,
            ObjectFilterCriteria criteria)
        {
            List<VisionPipelineObjectResult> rows = CaptureAll(tool, criteria);
            foreach (VisionPipelineObjectResult row in rows)
            {
                row.Accepted = true;
                row.RejectReason = string.Empty;
            }

            return rows;
        }

        private static VisionPipelineObjectResult Create(
            double area,
            double centerX,
            double centerY,
            int boundsX,
            int boundsY,
            int boundsWidth,
            int boundsHeight,
            double angle,
            ObjectFilterCriteria criteria)
        {
            string reason = criteria.GetRejectReason(area, boundsWidth, boundsHeight);
            return new VisionPipelineObjectResult
            {
                Accepted = string.IsNullOrEmpty(reason),
                Area = area,
                CenterX = centerX,
                CenterY = centerY,
                BoundsX = boundsX,
                BoundsY = boundsY,
                BoundsWidth = boundsWidth,
                BoundsHeight = boundsHeight,
                Angle = angle,
                RejectReason = reason
            };
        }

        private static void ApplyFilter(
            IVisionTool tool,
            VisionToolResult toolResult,
            ObjectFilterCriteria criteria)
        {
            if (toolResult?.Success != true || criteria == null)
            {
                return;
            }

            List<VisionPipelineObjectResult> originalRows = CaptureAll(tool, criteria);
            if (tool is BlobTool blob)
            {
                blob.results?.RemoveAll(item =>
                    item == null
                    || !criteria.IsAccepted(item.Area, item.Bounding.Width, item.Bounding.Height));
            }
            else if (tool is ContourTool contour)
            {
                contour.results?.RemoveAll(item =>
                    item == null
                    || !criteria.IsAccepted(item.Area, item.Bounding.Width, item.Bounding.Height));
            }

            List<VisionPipelineObjectResult> acceptedRows = originalRows
                .Where(item => item.Accepted)
                .ToList();
            SynchronizeMetrics(toolResult, acceptedRows);
            SynchronizeObjectOverlays(toolResult, originalRows, acceptedRows);
        }

        private static void SynchronizeMetrics(
            VisionToolResult toolResult,
            IReadOnlyList<VisionPipelineObjectResult> acceptedRows)
        {
            if (toolResult?.Metrics == null)
            {
                return;
            }

            string[] objectMetricNames =
            {
                VisionPipelineKnownMetrics.ResultCount,
                VisionPipelineKnownMetrics.AreaMin,
                VisionPipelineKnownMetrics.AreaMax,
                VisionPipelineKnownMetrics.AreaAvg,
                VisionPipelineKnownMetrics.AngleMin,
                VisionPipelineKnownMetrics.AngleMax,
                VisionPipelineKnownMetrics.AngleAvg,
                VisionPipelineKnownMetrics.BoundsWidthMin,
                VisionPipelineKnownMetrics.BoundsWidthMax,
                VisionPipelineKnownMetrics.BoundsWidthAvg,
                VisionPipelineKnownMetrics.BoundsWidthMmMin,
                VisionPipelineKnownMetrics.BoundsWidthMmMax,
                VisionPipelineKnownMetrics.BoundsWidthMmAvg,
                VisionPipelineKnownMetrics.BoundsHeightMin,
                VisionPipelineKnownMetrics.BoundsHeightMax,
                VisionPipelineKnownMetrics.BoundsHeightAvg,
                VisionPipelineKnownMetrics.BoundsHeightMmMin,
                VisionPipelineKnownMetrics.BoundsHeightMmMax,
                VisionPipelineKnownMetrics.BoundsHeightMmAvg
            };
            foreach (string metricName in objectMetricNames)
            {
                toolResult.Metrics.Remove(metricName);
            }

            toolResult.Metrics[VisionPipelineKnownMetrics.ResultCount] = acceptedRows?.Count ?? 0;
            if (acceptedRows == null || acceptedRows.Count == 0)
            {
                return;
            }

            toolResult.Metrics[VisionPipelineKnownMetrics.AreaMin] = acceptedRows.Min(item => item.Area);
            toolResult.Metrics[VisionPipelineKnownMetrics.AreaMax] = acceptedRows.Max(item => item.Area);
            toolResult.Metrics[VisionPipelineKnownMetrics.AreaAvg] = acceptedRows.Average(item => item.Area);
            toolResult.Metrics[VisionPipelineKnownMetrics.AngleMin] = acceptedRows.Min(item => item.Angle);
            toolResult.Metrics[VisionPipelineKnownMetrics.AngleMax] = acceptedRows.Max(item => item.Angle);
            toolResult.Metrics[VisionPipelineKnownMetrics.AngleAvg] = acceptedRows.Average(item => item.Angle);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsWidthMin] = acceptedRows.Min(item => item.BoundsWidth);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsWidthMax] = acceptedRows.Max(item => item.BoundsWidth);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsWidthAvg] = acceptedRows.Average(item => item.BoundsWidth);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsHeightMin] = acceptedRows.Min(item => item.BoundsHeight);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsHeightMax] = acceptedRows.Max(item => item.BoundsHeight);
            toolResult.Metrics[VisionPipelineKnownMetrics.BoundsHeightAvg] = acceptedRows.Average(item => item.BoundsHeight);
        }

        private static void SynchronizeObjectOverlays(
            VisionToolResult toolResult,
            IReadOnlyList<VisionPipelineObjectResult> originalRows,
            IReadOnlyList<VisionPipelineObjectResult> acceptedRows)
        {
            if (toolResult?.Overlays == null)
            {
                return;
            }

            for (int index = toolResult.Overlays.Count - 1; index >= 0; index--)
            {
                VisionToolOverlay overlay = toolResult.Overlays[index];
                if (overlay?.Kind == VisionToolOverlayKind.Rectangle
                    && originalRows.Any(row => Matches(row, overlay.Bounds)))
                {
                    toolResult.Overlays.RemoveAt(index);
                }
            }

            foreach (VisionPipelineObjectResult row in acceptedRows)
            {
                toolResult.Overlays.Add(new VisionToolOverlay
                {
                    Kind = VisionToolOverlayKind.Rectangle,
                    Label = "Accepted object",
                    Bounds = new DrawingRectangleF(
                        row.BoundsX,
                        row.BoundsY,
                        row.BoundsWidth,
                        row.BoundsHeight),
                    Center = new DrawingPointF((float)row.CenterX, (float)row.CenterY),
                    Angle = row.Angle
                });
            }
        }

        private static bool Matches(VisionPipelineObjectResult row, DrawingRectangleF bounds)
        {
            const float tolerance = 0.5f;
            return Math.Abs(row.BoundsX - bounds.X) <= tolerance
                && Math.Abs(row.BoundsY - bounds.Y) <= tolerance
                && Math.Abs(row.BoundsWidth - bounds.Width) <= tolerance
                && Math.Abs(row.BoundsHeight - bounds.Height) <= tolerance;
        }

        private static IReadOnlyList<VisionPipelineObjectResult> Stabilize(IEnumerable<VisionPipelineObjectResult> rows)
        {
            List<VisionPipelineObjectResult> stable = (rows ?? Enumerable.Empty<VisionPipelineObjectResult>())
                .OrderBy(item => item.CenterY)
                .ThenBy(item => item.CenterX)
                .ThenByDescending(item => item.Area)
                .ToList();
            for (int index = 0; index < stable.Count; index++)
            {
                stable[index].Number = index + 1;
            }

            return stable;
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int fallback)
        {
            return parameters != null
                && parameters.TryGetValue(key, out string text)
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                    ? value
                    : fallback;
        }

        private sealed class ObjectFilterCriteria
        {
            public int MinimumArea { get; private set; }
            public int MaximumArea { get; private set; }
            public int MinimumWidth { get; private set; }
            public int MaximumWidth { get; private set; }
            public int MinimumHeight { get; private set; }
            public int MaximumHeight { get; private set; }

            public static ObjectFilterCriteria From(IDictionary<string, string> parameters)
            {
                return new ObjectFilterCriteria
                {
                    MinimumArea = GetInt(parameters, "MIN_AREA", 200),
                    MaximumArea = GetInt(parameters, "MAX_AREA", UnboundedMaximum),
                    MinimumWidth = GetInt(parameters, "MIN_WIDTH", 0),
                    MaximumWidth = GetInt(parameters, "MAX_WIDTH", UnboundedMaximum),
                    MinimumHeight = GetInt(parameters, "MIN_HEIGHT", 0),
                    MaximumHeight = GetInt(parameters, "MAX_HEIGHT", UnboundedMaximum)
                };
            }

            public static ObjectFilterCriteria From(BlobProperty property)
            {
                BlobProperty resolved = property ?? new BlobProperty();
                return new ObjectFilterCriteria
                {
                    MinimumArea = resolved.MIN_AREA,
                    MaximumArea = resolved.MAX_AREA,
                    MinimumWidth = resolved.MIN_WIDTH,
                    MaximumWidth = resolved.MAX_WIDTH,
                    MinimumHeight = resolved.MIN_HEIGHT,
                    MaximumHeight = resolved.MAX_HEIGHT
                };
            }

            public static ObjectFilterCriteria From(ContourProperty property)
            {
                ContourProperty resolved = property ?? new ContourProperty();
                return new ObjectFilterCriteria
                {
                    MinimumArea = resolved.MIN_AREA,
                    MaximumArea = resolved.MAX_AREA,
                    MinimumWidth = resolved.MIN_WIDTH,
                    MaximumWidth = resolved.MAX_WIDTH,
                    MinimumHeight = resolved.MIN_HEIGHT,
                    MaximumHeight = resolved.MAX_HEIGHT
                };
            }

            public bool IsAccepted(double area, int width, int height)
            {
                return string.IsNullOrEmpty(GetRejectReason(area, width, height));
            }

            public string GetRejectReason(double area, int width, int height)
            {
                if (area < MinimumArea)
                {
                    return $"Area {area:0.###} < MIN_AREA {MinimumArea}";
                }

                if (area > MaximumArea)
                {
                    return $"Area {area:0.###} > MAX_AREA {MaximumArea}";
                }

                if (width < MinimumWidth)
                {
                    return $"Width {width} < MIN_WIDTH {MinimumWidth}";
                }

                if (width > MaximumWidth)
                {
                    return $"Width {width} > MAX_WIDTH {MaximumWidth}";
                }

                if (height < MinimumHeight)
                {
                    return $"Height {height} < MIN_HEIGHT {MinimumHeight}";
                }

                if (height > MaximumHeight)
                {
                    return $"Height {height} > MAX_HEIGHT {MaximumHeight}";
                }

                return string.Empty;
            }
        }
    }
}
