using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostMainActionPresenter
    {
        private readonly UIElement overlay;
        private readonly TextBlock titleText;
        private readonly TextBlock detailText;
        private readonly TextBlock metaText;
        private readonly TextBlock thresholdButtonText;
        private readonly TextBlock matchingButtonText;
        private readonly TextBlock lineButtonText;
        private string currentLayerTitle;
        private string currentLayerMeta;

        public OpenVisionShellHostMainActionPresenter(
            UIElement overlay,
            TextBlock titleText,
            TextBlock detailText,
            TextBlock metaText,
            TextBlock thresholdButtonText,
            TextBlock matchingButtonText,
            TextBlock lineButtonText)
        {
            this.overlay = overlay;
            this.titleText = titleText;
            this.detailText = detailText;
            this.metaText = metaText;
            this.thresholdButtonText = thresholdButtonText;
            this.matchingButtonText = matchingButtonText;
            this.lineButtonText = lineButtonText;
        }

        public bool IsVisible => overlay?.Visibility == Visibility.Visible;

        public string Title => titleText?.Text ?? string.Empty;

        public string Detail => detailText?.Text ?? string.Empty;

        public string Meta => metaText?.Text ?? string.Empty;

        public void ApplyLocalization()
        {
            SetText(thresholdButtonText, T("VisionMenu.Threshold", "Threshold"));
            SetText(matchingButtonText, T("VisionMenu.Matching", "Matching"));
            SetText(lineButtonText, T("VisionMenu.Line", "Line"));

            if (IsVisible)
            {
                ApplyImageReadyText();
            }
        }

        public void ShowImageReady(string layerTitle, string layerMeta)
        {
            currentLayerTitle = layerTitle;
            currentLayerMeta = layerMeta;
            ApplyLocalization();
            ApplyImageReadyText();

            if (overlay != null)
            {
                overlay.Visibility = Visibility.Visible;
                AutomationProperties.SetName(overlay, Title + " " + Detail);
            }
        }

        public void Hide()
        {
            if (overlay != null)
            {
                overlay.Visibility = Visibility.Collapsed;
                AutomationProperties.SetName(overlay, string.Empty);
            }
        }

        private void ApplyImageReadyText()
        {
            SetText(titleText, T("Shell.MainAction.ImageReadyTitle", "\uC774\uBBF8\uC9C0 \uC900\uBE44\uB428"));
            SetText(detailText, T("Shell.MainAction.ImageReadyDetail", "\uB2E4\uC74C: \uB3C4\uAD6C \uC120\uD0DD -> \uBBF8\uB9AC\uBCF4\uAE30 \uD655\uC778 -> \uD30C\uC774\uD504\uB77C\uC778 \uCD94\uAC00"));
            SetText(metaText, CreateMeta(currentLayerTitle, currentLayerMeta));
            if (overlay != null)
            {
                AutomationProperties.SetName(overlay, Title + " " + Detail);
            }
        }

        private static string CreateMeta(string layerTitle, string layerMeta)
        {
            string title = string.IsNullOrWhiteSpace(layerTitle) ? "Main" : layerTitle.Trim();
            string meta = string.IsNullOrWhiteSpace(layerMeta) || string.Equals(layerMeta.Trim(), "-", StringComparison.Ordinal)
                ? string.Empty
                : " / " + layerMeta.Trim();
            return title + meta;
        }

        private static void SetText(TextBlock textBlock, string text)
        {
            if (textBlock != null)
            {
                textBlock.Text = text ?? string.Empty;
            }
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText
                : value;
        }
    }
}
