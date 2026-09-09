using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    /// <summary>Owns the acceptance lesson controls and timer; the presenter owns sample judgments and explanations.</summary>
    public sealed partial class MetricsAcceptanceLearnView : UserControl
    {
        private readonly MetricsAcceptanceLearnPresenter presenter = new();
        private readonly DispatcherTimer animationTimer;
        private readonly List<Border> sampleCells = new();
        private readonly List<TextBlock> sampleTexts = new();
        private readonly Brush neutralBrush;
        private readonly Brush passBrush;
        private readonly Brush warningBrush;

        public MetricsAcceptanceLearnView()
        {
            InitializeComponent();
            neutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            passBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            warningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            for (int index = 0; index < presenter.Samples.Count; index++)
            {
                Border cell = CreateSmallValueCell(presenter.GetSampleText(index), 230);
                metricsAcceptanceSampleGrid.Children.Add(cell);
                sampleCells.Add(cell);
                sampleTexts.Add((TextBlock)cell.Child);
            }
            RefreshFrame();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal int AnimationStepForTest => presenter.AnimationStep;
        internal string FormulaTextForTest => txtMetricsAcceptanceFormula.Text ?? string.Empty;
        internal string AnimationStatusTextForTest => txtMetricsAcceptanceAnimationStatus.Text ?? string.Empty;
        internal bool IsCheatSheetExpandedForTest => metricGateCheatSheetExpander.IsExpanded;

        internal void RefreshFrame()
        {
            for (int index = 0; index < sampleCells.Count; index++)
            {
                sampleCells[index].Background = !presenter.ShowsSampleDecision
                    ? neutralBrush
                    : presenter.IsOutlier(index) ? warningBrush : passBrush;
                sampleTexts[index].Foreground = Brushes.White;
                sampleTexts[index].Text = presenter.GetSampleText(index);
            }
            txtMetricsAcceptanceFormula.Text = presenter.FormulaText;
            txtMetricsAcceptanceAnimationStatus.Text = presenter.AnimationStatusText;
        }

        internal void StopAnimation()
        {
            animationTimer.Stop();
        }

        internal void ResetAnimation()
        {
            StopAnimation();
            btnMetricsAcceptancePlay.Content = "Play";
            presenter.ResetAnimation();
            RefreshFrame();
        }

        internal void AdvanceAnimation()
        {
            presenter.AdvanceAnimation();
            RefreshFrame();
            if (presenter.IsAnimationComplete)
            {
                StopAnimation();
                btnMetricsAcceptancePlay.Content = "Play";
            }
        }

        internal void ToggleAnimation()
        {
            if (animationTimer.IsEnabled)
            {
                StopAnimation();
                btnMetricsAcceptancePlay.Content = "Play";
                return;
            }
            if (presenter.IsAnimationComplete)
            {
                ResetAnimation();
            }
            btnMetricsAcceptancePlay.Content = "Pause";
            animationTimer.Start();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Rehosting retains the lesson stage but must not retain playback or duplicate timer callbacks.
            StopAnimation();
            animationTimer.Tick -= AnimationTimer_Tick;
            btnMetricsAcceptancePlay.Content = "Play";
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceAnimation();
        }

        private void MetricsAcceptancePlayButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleAnimation();
        }

        private void MetricsAcceptanceStepButton_Click(object sender, RoutedEventArgs e)
        {
            StopAnimation();
            btnMetricsAcceptancePlay.Content = "Play";
            AdvanceAnimation();
        }

        private void MetricsAcceptanceResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAnimation();
        }
    }
}
