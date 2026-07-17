using Lib.Common;
using Lib.OpenCV.Pipeline;
using OpenVisionLab._1._Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    public sealed class VisionPipelineProjectManifest
    {
        public string RecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string SavedAt { get; set; } = string.Empty;
        public string PipelineFile { get; set; } = string.Empty;
        public string LayerManifestFile { get; set; } = string.Empty;
        public string StepPreviewManifestFile { get; set; } = string.Empty;
        public int LayerCount { get; set; }
        public int StepCount { get; set; }
        public List<VisionPipelineProjectLayer> Layers { get; set; } = new List<VisionPipelineProjectLayer>();
        public List<VisionPipelineProjectStep> Steps { get; set; } = new List<VisionPipelineProjectStep>();
    }

    public sealed class VisionPipelineProjectLayer
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageFile { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public sealed class VisionPipelineProjectStep
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string InputLayer { get; set; } = string.Empty;
        public string OutputLayer { get; set; } = string.Empty;
        public bool UseAcceptance { get; set; }
        public string AcceptanceMetricName { get; set; } = string.Empty;
        public int ParameterCount { get; set; }
        public string PreviewImageFile { get; set; } = string.Empty;
    }

    internal static class VisionPipelineProjectManifestStorage
    {
        private const string ProjectManifestFileName = "project.xml";
        private const string LayerManifestFileName = "layers.tsv";
        private const string StepPreviewDirectoryName = "StepPreviews";
        private const string StepPreviewManifestFileName = "steps.tsv";

        public static string Save(
            string recipeName,
            VisionPipeline pipeline,
            IDisplayManager displayManager,
            string pipelineImageDirectory)
        {
            if (string.IsNullOrWhiteSpace(pipelineImageDirectory))
            {
                throw new ArgumentException("Pipeline image directory is empty.", nameof(pipelineImageDirectory));
            }

            Directory.CreateDirectory(pipelineImageDirectory);

            VisionPipeline target = pipeline ?? new VisionPipeline { Name = "Pipeline" };
            Dictionary<string, string> layerFiles = ReadLayerManifest(pipelineImageDirectory);
            Dictionary<string, string> previewFiles = ReadStepPreviewManifest(pipelineImageDirectory);

            VisionPipelineProjectManifest manifest = new VisionPipelineProjectManifest
            {
                RecipeName = recipeName ?? string.Empty,
                PipelineName = string.IsNullOrWhiteSpace(target.Name) ? "Pipeline" : target.Name,
                SavedAt = DateTime.Now.ToString("o"),
                PipelineFile = Path.GetFileName(RecipeWorkspaceService.GetVisionPipelinePath(recipeName, target.Name)),
                LayerManifestFile = LayerManifestFileName,
                StepPreviewManifestFile = Path.Combine(StepPreviewDirectoryName, StepPreviewManifestFileName),
                StepCount = target.Steps.Count
            };

            manifest.Layers = CreateLayerEntries(displayManager, layerFiles);
            manifest.LayerCount = manifest.Layers.Count;
            manifest.Steps = CreateStepEntries(target, previewFiles);

            string manifestPath = Path.Combine(pipelineImageDirectory, ProjectManifestFileName);
            SerializeHelper.SaveXmlFile(manifestPath, manifest);
            return manifestPath;
        }

        private static List<VisionPipelineProjectLayer> CreateLayerEntries(
            IDisplayManager displayManager,
            IDictionary<string, string> layerFiles)
        {
            List<VisionPipelineProjectLayer> layers = new List<VisionPipelineProjectLayer>();
            if (displayManager == null)
            {
                return layers;
            }

            for (int index = 0; index < displayManager.LayerCount; index++)
            {
                string title = displayManager.GetLayerTitle(index);
                if (string.IsNullOrWhiteSpace(title))
                {
                    continue;
                }

                Bitmap image = displayManager.GetLayerImage(index);
                layerFiles.TryGetValue(title, out string imageFile);
                layers.Add(new VisionPipelineProjectLayer
                {
                    Index = index,
                    Name = title,
                    ImageFile = imageFile ?? string.Empty,
                    Width = image?.Width ?? 0,
                    Height = image?.Height ?? 0
                });
            }

            return layers;
        }

        private static List<VisionPipelineProjectStep> CreateStepEntries(
            VisionPipeline pipeline,
            IDictionary<string, string> previewFiles)
        {
            List<VisionPipelineProjectStep> steps = new List<VisionPipelineProjectStep>();
            if (pipeline?.Steps == null)
            {
                return steps;
            }

            for (int index = 0; index < pipeline.Steps.Count; index++)
            {
                VisionPipelineStep step = pipeline.Steps[index];
                if (step == null)
                {
                    continue;
                }

                previewFiles.TryGetValue(CreateStepPreviewKey(index, step.Name, step.OutputLayer), out string previewFile);
                steps.Add(new VisionPipelineProjectStep
                {
                    Index = index + 1,
                    Name = step.Name ?? string.Empty,
                    ToolType = step.ToolType ?? string.Empty,
                    Enabled = step.Enabled,
                    InputLayer = step.InputLayer ?? string.Empty,
                    OutputLayer = step.OutputLayer ?? string.Empty,
                    UseAcceptance = step.UseAcceptance,
                    AcceptanceMetricName = step.AcceptanceMetricName ?? string.Empty,
                    ParameterCount = step.Parameters?.Count ?? 0,
                    PreviewImageFile = previewFile ?? string.Empty
                });
            }

            return steps;
        }

        private static Dictionary<string, string> ReadLayerManifest(string pipelineImageDirectory)
        {
            string manifestPath = Path.Combine(pipelineImageDirectory, LayerManifestFileName);
            Dictionary<string, string> entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(manifestPath))
            {
                return entries;
            }

            foreach (string line in File.ReadAllLines(manifestPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length != 2)
                {
                    continue;
                }

                string title = UnescapeManifestValue(parts[0]);
                string fileName = UnescapeManifestValue(parts[1]);
                if (!string.IsNullOrWhiteSpace(title))
                {
                    entries[title] = fileName ?? string.Empty;
                }
            }

            return entries;
        }

        private static Dictionary<string, string> ReadStepPreviewManifest(string pipelineImageDirectory)
        {
            string previewDirectory = Path.Combine(pipelineImageDirectory, StepPreviewDirectoryName);
            string manifestPath = Path.Combine(previewDirectory, StepPreviewManifestFileName);
            Dictionary<string, string> entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(manifestPath))
            {
                return entries;
            }

            foreach (string line in File.ReadAllLines(manifestPath))
            {
                string[] parts = line.Split('\t');
                if (parts.Length != 4)
                {
                    continue;
                }

                int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int stepIndex);
                string stepName = UnescapeManifestValue(parts[1]);
                string outputLayer = UnescapeManifestValue(parts[2]);
                string fileName = UnescapeManifestValue(parts[3]);
                string relativeFile = Path.Combine(StepPreviewDirectoryName, fileName ?? string.Empty);
                entries[CreateStepPreviewKey(stepIndex, stepName, outputLayer)] = relativeFile;
            }

            return entries;
        }

        private static string CreateStepPreviewKey(int zeroBasedIndex, string stepName, string outputLayer)
        {
            return string.Join(
                "|",
                zeroBasedIndex.ToString(CultureInfo.InvariantCulture),
                stepName ?? string.Empty,
                outputLayer ?? string.Empty);
        }

        private static string UnescapeManifestValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\\", "\\");
        }
    }
}
