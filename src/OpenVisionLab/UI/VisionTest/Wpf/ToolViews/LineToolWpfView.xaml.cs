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
        internal bool SignalInspectorHasEvidenceForTest => signalInspector.HasEvidence;
        internal string SignalInspectorEvidenceIdForTest => signalInspector.EvidenceId;
        internal string SignalInspectorSourceSha256ForTest => signalInspector.SourceSha256;
        internal int SignalInspectorSeriesCountForTest => signalInspector.SeriesCount;
        internal int SignalInspectorMarkerCountForTest => signalInspector.MarkerCount;
        internal bool IsSignalInspectorOverlayVisibleForTest =>
            lineSignalInspectorOverlay.Visibility == Visibility.Visible;

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

        internal void ApplySampleLinePair(
            LineGaugeProperty lineA,
            LineGaugeProperty lineB,
            string purpose)
        {
            if (lineA == null || lineB == null)
            {
                return;
            }

            CopyLineProperty(lineA, presenter.LineAProperty);
            CopyLineProperty(lineB, presenter.LineBProperty);
            interactionController.SetPurposeForTest(purpose);
            PersistLineProperties();
            propertyGridController.RefreshAndApplyVisibilityRules();
            previewController.UpdateInputRoiOverlay();
            UpdateSummary();
            ClearResultReview();
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

        internal string GetSignalInspectorAttributeForTest(string name)
        {
            return signalInspector.GetAttribute(name);
        }

        internal bool ExerciseSignalInspectorNavigationForTest()
        {
            return signalInspector.ExerciseNavigationForTest();
        }

        internal void ExportSignalEvidenceForTest(string path)
        {
            signalInspector.ExportForTest(path);
        }

        internal void ShowSignalEvidence(VisionToolSignalEvidence evidence)
        {
            signalInspector.ShowEvidence(evidence);
            btnOpenSignalInspector.Visibility = Visibility.Visible;
            lineSignalInspectorOverlay.Visibility = Visibility.Visible;
        }

        internal void ClearSignalEvidence()
        {
            signalInspector.ClearEvidence();
            btnOpenSignalInspector.Visibility = Visibility.Collapsed;
            lineSignalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void CloseSignalInspectorForTest()
        {
            lineSignalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void OpenSignalInspectorForTest()
        {
            if (signalInspector.HasEvidence)
            {
                lineSignalInspectorOverlay.Visibility = Visibility.Visible;
            }
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
            signalInspector.ApplyLocalization();
            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            btnOpenSignalInspector.Content = korean ? "\uC2E0\uD638 \uAC80\uD1A0" : "Review signal";
            btnOpenSignalInspector.ToolTip = korean
                ? "\uB300\uD45C \uC2A4\uCE94\uC758 \uBC1D\uAE30\uC640 \uC5E3\uC9C0 \uC751\uB2F5\uC744 \uAC80\uD1A0\uD569\uB2C8\uB2E4."
                : "Review the representative scan intensity and edge response";
            btnCloseSignalInspector.Content = korean
                ? "\uB9E4\uAC1C\uBCC0\uC218\uB85C \uB3CC\uC544\uAC00\uAE30"
                : "Back to parameters";
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

        private static void CopyLineProperty(LineGaugeProperty source, LineGaugeProperty target)
        {
            target.PIXELPERMM = source.PIXELPERMM;
            target.USE_THRESHOLD = source.USE_THRESHOLD;
            target.USE_BITWISENOT = source.USE_BITWISENOT;
            target.THRESHOLD_TYPES = source.THRESHOLD_TYPES;
            target.THRESHOLD = source.THRESHOLD;
            target.USE_ADAPTIVE_THRESHOLD = source.USE_ADAPTIVE_THRESHOLD;
            target.ADAPTIVE_THRESHOLD = source.ADAPTIVE_THRESHOLD;
            target.ADAPTIVE_THRESHOLD_TYPES = source.ADAPTIVE_THRESHOLD_TYPES;
            target.ADAPTIVE_THRESHOLD_ALGORITHM = source.ADAPTIVE_THRESHOLD_ALGORITHM;
            target.BlockSize = source.BlockSize;
            target.Weight = source.Weight;
            target.USE_ROI = source.USE_ROI;
            target.CvROI = source.CvROI;
            target.USE_MULTI_ROI = source.USE_MULTI_ROI;
            target.CvROIS = source.CvROIS == null
                ? new List<OpenCvSharp.Rect>()
                : new List<OpenCvSharp.Rect>(source.CvROIS);
            target.USE_MASKING = source.USE_MASKING;
            target.CvMASKS = source.CvMASKS == null
                ? new List<OpenCvSharp.Rect>()
                : new List<OpenCvSharp.Rect>(source.CvMASKS);
            target.PRJ_PORALITY = source.PRJ_PORALITY;
            target.PRJ_DIR = source.PRJ_DIR;
            target.CONTRAST = source.CONTRAST;
            target.THICKNESS = source.THICKNESS;
            target.SAMPLING_STEP = source.SAMPLING_STEP;
            target.VER_PRJ_DIR = source.VER_PRJ_DIR;
            target.POINT_RANGE = source.POINT_RANGE;
            target.USE_MANUAL_ANGLE = source.USE_MANUAL_ANGLE;
            target.MANUAL_ANGLE_VALUE = source.MANUAL_ANGLE_VALUE;
            target.USE_EXTEND_FIT_LINE = source.USE_EXTEND_FIT_LINE;
            target.EXTEND_FIT_LINE_VALUE = source.EXTEND_FIT_LINE_VALUE;
            target.AVERAGE_Diff = source.AVERAGE_Diff;
            target.USE_AVERAGE_FILTER = source.USE_AVERAGE_FILTER;
            target.AVERAGE_FILTER_TYPE = source.AVERAGE_FILTER_TYPE;
            target.SHOW_VERTICAL_LINE = source.SHOW_VERTICAL_LINE;
            target.SHOW_EDGE = source.SHOW_EDGE;
            target.SHOW_CONTOUR = source.SHOW_CONTOUR;
            target.SHOW_FITLINE = source.SHOW_FITLINE;
        }

        private void ClearResultReview()
        {
            reviewController?.ClearResultReview();
            ClearSignalEvidence();
        }

        private void OpenSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            OpenSignalInspectorForTest();
        }

        private void CloseSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            CloseSignalInspectorForTest();
        }

    }
}
