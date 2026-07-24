using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionLearnWindow : Window
    {
        private const int ArithmeticAnimationStepCount = 3;
        private const int BrightnessAnimationStepCount = 3;
        private const int ColorHsvAnimationStepCount = 4;
        private const int FilterAnimationStepCount = 3;
        private const int FoundationAnimationStepCount = 5;
        private const int MatChannelAnimationStepCount = 4;
        private const int GeometryAnimationStepCount = 3;
        private const int ContourAnimationStepCount = 3;
        private const int EdgeLineAnimationStepCount = 3;
        private const int LineDistanceAnimationStepCount = 3;
        private const int MatchingAnimationStepCount = 3;
        private const int FeatureMatchingAnimationStepCount = 3;
        private const int MetricsAcceptanceAnimationStepCount = 3;
        private const int LayerRecipeAnimationStepCount = 4;
        private readonly DispatcherTimer animationTimer;
        private readonly DispatcherTimer arithmeticAnimationTimer;
        private readonly DispatcherTimer brightnessAnimationTimer;
        private readonly DispatcherTimer colorHsvAnimationTimer;
        private readonly DispatcherTimer filterAnimationTimer;
        private readonly DispatcherTimer foundationAnimationTimer;
        private readonly DispatcherTimer matChannelAnimationTimer;
        private readonly DispatcherTimer geometryAnimationTimer;
        private readonly DispatcherTimer morphologyAnimationTimer;
        private readonly DispatcherTimer blobAnimationTimer;
        private readonly DispatcherTimer contourAnimationTimer;
        private readonly DispatcherTimer edgeLineAnimationTimer;
        private readonly DispatcherTimer lineDistanceAnimationTimer;
        private readonly DispatcherTimer matchingAnimationTimer;
        private readonly DispatcherTimer featureMatchingAnimationTimer;
        private readonly DispatcherTimer metricsAcceptanceAnimationTimer;
        private readonly DispatcherTimer layerRecipeAnimationTimer;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
        private readonly int[] sampleValues = { 24, 60, 96, 119, 128, 151, 190, 230 };
        private readonly int[] brightnessSampleValues = { 22, 48, 75, 103, 126, 152, 184, 218 };
        private readonly int[] arithmeticInputAValues = { 20, 45, 90, 120, 150, 180, 210, 240 };
        private readonly int[] arithmeticInputBValues = { 10, 60, 40, 140, 110, 200, 30, 220 };
        private readonly int[] filterSampleValues = { 42, 58, 54, 60, 220, 65, 57, 62, 59 };
        private readonly int[] morphologySampleValues =
        {
            0, 0, 0, 0, 0,
            0, 255, 255, 255, 0,
            0, 255, 255, 255, 0,
            0, 255, 255, 255, 0,
            255, 0, 0, 0, 0
        };
        private readonly int[] blobSampleValues =
        {
            0, 0, 0, 0, 0, 0,
            0, 255, 255, 0, 255, 0,
            0, 255, 255, 0, 255, 255,
            0, 0, 0, 0, 255, 255,
            255, 0, 0, 0, 0, 0
        };
        private readonly int[] contourSampleValues =
        {
            0, 0, 0, 0, 0, 0, 0,
            0, 255, 255, 255, 255, 0, 0,
            0, 255, 255, 0, 255, 0, 0,
            0, 255, 255, 255, 255, 0, 0,
            0, 0, 0, 0, 0, 0, 255
        };
        private readonly int[] edgeLineSampleValues =
        {
            42, 48, 54, 186, 193,
            45, 50, 57, 190, 198,
            43, 49, 55, 188, 196,
            47, 53, 60, 194, 201,
            44, 51, 58, 191, 199
        };
        private readonly int[] lineDistanceLeftEdges = { 2, 2, 2, 2, 2 };
        private readonly int[] lineDistanceRightEdges = { 6, 6, 7, 6, 6 };
        private readonly int[] matchingSearchValues =
        {
            0, 0, 0, 0, 0,
            0, 1, 1, 0, 0,
            0, 1, 0, 0, 1,
            1, 1, 1, 0, 1,
            1, 0, 0, 0, 0
        };
        private readonly int[] matchingTemplateValues =
        {
            1, 1,
            1, 0
        };
        private readonly (int X, int Y)[] matchingCandidatePositions =
        {
            (0, 0),
            (1, 1),
            (3, 1),
            (2, 2),
            (3, 3)
        };
        private readonly (int X, int Y)[] featureReferencePoints =
        {
            (1, 1),
            (3, 1),
            (2, 2),
            (1, 3),
            (3, 3),
            (0, 4)
        };
        private readonly (int X, int Y)[] featureScenePoints =
        {
            (1, 1),
            (3, 1),
            (2, 2),
            (1, 3),
            (3, 3),
            (4, 4)
        };
        private readonly double[] featureMatchScores = { 0.92D, 0.88D, 0.81D, 0.74D, 0.67D, 0.42D };
        private readonly double[] metricsAcceptanceSamples = { 0.50D, 0.51D, 0.49D, 0.82D, 0.50D };
        private readonly string[] layerRecipeLayers = { "Main", "Pin_Binary", "Pin_Gap", "Pin_Review" };
        private readonly (string Input, string Tool, string Output)[] layerRecipeSteps =
        {
            ("Main", "Threshold", "Pin_Binary"),
            ("Pin_Binary", "LineDistance", "Pin_Gap"),
            ("Main + Pin_Gap", "Overlay", "Pin_Review"),
            ("Pin_Gap", "Accept", "Inspection")
        };
        private Action<string> openPracticeSamplesAction;
        private Action<VISION_MENU> openRelatedToolAction;
        private readonly List<Border> resultCells = new();
        private readonly List<TextBlock> resultTexts = new();
        private readonly List<Border> brightnessInputCells = new();
        private readonly List<Border> brightnessOutputCells = new();
        private readonly List<TextBlock> brightnessOutputTexts = new();
        private readonly List<Border> histogramBars = new();
        private readonly List<TextBlock> histogramLabels = new();
        private readonly List<Border> arithmeticInputACells = new();
        private readonly List<Border> arithmeticInputBCells = new();
        private readonly List<Border> arithmeticResultCells = new();
        private readonly List<TextBlock> arithmeticResultTexts = new();
        private readonly List<Border> filterInputCells = new();
        private readonly List<Border> filterOutputCells = new();
        private readonly List<TextBlock> filterOutputTexts = new();
        private readonly List<Border> foundationMatCells = new();
        private readonly List<Border> morphologyInputCells = new();
        private readonly List<Border> morphologyOutputCells = new();
        private readonly List<TextBlock> morphologyOutputTexts = new();
        private readonly List<Border> blobInputCells = new();
        private readonly List<Border> blobOutputCells = new();
        private readonly List<TextBlock> blobOutputTexts = new();
        private readonly List<Border> contourInputCells = new();
        private readonly List<Border> contourOutputCells = new();
        private readonly List<TextBlock> contourOutputTexts = new();
        private readonly List<Border> edgeLineInputCells = new();
        private readonly List<Border> edgeLineOutputCells = new();
        private readonly List<TextBlock> edgeLineOutputTexts = new();
        private readonly List<Border> lineDistanceInputCells = new();
        private readonly List<Border> lineDistanceOutputCells = new();
        private readonly List<TextBlock> lineDistanceOutputTexts = new();
        private readonly List<Border> matchingSearchCells = new();
        private readonly List<TextBlock> matchingSearchTexts = new();
        private readonly List<Border> matchingScoreCells = new();
        private readonly List<TextBlock> matchingScoreTexts = new();
        private readonly List<Border> featureReferenceCells = new();
        private readonly List<TextBlock> featureReferenceTexts = new();
        private readonly List<Border> featureSceneCells = new();
        private readonly List<TextBlock> featureSceneTexts = new();
        private readonly List<Border> featureMatchScoreCells = new();
        private readonly List<TextBlock> featureMatchScoreTexts = new();
        private readonly List<Border> metricsAcceptanceSampleCells = new();
        private readonly List<TextBlock> metricsAcceptanceSampleTexts = new();
        private readonly List<Border> layerRecipeLayerCells = new();
        private readonly List<TextBlock> layerRecipeLayerTexts = new();
        private readonly List<Border> layerRecipeFlowCells = new();
        private readonly List<TextBlock> layerRecipeFlowTexts = new();
        private bool animationForward = true;
        private int arithmeticAnimationStep = ArithmeticAnimationStepCount;
        private int brightnessAnimationStep = BrightnessAnimationStepCount;
        private int colorHsvAnimationStep = ColorHsvAnimationStepCount;
        private int filterAnimationStep = FilterAnimationStepCount;
        private int foundationAnimationStep = FoundationAnimationStepCount;
        private int foundationSelectedCellCount;
        private int matChannelAnimationStep = MatChannelAnimationStepCount;
        private int geometryAnimationStep = GeometryAnimationStepCount;
        private int morphologyAnimationStep = 25;
        private int blobAnimationStep;
        private int contourAnimationStep = ContourAnimationStepCount;
        private int edgeLineAnimationStep = EdgeLineAnimationStepCount;
        private int lineDistanceAnimationStep = LineDistanceAnimationStepCount;
        private int matchingAnimationStep = MatchingAnimationStepCount;
        private int featureMatchingAnimationStep = FeatureMatchingAnimationStepCount;
        private int metricsAcceptanceAnimationStep = MetricsAcceptanceAnimationStepCount;
        private int layerRecipeAnimationStep = LayerRecipeAnimationStepCount;
        private bool isLayerRecipeAnimationAdvancing;
        private int maxValue = 255;

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
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            this.maxValue = ClampToByte(maxValue);
            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
            animationTimer.Tick += AnimationTimer_Tick;
            arithmeticAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            arithmeticAnimationTimer.Tick += ArithmeticAnimationTimer_Tick;
            brightnessAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            brightnessAnimationTimer.Tick += BrightnessAnimationTimer_Tick;
            colorHsvAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            colorHsvAnimationTimer.Tick += ColorHsvAnimationTimer_Tick;
            filterAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            filterAnimationTimer.Tick += FilterAnimationTimer_Tick;
            foundationAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            foundationAnimationTimer.Tick += FoundationAnimationTimer_Tick;
            matChannelAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(620) };
            matChannelAnimationTimer.Tick += MatChannelAnimationTimer_Tick;
            geometryAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            geometryAnimationTimer.Tick += GeometryAnimationTimer_Tick;
            morphologyAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(280) };
            morphologyAnimationTimer.Tick += MorphologyAnimationTimer_Tick;
            blobAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            blobAnimationTimer.Tick += BlobAnimationTimer_Tick;
            contourAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            contourAnimationTimer.Tick += ContourAnimationTimer_Tick;
            edgeLineAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            edgeLineAnimationTimer.Tick += EdgeLineAnimationTimer_Tick;
            lineDistanceAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            lineDistanceAnimationTimer.Tick += LineDistanceAnimationTimer_Tick;
            matchingAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            matchingAnimationTimer.Tick += MatchingAnimationTimer_Tick;
            featureMatchingAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            featureMatchingAnimationTimer.Tick += FeatureMatchingAnimationTimer_Tick;
            metricsAcceptanceAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            metricsAcceptanceAnimationTimer.Tick += MetricsAcceptanceAnimationTimer_Tick;
            layerRecipeAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            layerRecipeAnimationTimer.Tick += LayerRecipeAnimationTimer_Tick;
            BuildSampleCells();
            BuildBrightnessCells();
            BuildArithmeticCells();
            BuildFilterCells();
            BuildFoundationCells();
            BuildMorphologyCells();
            BuildBlobCells();
            blobAnimationStep = GetBlobCandidateCount();
            BuildContourCells();
            BuildEdgeLineCells();
            BuildLineDistanceCells();
            BuildMatchingCells();
            BuildFeatureMatchingCells();
            BuildMetricsAcceptanceCells();
            BuildLayerRecipeCells();
            thresholdSlider.Value = ClampToByte(threshold);
            chkInvert.IsChecked = invert;
            txtMaxValue.Text = "MaxValue = " + this.maxValue.ToString(CultureInfo.InvariantCulture);
            brightnessOffsetSlider.Value = 35;
            blobMinAreaSlider.Value = 3;
            edgeThresholdSlider.Value = 80;
            lineDistanceRangeMaxSlider.Value = 0.5;
            matchingThresholdSlider.Value = 0.85;
            featureGoodMatchMinSlider.Value = 4;
            layerRecipeStepSlider.Value = 2;
            geometryAngleSlider.Value = 15;
            geometryScaleSlider.Value = 100;
            colorHueSlider.Value = 30;
            colorValueSlider.Value = 190;
            topicList.SelectedIndex = NormalizeTopicIndex(selectedTopicIndex);
            UpdateGuide();
            UpdateBrightnessGuide();
            UpdateArithmeticGuide();
            UpdateFilterGuide();
            UpdateFoundationGuide();
            UpdateMatChannelGuide();
            UpdateMorphologyGuide();
            UpdateBlobGuide();
            UpdateContourGuide();
            UpdateEdgeLineGuide();
            UpdateLineDistanceGuide();
            UpdateMatchingGuide();
            UpdateFeatureMatchingGuide();
            UpdateMetricsAcceptanceGuide();
            UpdateLayerRecipeGuide();
            UpdateGeometryGuide();
            UpdateColorHsvGuide();
            UpdateSelectedTopic();
        }

        public double ThresholdValueForTest
        {
            get => thresholdSlider.Value;
            set => thresholdSlider.Value = ClampToByte(value);
        }

        public bool IsInvertedForTest
        {
            get => chkInvert.IsChecked == true;
            set => chkInvert.IsChecked = value;
        }

        public string FormulaTextForTest => txtFormula.Text ?? string.Empty;

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

        public bool AreMatchingFamilyDecisionsCollapsedForTest =>
            !matchingFamilyDecisionExpander.IsExpanded && !featureMatchingFamilyDecisionExpander.IsExpanded;

        public bool CanOpenPracticeSamplesForTest => btnPracticeSamples.IsEnabled;

        public bool CanOpenRelatedToolsForTest =>
            btnFoundationOpenRoiTool.IsEnabled
            && btnFoundationOpenKernelTool.IsEnabled
            && btnFoundationOpenOutputSizeTool.IsEnabled;

        public bool CanOpenColorHsvToolForTest => btnColorHsvOpenTool.IsEnabled;

        public bool CanOpenThresholdToolForTest => btnThresholdOpenTool.IsEnabled;

        public bool CanOpenBrightnessToolsForTest =>
            btnBrightnessOpenMeanTool.IsEnabled
            && btnBrightnessOpenHistogramTool.IsEnabled;

        public bool CanOpenArithmeticToolForTest => btnArithmeticOpenTool.IsEnabled;

        public bool CanOpenGeometryToolForTest => btnGeometryOpenTool.IsEnabled;

        public bool CanOpenAffineTransformToolForTest => btnGeometryOpenAffineTool.IsEnabled;

        public bool CanOpenFilteringToolForTest => btnFilteringOpenTool.IsEnabled;

        public bool CanOpenMorphologyToolForTest => btnMorphologyOpenTool.IsEnabled;

        public bool CanOpenBlobToolForTest => btnBlobOpenTool.IsEnabled;

        public bool CanOpenContourToolForTest => btnContourOpenTool.IsEnabled;

        public bool CanOpenEdgeLineToolsForTest =>
            btnEdgeDetectionOpenTool.IsEnabled && btnEdgeLineOpenLineTool.IsEnabled;

        public bool CanOpenLineDistanceToolForTest => btnLineDistanceOpenTool.IsEnabled;

        public bool CanOpenMatchingToolForTest => btnMatchingOpenTool.IsEnabled;

        public bool CanOpenEdgeBasedMatchingToolForTest =>
            btnMatchingOpenTool.IsEnabled
            && string.Equals(Convert.ToString(btnMatchingOpenTool.Tag, CultureInfo.InvariantCulture), nameof(VISION_MENU.EdgeBasedMatching), StringComparison.Ordinal);

        public bool CanOpenFeatureMatchingToolForTest => btnFeatureMatchingOpenTool.IsEnabled;

        public string FoundationToolLocationTitleForTest => txtFoundationToolLocationTitle.Text ?? string.Empty;

        public string FoundationToolLocationDetailForTest => txtFoundationToolLocationDetail.Text ?? string.Empty;

        public void BringFoundationToolLocationIntoViewForTest()
        {
            foundationToolLocationPanel.BringIntoView();
        }

        public string ColorHsvToolLocationTitleForTest => txtColorHsvToolLocationTitle.Text ?? string.Empty;

        public string ColorHsvToolLocationDetailForTest => txtColorHsvToolLocationDetail.Text ?? string.Empty;

        public string BrightnessToolLocationTitleForTest => txtBrightnessToolLocationTitle.Text ?? string.Empty;

        public string BrightnessToolLocationDetailForTest => txtBrightnessToolLocationDetail.Text ?? string.Empty;

        public string ArithmeticToolLocationTitleForTest => txtArithmeticToolLocationTitle.Text ?? string.Empty;

        public string ArithmeticToolLocationDetailForTest => txtArithmeticToolLocationDetail.Text ?? string.Empty;

        public string GeometryToolLocationTitleForTest => txtGeometryToolLocationTitle.Text ?? string.Empty;

        public string GeometryToolLocationDetailForTest => txtGeometryToolLocationDetail.Text ?? string.Empty;

        public string FilteringToolLocationTitleForTest => txtFilteringToolLocationTitle.Text ?? string.Empty;

        public string FilteringToolLocationDetailForTest => txtFilteringToolLocationDetail.Text ?? string.Empty;

        public string MorphologyToolLocationTitleForTest => txtMorphologyToolLocationTitle.Text ?? string.Empty;

        public string MorphologyToolLocationDetailForTest => txtMorphologyToolLocationDetail.Text ?? string.Empty;

        public string BlobToolLocationTitleForTest => txtBlobToolLocationTitle.Text ?? string.Empty;

        public string BlobToolLocationDetailForTest => txtBlobToolLocationDetail.Text ?? string.Empty;

        public string ContourToolLocationTitleForTest => txtContourToolLocationTitle.Text ?? string.Empty;

        public string ContourToolLocationDetailForTest => txtContourToolLocationDetail.Text ?? string.Empty;

        public string EdgeLineToolLocationTitleForTest => txtEdgeLineToolLocationTitle.Text ?? string.Empty;

        public string EdgeLineToolLocationDetailForTest => txtEdgeLineToolLocationDetail.Text ?? string.Empty;

        public string LineDistanceToolLocationTitleForTest => txtLineDistanceToolLocationTitle.Text ?? string.Empty;

        public string LineDistanceToolLocationDetailForTest => txtLineDistanceToolLocationDetail.Text ?? string.Empty;

        public string MatchingToolLocationTitleForTest => txtMatchingToolLocationTitle.Text ?? string.Empty;

        public string MatchingToolLocationDetailForTest => txtMatchingToolLocationDetail.Text ?? string.Empty;

        public string EdgeBasedMatchingToolLocationTitleForTest => txtMatchingToolLocationTitle.Text ?? string.Empty;

        public string EdgeBasedMatchingToolLocationDetailForTest => txtMatchingToolLocationDetail.Text ?? string.Empty;

        public string FeatureMatchingToolLocationTitleForTest => txtFeatureMatchingToolLocationTitle.Text ?? string.Empty;

        public string FeatureMatchingToolLocationDetailForTest => txtFeatureMatchingToolLocationDetail.Text ?? string.Empty;

        public void SetOpenPracticeSamplesAction(Action<string> action)
        {
            openPracticeSamplesAction = action;
            btnPracticeSamples.IsEnabled = action != null;
        }

        public void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            bool enabled = action != null;
            btnFoundationOpenRoiTool.IsEnabled = enabled;
            btnFoundationOpenKernelTool.IsEnabled = enabled;
            btnFoundationOpenOutputSizeTool.IsEnabled = enabled;
            btnBrightnessOpenMeanTool.IsEnabled = enabled;
            btnBrightnessOpenHistogramTool.IsEnabled = enabled;
            btnArithmeticOpenTool.IsEnabled = enabled;
            btnGeometryOpenTool.IsEnabled = enabled;
            btnGeometryOpenAffineTool.IsEnabled = enabled;
            btnFilteringOpenTool.IsEnabled = enabled;
            btnMorphologyOpenTool.IsEnabled = enabled;
            btnBlobOpenTool.IsEnabled = enabled;
            btnContourOpenTool.IsEnabled = enabled;
            btnEdgeDetectionOpenTool.IsEnabled = enabled;
            btnEdgeLineOpenLineTool.IsEnabled = enabled;
            btnLineDistanceOpenTool.IsEnabled = enabled;
            btnMatchingOpenTool.IsEnabled = enabled;
            btnFeatureMatchingOpenTool.IsEnabled = enabled;
            btnColorHsvOpenTool.IsEnabled = enabled;
            btnThresholdOpenTool.IsEnabled = enabled;
        }

        public double BrightnessOffsetForTest
        {
            get => brightnessOffsetSlider.Value;
            set => brightnessOffsetSlider.Value = Math.Max(-80, Math.Min(80, value));
        }

        public string BrightnessFormulaTextForTest => txtBrightnessFormula.Text ?? string.Empty;

        public int BrightnessAnimationStepForTest => brightnessAnimationStep;

        public string BrightnessAnimationStatusTextForTest => txtBrightnessAnimationStatus.Text ?? string.Empty;

        public void ResetBrightnessAnimationForTest()
        {
            ResetBrightnessAnimation();
        }

        public void AdvanceBrightnessAnimationForTest()
        {
            AdvanceBrightnessAnimation();
        }

        public void ToggleBrightnessAnimationForTest()
        {
            BrightnessPlayButton_Click(this, new RoutedEventArgs());
        }

        public int ArithmeticModeIndexForTest
        {
            get => arithmeticModeCombo.SelectedIndex;
            set => arithmeticModeCombo.SelectedIndex = Math.Max(0, Math.Min(4, value));
        }

        public string ArithmeticFormulaTextForTest => txtArithmeticFormula.Text ?? string.Empty;

        public int ArithmeticAnimationStepForTest => arithmeticAnimationStep;

        public string ArithmeticAnimationStatusTextForTest => txtArithmeticAnimationStatus.Text ?? string.Empty;

        public void ResetArithmeticAnimationForTest()
        {
            ResetArithmeticAnimation();
        }

        public void AdvanceArithmeticAnimationForTest()
        {
            AdvanceArithmeticAnimation();
        }

        public void ToggleArithmeticAnimationForTest()
        {
            ArithmeticPlayButton_Click(this, new RoutedEventArgs());
        }

        public int FilterModeIndexForTest
        {
            get => filterModeCombo.SelectedIndex;
            set => filterModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        public string FilterFormulaTextForTest => txtFilterFormula.Text ?? string.Empty;

        public int FilterAnimationStepForTest => filterAnimationStep;

        public string FilterAnimationStatusTextForTest => txtFilterAnimationStatus.Text ?? string.Empty;

        public void ResetFilterAnimationForTest()
        {
            ResetFilterAnimation();
        }

        public void AdvanceFilterAnimationForTest()
        {
            AdvanceFilterAnimation();
        }

        public void ToggleFilterAnimationForTest()
        {
            FilterPlayButton_Click(this, new RoutedEventArgs());
        }

        public int FoundationAnimationStepForTest => foundationAnimationStep;

        public int FoundationSelectedCellCountForTest => foundationSelectedCellCount;

        public string FoundationAnimationStatusTextForTest => txtFoundationAnimationStatus.Text ?? string.Empty;

        public bool IsFoundationPointVisibleForTest => foundationPointMarker.Visibility == Visibility.Visible;

        public bool IsFoundationRectVisibleForTest => foundationRoiRect.Visibility == Visibility.Visible;

        public bool IsFoundationRotatedRectVisibleForTest => foundationRotatedRect.Visibility == Visibility.Visible;

        public bool IsFoundationRotatedBoundsVisibleForTest => foundationRotatedBoundsRect.Visibility == Visibility.Visible;

        public bool IsFoundationRotatedCenterVisibleForTest => foundationRotatedCenterMarker.Visibility == Visibility.Visible;

        public double FoundationRotatedRectAngleForTest =>
            (foundationRotatedRect.RenderTransform as RotateTransform)?.Angle ?? 0D;

        public void ResetFoundationAnimationForTest()
        {
            ResetFoundationAnimation();
        }

        public void AdvanceFoundationAnimationForTest()
        {
            AdvanceFoundationAnimation();
        }

        public void ToggleFoundationAnimationForTest()
        {
            FoundationPlayButton_Click(this, new RoutedEventArgs());
        }

        public int MatChannelAnimationStepForTest => matChannelAnimationStep;

        public string MatChannelAnimationStatusTextForTest => txtMatChannelAnimationStatus.Text ?? string.Empty;

        public double MatChannelSplitOpacityForTest => matChannelBlueCell.Opacity;

        public double MatChannelGrayOpacityForTest => matChannelGrayCell.Opacity;

        public string MatChannelBgrShapeTextForTest => txtMatChannelBgrShape.Text ?? string.Empty;

        public string MatChannelGrayShapeTextForTest => txtMatChannelGrayShape.Text ?? string.Empty;

        public double MatChannelTypeGuideOpacityForTest => matChannelTypeGuidePanel.Opacity;

        public string MatChannelTypeTitleForTest => txtMatChannelTypeTitle.Text ?? string.Empty;

        public string MatChannelTypeDetailForTest => txtMatChannelTypeDetail.Text ?? string.Empty;

        public void ResetMatChannelAnimationForTest()
        {
            ResetMatChannelAnimation();
        }

        public void AdvanceMatChannelAnimationForTest()
        {
            AdvanceMatChannelAnimation();
        }

        public void ToggleMatChannelAnimationForTest()
        {
            MatChannelPlayButton_Click(this, new RoutedEventArgs());
        }

        public int MorphologyModeIndexForTest
        {
            get => morphologyModeCombo.SelectedIndex;
            set => morphologyModeCombo.SelectedIndex = Math.Max(0, Math.Min(3, value));
        }

        public string MorphologyFormulaTextForTest => txtMorphologyFormula.Text ?? string.Empty;

        public int MorphologyAnimationStepForTest => morphologyAnimationStep;

        public string MorphologyAnimationStatusTextForTest => txtMorphologyAnimationStatus.Text ?? string.Empty;

        public void ResetMorphologyAnimationForTest()
        {
            ResetMorphologyAnimation();
        }

        public void AdvanceMorphologyAnimationForTest()
        {
            AdvanceMorphologyAnimation();
        }

        public void ToggleMorphologyAnimationForTest()
        {
            MorphologyPlayButton_Click(this, new RoutedEventArgs());
        }

        public double BlobMinAreaForTest
        {
            get => blobMinAreaSlider.Value;
            set => blobMinAreaSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        public string BlobFormulaTextForTest => txtBlobFormula.Text ?? string.Empty;

        public int BlobAnimationStepForTest => blobAnimationStep;

        public string BlobAnimationStatusTextForTest => txtBlobAnimationStatus.Text ?? string.Empty;

        public void ResetBlobAnimationForTest()
        {
            ResetBlobAnimation();
        }

        public void AdvanceBlobAnimationForTest()
        {
            AdvanceBlobAnimation();
        }

        public void ToggleBlobAnimationForTest()
        {
            BlobPlayButton_Click(this, new RoutedEventArgs());
        }

        public int ContourDrawModeIndexForTest
        {
            get => contourDrawModeCombo.SelectedIndex;
            set => contourDrawModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        public string ContourFormulaTextForTest => txtContourFormula.Text ?? string.Empty;

        public int ContourAnimationStepForTest => contourAnimationStep;

        public string ContourAnimationStatusTextForTest => txtContourAnimationStatus.Text ?? string.Empty;

        public void ResetContourAnimationForTest()
        {
            ResetContourAnimation();
        }

        public void AdvanceContourAnimationForTest()
        {
            AdvanceContourAnimation();
        }

        public void ToggleContourAnimationForTest()
        {
            ContourPlayButton_Click(this, new RoutedEventArgs());
        }

        public double EdgeThresholdForTest
        {
            get => edgeThresholdSlider.Value;
            set => edgeThresholdSlider.Value = Math.Max(10, Math.Min(150, value));
        }

        public string EdgeLineFormulaTextForTest => txtEdgeLineFormula.Text ?? string.Empty;

        public int EdgeLineAnimationStepForTest => edgeLineAnimationStep;

        public string EdgeLineAnimationStatusTextForTest => txtEdgeLineAnimationStatus.Text ?? string.Empty;

        public void ResetEdgeLineAnimationForTest()
        {
            ResetEdgeLineAnimation();
        }

        public void AdvanceEdgeLineAnimationForTest()
        {
            AdvanceEdgeLineAnimation();
        }

        public void ToggleEdgeLineAnimationForTest()
        {
            EdgeLinePlayButton_Click(this, new RoutedEventArgs());
        }

        public double LineDistanceRangeMaxForTest
        {
            get => lineDistanceRangeMaxSlider.Value;
            set => lineDistanceRangeMaxSlider.Value = Math.Max(0, Math.Min(2, value));
        }

        public string LineDistanceFormulaTextForTest => txtLineDistanceFormula.Text ?? string.Empty;

        public int LineDistanceAnimationStepForTest => lineDistanceAnimationStep;

        public string LineDistanceAnimationStatusTextForTest => txtLineDistanceAnimationStatus.Text ?? string.Empty;

        public void ResetLineDistanceAnimationForTest()
        {
            ResetLineDistanceAnimation();
        }

        public void AdvanceLineDistanceAnimationForTest()
        {
            AdvanceLineDistanceAnimation();
        }

        public void ToggleLineDistanceAnimationForTest()
        {
            LineDistancePlayButton_Click(this, new RoutedEventArgs());
        }

        public double MatchingThresholdForTest
        {
            get => matchingThresholdSlider.Value;
            set => matchingThresholdSlider.Value = Math.Max(0.50, Math.Min(1.00, value));
        }

        public string MatchingFormulaTextForTest => txtMatchingFormula.Text ?? string.Empty;

        public int MatchingAnimationStepForTest => matchingAnimationStep;

        public string MatchingAnimationStatusTextForTest => txtMatchingAnimationStatus.Text ?? string.Empty;

        public void ResetMatchingAnimationForTest()
        {
            ResetMatchingAnimation();
        }

        public void AdvanceMatchingAnimationForTest()
        {
            AdvanceMatchingAnimation();
        }

        public void ToggleMatchingAnimationForTest()
        {
            MatchingPlayButton_Click(this, new RoutedEventArgs());
        }

        public double FeatureGoodMatchMinForTest
        {
            get => featureGoodMatchMinSlider.Value;
            set => featureGoodMatchMinSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        public string FeatureMatchingFormulaTextForTest => txtFeatureMatchingFormula.Text ?? string.Empty;

        public int FeatureMatchingAnimationStepForTest => featureMatchingAnimationStep;

        public string FeatureMatchingAnimationStatusTextForTest => txtFeatureMatchingAnimationStatus.Text ?? string.Empty;

        public void ResetFeatureMatchingAnimationForTest()
        {
            ResetFeatureMatchingAnimation();
        }

        public void AdvanceFeatureMatchingAnimationForTest()
        {
            AdvanceFeatureMatchingAnimation();
        }

        public void ToggleFeatureMatchingAnimationForTest()
        {
            FeatureMatchingPlayButton_Click(this, new RoutedEventArgs());
        }

        public int MetricsAcceptanceAnimationStepForTest => metricsAcceptanceAnimationStep;

        public string MetricsAcceptanceAnimationStatusTextForTest => txtMetricsAcceptanceAnimationStatus.Text ?? string.Empty;

        public string MetricsAcceptanceFormulaTextForTest => txtMetricsAcceptanceFormula.Text ?? string.Empty;

        public bool IsMetricGateCheatSheetExpandedForTest => metricGateCheatSheetExpander.IsExpanded;

        public bool IsAnimationLegendVisibleForTest => animationLegendPanel.Visibility == Visibility.Visible;

        public string AnimationLegendColorsForTest => string.Join(
            ",",
            ((SolidColorBrush)animationNeutralBrush).Color,
            ((SolidColorBrush)animationCandidateBrush).Color,
            ((SolidColorBrush)animationPassBrush).Color,
            ((SolidColorBrush)animationWarningBrush).Color);

        public void ResetMetricsAcceptanceAnimationForTest()
        {
            ResetMetricsAcceptanceAnimation();
        }

        public void AdvanceMetricsAcceptanceAnimationForTest()
        {
            AdvanceMetricsAcceptanceAnimation();
        }

        public void ToggleMetricsAcceptanceAnimationForTest()
        {
            MetricsAcceptancePlayButton_Click(this, new RoutedEventArgs());
        }

        public double LayerRecipeSelectedStepForTest
        {
            get => layerRecipeStepSlider.Value;
            set => layerRecipeStepSlider.Value = Math.Max(1, Math.Min(4, value));
        }

        public string LayerRecipeFormulaTextForTest => txtLayerRecipeFormula.Text ?? string.Empty;

        public int LayerRecipeAnimationStepForTest => layerRecipeAnimationStep;

        public string LayerRecipeAnimationStatusTextForTest => txtLayerRecipeAnimationStatus.Text ?? string.Empty;

        public void ResetLayerRecipeAnimationForTest()
        {
            ResetLayerRecipeAnimation();
        }

        public void AdvanceLayerRecipeAnimationForTest()
        {
            AdvanceLayerRecipeAnimation();
        }

        public void ToggleLayerRecipeAnimationForTest()
        {
            LayerRecipePlayButton_Click(this, new RoutedEventArgs());
        }

        public double GeometryAngleForTest
        {
            get => geometryAngleSlider.Value;
            set => geometryAngleSlider.Value = Math.Max(-45, Math.Min(45, value));
        }

        public double GeometryScaleForTest
        {
            get => geometryScaleSlider.Value;
            set => geometryScaleSlider.Value = Math.Max(50, Math.Min(150, value));
        }

        public string GeometryFormulaTextForTest => txtGeometryFormula.Text ?? string.Empty;

        public int GeometryAnimationStepForTest => geometryAnimationStep;

        public string GeometryAnimationStatusTextForTest => txtGeometryAnimationStatus.Text ?? string.Empty;

        public double GeometryRenderedAngleForTest => geometryRotateTransform.Angle;

        public double GeometryRenderedScaleForTest => geometryScaleTransform.ScaleX;

        public void ResetGeometryAnimationForTest()
        {
            ResetGeometryAnimation();
        }

        public void AdvanceGeometryAnimationForTest()
        {
            AdvanceGeometryAnimation();
        }

        public void ToggleGeometryAnimationForTest()
        {
            GeometryPlayButton_Click(this, new RoutedEventArgs());
        }

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

        public void ApplyForTest()
        {
            ApplyButton_Click(this, new RoutedEventArgs());
        }

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
            animationTimer.Stop();
            arithmeticAnimationTimer.Stop();
            brightnessAnimationTimer.Stop();
            colorHsvAnimationTimer.Stop();
            filterAnimationTimer.Stop();
            foundationAnimationTimer.Stop();
            matChannelAnimationTimer.Stop();
            geometryAnimationTimer.Stop();
            morphologyAnimationTimer.Stop();
            blobAnimationTimer.Stop();
            contourAnimationTimer.Stop();
            edgeLineAnimationTimer.Stop();
            lineDistanceAnimationTimer.Stop();
            matchingAnimationTimer.Stop();
            featureMatchingAnimationTimer.Stop();
            metricsAcceptanceAnimationTimer.Stop();
            layerRecipeAnimationTimer.Stop();
            base.OnClosed(e);
        }

        private void BuildSampleCells()
        {
            sampleGrid.Children.Clear();
            resultGrid.Children.Clear();
            resultCells.Clear();
            resultTexts.Clear();

            foreach (int value in sampleValues)
            {
                sampleGrid.Children.Add(CreateCell(value.ToString(CultureInfo.InvariantCulture), value));
                Border resultCell = CreateCell(string.Empty, 0);
                TextBlock resultText = (TextBlock)resultCell.Child;
                resultGrid.Children.Add(resultCell);
                resultCells.Add(resultCell);
                resultTexts.Add(resultText);
            }
        }

        private void BuildBrightnessCells()
        {
            brightnessInputGrid.Children.Clear();
            brightnessOutputGrid.Children.Clear();
            histogramGrid.Children.Clear();
            brightnessInputCells.Clear();
            brightnessOutputCells.Clear();
            brightnessOutputTexts.Clear();
            histogramBars.Clear();
            histogramLabels.Clear();

            foreach (int value in brightnessSampleValues)
            {
                Border inputCell = CreateCell(value.ToString(CultureInfo.InvariantCulture), value);
                brightnessInputGrid.Children.Add(inputCell);
                brightnessInputCells.Add(inputCell);
                Border outputCell = CreateCell(string.Empty, value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                brightnessOutputGrid.Children.Add(outputCell);
                brightnessOutputCells.Add(outputCell);
                brightnessOutputTexts.Add(outputText);
            }

            for (int i = 0; i < 8; i++)
            {
                Grid column = new()
                {
                    Margin = new Thickness(3, 0, 3, 0)
                };
                column.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                column.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Border bar = new()
                {
                    MinHeight = 4,
                    Width = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = new SolidColorBrush(Color.FromRgb(21, 124, 134)),
                    CornerRadius = new CornerRadius(3, 3, 0, 0)
                };
                TextBlock label = new()
                {
                    Margin = new Thickness(0, 6, 0, 0),
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(82, 101, 121)),
                    TextWrapping = TextWrapping.Wrap
                };

                Grid.SetRow(bar, 0);
                Grid.SetRow(label, 1);
                column.Children.Add(bar);
                column.Children.Add(label);
                histogramGrid.Children.Add(column);
                histogramBars.Add(bar);
                histogramLabels.Add(label);
            }
        }

        private void BuildFilterCells()
        {
            filterInputGrid.Children.Clear();
            filterOutputGrid.Children.Clear();
            filterInputCells.Clear();
            filterOutputCells.Clear();
            filterOutputTexts.Clear();

            foreach (int value in filterSampleValues)
            {
                Border inputCell = CreateCell(value.ToString(CultureInfo.InvariantCulture), value);
                filterInputGrid.Children.Add(inputCell);
                filterInputCells.Add(inputCell);
                Border outputCell = CreateCell(string.Empty, value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                filterOutputGrid.Children.Add(outputCell);
                filterOutputCells.Add(outputCell);
                filterOutputTexts.Add(outputText);
            }
        }

        private void BuildFoundationCells()
        {
            foundationMatGrid.Children.Clear();
            foundationMatCells.Clear();
            for (int i = 0; i < 48; i++)
            {
                Border cell = new()
                {
                    Margin = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(203, 213, 225)),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Color.FromRgb(248, 250, 252))
                };
                foundationMatGrid.Children.Add(cell);
                foundationMatCells.Add(cell);
            }
        }

        private void BuildArithmeticCells()
        {
            arithmeticInputAGrid.Children.Clear();
            arithmeticInputBGrid.Children.Clear();
            arithmeticResultGrid.Children.Clear();
            arithmeticInputACells.Clear();
            arithmeticInputBCells.Clear();
            arithmeticResultCells.Clear();
            arithmeticResultTexts.Clear();

            for (int i = 0; i < arithmeticInputAValues.Length; i++)
            {
                int inputA = arithmeticInputAValues[i];
                int inputB = arithmeticInputBValues[i];
                Border inputACell = CreateCell(inputA.ToString(CultureInfo.InvariantCulture), inputA);
                Border inputBCell = CreateCell(inputB.ToString(CultureInfo.InvariantCulture), inputB);
                arithmeticInputAGrid.Children.Add(inputACell);
                arithmeticInputBGrid.Children.Add(inputBCell);
                arithmeticInputACells.Add(inputACell);
                arithmeticInputBCells.Add(inputBCell);

                Border outputCell = CreateCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                arithmeticResultGrid.Children.Add(outputCell);
                arithmeticResultCells.Add(outputCell);
                arithmeticResultTexts.Add(outputText);
            }
        }

        private void BuildMorphologyCells()
        {
            morphologyInputGrid.Children.Clear();
            morphologyOutputGrid.Children.Clear();
            morphologyInputCells.Clear();
            morphologyOutputCells.Clear();
            morphologyOutputTexts.Clear();

            foreach (int value in morphologySampleValues)
            {
                Border inputCell = CreateBinaryCell(value);
                morphologyInputGrid.Children.Add(inputCell);
                morphologyInputCells.Add(inputCell);
                Border outputCell = CreateBinaryCell(value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                morphologyOutputGrid.Children.Add(outputCell);
                morphologyOutputCells.Add(outputCell);
                morphologyOutputTexts.Add(outputText);
            }
        }

        private void BuildBlobCells()
        {
            blobInputGrid.Children.Clear();
            blobOutputGrid.Children.Clear();
            blobInputCells.Clear();
            blobOutputCells.Clear();
            blobOutputTexts.Clear();

            foreach (int value in blobSampleValues)
            {
                Border inputCell = CreateBinaryCell(value);
                blobInputGrid.Children.Add(inputCell);
                blobInputCells.Add(inputCell);
                Border outputCell = CreateBinaryCell(value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                blobOutputGrid.Children.Add(outputCell);
                blobOutputCells.Add(outputCell);
                blobOutputTexts.Add(outputText);
            }
        }

        private void BuildContourCells()
        {
            contourInputGrid.Children.Clear();
            contourOutputGrid.Children.Clear();
            contourInputCells.Clear();
            contourOutputCells.Clear();
            contourOutputTexts.Clear();

            foreach (int value in contourSampleValues)
            {
                Border inputCell = CreateBinaryCell(value);
                contourInputGrid.Children.Add(inputCell);
                contourInputCells.Add(inputCell);
                Border outputCell = CreateBinaryCell(value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                contourOutputGrid.Children.Add(outputCell);
                contourOutputCells.Add(outputCell);
                contourOutputTexts.Add(outputText);
            }
        }

        private void BuildEdgeLineCells()
        {
            edgeLineInputGrid.Children.Clear();
            edgeLineOutputGrid.Children.Clear();
            edgeLineInputCells.Clear();
            edgeLineOutputCells.Clear();
            edgeLineOutputTexts.Clear();

            foreach (int value in edgeLineSampleValues)
            {
                Border inputCell = CreateSmallValueCell(value.ToString(CultureInfo.InvariantCulture), value);
                edgeLineInputGrid.Children.Add(inputCell);
                edgeLineInputCells.Add(inputCell);
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                edgeLineOutputGrid.Children.Add(outputCell);
                edgeLineOutputCells.Add(outputCell);
                edgeLineOutputTexts.Add(outputText);
            }
        }

        private void BuildLineDistanceCells()
        {
            lineDistanceInputGrid.Children.Clear();
            lineDistanceOutputGrid.Children.Clear();
            lineDistanceInputCells.Clear();
            lineDistanceOutputCells.Clear();
            lineDistanceOutputTexts.Clear();

            for (int y = 0; y < lineDistanceLeftEdges.Length; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Border cell = CreateSmallValueCell(string.Empty, 0);
                    TextBlock text = (TextBlock)cell.Child;
                    if (x == lineDistanceLeftEdges[y])
                    {
                        cell.Background = animationCandidateBrush;
                        text.Foreground = Brushes.White;
                        text.Text = "L";
                    }
                    else if (x == lineDistanceRightEdges[y])
                    {
                        cell.Background = animationPassBrush;
                        text.Foreground = Brushes.White;
                        text.Text = "R";
                    }
                    else if (x > lineDistanceLeftEdges[y] && x < lineDistanceRightEdges[y])
                    {
                        cell.Background = new SolidColorBrush(Color.FromRgb(229, 244, 247));
                        text.Foreground = Brushes.Black;
                        text.Text = "-";
                    }
                    else
                    {
                        text.Text = "0";
                    }

                    lineDistanceInputGrid.Children.Add(cell);
                    lineDistanceInputCells.Add(cell);
                }
            }

            for (int i = 0; i < lineDistanceLeftEdges.Length; i++)
            {
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                lineDistanceOutputGrid.Children.Add(outputCell);
                lineDistanceOutputCells.Add(outputCell);
                lineDistanceOutputTexts.Add(outputText);
            }
        }

        private void BuildMatchingCells()
        {
            matchingSearchGrid.Children.Clear();
            matchingTemplateGrid.Children.Clear();
            matchingScoreGrid.Children.Clear();
            matchingSearchCells.Clear();
            matchingSearchTexts.Clear();
            matchingScoreCells.Clear();
            matchingScoreTexts.Clear();

            for (int i = 0; i < matchingSearchValues.Length; i++)
            {
                Border cell = CreateSmallValueCell(matchingSearchValues[i] > 0 ? "1" : "0", matchingSearchValues[i] > 0 ? 230 : 20);
                TextBlock text = (TextBlock)cell.Child;
                matchingSearchGrid.Children.Add(cell);
                matchingSearchCells.Add(cell);
                matchingSearchTexts.Add(text);
            }

            foreach (int value in matchingTemplateValues)
            {
                Border cell = CreateSmallValueCell(value > 0 ? "T" : "0", value > 0 ? 230 : 20);
                TextBlock text = (TextBlock)cell.Child;
                if (value > 0)
                {
                    cell.Background = animationCandidateBrush;
                    text.Foreground = Brushes.White;
                }

                matchingTemplateGrid.Children.Add(cell);
            }

            for (int i = 0; i < matchingCandidatePositions.Length; i++)
            {
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                matchingScoreGrid.Children.Add(outputCell);
                matchingScoreCells.Add(outputCell);
                matchingScoreTexts.Add(outputText);
            }
        }

        private void BuildFeatureMatchingCells()
        {
            featureReferenceGrid.Children.Clear();
            featureSceneGrid.Children.Clear();
            featureMatchScoreGrid.Children.Clear();
            featureReferenceCells.Clear();
            featureReferenceTexts.Clear();
            featureSceneCells.Clear();
            featureSceneTexts.Clear();
            featureMatchScoreCells.Clear();
            featureMatchScoreTexts.Clear();

            for (int i = 0; i < 25; i++)
            {
                Border referenceCell = CreateSmallValueCell("0", 20);
                TextBlock referenceText = (TextBlock)referenceCell.Child;
                featureReferenceGrid.Children.Add(referenceCell);
                featureReferenceCells.Add(referenceCell);
                featureReferenceTexts.Add(referenceText);

                Border sceneCell = CreateSmallValueCell("0", 20);
                TextBlock sceneText = (TextBlock)sceneCell.Child;
                featureSceneGrid.Children.Add(sceneCell);
                featureSceneCells.Add(sceneCell);
                featureSceneTexts.Add(sceneText);
            }

            for (int i = 0; i < featureMatchScores.Length; i++)
            {
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                featureMatchScoreGrid.Children.Add(outputCell);
                featureMatchScoreCells.Add(outputCell);
                featureMatchScoreTexts.Add(outputText);
            }
        }

        private void BuildMetricsAcceptanceCells()
        {
            metricsAcceptanceSampleGrid.Children.Clear();
            metricsAcceptanceSampleCells.Clear();
            metricsAcceptanceSampleTexts.Clear();

            foreach (double sample in metricsAcceptanceSamples)
            {
                Border cell = CreateSmallValueCell(sample.ToString("0.00", CultureInfo.InvariantCulture), 230);
                TextBlock text = (TextBlock)cell.Child;
                metricsAcceptanceSampleGrid.Children.Add(cell);
                metricsAcceptanceSampleCells.Add(cell);
                metricsAcceptanceSampleTexts.Add(text);
            }
        }

        private void BuildLayerRecipeCells()
        {
            layerRecipeLayerGrid.Children.Clear();
            layerRecipeFlowGrid.Children.Clear();
            layerRecipeLayerCells.Clear();
            layerRecipeLayerTexts.Clear();
            layerRecipeFlowCells.Clear();
            layerRecipeFlowTexts.Clear();

            foreach (string layer in layerRecipeLayers)
            {
                Border cell = CreateSmallValueCell(layer, 230);
                TextBlock text = (TextBlock)cell.Child;
                layerRecipeLayerGrid.Children.Add(cell);
                layerRecipeLayerCells.Add(cell);
                layerRecipeLayerTexts.Add(text);
            }

            for (int i = 0; i < layerRecipeSteps.Length; i++)
            {
                AddLayerRecipeFlowCell((i + 1).ToString(CultureInfo.InvariantCulture));
                AddLayerRecipeFlowCell(layerRecipeSteps[i].Input);
                AddLayerRecipeFlowCell(layerRecipeSteps[i].Tool);
                AddLayerRecipeFlowCell(layerRecipeSteps[i].Output);
            }
        }

        private void AddLayerRecipeFlowCell(string text)
        {
            Border cell = CreateSmallValueCell(text, 230);
            TextBlock textBlock = (TextBlock)cell.Child;
            layerRecipeFlowGrid.Children.Add(cell);
            layerRecipeFlowCells.Add(cell);
            layerRecipeFlowTexts.Add(textBlock);
        }

        private static Border CreateCell(string text, int grayValue)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Border cell = new()
            {
                Height = 44,
                Margin = new Thickness(0, 0, 8, 8),
                CornerRadius = new CornerRadius(4),
                BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                Background = CreateGrayBrush(grayValue),
                Child = textBlock
            };

            textBlock.Foreground = grayValue > 128 ? Brushes.Black : Brushes.White;
            return cell;
        }

        private static Border CreateBinaryCell(int value)
        {
            TextBlock textBlock = new()
            {
                Text = value > 0 ? "1" : "0",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Border cell = new()
            {
                Height = 28,
                Margin = new Thickness(0, 0, 5, 5),
                CornerRadius = new CornerRadius(3),
                BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                Background = CreateGrayBrush(value),
                Child = textBlock
            };

            textBlock.Foreground = value > 0 ? Brushes.Black : Brushes.White;
            return cell;
        }

        private static Border CreateSmallValueCell(string text, int grayValue)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Border cell = new()
            {
                Height = 30,
                Margin = new Thickness(0, 0, 5, 5),
                CornerRadius = new CornerRadius(3),
                BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                Background = CreateGrayBrush(grayValue),
                Child = textBlock
            };

            textBlock.Foreground = grayValue > 128 ? Brushes.Black : Brushes.White;
            return cell;
        }

        private void UpdateGuide()
        {
            int threshold = ClampToByte(thresholdSlider.Value);
            bool invert = chkInvert.IsChecked == true;
            txtThresholdValue.Text = "Threshold = " + threshold.ToString(CultureInfo.InvariantCulture);
            txtFormula.Text = invert
                ? "BinaryInv: GV >= threshold -> 0, GV < threshold -> MaxValue"
                : "Binary: GV >= threshold -> MaxValue, GV < threshold -> 0";

            for (int i = 0; i < sampleValues.Length; i++)
            {
                int source = sampleValues[i];
                bool high = source >= threshold;
                int result = invert
                    ? high ? 0 : maxValue
                    : high ? maxValue : 0;
                resultCells[i].Background = CreateGrayBrush(result);
                resultTexts[i].Foreground = result > 128 ? Brushes.Black : Brushes.White;
                resultTexts[i].Text = source.ToString(CultureInfo.InvariantCulture)
                    + " -> "
                    + result.ToString(CultureInfo.InvariantCulture);
            }

            UpdateThresholdMarker();
        }

        private void UpdateBrightnessGuide()
        {
            int offset = (int)Math.Round(brightnessOffsetSlider.Value);
            txtBrightnessOffset.Text = "Offset = " + FormatSigned(offset) + " GV";
            txtBrightnessFormula.Text = "Result GV = clamp(Source GV " + FormatSigned(offset) + ", 0, 255)";

            int[] bins = new int[8];
            for (int i = 0; i < brightnessSampleValues.Length; i++)
            {
                int result = ClampToByte(brightnessSampleValues[i] + offset);
                int bin = Math.Min(7, result / 32);
                bins[bin]++;
            }

            PaintBrightnessAnimationFrame(offset, bins);
        }

        private void PaintBrightnessAnimationFrame(int offset, int[] bins)
        {
            int visibleStep = Math.Max(0, Math.Min(brightnessAnimationStep, BrightnessAnimationStepCount));
            Brush defaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
            for (int i = 0; i < brightnessSampleValues.Length; i++)
            {
                int source = brightnessSampleValues[i];
                int result = ClampToByte(source + offset);
                brightnessInputCells[i].BorderBrush = visibleStep == 1 ? animationCandidateBrush : defaultBorderBrush;
                brightnessInputCells[i].BorderThickness = visibleStep == 1 ? new Thickness(2) : new Thickness(1);
                brightnessOutputCells[i].BorderBrush = visibleStep >= 2
                    ? visibleStep >= 3 ? animationPassBrush : animationCandidateBrush
                    : defaultBorderBrush;
                brightnessOutputCells[i].BorderThickness = visibleStep >= 2 ? new Thickness(2) : new Thickness(1);
                brightnessOutputCells[i].Background = visibleStep >= 2 ? CreateGrayBrush(result) : animationNeutralBrush;
                brightnessOutputTexts[i].Foreground = visibleStep >= 2 && result > 128 ? Brushes.Black : Brushes.White;
                brightnessOutputTexts[i].Text = visibleStep >= 2
                    ? source.ToString(CultureInfo.InvariantCulture) + " -> " + result.ToString(CultureInfo.InvariantCulture)
                    : "-";
            }

            int maxCount = Math.Max(1, bins.Max());
            for (int i = 0; i < histogramBars.Count; i++)
            {
                int low = i * 32;
                int high = i == 7 ? 255 : low + 31;
                histogramBars[i].Height = visibleStep >= 3
                    ? bins[i] == 0 ? 4D : 120D * bins[i] / maxCount
                    : 4D;
                histogramBars[i].Background = visibleStep >= 3 ? animationPassBrush : animationNeutralBrush;
                histogramLabels[i].Text = low.ToString(CultureInfo.InvariantCulture)
                    + "-"
                    + high.ToString(CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + (visibleStep >= 3 ? bins[i].ToString(CultureInfo.InvariantCulture) : "-");
            }

            int sourceAverage = (int)Math.Round(brightnessSampleValues.Average());
            int resultAverage = (int)Math.Round(brightnessSampleValues.Average(value => ClampToByte(value + offset)));
            string direction = resultAverage > sourceAverage ? "오른쪽" : resultAverage < sourceAverage ? "왼쪽" : "같은 위치";
            txtBrightnessAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - Reset: 원본 GV부터 확인합니다.",
                1 => "1 / 3 - Input GV: 각 픽셀의 밝기 값을 읽습니다.",
                2 => "2 / 3 - Brightness: GV " + FormatSigned(offset) + " 후 0~255로 제한합니다.",
                _ => "3 / 3 - Histogram shift: 평균 GV "
                    + sourceAverage.ToString(CultureInfo.InvariantCulture)
                    + " -> "
                    + resultAverage.ToString(CultureInfo.InvariantCulture)
                    + ", 분포가 "
                    + direction
                    + "으로 이동합니다."
            };
        }

        private void ResetBrightnessAnimation()
        {
            brightnessAnimationTimer.Stop();
            btnBrightnessPlay.Content = "Play";
            brightnessAnimationStep = 0;
            UpdateBrightnessGuide();
        }

        private void AdvanceBrightnessAnimation()
        {
            if (brightnessAnimationStep >= BrightnessAnimationStepCount)
            {
                brightnessAnimationStep = 0;
            }

            brightnessAnimationStep++;
            UpdateBrightnessGuide();
            if (brightnessAnimationStep >= BrightnessAnimationStepCount)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
            }
        }

        private void UpdateArithmeticGuide()
        {
            string mode = GetSelectedArithmeticMode();
            txtArithmeticFormula.Text = mode + ": A/B -> Output";
            txtArithmeticMeaning.Text = mode switch
            {
                "Add" => "Add는 두 Layer 값을 더합니다. 결과가 255를 넘으면 255로 제한됩니다.",
                "Subtract" => "Subtract는 A에서 B를 빼며 0보다 작은 결과는 0으로 제한됩니다.",
                "Bitwise AND" => "Bitwise AND는 두 이진 마스크에서 모두 흰색인 픽셀만 남깁니다.",
                "Bitwise OR" => "Bitwise OR는 두 이진 마스크 중 하나라도 흰색인 픽셀을 남깁니다.",
                _ => "AbsDiff는 A와 B의 절대 차이를 계산해 달라진 픽셀을 강조합니다."
            };

            PaintArithmeticAnimationFrame(mode);
        }

        private void PaintArithmeticAnimationFrame(string mode)
        {
            int visibleStep = Math.Max(0, Math.Min(arithmeticAnimationStep, ArithmeticAnimationStepCount));
            Brush defaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));

            for (int i = 0; i < arithmeticResultCells.Count; i++)
            {
                int inputA = arithmeticInputAValues[i];
                int inputB = arithmeticInputBValues[i];
                int result = CalculateArithmeticResult(mode, inputA, inputB);
                bool showInputs = visibleStep == 1 || visibleStep == 2;
                arithmeticInputACells[i].BorderBrush = showInputs ? animationCandidateBrush : defaultBorderBrush;
                arithmeticInputBCells[i].BorderBrush = showInputs ? animationCandidateBrush : defaultBorderBrush;
                arithmeticInputACells[i].BorderThickness = showInputs ? new Thickness(2) : new Thickness(1);
                arithmeticInputBCells[i].BorderThickness = showInputs ? new Thickness(2) : new Thickness(1);

                arithmeticResultCells[i].BorderBrush = visibleStep >= 2
                    ? visibleStep >= 3 ? animationPassBrush : animationCandidateBrush
                    : defaultBorderBrush;
                arithmeticResultCells[i].BorderThickness = visibleStep >= 2 ? new Thickness(2) : new Thickness(1);
                arithmeticResultCells[i].Background = visibleStep >= 2 ? CreateGrayBrush(result) : animationNeutralBrush;
                arithmeticResultTexts[i].Foreground = visibleStep >= 2 && result > 128 ? Brushes.Black : Brushes.White;
                arithmeticResultTexts[i].Text = visibleStep >= 2
                    ? result.ToString(CultureInfo.InvariantCulture)
                    : "-";
            }

            int firstResult = CalculateArithmeticResult(mode, arithmeticInputAValues[0], arithmeticInputBValues[0]);
            string firstExpression = mode switch
            {
                "Add" => "clamp(20 + 10) = " + firstResult.ToString(CultureInfo.InvariantCulture),
                "Subtract" => "clamp(20 - 10) = " + firstResult.ToString(CultureInfo.InvariantCulture),
                "Bitwise AND" => "20 & 10 = " + firstResult.ToString(CultureInfo.InvariantCulture),
                "Bitwise OR" => "20 | 10 = " + firstResult.ToString(CultureInfo.InvariantCulture),
                _ => "|20 - 10| = " + firstResult.ToString(CultureInfo.InvariantCulture)
            };
            txtArithmeticAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - Reset: InputLayer A/B를 먼저 확인합니다.",
                1 => "1 / 3 - Inputs: 8개 A/B 픽셀 쌍을 읽습니다.",
                2 => "2 / 3 - " + mode + " 적용: " + firstExpression,
                _ => "3 / 3 - OutputLayer: 8개 연산 결과를 비교할 준비가 됐습니다."
            };
        }

        private static int CalculateArithmeticResult(string mode, int inputA, int inputB)
        {
            return mode switch
            {
                "Add" => ClampToByte(inputA + inputB),
                "Subtract" => ClampToByte(inputA - inputB),
                "Bitwise AND" => inputA & inputB,
                "Bitwise OR" => inputA | inputB,
                _ => Math.Abs(inputA - inputB)
            };
        }

        private void ResetArithmeticAnimation()
        {
            arithmeticAnimationTimer.Stop();
            btnArithmeticPlay.Content = "Play";
            arithmeticAnimationStep = 0;
            UpdateArithmeticGuide();
        }

        private void AdvanceArithmeticAnimation()
        {
            if (arithmeticAnimationStep >= ArithmeticAnimationStepCount)
            {
                arithmeticAnimationStep = 0;
            }

            arithmeticAnimationStep++;
            UpdateArithmeticGuide();
            if (arithmeticAnimationStep >= ArithmeticAnimationStepCount)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
            }
        }

        private void UpdateFilterGuide()
        {
            string mode = GetSelectedFilterMode();
            int center = filterSampleValues[4];
            int result = mode switch
            {
                "Median" => filterSampleValues.OrderBy(value => value).ElementAt(4),
                "Sharpen" => ClampToByte(center * 5 - filterSampleValues[1] - filterSampleValues[3] - filterSampleValues[5] - filterSampleValues[7]),
                _ => ClampToByte(filterSampleValues.Average())
            };

            txtFilterFormula.Text = mode switch
            {
                "Median" => "Center = median(3x3 values) = " + result.ToString(CultureInfo.InvariantCulture),
                "Sharpen" => "Center = clamp(center x 5 - up - left - right - down) = " + result.ToString(CultureInfo.InvariantCulture),
                _ => "Center = average(3x3 values) = " + result.ToString(CultureInfo.InvariantCulture)
            };
            txtFilterMeaning.Text = mode switch
            {
                "Median" => "미디언은 정렬한 값의 가운데 값을 사용합니다. 작은 먼지나 점 노이즈처럼 튀는 값을 줄일 때 유리합니다.",
                "Sharpen" => "샤프닝은 중심 픽셀과 주변 픽셀 차이를 키웁니다. 흐릿한 경계를 강조하지만 노이즈도 같이 커질 수 있습니다.",
                _ => "평균 블러는 주변 값을 평균내서 부드럽게 만듭니다. 랜덤 노이즈는 줄지만 경계도 함께 흐려질 수 있습니다."
            };

            PaintFilterAnimationFrame(mode, result);
        }

        private void PaintFilterAnimationFrame(string mode, int result)
        {
            int visibleStep = Math.Max(0, Math.Min(filterAnimationStep, FilterAnimationStepCount));
            Brush defaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));

            for (int i = 0; i < filterSampleValues.Length; i++)
            {
                bool isCenter = i == 4;
                bool isKernelStep = visibleStep == 1;
                bool isCalculationStep = visibleStep == 2;
                int outputValue = isCenter ? result : filterSampleValues[i];

                filterInputCells[i].BorderBrush = isCalculationStep && isCenter
                    ? animationWarningBrush
                    : isKernelStep ? animationCandidateBrush : defaultBorderBrush;
                filterInputCells[i].BorderThickness = isKernelStep || isCalculationStep && isCenter
                    ? new Thickness(2)
                    : new Thickness(1);

                filterOutputCells[i].BorderBrush = isCenter && visibleStep >= 2
                    ? visibleStep >= 3 ? animationPassBrush : animationCandidateBrush
                    : defaultBorderBrush;
                filterOutputCells[i].BorderThickness = isCenter && visibleStep >= 2
                    ? new Thickness(2)
                    : new Thickness(1);
                filterOutputCells[i].Background = visibleStep >= 3
                    ? CreateGrayBrush(outputValue)
                    : isCalculationStep && isCenter ? animationCandidateBrush : animationNeutralBrush;
                filterOutputTexts[i].Foreground = visibleStep >= 3 && outputValue > 128 ? Brushes.Black : Brushes.White;
                filterOutputTexts[i].Text = visibleStep >= 3
                    ? outputValue.ToString(CultureInfo.InvariantCulture)
                    : isCalculationStep && isCenter ? "..." : "-";
            }

            string calculation = mode switch
            {
                "Median" => "Sort: " + string.Join(", ", filterSampleValues.OrderBy(value => value)),
                "Sharpen" => "220 x 5 - 58 - 60 - 65 - 62",
                _ => "Mean: " + filterSampleValues.Sum().ToString(CultureInfo.InvariantCulture) + " / 9"
            };
            string outputMeaning = mode switch
            {
                "Median" => "220 노이즈를 중앙값 " + result.ToString(CultureInfo.InvariantCulture) + "로 바꿉니다.",
                "Sharpen" => "중심 GV가 " + result.ToString(CultureInfo.InvariantCulture) + "로 커져 경계와 노이즈가 함께 강조됩니다.",
                _ => "중심 220을 평균 " + result.ToString(CultureInfo.InvariantCulture) + "로 낮춰 밝은 노이즈를 완화합니다."
            };
            txtFilterAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - Reset: 원본 3x3 GV부터 확인합니다.",
                1 => "1 / 3 - Kernel: 중심 220 주변의 9개 GV를 수집합니다.",
                2 => "2 / 3 - Calculate: " + calculation,
                _ => "3 / 3 - Output: " + outputMeaning
            };
        }

        private void ResetFilterAnimation()
        {
            filterAnimationTimer.Stop();
            btnFilterPlay.Content = "Play";
            filterAnimationStep = 0;
            UpdateFilterGuide();
        }

        private void AdvanceFilterAnimation()
        {
            if (filterAnimationStep >= FilterAnimationStepCount)
            {
                filterAnimationStep = 0;
            }

            filterAnimationStep++;
            UpdateFilterGuide();
            if (filterAnimationStep >= FilterAnimationStepCount)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
            }
        }

        private void UpdateFoundationGuide()
        {
            int visibleStep = Math.Max(0, Math.Min(foundationAnimationStep, FoundationAnimationStepCount));
            bool showAxisAlignedRoi = visibleStep >= 2 && visibleStep <= 4;
            bool showRotatedRect = visibleStep >= 5;
            foundationSelectedCellCount = visibleStep >= 3 && visibleStep <= 4 ? 12 : 0;
            for (int i = 0; i < foundationMatCells.Count; i++)
            {
                int x = i % 8;
                int y = i / 8;
                bool inRoi = x >= 2 && x < 6 && y >= 1 && y < 4;
                foundationMatCells[i].Background = foundationSelectedCellCount > 0 && inRoi
                    ? visibleStep >= 4 ? animationPassBrush : animationCandidateBrush
                    : visibleStep == 1 && x == 2 && y == 1
                        ? animationWarningBrush
                        : new SolidColorBrush(Color.FromRgb(248, 250, 252));
            }

            foundationPointMarker.Visibility = visibleStep >= 1 && visibleStep <= 4 ? Visibility.Visible : Visibility.Collapsed;
            foundationPointMarker.Fill = visibleStep >= 4 ? animationPassBrush : visibleStep == 1 ? animationWarningBrush : animationCandidateBrush;
            foundationRoiRect.Visibility = showAxisAlignedRoi ? Visibility.Visible : Visibility.Collapsed;
            foundationRoiRect.Stroke = visibleStep >= 4 ? animationPassBrush : animationCandidateBrush;
            foundationRotatedRect.Visibility = showRotatedRect ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedRect.Stroke = animationPassBrush;
            foundationRotatedBoundsRect.Visibility = showRotatedRect ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedBoundsRect.Stroke = animationWarningBrush;
            foundationRotatedCenterMarker.Visibility = showRotatedRect ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedCenterMarker.Fill = animationCandidateBrush;
            txtFoundationAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 5 - Mat: 6행 x 8열의 픽셀 격자입니다.",
                1 => "1 / 5 - Point(2,1): X=2 열, Y=1 행의 위치입니다.",
                2 => "2 / 5 - Size(4,3): Width=4 열, Height=3 행이며 위치 정보는 없습니다.",
                3 => "3 / 5 - Rect(2,1,4,3): Point와 Size를 결합하면 CvROI=2,1,4,3이 됩니다.",
                4 => "4 / 5 - Mat ROI: CvROI(2,1,4,3)가 선택한 12개 픽셀을 별도 Mat 영역으로 보여줍니다.",
                _ => "5 / 5 - RotatedRect: Center(4,2.5), Size(4,3), Angle(25°). 점선은 회전 영역 전체를 감싸는 BoundingRect입니다."
            };
        }

        private void ResetFoundationAnimation()
        {
            foundationAnimationTimer.Stop();
            btnFoundationPlay.Content = "자동 재생";
            foundationAnimationStep = 0;
            UpdateFoundationGuide();
        }

        private void UpdateMatChannelGuide()
        {
            int visibleStep = Math.Max(0, Math.Min(matChannelAnimationStep, MatChannelAnimationStepCount));
            Brush defaultBorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));

            matChannelBgrCell.Opacity = 1D;
            matChannelBgrCell.BorderBrush = visibleStep == 1 ? animationWarningBrush : defaultBorderBrush;
            matChannelBlueCell.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            matChannelGreenCell.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            matChannelRedCell.Opacity = visibleStep >= 2 ? 1D : 0.28D;
            matChannelBlueCell.BorderBrush = visibleStep == 2 ? animationCandidateBrush : defaultBorderBrush;
            matChannelGreenCell.BorderBrush = visibleStep == 2 ? animationCandidateBrush : defaultBorderBrush;
            matChannelRedCell.BorderBrush = visibleStep == 2 ? animationCandidateBrush : defaultBorderBrush;
            matChannelGrayCell.Opacity = visibleStep >= 3 ? 1D : 0.28D;
            matChannelGrayCell.BorderBrush = visibleStep >= 3 ? animationPassBrush : defaultBorderBrush;
            txtMatChannelBgrShape.Opacity = visibleStep >= 2 ? 1D : 0.45D;
            txtMatChannelGrayShape.Opacity = visibleStep >= 3 ? 1D : 0.45D;
            matChannelTypeGuidePanel.Opacity = visibleStep >= 4 ? 1D : 0.28D;

            txtMatChannelAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 4 - 한 픽셀은 Mat의 행, 열, 채널 위치에 저장됩니다.",
                1 => "1 / 4 - BGR 원본: OpenCV 채널 순서는 B, G, R이며 값은 220, 110, 40입니다.",
                2 => "2 / 4 - 채널 분리: B=220, G=110, R=40이며 BGR Mat 크기는 행 x 열 x 3입니다.",
                3 => "3 / 4 - Gray 변환: 0.114B + 0.587G + 0.299R = 102이며 Gray Mat 크기는 행 x 열 x 1입니다.",
                _ => "4 / 4 - Mat 형식: CV_8U는 0~255의 8비트 값이고 C1/C3은 채널 수입니다. Gray=CV_8UC1, BGR=CV_8UC3."
            };
        }

        private void ResetMatChannelAnimation()
        {
            matChannelAnimationTimer.Stop();
            btnMatChannelPlay.Content = "자동 재생";
            matChannelAnimationStep = 0;
            UpdateMatChannelGuide();
        }

        private void AdvanceMatChannelAnimation()
        {
            if (matChannelAnimationStep >= MatChannelAnimationStepCount)
            {
                matChannelAnimationStep = 0;
            }

            matChannelAnimationStep++;
            UpdateMatChannelGuide();
            if (matChannelAnimationStep >= MatChannelAnimationStepCount)
            {
                matChannelAnimationTimer.Stop();
                btnMatChannelPlay.Content = "자동 재생";
            }
        }

        private void AdvanceFoundationAnimation()
        {
            if (foundationAnimationStep >= FoundationAnimationStepCount)
            {
                foundationAnimationStep = 0;
            }

            foundationAnimationStep++;
            UpdateFoundationGuide();
            if (foundationAnimationStep >= FoundationAnimationStepCount)
            {
                foundationAnimationTimer.Stop();
                btnFoundationPlay.Content = "자동 재생";
            }
        }

        private void UpdateMorphologyGuide()
        {
            string mode = GetSelectedMorphologyMode();

            txtMorphologyFormula.Text = mode switch
            {
                "Dilation" => "Dilation: 3x3 이웃 중 하나라도 흰색이면 결과를 흰색으로 만듭니다.",
                "Opening" => "Opening: Erosion 후 Dilation을 적용합니다.",
                "Closing" => "Closing: Dilation 후 Erosion을 적용합니다.",
                _ => "Erosion: 3x3 이웃이 모두 흰색일 때만 결과를 흰색으로 만듭니다."
            };
            txtMorphologyMeaning.Text = mode switch
            {
                "Dilation" => "팽창은 흰 영역을 키웁니다. 끊어진 부분이나 작은 구멍을 메우는 데 도움이 되지만 대상이 두꺼워질 수 있습니다.",
                "Opening" => "열기는 침식 후 팽창입니다. 작은 흰 점 노이즈를 지운 뒤 원래 크기에 가깝게 되돌릴 때 씁니다.",
                "Closing" => "닫기는 팽창 후 침식입니다. 작은 검은 구멍이나 끊어진 틈을 메우는 데 씁니다.",
                _ => "침식은 흰 영역을 줄입니다. 작은 흰 점 노이즈를 제거하지만 얇은 대상은 사라질 수 있습니다."
            };

            morphologyAnimationTimer.Stop();
            btnMorphologyPlay.Content = "Play";
            morphologyAnimationStep = morphologySampleValues.Length;
            PaintMorphologyAnimationFrame();
        }

        private bool[] CalculateMorphologyResult()
        {
            bool[] source = morphologySampleValues.Select(value => value > 0).ToArray();
            return GetSelectedMorphologyMode() switch
            {
                "Dilation" => Dilate(source),
                "Opening" => Dilate(Erode(source)),
                "Closing" => Erode(Dilate(source)),
                _ => Erode(source)
            };
        }

        private void PaintMorphologyAnimationFrame()
        {
            bool[] result = CalculateMorphologyResult();
            SolidColorBrush defaultBorder = new(Color.FromRgb(209, 213, 219));
            SolidColorBrush kernelBorder = new(Color.FromRgb(21, 124, 134));
            SolidColorBrush centerBorder = new(Color.FromRgb(217, 119, 6));

            foreach (Border inputCell in morphologyInputCells)
            {
                inputCell.BorderBrush = defaultBorder;
                inputCell.BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < result.Length; i++)
            {
                bool processed = i < morphologyAnimationStep;
                int value = processed && result[i] ? 255 : processed ? 0 : 226;
                morphologyOutputCells[i].Background = CreateGrayBrush(value);
                morphologyOutputTexts[i].Foreground = processed
                    ? value > 0 ? Brushes.Black : Brushes.White
                    : new SolidColorBrush(Color.FromRgb(75, 85, 99));
                morphologyOutputTexts[i].Text = processed ? value > 0 ? "1" : "0" : ".";
            }

            if (morphologyAnimationStep > 0 && morphologyAnimationStep < result.Length)
            {
                int active = morphologyAnimationStep - 1;
                int x = active % 5;
                int y = active / 5;
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || nx >= 5 || ny < 0 || ny >= 5)
                        {
                            continue;
                        }

                        Border cell = morphologyInputCells[ny * 5 + nx];
                        cell.BorderBrush = kernelBorder;
                        cell.BorderThickness = new Thickness(2);
                    }
                }

                morphologyInputCells[active].BorderBrush = centerBorder;
                morphologyInputCells[active].BorderThickness = new Thickness(3);
                txtMorphologyAnimationStatus.Text = $"커널 중심 ({x + 1}, {y + 1}) · {morphologyAnimationStep} / {result.Length}";
                return;
            }

            txtMorphologyAnimationStatus.Text = morphologyAnimationStep == 0
                ? "준비 · 외곽 밖은 검정(0)으로 계산"
                : "전체 결과 · Play 또는 Step으로 커널 이동 확인";
        }

        private void ResetMorphologyAnimation()
        {
            morphologyAnimationTimer.Stop();
            btnMorphologyPlay.Content = "Play";
            morphologyAnimationStep = 0;
            PaintMorphologyAnimationFrame();
        }

        private void AdvanceMorphologyAnimation()
        {
            if (morphologyAnimationStep >= morphologySampleValues.Length)
            {
                morphologyAnimationStep = 0;
            }

            morphologyAnimationStep++;
            PaintMorphologyAnimationFrame();
            if (morphologyAnimationStep >= morphologySampleValues.Length)
            {
                morphologyAnimationTimer.Stop();
                btnMorphologyPlay.Content = "Play";
            }
        }

        private void UpdateBlobGuide()
        {
            int minArea = Math.Max(1, (int)Math.Round(blobMinAreaSlider.Value));
            (int[] labels, int[] areas) = LabelConnectedBlobs(blobSampleValues, 6, 5);
            int acceptedCount = areas.Count(area => area >= minArea);
            string areaText = string.Join(
                ", ",
                areas.Select((area, index) => ((char)('A' + index)).ToString() + "=" + area.ToString(CultureInfo.InvariantCulture)));

            txtBlobMinArea.Text = minArea.ToString(CultureInfo.InvariantCulture) + " px";
            txtBlobFormula.Text = "ResultCount = " + acceptedCount.ToString(CultureInfo.InvariantCulture)
                + " / Areas: "
                + areaText;
            txtBlobMeaning.Text = "MIN_AREA보다 작은 후보는 먼지나 노이즈로 보고 제외합니다. 지금 설정에서는 면적 "
                + minArea.ToString(CultureInfo.InvariantCulture)
                + " px 이상인 Blob만 통과합니다.";

            PaintBlobAnimationFrame(labels, areas, minArea);
        }

        private void PaintBlobAnimationFrame()
        {
            int minArea = Math.Max(1, (int)Math.Round(blobMinAreaSlider.Value));
            (int[] labels, int[] areas) = LabelConnectedBlobs(blobSampleValues, 6, 5);
            PaintBlobAnimationFrame(labels, areas, minArea);
        }

        private void PaintBlobAnimationFrame(int[] labels, int[] areas, int minArea)
        {
            int visibleCount = Math.Max(0, Math.Min(blobAnimationStep, areas.Length));
            for (int i = 0; i < blobInputCells.Count; i++)
            {
                Border inputCell = blobInputCells[i];
                inputCell.BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                inputCell.BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < labels.Length; i++)
            {
                int label = labels[i];
                if (label == 0)
                {
                    PaintBlobCell(i, Brushes.Black, Brushes.White, "0");
                    continue;
                }

                int area = areas[label - 1];
                bool accepted = area >= minArea;
                bool visible = label <= visibleCount;
                if (!visible)
                {
                    PaintBlobCell(i, new SolidColorBrush(Color.FromRgb(31, 41, 55)), Brushes.White, ".");
                    continue;
                }

                Brush background = CreateBlobDecisionBrush(label, accepted);
                string text = accepted ? ((char)('A' + label - 1)).ToString() : "x";
                PaintBlobCell(i, background, Brushes.White, text);
                if (i < blobInputCells.Count)
                {
                    blobInputCells[i].BorderBrush = background;
                    blobInputCells[i].BorderThickness = label == visibleCount ? new Thickness(3) : new Thickness(2);
                }
            }

            if (areas.Length == 0)
            {
                txtBlobAnimationStatus.Text = "Blob 후보 없음";
            }
            else if (visibleCount == 0)
            {
                txtBlobAnimationStatus.Text = "초기 상태 · 후보 "
                    + areas.Length.ToString(CultureInfo.InvariantCulture)
                    + "개를 차례로 확인합니다.";
            }
            else if (visibleCount >= areas.Length)
            {
                int acceptedCount = areas.Count(area => area >= minArea);
                txtBlobAnimationStatus.Text = "완료 · 통과 "
                    + acceptedCount.ToString(CultureInfo.InvariantCulture)
                    + " / 전체 "
                    + areas.Length.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                int label = visibleCount;
                int area = areas[label - 1];
                string labelName = ((char)('A' + label - 1)).ToString();
                txtBlobAnimationStatus.Text = "후보 "
                    + labelName
                    + " · 면적 "
                    + area.ToString(CultureInfo.InvariantCulture)
                    + " px · "
                    + (area >= minArea ? "통과" : "제외")
                    + " · "
                    + visibleCount.ToString(CultureInfo.InvariantCulture)
                    + " / "
                    + areas.Length.ToString(CultureInfo.InvariantCulture);
            }
        }

        private int GetBlobCandidateCount()
        {
            return LabelConnectedBlobs(blobSampleValues, 6, 5).Areas.Length;
        }

        private Brush CreateBlobDecisionBrush(int label, bool accepted)
        {
            if (!accepted)
            {
                return animationNeutralBrush;
            }

            return label == 1 ? animationCandidateBrush : animationPassBrush;
        }

        private void ResetBlobAnimation()
        {
            blobAnimationTimer.Stop();
            btnBlobPlay.Content = "Play";
            blobAnimationStep = 0;
            PaintBlobAnimationFrame();
        }

        private void AdvanceBlobAnimation()
        {
            int count = GetBlobCandidateCount();
            if (count <= 0)
            {
                blobAnimationStep = 0;
                blobAnimationTimer.Stop();
                btnBlobPlay.Content = "Play";
                PaintBlobAnimationFrame();
                return;
            }

            if (blobAnimationStep >= count)
            {
                blobAnimationStep = 0;
            }

            blobAnimationStep++;
            PaintBlobAnimationFrame();
            if (blobAnimationStep >= count)
            {
                blobAnimationTimer.Stop();
                btnBlobPlay.Content = "Play";
            }
        }

        private void UpdateContourGuide()
        {
            string mode = GetSelectedContourDrawMode();
            (int[] labels, int[] areas) = LabelConnectedBlobs(contourSampleValues, 7, 5);
            bool[] accepted = labels.Select(label => label > 0 && areas[label - 1] >= 4).ToArray();
            bool[] contour = FindContourPixels(accepted, 7, 5);
            (int MinX, int MinY, int MaxX, int MaxY)? bounds = FindBounds(accepted, 7, 5);
            int contourPixels = contour.Count(item => item);

            txtContourFormula.Text = mode switch
            {
                "Bounding box" when bounds.HasValue => "BoundingBox = x"
                    + bounds.Value.MinX.ToString(CultureInfo.InvariantCulture)
                    + ", y"
                    + bounds.Value.MinY.ToString(CultureInfo.InvariantCulture)
                    + ", w"
                    + (bounds.Value.MaxX - bounds.Value.MinX + 1).ToString(CultureInfo.InvariantCulture)
                    + ", h"
                    + (bounds.Value.MaxY - bounds.Value.MinY + 1).ToString(CultureInfo.InvariantCulture),
                "Contour + box" => "Contour pixels = "
                    + contourPixels.ToString(CultureInfo.InvariantCulture)
                    + ", with BoundingBox",
                _ => "Contour pixels = " + contourPixels.ToString(CultureInfo.InvariantCulture)
            };
            txtContourMeaning.Text = mode switch
            {
                "Bounding box" => "Bounding box는 후보의 대략 위치와 폭/높이를 빠르게 봅니다. 실제 모양 결함은 외곽선과 같이 확인해야 합니다.",
                "Contour + box" => "외곽선과 박스를 같이 보면 모양 결함과 크기/위치 차이를 한 화면에서 비교할 수 있습니다.",
                _ => "Contour는 통과한 후보의 경계 픽셀만 강조합니다. 끊김, 찌그러짐, 튀어나온 부분을 볼 때 유리합니다."
            };

            PaintContourAnimationFrame(labels, accepted, contour, bounds, mode);
        }

        private void PaintContourAnimationFrame()
        {
            string mode = GetSelectedContourDrawMode();
            (int[] labels, int[] areas) = LabelConnectedBlobs(contourSampleValues, 7, 5);
            bool[] accepted = labels.Select(label => label > 0 && areas[label - 1] >= 4).ToArray();
            bool[] contour = FindContourPixels(accepted, 7, 5);
            (int MinX, int MinY, int MaxX, int MaxY)? bounds = FindBounds(accepted, 7, 5);
            PaintContourAnimationFrame(labels, accepted, contour, bounds, mode);
        }

        private void PaintContourAnimationFrame(
            int[] labels,
            bool[] accepted,
            bool[] contour,
            (int MinX, int MinY, int MaxX, int MaxY)? bounds,
            string mode)
        {
            int visibleStep = Math.Max(0, Math.Min(contourAnimationStep, ContourAnimationStepCount));
            Brush rejectedBrush = animationNeutralBrush;
            Brush acceptedBrush = new SolidColorBrush(Color.FromRgb(229, 244, 247));
            Brush contourBrush = animationCandidateBrush;
            Brush boxBrush = animationPassBrush;

            for (int i = 0; i < contourInputCells.Count; i++)
            {
                contourInputCells[i].BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                contourInputCells[i].BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < contourSampleValues.Length; i++)
            {
                int label = labels[i];
                bool isRejected = label > 0 && !accepted[i];
                bool isContour = contour[i];
                bool isBox = bounds.HasValue && IsOnBounds(i, 7, bounds.Value);

                if (label > 0 && visibleStep > 0 && i < contourInputCells.Count)
                {
                    contourInputCells[i].BorderBrush = isRejected ? rejectedBrush : contourBrush;
                    contourInputCells[i].BorderThickness = new Thickness(2);
                }

                if (visibleStep == 0)
                {
                    PaintContourCell(i, Brushes.Black, Brushes.White, "0");
                    continue;
                }

                if (isRejected)
                {
                    PaintContourCell(i, rejectedBrush, Brushes.White, "x");
                    continue;
                }

                if (!accepted[i])
                {
                    PaintContourCell(i, Brushes.Black, Brushes.White, "0");
                    continue;
                }

                if (visibleStep == 1)
                {
                    PaintContourCell(i, acceptedBrush, Brushes.Black, "1");
                    continue;
                }

                if (visibleStep == 2)
                {
                    PaintContourCell(i, isContour ? contourBrush : acceptedBrush, isContour ? Brushes.White : Brushes.Black, isContour ? "C" : "1");
                    continue;
                }

                PaintContourFinalCell(i, mode, isContour, isBox, acceptedBrush, contourBrush, boxBrush);
            }

            txtContourAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - 이진 입력에서 연결 영역을 확인합니다.",
                1 => "1 / 3 - 면적 기준으로 작은 연결 영역을 제외합니다.",
                2 => "2 / 3 - 통과한 영역의 경계 픽셀을 표시합니다.",
                _ => "3 / 3 - 표시 방식: " + mode
            };
        }

        private void PaintContourFinalCell(
            int index,
            string mode,
            bool isContour,
            bool isBox,
            Brush acceptedBrush,
            Brush contourBrush,
            Brush boxBrush)
        {
            bool drawContour = mode == "Contour" || mode == "Contour + box";
            bool drawBox = mode == "Bounding box" || mode == "Contour + box";

            if (mode == "Contour + box" && drawBox && isBox)
            {
                PaintContourCell(index, boxBrush, Brushes.White, "B");
            }
            else if (drawContour && isContour)
            {
                PaintContourCell(index, contourBrush, Brushes.White, "C");
            }
            else if (drawBox && isBox)
            {
                PaintContourCell(index, boxBrush, Brushes.White, "B");
            }
            else
            {
                PaintContourCell(index, acceptedBrush, Brushes.Black, "1");
            }
        }

        private void ResetContourAnimation()
        {
            contourAnimationTimer.Stop();
            btnContourPlay.Content = "Play";
            contourAnimationStep = 0;
            PaintContourAnimationFrame();
        }

        private void AdvanceContourAnimation()
        {
            if (contourAnimationStep >= ContourAnimationStepCount)
            {
                contourAnimationStep = 0;
            }

            contourAnimationStep++;
            PaintContourAnimationFrame();
            if (contourAnimationStep >= ContourAnimationStepCount)
            {
                contourAnimationTimer.Stop();
                btnContourPlay.Content = "Play";
            }
        }

        private void UpdateEdgeLineGuide()
        {
            int threshold = Math.Max(10, (int)Math.Round(edgeThresholdSlider.Value));
            bool[] edges = new bool[edgeLineSampleValues.Length];
            int[] strengths = new int[edgeLineSampleValues.Length];
            int bestColumn = 0;
            int bestRun = 0;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    int index = y * 5 + x;
                    int strength = x < 4
                        ? Math.Abs(edgeLineSampleValues[index + 1] - edgeLineSampleValues[index])
                        : 0;
                    strengths[index] = strength;
                    edges[index] = strength >= threshold;
                }
            }

            for (int x = 0; x < 5; x++)
            {
                int run = 0;
                for (int y = 0; y < 5; y++)
                {
                    int index = y * 5 + x;
                    run = edges[index] ? run + 1 : 0;
                    if (run > bestRun)
                    {
                        bestRun = run;
                        bestColumn = x;
                    }
                }
            }

            txtEdgeThreshold.Text = threshold.ToString(CultureInfo.InvariantCulture) + " GV";
            txtEdgeLineFormula.Text = "Edge = abs(right GV - left GV) >= "
                + threshold.ToString(CultureInfo.InvariantCulture)
                + ", LineRun = "
                + bestRun.ToString(CultureInfo.InvariantCulture)
                + " px";
            txtEdgeLineMeaning.Text = bestRun >= 3
                ? "같은 X 위치에서 Edge 후보가 3 px 이상 이어져 Line 후보로 볼 수 있습니다. 실제 검사는 ROI, 방향, 길이 조건을 같이 둡니다."
                : "Edge 기준이 너무 높으면 후보가 끊겨 Line으로 보기 어렵습니다. 기준값, ROI, 전처리 상태를 순서대로 확인합니다.";

            PaintEdgeLineAnimationFrame(threshold, strengths, edges, bestColumn, bestRun);
        }

        private void PaintEdgeLineAnimationFrame()
        {
            int threshold = Math.Max(10, (int)Math.Round(edgeThresholdSlider.Value));
            bool[] edges = new bool[edgeLineSampleValues.Length];
            int[] strengths = new int[edgeLineSampleValues.Length];
            int bestColumn = 0;
            int bestRun = 0;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    int index = y * 5 + x;
                    int strength = x < 4
                        ? Math.Abs(edgeLineSampleValues[index + 1] - edgeLineSampleValues[index])
                        : 0;
                    strengths[index] = strength;
                    edges[index] = strength >= threshold;
                }
            }

            for (int x = 0; x < 5; x++)
            {
                int run = 0;
                for (int y = 0; y < 5; y++)
                {
                    int index = y * 5 + x;
                    run = edges[index] ? run + 1 : 0;
                    if (run > bestRun)
                    {
                        bestRun = run;
                        bestColumn = x;
                    }
                }
            }

            PaintEdgeLineAnimationFrame(threshold, strengths, edges, bestColumn, bestRun);
        }

        private void PaintEdgeLineAnimationFrame(
            int threshold,
            int[] strengths,
            bool[] edges,
            int bestColumn,
            int bestRun)
        {
            int visibleStep = Math.Max(0, Math.Min(edgeLineAnimationStep, EdgeLineAnimationStepCount));
            Brush edgeBrush = animationCandidateBrush;
            Brush lineBrush = animationPassBrush;

            for (int i = 0; i < edgeLineInputCells.Count; i++)
            {
                edgeLineInputCells[i].BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                edgeLineInputCells[i].BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < edgeLineSampleValues.Length; i++)
            {
                int x = i % 5;
                if (visibleStep == 0)
                {
                    PaintEdgeLineCell(i, animationNeutralBrush, Brushes.White, "-");
                    continue;
                }

                int shade = Math.Min(220, strengths[i] + 35);
                if (visibleStep == 1)
                {
                    PaintEdgeLineCell(i, CreateGrayBrush(shade), shade > 128 ? Brushes.Black : Brushes.White, strengths[i].ToString(CultureInfo.InvariantCulture));
                    continue;
                }

                if (edges[i] && i < edgeLineInputCells.Count)
                {
                    edgeLineInputCells[i].BorderBrush = visibleStep >= 3 && x == bestColumn && bestRun >= 3 ? lineBrush : edgeBrush;
                    edgeLineInputCells[i].BorderThickness = new Thickness(2);
                }

                if (visibleStep >= 3 && edges[i] && x == bestColumn && bestRun >= 3)
                {
                    PaintEdgeLineCell(i, lineBrush, Brushes.White, "L");
                }
                else if (edges[i])
                {
                    PaintEdgeLineCell(i, edgeBrush, Brushes.White, "E");
                }
                else
                {
                    PaintEdgeLineCell(i, CreateGrayBrush(shade), shade > 128 ? Brushes.Black : Brushes.White, strengths[i].ToString(CultureInfo.InvariantCulture));
                }
            }

            txtEdgeLineAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - GV 샘플에서 밝기 변화를 확인합니다.",
                1 => "1 / 3 - Gradient: abs(오른쪽 GV - 왼쪽 GV)",
                2 => "2 / 3 - Edge: strength >= " + threshold.ToString(CultureInfo.InvariantCulture),
                _ => "3 / 3 - LineRun: best vertical chain = " + bestRun.ToString(CultureInfo.InvariantCulture) + " px"
            };
        }

        private void ResetEdgeLineAnimation()
        {
            edgeLineAnimationTimer.Stop();
            btnEdgeLinePlay.Content = "Play";
            edgeLineAnimationStep = 0;
            PaintEdgeLineAnimationFrame();
        }

        private void AdvanceEdgeLineAnimation()
        {
            if (edgeLineAnimationStep >= EdgeLineAnimationStepCount)
            {
                edgeLineAnimationStep = 0;
            }

            edgeLineAnimationStep++;
            PaintEdgeLineAnimationFrame();
            if (edgeLineAnimationStep >= EdgeLineAnimationStepCount)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
            }
        }

        private void UpdateLineDistanceGuide()
        {
            double rangeMax = lineDistanceRangeMaxSlider.Value;
            int[] distances = lineDistanceRightEdges
                .Select((right, index) => right - lineDistanceLeftEdges[index])
                .ToArray();
            double avg = distances.Average();
            int min = distances.Min();
            int max = distances.Max();
            int range = max - min;
            double pixelPerMm = 0.006D;
            double avgMm = avg * pixelPerMm;
            double rangeMm = range * pixelPerMm;
            double maxMm = max * pixelPerMm;
            bool rangeOk = range <= rangeMax;

            txtLineDistanceRangeMax.Text = rangeMax.ToString("0.00", CultureInfo.InvariantCulture) + " px";
            txtLineDistanceFormula.Text = "DistancePxAvg="
                + avg.ToString("0.0", CultureInfo.InvariantCulture)
                + ", DistancePxRange="
                + range.ToString(CultureInfo.InvariantCulture)
                + ", DistanceMmAvg="
                + avgMm.ToString("0.000", CultureInfo.InvariantCulture)
                + ", DistanceMmRange="
                + rangeMm.ToString("0.000", CultureInfo.InvariantCulture)
                + ", DistanceMmMax="
                + maxMm.ToString("0.000", CultureInfo.InvariantCulture);
            txtLineDistanceMeaning.Text = rangeOk
                ? "평균과 줄별 흔들림이 모두 기준 안입니다. 실제 레시피도 DistanceAvg와 DistanceRange를 함께 판정합니다."
                : "평균값만 보면 지나칠 수 있지만 줄별 거리 차이가 큽니다. Range/Max 게이트로 긴 측정선을 NG 처리해야 합니다.";

            PaintLineDistanceAnimationFrame(distances, avg, range, rangeMax, rangeOk);
        }

        private void PaintLineDistanceAnimationFrame()
        {
            double rangeMax = lineDistanceRangeMaxSlider.Value;
            int[] distances = lineDistanceRightEdges
                .Select((right, index) => right - lineDistanceLeftEdges[index])
                .ToArray();
            double avg = distances.Average();
            int range = distances.Max() - distances.Min();
            bool rangeOk = range <= rangeMax;
            PaintLineDistanceAnimationFrame(distances, avg, range, rangeMax, rangeOk);
        }

        private void PaintLineDistanceAnimationFrame(
            int[] distances,
            double avg,
            int range,
            double rangeMax,
            bool rangeOk)
        {
            int visibleStep = Math.Max(0, Math.Min(lineDistanceAnimationStep, LineDistanceAnimationStepCount));
            int max = distances.Max();
            Brush normalBrush = animationPassBrush;
            Brush sampleBrush = animationCandidateBrush;
            Brush outlierBrush = new SolidColorBrush(Color.FromRgb(185, 91, 36));

            for (int i = 0; i < lineDistanceInputCells.Count; i++)
            {
                lineDistanceInputCells[i].BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                lineDistanceInputCells[i].BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < distances.Length; i++)
            {
                if (visibleStep >= 1)
                {
                    Brush rowBrush = !rangeOk && distances[i] == max && visibleStep >= 3 ? outlierBrush : sampleBrush;
                    HighlightLineDistanceInputRow(i, rowBrush);
                }

                if (visibleStep == 0)
                {
                    PaintLineDistanceCell(i, animationNeutralBrush, Brushes.White, "-");
                }
                else if (visibleStep == 1)
                {
                    PaintLineDistanceCell(i, sampleBrush, Brushes.White, distances[i].ToString(CultureInfo.InvariantCulture) + " px");
                }
                else if (visibleStep == 2)
                {
                    PaintLineDistanceCell(i, sampleBrush, Brushes.White, "avg " + avg.ToString("0.0", CultureInfo.InvariantCulture));
                }
                else
                {
                    bool outlier = !rangeOk && distances[i] == max;
                    PaintLineDistanceCell(i, outlier ? outlierBrush : normalBrush, Brushes.White, distances[i].ToString(CultureInfo.InvariantCulture) + " px");
                }
            }

            txtLineDistanceAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - 각 스캔선의 왼쪽/오른쪽 edge 쌍을 확인합니다.",
                1 => "1 / 3 - 각 스캔선에서 Gap/Pitch를 측정합니다.",
                2 => "2 / 3 - Average: DistancePxAvg = " + avg.ToString("0.0", CultureInfo.InvariantCulture),
                _ => "3 / 3 - Range 판정: " + (rangeOk ? "OK" : "NG") + ", DistancePxRange = "
                    + range.ToString(CultureInfo.InvariantCulture)
                    + (rangeOk ? " <= " : " > ")
                    + rangeMax.ToString("0.00", CultureInfo.InvariantCulture)
            };
        }

        private void HighlightLineDistanceInputRow(int row, Brush borderBrush)
        {
            int start = row * 9;
            for (int i = start; i < start + 9 && i < lineDistanceInputCells.Count; i++)
            {
                lineDistanceInputCells[i].BorderBrush = borderBrush;
                lineDistanceInputCells[i].BorderThickness = new Thickness(2);
            }
        }

        private void ResetLineDistanceAnimation()
        {
            lineDistanceAnimationTimer.Stop();
            btnLineDistancePlay.Content = "Play";
            lineDistanceAnimationStep = 0;
            PaintLineDistanceAnimationFrame();
        }

        private void AdvanceLineDistanceAnimation()
        {
            if (lineDistanceAnimationStep >= LineDistanceAnimationStepCount)
            {
                lineDistanceAnimationStep = 0;
            }

            lineDistanceAnimationStep++;
            PaintLineDistanceAnimationFrame();
            if (lineDistanceAnimationStep >= LineDistanceAnimationStepCount)
            {
                lineDistanceAnimationTimer.Stop();
                btnLineDistancePlay.Content = "Play";
            }
        }

        private void UpdateMatchingGuide()
        {
            bool isEdgeBasedMatching = topicList.SelectedIndex == 12;
            double threshold = matchingThresholdSlider.Value;
            double[] scores = matchingCandidatePositions
                .Select(position => CalculateTemplateScore(position.X, position.Y))
                .ToArray();
            double bestScore = scores.Max();
            int bestIndex = Array.IndexOf(scores, bestScore);
            bool pass = bestScore >= threshold;

            txtMatchingThreshold.Text = threshold.ToString("0.00", CultureInfo.InvariantCulture);
            txtMatchingConceptTitle.Text = isEdgeBasedMatching
                ? "EdgeBasedMatching은 밝기보다 edge 형상으로 판정합니다"
                : "Matching은 작은 Template을 찾아 점수로 판정합니다";
            txtMatchingConceptDescription.Text = isEdgeBasedMatching
                ? "검색 이미지와 기준 Template에서 edge를 추출한 뒤, edge 점이 가장 잘 겹치는 위치를 찾습니다."
                : "Template Matching은 기준 모양을 검색 영역 위로 움직이며 가장 비슷한 위치와 Score를 찾습니다.";
            txtMatchingThresholdDescription.Text = isEdgeBasedMatching
                ? "Canny 기준이 너무 낮으면 배경 edge가 늘고, 너무 높으면 기준 형상의 edge가 끊겨 Score와 ResultCount가 불안정해집니다."
                : "ScoreThreshold가 낮으면 오검출이 늘고, 너무 높으면 조명이나 작은 흔들림에도 NG가 날 수 있습니다.";
            txtMatchingSearchTitle.Text = isEdgeBasedMatching ? "검색 edge map과 edge Template" : "검색 이미지와 Template";
            txtMatchingSearchSubtitle.Text = isEdgeBasedMatching
                ? "E는 추출 edge, S는 검색 후보, B는 최고 edge score 위치입니다."
                : "T는 기준 Template, B는 최고 점수 위치입니다.";
            txtMatchingScoreTitle.Text = isEdgeBasedMatching ? "Edge 후보 Score" : "후보 Score";
            btnMatchingPlay.ToolTip = isEdgeBasedMatching
                ? "edge 추출, 후보 점수 계산, 판정 기준 적용 순서를 보여줍니다."
                : "Template 검색, 후보 점수 계산, 판정 기준 적용 순서를 보여줍니다.";
            UpdateMatchingTemplateLabels(isEdgeBasedMatching);

            txtMatchingFormula.Text = (isEdgeBasedMatching ? "EdgeScoreMax=" : "BestScore=")
                + bestScore.ToString("0.00", CultureInfo.InvariantCulture)
                + ", Threshold="
                + threshold.ToString("0.00", CultureInfo.InvariantCulture)
                + ", Result="
                + (pass ? "OK" : "NG");
            txtMatchingMeaning.Text = isEdgeBasedMatching
                ? pass
                    ? "EdgeScoreMax가 기준 이상입니다. 실제 Run Review에서는 ResultCount와 overlay 위치가 대상 형상에 맞는지도 확인합니다."
                    : "Edge score가 부족합니다. Canny 기준, edge Template, ROI, ScoreThreshold 순서로 확인합니다."
                : pass
                    ? "최고 점수가 기준 이상이면 Template 위치 후보로 볼 수 있습니다. 회전/스케일 변화가 크면 EdgeBasedMatching이나 FeatureMatching을 검토합니다."
                    : "최고 점수가 기준보다 낮으면 NG입니다. Template, ROI, 조명, ScoreThreshold를 순서대로 확인합니다.";

            PaintMatchingAnimationFrame(scores, threshold, bestIndex, bestScore, pass);
        }

        private void PaintMatchingAnimationFrame()
        {
            double threshold = matchingThresholdSlider.Value;
            double[] scores = matchingCandidatePositions
                .Select(position => CalculateTemplateScore(position.X, position.Y))
                .ToArray();
            double bestScore = scores.Max();
            int bestIndex = Array.IndexOf(scores, bestScore);
            bool pass = bestScore >= threshold;
            PaintMatchingAnimationFrame(scores, threshold, bestIndex, bestScore, pass);
        }

        private void PaintMatchingAnimationFrame(
            double[] scores,
            double threshold,
            int bestIndex,
            double bestScore,
            bool pass)
        {
            bool isEdgeBasedMatching = topicList.SelectedIndex == 12;
            int visibleStep = Math.Max(0, Math.Min(matchingAnimationStep, MatchingAnimationStepCount));
            if (visibleStep == 0)
            {
                PaintMatchingSearchGrid(-1);
                for (int i = 0; i < scores.Length; i++)
                {
                    PaintMatchingScoreCell(i, animationNeutralBrush, Brushes.White, "-");
                }

                txtMatchingAnimationStatus.Text = isEdgeBasedMatching
                    ? "0 / 3 - 검색 이미지와 edge Template을 확인합니다."
                    : "0 / 3 - 검색 이미지와 Template을 확인합니다.";
                return;
            }

            if (visibleStep == 1)
            {
                PaintMatchingCandidateScanGrid();
                for (int i = 0; i < scores.Length; i++)
                {
                    PaintMatchingScoreCell(i, animationCandidateBrush, Brushes.White, scores[i].ToString("0.00", CultureInfo.InvariantCulture));
                }

                txtMatchingAnimationStatus.Text = isEdgeBasedMatching
                    ? "1 / 3 - 각 후보 위치에서 edge 점의 일치도를 계산합니다."
                    : "1 / 3 - 각 Template 후보 위치의 점수를 계산합니다.";
                return;
            }

            PaintMatchingSearchGrid(bestIndex);
            for (int i = 0; i < scores.Length; i++)
            {
                bool isBest = i == bestIndex;
                bool accepted = scores[i] >= threshold;
                Brush background;
                if (visibleStep == 2)
                {
                    background = isBest
                        ? animationPassBrush
                        : animationNeutralBrush;
                }
                else
                {
                    background = isBest && accepted
                        ? animationPassBrush
                        : accepted
                            ? animationCandidateBrush
                            : animationNeutralBrush;
                }

                PaintMatchingScoreCell(i, background, Brushes.White, scores[i].ToString("0.00", CultureInfo.InvariantCulture));
            }

            txtMatchingAnimationStatus.Text = visibleStep == 2
                ? (isEdgeBasedMatching ? "2 / 3 - EdgeScoreMax: " : "2 / 3 - BestScore: ")
                    + bestScore.ToString("0.00", CultureInfo.InvariantCulture)
                : (isEdgeBasedMatching ? "3 / 3 - Edge score 판정: " : "3 / 3 - Threshold 판정: ")
                    + (pass ? "OK" : "NG") + ", "
                    + (isEdgeBasedMatching ? "EdgeScoreMax " : "BestScore ")
                    + bestScore.ToString("0.00", CultureInfo.InvariantCulture)
                    + (pass ? " >= " : " < ")
                    + threshold.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void UpdateMatchingTemplateLabels(bool isEdgeBasedMatching)
        {
            int index = 0;
            foreach (Border cell in matchingTemplateGrid.Children.OfType<Border>())
            {
                if (cell.Child is TextBlock text && index < matchingTemplateValues.Length)
                {
                    text.Text = matchingTemplateValues[index] > 0
                        ? isEdgeBasedMatching ? "E" : "T"
                        : "0";
                }

                index++;
            }
        }

        private void ResetMatchingAnimation()
        {
            matchingAnimationTimer.Stop();
            btnMatchingPlay.Content = "Play";
            matchingAnimationStep = 0;
            PaintMatchingAnimationFrame();
        }

        private void AdvanceMatchingAnimation()
        {
            if (matchingAnimationStep >= MatchingAnimationStepCount)
            {
                matchingAnimationStep = 0;
            }

            matchingAnimationStep++;
            PaintMatchingAnimationFrame();
            if (matchingAnimationStep >= MatchingAnimationStepCount)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
            }
        }

        private void UpdateFeatureMatchingGuide()
        {
            int required = Math.Max(1, (int)Math.Round(featureGoodMatchMinSlider.Value));
            const double goodScoreThreshold = 0.65D;
            bool[] goodMatches = featureMatchScores
                .Select(score => score >= goodScoreThreshold)
                .ToArray();
            int goodCount = goodMatches.Count(item => item);
            bool pass = goodCount >= required;

            txtFeatureGoodMatchMin.Text = required.ToString(CultureInfo.InvariantCulture);
            txtFeatureMatchingFormula.Text = "GoodMatches="
                + goodCount.ToString(CultureInfo.InvariantCulture)
                + ", Required="
                + required.ToString(CultureInfo.InvariantCulture)
                + ", DescriptorScore>=0.65";
            txtFeatureMatchingMeaning.Text = pass
                ? "Good match가 충분하면 대상 후보로 볼 수 있습니다. 실제 검사는 match 위치 일관성과 결과 overlay를 같이 확인합니다."
                : "Good match 수가 부족하면 NG입니다. 특징점이 적거나 조명/초점/ROI가 흔들린 상태인지 먼저 확인합니다.";

            PaintFeatureMatchingAnimationFrame(goodMatches, goodCount, required, pass);
        }

        private void PaintFeatureMatchingAnimationFrame()
        {
            int required = Math.Max(1, (int)Math.Round(featureGoodMatchMinSlider.Value));
            bool[] goodMatches = featureMatchScores.Select(score => score >= 0.65D).ToArray();
            int goodCount = goodMatches.Count(item => item);
            PaintFeatureMatchingAnimationFrame(goodMatches, goodCount, required, goodCount >= required);
        }

        private void PaintFeatureMatchingAnimationFrame(
            bool[] goodMatches,
            int goodCount,
            int required,
            bool pass)
        {
            int visibleStep = Math.Max(0, Math.Min(featureMatchingAnimationStep, FeatureMatchingAnimationStepCount));
            bool[] detectedPoints = featureMatchScores.Select(_ => true).ToArray();
            bool[] ransacInliers = { true, true, true, true, false, false };

            if (visibleStep == 0)
            {
                PaintFeaturePointGrid(featureReferenceCells, featureReferenceTexts, featureReferencePoints, Array.Empty<bool>(), "K");
                PaintFeaturePointGrid(featureSceneCells, featureSceneTexts, featureScenePoints, Array.Empty<bool>(), "M");
                for (int i = 0; i < featureMatchScores.Length; i++)
                {
                    PaintFeatureScoreCell(i, animationNeutralBrush, Brushes.White, "-");
                }

                txtFeatureMatchingAnimationStatus.Text = "0 / 3 - Reference와 Scene 영상을 확인합니다.";
                return;
            }

            PaintFeaturePointGrid(
                featureReferenceCells,
                featureReferenceTexts,
                featureReferencePoints,
                visibleStep == 1 ? detectedPoints : visibleStep == 2 ? goodMatches : ransacInliers,
                "K");
            PaintFeaturePointGrid(
                featureSceneCells,
                featureSceneTexts,
                featureScenePoints,
                visibleStep == 1 ? detectedPoints : visibleStep == 2 ? goodMatches : ransacInliers,
                "M");

            for (int i = 0; i < featureMatchScores.Length; i++)
            {
                Brush background = visibleStep == 1
                    ? animationCandidateBrush
                    : visibleStep == 2
                        ? goodMatches[i]
                            ? animationPassBrush
                            : animationNeutralBrush
                        : ransacInliers[i]
                            ? animationPassBrush
                            : goodMatches[i]
                                ? animationWarningBrush
                                : animationNeutralBrush;
                PaintFeatureScoreCell(i, background, Brushes.White, featureMatchScores[i].ToString("0.00", CultureInfo.InvariantCulture));
            }

            txtFeatureMatchingAnimationStatus.Text = visibleStep switch
            {
                1 => "1 / 3 - Reference와 Scene에서 반복 검출 가능한 특징점을 찾습니다.",
                2 => "2 / 3 - Descriptor 기준: score >= 0.65인 Good Match " + goodCount.ToString(CultureInfo.InvariantCulture) + "개",
                _ => "3 / 3 - GoodMatches 판정: " + (pass ? "OK" : "NG")
                    + ", 검출 " + goodCount.ToString(CultureInfo.InvariantCulture) + "개"
                    + (pass ? " >= " : " < ") + required.ToString(CultureInfo.InvariantCulture)
                    + "; RANSAC과 overlay 위치도 함께 확인합니다."
            };
        }

        private void ResetFeatureMatchingAnimation()
        {
            featureMatchingAnimationTimer.Stop();
            btnFeatureMatchingPlay.Content = "Play";
            featureMatchingAnimationStep = 0;
            PaintFeatureMatchingAnimationFrame();
        }

        private void AdvanceFeatureMatchingAnimation()
        {
            if (featureMatchingAnimationStep >= FeatureMatchingAnimationStepCount)
            {
                featureMatchingAnimationStep = 0;
            }

            featureMatchingAnimationStep++;
            PaintFeatureMatchingAnimationFrame();
            if (featureMatchingAnimationStep >= FeatureMatchingAnimationStepCount)
            {
                featureMatchingAnimationTimer.Stop();
                btnFeatureMatchingPlay.Content = "Play";
            }
        }

        private void UpdateMetricsAcceptanceGuide()
        {
            double average = metricsAcceptanceSamples.Average();
            double range = metricsAcceptanceSamples.Max() - metricsAcceptanceSamples.Min();
            double maximum = metricsAcceptanceSamples.Max();
            PaintMetricsAcceptanceAnimationFrame(average, range, maximum);
        }

        private void PaintMetricsAcceptanceAnimationFrame()
        {
            PaintMetricsAcceptanceAnimationFrame(
                metricsAcceptanceSamples.Average(),
                metricsAcceptanceSamples.Max() - metricsAcceptanceSamples.Min(),
                metricsAcceptanceSamples.Max());
        }

        private void PaintMetricsAcceptanceAnimationFrame(double average, double range, double maximum)
        {
            const double averageMin = 0.45D;
            const double averageMax = 0.60D;
            const double rangeMax = 0.10D;
            const double valueMax = 0.65D;
            int visibleStep = Math.Max(0, Math.Min(metricsAcceptanceAnimationStep, MetricsAcceptanceAnimationStepCount));
            bool averageOk = average >= averageMin && average <= averageMax;
            bool rangeOk = range <= rangeMax;
            bool maximumOk = maximum <= valueMax;

            for (int i = 0; i < metricsAcceptanceSampleCells.Count; i++)
            {
                bool isOutlier = metricsAcceptanceSamples[i] > valueMax;
                Brush background = visibleStep == 0
                    ? animationNeutralBrush
                    : visibleStep < 3
                        ? animationNeutralBrush
                        : isOutlier
                            ? animationWarningBrush
                            : animationPassBrush;
                metricsAcceptanceSampleCells[i].Background = background;
                metricsAcceptanceSampleTexts[i].Foreground = Brushes.White;
                metricsAcceptanceSampleTexts[i].Text = visibleStep == 0
                    ? "-"
                    : metricsAcceptanceSamples[i].ToString("0.00", CultureInfo.InvariantCulture);
            }

            txtMetricsAcceptanceFormula.Text = visibleStep switch
            {
                0 => "Avg 0.45..0.60 | Range <= 0.10 | Max <= 0.65",
                1 => "Samples=5 | 측정값 5개를 모두 확인합니다.",
                2 => "DistanceMmAvg=" + average.ToString("0.00", CultureInfo.InvariantCulture) + " -> " + (averageOk ? "OK" : "NG"),
                _ => "Range=" + range.ToString("0.00", CultureInfo.InvariantCulture)
                    + " / Max=" + maximum.ToString("0.00", CultureInfo.InvariantCulture)
                    + " -> " + (rangeOk && maximumOk ? "OK" : "NG")
            };
            txtMetricsAcceptanceAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - 평균, 범위, 최대값 판정 기준을 확인합니다.",
                1 => "1 / 3 - Samples: 5개 측정값을 모두 확인합니다.",
                2 => "2 / 3 - 평균 판정: " + (averageOk ? "OK" : "NG") + "; 평균만 보면 통과합니다.",
                _ => "3 / 3 - 범위/최대값 판정: " + (rangeOk && maximumOk ? "OK" : "NG")
                    + "; 0.82 mm 이상치를 검출합니다."
            };
        }

        private void ResetMetricsAcceptanceAnimation()
        {
            metricsAcceptanceAnimationTimer.Stop();
            btnMetricsAcceptancePlay.Content = "Play";
            metricsAcceptanceAnimationStep = 0;
            PaintMetricsAcceptanceAnimationFrame();
        }

        private void AdvanceMetricsAcceptanceAnimation()
        {
            if (metricsAcceptanceAnimationStep >= MetricsAcceptanceAnimationStepCount)
            {
                metricsAcceptanceAnimationStep = 0;
            }

            metricsAcceptanceAnimationStep++;
            PaintMetricsAcceptanceAnimationFrame();
            if (metricsAcceptanceAnimationStep >= MetricsAcceptanceAnimationStepCount)
            {
                metricsAcceptanceAnimationTimer.Stop();
                btnMetricsAcceptancePlay.Content = "Play";
            }
        }

        private void UpdateLayerRecipeGuide()
        {
            int selected = Math.Max(1, Math.Min(4, (int)Math.Round(layerRecipeStepSlider.Value)));
            layerRecipeAnimationStep = selected;
            int index = selected - 1;
            (string Input, string Tool, string Output) step = layerRecipeSteps[index];

            txtLayerRecipeSelectedStep.Text = selected.ToString(CultureInfo.InvariantCulture) + " / 4";
            txtLayerRecipeFormula.Text = "Step "
                + selected.ToString(CultureInfo.InvariantCulture)
                + ": Input="
                + step.Input
                + " -> Tool="
                + step.Tool
                + " -> Output="
                + step.Output;
            txtLayerRecipeMeaning.Text = selected switch
            {
                1 => "첫 Step은 Main을 읽어 이진 결과 레이어를 만듭니다.",
                2 => "두 번째 Step은 앞 단계의 Binary_Output을 InputLayer로 사용해 거리를 측정합니다.",
                3 => "OverlayMerge는 Main과 Pin_Gap을 합쳐 측정 위치가 보이는 Pin_Review를 만듭니다.",
                _ => "Recipe는 Step 연결과 acceptance 기준을 함께 저장해 같은 검사를 다시 실행할 수 있게 합니다."
            };
            txtLayerRecipeAnimationStatus.Text = selected switch
            {
                1 => "1 / 4 - Main -> Threshold -> Pin_Binary",
                2 => "2 / 4 - Pin_Binary -> LineDistance -> Pin_Gap",
                3 => "3 / 4 - Main + Pin_Gap -> Overlay -> Pin_Review",
                _ => "4 / 4 - Pin_Gap 결과를 Acceptance 기준과 비교해 최종 OK/NG를 판단합니다."
            };

            for (int i = 0; i < layerRecipeFlowCells.Count; i++)
            {
                bool selectedRow = i / 4 == index;
                layerRecipeFlowCells[i].Background = selectedRow
                    ? animationCandidateBrush
                    : CreateGrayBrush(230);
                layerRecipeFlowTexts[i].Foreground = selectedRow ? Brushes.White : Brushes.Black;
            }

            for (int i = 0; i < layerRecipeLayerCells.Count; i++)
            {
                bool routeLayer = layerRecipeLayers[i] == step.Input
                    || layerRecipeLayers[i] == step.Output
                    || step.Input.Contains(layerRecipeLayers[i], StringComparison.Ordinal);
                layerRecipeLayerCells[i].Background = routeLayer
                    ? animationPassBrush
                    : CreateGrayBrush(230);
                layerRecipeLayerTexts[i].Foreground = routeLayer ? Brushes.White : Brushes.Black;
            }
        }

        private void ResetLayerRecipeAnimation()
        {
            layerRecipeAnimationTimer.Stop();
            btnLayerRecipePlay.Content = "Play";
            layerRecipeAnimationStep = 0;
            txtLayerRecipeSelectedStep.Text = "0 / 4";
            txtLayerRecipeAnimationStatus.Text = "0 / 4 - Main 입력부터 Layer 경로를 따라가 보세요.";

            for (int i = 0; i < layerRecipeFlowCells.Count; i++)
            {
                layerRecipeFlowCells[i].Background = animationNeutralBrush;
                layerRecipeFlowTexts[i].Foreground = Brushes.White;
            }

            for (int i = 0; i < layerRecipeLayerCells.Count; i++)
            {
                layerRecipeLayerCells[i].Background = animationNeutralBrush;
                layerRecipeLayerTexts[i].Foreground = Brushes.White;
            }
        }

        private void AdvanceLayerRecipeAnimation()
        {
            if (layerRecipeAnimationStep >= LayerRecipeAnimationStepCount)
            {
                layerRecipeAnimationStep = 0;
            }

            layerRecipeAnimationStep++;
            isLayerRecipeAnimationAdvancing = true;
            try
            {
                layerRecipeStepSlider.Value = layerRecipeAnimationStep;
                UpdateLayerRecipeGuide();
            }
            finally
            {
                isLayerRecipeAnimationAdvancing = false;
            }
            if (layerRecipeAnimationStep >= LayerRecipeAnimationStepCount)
            {
                layerRecipeAnimationTimer.Stop();
                btnLayerRecipePlay.Content = "Play";
            }
        }

        private void UpdateGeometryGuide()
        {
            double angle = geometryAngleSlider.Value;
            double scale = geometryScaleSlider.Value;
            int outputWidth = (int)Math.Round(768D * scale / 100D);
            int outputHeight = (int)Math.Round(576D * scale / 100D);
            txtGeometryAngle.Text = angle.ToString("0", CultureInfo.InvariantCulture) + " deg";
            txtGeometryScale.Text = scale.ToString("0", CultureInfo.InvariantCulture) + "%";
            txtGeometryFormula.Text = "RotateScale: Angle="
                + angle.ToString("0", CultureInfo.InvariantCulture)
                + " deg, Scale="
                + scale.ToString("0", CultureInfo.InvariantCulture)
                + "%, OutputSize~"
                + outputWidth.ToString(CultureInfo.InvariantCulture)
                + "x"
                + outputHeight.ToString(CultureInfo.InvariantCulture);
            txtGeometryMeaning.Text = "회전과 배율이 바뀌면 기존 ROI와 측정점의 좌표도 달라집니다. Preview 결과에서 새 위치를 확인하고 ROI를 다시 맞추세요.";
            txtGeometryMeaning.Text = "회전과 배율이 바뀌면 기존 ROI와 측정점의 좌표도 달라집니다. Preview 결과에서 새 위치를 확인하고 ROI를 다시 맞추세요.";
            PaintGeometryAnimationFrame(angle, scale, outputWidth, outputHeight);
        }

        private void PaintGeometryAnimationFrame(double angle, double scale, int outputWidth, int outputHeight)
        {
            int visibleStep = Math.Max(0, Math.Min(geometryAnimationStep, GeometryAnimationStepCount));
            geometrySourceBox.Stroke = visibleStep == 0 ? animationCandidateBrush : animationNeutralBrush;
            geometryTargetBox.Background = visibleStep switch
            {
                1 => animationCandidateBrush,
                2 => animationPassBrush,
                3 => animationWarningBrush,
                _ => animationNeutralBrush
            };
            geometryTargetBox.Opacity = visibleStep == 0 ? 0.28D : 0.78D;
            geometryRotateTransform.Angle = visibleStep >= 1 ? angle : 0D;
            geometryScaleTransform.ScaleX = visibleStep >= 2 ? scale / 100D : 1D;
            geometryScaleTransform.ScaleY = visibleStep >= 2 ? scale / 100D : 1D;

            txtGeometryAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - Reset: 원본 ROI와 좌표를 확인합니다.",
                1 => "1 / 3 - Rotate: 중심 기준 " + angle.ToString("0", CultureInfo.InvariantCulture) + " deg 회전으로 좌표가 이동합니다.",
                2 => "2 / 3 - Scale: "
                    + scale.ToString("0", CultureInfo.InvariantCulture)
                    + "% 적용 후 OutputSize~"
                    + outputWidth.ToString(CultureInfo.InvariantCulture)
                    + "x"
                    + outputHeight.ToString(CultureInfo.InvariantCulture),
                _ => "3 / 3 - ROI 검토: 변환 결과에서 Rect, Template, Edge 방향, Pixel/mm 기준의 새 위치를 확인합니다."
            };
            txtGeometryAnimationStatus.Text = visibleStep switch
            {
                0 => "0 / 3 - Reset: 원본 ROI와 좌표를 확인합니다.",
                1 => "1 / 3 - Rotate: 중심 기준 " + angle.ToString("0", CultureInfo.InvariantCulture) + " deg 회전으로 좌표가 이동합니다.",
                2 => "2 / 3 - Scale: "
                    + scale.ToString("0", CultureInfo.InvariantCulture)
                    + "% 적용 후 OutputSize~"
                    + outputWidth.ToString(CultureInfo.InvariantCulture)
                    + "x"
                    + outputHeight.ToString(CultureInfo.InvariantCulture),
                _ => "3 / 3 - ROI review: 변환 결과에서 Rect, Template, Edge 방향, Pixel/mm 기준의 새 위치를 확인한 뒤 Preview/Run을 명시적으로 실행합니다."
            };
        }

        private void ResetGeometryAnimation()
        {
            geometryAnimationTimer.Stop();
            btnGeometryPlay.Content = "Play";
            geometryAnimationStep = 0;
            UpdateGeometryGuide();
        }

        private void AdvanceGeometryAnimation()
        {
            if (geometryAnimationStep >= GeometryAnimationStepCount)
            {
                geometryAnimationStep = 0;
            }

            geometryAnimationStep++;
            UpdateGeometryGuide();
            if (geometryAnimationStep >= GeometryAnimationStepCount)
            {
                geometryAnimationTimer.Stop();
                btnGeometryPlay.Content = "Play";
            }
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

        private void PaintFeaturePointGrid(
            IReadOnlyList<Border> cells,
            IReadOnlyList<TextBlock> texts,
            IReadOnlyList<(int X, int Y)> points,
            IReadOnlyList<bool> goodMatches,
            string label)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].Background = Brushes.Black;
                texts[i].Foreground = Brushes.White;
                texts[i].Text = "0";
            }

            for (int i = 0; i < points.Count; i++)
            {
                int index = points[i].Y * 5 + points[i].X;
                bool good = i < goodMatches.Count && goodMatches[i];
                cells[index].Background = good ? animationCandidateBrush : animationNeutralBrush;
                texts[index].Foreground = Brushes.White;
                texts[index].Text = good ? label : "x";
            }
        }

        private void PaintFeatureScoreCell(int index, Brush background, Brush foreground, string text)
        {
            featureMatchScoreCells[index].Background = background;
            featureMatchScoreTexts[index].Foreground = foreground;
            featureMatchScoreTexts[index].Text = text;
        }

        private double CalculateTemplateScore(int startX, int startY)
        {
            int matches = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    int search = matchingSearchValues[(startY + y) * 5 + startX + x];
                    int template = matchingTemplateValues[y * 2 + x];
                    if (search == template)
                    {
                        matches++;
                    }
                }
            }

            return matches / 4D;
        }

        private void PaintMatchingSearchGrid(int bestIndex)
        {
            (int X, int Y) best = bestIndex >= 0 ? matchingCandidatePositions[bestIndex] : (-1, -1);
            for (int i = 0; i < matchingSearchValues.Length; i++)
            {
                int x = i % 5;
                int y = i / 5;
                bool inBest = bestIndex >= 0 && x >= best.X && x < best.X + 2 && y >= best.Y && y < best.Y + 2;
                int value = matchingSearchValues[i];

                matchingSearchCells[i].BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                matchingSearchCells[i].BorderThickness = new Thickness(1);
                if (inBest)
                {
                    matchingSearchCells[i].Background = animationPassBrush;
                    matchingSearchTexts[i].Foreground = Brushes.White;
                    matchingSearchTexts[i].Text = value > 0 ? "B" : "0";
                }
                else
                {
                    matchingSearchCells[i].Background = CreateGrayBrush(value > 0 ? 230 : 20);
                    matchingSearchTexts[i].Foreground = value > 0 ? Brushes.Black : Brushes.White;
                    matchingSearchTexts[i].Text = value > 0
                        ? topicList.SelectedIndex == 12 ? "E" : "1"
                        : "0";
                }
            }
        }

        private void PaintMatchingCandidateScanGrid()
        {
            PaintMatchingSearchGrid(-1);
            Brush candidateBrush = animationCandidateBrush;
            foreach ((int X, int Y) candidate in matchingCandidatePositions)
            {
                int index = candidate.Y * 5 + candidate.X;
                if (index < 0 || index >= matchingSearchCells.Count)
                {
                    continue;
                }

                matchingSearchCells[index].BorderBrush = candidateBrush;
                matchingSearchCells[index].BorderThickness = new Thickness(3);
                matchingSearchTexts[index].Text = "S";
                matchingSearchTexts[index].Foreground = Brushes.White;
            }
        }

        private void PaintMatchingScoreCell(int index, Brush background, Brush foreground, string text)
        {
            matchingScoreCells[index].Background = background;
            matchingScoreTexts[index].Foreground = foreground;
            matchingScoreTexts[index].Text = text;
        }

        private void PaintLineDistanceCell(int index, Brush background, Brush foreground, string text)
        {
            lineDistanceOutputCells[index].Background = background;
            lineDistanceOutputTexts[index].Foreground = foreground;
            lineDistanceOutputTexts[index].Text = text;
        }

        private void PaintEdgeLineCell(int index, Brush background, Brush foreground, string text)
        {
            edgeLineOutputCells[index].Background = background;
            edgeLineOutputTexts[index].Foreground = foreground;
            edgeLineOutputTexts[index].Text = text;
        }

        private void PaintContourCell(int index, Brush background, Brush foreground, string text)
        {
            contourOutputCells[index].Background = background;
            contourOutputTexts[index].Foreground = foreground;
            contourOutputTexts[index].Text = text;
        }

        private void PaintBlobCell(int index, Brush background, Brush foreground, string text)
        {
            blobOutputCells[index].Background = background;
            blobOutputTexts[index].Foreground = foreground;
            blobOutputTexts[index].Text = text;
        }

        private static bool[] FindContourPixels(bool[] source, int width, int height)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    if (!source[index])
                    {
                        continue;
                    }

                    result[index] = !GetBinary(source, width, height, x - 1, y)
                        || !GetBinary(source, width, height, x + 1, y)
                        || !GetBinary(source, width, height, x, y - 1)
                        || !GetBinary(source, width, height, x, y + 1);
                }
            }

            return result;
        }

        private static (int MinX, int MinY, int MaxX, int MaxY)? FindBounds(bool[] source, int width, int height)
        {
            int minX = width;
            int minY = height;
            int maxX = -1;
            int maxY = -1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!source[y * width + x])
                    {
                        continue;
                    }

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }

            return maxX < 0 ? null : (minX, minY, maxX, maxY);
        }

        private static bool IsOnBounds(int index, int width, (int MinX, int MinY, int MaxX, int MaxY) bounds)
        {
            int x = index % width;
            int y = index / width;
            return x >= bounds.MinX
                && x <= bounds.MaxX
                && y >= bounds.MinY
                && y <= bounds.MaxY
                && (x == bounds.MinX || x == bounds.MaxX || y == bounds.MinY || y == bounds.MaxY);
        }

        private static (int[] Labels, int[] Areas) LabelConnectedBlobs(int[] values, int width, int height)
        {
            int[] labels = new int[values.Length];
            List<int> areas = new();
            int nextLabel = 1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0 || labels[i] != 0)
                {
                    continue;
                }

                int area = FloodFillBlob(values, labels, width, height, i, nextLabel);
                areas.Add(area);
                nextLabel++;
            }

            return (labels, areas.ToArray());
        }

        private static int FloodFillBlob(int[] values, int[] labels, int width, int height, int start, int label)
        {
            Queue<int> queue = new();
            queue.Enqueue(start);
            labels[start] = label;
            int area = 0;

            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                area++;
                int x = index % width;
                int y = index / width;

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {
                            continue;
                        }

                        int next = ny * width + nx;
                        if (values[next] == 0 || labels[next] != 0)
                        {
                            continue;
                        }

                        labels[next] = label;
                        queue.Enqueue(next);
                    }
                }
            }

            return area;
        }

        private static bool[] Erode(bool[] source)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    result[y * 5 + x] = AllNeighbors(source, x, y);
                }
            }

            return result;
        }

        private static bool[] Dilate(bool[] source)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    result[y * 5 + x] = AnyNeighbor(source, x, y);
                }
            }

            return result;
        }

        private static bool AllNeighbors(bool[] source, int x, int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (!GetBinary(source, x + dx, y + dy))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool AnyNeighbor(bool[] source, int x, int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (GetBinary(source, x + dx, y + dy))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool GetBinary(bool[] source, int x, int y)
        {
            return x >= 0 && x < 5 && y >= 0 && y < 5 && source[y * 5 + x];
        }

        private static bool GetBinary(bool[] source, int width, int height, int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height && source[y * width + x];
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
            bool isPixelTopic = topicList.SelectedIndex == 0;
            bool isBrightnessTopic = topicList.SelectedIndex == 1;
            bool isThresholdTopic = topicList.SelectedIndex == 2;
            bool isFilterTopic = topicList.SelectedIndex == 3;
            bool isMorphologyTopic = topicList.SelectedIndex == 4;
            bool isBlobTopic = topicList.SelectedIndex == 5;
            bool isContourTopic = topicList.SelectedIndex == 6;
            bool isEdgeLineTopic = topicList.SelectedIndex == 7;
            bool isLineDistanceTopic = topicList.SelectedIndex == 8;
            bool isMatchingTopic = topicList.SelectedIndex == 9;
            bool isFeatureMatchingTopic = topicList.SelectedIndex == 10;
            bool isLayerRecipeTopic = topicList.SelectedIndex == 11;
            bool isEdgeBasedMatchingTopic = topicList.SelectedIndex == 12;
            bool isMetricsAcceptanceTopic = topicList.SelectedIndex == 13;
            bool isArithmeticTopic = topicList.SelectedIndex == 14;
            bool isGeometryTopic = topicList.SelectedIndex == 15;
            bool isColorHsvTopic = topicList.SelectedIndex == 16;
            pixelTopicPanel.Visibility = isPixelTopic ? Visibility.Visible : Visibility.Collapsed;
            brightnessTopicPanel.Visibility = isBrightnessTopic ? Visibility.Visible : Visibility.Collapsed;
            arithmeticTopicPanel.Visibility = isArithmeticTopic ? Visibility.Visible : Visibility.Collapsed;
            geometryTopicPanel.Visibility = isGeometryTopic ? Visibility.Visible : Visibility.Collapsed;
            colorHsvTopicPanel.Visibility = isColorHsvTopic ? Visibility.Visible : Visibility.Collapsed;
            filterTopicPanel.Visibility = isFilterTopic ? Visibility.Visible : Visibility.Collapsed;
            morphologyTopicPanel.Visibility = isMorphologyTopic ? Visibility.Visible : Visibility.Collapsed;
            blobTopicPanel.Visibility = isBlobTopic ? Visibility.Visible : Visibility.Collapsed;
            contourTopicPanel.Visibility = isContourTopic ? Visibility.Visible : Visibility.Collapsed;
            edgeLineTopicPanel.Visibility = isEdgeLineTopic ? Visibility.Visible : Visibility.Collapsed;
            lineDistanceTopicPanel.Visibility = isLineDistanceTopic ? Visibility.Visible : Visibility.Collapsed;
            matchingTopicPanel.Visibility = (isMatchingTopic || isEdgeBasedMatchingTopic) ? Visibility.Visible : Visibility.Collapsed;
            featureMatchingTopicPanel.Visibility = isFeatureMatchingTopic ? Visibility.Visible : Visibility.Collapsed;
            layerRecipeTopicPanel.Visibility = isLayerRecipeTopic ? Visibility.Visible : Visibility.Collapsed;
            metricsAcceptanceTopicPanel.Visibility = isMetricsAcceptanceTopic ? Visibility.Visible : Visibility.Collapsed;
            thresholdTabs.Visibility = isThresholdTopic ? Visibility.Visible : Visibility.Collapsed;
            thresholdControls.Visibility = isThresholdTopic ? Visibility.Visible : Visibility.Collapsed;
            animationLegendPanel.Visibility = isPixelTopic
                || isArithmeticTopic
                || isBrightnessTopic
                || isColorHsvTopic
                || isThresholdTopic
                || isFilterTopic
                || isGeometryTopic
                || isMorphologyTopic
                || isBlobTopic
                || isContourTopic
                || isEdgeLineTopic
                || isLineDistanceTopic
                || isMatchingTopic
                || isFeatureMatchingTopic
                || isEdgeBasedMatchingTopic
                || isMetricsAcceptanceTopic
                || isLayerRecipeTopic
                ? Visibility.Visible
                : Visibility.Collapsed;
            practiceWorkflowExpander.IsExpanded = !(isBlobTopic || isMatchingTopic || isFeatureMatchingTopic || isEdgeBasedMatchingTopic || isMetricsAcceptanceTopic);
            txtTopicPractice.Text = ResolveSelectedTopicPracticeText(topicList.SelectedIndex);
            if (isPixelTopic)
            {
                txtTopicTitle.Text = "0. 커리큘럼 / 영상 기초";
                txtTopicSubtitle.Text = "픽셀과 Mat, 좌표와 ROI를 익힌 뒤 전처리·검출·측정·판정 흐름으로 이어갑니다.";
                UpdateFoundationGuide();
                UpdateMatChannelGuide();
                return;
            }

            if (isBrightnessTopic)
            {
                txtTopicTitle.Text = "1. 밝기와 히스토그램";
                txtTopicSubtitle.Text = "GV 분포를 보면 조명 변화, 배경 분리, Threshold 기준값 후보를 더 안정적으로 판단할 수 있습니다.";
                UpdateBrightnessGuide();
                return;
            }

            if (isFilterTopic)
            {
                txtTopicTitle.Text = "3. 필터링";
                txtTopicSubtitle.Text = "주변 픽셀을 같이 계산해 노이즈를 줄이거나 경계를 강조하고, 다음 검출 도구가 안정적으로 동작하도록 준비합니다.";
                UpdateFilterGuide();
                return;
            }

            if (isMorphologyTopic)
            {
                txtTopicTitle.Text = "4. 모폴로지";
                txtTopicSubtitle.Text = "Threshold로 만든 흰 영역을 줄이거나 키워 작은 노이즈, 구멍, 끊어진 부분을 정리합니다.";
                UpdateMorphologyGuide();
                return;
            }

            if (isBlobTopic)
            {
                txtTopicTitle.Text = "5. Blob / 영역 검출";
                txtTopicSubtitle.Text = "연결된 흰 영역을 후보로 세고, 면적과 개수 기준으로 OK/NG 판단의 기초 값을 만듭니다.";
                UpdateBlobGuide();
                return;
            }

            if (isContourTopic)
            {
                txtTopicTitle.Text = "6. Contour / 외곽선";
                txtTopicSubtitle.Text = "통과한 Blob 후보의 경계를 따라 실제 모양, 위치, 폭/높이를 검토하는 단계입니다.";
                UpdateContourGuide();
                return;
            }

            if (isEdgeLineTopic)
            {
                txtTopicTitle.Text = "7. Edge / Line";
                txtTopicSubtitle.Text = "밝기 차이가 큰 경계를 찾고, 같은 방향으로 이어진 후보를 라인으로 해석하는 단계입니다.";
                UpdateEdgeLineGuide();
                return;
            }

            if (isLineDistanceTopic)
            {
                txtTopicTitle.Text = "8. LineDistance 측정";
                txtTopicSubtitle.Text = "두 Edge 사이 거리를 여러 줄에서 재고 평균과 흔들림을 함께 판정합니다.";
                UpdateLineDistanceGuide();
                return;
            }

            if (isMatchingTopic)
            {
                txtTopicTitle.Text = "9. Matching";
                txtTopicSubtitle.Text = "기준 Template과 검색 영역을 비교해 최고 Score 위치를 찾고 ScoreThreshold로 판정합니다.";
                ConfigureMatchingToolLink(edgeBased: false);
                UpdateMatchingGuide();
                return;
            }

            if (isFeatureMatchingTopic)
            {
                txtTopicTitle.Text = "10. Feature Matching";
                txtTopicSubtitle.Text = "keypoint와 descriptor match를 이용해 회전/크기 변화가 있는 대상 후보를 찾습니다.";
                UpdateFeatureMatchingGuide();
                return;
            }

            if (isLayerRecipeTopic)
            {
                txtTopicTitle.Text = "11. Layer / Pipeline / Recipe";
                txtTopicSubtitle.Text = "각 Step의 InputLayer와 OutputLayer를 따라가며 이미지 처리 순서와 결과 위치를 읽습니다.";
                UpdateLayerRecipeGuide();
                return;
            }

            if (isMetricsAcceptanceTopic)
            {
                txtTopicTitle.Text = "13. Metrics / Acceptance";
                txtTopicSubtitle.Text = "Good/Bad 샘플의 실제 지표를 OK/NG 기준과 연결하고, 평균이 이상치를 가릴 때 Range/Max 기준을 함께 사용합니다.";
                UpdateMetricsAcceptanceGuide();
                return;
            }

            if (isArithmeticTopic)
            {
                txtTopicTitle.Text = "14. Arithmetic / Logic";
                txtTopicSubtitle.Text = "Add, Subtract, AbsDiff, Bitwise AND/OR로 두 이미지의 차이와 공통 영역을 계산합니다.";
                UpdateArithmeticGuide();
                return;
            }

            if (isGeometryTopic)
            {
                txtTopicTitle.Text = "15. Geometry Transform";
                txtTopicSubtitle.Text = "RotateScale의 회전각과 배율이 OutputSize, 여백, ROI 좌표에 미치는 영향을 확인합니다.";
                UpdateGeometryGuide();
                return;
            }

            if (isColorHsvTopic)
            {
                txtTopicTitle.Text = "16. Color / HSV";
                txtTopicSubtitle.Text = "Hue, Saturation, Value를 나누어 원하는 색 영역을 마스크로 분리하고 MaskPixelRatio로 비교합니다.";
                UpdateColorHsvGuide();
                return;
            }

            if (isEdgeBasedMatchingTopic)
            {
                txtTopicTitle.Text = "12. EdgeBasedMatching";
                txtTopicSubtitle.Text = "밝기 무늬보다 Edge 형상과 Score를 기준으로 대상을 찾고 ResultCount와 ScoreMax를 비교합니다.";
                ConfigureMatchingToolLink(edgeBased: true);
                UpdateMatchingGuide();
                return;
            }

            txtTopicTitle.Text = "2. 이진화 / Threshold";
            txtTopicSubtitle.Text = "이미지의 각 픽셀 GV를 기준값과 비교해서 검정/흰색 결과 이미지를 만드는 전처리입니다.";
        }

        private void UpdateThresholdMarker()
        {
            double width = thresholdBarHost.ActualWidth;
            if (width <= 0D)
            {
                return;
            }

            double markerWidth = thresholdMarker.ActualWidth <= 0D ? 4D : thresholdMarker.ActualWidth;
            double x = (width - markerWidth) * thresholdSlider.Value / 255D;
            thresholdMarkerTransform.X = Math.Max(0D, Math.Min(width - markerWidth, x));
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            double next = thresholdSlider.Value + (animationForward ? 5D : -5D);
            if (next >= 230D)
            {
                next = 230D;
                animationForward = false;
            }
            else if (next <= 25D)
            {
                next = 25D;
                animationForward = true;
            }

            thresholdSlider.Value = next;
        }

        private void MorphologyAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMorphologyAnimation();
        }

        private void BrightnessAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceBrightnessAnimation();
        }

        private void ArithmeticAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceArithmeticAnimation();
        }

        private void ColorHsvAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceColorHsvAnimation();
        }

        private void FilterAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFilterAnimation();
        }

        private void FoundationAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFoundationAnimation();
        }

        private void MatChannelAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMatChannelAnimation();
        }

        private void GeometryAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceGeometryAnimation();
        }

        private void BlobAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceBlobAnimation();
        }

        private void ContourAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceContourAnimation();
        }

        private void EdgeLineAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceEdgeLineAnimation();
        }

        private void LineDistanceAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceLineDistanceAnimation();
        }

        private void MatchingAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMatchingAnimation();
        }

        private void FeatureMatchingAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFeatureMatchingAnimation();
        }

        private void MetricsAcceptanceAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMetricsAcceptanceAnimation();
        }

        private void LayerRecipeAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceLayerRecipeAnimation();
        }

        private void ThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateGuide();
            }
        }

        private void InvertCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateGuide();
            }
        }

        private void BrightnessOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
                brightnessAnimationStep = BrightnessAnimationStepCount;
                UpdateBrightnessGuide();
            }
        }

        private void BrightnessPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (brightnessAnimationTimer.IsEnabled)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
                return;
            }

            if (brightnessAnimationStep >= BrightnessAnimationStepCount)
            {
                ResetBrightnessAnimation();
            }

            btnBrightnessPlay.Content = "Pause";
            brightnessAnimationTimer.Start();
        }

        private void BrightnessStepButton_Click(object sender, RoutedEventArgs e)
        {
            brightnessAnimationTimer.Stop();
            btnBrightnessPlay.Content = "Play";
            AdvanceBrightnessAnimation();
        }

        private void BrightnessResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBrightnessAnimation();
        }

        private void ArithmeticModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
                arithmeticAnimationStep = ArithmeticAnimationStepCount;
                UpdateArithmeticGuide();
            }
        }

        private void ArithmeticPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (arithmeticAnimationTimer.IsEnabled)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
                return;
            }

            if (arithmeticAnimationStep >= ArithmeticAnimationStepCount)
            {
                ResetArithmeticAnimation();
            }

            btnArithmeticPlay.Content = "Pause";
            arithmeticAnimationTimer.Start();
        }

        private void ArithmeticStepButton_Click(object sender, RoutedEventArgs e)
        {
            arithmeticAnimationTimer.Stop();
            btnArithmeticPlay.Content = "Play";
            AdvanceArithmeticAnimation();
        }

        private void ArithmeticResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetArithmeticAnimation();
        }

        private void FilterModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
                filterAnimationStep = FilterAnimationStepCount;
                UpdateFilterGuide();
            }
        }

        private void FilterPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (filterAnimationTimer.IsEnabled)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
                return;
            }

            if (filterAnimationStep >= FilterAnimationStepCount)
            {
                ResetFilterAnimation();
            }

            btnFilterPlay.Content = "Pause";
            filterAnimationTimer.Start();
        }

        private void FilterStepButton_Click(object sender, RoutedEventArgs e)
        {
            filterAnimationTimer.Stop();
            btnFilterPlay.Content = "Play";
            AdvanceFilterAnimation();
        }

        private void FilterResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFilterAnimation();
        }

        private void FoundationPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (foundationAnimationTimer.IsEnabled)
            {
                foundationAnimationTimer.Stop();
                btnFoundationPlay.Content = "자동 재생";
                return;
            }

            if (foundationAnimationStep >= FoundationAnimationStepCount)
            {
                ResetFoundationAnimation();
            }

            btnFoundationPlay.Content = "일시 정지";
            foundationAnimationTimer.Start();
        }

        private void FoundationStepButton_Click(object sender, RoutedEventArgs e)
        {
            foundationAnimationTimer.Stop();
            btnFoundationPlay.Content = "자동 재생";
            AdvanceFoundationAnimation();
        }

        private void FoundationResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFoundationAnimation();
        }

        private void MatChannelPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (matChannelAnimationTimer.IsEnabled)
            {
                matChannelAnimationTimer.Stop();
                btnMatChannelPlay.Content = "자동 재생";
                return;
            }

            if (matChannelAnimationStep >= MatChannelAnimationStepCount)
            {
                ResetMatChannelAnimation();
            }

            btnMatChannelPlay.Content = "일시 정지";
            matChannelAnimationTimer.Start();
        }

        private void MatChannelStepButton_Click(object sender, RoutedEventArgs e)
        {
            matChannelAnimationTimer.Stop();
            btnMatChannelPlay.Content = "자동 재생";
            AdvanceMatChannelAnimation();
        }

        private void MatChannelResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMatChannelAnimation();
        }

        private void MorphologyModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateMorphologyGuide();
            }
        }

        private void MorphologyPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (morphologyAnimationTimer.IsEnabled)
            {
                morphologyAnimationTimer.Stop();
                btnMorphologyPlay.Content = "Play";
                return;
            }

            if (morphologyAnimationStep >= morphologySampleValues.Length)
            {
                ResetMorphologyAnimation();
            }

            morphologyAnimationTimer.Start();
            btnMorphologyPlay.Content = "Pause";
        }

        private void MorphologyStepButton_Click(object sender, RoutedEventArgs e)
        {
            morphologyAnimationTimer.Stop();
            btnMorphologyPlay.Content = "Play";
            AdvanceMorphologyAnimation();
        }

        private void MorphologyResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMorphologyAnimation();
        }

        private void BlobPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (blobAnimationTimer.IsEnabled)
            {
                blobAnimationTimer.Stop();
                btnBlobPlay.Content = "Play";
                return;
            }

            if (blobAnimationStep >= GetBlobCandidateCount())
            {
                ResetBlobAnimation();
            }

            blobAnimationTimer.Start();
            btnBlobPlay.Content = "Pause";
        }

        private void BlobStepButton_Click(object sender, RoutedEventArgs e)
        {
            blobAnimationTimer.Stop();
            btnBlobPlay.Content = "Play";
            AdvanceBlobAnimation();
        }

        private void BlobResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBlobAnimation();
        }

        private void ContourPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (contourAnimationTimer.IsEnabled)
            {
                contourAnimationTimer.Stop();
                btnContourPlay.Content = "Play";
                return;
            }

            if (contourAnimationStep >= ContourAnimationStepCount)
            {
                ResetContourAnimation();
            }

            contourAnimationTimer.Start();
            btnContourPlay.Content = "Pause";
        }

        private void ContourStepButton_Click(object sender, RoutedEventArgs e)
        {
            contourAnimationTimer.Stop();
            btnContourPlay.Content = "Play";
            AdvanceContourAnimation();
        }

        private void ContourResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetContourAnimation();
        }

        private void EdgeLinePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (edgeLineAnimationTimer.IsEnabled)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
                return;
            }

            if (edgeLineAnimationStep >= EdgeLineAnimationStepCount)
            {
                ResetEdgeLineAnimation();
            }

            edgeLineAnimationTimer.Start();
            btnEdgeLinePlay.Content = "Pause";
        }

        private void EdgeLineStepButton_Click(object sender, RoutedEventArgs e)
        {
            edgeLineAnimationTimer.Stop();
            btnEdgeLinePlay.Content = "Play";
            AdvanceEdgeLineAnimation();
        }

        private void EdgeLineResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetEdgeLineAnimation();
        }

        private void LineDistancePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (lineDistanceAnimationTimer.IsEnabled)
            {
                lineDistanceAnimationTimer.Stop();
                btnLineDistancePlay.Content = "Play";
                return;
            }

            if (lineDistanceAnimationStep >= LineDistanceAnimationStepCount)
            {
                ResetLineDistanceAnimation();
            }

            lineDistanceAnimationTimer.Start();
            btnLineDistancePlay.Content = "Pause";
        }

        private void LineDistanceStepButton_Click(object sender, RoutedEventArgs e)
        {
            lineDistanceAnimationTimer.Stop();
            btnLineDistancePlay.Content = "Play";
            AdvanceLineDistanceAnimation();
        }

        private void LineDistanceResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetLineDistanceAnimation();
        }

        private void MatchingPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchingAnimationTimer.IsEnabled)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
                return;
            }

            if (matchingAnimationStep >= MatchingAnimationStepCount)
            {
                ResetMatchingAnimation();
            }

            matchingAnimationTimer.Start();
            btnMatchingPlay.Content = "Pause";
        }

        private void MatchingStepButton_Click(object sender, RoutedEventArgs e)
        {
            matchingAnimationTimer.Stop();
            btnMatchingPlay.Content = "Play";
            AdvanceMatchingAnimation();
        }

        private void MatchingResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMatchingAnimation();
        }

        private void FeatureMatchingPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (featureMatchingAnimationTimer.IsEnabled)
            {
                featureMatchingAnimationTimer.Stop();
                btnFeatureMatchingPlay.Content = "Play";
                return;
            }

            if (featureMatchingAnimationStep >= FeatureMatchingAnimationStepCount)
            {
                ResetFeatureMatchingAnimation();
            }

            btnFeatureMatchingPlay.Content = "Pause";
            featureMatchingAnimationTimer.Start();
        }

        private void FeatureMatchingStepButton_Click(object sender, RoutedEventArgs e)
        {
            featureMatchingAnimationTimer.Stop();
            btnFeatureMatchingPlay.Content = "Play";
            AdvanceFeatureMatchingAnimation();
        }

        private void FeatureMatchingResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFeatureMatchingAnimation();
        }

        private void MetricsAcceptancePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (metricsAcceptanceAnimationTimer.IsEnabled)
            {
                metricsAcceptanceAnimationTimer.Stop();
                btnMetricsAcceptancePlay.Content = "Play";
                return;
            }

            if (metricsAcceptanceAnimationStep >= MetricsAcceptanceAnimationStepCount)
            {
                ResetMetricsAcceptanceAnimation();
            }

            btnMetricsAcceptancePlay.Content = "Pause";
            metricsAcceptanceAnimationTimer.Start();
        }

        private void MetricsAcceptanceStepButton_Click(object sender, RoutedEventArgs e)
        {
            metricsAcceptanceAnimationTimer.Stop();
            btnMetricsAcceptancePlay.Content = "Play";
            AdvanceMetricsAcceptanceAnimation();
        }

        private void MetricsAcceptanceResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMetricsAcceptanceAnimation();
        }

        private void BlobMinAreaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateBlobGuide();
            }
        }

        private void ContourDrawModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                contourAnimationTimer.Stop();
                btnContourPlay.Content = "Play";
                contourAnimationStep = ContourAnimationStepCount;
                UpdateContourGuide();
            }
        }

        private void EdgeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
                edgeLineAnimationStep = EdgeLineAnimationStepCount;
                UpdateEdgeLineGuide();
            }
        }

        private void LineDistanceRangeMaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                lineDistanceAnimationTimer.Stop();
                btnLineDistancePlay.Content = "Play";
                lineDistanceAnimationStep = LineDistanceAnimationStepCount;
                UpdateLineDistanceGuide();
            }
        }

        private void MatchingThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
                matchingAnimationStep = MatchingAnimationStepCount;
                UpdateMatchingGuide();
            }
        }

        private void FeatureGoodMatchMinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                featureMatchingAnimationTimer.Stop();
                btnFeatureMatchingPlay.Content = "Play";
                featureMatchingAnimationStep = FeatureMatchingAnimationStepCount;
                UpdateFeatureMatchingGuide();
            }
        }

        private void LayerRecipeStepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                if (!isLayerRecipeAnimationAdvancing)
                {
                    layerRecipeAnimationTimer.Stop();
                    btnLayerRecipePlay.Content = "Play";
                }
                UpdateLayerRecipeGuide();
            }
        }

        private void LayerRecipePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (layerRecipeAnimationTimer.IsEnabled)
            {
                layerRecipeAnimationTimer.Stop();
                btnLayerRecipePlay.Content = "Play";
                return;
            }

            if (layerRecipeAnimationStep >= LayerRecipeAnimationStepCount)
            {
                ResetLayerRecipeAnimation();
            }

            btnLayerRecipePlay.Content = "Pause";
            layerRecipeAnimationTimer.Start();
        }

        private void LayerRecipeStepButton_Click(object sender, RoutedEventArgs e)
        {
            layerRecipeAnimationTimer.Stop();
            btnLayerRecipePlay.Content = "Play";
            AdvanceLayerRecipeAnimation();
        }

        private void LayerRecipeResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetLayerRecipeAnimation();
        }

        private void GeometrySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                geometryAnimationTimer.Stop();
                btnGeometryPlay.Content = "Play";
                geometryAnimationStep = GeometryAnimationStepCount;
                UpdateGeometryGuide();
            }
        }

        private void GeometryPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (geometryAnimationTimer.IsEnabled)
            {
                geometryAnimationTimer.Stop();
                btnGeometryPlay.Content = "Play";
                return;
            }

            if (geometryAnimationStep >= GeometryAnimationStepCount)
            {
                ResetGeometryAnimation();
            }

            btnGeometryPlay.Content = "Pause";
            geometryAnimationTimer.Start();
        }

        private void GeometryStepButton_Click(object sender, RoutedEventArgs e)
        {
            geometryAnimationTimer.Stop();
            btnGeometryPlay.Content = "Play";
            AdvanceGeometryAnimation();
        }

        private void GeometryResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetGeometryAnimation();
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

        private void ThresholdBarHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThresholdMarker();
        }

        private void AnimateButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationTimer.IsEnabled)
            {
                animationTimer.Stop();
                btnAnimate.Content = "Play";
                return;
            }

            animationTimer.Start();
            btnAnimate.Content = "Stop";
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyThresholdRequested?.Invoke(
                this,
                new OpenVisionLearnThresholdApplyEventArgs(ClampToByte(thresholdSlider.Value), chkInvert.IsChecked == true));
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
                if (menu == VISION_MENU.Filter && ReferenceEquals(button, btnFilteringOpenTool))
                {
                    txtFilteringToolLocationTitle.Text =
                        "열림: Filter | 찾을 위치: Input/Output Layer, Filter Type, Border Type, Kernel Width/Height";
                    txtFilteringToolLocationDetail.Text =
                        "Filter에서 Median Kernel 또는 Bilateral의 Diameter/Sigma를 확인하고 Preview를 눌러 입력과 출력 영상을 비교하세요.";
                    return;
                }

                if (menu == VISION_MENU.Morphology && ReferenceEquals(button, btnMorphologyOpenTool))
                {
                    txtMorphologyToolLocationTitle.Text =
                        "열림: Morphology | 찾을 위치: Input/Output Layer, Operation, Kernel Width/Height, Shape";
                    txtMorphologyToolLocationDetail.Text =
                        "Morphology에서 Kernel 프리셋과 Shape를 확인하고 Preview를 눌러 연산 전후의 형상 변화를 비교하세요.";
                    return;
                }

                if (menu == VISION_MENU.Blob && ReferenceEquals(button, btnBlobOpenTool))
                {
                    txtBlobToolLocationTitle.Text =
                        "열림: Blob | PropertyGrid: Use ROI / ROI, Blob Parameter > Min area / Max area";
                    txtBlobToolLocationDetail.Text =
                        "Blob에서 ROI와 면적 범위를 설정하고 Preview 또는 Run Review에서 ResultCount, AreaMin/AreaMax, BoundsWidth/BoundsHeight를 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.Contour && ReferenceEquals(button, btnContourOpenTool))
                {
                    txtContourToolLocationTitle.Text =
                        "열림: Contour | PropertyGrid: Contour > 컨투어 표시, Retrieval mode, Min area / Max area";
                    txtContourToolLocationDetail.Text =
                        "Contour에서 검색 방식과 면적 범위를 설정하고 Preview 또는 Run Review에서 ResultCount, AreaMax, BoundsWidthMax, BoundsHeightMax를 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.EdgeDetection && ReferenceEquals(button, btnEdgeDetectionOpenTool))
                {
                    txtEdgeLineToolLocationTitle.Text =
                        "열림: Edge Detection | 찾을 위치: Edge Type, Canny Low/High, Canny Aperture, Use L2 Gradient";
                    txtEdgeLineToolLocationDetail.Text =
                        "Edge Detection에서 Canny/Sobel/Scharr/Laplacian을 선택하고 방향과 Kernel 값을 확인한 뒤 Preview에서 에지 영상을 비교하세요.";
                    return;
                }

                if (menu == VISION_MENU.Line && ReferenceEquals(button, btnEdgeLineOpenLineTool))
                {
                    txtEdgeLineToolLocationTitle.Text =
                        "열림: Line | 찾을 위치: Purpose, Line A/B, ROI, Edge > Polarity/Direction/Contrast/Thickness";
                    txtEdgeLineToolLocationDetail.Text =
                        "Line에서 Scan direction/interval과 표시 옵션을 설정하고 Preview에서 검출선의 위치와 방향을 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.Line && ReferenceEquals(button, btnLineDistanceOpenTool))
                {
                    txtLineDistanceToolLocationTitle.Text =
                        "열림: Line | 찾을 위치: Purpose > Measure, Line A/B, ROI";
                    txtLineDistanceToolLocationDetail.Text =
                        "Line의 Purpose를 Measure로 선택하고 Pixel/mm와 edge/scan 값을 설정한 뒤 Preview 또는 Run Review에서 DistanceMmAvg와 DistanceMmRange/Max를 함께 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.Matching && ReferenceEquals(button, btnMatchingOpenTool))
                {
                    txtMatchingToolLocationTitle.Text =
                        "열림: Matching | 찾을 위치: Template Ready / Pattern path, Matching > Min score / Match count, ROI";
                    txtMatchingToolLocationDetail.Text =
                        "Matching에서 Template과 검색 ROI, 필요한 angle/scale을 설정하고 Preview 또는 Run Review에서 overlay 위치, ScoreMax, ResultCount를 함께 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.EdgeBasedMatching && ReferenceEquals(button, btnMatchingOpenTool))
                {
                    txtMatchingToolLocationTitle.Text =
                        "열림: EdgeBasedMatching | 찾을 위치: Template Ready / Pattern path, Matching > Min score / Match count, Edge Model > Canny range / Max template points, Search > Search step, ROI";
                    txtMatchingToolLocationDetail.Text =
                        "EdgeBasedMatching에서 edge Template과 검색 ROI를 설정하고 Preview 또는 Run Review에서 overlay 위치, ScoreMax, ResultCount를 함께 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.FeatureMatching && ReferenceEquals(button, btnFeatureMatchingOpenTool))
                {
                    txtFeatureMatchingToolLocationTitle.Text =
                        "열림: FeatureMatching | 찾을 위치: Template Ready / Feature template path, Matching > Ratio threshold / RANSAC tolerance, ROI";
                    txtFeatureMatchingToolLocationDetail.Text =
                        "FeatureMatching에서 특징점이 충분한 Template과 검색 ROI, Ratio 기준과 RANSAC 허용 오차를 설정하고 Preview 또는 Run Review에서 overlay 위치, ScoreMax, ResultCount를 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.RotateAndScale && ReferenceEquals(button, btnGeometryOpenTool))
                {
                    txtGeometryToolLocationTitle.Text =
                        "열림: Rotate / Scale | 찾을 위치: Input/Output Layer, Angle, Scale X, Scale Y";
                    txtGeometryToolLocationDetail.Text =
                        "Rotate / Scale에서 Angle과 Scale X/Y를 설정하고 Preview 결과의 OutputSize와 영상 방향을 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.AffineTransform && ReferenceEquals(button, btnGeometryOpenAffineTool))
                {
                    txtGeometryToolLocationTitle.Text =
                        "열림: Affine Transform | 찾을 위치: Source/Destination Points, Output, Sampling, Validation Gates";
                    txtGeometryToolLocationDetail.Text =
                        "대응하는 세 점을 같은 순서로 입력하고 Preview에서 destination triangle, transformed frame, Affine 2x3 행렬, AffineValidPixelRatio를 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.Arithmetic)
                {
                    txtArithmeticToolLocationTitle.Text =
                        "열림: Arithmetic | 찾을 위치: Input A / Input B / Output Layer, Mode, Arithmetic Type, Input B Source";
                    txtArithmeticToolLocationDetail.Text =
                        "Arithmetic에서 두 입력 레이어 또는 Constant/Offset 모드를 설정하고 Preview에서 픽셀 연산 결과를 확인하세요.";
                    return;
                }

                if (menu == VISION_MENU.Mean || menu == VISION_MENU.Histogram)
                {
                    txtBrightnessToolLocationTitle.Text = menu == VISION_MENU.Mean
                        ? "열림: Mean | 찾을 위치: Mean Type, Min Mean, Max Mean, Input/Output Layer"
                        : "열림: Histogram | 찾을 위치: Type, Clip Limit, Tile Grid 또는 Normalize Alpha/Beta";
                    txtBrightnessToolLocationDetail.Text =
                        "입력과 출력 레이어를 확인하고 Preview 또는 Run Review에서 밝기 분포와 결과 지표를 비교하세요.";
                    return;
                }

                if (menu == VISION_MENU.HSV)
                {
                    txtColorHsvToolLocationTitle.Text =
                        "열림: HSV | 찾을 위치: Hue Min/Max, Saturation Min/Max, Value Min/Max, ROI, OutputLayer";
                    txtColorHsvToolLocationDetail.Text =
                        "HSV에서 색 범위와 ROI를 설정하고 Preview 또는 Run Review에서 선택된 색 영역과 결과 레이어를 확인하세요.";
                    return;
                }

                txtFoundationToolLocationTitle.Text = menu switch
                {
                    VISION_MENU.Blob => "열림: Blob | 찾을 위치: PropertyGrid > ROI > Use ROI / ROI (CvROI)",
                    VISION_MENU.Filter => "열림: Filter | 찾을 위치: Filter options > Kernel Width / Kernel Height",
                    VISION_MENU.RotateAndScale => "열림: Rotate / Scale | 찾을 위치: Angle / Scale X / Scale Y",
                    VISION_MENU.AffineTransform => "열림: Affine Transform | 찾을 위치: Source/Destination 3 points / Validation Gates",
                    _ => "열림: " + menu + " | 찾을 위치: Parameter panel"
                };
                txtFoundationToolLocationDetail.Text = menu switch
                {
                    VISION_MENU.RotateAndScale => "Angle과 Scale X/Y를 설정하고 Preview 결과에서 OutputSize와 영상 방향을 확인하세요.",
                    VISION_MENU.AffineTransform => "세 대응점을 입력하고 Preview에서 destination/frame 드로잉과 valid-pixel ratio를 확인하세요.",
                    _ => "강조된 PropertyGrid 항목과 입력 레이어를 확인하고 Preview에서 값의 영향을 비교하세요."
                };
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static SolidColorBrush CreateGrayBrush(int value)
        {
            byte channel = (byte)ClampToByte(value);
            return new SolidColorBrush(Color.FromRgb(channel, channel, channel));
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

        private void ConfigureMatchingToolLink(bool edgeBased)
        {
            btnMatchingOpenTool.Content = edgeBased ? "EdgeBasedMatching Tool 열기" : "Matching Tool 열기";
            btnMatchingOpenTool.Tag = edgeBased ? nameof(VISION_MENU.EdgeBasedMatching) : nameof(VISION_MENU.Matching);
            btnMatchingOpenTool.ToolTip = edgeBased
                ? "EdgeBasedMatching 파라미터에서 edge Template, 검색 ROI, Score 기준을 확인합니다."
                : "Matching 파라미터에서 Template, 검색 ROI, Score 기준을 확인합니다.";
            AutomationProperties.SetAutomationId(
                btnMatchingOpenTool,
                edgeBased ? "OpenVisionLearnEdgeBasedMatchingOpenToolButton" : "OpenVisionLearnMatchingOpenToolButton");
            txtMatchingToolLocationTitle.Text = edgeBased
                ? "EdgeBasedMatching Tool: Template Ready / Pattern path, Matching > Min score / Match count, Edge Model > Canny range / Max template points, Search > Search step, ROI"
                : "Matching Tool: Template Ready / Pattern path, Matching > Min score / Match count, ROI";
            txtMatchingToolLocationDetail.Text = edgeBased
                ? "edge Template과 검색 ROI를 정하고 필요한 angle/scale search를 설정합니다. Preview 또는 Run Review에서 overlay 위치, ScoreMax, ResultCount를 함께 확인합니다."
                : "Template과 검색 ROI, 필요한 angle/scale을 설정합니다. Preview 또는 Run Review에서 overlay 위치, ScoreMax, ResultCount를 함께 확인합니다.";
        }

        private static string ResolveSelectedTopicDocumentFileName(int index)
        {
            return OpenVisionLearnTopicCatalog.Resolve(index).Document;
        }

        private static string ResolveSelectedTopicLearnPathId(int index)
        {
            return OpenVisionLearnTopicCatalog.Resolve(index).PracticePathId;
        }

        private static string ResolveSelectedTopicPracticeText(int index)
        {
            return OpenVisionLearnTopicCatalog.Resolve(index).PracticeText;
        }

        private static string FormatSigned(int value)
        {
            return value >= 0
                ? "+" + value.ToString(CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        private string GetSelectedFilterMode()
        {
            return (filterModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Mean blur";
        }

        private string GetSelectedArithmeticMode()
        {
            return (arithmeticModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "AbsDiff";
        }

        private string GetSelectedMorphologyMode()
        {
            return (morphologyModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Erosion";
        }

        private string GetSelectedContourDrawMode()
        {
            return (contourDrawModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Contour";
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
