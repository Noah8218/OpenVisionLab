using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    /// <summary>Hosts Edge/Line and Line Distance controls; the presenter owns lesson state and cell meaning.</summary>
    public sealed partial class LineLearnView : UserControl
    {
        private readonly LineLearnPresenter presenter = new();
        private readonly DispatcherTimer edgeLineAnimationTimer;
        private readonly DispatcherTimer lineDistanceAnimationTimer;
        private readonly List<Border> edgeLineInputCells = new();
        private readonly List<Border> edgeLineOutputCells = new();
        private readonly List<TextBlock> edgeLineOutputTexts = new();
        private readonly List<Border> lineDistanceInputCells = new();
        private readonly List<Border> lineDistanceOutputCells = new();
        private readonly List<TextBlock> lineDistanceOutputTexts = new();
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private Action<VISION_MENU> openRelatedToolAction;

        public LineLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            edgeLineAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            lineDistanceAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            BuildEdgeLineCells();
            BuildLineDistanceCells();
            edgeThresholdSlider.Value = 80;
            lineDistanceRangeMaxSlider.Value = 0.5;
            UpdateEdgeLineGuide();
            UpdateLineDistanceGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal void SelectTopic(int topicIndex)
        {
            Visibility = topicIndex == 7 || topicIndex == 8 ? Visibility.Visible : Visibility.Collapsed;
            edgeLineTopicPanel.Visibility = topicIndex == 7 ? Visibility.Visible : Visibility.Collapsed;
            lineDistanceTopicPanel.Visibility = topicIndex == 8 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            bool enabled = action != null;
            btnEdgeDetectionOpenTool.IsEnabled = enabled;
            btnEdgeLineOpenLineTool.IsEnabled = enabled;
            btnLineDistanceOpenTool.IsEnabled = enabled;
        }

        internal void StopAnimations()
        {
            edgeLineAnimationTimer.Stop();
            lineDistanceAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            edgeLineAnimationTimer.Tick += EdgeLineAnimationTimer_Tick;
            lineDistanceAnimationTimer.Tick += LineDistanceAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Detaching retires both timers; a later host keeps the lesson state without autoplay.
            StopAnimations();
            edgeLineAnimationTimer.Tick -= EdgeLineAnimationTimer_Tick;
            lineDistanceAnimationTimer.Tick -= LineDistanceAnimationTimer_Tick;
            btnEdgeLinePlay.Content = "Play";
            btnLineDistancePlay.Content = "Play";
        }

        internal bool CanOpenEdgeLineToolsForTest =>
            btnEdgeDetectionOpenTool.IsEnabled && btnEdgeLineOpenLineTool.IsEnabled;

        internal bool CanOpenLineDistanceToolForTest => btnLineDistanceOpenTool.IsEnabled;

        internal string EdgeLineToolLocationTitleForTest => txtEdgeLineToolLocationTitle.Text ?? string.Empty;

        internal string EdgeLineToolLocationDetailForTest => txtEdgeLineToolLocationDetail.Text ?? string.Empty;

        internal string LineDistanceToolLocationTitleForTest => txtLineDistanceToolLocationTitle.Text ?? string.Empty;

        internal string LineDistanceToolLocationDetailForTest => txtLineDistanceToolLocationDetail.Text ?? string.Empty;

        internal double EdgeThresholdForTest
        {
            get => edgeThresholdSlider.Value;
            set => edgeThresholdSlider.Value = Math.Max(10, Math.Min(150, value));
        }

        internal string EdgeLineFormulaTextForTest => txtEdgeLineFormula.Text ?? string.Empty;

        internal int EdgeLineAnimationStepForTest => presenter.EdgeLineAnimationStep;

        internal string EdgeLineAnimationStatusTextForTest => txtEdgeLineAnimationStatus.Text ?? string.Empty;

        internal void ResetEdgeLineAnimationForTest()
        {
            ResetEdgeLineAnimation();
        }

        internal void AdvanceEdgeLineAnimationForTest()
        {
            AdvanceEdgeLineAnimation();
        }

        internal void ToggleEdgeLineAnimationForTest()
        {
            EdgeLinePlayButton_Click(this, new RoutedEventArgs());
        }

        internal double LineDistanceRangeMaxForTest
        {
            get => lineDistanceRangeMaxSlider.Value;
            set => lineDistanceRangeMaxSlider.Value = Math.Max(0, Math.Min(2, value));
        }

        internal string LineDistanceFormulaTextForTest => txtLineDistanceFormula.Text ?? string.Empty;

        internal int LineDistanceAnimationStepForTest => presenter.LineDistanceAnimationStep;

        internal string LineDistanceAnimationStatusTextForTest => txtLineDistanceAnimationStatus.Text ?? string.Empty;

        internal void ResetLineDistanceAnimationForTest()
        {
            ResetLineDistanceAnimation();
        }

        internal void AdvanceLineDistanceAnimationForTest()
        {
            AdvanceLineDistanceAnimation();
        }

        internal void ToggleLineDistanceAnimationForTest()
        {
            LineDistancePlayButton_Click(this, new RoutedEventArgs());
        }

        private void BuildEdgeLineCells()
        {
            edgeLineInputGrid.Children.Clear();
            edgeLineOutputGrid.Children.Clear();
            edgeLineInputCells.Clear();
            edgeLineOutputCells.Clear();
            edgeLineOutputTexts.Clear();

            foreach (int value in presenter.EdgeSampleValues)
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

        private void HighlightLineDistanceInputRow(int row, Brush borderBrush)
        {
            int start = row * 9;
            for (int i = start; i < start + 9 && i < lineDistanceInputCells.Count; i++)
            {
                lineDistanceInputCells[i].BorderBrush = borderBrush;
                lineDistanceInputCells[i].BorderThickness = new Thickness(2);
            }
        }

        private void EdgeLineAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceEdgeLineAnimation();
        }

        private void LineDistanceAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceLineDistanceAnimation();
        }

        private void EdgeLinePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (edgeLineAnimationTimer.IsEnabled)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
                return;
            }

            if (presenter.IsEdgeLineAnimationComplete)
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

            if (presenter.IsLineDistanceAnimationComplete)
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

        private void EdgeThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
                presenter.CompleteEdgeLineAnimation();
                UpdateEdgeLineGuide();
            }
        }

        private void LineDistanceRangeMaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                lineDistanceAnimationTimer.Stop();
                btnLineDistancePlay.Content = "Play";
                presenter.CompleteLineDistanceAnimation();
                UpdateLineDistanceGuide();
            }
        }

        internal void UpdateEdgeLineGuide()
        {
            presenter.UpdateEdgeThreshold(edgeThresholdSlider.Value);
            txtEdgeThreshold.Text = presenter.EdgeThresholdText;
            txtEdgeLineFormula.Text = presenter.EdgeLineFormulaText;
            txtEdgeLineMeaning.Text = presenter.EdgeLineMeaningText;
            PaintEdgeLineAnimationFrame();
        }

        internal void UpdateLineDistanceGuide()
        {
            presenter.UpdateLineDistanceRangeMaximum(lineDistanceRangeMaxSlider.Value);
            txtLineDistanceRangeMax.Text = presenter.LineDistanceRangeMaximumText;
            txtLineDistanceFormula.Text = presenter.LineDistanceFormulaText;
            txtLineDistanceMeaning.Text = presenter.LineDistanceMeaningText;
            PaintLineDistanceAnimationFrame();
        }

        private void PaintEdgeLineAnimationFrame()
        {
            for (int i = 0; i < edgeLineInputCells.Count; i++)
            {
                SetInputHighlight(edgeLineInputCells[i], presenter.GetEdgeInputHighlight(i));
                PaintCell(edgeLineOutputCells[i], edgeLineOutputTexts[i], presenter.GetEdgeOutputCell(i));
            }
            txtEdgeLineAnimationStatus.Text = presenter.EdgeLineAnimationStatusText;
        }

        private void BuildLineDistanceCells()
        {
            lineDistanceInputGrid.Children.Clear();
            lineDistanceOutputGrid.Children.Clear();
            lineDistanceInputCells.Clear();
            lineDistanceOutputCells.Clear();
            lineDistanceOutputTexts.Clear();
            for (int y = 0; y < presenter.DistanceSampleCount; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Border cell = CreateSmallValueCell(string.Empty, 0);
                    PaintCell(cell, (TextBlock)cell.Child, presenter.GetDistanceInputCell(y, x));
                    lineDistanceInputGrid.Children.Add(cell);
                    lineDistanceInputCells.Add(cell);
                }
            }
            for (int i = 0; i < presenter.DistanceSampleCount; i++)
            {
                Border outputCell = CreateSmallValueCell(string.Empty, 0);
                TextBlock outputText = (TextBlock)outputCell.Child;
                lineDistanceOutputGrid.Children.Add(outputCell);
                lineDistanceOutputCells.Add(outputCell);
                lineDistanceOutputTexts.Add(outputText);
            }
        }

        private void PaintLineDistanceAnimationFrame()
        {
            foreach (Border cell in lineDistanceInputCells)
                SetInputHighlight(cell, null);
            for (int i = 0; i < presenter.DistanceSampleCount; i++)
            {
                LineLearnCellRole? highlight = presenter.GetDistanceInputHighlight(i);
                if (highlight.HasValue)
                    HighlightLineDistanceInputRow(i, GetBrush(highlight.Value));
                PaintCell(lineDistanceOutputCells[i], lineDistanceOutputTexts[i], presenter.GetDistanceOutputCell(i));
            }
            txtLineDistanceAnimationStatus.Text = presenter.LineDistanceAnimationStatusText;
        }

        private void SetInputHighlight(Border cell, LineLearnCellRole? role)
        {
            cell.BorderBrush = role.HasValue ? GetBrush(role.Value) : new SolidColorBrush(Color.FromRgb(209, 213, 219));
            cell.BorderThickness = new Thickness(role.HasValue ? 2 : 1);
        }

        private void PaintCell(Border cell, TextBlock text, LineLearnCell presentation)
        {
            cell.Background = presentation.Role == LineLearnCellRole.Gray ? CreateGrayBrush(presentation.Gray) : GetBrush(presentation.Role);
            text.Foreground = presentation.Role == LineLearnCellRole.Gap
                || presentation.Role == LineLearnCellRole.Gray && presentation.Gray > 128 ? Brushes.Black : Brushes.White;
            text.Text = presentation.Text;
        }

        private Brush GetBrush(LineLearnCellRole role) => role switch
        {
            LineLearnCellRole.Neutral => animationNeutralBrush,
            LineLearnCellRole.Candidate => animationCandidateBrush,
            LineLearnCellRole.Pass => animationPassBrush,
            LineLearnCellRole.Gap => new SolidColorBrush(Color.FromRgb(229, 244, 247)),
            LineLearnCellRole.Outlier => new SolidColorBrush(Color.FromRgb(185, 91, 36)),
            _ => CreateGrayBrush(0)
        };

        private void ResetEdgeLineAnimation()
        {
            edgeLineAnimationTimer.Stop();
            btnEdgeLinePlay.Content = "Play";
            presenter.ResetEdgeLineAnimation();
            // Read the control value even when a detached test changes it before Loaded.
            presenter.UpdateEdgeThreshold(edgeThresholdSlider.Value);
            PaintEdgeLineAnimationFrame();
        }

        private void AdvanceEdgeLineAnimation()
        {
            presenter.AdvanceEdgeLineAnimation();
            presenter.UpdateEdgeThreshold(edgeThresholdSlider.Value);
            PaintEdgeLineAnimationFrame();
            if (presenter.IsEdgeLineAnimationComplete)
            {
                edgeLineAnimationTimer.Stop();
                btnEdgeLinePlay.Content = "Play";
            }
        }

        private void ResetLineDistanceAnimation()
        {
            lineDistanceAnimationTimer.Stop();
            btnLineDistancePlay.Content = "Play";
            presenter.ResetLineDistanceAnimation();
            presenter.UpdateLineDistanceRangeMaximum(lineDistanceRangeMaxSlider.Value);
            PaintLineDistanceAnimationFrame();
        }

        private void AdvanceLineDistanceAnimation()
        {
            presenter.AdvanceLineDistanceAnimation();
            presenter.UpdateLineDistanceRangeMaximum(lineDistanceRangeMaxSlider.Value);
            PaintLineDistanceAnimationFrame();
            if (presenter.IsLineDistanceAnimationComplete)
            {
                lineDistanceAnimationTimer.Stop();
                btnLineDistancePlay.Content = "Play";
            }
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                // Preserve callback-first ordering and distinguish the two buttons that both open Line.
                openRelatedToolAction?.Invoke(menu);
                if (menu == VISION_MENU.EdgeDetection && ReferenceEquals(button, btnEdgeDetectionOpenTool))
                {
                    txtEdgeLineToolLocationTitle.Text = LineLearnPresenter.EdgeDetectionToolTitle;
                    txtEdgeLineToolLocationDetail.Text = LineLearnPresenter.EdgeDetectionToolDetail;
                    return;
                }
                if (menu == VISION_MENU.Line && ReferenceEquals(button, btnEdgeLineOpenLineTool))
                {
                    txtEdgeLineToolLocationTitle.Text = LineLearnPresenter.EdgeLineToolTitle;
                    txtEdgeLineToolLocationDetail.Text = LineLearnPresenter.EdgeLineToolDetail;
                    return;
                }
                if (menu == VISION_MENU.Line && ReferenceEquals(button, btnLineDistanceOpenTool))
                {
                    txtLineDistanceToolLocationTitle.Text = LineLearnPresenter.LineDistanceToolTitle;
                    txtLineDistanceToolLocationDetail.Text = LineLearnPresenter.LineDistanceToolDetail;
                }
            }
        }
    }
}
