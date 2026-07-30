using System;
using System.Windows;
using System.Windows.Controls;
using WpgPropertyGrid = System.Windows.Controls.WpfPropertyGrid.PropertyGrid;

namespace OpenVisionLab
{
    public partial class VisionToolSingleInputPropertyToolShell
    {
        private sealed class DockedInspectorLayoutController
        {
            private const double DockedPreviewFrameHeight = 60D;
            private const double DockedPreviewCardMinHeight = 92D;
            private const double FloatingPreviewCardPadding = 8D;
            private const double DockedPreviewCardPadding = 6D;
            private const double FloatingParameterGroupMinHeight = 400D;
            private const double DockedParameterGroupMinHeight = 0D;
            private const double FloatingPropertyGridMinHeight = 370D;
            private const double DockedPropertyGridMinHeight = 0D;
            private const double FloatingResultReviewMinHeight = 48D;
            private const double DockedResultReviewMinHeight = 34D;
            private const double DockedResultReviewMaxHeight = 64D;
            private const double DockedResultReviewChipsMaxHeight = 28D;
            private const double DockedSummaryMinHeight = 32D;
            private const double DockedStatusMinHeight = 28D;
            private const double ActionHeight = 36D;
            private const double ActionGap = 4D;

            private readonly VisionToolSingleInputPropertyToolShell shell;

            public DockedInspectorLayoutController(VisionToolSingleInputPropertyToolShell shell)
            {
                this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            }

            public void Apply()
            {
                bool docked = shell.IsDockedInspectorMode;

                shell.MinWidth = docked ? 0D : 900D;
                shell.MinHeight = docked ? 0D : 620D;
                shell.shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
                shell.previewColumn.Width = docked ? new GridLength(1D, GridUnitType.Star) : new GridLength(390D);
                shell.flowColumn.Width = docked ? new GridLength(8D) : new GridLength(16D);
                shell.parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
                shell.flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
                Thickness previewPadding = new Thickness(docked ? DockedPreviewCardPadding : FloatingPreviewCardPadding);
                shell.gbInputLayer.Padding = previewPadding;
                shell.gbOutputLayer.Padding = previewPadding;

                shell.previewInputRow.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.previewSpacerRow.Height = docked ? new GridLength(2D) : new GridLength(10D);
                shell.previewOutputRow.Height = new GridLength(1D, GridUnitType.Star);

                shell.inputPreviewRow.Height = docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
                shell.inputPreviewSpacerRow.Height = docked ? new GridLength(4D) : new GridLength(8D);
                shell.outputPreviewRow.Height = docked ? new GridLength(DockedPreviewFrameHeight) : new GridLength(1D, GridUnitType.Star);
                shell.outputPreviewSpacerRow.Height = docked ? new GridLength(4D) : new GridLength(8D);
                shell.bdInputPreview.Visibility = Visibility.Visible;
                shell.bdOutputPreview.Visibility = Visibility.Visible;

                Grid.SetRow(shell.gbInputLayer, 0);
                Grid.SetColumn(shell.gbInputLayer, 0);
                Grid.SetRowSpan(shell.gbInputLayer, 1);
                Grid.SetColumnSpan(shell.gbInputLayer, 1);

                Grid.SetRow(shell.gbOutputLayer, docked ? 0 : 2);
                Grid.SetColumn(shell.gbOutputLayer, docked ? 2 : 0);
                Grid.SetRowSpan(shell.gbOutputLayer, 1);
                Grid.SetColumnSpan(shell.gbOutputLayer, 1);

                Grid.SetRow(shell.parameterPanel, docked ? 2 : 0);
                Grid.SetColumn(shell.parameterPanel, docked ? 0 : 2);
                Grid.SetRowSpan(shell.parameterPanel, docked ? 1 : 3);
                Grid.SetColumnSpan(shell.parameterPanel, docked ? 3 : 1);

                shell.titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
                shell.titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(6D);
                shell.summaryGapRow.Height = docked ? new GridLength(4D) : new GridLength(6D);
                shell.actionGapRow.Height = docked ? new GridLength(2D) : new GridLength(6D);

                shell.gbInputLayer.MinHeight = docked ? DockedPreviewCardMinHeight : 0D;
                shell.gbOutputLayer.MinHeight = docked ? DockedPreviewCardMinHeight : 0D;
                shell.gbParameters.MinHeight = docked ? DockedParameterGroupMinHeight : FloatingParameterGroupMinHeight;
                shell.propertyGridHost.MinHeight = docked ? DockedPropertyGridMinHeight : FloatingPropertyGridMinHeight;
                shell.parameterGuideView.SetCompactMode(docked);
                ApplyPresetDensity(docked);
                ApplySummaryStatusDensity(docked);
                ApplyResultReviewDensity(docked);
                ApplyPropertyGridDensity(docked);
                ApplyToolContentDensity();
                shell.rowAddPipelineAction.Height = new GridLength(ActionHeight);
                shell.rowRunPreviewGap.Height = new GridLength(docked ? 2D : ActionGap);
                shell.rowRunPreviewAction.Height = new GridLength(ActionHeight);
            }

            private void ApplyPresetDensity(bool docked)
            {
                shell.bdPresetHost.Padding = docked ? new Thickness(7, 4, 7, 4) : new Thickness(10, 8, 10, 8);
                shell.txtPresetDetail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
                shell.txtPresetDetail.FontSize = docked ? 10D : 11D;
                shell.btnPresetBasic.Height = docked ? 24D : 28D;
                shell.btnPresetFast.Height = docked ? 24D : 28D;
                shell.btnPresetPrecise.Height = docked ? 24D : 28D;
            }

            private void ApplySummaryStatusDensity(bool docked)
            {
                shell.bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
                shell.bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
                shell.txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

                shell.bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
                shell.bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 6, 0, 0);
                shell.bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
                shell.txtStatus.MinHeight = docked ? 14D : 18D;
                shell.txtStatus.TextTrimming = TextTrimming.CharacterEllipsis;
            }

            private void ApplyResultReviewDensity(bool docked)
            {
                shell.bdResultReview.MinHeight = docked ? DockedResultReviewMinHeight : FloatingResultReviewMinHeight;
                shell.bdResultReview.MaxHeight = docked ? DockedResultReviewMaxHeight : double.PositiveInfinity;
                shell.svResultReviewChips.MaxHeight = docked ? DockedResultReviewChipsMaxHeight : double.PositiveInfinity;
                shell.svResultReviewChips.Margin = docked ? new Thickness(0, 3, 0, 0) : new Thickness(0, 4, 0, 0);
                shell.svResultReviewChips.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;
                shell.svResultReviewChips.VerticalScrollBarVisibility = docked
                    ? ScrollBarVisibility.Auto
                    : ScrollBarVisibility.Disabled;
                shell.txtResultGuidance.FontSize = docked ? 10D : 11D;
                shell.txtResultGuidance.MaxHeight = double.PositiveInfinity;
            }

            private void ApplyPropertyGridDensity(bool compactDensity)
            {
                if (shell.propertyGridHost.Child is WpgPropertyGrid grid)
                {
                    grid.IsCompactDensity = compactDensity;
                }
            }

            private void ApplyToolContentDensity()
            {
                if (shell.ToolContent is VisionToolVerificationGuideView guideView)
                {
                    guideView.IsCompactMode = true;
                }
            }
        }
    }
}
