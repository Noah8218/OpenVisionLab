using System.Collections.Generic;

namespace OpenVisionLab
{
    internal sealed class VisionToolParameterGuideContent
    {
        public string PropertyName { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Identity { get; init; } = string.Empty;
        public string Coverage { get; init; } = string.Empty;
        public string Applicability { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;
        public string Impact { get; init; } = string.Empty;
        public string BestWhen { get; init; } = string.Empty;
        public string Risk { get; init; } = string.Empty;
        public string CheckAfterPreview { get; init; } = string.Empty;
        public IReadOnlyList<string> RelatedPropertyNames { get; init; } = new List<string>();
    }
}
