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
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    /// <summary>Owns Matching-family controls, rendering and timer lifetime; presenters own teaching decisions.</summary>
    public sealed partial class MatchingLearnView : UserControl
    {
        private readonly MatchingLearnPresenter matchingPresenter = new();
        private readonly FeatureMatchingLearnPresenter featureMatchingPresenter = new();
        private readonly DispatcherTimer matchingAnimationTimer;
        private readonly DispatcherTimer featureMatchingAnimationTimer;
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
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
        private Action<VISION_MENU> openRelatedToolAction;
        private int selectedTopicIndex;

        public MatchingLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            matchingAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            featureMatchingAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            BuildMatchingCells();
            BuildFeatureMatchingCells();
            matchingThresholdSlider.Value = 0.85;
            featureGoodMatchMinSlider.Value = 4;
            UpdateMatchingGuide();
            UpdateFeatureMatchingGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal void SelectTopic(int topicIndex)
        {
            selectedTopicIndex = topicIndex;
            bool template = topicIndex == 9 || topicIndex == 12;
            bool feature = topicIndex == 10;
            Visibility = template || feature ? Visibility.Visible : Visibility.Collapsed;
            matchingTopicPanel.Visibility = template ? Visibility.Visible : Visibility.Collapsed;
            featureMatchingTopicPanel.Visibility = feature ? Visibility.Visible : Visibility.Collapsed;
            if (template)
            {
                ConfigureMatchingToolLink(edgeBased: topicIndex == 12);
                UpdateMatchingGuide();
            }
            else if (feature)
            {
                UpdateFeatureMatchingGuide();
            }
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            btnMatchingOpenTool.IsEnabled = action != null;
            btnFeatureMatchingOpenTool.IsEnabled = action != null;
        }

        internal void StopAnimations()
        {
            matchingAnimationTimer.Stop();
            featureMatchingAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            matchingAnimationTimer.Tick += MatchingAnimationTimer_Tick;
            featureMatchingAnimationTimer.Tick += FeatureMatchingAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Removing/rehosting the View retires callbacks; loading it again never starts playback.
            StopAnimations();
            matchingAnimationTimer.Tick -= MatchingAnimationTimer_Tick;
            featureMatchingAnimationTimer.Tick -= FeatureMatchingAnimationTimer_Tick;
            btnMatchingPlay.Content = "Play";
            btnFeatureMatchingPlay.Content = "Play";
        }

        internal bool AreMatchingFamilyDecisionsCollapsedForTest =>
            !matchingFamilyDecisionExpander.IsExpanded && !featureMatchingFamilyDecisionExpander.IsExpanded;

        internal bool CanOpenMatchingToolForTest => btnMatchingOpenTool.IsEnabled;

        internal bool CanOpenEdgeBasedMatchingToolForTest =>
            btnMatchingOpenTool.IsEnabled
            && string.Equals(Convert.ToString(btnMatchingOpenTool.Tag, CultureInfo.InvariantCulture), nameof(VISION_MENU.EdgeBasedMatching), StringComparison.Ordinal);

        internal bool CanOpenFeatureMatchingToolForTest => btnFeatureMatchingOpenTool.IsEnabled;

        internal string MatchingToolLocationTitleForTest => txtMatchingToolLocationTitle.Text ?? string.Empty;

        internal string MatchingToolLocationDetailForTest => txtMatchingToolLocationDetail.Text ?? string.Empty;

        internal string EdgeBasedMatchingToolLocationTitleForTest => txtMatchingToolLocationTitle.Text ?? string.Empty;

        internal string EdgeBasedMatchingToolLocationDetailForTest => txtMatchingToolLocationDetail.Text ?? string.Empty;

        internal string FeatureMatchingToolLocationTitleForTest => txtFeatureMatchingToolLocationTitle.Text ?? string.Empty;

        internal string FeatureMatchingToolLocationDetailForTest => txtFeatureMatchingToolLocationDetail.Text ?? string.Empty;

        internal double MatchingThresholdForTest
        {
            get => matchingThresholdSlider.Value;
            set => matchingThresholdSlider.Value = Math.Max(0.50, Math.Min(1.00, value));
        }

        internal string MatchingFormulaTextForTest => txtMatchingFormula.Text ?? string.Empty;

        internal int MatchingAnimationStepForTest => matchingPresenter.AnimationStep;

        internal string MatchingAnimationStatusTextForTest => txtMatchingAnimationStatus.Text ?? string.Empty;

        internal double FeatureGoodMatchMinForTest
        {
            get => featureGoodMatchMinSlider.Value;
            set => featureGoodMatchMinSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        internal string FeatureMatchingFormulaTextForTest => txtFeatureMatchingFormula.Text ?? string.Empty;

        internal int FeatureMatchingAnimationStepForTest => featureMatchingPresenter.AnimationStep;

        internal string FeatureMatchingAnimationStatusTextForTest => txtFeatureMatchingAnimationStatus.Text ?? string.Empty;

        internal void ResetMatchingAnimationForTest()
        {
            ResetMatchingAnimation();
        }

        internal void AdvanceMatchingAnimationForTest()
        {
            AdvanceMatchingAnimation();
        }

        internal void ToggleMatchingAnimationForTest()
        {
            MatchingPlayButton_Click(this, new RoutedEventArgs());
        }

        internal void ResetFeatureMatchingAnimationForTest()
        {
            ResetFeatureMatchingAnimation();
        }

        internal void AdvanceFeatureMatchingAnimationForTest()
        {
            AdvanceFeatureMatchingAnimation();
        }

        internal void ToggleFeatureMatchingAnimationForTest()
        {
            FeatureMatchingPlayButton_Click(this, new RoutedEventArgs());
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

            for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.SearchValues.Count; i++)
            {
                int value = OpenVisionLearnMatchingSimulationModel.SearchValues[i];
                Border cell = CreateSmallValueCell(value > 0 ? "1" : "0", value > 0 ? 230 : 20);
                TextBlock text = (TextBlock)cell.Child;
                matchingSearchGrid.Children.Add(cell);
                matchingSearchCells.Add(cell);
                matchingSearchTexts.Add(text);
            }

            foreach (int value in OpenVisionLearnMatchingSimulationModel.TemplateValues)
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

            for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.CandidatePositions.Count; i++)
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

            for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.FeatureScores.Count; i++)
            {
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                featureMatchScoreGrid.Children.Add(outputCell);
                featureMatchScoreCells.Add(outputCell);
                featureMatchScoreTexts.Add(outputText);
            }
        }

        private void UpdateMatchingGuide()
        {
            matchingPresenter.Update(matchingThresholdSlider.Value, selectedTopicIndex == 12);
            txtMatchingThreshold.Text = matchingPresenter.ThresholdText;
            txtMatchingConceptTitle.Text = matchingPresenter.ConceptTitle;
            txtMatchingConceptDescription.Text = matchingPresenter.ConceptDescription;
            txtMatchingThresholdDescription.Text = matchingPresenter.ThresholdDescription;
            txtMatchingSearchTitle.Text = matchingPresenter.SearchTitle;
            txtMatchingSearchSubtitle.Text = matchingPresenter.SearchSubtitle;
            txtMatchingScoreTitle.Text = matchingPresenter.ScoreTitle;
            btnMatchingPlay.ToolTip = matchingPresenter.PlayToolTip;
            UpdateMatchingTemplateLabels();
            txtMatchingFormula.Text = matchingPresenter.FormulaText;
            txtMatchingMeaning.Text = matchingPresenter.MeaningText;
            RenderMatchingAnimationFrame();
        }

        private void PaintMatchingAnimationFrame()
        {
            matchingPresenter.Update(matchingThresholdSlider.Value, selectedTopicIndex == 12);
            RenderMatchingAnimationFrame();
        }

        private void RenderMatchingAnimationFrame()
        {
            double[] scores = matchingPresenter.Evaluation.Scores;
            int bestIndex = matchingPresenter.Evaluation.BestIndex;
            int visibleStep = matchingPresenter.AnimationStep;
            if (visibleStep == 0)
            {
                PaintMatchingSearchGrid(-1);
                for (int i = 0; i < scores.Length; i++)
                {
                    PaintMatchingScoreCell(i, animationNeutralBrush, Brushes.White, "-");
                }

                txtMatchingAnimationStatus.Text = matchingPresenter.AnimationStatusText;
                return;
            }

            if (visibleStep == 1)
            {
                PaintMatchingCandidateScanGrid();
                for (int i = 0; i < scores.Length; i++)
                {
                    PaintMatchingScoreCell(i, animationCandidateBrush, Brushes.White, scores[i].ToString("0.00", CultureInfo.InvariantCulture));
                }

                txtMatchingAnimationStatus.Text = matchingPresenter.AnimationStatusText;
                return;
            }

            PaintMatchingSearchGrid(bestIndex);
            for (int i = 0; i < scores.Length; i++)
            {
                bool isBest = i == bestIndex;
                bool accepted = matchingPresenter.IsCandidateAccepted(i);
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

            txtMatchingAnimationStatus.Text = matchingPresenter.AnimationStatusText;
        }

        private void UpdateMatchingTemplateLabels()
        {
            int index = 0;
            foreach (Border cell in matchingTemplateGrid.Children.OfType<Border>())
            {
                if (cell.Child is TextBlock text
                    && index < OpenVisionLearnMatchingSimulationModel.TemplateValues.Count)
                {
                    text.Text = OpenVisionLearnMatchingSimulationModel.TemplateValues[index] > 0
                        ? matchingPresenter.TemplateMark
                        : "0";
                }

                index++;
            }
        }

        private void ResetMatchingAnimation()
        {
            matchingAnimationTimer.Stop();
            btnMatchingPlay.Content = "Play";
            matchingPresenter.ResetAnimation();
            PaintMatchingAnimationFrame();
        }

        private void AdvanceMatchingAnimation()
        {
            matchingPresenter.AdvanceAnimation();
            PaintMatchingAnimationFrame();
            if (matchingPresenter.IsAnimationComplete)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
            }
        }

        private void UpdateFeatureMatchingGuide()
        {
            featureMatchingPresenter.Update(featureGoodMatchMinSlider.Value);
            txtFeatureGoodMatchMin.Text = featureMatchingPresenter.RequiredText;
            txtFeatureMatchingFormula.Text = featureMatchingPresenter.FormulaText;
            txtFeatureMatchingMeaning.Text = featureMatchingPresenter.MeaningText;
            RenderFeatureMatchingAnimationFrame();
        }

        private void PaintFeatureMatchingAnimationFrame()
        {
            featureMatchingPresenter.Update(featureGoodMatchMinSlider.Value);
            RenderFeatureMatchingAnimationFrame();
        }

        private void RenderFeatureMatchingAnimationFrame()
        {
            bool[] goodMatches = featureMatchingPresenter.Evaluation.GoodMatches;
            int visibleStep = featureMatchingPresenter.AnimationStep;
            bool[] detectedPoints = OpenVisionLearnMatchingSimulationModel.FeatureScores
                .Select(_ => true)
                .ToArray();
            IReadOnlyList<bool> ransacInliers =
                OpenVisionLearnMatchingSimulationModel.FeatureRansacInliers;

            if (visibleStep == 0)
            {
                PaintFeaturePointGrid(
                    featureReferenceCells,
                    featureReferenceTexts,
                    OpenVisionLearnMatchingSimulationModel.FeatureReferencePoints,
                    Array.Empty<bool>(),
                    "K");
                PaintFeaturePointGrid(
                    featureSceneCells,
                    featureSceneTexts,
                    OpenVisionLearnMatchingSimulationModel.FeatureScenePoints,
                    Array.Empty<bool>(),
                    "M");
                for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.FeatureScores.Count; i++)
                {
                    PaintFeatureScoreCell(i, animationNeutralBrush, Brushes.White, "-");
                }

                txtFeatureMatchingAnimationStatus.Text = featureMatchingPresenter.AnimationStatusText;
                return;
            }

            PaintFeaturePointGrid(
                featureReferenceCells,
                featureReferenceTexts,
                OpenVisionLearnMatchingSimulationModel.FeatureReferencePoints,
                visibleStep == 1 ? detectedPoints : visibleStep == 2 ? goodMatches : ransacInliers,
                "K");
            PaintFeaturePointGrid(
                featureSceneCells,
                featureSceneTexts,
                OpenVisionLearnMatchingSimulationModel.FeatureScenePoints,
                visibleStep == 1 ? detectedPoints : visibleStep == 2 ? goodMatches : ransacInliers,
                "M");

            for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.FeatureScores.Count; i++)
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
                PaintFeatureScoreCell(
                    i,
                    background,
                    Brushes.White,
                    OpenVisionLearnMatchingSimulationModel.FeatureScores[i]
                        .ToString("0.00", CultureInfo.InvariantCulture));
            }

            txtFeatureMatchingAnimationStatus.Text = featureMatchingPresenter.AnimationStatusText;
        }

        private void ResetFeatureMatchingAnimation()
        {
            featureMatchingAnimationTimer.Stop();
            btnFeatureMatchingPlay.Content = "Play";
            featureMatchingPresenter.ResetAnimation();
            PaintFeatureMatchingAnimationFrame();
        }

        private void AdvanceFeatureMatchingAnimation()
        {
            featureMatchingPresenter.AdvanceAnimation();
            PaintFeatureMatchingAnimationFrame();
            if (featureMatchingPresenter.IsAnimationComplete)
            {
                featureMatchingAnimationTimer.Stop();
                btnFeatureMatchingPlay.Content = "Play";
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

        private void PaintMatchingSearchGrid(int bestIndex)
        {
            (int X, int Y) best = bestIndex >= 0
                ? OpenVisionLearnMatchingSimulationModel.CandidatePositions[bestIndex]
                : (-1, -1);
            for (int i = 0; i < OpenVisionLearnMatchingSimulationModel.SearchValues.Count; i++)
            {
                int x = i % 5;
                int y = i / 5;
                bool inBest = bestIndex >= 0 && x >= best.X && x < best.X + 2 && y >= best.Y && y < best.Y + 2;
                int value = OpenVisionLearnMatchingSimulationModel.SearchValues[i];

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
                        ? matchingPresenter.SearchMark
                        : "0";
                }
            }
        }

        private void PaintMatchingCandidateScanGrid()
        {
            PaintMatchingSearchGrid(-1);
            Brush candidateBrush = animationCandidateBrush;
            foreach ((int X, int Y) candidate in OpenVisionLearnMatchingSimulationModel.CandidatePositions)
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

        private void MatchingAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMatchingAnimation();
        }

        private void FeatureMatchingAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFeatureMatchingAnimation();
        }

        private void MatchingPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchingAnimationTimer.IsEnabled)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
                return;
            }

            if (matchingPresenter.IsAnimationComplete)
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

            if (featureMatchingPresenter.IsAnimationComplete)
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

        private void MatchingThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                matchingAnimationTimer.Stop();
                btnMatchingPlay.Content = "Play";
                matchingPresenter.ShowResult();
                UpdateMatchingGuide();
            }
        }

        private void FeatureGoodMatchMinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                featureMatchingAnimationTimer.Stop();
                btnFeatureMatchingPlay.Content = "Play";
                featureMatchingPresenter.ShowResult();
                UpdateFeatureMatchingGuide();
            }
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

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                openRelatedToolAction?.Invoke(menu);
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
            }
        }
    }
}
