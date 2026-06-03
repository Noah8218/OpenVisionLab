using Lib.Common;
using OpenCvSharp;
using System.Diagnostics;
using System.Drawing;

namespace Lib.OpenCV.Property
{
    public abstract class COpenCVAlgorithmBase
    {
        public COpenCVAlgorithmBase() { }
        public Mat imageSource { get; set; } = new Mat();
        public Mat imageResult { get; set; } = new Mat();
        public Mat imageTemplate { get; set; } = new Mat();

        public Stopwatch swTaktTimems = new Stopwatch();

        public OpenCvSharp.Size size { get; set; } = new OpenCvSharp.Size();

        public virtual void SetSourceImage(Mat Image)
        {
            Image.CopyTo(imageSource);
            size = imageSource.Size();
        }

        public virtual void SetSourceImage(Bitmap Image)
        {
            imageSource = CImageConverter.ToMat(Image);
            size = imageSource.Size();
        }

        public abstract void Run();

    }
}
