using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class VisionToolVerificationGuideView : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata("검증 흐름"));

        public static readonly DependencyProperty StateTextProperty =
            DependencyProperty.Register(
                nameof(StateText),
                typeof(string),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CriteriaTextProperty =
            DependencyProperty.Register(
                nameof(CriteriaText),
                typeof(string),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NextActionTextProperty =
            DependencyProperty.Register(
                nameof(NextActionText),
                typeof(string),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StateBrushProperty =
            DependencyProperty.Register(
                nameof(StateBrush),
                typeof(Brush),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsCompactModeProperty =
            DependencyProperty.Register(
                nameof(IsCompactMode),
                typeof(bool),
                typeof(VisionToolVerificationGuideView),
                new PropertyMetadata(false, OnIsCompactModeChanged));

        public VisionToolVerificationGuideView()
        {
            InitializeComponent();
            ApplyDensity();
        }

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public string StateText
        {
            get => (string)GetValue(StateTextProperty);
            set => SetValue(StateTextProperty, value);
        }

        public string CriteriaText
        {
            get => (string)GetValue(CriteriaTextProperty);
            set => SetValue(CriteriaTextProperty, value);
        }

        public string NextActionText
        {
            get => (string)GetValue(NextActionTextProperty);
            set => SetValue(NextActionTextProperty, value);
        }

        public Brush StateBrush
        {
            get => (Brush)GetValue(StateBrushProperty);
            set => SetValue(StateBrushProperty, value);
        }

        public bool IsCompactMode
        {
            get => (bool)GetValue(IsCompactModeProperty);
            set => SetValue(IsCompactModeProperty, value);
        }

        private static void OnIsCompactModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VisionToolVerificationGuideView view)
            {
                view.ApplyDensity();
            }
        }

        private void ApplyDensity()
        {
            bool compact = IsCompactMode;
            headerGrid.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            guideChrome.Padding = compact ? new Thickness(8, 2, 8, 2) : new Thickness(8, 5, 8, 5);
            rowHeader.Height = compact ? new GridLength(0D) : GridLength.Auto;
            rowHeaderGap.Height = compact ? new GridLength(0D) : new GridLength(1D);
            txtNextAction.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            rowNextActionGap.Height = compact ? new GridLength(0D) : new GridLength(1D);
            rowNextAction.Height = compact ? new GridLength(0D) : GridLength.Auto;
        }
    }
}
