namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerDocumentState
    {
        public OpenVisionDockedLayerDocumentState(
            string contentId,
            bool canFloat,
            int textureTileCount,
            bool isCompactSizeReady,
            bool isCompactChrome)
        {
            ContentId = contentId ?? string.Empty;
            CanFloat = canFloat;
            TextureTileCount = textureTileCount;
            IsCompactSizeReady = isCompactSizeReady;
            IsCompactChrome = isCompactChrome;
        }

        public string ContentId { get; }

        public bool CanFloat { get; }

        public int TextureTileCount { get; }

        public bool IsCompactSizeReady { get; }

        public bool IsCompactChrome { get; }
    }
}
