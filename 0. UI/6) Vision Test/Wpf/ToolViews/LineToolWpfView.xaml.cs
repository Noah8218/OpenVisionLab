using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    public partial class LineToolWpfView : VisionToolSingleInputPropertyToolViewBase, ISingleInputPropertyVisionToolWpfView<LineGaugeProperty>
    {
        public const string LinePurposeParameterName = "LinePurpose";
        private readonly LineToolPresenter presenter;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly VisionToolSingleInputSpecialPropertyToolController toolController;
        private readonly LineToolInteractionController interactionController;
        private readonly LineToolReviewController reviewController;
        private readonly LineToolPreviewController previewController;
        private readonly VisionToolPresetButtonPresenter<LineGaugeProperty> presetPresenter;
        private readonly LineToolTextPresenter textPresenter;

        internal LineToolWpfView(LineToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            toolController = VisionToolSingleInputSpecialPropertyToolController.Attach(
                this,
                "VisionMenu.Line",
                lineToolContentHost,
                ClearResultReview);
            AttachPropertyToolController(toolController);
            LineToolVerificationGuidePresenter lineVerificationGuidePresenter = new LineToolVerificationGuidePresenter(
                toolController.SummaryText,
                toolController.ResultGuidanceText);
            toolController.ResultGuidanceText.Visibility = Visibility.Collapsed;
            propertyChangeController = new VisionToolPropertyChangeController(
                UpdateSummary,
                ClearResultReview,
                _ => PersistLineProperties(),
                refreshOverlay: () => previewController?.UpdateInputRoiOverlay(),
                schedulePreview: () => previewController?.ScheduleAutoPreview(),
                cancelPreview: () => previewController?.CancelAutoPreview(),
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
                PersistLineProperties,
                () => EditSelectedRoiRequested(this, EventArgs.Empty));
            previewController = new LineToolPreviewController(
                this,
                presenter,
                toolController,
                interactionController);
            LineToolResultReviewPresenter resultReviewPresenter = new LineToolResultReviewPresenter(
                this,
                toolController.ResultReviewText,
                toolController.ResultReviewChips,
                () => interactionController.SelectedPurpose,
                () => interactionController.GetSelectedLineProperty());
            textPresenter = new LineToolTextPresenter(
                presenter,
                txtPurposeLabel,
                txtLineSelectorLabel,
                rdoPurposeEdge,
                rdoPurposeMeasure,
                rdoPurposeIntersection,
                rdoLineA,
                rdoLineB,
                txtPurposeHint,
                btnEditSelectedRoi,
                toolController.SetSummaryText);
            presetPresenter = toolController.AttachPresetPresenter(
                VisionToolPresetCatalog.GetLinePresets(),
                ApplyPreset);
            reviewController = new LineToolReviewController(
                toolController,
                interactionController,
                resultReviewPresenter,
                lineVerificationGuidePresenter,
                textPresenter);
            ApplyLocalization();
            UpdateSummary();
            ClearResultReview();
            toolController.AttachLanguageChange(RefreshLocalization);
        }

        public event EventHandler EditSelectedRoiRequested = delegate { };

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
            return previewController.ConsumeThresholdTeachingPreviewRequest();
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
            interactionController.ConfigureSelectedLineForTest(projectionDirection, polarity, verticalDirection);
        }

        public void ConfigureSelectedLineThresholdForTest(double threshold, bool invert)
        {
            interactionController.ConfigureSelectedLineThresholdForTest(threshold, invert);
            // Test hooks bypass WPG change events; keep their behavior aligned with a real threshold slider edit.
            previewController.ScheduleAutoPreview();
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
            interactionController.ConfigureSelectedLineMeasureTuningForTest(
                useThreshold,
                useAdaptiveThreshold,
                contrast,
                thickness,
                samplingStep,
                pointRange,
                useManualAngle,
                manualAngleValue);
        }

        public void ConfigureSelectedLineDrawForTest(bool showVerticalLine, bool showEdge, bool showContour, bool showFitLine)
        {
            interactionController.ConfigureSelectedLineDrawForTest(showVerticalLine, showEdge, showContour, showFitLine);
        }

        public void EnsureDefaultRoi(int width, int height)
        {
            interactionController.EnsureDefaultRoi(width, height);
        }

        public void ApplySelectedLineRoi(OpenCvSharp.Rect roi)
        {
            interactionController.ApplySelectedLineRoi(roi);
        }

        public void SetRoiForTest(OpenCvSharp.Rect roi)
        {
            interactionController.SetRoiForTest(roi);
        }

        public override void SetInputPreview(Bitmap image)
        {
            previewController.SetInputPreview(image);
        }

        public void SetResultReview(IEnumerable<LineGaugeResult> results)
        {
            reviewController.ShowLineResult(results);
        }

        public void SetDistanceResultReview(VisionToolResult result)
        {
            reviewController.ShowDistanceResult(result);
        }

        public void SetIntersectionResultReview(LineGaugeTool lineA, LineGaugeTool lineB, OpenCvSharp.Point intersectionPoint)
        {
            reviewController.ShowIntersectionResult(lineA, lineB, intersectionPoint);
        }

        public void SetIntersectionResultReview(VisionToolResult result)
        {
            reviewController.ShowIntersectionResult(result);
        }

        private void RefreshLocalization()
        {
            ApplyLocalization();
            propertyGridController.RefreshSelectedObject();
            UpdateSummary();
        }

        protected override void DisposeToolResources()
        {
            presetPresenter.Dispose();
            previewController.Dispose();
            interactionController.Detach();
            propertyGridController.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
            textPresenter.ApplyLocalization(interactionController.SelectedPurpose);
            presetPresenter?.ApplyLocalization();
        }

        private void UpdateSummary()
        {
            reviewController?.RefreshTeachingSummary();
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

            previewController.CancelAutoPreview();
            preset.ApplyTo(property);
            PersistLineProperties();
            propertyGridController.RefreshAndApplyVisibilityRules();
            UpdateSummary();
            previewController.UpdateInputRoiOverlay();
            ClearResultReview();
        }

        private void PersistLineProperties()
        {
            OpenVisionNativeToolPropertySessionStore.Save("Line(L)_1", presenter.LineAProperty);
            OpenVisionNativeToolPropertySessionStore.Save("Line(R)_1", presenter.LineBProperty);
        }

        private void ClearResultReview()
        {
            reviewController?.ClearResultReview();
        }

    }
}
