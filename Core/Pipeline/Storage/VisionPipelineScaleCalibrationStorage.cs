using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    public enum VisionScaleCalibrationUnit
    {
        Millimeter,
        Micrometer,
        Inch
    }

    [XmlRoot("VisionPipelineScaleCalibration")]
    public sealed class VisionPipelineScaleCalibrationRecord
    {
        [XmlAttribute]
        public string Version { get; set; } = VisionPipelineScaleCalibrationStorage.RecordVersion;

        [XmlAttribute]
        public string PipelineName { get; set; } = string.Empty;

        [XmlAttribute]
        public DateTime CreatedUtc { get; set; }

        public string CoordinateLayer { get; set; } = string.Empty;
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string SourceImageSha256 { get; set; } = string.Empty;
        public string PointAIdentity { get; set; } = string.Empty;
        public double PointAX { get; set; }
        public double PointAY { get; set; }
        public string PointBIdentity { get; set; } = string.Empty;
        public double PointBX { get; set; }
        public double PointBY { get; set; }
        public double PixelDistance { get; set; }
        public double KnownDistance { get; set; }
        public VisionScaleCalibrationUnit KnownDistanceUnit { get; set; }
        public double KnownDistanceMillimeters { get; set; }
        public double MillimetersPerPixel { get; set; }

        [XmlArrayItem("Step")]
        public List<string> AppliedStepNames { get; set; } = new List<string>();
    }

    internal sealed class VisionPipelineScaleTargetOption
    {
        public int StepIndex { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public string DisplayText => $"{StepIndex + 1:00} {StepName} ({ToolType})";
    }

    internal static class VisionPipelineScaleCalibrationStorage
    {
        internal const string RecordVersion = "v1";
        private const string PixelPerMmParameter = "PIXELPERMM";
        private static readonly HashSet<string> CompatibleToolTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "line", "linegauge",
            "linedistance", "linedistancegauge",
            "pinarraygap", "adjacentpingap",
            "gapedgepair",
            "curvebandprofile", "darkbandcurve",
            "circlegauge",
            "geometrymeasure", "geometricmeasurement"
        };

        public static bool TryCalculate(
            string pipelineName,
            VisionPipelineGeometryFeatureResult pointA,
            VisionPipelineGeometryFeatureResult pointB,
            double knownDistance,
            VisionScaleCalibrationUnit unit,
            Bitmap coordinateImage,
            out VisionPipelineScaleCalibrationRecord record,
            out string error)
        {
            record = null;
            if (!TryValidatePoint(pointA, "Point A", out error)
                || !TryValidatePoint(pointB, "Point B", out error))
            {
                return false;
            }

            if (string.Equals(pointA.Identity, pointB.Identity, StringComparison.OrdinalIgnoreCase))
            {
                error = "Point A and Point B must be different typed results.";
                return false;
            }

            if (!string.Equals(pointA.CoordinateLayer, pointB.CoordinateLayer, StringComparison.OrdinalIgnoreCase)
                || pointA.ImageWidth != pointB.ImageWidth
                || pointA.ImageHeight != pointB.ImageHeight)
            {
                error = "Point A and Point B must use the same coordinate layer and image dimensions.";
                return false;
            }

            if (coordinateImage == null
                || coordinateImage.Width != pointA.ImageWidth
                || coordinateImage.Height != pointA.ImageHeight)
            {
                error = "The current coordinate-layer image is missing or its dimensions do not match the selected points.";
                return false;
            }

            if (!IsFinite(knownDistance) || knownDistance <= 0D)
            {
                error = "Known distance must be finite and greater than zero.";
                return false;
            }

            double dx = pointB.CenterX - pointA.CenterX;
            double dy = pointB.CenterY - pointA.CenterY;
            double pixelDistance = Math.Sqrt(dx * dx + dy * dy);
            if (!IsFinite(pixelDistance) || pixelDistance <= 0.000000001D)
            {
                error = "Selected points are coincident; pixel distance must be greater than zero.";
                return false;
            }

            double millimeters = ConvertToMillimeters(knownDistance, unit);
            double millimetersPerPixel = millimeters / pixelDistance;
            if (!IsFinite(millimetersPerPixel) || millimetersPerPixel <= 0D)
            {
                error = "The derived millimeters-per-pixel value is invalid.";
                return false;
            }

            record = new VisionPipelineScaleCalibrationRecord
            {
                Version = RecordVersion,
                PipelineName = NormalizePipelineName(pipelineName),
                CreatedUtc = DateTime.UtcNow,
                CoordinateLayer = pointA.CoordinateLayer?.Trim() ?? string.Empty,
                ImageWidth = pointA.ImageWidth,
                ImageHeight = pointA.ImageHeight,
                SourceImageSha256 = ComputeBitmapSha256(coordinateImage),
                PointAIdentity = pointA.Identity,
                PointAX = pointA.CenterX,
                PointAY = pointA.CenterY,
                PointBIdentity = pointB.Identity,
                PointBX = pointB.CenterX,
                PointBY = pointB.CenterY,
                PixelDistance = pixelDistance,
                KnownDistance = knownDistance,
                KnownDistanceUnit = unit,
                KnownDistanceMillimeters = millimeters,
                MillimetersPerPixel = millimetersPerPixel
            };
            error = string.Empty;
            return true;
        }

        public static bool TrySave(
            string recipeName,
            VisionPipelineScaleCalibrationRecord record,
            out string path,
            out string error)
        {
            path = string.Empty;
            if (!IsValidRecord(record, out error))
            {
                return false;
            }

            try
            {
                path = GetPath(recipeName, record.PipelineName);
                if (!SerializeHelper.SaveXmlFile(path, record))
                {
                    error = "Scale calibration evidence could not be saved.";
                    return false;
                }

                if (!TryLoad(recipeName, record.PipelineName, out VisionPipelineScaleCalibrationRecord loaded, out error)
                    || !RecordsMatch(record, loaded))
                {
                    error = string.IsNullOrWhiteSpace(error)
                        ? "Scale calibration evidence did not round-trip exactly."
                        : error;
                    return false;
                }

                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool TryLoad(
            string recipeName,
            string pipelineName,
            out VisionPipelineScaleCalibrationRecord record,
            out string error)
        {
            record = null;
            error = string.Empty;
            try
            {
                string path = GetPath(recipeName, pipelineName);
                if (!File.Exists(path))
                {
                    error = "No saved two-point scale evidence exists for this pipeline.";
                    return false;
                }

                if (!SerializeHelper.TryLoadFromXmlFile(path, out record)
                    || !IsValidRecord(record, out error)
                    || !string.Equals(record.PipelineName, NormalizePipelineName(pipelineName), StringComparison.OrdinalIgnoreCase))
                {
                    record = null;
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        error = "Saved two-point scale evidence is invalid.";
                    }
                    return false;
                }

                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool TryValidateCurrentSource(
            VisionPipelineScaleCalibrationRecord record,
            string coordinateLayer,
            Bitmap coordinateImage,
            out string error)
        {
            if (!IsValidRecord(record, out error))
            {
                return false;
            }

            if (!string.Equals(record.CoordinateLayer, coordinateLayer?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                error = "The current coordinate layer differs from the saved scale evidence.";
                return false;
            }

            if (coordinateImage == null
                || coordinateImage.Width != record.ImageWidth
                || coordinateImage.Height != record.ImageHeight)
            {
                error = "The current coordinate-layer image is missing or its dimensions changed.";
                return false;
            }

            string currentHash = ComputeBitmapSha256(coordinateImage);
            if (!string.Equals(currentHash, record.SourceImageSha256, StringComparison.OrdinalIgnoreCase))
            {
                error = "The current coordinate-layer image content changed; recalculate the two-point scale.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public static bool TryApply(
            VisionPipelineScaleCalibrationRecord record,
            Bitmap currentCoordinateImage,
            VisionPipelineStep targetStep,
            out string error)
        {
            if (!TryValidateCurrentSource(record, record?.CoordinateLayer, currentCoordinateImage, out error))
            {
                return false;
            }

            if (targetStep == null || !IsCompatible(targetStep))
            {
                error = "Select one compatible measurement Step.";
                return false;
            }

            if (!string.Equals(targetStep.InputLayer?.Trim(), record.CoordinateLayer, StringComparison.OrdinalIgnoreCase))
            {
                error = "The target Step input layer differs from the calibration coordinate layer.";
                return false;
            }

            string scale = record.MillimetersPerPixel.ToString("0.###############", CultureInfo.InvariantCulture);
            targetStep.Parameters[PixelPerMmParameter] = scale;
            string normalized = NormalizeToolType(targetStep.ToolType);
            if (normalized == "linedistance" || normalized == "linedistancegauge")
            {
                targetStep.Parameters["LeftPIXELPERMM"] = scale;
                targetStep.Parameters["RightPIXELPERMM"] = scale;
            }

            if (!record.AppliedStepNames.Contains(targetStep.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                record.AppliedStepNames.Add(targetStep.Name ?? string.Empty);
            }

            error = string.Empty;
            return true;
        }

        public static IReadOnlyList<VisionPipelineScaleTargetOption> GetCompatibleTargets(VisionPipeline pipeline)
        {
            return (pipeline?.Steps ?? new List<VisionPipelineStep>())
                .Select((step, index) => new { Step = step, Index = index })
                .Where(item => item.Step?.Enabled != false && IsCompatible(item.Step))
                .Select(item => new VisionPipelineScaleTargetOption
                {
                    StepIndex = item.Index,
                    StepName = item.Step.Name ?? string.Empty,
                    ToolType = item.Step.ToolType ?? string.Empty
                })
                .ToList();
        }

        public static bool IsCompatible(VisionPipelineStep step)
        {
            return step != null && CompatibleToolTypes.Contains(NormalizeToolType(step.ToolType));
        }

        public static string ComputeBitmapSha256(Bitmap image)
        {
            if (image == null)
            {
                return string.Empty;
            }

            using Bitmap normalized = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(normalized))
            {
                graphics.DrawImageUnscaled(image, 0, 0);
            }

            Rectangle bounds = new Rectangle(0, 0, normalized.Width, normalized.Height);
            BitmapData data = normalized.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int byteCount = Math.Abs(data.Stride) * data.Height;
                byte[] pixels = new byte[byteCount];
                Marshal.Copy(data.Scan0, pixels, 0, byteCount);
                using SHA256 sha256 = SHA256.Create();
                byte[] metadata = Encoding.UTF8.GetBytes($"{normalized.Width}x{normalized.Height}:32bppArgb:");
                sha256.TransformBlock(metadata, 0, metadata.Length, null, 0);
                sha256.TransformFinalBlock(pixels, 0, pixels.Length);
                return Convert.ToHexString(sha256.Hash);
            }
            finally
            {
                normalized.UnlockBits(data);
            }
        }

        private static bool TryValidatePoint(VisionPipelineGeometryFeatureResult point, string label, out string error)
        {
            if (point == null || point.Kind != VisionPipelineGeometryKind.Point)
            {
                error = label + " must be a typed Point result from the current Run.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(point.Identity)
                || string.IsNullOrWhiteSpace(point.CoordinateLayer)
                || point.ImageWidth <= 0
                || point.ImageHeight <= 0
                || !IsFinite(point.CenterX)
                || !IsFinite(point.CenterY))
            {
                error = label + " has incomplete or non-finite coordinate evidence.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool IsValidRecord(VisionPipelineScaleCalibrationRecord record, out string error)
        {
            if (record == null
                || !string.Equals(record.Version, RecordVersion, StringComparison.Ordinal)
                || string.IsNullOrWhiteSpace(record.PipelineName)
                || string.IsNullOrWhiteSpace(record.CoordinateLayer)
                || record.ImageWidth <= 0
                || record.ImageHeight <= 0
                || string.IsNullOrWhiteSpace(record.SourceImageSha256)
                || string.IsNullOrWhiteSpace(record.PointAIdentity)
                || string.IsNullOrWhiteSpace(record.PointBIdentity)
                || !IsFinite(record.PixelDistance)
                || record.PixelDistance <= 0D
                || !IsFinite(record.KnownDistanceMillimeters)
                || record.KnownDistanceMillimeters <= 0D
                || !IsFinite(record.MillimetersPerPixel)
                || record.MillimetersPerPixel <= 0D)
            {
                error = "Scale calibration evidence is incomplete or invalid.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool RecordsMatch(VisionPipelineScaleCalibrationRecord expected, VisionPipelineScaleCalibrationRecord actual)
        {
            const double tolerance = 0.000000000001D;
            return actual != null
                && string.Equals(expected.Version, actual.Version, StringComparison.Ordinal)
                && string.Equals(expected.PipelineName, actual.PipelineName, StringComparison.Ordinal)
                && string.Equals(expected.CoordinateLayer, actual.CoordinateLayer, StringComparison.Ordinal)
                && expected.ImageWidth == actual.ImageWidth
                && expected.ImageHeight == actual.ImageHeight
                && string.Equals(expected.SourceImageSha256, actual.SourceImageSha256, StringComparison.Ordinal)
                && string.Equals(expected.PointAIdentity, actual.PointAIdentity, StringComparison.Ordinal)
                && string.Equals(expected.PointBIdentity, actual.PointBIdentity, StringComparison.Ordinal)
                && Math.Abs(expected.PointAX - actual.PointAX) <= tolerance
                && Math.Abs(expected.PointAY - actual.PointAY) <= tolerance
                && Math.Abs(expected.PointBX - actual.PointBX) <= tolerance
                && Math.Abs(expected.PointBY - actual.PointBY) <= tolerance
                && Math.Abs(expected.PixelDistance - actual.PixelDistance) <= tolerance
                && Math.Abs(expected.KnownDistance - actual.KnownDistance) <= tolerance
                && expected.KnownDistanceUnit == actual.KnownDistanceUnit
                && Math.Abs(expected.KnownDistanceMillimeters - actual.KnownDistanceMillimeters) <= tolerance
                && Math.Abs(expected.MillimetersPerPixel - actual.MillimetersPerPixel) <= tolerance
                && expected.AppliedStepNames.SequenceEqual(actual.AppliedStepNames, StringComparer.Ordinal);
        }

        private static double ConvertToMillimeters(double value, VisionScaleCalibrationUnit unit)
        {
            return unit == VisionScaleCalibrationUnit.Micrometer
                ? value / 1000D
                : unit == VisionScaleCalibrationUnit.Inch
                    ? value * 25.4D
                    : value;
        }

        private static string GetPath(string recipeName, string pipelineName)
        {
            return RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                NormalizePipelineName(pipelineName) + ".scale-calibration");
        }

        private static string NormalizePipelineName(string pipelineName)
        {
            string value = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim();
            char[] invalid = Path.GetInvalidFileNameChars();
            return new string(value.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
