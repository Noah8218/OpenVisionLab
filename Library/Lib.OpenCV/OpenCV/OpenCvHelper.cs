using Lib.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Lib.OpenCV
{
    public class OpenCvHelper
    {
        public static bool IsMatEmpty(Mat MatSource)
        {
                        if (MatSource.IsDisposed)
            {
                return true;
            }

            if (MatSource == null)
            {
                return true;
            }

            if (MatSource.Width == 0 || MatSource.Height == 0)
            {
                return true;
            }
            return false;
        
        }

        public static bool IsRectEmpty(Rect rt)
        {
            if (rt == null)
            {

                Debug.WriteLine("Rect is null");
                return true;
            }

            if (rt.Width == 0 || rt.Height == 0)
            {

                Debug.WriteLine("Rect Size Empty");
                return true;
            }

            return false;
        }

        public static bool IsImageEmpty(Mat image)
        {
            if (image == null)
            {

                Debug.WriteLine("Image is null");
                return true;
            }

            if (image.IsDisposed)
            {

                Debug.WriteLine("Image Disposed");
                return true;
            }

            if (image.Width == 0 || image.Height == 0)
            {

                Debug.WriteLine("Image Size Empty");
                return true;
            }

            return false;
        }

        public static bool SetImageChannel4(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2RGBA);

            return true;
        }

        public static bool SetImageChannel3(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2RGB);

            return true;
        }

        public static Mat SetImageChannel3ToMat(Mat image)
        {
            if (IsImageEmpty(image)) return null;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2RGB);

            return image;
        }

        public static bool SetImageChannel1(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2GRAY);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2GRAY);

            return true;
        }
    }
}
