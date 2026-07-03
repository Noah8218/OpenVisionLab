using System;
using System.Text.RegularExpressions;
using MahApps.Metro.IconPacks;

namespace OpenVisionLab
{
    internal sealed class SimplePreprocessTextPresenter
    {
        private readonly Action<string> setTitleText;
        private readonly Action<PackIconMaterialKind> setTitleIconKind;
        private readonly Action<string> setSummaryText;
        private string headerLocalizationKey;
        private string headerFallbackText = "Preprocess";
        private string rawSummaryText = string.Empty;

        public SimplePreprocessTextPresenter(
            Action<string> setTitleText,
            Action<PackIconMaterialKind> setTitleIconKind,
            Action<string> setSummaryText)
        {
            this.setTitleText = setTitleText ?? throw new ArgumentNullException(nameof(setTitleText));
            this.setTitleIconKind = setTitleIconKind ?? throw new ArgumentNullException(nameof(setTitleIconKind));
            this.setSummaryText = setSummaryText ?? throw new ArgumentNullException(nameof(setSummaryText));
        }

        public void SetHeader(string title, PackIconMaterialKind iconKind)
        {
            headerLocalizationKey = null;
            headerFallbackText = title ?? string.Empty;
            setTitleIconKind(iconKind);
            ApplyHeader();
        }

        public void SetLocalizedHeader(string localizationKey, string fallbackTitle, PackIconMaterialKind iconKind)
        {
            headerLocalizationKey = localizationKey;
            headerFallbackText = fallbackTitle ?? string.Empty;
            setTitleIconKind(iconKind);
            ApplyHeader();
        }

        public void SetSummary(string summary)
        {
            rawSummaryText = summary ?? string.Empty;
            RefreshSummary();
        }

        public void ApplyLocalization()
        {
            ApplyHeader();
            RefreshSummary();
        }

        public void RefreshSummary()
        {
            // Summary localization is text presentation state; keep it centralized instead of scattering word replacements through tool views.
            setSummaryText(LocalizeSummaryText(rawSummaryText));
        }

        private void ApplyHeader()
        {
            setTitleText(ResolveText(headerLocalizationKey, headerFallbackText));
        }

        private static string LocalizeSummaryText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = ReplaceSummaryWord(text, "Min", "VisionTool.Summary.Min");
            text = ReplaceSummaryWord(text, "Max", "VisionTool.Summary.Max");
            text = ReplaceSummaryWord(text, "Low", "VisionTool.Summary.Low");
            text = ReplaceSummaryWord(text, "High", "VisionTool.Summary.High");
            text = ReplaceSummaryWord(text, "Aperture", "VisionTool.Summary.Aperture");
            text = ReplaceSummaryWord(text, "Clip", "VisionTool.Summary.Clip");
            text = ReplaceSummaryWord(text, "Tile", "VisionTool.Summary.Tile");
            text = ReplaceSummaryWord(text, "Angle", "VisionTool.Summary.Angle");
            text = ReplaceSummaryWord(text, "Scale", "VisionTool.Summary.Scale");
            return text;
        }

        private static string ReplaceSummaryWord(string text, string word, string localizationKey)
        {
            string localizedWord = ResolveText(localizationKey, word);
            return Regex.Replace(
                text,
                $@"\b{Regex.Escape(word)}\b",
                localizedWord,
                RegexOptions.CultureInvariant);
        }

        private static string ResolveText(string localizationKey, string fallbackText)
        {
            if (string.IsNullOrWhiteSpace(localizationKey))
            {
                return fallbackText ?? string.Empty;
            }

            string value = OpenVisionLanguageService.T(localizationKey);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, localizationKey, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }
    }
}