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
    }

    public sealed class VisionPipelineMetricRunReport
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
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
                AcceptancePassed = stepResult?.AcceptancePassed != false,
                AcceptanceMessage = stepResult?.AcceptanceMessage ?? string.Empty,
                ElapsedMilliseconds = summary.ElapsedMilliseconds,
                Message = summary.Message,
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
                    .ToList()
            };
        }

        private static string SaveResultImage(string directory, int index, VisionPipelineStep step, Mat image)
        {
            if (image == null || image.Empty())
            {
                return string.Empty;
            }

            string fileName = $"{index:00}_{SanitizeFileName(step?.Name)}_{SanitizeFileName(step?.OutputLayer)}.png";
            string path = Path.Combine(directory, fileName);
            using (Bitmap bitmap = BitmapImageConverter.ToBitmap(image))
            {
                bitmap.Save(path, ImageFormat.Png);
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
            using (Bitmap bitmap = BitmapImageConverter.ToBitmap(image))
            using (Bitmap overlay = VisionPipelineRunReportImageRenderer.Render(bitmap, stepResult, index))
            {
                overlay?.Save(path, ImageFormat.Png);
            }

            return File.Exists(path) ? fileName : string.Empty;
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
