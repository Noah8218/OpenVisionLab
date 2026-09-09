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
    /// <summary>Owns binary-topic controls and timer lifetime; BinaryLearnPresenter owns fixed lesson state and decisions.</summary>
    public sealed partial class BinaryLearnView : UserControl
    {
        private readonly BinaryLearnPresenter presenter = new();
        private readonly DispatcherTimer morphologyAnimationTimer;
        private readonly DispatcherTimer blobAnimationTimer;
        private readonly DispatcherTimer contourAnimationTimer;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly List<Border> morphologyInputCells = new();
        private readonly List<Border> morphologyOutputCells = new();
        private readonly List<TextBlock> morphologyOutputTexts = new();
        private readonly List<Border> blobInputCells = new();
        private readonly List<Border> blobOutputCells = new();
        private readonly List<TextBlock> blobOutputTexts = new();
        private readonly List<Border> contourInputCells = new();
        private readonly List<Border> contourOutputCells = new();
        private readonly List<TextBlock> contourOutputTexts = new();
        private Action<VISION_MENU> openRelatedToolAction;

        public BinaryLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            morphologyAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(280) };
            blobAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            contourAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
            BuildMorphologyCells();
            BuildBlobCells();
            BuildContourCells();
            blobMinAreaSlider.Value = 3;
            UpdateMorphologyGuide();
            UpdateBlobGuide();
            UpdateContourGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal void SelectTopic(int topicIndex)
        {
            Visibility = topicIndex is 4 or 5 or 6 ? Visibility.Visible : Visibility.Collapsed;
            morphologyTopicPanel.Visibility = topicIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
            blobTopicPanel.Visibility = topicIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
            contourTopicPanel.Visibility = topicIndex == 6 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            btnMorphologyOpenTool.IsEnabled = action != null;
            btnBlobOpenTool.IsEnabled = action != null;
            btnContourOpenTool.IsEnabled = action != null;
        }

        internal void StopAnimations()
        {
            morphologyAnimationTimer.Stop();
            blobAnimationTimer.Stop();
            contourAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            morphologyAnimationTimer.Tick += MorphologyAnimationTimer_Tick;
            blobAnimationTimer.Tick += BlobAnimationTimer_Tick;
            contourAnimationTimer.Tick += ContourAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Retire callbacks on removal; rehosting preserves the lesson without starting playback.
            StopAnimations();
            morphologyAnimationTimer.Tick -= MorphologyAnimationTimer_Tick;
            blobAnimationTimer.Tick -= BlobAnimationTimer_Tick;
            contourAnimationTimer.Tick -= ContourAnimationTimer_Tick;
            btnMorphologyPlay.Content = "Play";
            btnBlobPlay.Content = "Play";
            btnContourPlay.Content = "Play";
        }

        internal bool CanOpenMorphologyToolForTest => btnMorphologyOpenTool.IsEnabled;
        internal bool CanOpenBlobToolForTest => btnBlobOpenTool.IsEnabled;
        internal bool CanOpenContourToolForTest => btnContourOpenTool.IsEnabled;
        internal string MorphologyToolLocationTitleForTest => txtMorphologyToolLocationTitle.Text ?? string.Empty;
        internal string MorphologyToolLocationDetailForTest => txtMorphologyToolLocationDetail.Text ?? string.Empty;
        internal string BlobToolLocationTitleForTest => txtBlobToolLocationTitle.Text ?? string.Empty;
        internal string BlobToolLocationDetailForTest => txtBlobToolLocationDetail.Text ?? string.Empty;
        internal string ContourToolLocationTitleForTest => txtContourToolLocationTitle.Text ?? string.Empty;
        internal string ContourToolLocationDetailForTest => txtContourToolLocationDetail.Text ?? string.Empty;

        internal int MorphologyModeIndexForTest
        {
            get => morphologyModeCombo.SelectedIndex;
            set => morphologyModeCombo.SelectedIndex = Math.Max(0, Math.Min(3, value));
        }

        internal string MorphologyFormulaTextForTest => txtMorphologyFormula.Text ?? string.Empty;

        internal int MorphologyAnimationStepForTest => presenter.MorphologyAnimationStep;

        internal string MorphologyAnimationStatusTextForTest => txtMorphologyAnimationStatus.Text ?? string.Empty;

        internal void ResetMorphologyAnimationForTest()
        {
            ResetMorphologyAnimation();
        }

        internal void AdvanceMorphologyAnimationForTest()
        {
            AdvanceMorphologyAnimation();
        }

        internal void ToggleMorphologyAnimationForTest()
        {
            MorphologyPlayButton_Click(this, new RoutedEventArgs());
        }

        internal double BlobMinAreaForTest
        {
            get => blobMinAreaSlider.Value;
            set => blobMinAreaSlider.Value = Math.Max(1, Math.Min(6, value));
        }

        internal string BlobFormulaTextForTest => txtBlobFormula.Text ?? string.Empty;

        internal int BlobAnimationStepForTest => presenter.BlobAnimationStep;

        internal string BlobAnimationStatusTextForTest => txtBlobAnimationStatus.Text ?? string.Empty;

        internal void ResetBlobAnimationForTest()
        {
            ResetBlobAnimation();
        }

        internal void AdvanceBlobAnimationForTest()
        {
            AdvanceBlobAnimation();
        }

        internal void ToggleBlobAnimationForTest()
        {
            BlobPlayButton_Click(this, new RoutedEventArgs());
        }

        internal int ContourDrawModeIndexForTest
        {
            get => contourDrawModeCombo.SelectedIndex;
            set => contourDrawModeCombo.SelectedIndex = Math.Max(0, Math.Min(2, value));
        }

        internal string ContourFormulaTextForTest => txtContourFormula.Text ?? string.Empty;

        internal int ContourAnimationStepForTest => presenter.ContourAnimationStep;

        internal string ContourAnimationStatusTextForTest => txtContourAnimationStatus.Text ?? string.Empty;

        internal void ResetContourAnimationForTest()
        {
            ResetContourAnimation();
        }

        internal void AdvanceContourAnimationForTest()
        {
            AdvanceContourAnimation();
        }

        internal void ToggleContourAnimationForTest()
        {
            ContourPlayButton_Click(this, new RoutedEventArgs());
        }

        private void BuildMorphologyCells()
        {
            morphologyInputGrid.Children.Clear();
            morphologyOutputGrid.Children.Clear();
            morphologyInputCells.Clear();
            morphologyOutputCells.Clear();
            morphologyOutputTexts.Clear();

            foreach (int value in presenter.MorphologySamples)
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

            foreach (int value in presenter.BlobSamples)
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

            foreach (int value in presenter.ContourSamples)
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

        internal void UpdateMorphologyGuide()
        {
            presenter.UpdateMorphology(GetSelectedMorphologyMode());
            txtMorphologyFormula.Text = presenter.MorphologyFormulaText;
            txtMorphologyMeaning.Text = presenter.MorphologyMeaningText;
            morphologyAnimationTimer.Stop();
            btnMorphologyPlay.Content = "Play";
            PaintMorphologyAnimationFrame();
        }

        private void PaintMorphologyAnimationFrame()
        {
            presenter.SynchronizeMorphologyMode(GetSelectedMorphologyMode());
            IReadOnlyList<bool> result = presenter.MorphologyResult;
            SolidColorBrush defaultBorder = new(Color.FromRgb(209, 213, 219));
            SolidColorBrush kernelBorder = new(Color.FromRgb(21, 124, 134));
            SolidColorBrush centerBorder = new(Color.FromRgb(217, 119, 6));

            foreach (Border inputCell in morphologyInputCells)
            {
                inputCell.BorderBrush = defaultBorder;
                inputCell.BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < result.Count; i++)
            {
                bool processed = presenter.IsMorphologyCellProcessed(i);
                int value = processed && result[i] ? 255 : processed ? 0 : 226;
                morphologyOutputCells[i].Background = CreateGrayBrush(value);
                morphologyOutputTexts[i].Foreground = processed
                    ? value > 0 ? Brushes.Black : Brushes.White
                    : new SolidColorBrush(Color.FromRgb(75, 85, 99));
                morphologyOutputTexts[i].Text = processed ? value > 0 ? "1" : "0" : ".";
            }

            if (presenter.ActiveMorphologyCellIndex >= 0)
            {
                int active = presenter.ActiveMorphologyCellIndex;
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
                txtMorphologyAnimationStatus.Text = presenter.MorphologyAnimationStatusText;
                return;
            }

            txtMorphologyAnimationStatus.Text = presenter.MorphologyAnimationStatusText;
        }

        internal void UpdateBlobGuide()
        {
            presenter.UpdateBlob(blobMinAreaSlider.Value);
            txtBlobMinArea.Text = presenter.BlobMinimumAreaText;
            txtBlobFormula.Text = presenter.BlobFormulaText;
            txtBlobMeaning.Text = presenter.BlobMeaningText;
            PaintBlobAnimationFrame();
        }

        private void PaintBlobAnimationFrame()
        {
            presenter.UpdateBlob(blobMinAreaSlider.Value);
            foreach (Border inputCell in blobInputCells)
            {
                inputCell.BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                inputCell.BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < presenter.BlobLabels.Count; i++)
            {
                BinaryLearnPresenter.CellKind kind = presenter.GetBlobCellKind(i);
                Brush background = kind switch
                {
                    BinaryLearnPresenter.CellKind.Background => Brushes.Black,
                    BinaryLearnPresenter.CellKind.Pending => new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                    BinaryLearnPresenter.CellKind.Rejected => animationNeutralBrush,
                    BinaryLearnPresenter.CellKind.Candidate => animationCandidateBrush,
                    _ => animationPassBrush
                };
                PaintBlobCell(i, background, Brushes.White, presenter.GetBlobCellText(i));
                if (kind != BinaryLearnPresenter.CellKind.Background && kind != BinaryLearnPresenter.CellKind.Pending && i < blobInputCells.Count)
                {
                    blobInputCells[i].BorderBrush = background;
                    blobInputCells[i].BorderThickness = presenter.BlobLabels[i] == presenter.VisibleBlobCount ? new Thickness(3) : new Thickness(2);
                }
            }

            txtBlobAnimationStatus.Text = presenter.BlobAnimationStatusText;
        }

        internal void UpdateContourGuide()
        {
            presenter.UpdateContour(GetSelectedContourDrawMode());
            txtContourFormula.Text = presenter.ContourFormulaText;
            txtContourMeaning.Text = presenter.ContourMeaningText;
            PaintContourAnimationFrame();
        }

        private void PaintContourAnimationFrame()
        {
            presenter.UpdateContour(GetSelectedContourDrawMode());
            Brush acceptedBrush = new SolidColorBrush(Color.FromRgb(229, 244, 247));
            foreach (Border inputCell in contourInputCells)
            {
                inputCell.BorderBrush = new SolidColorBrush(Color.FromRgb(209, 213, 219));
                inputCell.BorderThickness = new Thickness(1);
            }

            for (int i = 0; i < presenter.ContourSamples.Count; i++)
            {
                if (presenter.IsContourInputHighlighted(i) && i < contourInputCells.Count)
                {
                    contourInputCells[i].BorderBrush = presenter.IsContourRegionAccepted(i) ? animationCandidateBrush : animationNeutralBrush;
                    contourInputCells[i].BorderThickness = new Thickness(2);
                }

                BinaryLearnPresenter.CellKind kind = presenter.GetContourCellKind(i);
                Brush background = kind switch
                {
                    BinaryLearnPresenter.CellKind.Background => Brushes.Black,
                    BinaryLearnPresenter.CellKind.Rejected => animationNeutralBrush,
                    BinaryLearnPresenter.CellKind.Contour => animationCandidateBrush,
                    BinaryLearnPresenter.CellKind.Box => animationPassBrush,
                    _ => acceptedBrush
                };
                Brush foreground = kind == BinaryLearnPresenter.CellKind.Region ? Brushes.Black : Brushes.White;
                PaintContourCell(i, background, foreground, presenter.GetContourCellText(i));
            }

            txtContourAnimationStatus.Text = presenter.ContourAnimationStatusText;
        }

        private void ResetMorphologyAnimation()
        {
            morphologyAnimationTimer.Stop();
            btnMorphologyPlay.Content = "Play";
            presenter.ResetMorphologyAnimation();
            PaintMorphologyAnimationFrame();
        }

        private void AdvanceMorphologyAnimation()
        {
            presenter.AdvanceMorphologyAnimation();
            PaintMorphologyAnimationFrame();
            if (presenter.IsMorphologyAnimationComplete)
            {
                morphologyAnimationTimer.Stop();
                btnMorphologyPlay.Content = "Play";
            }
        }

        private void ResetBlobAnimation()
        {
            blobAnimationTimer.Stop();
            btnBlobPlay.Content = "Play";
            presenter.ResetBlobAnimation();
            PaintBlobAnimationFrame();
        }

        private void AdvanceBlobAnimation()
        {
            presenter.AdvanceBlobAnimation();
            PaintBlobAnimationFrame();
            if (presenter.IsBlobAnimationComplete)
            {
                blobAnimationTimer.Stop();
                btnBlobPlay.Content = "Play";
            }
        }

        private void ResetContourAnimation()
        {
            contourAnimationTimer.Stop();
            btnContourPlay.Content = "Play";
            presenter.ResetContourAnimation();
            PaintContourAnimationFrame();
        }

        private void AdvanceContourAnimation()
        {
            presenter.AdvanceContourAnimation();
            PaintContourAnimationFrame();
            if (presenter.IsContourAnimationComplete)
            {
                contourAnimationTimer.Stop();
                btnContourPlay.Content = "Play";
            }
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

        private void PaintBlobCell(int index, Brush background, Brush foreground, string text)
        {
            blobOutputCells[index].Background = background;
            blobOutputTexts[index].Foreground = foreground;
            blobOutputTexts[index].Text = text;
        }

        private void PaintContourCell(int index, Brush background, Brush foreground, string text)
        {
            contourOutputCells[index].Background = background;
            contourOutputTexts[index].Foreground = foreground;
            contourOutputTexts[index].Text = text;
        }

        private void MorphologyAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMorphologyAnimation();
        }

        private void BlobAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceBlobAnimation();
        }

        private void ContourAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceContourAnimation();
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

            if (presenter.IsMorphologyAnimationComplete)
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

            if (presenter.IsBlobAnimationComplete)
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

            if (presenter.IsContourAnimationComplete)
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
                presenter.CompleteContourAnimation();
                UpdateContourGuide();
            }
        }

        private string GetSelectedMorphologyMode()
        {
            return (morphologyModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Erosion";
        }

        private string GetSelectedContourDrawMode()
        {
            return (contourDrawModeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Contour";
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                // Keep callback-first ordering: a failed Tool open must not display a successful-open hint.
                openRelatedToolAction?.Invoke(menu);
                if (menu == VISION_MENU.Morphology && ReferenceEquals(button, btnMorphologyOpenTool))
                {
                    txtMorphologyToolLocationTitle.Text = BinaryLearnPresenter.MorphologyOpenedTitle;
                    txtMorphologyToolLocationDetail.Text = BinaryLearnPresenter.MorphologyOpenedDetail;
                    return;
                }

                if (menu == VISION_MENU.Blob && ReferenceEquals(button, btnBlobOpenTool))
                {
                    txtBlobToolLocationTitle.Text = BinaryLearnPresenter.BlobOpenedTitle;
                    txtBlobToolLocationDetail.Text = BinaryLearnPresenter.BlobOpenedDetail;
                    return;
                }

                if (menu == VISION_MENU.Contour && ReferenceEquals(button, btnContourOpenTool))
                {
                    txtContourToolLocationTitle.Text = BinaryLearnPresenter.ContourOpenedTitle;
                    txtContourToolLocationDetail.Text = BinaryLearnPresenter.ContourOpenedDetail;
                }
            }
        }
    }
}
