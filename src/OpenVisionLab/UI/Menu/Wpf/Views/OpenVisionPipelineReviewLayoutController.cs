using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class OpenVisionPipelineReviewLayoutController
    {
        private readonly Func<string, string, string> localize;
        private readonly Grid reviewSummaryGrid;
        private readonly RowDefinition reviewSummaryRow;
        private readonly RowDefinition reviewSummaryGapRow;
        private readonly FrameworkElement reviewDetailHost;
        private readonly FrameworkElement reviewDetailTabs;
        private readonly FrameworkElement reviewDetailSplitter;
        private readonly RowDefinition reviewDetailGapRow;
        private readonly RowDefinition reviewDetailRow;
        private readonly ToggleButton reviewGuideToggle;
        private readonly ToggleButton reviewDetailsToggle;
        private readonly PackIconMaterial reviewDetailsToggleIcon;
        private readonly TabItem matcherDiagnosticTab;
        private readonly TabItem objectInspectorTab;
        private readonly TabItem circleEvidenceTab;
        private readonly ToggleButton stepFlowToggle;
        private readonly ColumnDefinition stepFlowColumn;
        private readonly Border stepFlowPanel;
        private readonly TextBlock stepFlowLabel;
        private readonly FrameworkElement stepFlowFocusHost;
        private readonly FrameworkElement pipelineFlowView;
        private readonly PackIconMaterial stepFlowToggleIcon;
        private bool useCompactImageLayout;
        private bool reviewDetailsExpanded;
        private bool stepFlowExpanded = true;

        internal OpenVisionPipelineReviewLayoutController(
            Func<string, string, string> localize,
            Grid reviewSummaryGrid,
            RowDefinition reviewSummaryRow,
            RowDefinition reviewSummaryGapRow,
            FrameworkElement reviewDetailHost,
            FrameworkElement reviewDetailTabs,
            FrameworkElement reviewDetailSplitter,
            RowDefinition reviewDetailGapRow,
            RowDefinition reviewDetailRow,
            ToggleButton reviewGuideToggle,
            ToggleButton reviewDetailsToggle,
            PackIconMaterial reviewDetailsToggleIcon,
            TabItem matcherDiagnosticTab,
            TabItem objectInspectorTab,
            TabItem circleEvidenceTab,
            ToggleButton stepFlowToggle,
            ColumnDefinition stepFlowColumn,
            Border stepFlowPanel,
            TextBlock stepFlowLabel,
            FrameworkElement stepFlowFocusHost,
            FrameworkElement pipelineFlowView,
            PackIconMaterial stepFlowToggleIcon)
        {
            this.localize = localize ?? throw new ArgumentNullException(nameof(localize));
            this.reviewSummaryGrid = reviewSummaryGrid ?? throw new ArgumentNullException(nameof(reviewSummaryGrid));
            this.reviewSummaryRow = reviewSummaryRow ?? throw new ArgumentNullException(nameof(reviewSummaryRow));
            this.reviewSummaryGapRow = reviewSummaryGapRow ?? throw new ArgumentNullException(nameof(reviewSummaryGapRow));
            this.reviewDetailHost = reviewDetailHost ?? throw new ArgumentNullException(nameof(reviewDetailHost));
            this.reviewDetailTabs = reviewDetailTabs ?? throw new ArgumentNullException(nameof(reviewDetailTabs));
            this.reviewDetailSplitter = reviewDetailSplitter ?? throw new ArgumentNullException(nameof(reviewDetailSplitter));
            this.reviewDetailGapRow = reviewDetailGapRow ?? throw new ArgumentNullException(nameof(reviewDetailGapRow));
            this.reviewDetailRow = reviewDetailRow ?? throw new ArgumentNullException(nameof(reviewDetailRow));
            this.reviewGuideToggle = reviewGuideToggle ?? throw new ArgumentNullException(nameof(reviewGuideToggle));
            this.reviewDetailsToggle = reviewDetailsToggle ?? throw new ArgumentNullException(nameof(reviewDetailsToggle));
            this.reviewDetailsToggleIcon = reviewDetailsToggleIcon ?? throw new ArgumentNullException(nameof(reviewDetailsToggleIcon));
            this.matcherDiagnosticTab = matcherDiagnosticTab ?? throw new ArgumentNullException(nameof(matcherDiagnosticTab));
            this.objectInspectorTab = objectInspectorTab ?? throw new ArgumentNullException(nameof(objectInspectorTab));
            this.circleEvidenceTab = circleEvidenceTab ?? throw new ArgumentNullException(nameof(circleEvidenceTab));
            this.stepFlowToggle = stepFlowToggle ?? throw new ArgumentNullException(nameof(stepFlowToggle));
            this.stepFlowColumn = stepFlowColumn ?? throw new ArgumentNullException(nameof(stepFlowColumn));
            this.stepFlowPanel = stepFlowPanel ?? throw new ArgumentNullException(nameof(stepFlowPanel));
            this.stepFlowLabel = stepFlowLabel ?? throw new ArgumentNullException(nameof(stepFlowLabel));
            this.stepFlowFocusHost = stepFlowFocusHost ?? throw new ArgumentNullException(nameof(stepFlowFocusHost));
            this.pipelineFlowView = pipelineFlowView ?? throw new ArgumentNullException(nameof(pipelineFlowView));
            this.stepFlowToggleIcon = stepFlowToggleIcon ?? throw new ArgumentNullException(nameof(stepFlowToggleIcon));
        }

        internal void Initialize()
        {
            UpdateReviewDetailsToggleVisuals();
            UpdateStepFlowToggleVisuals();
            UpdateReviewDetailRowHeight();
        }

        internal void OnSizeChanged(double actualHeight)
        {
            bool compact = actualHeight < 650D;
            if (compact == useCompactImageLayout)
            {
                return;
            }

            useCompactImageLayout = compact;
            reviewSummaryGrid.Visibility = compact
                ? Visibility.Collapsed
                : Visibility.Visible;
            reviewSummaryRow.Height = compact
                ? new GridLength(0D)
                : GridLength.Auto;
            reviewSummaryGapRow.Height = new GridLength(compact ? 0D : 8D);
            UpdateReviewDetailRowHeight();
        }

        internal void OnReviewGuideToggleChanged()
        {
            UpdateReviewDetailRowHeight();
        }

        internal void OnReviewDetailsToggleChanged()
        {
            reviewDetailsExpanded = reviewDetailsToggle.IsChecked == true;
            UpdateReviewDetailsToggleVisuals();
            UpdateReviewDetailRowHeight();
        }

        internal void OnStepFlowToggleChanged()
        {
            stepFlowExpanded = stepFlowToggle.IsChecked == true;
            UpdateStepFlowLayout();
        }

        internal void UpdateReviewDetailsToggleVisuals()
        {
            reviewDetailsExpanded = reviewDetailsToggle.IsChecked == true;
            reviewDetailsToggleIcon.RenderTransform = new RotateTransform(
                reviewDetailsExpanded ? 180D : 0D);
            reviewDetailsToggle.ToolTip = reviewDetailsExpanded
                ? localize("PipelineReview.Details.HideToolTip", "Hide review details")
                : localize("PipelineReview.Details.ShowToolTip", "Show review details");
        }

        internal void UpdateStepFlowLayout()
        {
            stepFlowExpanded = stepFlowToggle.IsChecked == true;
            stepFlowColumn.Width = new GridLength(stepFlowExpanded ? 300D : 44D);
            stepFlowPanel.Padding = stepFlowExpanded
                ? new Thickness(10D)
                : new Thickness(4D);
            stepFlowLabel.Visibility = stepFlowExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;
            stepFlowFocusHost.Visibility = stepFlowExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;
            pipelineFlowView.Visibility = stepFlowExpanded
                ? Visibility.Visible
                : Visibility.Collapsed;
            stepFlowToggleIcon.Kind = stepFlowExpanded
                ? PackIconMaterialKind.ChevronLeft
                : PackIconMaterialKind.ChevronRight;
            stepFlowToggle.ToolTip = stepFlowExpanded
                ? localize("PipelineReview.StepFlow.HideToolTip", "Collapse Step Flow")
                : localize("PipelineReview.StepFlow.ShowToolTip", "Expand Step Flow");
        }

        internal void UpdateStepFlowToggleVisuals()
        {
            UpdateStepFlowLayout();
        }

        internal void UpdateReviewDetailRowHeight()
        {
            bool compactGuideExpanded = useCompactImageLayout && reviewGuideToggle.IsChecked == true;
            bool detailsVisible = reviewDetailsExpanded && !compactGuideExpanded;
            reviewDetailHost.Visibility = compactGuideExpanded
                ? Visibility.Collapsed
                : Visibility.Visible;
            reviewDetailTabs.Visibility = detailsVisible
                ? Visibility.Visible
                : Visibility.Collapsed;
            reviewDetailSplitter.Visibility = detailsVisible
                ? Visibility.Visible
                : Visibility.Collapsed;
            reviewDetailGapRow.Height = new GridLength(detailsVisible ? 8D : 4D);
            reviewDetailRow.MinHeight = compactGuideExpanded ? 0D : 34D;
            if (compactGuideExpanded)
            {
                reviewDetailRow.Height = new GridLength(0D);
                return;
            }

            if (!detailsVisible)
            {
                reviewDetailRow.Height = new GridLength(34D);
                return;
            }

            if (useCompactImageLayout)
            {
                double compactHeight = matcherDiagnosticTab.Visibility == Visibility.Visible
                    ? 240D
                    : circleEvidenceTab.Visibility == Visibility.Visible
                        ? 220D
                        : 220D;
                reviewDetailRow.Height = new GridLength(compactHeight);
                return;
            }

            double height = matcherDiagnosticTab.Visibility == Visibility.Visible
                ? 300D
                : objectInspectorTab.Visibility == Visibility.Visible
                    ? 240D
                : circleEvidenceTab.Visibility == Visibility.Visible
                    ? 280D
                    : 240D;
            reviewDetailRow.Height = new GridLength(height);
        }
    }
}
