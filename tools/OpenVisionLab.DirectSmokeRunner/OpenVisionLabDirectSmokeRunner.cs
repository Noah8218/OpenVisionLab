using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using OpenVisionLab.Vision2D.Property;
using Microsoft.Web.WebView2.Wpf;
using OpenVisionLab.Docking.Controls;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;
using static OpenVisionLab.Core.FormulaUtil;

namespace OpenVisionLab
{
    internal static class OpenVisionLabDirectSmokeRunner
    {
        private const uint MouseEventLeftDown = 0x0002;
        private const uint MouseEventLeftUp = 0x0004;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(
            IntPtr hdc,
            IntPtr clip,
            MonitorEnumCallback callback,
            IntPtr data);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref SmokeMonitorInfo monitorInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr window, out SmokeNativeRect rect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr window,
            IntPtr insertAfter,
            int x,
            int y,
            int width,
            int height,
            uint flags);

        private delegate bool MonitorEnumCallback(
            IntPtr monitor,
            IntPtr hdc,
            ref SmokeNativeRect rect,
            IntPtr data);

        public static bool TryRun(string[] args)
        {
            if (args == null
                || args.Length == 0
                || !string.Equals(args[0], "--smoke", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string scenario = args.Length > 1 ? args[1] : string.Empty;
            string outputDirectory = ResolveOutputDirectory(args);

            try
            {
                if (string.Equals(scenario, "startup-loading-feedback", StringComparison.OrdinalIgnoreCase))
                {
                    RunStartupLoadingFeedback(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "line-pins-measure", StringComparison.OrdinalIgnoreCase))
                {
                    RunLinePinsMeasure(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "property-grid-roi-editor", StringComparison.OrdinalIgnoreCase))
                {
                    RunPropertyGridRoiEditor(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "edge-based-scale-matching", StringComparison.OrdinalIgnoreCase))
                {
                    RunEdgeBasedScaleMatching(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "matching-vs-edge-based-scale-comparison", StringComparison.OrdinalIgnoreCase))
                {
                    RunMatchingVsEdgeBasedScaleComparison(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "matching-pyramid-scale", StringComparison.OrdinalIgnoreCase))
                {
                    RunMatchingPyramidScale(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "matching-c9-batch", StringComparison.OrdinalIgnoreCase))
                {
                    RunMatchingC9Batch(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "matching-die-pad-batch", StringComparison.OrdinalIgnoreCase))
                {
                    RunMatchingDiePadBatch(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "tool-dock-float-cycle", StringComparison.OrdinalIgnoreCase))
                {
                    RunToolDockFloatCycle(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "tool-preview-popout", StringComparison.OrdinalIgnoreCase))
                {
                    RunToolPreviewPopout(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "workspace-startup-empty", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "startup-empty-workspace", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "shell-startup-empty-workspace", StringComparison.OrdinalIgnoreCase))
                {
                    RunWorkspaceStartupEmpty(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-threshold-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnThresholdPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-filter-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnFilterPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-morphology-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnMorphologyPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-blob-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnBlobPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-contour-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnContourPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-edge-detection-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnEdgeDetectionPractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "parameter-guide-layout", StringComparison.OrdinalIgnoreCase))
                {
                    RunParameterGuideLayout(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "property-persistence-feedback",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunPropertyPersistenceFeedback(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "property-load-feedback",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunPropertyLoadFeedback(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "settings-persistence-feedback",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunSettingsPersistenceFeedback(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "runtime-data-root-contract",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunRuntimeDataRootContract(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "learn-line-distance-practice", StringComparison.OrdinalIgnoreCase))
                {
                    RunLearnLineDistancePractice(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "recipe-manager-tabs", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "recipe-manager-direct", StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipeManagerTabs(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "recipe-pipeline-roundtrip", StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipePipelineRoundtrip(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "recipe-pipeline-persistence-feedback",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipePipelinePersistenceFeedback(args, outputDirectory);
                    return true;
                }

                if (string.Equals(
                    scenario,
                    "recipe-persistence-reopen-probe",
                    StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipePersistenceReopenProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "recipe-manager-llm-intent-skills", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "llm-intent-skills", StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipeManagerLlmIntentSkills(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "recipe-manager-reference-difference-guided-setup", StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipeManagerReferenceDifferenceGuidedSetup(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "llm-xml-draft-file", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "llm-draft-file", StringComparison.OrdinalIgnoreCase))
                {
                    RunLlmXmlDraftFile(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "llm-xml-branch-review-file", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "llm-draft-branch-review", StringComparison.OrdinalIgnoreCase))
                {
                    RunLlmXmlBranchReviewFile(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "llm-xml-image-run", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "llm-draft-image-run", StringComparison.OrdinalIgnoreCase))
                {
                    RunLlmXmlImageRun(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-load-matching-flow", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "layer-image-matching-flow", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerLoadMatchingFlow(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-global-docking", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerGlobalDocking(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-docking-tab-click-no-guide", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "docking-tab-click-no-guide", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerDockingTabClickNoGuide(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-docking-mouse-drag", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "docking-mouse-drag", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerDockingMouseDrag(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-initial-docked-workspace", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "initial-docked-workspace", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "layer-host-tab-drag-dock", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "host-tab-drag-dock", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerInitialDockedWorkspace(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "layer-docking-verification", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "docking-verification", StringComparison.OrdinalIgnoreCase))
                {
                    RunLayerDockingVerification(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "tutorial-captures", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "documentation-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunTutorialCaptures(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-industrial-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioIndustrialCaptures(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-card-match-probe", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioCardMatchProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-pattern-grid-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioPatternGridCaptures(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-stage-grid-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioStageGridCaptures(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-pin-array-probe", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioPinArrayProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-gap-recipe-probe", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioGapRecipeProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-gap-matching-correction-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioGapMatchingCorrectionCaptures(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-line-intersection-probe", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioLineIntersectionProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-contour-probe", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioContourProbe(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "portfolio-extended-captures", StringComparison.OrdinalIgnoreCase))
                {
                    RunPortfolioExtendedCaptures(args, outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "public-fixture-review", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "fixture-pipeline-review", StringComparison.OrdinalIgnoreCase))
                {
                    RunPublicFixtureReview(outputDirectory);
                    return true;
                }

                throw new InvalidOperationException("Unknown OpenVisionLab smoke scenario: " + scenario);
            }
            catch (Exception ex)
            {
                Directory.CreateDirectory(outputDirectory);
                string reportPath = Path.Combine(outputDirectory, "report.txt");
                string previousReport = File.Exists(reportPath)
                    ? Environment.NewLine + "Previous report:" + Environment.NewLine + File.ReadAllText(reportPath)
                    : string.Empty;
                File.WriteAllText(
                    reportPath,
                    "Result: FAIL" + previousReport + Environment.NewLine + ex,
                    Encoding.UTF8);
                Environment.ExitCode = 1;
                return true;
            }
        }

        private static void RunLlmXmlImageRun(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string draftPath = ResolveRequiredOption(args, "--draft");
            string imagePath = ResolveRequiredOption(args, "--image");
            int timeoutMilliseconds = ResolveOptionalIntOption(args, "--timeout-ms", 5000);
            bool expectedRunSuccess = ResolveOptionalBoolOption(args, "--expect-run-success", true);

            if (!File.Exists(draftPath))
            {
                throw new FileNotFoundException("LLM XML draft file was not found.", draftPath);
            }

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("LLM XML draft verification image was not found.", imagePath);
            }

            if (!SerializeHelper.TryLoadFromXmlFile(draftPath, out VisionPipeline pipeline) || pipeline == null)
            {
                throw new InvalidOperationException("LLM XML draft could not be loaded for image execution: " + draftPath);
            }

            File.Copy(draftPath, Path.Combine(outputDirectory, "LlmDraft.pipeline.xml"), true);

            VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
            VisionRecipeRunResult imageRunResult = RunLlmDraftOnImage(pipeline, imagePath, outputDirectory, timeoutMilliseconds);
            bool actualRunSuccess = imageRunResult != null && imageRunResult.Success;
            bool pass = validation.Success && actualRunSuccess == expectedRunSuccess;

            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: " + (pass ? "PASS" : "FAIL") + Environment.NewLine
                + "Scenario: llm-xml-image-run" + Environment.NewLine
                + "DraftPath: " + draftPath + Environment.NewLine
                + "DraftSnapshot: LlmDraft.pipeline.xml" + Environment.NewLine
                + "DraftSha256: " + ComputeC9FileSha256(draftPath) + Environment.NewLine
                + "ImagePath: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "TimeoutMs: " + timeoutMilliseconds.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "ExpectedRunSuccess: " + expectedRunSuccess + Environment.NewLine
                + "ActualRunSuccess: " + actualRunSuccess + Environment.NewLine
                + "ValidationSuccess: " + validation.Success + Environment.NewLine
                + "ValidationErrors: " + validation.Errors.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + string.Join(Environment.NewLine, validation.Errors.Select(error => "ValidationError: " + error)) + Environment.NewLine
                + "ValidationWarnings: " + validation.Warnings.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + string.Join(Environment.NewLine, validation.Warnings.Select(warning => "ValidationWarning: " + warning)) + Environment.NewLine
                + BuildLlmDraftImageRunReport(imageRunResult, imagePath),
                Encoding.UTF8);

            imageRunResult?.ResultImage?.Dispose();

            if (!pass)
            {
                throw new InvalidOperationException(
                    "LLM XML draft image run failed. "
                    + $"ValidationSuccess={validation.Success}, ExpectedRunSuccess={expectedRunSuccess}, ActualRunSuccess={actualRunSuccess}");
            }
        }

        private static void RunLlmXmlDraftFile(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces("Smoke_LlmDraft_");
            string draftPath = ResolveRequiredOption(args, "--draft");
            if (!File.Exists(draftPath))
            {
                throw new FileNotFoundException("LLM XML draft file was not found.", draftPath);
            }

            string expectedPipelineName = ResolveDraftPipelineName(draftPath);
            string imagePath = ResolveOptionalOption(args, "--image");
            bool expectedRunSuccess = ResolveOptionalBoolOption(args, "--expect-run-success", true);
            if (!string.IsNullOrWhiteSpace(imagePath) && !File.Exists(imagePath))
            {
                throw new FileNotFoundException("LLM XML draft verification image was not found.", imagePath);
            }

            string recipeName = "Smoke_LlmDraft_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Direct_LlmDraft_Baseline";
            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(pipelineName, 1));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(40);

                bool validationOk = shellHost.RecipeCommands.LoadLlmXmlDraftFromPath(draftPath);
                bool importEnabled = shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null);
                string selectedBeforeImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
                if (validationOk && importEnabled)
                {
                    shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
                    Pump(80);
                }

                string selectedAfterImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
                bool imported = validationOk
                    && importEnabled
                    && !string.Equals(selectedBeforeImport, selectedAfterImport, StringComparison.OrdinalIgnoreCase)
                    && (string.IsNullOrWhiteSpace(expectedPipelineName)
                        || selectedAfterImport.IndexOf(expectedPipelineName, StringComparison.OrdinalIgnoreCase) >= 0);

                VisionRecipeRunResult imageRunResult = null;
                string imageRunReport = "ImageRun: SKIPPED";
                bool imageRunOk = string.IsNullOrWhiteSpace(imagePath);
                bool actualRunSuccess = false;
                if (validationOk && imported && !string.IsNullOrWhiteSpace(imagePath))
                {
                    VisionPipeline pipeline = VisionPipelineStorage.Load(recipeName, selectedAfterImport);
                    if (pipeline == null)
                    {
                        throw new InvalidOperationException("Imported LLM XML draft pipeline could not be loaded for image execution: " + selectedAfterImport);
                    }

                    imageRunResult = RunLlmDraftOnImage(pipeline, imagePath, outputDirectory, 5000);
                    actualRunSuccess = imageRunResult != null && imageRunResult.Success;
                    imageRunOk = actualRunSuccess == expectedRunSuccess;
                    imageRunReport = BuildLlmDraftImageRunReport(imageRunResult, imagePath);
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: " + (validationOk && imported && imageRunOk ? "PASS" : "FAIL") + Environment.NewLine
                    + "Scenario: llm-xml-draft-file" + Environment.NewLine
                    + "DraftPath: " + draftPath + Environment.NewLine
                    + "ImagePath: " + (string.IsNullOrWhiteSpace(imagePath) ? "-" : imagePath) + Environment.NewLine
                    + "ValidationOk: " + validationOk + Environment.NewLine
                    + "ImportEnabled: " + importEnabled + Environment.NewLine
                    + "Imported: " + imported + Environment.NewLine
                    + "SelectedBeforeImport: " + selectedBeforeImport + Environment.NewLine
                    + "SelectedAfterImport: " + selectedAfterImport + Environment.NewLine
                    + "ExpectedRunSuccess: " + (string.IsNullOrWhiteSpace(imagePath) ? "-" : expectedRunSuccess.ToString()) + Environment.NewLine
                    + "ActualRunSuccess: " + (string.IsNullOrWhiteSpace(imagePath) ? "-" : actualRunSuccess.ToString()) + Environment.NewLine
                    + imageRunReport + Environment.NewLine
                    + "ValidationReport:" + Environment.NewLine
                    + shellHost.RecipeCommands.LlmXmlDraftValidationReport + Environment.NewLine
                    + "DependencyReport:" + Environment.NewLine
                    + shellHost.RecipeCommands.LlmXmlDraftDependencyReport + Environment.NewLine
                    + "ReviewReport:" + Environment.NewLine
                    + shellHost.RecipeCommands.LlmXmlDraftReviewReport + Environment.NewLine
                    + "DiffReport:" + Environment.NewLine
                    + shellHost.RecipeCommands.LlmXmlDraftDiffReport,
                    Encoding.UTF8);

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_LlmXmlDraftFile.png"));

                imageRunResult?.ResultImage?.Dispose();

                if (!validationOk || !imported || !imageRunOk)
                {
                    throw new InvalidOperationException(
                        "LLM XML draft file did not validate/import. "
                        + $"ValidationOk={validationOk}, ImportEnabled={importEnabled}, Imported={imported}, ExpectedRunSuccess={expectedRunSuccess}, ActualRunSuccess={actualRunSuccess}, ImageRunOk={imageRunOk}");
                }
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunLlmXmlBranchReviewFile(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces("Smoke_LlmBranch_");
            string draftPath = ResolveRequiredOption(args, "--draft");
            if (!File.Exists(draftPath))
            {
                throw new FileNotFoundException("LLM XML draft file was not found.", draftPath);
            }

            if (!SerializeHelper.TryLoadFromXmlFile(draftPath, out VisionPipeline pipeline) || pipeline == null)
            {
                throw new InvalidOperationException("LLM XML draft could not be loaded for branch review: " + draftPath);
            }

            VisionPipelineStep overlayStep = pipeline.Steps
                .FirstOrDefault(step => step != null
                    && step.Enabled
                    && string.Equals(step.ToolType, "OverlayMerge", StringComparison.OrdinalIgnoreCase)
                    && (ReadSmokeListParameter(step, "SourceLayers").Count > 0
                        || ReadSmokeListParameter(step, "SourceSteps").Count > 0))
                ?? throw new InvalidOperationException("Branch review requires an enabled OverlayMerge with SourceLayers or SourceSteps.");
            string sourceReferenceKey = ReadSmokeListParameter(overlayStep, "SourceLayers").Count > 0
                ? "SourceLayers"
                : "SourceSteps";
            IReadOnlyList<string> sourceReferences = ReadSmokeListParameter(overlayStep, sourceReferenceKey);
            List<VisionPipelineStep> sourceSteps = sourceReferences
                .Select(reference => pipeline.Steps.FirstOrDefault(step => step != null
                    && step.Enabled
                    && (string.Equals(sourceReferenceKey, "SourceSteps", StringComparison.OrdinalIgnoreCase)
                        ? string.Equals(step.Name, reference, StringComparison.OrdinalIgnoreCase)
                        : string.Equals(step.OutputLayer, reference, StringComparison.OrdinalIgnoreCase))))
                .ToList();
            if (sourceSteps.Any(step => step == null))
            {
                throw new InvalidOperationException(
                    "Branch review could not resolve every OverlayMerge " + sourceReferenceKey + " producer. "
                    + "Sources=" + string.Join(",", sourceReferences));
            }

            string recipeName = "Smoke_LlmBranch_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(50);

                int previewRunsBefore = shellHost.NativePreviewRunCount;
                string activeLayerBefore = shellHost.WorkspaceLayerTitle;
                System.Windows.Controls.Primitives.ToggleButton recipeManagerButton =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "btnHostRecipeManager",
                        "LLM XML branch review");
                recipeManagerButton.IsChecked = true;
                Pump(50);

                System.Windows.Controls.Primitives.ToggleButton advancedReviewToggle =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "recipeAdvancedReviewToggle",
                        "LLM XML branch review");
                advancedReviewToggle.IsChecked = true;
                Pump(50);

                TabItem pipelineTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipeline",
                    "LLM XML branch review");
                pipelineTab.IsSelected = true;
                Pump(40);
                TabItem xmlStepsTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineXmlSteps",
                    "LLM XML branch review");
                xmlStepsTab.IsSelected = true;
                Pump(50);

                IReadOnlyList<OpenVisionRecipePipelineStepPreview> previewSteps =
                    shellHost.RecipeCommands.SelectedRecipeSummary?.PipelinePreviewSteps
                    ?? Array.Empty<OpenVisionRecipePipelineStepPreview>();
                OpenVisionRecipePipelineStepPreview overlayPreview = previewSteps.FirstOrDefault(step =>
                    step != null
                    && string.Equals(step.Name, overlayStep.Name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException("Branch review could not find the OverlayMerge preview Step.");
                List<OpenVisionRecipePipelineStepPreview> sourcePreviews = sourceSteps
                    .Select(source => previewSteps.FirstOrDefault(step => step != null
                        && string.Equals(step.Name, source.Name, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(step.OutputLayer, source.OutputLayer, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                if (sourcePreviews.Any(step => step == null))
                {
                    throw new InvalidOperationException("Branch review could not find every source preview Step.");
                }

                List<string> consumerReports = new List<string>();
                int visibleConsumerRelations = 0;
                for (int i = 0; i < sourcePreviews.Count; i++)
                {
                    OpenVisionRecipePipelineStepPreview sourcePreview = sourcePreviews[i];
                    shellHost.RecipeCommands.SelectedPipelinePreviewStep = sourcePreview;
                    Pump(24);
                    IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> rows =
                        shellHost.RecipeCommands.BranchOutputComparisonRows;
                    string expectedRoute = sourcePreview.OutputLayer + " -> " + overlayPreview.OutputLayer;
                    bool visible = rows.Any(row =>
                        row.StepName.Contains(overlayPreview.Name, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(row.Route, expectedRoute, StringComparison.OrdinalIgnoreCase));
                    if (visible)
                    {
                        visibleConsumerRelations++;
                    }

                    consumerReports.Add(
                        sourcePreview.OutputLayer
                        + " -> "
                        + overlayPreview.OutputLayer
                        + " | Visible="
                        + visible
                        + " | Rows="
                        + DescribeBranchRows(rows));
                }

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = overlayPreview;
                Pump(30);
                IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> overlayRows =
                    shellHost.RecipeCommands.BranchOutputComparisonRows;
                int visibleProducerRelations = sourcePreviews.Count(sourcePreview =>
                    overlayRows.Any(row =>
                        row.StepName.Contains(sourcePreview.Name, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(
                            row.Route,
                            sourcePreview.OutputLayer + " -> " + overlayPreview.OutputLayer,
                            StringComparison.OrdinalIgnoreCase)));

                System.Windows.FrameworkElement branchPanel = FindVisualChildren<System.Windows.FrameworkElement>(shellHost)
                    .FirstOrDefault(element => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(element),
                        "HostRecipeBranchOutputComparisonPanel",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Branch comparison panel was not found.");

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = sourcePreviews[0];
                branchPanel.BringIntoView();
                MoveCursorInsideWindow(window, 24D, 24D);
                Pump(60);
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_PipelineReview_SourceConsumer.png"));

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = overlayPreview;
                branchPanel.BringIntoView();
                MoveCursorInsideWindow(window, 24D, 24D);
                Pump(60);
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_PipelineReview_OverlaySources.png"));

                bool statePreserved = shellHost.NativePreviewRunCount == previewRunsBefore
                    && string.Equals(shellHost.WorkspaceLayerTitle, activeLayerBefore, StringComparison.OrdinalIgnoreCase);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: llm-xml-branch-review-file" + Environment.NewLine
                    + "DraftPath: " + draftPath + Environment.NewLine
                    + "Pipeline: " + pipeline.Name + Environment.NewLine
                    + "OverlayStep: " + overlayPreview.Index + " | " + overlayPreview.Name + Environment.NewLine
                    + "Declared" + sourceReferenceKey + ": " + sourceReferences.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "SourceConsumerRelationsVisible: " + visibleConsumerRelations.ToString(CultureInfo.InvariantCulture)
                    + "/" + sourceReferences.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "OverlaySourceProducersVisible: " + visibleProducerRelations.ToString(CultureInfo.InvariantCulture)
                    + "/" + sourceReferences.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "OverlayRows: " + DescribeBranchRows(overlayRows) + Environment.NewLine
                    + "ConsumerRelations:" + Environment.NewLine
                    + string.Join(Environment.NewLine, consumerReports) + Environment.NewLine
                    + "PreviewRunCountUnchanged: " + (shellHost.NativePreviewRunCount - previewRunsBefore).ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ActiveLayerUnchanged: " + statePreserved,
                    Encoding.UTF8);

                if (!statePreserved)
                {
                    throw new InvalidOperationException(
                        "Branch review changed execution or active-layer state. "
                        + $"Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, Layer={activeLayerBefore}->{shellHost.WorkspaceLayerTitle}");
                }
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static IReadOnlyList<string> ReadSmokeListParameter(VisionPipelineStep step, string key)
        {
            if (step?.Parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return Array.Empty<string>();
            }

            string value = step.Parameters
                .Where(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Value)
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            return value
                .Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void CleanupKnownSmokeRecipeWorkspaces(params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                return;
            }

            foreach (string recipeName in RecipeWorkspaceService.GetRecipeNames())
            {
                string prefix = prefixes.FirstOrDefault(candidate =>
                    !string.IsNullOrWhiteSpace(candidate)
                    && recipeName.StartsWith(candidate, StringComparison.Ordinal));
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    continue;
                }

                string suffix = recipeName.Substring(prefix.Length);
                if (suffix.Length == 12 && suffix.All(Uri.IsHexDigit))
                {
                    RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
                }
            }
        }

        private static VisionRecipeRunResult RunLlmDraftOnImage(
            VisionPipeline pipeline,
            string imagePath,
            string outputDirectory,
            int timeoutMilliseconds)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            using (OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged))
            {
                if (source.Empty())
                {
                    throw new InvalidOperationException("LLM XML draft verification image could not be loaded: " + imagePath);
                }

                OpenCvSharp.Cv2.ImWrite(Path.Combine(outputDirectory, "LlmDraft_Source.png"), source);

                VisionRecipeRunner runner = new VisionRecipeRunner();
                Task<VisionRecipeRunResult> runTask = runner.RunAsync(
                    pipeline,
                    source,
                    VisionRecipeRunner.DefaultInputLayer,
                    timeoutMilliseconds,
                    CancellationToken.None);

                // The combined import smoke runs on the WPF Dispatcher thread.
                Application application = Application.Current;
                if (application != null && application.Dispatcher == Dispatcher.CurrentDispatcher)
                {
                    WaitForTaskWithPump(runTask, "LLM XML draft image run");
                }

                VisionRecipeRunResult result = runTask.GetAwaiter().GetResult();
                if (result?.ResultImage != null && !result.ResultImage.Empty())
                {
                    OpenCvSharp.Cv2.ImWrite(Path.Combine(outputDirectory, "LlmDraft_RunResult.png"), result.ResultImage);
                }


                VisionRecipeStepRunSummary overlaySummary = result?.Steps?
                    .LastOrDefault(step => step?.Overlays != null && step.Overlays.Count > 0);
                if (overlaySummary != null)
                {
                    int stepIndex = overlaySummary.Index - 1;
                    VisionPipelineStep pipelineStep = stepIndex >= 0 && stepIndex < (pipeline.Steps?.Count ?? 0)
                        ? pipeline.Steps[stepIndex]
                        : pipeline.Steps?.FirstOrDefault(step => string.Equals(step?.Name, overlaySummary.Name, StringComparison.OrdinalIgnoreCase));
                    using OpenCvSharp.Mat overlaySource = new OpenCvSharp.Mat();
                    if (source.Channels() == 1)
                    {
                        OpenCvSharp.Cv2.CvtColor(source, overlaySource, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
                    }
                    else
                    {
                        source.CopyTo(overlaySource);
                    }

                    using Bitmap overlayBitmap = BitmapImageConverter.ToBitmap(overlaySource);
                    VisionPipelineRunReportImageRenderer.RenderInPlace(overlayBitmap, overlaySummary, pipelineStep);
                    overlayBitmap.Save(
                        Path.Combine(outputDirectory, "LlmDraft_RuntimeOverlay.png"),
                        System.Drawing.Imaging.ImageFormat.Png);
                }

                return result;
            }
        }

        private static string BuildLlmDraftImageRunReport(VisionRecipeRunResult result, string imagePath)
        {
            if (result == null)
            {
                return "ImageRun: FAIL" + Environment.NewLine + "ImageRunMessage: no run result";
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ImageRun: " + (result.Success ? "PASS" : "FAIL"));
            builder.AppendLine("ImageRunPath: " + imagePath);
            builder.AppendLine("ImageRunMessage: " + result.Message);
            builder.AppendLine("ImageRunFinalLayer: " + result.FinalLayer);
            builder.AppendLine("ImageRunFinalStep: " + result.FinalStepName);
            builder.AppendLine("ImageRunTotalMs: " + result.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture));
            builder.AppendLine("ImageRunResultSize: " + result.ResultImageWidth.ToString(CultureInfo.InvariantCulture) + "x" + result.ResultImageHeight.ToString(CultureInfo.InvariantCulture));

            foreach (VisionRecipeStepRunSummary step in result.Steps ?? new List<VisionRecipeStepRunSummary>())
            {
                builder.AppendLine(
                    "ImageRunStep: "
                    + step.Index.ToString(CultureInfo.InvariantCulture)
                    + " | " + step.Name
                    + " | " + step.ToolType
                    + " | " + step.Status
                    + " | Acceptance=" + step.AcceptancePassed
                    + " | OverlayCount=" + step.OverlayCount.ToString(CultureInfo.InvariantCulture)
                    + " | " + step.AcceptanceMessage);
                if (!string.IsNullOrWhiteSpace(step.MetricsText))
                {
                    builder.AppendLine("ImageRunMetrics: " + step.MetricsText);
                }

                foreach (VisionRecipeOverlaySummary overlay in step.Overlays ?? new List<VisionRecipeOverlaySummary>())
                {
                    builder.AppendLine(
                        "ImageRunOverlay: Kind=" + overlay.Kind
                        + " | Label=" + overlay.Label
                        + " | Bounds="
                        + overlay.BoundsX.ToString("0.###", CultureInfo.InvariantCulture) + ","
                        + overlay.BoundsY.ToString("0.###", CultureInfo.InvariantCulture) + ","
                        + overlay.BoundsWidth.ToString("0.###", CultureInfo.InvariantCulture) + ","
                        + overlay.BoundsHeight.ToString("0.###", CultureInfo.InvariantCulture)
                        + " | Center="
                        + overlay.CenterX.ToString("0.###", CultureInfo.InvariantCulture) + ","
                        + overlay.CenterY.ToString("0.###", CultureInfo.InvariantCulture)
                        + " | Angle=" + overlay.Angle.ToString("0.###", CultureInfo.InvariantCulture));
                }

                if (step.Metrics != null && step.Metrics.TryGetValue("BoundsHeightAvg", out double boundsHeightAvg))
                {
                    builder.AppendLine("MeasuredBoundsHeightAvg: " + boundsHeightAvg.ToString("0.###", CultureInfo.InvariantCulture));
                }
            }

            return builder.ToString().TrimEnd();
        }

        private static string ResolveDraftPipelineName(string draftPath)
        {
            return SerializeHelper.TryLoadFromXmlFile(draftPath, out VisionPipeline pipeline) && pipeline != null
                ? pipeline.Name ?? string.Empty
                : string.Empty;
        }

        private static void RunLinePinsMeasure(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string pinsPath = Path.Combine(
                repoRoot,
                "docs",
                "samples",
                "public",
                "Line_Pins_Synthetic_OK.png");
            if (!File.Exists(pinsPath))
            {
                throw new FileNotFoundException("Public pin measurement sample image was not found.", pinsPath);
            }

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap pinsBitmap = new Bitmap(pinsPath))
                {
                    shellHost.SetMainLayerImageForTest(pinsBitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(60);
                OpenVisionFloatingToolWindow toolWindow = Application.Current.Windows
                    .OfType<OpenVisionFloatingToolWindow>()
                    .FirstOrDefault(item => item.IsVisible
                        && FindVisualChildren<LineToolWpfView>(item).Any())
                    ?? throw new InvalidOperationException("Line pins measure did not open the Line tool window.");
                string toolMonitorEvidence = PlaceWindowOnLeftmostMonitor(toolWindow);

                ConfigurePinsVerticalDistanceLine(shellHost, "Line A", "X_LTOR");
                ConfigurePinsVerticalDistanceLine(shellHost, "Line B", "X_RTOL");
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveLinePurposeForTest("Edge");
                Pump(16);

                if (shellHost.ActiveLineInputRoiOverlayCount < 2)
                {
                    throw new InvalidOperationException(
                        "Line tool did not publish both Line A/B ROI overlays. Count="
                        + shellHost.ActiveLineInputRoiOverlayCount.ToString(CultureInfo.InvariantCulture));
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                if (!shellHost.ActiveLineSignalInspectorHasEvidenceForTest
                    || shellHost.ActiveLineSignalInspectorOverlayVisibleForTest
                    || !shellHost.ActiveLineSignalEvidenceCueVisibleForTest)
                {
                    throw new InvalidOperationException(
                        "Line preview did not retain the parameter editor and show only the transient signal cue.");
                }

                int previewRunsBeforeSignalReview = shellHost.NativePreviewRunCount;
                int layerCountBeforeSignalReview = shellHost.LayerDocumentCount;
                string activeLayerBeforeSignalReview = shellHost.ActiveHostLayerTitle;
                string inputRouteBeforeSignalReview = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBeforeSignalReview = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                SaveWindowScreenshot(
                    toolWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_LineSignal_TransientCue.png"));

                Stopwatch cueWait = Stopwatch.StartNew();
                while (cueWait.Elapsed < TimeSpan.FromMilliseconds(3400))
                {
                    Pump(1);
                    Thread.Sleep(20);
                }

                if (shellHost.ActiveLineSignalEvidenceCueVisibleForTest
                    || shellHost.ActiveLineSignalInspectorOverlayVisibleForTest)
                {
                    throw new InvalidOperationException(
                        "Line signal cue did not dismiss while the parameter editor remained visible.");
                }

                AssertLineSignalReviewSideEffectsUnchanged(
                    shellHost,
                    previewRunsBeforeSignalReview,
                    layerCountBeforeSignalReview,
                    activeLayerBeforeSignalReview,
                    inputRouteBeforeSignalReview,
                    outputRouteBeforeSignalReview,
                    "transient signal cue");
                SaveWindowScreenshot(
                    toolWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_LineSignal_ParametersRetained.png"));

                shellHost.OpenActiveLineSignalInspectorForTest();
                Pump(12);
                if (!shellHost.ActiveLineSignalInspectorOverlayVisibleForTest
                    || shellHost.ActiveLineSignalEvidenceCueVisibleForTest)
                {
                    throw new InvalidOperationException(
                        "Manual Line signal review did not open the retained detailed inspector.");
                }

                AssertLineSignalReviewSideEffectsUnchanged(
                    shellHost,
                    previewRunsBeforeSignalReview,
                    layerCountBeforeSignalReview,
                    activeLayerBeforeSignalReview,
                    inputRouteBeforeSignalReview,
                    outputRouteBeforeSignalReview,
                    "manual signal review");
                SaveWindowScreenshot(
                    toolWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_LineSignal_ManualInspector.png"));
                shellHost.CloseActiveLineSignalInspectorForTest();
                Pump(8);

                shellHost.SetActiveLinePurposeForTest("Measure");
                Pump(16);
                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                string review = shellHost.ActiveNativeResultReviewText;
                string status = shellHost.ActiveNativeStatusText;
                bool hasMeasurementReview = review.IndexOf("Measure /", StringComparison.OrdinalIgnoreCase) >= 0
                    || review.IndexOf("\uCE21\uC815 /", StringComparison.Ordinal) >= 0;
                bool hasNoDistance = review.IndexOf("Distance none", StringComparison.OrdinalIgnoreCase) >= 0
                    || review.IndexOf("\uAC70\uB9AC \uC5C6\uC74C", StringComparison.Ordinal) >= 0;
                bool hasNoDetection = review.IndexOf("Count 0", StringComparison.OrdinalIgnoreCase) >= 0
                    || review.IndexOf("\uAC80\uCD9C 0\uAC1C", StringComparison.Ordinal) >= 0;
                if (!shellHost.HasNativePreviewResult
                    || !hasMeasurementReview
                    || hasNoDistance
                    || hasNoDetection)
                {
                    throw new InvalidOperationException(
                        "Line pins measure preview did not complete successfully."
                        + Environment.NewLine
                        + "Status: " + status
                        + Environment.NewLine
                        + "Review: " + review);
                }

                double measuredMm = ExtractMm(review);
                if (measuredMm < 0.18D || measuredMm > 0.25D)
                {
                    throw new InvalidOperationException(
                        "Line pins measure was outside expected range. mm="
                        + measuredMm.ToString("0.###", CultureInfo.InvariantCulture)
                        + ", Review=" + review);
                }

                using (Bitmap preview = shellHost.GetLayerImageCloneForTest("Line_Preview"))
                {
                    if (preview == null)
                    {
                        throw new InvalidOperationException("Line_Preview output layer was not created.");
                    }

                    preview.Save(Path.Combine(outputDirectory, "Line_Preview.png"));
                }

                VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
                ValidatePinsMeasurePipelineStep(step);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_LinePinsMeasure.png"));

                string report = BuildPassReport(review, status, measuredMm, step)
                    + Environment.NewLine
                    + monitorEvidence
                    + Environment.NewLine
                    + "Tool" + toolMonitorEvidence
                    + Environment.NewLine
                    + "SignalCuePurpose: Edge"
                    + Environment.NewLine
                    + "SignalInspectorAutoOpen: false"
                    + Environment.NewLine
                    + "SignalCueAutoDismiss: true"
                    + Environment.NewLine
                    + "ManualSignalInspector: retained"
                    + Environment.NewLine
                    + "SignalReviewSideEffects: unchanged";
                File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunStartupLoadingFeedback(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionStartupLoadingWindow window = null;

            try
            {
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
                window = new OpenVisionStartupLoadingWindow();
                window.ShowReady();
                string monitor = PlaceWindowOnLeftmostMonitor(window);
                Pump(12);

                if (!string.Equals(window.LoadingTitleForTest, "프로그램 준비 중", StringComparison.Ordinal)
                    || !window.LoadingDetailForTest.Contains("마지막 레시피", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Korean startup loading copy mismatch. Title='{window.LoadingTitleForTest}', Detail='{window.LoadingDetailForTest}'");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_startup_loading_ko.png"));
                window.Close();
                Pump(4);
                if (!window.IsVisible)
                {
                    throw new InvalidOperationException("Startup loading window closed before startup completion.");
                }

                window.Complete();
                Pump(4);
                if (window.IsVisible)
                {
                    throw new InvalidOperationException("Startup loading window remained visible after completion.");
                }

                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
                window = new OpenVisionStartupLoadingWindow();
                window.ShowReady();
                PlaceWindowOnLeftmostMonitor(window);
                Pump(8);
                if (!string.Equals(window.LoadingTitleForTest, "Preparing OpenVisionLab", StringComparison.Ordinal)
                    || !window.LoadingDetailForTest.Contains("last recipe", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"English startup loading copy mismatch. Title='{window.LoadingTitleForTest}', Detail='{window.LoadingDetailForTest}'");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_startup_loading_en.png"));
                window.Complete();
                window = null;

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: startup-loading-feedback" + Environment.NewLine
                    + "Monitor: " + monitor + Environment.NewLine
                    + "KoreanLocalized: True" + Environment.NewLine
                    + "EnglishLocalized: True" + Environment.NewLine
                    + "CloseBlockedUntilComplete: True" + Environment.NewLine
                    + "Screenshots: 01_startup_loading_ko.png; 02_startup_loading_en.png" + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                window?.Complete();
                app.Shutdown();
            }
        }

        private static void RunToolPreviewPopout(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string pinsPath = Path.Combine(
                FindRepositoryRoot(),
                "docs",
                "samples",
                "public",
                "Line_Pins_Synthetic_OK.png");
            if (!File.Exists(pinsPath))
            {
                throw new FileNotFoundException("Public pin measurement sample image was not found.", pinsPath);
            }

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                window = new OpenVisionShellHostWindow(ApplicationRuntimeContext.CreateDefault())
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                app.MainWindow = window;
                window.Show();
                Pump(24);
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                using (Bitmap pinsBitmap = new Bitmap(pinsPath))
                {
                    shellHost.SetMainLayerImageForTest(pinsBitmap);
                }

                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(60);
                OpenVisionFloatingToolWindow toolWindow = Application.Current.Windows
                    .OfType<OpenVisionFloatingToolWindow>()
                    .FirstOrDefault(item => item.IsVisible
                        && FindVisualChildren<LineToolWpfView>(item).Any())
                    ?? throw new InvalidOperationException("Line Tool window was not found.");
                string toolMonitorEvidence = PlaceWindowOnLeftmostMonitor(toolWindow);

                ConfigurePinsVerticalDistanceLine(shellHost, "Line A", "X_LTOR");
                ConfigurePinsVerticalDistanceLine(shellHost, "Line B", "X_RTOL");
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveLinePurposeForTest("Edge");
                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                VisionToolInlinePreviewSlot inputSlot = FindNamedPreviewSlot(toolWindow, "imgInputPreview");
                VisionToolInlinePreviewSlot outputSlot = FindNamedPreviewSlot(toolWindow, "imgOutputPreview");
                int previewRunsBeforeOpen = shellHost.NativePreviewRunCount;
                int layerCountBeforeOpen = shellHost.LayerDocumentCount;
                string inputRouteBeforeOpen = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBeforeOpen = shellHost.ActiveNativeRouteOutputLayerNameForTest;

                RaisePreviewDoubleClick(outputSlot);
                Pump(20);
                OpenVisionFloatingToolWindow previewWindow = FindToolPreviewWindow();
                string previewMonitorEvidence = PlaceWindowOnLeftmostMonitor(previewWindow);
                OpenVisionLayerViewerView previewViewer = FindVisualChildren<OpenVisionLayerViewerView>(previewWindow).Single();
                if (shellHost.OpenLayerViewerWindowCount != 1
                    || !previewViewer.HasImage
                    || previewWindow.Title.IndexOf("출력", StringComparison.Ordinal) < 0
                    || previewWindow.Title.IndexOf("Line_Preview", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Output thumbnail did not open the localized large viewer.");
                }

                AssertToolPreviewOpenSideEffects(
                    shellHost,
                    previewRunsBeforeOpen,
                    layerCountBeforeOpen,
                    inputRouteBeforeOpen,
                    outputRouteBeforeOpen,
                    "floating output open");
                string edgeImagePath = Path.Combine(outputDirectory, "ToolPreview_Output_Edge.png");
                if (!previewViewer.SaveImageToFileForTest(edgeImagePath))
                {
                    throw new InvalidOperationException("Large output viewer did not save the Edge image.");
                }

                shellHost.SetActiveLinePurposeForTest("Measure");
                shellHost.RunActiveNativePreviewForTest();
                Pump(48);
                string measureImagePath = Path.Combine(outputDirectory, "ToolPreview_Output_Measure.png");
                if (!previewViewer.SaveImageToFileForTest(measureImagePath)
                    || string.Equals(
                        ComputeC9FileSha256(edgeImagePath),
                        ComputeC9FileSha256(measureImagePath),
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Open output viewer did not refresh after explicit Preview.");
                }

                int viewerCountBeforeInput = shellHost.OpenLayerViewerWindowCount;
                RaisePreviewDoubleClick(inputSlot);
                Pump(16);
                OpenVisionFloatingToolWindow reusedWindow = FindToolPreviewWindow();
                if (!ReferenceEquals(previewWindow, reusedWindow)
                    || shellHost.OpenLayerViewerWindowCount != viewerCountBeforeInput
                    || reusedWindow.Title.IndexOf("입력", StringComparison.Ordinal) < 0
                    || reusedWindow.Title.IndexOf("Main", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Input thumbnail did not reuse the same large viewer.");
                }

                SaveWindowScreenshot(
                    reusedWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_ToolPreview_Input_Large.png"));
                RaisePreviewDoubleClick(outputSlot);
                Pump(16);
                SaveWindowScreenshot(
                    reusedWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_ToolPreview_Output_Large.png"));

                reusedWindow.Close();
                Pump(12);
                if (shellHost.OpenLayerViewerWindowCount != 0
                    || !shellHost.DockActiveWpfToolWindowForTest())
                {
                    throw new InvalidOperationException("Tool preview viewer did not close before the docked check.");
                }

                Pump(24);
                VisionToolInlinePreviewSlot dockedOutputSlot = FindNamedPreviewSlot(window, "imgOutputPreview");
                RaisePreviewDoubleClick(dockedOutputSlot);
                Pump(16);
                OpenVisionFloatingToolWindow dockedPreviewWindow = FindToolPreviewWindow();
                string dockedPreviewMonitorEvidence = PlaceWindowOnLeftmostMonitor(dockedPreviewWindow);
                if (shellHost.OpenLayerViewerWindowCount != 1
                    || dockedPreviewWindow.Title.IndexOf("출력", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Docked Tool output thumbnail did not open the large viewer.");
                }

                SaveWindowScreenshot(
                    dockedPreviewWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_ToolPreview_Docked_Output_Large.png"));

                using (Bitmap secondInput = new Bitmap(pinsPath))
                {
                    if (!shellHost.AddLayerImageForTest("Arithmetic_B", secondInput))
                    {
                        throw new InvalidOperationException("Arithmetic second input layer could not be created.");
                    }
                }

                shellHost.SelectToolForTest(VISION_MENU.Arithmetic);
                Pump(60);
                if (shellHost.OpenLayerViewerWindowCount != 0)
                {
                    throw new InvalidOperationException("Changing tools did not close the previous large preview viewer.");
                }

                ArithmeticToolWpfView arithmeticView = FindVisualChildren<ArithmeticToolWpfView>(window).Single();
                arithmeticView.ApplyPersistedSettings(new ArithmeticToolSettings
                {
                    SelectedOperation = "ABSDIFF",
                    UseConstantInput = false,
                    UseColorConstant = false,
                    UseOffsetMode = false
                });
                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                VisionToolInlinePreviewSlot inputASlot = FindNamedPreviewSlot(window, "imgInputA");
                VisionToolInlinePreviewSlot inputBSlot = FindNamedPreviewSlot(window, "imgInputB");
                VisionToolInlinePreviewSlot arithmeticOutputSlot = FindNamedPreviewSlot(window, "imgOutputPreview");
                RaisePreviewDoubleClick(inputASlot);
                Pump(12);
                OpenVisionFloatingToolWindow arithmeticPreviewWindow = FindToolPreviewWindow();
                if (arithmeticPreviewWindow.Title.IndexOf("입력 A", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Arithmetic Input A did not open the large viewer.");
                }

                RaisePreviewDoubleClick(inputBSlot);
                Pump(12);
                if (!ReferenceEquals(arithmeticPreviewWindow, FindToolPreviewWindow())
                    || arithmeticPreviewWindow.Title.IndexOf("입력 B", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Arithmetic Input B did not reuse the large viewer.");
                }

                SaveWindowScreenshot(
                    arithmeticPreviewWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_ToolPreview_Arithmetic_InputB_Large.png"));
                int previewRunsBeforeLanguageSwitch = shellHost.NativePreviewRunCount;
                int layerCountBeforeLanguageSwitch = shellHost.LayerDocumentCount;
                string inputRouteBeforeLanguageSwitch = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBeforeLanguageSwitch = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
                Pump(12);
                if (arithmeticPreviewWindow.Title.IndexOf("Arithmetic", StringComparison.Ordinal) < 0
                    || arithmeticPreviewWindow.Title.IndexOf("Input B", StringComparison.Ordinal) < 0)
                {
                    throw new InvalidOperationException("Open Tool preview viewer did not follow the English UI language.");
                }

                AssertToolPreviewOpenSideEffects(
                    shellHost,
                    previewRunsBeforeLanguageSwitch,
                    layerCountBeforeLanguageSwitch,
                    inputRouteBeforeLanguageSwitch,
                    outputRouteBeforeLanguageSwitch,
                    "language switch");
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
                Pump(12);
                RaisePreviewDoubleClick(arithmeticOutputSlot);
                Pump(12);
                if (!ReferenceEquals(arithmeticPreviewWindow, FindToolPreviewWindow())
                    || arithmeticPreviewWindow.Title.IndexOf("출력", StringComparison.Ordinal) < 0
                    || shellHost.OpenLayerViewerWindowCount != 1)
                {
                    throw new InvalidOperationException("Arithmetic Output did not reuse the large viewer.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: tool-preview-popout" + Environment.NewLine
                    + "InputOutputWindowReuse: true" + Environment.NewLine
                    + "ExplicitPreviewRefresh: true" + Environment.NewLine
                    + "FloatingAndDocked: true" + Environment.NewLine
                    + "ArithmeticInputAInputBOutput: true" + Environment.NewLine
                    + "KoreanEnglishLiveLocalization: true" + Environment.NewLine
                    + "PreviewRunSideEffects: none" + Environment.NewLine
                    + "LayerRouteSideEffects: none" + Environment.NewLine
                    + monitorEvidence + Environment.NewLine
                    + "Tool" + toolMonitorEvidence + Environment.NewLine
                    + "Preview" + previewMonitorEvidence + Environment.NewLine
                    + "DockedPreview" + dockedPreviewMonitorEvidence,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static VisionToolInlinePreviewSlot FindNamedPreviewSlot(DependencyObject root, string name)
        {
            return FindVisualChildren<VisionToolInlinePreviewSlot>(root)
                .FirstOrDefault(slot => string.Equals(slot.Name, name, StringComparison.Ordinal))
                ?? throw new InvalidOperationException("Tool preview slot was not found: " + name);
        }

        private static OpenVisionFloatingToolWindow FindToolPreviewWindow()
        {
            return Application.Current.Windows
                .OfType<OpenVisionFloatingToolWindow>()
                .SingleOrDefault(item => item.IsVisible
                    && FindVisualChildren<OpenVisionLayerViewerView>(item).Any())
                ?? throw new InvalidOperationException("Large Tool preview window was not found.");
        }

        private static void RaisePreviewDoubleClick(VisionToolInlinePreviewSlot slot)
        {
            slot.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
            {
                RoutedEvent = Control.MouseDoubleClickEvent
            });
        }

        private static void AssertToolPreviewOpenSideEffects(
            OpenVisionShellHostView shellHost,
            int previewRunCount,
            int layerCount,
            string inputRoute,
            string outputRoute,
            string action)
        {
            if (shellHost.NativePreviewRunCount != previewRunCount
                || shellHost.LayerDocumentCount != layerCount
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRoute, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRoute, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Tool preview " + action + " changed Preview/Run, layers, or routing.");
            }
        }

        private static void AssertLineSignalReviewSideEffectsUnchanged(
            OpenVisionShellHostView shellHost,
            int previewRunCount,
            int layerCount,
            string activeLayer,
            string inputRoute,
            string outputRoute,
            string action)
        {
            if (shellHost.NativePreviewRunCount != previewRunCount
                || shellHost.LayerDocumentCount != layerCount
                || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayer, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRoute, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRoute, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Line " + action + " changed Preview/Run, layer, active-layer, or routing state.");
            }
        }

        private static void RunTutorialCaptures(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string workspacePath = Path.Combine(repoRoot, "docs", "samples", "public", "Workspace_Inspection_Synthetic_OK.png");
            string blobPath = Path.Combine(repoRoot, "docs", "samples", "public", "Blob_Particles_Synthetic_OK.png");
            string linePath = Path.Combine(repoRoot, "docs", "samples", "public", "Line_Pins_Synthetic_OK.png");
            string diePadPath = Path.Combine(repoRoot, "docs", "samples", "public", "Matching_DiePad_Synthetic_OK.png");
            string matchingTemplatePath = Path.Combine(repoRoot, "docs", "samples", "public", "templates", "Matching_DiePad_Synthetic_Template.png");

            EnsureFileExists(workspacePath, "Tutorial workspace synthetic sample");
            EnsureFileExists(blobPath, "Tutorial blob synthetic sample");
            EnsureFileExists(linePath, "Tutorial line synthetic sample");
            EnsureFileExists(diePadPath, "Tutorial Die Pad matching sample");
            EnsureFileExists(matchingTemplatePath, "Tutorial Die Pad matching template");

            const string tutorialRecipeName = "Documentation_Public";
            const string tutorialPipelineName = "Public_Synthetic_Matching";
            VisionPipelineStorage.Save(
                tutorialRecipeName,
                new VisionPipeline
                {
                    Name = tutorialPipelineName
                });
            VisionPipelineStorage.SaveActivePipelineName(tutorialRecipeName, tutorialPipelineName);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            StringBuilder report = new StringBuilder();
            string reportPath = Path.Combine(outputDirectory, "report.txt");
            report.AppendLine("Result: PASS");
            report.AppendLine("Scenario: tutorial-captures");
            report.AppendLine("Rule: Captures are generated from the current OpenVisionLab EXE.");
            report.AppendLine("SmokeBuild: tutorial-captures-public-sample-catalog-v4");
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                shellHost.SwitchRecipeContextForTest(tutorialRecipeName);
                Pump(40);

                if (!shellHost.LoadMainImageFromFileForTest(workspacePath))
                {
                    throw new InvalidOperationException("Tutorial workspace synthetic image could not be loaded: " + workspacePath);
                }

                Pump(80);
                SaveTutorialScreenshot(window, outputDirectory, "01_main_workspace_current.png", report, reportPath);

                shellHost.SetShellLogExpandedForTest(false);
                Pump(20);
                if (shellHost.IsShellLogExpandedForTest)
                {
                    throw new InvalidOperationException("Tutorial Run Log collapsed capture expected the shell log to be collapsed.");
                }

                SaveTutorialScreenshot(window, outputDirectory, "08_run_log_collapsed_current.png", report, reportPath);

                shellHost.SetShellLogExpandedForTest(true);
                Pump(30);
                if (!shellHost.IsShellLogExpandedForTest)
                {
                    throw new InvalidOperationException("Tutorial Run Log open capture expected the shell log to be expanded.");
                }

                SaveTutorialScreenshot(window, outputDirectory, "09_run_log_open_current.png", report, reportPath);

                shellHost.SetShellLogExpandedForTest(false);
                Pump(20);

                PrepareDockingVerificationLayers(shellHost);
                if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Tutorial capture could not dock Main and HSV_Preview layers.");
                }

                Pump(60);
                SaveTutorialScreenshot(window, outputDirectory, "03_layer_docking_current.png", report, reportPath);

                shellHost.ClearDockedLayersForTest();
                Pump(20);
                if (shellHost.HasLayerForTest("HSV_Preview"))
                {
                    shellHost.DeleteLayerForTest("HSV_Preview");
                    Pump(20);
                }

                report.AppendLine("Step: matching preview capture");
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

                if (!shellHost.LoadMainImageFromFileForTest(diePadPath))
                {
                    throw new InvalidOperationException("Tutorial Matching Die Pad image could not be loaded: " + diePadPath);
                }

                Pump(30);
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(80);
                shellHost.SetActiveMatchingTemplatePathForTest(matchingTemplatePath);
                Pump(20);
                shellHost.ConfigureActiveMatchingForTest(ConfigureTutorialMatchingProperty);
                Pump(20);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);
                string matchingStatus = shellHost.ActiveNativeStatusText;
                string matchingReview = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || matchingReview.IndexOf("Template Match", StringComparison.OrdinalIgnoreCase) < 0
                    || (matchingReview.IndexOf("Score", StringComparison.OrdinalIgnoreCase) < 0
                        && matchingReview.IndexOf("점수", StringComparison.OrdinalIgnoreCase) < 0)
                    || (matchingStatus.IndexOf("Preview OK", StringComparison.OrdinalIgnoreCase) < 0
                        && matchingReview.IndexOf("미리보기 OK", StringComparison.OrdinalIgnoreCase) < 0))
                {
                    throw new InvalidOperationException(
                        "Tutorial Matching preview did not produce a documented actual result."
                        + Environment.NewLine
                        + "Status: " + matchingStatus
                        + Environment.NewLine
                        + "Review: " + matchingReview);
                }

                using (Bitmap matchingPreview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
                {
                    if (matchingPreview == null)
                    {
                        throw new InvalidOperationException("Tutorial Matching preview output layer was not created.");
                    }

                    matchingPreview.Save(Path.Combine(outputDirectory, "matching_preview_actual_current.png"));
                    report.AppendLine("Captured: matching_preview_actual_current.png");
                    File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
                }

                VisionPipelineStep matchingStep = shellHost.AddActiveNativePipelineStepForTest();
                if (matchingStep == null)
                {
                    throw new InvalidOperationException("Tutorial Matching preview could not be added to the active pipeline.");
                }

                Pump(30);
                shellHost.OpenSamplePipelineForTest();
                Pump(80);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "tutorial pipeline review");
                Pump(100);
                SaveTutorialScreenshot(window, outputDirectory, "02_pipeline_review_current.png", report, reportPath);

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(80);

                if (!shellHost.IsDockedToolInspectorVisibleForTest)
                {
                    MoveCursorInsideWindow(window, 24D, 24D);
                    Pump(12);
                    shellHost.DockActiveWpfToolWindowForTest();
                    Pump(60);
                }

                SaveTutorialScreenshot(window, outputDirectory, "04_matching_tool_current.png", report, reportPath);

                report.AppendLine("Step: blob preview capture");
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

                if (!shellHost.LoadMainImageFromFileForTest(blobPath))
                {
                    throw new InvalidOperationException("Tutorial Blob synthetic image could not be loaded: " + blobPath);
                }

                Pump(30);
                shellHost.SelectToolForTest(VISION_MENU.Blob);
                Pump(80);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);
                if (!shellHost.IsDockedToolInspectorVisibleForTest)
                {
                    shellHost.DockActiveWpfToolWindowForTest();
                    Pump(60);
                }

                SaveTutorialScreenshot(window, outputDirectory, "05_blob_tool_current.png", report, reportPath);

                report.AppendLine("Step: line preview capture");
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

                if (!shellHost.LoadMainImageFromFileForTest(linePath))
                {
                    throw new InvalidOperationException("Tutorial Line synthetic image could not be loaded: " + linePath);
                }

                Pump(30);
                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(80);
                ConfigurePinsVerticalDistanceLine(shellHost, "Line A", "X_LTOR");
                ConfigurePinsVerticalDistanceLine(shellHost, "Line B", "X_RTOL");
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveLinePurposeForTest("Measure");
                Pump(24);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);
                if (!shellHost.IsDockedToolInspectorVisibleForTest)
                {
                    shellHost.DockActiveWpfToolWindowForTest();
                    Pump(60);
                }

                SaveTutorialScreenshot(window, outputDirectory, "06_line_tool_current.png", report, reportPath);

                CaptureTutorialSampleCatalogWindow(outputDirectory, report, reportPath);

                ValidateTutorialCaptureFiles(outputDirectory, report, reportPath);
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunPublicFixtureReview(string outputDirectory)
        {
            const string sampleName = "Public_Fixture_Pad_Good";
            string recipeName = "Smoke_PublicFixtureReview_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string baselinePipelineName = "Direct_PublicFixtureReview_Baseline";
            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(baselinePipelineName, 1));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, baselinePipelineName);
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            StringBuilder report = new StringBuilder();
            string reportPath = Path.Combine(outputDirectory, "report.txt");
            report.AppendLine("Result: PASS");
            report.AppendLine("Scenario: public-fixture-review");
            report.AppendLine("Executable: " + (Environment.ProcessPath ?? "-"));
            report.AppendLine("Rule: sample open and Pipeline Review run are explicit smoke actions.");
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(40);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(40);
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                if (!shellHost.OpenWorkspaceSampleForTest(sampleName))
                {
                    throw new InvalidOperationException("Public Fixture sample could not be opened: " + sampleName);
                }

                Pump(100);
                if (!shellHost.CanOpenSamplePipelineForTest
                    || shellHost.ActivePipelineStepCountForTest != 3
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException(
                        "Public Fixture sample open did not prepare a three-step pipeline without Preview side effects. "
                        + $"CanOpen={shellHost.CanOpenSamplePipelineForTest}, Steps={shellHost.ActivePipelineStepCountForTest}, "
                        + $"RunsBefore={previewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
                }

                shellHost.OpenSamplePipelineForTest();
                Pump(120);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "public Fixture Pipeline Review");
                Pump(120);
                shellHost.SelectPipelineReviewStepForTest(1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(80);

                string reviewText = string.Join(
                    " | ",
                    shellHost.PipelineReviewResultSummaryText,
                    shellHost.PipelineReviewResultDetailText,
                    shellHost.PipelineReviewRunLogText,
                    shellHost.PipelineReviewGuidePairText);
                if (!shellHost.PipelineReviewSelectedStepName.Contains("02  Inspect Fixture Pad", StringComparison.OrdinalIgnoreCase)
                    || shellHost.PipelineReviewSelectedStepName.Contains("02  02", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewGuideCurrentStepText.Contains("02 Inspect Fixture Pad", StringComparison.OrdinalIgnoreCase)
                    || shellHost.PipelineReviewGuideCurrentStepText.Contains("02 02", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewGuideCurrentStepText.Contains("Main -> FixturePadBlob", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewFlowSummaryText.Contains("FixtureMatch", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewParameterSummaryText.Contains("CvROI: 320,180,60,50", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewParameterSummaryText.Contains("FIXTURE_FRAME_NAME: PartFrame", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewResultSummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewResultSummaryText.Contains("ms", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewResultDetailText.Contains("Fixture", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.PipelineReviewResultDetailText.Contains("80", StringComparison.Ordinal)
                    || !shellHost.PipelineReviewResultDetailText.Contains("55", StringComparison.Ordinal)
                    || !shellHost.PipelineReviewResultDetailText.Contains("400", StringComparison.Ordinal)
                    || !shellHost.PipelineReviewResultDetailText.Contains("235", StringComparison.Ordinal)
                    || !shellHost.PipelineReviewGuidePairText.Contains("Public_Fixture_Pad_Missing_Bad", StringComparison.Ordinal)
                    || !shellHost.HasPipelineReviewInputPreview
                    || !shellHost.HasPipelineReviewOutputPreview
                    || shellHost.CanSelectFirstIssuePipelineReviewStepForTest)
                {
                    throw new InvalidOperationException(
                        "Current EXE Fixture Pipeline Review did not expose its OK decision, pose evidence, pair reference, and output preview. "
                        + $"Selected='{shellHost.PipelineReviewSelectedStepName}', GuideStep='{shellHost.PipelineReviewGuideCurrentStepText}', "
                        + $"Input={shellHost.HasPipelineReviewInputPreview}, Output={shellHost.HasPipelineReviewOutputPreview}, "
                        + $"FirstIssue={shellHost.CanSelectFirstIssuePipelineReviewStepForTest}, "
                        + $"Parameters='{shellHost.PipelineReviewParameterSummaryText}', Review='{reviewText}'");
                }

                Window reviewWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(item => item.IsVisible && IsFloatingToolWindow(item));
                if (reviewWindow == null)
                {
                    throw new InvalidOperationException("Current EXE Fixture Pipeline Review floating window was not found for capture.");
                }

                AssertVisibleAutomationIds(
                    reviewWindow,
                    "Current EXE Fixture selected-tool Learn entry",
                    "PipelineReviewOpenSelectedToolLearnButton");
                int previewRunsBeforeLearn = shellHost.NativePreviewRunCount;
                int layerCountBeforeLearn = shellHost.LayerDocumentCount;
                string activeLayerBeforeLearn = shellHost.ActiveHostLayerTitle;
                string routeInputBeforeLearn = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string routeOutputBeforeLearn = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                string selectedStepBeforeLearn = shellHost.PipelineReviewSelectedStepName;
                string parameterSummaryBeforeLearn = shellHost.PipelineReviewParameterSummaryText;
                string resultSummaryBeforeLearn = shellHost.PipelineReviewResultSummaryText;
                string resultDetailBeforeLearn = shellHost.PipelineReviewResultDetailText;
                string executionStateBeforeLearn = shellHost.PipelineReviewExecutionState;
                Button learnButton = FindVisualChildren<Button>(reviewWindow)
                    .First(item => item.IsVisible && string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "PipelineReviewOpenSelectedToolLearnButton",
                        StringComparison.Ordinal));
                learnButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, learnButton));
                Pump(40);

                OpenVisionLearnWindow learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible);
                if (learnWindow == null
                    || learnWindow.SelectedTopicIndexForTest != (int)OpenVisionLearnTopicIndex.Blob)
                {
                    throw new InvalidOperationException(
                        "Current EXE Pipeline Review selected-tool Learn entry did not open the Blob topic. "
                        + $"Window={learnWindow != null}, Topic={learnWindow?.SelectedTopicIndexForTest}");
                }

                SaveTutorialScreenshot(
                    learnWindow,
                    outputDirectory,
                    "public_fixture_blob_learn_current_exe.png",
                    report,
                    reportPath);

                learnWindow.Close();
                Pump(24);
                reviewWindow.Activate();
                Pump(20);
                if (shellHost.NativePreviewRunCount != previewRunsBeforeLearn
                    || shellHost.LayerDocumentCount != layerCountBeforeLearn
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, routeInputBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, routeOutputBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewSelectedStepName, selectedStepBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewParameterSummaryText, parameterSummaryBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewResultSummaryText, resultSummaryBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewResultDetailText, resultDetailBeforeLearn, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewExecutionState, executionStateBeforeLearn, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Current EXE Pipeline Review selected-tool Learn entry changed review or workspace state. "
                        + $"Runs={shellHost.NativePreviewRunCount}/{previewRunsBeforeLearn}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBeforeLearn}, "
                        + $"Active='{shellHost.ActiveHostLayerTitle}'/'{activeLayerBeforeLearn}', "
                        + $"Route='{shellHost.ActiveNativeRouteInputLayerNameForTest}->{shellHost.ActiveNativeRouteOutputLayerNameForTest}'/"
                        + $"'{routeInputBeforeLearn}->{routeOutputBeforeLearn}', "
                        + $"Step='{shellHost.PipelineReviewSelectedStepName}'/'{selectedStepBeforeLearn}', "
                        + $"Execution='{shellHost.PipelineReviewExecutionState}'/'{executionStateBeforeLearn}'");
                }

                SaveTutorialScreenshot(
                    reviewWindow,
                    outputDirectory,
                    "public_fixture_pipeline_review_current_exe.png",
                    report,
                    reportPath);

                const int originalMinArea = 700;
                const int editedMinArea = 750;
                const string badSampleName = "Public_Fixture_Pad_Missing_Bad";
                string fixturePipelineName = shellHost.ActivePipelineNameForTest;
                int editRunsBefore = shellHost.NativePreviewRunCount;
                int editLayersBefore = shellHost.LayerDocumentCount;
                string editActiveLayerBefore = shellHost.ActiveHostLayerTitle;
                string editRouteInputBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string editRouteOutputBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;

                Button editSelectedStepButton = FindNamedVisualChild<Button>(
                    reviewWindow,
                    "btnEditSelectedStep",
                    "Current EXE Fixture Step edit handoff");
                editSelectedStepButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, editSelectedStepButton));
                Pump(180);

                BlobProperty editProperty = shellHost.RecipeCommands.SelectedStepEditObject as BlobProperty
                    ?? throw new InvalidOperationException("Current EXE Fixture Step did not load the Blob PropertyGrid editor.");
                if (editProperty.MIN_AREA != originalMinArea
                    || !string.Equals(
                        shellHost.RecipeCommands.SelectedSampleOption?.SampleName,
                        sampleName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Current EXE Fixture Step edit handoff lost the source sample context. "
                        + $"MIN_AREA={editProperty.MIN_AREA}/{originalMinArea}, "
                        + $"Sample='{shellHost.RecipeCommands.SelectedSampleOption?.SampleName}'/'{sampleName}'");
                }

                TextBox[] minAreaEditors = FindVisualChildren<TextBox>(shellHost)
                    .Where(item => item.IsVisible
                        && item.IsEnabled
                        && string.Equals(
                            item.Text,
                            originalMinArea.ToString(CultureInfo.InvariantCulture),
                            StringComparison.Ordinal))
                    .ToArray();
                if (minAreaEditors.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"Current EXE Fixture MIN_AREA editor was not uniquely visible. Count={minAreaEditors.Length}");
                }

                TextBox minAreaEditor = minAreaEditors[0];
                minAreaEditor.Focus();
                minAreaEditor.Text = editedMinArea.ToString(CultureInfo.InvariantCulture);
                Pump(40);
                Button applyButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => item.IsVisible
                        && string.Equals(
                            System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                            "HostRecipeApplySelectedStepParametersButton",
                            StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Current EXE Fixture XML apply button was not visible.");
                if (!applyButton.Focus())
                {
                    throw new InvalidOperationException("Current EXE Fixture XML apply button could not receive focus.");
                }

                Pump(80);
                if (editProperty.MIN_AREA != editedMinArea
                    || !shellHost.RecipeCommands.IsSelectedStepEditDirty
                    || shellHost.NativePreviewRunCount != editRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Current EXE Fixture PropertyGrid edit did not wait for explicit XML apply. "
                        + $"MIN_AREA={editProperty.MIN_AREA}/{editedMinArea}, "
                        + $"Dirty={shellHost.RecipeCommands.IsSelectedStepEditDirty}, "
                        + $"Runs={shellHost.NativePreviewRunCount}/{editRunsBefore}");
                }

                if (applyButton.Command == null
                    || !applyButton.Command.CanExecute(applyButton.CommandParameter))
                {
                    throw new InvalidOperationException("Current EXE Fixture XML apply command was disabled.");
                }

                applyButton.Command.Execute(applyButton.CommandParameter);
                Pump(260);
                VisionPipeline appliedPipeline = VisionPipelineStorage.Load(recipeName, fixturePipelineName);
                string appliedMinArea = appliedPipeline.Steps[1].Parameters["MIN_AREA"];
                if (!string.Equals(
                        appliedMinArea,
                        editedMinArea.ToString(CultureInfo.InvariantCulture),
                        StringComparison.Ordinal)
                    || shellHost.RecipeCommands.IsSelectedStepEditDirty
                    || shellHost.NativePreviewRunCount != editRunsBefore
                    || shellHost.LayerDocumentCount != editLayersBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, editActiveLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, editRouteInputBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, editRouteOutputBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Current EXE Fixture XML apply changed execution, layer, or routing state. "
                        + $"MIN_AREA={appliedMinArea}/{editedMinArea}, Dirty={shellHost.RecipeCommands.IsSelectedStepEditDirty}, "
                        + $"Runs={shellHost.NativePreviewRunCount}/{editRunsBefore}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{editLayersBefore}, "
                        + $"Route='{shellHost.ActiveNativeRouteInputLayerNameForTest}->{shellHost.ActiveNativeRouteOutputLayerNameForTest}'/"
                        + $"'{editRouteInputBefore}->{editRouteOutputBefore}'");
                }

                ScrollViewer editScrollViewer = FindNamedVisualChild<ScrollViewer>(
                    shellHost,
                    "recipePipelineTabScrollViewer",
                    "Current EXE Fixture Step edit capture");
                editScrollViewer.ScrollToEnd();
                Pump(80);
                SaveTutorialScreenshot(
                    window,
                    outputDirectory,
                    "public_fixture_step_edit_applied_current_exe.png",
                    report,
                    reportPath);

                if (!shellHost.RecipeCommands.RunSelectedSamplePairCheckCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Current EXE Fixture Good/Bad rerun command was disabled.");
                }

                OpenVisionRecipePairRunSummary pairSummaryBeforeRerun = shellHost.RecipeCommands.LatestPairRunSummary;
                shellHost.RecipeCommands.RunSelectedSamplePairCheckCommand.Execute(null);
                Stopwatch rerunStopwatch = Stopwatch.StartNew();
                while (ReferenceEquals(shellHost.RecipeCommands.LatestPairRunSummary, pairSummaryBeforeRerun)
                    || !shellHost.RecipeCommands.LatestPairRunSummary.HasResult)
                {
                    Pump(8);
                    Thread.Sleep(20);
                    if (rerunStopwatch.Elapsed > TimeSpan.FromSeconds(30))
                    {
                        throw new TimeoutException("Current EXE Fixture Good/Bad rerun did not complete within 30 seconds.");
                    }
                }

                OpenVisionRecipePairRunSummary pairSummary = shellHost.RecipeCommands.LatestPairRunSummary;
                if (!pairSummary.StatusText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                    || pairSummary.SampleResults.Count != 2
                    || !pairSummary.SampleResults.Any(result => string.Equals(result.SampleName, sampleName, StringComparison.OrdinalIgnoreCase))
                    || !pairSummary.SampleResults.Any(result => string.Equals(result.SampleName, badSampleName, StringComparison.OrdinalIgnoreCase))
                    || shellHost.NativePreviewRunCount != editRunsBefore
                    || shellHost.LayerDocumentCount != editLayersBefore)
                {
                    throw new InvalidOperationException(
                        "Current EXE Fixture Good/Bad rerun did not use the aligned Fixture pair. "
                        + $"Status='{pairSummary.StatusText}', "
                        + $"Samples='{string.Join(", ", pairSummary.SampleResults.Select(result => result.SampleName))}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{editRunsBefore}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{editLayersBefore}");
                }

                TabItem pipelineTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipeline",
                    "Current EXE Fixture Good/Bad rerun");
                TabItem pipelineReviewTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineReview",
                    "Current EXE Fixture Good/Bad rerun");
                pipelineTab.IsSelected = true;
                pipelineReviewTab.IsSelected = true;
                Pump(80);
                const string pairCaptureName = "public_fixture_pair_rerun_current_exe.png";
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, pairCaptureName));
                report.AppendLine("Screenshot: " + pairCaptureName);

                report.AppendLine("Sample: " + sampleName);
                report.AppendLine("Pipeline: " + fixturePipelineName);
                report.AppendLine("Selected Step: " + selectedStepBeforeLearn);
                report.AppendLine("Result: " + resultSummaryBeforeLearn);
                report.AppendLine("Fixture: " + resultDetailBeforeLearn);
                report.AppendLine("Context Learn: Blob topic opened without Preview, layer, routing, parameter, or review-state changes.");
                report.AppendLine("Step Edit: MIN_AREA " + originalMinArea.ToString(CultureInfo.InvariantCulture)
                    + " -> " + editedMinArea.ToString(CultureInfo.InvariantCulture) + " applied to XML.");
                report.AppendLine("Work Sample: " + shellHost.RecipeCommands.SelectedSampleOption?.SampleName);
                report.AppendLine("Pair Rerun: " + pairSummary.StatusText + " / "
                    + string.Join(", ", pairSummary.SampleResults.Select(result => result.SampleName)));
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void CaptureTutorialSampleCatalogWindow(string outputDirectory, StringBuilder report, string reportPath)
        {
            report.AppendLine("Step: public sample catalog capture");
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);

            List<VisionPipelineSampleCatalogItem> publicSamples = VisionPipelineSampleCatalogItem.LoadRunnable()
                .Where(item => item.CanOpen && item.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Public)
                .ToList();
            if (publicSamples.Count < 16)
            {
                throw new InvalidOperationException(
                    "Tutorial public sample catalog capture expected at least 16 public runnable samples. Count=" + publicSamples.Count);
            }

            VisionPipelineSampleCatalogItem selectedSample = publicSamples.FirstOrDefault(item =>
                    string.Equals(item.SampleName, "Public_Edge_Fiducial_Good", StringComparison.OrdinalIgnoreCase))
                ?? publicSamples.FirstOrDefault(item => item.HasPair
                    && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase))
                ?? publicSamples[0];

            OpenVisionWorkspaceSamplePickerViewModel viewModel = new OpenVisionWorkspaceSamplePickerViewModel(publicSamples)
            {
                SelectedSample = selectedSample
            };

            OpenVisionWorkspaceSamplePickerWindow sampleWindow = new OpenVisionWorkspaceSamplePickerWindow(viewModel)
            {
                Width = 1040,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            try
            {
                sampleWindow.Show();
                Pump(80);
                SaveTutorialScreenshot(sampleWindow, outputDirectory, "07_sample_catalog_public_current.png", report, reportPath);

                string visibleSummary = string.Join(" | ", publicSamples
                    .GroupBy(item => item.PairGroup ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .Where(group => !string.IsNullOrWhiteSpace(group.Key))
                    .Select(group => group.Key)
                    .OrderBy(group => group, StringComparer.OrdinalIgnoreCase));
                report.AppendLine("PublicCatalogSamples: " + publicSamples.Count.ToString(CultureInfo.InvariantCulture));
                report.AppendLine("PublicCatalogSelected: " + selectedSample.SampleName);
                report.AppendLine("PublicCatalogPairGroups: " + visibleSummary);
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
            }
            finally
            {
                sampleWindow.Close();
            }
        }

        private static void RunToolDockFloatCycle(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap bitmap = CreateDockFloatSmokeBitmap())
                {
                    shellHost.SetMainLayerImageForTest(bitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.Blob);
                Pump(40);
                AssertToolWindowState(shellHost, "BlobToolWpfView", false, 1, "Blob initial floating");

                if (!shellHost.DockActiveWpfToolWindowForTest())
                {
                    throw new InvalidOperationException("Blob floating tool did not accept the dock-to-right request.");
                }

                Pump(40);
                AssertToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after dock");

                shellHost.SetDockedToolInspectorWidthForTest(680D);
                Pump(8);
                AssertDockedWidth(shellHost, 680D, "Blob adjusted dock width");

                if (!shellHost.FloatDockedWpfToolWindowForTest())
                {
                    throw new InvalidOperationException("Docked Blob tool did not accept the float request.");
                }

                Pump(40);
                AssertToolWindowState(shellHost, "BlobToolWpfView", false, 1, "Blob after float");

                if (!shellHost.DockActiveWpfToolWindowForTest())
                {
                    throw new InvalidOperationException("Floated Blob tool did not accept the second dock request.");
                }

                Pump(40);
                AssertToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after re-dock");
                AssertDockedWidth(shellHost, 680D, "Blob re-dock width");

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(40);
                AssertToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after switching from docked Blob");

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(24);
                AssertToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after repeated docked selection");
                AssertDockedWidth(shellHost, 680D, "Matching repeated selection width");

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_ToolDockFloatCycle.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: tool-dock-float-cycle" + Environment.NewLine
                    + "FinalTool: " + shellHost.ActiveWpfToolWindowTypeName + Environment.NewLine
                    + "Docked: " + shellHost.IsDockedToolInspectorVisibleForTest + Environment.NewLine
                    + "FloatingWindows: " + CountVisibleFloatingToolWindows().ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "DockWidth: " + shellHost.DockedToolInspectorWidthForTest.ToString("0.0", CultureInfo.InvariantCulture),
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLayerGlobalDocking(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap bitmap = CreateDockFloatSmokeBitmap())
                {
                    shellHost.SetMainLayerImageForTest(bitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.HSV);
                Pump(32);
                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                if (!shellHost.HasNativePreviewResult
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "HSV preview did not create the expected output layer before layer docking. "
                        + "WorkspaceLayer=" + shellHost.WorkspaceLayerTitle);
                }

                if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Layer global docking setup could not dock both Main and HSV_Preview.");
                }

                Pump(20);
                if (shellHost.DockedLayerPaneCount != 1)
                {
                    throw new InvalidOperationException(
                        "Layer global docking should start from a single tabbed pane before global split. "
                        + "PaneCount=" + shellHost.DockedLayerPaneCount.ToString(CultureInfo.InvariantCulture));
                }

                if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
                {
                    throw new InvalidOperationException("HSV_Preview did not accept the GlobalRight docking command.");
                }

                Pump(32);
                if (shellHost.DockedLayerCount != 2
                    || shellHost.DockedLayerPaneCount < 2
                    || shellHost.DockedLayerTextureTileCount < 2
                    || !string.Equals(shellHost.DockedLayerRootOrientationForTest, "Horizontal", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "GlobalRight did not create a workspace-level horizontal split. "
                        + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}, Orientation={shellHost.DockedLayerRootOrientationForTest}");
                }

                shellHost.CloseActiveWpfToolWindowForTest();
                Pump(20);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_LayerGlobalDocking.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: layer-global-docking" + Environment.NewLine
                    + "DockedLayers: " + shellHost.DockedLayerCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "DockedPanes: " + shellHost.DockedLayerPaneCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "DockedTiles: " + shellHost.DockedLayerTextureTileCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "RootOrientation: " + shellHost.DockedLayerRootOrientationForTest + Environment.NewLine
                    + "Titles: " + shellHost.DockedLayerTitles,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunRecipeManagerLlmIntentSkills(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            VerifyLlmTemplateDraftBuilderContract();
            CleanupKnownSmokeRecipeWorkspaces("Smoke_LlmIntentSkills_");
            string recipeName = "Smoke_LlmIntentSkills_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Direct_LlmIntentSkill_Check";
            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(pipelineName, 2));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(40);

                System.Windows.Controls.Primitives.ToggleButton recipeManagerButton =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "btnHostRecipeManager",
                        "Recipe manager LLM intent skill smoke");
                recipeManagerButton.IsChecked = true;
                Pump(60);

                System.Windows.Controls.Primitives.ToggleButton advancedReviewToggle =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "recipeAdvancedReviewToggle",
                        "Recipe manager LLM intent skill smoke");
                advancedReviewToggle.IsChecked = true;
                Pump(40);

                TabItem llmXmlTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeLlmXml",
                    "Recipe manager LLM intent skill smoke");
                llmXmlTab.IsSelected = true;
                Pump(40);

                int beforeRuns = shellHost.NativePreviewRunCount;
                OpenVisionShellHostRecipeCommandSurface commands = shellHost.RecipeCommands;

                commands.SelectedLlmToolTemplate = "Template Matching";
                Pump(20);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM intent skill baseline",
                    "HostRecipeLlmAssistantPanel",
                    "HostRecipeLlmTemplateSelector",
                    "HostRecipeLlmResultChannelContract");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM intent skill baseline",
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeNameEditor",
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeLlmPinGapIntentSkill",
                    "HostRecipeLlmBlobCountIntentSkill",
                    "HostRecipeLlmContourCountIntentSkill");

                commands.BuildLlmPromptCommand.Execute(null);
                Pump(20);
                string matchingPrompt = commands.LlmPromptText ?? string.Empty;
                if (!matchingPrompt.Contains("OpenVisionLab VisionPipeline XML draft", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("Intent contract", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("Do not run Preview/Run automatically", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("0..1 decimals", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("FIND_ANGLE_MIN", StringComparison.OrdinalIgnoreCase)
                    || !matchingPrompt.Contains("existing template/image dependency paths", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM intent skill did not build the expected template-matching prompt. "
                        + matchingPrompt);
                }

                if (shellHost.NativePreviewRunCount != beforeRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM prompt construction triggered Preview/Run. "
                        + $"Before={beforeRuns}, After={shellHost.NativePreviewRunCount}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_TemplateMatching.png"));

                TabItem browserAssistTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeLlmBrowserAssist",
                    "Recipe manager LLM browser assist smoke");
                browserAssistTab.IsSelected = true;
                Pump(30);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM browser assist",
                    "HostRecipeLlmBrowserAssistPanel",
                    "HostRecipeBrowserAssistOpenChatGptButton",
                    "HostRecipeBrowserAssistOpenExternalButton",
                    "HostRecipeBrowserAssistCopyPromptButton",
                    "HostRecipeBrowserAssistPasteXmlButton",
                    "HostRecipeBrowserAssistPlaceholder");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM browser assist",
                    "HostRecipeLlmBrowserAssistWebView");

                Button openChatGptButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostRecipeBrowserAssistOpenChatGptButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Recipe manager LLM browser assist could not find the ChatGPT open button.");
                WebView2 embeddedBrowser = FindVisualChildren<WebView2>(shellHost).FirstOrDefault()
                    ?? throw new InvalidOperationException("Recipe manager LLM browser assist did not create the embedded browser control.");
                bool chatGptNavigationCompleted = false;
                bool chatGptNavigationSucceeded = false;
                embeddedBrowser.NavigationCompleted += (_, eventArgs) =>
                {
                    chatGptNavigationCompleted = true;
                    chatGptNavigationSucceeded = eventArgs.IsSuccess;
                };
                openChatGptButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                DateTime embeddedBrowserDeadline = DateTime.UtcNow.AddSeconds(30);
                while (DateTime.UtcNow < embeddedBrowserDeadline
                    && (embeddedBrowser.CoreWebView2 == null
                        || !(embeddedBrowser.CoreWebView2.Source ?? string.Empty).StartsWith("https://chatgpt.com/", StringComparison.OrdinalIgnoreCase)
                        || !chatGptNavigationCompleted
                        || !chatGptNavigationSucceeded))
                {
                    Pump(1);
                }

                if (embeddedBrowser.CoreWebView2 == null
                    || !(embeddedBrowser.CoreWebView2.Source ?? string.Empty).StartsWith("https://chatgpt.com/", StringComparison.OrdinalIgnoreCase)
                    || !chatGptNavigationCompleted
                    || !chatGptNavigationSucceeded)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM browser assist did not complete ChatGPT navigation after the explicit open action. "
                        + "Source=" + (embeddedBrowser.CoreWebView2?.Source ?? "<none>")
                        + ", Completed=" + chatGptNavigationCompleted
                        + ", Succeeded=" + chatGptNavigationSucceeded);
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmBrowserAssist_ChatGptOpened.png"));
                FrameworkElement browserAssistPlaceholder = FindNamedVisualChild<FrameworkElement>(
                    shellHost,
                    "recipeLlmBrowserAssistPlaceholder",
                    "Recipe manager LLM browser assist");
                embeddedBrowser.Visibility = Visibility.Collapsed;
                browserAssistPlaceholder.Visibility = Visibility.Visible;
                Pump(20);

                if (shellHost.NativePreviewRunCount != beforeRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM browser assist opening triggered Preview/Run. "
                        + $"Before={beforeRuns}, After={shellHost.NativePreviewRunCount}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmBrowserAssist.png"));
                llmXmlTab.IsSelected = true;
                Pump(20);

                commands.SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
                Pump(20);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM pin-gap intent focus",
                    "HostRecipeLlmPinGapIntentSkill",
                    "HostRecipeSuggestPinGapIntentRoiButton",
                    "HostRecipePinGapIntentWorkflowText",
                    "HostRecipePinGapIntentFeedbackText",
                    "HostRecipePinGapIntentLatestRunText");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM pin-gap intent focus",
                    "HostRecipeLlmBlobCountIntentSkill",
                    "HostRecipeLlmContourCountIntentSkill");

                commands.BuildLlmPromptCommand.Execute(null);
                Pump(20);
                string pinGapPrompt = commands.LlmPromptText ?? string.Empty;
                if (!pinGapPrompt.Contains("self-contained GPT task packet", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPrompt.Contains("measure pin-to-pin distance", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPrompt.Contains("DistanceMmRange", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPrompt.Contains("Do not use Contour", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPrompt.Contains("Response format: return XML only", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM intent skill did not build the expected LineDistance prompt packet. "
                        + pinGapPrompt);
                }

                if (shellHost.NativePreviewRunCount != beforeRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM pin-gap prompt construction triggered Preview/Run. "
                        + $"Before={beforeRuns}, After={shellHost.NativePreviewRunCount}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_PinGap.png"));

                commands.SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.DarkBandGapTemplate;
                commands.DarkBandGapIntentRoiText = OpenVisionRecipeDarkBandGapIntentSkill.DefaultRoiText;
                Pump(20);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM dark-band Gap intent focus",
                    "HostRecipeLlmDarkBandGapIntentSkill",
                    "HostRecipeDarkBandGapIntentRoiText");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM dark-band Gap intent focus",
                    "HostRecipeLlmPinGapIntentSkill",
                    "HostRecipeLlmBlobCountIntentSkill",
                    "HostRecipeLlmContourCountIntentSkill");
                commands.BuildLlmPromptCommand.Execute(null);
                Pump(20);
                string darkBandGapPrompt = commands.LlmPromptText ?? string.Empty;
                if (!darkBandGapPrompt.Contains("direct dark-band Gap measurement", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("USE_GAP_EDGE_PAIR=true", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("Do not add Matching", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("acceptance gate", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("candidate lines", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("nearest sustained bright transition", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapPrompt.Contains("farther Hough line", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM intent skill did not build the expected dark-band Gap prompt packet. "
                        + darkBandGapPrompt);
                }

                if (!commands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Dark-band Gap Guided Setup starter was not ready for one reviewed coarse ROI.");
                }

                commands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(40);
                string darkBandGapXml = commands.LlmXmlDraftText ?? string.Empty;
                string darkBandGapValidation = commands.LlmXmlDraftValidationReport ?? string.Empty;
                if (!darkBandGapXml.Contains("<ToolType>LineDistance</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapXml.Contains("<Key>USE_GAP_EDGE_PAIR</Key>", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapXml.Contains("<Value>100,80,530,230</Value>", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapXml.Contains("<Key>PIXELPERMM</Key>", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapValidation.Contains("MEASURE ONLY / NOT JUDGED", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapValidation.Contains("Dark-band Gap evidence metrics: OK", StringComparison.OrdinalIgnoreCase)
                    || !darkBandGapValidation.Contains("Dark-band Gap drawings: WAIT", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Dark-band Gap starter XML or strict intent validation did not match the frozen contract. "
                        + darkBandGapValidation
                        + Environment.NewLine
                        + darkBandGapXml);
                }

                commands.LlmXmlDraftText = darkBandGapXml.Replace(
                    "<UseAcceptance>false</UseAcceptance>",
                    "<UseAcceptance>true</UseAcceptance>",
                    StringComparison.OrdinalIgnoreCase);
                commands.ValidateLlmXmlDraftCommand.Execute(null);
                Pump(20);
                if (!commands.LlmXmlDraftValidationReport.Contains("Dark-band Gap contract: NG", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Dark-band Gap intent validation accepted an unapproved product acceptance gate.");
                }
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapRejectedAcceptanceValidation.txt"),
                    commands.LlmXmlDraftValidationReport,
                    Encoding.UTF8);

                commands.LlmXmlDraftText = darkBandGapXml.Replace(
                    "<ToolType>LineDistance</ToolType>",
                    "<ToolType>Matching</ToolType>",
                    StringComparison.OrdinalIgnoreCase);
                commands.ValidateLlmXmlDraftCommand.Execute(null);
                Pump(20);
                if (!commands.LlmXmlDraftValidationReport.Contains("Dark-band Gap contract: NG", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Dark-band Gap intent validation accepted Matching in the locked tool family.");
                }
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapRejectedMatchingValidation.txt"),
                    commands.LlmXmlDraftValidationReport,
                    Encoding.UTF8);

                commands.LlmXmlDraftText = darkBandGapXml.Replace(
                    "<Value>100,80,530,230</Value>",
                    "<Value>101,80,530,230</Value>",
                    StringComparison.OrdinalIgnoreCase);
                commands.ValidateLlmXmlDraftCommand.Execute(null);
                Pump(20);
                if (!commands.LlmXmlDraftValidationReport.Contains("Dark-band Gap contract: NG", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Dark-band Gap intent validation accepted an ROI different from the operator-reviewed coarse ROI.");
                }
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapRejectedRoiValidation.txt"),
                    commands.LlmXmlDraftValidationReport,
                    Encoding.UTF8);

                commands.LlmXmlDraftText = darkBandGapXml;
                commands.ValidateLlmXmlDraftCommand.Execute(null);
                Pump(20);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapPrompt.txt"),
                    darkBandGapPrompt,
                    Encoding.UTF8);
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapStarter.pipeline.xml"),
                    darkBandGapXml,
                    Encoding.Unicode);
                if (!SerializeHelper.TryLoadFromXmlFile(
                        Path.Combine(outputDirectory, "DarkBandGapStarter.pipeline.xml"),
                        out VisionPipeline persistedDarkBandGapPipeline)
                    || persistedDarkBandGapPipeline == null
                    || persistedDarkBandGapPipeline.Steps.Count != 1)
                {
                    throw new InvalidOperationException(
                        "Persisted dark-band Gap starter XML did not round-trip through the runtime file loader.");
                }
                File.WriteAllText(
                    Path.Combine(outputDirectory, "DarkBandGapValidation.txt"),
                    commands.LlmXmlDraftValidationReport,
                    Encoding.UTF8);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_DarkBandGap.png"));

                if (shellHost.NativePreviewRunCount != beforeRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM dark-band Gap prompt/XML validation triggered Preview/Run. "
                        + $"Before={beforeRuns}, After={shellHost.NativePreviewRunCount}");
                }

                commands.SelectedLlmToolTemplate = "Threshold + Blob";
                Pump(20);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM blob-count intent focus",
                    "HostRecipeLlmBlobCountIntentSkill",
                    "HostRecipeBlobCountIntentWorkflowText",
                    "HostRecipeBlobCountIntentFeedbackText",
                    "HostRecipeBlobCountIntentLatestRunText");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM blob-count intent focus",
                    "HostRecipeLlmPinGapIntentSkill",
                    "HostRecipeLlmContourCountIntentSkill");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_BlobCount.png"));

                commands.SelectedLlmToolTemplate = "Shape boundary (Contour)";
                Pump(20);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM contour-count intent focus",
                    "HostRecipeLlmContourCountIntentSkill",
                    "HostRecipeContourCountIntentWorkflowText",
                    "HostRecipeContourCountIntentFeedbackText",
                    "HostRecipeContourCountIntentLatestRunText");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager LLM contour-count intent focus",
                    "HostRecipeLlmPinGapIntentSkill",
                    "HostRecipeLlmBlobCountIntentSkill");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_ContourCount.png"));

                commands.SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
                Pump(20);
                VisionPipeline wrongPinGapDraft = new VisionPipeline { Name = "Direct_LLM_PinGap_WrongContour" };
                VisionPipelineStep wrongThresholdStep = new VisionPipelineStep
                {
                    Name = "01 Pin Binary",
                    ToolType = "Threshold",
                    InputLayer = "Main",
                    OutputLayer = "Pin_Binary"
                };
                wrongThresholdStep.Parameters["Threshold"] = "128";
                wrongThresholdStep.Parameters["MaxValue"] = "255";
                wrongPinGapDraft.Steps.Add(wrongThresholdStep);
                VisionPipelineStep wrongContourStep = new VisionPipelineStep
                {
                    Name = "02 Wrong Pin Distance Contour",
                    ToolType = "Contour",
                    InputLayer = "Pin_Binary",
                    OutputLayer = "Pin_Contour"
                };
                wrongContourStep.Parameters["USE_THRESHOLD"] = "false";
                wrongContourStep.Parameters["MIN_AREA"] = "100";
                wrongContourStep.Parameters["MAX_AREA"] = "5000";
                wrongPinGapDraft.Steps.Add(wrongContourStep);
                commands.LlmXmlDraftText = SerializePipelineToXmlText(wrongPinGapDraft);
                if (commands.ValidateLlmXmlDraftTextForTest()
                    || !commands.LlmXmlDraftValidationReport.Contains("Intent contract mismatch", StringComparison.OrdinalIgnoreCase)
                    || !commands.LlmXmlDraftValidationReport.Contains("LineDistance", StringComparison.OrdinalIgnoreCase)
                    || !commands.LlmXmlDraftValidationReport.Contains("Contour", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM pin-gap intent did not block a Contour-only distance draft. "
                        + commands.LlmXmlDraftValidationReport);
                }
                File.WriteAllText(
                    Path.Combine(outputDirectory, "PinGapContourMismatchValidation.txt"),
                    commands.LlmXmlDraftValidationReport,
                    Encoding.UTF8);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentSkills_PinGapContourMismatch.png"));

                commands.SelectedLlmToolTemplate = "Template Matching";
                string dependencyImagePath = Path.Combine(
                    Path.GetTempPath(),
                    "OpenVisionLab_llm_dependency_review_" + Guid.NewGuid().ToString("N") + ".png");
                try
                {
                    using (Bitmap dependencyImage = CreateDockFloatSmokeBitmap())
                    {
                        dependencyImage.Save(dependencyImagePath);
                    }

                    VisionPipeline dependencyDraft = CreateDirectSmokePipeline("Direct_LLM_DependencyReview", 2);
                    dependencyDraft.Steps[0].Parameters["TemplatePath"] = dependencyImagePath;
                    commands.LlmXmlDraftText = SerializePipelineToXmlText(dependencyDraft);
                    if (!commands.ValidateLlmXmlDraftTextForTest())
                    {
                        throw new InvalidOperationException(
                            "Recipe manager LLM dependency review did not validate an existing template path. "
                            + commands.LlmXmlDraftValidationReport);
                    }

                    OpenVisionRecipeDependencyReviewRow dependencyRow = commands.LlmXmlDraftDependencyRows
                        .FirstOrDefault(row => string.Equals(row.ParameterName, "TemplatePath", StringComparison.OrdinalIgnoreCase));
                    if (dependencyRow == null
                        || !string.Equals(dependencyRow.Path, dependencyImagePath, StringComparison.OrdinalIgnoreCase)
                        || (!dependencyRow.Status.Contains("Found", StringComparison.OrdinalIgnoreCase)
                            && !dependencyRow.Status.Contains("확인", StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new InvalidOperationException(
                            "Recipe manager LLM dependency review did not expose the existing template path. "
                            + "Rows=" + string.Join(" | ", commands.LlmXmlDraftDependencyRows.Select(row => row.Status + ":" + row.ParameterName + ":" + row.Path)));
                    }
                }
                finally
                {
                    if (File.Exists(dependencyImagePath))
                    {
                        File.Delete(dependencyImagePath);
                    }
                }

                string reviewBundleText = commands.BuildLlmReviewBundleTextForTest();
                if (!reviewBundleText.Contains("OpenVisionLab LLM XML review bundle", StringComparison.OrdinalIgnoreCase)
                    || !reviewBundleText.Contains("Intent contract", StringComparison.OrdinalIgnoreCase)
                    || !reviewBundleText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !reviewBundleText.Contains("Direct_LLM_DependencyReview", StringComparison.OrdinalIgnoreCase)
                    || !reviewBundleText.Contains(dependencyImagePath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM review bundle builder did not preserve the correction packet context. "
                        + reviewBundleText);
                }

                if (shellHost.NativePreviewRunCount != beforeRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM intent skill selection triggered Preview/Run. "
                        + $"Before={beforeRuns}, After={shellHost.NativePreviewRunCount}");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: recipe-manager-llm-intent-skills" + Environment.NewLine
                    + "TemplateMatching: intent skill blocks collapsed" + Environment.NewLine
                    + "PinGapFocus: only pin-gap skill block visible" + Environment.NewLine
                    + "DarkBandGapFocus: one coarse ROI, px-only starter, strict direct-Gap validation, and no Preview/Run" + Environment.NewLine
                    + "BlobCountFocus: only blob-count skill block visible" + Environment.NewLine
                    + "ContourCountFocus: only contour-count skill block visible" + Environment.NewLine
                    + "TemplateDraftBuilder: LineDistance, Blob, Contour, EdgeBasedMatching, Mean, and Matching starters verified" + Environment.NewLine
                    + "PromptBuilder: matching, generic LineDistance, and dark-band Gap packet content verified without clipboard" + Environment.NewLine
                    + "BrowserAssist: ChatGPT open actions and explicit copy/paste controls visible; ChatGPT navigated only after explicit click; Preview/Run unchanged" + Environment.NewLine
                    + "PinGapContourMismatch: blocked by intent contract" + Environment.NewLine
                    + "DependencyReview: existing template path found without clipboard" + Environment.NewLine
                    + "CorrectionBundle: current validation and dependency context assembled without clipboard" + Environment.NewLine
                    + "PreviewRunCountUnchanged: " + beforeRuns.ToString(CultureInfo.InvariantCulture),
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunRecipeManagerReferenceDifferenceGuidedSetup(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces("Smoke_ReferenceDifferenceGuided_");
            string recipeName = "Smoke_ReferenceDifferenceGuided_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Direct_ReferenceDifference_Guided_Setup";
            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(pipelineName, 1));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            OpenVisionShellHostWindow window = null;
            try
            {
                window = new OpenVisionShellHostWindow(ApplicationRuntimeContext.CreateDefault())
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(40);

                FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                    shellHost,
                    "btnHostRecipeManager",
                    "ReferenceDifference Guided setup smoke").IsChecked = true;
                Pump(50);
                FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                    shellHost,
                    "recipeAdvancedReviewToggle",
                    "ReferenceDifference Guided setup smoke").IsChecked = true;
                Pump(40);
                FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeGuidedSetup",
                    "ReferenceDifference Guided setup smoke").IsSelected = true;
                Pump(40);

                int runsBefore = shellHost.NativePreviewRunCount;
                int layersBefore = shellHost.LayerDocumentCount;
                string inputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;
                OpenVisionShellHostRecipeCommandSurface commands = shellHost.RecipeCommands;
                commands.SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.ReferenceDifferenceTemplate;
                commands.LlmReferenceImagePath = string.Empty;
                Pump(20);
                if (commands.IsGuidedSetupIntentInputReady
                    || commands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("ReferenceDifference Guided setup accepted a missing Good reference.");
                }

                string repositoryRoot = FindRepositoryRoot();
                string[] referencePaths = Enumerable.Range(1, 4)
                    .Select(index => Path.Combine(repositoryRoot, "Sample", "EasyMatch", "Die Pad " + index + ".bmp"))
                    .ToArray();
                if (referencePaths.Any(path => !File.Exists(path)))
                {
                    throw new InvalidOperationException(
                        "ReferenceDifference Guided setup references were not found: "
                        + string.Join(" | ", referencePaths.Where(path => !File.Exists(path))));
                }

                commands.LlmReferenceImagePath = referencePaths[0];
                commands.ReferenceDifferencePath2 = referencePaths[1];
                commands.ReferenceDifferencePath3 = referencePaths[2];
                commands.ReferenceDifferencePath4 = referencePaths[3];
                commands.ReferenceDifferenceThresholdText = "35";
                commands.ReferenceDifferenceMinimumAreaText = "80";
                commands.ReferenceDifferenceMaximumAreaText = "20000";
                Pump(40);
                if (!commands.IsGuidedSetupIntentInputReady
                    || !commands.GuidedSetupIntentInputStatusText.Contains("ResultCount=0", StringComparison.OrdinalIgnoreCase)
                    || !commands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "ReferenceDifference Guided setup was not ready. "
                        + commands.GuidedSetupIntentInputStatusText);
                }

                commands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(80);
                if (!commands.LlmXmlDraftText.Contains("<ToolType>ReferenceDifference</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !commands.LlmXmlDraftText.Contains("<Key>ReferencePath4</Key>", StringComparison.OrdinalIgnoreCase)
                    || !commands.LlmXmlDraftText.Contains("<AcceptanceMetricMaximum>0</AcceptanceMetricMaximum>", StringComparison.OrdinalIgnoreCase)
                    || !commands.ValidateLlmXmlDraftTextForTest()
                    || !commands.ImportLlmXmlDraftCommand.CanExecute(null)
                    || commands.LlmXmlDraftDependencyRows.Count < 4
                    || shellHost.NativePreviewRunCount != runsBefore
                    || shellHost.LayerDocumentCount != layersBefore
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("ReferenceDifference Guided setup draft was invalid or caused execution/layer/route side effects.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "ReferenceDifference Guided setup smoke",
                    "HostRecipeGuidedSetupReferenceDifferenceInputs",
                    "HostRecipeGuidedSetupReferenceDifferencePath1Text",
                    "HostRecipeGuidedSetupReferenceDifferencePath4Text",
                    "HostRecipeGuidedSetupReferenceDifferenceThresholdText",
                    "HostRecipeGuidedSetupReferenceDifferenceMinimumAreaText",
                    "HostRecipeGuidedSetupReferenceDifferenceMaximumAreaText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_ReferenceDifference.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: recipe-manager-reference-difference-guided-setup" + Environment.NewLine
                    + "References: 4 repository Sample\\EasyMatch Good references" + Environment.NewLine
                    + "StarterXML: ReferenceDifference + ResultCount=0" + Environment.NewLine
                    + "DependencyRows: " + commands.LlmXmlDraftDependencyRows.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PreviewRunCountUnchanged: " + runsBefore.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerAndRouteStateUnchanged: true",
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void VerifyLlmTemplateDraftBuilderContract()
        {
            VisionPipeline lineDistance = OpenVisionRecipeLlmTemplateDraftBuilder.Create(
                "Pin gap / edge distance (LineDistance)",
                string.Empty,
                OpenVisionRecipePinGapIntentSkill.DefaultRoiSamplesText);
            if (!lineDistance.Steps.Any(step => string.Equals(step.ToolType, "LineDistance", StringComparison.OrdinalIgnoreCase))
                || !lineDistance.Steps.Any(step => string.Equals(step.ToolType, "OverlayMerge", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("LLM template draft builder did not preserve the LineDistance starter contract.");
            }

            VisionPipeline blob = OpenVisionRecipeLlmTemplateDraftBuilder.Create("Threshold + Blob", string.Empty, string.Empty);
            if (blob.Steps.Count != 2
                || !string.Equals(blob.Steps[0].ToolType, "Threshold", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(blob.Steps[1].ToolType, "Blob", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(blob.Steps[1].InputLayer, "Threshold_Preview", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("LLM template draft builder did not preserve the Blob starter contract.");
            }

            VisionPipeline contour = OpenVisionRecipeLlmTemplateDraftBuilder.Create("Shape boundary (Contour)", string.Empty, string.Empty);
            if (contour.Steps.Count != 1
                || !string.Equals(contour.Steps[0].ToolType, "Contour", StringComparison.OrdinalIgnoreCase)
                || !contour.Steps[0].UseAcceptance
                || !string.Equals(contour.Steps[0].AcceptanceMetricName, "ResultCount", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("LLM template draft builder did not preserve the Contour starter contract.");
            }

            VisionPipeline edge = OpenVisionRecipeLlmTemplateDraftBuilder.Create("Edge Based Matching", "edge-template.png", string.Empty);
            VisionPipeline mean = OpenVisionRecipeLlmTemplateDraftBuilder.Create("Mean Intensity", string.Empty, string.Empty);
            VisionPipeline matching = OpenVisionRecipeLlmTemplateDraftBuilder.Create("Template Matching", "matching-template.png", string.Empty);
            if (edge.Steps.Count != 1
                || !string.Equals(edge.Steps[0].ToolType, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                || mean.Steps.Count != 1
                || !string.Equals(mean.Steps[0].ToolType, "Mean", StringComparison.OrdinalIgnoreCase)
                || matching.Steps.Count != 1
                || !string.Equals(matching.Steps[0].ToolType, "Matching", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(matching.Steps[0].Parameters["TemplatePath"], "matching-template.png", StringComparison.Ordinal)
                || !string.Equals(matching.Steps[0].Parameters["PATTERN_PATH"], "matching-template.png", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("LLM template draft builder did not preserve the EdgeBasedMatching, Mean, or Matching starter contract.");
            }
        }

        private static string VerifyPinGapUnitSampleContract(string outputDirectory)
        {
            const double mmPerPixel = 0.006;
            IReadOnlyList<OpenVisionRecipePinGapIntentSkill.RoiSample> samples = new[]
            {
                new OpenVisionRecipePinGapIntentSkill.RoiSample(430, 170, 125, 145)
            };
            VisionPipeline mmPipeline = OpenVisionRecipePinGapIntentSkill.CreatePipeline(samples, 0.20, 0.25, 0.03, mmPerPixel);
            VisionPipeline pixelPipeline = OpenVisionRecipePinGapIntentSkill.CreatePixelPipeline(
                samples,
                0.20 / mmPerPixel,
                0.25 / mmPerPixel,
                0.03 / mmPerPixel);

            VisionPipelineValidationResult mmValidation = VisionPipelineValidator.Validate(mmPipeline, new[] { "Main" });
            VisionPipelineValidationResult pixelValidation = VisionPipelineValidator.Validate(pixelPipeline, new[] { "Main" });
            if (!mmValidation.Success || !pixelValidation.Success)
            {
                throw new InvalidOperationException(
                    "Pin gap unit sample pipelines did not validate. MM="
                    + mmValidation.FormatErrors()
                    + " PX="
                    + pixelValidation.FormatErrors());
            }

            string repoRoot = FindRepositoryRoot();
            string goodPath = Path.Combine(repoRoot, "docs", "samples", "public", "Line_Pins_Synthetic_OK.png");
            string badPath = Path.Combine(repoRoot, "docs", "samples", "public", "Line_Pins_Synthetic_WidePin_NG.png");
            EnsureFileExists(goodPath, "Pin gap public Good sample");
            EnsureFileExists(badPath, "Pin gap public Bad sample");

            using (VisionRecipeRunResult mmGood = RunPinGapUnitPipeline(mmPipeline, goodPath))
            using (VisionRecipeRunResult pixelGood = RunPinGapUnitPipeline(pixelPipeline, goodPath))
            using (VisionRecipeRunResult mmBad = RunPinGapUnitPipeline(mmPipeline, badPath))
            using (VisionRecipeRunResult pixelBad = RunPinGapUnitPipeline(pixelPipeline, badPath))
            {
                SaveRecipeResultImage(mmGood, Path.Combine(outputDirectory, "PinGapUnit_MM_Good.png"));
                SaveRecipeResultImage(pixelGood, Path.Combine(outputDirectory, "PinGapUnit_PX_Good.png"));
                SaveRecipeResultImage(mmBad, Path.Combine(outputDirectory, "PinGapUnit_MM_Bad.png"));
                SaveRecipeResultImage(pixelBad, Path.Combine(outputDirectory, "PinGapUnit_PX_Bad.png"));

                if (!mmGood.Success || !pixelGood.Success || mmBad.Success || pixelBad.Success)
                {
                    throw new InvalidOperationException(
                        "Pin gap public sample outcomes did not match the Good/Bad contract. "
                        + FormatPinGapUnitOutcome("MM Good", mmGood) + "; "
                        + FormatPinGapUnitOutcome("PX Good", pixelGood) + "; "
                        + FormatPinGapUnitOutcome("MM Bad", mmBad) + "; "
                        + FormatPinGapUnitOutcome("PX Bad", pixelBad));
                }

                double mmGoodAverage = ReadRecipeMetric(mmGood, VisionPipelineKnownMetrics.DistanceMmAvg);
                double mmGoodPixels = ReadRecipeMetric(mmGood, VisionPipelineKnownMetrics.DistancePxAvg);
                double pixelGoodAverage = ReadRecipeMetric(pixelGood, VisionPipelineKnownMetrics.DistancePxAvg);
                double mmBadAverage = ReadRecipeMetric(mmBad, VisionPipelineKnownMetrics.DistanceMmAvg);
                double pixelBadAverage = ReadRecipeMetric(pixelBad, VisionPipelineKnownMetrics.DistancePxAvg);

                if (Math.Abs(mmGoodPixels - pixelGoodAverage) > 0.001
                    || Math.Abs(mmGoodAverage - pixelGoodAverage * mmPerPixel) > 0.001
                    || Math.Abs(mmBadAverage - pixelBadAverage * mmPerPixel) > 0.001
                    || pixelGood.Steps.Any(step => step.Metrics.Keys.Any(metric => metric.IndexOf("Mm", StringComparison.OrdinalIgnoreCase) >= 0)))
                {
                    throw new InvalidOperationException(
                        "Pin gap public sample px/mm metrics were not equivalent or px-only emitted mm metrics. "
                        + $"MM Good={mmGoodAverage:0.###}, MM Good px={mmGoodPixels:0.###}, PX Good={pixelGoodAverage:0.###}, MM Bad={mmBadAverage:0.###}, PX Bad={pixelBadAverage:0.###}");
                }

                return "Good OK in both modes, Bad NG in both modes, "
                    + "DistanceMmAvg=" + mmGoodAverage.ToString("0.###", CultureInfo.InvariantCulture)
                    + ", DistancePxAvg=" + pixelGoodAverage.ToString("0.###", CultureInfo.InvariantCulture)
                    + ", parity OK";
            }
        }

        private static VisionRecipeRunResult RunPinGapUnitPipeline(VisionPipeline pipeline, string imagePath)
        {
            using (OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged))
            {
                if (source.Empty())
                {
                    throw new InvalidOperationException("Pin gap public sample could not be loaded: " + imagePath);
                }

                return new VisionRecipeRunner()
                    .RunAsync(pipeline, source, VisionRecipeRunner.DefaultInputLayer, 5000, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private static double ReadRecipeMetric(VisionRecipeRunResult result, string metricName)
        {
            foreach (VisionRecipeStepRunSummary step in result?.Steps ?? new List<VisionRecipeStepRunSummary>())
            {
                if (step.Metrics.TryGetValue(metricName, out double value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("Pin gap sample metric was not found: " + metricName);
        }

        private static string FormatPinGapUnitOutcome(string label, VisionRecipeRunResult result)
        {
            return label
                + " Success=" + (result?.Success ?? false)
                + ", DistanceMmAvg=" + FormatRecipeMetric(result, VisionPipelineKnownMetrics.DistanceMmAvg)
                + ", DistancePxAvg=" + FormatRecipeMetric(result, VisionPipelineKnownMetrics.DistancePxAvg)
                + ", DistanceMmRange=" + FormatRecipeMetric(result, VisionPipelineKnownMetrics.DistanceMmRange)
                + ", DistancePxRange=" + FormatRecipeMetric(result, VisionPipelineKnownMetrics.DistancePxRange);
        }

        private static string FormatRecipeMetric(VisionRecipeRunResult result, string metricName)
        {
            foreach (VisionRecipeStepRunSummary step in result?.Steps ?? new List<VisionRecipeStepRunSummary>())
            {
                if (step.Metrics.TryGetValue(metricName, out double value))
                {
                    return value.ToString("0.###", CultureInfo.InvariantCulture);
                }
            }

            return "n/a";
        }

        private static void SaveRecipeResultImage(VisionRecipeRunResult result, string outputPath)
        {
            if (result?.ResultImage == null || result.ResultImage.Empty())
            {
                throw new InvalidOperationException("Pin gap sample run did not produce a result image: " + outputPath);
            }

            OpenCvSharp.Cv2.ImWrite(outputPath, result.ResultImage);
        }

        private static void RunRecipePipelineRoundtrip(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces("Smoke_RecipeRoundtrip_");
            string requestedRecipeName = ResolveOptionalTextOption(args, "--recipe-name");
            string requestedPipelineName = ResolveOptionalTextOption(args, "--pipeline-name");
            bool usesOperatorName = !string.IsNullOrWhiteSpace(requestedRecipeName);
            string recipeName = usesOperatorName
                ? requestedRecipeName
                : "Smoke_RecipeRoundtrip_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            string pipelineName = string.IsNullOrWhiteSpace(requestedPipelineName)
                ? "Direct_Recipe_Roundtrip"
                : requestedPipelineName;
            if (!RecipeWorkspaceService.IsValidRecipeName(recipeName)
                || !RecipeWorkspaceService.IsValidRecipeName(pipelineName))
            {
                throw new ArgumentException("Recipe Pipeline roundtrip received an invalid recipe or pipeline name.");
            }

            if (usesOperatorName
                && RecipeWorkspaceService.GetRecipeNames().Contains(recipeName, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe Pipeline roundtrip will not overwrite an existing operator recipe: " + recipeName);
            }

            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(pipelineName, 2));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                using (Bitmap mainImage = new Bitmap(160, 120))
                {
                    using Graphics graphics = Graphics.FromImage(mainImage);
                    graphics.Clear(System.Drawing.Color.White);
                    graphics.FillRectangle(System.Drawing.Brushes.Black, 24, 24, 48, 56);
                    graphics.FillRectangle(System.Drawing.Brushes.Gray, 96, 36, 36, 48);
                    shellHost.SetMainLayerImageForTest(mainImage);
                }
                Pump(60);

                int nativeRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;
                string recipeLayerBefore = shellHost.ActiveRecipeContextLayerNameForTest;
                System.Windows.Controls.Primitives.ToggleButton recipeManagerButton =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "btnHostRecipeManager",
                        "Recipe Pipeline roundtrip");

                recipeManagerButton.IsChecked = true;
                Pump(60);
                System.Windows.Controls.Primitives.ToggleButton advancedReviewToggle =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "recipeAdvancedReviewToggle",
                        "Recipe Pipeline roundtrip");
                TabItem overviewTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeOverview",
                    "Recipe Pipeline roundtrip");
                if (!overviewTab.IsSelected
                    || advancedReviewToggle.IsChecked == true
                    || !string.Equals(shellHost.SelectedRecipeNameForTest, recipeName, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip did not start from the selected no-run recipe summary. "
                        + $"Overview={overviewTab.IsSelected}, Advanced={advancedReviewToggle.IsChecked}, "
                        + $"Recipe='{shellHost.SelectedRecipeNameForTest}', Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}");
                }

                string workSampleName = shellHost.RecipeCommands.SelectedSampleOption?.SampleName ?? string.Empty;
                if (string.IsNullOrWhiteSpace(workSampleName)
                    || shellHost.RecipeCommands.HasCurrentRecipeSampleExecution
                    || !shellHost.RecipeCommands.RecipeOverviewLastResultValueText.Contains("검사하지 않음", StringComparison.Ordinal)
                    || shellHost.RecipeCommands.RecipeOverviewLastResultValueText.Contains(workSampleName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip presented a workspace sample as recipe validation evidence before an explicit sample run. "
                        + $"Sample='{workSampleName}', Latest='{shellHost.RecipeCommands.RecipeOverviewLastResultValueText}', "
                        + $"HasExecution={shellHost.RecipeCommands.HasCurrentRecipeSampleExecution}");
                }

                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Operator_Summary.png"));

                shellHost.RecipeCommands.OpenPipelineReviewCommand.Execute(null);
                Pump(80);
                Window pipelineReviewWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(item => item.IsVisible && IsFloatingToolWindow(item));
                if (pipelineReviewWindow == null
                    || recipeManagerButton.IsChecked == true
                    || !string.Equals(shellHost.PipelineReviewRecipeContextNameForTest, recipeName, StringComparison.Ordinal)
                    || !string.Equals(shellHost.PipelineReviewRecipeContextPipelineNameForTest, pipelineName, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip did not open Pipeline Review with the selected recipe and no native Preview. "
                        + $"Window={pipelineReviewWindow != null}, ManagerOpen={recipeManagerButton.IsChecked}, "
                        + $"Recipe='{shellHost.PipelineReviewRecipeContextNameForTest}', "
                        + $"Pipeline='{shellHost.PipelineReviewRecipeContextPipelineNameForTest}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}");
                }

                shellHost.SelectPipelineReviewStepForTest(1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Overlay);
                Pump(24);
                string pendingInputNext = OpenVisionLanguageService.T("PipelineReview.Guide.InputPendingNext");
                string pendingInputDetail = OpenVisionLanguageService.T("PipelineReview.Guide.InputPendingDetail");
                if (!string.Equals(shellHost.PipelineReviewSelectedStatusText, "WAIT", StringComparison.Ordinal)
                    || !shellHost.PipelineReviewGuideNextActionText.Contains(pendingInputNext, StringComparison.Ordinal)
                    || !shellHost.PipelineReviewGuideDetailText.Contains(pendingInputDetail, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip did not keep an upstream-produced input in the explicit-run waiting state. "
                        + $"Status='{shellHost.PipelineReviewSelectedStatusText}', Next='{shellHost.PipelineReviewGuideNextActionText}', "
                        + $"Detail='{shellHost.PipelineReviewGuideDetailText}', Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}");
                }
                string pendingInputState = shellHost.PipelineReviewSelectedStatusText + " / " + shellHost.PipelineReviewGuideNextActionText;

                AssertVisibleAutomationIds(
                    pipelineReviewWindow,
                    "Recipe Pipeline roundtrip Pipeline Review header",
                    "PipelineReviewReturnToRecipeButton",
                    "PipelineReviewRecipeContext",
                    "PipelineReviewRunReviewButton",
                    "PipelineReviewReadinessStrip",
                    "PipelineReviewReadinessSummary",
                    "PipelineReviewReadinessInput",
                    "PipelineReviewReadinessRoute",
                    "PipelineReviewReadinessAcceptance",
                    "PipelineReviewReadinessGoodBad",
                    "PipelineReviewReadinessCalibration");
                string pipelineReviewText = string.Join(
                    " | ",
                    FindVisualChildren<TextBlock>(pipelineReviewWindow)
                        .Where(item => item.IsVisible && !string.IsNullOrWhiteSpace(item.Text))
                        .Select(item => item.Text));
                if (!pipelineReviewText.Contains(recipeName, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip did not show the selected recipe context. Text='" + pipelineReviewText + "'.");
                }

                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "Recipe Pipeline roundtrip explicit review");
                Pump(80);
                if (!shellHost.PipelineReviewResultSummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip explicit review did not produce an isolated OK result. "
                        + $"Result='{shellHost.PipelineReviewResultSummaryText}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBefore}");
                }

                string reviewResult = shellHost.PipelineReviewResultSummaryText;

                SaveWindowScreenScreenshot(
                    pipelineReviewWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_PipelineReview_Roundtrip.png"));
                Button returnToRecipeButton = FindNamedVisualChild<Button>(
                    pipelineReviewWindow,
                    "btnReturnToRecipe",
                    "Recipe Pipeline roundtrip return");
                returnToRecipeButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, returnToRecipeButton));
                Pump(80);

                if (recipeManagerButton.IsChecked != true
                    || shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !overviewTab.IsSelected
                    || advancedReviewToggle.IsChecked == true
                    || !string.Equals(shellHost.SelectedRecipeNameForTest, recipeName, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextNameForTest, recipeName, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, pipelineName, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextLayerNameForTest, recipeLayerBefore, StringComparison.Ordinal)
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore
                    || shellHost.RecipeCommands.HasCurrentRecipeSampleExecution
                    || shellHost.RecipeCommands.RecipeOverviewLastResultValueText.Contains(workSampleName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip return changed recipe, layer, routing, or native Preview state. "
                        + $"ManagerOpen={recipeManagerButton.IsChecked}, ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                        + $"Overview={overviewTab.IsSelected}, Advanced={advancedReviewToggle.IsChecked}, "
                        + $"SelectedRecipe='{shellHost.SelectedRecipeNameForTest}', Context='{shellHost.ActiveRecipeContextNameForTest}', "
                        + $"Pipeline='{shellHost.ActiveRecipeContextPipelineNameForTest}', "
                        + $"RecipeLayer='{shellHost.ActiveRecipeContextLayerNameForTest}/{recipeLayerBefore}', "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBefore}, "
                        + $"ActiveLayer='{shellHost.ActiveHostLayerTitle}/{activeLayerBefore}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}, "
                        + $"SampleExecution={shellHost.RecipeCommands.HasCurrentRecipeSampleExecution}, "
                        + $"SampleResult='{shellHost.RecipeCommands.RecipeOverviewLastResultValueText}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip returned summary",
                    "HostRecipeOverviewPanel",
                    "HostRecipeOverviewName",
                    "HostRecipeOverviewSelectedSample",
                    "HostRecipeOverviewSampleContext",
                    "HostRecipeOverviewLastResult",
                    "HostRecipeOpenPipelineReviewButton",
                    "HostRecipeAdvancedReviewToggle");
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Roundtrip_Return.png"));

                advancedReviewToggle.IsChecked = true;
                Pump(60);
                TabItem pipelineTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipeline",
                    "Recipe Pipeline roundtrip advanced review");
                if (!pipelineTab.IsSelected
                    || overviewTab.IsSelected
                    || !string.Equals(shellHost.SelectedRecipeNameForTest, recipeName, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextLayerNameForTest, recipeLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip advanced review changed execution, layer, or recipe state. "
                        + $"Pipeline={pipelineTab.IsSelected}, Overview={overviewTab.IsSelected}, "
                        + $"Recipe='{shellHost.SelectedRecipeNameForTest}/{recipeName}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBefore}, "
                        + $"Active='{shellHost.ActiveHostLayerTitle}/{activeLayerBefore}', "
                        + $"RecipeLayer='{shellHost.ActiveRecipeContextLayerNameForTest}/{recipeLayerBefore}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip advanced review",
                    "HostRecipeAdvancedTransferCommands",
                    "HostRecipePipelineTab");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip advanced review",
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeOverviewPanel");
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Operator_Advanced.png"));

                advancedReviewToggle.IsChecked = false;
                Pump(60);
                if (!overviewTab.IsSelected
                    || advancedReviewToggle.IsChecked == true
                    || !string.Equals(shellHost.SelectedRecipeNameForTest, recipeName, StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != nativeRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextLayerNameForTest, recipeLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip summary return changed execution, layer, or recipe state. "
                        + $"Overview={overviewTab.IsSelected}, Advanced={advancedReviewToggle.IsChecked}, "
                        + $"Recipe='{shellHost.SelectedRecipeNameForTest}/{recipeName}', "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBefore}, "
                        + $"Active='{shellHost.ActiveHostLayerTitle}/{activeLayerBefore}', "
                        + $"RecipeLayer='{shellHost.ActiveRecipeContextLayerNameForTest}/{recipeLayerBefore}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip summary return",
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeOverviewPanel");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip summary return",
                    "HostRecipeAdvancedTransferCommands");
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Operator_Summary_Return.png"));

                shellHost.RecipeCommands.OpenPipelineReviewCommand.Execute(null);
                Pump(80);
                pipelineReviewWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(item => item.IsVisible && IsFloatingToolWindow(item));
                if (pipelineReviewWindow == null)
                {
                    throw new InvalidOperationException("Recipe Pipeline roundtrip could not reopen Pipeline Review for Step edit handoff.");
                }

                shellHost.SelectPipelineReviewStepForTest(1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(24);
                Button editSelectedStepButton = FindNamedVisualChild<Button>(
                    pipelineReviewWindow,
                    "btnEditSelectedStep",
                    "Recipe Pipeline roundtrip Step edit handoff");
                editSelectedStepButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, editSelectedStepButton));
                Pump(160);

                TabItem xmlStepsTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineXmlSteps",
                    "Recipe Pipeline roundtrip Step edit handoff");
                ScrollViewer pipelineScrollViewer = FindNamedVisualChild<ScrollViewer>(
                    shellHost,
                    "recipePipelineTabScrollViewer",
                    "Recipe Pipeline roundtrip Step edit handoff");
                Border propertyGridHost = FindNamedVisualChild<Border>(
                    shellHost,
                    "recipeStepPropertyGridHost",
                    "Recipe Pipeline roundtrip Step edit handoff");
                Rect propertyGridBounds = propertyGridHost
                    .TransformToAncestor(pipelineScrollViewer)
                    .TransformBounds(new Rect(new System.Windows.Size(propertyGridHost.ActualWidth, propertyGridHost.ActualHeight)));
                bool propertyGridVisible = propertyGridBounds.Height > 0D
                    && propertyGridBounds.Bottom > 0D
                    && propertyGridBounds.Top < pipelineScrollViewer.ActualHeight;
                OpenVisionRecipePipelineStepPreview selectedEditStep = shellHost.RecipeCommands.SelectedPipelinePreviewStep;

                if (recipeManagerButton.IsChecked != true
                    || shellHost.IsActiveWpfToolWindowVisibleForTest
                    || advancedReviewToggle.IsChecked != true
                    || !pipelineTab.IsSelected
                    || !xmlStepsTab.IsSelected
                    || selectedEditStep?.Index != 2
                    || !string.Equals(selectedEditStep.ToolType, "Threshold", StringComparison.OrdinalIgnoreCase)
                    || shellHost.RecipeCommands.SelectedStepEditObject == null
                    || !string.Equals(
                        shellHost.RecipeCommands.SelectedSampleOption?.SampleName,
                        workSampleName,
                        StringComparison.OrdinalIgnoreCase)
                    || !propertyGridVisible
                    || shellHost.NativePreviewRunCount != nativeRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextLayerNameForTest, recipeLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe Pipeline roundtrip Step edit handoff did not expose the authoritative PropertyGrid without execution or layer changes. "
                        + $"Manager={recipeManagerButton.IsChecked}, ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                        + $"Advanced={advancedReviewToggle.IsChecked}, PipelineTab={pipelineTab.IsSelected}, XmlStepTab={xmlStepsTab.IsSelected}, "
                        + $"Step='{selectedEditStep?.Index}:{selectedEditStep?.ToolType}', Property='{shellHost.RecipeCommands.SelectedStepEditObject?.GetType().Name}', "
                        + $"WorkSample='{shellHost.RecipeCommands.SelectedSampleOption?.SampleName}/{workSampleName}', "
                        + $"PropertyGrid={propertyGridBounds.Top:F1},{propertyGridBounds.Bottom:F1}/{pipelineScrollViewer.ActualHeight:F1}, "
                        + $"Runs={shellHost.NativePreviewRunCount}/{nativeRunsBefore}, Preview={shellHost.HasNativePreviewResult}, "
                        + $"Layers={shellHost.LayerDocumentCount}/{layerCountBefore}, Active='{shellHost.ActiveHostLayerTitle}/{activeLayerBefore}', "
                        + $"RecipeLayer='{shellHost.ActiveRecipeContextLayerNameForTest}/{recipeLayerBefore}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe Pipeline roundtrip Step edit handoff destination",
                    "HostRecipePipelineXmlStepsTab",
                    "HostRecipeLoadSelectedStepParametersButton",
                    "HostRecipeApplySelectedStepParametersButton",
                    "HostRecipeSelectedStepPropertyGridHost");
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Step_Edit_Handoff.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: recipe-pipeline-roundtrip" + Environment.NewLine
                    + "Executable: " + (Environment.ProcessPath ?? "-") + Environment.NewLine
                    + "Recipe: " + recipeName + Environment.NewLine
                    + "Pipeline: " + pipelineName + Environment.NewLine
                    + "RecipeNameMode: " + (usesOperatorName ? "operator-supplied" : "generated-smoke") + Environment.NewLine
                    + "SummaryAdvancedRoundtrip: no Preview/Run, layer, active-layer, or recipe-routing changes" + Environment.NewLine
                    + "PendingProducedInput: " + pendingInputState + Environment.NewLine
                    + "ReviewResult: " + reviewResult + Environment.NewLine
                    + "WorkSample: " + workSampleName + Environment.NewLine
                    + "RecipeSampleExecution: " + shellHost.RecipeCommands.HasCurrentRecipeSampleExecution + Environment.NewLine
                    + "RecipeSampleResult: " + shellHost.RecipeCommands.RecipeOverviewLastResultValueText + Environment.NewLine
                    + "StepEditHandoff: " + selectedEditStep.Index.ToString(CultureInfo.InvariantCulture) + " / " + selectedEditStep.ToolType + Environment.NewLine
                    + "NativePreviewRuns: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunRecipeManagerTabs(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces(
                "Smoke_LlmBranch_",
                "Smoke_LlmDraft_",
                "Smoke_LlmIntentSkills_",
                "Smoke_RecipeManager_");
            string pinGapUnitSampleEvidence = VerifyPinGapUnitSampleContract(outputDirectory);
            string recipeName = "Smoke_RecipeManager_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Direct_RecipeManager_Check";
            VisionPipelineStorage.Save(recipeName, CreateDirectSmokePipeline(pipelineName, 2));
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);
            File.WriteAllText(
                RecipeWorkspaceService.GetVisionConfigPath(recipeName, "Direct_ToolState_Probe"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><DirectToolState />");
            File.WriteAllText(
                RecipeWorkspaceService.GetVisionConfigPath(recipeName, "Direct_Malformed_Pipeline_Probe"),
                "<VisionPipeline");

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(40);

                string[] listedPipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
                string[] visiblePipelineNames = shellHost.RecipeCommands.PipelineOptions
                    .Select(option => option.PipelineName)
                    .ToArray();
                if (!listedPipelineNames.SequenceEqual(new[] { pipelineName }, StringComparer.OrdinalIgnoreCase)
                    || !visiblePipelineNames.SequenceEqual(new[] { pipelineName }, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pipeline inventory included tool-state or malformed XML. "
                        + $"Storage=[{string.Join(", ", listedPipelineNames)}], UI=[{string.Join(", ", visiblePipelineNames)}]");
                }

                if (!string.Equals(shellHost.ActiveRecipeContextNameForTest, recipeName, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, pipelineName, StringComparison.Ordinal)
                    || shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count != 2)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not load the expected recipe context. "
                        + $"Recipe={shellHost.ActiveRecipeContextNameForTest}, Pipeline={shellHost.ActiveRecipeContextPipelineNameForTest}, "
                        + $"PreviewSteps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
                }

                System.Windows.Controls.Primitives.ToggleButton recipeManagerButton =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "btnHostRecipeManager",
                        "Recipe manager direct smoke");
                int recipeManagerOpenRunsBefore = shellHost.NativePreviewRunCount;
                recipeManagerButton.IsChecked = true;
                Pump(60);

                System.Windows.Controls.Primitives.ToggleButton advancedReviewToggle =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        shellHost,
                        "recipeAdvancedReviewToggle",
                        "Recipe manager direct smoke summary");
                TabItem overviewTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeOverview",
                    "Recipe manager direct smoke summary");
                if (advancedReviewToggle.IsChecked == true
                    || !overviewTab.IsSelected
                    || shellHost.NativePreviewRunCount != recipeManagerOpenRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not open on the no-run summary. "
                        + $"Advanced={advancedReviewToggle.IsChecked}, Overview={overviewTab.IsSelected}, "
                        + $"RunsBefore={recipeManagerOpenRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke summary",
                    "HostRecipeManagerPanel",
                    "HostRecipeOverviewPanel",
                    "HostRecipeOverviewName",
                    "HostRecipeOverviewPipelineCard",
                    "HostRecipeOverviewValidationCard",
                    "HostRecipeOpenPipelineReviewButton",
                    "HostRecipeAdvancedReviewToggle",
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeFilterTextBox",
                    "HostRecipeManagerList",
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeNameEditor",
                    "HostRecipeCreateNamedButton");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke summary",
                    "HostRecipeAdvancedTransferCommands",
                    "HostRecipeImportXmlButton",
                    "HostRecipeExportXmlButton",
                    "HostRecipeExportReviewBundleButton");
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Summary.png"));

                advancedReviewToggle.IsChecked = true;
                Pump(60);

                TabItem guidedSetupTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipeGuidedSetup",
                    "Recipe manager direct smoke Guided setup");
                TabItem pipelineTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipeline",
                    "Recipe manager direct smoke Guided setup");
                string guidedSetupTemplateBefore = shellHost.RecipeCommands.SelectedLlmToolTemplate;
                string guidedSetupDraftBefore = shellHost.RecipeCommands.LlmXmlDraftText;
                string guidedSetupReferenceBefore = shellHost.RecipeCommands.LlmReferenceImagePath;
                string guidedSetupMatchingRoiBefore = shellHost.RecipeCommands.MatchingIntentSearchRoiText;
                string guidedSetupMatchingScoreBefore = shellHost.RecipeCommands.MatchingIntentScoreMinText;
                string guidedSetupMatchingCountBefore = shellHost.RecipeCommands.MatchingIntentExpectedCountText;
                string guidedSetupFeatureScoreBefore = shellHost.RecipeCommands.FeatureMatchingIntentScoreMinText;
                string guidedSetupFeatureRansacBefore = shellHost.RecipeCommands.FeatureMatchingIntentRansacReprojThresholdText;
                string guidedSetupFeatureAcceptanceBefore = shellHost.RecipeCommands.FeatureMatchingIntentAcceptanceScoreMinText;
                string guidedSetupEdgeScoreBefore = shellHost.RecipeCommands.EdgeBasedIntentScoreMinText;
                string guidedSetupEdgeSearchCountBefore = shellHost.RecipeCommands.EdgeBasedIntentSearchCountText;
                string guidedSetupEdgeCannyLowBefore = shellHost.RecipeCommands.EdgeBasedIntentCannyLowText;
                string guidedSetupEdgeCannyHighBefore = shellHost.RecipeCommands.EdgeBasedIntentCannyHighText;
                string guidedSetupEdgeAcceptanceBefore = shellHost.RecipeCommands.EdgeBasedIntentAcceptanceScoreMinText;
                string guidedSetupReferenceDifferencePath2Before = shellHost.RecipeCommands.ReferenceDifferencePath2;
                string guidedSetupReferenceDifferencePath3Before = shellHost.RecipeCommands.ReferenceDifferencePath3;
                string guidedSetupReferenceDifferencePath4Before = shellHost.RecipeCommands.ReferenceDifferencePath4;
                string guidedSetupReferenceDifferenceThresholdBefore = shellHost.RecipeCommands.ReferenceDifferenceThresholdText;
                string guidedSetupReferenceDifferenceMinimumAreaBefore = shellHost.RecipeCommands.ReferenceDifferenceMinimumAreaText;
                string guidedSetupReferenceDifferenceMaximumAreaBefore = shellHost.RecipeCommands.ReferenceDifferenceMaximumAreaText;
                string guidedSetupMeanRoiBefore = shellHost.RecipeCommands.MeanIntentRoiText;
                string guidedSetupMeanTypeBefore = shellHost.RecipeCommands.MeanIntentTypeText;
                string guidedSetupMeanMinimumBefore = shellHost.RecipeCommands.MeanIntentMinimumText;
                string guidedSetupMeanMaximumBefore = shellHost.RecipeCommands.MeanIntentMaximumText;
                guidedSetupTab.IsSelected = true;
                shellHost.RecipeCommands.SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
                Pump(60);

                int guidedSetupRunsBefore = shellHost.NativePreviewRunCount;
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.GuidedSetupIntentInputStatusText.Contains("MM-READY", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.PinGapIntentCalibrationReviewText.Contains("MM-READY", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Pin gap inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("DistanceMmRange", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Starter XML triggered Preview/Run or missed the range gate.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup tab",
                    "HostRecipeGuidedSetupTab",
                    "HostRecipeGuidedSetupIntentSelector",
                    "HostRecipeGuidedSetupCreateStarterButton",
                    "HostRecipeGuidedSetupPinGapInputs",
                    "HostRecipeGuidedSetupPinGapRoiText",
                    "HostRecipeGuidedSetupPinGapScaleText",
                    "HostRecipeGuidedSetupPinGapCalibrationReview",
                    "HostRecipeGuidedSetupIntentInputStatus",
                    "HostRecipeGuidedSetupDraftText");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup.png"));

                shellHost.RecipeCommands.PinGapIntentScaleText = string.Empty;
                shellHost.RecipeCommands.PinGapIntentDistanceMinText = "60";
                shellHost.RecipeCommands.PinGapIntentDistanceMaxText = "90";
                shellHost.RecipeCommands.PinGapIntentRangeMaxText = "8";
                Pump(40);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.GuidedSetupIntentInputStatusText.Contains("PX-ONLY", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.PinGapIntentCalibrationReviewText.Contains("PX-ONLY", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup px-only Pin gap inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>DistancePxAvg</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>DistancePxRange</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>DistanceMmAvg</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup px-only Pin gap XML was invalid or triggered Preview/Run.");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_PinGapPxOnly.png"));

                shellHost.RecipeCommands.PinGapIntentScaleText = "invalid";
                Pump(40);
                if (shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup accepted an invalid Pin gap scale.");
                }

                shellHost.RecipeCommands.PinGapIntentDistanceMinText = "0.40";
                shellHost.RecipeCommands.PinGapIntentDistanceMaxText = "0.55";
                shellHost.RecipeCommands.PinGapIntentRangeMaxText = "0.06";
                shellHost.RecipeCommands.PinGapIntentScaleText = "0.006";

                shellHost.RecipeCommands.SelectedLlmToolTemplate = "Shape boundary (Contour)";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Contour inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>Contour</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>AreaMax</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Contour Starter XML triggered Preview/Run or missed AreaMax.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup Contour inputs",
                    "HostRecipeGuidedSetupContourInputs",
                    "HostRecipeGuidedSetupContourRoiText",
                    "HostRecipeGuidedSetupContourThresholdText",
                    "HostRecipeGuidedSetupContourMaxAreaText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_Contour.png"));

                shellHost.RecipeCommands.SelectedLlmToolTemplate = "Template Matching";
                shellHost.RecipeCommands.LlmReferenceImagePath = string.Empty;
                Pump(40);
                if (shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Matching did not block a missing template path.");
                }

                string guidedSetupMatchingTemplatePath = Path.GetFullPath(Path.Combine(
                    "docs",
                    "samples",
                    "public",
                    "templates",
                    "Matching_DiePad_Synthetic_Template.png"));
                if (!File.Exists(guidedSetupMatchingTemplatePath))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Matching template was not found: " + guidedSetupMatchingTemplatePath);
                }

                shellHost.RecipeCommands.LlmReferenceImagePath = guidedSetupMatchingTemplatePath;
                shellHost.RecipeCommands.MatchingIntentScoreMinText = "0.75";
                shellHost.RecipeCommands.MatchingIntentExpectedCountText = "1";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Matching inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>Matching</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>PATTERN_PATH</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>CvROI</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Value>0.75</Value>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>ResultCount</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Matching Starter XML triggered Preview/Run or missed readiness values.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup Matching inputs",
                    "HostRecipeGuidedSetupMatchingInputs",
                    "HostRecipeGuidedSetupMatchingTemplatePathText",
                    "HostRecipeGuidedSetupMatchingUseSampleButton",
                    "HostRecipeGuidedSetupMatchingSearchRoiText",
                    "HostRecipeGuidedSetupMatchingScoreMinText",
                    "HostRecipeGuidedSetupMatchingExpectedCountText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_Matching.png"));

                shellHost.RecipeCommands.SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.FeatureMatchingTemplate;
                shellHost.RecipeCommands.LlmReferenceImagePath = string.Empty;
                Pump(40);
                if (shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Feature Matching did not block a missing template path.");
                }

                string guidedSetupFeatureTemplatePath = Path.GetFullPath(Path.Combine(
                    "docs",
                    "samples",
                    "public",
                    "templates",
                    "Feature_Card_Synthetic_Template.png"));
                if (!File.Exists(guidedSetupFeatureTemplatePath))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Feature Matching template was not found: " + guidedSetupFeatureTemplatePath);
                }

                shellHost.RecipeCommands.LlmReferenceImagePath = guidedSetupFeatureTemplatePath;
                shellHost.RecipeCommands.FeatureMatchingIntentScoreMinText = "0.85";
                shellHost.RecipeCommands.FeatureMatchingIntentRansacReprojThresholdText = "4";
                shellHost.RecipeCommands.FeatureMatchingIntentAcceptanceScoreMinText = "80";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Feature Matching inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>FeatureMatching</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>RANSAC_REPROJ_THRESHOLD</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>ScoreMax</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricMinimum>80</AcceptanceMetricMinimum>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Feature Matching contract: OK", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Feature Matching Starter XML triggered Preview/Run or missed the ScoreMax contract.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup Feature Matching inputs",
                    "HostRecipeGuidedSetupFeatureMatchingInputs",
                    "HostRecipeGuidedSetupFeatureMatchingTemplatePathText",
                    "HostRecipeGuidedSetupFeatureMatchingUseSampleButton",
                    "HostRecipeGuidedSetupFeatureMatchingScoreMinText",
                    "HostRecipeGuidedSetupFeatureMatchingRansacReprojThresholdText",
                    "HostRecipeGuidedSetupFeatureMatchingAcceptanceScoreMinText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_FeatureMatching.png"));

                shellHost.RecipeCommands.SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.EdgeBasedMatchingTemplate;
                shellHost.RecipeCommands.LlmReferenceImagePath = string.Empty;
                Pump(40);
                if (shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Edge Based Matching did not block a missing template path.");
                }

                string guidedSetupEdgeTemplatePath = Path.GetFullPath(Path.Combine(
                    "docs",
                    "samples",
                    "public",
                    "templates",
                    "Edge_Fiducial_Synthetic_Template.png"));
                if (!File.Exists(guidedSetupEdgeTemplatePath))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Edge Based Matching template was not found: " + guidedSetupEdgeTemplatePath);
                }

                shellHost.RecipeCommands.LlmReferenceImagePath = guidedSetupEdgeTemplatePath;
                shellHost.RecipeCommands.EdgeBasedIntentScoreMinText = "0.70";
                shellHost.RecipeCommands.EdgeBasedIntentSearchCountText = "1";
                shellHost.RecipeCommands.EdgeBasedIntentCannyLowText = "30";
                shellHost.RecipeCommands.EdgeBasedIntentCannyHighText = "90";
                shellHost.RecipeCommands.EdgeBasedIntentAcceptanceScoreMinText = "70";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Edge Based Matching inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>EdgeBasedMatching</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>CANNY_LOW</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>CANNY_HIGH</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>ScoreMax</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricMinimum>70</AcceptanceMetricMinimum>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Edge Based Matching contract: OK", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Edge Based Matching Starter XML triggered Preview/Run or missed the ScoreMax contract.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup Edge Based Matching inputs",
                    "HostRecipeGuidedSetupEdgeBasedInputs",
                    "HostRecipeGuidedSetupEdgeBasedTemplatePathText",
                    "HostRecipeGuidedSetupEdgeBasedUseSampleButton",
                    "HostRecipeGuidedSetupEdgeBasedScoreMinText",
                    "HostRecipeGuidedSetupEdgeBasedSearchCountText",
                    "HostRecipeGuidedSetupEdgeBasedCannyLowText",
                    "HostRecipeGuidedSetupEdgeBasedCannyHighText",
                    "HostRecipeGuidedSetupEdgeBasedAcceptanceScoreMinText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_EdgeBasedMatching.png"));

                shellHost.RecipeCommands.SelectedLlmToolTemplate = "Mean Intensity";
                shellHost.RecipeCommands.MeanIntentRoiText = string.Empty;
                shellHost.RecipeCommands.MeanIntentTypeText = "Mean";
                shellHost.RecipeCommands.MeanIntentMinimumText = "185";
                shellHost.RecipeCommands.MeanIntentMaximumText = "220";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup Mean inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>Mean</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>MEAN_TYPES</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>USE_ROI</Key>", StringComparison.OrdinalIgnoreCase)
                    || shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>CvROI</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>MeanValueAvg</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricMinimum>185</AcceptanceMetricMinimum>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricMaximum>220</AcceptanceMetricMaximum>", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup Mean Starter XML triggered Preview/Run or missed MeanValueAvg gates.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup Mean inputs",
                    "HostRecipeGuidedSetupMeanInputs",
                    "HostRecipeGuidedSetupMeanRoiText",
                    "HostRecipeGuidedSetupMeanTypeSelector",
                    "HostRecipeGuidedSetupMeanMinimumText",
                    "HostRecipeGuidedSetupMeanMaximumText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_Mean.png"));

                int referenceDifferenceLayerCountBefore = shellHost.LayerDocumentCount;
                string referenceDifferenceInputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string referenceDifferenceOutputRouteBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                string referenceDifferenceActiveLayerBefore = shellHost.ActiveHostLayerTitle;
                string[] referenceDifferencePaths = Enumerable.Range(1, 4)
                    .Select(index => Path.Combine(FindRepositoryRoot(), "Sample", "EasyMatch", "Die Pad " + index + ".bmp"))
                    .ToArray();
                if (referenceDifferencePaths.Any(path => !File.Exists(path)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup ReferenceDifference references were not found: "
                        + string.Join(" | ", referenceDifferencePaths.Where(path => !File.Exists(path))));
                }

                shellHost.RecipeCommands.SelectedLlmToolTemplate = OpenVisionGuidedSetupCatalog.ReferenceDifferenceTemplate;
                shellHost.RecipeCommands.LlmReferenceImagePath = referenceDifferencePaths[0];
                shellHost.RecipeCommands.ReferenceDifferencePath2 = referenceDifferencePaths[1];
                shellHost.RecipeCommands.ReferenceDifferencePath3 = referenceDifferencePaths[2];
                shellHost.RecipeCommands.ReferenceDifferencePath4 = referenceDifferencePaths[3];
                shellHost.RecipeCommands.ReferenceDifferenceThresholdText = "35";
                shellHost.RecipeCommands.ReferenceDifferenceMinimumAreaText = "80";
                shellHost.RecipeCommands.ReferenceDifferenceMaximumAreaText = "20000";
                Pump(60);
                if (!shellHost.RecipeCommands.IsGuidedSetupIntentInputReady
                    || !shellHost.RecipeCommands.GuidedSetupIntentInputStatusText.Contains("ResultCount=0", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke Guided setup ReferenceDifference inputs were not ready. "
                        + shellHost.RecipeCommands.GuidedSetupIntentInputStatusText);
                }

                shellHost.RecipeCommands.CreateGuidedSetupStarterXmlCommand.Execute(null);
                Pump(100);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<ToolType>ReferenceDifference</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<Key>ReferencePath4</Key>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricName>ResultCount</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftText.Contains("<AcceptanceMetricMaximum>0</AcceptanceMetricMaximum>", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || !shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null)
                    || shellHost.RecipeCommands.LlmXmlDraftDependencyRows.Count < 4
                    || shellHost.NativePreviewRunCount != guidedSetupRunsBefore
                    || shellHost.LayerDocumentCount != referenceDifferenceLayerCountBefore
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, referenceDifferenceInputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, referenceDifferenceOutputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, referenceDifferenceActiveLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke Guided setup ReferenceDifference draft was invalid or caused execution/layer/route side effects.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke Guided setup ReferenceDifference inputs",
                    "HostRecipeGuidedSetupReferenceDifferenceInputs",
                    "HostRecipeGuidedSetupReferenceDifferencePath1Text",
                    "HostRecipeGuidedSetupReferenceDifferencePath2Text",
                    "HostRecipeGuidedSetupReferenceDifferencePath3Text",
                    "HostRecipeGuidedSetupReferenceDifferencePath4Text",
                    "HostRecipeGuidedSetupReferenceDifferenceThresholdText",
                    "HostRecipeGuidedSetupReferenceDifferenceMinimumAreaText",
                    "HostRecipeGuidedSetupReferenceDifferenceMaximumAreaText",
                    "HostRecipeGuidedSetupIntentInputStatus");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_GuidedSetup_ReferenceDifference.png"));

                shellHost.RecipeCommands.SelectedLlmToolTemplate = guidedSetupTemplateBefore;
                shellHost.RecipeCommands.LlmXmlDraftText = guidedSetupDraftBefore;
                shellHost.RecipeCommands.LlmReferenceImagePath = guidedSetupReferenceBefore;
                shellHost.RecipeCommands.MatchingIntentSearchRoiText = guidedSetupMatchingRoiBefore;
                shellHost.RecipeCommands.MatchingIntentScoreMinText = guidedSetupMatchingScoreBefore;
                shellHost.RecipeCommands.MatchingIntentExpectedCountText = guidedSetupMatchingCountBefore;
                shellHost.RecipeCommands.FeatureMatchingIntentScoreMinText = guidedSetupFeatureScoreBefore;
                shellHost.RecipeCommands.FeatureMatchingIntentRansacReprojThresholdText = guidedSetupFeatureRansacBefore;
                shellHost.RecipeCommands.FeatureMatchingIntentAcceptanceScoreMinText = guidedSetupFeatureAcceptanceBefore;
                shellHost.RecipeCommands.EdgeBasedIntentScoreMinText = guidedSetupEdgeScoreBefore;
                shellHost.RecipeCommands.EdgeBasedIntentSearchCountText = guidedSetupEdgeSearchCountBefore;
                shellHost.RecipeCommands.EdgeBasedIntentCannyLowText = guidedSetupEdgeCannyLowBefore;
                shellHost.RecipeCommands.EdgeBasedIntentCannyHighText = guidedSetupEdgeCannyHighBefore;
                shellHost.RecipeCommands.EdgeBasedIntentAcceptanceScoreMinText = guidedSetupEdgeAcceptanceBefore;
                shellHost.RecipeCommands.ReferenceDifferencePath2 = guidedSetupReferenceDifferencePath2Before;
                shellHost.RecipeCommands.ReferenceDifferencePath3 = guidedSetupReferenceDifferencePath3Before;
                shellHost.RecipeCommands.ReferenceDifferencePath4 = guidedSetupReferenceDifferencePath4Before;
                shellHost.RecipeCommands.ReferenceDifferenceThresholdText = guidedSetupReferenceDifferenceThresholdBefore;
                shellHost.RecipeCommands.ReferenceDifferenceMinimumAreaText = guidedSetupReferenceDifferenceMinimumAreaBefore;
                shellHost.RecipeCommands.ReferenceDifferenceMaximumAreaText = guidedSetupReferenceDifferenceMaximumAreaBefore;
                shellHost.RecipeCommands.MeanIntentRoiText = guidedSetupMeanRoiBefore;
                shellHost.RecipeCommands.MeanIntentTypeText = guidedSetupMeanTypeBefore;
                shellHost.RecipeCommands.MeanIntentMinimumText = guidedSetupMeanMinimumBefore;
                shellHost.RecipeCommands.MeanIntentMaximumText = guidedSetupMeanMaximumBefore;
                pipelineTab.IsSelected = true;
                Pump(40);

                OpenVisionRecipeSampleOption sampleOption = shellHost.RecipeCommands.SampleOptions
                    .FirstOrDefault(option => option?.Sample != null
                        && option.Sample.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Product
                        && option.Sample.CanOpen
                        && !option.Sample.ExpectsFailure)
                    ?? shellHost.RecipeCommands.SampleOptions.FirstOrDefault(option => option?.Sample?.CanOpen == true);
                if (sampleOption == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not find a runnable sample.");
                }

                shellHost.RecipeCommands.SelectedSampleOption = sampleOption;
                if (!shellHost.RecipeCommands.DuplicatePipelineFromSampleOption(sampleOption))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not duplicate the selected sample pipeline.");
                }

                Pump(80);
                OpenVisionRecipePipelineOption activeVariant = shellHost.RecipeCommands.SelectedPipelineOption;
                OpenVisionRecipePipelineOption comparisonVariant = shellHost.RecipeCommands.PipelineOptions
                    .FirstOrDefault(option => option != null
                        && string.Equals(option.PipelineName, pipelineName, StringComparison.OrdinalIgnoreCase));
                if (activeVariant == null || comparisonVariant == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not prepare active/selected pipeline variants.");
                }

                int variantComparisonRunsBefore = shellHost.NativePreviewRunCount;
                shellHost.RecipeCommands.SelectedPipelineOption = comparisonVariant;
                Pump(60);
                string variantComparisonReport = shellHost.RecipeCommands.PipelineVariantComparisonReport;
                if (!variantComparisonReport.Contains(activeVariant.PipelineName, StringComparison.OrdinalIgnoreCase)
                    || !variantComparisonReport.Contains(comparisonVariant.PipelineName, StringComparison.OrdinalIgnoreCase)
                    || (!variantComparisonReport.Contains("변경 요약", StringComparison.OrdinalIgnoreCase)
                        && !variantComparisonReport.Contains("Change summary", StringComparison.OrdinalIgnoreCase))
                    || shellHost.NativePreviewRunCount != variantComparisonRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pipeline variant comparison was incomplete or triggered Preview/Run. "
                        + variantComparisonReport);
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pipeline variant comparison",
                    "HostRecipePipelineVariantComparison");
                shellHost.RecipeCommands.SelectedPipelineOption = activeVariant;
                Pump(60);
                shellHost.RecipeCommands.RunSelectedSampleCheckCommand.Execute(null);
                Stopwatch sampleCheckStopwatch = Stopwatch.StartNew();
                while (!shellHost.RecipeCommands.LatestSampleRunSummary.HasResult)
                {
                    Pump(8);
                    Thread.Sleep(20);
                    if (sampleCheckStopwatch.Elapsed > TimeSpan.FromSeconds(20))
                    {
                        throw new TimeoutException("Recipe manager sample check did not complete within 20 seconds.");
                    }
                }

                shellHost.RecipeCommands.RunSelectedSamplePairCheckCommand.Execute(null);
                Stopwatch pairCheckStopwatch = Stopwatch.StartNew();
                while (!shellHost.RecipeCommands.LatestPairRunSummary.HasResult)
                {
                    Pump(8);
                    Thread.Sleep(20);
                    if (pairCheckStopwatch.Elapsed > TimeSpan.FromSeconds(30))
                    {
                        throw new TimeoutException("Recipe manager pair check did not complete within 30 seconds.");
                    }
                }

                if (!shellHost.RecipeCommands.LatestPairRunSummary.StatusText.Contains("OK"))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke expected pair check OK. "
                        + shellHost.RecipeCommands.LatestPairRunSummary.DisplayText);
                }

                string pairCheckStatusText = shellHost.RecipeCommands.LatestPairRunSummary.StatusText;
                int pairRoleCardCount = shellHost.RecipeCommands.LatestPairRunSummary.SampleResults.Count;
                string validationSuitePipelineName = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? pipelineName;
                int validationSuiteHistoryCountBefore = VisionPipelineBatchRunSummaryStorage
                    .List(recipeName, validationSuitePipelineName)
                    .Count;
                OpenVisionRecipeValidationSuiteScopeOption selectedSampleSuiteScope =
                    shellHost.RecipeCommands.ValidationSuiteScopeOptions.FirstOrDefault(option =>
                        string.Equals(option.Key, OpenVisionRecipeValidationSuiteScopeOption.SelectedSampleKey, StringComparison.OrdinalIgnoreCase));
                if (selectedSampleSuiteScope == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not find the selected-sample validation suite scope.");
                }

                shellHost.RecipeCommands.SelectedValidationSuiteScopeOption = selectedSampleSuiteScope;
                if (!shellHost.RecipeCommands.RunValidationSuiteCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke validation suite command was disabled for selected sample.");
                }

                shellHost.RecipeCommands.RunValidationSuiteCommand.Execute(null);
                Stopwatch validationSuiteStopwatch = Stopwatch.StartNew();
                while (VisionPipelineBatchRunSummaryStorage.List(recipeName, validationSuitePipelineName).Count <= validationSuiteHistoryCountBefore)
                {
                    Pump(8);
                    Thread.Sleep(20);
                    if (validationSuiteStopwatch.Elapsed > TimeSpan.FromSeconds(20))
                    {
                        throw new TimeoutException("Recipe manager validation suite did not save selected-sample history within 20 seconds.");
                    }
                }

                IReadOnlyList<OpenVisionRecipeBatchRunOption> validationSuiteRecentRuns =
                    shellHost.RecipeCommands.RecentBatchRunOptions ?? Array.Empty<OpenVisionRecipeBatchRunOption>();
                OpenVisionRecipeBatchRunOption selectedSampleSuiteRun = validationSuiteRecentRuns.FirstOrDefault(option => option != null
                    && option.SampleResults.Any(result => string.Equals(result.SampleName, sampleOption.SampleName, StringComparison.OrdinalIgnoreCase)));
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.ValidationSuiteStatusText)
                    || (!shellHost.RecipeCommands.ValidationSuiteStatusText.Contains("saved", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.ValidationSuiteStatusText.Contains("저장", StringComparison.OrdinalIgnoreCase))
                    || selectedSampleSuiteRun == null)
                {
                    throw new InvalidOperationException(
                        "Recipe manager validation suite did not expose saved selected-sample evidence. "
                        + $"Status='{shellHost.RecipeCommands.ValidationSuiteStatusText}'");
                }

                VisionPipelineBatchRunSummary selectedSampleSuiteSummary =
                    VisionPipelineBatchRunSummaryStorage.Load(selectedSampleSuiteRun.SummaryPath);
                VisionPipelineBatchSampleRunResult selectedSampleSuiteResult = selectedSampleSuiteSummary?.Results
                    .FirstOrDefault(result => string.Equals(result.SampleName, sampleOption.SampleName, StringComparison.OrdinalIgnoreCase));
                VisionPipelineRunReport selectedSampleRunReport = string.IsNullOrWhiteSpace(selectedSampleSuiteResult?.RunReportPath)
                    ? null
                    : VisionPipelineRunReportStorage.Load(selectedSampleSuiteResult.RunReportPath);
                if (selectedSampleSuiteResult == null
                    || string.IsNullOrWhiteSpace(selectedSampleSuiteResult.RunReportPath)
                    || !File.Exists(selectedSampleSuiteResult.RunReportPath)
                    || selectedSampleRunReport?.Steps == null
                    || selectedSampleRunReport.Steps.Count == 0
                    || !selectedSampleRunReport.Steps.Any(step => step.ElapsedMilliseconds > 0D))
                {
                    throw new InvalidOperationException(
                        "Recipe manager validation suite did not persist a linked structured Step report. "
                        + $"RunReportPath='{selectedSampleSuiteResult?.RunReportPath}', Steps={selectedSampleRunReport?.Steps?.Count ?? 0}");
                }

                VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis selectedSampleStepTiming =
                    selectedSampleSuiteRun.StepTimingAnalysis;
                if (!selectedSampleStepTiming.IsAvailable
                    || selectedSampleStepTiming.SampleCount != 1
                    || selectedSampleStepTiming.ReportCount != 1
                    || selectedSampleStepTiming.Steps.Count == 0
                    || !selectedSampleStepTiming.Steps.Any(step => step.TimingCount == 1)
                    || selectedSampleSuiteRun.StepTimingRows.Count != selectedSampleStepTiming.Steps.Count
                    || !selectedSampleSuiteRun.StepTimingStatusText.Contains("1/1", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager validation suite did not aggregate its linked Step report. "
                        + $"Availability={selectedSampleStepTiming.Availability}, Samples={selectedSampleStepTiming.SampleCount}, "
                        + $"Reports={selectedSampleStepTiming.ReportCount}, Steps={selectedSampleStepTiming.Steps.Count}, "
                        + $"Status='{selectedSampleSuiteRun.StepTimingStatusText}'");
                }

                string validationSuiteStepReportEvidenceDirectory = Path.Combine(outputDirectory, "validation-suite-step-report");
                Directory.CreateDirectory(validationSuiteStepReportEvidenceDirectory);
                string validationSuiteStepReportEvidencePath = Path.Combine(validationSuiteStepReportEvidenceDirectory, "report.xml");
                File.Copy(selectedSampleSuiteResult.RunReportPath, validationSuiteStepReportEvidencePath, overwrite: true);
                string validationSuitePipelineSnapshotPath = Path.Combine(
                    Path.GetDirectoryName(selectedSampleSuiteResult.RunReportPath) ?? string.Empty,
                    selectedSampleRunReport.PipelineSnapshotFile ?? string.Empty);
                if (!File.Exists(validationSuitePipelineSnapshotPath))
                {
                    throw new InvalidOperationException(
                        "Recipe manager validation suite Step report did not retain its pipeline snapshot. "
                        + validationSuitePipelineSnapshotPath);
                }

                File.Copy(
                    validationSuitePipelineSnapshotPath,
                    Path.Combine(validationSuiteStepReportEvidenceDirectory, "pipeline.xml"),
                    overwrite: true);

                OpenVisionRecipePipelineStepPreview failedStepPreview =
                    shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.FirstOrDefault()
                    ?? throw new InvalidOperationException("Recipe manager direct smoke could not find a preview step for failure-link verification.");

                OpenVisionRecipePairSampleRunSummary forcedFailedRole =
                    OpenVisionRecipePairSampleRunSummary.CreateForTest(
                        "Bad",
                        sampleOption.SampleName + "_ForcedBad",
                        "NG",
                        false,
                        "Metric gate failed for smoke",
                        "Smoke forced role failure.",
                        failedStepPreview.Name);
                shellHost.RecipeCommands.SetPairRunSummaryForTest(new[]
                {
                    OpenVisionRecipePairSampleRunSummary.CreateForTest(
                        "Good",
                        sampleOption.SampleName + "_ForcedGood",
                        "OK",
                        true,
                        "Metric gate matched for smoke",
                        "Smoke role pass.",
                        string.Empty),
                    forcedFailedRole
                });
                Pump(40);
                if (!shellHost.RecipeCommands.SelectPairSampleResultCommand.CanExecute(forcedFailedRole))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke role drill-down command was not enabled.");
                }

                shellHost.RecipeCommands.SelectPairSampleResultCommand.Execute(forcedFailedRole);
                Pump(40);
                if (!ReferenceEquals(shellHost.RecipeCommands.SelectedPairSampleResult, forcedFailedRole)
                    || shellHost.RecipeCommands.SelectedPipelinePreviewStep == null
                    || shellHost.RecipeCommands.SelectedPipelinePreviewStep.Index != failedStepPreview.Index)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke role drill-down did not focus the failed step. "
                        + $"Expected={failedStepPreview.Index}:{failedStepPreview.Name}, Actual={shellHost.RecipeCommands.SelectedPipelinePreviewStep?.Index}:{shellHost.RecipeCommands.SelectedPipelinePreviewStep?.Name}");
                }

                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.SelectedPairSampleResult.ReviewText)
                    || !shellHost.RecipeCommands.SelectedPairSampleResult.ReviewText.Contains(failedStepPreview.Name, StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.SelectedPairSampleResult.ReviewText.Contains("Next", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.SelectedPairSampleResult.ReviewText.Contains("다음", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke role drill-down did not expose failed-step correction guidance. "
                        + shellHost.RecipeCommands.SelectedPairSampleResult.ReviewText);
                }

                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorRunReviewText)
                    || !shellHost.RecipeCommands.OperatorRunReviewText.Contains(forcedFailedRole.Role, StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.OperatorRunReviewText.Contains("Role next", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorRunReviewText.Contains("역할 다음", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke role drill-down did not update the operator run review summary. "
                        + shellHost.RecipeCommands.OperatorRunReviewText);
                }

                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.RecipeGuidedSetupText)
                    || (!shellHost.RecipeCommands.RecipeGuidedSetupText.Contains("Guide", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.RecipeGuidedSetupText.Contains("가이드", StringComparison.OrdinalIgnoreCase))
                    || !shellHost.RecipeCommands.RecipeGuidedSetupText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.RecipeGuidedSetupText.Contains("Next", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.RecipeGuidedSetupText.Contains("다음", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke guided setup strip did not expose the commercial-style review path. "
                        + shellHost.RecipeCommands.RecipeGuidedSetupText);
                }
                if (!shellHost.RecipeCommands.RunRecipeGuidedNextActionCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke guided next action command was disabled during failed-step review.");
                }
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.RecipeGuidedNextActionText)
                    || (!shellHost.RecipeCommands.RecipeGuidedNextActionText.Contains("tool", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.RecipeGuidedNextActionText.Contains("도구", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.RecipeGuidedNextActionText.Contains("parameter", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.RecipeGuidedNextActionText.Contains("파라미터", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke guided next action label did not expose the concrete failed-step action. "
                        + shellHost.RecipeCommands.RecipeGuidedNextActionText);
                }

                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorDecisionXmlCardText)
                    || !shellHost.RecipeCommands.OperatorDecisionXmlCardText.Contains("Step", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorDecisionSampleCardText)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorDecisionPairCardText)
                    || !shellHost.RecipeCommands.OperatorDecisionPairCardText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorDecisionNextActionText)
                    || !shellHost.RecipeCommands.OperatorDecisionNextActionText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorDecisionEvidenceText)
                    || !shellHost.RecipeCommands.OperatorDecisionEvidenceText.Contains("Metric review", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator decision board was incomplete. "
                        + $"Xml='{shellHost.RecipeCommands.OperatorDecisionXmlCardText}', "
                        + $"Sample='{shellHost.RecipeCommands.OperatorDecisionSampleCardText}', "
                        + $"Pair='{shellHost.RecipeCommands.OperatorDecisionPairCardText}', "
                        + $"Next='{shellHost.RecipeCommands.OperatorDecisionNextActionText}', "
                        + $"Evidence='{shellHost.RecipeCommands.OperatorDecisionEvidenceText}'");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_RoleDrilldown.png"));
                string selectedPipelineName = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? pipelineName;
                string sampleImagePath = sampleOption.Sample?.ImageFullPath ?? string.Empty;
                DateTime failedStartedAt = DateTime.Now.AddSeconds(1);
                DateTime baselineStartedAt = failedStartedAt.AddSeconds(-1);
                DateTime olderStartedAt = baselineStartedAt.AddSeconds(-1);
                string olderSummaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    selectedPipelineName,
                    olderStartedAt,
                    olderStartedAt.AddMilliseconds(36D),
                    new[]
                    {
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedGood",
                            Status = "OK",
                            Success = true,
                            TotalMilliseconds = 2.0D,
                            Message = "Smoke older pass row.",
                            ReportPath = sampleImagePath
                        },
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedBad",
                            Status = "NG",
                            Success = false,
                            TotalMilliseconds = 3.3D,
                            FailedStep = failedStepPreview.Name,
                            Message = "Smoke older failure row for baseline selection.",
                            ReportPath = sampleImagePath
                        }
                    },
                    "Other Direct Smoke Set",
                    "Catalog");
                string baselineSummaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    selectedPipelineName,
                    baselineStartedAt,
                    baselineStartedAt.AddMilliseconds(38D),
                    new[]
                    {
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedGood",
                            Status = "OK",
                            Success = true,
                            TotalMilliseconds = 2.1D,
                            Message = "Smoke baseline pass row.",
                            ReportPath = sampleImagePath
                        },
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedBad",
                            Status = "OK",
                            Success = true,
                            TotalMilliseconds = 2.8D,
                            Message = "Smoke baseline pass row.",
                            ReportPath = sampleImagePath
                        }
                    },
                    "Direct Smoke Benchmark",
                    "DirectSmokeBenchmark");
                string failedSummaryPath = VisionPipelineBatchRunSummaryStorage.Save(
                    recipeName,
                    selectedPipelineName,
                    failedStartedAt,
                    failedStartedAt.AddMilliseconds(42D),
                    new[]
                    {
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedGood",
                            Status = "OK",
                            Success = true,
                            TotalMilliseconds = 2.4D,
                            Message = "Smoke pass row.",
                            ReportPath = sampleImagePath
                        },
                        new VisionPipelineBatchSampleRunResult
                        {
                            SampleName = sampleOption.SampleName + "_ForcedBad",
                            Status = "NG",
                            Success = false,
                            TotalMilliseconds = 3.1D,
                            FailedStep = failedStepPreview.Name,
                            Message = "Smoke forced failure for step-link verification.",
                            ReportPath = sampleImagePath
                        }
                    },
                    "Direct Smoke Benchmark",
                    "DirectSmokeBenchmark");
                shellHost.RecipeCommands.RefreshRecentBatchRunOptionsForTest();
                shellHost.RecipeCommands.SelectedRecentBatchRunOption =
                    shellHost.RecipeCommands.RecentBatchRunOptions.FirstOrDefault(option =>
                        string.Equals(option.SummaryPath, failedSummaryPath, StringComparison.OrdinalIgnoreCase));
                Pump(40);
                if (shellHost.RecipeCommands.SelectedRecentBatchRunOption == null
                    || !string.Equals(shellHost.RecipeCommands.SelectedRecentBatchRunOption.SummaryPath, failedSummaryPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke did not select the forced failed batch run.");
                }

                VisionPipelineBatchRunSummaryStorage.BatchStepTimingAnalysis forcedRunStepTiming =
                    shellHost.RecipeCommands.SelectedRecentBatchRunOption.StepTimingAnalysis;
                if (forcedRunStepTiming.IsAvailable
                    || forcedRunStepTiming.Availability
                        != VisionPipelineBatchRunSummaryStorage.StepTimingAvailability.MissingReportPath
                    || shellHost.RecipeCommands.SelectedRecentBatchRunOption.StepTimingRows.Count != 0)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not block partial Step timing without linked reports. "
                        + forcedRunStepTiming.Availability);
                }

                string compatiblePerformanceComparison = shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText;
                if ((!compatiblePerformanceComparison.Contains("성능 비교:", StringComparison.Ordinal)
                        && !compatiblePerformanceComparison.Contains("Performance comparison:", StringComparison.OrdinalIgnoreCase))
                    || !compatiblePerformanceComparison.Contains("+0.3", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not compare timing for equivalent validation sample sets. "
                        + compatiblePerformanceComparison);
                }

                VisionPipelineBatchRunSummaryStorage.BatchRunStatistics failedRunStatistics =
                    shellHost.RecipeCommands.SelectedRecentBatchRunOption.Statistics;
                if (failedRunStatistics.ResultCount != 2
                    || failedRunStatistics.TimingCount != 2
                    || failedRunStatistics.FailureCount != 1
                    || Math.Abs(failedRunStatistics.FailureRatePercent - 50D) > 0.001D
                    || Math.Abs(failedRunStatistics.AverageMilliseconds - 2.75D) > 0.001D
                    || Math.Abs(failedRunStatistics.MedianMilliseconds - 2.75D) > 0.001D
                    || Math.Abs(failedRunStatistics.P95Milliseconds - 3.1D) > 0.001D
                    || Math.Abs(failedRunStatistics.MaximumMilliseconds - 3.1D) > 0.001D
                    || !shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("p95", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("%", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke batch analytics were incomplete. "
                        + $"Results={failedRunStatistics.ResultCount}, Timings={failedRunStatistics.TimingCount}, "
                        + $"Failures={failedRunStatistics.FailureCount}, FailureRate={failedRunStatistics.FailureRatePercent:0.###}, "
                        + $"Avg={failedRunStatistics.AverageMilliseconds:0.###}, Median={failedRunStatistics.MedianMilliseconds:0.###}, "
                        + $"P95={failedRunStatistics.P95Milliseconds:0.###}, Max={failedRunStatistics.MaximumMilliseconds:0.###}, "
                        + $"Summary='{shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText}'");
                }

                if (shellHost.RecipeCommands.RecentBatchRunComparisonRows == null
                    || !shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsRegression)
                    || !shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("Regression", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.SelectedRecentBatchRunComparisonReviewText.Contains("REGRESSION", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose benchmark regression comparison. "
                        + $"Summary='{shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText}', "
                        + $"Review='{shellHost.RecipeCommands.SelectedRecentBatchRunComparisonReviewText}'");
                }

                if (shellHost.RecipeCommands.BenchmarkBaselineRunOptions == null
                    || shellHost.RecipeCommands.BenchmarkBaselineRunOptions.Count < 2
                    || !string.Equals(shellHost.RecipeCommands.SelectedBenchmarkBaselineRunOption?.SummaryPath, baselineSummaryPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose selectable benchmark baselines. "
                        + $"Selected='{shellHost.RecipeCommands.SelectedBenchmarkBaselineRunOption?.SummaryPath}', "
                        + $"Expected='{baselineSummaryPath}', Count={shellHost.RecipeCommands.BenchmarkBaselineRunOptions?.Count ?? 0}");
                }

                OpenVisionRecipeBatchRunOption olderBaselineOption =
                    shellHost.RecipeCommands.BenchmarkBaselineRunOptions.FirstOrDefault(option =>
                        string.Equals(option.SummaryPath, olderSummaryPath, StringComparison.OrdinalIgnoreCase));
                if (olderBaselineOption == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not find the older selectable benchmark baseline.");
                }

                shellHost.RecipeCommands.SelectedBenchmarkBaselineRunOption = olderBaselineOption;
                Pump(60);
                if (!shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsStillFailing)
                    || shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsRegression)
                    || (!shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("성능 비교 안 함", StringComparison.Ordinal)
                        && !shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("Performance comparison skipped", StringComparison.OrdinalIgnoreCase))
                    || shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains(" -> ", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke benchmark baseline selection did not keep outcome comparison while blocking unrelated timing. "
                        + $"Summary='{shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText}'");
                }

                OpenVisionRecipeBatchRunOption defaultBaselineOption =
                    shellHost.RecipeCommands.BenchmarkBaselineRunOptions.FirstOrDefault(option =>
                        string.Equals(option.SummaryPath, baselineSummaryPath, StringComparison.OrdinalIgnoreCase));
                shellHost.RecipeCommands.SelectedBenchmarkBaselineRunOption = defaultBaselineOption;
                Pump(60);
                if (!shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsRegression)
                    || !shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Contains("+0.3", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke could not restore the default outcome and timing baseline. "
                        + shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText);
                }

                OpenVisionRecipeBatchSampleResultOption selectedSampleResult =
                    shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption;
                if (selectedSampleResult == null
                    || selectedSampleResult.Success
                    || !string.Equals(selectedSampleResult.FailedStep, failedStepPreview.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not select the failed sample result. "
                        + $"ExpectedStep={failedStepPreview.Name}, ActualStep={selectedSampleResult?.FailedStep}, Success={selectedSampleResult?.Success}");
                }

                if (string.IsNullOrWhiteSpace(selectedSampleResult.ReviewText)
                    || !selectedSampleResult.ReviewText.Contains("Step", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose the failed sample review text. "
                        + $"Review='{selectedSampleResult?.ReviewText}'");
                }

                OpenVisionRecipePipelineStepPreview selectedPreviewStep =
                    shellHost.RecipeCommands.SelectedPipelinePreviewStep;
                if (selectedPreviewStep == null
                    || selectedPreviewStep.Index != failedStepPreview.Index
                    || !string.Equals(selectedPreviewStep.Name, failedStepPreview.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not link failed sample to the matching preview step. "
                        + $"Expected={failedStepPreview.Index}:{failedStepPreview.Name}, Actual={selectedPreviewStep?.Index}:{selectedPreviewStep?.Name}");
                }

                if (string.IsNullOrWhiteSpace(selectedPreviewStep.RouteText)
                    || string.IsNullOrWhiteSpace(selectedPreviewStep.ParameterPreviewText)
                    || selectedPreviewStep.ParameterPreviewText.Contains("none", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose a readable step route/parameter summary. "
                        + $"Route='{selectedPreviewStep.RouteText}', Params='{selectedPreviewStep.ParameterPreviewText}'");
                }

                string selectedRunReviewText = shellHost.RecipeCommands.SelectedRecentBatchRunReviewText ?? string.Empty;
                if (!selectedRunReviewText.Contains(selectedPreviewStep.Name, StringComparison.OrdinalIgnoreCase)
                    || !selectedRunReviewText.Contains(selectedSampleResult.FailedStep, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not show failed-step details in selected run review. "
                        + $"Expected='{selectedPreviewStep.Name}', Review='{selectedRunReviewText}'");
                }
                string[] selectedRunReviewLines = selectedRunReviewText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (!selectedRunReviewLines
                    .Take(4)
                    .Any(line => line.Contains("다음:", StringComparison.Ordinal)
                        || line.Contains("Next:", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager selected run review did not expose its next action in the first four lines. "
                        + selectedRunReviewText);
                }

                if (!shellHost.RecipeCommands.FocusSelectedRunFailureStepCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke failed-step focus command was not enabled.");
                }

                shellHost.RecipeCommands.FocusSelectedRunFailureStepCommand.Execute(null);
                Pump(40);
                if (shellHost.RecipeCommands.SelectedPipelinePreviewStep == null
                    || shellHost.RecipeCommands.SelectedPipelinePreviewStep.Index != failedStepPreview.Index)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke failed-step focus command did not select the failed preview step.");
                }

                if (!shellHost.RecipeCommands.LoadSelectedRunSampleImageToInputLayerCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke sample-to-input command was not enabled. "
                        + $"SampleImage='{sampleImagePath}', ReportPath='{selectedSampleResult.ReportPath}'");
                }

                int beforeSampleInputLoadRuns = shellHost.NativePreviewRunCount;
                shellHost.RecipeCommands.LoadSelectedRunSampleImageToInputLayerCommand.Execute(null);
                Pump(160);
                if (!string.Equals(shellHost.WorkspaceLayerTitle, selectedPreviewStep.InputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke sample-to-input command did not activate the failed step input layer. "
                        + $"Expected={selectedPreviewStep.InputLayer}, Actual={shellHost.WorkspaceLayerTitle}");
                }

                if (shellHost.NativePreviewRunCount != beforeSampleInputLoadRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke sample-to-input command triggered an automatic preview/run. "
                        + $"Before={beforeSampleInputLoadRuns}, After={shellHost.NativePreviewRunCount}");
                }

                int variantCaptureRunsBefore = shellHost.NativePreviewRunCount;
                shellHost.RecipeCommands.SelectedPipelineOption = comparisonVariant;
                Pump(180);
                window.UpdateLayout();
                Pump(120);
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_PipelineVariantComparison.png"));
                shellHost.RecipeCommands.SelectedPipelineOption = activeVariant;
                Pump(120);
                if (shellHost.NativePreviewRunCount != variantCaptureRunsBefore)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke variant comparison capture triggered Preview/Run.");
                }

                const string pipelineFilterText = "CellVent";
                shellHost.RecipeCommands.PipelineFilterText = pipelineFilterText;
                Pump(20);
                if (shellHost.RecipeCommands.FilteredPipelineOptions.Count == 0
                    || shellHost.RecipeCommands.FilteredPipelineOptions.Any(option =>
                        option == null
                        || option.PipelineName.IndexOf(pipelineFilterText, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pipeline filter did not narrow the pipeline list. "
                        + $"Filter='{pipelineFilterText}', Count={shellHost.RecipeCommands.FilteredPipelineOptions.Count}");
                }

                TextBox pipelineFilterTextBox = FindVisualChildren<TextBox>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostRecipePipelineFilterTextBox",
                        StringComparison.Ordinal));
                if (pipelineFilterTextBox == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not find the pipeline filter text box.");
                }

                pipelineFilterTextBox.BringIntoView();
                Pump(20);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_PipelineFilter.png"));

                Pump(40);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pipeline review tab",
                    "HostRecipeManagerPanel",
                    "HostRecipeManagerTitleBar",
                    "HostRecipeManagerCloseButton",
                    "HostRecipeManagerWorkbenchHeader",
                    "HostRecipeManagerWorkbenchGrid",
                    "HostRecipeDetailText",
                    "HostRecipeDetailTabs",
                    "HostRecipePipelineTab",
                    "HostRecipePipelineManagerList",
                    "HostRecipePipelineFilterTextBox",
                    "HostRecipePipelineNameEditor",
                    "HostRecipePipelineEditValidation",
                    "HostRecipePipelineActivateButton",
                    "HostRecipePipelineDuplicateButton",
                    "HostRecipePipelineRenameButton",
                    "HostRecipePipelineDeleteButton",
                    "HostRecipeSampleSelector",
                    "HostRecipeDuplicateFromSampleButton",
                    "HostRecipeSampleAcceptanceSummary",
                    "HostRecipeRunSampleCheckButton",
                    "HostRecipeSampleCheckSummary",
                    "HostRecipeRunPairCheckButton",
                    "HostRecipePairCheckSummary",
                    "HostRecipeSampleMatrixPanel",
                    "HostRecipeSampleMatrixSummary",
                    "HostRecipeSampleMatrixList",
                    "HostRecipeSelectedSampleMatrixReview",
                    "HostRecipeRunCatalogBenchmarkCompactButton",
                    "HostRecipePipelineReviewPanel",
                    "HostRecipePipelineReviewTab",
                    "HostRecipePipelineVariantComparison",
                    "HostRecipePipelineReportTab",
                    "HostRecipeOperatorDecisionBoard",
                    "HostRecipeOperatorDecisionXmlCard",
                    "HostRecipeOperatorDecisionSampleCard",
                    "HostRecipeOperatorDecisionPairCard",
                    "HostRecipeOperatorDecisionNextAction",
                    "HostRecipeOperatorDecisionEvidence",
                    "HostRecipePipelineOperatorReview",
                    "HostRecipePipelineOperatorChecklist",
                    "HostRecipePipelineRunReview",
                    "HostRecipeCatalogBenchmarkPanel",
                    "HostRecipeCatalogBenchmarkSummary",
                    "HostRecipeRunCatalogBenchmarkButton",
                    "HostRecipeCatalogBenchmarkDetail",
                    "HostRecipeFailureRerunComparisonPanel",
                    "HostRecipeFailureRerunComparisonText",
                    "HostRecipeFailureViewOutputButton",
                    "HostRecipeFailureViewInputButton",
                    "HostRecipeFailureLoadParametersButton",
                    "HostRecipeFailureRerunPairButton",
                    "HostRecipeManagerNameStrip",
                    "HostRecipeAdvancedTransferCommands",
                    "HostRecipeImportXmlButton",
                    "HostRecipeExportXmlButton");
                AssertNotVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke advanced pipeline review",
                    "HostRecipeOverviewTab",
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeFilterTextBox",
                    "HostRecipeManagerList",
                    "HostRecipeGuidedSetupStrip",
                    "HostRecipeGuidedNextActionButton",
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeNameEditor",
                    "HostRecipeEditValidation",
                    "HostRecipeCreateNamedButton",
                    "HostRecipeDuplicateButton",
                    "HostRecipeRenameButton",
                    "HostRecipeDeleteButton");
                if (!shellHost.RecipeCommands.FailureReviewText.Contains(failedStepPreview.Name, StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.FailureReviewText.Contains("Compare", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.FailureReviewText.Contains("비교", StringComparison.OrdinalIgnoreCase))
                    || (!shellHost.RecipeCommands.FailureReviewText.Contains("Rerun", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.FailureReviewText.Contains("재검사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke failed-step rerun/comparison review was not actionable. "
                        + $"Text='{shellHost.RecipeCommands.FailureReviewText}'");
                }

                TabItem reportTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineReport",
                    "Recipe manager direct smoke");
                reportTab.IsSelected = true;
                Pump(40);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pipeline report tab",
                    "HostRecipePipelineReportTab",
                    "HostRecipeCopyOperatorHandoffReportButton",
                    "HostRecipeOperatorDecisionSummaryBand",
                    "HostRecipeOperatorDecisionSummaryStatus",
                    "HostRecipeOperatorDecisionSummaryMetric",
                    "HostRecipeOperatorDecisionSummaryNextAction",
                    "HostRecipeOperatorValidationChecklistPanel",
                    "HostRecipeOperatorValidationChecklist",
                    "HostRecipeOperatorResultChannelsPanel",
                    "HostRecipeOperatorResultChannelBoard",
                    "HostRecipeOperatorResultChannels",
                    "HostRecipeOperatorHandoffReport");
                if (!shellHost.RecipeCommands.OperatorDecisionSummaryStatusText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorDecisionSummaryStatusText.Contains("Inspection.FailedStep", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorDecisionSummaryStatusText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorDecisionEvidenceText.Contains("Metric review", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorDecisionEvidenceText.Contains("expected", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorDecisionEvidenceText.Contains("actual", StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.OperatorDecisionNextActionText.Contains("Next action", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorDecisionNextActionText.Contains("다음 작업", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke consolidated decision summary was incomplete. "
                        + shellHost.RecipeCommands.OperatorDecisionEvidenceText);
                }
                IReadOnlyList<OpenVisionRecipeOperatorValidationRow> validationRows =
                    shellHost.RecipeCommands.OperatorValidationChecklistRows;
                if (validationRows == null
                    || validationRows.Count < 5
                    || !validationRows.Any(row => string.Equals(row.StateText, "OK", StringComparison.OrdinalIgnoreCase))
                    || !validationRows.Any(row => string.Equals(row.StateText, "NG", StringComparison.OrdinalIgnoreCase))
                    || !validationRows.Any(row => row.ItemText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator validation checklist was incomplete. "
                        + $"Rows='{string.Join(" | ", validationRows?.Select(row => row.DisplayText) ?? Array.Empty<string>())}'");
                }

                IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannels =
                    shellHost.RecipeCommands.OperatorResultChannelRows;
                IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> resultChannelBoard =
                    shellHost.RecipeCommands.OperatorResultChannelBoardRows;
                if (resultChannels == null
                    || resultChannels.Count < 5
                    || !resultChannels.Any(row => row.ChannelText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase))
                    || !resultChannels.Any(row => row.ChannelText.Contains("Inspection.FailedStep", StringComparison.OrdinalIgnoreCase))
                    || !resultChannels.Any(row => row.ChannelText.Contains("Inspection.NextAction", StringComparison.OrdinalIgnoreCase))
                    || !resultChannels.Any(row => string.Equals(row.ValueText, "NG", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator result channels were incomplete. "
                        + $"Rows='{string.Join(" | ", resultChannels?.Select(row => row.DisplayText) ?? Array.Empty<string>())}'");
                }

                if (resultChannelBoard == null
                    || resultChannelBoard.Count < 5
                    || !resultChannelBoard.Any(row => row.ChannelText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase))
                    || !resultChannelBoard.Any(row => row.ChannelText.Contains("Inspection.Evidence", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator result channel board was incomplete. "
                        + $"Rows='{string.Join(" | ", resultChannelBoard?.Select(row => row.DisplayText) ?? Array.Empty<string>())}'");
                }

                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorHandoffReportText)
                    || !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("OpenVisionLab", StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Validation checklist", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("검증 체크리스트", StringComparison.OrdinalIgnoreCase))
                    || (!shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Judgement outputs", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("판정 출력 정의", StringComparison.OrdinalIgnoreCase))
                    || !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Metric evidence", StringComparison.OrdinalIgnoreCase)
                    || (!shellHost.RecipeCommands.OperatorHandoffReportText.Contains("Next action", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorHandoffReportText.Contains("다음 작업", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator handoff report was incomplete. "
                        + shellHost.RecipeCommands.OperatorHandoffReportText);
                }

                if (!shellHost.RecipeCommands.CopyOperatorHandoffReportCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke operator handoff report copy command was disabled.");
                }

                shellHost.RecipeCommands.CopyOperatorHandoffReportCommand.Execute(null);
                Pump(40);
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.OperatorHandoffReportStatusText)
                    || (!shellHost.RecipeCommands.OperatorHandoffReportStatusText.Contains("copied", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.OperatorHandoffReportStatusText.Contains("복사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator handoff report copy command did not report success. "
                        + shellHost.RecipeCommands.OperatorHandoffReportStatusText);
                }
                string operatorReportClipboard = GetClipboardTextWithRetry();
                if (!operatorReportClipboard.Contains("OpenVisionLab", StringComparison.OrdinalIgnoreCase)
                    || (!operatorReportClipboard.Contains("Validation checklist", StringComparison.OrdinalIgnoreCase)
                        && !operatorReportClipboard.Contains("검증 체크리스트", StringComparison.OrdinalIgnoreCase))
                    || (!operatorReportClipboard.Contains("Judgement outputs", StringComparison.OrdinalIgnoreCase)
                        && !operatorReportClipboard.Contains("판정 출력 정의", StringComparison.OrdinalIgnoreCase))
                    || !operatorReportClipboard.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !operatorReportClipboard.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke operator handoff report copy did not write the expected clipboard content.");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Report.png"));

                TabItem runHistoryTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineRunHistory",
                    "Recipe manager direct smoke");
                runHistoryTab.IsSelected = true;
                Pump(40);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pipeline run history tab",
                    "HostRecipePipelineRunHistoryTab",
                    "HostRecipeValidationSuitePanel",
                    "HostRecipeValidationSuiteScopeCombo",
                    "HostRecipeRunValidationSuiteButton",
                    "HostRecipeValidationSuiteStatus",
                    "HostRecipeValidationSuiteSummary",
                    "HostRecipeRecentBatchRunList",
                    "HostRecipeRecentBatchRunSampleList",
                    "HostRecipeRecentBatchRunSampleFilterPanel",
                    "HostRecipeRecentBatchRunNgOnlyToggle",
                    "HostRecipeRecentBatchRunNgFilterSummary",
                    "HostRecipeRecentBatchRunComparisonPanel",
                    "HostRecipeBenchmarkBaselineRunSelector",
                    "HostRecipeBenchmarkBaselineRunCombo",
                    "HostRecipeRecentBatchRunComparisonSummary",
                    "HostRecipeRecentBatchRunComparisonList",
                    "HostRecipeSelectedRunComparisonReview",
                    "HostRecipeCopySelectedRunReviewButton",
                    "HostRecipeSelectedRunReview");
                TextBox selectedRunReviewBox = FindNamedVisualChild<TextBox>(
                    shellHost,
                    "txtRecipeSelectedRunReview",
                    "Recipe manager direct smoke");
                if (selectedRunReviewBox.ActualHeight < 72D || selectedRunReviewBox.ActualHeight > 88D)
                {
                    throw new InvalidOperationException(
                        "Recipe manager selected run review did not keep its bounded scrollable height. "
                        + $"ActualHeight={selectedRunReviewBox.ActualHeight:0.0}");
                }
                shellHost.RecipeCommands.ShowRecentBatchNgOnly = true;
                Pump(20);
                IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> filteredRunSamples =
                    shellHost.RecipeCommands.FilteredRecentBatchRunSampleResults;
                if (filteredRunSamples == null
                    || filteredRunSamples.Count == 0
                    || filteredRunSamples.Any(result => result == null || result.Success)
                    || !shellHost.RecipeCommands.RecentBatchRunNgFilterSummaryText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                    || shellHost.RecipeCommands.SelectedRecentBatchSampleResultOption?.Success == true)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke run-history NG filter did not narrow to failed samples. "
                        + $"Summary='{shellHost.RecipeCommands.RecentBatchRunNgFilterSummaryText}', "
                        + $"Rows='{string.Join(" | ", filteredRunSamples?.Select(result => result?.DisplayText ?? "-") ?? Array.Empty<string>())}'");
                }
                if (!shellHost.RecipeCommands.CopySelectedRecentBatchRunReviewCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke selected run review copy command was disabled.");
                }

                shellHost.RecipeCommands.CopySelectedRecentBatchRunReviewCommand.Execute(null);
                Pump(40);
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText)
                    || (!shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText.Contains("copied", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText.Contains("복사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke selected run review copy command did not report success. "
                        + shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText);
                }
                string runReviewClipboard = GetClipboardTextWithRetry();
                if (!runReviewClipboard.Contains(forcedFailedRole.SampleName, StringComparison.OrdinalIgnoreCase)
                    || !runReviewClipboard.Contains(failedStepPreview.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke selected run review copy did not write the expected clipboard content.");
                }

                OpenVisionRecipeValidationSuiteScopeOption localValidationSetScope =
                    shellHost.RecipeCommands.ValidationSuiteScopeOptions.FirstOrDefault(option =>
                        string.Equals(option.Key, OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey, StringComparison.OrdinalIgnoreCase));
                if (localValidationSetScope == null)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke did not expose the local validation set scope.");
                }

                int localSetPreviewRunsBefore = shellHost.NativePreviewRunCount;
                int localSetLayerCountBefore = shellHost.LayerDocumentCount;
                string localSetWorkspaceLayerBefore = shellHost.WorkspaceLayerTitle;
                string localSetInputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string localSetOutputRouteBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                shellHost.RecipeCommands.SelectedValidationSuiteScopeOption = localValidationSetScope;
                shellHost.RecipeCommands.NewValidationSetName = "Direct Local Set";
                if (!shellHost.RecipeCommands.CreateValidationSetCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke local validation set create command was disabled.");
                }

                shellHost.RecipeCommands.CreateValidationSetCommand.Execute(null);
                string directValidationFolder = Path.Combine(outputDirectory, "validation-set-folder-input");
                string directValidationNestedFolder = Path.Combine(directValidationFolder, "nested");
                Directory.CreateDirectory(directValidationNestedFolder);
                string directValidationFolderImage = Path.Combine(
                    directValidationFolder,
                    "folder-expected-ng" + Path.GetExtension(sampleImagePath));
                string directValidationRepairedImage = Path.Combine(
                    directValidationFolder,
                    "folder-expected-ng-repaired" + Path.GetExtension(sampleImagePath));
                string directValidationNestedImage = Path.Combine(
                    directValidationNestedFolder,
                    "nested-must-not-register" + Path.GetExtension(sampleImagePath));
                File.Copy(sampleImagePath, directValidationFolderImage, overwrite: true);
                File.Copy(sampleImagePath, directValidationNestedImage, overwrite: true);
                if (!shellHost.RecipeCommands.AddValidationSetImagesForTest(
                        OpenVisionRecipeValidationSetImage.ExpectedOk,
                        new[] { sampleImagePath },
                        "Direct EXE registration proof")
                    || !shellHost.RecipeCommands.AddValidationSetFolderForTest(
                        OpenVisionRecipeValidationSetImage.ExpectedNg,
                        directValidationFolder,
                        "Direct EXE folder proof")
                    || shellHost.RecipeCommands.ValidationSetImageRows.Count != 2
                    || shellHost.RecipeCommands.SelectedValidationSetOption?.OkCount != 1
                    || shellHost.RecipeCommands.SelectedValidationSetOption?.NgCount != 1
                    || shellHost.RecipeCommands.ValidationSetImageRows.Any(row =>
                        string.Equals(row.Path, directValidationNestedImage, StringComparison.OrdinalIgnoreCase))
                    || shellHost.NativePreviewRunCount != localSetPreviewRunsBefore
                    || shellHost.LayerDocumentCount != localSetLayerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, localSetWorkspaceLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, localSetInputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, localSetOutputRouteBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke local validation set registration failed or changed runtime state. "
                        + shellHost.RecipeCommands.ValidationSetSelectionSummaryText);
                }

                File.Delete(directValidationFolderImage);
                shellHost.RecipeCommands.RefreshOptions();
                OpenVisionRecipeValidationSetImageRow directMissingValidationRow = shellHost.RecipeCommands.ValidationSetImageRows
                    .Single(row => string.Equals(row.Path, directValidationFolderImage, StringComparison.OrdinalIgnoreCase));
                shellHost.RecipeCommands.SelectedValidationSetImageRow = directMissingValidationRow;
                File.Copy(sampleImagePath, directValidationRepairedImage, overwrite: true);
                if (!shellHost.RecipeCommands.RepairValidationSetImagePathCommand.CanExecute(null)
                    || !shellHost.RecipeCommands.RepairValidationSetImagePathForTest(directValidationRepairedImage))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke local validation missing-path repair failed. "
                        + shellHost.RecipeCommands.ValidationSuiteStatusText);
                }

                OpenVisionRecipeValidationSetImageRow directRepairedValidationRow = shellHost.RecipeCommands.ValidationSetImageRows
                    .Single(row => string.Equals(row.Path, directValidationRepairedImage, StringComparison.OrdinalIgnoreCase));
                if (!string.Equals(directRepairedValidationRow.Expected, OpenVisionRecipeValidationSetImage.ExpectedNg, StringComparison.Ordinal)
                    || !string.Equals(directRepairedValidationRow.Notes, "Direct EXE folder proof", StringComparison.Ordinal)
                    || shellHost.RecipeCommands.SelectedValidationSetOption?.MissingCount != 0
                    || shellHost.RecipeCommands.ValidationSetImageRows.Any(row =>
                        string.Equals(row.Path, directValidationFolderImage, StringComparison.OrdinalIgnoreCase))
                    || shellHost.NativePreviewRunCount != localSetPreviewRunsBefore
                    || shellHost.LayerDocumentCount != localSetLayerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, localSetWorkspaceLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, localSetInputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, localSetOutputRouteBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke local validation path repair changed metadata or runtime state.");
                }

                Pump(30);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke local validation set",
                    "HostRecipeLocalValidationSetEditor",
                    "HostRecipeValidationSetCombo",
                    "HostRecipeNewValidationSetNameTextBox",
                    "HostRecipeCreateValidationSetButton",
                    "HostRecipeDeleteValidationSetButton",
                    "HostRecipeValidationSetNotesTextBox",
                    "HostRecipeAddValidationSetOkImagesButton",
                    "HostRecipeAddValidationSetNgImagesButton",
                    "HostRecipeAddValidationSetOkFolderButton",
                    "HostRecipeAddValidationSetNgFolderButton",
                    "HostRecipeRepairValidationSetImagePathButton",
                    "HostRecipeRemoveValidationSetImageButton",
                    "HostRecipeValidationSetImageList");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_RunHistory.png"));
                shellHost.RecipeCommands.SelectedValidationSuiteScopeOption = selectedSampleSuiteScope;

                TabItem xmlStepsTab = FindNamedVisualChild<TabItem>(
                    shellHost,
                    "tabRecipePipelineXmlSteps",
                    "Recipe manager direct smoke");
                xmlStepsTab.IsSelected = true;
                Pump(40);
                if (!shellHost.HasLayerForTest(selectedPreviewStep.InputLayer))
                {
                    using Bitmap inputLayerBitmap = CreateDockFloatSmokeBitmap();
                    if (!shellHost.AddLayerImageForTest(selectedPreviewStep.InputLayer, inputLayerBitmap))
                    {
                        throw new InvalidOperationException("Recipe manager direct smoke could not add the selected step input layer for navigation verification.");
                    }
                }

                if (!shellHost.HasLayerForTest(selectedPreviewStep.OutputLayer))
                {
                    using Bitmap outputLayerBitmap = CreateDockFloatSmokeBitmap();
                    if (!shellHost.AddLayerImageForTest(selectedPreviewStep.OutputLayer, outputLayerBitmap))
                    {
                        throw new InvalidOperationException("Recipe manager direct smoke could not add the selected step output layer for navigation verification.");
                    }

                    shellHost.RecipeCommands.RefreshOptions();
                    Pump(40);
                    selectedPreviewStep = shellHost.RecipeCommands.SelectedPipelinePreviewStep ?? selectedPreviewStep;
                }

                shellHost.ActivateHostLayerForTest(selectedPreviewStep.InputLayer);
                Pump(40);
                AssertSelectedListBoxItemVisible(
                    shellHost,
                    "HostRecipePipelineInlinePreviewStepList",
                    selectedPreviewStep,
                    "Recipe manager direct smoke pipeline failed-step selection");
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pipeline XML/steps tab",
                    "HostRecipePipelineXmlStepsTab",
                    "HostRecipePipelineInlineValidationReport",
                    "HostRecipePipelineValidationIssueList",
                    "HostRecipePipelineStepComparisonGrid",
                    "HostRecipePipelineInlinePreviewStepList",
                    "HostRecipePipelineSelectedStepDetailPanel",
                    "HostRecipePipelineSelectedStepOperatorContext",
                    "HostRecipePipelineSelectedStepRoute",
                    "HostRecipePipelineSelectedStepInputLayer",
                    "HostRecipePipelineSelectedStepInputLayerThumbnail",
                    "HostRecipePipelineSelectedStepOutputLayer",
                    "HostRecipePipelineSelectedStepOutputLayerThumbnail",
                    "HostRecipePipelineSelectedStepAcceptance",
                    "HostRecipePipelineSelectedStepRoiTemplate",
                    "HostRecipeOpenSelectedStepToolButton",
                    "HostRecipePipelineSelectedStepParameters",
                    "HostRecipeBranchOutputComparisonPanel",
                    "HostRecipeBranchOutputComparisonText",
                    "HostRecipeBranchOutputComparisonList",
                    "HostRecipeLoadSelectedStepParametersButton",
                    "HostRecipeApplySelectedStepParametersButton",
                    "HostRecipeSelectedStepEditStatus",
                    "HostRecipeCorrectedOutputReviewPanel",
                    "HostRecipeCorrectedOutputReviewText",
                    "HostRecipeCorrectedOutputViewButton",
                    "HostRecipeCorrectedOutputRerunButton");
                string selectedStepOperatorContext = shellHost.RecipeCommands.PipelineSelectedStepOperatorContextText;
                if (string.IsNullOrWhiteSpace(selectedStepOperatorContext)
                    || !selectedStepOperatorContext.Contains(selectedPreviewStep.Name, StringComparison.OrdinalIgnoreCase)
                    || (!selectedStepOperatorContext.Contains("Next", StringComparison.OrdinalIgnoreCase)
                        && !selectedStepOperatorContext.Contains("다음", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose selected-step operator context. "
                        + $"Text='{selectedStepOperatorContext}'");
                }

                if (!shellHost.RecipeCommands.OpenSelectedStepToolCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke selected step tool command was not enabled.");
                }

                if (shellHost.RecipeCommands.BranchOutputComparisonRows.Count == 0
                    || (!shellHost.RecipeCommands.BranchOutputComparisonText.Contains("output", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.BranchOutputComparisonText.Contains("출력", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose branch/output comparison rows. "
                        + $"Text='{shellHost.RecipeCommands.BranchOutputComparisonText}'");
                }

                if (!shellHost.RecipeCommands.NavigateSelectedStepOutputLayerCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke output layer navigation command was not enabled.");
                }

                shellHost.RecipeCommands.NavigateSelectedStepOutputLayerCommand.Execute(null);
                Pump(40);
                if (!string.Equals(shellHost.WorkspaceLayerTitle, selectedPreviewStep.OutputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke output layer navigation failed. "
                        + $"Expected={selectedPreviewStep.OutputLayer}, Actual={shellHost.WorkspaceLayerTitle}");
                }

                if (!shellHost.RecipeCommands.NavigateSelectedStepInputLayerCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke input layer navigation command was not enabled.");
                }

                shellHost.RecipeCommands.NavigateSelectedStepInputLayerCommand.Execute(null);
                Pump(40);
                if (!string.Equals(shellHost.WorkspaceLayerTitle, selectedPreviewStep.InputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke input layer navigation failed. "
                        + $"Expected={selectedPreviewStep.InputLayer}, Actual={shellHost.WorkspaceLayerTitle}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Pipeline.png"));

                if (!shellHost.RecipeCommands.LoadSelectedStepParametersCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke selected step parameter load command was not enabled.");
                }

                int beforeStepParameterApplyRuns = shellHost.NativePreviewRunCount;
                shellHost.RecipeCommands.LoadSelectedStepParametersCommand.Execute(null);
                Pump(160);
                if (shellHost.RecipeCommands.SelectedStepEditObject == null
                    || !shellHost.RecipeCommands.ApplySelectedStepParametersCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke did not load selected step parameters into the PropertyGrid edit object.");
                }

                shellHost.RecipeCommands.ApplySelectedStepParametersCommand.Execute(null);
                Pump(220);
                if ((!shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("Applied to XML", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("XML 반영 완료", StringComparison.OrdinalIgnoreCase))
                    || (!shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("corrected output", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("수정", StringComparison.OrdinalIgnoreCase))
                    || (!shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("Rerun", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.CorrectedOutputReviewText.Contains("재검사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not expose corrected-output review after XML apply. "
                        + $"Text='{shellHost.RecipeCommands.CorrectedOutputReviewText}'");
                }

                if (shellHost.NativePreviewRunCount != beforeStepParameterApplyRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke selected step XML apply triggered Preview/Run. "
                        + $"RunsBefore={beforeStepParameterApplyRuns}, RunsAfter={shellHost.NativePreviewRunCount}");
                }

                AssertHostedPropertyGridRowsRendered(shellHost, "Recipe manager direct smoke selected step PropertyGrid");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_StepPropertyGrid.png"));

                TabItem llmXmlTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipeLlmXml", "Recipe manager direct smoke");
                llmXmlTab.IsSelected = true;
                Pump(40);
                if (!shellHost.RecipeCommands.BuildLlmPromptCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM prompt command was disabled.");
                }

                shellHost.RecipeCommands.BuildLlmPromptCommand.Execute(null);
                Pump(20);
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmPromptText)
                    || !shellHost.RecipeCommands.LlmPromptText.Contains("OpenVisionLab VisionPipeline XML draft", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM prompt was not generated. "
                        + shellHost.RecipeCommands.LlmPromptText);
                }

                if (!shellHost.RecipeCommands.CopyLlmPromptCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM prompt copy command was disabled after generation.");
                }

                shellHost.RecipeCommands.CopyLlmPromptCommand.Execute(null);
                Pump(40);
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmPromptCopyStatusText)
                    || (!shellHost.RecipeCommands.LlmPromptCopyStatusText.Contains("copied", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.LlmPromptCopyStatusText.Contains("복사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM prompt copy command did not report success. "
                        + shellHost.RecipeCommands.LlmPromptCopyStatusText);
                }
                string llmPromptClipboard = GetClipboardTextWithRetry();
                if (!llmPromptClipboard.Contains("OpenVisionLab VisionPipeline XML draft", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("Template Matching", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("Intent contract", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("0..1 decimals", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("FIND_ANGLE_MIN", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("existing template/image dependency paths", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM prompt copy did not write the expected clipboard content.");
                }

                string previousLlmToolTemplate = shellHost.RecipeCommands.SelectedLlmToolTemplate;
                shellHost.RecipeCommands.SelectedLlmToolTemplate = "Pin gap / edge distance (LineDistance)";
                shellHost.RecipeCommands.CreateLlmTemplateXmlDraftForTest();
                Pump(20);
                string lineDistanceTemplateDraft = shellHost.RecipeCommands.LlmXmlDraftText ?? string.Empty;
                if (!lineDistanceTemplateDraft.Contains("<ToolType>LineDistance</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<AcceptanceMetricName>DistanceMmRange</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<ToolType>OverlayMerge</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<Value>42,150,80,80</Value>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<Value>151,150,80,80</Value>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>", StringComparison.OrdinalIgnoreCase)
                    || !lineDistanceTemplateDraft.Contains("<Key>LeftPRJ_DIR</Key>", StringComparison.OrdinalIgnoreCase)
                    || lineDistanceTemplateDraft.Contains("<ToolType>Contour</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || lineDistanceTemplateDraft.Contains("<ToolType>Line</ToolType>", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM pin-gap intent did not create a locked LineDistance XML starter. "
                        + lineDistanceTemplateDraft);
                }

                string lineDistancePrompt = shellHost.RecipeCommands.LlmPromptText ?? string.Empty;
                if (!lineDistancePrompt.Contains("self-contained GPT task packet", StringComparison.OrdinalIgnoreCase)
                    || !lineDistancePrompt.Contains("measure pin-to-pin distance", StringComparison.OrdinalIgnoreCase)
                    || !lineDistancePrompt.Contains("Response format: return XML only", StringComparison.OrdinalIgnoreCase)
                    || !lineDistancePrompt.Contains("Do not use Contour", StringComparison.OrdinalIgnoreCase)
                    || !lineDistancePrompt.Contains(OpenVisionRecipePinGapIntentSkill.DefaultRoiSamplesText, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM pin-gap prompt did not include the copy-ready GPT packet. "
                        + lineDistancePrompt);
                }

                if (!shellHost.RecipeCommands.CopyLlmPromptCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM pin-gap prompt copy command was disabled.");
                }

                shellHost.RecipeCommands.CopyLlmPromptCommand.Execute(null);
                Pump(40);
                string pinGapPromptClipboard = GetClipboardTextWithRetry();
                if (!pinGapPromptClipboard.Contains("self-contained GPT task packet", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPromptClipboard.Contains("Response format: return XML only", StringComparison.OrdinalIgnoreCase)
                    || !pinGapPromptClipboard.Contains("DistanceMmRange", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM pin-gap prompt copy did not write the expected GPT packet.");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmIntentLineDistance.png"));
                if (!shellHost.RecipeCommands.SuggestPinGapIntentRoiSamplesCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke pin-gap ROI suggestion command was disabled.");
                }

                int beforePinGapRoiSuggestRuns = shellHost.NativePreviewRunCount;
                shellHost.RecipeCommands.PinGapIntentRoiText = string.Empty;
                shellHost.RecipeCommands.SuggestPinGapIntentRoiSamplesCommand.Execute(null);
                Pump(20);
                string suggestedPinGapRoiText = shellHost.RecipeCommands.PinGapIntentRoiText ?? string.Empty;
                if (string.IsNullOrWhiteSpace(suggestedPinGapRoiText)
                    || !suggestedPinGapRoiText.Contains(";", StringComparison.Ordinal)
                    || !shellHost.RecipeCommands.StatusText.Contains("ROI", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap ROI suggestion did not populate multi-sample ROI text. "
                        + suggestedPinGapRoiText);
                }

                if (shellHost.NativePreviewRunCount != beforePinGapRoiSuggestRuns)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap ROI suggestion triggered Preview/Run. "
                        + $"Before={beforePinGapRoiSuggestRuns}, After={shellHost.NativePreviewRunCount}");
                }

                shellHost.RecipeCommands.PinGapIntentRoiText = OpenVisionRecipePinGapIntentSkill.DefaultRoiSamplesText;
                shellHost.RecipeCommands.PinGapIntentDistanceMinText = "0.40";
                shellHost.RecipeCommands.PinGapIntentDistanceMaxText = "0.55";
                shellHost.RecipeCommands.PinGapIntentRangeMaxText = "0.06";
                shellHost.RecipeCommands.PinGapIntentScaleText = "0.006";
                if (!shellHost.RecipeCommands.CreatePinGapIntentXmlDraftCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke pin-gap skill command was disabled.");
                }

                shellHost.RecipeCommands.CreatePinGapIntentXmlDraftCommand.Execute(null);
                Pump(40);
                string pinGapSkillDraft = shellHost.RecipeCommands.LlmXmlDraftText ?? string.Empty;
                if (!pinGapSkillDraft.Contains("<Name>LLM_PinGap_DistanceSkill</Name>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Name>01 Pin Array LeftA Avg</Name>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Name>08 Pin Array Right Range</Name>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Name>09 Pin Array Review Overlay</Name>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<AcceptanceMetricName>DistanceMmAvg</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<AcceptanceMetricName>DistanceMmRange</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<ToolType>OverlayMerge</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Value>42,150,80,80</Value>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Value>478,150,80,80</Value>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Key>ALLOW_BRANCH_INPUT</Key>", StringComparison.OrdinalIgnoreCase)
                    || !pinGapSkillDraft.Contains("<Key>SourceLayers</Key>", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap skill did not create the expected whole-array LineDistance XML draft. "
                        + pinGapSkillDraft);
                }

                string pinGapWorkflowText = shellHost.RecipeCommands.PinGapIntentWorkflowText ?? string.Empty;
                if (!pinGapWorkflowText.Contains("DistanceMmAvg", StringComparison.OrdinalIgnoreCase)
                    || !pinGapWorkflowText.Contains("DistanceMmRange", StringComparison.OrdinalIgnoreCase)
                    || (!pinGapWorkflowText.Contains("whole pin-array", StringComparison.OrdinalIgnoreCase)
                        && !pinGapWorkflowText.Contains("전체 핀 배열", StringComparison.OrdinalIgnoreCase))
                    || (!pinGapWorkflowText.Contains("Validate", StringComparison.OrdinalIgnoreCase)
                        && !pinGapWorkflowText.Contains("검증", StringComparison.OrdinalIgnoreCase))
                    || (!pinGapWorkflowText.Contains("Import", StringComparison.OrdinalIgnoreCase)
                        && !pinGapWorkflowText.Contains("가져오기", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap workflow summary did not expose the expected gates and next actions. "
                        + pinGapWorkflowText);
                }

                string pinGapFeedbackText = shellHost.RecipeCommands.PinGapIntentFeedbackText ?? string.Empty;
                if (!pinGapFeedbackText.Contains("Avg NG", StringComparison.OrdinalIgnoreCase)
                    || !pinGapFeedbackText.Contains("Range NG", StringComparison.OrdinalIgnoreCase)
                    || (!pinGapFeedbackText.Contains("whole-array", StringComparison.OrdinalIgnoreCase)
                        && !pinGapFeedbackText.Contains("전체 핀 배열", StringComparison.OrdinalIgnoreCase))
                    || !pinGapFeedbackText.Contains("ROI", StringComparison.OrdinalIgnoreCase)
                    || !pinGapFeedbackText.Contains("mm/px", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap feedback did not expose the expected tuning axes. "
                        + pinGapFeedbackText);
                }

                string pinGapLatestRunText = shellHost.RecipeCommands.PinGapIntentLatestRunText ?? string.Empty;
                if (!pinGapLatestRunText.Contains("DistanceMmAvg", StringComparison.OrdinalIgnoreCase)
                    || !pinGapLatestRunText.Contains("DistanceMmRange", StringComparison.OrdinalIgnoreCase)
                    || (!pinGapLatestRunText.Contains("Decision", StringComparison.OrdinalIgnoreCase)
                        && !pinGapLatestRunText.Contains("판정", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke pin-gap latest run summary did not expose actual distance metrics and decision text. "
                        + pinGapLatestRunText);
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke pin-gap workflow summary",
                    "HostRecipePinGapIntentWorkflowText",
                    "HostRecipeSuggestPinGapIntentRoiButton",
                    "HostRecipePinGapIntentFeedbackText",
                    "HostRecipePinGapIntentLatestRunText");
                File.WriteAllText(Path.Combine(outputDirectory, "LlmPinGapSkill.xml"), pinGapSkillDraft, Encoding.Unicode);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmPinGapSkill.png"));

                shellHost.RecipeCommands.BlobCountIntentRoiText = "0,0,572,420";
                shellHost.RecipeCommands.BlobCountIntentThresholdText = "150";
                shellHost.RecipeCommands.BlobCountIntentMinCountText = "8";
                shellHost.RecipeCommands.BlobCountIntentMaxCountText = "14";
                shellHost.RecipeCommands.BlobCountIntentMinAreaText = "200";
                shellHost.RecipeCommands.BlobCountIntentMaxAreaText = "2000";
                if (!shellHost.RecipeCommands.CreateBlobCountIntentXmlDraftCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke blob-count skill command was disabled.");
                }

                shellHost.RecipeCommands.CreateBlobCountIntentXmlDraftCommand.Execute(null);
                Pump(40);
                string blobCountSkillDraft = shellHost.RecipeCommands.LlmXmlDraftText ?? string.Empty;
                if (!blobCountSkillDraft.Contains("<Name>LLM_BlobCount_Skill</Name>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Name>01 Blob Count Binary</Name>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<ToolType>Threshold</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Name>02 Blob Count Inspect</Name>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<ToolType>Blob</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<AcceptanceMetricName>ResultCount</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Value>0,0,572,420</Value>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Key>MIN_AREA</Key>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Value>200</Value>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Key>MAX_AREA</Key>", StringComparison.OrdinalIgnoreCase)
                    || !blobCountSkillDraft.Contains("<Value>2000</Value>", StringComparison.OrdinalIgnoreCase)
                    || blobCountSkillDraft.Contains("<ToolType>LineDistance</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || blobCountSkillDraft.Contains("<ToolType>Contour</ToolType>", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke blob-count skill did not create the expected Threshold + Blob ResultCount XML draft. "
                        + blobCountSkillDraft);
                }

                string blobCountWorkflowText = shellHost.RecipeCommands.BlobCountIntentWorkflowText ?? string.Empty;
                if (!blobCountWorkflowText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase)
                    || (!blobCountWorkflowText.Contains("Validate", StringComparison.OrdinalIgnoreCase)
                        && !blobCountWorkflowText.Contains("검증", StringComparison.OrdinalIgnoreCase))
                    || (!blobCountWorkflowText.Contains("Import", StringComparison.OrdinalIgnoreCase)
                        && !blobCountWorkflowText.Contains("가져오기", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke blob-count workflow summary did not expose the expected gate and next actions. "
                        + blobCountWorkflowText);
                }

                string blobCountFeedbackText = shellHost.RecipeCommands.BlobCountIntentFeedbackText ?? string.Empty;
                if (!blobCountFeedbackText.Contains("Count NG", StringComparison.OrdinalIgnoreCase)
                    || !blobCountFeedbackText.Contains("threshold", StringComparison.OrdinalIgnoreCase)
                    || !blobCountFeedbackText.Contains("ROI", StringComparison.OrdinalIgnoreCase)
                    || !blobCountFeedbackText.Contains("area", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke blob-count feedback did not expose the expected tuning axes. "
                        + blobCountFeedbackText);
                }

                string blobCountLatestRunText = shellHost.RecipeCommands.BlobCountIntentLatestRunText ?? string.Empty;
                if (!blobCountLatestRunText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke blob-count latest run summary did not expose ResultCount guidance. "
                        + blobCountLatestRunText);
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke blob-count workflow summary",
                    "HostRecipeLlmBlobCountIntentSkill",
                    "HostRecipeBlobCountIntentWorkflowText",
                    "HostRecipeBlobCountIntentFeedbackText",
                    "HostRecipeBlobCountIntentLatestRunText");
                File.WriteAllText(Path.Combine(outputDirectory, "LlmBlobCountSkill.xml"), blobCountSkillDraft, Encoding.Unicode);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmBlobCountSkill.png"));

                shellHost.RecipeCommands.ContourCountIntentRoiText = "0,0,572,420";
                shellHost.RecipeCommands.ContourCountIntentThresholdText = "150";
                shellHost.RecipeCommands.ContourCountIntentMinCountText = "5";
                shellHost.RecipeCommands.ContourCountIntentMaxCountText = "5";
                shellHost.RecipeCommands.ContourCountIntentMinAreaText = "700";
                shellHost.RecipeCommands.ContourCountIntentMaxAreaText = "9000";
                if (!shellHost.RecipeCommands.CreateContourCountIntentXmlDraftCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke contour-count skill command was disabled.");
                }

                shellHost.RecipeCommands.CreateContourCountIntentXmlDraftCommand.Execute(null);
                Pump(40);
                string contourCountSkillDraft = shellHost.RecipeCommands.LlmXmlDraftText ?? string.Empty;
                if (!contourCountSkillDraft.Contains("<Name>LLM_ContourCountSize_Skill</Name>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Name>01 Contour Binary</Name>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<ToolType>Threshold</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Name>02 Contour Count</Name>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<ToolType>Contour</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Name>03 Contour Size Guard</Name>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Name>04 Contour Review Overlay</Name>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<ToolType>OverlayMerge</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<AcceptanceMetricName>ResultCount</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<AcceptanceMetricName>AreaMax</AcceptanceMetricName>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Value>0,0,572,420</Value>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Key>ALLOW_BRANCH_INPUT</Key>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Key>SourceLayers</Key>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Value>ContourSize_Result</Value>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Key>MIN_AREA</Key>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Value>700</Value>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Key>MAX_AREA</Key>", StringComparison.OrdinalIgnoreCase)
                    || !contourCountSkillDraft.Contains("<Value>9000</Value>", StringComparison.OrdinalIgnoreCase)
                    || contourCountSkillDraft.Contains("<ToolType>LineDistance</ToolType>", StringComparison.OrdinalIgnoreCase)
                    || contourCountSkillDraft.Contains("<ToolType>Blob</ToolType>", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke contour-count skill did not create the expected Threshold + Contour ResultCount/AreaMax XML draft. "
                        + contourCountSkillDraft);
                }

                string contourCountWorkflowText = shellHost.RecipeCommands.ContourCountIntentWorkflowText ?? string.Empty;
                if (!contourCountWorkflowText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase)
                    || !contourCountWorkflowText.Contains("AreaMax", StringComparison.OrdinalIgnoreCase)
                    || !contourCountWorkflowText.Contains("Review overlay", StringComparison.OrdinalIgnoreCase)
                    || (!contourCountWorkflowText.Contains("Validate", StringComparison.OrdinalIgnoreCase)
                        && !contourCountWorkflowText.Contains("검증", StringComparison.OrdinalIgnoreCase))
                    || (!contourCountWorkflowText.Contains("Import", StringComparison.OrdinalIgnoreCase)
                        && !contourCountWorkflowText.Contains("가져오기", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke contour-count workflow summary did not expose the expected gates and next actions. "
                        + contourCountWorkflowText);
                }

                string contourCountFeedbackText = shellHost.RecipeCommands.ContourCountIntentFeedbackText ?? string.Empty;
                if (!contourCountFeedbackText.Contains("Count NG", StringComparison.OrdinalIgnoreCase)
                    || !contourCountFeedbackText.Contains("AreaMax NG", StringComparison.OrdinalIgnoreCase)
                    || !contourCountFeedbackText.Contains("threshold", StringComparison.OrdinalIgnoreCase)
                    || !contourCountFeedbackText.Contains("ROI", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke contour-count feedback did not expose the expected tuning axes. "
                        + contourCountFeedbackText);
                }

                string contourCountLatestRunText = shellHost.RecipeCommands.ContourCountIntentLatestRunText ?? string.Empty;
                if (!contourCountLatestRunText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase)
                    || !contourCountLatestRunText.Contains("AreaMax", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke contour-count latest run summary did not expose ResultCount/AreaMax guidance. "
                        + contourCountLatestRunText);
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke contour-count workflow summary",
                    "HostRecipeLlmContourCountIntentSkill",
                    "HostRecipeContourCountIntentWorkflowText",
                    "HostRecipeContourCountIntentFeedbackText",
                    "HostRecipeContourCountIntentLatestRunText");
                File.WriteAllText(Path.Combine(outputDirectory, "LlmContourCountSkill.xml"), contourCountSkillDraft, Encoding.Unicode);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmContourCountSkill.png"));
                shellHost.RecipeCommands.SelectedLlmToolTemplate = previousLlmToolTemplate;

                string llmDependencyPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_direct_llm_dependency_" + Guid.NewGuid().ToString("N") + ".png");
                using (Bitmap dependencyImage = CreateDockFloatSmokeBitmap())
                {
                    dependencyImage.Save(llmDependencyPath);
                }

                string llmDraftPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_direct_llm_draft_" + Guid.NewGuid().ToString("N") + ".xml");
                VisionPipeline llmDraftPipeline = CreateDirectSmokePipeline("Direct_LLM_Draft", 2);
                llmDraftPipeline.Steps[0].Parameters["TemplatePath"] = llmDependencyPath;
                if (!VisionPipelineStorage.TrySaveToFile(llmDraftPath, llmDraftPipeline, out string llmDraftSaveMessage))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not save LLM draft XML: " + llmDraftSaveMessage);
                }

                SetClipboardTextWithRetry(File.ReadAllText(llmDraftPath));
                shellHost.RecipeCommands.PasteLlmXmlDraftFromClipboardCommand.Execute(null);
                Pump(40);
                if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("Direct_LLM_Draft", StringComparison.OrdinalIgnoreCase)
                    || (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmXmlDraftPasteStatusText)
                        || (!shellHost.RecipeCommands.LlmXmlDraftPasteStatusText.Contains("Pasted", StringComparison.OrdinalIgnoreCase)
                            && !shellHost.RecipeCommands.LlmXmlDraftPasteStatusText.Contains("붙여넣", StringComparison.OrdinalIgnoreCase))))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM XML draft was not pasted from the clipboard. "
                        + shellHost.RecipeCommands.LlmXmlDraftPasteStatusText);
                }

                if (!shellHost.RecipeCommands.LoadLlmXmlDraftFromPath(llmDraftPath))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke could not load LLM draft XML. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', Dependencies='{shellHost.RecipeCommands.LlmXmlDraftDependencyReport}', Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
                }

                Pump(40);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke LLM dependency path drilldown",
                    "HostRecipeLlmDependencyPathList");
                if (shellHost.RecipeCommands.LlmXmlDraftDependencyRows.Count == 0)
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM dependency drilldown did not expose any path rows.");
                }

                if (!shellHost.RecipeCommands.LlmXmlDraftDiffReport.Contains("LLM XML diff review: READY", StringComparison.OrdinalIgnoreCase)
                    && !shellHost.RecipeCommands.LlmXmlDraftDiffReport.Contains("LLM XML 변경점: 준비됨", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM XML diff report was not ready. "
                        + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
                }

                if (!shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Inspection.Evidence", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM XML validation report did not expose result channel requirements. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }

                if (!shellHost.RecipeCommands.CopyLlmReviewBundleCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM review bundle copy command was disabled after draft load.");
                }

                shellHost.RecipeCommands.CopyLlmReviewBundleCommand.Execute(null);
                Pump(40);
                if (string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmReviewBundleCopyStatusText)
                    || (!shellHost.RecipeCommands.LlmReviewBundleCopyStatusText.Contains("copied", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.LlmReviewBundleCopyStatusText.Contains("복사", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke LLM review bundle copy command did not report success. "
                        + shellHost.RecipeCommands.LlmReviewBundleCopyStatusText);
                }
                string llmReviewBundleClipboard = GetClipboardTextWithRetry();
                if (!llmReviewBundleClipboard.Contains("OpenVisionLab LLM XML review bundle", StringComparison.OrdinalIgnoreCase)
                    || !llmReviewBundleClipboard.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !llmReviewBundleClipboard.Contains("Selected step operator context", StringComparison.OrdinalIgnoreCase)
                    || !llmReviewBundleClipboard.Contains("Failure review", StringComparison.OrdinalIgnoreCase)
                    || !llmReviewBundleClipboard.Contains("Direct_LLM_Draft", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM review bundle copy did not write the expected clipboard content.");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke LLM XML tab",
                    "HostRecipeLlmXmlTab",
                    "HostRecipeLlmAssistantPanel",
                    "HostRecipeLlmTemplateSelector",
                    "HostRecipeLlmGoalText",
                    "HostRecipeLlmDetectionPointsText",
                    "HostRecipeLlmResultChannelContract",
                    "HostRecipeBuildLlmPromptButton",
                    "HostRecipeCopyLlmPromptButton",
                    "HostRecipeCreateLlmTemplateXmlButton",
                    "HostRecipeCopyLlmReviewBundleButton",
                    "HostRecipePasteLlmXmlDraftButton",
                    "HostRecipeRefreshLlmDraftReviewButton",
                    "HostRecipeLlmXmlDraftPanel",
                    "HostRecipeLlmXmlDraftText",
                    "HostRecipeLoadLlmXmlDraftButton",
                    "HostRecipeValidateLlmXmlDraftButton",
                    "HostRecipeImportLlmXmlDraftButton",
                    "HostRecipeLlmDependencyReport",
                    "HostRecipeLlmDraftReviewReport",
                    "HostRecipeLlmDiffReport",
                    "HostRecipeLlmValidationReport",
                    "HostRecipeLlmValidationIssueList");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_LlmXml.png"));

                string originalLlmDraft = shellHost.RecipeCommands.LlmXmlDraftText;
                shellHost.RecipeCommands.LlmXmlDraftText = "<VisionPipeline>";
                if (shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || (!shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Next: Fix malformed XML", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("다음: 보고된 줄/위치", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM XML failure report did not include the expected next action. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }

                VisionPipeline badRouteDraftPipeline = new VisionPipeline { Name = "Direct_LLM_BadRoute" };
                badRouteDraftPipeline.Steps.Add(new VisionPipelineStep
                {
                    Name = "Missing_Input_Step",
                    ToolType = "Threshold",
                    InputLayer = "Missing_Input_Layer",
                    OutputLayer = "BadRoute_Output"
                });
                shellHost.RecipeCommands.LlmXmlDraftText = SerializePipelineToXmlText(badRouteDraftPipeline);
                if (shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || (!shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("input layer", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("InputLayer", StringComparison.OrdinalIgnoreCase))
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("Missing_Input_Layer", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("does not exist", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmXmlDraftReviewReport)
                    || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.LlmXmlDraftDiffReport))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM XML bad-route failure report did not block import review with actionable route guidance. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                        + $"Review='{shellHost.RecipeCommands.LlmXmlDraftReviewReport}', "
                        + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
                }

                string selectedPipelineBeforeInvalidImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
                VisionPipeline unsupportedToolDraftPipeline = new VisionPipeline { Name = "Direct_LLM_UnsupportedTool" };
                unsupportedToolDraftPipeline.Steps.Add(new VisionPipelineStep
                {
                    Name = "Unsupported_Tool_Step",
                    ToolType = "ImaginaryLlmTool",
                    InputLayer = "Main",
                    OutputLayer = "UnsupportedTool_Output"
                });
                shellHost.RecipeCommands.LlmXmlDraftText = SerializePipelineToXmlText(unsupportedToolDraftPipeline);
                if (shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("unsupported ToolType", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("ImaginaryLlmTool", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM XML unsupported-tool failure report did not block the draft. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }

                if (shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
                {
                    shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
                    Pump(40);
                }

                string selectedPipelineAfterInvalidImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
                if (!string.Equals(selectedPipelineBeforeInvalidImport, selectedPipelineAfterInvalidImport, StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("unsupported ToolType", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM XML unsupported-tool import attempt changed pipeline state or lost validation context. "
                        + $"Before='{selectedPipelineBeforeInvalidImport}', After='{selectedPipelineAfterInvalidImport}', "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }

                VisionPipeline missingDependencyDraftPipeline = new VisionPipeline { Name = "Direct_LLM_MissingDependency" };
                VisionPipelineStep missingDependencyStep = new VisionPipelineStep
                {
                    Name = "Missing_Template_Step",
                    ToolType = "Matching",
                    InputLayer = "Main",
                    OutputLayer = "MissingTemplate_Output"
                };
                missingDependencyStep.Parameters["TemplatePath"] = Path.Combine(
                    Path.GetTempPath(),
                    "OpenVisionLab_missing_llm_template_" + Guid.NewGuid().ToString("N") + ".png");
                missingDependencyStep.Parameters["SCORE_MIN"] = "0.6";
                missingDependencyStep.Parameters["NUM_MATCH"] = "1";
                missingDependencyDraftPipeline.Steps.Add(missingDependencyStep);
                AssertLlmDraftBlocked(
                    shellHost,
                    missingDependencyDraftPipeline,
                    "missing dependency",
                    "dependency file path|의존 파일 경로",
                    "missing|누락");

                VisionPipeline badParameterDraftPipeline = new VisionPipeline { Name = "Direct_LLM_BadParameters" };
                VisionPipelineStep badParameterStep = new VisionPipelineStep
                {
                    Name = "Bad_Parameter_Step",
                    ToolType = "Threshold",
                    InputLayer = "Main",
                    OutputLayer = "BadParameter_Output"
                };
                badParameterStep.Parameters["Threshold"] = "bright";
                badParameterStep.Parameters["USE_ROI"] = "sometimes";
                badParameterDraftPipeline.Steps.Add(badParameterStep);
                AssertLlmDraftBlocked(
                    shellHost,
                    badParameterDraftPipeline,
                    "bad parameter",
                    "Threshold",
                    "expects a numeric value",
                    "USE_ROI",
                    "expects True or False");

                VisionPipeline badScoreDraftPipeline = new VisionPipeline { Name = "Direct_LLM_BadScoreRange" };
                VisionPipelineStep badScoreStep = new VisionPipelineStep
                {
                    Name = "Bad_Score_Range_Step",
                    ToolType = "Matching",
                    InputLayer = "Main",
                    OutputLayer = "BadScore_Output"
                };
                badScoreStep.Parameters["SCORE_MIN"] = "80";
                badScoreStep.Parameters["NUM_MATCH"] = "1";
                badScoreDraftPipeline.Steps.Add(badScoreStep);
                AssertLlmDraftBlocked(
                    shellHost,
                    badScoreDraftPipeline,
                    "bad score range",
                    "SCORE_MIN",
                    "0..1",
                    "percentage");

                VisionPipeline customInspectionDraftPipeline = new VisionPipeline { Name = "Direct_LLM_CustomInspectionNode" };
                VisionPipelineStep customInspectionStep = new VisionPipelineStep
                {
                    Name = "Custom_Inspection_Node_Step",
                    ToolType = "Threshold",
                    InputLayer = "Main",
                    OutputLayer = "CustomInspection_Output"
                };
                customInspectionStep.Parameters["Inspection.Status"] = "OK";
                customInspectionDraftPipeline.Steps.Add(customInspectionStep);
                AssertLlmDraftBlocked(
                    shellHost,
                    customInspectionDraftPipeline,
                    "custom Inspection.* XML",
                    "Inspection.*",
                    "not XML nodes|리뷰 채널",
                    "Remove custom XML nodes|제거하세요");

                shellHost.RecipeCommands.LlmXmlDraftText = SerializePipelineToXmlText(badParameterDraftPipeline);
                shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest();
                if (!shellHost.RecipeCommands.CopyLlmReviewBundleCommand.CanExecute(null))
                {
                    throw new InvalidOperationException("Recipe manager LLM correction bundle command was disabled after a bad-parameter draft.");
                }

                shellHost.RecipeCommands.CopyLlmReviewBundleCommand.Execute(null);
                Pump(40);
                string correctionBundleClipboard = GetClipboardTextWithRetry();
                if (!correctionBundleClipboard.Contains("Correction rules", StringComparison.OrdinalIgnoreCase)
                    || !correctionBundleClipboard.Contains("Bad_Parameter_Step", StringComparison.OrdinalIgnoreCase)
                    || !correctionBundleClipboard.Contains("expects a numeric value", StringComparison.OrdinalIgnoreCase)
                    || !correctionBundleClipboard.Contains("do not invent layers", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM correction bundle did not include enough context for a corrected XML retry. "
                        + correctionBundleClipboard);
                }

                OpenVisionRecipePipelineOption pipelineBeforeCorrectionImport = shellHost.RecipeCommands.SelectedPipelineOption;
                VisionPipeline correctedDraftPipeline = new VisionPipeline { Name = "Direct_LLM_CorrectedThreshold" };
                VisionPipelineStep correctedStep = new VisionPipelineStep
                {
                    Name = "Corrected_Threshold_Step",
                    ToolType = "Threshold",
                    InputLayer = "Main",
                    OutputLayer = "CorrectedThreshold_Output"
                };
                correctedStep.Parameters["Threshold"] = "128";
                correctedStep.Parameters["USE_ROI"] = "False";
                correctedDraftPipeline.Steps.Add(correctedStep);
                shellHost.RecipeCommands.LlmXmlDraftText = SerializePipelineToXmlText(correctedDraftPipeline);
                if (!shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                    || !shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains("OK", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM corrected draft did not validate as importable. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }

                shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
                Pump(80);
                string selectedPipelineAfterCorrectionImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
                if (!selectedPipelineAfterCorrectionImport.Contains("Direct_LLM_CorrectedThreshold", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Any(step =>
                        string.Equals(step.Name, "Corrected_Threshold_Step", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM corrected draft import did not select the imported corrected pipeline. "
                        + $"Selected='{selectedPipelineAfterCorrectionImport}'");
                }

                if (pipelineBeforeCorrectionImport != null)
                {
                    OpenVisionRecipePipelineOption restoreOption = shellHost.RecipeCommands.PipelineOptions
                        .FirstOrDefault(option => string.Equals(
                            option.PipelineName,
                            pipelineBeforeCorrectionImport.PipelineName,
                            StringComparison.OrdinalIgnoreCase));
                    if (restoreOption != null)
                    {
                        shellHost.RecipeCommands.SelectedPipelineOption = restoreOption;
                        Pump(40);
                    }
                }

                VisionPipeline missingInputBDraftPipeline = new VisionPipeline { Name = "Direct_LLM_MissingInputB" };
                VisionPipelineStep missingInputBStep = new VisionPipelineStep
                {
                    Name = "Missing_Input_B_Step",
                    ToolType = "Arithmetic",
                    InputLayer = "Main",
                    OutputLayer = "MissingInputB_Output"
                };
                missingInputBStep.Parameters[VisionPipelineArithmeticStep.ParameterMode] = VisionPipelineArithmeticStep.ModeOperation;
                missingInputBStep.Parameters[VisionPipelineArithmeticStep.ParameterOperation] = "ADD";
                missingInputBStep.Parameters[VisionPipelineArithmeticStep.ParameterInputLayerB] = "Missing_Input_B";
                missingInputBDraftPipeline.Steps.Add(missingInputBStep);
                AssertLlmDraftBlocked(
                    shellHost,
                    missingInputBDraftPipeline,
                    "missing InputLayerB",
                    "input layer B",
                    "Missing_Input_B",
                    "does not exist");

                int actualMultiBranchRows = VerifyActualMultiBranchPipeline(shellHost);
                int actualThreeWayBranchRows = VerifyActualThreeWayBranchPipeline(shellHost);

                shellHost.RecipeCommands.LlmXmlDraftText = originalLlmDraft;
                shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest();

                TabItem previewTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePreview", "Recipe manager direct smoke");
                previewTab.IsSelected = true;
                Pump(40);
                AssertVisibleAutomationIds(
                    shellHost,
                    "Recipe manager direct smoke preview tab",
                    "HostRecipePreviewTab",
                    "HostRecipePipelinePreviewStepList");
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Preview.png"));

                System.Windows.Point offsetBefore = shellHost.RecipeManagerPanelOffsetForTest;
                if (!shellHost.MoveRecipeManagerPanelForTest(-180D, 18D))
                {
                    throw new InvalidOperationException("Recipe manager panel did not move in the direct EXE smoke.");
                }

                Pump(30);
                System.Windows.Point offsetAfter = shellHost.RecipeManagerPanelOffsetForTest;
                if (offsetAfter.X >= offsetBefore.X - 10D || offsetAfter.Y <= offsetBefore.Y)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke movement was too small. "
                        + $"Before={offsetBefore.X:0.0},{offsetBefore.Y:0.0}; After={offsetAfter.X:0.0},{offsetAfter.Y:0.0}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_Moved.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: recipe-manager-tabs" + Environment.NewLine
                    + "Recipe: " + recipeName + Environment.NewLine
                    + "Pipeline: " + (shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? pipelineName) + Environment.NewLine
                    + "PreviewSteps: " + shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "RecipeManagerSummaryMode: library + summary + lifecycle commands" + Environment.NewLine
                    + "RecipeManagerAdvancedMode: technical review full width; library and lifecycle commands hidden" + Environment.NewLine
                    + "SampleCheck: " + shellHost.RecipeCommands.LatestSampleRunSummary.StatusText + Environment.NewLine
                    + "PairCheck: " + pairCheckStatusText + Environment.NewLine
                    + "ValidationSuite: selected sample saved to run history" + Environment.NewLine
                    + "ValidationSuiteStepReport: linked " + selectedSampleRunReport.Steps.Count.ToString(CultureInfo.InvariantCulture)
                    + " Step(s) | " + validationSuiteStepReportEvidencePath + Environment.NewLine
                    + "LocalValidationSet: direct EXE file/folder/repair controls visible; top-level folder registration and explicit missing-path repair preserved metadata, Preview/Run, layers, and routing" + Environment.NewLine
                    + "PipelineInventory: valid VisionPipeline XML only" + Environment.NewLine
                    + "PairRoleCards: " + pairRoleCardCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "RoleDrilldown: " + forcedFailedRole.Role + " -> " + failedStepPreview.Name + Environment.NewLine
                    + "FailedRunLink: " + selectedPreviewStep.Index.ToString(CultureInfo.InvariantCulture) + " | " + selectedPreviewStep.Name + Environment.NewLine
                    + "FailedRunStepVisible: True" + Environment.NewLine
                    + "BenchmarkBaselineSelection: selectable baseline changed diff to Still NG and restored Regression" + Environment.NewLine
                    + "RunHistoryAnalytics: " + shellHost.RecipeCommands.SelectedRecentBatchRunOption.AnalyticsText + Environment.NewLine
                    + "RunHistoryPerformanceComparison: " + shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText.Replace(Environment.NewLine, " | ") + Environment.NewLine
                    + "RunHistoryNgFilter: " + shellHost.RecipeCommands.RecentBatchRunNgFilterSummaryText + Environment.NewLine
                    + "SampleToInputLoad: explicit load without Preview/Run | " + selectedPreviewStep.InputLayer + " | " + sampleImagePath + Environment.NewLine
                    + "FailedStepRerunComparison: visible" + Environment.NewLine
                    + "CorrectedOutputReview: visible after XML apply" + Environment.NewLine
                    + "SelectedRunReview: linked failed step" + Environment.NewLine
                    + "SelectedRunReviewCopy: copied" + Environment.NewLine
                    + "PipelineFilter: " + pipelineFilterText + " -> " + shellHost.RecipeCommands.FilteredPipelineOptions.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PipelineVariantComparison: active/selected diff visible without Preview/Run" + Environment.NewLine
                    + "StepParameters: " + selectedPreviewStep.ParameterPreviewText + Environment.NewLine
                    + "StepRoiTemplate: " + selectedPreviewStep.RoiMetadataText + " | " + selectedPreviewStep.TemplateMetadataText + Environment.NewLine
                    + "StepToolEntry: " + shellHost.RecipeCommands.OpenSelectedStepToolText + Environment.NewLine
                    + "StepPropertyGridApply: explicit XML apply without Preview/Run" + Environment.NewLine
                    + "GuidedSetupStandalone: Pin gap + Blob + Contour + Matching + Feature Matching + Edge Based Matching + Mean + ReferenceDifference Starter XML without Preview/Run" + Environment.NewLine
                    + "GuidedSetupPinGapUnits: MM-READY conversion review + PX-ONLY DistancePx gates + invalid scale blocked" + Environment.NewLine
                    + "GuidedSetupPinGapPublicSample: " + pinGapUnitSampleEvidence + Environment.NewLine
                    + "OperatorDecisionSummaryBand: final Good/Bad status + expected/actual metric evidence + next action" + Environment.NewLine
                    + "LlmIntentTemplate: LineDistance locked" + Environment.NewLine
                    + "LlmPinGapPromptPacket: copy-ready GPT XML-only packet copied" + Environment.NewLine
                    + "LlmPinGapIntentSkill: generated whole-array DistanceMmAvg + DistanceMmRange gates plus review OverlayMerge" + Environment.NewLine
                    + "LlmPinGapRoiSuggest: selected sample image suggested multi-sample ROI without Preview/Run" + Environment.NewLine
                    + "LlmPinGapWorkflow: visible whole-array Validate/Import/sample-run next actions" + Environment.NewLine
                    + "LlmPinGapFeedback: visible whole-array Avg NG vs Range NG tuning axes" + Environment.NewLine
                    + "LlmPinGapLatestRun: visible actual DistanceMmAvg/DistanceMmRange decision" + Environment.NewLine
                    + "LlmBlobCountIntentSkill: generated Threshold + Blob ResultCount gate" + Environment.NewLine
                    + "LlmBlobCountWorkflow: visible Validate/Import/sample-run next actions" + Environment.NewLine
                    + "LlmBlobCountFeedback: visible count/threshold/ROI/area tuning axes" + Environment.NewLine
                    + "LlmContourCountIntentSkill: generated Threshold + Contour ResultCount/AreaMax gates plus review OverlayMerge" + Environment.NewLine
                    + "LlmContourCountWorkflow: visible Validate/Import/sample-run next actions" + Environment.NewLine
                    + "LlmContourCountFeedback: visible count/area/threshold/ROI tuning axes" + Environment.NewLine
                    + "LlmValidationIssues: visible" + Environment.NewLine
                    + "LlmBadRouteValidation: blocked" + Environment.NewLine
                    + "LlmUnsupportedToolImport: blocked" + Environment.NewLine
                    + "LlmMissingDependencyImport: blocked" + Environment.NewLine
                    + "LlmBadParameterImport: blocked" + Environment.NewLine
                    + "LlmBadScoreRangeImport: blocked" + Environment.NewLine
                    + "LlmCustomInspectionImport: blocked" + Environment.NewLine
                    + "LlmCorrectionBundle: copied" + Environment.NewLine
                    + "LlmCorrectedDraftImport: imported" + Environment.NewLine
                    + "LlmMissingInputBImport: blocked" + Environment.NewLine
                    + "LlmDependencyRows: " + shellHost.RecipeCommands.LlmXmlDraftDependencyRows.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LlmXmlDiff: visible" + Environment.NewLine
                    + "StepComparisonGrid: visible" + Environment.NewLine
                    + "BranchOutputComparison: " + shellHost.RecipeCommands.BranchOutputComparisonRows.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ActualMultiBranchComparison: " + actualMultiBranchRows.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ActualThreeWayBranchComparison: " + actualThreeWayBranchRows.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "SelectedStepDetail: visible" + Environment.NewLine
                    + "StepLayerCards: visible" + Environment.NewLine
                    + "StepLayerNavigation: " + selectedPreviewStep.OutputLayer + " -> " + selectedPreviewStep.InputLayer + Environment.NewLine
                    + "MovedFrom: " + offsetBefore.X.ToString("0.0", CultureInfo.InvariantCulture) + "," + offsetBefore.Y.ToString("0.0", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "MovedTo: " + offsetAfter.X.ToString("0.0", CultureInfo.InvariantCulture) + "," + offsetAfter.Y.ToString("0.0", CultureInfo.InvariantCulture),
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static int VerifyActualMultiBranchPipeline(OpenVisionShellHostView shellHost)
        {
            string path = Path.Combine(FindRepositoryRoot(), "docs", "samples", "BentPin_TopBottom_Overlay.pipeline.xml");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Actual multi-branch sample pipeline was not found.", path);
            }

            OpenVisionRecipePipelineOption originalOption = shellHost.RecipeCommands.SelectedPipelineOption;
            try
            {
                if (!shellHost.RecipeCommands.ImportPipelineXmlFromPath(path))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not import the actual multi-branch sample pipeline.");
                }

                Pump(60);
                if (!string.Equals(
                        shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName,
                        "BentPin_TopBottom_Overlay",
                        StringComparison.OrdinalIgnoreCase)
                    || shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count < 5)
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke did not select the imported multi-branch pipeline. "
                        + $"Selected='{shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName}', "
                        + $"Steps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
                }

                OpenVisionRecipePipelineStepPreview cleanStep =
                    shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps
                        .FirstOrDefault(step => step.Name.Contains("Bent Pin Close", StringComparison.OrdinalIgnoreCase));
                if (cleanStep == null)
                {
                    throw new InvalidOperationException("Actual multi-branch sample did not expose the Bent Pin Close step.");
                }

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = cleanStep;
                Pump(40);
                IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> cleanRows =
                    shellHost.RecipeCommands.BranchOutputComparisonRows;
                bool hasTopConsumer = cleanRows.Any(row =>
                    row.Route.Contains("BentPin_Clean -> BentPin_TopContour", StringComparison.OrdinalIgnoreCase));
                bool hasBottomConsumer = cleanRows.Any(row =>
                    row.Route.Contains("BentPin_Clean -> BentPin_BottomContour", StringComparison.OrdinalIgnoreCase));
                if (!hasTopConsumer || !hasBottomConsumer)
                {
                    throw new InvalidOperationException(
                        "Actual multi-branch sample did not expose both output consumers for BentPin_Clean. "
                        + $"Rows='{DescribeBranchRows(cleanRows)}'");
                }

                OpenVisionRecipePipelineStepPreview topStep =
                    shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps
                        .FirstOrDefault(step => step.Name.Contains("Top Region", StringComparison.OrdinalIgnoreCase));
                if (topStep == null)
                {
                    throw new InvalidOperationException("Actual multi-branch sample did not expose the Top Region step.");
                }

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = topStep;
                Pump(40);
                IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> topRows =
                    shellHost.RecipeCommands.BranchOutputComparisonRows;
                bool hasSameInput = topRows.Any(row =>
                    row.Route.Contains("BentPin_Clean -> BentPin_BottomContour", StringComparison.OrdinalIgnoreCase));
                bool hasInputProducer = topRows.Any(row =>
                    row.Route.Contains("BentPin_Binary -> BentPin_Clean", StringComparison.OrdinalIgnoreCase));
                if (!hasSameInput || !hasInputProducer)
                {
                    throw new InvalidOperationException(
                        "Actual multi-branch sample did not expose same-input and input-producer rows for Top Region. "
                        + $"Rows='{DescribeBranchRows(topRows)}'");
                }

                return cleanRows.Count + topRows.Count;
            }
            finally
            {
                if (originalOption != null)
                {
                    OpenVisionRecipePipelineOption restoreOption = shellHost.RecipeCommands.PipelineOptions
                        .FirstOrDefault(option => string.Equals(
                            option.PipelineName,
                            originalOption.PipelineName,
                            StringComparison.OrdinalIgnoreCase));
                    if (restoreOption != null)
                    {
                        shellHost.RecipeCommands.SelectedPipelineOption = restoreOption;
                        Pump(40);
                    }
                }
            }
        }

        private static int VerifyActualThreeWayBranchPipeline(OpenVisionShellHostView shellHost)
        {
            string path = Path.Combine(FindRepositoryRoot(), "docs", "samples", "Contour_AllSymbolsAndFaint_LLM.pipeline.xml");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Actual 3+ branch sample pipeline was not found.", path);
            }

            OpenVisionRecipePipelineOption originalOption = shellHost.RecipeCommands.SelectedPipelineOption;
            try
            {
                if (!shellHost.RecipeCommands.ImportPipelineXmlFromPath(path))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not import the actual 3+ branch sample pipeline.");
                }

                Pump(60);
                OpenVisionRecipePipelineStepPreview fanOutStep =
                    shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps
                        .FirstOrDefault(step =>
                            string.Equals(step.InputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                            && string.Equals(step.OutputLayer, "TextSymbol_Binary", StringComparison.OrdinalIgnoreCase));
                if (fanOutStep == null)
                {
                    throw new InvalidOperationException(
                        "Actual 3+ branch sample did not expose the expected Main -> TextSymbol_Binary fan-out step. "
                        + $"Selected='{shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName}', "
                        + $"Steps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
                }

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = fanOutStep;
                Pump(40);
                IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> rows =
                    shellHost.RecipeCommands.BranchOutputComparisonRows;
                int sameInputRows = rows.Count(row =>
                    row.Status.Contains("같은", StringComparison.OrdinalIgnoreCase)
                    || row.Status.Contains("Same input", StringComparison.OrdinalIgnoreCase));
                bool hasFaintTop = rows.Any(row =>
                    row.Route.Contains("Main -> FaintTop_Range", StringComparison.OrdinalIgnoreCase));
                bool hasFaintPhone = rows.Any(row =>
                    row.Route.Contains("Main -> FaintPhone_Range", StringComparison.OrdinalIgnoreCase));
                bool hasIncorrectOverlayAlternative = rows.Any(row =>
                    row.Route.Contains("Main -> AllSymbols_Overlay", StringComparison.OrdinalIgnoreCase));
                bool hasOutputConsumer = rows.Any(row =>
                    row.Route.Contains("TextSymbol_Binary -> TextSymbol_Clean", StringComparison.OrdinalIgnoreCase));
                if (sameInputRows < 2 || !hasFaintTop || !hasFaintPhone || hasIncorrectOverlayAlternative || !hasOutputConsumer)
                {
                    throw new InvalidOperationException(
                        "Actual 3+ branch sample did not expose accurate same-input alternatives and output consumer rows. "
                        + $"SameInputRows={sameInputRows}, Rows='{DescribeBranchRows(rows)}'");
                }

                OpenVisionRecipePipelineStepPreview contourSourceStep =
                    shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps
                        .FirstOrDefault(step => string.Equals(
                            step.OutputLayer,
                            "TextSymbol_Contour",
                            StringComparison.OrdinalIgnoreCase));
                if (contourSourceStep == null)
                {
                    throw new InvalidOperationException("Actual 3+ branch sample did not expose TextSymbol_Contour.");
                }

                shellHost.RecipeCommands.SelectedPipelinePreviewStep = contourSourceStep;
                Pump(40);
                IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> contourRows =
                    shellHost.RecipeCommands.BranchOutputComparisonRows;
                bool hasDeclaredOverlayConsumer = contourRows.Any(row =>
                    row.Route.Contains("TextSymbol_Contour -> AllSymbols_Overlay", StringComparison.OrdinalIgnoreCase));
                if (!hasDeclaredOverlayConsumer)
                {
                    throw new InvalidOperationException(
                        "Actual 3+ branch sample did not expose its declared OverlayMerge source relation. "
                        + $"Rows='{DescribeBranchRows(contourRows)}'");
                }

                return rows.Count + contourRows.Count;
            }
            finally
            {
                if (originalOption != null)
                {
                    OpenVisionRecipePipelineOption restoreOption = shellHost.RecipeCommands.PipelineOptions
                        .FirstOrDefault(option => string.Equals(
                            option.PipelineName,
                            originalOption.PipelineName,
                            StringComparison.OrdinalIgnoreCase));
                    if (restoreOption != null)
                    {
                        shellHost.RecipeCommands.SelectedPipelineOption = restoreOption;
                        Pump(40);
                    }
                }
            }
        }

        private static string DescribeBranchRows(IEnumerable<OpenVisionRecipeBranchOutputComparisonRow> rows)
        {
            return string.Join(
                " | ",
                (rows ?? Array.Empty<OpenVisionRecipeBranchOutputComparisonRow>())
                    .Select(row => row.Status + " / " + row.StepName + " / " + row.Route + " / " + row.Action));
        }

        private static void RunLayerLoadMatchingFlow(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string matchingImagePath = Path.Combine(repoRoot, "docs", "samples", "public", "Matching_DiePad_Synthetic_OK.png");
            string matchingTemplatePath = Path.Combine(repoRoot, "docs", "samples", "public", "templates", "Matching_DiePad_Synthetic_Template.png");
            EnsureFileExists(matchingImagePath, "Direct EXE Matching sample image");
            EnsureFileExists(matchingTemplatePath, "Direct EXE Matching template image");

            string recipeName = "Smoke_LayerMatching_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                if (shellHost.IsWorkspaceLoadImageIntoLayerMenuVisibleForTest)
                {
                    throw new InvalidOperationException("Workspace context menu still exposes the duplicate load-into-layer image command.");
                }

                int runsBeforeLayerActions = shellHost.NativePreviewRunCount;
                int rowsBeforeLayerActions = shellHost.HostLayerRowCount;
                string loadedLayer = shellHost.CreateLayerForTest();
                Pump(40);
                if (string.IsNullOrWhiteSpace(loadedLayer)
                    || !shellHost.HasLayerForTest(loadedLayer)
                    || shellHost.HostLayerRowCount <= rowsBeforeLayerActions
                    || !string.Equals(shellHost.WorkspaceLayerTitle, loadedLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Direct EXE layer create did not create and show a new layer. "
                        + $"Layer={loadedLayer}, RowsBefore={rowsBeforeLayerActions}, RowsAfter={shellHost.HostLayerRowCount}, Workspace={shellHost.WorkspaceLayerTitle}");
                }

                if (!shellHost.LoadImageIntoLayerForTest(loadedLayer, matchingImagePath))
                {
                    throw new InvalidOperationException("Direct EXE load-image-into-layer failed for " + loadedLayer);
                }

                Pump(60);
                using (Bitmap loadedImage = shellHost.GetLayerImageCloneForTest(loadedLayer))
                {
                    if (loadedImage == null || loadedImage.Width <= 0 || loadedImage.Height <= 0)
                    {
                        throw new InvalidOperationException("Direct EXE loaded layer did not expose a valid image.");
                    }
                }

                if (!shellHost.HasWorkspaceLayerPreview
                    || !string.Equals(shellHost.WorkspaceLayerTitle, loadedLayer, StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != runsBeforeLayerActions
                    || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException(
                        "Direct EXE layer create/load changed preview state unexpectedly. "
                        + $"Layer={loadedLayer}, Workspace={shellHost.WorkspaceLayerTitle}, "
                        + $"RunsBefore={runsBeforeLayerActions}, RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_LayerLoaded.png"));

                if (!shellHost.LoadMainImageFromFileForTest(matchingImagePath))
                {
                    throw new InvalidOperationException("Direct EXE main image load failed: " + matchingImagePath);
                }

                Pump(60);
                if (!shellHost.HasWorkspaceLayerPreview
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || shellHost.NativePreviewRunCount != runsBeforeLayerActions
                    || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException(
                        "Direct EXE main image load changed preview state unexpectedly. "
                        + $"Workspace={shellHost.WorkspaceLayerTitle}, RunsBefore={runsBeforeLayerActions}, "
                        + $"RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
                }

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(80);
                int runsBeforeMatchingPreview = shellHost.NativePreviewRunCount;
                shellHost.SetActiveMatchingTemplatePathForTest(matchingTemplatePath);
                Pump(20);
                shellHost.ConfigureActiveMatchingForTest(ConfigureTutorialMatchingProperty);
                Pump(20);
                if (shellHost.NativePreviewRunCount != runsBeforeMatchingPreview)
                {
                    throw new InvalidOperationException(
                        "Direct EXE Matching setup triggered Preview before explicit run. "
                        + $"RunsBefore={runsBeforeMatchingPreview}, RunsAfter={shellHost.NativePreviewRunCount}");
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(140);

                string matchingStatus = shellHost.ActiveNativeStatusText;
                string matchingReview = shellHost.ActiveNativeResultReviewText;
                int runsAfterMatchingPreview = shellHost.NativePreviewRunCount;
                if (!shellHost.HasNativePreviewResult
                    || runsAfterMatchingPreview <= runsBeforeMatchingPreview
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Matching_Preview", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Matching_Preview", StringComparison.OrdinalIgnoreCase)
                    || matchingReview.IndexOf("Template Match", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "Direct EXE Matching preview did not produce the expected visible result layer. "
                        + $"Status={matchingStatus}, Review={matchingReview}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                        + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Workspace={shellHost.WorkspaceLayerTitle}, "
                        + $"RunsBefore={runsBeforeMatchingPreview}, RunsAfter={runsAfterMatchingPreview}");
                }

                using (Bitmap matchingPreview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
                {
                    if (matchingPreview == null || matchingPreview.Width <= 0 || matchingPreview.Height <= 0)
                    {
                        throw new InvalidOperationException("Direct EXE Matching_Preview output layer image was not created.");
                    }

                    matchingPreview.Save(Path.Combine(outputDirectory, "Matching_Preview.png"));
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingPreview.png"));

                if (!shellHost.CloseActiveWpfToolWindowForTest())
                {
                    throw new InvalidOperationException("Direct EXE Matching tool window did not close for workspace result verification.");
                }

                Pump(40);
                if (!shellHost.HasWorkspaceLayerPreview
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Matching_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Direct EXE Matching output was not visible in the main workspace after closing the tool window. "
                        + $"Workspace={shellHost.WorkspaceLayerTitle}, Preview={shellHost.HasWorkspaceLayerPreview}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingWorkspaceResult.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: layer-load-matching-flow" + Environment.NewLine
                    + "Recipe: " + recipeName + Environment.NewLine
                    + "LoadedLayer: " + loadedLayer + Environment.NewLine
                    + "PreviewRunsBeforeLayerLoad: " + runsBeforeLayerActions.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PreviewRunsBeforeMatching: " + runsBeforeMatchingPreview.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PreviewRunsAfterMatching: " + runsAfterMatchingPreview.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "WorkspaceAfterMatching: " + shellHost.WorkspaceLayerTitle + Environment.NewLine
                    + "Status: " + matchingStatus + Environment.NewLine
                    + "Review: " + matchingReview,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunWorkspaceStartupEmpty(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            WithDockingStateFileBackup(() =>
            {
                ClearCurrentDockingStateFiles();

                Application app = Application.Current ?? new Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

                OpenVisionShellHostWindow window = null;
                try
                {
                    ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                    window = new OpenVisionShellHostWindow(runtimeContext)
                    {
                        Width = 1600,
                        Height = 900,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    app.MainWindow = window;
                    window.Show();
                    Pump(36);

                    OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                        ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                    WaitForTaskWithPump(window.StartupPreparationTask, "shell startup Pipeline Review preparation");

                    shellHost.ShowPipelineLoadingForTest();
                    Pump(8);
                    if (!shellHost.IsShellBusyOverlayVisibleForTest
                        || !string.Equals(
                            shellHost.ShellBusyTitleForTest,
                            OpenVisionLanguageService.T("Shell.PipelineLoading.Title"),
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            $"Pipeline loading overlay mismatch. Visible={shellHost.IsShellBusyOverlayVisibleForTest}, Title='{shellHost.ShellBusyTitleForTest}'");
                    }

                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_PipelineLoadingFeedback.png"));
                    shellHost.HideShellBusyForTest();
                    Pump(4);
                    if (shellHost.IsShellBusyOverlayVisibleForTest)
                    {
                        throw new InvalidOperationException("Pipeline loading overlay remained visible after completion.");
                    }

                    if (shellHost.HasMainLayer || shellHost.HasWorkspaceLayerPreview)
                    {
                        throw new InvalidOperationException("Startup empty workspace must not create or preview a seeded Main image.");
                    }

                    if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
                    {
                        throw new InvalidOperationException("Startup empty workspace must not auto-open a tool window.");
                    }

                    if (!shellHost.IsSingleWorkspaceVisibleForTest)
                    {
                        throw new InvalidOperationException("Startup empty workspace must show the single workspace surface.");
                    }

                    if (shellHost.IsDockedWorkspaceVisibleForTest)
                    {
                        throw new InvalidOperationException("Startup empty workspace must not show the docked AvalonDock workspace.");
                    }

                    if (shellHost.DockedLayerCount != 0 || shellHost.DockedLayerTextureTileCount != 0)
                    {
                        throw new InvalidOperationException(
                            "Startup empty workspace must not create docked layer documents. "
                            + $"DockedLayers={shellHost.DockedLayerCount}, DockedTiles={shellHost.DockedLayerTextureTileCount}, Titles={shellHost.DockedLayerTitles}");
                    }

                    if (!shellHost.IsWorkspaceEmptyPromptVisible
                        || !shellHost.WorkspaceEmptyTitle.Contains(OpenVisionLanguageService.T("Shell.WorkspaceEmptyTitle"), StringComparison.Ordinal)
                        || !shellHost.WorkspaceEmptyDetail.Contains(OpenVisionLanguageService.T("Shell.WorkspaceEmptyDetail"), StringComparison.Ordinal)
                        || !shellHost.WorkspaceLoadImageButtonText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceLoadImageButton"), StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Startup empty workspace did not expose the localized image-load prompt. "
                            + $"PromptVisible={shellHost.IsWorkspaceEmptyPromptVisible}, Title='{shellHost.WorkspaceEmptyTitle}', Detail='{shellHost.WorkspaceEmptyDetail}', Button='{shellHost.WorkspaceLoadImageButtonText}'");
                    }

                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_StartupEmptyWorkspace.png"));
                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: workspace-startup-empty" + Environment.NewLine
                        + "Screenshots: OpenVisionLab_PipelineLoadingFeedback.png; OpenVisionLab_StartupEmptyWorkspace.png" + Environment.NewLine
                        + "PipelineLoadingOverlayVerified: True" + Environment.NewLine
                        + "SingleWorkspaceVisible: " + shellHost.IsSingleWorkspaceVisibleForTest + Environment.NewLine
                        + "DockedWorkspaceVisible: " + shellHost.IsDockedWorkspaceVisibleForTest + Environment.NewLine
                        + "WorkspaceEmptyPromptVisible: " + shellHost.IsWorkspaceEmptyPromptVisible + Environment.NewLine
                        + "DockedLayers: " + shellHost.DockedLayerCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                        + "DockedTiles: " + shellHost.DockedLayerTextureTileCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine,
                        Encoding.UTF8);
                }
                finally
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    app.Shutdown();
                }
            });
        }

        private static void RunLearnThresholdPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.Threshold);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenThresholdToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "threshold", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Threshold_BandPads_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 4", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Threshold Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE Threshold Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnThresholdPracticePanel",
                    "OpenVisionLearnThresholdOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_Threshold_Practice.png"));

                Button thresholdToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnThresholdOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Threshold Learn Tool button was not found.");
                thresholdToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, thresholdToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("ThresholdToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Threshold Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-threshold-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_Threshold_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnFilterPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.Filtering);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenFilteringToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "filter", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Filter_Denoise_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 4", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Filtering Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE Filtering Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnFilteringPracticePanel",
                    "OpenVisionLearnFilteringOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_Filter_Practice.png"));

                Button filterToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnFilteringOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Filtering Learn Tool button was not found.");
                filterToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, filterToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("FilterToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Filtering Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-filter-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_Filter_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnMorphologyPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.Morphology);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenMorphologyToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "morphology", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Morphology_Cleanup_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 4", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Morphology Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE Morphology Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnMorphologyPracticePanel",
                    "OpenVisionLearnMorphologyOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_Morphology_Practice.png"));

                Button morphologyToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnMorphologyOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Morphology Learn Tool button was not found.");
                morphologyToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, morphologyToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("MorphologyToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Morphology Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-morphology-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_Morphology_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnBlobPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.Blob);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (learnWindow.IsPracticeWorkflowExpandedForTest
                    || !learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenBlobToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "blob", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Blob_Particles_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 8..14", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Blob Learn did not expose the compact dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE Blob Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnBlobPracticePanel",
                    "OpenVisionLearnBlobOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_Blob_Practice.png"));

                Button blobToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnBlobOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Blob Learn Tool button was not found.");
                blobToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, blobToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Blob Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-blob-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_Blob_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnContourPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.Contour);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenContourToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "contour", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Contour_Shapes_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 5", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Contour Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE Contour Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnContourPracticePanel",
                    "OpenVisionLearnContourOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_Contour_Practice.png"));

                Button contourToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnContourOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Contour Learn Tool button was not found.");
                contourToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, contourToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("ContourToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE Contour Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                Window contourToolWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(item => item.IsVisible && FindVisualChildren<ContourToolWpfView>(item).Any())
                    ?? throw new InvalidOperationException("Contour Tool window was not found after opening it from Learn.");
                string contourToolMonitorEvidence = PlaceWindowOnLeftmostMonitor(contourToolWindow);
                Pump(24);
                SaveWindowScreenScreenshot(
                    contourToolWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Contour_Tool.png"));

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-contour-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_Contour_Practice.png" + Environment.NewLine
                    + "ToolScreenshot: OpenVisionLab_Contour_Tool.png" + Environment.NewLine
                    + "ToolMonitor: " + contourToolMonitorEvidence + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnEdgeDetectionPractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.EdgeDetection);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenEdgeLineToolsForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "edge-detection", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_EdgeDetection_Shapes_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("ResultCount 4", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE EdgeDetection Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE EdgeDetection Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnEdgeDetectionPracticePanel",
                    "OpenVisionLearnEdgeDetectionOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_EdgeDetection_Practice.png"));

                Button edgeDetectionToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnEdgeDetectionOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("EdgeDetection Learn Tool button was not found.");
                edgeDetectionToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, edgeDetectionToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("SimplePreprocessToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE EdgeDetection Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-edge-detection-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_EdgeDetection_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunLearnLineDistancePractice(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionLearnWindow learnWindow = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string workspaceLayerBefore = shellHost.WorkspaceLayerTitle;

                Button learnButton = FindVisualChildren<Button>(shellHost)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "HostLearnButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("Shell Learn button was not found.");
                if (learnButton.Command == null || !learnButton.Command.CanExecute(learnButton.CommandParameter))
                {
                    throw new InvalidOperationException("Shell Learn button command was not ready.");
                }

                learnButton.Command.Execute(learnButton.CommandParameter);
                Pump(24);

                learnWindow = Application.Current.Windows
                    .OfType<OpenVisionLearnWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Shell Learn button did not open the Learn window.");
                learnWindow.SelectTopic(OpenVisionLearnTopicIndex.LineDistance);
                learnWindow.Width = 1040;
                learnWindow.Height = 700;
                learnWindow.Activate();
                Pump(24);

                if (!learnWindow.CanOpenPracticeSamplesForTest
                    || !learnWindow.CanOpenLineDistanceToolForTest
                    || !string.Equals(learnWindow.SelectedTopicLearnPathIdForTest, "line", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("Public_Line_Pins_Good", StringComparison.Ordinal)
                    || !learnWindow.SelectedTopicPracticeTextForTest.Contains("DistanceMmRange 0.03", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE LineDistance Learn did not expose the dedicated practice workflow. "
                        + $"Path='{learnWindow.SelectedTopicLearnPathIdForTest}', Text='{learnWindow.SelectedTopicPracticeTextForTest}'.");
                }

                AssertVisibleAutomationIds(
                    learnWindow,
                    "Latest EXE LineDistance Learn practice",
                    "OpenVisionLearnPracticeSamplesButton",
                    "OpenVisionLearnLineDistancePracticePanel",
                    "OpenVisionLearnLineDistanceOpenToolButton");
                SaveWindowScreenScreenshot(
                    learnWindow,
                    Path.Combine(outputDirectory, "OpenVisionLab_Learn_LineDistance_Practice.png"));

                Button lineToolButton = FindVisualChildren<Button>(learnWindow)
                    .FirstOrDefault(item => string.Equals(
                        System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                        "OpenVisionLearnLineDistanceOpenToolButton",
                        StringComparison.Ordinal))
                    ?? throw new InvalidOperationException("LineDistance Learn Tool button was not found.");
                lineToolButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, lineToolButton));
                Pump(48);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.IsActiveWpfToolWindowVisibleForTest
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("LineToolWpfView", StringComparison.Ordinal)
                    || shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.HasNativePreviewResult
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.WorkspaceLayerTitle, workspaceLayerBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Latest EXE LineDistance Learn Tool action changed execution or workspace state. "
                        + $"Type='{shellHost.ActiveNativeDocumentTypeName}', Runs={previewRunsBefore}->{shellHost.NativePreviewRunCount}, "
                        + $"Layers={layerCountBefore}->{shellHost.LayerDocumentCount}, Workspace='{workspaceLayerBefore}'->'{shellHost.WorkspaceLayerTitle}'.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: learn-line-distance-practice" + Environment.NewLine
                    + "Screenshot: OpenVisionLab_Learn_LineDistance_Practice.png" + Environment.NewLine
                    + "PracticePath: " + learnWindow.SelectedTopicLearnPathIdForTest + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Tool: " + shellHost.ActiveNativeDocumentTypeName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (learnWindow?.IsVisible == true)
                {
                    learnWindow.Close();
                }

                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunPortfolioIndustrialCaptures(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string matchingImagePath = ResolveRequiredOption(args, "--matching-image");
            string matchingTemplatePath = ResolveRequiredOption(args, "--matching-template");
            string lineImagePath = ResolveRequiredOption(args, "--line-image");
            string blobImagePath = ResolveRequiredOption(args, "--blob-image");
            EnsureFileExists(matchingImagePath, "Portfolio Matching image");
            EnsureFileExists(matchingTemplatePath, "Portfolio Matching template");
            EnsureFileExists(lineImagePath, "Portfolio Line image");
            EnsureFileExists(blobImagePath, "Portfolio Blob image");

            string recipeName = "Smoke_PortfolioIndustrial_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            VisionPipeline matchingPipeline = CreatePortfolioImageMatchingPipeline(matchingTemplatePath);
            if (!SerializeHelper.SaveXmlFile(
                    Path.Combine(outputDirectory, "Portfolio_Card_ImageMatching.xml"),
                    matchingPipeline))
            {
                throw new InvalidOperationException("Portfolio Image Matching pipeline XML could not be saved.");
            }

            double matchingPipelineScore = SavePortfolioPipelineStage(
                matchingPipeline,
                1,
                matchingImagePath,
                Path.Combine(outputDirectory, "image_matching_runtime_overlay.png"),
                VisionPipelineKnownMetrics.ScoreMax,
                renderRuntimeOverlays: true);
            if (matchingPipelineScore < 70D)
            {
                throw new InvalidOperationException(
                    "Portfolio Image Matching direct runtime score was below 70. Score="
                    + matchingPipelineScore.ToString("0.###", CultureInfo.InvariantCulture));
            }

            VisionPipeline blobPipeline = CreatePortfolioBlobPipeline();
            if (!SerializeHelper.SaveXmlFile(
                    Path.Combine(outputDirectory, "Portfolio_Target_Threshold_Morphology_Blob.xml"),
                    blobPipeline))
            {
                throw new InvalidOperationException("Portfolio Blob pipeline XML could not be saved.");
            }

            RunPortfolioPinArrayProbe(
                new[] { "--smoke", "portfolio-pin-array-probe", "--line-image", lineImagePath },
                outputDirectory);
            string pinArrayOverlayPath = Path.Combine(outputDirectory, "pin_array_runtime_overlay.png");
            string pinArrayCsvPath = Path.Combine(outputDirectory, "pin_measurements.csv");
            if (!File.Exists(pinArrayOverlayPath) || !File.Exists(pinArrayCsvPath))
            {
                throw new InvalidOperationException("Portfolio pin-array evidence was not produced.");
            }
            string pinArrayProbeSummary = File.ReadAllText(Path.Combine(outputDirectory, "report.txt"), Encoding.UTF8)
                .Replace(Environment.NewLine, " | ");
            VisionPipelineStorage.Save(recipeName, blobPipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, blobPipeline.Name);
            SavePortfolioPipelineStage(blobPipeline, 1, blobImagePath, Path.Combine(outputDirectory, "Target_Binary.png"));
            SavePortfolioPipelineStage(blobPipeline, 2, blobImagePath, Path.Combine(outputDirectory, "Target_Clean.png"));
            double blobDirectCount = SavePortfolioPipelineStage(
                blobPipeline,
                3,
                blobImagePath,
                Path.Combine(outputDirectory, "blob_result_overlay.png"),
                VisionPipelineKnownMetrics.ResultCount,
                renderRuntimeOverlays: true);
            if (blobDirectCount != 16D)
            {
                throw new InvalidOperationException(
                    "Portfolio Blob direct runtime did not produce 16 objects. Count="
                    + blobDirectCount.ToString("0.###", CultureInfo.InvariantCulture));
            }

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);

            OpenVisionShellHostWindow window = null;
            StringBuilder report = new StringBuilder();
            string reportPath = Path.Combine(outputDirectory, "report.txt");
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);
                window.Activate();
                Pump(40);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(36);

                if (!shellHost.LoadMainImageFromFileForTest(matchingImagePath))
                {
                    throw new InvalidOperationException("Portfolio Matching image could not be loaded: " + matchingImagePath);
                }

                Pump(36);
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(70);
                int matchingRunsBefore = shellHost.NativePreviewRunCount;
                shellHost.ConfigureActiveMatchingForTest(property => property.AUTO_PREVIEW = false);
                shellHost.SetActiveMatchingTemplatePathForTest(matchingTemplatePath);
                shellHost.ConfigureActiveMatchingForTest(property =>
                {
                    property.AUTO_PREVIEW = false;
                    property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
                    property.SCORE_MIN = 0D;
                    property.NUM_MATCH = 1;
                    property.MAGNIFIATION = 1D;
                    property.USE_FIND_ANGLE = true;
                    property.FIND_ANGLE_MIN = -100;
                    property.FIND_ANGLE_MAX = 100;
                    property.FIND_ANGLE = 1D;
                    property.USE_COARSE_TO_FINE_ANGLE_SEARCH = true;
                    property.COARSE_ANGLE_STEP = 5D;
                    property.COARSE_ANGLE_TOP_K = 5;
                    property.USE_FIND_SCALE = true;
                    property.FIND_SCALE_MIN = 0.8D;
                    property.FIND_SCALE_MAX = 1.25D;
                    property.FIND_SCALE_STEP = 0.05D;
                    property.USE_CANNY = false;
                    property.USE_PADDING_COLOR_WHITE = false;
                    property.USE_THRESHOLD = false;
                    property.USE_ADAPTIVE_THRESHOLD = false;
                    property.USE_ROI = false;
                    property.ReloadTemplateImage();
                });
                Pump(36);
                if (shellHost.NativePreviewRunCount != matchingRunsBefore)
                {
                    throw new InvalidOperationException(
                        "Portfolio Image Matching setup ran Preview before the explicit action. "
                        + $"Runs={matchingRunsBefore}->{shellHost.NativePreviewRunCount}.");
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(140);
                string matchingStatus = shellHost.ActiveNativeStatusText;
                string matchingReview = shellHost.ActiveNativeResultReviewText;
                if (!TryParseMatchingC9Review(
                        matchingReview,
                        out int matchingCount,
                        out double matchingScore,
                        out _,
                        out _,
                        out _,
                        out _,
                        out _,
                        out _,
                        out _)
                    || matchingCount != 1
                    || matchingScore < 70D
                    || !shellHost.HasNativePreviewResult
                    || shellHost.NativePreviewRunCount != matchingRunsBefore + 1)
                {
                    throw new InvalidOperationException(
                        "Portfolio Image Matching did not produce one accepted independent result. "
                        + $"Count={matchingCount}, Score={matchingScore:0.###}, Status='{matchingStatus}', Review='{matchingReview}'");
                }

                if (Math.Abs(matchingPipelineScore - matchingScore) > 0.5D)
                {
                    throw new InvalidOperationException(
                        "Portfolio Image Matching Tool View and pipeline scores did not agree. "
                        + $"Tool={matchingScore:0.###}, Pipeline={matchingPipelineScore:0.###}");
                }

                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "01_image_matching_tool.png"));
                using (Bitmap matchingPreview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
                {
                    matchingPreview?.Save(Path.Combine(outputDirectory, "image_matching_result_overlay.png"));
                }

                using (Bitmap templateBitmap = new Bitmap(matchingTemplatePath))
                {
                    if (!shellHost.AddLayerImageForTest("Registered_Template", templateBitmap))
                    {
                        throw new InvalidOperationException("Portfolio Matching template comparison layer could not be created.");
                    }
                }

                shellHost.CloseActiveWpfToolWindowForTest();
                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Registered_Template", "Matching_Preview" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio Matching comparison could not dock " + layer + ".");
                    }
                }

                if (!shellHost.ArrangeDockedLayerGridForTest("Main", "Registered_Template", "Matching_Preview"))
                {
                    throw new InvalidOperationException("Portfolio Matching comparison grid could not be arranged.");
                }

                Pump(80);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_matching_three_panel_comparison.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(lineImagePath))
                {
                    throw new InvalidOperationException("Portfolio Line image could not be loaded: " + lineImagePath);
                }

                Pump(36);
                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(40);
                shellHost.CloseActiveWpfToolWindowForTest();
                Pump(20);
                using (Bitmap lineEvidence = new Bitmap(pinArrayOverlayPath))
                {
                    if (!shellHost.AddLayerImageForTest("Pin_Array_Measurement_Result", lineEvidence)
                        && !shellHost.SetLayerImageForTest("Pin_Array_Measurement_Result", lineEvidence))
                    {
                        throw new InvalidOperationException("Portfolio pin-array measurement evidence layer could not be published.");
                    }
                }

                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Pin_Array_Measurement_Result" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio pin-array comparison could not dock " + layer + ".");
                    }
                }

                if (!shellHost.ArrangeDockedLayerPanesForTest("Horizontal", "Main", "Pin_Array_Measurement_Result"))
                {
                    throw new InvalidOperationException("Portfolio pin-array comparison panes could not be arranged.");
                }

                Pump(70);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "03_pin_array_before_after_comparison.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(blobImagePath))
                {
                    throw new InvalidOperationException("Portfolio Blob image could not be loaded: " + blobImagePath);
                }

                Pump(36);
                shellHost.OpenSamplePipelineForTest();
                Pump(70);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "portfolio Blob pipeline review");
                shellHost.SelectPipelineReviewStepForTest(2, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(90);
                int blobCount = shellHost.PipelineReviewObjectResultCountForTest;
                string blobExecutionState = shellHost.PipelineReviewExecutionState;
                string blobSummary = shellHost.PipelineReviewResultSummaryText;
                if (shellHost.PipelineReviewStepCount != 3
                    || !shellHost.HasPipelineReviewInputPreview
                    || !shellHost.HasPipelineReviewOutputPreview
                    || blobCount != 16
                    || !blobExecutionState.StartsWith("Completed", StringComparison.OrdinalIgnoreCase)
                    || !blobSummary.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Portfolio Blob pipeline did not produce the expected 16-object review. "
                        + $"Steps={shellHost.PipelineReviewStepCount}, Count={blobCount}, State='{blobExecutionState}', "
                        + $"Summary='{shellHost.PipelineReviewResultSummaryText}', Detail='{shellHost.PipelineReviewResultDetailText}'");
                }

                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "05_blob_pipeline_review.png"));
                shellHost.SelectToolForTest(VISION_MENU.Blob);
                Pump(50);
                shellHost.CloseActiveWpfToolWindowForTest();
                Pump(30);
                foreach (KeyValuePair<string, string> layer in new Dictionary<string, string>
                {
                    ["Target_Binary"] = Path.Combine(outputDirectory, "Target_Binary.png"),
                    ["Target_Clean"] = Path.Combine(outputDirectory, "Target_Clean.png"),
                    ["Target_Blob_Result"] = Path.Combine(outputDirectory, "blob_result_overlay.png")
                })
                {
                    using Bitmap layerImage = new Bitmap(layer.Value);
                    if (!shellHost.AddLayerImageForTest(layer.Key, layerImage)
                        && !shellHost.SetLayerImageForTest(layer.Key, layerImage))
                    {
                        throw new InvalidOperationException("Portfolio Blob comparison layer could not be published: " + layer.Key);
                    }
                }

                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Target_Binary", "Target_Clean", "Target_Blob_Result" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio Blob comparison could not dock " + layer + ".");
                    }
                }

                if (!shellHost.ArrangeDockedLayerGridForTest("Main", "Target_Binary", "Target_Clean", "Target_Blob_Result"))
                {
                    throw new InvalidOperationException("Portfolio Blob four-panel grid could not be arranged.");
                }

                Pump(90);
                if (shellHost.DockedLayerCount != 4
                    || shellHost.DockedLayerPaneCount < 4
                    || shellHost.DockedLayerTextureTileCount < 4)
                {
                    throw new InvalidOperationException(
                        "Portfolio Blob comparison grid did not keep four rendered panels. "
                        + $"Layers={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}");
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "06_blob_pipeline_four_panel.png"));

                report.AppendLine("Result: PASS");
                report.AppendLine("Scenario: portfolio-industrial-captures");
                report.AppendLine("Executable: " + (Environment.ProcessPath ?? string.Empty));
                report.AppendLine("ExecutableSha256: " + ComputeC9FileSha256(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location));
                report.AppendLine("ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location);
                report.AppendLine("ManagedAssemblySha256: " + ComputeC9FileSha256(typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location));
                report.AppendLine(monitorEvidence);
                report.AppendLine("MatchingImage: " + matchingImagePath);
                report.AppendLine("MatchingImageSha256: " + ComputeC9FileSha256(matchingImagePath));
                report.AppendLine("MatchingTemplate: " + matchingTemplatePath);
                report.AppendLine("MatchingTemplateSha256: " + ComputeC9FileSha256(matchingTemplatePath));
                report.AppendLine("MatchingCount: " + matchingCount.ToString(CultureInfo.InvariantCulture));
                report.AppendLine("MatchingScore: " + matchingScore.ToString("0.###", CultureInfo.InvariantCulture));
                report.AppendLine("MatchingPipelineScore: " + matchingPipelineScore.ToString("0.###", CultureInfo.InvariantCulture));
                report.AppendLine("MatchingAlgorithm: Image Matching / CCoeffNormed / angle and scale search / independent template and input");
                report.AppendLine("LineImage: " + lineImagePath);
                report.AppendLine("LineImageSha256: " + ComputeC9FileSha256(lineImagePath));
                report.AppendLine("PinArrayPipeline: Portfolio_Pin_Array_LineDistance");
                report.AppendLine("PinArrayEvidence: " + pinArrayOverlayPath);
                report.AppendLine("PinArrayMeasurements: " + pinArrayCsvPath);
                report.AppendLine("PinArrayRuntime: " + pinArrayProbeSummary);
                report.AppendLine("LineCalibration: Not calibrated; pixel-only evidence");
                report.AppendLine("BlobImage: " + blobImagePath);
                report.AppendLine("BlobImageSha256: " + ComputeC9FileSha256(blobImagePath));
                report.AppendLine("BlobResultCount: " + blobCount.ToString(CultureInfo.InvariantCulture));
                report.AppendLine("BlobDirectRuntimeCount: " + blobDirectCount.ToString("0.###", CultureInfo.InvariantCulture));
                report.AppendLine("BlobPipeline: " + blobPipeline.Name);
                report.AppendLine("BlobPipelineSteps: " + blobPipeline.Steps.Count.ToString(CultureInfo.InvariantCulture));
                report.AppendLine("DockedComparisonPanels: " + shellHost.DockedLayerCount.ToString(CultureInfo.InvariantCulture));
                report.AppendLine("Boundary: selected-image actual-EXE evidence backed by separate bounded N-image runs; not calibrated metrology or field robustness.");
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunPortfolioCardMatchProbe(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string imagePath = ResolveRequiredOption(args, "--matching-image");
            string templatePath = ResolveRequiredOption(args, "--matching-template");
            string mode = ResolveOptionalTextOption(args, "--matching-mode");
            bool edgeBased = !string.Equals(mode, "image", StringComparison.OrdinalIgnoreCase);
            VisionPipeline pipeline = edgeBased
                ? CreatePortfolioEdgeBasedMatchingPipeline(templatePath)
                : CreatePortfolioImageMatchingPipeline(templatePath);
            string prefix = edgeBased ? "edge" : "image";
            SerializeHelper.SaveXmlFile(Path.Combine(outputDirectory, prefix + "_matching.xml"), pipeline);
            double score = SavePortfolioPipelineStage(
                pipeline,
                1,
                imagePath,
                Path.Combine(outputDirectory, prefix + "_matching_overlay.png"),
                VisionPipelineKnownMetrics.ScoreMax,
                renderRuntimeOverlays: true);
            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: PASS" + Environment.NewLine
                + "Mode: " + (edgeBased ? "EdgeBasedMatching" : "Matching") + Environment.NewLine
                + "Image: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "Template: " + templatePath + Environment.NewLine
                + "TemplateSha256: " + ComputeC9FileSha256(templatePath) + Environment.NewLine
                + "ScoreMax: " + score.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "Acceptance: ScoreMax >= " + (edgeBased ? "75" : "80"),
                Encoding.UTF8);
        }

        private static void RunPortfolioPatternGridCaptures(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string layersDirectory = ResolveRequiredOption(args, "--layers-dir");
            string pipelinePath = ResolveRequiredOption(args, "--pipeline");
            string sourceImagePath = ResolveRequiredOption(args, "--source-image");
            EnsureFileExists(pipelinePath, "Portfolio pipeline");
            EnsureFileExists(sourceImagePath, "Portfolio source image");

            string[] imageNames = Enumerable.Range(1, 6)
                .Select(index => Path.Combine(layersDirectory, index.ToString("00", CultureInfo.InvariantCulture) + ".png"))
                .ToArray();
            foreach (string imagePath in imageNames)
            {
                EnsureFileExists(imagePath, "Portfolio layer image");
            }

            if (!SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline)
                || pipeline == null)
            {
                throw new InvalidOperationException("Portfolio pipeline could not be loaded: " + pipelinePath);
            }
            string recipeName = "Smoke_PortfolioPatternGrid_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);

            string[] layerTitles =
            {
                "MATCH A0 S1.00 | 99.1",
                "MATCH A+15 S1.00 | 94.3",
                "MATCH A-10 S1.00 | 91.9",
                "MATCH A0 S0.90 | 99.0",
                "MATCH A+10 S1.10 | 97.9",
                "THRESHOLD T130"
            };
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1920,
                    Height = 1032,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);
                window.Activate();
                Pump(50);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(30);

                for (int index = 0; index < layerTitles.Length; index++)
                {
                    using Bitmap image = new Bitmap(imageNames[index]);
                    if (!shellHost.AddLayerImageForTest(layerTitles[index], image)
                        && !shellHost.SetLayerImageForTest(layerTitles[index], image))
                    {
                        throw new InvalidOperationException("Portfolio layer could not be published: " + layerTitles[index]);
                    }
                }

                shellHost.ClearDockedLayersForTest();
                foreach (string layerTitle in layerTitles)
                {
                    if (!shellHost.DockLayerForTest(layerTitle))
                    {
                        throw new InvalidOperationException("Portfolio layer could not be docked: " + layerTitle);
                    }
                }
                if (!shellHost.ArrangeDockedLayerGridForTest(layerTitles))
                {
                    throw new InvalidOperationException("Portfolio 2x3 layer grid could not be arranged.");
                }
                Pump(100);
                if (shellHost.DockedLayerCount != 6
                    || shellHost.DockedLayerPaneCount < 6
                    || shellHost.DockedLayerTextureTileCount < 6)
                {
                    throw new InvalidOperationException(
                        $"Portfolio grid is incomplete. Layers={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}");
                }
                int gridPaneCount = shellHost.DockedLayerPaneCount;
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_pattern_rotation_scale_2x3.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(sourceImagePath))
                {
                    throw new InvalidOperationException("Portfolio source image could not be loaded: " + sourceImagePath);
                }
                shellHost.OpenSamplePipelineForTest();
                Pump(50);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "portfolio Pattern Pipeline Review");
                shellHost.SelectPipelineReviewStepForTest(0, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(80);
                if (!shellHost.HasPipelineReviewInputPreview
                    || !shellHost.HasPipelineReviewOutputPreview
                    || (!shellHost.PipelineReviewExecutionState.StartsWith("Completed", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.PipelineReviewExecutionState.StartsWith("완료", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Portfolio Pipeline Review did not complete with input and output evidence. State="
                        + shellHost.PipelineReviewExecutionState);
                }
                window.Height = 1500;
                window.UpdateLayout();
                Pump(30);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_pipeline_review_runtime_overlay.png"));

                string executablePath = Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
                string managedAssemblyPath = typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location;
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: portfolio-pattern-grid-captures" + Environment.NewLine
                    + "Executable: " + executablePath + Environment.NewLine
                    + "ExecutableSha256: " + ComputeC9FileSha256(executablePath) + Environment.NewLine
                    + "ManagedAssembly: " + managedAssemblyPath + Environment.NewLine
                    + "ManagedAssemblySha256: " + ComputeC9FileSha256(managedAssemblyPath) + Environment.NewLine
                    + monitorEvidence + Environment.NewLine
                    + "Layers: 6" + Environment.NewLine
                    + "Panes: " + gridPaneCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "SourceImage: " + sourceImagePath + Environment.NewLine
                    + "SourceImageSha256: " + ComputeC9FileSha256(sourceImagePath) + Environment.NewLine
                    + "PipelinePath: " + pipelinePath + Environment.NewLine
                    + "PipelineSha256: " + ComputeC9FileSha256(pipelinePath) + Environment.NewLine
                    + "Pipeline: " + pipeline.Name + Environment.NewLine
                    + "PipelineReviewState: " + shellHost.PipelineReviewExecutionState + Environment.NewLine
                    + "Boundary: actual OpenVisionLab EXE rendering; no Computer Use overlay or cursor visualization.",
                    Encoding.UTF8);
            }
            finally
            {
                window?.Close();
                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunPortfolioStageGridCaptures(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string pipelinePath = ResolveRequiredOption(args, "--pipeline");
            string sourceImagePath = ResolveRequiredOption(args, "--source-image");
            string requestedLanguage = ResolveOptionalTextOption(args, "--language");
            bool korean = !string.Equals(requestedLanguage, "en", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(requestedLanguage, "english", StringComparison.OrdinalIgnoreCase);
            EnsureFileExists(pipelinePath, "Portfolio stage pipeline");
            EnsureFileExists(sourceImagePath, "Portfolio stage source image");

            if (!SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline)
                || pipeline == null
                || pipeline.Steps.Count == 0)
            {
                throw new InvalidOperationException("Portfolio stage pipeline could not be loaded: " + pipelinePath);
            }

            string stageDirectory = Path.Combine(outputDirectory, "stage_images");
            Directory.CreateDirectory(stageDirectory);
            List<string> stageImagePaths = new List<string>();
            List<string> layerTitles = new List<string>();
            string sourceCopyPath = Path.Combine(stageDirectory, "01_source.png");
            File.Copy(sourceImagePath, sourceCopyPath, true);
            stageImagePaths.Add(sourceCopyPath);
            layerTitles.Add(korean ? "1 원본 이미지" : "1 SOURCE IMAGE");

            for (int index = 0; index < pipeline.Steps.Count; index++)
            {
                VisionPipelineStep step = pipeline.Steps[index];
                string stagePath = Path.Combine(
                    stageDirectory,
                    (index + 2).ToString("D2", CultureInfo.InvariantCulture)
                    + "_"
                    + Regex.Replace(step.ToolType ?? "Step", "[^A-Za-z0-9_-]", "_")
                    + ".png");
                SavePortfolioPipelineStage(
                    pipeline,
                    index + 1,
                    sourceImagePath,
                    stagePath,
                    renderRuntimeOverlaysWhenAvailable: true,
                    suppressRuntimeOverlayLabels: true);
                stageImagePaths.Add(stagePath);
                layerTitles.Add(FormatPortfolioStageTitle(index + 2, step.ToolType, korean));
            }

            string recipeStem = pipeline.Name.Contains("HoleArray", StringComparison.OrdinalIgnoreCase)
                || pipeline.Name.Contains("PerforatedPlate", StringComparison.OrdinalIgnoreCase)
                ? "Hole_Array"
                : pipeline.Name.Contains("Shaft", StringComparison.OrdinalIgnoreCase)
                    ? "Shaft_Pitting"
                    : pipeline.Name.Contains("LeadWidth", StringComparison.OrdinalIgnoreCase)
                        ? "Lead_Width"
                        : "Stage_Showcase";
            string recipeName = recipeStem + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(
                korean ? OpenVisionLanguage.Korean : OpenVisionLanguage.English,
                false);
            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1920,
                    Height = 1032,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);
                window.Activate();
                Pump(50);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SelectLanguageForTest(
                    korean ? OpenVisionLanguage.Korean : OpenVisionLanguage.English);
                Pump(30);
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(30);

                for (int index = 0; index < layerTitles.Count; index++)
                {
                    using Bitmap image = new Bitmap(stageImagePaths[index]);
                    if (!shellHost.AddLayerImageForTest(layerTitles[index], image)
                        && !shellHost.SetLayerImageForTest(layerTitles[index], image))
                    {
                        throw new InvalidOperationException("Portfolio stage layer could not be published: " + layerTitles[index]);
                    }
                }

                shellHost.ClearDockedLayersForTest();
                foreach (string layerTitle in layerTitles)
                {
                    if (!shellHost.DockLayerForTest(layerTitle))
                    {
                        throw new InvalidOperationException("Portfolio stage layer could not be docked: " + layerTitle);
                    }
                }
                if (!shellHost.ArrangeDockedLayerGridForTest(layerTitles.ToArray()))
                {
                    throw new InvalidOperationException("Portfolio stage layer grid could not be arranged.");
                }
                Pump(100);
                if (shellHost.DockedLayerCount != layerTitles.Count
                    || shellHost.DockedLayerPaneCount < layerTitles.Count
                    || shellHost.DockedLayerTextureTileCount < layerTitles.Count)
                {
                    throw new InvalidOperationException(
                        $"Portfolio stage grid is incomplete. Layers={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}");
                }
                int gridPaneCount = shellHost.DockedLayerPaneCount;
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_stage_grid_actual_exe.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(sourceImagePath))
                {
                    throw new InvalidOperationException("Portfolio source image could not be loaded: " + sourceImagePath);
                }
                bool pipelineReviewCachedBeforeOpen = shellHost.HasPipelineReviewDocumentForTest;
                Stopwatch pipelineReviewOpenStopwatch = Stopwatch.StartNew();
                shellHost.OpenSamplePipelineForTest();
                long pipelineReviewSelectMilliseconds = pipelineReviewOpenStopwatch.ElapsedMilliseconds;
                Pump(50);
                long pipelineReviewReadyMilliseconds = pipelineReviewOpenStopwatch.ElapsedMilliseconds;
                string pipelineReviewOpenTiming = shellHost.LastToolOpenTimingTextForTest;
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "portfolio stage Pipeline Review");
                shellHost.SelectPipelineReviewStepForTest(
                    pipeline.Steps.Count - 1,
                    OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(80);
                if (!shellHost.HasPipelineReviewInputPreview
                    || !shellHost.HasPipelineReviewOutputPreview
                    || (!shellHost.PipelineReviewExecutionState.StartsWith("Completed", StringComparison.OrdinalIgnoreCase)
                        && !shellHost.PipelineReviewExecutionState.StartsWith("완료", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Portfolio stage Pipeline Review did not complete with input and output evidence. State="
                        + shellHost.PipelineReviewExecutionState);
                }
                window.Height = 1500;
                window.UpdateLayout();
                Pump(30);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_pipeline_review_actual_exe.png"));

                System.Windows.Controls.Image inputPreview = FindNamedVisualChild<System.Windows.Controls.Image>(window, "imgInputPreview", "Portfolio Pipeline Review");
                System.Windows.Controls.Image outputPreview = FindNamedVisualChild<System.Windows.Controls.Image>(window, "imgOutputPreview", "Portfolio Pipeline Review");
                string fullPreviewSize = FormatPreviewSize(inputPreview, outputPreview);

                window.Width = 1600;
                window.Height = 900;
                window.UpdateLayout();
                Pump(30);
                string widePreviewSize = FormatPreviewSize(inputPreview, outputPreview);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "03_pipeline_review_wide_actual_exe.png"));

                window.Width = 1280;
                window.Height = 800;
                window.UpdateLayout();
                Pump(30);
                string compactPreviewSize = FormatPreviewSize(inputPreview, outputPreview);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "04_pipeline_review_compact_actual_exe.png"));
                if (inputPreview.ActualWidth < 200D
                    || outputPreview.ActualWidth < 200D
                    || inputPreview.ActualHeight < 150D
                    || outputPreview.ActualHeight < 150D)
                {
                    throw new InvalidOperationException(
                        "Portfolio Pipeline Review compact image area is too small. " + compactPreviewSize);
                }

                System.Windows.Controls.Primitives.ToggleButton guideToggle =
                    FindNamedVisualChild<System.Windows.Controls.Primitives.ToggleButton>(
                        window,
                        "btnReviewGuideToggle",
                        "Portfolio Pipeline Review guide toggle");
                if (guideToggle.IsChecked == true)
                {
                    throw new InvalidOperationException("Pipeline Review guidance must be collapsed by default.");
                }
                guideToggle.IsChecked = true;
                Pump(20);
                string compactGuidePreviewSize = FormatPreviewSize(inputPreview, outputPreview);
                if (inputPreview.ActualWidth < 150D
                    || outputPreview.ActualWidth < 150D
                    || inputPreview.ActualHeight < 110D
                    || outputPreview.ActualHeight < 110D)
                {
                    throw new InvalidOperationException(
                        "Portfolio Pipeline Review compact guide image area is too small. " + compactGuidePreviewSize);
                }
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "05_pipeline_review_compact_guide_expanded_actual_exe.png"));
                guideToggle.IsChecked = false;
                Pump(8);

                string executablePath = Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
                string managedAssemblyPath = typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location;
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: portfolio-stage-grid-captures" + Environment.NewLine
                    + "Language: " + (korean ? "ko" : "en") + Environment.NewLine
                    + "LanguageDisplay: " + shellHost.SelectedLanguageDisplayNameForTest + Environment.NewLine
                    + "Executable: " + executablePath + Environment.NewLine
                    + "ExecutableSha256: " + ComputeC9FileSha256(executablePath) + Environment.NewLine
                    + "ManagedAssembly: " + managedAssemblyPath + Environment.NewLine
                    + "ManagedAssemblySha256: " + ComputeC9FileSha256(managedAssemblyPath) + Environment.NewLine
                    + monitorEvidence + Environment.NewLine
                    + "Layers: " + layerTitles.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Panes: " + gridPaneCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "SourceImage: " + sourceImagePath + Environment.NewLine
                    + "SourceImageSha256: " + ComputeC9FileSha256(sourceImagePath) + Environment.NewLine
                    + "PipelinePath: " + pipelinePath + Environment.NewLine
                    + "PipelineSha256: " + ComputeC9FileSha256(pipelinePath) + Environment.NewLine
                    + "Pipeline: " + pipeline.Name + Environment.NewLine
                    + "PipelineReviewState: " + shellHost.PipelineReviewExecutionState + Environment.NewLine
                    + "PipelineReviewCachedBeforeOpen: " + pipelineReviewCachedBeforeOpen.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PipelineReviewSelectMs: " + pipelineReviewSelectMilliseconds.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PipelineReviewReadyMs: " + pipelineReviewReadyMilliseconds.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PipelineReviewInternalTiming: " + pipelineReviewOpenTiming + Environment.NewLine
                    + "PipelineReviewFullPreviewSize: " + fullPreviewSize + Environment.NewLine
                    + "PipelineReviewWidePreviewSize: " + widePreviewSize + Environment.NewLine
                    + "PipelineReviewCompactPreviewSize: " + compactPreviewSize + Environment.NewLine
                    + "PipelineReviewCompactGuidePreviewSize: " + compactGuidePreviewSize + Environment.NewLine
                    + "Boundary: actual OpenVisionLab desktop EXE rendering; no Computer Use overlay or cursor visualization.",
                    Encoding.UTF8);
            }
            finally
            {
                window?.Close();
                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static string FormatPreviewSize(
            System.Windows.Controls.Image inputPreview,
            System.Windows.Controls.Image outputPreview)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Input={0:0}x{1:0}|Output={2:0}x{3:0}",
                inputPreview.ActualWidth,
                inputPreview.ActualHeight,
                outputPreview.ActualWidth,
                outputPreview.ActualHeight);
        }

        private static string FormatPortfolioStageTitle(int index, string toolType, bool korean)
        {
            string normalizedToolType = string.IsNullOrWhiteSpace(toolType) ? "STEP" : toolType.Trim();
            string title = normalizedToolType.ToUpperInvariant();
            if (korean)
            {
                title = normalizedToolType switch
                {
                    "Filter" => "필터",
                    "Threshold" => "스레시홀드",
                    "Morphology" => "모폴로지",
                    "Blob" => "블랍",
                    "Contour" => "컨투어",
                    "LineDistance" => "길이 측정",
                    "EdgeDetection" => "엣지",
                    _ => normalizedToolType
                };
            }
            else
            {
                title = normalizedToolType switch
                {
                    "Blob" => "BLOB",
                    "Contour" => "CONTOUR",
                    "LineDistance" => "LINE WIDTH",
                    "EdgeDetection" => "EDGE",
                    _ => title
                };
            }

            return index.ToString(CultureInfo.InvariantCulture) + " " + title;
        }

        private static VisionPipeline CreatePortfolioEdgeBasedMatchingPipeline(string templatePath)
        {
            EdgeBasedMatchingProperty property = new EdgeBasedMatchingProperty("Portfolio_Card_EdgeBasedMatching");
            ConfigurePortfolioEdgeBasedMatchingProperty(property, templatePath);
            VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(
                property,
                "Main",
                "EdgeBasedMatching_Preview");
            step.Name = "01 Card Edge Match";
            step.MaxElapsedMilliseconds = 15000;
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Card_EdgeBasedMatching" };
            pipeline.Steps.Add(step);
            return pipeline;
        }

        private static void ConfigurePortfolioEdgeBasedMatchingProperty(
            EdgeBasedMatchingProperty property,
            string templatePath)
        {
            property.PATTERN_PATH = templatePath;
            property.SCORE_MIN = 0.75D;
            property.NUM_MATCH = 1;
            property.USE_UNIQUE_MATCH_VALIDATION = false;
            property.ALLOW_GLOBAL_POLARITY_REVERSAL = false;
            property.USE_FIND_ANGLE = true;
            property.FIND_ANGLE_MIN = -10;
            property.FIND_ANGLE_MAX = 10;
            property.FIND_ANGLE = 1D;
            property.USE_COARSE_TO_FINE_ANGLE_SEARCH = true;
            property.COARSE_ANGLE_STEP = 5D;
            property.COARSE_ANGLE_TOP_K = 3;
            property.USE_FIND_SCALE = true;
            property.FIND_SCALE_MIN = 0.9D;
            property.FIND_SCALE_MAX = 1.1D;
            property.FIND_SCALE_STEP = 0.05D;
            property.CANNY_LOW = 30;
            property.CANNY_HIGH = 90;
            property.CANNY_APERTURE_SIZE = 3;
            property.USE_L2_GRADIENT = true;
            property.SEARCH_STEP = 2;
            property.USE_POSITION_REFINE = false;
            property.USE_SUBPIXEL_REFINE = false;
            property.GREEDINESS = 0.9D;
            property.USE_PYRAMID_POSITION_PROPOSAL = false;
            property.USE_HYBRID_VERIFY = false;
            property.MAX_TEMPLATE_POINTS = 300;
            property.MIN_GRADIENT_MAGNITUDE = 1D;
            property.USE_DRAW_IMAGE = true;
            property.USE_THRESHOLD = false;
            property.USE_ADAPTIVE_THRESHOLD = false;
            property.USE_ROI = false;
        }

        private static VisionPipeline CreatePortfolioImageMatchingPipeline(string templatePath)
        {
            MatchingProperty property = new MatchingProperty("Portfolio_Card_ImageMatching")
            {
                PATTERN_PATH = templatePath,
                AUTO_PREVIEW = false,
                MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
                SCORE_MIN = 0.8D,
                NUM_MATCH = 1,
                USE_FIND_ANGLE = true,
                FIND_ANGLE_MIN = -20,
                FIND_ANGLE_MAX = 20,
                FIND_ANGLE = 1D,
                USE_COARSE_TO_FINE_ANGLE_SEARCH = true,
                COARSE_ANGLE_STEP = 5D,
                COARSE_ANGLE_TOP_K = 3,
                USE_FIND_SCALE = true,
                FIND_SCALE_MIN = 0.85D,
                FIND_SCALE_MAX = 1.15D,
                FIND_SCALE_STEP = 0.05D,
                USE_CANNY = false,
                USE_PADDING_COLOR_WHITE = false,
                USE_THRESHOLD = false,
                USE_ADAPTIVE_THRESHOLD = false,
                USE_ROI = false
            };
            VisionPipelineStep step = VisionPipelineStepBuilder.FromProperty(
                property,
                "Main",
                "Matching_Preview");
            step.Name = "01 Card Image Match";
            step.MaxElapsedMilliseconds = 15000;
            step.UseAcceptance = true;
            step.ExpectedSuccess = true;
            step.AcceptanceMetricName = VisionPipelineKnownMetrics.ScoreMax;
            step.UseAcceptanceMetricMinimum = true;
            step.AcceptanceMetricMinimum = 80D;
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Card_ImageMatching" };
            pipeline.Steps.Add(step);
            return pipeline;
        }

        private static VisionPipeline CreatePortfolioBlobPipeline()
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Target_Threshold_Morphology_Blob" };
            VisionPipelineStep threshold = new VisionPipelineStep
            {
                Name = "01 Target Binary",
                ToolType = "Threshold",
                InputLayer = "Main",
                OutputLayer = "Target_Binary"
            };
            threshold.Parameters["Mode"] = "Threshold";
            threshold.Parameters["Threshold"] = "150";
            threshold.Parameters["MaxValue"] = "255";
            threshold.Parameters["ThresholdType"] = "Binary";
            pipeline.Steps.Add(threshold);

            VisionPipelineStep morphology = new VisionPipelineStep
            {
                Name = "02 Target Cleanup",
                ToolType = "Morphology",
                InputLayer = "Target_Binary",
                OutputLayer = "Target_Clean"
            };
            morphology.Parameters["Shape"] = "Ellipse";
            morphology.Parameters["Operator"] = "Open";
            morphology.Parameters["KernelWidth"] = "3";
            morphology.Parameters["KernelHeight"] = "3";
            morphology.Parameters["Iterations"] = "1";
            pipeline.Steps.Add(morphology);

            VisionPipelineStep blob = new VisionPipelineStep
            {
                Name = "03 Target Blob Count",
                ToolType = "Blob",
                InputLayer = "Target_Clean",
                OutputLayer = "Target_Blob_Result",
                UseAcceptance = true,
                ExpectedSuccess = true,
                AcceptanceMetricName = "ResultCount",
                UseAcceptanceMetricMinimum = true,
                AcceptanceMetricMinimum = 16D,
                UseAcceptanceMetricMaximum = true,
                AcceptanceMetricMaximum = 16D,
                MaxElapsedMilliseconds = 1000
            };
            blob.Parameters["Name"] = "Portfolio_Target_Blob_Count";
            blob.Parameters["PIXELPERMM"] = "0";
            blob.Parameters["USE_THRESHOLD"] = "false";
            blob.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            blob.Parameters["USE_BITWISENOT"] = "false";
            blob.Parameters["USE_ROI"] = "true";
            blob.Parameters["USE_MULTI_ROI"] = "false";
            blob.Parameters["CvROI"] = "20,20,540,530";
            blob.Parameters["MIN_AREA"] = "700";
            blob.Parameters["MAX_AREA"] = "2500";
            pipeline.Steps.Add(blob);
            return pipeline;
        }

        private static VisionPipeline CreatePortfolioPinArrayPipeline()
        {
            int[] centers = { 89, 161, 234, 305, 376, 448, 521, 591, 659 };
            int[] tips = { 98, 115, 133, 149, 165, 182, 202, 217, 233 };
            int[] bases = { 226, 241, 256, 272, 287, 303, 320, 335, 351 };
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Pin_Array_LineDistance" };
            for (int index = 0; index < centers.Length; index++)
            {
                string pinName = "P" + (index + 1).ToString(CultureInfo.InvariantCulture);
                LineGaugeProperty top = CreatePortfolioPinLengthLineProperty(
                    "Portfolio_" + pinName + "_Tip",
                    new OpenCvSharp.Rect(centers[index] - 16, tips[index] - 60, 50, 75),
                    PROJECTION_POLARITY.BTOW);
                LineGaugeProperty bottom = CreatePortfolioPinLengthLineProperty(
                    "Portfolio_" + pinName + "_Base",
                    new OpenCvSharp.Rect(centers[index] - 16, bases[index] - 52, 50, 80),
                    PROJECTION_POLARITY.WTOB);
                VisionPipelineStep step = VisionPipelineStepBuilder.FromLineGaugePair(
                    (index + 1).ToString("D2", CultureInfo.InvariantCulture) + " Pin " + pinName,
                    "LineDistance",
                    top,
                    bottom,
                    "Main",
                    "Pin_" + pinName + "_Result",
                    "Measure");
                step.UseAcceptance = true;
                step.ExpectedSuccess = true;
                step.AcceptanceMetricName = VisionPipelineKnownMetrics.DistancePxAvg;
                step.UseAcceptanceMetricMinimum = true;
                step.AcceptanceMetricMinimum = 100D;
                step.UseAcceptanceMetricMaximum = true;
                step.AcceptanceMetricMaximum = 135D;
                step.MaxElapsedMilliseconds = 1000;
                pipeline.Steps.Add(step);
            }

            return pipeline;
        }

        private static void RunPortfolioExtendedCaptures(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string edgeImagePath = ResolveRequiredOption(args, "--edge-image");
            string edgeTemplatePath = ResolveRequiredOption(args, "--edge-template");
            string intersectionImagePath = ResolveRequiredOption(args, "--intersection-image");
            string contourImagePath = ResolveRequiredOption(args, "--contour-image");
            EnsureFileExists(edgeImagePath, "Portfolio EdgeBasedMatching image");
            EnsureFileExists(edgeTemplatePath, "Portfolio EdgeBasedMatching template");
            EnsureFileExists(intersectionImagePath, "Portfolio line-intersection image");
            EnsureFileExists(contourImagePath, "Portfolio Contour image");

            VisionPipeline edgePipeline = CreatePortfolioEdgeBasedMatchingPipeline(edgeTemplatePath);
            VisionPipeline intersectionPipeline = CreatePortfolioLineIntersectionPipeline();
            VisionPipeline contourPipeline = CreatePortfolioContourPipeline();
            SerializeHelper.SaveXmlFile(Path.Combine(outputDirectory, "Portfolio_Card_EdgeBasedMatching.xml"), edgePipeline);
            SerializeHelper.SaveXmlFile(Path.Combine(outputDirectory, "Portfolio_Card_LineIntersection.xml"), intersectionPipeline);
            SerializeHelper.SaveXmlFile(Path.Combine(outputDirectory, "Portfolio_Target_Threshold_Morphology_Contour.xml"), contourPipeline);

            double edgeScore = double.NaN;
            double intersectionCount = SavePortfolioPipelineStage(
                intersectionPipeline,
                1,
                intersectionImagePath,
                Path.Combine(outputDirectory, "line_intersection_result.png"),
                VisionPipelineKnownMetrics.ResultCount);
            double intersectionX = SavePortfolioPipelineStage(
                intersectionPipeline,
                1,
                intersectionImagePath,
                Path.Combine(outputDirectory, "line_intersection_overlay.png"),
                VisionPipelineKnownMetrics.IntersectionX,
                renderRuntimeOverlays: true);
            SavePortfolioPipelineStage(contourPipeline, 1, contourImagePath, Path.Combine(outputDirectory, "Target_Contour_Binary.png"));
            SavePortfolioPipelineStage(contourPipeline, 2, contourImagePath, Path.Combine(outputDirectory, "Target_Contour_Clean.png"));
            double contourCount = SavePortfolioPipelineStage(
                contourPipeline,
                3,
                contourImagePath,
                Path.Combine(outputDirectory, "contour_result_overlay.png"),
                VisionPipelineKnownMetrics.ResultCount,
                renderRuntimeOverlays: true);
            if (intersectionCount != 1D || intersectionX < 420D || intersectionX > 480D || contourCount != 16D)
            {
                throw new InvalidOperationException(
                    $"Portfolio extended runtime qualification failed. Intersection={intersectionCount:0.###}@{intersectionX:0.###}, Contour={contourCount:0.###}.");
            }

            string recipeName = "Smoke_PortfolioExtended_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            VisionPipelineStorage.Save(recipeName, contourPipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, contourPipeline.Name);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
            OpenVisionShellHostWindow window = null;
            string reportPath = Path.Combine(outputDirectory, "report.txt");
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);
                window.Activate();
                Pump(40);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(36);

                if (!shellHost.LoadMainImageFromFileForTest(edgeImagePath))
                {
                    throw new InvalidOperationException("Portfolio EdgeBasedMatching image could not be loaded.");
                }
                shellHost.SelectToolForTest(VISION_MENU.EdgeBasedMatching);
                Pump(70);
                shellHost.SetActiveEdgeBasedMatchingTemplatePathForTest(edgeTemplatePath);
                shellHost.ConfigureActiveEdgeBasedMatchingForTest(
                    property => ConfigurePortfolioEdgeBasedMatchingProperty(property, edgeTemplatePath));
                Pump(30);
                shellHost.RunActiveNativePreviewForTest();
                Pump(160);
                string edgeReview = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || edgeReview.IndexOf("Edge Match", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "Portfolio EdgeBasedMatching Tool View did not produce the qualified result. Review=" + edgeReview);
                }
                Match edgeScoreMatch = Regex.Match(edgeReview, @"Score(?:Max)?\s*[:=]?\s*(?<score>[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
                if (edgeScoreMatch.Success)
                {
                    edgeScore = double.Parse(edgeScoreMatch.Groups["score"].Value, CultureInfo.InvariantCulture);
                }
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "07_edge_based_matching_tool.png"));
                using (Bitmap edgePreview = shellHost.GetLayerImageCloneForTest("EdgeBasedMatching_Preview"))
                {
                    edgePreview?.Save(Path.Combine(outputDirectory, "edge_based_live_result.png"));
                }
                using (Bitmap templateBitmap = new Bitmap(edgeTemplatePath))
                {
                    if (!shellHost.AddLayerImageForTest("Edge_Registered_Template", templateBitmap))
                    {
                        throw new InvalidOperationException("Portfolio EdgeBasedMatching template layer could not be published.");
                    }
                }
                shellHost.CloseActiveWpfToolWindowForTest();
                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Edge_Registered_Template", "EdgeBasedMatching_Preview" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio EdgeBasedMatching comparison could not dock " + layer + ".");
                    }
                }
                if (!shellHost.ArrangeDockedLayerGridForTest("Main", "Edge_Registered_Template", "EdgeBasedMatching_Preview"))
                {
                    throw new InvalidOperationException("Portfolio EdgeBasedMatching comparison grid could not be arranged.");
                }
                Pump(90);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "08_edge_based_three_panel_comparison.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(intersectionImagePath))
                {
                    throw new InvalidOperationException("Portfolio line-intersection image could not be loaded.");
                }
                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(70);
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveSelectedLineRoiForTest(180, 5, 220, 70);
                shellHost.ConfigureActiveSelectedLineForTest("Y_TTOB", "BTOW", "Y_BTOT");
                shellHost.ConfigureActiveSelectedLineThresholdForTest(150D, false);
                shellHost.ConfigureActiveSelectedLineMeasureTuningForTest(true, false, 20D, 5D, 10D, 10, false, 0D);
                shellHost.ConfigureActiveSelectedLineDrawForTest(false, true, false, true);
                shellHost.SetActiveLineSettingForTest("Line B");
                shellHost.SetActiveSelectedLineRoiForTest(410, 80, 90, 230);
                shellHost.ConfigureActiveSelectedLineForTest("X_LTOR", "WTOB", "X_LTOR");
                shellHost.ConfigureActiveSelectedLineThresholdForTest(150D, false);
                shellHost.ConfigureActiveSelectedLineMeasureTuningForTest(true, false, 20D, 5D, 10D, 10, false, 0D);
                shellHost.ConfigureActiveSelectedLineDrawForTest(false, true, false, true);
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveLinePurposeForTest("Intersection");
                Pump(30);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);
                string intersectionReview = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || intersectionReview.IndexOf("Intersection", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "Portfolio Line intersection Tool View did not produce the qualified result. Review=" + intersectionReview);
                }
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "09_line_intersection_tool.png"));
                using (Bitmap intersectionPreview = shellHost.GetLayerImageCloneForTest("Line_Preview"))
                {
                    intersectionPreview?.Save(Path.Combine(outputDirectory, "line_intersection_live_result.png"));
                }
                shellHost.CloseActiveWpfToolWindowForTest();
                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Line_Preview" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio Line intersection comparison could not dock " + layer + ".");
                    }
                }
                if (!shellHost.ArrangeDockedLayerPanesForTest("Horizontal", "Main", "Line_Preview"))
                {
                    throw new InvalidOperationException("Portfolio Line intersection comparison panes could not be arranged.");
                }
                Pump(80);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "10_line_intersection_before_after.png"));

                shellHost.ClearDockedLayersForTest();
                if (!shellHost.LoadMainImageFromFileForTest(contourImagePath))
                {
                    throw new InvalidOperationException("Portfolio Contour image could not be loaded.");
                }
                shellHost.OpenSamplePipelineForTest();
                Pump(70);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "portfolio Contour pipeline review");
                shellHost.SelectPipelineReviewStepForTest(2, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(90);
                if (shellHost.PipelineReviewStepCount != 3
                    || shellHost.PipelineReviewObjectResultCountForTest != 16
                    || !shellHost.PipelineReviewExecutionState.StartsWith("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Portfolio Contour pipeline did not retain 16 reviewed contours. "
                        + $"Steps={shellHost.PipelineReviewStepCount}, Count={shellHost.PipelineReviewObjectResultCountForTest}, State={shellHost.PipelineReviewExecutionState}.");
                }
                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "11_contour_pipeline_review.png"));
                shellHost.SelectToolForTest(VISION_MENU.Contour);
                Pump(50);
                shellHost.CloseActiveWpfToolWindowForTest();
                foreach (KeyValuePair<string, string> layer in new Dictionary<string, string>
                {
                    ["Target_Contour_Binary"] = Path.Combine(outputDirectory, "Target_Contour_Binary.png"),
                    ["Target_Contour_Clean"] = Path.Combine(outputDirectory, "Target_Contour_Clean.png"),
                    ["Target_Contour_Result"] = Path.Combine(outputDirectory, "contour_result_overlay.png")
                })
                {
                    using Bitmap layerImage = new Bitmap(layer.Value);
                    if (!shellHost.AddLayerImageForTest(layer.Key, layerImage)
                        && !shellHost.SetLayerImageForTest(layer.Key, layerImage))
                    {
                        throw new InvalidOperationException("Portfolio Contour comparison layer could not be published: " + layer.Key);
                    }
                }
                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Target_Contour_Binary", "Target_Contour_Clean", "Target_Contour_Result" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Portfolio Contour comparison could not dock " + layer + ".");
                    }
                }
                if (!shellHost.ArrangeDockedLayerGridForTest("Main", "Target_Contour_Binary", "Target_Contour_Clean", "Target_Contour_Result"))
                {
                    throw new InvalidOperationException("Portfolio Contour comparison grid could not be arranged.");
                }
                Pump(90);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "12_contour_four_panel_comparison.png"));

                File.WriteAllText(
                    reportPath,
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: portfolio-extended-captures" + Environment.NewLine
                    + "Executable: " + (Environment.ProcessPath ?? string.Empty) + Environment.NewLine
                    + "ExecutableSha256: " + ComputeC9FileSha256(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location) + Environment.NewLine
                    + monitorEvidence + Environment.NewLine
                    + "EdgeImage: " + edgeImagePath + Environment.NewLine
                    + "EdgeTemplate: " + edgeTemplatePath + Environment.NewLine
                    + "EdgeScore: " + edgeScore.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "EdgeReview: " + edgeReview.Replace(Environment.NewLine, " | ") + Environment.NewLine
                    + "IntersectionImage: " + intersectionImagePath + Environment.NewLine
                    + "IntersectionCount: " + intersectionCount.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "IntersectionX: " + intersectionX.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ContourImage: " + contourImagePath + Environment.NewLine
                    + "ContourCount: " + contourCount.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Boundary: current selected-image actual-EXE evidence; not calibrated metrology or field robustness.",
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }
                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunPortfolioLineIntersectionProbe(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string imagePath = ResolveRequiredOption(args, "--intersection-image");
            EnsureFileExists(imagePath, "Portfolio line-intersection image");

            VisionPipeline pipeline = CreatePortfolioLineIntersectionPipeline();
            string pipelinePath = Path.Combine(outputDirectory, "Portfolio_Card_LineIntersection.xml");
            if (!SerializeHelper.SaveXmlFile(pipelinePath, pipeline))
            {
                throw new InvalidOperationException("Portfolio line-intersection pipeline XML could not be saved.");
            }

            double resultCount = SavePortfolioPipelineStage(
                pipeline,
                1,
                imagePath,
                Path.Combine(outputDirectory, "line_intersection_result.png"),
                VisionPipelineKnownMetrics.ResultCount);
            double intersectionX = SavePortfolioPipelineStage(
                pipeline,
                1,
                imagePath,
                Path.Combine(outputDirectory, "line_intersection_overlay.png"),
                VisionPipelineKnownMetrics.IntersectionX,
                renderRuntimeOverlays: true);

            using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged);
            using VisionRecipeRunResult result = new VisionRecipeRunner()
                .RunAsync(pipeline, source, VisionRecipeRunner.DefaultInputLayer, 5000, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            double intersectionY = ReadRecipeMetric(result, VisionPipelineKnownMetrics.IntersectionY);
            if (resultCount != 1D
                || intersectionX < 420D
                || intersectionX > 480D
                || intersectionY < 0D
                || intersectionY > 60D)
            {
                throw new InvalidOperationException(
                    "Portfolio line intersection did not land on the card top-right corner. "
                    + $"Count={resultCount:0.###}, Point={intersectionX:0.###},{intersectionY:0.###}.");
            }

            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: PASS" + Environment.NewLine
                + "Scenario: portfolio-line-intersection-probe" + Environment.NewLine
                + "Image: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "Pipeline: " + pipelinePath + Environment.NewLine
                + "IntersectionX: " + intersectionX.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "IntersectionY: " + intersectionY.ToString("0.###", CultureInfo.InvariantCulture),
                Encoding.UTF8);
        }

        private static VisionPipeline CreatePortfolioLineIntersectionPipeline()
        {
            LineGaugeProperty horizontal = CreatePortfolioIntersectionLineProperty(
                "Portfolio_Card_Top_Edge",
                new OpenCvSharp.Rect(180, 5, 220, 70),
                PROJECTION_DIR.Y_TTOB,
                PROJECTION_POLARITY.BTOW);
            LineGaugeProperty vertical = CreatePortfolioIntersectionLineProperty(
                "Portfolio_Card_Right_Edge",
                new OpenCvSharp.Rect(410, 80, 90, 230),
                PROJECTION_DIR.X_LTOR,
                PROJECTION_POLARITY.WTOB);
            VisionPipelineStep step = VisionPipelineStepBuilder.FromLineGaugePair(
                "01 Card Top-Right Intersection",
                "LineIntersection",
                horizontal,
                vertical,
                "Main",
                "Card_Intersection_Result",
                "Intersection");
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Card_LineIntersection" };
            pipeline.Steps.Add(step);
            return pipeline;
        }

        private static LineGaugeProperty CreatePortfolioIntersectionLineProperty(
            string name,
            OpenCvSharp.Rect roi,
            PROJECTION_DIR direction,
            PROJECTION_POLARITY polarity)
        {
            return new LineGaugeProperty(name)
            {
                PIXELPERMM = 0D,
                USE_THRESHOLD = true,
                USE_ADAPTIVE_THRESHOLD = false,
                USE_BITWISENOT = false,
                THRESHOLD = 150D,
                USE_ROI = true,
                USE_MULTI_ROI = false,
                CvROI = roi,
                PRJ_PORALITY = polarity,
                PRJ_DIR = direction,
                CONTRAST = 20D,
                THICKNESS = 5D,
                SAMPLING_STEP = 10D,
                VER_PRJ_DIR = direction == PROJECTION_DIR.Y_TTOB
                    ? PROJECTION_DIR.Y_BTOT
                    : direction,
                POINT_RANGE = 10,
                USE_MANUAL_ANGLE = false,
                USE_EXTEND_FIT_LINE = true,
                EXTEND_FIT_LINE_VALUE = 600,
                SHOW_VERTICAL_LINE = false,
                SHOW_EDGE = true,
                SHOW_CONTOUR = false,
                SHOW_FITLINE = true
            };
        }

        private static void RunPortfolioContourProbe(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string imagePath = ResolveRequiredOption(args, "--contour-image");
            EnsureFileExists(imagePath, "Portfolio Contour image");
            VisionPipeline pipeline = CreatePortfolioContourPipeline();
            string pipelinePath = Path.Combine(outputDirectory, "Portfolio_Target_Threshold_Morphology_Contour.xml");
            if (!SerializeHelper.SaveXmlFile(pipelinePath, pipeline))
            {
                throw new InvalidOperationException("Portfolio Contour pipeline XML could not be saved.");
            }

            SavePortfolioPipelineStage(pipeline, 1, imagePath, Path.Combine(outputDirectory, "Target_Contour_Binary.png"));
            SavePortfolioPipelineStage(pipeline, 2, imagePath, Path.Combine(outputDirectory, "Target_Contour_Clean.png"));
            double contourCount = SavePortfolioPipelineStage(
                pipeline,
                3,
                imagePath,
                Path.Combine(outputDirectory, "contour_result_overlay.png"),
                VisionPipelineKnownMetrics.ResultCount,
                renderRuntimeOverlays: true);
            if (contourCount != 16D)
            {
                throw new InvalidOperationException(
                    "Portfolio Contour runtime did not outline 16 target objects. Count="
                    + contourCount.ToString("0.###", CultureInfo.InvariantCulture));
            }

            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: PASS" + Environment.NewLine
                + "Scenario: portfolio-contour-probe" + Environment.NewLine
                + "Image: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "Pipeline: " + pipelinePath + Environment.NewLine
                + "ContourCount: " + contourCount.ToString("0.###", CultureInfo.InvariantCulture),
                Encoding.UTF8);
        }

        private static VisionPipeline CreatePortfolioContourPipeline()
        {
            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Target_Threshold_Morphology_Contour" };
            VisionPipelineStep threshold = new VisionPipelineStep
            {
                Name = "01 Target Binary",
                ToolType = "Threshold",
                InputLayer = "Main",
                OutputLayer = "Target_Contour_Binary"
            };
            threshold.Parameters["Mode"] = "Threshold";
            threshold.Parameters["Threshold"] = "150";
            threshold.Parameters["MaxValue"] = "255";
            threshold.Parameters["ThresholdType"] = "Binary";
            pipeline.Steps.Add(threshold);

            VisionPipelineStep morphology = new VisionPipelineStep
            {
                Name = "02 Target Cleanup",
                ToolType = "Morphology",
                InputLayer = "Target_Contour_Binary",
                OutputLayer = "Target_Contour_Clean"
            };
            morphology.Parameters["Shape"] = "Ellipse";
            morphology.Parameters["Operator"] = "Open";
            morphology.Parameters["KernelWidth"] = "3";
            morphology.Parameters["KernelHeight"] = "3";
            morphology.Parameters["Iterations"] = "1";
            pipeline.Steps.Add(morphology);

            VisionPipelineStep contour = new VisionPipelineStep
            {
                Name = "03 Target Contours",
                ToolType = "Contour",
                InputLayer = "Target_Contour_Clean",
                OutputLayer = "Target_Contour_Result"
            };
            contour.Parameters["Name"] = "Portfolio_Target_Contours";
            contour.Parameters["PIXELPERMM"] = "0";
            contour.Parameters["USE_THRESHOLD"] = "false";
            contour.Parameters["USE_ADAPTIVE_THRESHOLD"] = "false";
            contour.Parameters["USE_BITWISENOT"] = "false";
            contour.Parameters["USE_ROI"] = "true";
            contour.Parameters["USE_MULTI_ROI"] = "false";
            contour.Parameters["USE_DRAW_IMAGE"] = "false";
            contour.Parameters["CvROI"] = "20,20,540,530";
            contour.Parameters["DrawMode"] = "Outline";
            contour.Parameters["ApproximationModes"] = "ApproxSimple";
            contour.Parameters["DetectMode"] = "External";
            contour.Parameters["MIN_AREA"] = "700";
            contour.Parameters["MAX_AREA"] = "2500";
            contour.Parameters["ClrGridHtml"] = "#00ff88";
            contour.Parameters["DrawThickness"] = "3";
            pipeline.Steps.Add(contour);
            return pipeline;
        }

        private static void RunPortfolioPinArrayProbe(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string imagePath = ResolveRequiredOption(args, "--line-image");
            EnsureFileExists(imagePath, "Portfolio pin-array image");
            VisionPipeline pipeline = CreatePortfolioPinArrayPipeline();
            string pipelinePath = Path.Combine(outputDirectory, "Portfolio_Pin_Array_LineDistance.xml");
            if (!SerializeHelper.SaveXmlFile(pipelinePath, pipeline))
            {
                throw new InvalidOperationException("Portfolio pin-array pipeline XML could not be saved.");
            }

            using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged);
            using VisionRecipeRunResult result = new VisionRecipeRunner()
                .RunAsync(pipeline, source, VisionRecipeRunner.DefaultInputLayer, 20000, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            if (!result.Success || result.Steps == null || result.Steps.Count != pipeline.Steps.Count)
            {
                throw new InvalidOperationException(
                    "Portfolio pin-array runtime failed. "
                    + string.Join(" | ", result.Steps?.Select(step => step.Name + ":" + step.Message)
                        ?? Enumerable.Empty<string>()));
            }

            List<double> distances = new List<double>();
            List<int> scanCounts = new List<int>();
            StringBuilder csv = new StringBuilder("Pin,DistancePxAvg,ScanCount,TipRoi,BaseRoi" + Environment.NewLine);
            for (int index = 0; index < result.Steps.Count; index++)
            {
                VisionRecipeStepRunSummary summary = result.Steps[index];
                List<VisionRecipeOverlaySummary> distanceLines = summary.Overlays?
                    .Where(overlay => string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                        && Regex.IsMatch(overlay.Label ?? string.Empty, @"^D[0-9]+$", RegexOptions.IgnoreCase))
                    .ToList() ?? new List<VisionRecipeOverlaySummary>();
                if (!summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxAvg, out double distance)
                    || distanceLines.Count < 3)
                {
                    throw new InvalidOperationException(
                        "Portfolio pin-array step did not return an average and three runtime scan lines. Step="
                        + pipeline.Steps[index].Name);
                }

                distances.Add(distance);
                scanCounts.Add(distanceLines.Count);
                VisionPipelineStep step = pipeline.Steps[index];
                csv.Append("P").Append(index + 1).Append(',')
                    .Append(distance.ToString("0.###", CultureInfo.InvariantCulture)).Append(',')
                    .Append(distanceLines.Count.ToString(CultureInfo.InvariantCulture)).Append(',')
                    .Append('"').Append(step.Parameters.TryGetValue("LeftCvROI", out string tipRoi) ? tipRoi : string.Empty).Append('"').Append(',')
                    .Append('"').Append(step.Parameters.TryGetValue("RightCvROI", out string baseRoi) ? baseRoi : string.Empty).Append('"')
                    .AppendLine();
            }

            string overlayPath = Path.Combine(outputDirectory, "pin_array_runtime_overlay.png");
            SavePortfolioPinArrayOverlay(source, pipeline, result.Steps, distances, overlayPath);
            File.WriteAllText(Path.Combine(outputDirectory, "pin_measurements.csv"), csv.ToString(), Encoding.UTF8);
            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: PASS" + Environment.NewLine
                + "Scenario: portfolio-pin-array-probe" + Environment.NewLine
                + "Image: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "Pipeline: " + pipelinePath + Environment.NewLine
                + "PinCount: " + distances.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "ScansPerPinMinimum: " + scanCounts.Min().ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "ScansPerPinMaximum: " + scanCounts.Max().ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "AveragePx: " + distances.Average().ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "MinimumPx: " + distances.Min().ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "MaximumPx: " + distances.Max().ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "RangePx: " + (distances.Max() - distances.Min()).ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "Calibration: Not calibrated; pixel-only evidence",
                Encoding.UTF8);
        }

        private static void RunPortfolioGapRecipeProbe(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string imagePath = ResolveRequiredOption(args, "--gap-image");
            string recipeRoot = ResolveRequiredOption(args, "--gap-recipe-root");
            EnsureFileExists(imagePath, "Gap recipe image");
            VisionPipeline pipeline = CreatePortfolioGapRecipePipeline(
                recipeRoot,
                out string leftPath,
                out string rightPath,
                out LineGaugeProperty left,
                out LineGaugeProperty right);
            string pipelinePath = Path.Combine(outputDirectory, "Portfolio_Gap_ExactRecipe.xml");
            if (!SerializeHelper.SaveXmlFile(pipelinePath, pipeline))
            {
                throw new InvalidOperationException("Exact Gap recipe pipeline XML could not be saved.");
            }

            using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged);
            using VisionRecipeRunResult result = new VisionRecipeRunner()
                .RunAsync(pipeline, source, VisionRecipeRunner.DefaultInputLayer, 10000, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            VisionRecipeStepRunSummary summary = result.Steps?.SingleOrDefault();
            if (!result.Success || summary == null)
            {
                throw new InvalidOperationException(
                    "Exact Gap recipe runtime failed. "
                    + string.Join(" | ", result.Steps?.Select(step => step.Name + ":" + step.Message)
                        ?? Enumerable.Empty<string>()));
            }

            List<VisionRecipeOverlaySummary> distanceLines = summary.Overlays?
                .Where(overlay => string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                    && Regex.IsMatch(overlay.Label ?? string.Empty, @"^D[0-9]+$", RegexOptions.IgnoreCase))
                .ToList() ?? new List<VisionRecipeOverlaySummary>();
            if (!summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxAvg, out double distancePx)
                || !summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistanceMmAvg, out double distanceMm)
                || !summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxRange, out double distanceRangePx)
                || distanceLines.Count < 3)
            {
                throw new InvalidOperationException("Exact Gap recipe did not return calibrated multi-scan distance evidence.");
            }

            using OpenCvSharp.Mat snapshot = new OpenCvSharp.Mat();
            if (source.Channels() == 1)
            {
                OpenCvSharp.Cv2.CvtColor(source, snapshot, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                source.CopyTo(snapshot);
            }

            OpenCvSharp.Cv2.ImWrite(Path.Combine(outputDirectory, "gap_source_snapshot.png"), snapshot);
            using Bitmap overlay = BitmapImageConverter.ToBitmap(snapshot);
            VisionPipelineRunReportImageRenderer.RenderInPlace(overlay, summary, pipeline.Steps[0]);
            overlay.Save(Path.Combine(outputDirectory, "gap_exact_runtime_overlay.png"), System.Drawing.Imaging.ImageFormat.Png);
            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                "Result: PASS" + Environment.NewLine
                + "Scenario: portfolio-gap-recipe-probe" + Environment.NewLine
                + "RecipeRoot: " + recipeRoot + Environment.NewLine
                + "Image: " + imagePath + Environment.NewLine
                + "ImageSha256: " + ComputeC9FileSha256(imagePath) + Environment.NewLine
                + "LeftState: " + leftPath + Environment.NewLine
                + "LeftStateSha256: " + ComputeC9FileSha256(leftPath) + Environment.NewLine
                + "RightState: " + rightPath + Environment.NewLine
                + "RightStateSha256: " + ComputeC9FileSha256(rightPath) + Environment.NewLine
                + "LeftRoi: " + left.CvROI + Environment.NewLine
                + "RightRoi: " + right.CvROI + Environment.NewLine
                + "DistanceScans: " + distanceLines.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "DistancePxAvg: " + distancePx.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "DistancePxRange: " + distanceRangePx.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "DistanceMmAvg: " + distanceMm.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                + "CalibrationMmPerPixel: " + left.PIXELPERMM.ToString("0.######", CultureInfo.InvariantCulture),
                Encoding.UTF8);
        }

        private static VisionPipeline CreatePortfolioGapRecipePipeline(
            string recipeRoot,
            out string leftPath,
            out string rightPath,
            out LineGaugeProperty left,
            out LineGaugeProperty right)
        {
            leftPath = Path.Combine(recipeRoot, "VISION", "Line(L)_1.xml");
            rightPath = Path.Combine(recipeRoot, "VISION", "Line(R)_1.xml");
            EnsureFileExists(leftPath, "Gap recipe left line state");
            EnsureFileExists(rightPath, "Gap recipe right line state");
            if (!SerializeHelper.TryLoadFromXmlFile(leftPath, out left) || left == null)
            {
                throw new InvalidOperationException("Gap recipe left line state could not be loaded: " + leftPath);
            }

            if (!SerializeHelper.TryLoadFromXmlFile(rightPath, out right) || right == null)
            {
                throw new InvalidOperationException("Gap recipe right line state could not be loaded: " + rightPath);
            }

            VisionPipeline pipeline = new VisionPipeline { Name = "Portfolio_Gap_ExactRecipe" };
            pipeline.Steps.Add(VisionPipelineStepBuilder.FromLineGaugePair(
                "01 Gap between adjacent pins",
                "LineDistance",
                left,
                right,
                "Main",
                "Gap_Result",
                "Measure"));
            return pipeline;
        }

        private static void RunPortfolioGapMatchingCorrectionCaptures(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string gapImagePath = ResolveRequiredOption(args, "--gap-image");
            string gapRecipeRoot = ResolveRequiredOption(args, "--gap-recipe-root");
            string matchingImagePath = ResolveRequiredOption(args, "--matching-image");
            string matchingTemplatePath = ResolveRequiredOption(args, "--matching-template");
            EnsureFileExists(gapImagePath, "Gap recipe image");
            EnsureFileExists(matchingImagePath, "Corner Matching image");
            EnsureFileExists(matchingTemplatePath, "Corner Matching template");

            VisionPipeline gapPipeline = CreatePortfolioGapRecipePipeline(
                gapRecipeRoot,
                out string leftPath,
                out string rightPath,
                out LineGaugeProperty left,
                out LineGaugeProperty right);
            VisionPipeline matchingPipeline = CreatePortfolioImageMatchingPipeline(matchingTemplatePath);
            double gapPx = SavePortfolioPipelineStage(
                gapPipeline,
                1,
                gapImagePath,
                Path.Combine(outputDirectory, "gap_runtime_overlay.png"),
                VisionPipelineKnownMetrics.DistancePxAvg,
                renderRuntimeOverlays: true);
            double gapMm = SavePortfolioPipelineStage(
                gapPipeline,
                1,
                gapImagePath,
                Path.Combine(outputDirectory, "gap_runtime_result.png"),
                VisionPipelineKnownMetrics.DistanceMmAvg);
            double matchingPipelineScore = SavePortfolioPipelineStage(
                matchingPipeline,
                1,
                matchingImagePath,
                Path.Combine(outputDirectory, "matching_runtime_overlay.png"),
                VisionPipelineKnownMetrics.ScoreMax,
                renderRuntimeOverlays: true);
            if (matchingPipelineScore < 80D)
            {
                throw new InvalidOperationException(
                    "Corner Matching runtime score was below the accepted minimum. Score="
                    + matchingPipelineScore.ToString("0.###", CultureInfo.InvariantCulture));
            }

            const string recipeName = "Gap";
            VisionPipelineStorage.Save(recipeName, gapPipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, gapPipeline.Name);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            OpenVisionShellHostWindow window = null;
            string reportPath = Path.Combine(outputDirectory, "report.txt");
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                string monitorEvidence = PlaceWindowOnLeftmostMonitor(window);
                window.Activate();
                Pump(40);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(36);
                if (!shellHost.LoadMainImageFromFileForTest(gapImagePath))
                {
                    throw new InvalidOperationException("Exact Gap image could not be loaded: " + gapImagePath);
                }

                shellHost.OpenSamplePipelineForTest();
                Pump(70);
                WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), "exact Gap pipeline review");
                shellHost.SelectPipelineReviewStepForTest(0, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
                Pump(90);
                string gapState = shellHost.PipelineReviewExecutionState;
                string gapSummary = shellHost.PipelineReviewResultSummaryText;
                string gapDetail = shellHost.PipelineReviewResultDetailText;
                if (shellHost.PipelineReviewStepCount != 1
                    || !shellHost.HasPipelineReviewInputPreview
                    || !shellHost.HasPipelineReviewOutputPreview
                    || (!gapState.StartsWith("Completed", StringComparison.OrdinalIgnoreCase)
                        && !gapState.StartsWith("완료", StringComparison.OrdinalIgnoreCase))
                    || !gapSummary.StartsWith("OK", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Exact Gap Pipeline Review did not show the completed result. "
                        + $"Steps={shellHost.PipelineReviewStepCount}, State='{gapState}', Summary='{gapSummary}', Detail='{gapDetail}'");
                }

                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "01_gap_exact_pipeline_review.png"));

                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(50);
                shellHost.CloseActiveWpfToolWindowForTest();
                using (Bitmap gapOverlay = new Bitmap(Path.Combine(outputDirectory, "gap_runtime_overlay.png")))
                {
                    if (!shellHost.AddLayerImageForTest("Gap_Runtime_Overlay", gapOverlay)
                        && !shellHost.SetLayerImageForTest("Gap_Runtime_Overlay", gapOverlay))
                    {
                        throw new InvalidOperationException("Exact Gap runtime overlay layer could not be published.");
                    }
                }

                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Gap_Runtime_Overlay" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Exact Gap comparison could not dock " + layer + ".");
                    }
                }

                if (!shellHost.ArrangeDockedLayerPanesForTest("Horizontal", "Main", "Gap_Runtime_Overlay"))
                {
                    throw new InvalidOperationException("Exact Gap comparison panes could not be arranged.");
                }

                Pump(80);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01b_gap_exact_two_panel.png"));

                if (!shellHost.LoadMainImageFromFileForTest(matchingImagePath))
                {
                    throw new InvalidOperationException("Corner Matching image could not be loaded: " + matchingImagePath);
                }

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(70);
                int matchingRunsBefore = shellHost.NativePreviewRunCount;
                shellHost.SetActiveMatchingTemplatePathForTest(matchingTemplatePath);
                shellHost.ConfigureActiveMatchingForTest(property =>
                {
                    property.AUTO_PREVIEW = false;
                    property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
                    property.SCORE_MIN = 0.8D;
                    property.NUM_MATCH = 1;
                    property.MAGNIFIATION = 1D;
                    property.USE_FIND_ANGLE = true;
                    property.FIND_ANGLE_MIN = -100;
                    property.FIND_ANGLE_MAX = 100;
                    property.FIND_ANGLE = 1D;
                    property.USE_COARSE_TO_FINE_ANGLE_SEARCH = true;
                    property.COARSE_ANGLE_STEP = 5D;
                    property.COARSE_ANGLE_TOP_K = 5;
                    property.USE_FIND_SCALE = true;
                    property.FIND_SCALE_MIN = 0.9D;
                    property.FIND_SCALE_MAX = 1.1D;
                    property.FIND_SCALE_STEP = 0.05D;
                    property.USE_CANNY = false;
                    property.USE_PADDING_COLOR_WHITE = false;
                    property.USE_THRESHOLD = false;
                    property.USE_ADAPTIVE_THRESHOLD = false;
                    property.USE_ROI = false;
                    property.ReloadTemplateImage();
                });
                Pump(36);
                if (shellHost.NativePreviewRunCount != matchingRunsBefore)
                {
                    throw new InvalidOperationException("Corner Matching setup ran Preview before the explicit action.");
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(150);
                string matchingStatus = shellHost.ActiveNativeStatusText;
                string matchingReview = shellHost.ActiveNativeResultReviewText;
                if (!TryParseMatchingC9Review(
                        matchingReview,
                        out int matchingCount,
                        out double matchingScore,
                        out _, out _, out _, out _, out _, out _, out _)
                    || matchingCount != 1
                    || matchingScore < 80D
                    || !shellHost.HasNativePreviewResult
                    || shellHost.NativePreviewRunCount != matchingRunsBefore + 1
                    || Math.Abs(matchingPipelineScore - matchingScore) > 0.5D)
                {
                    throw new InvalidOperationException(
                        "Corner Matching Tool View did not agree with the qualified runtime. "
                        + $"Count={matchingCount}, ToolScore={matchingScore:0.###}, PipelineScore={matchingPipelineScore:0.###}, Status='{matchingStatus}', Review='{matchingReview}'");
                }

                SaveWindowScreenScreenshot(window, Path.Combine(outputDirectory, "02_corner_image_matching_tool.png"));
                using (Bitmap templateBitmap = new Bitmap(matchingTemplatePath))
                {
                    if (!shellHost.AddLayerImageForTest("Corner_Template", templateBitmap))
                    {
                        throw new InvalidOperationException("Corner Matching template layer could not be created.");
                    }
                }

                shellHost.CloseActiveWpfToolWindowForTest();
                shellHost.ClearDockedLayersForTest();
                foreach (string layer in new[] { "Main", "Corner_Template", "Matching_Preview" })
                {
                    if (!shellHost.DockLayerForTest(layer))
                    {
                        throw new InvalidOperationException("Corner Matching comparison could not dock " + layer + ".");
                    }
                }

                if (!shellHost.ArrangeDockedLayerGridForTest("Main", "Corner_Template", "Matching_Preview"))
                {
                    throw new InvalidOperationException("Corner Matching comparison grid could not be arranged.");
                }

                Pump(80);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "03_corner_matching_three_panel.png"));
                File.WriteAllText(
                    reportPath,
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: portfolio-gap-matching-correction-captures" + Environment.NewLine
                    + "Executable: " + (Environment.ProcessPath ?? string.Empty) + Environment.NewLine
                    + "ExecutableSha256: " + ComputeC9FileSha256(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location) + Environment.NewLine
                    + "ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location + Environment.NewLine
                    + "ManagedAssemblySha256: " + ComputeC9FileSha256(typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location) + Environment.NewLine
                    + monitorEvidence + Environment.NewLine
                    + "GapImage: " + gapImagePath + Environment.NewLine
                    + "GapImageSha256: " + ComputeC9FileSha256(gapImagePath) + Environment.NewLine
                    + "GapLeftState: " + leftPath + Environment.NewLine
                    + "GapLeftStateSha256: " + ComputeC9FileSha256(leftPath) + Environment.NewLine
                    + "GapRightState: " + rightPath + Environment.NewLine
                    + "GapRightStateSha256: " + ComputeC9FileSha256(rightPath) + Environment.NewLine
                    + "GapLeftRoi: " + left.CvROI + Environment.NewLine
                    + "GapRightRoi: " + right.CvROI + Environment.NewLine
                    + "GapDistancePxAvg: " + gapPx.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "GapDistanceMmAvg: " + gapMm.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "GapPipelineReviewState: " + gapState + Environment.NewLine
                    + "GapPipelineReviewSummary: " + gapSummary + Environment.NewLine
                    + "GapPipelineReviewDetail: " + gapDetail + Environment.NewLine
                    + "MatchingImage: " + matchingImagePath + Environment.NewLine
                    + "MatchingImageSha256: " + ComputeC9FileSha256(matchingImagePath) + Environment.NewLine
                    + "MatchingTemplate: " + matchingTemplatePath + Environment.NewLine
                    + "MatchingTemplateSha256: " + ComputeC9FileSha256(matchingTemplatePath) + Environment.NewLine
                    + "MatchingScoreMax: " + matchingScore.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "MatchingAcceptance: ScoreMax >= 80" + Environment.NewLine
                    + "MatchingStatus: " + matchingStatus + Environment.NewLine
                    + "MatchingReview: " + matchingReview,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void SavePortfolioPinArrayOverlay(
            OpenCvSharp.Mat source,
            VisionPipeline pipeline,
            IReadOnlyList<VisionRecipeStepRunSummary> summaries,
            IReadOnlyList<double> distances,
            string outputPath)
        {
            using OpenCvSharp.Mat color = new OpenCvSharp.Mat();
            if (source.Channels() == 1)
            {
                OpenCvSharp.Cv2.CvtColor(source, color, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                source.CopyTo(color);
            }

            using Bitmap bitmap = BitmapImageConverter.ToBitmap(color);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using System.Drawing.Pen tipPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1.5F);
            using System.Drawing.Pen basePen = new System.Drawing.Pen(System.Drawing.Color.Magenta, 1.5F);
            using System.Drawing.Pen scanPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 235, 120), 2F);
            using System.Drawing.Pen centerScanPen = new System.Drawing.Pen(System.Drawing.Color.Yellow, 2.5F);
            using Font pinFont = new Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
            using Font headerFont = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
            using SolidBrush textBrush = new SolidBrush(System.Drawing.Color.White);
            using SolidBrush labelBrush = new SolidBrush(System.Drawing.Color.FromArgb(225, 0, 110, 70));
            using SolidBrush headerBrush = new SolidBrush(System.Drawing.Color.FromArgb(235, 0, 118, 74));

            for (int index = 0; index < pipeline.Steps.Count; index++)
            {
                VisionPipelineStep step = pipeline.Steps[index];
                DrawPortfolioLineRoi(graphics, pinFont, step.Parameters, "LeftCvROI", string.Empty, System.Drawing.Color.Cyan);
                DrawPortfolioLineRoi(graphics, pinFont, step.Parameters, "RightCvROI", string.Empty, System.Drawing.Color.Magenta);
                List<VisionRecipeOverlaySummary> lines = summaries[index].Overlays
                    .Where(overlay => string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                        && Regex.IsMatch(overlay.Label ?? string.Empty, @"^D[0-9]+$", RegexOptions.IgnoreCase))
                    .ToList();
                for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                {
                    VisionRecipeOverlaySummary line = lines[lineIndex];
                    graphics.DrawLine(
                        lineIndex == lines.Count / 2 ? centerScanPen : scanPen,
                        line.StartX,
                        line.StartY,
                        line.EndX,
                        line.EndY);
                }

                ParsePortfolioRoi(step.Parameters, "LeftCvROI", out int roiX, out int roiY, out _, out _);
                string label = "P" + (index + 1).ToString(CultureInfo.InvariantCulture)
                    + " " + distances[index].ToString("0.0", CultureInfo.InvariantCulture) + " px";
                SizeF labelSize = graphics.MeasureString(label, pinFont);
                float labelX = Math.Max(0F, Math.Min(bitmap.Width - labelSize.Width - 6F, roiX - 3F));
                float labelY = Math.Max(26F, roiY - labelSize.Height - 4F);
                graphics.FillRectangle(labelBrush, labelX, labelY, labelSize.Width + 6F, labelSize.Height + 2F);
                graphics.DrawString(label, pinFont, textBrush, labelX + 3F, labelY + 1F);
            }

            string header = "OK | PIN LENGTH ARRAY | "
                + pipeline.Steps.Count.ToString(CultureInfo.InvariantCulture)
                + " pins x 3+ scans | Avg "
                + distances.Average().ToString("0.0", CultureInfo.InvariantCulture)
                + " px | Range "
                + (distances.Max() - distances.Min()).ToString("0.0", CultureInfo.InvariantCulture)
                + " px";
            SizeF headerSize = graphics.MeasureString(header, headerFont);
            graphics.FillRectangle(headerBrush, 6F, 6F, headerSize.Width + 14F, headerSize.Height + 6F);
            graphics.DrawString(header, headerFont, textBrush, 13F, 9F);
            bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static bool ParsePortfolioRoi(
            IReadOnlyDictionary<string, string> parameters,
            string key,
            out int x,
            out int y,
            out int width,
            out int height)
        {
            x = y = width = height = 0;
            if (parameters == null || !parameters.TryGetValue(key, out string text))
            {
                return false;
            }

            string[] values = (text ?? string.Empty).Split(',');
            return values.Length == 4
                && int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out x)
                && int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out y)
                && int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out width)
                && int.TryParse(values[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out height);
        }

        private static LineGaugeProperty CreatePortfolioPinLengthLineProperty(
            string name,
            OpenCvSharp.Rect roi,
            PROJECTION_POLARITY polarity)
        {
            return new LineGaugeProperty(name)
            {
                PIXELPERMM = 0D,
                USE_THRESHOLD = false,
                USE_ADAPTIVE_THRESHOLD = false,
                USE_BITWISENOT = false,
                USE_ROI = true,
                USE_MULTI_ROI = false,
                CvROI = roi,
                PRJ_PORALITY = polarity,
                PRJ_DIR = PROJECTION_DIR.Y_TTOB,
                CONTRAST = 20D,
                THICKNESS = 8D,
                SAMPLING_STEP = 4D,
                VER_PRJ_DIR = PROJECTION_DIR.Y_TTOB,
                POINT_RANGE = 8,
                USE_MANUAL_ANGLE = true,
                MANUAL_ANGLE_VALUE = 0D,
                USE_EXTEND_FIT_LINE = true,
                EXTEND_FIT_LINE_VALUE = 300,
                SHOW_VERTICAL_LINE = false,
                SHOW_EDGE = true,
                SHOW_CONTOUR = false,
                SHOW_FITLINE = false
            };
        }

        private static double SavePortfolioPipelineStage(
            VisionPipeline sourcePipeline,
            int stepCount,
            string imagePath,
            string outputPath,
            string metricName = null,
            bool renderRuntimeOverlays = false,
            bool renderRuntimeOverlaysWhenAvailable = false,
            bool suppressRuntimeOverlayLabels = false,
            bool distanceLinesOnly = false)
        {
            VisionPipeline stage = new VisionPipeline
            {
                Name = sourcePipeline.Name + "_Stage_" + stepCount.ToString(CultureInfo.InvariantCulture)
            };
            foreach (VisionPipelineStep step in sourcePipeline.Steps.Take(stepCount))
            {
                stage.Steps.Add(step);
            }

            using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(imagePath, OpenCvSharp.ImreadModes.Unchanged);
            using VisionRecipeRunResult result = new VisionRecipeRunner()
                .RunAsync(stage, source, VisionRecipeRunner.DefaultInputLayer, 5000, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            bool expectedAcceptanceNg = result.FirstFailedStep?.ToolSuccess == true
                && result.FirstFailedStep.AcceptancePassed == false
                && result.FirstFailedStep.ErrorCode == 0;
            if (!result.Success && !expectedAcceptanceNg)
            {
                throw new InvalidOperationException(
                    "Portfolio pipeline stage failed. "
                    + $"Stage={stepCount}, Steps={string.Join(" | ", result.Steps.Select(step => step.Name + ":" + step.Message))}");
            }

            if (renderRuntimeOverlays || renderRuntimeOverlaysWhenAvailable)
            {
                VisionRecipeStepRunSummary summary = result.Steps?
                    .LastOrDefault(step => step?.Overlays != null && step.Overlays.Count > 0);
                if (summary == null)
                {
                    if (renderRuntimeOverlaysWhenAvailable)
                    {
                        SaveRecipeResultImage(result, outputPath);
                        return string.IsNullOrWhiteSpace(metricName) ? double.NaN : ReadRecipeMetric(result, metricName);
                    }

                    throw new InvalidOperationException("Portfolio pipeline stage returned no runtime overlays.");
                }

                int summaryIndex = summary.Index - 1;
                VisionPipelineStep pipelineStep = summaryIndex >= 0 && summaryIndex < stage.Steps.Count
                    ? stage.Steps[summaryIndex]
                    : stage.Steps.Last();
                VisionPipelineStep configuredStep = pipelineStep;
                if (distanceLinesOnly)
                {
                    List<VisionRecipeOverlaySummary> distanceOverlays = summary.Overlays
                        .Where(overlay => string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                            && Regex.IsMatch(overlay.Label ?? string.Empty, @"^D[0-9]+$", RegexOptions.IgnoreCase))
                        .ToList();
                    if (distanceOverlays.Count == 0
                        || !summary.Metrics.TryGetValue(VisionPipelineKnownMetrics.DistancePxAvg, out double distanceAverage))
                    {
                        throw new InvalidOperationException("Portfolio LineDistance runtime did not return labeled distance-line evidence.");
                    }

                    summary.Overlays = distanceOverlays;
                    pipelineStep = new VisionPipelineStep
                    {
                        Name = "Pin Length | Avg "
                            + distanceAverage.ToString("0.###", CultureInfo.InvariantCulture)
                            + " px | "
                            + distanceOverlays.Count.ToString(CultureInfo.InvariantCulture)
                            + " scans",
                        ToolType = pipelineStep.ToolType,
                        InputLayer = pipelineStep.InputLayer,
                        OutputLayer = pipelineStep.OutputLayer
                    };
                }

                if (suppressRuntimeOverlayLabels)
                {
                    foreach (VisionRecipeOverlaySummary overlay in summary.Overlays)
                    {
                        overlay.Label = string.Empty;
                    }
                }

                using OpenCvSharp.Mat overlaySource = new OpenCvSharp.Mat();
                if (source.Channels() == 1)
                {
                    OpenCvSharp.Cv2.CvtColor(source, overlaySource, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
                }
                else
                {
                    source.CopyTo(overlaySource);
                }

                using Bitmap overlayBitmap = BitmapImageConverter.ToBitmap(overlaySource);
                VisionPipelineRunReportImageRenderer.RenderInPlace(overlayBitmap, summary, pipelineStep);
                if (distanceLinesOnly)
                {
                    DrawPortfolioLinePairRois(overlayBitmap, configuredStep);
                }
                overlayBitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                SaveRecipeResultImage(result, outputPath);
            }

            return string.IsNullOrWhiteSpace(metricName) ? double.NaN : ReadRecipeMetric(result, metricName);
        }

        private static void DrawPortfolioLinePairRois(Bitmap bitmap, VisionPipelineStep step)
        {
            if (bitmap == null || step?.Parameters == null)
            {
                return;
            }

            using Graphics graphics = Graphics.FromImage(bitmap);
            using Font font = new Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            DrawPortfolioLineRoi(graphics, font, step.Parameters, "LeftCvROI", "TIP ROI", System.Drawing.Color.Cyan);
            DrawPortfolioLineRoi(graphics, font, step.Parameters, "RightCvROI", "BASE ROI", System.Drawing.Color.Magenta);
        }

        private static void DrawPortfolioLineRoi(
            Graphics graphics,
            Font font,
            IReadOnlyDictionary<string, string> parameters,
            string key,
            string label,
            System.Drawing.Color color)
        {
            if (graphics == null
                || font == null
                || parameters == null
                || !parameters.TryGetValue(key, out string text))
            {
                return;
            }

            string[] values = (text ?? string.Empty).Split(',');
            if (values.Length != 4
                || !int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x)
                || !int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y)
                || !int.TryParse(values[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width)
                || !int.TryParse(values[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height)
                || width <= 0
                || height <= 0)
            {
                return;
            }

            using System.Drawing.Pen pen = new System.Drawing.Pen(color, 2F);
            using SolidBrush brush = new SolidBrush(color);
            graphics.DrawRectangle(pen, x, y, width, height);
            graphics.DrawString(label, font, brush, x + 2, Math.Max(0, y - 18));
        }

        private static void RunRecipePipelinePersistenceFeedback(
            string[] args,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            CleanupKnownSmokeRecipeWorkspaces("Smoke_RecipePersistence_");
            bool expectProtection = ResolveOptionalBoolOption(
                args,
                "--expect-protection",
                false);
            string matrixEvidence = expectProtection
                ? VerifyRecipePersistenceMatrix(
                    outputDirectory)
                : "NotApplicable";
            string recipeName =
                "Smoke_RecipePersistence_"
                + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Persistence_Probe";
            string pipelinePath =
                RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    pipelineName);

            VisionPipelineStorage.Save(
                recipeName,
                CreateDirectSmokePipeline(pipelineName, 2));
            VisionPipelineStorage.SaveActivePipelineName(
                recipeName,
                pipelineName);
            byte[] validBytes = File.ReadAllBytes(pipelinePath);
            File.WriteAllText(
                pipelinePath,
                "<VisionPipeline",
                Encoding.UTF8);
            byte[] invalidBytes =
                File.ReadAllBytes(pipelinePath);
            string reopenProbeEvidence = "NotApplicable";
            if (expectProtection)
            {
                string reopenProbeDirectory =
                    Path.Combine(
                        outputDirectory,
                        "cross_process_reopen_probe");
                Directory.CreateDirectory(
                    reopenProbeDirectory);
                ProcessStartInfo startInfo =
                    new ProcessStartInfo
                    {
                        FileName =
                            Environment.ProcessPath
                            ?? throw new InvalidOperationException(
                                "Current executable path is unavailable."),
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                startInfo.ArgumentList.Add("--smoke");
                startInfo.ArgumentList.Add(
                    "recipe-persistence-reopen-probe");
                startInfo.ArgumentList.Add("--recipe-name");
                startInfo.ArgumentList.Add(recipeName);
                startInfo.ArgumentList.Add("--pipeline-name");
                startInfo.ArgumentList.Add(pipelineName);
                startInfo.ArgumentList.Add("--output");
                startInfo.ArgumentList.Add(
                    reopenProbeDirectory);
                using Process reopenProbe =
                    Process.Start(startInfo)
                    ?? throw new InvalidOperationException(
                        "Could not start the cross-process reopen probe.");
                if (!reopenProbe.WaitForExit(30000))
                {
                    reopenProbe.Kill(
                        entireProcessTree: true);
                    throw new TimeoutException(
                        "Cross-process reopen probe timed out.");
                }

                string reopenReportPath =
                    Path.Combine(
                        reopenProbeDirectory,
                        "report.txt");
                reopenProbeEvidence =
                    File.Exists(reopenReportPath)
                        ? File.ReadAllText(
                            reopenReportPath,
                            Encoding.UTF8)
                        : string.Empty;
                if (reopenProbe.ExitCode != 0
                    || !reopenProbeEvidence.StartsWith(
                        "Result: PASS",
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Cross-process reopen probe failed. "
                        + reopenProbeEvidence);
                }
            }

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(
                OpenVisionLanguage.Korean,
                false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext =
                    ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation =
                        WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost =
                    window.ShellHostForSmoke
                    ?? throw new InvalidOperationException(
                        "Recipe persistence feedback smoke did not create the shell host.");
                int previewRunsBefore =
                    shellHost.NativePreviewRunCount;
                int layerCountBefore =
                    shellHost.LayerDocumentCount;
                string activeLayerBefore =
                    shellHost.ActiveHostLayerTitle;
                string recipeLayerBefore =
                    shellHost.ActiveRecipeContextLayerNameForTest;

                shellHost.SwitchRecipeContextForTest(recipeName);
                Pump(48);

                string workspaceDirectory =
                    Path.GetDirectoryName(pipelinePath)
                    ?? string.Empty;
                string invalidBackupPath =
                    Directory.GetFiles(
                        workspaceDirectory,
                        pipelineName + ".invalid-*.xml")
                    .SingleOrDefault();
                if (string.IsNullOrWhiteSpace(invalidBackupPath)
                    || !File.Exists(invalidBackupPath)
                    || !File.ReadAllBytes(invalidBackupPath)
                        .SequenceEqual(invalidBytes)
                    || !File.ReadAllBytes(pipelinePath)
                        .SequenceEqual(invalidBytes))
                {
                    throw new InvalidOperationException(
                        "Recipe persistence feedback smoke did not retain the malformed pipeline backup.");
                }

                VisionPipeline substituted =
                    VisionPipelineStorage.Load(
                        recipeName,
                        pipelineName);
                OpenVisionRecipePipelineOption option =
                    shellHost.RecipeCommands.PipelineOptions
                        .FirstOrDefault(item => string.Equals(
                            item.PipelineName,
                            pipelineName,
                            StringComparison.OrdinalIgnoreCase));
                bool appearsAsOrdinaryXml =
                    option != null
                    && option.XmlValid
                    && substituted?.Steps?.Count == 0;
                bool protectionVisible =
                    option != null
                    && (option.StatusText.Contains(
                            "\uBCF5\uC6D0",
                            StringComparison.Ordinal)
                        || option.StatusText.Contains(
                            "restore",
                            StringComparison.OrdinalIgnoreCase)
                        || option.StatusText.Contains(
                            "\uC190\uC0C1",
                            StringComparison.Ordinal)
                        || option.StatusText.Contains(
                            "damaged",
                            StringComparison.OrdinalIgnoreCase)
                        || option.StatusText.Contains(
                            "could not be loaded",
                            StringComparison.OrdinalIgnoreCase));
                if (protectionVisible != expectProtection)
                {
                    throw new InvalidOperationException(
                        "Recipe persistence protection did not match the expected state. "
                        + "Expected="
                        + expectProtection
                        + ", Option='"
                        + option?.StatusText
                        + "', Steps="
                        + (substituted?.Steps?.Count ?? -1)
                            .ToString(CultureInfo.InvariantCulture)
                        + ".");
                }
                if (!expectProtection && !appearsAsOrdinaryXml)
                {
                    throw new InvalidOperationException(
                        "Current-source baseline no longer reproduces the silent default-substitution defect. "
                        + "Option='"
                        + option?.StatusText
                        + "', XmlValid="
                        + option?.XmlValid
                        + ", RouteValid="
                        + option?.RouteValid
                        + ", Steps="
                        + (substituted?.Steps?.Count ?? -1)
                            .ToString(CultureInfo.InvariantCulture)
                        + ".");
                }

                System.Windows.Controls.Primitives.ToggleButton
                    recipeManagerButton =
                        FindNamedVisualChild<
                            System.Windows.Controls.Primitives.ToggleButton>(
                            shellHost,
                            "btnHostRecipeManager",
                            "Recipe persistence feedback");
                recipeManagerButton.IsChecked = true;
                Pump(60);
                string screenshotName = expectProtection
                    ? "OpenVisionLab_Recipe_Persistence_Protected_After.png"
                    : "OpenVisionLab_Recipe_Persistence_Silent_Before.png";
                SaveWindowScreenScreenshot(
                    window,
                    Path.Combine(
                        outputDirectory,
                        screenshotName));

                string recoveryStatus = "NotApplicable";
                string ordinarySaveStatus = "NotApplicable";
                string englishFailureStatus = "NotApplicable";
                string directSaveFailureStatus = "NotApplicable";
                string directSaveRecoveryState = "NotApplicable";
                if (expectProtection)
                {
                    if (!shellHost.RecipeCommands
                            .HasSelectedRecipePersistenceFailure
                        || shellHost.RecipeCommands
                            .SelectedRecipeSummary.XmlValid
                        || shellHost.RecipeCommands
                            .RunSelectedSampleCheckCommand
                            .CanExecute(null))
                    {
                        throw new InvalidOperationException(
                            "Recipe persistence failure did not fail closed for summary and explicit sample Run.");
                    }

                    Border persistenceBorder =
                        FindVisualChildren<Border>(window)
                            .FirstOrDefault(item =>
                                item.IsVisible
                                && string.Equals(
                                    System.Windows.Automation
                                        .AutomationProperties
                                        .GetAutomationId(item),
                                    "HostRecipePersistenceStatus",
                                    StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Recipe persistence status border was not visible.");
                    TextBlock persistenceText =
                        FindVisualChildren<TextBlock>(
                            persistenceBorder)
                            .FirstOrDefault(item =>
                                item.IsVisible
                                && string.Equals(
                                    item.Text,
                                    shellHost.RecipeCommands
                                        .SelectedRecipePersistenceStatusText,
                                    StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Recipe persistence status text was not visible.");
                    if (!string.Equals(
                            persistenceText.ToolTip?.ToString(),
                            shellHost.RecipeCommands
                                .SelectedRecipePersistenceHelpText,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            System.Windows.Automation
                                .AutomationProperties
                                .GetHelpText(persistenceText),
                            shellHost.RecipeCommands
                                .SelectedRecipePersistenceHelpText,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Recipe persistence full cause/path was not available through tooltip and accessibility HelpText.");
                    }

                    OpenVisionLanguageService.SetLanguage(
                        OpenVisionLanguage.English,
                        false);
                    shellHost.SwitchRecipeContextForTest(
                        "Default");
                    shellHost.SwitchRecipeContextForTest(
                        recipeName);
                    Pump(48);
                    englishFailureStatus =
                        shellHost.RecipeCommands
                            .SelectedRecipePersistenceStatusText;
                    if (!englishFailureStatus.Contains(
                            "damaged",
                            StringComparison.OrdinalIgnoreCase)
                        || !shellHost.RecipeCommands
                            .SelectedRecipePersistenceHelpText
                            .Contains(
                                pipelinePath,
                                StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Recipe persistence failure was not localized with the full saved path. Status='"
                            + englishFailureStatus
                            + "'.");
                    }
                    SaveWindowScreenScreenshot(
                        window,
                        Path.Combine(
                            outputDirectory,
                            "OpenVisionLab_Recipe_Persistence_Protected_After_EN.png"));
                    OpenVisionLanguageService.SetLanguage(
                        OpenVisionLanguage.Korean,
                        false);
                    shellHost.SwitchRecipeContextForTest(
                        "Default");
                    shellHost.SwitchRecipeContextForTest(
                        recipeName);
                    Pump(48);

                    VisionPipelineStorage.Save(
                        recipeName,
                        substituted);
                    shellHost.SwitchRecipeContextForTest(
                        "Default");
                    shellHost.SwitchRecipeContextForTest(
                        recipeName);
                    Pump(48);
                    recoveryStatus =
                        shellHost.RecipeCommands
                            .SelectedRecipePersistenceStatusText;
                    if (shellHost.RecipeCommands
                            .HasSelectedRecipePersistenceFailure
                        || !recoveryStatus.Contains(
                            "\uBCF5\uAD6C",
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Recipe persistence successful save did not expose one recovery state. Status='"
                            + recoveryStatus
                            + "'.");
                    }

                    VisionPipelineStorage.Save(
                        recipeName,
                        substituted);
                    shellHost.SwitchRecipeContextForTest(
                        "Default");
                    shellHost.SwitchRecipeContextForTest(
                        recipeName);
                    Pump(48);
                    ordinarySaveStatus =
                        shellHost.RecipeCommands
                            .SelectedRecipePersistenceStatusText;
                    if (shellHost.RecipeCommands
                            .HasSelectedRecipePersistenceStatus
                        || !string.IsNullOrWhiteSpace(
                            ordinarySaveStatus))
                    {
                        throw new InvalidOperationException(
                            "Recipe persistence recovery notice repeated after an ordinary successful save. Status='"
                            + ordinarySaveStatus
                            + "'.");
                    }

                    recipeManagerButton.IsChecked = false;
                    shellHost.SelectToolForTest(
                        VISION_MENU.Blob);
                    Pump(48);
                    int directRunsBefore =
                        shellHost.NativePreviewRunCount;
                    int directLayersBefore =
                        shellHost.LayerDocumentCount;
                    string directActiveLayerBefore =
                        shellHost.ActiveHostLayerTitle;
                    string directInputRouteBefore =
                        shellHost
                            .ActiveNativeRouteInputLayerNameForTest;
                    string directOutputRouteBefore =
                        shellHost
                            .ActiveNativeRouteOutputLayerNameForTest;
                    string directDiskHashBefore =
                        ComputeC9FileSha256(pipelinePath);
                    VisionPipelineStep failedAdd;
                    using (FileStream lockedPipeline =
                        new FileStream(
                            pipelinePath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read))
                    {
                        failedAdd =
                            shellHost
                                .AddActiveNativePipelineStepForTest();
                    }
                    Pump(24);
                    directSaveFailureStatus =
                        shellHost.ActiveNativeStatusText;
                    if (failedAdd != null
                        || !directSaveFailureStatus.Contains(
                            "\uBA54\uBAA8\uB9AC",
                            StringComparison.Ordinal)
                        || !directSaveFailureStatus.Contains(
                            "\uC190\uC2E4",
                            StringComparison.Ordinal)
                        || !string.Equals(
                            ComputeC9FileSha256(pipelinePath),
                            directDiskHashBefore,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Direct teaching save failure did not distinguish memory and disk state. Status='"
                            + directSaveFailureStatus
                            + "'.");
                    }

                    VisionPipelineStep recoveredAdd =
                        shellHost
                            .AddActiveNativePipelineStepForTest();
                    Pump(24);
                    if (recoveredAdd == null
                        || !VisionPipelineStorage
                            .TryGetPersistenceState(
                                recipeName,
                                pipelineName,
                                out VisionPipelinePersistenceState
                                    directRecovered)
                        || directRecovered.Kind
                            != VisionPipelinePersistenceStateKind
                                .SaveRecovered
                        || VisionPipelineStorage.Load(
                                recipeName,
                                pipelineName)
                            .Steps.Count != 1)
                    {
                        throw new InvalidOperationException(
                            "Direct teaching retry did not recover and persist exactly one Step.");
                    }
                    directSaveRecoveryState =
                        directRecovered.Kind.ToString();
                    if (shellHost.NativePreviewRunCount
                            != directRunsBefore
                        || shellHost.LayerDocumentCount
                            != directLayersBefore
                        || !string.Equals(
                            shellHost.ActiveHostLayerTitle,
                            directActiveLayerBefore,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            shellHost
                                .ActiveNativeRouteInputLayerNameForTest,
                            directInputRouteBefore,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            shellHost
                                .ActiveNativeRouteOutputLayerNameForTest,
                            directOutputRouteBefore,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Direct teaching save failure/recovery changed Preview/Run, layers, active layer, or routes.");
                    }
                }

                if (shellHost.NativePreviewRunCount
                        != previewRunsBefore
                    || shellHost.LayerDocumentCount
                        != layerCountBefore
                    || !string.Equals(
                        shellHost.ActiveHostLayerTitle,
                        activeLayerBefore,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        shellHost.ActiveRecipeContextLayerNameForTest,
                        recipeLayerBefore,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe persistence feedback changed Preview/Run, layers, active layer, or routing.");
                }

                File.WriteAllText(
                    Path.Combine(
                        outputDirectory,
                        "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: recipe-pipeline-persistence-feedback"
                    + Environment.NewLine
                    + "RuntimeExe: "
                    + Process.GetCurrentProcess().MainModule?.FileName
                    + Environment.NewLine
                    + "ExpectedProtection: "
                    + expectProtection
                    + Environment.NewLine
                    + "MatrixEvidence: "
                    + matrixEvidence
                    + Environment.NewLine
                    + "CrossProcessReopenProbe: "
                    + (reopenProbeEvidence
                        .Split(
                            new[]
                            {
                                "\r\n",
                                "\n"
                            },
                            StringSplitOptions.None)
                        .FirstOrDefault()
                        ?? string.Empty)
                    + Environment.NewLine
                    + "OriginalValidBytes: "
                    + validBytes.Length.ToString(
                        CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "InvalidBackup: "
                    + invalidBackupPath
                    + Environment.NewLine
                    + "InvalidBackupExact: True"
                    + Environment.NewLine
                    + "CanonicalInvalidRetainedUntilExplicitSave: True"
                    + Environment.NewLine
                    + "SubstitutedStepCount: "
                    + (substituted?.Steps?.Count ?? -1)
                        .ToString(CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "PipelineStatus: "
                    + option?.StatusText
                    + Environment.NewLine
                    + "XmlValid: "
                    + option?.XmlValid
                    + Environment.NewLine
                    + "RouteValid: "
                    + option?.RouteValid
                    + Environment.NewLine
                    + "RecoveryStatus: "
                    + recoveryStatus
                    + Environment.NewLine
                    + "OrdinarySaveStatus: "
                    + ordinarySaveStatus
                    + Environment.NewLine
                    + "EnglishFailureStatus: "
                    + englishFailureStatus
                    + Environment.NewLine
                    + "DirectSaveFailureStatus: "
                    + directSaveFailureStatus
                    + Environment.NewLine
                    + "DirectSaveRecoveryState: "
                    + directSaveRecoveryState
                    + Environment.NewLine
                    + "PreviewRunCount: "
                    + shellHost.NativePreviewRunCount.ToString(
                        CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "LayerCount: "
                    + shellHost.LayerDocumentCount.ToString(
                        CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "Screenshot: "
                    + screenshotName
                    + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                window?.Close();
                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(
                    recipeName);
            }
        }

        private static void RunRecipePersistenceReopenProbe(
            string[] args,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string recipeName =
                ResolveOptionalTextOption(
                    args,
                    "--recipe-name");
            string pipelineName =
                ResolveOptionalTextOption(
                    args,
                    "--pipeline-name");
            if (string.IsNullOrWhiteSpace(recipeName)
                || string.IsNullOrWhiteSpace(pipelineName))
            {
                throw new ArgumentException(
                    "Cross-process reopen probe requires recipe and pipeline names.");
            }

            string pipelinePath =
                RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    pipelineName);
            string canonicalHash =
                ComputeC9FileSha256(pipelinePath);
            VisionPipeline loaded =
                VisionPipelineStorage.Load(
                    recipeName,
                    pipelineName);
            string backupPath =
                Directory.GetFiles(
                    Path.GetDirectoryName(pipelinePath)
                        ?? string.Empty,
                    pipelineName + ".invalid-*.xml")
                    .Single();
            if (loaded.Steps.Count != 0
                || !VisionPipelineStorage
                    .TryGetPersistenceState(
                        recipeName,
                        pipelineName,
                        out VisionPipelinePersistenceState state)
                || state.Kind
                    != VisionPipelinePersistenceStateKind
                        .InvalidFileSubstituted
                || !string.Equals(
                    ComputeC9FileSha256(pipelinePath),
                    canonicalHash,
                    StringComparison.Ordinal)
                || !string.Equals(
                    ComputeC9FileSha256(backupPath),
                    canonicalHash,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Cross-process reopen did not retain the invalid canonical file, exact backup, memory default, and failure state.");
            }

            File.WriteAllText(
                Path.Combine(
                    outputDirectory,
                    "report.txt"),
                "Result: PASS"
                + Environment.NewLine
                + "Scenario: recipe-persistence-reopen-probe"
                + Environment.NewLine
                + "Recipe: "
                + recipeName
                + Environment.NewLine
                + "Pipeline: "
                + pipelineName
                + Environment.NewLine
                + "CanonicalHash: "
                + canonicalHash
                + Environment.NewLine
                + "Backup: "
                + backupPath
                + Environment.NewLine,
                Encoding.UTF8);
        }

        private static string VerifyRecipePersistenceMatrix(
            string outputDirectory)
        {
            string prefix =
                "Smoke_RecipePersistence_Matrix_"
                + Guid.NewGuid().ToString("N").Substring(0, 8);
            List<string> recipes = new List<string>();
            List<string> rows = new List<string>
            {
                "ID\tPipeline\tRecipeData\tResult\tEvidence"
            };

            string NewRecipe(string id)
            {
                string recipe = prefix + "_" + id;
                recipes.Add(recipe);
                return recipe;
            }

            void Require(bool condition, string message)
            {
                if (!condition)
                {
                    throw new InvalidOperationException(
                        "P272 matrix: " + message);
                }
            }

            string FindBackup(string path)
            {
                return Directory.GetFiles(
                    Path.GetDirectoryName(path) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(path)
                        + ".invalid-*"
                        + Path.GetExtension(path))
                    .Single();
            }

            void VerifyInvalid(
                string id,
                string pipelineXml,
                string dataXml)
            {
                string recipe = NewRecipe(id);
                string pipelineName = id + "_Pipeline";
                string pipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        recipe,
                        pipelineName);
                string dataPath =
                    RecipeWorkspaceService.GetVisionDataPath(
                        recipe);
                Directory.CreateDirectory(
                    Path.GetDirectoryName(pipelinePath)
                    ?? string.Empty);
                File.WriteAllText(
                    pipelinePath,
                    pipelineXml,
                    Encoding.UTF8);
                File.WriteAllText(
                    dataPath,
                    dataXml,
                    Encoding.UTF8);
                string pipelineHash =
                    ComputeC9FileSha256(pipelinePath);
                string dataHash =
                    ComputeC9FileSha256(dataPath);
                VisionPipeline loadedPipeline =
                    VisionPipelineStorage.Load(
                        recipe,
                        pipelineName);
                DataState loadedData =
                    RecipeDataStorage.Load(
                        recipe,
                        new DataState());
                string pipelineBackup =
                    FindBackup(pipelinePath);
                string dataBackup =
                    FindBackup(dataPath);
                Require(
                    loadedPipeline.Steps.Count == 0
                    && loadedData != null
                    && string.Equals(
                        ComputeC9FileSha256(
                            pipelineBackup),
                        pipelineHash,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeC9FileSha256(pipelinePath),
                        pipelineHash,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeC9FileSha256(dataBackup),
                        dataHash,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeC9FileSha256(dataPath),
                        dataHash,
                        StringComparison.Ordinal)
                    && VisionPipelineStorage
                        .TryGetPersistenceState(
                            recipe,
                            pipelineName,
                            out VisionPipelinePersistenceState
                                pipelineState)
                    && pipelineState.Kind
                        == VisionPipelinePersistenceStateKind
                            .InvalidFileSubstituted
                    && RecipeDataStorage
                        .TryGetPersistenceState(
                            recipe,
                            out RecipeDataPersistenceState dataState)
                    && dataState.Kind
                        == RecipeDataPersistenceStateKind
                            .InvalidFileSubstituted,
                    id
                        + " did not retain exact invalid backups and explicit substitution state.");
                rows.Add(
                    id
                    + "\tExact backup; canonical invalid retained; memory default\tExact backup; canonical invalid retained; memory default\tPASS\t"
                    + pipelineHash
                    + " / "
                    + dataHash);
            }

            try
            {
                string r1 = NewRecipe("R1");
                const string r1PipelineName = "Missing_First_Use";
                VisionPipeline r1Pipeline =
                    VisionPipelineStorage.Load(
                        r1,
                        r1PipelineName);
                DataState r1Data =
                    RecipeDataStorage.Load(
                        r1,
                        new DataState());
                string r1PipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        r1,
                        r1PipelineName);
                string r1DataPath =
                    RecipeWorkspaceService.GetVisionDataPath(r1);
                Require(
                    r1Pipeline.Steps.Count == 0
                    && r1Data != null
                    && File.Exists(r1PipelinePath)
                    && File.Exists(r1DataPath)
                    && !VisionPipelineStorage
                        .TryGetPersistenceState(
                            r1,
                            r1PipelineName,
                            out _)
                    && !RecipeDataStorage
                        .TryGetPersistenceState(r1, out _),
                    "R1 first use was not quiet.");
                rows.Add(
                    "R1\tQuiet editable default\tQuiet empty CData\tPASS\t"
                    + ComputeC9FileSha256(r1PipelinePath)
                    + " / "
                    + ComputeC9FileSha256(r1DataPath));

                string r2 = NewRecipe("R2");
                const string r2PipelineName = "Valid_Roundtrip";
                VisionPipeline r2Source =
                    CreateDirectSmokePipeline(
                        r2PipelineName,
                        2);
                r2Source.Steps[0].Name =
                    "Distinctive_Source";
                r2Source.Steps[1].Name =
                    "Distinctive_Result";
                r2Source.Steps[1].OutputLayer =
                    "Distinctive_Output";
                VisionPipelineStorage.Save(r2, r2Source);
                RecipeDataStorage.Save(
                    r2,
                    new DataState());
                VisionPipeline r2Loaded =
                    VisionPipelineStorage.Load(
                        r2,
                        r2PipelineName);
                DataState r2Data =
                    RecipeDataStorage.Load(
                        r2,
                        new DataState());
                Require(
                    r2Loaded.Steps.Count == 2
                    && string.Equals(
                        r2Loaded.Steps[0].Name,
                        "Distinctive_Source",
                        StringComparison.Ordinal)
                    && string.Equals(
                        r2Loaded.Steps[1].OutputLayer,
                        "Distinctive_Output",
                        StringComparison.Ordinal)
                    && r2Data != null,
                    "R2 valid identity did not round-trip.");
                rows.Add(
                    "R2\tExact Step/order/route restored\tCData restored; current contract has no fields\tPASS\tValid round-trip");

                VerifyInvalid(
                    "R3",
                    "<VisionPipeline",
                    "<CData");
                VerifyInvalid(
                    "R4",
                    "<?xml version=\"1.0\"?><WrongPipeline />",
                    "<?xml version=\"1.0\"?><WrongData />");

                string r5 = NewRecipe("R5");
                const string r5PipelineName = "Unreadable";
                VisionPipelineStorage.Save(
                    r5,
                    CreateDirectSmokePipeline(
                        r5PipelineName,
                        2));
                RecipeDataStorage.Save(
                    r5,
                    new DataState());
                string r5PipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        r5,
                        r5PipelineName);
                string r5DataPath =
                    RecipeWorkspaceService.GetVisionDataPath(r5);
                string r5PipelineHash =
                    ComputeC9FileSha256(r5PipelinePath);
                string r5DataHash =
                    ComputeC9FileSha256(r5DataPath);
                using (FileStream lockedPipeline =
                    new FileStream(
                        r5PipelinePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.None))
                {
                    VisionPipeline loaded =
                        VisionPipelineStorage.Load(
                            r5,
                            r5PipelineName);
                    Require(
                        loaded.Steps.Count == 0
                        && VisionPipelineStorage
                            .TryGetPersistenceState(
                                r5,
                                r5PipelineName,
                                out VisionPipelinePersistenceState
                                    state)
                        && state.Kind
                            == VisionPipelinePersistenceStateKind
                                .LoadFailed,
                        "R5 Pipeline lock was not fail-closed.");
                }
                Require(
                    string.Equals(
                        ComputeC9FileSha256(r5PipelinePath),
                        r5PipelineHash,
                        StringComparison.Ordinal),
                    "R5 Pipeline disk identity changed.");
                using (FileStream lockedData =
                    new FileStream(
                        r5DataPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.None))
                {
                    DataState loaded =
                        RecipeDataStorage.Load(
                            r5,
                            new DataState());
                    Require(
                        loaded != null
                        && RecipeDataStorage
                            .TryGetPersistenceState(
                                r5,
                                out RecipeDataPersistenceState state)
                        && state.Kind
                            == RecipeDataPersistenceStateKind
                                .LoadFailed,
                        "R5 Data lock was not fail-closed.");
                }
                Require(
                    string.Equals(
                        ComputeC9FileSha256(r5DataPath),
                        r5DataHash,
                        StringComparison.Ordinal),
                    "R5 Recipe Data disk identity changed.");
                rows.Add(
                    "R5\tMemory default + LoadFailed; disk unchanged\tCurrent default + LoadFailed; disk unchanged\tPASS\t"
                    + r5PipelineHash
                    + " / "
                    + r5DataHash);

                string r6 = NewRecipe("R6");
                const string r6PipelineName = "Save_Exception";
                VisionPipelineStorage.Save(
                    r6,
                    CreateDirectSmokePipeline(
                        r6PipelineName,
                        1));
                RecipeDataStorage.Save(
                    r6,
                    new DataState());
                string r6PipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        r6,
                        r6PipelineName);
                string r6DataPath =
                    RecipeWorkspaceService.GetVisionDataPath(r6);
                string r6PipelineHash =
                    ComputeC9FileSha256(r6PipelinePath);
                string r6DataHash =
                    ComputeC9FileSha256(r6DataPath);
                bool r6PipelineFailed = false;
                using (FileStream lockedPipeline =
                    new FileStream(
                        r6PipelinePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read))
                {
                    try
                    {
                        VisionPipelineStorage.Save(
                            r6,
                            CreateDirectSmokePipeline(
                                r6PipelineName,
                                2));
                    }
                    catch (IOException)
                    {
                        r6PipelineFailed = true;
                    }
                }
                bool r6DataFailed = false;
                using (FileStream lockedData =
                    new FileStream(
                        r6DataPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read))
                {
                    try
                    {
                        RecipeDataStorage.Save(
                            r6,
                            new DataState());
                    }
                    catch (IOException)
                    {
                        r6DataFailed = true;
                    }
                }
                Require(
                    r6PipelineFailed
                    && r6DataFailed
                    && VisionPipelineStorage
                        .TryGetPersistenceState(
                            r6,
                            r6PipelineName,
                            out VisionPipelinePersistenceState
                                r6PipelineState)
                    && r6PipelineState.Kind
                        == VisionPipelinePersistenceStateKind
                            .SaveFailed
                    && RecipeDataStorage
                        .TryGetPersistenceState(
                            r6,
                            out RecipeDataPersistenceState
                                r6DataState)
                    && r6DataState.Kind
                        == RecipeDataPersistenceStateKind
                            .SaveFailed
                    && string.Equals(
                        ComputeC9FileSha256(r6PipelinePath),
                        r6PipelineHash,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeC9FileSha256(r6DataPath),
                        r6DataHash,
                        StringComparison.Ordinal),
                    "R6 save failure did not retain old disk identity.");
                rows.Add(
                    "R6\tSaveFailed; edit memory-only; old hash intact\tSaveFailed; old hash intact\tPASS\t"
                    + r6PipelineHash
                    + " / "
                    + r6DataHash);

                VerifyInvalid(
                    "R7",
                    "<?xml version=\"1.0\"?><VisionPipeline><Steps>",
                    "<?xml version=\"1.0\"?><CData>");

                string r8 = NewRecipe("R8");
                const string r8PipelineName = "Replace_Failure";
                VisionPipelineStorage.Save(
                    r8,
                    CreateDirectSmokePipeline(
                        r8PipelineName,
                        1));
                RecipeDataStorage.Save(
                    r8,
                    new DataState());
                string r8PipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        r8,
                        r8PipelineName);
                string r8DataPath =
                    RecipeWorkspaceService.GetVisionDataPath(r8);
                string r8PipelineHash =
                    ComputeC9FileSha256(r8PipelinePath);
                string r8DataHash =
                    ComputeC9FileSha256(r8DataPath);
                VisionPipeline r8Modified =
                    CreateDirectSmokePipeline(
                        r8PipelineName,
                        2);
                using (FileStream lockedPipeline =
                    new FileStream(
                        r8PipelinePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read))
                {
                    try
                    {
                        VisionPipelineStorage.Save(
                            r8,
                            r8Modified);
                    }
                    catch (IOException)
                    {
                    }
                }
                using (FileStream lockedData =
                    new FileStream(
                        r8DataPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read))
                {
                    try
                    {
                        RecipeDataStorage.Save(
                            r8,
                            new DataState());
                    }
                    catch (IOException)
                    {
                    }
                }
                Require(
                    string.Equals(
                        ComputeC9FileSha256(r8PipelinePath),
                        r8PipelineHash,
                        StringComparison.Ordinal)
                    && string.Equals(
                        ComputeC9FileSha256(r8DataPath),
                        r8DataHash,
                        StringComparison.Ordinal),
                    "R8 replace failure damaged the prior file.");
                rows.Add(
                    "R8\tFailed replace preserved prior file byte-exact\tFailed replace preserved prior file byte-exact\tPASS\t"
                    + r8PipelineHash
                    + " / "
                    + r8DataHash);

                VisionPipelineStorage.Save(r8, r8Modified);
                RecipeDataStorage.Save(
                    r8,
                    new DataState());
                Require(
                    VisionPipelineStorage
                        .TryGetPersistenceState(
                            r8,
                            r8PipelineName,
                            out VisionPipelinePersistenceState
                                recoveredPipeline)
                    && recoveredPipeline.Kind
                        == VisionPipelinePersistenceStateKind
                            .SaveRecovered
                    && RecipeDataStorage
                        .TryGetPersistenceState(
                            r8,
                            out RecipeDataPersistenceState
                                recoveredData)
                    && recoveredData.Kind
                        == RecipeDataPersistenceStateKind
                            .SaveRecovered
                    && VisionPipelineStorage.Load(
                            r8,
                            r8PipelineName)
                        .Steps.Count == 2,
                    "R9 recovery was not retained once.");
                VisionPipelineStorage.Save(r8, r8Modified);
                RecipeDataStorage.Save(
                    r8,
                    new DataState());
                Require(
                    !VisionPipelineStorage
                        .TryGetPersistenceState(
                            r8,
                            r8PipelineName,
                            out _)
                    && !RecipeDataStorage
                        .TryGetPersistenceState(r8, out _),
                    "R9 recovery repeated after ordinary save.");
                rows.Add(
                    "R9\tSaveRecovered once + exact two-Step reopen; next save cleared\tSaveRecovered once; next save cleared\tPASS\tOne-time recovery");

                string r10 = NewRecipe("R10");
                const string r10PipelineName = "Semantic_Boundary";
                VisionPipelineStorage.Save(
                    r10,
                    CreateDirectSmokePipeline(
                        r10PipelineName,
                        1));
                RecipeDataStorage.Save(
                    r10,
                    new DataState());
                string r10PipelinePath =
                    RecipeWorkspaceService.GetVisionPipelinePath(
                        r10,
                        r10PipelineName);
                string r10DataPath =
                    RecipeWorkspaceService.GetVisionDataPath(r10);
                File.WriteAllText(
                    r10PipelinePath,
                    File.ReadAllText(r10PipelinePath)
                        .Replace(
                            "</VisionPipeline>",
                            "<LegacyUnknown>1</LegacyUnknown></VisionPipeline>",
                            StringComparison.Ordinal),
                    Encoding.UTF8);
                File.WriteAllText(
                    r10DataPath,
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                    + "<CData><LegacyUnknown>1</LegacyUnknown></CData>",
                    Encoding.UTF8);
                VisionPipeline r10Loaded =
                    VisionPipelineStorage.Load(
                        r10,
                        r10PipelineName);
                DataState r10Data =
                    RecipeDataStorage.Load(
                        r10,
                        new DataState());
                Require(
                    r10Loaded.Steps.Count == 1
                    && r10Data != null
                    && !VisionPipelineStorage
                        .TryGetPersistenceState(
                            r10,
                            r10PipelineName,
                            out _)
                    && !RecipeDataStorage
                        .TryGetPersistenceState(r10, out _),
                    "R10 compatibility boundary changed.");
                rows.Add(
                    "R10\tUnknown element accepted; no schema/version stale detection\tUnknown element accepted; CData has no version/fields\tPASS\tDocumented detectability boundary");

                string matrixPath =
                    Path.Combine(
                        outputDirectory,
                        "p272_persistence_matrix.tsv");
                File.WriteAllLines(
                    matrixPath,
                    rows,
                    Encoding.UTF8);
                return Path.GetFileName(matrixPath);
            }
            finally
            {
                foreach (string recipe in recipes)
                {
                    RecipeWorkspaceService
                        .DeleteVisionWorkspace(recipe);
                }
            }
        }

        private static void RunPropertyPersistenceFeedback(
            string[] args,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            bool expectFeedback = ResolveOptionalBoolOption(
                args,
                "--expect-feedback",
                false);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext =
                    ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException(
                        "Property persistence feedback smoke did not create the shell host.");
                shellHost.SelectToolForTest(VISION_MENU.Blob);
                Pump(48);
                OpenVisionFloatingToolWindow toolWindow = Application.Current.Windows
                    .OfType<OpenVisionFloatingToolWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException(
                        "Property persistence feedback smoke did not open Blob.");
                toolWindow.Width = 920D;
                toolWindow.Height = 660D;
                toolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                toolWindow.Left = Math.Max(0D, SystemParameters.WorkArea.Left + 20D);
                toolWindow.Top = Math.Max(0D, SystemParameters.WorkArea.Top + 20D);
                toolWindow.Activate();
                Pump(24);

                System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid =
                    FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(
                        toolWindow)
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException(
                        "Property persistence feedback smoke did not find the Blob PropertyGrid.");
                BlobProperty property = propertyGrid.SelectedObject as BlobProperty
                    ?? throw new InvalidOperationException(
                        "Property persistence feedback smoke did not find the Blob property.");

                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;
                string inputRouteBefore =
                    shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBefore =
                    shellHost.ActiveNativeRouteOutputLayerNameForTest;
                string statusBefore = shellHost.ActiveNativeStatusText;

                OpenVisionNativeToolPropertySessionStore.FailNextSaveForTest = true;
                OpenVisionNativeToolPropertySessionStore.Save("Blob", property);
                Pump(24);
                string failureStatus = shellHost.ActiveNativeStatusText;
                bool feedbackVisible =
                    failureStatus.Contains("저장", StringComparison.Ordinal)
                    && failureStatus.Contains("메모리", StringComparison.Ordinal);
                if (feedbackVisible != expectFeedback)
                {
                    throw new InvalidOperationException(
                        "Property persistence failure feedback did not match the expected state. "
                        + "Expected="
                        + expectFeedback
                        + ", Before='"
                        + statusBefore
                        + "', After='"
                        + failureStatus
                        + "'.");
                }
                if (expectFeedback)
                {
                    TextBlock statusText = FindVisualChildren<TextBlock>(toolWindow)
                        .FirstOrDefault(item => item.IsVisible
                            && string.Equals(
                                item.Name,
                                "txtStatus",
                                StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Property persistence feedback smoke did not find the Tool status text.");
                    if (!string.Equals(
                            statusText.ToolTip?.ToString(),
                            failureStatus,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            System.Windows.Automation.AutomationProperties.GetHelpText(
                                statusText),
                            failureStatus,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "The complete persistence failure message was not available through tooltip and accessibility help.");
                    }
                }

                if (shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(
                        shellHost.ActiveHostLayerTitle,
                        activeLayerBefore,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        shellHost.ActiveNativeRouteInputLayerNameForTest,
                        inputRouteBefore,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        shellHost.ActiveNativeRouteOutputLayerNameForTest,
                        outputRouteBefore,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Property persistence feedback changed Preview/Run, layers, active layer, or routes.");
                }

                string screenshotName = expectFeedback
                    ? "OpenVisionLab_Property_Save_Failure_Visible_After.png"
                    : "OpenVisionLab_Property_Save_Failure_Silent_Before.png";
                SaveWindowsScreenScreenshot(
                    new[] { toolWindow },
                    Path.Combine(outputDirectory, screenshotName));

                string recoveryStatus = "NotApplicable";
                if (expectFeedback)
                {
                    OpenVisionNativeToolPropertySessionStore.Save("Blob", property);
                    Pump(24);
                    recoveryStatus = shellHost.ActiveNativeStatusText;
                    if (!recoveryStatus.Contains("저장", StringComparison.Ordinal)
                        || !recoveryStatus.Contains("복구", StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Property persistence recovery feedback was not visible. Status='"
                            + recoveryStatus
                            + "'.");
                    }
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: property-persistence-feedback" + Environment.NewLine
                    + "RuntimeExe: " + Process.GetCurrentProcess().MainModule?.FileName + Environment.NewLine
                    + "ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location + Environment.NewLine
                    + "ExpectedFeedback: " + expectFeedback + Environment.NewLine
                    + "StatusBefore: " + statusBefore + Environment.NewLine
                    + "FailureStatus: " + failureStatus + Environment.NewLine
                    + "RecoveryStatus: " + recoveryStatus + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "RoutesUnchanged: True" + Environment.NewLine
                    + "Screenshot: " + screenshotName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                OpenVisionNativeToolPropertySessionStore.FailNextSaveForTest = false;
                window?.Close();
                app.Shutdown();
            }
        }

        private static void RunPropertyLoadFeedback(
            string[] args,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            bool expectFeedback = ResolveOptionalBoolOption(
                args,
                "--expect-feedback",
                false);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);

            OpenVisionShellHostWindow window = null;
            OpenVisionFloatingToolWindow toolWindow = null;
            OpenVisionNativeToolDocument loadDocument = null;
            try
            {
                string[] fileContractEvidence =
                    VerifyPropertyLoadFileContract(outputDirectory);
                string saveRecoveryEvidence =
                    VerifyPropertyLoadSaveRecovery();
                ApplicationRuntimeContext runtimeContext =
                    ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException(
                        "Property load feedback smoke did not create the shell host.");
                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;

                // Force this smoke through the Direct Tool session-store load path
                // instead of the already-populated runtime repository.
                OpenVisionNativeToolPropertySessionStore.SetRepositoryContext(() => null);
                OpenVisionNativeToolPropertySessionStore.FailNextLoadKeyForTest =
                    "Blob_1";
                _ = OpenVisionNativeToolPropertySessionStore.GetOrLoad(
                    "Blob_1",
                    () => new BlobProperty("Blob_1"));
                loadDocument =
                    OpenVisionNativePropertyGridToolFactory.CreateBlob(
                        runtimeContext.DisplayManager);
                toolWindow = new OpenVisionFloatingToolWindow(
                    "Blob",
                    loadDocument.View)
                {
                    Owner = window
                };
                toolWindow.Width = 920D;
                toolWindow.Height = 660D;
                toolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                toolWindow.Left = Math.Max(0D, SystemParameters.WorkArea.Left + 20D);
                toolWindow.Top = Math.Max(0D, SystemParameters.WorkArea.Top + 20D);
                toolWindow.Show();
                toolWindow.Activate();
                Pump(24);

                string loadStatus = loadDocument.LastStatusText;
                bool feedbackVisible =
                    (loadStatus.Contains("saved settings", StringComparison.OrdinalIgnoreCase)
                        && loadStatus.Contains("default values", StringComparison.OrdinalIgnoreCase))
                    || (loadStatus.Contains(
                            "\uC800\uC7A5 \uC124\uC815",
                            StringComparison.Ordinal)
                        && loadStatus.Contains(
                            "\uAE30\uBCF8\uAC12",
                            StringComparison.Ordinal));
                if (feedbackVisible != expectFeedback)
                {
                    throw new InvalidOperationException(
                        "Property load failure feedback did not match the expected state. "
                        + "Expected="
                        + expectFeedback
                        + ", Status='"
                        + loadStatus
                        + "'.");
                }

                if (expectFeedback)
                {
                    TextBlock statusText = FindVisualChildren<TextBlock>(toolWindow)
                        .FirstOrDefault(item => item.IsVisible
                            && string.Equals(
                                item.Name,
                                "txtStatus",
                                StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Property load feedback smoke did not find the visible Tool status text.");
                    if (!string.Equals(
                            statusText.ToolTip?.ToString(),
                            loadStatus,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            System.Windows.Automation.AutomationProperties.GetHelpText(
                                statusText),
                            loadStatus,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "The complete property load failure message was not available through tooltip and accessibility help.");
                    }
                }

                string inputRoute =
                    loadDocument.RouteInputLayerName;
                string outputRoute =
                    loadDocument.RouteOutputLayerName;
                if (loadDocument.PreviewRunCount != previewRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(
                        shellHost.ActiveHostLayerTitle,
                        activeLayerBefore,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Property load feedback changed Preview/Run, layers, or active layer.");
                }

                string screenshotName = expectFeedback
                    ? "OpenVisionLab_Property_Load_Failure_Visible_After.png"
                    : "OpenVisionLab_Property_Load_Failure_Silent_Before.png";
                SaveWindowsScreenScreenshot(
                    new[] { toolWindow },
                    Path.Combine(outputDirectory, screenshotName));

                OpenVisionLanguageService.SetLanguage(
                    OpenVisionLanguage.English,
                    false);
                using (OpenVisionNativeToolDocument englishDocument =
                    OpenVisionNativePropertyGridToolFactory.CreateBlob(
                        runtimeContext.DisplayManager))
                {
                    if (!englishDocument.LastStatusText.Contains(
                            "Saved settings could not be loaded",
                            StringComparison.Ordinal)
                        || !englishDocument.LastStatusText.Contains(
                            "default values",
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "P270 English load failure feedback was not localized.");
                    }
                }
                OpenVisionLanguageService.SetLanguage(
                    OpenVisionLanguage.Korean,
                    false);

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: property-load-feedback" + Environment.NewLine
                    + "RuntimeExe: " + Process.GetCurrentProcess().MainModule?.FileName + Environment.NewLine
                    + "ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location + Environment.NewLine
                    + "ExpectedFeedback: " + expectFeedback + Environment.NewLine
                    + "LoadStatus: " + loadStatus + Environment.NewLine
                    + "PreviewRunCount: " + loadDocument.PreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "InputRoute: " + inputRoute + Environment.NewLine
                    + "OutputRoute: " + outputRoute + Environment.NewLine
                    + string.Join(Environment.NewLine, fileContractEvidence)
                    + Environment.NewLine
                    + saveRecoveryEvidence
                    + Environment.NewLine
                    + "LocalizationContract: Korean,English"
                    + Environment.NewLine
                    + "Screenshot: " + screenshotName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                OpenVisionNativeToolPropertySessionStore.FailNextLoadKeyForTest =
                    string.Empty;
                toolWindow?.ClearHostedContent();
                toolWindow?.Close();
                loadDocument?.Dispose();
                window?.Close();
                app.Shutdown();
            }
        }

        private static string[] VerifyPropertyLoadFileContract(
            string outputDirectory)
        {
            string contractDirectory = Path.Combine(
                outputDirectory,
                "file_contract_"
                + DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff",
                    CultureInfo.InvariantCulture));
            Directory.CreateDirectory(contractDirectory);

            string missingPath = Path.Combine(
                contractDirectory,
                "missing_then_valid.xml");
            BlobProperty defaultMissing =
                new BlobProperty("P270_MissingThenValid");
            BlobProperty created = SerializeHelper.LoadOrCreateXmlFile(
                missingPath,
                defaultMissing,
                out bool missingLoaded,
                out XmlFileLoadResult missingResult);
            if (missingLoaded
                || missingResult.Disposition
                    != XmlFileLoadDisposition.CreatedDefaultForMissingFile
                || !File.Exists(missingPath)
                || !ReferenceEquals(created, defaultMissing))
            {
                throw new InvalidOperationException(
                    "P270 missing configuration was not created as a normal default.");
            }

            BlobProperty valid = SerializeHelper.LoadOrCreateXmlFile(
                missingPath,
                new BlobProperty("P270_MissingThenValid"),
                out bool validLoaded,
                out XmlFileLoadResult validResult);
            if (!validLoaded
                || validResult.Disposition != XmlFileLoadDisposition.Loaded
                || valid == null)
            {
                throw new InvalidOperationException(
                    "P270 valid configuration did not restore as a loaded value.");
            }

            string invalidPath = Path.Combine(
                contractDirectory,
                "invalid_then_recovered.xml");
            File.WriteAllText(
                invalidPath,
                "<BlobProperty><MIN_AREA>not-a-number</MIN_AREA>",
                Encoding.UTF8);
            BlobProperty recovered = SerializeHelper.LoadOrCreateXmlFile(
                invalidPath,
                new BlobProperty("P270_Invalid"),
                out bool invalidLoaded,
                out XmlFileLoadResult invalidResult);
            if (invalidLoaded
                || invalidResult.Disposition
                    != XmlFileLoadDisposition.ReplacedInvalidFile
                || string.IsNullOrWhiteSpace(invalidResult.BackupPath)
                || !File.Exists(invalidResult.BackupPath)
                || !File.Exists(invalidPath)
                || recovered == null)
            {
                throw new InvalidOperationException(
                    "P270 invalid configuration was not backed up and replaced with a default.");
            }

            return new[]
            {
                "MissingConfigDisposition: "
                    + missingResult.Disposition,
                "ValidConfigDisposition: "
                    + validResult.Disposition,
                "InvalidConfigDisposition: "
                    + invalidResult.Disposition,
                "InvalidBackup: "
                    + invalidResult.BackupPath,
                "FileContractDirectory: "
                    + contractDirectory
            };
        }

        private static string VerifyPropertyLoadSaveRecovery()
        {
            string propertyKey = "P270_LoadRecovery_"
                + Guid.NewGuid().ToString("N");
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            string configPath = RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                propertyKey);
            string invalidBackupPath = string.Empty;
            OpenVisionNativeToolPropertySavedEventArgs savedResult = null;
            EventHandler<OpenVisionNativeToolPropertySavedEventArgs> handler =
                (_, args) =>
                {
                    if (string.Equals(
                            args?.ToolName,
                            "P270Recovery",
                            StringComparison.Ordinal))
                    {
                        savedResult = args;
                    }
                };
            OpenVisionNativeToolPropertySessionStore.PropertySaved += handler;
            try
            {
                if (File.Exists(configPath))
                {
                    throw new InvalidOperationException(
                        "P270 recovery probe unexpectedly reused an existing configuration.");
                }

                File.WriteAllText(
                    configPath,
                    "<BlobProperty><MIN_AREA>not-a-number</MIN_AREA>",
                    Encoding.UTF8);
                BlobProperty preloadedProperty =
                    new BlobProperty(propertyKey).LoadConfig(recipeName);
                VisionToolRepository repository = new VisionToolRepository();
                repository.Blobs.Add(preloadedProperty);
                OpenVisionNativeToolPropertySessionStore.SetRepositoryContext(
                    () => repository);
                BlobProperty property =
                    OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                        propertyKey,
                        item => item.Blobs,
                        () => new BlobProperty(propertyKey));
                if (!OpenVisionNativeToolPropertySessionStore.TryGetLoadFailure(
                        propertyKey,
                        out OpenVisionNativeToolPropertyLoadFailure loadFailure)
                    || !loadFailure.PreviousFileWasBackedUp
                    || !File.Exists(loadFailure.BackupPath))
                {
                    throw new InvalidOperationException(
                        "P270 recovery probe did not retain the invalid-file backup.");
                }
                invalidBackupPath = loadFailure.BackupPath;

                OpenVisionNativeToolPropertySessionStore.Save(
                    "P270Recovery",
                    property);
                _ = OpenVisionNativeToolPropertySessionStore.GetRepositoryProperty(
                    propertyKey,
                    item => item.Blobs,
                    () => new BlobProperty(propertyKey));
                if (savedResult == null
                    || !savedResult.Succeeded
                    || !savedResult.RecoveredFromFailure
                    || OpenVisionNativeToolPropertySessionStore.TryGetLoadFailure(
                        propertyKey,
                        out _)
                    || !File.Exists(configPath))
                {
                    throw new InvalidOperationException(
                        "P270 explicit save did not clear the retained load failure.");
                }

                return "RepositoryPreloadedInvalidBackupAndSaveRecovery: Passed";
            }
            finally
            {
                OpenVisionNativeToolPropertySessionStore.PropertySaved -= handler;
                OpenVisionNativeToolPropertySessionStore.FailNextLoadKeyForTest =
                    string.Empty;
                string fullConfigPath = Path.GetFullPath(configPath);
                string fullRecipeRoot = Path.GetFullPath(
                    Path.Combine(
                        AppPathService.DataRootDirectory,
                        "RECIPE"))
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                if (fullConfigPath.StartsWith(
                        fullRecipeRoot,
                        StringComparison.OrdinalIgnoreCase)
                    && File.Exists(fullConfigPath))
                {
                    File.Delete(fullConfigPath);
                }

                if (!string.IsNullOrWhiteSpace(invalidBackupPath))
                {
                    string fullBackupPath =
                        Path.GetFullPath(invalidBackupPath);
                    if (fullBackupPath.StartsWith(
                            fullRecipeRoot,
                            StringComparison.OrdinalIgnoreCase)
                        && File.Exists(fullBackupPath))
                    {
                        File.Delete(fullBackupPath);
                    }
                }
            }
        }

        private static void RunSettingsPersistenceFeedback(
            string[] args,
            string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string fileContractEvidence =
                VerifySettingsStoreFileContract();
            bool expectFeedback = ResolveOptionalBoolOption(
                args,
                "--expect-feedback",
                false);
            string configName =
                OpenVisionNativeToolSettingsStore.CreateConfigName(
                    "Threshold");
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            string configPath = RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                configName);
            bool configExisted = File.Exists(configPath);
            byte[] originalConfig = configExisted
                ? File.ReadAllBytes(configPath)
                : Array.Empty<byte>();
            string previousDisablePrewarm =
                Environment.GetEnvironmentVariable(
                    "OPENVISIONLAB_DISABLE_NATIVE_PREWARM");
            Environment.SetEnvironmentVariable(
                "OPENVISIONLAB_DISABLE_NATIVE_PREWARM",
                "1");

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(
                OpenVisionLanguage.Korean,
                false);
            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext =
                    ApplicationRuntimeContext.CreateDefault();
                OpenVisionNativeToolSettingsStore.FailNextLoadKeyForTest =
                    configName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };
                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(28);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException(
                        "Settings persistence feedback smoke did not create the shell host.");
                shellHost.SelectToolForTest(VISION_MENU.Threshold);
                Pump(40);
                OpenVisionFloatingToolWindow toolWindow =
                    Application.Current.Windows
                        .OfType<OpenVisionFloatingToolWindow>()
                        .FirstOrDefault(item => item.IsVisible
                            && FindVisualChildren<ThresholdToolWpfView>(item)
                                .Any())
                    ?? throw new InvalidOperationException(
                        "Settings persistence feedback smoke did not open Threshold.");
                toolWindow.Width = 920D;
                toolWindow.Height = 660D;
                toolWindow.WindowStartupLocation =
                    WindowStartupLocation.Manual;
                toolWindow.Left = Math.Max(
                    0D,
                    SystemParameters.WorkArea.Left + 20D);
                toolWindow.Top = Math.Max(
                    0D,
                    SystemParameters.WorkArea.Top + 20D);
                toolWindow.Activate();
                Pump(24);

                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;
                string inputRouteBefore =
                    shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBefore =
                    shellHost.ActiveNativeRouteOutputLayerNameForTest;
                string loadStatus = shellHost.ActiveNativeStatusText;
                bool loadFeedbackVisible =
                    ContainsSettingsLoadFailureFeedback(loadStatus);
                if (loadFeedbackVisible != expectFeedback)
                {
                    throw new InvalidOperationException(
                        "Settings load feedback did not match the expected state. "
                        + "Expected="
                        + expectFeedback
                        + ", Status='"
                        + loadStatus
                        + "'.");
                }

                TextBlock statusText = FindVisualChildren<TextBlock>(toolWindow)
                    .FirstOrDefault(item => item.IsVisible
                        && string.Equals(
                            item.Name,
                            "txtStatus",
                            StringComparison.Ordinal))
                    ?? throw new InvalidOperationException(
                        "Settings persistence feedback smoke did not find the visible Tool status text.");
                if (expectFeedback
                    && (!string.Equals(
                            statusText.ToolTip?.ToString(),
                            loadStatus,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            System.Windows.Automation.AutomationProperties.GetHelpText(
                                statusText),
                            loadStatus,
                            StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException(
                        "The complete settings load failure message was not available through tooltip and accessibility help.");
                }

                string loadScreenshot = expectFeedback
                    ? "OpenVisionLab_Settings_Load_Failure_Visible_After.png"
                    : "OpenVisionLab_Settings_Load_Save_Failure_Silent_Before.png";
                SaveWindowsScreenScreenshot(
                    new[] { toolWindow },
                    Path.Combine(outputDirectory, loadScreenshot));

                OpenVisionNativeToolSettingsStore.FailNextSaveKeyForTest =
                    configName;
                OpenVisionNativeToolSettingsStore.Save(
                    configName,
                    new ThresholdToolSettings());
                Pump(20);
                string saveStatus = shellHost.ActiveNativeStatusText;
                bool saveFeedbackVisible =
                    ContainsSettingsSaveFailureFeedback(saveStatus);
                if (saveFeedbackVisible != expectFeedback)
                {
                    throw new InvalidOperationException(
                        "Settings save feedback did not match the expected state. "
                        + "Expected="
                        + expectFeedback
                        + ", Status='"
                        + saveStatus
                        + "'.");
                }

                if (expectFeedback
                    && (!string.Equals(
                            statusText.ToolTip?.ToString(),
                            saveStatus,
                            StringComparison.Ordinal)
                        || !string.Equals(
                            System.Windows.Automation.AutomationProperties.GetHelpText(
                                statusText),
                            saveStatus,
                            StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException(
                        "The complete settings save failure message was not available through tooltip and accessibility help.");
                }

                string saveScreenshot = "NotApplicable";
                string recoveryStatus = "NotApplicable";
                string englishFailureStatus = "NotApplicable";
                string englishRecoveryStatus = "NotApplicable";
                if (expectFeedback)
                {
                    saveScreenshot =
                        "OpenVisionLab_Settings_Save_Failure_Visible_After.png";
                    SaveWindowsScreenScreenshot(
                        new[] { toolWindow },
                        Path.Combine(outputDirectory, saveScreenshot));
                    OpenVisionNativeToolSettingsStore.Save(
                        configName,
                        new ThresholdToolSettings());
                    Pump(20);
                    recoveryStatus = shellHost.ActiveNativeStatusText;
                    if (!recoveryStatus.Contains(
                            "\uBCF5\uAD6C",
                            StringComparison.Ordinal)
                        && !recoveryStatus.Contains(
                            "recovered",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Settings save recovery feedback was not visible. Status='"
                            + recoveryStatus
                            + "'.");
                    }

                    OpenVisionNativeToolSettingsStore.Save(
                        configName,
                        new ThresholdToolSettings());
                    Pump(20);
                    if (!string.Equals(
                            shellHost.ActiveNativeStatusText,
                            recoveryStatus,
                            StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "An ordinary settings save replaced the one-time recovery feedback.");
                    }

                    OpenVisionLanguageService.SetLanguage(
                        OpenVisionLanguage.English,
                        false);
                    OpenVisionNativeToolSettingsStore.FailNextSaveKeyForTest =
                        configName;
                    OpenVisionNativeToolSettingsStore.Save(
                        configName,
                        new ThresholdToolSettings());
                    Pump(20);
                    englishFailureStatus = shellHost.ActiveNativeStatusText;
                    if (!englishFailureStatus.Contains(
                            "could not be saved",
                            StringComparison.OrdinalIgnoreCase)
                        || !englishFailureStatus.Contains(
                            "remain in memory",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Settings save failure feedback was not localized in English. Status='"
                            + englishFailureStatus
                            + "'.");
                    }

                    OpenVisionNativeToolSettingsStore.Save(
                        configName,
                        new ThresholdToolSettings());
                    Pump(20);
                    englishRecoveryStatus = shellHost.ActiveNativeStatusText;
                    if (!englishRecoveryStatus.Contains(
                            "recovered",
                            StringComparison.OrdinalIgnoreCase)
                        || !englishRecoveryStatus.Contains(
                            "persisted",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Settings save recovery feedback was not localized in English. Status='"
                            + englishRecoveryStatus
                            + "'.");
                    }
                    OpenVisionLanguageService.SetLanguage(
                        OpenVisionLanguage.Korean,
                        false);
                }

                if (shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(
                        shellHost.ActiveHostLayerTitle,
                        activeLayerBefore,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        shellHost.ActiveNativeRouteInputLayerNameForTest,
                        inputRouteBefore,
                        StringComparison.Ordinal)
                    || !string.Equals(
                        shellHost.ActiveNativeRouteOutputLayerNameForTest,
                        outputRouteBefore,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Settings persistence feedback changed Preview/Run, layers, active layer, or routes.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: settings-persistence-feedback"
                    + Environment.NewLine
                    + "RuntimeExe: "
                    + Process.GetCurrentProcess().MainModule?.FileName
                    + Environment.NewLine
                    + "ManagedAssembly: "
                    + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location
                    + Environment.NewLine
                    + "ExpectedFeedback: "
                    + expectFeedback
                    + Environment.NewLine
                    + "LoadStatus: "
                    + loadStatus
                    + Environment.NewLine
                    + "SaveStatus: "
                    + saveStatus
                    + Environment.NewLine
                    + "RecoveryStatus: "
                    + recoveryStatus
                    + Environment.NewLine
                    + "EnglishFailureStatus: "
                    + englishFailureStatus
                    + Environment.NewLine
                    + "EnglishRecoveryStatus: "
                    + englishRecoveryStatus
                    + Environment.NewLine
                    + fileContractEvidence
                    + Environment.NewLine
                    + "StatusTooltipAccessibility: "
                    + (expectFeedback ? "Passed" : "NotApplicable")
                    + Environment.NewLine
                    + "PreviewRunCount: "
                    + shellHost.NativePreviewRunCount.ToString(
                        CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "LayerCount: "
                    + shellHost.LayerDocumentCount.ToString(
                        CultureInfo.InvariantCulture)
                    + Environment.NewLine
                    + "InputRoute: "
                    + shellHost.ActiveNativeRouteInputLayerNameForTest
                    + Environment.NewLine
                    + "OutputRoute: "
                    + shellHost.ActiveNativeRouteOutputLayerNameForTest
                    + Environment.NewLine
                    + "LoadScreenshot: "
                    + loadScreenshot
                    + Environment.NewLine
                    + "SaveScreenshot: "
                    + saveScreenshot
                    + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                OpenVisionNativeToolSettingsStore.FailNextLoadKeyForTest =
                    string.Empty;
                OpenVisionNativeToolSettingsStore.FailNextSaveKeyForTest =
                    string.Empty;
                window?.Close();
                app.Shutdown();
                Environment.SetEnvironmentVariable(
                    "OPENVISIONLAB_DISABLE_NATIVE_PREWARM",
                    previousDisablePrewarm);

                string fullConfigPath = Path.GetFullPath(configPath);
                string fullRecipeRoot = Path.GetFullPath(
                    Path.Combine(
                        AppPathService.DataRootDirectory,
                        "RECIPE"))
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                if (fullConfigPath.StartsWith(
                        fullRecipeRoot,
                        StringComparison.OrdinalIgnoreCase))
                {
                    if (configExisted)
                    {
                        File.WriteAllBytes(fullConfigPath, originalConfig);
                    }
                    else if (File.Exists(fullConfigPath))
                    {
                        File.Delete(fullConfigPath);
                    }
                }
            }
        }

        private static string VerifySettingsStoreFileContract()
        {
            string toolName = "P271Recovery_"
                + Guid.NewGuid().ToString("N");
            string configName =
                OpenVisionNativeToolSettingsStore.CreateConfigName(toolName);
            string recipeName = PropertyGridEditorFactory.GetRecipeName();
            string configPath = RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                configName);
            string backupPath = string.Empty;
            OpenVisionNativeToolSettingsSavedEventArgs firstSave = null;
            OpenVisionNativeToolSettingsSavedEventArgs secondSave = null;
            EventHandler<OpenVisionNativeToolSettingsSavedEventArgs> handler =
                (_, args) =>
                {
                    if (!string.Equals(
                            args?.ConfigName,
                            configName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    if (firstSave == null)
                    {
                        firstSave = args;
                    }
                    else
                    {
                        secondSave = args;
                    }
                };

            OpenVisionNativeToolSettingsStore.SettingsSaved += handler;
            try
            {
                OpenVisionNativeToolSettingsStore.ResetContext();
                if (File.Exists(configPath))
                {
                    throw new InvalidOperationException(
                        "P271 Settings Store recovery probe unexpectedly reused an existing configuration.");
                }

                ThresholdToolSettings missing =
                    OpenVisionNativeToolSettingsStore.Load(
                        configName,
                        new ThresholdToolSettings());
                if (missing == null
                    || !File.Exists(configPath)
                    || OpenVisionNativeToolSettingsStore.TryGetLoadFailure(
                        configName,
                        out _))
                {
                    throw new InvalidOperationException(
                        "P271 missing Settings Store configuration was not created as a normal default.");
                }

                ThresholdToolSettings valid =
                    OpenVisionNativeToolSettingsStore.Load(
                        configName,
                        new ThresholdToolSettings());
                if (valid == null
                    || OpenVisionNativeToolSettingsStore.TryGetLoadFailure(
                        configName,
                        out _))
                {
                    throw new InvalidOperationException(
                        "P271 valid Settings Store configuration did not load without a warning.");
                }

                File.WriteAllText(
                    configPath,
                    "<ThresholdToolSettings><Threshold>not-a-number</Threshold>",
                    Encoding.UTF8);
                OpenVisionNativeToolSettingsStore.ResetContext();
                ThresholdToolSettings recoveredDefault =
                    OpenVisionNativeToolSettingsStore.Load(
                        configName,
                        new ThresholdToolSettings());
                if (recoveredDefault == null
                    || !OpenVisionNativeToolSettingsStore.TryGetLoadFailure(
                        configName,
                        out OpenVisionNativeToolSettingsLoadFailure loadFailure)
                    || !loadFailure.PreviousFileWasBackedUp
                    || string.IsNullOrWhiteSpace(loadFailure.BackupPath)
                    || !File.Exists(loadFailure.BackupPath))
                {
                    throw new InvalidOperationException(
                        "P271 invalid Settings Store configuration was not backed up and retained as a visible load failure.");
                }
                backupPath = loadFailure.BackupPath;

                OpenVisionNativeToolSettingsStore.Save(
                    configName,
                    recoveredDefault);
                if (firstSave == null
                    || !firstSave.Succeeded
                    || !firstSave.RecoveredFromFailure
                    || OpenVisionNativeToolSettingsStore.TryGetLoadFailure(
                        configName,
                        out _))
                {
                    throw new InvalidOperationException(
                        "P271 first explicit save did not clear the retained load failure as a recovery.");
                }

                OpenVisionNativeToolSettingsStore.Save(
                    configName,
                    recoveredDefault);
                if (secondSave == null
                    || !secondSave.Succeeded
                    || secondSave.RecoveredFromFailure)
                {
                    throw new InvalidOperationException(
                        "P271 ordinary Settings Store save incorrectly repeated recovery.");
                }

                return "MissingValidInvalidBackupRecovery: Passed"
                    + Environment.NewLine
                    + "InvalidBackup: "
                    + backupPath;
            }
            finally
            {
                OpenVisionNativeToolSettingsStore.SettingsSaved -= handler;
                OpenVisionNativeToolSettingsStore.ResetContext();

                string fullConfigPath = Path.GetFullPath(configPath);
                string fullRecipeRoot = Path.GetFullPath(
                    Path.Combine(
                        AppPathService.DataRootDirectory,
                        "RECIPE"))
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                if (fullConfigPath.StartsWith(
                        fullRecipeRoot,
                        StringComparison.OrdinalIgnoreCase)
                    && File.Exists(fullConfigPath))
                {
                    File.Delete(fullConfigPath);
                }

                if (!string.IsNullOrWhiteSpace(backupPath))
                {
                    string fullBackupPath = Path.GetFullPath(backupPath);
                    if (fullBackupPath.StartsWith(
                            fullRecipeRoot,
                            StringComparison.OrdinalIgnoreCase)
                        && File.Exists(fullBackupPath))
                    {
                        File.Delete(fullBackupPath);
                    }
                }
            }
        }

        private static bool ContainsSettingsLoadFailureFeedback(string status)
        {
            return (status ?? string.Empty).Contains(
                    "\uC800\uC7A5 \uC124\uC815",
                    StringComparison.Ordinal)
                && (status ?? string.Empty).Contains(
                    "\uAE30\uBCF8\uAC12",
                    StringComparison.Ordinal);
        }

        private static bool ContainsSettingsSaveFailureFeedback(string status)
        {
            return (status ?? string.Empty).Contains(
                    "\uC800\uC7A5\uD558\uC9C0 \uBABB",
                    StringComparison.Ordinal)
                && (status ?? string.Empty).Contains(
                    "\uBA54\uBAA8\uB9AC",
                    StringComparison.Ordinal);
        }

        private static void RunParameterGuideLayout(string[] args, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            bool expectObstruction = ResolveOptionalBoolOption(args, "--expect-obstruction", false);
            bool expectGuide = ResolveOptionalBoolOption(args, "--expect-guide", true);
            bool expectBasicGuide = ResolveOptionalBoolOption(args, "--expect-basic-guide", false);
            bool expectLineLegacyReadOnly = ResolveOptionalBoolOption(
                args,
                "--expect-line-legacy-read-only",
                false);
            string toolName = ResolveOptionalTextOption(args, "--tool");
            bool useRotateScale = string.Equals(
                toolName,
                "RotateScale",
                StringComparison.OrdinalIgnoreCase);
            bool useMean = string.Equals(
                toolName,
                "Mean",
                StringComparison.OrdinalIgnoreCase);
            bool useFeatureMatching = string.Equals(
                toolName,
                "FeatureMatching",
                StringComparison.OrdinalIgnoreCase);
            bool useMatching = string.Equals(
                toolName,
                "Matching",
                StringComparison.OrdinalIgnoreCase);
            bool useEdgeBasedMatching = string.Equals(
                toolName,
                "EdgeBasedMatching",
                StringComparison.OrdinalIgnoreCase);
            bool useAffineTransform = string.Equals(
                toolName,
                "AffineTransform",
                StringComparison.OrdinalIgnoreCase);
            bool useLineLegacy = string.Equals(
                toolName,
                "LineLegacy",
                StringComparison.OrdinalIgnoreCase);
            bool useLine = useLineLegacy || string.Equals(
                toolName,
                "Line",
                StringComparison.OrdinalIgnoreCase);
            bool usePropertyGrid =
                useFeatureMatching
                || useMatching
                || useEdgeBasedMatching
                || useAffineTransform
                || useLine;
            VISION_MENU selectedMenu = useRotateScale
                ? VISION_MENU.RotateAndScale
                : useMean
                    ? VISION_MENU.Mean
                    : useFeatureMatching
                        ? VISION_MENU.FeatureMatching
                        : useMatching
                            ? VISION_MENU.Matching
                            : useEdgeBasedMatching
                                ? VISION_MENU.EdgeBasedMatching
                                : useAffineTransform
                                ? VISION_MENU.AffineTransform
                            : useLine
                                ? VISION_MENU.Line
                                : VISION_MENU.EdgeDetection;
            string selectedEditorName = useRotateScale
                ? "txtAngle"
                : useMean
                    ? "txtMeanMin"
                    : useFeatureMatching
                        ? nameof(FeatureMatchingProperty.SCORE_MIN)
                        : useMatching
                            ? nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH)
                            : useEdgeBasedMatching
                                ? nameof(EdgeBasedMatchingProperty.PATTERN_PATH)
                                : useAffineTransform
                                ? nameof(AffineTransformProperty.SourcePoint1X)
                            : useLine
                                ? useLineLegacy
                                    ? nameof(LineGaugeProperty.USE_AVERAGE_FILTER)
                                    : nameof(LineGaugeProperty.USE_MANUAL_ANGLE)
                                : "txtCannyThresholdHigh";
            string selectedToolName = useRotateScale
                ? "RotateScale"
                : useMean
                    ? "Mean"
                        : useFeatureMatching
                            ? "FeatureMatching"
                            : useMatching
                                ? "Matching"
                                : useEdgeBasedMatching
                                    ? "EdgeBasedMatching"
                                    : useAffineTransform
                                    ? "AffineTransform"
                                : useLine
                                    ? "Line"
                                    : "EdgeDetection";
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1600,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true
                };

                app.MainWindow = window;
                window.Show();
                window.Activate();
                Pump(36);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                shellHost.SelectToolForTest(selectedMenu);
                Pump(48);
                OpenVisionFloatingToolWindow toolWindow = Application.Current.Windows
                    .OfType<OpenVisionFloatingToolWindow>()
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Parameter Guide layout smoke did not open a floating Tool window.");
                toolWindow.Width = 920D;
                toolWindow.Height = useLineLegacy ? 860D : 660D;
                toolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                toolWindow.Left = Math.Max(0D, SystemParameters.WorkArea.Left + 20D);
                toolWindow.Top = Math.Max(0D, SystemParameters.WorkArea.Top + 20D);
                toolWindow.Activate();
                Pump(24);

                int previewRunsBefore = shellHost.NativePreviewRunCount;
                int layerCountBefore = shellHost.LayerDocumentCount;
                string activeLayerBefore = shellHost.ActiveHostLayerTitle;
                string inputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                string outputRouteBefore = shellHost.ActiveNativeRouteOutputLayerNameForTest;
                FrameworkElement selectedEditor;
                if (usePropertyGrid)
                {
                    string selectedPropertyName = useFeatureMatching
                        ? nameof(FeatureMatchingProperty.SCORE_MIN)
                        : useMatching
                            ? nameof(MatchingProperty.USE_COARSE_TO_FINE_ANGLE_SEARCH)
                            : useEdgeBasedMatching
                                ? nameof(EdgeBasedMatchingProperty.PATTERN_PATH)
                                : useAffineTransform
                                ? nameof(AffineTransformProperty.SourcePoint1X)
                            : useLineLegacy
                                ? nameof(LineGaugeProperty.USE_AVERAGE_FILTER)
                                : nameof(LineGaugeProperty.USE_MANUAL_ANGLE);
                    System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid =
                        FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(toolWindow)
                            .FirstOrDefault(item => item.IsVisible)
                        ?? throw new InvalidOperationException(
                            "Parameter Guide layout smoke did not find the "
                            + selectedToolName
                            + " PropertyGrid.");
                    if (useLineLegacy)
                    {
                        LineGaugeProperty lineProperty = propertyGrid.SelectedObject as LineGaugeProperty
                            ?? throw new InvalidOperationException(
                                "Parameter Guide layout smoke did not find the Line property model.");
                        lineProperty.USE_AVERAGE_FILTER = true;
                        new OpenVisionLab.Common.PropertyGridEventBinder(null)
                            .ApplyVisibilityRules(propertyGrid);
                        Pump(12);
                    }
                    if (!propertyGrid.FocusProperty(selectedPropertyName))
                    {
                        throw new InvalidOperationException(
                            "Parameter Guide layout smoke could not focus the "
                            + selectedToolName
                            + " "
                            + selectedPropertyName
                            + " row.");
                    }
                    Pump(12);
                    FrameworkElement propertyRow = FindVisualChildren<FrameworkElement>(toolWindow)
                        .FirstOrDefault(item => item.IsVisible
                            && string.Equals(
                                ResolvePropertyName(item.DataContext),
                                selectedPropertyName,
                                StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Parameter Guide layout smoke did not find the "
                            + selectedToolName
                            + " "
                            + selectedPropertyName
                            + " row.");
                    selectedEditor = FindVisualChildren<FrameworkElement>(propertyRow)
                        .FirstOrDefault(item => item.IsVisible
                            && (item is TextBox || item is CheckBox || item is ComboBox))
                        ?? propertyRow;
                    selectedEditor.RaiseEvent(new MouseButtonEventArgs(
                        Mouse.PrimaryDevice,
                        Environment.TickCount,
                        MouseButton.Left)
                    {
                        RoutedEvent = Mouse.PreviewMouseDownEvent,
                        Source = selectedEditor
                    });
                }
                else
                {
                    selectedEditor = FindVisualChildren<TextBox>(toolWindow)
                        .FirstOrDefault(item => item.IsVisible
                            && string.Equals(item.Name, selectedEditorName, StringComparison.Ordinal))
                        ?? throw new InvalidOperationException(
                            "Parameter Guide layout smoke did not find " + selectedEditorName + ".");
                }

                selectedEditor.Focus();
                Pump(24);

                if (!expectGuide)
                {
                    if (Application.Current.Windows
                        .OfType<Window>()
                        .Where(item => item.IsVisible)
                        .SelectMany(FindVisualChildren<VisionToolParameterGuideView>)
                        .Any(item => item.IsVisible))
                    {
                        throw new InvalidOperationException(
                            selectedToolName + " baseline unexpectedly showed a Parameter Guide.");
                    }

                    if (shellHost.NativePreviewRunCount != previewRunsBefore
                        || shellHost.LayerDocumentCount != layerCountBefore
                        || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                        || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
                        || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRouteBefore, StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException(
                            "Parameter Guide baseline inspection changed execution, layers, active layer, or routes.");
                    }

                    const string baselineScreenshotName =
                        "OpenVisionLab_Mean_ParameterGuide_Missing_Before.png";
                    SaveWindowsScreenScreenshot(
                        new[] { toolWindow },
                        Path.Combine(outputDirectory, baselineScreenshotName));
                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: parameter-guide-layout" + Environment.NewLine
                        + "Tool: " + selectedToolName + Environment.NewLine
                        + "RuntimeExe: " + Process.GetCurrentProcess().MainModule?.FileName + Environment.NewLine
                        + "ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location + Environment.NewLine
                        + "WindowSize: " + toolWindow.ActualWidth.ToString("0", CultureInfo.InvariantCulture)
                        + "x" + toolWindow.ActualHeight.ToString("0", CultureInfo.InvariantCulture) + Environment.NewLine
                        + "GuideVisible: False" + Environment.NewLine
                        + "SelectedEditor: " + selectedEditorName + Environment.NewLine
                        + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                        + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                        + "Screenshot: " + baselineScreenshotName + Environment.NewLine,
                        Encoding.UTF8);
                    return;
                }

                VisionToolParameterGuideView guide = Application.Current.Windows
                    .OfType<Window>()
                    .Where(item => item.IsVisible)
                    .SelectMany(FindVisualChildren<VisionToolParameterGuideView>)
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException("Parameter Guide layout smoke did not show the guide.");
                if (!guide.IsExpandedForTest)
                {
                    throw new InvalidOperationException("Parameter Guide layout smoke did not expand the focused guide.");
                }

                if (useLineLegacy)
                {
                    if (guide.IsKeyboardFocusWithin || Window.GetWindow(guide)?.IsActive == true)
                    {
                        throw new InvalidOperationException(
                            "Parameter Guide sidecar took focus from the read-only Line compatibility row.");
                    }
                }
                else if (!selectedEditor.IsKeyboardFocused && !selectedEditor.IsKeyboardFocusWithin)
                {
                    throw new InvalidOperationException(
                        "Parameter Guide sidecar stole keyboard focus from " + selectedEditorName + ".");
                }

                if (usePropertyGrid)
                {
                    bool basicCoverage =
                        guide.CoverageForTest.Contains("Basic", StringComparison.OrdinalIgnoreCase)
                        || guide.CoverageForTest.Contains("기본", StringComparison.Ordinal);
                    if (basicCoverage != expectBasicGuide)
                    {
                        throw new InvalidOperationException(
                            selectedToolName + " guide coverage did not match the expected baseline. "
                            + "ExpectedBasic="
                            + expectBasicGuide
                            + ", Actual="
                            + guide.CoverageForTest);
                    }
                }
                if (useLineLegacy)
                {
                    System.Windows.Controls.WpfPropertyGrid.PropertyGrid propertyGrid =
                        FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(toolWindow)
                            .First(item => item.IsVisible);
                    PropertyDescriptor descriptor = TypeDescriptor.GetProperties(
                        propertyGrid.SelectedObject)[nameof(LineGaugeProperty.USE_AVERAGE_FILTER)];
                    bool compatibilityReadOnly =
                        descriptor?.Attributes[typeof(PropertyGridCompatibilityReadOnlyAttribute)]
                            is PropertyGridCompatibilityReadOnlyAttribute;
                    if (descriptor == null
                        || compatibilityReadOnly != expectLineLegacyReadOnly)
                    {
                        throw new InvalidOperationException(
                            "Line legacy control read-only state did not match. Expected="
                            + expectLineLegacyReadOnly
                            + ", Actual="
                            + (descriptor == null ? "missing" : compatibilityReadOnly.ToString()));
                    }
                }

                Button guideButton = FindVisualChildren<Button>(toolWindow)
                    .FirstOrDefault(item => item.IsVisible
                        && string.Equals(
                            System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                            "VisionToolParameterGuideButton",
                            StringComparison.Ordinal))
                    ?? throw new InvalidOperationException(
                        "Parameter Guide layout smoke did not find the explicit guide toggle.");
                guideButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, guideButton));
                Pump(12);
                if (Application.Current.Windows
                    .OfType<Window>()
                    .Where(item => item.IsVisible)
                    .SelectMany(FindVisualChildren<VisionToolParameterGuideView>)
                    .Any(item => item.IsVisible))
                {
                    throw new InvalidOperationException(
                        "Parameter Guide toggle did not hide the sidecar.");
                }

                guideButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, guideButton));
                Pump(18);
                guide = Application.Current.Windows
                    .OfType<Window>()
                    .Where(item => item.IsVisible)
                    .SelectMany(FindVisualChildren<VisionToolParameterGuideView>)
                    .FirstOrDefault(item => item.IsVisible)
                    ?? throw new InvalidOperationException(
                        "Parameter Guide toggle did not reopen the sidecar.");

                string[] teachingControlNames = useRotateScale
                    ? new[]
                    {
                        "txtAngle",
                        "sliderAngle",
                        "txtScaleXPercent",
                        "sliderScaleXPercent",
                        "txtScaleYPercent",
                        "sliderScaleYPercent",
                        "btnRunPreview",
                        "cbInputLayer",
                        "cbOutputLayer",
                        "bdInputPreview",
                        "bdOutputPreview"
                    }
                    : useMean
                        ? new[]
                        {
                            "cbMeanType",
                            "txtMeanMin",
                            "sliderMeanMin",
                            "txtMeanMax",
                            "sliderMeanMax",
                            "btnRunPreview",
                            "cbInputLayer",
                            "cbOutputLayer",
                            "bdInputPreview",
                            "bdOutputPreview"
                        }
                    : usePropertyGrid
                        ? new[]
                        {
                            "propertyGridHost",
                            "btnRunPreview",
                            "cbInputLayer",
                            "cbOutputLayer",
                            "bdInputPreview",
                            "bdOutputPreview"
                        }
                    : new[]
                    {
                        "cbEdgeType",
                        "txtCannyThresholdLow",
                        "txtCannyThresholdHigh",
                        "txtCannyApertureSize",
                        "chkUseL2Gradient",
                        "btnRunPreview",
                        "cbInputLayer",
                        "cbOutputLayer",
                        "bdInputPreview",
                        "bdOutputPreview"
                    };
                Rect guideBounds = GetElementScreenBounds(guide);
                List<string> obstructed = FindVisualChildren<FrameworkElement>(toolWindow)
                    .Where(item => item.IsVisible && teachingControlNames.Contains(item.Name, StringComparer.Ordinal))
                    .Where(item => guideBounds.IntersectsWith(GetElementScreenBounds(item)))
                    .Select(item => item.Name)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(item => item, StringComparer.Ordinal)
                    .ToList();

                if (expectObstruction && obstructed.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Parameter Guide baseline expected an obstruction but found none.");
                }

                if (!expectObstruction && obstructed.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Parameter Guide still obstructs teaching controls: "
                        + string.Join(",", obstructed));
                }

                if (shellHost.NativePreviewRunCount != previewRunsBefore
                    || shellHost.LayerDocumentCount != layerCountBefore
                    || !string.Equals(shellHost.ActiveHostLayerTitle, activeLayerBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputRouteBefore, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Parameter Guide layout inspection changed execution, layers, active layer, or routes.");
                }

                string screenshotName = expectObstruction
                    ? "OpenVisionLab_ParameterGuide_Obstructing_Before.png"
                    : useRotateScale
                        ? "OpenVisionLab_RotateScale_Guide_NonObstructing_After.png"
                        : useMean
                            ? "OpenVisionLab_Mean_Guide_NonObstructing_After.png"
                            : useFeatureMatching && expectBasicGuide
                                ? "OpenVisionLab_FeatureMatching_Basic_Guide_Before.png"
                                : useFeatureMatching
                                    ? "OpenVisionLab_FeatureMatching_Detailed_Guide_After.png"
                                    : useMatching && expectBasicGuide
                                        ? "OpenVisionLab_Matching_Basic_Guide_Before.png"
                                    : useMatching
                                            ? "OpenVisionLab_Matching_Detailed_Guide_After.png"
                                            : useEdgeBasedMatching && expectBasicGuide
                                                ? "OpenVisionLab_EdgeBasedMatching_Basic_Guide_Before.png"
                                            : useEdgeBasedMatching
                                                    ? "OpenVisionLab_EdgeBasedMatching_Detailed_Guide_After.png"
                                            : useAffineTransform && expectBasicGuide
                                                ? "OpenVisionLab_AffineTransform_Basic_Guide_Before.png"
                                            : useAffineTransform
                                                    ? "OpenVisionLab_AffineTransform_Detailed_Guide_After.png"
                                            : useLineLegacy && expectLineLegacyReadOnly
                                                ? "OpenVisionLab_Line_Legacy_Controls_ReadOnly_After.png"
                                                : useLineLegacy
                                                    ? "OpenVisionLab_Line_Legacy_Controls_Editable_Before.png"
                                            : useLine && expectBasicGuide
                                                ? "OpenVisionLab_Line_Basic_Guide_Before.png"
                                                : useLine
                                                    ? "OpenVisionLab_Line_Detailed_Guide_After.png"
                                            : "OpenVisionLab_ParameterGuide_NonObstructing_After.png";
                Window guideWindow = Window.GetWindow(guide);
                SaveWindowsScreenScreenshot(
                    new[] { toolWindow, guideWindow },
                    Path.Combine(outputDirectory, screenshotName));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: parameter-guide-layout" + Environment.NewLine
                    + "Tool: " + selectedToolName + Environment.NewLine
                    + "RuntimeExe: " + Process.GetCurrentProcess().MainModule?.FileName + Environment.NewLine
                    + "ManagedAssembly: " + typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location + Environment.NewLine
                    + "WindowSize: " + toolWindow.ActualWidth.ToString("0", CultureInfo.InvariantCulture)
                    + "x" + toolWindow.ActualHeight.ToString("0", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "GuideBounds: " + FormatGuideLayoutRect(guideBounds) + Environment.NewLine
                    + "ObstructedControls: " + (obstructed.Count == 0 ? "None" : string.Join(",", obstructed)) + Environment.NewLine
                    + "GuideCoverage: " + guide.CoverageForTest + Environment.NewLine
                    + "LineLegacyReadOnly: " + (useLineLegacy
                        ? expectLineLegacyReadOnly.ToString()
                        : "NotApplicable") + Environment.NewLine
                    + "AutomaticShowFocusRetained: True" + Environment.NewLine
                    + "ExplicitHideReopen: PASS" + Environment.NewLine
                    + "PreviewRunCount: " + shellHost.NativePreviewRunCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "LayerCount: " + shellHost.LayerDocumentCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Screenshot: " + screenshotName + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static Rect GetElementScreenBounds(FrameworkElement element)
        {
            if (element == null || element.ActualWidth <= 0D || element.ActualHeight <= 0D)
            {
                return Rect.Empty;
            }

            System.Windows.Point topLeft = element.PointToScreen(new System.Windows.Point(0D, 0D));
            System.Windows.Point bottomRight = element.PointToScreen(
                new System.Windows.Point(element.ActualWidth, element.ActualHeight));
            return new Rect(topLeft, bottomRight);
        }

        private static string FormatGuideLayoutRect(Rect rect)
        {
            return string.Join(
                ",",
                rect.X.ToString("0.0", CultureInfo.InvariantCulture),
                rect.Y.ToString("0.0", CultureInfo.InvariantCulture),
                rect.Width.ToString("0.0", CultureInfo.InvariantCulture),
                rect.Height.ToString("0.0", CultureInfo.InvariantCulture));
        }

        private static void SaveWindowsScreenScreenshot(IEnumerable<Window> windows, string path)
        {
            List<Window> visibleWindows = windows
                .Where(window => window?.IsVisible == true)
                .Distinct()
                .ToList();
            if (visibleWindows.Count == 0)
            {
                throw new InvalidOperationException("No visible windows were supplied for screen capture.");
            }

            foreach (Window window in visibleWindows)
            {
                BringWindowToFront(window);
            }

            Pump(24);
            Rect union = Rect.Empty;
            foreach (Window window in visibleWindows)
            {
                System.Windows.Point topLeft = window.PointToScreen(new System.Windows.Point(0D, 0D));
                System.Windows.Point bottomRight = window.PointToScreen(
                    new System.Windows.Point(window.ActualWidth, window.ActualHeight));
                Rect bounds = new Rect(topLeft, bottomRight);
                union = union.IsEmpty ? bounds : Rect.Union(union, bounds);
            }

            int pixelWidth = Math.Max(1, (int)Math.Ceiling(union.Width));
            int pixelHeight = Math.Max(1, (int)Math.Ceiling(union.Height));
            using (Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    (int)Math.Floor(union.X),
                    (int)Math.Floor(union.Y),
                    0,
                    0,
                    new System.Drawing.Size(pixelWidth, pixelHeight));
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void RunLayerInitialDockedWorkspace(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            WithDockingStateFileBackup(() =>
            {
                Application app = Application.Current ?? new Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

                OpenVisionShellHostWindow window = null;
                StringBuilder report = new StringBuilder();
                try
                {
                    ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                    window = new OpenVisionShellHostWindow(runtimeContext)
                    {
                        Width = 1600,
                        Height = 900,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    app.MainWindow = window;
                    window.Show();
                    Pump(24);

                    OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                        ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                    PrepareDockingVerificationLayers(shellHost);
                    Pump(48);

                    if (!shellHost.AreHostLayerTabsReadableForTest)
                    {
                        throw new InvalidOperationException("Initial docked workspace could not read the hidden host layer model rows.");
                    }

                    if (shellHost.IsSingleWorkspaceVisibleForTest)
                    {
                        throw new InvalidOperationException("Initial docked workspace must not show the legacy single-layer preview surface.");
                    }

                    if (!shellHost.IsDockedWorkspaceVisibleForTest)
                    {
                        throw new InvalidOperationException("Initial docked workspace is not visible.");
                    }

                    if (shellHost.DockedLayerCount != 2
                        || !ContainsDockedLayerTitle(shellHost, "Main")
                        || !ContainsDockedLayerTitle(shellHost, "HSV_Preview"))
                    {
                        throw new InvalidOperationException(
                            "Initial docked workspace did not auto-create the expected layer documents. "
                            + $"DockedLayers={shellHost.DockedLayerCount}, Titles={shellHost.DockedLayerTitles}");
                    }

                    AssertDockingVerificationStage(shellHost, window, report, "01_initial_auto_docked", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_initial_auto_docked.png"));

                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: layer-initial-docked-workspace" + Environment.NewLine
                        + "Screenshots: 01_initial_auto_docked.png" + Environment.NewLine
                        + report
                        + "DockedLayers=" + shellHost.DockedLayerCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                        + "DockedPanes=" + shellHost.DockedLayerPaneCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                        + "Titles=" + shellHost.DockedLayerTitles + Environment.NewLine,
                        Encoding.UTF8);
                }
                catch
                {
                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: FAIL" + Environment.NewLine
                        + "Scenario: layer-initial-docked-workspace" + Environment.NewLine
                        + report,
                        Encoding.UTF8);
                    throw;
                }
                finally
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    app.Shutdown();
                }
            });
        }

        private static void RunLayerDockingMouseDrag(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            WithDockingStateFileBackup(() =>
            {
                Application app = Application.Current ?? new Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

                OpenVisionShellHostWindow window = null;
                StringBuilder report = new StringBuilder();
                try
                {
                    ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                    window = new OpenVisionShellHostWindow(runtimeContext)
                    {
                        Width = 1600,
                        Height = 900,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    app.MainWindow = window;
                    window.Show();
                    Pump(24);

                    OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                        ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                    PrepareDockingVerificationLayers(shellHost);

                    ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview");
                    AssertDockingVerificationStage(shellHost, window, report, "01_mouse_drag_global_right_setup", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_mouse_drag_global_right_setup.png"));
                    DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "01_mouse_drag_global_right_input",
                        "HSV_Preview",
                        CreateGlobalMouseDragTarget(shellHost.DockedLayerVisualSnapshotForTest, DockingGuideZone.GlobalRight));
                    AssertDockingVerificationStage(shellHost, window, report, "02_mouse_drag_global_right", 2, 2, "Horizontal", 2);
                    AssertGlobalRightSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_mouse_drag_global_right.png"));

                    ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview");
                    AssertDockingVerificationStage(shellHost, window, report, "03_mouse_drag_global_bottom_setup", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "03_mouse_drag_global_bottom_setup.png"));
                    DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "03_mouse_drag_global_bottom_input",
                        "HSV_Preview",
                        CreateGlobalMouseDragTarget(shellHost.DockedLayerVisualSnapshotForTest, DockingGuideZone.GlobalBottom));
                    AssertDockingVerificationStage(shellHost, window, report, "04_mouse_drag_global_bottom", 2, 2, "Vertical", 2);
                    AssertGlobalBottomSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "04_mouse_drag_global_bottom.png"));

                    ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview");
                    AssertDockingVerificationStage(shellHost, window, report, "05_mouse_drag_global_left_setup", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "05_mouse_drag_global_left_setup.png"));
                    DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "05_mouse_drag_global_left_input",
                        "HSV_Preview",
                        CreateGlobalMouseDragTarget(shellHost.DockedLayerVisualSnapshotForTest, DockingGuideZone.GlobalLeft));
                    AssertDockingVerificationStage(shellHost, window, report, "06_mouse_drag_global_left", 2, 2, "Horizontal", 2);
                    AssertGlobalLeftSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "06_mouse_drag_global_left.png"));

                    ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview");
                    AssertDockingVerificationStage(shellHost, window, report, "07_mouse_drag_global_top_setup", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "07_mouse_drag_global_top_setup.png"));
                    DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "07_mouse_drag_global_top_input",
                        "HSV_Preview",
                        CreateGlobalMouseDragTarget(shellHost.DockedLayerVisualSnapshotForTest, DockingGuideZone.GlobalTop));
                    AssertDockingVerificationStage(shellHost, window, report, "08_mouse_drag_global_top", 2, 2, "Vertical", 2);
                    AssertGlobalTopSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "08_mouse_drag_global_top.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_MouseLocalBottom", "MB", 71);
                    SetupPaneLocalSideVerification(shellHost, "Dock_MouseLocalBottom");
                    AssertDockingVerificationStage(shellHost, window, report, "09_mouse_drag_local_bottom_setup", 3, 2, "Horizontal", 2);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "09_mouse_drag_local_bottom_setup.png"));
                    Action dragLocalBottom = () => DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "09_mouse_drag_local_bottom_input",
                        "Dock_MouseLocalBottom",
                        CreatePaneCompassMouseDragTarget(
                            shellHost.DockedLayerVisualSnapshotForTest,
                            "Main",
                            DockingGuideZone.Bottom,
                            "mouse drag local bottom"));
                    dragLocalBottom();
                    AssertDockingVerificationStageAfterMouseDrag(shellHost, window, report, "10_mouse_drag_local_bottom", 3, 3, "Horizontal", 3, dragLocalBottom);
                    AssertPaneLocalBottomSemantics(shellHost, window, "Dock_MouseLocalBottom", "Main", "HSV_Preview");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "10_mouse_drag_local_bottom.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_MouseLocalLeft", "ML", 72);
                    SetupPaneLocalSideVerification(shellHost, "Dock_MouseLocalLeft");
                    AssertDockingVerificationStage(shellHost, window, report, "11_mouse_drag_local_left_setup", 3, 2, "Horizontal", 2);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "11_mouse_drag_local_left_setup.png"));
                    Action dragLocalLeft = () => DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "11_mouse_drag_local_left_input",
                        "Dock_MouseLocalLeft",
                        CreatePaneCompassMouseDragTarget(
                            shellHost.DockedLayerVisualSnapshotForTest,
                            "Main",
                            DockingGuideZone.Left,
                            "mouse drag local left"));
                    dragLocalLeft();
                    AssertDockingVerificationStageAfterMouseDrag(shellHost, window, report, "12_mouse_drag_local_left", 3, 3, "Horizontal", 3, dragLocalLeft);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_MouseLocalLeft", "Main", "HSV_Preview", "Left");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "12_mouse_drag_local_left.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_MouseLocalRight", "MR", 73);
                    SetupPaneLocalSideVerification(shellHost, "Dock_MouseLocalRight");
                    AssertDockingVerificationStage(shellHost, window, report, "13_mouse_drag_local_right_setup", 3, 2, "Horizontal", 2);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "13_mouse_drag_local_right_setup.png"));
                    Action dragLocalRight = () => DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "13_mouse_drag_local_right_input",
                        "Dock_MouseLocalRight",
                        CreatePaneCompassMouseDragTarget(
                            shellHost.DockedLayerVisualSnapshotForTest,
                            "Main",
                            DockingGuideZone.Right,
                            "mouse drag local right"));
                    dragLocalRight();
                    AssertDockingVerificationStageAfterMouseDrag(shellHost, window, report, "14_mouse_drag_local_right", 3, 3, "Horizontal", 3, dragLocalRight);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_MouseLocalRight", "Main", "HSV_Preview", "Right");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "14_mouse_drag_local_right.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_MouseLocalTop", "MT", 74);
                    SetupPaneLocalSideVerification(shellHost, "Dock_MouseLocalTop");
                    AssertDockingVerificationStage(shellHost, window, report, "15_mouse_drag_local_top_setup", 3, 2, "Horizontal", 2);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "15_mouse_drag_local_top_setup.png"));
                    Action dragLocalTop = () => DragDockedLayerHeaderToWorkspacePoint(
                        shellHost,
                        report,
                        "15_mouse_drag_local_top_input",
                        "Dock_MouseLocalTop",
                        CreatePaneCompassMouseDragTarget(
                            shellHost.DockedLayerVisualSnapshotForTest,
                            "Main",
                            DockingGuideZone.Top,
                            "mouse drag local top"));
                    dragLocalTop();
                    AssertDockingVerificationStageAfterMouseDrag(shellHost, window, report, "16_mouse_drag_local_top", 3, 3, "Horizontal", 3, dragLocalTop);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_MouseLocalTop", "Main", "HSV_Preview", "Top");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "16_mouse_drag_local_top.png"));

                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: layer-docking-mouse-drag" + Environment.NewLine
                        + report,
                        Encoding.UTF8);
                }
                catch
                {
                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: FAIL" + Environment.NewLine
                        + "Scenario: layer-docking-mouse-drag" + Environment.NewLine
                        + report,
                        Encoding.UTF8);
                    throw;
                }
                finally
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    app.Shutdown();
                }
            });
        }

        private static void RunLayerDockingTabClickNoGuide(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            WithDockingStateFileBackup(() =>
            {
                Application app = Application.Current ?? new Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

                OpenVisionShellHostWindow window = null;
                StringBuilder report = new StringBuilder();
                try
                {
                    ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                    window = new OpenVisionShellHostWindow(runtimeContext)
                    {
                        Width = 1600,
                        Height = 900,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    app.MainWindow = window;
                    window.Show();
                    Pump(24);

                    OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                        ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                    PrepareDockingVerificationLayers(shellHost);
                    EnsureDockingVerificationLayer(shellHost, "Matching_Preview_002", "T2", 75);
                    ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview", "Matching_Preview_002");
                    AssertDockingVerificationStage(shellHost, window, report, "01_tab_click_setup", 3, 1, null, 3);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_tab_click_setup.png"));

                    ClickDockedLayerHeaderWithoutDrag(shellHost, report, "02_click_hsv_tab_no_guide", "HSV_Preview");
                    ClickDockedLayerHeaderWithoutDrag(shellHost, report, "03_click_extra_tab_no_guide", "Matching_Preview_002");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_after_tab_clicks_no_guide.png"));

                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: layer-docking-tab-click-no-guide" + Environment.NewLine
                        + report,
                        Encoding.UTF8);
                }
                catch
                {
                    File.WriteAllText(
                        Path.Combine(outputDirectory, "report.txt"),
                        "Result: FAIL" + Environment.NewLine
                        + "Scenario: layer-docking-tab-click-no-guide" + Environment.NewLine
                        + report,
                        Encoding.UTF8);
                    throw;
                }
                finally
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    app.Shutdown();
                }
            });
        }

        private static void RunLayerDockingVerification(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            WithDockingStateFileBackup(() =>
            {
                Application app = Application.Current ?? new Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

                OpenVisionShellHostWindow window = null;
                StringBuilder report = new StringBuilder();
                try
                {
                    ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                    window = new OpenVisionShellHostWindow(runtimeContext)
                    {
                        Width = 1600,
                        Height = 900,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    app.MainWindow = window;
                    window.Show();
                    Pump(24);

                    OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                        ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                    PrepareDockingVerificationLayers(shellHost);

                    if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
                    {
                        throw new InvalidOperationException("Docking verification setup could not dock Main and HSV_Preview.");
                    }

                    Pump(28);
                    AssertDockingVerificationStage(shellHost, window, report, "01_tabbed_top_headers", 2, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "01_tabbed_top_headers.png"));

                    if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
                    {
                        throw new InvalidOperationException("Docking verification could not move HSV_Preview to GlobalRight.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "02_global_right_workspace_split", 2, 2, "Horizontal", 2);
                    AssertGlobalRightSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "02_global_right_workspace_split.png"));

                    if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
                    {
                        throw new InvalidOperationException("Docking verification could not merge HSV_Preview back before GlobalBottom.");
                    }

                    Pump(18);
                    if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalBottom"))
                    {
                        throw new InvalidOperationException("Docking verification could not move HSV_Preview to GlobalBottom.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "03_global_bottom_full_width", 2, 2, "Vertical", 2);
                    AssertGlobalBottomSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "03_global_bottom_full_width.png"));

                    if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
                    {
                        throw new InvalidOperationException("Docking verification could not merge HSV_Preview back before local bottom.");
                    }

                    Pump(18);
                    using (Bitmap localBottomBitmap = CreateDockingVerificationBitmap("LOCAL", 42))
                    {
                        if (!shellHost.AddLayerImageForTest("Dock_LocalBottom", localBottomBitmap)
                            || !shellHost.DockLayerForTest("Dock_LocalBottom"))
                        {
                            throw new InvalidOperationException("Docking verification could not create Dock_LocalBottom.");
                        }
                    }

                    Pump(24);
                    if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
                    {
                        throw new InvalidOperationException("Docking verification could not create the right pane before local bottom.");
                    }

                    Pump(24);
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalBottom", "Bottom"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_LocalBottom to pane-local Bottom.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "04_pane_local_bottom_nested", 3, 3, "Horizontal", 3);
                    AssertPaneLocalBottomSemantics(shellHost, window, "Dock_LocalBottom", "Main", "HSV_Preview");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "04_pane_local_bottom_nested.png"));

                    shellHost.SaveDockingWorkspaceStateForTest();
                    Pump(12);
                    if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview")
                        || !shellHost.MoveDockedLayerToPrimaryPaneForTest("Dock_LocalBottom"))
                    {
                        throw new InvalidOperationException("Docking verification could not flatten the current layout before restore.");
                    }

                    Pump(24);
                    AssertDockingVerificationStage(shellHost, window, report, "05_flatten_before_restore", 3, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "05_flatten_before_restore.png"));

                    if (!shellHost.RestoreDockingLayoutStateForTest())
                    {
                        throw new InvalidOperationException("Docking verification restore command returned false.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "06_restore_pane_local_bottom", 3, 3, "Horizontal", 3);
                    AssertPaneLocalBottomSemantics(shellHost, window, "Dock_LocalBottom", "Main", "HSV_Preview");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "06_restore_pane_local_bottom.png"));

                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalBottom", "Center"))
                    {
                        throw new InvalidOperationException("Docking verification could not merge Dock_LocalBottom back as a center tab.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "07_center_tab_merge", 3, 2, "Horizontal", 2);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "07_center_tab_merge.png"));

                    if (!shellHost.DockLayerToGuideZoneForTest("Main", "GlobalRight"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Main after a center-tab merge.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "08_move_main_after_center_merge", 3, 3, "Horizontal", 3);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "08_move_main_after_center_merge.png"));

                    if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalBottom"))
                    {
                        throw new InvalidOperationException("Docking verification could not move HSV_Preview after Main was moved.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "09_move_other_after_main_global_bottom", 3, 3, "Vertical", 3);
                    AssertGlobalBottomSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "09_move_other_after_main_global_bottom.png"));

                    shellHost.SaveDockingWorkspaceStateForTest();
                    CopyCurrentDockingStateFile("LayerDocking.layout", outputDirectory, "09_saved_layout.txt");
                    CopyCurrentDockingStateFile("LayerDocking.layers", outputDirectory, "09_saved_layers.txt");
                    Pump(12);
                    if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("Main")
                        || !shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview")
                        || !shellHost.MoveDockedLayerToPrimaryPaneForTest("Dock_LocalBottom"))
                    {
                        throw new InvalidOperationException("Docking verification could not flatten the post-move layout before restore.");
                    }

                    Pump(24);
                    AssertDockingVerificationStage(shellHost, window, report, "10_flatten_after_repeated_moves", 3, 1, null, 1);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "10_flatten_after_repeated_moves.png"));

                    if (!shellHost.RestoreDockingLayoutStateForTest())
                    {
                        throw new InvalidOperationException("Docking verification post-move restore command returned false.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "11_restore_after_repeated_moves", 3, 3, "Vertical", 3);
                    AssertGlobalBottomSemantics(shellHost, window, "HSV_Preview", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "11_restore_after_repeated_moves.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_GlobalLeft", "LEFT", 51);
                    ResetDockingVerificationDocuments(shellHost, "Main", "Dock_GlobalLeft");
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_GlobalLeft", "GlobalLeft"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_GlobalLeft to GlobalLeft.");
                    }

                    Pump(32);
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "12_global_left_workspace_split_preassert.png"));
                    AssertDockingVerificationStage(shellHost, window, report, "12_global_left_workspace_split", 2, 2, "Horizontal", 2);
                    AssertGlobalLeftSemantics(shellHost, window, "Dock_GlobalLeft", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "12_global_left_workspace_split.png"));

                    ResetDockingVerificationDocuments(shellHost, "Main", "Dock_GlobalLeft");
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_GlobalLeft", "GlobalTop"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_GlobalLeft to GlobalTop.");
                    }

                    Pump(32);
                    AssertDockingVerificationStage(shellHost, window, report, "13_global_top_workspace_split", 2, 2, "Vertical", 2);
                    AssertGlobalTopSemantics(shellHost, window, "Dock_GlobalLeft", "Main");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "13_global_top_workspace_split.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_LocalLeft", "L", 61);
                    SetupPaneLocalSideVerification(shellHost, "Dock_LocalLeft");
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalLeft", "Left"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_LocalLeft to pane-local Left.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "14_pane_local_left_nested", 3, 3, "Horizontal", 3);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_LocalLeft", "Main", "HSV_Preview", "Left");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "14_pane_local_left_nested.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_LocalRight", "R", 62);
                    SetupPaneLocalSideVerification(shellHost, "Dock_LocalRight");
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalRight", "Right"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_LocalRight to pane-local Right.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "15_pane_local_right_nested", 3, 3, "Horizontal", 3);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_LocalRight", "Main", "HSV_Preview", "Right");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "15_pane_local_right_nested.png"));

                    EnsureDockingVerificationLayer(shellHost, "Dock_LocalTop", "TOP", 63);
                    SetupPaneLocalSideVerification(shellHost, "Dock_LocalTop");
                    if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalTop", "Top"))
                    {
                        throw new InvalidOperationException("Docking verification could not move Dock_LocalTop to pane-local Top.");
                    }

                    Pump(36);
                    AssertDockingVerificationStage(shellHost, window, report, "16_pane_local_top_nested", 3, 3, "Horizontal", 3);
                    AssertPaneLocalSideSemantics(shellHost, window, "Dock_LocalTop", "Main", "HSV_Preview", "Top");
                    SaveWindowScreenshot(window, Path.Combine(outputDirectory, "16_pane_local_top_nested.png"));

                    report.Insert(0,
                        "Result: PASS" + Environment.NewLine
                        + "Scenario: layer-docking-verification" + Environment.NewLine
                        + "Screenshots: 01_tabbed_top_headers.png, 02_global_right_workspace_split.png, 03_global_bottom_full_width.png, 04_pane_local_bottom_nested.png, 05_flatten_before_restore.png, 06_restore_pane_local_bottom.png, 07_center_tab_merge.png, 08_move_main_after_center_merge.png, 09_move_other_after_main_global_bottom.png, 10_flatten_after_repeated_moves.png, 11_restore_after_repeated_moves.png, 12_global_left_workspace_split.png, 13_global_top_workspace_split.png, 14_pane_local_left_nested.png, 15_pane_local_right_nested.png, 16_pane_local_top_nested.png"
                        + Environment.NewLine
                        + Environment.NewLine);
                    File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report.ToString(), Encoding.UTF8);
                }
                finally
                {
                    if (window != null)
                    {
                        window.Close();
                    }

                    app.Shutdown();
                }
            });
        }

        private static void RunPropertyGridRoiEditor(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string pinsPath = Path.Combine(repoRoot, "Sample", "EasyGauge", "Pins.bmp");
            if (!File.Exists(pinsPath))
            {
                throw new FileNotFoundException("Pin measurement sample image was not found.", pinsPath);
            }

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1280,
                    Height = 820,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap pinsBitmap = new Bitmap(pinsPath))
                {
                    shellHost.SetMainLayerImageForTest(pinsBitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.Line);
                Pump(24);

                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Line tool did not keep Main as the active input layer. Input="
                        + shellHost.ActiveNativeRouteInputLayerNameForTest);
                }

                PropertyGridEditorRuntime editorRuntime = GetPropertyGridEditorRuntime();
                using OpenCvSharp.Mat sourceImage = editorRuntime.ImageEditorService.GetSourceImage();
                if (sourceImage == null || sourceImage.Empty())
                {
                    throw new InvalidOperationException("PropertyGrid ROI editor source image was empty.");
                }

                bool clickedActualPropertyGridButton = ClickPropertyGridRoiButton(app, window);

                FakePropertyItemValueInner roiValue = new FakePropertyItemValueInner
                {
                    Value = new OpenCvSharp.Rect()
                };
                PropertyItemValue propertyItemValue = CreateBridgePropertyItemValue(roiValue);

                DispatcherTimer closeTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
                {
                    Interval = TimeSpan.FromMilliseconds(150)
                };
                closeTimer.Tick += (sender, e) =>
                {
                    foreach (Window openWindow in app.Windows)
                    {
                        if (openWindow is RoiEditorWindow)
                        {
                            AcceptDialogWindow(openWindow);
                            closeTimer.Stop();
                            break;
                        }
                    }
                };
                closeTimer.Start();
                new PropertyGridEditorFactory.WpgROIEditor().ShowDialog(propertyItemValue, window);
                closeTimer.Stop();

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: property-grid-roi-editor" + Environment.NewLine
                    + "InputLayer: " + shellHost.ActiveNativeRouteInputLayerNameForTest + Environment.NewLine
                    + "ClickedActualPropertyGridButton: " + clickedActualPropertyGridButton.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "SourceSize: " + sourceImage.Width.ToString(CultureInfo.InvariantCulture)
                    + "x" + sourceImage.Height.ToString(CultureInfo.InvariantCulture) + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunMatchingVsEdgeBasedScaleComparison(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string samplePath = Path.Combine(repoRoot, "bin", "Debug", "EasyMatch", "BOARD.JPG");
            if (!File.Exists(samplePath))
            {
                throw new FileNotFoundException("EasyMatch BOARD sample image was not found.", samplePath);
            }

            string templatePath;
            string scaledSourcePath;
            OpenCvSharp.Rect templateRect;
            const double expectedScale = 0.90D;
            CreateEasyMatchScaleSmokeFiles(samplePath, outputDirectory, expectedScale, out templatePath, out scaledSourcePath, out templateRect);
            double expectedCenterX = (templateRect.X + (templateRect.Width / 2D)) * expectedScale;
            double expectedCenterY = (templateRect.Y + (templateRect.Height / 2D)) * expectedScale;

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = "Codex_MatchingVsEdgeScaleSmoke";

                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1500,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap scaledBitmap = new Bitmap(scaledSourcePath))
                {
                    shellHost.SetMainLayerImageForTest(scaledBitmap);
                }

                Pump(24);

                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(60);
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveMatchingForTest(ConfigureImageMatchingScaleComparisonProperty);
                Pump(12);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);

                string imageStatus = shellHost.ActiveNativeStatusText;
                string imageReview = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || imageReview.IndexOf("Template Match", StringComparison.OrdinalIgnoreCase) < 0
                    || imageReview.IndexOf("Scale", StringComparison.OrdinalIgnoreCase) < 0
                    || imageReview.IndexOf("0.9", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "Image Matching scale comparison preview did not complete."
                        + Environment.NewLine
                        + "Status: " + imageStatus
                        + Environment.NewLine
                        + "Review: " + imageReview);
                }

                using (Bitmap preview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
                {
                    if (preview != null)
                    {
                        preview.Save(Path.Combine(outputDirectory, "Matching_Preview.png"));
                    }
                }

                shellHost.SelectToolForTest(VISION_MENU.EdgeBasedMatching);
                Pump(60);
                shellHost.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveEdgeBasedMatchingForTest(ConfigureEdgeBasedScaleSmokeProperty);
                Pump(12);
                shellHost.RunActiveNativePreviewForTest();
                Pump(120);

                string edgeStatus = shellHost.ActiveNativeStatusText;
                string edgeReview = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || edgeReview.IndexOf("Edge Match", StringComparison.OrdinalIgnoreCase) < 0
                    || edgeReview.IndexOf("Scale", StringComparison.OrdinalIgnoreCase) < 0
                    || edgeReview.IndexOf("0.9", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "EdgeBased scale comparison preview did not complete."
                        + Environment.NewLine
                        + "Status: " + edgeStatus
                        + Environment.NewLine
                        + "Review: " + edgeReview);
                }

                using (Bitmap preview = shellHost.GetLayerImageCloneForTest("EdgeBasedMatching_Preview"))
                {
                    if (preview != null)
                    {
                        preview.Save(Path.Combine(outputDirectory, "EdgeBasedMatching_Preview.png"));
                    }
                }

                string imageCenterError = FormatCenterError(imageReview, expectedCenterX, expectedCenterY);
                string edgeCenterError = FormatCenterError(edgeReview, expectedCenterX, expectedCenterY);
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingVsEdgeBasedScale.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: matching-vs-edge-based-scale-comparison" + Environment.NewLine
                    + "ExpectedScale: " + expectedScale.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ExpectedCenter: " + expectedCenterX.ToString("0.#", CultureInfo.InvariantCulture)
                    + "," + expectedCenterY.ToString("0.#", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ImageMatchingStatus: " + imageStatus + Environment.NewLine
                    + "ImageMatchingReview: " + imageReview + Environment.NewLine
                    + "ImageMatchingCenterErrorPx: " + imageCenterError + Environment.NewLine
                    + "EdgeBasedStatus: " + edgeStatus + Environment.NewLine
                    + "EdgeBasedReview: " + edgeReview + Environment.NewLine
                    + "EdgeBasedCenterErrorPx: " + edgeCenterError + Environment.NewLine
                    + "Template: " + templatePath + Environment.NewLine
                    + "ScaledSource: " + scaledSourcePath + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunMatchingPyramidScale(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string samplePath = Path.Combine(repoRoot, "bin", "Debug", "EasyMatch", "BOARD.JPG");
            if (!File.Exists(samplePath))
            {
                throw new FileNotFoundException("EasyMatch BOARD sample image was not found.", samplePath);
            }

            string templatePath;
            string scaledSourcePath;
            OpenCvSharp.Rect templateRect;
            const double expectedScale = 0.90D;
            CreateEasyMatchScaleSmokeFiles(samplePath, outputDirectory, expectedScale, out templatePath, out scaledSourcePath, out templateRect);
            double expectedCenterX = (templateRect.X + (templateRect.Width / 2D)) * expectedScale;
            double expectedCenterY = (templateRect.Y + (templateRect.Height / 2D)) * expectedScale;

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = "Codex_MatchingPyramidScaleSmoke";

                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1500,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap scaledBitmap = new Bitmap(scaledSourcePath))
                {
                    shellHost.SetMainLayerImageForTest(scaledBitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(60);
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveMatchingForTest(ConfigureImageMatchingPyramidScaleProperty);
                Pump(12);

                MatchingProperty property = runtimeContext.Global.VisionTools.Matchings.Count > 0
                    ? runtimeContext.Global.VisionTools.Matchings[0]
                    : throw new InvalidOperationException("Matching_1 repository property was not created.");

                string xmlPath = Path.Combine(outputDirectory, "Matching_1.pyramid-scale-smoke.xml");
                property.SaveTestConfig(xmlPath);
                MatchingProperty loadedProperty = new MatchingProperty("Matching_1").LoadTestConfig(xmlPath);
                ValidateImageMatchingPyramidScaleProperty(loadedProperty);

                shellHost.RunActiveNativePreviewForTest();
                Pump(120);

                string status = shellHost.ActiveNativeStatusText;
                string review = shellHost.ActiveNativeResultReviewText;
                if (!shellHost.HasNativePreviewResult
                    || review.IndexOf("Template Match", StringComparison.OrdinalIgnoreCase) < 0
                    || review.IndexOf("Scale", StringComparison.OrdinalIgnoreCase) < 0
                    || review.IndexOf("0.9", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "Image Matching pyramid scale preview did not report the expected scale result."
                        + Environment.NewLine
                        + "Status: " + status
                        + Environment.NewLine
                        + "Review: " + review);
                }

                double centerError = CalculateCenterError(review, expectedCenterX, expectedCenterY);
                if (centerError > 2D)
                {
                    throw new InvalidOperationException(
                        "Image Matching pyramid scale center error was too large. Error="
                        + centerError.ToString("0.###", CultureInfo.InvariantCulture)
                        + ", Review=" + review);
                }

                using (Bitmap preview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
                {
                    if (preview == null)
                    {
                        throw new InvalidOperationException("Matching_Preview output layer was not created.");
                    }

                    preview.Save(Path.Combine(outputDirectory, "Matching_Preview.png"));
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingPyramidScale.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: matching-pyramid-scale" + Environment.NewLine
                    + "ExpectedScale: " + expectedScale.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ExpectedCenter: " + expectedCenterX.ToString("0.#", CultureInfo.InvariantCulture)
                    + "," + expectedCenterY.ToString("0.#", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Status: " + status + Environment.NewLine
                    + "Review: " + review + Environment.NewLine
                    + "CenterErrorPx: " + centerError.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PyramidProposalEnabled: "
                    + loadedProperty.USE_PYRAMID_POSITION_PROPOSAL.ToString() + Environment.NewLine
                    + "PyramidProposalTopN: "
                    + loadedProperty.PYRAMID_POSITION_TOP_N.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "PyramidProposalMinScore: "
                    + loadedProperty.PYRAMID_POSITION_MIN_SCORE.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Template: " + templatePath + Environment.NewLine
                    + "ScaledSource: " + scaledSourcePath + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static void RunMatchingC9Batch(string[] args, string outputDirectory)
        {
            if (Directory.Exists(outputDirectory)
                && Directory.EnumerateFileSystemEntries(outputDirectory).Any())
            {
                throw new InvalidOperationException(
                    "Matching C9 batch output must be a new or empty directory: " + outputDirectory);
            }

            Directory.CreateDirectory(outputDirectory);
            string sourceRoot = ResolveRequiredOption(args, "--source-root");
            string p174RowsPath = ResolveRequiredOption(args, "--rows");
            string p174TransformsPath = ResolveRequiredOption(args, "--transforms");
            string p173StripMetricsPath = ResolveRequiredOption(args, "--strip-metrics");
            string p174TemplatePath = ResolveRequiredOption(args, "--template");
            string p174ReferencePath = ResolveRequiredOption(args, "--reference");
            string p174ReferenceOverlayPath = ResolveRequiredOption(args, "--reference-overlay");
            if (!Directory.Exists(sourceRoot))
            {
                throw new DirectoryNotFoundException("Matching C9 source root was not found: " + sourceRoot);
            }

            foreach (string requiredPath in new[]
            {
                p174RowsPath,
                p174TransformsPath,
                p173StripMetricsPath,
                p174TemplatePath,
                p174ReferencePath,
                p174ReferenceOverlayPath
            })
            {
                if (!File.Exists(requiredPath))
                {
                    throw new FileNotFoundException("P174 C9 evidence input was not found.", requiredPath);
                }
            }

            string referenceDirectory = Path.Combine(outputDirectory, "reference");
            string calibrationDirectory = Path.Combine(outputDirectory, "calibration");
            string runsDirectory = Path.Combine(outputDirectory, "runs");
            string contactsDirectory = Path.Combine(outputDirectory, "contacts");
            Directory.CreateDirectory(referenceDirectory);
            Directory.CreateDirectory(calibrationDirectory);
            Directory.CreateDirectory(runsDirectory);
            Directory.CreateDirectory(contactsDirectory);

            string templatePath = Path.Combine(referenceDirectory, "locator_template.png");
            File.Copy(p174TemplatePath, templatePath, true);
            File.Copy(p174ReferencePath, Path.Combine(referenceDirectory, "operator_reference_ok0001.jpg"), true);
            File.Copy(p174ReferenceOverlayPath, Path.Combine(referenceDirectory, "locator_and_measurement_roi_overlay.png"), true);
            File.Copy(p174RowsPath, Path.Combine(referenceDirectory, "p174_rows.csv"), true);
            File.Copy(p174TransformsPath, Path.Combine(referenceDirectory, "p174_transforms.csv"), true);
            File.Copy(p173StripMetricsPath, Path.Combine(referenceDirectory, "p173_strip_metrics.csv"), true);

            const string expectedTemplateSha256 = "BA09B78D79D3A2936504B04FE70DDA066021754395772E49465B9F2BA192D9D2";
            string templateSha256 = ComputeC9FileSha256(templatePath);
            if (!string.Equals(templateSha256, expectedTemplateSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "C9 template hash changed. Expected=" + expectedTemplateSha256 + ", Actual=" + templateSha256);
            }

            List<Dictionary<string, string>> sourceRows = ReadC9Csv(p174RowsPath);
            if (sourceRows.Count != 24)
            {
                throw new InvalidOperationException("P174 C9 native batch requires exactly 24 source rows. Actual=" + sourceRows.Count);
            }

            Dictionary<string, Dictionary<string, string>> transformsByImage = ReadC9Csv(p174TransformsPath)
                .ToDictionary(row => row["image"], row => row, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Dictionary<string, string>> stripMetricsByImage = ReadC9Csv(p173StripMetricsPath)
                .ToDictionary(row => row["image"], row => row, StringComparer.OrdinalIgnoreCase);
            foreach (Dictionary<string, string> sourceRow in sourceRows)
            {
                if (!transformsByImage.ContainsKey(sourceRow["Image"]))
                {
                    throw new InvalidOperationException("P174 transform is missing for " + sourceRow["Image"]);
                }

                if (!stripMetricsByImage.ContainsKey(sourceRow["Image"]))
                {
                    throw new InvalidOperationException("P173 local strip-angle metric is missing for " + sourceRow["Image"]);
                }
            }

            RecipeWorkspaceService.DeleteVisionWorkspace("Codex_MatchingC9BatchSmoke");
            CleanupKnownSmokeRecipeWorkspaces("Smoke_MatchingC9_");
            string recipeName = "Smoke_MatchingC9_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            List<MatchingC9CalibrationRow> calibrationRows = new List<MatchingC9CalibrationRow>();
            List<MatchingC9EvidenceRow> evidenceRows = new List<MatchingC9EvidenceRow>();
            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1500,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(60);
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveMatchingForTest(ConfigureMatchingC9Property);
                Pump(12);

                MatchingProperty property = runtimeContext.Global.VisionTools.Matchings.Count > 0
                    ? runtimeContext.Global.VisionTools.Matchings[0]
                    : throw new InvalidOperationException("Matching_1 repository property was not created.");
                string propertyXmlPath = Path.Combine(referenceDirectory, "Matching_1.c9-native.xml");
                property.SaveTestConfig(propertyXmlPath);
                MatchingProperty loadedProperty = new MatchingProperty("Matching_1").LoadTestConfig(propertyXmlPath);
                ValidateMatchingC9Property(loadedProperty);

                MatchingC9SyntheticCase[] syntheticCases =
                {
                    new MatchingC9SyntheticCase("scale_0p8_angle_m3", 0.8D, -3D),
                    new MatchingC9SyntheticCase("scale_1p0_angle_0", 1.0D, 0D),
                    new MatchingC9SyntheticCase("scale_1p8_angle_p3", 1.8D, 3D)
                };
                foreach (MatchingC9SyntheticCase syntheticCase in syntheticCases)
                {
                    string caseDirectory = Path.Combine(calibrationDirectory, syntheticCase.Name);
                    Directory.CreateDirectory(caseDirectory);
                    string sourcePath = Path.Combine(caseDirectory, "source.png");
                    CreateMatchingC9SyntheticSource(templatePath, sourcePath, syntheticCase.Scale, syntheticCase.Angle, 320D, 240D);
                    MatchingC9PreviewCapture capture = CaptureMatchingC9Preview(
                        shellHost,
                        sourcePath,
                        caseDirectory,
                        syntheticCase.Name,
                        320D,
                        240D,
                        Array.Empty<System.Drawing.PointF>());
                    calibrationRows.Add(new MatchingC9CalibrationRow(syntheticCase, capture));
                }

                bool calibrationPassed = calibrationRows.All(row => row.Pass);

                foreach (Dictionary<string, string> sourceRow in sourceRows)
                {
                    string rowId = sourceRow["RowId"];
                    string sourcePath = Path.GetFullPath(Path.Combine(sourceRoot, sourceRow["CopiedSourcePath"].Replace('/', Path.DirectorySeparatorChar)));
                    if (!File.Exists(sourcePath))
                    {
                        throw new FileNotFoundException("P174 copied source was not found.", sourcePath);
                    }

                    string actualSourceSha256 = ComputeC9FileSha256(sourcePath);
                    string expectedSourceSha256 = sourceRow["SourceSha256"];
                    if (!string.Equals(actualSourceSha256, expectedSourceSha256, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "P174 source hash changed for " + rowId + ". Expected=" + expectedSourceSha256 + ", Actual=" + actualSourceSha256);
                    }

                    Dictionary<string, string> transform = transformsByImage[sourceRow["Image"]];
                    double oracleScale = ParseC9Double(transform["scale"]);
                    double oracleAngle = ParseC9Double(transform["angle_deg"]);
                    double stripAngle = ParseC9Double(stripMetricsByImage[sourceRow["Image"]]["angle_deg"]);
                    double tx = ParseC9Double(transform["tx"]);
                    double ty = ParseC9Double(transform["ty"]);
                    System.Drawing.PointF[] expectedPolygon = CreateMatchingC9ExpectedPolygon(oracleScale, oracleAngle, tx, ty);
                    double expectedCenterX = ParseC9Double(sourceRow["ConsensusCenterX"]);
                    double expectedCenterY = ParseC9Double(sourceRow["ConsensusCenterY"]);
                    string rowDirectory = Path.Combine(runsDirectory, rowId);
                    Directory.CreateDirectory(rowDirectory);
                    string copiedSourcePath = Path.Combine(rowDirectory, "source.jpg");
                    File.Copy(sourcePath, copiedSourcePath, true);

                    MatchingC9PreviewCapture capture = CaptureMatchingC9Preview(
                        shellHost,
                        sourcePath,
                        rowDirectory,
                        rowId,
                        expectedCenterX,
                        expectedCenterY,
                        expectedPolygon);
                    MatchingC9EvidenceRow evidenceRow = new MatchingC9EvidenceRow(
                        sourceRow,
                        transform,
                        capture,
                        expectedCenterX,
                        expectedCenterY,
                        oracleScale,
                        oracleAngle,
                        stripAngle,
                        expectedPolygon,
                        actualSourceSha256,
                        rowDirectory);
                    evidenceRows.Add(evidenceRow);
                    DrawMatchingC9EvidenceOverlay(evidenceRow);
                }

                foreach (IGrouping<string, MatchingC9EvidenceRow> split in evidenceRows.GroupBy(row => row.Split))
                {
                    SaveMatchingC9ContactSheet(
                        split.OrderBy(row => row.RoleLabelOnly, StringComparer.Ordinal).ThenBy(row => row.Image, StringComparer.Ordinal),
                        Path.Combine(contactsDirectory, split.Key + ".png"));
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingC9Batch_Final.png"));
                WriteMatchingC9Outputs(
                    outputDirectory,
                    templateSha256,
                    calibrationRows,
                    calibrationPassed,
                    evidenceRows);

                if (!calibrationPassed || evidenceRows.Any(row => !row.Pass))
                {
                    throw new InvalidOperationException(
                        "Matching C9 native batch did not satisfy every gate. CalibrationPass="
                        + calibrationPassed
                        + ", DatasetPass="
                        + evidenceRows.Count(row => row.Pass)
                        + "/"
                        + evidenceRows.Count
                        + ". See native_rows.csv and current-run overlays.");
                }
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static void RunMatchingDiePadBatch(string[] args, string outputDirectory)
        {
            if (Directory.Exists(outputDirectory)
                && Directory.EnumerateFileSystemEntries(outputDirectory).Any())
            {
                throw new InvalidOperationException(
                    "Matching Die Pad batch output must be a new or empty directory: " + outputDirectory);
            }

            Directory.CreateDirectory(outputDirectory);
            string datasetRoot = ResolveRequiredOption(args, "--dataset-root");
            const string sourceFile = "Die Pad 1.bmp";
            string referenceImagePath = ResolveRequiredOption(args, "--reference");
            string profile = ResolveOptionalTextOption(args, "--profile");
            bool zeroReferenceProfile = string.Equals(profile, "zero-reference", StringComparison.OrdinalIgnoreCase);
            bool objectOnlyZeroReferenceProfile = string.Equals(profile, "object-only-zero-reference", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(profile) && !zeroReferenceProfile && !objectOnlyZeroReferenceProfile)
            {
                throw new ArgumentException("Unsupported Matching Die Pad profile: " + profile);
            }

            zeroReferenceProfile = zeroReferenceProfile || objectOnlyZeroReferenceProfile;
            profile = objectOnlyZeroReferenceProfile
                ? "object-only-zero-reference"
                : zeroReferenceProfile ? "zero-reference" : "legacy-p176";
            string metadataPath = Path.Combine(datasetRoot, "metadata.csv");
            string generatorPath = Path.Combine(datasetRoot, "scripts", "generate_dataset.py");
            foreach (string requiredPath in new[] { metadataPath, generatorPath, referenceImagePath })
            {
                if (!File.Exists(requiredPath))
                {
                    throw new FileNotFoundException("Matching Die Pad evidence input was not found.", requiredPath);
                }
            }

            List<Dictionary<string, string>> rows = ReadC9Csv(metadataPath)
                .Where(row => string.Equals(row["source_file"], sourceFile, StringComparison.OrdinalIgnoreCase))
                .OrderBy(row => MatchingDiePadSplitOrder(row["detection_segmentation_split"]))
                .ThenBy(row => row["status"], StringComparer.Ordinal)
                .ThenBy(row => row["filename"], StringComparer.Ordinal)
                .ToList();
            if (rows.Count != 122
                || rows.Count(row => string.Equals(row["status"], "OK", StringComparison.OrdinalIgnoreCase)) != 62
                || rows.Count(row => string.Equals(row["status"], "NG", StringComparison.OrdinalIgnoreCase)) != 60)
            {
                throw new InvalidOperationException(
                    "Matching Die Pad 1 subset identity changed. Expected 122 rows (OK 62 / NG 60). Actual="
                    + rows.Count
                    + " (OK "
                    + rows.Count(row => string.Equals(row["status"], "OK", StringComparison.OrdinalIgnoreCase))
                    + " / NG "
                    + rows.Count(row => string.Equals(row["status"], "NG", StringComparison.OrdinalIgnoreCase))
                    + ").");
            }

            string referenceFileName = Path.GetFileName(referenceImagePath);
            if (!rows.Any(row => string.Equals(row["filename"], referenceFileName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(row["status"], "OK", StringComparison.OrdinalIgnoreCase)
                && string.Equals(row["detection_segmentation_split"], "train", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Matching Die Pad reference must be an OK/train row in the selected source family.");
            }

            string referenceDirectory = Path.Combine(outputDirectory, "reference");
            string runsDirectory = Path.Combine(outputDirectory, "runs");
            string contactsDirectory = Path.Combine(outputDirectory, "contacts");
            Directory.CreateDirectory(referenceDirectory);
            Directory.CreateDirectory(runsDirectory);
            Directory.CreateDirectory(contactsDirectory);
            File.Copy(metadataPath, Path.Combine(referenceDirectory, "metadata.csv"), true);
            File.Copy(generatorPath, Path.Combine(referenceDirectory, "generate_dataset.py"), true);
            string copiedReferencePath = Path.Combine(referenceDirectory, referenceFileName);
            File.Copy(referenceImagePath, copiedReferencePath, true);

            System.Drawing.Rectangle templateRoi = objectOnlyZeroReferenceProfile
                ? new System.Drawing.Rectangle(190, 220, 175, 130)
                : zeroReferenceProfile
                    ? new System.Drawing.Rectangle(190, 220, 175, 145)
                    : new System.Drawing.Rectangle(90, 150, 270, 220);
            System.Drawing.RectangleF allowedCenterRegion = zeroReferenceProfile
                ? new System.Drawing.RectangleF(140F, 180F, 250F, 230F)
                : new System.Drawing.RectangleF(120F, 175F, 220F, 205F);
            double angleMinimum = zeroReferenceProfile ? -5D : -3D;
            double angleMaximum = zeroReferenceProfile ? 5D : 3D;
            double referenceAngleBefore = 0D;
            double referenceCorrectionApplied = 0D;
            double referenceAngleAfter = 0D;
            string preparedReferencePath = copiedReferencePath;
            if (zeroReferenceProfile)
            {
                preparedReferencePath = Path.Combine(referenceDirectory, "die_pad_001_ok_zero_degree.png");
                string angleEvidencePath = Path.Combine(referenceDirectory, "zero_degree_reference_evidence.png");
                CreateMatchingDiePadZeroDegreeReference(
                    copiedReferencePath,
                    preparedReferencePath,
                    angleEvidencePath,
                    templateRoi,
                    out referenceAngleBefore,
                    out referenceCorrectionApplied,
                    out referenceAngleAfter);
            }

            string templatePath = Path.Combine(referenceDirectory, "die_pad_1_template.png");
            string referenceOverlayPath = Path.Combine(referenceDirectory, "die_pad_1_template_roi.png");
            CreateMatchingDiePadReference(preparedReferencePath, templatePath, referenceOverlayPath, templateRoi, allowedCenterRegion);
            string templateSha256 = ComputeC9FileSha256(templatePath);

            RecipeWorkspaceService.DeleteVisionWorkspace("Codex_MatchingDiePadBatchSmoke");
            CleanupKnownSmokeRecipeWorkspaces("Smoke_MatchingDiePad_");
            string recipeName = "Smoke_MatchingDiePad_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            List<MatchingDiePadEvidenceRow> evidenceRows = new List<MatchingDiePadEvidenceRow>();
            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = recipeName;
                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1500,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);
                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(60);
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveMatchingForTest(property => ConfigureMatchingDiePadProperty(property, zeroReferenceProfile));
                Pump(12);

                MatchingProperty property = runtimeContext.Global.VisionTools.Matchings.Count > 0
                    ? runtimeContext.Global.VisionTools.Matchings[0]
                    : throw new InvalidOperationException("Matching_1 repository property was not created.");
                string propertyXmlPath = Path.Combine(referenceDirectory, "Matching_1.die-pad-native.xml");
                property.SaveTestConfig(propertyXmlPath);
                MatchingProperty loadedProperty = new MatchingProperty("Matching_1").LoadTestConfig(propertyXmlPath);
                ValidateMatchingDiePadProperty(loadedProperty, zeroReferenceProfile);

                foreach (Dictionary<string, string> sourceRow in rows)
                {
                    string status = sourceRow["status"];
                    string filename = sourceRow["filename"];
                    string split = sourceRow["detection_segmentation_split"];
                    string sourcePath = Path.Combine(datasetRoot, "all_images", status, filename);
                    if (!File.Exists(sourcePath))
                    {
                        throw new FileNotFoundException("Matching Die Pad source image was not found.", sourcePath);
                    }

                    string actualMd5 = ComputeMatchingDiePadMd5(sourcePath);
                    if (!string.Equals(actualMd5, sourceRow["md5"], StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Matching Die Pad source hash changed for " + filename + ". Expected=" + sourceRow["md5"] + ", Actual=" + actualMd5);
                    }

                    string rowId = split + "_" + status + "_" + Path.GetFileNameWithoutExtension(filename);
                    string rowDirectory = Path.Combine(runsDirectory, rowId);
                    Directory.CreateDirectory(rowDirectory);
                    string copiedSourcePath = Path.Combine(rowDirectory, "source.jpg");
                    File.Copy(sourcePath, copiedSourcePath, true);
                    MatchingC9PreviewCapture capture = CaptureMatchingC9Preview(
                        shellHost,
                        sourcePath,
                        rowDirectory,
                        rowId,
                        templateRoi.X + (templateRoi.Width / 2D),
                        templateRoi.Y + (templateRoi.Height / 2D),
                        Array.Empty<System.Drawing.PointF>());
                    MatchingDiePadEvidenceRow evidenceRow = new MatchingDiePadEvidenceRow(
                        sourceRow,
                        capture,
                        actualMd5,
                        ComputeC9FileSha256(sourcePath),
                        rowDirectory,
                        allowedCenterRegion,
                        templateRoi,
                        angleMinimum,
                        angleMaximum,
                        0.75D,
                        1.35D);
                    evidenceRows.Add(evidenceRow);
                    DrawMatchingDiePadEvidenceOverlay(evidenceRow);
                }

                foreach (IGrouping<string, MatchingDiePadEvidenceRow> split in evidenceRows.GroupBy(row => row.Split))
                {
                    SaveMatchingDiePadContactSheet(
                        split.OrderBy(row => row.RoleLabelOnly, StringComparer.Ordinal).ThenBy(row => row.Filename, StringComparer.Ordinal),
                        Path.Combine(contactsDirectory, split.Key + ".png"));
                }

                SaveMatchingDiePadContactSheet(
                    SelectMatchingDiePadReviewRows(evidenceRows),
                    Path.Combine(contactsDirectory, "representative_boundary_failures.png"));
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_MatchingDiePadBatch_Final.png"));
                WriteMatchingDiePadOutputs(
                    outputDirectory,
                    sourceFile,
                    profile,
                    templateRoi,
                    allowedCenterRegion,
                    templateSha256,
                    angleMinimum,
                    angleMaximum,
                    referenceAngleBefore,
                    referenceCorrectionApplied,
                    referenceAngleAfter,
                    evidenceRows);
                if (evidenceRows.Any(row => !row.Pass))
                {
                    throw new InvalidOperationException(
                        "Matching Die Pad native batch did not satisfy every gate. Pass="
                        + evidenceRows.Count(row => row.Pass)
                        + "/"
                        + evidenceRows.Count
                        + ". See native_rows.csv and current-run overlays.");
                }
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static int MatchingDiePadSplitOrder(string split)
        {
            if (string.Equals(split, "train", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (string.Equals(split, "val", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            return 2;
        }

        private static void CreateMatchingDiePadReference(
            string referencePath,
            string templatePath,
            string overlayPath,
            System.Drawing.Rectangle templateRoi,
            System.Drawing.RectangleF allowedCenterRegion)
        {
            using Bitmap reference = new Bitmap(referencePath);
            if (templateRoi.Left < 0
                || templateRoi.Top < 0
                || templateRoi.Right > reference.Width
                || templateRoi.Bottom > reference.Height)
            {
                throw new InvalidOperationException("Matching Die Pad template ROI is outside the reference image.");
            }

            using (Bitmap template = reference.Clone(templateRoi, reference.PixelFormat))
            {
                template.Save(templatePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            using Bitmap overlay = new Bitmap(reference);
            using Graphics graphics = Graphics.FromImage(overlay);
            using System.Drawing.Pen templatePen = new System.Drawing.Pen(System.Drawing.Color.Yellow, 3F);
            using System.Drawing.Pen allowedPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 2F);
            using System.Drawing.Brush headerBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(190, 0, 0, 0));
            using System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            using System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 11F, System.Drawing.FontStyle.Bold);
            graphics.DrawRectangle(templatePen, templateRoi);
            graphics.DrawRectangle(allowedPen, allowedCenterRegion.X, allowedCenterRegion.Y, allowedCenterRegion.Width, allowedCenterRegion.Height);
            graphics.FillRectangle(headerBrush, 0F, 0F, overlay.Width, 34F);
            graphics.DrawString("YELLOW template ROI | CYAN allowed detected-center region", font, textBrush, 6F, 6F);
            overlay.Save(overlayPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static void CreateMatchingDiePadZeroDegreeReference(
            string sourcePath,
            string correctedPath,
            string evidencePath,
            System.Drawing.Rectangle templateRoi,
            out double angleBefore,
            out double correctionApplied,
            out double angleAfter)
        {
            using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(sourcePath, OpenCvSharp.ImreadModes.Grayscale);
            if (source.Empty())
            {
                throw new InvalidOperationException("Matching Die Pad zero-degree reference could not be loaded.");
            }

            OpenCvSharp.LineSegmentPoint beforeLine = DetectMatchingDiePadBaseline(source, out angleBefore);
            correctionApplied = angleBefore;
            OpenCvSharp.Point2f center = new OpenCvSharp.Point2f(
                templateRoi.X + (templateRoi.Width / 2F),
                templateRoi.Y + (templateRoi.Height / 2F));
            using OpenCvSharp.Mat matrix = OpenCvSharp.Cv2.GetRotationMatrix2D(center, correctionApplied, 1D);
            using OpenCvSharp.Mat corrected = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.WarpAffine(
                source,
                corrected,
                matrix,
                source.Size(),
                OpenCvSharp.InterpolationFlags.Cubic,
                OpenCvSharp.BorderTypes.Reflect101);
            OpenCvSharp.LineSegmentPoint afterLine = DetectMatchingDiePadBaseline(corrected, out angleAfter);
            if (Math.Abs(angleBefore) < 0.5D || Math.Abs(angleAfter) > 0.2D)
            {
                throw new InvalidOperationException(
                    "Matching Die Pad zero-degree reference gate failed. Before="
                    + angleBefore.ToString("0.###", CultureInfo.InvariantCulture)
                    + ", After="
                    + angleAfter.ToString("0.###", CultureInfo.InvariantCulture));
            }

            OpenCvSharp.Cv2.ImWrite(correctedPath, corrected);
            SaveMatchingDiePadZeroDegreeEvidence(
                source,
                corrected,
                beforeLine,
                afterLine,
                angleBefore,
                correctionApplied,
                angleAfter,
                evidencePath);
        }

        private static OpenCvSharp.LineSegmentPoint DetectMatchingDiePadBaseline(
            OpenCvSharp.Mat source,
            out double angle)
        {
            OpenCvSharp.Rect searchRoi = new OpenCvSharp.Rect(170, 320, 210, 60);
            using OpenCvSharp.Mat crop = new OpenCvSharp.Mat(source, searchRoi);
            using OpenCvSharp.Mat edges = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Canny(crop, edges, 35D, 100D);
            OpenCvSharp.LineSegmentPoint[] lines = OpenCvSharp.Cv2.HoughLinesP(
                edges,
                1D,
                Math.PI / 1800D,
                45,
                120D,
                15D);
            OpenCvSharp.LineSegmentPoint? selected = lines
                .Select(line => new
                {
                    Line = line,
                    Angle = Math.Atan2(line.P2.Y - line.P1.Y, line.P2.X - line.P1.X) * 180D / Math.PI,
                    Length = Math.Sqrt(
                        Math.Pow(line.P2.X - line.P1.X, 2D)
                        + Math.Pow(line.P2.Y - line.P1.Y, 2D))
                })
                .Where(candidate => Math.Abs(candidate.Angle) <= 10D)
                .OrderByDescending(candidate => candidate.Length)
                .Select(candidate => (OpenCvSharp.LineSegmentPoint?)candidate.Line)
                .FirstOrDefault();
            if (!selected.HasValue)
            {
                throw new InvalidOperationException("Matching Die Pad reference baseline was not found.");
            }

            OpenCvSharp.LineSegmentPoint local = selected.Value;
            OpenCvSharp.LineSegmentPoint result = new OpenCvSharp.LineSegmentPoint(
                new OpenCvSharp.Point(local.P1.X + searchRoi.X, local.P1.Y + searchRoi.Y),
                new OpenCvSharp.Point(local.P2.X + searchRoi.X, local.P2.Y + searchRoi.Y));
            angle = Math.Atan2(result.P2.Y - result.P1.Y, result.P2.X - result.P1.X) * 180D / Math.PI;
            return result;
        }

        private static void SaveMatchingDiePadZeroDegreeEvidence(
            OpenCvSharp.Mat source,
            OpenCvSharp.Mat corrected,
            OpenCvSharp.LineSegmentPoint beforeLine,
            OpenCvSharp.LineSegmentPoint afterLine,
            double angleBefore,
            double correctionApplied,
            double angleAfter,
            string outputPath)
        {
            using OpenCvSharp.Mat beforeColor = new OpenCvSharp.Mat();
            using OpenCvSharp.Mat afterColor = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.CvtColor(source, beforeColor, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
            OpenCvSharp.Cv2.CvtColor(corrected, afterColor, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
            OpenCvSharp.Cv2.Line(beforeColor, beforeLine.P1, beforeLine.P2, OpenCvSharp.Scalar.Red, 3);
            OpenCvSharp.Cv2.Line(afterColor, afterLine.P1, afterLine.P2, OpenCvSharp.Scalar.Lime, 3);
            OpenCvSharp.Cv2.PutText(
                beforeColor,
                "BEFORE baseline " + angleBefore.ToString("0.000", CultureInfo.InvariantCulture) + " deg",
                new OpenCvSharp.Point(12, 28),
                OpenCvSharp.HersheyFonts.HersheySimplex,
                0.65D,
                OpenCvSharp.Scalar.Red,
                2);
            OpenCvSharp.Cv2.PutText(
                afterColor,
                "AFTER correction " + correctionApplied.ToString("0.000", CultureInfo.InvariantCulture)
                    + " deg / residual " + angleAfter.ToString("0.000", CultureInfo.InvariantCulture) + " deg",
                new OpenCvSharp.Point(12, 28),
                OpenCvSharp.HersheyFonts.HersheySimplex,
                0.55D,
                OpenCvSharp.Scalar.Lime,
                2);
            using OpenCvSharp.Mat comparison = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.HConcat(new[] { beforeColor, afterColor }, comparison);
            OpenCvSharp.Cv2.ImWrite(outputPath, comparison);
        }

        private static void ConfigureMatchingDiePadProperty(MatchingProperty property, bool zeroReferenceProfile)
        {
            property.AUTO_PREVIEW = false;
            property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
            property.SCORE_MIN = 0D;
            property.NUM_MATCH = 1;
            property.MAGNIFIATION = 1D;
            property.USE_FIND_ANGLE = true;
            property.FIND_ANGLE_MIN = zeroReferenceProfile ? -5 : -3;
            property.FIND_ANGLE_MAX = zeroReferenceProfile ? 5 : 3;
            property.FIND_ANGLE = 0.5D;
            property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
            property.USE_FIND_SCALE = true;
            property.FIND_SCALE_MIN = 0.75D;
            property.FIND_SCALE_MAX = 1.35D;
            property.FIND_SCALE_STEP = 0.05D;
            property.USE_PYRAMID_POSITION_PROPOSAL = false;
            property.USE_CANNY = false;
            property.USE_THRESHOLD = false;
            property.USE_ADAPTIVE_THRESHOLD = false;
            property.USE_ROI = false;
            property.USE_MULTI_ROI = false;
            property.USE_PADDING_COLOR_WHITE = false;
        }

        private static void ValidateMatchingDiePadProperty(MatchingProperty property, bool zeroReferenceProfile)
        {
            int expectedAngleMinimum = zeroReferenceProfile ? -5 : -3;
            int expectedAngleMaximum = zeroReferenceProfile ? 5 : 3;
            if (property == null
                || property.AUTO_PREVIEW
                || property.MATCH_MODE != OpenCvSharp.TemplateMatchModes.CCoeffNormed
                || Math.Abs(property.SCORE_MIN) > 0.000001D
                || property.NUM_MATCH != 1
                || Math.Abs(property.MAGNIFIATION - 1D) > 0.000001D
                || !property.USE_FIND_ANGLE
                || property.FIND_ANGLE_MIN != expectedAngleMinimum
                || property.FIND_ANGLE_MAX != expectedAngleMaximum
                || Math.Abs(property.FIND_ANGLE - 0.5D) > 0.000001D
                || property.USE_COARSE_TO_FINE_ANGLE_SEARCH
                || !property.USE_FIND_SCALE
                || Math.Abs(property.FIND_SCALE_MIN - 0.75D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_MAX - 1.35D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_STEP - 0.05D) > 0.000001D
                || property.USE_PYRAMID_POSITION_PROPOSAL
                || property.USE_CANNY
                || property.USE_THRESHOLD
                || property.USE_ADAPTIVE_THRESHOLD
                || property.USE_ROI
                || property.USE_MULTI_ROI
                || property.USE_PADDING_COLOR_WHITE)
            {
                throw new InvalidOperationException("Matching Die Pad settings did not survive native property XML save/load.");
            }
        }

        private static string ComputeMatchingDiePadMd5(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(stream)).ToLowerInvariant();
        }

        private static void DrawMatchingDiePadEvidenceOverlay(MatchingDiePadEvidenceRow row)
        {
            if (!File.Exists(row.Capture.NativePreviewPath))
            {
                return;
            }

            string evidencePath = Path.Combine(row.RowDirectory, "native_evidence_overlay.png");
            using Bitmap evidence = new Bitmap(row.Capture.NativePreviewPath);
            using Graphics graphics = Graphics.FromImage(evidence);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using System.Drawing.Pen allowedPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 2F);
            using System.Drawing.Pen detectedPen = new System.Drawing.Pen(row.Pass ? System.Drawing.Color.Lime : System.Drawing.Color.Red, 3F);
            using System.Drawing.Pen framePen = new System.Drawing.Pen(row.Pass ? System.Drawing.Color.Lime : System.Drawing.Color.Red, 4F);
            using System.Drawing.Brush headerBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(205, 0, 0, 0));
            using System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            using System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 10F, System.Drawing.FontStyle.Bold);
            graphics.DrawRectangle(
                allowedPen,
                row.AllowedCenterRegion.X,
                row.AllowedCenterRegion.Y,
                row.AllowedCenterRegion.Width,
                row.AllowedCenterRegion.Height);
            if (row.Capture.Parsed)
            {
                DrawMatchingC9Cross(graphics, detectedPen, (float)row.Capture.CenterX, (float)row.Capture.CenterY, 9F);
            }

            graphics.DrawRectangle(framePen, 2F, 2F, evidence.Width - 5F, evidence.Height - 5F);
            graphics.FillRectangle(headerBrush, 0F, 0F, evidence.Width, 52F);
            string label = string.Format(
                CultureInfo.InvariantCulture,
                "{0} | {1} | score {2:0.###} | center {3:0.0},{4:0.0} | angle {5:0.###} | scale {6:0.###} | CYAN allowed / cross detected",
                row.Pass ? "PASS" : "FAIL",
                row.RowId,
                row.Capture.Score,
                row.Capture.CenterX,
                row.Capture.CenterY,
                row.Capture.Angle,
                row.Capture.Scale);
            graphics.DrawString(label, font, textBrush, new RectangleF(8F, 4F, evidence.Width - 16F, 44F));
            evidence.Save(evidencePath, System.Drawing.Imaging.ImageFormat.Png);
            row.EvidenceOverlayPath = evidencePath;
        }

        private static void SaveMatchingDiePadContactSheet(IEnumerable<MatchingDiePadEvidenceRow> rows, string outputPath)
        {
            MatchingDiePadEvidenceRow[] items = rows.ToArray();
            if (items.Length == 0)
            {
                return;
            }

            const int columns = 4;
            const int cellWidth = 320;
            const int cellHeight = 320;
            int rowCount = (int)Math.Ceiling(items.Length / (double)columns);
            using Bitmap sheet = new Bitmap(columns * cellWidth, rowCount * cellHeight);
            using Graphics graphics = Graphics.FromImage(sheet);
            graphics.Clear(System.Drawing.Color.Black);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            for (int index = 0; index < items.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(items[index].EvidenceOverlayPath)
                    || !File.Exists(items[index].EvidenceOverlayPath))
                {
                    continue;
                }

                using Bitmap image = new Bitmap(items[index].EvidenceOverlayPath);
                int x = (index % columns) * cellWidth;
                int y = (index / columns) * cellHeight;
                graphics.DrawImage(image, new System.Drawing.Rectangle(x, y, cellWidth, cellHeight));
            }

            sheet.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static IEnumerable<MatchingDiePadEvidenceRow> SelectMatchingDiePadReviewRows(
            IReadOnlyList<MatchingDiePadEvidenceRow> rows)
        {
            return rows.Where(row => !row.Pass)
                .Concat(rows.GroupBy(row => row.Split).Select(group => group.OrderBy(row => row.Capture.Score).First()))
                .Concat(rows.GroupBy(row => row.Split).Select(group => group.OrderByDescending(row => row.Capture.Score).First()))
                .Distinct()
                .OrderBy(row => MatchingDiePadSplitOrder(row.Split))
                .ThenBy(row => row.Capture.Score)
                .Take(24);
        }

        private static void WriteMatchingDiePadOutputs(
            string outputDirectory,
            string sourceFile,
            string profile,
            System.Drawing.Rectangle templateRoi,
            System.Drawing.RectangleF allowedCenterRegion,
            string templateSha256,
            double angleMinimum,
            double angleMaximum,
            double referenceAngleBefore,
            double referenceCorrectionApplied,
            double referenceAngleAfter,
            IReadOnlyList<MatchingDiePadEvidenceRow> rows)
        {
            string header = "RowId,Split,RoleLabelOnly,Filename,SourceFile,DefectCount,DefectTypes,ExpectedMd5,ActualMd5,SourceSha256,Parsed,ResultCount,Score,CenterX,CenterY,BoxWidth,BoxHeight,Angle,Scale,CenterInsideAllowed,ScorePass,GeometryPass,LoadRunDelta,ExplicitPreviewRunDelta,HasNativePreviewResult,NativePreviewCaptured,Pass,TactMs,WallElapsedMs,Status,Review,SourcePath,NativePreviewPath,EvidenceOverlayPath";
            File.WriteAllLines(
                Path.Combine(outputDirectory, "native_rows.csv"),
                new[] { header }.Concat(rows.Select(row => row.ToCsv())),
                Encoding.UTF8);

            string executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            string entryAssemblyPath = typeof(OpenVisionLabDirectSmokeRunner).Assembly.Location;
            string nativeAssemblyPath = typeof(OpenVisionLab.Vision2D.Tool.MatchingTool).Assembly.Location;
            var summary = new
            {
                scenario = "matching-die-pad-batch",
                profile,
                source_file = sourceFile,
                boundary = "Current EXE Matching Tool View locator evidence only; OK/NG are corpus role labels, not Matching pass/fail truth.",
                rows = rows.Count,
                passed = rows.Count(row => row.Pass),
                failed = rows.Count(row => !row.Pass),
                by_split = rows.GroupBy(row => row.Split).ToDictionary(
                    group => group.Key,
                    group => new { rows = group.Count(), passed = group.Count(row => row.Pass), failed = group.Count(row => !row.Pass) }),
                by_role = rows.GroupBy(row => row.RoleLabelOnly).ToDictionary(
                    group => group.Key,
                    group => new { rows = group.Count(), passed = group.Count(row => row.Pass), failed = group.Count(row => !row.Pass) }),
                score = new
                {
                    minimum = rows.Min(row => row.Capture.Score),
                    average = rows.Average(row => row.Capture.Score),
                    maximum = rows.Max(row => row.Capture.Score),
                    gate_minimum = 60D
                },
                explicit_run_contract = new
                {
                    loads_without_preview = rows.Count(row => row.Capture.LoadRunDelta == 0),
                    explicit_preview_increment_one = rows.Count(row => row.Capture.ExplicitPreviewRunDelta == 1),
                    native_results = rows.Count(row => row.Capture.HasNativePreviewResult),
                    native_preview_files = rows.Count(row => row.Capture.NativePreviewCaptured),
                    all_pass = rows.All(row => row.Capture.ExplicitRunContractPassed)
                },
                template = new
                {
                    roi = new { templateRoi.X, templateRoi.Y, templateRoi.Width, templateRoi.Height },
                    sha256 = templateSha256,
                    reference_angle_before_deg = referenceAngleBefore,
                    reference_correction_applied_deg = referenceCorrectionApplied,
                    reference_angle_after_deg = referenceAngleAfter
                },
                allowed_center_region = new
                {
                    x = allowedCenterRegion.X,
                    y = allowedCenterRegion.Y,
                    width = allowedCenterRegion.Width,
                    height = allowedCenterRegion.Height
                },
                settings = new
                {
                    mode = "CCoeffNormed",
                    runtime_score_min = 0D,
                    report_score_gate = 60D,
                    result_count = 1,
                    angle_min = angleMinimum,
                    angle_max = angleMaximum,
                    angle_step = 0.5D,
                    scale_min = 0.75D,
                    scale_max = 1.35D,
                    scale_step = 0.05D,
                    auto_preview = false
                },
                runtime = new
                {
                    executable = executablePath,
                    executable_sha256 = File.Exists(executablePath) ? ComputeC9FileSha256(executablePath) : string.Empty,
                    entry_assembly = entryAssemblyPath,
                    entry_assembly_sha256 = ComputeC9FileSha256(entryAssemblyPath),
                    native_matching_assembly = nativeAssemblyPath,
                    native_matching_assembly_sha256 = ComputeC9FileSha256(nativeAssemblyPath)
                }
            };
            File.WriteAllText(
                Path.Combine(outputDirectory, "summary.json"),
                System.Text.Json.JsonSerializer.Serialize(summary, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                Encoding.UTF8);
            File.WriteAllText(
                Path.Combine(outputDirectory, "report.txt"),
                string.Join(Environment.NewLine, new[]
                {
                    "Result: " + (rows.All(row => row.Pass) ? "PASS" : "FAIL"),
                    "Scenario: matching-die-pad-batch",
                    "Profile: " + profile,
                    "SourceFile: " + sourceFile,
                    "Dataset: " + rows.Count(row => row.Pass) + "/" + rows.Count,
                    "Train/Val/Test: " + string.Join(" / ", rows.GroupBy(row => row.Split).OrderBy(group => MatchingDiePadSplitOrder(group.Key)).Select(group => group.Key + " " + group.Count(row => row.Pass) + "/" + group.Count())),
                    "OK/NG role labels: " + string.Join(" / ", rows.GroupBy(row => row.RoleLabelOnly).OrderBy(group => group.Key).Select(group => group.Key + " " + group.Count(row => row.Pass) + "/" + group.Count())),
                    "Score min/avg/max: " + rows.Min(row => row.Capture.Score).ToString("0.###", CultureInfo.InvariantCulture) + " / " + rows.Average(row => row.Capture.Score).ToString("0.###", CultureInfo.InvariantCulture) + " / " + rows.Max(row => row.Capture.Score).ToString("0.###", CultureInfo.InvariantCulture),
                    "ExplicitRunContract: " + rows.Count(row => row.Capture.ExplicitRunContractPassed) + "/" + rows.Count,
                    "Reference angle before/correction/after: "
                        + referenceAngleBefore.ToString("0.###", CultureInfo.InvariantCulture)
                        + " / "
                        + referenceCorrectionApplied.ToString("0.###", CultureInfo.InvariantCulture)
                        + " / "
                        + referenceAngleAfter.ToString("0.###", CultureInfo.InvariantCulture),
                    "TemplateSha256: " + templateSha256,
                    "Boundary: current EXE Matching Tool View locator evidence only; no defect classification or exact pose-error truth"
                }),
                Encoding.UTF8);
        }

        private static void ConfigureMatchingC9Property(MatchingProperty property)
        {
            property.AUTO_PREVIEW = false;
            property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
            property.SCORE_MIN = 0D;
            property.NUM_MATCH = 1;
            property.MAGNIFIATION = 1D;
            property.USE_FIND_ANGLE = true;
            property.FIND_ANGLE_MIN = -5;
            property.FIND_ANGLE_MAX = 5;
            property.FIND_ANGLE = 1D;
            property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
            property.USE_FIND_SCALE = true;
            property.FIND_SCALE_MIN = 0.8D;
            property.FIND_SCALE_MAX = 1.9D;
            property.FIND_SCALE_STEP = 0.1D;
            property.USE_PYRAMID_POSITION_PROPOSAL = false;
            property.USE_CANNY = false;
            property.USE_THRESHOLD = false;
            property.USE_ADAPTIVE_THRESHOLD = false;
            property.USE_ROI = false;
            property.USE_MULTI_ROI = false;
            property.USE_PADDING_COLOR_WHITE = true;
        }

        private static void ValidateMatchingC9Property(MatchingProperty property)
        {
            if (property == null
                || property.AUTO_PREVIEW
                || property.MATCH_MODE != OpenCvSharp.TemplateMatchModes.CCoeffNormed
                || Math.Abs(property.SCORE_MIN) > 0.000001D
                || property.NUM_MATCH != 1
                || Math.Abs(property.MAGNIFIATION - 1D) > 0.000001D
                || !property.USE_FIND_ANGLE
                || property.FIND_ANGLE_MIN != -5
                || property.FIND_ANGLE_MAX != 5
                || Math.Abs(property.FIND_ANGLE - 1D) > 0.000001D
                || property.USE_COARSE_TO_FINE_ANGLE_SEARCH
                || !property.USE_FIND_SCALE
                || Math.Abs(property.FIND_SCALE_MIN - 0.8D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_MAX - 1.9D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_STEP - 0.1D) > 0.000001D
                || property.USE_PYRAMID_POSITION_PROPOSAL
                || property.USE_CANNY
                || property.USE_THRESHOLD
                || property.USE_ADAPTIVE_THRESHOLD
                || property.USE_ROI
                || property.USE_MULTI_ROI
                || !property.USE_PADDING_COLOR_WHITE)
            {
                throw new InvalidOperationException("Matching C9 settings did not survive native property XML save/load.");
            }
        }

        private static MatchingC9PreviewCapture CaptureMatchingC9Preview(
            OpenVisionShellHostView shellHost,
            string sourcePath,
            string outputDirectory,
            string rowId,
            double expectedCenterX,
            double expectedCenterY,
            IReadOnlyList<System.Drawing.PointF> expectedPolygon)
        {
            int previewRunCountBeforeLoad = shellHost.NativePreviewRunCount;
            using (Bitmap source = new Bitmap(sourcePath))
            {
                shellHost.SetMainLayerImageForTest(source);
            }

            Pump(8);
            int previewRunCountAfterLoad = shellHost.NativePreviewRunCount;
            string nativePreviewPath = Path.Combine(outputDirectory, "native_preview.png");
            if (File.Exists(nativePreviewPath))
            {
                File.Delete(nativePreviewPath);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            shellHost.RunActiveNativePreviewForTest();
            Pump(16);
            stopwatch.Stop();
            int previewRunCountAfterPreview = shellHost.NativePreviewRunCount;
            bool hasNativePreviewResult = shellHost.HasNativePreviewResult;

            string status = shellHost.ActiveNativeStatusText ?? string.Empty;
            string review = shellHost.ActiveNativeResultReviewText ?? string.Empty;
            bool parsed = TryParseMatchingC9Review(
                review,
                out int resultCount,
                out double score,
                out double centerX,
                out double centerY,
                out double boxWidth,
                out double boxHeight,
                out double angle,
                out double scale,
                out double tactMs);
            using (Bitmap preview = shellHost.GetLayerImageCloneForTest("Matching_Preview"))
            {
                if (preview != null)
                {
                    preview.Save(nativePreviewPath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            bool nativePreviewCaptured = File.Exists(nativePreviewPath)
                && new FileInfo(nativePreviewPath).Length > 0L;

            double dx = centerX - expectedCenterX;
            double dy = centerY - expectedCenterY;
            double centerError = parsed ? Math.Sqrt((dx * dx) + (dy * dy)) : double.PositiveInfinity;
            return new MatchingC9PreviewCapture(
                rowId,
                sourcePath,
                nativePreviewPath,
                status,
                review,
                parsed,
                resultCount,
                score,
                centerX,
                centerY,
                boxWidth,
                boxHeight,
                angle,
                scale,
                tactMs,
                stopwatch.Elapsed.TotalMilliseconds,
                centerError,
                previewRunCountBeforeLoad,
                previewRunCountAfterLoad,
                previewRunCountAfterPreview,
                hasNativePreviewResult,
                nativePreviewCaptured,
                expectedPolygon);
        }

        private static bool TryParseMatchingC9Review(
            string review,
            out int resultCount,
            out double score,
            out double centerX,
            out double centerY,
            out double boxWidth,
            out double boxHeight,
            out double angle,
            out double scale,
            out double tactMs)
        {
            resultCount = 0;
            score = centerX = centerY = boxWidth = boxHeight = angle = tactMs = double.NaN;
            scale = 1D;
            Match countMatch = Regex.Match(review ?? string.Empty, @"(?:Count\s+(?<v>[0-9]+)|검출\s+(?<v>[0-9]+)개)", RegexOptions.IgnoreCase);
            Match scoreMatch = Regex.Match(review ?? string.Empty, @"(?:Score|점수)\s+(?<v>-?[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            Match centerMatch = Regex.Match(review ?? string.Empty, @"(?:Center|중심)\s+(?<x>-?[0-9]+(?:\.[0-9]+)?),\s*(?<y>-?[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            Match boxMatch = Regex.Match(review ?? string.Empty, @"(?:Box|박스)\s+(?<w>[0-9]+(?:\.[0-9]+)?)\s*x\s*(?<h>[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            Match angleMatch = Regex.Match(review ?? string.Empty, @"(?:Angle|각도)\s+(?<v>-?[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            Match scaleMatch = Regex.Match(review ?? string.Empty, @"(?:Scale|배율)\s+(?<v>[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            Match tactMatch = Regex.Match(review ?? string.Empty, @"(?:Tact|처리)\s+(?<v>[0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            if (!countMatch.Success
                || !scoreMatch.Success
                || !centerMatch.Success
                || !boxMatch.Success
                || !angleMatch.Success
                || !int.TryParse(countMatch.Groups["v"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out resultCount)
                || !double.TryParse(scoreMatch.Groups["v"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out score)
                || !double.TryParse(centerMatch.Groups["x"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out centerX)
                || !double.TryParse(centerMatch.Groups["y"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out centerY)
                || !double.TryParse(boxMatch.Groups["w"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out boxWidth)
                || !double.TryParse(boxMatch.Groups["h"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out boxHeight)
                || !double.TryParse(angleMatch.Groups["v"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out angle))
            {
                return false;
            }

            if (scaleMatch.Success
                && !double.TryParse(scaleMatch.Groups["v"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out scale))
            {
                return false;
            }

            if (tactMatch.Success)
            {
                double.TryParse(tactMatch.Groups["v"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out tactMs);
            }

            return true;
        }

        private static void CreateMatchingC9SyntheticSource(
            string templatePath,
            string outputPath,
            double scale,
            double angle,
            double centerX,
            double centerY)
        {
            using OpenCvSharp.Mat template = OpenCvSharp.Cv2.ImRead(templatePath, OpenCvSharp.ImreadModes.Color);
            if (template.Empty())
            {
                throw new InvalidOperationException("C9 template could not be loaded for native calibration.");
            }

            using OpenCvSharp.Mat matrix = OpenCvSharp.Cv2.GetRotationMatrix2D(
                new OpenCvSharp.Point2f(template.Width / 2F, template.Height / 2F),
                angle,
                scale);
            matrix.Set(0, 2, matrix.At<double>(0, 2) + centerX - (template.Width / 2D));
            matrix.Set(1, 2, matrix.At<double>(1, 2) + centerY - (template.Height / 2D));
            using OpenCvSharp.Mat canvas = new OpenCvSharp.Mat(
                new OpenCvSharp.Size(640, 480),
                OpenCvSharp.MatType.CV_8UC3,
                OpenCvSharp.Scalar.White);
            OpenCvSharp.Cv2.WarpAffine(
                template,
                canvas,
                matrix,
                canvas.Size(),
                OpenCvSharp.InterpolationFlags.Linear,
                OpenCvSharp.BorderTypes.Constant,
                OpenCvSharp.Scalar.White);
            OpenCvSharp.Cv2.ImWrite(outputPath, canvas);
        }

        private static System.Drawing.PointF[] CreateMatchingC9ExpectedPolygon(double scale, double angle, double tx, double ty)
        {
            double radians = angle * Math.PI / 180D;
            double cos = Math.Cos(radians) * scale;
            double sin = Math.Sin(radians) * scale;
            System.Drawing.PointF Transform(double x, double y)
            {
                return new System.Drawing.PointF(
                    (float)(cos * x - sin * y + tx),
                    (float)(sin * x + cos * y + ty));
            }

            return new[]
            {
                Transform(240D, 270D),
                Transform(305D, 270D),
                Transform(305D, 330D),
                Transform(240D, 330D)
            };
        }

        private static System.Drawing.PointF[] CreateMatchingC9NativePolygon(MatchingC9PreviewCapture capture)
        {
            double radians = capture.Angle * Math.PI / 180D;
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            double halfWidth = capture.BoxWidth / 2D;
            double halfHeight = capture.BoxHeight / 2D;
            System.Drawing.PointF Transform(double x, double y)
            {
                return new System.Drawing.PointF(
                    (float)(capture.CenterX + x * cos + y * sin),
                    (float)(capture.CenterY - x * sin + y * cos));
            }

            return new[]
            {
                Transform(-halfWidth, -halfHeight),
                Transform(halfWidth, -halfHeight),
                Transform(halfWidth, halfHeight),
                Transform(-halfWidth, halfHeight)
            };
        }

        private static double CalculateMatchingC9PolygonIou(
            IReadOnlyList<System.Drawing.PointF> expected,
            IReadOnlyList<System.Drawing.PointF> actual,
            int width,
            int height)
        {
            if (expected == null || expected.Count != 4 || actual == null || actual.Count != 4)
            {
                return 0D;
            }

            OpenCvSharp.Point[] expectedPoints = expected.Select(point => new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y))).ToArray();
            OpenCvSharp.Point[] actualPoints = actual.Select(point => new OpenCvSharp.Point((int)Math.Round(point.X), (int)Math.Round(point.Y))).ToArray();
            using OpenCvSharp.Mat expectedMask = OpenCvSharp.Mat.Zeros(height, width, OpenCvSharp.MatType.CV_8UC1);
            using OpenCvSharp.Mat actualMask = OpenCvSharp.Mat.Zeros(height, width, OpenCvSharp.MatType.CV_8UC1);
            using OpenCvSharp.Mat intersection = new OpenCvSharp.Mat();
            using OpenCvSharp.Mat union = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.FillConvexPoly(expectedMask, expectedPoints, OpenCvSharp.Scalar.White);
            OpenCvSharp.Cv2.FillConvexPoly(actualMask, actualPoints, OpenCvSharp.Scalar.White);
            OpenCvSharp.Cv2.BitwiseAnd(expectedMask, actualMask, intersection);
            OpenCvSharp.Cv2.BitwiseOr(expectedMask, actualMask, union);
            int unionPixels = OpenCvSharp.Cv2.CountNonZero(union);
            return unionPixels <= 0 ? 0D : (double)OpenCvSharp.Cv2.CountNonZero(intersection) / unionPixels;
        }

        private static bool IsMatchingC9CenterInside(
            IReadOnlyList<System.Drawing.PointF> polygon,
            double x,
            double y)
        {
            if (polygon == null || polygon.Count != 4)
            {
                return false;
            }

            OpenCvSharp.Point2f[] points = polygon.Select(point => new OpenCvSharp.Point2f(point.X, point.Y)).ToArray();
            return OpenCvSharp.Cv2.PointPolygonTest(points, new OpenCvSharp.Point2f((float)x, (float)y), false) >= 0D;
        }

        private static void DrawMatchingC9EvidenceOverlay(MatchingC9EvidenceRow row)
        {
            if (!File.Exists(row.Capture.NativePreviewPath))
            {
                return;
            }

            string evidencePath = Path.Combine(row.RowDirectory, "native_evidence_overlay.png");
            using Bitmap evidence = new Bitmap(row.Capture.NativePreviewPath);
            using Graphics graphics = Graphics.FromImage(evidence);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using System.Drawing.Pen expectedPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 2F);
            using System.Drawing.Pen relationPen = new System.Drawing.Pen(row.Pass ? System.Drawing.Color.Lime : System.Drawing.Color.Red, 2F);
            using System.Drawing.Pen framePen = new System.Drawing.Pen(row.Pass ? System.Drawing.Color.Lime : System.Drawing.Color.Red, 4F);
            using System.Drawing.Brush headerBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(190, 0, 0, 0));
            using System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            using System.Drawing.Font font = new System.Drawing.Font(
                System.Drawing.FontFamily.GenericSansSerif,
                10F,
                System.Drawing.FontStyle.Bold);
            graphics.DrawPolygon(expectedPen, row.ExpectedPolygon);
            DrawMatchingC9Cross(graphics, expectedPen, (float)row.ExpectedCenterX, (float)row.ExpectedCenterY, 8F);
            if (row.Capture.Parsed)
            {
                graphics.DrawLine(
                    relationPen,
                    (float)row.ExpectedCenterX,
                    (float)row.ExpectedCenterY,
                    (float)row.Capture.CenterX,
                    (float)row.Capture.CenterY);
            }

            graphics.DrawRectangle(framePen, 2F, 2F, evidence.Width - 5F, evidence.Height - 5F);
            graphics.FillRectangle(headerBrush, 0F, 0F, evidence.Width, 42F);
            string label = string.Format(
                CultureInfo.InvariantCulture,
                "{0} | {1} | score {2:0.###} | center err {3:0.###}px | scale {4:0.###}/{5:0.###} | angle {6:0.###}/{7:0.###} | IoU {8:0.###}",
                row.Pass ? "PASS" : "FAIL",
                row.RowId,
                row.Capture.Score,
                row.Capture.CenterErrorPx,
                row.Capture.Scale,
                row.OracleScale,
                row.Capture.Angle,
                -row.StripAngle,
                row.PolygonIou);
            graphics.DrawString(label, font, textBrush, new RectangleF(8F, 4F, evidence.Width - 16F, 34F));
            evidence.Save(evidencePath, System.Drawing.Imaging.ImageFormat.Png);
            row.EvidenceOverlayPath = evidencePath;
        }

        private static void DrawMatchingC9Cross(Graphics graphics, System.Drawing.Pen pen, float x, float y, float radius)
        {
            graphics.DrawLine(pen, x - radius, y, x + radius, y);
            graphics.DrawLine(pen, x, y - radius, x, y + radius);
        }

        private static void SaveMatchingC9ContactSheet(IEnumerable<MatchingC9EvidenceRow> rows, string outputPath)
        {
            MatchingC9EvidenceRow[] items = rows.ToArray();
            if (items.Length == 0)
            {
                return;
            }

            const int columns = 4;
            const int cellWidth = 320;
            const int cellHeight = 240;
            int rowCount = (int)Math.Ceiling(items.Length / (double)columns);
            using Bitmap sheet = new Bitmap(columns * cellWidth, rowCount * cellHeight);
            using Graphics graphics = Graphics.FromImage(sheet);
            graphics.Clear(System.Drawing.Color.Black);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            for (int index = 0; index < items.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(items[index].EvidenceOverlayPath)
                    || !File.Exists(items[index].EvidenceOverlayPath))
                {
                    continue;
                }

                using Bitmap image = new Bitmap(items[index].EvidenceOverlayPath);
                int x = (index % columns) * cellWidth;
                int y = (index / columns) * cellHeight;
                graphics.DrawImage(image, new Rectangle(x, y, cellWidth, cellHeight));
            }

            sheet.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static void WriteMatchingC9Outputs(
            string outputDirectory,
            string templateSha256,
            IReadOnlyList<MatchingC9CalibrationRow> calibrationRows,
            bool calibrationPassed,
            IReadOnlyList<MatchingC9EvidenceRow> evidenceRows)
        {
            List<string> calibrationCsv = new List<string>
            {
                "Name,ExpectedScale,NativeScale,ScaleError,ExpectedAngleDeg,NativeAngleDeg,AngleErrorDeg,ExpectedCenterX,ExpectedCenterY,NativeCenterX,NativeCenterY,CenterErrorPx,Score,ResultCount,PreviewRunCountBeforeLoad,PreviewRunCountAfterLoad,PreviewRunCountAfterPreview,LoadRunDelta,ExplicitPreviewRunDelta,HasNativePreviewResult,NativePreviewCaptured,Parsed,Pass,Review,SourcePath,NativePreviewPath"
            };
            calibrationCsv.AddRange(calibrationRows.Select(row => row.ToCsv()));
            File.WriteAllLines(Path.Combine(outputDirectory, "calibration.csv"), calibrationCsv, Encoding.UTF8);

            List<string> nativeCsv = new List<string>
            {
                "RowId,Split,RoleLabelOnly,Image,SourceSha256,TemplateSha256,ResultCount,Score,NormalizedScore,CenterX,CenterY,ExpectedCenterX,ExpectedCenterY,CenterErrorPx,BoxWidth,BoxHeight,Scale,OracleScale,ScaleError,AngleDeg,StripAngleDeg,ExpectedNativeStripAngleDeg,AngleErrorDeg,OrbGlobalAngleDeg,ExpectedNativeOrbAngleDeg,OrbGlobalAngleErrorDeg,CenterInsideOraclePolygon,PolygonIou,TactMs,WallElapsedMs,PreviewRunCountBeforeLoad,PreviewRunCountAfterLoad,PreviewRunCountAfterPreview,LoadRunDelta,ExplicitPreviewRunDelta,HasNativePreviewResult,NativePreviewCaptured,Parsed,Pass,Review,SourceCopyPath,NativePreviewPath,EvidenceOverlayPath"
            };
            nativeCsv.AddRange(evidenceRows.Select(row => row.ToCsv(templateSha256, outputDirectory)));
            File.WriteAllLines(Path.Combine(outputDirectory, "native_rows.csv"), nativeCsv, Encoding.UTF8);

            string entryAssemblyPath = Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly().Location;
            string siblingExecutablePath = Path.ChangeExtension(entryAssemblyPath, ".exe");
            string executablePath = File.Exists(siblingExecutablePath)
                ? siblingExecutablePath
                : Environment.ProcessPath ?? entryAssemblyPath;
            string nativeAssemblyPath = typeof(OpenVisionLab.Vision2D.Tool.MatchingTool).Assembly.Location;
            bool datasetPassed = evidenceRows.Count == 24 && evidenceRows.All(row => row.Pass);
            MatchingC9PreviewCapture[] captures = calibrationRows.Select(row => row.Capture)
                .Concat(evidenceRows.Select(row => row.Capture))
                .ToArray();
            object summary = new
            {
                status = calibrationPassed && datasetPassed ? "PASS" : "FAIL",
                scenario = "matching-c9-batch",
                boundary = "Current EXE Matching Tool View qualification only; not Pipeline XML, similarity normalization, gap measurement, or OK/NG evidence",
                template_sha256 = templateSha256,
                current_exe = new { path = executablePath, sha256 = ComputeC9FileSha256(executablePath) },
                entry_assembly = new { path = entryAssemblyPath, sha256 = ComputeC9FileSha256(entryAssemblyPath) },
                native_matching_assembly = new { path = nativeAssemblyPath, sha256 = ComputeC9FileSha256(nativeAssemblyPath) },
                config = new
                {
                    match_mode = "CCoeffNormed",
                    score_min = 0D,
                    num_match = 1,
                    magnification = 1D,
                    angle_min = -5,
                    angle_max = 5,
                    angle_step = 1D,
                    scale_min = 0.8D,
                    scale_max = 1.9D,
                    scale_step = 0.1D,
                    white_padding = true,
                    auto_preview = false,
                    coarse_angle = false,
                    pyramid_proposal = false,
                    canny = false,
                    roi = false
                },
                calibration = new
                {
                    rows = calibrationRows.Count,
                    passed = calibrationRows.Count(row => row.Pass),
                    all_pass = calibrationPassed
                },
                explicit_run_contract = new
                {
                    rows = captures.Length,
                    loads_without_preview_run = captures.Count(capture => capture.LoadRunDelta == 0),
                    explicit_preview_increment_one = captures.Count(capture => capture.ExplicitPreviewRunDelta == 1),
                    native_preview_results = captures.Count(capture => capture.HasNativePreviewResult),
                    native_preview_files = captures.Count(capture => capture.NativePreviewCaptured),
                    all_pass = captures.All(capture => capture.ExplicitRunContractPassed)
                },
                dataset = new
                {
                    rows = evidenceRows.Count,
                    passed = evidenceRows.Count(row => row.Pass),
                    failed = evidenceRows.Count(row => !row.Pass),
                    min_score = evidenceRows.Where(row => row.Capture.Parsed).Select(row => row.Capture.Score).DefaultIfEmpty().Min(),
                    max_center_error_px = evidenceRows.Select(row => row.Capture.CenterErrorPx).DefaultIfEmpty().Max(),
                    min_polygon_iou = evidenceRows.Select(row => row.PolygonIou).DefaultIfEmpty().Min(),
                    max_scale_error = evidenceRows.Select(row => row.ScaleError).DefaultIfEmpty().Max(),
                    max_local_strip_angle_error_deg = evidenceRows.Select(row => row.AngleError).DefaultIfEmpty().Max(),
                    max_global_orb_angle_error_deg_diagnostic = evidenceRows.Select(row => row.OrbGlobalAngleError).DefaultIfEmpty().Max()
                }
            };
            File.WriteAllText(
                Path.Combine(outputDirectory, "summary.json"),
                System.Text.Json.JsonSerializer.Serialize(
                    summary,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                    }),
                Encoding.UTF8);

            string report = string.Join(Environment.NewLine, new[]
            {
                "Result: " + (calibrationPassed && datasetPassed ? "PASS" : "FAIL"),
                "Scenario: matching-c9-batch",
                "Calibration: " + calibrationRows.Count(row => row.Pass) + "/" + calibrationRows.Count,
                "Dataset: " + evidenceRows.Count(row => row.Pass) + "/" + evidenceRows.Count,
                "TemplateSha256: " + templateSha256,
                "CurrentExe: " + executablePath,
                "CurrentExeSha256: " + ComputeC9FileSha256(executablePath),
                "EntryAssembly: " + entryAssemblyPath,
                "EntryAssemblySha256: " + ComputeC9FileSha256(entryAssemblyPath),
                "NativeMatchingAssembly: " + nativeAssemblyPath,
                "NativeMatchingAssemblySha256: " + ComputeC9FileSha256(nativeAssemblyPath),
                "ExplicitRunContract: " + captures.Count(capture => capture.ExplicitRunContractPassed) + "/" + captures.Length,
                "Boundary: current EXE Matching Tool View qualification only; not Pipeline XML, similarity normalization, gap measurement, or OK/NG evidence"
            });
            File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, Encoding.UTF8);
        }

        private static List<Dictionary<string, string>> ReadC9Csv(string path)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            using Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path, Encoding.UTF8);
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            string[] headers = parser.ReadFields() ?? Array.Empty<string>();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields() ?? Array.Empty<string>();
                Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int index = 0; index < headers.Length; index++)
                {
                    row[headers[index]] = index < fields.Length ? fields[index] : string.Empty;
                }

                rows.Add(row);
            }

            return rows;
        }

        private static string ComputeC9FileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream));
        }

        private static double ParseC9Double(string value)
        {
            return double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private static string C9Csv(string value)
        {
            value ??= string.Empty;
            return value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0
                ? value
                : "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private sealed class MatchingC9SyntheticCase
        {
            public MatchingC9SyntheticCase(string name, double scale, double angle)
            {
                Name = name;
                Scale = scale;
                Angle = angle;
            }

            public string Name { get; }
            public double Scale { get; }
            public double Angle { get; }
        }

        private sealed class MatchingC9PreviewCapture
        {
            public MatchingC9PreviewCapture(
                string rowId,
                string sourcePath,
                string nativePreviewPath,
                string status,
                string review,
                bool parsed,
                int resultCount,
                double score,
                double centerX,
                double centerY,
                double boxWidth,
                double boxHeight,
                double angle,
                double scale,
                double tactMs,
                double wallElapsedMs,
                double centerErrorPx,
                int previewRunCountBeforeLoad,
                int previewRunCountAfterLoad,
                int previewRunCountAfterPreview,
                bool hasNativePreviewResult,
                bool nativePreviewCaptured,
                IReadOnlyList<System.Drawing.PointF> expectedPolygon)
            {
                RowId = rowId;
                SourcePath = sourcePath;
                NativePreviewPath = nativePreviewPath;
                Status = status;
                Review = review;
                Parsed = parsed;
                ResultCount = resultCount;
                Score = score;
                CenterX = centerX;
                CenterY = centerY;
                BoxWidth = boxWidth;
                BoxHeight = boxHeight;
                Angle = angle;
                Scale = scale;
                TactMs = tactMs;
                WallElapsedMs = wallElapsedMs;
                CenterErrorPx = centerErrorPx;
                PreviewRunCountBeforeLoad = previewRunCountBeforeLoad;
                PreviewRunCountAfterLoad = previewRunCountAfterLoad;
                PreviewRunCountAfterPreview = previewRunCountAfterPreview;
                HasNativePreviewResult = hasNativePreviewResult;
                NativePreviewCaptured = nativePreviewCaptured;
                ExpectedPolygon = expectedPolygon;
            }

            public string RowId { get; }
            public string SourcePath { get; }
            public string NativePreviewPath { get; }
            public string Status { get; }
            public string Review { get; }
            public bool Parsed { get; }
            public int ResultCount { get; }
            public double Score { get; }
            public double CenterX { get; }
            public double CenterY { get; }
            public double BoxWidth { get; }
            public double BoxHeight { get; }
            public double Angle { get; }
            public double Scale { get; }
            public double TactMs { get; }
            public double WallElapsedMs { get; }
            public double CenterErrorPx { get; }
            public int PreviewRunCountBeforeLoad { get; }
            public int PreviewRunCountAfterLoad { get; }
            public int PreviewRunCountAfterPreview { get; }
            public int LoadRunDelta => PreviewRunCountAfterLoad - PreviewRunCountBeforeLoad;
            public int ExplicitPreviewRunDelta => PreviewRunCountAfterPreview - PreviewRunCountAfterLoad;
            public bool HasNativePreviewResult { get; }
            public bool NativePreviewCaptured { get; }
            public bool ExplicitRunContractPassed => LoadRunDelta == 0
                && ExplicitPreviewRunDelta == 1
                && HasNativePreviewResult
                && NativePreviewCaptured;
            public IReadOnlyList<System.Drawing.PointF> ExpectedPolygon { get; }
        }

        private sealed class MatchingDiePadEvidenceRow
        {
            public MatchingDiePadEvidenceRow(
                IReadOnlyDictionary<string, string> sourceRow,
                MatchingC9PreviewCapture capture,
                string actualMd5,
                string sourceSha256,
                string rowDirectory,
                System.Drawing.RectangleF allowedCenterRegion,
                System.Drawing.Rectangle templateRoi,
                double angleMinimum,
                double angleMaximum,
                double scaleMinimum,
                double scaleMaximum)
            {
                RowId = capture.RowId;
                Split = sourceRow["detection_segmentation_split"];
                RoleLabelOnly = sourceRow["status"];
                Filename = sourceRow["filename"];
                SourceFile = sourceRow["source_file"];
                DefectCount = sourceRow["defect_count"];
                DefectTypes = sourceRow["defect_types"];
                ExpectedMd5 = sourceRow["md5"];
                ActualMd5 = actualMd5;
                SourceSha256 = sourceSha256;
                Capture = capture;
                RowDirectory = rowDirectory;
                AllowedCenterRegion = allowedCenterRegion;
                MinimumBoxWidth = (templateRoi.Width * scaleMinimum) - 5D;
                MaximumBoxWidth = (templateRoi.Width * scaleMaximum) + 5D;
                MinimumBoxHeight = (templateRoi.Height * scaleMinimum) - 5D;
                MaximumBoxHeight = (templateRoi.Height * scaleMaximum) + 5D;
                AngleMinimum = angleMinimum;
                AngleMaximum = angleMaximum;
                ScaleMinimum = scaleMinimum;
                ScaleMaximum = scaleMaximum;
            }

            public string RowId { get; }
            public string Split { get; }
            public string RoleLabelOnly { get; }
            public string Filename { get; }
            public string SourceFile { get; }
            public string DefectCount { get; }
            public string DefectTypes { get; }
            public string ExpectedMd5 { get; }
            public string ActualMd5 { get; }
            public string SourceSha256 { get; }
            public MatchingC9PreviewCapture Capture { get; }
            public string RowDirectory { get; }
            public System.Drawing.RectangleF AllowedCenterRegion { get; }
            public double MinimumBoxWidth { get; }
            public double MaximumBoxWidth { get; }
            public double MinimumBoxHeight { get; }
            public double MaximumBoxHeight { get; }
            public double AngleMinimum { get; }
            public double AngleMaximum { get; }
            public double ScaleMinimum { get; }
            public double ScaleMaximum { get; }
            public string EvidenceOverlayPath { get; set; }
            public bool CenterInsideAllowed => Capture.Parsed
                && AllowedCenterRegion.Contains((float)Capture.CenterX, (float)Capture.CenterY);
            public bool ScorePass => Capture.Parsed && Capture.Score >= 60D;
            public bool GeometryPass => Capture.Parsed
                && Capture.ResultCount == 1
                && CenterInsideAllowed
                && Capture.BoxWidth >= MinimumBoxWidth
                && Capture.BoxWidth <= MaximumBoxWidth
                && Capture.BoxHeight >= MinimumBoxHeight
                && Capture.BoxHeight <= MaximumBoxHeight
                && Capture.Angle >= AngleMinimum - 0.001D
                && Capture.Angle <= AngleMaximum + 0.001D
                && Capture.Scale >= ScaleMinimum - 0.001D
                && Capture.Scale <= ScaleMaximum + 0.001D;
            public bool Pass => Capture.ExplicitRunContractPassed && ScorePass && GeometryPass;

            public string ToCsv()
            {
                return string.Join(",", new[]
                {
                    C9Csv(RowId),
                    C9Csv(Split),
                    C9Csv(RoleLabelOnly),
                    C9Csv(Filename),
                    C9Csv(SourceFile),
                    C9Csv(DefectCount),
                    C9Csv(DefectTypes),
                    ExpectedMd5,
                    ActualMd5,
                    SourceSha256,
                    Capture.Parsed.ToString(),
                    Capture.ResultCount.ToString(CultureInfo.InvariantCulture),
                    Capture.Score.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.CenterX.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.CenterY.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.BoxWidth.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.BoxHeight.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Scale.ToString("0.###", CultureInfo.InvariantCulture),
                    CenterInsideAllowed.ToString(),
                    ScorePass.ToString(),
                    GeometryPass.ToString(),
                    Capture.LoadRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.ExplicitPreviewRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.HasNativePreviewResult.ToString(),
                    Capture.NativePreviewCaptured.ToString(),
                    Pass.ToString(),
                    Capture.TactMs.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.WallElapsedMs.ToString("0.###", CultureInfo.InvariantCulture),
                    C9Csv(Capture.Status),
                    C9Csv(Capture.Review),
                    C9Csv(Capture.SourcePath),
                    C9Csv(Capture.NativePreviewPath),
                    C9Csv(EvidenceOverlayPath)
                });
            }
        }

        private sealed class MatchingC9CalibrationRow
        {
            public MatchingC9CalibrationRow(MatchingC9SyntheticCase expected, MatchingC9PreviewCapture capture)
            {
                Expected = expected;
                Capture = capture;
            }

            public MatchingC9SyntheticCase Expected { get; }
            public MatchingC9PreviewCapture Capture { get; }
            public double CenterErrorPx => Capture.CenterErrorPx;
            public double ScaleError => Math.Abs(Capture.Scale - Expected.Scale);
            public double AngleError => Math.Abs(Capture.Angle - Expected.Angle);
            public bool Pass => Capture.Parsed
                && Capture.ResultCount == 1
                && Capture.ExplicitRunContractPassed
                && CenterErrorPx <= 2D
                && ScaleError <= 0.051D
                && AngleError <= 0.51D;

            public string ToCsv()
            {
                return string.Join(",", new[]
                {
                    C9Csv(Expected.Name),
                    Expected.Scale.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Scale.ToString("0.###", CultureInfo.InvariantCulture),
                    ScaleError.ToString("0.###", CultureInfo.InvariantCulture),
                    Expected.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    AngleError.ToString("0.###", CultureInfo.InvariantCulture),
                    "320",
                    "240",
                    Capture.CenterX.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.CenterY.ToString("0.###", CultureInfo.InvariantCulture),
                    CenterErrorPx.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Score.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.ResultCount.ToString(CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountBeforeLoad.ToString(CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountAfterLoad.ToString(CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountAfterPreview.ToString(CultureInfo.InvariantCulture),
                    Capture.LoadRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.ExplicitPreviewRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.HasNativePreviewResult.ToString(),
                    Capture.NativePreviewCaptured.ToString(),
                    Capture.Parsed.ToString(),
                    Pass.ToString(),
                    C9Csv(Capture.Review),
                    C9Csv(Capture.SourcePath),
                    C9Csv(Capture.NativePreviewPath)
                });
            }
        }

        private sealed class MatchingC9EvidenceRow
        {
            public MatchingC9EvidenceRow(
                IReadOnlyDictionary<string, string> sourceRow,
                IReadOnlyDictionary<string, string> transform,
                MatchingC9PreviewCapture capture,
                double expectedCenterX,
                double expectedCenterY,
                double oracleScale,
                double oracleAngle,
                double stripAngle,
                System.Drawing.PointF[] expectedPolygon,
                string sourceSha256,
                string rowDirectory)
            {
                RowId = sourceRow["RowId"];
                Split = sourceRow["Split"];
                RoleLabelOnly = sourceRow["RoleLabelOnly"];
                Image = sourceRow["Image"];
                SourceSha256 = sourceSha256;
                Capture = capture;
                ExpectedCenterX = expectedCenterX;
                ExpectedCenterY = expectedCenterY;
                OracleScale = oracleScale;
                OracleAngle = oracleAngle;
                StripAngle = stripAngle;
                ExpectedPolygon = expectedPolygon;
                RowDirectory = rowDirectory;
                System.Drawing.PointF[] nativePolygon = capture.Parsed
                    ? CreateMatchingC9NativePolygon(capture)
                    : Array.Empty<System.Drawing.PointF>();
                PolygonIou = capture.Parsed
                    ? CalculateMatchingC9PolygonIou(expectedPolygon, nativePolygon, 640, 480)
                    : 0D;
                CenterInsideOraclePolygon = capture.Parsed
                    && IsMatchingC9CenterInside(expectedPolygon, capture.CenterX, capture.CenterY);
            }

            public string RowId { get; }
            public string Split { get; }
            public string RoleLabelOnly { get; }
            public string Image { get; }
            public string SourceSha256 { get; }
            public MatchingC9PreviewCapture Capture { get; }
            public double ExpectedCenterX { get; }
            public double ExpectedCenterY { get; }
            public double OracleScale { get; }
            public double OracleAngle { get; }
            public double StripAngle { get; }
            public System.Drawing.PointF[] ExpectedPolygon { get; }
            public string RowDirectory { get; }
            public string EvidenceOverlayPath { get; set; } = string.Empty;
            public double ScaleError => Math.Abs(Capture.Scale - OracleScale);
            public double AngleError => Math.Abs(Capture.Angle + StripAngle);
            public double OrbGlobalAngleError => Math.Abs(Capture.Angle + OracleAngle);
            public double PolygonIou { get; }
            public bool CenterInsideOraclePolygon { get; }
            public bool Pass => Capture.Parsed
                && Capture.ResultCount == 1
                && Capture.ExplicitRunContractPassed
                && Capture.CenterErrorPx <= 12D
                && CenterInsideOraclePolygon
                && PolygonIou >= 0.55D
                && ScaleError <= 0.08D
                && AngleError <= 1.5D;

            public string ToCsv(string templateSha256, string outputDirectory)
            {
                string sourceCopyPath = Path.GetRelativePath(outputDirectory, Path.Combine(RowDirectory, "source.jpg")).Replace('\\', '/');
                string nativePreviewPath = Path.GetRelativePath(outputDirectory, Capture.NativePreviewPath).Replace('\\', '/');
                string evidenceOverlayPath = string.IsNullOrWhiteSpace(EvidenceOverlayPath)
                    ? string.Empty
                    : Path.GetRelativePath(outputDirectory, EvidenceOverlayPath).Replace('\\', '/');
                return string.Join(",", new[]
                {
                    C9Csv(RowId),
                    C9Csv(Split),
                    C9Csv(RoleLabelOnly),
                    C9Csv(Image),
                    SourceSha256,
                    templateSha256,
                    Capture.ResultCount.ToString(CultureInfo.InvariantCulture),
                    Capture.Score.ToString("0.###", CultureInfo.InvariantCulture),
                    (Capture.Score / 100D).ToString("0.######", CultureInfo.InvariantCulture),
                    Capture.CenterX.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.CenterY.ToString("0.###", CultureInfo.InvariantCulture),
                    ExpectedCenterX.ToString("0.###", CultureInfo.InvariantCulture),
                    ExpectedCenterY.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.CenterErrorPx.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.BoxWidth.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.BoxHeight.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Scale.ToString("0.###", CultureInfo.InvariantCulture),
                    OracleScale.ToString("0.######", CultureInfo.InvariantCulture),
                    ScaleError.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.Angle.ToString("0.###", CultureInfo.InvariantCulture),
                    StripAngle.ToString("0.######", CultureInfo.InvariantCulture),
                    (-StripAngle).ToString("0.######", CultureInfo.InvariantCulture),
                    AngleError.ToString("0.###", CultureInfo.InvariantCulture),
                    OracleAngle.ToString("0.######", CultureInfo.InvariantCulture),
                    (-OracleAngle).ToString("0.######", CultureInfo.InvariantCulture),
                    OrbGlobalAngleError.ToString("0.###", CultureInfo.InvariantCulture),
                    CenterInsideOraclePolygon.ToString(),
                    PolygonIou.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.TactMs.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.WallElapsedMs.ToString("0.###", CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountBeforeLoad.ToString(CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountAfterLoad.ToString(CultureInfo.InvariantCulture),
                    Capture.PreviewRunCountAfterPreview.ToString(CultureInfo.InvariantCulture),
                    Capture.LoadRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.ExplicitPreviewRunDelta.ToString(CultureInfo.InvariantCulture),
                    Capture.HasNativePreviewResult.ToString(),
                    Capture.NativePreviewCaptured.ToString(),
                    Capture.Parsed.ToString(),
                    Pass.ToString(),
                    C9Csv(Capture.Review),
                    C9Csv(sourceCopyPath),
                    C9Csv(nativePreviewPath),
                    C9Csv(evidenceOverlayPath)
                });
            }
        }

        private static void RunEdgeBasedScaleMatching(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string repoRoot = FindRepositoryRoot();
            string samplePath = Path.Combine(repoRoot, "bin", "Debug", "EasyMatch", "BOARD.JPG");
            if (!File.Exists(samplePath))
            {
                throw new FileNotFoundException("EasyMatch BOARD sample image was not found.", samplePath);
            }

            string templatePath;
            string scaledSourcePath;
            OpenCvSharp.Rect templateRect;
            const double expectedScale = 0.90D;
            CreateEasyMatchScaleSmokeFiles(samplePath, outputDirectory, expectedScale, out templatePath, out scaledSourcePath, out templateRect);

            Application app = Application.Current ?? new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

            OpenVisionShellHostWindow window = null;
            try
            {
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.Recipe.Name = "Codex_EdgeBasedScaleSmoke";

                window = new OpenVisionShellHostWindow(runtimeContext)
                {
                    Width = 1500,
                    Height = 900,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                app.MainWindow = window;
                window.Show();
                Pump(24);

                OpenVisionShellHostView shellHost = window.ShellHostForSmoke
                    ?? throw new InvalidOperationException("OpenVision shell host was not created.");

                using (Bitmap scaledBitmap = new Bitmap(scaledSourcePath))
                {
                    shellHost.SetMainLayerImageForTest(scaledBitmap);
                }

                Pump(24);
                shellHost.SelectToolForTest(VISION_MENU.EdgeBasedMatching);
                Pump(60);
                shellHost.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath);
                Pump(12);
                shellHost.ConfigureActiveEdgeBasedMatchingForTest(ConfigureEdgeBasedScaleSmokeProperty);
                Pump(12);

                EdgeBasedMatchingProperty property = runtimeContext.Global.VisionTools.EdgeBasedMatchings.Count > 0
                    ? runtimeContext.Global.VisionTools.EdgeBasedMatchings[0]
                    : throw new InvalidOperationException("EdgeBasedMatching_1 repository property was not created.");

                string xmlPath = Path.Combine(outputDirectory, "EdgeBasedMatching_1.scale-smoke.xml");
                property.SaveTestConfig(xmlPath);
                EdgeBasedMatchingProperty loadedProperty = new EdgeBasedMatchingProperty("EdgeBasedMatching_1").LoadTestConfig(xmlPath);
                ValidateEdgeBasedScaleSmokeProperty(loadedProperty);

                shellHost.RunActiveNativePreviewForTest();
                Pump(120);

                string review = shellHost.ActiveNativeResultReviewText;
                string status = shellHost.ActiveNativeStatusText;
                if (!shellHost.HasNativePreviewResult
                    || review.IndexOf("Edge Match", StringComparison.OrdinalIgnoreCase) < 0
                    || review.IndexOf("Scale", StringComparison.OrdinalIgnoreCase) < 0
                    || review.IndexOf("0.9", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException(
                        "EdgeBased scale matching did not report the expected scale result."
                        + Environment.NewLine
                        + "Status: " + status
                        + Environment.NewLine
                        + "Review: " + review);
                }

                using (Bitmap preview = shellHost.GetLayerImageCloneForTest("EdgeBasedMatching_Preview"))
                {
                    if (preview == null)
                    {
                        throw new InvalidOperationException("EdgeBasedMatching_Preview output layer was not created.");
                    }

                    preview.Save(Path.Combine(outputDirectory, "EdgeBasedMatching_Preview.png"));
                }

                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_EdgeBasedScaleMatching.png"));
                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: edge-based-scale-matching" + Environment.NewLine
                    + "Status: " + status + Environment.NewLine
                    + "Review: " + review + Environment.NewLine
                    + "TemplateRect: " + templateRect.X.ToString(CultureInfo.InvariantCulture)
                    + "," + templateRect.Y.ToString(CultureInfo.InvariantCulture)
                    + "," + templateRect.Width.ToString(CultureInfo.InvariantCulture)
                    + "," + templateRect.Height.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "ExpectedScale: " + expectedScale.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "XmlReloadedScale: "
                    + loadedProperty.FIND_SCALE_MIN.ToString("0.###", CultureInfo.InvariantCulture)
                    + ".." + loadedProperty.FIND_SCALE_MAX.ToString("0.###", CultureInfo.InvariantCulture)
                    + " step " + loadedProperty.FIND_SCALE_STEP.ToString("0.###", CultureInfo.InvariantCulture) + Environment.NewLine
                    + "Template: " + templatePath + Environment.NewLine
                    + "ScaledSource: " + scaledSourcePath + Environment.NewLine,
                    Encoding.UTF8);
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }

                app.Shutdown();
            }
        }

        private static PropertyGridEditorRuntime GetPropertyGridEditorRuntime()
        {
            FieldInfo runtimeField = typeof(PropertyGridEditorFactory).GetField(
                "runtime",
                BindingFlags.Static | BindingFlags.NonPublic);
            return runtimeField?.GetValue(null) as PropertyGridEditorRuntime
                ?? throw new InvalidOperationException("PropertyGrid editor runtime was not available.");
        }

        private static bool ClickPropertyGridRoiButton(Application app, Window window)
        {
            Button button = FindDialogButtonForProperty(app, "CvROI");
            if (button == null)
            {
                throw new InvalidOperationException("Could not find the actual PropertyGrid CvROI dialog button.");
            }

            bool openedRoiEditor = false;
            string previousSuppressMessageBox = Environment.GetEnvironmentVariable("OPENVISIONLAB_SUPPRESS_MESSAGEBOX");
            Environment.SetEnvironmentVariable("OPENVISIONLAB_SUPPRESS_MESSAGEBOX", "1");

            DispatcherTimer closeTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            closeTimer.Tick += (sender, e) =>
            {
                    foreach (Window openWindow in app.Windows)
                    {
                        if (openWindow is RoiEditorWindow)
                        {
                            openedRoiEditor = true;
                            AcceptDialogWindow(openWindow);
                            closeTimer.Stop();
                            break;
                        }
                }
            };

            try
            {
                closeTimer.Start();
                MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                {
                    RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                    Source = button
                };
                button.RaiseEvent(args);
                Pump(40);
            }
            finally
            {
                closeTimer.Stop();
                Environment.SetEnvironmentVariable("OPENVISIONLAB_SUPPRESS_MESSAGEBOX", previousSuppressMessageBox);
            }

            if (!openedRoiEditor)
            {
                throw new InvalidOperationException("Actual PropertyGrid CvROI button did not open the ROI editor.");
            }

            return true;
        }

        private static Button FindDialogButtonForProperty(Application app, string propertyName)
        {
            foreach (Window openWindow in app.Windows)
            {
                Button button = FindDialogButtonForProperty(openWindow, propertyName);
                if (button != null)
                {
                    return button;
                }
            }

            return null;
        }

        private static Button FindDialogButtonForProperty(DependencyObject root, string propertyName)
        {
            foreach (Button button in FindVisualChildren<Button>(root))
            {
                if (IsDialogButtonForProperty(button, propertyName))
                {
                    return button;
                }
            }

            return null;
        }

        private static bool IsDialogButtonForProperty(Button button, string propertyName)
        {
            if (button == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            foreach (object candidate in EnumerateDialogPropertyCandidates(button))
            {
                string resolvedName = ResolvePropertyName(candidate);
                if (string.Equals(resolvedName, propertyName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<object> EnumerateDialogPropertyCandidates(Button button)
        {
            if (button == null)
            {
                yield break;
            }

            yield return button.DataContext;
            yield return button.CommandParameter;

            DependencyObject current = button;
            while (current != null)
            {
                if (current is FrameworkElement frameworkElement)
                {
                    yield return frameworkElement.DataContext;
                }

                current = GetParent(current);
            }
        }

        private static string ResolvePropertyName(object candidate)
        {
            if (candidate == null)
            {
                return string.Empty;
            }

            object propertyValue = null;
            string typeName = candidate.GetType().FullName ?? string.Empty;
            if (typeName.EndsWith(".PropertyItemValue", StringComparison.Ordinal))
            {
                propertyValue = candidate;
            }
            else if (typeName.EndsWith(".PropertyItem", StringComparison.Ordinal))
            {
                propertyValue = ReadObjectProperty(candidate, "PropertyValue");
            }
            else if (ReadObjectProperty(candidate, "ParentProperty") != null)
            {
                propertyValue = candidate;
            }

            object parent = ReadObjectProperty(propertyValue, "ParentProperty");
            PropertyDescriptor descriptor = ReadObjectProperty(parent, "PropertyDescriptor") as PropertyDescriptor;
            if (!string.IsNullOrWhiteSpace(descriptor?.Name))
            {
                return descriptor.Name;
            }

            return ReadObjectProperty(parent, "Name") as string ?? string.Empty;
        }

        private static object ReadObjectProperty(object instance, string propertyName)
        {
            if (instance == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            try
            {
                PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                return property?.GetValue(instance, null);
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject root) where T : DependencyObject
        {
            if (root == null)
            {
                yield break;
            }

            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                foreach (T descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private static T FindNamedVisualChild<T>(DependencyObject root, string name, string scenario)
            where T : FrameworkElement
        {
            T element = FindVisualChildren<T>(root)
                .FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.Ordinal));
            if (element == null)
            {
                throw new InvalidOperationException(scenario + " could not find element '" + name + "'.");
            }

            return element;
        }

        private static void AssertSelectedListBoxItemVisible(DependencyObject root, string automationId, object selectedItem, string scenario)
        {
            ListBox listBox = FindVisualChildren<ListBox>(root)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    automationId,
                    StringComparison.Ordinal));
            if (listBox == null)
            {
                throw new InvalidOperationException(scenario + " could not find list box '" + automationId + "'.");
            }

            listBox.UpdateLayout();
            Pump(8);
            if (!Equals(listBox.SelectedItem, selectedItem))
            {
                throw new InvalidOperationException(scenario + " did not select the expected list item.");
            }

            ListBoxItem itemContainer = listBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
            if (itemContainer == null)
            {
                listBox.UpdateLayout();
                Pump(12);
                itemContainer = listBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
            }

            if (itemContainer == null || !itemContainer.IsVisible || itemContainer.ActualWidth <= 0D || itemContainer.ActualHeight <= 0D)
            {
                throw new InvalidOperationException(scenario + " did not show the selected list item.");
            }
        }

        private static void AssertVisibleAutomationIds(DependencyObject root, string scenario, params string[] requiredIds)
        {
            HashSet<string> visibleIds = FindVisualChildren<FrameworkElement>(root)
                .Where(item => item.IsVisible)
                .Select(System.Windows.Automation.AutomationProperties.GetAutomationId)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToHashSet(StringComparer.Ordinal);

            string missing = requiredIds.FirstOrDefault(id => !visibleIds.Contains(id));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    scenario + " did not show expected AutomationId '" + missing + "'. "
                    + "VisibleIds='" + string.Join(", ", visibleIds.OrderBy(item => item, StringComparer.Ordinal)) + "'");
            }
        }

        private static void AssertNotVisibleAutomationIds(DependencyObject root, string scenario, params string[] hiddenIds)
        {
            HashSet<string> visibleIds = FindVisualChildren<FrameworkElement>(root)
                .Where(item => item.IsVisible)
                .Select(System.Windows.Automation.AutomationProperties.GetAutomationId)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToHashSet(StringComparer.Ordinal);

            string visible = hiddenIds.FirstOrDefault(id => visibleIds.Contains(id));
            if (!string.IsNullOrWhiteSpace(visible))
            {
                throw new InvalidOperationException(
                    scenario + " still showed AutomationId '" + visible + "'. "
                    + "VisibleIds='" + string.Join(", ", visibleIds.OrderBy(item => item, StringComparer.Ordinal)) + "'");
            }
        }

        private static void AssertHostedPropertyGridRowsRendered(DependencyObject root, string scenario)
        {
            System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(root)
                .FirstOrDefault(item => item.IsVisible && item.ActualWidth >= 200D && item.ActualHeight >= 100D);
            if (grid == null)
            {
                throw new InvalidOperationException(scenario + " was not visible.");
            }

            object selectedObject = grid.SelectedObject;
            int descriptorCount = selectedObject == null
                ? 0
                : TypeDescriptor.GetProperties(selectedObject).Count;
            int renderedTextCount = FindVisualChildren<TextBlock>(grid)
                .Count(item => item.IsVisible
                    && !string.IsNullOrWhiteSpace(item.Text)
                    && !string.Equals(item.Text, "Search", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(item.Text, "검색", StringComparison.OrdinalIgnoreCase));

            if (selectedObject == null || descriptorCount <= 0 || renderedTextCount < 2)
            {
                throw new InvalidOperationException(
                    scenario + " did not render property rows. "
                    + $"Selected={selectedObject?.GetType().Name ?? "<null>"}, Descriptors={descriptorCount}, RenderedTextBlocks={renderedTextCount}");
            }
        }

        private static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                return null;
            }

            try
            {
                return VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
            }
            catch
            {
                return LogicalTreeHelper.GetParent(current);
            }
        }

        private static PropertyItemValue CreateBridgePropertyItemValue(object innerValue)
        {
            ConstructorInfo constructor = typeof(PropertyItemValue).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(object) },
                null);
            return constructor?.Invoke(new[] { innerValue }) as PropertyItemValue
                ?? throw new InvalidOperationException("Could not create a bridge PropertyItemValue.");
        }

        private static void AcceptDialogWindow(Window window)
        {
            if (window == null)
            {
                return;
            }

            try
            {
                window.DialogResult = true;
            }
            catch
            {
                window.Close();
            }
        }

        private static void CreateEasyMatchScaleSmokeFiles(
            string samplePath,
            string outputDirectory,
            double scale,
            out string templatePath,
            out string scaledSourcePath,
            out OpenCvSharp.Rect templateRect)
        {
            using (OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(samplePath, OpenCvSharp.ImreadModes.Grayscale))
            {
                if (source.Empty())
                {
                    throw new InvalidOperationException("EasyMatch BOARD sample could not be loaded.");
                }

                templateRect = SelectHighEdgeTemplateRect(source, 120, 90);
                using (OpenCvSharp.Mat template = source.SubMat(templateRect).Clone())
                using (OpenCvSharp.Mat scaledSource = ResizeWholeImage(source, scale))
                {
                    templatePath = Path.Combine(outputDirectory, "Board_template.bmp");
                    scaledSourcePath = Path.Combine(outputDirectory, "Board_scale_0p90.bmp");
                    OpenCvSharp.Cv2.ImWrite(templatePath, template);
                    OpenCvSharp.Cv2.ImWrite(scaledSourcePath, scaledSource);
                }
            }
        }

        private static void ConfigureEdgeBasedScaleSmokeProperty(EdgeBasedMatchingProperty property)
        {
            // This smoke validates the real WPF/EXE path for opt-in scale search; keep values narrow and deterministic.
            property.SCORE_MIN = 0.0D;
            property.NUM_MATCH = 5;
            property.CANNY_LOW = 30;
            property.CANNY_HIGH = 90;
            property.CANNY_APERTURE_SIZE = 3;
            property.USE_L2_GRADIENT = true;
            property.CONTOUR_RETRIEVAL_MODE = OpenCvSharp.RetrievalModes.External;
            property.CONTOUR_APPROXIMATION_MODE = OpenCvSharp.ContourApproximationModes.ApproxNone;
            property.USE_FIND_ANGLE = false;
            property.USE_FIND_SCALE = true;
            property.FIND_SCALE_MIN = 0.85D;
            property.FIND_SCALE_MAX = 0.95D;
            property.FIND_SCALE_STEP = 0.05D;
            property.SEARCH_STEP = 4;
            property.USE_POSITION_REFINE = true;
            property.USE_SUBPIXEL_REFINE = true;
            property.GREEDINESS = 0.9D;
            property.USE_PYRAMID_POSITION_PROPOSAL = false;
            property.USE_HYBRID_VERIFY = true;
            property.HYBRID_VERIFY_TOP_N = 8;
            property.HYBRID_VERIFY_IMAGE_WEIGHT = 0.45D;
            property.MAX_TEMPLATE_POINTS = 260;
            property.MIN_GRADIENT_MAGNITUDE = 1.0D;
            property.USE_DRAW_IMAGE = true;
            property.ReloadTemplateImage();
        }

        private static void ConfigureImageMatchingScaleComparisonProperty(MatchingProperty property)
        {
            property.AUTO_PREVIEW = false;
            property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
            property.SCORE_MIN = 0.0D;
            property.NUM_MATCH = 5;
            property.MAGNIFIATION = 1.0D;
            property.USE_FIND_ANGLE = false;
            property.USE_FIND_SCALE = true;
            property.FIND_SCALE_MIN = 0.85D;
            property.FIND_SCALE_MAX = 0.95D;
            property.FIND_SCALE_STEP = 0.05D;
            property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
            property.USE_CANNY = false;
            property.USE_PADDING_COLOR_WHITE = false;
            property.ReloadTemplateImage();
        }

        private static void ConfigureTutorialMatchingProperty(MatchingProperty property)
        {
            property.AUTO_PREVIEW = false;
            property.MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed;
            property.SCORE_MIN = 0.60D;
            property.NUM_MATCH = 3;
            property.MAGNIFIATION = 1.0D;
            property.USE_FIND_ANGLE = true;
            property.FIND_ANGLE_MIN = -20;
            property.FIND_ANGLE_MAX = 7;
            property.FIND_ANGLE = 0.5D;
            property.USE_FIND_SCALE = false;
            property.USE_COARSE_TO_FINE_ANGLE_SEARCH = false;
            property.USE_CANNY = false;
            property.ReloadTemplateImage();
        }

        private static void ConfigureImageMatchingPyramidScaleProperty(MatchingProperty property)
        {
            // Pyramid proposal is a candidate-pruning option for scale search; keep it explicit and off by default elsewhere.
            ConfigureImageMatchingScaleComparisonProperty(property);
            property.USE_PYRAMID_POSITION_PROPOSAL = true;
            property.PYRAMID_POSITION_TOP_N = 8;
            property.PYRAMID_POSITION_MIN_SCORE = 0.70D;
            property.ReloadTemplateImage();
        }

        private static void ValidateEdgeBasedScaleSmokeProperty(EdgeBasedMatchingProperty property)
        {
            if (property == null
                || !property.USE_FIND_SCALE
                || !property.USE_SUBPIXEL_REFINE
                || Math.Abs(property.FIND_SCALE_MIN - 0.85D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_MAX - 0.95D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_STEP - 0.05D) > 0.000001D)
            {
                throw new InvalidOperationException("EdgeBased scale settings did not survive XML save/load.");
            }
        }

        private static void ValidateImageMatchingPyramidScaleProperty(MatchingProperty property)
        {
            if (property == null
                || !property.USE_FIND_SCALE
                || Math.Abs(property.FIND_SCALE_MIN - 0.85D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_MAX - 0.95D) > 0.000001D
                || Math.Abs(property.FIND_SCALE_STEP - 0.05D) > 0.000001D
                || property.USE_FIND_ANGLE
                || !property.USE_PYRAMID_POSITION_PROPOSAL
                || property.PYRAMID_POSITION_TOP_N != 8
                || Math.Abs(property.PYRAMID_POSITION_MIN_SCORE - 0.70D) > 0.000001D)
            {
                throw new InvalidOperationException("Image Matching pyramid scale settings did not survive XML save/load.");
            }
        }

        private static OpenCvSharp.Rect SelectHighEdgeTemplateRect(OpenCvSharp.Mat image, int desiredWidth, int desiredHeight)
        {
            int width = Math.Min(desiredWidth, Math.Max(32, image.Width / 2));
            int height = Math.Min(desiredHeight, Math.Max(32, image.Height / 2));
            width = Math.Min(width, image.Width - 2);
            height = Math.Min(height, image.Height - 2);

            using (OpenCvSharp.Mat edges = new OpenCvSharp.Mat())
            {
                OpenCvSharp.Cv2.Canny(image, edges, 40, 120);
                int stepX = Math.Max(8, width / 5);
                int stepY = Math.Max(8, height / 5);
                int marginX = Math.Max(1, width / 4);
                int marginY = Math.Max(1, height / 4);
                OpenCvSharp.Rect best = new OpenCvSharp.Rect(
                    Math.Max(0, image.Width / 2 - width / 2),
                    Math.Max(0, image.Height / 2 - height / 2),
                    width,
                    height);
                int bestScore = -1;

                for (int y = marginY; y <= image.Height - height - marginY; y += stepY)
                {
                    for (int x = marginX; x <= image.Width - width - marginX; x += stepX)
                    {
                        OpenCvSharp.Rect rect = new OpenCvSharp.Rect(x, y, width, height);
                        using (OpenCvSharp.Mat roi = edges.SubMat(rect))
                        {
                            int score = OpenCvSharp.Cv2.CountNonZero(roi);
                            if (score > bestScore)
                            {
                                bestScore = score;
                                best = rect;
                            }
                        }
                    }
                }

                return best;
            }
        }

        private static OpenCvSharp.Mat ResizeWholeImage(OpenCvSharp.Mat source, double scale)
        {
            OpenCvSharp.Mat resized = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Resize(
                source,
                resized,
                new OpenCvSharp.Size(
                    Math.Max(1, (int)Math.Round(source.Width * scale)),
                    Math.Max(1, (int)Math.Round(source.Height * scale))),
                0D,
                0D,
                scale < 1D ? OpenCvSharp.InterpolationFlags.Area : OpenCvSharp.InterpolationFlags.Linear);
            return resized;
        }

        private static string FormatCenterError(string review, double expectedCenterX, double expectedCenterY)
        {
            if (!TryExtractCenter(review, out double centerX, out double centerY))
            {
                return "n/a";
            }

            double dx = centerX - expectedCenterX;
            double dy = centerY - expectedCenterY;
            return Math.Sqrt((dx * dx) + (dy * dy)).ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static double CalculateCenterError(string review, double expectedCenterX, double expectedCenterY)
        {
            if (!TryExtractCenter(review, out double centerX, out double centerY))
            {
                throw new InvalidOperationException("Review did not contain a center value: " + review);
            }

            double dx = centerX - expectedCenterX;
            double dy = centerY - expectedCenterY;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        private static bool TryExtractCenter(string review, out double centerX, out double centerY)
        {
            centerX = 0D;
            centerY = 0D;
            Match match = Regex.Match(
                review ?? string.Empty,
                @"Center\s+(?<x>-?[0-9]+(?:\.[0-9]+)?),\s*(?<y>-?[0-9]+(?:\.[0-9]+)?)",
                RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return false;
            }

            return double.TryParse(match.Groups["x"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out centerX)
                && double.TryParse(match.Groups["y"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out centerY);
        }

        private sealed class FakePropertyItemValueInner
        {
            public object Value { get; set; }
            public string StringValue { get; set; } = string.Empty;
            public FakePropertyItemInner ParentProperty { get; } = new FakePropertyItemInner();
        }

        private sealed class FakePropertyItemInner
        {
            public bool IsReadOnly { get; set; }
        }

        private static void ConfigurePinsVerticalDistanceLine(
            OpenVisionShellHostView shellHost,
            string setting,
            string projectionDirection)
        {
            shellHost.SetActiveLineSettingForTest(setting);
            Pump(6);
            shellHost.SetActiveSelectedLineRoiForTest(430, 170, 125, 145);
            Pump(6);
            shellHost.ConfigureActiveSelectedLineForTest(projectionDirection, "WTOB", "X_RTOL");
            Pump(6);

            // Direct EXE smoke uses the same runtime but sets deterministic gauge parameters explicitly.
            shellHost.ConfigureActiveSelectedLineMeasureTuningForTest(
                useThreshold: false,
                useAdaptiveThreshold: false,
                contrast: 18D,
                thickness: 2D,
                samplingStep: 6D,
                pointRange: 8,
                useManualAngle: true,
                manualAngleValue: 89D);
            shellHost.ConfigureActiveSelectedLineDrawForTest(
                showVerticalLine: true,
                showEdge: true,
                showContour: false,
                showFitLine: true);
            Pump(10);
        }

        private static Bitmap CreateDockFloatSmokeBitmap()
        {
            Bitmap bitmap = new Bitmap(320, 240);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", 24F, System.Drawing.FontStyle.Bold))
            {
                graphics.Clear(System.Drawing.Color.White);
                graphics.FillRectangle(System.Drawing.Brushes.Black, 24, 28, 72, 48);
                graphics.DrawRectangle(System.Drawing.Pens.Black, 120, 36, 120, 82);
                graphics.DrawEllipse(System.Drawing.Pens.Black, 58, 132, 96, 64);
                graphics.DrawString("OVL", font, System.Drawing.Brushes.Black, 164, 140);
            }

            return bitmap;
        }

        private static Bitmap CreateDockingVerificationBitmap(string text, int index)
        {
            Bitmap bitmap = new Bitmap(320, 240);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", 24F, System.Drawing.FontStyle.Bold))
            using (System.Drawing.Pen darkPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(30, 30, 30), 3F))
            using (System.Drawing.Pen accentPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 128 + (index % 80), 180), 4F))
            {
                graphics.Clear(System.Drawing.Color.White);
                graphics.DrawRectangle(darkPen, 22, 22, 276, 196);
                graphics.DrawEllipse(accentPen, 54, 54, 124, 82);
                graphics.DrawLine(accentPen, 36, 186, 286, 42);
                graphics.DrawString(text ?? "DOCK", font, System.Drawing.Brushes.Black, 96, 132);
            }

            return bitmap;
        }

        private static void PrepareDockingVerificationLayers(OpenVisionShellHostView shellHost)
        {
            using (Bitmap bitmap = CreateDockFloatSmokeBitmap())
            {
                shellHost.SetMainLayerImageForTest(bitmap);
            }

            Pump(24);
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(32);
            shellHost.RunActiveNativePreviewForTest();
            Pump(48);

            if (!shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Docking verification setup did not create HSV_Preview. "
                    + "WorkspaceLayer=" + shellHost.WorkspaceLayerTitle);
            }

            shellHost.CloseActiveWpfToolWindowForTest();
            Pump(12);
        }

        private static void EnsureDockingVerificationLayer(
            OpenVisionShellHostView shellHost,
            string layerTitle,
            string text,
            int index)
        {
            if (shellHost.HasLayerForTest(layerTitle))
            {
                return;
            }

            using (Bitmap bitmap = CreateDockingVerificationBitmap(text, index))
            {
                if (!shellHost.AddLayerImageForTest(layerTitle, bitmap))
                {
                    throw new InvalidOperationException("Docking verification could not create layer " + layerTitle + ".");
                }
            }

            Pump(12);
        }

        private static void ResetDockingVerificationDocuments(
            OpenVisionShellHostView shellHost,
            params string[] layerTitles)
        {
            AssertDockingVerificationLayerImages(shellHost, "before reset clear", layerTitles);
            shellHost.ClearDockedLayersForTest();
            Pump(18);
            if (shellHost.DockedLayerCount != 0)
            {
                throw new InvalidOperationException(
                    "Docking verification reset did not clear docked documents. "
                    + $"Actual={shellHost.DockedLayerCount}, Titles={shellHost.DockedLayerTitles}");
            }

            foreach (string layerTitle in layerTitles ?? Array.Empty<string>())
            {
                if (!shellHost.DockLayerForTest(layerTitle))
                {
                    throw new InvalidOperationException(
                        "Docking verification could not dock "
                        + layerTitle
                        + ". LayerImages="
                        + DescribeDockingVerificationLayerImages(shellHost, layerTitles));
                }
            }

            Pump(28);
            AssertDockingVerificationLayerImages(shellHost, "after reset redock", layerTitles);
        }

        private static void SetupPaneLocalSideVerification(OpenVisionShellHostView shellHost, string localLayerTitle)
        {
            ResetDockingVerificationDocuments(shellHost, "Main", "HSV_Preview", localLayerTitle);
            if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
            {
                throw new InvalidOperationException("Docking verification could not create the outside right pane for " + localLayerTitle + ".");
            }

            Pump(28);
        }

        private static bool ContainsDockedLayerTitle(OpenVisionShellHostView shellHost, string layerTitle)
        {
            if (shellHost == null || string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return (shellHost.DockedLayerTitles ?? string.Empty)
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(title => string.Equals(title.Trim(), layerTitle, StringComparison.OrdinalIgnoreCase));
        }

        private static void AssertDockingVerificationStage(
            OpenVisionShellHostView shellHost,
            Window window,
            StringBuilder report,
            string stage,
            int expectedLayerCount,
            int minimumPaneCount,
            string expectedRootOrientation,
            int minimumTopHeaderCount)
        {
            if (shellHost.DockedLayerCount != expectedLayerCount)
            {
                throw new InvalidOperationException(
                    stage + " docked layer count mismatch. "
                    + $"Expected={expectedLayerCount}, Actual={shellHost.DockedLayerCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (shellHost.DockedLayerPaneCount < minimumPaneCount)
            {
                throw new InvalidOperationException(
                    stage + " pane count is too low. "
                    + $"ExpectedMin={minimumPaneCount}, Actual={shellHost.DockedLayerPaneCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!string.IsNullOrWhiteSpace(expectedRootOrientation)
                && !string.Equals(shellHost.DockedLayerRootOrientationForTest, expectedRootOrientation, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    stage + " root orientation mismatch. "
                    + $"Expected={expectedRootOrientation}, Actual={shellHost.DockedLayerRootOrientationForTest}, Titles={shellHost.DockedLayerTitles}");
            }

            if (shellHost.DockedLayerTextureTileCount < expectedLayerCount)
            {
                throw new InvalidOperationException(
                    stage + " did not keep every docked layer rendered. "
                    + $"ExpectedTiles={expectedLayerCount}, ActualTiles={shellHost.DockedLayerTextureTileCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.AreDockedLayerViewersCompactForTest || !shellHost.AreDockedLayerViewersCompactSizeReadyForTest)
            {
                throw new InvalidOperationException(stage + " docked viewers are not in compact comparison mode.");
            }

            if (!shellHost.AreDockedLayersNativeFloatingDisabledForTest)
            {
                throw new InvalidOperationException(stage + " docked layer documents must suppress AvalonDock native floating previews.");
            }

            if (!shellHost.AreDockedLayerTabHeadersGestureReadyForTest
                || !shellHost.AreDockedLayerTabHeadersReadableForTest
                || !shellHost.AreDockedLayerTabHeaderGripsReadyForTest)
            {
                throw new InvalidOperationException(
                    stage + " dock headers are not ready for drag, selection, and reading. "
                    + $"HeaderCount={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}, Headers={shellHost.DockedLayerTabHeaderDiagnosticsForTest}");
            }

            OpenVisionDockingVisualSnapshot snapshot = shellHost.DockedLayerVisualSnapshotForTest;
            AssertDockingVisualSnapshot(snapshot, stage, minimumPaneCount, minimumTopHeaderCount);
            AssertVisibleLayerViewerBounds(
                window,
                Math.Min(expectedLayerCount, Math.Max(1, shellHost.DockedLayerPaneCount)),
                stage);

            report.AppendLine("[" + stage + "]");
            report.AppendLine(
                "DockedLayers="
                + shellHost.DockedLayerCount.ToString(CultureInfo.InvariantCulture)
                + ", Panes="
                + shellHost.DockedLayerPaneCount.ToString(CultureInfo.InvariantCulture)
                + ", Tiles="
                + shellHost.DockedLayerTextureTileCount.ToString(CultureInfo.InvariantCulture)
                + ", RootOrientation="
                + shellHost.DockedLayerRootOrientationForTest
                + ", Titles="
                + shellHost.DockedLayerTitles);
            report.Append(snapshot.ToReport());
            report.AppendLine();
        }

        private static void AssertDockingVerificationStageAfterMouseDrag(
            OpenVisionShellHostView shellHost,
            Window window,
            StringBuilder report,
            string stage,
            int expectedLayerCount,
            int minimumPaneCount,
            string expectedRootOrientation,
            int minimumTopHeaderCount,
            Action retryMouseDrag)
        {
            try
            {
                AssertDockingVerificationStage(
                    shellHost,
                    window,
                    report,
                    stage,
                    expectedLayerCount,
                    minimumPaneCount,
                    expectedRootOrientation,
                    minimumTopHeaderCount);
                return;
            }
            catch (InvalidOperationException ex)
            {
                if (retryMouseDrag == null)
                {
                    throw;
                }

                report.AppendLine("[" + stage + "_retry]");
                report.AppendLine("FirstAttempt=" + ex.Message);
                retryMouseDrag();
            }

            AssertDockingVerificationStage(
                shellHost,
                window,
                report,
                stage,
                expectedLayerCount,
                minimumPaneCount,
                expectedRootOrientation,
                minimumTopHeaderCount);
        }

        private static void AssertDockingVisualSnapshot(
            OpenVisionDockingVisualSnapshot snapshot,
            string stage,
            int minimumPaneCount,
            int minimumTopHeaderCount)
        {
            if (snapshot == null || snapshot.WorkspaceSize.Width <= 0D || snapshot.WorkspaceSize.Height <= 0D)
            {
                throw new InvalidOperationException(stage + " did not expose a valid docking workspace snapshot.");
            }

            if (snapshot.Panes.Count < minimumPaneCount)
            {
                throw new InvalidOperationException(
                    stage + " visual pane snapshot count is too low. "
                    + $"ExpectedMin={minimumPaneCount}, Actual={snapshot.Panes.Count}. "
                    + "Panes="
                    + string.Join(" || ", snapshot.Panes.Select(pane => pane.ToReportLine()))
                    + " Headers="
                    + string.Join(" || ", snapshot.Headers.Select(header => header.ToReportLine())));
            }

            List<OpenVisionDockingVisualElementSnapshot> headers = snapshot.Headers.ToList();
            if (headers.Count < minimumTopHeaderCount)
            {
                throw new InvalidOperationException(
                    stage + " does not expose enough top dock headers. "
                    + $"ExpectedMin={minimumTopHeaderCount}, Actual={headers.Count}");
            }

            foreach (OpenVisionDockingVisualElementSnapshot pane in snapshot.Panes)
            {
                if (pane.Bounds.Left < -2D
                    || pane.Bounds.Top < -2D
                    || pane.Bounds.Right > snapshot.WorkspaceSize.Width + 2D
                    || pane.Bounds.Bottom > snapshot.WorkspaceSize.Height + 2D
                    || pane.Bounds.Width < 180D
                    || pane.Bounds.Height < 140D)
                {
                    throw new InvalidOperationException(
                        stage + " pane bounds are invalid or collapsed. "
                        + pane.ToReportLine());
                }
            }

            for (int i = 0; i < snapshot.Panes.Count; i++)
            {
                Rect a = snapshot.Panes[i].Bounds;
                for (int j = i + 1; j < snapshot.Panes.Count; j++)
                {
                    Rect b = snapshot.Panes[j].Bounds;
                    Rect overlap = Rect.Intersect(a, b);
                    if (!overlap.IsEmpty && overlap.Width > 8D && overlap.Height > 8D)
                    {
                        throw new InvalidOperationException(
                            stage + " pane bounds overlap. "
                            + snapshot.Panes[i].ToReportLine()
                            + " / "
                            + snapshot.Panes[j].ToReportLine());
                    }
                }
            }

            foreach (OpenVisionDockingVisualElementSnapshot header in headers)
            {
                if (header.PaneIndex < 0 || header.PaneIndex >= snapshot.Panes.Count)
                {
                    throw new InvalidOperationException(stage + " header is not attached to a pane. " + header.ToReportLine());
                }

                if (header.Bounds.Width < 72D || header.Bounds.Height < 20D)
                {
                    throw new InvalidOperationException(stage + " header is too small for reliable drag/reading. " + header.ToReportLine());
                }

                Rect paneBounds = snapshot.Panes[header.PaneIndex].Bounds;
                if (header.Bounds.Top > paneBounds.Top + 72D)
                {
                    throw new InvalidOperationException(
                        stage + " header is not top-aligned like a Visual Studio dock tab/title. "
                        + header.ToReportLine()
                        + " pane="
                        + snapshot.Panes[header.PaneIndex].ToReportLine());
                }

                if (paneBounds.Height > 120D && header.Bounds.Bottom > paneBounds.Bottom - 32D)
                {
                    throw new InvalidOperationException(
                        stage + " header appears at the bottom of the pane. "
                        + header.ToReportLine()
                        + " pane="
                        + snapshot.Panes[header.PaneIndex].ToReportLine());
                }
            }
        }

        private static void AssertVisibleLayerViewerBounds(Window window, int expectedLayerCount, string stage)
        {
            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            if (bounds.Count < expectedLayerCount)
            {
                throw new InvalidOperationException(
                    stage + " does not have all visible docked layer viewers. "
                    + $"Expected={expectedLayerCount}, Actual={bounds.Count}, Titles={string.Join("|", bounds.Keys)}, "
                    + "Viewers="
                    + DescribeLayerViewers(window));
            }

            List<KeyValuePair<string, Rect>> entries = bounds.ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                Rect a = entries[i].Value;
                if (a.Width < 180D || a.Height < 110D)
                {
                    throw new InvalidOperationException(
                        stage + " viewer collapsed too far. "
                        + $"{entries[i].Key}={FormatRect(a)}");
                }

                for (int j = i + 1; j < entries.Count; j++)
                {
                    Rect b = entries[j].Value;
                    Rect overlap = Rect.Intersect(a, b);
                    if (!overlap.IsEmpty && overlap.Width > 8D && overlap.Height > 8D)
                    {
                        throw new InvalidOperationException(
                            stage + " visible layer viewers overlap. "
                            + $"{entries[i].Key}={FormatRect(a)} / {entries[j].Key}={FormatRect(b)}");
                    }
                }
            }
        }

        private static void AssertGlobalBottomSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string bottomLayerTitle,
            string referenceLayerTitle)
        {
            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect bottom = GetRequiredLayerBounds(bounds, bottomLayerTitle, "global bottom");
            Rect reference = GetRequiredLayerBounds(bounds, referenceLayerTitle, "global bottom");
            double workspaceWidth = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize.Width;

            if (bottom.Top <= reference.Top + 40D)
            {
                throw new InvalidOperationException(
                    "GlobalBottom must place the layer below the existing workspace group. "
                    + $"{bottomLayerTitle}={FormatRect(bottom)}, {referenceLayerTitle}={FormatRect(reference)}");
            }

            if (bottom.Width < Math.Min(reference.Width * 0.85D, workspaceWidth * 0.70D))
            {
                throw new InvalidOperationException(
                    "GlobalBottom must take the workspace-wide bottom area, not only a pane-local bottom. "
                    + $"{bottomLayerTitle}={FormatRect(bottom)}, WorkspaceWidth={workspaceWidth:0.0}");
            }
        }

        private static void AssertGlobalTopSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string topLayerTitle,
            string referenceLayerTitle)
        {
            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect top = GetRequiredLayerBounds(bounds, topLayerTitle, "global top");
            Rect reference = GetRequiredLayerBounds(bounds, referenceLayerTitle, "global top");
            double workspaceWidth = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize.Width;

            if (GetRectCenterY(top) >= GetRectCenterY(reference) - 40D)
            {
                throw new InvalidOperationException(
                    "GlobalTop must place the layer above the existing workspace group. "
                    + $"{topLayerTitle}={FormatRect(top)}, {referenceLayerTitle}={FormatRect(reference)}");
            }

            if (top.Width < Math.Min(reference.Width * 0.85D, workspaceWidth * 0.70D))
            {
                throw new InvalidOperationException(
                    "GlobalTop must take the workspace-wide top area, not only a pane-local top. "
                    + $"{topLayerTitle}={FormatRect(top)}, WorkspaceWidth={workspaceWidth:0.0}");
            }
        }

        private static void AssertGlobalLeftSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string leftLayerTitle,
            string referenceLayerTitle)
        {
            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect left = GetRequiredLayerBounds(bounds, leftLayerTitle, "global left");
            Rect reference = GetRequiredLayerBounds(bounds, referenceLayerTitle, "global left");
            double workspaceHeight = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize.Height;

            if (GetRectCenterX(left) >= GetRectCenterX(reference) - 40D)
            {
                throw new InvalidOperationException(
                    "GlobalLeft must place the layer left of the existing workspace group. "
                    + $"{leftLayerTitle}={FormatRect(left)}, {referenceLayerTitle}={FormatRect(reference)}");
            }

            if (left.Height < Math.Min(reference.Height * 0.85D, workspaceHeight * 0.70D))
            {
                throw new InvalidOperationException(
                    "GlobalLeft must take the workspace-wide left area, not only a pane-local left. "
                    + $"{leftLayerTitle}={FormatRect(left)}, WorkspaceHeight={workspaceHeight:0.0}");
            }
        }

        private static void AssertGlobalRightSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string rightLayerTitle,
            string referenceLayerTitle)
        {
            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect right = GetRequiredLayerBounds(bounds, rightLayerTitle, "global right");
            Rect reference = GetRequiredLayerBounds(bounds, referenceLayerTitle, "global right");
            double workspaceHeight = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize.Height;

            if (GetRectCenterX(right) <= GetRectCenterX(reference) + 40D)
            {
                throw new InvalidOperationException(
                    "GlobalRight must place the layer right of the existing workspace group. "
                    + $"{rightLayerTitle}={FormatRect(right)}, {referenceLayerTitle}={FormatRect(reference)}");
            }

            if (right.Height < Math.Min(reference.Height * 0.85D, workspaceHeight * 0.70D))
            {
                throw new InvalidOperationException(
                    "GlobalRight must take the workspace-wide right area, not only a pane-local right. "
                    + $"{rightLayerTitle}={FormatRect(right)}, WorkspaceHeight={workspaceHeight:0.0}");
            }
        }

        private static void AssertPaneLocalSideSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string localLayerTitle,
            string paneReferenceLayerTitle,
            string outsidePaneLayerTitle,
            string sideName)
        {
            if (shellHost.DockedLayerNestedLayoutPanelCountForTest < 1)
            {
                throw new InvalidOperationException(
                    "Pane-local " + sideName + " must create a nested panel inside the target pane. "
                    + $"NestedPanels={shellHost.DockedLayerNestedLayoutPanelCountForTest}");
            }

            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect local = GetRequiredLayerBounds(bounds, localLayerTitle, "pane-local " + sideName);
            Rect paneReference = GetRequiredLayerBounds(bounds, paneReferenceLayerTitle, "pane-local " + sideName);
            Rect outsidePane = GetRequiredLayerBounds(bounds, outsidePaneLayerTitle, "pane-local " + sideName);
            System.Windows.Size workspaceSize = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize;

            if (string.Equals(sideName, "Left", StringComparison.OrdinalIgnoreCase)
                && GetRectCenterX(local) >= GetRectCenterX(paneReference) - 40D)
            {
                throw new InvalidOperationException(
                    "Pane-local Left must be left of the target pane content. "
                    + $"{localLayerTitle}={FormatRect(local)}, {paneReferenceLayerTitle}={FormatRect(paneReference)}");
            }

            if (string.Equals(sideName, "Right", StringComparison.OrdinalIgnoreCase)
                && GetRectCenterX(local) <= GetRectCenterX(paneReference) + 40D)
            {
                throw new InvalidOperationException(
                    "Pane-local Right must be right of the target pane content. "
                    + $"{localLayerTitle}={FormatRect(local)}, {paneReferenceLayerTitle}={FormatRect(paneReference)}");
            }

            if (string.Equals(sideName, "Top", StringComparison.OrdinalIgnoreCase)
                && GetRectCenterY(local) >= GetRectCenterY(paneReference) - 40D)
            {
                throw new InvalidOperationException(
                    "Pane-local Top must be above the target pane content. "
                    + $"{localLayerTitle}={FormatRect(local)}, {paneReferenceLayerTitle}={FormatRect(paneReference)}");
            }

            if ((string.Equals(sideName, "Left", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(sideName, "Right", StringComparison.OrdinalIgnoreCase))
                && local.Width >= workspaceSize.Width * 0.75D)
            {
                throw new InvalidOperationException(
                    "Pane-local " + sideName + " is taking too much width and looks like a global side dock. "
                    + $"{localLayerTitle}={FormatRect(local)}, WorkspaceWidth={workspaceSize.Width:0.0}");
            }

            if (string.Equals(sideName, "Top", StringComparison.OrdinalIgnoreCase)
                && local.Width >= workspaceSize.Width * 0.75D)
            {
                throw new InvalidOperationException(
                    "Pane-local Top is taking too much width and looks like GlobalTop. "
                    + $"{localLayerTitle}={FormatRect(local)}, WorkspaceWidth={workspaceSize.Width:0.0}");
            }

            Rect targetOverlap = Rect.Intersect(local, paneReference);
            if (!targetOverlap.IsEmpty && targetOverlap.Width > 2D && targetOverlap.Height > 2D)
            {
                throw new InvalidOperationException(
                    "Pane-local " + sideName + " overlaps the target pane content. "
                    + $"{localLayerTitle}={FormatRect(local)}, {paneReferenceLayerTitle}={FormatRect(paneReference)}");
            }

            Rect outsideOverlap = Rect.Intersect(local, outsidePane);
            if (!outsideOverlap.IsEmpty && outsideOverlap.Width > 2D && outsideOverlap.Height > 2D)
            {
                throw new InvalidOperationException(
                    "Pane-local " + sideName + " intrudes into the adjacent workspace pane. "
                    + $"{localLayerTitle}={FormatRect(local)}, {outsidePaneLayerTitle}={FormatRect(outsidePane)}");
            }
        }

        private static void AssertPaneLocalBottomSemantics(
            OpenVisionShellHostView shellHost,
            Window window,
            string localBottomLayerTitle,
            string paneReferenceLayerTitle,
            string outsidePaneLayerTitle)
        {
            if (shellHost.DockedLayerNestedLayoutPanelCountForTest < 1)
            {
                throw new InvalidOperationException(
                    "Pane-local Bottom must create a nested panel inside the target pane. "
                    + $"NestedPanels={shellHost.DockedLayerNestedLayoutPanelCountForTest}");
            }

            Dictionary<string, Rect> bounds = GetVisibleLayerViewerBounds(window);
            Rect localBottom = GetRequiredLayerBounds(bounds, localBottomLayerTitle, "pane-local bottom");
            Rect paneReference = GetRequiredLayerBounds(bounds, paneReferenceLayerTitle, "pane-local bottom");
            Rect outsidePane = GetRequiredLayerBounds(bounds, outsidePaneLayerTitle, "pane-local bottom");
            double workspaceWidth = shellHost.DockedLayerVisualSnapshotForTest.WorkspaceSize.Width;

            if (localBottom.Top <= paneReference.Top + 40D)
            {
                throw new InvalidOperationException(
                    "Pane-local Bottom must be below the target pane content. "
                    + $"{localBottomLayerTitle}={FormatRect(localBottom)}, {paneReferenceLayerTitle}={FormatRect(paneReference)}");
            }

            if (localBottom.Width >= workspaceWidth * 0.75D)
            {
                throw new InvalidOperationException(
                    "Pane-local Bottom is taking too much width and looks like GlobalBottom. "
                    + $"{localBottomLayerTitle}={FormatRect(localBottom)}, WorkspaceWidth={workspaceWidth:0.0}");
            }

            if (localBottom.Width < workspaceWidth * 0.35D)
            {
                throw new InvalidOperationException(
                    "Pane-local Bottom viewer must stretch across the target pane instead of hugging the left edge. "
                    + $"{localBottomLayerTitle}={FormatRect(localBottom)}, WorkspaceWidth={workspaceWidth:0.0}");
            }

            Rect overlap = Rect.Intersect(localBottom, outsidePane);
            if (!overlap.IsEmpty && overlap.Width > 2D && overlap.Height > 2D)
            {
                throw new InvalidOperationException(
                    "Pane-local Bottom intrudes into the adjacent workspace pane. "
                    + $"{localBottomLayerTitle}={FormatRect(localBottom)}, {outsidePaneLayerTitle}={FormatRect(outsidePane)}");
            }
        }

        private static Rect GetRequiredLayerBounds(Dictionary<string, Rect> bounds, string layerTitle, string scenario)
        {
            if (bounds.TryGetValue(layerTitle, out Rect rect))
            {
                return rect;
            }

            throw new InvalidOperationException(
                scenario + " did not expose a visible viewer for " + layerTitle + ". "
                + "Visible=" + string.Join("|", bounds.Keys));
        }

        private static Dictionary<string, Rect> GetVisibleLayerViewerBounds(Window window)
        {
            return FindVisualChildren<OpenVisionLayerViewerView>(window)
                .Where(viewer => viewer.IsVisible
                    && viewer.HasImage
                    && !string.IsNullOrWhiteSpace(viewer.LayerTitle)
                    && viewer.ActualWidth > 0D
                    && viewer.ActualHeight > 0D)
                .GroupBy(viewer => viewer.LayerTitle, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        OpenVisionLayerViewerView viewer = group
                            .OrderByDescending(item => item.ActualWidth * item.ActualHeight)
                            .First();
                        System.Windows.Point topLeft = viewer.TranslatePoint(new System.Windows.Point(0D, 0D), window);
                        return new Rect(topLeft, new System.Windows.Size(viewer.ActualWidth, viewer.ActualHeight));
                    },
                    StringComparer.OrdinalIgnoreCase);
        }

        private static string DescribeLayerViewers(Window window)
        {
            List<string> viewers = FindVisualChildren<OpenVisionLayerViewerView>(window)
                .Select((viewer, index) =>
                {
                    System.Windows.Point topLeft = viewer.TranslatePoint(new System.Windows.Point(0D, 0D), window);
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "#{0}:{1},Visible={2},HasImage={3},Texture={4},Image={5}x{6},Bounds={7}",
                        index,
                        string.IsNullOrWhiteSpace(viewer.LayerTitle) ? "<empty>" : viewer.LayerTitle,
                        viewer.IsVisible,
                        viewer.HasImage,
                        viewer.TextureTileCount,
                        viewer.ImagePixelWidth,
                        viewer.ImagePixelHeight,
                        FormatRect(new Rect(topLeft, new System.Windows.Size(viewer.ActualWidth, viewer.ActualHeight))));
                })
                .ToList();
            return viewers.Count == 0 ? "<none>" : string.Join(" || ", viewers);
        }

        private static void AssertDockingVerificationLayerImages(
            OpenVisionShellHostView shellHost,
            string stage,
            params string[] layerTitles)
        {
            string missing = string.Join(
                "|",
                (layerTitles ?? Array.Empty<string>())
                    .Where(title => !HasDockingVerificationLayerImage(shellHost, title)));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Docking verification layer image missing at "
                    + stage
                    + ". Missing="
                    + missing
                    + ", LayerImages="
                    + DescribeDockingVerificationLayerImages(shellHost, layerTitles));
            }
        }

        private static bool HasDockingVerificationLayerImage(OpenVisionShellHostView shellHost, string layerTitle)
        {
            try
            {
                using Bitmap image = shellHost.GetLayerImageCloneForTest(layerTitle);
                return image != null && image.Width > 0 && image.Height > 0;
            }
            catch
            {
                return false;
            }
        }

        private static string DescribeDockingVerificationLayerImages(
            OpenVisionShellHostView shellHost,
            params string[] layerTitles)
        {
            return string.Join(
                ", ",
                (layerTitles ?? Array.Empty<string>())
                    .Select(title => title + "=" + DescribeDockingVerificationLayerImage(shellHost, title)));
        }

        private static string DescribeDockingVerificationLayerImage(OpenVisionShellHostView shellHost, string layerTitle)
        {
            try
            {
                using Bitmap image = shellHost.GetLayerImageCloneForTest(layerTitle);
                return image == null
                    ? "null"
                    : image.Width.ToString(CultureInfo.InvariantCulture)
                        + "x"
                        + image.Height.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return "error:" + ex.GetType().Name;
            }
        }

        private static string FormatRect(Rect rect)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "x={0:0.0},y={1:0.0},w={2:0.0},h={3:0.0}",
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height);
        }

        private static double GetRectCenterX(Rect rect)
        {
            return rect.Left + (rect.Width * 0.5D);
        }

        private static double GetRectCenterY(Rect rect)
        {
            return rect.Top + (rect.Height * 0.5D);
        }

        private static System.Windows.Point GetRectCenter(Rect rect)
        {
            return new System.Windows.Point(GetRectCenterX(rect), GetRectCenterY(rect));
        }

        private static OpenVisionDockingVisualElementSnapshot FindDockingHeaderSnapshot(
            OpenVisionDockingVisualSnapshot snapshot,
            string title,
            string scenario)
        {
            if (snapshot == null)
            {
                throw new InvalidOperationException(scenario + " did not expose a docking visual snapshot.");
            }

            OpenVisionDockingVisualElementSnapshot header = snapshot.TabHeaders
                .Concat(snapshot.PaneHeaders)
                .FirstOrDefault(item => string.Equals(item.Title, title, StringComparison.OrdinalIgnoreCase));
            if (header != null)
            {
                return header;
            }

            throw new InvalidOperationException(
                scenario + " could not find a visible dock header for " + title + ". Headers="
                + string.Join(" || ", snapshot.Headers.Select(item => item.ToReportLine())));
        }

        private static void DragDockedLayerHeaderToWorkspacePoint(
            OpenVisionShellHostView shellHost,
            StringBuilder report,
            string stage,
            string layerTitle,
            System.Windows.Point targetWorkspacePoint)
        {
            OpenVisionDockingVisualSnapshot snapshot = shellHost.DockedLayerVisualSnapshotForTest;
            OpenVisionDockingVisualElementSnapshot sourceHeader = FindDockingHeaderSnapshot(snapshot, layerTitle, stage);
            System.Windows.Point sourceWorkspacePoint = GetRectCenter(sourceHeader.Bounds);
            double sourceStartOffset = sourceWorkspacePoint.X + 28D < sourceHeader.Bounds.Right - 2D
                ? 28D
                : -28D;
            System.Windows.Point dragStartConfirmationWorkspacePoint = new System.Windows.Point(
                sourceWorkspacePoint.X + sourceStartOffset,
                sourceWorkspacePoint.Y);
            System.Windows.Point sourceScreenPoint = shellHost.GetDockedWorkspaceScreenPointForTest(
                sourceWorkspacePoint.X,
                sourceWorkspacePoint.Y);
            System.Windows.Point dragStartConfirmationScreenPoint = shellHost.GetDockedWorkspaceScreenPointForTest(
                dragStartConfirmationWorkspacePoint.X,
                dragStartConfirmationWorkspacePoint.Y);
            System.Windows.Point targetScreenPoint = shellHost.GetDockedWorkspaceScreenPointForTest(
                targetWorkspacePoint.X,
                targetWorkspacePoint.Y);

            report.AppendLine("[" + stage + "]");
            report.AppendLine("SourceLayer=" + layerTitle);
            report.AppendLine("SourceHeader=" + sourceHeader.ToReportLine());
            report.AppendLine("SourceWorkspace=" + FormatPoint(sourceWorkspacePoint));
            report.AppendLine("TargetWorkspace=" + FormatPoint(targetWorkspacePoint));
            report.AppendLine("SourceScreen=" + FormatPoint(sourceScreenPoint));
            report.AppendLine("DragStartConfirmationScreen=" + FormatPoint(dragStartConfirmationScreenPoint));
            report.AppendLine("TargetScreen=" + FormatPoint(targetScreenPoint));

            DragMouseViaPointOnBackgroundThread(sourceScreenPoint, dragStartConfirmationScreenPoint, targetScreenPoint);
            Pump(72);
        }

        private static void ClickDockedLayerHeaderWithoutDrag(
            OpenVisionShellHostView shellHost,
            StringBuilder report,
            string stage,
            string layerTitle)
        {
            shellHost.HideDockingGuideForTest();
            Pump(8);

            OpenVisionDockingVisualSnapshot snapshot = shellHost.DockedLayerVisualSnapshotForTest;
            OpenVisionDockingVisualElementSnapshot sourceHeader = FindDockingHeaderSnapshot(snapshot, layerTitle, stage);
            System.Windows.Point sourceWorkspacePoint = GetRectCenter(sourceHeader.Bounds);
            System.Windows.Point sourceScreenPoint = shellHost.GetDockedWorkspaceScreenPointForTest(
                sourceWorkspacePoint.X,
                sourceWorkspacePoint.Y);

            report.AppendLine("[" + stage + "]");
            report.AppendLine("SourceLayer=" + layerTitle);
            report.AppendLine("SourceHeader=" + sourceHeader.ToReportLine());
            report.AppendLine("SourceWorkspace=" + FormatPoint(sourceWorkspacePoint));
            report.AppendLine("SourceScreen=" + FormatPoint(sourceScreenPoint));

            int sourceX = RoundToScreenPixel(sourceScreenPoint.X);
            int sourceY = RoundToScreenPixel(sourceScreenPoint.Y);
            mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
            Thread.Sleep(80);
            SetCursorPosOrThrow(sourceX, sourceY);
            Thread.Sleep(120);

            try
            {
                mouse_event(MouseEventLeftDown, 0U, 0U, 0U, UIntPtr.Zero);
                Pump(14);
                report.AppendLine("GuideVisibleDuringMouseDown=" + shellHost.IsDockingGuideOverlayVisibleForTest);
                report.AppendLine("ActiveGuideDuringMouseDown=" + shellHost.ActiveDockingGuideZoneForTest);
                if (shellHost.IsDockingGuideOverlayVisibleForTest)
                {
                    throw new InvalidOperationException(stage + " showed the docking guide on tab mouse down without drag.");
                }
            }
            finally
            {
                mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
            }

            Pump(36);
            report.AppendLine("GuideVisibleAfterClick=" + shellHost.IsDockingGuideOverlayVisibleForTest);
            report.AppendLine("ActiveGuideAfterClick=" + shellHost.ActiveDockingGuideZoneForTest);
            report.AppendLine();
            if (shellHost.IsDockingGuideOverlayVisibleForTest)
            {
                throw new InvalidOperationException(stage + " left the docking guide visible after tab click.");
            }
        }

        private static void DragHostLayerTabToWorkspace(
            OpenVisionShellHostView shellHost,
            StringBuilder report,
            string stage,
            string layerTitle)
        {
            System.Windows.Point sourceScreenPoint = GetHostLayerTabScreenPoint(shellHost, layerTitle);
            System.Windows.Point targetScreenPoint = GetNamedElementScreenPoint(
                shellHost,
                "wpfLayerWorkspace",
                0.50D,
                0.42D,
                stage);

            report.AppendLine("[" + stage + "]");
            report.AppendLine("SourceLayer=" + layerTitle);
            report.AppendLine("SourceScreen=" + FormatPoint(sourceScreenPoint));
            report.AppendLine("TargetScreen=" + FormatPoint(targetScreenPoint));

            System.Windows.Point dragStartConfirmationPoint = new System.Windows.Point(
                sourceScreenPoint.X + 28D,
                sourceScreenPoint.Y);
            report.AppendLine("DragStartConfirmationScreen=" + FormatPoint(dragStartConfirmationPoint));

            DragMouseViaPointOnBackgroundThread(sourceScreenPoint, dragStartConfirmationPoint, targetScreenPoint);
            Pump(96);
        }

        private static System.Windows.Point GetHostLayerTabScreenPoint(OpenVisionShellHostView shellHost, string layerTitle)
        {
            shellHost.UpdateLayout();
            Pump(4);

            ListBox listBox = FindVisualChildren<ListBox>(shellHost)
                .FirstOrDefault(item => item.Items.Cast<object>().Any(row => row is OpenVisionShellHostLayerTabItem));
            if (listBox == null)
            {
                throw new InvalidOperationException("Could not find the host layer tab list.");
            }

            object rowItem = listBox.Items
                .Cast<object>()
                .FirstOrDefault(item =>
                    item is OpenVisionShellHostLayerTabItem row
                    && string.Equals(row.Title, layerTitle, StringComparison.OrdinalIgnoreCase));
            if (rowItem == null)
            {
                throw new InvalidOperationException("Could not find host layer tab row for " + layerTitle + ".");
            }

            listBox.ScrollIntoView(rowItem);
            listBox.UpdateLayout();
            Pump(8);

            ListBoxItem itemContainer = listBox.ItemContainerGenerator.ContainerFromItem(rowItem) as ListBoxItem;
            if (itemContainer == null)
            {
                listBox.UpdateLayout();
                Pump(12);
                itemContainer = listBox.ItemContainerGenerator.ContainerFromItem(rowItem) as ListBoxItem;
            }

            if (itemContainer == null || itemContainer.ActualWidth <= 0D || itemContainer.ActualHeight <= 0D)
            {
                throw new InvalidOperationException("Could not resolve a visible host layer tab container for " + layerTitle + ".");
            }

            return itemContainer.PointToScreen(new System.Windows.Point(
                Math.Max(2D, itemContainer.ActualWidth * 0.5D),
                Math.Max(2D, itemContainer.ActualHeight * 0.5D)));
        }

        private static System.Windows.Point GetNamedElementScreenPoint(
            DependencyObject root,
            string elementName,
            double xRatio,
            double yRatio,
            string scenario)
        {
            FrameworkElement element = FindVisualChildren<FrameworkElement>(root)
                .FirstOrDefault(item => string.Equals(item.Name, elementName, StringComparison.Ordinal));
            if (element == null || element.ActualWidth <= 0D || element.ActualHeight <= 0D)
            {
                throw new InvalidOperationException(scenario + " could not find a visible element named " + elementName + ".");
            }

            return element.PointToScreen(new System.Windows.Point(
                Math.Max(1D, element.ActualWidth * xRatio),
                Math.Max(1D, element.ActualHeight * yRatio)));
        }

        private static System.Windows.Point CreateGlobalMouseDragTarget(
            OpenVisionDockingVisualSnapshot snapshot,
            DockingGuideZone zone)
        {
            if (snapshot == null || snapshot.WorkspaceSize.Width <= 0D || snapshot.WorkspaceSize.Height <= 0D)
            {
                throw new InvalidOperationException("Mouse drag global target could not read a valid docking workspace size.");
            }

            const double edgeInset = 12D;
            double centerX = snapshot.WorkspaceSize.Width * 0.5D;
            double centerY = snapshot.WorkspaceSize.Height * 0.5D;
            return zone switch
            {
                DockingGuideZone.GlobalLeft => new System.Windows.Point(
                    Math.Min(edgeInset, Math.Max(1D, snapshot.WorkspaceSize.Width * 0.05D)),
                    centerY),
                DockingGuideZone.GlobalRight => new System.Windows.Point(
                    Math.Max(1D, snapshot.WorkspaceSize.Width - edgeInset),
                    centerY),
                DockingGuideZone.GlobalTop => new System.Windows.Point(
                    centerX,
                    Math.Min(edgeInset, Math.Max(1D, snapshot.WorkspaceSize.Height * 0.05D))),
                DockingGuideZone.GlobalBottom => new System.Windows.Point(
                    centerX,
                    Math.Max(1D, snapshot.WorkspaceSize.Height - edgeInset)),
                _ => throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone is not a global docking target.")
            };
        }

        private static System.Windows.Point CreatePaneCompassMouseDragTarget(
            OpenVisionDockingVisualSnapshot snapshot,
            string targetPaneReferenceLayerTitle,
            DockingGuideZone zone,
            string scenario)
        {
            OpenVisionDockingVisualElementSnapshot referenceHeader = FindDockingHeaderSnapshot(
                snapshot,
                targetPaneReferenceLayerTitle,
                scenario + " target pane reference");
            if (referenceHeader.PaneIndex < 0 || referenceHeader.PaneIndex >= snapshot.Panes.Count)
            {
                throw new InvalidOperationException(
                    scenario + " could not resolve the target pane for "
                    + targetPaneReferenceLayerTitle
                    + ". Header="
                    + referenceHeader.ToReportLine());
            }

            return CreatePaneCompassMouseDragTarget(snapshot.Panes[referenceHeader.PaneIndex].Bounds, zone);
        }

        private static System.Windows.Point CreatePaneCompassMouseDragTarget(Rect paneBounds, DockingGuideZone zone)
        {
            if (paneBounds.IsEmpty || paneBounds.Width <= 0D || paneBounds.Height <= 0D)
            {
                throw new InvalidOperationException("Mouse drag pane-local target could not read a valid pane bounds.");
            }

            const double paneEdgeTargetRatio = 0.20D;
            double x = paneBounds.Left + (paneBounds.Width * 0.5D);
            double y = paneBounds.Top + (paneBounds.Height * 0.5D);

            switch (zone)
            {
                case DockingGuideZone.Left:
                    x = paneBounds.Left + (paneBounds.Width * paneEdgeTargetRatio);
                    break;
                case DockingGuideZone.Right:
                    x = paneBounds.Right - (paneBounds.Width * paneEdgeTargetRatio);
                    break;
                case DockingGuideZone.Top:
                    y = paneBounds.Top + (paneBounds.Height * paneEdgeTargetRatio);
                    break;
                case DockingGuideZone.Bottom:
                    y = paneBounds.Bottom - (paneBounds.Height * paneEdgeTargetRatio);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, "Zone is not a pane-local docking target.");
            }

            return new System.Windows.Point(x, y);
        }

        private static string FormatPoint(System.Windows.Point point)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "x={0:0.0},y={1:0.0}",
                point.X,
                point.Y);
        }

        private static void DragMouseOnBackgroundThread(System.Windows.Point sourceScreenPoint, System.Windows.Point targetScreenPoint)
        {
            Exception inputException = null;
            Thread inputThread = new Thread(() =>
            {
                try
                {
                    SendMouseDrag(sourceScreenPoint, targetScreenPoint);
                }
                catch (Exception ex)
                {
                    inputException = ex;
                }
            });
            inputThread.IsBackground = true;
            inputThread.Name = "OpenVisionDockingMouseDragSmoke";
            inputThread.Start();

            DateTime deadline = DateTime.UtcNow.AddSeconds(8D);
            while (inputThread.IsAlive && DateTime.UtcNow < deadline)
            {
                Pump(1);
            }

            if (inputThread.IsAlive)
            {
                throw new TimeoutException("Mouse drag input thread did not finish within the expected time.");
            }

            inputThread.Join();
            if (inputException != null)
            {
                throw new InvalidOperationException("Mouse drag input failed.", inputException);
            }
        }

        private static void DragMouseViaPointOnBackgroundThread(
            System.Windows.Point sourceScreenPoint,
            System.Windows.Point viaScreenPoint,
            System.Windows.Point targetScreenPoint)
        {
            Exception inputException = null;
            Thread inputThread = new Thread(() =>
            {
                try
                {
                    SendMouseDragThroughPoints(sourceScreenPoint, viaScreenPoint, targetScreenPoint);
                }
                catch (Exception ex)
                {
                    inputException = ex;
                }
            });
            inputThread.IsBackground = true;
            inputThread.Name = "OpenVisionHostTabMouseDragSmoke";
            inputThread.Start();

            DateTime deadline = DateTime.UtcNow.AddSeconds(8D);
            while (inputThread.IsAlive && DateTime.UtcNow < deadline)
            {
                Pump(1);
            }

            if (inputThread.IsAlive)
            {
                throw new TimeoutException("Host tab mouse drag input thread did not finish within the expected time.");
            }

            inputThread.Join();
            if (inputException != null)
            {
                throw new InvalidOperationException("Host tab mouse drag input failed.", inputException);
            }
        }

        private static void SendMouseDrag(System.Windows.Point sourceScreenPoint, System.Windows.Point targetScreenPoint)
        {
            int sourceX = RoundToScreenPixel(sourceScreenPoint.X);
            int sourceY = RoundToScreenPixel(sourceScreenPoint.Y);
            int targetX = RoundToScreenPixel(targetScreenPoint.X);
            int targetY = RoundToScreenPixel(targetScreenPoint.Y);

            mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
            Thread.Sleep(80);
            SetCursorPosOrThrow(sourceX, sourceY);
            Thread.Sleep(160);
            mouse_event(MouseEventLeftDown, 0U, 0U, 0U, UIntPtr.Zero);
            Thread.Sleep(120);

            const int steps = 34;
            for (int step = 1; step <= steps; step++)
            {
                double ratio = step / (double)steps;
                int x = RoundToScreenPixel(sourceX + ((targetX - sourceX) * ratio));
                int y = RoundToScreenPixel(sourceY + ((targetY - sourceY) * ratio));
                SetCursorPosOrThrow(x, y);
                Thread.Sleep(18);
            }

            Thread.Sleep(180);
            mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
        }

        private static void SendMouseDragThroughPoints(params System.Windows.Point[] screenPoints)
        {
            if (screenPoints == null || screenPoints.Length < 2)
            {
                throw new ArgumentException("At least two screen points are required.", nameof(screenPoints));
            }

            int sourceX = RoundToScreenPixel(screenPoints[0].X);
            int sourceY = RoundToScreenPixel(screenPoints[0].Y);

            mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
            Thread.Sleep(80);
            SetCursorPosOrThrow(sourceX, sourceY);
            Thread.Sleep(180);
            mouse_event(MouseEventLeftDown, 0U, 0U, 0U, UIntPtr.Zero);
            Thread.Sleep(120);

            for (int segment = 1; segment < screenPoints.Length; segment++)
            {
                int fromX = RoundToScreenPixel(screenPoints[segment - 1].X);
                int fromY = RoundToScreenPixel(screenPoints[segment - 1].Y);
                int toX = RoundToScreenPixel(screenPoints[segment].X);
                int toY = RoundToScreenPixel(screenPoints[segment].Y);
                int steps = segment == 1 ? 10 : 34;

                for (int step = 1; step <= steps; step++)
                {
                    double ratio = step / (double)steps;
                    int x = RoundToScreenPixel(fromX + ((toX - fromX) * ratio));
                    int y = RoundToScreenPixel(fromY + ((toY - fromY) * ratio));
                    SetCursorPosOrThrow(x, y);
                    Thread.Sleep(segment == 1 ? 24 : 18);
                }
            }

            Thread.Sleep(180);
            mouse_event(MouseEventLeftUp, 0U, 0U, 0U, UIntPtr.Zero);
        }

        private static void SetCursorPosOrThrow(int x, int y)
        {
            if (!SetCursorPos(x, y))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetCursorPos failed.");
            }
        }

        private static int RoundToScreenPixel(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static bool IsFloatingToolWindow(Window window)
        {
            return window != null && string.Equals(window.GetType().Name, "OpenVisionFloatingToolWindow", StringComparison.Ordinal);
        }

        private static int CountVisibleFloatingToolWindows()
        {
            return Application.Current.Windows
                .OfType<Window>()
                .Count(item => item.IsVisible && IsFloatingToolWindow(item));
        }

        private static void AssertToolWindowState(
            OpenVisionShellHostView shellHost,
            string expectedViewTypeName,
            bool docked,
            int floatingWindowCount,
            string scenario)
        {
            int actualFloatingWindowCount = CountVisibleFloatingToolWindows();
            bool actualDocked = shellHost.IsDockedToolInspectorVisibleForTest;
            string actualTypeName = docked
                ? shellHost.ActiveWpfToolWindowTypeName ?? string.Empty
                : shellHost.ActiveNativeDocumentTypeName ?? string.Empty;
            if (!shellHost.IsActiveWpfToolWindowVisibleForTest
                || !shellHost.IsNativeDocumentActive
                || actualDocked != docked
                || actualFloatingWindowCount != floatingWindowCount
                || actualTypeName.IndexOf(expectedViewTypeName, StringComparison.Ordinal) < 0)
            {
                throw new InvalidOperationException(
                    scenario + " did not keep a single active WPF tool window. "
                    + $"ExpectedType={expectedViewTypeName}, ActualType={actualTypeName}, "
                    + $"ExpectedDocked={docked}, ActualDocked={actualDocked}, "
                    + $"ExpectedFloatingWindows={floatingWindowCount}, ActualFloatingWindows={actualFloatingWindowCount}, "
                    + $"NativeActive={shellHost.IsNativeDocumentActive}");
            }
        }

        private static void AssertDockedWidth(OpenVisionShellHostView shellHost, double expectedWidth, string scenario)
        {
            double actualWidth = shellHost.DockedToolInspectorWidthForTest;
            if (Math.Abs(actualWidth - expectedWidth) > 2D)
            {
                throw new InvalidOperationException(
                    scenario + " did not preserve the operator-adjusted dock width. "
                    + $"Expected={expectedWidth:0.0}, Actual={actualWidth:0.0}");
            }
        }

        private static void ValidatePinsMeasurePipelineStep(VisionPipelineStep step)
        {
            if (step == null
                || !string.Equals(step.ToolType, "LineDistance", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Line_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.TryGetValue("LeftCvROI", out string leftRoi)
                || !string.Equals(leftRoi, "430,170,125,145", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("RightCvROI", out string rightRoi)
                || !string.Equals(rightRoi, "430,170,125,145", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("LeftPRJ_DIR", out string leftDirection)
                || !string.Equals(leftDirection, "X_LTOR", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("RightPRJ_DIR", out string rightDirection)
                || !string.Equals(rightDirection, "X_RTOL", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("LeftMANUAL_ANGLE_VALUE", out string manualAngle)
                || manualAngle.IndexOf("89", StringComparison.Ordinal) < 0)
            {
                throw new InvalidOperationException("Line pins measure did not create the expected LineDistance pipeline step.");
            }
        }

        private static string BuildPassReport(string review, string status, double measuredMm, VisionPipelineStep step)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Result: PASS");
            builder.AppendLine("Scenario: line-pins-measure");
            builder.AppendLine("Status: " + status);
            builder.AppendLine("Review: " + review);
            builder.AppendLine("MeasuredMm: " + measuredMm.ToString("0.###", CultureInfo.InvariantCulture));
            builder.AppendLine("ToolType: " + step.ToolType);
            builder.AppendLine("InputLayer: " + step.InputLayer);
            builder.AppendLine("OutputLayer: " + step.OutputLayer);
            return builder.ToString();
        }

        private static double ExtractMm(string review)
        {
            Match match = Regex.Match(review ?? string.Empty, @"(?<value>[0-9]+(?:\.[0-9]+)?)\s*mm", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new InvalidOperationException("Line measure review did not contain an mm value: " + review);
            }

            return double.Parse(match.Groups["value"].Value, CultureInfo.InvariantCulture);
        }

        private static VisionPipeline CreateDirectSmokePipeline(string name, int stepCount)
        {
            VisionPipeline pipeline = new VisionPipeline { Name = name };
            for (int index = 0; index < stepCount; index++)
            {
                pipeline.Steps.Add(new VisionPipelineStep
                {
                    Name = name + "_Step_" + (index + 1).ToString(CultureInfo.InvariantCulture),
                    ToolType = "Threshold",
                    InputLayer = index == 0 ? "Main" : name + "_Preview_" + index.ToString(CultureInfo.InvariantCulture),
                    OutputLayer = name + "_Preview_" + (index + 1).ToString(CultureInfo.InvariantCulture)
                });
            }

            return pipeline;
        }

        private static string SerializePipelineToXmlText(VisionPipeline pipeline)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VisionPipeline));
                serializer.Serialize(writer, pipeline);
                return writer.ToString();
            }
        }

        private static void AssertLlmDraftBlocked(
            OpenVisionShellHostView shellHost,
            VisionPipeline draftPipeline,
            string scenario,
            params string[] expectedValidationFragments)
        {
            string selectedPipelineBeforeImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
            shellHost.RecipeCommands.LlmXmlDraftText = SerializePipelineToXmlText(draftPipeline);
            if (shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest())
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML " + scenario + " draft unexpectedly passed validation. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
            }

            foreach (string fragment in expectedValidationFragments ?? Array.Empty<string>())
            {
                string[] alternatives = (fragment ?? string.Empty)
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .ToArray();
                if (alternatives.Length == 0)
                {
                    continue;
                }

                if (!alternatives.Any(item => shellHost.RecipeCommands.LlmXmlDraftValidationReport.Contains(item, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Recipe manager LLM XML " + scenario + " failure report missed expected text '" + fragment + "'. "
                        + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
                }
            }

            if (shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
            {
                shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
                Pump(40);
            }

            string selectedPipelineAfterImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
            if (!string.Equals(selectedPipelineBeforeImport, selectedPipelineAfterImport, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML " + scenario + " import attempt changed pipeline state. "
                    + $"Before='{selectedPipelineBeforeImport}', After='{selectedPipelineAfterImport}', "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
            }
        }

        private static void RunRuntimeDataRootContract(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string dataRoot = Path.GetFullPath(
                AppPathService.DataRootDirectory);
            string installationRoot = Path.GetFullPath(
                AppPathService.InstallationRootDirectory);
            if (string.Equals(
                    dataRoot,
                    installationRoot,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Runtime data-root smoke requires an external "
                    + AppPathService.DataRootEnvironmentVariable
                    + " value.");
            }

            string recipeName =
                "Smoke_RuntimeDataRoot_"
                + Guid.NewGuid().ToString("N").Substring(0, 12);
            string relativeTemplate = string.Empty;
            try
            {
                string templateDirectory =
                    RecipeWorkspaceService.GetTemplateDirectory(recipeName);
                string templatePath = Path.Combine(
                    templateDirectory,
                    "resolver-marker.png");
                File.WriteAllText(
                    templatePath,
                    "runtime-data-root-marker",
                    Encoding.UTF8);
                relativeTemplate =
                    AppPathService.GetDataRelativePath(templatePath);
                string runtimeResolved =
                    VisionPipelineAppToolFactory.ResolveTemplatePath(
                        relativeTemplate);
                string reviewResolved =
                    OpenVisionRecipeDependencyReviewService
                    .ResolveDependencySourcePath(relativeTemplate);
                string expected = Path.GetFullPath(templatePath);
                if (!string.Equals(
                        runtimeResolved,
                        expected,
                        StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(
                        reviewResolved,
                        expected,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Data-root-relative template resolution did not "
                        + "round-trip through runtime and review paths.");
                }

                string configPath = Path.Combine(
                    AppPathService.ConfigRootDirectory,
                    "UI",
                    "runtime-data-root-smoke.txt");
                Directory.CreateDirectory(
                    Path.GetDirectoryName(configPath));
                File.WriteAllText(
                    configPath,
                    "runtime-data-root-config",
                    Encoding.UTF8);
                LogConfig relativeLog = new LogConfig
                {
                    LogDirectory = "Log"
                }.Normalize();
                if (!AppPathService.IsPathUnderInstallationRoot(
                        installationRoot)
                    || !StartsWithRoot(templatePath, dataRoot)
                    || !StartsWithRoot(configPath, dataRoot)
                    || !StartsWithRoot(
                        AppPathService.QualifiedRecipeRootDirectory,
                        dataRoot)
                    || !StartsWithRoot(relativeLog.LogDirectory, dataRoot)
                    || !StartsWithRoot(
                        OpenVisionLanguageService.CatalogPath,
                        dataRoot))
                {
                    throw new InvalidOperationException(
                        "One or more writable runtime paths escaped the "
                        + "selected data root.");
                }

                File.WriteAllText(
                    Path.Combine(outputDirectory, "report.txt"),
                    "Result: PASS" + Environment.NewLine
                    + "Scenario: runtime-data-root-contract" + Environment.NewLine
                    + "InstallationRoot: " + installationRoot + Environment.NewLine
                    + "DataRoot: " + dataRoot + Environment.NewLine
                    + "ConfigRoot: " + AppPathService.ConfigRootDirectory + Environment.NewLine
                    + "RecipeRoot: " + AppPathService.RecipeRootDirectory + Environment.NewLine
                    + "QualifiedRecipeRoot: " + AppPathService.QualifiedRecipeRootDirectory + Environment.NewLine
                    + "LogRoot: " + relativeLog.LogDirectory + Environment.NewLine
                    + "RelativeTemplate: " + relativeTemplate + Environment.NewLine
                    + "RuntimeResolvedTemplate: " + runtimeResolved + Environment.NewLine
                    + "ReviewResolvedTemplate: " + reviewResolved + Environment.NewLine
                    + "PreviewRunCount: 0" + Environment.NewLine
                    + "LayerCount: 0" + Environment.NewLine
                    + "RoutesChanged: False",
                    Encoding.UTF8);
            }
            finally
            {
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }

        private static bool StartsWithRoot(string path, string root)
        {
            string fullRoot = Path.GetFullPath(root)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(path);
            return fullPath.StartsWith(
                    fullRoot,
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    fullPath.TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar),
                    fullRoot.TrimEnd(Path.DirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveOutputDirectory(string[] args)
        {
            for (int i = 2; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], "--output", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[i + 1]);
                }
            }

            if (args.Length > 2 && !args[2].StartsWith("--", StringComparison.Ordinal))
            {
                return Path.GetFullPath(args[2]);
            }

            return Path.Combine(FindRepositoryRoot(), ".codex", "smoke-output", "actual-exe-line-pins-measure");
        }

        private static string ResolveRequiredOption(string[] args, string optionName)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[i + 1]);
                }
            }

            throw new ArgumentException("Missing required smoke option: " + optionName);
        }

        private static string ResolveOptionalOption(string[] args, string optionName)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[i + 1]);
                }
            }

            return string.Empty;
        }

        private static string ResolveOptionalTextOption(string[] args, string optionName)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1]?.Trim() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static int ResolveOptionalIntOption(string[] args, string optionName, int fallback)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                {
                    return value;
                }
            }

            return fallback;
        }

        private static bool ResolveOptionalBoolOption(string[] args, string optionName, bool fallback)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase)
                    && bool.TryParse(args[i + 1], out bool value))
                {
                    return value;
                }
            }

            return fallback;
        }

        private static string FindRepositoryRoot()
        {
            string[] starts =
            {
                Environment.CurrentDirectory,
                AppContext.BaseDirectory
            };

            foreach (string start in starts)
            {
                DirectoryInfo current = new DirectoryInfo(start);
                while (current != null)
                {
                    if (IsRepositoryRootCandidate(current.FullName))
                    {
                        return current.FullName;
                    }

                    current = current.Parent;
                }
            }

            throw new DirectoryNotFoundException("Could not locate the OpenVisionLab repository root.");
        }

        private static bool IsRepositoryRootCandidate(string path)
        {
            return File.Exists(Path.Combine(path, "OpenVisionLab.sln"))
                && Directory.Exists(Path.Combine(path, "docs", "samples"));
        }

        private static void SaveWindowScreenshot(Window window, string path)
        {
            BringWindowToFront(window);
            window.UpdateLayout();
            Pump(4);
            double scaleX = 1D;
            double scaleY = 1D;
            PresentationSource source = PresentationSource.FromVisual(window);
            if (source?.CompositionTarget != null)
            {
                Matrix transform = source.CompositionTarget.TransformToDevice;
                scaleX = transform.M11;
                scaleY = transform.M22;
            }

            int pixelWidth = Math.Max(1, (int)Math.Ceiling(window.ActualWidth * scaleX));
            int pixelHeight = Math.Max(1, (int)Math.Ceiling(window.ActualHeight * scaleY));
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                pixelWidth,
                pixelHeight,
                96D * scaleX,
                96D * scaleY,
                PixelFormats.Pbgra32);
            renderTarget.Render(window);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (FileStream stream = File.Create(path))
            {
                encoder.Save(stream);
            }
        }

        private static void SaveWindowScreenScreenshot(Window window, string path)
        {
            BringWindowToFront(window);
            window.UpdateLayout();
            Pump(24);
            System.Windows.Point topLeft = window.PointToScreen(new System.Windows.Point(0D, 0D));
            System.Windows.Point bottomRight = window.PointToScreen(new System.Windows.Point(window.ActualWidth, window.ActualHeight));
            int pixelWidth = Math.Max(1, (int)Math.Ceiling(bottomRight.X - topLeft.X));
            int pixelHeight = Math.Max(1, (int)Math.Ceiling(bottomRight.Y - topLeft.Y));
            using (Bitmap bitmap = new Bitmap(pixelWidth, pixelHeight))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    (int)Math.Floor(topLeft.X),
                    (int)Math.Floor(topLeft.Y),
                    0,
                    0,
                    new System.Drawing.Size(pixelWidth, pixelHeight));
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void BringWindowToFront(Window window)
        {
            if (window == null)
            {
                return;
            }

            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }

            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            if (handle != IntPtr.Zero)
            {
                const int showRestore = 9;
                ShowWindow(handle, showRestore);
                SetForegroundWindow(handle);
            }

            window.Topmost = true;
            window.Activate();
            window.Focus();
            Pump(12);
        }

        private static void MoveCursorInsideWindow(Window window, double x, double y)
        {
            if (window == null)
            {
                return;
            }

            System.Windows.Point screenPoint = window.PointToScreen(new System.Windows.Point(x, y));
            SetCursorPosOrThrow((int)Math.Round(screenPoint.X), (int)Math.Round(screenPoint.Y));
        }

        private static void SaveTutorialScreenshot(
            Window window,
            string outputDirectory,
            string fileName,
            StringBuilder report,
            string reportPath)
        {
            MoveCursorInsideWindow(window, 24D, 24D);
            Pump(8);
            string path = Path.Combine(outputDirectory, fileName);
            SaveWindowScreenshot(window, path);
            report.AppendLine("Screenshot: " + fileName);
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
        }

        private static void ValidateTutorialCaptureFiles(
            string outputDirectory,
            StringBuilder report,
            string reportPath)
        {
            string[] requiredFiles =
            {
                "01_main_workspace_current.png",
                "02_pipeline_review_current.png",
                "03_layer_docking_current.png",
                "04_matching_tool_current.png",
                "matching_preview_actual_current.png",
                "05_blob_tool_current.png",
                "06_line_tool_current.png",
                "07_sample_catalog_public_current.png",
                "08_run_log_collapsed_current.png",
                "09_run_log_open_current.png"
            };

            string missing = string.Join(
                ", ",
                requiredFiles.Where(fileName => !File.Exists(Path.Combine(outputDirectory, fileName))));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                report.AppendLine("Missing: " + missing);
                File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
                throw new InvalidOperationException("Tutorial capture did not generate required files: " + missing);
            }

            report.AppendLine("Verified: required tutorial capture files");
            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
        }

        private static void EnsureFileExists(string path, string description)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(description + " was not found.", path);
            }
        }

        private static void WaitForTaskWithPump(Task task, string description)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!task.IsCompleted)
            {
                Pump(4);
                Thread.Sleep(20);
                if (stopwatch.Elapsed > TimeSpan.FromSeconds(20))
                {
                    throw new TimeoutException(description + " did not complete within 20 seconds.");
                }
            }

            task.GetAwaiter().GetResult();
        }

        private static string PlaceWindowOnLeftmostMonitor(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            List<SmokeMonitorInfo> monitors = new List<SmokeMonitorInfo>();
            MonitorEnumCallback callback = (IntPtr monitor, IntPtr _, ref SmokeNativeRect __, IntPtr ___) =>
            {
                SmokeMonitorInfo info = SmokeMonitorInfo.Create();
                if (GetMonitorInfo(monitor, ref info))
                {
                    monitors.Add(info);
                }

                return true;
            };

            if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero) || monitors.Count == 0)
            {
                throw new InvalidOperationException("No display monitor was available for the EXE capture.");
            }

            SmokeMonitorInfo selected = monitors
                .OrderBy(info => info.Monitor.Left)
                .ThenBy(info => info.Monitor.Top)
                .First();
            IntPtr handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero || !GetWindowRect(handle, out SmokeNativeRect initialWindow))
            {
                throw new InvalidOperationException("The EXE window rectangle was unavailable before monitor placement.");
            }

            int width = initialWindow.Right - initialWindow.Left;
            int height = initialWindow.Bottom - initialWindow.Top;
            int left = selected.WorkArea.Left + Math.Max(0, (selected.WorkArea.Right - selected.WorkArea.Left - width) / 2);
            int top = selected.WorkArea.Top + Math.Max(0, (selected.WorkArea.Bottom - selected.WorkArea.Top - height) / 2);
            const uint noSize = 0x0001;
            const uint noZOrder = 0x0004;
            if (!SetWindowPos(handle, IntPtr.Zero, left, top, 0, 0, noSize | noZOrder)
                || !GetWindowRect(handle, out SmokeNativeRect actualWindow))
            {
                throw new InvalidOperationException("The EXE window could not be placed on the leftmost monitor.");
            }

            bool intersects = actualWindow.Left < selected.Monitor.Right
                && actualWindow.Right > selected.Monitor.Left
                && actualWindow.Top < selected.Monitor.Bottom
                && actualWindow.Bottom > selected.Monitor.Top;
            if (!intersects)
            {
                throw new InvalidOperationException(
                    "The EXE window did not intersect the selected leftmost monitor. "
                    + $"Window={actualWindow}; Monitor={selected.Monitor}");
            }

            return "CaptureMonitor: " + selected.DeviceName
                + "; Bounds=" + selected.Monitor
                + "; WorkArea=" + selected.WorkArea
                + "; Window=" + actualWindow
                + "; Intersects=true";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SmokeNativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public override readonly string ToString()
            {
                return $"{Left},{Top},{Right - Left},{Bottom - Top}";
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SmokeMonitorInfo
        {
            public int Size;
            public SmokeNativeRect Monitor;
            public SmokeNativeRect WorkArea;
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            public static SmokeMonitorInfo Create()
            {
                return new SmokeMonitorInfo
                {
                    Size = Marshal.SizeOf<SmokeMonitorInfo>(),
                    DeviceName = string.Empty
                };
            }
        }

        private static string GetClipboardTextWithRetry()
        {
            return RunClipboardActionWithRetry(() => System.Windows.Clipboard.GetText());
        }

        private static void SetClipboardTextWithRetry(string text)
        {
            RunClipboardActionWithRetry(() =>
            {
                System.Windows.Clipboard.SetText(text ?? string.Empty);
                return true;
            });
        }

        private static T RunClipboardActionWithRetry<T>(Func<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            COMException lastException = null;
            for (int attempt = 0; attempt < 40; attempt++)
            {
                try
                {
                    return action();
                }
                catch (COMException ex) when ((uint)ex.ErrorCode == 0x800401D0)
                {
                    lastException = ex;
                    Pump(4);
                    Thread.Sleep(Math.Min(250, 50 + attempt * 10));
                }
            }

            throw lastException ?? new COMException("Clipboard operation failed.");
        }

        private static void CopyCurrentDockingStateFile(string fileName, string outputDirectory, string outputFileName)
        {
            string path = Path.Combine(AppPathService.EnsureDirectory("CONFIG", "UI"), fileName);
            if (File.Exists(path))
            {
                File.Copy(path, Path.Combine(outputDirectory, outputFileName), true);
            }
        }

        private static void ClearCurrentDockingStateFiles()
        {
            string uiConfigDirectory = AppPathService.EnsureDirectory("CONFIG", "UI");
            string[] paths =
            {
                Path.Combine(uiConfigDirectory, "LayerDocking.layers"),
                Path.Combine(uiConfigDirectory, "LayerDocking.layout")
            };

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static void WithDockingStateFileBackup(Action action)
        {
            string uiConfigDirectory = AppPathService.EnsureDirectory("CONFIG", "UI");
            string[] paths =
            {
                Path.Combine(uiConfigDirectory, "LayerDocking.layers"),
                Path.Combine(uiConfigDirectory, "LayerDocking.layout")
            };
            Dictionary<string, byte[]> backups = paths
                .Where(File.Exists)
                .ToDictionary(path => path, File.ReadAllBytes, StringComparer.OrdinalIgnoreCase);

            try
            {
                action();
            }
            finally
            {
                foreach (string path in paths)
                {
                    try
                    {
                        if (backups.TryGetValue(path, out byte[] bytes) && bytes != null)
                        {
                            File.WriteAllBytes(path, bytes);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void Pump(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(_ =>
                    {
                        frame.Continue = false;
                        return null;
                    }),
                    null);
                Dispatcher.PushFrame(frame);
                Thread.Sleep(10);
            }
        }
    }
}
