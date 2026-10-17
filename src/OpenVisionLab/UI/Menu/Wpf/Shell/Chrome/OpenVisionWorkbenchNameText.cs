using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenVisionLab
{
    // Only the Main tool title and managed-layer name opt in to space-delimited wrapping.
    // Text stays unchanged for binding and automation; an over-wide word still wraps.
    public sealed class OpenVisionWorkbenchNameText : Control
    {
        public static readonly DependencyProperty TextProperty = TextBlock.TextProperty.AddOwner(typeof(OpenVisionWorkbenchNameText),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(OpenVisionWorkbenchNameText),
            new FrameworkPropertyMetadata(TextWrapping.Wrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TextTrimmingProperty = TextBlock.TextTrimmingProperty.AddOwner(typeof(OpenVisionWorkbenchNameText),
            new FrameworkPropertyMetadata(TextTrimming.None));

        static OpenVisionWorkbenchNameText()
        {
            FocusableProperty.OverrideMetadata(typeof(OpenVisionWorkbenchNameText), new FrameworkPropertyMetadata(false));
            IsTabStopProperty.OverrideMetadata(typeof(OpenVisionWorkbenchNameText), new FrameworkPropertyMetadata(false));
        }

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }
        public TextTrimming TextTrimming { get => (TextTrimming)GetValue(TextTrimmingProperty); set => SetValue(TextTrimmingProperty, value); }

        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0D;
            double height = 0D;
            foreach (string line in WrapLines(constraint.Width))
            {
                FormattedText formatted = Format(line);
                width = Math.Max(width, formatted.Width);
                height += formatted.Height;
            }
            return new Size(Math.Min(width, constraint.Width), height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Preserve TextBlock's full hit target between lines and after the last glyph.
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
            double top = 0D;
            foreach (string line in WrapLines(RenderSize.Width))
            {
                FormattedText formatted = Format(line);
                drawingContext.DrawText(formatted, new Point(FlowDirection == FlowDirection.RightToLeft ? RenderSize.Width : 0D, top));
                top += formatted.Height;
            }
        }

        private FormattedText Format(string text) => new FormattedText(text.Length == 0 ? " " : text, Language.GetSpecificCulture(), FlowDirection,
            new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground,
            null, TextOptions.GetTextFormattingMode(this), VisualTreeHelper.GetDpi(this).PixelsPerDip);

        private IEnumerable<string> WrapLines(double availableWidth)
        {
            string text = Text ?? string.Empty;
            double width = Math.Max(1D, availableWidth);
            for (int paragraphStart = 0; paragraphStart <= text.Length;)
            {
                int paragraphEnd = paragraphStart;
                while (paragraphEnd < text.Length && text[paragraphEnd] != '\r' && text[paragraphEnd] != '\n') { paragraphEnd++; }
                string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);
                int[] characters = StringInfo.ParseCombiningCharacters(paragraph);
                if (characters.Length == 0) { yield return string.Empty; }
                for (int first = 0; first < characters.Length;)
                {
                    int low = first + 1;
                    int high = characters.Length;
                    int fit = low;
                    while (low <= high)
                    {
                        int middle = low + (high - low) / 2;
                        int end = middle == characters.Length ? paragraph.Length : characters[middle];
                        if (TextWrapping == TextWrapping.NoWrap || Format(paragraph.Substring(characters[first], end - characters[first])).Width <= width)
                        {
                            fit = middle;
                            low = middle + 1;
                        }
                        else { high = middle - 1; }
                    }
                    int breakAt = fit == characters.Length ? paragraph.Length : characters[fit];
                    if (fit < characters.Length && !char.IsWhiteSpace(paragraph[breakAt]) && !char.IsWhiteSpace(paragraph[breakAt - 1]))
                    {
                        for (int candidate = fit - 1; candidate > first; candidate--)
                        {
                            if (char.IsWhiteSpace(paragraph[characters[candidate] - 1])) { fit = candidate; break; }
                        }
                    }
                    breakAt = fit == characters.Length ? paragraph.Length : characters[fit];
                    yield return paragraph.Substring(characters[first], breakAt - characters[first]);
                    first = fit;
                }
                if (paragraphEnd == text.Length) { break; }
                paragraphStart = paragraphEnd + (text[paragraphEnd] == '\r' && paragraphEnd + 1 < text.Length && text[paragraphEnd + 1] == '\n' ? 2 : 1);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer() => new NameTextAutomationPeer(this);

        private sealed class NameTextAutomationPeer : FrameworkElementAutomationPeer
        {
            public NameTextAutomationPeer(OpenVisionWorkbenchNameText owner) : base(owner) { }
            protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;
            protected override string GetClassNameCore() => "TextBlock";
            protected override string GetNameCore()
            {
                string explicitName = base.GetNameCore();
                return string.IsNullOrEmpty(explicitName) ? ((OpenVisionWorkbenchNameText)Owner).Text ?? string.Empty : explicitName;
            }
        }
    }
}
