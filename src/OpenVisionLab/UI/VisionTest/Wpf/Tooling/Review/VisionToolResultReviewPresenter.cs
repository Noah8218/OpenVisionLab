using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal readonly struct VisionToolResultReviewItem
    {
        public VisionToolResultReviewItem(string label, string value)
        {
            Label = label ?? string.Empty;
            Value = value ?? string.Empty;
        }

        public string Label { get; }
        public string Value { get; }
    }

    internal static class VisionToolResultReviewPresenter
    {
        public static void Show(
            FrameworkElement owner,
            TextBlock summaryText,
            Panel chipPanel,
            string summary,
            bool isSuccess,
            IEnumerable<VisionToolResultReviewItem> items)
        {
            if (summaryText == null || chipPanel == null)
            {
                return;
            }

            summaryText.Text = string.IsNullOrWhiteSpace(summary) ? "-" : summary.Trim();
            summaryText.Foreground = ResolveStatusBrush(owner, isSuccess);
            summaryText.ToolTip = summaryText.Text;

            chipPanel.Children.Clear();
            foreach (VisionToolResultReviewItem item in items ?? Array.Empty<VisionToolResultReviewItem>())
            {
                if (string.IsNullOrWhiteSpace(item.Label) && string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }

                chipPanel.Children.Add(CreateChip(owner, item, isSuccess));
            }
        }

        public static void Clear(FrameworkElement owner, TextBlock summaryText, Panel chipPanel)
        {
            Show(owner, summaryText, chipPanel, VisionToolVerificationText.ResultNotRun, false, Array.Empty<VisionToolResultReviewItem>());
        }

        public static VisionToolResultReviewItem Item(string label, object value)
        {
            return new VisionToolResultReviewItem(label, Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty);
        }

        public static string FormatPoint(double x, double y)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:0.#}, {1:0.#}", x, y);
        }

        public static string FormatSize(double width, double height)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:0.#} x {1:0.#}", width, height);
        }

        private static Border CreateChip(FrameworkElement owner, VisionToolResultReviewItem item, bool isSuccess)
        {
            Brush accent = ResolveStatusBrush(owner, isSuccess);
            Brush background = FindBrush(owner, isSuccess ? "VisionTool.SelectionBrush" : "VisionTool.HeaderBrush", Brushes.WhiteSmoke);
            Brush border = FindBrush(owner, "VisionTool.HeaderBorderBrush", Brushes.LightGray);
            Brush secondary = FindBrush(owner, "VisionTool.SecondaryTextBrush", Brushes.DimGray);

            StackPanel content = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            content.Children.Add(new TextBlock
            {
                Text = item.Label,
                Foreground = secondary,
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                TextTrimming = TextTrimming.CharacterEllipsis
            });
            content.Children.Add(new TextBlock
            {
                Text = item.Value,
                Foreground = accent,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 2, 0, 0)
            });

            return new Border
            {
                Background = background,
                BorderBrush = border,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 5, 8, 5),
                Margin = new Thickness(0, 0, 6, 6),
                MinWidth = 82,
                Child = content,
                ToolTip = item.Label + ": " + item.Value
            };
        }

        internal static Brush ResolveStatusBrush(FrameworkElement owner, bool isSuccess)
        {
            return FindBrush(
                owner,
                isSuccess ? "VisionTool.StatusSuccessBrush" : "VisionTool.StatusWarningBrush",
                isSuccess ? Brushes.SeaGreen : Brushes.DarkGoldenrod);
        }

        private static Brush FindBrush(FrameworkElement owner, string key, Brush fallback)
        {
            return owner?.TryFindResource(key) as Brush ?? fallback;
        }
    }
}
