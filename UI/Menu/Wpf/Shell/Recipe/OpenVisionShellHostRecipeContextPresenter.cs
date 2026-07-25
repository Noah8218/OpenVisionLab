using System;
using System.Globalization;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostRecipeContextPresenter
    {
        private readonly TextBlock labelText;
        private readonly TextBlock valueText;
        private readonly Func<OpenVisionRecipeContext> contextProvider;

        public OpenVisionShellHostRecipeContextPresenter(
            TextBlock labelText,
            TextBlock valueText,
            Func<OpenVisionRecipeContext> contextProvider)
        {
            this.labelText = labelText ?? throw new ArgumentNullException(nameof(labelText));
            this.valueText = valueText ?? throw new ArgumentNullException(nameof(valueText));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void Refresh()
        {
            OpenVisionRecipeContext context = contextProvider();
            labelText.Text = T("RecipeContext.Label", LocalText("레시피", "Recipe"));
            valueText.Text = FormatScopeText(context);
            valueText.ToolTip = string.Format(
                CultureInfo.CurrentCulture,
                "{0}: {1}\n{2}: {3}\n{4}: {5}\n{6}",
                T("RecipeContext.Recipe", LocalText("레시피", "Recipe")),
                context.Name,
                T("RecipeContext.Pipeline", LocalText("파이프라인", "Pipeline")),
                context.PipelineName,
                T("RecipeContext.ActiveLayer", LocalText("활성 레이어", "Active Layer")),
                context.ActiveLayerName,
                T(
                    "RecipeContext.ScopeHint",
                    LocalText(
                        "Add Pipeline은 이 레시피/파이프라인에만 저장되며 Preview는 수동 실행입니다.",
                        "Add Pipeline writes only to this recipe/pipeline; Preview runs manually.")));
        }

        private static string FormatScopeText(OpenVisionRecipeContext context)
        {
            string dirtySuffix = context.IsDirty ? " *" : string.Empty;
            return string.Format(
                CultureInfo.CurrentCulture,
                T("RecipeContext.ScopeFormat", LocalText("범위: {0}{1}", "Scope: {0}{1}")),
                context.PipelineName,
                dirtySuffix);
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText
                : value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                ? korean
                : english;
        }
    }
}
