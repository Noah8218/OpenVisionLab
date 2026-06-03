using OpenCvSharp;
namespace Lib.OpenCV.Property
{
    public interface IOpenCVPropertyContour : IOpenCVPropertyBase
    {        
        bool USE_APPROXPOLYDP { get; set; }
        
        bool USE_DRAW_IMAGE { get; set; }
        
        ContourApproximationModes ApproximationModes { get; set; }

        RetrievalModes DetectMode { get; set; } 
        
        double EPSILON { get; set; } 

        int MIN_AREA { get; set; } 
        
        int MAX_AREA { get; set; } 
        
        System.Drawing.Color DrawColor { get; set; }        
        
        int DrawThickness { get; set; } 
        
        string ClrGridHtml { get; set; }        
    }
}
