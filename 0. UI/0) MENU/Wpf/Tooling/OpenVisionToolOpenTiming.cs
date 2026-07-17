using System.Globalization;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionToolOpenTiming
    {
        public VISION_MENU Menu { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public bool ReusedFloatingWindow { get; set; }
        public long ActivateDocumentMs { get; set; }
        public long RefreshLayerStateMs { get; set; }
        public long ResolveSizeMs { get; set; }
        public long PrepareHostedDocumentMs { get; set; }
        public long FloatingHostShowMs { get; set; }
        public long CompleteSelectionMs { get; set; }
        public long TotalMs { get; set; }
        public string DetailText { get; set; } = string.Empty;

        public string ToPerfText()
        {
            string summary = string.Join(
                "|",
                "InternalPath=" + Path,
                "InternalMenu=" + Menu.ToString(),
                "InternalDocument=" + Document,
                "InternalReusedWindow=" + ReusedFloatingWindow.ToString(CultureInfo.InvariantCulture),
                "InternalActivateDocumentMs=" + ActivateDocumentMs.ToString(CultureInfo.InvariantCulture),
                "InternalRefreshLayerStateMs=" + RefreshLayerStateMs.ToString(CultureInfo.InvariantCulture),
                "InternalResolveSizeMs=" + ResolveSizeMs.ToString(CultureInfo.InvariantCulture),
                "InternalPrepareHostedDocumentMs=" + PrepareHostedDocumentMs.ToString(CultureInfo.InvariantCulture),
                "InternalFloatingHostShowMs=" + FloatingHostShowMs.ToString(CultureInfo.InvariantCulture),
                "InternalCompleteSelectionMs=" + CompleteSelectionMs.ToString(CultureInfo.InvariantCulture),
                "InternalTotalMs=" + TotalMs.ToString(CultureInfo.InvariantCulture));
            return string.IsNullOrWhiteSpace(DetailText)
                ? summary
                : summary + "|" + DetailText;
        }
    }
}
