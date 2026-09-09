using OpenVisionLab.Common;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    // Manual v1 bridge from an explicit Matching run to the hash-verified packet.
    // It accepts retained runtime candidates only; it never invents coordinates,
    // tolerances, or a replacement run.
    internal static class OpenVisionRecipeLocatorRelativeBlobEvidenceExporter
    {
        internal static bool TryExport(
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            VisionPipeline pipeline,
            VisionPipelineRunResult runResult,
            Mat sourceImage,
            string sourceImagePath,
            string outputDirectory,
            string producerVersion,
            out OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
            out string packetPath,
            out string overlayPath,
            out string message)
        {
            packet = null;
            packetPath = string.Empty;
            overlayPath = string.Empty;
            message = string.Empty;

            try
            {
                if (plan == null || pipeline == null || runResult == null || sourceImage == null || sourceImage.Empty())
                {
                    message = "A reviewed plan, pipeline, runtime result, and source image are required.";
                    return false;
                }

                if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out message))
                {
                    return false;
                }

                string sourcePath = Path.GetFullPath(sourceImagePath ?? string.Empty);
                if (!File.Exists(sourcePath))
                {
                    message = "The runtime source image is missing.";
                    return false;
                }

                if (sourceImage.Width != plan.ReferencePose.ImageWidth
                    || sourceImage.Height != plan.ReferencePose.ImageHeight)
                {
                    message = "The runtime source dimensions do not match the reviewed reference pose.";
                    return false;
                }

                int locatorIndex = pipeline.Steps.FindIndex(step =>
                    step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(step.Parameters?.GetValueOrDefault("NUM_MATCH"), "2", StringComparison.Ordinal));
                if (locatorIndex < 0 || locatorIndex >= runResult.StepResults.Count)
                {
                    message = "The ambiguity-gated Matching step was not retained in the runtime result.";
                    return false;
                }

                VisionPipelineStepResult locatorResult = runResult.StepResults[locatorIndex];
                if (locatorResult?.ToolResult?.Success != true || locatorResult.AcceptancePassed != true)
                {
                    message = "The runtime locator did not pass its explicit success and ambiguity gates.";
                    return false;
                }

                IReadOnlyList<VisionPipelineMatchResultEvidence> matches =
                    VisionPipelineMatchResultStore.Get(locatorResult.ToolResult);
                if (matches == null || matches.Count == 0)
                {
                    message = "The runtime locator produced no retained candidate evidence.";
                    return false;
                }

                if (matches.Count < 2)
                {
                    message = "The runtime locator did not retain a second candidate; ScoreMargin cannot be used as observed competition evidence.";
                    return false;
                }

                if (!locatorResult.ToolResult.Metrics.TryGetValue(
                        VisionPipelineKnownMetrics.ScoreMargin,
                        out double scoreMargin)
                    || double.IsNaN(scoreMargin)
                    || double.IsInfinity(scoreMargin)
                    || scoreMargin < 0D)
                {
                    message = "The runtime locator did not retain a finite score-margin metric.";
                    return false;
                }

                List<VisionPipelineMatchResultEvidence> orderedMatches = matches
                    .OrderByDescending(item => item.Score)
                    .ThenBy(item => item.NativeIndex)
                    .ToList();
                VisionPipelineMatchResultEvidence selected = orderedMatches[0];

                Directory.CreateDirectory(Path.GetFullPath(outputDirectory));
                string currentOverlayPath = Path.Combine(
                    Path.GetFullPath(outputDirectory),
                    "locator-runtime-overlay.png");
                if (!SaveLocatorOverlay(sourceImage, locatorResult, locatorIndex + 1, currentOverlayPath, out message))
                {
                    return false;
                }
                overlayPath = currentOverlayPath;

                string templatePath = Path.GetFullPath(plan.LocatorTemplatePath);
                if (!File.Exists(templatePath))
                {
                    message = "The reviewed locator template is missing.";
                    return false;
                }

                string version = string.IsNullOrWhiteSpace(producerVersion)
                    ? "unknown"
                    : producerVersion.Trim();
                List<OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate> candidates = orderedMatches
                    .Select((item, index) => CreateCandidate(item, index == 0, scoreMargin, currentOverlayPath))
                    .ToList();
                packet = new OpenVisionRecipeLocatorRelativeBlobEvidencePacket
                {
                    SourceImagePath = sourcePath,
                    SourceImageSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(sourcePath),
                    LocatorTemplatePath = templatePath,
                    LocatorTemplateSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(templatePath),
                    PreviewOverlayPath = overlayPath,
                    PreviewOverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath),
                    SourceImageWidth = sourceImage.Width,
                    SourceImageHeight = sourceImage.Height,
                    CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
                    Producer = nameof(OpenVisionRecipeLocatorRelativeBlobEvidenceExporter),
                    ProducerVersion = version,
                    SelectedCandidateId = "locator-" + selected.NativeIndex.ToString(CultureInfo.InvariantCulture),
                    CreatedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                    Candidates = candidates
                };

                packetPath = Path.Combine(Path.GetFullPath(outputDirectory), "evidence.packet.json");
                if (!packet.TrySave(packetPath, out message))
                {
                    packet = null;
                    packetPath = string.Empty;
                    return false;
                }

                message = "Runtime locator candidates and the current overlay were exported to a hash-verified evidence packet.";
                return true;
            }
            catch (Exception exception)
            {
                packet = null;
                packetPath = string.Empty;
                overlayPath = string.Empty;
                message = exception.GetBaseException().Message;
                return false;
            }
        }

        private static OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate CreateCandidate(
            VisionPipelineMatchResultEvidence match,
            bool accepted,
            double scoreMargin,
            string overlayPath)
        {
            double normalizedScore = match.Score > 1D ? match.Score / 100D : match.Score;
            return new OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
            {
                CandidateId = "locator-" + match.NativeIndex.ToString(CultureInfo.InvariantCulture),
                NativeIndex = match.NativeIndex,
                Accepted = accepted,
                CenterX = match.CenterX,
                CenterY = match.CenterY,
                Angle = match.Angle,
                Scale = match.Scale,
                BoundsX = ToBound(match.BoundsX),
                BoundsY = ToBound(match.BoundsY),
                BoundsWidth = ToPositiveBound(match.BoundsWidth),
                BoundsHeight = ToPositiveBound(match.BoundsHeight),
                Score = Math.Max(0D, Math.Min(1D, normalizedScore)),
                ScoreMargin = scoreMargin,
                CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
                OverlayPath = overlayPath,
                OverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath),
                Reason = accepted
                    ? "accepted by the explicit locator success and ambiguity gates"
                    : "lower-ranked candidate retained for review"
            };
        }

        private static int ToBound(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static int ToPositiveBound(double value)
        {
            return Math.Max(1, ToBound(value));
        }

        private static bool SaveLocatorOverlay(
            Mat sourceImage,
            VisionPipelineStepResult locatorResult,
            int stepIndex,
            string outputPath,
            out string message)
        {
            message = string.Empty;
            try
            {
                using Bitmap source = BitmapImageConverter.ToBitmap(sourceImage);
                using Bitmap rendered = new Bitmap(source);
                VisionPipelineRunReportImageRenderer.RenderInPlace(
                    rendered,
                    CreatePersistedOverlaySummary(locatorResult, stepIndex),
                    locatorResult?.Step);

                using FileStream stream = new FileStream(
                    outputPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None);
                rendered.Save(stream, ImageFormat.Png);
                if (!File.Exists(outputPath) || new FileInfo(outputPath).Length == 0)
                {
                    message = "The locator overlay file was not written.";
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                message = "The locator overlay could not be rendered: " + exception.GetBaseException().Message;
                return false;
            }
        }

        private static VisionRecipeStepRunSummary CreatePersistedOverlaySummary(
            VisionPipelineStepResult locatorResult,
            int stepIndex)
        {
            return new VisionRecipeStepRunSummary
            {
                Index = stepIndex,
                Name = locatorResult?.Step?.Name ?? string.Empty,
                ToolType = locatorResult?.Step?.ToolType ?? string.Empty,
                Status = locatorResult?.ToolResult?.Success == true
                    && locatorResult.AcceptancePassed
                        ? "OK"
                        : "NG",
                ToolSuccess = locatorResult?.ToolResult?.Success == true,
                Success = locatorResult?.ToolResult?.Success == true
                    && locatorResult?.AcceptancePassed == true,
                AcceptancePassed = locatorResult?.AcceptancePassed == true,
                AcceptanceMessage = locatorResult?.AcceptanceMessage ?? string.Empty,
                Message = locatorResult?.ToolResult?.Message ?? string.Empty,
                Overlays = (locatorResult?.ToolResult?.Overlays ?? new List<VisionToolOverlay>())
                    .Where(overlay => overlay != null)
                    .Select(overlay => new VisionRecipeOverlaySummary
                    {
                        Kind = overlay.Kind.ToString(),
                        Label = overlay.Label ?? string.Empty,
                        BoundsX = overlay.Bounds.X,
                        BoundsY = overlay.Bounds.Y,
                        BoundsWidth = overlay.Bounds.Width,
                        BoundsHeight = overlay.Bounds.Height,
                        CenterX = overlay.Center.X,
                        CenterY = overlay.Center.Y,
                        StartX = overlay.Start.X,
                        StartY = overlay.Start.Y,
                        EndX = overlay.End.X,
                        EndY = overlay.End.Y,
                        Angle = overlay.Angle,
                        PointCount = overlay.Points?.Count ?? 0,
                        Points = (overlay.Points ?? new List<PointF>())
                            .Select(point => new VisionRecipeOverlayPointSummary
                            {
                                X = point.X,
                                Y = point.Y
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
