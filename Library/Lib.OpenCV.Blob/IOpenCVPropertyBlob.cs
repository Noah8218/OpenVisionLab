using Lib.OpenCV.Property;

namespace Lib.OpenCV.Blob
{
    public interface IOpenCVPropertyBlob : IOpenCVPropertyBase
    {
        int MIN_AREA { get; set; }
        int MAX_AREA { get; set; }
    }
}
