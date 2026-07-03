using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolRailPresenter
    {
        public void Apply(
            bool compact,
            ColumnDefinition toolRailColumn,
            Border toolRailBorder,
            Button toggleButton,
            ScrollViewer toolRailScroll,
            StackPanel toggleContent,
            PackIconMaterial toggleIcon,
            TextBlock toggleText)
        {
            if (toolRailColumn != null)
            {
                toolRailColumn.Width = compact ? new GridLength(52) : new GridLength(220);
            }

            if (toolRailBorder != null)
            {
                toolRailBorder.Padding = compact ? new Thickness(8, 10, 8, 10) : new Thickness(10, 12, 10, 10);
            }

            if (toggleButton != null)
            {
                toggleButton.Height = compact ? 36D : 30D;
                toggleButton.Padding = compact ? new Thickness(0) : new Thickness(8, 0, 8, 0);
                toggleButton.Margin = new Thickness(0, 0, 0, 8);
                toggleButton.ToolTip = compact
                    ? OpenVisionLanguageService.T("Shell.ToolRailExpand")
                    : OpenVisionLanguageService.T("Shell.ToolRailCollapse");
            }

            if (toolRailScroll != null)
            {
                toolRailScroll.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            }

            if (toggleContent != null)
            {
                toggleContent.Orientation = Orientation.Horizontal;
            }

            if (toggleIcon != null)
            {
                toggleIcon.Kind = compact
                    ? PackIconMaterialKind.ChevronRight
                    : PackIconMaterialKind.ChevronLeft;
                toggleIcon.Margin = compact ? new Thickness(0) : new Thickness(0, 0, 6, 0);
            }

            if (toggleText != null)
            {
                toggleText.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
                toggleText.Text = OpenVisionLanguageService.T("Shell.ToolRailCollapse");
            }
        }
    }
}
