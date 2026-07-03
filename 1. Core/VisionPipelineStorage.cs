using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
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
