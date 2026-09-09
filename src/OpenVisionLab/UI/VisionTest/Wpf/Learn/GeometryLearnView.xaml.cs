using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    /// <summary>Owns Geometry Transform controls, rendering and timer lifetime; GeometryLearnPresenter owns lesson policy.</summary>
    public sealed partial class GeometryLearnView : UserControl
    {
        private readonly GeometryLearnPresenter presenter = new();
        private readonly DispatcherTimer animationTimer;
        private readonly Brush animationNeutralBrush;
        private readonly Brush animationCandidateBrush;
        private readonly Brush animationPassBrush;
        private readonly Brush animationWarningBrush;
        private Action<VISION_MENU> openRelatedToolAction;

        public GeometryLearnView()
        {
            InitializeComponent();
            animationNeutralBrush = (Brush)FindResource("Learn.Animation.NeutralBrush");
            animationCandidateBrush = (Brush)FindResource("Learn.Animation.CandidateBrush");
            animationPassBrush = (Brush)FindResource("Learn.Animation.PassBrush");
            animationWarningBrush = (Brush)FindResource("Learn.Animation.WarningBrush");
            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            geometryAngleSlider.Value = 15;
            geometryScaleSlider.Value = 100;
            UpdateGeometryGuide();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        internal void SelectTopic(int topicIndex)
        {
            Visibility = topicIndex == 15 ? Visibility.Visible : Visibility.Collapsed;
            geometryTopicPanel.Visibility = topicIndex == 15 ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void SetOpenRelatedToolAction(Action<VISION_MENU> action)
        {
            openRelatedToolAction = action;
            bool enabled = action != null;
            btnGeometryOpenTool.IsEnabled = enabled;
            btnGeometryOpenAffineTool.IsEnabled = enabled;
        }

        internal void StopAnimations()
        {
            animationTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopAnimations();
            animationTimer.Tick -= AnimationTimer_Tick;
            btnGeometryPlay.Content = "Play";
        }

        internal bool CanOpenGeometryToolForTest => btnGeometryOpenTool.IsEnabled;

        internal bool CanOpenAffineTransformToolForTest => btnGeometryOpenAffineTool.IsEnabled;

        internal string GeometryToolLocationTitleForTest => txtGeometryToolLocationTitle.Text ?? string.Empty;

        internal string GeometryToolLocationDetailForTest => txtGeometryToolLocationDetail.Text ?? string.Empty;

        internal void BringGeometryToolLocationIntoViewForTest()
        {
            geometryToolLocationPanel.BringIntoView();
        }

        internal double GeometryAngleForTest
        {
            get => geometryAngleSlider.Value;
            set => geometryAngleSlider.Value = Math.Max(-45, Math.Min(45, value));
        }

        internal double GeometryScaleForTest
        {
            get => geometryScaleSlider.Value;
            set => geometryScaleSlider.Value = Math.Max(50, Math.Min(150, value));
        }

        internal string GeometryFormulaTextForTest => txtGeometryFormula.Text ?? string.Empty;

        internal int GeometryAnimationStepForTest => presenter.AnimationStep;

        internal string GeometryAnimationStatusTextForTest => txtGeometryAnimationStatus.Text ?? string.Empty;

        internal double GeometryRenderedAngleForTest => geometryRotateTransform.Angle;

        internal double GeometryRenderedScaleForTest => geometryScaleTransform.ScaleX;

        internal void ResetGeometryAnimationForTest()
        {
            ResetAnimation();
        }

        internal void AdvanceGeometryAnimationForTest()
        {
            AdvanceAnimation();
        }

        internal void ToggleGeometryAnimationForTest()
        {
            GeometryPlayButton_Click(this, new RoutedEventArgs());
        }

        internal void UpdateGeometryGuide()
        {
            txtGeometryAngle.Text = presenter.AngleText;
            txtGeometryScale.Text = presenter.ScaleText;
            txtGeometryFormula.Text = presenter.FormulaText;
            txtGeometryMeaning.Text = presenter.MeaningText;
            geometrySourceBox.Stroke = ResolveBrush(presenter.SourceRole);
            geometryTargetBox.Background = ResolveBrush(presenter.TargetRole);
            geometryTargetBox.Opacity = presenter.AnimationStep == 0 ? 0.28D : 0.78D;
            geometryRotateTransform.Angle = presenter.IsRotationApplied ? presenter.Angle : 0D;
            geometryScaleTransform.ScaleX = presenter.IsScaleApplied ? presenter.Scale / 100D : 1D;
            geometryScaleTransform.ScaleY = presenter.IsScaleApplied ? presenter.Scale / 100D : 1D;
            txtGeometryAnimationStatus.Text = presenter.AnimationStatusText;
        }

        internal void UpdateToolLocation(VISION_MENU menu)
        {
            presenter.UpdateToolLocation(menu);
            txtGeometryToolLocationTitle.Text = presenter.ToolLocationTitle;
            txtGeometryToolLocationDetail.Text = presenter.ToolLocationDetail;
        }

        private Brush ResolveBrush(GeometryLearnRole role)
        {
            return role switch
            {
                GeometryLearnRole.Candidate => animationCandidateBrush,
                GeometryLearnRole.Pass => animationPassBrush,
                GeometryLearnRole.Warning => animationWarningBrush,
                _ => animationNeutralBrush
            };
        }

        private void OpenRelatedToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && Enum.TryParse(Convert.ToString(button.Tag, CultureInfo.InvariantCulture), out VISION_MENU menu))
            {
                // Preserve callback ordering: a failed Tool open leaves the existing hint unchanged.
                openRelatedToolAction?.Invoke(menu);
                UpdateToolLocation(menu);
            }
        }

        private void ResetAnimation()
        {
            animationTimer.Stop();
            btnGeometryPlay.Content = "Play";
            presenter.ResetAnimation();
            UpdateGeometryGuide();
        }

        private void AdvanceAnimation()
        {
            presenter.AdvanceAnimation();
            UpdateGeometryGuide();
            if (presenter.IsAnimationComplete)
            {
                animationTimer.Stop();
                btnGeometryPlay.Content = "Play";
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceAnimation();
        }

        private void GeometryPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (animationTimer.IsEnabled)
            {
                animationTimer.Stop();
                btnGeometryPlay.Content = "Play";
                return;
            }

            if (presenter.IsAnimationComplete)
            {
                ResetAnimation();
            }

            btnGeometryPlay.Content = "Pause";
            animationTimer.Start();
        }

        private void GeometryStepButton_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Stop();
            btnGeometryPlay.Content = "Play";
            AdvanceAnimation();
        }

        private void GeometryResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAnimation();
        }

        private void GeometrySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                animationTimer.Stop();
                btnGeometryPlay.Content = "Play";
                presenter.UpdateSettings(geometryAngleSlider.Value, geometryScaleSlider.Value);
                UpdateGeometryGuide();
            }
        }
    }
}
