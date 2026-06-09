namespace OpenVisionLab.Vision._1._Tools.OpenCV
{
    public interface IOpenCvConfigurableProperty<T> where T : OpenCvPropertyBase
    {
        T LoadConfig(string recipeName);
        T LoadTestConfig(string path);
    }
}
