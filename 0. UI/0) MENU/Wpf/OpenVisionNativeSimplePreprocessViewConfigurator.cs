using Lib.OpenCV;
using Lib.OpenCV.Property;
using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeSimplePreprocessViewConfigurator
    {
        public static void ConfigureEdgeDetectionView(SimplePreprocessToolWpfView view)
        {
            view.SetLocalizedHeader("VisionMenu.EdgeDetection", "Edge Detection", PackIconMaterialKind.Filter);
            view.AddChoice(
                "EdgeType",
                "Edge Type",
                Enum.GetValues(typeof(EdgeDetectionToolType)).Cast<object>(),
                EdgeDetectionToolType.Canny,
                "PropertyGrid.Property.EdgeType.DisplayName");
            view.AddNumber("CannyThresholdLow", "Canny Low", 100, 0, 255, false, false, "PropertyGrid.Property.CannyThresholdLow.DisplayName");
            view.AddNumber("CannyThresholdHigh", "Canny High", 200, 0, 255, false, false, "PropertyGrid.Property.CannyThresholdHigh.DisplayName");
            view.AddNumber("CannyApertureSize", "Canny Aperture", 3, 1, 31, false, false, "PropertyGrid.Property.CannyApertureSize.DisplayName");
            view.AddCheck("UseL2Gradient", "Use L2 Gradient", true, "PropertyGrid.Property.UseL2Gradient.DisplayName");
            view.AddNumber("SobelDegreeX", "Sobel X", 1, 0, 9, false, false, "PropertyGrid.Property.SobelDegreeX.DisplayName");
            view.AddNumber("SobelDegreeY", "Sobel Y", 0, 0, 9, false, false, "PropertyGrid.Property.SobelDegreeY.DisplayName");
            view.AddNumber("SobelKernelSize", "Sobel Kernel", 3, 1, 31, false, false, "PropertyGrid.Property.SobelKernelSize.DisplayName");
            view.AddNumber("ScharrDegreeX", "Scharr X", 1, 0, 9, false, false, "PropertyGrid.Property.ScharrDegreeX.DisplayName");
            view.AddNumber("ScharrDegreeY", "Scharr Y", 0, 0, 9, false, false, "PropertyGrid.Property.ScharrDegreeY.DisplayName");
            view.AddNumber("LaplacianKernelSize", "Laplacian Kernel", 3, 1, 31, false, false, "PropertyGrid.Property.LaplacianKernelSize.DisplayName");
            view.ParameterChanged += (sender, e) =>
            {
                UpdateEdgeDetectionParameterVisibility(view);
                UpdateEdgeDetectionSummary(view);
            };
            UpdateEdgeDetectionParameterVisibility(view);
            UpdateEdgeDetectionSummary(view);
        }

        public static void ConfigureRotateScaleView(SimplePreprocessToolWpfView view)
        {
            view.SetLocalizedHeader("VisionMenu.RotateAndScale", "Rotate / Scale", PackIconMaterialKind.Rotate3dVariant);
            view.AddSlider("Angle", "Angle", -180, 180, 0, 1, "RotateScale.rjLabel4.Text");
            view.AddSlider("ScaleXPercent", "Scale X", 10, 300, 100, 1, "RotateScale.rjLabelScaleX.Text");
            view.AddSlider("ScaleYPercent", "Scale Y", 10, 300, 100, 1, "RotateScale.rjLabelScaleY.Text");
            view.ParameterChanged += (sender, e) => UpdateRotateScaleSummary(view);
            UpdateRotateScaleSummary(view);
        }

        public static void ConfigureMeanView(SimplePreprocessToolWpfView view)
        {
            MeanProperty property = new MeanProperty("Mean");
            view.SetLocalizedHeader("VisionMenu.Mean", "Mean", PackIconMaterialKind.FunctionVariant);
            view.AddChoice(
                "MeanType",
                "Mean Type",
                Enum.GetValues(typeof(MeanType)).Cast<object>(),
                property.MEAN_TYPES,
                "SimplePreprocess.MeanType");
            view.AddSlider("MeanMin", "Min Mean", 0, 255, property.MEAN_MIN, 1, "SimplePreprocess.MeanMin");
            view.AddSlider("MeanMax", "Max Mean", 0, 255, property.MEAN_MAX, 1, "SimplePreprocess.MeanMax");
            view.ParameterChanged += (sender, e) => UpdateMeanSummary(view);
            UpdateMeanSummary(view);
        }

        public static void ConfigureHsvView(SimplePreprocessToolWpfView view)
        {
            view.SetLocalizedHeader("VisionMenu.HSV", "HSV", PackIconMaterialKind.Palette);
            view.SetAddPipelineVisible(false);
            view.AddRangeSliderPair(
                "HueRange",
                "Hue",
                "HueMin",
                "Hue Min",
                "HueMax",
                "Hue Max",
                0,
                179,
                0,
                179,
                1,
                "SimplePreprocess.HueRange",
                "SimplePreprocess.HueMin",
                "SimplePreprocess.HueMax");
            view.AddRangeSliderPair(
                "SaturationRange",
                "Saturation",
                "SatMin",
                "Saturation Min",
                "SatMax",
                "Saturation Max",
                0,
                255,
                0,
                255,
                1,
                "SimplePreprocess.SaturationRange",
                "SimplePreprocess.SaturationMin",
                "SimplePreprocess.SaturationMax");
            view.AddRangeSliderPair(
                "ValueRange",
                "Value",
                "ValMin",
                "Value Min",
                "ValMax",
                "Value Max",
                0,
                255,
                0,
                255,
                1,
                "SimplePreprocess.ValueRange",
                "SimplePreprocess.ValueMin",
                "SimplePreprocess.ValueMax");
            view.ParameterChanged += (sender, e) => UpdateHsvSummary(view);
            UpdateHsvSummary(view);
        }

        public static void ConfigureHistogramView(SimplePreprocessToolWpfView view)
        {
            view.SetLocalizedHeader("VisionMenu.Histogram", "Histogram", PackIconMaterialKind.ChartHistogram);
            view.SetAddPipelineVisible(false);
            view.AddChoice(
                "HistogramType",
                "Type",
                Enum.GetValues(typeof(HistogramPreviewType)).Cast<object>(),
                HistogramPreviewType.clahe,
                "SimplePreprocess.HistogramType");
            view.AddNumber("ClipLimit", "Clip Limit", 3, 0, 999, true, false, "SimplePreprocess.ClipLimit");
            view.AddNumber("TilesGridSize", "Tile Grid", 3, 1, 99, false, false, "SimplePreprocess.TileGrid");
            view.AddNumber("Alpha", "Normalize Alpha", 0, 0, 255, false, false, "SimplePreprocess.NormalizeAlpha");
            view.AddNumber("Beta", "Normalize Beta", 100, 0, 255, false, false, "SimplePreprocess.NormalizeBeta");
            view.ParameterChanged += (sender, e) =>
            {
                UpdateHistogramParameterVisibility(view);
                UpdateHistogramSummary(view);
            };
            UpdateHistogramParameterVisibility(view);
            UpdateHistogramSummary(view);
        }

        private static void UpdateEdgeDetectionParameterVisibility(SimplePreprocessToolWpfView view)
        {
            EdgeDetectionToolType edgeType = view.GetEnum("EdgeType", EdgeDetectionToolType.Canny);
            view.SetParametersVisible(CannyParameterKeys, edgeType == EdgeDetectionToolType.Canny);
            view.SetParametersVisible(SobelParameterKeys, edgeType == EdgeDetectionToolType.Sobel);
            view.SetParametersVisible(ScharrParameterKeys, edgeType == EdgeDetectionToolType.Scharr);
            view.SetParametersVisible(LaplacianParameterKeys, edgeType == EdgeDetectionToolType.Laplacian);
        }

        private static void UpdateEdgeDetectionSummary(SimplePreprocessToolWpfView view)
        {
            EdgeDetectionToolType edgeType = view.GetEnum("EdgeType", EdgeDetectionToolType.Canny);
            string detail;
            switch (edgeType)
            {
                case EdgeDetectionToolType.Sobel:
                    detail = string.Format(
                        CultureInfo.CurrentCulture,
                        "X {0} / Y {1} / K {2}",
                        view.GetInt("SobelDegreeX", 1),
                        view.GetInt("SobelDegreeY", 0),
                        view.GetInt("SobelKernelSize", 3));
                    break;
                case EdgeDetectionToolType.Scharr:
                    detail = string.Format(
                        CultureInfo.CurrentCulture,
                        "X {0} / Y {1}",
                        view.GetInt("ScharrDegreeX", 1),
                        view.GetInt("ScharrDegreeY", 0));
                    break;
                case EdgeDetectionToolType.Laplacian:
                    detail = string.Format(
                        CultureInfo.CurrentCulture,
                        "K {0}",
                        view.GetInt("LaplacianKernelSize", 3));
                    break;
                default:
                    detail = string.Format(
                        CultureInfo.CurrentCulture,
                        "Low {0} / High {1} / Aperture {2}",
                        view.GetInt("CannyThresholdLow", 100),
                        view.GetInt("CannyThresholdHigh", 200),
                        view.GetInt("CannyApertureSize", 3));
                    break;
            }

            view.SetSummary(string.Format(CultureInfo.CurrentCulture, "{0} / {1}", edgeType, detail));
        }

        private static void UpdateRotateScaleSummary(SimplePreprocessToolWpfView view)
        {
            view.SetSummary(string.Format(
                CultureInfo.CurrentCulture,
                "Angle {0:0.#} / X {1:0.#}% / Y {2:0.#}%",
                view.GetDouble("Angle", 0),
                view.GetDouble("ScaleXPercent", 100),
                view.GetDouble("ScaleYPercent", 100)));
        }

        private static void UpdateMeanSummary(SimplePreprocessToolWpfView view)
        {
            MeanType meanType = view.GetEnum("MeanType", MeanType.Mean);
            int min = view.GetInt("MeanMin", 100);
            int max = view.GetInt("MeanMax", 240);
            view.SetSummary(string.Format(
                CultureInfo.CurrentCulture,
                "{0} / Min {1} / Max {2}",
                meanType,
                Math.Min(min, max),
                Math.Max(min, max)));
        }

        private static void UpdateHsvSummary(SimplePreprocessToolWpfView view)
        {
            int hueMin = Math.Min(view.GetInt("HueMin", 0), view.GetInt("HueMax", 179));
            int hueMax = Math.Max(view.GetInt("HueMin", 0), view.GetInt("HueMax", 179));
            int satMin = Math.Min(view.GetInt("SatMin", 0), view.GetInt("SatMax", 255));
            int satMax = Math.Max(view.GetInt("SatMin", 0), view.GetInt("SatMax", 255));
            int valMin = Math.Min(view.GetInt("ValMin", 0), view.GetInt("ValMax", 255));
            int valMax = Math.Max(view.GetInt("ValMin", 0), view.GetInt("ValMax", 255));
            view.SetSummary(string.Format(
                CultureInfo.CurrentCulture,
                "H {0}-{1} / S {2}-{3} / V {4}-{5}",
                hueMin,
                hueMax,
                satMin,
                satMax,
                valMin,
                valMax));
        }

        private static void UpdateHistogramSummary(SimplePreprocessToolWpfView view)
        {
            HistogramPreviewType histogramType = view.GetEnum("HistogramType", HistogramPreviewType.clahe);
            string detail = histogramType == HistogramPreviewType.clahe
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    "Clip {0} / Tile {1}",
                    view.GetDouble("ClipLimit", 3),
                    view.GetInt("TilesGridSize", 3))
                : histogramType == HistogramPreviewType.Normalize
                    ? string.Format(
                        CultureInfo.CurrentCulture,
                        "Alpha {0} / Beta {1}",
                        view.GetInt("Alpha", 0),
                        view.GetInt("Beta", 100))
                    : "Global equalize";
            view.SetSummary(string.Format(CultureInfo.CurrentCulture, "{0} / {1}", histogramType, detail));
        }

        private static void UpdateHistogramParameterVisibility(SimplePreprocessToolWpfView view)
        {
            HistogramPreviewType histogramType = view.GetEnum("HistogramType", HistogramPreviewType.clahe);
            view.SetParametersVisible(ClaheParameterKeys, histogramType == HistogramPreviewType.clahe);
            view.SetParametersVisible(NormalizeParameterKeys, histogramType == HistogramPreviewType.Normalize);
        }

        private static readonly string[] CannyParameterKeys =
        {
            "CannyThresholdLow",
            "CannyThresholdHigh",
            "CannyApertureSize",
            "UseL2Gradient"
        };

        private static readonly string[] SobelParameterKeys =
        {
            "SobelDegreeX",
            "SobelDegreeY",
            "SobelKernelSize"
        };

        private static readonly string[] ScharrParameterKeys =
        {
            "ScharrDegreeX",
            "ScharrDegreeY"
        };

        private static readonly string[] LaplacianParameterKeys =
        {
            "LaplacianKernelSize"
        };

        private static readonly string[] ClaheParameterKeys =
        {
            "ClipLimit",
            "TilesGridSize"
        };

        private static readonly string[] NormalizeParameterKeys =
        {
            "Alpha",
            "Beta"
        };
    }

    internal enum HistogramPreviewType
    {
        clahe,
        equalizeHist,
        Normalize
    }
}
