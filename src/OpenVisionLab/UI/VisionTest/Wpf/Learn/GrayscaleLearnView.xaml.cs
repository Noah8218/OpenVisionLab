using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    /// <summary>Owns grayscale-topic controls and timers; GrayscaleLearnPresenter owns lesson state and presentation decisions.</summary>
    public sealed partial class GrayscaleLearnView : UserControl
    {
        private readonly GrayscaleLearnPresenter presenter = new();
        private readonly DispatcherTimer animationTimer;
        private readonly DispatcherTimer arithmeticAnimationTimer;
        private readonly DispatcherTimer brightnessAnimationTimer;
        private readonly DispatcherTimer filterAnimationTimer;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
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
        private Action<VISION_MENU> openRelatedToolAction;

        public GrayscaleLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
            arithmeticAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            brightnessAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            filterAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            BuildSampleCells();
            BuildBrightnessCells();
            BuildArithmeticCells();
            BuildFilterCells();
            InitializeThreshold(127, 255, false);
            brightnessOffsetSlider.Value = 35;
            UpdateGuide();
            UpdateBrightnessGuide();
            UpdateArithmeticGuide();
            UpdateFilterGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal event EventHandler<OpenVisionLearnThresholdApplyEventArgs> ApplyThresholdRequested;
        internal event EventHandler CloseRequested;
        internal event EventHandler ThresholdToolOpened;

        internal void InitializeThreshold(double threshold, double maxValue, bool invert)
        {
            presenter.ConfigureMaximumValue(maxValue);
            thresholdSlider.Value = GrayscaleLearnPresenter.NormalizeThresholdValue(threshold);
            chkInvert.IsChecked = invert;
            txtMaxValue.Text = presenter.MaximumValueText;
        }

        internal void SelectTopic(int topicIndex)
        {
            Visibility = topicIndex is 1 or 2 or 3 or 14 ? Visibility.Visible : Visibility.Collapsed;
            brightnessTopicPanel.Visibility = topicIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            thresholdTabs.Visibility = topicIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
            thresholdControls.Visibility = topicIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
            filterTopicPanel.Visibility = topicIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
            arithmeticTopicPanel.Visibility = topicIndex == 14 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            btnThresholdOpenTool.IsEnabled = action != null;
            btnBrightnessOpenMeanTool.IsEnabled = action != null;
            btnBrightnessOpenHistogramTool.IsEnabled = action != null;
            btnArithmeticOpenTool.IsEnabled = action != null;
            btnFilteringOpenTool.IsEnabled = action != null;
        }

        internal void StopAnimations()
        {
            animationTimer.Stop();
            arithmeticAnimationTimer.Stop();
            brightnessAnimationTimer.Stop();
            filterAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            animationTimer.Tick += AnimationTimer_Tick;
            arithmeticAnimationTimer.Tick += ArithmeticAnimationTimer_Tick;
            brightnessAnimationTimer.Tick += BrightnessAnimationTimer_Tick;
            filterAnimationTimer.Tick += FilterAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Retire callbacks on removal; rehosting retains settings and stages without starting playback.
            StopAnimations();
            animationTimer.Tick -= AnimationTimer_Tick;
            arithmeticAnimationTimer.Tick -= ArithmeticAnimationTimer_Tick;
            brightnessAnimationTimer.Tick -= BrightnessAnimationTimer_Tick;
            filterAnimationTimer.Tick -= FilterAnimationTimer_Tick;
            btnAnimate.Content = "Play";
            btnArithmeticPlay.Content = "Play";
            btnBrightnessPlay.Content = "Play";
            btnFilterPlay.Content = "Play";
        }

        internal double ThresholdValueForTest
        {
            get => thresholdSlider.Value;
            set => thresholdSlider.Value = GrayscaleLearnPresenter.NormalizeThresholdValue(value);
        }

        internal bool IsInvertedForTest
        {
            get => chkInvert.IsChecked == true;
            set => chkInvert.IsChecked = value;
        }

        internal string FormulaTextForTest => txtFormula.Text ?? string.Empty;
        internal bool CanOpenThresholdToolForTest => btnThresholdOpenTool.IsEnabled;
        internal bool CanOpenBrightnessToolsForTest => btnBrightnessOpenMeanTool.IsEnabled && btnBrightnessOpenHistogramTool.IsEnabled;
        internal bool CanOpenArithmeticToolForTest => btnArithmeticOpenTool.IsEnabled;
        internal bool CanOpenFilteringToolForTest => btnFilteringOpenTool.IsEnabled;
        internal string BrightnessToolLocationTitleForTest => txtBrightnessToolLocationTitle.Text ?? string.Empty;
        internal string BrightnessToolLocationDetailForTest => txtBrightnessToolLocationDetail.Text ?? string.Empty;
        internal string ArithmeticToolLocationTitleForTest => txtArithmeticToolLocationTitle.Text ?? string.Empty;
        internal string ArithmeticToolLocationDetailForTest => txtArithmeticToolLocationDetail.Text ?? string.Empty;
        internal string FilteringToolLocationTitleForTest => txtFilteringToolLocationTitle.Text ?? string.Empty;
        internal string FilteringToolLocationDetailForTest => txtFilteringToolLocationDetail.Text ?? string.Empty;

        internal double BrightnessOffsetForTest
        {
            get => brightnessOffsetSlider.Value;
            set => brightnessOffsetSlider.Value = Math.Max(-80, Math.Min(80, value));
        }

        internal string BrightnessFormulaTextForTest => txtBrightnessFormula.Text ?? string.Empty;

        internal int BrightnessAnimationStepForTest => presenter.BrightnessAnimationStep;

        internal string BrightnessAnimationStatusTextForTest => txtBrightnessAnimationStatus.Text ?? string.Empty;

        internal void ResetBrightnessAnimationForTest()
        {
            ResetBrightnessAnimation();
        }

        internal void AdvanceBrightnessAnimationForTest()
        {
            AdvanceBrightnessAnimation();
        }

        internal void ToggleBrightnessAnimationForTest()
        {
            BrightnessPlayButton_Click(this, new RoutedEventArgs());
        }

        internal int ArithmeticModeIndexForTest
        {
            get => arithmeticModeCombo.SelectedIndex;
            set => arithmeticModeCombo.SelectedIndex = Math.Max(0, Math.Min(4, value));
        }

        internal string ArithmeticFormulaTextForTest => txtArithmeticFormula.Text ?? string.Empty;

        internal int ArithmeticAnimationStepForTest => presenter.ArithmeticAnimationStep;

        internal string ArithmeticAnimationStatusTextForTest => txtArithmeticAnimationStatus.Text ?? string.Empty;

        internal void ResetArithmeticAnimationForTest()
        {
            ResetArithmeticAnimation();
        }

        internal void AdvanceArithmeticAnimationForTest()
        {
            AdvanceArithmeticAnimation();
        }

        internal void ToggleArithmeticAnimationForTest()
        {
            ArithmeticPlayButton_Click(this, new RoutedEventArgs());
        }

        internal int FilterModeIndexForTest
        {
            get => filterModeCombo.SelectedIndex;
            set => filterModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        internal string FilterFormulaTextForTest => txtFilterFormula.Text ?? string.Empty;

        internal int FilterAnimationStepForTest => presenter.FilterAnimationStep;

        internal string FilterAnimationStatusTextForTest => txtFilterAnimationStatus.Text ?? string.Empty;

        internal void ResetFilterAnimationForTest()
        {
            ResetFilterAnimation();
        }

        internal void AdvanceFilterAnimationForTest()
        {
            AdvanceFilterAnimation();
        }

        internal void ToggleFilterAnimationForTest()
        {
            FilterPlayButton_Click(this, new RoutedEventArgs());
        }

        private void BuildSampleCells()
        {
            sampleGrid.Children.Clear();
            resultGrid.Children.Clear();
            resultCells.Clear();
            resultTexts.Clear();

            foreach (int value in presenter.ThresholdSamples)
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

            foreach (int value in presenter.BrightnessSamples)
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

        private void BuildArithmeticCells()
        {
            arithmeticInputAGrid.Children.Clear();
            arithmeticInputBGrid.Children.Clear();
            arithmeticResultGrid.Children.Clear();
            arithmeticInputACells.Clear();
            arithmeticInputBCells.Clear();
            arithmeticResultCells.Clear();
            arithmeticResultTexts.Clear();

            for (int i = 0; i < presenter.ArithmeticInputA.Count; i++)
            {
                int inputA = presenter.ArithmeticInputA[i];
                int inputB = presenter.ArithmeticInputB[i];
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

        private void BuildFilterCells()
        {
            filterInputGrid.Children.Clear();
            filterOutputGrid.Children.Clear();
            filterInputCells.Clear();
            filterOutputCells.Clear();
            filterOutputTexts.Clear();

            foreach (int value in presenter.FilterSamples)
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

        internal void UpdateGuide()
        {
            presenter.UpdateThreshold(thresholdSlider.Value, chkInvert.IsChecked == true);
            txtThresholdValue.Text = presenter.ThresholdValueText;
            txtFormula.Text = presenter.ThresholdFormulaText;
            for (int i = 0; i < resultCells.Count; i++)
            {
                PaintCell(resultCells[i], resultTexts[i], presenter.GetThresholdOutputCell(i));
            }

            UpdateThresholdMarker();
        }

        internal void UpdateBrightnessGuide()
        {
            presenter.UpdateBrightness(brightnessOffsetSlider.Value);
            txtBrightnessOffset.Text = presenter.BrightnessOffsetText;
            txtBrightnessFormula.Text = presenter.BrightnessFormulaText;
            for (int i = 0; i < brightnessOutputCells.Count; i++)
            {
                PaintBorder(brightnessInputCells[i], presenter.BrightnessInputBorderRole);
                PaintCell(brightnessOutputCells[i], brightnessOutputTexts[i], presenter.GetBrightnessOutputCell(i));
            }

            int maxCount = Math.Max(1, presenter.BrightnessHistogramBins.Max());
            for (int i = 0; i < histogramBars.Count; i++)
            {
                histogramBars[i].Height = presenter.ShowBrightnessHistogram
                    ? presenter.BrightnessHistogramBins[i] == 0 ? 4D : 120D * presenter.BrightnessHistogramBins[i] / maxCount
                    : 4D;
                histogramBars[i].Background = presenter.ShowBrightnessHistogram ? animationPassBrush : animationNeutralBrush;
                histogramLabels[i].Text = presenter.GetBrightnessHistogramLabel(i);
            }

            txtBrightnessAnimationStatus.Text = presenter.BrightnessAnimationStatusText;
        }

        internal void UpdateArithmeticGuide()
        {
            presenter.UpdateArithmetic(GetSelectedArithmeticMode());
            txtArithmeticFormula.Text = presenter.ArithmeticFormulaText;
            txtArithmeticMeaning.Text = presenter.ArithmeticMeaningText;
            for (int i = 0; i < arithmeticResultCells.Count; i++)
            {
                PaintBorder(arithmeticInputACells[i], presenter.ArithmeticInputBorderRole);
                PaintBorder(arithmeticInputBCells[i], presenter.ArithmeticInputBorderRole);
                PaintCell(arithmeticResultCells[i], arithmeticResultTexts[i], presenter.GetArithmeticOutputCell(i));
            }

            txtArithmeticAnimationStatus.Text = presenter.ArithmeticAnimationStatusText;
        }

        internal void UpdateFilterGuide()
        {
            presenter.UpdateFilter(GetSelectedFilterMode());
            txtFilterFormula.Text = presenter.FilterFormulaText;
            txtFilterMeaning.Text = presenter.FilterMeaningText;
            for (int i = 0; i < filterOutputCells.Count; i++)
            {
                PaintBorder(filterInputCells[i], presenter.GetFilterInputBorderRole(i));
                PaintCell(filterOutputCells[i], filterOutputTexts[i], presenter.GetFilterOutputCell(i));
            }

            txtFilterAnimationStatus.Text = presenter.FilterAnimationStatusText;
        }

        private void PaintCell(Border border, TextBlock text, GrayscaleLearnCell cell)
        {
            PaintBorder(border, cell.BorderRole);
            border.Background = cell.BackgroundRole == GrayscaleLearnCellRole.Gray ? CreateGrayBrush(cell.Gray) : ResolveBrush(cell.BackgroundRole);
            text.Foreground = cell.BackgroundRole == GrayscaleLearnCellRole.Gray && cell.Gray > 128 ? Brushes.Black : Brushes.White;
            text.Text = cell.Text;
        }

        private void PaintBorder(Border border, GrayscaleLearnCellRole role)
        {
            border.BorderBrush = ResolveBrush(role);
            border.BorderThickness = role == GrayscaleLearnCellRole.Default ? new Thickness(1) : new Thickness(2);
        }

        private Brush ResolveBrush(GrayscaleLearnCellRole role)
        {
            return role switch
            {
                GrayscaleLearnCellRole.Neutral => animationNeutralBrush,
                GrayscaleLearnCellRole.Candidate => animationCandidateBrush,
                GrayscaleLearnCellRole.Pass => animationPassBrush,
                GrayscaleLearnCellRole.Warning => animationWarningBrush,
                _ => new SolidColorBrush(Color.FromRgb(209, 213, 219))
            };
        }

        private void ResetBrightnessAnimation()
        {
            brightnessAnimationTimer.Stop();
            btnBrightnessPlay.Content = "Play";
            presenter.ResetBrightnessAnimation();
            UpdateBrightnessGuide();
        }

        private void AdvanceBrightnessAnimation()
        {
            presenter.AdvanceBrightnessAnimation();
            UpdateBrightnessGuide();
            if (presenter.IsBrightnessAnimationComplete)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
            }
        }

        private void ResetArithmeticAnimation()
        {
            arithmeticAnimationTimer.Stop();
            btnArithmeticPlay.Content = "Play";
            presenter.ResetArithmeticAnimation();
            UpdateArithmeticGuide();
        }

        private void AdvanceArithmeticAnimation()
        {
            presenter.AdvanceArithmeticAnimation();
            UpdateArithmeticGuide();
            if (presenter.IsArithmeticAnimationComplete)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
            }
        }

        private void ResetFilterAnimation()
        {
            filterAnimationTimer.Stop();
            btnFilterPlay.Content = "Play";
            presenter.ResetFilterAnimation();
            UpdateFilterGuide();
        }

        private void AdvanceFilterAnimation()
        {
            presenter.AdvanceFilterAnimation();
            UpdateFilterGuide();
            if (presenter.IsFilterAnimationComplete)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
            }
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

        private void BrightnessAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceBrightnessAnimation();
        }

        private void ArithmeticAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceArithmeticAnimation();
        }

        private void FilterAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFilterAnimation();
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

        private string GetSelectedFilterMode()
        {
            return (filterModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Mean blur";
        }

        private string GetSelectedArithmeticMode()
        {
            return (arithmeticModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "AbsDiff";
        }

        private void BrightnessPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (brightnessAnimationTimer.IsEnabled)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
                return;
            }

            if (presenter.IsBrightnessAnimationComplete)
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

        private void ArithmeticPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (arithmeticAnimationTimer.IsEnabled)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
                return;
            }

            if (presenter.IsArithmeticAnimationComplete)
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

        private void FilterPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (filterAnimationTimer.IsEnabled)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
                return;
            }

            if (presenter.IsFilterAnimationComplete)
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

        private void BrightnessOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                brightnessAnimationTimer.Stop();
                btnBrightnessPlay.Content = "Play";
                presenter.CompleteBrightnessAnimation();
                UpdateBrightnessGuide();
            }
        }

        private void ArithmeticModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                arithmeticAnimationTimer.Stop();
                btnArithmeticPlay.Content = "Play";
                presenter.CompleteArithmeticAnimation();
                UpdateArithmeticGuide();
            }
        }

        private void FilterModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                filterAnimationTimer.Stop();
                btnFilterPlay.Content = "Play";
                presenter.CompleteFilterAnimation();
                UpdateFilterGuide();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            thresholdSlider.Value = presenter.NextThresholdAnimationValue(thresholdSlider.Value);
        }

        internal void ApplyForTest() => ApplyButton_Click(this, new RoutedEventArgs());

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyThresholdRequested?.Invoke(this, new OpenVisionLearnThresholdApplyEventArgs(
                GrayscaleLearnPresenter.NormalizeThresholdValue(thresholdSlider.Value), chkInvert.IsChecked == true));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                // The host may open a modal Tool; update the location hint only after that callback succeeds.
                openRelatedToolAction?.Invoke(menu);
                if (menu == VISION_MENU.Threshold)
                {
                    ThresholdToolOpened?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (menu == VISION_MENU.Filter && ReferenceEquals(button, btnFilteringOpenTool))
                {
                    txtFilteringToolLocationTitle.Text = GrayscaleLearnPresenter.FilteringOpenedTitle;
                    txtFilteringToolLocationDetail.Text = GrayscaleLearnPresenter.FilteringOpenedDetail;
                    return;
                }

                if (menu == VISION_MENU.Arithmetic)
                {
                    txtArithmeticToolLocationTitle.Text = GrayscaleLearnPresenter.ArithmeticOpenedTitle;
                    txtArithmeticToolLocationDetail.Text = GrayscaleLearnPresenter.ArithmeticOpenedDetail;
                    return;
                }

                if (menu == VISION_MENU.Mean || menu == VISION_MENU.Histogram)
                {
                    txtBrightnessToolLocationTitle.Text = menu == VISION_MENU.Mean
                        ? GrayscaleLearnPresenter.MeanOpenedTitle
                        : GrayscaleLearnPresenter.HistogramOpenedTitle;
                    txtBrightnessToolLocationDetail.Text = GrayscaleLearnPresenter.BrightnessOpenedDetail;
                }
            }
        }
    }
}
