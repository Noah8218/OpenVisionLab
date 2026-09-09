using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionLearnWindow : Window
    {
        private const int ColorHsvAnimationStepCount = 4;
        private readonly DispatcherTimer colorHsvAnimationTimer;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
        private Action<string> openPracticeSamplesAction;
        private Action<VISION_MENU> openRelatedToolAction;
        private readonly OpenVisionLearnTopicPresentationPolicy topicPresentationPolicy = new OpenVisionLearnTopicPresentationPolicy();
        private int colorHsvAnimationStep = ColorHsvAnimationStepCount;

        public event EventHandler<OpenVisionLearnThresholdApplyEventArgs> ApplyThresholdRequested;

        public OpenVisionLearnWindow()
            : this(127, 255, false, 0)
        {
        }

        public OpenVisionLearnWindow(double threshold, double maxValue, bool invert)
            : this(threshold, maxValue, invert, 2)
        {
        }

        public OpenVisionLearnWindow(double threshold, double maxValue, bool invert, int selectedTopicIndex)
        {
            InitializeComponent();
            grayscaleLearnView.InitializeThreshold(threshold, maxValue, invert);
            grayscaleLearnView.ApplyThresholdRequested += OnGrayscaleThresholdApplied;
            grayscaleLearnView.CloseRequested += OnGrayscaleCloseRequested;
            grayscaleLearnView.ThresholdToolOpened += OnGrayscaleThresholdToolOpened;
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            colorHsvAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            colorHsvAnimationTimer.Tick += ColorHsvAnimationTimer_Tick;
            colorHueSlider.Value = 30;
            colorValueSlider.Value = 190;
            topicList.ItemsSource = OpenVisionLearnTopicCatalog.TopicTitles;
            topicList.SelectedIndex = NormalizeTopicIndex(selectedTopicIndex);
            grayscaleLearnView.UpdateGuide();
            grayscaleLearnView.UpdateBrightnessGuide();
            grayscaleLearnView.UpdateArithmeticGuide();
            grayscaleLearnView.UpdateFilterGuide();
            foundationLearnView.UpdateFoundationGuide();
            foundationLearnView.UpdateMatChannelGuide();
            binaryLearnView.UpdateMorphologyGuide();
            binaryLearnView.UpdateBlobGuide();
            binaryLearnView.UpdateContourGuide();
            lineLearnView.UpdateEdgeLineGuide();
            lineLearnView.UpdateLineDistanceGuide();
            metricsAcceptanceLearnView.RefreshFrame();
            geometryLearnView.UpdateGeometryGuide();
            UpdateColorHsvGuide();
            UpdateSelectedTopic();
        }

        public double ThresholdValueForTest
        {
            get => grayscaleLearnView.ThresholdValueForTest;
            set => grayscaleLearnView.ThresholdValueForTest = value;
        }

        public bool IsInvertedForTest
        {
            get => grayscaleLearnView.IsInvertedForTest;
            set => grayscaleLearnView.IsInvertedForTest = value;
        }

        public string FormulaTextForTest => grayscaleLearnView.FormulaTextForTest;

        public int SelectedTopicIndexForTest => topicList.SelectedIndex;

        public void SelectTopic(OpenVisionLearnTopicIndex topicIndex)
        {
            topicList.SelectedIndex = NormalizeTopicIndex((int)topicIndex);
            topicList.ScrollIntoView(topicList.SelectedItem);
        }

        public string SelectedTopicDocumentFileNameForTest => ResolveSelectedTopicDocumentFileName(topicList.SelectedIndex);

        public string SelectedTopicLearnPathIdForTest => ResolveSelectedTopicLearnPathId(topicList.SelectedIndex);

        public string SelectedTopicPracticeTextForTest => txtTopicPractice.Text ?? string.Empty;

        public bool IsPracticeWorkflowExpandedForTest => practiceWorkflowExpander.IsExpanded;

        public bool AreMatchingFamilyDecisionsCollapsedForTest => matchingLearnView.AreMatchingFamilyDecisionsCollapsedForTest;

        public bool CanOpenPracticeSamplesForTest => btnPracticeSamples.IsEnabled;

        public bool CanOpenRelatedToolsForTest => foundationLearnView.CanOpenRelatedToolsForTest;

        public bool CanOpenColorHsvToolForTest => btnColorHsvOpenTool.IsEnabled;

        public bool CanOpenThresholdToolForTest => grayscaleLearnView.CanOpenThresholdToolForTest;

        public bool CanOpenBrightnessToolsForTest => grayscaleLearnView.CanOpenBrightnessToolsForTest;

        public bool CanOpenArithmeticToolForTest => grayscaleLearnView.CanOpenArithmeticToolForTest;

        public bool CanOpenGeometryToolForTest => geometryLearnView.CanOpenGeometryToolForTest;

        public bool CanOpenAffineTransformToolForTest => geometryLearnView.CanOpenAffineTransformToolForTest;

        public bool CanOpenFilteringToolForTest => grayscaleLearnView.CanOpenFilteringToolForTest;

        public bool CanOpenMorphologyToolForTest => binaryLearnView.CanOpenMorphologyToolForTest;

        public bool CanOpenBlobToolForTest => binaryLearnView.CanOpenBlobToolForTest;

        public bool CanOpenContourToolForTest => binaryLearnView.CanOpenContourToolForTest;

        public bool CanOpenEdgeLineToolsForTest => lineLearnView.CanOpenEdgeLineToolsForTest;

        public bool CanOpenLineDistanceToolForTest => lineLearnView.CanOpenLineDistanceToolForTest;

        public bool CanOpenMatchingToolForTest => matchingLearnView.CanOpenMatchingToolForTest;

        public bool CanOpenEdgeBasedMatchingToolForTest => matchingLearnView.CanOpenEdgeBasedMatchingToolForTest;

        public bool CanOpenFeatureMatchingToolForTest => matchingLearnView.CanOpenFeatureMatchingToolForTest;

        public string FoundationToolLocationTitleForTest => foundationLearnView.FoundationToolLocationTitleForTest;

        public string FoundationToolLocationDetailForTest => foundationLearnView.FoundationToolLocationDetailForTest;

        public void BringFoundationToolLocationIntoViewForTest() => foundationLearnView.BringFoundationToolLocationIntoViewForTest();

        public string ColorHsvToolLocationTitleForTest => txtColorHsvToolLocationTitle.Text ?? string.Empty;

        public string ColorHsvToolLocationDetailForTest => txtColorHsvToolLocationDetail.Text ?? string.Empty;

        public string BrightnessToolLocationTitleForTest => grayscaleLearnView.BrightnessToolLocationTitleForTest;

        public string BrightnessToolLocationDetailForTest => grayscaleLearnView.BrightnessToolLocationDetailForTest;

        public string ArithmeticToolLocationTitleForTest => grayscaleLearnView.ArithmeticToolLocationTitleForTest;

        public string ArithmeticToolLocationDetailForTest => grayscaleLearnView.ArithmeticToolLocationDetailForTest;

        public string GeometryToolLocationTitleForTest => geometryLearnView.GeometryToolLocationTitleForTest;

        public string GeometryToolLocationDetailForTest => geometryLearnView.GeometryToolLocationDetailForTest;

        public string FilteringToolLocationTitleForTest => grayscaleLearnView.FilteringToolLocationTitleForTest;

        public string FilteringToolLocationDetailForTest => grayscaleLearnView.FilteringToolLocationDetailForTest;

        public string MorphologyToolLocationTitleForTest => binaryLearnView.MorphologyToolLocationTitleForTest;

        public string MorphologyToolLocationDetailForTest => binaryLearnView.MorphologyToolLocationDetailForTest;

        public string BlobToolLocationTitleForTest => binaryLearnView.BlobToolLocationTitleForTest;

        public string BlobToolLocationDetailForTest => binaryLearnView.BlobToolLocationDetailForTest;

        public string ContourToolLocationTitleForTest => binaryLearnView.ContourToolLocationTitleForTest;

        public string ContourToolLocationDetailForTest => binaryLearnView.ContourToolLocationDetailForTest;

        public string EdgeLineToolLocationTitleForTest => lineLearnView.EdgeLineToolLocationTitleForTest;

        public string EdgeLineToolLocationDetailForTest => lineLearnView.EdgeLineToolLocationDetailForTest;

        public string LineDistanceToolLocationTitleForTest => lineLearnView.LineDistanceToolLocationTitleForTest;

        public string LineDistanceToolLocationDetailForTest => lineLearnView.LineDistanceToolLocationDetailForTest;

        public string MatchingToolLocationTitleForTest => matchingLearnView.MatchingToolLocationTitleForTest;

        public string MatchingToolLocationDetailForTest => matchingLearnView.MatchingToolLocationDetailForTest;

        public string EdgeBasedMatchingToolLocationTitleForTest => matchingLearnView.EdgeBasedMatchingToolLocationTitleForTest;

        public string EdgeBasedMatchingToolLocationDetailForTest => matchingLearnView.EdgeBasedMatchingToolLocationDetailForTest;

        public string FeatureMatchingToolLocationTitleForTest => matchingLearnView.FeatureMatchingToolLocationTitleForTest;

        public string FeatureMatchingToolLocationDetailForTest => matchingLearnView.FeatureMatchingToolLocationDetailForTest;

        public void SetOpenPracticeSamplesAction(Action<string> action)
        {
            openPracticeSamplesAction = action;
            btnPracticeSamples.IsEnabled = action != null;
        }

        public void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            bool enabled = action != null;
            foundationLearnView.SetOpenRelatedToolAction(action);
            binaryLearnView.SetOpenRelatedToolAction(action);
            lineLearnView.SetOpenRelatedToolAction(action);
            matchingLearnView.SetOpenRelatedToolAction(action);
            geometryLearnView.SetOpenRelatedToolAction(action);
            btnColorHsvOpenTool.IsEnabled = enabled;
            grayscaleLearnView.SetOpenRelatedToolAction(action);
        }

        public double BrightnessOffsetForTest
        {
            get => grayscaleLearnView.BrightnessOffsetForTest;
            set => grayscaleLearnView.BrightnessOffsetForTest = value;
        }

        public string BrightnessFormulaTextForTest => grayscaleLearnView.BrightnessFormulaTextForTest;

        public int BrightnessAnimationStepForTest => grayscaleLearnView.BrightnessAnimationStepForTest;

        public string BrightnessAnimationStatusTextForTest => grayscaleLearnView.BrightnessAnimationStatusTextForTest;

        public void ResetBrightnessAnimationForTest() => grayscaleLearnView.ResetBrightnessAnimationForTest();

        public void AdvanceBrightnessAnimationForTest() => grayscaleLearnView.AdvanceBrightnessAnimationForTest();

        public void ToggleBrightnessAnimationForTest() => grayscaleLearnView.ToggleBrightnessAnimationForTest();

        public int ArithmeticModeIndexForTest
        {
            get => grayscaleLearnView.ArithmeticModeIndexForTest;
            set => grayscaleLearnView.ArithmeticModeIndexForTest = value;
        }

        public string ArithmeticFormulaTextForTest => grayscaleLearnView.ArithmeticFormulaTextForTest;

        public int ArithmeticAnimationStepForTest => grayscaleLearnView.ArithmeticAnimationStepForTest;

        public string ArithmeticAnimationStatusTextForTest => grayscaleLearnView.ArithmeticAnimationStatusTextForTest;

        public void ResetArithmeticAnimationForTest() => grayscaleLearnView.ResetArithmeticAnimationForTest();

        public void AdvanceArithmeticAnimationForTest() => grayscaleLearnView.AdvanceArithmeticAnimationForTest();

        public void ToggleArithmeticAnimationForTest() => grayscaleLearnView.ToggleArithmeticAnimationForTest();

        public int FilterModeIndexForTest
        {
            get => grayscaleLearnView.FilterModeIndexForTest;
            set => grayscaleLearnView.FilterModeIndexForTest = value;
        }

        public string FilterFormulaTextForTest => grayscaleLearnView.FilterFormulaTextForTest;

        public int FilterAnimationStepForTest => grayscaleLearnView.FilterAnimationStepForTest;

        public string FilterAnimationStatusTextForTest => grayscaleLearnView.FilterAnimationStatusTextForTest;

        public void ResetFilterAnimationForTest() => grayscaleLearnView.ResetFilterAnimationForTest();

        public void AdvanceFilterAnimationForTest() => grayscaleLearnView.AdvanceFilterAnimationForTest();

        public void ToggleFilterAnimationForTest() => grayscaleLearnView.ToggleFilterAnimationForTest();

        public int FoundationAnimationStepForTest => foundationLearnView.FoundationAnimationStepForTest;

        public int FoundationSelectedCellCountForTest => foundationLearnView.FoundationSelectedCellCountForTest;

        public string FoundationAnimationStatusTextForTest => foundationLearnView.FoundationAnimationStatusTextForTest;

        public bool IsFoundationPointVisibleForTest => foundationLearnView.IsFoundationPointVisibleForTest;

        public bool IsFoundationRectVisibleForTest => foundationLearnView.IsFoundationRectVisibleForTest;

        public bool IsFoundationRotatedRectVisibleForTest => foundationLearnView.IsFoundationRotatedRectVisibleForTest;

        public bool IsFoundationRotatedBoundsVisibleForTest => foundationLearnView.IsFoundationRotatedBoundsVisibleForTest;

        public bool IsFoundationRotatedCenterVisibleForTest => foundationLearnView.IsFoundationRotatedCenterVisibleForTest;

        public double FoundationRotatedRectAngleForTest => foundationLearnView.FoundationRotatedRectAngleForTest;

        public void ResetFoundationAnimationForTest() => foundationLearnView.ResetFoundationAnimationForTest();

        public void AdvanceFoundationAnimationForTest() => foundationLearnView.AdvanceFoundationAnimationForTest();

        public void ToggleFoundationAnimationForTest() => foundationLearnView.ToggleFoundationAnimationForTest();

        public int MatChannelAnimationStepForTest => foundationLearnView.MatChannelAnimationStepForTest;

        public string MatChannelAnimationStatusTextForTest => foundationLearnView.MatChannelAnimationStatusTextForTest;

        public double MatChannelSplitOpacityForTest => foundationLearnView.MatChannelSplitOpacityForTest;

        public double MatChannelGrayOpacityForTest => foundationLearnView.MatChannelGrayOpacityForTest;

        public string MatChannelBgrShapeTextForTest => foundationLearnView.MatChannelBgrShapeTextForTest;

        public string MatChannelGrayShapeTextForTest => foundationLearnView.MatChannelGrayShapeTextForTest;

        public double MatChannelTypeGuideOpacityForTest => foundationLearnView.MatChannelTypeGuideOpacityForTest;

        public string MatChannelTypeTitleForTest => foundationLearnView.MatChannelTypeTitleForTest;

        public string MatChannelTypeDetailForTest => foundationLearnView.MatChannelTypeDetailForTest;

        public void ResetMatChannelAnimationForTest() => foundationLearnView.ResetMatChannelAnimationForTest();

        public void AdvanceMatChannelAnimationForTest() => foundationLearnView.AdvanceMatChannelAnimationForTest();

        public void ToggleMatChannelAnimationForTest() => foundationLearnView.ToggleMatChannelAnimationForTest();

        public int MorphologyModeIndexForTest
        {
            get => binaryLearnView.MorphologyModeIndexForTest;
            set => binaryLearnView.MorphologyModeIndexForTest = value;
        }

        public string MorphologyFormulaTextForTest => binaryLearnView.MorphologyFormulaTextForTest;

        public int MorphologyAnimationStepForTest => binaryLearnView.MorphologyAnimationStepForTest;

        public string MorphologyAnimationStatusTextForTest => binaryLearnView.MorphologyAnimationStatusTextForTest;

        public void ResetMorphologyAnimationForTest() => binaryLearnView.ResetMorphologyAnimationForTest();

        public void AdvanceMorphologyAnimationForTest() => binaryLearnView.AdvanceMorphologyAnimationForTest();

        public void ToggleMorphologyAnimationForTest() => binaryLearnView.ToggleMorphologyAnimationForTest();

        public double BlobMinAreaForTest
        {
            get => binaryLearnView.BlobMinAreaForTest;
            set => binaryLearnView.BlobMinAreaForTest = value;
        }

        public string BlobFormulaTextForTest => binaryLearnView.BlobFormulaTextForTest;

        public int BlobAnimationStepForTest => binaryLearnView.BlobAnimationStepForTest;

        public string BlobAnimationStatusTextForTest => binaryLearnView.BlobAnimationStatusTextForTest;

        public void ResetBlobAnimationForTest() => binaryLearnView.ResetBlobAnimationForTest();

        public void AdvanceBlobAnimationForTest() => binaryLearnView.AdvanceBlobAnimationForTest();

        public void ToggleBlobAnimationForTest() => binaryLearnView.ToggleBlobAnimationForTest();

        public int ContourDrawModeIndexForTest
        {
            get => binaryLearnView.ContourDrawModeIndexForTest;
            set => binaryLearnView.ContourDrawModeIndexForTest = value;
        }

        public string ContourFormulaTextForTest => binaryLearnView.ContourFormulaTextForTest;

        public int ContourAnimationStepForTest => binaryLearnView.ContourAnimationStepForTest;

        public string ContourAnimationStatusTextForTest => binaryLearnView.ContourAnimationStatusTextForTest;

        public void ResetContourAnimationForTest() => binaryLearnView.ResetContourAnimationForTest();

        public void AdvanceContourAnimationForTest() => binaryLearnView.AdvanceContourAnimationForTest();

        public void ToggleContourAnimationForTest() => binaryLearnView.ToggleContourAnimationForTest();

        public double EdgeThresholdForTest
        {
            get => lineLearnView.EdgeThresholdForTest;
            set => lineLearnView.EdgeThresholdForTest = value;
        }

        public string EdgeLineFormulaTextForTest => lineLearnView.EdgeLineFormulaTextForTest;

        public int EdgeLineAnimationStepForTest => lineLearnView.EdgeLineAnimationStepForTest;

        public string EdgeLineAnimationStatusTextForTest => lineLearnView.EdgeLineAnimationStatusTextForTest;

        public void ResetEdgeLineAnimationForTest() => lineLearnView.ResetEdgeLineAnimationForTest();

        public void AdvanceEdgeLineAnimationForTest() => lineLearnView.AdvanceEdgeLineAnimationForTest();

        public void ToggleEdgeLineAnimationForTest() => lineLearnView.ToggleEdgeLineAnimationForTest();

        public double LineDistanceRangeMaxForTest
        {
            get => lineLearnView.LineDistanceRangeMaxForTest;
            set => lineLearnView.LineDistanceRangeMaxForTest = value;
        }

        public string LineDistanceFormulaTextForTest => lineLearnView.LineDistanceFormulaTextForTest;

        public int LineDistanceAnimationStepForTest => lineLearnView.LineDistanceAnimationStepForTest;

        public string LineDistanceAnimationStatusTextForTest => lineLearnView.LineDistanceAnimationStatusTextForTest;

        public void ResetLineDistanceAnimationForTest() => lineLearnView.ResetLineDistanceAnimationForTest();

        public void AdvanceLineDistanceAnimationForTest() => lineLearnView.AdvanceLineDistanceAnimationForTest();

        public void ToggleLineDistanceAnimationForTest() => lineLearnView.ToggleLineDistanceAnimationForTest();

        public double MatchingThresholdForTest
        {
            get => matchingLearnView.MatchingThresholdForTest;
            set => matchingLearnView.MatchingThresholdForTest = value;
        }

        public string MatchingFormulaTextForTest => matchingLearnView.MatchingFormulaTextForTest;

        public int MatchingAnimationStepForTest => matchingLearnView.MatchingAnimationStepForTest;

        public string MatchingAnimationStatusTextForTest => matchingLearnView.MatchingAnimationStatusTextForTest;

        public void ResetMatchingAnimationForTest()
        {
            matchingLearnView.ResetMatchingAnimationForTest();
        }

        public void AdvanceMatchingAnimationForTest()
        {
            matchingLearnView.AdvanceMatchingAnimationForTest();
        }

        public void ToggleMatchingAnimationForTest()
        {
            matchingLearnView.ToggleMatchingAnimationForTest();
        }

        public double FeatureGoodMatchMinForTest
        {
            get => matchingLearnView.FeatureGoodMatchMinForTest;
            set => matchingLearnView.FeatureGoodMatchMinForTest = value;
        }

        public string FeatureMatchingFormulaTextForTest => matchingLearnView.FeatureMatchingFormulaTextForTest;

        public int FeatureMatchingAnimationStepForTest => matchingLearnView.FeatureMatchingAnimationStepForTest;

        public string FeatureMatchingAnimationStatusTextForTest => matchingLearnView.FeatureMatchingAnimationStatusTextForTest;

        public void ResetFeatureMatchingAnimationForTest()
        {
            matchingLearnView.ResetFeatureMatchingAnimationForTest();
        }

        public void AdvanceFeatureMatchingAnimationForTest()
        {
            matchingLearnView.AdvanceFeatureMatchingAnimationForTest();
        }

        public void ToggleFeatureMatchingAnimationForTest()
        {
            matchingLearnView.ToggleFeatureMatchingAnimationForTest();
        }

        public int MetricsAcceptanceAnimationStepForTest => metricsAcceptanceLearnView.AnimationStepForTest;

        public string MetricsAcceptanceAnimationStatusTextForTest => metricsAcceptanceLearnView.AnimationStatusTextForTest;

        public string MetricsAcceptanceFormulaTextForTest => metricsAcceptanceLearnView.FormulaTextForTest;

        public bool IsMetricGateCheatSheetExpandedForTest => metricsAcceptanceLearnView.IsCheatSheetExpandedForTest;

        public bool IsAnimationLegendVisibleForTest => animationLegendPanel.Visibility == Visibility.Visible;

        public string AnimationLegendColorsForTest => string.Join(
            ",",
            ((SolidColorBrush)animationNeutralBrush).Color,
            ((SolidColorBrush)animationCandidateBrush).Color,
            ((SolidColorBrush)animationPassBrush).Color,
            ((SolidColorBrush)animationWarningBrush).Color);

        public void ResetMetricsAcceptanceAnimationForTest()
        {
            metricsAcceptanceLearnView.ResetAnimation();
        }

        public void AdvanceMetricsAcceptanceAnimationForTest()
        {
            metricsAcceptanceLearnView.AdvanceAnimation();
        }

        public void ToggleMetricsAcceptanceAnimationForTest()
        {
            metricsAcceptanceLearnView.ToggleAnimation();
        }

        public double LayerRecipeSelectedStepForTest
        {
            get => layerRecipeLearnView.LayerRecipeSelectedStepForTest;
            set => layerRecipeLearnView.LayerRecipeSelectedStepForTest = value;
        }

        public string LayerRecipeFormulaTextForTest => layerRecipeLearnView.LayerRecipeFormulaTextForTest;

        public int LayerRecipeAnimationStepForTest => layerRecipeLearnView.LayerRecipeAnimationStepForTest;

        public string LayerRecipeAnimationStatusTextForTest => layerRecipeLearnView.LayerRecipeAnimationStatusTextForTest;

        public void ResetLayerRecipeAnimationForTest()
        {
            layerRecipeLearnView.ResetLayerRecipeAnimationForTest();
        }

        public void AdvanceLayerRecipeAnimationForTest()
        {
            layerRecipeLearnView.AdvanceLayerRecipeAnimationForTest();
        }

        public void ToggleLayerRecipeAnimationForTest()
        {
            layerRecipeLearnView.ToggleLayerRecipeAnimationForTest();
        }

        public double GeometryAngleForTest
        {
            get => geometryLearnView.GeometryAngleForTest;
            set => geometryLearnView.GeometryAngleForTest = value;
        }

        public double GeometryScaleForTest
        {
            get => geometryLearnView.GeometryScaleForTest;
            set => geometryLearnView.GeometryScaleForTest = value;
        }

        public string GeometryFormulaTextForTest => geometryLearnView.GeometryFormulaTextForTest;

        public int GeometryAnimationStepForTest => geometryLearnView.GeometryAnimationStepForTest;

        public string GeometryAnimationStatusTextForTest => geometryLearnView.GeometryAnimationStatusTextForTest;

        public double GeometryRenderedAngleForTest => geometryLearnView.GeometryRenderedAngleForTest;

        public double GeometryRenderedScaleForTest => geometryLearnView.GeometryRenderedScaleForTest;

        public void ResetGeometryAnimationForTest() => geometryLearnView.ResetGeometryAnimationForTest();

        public void AdvanceGeometryAnimationForTest() => geometryLearnView.AdvanceGeometryAnimationForTest();

        public void ToggleGeometryAnimationForTest() => geometryLearnView.ToggleGeometryAnimationForTest();

        public double ColorHueForTest
        {
            get => colorHueSlider.Value;
            set => colorHueSlider.Value = Math.Max(0, Math.Min(179, value));
        }

        public double ColorValueForTest
        {
            get => colorValueSlider.Value;
            set => colorValueSlider.Value = Math.Max(40, Math.Min(255, value));
        }

        public string ColorHsvFormulaTextForTest => txtColorHsvFormula.Text ?? string.Empty;

        public int ColorHsvAnimationStepForTest => colorHsvAnimationStep;

        public string ColorHsvAnimationStatusTextForTest => txtColorHsvAnimationStatus.Text ?? string.Empty;

        public string ColorHsvPreviewLabelForTest => txtColorHsvPreviewLabel.Text ?? string.Empty;

        public string ColorHsvVec3bTypeTextForTest => txtColorHsvVec3bType.Text ?? string.Empty;

        public string ColorHsvScalarBoundsTextForTest => txtColorHsvScalarBounds.Text ?? string.Empty;

        public string ColorHsvInRangeTextForTest => txtColorHsvInRange.Text ?? string.Empty;

        public double ColorHsvVec3bTypeOpacityForTest => txtColorHsvVec3bType.Opacity;

        public double ColorHsvScalarBoundsOpacityForTest => txtColorHsvScalarBounds.Opacity;

        public string ColorBgrPixelValueForTest => txtColorBgrPixelValue.Text ?? string.Empty;

        public string ColorHsvPixelValueForTest => txtColorHsvPixelValue.Text ?? string.Empty;

        public double ColorHsvConvertedPixelOpacityForTest => colorHsvPixelCard.Opacity;

        public double ColorHsvConversionArrowOpacityForTest => colorHsvConversionArrow.Opacity;

        public double ColorBgrSplitChannelsOpacityForTest => colorBgrSplitChannels.Opacity;

        public double ColorBgrMergeResultOpacityForTest => colorBgrMergeResult.Opacity;

        public string ColorBgrMergeResultTextForTest => txtColorBgrMergeResult.Text ?? string.Empty;

        public void ResetColorHsvAnimationForTest()
        {
            ResetColorHsvAnimation();
        }

        public void AdvanceColorHsvAnimationForTest()
        {
            AdvanceColorHsvAnimation();
        }

        public void ToggleColorHsvAnimationForTest()
        {
            ColorHsvPlayButton_Click(this, new RoutedEventArgs());
        }

        public void ApplyForTest() => grayscaleLearnView.ApplyForTest();

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (System.Windows.Input.Keyboard.FocusedElement == null
                || ReferenceEquals(System.Windows.Input.Keyboard.FocusedElement, this))
            {
                if (topicList.ItemContainerGenerator.ContainerFromIndex(topicList.SelectedIndex) is ListBoxItem selectedItem)
                {
                    selectedItem.Focus();
                }
                else
                {
                    topicList.Focus();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            grayscaleLearnView.StopAnimations();
            grayscaleLearnView.ApplyThresholdRequested -= OnGrayscaleThresholdApplied;
            grayscaleLearnView.CloseRequested -= OnGrayscaleCloseRequested;
            grayscaleLearnView.ThresholdToolOpened -= OnGrayscaleThresholdToolOpened;
            colorHsvAnimationTimer.Stop();
            foundationLearnView.StopAnimations();
            geometryLearnView.StopAnimations();
            binaryLearnView.StopAnimations();
            lineLearnView.StopAnimations();
            matchingLearnView.StopAnimations();
            metricsAcceptanceLearnView.StopAnimation();
            layerRecipeLearnView.StopAnimation();
            base.OnClosed(e);
        }

        private void UpdateColorHsvGuide()
        {
            int hue = Math.Max(0, Math.Min(179, (int)Math.Round(colorHueSlider.Value)));
            int value = Math.Max(40, Math.Min(255, (int)Math.Round(colorValueSlider.Value)));
            int hueMin = Math.Max(0, hue - 10);
            int hueMax = Math.Min(179, hue + 10);
            const int saturationMinimum = 60;

            txtColorHsvHue.Text = hue.ToString(CultureInfo.InvariantCulture) + " / 179";
            txtColorHsvValue.Text = value.ToString(CultureInfo.InvariantCulture) + " / 255";
            txtColorHsvFormula.Text = "HSV mask: H="
                + hueMin.ToString(CultureInfo.InvariantCulture)
                + ".."
                + hueMax.ToString(CultureInfo.InvariantCulture)
                + ", S>="
                + saturationMinimum.ToString(CultureInfo.InvariantCulture)
                + ", V>="
                + value.ToString(CultureInfo.InvariantCulture)
                + " -> OutputLayer=HSV_Mask, metric=MaskPixelRatio 또는 후속 ResultCount/Area";
            txtColorHsvMeaning.Text = value < 110
                ? "Value가 낮으면 색이 어둡습니다. Hue만으로 영역이 불안정하면 Mean/Histogram의 밝기 분포도 함께 확인하세요."
                : "Hue는 색상 계열, Saturation은 회색 배경과의 차이, Value는 어두운 픽셀을 구분하는 기준입니다.";
            txtColorHsvVec3bType.Text = "변환된 HSV Mat 픽셀 = Vec3b(H,S,V) = (45,221,185): 0~255의 8비트 채널 값 3개";
            txtColorHsvScalarBounds.Text = "lower = Scalar("
                + hueMin.ToString(CultureInfo.InvariantCulture)
                + ",60,"
                + value.ToString(CultureInfo.InvariantCulture)
                + ") | upper = Scalar("
                + hueMax.ToString(CultureInfo.InvariantCulture)
                + ",255,255); Scalar는 값 4개를 담고 HSV에서는 앞의 3개를 사용합니다.";

            PaintColorHsvAnimationFrame(hueMin, hueMax, saturationMinimum, value);
        }

        private void PaintColorHsvAnimationFrame(int hueMin, int hueMax, int saturationMinimum, int valueMinimum)
        {
            const int sampleHue = 45;
            const int sampleSaturation = 221;
            const int sampleValue = 185;
            int visibleStep = Math.Max(0, Math.Min(colorHsvAnimationStep, ColorHsvAnimationStepCount));
            bool hueOk = sampleHue >= hueMin && sampleHue <= hueMax;
            bool saturationOk = sampleSaturation >= saturationMinimum;
            bool valueOk = sampleValue >= valueMinimum;
            bool maskPass = hueOk && saturationOk && valueOk;
            Color sampleColor = CreateColorFromOpenCvHsv(sampleHue, sampleSaturation, sampleValue);

            Border[] channels = { colorHueChannel, colorSaturationChannel, colorValueChannel };
            bool[] channelPass = { hueOk, saturationOk, valueOk };
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i].BorderBrush = visibleStep == 2
                    ? animationCandidateBrush
                    : visibleStep >= 3 ? channelPass[i] ? animationPassBrush : animationWarningBrush : Brushes.Transparent;
                channels[i].BorderThickness = visibleStep >= 2 ? new Thickness(2) : new Thickness(1);
            }

            colorHsvPreviewSwatch.BorderBrush = visibleStep >= 4
                ? maskPass ? animationPassBrush : animationWarningBrush
                : new SolidColorBrush(Color.FromRgb(203, 213, 225));
            colorHsvPreviewSwatch.BorderThickness = visibleStep >= 4 ? new Thickness(2) : new Thickness(1);
            colorHsvPreviewSwatch.Background = visibleStep >= 4
                ? maskPass ? Brushes.White : Brushes.Black
                : new SolidColorBrush(sampleColor);
            txtColorHsvPreviewLabel.Text = visibleStep >= 4
                ? "MASK " + (maskPass ? "255" : "0")
                : "Sample H 45 / S 221 / V 185";
            txtColorHsvPreviewLabel.Foreground = visibleStep >= 4
                ? maskPass ? Brushes.Black : Brushes.White
                : sampleValue < 150 ? Brushes.White : Brushes.Black;
            Brush defaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
            colorBgrPixelCard.BorderBrush = visibleStep == 0 ? animationCandidateBrush : defaultBorderBrush;
            colorHsvPixelCard.BorderBrush = visibleStep == 2 ? animationPassBrush : defaultBorderBrush;
            colorBgrMergeResult.BorderBrush = visibleStep == 2 ? animationPassBrush : new SolidColorBrush(Color.FromRgb(165, 212, 220));
            colorBgrPixelCard.Opacity = 1D;
            txtColorBgrSplitTitle.Opacity = visibleStep >= 1 ? 1D : 0.28D;
            colorBgrSplitChannels.Opacity = visibleStep >= 1 ? 1D : 0.28D;
            colorBgrMergeResult.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            colorHsvConversionArrow.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            colorHsvPixelCard.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            txtColorHsvVec3bType.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            txtColorHsvScalarBounds.Opacity = visibleStep >= 3 ? 1D : 0.28D;
            txtColorHsvInRange.Opacity = visibleStep >= 3 ? 1D : 0.28D;

            txtColorHsvAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 4 - BGR 입력: Vec3b(B,G,R)=(25,185,105) 픽셀부터 확인합니다.",
                1 => "1 / 4 - Cv2.Split: B=25, G=185, R=105인 CV_8UC1 채널 Mat 3개로 분리합니다.",
                2 => "2 / 4 - Cv2.Merge로 BGR을 복원하고 Cv2.CvtColor(BGR2HSV)로 H=45, S=221, V=185를 얻습니다.",
                3 => "3 / 4 - 범위 판정: H "
                    + (hueOk ? "OK" : "NG")
                    + ", S "
                    + (saturationOk ? "OK" : "NG")
                    + ", V "
                    + (valueOk ? "OK" : "NG")
                    + " -> "
                    + (maskPass ? "IN RANGE" : "OUT OF RANGE"),
                _ => "4 / 4 - Mask="
                    + (maskPass ? "255" : "0")
                    + ": MaskPixelRatio와 후속 ResultCount/Area를 Preview/Run 후 검토합니다."
            };
        }

        private void ResetColorHsvAnimation()
        {
            colorHsvAnimationTimer.Stop();
            btnColorHsvPlay.Content = "Play";
            colorHsvAnimationStep = 0;
            UpdateColorHsvGuide();
        }

        private void AdvanceColorHsvAnimation()
        {
            if (colorHsvAnimationStep >= ColorHsvAnimationStepCount)
            {
                colorHsvAnimationStep = 0;
            }

            colorHsvAnimationStep++;
            UpdateColorHsvGuide();
            if (colorHsvAnimationStep >= ColorHsvAnimationStepCount)
            {
                colorHsvAnimationTimer.Stop();
                btnColorHsvPlay.Content = "Play";
            }
        }

        private void TopicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                var previousFocus = System.Windows.Input.Keyboard.FocusedElement as UIElement;
                UpdateSelectedTopic();
                if (previousFocus != null && !previousFocus.IsVisible)
                {
                    topicList.Focus();
                }
            }
        }

        private void UpdateSelectedTopic()
        {
            OpenVisionLearnTopicPresentation presentation = topicPresentationPolicy.Resolve(topicList.SelectedIndex);
            int topicIndex = (int)presentation.TopicIndex;
            txtTopicTitle.Text = presentation.Title;
            foundationLearnView.SelectTopic(topicIndex);
            grayscaleLearnView.SelectTopic(topicIndex);
            geometryLearnView.SelectTopic(topicIndex);
            colorHsvTopicPanel.Visibility = presentation.ShowColorHsvTopic ? Visibility.Visible : Visibility.Collapsed;
            binaryLearnView.SelectTopic(topicIndex);
            lineLearnView.SelectTopic(topicIndex);
            matchingLearnView.SelectTopic(topicIndex);
            layerRecipeLearnView.Visibility = presentation.ShowLayerRecipeTopic ? Visibility.Visible : Visibility.Collapsed;
            metricsAcceptanceLearnView.Visibility = presentation.ShowMetricsAcceptanceTopic ? Visibility.Visible : Visibility.Collapsed;
            animationLegendPanel.Visibility = presentation.ShowAnimationLegend ? Visibility.Visible : Visibility.Collapsed;
            practiceWorkflowExpander.IsExpanded = presentation.IsPracticeWorkflowExpanded;
            txtTopicSubtitle.Text = presentation.Subtitle;
            txtTopicPractice.Text = presentation.PracticeText;
            ApplyTopicGuideUpdates(presentation.GuideUpdates);
        }

        private void ApplyTopicGuideUpdates(OpenVisionLearnTopicGuideUpdates updates)
        {
            if ((updates & OpenVisionLearnTopicGuideUpdates.Foundation) != 0)
            {
                foundationLearnView.UpdateFoundationGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.MatChannel) != 0)
            {
                foundationLearnView.UpdateMatChannelGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Brightness) != 0)
            {
                grayscaleLearnView.UpdateBrightnessGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Filtering) != 0)
            {
                grayscaleLearnView.UpdateFilterGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Morphology) != 0)
            {
                binaryLearnView.UpdateMorphologyGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Blob) != 0)
            {
                binaryLearnView.UpdateBlobGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Contour) != 0)
            {
                binaryLearnView.UpdateContourGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.EdgeLine) != 0)
            {
                lineLearnView.UpdateEdgeLineGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.LineDistance) != 0)
            {
                lineLearnView.UpdateLineDistanceGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.LayerRecipe) != 0)
            {
                layerRecipeLearnView.RefreshSelection();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.MetricsAcceptance) != 0)
            {
                metricsAcceptanceLearnView.RefreshFrame();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Arithmetic) != 0)
            {
                grayscaleLearnView.UpdateArithmeticGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.Geometry) != 0)
            {
                geometryLearnView.UpdateGeometryGuide();
            }

            if ((updates & OpenVisionLearnTopicGuideUpdates.ColorHsv) != 0)
            {
                UpdateColorHsvGuide();
            }
        }

        private void ColorHsvAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceColorHsvAnimation();
        }

        private void ColorHsvSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                colorHsvAnimationTimer.Stop();
                btnColorHsvPlay.Content = "Play";
                colorHsvAnimationStep = ColorHsvAnimationStepCount;
                UpdateColorHsvGuide();
            }
        }

        private void ColorHsvPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (colorHsvAnimationTimer.IsEnabled)
            {
                colorHsvAnimationTimer.Stop();
                btnColorHsvPlay.Content = "Play";
                return;
            }

            if (colorHsvAnimationStep >= ColorHsvAnimationStepCount)
            {
                ResetColorHsvAnimation();
            }

            btnColorHsvPlay.Content = "Pause";
            colorHsvAnimationTimer.Start();
        }

        private void ColorHsvStepButton_Click(object sender, RoutedEventArgs e)
        {
            colorHsvAnimationTimer.Stop();
            btnColorHsvPlay.Content = "Play";
            AdvanceColorHsvAnimation();
        }

        private void ColorHsvResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetColorHsvAnimation();
        }

        private void OpenLearnDocsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile(ResolveSelectedTopicDocumentFileName(topicList.SelectedIndex));
        }

        private void OpenFoundationDocsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile("LEARN_OPENCVSHARP_FOUNDATIONS.md");
        }

        private void OpenPracticeSamplesButton_Click(object sender, RoutedEventArgs e)
        {
            openPracticeSamplesAction?.Invoke(ResolveSelectedTopicLearnPathId(topicList.SelectedIndex));
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                openRelatedToolAction?.Invoke(menu);
                if (menu == VISION_MENU.HSV)
                {
                    txtColorHsvToolLocationTitle.Text =
                        "열림: HSV | 찾을 위치: Hue Min/Max, Saturation Min/Max, Value Min/Max, ROI, OutputLayer";
                    txtColorHsvToolLocationDetail.Text =
                        "HSV에서 색 범위와 ROI를 설정하고 Preview 또는 Run Review에서 선택된 색 영역과 결과 레이어를 확인하세요.";
                    return;
                }

                foundationLearnView.UpdateToolLocation(menu);
            }
        }

        private void OnGrayscaleThresholdApplied(object sender, OpenVisionLearnThresholdApplyEventArgs e)
        {
            // Preserve the public Window event sender for existing Tool Learn controllers.
            ApplyThresholdRequested?.Invoke(this, e);
        }

        private void OnGrayscaleCloseRequested(object sender, EventArgs e) => Close();

        private void OnGrayscaleThresholdToolOpened(object sender, EventArgs e)
        {
            foundationLearnView.UpdateToolLocation(VISION_MENU.Threshold);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static Color CreateColorFromOpenCvHsv(int hueOpenCv, int saturation, int value)
        {
            double hue = Math.Max(0, Math.Min(179, hueOpenCv)) * 2D;
            double sat = Math.Max(0, Math.Min(255, saturation)) / 255D;
            double val = Math.Max(0, Math.Min(255, value)) / 255D;
            double chroma = val * sat;
            double x = chroma * (1D - Math.Abs((hue / 60D) % 2D - 1D));
            double m = val - chroma;
            double red;
            double green;
            double blue;

            if (hue < 60D)
            {
                red = chroma;
                green = x;
                blue = 0D;
            }
            else if (hue < 120D)
            {
                red = x;
                green = chroma;
                blue = 0D;
            }
            else if (hue < 180D)
            {
                red = 0D;
                green = chroma;
                blue = x;
            }
            else if (hue < 240D)
            {
                red = 0D;
                green = x;
                blue = chroma;
            }
            else if (hue < 300D)
            {
                red = x;
                green = 0D;
                blue = chroma;
            }
            else
            {
                red = chroma;
                green = 0D;
                blue = x;
            }

            return Color.FromRgb(
                (byte)ClampToByte((red + m) * 255D),
                (byte)ClampToByte((green + m) * 255D),
                (byte)ClampToByte((blue + m) * 255D));
        }

        private static int ClampToByte(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return 0;
            }

            return Math.Max(0, Math.Min(255, (int)Math.Round(value)));
        }

        private static int NormalizeTopicIndex(int index)
        {
            return OpenVisionLearnTopicCatalog.NormalizeTopicIndex(index);
        }

        private static string ResolveSelectedTopicDocumentFileName(int index)
        {
            return OpenVisionLearnTopicCatalog.Resolve(index).Document;
        }

        private static string ResolveSelectedTopicLearnPathId(int index)
        {
            return OpenVisionLearnTopicCatalog.Resolve(index).PracticePathId;
        }

    }

    public sealed class OpenVisionLearnThresholdApplyEventArgs : EventArgs
    {
        public OpenVisionLearnThresholdApplyEventArgs(int threshold, bool invert)
        {
            Threshold = threshold;
            Invert = invert;
        }

        public int Threshold { get; }

        public bool Invert { get; }
    }
}
