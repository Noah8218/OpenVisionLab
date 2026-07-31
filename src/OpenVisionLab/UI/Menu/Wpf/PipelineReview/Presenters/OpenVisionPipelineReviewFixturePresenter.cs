using Lib.OpenCV.Pipeline;
using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewFixtureState : IDisposable
    {
        public bool IsVisible { get; init; }
        public int ProducerIndex { get; init; } = -1;
        public int NormalizeIndex { get; init; } = -1;
        public int MeasurementIndex { get; init; } = -1;
        public string RelationshipText { get; init; } = string.Empty;
        public string TemplateText { get; init; } = string.Empty;
        public string ReferenceText { get; init; } = string.Empty;
        public string CurrentText { get; init; } = string.Empty;
        public string QualityText { get; init; } = string.Empty;
        public string SourceText { get; init; } = string.Empty;
        public Bitmap SourcePreview { get; init; }
        public string NormalizedText { get; init; } = string.Empty;
        public Bitmap NormalizedPreview { get; init; }
        public Bitmap TemplatePreview { get; init; }
        public bool CanTeachReference { get; init; }
        public bool CanEditProducer { get; init; }
        public bool CanEditMeasurement { get; init; }
        public IReadOnlyList<OpenVisionPipelineReviewFixtureConsumerRow> Consumers { get; init; } =
            Array.Empty<OpenVisionPipelineReviewFixtureConsumerRow>();

        public void Dispose()
        {
            SourcePreview?.Dispose();
            NormalizedPreview?.Dispose();
            TemplatePreview?.Dispose();
        }
    }

    internal static class OpenVisionPipelineReviewFixturePresenter
    {
        public static OpenVisionPipelineReviewFixtureState Create(
            VisionPipeline pipeline,
            Func<VisionPipelineStep, VisionPipelineStepResultSummary> resolveSummary,
            Func<string, Bitmap> resolveLayerPreview,
            int preferredMeasurementIndex = -1)
        {
            IReadOnlyList<VisionPipelineStep> steps = pipeline?.Steps;
            if (!TryResolveFixtureChain(
                    steps,
                    out int producerIndex,
                    out int normalizeIndex,
                    out IReadOnlyList<int> measurementIndices,
                    out string frameName))
            {
                return new OpenVisionPipelineReviewFixtureState();
            }

            int measurementIndex = measurementIndices.Contains(preferredMeasurementIndex)
                ? preferredMeasurementIndex
                : measurementIndices[0];
            VisionPipelineStep producer = steps[producerIndex];
            VisionPipelineStep normalize = steps[normalizeIndex];
            VisionPipelineStep measurement = steps[measurementIndex];
            VisionPipelineStepResultSummary producerSummary = resolveSummary?.Invoke(producer);
            VisionPipelineStepResultSummary normalizeSummary = resolveSummary?.Invoke(normalize);
            IReadOnlyList<OpenVisionPipelineReviewFixtureConsumerRow> consumers =
                measurementIndices
                    .Select(index => CreateConsumerRow(
                        steps,
                        producerIndex,
                        normalizeIndex,
                        index,
                        frameName,
                        resolveSummary?.Invoke(steps[index])))
                    .ToList();

            bool hasPose = TryGetReviewedFixturePose(
                producer,
                producerSummary,
                out double currentX,
                out double currentY,
                out double currentAngle,
                out double currentScale);
            double referenceX = 0d;
            double referenceY = 0d;
            double referenceAngle = 0d;
            double referenceScale = 0d;
            bool hasReference =
                TryGetParameterDouble(
                    producer,
                    VisionPipelineFixtureFrameService.ReferenceXParameter,
                    out referenceX)
                && TryGetParameterDouble(
                    producer,
                    VisionPipelineFixtureFrameService.ReferenceYParameter,
                    out referenceY)
                && TryGetParameterDouble(
                    producer,
                    VisionPipelineFixtureFrameService.ReferenceAngleParameter,
                    out referenceAngle)
                && TryGetParameterDouble(
                    producer,
                    VisionPipelineFixtureFrameService.ReferenceScaleParameter,
                    out referenceScale)
                && referenceScale > 0d;
            int referenceWidth = GetParameterInt(
                producer,
                VisionPipelineFixtureFrameService.ReferenceImageWidthParameter);
            int referenceHeight = GetParameterInt(
                producer,
                VisionPipelineFixtureFrameService.ReferenceImageHeightParameter);
            bool hasRoi = TryGetStepRoi(measurement, out RectangleF referenceRoi);
            string templateValue = GetTemplateValue(producer);
            bool lineFixtureProducer = VisionPipelineLineFixtureService.IsLineFixture(producer.ToolType);
            string searchRoi = GetParameterBool(producer, "USE_ROI")
                ? GetParameter(producer, "CvROI")
                : T("PipelineReview.FixtureDesigner.FullImage", "full image");

            string relationshipText = TF(
                "PipelineReview.FixtureDesigner.RelationshipFormat",
                "{0}: {1:00} {2} -> {3:00} {4} / consumers {5} / selected {6:00} {7} / ROI {8}",
                frameName,
                producerIndex + 1,
                SafeText(producer.ToolType, "Matching"),
                normalizeIndex + 1,
                "NormalizeImage",
                consumers.Count,
                measurementIndex + 1,
                SafeText(measurement.ToolType, "Tool"),
                hasRoi ? FormatRoi(referenceRoi) : "-");
            string templateText = lineFixtureProducer
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    "Datums: {0}/{1} + {2}/{3}",
                    GetParameter(producer, VisionPipelineLineFixtureService.SourceStepAParameter),
                    GetParameter(producer, VisionPipelineLineFixtureService.SourceFeatureAParameter),
                    GetParameter(producer, VisionPipelineLineFixtureService.SourceStepBParameter),
                    GetParameter(producer, VisionPipelineLineFixtureService.SourceFeatureBParameter))
                : TF(
                    "PipelineReview.FixtureDesigner.TemplateFormat",
                    "Template: {0} / search ROI: {1}",
                    string.IsNullOrWhiteSpace(templateValue) ? "-" : Path.GetFileName(templateValue),
                    string.IsNullOrWhiteSpace(searchRoi) ? "-" : searchRoi);
            string referenceText = hasReference
                ? TF(
                    "PipelineReview.FixtureDesigner.ReferenceCompactFormat",
                    "Ref ({0},{1}) / {2} deg / {3}x / {4}x{5}",
                    FormatPoseValue(referenceX),
                    FormatPoseValue(referenceY),
                    FormatPoseValue(referenceAngle),
                    FormatPoseValue(referenceScale),
                    referenceWidth,
                    referenceHeight)
                : T(
                    "PipelineReview.FixtureDesigner.ReferenceMissing",
                    "Reference pose or image size is incomplete.");
            string currentText = hasPose
                ? TF(
                    "PipelineReview.FixtureDesigner.CurrentCompactFormat",
                    "Now ({0},{1}) / {2} deg / {3}x",
                    FormatPoseValue(currentX),
                    FormatPoseValue(currentY),
                    FormatPoseValue(currentAngle),
                    FormatPoseValue(currentScale))
                : T(
                    "PipelineReview.FixtureDesigner.CurrentWaiting",
                    "Current pose: Run Review required");
            string qualityText = FormatFixtureQuality(
                steps,
                producerIndex,
                producerSummary,
                normalizeSummary,
                resolveSummary);

            Bitmap sourcePreview = null;
            Bitmap normalizedPreview = null;
            Bitmap templatePreview = null;
            try
            {
                templatePreview = lineFixtureProducer
                    ? null
                    : TryLoadTemplatePreview(templateValue);
                Bitmap source = resolveLayerPreview?.Invoke(producer.InputLayer);
                Bitmap normalized = normalizeSummary?.Success == true
                    ? resolveLayerPreview?.Invoke(normalize.OutputLayer)
                    : null;
                string sourceText = SafeText(producer.InputLayer, "-");
                string normalizedText = SafeText(normalize.OutputLayer, "-");
                if (source != null && hasPose && hasReference)
                {
                    sourcePreview = DrawRoiOverlays(
                        source,
                        measurementIndices
                            .Select(index => new FixtureRoiOverlay(
                                index,
                                TransformReferenceRoi(
                                    GetStepRoi(steps[index]),
                                    referenceX,
                                    referenceY,
                                    currentX,
                                    currentY,
                                    VisionPipelineFixtureFrameService.NormalizeAngle(currentAngle - referenceAngle),
                                    currentScale / referenceScale),
                                $"{index + 1:00} {SafeText(steps[index].Name, steps[index].ToolType)}"))
                            .ToList(),
                        measurementIndex,
                        Color.Magenta,
                        Color.DeepSkyBlue);
                    sourceText = TF(
                        "PipelineReview.FixtureDesigner.SourceLayerFormat",
                        "{0} / selected ROI {1} / {2} transformed consumers",
                        SafeText(producer.InputLayer, "-"),
                        FormatRoi(referenceRoi),
                        consumers.Count);
                }
                else if (source != null)
                {
                    sourcePreview = new Bitmap(source);
                    sourceText = TF(
                        "PipelineReview.FixtureDesigner.SourceWaitingFormat",
                        "{0} / Run Review for transformed ROI",
                        SafeText(producer.InputLayer, "-"));
                }

                if (normalized != null)
                {
                    normalizedPreview = DrawRoiOverlays(
                        normalized,
                        measurementIndices
                            .Select(index => new FixtureRoiOverlay(
                                index,
                                RectanglePoints(GetStepRoi(steps[index])),
                                $"{index + 1:00} {SafeText(steps[index].Name, steps[index].ToolType)}"))
                            .ToList(),
                        measurementIndex,
                        Color.LimeGreen,
                        Color.Gold);
                    normalizedText = TF(
                        "PipelineReview.FixtureDesigner.NormalizedLayerFormat",
                        "{0} / selected ROI {1} / {2} consumers",
                        SafeText(normalize.OutputLayer, "-"),
                        FormatRoi(referenceRoi),
                        consumers.Count);
                }

                return new OpenVisionPipelineReviewFixtureState
                {
                    IsVisible = true,
                    ProducerIndex = producerIndex,
                    NormalizeIndex = normalizeIndex,
                    MeasurementIndex = measurementIndex,
                    RelationshipText = relationshipText,
                    TemplateText = templateText,
                    ReferenceText = referenceText,
                    CurrentText = currentText,
                    QualityText = qualityText,
                    SourceText = sourceText,
                    SourcePreview = sourcePreview,
                    NormalizedText = normalizedText,
                    NormalizedPreview = normalizedPreview,
                    TemplatePreview = templatePreview,
                    CanTeachReference = hasPose && referenceWidth > 0 && referenceHeight > 0,
                    CanEditProducer = true,
                    CanEditMeasurement = true,
                    Consumers = consumers
                };
            }
            catch
            {
                sourcePreview?.Dispose();
                normalizedPreview?.Dispose();
                templatePreview?.Dispose();
                throw;
            }
        }

        public static bool TryGetReviewedFixturePose(
            VisionPipelineStep step,
            VisionPipelineStepResultSummary summary,
            out double x,
            out double y,
            out double angle,
            out double scale)
        {
            x = 0d;
            y = 0d;
            angle = 0d;
            scale = 0d;
            if (!VisionPipelineFixtureFrameService.IsProducer(step)
                || summary?.Success != true
                || summary.Metrics == null)
            {
                return false;
            }

            string toolType = VisionPipelineNormalizer.NormalizeToolType(step.ToolType);
            return (toolType == "matching"
                    || toolType == "templatematching"
                    || VisionPipelineLineFixtureService.IsLineFixture(toolType))
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureCenterX, out x)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureCenterY, out y)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureAngle, out angle)
                && summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.FixtureScale, out scale)
                && IsFinite(x)
                && IsFinite(y)
                && IsFinite(angle)
                && IsFinite(scale)
                && scale > 0d;
        }

        public static string FormatPoseValue(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static bool TryResolveFixtureChain(
            IReadOnlyList<VisionPipelineStep> steps,
            out int producerIndex,
            out int normalizeIndex,
            out IReadOnlyList<int> measurementIndices,
            out string frameName)
        {
            producerIndex = -1;
            normalizeIndex = -1;
            measurementIndices = Array.Empty<int>();
            frameName = string.Empty;
            if (steps == null)
            {
                return false;
            }

            for (int index = 0; index < steps.Count; index++)
            {
                VisionPipelineStep producer = steps[index];
                if (producer?.Enabled != true || !VisionPipelineFixtureFrameService.IsProducer(producer))
                {
                    continue;
                }

                string candidateFrame = GetParameter(
                    producer,
                    VisionPipelineFixtureFrameService.FrameNameParameter);
                int candidateNormalize = Enumerable.Range(index + 1, steps.Count - index - 1)
                    .FirstOrDefault(candidate =>
                        steps[candidate]?.Enabled == true
                        && VisionPipelineFixtureFrameService.IsNormalizeImageConsumer(steps[candidate])
                        && string.Equals(
                            GetParameter(
                                steps[candidate],
                                VisionPipelineFixtureFrameService.FrameNameParameter),
                            candidateFrame,
                            StringComparison.OrdinalIgnoreCase));
                if (candidateNormalize <= index)
                {
                    continue;
                }

                HashSet<string> reachableLayers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    SafeText(steps[candidateNormalize].OutputLayer, string.Empty)
                };
                List<int> candidateMeasurements = new List<int>();
                for (int candidate = candidateNormalize + 1; candidate < steps.Count; candidate++)
                {
                    VisionPipelineStep downstream = steps[candidate];
                    if (downstream?.Enabled != true
                        || !reachableLayers.Contains(SafeText(downstream.InputLayer, string.Empty)))
                    {
                        continue;
                    }

                    if (GetParameterBool(downstream, "USE_ROI") && TryGetStepRoi(downstream, out _))
                    {
                        candidateMeasurements.Add(candidate);
                    }

                    if (!string.IsNullOrWhiteSpace(downstream.OutputLayer))
                    {
                        reachableLayers.Add(downstream.OutputLayer.Trim());
                    }
                }

                if (candidateMeasurements.Count > 0)
                {
                    producerIndex = index;
                    normalizeIndex = candidateNormalize;
                    measurementIndices = candidateMeasurements;
                    frameName = string.IsNullOrWhiteSpace(candidateFrame) ? "Fixture" : candidateFrame;
                    return true;
                }
            }

            return false;
        }

        private static string FormatFixtureQuality(
            IReadOnlyList<VisionPipelineStep> steps,
            int producerIndex,
            VisionPipelineStepResultSummary producerSummary,
            VisionPipelineStepResultSummary normalizeSummary,
            Func<VisionPipelineStep, VisionPipelineStepResultSummary> resolveSummary)
        {
            VisionPipelineStep producer = steps?.ElementAtOrDefault(producerIndex);
            if (VisionPipelineLineFixtureService.IsLineFixture(producer?.ToolType))
            {
                string supportA = TryGetMetric(
                    producerSummary,
                    VisionPipelineKnownMetrics.FixtureLineASupportCount,
                    out double supportAValue)
                        ? FormatPoseValue(supportAValue)
                        : "-";
                string supportB = TryGetMetric(
                    producerSummary,
                    VisionPipelineKnownMetrics.FixtureLineBSupportCount,
                    out double supportBValue)
                        ? FormatPoseValue(supportBValue)
                        : "-";
                string residualA = TryGetMetric(
                    producerSummary,
                    VisionPipelineKnownMetrics.FixtureLineAFitResidualPx,
                    out double residualAValue)
                        ? FormatPoseValue(residualAValue)
                        : "-";
                string residualB = TryGetMetric(
                    producerSummary,
                    VisionPipelineKnownMetrics.FixtureLineBFitResidualPx,
                    out double residualBValue)
                        ? FormatPoseValue(residualBValue)
                        : "-";
                string included = TryGetMetric(
                    producerSummary,
                    VisionPipelineKnownMetrics.FixtureIncludedAngleDeg,
                    out double includedValue)
                        ? FormatPoseValue(includedValue)
                        : "-";
                string validRatio = TryGetMetric(
                    normalizeSummary,
                    VisionPipelineKnownMetrics.FixtureValidPixelRatio,
                    out double validRatioValue)
                        ? validRatioValue.ToString("P1", CultureInfo.CurrentCulture)
                        : "-";
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "Support A/B {0}/{1} / residual {2}/{3}px / angle {4}° / valid {5}",
                    supportA,
                    supportB,
                    residualA,
                    residualB,
                    included,
                    validRatio);
            }

            VisionPipelineStepResultSummary scoreSummary = producerSummary;
            if (!TryGetMetric(scoreSummary, VisionPipelineKnownMetrics.ScoreMargin, out _))
            {
                string producerTemplate = GetTemplateValue(producer);
                for (int index = producerIndex - 1; index >= 0; index--)
                {
                    VisionPipelineStep candidate = steps[index];
                    string toolType = VisionPipelineNormalizer.NormalizeToolType(candidate?.ToolType);
                    if ((toolType != "matching" && toolType != "templatematching")
                        || !string.Equals(
                            candidate.InputLayer,
                            producer?.InputLayer,
                            StringComparison.OrdinalIgnoreCase)
                        || !string.Equals(
                            GetTemplateValue(candidate),
                            producerTemplate,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    VisionPipelineStepResultSummary candidateSummary = resolveSummary?.Invoke(candidate);
                    if (TryGetMetric(candidateSummary, VisionPipelineKnownMetrics.ScoreMargin, out _))
                    {
                        scoreSummary = candidateSummary;
                        break;
                    }
                }
            }

            string score = TryGetMetric(
                producerSummary,
                VisionPipelineKnownMetrics.ScoreMax,
                out double scoreValue)
                ? FormatPoseValue(scoreValue)
                : "-";
            string margin = TryGetMetric(
                scoreSummary,
                VisionPipelineKnownMetrics.ScoreMargin,
                out double marginValue)
                ? FormatPoseValue(marginValue)
                : "-";
            string valid = TryGetMetric(
                normalizeSummary,
                VisionPipelineKnownMetrics.FixtureValidPixelRatio,
                out double validValue)
                ? validValue.ToString("P1", CultureInfo.CurrentCulture)
                : "-";
            return TF(
                "PipelineReview.FixtureDesigner.QualityCompactFormat",
                "Score {0} / margin {1} / valid {2}",
                score,
                margin,
                valid);
        }

        private static bool TryGetMetric(
            VisionPipelineStepResultSummary summary,
            string name,
            out double value)
        {
            value = 0d;
            return summary?.Metrics != null
                && summary.Metrics.TryGetValue(name, out value)
                && IsFinite(value);
        }

        private static Bitmap TryLoadTemplatePreview(string templateValue)
        {
            if (string.IsNullOrWhiteSpace(templateValue))
            {
                return null;
            }

            try
            {
                string path = VisionPipelineAppToolFactory.ResolveTemplatePath(templateValue);
                return File.Exists(path) ? new Bitmap(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap DrawRoiOverlays(
            Bitmap source,
            IReadOnlyList<FixtureRoiOverlay> overlays,
            int selectedIndex,
            Color selectedColor,
            Color otherColor)
        {
            if (source == null || overlays == null || overlays.Count == 0)
            {
                return null;
            }

            Bitmap result = new Bitmap(source);
            using Graphics graphics = Graphics.FromImage(result);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using Font font = new Font("Segoe UI", 11f, FontStyle.Bold);
            using SolidBrush background = new SolidBrush(Color.FromArgb(210, 16, 32, 39));
            using SolidBrush foreground = new SolidBrush(Color.White);
            foreach (FixtureRoiOverlay overlay in overlays.OrderBy(item => item.StepIndex == selectedIndex ? 1 : 0))
            {
                if (overlay.Points == null || overlay.Points.Length < 4)
                {
                    continue;
                }

                bool selected = overlay.StepIndex == selectedIndex;
                using Pen shadow = new Pen(Color.Black, selected ? 7f : 5f);
                using Pen pen = new Pen(selected ? selectedColor : otherColor, selected ? 4f : 2f);
                graphics.DrawPolygon(shadow, overlay.Points);
                graphics.DrawPolygon(pen, overlay.Points);
                SizeF size = graphics.MeasureString(overlay.Label, font);
                float labelX = Math.Max(
                    0f,
                    Math.Min(overlay.Points.Min(point => point.X), result.Width - size.Width - 8f));
                float labelY = Math.Max(
                    0f,
                    Math.Min(
                        overlay.Points.Min(point => point.Y) - size.Height - 4f,
                        result.Height - size.Height - 4f));
                graphics.FillRectangle(background, labelX, labelY, size.Width + 6f, size.Height + 2f);
                graphics.DrawString(overlay.Label, font, foreground, labelX + 3f, labelY + 1f);
            }

            return result;
        }

        private static OpenVisionPipelineReviewFixtureConsumerRow CreateConsumerRow(
            IReadOnlyList<VisionPipelineStep> steps,
            int producerIndex,
            int normalizeIndex,
            int measurementIndex,
            string frameName,
            VisionPipelineStepResultSummary summary)
        {
            VisionPipelineStep step = steps[measurementIndex];
            RectangleF roi = GetStepRoi(step);
            string roiText = FormatRoi(roi);
            string evidenceId = CreateConsumerEvidenceId(
                steps[producerIndex],
                producerIndex,
                steps[normalizeIndex],
                normalizeIndex,
                step,
                measurementIndex,
                frameName,
                roiText);
            return new OpenVisionPipelineReviewFixtureConsumerRow
            {
                StepIndex = measurementIndex,
                StepNumber = measurementIndex + 1,
                StepName = SafeText(step.Name, step.ToolType),
                ToolType = SafeText(step.ToolType, "Tool"),
                RoiText = roiText,
                RouteText = $"{SafeText(step.InputLayer, "-")} -> {SafeText(step.OutputLayer, "-")}",
                StatusText = string.IsNullOrWhiteSpace(summary?.Status) ? "WAIT" : summary.Status,
                EvidenceId = evidenceId,
                EvidenceShortId = evidenceId.Substring(0, 12)
            };
        }

        private static string CreateConsumerEvidenceId(
            VisionPipelineStep producer,
            int producerIndex,
            VisionPipelineStep normalize,
            int normalizeIndex,
            VisionPipelineStep consumer,
            int consumerIndex,
            string frameName,
            string roiText)
        {
            string canonical = string.Join(
                "|",
                "FixtureConsumer",
                frameName ?? string.Empty,
                producerIndex.ToString(CultureInfo.InvariantCulture),
                producer?.Name ?? string.Empty,
                producer?.InputLayer ?? string.Empty,
                normalizeIndex.ToString(CultureInfo.InvariantCulture),
                normalize?.Name ?? string.Empty,
                normalize?.OutputLayer ?? string.Empty,
                consumerIndex.ToString(CultureInfo.InvariantCulture),
                consumer?.Name ?? string.Empty,
                consumer?.ToolType ?? string.Empty,
                consumer?.InputLayer ?? string.Empty,
                consumer?.OutputLayer ?? string.Empty,
                roiText ?? string.Empty);
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        }

        private static RectangleF GetStepRoi(VisionPipelineStep step)
        {
            return TryGetStepRoi(step, out RectangleF roi) ? roi : default;
        }

        private sealed class FixtureRoiOverlay
        {
            public FixtureRoiOverlay(int stepIndex, PointF[] points, string label)
            {
                StepIndex = stepIndex;
                Points = points;
                Label = label ?? string.Empty;
            }

            public int StepIndex { get; }
            public PointF[] Points { get; }
            public string Label { get; }
        }

        private static PointF[] RectanglePoints(RectangleF roi)
        {
            return new[]
            {
                new PointF(roi.Left, roi.Top),
                new PointF(roi.Right, roi.Top),
                new PointF(roi.Right, roi.Bottom),
                new PointF(roi.Left, roi.Bottom)
            };
        }

        private static PointF[] TransformReferenceRoi(
            RectangleF roi,
            double referenceX,
            double referenceY,
            double currentX,
            double currentY,
            double angleDelta,
            double scaleRatio)
        {
            double radians = angleDelta * Math.PI / 180d;
            double cosine = Math.Cos(radians);
            double sine = Math.Sin(radians);
            return RectanglePoints(roi)
                .Select(point =>
                {
                    double x = point.X - referenceX;
                    double y = point.Y - referenceY;
                    return new PointF(
                        (float)(currentX + scaleRatio * ((cosine * x) + (sine * y))),
                        (float)(currentY + scaleRatio * ((-sine * x) + (cosine * y))));
                })
                .ToArray();
        }

        private static bool TryGetStepRoi(VisionPipelineStep step, out RectangleF roi)
        {
            roi = default;
            string[] parts = GetParameter(step, "CvROI").Split(',');
            if (parts.Length != 4
                || !float.TryParse(
                    parts[0],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float x)
                || !float.TryParse(
                    parts[1],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float y)
                || !float.TryParse(
                    parts[2],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float width)
                || !float.TryParse(
                    parts[3],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float height)
                || width <= 0f
                || height <= 0f)
            {
                return false;
            }

            roi = new RectangleF(x, y, width, height);
            return true;
        }

        private static string FormatRoi(RectangleF roi)
        {
            return string.Join(",", new[]
            {
                roi.X.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Y.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Width.ToString("0.###", CultureInfo.InvariantCulture),
                roi.Height.ToString("0.###", CultureInfo.InvariantCulture)
            });
        }

        private static string GetParameter(VisionPipelineStep step, string key)
        {
            if (step?.Parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return step.Parameters
                .FirstOrDefault(parameter =>
                    string.Equals(parameter.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value
                ?.Trim()
                ?? string.Empty;
        }

        private static string GetTemplateValue(VisionPipelineStep step)
        {
            string value = GetParameter(step, "TemplatePath");
            return string.IsNullOrWhiteSpace(value)
                ? GetParameter(step, "PATTERN_PATH")
                : value;
        }

        private static bool GetParameterBool(VisionPipelineStep step, string key)
        {
            return bool.TryParse(GetParameter(step, key), out bool value) && value;
        }

        private static bool TryGetParameterDouble(
            VisionPipelineStep step,
            string key,
            out double value)
        {
            return double.TryParse(
                    GetParameter(step, key),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value)
                && IsFinite(value);
        }

        private static int GetParameterInt(VisionPipelineStep step, string key)
        {
            return int.TryParse(
                GetParameter(step, key),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int value)
                ? value
                : 0;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                T(key, fallbackFormat),
                args ?? Array.Empty<object>());
        }
    }
}
