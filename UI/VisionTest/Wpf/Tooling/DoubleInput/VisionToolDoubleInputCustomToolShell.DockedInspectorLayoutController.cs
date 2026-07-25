using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class VisionToolDoubleInputCustomToolShell
    {
        private sealed class DockedInspectorLayoutController
        {
            private const double DockedPreviewCardHeight = 132D;
            private const double FloatingActionHeight = 40D;
            private const double DockedActionHeight = 36D;
            private const double FloatingActionGap = 8D;
            private const double DockedActionGap = 4D;
            private const double DockedSummaryMinHeight = 32D;
            private const double DockedStatusMinHeight = 28D;

            private readonly VisionToolDoubleInputCustomToolShell shell;

            public DockedInspectorLayoutController(VisionToolDoubleInputCustomToolShell shell)
            {
                this.shell = shell ?? throw new ArgumentNullException(nameof(shell));
            }

            public void Apply()
            {
                bool docked = shell.IsDockedInspectorMode;
                bool inputBVisible = shell.gbInputB.Visibility == Visibility.Visible;

                shell.MinWidth = docked ? 0D : 920D;
                shell.MinHeight = docked ? 0D : 620D;
                shell.shellRoot.Margin = docked ? new Thickness(8) : new Thickness(14);
                shell.previewColumn.Width = docked ? new GridLength(180D) : new GridLength(390D);
                shell.flowColumn.Width = docked ? new GridLength(0D) : new GridLength(16D);
                shell.parameterColumn.Width = new GridLength(1D, GridUnitType.Star);
                shell.flowRail.Visibility = docked ? Visibility.Collapsed : Visibility.Visible;

                shell.rowInputA.Height = docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star);
                shell.rowInputBGap.Height = inputBVisible ? new GridLength(docked ? 6D : 8D) : new GridLength(0D);
                shell.rowInputB.Height = inputBVisible
                    ? docked ? GridLength.Auto : new GridLength(1D, GridUnitType.Star)
                    : new GridLength(0D);
                shell.rowOutputGap.Height = docked ? new GridLength(6D) : new GridLength(8D);
                shell.rowOutput.Height = new GridLength(1D, GridUnitType.Star);

                shell.titleRow.Height = docked ? new GridLength(0D) : GridLength.Auto;
                shell.titleGapRow.Height = docked ? new GridLength(0D) : new GridLength(14D);
                shell.summaryGapRow.Height = docked ? new GridLength(8D) : new GridLength(12D);
                shell.actionGapRow.Height = docked ? new GridLength(8D) : new GridLength(14D);

                ApplyPreviewCardDocking(shell.gbInputA, docked, visible: true);
                ApplyPreviewCardDocking(shell.gbInputB, docked, inputBVisible);
                ApplyPreviewCardDocking(shell.gbOutputLayer, docked, visible: true);
                ApplySummaryStatusDensity(docked);
                ApplyActionRows(IsOffsetActionActive());
            }

            public void SetInputBPreviewVisible(bool visible)
            {
                shell.gbInputB.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                shell.gbInputB.IsEnabled = visible;
                shell.rowInputB.Height = visible
                    ? shell.IsDockedInspectorMode ? GridLength.Auto : new GridLength(1, GridUnitType.Star)
                    : new GridLength(0);
                shell.rowInputBGap.Height = visible ? new GridLength(shell.IsDockedInspectorMode ? 6D : 8D) : new GridLength(0);
                ApplyPreviewCardDocking(shell.gbInputB, shell.IsDockedInspectorMode, visible);
            }

            public void SetOffsetActionsVisible(bool useOffsetMode)
            {
                shell.btnRunPreview.Visibility = useOffsetMode ? Visibility.Collapsed : Visibility.Visible;
                shell.btnRunOffset.Visibility = useOffsetMode ? Visibility.Visible : Visibility.Collapsed;
                ApplyActionRows(useOffsetMode);
            }

            private void ApplySummaryStatusDensity(bool docked)
            {
                shell.bdSummary.MinHeight = docked ? DockedSummaryMinHeight : 0D;
                shell.bdSummary.Padding = docked ? new Thickness(9, 5, 9, 5) : new Thickness(12, 8, 12, 8);
                shell.txtSummary.TextTrimming = TextTrimming.CharacterEllipsis;

                shell.bdStatus.MinHeight = docked ? DockedStatusMinHeight : 0D;
                shell.bdStatus.Margin = docked ? new Thickness(0, 4, 0, 0) : new Thickness(0, 8, 0, 0);
                shell.bdStatus.Padding = docked ? new Thickness(8, 3, 8, 3) : new Thickness(8, 4, 8, 4);
                shell.txtStatus.MinHeight = docked ? 14D : 18D;
                shell.txtStatus.TextTrimming = TextTrimming.CharacterEllipsis;
            }

            private static void ApplyPreviewCardDocking(HeaderedContentControl group, bool docked, bool visible)
            {
                if (group == null)
                {
                    return;
                }

                group.VerticalAlignment = docked ? VerticalAlignment.Top : VerticalAlignment.Stretch;
                group.Height = docked && visible ? DockedPreviewCardHeight : double.NaN;
                group.MinHeight = docked && visible ? DockedPreviewCardHeight : 0D;
            }

            private bool IsOffsetActionActive()
            {
                return shell.btnRunOffset.Visibility == Visibility.Visible
                    && shell.rowRunOffsetAction.Height.Value > 0D;
            }

            private void ApplyActionRows(bool useOffsetMode)
            {
                double actionHeight = shell.IsDockedInspectorMode ? DockedActionHeight : FloatingActionHeight;
                double actionGap = shell.IsDockedInspectorMode ? DockedActionGap : FloatingActionGap;

                shell.rowAddPipelineAction.Height = new GridLength(actionHeight);
                shell.rowRunPreviewGap.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionGap);
                shell.rowRunPreviewAction.Height = useOffsetMode ? new GridLength(0) : new GridLength(actionHeight);
                shell.rowRunOffsetGap.Height = useOffsetMode ? new GridLength(actionGap) : new GridLength(0);
                shell.rowRunOffsetAction.Height = useOffsetMode ? new GridLength(actionHeight) : new GridLength(0);
            }
        }
    }
}
