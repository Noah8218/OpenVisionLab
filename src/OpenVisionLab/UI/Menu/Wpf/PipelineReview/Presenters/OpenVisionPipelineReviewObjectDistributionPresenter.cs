using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal enum VisionPipelineObjectMetricKind
    {
        Area,
        BoundsWidth,
        BoundsHeight
    }

    internal sealed class VisionPipelineObjectMetricDistribution
    {
        public VisionPipelineObjectMetricKind MetricKind { get; init; }
        public string MetricName { get; init; } = string.Empty;
        public string MinimumKey { get; init; } = string.Empty;
        public string MaximumKey { get; init; } = string.Empty;
        public double MinimumValue { get; init; }
        public double MaximumValue { get; init; }
        public bool MaximumIsUnbounded { get; init; }
        public string SummaryText { get; init; } = string.Empty;
        public VisionToolSignalEvidence Evidence { get; init; }

        public double GetValue(VisionPipelineObjectResult item)
        {
            if (item == null)
            {
                return 0D;
            }

            return MetricKind switch
            {
                VisionPipelineObjectMetricKind.BoundsWidth => item.BoundsWidth,
                VisionPipelineObjectMetricKind.BoundsHeight => item.BoundsHeight,
                _ => item.Area
            };
        }
    }

    internal static class OpenVisionPipelineReviewObjectDistributionPresenter
    {
        private const int UnboundedMaximum = 1000000;

        public static VisionPipelineObjectMetricDistribution Create(
            VisionPipelineStep step,
            IEnumerable<VisionPipelineObjectResult> results,
            VisionPipelineObjectMetricKind metricKind,
            Bitmap sourceImage,
            Bitmap resultImage)
        {
            List<VisionPipelineObjectResult> rows = (results ?? Enumerable.Empty<VisionPipelineObjectResult>())
                .Where(item => item != null)
                .OrderBy(item => item.Number)
                .ToList();
            if (step == null || rows.Count == 0)
            {
                return null;
            }

            ResolveMetric(
                metricKind,
                out string metricName,
                out string minimumKey,
                out string maximumKey,
                out int defaultMinimum,
                out Func<VisionPipelineObjectResult, double> valueSelector);
            int minimum = GetInt(step.Parameters, minimumKey, defaultMinimum);
            int maximum = GetInt(step.Parameters, maximumKey, UnboundedMaximum);
            bool maximumIsUnbounded = maximum >= UnboundedMaximum;
            double[] metricValues = rows.Select(valueSelector).ToArray();
            double domainMinimum = Math.Min(metricValues.Min(), minimum);
            double domainMaximum = Math.Max(
                metricValues.Max(),
                maximumIsUnbounded ? metricValues.Max() : maximum);
            if (domainMaximum <= domainMinimum)
            {
                domainMaximum = domainMinimum + 1D;
            }

            int binCount = Math.Clamp(Math.Max(8, rows.Count * 2), 8, 24);
            double binStep = (domainMaximum - domainMinimum) / (binCount - 1);
            double[] accepted = new double[binCount];
            double[] rejected = new double[binCount];
            foreach (VisionPipelineObjectResult row in rows)
            {
                int bin = (int)Math.Round((valueSelector(row) - domainMinimum) / binStep);
                bin = Math.Clamp(bin, 0, binCount - 1);
                if (row.Accepted)
                {
                    accepted[bin]++;
                }
                else
                {
                    rejected[bin]++;
                }
            }

            string sourceHash = ComputeBitmapSha256(sourceImage);
            string resultHash = ComputeBitmapSha256(resultImage);
            string parameters = string.Format(
                CultureInfo.InvariantCulture,
                "{0}={1} / {2}={3}",
                minimumKey,
                minimum,
                maximumKey,
                maximumIsUnbounded ? "unbounded" : maximum.ToString(CultureInfo.InvariantCulture));
            string canonical = string.Join(
                "|",
                "ObjectMetricDistribution",
                NormalizeToolType(step.ToolType),
                sourceHash,
                resultHash,
                metricKind,
                parameters,
                string.Join(
                    ";",
                    rows.Select(row => string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}:{1:0.###############}:{2}:{3}",
                        row.Number,
                        valueSelector(row),
                        row.Accepted,
                        row.RejectReason ?? string.Empty))));
            List<VisionToolSignalMarker> markers = new List<VisionToolSignalMarker>
            {
                new VisionToolSignalMarker(
                    minimumKey,
                    minimumKey,
                    minimum,
                    "#2C7A7B",
                    false)
            };
            if (!maximumIsUnbounded)
            {
                markers.Add(new VisionToolSignalMarker(
                    maximumKey,
                    maximumKey,
                    maximum,
                    "#8E44AD",
                    false));
            }

            string maximumText = maximumIsUnbounded
                ? maximumKey + " unbounded"
                : maximumKey + " " + maximum.ToString(CultureInfo.CurrentCulture);
            string summary = string.Format(
                CultureInfo.CurrentCulture,
                "{0} distribution | objects {1} | OK {2} / REJECT {3} | {4} {5} .. {6}",
                metricName,
                rows.Count,
                rows.Count(item => item.Accepted),
                rows.Count(item => !item.Accepted),
                minimumKey,
                minimum,
                maximumText);
            VisionToolSignalEvidence evidence = new VisionToolSignalEvidence(
                ComputeTextSha256(canonical),
                sourceHash,
                resultHash,
                NormalizeToolType(step.ToolType) + "/" + (step.Name ?? string.Empty),
                step.InputLayer,
                ResolveRegion(step),
                parameters,
                metricKind == VisionPipelineObjectMetricKind.Area
                    ? "Area (px²)"
                    : metricName + " (px)",
                "Object count / bin",
                new[]
                {
                    new VisionToolSignalSeries(
                        "Accepted objects",
                        "#238B65",
                        domainMinimum,
                        binStep,
                        accepted),
                    new VisionToolSignalSeries(
                        "Rejected objects",
                        "#C0392B",
                        domainMinimum,
                        binStep,
                        rejected)
                },
                markers,
                "Select Area, Bounds width, or Bounds height. Click the distribution near one value to select the nearest retained object row and drawing. Range markers are read-only current Pipeline/PropertyGrid values.",
                new[]
                {
                    Pair("Metric", metricName),
                    Pair("MinimumKey", minimumKey),
                    Pair("MinimumValue", minimum.ToString(CultureInfo.InvariantCulture)),
                    Pair("MaximumKey", maximumKey),
                    Pair("MaximumValue", maximum.ToString(CultureInfo.InvariantCulture)),
                    Pair("MaximumIsUnbounded", maximumIsUnbounded.ToString(CultureInfo.InvariantCulture)),
                    Pair("ObjectCount", rows.Count.ToString(CultureInfo.InvariantCulture)),
                    Pair("AcceptedCount", rows.Count(item => item.Accepted).ToString(CultureInfo.InvariantCulture)),
                    Pair("RejectedCount", rows.Count(item => !item.Accepted).ToString(CultureInfo.InvariantCulture))
                });

            return new VisionPipelineObjectMetricDistribution
            {
                MetricKind = metricKind,
                MetricName = metricName,
                MinimumKey = minimumKey,
                MaximumKey = maximumKey,
                MinimumValue = minimum,
                MaximumValue = maximum,
                MaximumIsUnbounded = maximumIsUnbounded,
                SummaryText = summary,
                Evidence = evidence
            };
        }

        private static void ResolveMetric(
            VisionPipelineObjectMetricKind metricKind,
            out string metricName,
            out string minimumKey,
            out string maximumKey,
            out int defaultMinimum,
            out Func<VisionPipelineObjectResult, double> valueSelector)
        {
            switch (metricKind)
            {
                case VisionPipelineObjectMetricKind.BoundsWidth:
                    metricName = "Bounds width";
                    minimumKey = "MIN_WIDTH";
                    maximumKey = "MAX_WIDTH";
                    defaultMinimum = 0;
                    valueSelector = item => item.BoundsWidth;
                    return;
                case VisionPipelineObjectMetricKind.BoundsHeight:
                    metricName = "Bounds height";
                    minimumKey = "MIN_HEIGHT";
                    maximumKey = "MAX_HEIGHT";
                    defaultMinimum = 0;
                    valueSelector = item => item.BoundsHeight;
                    return;
                default:
                    metricName = "Area";
                    minimumKey = "MIN_AREA";
                    maximumKey = "MAX_AREA";
                    defaultMinimum = 200;
                    valueSelector = item => item.Area;
                    return;
            }
        }

        private static int GetInt(IDictionary<string, string> parameters, string key, int fallback)
        {
            return parameters != null
                && parameters.TryGetValue(key, out string text)
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                    ? value
                    : fallback;
        }

        private static string ResolveRegion(VisionPipelineStep step)
        {
            if (step?.Parameters != null
                && step.Parameters.TryGetValue("USE_ROI", out string useRoi)
                && bool.TryParse(useRoi, out bool enabled)
                && enabled
                && step.Parameters.TryGetValue("CvROI", out string roi)
                && !string.IsNullOrWhiteSpace(roi))
            {
                return "ROI " + roi.Trim();
            }

            return "Full image";
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            return value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase)
                ? value.Substring(0, value.Length - 4)
                : value;
        }

        private static KeyValuePair<string, string> Pair(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value ?? string.Empty);
        }

        private static string ComputeBitmapSha256(Bitmap image)
        {
            if (image == null)
            {
                return new string('0', 64);
            }

            using MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return Convert.ToHexString(SHA256.HashData(stream.ToArray()));
        }

        private static string ComputeTextSha256(string value)
        {
            return Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)));
        }
    }
}
