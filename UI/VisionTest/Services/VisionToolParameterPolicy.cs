using OpenCvSharp;
using System;

namespace OpenVisionLab.Services
{
    internal static class VisionToolParameterPolicy
    {
        public static int NormalizeThresholdBlockSize(int value)
        {
            value = Math.Max(3, value);
            if (value % 2 == 0)
            {
                value++;
            }

            return Math.Min(99, value);
        }

        public static int NormalizePositiveSize(int value)
        {
            return Math.Max(1, value);
        }

        public static int NormalizeOddKernelSize(int value)
        {
            value = NormalizePositiveSize(value);
            return value % 2 == 1 ? value : value + 1;
        }

        public static MorphShapes ParseMorphShape(string value)
        {
            return Enum.TryParse(value, true, out MorphShapes parsedShape) ? parsedShape : MorphShapes.Rect;
        }

        public static MorphTypes ParseMorphOperation(string value)
        {
            return Enum.TryParse(value, true, out MorphTypes parsedOperation) ? parsedOperation : MorphTypes.Erode;
        }
    }
}
