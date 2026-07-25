using OpenVisionLab.Contracts;
using Lib.Common;
using OpenVisionLab.Mvvm;
using System;
using System.Globalization;

namespace OpenVisionLab.ViewModels
{
    internal sealed class LineToolViewModel : ObservableObject, ILineToolViewModel
    {
        public LineToolViewModel(LineGaugeProperty lineAProperty, LineGaugeProperty lineBProperty)
        {
            LineAProperty = lineAProperty ?? new LineGaugeProperty("Line_A");
            LineBProperty = lineBProperty ?? new LineGaugeProperty("Line_B");
            ApplyDefaultDirections();
        }

        public LineGaugeProperty LineAProperty { get; }

        public LineGaugeProperty LineBProperty { get; }

        public LineGaugeProperty GetSelectedLineProperty(bool isLineBSelected)
        {
            return isLineBSelected ? LineBProperty : LineAProperty;
        }

        public LineGaugeProperty CreateSelectedLineProperty(bool isLineBSelected)
        {
            return isLineBSelected ? CreateLineBProperty() : CreateLineAProperty();
        }

        public LineGaugeProperty CreateLineAProperty()
        {
            Normalize(LineAProperty, "Line_A");
            return LineAProperty.DeepCopy();
        }

        public LineGaugeProperty CreateLineBProperty()
        {
            Normalize(LineBProperty, "Line_B");
            return LineBProperty.DeepCopy();
        }

        public string CreateSummary(LineToolPurpose purpose, bool isLineBSelected, string purposeText, string lineText)
        {
            LineGaugeProperty property = GetSelectedLineProperty(isLineBSelected);
            Normalize(property, isLineBSelected ? "Line_B" : "Line_A");

            string threshold = VisionToolPropertySummaryViewModel.CreateThresholdSummary(property);
            string roi = property.CvROI.Width > 0 && property.CvROI.Height > 0
                ? string.Format(CultureInfo.CurrentCulture, "ROI {0}x{1}", property.CvROI.Width, property.CvROI.Height)
                : "ROI not set";

            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} / {1} / {2} / {3} / C {4:0.#} / Step {5:0.#} / {6}",
                purposeText,
                lineText,
                property.PRJ_DIR,
                property.PRJ_PORALITY,
                property.CONTRAST,
                property.SAMPLING_STEP,
                string.Join(" / ", threshold, roi));
        }

        public string CreatePurposeHint(LineToolPurpose purpose)
        {
            switch (purpose)
            {
                case LineToolPurpose.Measure:
                    return "Line A scan lines to Line B edge intersections";
                case LineToolPurpose.Intersection:
                    return "Intersection point from Line A and Line B fit-line crossing";
                default:
                    return "Edge points and fitted line stability for the selected line";
            }
        }

        private void ApplyDefaultDirections()
        {
            // Line A/B have opposite defaults; centralize them so future UI changes do not silently diverge.
            LineBProperty.PRJ_DIR = FormulaUtil.PROJECTION_DIR.X_RTOL;
            LineAProperty.VER_PRJ_DIR = FormulaUtil.PROJECTION_DIR.X_RTOL;
        }

        private static void Normalize(LineGaugeProperty property, string defaultName)
        {
            if (property == null)
            {
                return;
            }

            property.NAME = string.IsNullOrWhiteSpace(property.NAME) ? defaultName : property.NAME;
            property.PIXELPERMM = Math.Max(0D, property.PIXELPERMM);
            property.CONTRAST = Math.Max(0D, property.CONTRAST);
            property.THICKNESS = Math.Max(1D, property.THICKNESS);
            property.SAMPLING_STEP = Math.Max(1D, property.SAMPLING_STEP);
            property.POINT_RANGE = Math.Max(1, property.POINT_RANGE);
            property.THRESHOLD = VisionToolPropertySummaryViewModel.ClampDouble(property.THRESHOLD, 0D, 255D);
            property.ADAPTIVE_THRESHOLD = VisionToolPropertySummaryViewModel.ClampDouble(property.ADAPTIVE_THRESHOLD, 0D, 255D);
            property.BlockSize = Math.Max(3, property.BlockSize | 1);
            property.Weight = Math.Max(0, property.Weight);
            property.EXTEND_FIT_LINE_VALUE = Math.Max(0, property.EXTEND_FIT_LINE_VALUE);
            property.AVERAGE_Diff = Math.Max(0D, property.AVERAGE_Diff);
        }
    }
}
