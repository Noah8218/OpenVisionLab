using static Lib.OpenCV.Tool.CVMean;

namespace Lib.OpenCV.Property
{
    public interface IOpenCVPropertyMean : IOpenCVPropertyBase
    {            
        int MEAN_MAX { get; set; }         
        int MEAN_MIN { get; set; } 
        MeanType MEAN_TYPES { get; set; } 
    }
}
