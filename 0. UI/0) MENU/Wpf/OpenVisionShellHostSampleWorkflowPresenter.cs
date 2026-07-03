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
        private readonly Func<OpenVisionRecipeContext> recipeContextProvider;
        private VISION_MENU? firstStepMenu;

        public OpenVisionShellHostSampleWorkflowPresenter(
            UIElement overlay,
            TextBlock titleText,
            TextBlock metaText,
            TextBlock detailText,
            Func<OpenVisionRecipeContext> recipeContextProvider)
        {
            this.overlay = overlay;
            this.titleText = titleText;
            this.metaText = metaText;
            this.detailText = detailText;
            this.recipeContextProvider = recipeContextProvider ?? throw new ArgumentNullException(nameof(recipeContextProvider));
        }

        public bool IsVisible => overlay?.Visibility == Visibility.Visible;

        public string Title => titleText?.Text ?? string.Empty;

        public string Meta => metaText?.Text ?? string.Empty;

        public string Detail => detailText?.Text ?? string.Empty;

        public VISION_MENU? FirstStepMenu => firstStepMenu;

        public bool CanOpenFirstStepTool => IsVisible && firstStepMenu.HasValue;

        public void ShowForActiveSample()
        {
            SampleWorkflowState state = LoadActiveSampleWorkflow();
            if (state == null)
            {
                Hide();
                return;
            }

            firstStepMenu = ResolveToolMenu(state.FirstTool);
            SetText(titleText, "\uC0D8\uD50C \uD30C\uC774\uD504\uB77C\uC778 \uC900\uBE44\uB428");
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
                        "\uC81C\uD488\uAD70: {0} / \uAE30\uC900: {1} / \uD750\uB984: {2} / \uB2E4\uC74C: Pipeline \uBCF4\uAE30 \uB610\uB294 \uCCAB \uB2E8\uACC4 \uC5F4\uAE30",
                        state.Category,
                        state.PairRole,
                        state.ToolFlow));
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
                        "\uCCAB \uB2E8\uACC4: {0} / \uD750\uB984: {1} / \uB2E4\uC74C: Pipeline \uBCF4\uAE30 \uB610\uB294 \uCCAB \uB2E8\uACC4 \uC5F4\uAE30",
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

            string firstTool = SafeText(
                steps.FirstOrDefault(step => step.Enabled)?.ToolType
                ?? steps[0].ToolType,
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
            return new SampleWorkflowState
            {
                PipelineName = pipelineName,
                SampleName = SafeText(catalogSample?.SampleName, pipelineName),
                Category = SafeText(catalogSample?.Category, "-"),
                PairRole = ResolveSampleRole(catalogSample),
                StepCount = steps.Length,
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

        private static string SafeText(string text, string fallback)
        {
            return string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
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

            public int StepCount { get; set; }

            public string FirstTool { get; set; } = string.Empty;

            public string ToolFlow { get; set; } = string.Empty;

            public bool HasCatalogSample => !string.IsNullOrWhiteSpace(Category) && Category != "-";
        }
    }
}
