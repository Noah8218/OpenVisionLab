using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    /// <summary>Owns foundation-topic controls and timers; FoundationLearnPresenter owns lesson state and presentation decisions.</summary>
    public sealed partial class FoundationLearnView : UserControl
    {
        private readonly FoundationLearnPresenter presenter = new();
        private readonly DispatcherTimer foundationAnimationTimer;
        private readonly DispatcherTimer matChannelAnimationTimer;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
        private readonly List<Border> foundationMatCells = new();
        private Action<VISION_MENU> openRelatedToolAction;

        public FoundationLearnView()
        {
            InitializeComponent();
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            foundationAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            matChannelAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(620) };
            BuildFoundationCells();
            UpdateFoundationGuide();
            UpdateMatChannelGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal void SelectTopic(int topicIndex)
        {
            Visibility = topicIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            pixelTopicPanel.Visibility = topicIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            btnFoundationOpenRoiTool.IsEnabled = action != null;
            btnFoundationOpenKernelTool.IsEnabled = action != null;
            btnFoundationOpenOutputSizeTool.IsEnabled = action != null;
        }

        internal void StopAnimations()
        {
            foundationAnimationTimer.Stop();
            matChannelAnimationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foundationAnimationTimer.Tick += FoundationAnimationTimer_Tick;
            matChannelAnimationTimer.Tick += MatChannelAnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Retire callbacks on removal; rehosting retains stages and does not start playback.
            StopAnimations();
            foundationAnimationTimer.Tick -= FoundationAnimationTimer_Tick;
            matChannelAnimationTimer.Tick -= MatChannelAnimationTimer_Tick;
            btnFoundationPlay.Content = "자동 재생";
            btnMatChannelPlay.Content = "자동 재생";
        }

        internal bool CanOpenRelatedToolsForTest => btnFoundationOpenRoiTool.IsEnabled
            && btnFoundationOpenKernelTool.IsEnabled
            && btnFoundationOpenOutputSizeTool.IsEnabled;

        internal string FoundationToolLocationTitleForTest => txtFoundationToolLocationTitle.Text ?? string.Empty;

        internal string FoundationToolLocationDetailForTest => txtFoundationToolLocationDetail.Text ?? string.Empty;

        internal void BringFoundationToolLocationIntoViewForTest()
        {
            foundationToolLocationPanel.BringIntoView();
        }

        internal int FoundationAnimationStepForTest => presenter.FoundationAnimationStep;

        internal int FoundationSelectedCellCountForTest => presenter.FoundationSelectedCellCount;

        internal string FoundationAnimationStatusTextForTest => txtFoundationAnimationStatus.Text ?? string.Empty;

        internal bool IsFoundationPointVisibleForTest => foundationPointMarker.Visibility == Visibility.Visible;

        internal bool IsFoundationRectVisibleForTest => foundationRoiRect.Visibility == Visibility.Visible;

        internal bool IsFoundationRotatedRectVisibleForTest => foundationRotatedRect.Visibility == Visibility.Visible;

        internal bool IsFoundationRotatedBoundsVisibleForTest => foundationRotatedBoundsRect.Visibility == Visibility.Visible;

        internal bool IsFoundationRotatedCenterVisibleForTest => foundationRotatedCenterMarker.Visibility == Visibility.Visible;

        internal double FoundationRotatedRectAngleForTest =>
            (foundationRotatedRect.RenderTransform as RotateTransform)?.Angle ?? 0D;

        internal void ResetFoundationAnimationForTest()
        {
            ResetFoundationAnimation();
        }

        internal void AdvanceFoundationAnimationForTest()
        {
            AdvanceFoundationAnimation();
        }

        internal void ToggleFoundationAnimationForTest()
        {
            FoundationPlayButton_Click(this, new RoutedEventArgs());
        }

        internal int MatChannelAnimationStepForTest => presenter.MatChannelAnimationStep;

        internal string MatChannelAnimationStatusTextForTest => txtMatChannelAnimationStatus.Text ?? string.Empty;

        internal double MatChannelSplitOpacityForTest => matChannelBlueCell.Opacity;

        internal double MatChannelGrayOpacityForTest => matChannelGrayCell.Opacity;

        internal string MatChannelBgrShapeTextForTest => txtMatChannelBgrShape.Text ?? string.Empty;

        internal string MatChannelGrayShapeTextForTest => txtMatChannelGrayShape.Text ?? string.Empty;

        internal double MatChannelTypeGuideOpacityForTest => matChannelTypeGuidePanel.Opacity;

        internal string MatChannelTypeTitleForTest => txtMatChannelTypeTitle.Text ?? string.Empty;

        internal string MatChannelTypeDetailForTest => txtMatChannelTypeDetail.Text ?? string.Empty;

        internal void ResetMatChannelAnimationForTest()
        {
            ResetMatChannelAnimation();
        }

        internal void AdvanceMatChannelAnimationForTest()
        {
            AdvanceMatChannelAnimation();
        }

        internal void ToggleMatChannelAnimationForTest()
        {
            MatChannelPlayButton_Click(this, new RoutedEventArgs());
        }

        private void BuildFoundationCells()
        {
            foundationMatGrid.Children.Clear();
            foundationMatCells.Clear();
            for (int i = 0; i < FoundationLearnPresenter.FoundationCellCount; i++)
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

        internal void UpdateFoundationGuide()
        {
            for (int i = 0; i < foundationMatCells.Count; i++)
            {
                FoundationLearnRole role = presenter.GetFoundationCellRole(i);
                foundationMatCells[i].Background = role == FoundationLearnRole.Default
                    ? new SolidColorBrush(Color.FromRgb(248, 250, 252))
                    : ResolveBrush(role);
            }

            foundationPointMarker.Visibility = presenter.IsFoundationPointVisible ? Visibility.Visible : Visibility.Collapsed;
            foundationPointMarker.Fill = ResolveBrush(presenter.FoundationPointRole);
            foundationRoiRect.Visibility = presenter.IsFoundationRectVisible ? Visibility.Visible : Visibility.Collapsed;
            foundationRoiRect.Stroke = ResolveBrush(presenter.FoundationRectRole);
            foundationRotatedRect.Visibility = presenter.IsFoundationRotatedRectVisible ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedRect.Stroke = animationPassBrush;
            foundationRotatedBoundsRect.Visibility = presenter.IsFoundationRotatedRectVisible ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedBoundsRect.Stroke = animationWarningBrush;
            foundationRotatedCenterMarker.Visibility = presenter.IsFoundationRotatedRectVisible ? Visibility.Visible : Visibility.Collapsed;
            foundationRotatedCenterMarker.Fill = animationCandidateBrush;
            txtFoundationAnimationStatus.Text = presenter.FoundationAnimationStatusText;
        }

        internal void UpdateMatChannelGuide()
        {
            matChannelBgrCell.Opacity = 1D;
            matChannelBgrCell.BorderBrush = ResolveBrush(presenter.MatBgrBorderRole);
            matChannelBlueCell.Opacity = presenter.AreMatSplitChannelsRevealed ? 1D : 0.28D;
            matChannelGreenCell.Opacity = presenter.AreMatSplitChannelsRevealed ? 1D : 0.28D;
            matChannelRedCell.Opacity = presenter.AreMatSplitChannelsRevealed ? 1D : 0.28D;
            matChannelBlueCell.BorderBrush = ResolveBrush(presenter.MatSplitBorderRole);
            matChannelGreenCell.BorderBrush = ResolveBrush(presenter.MatSplitBorderRole);
            matChannelRedCell.BorderBrush = ResolveBrush(presenter.MatSplitBorderRole);
            matChannelGrayCell.Opacity = presenter.IsMatGrayChannelRevealed ? 1D : 0.28D;
            matChannelGrayCell.BorderBrush = ResolveBrush(presenter.MatGrayBorderRole);
            txtMatChannelBgrShape.Opacity = presenter.AreMatSplitChannelsRevealed ? 1D : 0.45D;
            txtMatChannelGrayShape.Opacity = presenter.IsMatGrayChannelRevealed ? 1D : 0.45D;
            matChannelTypeGuidePanel.Opacity = presenter.IsMatTypeGuideRevealed ? 1D : 0.28D;
            txtMatChannelAnimationStatus.Text = presenter.MatChannelAnimationStatusText;
        }

        private Brush ResolveBrush(FoundationLearnRole role)
        {
            return role switch
            {
                FoundationLearnRole.Candidate => animationCandidateBrush,
                FoundationLearnRole.Pass => animationPassBrush,
                FoundationLearnRole.Warning => animationWarningBrush,
                _ => new SolidColorBrush(Color.FromRgb(209, 213, 219))
            };
        }

        internal void UpdateToolLocation(VISION_MENU menu)
        {
            presenter.UpdateToolLocation(menu);
            txtFoundationToolLocationTitle.Text = presenter.ToolLocationTitle;
            txtFoundationToolLocationDetail.Text = presenter.ToolLocationDetail;
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                // Preserve callback ordering: a failed Tool open must leave the existing hint untouched.
                openRelatedToolAction?.Invoke(menu);
                UpdateToolLocation(menu);
            }
        }

        private void ResetFoundationAnimation()
        {
            foundationAnimationTimer.Stop();
            btnFoundationPlay.Content = "자동 재생";
            presenter.ResetFoundationAnimation();
            UpdateFoundationGuide();
        }

        private void AdvanceFoundationAnimation()
        {
            presenter.AdvanceFoundationAnimation();
            UpdateFoundationGuide();
            if (presenter.IsFoundationAnimationComplete)
            {
                foundationAnimationTimer.Stop();
                btnFoundationPlay.Content = "자동 재생";
            }
        }

        private void FoundationAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceFoundationAnimation();
        }

        private void FoundationPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (foundationAnimationTimer.IsEnabled)
            {
                foundationAnimationTimer.Stop();
                btnFoundationPlay.Content = "자동 재생";
                return;
            }

            if (presenter.IsFoundationAnimationComplete)
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

        private void ResetMatChannelAnimation()
        {
            matChannelAnimationTimer.Stop();
            btnMatChannelPlay.Content = "자동 재생";
            presenter.ResetMatChannelAnimation();
            UpdateMatChannelGuide();
        }

        private void AdvanceMatChannelAnimation()
        {
            presenter.AdvanceMatChannelAnimation();
            UpdateMatChannelGuide();
            if (presenter.IsMatChannelAnimationComplete)
            {
                matChannelAnimationTimer.Stop();
                btnMatChannelPlay.Content = "자동 재생";
            }
        }

        private void MatChannelAnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceMatChannelAnimation();
        }

        private void MatChannelPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (matChannelAnimationTimer.IsEnabled)
            {
                matChannelAnimationTimer.Stop();
                btnMatChannelPlay.Content = "자동 재생";
                return;
            }

            if (presenter.IsMatChannelAnimationComplete)
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
    }
}
