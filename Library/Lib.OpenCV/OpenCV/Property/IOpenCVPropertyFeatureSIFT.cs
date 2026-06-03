namespace Lib.OpenCV.Property
{
    public interface IOpenCVPropertyFeatureSIFT : IOpenCVPropertyBase
    {
        double RANSAC_REPROJ_THRESHOLD { get; set; }
        double SCORE_MIN { get; set; }                
        string PATTERN_PATH { get; set; }
    }
}
