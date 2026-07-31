using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeGraySignalEvidenceCalculator
    {
        private const int BinCount = 256;

        public static IReadOnlyList<double> CreateNormalizedHistogram(Mat image)
        {
            if (image == null || image.Empty())
            {
                throw new ArgumentException("An image is required.", nameof(image));
            }

            using Mat gray = CreateGray8Copy(image);
            int[] counts = new int[BinCount];
            long pixelCount = checked((long)gray.Rows * gray.Cols);
            if (pixelCount <= 0)
            {
                return new double[BinCount];
            }

            byte[] rowPixels = new byte[gray.Cols];
            for (int row = 0; row < gray.Rows; row++)
            {
                Marshal.Copy(gray.Ptr(row), rowPixels, 0, rowPixels.Length);
                for (int column = 0; column < rowPixels.Length; column++)
                {
                    counts[rowPixels[column]]++;
                }
            }

            double[] normalized = new double[BinCount];
            for (int index = 0; index < normalized.Length; index++)
            {
                normalized[index] = counts[index] * 100d / pixelCount;
            }

            return normalized;
        }

        public static string ComputeImageSha256(Mat image)
        {
            if (image == null || image.Empty())
            {
                throw new ArgumentException("An image is required.", nameof(image));
            }

            Cv2.ImEncode(".png", image, out byte[] encoded);
            return Convert.ToHexString(SHA256.HashData(encoded));
        }

        public static string CreateEvidenceId(params string[] identityParts)
        {
            string canonicalIdentity = string.Join("\n", identityParts ?? Array.Empty<string>());
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalIdentity)));
        }

        internal static Mat CreateGray8Copy(Mat image)
        {
            Mat gray = new Mat();
            if (image.Channels() == 1)
            {
                image.CopyTo(gray);
            }
            else if (image.Channels() == 4)
            {
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            }

            if (gray.Type() == MatType.CV_8UC1)
            {
                return gray;
            }

            Mat normalized = new Mat();
            Cv2.Normalize(gray, normalized, 0, 255, NormTypes.MinMax);
            gray.Dispose();
            Mat converted = new Mat();
            normalized.ConvertTo(converted, MatType.CV_8UC1);
            normalized.Dispose();
            return converted;
        }
    }
}
