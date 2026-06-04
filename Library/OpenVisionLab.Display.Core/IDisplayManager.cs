using OpenVisionLab.ImageSpace.Core;

namespace OpenVisionLab._1._Core
{
    public interface IDisplayManager : IVisionRuntimeContext, IDisplayLayerManager
    {
        IImageSpace ImageSpace { get; }
    }
}
