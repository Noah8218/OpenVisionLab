using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.LearnCellVisuals;

namespace OpenVisionLab
{
    /// <summary>Owns the Layer/Recipe lesson controls and timer; the presenter owns its teaching routes.</summary>
    public sealed partial class LayerRecipeLearnView : UserControl
    {
        private readonly LayerRecipeLearnPresenter presenter = new();
        private readonly DispatcherTimer layerRecipeAnimationTimer;
        private readonly List<Border> layerRecipeLayerCells = new();
        private readonly List<TextBlock> layerRecipeLayerTexts = new();
        private readonly List<Border> layerRecipeFlowCells = new();
        private readonly List<TextBlock> layerRecipeFlowTexts = new();
        private bool isLayerRecipeAnimationAdvancing;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;

        public LayerRecipeLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            layerRecipeAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            BuildLayerRecipeCells();
            layerRecipeStepSlider.Value = 2;
            UpdateLayerRecipeGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal double LayerRecipeSelectedStepForTest
        {
            get => layerRecipeStepSlider.Value;
            set => layerRecipeStepSlider.Value = Math.Max(1, Math.Min(4, value));
        }

        internal string LayerRecipeFormulaTextForTest => txtLayerRecipeFormula.Text ?? string.Empty;

        internal int LayerRecipeAnimationStepForTest => presenter.AnimationStep;

        internal string LayerRecipeAnimationStatusTextForTest => txtLayerRecipeAnimationStatus.Text ?? string.Empty;

        internal void RefreshSelection()
        {
            UpdateLayerRecipeGuide();
        }

        internal void StopAnimation()
        {
            layerRecipeAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            layerRecipeAnimationTimer.Tick += LayerRecipeAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // A detached View retires its timer; rehosting preserves selection without restarting playback.
            StopAnimation();
            layerRecipeAnimationTimer.Tick -= LayerRecipeAnimationTimer_Tick;
            btnLayerRecipePlay.Content = "Play";
        }

        internal void ResetLayerRecipeAnimationForTest()
        {
            ResetLayerRecipeAnimation();
        }

        internal void AdvanceLayerRecipeAnimationForTest()
        {
            AdvanceLayerRecipeAnimation();
        }

        internal void ToggleLayerRecipeAnimationForTest()
        {
            LayerRecipePlayButton_Click(this, new RoutedEventArgs());
        }

        private void BuildLayerRecipeCells()
        {
            layerRecipeLayerGrid.Children.Clear();
            layerRecipeFlowGrid.Children.Clear();
            layerRecipeLayerCells.Clear();
            layerRecipeLayerTexts.Clear();
            layerRecipeFlowCells.Clear();
            layerRecipeFlowTexts.Clear();

            foreach (string layer in presenter.Layers)
            {
                Border cell = CreateSmallValueCell(layer, 230);
                TextBlock text = (TextBlock)cell.Child;
                layerRecipeLayerGrid.Children.Add(cell);
                layerRecipeLayerCells.Add(cell);
                layerRecipeLayerTexts.Add(text);
            }

            for (int i = 0; i < presenter.Steps.Count; i++)
            {
                AddLayerRecipeFlowCell((i + 1).ToString(CultureInfo.InvariantCulture));
                AddLayerRecipeFlowCell(presenter.Steps[i].Input);
                AddLayerRecipeFlowCell(presenter.Steps[i].Tool);
                AddLayerRecipeFlowCell(presenter.Steps[i].Output);
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

        private void UpdateLayerRecipeGuide()
        {
            presenter.SelectStep(layerRecipeStepSlider.Value);
            txtLayerRecipeSelectedStep.Text = presenter.SelectedStepText;
            txtLayerRecipeFormula.Text = presenter.FormulaText;
            txtLayerRecipeMeaning.Text = presenter.MeaningText;
            txtLayerRecipeAnimationStatus.Text = presenter.AnimationStatusText;

            for (int i = 0; i < layerRecipeFlowCells.Count; i++)
            {
                bool selectedRow = presenter.IsSelectedFlowCell(i);
                layerRecipeFlowCells[i].Background = selectedRow ? animationCandidateBrush : CreateGrayBrush(230);
                layerRecipeFlowTexts[i].Foreground = selectedRow ? Brushes.White : Brushes.Black;
            }

            for (int i = 0; i < layerRecipeLayerCells.Count; i++)
            {
                bool routeLayer = presenter.IsRouteLayer(i);
                layerRecipeLayerCells[i].Background = routeLayer ? animationPassBrush : CreateGrayBrush(230);
                layerRecipeLayerTexts[i].Foreground = routeLayer ? Brushes.White : Brushes.Black;
            }
        }

        private void ResetLayerRecipeAnimation()
        {
            layerRecipeAnimationTimer.Stop();
            btnLayerRecipePlay.Content = "Play";
            presenter.ResetAnimation();
            txtLayerRecipeSelectedStep.Text = presenter.SelectedStepText;
            txtLayerRecipeAnimationStatus.Text = presenter.AnimationStatusText;

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
            presenter.AdvanceAnimation();
            // Timer-driven slider synchronization must not be treated as a manual pause.
            isLayerRecipeAnimationAdvancing = true;
            try
            {
                layerRecipeStepSlider.Value = presenter.AnimationStep;
                UpdateLayerRecipeGuide();
            }
            finally
            {
                isLayerRecipeAnimationAdvancing = false;
            }
            if (presenter.IsAnimationComplete)
            {
                layerRecipeAnimationTimer.Stop();
                btnLayerRecipePlay.Content = "Play";
            }
        }

        private void LayerRecipeAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceLayerRecipeAnimation();
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

            if (presenter.IsAnimationComplete)
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
    }
}
