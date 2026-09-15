namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentState
    {
        public OpenVisionDockDocumentState(string contentId, bool canFloat, object content)
        {
            ContentId = contentId ?? string.Empty;
            CanFloat = canFloat;
            Content = content;
        }

        public string ContentId { get; }

        public bool CanFloat { get; }

        public object Content { get; }
    }
}
