namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerViewerFactory : IOpenVisionDockedLayerViewerFactory
    {
        public IOpenVisionDockedLayerViewer Create()
        {
            return new OpenVisionLayerViewerView();
        }
    }
}
