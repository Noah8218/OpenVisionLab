using Lib.Common;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace OpenVisionLab
{
    public sealed class VisionPipelineRunReport
    {
        public string RecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string StartedAt { get; set; } = string.Empty;
        public string FinishedAt { get; set; } = string.Empty;
        public double TotalMilliseconds { get; set; }
        public bool Success { get; set; }
        public bool PublishAllOutputs { get; set; }
        public string PipelineSnapshotFile { get; set; } = string.Empty;
        public string SourceImageFile { get; set; } = string.Empty;
        public string SourceImageSha256 { get; set; } = string.Empty;
        public List<VisionPipelineStepRunReport> Steps { get; set; } = new List<VisionPipelineStepRunReport>();
    }

    public sealed class VisionPipelineStepRunReport
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public bool Skipped { get; set; }
        public string InputLayer { get; set; } = string.Empty;
        public string OutputLayer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool ToolSuccess { get; set; }
        public bool AcceptancePassed { get; set; }
        public string AcceptanceMessage { get; set; } = string.Empty;
        public double ElapsedMilliseconds { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public string ErrorName { get; set; } = string.Empty;
        public string ResultStatus { get; set; } = string.Empty;
        public string DiagnosticHint { get; set; } = string.Empty;
        public string SuggestedFix { get; set; } = string.Empty;
        public string ResultImageFile { get; set; } = string.Empty;
        public string OverlayImageFile { get; set; } = string.Empty;
        public int ResultImageWidth { get; set; }
        public int ResultImageHeight { get; set; }
        public string ResultImageSize { get; set; } = string.Empty;
        public int OverlayCount { get; set; }
        public int MetricCount { get; set; }
        public int ParameterCount { get; set; }
        public List<VisionPipelineMetricRunReport> Metrics { get; set; } = new List<VisionPipelineMetricRunReport>();
        public List<VisionPipelineParameter> Parameters { get; set; } = new List<VisionPipelineParameter>();
        public List<VisionPipelineObjectRunReport> Objects { get; set; } = new List<VisionPipelineObjectRunReport>();
        public List<VisionPipelineInstanceRunReport> Instances { get; set; } = new List<VisionPipelineInstanceRunReport>();
        public List<VisionPipelineGeometryFeatureResult> GeometryFeatures { get; set; } = new List<VisionPipelineGeometryFeatureResult>();
    }

    public sealed class VisionPipelineObjectRunReport
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
    }

    public sealed class VisionPipelineMetricRunReport
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public sealed class VisionPipelineInstanceRunReport
    {
        public int Number { get; set; }
        public string InstanceId { get; set; } = string.Empty;
        public string SourceStep { get; set; } = string.Empty;
        public bool Accepted { get; set; }
        public double Score { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; }
        public double RoiCenterX { get; set; }
        public double RoiCenterY { get; set; }
        public double RoiWidth { get; set; }
        public double RoiHeight { get; set; }
        public double RoiAngle { get; set; }
        public double MeanValue { get; set; }
        public double ValidPixelRatio { get; set; }
        public string RejectReason { get; set; } = string.Empty;
    }

    internal static class VisionPipelineRunReportStorage
    {
        public sealed class RunReportInfo
        {
            public string Name { get; set; } = string.Empty;
            public string DirectoryPath { get; set; } = string.Empty;
            public string ReportPath { get; set; } = string.Empty;
            public DateTime StartedAt { get; set; }
            public bool Success { get; set; }

            public override string ToString()
            {
                string status = Success ? "OK" : "NG";
                return $"{StartedAt:yyyy-MM-dd HH:mm:ss.fff} [{status}]";
            }
        }

        public static string Save(
            string recipeName,
            VisionPipeline pipeline,
            VisionPipelineRunResult result,
            DateTime startedAt,
            DateTime finishedAt,
            bool publishAllOutputs,
            string runLabel = null)
        {
            string pipelineName = string.IsNullOrWhiteSpace(pipeline?.Name) ? "Pipeline" : pipeline.Name;
            string runName = CreateUniqueRunName(recipeName, pipelineName, startedAt, runLabel);
            string directory = RecipeWorkspaceService.GetVisionPipelineRunDirectory(recipeName, pipelineName, runName);

            string pipelineSnapshotFile = "pipeline.xml";
            SerializeHelper.SaveXmlFile(Path.Combine(directory, pipelineSnapshotFile), pipeline ?? new VisionPipeline { Name = pipelineName });

            VisionPipelineRunReport report = new VisionPipelineRunReport
            {
                RecipeName = recipeName ?? string.Empty,
                PipelineName = pipelineName,
                StartedAt = startedAt.ToString("o"),
                FinishedAt = finishedAt.ToString("o"),
                TotalMilliseconds = (finishedAt - startedAt).TotalMilliseconds,
                Success = result?.Success == true,
                PublishAllOutputs = publishAllOutputs,
                PipelineSnapshotFile = pipelineSnapshotFile
            };

            List<VisionPipelineStepResult> stepResults = result?.StepResults ?? new List<VisionPipelineStepResult>();
            for (int i = 0; i < stepResults.Count; i++)
            {
                VisionPipelineStepResult stepResult = stepResults[i];
                report.Steps.Add(CreateStepReport(directory, i + 1, stepResult));
            }

            string reportPath = Path.Combine(directory, "report.xml");
            SerializeHelper.SaveXmlFile(reportPath, report);
            return reportPath;
        }

        public static string Save(
            string recipeName,
            VisionPipeline pipeline,
            VisionRecipeRunResult result,
            DateTime startedAt,
            DateTime finishedAt,
            string runLabel = null,
            Mat sourceImage = null)
        {
            string pipelineName = string.IsNullOrWhiteSpace(pipeline?.Name) ? "Pipeline" : pipeline.Name;
            string runName = CreateUniqueRunName(recipeName, pipelineName, startedAt, runLabel);
            string directory = RecipeWorkspaceService.GetVisionPipelineRunDirectory(recipeName, pipelineName, runName);

            const string pipelineSnapshotFile = "pipeline.xml";
            SerializeHelper.SaveXmlFile(Path.Combine(directory, pipelineSnapshotFile), pipeline ?? new VisionPipeline { Name = pipelineName });
            string sourceImageFile = SaveSourceImage(directory, sourceImage);
            string sourceImagePath = string.IsNullOrWhiteSpace(sourceImageFile)
                ? string.Empty
                : Path.Combine(directory, sourceImageFile);

            List<VisionRecipeStepRunSummary> summaries = (result?.Steps ?? new List<VisionRecipeStepRunSummary>())
                .Where(step => step != null)
                .ToList();
            VisionRecipeStepRunSummary reviewEvidenceStep = ResolveReviewEvidenceStep(result, summaries);
            bool saveEveryPinArrayGapDrawing = summaries.Count(IsExecutedPinArrayGapStep) > 1;
            VisionPipelineRunReport report = new VisionPipelineRunReport
            {
                RecipeName = recipeName ?? string.Empty,
                PipelineName = pipelineName,
                StartedAt = startedAt.ToString("o"),
                FinishedAt = finishedAt.ToString("o"),
                TotalMilliseconds = result?.TotalMilliseconds ?? 0D,
                Success = result?.Success == true,
                PublishAllOutputs = false,
                PipelineSnapshotFile = pipelineSnapshotFile,
                SourceImageFile = sourceImageFile,
                SourceImageSha256 = ComputeFileSha256(sourceImagePath),
                Steps = summaries
                    .Select(summary => CreateStepReport(
                        directory,
                        summary,
                        ResolvePipelineStep(pipeline, summary),
                        ReferenceEquals(summary, reviewEvidenceStep),
                        ReferenceEquals(summary, reviewEvidenceStep)
                            || (saveEveryPinArrayGapDrawing && IsExecutedPinArrayGapStep(summary)),
                        result?.ResultImage,
                        sourceImage))
                    .ToList()
            };

            string reportPath = Path.Combine(directory, "report.xml");
            SerializeHelper.SaveXmlFile(reportPath, report);
            return reportPath;
        }

        private static string CreateUniqueRunName(string recipeName, string pipelineName, DateTime startedAt, string runLabel)
        {
            string baseName = startedAt.ToString("yyyyMMdd_HHmmssfff");
            if (!string.IsNullOrWhiteSpace(runLabel))
            {
                baseName = $"{baseName}_{SanitizeFileName(runLabel)}";
            }

            string rootDirectory = RecipeWorkspaceService.GetVisionPipelineRunRootDirectory(recipeName, pipelineName);
            string candidate = baseName;
            int suffix = 2;
            while (Directory.Exists(Path.Combine(rootDirectory, candidate)))
            {
                candidate = $"{baseName}_{suffix++}";
            }

            return candidate;
        }

        public static List<RunReportInfo> List(string recipeName, string pipelineName)
        {
            string rootDirectory = RecipeWorkspaceService.GetVisionPipelineRunRootDirectory(recipeName, pipelineName);
            if (!Directory.Exists(rootDirectory))
            {
                return new List<RunReportInfo>();
            }

            List<RunReportInfo> reports = new List<RunReportInfo>();
            foreach (string directory in Directory.EnumerateDirectories(rootDirectory))
            {
                string reportPath = Path.Combine(directory, "report.xml");
                if (!File.Exists(reportPath))
                {
                    continue;
                }

                VisionPipelineRunReport report = Load(reportPath);
                if (report == null)
                {
                    continue;
                }

                DateTime.TryParse(report.StartedAt, out DateTime startedAt);
                reports.Add(new RunReportInfo
                {
                    Name = Path.GetFileName(directory),
                    DirectoryPath = directory,
                    ReportPath = reportPath,
                    StartedAt = startedAt == default ? File.GetCreationTime(directory) : startedAt,
                    Success = report.Success
                });
            }

            return reports
                .OrderByDescending(report => report.StartedAt)
                .ToList();
        }

        public static VisionPipelineRunReport Load(string reportPath)
        {
            return SerializeHelper.TryLoadFromXmlFile(reportPath, out VisionPipelineRunReport report)
                ? report
                : null;
        }

        public static bool IsFileSha256Match(string path, string expectedSha256)
        {
            return !string.IsNullOrWhiteSpace(expectedSha256)
                && string.Equals(
                    ComputeFileSha256(path),
                    expectedSha256.Trim(),
                    StringComparison.OrdinalIgnoreCase);
        }

        private static VisionPipelineStepRunReport CreateStepReport(string directory, int index, VisionPipelineStepResult stepResult)
        {
            VisionPipelineStep step = stepResult?.Step;
            VisionToolResult toolResult = stepResult?.ToolResult;
            VisionPipelineStepResultSummary summary = VisionPipelineResultSummaryService.CreateStepSummary(index, stepResult);
            string imageFile = SaveResultImage(directory, index, step, toolResult?.ResultImage);
            string overlayImageFile = SaveOverlayImage(directory, index, stepResult);

            return new VisionPipelineStepRunReport
            {
                Index = index,
                Name = step?.Name ?? string.Empty,
                ToolType = step?.ToolType ?? string.Empty,
                Enabled = step?.Enabled == true,
                Skipped = stepResult?.Skipped == true,
                InputLayer = step?.InputLayer ?? string.Empty,
                OutputLayer = step?.OutputLayer ?? string.Empty,
                Status = summary.Status,
                ToolSuccess = toolResult?.Success == true,
                AcceptancePassed = stepResult?.AcceptancePassed == true,
                AcceptanceMessage = stepResult?.AcceptanceMessage ?? string.Empty,
                ElapsedMilliseconds = summary.ElapsedMilliseconds,
                Message = summary.Message,
                ErrorCode = summary.ErrorCode,
                ErrorName = summary.ErrorName,
                ResultStatus = summary.ResultStatus,
                DiagnosticHint = summary.DiagnosticHint,
                SuggestedFix = summary.SuggestedFix,
                ResultImageFile = imageFile,
                OverlayImageFile = overlayImageFile,
                ResultImageWidth = summary.ResultImageWidth,
                ResultImageHeight = summary.ResultImageHeight,
                ResultImageSize = summary.ResultImageSizeText,
                OverlayCount = summary.OverlayCount,
                MetricCount = summary.MetricCount,
                ParameterCount = summary.ParameterCount,
                Metrics = (toolResult?.Metrics ?? new Dictionary<string, double>())
                    .OrderBy(metric => metric.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(metric => new VisionPipelineMetricRunReport
                    {
                        Name = metric.Key,
                        Value = metric.Value
                    })
                    .ToList(),
                Parameters = (step?.Parameters ?? new Dictionary<string, string>())
                    .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(parameter => new VisionPipelineParameter(parameter.Key, parameter.Value))
                    .ToList(),
                Objects = CreateObjectReports(summary.ObjectResults),
                Instances = CreateInstanceReports(summary.InstanceResults),
                GeometryFeatures = CreateGeometryReports(summary.GeometryFeatures)
            };
        }

        private static VisionPipelineStepRunReport CreateStepReport(
            string directory,
            VisionRecipeStepRunSummary summary,
            VisionPipelineStep pipelineStep,
            bool saveResultImage,
            bool saveOverlayImage,
            Mat resultImage,
            Mat sourceImage)
        {
            string imageFile = saveResultImage
                ? SaveResultImage(directory, summary.Index, pipelineStep, resultImage)
                : string.Empty;
            string overlayImageFile = saveOverlayImage
                ? SaveRecipeOverlayImage(directory, summary, pipelineStep, sourceImage)
                : string.Empty;
            return new VisionPipelineStepRunReport
            {
                Index = summary.Index,
                Name = summary.Name,
                ToolType = summary.ToolType,
                Enabled = summary.Enabled,
                Skipped = summary.Skipped,
                InputLayer = summary.InputLayer,
                OutputLayer = summary.OutputLayer,
                Status = summary.Status,
                ToolSuccess = summary.ToolSuccess,
                AcceptancePassed = summary.AcceptancePassed,
                AcceptanceMessage = summary.AcceptanceMessage,
                ElapsedMilliseconds = summary.ElapsedMilliseconds,
                Message = summary.Message,
                ErrorCode = summary.ErrorCode,
                ErrorName = summary.ErrorName,
                ResultStatus = summary.ResultStatus,
                DiagnosticHint = summary.DiagnosticHint,
                SuggestedFix = summary.SuggestedFix,
                ResultImageFile = imageFile,
                OverlayImageFile = overlayImageFile,
                ResultImageWidth = summary.ResultImageWidth,
                ResultImageHeight = summary.ResultImageHeight,
                ResultImageSize = summary.ResultImageSizeText,
                OverlayCount = summary.OverlayCount,
                MetricCount = summary.MetricCount,
                ParameterCount = summary.ParameterCount,
                Metrics = (summary.Metrics ?? new Dictionary<string, double>())
                    .OrderBy(metric => metric.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(metric => new VisionPipelineMetricRunReport
                    {
                        Name = metric.Key,
                        Value = metric.Value
                    })
                    .ToList(),
                Parameters = (summary.Parameters ?? new Dictionary<string, string>())
                    .OrderBy(parameter => parameter.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(parameter => new VisionPipelineParameter(parameter.Key, parameter.Value))
                    .ToList(),
                Objects = CreateObjectReports(summary.ObjectResults),
                Instances = CreateInstanceReports(summary.InstanceResults),
                GeometryFeatures = CreateGeometryReports(summary.GeometryFeatures)
            };
        }

        private static List<VisionPipelineObjectRunReport> CreateObjectReports(
            IEnumerable<VisionPipelineObjectResult> objects)
        {
            return (objects ?? Enumerable.Empty<VisionPipelineObjectResult>())
                .Select(item => new VisionPipelineObjectRunReport
                {
                    Number = item.Number,
                    Accepted = item.Accepted,
                    Area = item.Area,
                    CenterX = item.CenterX,
                    CenterY = item.CenterY,
                    BoundsX = item.BoundsX,
                    BoundsY = item.BoundsY,
                    BoundsWidth = item.BoundsWidth,
                    BoundsHeight = item.BoundsHeight,
                    Angle = item.Angle,
                    RejectReason = item.RejectReason
                })
                .ToList();
        }

        private static List<VisionPipelineGeometryFeatureResult> CreateGeometryReports(
            IEnumerable<VisionPipelineGeometryFeatureResult> features)
        {
            return (features ?? Enumerable.Empty<VisionPipelineGeometryFeatureResult>())
                .Where(item => item != null)
                .Select(item => item.Clone())
                .ToList();
        }

        private static List<VisionPipelineInstanceRunReport> CreateInstanceReports(
            IEnumerable<VisionPipelineInstanceResult> instances)
        {
            return (instances ?? Enumerable.Empty<VisionPipelineInstanceResult>())
                .Where(item => item != null)
                .OrderBy(item => item.Number)
                .Select(item => new VisionPipelineInstanceRunReport
                {
                    Number = item.Number,
                    InstanceId = item.InstanceId,
                    SourceStep = item.SourceStep,
                    Accepted = item.Accepted,
                    Score = item.Score,
                    CenterX = item.CenterX,
                    CenterY = item.CenterY,
                    Angle = item.Angle,
                    Scale = item.Scale,
                    RoiCenterX = item.RoiCenterX,
                    RoiCenterY = item.RoiCenterY,
                    RoiWidth = item.RoiWidth,
                    RoiHeight = item.RoiHeight,
                    RoiAngle = item.RoiAngle,
                    MeanValue = item.MeanValue,
                    ValidPixelRatio = item.ValidPixelRatio,
                    RejectReason = item.RejectReason
                })
                .ToList();
        }

        private static VisionRecipeStepRunSummary ResolveReviewEvidenceStep(
            VisionRecipeRunResult result,
            IReadOnlyList<VisionRecipeStepRunSummary> summaries)
        {
            return result?.FirstFailedStep
                ?? summaries?.LastOrDefault(step => !step.Skipped && (step.Overlays?.Count ?? 0) > 0)
                ?? result?.FinalStepSummary
                ?? summaries?.LastOrDefault(step => !step.Skipped);
        }

        private static bool IsExecutedPinArrayGapStep(VisionRecipeStepRunSummary summary)
        {
            return summary?.Enabled == true
                && !summary.Skipped
                && string.Equals(summary.ToolType, "PinArrayGap", StringComparison.OrdinalIgnoreCase);
        }

        private static VisionPipelineStep ResolvePipelineStep(VisionPipeline pipeline, VisionRecipeStepRunSummary summary)
        {
            if (summary == null || pipeline?.Steps == null)
            {
                return null;
            }

            int index = summary.Index - 1;
            return index >= 0 && index < pipeline.Steps.Count
                ? pipeline.Steps[index]
                : pipeline.Steps.FirstOrDefault(step => string.Equals(step?.Name, summary.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static string SaveRecipeOverlayImage(
            string directory,
            VisionRecipeStepRunSummary summary,
            VisionPipelineStep pipelineStep,
            Mat sourceImage)
        {
            if (sourceImage == null || sourceImage.Empty() || summary == null)
            {
                return string.Empty;
            }

            string fileName = $"{summary.Index:00}_{SanitizeFileName(summary.Name)}_{SanitizeFileName(summary.OutputLayer)}_overlay.png";
            string path = Path.Combine(directory, fileName);
            try
            {
                Directory.CreateDirectory(directory);
                using (Bitmap bitmap = CreatePngCompatibleBitmap(sourceImage))
                {
                    VisionPipelineRunReportImageRenderer.RenderInPlace(bitmap, summary, pipelineStep);
                    if (!TrySavePng(bitmap, path))
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return File.Exists(path) ? fileName : string.Empty;
        }

        private static string SaveSourceImage(string directory, Mat image)
        {
            if (image == null || image.Empty())
            {
                return string.Empty;
            }

            const string fileName = "source.png";
            string path = Path.Combine(directory, fileName);
            try
            {
                Directory.CreateDirectory(directory);
                using (Bitmap bitmap = CreatePngCompatibleBitmap(image))
                {
                    if (!TrySavePng(bitmap, path))
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return File.Exists(path) ? fileName : string.Empty;
        }

        private static string SaveResultImage(string directory, int index, VisionPipelineStep step, Mat image)
        {
            if (image == null || image.Empty())
            {
                return string.Empty;
            }

            string fileName = $"{index:00}_{SanitizeFileName(step?.Name)}_{SanitizeFileName(step?.OutputLayer)}.png";
            string path = Path.Combine(directory, fileName);
            try
            {
                Directory.CreateDirectory(directory);
                using (Bitmap bitmap = CreatePngCompatibleBitmap(image))
                {
                    if (!TrySavePng(bitmap, path))
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return fileName;
        }

        private static string SaveOverlayImage(string directory, int index, VisionPipelineStepResult stepResult)
        {
            VisionPipelineStep step = stepResult?.Step;
            Mat image = stepResult?.ToolResult?.ResultImage;
            if (image == null || image.Empty())
            {
                return string.Empty;
            }

            string fileName = $"{index:00}_{SanitizeFileName(step?.Name)}_{SanitizeFileName(step?.OutputLayer)}_overlay.png";
            string path = Path.Combine(directory, fileName);
            Directory.CreateDirectory(directory);
            using (Bitmap bitmap = CreatePngCompatibleBitmap(image))
            {
                VisionPipelineRunReportImageRenderer.RenderInPlace(bitmap, stepResult, index);
                if (!TrySavePng(bitmap, path))
                {
                    return string.Empty;
                }
            }

            return File.Exists(path) ? fileName : string.Empty;
        }

        private static Bitmap CreatePngCompatibleBitmap(Mat image)
        {
            using (Bitmap source = BitmapImageConverter.ToBitmap(image))
            {
                Bitmap target = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(target))
                {
                    graphics.DrawImageUnscaled(source, 0, 0);
                }

                return target;
            }
        }

        private static bool TrySavePng(Bitmap bitmap, string path)
        {
            if (bitmap == null || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    bitmap.Save(stream, ImageFormat.Png);
                }

                return File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        private static string ComputeFileSha256(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return string.Empty;
            }

            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using SHA256 sha256 = SHA256.Create();
                return Convert.ToHexString(sha256.ComputeHash(stream));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SanitizeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Item";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Item" : sanitized;
        }
    }
}
