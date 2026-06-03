using OpenCvSharp;
using System.Collections.Generic;

namespace Lib.OpenCV.Property
{
    public interface IOpenCVPropertyBase
    {
        string NAME { get; set; }
        double PIXELPERMM { get; set; }
        bool USE_THRESHOLD { get; set; }
        bool USE_BITWISENOT { get; set; }
        ThresholdTypes THRESHOLD_TYPES { get; set; }
        double THRESHOLD { get; set; }
        bool USE_ADAPTIVE_THRESHOLD { get; set; }
        double ADAPTIVE_THRESHOLD { get; set; }
        ThresholdTypes ADAPTIVE_THRESHOLD_TYPES { get; set; }
        AdaptiveThresholdTypes ADAPTIVE_THRESHOLD_ALGORITHM { get; set; }
        int BlockSize { get; set; }
        int Weight { get; set; }
        bool USE_ROI { get; set; }
        bool USE_MULTI_ROI { get; set; }
        Rect CvROI { get; set; }
        List<Rect> CvROIS { get; set; }
        List<Rect> CvMASKS { get; set; }
    }
}
