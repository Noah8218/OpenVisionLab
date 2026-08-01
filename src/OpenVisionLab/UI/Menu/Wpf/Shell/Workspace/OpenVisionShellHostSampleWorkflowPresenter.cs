using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostSampleWorkflowPresenter
    {
        private readonly UIElement overlay;
        private readonly TextBlock titleText;
        private readonly TextBlock metaText;
        private readonly TextBlock detailText;
        private readonly Button counterpartButton;
        private readonly TextBlock counterpartButtonText;
        private readonly TextBlock pipelineButtonText;
        private readonly TextBlock firstStepButtonText;
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;
        private VISION_MENU? firstStepMenu;
        private VisionPipelineStep firstStep;
        private string counterpartSampleName = string.Empty;

        public OpenVisionShellHostSampleWorkflowPresenter(
            UIElement overlay,
            TextBlock titleText,
            TextBlock metaText,
            TextBlock detailText,
            Button counterpartButton,
            TextBlock counterpartButtonText,
            TextBlock pipelineButtonText,
            TextBlock firstStepButtonText,
            Func<OpenVisionRecipeContext> recipeContextProvider)
        {
            this.overlay = overlay;
            this.titleText = titleText;
            this.metaText = metaText;
            this.detailText = detailText;
            this.counterpartButton = counterpartButton;
            this.counterpartButtonText = counterpartButtonText;
            this.pipelineButtonText = pipelineButtonText;
            this.firstStepButtonText = firstStepButtonText;
            this.recipeContextProvider = recipeContextProvider ?? throw new ArgumentNullException(nameof(recipeContextProvider));
            ApplyLocalization();
        }

        public bool IsVisible => overlay?.Visibility == Visibility.Visible;

        public string Title => titleText?.Text ?? string.Empty;

        public string Meta => metaText?.Text ?? string.Empty;

        public string Detail => detailText?.Text ?? string.Empty;

        public VISION_MENU? FirstStepMenu => firstStepMenu;

        public VisionPipelineStep FirstStep => firstStep;

        public string CounterpartSampleName => counterpartSampleName;

        public bool CanOpenFirstStepTool => IsVisible && firstStepMenu.HasValue;

        public bool CanOpenCounterpartSample => IsVisible && !string.IsNullOrWhiteSpace(counterpartSampleName);

        public void ApplyLocalization()
        {
            SetText(pipelineButtonText, Local("Pipeline 보기", "View Pipeline"));
            SetText(firstStepButtonText, Local("첫 단계 열기", "Open First Step"));
        }

        public void ShowForActiveSample()
        {
            ApplyLocalization();
            SampleWorkflowState state = LoadActiveSampleWorkflow();
            if (state == null)
            {
                Hide();
                return;
            }

            firstStepMenu = ResolveToolMenu(state.FirstTool);
            firstStep = state.FirstStep;
            counterpartSampleName = state.CounterpartSampleName;
            SetText(titleText, Local("샘플 파이프라인 준비됨", "Sample pipeline ready"));
            SetCounterpartButton(state.CounterpartActionText);
            if (state.HasCatalogSample)
            {
                SetText(
                    metaText,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} / {1} steps",
                        state.SampleName,
                        state.StepCount));
                SetText(
                    detailText,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Local(
                            "제품군: {0} / 기준: {1}{3} / 다음: Pipeline 보기 -> Run Review 또는 첫 단계 열기 / 흐름: {2}",
                            "Product group: {0} / Criteria: {1}{3} / Next: View Pipeline -> Run Review or open the first step / Flow: {2}"),
                        state.Category,
                        state.PairRole,
                        state.ToolFlow,
                        FormatPairReviewSuffix(state.PairReviewHint)));
            }
            else
            {
                SetText(
                    metaText,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} / {1} steps",
                        state.PipelineName,
                        state.StepCount));
                SetText(
                    detailText,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Local(
                            "첫 단계: {0} / 흐름: {1} / 다음: Pipeline 보기 -> Run Review 또는 첫 단계 열기",
                            "First step: {0} / Flow: {1} / Next: View Pipeline -> Run Review or open the first step"),
                        state.FirstTool,
                        state.ToolFlow));
            }

            if (overlay != null)
            {
                overlay.Visibility = Visibility.Visible;
                AutomationProperties.SetName(overlay, Title + " " + Meta + " " + Detail);
            }
        }

        public void Hide()
        {
            firstStepMenu = null;
            firstStep = null;
            counterpartSampleName = string.Empty;
            SetCounterpartButton(string.Empty);
            if (overlay != null)
            {
                overlay.Visibility = Visibility.Collapsed;
                AutomationProperties.SetName(overlay, string.Empty);
            }
        }

        private SampleWorkflowState LoadActiveSampleWorkflow()
        {
            OpenVisionRecipeContext recipeContext = ResolveRecipeContext();
            string recipeName = recipeContext.Name;
            string pipelineName = recipeContext.PipelineName;
            if (string.IsNullOrWhiteSpace(pipelineName)
                || !pipelineName.StartsWith("Sample_", StringComparison.Ordinal))
            {
                return null;
            }

            VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, pipelineName);
            VisionPipelineStep[] steps = pipeline?.Steps?
                .Where(step => step != null)
                .ToArray();
            if (steps == null || steps.Length == 0)
            {
                return null;
            }

            VisionPipelineStep firstEnabledStep = steps.FirstOrDefault(step => step.Enabled) ?? steps[0];
            string firstTool = SafeText(
                firstEnabledStep.ToolType,
                "Tool");
            string toolFlow = string.Join(
                " -> ",
                steps
                    .Take(4)
                    .Select(step => SafeText(step.ToolType, "Tool")));
            if (steps.Length > 4)
            {
                toolFlow += " -> ...";
            }

            VisionPipelineSampleCatalogItem catalogSample = ResolveCatalogSample(pipelineName);
            VisionPipelineSampleCatalogItem counterpartSample = ResolvePairCounterpartSample(catalogSample);
            return new SampleWorkflowState
            {
                PipelineName = pipelineName,
                SampleName = SafeText(catalogSample?.SampleName, pipelineName),
                Category = ResolveCategoryDisplayText(catalogSample?.Category),
                PairRole = ResolveSampleRole(catalogSample),
                PairReviewHint = ResolvePairReviewHint(catalogSample),
                CounterpartSampleName = counterpartSample?.SampleName ?? string.Empty,
                CounterpartActionText = ResolveCounterpartActionText(counterpartSample),
                StepCount = steps.Length,
                FirstStep = firstEnabledStep,
                FirstTool = firstTool,
                ToolFlow = toolFlow
            };
        }

        private static VisionPipelineSampleCatalogItem ResolveCatalogSample(string pipelineName)
        {
            if (string.IsNullOrWhiteSpace(pipelineName)
                || !pipelineName.StartsWith("Sample_", StringComparison.Ordinal))
            {
                return null;
            }

            return VisionPipelineSampleCatalogItem.LoadRunnable()
                .FirstOrDefault(item => string.Equals(
                    CreateSamplePipelineName(item.SampleName),
                    pipelineName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static string ResolveSampleRole(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null)
            {
                return "-";
            }

            if (!string.IsNullOrWhiteSpace(sample.PairRole))
            {
                return sample.PairRole.Trim();
            }

            return sample.ExpectsFailure ? "NG" : "OK";
        }

        private static string ResolveCategoryDisplayText(string category)
        {
            string text = SafeText(category, "-");
            string[] parts = text
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
            return parts.Length >= 2 ? parts[1] : text;
        }

        private static string ResolvePairReviewHint(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || string.IsNullOrWhiteSpace(sample.PairGroup))
            {
                return string.Empty;
            }

            string oppositeRole = IsOkSampleReference(sample)
                ? "NG"
                : IsNgSampleReference(sample)
                    ? "OK"
                    : "Good/Bad";
            return string.Format(
                CultureInfo.CurrentCulture,
                Local(
                    "비교: Pipeline Review에서 {0} 기준 열고 Run Review",
                    "Compare: open the {0} reference in Pipeline Review, then Run Review"),
                oppositeRole);
        }

        private static bool IsOkSampleReference(VisionPipelineSampleCatalogItem sample)
        {
            return sample != null
                && !sample.ExpectsFailure
                && string.Equals(sample.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNgSampleReference(VisionPipelineSampleCatalogItem sample)
        {
            return sample != null
                && (sample.ExpectsFailure
                    || string.Equals(sample.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
        }

        private static VisionPipelineSampleCatalogItem ResolvePairCounterpartSample(VisionPipelineSampleCatalogItem sample)
        {
            if (sample == null || string.IsNullOrWhiteSpace(sample.PairGroup))
            {
                return null;
            }

            bool selectedIsOk = IsOkSampleReference(sample);
            bool selectedIsNg = IsNgSampleReference(sample);
            string pairGroup = sample.PairGroup.Trim();
            return VisionPipelineSampleCatalogItem.LoadRunnable(sample.CatalogSourceKind)
                .Where(item => item != null
                    && item.CanOpen
                    && !string.Equals(item.SampleName?.Trim(), sample.SampleName?.Trim(), StringComparison.OrdinalIgnoreCase)
                    && string.Equals(item.PairGroup?.Trim(), pairGroup, StringComparison.OrdinalIgnoreCase))
                .Where(item =>
                    selectedIsOk
                        ? IsNgSampleReference(item)
                        : selectedIsNg
                            ? IsOkSampleReference(item)
                            : true)
                .OrderBy(item => IsOkSampleReference(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        private static string ResolveCounterpartActionText(VisionPipelineSampleCatalogItem counterpartSample)
        {
            if (counterpartSample == null)
            {
                return string.Empty;
            }

            string role = IsOkSampleReference(counterpartSample)
                ? "OK"
                : IsNgSampleReference(counterpartSample)
                    ? "NG"
                    : "Good/Bad";
            return string.Format(
                CultureInfo.CurrentCulture,
                Local("{0} 기준 열기", "Open {0} reference"),
                role);
        }

        private static string FormatPairReviewSuffix(string pairReviewHint)
        {
            return string.IsNullOrWhiteSpace(pairReviewHint)
                ? string.Empty
                : " / " + pairReviewHint.Trim();
        }

        private OpenVisionRecipeContext ResolveRecipeContext()
        {
            OpenVisionRecipeContext context = recipeContextProvider();
            return context ?? new OpenVisionRecipeContext(
                id: "Default",
                name: "Default",
                pipelineName: VisionPipelineAppendService.DefaultPipelineName,
                sourcePath: string.Empty,
                isDirty: false,
                activeLayerName: "Main",
                lastReviewState: string.Empty);
        }

        private static void SetText(TextBlock textBlock, string text)
        {
            if (textBlock != null)
            {
                textBlock.Text = text ?? string.Empty;
            }
        }

        private void SetCounterpartButton(string text)
        {
            bool canOpen = !string.IsNullOrWhiteSpace(text);
            SetText(counterpartButtonText, canOpen ? text : string.Empty);
            if (counterpartButton != null)
            {
                counterpartButton.Visibility = canOpen ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static string SafeText(string text, string fallback)
        {
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        }

        private static string Local(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english
                : korean;
        }

        private static string CreateSamplePipelineName(string sampleName)
        {
            string rawName = string.IsNullOrWhiteSpace(sampleName) ? "Sample" : sampleName.Trim();
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string safeName = new string(rawName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return "Sample_" + (string.IsNullOrWhiteSpace(safeName) ? "Pipeline" : safeName);
        }

        private static VISION_MENU? ResolveToolMenu(string toolType)
        {
            string normalized = NormalizeToolType(toolType);
            switch (normalized)
            {
                case "threshold":
                    return VISION_MENU.Threshold;
                case "morphology":
                    return VISION_MENU.Morphology;
                case "filter":
                    return VISION_MENU.Filter;
                case "arithmetic":
                    return VISION_MENU.Arithmetic;
                case "edgedetection":
                    return VISION_MENU.EdgeDetection;
                case "rotatescale":
                case "rotateandscale":
                    return VISION_MENU.RotateAndScale;
                case "histogram":
                    return VISION_MENU.Histogram;
                case "hsv":
                    return VISION_MENU.HSV;
                case "mean":
                    return VISION_MENU.Mean;
                case "blob":
                    return VISION_MENU.Blob;
                case "contour":
                    return VISION_MENU.Contour;
                case "line":
                case "linegauge":
                case "linedistance":
                case "linedistancegauge":
                case "lineintersection":
                    return VISION_MENU.Line;
                case "matching":
                case "templatematching":
                    return VISION_MENU.Matching;
                case "edgebasedmatching":
                case "edgebasedtemplatematching":
                case "edgetemplatematching":
                    return VISION_MENU.EdgeBasedMatching;
                case "feature":
                case "featurematching":
                case "sift":
                    return VISION_MENU.FeatureMatching;
                default:
                    return null;
            }
        }

        private static string NormalizeToolType(string toolType)
        {
            if (string.IsNullOrWhiteSpace(toolType))
            {
                return string.Empty;
            }

            return new string(
                toolType
                    .Where(char.IsLetterOrDigit)
                    .Select(char.ToLowerInvariant)
                    .ToArray());
        }

        private sealed class SampleWorkflowState
        {
            public string PipelineName { get; set; } = string.Empty;

            public string SampleName { get; set; } = string.Empty;

            public string Category { get; set; } = string.Empty;

            public string PairRole { get; set; } = string.Empty;

            public string PairReviewHint { get; set; } = string.Empty;

            public string CounterpartSampleName { get; set; } = string.Empty;

            public string CounterpartActionText { get; set; } = string.Empty;

            public int StepCount { get; set; }

            public VisionPipelineStep FirstStep { get; set; }

            public string FirstTool { get; set; } = string.Empty;

            public string ToolFlow { get; set; } = string.Empty;

            public bool HasCatalogSample => !string.IsNullOrWhiteSpace(Category) && Category != "-";
        }
    }
}
