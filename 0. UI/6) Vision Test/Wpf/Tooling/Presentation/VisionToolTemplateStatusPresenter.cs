using OpenVisionLab.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal static class VisionToolTemplateStatusPresenter
    {
        public static void Apply(
            TextBlock statusText,
            Control statusIcon,
            VisionToolTemplateStatus status)
        {
            ApplyCore(statusText, statusText, statusIcon, status);
        }

        public static void Apply(
            FrameworkElement owner,
            TextBlock statusText,
            Control statusIcon,
            VisionToolTemplateStatus status)
        {
            ApplyCore(owner, statusText, statusIcon, status);
        }

        private static void ApplyCore(
            FrameworkElement owner,
            TextBlock statusText,
            Control statusIcon,
            VisionToolTemplateStatus status)
        {
            if (statusText == null)
            {
                return;
            }

            VisionToolTemplateStatus resolvedStatus = status ?? new VisionToolTemplateStatus(string.Empty, false);
            Brush brush = ResolveStatusBrush(owner, resolvedStatus.IsReady);
            statusText.Text = CreateDisplayText(resolvedStatus);
            statusText.Foreground = brush;
            if (statusIcon != null)
            {
                statusIcon.Foreground = brush;
            }
        }

        private static string CreateDisplayText(VisionToolTemplateStatus status)
        {
            if (status == null)
            {
                return VisionToolVerificationText.TemplateNotSelectedStatus;
            }

            string raw = status.Text ?? string.Empty;
            if (status.IsReady)
            {
                string detail = raw.StartsWith("Template ready / ", System.StringComparison.OrdinalIgnoreCase)
                    ? raw.Substring("Template ready / ".Length)
                    : raw;
                return VisionToolVerificationText.FormatTemplateReadyStatus(detail);
            }

            if (raw.StartsWith("Template file missing / ", System.StringComparison.OrdinalIgnoreCase))
            {
                return VisionToolVerificationText.FormatTemplateMissingStatus(raw.Substring("Template file missing / ".Length));
            }

            return VisionToolVerificationText.TemplateNotSelectedStatus;
        }

        private static Brush ResolveStatusBrush(FrameworkElement owner, bool isReady)
        {
            string key = isReady
                ? "VisionTool.StatusSuccessBrush"
                : "VisionTool.SecondaryTextBrush";
            Brush fallback = isReady ? Brushes.SeaGreen : Brushes.DimGray;
            return owner?.TryFindResource(key) as Brush ?? fallback;
        }
    }
}
