using OpenVisionLab.Contracts;
using OpenVisionLab.Vision2D;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Globalization;
using System.IO;

namespace OpenVisionLab.ViewModels
{

    internal static class VisionToolPropertySummaryViewModel
    {
        public static string CreateThresholdSummary(OpenCvPropertyBase property)
        {
            if (property == null)
            {
                return "Original";
            }

            if (property.USE_THRESHOLD)
            {
                return string.Format(CultureInfo.CurrentCulture, "T {0:0.#}", property.THRESHOLD);
            }

            if (property.USE_ADAPTIVE_THRESHOLD)
            {
                return string.Format(CultureInfo.CurrentCulture, "Adaptive {0:0.#}", property.ADAPTIVE_THRESHOLD);
            }

            return "Original";
        }

        public static string CreateRoiSummary(OpenCvPropertyBase property)
        {
            if (property == null || !property.USE_ROI)
            {
                return "Full image";
            }

            return property.USE_MULTI_ROI ? "Multi ROI" : "ROI";
        }

        public static VisionToolTemplateStatus CreateTemplateStatus(string path, Mat imageTemplate)
        {
            bool hasPath = !string.IsNullOrWhiteSpace(path);
            bool exists = hasPath && File.Exists(path);
            bool loaded = exists && !OpenCvHelper.IsImageEmpty(imageTemplate);

            if (loaded)
            {
                string text = string.Format(
                    CultureInfo.CurrentCulture,
                    "Template ready / {0}x{1} / {2}",
                    imageTemplate.Width,
                    imageTemplate.Height,
                    Path.GetFileName(path));
                return new VisionToolTemplateStatus(text, true);
            }

            return new VisionToolTemplateStatus(
                hasPath ? "Template file missing / " + path : "Template not selected",
                false);
        }

        public static void DisableImagePreprocessDefaults(OpenCvPropertyBase property, bool includeCanny)
        {
            if (property == null)
            {
                return;
            }

            // Template tools should start from the loaded source/template, not hidden threshold/ROI defaults.
            property.USE_THRESHOLD = false;
            property.USE_ADAPTIVE_THRESHOLD = false;
            property.USE_BITWISENOT = false;
            property.USE_ROI = false;
            property.USE_MULTI_ROI = false;

            if (includeCanny && property is MatchingProperty matching)
            {
                matching.USE_CANNY = false;
            }
        }

        public static void OrderRange(ref int minimum, ref int maximum)
        {
            if (minimum <= maximum)
            {
                return;
            }

            int temp = minimum;
            minimum = maximum;
            maximum = temp;
        }

        public static void OrderRange(ref double minimum, ref double maximum)
        {
            if (minimum <= maximum)
            {
                return;
            }

            double temp = minimum;
            minimum = maximum;
            maximum = temp;
        }

        public static int ClampInt(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        public static double ClampDouble(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
