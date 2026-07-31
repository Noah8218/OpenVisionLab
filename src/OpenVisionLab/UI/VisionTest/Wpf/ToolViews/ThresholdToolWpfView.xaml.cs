using Lib.OpenCV;
using Lib.OpenCV.Property;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using OpenVisionLab.Contracts;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    public partial class ThresholdToolWpfView : VisionToolSingleInputCustomToolViewBase, ISingleInputPropertyVisionToolWpfView<ThresholdToolProperty>
    {
        private readonly ThresholdToolPresenter presenter;

        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolThresholdInteractionController thresholdInteractionController;
        private readonly ThresholdToolLearnWindowController learnWindowController;
        private readonly ThresholdToolTextPresenter textPresenter;
        private readonly VisionToolCustomParameterGuideBinder parameterGuideBinder;
        private VisionToolThresholdSuggestion thresholdSuggestion;
        private ThresholdSuggestionUndoState thresholdSuggestionUndo;
        private bool suppressEvents = true;

        internal ThresholdToolWpfView(ThresholdToolPresenter presenter)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            InitializeComponent();
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RequestRunPreview);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                RefreshSummaryAndClearSignalEvidence,
                schedulePreview: previewScheduler.Schedule);
            thresholdInteractionController = new VisionToolThresholdInteractionController(
                presenter,
                parameterChangeController,
                () => suppressEvents,
                value => suppressEvents = value,
                rbBasic,
                rbRange,
                rbAdaptive,
                rbBasicBinary,
                rbBasicInvert,
                rbAdaptiveMean,
                rbAdaptiveGaussian,
                rbAdaptiveBinary,
                rbAdaptiveInvert,
                chkRangeInvert,
                sliderThreshold,
                sliderRangeMin,
                sliderRangeMax,
                sliderBlockSize,
                txtThreshold,
                txtMaxValue,
                txtRangeMin,
                txtRangeMax,
                txtAdaptiveMaxValue,
                txtWeight,
                txtBlockSize,
                panelBasic,
                panelRange,
                panelAdaptive);
            signalInspector.MarkerValueChangeRequested += SignalInspector_MarkerValueChangeRequested;
            learnWindowController = new ThresholdToolLearnWindowController(
                presenter,
                thresholdInteractionController,
                () => Window.GetWindow(this));
            textPresenter = new ThresholdToolTextPresenter(
                gbThresholdParameters,
                gbMode,
                txtModeBasicTitle,
                txtModeBasicHint,
                txtModeRangeTitle,
                txtModeRangeHint,
                txtModeAdaptiveTitle,
                txtModeAdaptiveHint,
                lblBasicType,
                rbBasicBinary,
                rbBasicInvert,
                lblBasicMaxValue,
                lblBasicThreshold,
                lblRangeTitle,
                lblRangeMin,
                lblRangeMax,
                chkRangeInvert,
                lblAdaptiveMethod,
                rbAdaptiveMean,
                rbAdaptiveGaussian,
                lblAdaptiveType,
                rbAdaptiveBinary,
                rbAdaptiveInvert,
                lblAdaptiveMaxValue,
                lblAdaptiveWeight,
                lblBlockSize);
            AttachToolController(
                "VisionMenu.Threshold",
                parameterContentHost,
                refreshViewState: UpdateSummary,
                clearResultReview: ClearSignalEvidence,
                applyToolLocalization: ApplyLocalization);
            ToolController.BindSummary(new Binding("Summary"));
            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                presenter.CreateProperty,
                new Dictionary<FrameworkElement, string>
                {
                    [rbBasic] = nameof(ThresholdToolProperty.Mode),
                    [rbRange] = nameof(ThresholdToolProperty.Mode),
                    [rbAdaptive] = nameof(ThresholdToolProperty.Mode),
                    [rbBasicBinary] = nameof(ThresholdToolProperty.ThresholdType),
                    [rbBasicInvert] = nameof(ThresholdToolProperty.ThresholdType),
                    [txtThreshold] = nameof(ThresholdToolProperty.Threshold),
                    [sliderThreshold] = nameof(ThresholdToolProperty.Threshold),
                    [txtMaxValue] = nameof(ThresholdToolProperty.MaxValue),
                    [txtRangeMin] = nameof(ThresholdToolProperty.RangeMin),
                    [sliderRangeMin] = nameof(ThresholdToolProperty.RangeMin),
                    [txtRangeMax] = nameof(ThresholdToolProperty.RangeMax),
                    [sliderRangeMax] = nameof(ThresholdToolProperty.RangeMax),
                    [chkRangeInvert] = nameof(ThresholdToolProperty.Invert),
                    [rbAdaptiveMean] = nameof(ThresholdToolProperty.AdaptiveType),
                    [rbAdaptiveGaussian] = nameof(ThresholdToolProperty.AdaptiveType),
                    [rbAdaptiveBinary] = nameof(ThresholdToolProperty.AdaptiveThresholdType),
                    [rbAdaptiveInvert] = nameof(ThresholdToolProperty.AdaptiveThresholdType),
                    [txtAdaptiveMaxValue] = nameof(ThresholdToolProperty.MaxValue),
                    [txtBlockSize] = nameof(ThresholdToolProperty.BlockSize),
                    [sliderBlockSize] = nameof(ThresholdToolProperty.BlockSize),
                    [txtWeight] = nameof(ThresholdToolProperty.Weight)
                });

            ApplyLocalization();
            suppressEvents = false;
            parameterChangeController.RefreshProgrammatic(thresholdInteractionController.RefreshModePanels);
        }

        protected override void DisposeToolResources()
        {
            parameterGuideBinder.Dispose();
            learnWindowController.Dispose();
            signalInspector.MarkerValueChangeRequested -= SignalInspector_MarkerValueChangeRequested;
            thresholdInteractionController.Detach();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
            signalInspector.ApplyLocalization();
            btnCloseSignalInspector.Content =
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "매개변수로 돌아가기"
                    : "Back to parameters";
            btnOpenSignalInspector.Content =
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "분포 다시 보기"
                    : "Review distribution";
            bool korean = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean;
            thresholdSuggestionTitle.Text = korean
                ? "Threshold 티칭 제안"
                : "Threshold teaching suggestion";
            btnAnalyzeThresholdSuggestion.Content = korean ? "제안 분석" : "Analyze suggestion";
            btnUseThresholdSuggestion.Content = korean ? "T 사용" : "Use T";
            btnUndoThresholdSuggestion.Content = korean ? "이전 T 복원" : "Undo";
        }

        public ThresholdToolProperty CreateProperty()
        {
            thresholdInteractionController.FlushParameterBindings();
            return presenter.CreateProperty();
        }

        public void ConfigureBasicInvertForTest(bool invert)
        {
            thresholdInteractionController.ConfigureBasicInvertForTest(invert);
        }

        public void OpenThresholdGuideForTest()
        {
            learnWindowController.Open();
        }

        internal bool SignalInspectorHasEvidenceForTest => signalInspector.HasEvidence;

        internal string SignalInspectorEvidenceIdForTest => signalInspector.EvidenceId;

        internal string SignalInspectorSourceSha256ForTest => signalInspector.SourceSha256;

        internal int SignalInspectorSeriesCountForTest => signalInspector.SeriesCount;

        internal int SignalInspectorMarkerCountForTest => signalInspector.MarkerCount;

        internal int SignalInspectorAdvisoryMarkerCountForTest => signalInspector.AdvisoryMarkerCount;

        internal bool IsThresholdSuggestionPanelVisibleForTest =>
            thresholdSuggestionPanel.Visibility == Visibility.Visible;

        internal bool HasThresholdSuggestionForTest => thresholdSuggestion?.Accepted == true;

        internal int ThresholdSuggestionValueForTest => thresholdSuggestion?.Threshold ?? -1;

        internal string ThresholdSuggestionStatusForTest => thresholdSuggestionStatus.Text ?? string.Empty;

        internal string ThresholdSuggestionEvidenceIdForTest => thresholdSuggestion?.EvidenceId ?? string.Empty;

        internal bool CanUseThresholdSuggestionForTest => btnUseThresholdSuggestion.IsEnabled;

        internal bool CanUndoThresholdSuggestionForTest => btnUndoThresholdSuggestion.IsEnabled;

        internal bool IsSignalInspectorOverlayVisibleForTest =>
            signalInspectorOverlay.Visibility == Visibility.Visible;

        internal double GetSignalInspectorMarkerValueForTest(string markerId)
        {
            return signalInspector.GetMarkerValue(markerId);
        }

        internal void CommitSignalInspectorMarkerForTest(string markerId, double value)
        {
            signalInspector.CommitMarkerForTest(markerId, value);
        }

        internal void ExportSignalEvidenceForTest(string path)
        {
            signalInspector.ExportForTest(path);
        }

        internal void AnalyzeThresholdSuggestionForTest()
        {
            AnalyzeThresholdSuggestion();
        }

        internal void UseThresholdSuggestionForTest()
        {
            UseThresholdSuggestion();
        }

        internal void UndoThresholdSuggestionForTest()
        {
            UndoThresholdSuggestion();
        }

        internal void ShowSignalEvidence(VisionToolSignalEvidence evidence)
        {
            signalInspector.ShowEvidence(evidence);
            btnOpenSignalInspector.Visibility = Visibility.Visible;
            signalInspectorOverlay.Visibility = Visibility.Visible;
            UpdateThresholdSuggestionAvailability(evidence);
        }

        internal void ClearSignalEvidence()
        {
            signalInspector.ClearEvidence();
            thresholdSuggestion = null;
            thresholdSuggestionPanel.Visibility = Visibility.Collapsed;
            btnUseThresholdSuggestion.IsEnabled = false;
            btnOpenSignalInspector.Visibility = Visibility.Collapsed;
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void CloseSignalInspectorForTest()
        {
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        internal void OpenSignalInspectorForTest()
        {
            if (signalInspector.HasEvidence)
            {
                signalInspectorOverlay.Visibility = Visibility.Visible;
            }
        }

        private void OpenThresholdGuide_Click(object sender, RoutedEventArgs e)
        {
            learnWindowController.Open();
        }

        private void CloseSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            signalInspectorOverlay.Visibility = Visibility.Collapsed;
        }

        private void OpenSignalInspector_Click(object sender, RoutedEventArgs e)
        {
            OpenSignalInspectorForTest();
        }

        private void AnalyzeThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeThresholdSuggestion();
        }

        private void UseThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            UseThresholdSuggestion();
        }

        private void UndoThresholdSuggestion_Click(object sender, RoutedEventArgs e)
        {
            UndoThresholdSuggestion();
        }

        private void SignalInspector_MarkerValueChangeRequested(
            object sender,
            VisionToolSignalMarkerValueChangedEventArgs e)
        {
            thresholdInteractionController.ApplySignalMarkerValue(e.MarkerId, e.Value);
        }

        private void AnalyzeThresholdSuggestion()
        {
            VisionToolSignalEvidence evidence = signalInspector.CurrentEvidence;
            ThresholdToolProperty property = CreateProperty();
            if (evidence == null || property.Mode != ThresholdToolMode.Threshold)
            {
                thresholdSuggestion = null;
                signalInspector.SetAdvisoryMarkers();
                btnUseThresholdSuggestion.IsEnabled = false;
                thresholdSuggestionStatus.Text =
                    "Rejected: Threshold Basic and one current Preview histogram are required.";
                return;
            }

            thresholdSuggestion = VisionToolThresholdSuggestionAnalyzer.Analyze(
                evidence,
                property.ThresholdType != OpenCvSharp.ThresholdTypes.BinaryInv);
            thresholdSuggestionStatus.Text = thresholdSuggestion.Reason
                + Environment.NewLine
                + "Suggestion evidence "
                + ShortId(thresholdSuggestion.EvidenceId)
                + " / source "
                + ShortId(evidence.SourceSha256)
                + " / region "
                + evidence.RegionDescription;
            btnUseThresholdSuggestion.IsEnabled = thresholdSuggestion.Accepted;
            signalInspector.SetAdvisoryMarkers(
                thresholdSuggestion.Accepted
                    ? new VisionToolSignalMarker(
                        "ThresholdSuggestion",
                        property.ThresholdType == OpenCvSharp.ThresholdTypes.BinaryInv
                            ? "Dark candidate"
                            : "Bright candidate",
                        thresholdSuggestion.Threshold,
                        "#E67E22",
                        false)
                    : null);
        }

        private void UseThresholdSuggestion()
        {
            VisionToolSignalEvidence evidence = signalInspector.CurrentEvidence;
            ThresholdToolProperty property = CreateProperty();
            VisionToolThresholdSuggestion currentAnalysis =
                VisionToolThresholdSuggestionAnalyzer.Analyze(
                    evidence,
                    property.ThresholdType != OpenCvSharp.ThresholdTypes.BinaryInv);
            if (thresholdSuggestion?.Accepted != true
                || evidence == null
                || !string.Equals(
                    thresholdSuggestion.EvidenceId,
                    currentAnalysis.EvidenceId,
                    StringComparison.Ordinal)
                || property.Mode != ThresholdToolMode.Threshold)
            {
                throw new InvalidOperationException(
                    "The Threshold suggestion is stale or no longer matches the current Preview evidence.");
            }

            int previousThreshold = Math.Clamp((int)Math.Round(property.Threshold), 0, 255);
            if (previousThreshold == thresholdSuggestion.Threshold)
            {
                thresholdSuggestionStatus.Text =
                    $"T={thresholdSuggestion.Threshold} is already the current teaching value; no Preview was scheduled.";
                btnUseThresholdSuggestion.IsEnabled = false;
                return;
            }

            thresholdSuggestionUndo = new ThresholdSuggestionUndoState
            {
                SourceSha256 = evidence.SourceSha256,
                PreviousThreshold = previousThreshold,
                AppliedThreshold = thresholdSuggestion.Threshold
            };
            thresholdInteractionController.ApplySignalMarkerValue(
                OpenVisionNativeThresholdSignalEvidenceFactory.ThresholdMarkerId,
                thresholdSuggestion.Threshold);
        }

        private void UndoThresholdSuggestion()
        {
            ThresholdSuggestionUndoState undo = thresholdSuggestionUndo;
            ThresholdToolProperty property = CreateProperty();
            VisionToolSignalEvidence evidence = signalInspector.CurrentEvidence;
            if (undo == null
                || evidence == null
                || property.Mode != ThresholdToolMode.Threshold
                || Math.Abs(property.Threshold - undo.AppliedThreshold) > 0.001D
                || !string.Equals(evidence.SourceSha256, undo.SourceSha256, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "The previous Threshold teaching value is stale and cannot be restored.");
            }

            thresholdSuggestionUndo = null;
            thresholdInteractionController.ApplySignalMarkerValue(
                OpenVisionNativeThresholdSignalEvidenceFactory.ThresholdMarkerId,
                undo.PreviousThreshold);
        }

        private void UpdateThresholdSuggestionAvailability(VisionToolSignalEvidence evidence)
        {
            bool isBasic = evidence != null
                && string.Equals(
                    evidence.ToolIdentity,
                    "Threshold/" + ThresholdToolMode.Threshold,
                    StringComparison.Ordinal);
            thresholdSuggestionPanel.Visibility = isBasic ? Visibility.Visible : Visibility.Collapsed;
            thresholdSuggestion = null;
            signalInspector.SetAdvisoryMarkers();
            btnUseThresholdSuggestion.IsEnabled = false;
            if (!isBasic)
            {
                btnUndoThresholdSuggestion.IsEnabled = false;
                return;
            }

            ThresholdToolProperty property = CreateProperty();
            bool canUndo = thresholdSuggestionUndo != null
                && string.Equals(
                    evidence.SourceSha256,
                    thresholdSuggestionUndo.SourceSha256,
                    StringComparison.Ordinal)
                && Math.Abs(property.Threshold - thresholdSuggestionUndo.AppliedThreshold) <= 0.001D;
            if (!canUndo)
            {
                thresholdSuggestionUndo = null;
            }

            btnUndoThresholdSuggestion.IsEnabled = canUndo;
            thresholdSuggestionStatus.Text = canUndo
                ? $"Applied suggested T={thresholdSuggestionUndo.AppliedThreshold}. Previous T={thresholdSuggestionUndo.PreviousThreshold} remains recoverable with Undo."
                : "Analyze the current Preview full-image histogram. No teaching value changes until Use.";
        }

        private static string ShortId(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "-"
                : value.Substring(0, Math.Min(12, value.Length));
        }

        private void RefreshSummaryAndClearSignalEvidence()
        {
            UpdateSummary();
            ClearSignalEvidence();
        }

        private void UpdateSummary()
        {
            if (!HasToolController)
            {
                return;
            }

            thresholdInteractionController?.FlushParameterBindings();
            ToolController.RefreshSummaryBinding();
        }

        private sealed class ThresholdSuggestionUndoState
        {
            public string SourceSha256 { get; init; } = string.Empty;
            public int PreviousThreshold { get; init; }
            public int AppliedThreshold { get; init; }
        }

    }
}
