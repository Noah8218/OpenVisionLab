namespace OpenVisionLab.Docking.Controls
{
    public sealed class OpenVisionDockDocumentRefreshResult
    {
        public OpenVisionDockDocumentRefreshResult(bool hasDocuments, int documentCount)
        {
            HasDocuments = hasDocuments;
            DocumentCount = documentCount;
        }

        public bool HasDocuments { get; }

        public int DocumentCount { get; }
    }
}
