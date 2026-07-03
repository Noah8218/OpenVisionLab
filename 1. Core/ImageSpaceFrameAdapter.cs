using Lib.Common;
using OpenCvSharp;
using OpenVisionLab.ImageSpace.Core;
using System.Drawing;

namespace OpenVisionLab._1._Core
{
    internal static class ImageSpaceFrameAdapter
    {
        public static ImageSpaceFrame FromBitmap(Bitmap image)
        {
            return ImageSpaceFrame.FromBitmap(image);
        }

        public static ImageSpaceFrame FromMat(Mat image)
        {
            if (image == null || image.Empty()) return null;
            return ImageSpaceFrame.FromBitmap(BitmapImageConverter.ToBitmap(image));
        }

        public static Mat ToMat(ImageSpaceFrame frame)
        {
            return ToMat(frame?.Image);
        }

        public static Mat ToMat(Bitmap image)
        {
            return image == null ? new Mat() : BitmapImageConverter.ToMat(image);
        }

        public static Bitmap ToBitmap(Mat image)
        {
            if (image == null || image.Empty()) return null;
            return BitmapImageConverter.ToBitmap(image);
        }
    }
}
