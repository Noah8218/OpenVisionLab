using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    public partial class LineToolWpfView : UserControl, ISingleInputPropertyVisionToolWpfView<LineGaugeProperty>, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        public const string LinePurposeParameterName = "LinePurpose";
        private readonly LineToolPresenter presenter;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolSingleInputSpecialPropertyToolController toolController;
        private readonly LineToolInteractionController interactionController;
        private readonly LineToolResultReviewPresenter resultReviewPresenter;
        private readonly LineToolVerificationGuidePresenter lineVerificationGuidePresenter;
        private readonly VisionToolPresetButtonPresenter<LineGaugeProperty> presetPresenter;
        private bool autoPreviewShouldShowThresholdTeachingImage;
        private bool thresholdTeachingPreviewRequested;

        internal LineToolWpfView(LineToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            toolController = VisionToolSingleInputSpecialPropertyToolController.Attach(
                this,
                "VisionMenu.Line",
                lineToolContentHost,
                ClearResultReview);
            lineVerificationGuidePresenter = new LineToolVerificationGuidePresenter(
                toolController.SummaryText,
                toolController.ResultGuidanceText);
            toolController.ResultGuidanceText.Visibility = Visibility.Collapsed;
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RunAutoPreview, 120);
            propertyChangeController = new VisionToolPropertyChangeController(
                UpdateSummary,
                ClearResultReview,
                _ => PersistLineProperties(),
                refreshOverlay: UpdateInputRoiOverlay,
                schedulePreview: ScheduleAutoPreview,
                cancelPreview: previewScheduler.Cancel,
                shouldSchedulePreview: VisionToolPropertyPreviewPolicy.ShouldScheduleAutoPreview);
            propertyGridController = VisionToolPropertyGridHost.Attach(
                toolController.PropertyGridHost,
                presenter.LineAProperty,
                propertyChangeController.OnPropertyValueChanged);
            interactionController = new LineToolInteractionController(
                presenter,
                propertyGridController,
                propertyChangeController,
                rdoPurposeEdge,
                rdoPurposeMeasure,
                rdoPurposeIntersection,
                rdoLineA,
                rdoLineB,
                btnEditSelectedRoi,
                UpdateSummary,
                ClearResultReview,
                () => EditSelectedRoiRequested(this, EventArgs.Empty));
            resultReviewPresenter = new LineToolResultReviewPresenter(
                this,
                toolController.ResultReviewText,
                toolController.ResultReviewChips,
                () => interactionController.SelectedPurpose,
                () => interactionController.GetSelectedLineProperty());
            presetPresenter = toolController.AttachPresetPresenter(
                VisionToolPresetCatalog.GetLinePresets(),
                ApplyPreset);
            ApplyLocalization();
            UpdateSummary();
            ClearResultReview();
            toolController.AttachLanguageChange(RefreshLocalization);
        }

        public event EventHandler SourceLayerChanged
        {
            add { toolController.SourceLayerChanged += value; }
            remove { toolController.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { toolController.DestinationLayerChanged += value; }
            remove { toolController.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { toolController.InputPreviewClicked += value; }
            remove { toolController.InputPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { toolController.OutputPreviewClicked += value; }
            remove { toolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { toolController.CreateOutputLayerRequested += value; }
            remove { toolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler EditSelectedRoiRequested = delegate { };

        public event EventHandler RunPreviewRequested
        {
            add { toolController.RunPreviewRequested += value; }
            remove { toolController.RunPreviewRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { toolController.AddPipelineRequested += value; }
            remove { toolController.AddPipelineRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { toolController.LoadPreviewImageRequested += value; }
            remove { toolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { toolController.SavePreviewImageRequested += value; }
            remove { toolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayer => toolController.SelectedInputLayer;
        public string SelectedOutputLayer => toolController.SelectedOutputLayer;
        public string SelectedPurpose => interactionController.SelectedPurpose.ToString();
        public string SelectedLineName => interactionController.SelectedLineName;
        public bool HasInputPreviewImage => toolController.InputPreview?.HasImage ?? false;
        public int InputPreviewTextureTileCount => toolController.InputPreview?.TextureTileCount ?? 0;
        public int InputPreviewRoiOverlayCount => toolController.InputPreview?.RoiOverlayCount ?? 0;
        public string ResultReviewTextForTest => toolController.ResultReviewText?.Text ?? string.Empty;

        public LineGaugeProperty CreateProperty()
        {
            return CreateSelectedLineProperty();
        }

        public LineGaugeProperty CreateLineAProperty()
        {
            CommitPendingPropertyGridEdit();
            return presenter.CreateLineAProperty();
        }

        public LineGaugeProperty CreateLineBProperty()
        {
            CommitPendingPropertyGridEdit();
            return presenter.CreateLineBProperty();
        }

        public LineGaugeProperty CreateSelectedLineProperty()
        {
            CommitPendingPropertyGridEdit();
            return interactionController.CreateSelectedLineProperty();
        }

        public bool ConsumeThresholdTeachingPreviewRequest()
        {
            bool requested = thresholdTeachingPreviewRequested;
            thresholdTeachingPreviewRequested = false;
            return requested;
        }

        public void SetPurposeForTest(string purpose)
        {
            interactionController.SetPurposeForTest(purpose);
        }

        public void SetLineSettingForTest(string setting)
        {
            interactionController.SetLineSettingForTest(setting);
        }

        public void ConfigureSelectedLineForTest(string projectionDirection, string polarity, string verticalDirection = null)
        {
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            if (Enum.TryParse(projectionDirection, true, out Lib.Common.FormulaUtil.PROJECTION_DIR parsedProjectionDirection))
            {
                property.PRJ_DIR = parsedProjectionDirection;
            }

            if (Enum.TryParse(polarity, true, out Lib.Common.FormulaUtil.PROJECTION_POLARITY parsedPolarity))
            {
                property.PRJ_PORALITY = parsedPolarity;
            }

            if (!string.IsNullOrWhiteSpace(verticalDirection)
                && Enum.TryParse(verticalDirection, true, out Lib.Common.FormulaUtil.PROJECTION_DIR parsedVerticalDirection))
            {
                property.VER_PRJ_DIR = parsedVerticalDirection;
            }

            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        public void ConfigureSelectedLineThresholdForTest(double threshold, bool invert)
        {
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            property.USE_THRESHOLD = true;
            property.THRESHOLD = threshold;
            property.USE_BITWISENOT = invert;
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
            // Test hooks bypass WPG change events; keep their behavior aligned with a real threshold slider edit.
            ScheduleAutoPreview();
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
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            property.USE_THRESHOLD = useThreshold;
            property.USE_ADAPTIVE_THRESHOLD = useAdaptiveThreshold;
            property.CONTRAST = contrast;
            property.THICKNESS = thickness;
            property.SAMPLING_STEP = samplingStep;
            property.POINT_RANGE = pointRange;
            property.USE_MANUAL_ANGLE = useManualAngle;
            property.MANUAL_ANGLE_VALUE = manualAngleValue;
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        public void ConfigureSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine)
        {
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            property.SHOW_VERTICAL_LINE = showVerticalLine;
            property.SHOW_EDGE = showEdge;
            property.SHOW_CONTOUR = showContour;
            property.SHOW_FITLINE = showFitLine;
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        public void EnsureDefaultRoi(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
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
                propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
            }
        }

        public void ApplySelectedLineRoi(OpenCvSharp.Rect roi)
        {
            if (roi.Width <= 0 || roi.Height <= 0)
            {
                return;
            }

            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            property.USE_ROI = true;
            property.USE_MULTI_ROI = false;
            property.CvROI = roi;
            PersistLineProperties();
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        public void SetRoiForTest(OpenCvSharp.Rect roi)
        {
            if (roi.Width <= 0 || roi.Height <= 0)
            {
                return;
            }

            presenter.LineAProperty.USE_ROI = true;
            presenter.LineAProperty.USE_MULTI_ROI = false;
            presenter.LineAProperty.CvROI = roi;
            presenter.LineBProperty.USE_ROI = true;
            presenter.LineBProperty.USE_MULTI_ROI = false;
            presenter.LineBProperty.CvROI = roi;
            PersistLineProperties();
            propertyChangeController.RefreshAfterExternalUpdate(propertyGridController);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolController.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolController.SetInputPreview(image, UpdateInputRoiOverlay);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolController.SetStatus(status);
        }

        public void SetResultReview(IEnumerable<LineGaugeResult> results)
        {
            List<LineGaugeResult> resultList = results?.Where(item => item != null).ToList() ?? new List<LineGaugeResult>();
            resultReviewPresenter.Show(resultList);
            lineVerificationGuidePresenter.ShowLineResult(
                resultList,
                interactionController.SelectedPurpose,
                SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void SetDistanceResultReview(VisionToolResult result)
        {
            resultReviewPresenter.ShowDistance(result);
            lineVerificationGuidePresenter.ShowDistanceResult(
                result,
                interactionController.SelectedPurpose,
                SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void SetIntersectionResultReview(LineGaugeTool lineA, LineGaugeTool lineB, OpenCvSharp.Point intersectionPoint)
        {
            resultReviewPresenter.ShowIntersection(lineA, lineB, intersectionPoint);
            lineVerificationGuidePresenter.ShowIntersectionResult(
                true,
                interactionController.SelectedPurpose,
                SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        public void SetIntersectionResultReview(VisionToolResult result)
        {
            resultReviewPresenter.ShowIntersection(result);
            bool crosses = result?.Success == true
                && result.Metrics != null
                && result.Metrics.TryGetValue("IntersectionCross", out double crossValue)
                && crossValue >= 0.5D;
            lineVerificationGuidePresenter.ShowIntersectionResult(
                crosses,
                interactionController.SelectedPurpose,
                SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        private void RefreshLocalization()
        {
            ApplyLocalization();
            propertyGridController.RefreshSelectedObject();
            UpdateSummary();
        }

        public void DisposeView()
        {
            presetPresenter.Dispose();
            previewScheduler.Dispose();
            toolController.Dispose();
            interactionController.Detach();
            propertyGridController.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
            txtPurposeLabel.Text = VisionToolVerificationText.LinePurposeLabel;
            txtLineSelectorLabel.Text = VisionToolVerificationText.LineSettingLabel;
            rdoPurposeEdge.Content = VisionToolVerificationText.LinePurposeEdge;
            rdoPurposeMeasure.Content = VisionToolVerificationText.LinePurposeMeasure;
            rdoPurposeIntersection.Content = VisionToolVerificationText.LinePurposeIntersection;
            rdoLineA.Content = VisionToolVerificationText.LineA;
            rdoLineB.Content = VisionToolVerificationText.LineB;
            txtPurposeHint.Text = VisionToolVerificationText.CreateLinePurposeHint(interactionController.SelectedPurpose.ToString());
            VisionToolChromePresenter.ApplyTooltip(btnEditSelectedRoi, VisionToolVerificationText.EditSelectedLineRoiTooltip);
            presetPresenter?.ApplyLocalization();
        }

        private void UpdateSummary()
        {
            LineToolPurpose purpose = interactionController.SelectedPurpose;
            if (!string.IsNullOrWhiteSpace(toolController.ResultReviewText.Text))
            {
                return;
            }

            string purposeText = VisionToolVerificationText.CreateLinePurposeText(purpose.ToString());
            toolController.SetSummaryText(presenter.CreateSummary(purpose, interactionController.IsLineBSelected, purposeText, SelectedLineName));
            txtPurposeHint.Text = VisionToolVerificationText.CreateLinePurposeHint(purpose.ToString());
            lineVerificationGuidePresenter?.ShowTeachingState(
                purpose,
                SelectedLineName,
                interactionController.GetSelectedLineProperty());
        }

        private void CommitPendingPropertyGridEdit()
        {
            if (propertyGridController.CommitPendingEdit())
            {
                PersistLineProperties();
                UpdateSummary();
            }
        }

        private void ApplyPreset(VisionToolPreset<LineGaugeProperty> preset)
        {
            if (preset == null)
            {
                return;
            }

            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            if (property == null)
            {
                return;
            }

            previewScheduler.Cancel();
            preset.ApplyTo(property);
            PersistLineProperties();
            propertyGridController.RefreshAndApplyVisibilityRules();
            UpdateSummary();
            UpdateInputRoiOverlay();
            ClearResultReview();
        }

        private void PersistLineProperties()
        {
            OpenVisionNativeToolPropertySessionStore.Save("Line(L)_1", presenter.LineAProperty);
            OpenVisionNativeToolPropertySessionStore.Save("Line(R)_1", presenter.LineBProperty);
        }

        private void ClearResultReview()
        {
            toolController?.ClearResultReview();
            if (interactionController != null)
            {
                lineVerificationGuidePresenter?.ShowTeachingState(
                    interactionController.SelectedPurpose,
                    SelectedLineName,
                    interactionController.GetSelectedLineProperty());
            }
        }

        private void ScheduleAutoPreview()
        {
            autoPreviewShouldShowThresholdTeachingImage = ShouldShowThresholdTeachingPreview();
            previewScheduler.Schedule();
        }

        private void RunAutoPreview()
        {
            thresholdTeachingPreviewRequested = autoPreviewShouldShowThresholdTeachingImage;
            autoPreviewShouldShowThresholdTeachingImage = false;
            toolController.RequestRunPreview();
        }

        private bool ShouldShowThresholdTeachingPreview()
        {
            LineGaugeProperty property = interactionController.GetSelectedLineProperty();
            return property != null && (property.USE_THRESHOLD || property.USE_ADAPTIVE_THRESHOLD);
        }

        private void UpdateInputRoiOverlay()
        {
            VisionToolInlinePreviewSlot inputPreview = toolController.InputPreview;
            if (inputPreview == null)
            {
                return;
            }

            if (!inputPreview.HasImage)
            {
                inputPreview.ClearRoiOverlays();
                return;
            }

            inputPreview.SetLineRoiOverlays(
                presenter.LineAProperty.CvROI,
                presenter.LineBProperty.CvROI,
                interactionController.IsLineBSelected);
        }
    }
}
