using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal static class VisionPipelineStorage
    {
        private const string ActivePipelineFileName = "pipeline.active";

        public static VisionPipeline Load(string recipeName, string pipelineName)
        {
            VisionPipeline defaultPipeline = new VisionPipeline
            {
                Name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName
            };

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, defaultPipeline.Name);
            return SerializeHelper.LoadOrCreateXmlFile(path, defaultPipeline, out _);
        }

        public static void Save(string recipeName, VisionPipeline pipeline)
        {
            VisionPipeline target = pipeline ?? new VisionPipeline { Name = "Pipeline" };
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, target.Name);
            SerializeHelper.SaveXmlFile(path, target);
        }

        public static bool TryLoadFromFile(string path, out VisionPipeline pipeline, out string message)
        {
            pipeline = null;
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                message = "Pipeline XML file was not found.";
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(path, out pipeline) || pipeline == null)
            {
                message = "Pipeline XML could not be loaded as an OpenVision pipeline.";
                return false;
            }

            message = $"Loaded pipeline '{pipeline.Name}' with {pipeline.Steps.Count} step(s).";
            return true;
        }

        public static bool TrySaveToFile(string path, VisionPipeline pipeline, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                message = "Export path is empty.";
                return false;
            }

            if (pipeline == null)
            {
                message = "Pipeline is null.";
                return false;
            }

            try
            {
                SerializeHelper.SaveXmlFile(path, pipeline);
                message = $"Exported pipeline '{pipeline.Name}' to {path}.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool TryDuplicatePipeline(
            string recipeName,
            string sourcePipelineName,
            string targetPipelineName,
            out string message)
        {
            message = string.Empty;
            string sourceName = NormalizePipelineName(sourcePipelineName);
            string targetName = NormalizePipelineName(targetPipelineName);
            if (string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
            {
                message = "Source and target pipeline names are the same.";
                return false;
            }

            string sourcePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, sourceName);
            string targetPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, targetName);
            if (!File.Exists(sourcePath))
            {
                message = "Source pipeline XML was not found.";
                return false;
            }

            if (File.Exists(targetPath))
            {
                message = "Target pipeline already exists.";
                return false;
            }

            if (!TryLoadFromFile(sourcePath, out VisionPipeline pipeline, out message))
            {
                return false;
            }

            pipeline.Name = targetName;
            Save(recipeName, pipeline);
            message = $"Duplicated pipeline '{sourceName}' to '{targetName}'.";
            return true;
        }

        public static bool TryRenamePipeline(
            string recipeName,
            string oldPipelineName,
            string newPipelineName,
            out string message)
        {
            message = string.Empty;
            string oldName = NormalizePipelineName(oldPipelineName);
            string newName = NormalizePipelineName(newPipelineName);
            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                message = "Pipeline name did not change.";
                return false;
            }

            string oldPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, oldName);
            string newPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, newName);
            if (!File.Exists(oldPath))
            {
                message = "Pipeline XML was not found.";
                return false;
            }

            if (File.Exists(newPath))
            {
                message = "Target pipeline already exists.";
                return false;
            }

            if (!TryLoadFromFile(oldPath, out VisionPipeline pipeline, out message))
            {
                return false;
            }

            pipeline.Name = newName;
            Save(recipeName, pipeline);
            File.Delete(oldPath);

            string activeName = LoadActivePipelineName(recipeName, "Pipeline");
            if (string.Equals(activeName, oldName, StringComparison.OrdinalIgnoreCase))
            {
                SaveActivePipelineName(recipeName, newName);
            }

            message = $"Renamed pipeline '{oldName}' to '{newName}'.";
            return true;
        }

        public static bool TryDeletePipeline(
            string recipeName,
            string pipelineName,
            out string fallbackPipelineName,
            out string message)
        {
            fallbackPipelineName = string.Empty;
            message = string.Empty;
            string name = NormalizePipelineName(pipelineName);
            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            if (pipelineNames.Length <= 1)
            {
                message = "Cannot delete the last pipeline in a recipe.";
                return false;
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, name);
            if (!File.Exists(path))
            {
                message = "Pipeline XML was not found.";
                return false;
            }

            fallbackPipelineName = pipelineNames
                .FirstOrDefault(candidate => !string.Equals(candidate, name, StringComparison.OrdinalIgnoreCase))
                ?? "Pipeline";
            File.Delete(path);

            string activeName = LoadActivePipelineName(recipeName, "Pipeline");
            if (string.Equals(activeName, name, StringComparison.OrdinalIgnoreCase))
            {
                SaveActivePipelineName(recipeName, fallbackPipelineName);
            }

            message = $"Deleted pipeline '{name}'.";
            return true;
        }

        public static string LoadActivePipelineName(string recipeName, string fallbackName)
        {
            string fallback = string.IsNullOrWhiteSpace(fallbackName) ? "Pipeline" : fallbackName.Trim();
            string path = GetActivePipelineNamePath(recipeName);
            if (!System.IO.File.Exists(path))
            {
                return fallback;
            }

            string name = System.IO.File.ReadAllText(path)?.Trim();
            return string.IsNullOrWhiteSpace(name) ? fallback : name;
        }

        public static void SaveActivePipelineName(string recipeName, string pipelineName)
        {
            string name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim();
            string path = GetActivePipelineNamePath(recipeName);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllText(path, name);
        }

        private static string GetActivePipelineNamePath(string recipeName)
        {
            return RecipeWorkspaceService.GetVisionConfigPath(recipeName, ActivePipelineFileName);
        }

        private static string NormalizePipelineName(string pipelineName)
        {
            string name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName.Trim();
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(name.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Pipeline" : sanitized;
        }

        public static bool TryValidateRoundTrip(string recipeName, VisionPipeline pipeline, out string message)
        {
            message = string.Empty;
            if (pipeline == null)
            {
                message = "Pipeline is null.";
                return false;
            }

            try
            {
                VisionPipeline loaded = Load(recipeName, pipeline.Name);
                if (loaded == null)
                {
                    message = "Saved pipeline could not be loaded.";
                    return false;
                }

                if (!SameText(pipeline.Name, loaded.Name))
                {
                    message = $"Pipeline name mismatch. saved='{pipeline.Name}', loaded='{loaded.Name}'.";
                    return false;
                }

                if (pipeline.Steps.Count != loaded.Steps.Count)
                {
                    message = $"Step count mismatch. saved={pipeline.Steps.Count}, loaded={loaded.Steps.Count}.";
                    return false;
                }

                for (int i = 0; i < pipeline.Steps.Count; i++)
                {
                    if (!CompareStep(pipeline.Steps[i], loaded.Steps[i], i, out message))
                    {
                        return false;
                    }
                }

                message = $"Round-trip validation passed. Steps={pipeline.Steps.Count}.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        private static bool CompareStep(VisionPipelineStep expected, VisionPipelineStep actual, int index, out string message)
        {
            message = string.Empty;
            if (expected == null || actual == null)
            {
                message = $"Step {index + 1} is null after load.";
                return false;
            }

            if (!SameText(expected.Name, actual.Name)
                || !SameText(expected.ToolType, actual.ToolType)
                || expected.Enabled != actual.Enabled
                || !SameText(expected.InputLayer, actual.InputLayer)
                || !SameText(expected.OutputLayer, actual.OutputLayer)
                || expected.UseAcceptance != actual.UseAcceptance
                || expected.ExpectedSuccess != actual.ExpectedSuccess
                || !SameDouble(expected.MaxElapsedMilliseconds, actual.MaxElapsedMilliseconds)
                || !SameText(expected.RequiredMessageText, actual.RequiredMessageText)
                || !SameText(expected.AcceptanceMetricName, actual.AcceptanceMetricName)
                || expected.UseAcceptanceMetricMinimum != actual.UseAcceptanceMetricMinimum
                || !SameDouble(expected.AcceptanceMetricMinimum, actual.AcceptanceMetricMinimum)
                || expected.UseAcceptanceMetricMaximum != actual.UseAcceptanceMetricMaximum
                || !SameDouble(expected.AcceptanceMetricMaximum, actual.AcceptanceMetricMaximum))
            {
                message = $"Step {index + 1} metadata mismatch. '{expected.Name}'";
                return false;
            }

            if (!CompareParameters(expected.Parameters, actual.Parameters, index, out message))
            {
                return false;
            }

            return true;
        }

        private static bool CompareParameters(
            IDictionary<string, string> expected,
            IDictionary<string, string> actual,
            int stepIndex,
            out string message)
        {
            message = string.Empty;
            Dictionary<string, string> expectedMap = new Dictionary<string, string>(expected ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> actualMap = new Dictionary<string, string>(actual ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);

            string missingKey = expectedMap.Keys.FirstOrDefault(key => !actualMap.ContainsKey(key));
            if (!string.IsNullOrWhiteSpace(missingKey))
            {
                message = $"Step {stepIndex + 1} parameter missing after load: {missingKey}.";
                return false;
            }

            string extraKey = actualMap.Keys.FirstOrDefault(key => !expectedMap.ContainsKey(key));
            if (!string.IsNullOrWhiteSpace(extraKey))
            {
                message = $"Step {stepIndex + 1} unexpected parameter after load: {extraKey}.";
                return false;
            }

            foreach (KeyValuePair<string, string> parameter in expectedMap)
            {
                if (!SameText(parameter.Value, actualMap[parameter.Key]))
                {
                    message = $"Step {stepIndex + 1} parameter mismatch: {parameter.Key}.";
                    return false;
                }
            }

            return true;
        }

        private static bool SameText(string left, string right)
        {
            return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
        }

        private static bool SameDouble(double left, double right)
        {
            return Math.Abs(left - right) < 0.0000001;
        }
    }
}
