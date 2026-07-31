using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal static class OpenVisionGuidedSetupCatalog
    {
        internal const string PinGapTemplate = "Pin gap / edge distance (LineDistance)";
        internal const string PinArrayGapTemplate = "Pin row edge-gap consistency (PinArrayGap)";
        internal const string DarkBandGapTemplate = "Dark band thickness / Gap (LineDistance)";
        internal const string HybridRelativeRoiGapTemplate = "Locator-aligned Gap (NormalizeImage)";
        internal const string MatchingTemplate = "Template Matching";
        internal const string FeatureMatchingTemplate = "Feature Matching";
        internal const string EdgeBasedMatchingTemplate = "Edge Based Matching";
        internal const string ReferenceDifferenceTemplate = "Golden-reference defect (ReferenceDifference)";
        internal const string ContourTemplate = "Shape boundary (Contour)";
        internal const string BlobTemplate = "Threshold + Blob";
        internal const string MeanTemplate = "Mean Intensity";

        internal static bool TryResolveTemplate(VISION_MENU menu, out string template)
        {
            switch (menu)
            {
                case VISION_MENU.Line:
                    template = PinGapTemplate;
                    return true;
                case VISION_MENU.Blob:
                    template = BlobTemplate;
                    return true;
                case VISION_MENU.Contour:
                    template = ContourTemplate;
                    return true;
                case VISION_MENU.Matching:
                    template = MatchingTemplate;
                    return true;
                case VISION_MENU.FeatureMatching:
                    template = FeatureMatchingTemplate;
                    return true;
                case VISION_MENU.EdgeBasedMatching:
                    template = EdgeBasedMatchingTemplate;
                    return true;
                case VISION_MENU.Mean:
                    template = MeanTemplate;
                    return true;
                default:
                    template = string.Empty;
                    return false;
            }
        }
    }
}
