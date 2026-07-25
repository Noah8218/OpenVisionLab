using OpenVisionLab.ImageSpace.Core;

namespace OpenVisionLab.Core
{
    public interface IDisplayManager : IVisionRuntimeContext, IDisplayLayerManager
    {
        IImageSpace ImageSpace { get; }
    }
}
