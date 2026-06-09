using static Lib.Common.FormulaUtil;

namespace Lib.OpenCV.Property
{
    public interface IOpenCvPropertyLineGauge : IOpenCVPropertyBase
    {        
        PROJECTION_POLARITY PRJ_PORALITY { get; set; }
        PROJECTION_DIR PRJ_DIR { get; set; } 
        double CONTRAST { get; set; }         
        double THICKNESS { get; set; } 
        double SAMPLING_STEP { get; set; }         
        PROJECTION_DIR VER_PRJ_DIR { get; set; } 
        int POINT_RANGE { get; set; }
        bool USE_MANUAL_ANGLE { get; set; }        
        double MANUAL_ANGLE_VALUE { get; set; } 
        bool USE_EXTEND_FIT_LINE { get; set; } 
        int EXTEND_FIT_LINE_VALUE { get; set; } 
        bool SHOW_VERTICAL_LINE { get; set; }         
        bool SHOW_EDGE { get; set; }         
        bool SHOW_CONTOUR { get; set; } 
        bool SHOW_FITLINE { get; set; }
    }
}
