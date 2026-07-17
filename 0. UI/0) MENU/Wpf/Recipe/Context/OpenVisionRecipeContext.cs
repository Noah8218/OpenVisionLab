using System;
using System.Globalization;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeContext
    {
        public OpenVisionRecipeContext(
            string id,
            string name,
            string pipelineName,
            string sourcePath,
            bool isDirty,
            string activeLayerName,
            string lastReviewState)
        {
            Id = Normalize(id, "Default");
            Name = Normalize(name, "Default");
            PipelineName = Normalize(pipelineName, VisionPipelineAppendService.DefaultPipelineName);
            SourcePath = sourcePath ?? string.Empty;
            IsDirty = isDirty;
            ActiveLayerName = Normalize(activeLayerName, "Main");
            LastReviewState = lastReviewState ?? string.Empty;
        }

        public string Id { get; }

        public string Name { get; }

        public string PipelineName { get; }

        public string SourcePath { get; }

        public bool IsDirty { get; }

        public string ActiveLayerName { get; }

        public string LastReviewState { get; }

        public string DisplayText
        {
            get
            {
                string dirtySuffix = IsDirty ? " *" : string.Empty;
                return string.Format(CultureInfo.CurrentCulture, "{0} / {1}{2}", Name, PipelineName, dirtySuffix);
            }
        }

        private static string Normalize(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }
    }
}
