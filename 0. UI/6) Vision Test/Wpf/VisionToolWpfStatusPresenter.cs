using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal static class VisionToolWpfStatusPresenter
    {
        public static void Apply(TextBlock textBlock, string status)
        {
            if (textBlock == null)
            {
                return;
            }

            string rawText = status ?? string.Empty;
            string displayText = LocalizeStatusText(rawText);
            textBlock.Text = displayText;
            textBlock.Foreground = ResolveBrush(textBlock, rawText, displayText);
            textBlock.FontWeight = string.IsNullOrWhiteSpace(rawText) ? FontWeights.Normal : FontWeights.SemiBold;
        }

        private static string LocalizeStatusText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = ReplaceLeadingStatus(text, "Preview OK", "VisionTool.Status.PreviewOk");
            text = ReplaceLeadingStatus(text, "Preview NG", "VisionTool.Status.PreviewNg");
            text = ReplaceLeadingStatus(text, "Offset OK", "VisionTool.Status.OffsetOk");
            text = ReplaceLeadingStatus(text, "Pipeline added", "VisionTool.Status.PipelineAdded");
            text = ReplaceLeadingStatus(text, "Output layer ready", "VisionTool.Status.OutputLayerReady");
            text = ReplaceLeadingStatus(text, "Pipeline add unavailable", "VisionTool.Status.PipelineAddUnavailable");
            return text;
        }

        private static string ReplaceLeadingStatus(string text, string prefix, string localizationKey)
        {
            if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            string localizedPrefix = OpenVisionLanguageService.T(localizationKey);
            if (string.IsNullOrWhiteSpace(localizedPrefix) || string.Equals(localizedPrefix, localizationKey, StringComparison.Ordinal))
            {
                localizedPrefix = prefix;
            }

            return localizedPrefix + text.Substring(prefix.Length);
        }

        private static Brush ResolveBrush(FrameworkElement element, string rawText, string displayText)
        {
            string raw = rawText ?? string.Empty;
            string combined = raw + " " + (displayText ?? string.Empty);
            if (string.IsNullOrWhiteSpace(combined))
            {
                return FindBrush(element, "VisionTool.SecondaryTextBrush");
            }

            if (StartsWithAny(raw, "Preview NG", "Pipeline add unavailable")
                || ContainsAny(combined, " error", "failed", " fail ", "unavailable"))
            {
                return FindBrush(element, "VisionTool.StatusErrorBrush");
            }

            if (StartsWithAny(raw, "Preview OK", "Offset OK", "Pipeline added", "Output layer ready")
                || ContainsAny(combined, "ok", "passed", "success", "ready", "added"))
            {
                return FindBrush(element, "VisionTool.StatusSuccessBrush");
            }

            if (ContainsAny(combined, "warn", "check", "review"))
            {
                return FindBrush(element, "VisionTool.StatusWarningBrush");
            }

            return FindBrush(element, "VisionTool.SecondaryTextBrush");
        }

        private static bool StartsWithAny(string text, params string[] prefixes)
        {
            foreach (string prefix in prefixes)
            {
                if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsAny(string text, params string[] tokens)
        {
            foreach (string token in tokens)
            {
                if (text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static Brush FindBrush(FrameworkElement element, string resourceKey)
        {
            return element.TryFindResource(resourceKey) as Brush ?? Brushes.DimGray;
        }
    }
}
