using OpenCvSharp;
namespace Lib.OpenCV.Property
{
    public interface IOpenCVPropertyMatching : IOpenCVPropertyBase
    {        
        TemplateMatchModes MATCH_MODE { get; set; }
        double SCORE_MIN { get; set; }
        double MAGNIFIATION { get; set; }
        int NUM_MATCH { get; set; } 
        bool USE_FIND_ANGLE { get; set; }
        double FIND_ANGLE { get; set; } 
        int FIND_ANGLE_MAX { get; set; } 
        int FIND_ANGLE_MIN { get; set; } 
        string PATTERN_PATH { get; set; } 
        bool USE_CANNY { get; set; }         
        int CANNY_HIGH { get; set; } 
        int CANNY_LOW { get; set; }         
        bool USE_PADDING_COLOR_WHITE { get; set; }         
    }
}
