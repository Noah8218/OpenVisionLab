using Lib.OpenCV.Pipeline;
using OpenVisionLab._1._Core;
using OpenVisionLab.Docking.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls.WpfPropertyGrid;
using System.Xml.Serialization;
using static OpenVisionLab.DEFINE;

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

                if (string.Equals(scenario, "tool-dock-float-cycle", StringComparison.OrdinalIgnoreCase))
                {
                    RunToolDockFloatCycle(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "workspace-startup-empty", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "startup-empty-workspace", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "shell-startup-empty-workspace", StringComparison.OrdinalIgnoreCase))
                {
                    RunWorkspaceStartupEmpty(outputDirectory);
                    return true;
                }

                if (string.Equals(scenario, "recipe-manager-tabs", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(scenario, "recipe-manager-direct", StringComparison.OrdinalIgnoreCase))
                {
                    RunRecipeManagerTabs(outputDirectory);
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

        private static void RunLinePinsMeasure(string outputDirectory)
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
                    Width = 1600,
                    Height = 900,
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
                Pump(60);

                ConfigurePinsVerticalDistanceLine(shellHost, "Line A", "X_LTOR");
                ConfigurePinsVerticalDistanceLine(shellHost, "Line B", "X_RTOL");
                shellHost.SetActiveLineSettingForTest("Line A");
                shellHost.SetActiveLinePurposeForTest("Measure");
                Pump(16);

                if (shellHost.ActiveLineInputRoiOverlayCount < 2)
                {
                    throw new InvalidOperationException(
                        "Line tool did not publish both Line A/B ROI overlays. Count="
                        + shellHost.ActiveLineInputRoiOverlayCount.ToString(CultureInfo.InvariantCulture));
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(48);

                string review = shellHost.ActiveNativeResultReviewText;
                string status = shellHost.ActiveNativeStatusText;
                if (!shellHost.HasNativePreviewResult
                    || review.IndexOf("Measure /", StringComparison.OrdinalIgnoreCase) < 0
                    || review.IndexOf("Distance none", StringComparison.OrdinalIgnoreCase) >= 0
                    || review.IndexOf("Count 0", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new InvalidOperationException(
                        "Line pins measure preview did not complete successfully."
                        + Environment.NewLine
                        + "Status: " + status
                        + Environment.NewLine
                        + "Review: " + review);
                }

                double measuredMm = ExtractMm(review);
                if (measuredMm < 0.30D || measuredMm > 0.45D)
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

                string report = BuildPassReport(review, status, measuredMm, step);
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

        private static void RunRecipeManagerTabs(string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string recipeName = "Smoke_RecipeManager_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            const string pipelineName = "Direct_RecipeManager_Check";
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
                recipeManagerButton.IsChecked = true;
                Pump(60);

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
                    || !shellHost.RecipeCommands.OperatorDecisionNextActionText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke operator decision board was incomplete. "
                        + $"Xml='{shellHost.RecipeCommands.OperatorDecisionXmlCardText}', "
                        + $"Sample='{shellHost.RecipeCommands.OperatorDecisionSampleCardText}', "
                        + $"Pair='{shellHost.RecipeCommands.OperatorDecisionPairCardText}', "
                        + $"Next='{shellHost.RecipeCommands.OperatorDecisionNextActionText}'");
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
                    });
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
                    });
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
                    });
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
                    || shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsRegression))
                {
                    throw new InvalidOperationException(
                        "Recipe manager direct smoke benchmark baseline selection did not change the diff result to Still NG. "
                        + $"Summary='{shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText}'");
                }

                OpenVisionRecipeBatchRunOption defaultBaselineOption =
                    shellHost.RecipeCommands.BenchmarkBaselineRunOptions.FirstOrDefault(option =>
                        string.Equals(option.SummaryPath, baselineSummaryPath, StringComparison.OrdinalIgnoreCase));
                shellHost.RecipeCommands.SelectedBenchmarkBaselineRunOption = defaultBaselineOption;
                Pump(60);
                if (!shellHost.RecipeCommands.RecentBatchRunComparisonRows.Any(row => row != null && row.IsRegression))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke could not restore the default regression baseline.");
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
                    "HostRecipeManagerLibraryPane",
                    "HostRecipeManagerList",
                    "HostRecipeDetailText",
                    "HostRecipeGuidedSetupStrip",
                    "HostRecipeGuidedNextActionButton",
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
                    "HostRecipePipelineReportTab",
                    "HostRecipeOperatorDecisionBoard",
                    "HostRecipeOperatorDecisionXmlCard",
                    "HostRecipeOperatorDecisionSampleCard",
                    "HostRecipeOperatorDecisionPairCard",
                    "HostRecipeOperatorDecisionNextAction",
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
                    "HostRecipeManagerCommandStrip",
                    "HostRecipeEditValidation",
                    "HostRecipeCreateNamedButton",
                    "HostRecipeImportXmlButton",
                    "HostRecipeExportXmlButton");
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
                    "HostRecipeOperatorValidationChecklistPanel",
                    "HostRecipeOperatorValidationChecklist",
                    "HostRecipeOperatorResultChannelsPanel",
                    "HostRecipeOperatorResultChannelBoard",
                    "HostRecipeOperatorResultChannels",
                    "HostRecipeOperatorHandoffReport");
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
                string operatorReportClipboard = System.Windows.Clipboard.GetText();
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
                    "HostRecipeRecentBatchRunList",
                    "HostRecipeRecentBatchRunSampleList",
                    "HostRecipeRecentBatchRunComparisonPanel",
                    "HostRecipeBenchmarkBaselineRunSelector",
                    "HostRecipeBenchmarkBaselineRunCombo",
                    "HostRecipeRecentBatchRunComparisonSummary",
                    "HostRecipeRecentBatchRunComparisonList",
                    "HostRecipeSelectedRunComparisonReview",
                    "HostRecipeCopySelectedRunReviewButton",
                    "HostRecipeSelectedRunReview");
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
                string runReviewClipboard = System.Windows.Clipboard.GetText();
                if (!runReviewClipboard.Contains(forcedFailedRole.SampleName, StringComparison.OrdinalIgnoreCase)
                    || !runReviewClipboard.Contains(failedStepPreview.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke selected run review copy did not write the expected clipboard content.");
                }
                SaveWindowScreenshot(window, Path.Combine(outputDirectory, "OpenVisionLab_RecipeManager_RunHistory.png"));

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
                string llmPromptClipboard = System.Windows.Clipboard.GetText();
                if (!llmPromptClipboard.Contains("OpenVisionLab VisionPipeline XML draft", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("Template Matching", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("0..1 decimals", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("FIND_ANGLE_MIN", StringComparison.OrdinalIgnoreCase)
                    || !llmPromptClipboard.Contains("existing template/image dependency paths", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Recipe manager direct smoke LLM prompt copy did not write the expected clipboard content.");
                }

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

                System.Windows.Clipboard.SetText(File.ReadAllText(llmDraftPath));
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
                string llmReviewBundleClipboard = System.Windows.Clipboard.GetText();
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
                string correctionBundleClipboard = System.Windows.Clipboard.GetText();
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
                    + "SampleCheck: " + shellHost.RecipeCommands.LatestSampleRunSummary.StatusText + Environment.NewLine
                    + "PairCheck: " + pairCheckStatusText + Environment.NewLine
                    + "PairRoleCards: " + pairRoleCardCount.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "RoleDrilldown: " + forcedFailedRole.Role + " -> " + failedStepPreview.Name + Environment.NewLine
                    + "FailedRunLink: " + selectedPreviewStep.Index.ToString(CultureInfo.InvariantCulture) + " | " + selectedPreviewStep.Name + Environment.NewLine
                    + "FailedRunStepVisible: True" + Environment.NewLine
                    + "BenchmarkBaselineSelection: selectable baseline changed diff to Still NG and restored Regression" + Environment.NewLine
                    + "SampleToInputLoad: explicit load without Preview/Run | " + selectedPreviewStep.InputLayer + " | " + sampleImagePath + Environment.NewLine
                    + "FailedStepRerunComparison: visible" + Environment.NewLine
                    + "CorrectedOutputReview: visible after XML apply" + Environment.NewLine
                    + "SelectedRunReview: linked failed step" + Environment.NewLine
                    + "SelectedRunReviewCopy: copied" + Environment.NewLine
                    + "PipelineFilter: " + pipelineFilterText + " -> " + shellHost.RecipeCommands.FilteredPipelineOptions.Count.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                    + "StepParameters: " + selectedPreviewStep.ParameterPreviewText + Environment.NewLine
                    + "StepRoiTemplate: " + selectedPreviewStep.RoiMetadataText + " | " + selectedPreviewStep.TemplateMetadataText + Environment.NewLine
                    + "StepToolEntry: " + shellHost.RecipeCommands.OpenSelectedStepToolText + Environment.NewLine
                    + "StepPropertyGridApply: explicit XML apply without Preview/Run" + Environment.NewLine
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
                        + "Screenshots: OpenVisionLab_StartupEmptyWorkspace.png" + Environment.NewLine
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
                    if (File.Exists(Path.Combine(current.FullName, "Sample", "EasyGauge", "Pins.bmp")))
                    {
                        return current.FullName;
                    }

                    current = current.Parent;
                }
            }

            throw new DirectoryNotFoundException("Could not locate the OpenVisionLab repository root.");
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
