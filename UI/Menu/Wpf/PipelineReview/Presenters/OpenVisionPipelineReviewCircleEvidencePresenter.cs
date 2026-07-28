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
    internal static class OpenVisionPipelineReviewCircleEvidencePresenter
    {
        public static VisionToolSignalEvidence CreateResidualEvidence(
            VisionPipelineCircleEvidence circle,
            Bitmap sourceImage,
            Bitmap resultImage)
        {
            if (circle?.Samples == null || circle.Samples.Count == 0)
            {
                return null;
            }

            string sourceHash = ComputeBitmapSha256(sourceImage);
            string resultHash = ComputeBitmapSha256(resultImage);
            string parameters = FormatParameters(circle);
            double[] values = circle.Samples
                .Select(sample => sample.HasFitResidual ? Math.Abs(sample.FitResidualPx) : 0D)
                .ToArray();
            string canonical = string.Join(
                "|",
                "CircleResidual",
                sourceHash,
                resultHash,
                parameters,
                string.Join(",", circle.Samples.Select(sample =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}:{1:0.###############}:{2}:{3}",
                        sample.Number,
                        sample.FitResidualPx,
                        sample.FitInlier,
                        sample.ContrastAccepted))));
            return new VisionToolSignalEvidence(
                ComputeTextSha256(canonical),
                sourceHash,
                resultHash,
                "CircleGauge/" + circle.StepName,
                circle.InputLayer,
                $"radial scans {circle.StartAngleDeg:0.###}..{circle.StartAngleDeg + circle.SweepAngleDeg:0.###} deg",
                parameters,
                "Scan #",
                "|Radial residual| (px)",
                new[]
                {
                    new VisionToolSignalSeries(
                        "Absolute residual (px)",
                        "#8E44AD",
                        1D,
                        1D,
                        values)
                },
                guidance:
                    "Click one scan to select its row and image drawing. Zero means no fitted residual was available; use the row state.",
                attributes: CreateAttributes(circle));
        }

        public static VisionToolSignalEvidence CreateProfileEvidence(
            VisionPipelineCircleEvidence circle,
            VisionPipelineCircleSampleEvidence sample,
            Bitmap sourceImage,
            Bitmap resultImage,
            string retainedSourceSha256 = null,
            string retainedResultSha256 = null)
        {
            if (circle == null
                || sample == null
                || sample.IntensityValues == null
                || sample.SignedResponseValues == null
                || sample.IntensityValues.Count == 0
                || sample.SignedResponseValues.Count == 0)
            {
                return null;
            }

            int count = Math.Min(sample.IntensityValues.Count, sample.SignedResponseValues.Count);
            double[] intensities = sample.IntensityValues.Take(count).ToArray();
            double[] responses = sample.SignedResponseValues.Take(count).ToArray();
            string sourceHash = IsSha256(retainedSourceSha256)
                ? retainedSourceSha256
                : ComputeBitmapSha256(sourceImage);
            string resultHash = IsSha256(retainedResultSha256)
                ? retainedResultSha256
                : ComputeBitmapSha256(resultImage);
            string parameters = FormatParameters(circle);
            string canonical = string.Join(
                "|",
                "CircleProfile",
                sourceHash,
                resultHash,
                parameters,
                sample.Number.ToString(CultureInfo.InvariantCulture),
                string.Join(",", intensities.Select(value => value.ToString("0.###############", CultureInfo.InvariantCulture))),
                string.Join(",", responses.Select(value => value.ToString("0.###############", CultureInfo.InvariantCulture))));
            List<VisionToolSignalMarker> markers = new List<VisionToolSignalMarker>();
            if (sample.HasEdgePoint)
            {
                markers.Add(new VisionToolSignalMarker(
                    "SelectedEdge",
                    "Selected edge",
                    sample.EdgeRadiusPx,
                    "#C0392B",
                    false));
            }

            return new VisionToolSignalEvidence(
                ComputeTextSha256(canonical),
                sourceHash,
                resultHash,
                "CircleGauge/" + circle.StepName + "/scan " + sample.Number,
                circle.InputLayer,
                $"angle {sample.AngleDeg:0.###} deg / radius {circle.RadiusMinPx:0.###}..{circle.RadiusMaxPx:0.###} px",
                parameters,
                "Radius (px)",
                "GV / signed response",
                new[]
                {
                    new VisionToolSignalSeries(
                        "Intensity (GV)",
                        "#2276B9",
                        sample.ProfileRadiusStartPx,
                        sample.ProfileRadiusStepPx,
                        intensities),
                    new VisionToolSignalSeries(
                        "Signed edge response (ΔGV)",
                        "#D35400",
                        sample.ProfileRadiusStartPx,
                        sample.ProfileRadiusStepPx,
                        responses)
                },
                markers,
                $"State {sample.StateText}. Edge strength {sample.EdgeStrengthGv:0.###} GV; "
                    + $"contrast gate {circle.MinimumContrastGv:0.###} GV. {sample.RejectReason}",
                CreateAttributes(circle, sample));
        }

        private static IEnumerable<KeyValuePair<string, string>> CreateAttributes(
            VisionPipelineCircleEvidence circle,
            VisionPipelineCircleSampleEvidence sample = null)
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
            {
                Pair("SupportGate", $"{circle.SupportRatio:0.###############}>={circle.MinimumSupportRatio:0.###############}"),
                Pair("ResidualGatePx", $"{circle.FitResidualPx:0.###############}<={circle.MaximumFitResidualPx:0.###############}"),
                Pair("RobustRejectionPx", circle.RobustRejectionPx.ToString("0.###############", CultureInfo.InvariantCulture)),
                Pair("InlierCount", circle.InlierCount.ToString(CultureInfo.InvariantCulture)),
                Pair("EdgeCandidateCount", circle.EdgeCandidateCount.ToString(CultureInfo.InvariantCulture))
            };
            if (sample != null)
            {
                values.Add(Pair("SelectedScan", sample.Number.ToString(CultureInfo.InvariantCulture)));
                values.Add(Pair("SelectedImagePoint", sample.PointText));
                values.Add(Pair("SelectedState", sample.StateText));
            }

            return values;
        }

        private static KeyValuePair<string, string> Pair(string key, string value)
        {
            return new KeyValuePair<string, string>(key, value ?? string.Empty);
        }

        private static string FormatParameters(VisionPipelineCircleEvidence circle)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Center=({0:0.###},{1:0.###}) / R={2:0.###}..{3:0.###}px / scans={4} / polarity={5} / contrast>={6:0.###}GV / support>={7:0.###} / RMS<={8:0.###}px",
                circle.TaughtCenterX,
                circle.TaughtCenterY,
                circle.RadiusMinPx,
                circle.RadiusMaxPx,
                circle.ScanCount,
                circle.EdgePolarity,
                circle.MinimumContrastGv,
                circle.MinimumSupportRatio,
                circle.MaximumFitResidualPx);
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

        private static bool IsSha256(string value)
        {
            return value?.Length == 64 && value.All(Uri.IsHexDigit);
        }
    }
}
