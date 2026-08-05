using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class LineToolInteractionController
    {
        private readonly LineToolPresenter presenter;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly RadioButton purposeEdgeRadioButton;
        private readonly RadioButton purposeMeasureRadioButton;
        private readonly RadioButton purposeIntersectionRadioButton;
        private readonly RadioButton lineARadioButton;
        private readonly RadioButton lineBRadioButton;
        private readonly Button editSelectedRoiButton;
        private readonly Action refreshSummary;
        private readonly Action clearResultReview;
        private readonly Action persistProperties;
        private readonly Action editSelectedRoiRequested;
        private bool disposed;

        public LineToolInteractionController(
            LineToolPresenter presenter,
            VisionToolPropertyGridHost propertyGridController,
            VisionToolPropertyChangeController propertyChangeController,
            RadioButton purposeEdgeRadioButton,
            RadioButton purposeMeasureRadioButton,
            RadioButton purposeIntersectionRadioButton,
            RadioButton lineARadioButton,
            RadioButton lineBRadioButton,
            Button editSelectedRoiButton,
            Action refreshSummary,
            Action clearResultReview,
            Action persistProperties,
            Action editSelectedRoiRequested)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.propertyGridController = propertyGridController ?? throw new ArgumentNullException(nameof(propertyGridController));
            this.propertyChangeController = propertyChangeController ?? throw new ArgumentNullException(nameof(propertyChangeController));
            this.purposeEdgeRadioButton = purposeEdgeRadioButton ?? throw new ArgumentNullException(nameof(purposeEdgeRadioButton));
            this.purposeMeasureRadioButton = purposeMeasureRadioButton ?? throw new ArgumentNullException(nameof(purposeMeasureRadioButton));
            this.purposeIntersectionRadioButton = purposeIntersectionRadioButton ?? throw new ArgumentNullException(nameof(purposeIntersectionRadioButton));
            this.lineARadioButton = lineARadioButton ?? throw new ArgumentNullException(nameof(lineARadioButton));
            this.lineBRadioButton = lineBRadioButton ?? throw new ArgumentNullException(nameof(lineBRadioButton));
            this.editSelectedRoiButton = editSelectedRoiButton ?? throw new ArgumentNullException(nameof(editSelectedRoiButton));
            this.refreshSummary = refreshSummary ?? throw new ArgumentNullException(nameof(refreshSummary));
            this.clearResultReview = clearResultReview ?? throw new ArgumentNullException(nameof(clearResultReview));
            this.persistProperties = persistProperties ?? throw new ArgumentNullException(nameof(persistProperties));
            this.editSelectedRoiRequested = editSelectedRoiRequested ?? throw new ArgumentNullException(nameof(editSelectedRoiRequested));

            AttachCommands();
        }

        public LineToolPurpose SelectedPurpose
        {
            get
            {
                if (purposeMeasureRadioButton.IsChecked == true)
                {
                    return LineToolPurpose.Measure;
                }

                if (purposeIntersectionRadioButton.IsChecked == true)
                {
                    return LineToolPurpose.Intersection;
                }

                return LineToolPurpose.Edge;
            }
        }

        public string SelectedLineName => IsLineBSelected ? "Line B" : "Line A";

        public bool IsLineBSelected => lineBRadioButton.IsChecked == true;

        public LineGaugeProperty GetSelectedLineProperty()
        {
            return presenter.GetSelectedLineProperty(IsLineBSelected);
        }

        public LineGaugeProperty CreateSelectedLineProperty()
        {
            return presenter.CreateSelectedLineProperty(IsLineBSelected);
        }

        public void SetPurposeForTest(string purpose)
        {
            if (string.Equals(purpose, nameof(LineToolPurpose.Measure), StringComparison.OrdinalIgnoreCase))
            {
                purposeMeasureRadioButton.IsChecked = true;
            }
            else if (string.Equals(purpose, nameof(LineToolPurpose.Intersection), StringComparison.OrdinalIgnoreCase))
            {
                purposeIntersectionRadioButton.IsChecked = true;
            }
            else
            {
                purposeEdgeRadioButton.IsChecked = true;
            }

            HandlePurposeChanged();
        }

        public void SetLineSettingForTest(string setting)
        {
            if (string.Equals(setting, "Line B", StringComparison.OrdinalIgnoreCase)
                || string.Equals(setting, "B", StringComparison.OrdinalIgnoreCase)
                || string.Equals(setting, "Right", StringComparison.OrdinalIgnoreCase))
            {
                lineBRadioButton.IsChecked = true;
            }
            else
            {
                lineARadioButton.IsChecked = true;
            }

            HandleLineSelectionChanged();
        }

        public void ConfigureSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null)
        {
            LineGaugeProperty property = GetSelectedLineProperty();
            if (Enum.TryParse(projectionDirection, true, out OpenVisionLab.Core.FormulaUtil.PROJECTION_DIR parsedProjectionDirection))
            {
                property.PRJ_DIR = parsedProjectionDirection;
            }

            if (Enum.TryParse(polarity, true, out OpenVisionLab.Core.FormulaUtil.PROJECTION_POLARITY parsedPolarity))
            {
                property.PRJ_PORALITY = parsedPolarity;
            }

            if (!string.IsNullOrWhiteSpace(verticalDirection)
                && Enum.TryParse(verticalDirection, true, out OpenVisionLab.Core.FormulaUtil.PROJECTION_DIR parsedVerticalDirection))
            {
                property.VER_PRJ_DIR = parsedVerticalDirection;
            }

            RefreshPropertyGridAfterExternalUpdate();
        }

        public void ConfigureSelectedLineThresholdForTest(double threshold, bool invert)
        {
            LineGaugeProperty property = GetSelectedLineProperty();
            property.USE_THRESHOLD = true;
            property.THRESHOLD = threshold;
            property.USE_BITWISENOT = invert;
            RefreshPropertyGridAfterExternalUpdate();
        }

        public void ConfigureSelectedLineMeasureTuningForTest(
            bool useThreshold,
            bool useAdaptiveThreshold,
            double contrast,
            double thickness,
            double samplingStep,
            int pointRange,
            bool useManualAngle,
            double manualAngleValue)
        {
            LineGaugeProperty property = GetSelectedLineProperty();
            property.USE_THRESHOLD = useThreshold;
            property.USE_ADAPTIVE_THRESHOLD = useAdaptiveThreshold;
            property.CONTRAST = contrast;
            property.THICKNESS = thickness;
            property.SAMPLING_STEP = samplingStep;
            property.POINT_RANGE = pointRange;
            property.USE_MANUAL_ANGLE = useManualAngle;
            property.MANUAL_ANGLE_VALUE = manualAngleValue;
            RefreshPropertyGridAfterExternalUpdate();
        }

        public void ConfigureSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine)
        {
            LineGaugeProperty property = GetSelectedLineProperty();
            property.SHOW_VERTICAL_LINE = showVerticalLine;
            property.SHOW_EDGE = showEdge;
            property.SHOW_CONTOUR = showContour;
            property.SHOW_FITLINE = showFitLine;
            RefreshPropertyGridAfterExternalUpdate();
        }

        public bool EnsureDefaultRoi(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return false;
            }

            bool changed = false;
            if (presenter.LineAProperty.CvROI.Width <= 0 || presenter.LineAProperty.CvROI.Height <= 0)
            {
                presenter.LineAProperty.CvROI = new OpenCvSharp.Rect(0, 0, width, height);
                changed = true;
            }

            if (presenter.LineBProperty.CvROI.Width <= 0 || presenter.LineBProperty.CvROI.Height <= 0)
            {
                presenter.LineBProperty.CvROI = new OpenCvSharp.Rect(0, 0, width, height);
                changed = true;
            }

            if (changed)
            {
                RefreshPropertyGridAfterExternalUpdate();
            }

            return changed;
        }

        public bool ApplySelectedLineRoi(OpenCvSharp.Rect roi)
        {
            if (roi.Width <= 0 || roi.Height <= 0)
            {
                return false;
            }

            ApplyRoi(GetSelectedLineProperty(), roi);
            persistProperties();
            RefreshPropertyGridAfterExternalUpdate();
            return true;
        }

        public bool SetRoiForTest(OpenCvSharp.Rect roi)
        {
            if (roi.Width <= 0 || roi.Height <= 0)
            {
                return false;
            }

            ApplyRoi(presenter.LineAProperty, roi);
            ApplyRoi(presenter.LineBProperty, roi);
            persistProperties();
            RefreshPropertyGridAfterExternalUpdate();
            return true;
        }

        public void Detach()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            purposeEdgeRadioButton.Command = null;
            purposeMeasureRadioButton.Command = null;
            purposeIntersectionRadioButton.Command = null;
            lineARadioButton.Command = null;
            lineBRadioButton.Command = null;
            editSelectedRoiButton.Command = null;
        }

        private void AttachCommands()
        {
            // Selection commands keep radio/button behavior on the MVVM command path instead of View event handlers.
            purposeEdgeRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            purposeMeasureRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            purposeIntersectionRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            lineARadioButton.Command = new RelayCommand(HandleLineSelectionChanged);
            lineBRadioButton.Command = new RelayCommand(HandleLineSelectionChanged);
            editSelectedRoiButton.Command = new RelayCommand(editSelectedRoiRequested);
        }

        private void HandlePurposeChanged()
        {
            refreshSummary();
            clearResultReview();
        }

        private void HandleLineSelectionChanged()
        {
            propertyGridController.SelectObject(GetSelectedLineProperty());
            propertyChangeController.RefreshViewState();
        }

        private void RefreshPropertyGridAfterExternalUpdate()
        {
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        private static void ApplyRoi(LineGaugeProperty property, OpenCvSharp.Rect roi)
        {
            property.USE_ROI = true;
            property.USE_MULTI_ROI = false;
            property.CvROI = roi;
        }
    }
}
