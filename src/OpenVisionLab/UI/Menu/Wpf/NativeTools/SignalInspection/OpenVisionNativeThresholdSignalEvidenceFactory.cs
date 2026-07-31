using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeThresholdSignalEvidenceFactory
    {
        public const string ThresholdMarkerId = "Threshold";
        public const string LowerMarkerId = "Lower";
        public const string UpperMarkerId = "Upper";

        public static VisionToolSignalEvidence Create(
            Mat source,
            Mat result,
            ThresholdToolProperty property,
            string inputLayer)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            string parameterSummary = CreateParameterSummary(property);
            string sourceHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(source);
            string resultHash = OpenVisionNativeGraySignalEvidenceCalculator.ComputeImageSha256(result);
            IReadOnlyList<VisionToolSignalMarker> markers = CreateMarkers(property);
            string markerIdentity = string.Join(
                "|",
                System.Linq.Enumerable.Select(
                    markers,
                    marker => string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}:{1:0.###############}",
                        marker.Id,
                        marker.X)));
            string toolIdentity = "Threshold/" + property.Mode;
            string evidenceId = OpenVisionNativeGraySignalEvidenceCalculator.CreateEvidenceId(
                toolIdentity,
                inputLayer ?? string.Empty,
                "Full image",
                parameterSummary,
                markerIdentity,
                sourceHash,
                resultHash);

            return new VisionToolSignalEvidence(
                evidenceId,
                sourceHash,
                resultHash,
                toolIdentity,
                inputLayer,
                "Full image",
                parameterSummary,
                "Gray level",
                "Pixel %",
                new[]
                {
                    new VisionToolSignalSeries(
                        "Gray population",
                        "#1F77B4",
                        0,
                        1,
                        OpenVisionNativeGraySignalEvidenceCalculator.CreateNormalizedHistogram(source))
                },
                markers,
                CreateGuidance(property));
        }

        private static IReadOnlyList<VisionToolSignalMarker> CreateMarkers(ThresholdToolProperty property)
        {
            if (property.Mode == ThresholdToolMode.Range)
            {
                return new[]
                {
                    new VisionToolSignalMarker(LowerMarkerId, "Lower", property.RangeMin, "#2E8B57", true),
                    new VisionToolSignalMarker(UpperMarkerId, "Upper", property.RangeMax, "#C0392B", true)
                };
            }

            if (property.Mode == ThresholdToolMode.Threshold)
            {
                return new[]
                {
                    new VisionToolSignalMarker(ThresholdMarkerId, "T", property.Threshold, "#C0392B", true)
                };
            }

            return Array.Empty<VisionToolSignalMarker>();
        }

        private static string CreateParameterSummary(ThresholdToolProperty property)
        {
            switch (property.Mode)
            {
                case ThresholdToolMode.Range:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "Range / Lower {0} / Upper {1} / {2}",
                        property.RangeMin,
                        property.RangeMax,
                        property.Invert ? "Invert" : "Normal");
                case ThresholdToolMode.Adaptive:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "Adaptive / {0} / {1} / Block {2} / C {3}",
                        property.AdaptiveType,
                        property.AdaptiveThresholdType,
                        property.BlockSize,
                        property.Weight);
                default:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "Basic / T {0:0.#} / {1} / Max {2:0.#}",
                        property.Threshold,
                        property.ThresholdType,
                        property.MaxValue);
            }
        }

        private static string CreateGuidance(ThresholdToolProperty property)
        {
            switch (property.Mode)
            {
                case ThresholdToolMode.Range:
                    return property.Invert
                        ? "Drag Lower/Upper to teach the excluded gray interval. Commit occurs on release."
                        : "Drag Lower/Upper to teach the included gray interval. Commit occurs on release.";
                case ThresholdToolMode.Adaptive:
                    return "Adaptive uses a local cutoff, so this global distribution has no editable cutoff marker.";
                default:
                    return property.ThresholdType == ThresholdTypes.BinaryInv
                        ? "Drag T to teach the cutoff; Binary Inv selects gray values at or below T."
                        : "Drag T to teach the cutoff; Binary selects gray values above T.";
            }
        }
    }
}
