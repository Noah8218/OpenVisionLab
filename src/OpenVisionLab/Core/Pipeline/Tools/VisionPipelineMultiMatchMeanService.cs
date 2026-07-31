using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineMultiMatchMeanService
    {
        public const string SourceStepParameter = "SOURCE_STEP";
        public const string ReferenceXParameter = "REFERENCE_X";
        public const string ReferenceYParameter = "REFERENCE_Y";
        public const string ReferenceAngleParameter = "REFERENCE_ANGLE";
        public const string ReferenceScaleParameter = "REFERENCE_SCALE";
        public const string ReferenceImageWidthParameter = "REFERENCE_IMAGE_WIDTH";
        public const string ReferenceImageHeightParameter = "REFERENCE_IMAGE_HEIGHT";
        public const string RelativeRoiParameter = "RELATIVE_ROI";
        public const string MinimumInstancesParameter = "MIN_INSTANCES";
        public const string MaximumInstancesParameter = "MAX_INSTANCES";
        public const string RowToleranceParameter = "ROW_TOLERANCE_PX";
        public const string MaximumOverlapParameter = "MAX_OVERLAP_RATIO";
        public const string MinimumMeanParameter = "MIN_MEAN";
        public const string MaximumMeanParameter = "MAX_MEAN";
        public const string RequireAllParameter = "REQUIRE_ALL";
        public const string MinimumPassCountParameter = "MIN_PASS_COUNT";
        public const string MaximumAngleDeltaParameter = "MAX_ANGLE_DELTA";
        public const string MinimumScaleRatioParameter = "MIN_SCALE_RATIO";
        public const string MaximumScaleRatioParameter = "MAX_SCALE_RATIO";
        public const string MinimumValidPixelRatioParameter = "MIN_VALID_PIXEL_RATIO";

        public const string InstanceCountMetric = "InstanceCount";
        public const string InstancePassCountMetric = "InstancePassCount";
        public const string InstanceFailCountMetric = "InstanceFailCount";
        public const string InstanceAggregatePassedMetric = "InstanceAggregatePassed";
        public const string InstanceMeanMinMetric = "InstanceMeanMin";
        public const string InstanceMeanMaxMetric = "InstanceMeanMax";
        public const string InstanceMeanAvgMetric = "InstanceMeanAvg";
        public const string InstanceScoreMinMetric = "InstanceScoreMin";
        public const string InstanceScoreMaxMetric = "InstanceScoreMax";
        public const string InstanceValidPixelRatioMinMetric = "InstanceValidPixelRatioMin";

        private const int DefaultMinimumInstances = 1;
        private const int DefaultMaximumInstances = 8;
        private const double DefaultRowTolerance = 20D;
        private const double DefaultMaximumOverlap = 0.20D;
        private const double DefaultMinimumMean = 0D;
        private const double DefaultMaximumMean = 255D;
        private const double DefaultMaximumAngleDelta = 10D;
        private const double DefaultMinimumScaleRatio = 0.8D;
        private const double DefaultMaximumScaleRatio = 1.2D;
        private const double DefaultMinimumValidPixelRatio = 0.50D;

        public static bool IsMultiMatchMean(string toolType)
        {
            string normalized = VisionPipelineNormalizer.NormalizeToolType(toolType);
            return normalized == "multimatchmean"
                || normalized == "multifixturemean";
        }

        public static VisionToolResult Execute(
            VisionPipelineStep step,
            Mat input,
            VisionPipelineRunResult runResult)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (step == null || input == null || input.Empty())
            {
                return VisionToolResult.Failed(
                    VisionToolErrorCode.InputImageInvalid,
                    "MultiMatchMean input image is empty.",
                    stopwatch.Elapsed);
            }

            Mat resultImage = CreateColor(input);
            List<VisionToolOverlay> overlays = new List<VisionToolOverlay>();
            Dictionary<string, double> metrics =
                new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            if (!TryReadConfiguration(step, input, out Configuration config, out string configError))
            {
                return Fail(resultImage, stopwatch, configError, metrics, overlays);
            }

            if (!TryResolveSource(
                    step,
                    input,
                    runResult,
                    config.SourceStep,
                    out IReadOnlyList<VisionPipelineMatchResultEvidence> matches,
                    out string sourceError))
            {
                return Fail(resultImage, stopwatch, sourceError, metrics, overlays);
            }

            if (matches.Count < config.MinimumInstances || matches.Count > config.MaximumInstances)
            {
                return Fail(
                    resultImage,
                    stopwatch,
                    $"MultiMatchMean source count {matches.Count} is outside {config.MinimumInstances}..{config.MaximumInstances}.",
                    metrics,
                    overlays);
            }

            if (TryFindOverlap(matches, config.MaximumOverlapRatio, out string overlapError))
            {
                return Fail(resultImage, stopwatch, overlapError, metrics, overlays);
            }

            List<VisionPipelineMatchResultEvidence> ordered =
                OrderRowMajor(matches, config.RowTolerance);
            List<VisionPipelineInstanceResult> instances =
                new List<VisionPipelineInstanceResult>();
            for (int index = 0; index < ordered.Count; index++)
            {
                VisionPipelineInstanceResult instance = InspectInstance(
                    input,
                    resultImage,
                    overlays,
                    ordered[index],
                    index + 1,
                    config);
                instances.Add(instance);
            }

            int passCount = instances.Count(item => item.Accepted);
            int failCount = instances.Count - passCount;
            bool aggregatePassed = config.RequireAll
                ? instances.Count > 0 && failCount == 0
                : passCount >= config.MinimumPassCount;

            metrics[VisionPipelineKnownMetrics.ResultCount] = instances.Count;
            metrics[InstanceCountMetric] = instances.Count;
            metrics[InstancePassCountMetric] = passCount;
            metrics[InstanceFailCountMetric] = failCount;
            metrics[InstanceAggregatePassedMetric] = aggregatePassed ? 1D : 0D;
            if (instances.Count > 0)
            {
                metrics[InstanceScoreMinMetric] = instances.Min(item => item.Score);
                metrics[InstanceScoreMaxMetric] = instances.Max(item => item.Score);
            }

            List<VisionPipelineInstanceResult> measured = instances
                .Where(item => IsFinite(item.MeanValue))
                .ToList();
            if (measured.Count > 0)
            {
                metrics[InstanceMeanMinMetric] = measured.Min(item => item.MeanValue);
                metrics[InstanceMeanMaxMetric] = measured.Max(item => item.MeanValue);
                metrics[InstanceMeanAvgMetric] = measured.Average(item => item.MeanValue);
                metrics[InstanceValidPixelRatioMinMetric] =
                    measured.Min(item => item.ValidPixelRatio);
            }

            DrawStatus(
                resultImage,
                aggregatePassed
                    ? $"OK MultiMatchMean {passCount}/{instances.Count}"
                    : $"NG MultiMatchMean {passCount}/{instances.Count}",
                aggregatePassed);
            stopwatch.Stop();
            VisionToolResult result = VisionToolResult.Passed(
                resultImage,
                stopwatch.Elapsed,
                metrics,
                overlays);
            result.Message = aggregatePassed
                ? $"MultiMatchMean aggregate passed. {passCount}/{instances.Count} instances passed."
                : $"MultiMatchMean aggregate rejected. {passCount}/{instances.Count} instances passed.";
            VisionPipelineInstanceResultStore.Set(result, instances);
            return result;
        }

        private static VisionPipelineInstanceResult InspectInstance(
            Mat input,
            Mat resultImage,
            ICollection<VisionToolOverlay> overlays,
            VisionPipelineMatchResultEvidence match,
            int number,
            Configuration config)
        {
            string id = $"I{number:00}";
            VisionPipelineInstanceResult instance = new VisionPipelineInstanceResult
            {
                Number = number,
                InstanceId = id,
                SourceStep = match.SourceStep,
                Score = match.Score,
                CenterX = match.CenterX,
                CenterY = match.CenterY,
                Angle = match.Angle,
                Scale = match.Scale,
                MeanValue = double.NaN,
                ValidPixelRatio = 0D
            };

            double angleDelta = VisionPipelineFixtureFrameService.NormalizeAngle(
                match.Angle - config.ReferenceAngle);
            double scaleRatio = match.Scale / config.ReferenceScale;
            if (Math.Abs(angleDelta) > config.MaximumAngleDelta)
            {
                instance.RejectReason =
                    $"Angle delta {angleDelta:0.###} exceeds {config.MaximumAngleDelta:0.###} deg";
                DrawInstance(resultImage, overlays, instance, config, angleDelta, false);
                return instance;
            }

            if (!IsFinite(scaleRatio)
                || scaleRatio < config.MinimumScaleRatio
                || scaleRatio > config.MaximumScaleRatio)
            {
                instance.RejectReason =
                    $"Scale ratio {scaleRatio:0.###} is outside {config.MinimumScaleRatio:0.###}..{config.MaximumScaleRatio:0.###}";
                DrawInstance(resultImage, overlays, instance, config, angleDelta, false);
                return instance;
            }

            Dictionary<string, string> normalizeParameters =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    [VisionPipelineFixtureFrameService.ApplyModeParameter] =
                        VisionPipelineFixtureApplyMode.NormalizeImage.ToString(),
                    [VisionPipelineFixtureFrameService.RuntimeReferenceXParameter] =
                        Format(config.ReferenceX),
                    [VisionPipelineFixtureFrameService.RuntimeReferenceYParameter] =
                        Format(config.ReferenceY),
                    [VisionPipelineFixtureFrameService.RuntimeCurrentXParameter] =
                        Format(match.CenterX),
                    [VisionPipelineFixtureFrameService.RuntimeCurrentYParameter] =
                        Format(match.CenterY),
                    [VisionPipelineFixtureFrameService.RuntimeAngleDeltaParameter] =
                        Format(angleDelta),
                    [VisionPipelineFixtureFrameService.RuntimeScaleRatioParameter] =
                        Format(scaleRatio),
                    [VisionPipelineFixtureFrameService.ReferenceImageWidthParameter] =
                        config.ReferenceImageWidth.ToString(CultureInfo.InvariantCulture),
                    [VisionPipelineFixtureFrameService.ReferenceImageHeightParameter] =
                        config.ReferenceImageHeight.ToString(CultureInfo.InvariantCulture),
                    [VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter] =
                        Format(config.MinimumValidPixelRatio)
                };
            VisionToolResult normalized = new VisionPipelineNormalizeImageTool(
                $"Normalize {id}",
                normalizeParameters).Execute(input);
            try
            {
                if (normalized?.Success != true
                    || normalized.ResultImage == null
                    || normalized.ResultImage.Empty())
                {
                    instance.RejectReason = normalized?.Message
                        ?? "NormalizeImage returned no result.";
                    DrawInstance(resultImage, overlays, instance, config, angleDelta, false);
                    return instance;
                }

                if (normalized.Metrics.TryGetValue(
                        VisionPipelineKnownMetrics.FixtureValidPixelRatio,
                        out double validPixelRatio))
                {
                    instance.ValidPixelRatio = validPixelRatio;
                }

                VisionPipelineStep meanStep = new VisionPipelineStep
                {
                    Name = $"Mean {id}",
                    ToolType = "Mean",
                    Enabled = true,
                    InputLayer = "Normalized",
                    OutputLayer = "Measured"
                };
                meanStep.Parameters["Name"] = meanStep.Name;
                meanStep.Parameters["USE_ROI"] = "true";
                meanStep.Parameters["CvROI"] = Format(config.RelativeRoi);
                meanStep.Parameters["USE_THRESHOLD"] = "false";
                meanStep.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
                meanStep.Parameters["USE_BITWISENOT"] = "false";
                meanStep.Parameters["MEAN_MIN"] = "0";
                meanStep.Parameters["MEAN_MAX"] = "255";

                IVisionTool meanTool = VisionPipelineAppToolFactory.Create(meanStep);
                VisionToolResult meanResult = meanTool.Execute(normalized.ResultImage);
                try
                {
                    if (meanResult?.Success != true
                        || !meanResult.Metrics.TryGetValue(
                            VisionPipelineKnownMetrics.MeanValueAvg,
                            out double meanValue)
                        || !IsFinite(meanValue))
                    {
                        instance.RejectReason = meanResult?.Message
                            ?? "Mean inspection returned no finite value.";
                    }
                    else
                    {
                        instance.MeanValue = meanValue;
                        instance.Accepted =
                            meanValue >= config.MinimumMean
                            && meanValue <= config.MaximumMean;
                        instance.RejectReason = instance.Accepted
                            ? string.Empty
                            : $"Mean {meanValue:0.###} is outside {config.MinimumMean:0.###}..{config.MaximumMean:0.###}";
                    }
                }
                finally
                {
                    meanResult?.ResultImage?.Dispose();
                }
            }
            finally
            {
                normalized?.ResultImage?.Dispose();
            }

            DrawInstance(
                resultImage,
                overlays,
                instance,
                config,
                angleDelta,
                instance.Accepted);
            return instance;
        }

        private static void DrawInstance(
            Mat image,
            ICollection<VisionToolOverlay> overlays,
            VisionPipelineInstanceResult instance,
            Configuration config,
            double angleDelta,
            bool accepted)
        {
            Rect referenceRoi = config.RelativeRoi;
            double scale = instance.Scale > 0D
                ? instance.Scale / config.ReferenceScale
                : 1D;
            double radians = -angleDelta * Math.PI / 180D;
            double referenceCenterX = referenceRoi.X + referenceRoi.Width / 2D;
            double referenceCenterY = referenceRoi.Y + referenceRoi.Height / 2D;
            double offsetX = referenceCenterX - config.ReferenceX;
            double offsetY = referenceCenterY - config.ReferenceY;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            double centerX = instance.CenterX + scale * (offsetX * cos - offsetY * sin);
            double centerY = instance.CenterY + scale * (offsetX * sin + offsetY * cos);
            double width = referenceRoi.Width * scale;
            double height = referenceRoi.Height * scale;
            instance.RoiCenterX = centerX;
            instance.RoiCenterY = centerY;
            instance.RoiWidth = width;
            instance.RoiHeight = height;
            instance.RoiAngle = angleDelta;

            Scalar color = accepted ? new Scalar(0, 220, 0) : new Scalar(0, 0, 255);
            RotatedRect rotated = new RotatedRect(
                new Point2f((float)centerX, (float)centerY),
                new OpenCvSharp.Size2f((float)width, (float)height),
                (float)angleDelta);
            Point2f[] corners = rotated.Points();
            OpenCvSharp.Point[] polygon = corners
                .Select(point => new OpenCvSharp.Point(
                    (int)Math.Round(point.X),
                    (int)Math.Round(point.Y)))
                .ToArray();
            Cv2.Polylines(image, new[] { polygon }, true, color, 2, LineTypes.AntiAlias);
            Cv2.PutText(
                image,
                $"{instance.InstanceId} {instance.StateText} {(IsFinite(instance.MeanValue) ? instance.MeanValue.ToString("0.0", CultureInfo.InvariantCulture) : "-")}",
                polygon.OrderBy(point => point.Y).ThenBy(point => point.X).First(),
                HersheyFonts.HersheySimplex,
                0.46D,
                color,
                1,
                LineTypes.AntiAlias);

            overlays.Add(new VisionToolOverlay
            {
                Kind = VisionToolOverlayKind.Rectangle,
                Label = $"{instance.InstanceId} {instance.StateText} mean {(IsFinite(instance.MeanValue) ? instance.MeanValue.ToString("0.0", CultureInfo.InvariantCulture) : "-")}",
                Bounds = new RectangleF(
                    (float)(centerX - width / 2D),
                    (float)(centerY - height / 2D),
                    (float)width,
                    (float)height),
                Center = new PointF((float)centerX, (float)centerY),
                Angle = angleDelta
            });
        }

        private static bool TryReadConfiguration(
            VisionPipelineStep step,
            Mat input,
            out Configuration config,
            out string error)
        {
            config = new Configuration
            {
                SourceStep = GetString(step.Parameters, SourceStepParameter),
                ReferenceX = GetDouble(step.Parameters, ReferenceXParameter, double.NaN),
                ReferenceY = GetDouble(step.Parameters, ReferenceYParameter, double.NaN),
                ReferenceAngle = GetDouble(step.Parameters, ReferenceAngleParameter, double.NaN),
                ReferenceScale = GetDouble(step.Parameters, ReferenceScaleParameter, 1D),
                ReferenceImageWidth = GetInt(
                    step.Parameters,
                    ReferenceImageWidthParameter,
                    input.Width),
                ReferenceImageHeight = GetInt(
                    step.Parameters,
                    ReferenceImageHeightParameter,
                    input.Height),
                MinimumInstances = GetInt(
                    step.Parameters,
                    MinimumInstancesParameter,
                    DefaultMinimumInstances),
                MaximumInstances = GetInt(
                    step.Parameters,
                    MaximumInstancesParameter,
                    DefaultMaximumInstances),
                RowTolerance = GetDouble(
                    step.Parameters,
                    RowToleranceParameter,
                    DefaultRowTolerance),
                MaximumOverlapRatio = GetDouble(
                    step.Parameters,
                    MaximumOverlapParameter,
                    DefaultMaximumOverlap),
                MinimumMean = GetDouble(
                    step.Parameters,
                    MinimumMeanParameter,
                    DefaultMinimumMean),
                MaximumMean = GetDouble(
                    step.Parameters,
                    MaximumMeanParameter,
                    DefaultMaximumMean),
                RequireAll = GetBool(
                    step.Parameters,
                    RequireAllParameter,
                    true),
                MinimumPassCount = GetInt(
                    step.Parameters,
                    MinimumPassCountParameter,
                    1),
                MaximumAngleDelta = GetDouble(
                    step.Parameters,
                    MaximumAngleDeltaParameter,
                    DefaultMaximumAngleDelta),
                MinimumScaleRatio = GetDouble(
                    step.Parameters,
                    MinimumScaleRatioParameter,
                    DefaultMinimumScaleRatio),
                MaximumScaleRatio = GetDouble(
                    step.Parameters,
                    MaximumScaleRatioParameter,
                    DefaultMaximumScaleRatio),
                MinimumValidPixelRatio = GetDouble(
                    step.Parameters,
                    MinimumValidPixelRatioParameter,
                    DefaultMinimumValidPixelRatio)
            };

            if (!TryParseRect(
                    GetString(step.Parameters, RelativeRoiParameter),
                    out Rect relativeRoi))
            {
                error = $"MultiMatchMean requires {RelativeRoiParameter}=X,Y,Width,Height.";
                return false;
            }

            config.RelativeRoi = relativeRoi;
            if (string.IsNullOrWhiteSpace(config.SourceStep))
            {
                error = $"MultiMatchMean requires {SourceStepParameter}.";
                return false;
            }

            if (!IsFinite(config.ReferenceX)
                || !IsFinite(config.ReferenceY)
                || !IsFinite(config.ReferenceAngle)
                || !IsFinite(config.ReferenceScale)
                || config.ReferenceScale <= 0D)
            {
                error = "MultiMatchMean requires a finite reference center/angle and positive scale.";
                return false;
            }

            if (config.ReferenceImageWidth != input.Width
                || config.ReferenceImageHeight != input.Height)
            {
                error =
                    $"MultiMatchMean reference image {config.ReferenceImageWidth}x{config.ReferenceImageHeight} must match input {input.Width}x{input.Height}.";
                return false;
            }

            if (relativeRoi.X < 0
                || relativeRoi.Y < 0
                || relativeRoi.Width <= 0
                || relativeRoi.Height <= 0
                || relativeRoi.Right > config.ReferenceImageWidth
                || relativeRoi.Bottom > config.ReferenceImageHeight)
            {
                error = $"MultiMatchMean {RelativeRoiParameter} must be fully inside the reference image.";
                return false;
            }

            if (config.MinimumInstances <= 0
                || config.MaximumInstances < config.MinimumInstances
                || config.MaximumInstances > 64)
            {
                error = "MultiMatchMean instance limits must satisfy 0 < MIN_INSTANCES <= MAX_INSTANCES <= 64.";
                return false;
            }

            if (!IsFinite(config.RowTolerance)
                || config.RowTolerance < 0D
                || !IsFinite(config.MaximumOverlapRatio)
                || config.MaximumOverlapRatio < 0D
                || config.MaximumOverlapRatio > 1D)
            {
                error = "MultiMatchMean row tolerance and overlap ratio are invalid.";
                return false;
            }

            if (!IsFinite(config.MinimumMean)
                || !IsFinite(config.MaximumMean)
                || config.MinimumMean < 0D
                || config.MaximumMean > 255D
                || config.MinimumMean > config.MaximumMean)
            {
                error = "MultiMatchMean mean range must satisfy 0 <= MIN_MEAN <= MAX_MEAN <= 255.";
                return false;
            }

            if (config.MinimumPassCount <= 0
                || config.MinimumPassCount > config.MaximumInstances)
            {
                error = "MultiMatchMean MIN_PASS_COUNT must be within 1..MAX_INSTANCES.";
                return false;
            }

            if (!IsFinite(config.MaximumAngleDelta)
                || config.MaximumAngleDelta < 0D
                || !IsFinite(config.MinimumScaleRatio)
                || !IsFinite(config.MaximumScaleRatio)
                || config.MinimumScaleRatio <= 0D
                || config.MaximumScaleRatio < config.MinimumScaleRatio
                || !IsFinite(config.MinimumValidPixelRatio)
                || config.MinimumValidPixelRatio <= 0D
                || config.MinimumValidPixelRatio > 1D)
            {
                error = "MultiMatchMean pose or valid-pixel gates are invalid.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryResolveSource(
            VisionPipelineStep consumer,
            Mat input,
            VisionPipelineRunResult runResult,
            string sourceStep,
            out IReadOnlyList<VisionPipelineMatchResultEvidence> matches,
            out string error)
        {
            matches = Array.Empty<VisionPipelineMatchResultEvidence>();
            List<VisionPipelineStepResult> sourceResults = (runResult?.StepResults
                    ?? new List<VisionPipelineStepResult>())
                .Where(item => item?.Step?.Enabled == true
                    && string.Equals(
                        item.Step.Name,
                        sourceStep,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (sourceResults.Count != 1)
            {
                error = sourceResults.Count == 0
                    ? $"MultiMatchMean source Step '{sourceStep}' is not an earlier enabled Step."
                    : $"MultiMatchMean source Step '{sourceStep}' is ambiguous.";
                return false;
            }

            VisionPipelineStepResult source = sourceResults[0];
            string sourceType = VisionPipelineNormalizer.NormalizeToolType(
                source.Step.ToolType);
            if (sourceType != "matching"
                && sourceType != "templatematching"
                && sourceType != "edgebasedmatching"
                && sourceType != "edgebasedtemplatematching"
                && sourceType != "edgetemplatematching")
            {
                error =
                    $"MultiMatchMean source '{sourceStep}' must use Matching or EdgeBasedMatching.";
                return false;
            }

            if (source.ToolResult?.Success != true || !source.AcceptancePassed)
            {
                error =
                    $"MultiMatchMean source '{sourceStep}' did not pass execution and acceptance.";
                return false;
            }

            IReadOnlyList<VisionPipelineMatchResultEvidence> stored =
                VisionPipelineMatchResultStore.Get(source.ToolResult);
            if (stored.Count == 0)
            {
                error =
                    $"MultiMatchMean source '{sourceStep}' retained no matching instances.";
                return false;
            }

            if (!string.Equals(
                    source.Step.InputLayer,
                    consumer.InputLayer,
                    StringComparison.OrdinalIgnoreCase)
                || stored.Any(item =>
                    !string.Equals(
                        item.CoordinateLayer,
                        consumer.InputLayer,
                        StringComparison.OrdinalIgnoreCase)
                    || item.ImageWidth != input.Width
                    || item.ImageHeight != input.Height))
            {
                error =
                    $"MultiMatchMean source '{sourceStep}' does not share the consumer input coordinate frame.";
                return false;
            }

            matches = stored.Select(item => item.Clone()).ToList();
            error = string.Empty;
            return true;
        }

        private static List<VisionPipelineMatchResultEvidence> OrderRowMajor(
            IEnumerable<VisionPipelineMatchResultEvidence> matches,
            double rowTolerance)
        {
            List<VisionPipelineMatchResultEvidence> remaining = matches
                .OrderBy(item => item.CenterY)
                .ThenBy(item => item.CenterX)
                .ToList();
            List<VisionPipelineMatchResultEvidence> ordered =
                new List<VisionPipelineMatchResultEvidence>();
            while (remaining.Count > 0)
            {
                double rowY = remaining[0].CenterY;
                List<VisionPipelineMatchResultEvidence> row = remaining
                    .Where(item => Math.Abs(item.CenterY - rowY) <= rowTolerance)
                    .OrderBy(item => item.CenterX)
                    .ThenBy(item => item.CenterY)
                    .ThenByDescending(item => item.Score)
                    .ToList();
                ordered.AddRange(row);
                foreach (VisionPipelineMatchResultEvidence item in row)
                {
                    remaining.Remove(item);
                }
            }

            return ordered;
        }

        private static bool TryFindOverlap(
            IReadOnlyList<VisionPipelineMatchResultEvidence> matches,
            double maximumOverlap,
            out string error)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                for (int j = i + 1; j < matches.Count; j++)
                {
                    double overlap = IntersectionOverUnion(matches[i], matches[j]);
                    if (overlap > maximumOverlap)
                    {
                        error =
                            $"MultiMatchMean source matches {i + 1} and {j + 1} overlap {overlap:0.###}, above {maximumOverlap:0.###}.";
                        return true;
                    }
                }
            }

            error = string.Empty;
            return false;
        }

        private static double IntersectionOverUnion(
            VisionPipelineMatchResultEvidence a,
            VisionPipelineMatchResultEvidence b)
        {
            double left = Math.Max(a.BoundsX, b.BoundsX);
            double top = Math.Max(a.BoundsY, b.BoundsY);
            double right = Math.Min(
                a.BoundsX + a.BoundsWidth,
                b.BoundsX + b.BoundsWidth);
            double bottom = Math.Min(
                a.BoundsY + a.BoundsHeight,
                b.BoundsY + b.BoundsHeight);
            double intersection =
                Math.Max(0D, right - left) * Math.Max(0D, bottom - top);
            double union =
                a.BoundsWidth * a.BoundsHeight
                + b.BoundsWidth * b.BoundsHeight
                - intersection;
            return union > 0D ? intersection / union : 0D;
        }

        private static VisionToolResult Fail(
            Mat image,
            Stopwatch stopwatch,
            string message,
            IDictionary<string, double> metrics,
            IEnumerable<VisionToolOverlay> overlays)
        {
            DrawStatus(image, "REJECT " + message, false);
            stopwatch.Stop();
            VisionToolResult failed = VisionToolResult.Failed(
                VisionToolErrorCode.InvalidParameter,
                message,
                stopwatch.Elapsed);
            failed.ResultImage = image;
            if (failed.Metrics != null)
            {
                foreach (KeyValuePair<string, double> metric in
                    metrics ?? new Dictionary<string, double>())
                {
                    failed.Metrics[metric.Key] = metric.Value;
                }
            }

            if (failed.Overlays != null)
            {
                failed.Overlays.AddRange(
                    overlays ?? Enumerable.Empty<VisionToolOverlay>());
            }

            return failed;
        }

        private static Mat CreateColor(Mat input)
        {
            Mat color = new Mat();
            if (input.Channels() == 1)
            {
                Cv2.CvtColor(input, color, ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                input.CopyTo(color);
            }

            return color;
        }

        private static void DrawStatus(Mat image, string text, bool success)
        {
            Cv2.PutText(
                image,
                text,
                new OpenCvSharp.Point(8, 22),
                HersheyFonts.HersheySimplex,
                0.55D,
                success ? new Scalar(0, 220, 0) : new Scalar(0, 0, 255),
                2,
                LineTypes.AntiAlias);
        }

        private static bool TryParseRect(string text, out Rect rect)
        {
            rect = default;
            string[] parts = (text ?? string.Empty).Split(',');
            if (parts.Length != 4
                || !int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(parts[2].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(parts[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int height))
            {
                return false;
            }

            rect = new Rect(x, y, width, height);
            return true;
        }

        private static string Format(Rect rect)
        {
            return string.Join(
                ",",
                rect.X.ToString(CultureInfo.InvariantCulture),
                rect.Y.ToString(CultureInfo.InvariantCulture),
                rect.Width.ToString(CultureInfo.InvariantCulture),
                rect.Height.ToString(CultureInfo.InvariantCulture));
        }

        private static string Format(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key)
        {
            return parameters != null
                && parameters.TryGetValue(key, out string value)
                    ? value?.Trim() ?? string.Empty
                    : string.Empty;
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int fallback)
        {
            return int.TryParse(
                GetString(parameters, key),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value)
                    ? value
                    : fallback;
        }

        private static double GetDouble(
            IDictionary<string, string> parameters,
            string key,
            double fallback)
        {
            return double.TryParse(
                GetString(parameters, key),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value)
                    ? value
                    : fallback;
        }

        private static bool GetBool(
            IDictionary<string, string> parameters,
            string key,
            bool fallback)
        {
            return bool.TryParse(GetString(parameters, key), out bool value)
                ? value
                : fallback;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private sealed class Configuration
        {
            public string SourceStep { get; set; } = string.Empty;
            public double ReferenceX { get; set; }
            public double ReferenceY { get; set; }
            public double ReferenceAngle { get; set; }
            public double ReferenceScale { get; set; } = 1D;
            public int ReferenceImageWidth { get; set; }
            public int ReferenceImageHeight { get; set; }
            public Rect RelativeRoi { get; set; }
            public int MinimumInstances { get; set; }
            public int MaximumInstances { get; set; }
            public double RowTolerance { get; set; }
            public double MaximumOverlapRatio { get; set; }
            public double MinimumMean { get; set; }
            public double MaximumMean { get; set; }
            public bool RequireAll { get; set; }
            public int MinimumPassCount { get; set; }
            public double MaximumAngleDelta { get; set; }
            public double MinimumScaleRatio { get; set; }
            public double MaximumScaleRatio { get; set; }
            public double MinimumValidPixelRatio { get; set; }
        }
    }
}
