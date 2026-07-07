using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionLearnWindow : Window
    {
        private readonly DispatcherTimer animationTimer;
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
        private readonly string[] layerRecipeLayers = { "Main", "Pin_Binary", "Pin_Gap", "Pin_Review" };
        private readonly (string Input, string Tool, string Output)[] layerRecipeSteps =
        {
            ("Main", "Threshold", "Pin_Binary"),
            ("Pin_Binary", "LineDistance", "Pin_Gap"),
            ("Main+Gap", "Overlay", "Pin_Review"),
            ("Pin_Gap", "Accept", "Inspection")
        };
        private Action<string> openPracticeSamplesAction;
        private readonly List<Border> resultCells = new();
        private readonly List<TextBlock> resultTexts = new();
        private readonly List<Border> brightnessOutputCells = new();
        private readonly List<TextBlock> brightnessOutputTexts = new();
        private readonly List<Border> histogramBars = new();
        private readonly List<TextBlock> histogramLabels = new();
        private readonly List<Border> arithmeticResultCells = new();
        private readonly List<TextBlock> arithmeticResultTexts = new();
        private readonly List<Border> filterOutputCells = new();
        private readonly List<TextBlock> filterOutputTexts = new();
        private readonly List<Border> morphologyOutputCells = new();
        private readonly List<TextBlock> morphologyOutputTexts = new();
        private readonly List<Border> blobOutputCells = new();
        private readonly List<TextBlock> blobOutputTexts = new();
        private readonly List<Border> contourOutputCells = new();
        private readonly List<TextBlock> contourOutputTexts = new();
        private readonly List<Border> edgeLineOutputCells = new();
        private readonly List<TextBlock> edgeLineOutputTexts = new();
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
        private readonly List<Border> layerRecipeLayerCells = new();
        private readonly List<TextBlock> layerRecipeLayerTexts = new();
        private readonly List<Border> layerRecipeFlowCells = new();
        private readonly List<TextBlock> layerRecipeFlowTexts = new();
        private bool animationForward = true;
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
            this.maxValue = ClampToByte(maxValue);
            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
            animationTimer.Tick += AnimationTimer_Tick;
            BuildSampleCells();
            BuildBrightnessCells();
            BuildArithmeticCells();
            BuildFilterCells();
            BuildMorphologyCells();
            BuildBlobCells();
            BuildContourCells();
            BuildEdgeLineCells();
            BuildLineDistanceCells();
            BuildMatchingCells();
            BuildFeatureMatchingCells();
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
            UpdateMorphologyGuide();
            UpdateBlobGuide();
            UpdateContourGuide();
            UpdateEdgeLineGuide();
            UpdateLineDistanceGuide();
            UpdateMatchingGuide();
            UpdateFeatureMatchingGuide();
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

        public string SelectedTopicDocumentFileNameForTest => ResolveSelectedTopicDocumentFileName(topicList.SelectedIndex);

        public string SelectedTopicLearnPathIdForTest => ResolveSelectedTopicLearnPathId(topicList.SelectedIndex);

        public string SelectedTopicPracticeTextForTest => txtTopicPractice.Text ?? string.Empty;

        public bool CanOpenPracticeSamplesForTest => btnPracticeSamples.IsEnabled;

        public void SetOpenPracticeSamplesAction(Action<string> action)
        {
            openPracticeSamplesAction = action;
            btnPracticeSamples.IsEnabled = action != null;
        }

        public double BrightnessOffsetForTest
        {
            get => brightnessOffsetSlider.Value;
            set => brightnessOffsetSlider.Value = Math.Max(-80, Math.Min(80, value));
        }

        public string BrightnessFormulaTextForTest => txtBrightnessFormula.Text ?? string.Empty;

        public int ArithmeticModeIndexForTest
        {
            get => arithmeticModeCombo.SelectedIndex;
            set => arithmeticModeCombo.SelectedIndex = Math.Max(0, Math.Min(4, value));
        }

        public string ArithmeticFormulaTextForTest => txtArithmeticFormula.Text ?? string.Empty;

        public int FilterModeIndexForTest
        {
            get => filterModeCombo.SelectedIndex;
            set => filterModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        public string FilterFormulaTextForTest => txtFilterFormula.Text ?? string.Empty;

        public int MorphologyModeIndexForTest
        {
            get => morphologyModeCombo.SelectedIndex;
            set => morphologyModeCombo.SelectedIndex = Math.Max(0, Math.Min(3, value));
        }

        public string MorphologyFormulaTextForTest => txtMorphologyFormula.Text ?? string.Empty;

        public double BlobMinAreaForTest
        {
            get => blobMinAreaSlider.Value;
            set => blobMinAreaSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        public string BlobFormulaTextForTest => txtBlobFormula.Text ?? string.Empty;

        public int ContourDrawModeIndexForTest
        {
            get => contourDrawModeCombo.SelectedIndex;
            set => contourDrawModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        public string ContourFormulaTextForTest => txtContourFormula.Text ?? string.Empty;

        public double EdgeThresholdForTest
        {
            get => edgeThresholdSlider.Value;
            set => edgeThresholdSlider.Value = Math.Max(10, Math.Min(150, value));
        }

        public string EdgeLineFormulaTextForTest => txtEdgeLineFormula.Text ?? string.Empty;

        public double LineDistanceRangeMaxForTest
        {
            get => lineDistanceRangeMaxSlider.Value;
            set => lineDistanceRangeMaxSlider.Value = Math.Max(0, Math.Min(2, value));
        }

        public string LineDistanceFormulaTextForTest => txtLineDistanceFormula.Text ?? string.Empty;

        public double MatchingThresholdForTest
        {
            get => matchingThresholdSlider.Value;
            set => matchingThresholdSlider.Value = Math.Max(0.50, Math.Min(1.00, value));
        }

        public string MatchingFormulaTextForTest => txtMatchingFormula.Text ?? string.Empty;

        public double FeatureGoodMatchMinForTest
        {
            get => featureGoodMatchMinSlider.Value;
            set => featureGoodMatchMinSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        public string FeatureMatchingFormulaTextForTest => txtFeatureMatchingFormula.Text ?? string.Empty;

        public double LayerRecipeSelectedStepForTest
        {
            get => layerRecipeStepSlider.Value;
            set => layerRecipeStepSlider.Value = Math.Max(1, Math.Min(4, value));
        }

        public string LayerRecipeFormulaTextForTest => txtLayerRecipeFormula.Text ?? string.Empty;

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

        public void ApplyForTest()
        {
            ApplyButton_Click(this, new RoutedEventArgs());
        }

        protected override void OnClosed(EventArgs e)
        {
            animationTimer.Stop();
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
            brightnessOutputCells.Clear();
            brightnessOutputTexts.Clear();
            histogramBars.Clear();
            histogramLabels.Clear();

            foreach (int value in brightnessSampleValues)
            {
                brightnessInputGrid.Children.Add(CreateCell(value.ToString(CultureInfo.InvariantCulture), value));
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
            filterOutputCells.Clear();
            filterOutputTexts.Clear();

            foreach (int value in filterSampleValues)
            {
                filterInputGrid.Children.Add(CreateCell(value.ToString(CultureInfo.InvariantCulture), value));
                Border outputCell = CreateCell(string.Empty, value);
                TextBlock outputText = (TextBlock)outputCell.Child;
                filterOutputGrid.Children.Add(outputCell);
                filterOutputCells.Add(outputCell);
                filterOutputTexts.Add(outputText);
            }
        }

        private void BuildArithmeticCells()
        {
            arithmeticInputAGrid.Children.Clear();
            arithmeticInputBGrid.Children.Clear();
            arithmeticResultGrid.Children.Clear();
            arithmeticResultCells.Clear();
            arithmeticResultTexts.Clear();

            for (int i = 0; i < arithmeticInputAValues.Length; i++)
            {
                int inputA = arithmeticInputAValues[i];
                int inputB = arithmeticInputBValues[i];
                arithmeticInputAGrid.Children.Add(CreateCell(inputA.ToString(CultureInfo.InvariantCulture), inputA));
                arithmeticInputBGrid.Children.Add(CreateCell(inputB.ToString(CultureInfo.InvariantCulture), inputB));

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
            morphologyOutputCells.Clear();
            morphologyOutputTexts.Clear();

            foreach (int value in morphologySampleValues)
            {
                morphologyInputGrid.Children.Add(CreateBinaryCell(value));
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
            blobOutputCells.Clear();
            blobOutputTexts.Clear();

            foreach (int value in blobSampleValues)
            {
                blobInputGrid.Children.Add(CreateBinaryCell(value));
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
            contourOutputCells.Clear();
            contourOutputTexts.Clear();

            foreach (int value in contourSampleValues)
            {
                contourInputGrid.Children.Add(CreateBinaryCell(value));
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
            edgeLineOutputCells.Clear();
            edgeLineOutputTexts.Clear();

            foreach (int value in edgeLineSampleValues)
            {
                edgeLineInputGrid.Children.Add(CreateSmallValueCell(value.ToString(CultureInfo.InvariantCulture), value));
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
                        cell.Background = new SolidColorBrush(Color.FromRgb(34, 132, 145));
                        text.Foreground = Brushes.White;
                        text.Text = "L";
                    }
                    else if (x == lineDistanceRightEdges[y])
                    {
                        cell.Background = new SolidColorBrush(Color.FromRgb(64, 143, 77));
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
                    cell.Background = new SolidColorBrush(Color.FromRgb(34, 132, 145));
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
                int source = brightnessSampleValues[i];
                int result = ClampToByte(source + offset);
                brightnessOutputCells[i].Background = CreateGrayBrush(result);
                brightnessOutputTexts[i].Foreground = result > 128 ? Brushes.Black : Brushes.White;
                brightnessOutputTexts[i].Text = source.ToString(CultureInfo.InvariantCulture)
                    + " -> "
                    + result.ToString(CultureInfo.InvariantCulture);

                int bin = Math.Min(7, result / 32);
                bins[bin]++;
            }

            int maxCount = Math.Max(1, bins.Max());
            for (int i = 0; i < histogramBars.Count; i++)
            {
                int low = i * 32;
                int high = i == 7 ? 255 : low + 31;
                double height = bins[i] == 0 ? 4D : 120D * bins[i] / maxCount;
                histogramBars[i].Height = height;
                histogramLabels[i].Text = low.ToString(CultureInfo.InvariantCulture)
                    + "-"
                    + high.ToString(CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + bins[i].ToString(CultureInfo.InvariantCulture);
            }
        }

        private void UpdateArithmeticGuide()
        {
            string mode = GetSelectedArithmeticMode();
            txtArithmeticFormula.Text = mode + ": A/B -> Output";
            txtArithmeticMeaning.Text = mode switch
            {
                "Add" => "Add brightens or combines two evidence layers. Watch for saturation at 255.",
                "Subtract" => "Subtract keeps pixels where A is stronger than B. Values below 0 are clipped.",
                "Bitwise AND" => "Bitwise AND keeps pixels that are active in both binary masks.",
                "Bitwise OR" => "Bitwise OR combines candidate masks from either input.",
                _ => "AbsDiff highlights changed pixels by using the absolute difference between A and B."
            };

            for (int i = 0; i < arithmeticResultCells.Count; i++)
            {
                int inputA = arithmeticInputAValues[i];
                int inputB = arithmeticInputBValues[i];
                int result = mode switch
                {
                    "Add" => ClampToByte(inputA + inputB),
                    "Subtract" => ClampToByte(inputA - inputB),
                    "Bitwise AND" => inputA & inputB,
                    "Bitwise OR" => inputA | inputB,
                    _ => Math.Abs(inputA - inputB)
                };

                arithmeticResultCells[i].Background = CreateGrayBrush(result);
                arithmeticResultTexts[i].Foreground = result > 128 ? Brushes.Black : Brushes.White;
                arithmeticResultTexts[i].Text = result.ToString(CultureInfo.InvariantCulture);
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

            for (int i = 0; i < filterSampleValues.Length; i++)
            {
                int value = i == 4 ? result : filterSampleValues[i];
                filterOutputCells[i].Background = CreateGrayBrush(value);
                filterOutputTexts[i].Foreground = value > 128 ? Brushes.Black : Brushes.White;
                filterOutputTexts[i].Text = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void UpdateMorphologyGuide()
        {
            string mode = GetSelectedMorphologyMode();
            bool[] source = morphologySampleValues.Select(value => value > 0).ToArray();
            bool[] result = mode switch
            {
                "Dilation" => Dilate(source),
                "Opening" => Dilate(Erode(source)),
                "Closing" => Erode(Dilate(source)),
                _ => Erode(source)
            };

            txtMorphologyFormula.Text = mode switch
            {
                "Dilation" => "Dilation = if any 3x3 neighbor is white, output white",
                "Opening" => "Opening = erosion then dilation",
                "Closing" => "Closing = dilation then erosion",
                _ => "Erosion = only if all 3x3 neighbors are white, output white"
            };
            txtMorphologyMeaning.Text = mode switch
            {
                "Dilation" => "팽창은 흰 영역을 키웁니다. 끊어진 부분이나 작은 구멍을 메우는 데 도움이 되지만 대상이 두꺼워질 수 있습니다.",
                "Opening" => "열기는 침식 후 팽창입니다. 작은 흰 점 노이즈를 지운 뒤 원래 크기에 가깝게 되돌릴 때 씁니다.",
                "Closing" => "닫기는 팽창 후 침식입니다. 작은 검은 구멍이나 끊어진 틈을 메우는 데 씁니다.",
                _ => "침식은 흰 영역을 줄입니다. 작은 흰 점 노이즈를 제거하지만 얇은 대상은 사라질 수 있습니다."
            };

            for (int i = 0; i < result.Length; i++)
            {
                int value = result[i] ? 255 : 0;
                morphologyOutputCells[i].Background = CreateGrayBrush(value);
                morphologyOutputTexts[i].Foreground = value > 0 ? Brushes.Black : Brushes.White;
                morphologyOutputTexts[i].Text = value > 0 ? "1" : "0";
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
                Brush background = accepted
                    ? new SolidColorBrush(label == 1 ? Color.FromRgb(34, 132, 145) : Color.FromRgb(64, 143, 77))
                    : new SolidColorBrush(Color.FromRgb(82, 91, 105));
                PaintBlobCell(i, background, Brushes.White, accepted ? ((char)('A' + label - 1)).ToString() : "x");
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

            for (int i = 0; i < contourSampleValues.Length; i++)
            {
                int label = labels[i];
                bool isRejected = label > 0 && !accepted[i];
                bool isContour = contour[i];
                bool isBox = bounds.HasValue && IsOnBounds(i, 7, bounds.Value);
                bool drawContour = mode == "Contour" || mode == "Contour + box";
                bool drawBox = mode == "Bounding box" || mode == "Contour + box";

                if (isRejected)
                {
                    PaintContourCell(i, new SolidColorBrush(Color.FromRgb(82, 91, 105)), Brushes.White, "x");
                }
                else if (mode == "Contour + box" && drawBox && isBox)
                {
                    PaintContourCell(i, new SolidColorBrush(Color.FromRgb(64, 143, 77)), Brushes.White, "B");
                }
                else if (drawContour && isContour)
                {
                    PaintContourCell(i, new SolidColorBrush(Color.FromRgb(34, 132, 145)), Brushes.White, "C");
                }
                else if (drawBox && isBox)
                {
                    PaintContourCell(i, new SolidColorBrush(Color.FromRgb(64, 143, 77)), Brushes.White, "B");
                }
                else if (accepted[i])
                {
                    PaintContourCell(i, new SolidColorBrush(Color.FromRgb(229, 244, 247)), Brushes.Black, "1");
                }
                else
                {
                    PaintContourCell(i, Brushes.Black, Brushes.White, "0");
                }
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

            for (int i = 0; i < edgeLineSampleValues.Length; i++)
            {
                int x = i % 5;
                if (edges[i] && x == bestColumn && bestRun >= 3)
                {
                    PaintEdgeLineCell(i, new SolidColorBrush(Color.FromRgb(64, 143, 77)), Brushes.White, "L");
                }
                else if (edges[i])
                {
                    PaintEdgeLineCell(i, new SolidColorBrush(Color.FromRgb(34, 132, 145)), Brushes.White, "E");
                }
                else
                {
                    int shade = Math.Min(220, strengths[i] + 35);
                    PaintEdgeLineCell(i, CreateGrayBrush(shade), shade > 128 ? Brushes.Black : Brushes.White, strengths[i].ToString(CultureInfo.InvariantCulture));
                }
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
            bool rangeOk = range <= rangeMax;

            txtLineDistanceRangeMax.Text = rangeMax.ToString("0.00", CultureInfo.InvariantCulture) + " px";
            txtLineDistanceFormula.Text = "DistancePxAvg="
                + avg.ToString("0.0", CultureInfo.InvariantCulture)
                + ", DistancePxRange="
                + range.ToString(CultureInfo.InvariantCulture)
                + ", DistanceMmAvg="
                + avgMm.ToString("0.000", CultureInfo.InvariantCulture);
            txtLineDistanceMeaning.Text = rangeOk
                ? "평균과 줄별 흔들림이 모두 기준 안입니다. 실제 레시피도 DistanceAvg와 DistanceRange를 함께 판정합니다."
                : "평균값만 보면 지나칠 수 있지만 줄별 거리 차이가 큽니다. Range/Max 게이트로 긴 측정선을 NG 처리해야 합니다.";

            for (int i = 0; i < distances.Length; i++)
            {
                bool outlier = !rangeOk && distances[i] == max;
                Brush background = outlier
                    ? new SolidColorBrush(Color.FromRgb(185, 91, 36))
                    : new SolidColorBrush(Color.FromRgb(64, 143, 77));
                PaintLineDistanceCell(i, background, Brushes.White, distances[i].ToString(CultureInfo.InvariantCulture) + " px");
            }
        }

        private void UpdateMatchingGuide()
        {
            double threshold = matchingThresholdSlider.Value;
            double[] scores = matchingCandidatePositions
                .Select(position => CalculateTemplateScore(position.X, position.Y))
                .ToArray();
            double bestScore = scores.Max();
            int bestIndex = Array.IndexOf(scores, bestScore);
            bool pass = bestScore >= threshold;

            txtMatchingThreshold.Text = threshold.ToString("0.00", CultureInfo.InvariantCulture);
            txtMatchingFormula.Text = "BestScore="
                + bestScore.ToString("0.00", CultureInfo.InvariantCulture)
                + ", Threshold="
                + threshold.ToString("0.00", CultureInfo.InvariantCulture)
                + ", Result="
                + (pass ? "OK" : "NG");
            txtMatchingMeaning.Text = pass
                ? "최고 점수가 기준 이상이면 Template 위치 후보로 볼 수 있습니다. 회전/스케일 변화가 크면 EdgeBasedMatching이나 FeatureMatching을 검토합니다."
                : "최고 점수가 기준보다 낮으면 NG입니다. Template, ROI, 조명, ScoreThreshold를 순서대로 확인합니다.";

            PaintMatchingSearchGrid(bestIndex);
            for (int i = 0; i < scores.Length; i++)
            {
                bool isBest = i == bestIndex;
                bool accepted = scores[i] >= threshold;
                Brush background = isBest && accepted
                    ? new SolidColorBrush(Color.FromRgb(64, 143, 77))
                    : accepted
                        ? new SolidColorBrush(Color.FromRgb(34, 132, 145))
                        : new SolidColorBrush(Color.FromRgb(82, 91, 105));
                PaintMatchingScoreCell(i, background, Brushes.White, scores[i].ToString("0.00", CultureInfo.InvariantCulture));
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

            PaintFeaturePointGrid(featureReferenceCells, featureReferenceTexts, featureReferencePoints, goodMatches, "K");
            PaintFeaturePointGrid(featureSceneCells, featureSceneTexts, featureScenePoints, goodMatches, "M");
            for (int i = 0; i < featureMatchScores.Length; i++)
            {
                Brush background = goodMatches[i]
                    ? new SolidColorBrush(Color.FromRgb(64, 143, 77))
                    : new SolidColorBrush(Color.FromRgb(82, 91, 105));
                PaintFeatureScoreCell(i, background, Brushes.White, featureMatchScores[i].ToString("0.00", CultureInfo.InvariantCulture));
            }
        }

        private void UpdateLayerRecipeGuide()
        {
            int selected = Math.Max(1, Math.Min(4, (int)Math.Round(layerRecipeStepSlider.Value)));
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
                1 => "첫 Step은 Main을 읽어 이진 결과 레이어를 만듭니다. Output 생성은 Input 선택을 자동 변경하지 않습니다.",
                2 => "LineDistance는 이전 Output을 Input으로 명시해서 측정합니다. Preview/Run은 사용자가 눌러야 합니다.",
                3 => "OverlayMerge는 리뷰용 결과 레이어를 만듭니다. 원본 Main 레이어 자체를 덮어쓰지 않습니다.",
                _ => "Recipe는 Step route와 acceptance 기준을 저장합니다. 검증과 import도 명시 액션으로 처리합니다."
            };

            for (int i = 0; i < layerRecipeFlowCells.Count; i++)
            {
                bool selectedRow = i / 4 == index;
                layerRecipeFlowCells[i].Background = selectedRow
                    ? new SolidColorBrush(Color.FromRgb(34, 132, 145))
                    : CreateGrayBrush(230);
                layerRecipeFlowTexts[i].Foreground = selectedRow ? Brushes.White : Brushes.Black;
            }

            for (int i = 0; i < layerRecipeLayerCells.Count; i++)
            {
                bool routeLayer = layerRecipeLayers[i] == step.Input
                    || layerRecipeLayers[i] == step.Output
                    || step.Input.Contains(layerRecipeLayers[i], StringComparison.Ordinal);
                layerRecipeLayerCells[i].Background = routeLayer
                    ? new SolidColorBrush(Color.FromRgb(64, 143, 77))
                    : CreateGrayBrush(230);
                layerRecipeLayerTexts[i].Foreground = routeLayer ? Brushes.White : Brushes.Black;
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
            txtGeometryMeaning.Text = "A Rect ROI or measurement point from the source layer is not automatically valid on the transformed layer. Recreate or verify the ROI after Preview.";
            geometryRotateTransform.Angle = angle;
            geometryScaleTransform.ScaleX = scale / 100D;
            geometryScaleTransform.ScaleY = scale / 100D;
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
                + " -> OutputLayer=HSV_Mask, future metric=MaskPixelRatio after runner support, or downstream ResultCount/Area";
            txtColorHsvMeaning.Text = value < 110
                ? "Low Value means the color is dark; hue alone may be unstable, so compare Mean/Histogram evidence before accepting the mask."
                : "Hue chooses the color family, Saturation rejects gray background, and Value rejects dark pixels before explicit Preview/Run.";

            Color color = CreateColorFromOpenCvHsv(hue, 220, value);
            colorHsvPreviewSwatch.Background = new SolidColorBrush(color);
            txtColorHsvPreviewLabel.Text = "H "
                + hue.ToString(CultureInfo.InvariantCulture)
                + " / V "
                + value.ToString(CultureInfo.InvariantCulture);
            txtColorHsvPreviewLabel.Foreground = value < 150 ? Brushes.White : Brushes.Black;
        }

        private static void PaintFeaturePointGrid(
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
                bool good = goodMatches[i];
                cells[index].Background = good
                    ? new SolidColorBrush(Color.FromRgb(34, 132, 145))
                    : new SolidColorBrush(Color.FromRgb(82, 91, 105));
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
            (int X, int Y) best = matchingCandidatePositions[bestIndex];
            for (int i = 0; i < matchingSearchValues.Length; i++)
            {
                int x = i % 5;
                int y = i / 5;
                bool inBest = x >= best.X && x < best.X + 2 && y >= best.Y && y < best.Y + 2;
                int value = matchingSearchValues[i];

                if (inBest)
                {
                    matchingSearchCells[i].Background = new SolidColorBrush(Color.FromRgb(64, 143, 77));
                    matchingSearchTexts[i].Foreground = Brushes.White;
                    matchingSearchTexts[i].Text = value > 0 ? "B" : "0";
                }
                else
                {
                    matchingSearchCells[i].Background = CreateGrayBrush(value > 0 ? 230 : 20);
                    matchingSearchTexts[i].Foreground = value > 0 ? Brushes.Black : Brushes.White;
                    matchingSearchTexts[i].Text = value > 0 ? "1" : "0";
                }
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
                UpdateSelectedTopic();
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
            txtTopicPractice.Text = ResolveSelectedTopicPracticeText(topicList.SelectedIndex);
            if (isPixelTopic)
            {
                txtTopicTitle.Text = "0. 커리큘럼 / 영상 기초";
                txtTopicSubtitle.Text = "세부 문서: Arithmetic, RotateScale, Color/HSV, Pipeline/Layer, Metrics/Acceptance.";
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
                txtTopicSubtitle.Text = "EdgeDetection: pixels; EdgeBasedMatching: edge shape; LineGauge: fitted line; LineDistance: gap/pitch.";
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
                txtTopicSubtitle.Text = "레이어 이름, Step route, Recipe 저장 단위를 읽고 명시 실행 원칙을 확인합니다.";
                UpdateLayerRecipeGuide();
                return;
            }

            if (isMetricsAcceptanceTopic)
            {
                txtTopicTitle.Text = "13. Metrics / Acceptance";
                txtTopicSubtitle.Text = "Connect tool results to OK/NG gates with Good/Bad samples, and add range/max gates when averages can hide outliers.";
                return;
            }

            if (isArithmeticTopic)
            {
                txtTopicTitle.Text = "14. Arithmetic / Logic";
                txtTopicSubtitle.Text = "Combine, compare, or mask layers with Add, Subtract, AbsDiff, Bitwise AND, and Bitwise OR before explicit Preview/Run.";
                UpdateArithmeticGuide();
                return;
            }

            if (isGeometryTopic)
            {
                txtTopicTitle.Text = "15. Geometry Transform";
                txtTopicSubtitle.Text = "Use RotateScale to review rotation, scale, output size, border area, and ROI coordinate changes before explicit Preview/Run.";
                UpdateGeometryGuide();
                return;
            }

            if (isColorHsvTopic)
            {
                txtTopicTitle.Text = "16. Color / HSV";
                txtTopicSubtitle.Text = "Separate hue, saturation, and value so color-range masks can be reviewed with explicit Preview/Run and metric gates.";
                UpdateColorHsvGuide();
                return;
            }

            if (isEdgeBasedMatchingTopic)
            {
                txtTopicTitle.Text = "12. EdgeBasedMatching";
                txtTopicSubtitle.Text = "Template appearance 대신 edge shape와 score를 기준으로 대상을 찾고, Good/Bad 샘플에서 ResultCount와 ScoreMax를 명시 Run Review로 확인합니다.";
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
                UpdateBrightnessGuide();
            }
        }

        private void ArithmeticModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateArithmeticGuide();
            }
        }

        private void FilterModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateFilterGuide();
            }
        }

        private void MorphologyModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                UpdateMorphologyGuide();
            }
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
                UpdateContourGuide();
            }
        }

        private void EdgeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateEdgeLineGuide();
            }
        }

        private void LineDistanceRangeMaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateLineDistanceGuide();
            }
        }

        private void MatchingThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateMatchingGuide();
            }
        }

        private void FeatureGoodMatchMinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateFeatureMatchingGuide();
            }
        }

        private void LayerRecipeStepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateLayerRecipeGuide();
            }
        }

        private void GeometrySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateGeometryGuide();
            }
        }

        private void ColorHsvSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                UpdateColorHsvGuide();
            }
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
            return index >= 0 && index <= 16 ? index : 2;
        }

        private static string ResolveSelectedTopicDocumentFileName(int index)
        {
            return index switch
            {
                0 => "OPENVISIONLAB_LEARN_CURRICULUM.md",
                1 => "LEARN_MEAN.md",
                2 => "LEARN_THRESHOLD.md",
                3 => "LEARN_FILTER.md",
                4 => "LEARN_MORPHOLOGY.md",
                5 => "LEARN_BLOB.md",
                6 => "LEARN_CONTOUR.md",
                7 => "LEARN_EDGE_DETECTION.md",
                8 => "LEARN_LINE.md",
                9 => "LEARN_MATCHING.md",
                10 => "LEARN_FEATURE_MATCHING.md",
                11 => "LEARN_PIPELINE_LAYER_ROUTING.md",
                12 => "LEARN_EDGE_BASED_MATCHING.md",
                13 => "LEARN_METRICS_ACCEPTANCE.md",
                14 => "LEARN_ARITHMETIC.md",
                15 => "LEARN_GEOMETRY_TRANSFORM.md",
                16 => "LEARN_COLOR_HSV.md",
                _ => "README.md"
            };
        }

        private static string ResolveSelectedTopicLearnPathId(int index)
        {
            return index switch
            {
                1 => "mean",
                2 => "preprocess",
                3 => "preprocess",
                4 => "preprocess",
                5 => "blob",
                6 => "contour",
                7 => "preprocess",
                8 => "line",
                9 => "template-matching",
                10 => "feature-matching",
                12 => "edge-matching",
                14 => "preprocess",
                15 => "geometry",
                16 => "mean",
                _ => "all"
            };
        }

        private static string ResolveSelectedTopicPracticeText(int index)
        {
            return index switch
            {
                0 => "Practice: Sample Picker path 'all', then open the related Tool View and run Preview manually.",
                1 => "Practice: Sample Picker path 'mean', then review Mean/Histogram metrics with explicit Run Review.",
                2 => "Practice: Tool View Threshold or Sample Picker path 'preprocess', then click Preview manually.",
                3 => "Practice: Tool View Filter or Sample Picker path 'preprocess', then click Preview manually.",
                4 => "Practice: Tool View Morphology or Sample Picker path 'preprocess', then click Preview manually.",
                5 => "Practice: Sample Picker path 'blob', then open the Blob Tool View or Pipeline Review manually.",
                6 => "Practice: Sample Picker path 'contour', then open the Contour Tool View or Pipeline Review manually.",
                7 => "Practice: Sample Picker path 'preprocess', then confirm EdgeDetection or Line points before measurement.",
                8 => "Practice: Sample Picker path 'line', then compare Line distance average and range after explicit Run Review.",
                9 => "Practice: Sample Picker path 'template-matching', then compare Template ScoreMax and ResultCount after explicit Run Review.",
                10 => "Practice: Sample Picker path 'feature-matching', then compare Feature match count and score after explicit Run Review.",
                11 => "Practice: Sample Picker path 'all', then open Pipeline Review and run review manually.",
                12 => "Practice: Sample Picker path 'edge-matching', then compare EdgeBasedMatching ScoreMax and ResultCount after explicit Run Review.",
                13 => "Practice: Sample Picker path 'all', then compare Metrics/Acceptance gates across Good/Bad samples after explicit Run Review.",
                14 => "Practice: Sample Picker path 'preprocess', then open Arithmetic Tool View and click Preview manually.",
                15 => "Practice: Sample Picker path 'geometry', then open RotateScale and click Preview manually.",
                16 => "Practice: Sample Picker path 'mean' is a temporary brightness bridge, not HSV sample evidence. Run Mean manually, then use HSV/Histogram only for review.",
                _ => "Practice: Sample Picker path 'all', then open the related Tool View and run Preview manually."
            };
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
