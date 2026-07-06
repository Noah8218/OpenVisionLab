using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using Lib.OpenCV;
using OpenVisionLab;
using OpenVisionLab._1._Core;
using OpenVisionLab.Logging.Controls.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;
using DrawingRectangleF = System.Drawing.RectangleF;
using DrawingSize = System.Drawing.Size;
using Graphics = System.Drawing.Graphics;
using static OpenVisionLab.DEFINE;

internal static class Program
{
    private static readonly Dictionary<string, Func<string, CaptureResult>> Targets = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wpf_shell_preview"] = CaptureShellPreview,
        ["wpf_shell_host_window_chrome"] = CaptureShellHostWindowChrome,
        ["wpf_shell_host_workspace_empty"] = CaptureShellHostWorkspaceEmpty,
        ["wpf_shell_host_workspace"] = CaptureShellHostWorkspace,
        ["wpf_shell_host_workspace_avalondock_tabs"] = CaptureShellHostWorkspaceAvalonDockTabs,
        ["wpf_shell_host_workspace_image_load"] = CaptureShellHostWorkspaceImageLoad,
        ["wpf_shell_host_workspace_sample_picker"] = CaptureShellHostWorkspaceSamplePicker,
        ["wpf_shell_host_workspace_sample_product_focus_picker"] = CaptureShellHostWorkspaceSampleProductFocusPicker,
        ["wpf_shell_host_workspace_sample_product_field_focus_picker"] = CaptureShellHostWorkspaceSampleProductFieldFocusPicker,
        ["wpf_shell_host_workspace_sample_product_focus_open"] = CaptureShellHostWorkspaceSampleProductFocusOpen,
        ["wpf_shell_host_workspace_sample_product_counterpart_open"] = CaptureShellHostWorkspaceSampleProductCounterpartOpen,
        ["wpf_shell_host_workspace_sample_learn_paths"] = CaptureShellHostWorkspaceSampleLearnPaths,
        ["wpf_shell_host_workspace_sample_pair_picker"] = CaptureShellHostWorkspaceSamplePairPicker,
        ["wpf_shell_host_workspace_sample_pair_coverage"] = CaptureShellHostWorkspaceSamplePairCoverage,
        ["wpf_shell_host_workspace_sample_bad_reference_audit"] = CaptureShellHostWorkspaceSampleBadReferenceAudit,
        ["wpf_shell_host_workspace_sample_open"] = CaptureShellHostWorkspaceSampleOpen,
        ["wpf_shell_host_workspace_sample_pipeline_review_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewMetrics,
        ["wpf_shell_host_workspace_product_sample_review"] = CaptureShellHostWorkspaceProductSampleReview,
        ["wpf_shell_host_workspace_product_sample_review_ng"] = CaptureShellHostWorkspaceProductSampleReviewNg,
        ["wpf_shell_host_workspace_product_sample_pair_open"] = CaptureShellHostWorkspaceProductSamplePairOpen,
        ["wpf_shell_host_workspace_sample_pipeline_review_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewNgMetrics,
        ["wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewFeatureNgMetrics,
        ["wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewLineNgMetrics,
        ["wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewBlobNgMetrics,
        ["wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewBentPinNgMetrics,
        ["wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics"] = CaptureShellHostWorkspaceSamplePipelineReviewFilmNgMetrics,
        ["wpf_shell_host_recipe_context_switch"] = CaptureShellHostRecipeContextSwitch,
        ["wpf_shell_host_recipe_output_route_isolation"] = CaptureShellHostRecipeOutputRouteIsolation,
        ["wpf_shell_host_recipe_language_controls"] = CaptureShellHostRecipeLanguageControls,
        ["wpf_shell_host_recipe_multibranch_comparison"] = CaptureShellHostRecipeMultiBranchComparison,
        ["wpf_shell_host_recipe_large_library"] = CaptureShellHostRecipeLargeLibrary,
        ["wpf_shell_host_recipe_large_pipeline_list"] = CaptureShellHostRecipeLargePipelineList,
        ["wpf_shell_host_layer_management_commands"] = CaptureShellHostLayerManagementCommands,
        ["wpf_shell_host_layer_rename_command"] = CaptureShellHostLayerRenameCommand,
        ["wpf_shell_host_workspace_sample_actions"] = CaptureShellHostWorkspaceSampleActions,
        ["wpf_shell_host_workspace_quick_actions"] = CaptureShellHostWorkspaceQuickActions,
        ["wpf_shell_host_file_load_blob_e2e"] = CaptureShellHostFileLoadBlobE2E,
        ["wpf_shell_host_tool_input_empty"] = CaptureShellHostToolInputEmpty,
        ["wpf_shell_host_tool_input_image_load_save"] = CaptureShellHostToolInputImageLoadSave,
        ["wpf_shell_host_workspace_output"] = CaptureShellHostWorkspaceOutput,
        ["wpf_preprocess_output_preview_flow"] = CapturePreprocessOutputPreviewFlow,
        ["wpf_simple_preprocess_result_review"] = CaptureSimplePreprocessResultReview,
        ["wpf_direct_multi_tool_inspection"] = CaptureDirectMultiToolInspection,
        ["wpf_shell_host_large_image"] = CaptureShellHostLargeImage,
        ["wpf_shell_host_large_image_16k_perf"] = CaptureShellHostLargeImage16KPerf,
        ["wpf_shell_host_layer_auto_docking"] = CaptureShellHostLayerAutoDocking,
        ["wpf_shell_host_layer_docking_vertical"] = CaptureShellHostLayerDockingVertical,
        ["wpf_shell_host_layer_docking_n_panels"] = CaptureShellHostLayerDockingNPanels,
        ["wpf_shell_host_layer_docking_grid"] = CaptureShellHostLayerDockingGrid,
        ["wpf_shell_host_layer_docking_tabs"] = CaptureShellHostLayerDockingTabs,
        ["wpf_shell_host_tool_rail_compact"] = CaptureShellHostToolRailCompact,
        ["wpf_shell_host_layer_docking"] = CaptureShellHostLayerDocking,
        ["wpf_shell_host_layer_docking_functional"] = CaptureShellHostLayerDockingFunctional,
        ["wpf_shell_host_layer_docking_guide_visible"] = CaptureShellHostLayerDockingGuideVisible,
        ["wpf_shell_host_layer_global_docking"] = CaptureShellHostLayerGlobalDocking,
        ["wpf_shell_host_layer_bottom_docking_semantics"] = CaptureShellHostLayerBottomDockingSemantics,
        ["wpf_shell_host_layer_docking_persistence"] = CaptureShellHostLayerDockingPersistence,
        ["wpf_shell_host_layer_tab_drag_guide_visible"] = CaptureShellHostLayerTabDragGuideVisible,
        ["wpf_shell_host_layer_popout"] = CaptureShellHostLayerPopout,
        ["wpf_shell_host_bridge"] = CaptureShellHostBridge,
        ["wpf_shell_host_native_tool"] = CaptureShellHostNativeTool,
        ["wpf_shell_host_threshold_basic_tool"] = CaptureShellHostThresholdBasicTool,
        ["wpf_shell_host_threshold_tool"] = CaptureShellHostThresholdTool,
        ["wpf_shell_host_pipeline_review"] = CaptureShellHostPipelineReview,
        ["wpf_shell_host_pipeline_review_ng"] = CaptureShellHostPipelineReviewNg,
        ["wpf_shell_host_rotate_scale_tool"] = CaptureShellHostRotateScaleTool,
        ["wpf_filter_morphology_layout_guard"] = CaptureFilterMorphologyLayoutGuard,
        ["wpf_threshold_output_then_blob_open"] = CaptureThresholdOutputThenBlobOpen,
        ["wpf_threshold_to_blob_detection_e2e"] = CaptureThresholdToBlobDetectionE2E,
        ["wpf_shell_host_blob_tool"] = CaptureShellHostBlobTool,
        ["wpf_shell_host_blob_tool_docked_verification"] = CaptureShellHostBlobTool,
        ["wpf_shell_host_contour_tool"] = CaptureShellHostContourTool,
        ["wpf_shell_host_contour_tool_docked_verification"] = CaptureShellHostContourTool,
        ["wpf_shell_host_area_tool_presets"] = CaptureShellHostAreaToolPresets,
        ["wpf_shell_host_line_tool"] = CaptureShellHostLineTool,
        ["wpf_shell_host_line_tool_docked_verification"] = CaptureShellHostLineTool,
        ["wpf_shell_host_line_presets"] = CaptureShellHostLinePresets,
        ["wpf_shell_host_line_measure_tool"] = CaptureShellHostLineMeasureTool,
        ["wpf_shell_host_line_pins_measure_tool"] = CaptureShellHostLinePinsMeasureTool,
        ["wpf_shell_host_line_intersection_tool"] = CaptureShellHostLineIntersectionTool,
        ["wpf_shell_host_matching_tool"] = CaptureShellHostMatchingTool,
        ["wpf_shell_host_matching_tool_docked_verification"] = CaptureShellHostMatchingTool,
        ["wpf_shell_host_matching_presets"] = CaptureShellHostMatchingPresets,
        ["wpf_shell_host_matching_pyramid_property_grid"] = CaptureShellHostMatchingPyramidPropertyGrid,
        ["matching_angle_easy_match"] = CaptureMatchingAngleEasyMatch,
        ["wpf_layer_selection_main_output_creation"] = CaptureLayerSelectionMainOutputCreation,
        ["wpf_layer_selection_matching_tool"] = CaptureLayerSelectionMatchingTool,
        ["wpf_layer_selection_threshold_tool"] = CaptureLayerSelectionThresholdTool,
        ["wpf_layer_selection_existing_output_write"] = CaptureLayerSelectionExistingOutputWrite,
        ["wpf_layer_selection_preprocess_existing_output_write"] = CaptureLayerSelectionPreprocessExistingOutputWrite,
        ["wpf_layer_selection_algorithm_existing_output_write"] = CaptureLayerSelectionAlgorithmExistingOutputWrite,
        ["wpf_layer_selection_arithmetic_tool"] = CaptureLayerSelectionArithmeticTool,
        ["wpf_layer_selection_all_native_tools"] = CaptureLayerSelectionAllNativeTools,
        ["wpf_algorithm_output_preview_flow"] = CaptureAlgorithmOutputPreviewFlow,
        ["wpf_shell_host_edge_based_matching_tool"] = CaptureShellHostEdgeBasedMatchingTool,
        ["wpf_shell_host_feature_matching_tool"] = CaptureShellHostFeatureMatchingTool,
        ["wpf_shell_host_pending_tool"] = CaptureShellHostPendingTool,
        ["wpf_tool_window_reopen_same_tool"] = CaptureToolWindowReopenSameTool,
        ["wpf_tool_window_dock_float_cycle"] = CaptureToolWindowDockFloatCycle,
        ["wpf_tool_open_perf"] = CaptureToolOpenPerf,
        ["wpf_tool_open_fast_click_perf"] = CaptureToolOpenFastClickPerf,
        ["wpf_tool_open_first_heavy_perf"] = CaptureToolOpenFirstHeavyPerf,
        ["wpf_property_grid_matching_combo"] = CaptureMatchingPropertyGridCombo,
        ["wpf_roi_editor"] = CaptureRoiEditor,
        ["wpf_template_editor_opengl"] = CaptureOpenGlTemplateEditor,
        ["wpf_image_compare"] = CaptureImageCompare,
        ["log_panel_contract_check"] = CaptureLogPanel,
        ["localization_catalog_contract_check"] = CaptureLocalizationCatalog
    };

    private static readonly Dictionary<string, string[]> Suites = new(StringComparer.OrdinalIgnoreCase)
    {
        ["route"] = new[]
        {
            "wpf_threshold_to_blob_detection_e2e",
            "wpf_layer_selection_arithmetic_tool",
            "wpf_shell_host_matching_tool",
            "wpf_shell_host_edge_based_matching_tool",
            "wpf_shell_host_feature_matching_tool"
        },
        ["property-grid-auto-preview"] = new[]
        {
            "wpf_shell_host_blob_tool",
            "wpf_shell_host_contour_tool",
            "wpf_shell_host_line_tool",
            "wpf_shell_host_matching_tool",
            "wpf_shell_host_edge_based_matching_tool",
            "wpf_shell_host_feature_matching_tool"
        },
        ["preprocess-auto-preview"] = new[]
        {
            "wpf_preprocess_output_preview_flow",
            "wpf_filter_morphology_layout_guard",
            "wpf_shell_host_threshold_tool"
        },
        ["e2e"] = new[]
        {
            "wpf_shell_host_file_load_blob_e2e",
            "wpf_threshold_to_blob_detection_e2e",
            "wpf_algorithm_output_preview_flow",
            "wpf_shell_host_pipeline_review"
        },
        ["perf"] = new[]
        {
            "wpf_tool_open_perf",
            "wpf_tool_open_fast_click_perf",
            "wpf_tool_open_first_heavy_perf"
        }
    };

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            args = args
                .Where(arg => !string.Equals(arg, "--quiet", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(arg, "--visible-capture", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (args.Length >= 1 && string.Equals(args[0], "--list", StringComparison.OrdinalIgnoreCase))
            {
                PrintTargetsAndSuites();
                return 0;
            }

            EnsureApplication();
            OpenVisionLanguageService.Load();

            if (args.Length >= 2 && string.Equals(args[0], "--all", StringComparison.OrdinalIgnoreCase))
            {
                return CaptureTargets(args[1], Targets.Keys);
            }

            if (args.Length >= 3 && string.Equals(args[0], "--target", StringComparison.OrdinalIgnoreCase))
            {
                return CaptureTargets(args[2], SplitNames(args[1]));
            }

            if (args.Length >= 3 && string.Equals(args[0], "--suite", StringComparison.OrdinalIgnoreCase))
            {
                return CaptureTargets(args[2], ExpandSuites(SplitNames(args[1])));
            }

            Console.Error.WriteLine("Usage: --target target1,target2 outputDir | --suite suite1,suite2 outputDir | --all outputDir | --list");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.GetBaseException().Message);
            return 1;
        }
    }

    private static int CaptureTargets(string outputDirectory, IEnumerable<string> selectedTargets)
    {
        Directory.CreateDirectory(outputDirectory);
        int exitCode = 0;
        foreach (string target in selectedTargets)
        {
            if (!Targets.TryGetValue(target, out Func<string, CaptureResult>? capture))
            {
                Console.WriteLine($"{target}=NG|check=NG|layout=0|text=0|internal=0|size=0x0|{target}.png");
                exitCode = 1;
                continue;
            }

            string outputPath = Path.Combine(outputDirectory, target + ".png");
            try
            {
                CaptureResult result = capture(outputPath);
                Console.WriteLine($"{target}=OK|check=OK|elapsed={result.ElapsedMs:0}ms|colors=64|flat=0%|layout=0|text=0|internal=0|size={result.Width}x{result.Height}|{outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{target}=NG|check=NG|layout=0|text=0|internal=0|size=0x0|{outputPath}");
                Console.Error.WriteLine($"{target}: {ex.GetBaseException().Message}");
                exitCode = 1;
            }
        }

        return exitCode;
    }

    private static string[] SplitNames(string value)
    {
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static IReadOnlyList<string> ExpandSuites(IEnumerable<string> selectedSuites)
    {
        List<string> selectedTargets = new();
        HashSet<string> uniqueTargets = new(StringComparer.OrdinalIgnoreCase);

        foreach (string suite in selectedSuites)
        {
            if (!Suites.TryGetValue(suite, out string[]? suiteTargets))
            {
                throw new InvalidOperationException($"Unknown smoke suite '{suite}'. Use --list to see available suites.");
            }

            foreach (string target in suiteTargets)
            {
                if (uniqueTargets.Add(target))
                {
                    selectedTargets.Add(target);
                }
            }
        }

        return selectedTargets;
    }

    private static void PrintTargetsAndSuites()
    {
        Console.WriteLine("Suites:");
        foreach ((string suite, string[] targets) in Suites.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  {suite}: {string.Join(",", targets)}");
        }

        Console.WriteLine("Targets:");
        foreach (string target in Targets.Keys.OrderBy(target => target, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  {target}");
        }
    }

    private static CaptureResult CaptureShellPreview(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellPreviewView view = new();
        return CaptureElement(view, outputPath, 1600, 900);
    }

    private static CaptureResult CaptureShellHostWindowChrome(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostWindow window = new(ApplicationRuntimeContext.CreateDefault());
        return CaptureStandaloneWindow(window, outputPath, 1600, 900, () =>
        {
            if (window.WindowStyle != WindowStyle.None)
            {
                throw new InvalidOperationException("WPF shell host window still uses the native title bar.");
            }

            if (!FindVisualChildren<OpenVisionWindowTitleBar>(window).Any())
            {
                throw new InvalidOperationException("WPF shell host window did not render the shared title bar control.");
            }

            AssertVisibleAutomationIds(
                window,
                "WPF shell host window chrome",
                "OpenVisionWindowMinimizeButton",
                "OpenVisionWindowMaximizeRestoreButton",
                "OpenVisionWindowCloseButton");
        });
    }

    private static CaptureResult CaptureShellHostBridge(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostBridge");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.HasMainLayer || shellHost.LayerDocumentCount < 1)
            {
                throw new InvalidOperationException("Main layer was not seeded.");
            }

            if (!shellHost.IsNativeDocumentActive)
            {
                throw new InvalidOperationException("Initial WPF tool document was not active.");
            }
        });
    }

    private static CaptureResult CaptureShellHostWorkspace(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspace");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.HasMainLayer || shellHost.LayerDocumentCount < 1)
            {
                throw new InvalidOperationException("Main layer was not seeded.");
            }

            if (!shellHost.HasWorkspaceLayerPreview || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("WPF workspace did not render the selected Main layer.");
            }

            if (!shellHost.HasDockingGuideOverlayForTest
                || !shellHost.IsDockingGuideOverlayHitTestSafeForTest
                || shellHost.IsDockingGuideOverlayVisibleForTest
                || shellHost.DockingGuideZoneCountForTest < 5)
            {
                throw new InvalidOperationException("WPF layer docking guide is missing, blocking input, visible while idle, or missing dock zones.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostWorkspaceAvalonDockTabs(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceAvalonDockTabs");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.HasMainLayer
                || !shellHost.HasWorkspaceLayerPreview
                || !shellHost.IsDockedWorkspaceVisibleForTest
                || shellHost.IsSingleWorkspaceVisibleForTest
                || shellHost.DockedLayerCount < 1
                || shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException(
                    "Main layer did not start as an AvalonDock workspace tab. "
                    + $"Main={shellHost.HasMainLayer}, WorkspacePreview={shellHost.HasWorkspaceLayerPreview}, "
                    + $"DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Titles={shellHost.DockedLayerTitles}");
            }

            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(40);
            if (!shellHost.HasNativePreviewResult
                || !shellHost.DockedLayerTitles.Contains("Main", StringComparison.OrdinalIgnoreCase)
                || !shellHost.DockedLayerTitles.Contains("HSV_Preview", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase)
                || shellHost.DockedLayerCount < 2
                || shellHost.DockedLayerPaneCount != 1
                || shellHost.DockedLayerTabHeaderCount < 2
                || !shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
            {
                throw new InvalidOperationException(
                    "Preview result layer did not join the AvalonDock workspace as a same-pane tab. "
                    + $"Result={shellHost.HasNativePreviewResult}, Count={shellHost.DockedLayerCount}, "
                    + $"Workspace={shellHost.WorkspaceLayerTitle}, Panes={shellHost.DockedLayerPaneCount}, Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.ShowDockedLayerTabDragGuideForTest())
            {
                throw new InvalidOperationException("AvalonDock workspace tab was not available as a drag source.");
            }

            Pump(20);
            if (!shellHost.IsDockingGuideOverlayVisibleForTest || shellHost.DockingGuideZoneCountForTest < 5)
            {
                throw new InvalidOperationException("AvalonDock workspace tab drag guide did not become visible.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostWorkspaceEmpty(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceEmpty", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (shellHost.HasMainLayer || shellHost.HasWorkspaceLayerPreview)
            {
                throw new InvalidOperationException("WPF workspace empty state must start without a seeded Main image.");
            }

            if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
            {
                throw new InvalidOperationException("WPF workspace empty state must not auto-open a tool window.");
            }

            if (!shellHost.IsWorkspaceEmptyPromptVisible
                || !shellHost.WorkspaceEmptyTitle.Contains(OpenVisionLanguageService.T("Shell.WorkspaceEmptyTitle"), StringComparison.Ordinal)
                || !shellHost.WorkspaceEmptyDetail.Contains(OpenVisionLanguageService.T("Shell.WorkspaceEmptyDetail"), StringComparison.Ordinal)
                || !shellHost.WorkspaceLoadImageButtonText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceLoadImageButton"), StringComparison.Ordinal))
            {
                throw new InvalidOperationException("WPF workspace empty image prompt was not visible or localized.");
            }

            if (!shellHost.DirectResultTitleText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.EmptyTitle"), StringComparison.Ordinal)
                || !shellHost.DirectResultRouteText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.EmptyRoute"), StringComparison.Ordinal)
                || !shellHost.DirectResultStatusText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.EmptyStatus"), StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "WPF workspace empty top status banner did not describe the image-load start state. "
                    + $"Title='{shellHost.DirectResultTitleText}', Route='{shellHost.DirectResultRouteText}', Status='{shellHost.DirectResultStatusText}'");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF workspace empty beginner flow",
                "WorkspaceEmptyBeginnerFlow",
                "WorkspaceEmptyBeginnerStepLoadImage",
                "WorkspaceEmptyBeginnerStepSelectTool",
                "WorkspaceEmptyBeginnerStepPreviewCheck",
                "WorkspaceEmptySampleButton",
                "WorkspaceEmptyGuideButton",
                "WorkspaceEmptyLogHint",
                "ShellLogCollapsiblePanel",
                "ShellLogCollapsedSummary",
                "ShellLogToggleButton");
            AssertVisibleTextContains(
                shellHost,
                "WPF workspace empty localized beginner copy",
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyStepLoadTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyStepSelectTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyStepPreviewTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptySampleButton"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyGuideButton"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyLogHint"),
                OpenVisionLanguageService.T("Pipeline.RunLog"),
                OpenVisionLanguageService.T("Shell.LogPanel.Open"));
            AssertVisibleTextDoesNotContain(shellHost, "WPF workspace empty Korean copy", "Preview 확인");

            if (shellHost.RecipeCommands.LlmXmlDraftDependencyRows.Count == 0)
            {
                throw new InvalidOperationException("Recipe manager LLM XML dependency path drilldown did not expose any rows.");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
            Pump(8);
            AssertVisibleTextContains(
                shellHost,
                "WPF workspace empty English copy",
                OpenVisionLanguageService.T("Shell.WorkspaceStatus.EmptyTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyStepPreviewTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptySampleButton"));
            if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
            {
                throw new InvalidOperationException("Changing workspace empty language must not open a tool window.");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            Pump(8);
            AssertVisibleTextContains(
                shellHost,
                "WPF workspace empty Korean copy after language restore",
                OpenVisionLanguageService.T("Shell.WorkspaceStatus.EmptyTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptyStepPreviewTitle"),
                OpenVisionLanguageService.T("Shell.WorkspaceEmptySampleButton"));
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostWorkspaceImageLoad(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceImageLoad", seedMainLayer: false);
        string imagePath = CreateWorkspaceLoadSmokeImageFile();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                if (!shellHost.IsWorkspaceEmptyPromptVisible)
                {
                    throw new InvalidOperationException("WPF workspace image load test must start from the empty prompt.");
                }

                if (!shellHost.HasWorkspaceLoadImageMenu
                    || !shellHost.WorkspaceLoadImageMenuText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceLoadImageButton"), StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("WPF workspace does not expose the right-click image load menu.");
                }

                if (!shellHost.LoadMainImageFromFileForTest(imagePath))
                {
                    throw new InvalidOperationException("WPF workspace image load test helper failed.");
                }

                Pump(16);
                if (!shellHost.HasWorkspaceLayerPreview
                    || shellHost.IsWorkspaceEmptyPromptVisible
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.WorkspaceLayerMeta.Contains("640x360", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("WPF workspace did not load the selected image into Main. Title='"
                        + shellHost.WorkspaceLayerTitle
                        + "', Meta='"
                        + shellHost.WorkspaceLayerMeta
                        + "'.");
                }

                if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
                {
                    throw new InvalidOperationException("Workspace image load must not open or retarget a tool window before the operator selects one.");
                }

                if (!shellHost.IsWorkspaceMainActionVisibleForTest
                    || !shellHost.WorkspaceMainActionTitleForTest.Contains(OpenVisionLanguageService.T("Shell.MainAction.ImageReadyTitle"), StringComparison.Ordinal)
                    || !shellHost.WorkspaceMainActionDetailForTest.Contains(OpenVisionLanguageService.T("Shell.MainAction.ImageReadyDetail"), StringComparison.Ordinal)
                    || !shellHost.WorkspaceMainActionMetaForTest.Contains("Main", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Workspace image-ready next-action bar was not visible after loading Main. "
                        + $"Visible={shellHost.IsWorkspaceMainActionVisibleForTest}, "
                        + $"Title='{shellHost.WorkspaceMainActionTitleForTest}', "
                        + $"Detail='{shellHost.WorkspaceMainActionDetailForTest}', "
                        + $"Meta='{shellHost.WorkspaceMainActionMetaForTest}'");
                }

                if (!shellHost.DirectResultTitleText.Contains(OpenVisionLanguageService.T("Shell.MainAction.ImageReadyTitle"), StringComparison.Ordinal)
                    || !shellHost.DirectResultRouteText.Contains(OpenVisionLanguageService.T("Shell.MainAction.ImageReadyRoute"), StringComparison.Ordinal)
                    || !shellHost.DirectResultStatusText.Contains(OpenVisionLanguageService.T("Shell.MainAction.ImageReadyStatus"), StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "WPF workspace image-ready top status banner did not match the loaded-image state. "
                        + $"Title='{shellHost.DirectResultTitleText}', Route='{shellHost.DirectResultRouteText}', Status='{shellHost.DirectResultStatusText}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "WPF workspace image-ready action bar",
                    "WorkspaceMainActionOverlay",
                    "WorkspaceMainActionThresholdButton",
                    "WorkspaceMainActionMatchingButton",
                    "WorkspaceMainActionLineButton");
                AssertVisibleTextContains(
                    shellHost,
                    "WPF workspace image-ready localized quick actions",
                    OpenVisionLanguageService.T("Shell.MainAction.ImageReadyDetail"),
                    OpenVisionLanguageService.T("VisionMenu.Threshold"),
                    OpenVisionLanguageService.T("VisionMenu.Matching"),
                    OpenVisionLanguageService.T("VisionMenu.Line"));

                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
                Pump(8);
                AssertVisibleTextContains(
                    shellHost,
                    "WPF workspace image-ready English copy",
                    OpenVisionLanguageService.T("Shell.MainAction.ImageReadyTitle"),
                    OpenVisionLanguageService.T("Shell.MainAction.ImageReadyDetail"),
                    OpenVisionLanguageService.T("VisionMenu.Threshold"),
                    OpenVisionLanguageService.T("VisionMenu.Matching"),
                    OpenVisionLanguageService.T("VisionMenu.Line"));
                if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
                {
                    throw new InvalidOperationException("Changing image-ready language must not open or retarget a tool window.");
                }

                OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
                Pump(8);
                AssertVisibleTextContains(
                    shellHost,
                    "WPF workspace image-ready Korean copy after language restore",
                    OpenVisionLanguageService.T("Shell.MainAction.ImageReadyTitle"),
                    OpenVisionLanguageService.T("Shell.MainAction.ImageReadyDetail"),
                    OpenVisionLanguageService.T("VisionMenu.Threshold"),
                    OpenVisionLanguageService.T("VisionMenu.Matching"),
                    OpenVisionLanguageService.T("VisionMenu.Line"));

                if (!shellHost.UpdateWorkspacePointerAtCenterForTest()
                    || string.Equals(shellHost.WorkspaceCoordinatesTextForTest, "X:- Y:-", StringComparison.Ordinal)
                    || string.Equals(shellHost.WorkspacePixelTextForTest, "GV - | RGB -", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Workspace fallback viewer did not update pointer coordinate/pixel status. "
                        + $"Coordinates={shellHost.WorkspaceCoordinatesTextForTest}, Pixel={shellHost.WorkspacePixelTextForTest}");
                }

                string coordinateBeforeZoom = shellHost.GetWorkspacePointerCoordinateForTest(0.62D, 0.54D);
                shellHost.ZoomWorkspaceAtForTest(0.62D, 0.54D, 1.25D);
                Pump(4);
                string coordinateAfterZoom = shellHost.GetWorkspacePointerCoordinateForTest(0.62D, 0.54D);
                if (string.IsNullOrWhiteSpace(coordinateBeforeZoom)
                    || !string.Equals(coordinateBeforeZoom, coordinateAfterZoom, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Workspace zoom did not keep the cursor-anchored image coordinate stable. "
                        + $"Before={coordinateBeforeZoom}, After={coordinateAfterZoom}");
                }

                string coordinateBeforePan = shellHost.GetWorkspacePointerCoordinateForTest(0.5D, 0.5D);
                shellHost.PanWorkspaceByForTest(24D, -18D);
                Pump(4);
                string coordinateAfterPan = shellHost.GetWorkspacePointerCoordinateForTest(0.5D, 0.5D);
                if (string.IsNullOrWhiteSpace(coordinateBeforePan)
                    || string.Equals(coordinateBeforePan, coordinateAfterPan, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Workspace pan did not move the image coordinate under the viewport point. "
                        + $"Before={coordinateBeforePan}, After={coordinateAfterPan}");
                }
            }, captureFloatingToolWindow: false, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
        }
        finally
        {
            TryDeleteFile(imagePath);
        }
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleOpen(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceSampleOpen", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.IsWorkspaceEmptyPromptVisible)
            {
                throw new InvalidOperationException("WPF workspace sample-open test must start from the empty prompt.");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF workspace sample entry",
                "WorkspaceEmptySampleButton");

            if (!shellHost.HasRunnableWorkspaceSampleForTest)
            {
                throw new InvalidOperationException("WPF workspace sample entry did not find a runnable sample catalog item.");
            }

            shellHost.OpenFirstRunnableWorkspaceSampleForTest();
            Pump(60);

            if (!shellHost.HasMainLayer
                || !shellHost.HasWorkspaceLayerPreview
                || shellHost.IsWorkspaceEmptyPromptVisible
                || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "WPF workspace sample entry did not load the sample image into Main. "
                    + $"HasMain={shellHost.HasMainLayer}, HasPreview={shellHost.HasWorkspaceLayerPreview}, "
                    + $"Empty={shellHost.IsWorkspaceEmptyPromptVisible}, Title='{shellHost.WorkspaceLayerTitle}'");
            }

            if (!shellHost.ActivePipelineNameForTest.StartsWith("Sample_", StringComparison.Ordinal)
                || shellHost.ActivePipelineStepCountForTest <= 0)
            {
                throw new InvalidOperationException(
                    "WPF workspace sample entry did not activate a sample pipeline. "
                    + $"Pipeline='{shellHost.ActivePipelineNameForTest}', Steps={shellHost.ActivePipelineStepCountForTest}");
            }

            if (shellHost.IsWorkspaceMainActionVisibleForTest)
            {
                throw new InvalidOperationException("Sample workspace should show the sample workflow bar instead of the generic image-ready bar.");
            }

            string workflowDetail = shellHost.WorkspaceSampleWorkflowDetailForTest ?? string.Empty;
            string firstStepMenu = shellHost.WorkspaceSampleFirstStepMenuForTest ?? string.Empty;
            string activeSampleName = shellHost.ActivePipelineNameForTest.StartsWith("Sample_", StringComparison.Ordinal)
                ? shellHost.ActivePipelineNameForTest.Substring("Sample_".Length)
                : shellHost.ActivePipelineNameForTest;
            if (!shellHost.IsWorkspaceSampleWorkflowVisibleForTest
                || !shellHost.WorkspaceSampleWorkflowTitleForTest.Contains("\uC0D8\uD50C", StringComparison.Ordinal)
                || !shellHost.WorkspaceSampleWorkflowMetaForTest.Contains(activeSampleName, StringComparison.Ordinal)
                || string.IsNullOrWhiteSpace(workflowDetail)
                || string.IsNullOrWhiteSpace(firstStepMenu)
                || !workflowDetail.Contains("\uC81C\uD488\uAD70", StringComparison.Ordinal)
                || !workflowDetail.Contains("\uAE30\uC900", StringComparison.Ordinal)
                || !workflowDetail.Contains("\uB2E4\uC74C", StringComparison.Ordinal)
                || !workflowDetail.Contains(firstStepMenu, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "WPF workspace sample entry did not show the next-action workflow hint. "
                    + $"Visible={shellHost.IsWorkspaceSampleWorkflowVisibleForTest}, "
                    + $"Title='{shellHost.WorkspaceSampleWorkflowTitleForTest}', "
                    + $"Meta='{shellHost.WorkspaceSampleWorkflowMetaForTest}', "
                    + $"Detail='{shellHost.WorkspaceSampleWorkflowDetailForTest}'");
            }

            if (!shellHost.DirectResultTitleText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.SampleTitle"), StringComparison.Ordinal)
                || !shellHost.DirectResultRouteText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.SampleRoute"), StringComparison.Ordinal)
                || !shellHost.DirectResultStatusText.Contains(OpenVisionLanguageService.T("Shell.WorkspaceStatus.SampleStatus"), StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "WPF workspace sample top status banner did not match the sample-ready state. "
                    + $"Title='{shellHost.DirectResultTitleText}', Route='{shellHost.DirectResultRouteText}', Status='{shellHost.DirectResultStatusText}'");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF workspace sample workflow",
                "WorkspaceSampleWorkflowOverlay",
                "WorkspaceSamplePipelineButton",
                "WorkspaceSampleFirstStepButton");

            if (!shellHost.CanOpenSamplePipelineForTest
                || !shellHost.CanOpenSampleFirstStepToolForTest
                || string.IsNullOrWhiteSpace(shellHost.WorkspaceSampleFirstStepMenuForTest))
            {
                throw new InvalidOperationException(
                    "WPF workspace sample workflow commands were not ready. "
                    + $"Pipeline={shellHost.CanOpenSamplePipelineForTest}, "
                    + $"FirstStep={shellHost.CanOpenSampleFirstStepToolForTest}, "
                    + $"FirstMenu='{shellHost.WorkspaceSampleFirstStepMenuForTest}'");
            }

            if (shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("WPF workspace sample entry must not auto-open a tool or run Preview.");
            }
        }, captureFloatingToolWindow: false, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
    }

    private static CaptureResult CaptureShellHostRecipeContextSwitch(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

        string recipeA = "Smoke_RecipeContext_A_" + Guid.NewGuid().ToString("N");
        string recipeB = "Smoke_RecipeContext_B_" + Guid.NewGuid().ToString("N");
        const string pipelineA = "Inspection_A";
        const string pipelineB = "Inspection_B";
        const int blobMinAreaA = 321;
        const int blobMaxAreaA = 6543;
        const int blobMinAreaB = 4321;
        const int blobMaxAreaB = 8765;
        const double thresholdValueA = 77D;
        const double thresholdMaxValueA = 240D;
        const int thresholdRangeMinB = 45;
        const int thresholdRangeMaxB = 168;

        VisionPipelineStorage.Save(recipeA, CreateRecipeContextSmokePipeline(pipelineA, 1));
        VisionPipelineStorage.SaveActivePipelineName(recipeA, pipelineA);
        VisionPipelineStorage.Save(recipeB, CreateRecipeContextSmokePipeline(pipelineB, 2));
        VisionPipelineStorage.SaveActivePipelineName(recipeB, pipelineB);
        new BlobProperty("Blob_1") { MIN_AREA = blobMinAreaA, MAX_AREA = blobMaxAreaA }.SaveConfig(recipeA);
        new BlobProperty("Blob_1") { MIN_AREA = blobMinAreaB, MAX_AREA = blobMaxAreaB }.SaveConfig(recipeB);
        SaveRecipeToolSettings(recipeA, "Threshold", new ThresholdToolSettings
        {
            Mode = ThresholdToolMode.Threshold,
            Threshold = thresholdValueA,
            MaxValue = thresholdMaxValueA,
            BasicInvert = true
        });
        SaveRecipeToolSettings(recipeB, "Threshold", new ThresholdToolSettings
        {
            Mode = ThresholdToolMode.Range,
            RangeMin = thresholdRangeMinB,
            RangeMax = thresholdRangeMaxB,
            RangeInvert = true
        });

        OpenVisionShellHostView shellHost = CreateShellHost(recipeA, seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(40);

            AssertRecipeContext(shellHost, recipeA, pipelineA);
            int beforeRuns = shellHost.NativePreviewRunCount;

            shellHost.SwitchRecipeContextForTest(recipeB);
            Pump(60);
            AssertRecipeContext(shellHost, recipeB, pipelineB);

            if (shellHost.NativePreviewRunCount != beforeRuns
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe context switching must not auto-open a tool or run Preview. "
                    + $"RunsBefore={beforeRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                    + $"NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(40);
            AssertRecipeContext(shellHost, recipeA, pipelineA);

            shellHost.SelectToolForTest(VISION_MENU.Pipeline);
            Pump(80);
            if (!shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.PipelineReviewStepCount != 1
                || !string.Equals(shellHost.PipelineReviewRecipeContextNameForTest, recipeA, StringComparison.Ordinal)
                || !string.Equals(shellHost.PipelineReviewRecipeContextPipelineNameForTest, pipelineA, StringComparison.Ordinal)
                || shellHost.NativePreviewRunCount != beforeRuns
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Pipeline Review did not open against the active recipe context without running Preview. "
                    + $"Recipe={shellHost.PipelineReviewRecipeContextNameForTest}, "
                    + $"Pipeline={shellHost.PipelineReviewRecipeContextPipelineNameForTest}, "
                    + $"Steps={shellHost.PipelineReviewStepCount}, RunsBefore={beforeRuns}, "
                    + $"RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.CloseActiveWpfToolWindowForTest();
            Pump(40);

            int recipeAStepCountBeforeNativeAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountBeforeNativeAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;

            shellHost.SwitchRecipeContextForTest(recipeB);
            Pump(40);
            AssertRecipeContext(shellHost, recipeB, pipelineB);

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(80);
            if (!shellHost.IsNativeDocumentActive
                || !string.Equals(shellHost.ActiveNativeRecipeContextNameForTest, recipeB, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRecipeContextPipelineNameForTest, pipelineB, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Native tool document did not receive the active recipe context. "
                    + $"Recipe={shellHost.ActiveNativeRecipeContextNameForTest}, "
                    + $"Pipeline={shellHost.ActiveNativeRecipeContextPipelineNameForTest}, "
                    + $"Expected={recipeB}/{pipelineB}");
            }

            int beforeNativeAddRuns = shellHost.NativePreviewRunCount;
            VisionPipelineStep addedStep = shellHost.AddActiveNativePipelineStepForTest();
            Pump(40);

            int recipeAStepCountAfterNativeAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountAfterNativeAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedStep == null
                || recipeAStepCountAfterNativeAdd != recipeAStepCountBeforeNativeAdd
                || recipeBStepCountAfterNativeAdd != recipeBStepCountBeforeNativeAdd + 1
                || !string.Equals(VisionPipelineStorage.LoadActivePipelineName(recipeB, VisionPipelineAppendService.DefaultPipelineName), pipelineB, StringComparison.Ordinal)
                || shellHost.NativePreviewRunCount != beforeNativeAddRuns
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Native Add Pipeline did not stay scoped to the active recipe/pipeline context. "
                    + $"Added={(addedStep == null ? "<null>" : addedStep.Name)}, "
                    + $"A={recipeAStepCountBeforeNativeAdd}->{recipeAStepCountAfterNativeAdd}, "
                    + $"B={recipeBStepCountBeforeNativeAdd}->{recipeBStepCountAfterNativeAdd}, "
                    + $"ActiveB={VisionPipelineStorage.LoadActivePipelineName(recipeB, VisionPipelineAppendService.DefaultPipelineName)}, "
                    + $"RunsBefore={beforeNativeAddRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"Preview={shellHost.HasNativePreviewResult}");
            }
            AssertStepParameter(addedStep, nameof(BlobProperty.MIN_AREA), blobMinAreaB, "Recipe B Blob Add Pipeline");
            AssertStepParameter(addedStep, nameof(BlobProperty.MAX_AREA), blobMaxAreaB, "Recipe B Blob Add Pipeline");

            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(60);
            AssertRecipeContext(shellHost, recipeA, pipelineA);

            if (shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe switch after a native PropertyGrid tool must close active tool documents without leaving stale preview state. "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                    + $"NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(80);
            if (!shellHost.IsNativeDocumentActive
                || !string.Equals(shellHost.ActiveNativeRecipeContextNameForTest, recipeA, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRecipeContextPipelineNameForTest, pipelineA, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Native tool document did not recreate against recipe A after switching back. "
                    + $"Recipe={shellHost.ActiveNativeRecipeContextNameForTest}, "
                    + $"Pipeline={shellHost.ActiveNativeRecipeContextPipelineNameForTest}, "
                    + $"Expected={recipeA}/{pipelineA}");
            }

            int beforeRecipeAAddRuns = shellHost.NativePreviewRunCount;
            VisionPipelineStep addedRecipeAStep = shellHost.AddActiveNativePipelineStepForTest();
            Pump(40);

            int recipeAStepCountAfterRecipeAAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountAfterRecipeAAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedRecipeAStep == null
                || recipeAStepCountAfterRecipeAAdd != recipeAStepCountBeforeNativeAdd + 1
                || recipeBStepCountAfterRecipeAAdd != recipeBStepCountAfterNativeAdd
                || !string.Equals(VisionPipelineStorage.LoadActivePipelineName(recipeA, VisionPipelineAppendService.DefaultPipelineName), pipelineA, StringComparison.Ordinal)
                || shellHost.NativePreviewRunCount != beforeRecipeAAddRuns
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Native Add Pipeline after switching back to recipe A did not stay scoped to recipe A. "
                    + $"Added={(addedRecipeAStep == null ? "<null>" : addedRecipeAStep.Name)}, "
                    + $"A={recipeAStepCountBeforeNativeAdd}->{recipeAStepCountAfterRecipeAAdd}, "
                    + $"B={recipeBStepCountAfterNativeAdd}->{recipeBStepCountAfterRecipeAAdd}, "
                    + $"ActiveA={VisionPipelineStorage.LoadActivePipelineName(recipeA, VisionPipelineAppendService.DefaultPipelineName)}, "
                    + $"RunsBefore={beforeRecipeAAddRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"Preview={shellHost.HasNativePreviewResult}");
            }
            AssertStepParameter(addedRecipeAStep, nameof(BlobProperty.MIN_AREA), blobMinAreaA, "Recipe A Blob Add Pipeline");
            AssertStepParameter(addedRecipeAStep, nameof(BlobProperty.MAX_AREA), blobMaxAreaA, "Recipe A Blob Add Pipeline");

            shellHost.SwitchRecipeContextForTest(recipeB);
            Pump(60);
            AssertRecipeContext(shellHost, recipeB, pipelineB);

            int recipeAStepCountBeforeThresholdBAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountBeforeThresholdBAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;

            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(80);
            if (!shellHost.IsNativeDocumentActive
                || !string.Equals(shellHost.ActiveNativeRecipeContextNameForTest, recipeB, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRecipeContextPipelineNameForTest, pipelineB, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Threshold native tool document did not recreate against recipe B. "
                    + $"Recipe={shellHost.ActiveNativeRecipeContextNameForTest}, "
                    + $"Pipeline={shellHost.ActiveNativeRecipeContextPipelineNameForTest}, "
                    + $"Expected={recipeB}/{pipelineB}");
            }

            int beforeThresholdBAddRuns = shellHost.NativePreviewRunCount;
            VisionPipelineStep addedThresholdBStep = shellHost.AddActiveNativePipelineStepForTest();
            Pump(40);

            int recipeAStepCountAfterThresholdBAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountAfterThresholdBAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedThresholdBStep == null
                || recipeAStepCountAfterThresholdBAdd != recipeAStepCountBeforeThresholdBAdd
                || recipeBStepCountAfterThresholdBAdd != recipeBStepCountBeforeThresholdBAdd + 1
                || shellHost.NativePreviewRunCount != beforeThresholdBAddRuns
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe B Threshold Add Pipeline did not stay scoped to recipe B or triggered Preview. "
                    + $"Added={(addedThresholdBStep == null ? "<null>" : addedThresholdBStep.Name)}, "
                    + $"A={recipeAStepCountBeforeThresholdBAdd}->{recipeAStepCountAfterThresholdBAdd}, "
                    + $"B={recipeBStepCountBeforeThresholdBAdd}->{recipeBStepCountAfterThresholdBAdd}, "
                    + $"RunsBefore={beforeThresholdBAddRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"Preview={shellHost.HasNativePreviewResult}");
            }
            AssertStepParameter(addedThresholdBStep, nameof(ThresholdToolProperty.Mode), ThresholdToolMode.Range, "Recipe B Threshold Add Pipeline");
            AssertStepParameter(addedThresholdBStep, nameof(ThresholdToolProperty.RangeMin), thresholdRangeMinB, "Recipe B Threshold Add Pipeline");
            AssertStepParameter(addedThresholdBStep, nameof(ThresholdToolProperty.RangeMax), thresholdRangeMaxB, "Recipe B Threshold Add Pipeline");

            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(60);
            AssertRecipeContext(shellHost, recipeA, pipelineA);

            int recipeAStepCountBeforeThresholdAAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountBeforeThresholdAAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;

            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(80);
            if (!shellHost.IsNativeDocumentActive
                || !string.Equals(shellHost.ActiveNativeRecipeContextNameForTest, recipeA, StringComparison.Ordinal)
                || !string.Equals(shellHost.ActiveNativeRecipeContextPipelineNameForTest, pipelineA, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Threshold native tool document did not recreate against recipe A. "
                    + $"Recipe={shellHost.ActiveNativeRecipeContextNameForTest}, "
                    + $"Pipeline={shellHost.ActiveNativeRecipeContextPipelineNameForTest}, "
                    + $"Expected={recipeA}/{pipelineA}");
            }

            int beforeThresholdAAddRuns = shellHost.NativePreviewRunCount;
            VisionPipelineStep addedThresholdAStep = shellHost.AddActiveNativePipelineStepForTest();
            Pump(40);

            int recipeAStepCountAfterThresholdAAdd = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepCountAfterThresholdAAdd = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedThresholdAStep == null
                || recipeAStepCountAfterThresholdAAdd != recipeAStepCountBeforeThresholdAAdd + 1
                || recipeBStepCountAfterThresholdAAdd != recipeBStepCountBeforeThresholdAAdd
                || shellHost.NativePreviewRunCount != beforeThresholdAAddRuns
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe A Threshold Add Pipeline did not stay scoped to recipe A or triggered Preview. "
                    + $"Added={(addedThresholdAStep == null ? "<null>" : addedThresholdAStep.Name)}, "
                    + $"A={recipeAStepCountBeforeThresholdAAdd}->{recipeAStepCountAfterThresholdAAdd}, "
                    + $"B={recipeBStepCountBeforeThresholdAAdd}->{recipeBStepCountAfterThresholdAAdd}, "
                    + $"RunsBefore={beforeThresholdAAddRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"Preview={shellHost.HasNativePreviewResult}");
            }
            AssertStepParameter(addedThresholdAStep, nameof(ThresholdToolProperty.Mode), ThresholdToolMode.Threshold, "Recipe A Threshold Add Pipeline");
            AssertStepParameter(addedThresholdAStep, nameof(ThresholdToolProperty.Threshold), thresholdValueA, "Recipe A Threshold Add Pipeline");
            AssertStepParameter(addedThresholdAStep, nameof(ThresholdToolProperty.MaxValue), thresholdMaxValueA, "Recipe A Threshold Add Pipeline");

            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe context status",
                "HostRecipeContext");
            AssertVisibleTextContains(shellHost, "WPF recipe context scope hint", "범위:");
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostRecipeOutputRouteIsolation(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);

        string recipeA = "Smoke_RecipeRoute_A_" + Guid.NewGuid().ToString("N");
        string recipeB = "Smoke_RecipeRoute_B_" + Guid.NewGuid().ToString("N");
        const string pipelineA = "OutputRoute_A";
        const string pipelineB = "OutputRoute_B";
        const string outputA = "RecipeA_Output";
        const string outputB = "RecipeB_Output";

        VisionPipelineStorage.Save(recipeA, CreateRecipeContextSmokePipeline(pipelineA, 1));
        VisionPipelineStorage.SaveActivePipelineName(recipeA, pipelineA);
        VisionPipelineStorage.Save(recipeB, CreateRecipeContextSmokePipeline(pipelineB, 1));
        VisionPipelineStorage.SaveActivePipelineName(recipeB, pipelineB);

        OpenVisionShellHostView shellHost = CreateShellHost(recipeA, seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap outputABitmap = CreateDockingPanelSmokeBitmap(5);
        using Bitmap outputBBitmap = CreateDockingPanelSmokeBitmap(6);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(40);
            AssertRecipeContext(shellHost, recipeA, pipelineA);
            shellHost.SetMainLayerImageForTest(mainBitmap);
            EnsureRecipeRouteLayer(shellHost, outputA, outputABitmap);
            EnsureRecipeRouteLayer(shellHost, outputB, outputBBitmap);
            ActivateMainOrThrow(shellHost, "Recipe route isolation A setup");

            int previewRunsBefore = shellHost.NativePreviewRunCount;
            int recipeAStepsBefore = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepsBefore = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;

            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(80);
            SelectThresholdOutputLayerForRecipeSmoke(shellHost, outputA, recipeA);
            VisionPipelineStep addedA = shellHost.AddActiveNativePipelineStepForTest();
            Pump(30);

            int recipeAStepsAfterA = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepsAfterA = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedA == null
                || !string.Equals(addedA.OutputLayer, outputA, StringComparison.Ordinal)
                || recipeAStepsAfterA != recipeAStepsBefore + 1
                || recipeBStepsAfterA != recipeBStepsBefore
                || shellHost.NativePreviewRunCount != previewRunsBefore
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe A selected output route was not written to recipe A Add Pipeline without Preview side effects. "
                    + $"AddedOutput={addedA?.OutputLayer}, A={recipeAStepsBefore}->{recipeAStepsAfterA}, "
                    + $"B={recipeBStepsBefore}->{recipeBStepsAfterA}, RunsBefore={previewRunsBefore}, "
                    + $"RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SwitchRecipeContextForTest(recipeB);
            Pump(60);
            AssertRecipeContext(shellHost, recipeB, pipelineB);
            if (shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe switch after selecting an output route must close the active tool without stale Preview state. "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SetMainLayerImageForTest(mainBitmap);
            EnsureRecipeRouteLayer(shellHost, outputB, outputBBitmap);
            ActivateMainOrThrow(shellHost, "Recipe route isolation B setup");
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(80);

            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            if (string.Equals(GetComboBoxCurrentText(outputLayerCombo), outputA, StringComparison.OrdinalIgnoreCase)
                || string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputA, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe B opened with Recipe A's selected output route. "
                    + $"OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, Route={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
            }

            int recipeAStepsBeforeB = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepsBeforeB = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            SelectThresholdOutputLayerForRecipeSmoke(shellHost, outputB, recipeB);
            VisionPipelineStep addedB = shellHost.AddActiveNativePipelineStepForTest();
            Pump(30);

            int recipeAStepsAfterB = VisionPipelineStorage.Load(recipeA, pipelineA).Steps.Count;
            int recipeBStepsAfterB = VisionPipelineStorage.Load(recipeB, pipelineB).Steps.Count;
            if (addedB == null
                || !string.Equals(addedB.OutputLayer, outputB, StringComparison.Ordinal)
                || recipeAStepsAfterB != recipeAStepsBeforeB
                || recipeBStepsAfterB != recipeBStepsBeforeB + 1
                || shellHost.NativePreviewRunCount != previewRunsBefore
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe B selected output route was not written to recipe B Add Pipeline without Preview side effects. "
                    + $"AddedOutput={addedB?.OutputLayer}, A={recipeAStepsBeforeB}->{recipeAStepsAfterB}, "
                    + $"B={recipeBStepsBeforeB}->{recipeBStepsAfterB}, RunsBefore={previewRunsBefore}, "
                    + $"RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SwitchRecipeContextForTest(recipeA);
            Pump(60);
            AssertRecipeContext(shellHost, recipeA, pipelineA);
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(80);
            outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            if (string.Equals(GetComboBoxCurrentText(outputLayerCombo), outputB, StringComparison.OrdinalIgnoreCase)
                || string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputB, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe A reopened with Recipe B's selected output route. "
                    + $"OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, Route={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static void EnsureRecipeRouteLayer(OpenVisionShellHostView shellHost, string layerTitle, Bitmap image)
    {
        if (shellHost.HasLayerForTest(layerTitle))
        {
            if (!shellHost.SetLayerImageForTest(layerTitle, image))
            {
                throw new InvalidOperationException($"Recipe route layer {layerTitle} could not be refreshed.");
            }

            return;
        }

        if (!shellHost.AddLayerImageForTest(layerTitle, image))
        {
            throw new InvalidOperationException($"Recipe route layer {layerTitle} could not be created.");
        }
    }

    private static void ActivateMainOrThrow(OpenVisionShellHostView shellHost, string name)
    {
        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException(name + " could not activate Main.");
        }
    }

    private static void SelectThresholdOutputLayerForRecipeSmoke(OpenVisionShellHostView shellHost, string outputLayer, string recipeName)
    {
        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!ComboBoxContainsText(outputLayerCombo, outputLayer))
        {
            throw new InvalidOperationException($"Threshold output combo for {recipeName} does not expose {outputLayer}.");
        }

        SelectComboBoxItemText(outputLayerCombo, outputLayer, recipeName + " Threshold output combo");
        Pump(20);
        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), outputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, outputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Threshold route selection for {recipeName} did not preserve Main input and selected output. "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, "
                + $"Active={shellHost.ActiveHostLayerTitle}");
        }
    }

    private static VisionPipeline CreateRecipeContextSmokePipeline(string name, int stepCount)
    {
        VisionPipeline pipeline = new() { Name = name };
        for (int index = 0; index < stepCount; index++)
        {
            pipeline.Steps.Add(new VisionPipelineStep
            {
                Name = $"{name}_Step_{index + 1}",
                ToolType = "Threshold",
                InputLayer = index == 0 ? "Main" : $"{name}_Preview_{index}",
                OutputLayer = $"{name}_Preview_{index + 1}"
            });
        }

        return pipeline;
    }

    private static void SaveRecipeToolSettings<TSettings>(string recipeName, string toolName, TSettings settings)
        where TSettings : class
    {
        string configName = OpenVisionNativeToolSettingsStore.CreateConfigName(toolName);
        string path = RecipeWorkspaceService.GetVisionConfigPath(recipeName, configName);
        SerializeHelper.SaveXmlFile(path, settings);
    }

    private static void CleanupTransientRecipeWorkspaces(params string[] keepRecipeNames)
    {
        HashSet<string> keep = new(
            (keepRecipeNames ?? Array.Empty<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);
        keep.Add("Default");

        foreach (string recipeName in RecipeWorkspaceService.GetRecipeNames())
        {
            if (keep.Contains(recipeName)
                || (!recipeName.StartsWith("Smoke_", StringComparison.OrdinalIgnoreCase)
                    && !recipeName.StartsWith("Recipe_", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    private static CaptureResult CaptureShellHostRecipeLanguageControls(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        CleanupTransientRecipeWorkspaces();

        string recipeA = "Smoke_RecipeControls_A_" + Guid.NewGuid().ToString("N");
        string recipeB = "Smoke_RecipeControls_B_" + Guid.NewGuid().ToString("N");
        VisionPipelineStorage.Save(recipeA, CreateRecipeContextSmokePipeline("Inspection_A", 1));
        VisionPipelineStorage.SaveActivePipelineName(recipeA, "Inspection_A");
        VisionPipelineStorage.Save(recipeB, CreateRecipeContextSmokePipeline("Inspection_B", 1));
        VisionPipelineStorage.SaveActivePipelineName(recipeB, "Inspection_B");

        OpenVisionShellHostView shellHost = CreateShellHost(recipeA, seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            AssertHostComboBoxInteraction(shellHost, "cbHostLanguage", "Host language combo");
            AssertHostComboBoxInteraction(shellHost, "cbHostRecipe", "Host recipe combo");

            const string koreanDisplayName = "\uD55C\uAD6D\uC5B4";
            if (!string.Equals(shellHost.SelectedLanguageDisplayNameForTest, koreanDisplayName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Language selector did not expose a readable Korean display name. "
                    + $"Actual='{shellHost.SelectedLanguageDisplayNameForTest}'");
            }

            shellHost.SelectLanguageForTest(OpenVisionLanguage.English);
            Pump(40);
            if (OpenVisionLanguageService.CurrentLanguage != OpenVisionLanguage.English
                || !string.Equals(shellHost.SelectedLanguageDisplayNameForTest, "English", StringComparison.Ordinal)
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Language selector did not switch to English through the Shell binding path without side effects. "
                    + $"Language={OpenVisionLanguageService.CurrentLanguage}, Display='{shellHost.SelectedLanguageDisplayNameForTest}', "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SelectLanguageForTest(OpenVisionLanguage.Korean);
            Pump(40);
            if (OpenVisionLanguageService.CurrentLanguage != OpenVisionLanguage.Korean
                || !string.Equals(shellHost.SelectedLanguageDisplayNameForTest, koreanDisplayName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Language selector did not switch back to Korean. "
                    + $"Language={OpenVisionLanguageService.CurrentLanguage}, Display='{shellHost.SelectedLanguageDisplayNameForTest}'");
            }

            if (!shellHost.RecipeOptionsForTest.Contains(recipeA, StringComparer.OrdinalIgnoreCase)
                || !shellHost.RecipeOptionsForTest.Contains(recipeB, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe selector did not list existing recipe workspaces. "
                    + $"Options='{string.Join(", ", shellHost.RecipeOptionsForTest)}'");
            }

            ComboBox hostRecipeCombo = FindNamedVisualChild<ComboBox>(shellHost, "cbHostRecipe")
                ?? throw new InvalidOperationException("Host recipe combo was not found for recipe switching.");
            SelectComboBoxItemText(hostRecipeCombo, recipeB, "Host recipe combo switch to recipe B");
            Pump(100);
            AssertRecipeContext(shellHost, recipeB, "Inspection_B");
            SelectComboBoxItemText(hostRecipeCombo, recipeA, "Host recipe combo switch back to recipe A");
            Pump(100);
            AssertRecipeContext(shellHost, recipeA, "Inspection_A");

            shellHost.RecipeCommands.RecipeFilterText = recipeB;
            Pump(20);
            if (!shellHost.RecipeCommands.FilteredRecipeOptions.Contains(recipeB, StringComparer.OrdinalIgnoreCase)
                || shellHost.RecipeCommands.FilteredRecipeOptions.Contains(recipeA, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe manager filter did not narrow the visible recipe list. "
                    + $"Filter='{shellHost.RecipeCommands.RecipeFilterText}', Results='{string.Join(", ", shellHost.RecipeCommands.FilteredRecipeOptions)}'");
            }

            shellHost.RecipeCommands.RecipeFilterText = string.Empty;
            string managedRecipe = "Smoke_RecipeManager_" + Guid.NewGuid().ToString("N");
            shellHost.RecipeCommands.EditRecipeName = managedRecipe;
            if (!shellHost.RecipeCommands.CreateNamedRecipeCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager named create command was disabled for " + managedRecipe);
            }

            shellHost.RecipeCommands.CreateNamedRecipeCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.SelectedRecipeNameForTest, managedRecipe, StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeOptionsForTest.Contains(managedRecipe, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe manager named create did not create and select the requested recipe. "
                    + $"Selected='{shellHost.SelectedRecipeNameForTest}', Options='{string.Join(", ", shellHost.RecipeOptionsForTest)}'");
            }

            string renamedManagedRecipe = managedRecipe + "_Renamed";
            shellHost.RecipeCommands.EditRecipeName = renamedManagedRecipe;
            if (!shellHost.RecipeCommands.RenameRecipeCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager rename command was disabled for " + renamedManagedRecipe);
            }

            shellHost.RecipeCommands.RenameRecipeCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.SelectedRecipeNameForTest, renamedManagedRecipe, StringComparison.OrdinalIgnoreCase)
                || shellHost.RecipeOptionsForTest.Contains(managedRecipe, StringComparer.OrdinalIgnoreCase)
                || !shellHost.RecipeOptionsForTest.Contains(renamedManagedRecipe, StringComparer.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.DeleteRecipeCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager rename/delete command state did not update. "
                    + $"Selected='{shellHost.SelectedRecipeNameForTest}', Options='{string.Join(", ", shellHost.RecipeOptionsForTest)}'");
            }

            int beforeRecipeManagerRuns = shellHost.NativePreviewRunCount;
            string duplicatedManagedRecipe = renamedManagedRecipe + "_Copy";
            shellHost.RecipeCommands.EditRecipeName = duplicatedManagedRecipe;
            if (!shellHost.RecipeCommands.DuplicateRecipeCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager duplicate command was disabled for " + duplicatedManagedRecipe);
            }

            shellHost.RecipeCommands.DuplicateRecipeCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.SelectedRecipeNameForTest, duplicatedManagedRecipe, StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeOptionsForTest.Contains(duplicatedManagedRecipe, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe manager duplicate did not create and select the copied recipe. "
                    + $"Selected='{shellHost.SelectedRecipeNameForTest}', Options='{string.Join(", ", shellHost.RecipeOptionsForTest)}'");
            }

            OpenVisionRecipeSampleOption? sampleOption = shellHost.RecipeCommands.SampleOptions.FirstOrDefault();
            if (sampleOption == null)
            {
                throw new InvalidOperationException("Recipe manager did not expose any runnable sample pipelines for duplicate-from-sample.");
            }

            int pipelineCountBeforeSampleDuplicate = shellHost.RecipeCommands.SelectedRecipeSummary.PipelineCount;
            shellHost.RecipeCommands.SelectedSampleOption = sampleOption;
            if (!shellHost.RecipeCommands.DuplicateFromSampleCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager duplicate-from-sample command was disabled for " + sampleOption.DisplayText);
            }

            shellHost.RecipeCommands.DuplicateFromSampleCommand.Execute(null);
            Pump(100);
            OpenVisionRecipeManagerSummary sampleDuplicateSummary = shellHost.RecipeCommands.SelectedRecipeSummary;
            if (!sampleDuplicateSummary.ActivePipelineName.StartsWith("Sample_", StringComparison.OrdinalIgnoreCase)
                || sampleDuplicateSummary.PipelineCount <= pipelineCountBeforeSampleDuplicate
                || sampleDuplicateSummary.PipelinePreviewSteps.Count == 0
                || !ContainsAny(
                    sampleDuplicateSummary.LlmXmlValidationReport,
                    "LLM XML validation: OK",
                    "LLM XML 검증: OK"))
            {
                throw new InvalidOperationException(
                    "Recipe manager duplicate-from-sample did not activate a validated sample pipeline preview. "
                    + $"Sample='{sampleOption.DisplayText}', Active='{sampleDuplicateSummary.ActivePipelineName}', "
                    + $"PipelinesBefore={pipelineCountBeforeSampleDuplicate}, PipelinesAfter={sampleDuplicateSummary.PipelineCount}, "
                    + $"Steps={sampleDuplicateSummary.PipelinePreviewSteps.Count}, Report='{sampleDuplicateSummary.LlmXmlValidationReport}'");
            }

            string importPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_import_pipeline_" + Guid.NewGuid().ToString("N") + ".xml");
            string exportPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_export_pipeline_" + Guid.NewGuid().ToString("N") + ".xml");
            VisionPipeline importedPipeline = CreateRecipeContextSmokePipeline("Imported_Manager", 2);
            SerializeHelper.SaveXmlFile(importPath, importedPipeline);
            if (!shellHost.RecipeCommands.ImportPipelineXmlFromPath(importPath)
                || !string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager", StringComparison.OrdinalIgnoreCase)
                || VisionPipelineStorage.Load(shellHost.SelectedRecipeNameForTest, "Imported_Manager").Steps.Count != 2)
            {
                throw new InvalidOperationException(
                    "Recipe manager import XML did not load and activate the imported pipeline. "
                    + $"Recipe='{shellHost.SelectedRecipeNameForTest}', Active='{shellHost.ActiveRecipeContextPipelineNameForTest}'");
            }

            OpenVisionRecipeManagerSummary importSummary = shellHost.RecipeCommands.SelectedRecipeSummary;
            if (importSummary.PipelinePreviewSteps.Count != 2
                || !importSummary.PipelinePreviewSteps[0].DisplayText.Contains("Imported_Manager_Step_1", StringComparison.OrdinalIgnoreCase)
                || !ContainsAny(
                    importSummary.LlmXmlValidationReport,
                    "Schema/routing: OK",
                    "스키마/경로: OK"))
            {
                throw new InvalidOperationException(
                    "Recipe manager did not expose the imported XML validation report and preview step list. "
                    + $"Steps={importSummary.PipelinePreviewSteps.Count}, Report='{importSummary.LlmXmlValidationReport}'");
            }

            int pipelineCountBeforeDuplicate = shellHost.RecipeCommands.PipelineOptions.Count;
            shellHost.RecipeCommands.PipelineEditName = "Imported_Manager_Copy";
            if (!shellHost.RecipeCommands.DuplicatePipelineCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager pipeline duplicate command was disabled. "
                    + $"Recipe='{shellHost.SelectedRecipeNameForTest}', SelectedPipeline='{shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName}', "
                    + $"PipelineEdit='{shellHost.RecipeCommands.PipelineEditName}', Options='{string.Join(", ", shellHost.RecipeCommands.PipelineOptions.Select(option => option.PipelineName))}'");
            }

            shellHost.RecipeCommands.DuplicatePipelineCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager_Copy", StringComparison.OrdinalIgnoreCase)
                || shellHost.RecipeCommands.PipelineOptions.Count <= pipelineCountBeforeDuplicate)
            {
                throw new InvalidOperationException(
                    "Recipe manager pipeline duplicate did not create and activate the copied pipeline. "
                    + $"Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', Before={pipelineCountBeforeDuplicate}, After={shellHost.RecipeCommands.PipelineOptions.Count}");
            }

            shellHost.RecipeCommands.PipelineEditName = "Imported_Manager_Renamed";
            if (!shellHost.RecipeCommands.RenamePipelineCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager pipeline rename command was disabled.");
            }

            shellHost.RecipeCommands.RenamePipelineCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager_Renamed", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.PipelineOptions.Any(option => string.Equals(option.PipelineName, "Imported_Manager_Renamed", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Recipe manager pipeline rename did not update active context and pipeline list. "
                    + $"Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', Options='{string.Join(", ", shellHost.RecipeCommands.PipelineOptions.Select(option => option.PipelineName))}'");
            }

            OpenVisionRecipePipelineOption? importedPipelineOption = shellHost.RecipeCommands.PipelineOptions.FirstOrDefault(option =>
                string.Equals(option.PipelineName, "Imported_Manager", StringComparison.OrdinalIgnoreCase));
            if (importedPipelineOption == null)
            {
                throw new InvalidOperationException("Recipe manager pipeline list did not contain Imported_Manager after duplicate/rename.");
            }

            shellHost.RecipeCommands.SelectedPipelineOption = importedPipelineOption;
            if (!shellHost.RecipeCommands.ActivatePipelineCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager pipeline activate command was disabled.");
            }

            shellHost.RecipeCommands.ActivatePipelineCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager", StringComparison.OrdinalIgnoreCase)
                || shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count != 2)
            {
                throw new InvalidOperationException(
                    "Recipe manager pipeline activate did not switch the active pipeline without running preview. "
                    + $"Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', Steps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
            }

            string llmDependencyPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_llm_dependency_" + Guid.NewGuid().ToString("N") + ".png");
            using (Bitmap dependencyImage = CreateDockingPanelSmokeBitmap(17))
            {
                dependencyImage.Save(llmDependencyPath, ImageFormat.Png);
            }

            string llmDraftPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_llm_draft_pipeline_" + Guid.NewGuid().ToString("N") + ".xml");
            VisionPipeline llmDraftPipeline = CreateRecipeContextSmokePipeline("LLM_Draft_Manager", 2);
            llmDraftPipeline.Steps[0].Parameters["TemplatePath"] = llmDependencyPath;
            SerializeHelper.SaveXmlFile(llmDraftPath, llmDraftPipeline);
            if (shellHost.RecipeCommands.UseSelectedSampleReferenceCommand.CanExecute(null))
            {
                shellHost.RecipeCommands.UseSelectedSampleReferenceCommand.Execute(null);
                Pump(20);
            }

            shellHost.RecipeCommands.SelectedLlmToolTemplate = "Template Matching";
            shellHost.RecipeCommands.LlmInspectionGoalText = "Find the marked product feature and fail if the score is below threshold.";
            shellHost.RecipeCommands.LlmDetectionPointText = "Use Main as input, write only to a new result layer, and keep template dependencies explicit.";
            if (!shellHost.RecipeCommands.BuildLlmPromptCommand.CanExecute(null)
                || !shellHost.RecipeCommands.CreateLlmTemplateXmlDraftCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager LLM assistant commands were disabled.");
            }

            shellHost.RecipeCommands.BuildLlmPromptCommand.Execute(null);
            Pump(20);
            if (!shellHost.RecipeCommands.LlmPromptText.Contains("OpenVisionLab VisionPipeline XML draft", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.LlmPromptText.Contains("Template Matching", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.LlmPromptText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.LlmPromptText.Contains("do not run Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Recipe manager LLM assistant did not build a grounded prompt.");
            }
            if (!shellHost.RecipeCommands.CopyLlmPromptCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager LLM prompt copy command was disabled after prompt generation.");
            }

            shellHost.RecipeCommands.CopyLlmPromptCommand.Execute(null);
            Pump(40);
            if (!ContainsAny(shellHost.RecipeCommands.LlmPromptCopyStatusText, "copied", "복사"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM prompt copy command did not report success. "
                    + $"Status='{shellHost.RecipeCommands.LlmPromptCopyStatusText}'");
            }

            shellHost.RecipeCommands.CreateLlmTemplateXmlDraftCommand.Execute(null);
            Pump(80);
            if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("<VisionPipeline", StringComparison.OrdinalIgnoreCase)
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftValidationReport,
                    "LLM draft validation: OK",
                    "LLM 초안 검증: OK")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.Status")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.Evidence")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftReviewReport,
                    "Draft import review: READY",
                    "초안 가져오기 검토: 준비됨")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftReviewReport,
                    "Step count delta",
                    "단계 수 변화")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDiffReport,
                    "LLM XML diff review: READY",
                    "LLM XML 변경점: 준비됨")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDiffReport,
                    "Step count delta",
                    "단계 수 변화")
                || !shellHost.RecipeCommands.RefreshLlmDraftReviewCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM assistant did not create a valid XML starter with import review. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                    + $"Review='{shellHost.RecipeCommands.LlmXmlDraftReviewReport}', "
                    + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
            }
            if (!shellHost.RecipeCommands.CopyLlmReviewBundleCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager LLM review bundle copy command was disabled after XML starter creation.");
            }

            shellHost.RecipeCommands.CopyLlmReviewBundleCommand.Execute(null);
            Pump(40);
            if (!ContainsAny(shellHost.RecipeCommands.LlmReviewBundleCopyStatusText, "copied", "복사"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM review bundle copy command did not report success. "
                    + $"Status='{shellHost.RecipeCommands.LlmReviewBundleCopyStatusText}'");
            }

            string llmReviewBundleClipboard = System.Windows.Clipboard.GetText();
            if (!llmReviewBundleClipboard.Contains("Selected step operator context", StringComparison.OrdinalIgnoreCase)
                || !llmReviewBundleClipboard.Contains("Failure review", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM review bundle did not include selected-step operator context. "
                    + $"Clipboard='{llmReviewBundleClipboard}'");
            }

            System.Windows.Clipboard.SetText(File.ReadAllText(llmDraftPath));
            shellHost.RecipeCommands.PasteLlmXmlDraftFromClipboardCommand.Execute(null);
            Pump(40);
            if (!shellHost.RecipeCommands.LlmXmlDraftText.Contains("LLM_Draft_Manager", StringComparison.OrdinalIgnoreCase)
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftPasteStatusText, "Pasted", "붙여넣"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft was not pasted from the clipboard. "
                    + $"Status='{shellHost.RecipeCommands.LlmXmlDraftPasteStatusText}'");
            }

            if (!shellHost.RecipeCommands.LoadLlmXmlDraftFromPath(llmDraftPath)
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftValidationReport,
                    "LLM draft validation: OK",
                    "LLM 초안 검증: OK")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDependencyReport,
                    "Found:",
                    "찾음:")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftReviewReport,
                    "Draft import review: READY",
                    "초안 가져오기 검토: 준비됨")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDiffReport,
                    "Change summary",
                    "변경 요약")
                || !shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                || !shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft did not load and validate before import. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                    + $"Dependencies='{shellHost.RecipeCommands.LlmXmlDraftDependencyReport}', "
                    + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
            }

            string customInspectionDraftPath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_llm_custom_inspection_" + Guid.NewGuid().ToString("N") + ".xml");
            VisionPipeline customInspectionDraftPipeline = CreateRecipeContextSmokePipeline("LLM_CustomInspection_Manager", 1);
            customInspectionDraftPipeline.Steps[0].Parameters["Inspection.Status"] = "OK";
            SerializeHelper.SaveXmlFile(customInspectionDraftPath, customInspectionDraftPipeline);
            string selectedPipelineBeforeCustomInspectionImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
            shellHost.RecipeCommands.LlmXmlDraftText = File.ReadAllText(customInspectionDraftPath);
            if (shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.*")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "not XML nodes", "리뷰 채널"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML custom Inspection.* draft was not blocked before import. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
            }

            if (shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
            {
                shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
                Pump(40);
            }

            string selectedPipelineAfterCustomInspectionImport = shellHost.RecipeCommands.SelectedPipelineOption?.PipelineName ?? string.Empty;
            if (!string.Equals(selectedPipelineBeforeCustomInspectionImport, selectedPipelineAfterCustomInspectionImport, StringComparison.OrdinalIgnoreCase)
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.*"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML custom Inspection.* import attempt changed pipeline state or lost validation context. "
                    + $"Before='{selectedPipelineBeforeCustomInspectionImport}', After='{selectedPipelineAfterCustomInspectionImport}', "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
            }

            shellHost.RecipeCommands.LlmXmlDraftText = File.ReadAllText(llmDraftPath);
            if (!shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                || !shellHost.RecipeCommands.ImportLlmXmlDraftCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft did not recover after custom Inspection.* validation failure. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}'");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
            shellHost.RecipeCommands.RefreshLocalization();
            if (!shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDependencyReport, "Dependency scan report")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDependencyReport, "Found:")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Result channels:")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.Status")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftReviewReport, "Draft import review: READY")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftReviewReport, "Step count delta")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDiffReport, "LLM XML diff review: READY")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDiffReport, "Change summary"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft reports were not available in English. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                    + $"Dependencies='{shellHost.RecipeCommands.LlmXmlDraftDependencyReport}', "
                    + $"Review='{shellHost.RecipeCommands.LlmXmlDraftReviewReport}', "
                    + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            shellHost.RecipeCommands.RefreshLocalization();
            if (!shellHost.RecipeCommands.ValidateLlmXmlDraftTextForTest()
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDependencyReport, "의존 파일 스캔 보고서")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDependencyReport, "찾음:")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "판정 출력 채널:")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftValidationReport, "Inspection.Status")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftReviewReport, "초안 가져오기 검토: 준비됨")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftReviewReport, "단계 수 변화")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDiffReport, "LLM XML 변경점: 준비됨")
                || !ContainsAny(shellHost.RecipeCommands.LlmXmlDraftDiffReport, "변경 요약"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft reports were not available in Korean. "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                    + $"Dependencies='{shellHost.RecipeCommands.LlmXmlDraftDependencyReport}', "
                    + $"Review='{shellHost.RecipeCommands.LlmXmlDraftReviewReport}', "
                    + $"Diff='{shellHost.RecipeCommands.LlmXmlDraftDiffReport}'");
            }

            shellHost.RecipeCommands.ImportLlmXmlDraftCommand.Execute(null);
            Pump(100);
            VisionPipeline importedLlmDraftPipeline = VisionPipelineStorage.Load(shellHost.SelectedRecipeNameForTest, "LLM_Draft_Manager");
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "LLM_Draft_Manager", StringComparison.OrdinalIgnoreCase)
                || importedLlmDraftPipeline.Steps.Count != 2
                || string.Equals(importedLlmDraftPipeline.Steps[0].Parameters["TemplatePath"], llmDependencyPath, StringComparison.OrdinalIgnoreCase)
                || !File.Exists(importedLlmDraftPipeline.Steps[0].Parameters["TemplatePath"])
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDependencyReport,
                    "Dependency copy report",
                    "의존 파일 복사 보고서")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDependencyReport,
                    "Copied:",
                    "복사됨:")
                || !ContainsAny(
                    shellHost.RecipeCommands.LlmXmlDraftDependencyReport,
                    "Summary: detected=1, copied=1, missing=0",
                    "요약: 감지=1, 복사=1, 누락=0"))
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft import did not copy dependencies and activate the imported pipeline. "
                    + $"Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', "
                    + $"Validation='{shellHost.RecipeCommands.LlmXmlDraftValidationReport}', "
                    + $"Dependencies='{shellHost.RecipeCommands.LlmXmlDraftDependencyReport}'");
            }

            shellHost.RecipeCommands.SelectedPipelineOption = importedPipelineOption;
            shellHost.RecipeCommands.ActivatePipelineCommand.Execute(null);
            Pump(80);
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager", StringComparison.OrdinalIgnoreCase)
                || shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count != 2)
            {
                throw new InvalidOperationException(
                    "Recipe manager LLM XML draft smoke did not restore the imported pipeline for export. "
                    + $"Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', Steps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
            }

            string exportMessage = string.Empty;
            if (!shellHost.RecipeCommands.ExportActivePipelineXmlToPath(exportPath)
                || !VisionPipelineStorage.TryLoadFromFile(exportPath, out VisionPipeline exportedPipeline, out exportMessage)
                || !string.Equals(exportedPipeline.Name, "Imported_Manager", StringComparison.OrdinalIgnoreCase)
                || exportedPipeline.Steps.Count != 2)
            {
                throw new InvalidOperationException(
                    "Recipe manager export XML did not write a loadable active pipeline. "
                    + $"Path='{exportPath}', Message='{exportMessage}'");
            }

            if (shellHost.NativePreviewRunCount != beforeRecipeManagerRuns
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Recipe manager duplicate/import/export must not auto-open a tool or run Preview. "
                    + $"RunsBefore={beforeRecipeManagerRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            string deleteProbeRecipe = "Smoke_RecipeDelete_" + Guid.NewGuid().ToString("N");
            RecipeWorkspaceService.EnsureVisionWorkspace(deleteProbeRecipe);
            if (!RecipeWorkspaceService.GetRecipeNames().Contains(deleteProbeRecipe, StringComparer.OrdinalIgnoreCase)
                || !RecipeWorkspaceService.DeleteVisionWorkspace(deleteProbeRecipe)
                || RecipeWorkspaceService.GetRecipeNames().Contains(deleteProbeRecipe, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Recipe workspace delete service did not remove " + deleteProbeRecipe);
            }

            string deletePipelineProbeRecipe = "Smoke_PipelineDelete_" + Guid.NewGuid().ToString("N");
            RecipeWorkspaceService.EnsureVisionWorkspace(deletePipelineProbeRecipe);
            VisionPipelineStorage.Save(deletePipelineProbeRecipe, CreateRecipeContextSmokePipeline("Delete_A", 1));
            VisionPipelineStorage.Save(deletePipelineProbeRecipe, CreateRecipeContextSmokePipeline("Delete_B", 1));
            VisionPipelineStorage.SaveActivePipelineName(deletePipelineProbeRecipe, "Delete_B");
            if (!VisionPipelineStorage.TryDeletePipeline(deletePipelineProbeRecipe, "Delete_B", out string fallbackPipelineName, out string deletePipelineMessage)
                || RecipeWorkspaceService.GetVisionPipelineNames(deletePipelineProbeRecipe).Contains("Delete_B", StringComparer.OrdinalIgnoreCase)
                || !string.Equals(VisionPipelineStorage.LoadActivePipelineName(deletePipelineProbeRecipe, "Pipeline"), fallbackPipelineName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Pipeline delete storage did not remove a pipeline and move active marker to fallback. "
                    + $"Fallback='{fallbackPipelineName}', Message='{deletePipelineMessage}'");
            }

            shellHost.SelectRecipeForTest(recipeB);
            Pump(60);
            AssertRecipeContext(shellHost, recipeB, "Inspection_B");

            int beforeCreateRuns = shellHost.NativePreviewRunCount;
            shellHost.CreateRecipeForTest();
            Pump(80);
            string createdRecipe = shellHost.SelectedRecipeNameForTest;
            if (string.IsNullOrWhiteSpace(createdRecipe)
                || !createdRecipe.StartsWith("Recipe_", StringComparison.Ordinal)
                || !shellHost.RecipeOptionsForTest.Contains(createdRecipe, StringComparer.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveRecipeContextNameForTest, createdRecipe, StringComparison.Ordinal)
                || !File.Exists(shellHost.ActiveRecipeContextSourcePathForTest)
                || shellHost.NativePreviewRunCount != beforeCreateRuns
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Create Recipe command did not create and switch to a recipe without tool/preview side effects. "
                    + $"Created='{createdRecipe}', Active={shellHost.ActiveRecipeContextNameForTest}, "
                    + $"Source='{shellHost.ActiveRecipeContextSourcePathForTest}', Options='{string.Join(", ", shellHost.RecipeOptionsForTest)}', "
                    + $"RunsBefore={beforeCreateRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            shellHost.SelectRecipeForTest(duplicatedManagedRecipe);
            Pump(80);
            CleanupTransientRecipeWorkspaces(duplicatedManagedRecipe);
            shellHost.RecipeCommands.RefreshOptions();
            Pump(80);
            if (!string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, "Imported_Manager", StringComparison.OrdinalIgnoreCase)
                || shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count != 2)
            {
                throw new InvalidOperationException(
                    "Recipe manager final screenshot state did not return to the imported pipeline with visible preview steps. "
                    + $"Recipe='{shellHost.SelectedRecipeNameForTest}', Active='{shellHost.ActiveRecipeContextPipelineNameForTest}', "
                    + $"Steps={shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.Count}");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe/language controls",
                "HostRecipeContext",
                "HostRecipeSelector",
                "HostRecipeManagerButton");
            ToggleButton? recipeManagerButton = FindNamedVisualChild<ToggleButton>(shellHost, "btnHostRecipeManager");
            if (recipeManagerButton == null)
            {
                throw new InvalidOperationException("Recipe manager button was not found.");
            }

            recipeManagerButton.IsChecked = true;
            Pump(80);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager pipeline tab",
                "HostRecipeManagerPanel",
                "HostRecipeManagerWorkbenchHeader",
                "HostRecipeManagerWorkbenchGrid",
                "HostRecipeManagerLibraryPane",
                "HostRecipeFilterTextBox",
                "HostRecipeManagerList",
                "HostRecipeDetailPanel",
                "HostRecipeDetailText",
                "HostRecipePipelineHeaderStepFlow",
                "HostRecipeGuidedSetupStrip",
                "HostRecipeGuidedNextActionButton",
                "HostRecipeDetailTabs",
                "HostRecipePipelineTab",
                "HostRecipePipelineManagerList",
                "HostRecipePipelineNameEditor",
                "HostRecipePipelineEditValidation",
                "HostRecipePipelineActivateButton",
                "HostRecipePipelineDuplicateButton",
                "HostRecipePipelineRenameButton",
                "HostRecipePipelineDeleteButton",
                "HostRecipeSampleSelector",
                "HostRecipeDuplicateFromSampleButton",
                "HostRecipePipelineReviewPanel",
                "HostRecipePipelineReviewTab",
                "HostRecipePipelineReportTab",
                "HostRecipeOperatorDecisionBoard",
                "HostRecipeOperatorDecisionXmlCard",
                "HostRecipeOperatorDecisionSampleCard",
                "HostRecipeOperatorDecisionPairCard",
                "HostRecipeOperatorDecisionNextAction",
                "HostRecipeSampleMatrixPanel",
                "HostRecipeSampleMatrixSummary",
                "HostRecipeSampleMatrixList",
                "HostRecipeSelectedSampleMatrixReview",
                "HostRecipeRunCatalogBenchmarkCompactButton",
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
                "HostRecipeNameEditor",
                "HostRecipeEditValidation",
                "HostRecipeCreateNamedButton",
                "HostRecipeDuplicateButton",
                "HostRecipeRenameButton",
                "HostRecipeDeleteButton",
                "HostRecipeImportXmlButton",
                "HostRecipeExportXmlButton",
                "HostRecipeManagerTitleBar",
                "HostRecipeManagerCloseButton");
            if (!shellHost.RecipeCommands.SampleMatrixRows.Any(row => row != null && !row.IsPlaceholder))
            {
                throw new InvalidOperationException("Recipe sample matrix did not expose any runnable sample rows.");
            }

            if (!ContainsAny(shellHost.RecipeCommands.SampleMatrixSummaryText, "Rows", "행"))
            {
                throw new InvalidOperationException(
                    "Recipe sample matrix summary did not expose row count. "
                    + $"Text='{shellHost.RecipeCommands.SampleMatrixSummaryText}'");
            }

            List<OpenVisionRecipeSampleMatrixRow> matrixRows = shellHost.RecipeCommands.SampleMatrixRows
                .Where(row => row != null && !row.IsPlaceholder)
                .ToList();
            OpenVisionRecipeSampleMatrixRow firstMatrixRow = matrixRows[0];
            OpenVisionRecipeSampleMatrixRow? secondMatrixRow = matrixRows.Skip(1).FirstOrDefault();
            List<OpenVisionRecipePairSampleRunSummary> injectedPairResults = new List<OpenVisionRecipePairSampleRunSummary>
            {
                OpenVisionRecipePairSampleRunSummary.CreateForTest(
                    firstMatrixRow.Role,
                    firstMatrixRow.SampleName,
                    secondMatrixRow == null ? "NG" : "OK",
                    secondMatrixRow != null,
                    "ScoreMax=0.95",
                    string.Empty,
                    secondMatrixRow == null ? "01 Match [NG]" : string.Empty)
            };
            if (secondMatrixRow != null)
            {
                injectedPairResults.Add(OpenVisionRecipePairSampleRunSummary.CreateForTest(
                    secondMatrixRow.Role,
                    secondMatrixRow.SampleName,
                    "NG",
                    false,
                    "ScoreMax=0.12",
                    "Injected matrix failure",
                    "02 Match [NG]"));
            }

            shellHost.RecipeCommands.SetPairRunSummaryForTest(injectedPairResults);
            Pump(40);
            if (!shellHost.RecipeCommands.SampleMatrixRows.Any(row => row != null && row.HasResult && !row.Success && row.ResultBadgeText == "NG"))
            {
                throw new InvalidOperationException("Recipe sample matrix did not reflect injected NG pair result.");
            }

            if (!ContainsAny(shellHost.RecipeCommands.SelectedSampleMatrixReviewText, "Failed step", "실패 Step"))
            {
                throw new InvalidOperationException(
                    "Recipe sample matrix selected review did not expose failed-step review text. "
                    + $"Text='{shellHost.RecipeCommands.SelectedSampleMatrixReviewText}'");
            }

            if (!shellHost.RecipeCommands.RunCatalogBenchmarkCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe catalog benchmark command was not executable for the current pipeline.");
            }

            shellHost.RecipeCommands.SetCatalogBenchmarkSummaryForTest(new[]
            {
                new VisionPipelineBatchSampleRunResult
                {
                    SampleName = "Product_Battery_CellVentAlignment_Good",
                    Status = "OK",
                    Success = true,
                    TotalMilliseconds = 12.3,
                    Message = "DistanceMmAvg min 0.20 max 0.26"
                },
                new VisionPipelineBatchSampleRunResult
                {
                    SampleName = "Product_Battery_CellVentAlignment_Bad",
                    Status = "NG",
                    Success = false,
                    TotalMilliseconds = 14.1,
                    FailedStep = "02 Match [NG]",
                    Message = "DistanceMmAvg max 0.12"
                }
            });
            Pump(40);
            if (!ContainsAny(shellHost.RecipeCommands.CatalogBenchmarkSummaryText, "Catalog", "카탈로그")
                || !ContainsAny(shellHost.RecipeCommands.CatalogBenchmarkDetailText, "Product", "Product")
                || !ContainsAny(shellHost.RecipeCommands.CatalogBenchmarkDetailText, "02 Match [NG]", "02 Match [NG]"))
            {
                throw new InvalidOperationException(
                    "Recipe catalog benchmark panel did not expose catalog summary, Product sample scope, and failure step. "
                    + $"Summary='{shellHost.RecipeCommands.CatalogBenchmarkSummaryText}', Detail='{shellHost.RecipeCommands.CatalogBenchmarkDetailText}'");
            }

            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeCatalogBenchmarkPanel",
                outputPath,
                "recipe-catalog-benchmark-panel.png");

            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeSampleMatrixPanel",
                outputPath,
                "recipe-sample-matrix-panel.png");

            shellHost.RecipeCommands.SelectedPipelinePreviewStep =
                shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps.FirstOrDefault();
            Pump(40);
            if (!ContainsAny(shellHost.RecipeCommands.FailureReviewText, "Compare:", "비교:")
                || !ContainsAny(shellHost.RecipeCommands.FailureReviewText, "Rerun", "재검사"))
            {
                throw new InvalidOperationException(
                    "Recipe manager failed-step rerun/comparison review text was not actionable. "
                    + $"Text='{shellHost.RecipeCommands.FailureReviewText}'");
            }
            TabItem? reportTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePipelineReport");
            if (reportTab == null)
            {
                throw new InvalidOperationException("Recipe manager report sub-tab was not found.");
            }

            reportTab.IsSelected = true;
            Pump(40);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager report sub-tab",
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
                    "Recipe manager operator validation checklist was incomplete. "
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
                    "Recipe manager operator result channels were incomplete. "
                    + $"Rows='{string.Join(" | ", resultChannels?.Select(row => row.DisplayText) ?? Array.Empty<string>())}'");
            }

            if (resultChannelBoard == null
                || resultChannelBoard.Count < 5
                || !resultChannelBoard.Any(row => row.ChannelText.Contains("Inspection.Status", StringComparison.OrdinalIgnoreCase))
                || !resultChannelBoard.Any(row => row.ChannelText.Contains("Inspection.Evidence", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Recipe manager operator result channel board was incomplete. "
                    + $"Rows='{string.Join(" | ", resultChannelBoard?.Select(row => row.DisplayText) ?? Array.Empty<string>())}'");
            }

            if (!ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "OpenVisionLab")
                || !ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "Validation checklist", "검증 체크리스트")
                || !ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "Judgement outputs", "판정 출력 정의")
                || !ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "Inspection.Status")
                || !ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "Good/Bad")
                || !ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportText, "Next action", "다음 작업"))
            {
                throw new InvalidOperationException(
                    "Recipe manager operator handoff report was incomplete. "
                    + $"Text='{shellHost.RecipeCommands.OperatorHandoffReportText}'");
            }
            if (!shellHost.RecipeCommands.CopyOperatorHandoffReportCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Recipe manager operator handoff report copy command was disabled.");
            }

            shellHost.RecipeCommands.CopyOperatorHandoffReportCommand.Execute(null);
            Pump(40);
            if (!ContainsAny(shellHost.RecipeCommands.OperatorHandoffReportStatusText, "copied", "복사"))
            {
                throw new InvalidOperationException(
                    "Recipe manager operator handoff report copy command did not report success. "
                    + $"Status='{shellHost.RecipeCommands.OperatorHandoffReportStatusText}'");
            }
            TabItem? runHistoryTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePipelineRunHistory");
            if (runHistoryTab == null)
            {
                throw new InvalidOperationException("Recipe manager run history sub-tab was not found.");
            }

            runHistoryTab.IsSelected = true;
            Pump(40);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager pipeline run history sub-tab",
                "HostRecipePipelineRunHistoryTab",
                "HostRecipeRecentBatchRunList",
                "HostRecipeRecentBatchRunSampleList",
                "HostRecipeRecentBatchRunComparisonPanel",
                "HostRecipeBenchmarkBaselineRunSelector",
                "HostRecipeBenchmarkBaselineRunCombo",
                "HostRecipeRecentBatchRunComparisonSummary",
                "HostRecipeRecentBatchRunComparisonList",
                "HostRecipeSelectedRunComparisonReview",
                "HostRecipeRunHistoryFailureActionPanel",
                "HostRecipeRunHistoryFocusFailureStepButton",
                "HostRecipeRunHistoryLoadSampleInputButton",
                "HostRecipeRunHistoryViewInputButton",
                "HostRecipeRunHistoryViewOutputButton",
                "HostRecipeCopySelectedRunReviewButton",
                "HostRecipeSelectedRunReview");
            if (shellHost.RecipeCommands.RecentBatchRunComparisonRows == null
                || shellHost.RecipeCommands.RecentBatchRunComparisonRows.Count == 0
                || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText)
                || string.IsNullOrWhiteSpace(shellHost.RecipeCommands.SelectedRecentBatchRunComparisonReviewText))
            {
                throw new InvalidOperationException(
                    "Recipe manager run history did not expose benchmark diff rows and review text. "
                    + $"Summary='{shellHost.RecipeCommands.RecentBatchRunComparisonSummaryText}', "
                    + $"Review='{shellHost.RecipeCommands.SelectedRecentBatchRunComparisonReviewText}'");
            }

            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeRecentBatchRunComparisonPanel",
                outputPath,
                "recipe-benchmark-diff-panel.png");

            bool hasSavedRunSelection = shellHost.RecipeCommands.SelectedRecentBatchRunOption != null
                && !string.IsNullOrWhiteSpace(shellHost.RecipeCommands.SelectedRecentBatchRunOption.SummaryPath);
            bool canCopySelectedRunReview = shellHost.RecipeCommands.CopySelectedRecentBatchRunReviewCommand.CanExecute(null);
            if (hasSavedRunSelection && !canCopySelectedRunReview)
            {
                throw new InvalidOperationException("Recipe manager selected run review copy command was disabled for a saved run.");
            }

            if (canCopySelectedRunReview)
            {
                shellHost.RecipeCommands.CopySelectedRecentBatchRunReviewCommand.Execute(null);
                Pump(40);
                if (!ContainsAny(shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText, "copied", "복사"))
                {
                    throw new InvalidOperationException(
                        "Recipe manager selected run review copy command did not report success. "
                        + $"Status='{shellHost.RecipeCommands.SelectedRecentBatchRunReviewCopyStatusText}'");
                }
            }

            TabItem? xmlStepsTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePipelineXmlSteps");
            if (xmlStepsTab == null)
            {
                throw new InvalidOperationException("Recipe manager XML/steps sub-tab was not found.");
            }

            xmlStepsTab.IsSelected = true;
            Pump(40);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager pipeline XML/steps sub-tab",
                "HostRecipePipelineXmlStepsTab",
                "HostRecipePipelineInlineValidationReport",
                "HostRecipePipelineValidationIssueList",
                "HostRecipePipelineStepComparisonGrid",
                "HostRecipePipelineStepFlowFocus",
                "HostRecipePipelineStepFlowReview",
                "HostRecipePipelineStepFlowSlots",
                "HostRecipePreviousPipelineStepButton",
                "HostRecipeNextPipelineStepButton",
                "HostRecipeBranchOutputComparisonPanel",
                "HostRecipeBranchOutputComparisonText",
                "HostRecipeBranchOutputComparisonList",
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
                "HostRecipeLoadSelectedStepParametersButton",
                "HostRecipeApplySelectedStepParametersButton",
                "HostRecipeSelectedStepEditStatus",
                "HostRecipeCorrectedOutputReviewPanel",
                "HostRecipeCorrectedOutputReviewText",
                "HostRecipeCorrectedOutputViewButton",
                "HostRecipeCorrectedOutputRerunButton");
            shellHost.RecipeCommands.SelectedPipelinePreviewStep = shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps[0];
            Pump(40);
            if (!shellHost.RecipeCommands.CurrentPipelineStepText.Contains("Imported_Manager_Step_1", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.NextPipelineStepText.Contains("Imported_Manager_Step_2", StringComparison.OrdinalIgnoreCase)
                || !shellHost.RecipeCommands.PipelineStepFlowReviewText.Contains("Imported_Manager_Preview_1", StringComparison.OrdinalIgnoreCase)
                || !ContainsAny(shellHost.RecipeCommands.BranchOutputComparisonText, "output consumers 1", "출력 소비 Step 1")
                || !shellHost.RecipeCommands.BranchOutputComparisonRows.Any(row => row.Route.Contains("Imported_Manager_Preview_1", StringComparison.OrdinalIgnoreCase))
                || !shellHost.RecipeCommands.SelectNextPipelinePreviewStepCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Recipe manager step flow focus did not expose the selected step and next step. "
                    + $"Current='{shellHost.RecipeCommands.CurrentPipelineStepText}', "
                    + $"Next='{shellHost.RecipeCommands.NextPipelineStepText}', "
                    + $"Review='{shellHost.RecipeCommands.PipelineStepFlowReviewText}', "
                    + $"Branch='{shellHost.RecipeCommands.BranchOutputComparisonText}'");
            }

            string selectedStepOperatorContext = shellHost.RecipeCommands.PipelineSelectedStepOperatorContextText;
            if (string.IsNullOrWhiteSpace(selectedStepOperatorContext)
                || !ContainsAny(selectedStepOperatorContext, "Selected step", "선택 Step")
                || !ContainsAny(selectedStepOperatorContext, "Next", "다음"))
            {
                throw new InvalidOperationException(
                    "Recipe manager step flow focus did not expose selected-step operator context. "
                    + $"Context='{selectedStepOperatorContext}'");
            }

            int beforeStepFlowNavigationRuns = shellHost.NativePreviewRunCount;
            shellHost.RecipeCommands.SelectNextPipelinePreviewStepCommand.Execute(null);
            Pump(40);
            if (shellHost.RecipeCommands.SelectedPipelinePreviewStep?.Index != 2
                || !shellHost.RecipeCommands.PreviousPipelineStepText.Contains("Imported_Manager_Step_1", StringComparison.OrdinalIgnoreCase)
                || shellHost.NativePreviewRunCount != beforeStepFlowNavigationRuns)
            {
                throw new InvalidOperationException(
                    "Recipe manager next-step flow navigation did not select step 2 without running Preview. "
                    + $"Selected={shellHost.RecipeCommands.SelectedPipelinePreviewStep?.Index}, "
                    + $"Previous='{shellHost.RecipeCommands.PreviousPipelineStepText}', "
                    + $"RunsBefore={beforeStepFlowNavigationRuns}, RunsAfter={shellHost.NativePreviewRunCount}");
            }

            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeManagerPanel",
                outputPath,
                "recipe-manager-panel.png");
            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeBranchOutputComparisonPanel",
                outputPath,
                "recipe-branch-output-comparison-panel.png");

            TabItem? llmXmlTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipeLlmXml");
            if (llmXmlTab == null)
            {
                throw new InvalidOperationException("Recipe manager LLM XML tab was not found.");
            }

            llmXmlTab.IsSelected = true;
            Pump(40);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager LLM XML tab",
                "HostRecipeLlmXmlTab",
                "HostRecipeLlmAssistantPanel",
                "HostRecipeLlmTemplateSelector",
                "HostRecipeLlmGoalText",
                "HostRecipeLlmDetectionPointsText",
                "HostRecipeLlmResultChannelContract",
                "HostRecipeBuildLlmPromptButton",
                "HostRecipeCopyLlmPromptButton",
                "HostRecipeCreateLlmTemplateXmlButton",
                "HostRecipeRefreshLlmDraftReviewButton",
                "HostRecipeLlmPromptPreview",
                "HostRecipeLlmXmlDraftPanel",
                "HostRecipeCopyLlmReviewBundleButton",
                "HostRecipePasteLlmXmlDraftButton",
                "HostRecipeLlmXmlDraftText",
                "HostRecipeLoadLlmXmlDraftButton",
                "HostRecipeValidateLlmXmlDraftButton",
                "HostRecipeImportLlmXmlDraftButton",
                "HostRecipeLlmReferenceImagePath",
                "HostRecipeUseSampleReferenceButton",
                "HostRecipeLlmDraftValidationReport",
                "HostRecipeLlmDependencyReport",
                "HostRecipeLlmDependencyPathList",
                "HostRecipeLlmDraftReviewReport",
                "HostRecipeLlmDiffReport",
                "HostRecipeLlmValidationReport",
                "HostRecipeLlmValidationIssueList");
            TabItem? previewTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePreview");
            if (previewTab == null)
            {
                throw new InvalidOperationException("Recipe manager Preview tab was not found.");
            }

            previewTab.IsSelected = true;
            Pump(40);
            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager Preview tab",
                "HostRecipePreviewTab",
                "HostRecipePipelinePreviewStepList");
            TabItem? pipelineTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePipeline");
            if (pipelineTab == null)
            {
                throw new InvalidOperationException("Recipe manager Pipeline tab was not found.");
            }

            pipelineTab.IsSelected = true;
            Pump(40);
            Button? loadSelectedStepParametersButton = FindVisualChildren<Button>(shellHost)
                .FirstOrDefault(button => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(button),
                    "HostRecipeLoadSelectedStepParametersButton",
                    StringComparison.Ordinal));
            if (loadSelectedStepParametersButton?.Command == null
                || !loadSelectedStepParametersButton.Command.CanExecute(loadSelectedStepParametersButton.CommandParameter))
            {
                throw new InvalidOperationException("Selected step parameter load command was not available.");
            }

            int beforeStepParameterApplyRuns = shellHost.NativePreviewRunCount;
            loadSelectedStepParametersButton.Command.Execute(loadSelectedStepParametersButton.CommandParameter);
            Pump(180);
            if (shellHost.RecipeCommands.SelectedStepEditObject == null
                || !shellHost.RecipeCommands.ApplySelectedStepParametersCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Selected step parameters did not load into the recipe PropertyGrid edit object.");
            }

            shellHost.RecipeCommands.ApplySelectedStepParametersCommand.Execute(null);
            Pump(260);
            if (!ContainsAny(shellHost.RecipeCommands.CorrectedOutputReviewText, "corrected output", "수정")
                || !ContainsAny(shellHost.RecipeCommands.CorrectedOutputReviewText, "Rerun", "재검사"))
            {
                throw new InvalidOperationException(
                    "Recipe manager corrected-output review did not update after XML apply. "
                    + $"Text='{shellHost.RecipeCommands.CorrectedOutputReviewText}'");
            }

            if (shellHost.NativePreviewRunCount != beforeStepParameterApplyRuns)
            {
                throw new InvalidOperationException(
                    "Selected step parameter XML apply triggered preview/run. "
                    + $"RunsBefore={beforeStepParameterApplyRuns}, RunsAfter={shellHost.NativePreviewRunCount}");
            }

            AssertVisibleTextContains(
                shellHost,
                "WPF selected step XML apply status",
                "XML 반영 완료");
            AssertHostedPropertyGridRowsRendered(shellHost, "WPF selected step PropertyGrid");
            Point recipeManagerOffsetBefore = shellHost.RecipeManagerPanelOffsetForTest;
            if (!shellHost.MoveRecipeManagerPanelForTest(-220D, 16D))
            {
                throw new InvalidOperationException("Recipe manager panel did not move through the title-bar move path.");
            }

            Pump(40);
            Point recipeManagerOffsetAfter = shellHost.RecipeManagerPanelOffsetForTest;
            if (recipeManagerOffsetAfter.X >= recipeManagerOffsetBefore.X - 10D
                || recipeManagerOffsetAfter.Y <= recipeManagerOffsetBefore.Y)
            {
                throw new InvalidOperationException(
                    "Recipe manager panel movement was too small. "
                    + $"Before={recipeManagerOffsetBefore.X:0.0},{recipeManagerOffsetBefore.Y:0.0}; "
                    + $"After={recipeManagerOffsetAfter.X:0.0},{recipeManagerOffsetAfter.Y:0.0}");
            }

            AssertVisibleTextContains(shellHost, "WPF recipe/language scope hint", "범위:");
            xmlStepsTab.IsSelected = true;
            shellHost.RecipeCommands.SelectedPipelinePreviewStep = shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps[0];
            Pump(80);
            FrameworkElement? branchOutputPanel = FindVisualChildren<FrameworkElement>(shellHost)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "HostRecipeBranchOutputComparisonPanel",
                    StringComparison.Ordinal));
            if (branchOutputPanel != null)
            {
                branchOutputPanel.BringIntoView();
                branchOutputPanel.UpdateLayout();
                Pump(80);
            }
        }, captureFloatingToolWindow: false, captureScreen: true);
    }

    private static CaptureResult CaptureShellHostRecipeMultiBranchComparison(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost(
            "Smoke_WpfShellHostRecipeMultiBranchComparison",
            seedMainLayer: true);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            ToggleButton? recipeManagerButton = FindNamedVisualChild<ToggleButton>(shellHost, "btnHostRecipeManager");
            if (recipeManagerButton == null)
            {
                throw new InvalidOperationException("Recipe manager button was not found.");
            }

            recipeManagerButton.IsChecked = true;
            Pump(80);

            string pipelinePath = Path.Combine(
                "docs",
                "samples",
                "Contour_AllSymbolsAndFaint_LLM.pipeline.xml");
            if (!File.Exists(pipelinePath)
                || !shellHost.RecipeCommands.ImportPipelineXmlFromPath(pipelinePath))
            {
                throw new InvalidOperationException("Could not import the actual 3+ branch pipeline: " + pipelinePath);
            }

            Pump(80);
            TabItem? xmlStepsTab = FindNamedVisualChild<TabItem>(shellHost, "tabRecipePipelineXmlSteps");
            if (xmlStepsTab == null)
            {
                throw new InvalidOperationException("Recipe manager XML/steps sub-tab was not found.");
            }

            xmlStepsTab.IsSelected = true;
            Pump(60);

            OpenVisionRecipePipelineStepPreview? fanOutStep = shellHost.RecipeCommands.SelectedRecipeSummary.PipelinePreviewSteps
                .FirstOrDefault(step =>
                    string.Equals(step.InputLayer, "Main", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(step.OutputLayer, "TextSymbol_Binary", StringComparison.OrdinalIgnoreCase));
            if (fanOutStep == null)
            {
                throw new InvalidOperationException("The actual 3+ branch pipeline did not expose the expected Main fan-out step.");
            }

            shellHost.RecipeCommands.SelectedPipelinePreviewStep = fanOutStep;
            Pump(80);

            IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> rows = shellHost.RecipeCommands.BranchOutputComparisonRows;
            string rowsText = string.Join(" | ", rows.Select(row => row.Status + ": " + row.Route));
            int sameInputRows = rows.Count(row => row.Status.Contains("같은", StringComparison.OrdinalIgnoreCase)
                || row.Status.Contains("Same input", StringComparison.OrdinalIgnoreCase));
            bool hasOutputConsumer = rows.Any(row =>
                row.Route.Contains("TextSymbol_Binary -> TextSymbol_Clean", StringComparison.OrdinalIgnoreCase));
            if (sameInputRows < 3 || !hasOutputConsumer)
            {
                throw new InvalidOperationException(
                    "Branch/output comparison did not expose the actual 3+ fan-out candidates. "
                    + $"SameInputRows={sameInputRows}, Rows='{rowsText}'");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF recipe manager actual 3+ branch comparison",
                "HostRecipeManagerPanel",
                "HostRecipePipelineXmlStepsTab",
                "HostRecipePipelineInlinePreviewStepList",
                "HostRecipeBranchOutputComparisonPanel",
                "HostRecipeBranchOutputComparisonText",
                "HostRecipeBranchOutputComparisonList");

            FrameworkElement? branchOutputPanel = FindVisualChildren<FrameworkElement>(shellHost)
                .FirstOrDefault(item => string.Equals(
                    AutomationProperties.GetAutomationId(item),
                    "HostRecipeBranchOutputComparisonPanel",
                    StringComparison.Ordinal));
            if (branchOutputPanel != null)
            {
                branchOutputPanel.BringIntoView();
                branchOutputPanel.UpdateLayout();
                Pump(80);
            }

            SaveVisibleAutomationElementPng(
                shellHost,
                "HostRecipeBranchOutputComparisonPanel",
                outputPath,
                "recipe-multibranch-comparison-panel.png");
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostRecipeLargeLibrary(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        CleanupTransientRecipeWorkspaces();

        string batchId = Guid.NewGuid().ToString("N").Substring(0, 8);
        List<string> recipeNames = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            string category = "Category_" + (i % 10).ToString("00", CultureInfo.InvariantCulture);
            string name = "Smoke_LargeLibrary_" + batchId + "_" + category + "_Very_Long_Product_Recipe_Name_" + i.ToString("000", CultureInfo.InvariantCulture);
            RecipeWorkspaceService.EnsureVisionWorkspace(name);
            VisionPipelineStorage.Save(name, CreateRecipeContextSmokePipeline("LargeLibrary_" + i.ToString("000", CultureInfo.InvariantCulture), 1));
            VisionPipelineStorage.SaveActivePipelineName(name, "LargeLibrary_" + i.ToString("000", CultureInfo.InvariantCulture));
            recipeNames.Add(name);
        }

        string selectedRecipe = recipeNames[0];
        OpenVisionShellHostView shellHost = CreateShellHost(selectedRecipe, seedMainLayer: false);
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                ToggleButton? recipeManagerButton = FindNamedVisualChild<ToggleButton>(shellHost, "btnHostRecipeManager");
                if (recipeManagerButton == null)
                {
                    throw new InvalidOperationException("Recipe manager button was not found.");
                }

                recipeManagerButton.IsChecked = true;
                Pump(80);
                shellHost.RecipeCommands.RecipeFilterText = "Category_07";
                Pump(80);

                int filteredCount = shellHost.RecipeCommands.FilteredRecipeOptions.Count(name =>
                    name.Contains("Smoke_LargeLibrary_" + batchId + "_Category_07", StringComparison.OrdinalIgnoreCase));
                if (filteredCount != 10)
                {
                    throw new InvalidOperationException(
                        "Recipe manager large-library filter did not narrow 100 long recipes to the expected 10. "
                        + $"Filtered={filteredCount}, TotalVisible={shellHost.RecipeCommands.FilteredRecipeOptions.Count}");
                }

                if (!shellHost.RecipeCommands.RecipeLibrarySummaryText.Contains("10/", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager large-library summary did not expose filtered/total count. "
                        + $"Summary='{shellHost.RecipeCommands.RecipeLibrarySummaryText}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "WPF recipe manager large recipe library",
                    "HostRecipeManagerPanel",
                    "HostRecipeLibrarySummaryText",
                    "HostRecipeFilterTextBox",
                    "HostRecipeManagerList");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            foreach (string recipeName in recipeNames)
            {
                RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
            }
        }
    }

    private static CaptureResult CaptureShellHostRecipeLargePipelineList(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        CleanupTransientRecipeWorkspaces();

        string batchId = Guid.NewGuid().ToString("N").Substring(0, 8);
        string recipeName = "Smoke_LargePipelineList_" + batchId;
        RecipeWorkspaceService.EnsureVisionWorkspace(recipeName);
        for (int i = 0; i < 100; i++)
        {
            string group = "Group_" + (i % 10).ToString("00", CultureInfo.InvariantCulture);
            string pipelineName = "LargePipeline_" + batchId + "_" + group + "_Very_Long_Inspection_Pipeline_Name_" + i.ToString("000", CultureInfo.InvariantCulture);
            VisionPipelineStorage.Save(recipeName, CreateRecipeContextSmokePipeline(pipelineName, 1));
            if (i == 0)
            {
                VisionPipelineStorage.SaveActivePipelineName(recipeName, pipelineName);
            }
        }

        OpenVisionShellHostView shellHost = CreateShellHost(recipeName, seedMainLayer: false);
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                ToggleButton? recipeManagerButton = FindNamedVisualChild<ToggleButton>(shellHost, "btnHostRecipeManager");
                if (recipeManagerButton == null)
                {
                    throw new InvalidOperationException("Recipe manager button was not found.");
                }

                recipeManagerButton.IsChecked = true;
                Pump(80);
                shellHost.RecipeCommands.PipelineFilterText = "Group_07";
                Pump(80);

                int filteredCount = shellHost.RecipeCommands.FilteredPipelineOptions.Count(option =>
                    option.PipelineName.Contains("LargePipeline_" + batchId + "_Group_07", StringComparison.OrdinalIgnoreCase));
                if (filteredCount != 10)
                {
                    throw new InvalidOperationException(
                        "Recipe manager large-pipeline filter did not narrow 100 long pipelines to the expected 10. "
                        + $"Filtered={filteredCount}, TotalVisible={shellHost.RecipeCommands.FilteredPipelineOptions.Count}");
                }

                if (!shellHost.RecipeCommands.PipelineListSummaryText.Contains("10/", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Recipe manager large-pipeline summary did not expose filtered/total count. "
                        + $"Summary='{shellHost.RecipeCommands.PipelineListSummaryText}'");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "WPF recipe manager large pipeline list",
                    "HostRecipeManagerPanel",
                    "HostRecipePipelineListSummaryText",
                    "HostRecipePipelineFilterTextBox",
                    "HostRecipePipelineManagerList");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            RecipeWorkspaceService.DeleteVisionWorkspace(recipeName);
        }
    }

    private static CaptureResult CaptureShellHostLayerManagementCommands(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        string loadImagePath = Path.Combine(
            Path.GetTempPath(),
            "OpenVisionLab_layer_load_smoke_" + Guid.NewGuid().ToString("N") + ".png");
        using (Bitmap image = CreateDockingPanelSmokeBitmap(7))
        {
            image.Save(loadImagePath, ImageFormat.Png);
        }

        try
        {
            OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerManagement", seedMainLayer: true);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                AssertHostComboBoxInteraction(shellHost, "cbHostLayer", "Host layer combo");
                if (shellHost.IsWorkspaceLoadImageIntoLayerMenuVisibleForTest)
                {
                    throw new InvalidOperationException("Workspace context menu still exposes the duplicate load-into-layer image command.");
                }

                int previewRunsBefore = shellHost.NativePreviewRunCount;
                string inputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;
                int initialRows = shellHost.HostLayerRowCount;

                string loadedLayer = shellHost.CreateLayerForTest();
                Pump(40);
                if (string.IsNullOrWhiteSpace(loadedLayer)
                    || !shellHost.HasLayerForTest(loadedLayer)
                    || shellHost.HostLayerRowCount <= initialRows
                    || !string.Equals(shellHost.WorkspaceLayerTitle, loadedLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Layer Create command did not create and activate a visible layer. "
                        + $"Created='{loadedLayer}', RowsBefore={initialRows}, RowsAfter={shellHost.HostLayerRowCount}, Workspace='{shellHost.WorkspaceLayerTitle}'");
                }

                if (!shellHost.LoadImageIntoLayerForTest(loadedLayer, loadImagePath))
                {
                    throw new InvalidOperationException("Layer Load Image command failed for " + loadedLayer);
                }

                Pump(40);
                using (Bitmap loaded = shellHost.GetLayerImageCloneForTest(loadedLayer))
                {
                    if (loaded.Width != 512 || loaded.Height != 384)
                    {
                        throw new InvalidOperationException(
                            "Layer Load Image command wrote an unexpected image size. "
                            + $"Layer={loadedLayer}, Size={loaded.Width}x{loaded.Height}");
                    }
                }

                string deleteLayer = shellHost.CreateLayerForTest();
                Pump(30);
                if (!shellHost.DockLayerForTest(deleteLayer))
                {
                    throw new InvalidOperationException("Layer management smoke could not dock the layer before delete: " + deleteLayer);
                }

                Pump(40);
                if (!shellHost.DeleteLayerForTest(deleteLayer))
                {
                    throw new InvalidOperationException("Delete Layer command failed for " + deleteLayer);
                }

                Pump(60);
                if (shellHost.HasLayerForTest(deleteLayer)
                    || shellHost.DockedLayerTitles.IndexOf(deleteLayer, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new InvalidOperationException(
                        "Delete Layer command left a removed layer in the host or docked workspace. "
                        + $"Deleted={deleteLayer}, Docked='{shellHost.DockedLayerTitles}'");
                }

                if (!shellHost.ActivateHostLayerForTest(loadedLayer))
                {
                    throw new InvalidOperationException("Layer management smoke could not reactivate the loaded layer: " + loadedLayer);
                }

                Pump(40);
                if (shellHost.NativePreviewRunCount != previewRunsBefore
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
                    || shellHost.IsActiveWpfToolWindowVisibleForTest
                    || shellHost.IsNativeDocumentActive
                    || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException(
                        "Layer management commands caused tool/route/preview side effects. "
                        + $"RunsBefore={previewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, "
                        + $"InputBefore='{inputRouteBefore}', InputAfter='{shellHost.ActiveNativeRouteInputLayerNameForTest}', "
                        + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
                }

                AssertVisibleAutomationIds(
                    shellHost,
                    "WPF top layer management commands",
                    "HostTopCreateLayerButton",
                    "HostTopLoadImageIntoLayerButton",
                    "HostTopDeleteLayerButton");
                AssertTopLayerIconButtonLayout(shellHost);
                AssertVisibleTextDoesNotContain(shellHost, "WPF shell top header account chrome", "작업자");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            TryDeleteFile(loadImagePath);
        }
    }

    private static CaptureResult CaptureShellHostLayerRenameCommand(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerRename", seedMainLayer: true);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            int previewRunsBefore = shellHost.NativePreviewRunCount;
            string inputRouteBefore = shellHost.ActiveNativeRouteInputLayerNameForTest;

            string createdLayer = shellHost.CreateLayerForTest();
            Pump(40);
            if (string.IsNullOrWhiteSpace(createdLayer) || !shellHost.HasLayerForTest(createdLayer))
            {
                throw new InvalidOperationException("Layer rename smoke could not create an operator layer.");
            }

            if (!shellHost.DockLayerForTest(createdLayer))
            {
                throw new InvalidOperationException("Layer rename smoke could not dock the created layer before rename: " + createdLayer);
            }

            Pump(60);
            const string renamedLayer = "Inspection_Output_A";
            if (!shellHost.RenameLayerForTest(createdLayer, renamedLayer))
            {
                throw new InvalidOperationException($"Layer rename command returned false. Old={createdLayer}, New={renamedLayer}");
            }

            Pump(80);
            if (shellHost.HasLayerForTest(createdLayer)
                || !shellHost.HasLayerForTest(renamedLayer)
                || !string.Equals(shellHost.ActiveHostLayerTitle, renamedLayer, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.SelectedHostLayerTitle, renamedLayer, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.WorkspaceLayerTitle, renamedLayer, StringComparison.OrdinalIgnoreCase)
                || shellHost.DockedLayerTitles.IndexOf(createdLayer, StringComparison.OrdinalIgnoreCase) >= 0
                || shellHost.DockedLayerTitles.IndexOf(renamedLayer, StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException(
                    "Layer rename did not refresh host/docked layer state. "
                    + $"Old={createdLayer}, New={renamedLayer}, Active={shellHost.ActiveHostLayerTitle}, "
                    + $"Selected={shellHost.SelectedHostLayerTitle}, Workspace={shellHost.WorkspaceLayerTitle}, Docked='{shellHost.DockedLayerTitles}'");
            }

            using (Bitmap renamedImage = shellHost.GetLayerImageCloneForTest(renamedLayer))
            {
                if (renamedImage.Width <= 0 || renamedImage.Height <= 0)
                {
                    throw new InvalidOperationException(
                        "Layer rename did not preserve the layer image. "
                        + $"Size={renamedImage.Width}x{renamedImage.Height}");
                }
            }

            if (shellHost.RenameLayerForTest("Main", "Main_Renamed")
                || shellHost.RenameLayerForTest(renamedLayer, "Main"))
            {
                throw new InvalidOperationException("Layer rename allowed Main or duplicate target names.");
            }

            if (shellHost.NativePreviewRunCount != previewRunsBefore
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, inputRouteBefore, StringComparison.Ordinal)
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Layer rename caused tool/route/preview side effects. "
                    + $"RunsBefore={previewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"InputBefore='{inputRouteBefore}', InputAfter='{shellHost.ActiveNativeRouteInputLayerNameForTest}', "
                    + $"ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }

            AssertVisibleAutomationIds(
                shellHost,
                "WPF layer rename detail controls",
                "HostLayerNameCombo",
                "HostTopCreateLayerButton",
                "HostTopLoadImageIntoLayerButton",
                "HostTopDeleteLayerButton");
            AssertHiddenAutomationIds(
                shellHost,
                "WPF top layer rename controls",
                "HostTopLayerNameEditor",
                "HostTopRenameLayerButton");
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleActions(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceSampleActions", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.HasRunnableWorkspaceSampleForTest)
            {
                throw new InvalidOperationException("WPF workspace sample actions did not find a runnable sample catalog item.");
            }

            shellHost.OpenFirstRunnableWorkspaceSampleForTest();
            Pump(60);

            if (!shellHost.CanOpenSamplePipelineForTest || !shellHost.CanOpenSampleFirstStepToolForTest)
            {
                throw new InvalidOperationException("WPF workspace sample action buttons are not executable after sample load.");
            }

            shellHost.OpenSamplePipelineForTest();
            Pump(80);
            if (!shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.PipelineReviewStepCount != shellHost.ActivePipelineStepCountForTest)
            {
                throw new InvalidOperationException(
                    "WPF workspace sample Pipeline action did not open the active sample pipeline review. "
                    + $"Window={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                    + $"ReviewSteps={shellHost.PipelineReviewStepCount}, ActiveSteps={shellHost.ActivePipelineStepCountForTest}");
            }

            if (shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("WPF workspace sample Pipeline action must not run Preview.");
            }

            shellHost.CloseActiveWpfToolWindowForTest();
            Pump(40);

            shellHost.OpenSampleFirstStepToolForTest();
            Pump(80);
            if (!shellHost.IsActiveWpfToolWindowVisibleForTest
                || !string.Equals(shellHost.WorkspaceSampleFirstStepMenuForTest, "Threshold", StringComparison.Ordinal)
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "WPF workspace sample first-step action did not open the expected tool without running Preview. "
                    + $"Window={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                    + $"FirstMenu='{shellHost.WorkspaceSampleFirstStepMenuForTest}', "
                    + $"Preview={shellHost.HasNativePreviewResult}, "
                    + $"Title='{shellHost.ActiveWpfToolWindowTitle}'");
            }
        }, captureFloatingToolWindow: true, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewMetrics(string outputPath)
    {
        const string sampleName = "Public_Blob_Particles_Good";

        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        VisionPipelineSampleCatalogItem sample = FindRunnableCatalogSample(sampleName);
        AssertSampleSourceKind(sample, sampleName, VisionPipelineSampleCatalogSourceKind.Public);
        VisionPipelineSampleCheckResult sampleCheck = VisionPipelineSampleCheckService.RunSampleCheckSafe(sample);
        if (!sampleCheck.Success)
        {
            throw new InvalidOperationException(
                "Workspace sample review metrics baseline sample check failed. "
                + $"Sample={sampleName}, Status={sampleCheck.Status}, Message={sampleCheck.Message}, Metrics={sampleCheck.MetricText}");
        }

        foreach (string metricName in new[] { "ResultCount" })
        {
            if (!sampleCheck.MetricText.Contains(metricName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Workspace sample review metrics baseline is missing expected metric. "
                    + $"Sample={sampleName}, Missing={metricName}, Metrics={sampleCheck.MetricText}");
            }
        }

        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceSampleReviewMetrics", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            (_, int activePipelineStepCount, _) = OpenWorkspaceSamplePipelineReviewForSmoke(
                shellHost,
                sampleName,
                "WPF workspace sample review metrics",
                minStepCount: 2);

            WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), 30000, "Workspace sample pipeline review metric execution");
            Pump(120);
            shellHost.SelectPipelineReviewStepForTest(activePipelineStepCount - 1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
            Pump(80);

            string resultCountMetricText = OpenVisionLanguageService.T("PipelineReview.Metric.ResultCount");
            string reviewText = string.Join(
                " | ",
                shellHost.PipelineReviewValidationStatusText,
                shellHost.PipelineReviewResultSummaryText,
                shellHost.PipelineReviewResultDetailText,
                shellHost.PipelineReviewRunLogText,
                shellHost.PipelineReviewGuideResultDecisionText,
                shellHost.PipelineReviewGuideDetailText);
            if (string.IsNullOrWhiteSpace(shellHost.PipelineReviewValidationStatusText)
                || !shellHost.PipelineReviewResultSummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewRunLogText)
                || !shellHost.PipelineReviewGuideResultDecisionText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewResultDetailText.Contains("Result", StringComparison.OrdinalIgnoreCase)
                || !shellHost.HasPipelineReviewOutputPreview)
            {
                throw new InvalidOperationException(
                    "WPF workspace sample Pipeline Review did not expose OK decision, primary result metric, run log, and output preview. "
                    + $"ReviewText='{reviewText}', OutputPreview={shellHost.HasPipelineReviewOutputPreview}");
            }
        }, captureFloatingToolWindow: true);
    }

    private static CaptureResult CaptureShellHostWorkspaceProductSampleReview(string outputPath)
    {
        const string sampleName = "Product_Display_Particle_Good";

        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        VisionPipelineSampleCatalogItem sample = FindRunnableCatalogSample(sampleName);
        AssertProductDisplayParticleSampleForSmoke(sample, sampleName, expectsFailure: false);

        VisionPipelineSampleCheckResult sampleCheck = VisionPipelineSampleCheckService.RunSampleCheckSafe(sample);
        if (!sampleCheck.Success
            || !sampleCheck.MetricText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Product sample review baseline sample check failed before UI open. "
                + $"Sample={sampleName}, Status={sampleCheck.Status}, Message={sampleCheck.Message}, Metrics={sampleCheck.MetricText}");
        }

        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceProductSampleReview", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            (_, int activePipelineStepCount, _) = OpenWorkspaceSamplePipelineReviewForSmoke(
                shellHost,
                sampleName,
                "WPF workspace Product sample review",
                minStepCount: 2);

            if (!shellHost.WorkspaceSampleWorkflowMetaForTest.Contains(sampleName, StringComparison.Ordinal)
                && !shellHost.WorkspaceSampleWorkflowDetailForTest.Contains("Blob", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Product sample workflow hint did not expose the Product sample or Blob flow after open. "
                    + $"Meta='{shellHost.WorkspaceSampleWorkflowMetaForTest}', Detail='{shellHost.WorkspaceSampleWorkflowDetailForTest}'");
            }

            WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), 30000, "Workspace Product sample Pipeline Review execution");
            Pump(120);
            shellHost.SelectPipelineReviewStepForTest(activePipelineStepCount - 1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
            Pump(80);

            string resultCountMetricText = OpenVisionLanguageService.T("PipelineReview.Metric.ResultCount");
            string reviewText = string.Join(
                " | ",
                shellHost.PipelineReviewValidationStatusText,
                shellHost.PipelineReviewResultSummaryText,
                shellHost.PipelineReviewResultDetailText,
                shellHost.PipelineReviewRunLogText,
                shellHost.PipelineReviewGuideResultDecisionText,
                shellHost.PipelineReviewGuideDetailText,
                shellHost.PipelineReviewGuidePairText,
                shellHost.PipelineReviewGuidePairMetricText,
                shellHost.PipelineReviewGuideChecklistText);
            if (string.IsNullOrWhiteSpace(shellHost.PipelineReviewValidationStatusText)
                || !shellHost.PipelineReviewResultSummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewResultDetailText.Contains(resultCountMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideResultDecisionText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains("Product_Display_Particle_Good", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains("Product_Display_Particle_Many_Bad", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(resultCountMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains(resultCountMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("Pipeline", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("OK 기준 안", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("NG 기준 밖", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("1", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideChecklistText.Contains("Good", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideChecklistText.Contains("Bad", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideChecklistText.Contains("Display_Particle", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideChecklistText.Contains("Product_Display_Particle_Many_Bad", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideChecklistText.Contains(resultCountMetricText, StringComparison.Ordinal)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewRunLogText)
                || !shellHost.HasPipelineReviewOutputPreview
                || shellHost.CanSelectFirstIssuePipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    "WPF workspace Product sample Pipeline Review did not expose OK decision, ResultCount-style detail, concrete Good/Bad PairGroup review habit, run log, and output preview. "
                    + $"ReviewText='{reviewText}', OutputPreview={shellHost.HasPipelineReviewOutputPreview}, FirstIssue={shellHost.CanSelectFirstIssuePipelineReviewStepForTest}");
            }

            AssertPipelineReviewProgressText(
                shellHost,
                expectedOk: activePipelineStepCount,
                expectedNg: 0,
                expectedWait: 0,
                "Workspace Product sample OK review");
        }, captureFloatingToolWindow: true);
    }

    private static CaptureResult CaptureShellHostWorkspaceProductSampleReviewNg(string outputPath)
    {
        const string sampleName = "Product_Display_Particle_Many_Bad";

        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        VisionPipelineSampleCatalogItem sample = FindRunnableCatalogSample(sampleName);
        AssertProductDisplayParticleSampleForSmoke(sample, sampleName, expectsFailure: true);

        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            sampleName,
            "ResultCount",
            "Result",
            "Product display particle",
            "Smoke_WpfShellHostWorkspaceProductSampleReviewNg",
            "WPF workspace Product sample review NG metrics",
            minStepCount: 2,
            expectedSourceKind: VisionPipelineSampleCatalogSourceKind.Product);
    }

    private static CaptureResult CaptureShellHostWorkspaceProductSamplePairOpen(string outputPath)
    {
        const string goodSampleName = "Product_Display_Particle_Good";
        const string badSampleName = "Product_Display_Particle_Many_Bad";

        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        VisionPipelineSampleCatalogItem goodSample = FindRunnableCatalogSample(goodSampleName);
        VisionPipelineSampleCatalogItem badSample = FindRunnableCatalogSample(badSampleName);
        AssertProductDisplayParticleSampleForSmoke(goodSample, goodSampleName, expectsFailure: false);
        AssertProductDisplayParticleSampleForSmoke(badSample, badSampleName, expectsFailure: true);

        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceProductSamplePairOpen", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            (_, int goodPipelineStepCount, _) = OpenWorkspaceSamplePipelineReviewForSmoke(
                shellHost,
                goodSampleName,
                "WPF workspace Product sample pair open",
                minStepCount: 2);

            if (!shellHost.CanOpenPipelineReviewPairSampleForTest
                || !shellHost.PipelineReviewGuidePairActionText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(goodSampleName, StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(badSampleName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Pipeline Review did not expose the explicit NG pair sample action from the Good sample. "
                    + $"CanOpen={shellHost.CanOpenPipelineReviewPairSampleForTest}, "
                    + $"Action='{shellHost.PipelineReviewGuidePairActionText}', Pair='{shellHost.PipelineReviewGuidePairText}'");
            }

            int nativePreviewRunsBefore = shellHost.NativePreviewRunCount;
            if (!shellHost.OpenPipelineReviewPairSampleForTest())
            {
                throw new InvalidOperationException("Pipeline Review pair sample action returned false.");
            }

            Pump(220);
            string activePipelineName = shellHost.ActivePipelineNameForTest;
            int activePipelineStepCount = shellHost.ActivePipelineStepCountForTest;
            if (!activePipelineName.Contains(badSampleName, StringComparison.OrdinalIgnoreCase)
                || activePipelineStepCount != goodPipelineStepCount
                || !string.Equals(shellHost.PipelineReviewRecipeContextPipelineNameForTest, activePipelineName, StringComparison.Ordinal)
                || !shellHost.WorkspaceSampleWorkflowMetaForTest.Contains(badSampleName, StringComparison.Ordinal)
                || !shellHost.CanOpenPipelineReviewPairSampleForTest
                || !shellHost.PipelineReviewGuidePairActionText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(goodSampleName, StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(badSampleName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Pipeline Review pair sample action did not switch the active sample and refresh the Review guide. "
                    + $"Pipeline='{activePipelineName}', Steps={activePipelineStepCount}/{goodPipelineStepCount}, "
                    + $"ReviewPipeline='{shellHost.PipelineReviewRecipeContextPipelineNameForTest}', "
                    + $"WorkflowMeta='{shellHost.WorkspaceSampleWorkflowMetaForTest}', "
                    + $"Action='{shellHost.PipelineReviewGuidePairActionText}', Pair='{shellHost.PipelineReviewGuidePairText}'");
            }

            if (shellHost.NativePreviewRunCount != nativePreviewRunsBefore
                || shellHost.HasNativePreviewResult
                || shellHost.HasPipelineReviewOutputPreview)
            {
                throw new InvalidOperationException(
                    "Pipeline Review pair sample action caused Preview/Review side effects. "
                    + $"RunsBefore={nativePreviewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"NativePreview={shellHost.HasNativePreviewResult}, ReviewOutput={shellHost.HasPipelineReviewOutputPreview}");
            }
        }, captureFloatingToolWindow: true);
    }

    private static void AssertProductDisplayParticleSampleForSmoke(
        VisionPipelineSampleCatalogItem sample,
        string sampleName,
        bool expectsFailure)
    {
        if (sample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.Product
            || sample.ExpectsFailure != expectsFailure
            || !string.Equals(sample.PairGroup, "Display_Particle", StringComparison.OrdinalIgnoreCase)
            || !sample.BaselinePipeline.Contains("Product_Display_Particle_Blob.pipeline.xml", StringComparison.OrdinalIgnoreCase)
            || !sample.ExpectedMetricName.Contains("ResultCount", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Product sample review smoke target is not bound to the expected Display_Particle Product catalog sample. "
                + $"Sample={sample.SampleName}, Requested={sampleName}, Source={sample.CatalogSourceKind}, "
                + $"ExpectsFailure={sample.ExpectsFailure}, ExpectedFailure={expectsFailure}, PairGroup='{sample.PairGroup}', "
                + $"Pipeline='{sample.BaselinePipeline}', Metric='{sample.ExpectedMetricName}'");
        }

        OpenVisionWorkspaceSampleLearnPathOption learnPath = OpenVisionWorkspaceSampleLearnPathOption
            .Create(new[] { sample })
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Product sample review smoke could not create a Learn path option.");
        if (!OpenVisionWorkspaceLearnDocumentService.TryResolveDocumentPath(sample, learnPath, out string learnDocumentPath)
            || !learnDocumentPath.EndsWith(Path.Combine("docs", "learn", "LEARN_PRODUCT_SAMPLES.md"), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Product sample review smoke did not resolve the Product Learn document. "
                + $"Sample={sampleName}, LearnPath={learnPath.Id}, Document='{learnDocumentPath}'");
        }
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Mean_Brightness_Dark_Bad",
            "MeanValueAvg",
            "Mean",
            "Public mean brightness",
            "Smoke_WpfShellHostWorkspaceSampleReviewNgMetrics",
            "WPF workspace sample review NG metrics",
            minStepCount: 1);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewFeatureNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Feature_Card_Wrong_Bad",
            "ScoreMax",
            "Score",
            "Public feature score",
            "Smoke_WpfShellHostWorkspaceSampleReviewFeatureNgMetrics",
            "WPF workspace sample review Feature NG metrics",
            minStepCount: 1);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewLineNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Line_Pins_WidePin_Bad",
            "DistanceMmAvg",
            "Distance",
            "Public line distance",
            "Smoke_WpfShellHostWorkspaceSampleReviewLineNgMetrics",
            "WPF workspace sample review LineGauge NG metrics",
            minStepCount: 1);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewBlobNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Blob_Particles_Sparse_Bad",
            "ResultCount",
            "Result",
            "Public blob particles",
            "Smoke_WpfShellHostWorkspaceSampleReviewBlobNgMetrics",
            "WPF workspace sample review Blob NG metrics",
            minStepCount: 2);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewBentPinNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Contour_Shapes_Missing_Bad",
            "ResultCount",
            "Result",
            "Public contour shapes",
            "Smoke_WpfShellHostWorkspaceSampleReviewBentPinNgMetrics",
            "WPF workspace sample review BentPin NG metrics",
            minStepCount: 1);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewFilmNgMetrics(string outputPath)
    {
        return CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
            outputPath,
            "Public_Threshold_BandPads_Missing_Bad",
            "ResultCount",
            "Result",
            "Public threshold band pads",
            "Smoke_WpfShellHostWorkspaceSampleReviewFilmNgMetrics",
            "WPF workspace sample review Film NG metrics",
            minStepCount: 1);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePipelineReviewControlledNgMetrics(
        string outputPath,
        string sampleName,
        string expectedMetricName,
        string resultDetailKeyword,
        string scenarioLabel,
        string shellHostTitle,
        string scenarioName,
        int minStepCount,
        VisionPipelineSampleCatalogSourceKind expectedSourceKind = VisionPipelineSampleCatalogSourceKind.Public)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        VisionPipelineSampleCatalogItem sample = FindRunnableCatalogSample(sampleName);
        AssertSampleSourceKind(sample, sampleName, expectedSourceKind);
        VisionPipelineSampleCheckResult sampleCheck = VisionPipelineSampleCheckService.RunSampleCheckSafe(sample);
        if (!sample.ExpectsFailure || !sampleCheck.Success)
        {
            throw new InvalidOperationException(
                $"Workspace sample review {scenarioLabel} NG baseline sample check did not confirm the expected-failure sample. "
                + $"Sample={sampleName}, ExpectsFailure={sample.ExpectsFailure}, Status={sampleCheck.Status}, "
                + $"Message={sampleCheck.Message}, Metrics={sampleCheck.MetricText}");
        }

        if (!sampleCheck.MetricText.Contains(expectedMetricName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Workspace sample review {scenarioLabel} NG baseline is missing {expectedMetricName} metric. "
                + $"Sample={sampleName}, Metrics={sampleCheck.MetricText}");
        }

        OpenVisionShellHostView shellHost = CreateShellHost(shellHostTitle, seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            (_, int activePipelineStepCount, _) = OpenWorkspaceSamplePipelineReviewForSmoke(
                shellHost,
                sampleName,
                scenarioName,
                minStepCount);

            WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), 30000, $"Workspace sample pipeline review {scenarioLabel} NG metric execution");
            Pump(120);
            shellHost.SelectPipelineReviewStepForTest(activePipelineStepCount - 1, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
            Pump(80);

            string ngNextAction = OpenVisionLanguageService.T("PipelineReview.Guide.NgNext");
            string fixDetailPrefix = OpenVisionLanguageService.T("PipelineReview.Guide.FixDetailPrefix");
            string parameterLocationPrefix = OpenVisionLanguageService.T("PipelineReview.Guide.ParameterLocationPrefix");
            string triageRerunPair = OpenVisionLanguageService.T("PipelineReview.Guide.TriageRerunPair");
            string metricDisplayKey = "PipelineReview.Metric." + expectedMetricName;
            string metricDisplayName = OpenVisionLanguageService.T(metricDisplayKey);
            bool hasLocalizedMetricDisplayName = !string.IsNullOrWhiteSpace(metricDisplayName)
                && !string.Equals(metricDisplayName, metricDisplayKey, StringComparison.Ordinal);
            string expectedGuideMetricName = hasLocalizedMetricDisplayName ? metricDisplayName : expectedMetricName;
            bool pairMetricHasExpectedResult = shellHost.PipelineReviewGuidePairMetricText.Contains(resultDetailKeyword, StringComparison.OrdinalIgnoreCase)
                || shellHost.PipelineReviewGuidePairMetricText.Contains(expectedGuideMetricName, StringComparison.Ordinal);
            bool resultDetailHasExpectedResult = shellHost.PipelineReviewResultDetailText.Contains(resultDetailKeyword, StringComparison.OrdinalIgnoreCase)
                || shellHost.PipelineReviewResultDetailText.Contains(expectedGuideMetricName, StringComparison.Ordinal);
            string reviewText = string.Join(
                " | ",
                shellHost.PipelineReviewValidationStatusText,
                shellHost.PipelineReviewResultSummaryText,
                shellHost.PipelineReviewResultDetailText,
                shellHost.PipelineReviewRunLogText,
                shellHost.PipelineReviewGuideNextActionText,
                shellHost.PipelineReviewGuideResultDecisionText,
                shellHost.PipelineReviewGuideDetailText,
                shellHost.PipelineReviewGuidePairText,
                shellHost.PipelineReviewGuidePairMetricText,
                shellHost.PipelineReviewGuideParameterFocusText,
                shellHost.PipelineReviewGuideTriageFailureText,
                shellHost.PipelineReviewGuideTriageAdjustmentText,
                shellHost.PipelineReviewGuideTriageRerunText);
            if (string.IsNullOrWhiteSpace(shellHost.PipelineReviewValidationStatusText)
                || !shellHost.PipelineReviewResultSummaryText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewRunLogText)
                || !shellHost.PipelineReviewGuideResultDecisionText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideNextActionText.Contains(ngNextAction, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideNextActionText.Contains(expectedGuideMetricName, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairText.Contains("Good/Bad", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(sampleName, StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairText.Contains(expectedGuideMetricName, StringComparison.Ordinal)
                || !pairMetricHasExpectedResult
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("Pipeline", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("기준 안", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuidePairMetricText.Contains("기준 밖", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || (hasLocalizedMetricDisplayName && !shellHost.PipelineReviewGuideDetailText.Contains(metricDisplayName, StringComparison.Ordinal))
                || !shellHost.PipelineReviewGuideDetailText.Contains(parameterLocationPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains("파라미터 패널", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideParameterFocusText.Contains(parameterLocationPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideParameterFocusText.Contains("파라미터 패널", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideTriageFailureText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideTriageAdjustmentText.Contains(fixDetailPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideTriageRerunText.Contains(triageRerunPair, StringComparison.Ordinal)
                || (hasLocalizedMetricDisplayName && shellHost.PipelineReviewGuideDetailText.Contains(expectedMetricName, StringComparison.OrdinalIgnoreCase))
                || (hasLocalizedMetricDisplayName && shellHost.PipelineReviewGuidePairText.Contains(expectedMetricName, StringComparison.OrdinalIgnoreCase))
                || (hasLocalizedMetricDisplayName && shellHost.PipelineReviewGuidePairMetricText.Contains(expectedMetricName, StringComparison.OrdinalIgnoreCase))
                || !resultDetailHasExpectedResult
                || !shellHost.HasPipelineReviewOutputPreview
                || !shellHost.CanSelectFirstIssuePipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    $"WPF workspace sample Pipeline Review did not expose {scenarioLabel} NG decision, beginner next action, metric detail, run log, output preview, and first issue navigation. "
                    + $"ReviewText='{reviewText}', ExpectedNext='{ngNextAction}', OutputPreview={shellHost.HasPipelineReviewOutputPreview}, FirstIssue={shellHost.CanSelectFirstIssuePipelineReviewStepForTest}");
            }

            AssertPipelineReviewProgressText(
                shellHost,
                expectedOk: Math.Max(0, activePipelineStepCount - 1),
                expectedNg: 1,
                expectedWait: 0,
                $"Workspace sample Pipeline Review {scenarioLabel} NG");

            AssertVisibleAutomationIds(
                GetActiveFloatingToolWindow($"Workspace sample Pipeline Review {scenarioLabel} NG operator focus"),
                $"Workspace sample Pipeline Review {scenarioLabel} NG operator focus",
                "PipelineReviewStepFlowOperatorFocus",
                "PipelineReviewFirstIssueStepButton");
        }, captureFloatingToolWindow: true);
    }

    private static void AssertPipelineReviewProgressText(
        OpenVisionShellHostView shellHost,
        int expectedOk,
        int expectedNg,
        int expectedWait,
        string context)
    {
        string progress = shellHost?.PipelineReviewProgressText ?? string.Empty;
        string expectedCounts = string.Format(
            CultureInfo.CurrentCulture,
            OpenVisionLanguageService.T("PipelineReview.Progress.CountsFormat"),
            expectedOk,
            expectedNg,
            expectedWait);
        if (!progress.Contains(expectedCounts, StringComparison.Ordinal)
            || progress.Contains(OpenVisionLanguageService.T("PipelineReview.Progress.Running"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                context + " did not expose the expected Pipeline Review progress summary. "
                + $"Expected='{expectedCounts}', Actual='{progress}'");
        }
    }

    private static VisionPipelineSampleCatalogItem FindRunnableCatalogSample(string sampleName)
    {
        return VisionPipelineSampleCatalogItem.LoadRunnable()
            .FirstOrDefault(item => string.Equals(item.SampleName, sampleName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Workspace sample review target could not find sample: " + sampleName);
    }

    private static void AssertSampleSourceKind(
        VisionPipelineSampleCatalogItem sample,
        string sampleName,
        VisionPipelineSampleCatalogSourceKind expectedSourceKind)
    {
        if (sample.CatalogSourceKind != expectedSourceKind)
        {
            throw new InvalidOperationException(
                "Workspace sample smoke target is bound to the wrong catalog source. "
                + $"Sample={sample.SampleName}, Requested={sampleName}, Source={sample.CatalogSourceKind}, Expected={expectedSourceKind}");
        }
    }

    private static (string ActivePipelineName, int ActivePipelineStepCount, int NativePreviewRunsBefore) OpenWorkspaceSamplePipelineReviewForSmoke(
        OpenVisionShellHostView shellHost,
        string sampleName,
        string scenarioName,
        int minStepCount)
    {
        int nativePreviewRunsBefore = shellHost.NativePreviewRunCount;
        if (!shellHost.OpenWorkspaceSampleForTest(sampleName))
        {
            throw new InvalidOperationException($"{scenarioName} failed to open runnable sample by name: {sampleName}");
        }

        Pump(80);
        if (!shellHost.CanOpenSamplePipelineForTest || !shellHost.CanOpenSampleFirstStepToolForTest)
        {
            throw new InvalidOperationException(
                scenarioName + " actions were not available after sample load. "
                + $"CanPipeline={shellHost.CanOpenSamplePipelineForTest}, CanFirstStep={shellHost.CanOpenSampleFirstStepToolForTest}");
        }

        string activePipelineName = shellHost.ActivePipelineNameForTest;
        int activePipelineStepCount = shellHost.ActivePipelineStepCountForTest;
        if (!activePipelineName.StartsWith("Sample_", StringComparison.Ordinal)
            || activePipelineStepCount < minStepCount
            || shellHost.NativePreviewRunCount != nativePreviewRunsBefore
            || shellHost.HasNativePreviewResult)
        {
            throw new InvalidOperationException(
                scenarioName + " did not activate a Sample_ pipeline without preview side effects. "
                + $"Pipeline='{activePipelineName}', Steps={activePipelineStepCount}, MinSteps={minStepCount}, "
                + $"RunsBefore={nativePreviewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
        }

        shellHost.OpenSamplePipelineForTest();
        Pump(120);
        if (!shellHost.IsActiveWpfToolWindowVisibleForTest
            || shellHost.PipelineReviewStepCount != activePipelineStepCount
            || !string.Equals(shellHost.PipelineReviewRecipeContextPipelineNameForTest, activePipelineName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                scenarioName + " Pipeline Review did not bind to the active sample pipeline. "
                + $"Window={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                + $"ReviewSteps={shellHost.PipelineReviewStepCount}, ActiveSteps={activePipelineStepCount}, "
                + $"ReviewPipeline='{shellHost.PipelineReviewRecipeContextPipelineNameForTest}', ActivePipeline='{activePipelineName}'");
        }

        if (shellHost.NativePreviewRunCount != nativePreviewRunsBefore || shellHost.HasNativePreviewResult)
        {
            throw new InvalidOperationException(
                scenarioName + " opened Pipeline Review with native Preview side effects. "
                + $"RunsBefore={nativePreviewRunsBefore}, RunsAfter={shellHost.NativePreviewRunCount}, Preview={shellHost.HasNativePreviewResult}");
        }

        return (activePipelineName, activePipelineStepCount, nativePreviewRunsBefore);
    }

    private static CaptureResult CaptureShellHostWorkspaceQuickActions(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceQuickActions", seedMainLayer: false);
        string imagePath = CreateWorkspaceLoadSmokeImageFile();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                if (!shellHost.LoadMainImageFromFileForTest(imagePath))
                {
                    throw new InvalidOperationException("WPF workspace quick action test helper failed to load Main image.");
                }

                Pump(24);
                if (!shellHost.IsWorkspaceMainActionVisibleForTest
                    || !shellHost.CanOpenWorkspaceThresholdToolForTest
                    || !shellHost.CanOpenWorkspaceMatchingToolForTest
                    || !shellHost.CanOpenWorkspaceLineToolForTest)
                {
                    throw new InvalidOperationException(
                        "WPF workspace quick action commands were not ready after loading Main. "
                        + $"ActionVisible={shellHost.IsWorkspaceMainActionVisibleForTest}, "
                        + $"Threshold={shellHost.CanOpenWorkspaceThresholdToolForTest}, "
                        + $"Matching={shellHost.CanOpenWorkspaceMatchingToolForTest}, "
                        + $"Line={shellHost.CanOpenWorkspaceLineToolForTest}");
                }

                AssertWorkspaceQuickActionOpensTool(
                    shellHost,
                    "Threshold quick action",
                    () => shellHost.CanOpenWorkspaceThresholdToolForTest,
                    shellHost.OpenWorkspaceThresholdToolForTest,
                    "ThresholdToolWpfView");

                AssertWorkspaceQuickActionOpensTool(
                    shellHost,
                    "Matching quick action",
                    () => shellHost.CanOpenWorkspaceMatchingToolForTest,
                    shellHost.OpenWorkspaceMatchingToolForTest,
                    "MatchingToolWpfView");

                AssertWorkspaceQuickActionOpensTool(
                    shellHost,
                    "Line quick action",
                    () => shellHost.CanOpenWorkspaceLineToolForTest,
                    shellHost.OpenWorkspaceLineToolForTest,
                    "LineToolWpfView");
            }, captureFloatingToolWindow: true, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
        }
        finally
        {
            TryDeleteFile(imagePath);
        }
    }

    private static void AssertWorkspaceQuickActionOpensTool(
        OpenVisionShellHostView shellHost,
        string actionName,
        Func<bool> canExecute,
        Action execute,
        string expectedDocumentType)
    {
        if (!canExecute())
        {
            throw new InvalidOperationException(actionName + " command was not executable.");
        }

        execute();
        Pump(80);

        if (!shellHost.IsActiveWpfToolWindowVisibleForTest
            || !shellHost.IsNativeDocumentActive
            || !shellHost.ActiveNativeDocumentTypeName.Contains(expectedDocumentType, StringComparison.Ordinal)
            || shellHost.HasNativePreviewResult
            || shellHost.NativePreviewRunCount != 0
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                actionName + " did not open the expected native WPF tool without running Preview. "
                + $"Window={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                + $"Native={shellHost.IsNativeDocumentActive}, "
                + $"Document='{shellHost.ActiveNativeDocumentTypeName}', "
                + $"Preview={shellHost.HasNativePreviewResult}, "
                + $"PreviewRuns={shellHost.NativePreviewRunCount}, "
                + $"Input='{shellHost.ActiveNativeRouteInputLayerNameForTest}'");
        }

        shellHost.CloseActiveWpfToolWindowForTest();
        Pump(40);
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePicker(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace sample picker could not find any runnable catalog samples.");
        }

        OpenVisionWorkspaceSamplePickerViewModel viewModel = new(samples);
        OpenVisionWorkspaceSamplePickerWindow window = new(viewModel);
        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            AssertVisibleAutomationIds(
                window,
                "WPF workspace sample picker",
                "WorkspaceSamplePickerView",
                "WorkspaceSamplePickerSearchBox",
                "WorkspaceSamplePickerCatalogSourceList",
                "WorkspaceSamplePickerCatalogSourceSummary",
                "WorkspaceSamplePickerSampleFocusList",
                "WorkspaceSamplePickerSampleFocusSummary",
                "WorkspaceSamplePickerLearnPathList",
                "WorkspaceSamplePickerLearnPathSummary",
                "WorkspaceSamplePickerList",
                "WorkspaceSamplePickerDetail",
                "WorkspaceSamplePickerSelectedSummary",
                "WorkspaceSamplePickerBenchmarkStrip",
                "WorkspaceSamplePickerPairDecisionQuickWorkflow",
                "WorkspaceSamplePickerLearnModeStrip",
                "WorkspaceSamplePickerOpenLearnDocumentButton",
                "WorkspaceSamplePickerPreviewImage",
                "WorkspaceSamplePickerToolFlow",
                "WorkspaceSamplePickerExpected",
                "WorkspaceSamplePickerOpenButton",
                "WorkspaceSamplePickerCancelButton");

            if (!viewModel.HasSamples || !viewModel.CanSelect || viewModel.SelectedSample == null)
            {
                throw new InvalidOperationException("Workspace sample picker did not select a runnable sample by default.");
            }

            if (!viewModel.HasLearnDocument || !viewModel.OpenLearnDocumentCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not resolve a Learn document for the default sample. "
                    + $"Sample={viewModel.SelectedSample.SampleName}, LearnPath={viewModel.SelectedLearnPathOption?.Id ?? "-"}");
            }

            if (!viewModel.CanOpenLearnAndSample)
            {
                throw new InvalidOperationException("Workspace sample picker did not enable the explicit guide-plus-sample action.");
            }

            ListBox? sampleList = FindVisualChildren<ListBox>(window)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "WorkspaceSamplePickerList",
                    StringComparison.Ordinal));
            if (sampleList == null || sampleList.Items.Count != viewModel.VisibleSampleCount)
            {
                throw new InvalidOperationException(
                    "Workspace sample picker list did not render the selected catalog source samples. "
                    + $"Expected={viewModel.VisibleSampleCount}, Actual={sampleList?.Items.Count ?? 0}");
            }

            if (viewModel.CatalogSourceOptions.Count < 3
                || viewModel.SelectedCatalogSourceOption == null
                || !string.Equals(viewModel.SelectedCatalogSourceOption.Id, "public", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not default to the public catalog source. "
                    + $"Sources={string.Join(",", viewModel.CatalogSourceOptions.Select(option => option.Id))}, "
                    + $"Selected={viewModel.SelectedCatalogSourceOption?.Id ?? "-"}");
            }

            OpenVisionWorkspaceSampleCatalogSourceOption? publicSource = viewModel.SelectedCatalogSourceOption;
            OpenVisionWorkspaceSampleCatalogSourceOption? productSource = viewModel.CatalogSourceOptions
                .FirstOrDefault(option => string.Equals(option.Id, "product", StringComparison.OrdinalIgnoreCase));
            OpenVisionWorkspaceSampleCatalogSourceOption? localLegacySource = viewModel.CatalogSourceOptions
                .FirstOrDefault(option => string.Equals(option.Id, "local-legacy", StringComparison.OrdinalIgnoreCase));
            if (productSource == null)
            {
                throw new InvalidOperationException("Workspace sample picker did not expose the Product catalog source.");
            }

            if (localLegacySource == null)
            {
                throw new InvalidOperationException("Workspace sample picker did not expose the Local Legacy catalog source.");
            }

            viewModel.SelectedCatalogSourceOption = productSource;
            Pump(40);
            if (sampleList.Items.Count != productSource.SampleCount
                || viewModel.SelectedSample == null
                || viewModel.SelectedSample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.Product)
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not filter to Product samples. "
                    + $"Expected={productSource.SampleCount}, Actual={sampleList.Items.Count}, "
                    + $"Selected={viewModel.SelectedSample?.CatalogSourceId ?? "-"}");
            }

            if (viewModel.SampleFocusOptions.Count < 6 || viewModel.SelectedSampleFocusOption == null)
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not expose Product domain/tool focus options. "
                    + $"Count={viewModel.SampleFocusOptions.Count}, Selected={viewModel.SelectedSampleFocusOption?.Id ?? "-"}");
            }

            OpenVisionWorkspaceSampleFocusOption? batteryFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "battery", StringComparison.OrdinalIgnoreCase));
            OpenVisionWorkspaceSampleFocusOption? displayFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "display", StringComparison.OrdinalIgnoreCase));
            OpenVisionWorkspaceSampleFocusOption? semiconductorFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "semiconductor", StringComparison.OrdinalIgnoreCase));
            if (batteryFocus == null || displayFocus == null || semiconductorFocus == null)
            {
                throw new InvalidOperationException(
                    "Workspace sample picker is missing one or more Product domain focus options. "
                    + "Found=" + string.Join(",", viewModel.SampleFocusOptions.Select(option => option.Id)));
            }

            viewModel.SelectedSampleFocusOption = batteryFocus;
            Pump(40);
            if (sampleList.Items.Count != batteryFocus.SampleCount
                || viewModel.SelectedSample == null
                || !batteryFocus.Matches(viewModel.SelectedSample))
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not filter to the selected Product focus. "
                    + $"Focus={batteryFocus.Id}, Expected={batteryFocus.SampleCount}, Actual={sampleList.Items.Count}, "
                    + $"Selected={viewModel.SelectedSample?.SampleName ?? "-"}");
            }

            viewModel.SelectedCatalogSourceOption = localLegacySource;
            Pump(40);
            if (sampleList.Items.Count != localLegacySource.SampleCount
                || viewModel.SelectedSample == null
                || viewModel.SelectedSample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.LocalLegacy)
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not filter to Local Legacy samples. "
                    + $"Expected={localLegacySource.SampleCount}, Actual={sampleList.Items.Count}, "
                    + $"Selected={viewModel.SelectedSample?.CatalogSourceId ?? "-"}");
            }

            viewModel.SelectedCatalogSourceOption = publicSource;
            Pump(40);

            if (!FindVisualChildren<Image>(window).Any(item => item.Source != null))
            {
                throw new InvalidOperationException("Workspace sample picker did not render the selected sample image.");
            }

            string visibleText = string.Join(
                " | ",
                FindVisualChildren<TextBlock>(window)
                    .Select(item => item.Text)
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
            string[] requiredTokens =
            {
                viewModel.SelectedSample.SampleName,
                viewModel.CatalogSourceLabelText,
                viewModel.ActiveCatalogSourceText,
                viewModel.SelectedCatalogSourceOption.DisplayName,
                viewModel.SampleFocusLabelText,
                viewModel.ActiveSampleFocusText,
                viewModel.SelectedSampleFocusOption.DisplayName,
                viewModel.LearnPathLabelText,
                viewModel.ActiveLearnPathText,
                viewModel.SelectedLearnPathOption.DisplayName,
                viewModel.BenchmarkLabelText,
                viewModel.BenchmarkOutcomeText,
                viewModel.BenchmarkSummaryText,
                viewModel.LearnModeText,
                viewModel.RecommendedStartText,
                viewModel.ResultExplanationText,
                viewModel.FailureCauseText,
                viewModel.LearnDocumentLabelText,
                viewModel.LearnDocumentTitleText,
                viewModel.OpenLearnDocumentButtonText,
                viewModel.OpenLearnAndSampleButtonText,
                viewModel.SelectedSample.ToolFlowText,
                viewModel.SelectedSample.ExpectedText,
                "Preview",
                "Run"
            };
            string? missing = requiredTokens.FirstOrDefault(token => !visibleText.Contains(token, StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Workspace sample picker did not show expected sample guidance token '" + missing + "'. Text='" + visibleText + "'");
            }
        });
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleProductFocusPicker(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace product sample focus picker could not find any runnable catalog samples.");
        }

        OpenVisionWorkspaceSamplePickerViewModel viewModel = new(samples);
        OpenVisionWorkspaceSamplePickerWindow window = new(viewModel);
        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            AssertVisibleAutomationIds(
                window,
                "WPF workspace product sample focus picker",
                "WorkspaceSamplePickerView",
                "WorkspaceSamplePickerSearchBox",
                "WorkspaceSamplePickerCatalogSourceList",
                "WorkspaceSamplePickerCatalogSourceSummary",
                "WorkspaceSamplePickerSampleFocusList",
                "WorkspaceSamplePickerSampleFocusSummary",
                "WorkspaceSamplePickerLearnPathList",
                "WorkspaceSamplePickerLearnPathSummary",
                "WorkspaceSamplePickerList",
                "WorkspaceSamplePickerDetail",
                "WorkspaceSamplePickerSelectedSummary",
                "WorkspaceSamplePickerPreviewImage",
                "WorkspaceSamplePickerOpenButton",
                "WorkspaceSamplePickerCancelButton");

            OpenVisionWorkspaceSampleCatalogSourceOption? productSource = viewModel.CatalogSourceOptions
                .FirstOrDefault(option => string.Equals(option.Id, "product", StringComparison.OrdinalIgnoreCase));
            if (productSource == null)
            {
                throw new InvalidOperationException("Workspace product sample focus picker did not expose the Product catalog source.");
            }

            viewModel.SelectedCatalogSourceOption = productSource;
            Pump(40);

            OpenVisionWorkspaceSampleFocusOption? batteryFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "battery", StringComparison.OrdinalIgnoreCase));
            OpenVisionWorkspaceSampleFocusOption? displayFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "display", StringComparison.OrdinalIgnoreCase));
            OpenVisionWorkspaceSampleFocusOption? semiconductorFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "semiconductor", StringComparison.OrdinalIgnoreCase));
            if (batteryFocus == null || displayFocus == null || semiconductorFocus == null)
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus picker is missing Product domain focus options. "
                    + "Found=" + string.Join(",", viewModel.SampleFocusOptions.Select(option => option.Id)));
            }

            viewModel.SelectedSampleFocusOption = batteryFocus;
            Pump(40);

            ListBox? sampleList = FindVisualChildren<ListBox>(window)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "WorkspaceSamplePickerList",
                    StringComparison.Ordinal));
            if (sampleList == null)
            {
                throw new InvalidOperationException("Workspace product sample focus picker could not find the sample list.");
            }

            if (sampleList.Items.Count != batteryFocus.SampleCount
                || viewModel.SelectedSample == null
                || viewModel.SelectedSample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.Product
                || !batteryFocus.Matches(viewModel.SelectedSample))
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus picker did not capture Product + Battery focus state. "
                    + $"Expected={batteryFocus.SampleCount}, Actual={sampleList.Items.Count}, "
                    + $"Selected={viewModel.SelectedSample?.SampleName ?? "-"}");
            }

            string visibleText = string.Join(
                " | ",
                FindVisualChildren<TextBlock>(window)
                    .Select(item => item.Text)
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
            string[] requiredTokens =
            {
                viewModel.CatalogSourceLabelText,
                viewModel.ActiveCatalogSourceText,
                productSource.DisplayName,
                viewModel.SampleFocusLabelText,
                viewModel.ActiveSampleFocusText,
                batteryFocus.DisplayName,
                displayFocus.DisplayName,
                semiconductorFocus.DisplayName,
                viewModel.LearnPathLabelText,
                viewModel.ResultCountText,
                viewModel.PairDecisionQuickWorkflowText,
                viewModel.SelectedSample.SampleName
            };
            string? missing = requiredTokens.FirstOrDefault(token => !visibleText.Contains(token, StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus picker did not show expected token '" + missing + "'. Text='" + visibleText + "'");
            }

            AssertVisibleAutomationIds(
                window,
                "WPF workspace product sample focus benchmark",
                "WorkspaceSamplePickerBenchmarkStrip",
                "WorkspaceSamplePickerPairDecisionQuickWorkflow");
        });
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleProductFieldFocusPicker(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace product field sample focus picker could not find any runnable catalog samples.");
        }

        OpenVisionWorkspaceSamplePickerViewModel viewModel = new(samples);
        OpenVisionWorkspaceSamplePickerWindow window = new(viewModel);
        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            OpenVisionWorkspaceSampleCatalogSourceOption? productSource = viewModel.CatalogSourceOptions
                .FirstOrDefault(option => string.Equals(option.Id, "product", StringComparison.OrdinalIgnoreCase));
            if (productSource == null)
            {
                throw new InvalidOperationException("Workspace product field sample focus picker did not expose the Product catalog source.");
            }

            viewModel.SelectedCatalogSourceOption = productSource;
            Pump(40);

            OpenVisionWorkspaceSampleFocusOption? fieldFocus = viewModel.SampleFocusOptions
                .FirstOrDefault(option => string.Equals(option.Id, "field", StringComparison.OrdinalIgnoreCase));
            if (fieldFocus == null)
            {
                throw new InvalidOperationException(
                    "Workspace product field sample focus picker is missing the Field focus option. "
                    + "Found=" + string.Join(",", viewModel.SampleFocusOptions.Select(option => option.Id)));
            }

            viewModel.SelectedSampleFocusOption = fieldFocus;
            Pump(40);

            if (viewModel.SelectedSample == null
                || viewModel.SelectedSample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.Product
                || !fieldFocus.Matches(viewModel.SelectedSample)
                || !string.Equals(viewModel.SelectedSample.ValidationMode, "Explore", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Workspace product field sample focus picker did not select a Product Explore field sample. "
                    + $"Selected={viewModel.SelectedSample?.SampleName ?? "-"}, "
                    + $"Mode={viewModel.SelectedSample?.ValidationMode ?? "-"}");
            }

            string visibleText = string.Join(
                " | ",
                FindVisualChildren<TextBlock>(window)
                    .Select(item => item.Text)
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
            string[] requiredTokens =
            {
                productSource.DisplayName,
                fieldFocus.DisplayName,
                viewModel.ActiveSampleFocusText,
                viewModel.SelectedSample.SampleName,
                viewModel.BenchmarkLabelText,
                viewModel.BenchmarkOutcomeText,
                viewModel.BenchmarkSummaryText,
                viewModel.ExploratoryGuideText,
                viewModel.SelectedSample.ExpectedText
            };
            string? missing = requiredTokens.FirstOrDefault(token => !visibleText.Contains(token, StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Workspace product field sample focus picker did not show expected token '" + missing + "'. Text='" + visibleText + "'");
            }

            AssertVisibleAutomationIds(
                window,
                "WPF workspace product field sample focus picker",
                "WorkspaceSamplePickerView",
                "WorkspaceSamplePickerSampleFocusList",
                "WorkspaceSamplePickerSampleFocusSummary",
                "WorkspaceSamplePickerBenchmarkStrip",
                "WorkspaceSamplePickerExploreGuide",
                "WorkspaceSamplePickerPreviewImage",
                "WorkspaceSamplePickerOpenButton",
                "WorkspaceSamplePickerCancelButton");
        });
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleProductFocusOpen(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace product sample focus open could not find any runnable catalog samples.");
        }

        OpenVisionWorkspaceSamplePickerViewModel pickerViewModel = new(samples);
        OpenVisionWorkspaceSampleCatalogSourceOption? productSource = pickerViewModel.CatalogSourceOptions
            .FirstOrDefault(option => string.Equals(option.Id, "product", StringComparison.OrdinalIgnoreCase));
        if (productSource == null)
        {
            throw new InvalidOperationException("Workspace product sample focus open did not expose the Product catalog source.");
        }

        pickerViewModel.SelectedCatalogSourceOption = productSource;
        OpenVisionWorkspaceSampleFocusOption? batteryFocus = pickerViewModel.SampleFocusOptions
            .FirstOrDefault(option => string.Equals(option.Id, "battery", StringComparison.OrdinalIgnoreCase));
        if (batteryFocus == null)
        {
            throw new InvalidOperationException(
                "Workspace product sample focus open did not expose the Battery focus option. "
                + "Found=" + string.Join(",", pickerViewModel.SampleFocusOptions.Select(option => option.Id)));
        }

        pickerViewModel.SelectedSampleFocusOption = batteryFocus;
        VisionPipelineSampleCatalogItem selectedSample = pickerViewModel.SelectedSample
            ?? throw new InvalidOperationException("Workspace product sample focus open did not select a Battery sample.");
        if (selectedSample.CatalogSourceKind != VisionPipelineSampleCatalogSourceKind.Product
            || !batteryFocus.Matches(selectedSample))
        {
            throw new InvalidOperationException(
                "Workspace product sample focus open selected a sample outside the Product + Battery focus. "
                + $"Sample={selectedSample.SampleName}, Source={selectedSample.CatalogSourceKind}, Focus={batteryFocus.Id}");
        }

        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceSampleProductFocusOpen", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.OpenWorkspaceSampleForTest(selectedSample.SampleName))
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus open could not open the selected Product + Battery sample. "
                    + $"Sample={selectedSample.SampleName}");
            }

            Pump(80);

            if (!shellHost.HasMainLayer
                || !shellHost.HasWorkspaceLayerPreview
                || shellHost.IsWorkspaceEmptyPromptVisible)
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus open did not load the selected sample image into Main. "
                    + $"HasMain={shellHost.HasMainLayer}, HasPreview={shellHost.HasWorkspaceLayerPreview}, "
                    + $"Empty={shellHost.IsWorkspaceEmptyPromptVisible}");
            }

            string workflowMeta = shellHost.WorkspaceSampleWorkflowMetaForTest ?? string.Empty;
            string workflowDetail = shellHost.WorkspaceSampleWorkflowDetailForTest ?? string.Empty;
            string role = string.IsNullOrWhiteSpace(selectedSample.PairRole)
                ? (selectedSample.ExpectsFailure ? "NG" : "OK")
                : selectedSample.PairRole.Trim();
            string firstTool = selectedSample.ToolFlowText.Split(new[] { "->" }, StringSplitOptions.None)
                .Select(part => part.Trim())
                .FirstOrDefault(part => !string.IsNullOrWhiteSpace(part))
                ?? string.Empty;
            string productGroup = selectedSample.Category.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Skip(1)
                .FirstOrDefault(part => !string.IsNullOrWhiteSpace(part))
                ?? selectedSample.Category;

            if (!shellHost.IsWorkspaceSampleWorkflowVisibleForTest
                || !workflowMeta.Contains(selectedSample.SampleName, StringComparison.Ordinal)
                || !workflowDetail.Contains(productGroup, StringComparison.Ordinal)
                || !workflowDetail.Contains(role, StringComparison.OrdinalIgnoreCase)
                || !workflowDetail.Contains("\uB2E4\uC74C", StringComparison.Ordinal)
                || !workflowDetail.Contains("\uBE44\uAD50", StringComparison.Ordinal)
                || !workflowDetail.Contains(selectedSample.ExpectsFailure ? "OK" : "NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.CanOpenSampleCounterpartForTest
                || (!string.IsNullOrWhiteSpace(firstTool) && !workflowDetail.Contains(firstTool, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Workspace product sample focus open did not carry the selected sample focus into the workflow breadcrumb. "
                    + $"Sample={selectedSample.SampleName}, ProductGroup={productGroup}, Role={role}, FirstTool={firstTool}, "
                    + $"Meta='{workflowMeta}', Detail='{workflowDetail}', Counterpart={shellHost.CanOpenSampleCounterpartForTest}");
            }

            if (shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Workspace product sample focus open must not auto-open a tool or run Preview.");
            }
        }, captureFloatingToolWindow: false, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleProductCounterpartOpen(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        OpenVisionWorkspaceSamplePickerViewModel pickerViewModel = new(samples);
        OpenVisionWorkspaceSampleCatalogSourceOption productSource = pickerViewModel.CatalogSourceOptions
            .FirstOrDefault(option => string.Equals(option.Id, "product", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Workspace product sample counterpart open did not expose the Product catalog source.");
        pickerViewModel.SelectedCatalogSourceOption = productSource;
        OpenVisionWorkspaceSampleFocusOption batteryFocus = pickerViewModel.SampleFocusOptions
            .FirstOrDefault(option => string.Equals(option.Id, "battery", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Workspace product sample counterpart open did not expose the Battery focus option.");
        pickerViewModel.SelectedSampleFocusOption = batteryFocus;

        VisionPipelineSampleCatalogItem selectedSample = pickerViewModel.SelectedSample
            ?? throw new InvalidOperationException("Workspace product sample counterpart open did not select a Battery sample.");
        VisionPipelineSampleCatalogItem counterpartSample = ResolveCounterpartCatalogSample(selectedSample)
            ?? throw new InvalidOperationException("Workspace product sample counterpart open could not resolve an opposite reference sample for " + selectedSample.SampleName);

        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceSampleProductCounterpartOpen", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            if (!shellHost.OpenWorkspaceSampleForTest(selectedSample.SampleName))
            {
                throw new InvalidOperationException("Workspace product sample counterpart open could not open selected sample: " + selectedSample.SampleName);
            }

            Pump(80);
            int runsBefore = shellHost.NativePreviewRunCount;
            if (!shellHost.CanOpenSampleCounterpartForTest)
            {
                throw new InvalidOperationException(
                    "Workspace product sample counterpart command was not enabled after opening the selected sample. "
                    + $"Sample={selectedSample.SampleName}, Workflow='{shellHost.WorkspaceSampleWorkflowDetailForTest}'");
            }

            shellHost.OpenSampleCounterpartForTest();
            Pump(120);

            string workflowMeta = shellHost.WorkspaceSampleWorkflowMetaForTest ?? string.Empty;
            if (!shellHost.HasMainLayer
                || !shellHost.HasWorkspaceLayerPreview
                || !workflowMeta.Contains(counterpartSample.SampleName, StringComparison.Ordinal)
                || shellHost.NativePreviewRunCount != runsBefore
                || shellHost.IsActiveWpfToolWindowVisibleForTest
                || shellHost.IsNativeDocumentActive
                || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Workspace product sample counterpart command did not switch samples without opening a tool or running Preview. "
                    + $"Expected={counterpartSample.SampleName}, Meta='{workflowMeta}', RunsBefore={runsBefore}, "
                    + $"RunsAfter={shellHost.NativePreviewRunCount}, ToolVisible={shellHost.IsActiveWpfToolWindowVisibleForTest}, "
                    + $"NativeActive={shellHost.IsNativeDocumentActive}, Preview={shellHost.HasNativePreviewResult}");
            }
        }, captureFloatingToolWindow: false, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
    }

    private static VisionPipelineSampleCatalogItem? ResolveCounterpartCatalogSample(VisionPipelineSampleCatalogItem sample)
    {
        if (sample == null || string.IsNullOrWhiteSpace(sample.PairGroup))
        {
            return null;
        }

        bool selectedIsOk = !sample.ExpectsFailure
            && string.Equals(sample.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
        bool selectedIsNg = sample.ExpectsFailure
            || string.Equals(sample.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase);
        string pairGroup = sample.PairGroup.Trim();
        return VisionPipelineSampleCatalogItem.LoadRunnable(sample.CatalogSourceKind)
            .Where(item => item != null
                && item.CanOpen
                && !string.Equals(item.SampleName?.Trim(), sample.SampleName?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.PairGroup?.Trim(), pairGroup, StringComparison.OrdinalIgnoreCase))
            .Where(item =>
                selectedIsOk
                    ? item.ExpectsFailure || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase)
                    : selectedIsNg
                        ? !item.ExpectsFailure && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase)
                        : true)
            .OrderBy(item => item.ExpectsFailure ? 1 : 0)
            .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleLearnPaths(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace sample learn paths could not find any runnable catalog samples.");
        }

        OpenVisionWorkspaceSamplePickerViewModel viewModel = new(samples);
        OpenVisionWorkspaceSamplePickerWindow window = new(viewModel);
        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            AssertVisibleAutomationIds(
                window,
                "WPF workspace sample learn paths",
                "WorkspaceSamplePickerView",
                "WorkspaceSamplePickerSearchBox",
                "WorkspaceSamplePickerCatalogSourceList",
                "WorkspaceSamplePickerCatalogSourceSummary",
                "WorkspaceSamplePickerSampleFocusList",
                "WorkspaceSamplePickerSampleFocusSummary",
                "WorkspaceSamplePickerLearnPathList",
                "WorkspaceSamplePickerLearnPathSummary",
                "WorkspaceSamplePickerList",
                "WorkspaceSamplePickerDetail",
                "WorkspaceSamplePickerSelectedSummary",
                "WorkspaceSamplePickerBenchmarkStrip",
                "WorkspaceSamplePickerLearnModeStrip",
                "WorkspaceSamplePickerOpenLearnDocumentButton",
                "WorkspaceSamplePickerPreviewImage",
                "WorkspaceSamplePickerToolFlow",
                "WorkspaceSamplePickerExpected",
                "WorkspaceSamplePickerOpenButton",
                "WorkspaceSamplePickerOpenGuideAndSampleButton",
                "WorkspaceSamplePickerCancelButton");

            if (viewModel.LearnPathOptions.Count < 4 || viewModel.SelectedLearnPathOption == null)
            {
                throw new InvalidOperationException(
                    "Workspace sample learn path selector did not expose enough task-oriented paths. "
                    + $"Count={viewModel.LearnPathOptions.Count}, Selected={viewModel.SelectedLearnPathOption?.Id ?? "-"}");
            }

            ListBox? sampleList = FindVisualChildren<ListBox>(window)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "WorkspaceSamplePickerList",
                    StringComparison.Ordinal));
            if (sampleList == null)
            {
                throw new InvalidOperationException("Workspace sample learn path smoke could not find the sample list.");
            }

            string[] pathIds = { "matching", "blob", "line", "pair" };
            List<OpenVisionWorkspaceSampleLearnPathOption> checkedPaths = viewModel.LearnPathOptions
                .Where(option => pathIds.Contains(option.Id, StringComparer.OrdinalIgnoreCase))
                .ToList();
            if (checkedPaths.Count < 3)
            {
                throw new InvalidOperationException(
                    "Workspace sample learn path selector is missing expected task groups. "
                    + "Found=" + string.Join(",", viewModel.LearnPathOptions.Select(option => option.Id)));
            }

            foreach (OpenVisionWorkspaceSampleLearnPathOption path in checkedPaths)
            {
                viewModel.SelectedLearnPathOption = path;
                Pump(40);

                if (sampleList.Items.Count != path.SampleCount)
                {
                    throw new InvalidOperationException(
                        "Workspace sample learn path did not filter the list to the expected count. "
                        + $"Path={path.Id}, Expected={path.SampleCount}, Actual={sampleList.Items.Count}");
                }

                if (viewModel.SelectedSample == null || !path.Matches(viewModel.SelectedSample))
                {
                    throw new InvalidOperationException(
                        "Workspace sample learn path did not keep a matching selected sample. "
                        + $"Path={path.Id}, Selected={viewModel.SelectedSample?.SampleName ?? "-"}");
                }
            }

            OpenVisionWorkspaceSampleLearnPathOption capturePath =
                checkedPaths.FirstOrDefault(option => string.Equals(option.Id, "matching", StringComparison.OrdinalIgnoreCase))
                ?? checkedPaths[0];
            viewModel.SelectedLearnPathOption = capturePath;
            Pump(40);

            if (!viewModel.HasLearnDocument || !viewModel.OpenLearnDocumentCommand.CanExecute(null))
            {
                throw new InvalidOperationException(
                    "Workspace sample learn path did not resolve a Learn document for the captured path. "
                    + $"Path={capturePath.Id}, Sample={viewModel.SelectedSample?.SampleName ?? "-"}");
            }

            if (!viewModel.CanOpenLearnAndSample)
            {
                throw new InvalidOperationException("Workspace sample learn path did not enable the explicit guide-plus-sample action.");
            }

            string visibleText = string.Join(
                " | ",
                FindVisualChildren<TextBlock>(window)
                    .Select(item => item.Text)
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
            string[] requiredTokens =
            {
                viewModel.CatalogSourceLabelText,
                viewModel.ActiveCatalogSourceText,
                viewModel.SelectedCatalogSourceOption.DisplayName,
                viewModel.LearnPathLabelText,
                viewModel.LearnDocumentLabelText,
                viewModel.OpenLearnDocumentButtonText,
                viewModel.OpenLearnAndSampleButtonText,
                capturePath.DisplayName,
                capturePath.Description,
                viewModel.ResultCountText,
                viewModel.SelectedSample.SampleName
            };
            string? missing = requiredTokens.FirstOrDefault(token => !visibleText.Contains(token, StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Workspace sample learn path did not show expected token '" + missing + "'. Text='" + visibleText + "'");
            }
        });
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePairPicker(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        VisionPipelineSampleCatalogItem? pairedSample = samples.FirstOrDefault(item =>
            item.HasPair
            && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase));
        if (pairedSample == null)
        {
            throw new InvalidOperationException("Workspace sample pair picker could not find a runnable Good pair sample.");
        }

        OpenVisionWorkspaceSamplePickerViewModel viewModel = new(samples)
        {
            SelectedSample = pairedSample
        };
        OpenVisionWorkspaceSamplePickerWindow window = new(viewModel);
        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            AssertVisibleAutomationIds(
                window,
                "WPF workspace sample pair picker",
                "WorkspaceSamplePickerView",
                "WorkspaceSamplePickerSearchBox",
                "WorkspaceSamplePickerCatalogSourceList",
                "WorkspaceSamplePickerCatalogSourceSummary",
                "WorkspaceSamplePickerSampleFocusList",
                "WorkspaceSamplePickerSampleFocusSummary",
                "WorkspaceSamplePickerLearnPathList",
                "WorkspaceSamplePickerLearnPathSummary",
                "WorkspaceSamplePickerList",
                "WorkspaceSamplePickerDetail",
                "WorkspaceSamplePickerSelectedSummary",
                "WorkspaceSamplePickerBenchmarkStrip",
                "WorkspaceSamplePickerPairDecisionQuickWorkflow",
                "WorkspaceSamplePickerPairComparisonStrip",
                "WorkspaceSamplePickerPairDecisionGuide",
                "WorkspaceSamplePickerPairDecisionChecklist",
                "WorkspaceSamplePickerPairDecisionNextAction",
                "WorkspaceSamplePickerSelectCounterpartButton",
                "WorkspaceSamplePickerLearnModeStrip",
                "WorkspaceSamplePickerPreviewImage",
                "WorkspaceSamplePickerToolFlow",
                "WorkspaceSamplePickerExpected",
                "WorkspaceSamplePickerOpenButton",
                "WorkspaceSamplePickerOpenGuideAndSampleButton",
                "WorkspaceSamplePickerCancelButton");

            if (viewModel.SelectedSample == null
                || !viewModel.SelectedSample.HasPair
                || !string.Equals(viewModel.SelectedSample.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Workspace sample pair picker did not keep a Good paired sample selected.");
            }

            string visibleText = string.Join(
                " | ",
                FindVisualChildren<TextBlock>(window)
                    .Select(item => item.Text)
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
            string[] requiredTokens =
            {
                viewModel.SelectedSample.SampleName,
                viewModel.CatalogSourceLabelText,
                viewModel.ActiveCatalogSourceText,
                viewModel.SelectedCatalogSourceOption.DisplayName,
                viewModel.SampleFocusLabelText,
                viewModel.ActiveSampleFocusText,
                viewModel.SelectedSampleFocusOption.DisplayName,
                viewModel.LearnPathLabelText,
                viewModel.ActiveLearnPathText,
                viewModel.SelectedLearnPathOption.DisplayName,
                viewModel.PairComparisonLabelText,
                viewModel.PairComparisonSummaryText,
                viewModel.PairComparisonDetailText,
                viewModel.PairDecisionLabelText,
                viewModel.PairDecisionSummaryText,
                viewModel.PairDecisionMetricText,
                viewModel.PairDecisionChecklistText,
                viewModel.PairDecisionNextActionText,
                viewModel.PairDecisionQuickWorkflowText,
                viewModel.PairDecisionWorkflowText,
                viewModel.LearnModeText,
                viewModel.RecommendedStartText,
                viewModel.ResultExplanationText,
                viewModel.FailureCauseText,
                viewModel.BenchmarkOutcomeText,
                viewModel.BenchmarkPairText,
                viewModel.SelectedSample.ExpectedText,
                viewModel.SelectedSample.PairGroup,
                "Preview",
                "Run"
            };
            string? missing = requiredTokens.FirstOrDefault(token => !visibleText.Contains(token, StringComparison.Ordinal));
            if (!string.IsNullOrWhiteSpace(missing))
            {
                throw new InvalidOperationException(
                    "Workspace sample pair picker did not show expected pair token '" + missing + "'. Text='" + visibleText + "'");
            }

            VisionPipelineSampleCatalogItem originalSample = viewModel.SelectedSample;
            if (!viewModel.SelectCounterpartSampleCommand.CanExecute(null))
            {
                throw new InvalidOperationException("Workspace sample pair picker did not enable the explicit opposite-reference selection command.");
            }

            viewModel.SelectCounterpartSampleCommand.Execute(null);
            Pump(40);
            if (viewModel.SelectedSample == null
                || ReferenceEquals(viewModel.SelectedSample, originalSample)
                || !viewModel.SelectedSample.ExpectsFailure
                || !string.Equals(viewModel.SelectedSample.PairGroup, originalSample.PairGroup, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Workspace sample pair picker did not switch from the Good sample to the paired NG reference. "
                    + $"Original={originalSample.SampleName}, Selected={viewModel.SelectedSample?.SampleName ?? "-"}");
            }

            viewModel.SelectCounterpartSampleCommand.Execute(null);
            Pump(40);
            if (!ReferenceEquals(viewModel.SelectedSample, originalSample))
            {
                throw new InvalidOperationException(
                    "Workspace sample pair picker did not switch back from the NG reference to the original Good sample. "
                    + $"Original={originalSample.SampleName}, Selected={viewModel.SelectedSample?.SampleName ?? "-"}");
            }
        });
    }

    private static CaptureResult CaptureShellHostWorkspaceSamplePairCoverage(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        DateTime started = DateTime.UtcNow;
        List<VisionPipelineSampleCatalogItem> samples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen)
            .ToList();
        if (samples.Count == 0)
        {
            throw new InvalidOperationException("Workspace sample pair coverage could not find runnable catalog samples.");
        }

        List<IGrouping<string, VisionPipelineSampleCatalogItem>> pairGroups = samples
            .Where(item => item.HasPair && !string.IsNullOrWhiteSpace(item.PairGroup))
            .GroupBy(item => item.PairGroup.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();
        string[] requiredPairGroups =
        {
            "Public_Blob_Particles",
            "Public_Contour_Shapes",
            "Public_Line_Pins",
            "Public_Matching_DiePad",
            "Public_Edge_Fiducial",
            "Public_Feature_Card",
            "Public_Mean_BrightnessDrift",
            "Public_Threshold_BandPads",
            "Battery_TabGap",
            "Display_Particle",
            "Semiconductor_PackagePolarity"
        };

        foreach (string requiredPairGroup in requiredPairGroups)
        {
            if (!pairGroups.Any(group => string.Equals(group.Key, requiredPairGroup, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Sample catalog is missing required Good/Bad pair group: " + requiredPairGroup);
            }
        }

        foreach (IGrouping<string, VisionPipelineSampleCatalogItem> group in pairGroups)
        {
            ValidateSamplePairGroup(group.Key, group.ToList());
        }

        string[] executionGroups =
        {
            "Public_Blob_Particles",
            "Public_Line_Pins",
            "Public_Matching_DiePad",
            "Public_Mean_BrightnessDrift",
            "Display_Particle"
        };
        List<string> executionLines = new();
        foreach (string groupName in executionGroups)
        {
            List<VisionPipelineSampleCatalogItem> groupSamples = pairGroups
                .First(group => string.Equals(group.Key, groupName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => IsSamplePairGood(item) ? 0 : 1)
                .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (VisionPipelineSampleCatalogItem sample in groupSamples.Where(item => IsSamplePairGood(item)).Take(1)
                .Concat(groupSamples.Where(item => IsSamplePairBad(item)).Take(1)))
            {
                VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.RunSampleCheckSafe(sample);
                if (!result.Success)
                {
                    throw new InvalidOperationException(
                        "Sample pair execution failed. "
                        + $"Group={groupName}, Sample={sample.SampleName}, Status={result.Status}, Message={result.Message}, Metrics={result.MetricText}");
                }

                executionLines.Add($"{groupName} / {sample.PairRole}: {sample.SampleName} / {result.MetricText}");
            }
        }

        string reportText =
            "Good/Bad Sample Pair Coverage OK"
            + Environment.NewLine
            + $"Runnable samples: {samples.Count}"
            + Environment.NewLine
            + $"Pair groups: {pairGroups.Count}"
            + Environment.NewLine
            + $"Required groups: {requiredPairGroups.Length}"
            + Environment.NewLine
            + $"Executed reference samples: {executionLines.Count}"
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(Environment.NewLine, executionLines);

        Border report = new()
        {
            Background = Brushes.White,
            Padding = new Thickness(24),
            Child = new TextBlock
            {
                Text = reportText,
                FontSize = 15,
                FontFamily = new FontFamily("Consolas"),
                Foreground = Brushes.DarkSlateGray,
                TextWrapping = TextWrapping.Wrap
            }
        };

        CaptureResult capture = CaptureElement(report, outputPath, 1120, 620);
        return new CaptureResult(capture.Width, capture.Height, (DateTime.UtcNow - started).TotalMilliseconds);
    }

    private static CaptureResult CaptureShellHostWorkspaceSampleBadReferenceAudit(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        DateTime started = DateTime.UtcNow;
        List<VisionPipelineSampleCatalogItem> badSamples = VisionPipelineSampleCatalogItem.LoadRunnable()
            .Where(item => item.CanOpen && IsSamplePairBad(item))
            .OrderBy(item => item.PairGroup, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.SampleName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (badSamples.Count == 0)
        {
            throw new InvalidOperationException("Workspace sample bad-reference audit could not find runnable Bad references.");
        }

        List<string> reportLines = new();
        int controlledNgCount = 0;
        int comparativeBadCount = 0;
        foreach (VisionPipelineSampleCatalogItem sample in badSamples)
        {
            VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.RunSampleCheckSafe(sample);
            if (!result.Success)
            {
                throw new InvalidOperationException(
                    "Bad reference audit failed sample execution or expected metric check. "
                    + $"Sample={sample.SampleName}, Mode={sample.ValidationMode}, Group={sample.PairGroup}, "
                    + $"Status={result.Status}, Message={result.Message}, Metrics={result.MetricText}");
            }

            string classification;
            if (sample.ExpectsFailure)
            {
                classification = "Controlled NG";
                controlledNgCount++;
            }
            else
            {
                classification = "Comparative Bad";
                comparativeBadCount++;
            }

            reportLines.Add(
                $"{classification,-16} | {sample.PairGroup,-28} | {sample.SampleName,-42} | {result.MetricText}");
        }

        if (!badSamples.Any(item => string.Equals(item.SampleName, "Public_Mean_Brightness_Dark_Bad", StringComparison.OrdinalIgnoreCase)
            && item.ExpectsFailure
            && item.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Public))
        {
            throw new InvalidOperationException("Bad reference audit requires Public_Mean_Brightness_Dark_Bad to remain a public controlled NG ExpectedFailure sample.");
        }

        if (controlledNgCount == 0)
        {
            throw new InvalidOperationException(
                "Bad reference audit expects public/product Bad references to be controlled NG ExpectedFailure samples. "
                + $"ControlledNG={controlledNgCount}, ComparativeBad={comparativeBadCount}");
        }

        string reportText =
            "Good/Bad Bad Reference Audit OK"
            + Environment.NewLine
            + $"Runnable Bad references: {badSamples.Count}"
            + Environment.NewLine
            + $"Controlled NG: {controlledNgCount}"
            + Environment.NewLine
            + $"Comparative Bad (legacy/private): {comparativeBadCount}"
            + Environment.NewLine
            + "Rule: public/product Bad references should be ExpectedFailure samples rejected through a stable metric acceptance gate."
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(Environment.NewLine, reportLines);

        Border report = new()
        {
            Background = Brushes.White,
            Padding = new Thickness(24),
            Child = new TextBlock
            {
                Text = reportText,
                FontSize = 13,
                FontFamily = new FontFamily("Consolas"),
                Foreground = Brushes.DarkSlateGray,
                TextWrapping = TextWrapping.Wrap
            }
        };

        CaptureResult capture = CaptureElement(report, outputPath, 1360, 760);
        return new CaptureResult(capture.Width, capture.Height, (DateTime.UtcNow - started).TotalMilliseconds);
    }

    private static void ValidateSamplePairGroup(string groupName, IReadOnlyList<VisionPipelineSampleCatalogItem> samples)
    {
        if (!samples.Any(IsSamplePairGood) || !samples.Any(IsSamplePairBad))
        {
            throw new InvalidOperationException("Sample pair group must include both Good and Bad references. Group=" + groupName);
        }

        HashSet<string> pipelines = new(samples
            .Select(item => item.BaselinePipeline?.Trim() ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value)), StringComparer.OrdinalIgnoreCase);
        if (pipelines.Count != 1)
        {
            throw new InvalidOperationException(
                "Sample pair group must share one baseline pipeline. "
                + $"Group={groupName}, Pipelines={string.Join(", ", pipelines)}");
        }

        HashSet<string> goodMetricNames = new(samples
            .Where(IsSamplePairGood)
            .SelectMany(item => item.ExpectedMetrics)
            .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
            .Select(metric => metric.Name.Trim()), StringComparer.OrdinalIgnoreCase);
        HashSet<string> badMetricNames = new(samples
            .Where(IsSamplePairBad)
            .SelectMany(item => item.ExpectedMetrics)
            .Where(metric => metric != null && !string.IsNullOrWhiteSpace(metric.Name))
            .Select(metric => metric.Name.Trim()), StringComparer.OrdinalIgnoreCase);
        if (!goodMetricNames.Any(badMetricNames.Contains))
        {
            throw new InvalidOperationException(
                "Sample pair group must expose at least one shared Good/Bad metric. "
                + $"Group={groupName}, Good={string.Join(", ", goodMetricNames)}, Bad={string.Join(", ", badMetricNames)}");
        }

        foreach (VisionPipelineSampleCatalogItem sample in samples)
        {
            if (sample.ExpectedMetrics.Count == 0)
            {
                throw new InvalidOperationException("Sample pair row is missing expected metrics. Sample=" + sample.SampleName);
            }

            foreach (VisionPipelineSampleExpectedMetric metric in sample.ExpectedMetrics)
            {
                if (string.IsNullOrWhiteSpace(metric.Minimum) || string.IsNullOrWhiteSpace(metric.Maximum))
                {
                    throw new InvalidOperationException(
                        "Sample pair metric must have min/max bounds. "
                        + $"Sample={sample.SampleName}, Metric={metric.Name}");
                }
            }
        }
    }

    private static bool IsSamplePairGood(VisionPipelineSampleCatalogItem item)
    {
        return item != null
            && !item.ExpectsFailure
            && string.Equals(item.PairRole?.Trim(), "Good", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSamplePairBad(VisionPipelineSampleCatalogItem item)
    {
        return item != null
            && (item.ExpectsFailure
                || string.Equals(item.PairRole?.Trim(), "Bad", StringComparison.OrdinalIgnoreCase));
    }

    private static CaptureResult CaptureShellHostFileLoadBlobE2E(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostFileLoadBlobE2E", seedMainLayer: false);
        string imagePath = CreateWorkspaceLoadSmokeImageFile();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                // Keep this close to the operator flow: file load, tool open, PropertyGrid, preview, and visible workspace capture.
                if (!shellHost.IsWorkspaceEmptyPromptVisible)
                {
                    throw new InvalidOperationException("File-load Blob E2E must start from the empty workspace prompt.");
                }

                if (!shellHost.LoadMainImageFromFileForTest(imagePath))
                {
                    throw new InvalidOperationException("File-load Blob E2E could not load the smoke image into Main.");
                }

                Pump(20);
                if (!shellHost.HasWorkspaceLayerPreview
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.WorkspaceLayerMeta.Contains("640x360", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "File-load Blob E2E did not show the loaded Main image before opening the tool. "
                        + $"Title={shellHost.WorkspaceLayerTitle}, Meta={shellHost.WorkspaceLayerMeta}");
                }

                shellHost.SelectToolForTest(VISION_MENU.Blob);
                Pump(24);

                ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                AssertVisionToolComboTemplate(inputLayerCombo, "File-load Blob E2E input layer combo");
                AssertComboBoxPopupLayout(inputLayerCombo, "File-load Blob E2E input layer combo");
                AssertVisionToolComboTemplate(outputLayerCombo, "File-load Blob E2E output layer combo");
                AssertComboBoxPopupLayout(outputLayerCombo, "File-load Blob E2E output layer combo");
                AssertFloatingPropertyGridVisible("File-load Blob E2E PropertyGrid");
                if (!HasFloatingToolPreviewImageSource()
                    || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.ActiveNativeDocumentTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "File-load Blob E2E did not open Blob with the loaded Main input preview. "
                        + $"Document={shellHost.ActiveNativeDocumentTypeName}, Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}");
                }

                shellHost.RunActiveNativePreviewForTest();
                Pump(28);
                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.HasNativePreviewResult
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Blob_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "File-load Blob E2E did not produce a Blob preview result. "
                        + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertResultReviewVisible("File-load Blob E2E", "Blob /", "검출", "최대 면적", "중심", "박스");
                WriteFloatingToolWindowCapture(outputPath);
            }, captureFloatingToolWindow: false, verifyCapture: AssertWorkspaceLoadImageVisibleInCapture);
        }
        finally
        {
            TryDeleteFile(imagePath);
        }
    }

    private static CaptureResult CaptureShellHostToolInputEmpty(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostToolInputEmpty", seedMainLayer: false);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Filter);
            Pump(24);

            if (!FloatingToolTextContains(OpenVisionLanguageService.T("ToolView.NoInputImageTitle"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("ToolView.LoadImageButton"))
                || !FloatingToolComboHasItem("Main"))
            {
                throw new InvalidOperationException("WPF tool input preview did not expose the empty-image prompt or Main input layer.");
            }
        });
    }

    private static CaptureResult CaptureShellHostToolInputImageLoadSave(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostToolInputImageLoadSave", seedMainLayer: false);
        string imagePath = CreateWorkspaceLoadSmokeImageFile();
        string savePath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_tool_input_save_smoke_" + Guid.NewGuid().ToString("N") + ".png");
        try
        {
            TryDeleteFile(savePath);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.Filter);
                Pump(24);

                if (!shellHost.LoadActiveNativePreviewImageFromFileForTest(imagePath))
                {
                    throw new InvalidOperationException("WPF tool input image load test helper failed.");
                }

                Pump(24);
                if (!HasFloatingToolPreviewImageSource()
                    || !shellHost.HasWorkspaceLayerPreview
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || !shellHost.WorkspaceLayerMeta.Contains("640x360", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("WPF tool input image load did not refresh the tool preview and Main workspace.");
                }

                if (!shellHost.SaveActiveNativePreviewImageToFileForTest(savePath)
                    || !File.Exists(savePath)
                    || new FileInfo(savePath).Length <= 0)
                {
                    throw new InvalidOperationException("WPF tool input image save did not create an output file.");
                }
            });
        }
        finally
        {
            TryDeleteFile(imagePath);
            TryDeleteFile(savePath);
        }
    }

    private static CaptureResult CaptureShellHostWorkspaceOutput(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostWorkspaceOutput");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF HSV tool did not produce a preview result.");
            }

            if (!shellHost.HasWorkspaceLayerPreview || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("WPF workspace did not switch to the output layer after preview.");
            }

            if (shellHost.WorkspaceTextureTileCount <= 0)
            {
                throw new InvalidOperationException("WPF workspace did not load the output OpenGL texture.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CapturePreprocessOutputPreviewFlow(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfPreprocessOutputPreviewFlow");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            foreach ((VISION_MENU Menu, string DocumentType, string OutputLayer) scenario in GetPreprocessOutputPreviewScenarios())
            {
                string expectedInputLayer = scenario.Menu == VISION_MENU.Morphology && shellHost.HasLayerForTest("Filter_Preview")
                    ? "Filter_Preview"
                    : "Main";
                shellHost.ActivateHostLayerForTest(expectedInputLayer);
                Pump(8);
                shellHost.SelectToolForTest(scenario.Menu);
                Pump(18);

                if (!string.Equals(expectedInputLayer, "Main", StringComparison.OrdinalIgnoreCase))
                {
                    ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                    SelectComboBoxItemText(inputLayerCombo, expectedInputLayer, scenario.Menu + " chained input layer combo");
                    Pump(12);
                }

                AssertActiveFloatingInlinePreviewSlotCount(scenario.Menu + " input", 1);
                int beforeRuns = shellHost.NativePreviewRunCount;
                if (ExercisePreprocessParameterAutoPreviewChange(scenario.Menu))
                {
                    Thread.Sleep(180);
                    Pump(30);
                    if (shellHost.NativePreviewRunCount <= beforeRuns)
                    {
                        throw new InvalidOperationException(
                            scenario.Menu + " parameter change did not auto-preview. "
                            + $"RunsBefore={beforeRuns}, RunsAfter={shellHost.NativePreviewRunCount}, Status={shellHost.ActiveNativeStatusText}");
                    }
                }
                else
                {
                    shellHost.RunActiveNativePreviewForTest();
                    Pump(28);
                }

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.HasNativePreviewResult
                    || !shellHost.ActiveNativeDocumentTypeName.Contains(scenario.DocumentType, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, expectedInputLayer, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, scenario.OutputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        scenario.Menu + " did not produce a routed preprocessing preview. "
                        + $"Document={shellHost.ActiveNativeDocumentTypeName}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                        + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertActiveFloatingInlinePreviewSlotCount(scenario.Menu + " input/output", 2);
                AssertSimplePreprocessResultReview(scenario.Menu, shellHost);
            }
        });
    }

    private static void AssertSimplePreprocessResultReview(VISION_MENU menu, OpenVisionShellHostView shellHost)
    {
        if (menu == VISION_MENU.Mean)
        {
            AssertResultReviewVisible(
                "Mean result review",
                "Mean /",
                OpenVisionLanguageService.T("VisionTool.Review.Label.Range"));
            AssertResultReviewVisible(
                "Mean result guidance",
                OpenVisionLanguageService.T("VisionTool.Status.PreviewNg"),
                OpenVisionLanguageService.T("VisionTool.Review.Label.Range"));
        }
        else if (menu == VISION_MENU.HSV)
        {
            AssertResultReviewVisible("HSV result review", "HSV /", "H ", "S ", "V ");
            AssertResultReviewVisible(
                "HSV result guidance",
                OpenVisionLanguageService.T("VisionTool.Status.PreviewOk"),
                OpenVisionLanguageService.T("VisionTool.Review.Label.Pixels"));
        }
        else if (menu == VISION_MENU.Histogram)
        {
            AssertResultReviewVisible(
                "Histogram result review",
                "Histogram /",
                OpenVisionLanguageService.T("VisionTool.Review.Label.Mean"),
                OpenVisionLanguageService.T("VisionTool.Review.Label.Contrast"));
            AssertResultReviewVisible(
                "Histogram result guidance",
                OpenVisionLanguageService.T("VisionTool.Status.PreviewOk"),
                OpenVisionLanguageService.T("VisionTool.Review.Label.Contrast"));
        }
        else
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(shellHost.ActiveNativeResultReviewText))
        {
            throw new InvalidOperationException(menu + " active document result review text was empty.");
        }
    }

    private static CaptureResult CaptureSimplePreprocessResultReview(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfSimplePreprocessResultReview");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            foreach ((VISION_MENU Menu, string DocumentType, string OutputLayer) scenario in GetSimplePreprocessResultReviewScenarios())
            {
                shellHost.ActivateHostLayerForTest("Main");
                Pump(8);
                shellHost.SelectToolForTest(scenario.Menu);
                Pump(18);
                shellHost.RunActiveNativePreviewForTest();
                Pump(28);

                if (!shellHost.IsNativeDocumentActive
                    || !shellHost.HasNativePreviewResult
                    || !shellHost.ActiveNativeDocumentTypeName.Contains(scenario.DocumentType, StringComparison.Ordinal)
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, scenario.OutputLayer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        scenario.Menu + " did not produce a SimplePreprocess result-review preview. "
                        + $"Document={shellHost.ActiveNativeDocumentTypeName}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                        + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertSimplePreprocessResultReview(scenario.Menu, shellHost);
            }
        });
    }

    private static IEnumerable<(VISION_MENU Menu, string DocumentType, string OutputLayer)> GetPreprocessOutputPreviewScenarios()
    {
        return new[]
        {
            (VISION_MENU.Filter, "FilterToolWpfView", "Filter_Preview"),
            (VISION_MENU.Morphology, "MorphologyToolWpfView", "Morphology_Preview"),
            (VISION_MENU.EdgeDetection, "SimplePreprocessToolWpfView", "EdgeDetection_Preview"),
            (VISION_MENU.RotateAndScale, "SimplePreprocessToolWpfView", "RotateScale_Preview"),
            (VISION_MENU.HSV, "SimplePreprocessToolWpfView", "HSV_Preview"),
            (VISION_MENU.Mean, "SimplePreprocessToolWpfView", "Mean_Preview"),
            (VISION_MENU.Histogram, "SimplePreprocessToolWpfView", "Histogram_Preview")
        };
    }

    private static IEnumerable<(VISION_MENU Menu, string DocumentType, string OutputLayer)> GetSimplePreprocessResultReviewScenarios()
    {
        return GetPreprocessOutputPreviewScenarios()
            .Where(scenario =>
                scenario.Menu == VISION_MENU.HSV
                || scenario.Menu == VISION_MENU.Mean
                || scenario.Menu == VISION_MENU.Histogram);
    }

    private static bool ExercisePreprocessParameterAutoPreviewChange(VISION_MENU menu)
    {
        if (menu == VISION_MENU.EdgeDetection)
        {
            ComboBox edgeTypeCombo = FindFloatingComboBox("cbEdgeType");
            SelectDifferentComboBoxItemText(
                edgeTypeCombo,
                "EdgeDetection type auto-preview combo",
                text => string.Equals(text, "Sobel", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(text, "Canny", StringComparison.OrdinalIgnoreCase));
            return true;
        }

        if (menu == VISION_MENU.Filter)
        {
            ComboBox filterTypeCombo = FindFloatingComboBox("cbFilterType");
            SelectDifferentComboBoxItemText(
                filterTypeCombo,
                "Filter type auto-preview combo",
                text => string.Equals(text, "MedianBlur", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(text, "Blur", StringComparison.OrdinalIgnoreCase));
            return true;
        }

        if (menu == VISION_MENU.Morphology)
        {
            ClickFloatingButtonByName("btnMorphOperationDilate", "Morphology operation auto-preview button");
            return true;
        }

        if (menu == VISION_MENU.RotateAndScale)
        {
            SetDifferentFloatingSliderValueByName("RotateScale angle auto-preview slider", "sliderAngle", 25D, 35D);
            return true;
        }

        if (menu == VISION_MENU.HSV)
        {
            SetDifferentFloatingSliderValueByName("HSV hue min auto-preview slider", "sliderHueMin", 12D, 24D);
            return true;
        }

        if (menu == VISION_MENU.Mean)
        {
            SetDifferentFloatingSliderValueByName("Mean min auto-preview slider", "sliderMeanMin", 80D, 100D);
            return true;
        }

        if (menu == VISION_MENU.Histogram)
        {
            ComboBox histogramTypeCombo = FindFloatingComboBox("cbHistogramType");
            SelectDifferentComboBoxItemText(
                histogramTypeCombo,
                "Histogram type auto-preview combo",
                text => string.Equals(text, "Normalize", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(text, "clahe", StringComparison.OrdinalIgnoreCase));
            return true;
        }

        return false;
    }

    private static CaptureResult CaptureDirectMultiToolInspection(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfDirectMultiToolInspection");
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        shellHost.SetMainLayerImageForTest(matchingBitmap);

        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                // This smoke intentionally avoids AddActiveNativePipelineStepForTest
                // and Pipeline review. It exercises the same direct tool path an
                // operator uses when opening tools and pressing preview one by one.
                RunDirectToolPreview(shellHost, VISION_MENU.Threshold, "ThresholdToolWpfView", "Threshold_Preview");
                RunDirectToolPreview(shellHost, VISION_MENU.Blob, "BlobToolWpfView", "Blob_Preview", "Blob /");
                RunDirectToolPreview(shellHost, VISION_MENU.Contour, "ContourToolWpfView", "Contour_Preview", "Contour /");
                RunDirectToolPreview(
                    shellHost,
                    VISION_MENU.Matching,
                    "MatchingToolWpfView",
                    "Matching_Preview",
                    "Template Match /",
                    () =>
                    {
                        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                        AssertVisionToolComboTemplate(inputLayerCombo, "Direct matching input layer combo");
                        AssertComboBoxPopupLayout(inputLayerCombo, "Direct matching input layer combo");
                        AssertComboBoxSelectionCanChange(inputLayerCombo, "Direct matching input layer combo");
                        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                        AssertVisionToolComboTemplate(outputLayerCombo, "Direct matching output layer combo");
                        AssertComboBoxPopupLayout(outputLayerCombo, "Direct matching output layer combo");
                        shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                        Pump(8);
                        AssertFloatingPropertyGridDialogButtonsReady("Direct matching property grid dialog button");
                    });
                RunDirectToolPreview(
                    shellHost,
                    VISION_MENU.EdgeBasedMatching,
                    "EdgeBasedMatchingToolWpfView",
                    "EdgeBasedMatching_Preview",
                    "Edge Match /",
                    () =>
                    {
                        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                        AssertVisionToolComboTemplate(inputLayerCombo, "Direct edge matching input layer combo");
                        AssertComboBoxPopupLayout(inputLayerCombo, "Direct edge matching input layer combo");
                        AssertComboBoxSelectionCanChange(inputLayerCombo, "Direct edge matching input layer combo");
                        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                        AssertVisionToolComboTemplate(outputLayerCombo, "Direct edge matching output layer combo");
                        AssertComboBoxPopupLayout(outputLayerCombo, "Direct edge matching output layer combo");
                        shellHost.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath);
                        Pump(8);
                        AssertFloatingPropertyGridDialogButtonsReady("Direct edge matching property grid dialog button");
                    });

                if (shellHost.LayerDocumentCount < 6)
                {
                    throw new InvalidOperationException("Direct multi-tool inspection did not create the expected preview layers.");
                }
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static void RunDirectToolPreview(
        OpenVisionShellHostView shellHost,
        VISION_MENU menu,
        string expectedDocumentType,
        string expectedOutputLayer,
        string? requiredResultText = null,
        Action? configure = null)
    {
        shellHost.ActivateHostLayerForTest("Main");
        Pump(6);
        shellHost.SelectToolForTest(menu);
        Pump(16);
        configure?.Invoke();
        shellHost.RunActiveNativePreviewForTest();
        Pump(24);

        if (!shellHost.IsNativeDocumentActive
            || !shellHost.HasNativePreviewResult
            || !string.Equals(shellHost.DirectResultBadgeText, OpenVisionLanguageService.T("Shell.DirectBadgeOk"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException(menu + " direct preview did not complete successfully: " + shellHost.ActiveNativeStatusText);
        }

        if (!shellHost.ActiveNativeDocumentTypeName.Contains(expectedDocumentType, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(menu + " opened the wrong WPF tool view: " + shellHost.ActiveNativeDocumentTypeName);
        }

        if (!shellHost.HasWorkspaceLayerPreview
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, expectedOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " direct preview changed the active input route. "
                + $"Workspace={shellHost.WorkspaceLayerTitle}, Active={shellHost.ActiveHostLayerTitle}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, ExpectedOutput={expectedOutputLayer}");
        }

        if (!string.Equals(shellHost.WorkspaceLayerTitle, expectedOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " direct preview did not switch the visible workspace to the output layer. "
                + $"Workspace={shellHost.WorkspaceLayerTitle}, ExpectedOutput={expectedOutputLayer}, ActiveInput={shellHost.ActiveHostLayerTitle}");
        }

        if (!string.IsNullOrWhiteSpace(requiredResultText))
        {
            AssertResultReviewVisible(menu.ToString(), requiredResultText);
        }
    }

    private static CaptureResult CaptureShellHostLargeImage(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLargeImage", seedMainLayer: false);
        using Bitmap largeBitmap = CreateLargeSmokeBitmap(5200, 5200);
        shellHost.SetMainLayerImageForTest(largeBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            Pump(36);
            if (!shellHost.HasMainLayer || !shellHost.HasWorkspaceLayerPreview || shellHost.WorkspaceTextureTileCount <= 0)
            {
                throw new InvalidOperationException(
                    "Large image did not load into the main OpenGL workspace. "
                    + $"Workspace={shellHost.WorkspaceTextureTileCount}, Layer={shellHost.WorkspaceLayerTitle}");
            }

            if (!shellHost.DockLayerForTest("Main"))
            {
                throw new InvalidOperationException("Large image layer could not be docked.");
            }

            Pump(36);
            if (shellHost.DockedLayerCount < 1 || shellHost.DockedLayerTextureTileCount <= 0)
            {
                throw new InvalidOperationException("Large image did not load in the docked OpenGL viewer.");
            }

            if (!shellHost.OpenLayerViewerForTest("Main"))
            {
                throw new InvalidOperationException("Large image layer could not be opened in a popout viewer.");
            }

            Pump(36);
            bool popoutLoaded = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<OpenVisionLayerViewerView>)
                .Any(viewer => viewer.HasImage && viewer.TextureTileCount > 0);
            if (!popoutLoaded)
            {
                throw new InvalidOperationException("Large image did not load in the popout OpenGL viewer.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLargeImage16KPerf(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLargeImage16KPerf", seedMainLayer: false);
        List<string> report = new();
        Stopwatch total = Stopwatch.StartNew();
        long memoryStart = GetWorkingSetBytes();

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            Stopwatch createWatch = Stopwatch.StartNew();
            Bitmap largeBitmap = CreateLargeSmokeBitmap(16384, 16384);
            createWatch.Stop();
            report.Add("OpenVisionLab 16K image performance smoke");
            report.Add("Image=16384x16384 8bpp grayscale");
            report.Add("CreateImageMs=" + createWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);

            Stopwatch setLayerWatch = Stopwatch.StartNew();
            try
            {
                shellHost.SetMainLayerImageForTest(largeBitmap);
            }
            finally
            {
                largeBitmap.Dispose();
            }

            setLayerWatch.Stop();
            report.Add("SetMainLayerMs=" + setLayerWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);

            Stopwatch workspaceWatch = Stopwatch.StartNew();
            Pump(96);
            workspaceWatch.Stop();
            report.Add("WorkspacePumpMs=" + workspaceWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            report.Add("WorkspaceTiles=" + shellHost.WorkspaceTextureTileCount.ToString(CultureInfo.InvariantCulture));
            WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
            if (!shellHost.HasMainLayer || !shellHost.HasWorkspaceLayerPreview || shellHost.WorkspaceTextureTileCount <= 0)
            {
                WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
                throw new InvalidOperationException(
                    "16K image did not load into the main OpenGL workspace. "
                    + $"Workspace={shellHost.WorkspaceTextureTileCount}, Layer={shellHost.WorkspaceLayerTitle}");
            }

            Stopwatch dockWatch = Stopwatch.StartNew();
            if (!shellHost.DockLayerForTest("Main"))
            {
                WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
                throw new InvalidOperationException("16K image layer could not be docked.");
            }

            Pump(96);
            dockWatch.Stop();
            report.Add("DockMs=" + dockWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            report.Add("DockedTiles=" + shellHost.DockedLayerTextureTileCount.ToString(CultureInfo.InvariantCulture));
            WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
            if (shellHost.DockedLayerCount < 1 || shellHost.DockedLayerTextureTileCount <= 0)
            {
                WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
                throw new InvalidOperationException("16K image did not load in the docked OpenGL viewer.");
            }

            Stopwatch popoutWatch = Stopwatch.StartNew();
            if (!shellHost.OpenLayerViewerForTest("Main"))
            {
                WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
                throw new InvalidOperationException("16K image layer could not be opened in a popout viewer.");
            }

            Pump(96);
            popoutWatch.Stop();
            int popoutTileCount = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<OpenVisionLayerViewerView>)
                .Where(viewer => viewer.HasImage)
                .Sum(viewer => viewer.TextureTileCount);
            if (popoutTileCount <= 0)
            {
                WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
                throw new InvalidOperationException("16K image did not load in the popout OpenGL viewer.");
            }

            report.Add("PopoutMs=" + popoutWatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            report.Add("PopoutTiles=" + popoutTileCount.ToString(CultureInfo.InvariantCulture));
            WriteLargeImagePerfReport(outputPath, report, total.ElapsedMilliseconds, memoryStart);
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerAutoDocking(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerAutoDocking");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            // Historical target name; the current contract is manual docking only.
            Pump(24);
            if (!shellHost.IsWorkspaceLayerDropEnabledForTest)
            {
                throw new InvalidOperationException("Layer workspace is not configured as a drop target.");
            }

            if (!shellHost.HasMainLayer || !shellHost.ActivateHostLayerForTest("Main"))
            {
                throw new InvalidOperationException("Selecting Main did not activate the layer.");
            }

            Pump(20);
            if (shellHost.DockedLayerCount != 0
                || shellHost.IsDockedWorkspaceVisibleForTest
                || !shellHost.IsSingleWorkspaceVisibleForTest)
            {
                throw new InvalidOperationException(
                    "Layer activation must not auto-dock the workspace. "
                    + $"Count={shellHost.DockedLayerCount}, DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, Tiles={shellHost.DockedLayerTextureTileCount}");
            }

            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(32);
            if (!shellHost.HasNativePreviewResult || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Preview execution did not create and select the HSV preview layer.");
            }

            if (shellHost.DockedLayerCount != 0
                || shellHost.IsDockedWorkspaceVisibleForTest
                || !shellHost.IsSingleWorkspaceVisibleForTest)
            {
                throw new InvalidOperationException(
                    "Preview execution must not auto-dock any result layer. "
                    + $"Count={shellHost.DockedLayerCount}, DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.DockLayerForTest("Main"))
            {
                throw new InvalidOperationException("Explicit Main docking did not add the layer.");
            }

            if (!shellHost.SelectHostLayerRowForTest("Main")
                || !shellHost.SelectHostLayerRowForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Layer list selection could not activate Main and HSV_Preview rows.");
            }

            Pump(18);
            if (shellHost.DockedLayerCount != 1 || !shellHost.DockedLayerTitles.Contains("Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Layer list selection must not dock preview results automatically. "
                    + $"Count={shellHost.DockedLayerCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.ActivateHostLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Selecting HSV_Preview did not activate the layer.");
            }

            if (shellHost.DockedLayerCount != 1 || !shellHost.DockedLayerTitles.Contains("Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Selecting a preview result must not auto-dock the output layer. "
                    + $"Count={shellHost.DockedLayerCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.DockLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Explicit HSV_Preview docking did not add the layer.");
            }

            Pump(24);
            if (shellHost.DockedLayerCount != 2
                || shellHost.DockedLayerPaneCount < 2
                || shellHost.DockedLayerTextureTileCount < 2
                || !shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
            {
                throw new InvalidOperationException(
                    "Explicit docking did not create a ready side-by-side comparison workspace. "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}, Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerDockingVertical(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingVertical");
        List<Bitmap> ownedBitmaps = new();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                Bitmap mainLayer = CreateWorkspaceSeedSmokeBitmap();
                Bitmap bottomLayer = CreateDockingPanelSmokeBitmap(1);
                ownedBitmaps.Add(mainLayer);
                ownedBitmaps.Add(bottomLayer);
                shellHost.SetMainLayerImageForTest(mainLayer);
                if (!shellHost.AddLayerImageForTest("Dock_Bottom", bottomLayer))
                {
                    throw new InvalidOperationException("Vertical docking setup could not create Dock_Bottom.");
                }

                Pump(16);
                if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("Dock_Bottom"))
                {
                    throw new InvalidOperationException("Vertical docking setup could not dock Main and Dock_Bottom.");
                }

                if (!shellHost.ArrangeDockedLayerPanesForTest("Vertical", "Main", "Dock_Bottom"))
                {
                    throw new InvalidOperationException("Vertical docking arrangement failed.");
                }

                Pump(40);
                AssertDockedLayerLayout(shellHost, 2, "Vertical", "vertical bottom-docking");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            foreach (Bitmap bitmap in ownedBitmaps)
            {
                bitmap.Dispose();
            }
        }
    }

    private static CaptureResult CaptureShellHostLayerDockingNPanels(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingNPanels");
        List<Bitmap> ownedBitmaps = new();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                string[] titles = { "Main", "Dock_A", "Dock_B", "Dock_C" };
                for (int i = 1; i < titles.Length; i++)
                {
                    Bitmap bitmap = CreateDockingPanelSmokeBitmap(i);
                    ownedBitmaps.Add(bitmap);
                    if (!shellHost.AddLayerImageForTest(titles[i], bitmap))
                    {
                        throw new InvalidOperationException("Could not create docking test layer: " + titles[i]);
                    }
                }

                foreach (string title in titles)
                {
                    if (!shellHost.DockLayerForTest(title))
                    {
                        throw new InvalidOperationException("Could not dock layer: " + title);
                    }
                }

                Pump(40);
                if (!shellHost.ArrangeDockedLayerPanesForTest("Horizontal", titles))
                {
                    throw new InvalidOperationException("N-panel horizontal docking arrangement failed.");
                }

                Pump(32);
                AssertDockedLayerLayout(shellHost, 4, "Horizontal", "n-panel horizontal docking");

                if (!shellHost.ArrangeDockedLayerPanesForTest("Vertical", titles))
                {
                    throw new InvalidOperationException("N-panel vertical docking arrangement failed.");
                }

                Pump(32);
                AssertDockedLayerLayout(shellHost, 4, "Vertical", "n-panel vertical docking");

                if (!shellHost.ArrangeDockedLayerPanesForTest("Horizontal", titles))
                {
                    throw new InvalidOperationException("N-panel final horizontal docking arrangement failed.");
                }

                Pump(32);
                AssertDockedLayerLayout(shellHost, 4, "Horizontal", "n-panel final docking");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            foreach (Bitmap bitmap in ownedBitmaps)
            {
                bitmap.Dispose();
            }
        }
    }

    private static CaptureResult CaptureShellHostLayerDockingGrid(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingGrid");
        List<Bitmap> ownedBitmaps = new();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                string[] titles = { "Main", "Dock_TopRight", "Dock_BottomLeft", "Dock_BottomRight" };
                for (int i = 1; i < titles.Length; i++)
                {
                    Bitmap bitmap = CreateDockingPanelSmokeBitmap(i + 10);
                    ownedBitmaps.Add(bitmap);
                    if (!shellHost.AddLayerImageForTest(titles[i], bitmap))
                    {
                        throw new InvalidOperationException("Could not create grid docking test layer: " + titles[i]);
                    }
                }

                foreach (string title in titles)
                {
                    if (!shellHost.DockLayerForTest(title))
                    {
                        throw new InvalidOperationException("Could not dock grid layer: " + title);
                    }
                }

                Pump(40);
                if (!shellHost.ArrangeDockedLayerGridForTest(titles))
                {
                    throw new InvalidOperationException("2x2 grid docking arrangement failed.");
                }

                Pump(48);
                AssertDockedLayerGridLayout(shellHost, 4, "2x2 grid docking");

                string[] reorderedTitles = { "Dock_BottomRight", "Main", "Dock_TopRight", "Dock_BottomLeft" };
                if (!shellHost.ArrangeDockedLayerGridForTest(reorderedTitles))
                {
                    throw new InvalidOperationException("2x2 grid docking rearrangement failed.");
                }

                Pump(48);
                AssertDockedLayerGridLayout(shellHost, 4, "2x2 grid docking after rearrange");
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            foreach (Bitmap bitmap in ownedBitmaps)
            {
                bitmap.Dispose();
            }
        }
    }

    private static CaptureResult CaptureShellHostLayerDockingTabs(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingTabs");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Layer docking tab setup did not create the HSV preview layer.");
            }

            if (!shellHost.AreHostLayerTabsReadableForTest
                || !shellHost.HostLayerTabTextsForTest.Contains("Main", StringComparison.OrdinalIgnoreCase)
                || !shellHost.HostLayerTabTextsForTest.Contains("HSV_Preview", StringComparison.OrdinalIgnoreCase)
                || shellHost.HostLayerTabTextsForTest.Contains("00 - none", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Central layer tabs are not readable or contain a placeholder row. Tabs=" + shellHost.HostLayerTabTextsForTest);
            }

            if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Layer docking tab setup could not dock both layers.");
            }

            Pump(20);
            if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Layer docking tab setup could not merge both layers into one tabbed pane.");
            }

            Pump(40);
            if (shellHost.DockedLayerCount != 2
                || shellHost.DockedLayerPaneCount != 1
                || shellHost.DockedLayerTextureTileCount < 2
                || !shellHost.AreDockedLayerTabHeadersGestureReadyForTest
                || !shellHost.AreDockedLayerTabHeadersReadableForTest
                || !shellHost.AreDockedLayerTabHeaderGripsReadyForTest)
            {
                throw new InvalidOperationException(
                    "Layer docking tabs are not ready in a single pane. "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}, Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostToolRailCompact(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostToolRailCompact");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            Pump(16);
            double expandedWidth = shellHost.ToolRailWidthForTest;
            if (expandedWidth < 200)
            {
                throw new InvalidOperationException("Tool rail did not start expanded. Width=" + expandedWidth.ToString(CultureInfo.InvariantCulture));
            }

            shellHost.ToggleToolRailForTest();
            Pump(24);
            if (!shellHost.IsToolRailCompactForTest
                || shellHost.ToolRailWidthForTest > 60
                || shellHost.IsToolRailNavigationVisibleForTest
                || !shellHost.IsToolRailCompactLabelHiddenForTest)
            {
                throw new InvalidOperationException(
                    "Tool rail compact mode did not collapse to a clean expander handle. "
                    + $"Width={shellHost.ToolRailWidthForTest.ToString(CultureInfo.InvariantCulture)}, NavVisible={shellHost.IsToolRailNavigationVisibleForTest}, LabelHidden={shellHost.IsToolRailCompactLabelHiddenForTest}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerDocking(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDocking");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Layer docking setup did not create the HSV preview layer.");
            }

            bool dockedMain = shellHost.DockLayerForTest("Main");
            bool dockedPreview = shellHost.DockLayerForTest("HSV_Preview");
            Pump(20);
            if (!dockedMain || !dockedPreview || shellHost.DockedLayerCount != 2)
            {
                throw new InvalidOperationException("Layer docking did not add both layers to the workspace.");
            }

            if (!shellHost.DockedLayerTitles.Contains("Main", StringComparison.Ordinal)
                || !shellHost.DockedLayerTitles.Contains("HSV_Preview", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Layer docking titles were not retained: " + shellHost.DockedLayerTitles);
            }

            if (shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException("Layer docking did not load multiple OpenGL layer textures.");
            }

            if (!shellHost.AreDockedLayerViewersCompactForTest)
            {
                throw new InvalidOperationException("Layer docking still shows duplicated viewer header/status chrome inside docked panes.");
            }

            if (!shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
            {
                throw new InvalidOperationException(
                    "Layer docking tab headers are not gesture-ready. "
                    + $"Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (shellHost.DockedLayerPaneCount < 2)
            {
                throw new InvalidOperationException("Layer docking did not place the second layer into a separate docking pane.");
            }

            Pump(20);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException("Layer docking split lost one of the docked layer views.");
            }

            string savePath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_docked_layer_save_smoke.png");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            if (!shellHost.SaveDockedLayerImageToFileForTest("HSV_Preview", savePath)
                || !File.Exists(savePath)
                || new FileInfo(savePath).Length == 0)
            {
                throw new InvalidOperationException("Layer docking did not save the docked OpenGL image source.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerDockingFunctional(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingFunctional");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Layer docking functional setup did not create the HSV preview layer.");
            }

            bool dockedMain = shellHost.DockLayerForTest("Main");
            bool dockedPreview = shellHost.DockLayerForTest("HSV_Preview");
            Pump(20);
            if (!dockedMain || !dockedPreview || shellHost.DockedLayerCount != 2)
            {
                throw new InvalidOperationException("Layer docking functional check could not dock both layers.");
            }

            if (!shellHost.AreDockedLayersNativeFloatingDisabledForTest)
            {
                throw new InvalidOperationException("Docked layer panels still allow native AvalonDock floating; OpenVisionLab guide docking should own layer movement.");
            }

            if (!shellHost.AreDockedLayerViewersCompactForTest)
            {
                throw new InvalidOperationException("Docked layer viewers are not in compact chrome mode.");
            }

            if (!shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
            {
                throw new InvalidOperationException(
                    "Docked layer tab headers do not expose a clear drag affordance. "
                    + $"Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.AreDockedLayerTabHeaderGripsReadyForTest)
            {
                throw new InvalidOperationException("Docked layer tab headers did not render drag grip icons.");
            }

            shellHost.ShowDockingGuideForTest(0.10D, 0.50D);
            Pump(20);
            if (!shellHost.IsDockingGuideOverlayVisibleForTest || shellHost.DockingGuideZoneCountForTest < 5)
            {
                throw new InvalidOperationException("Docked layer drag guide did not become visible with all dock zones.");
            }

            shellHost.HideDockingGuideForTest();
            Pump(20);
            if (shellHost.IsDockingGuideOverlayVisibleForTest)
            {
                throw new InvalidOperationException("Docked layer drag guide did not hide after the drag ended.");
            }

            if (shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException("Layer docking functional check should start as same-pane tabs before the operator chooses a split zone.");
            }

            if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "Right"))
            {
                throw new InvalidOperationException("Docked layer could not be moved into the right guide zone.");
            }

            Pump(12);
            if (shellHost.DockedLayerPaneCount < 2)
            {
                throw new InvalidOperationException("Layer docking functional check did not split after dropping on the right guide zone.");
            }

            bool duplicateDock = shellHost.DockLayerForTest("HSV_Preview");
            Pump(12);
            if (!duplicateDock || shellHost.DockedLayerCount != 2 || shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException("Repeated dock action changed or lost an existing docked layer.");
            }

            if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Docked layer could not be moved back to the primary pane.");
            }

            Pump(12);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException("Docked layer primary-pane move did not merge the layout cleanly.");
            }

            if (!shellHost.SplitDockedLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Docked layer could not be split back to a second pane.");
            }

            Pump(12);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerPaneCount < 2 || !shellHost.AreDockedLayersNativeFloatingDisabledForTest)
            {
                throw new InvalidOperationException("Docked layer split/native-floating-disabled state was not preserved after re-docking.");
            }

            shellHost.SaveDockingWorkspaceStateForTest();
            Pump(12);
            if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Docking layout persistence setup could not merge the preview layer.");
            }

            Pump(12);
            if (shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException("Docking layout persistence setup did not create a single-pane baseline.");
            }

            if (!shellHost.RestoreDockingLayoutStateForTest())
            {
                throw new InvalidOperationException("Saved docking layout could not be restored.");
            }

            Pump(24);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerPaneCount < 2 || shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException("Saved docking layout did not restore the split layer workspace.");
            }

            shellHost.ClearDockedLayersForTest();
            Pump(12);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException("Workspace clear should reset comparison panes while keeping live layer tabs visible.");
            }

            if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "Right"))
            {
                throw new InvalidOperationException("Docked layer could not be split again after clearing the comparison layout.");
            }

            Pump(24);
            if (shellHost.DockedLayerCount != 2 || shellHost.DockedLayerPaneCount < 2 || shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException(
                    "Docked layers were not restored after clear and re-dock. "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Textures={shellHost.DockedLayerTextureTileCount}, Titles={shellHost.DockedLayerTitles}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerDockingGuideVisible(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingGuideVisible");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Layer docking guide setup did not create a preview layer.");
            }

            bool dockedMain = shellHost.DockLayerForTest("Main");
            bool dockedPreview = shellHost.DockLayerForTest("HSV_Preview");
            Pump(20);
            if (!dockedMain || !dockedPreview || shellHost.DockedLayerCount != 2)
            {
                throw new InvalidOperationException("Layer docking guide setup could not create the layer tabs.");
            }

            if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "Right"))
            {
                throw new InvalidOperationException("Layer docking guide setup could not split via the right guide zone.");
            }

            Pump(12);
            if (shellHost.DockedLayerPaneCount < 2)
            {
                throw new InvalidOperationException("Layer docking guide setup did not create a split comparison layout.");
            }

            shellHost.ShowDockingGuideForTest(0.50D, 0.50D);
            Pump(20);
            if (!shellHost.IsDockingGuideOverlayVisibleForTest || shellHost.DockingGuideZoneCountForTest < 5)
            {
                throw new InvalidOperationException("Docked layer docking guide did not remain visible for capture.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerGlobalDocking(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerGlobalDocking");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Global docking setup did not create a preview layer.");
            }

            if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Global docking setup could not dock both layers.");
            }

            Pump(20);
            if (shellHost.DockedLayerPaneCount != 1)
            {
                throw new InvalidOperationException("Global docking setup should start as a single tabbed pane.");
            }

            if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
            {
                throw new InvalidOperationException("Global docking could not move the layer to the workspace right zone.");
            }

            Pump(24);
            if (shellHost.DockedLayerCount != 2
                || shellHost.DockedLayerPaneCount < 2
                || !string.Equals(shellHost.DockedLayerRootOrientationForTest, "Horizontal", StringComparison.OrdinalIgnoreCase)
                || shellHost.DockedLayerTextureTileCount < 2)
            {
                throw new InvalidOperationException(
                    "Global right docking did not create a workspace-level horizontal split. "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Orientation={shellHost.DockedLayerRootOrientationForTest}, Tiles={shellHost.DockedLayerTextureTileCount}");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostLayerBottomDockingSemantics(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerBottomDockingSemantics");
        List<Bitmap> ownedBitmaps = new();
        try
        {
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.HSV);
                Pump(16);
                shellHost.RunActiveNativePreviewForTest();
                Pump(24);

                if (!shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Bottom docking semantics setup did not create a preview layer.");
                }

                if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Bottom docking semantics setup could not dock Main and HSV_Preview.");
                }

                Pump(24);
                if (shellHost.DockedLayerPaneCount != 1)
                {
                    throw new InvalidOperationException("Bottom docking semantics setup should start as one tabbed pane.");
                }

                shellHost.ShowDockingGuideForTest(0.50D, 0.61D);
                Pump(16);
                if (!string.Equals(shellHost.ActiveDockingGuideZoneForTest, "Bottom", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Pane-local bottom guide should resolve to Bottom near the local compass, not to a workspace-global zone. "
                        + $"ActiveZone={shellHost.ActiveDockingGuideZoneForTest}");
                }

                shellHost.HideDockingGuideForTest();
                Pump(8);
                shellHost.ShowDockingGuideForTest(0.50D, 0.98D);
                Pump(16);
                if (!string.Equals(shellHost.ActiveDockingGuideZoneForTest, "GlobalBottom", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Workspace edge bottom guide should resolve to GlobalBottom. "
                        + $"ActiveZone={shellHost.ActiveDockingGuideZoneForTest}");
                }

                shellHost.HideDockingGuideForTest();
                Pump(8);
                if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalBottom"))
                {
                    throw new InvalidOperationException("GlobalBottom command could not dock HSV_Preview to the workspace bottom.");
                }

                Pump(24);
                if (shellHost.DockedLayerPaneCount < 2
                    || !string.Equals(shellHost.DockedLayerRootOrientationForTest, "Vertical", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "GlobalBottom should create a workspace-level vertical split. "
                        + $"Panes={shellHost.DockedLayerPaneCount}, Orientation={shellHost.DockedLayerRootOrientationForTest}, Titles={shellHost.DockedLayerTitles}");
                }

                if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Bottom docking semantics setup could not merge HSV_Preview back to the primary pane.");
                }

                Pump(16);
                Bitmap localBottomBitmap = CreateDockingPanelSmokeBitmap(42);
                ownedBitmaps.Add(localBottomBitmap);
                if (!shellHost.AddLayerImageForTest("Dock_LocalBottom", localBottomBitmap)
                    || !shellHost.DockLayerForTest("Dock_LocalBottom"))
                {
                    throw new InvalidOperationException("Bottom docking semantics setup could not create Dock_LocalBottom.");
                }

                Pump(24);
                if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
                {
                    throw new InvalidOperationException("Bottom docking semantics setup could not create the workspace-level right split.");
                }

                Pump(24);
                if (!string.Equals(shellHost.DockedLayerRootOrientationForTest, "Horizontal", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "GlobalRight should create a workspace-level horizontal split before local bottom docking. "
                        + $"Orientation={shellHost.DockedLayerRootOrientationForTest}, Titles={shellHost.DockedLayerTitles}");
                }

                if (!shellHost.DockLayerToGuideZoneForTest("Dock_LocalBottom", "Bottom"))
                {
                    throw new InvalidOperationException("Pane-local Bottom command could not dock Dock_LocalBottom under the target pane.");
                }

                Pump(40);
                AssertDockedLayerLayout(shellHost, 3, "Horizontal", "pane-local bottom docking semantics");
                if (shellHost.DockedLayerNestedLayoutPanelCountForTest < 1)
                {
                    throw new InvalidOperationException(
                        "Pane-local Bottom should create a nested vertical split inside the target pane, not a workspace-wide bottom row. "
                        + $"NestedPanels={shellHost.DockedLayerNestedLayoutPanelCountForTest}, Panes={shellHost.DockedLayerPaneCount}, Titles={shellHost.DockedLayerTitles}");
                }
            }, captureFloatingToolWindow: false);
        }
        finally
        {
            foreach (Bitmap bitmap in ownedBitmaps)
            {
                bitmap.Dispose();
            }
        }
    }

    private static CaptureResult CaptureShellHostLayerDockingPersistence(string outputPath)
    {
        return WithDockingStateFileBackup(() =>
        {
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerDockingPersistence");
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.HSV);
                Pump(16);
                shellHost.RunActiveNativePreviewForTest();
                Pump(24);

                if (!shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Docking persistence setup did not create a preview layer.");
                }

                if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Docking persistence setup could not dock both layers.");
                }

                Pump(20);
                if (!shellHost.DockLayerToGuideZoneForTest("HSV_Preview", "GlobalRight"))
                {
                    throw new InvalidOperationException("Docking persistence setup could not create a saved split layout.");
                }

                Pump(24);
                if (shellHost.DockedLayerPaneCount < 2
                    || !string.Equals(shellHost.DockedLayerRootOrientationForTest, "Horizontal", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Docking persistence setup did not reach the expected horizontal split. "
                        + $"Panes={shellHost.DockedLayerPaneCount}, Orientation={shellHost.DockedLayerRootOrientationForTest}");
                }

                shellHost.SaveDockingWorkspaceStateForTest();
                if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
                {
                    throw new InvalidOperationException("Docking persistence setup could not replace the split with a tabbed pane.");
                }

                Pump(12);
                if (shellHost.DockedLayerPaneCount != 1)
                {
                    throw new InvalidOperationException("Docking persistence setup did not produce the single-pane baseline.");
                }

                if (!shellHost.RestoreDockingLayoutStateForTest())
                {
                    throw new InvalidOperationException("Docking persistence restore command returned false.");
                }

                Pump(24);
                if (shellHost.DockedLayerCount != 2
                    || shellHost.DockedLayerPaneCount < 2
                    || !string.Equals(shellHost.DockedLayerRootOrientationForTest, "Horizontal", StringComparison.OrdinalIgnoreCase)
                    || shellHost.DockedLayerTextureTileCount < 2)
                {
                    throw new InvalidOperationException(
                        "Docking persistence did not restore the saved split workspace. "
                        + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, "
                        + $"Orientation={shellHost.DockedLayerRootOrientationForTest}, Tiles={shellHost.DockedLayerTextureTileCount}, Titles={shellHost.DockedLayerTitles}");
                }
            }, captureFloatingToolWindow: false);
        });
    }

    private static CaptureResult CaptureShellHostLayerTabDragGuideVisible(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerTabDragGuideVisible");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Layer tab drag guide setup did not create a preview layer.");
            }

            if (!shellHost.DockLayerForTest("Main") || !shellHost.DockLayerForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Layer tab drag guide setup could not dock both layers.");
            }

            Pump(20);
            if (!shellHost.MoveDockedLayerToPrimaryPaneForTest("HSV_Preview"))
            {
                throw new InvalidOperationException("Layer tab drag guide setup could not merge both layers into one tabbed pane.");
            }

            Pump(40);
            if (shellHost.DockedLayerPaneCount != 1
                || shellHost.DockedLayerTabHeaderCount < 2
                || !shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
            {
                throw new InvalidOperationException(
                    "Layer tab drag guide setup did not produce a two-tab docked pane. "
                    + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Headers={shellHost.DockedLayerTabHeaderCount}, Titles={shellHost.DockedLayerTitles}");
            }

            if (!shellHost.ShowDockedLayerTabDragGuideForTest())
            {
                throw new InvalidOperationException("Docked layer tab header was not recognized as a docking gesture source.");
            }

            Pump(20);
            if (!shellHost.IsDockingGuideOverlayVisibleForTest || shellHost.DockingGuideZoneCountForTest < 5)
            {
                throw new InvalidOperationException("Docked layer tab drag guide did not remain visible for capture.");
            }
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostNativeTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostNative");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF HSV tool did not produce a preview result.");
            }
        });
    }

    private static CaptureResult CaptureShellHostLayerPopout(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLayerPopout");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.HSV);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult || !string.Equals(shellHost.WorkspaceLayerTitle, "HSV_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Layer popout setup did not create the HSV preview layer.");
            }

            bool openedMain = shellHost.OpenLayerViewerForTest("Main");
            bool openedPreview = shellHost.OpenLayerViewerForTest("HSV_Preview");
            Pump(20);
            if (!openedMain || !openedPreview || shellHost.OpenLayerViewerWindowCount < 2)
            {
                throw new InvalidOperationException("Layer popout did not open two independent layer viewer windows.");
            }

            if (!shellHost.OpenLayerViewerWindowTitles.Contains("Main", StringComparison.Ordinal)
                || !shellHost.OpenLayerViewerWindowTitles.Contains("HSV_Preview", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Layer popout windows did not retain layer-specific titles: " + shellHost.OpenLayerViewerWindowTitles);
            }

            bool previewWindowHasImage = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible
                    && item.GetType().Name == "OpenVisionFloatingToolWindow"
                    && item.Title.Contains("HSV_Preview", StringComparison.Ordinal))
                .SelectMany(FindVisualChildren<OpenVisionLayerViewerView>)
                .Any(item => item.HasImage && item.TextureTileCount > 0);
            if (!previewWindowHasImage)
            {
                throw new InvalidOperationException("Layer popout did not load the output layer OpenGL texture.");
            }
        });
    }

    private static void AssertDockedLayerLayout(OpenVisionShellHostView shellHost, int expectedCount, string expectedOrientation, string scenario)
    {
        if (shellHost.DockedLayerCount != expectedCount
            || shellHost.DockedLayerPaneCount < expectedCount
            || shellHost.DockedLayerTextureTileCount < expectedCount)
        {
            throw new InvalidOperationException(
                "Docking layout did not keep all expected layer panes for " + scenario + ". "
                + $"Count={shellHost.DockedLayerCount}, Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}, Titles={shellHost.DockedLayerTitles}");
        }

        if (!string.Equals(shellHost.DockedLayerRootOrientationForTest, expectedOrientation, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Docking root orientation mismatch for " + scenario + ". "
                + $"Expected={expectedOrientation}, Actual={shellHost.DockedLayerRootOrientationForTest}");
        }

        if (!shellHost.AreDockedLayerViewersCompactForTest || !shellHost.AreDockedLayerViewersCompactSizeReadyForTest)
        {
            throw new InvalidOperationException("Docked layer viewers are not in compact small-pane mode for " + scenario + ".");
        }

        if (!shellHost.AreDockedLayerTabHeadersGestureReadyForTest)
        {
            throw new InvalidOperationException("Docked layer headers are not drag-ready for " + scenario + ".");
        }

        List<OpenVisionLayerViewerView> allViewers = FindVisualChildren<OpenVisionLayerViewerView>(shellHost)
            .ToList();
        List<OpenVisionLayerViewerView> viewers = allViewers
            .Where(viewer => viewer.IsVisible && viewer.HasImage)
            .ToList();
        if (viewers.Count < expectedCount)
        {
            throw new InvalidOperationException(
                "Visual tree does not contain all docked layer viewers for " + scenario + ". "
                + $"VisibleImageViewers={viewers.Count}, VisualViewers={allViewers.Count}, "
                + $"ImageViewers={allViewers.Count(viewer => viewer.HasImage)}, Expected={expectedCount}, DockedCount={shellHost.DockedLayerCount}, "
                + $"Panes={shellHost.DockedLayerPaneCount}, Tiles={shellHost.DockedLayerTextureTileCount}, "
                + $"DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, "
                + $"Titles={shellHost.DockedLayerTitles}");
        }

        foreach (OpenVisionLayerViewerView viewer in viewers)
        {
            if (viewer.ActualWidth < 220 || viewer.ActualHeight < 120)
            {
                throw new InvalidOperationException(
                    "Docked layer viewer collapsed too far for " + scenario + ". "
                    + $"Title={viewer.LayerTitle}, Size={viewer.ActualWidth:0}x{viewer.ActualHeight:0}");
            }
        }

        List<Rect> viewerBounds = viewers
            .Select(viewer =>
            {
                Point topLeft = viewer.TranslatePoint(new Point(0, 0), shellHost);
                return new Rect(topLeft, new Size(viewer.ActualWidth, viewer.ActualHeight));
            })
            .ToList();
        for (int i = 0; i < viewerBounds.Count; i++)
        {
            Rect bounds = viewerBounds[i];
            if (double.IsNaN(bounds.X)
                || double.IsNaN(bounds.Y)
                || bounds.Width <= 0
                || bounds.Height <= 0)
            {
                throw new InvalidOperationException("Docked layer viewer has invalid layout bounds for " + scenario + ".");
            }

            if (bounds.Left < -2
                || bounds.Top < -2
                || bounds.Right > shellHost.ActualWidth + 2
                || bounds.Bottom > shellHost.ActualHeight + 2)
            {
                throw new InvalidOperationException(
                    "Docked layer viewer escaped the shell bounds for " + scenario + ". "
                    + $"Bounds={bounds.Left:0},{bounds.Top:0},{bounds.Width:0}x{bounds.Height:0}");
            }

            for (int j = i + 1; j < viewerBounds.Count; j++)
            {
                Rect intersection = Rect.Intersect(bounds, viewerBounds[j]);
                if (!intersection.IsEmpty && intersection.Width > 8 && intersection.Height > 8)
                {
                    throw new InvalidOperationException(
                        "Docked layer viewers overlap in the workspace for " + scenario + ". "
                        + $"A={bounds.Left:0},{bounds.Top:0},{bounds.Width:0}x{bounds.Height:0}; "
                        + $"B={viewerBounds[j].Left:0},{viewerBounds[j].Top:0},{viewerBounds[j].Width:0}x{viewerBounds[j].Height:0}; "
                        + $"Overlap={intersection.Width:0}x{intersection.Height:0}");
                }
            }
        }
    }

    private static void AssertDockedLayerGridLayout(OpenVisionShellHostView shellHost, int expectedCount, string scenario)
    {
        AssertDockedLayerLayout(shellHost, expectedCount, "Vertical", scenario);

        if (shellHost.DockedLayerNestedLayoutPanelCountForTest < 2)
        {
            throw new InvalidOperationException(
                "Docking layout did not create nested row panels for " + scenario + ". "
                + $"NestedPanels={shellHost.DockedLayerNestedLayoutPanelCountForTest}, Titles={shellHost.DockedLayerTitles}");
        }
    }

    private static CaptureResult CaptureShellHostThresholdTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostThreshold");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);
            ComboBox? inputLayerCombo = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<ComboBox>)
                .FirstOrDefault(item => string.Equals(item.Name, "cbInputLayer", StringComparison.Ordinal));
            if (inputLayerCombo == null || inputLayerCombo.Items.Count == 0)
            {
                throw new InvalidOperationException("Threshold layer combo did not expose items.");
            }

            inputLayerCombo.IsDropDownOpen = true;
            Pump(6);
            if (!inputLayerCombo.IsDropDownOpen)
            {
                throw new InvalidOperationException("Threshold layer combo did not open.");
            }

            inputLayerCombo.IsDropDownOpen = false;
            RadioButton? adaptiveMode = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<RadioButton>)
                .FirstOrDefault(item => string.Equals(item.Name, "rbAdaptive", StringComparison.Ordinal));
            RadioButton? basicMode = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<RadioButton>)
                .FirstOrDefault(item => string.Equals(item.Name, "rbBasic", StringComparison.Ordinal));
            RadioButton? rangeMode = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<RadioButton>)
                .FirstOrDefault(item => string.Equals(item.Name, "rbRange", StringComparison.Ordinal));
            RadioButton? gaussianMethod = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<RadioButton>)
                .FirstOrDefault(item => string.Equals(item.Name, "rbAdaptiveGaussian", StringComparison.Ordinal));
            Slider? blockSlider = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<Slider>)
                .FirstOrDefault(item => string.Equals(item.Name, "sliderBlockSize", StringComparison.Ordinal));
            if (basicMode == null || adaptiveMode == null || rangeMode == null || gaussianMethod == null || blockSlider == null)
            {
                throw new InvalidOperationException("Threshold adaptive controls were not found.");
            }

            ThresholdToolWpfView? thresholdView = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<ThresholdToolWpfView>)
                .FirstOrDefault();
            if (thresholdView == null)
            {
                throw new InvalidOperationException("Threshold WPF view was not found.");
            }

            thresholdView.ConfigureBasicInvertForTest(false);
            Pump(12);
            AssertFloatingNamedElementsDoNotOverlap(
                "Threshold basic slider layout",
                "txtThreshold",
                "txtMaxValue",
                "sliderThreshold");
            rangeMode.IsChecked = true;
            Pump(12);
            AssertFloatingNamedElementsDoNotOverlap(
                "Threshold range slider layout",
                "txtRangeMin",
                "sliderRangeMin",
                "txtRangeMax",
                "sliderRangeMax");

            int beforeRuns = shellHost.NativePreviewRunCount;
            adaptiveMode.IsChecked = true;
            gaussianMethod.IsChecked = true;
            blockSlider.Value = 31;
            Thread.Sleep(180);
            Pump(24);
            AssertFloatingNamedElementsDoNotOverlap(
                "Threshold adaptive block slider layout",
                "txtBlockSize",
                "sliderBlockSize");
            AssertFloatingSlidersHaveBreathingRoom("Threshold");
            if (!shellHost.IsNativeDocumentActive
                || !shellHost.HasNativePreviewResult
                || shellHost.NativePreviewRunCount <= beforeRuns)
            {
                throw new InvalidOperationException("Native WPF Threshold tool did not auto-preview from adaptive control changes: " + shellHost.ActiveNativeStatusText);
            }

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("ThresholdToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Threshold did not open the WPF Threshold tool view.");
            }

            if (!FloatingToolTextContains(OpenVisionLanguageService.T("VisionMenu.Threshold"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("Threshold.ModeBasic"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("Threshold.ModeRange"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("Threshold.ModeAdaptive"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("Threshold.BinaryInv"))
                || !FloatingToolTextContains(OpenVisionLanguageService.T("Threshold.GaussianC"))
                || !FloatingToolTextContains("Block 31"))
            {
                throw new InvalidOperationException("Threshold WPF tool did not expose the modern mode selector.");
            }

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "Threshold", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Threshold_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.ContainsKey("Mode")
                || !step.Parameters.TryGetValue("Mode", out string? mode)
                || !string.Equals(mode, "Adaptive", StringComparison.Ordinal)
                || !step.Parameters.ContainsKey("ThresholdType")
                || !step.Parameters.ContainsKey("RangeMin")
                || !step.Parameters.TryGetValue("AdaptiveType", out string? adaptiveType)
                || !string.Equals(adaptiveType, "GaussianC", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("BlockSize", out string? blockSize)
                || !string.Equals(blockSize, "31", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Threshold WPF tool did not create a valid pipeline step.");
            }

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Threshold floating tool did not accept the dock-to-right request.");
            }

            Pump(24);
            AssertDockedSingleInputToolLayout("Docked Threshold tool layout");
            AssertActiveToolNamedElementsDoNotOverlap(
                "Docked Threshold adaptive layout",
                "txtBlockSize",
                "sliderBlockSize");
            AssertActiveToolNamedElementsVisibleWithinAncestors(
                "Docked Threshold adaptive controls",
                "rbBasic",
                "rbRange",
                "rbAdaptive",
                "txtBlockSize",
                "sliderBlockSize");
            AssertFloatingSlidersHaveBreathingRoom("Docked Threshold");
        });
    }

    private static CaptureResult CaptureShellHostThresholdBasicTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostThresholdBasic");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF Threshold basic tool did not produce a preview result: " + shellHost.ActiveNativeStatusText);
            }

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("ThresholdToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Threshold basic did not open the WPF Threshold tool view.");
            }

            if (!FloatingToolTextContains("Basic / T"))
            {
                throw new InvalidOperationException("Threshold basic summary was not visible.");
            }
        });
    }

    private static CaptureResult CaptureShellHostRotateScaleTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostRotateScale");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.RotateAndScale);
            Pump(16);
            Slider? angleSlider = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<Slider>)
                .FirstOrDefault(item => string.Equals(item.Name, "sliderAngle", StringComparison.Ordinal));
            if (angleSlider == null)
            {
                throw new InvalidOperationException("RotateScale angle slider was not found.");
            }

            angleSlider.Value = 25;
            Pump(16);
            AssertFloatingSlidersHaveBreathingRoom("RotateScale");
            TextBox? angleTextBox = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<TextBox>)
                .FirstOrDefault(item => string.Equals(item.Name, "txtAngle", StringComparison.Ordinal));
            if (angleTextBox == null || !angleTextBox.Text.Contains("25", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("RotateScale slider did not update the value text.");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            if (!shellHost.IsNativeDocumentActive
                || !shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "RotateScale_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "RotateScale preview did not create the expected output layer. "
                    + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertFloatingInlinePreviewSlotCount("RotateScale", 2);
        });
    }

    private static CaptureResult CaptureFilterMorphologyLayoutGuard(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfFilterMorphologyLayoutGuard");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            // This guard checks actual WPF bounds, not only screenshots, so visible control overlaps fail the smoke.
            shellHost.SelectToolForTest(VISION_MENU.Filter);
            Pump(20);
            ComboBox filterTypeCombo = FindFloatingComboBox("cbFilterType");
            SelectComboBoxItemText(filterTypeCombo, "Blur", "Filter type combo");
            Pump(12);
            AssertFloatingNamedElementsDoNotOverlap(
                "Filter blur kernel layout",
                "panelWidth",
                "panelHeight",
                "panelKernelPresets");
            ClickFloatingButtonByName("btnFilterKernelPreset5", "Filter kernel preset button");

            SelectComboBoxItemText(filterTypeCombo, "MedianBlur", "Filter type combo");
            Pump(12);
            AssertFloatingNamedElementsDoNotOverlap(
                "Filter median kernel layout",
                "panelMedian",
                "panelKernelPresets");

            SelectComboBoxItemText(filterTypeCombo, "BilateralFilter", "Filter type combo");
            Pump(12);
            AssertFloatingNamedElementsDoNotOverlap(
                "Filter bilateral kernel layout",
                "panelDiameter",
                "panelSigmaColor",
                "panelSigmaSpace");

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Filter floating tool did not accept the dock-to-right request.");
            }

            Pump(24);
            AssertDockedSingleInputToolLayout("Docked Filter tool layout");
            AssertActiveToolNamedElementsDoNotOverlap(
                "Docked Filter bilateral kernel layout",
                "panelDiameter",
                "panelSigmaColor",
                "panelSigmaSpace");
            AssertActiveToolNamedElementsVisibleWithinAncestors(
                "Docked Filter required controls",
                "gbFilterOptions",
                "gbKernel",
                "panelDiameter",
                "panelSigmaColor",
                "panelSigmaSpace");

            shellHost.SelectToolForTest(VISION_MENU.Morphology);
            Pump(20);
            AssertDockedSingleInputToolLayout("Docked Morphology tool layout");
            ClickFloatingButtonByName("btnKernelPreset5", "Morphology kernel preset button");
            AssertActiveToolNamedElementsDoNotOverlap(
                "Docked Morphology kernel layout",
                "panelKernelWidth",
                "panelKernelHeight",
                "panelKernelLock",
                "panelKernelPresets",
                "panelShape");
            AssertActiveToolNamedElementsVisibleWithinAncestors(
                "Docked Morphology kernel required controls",
                "panelKernelWidth",
                "panelKernelHeight",
                "panelKernelLock",
                "panelKernelPresets",
                "panelShape");
            if (FloatingToolTextContains("Morphology.Operation.")
                || FloatingToolTextContains("Morphology.Shape."))
            {
                throw new InvalidOperationException("Docked Morphology exposed localization keys instead of operator-facing labels.");
            }
        });
    }

    private static CaptureResult CaptureShellHostPipelineReview(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        string recipeName = "Smoke_WpfPipelineReview_" + Guid.NewGuid().ToString("N");
        OpenVisionShellHostView shellHost = CreateShellHost(recipeName);
        VisionPipeline pipeline = CreatePipelineReviewReadabilityPipeline();
        VisionPipelineStorage.Save(recipeName, pipeline);
        VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Pipeline);
            Pump(24);
            string koreanBeforeRunDetail = OpenVisionLanguageService.T("PipelineReview.Guide.BeforeRunDetail");
            string koreanBranchFlow = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("PipelineReview.Flow.BranchInputFormat"),
                "Main",
                "Morphology_Preview");
            string koreanBranchDetail = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("PipelineReview.Guide.BranchDetailFormat"),
                "Main",
                "Morphology_Preview");
            string koreanFinalNextAction = OpenVisionLanguageService.T("PipelineReview.Guide.OkFinalNext");
            string koreanFinalDetail = OpenVisionLanguageService.T("PipelineReview.Guide.OkFinalDetail");
            if (shellHost.PipelineReviewStepCount != 3
                || !shellHost.PipelineReviewSelectedStepName.Contains("Threshold", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewFlowSummaryText)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewParameterSummaryText))
            {
                throw new InvalidOperationException("Pipeline review did not show the configured multi-step pipeline.");
            }

            if (!shellHost.PipelineReviewGuideStageText.Contains("1/3", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideCurrentStepText.Contains("Threshold", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideCurrentStepText.Contains("Main", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewGuideNextActionText)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewGuideResultDecisionText)
                || shellHost.PipelineReviewGuideResultDecisionText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideDetailText.Contains(koreanBeforeRunDetail, StringComparison.Ordinal)
                || shellHost.CanSelectPreviousPipelineReviewStepForTest
                || !shellHost.CanSelectNextPipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    "Pipeline review guide did not expose a beginner-readable pre-run state. "
                    + $"Stage='{shellHost.PipelineReviewGuideStageText}', Current='{shellHost.PipelineReviewGuideCurrentStepText}', "
                    + $"Next='{shellHost.PipelineReviewGuideNextActionText}', Decision='{shellHost.PipelineReviewGuideResultDecisionText}', "
                    + $"Detail='{shellHost.PipelineReviewGuideDetailText}', Prev={shellHost.CanSelectPreviousPipelineReviewStepForTest}, Next={shellHost.CanSelectNextPipelineReviewStepForTest}");
            }

            shellHost.SelectPipelineReviewStepForTest(0, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Input);
            Pump(8);
            if (!shellHost.HasPipelineReviewInputPreview)
            {
                throw new InvalidOperationException("Pipeline review did not expose the selected input preview.");
            }

            ClickVisibleButtonByName("btnNextStep", "Pipeline review next step button");
            if (!shellHost.PipelineReviewSelectedStepName.Contains("Morphology", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideStageText.Contains("2/3", StringComparison.Ordinal)
                || !shellHost.CanSelectPreviousPipelineReviewStepForTest
                || !shellHost.CanSelectNextPipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    "Pipeline review next-step button did not select the second step. "
                    + $"Selected='{shellHost.PipelineReviewSelectedStepName}', Stage='{shellHost.PipelineReviewGuideStageText}', "
                    + $"Prev={shellHost.CanSelectPreviousPipelineReviewStepForTest}, Next={shellHost.CanSelectNextPipelineReviewStepForTest}");
            }

            ClickVisibleButtonByName("btnNextStep", "Pipeline review branch step button");
            if (!shellHost.PipelineReviewSelectedStepName.Contains("Filter", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideStageText.Contains("3/3", StringComparison.Ordinal)
                || !shellHost.PipelineReviewFlowSummaryText.Contains(koreanBranchFlow, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains(koreanBranchDetail, StringComparison.Ordinal)
                || !shellHost.CanSelectPreviousPipelineReviewStepForTest
                || shellHost.CanSelectNextPipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    "Pipeline review did not expose the branch-step explanation. "
                    + $"Selected='{shellHost.PipelineReviewSelectedStepName}', Stage='{shellHost.PipelineReviewGuideStageText}', "
                    + $"Flow='{shellHost.PipelineReviewFlowSummaryText}', Detail='{shellHost.PipelineReviewGuideDetailText}', "
                    + $"Prev={shellHost.CanSelectPreviousPipelineReviewStepForTest}, Next={shellHost.CanSelectNextPipelineReviewStepForTest}");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, false);
            Pump(16);
            if (!shellHost.PipelineReviewGuideCurrentStepText.Contains("Filter", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideDetailText.Contains("Branch:", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideNextActionText.Contains("Run Review", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Pipeline review guide did not recalculate dynamic text after switching to English. "
                    + $"Current='{shellHost.PipelineReviewGuideCurrentStepText}', Stage='{shellHost.PipelineReviewGuideStageText}', "
                    + $"Flow='{shellHost.PipelineReviewFlowSummaryText}', Next='{shellHost.PipelineReviewGuideNextActionText}', "
                    + $"Detail='{shellHost.PipelineReviewGuideDetailText}'");
            }

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
            Pump(16);
            WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), 30000, "Pipeline review execution");
            Pump(32);
            if (string.IsNullOrWhiteSpace(shellHost.PipelineReviewValidationStatusText)
                || !shellHost.PipelineReviewResultSummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewRunLogText))
            {
                throw new InvalidOperationException("Pipeline review did not complete with a visible OK result. Validation='"
                    + shellHost.PipelineReviewValidationStatusText
                    + "', Result='"
                    + shellHost.PipelineReviewResultSummaryText
                    + "'.");
            }

            if (!shellHost.HasPipelineReviewOutputPreview)
            {
                throw new InvalidOperationException("Pipeline review did not expose the selected branch output preview after review.");
            }

            if (!shellHost.PipelineReviewGuideResultDecisionText.Contains("OK", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideNextActionText.Contains(koreanFinalNextAction, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideCurrentStepText.Contains("Filter", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideDetailText.Contains(koreanFinalDetail, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Pipeline review guide did not expose the completed OK decision state. "
                    + $"Current='{shellHost.PipelineReviewGuideCurrentStepText}', "
                    + $"Next='{shellHost.PipelineReviewGuideNextActionText}', Decision='{shellHost.PipelineReviewGuideResultDecisionText}', "
                    + $"Detail='{shellHost.PipelineReviewGuideDetailText}'");
            }
        });
    }

    private static CaptureResult CaptureShellHostPipelineReviewNg(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        string recipeName = "Smoke_WpfPipelineReviewNg_" + Guid.NewGuid().ToString("N");
        OpenVisionShellHostView shellHost = CreateShellHost(recipeName);
        VisionPipeline pipeline = CreatePipelineReviewAcceptanceNgPipeline();
        VisionPipelineStorage.Save(recipeName, pipeline);
        VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Pipeline);
            Pump(24);
            if (shellHost.PipelineReviewStepCount != 3
                || !shellHost.PipelineReviewSelectedStepName.Contains("Threshold", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideResultDecisionText.Contains(OpenVisionLanguageService.T("PipelineReview.Guide.NoRunDecision"), StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Pipeline review NG smoke did not start from the expected pre-run threshold step. "
                    + $"Steps={shellHost.PipelineReviewStepCount}, Selected='{shellHost.PipelineReviewSelectedStepName}', "
                    + $"Decision='{shellHost.PipelineReviewGuideResultDecisionText}'.");
            }

            WaitForTaskWithPump(shellHost.RunPipelineReviewForTestAsync(), 30000, "Pipeline review NG execution");
            Pump(32);
            string ngNextAction = OpenVisionLanguageService.T("PipelineReview.Guide.NgNext");
            string fixDetailPrefix = OpenVisionLanguageService.T("PipelineReview.Guide.FixDetailPrefix");
            string parameterLocationPrefix = OpenVisionLanguageService.T("PipelineReview.Guide.ParameterLocationPrefix");
            string resultWidthMetricText = OpenVisionLanguageService.T("PipelineReview.Metric.ResultImageWidth");
            string waitText = OpenVisionLanguageService.T("PipelineReview.Progress.CountsFormat")
                .Contains("대기", StringComparison.Ordinal)
                    ? "대기"
                    : "WAIT";
            if (string.IsNullOrWhiteSpace(shellHost.PipelineReviewValidationStatusText)
                || !shellHost.PipelineReviewResultSummaryText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewProgressText.Contains("OK 0", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewProgressText.Contains("NG 1", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewProgressText.Contains(waitText, StringComparison.OrdinalIgnoreCase)
                || shellHost.PipelineReviewProgressText.Contains(OpenVisionLanguageService.T("PipelineReview.Progress.Running"), StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideResultDecisionText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideNextActionText.Contains(ngNextAction, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideNextActionText.Contains(resultWidthMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains("NG", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideDetailText.Contains(fixDetailPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains(parameterLocationPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains("파라미터 패널", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideParameterFocusText.Contains(parameterLocationPrefix, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideParameterFocusText.Contains("파라미터 패널", StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains(resultWidthMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewGuideDetailText.Contains("<= 1", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideDetailText.Contains("511", StringComparison.Ordinal)
                || shellHost.PipelineReviewGuideDetailText.Contains("Result Width", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewResultDetailText.Contains(resultWidthMetricText, StringComparison.Ordinal)
                || !shellHost.PipelineReviewResultDetailText.Contains("511", StringComparison.Ordinal)
                || shellHost.PipelineReviewResultDetailText.Contains("Result Width", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(shellHost.PipelineReviewRunLogText))
            {
                throw new InvalidOperationException(
                    "Pipeline review did not expose the acceptance NG decision and beginner-readable fix guidance. "
                    + $"Validation='{shellHost.PipelineReviewValidationStatusText}', Result='{shellHost.PipelineReviewResultSummaryText}', "
                    + $"Progress='{shellHost.PipelineReviewProgressText}', Next='{shellHost.PipelineReviewGuideNextActionText}', Decision='{shellHost.PipelineReviewGuideResultDecisionText}', "
                    + $"Detail='{shellHost.PipelineReviewGuideDetailText}', ResultDetail='{shellHost.PipelineReviewResultDetailText}', "
                    + $"RunLog='{shellHost.PipelineReviewRunLogText}'.");
            }

            if (!shellHost.HasPipelineReviewOutputPreview)
            {
                throw new InvalidOperationException("Pipeline review NG did not keep the failed step output image visible for inspection.");
            }

            int nativePreviewRunsAfterReview = shellHost.NativePreviewRunCount;
            shellHost.SelectPipelineReviewStepForTest(2, OpenVisionLab.Pipeline.Controls.PipelineFlowPreviewMode.Output);
            Pump(32);
            if (!shellHost.PipelineReviewSelectedStepName.Contains("Filter", StringComparison.OrdinalIgnoreCase)
                || !shellHost.CanSelectFirstIssuePipelineReviewStepForTest)
            {
                throw new InvalidOperationException(
                    "Pipeline review NG smoke could not move away from the first issue while keeping first-issue navigation available. "
                    + $"Selected='{shellHost.PipelineReviewSelectedStepName}', FirstIssue={shellHost.CanSelectFirstIssuePipelineReviewStepForTest}");
            }

            ClickFloatingButtonByName("btnFirstIssueStep", "Pipeline review first-issue navigation button");
            Pump(32);
            if (!shellHost.PipelineReviewSelectedStepName.Contains("Threshold", StringComparison.OrdinalIgnoreCase)
                || !shellHost.PipelineReviewGuideStageText.Contains("1/3", StringComparison.Ordinal)
                || !shellHost.PipelineReviewResultSummaryText.Contains("NG", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Pipeline review first-issue navigation did not select the first NG step. "
                    + $"Selected='{shellHost.PipelineReviewSelectedStepName}', Stage='{shellHost.PipelineReviewGuideStageText}', Result='{shellHost.PipelineReviewResultSummaryText}'");
            }

            if (shellHost.NativePreviewRunCount != nativePreviewRunsAfterReview)
            {
                throw new InvalidOperationException(
                    "Pipeline review first-issue navigation triggered an unexpected native Preview/Run. "
                    + $"RunsBefore={nativePreviewRunsAfterReview}, RunsAfter={shellHost.NativePreviewRunCount}");
            }

            if (!FloatingToolTextContains(resultWidthMetricText))
            {
                throw new InvalidOperationException(
                    "Pipeline review NG visible detail did not keep the acceptance metric localized. "
                    + $"ExpectedMetric='{resultWidthMetricText}', Detail='{shellHost.PipelineReviewGuideDetailText}'.");
            }
        });
    }

    private static VisionPipeline CreatePipelineReviewReadabilityPipeline()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Pipeline"
        };

        ThresholdToolProperty threshold = new()
        {
            Threshold = 112,
            MaxValue = 255,
            RangeMin = 40,
            RangeMax = 220,
            BlockSize = 25,
            Weight = 5
        };
        MorphologyToolProperty morphology = new()
        {
            KernelWidth = 3,
            KernelHeight = 3,
            Iterations = 1
        };
        FilterToolProperty branchFilter = new()
        {
            KernelWidth = 3,
            KernelHeight = 3,
            MedianKernelSize = 3,
            Diameter = 3,
            SigmaColor = 3,
            SigmaSpace = 3
        };

        pipeline.Steps.Add(VisionPipelineStepBuilder.FromThresholdProperty(threshold, "Threshold_Main", "Main", "Threshold_Preview"));
        pipeline.Steps.Add(VisionPipelineStepBuilder.FromMorphologyProperty(morphology, "Morphology_From_Threshold", "Threshold_Preview", "Morphology_Preview"));
        pipeline.Steps.Add(VisionPipelineStepBuilder.FromFilterProperty(branchFilter, "Filter_Branch_Main", "Main", "Filter_Branch_Preview"));
        return pipeline;
    }

    private static VisionPipeline CreatePipelineReviewAcceptanceNgPipeline()
    {
        VisionPipeline pipeline = CreatePipelineReviewReadabilityPipeline();
        pipeline.Name = "Pipeline_NG";

        VisionPipelineStep thresholdStep = pipeline.Steps.First();
        thresholdStep.UseAcceptance = true;
        thresholdStep.ExpectedSuccess = true;
        thresholdStep.RequiredMessageText = string.Empty;
        thresholdStep.AcceptanceMetricName = VisionPipelineKnownMetrics.ResultImageWidth;
        thresholdStep.UseAcceptanceMetricMinimum = false;
        thresholdStep.AcceptanceMetricMinimum = 0;
        thresholdStep.UseAcceptanceMetricMaximum = true;
        thresholdStep.AcceptanceMetricMaximum = 1;
        return pipeline;
    }

    private static void WaitForTaskWithPump(Task task, int timeoutMilliseconds, string description)
    {
        if (task == null)
        {
            return;
        }

        DateTime deadline = DateTime.UtcNow.AddMilliseconds(Math.Max(1000, timeoutMilliseconds));
        while (!task.IsCompleted && DateTime.UtcNow < deadline)
        {
            Pump(4);
            Thread.Sleep(10);
        }

        if (!task.IsCompleted)
        {
            throw new TimeoutException(description + " timed out.");
        }

        task.GetAwaiter().GetResult();
    }

    private static CaptureResult CaptureThresholdOutputThenBlobOpen(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfThresholdOutputThenBlobOpen", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        shellHost.SetMainLayerImageForTest(mainBitmap);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);
            RadioButton? basicInvert = Application.Current.Windows
                .OfType<Window>()
                .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                .SelectMany(FindVisualChildren<RadioButton>)
                .FirstOrDefault(item => string.Equals(item.Name, "rbBasicInvert", StringComparison.Ordinal));
            if (basicInvert == null)
            {
                throw new InvalidOperationException("Threshold BinaryInv option was not found for Blob detection teaching.");
            }

            basicInvert.IsChecked = true;
            Pump(12);
            shellHost.CreateActiveNativeOutputLayerForTest();
            Pump(12);
            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold did not publish a stable Main -> Threshold_Preview result before Blob open. "
                    + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertNoAutoDockedLayers(shellHost, "Threshold auto-preview");

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(20);
            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Blob after Threshold input layer combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Blob after Threshold input layer combo");
            AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Blob after Threshold input layer combo");
            if (!shellHost.IsNativeDocumentActive
                || !shellHost.ActiveNativeDocumentTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal)
                || !ComboBoxContainsText(inputLayerCombo, "Main")
                || !ComboBoxContainsText(inputLayerCombo, "Threshold_Preview"))
            {
                throw new InvalidOperationException(
                    "Blob did not open correctly after Threshold output creation. "
                    + $"Document={shellHost.ActiveNativeDocumentTypeName}, Combo={GetComboBoxCurrentText(inputLayerCombo)}");
            }
        });
    }

    private static CaptureResult CaptureThresholdToBlobDetectionE2E(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfThresholdToBlobDetection", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        shellHost.SetMainLayerImageForTest(mainBitmap);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);
            if (!shellHost.ConfigureActiveThresholdBasicInvertForTest(true))
            {
                throw new InvalidOperationException("Threshold -> Blob detection could not configure Threshold as BinaryInv.");
            }

            Pump(8);
            int beforeThresholdRuns = shellHost.NativePreviewRunCount;
            SetFloatingSliderValueByName("Threshold basic auto-preview slider", "sliderThreshold", 84D);
            Thread.Sleep(180);
            Pump(30);
            if (!shellHost.HasNativePreviewResult || shellHost.NativePreviewRunCount <= beforeThresholdRuns)
            {
                throw new InvalidOperationException(
                    "Threshold slider did not auto-preview into Threshold_Preview. "
                    + $"RunsBefore={beforeThresholdRuns}, RunsAfter={shellHost.NativePreviewRunCount}, Status={shellHost.ActiveNativeStatusText}");
            }

            using Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main");
            using Bitmap thresholdLayer = shellHost.GetLayerImageCloneForTest("Threshold_Preview");
            AssertBitmapPresent(mainLayer, "Main layer after Threshold preview");
            AssertBitmapPresent(thresholdLayer, "Threshold_Preview layer after Threshold preview");
            AssertBitmapVisiblyDifferent(mainLayer, thresholdLayer, "Threshold output should visibly differ from Main");
            AssertBitmapBinaryLike(thresholdLayer, "Threshold_Preview output");
            SaveDiagnosticBitmap(outputPath, "threshold-to-blob-main.png", mainLayer);
            SaveDiagnosticBitmap(outputPath, "threshold-to-blob-threshold.png", thresholdLayer);

            if (!shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold did not publish a teachable binary output before Blob detection. "
                    + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(20);
            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            if (inputLayerCombo == null)
            {
                throw new InvalidOperationException("Blob input layer combo was not found for Threshold_Preview detection.");
            }

            AssertVisionToolComboTemplate(inputLayerCombo, "Blob Threshold_Preview input layer combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Blob Threshold_Preview input layer combo");
            AssertFloatingPropertyGridRowsRendered("Threshold -> Blob property grid");
            Pump(80);
            SelectComboBoxItemText(inputLayerCombo, "Threshold_Preview", "Blob Threshold_Preview input layer combo");
            Pump(24);

            if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase)
                || !HasFloatingToolPreviewImageSource())
            {
                throw new InvalidOperationException(
                    "Blob did not switch its input preview to the Threshold_Preview layer. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Route={shellHost.ActiveNativeRouteInputLayerNameForTest}");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(32);
            if (!shellHost.IsNativeDocumentActive
                || !shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Blob_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Blob did not produce a detection preview from Threshold_Preview. "
                    + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertResultReviewVisible("Threshold -> Blob detection", "Blob /", "검출", "중심", "박스");
            AssertResultReviewDoesNotContain("Threshold -> Blob detection", "검출 0", "블랍 없음", "박스 512x384");
            AssertNoAutoDockedLayers(shellHost, "Blob preview from Threshold_Preview");

            using Bitmap blobLayer = shellHost.GetLayerImageCloneForTest("Blob_Preview");
            AssertBitmapPresent(blobLayer, "Blob_Preview layer after detection");
            SaveDiagnosticBitmap(outputPath, "threshold-to-blob-blob.png", blobLayer);

            shellHost.SelectToolForTest(VISION_MENU.Contour);
            Pump(20);
            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Contour Threshold_Preview input layer combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Contour Threshold_Preview input layer combo");
            AssertFloatingPropertyGridRowsRendered("Threshold -> Contour property grid");
            SelectComboBoxItemText(inputLayerCombo, "Threshold_Preview", "Contour Threshold_Preview input layer combo");
            Pump(24);

            if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase)
                || !HasFloatingToolPreviewImageSource())
            {
                throw new InvalidOperationException(
                    "Contour did not switch its input preview to the Threshold_Preview layer. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Route={shellHost.ActiveNativeRouteInputLayerNameForTest}");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(32);
            if (!shellHost.IsNativeDocumentActive
                || !shellHost.HasNativePreviewResult
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Contour_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Contour did not produce a detection preview from Threshold_Preview. "
                    + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertResultReviewVisible("Threshold -> Contour detection", "Contour /", "검출", "중심", "박스");
            AssertResultReviewDoesNotContain("Threshold -> Contour detection", "검출 0", "컨투어 없음", "박스 512x384");
            AssertNoAutoDockedLayers(shellHost, "Contour preview from Threshold_Preview");

            using Bitmap contourLayer = shellHost.GetLayerImageCloneForTest("Contour_Preview");
            AssertBitmapPresent(contourLayer, "Contour_Preview layer after detection");
            SaveDiagnosticBitmap(outputPath, "threshold-to-contour-contour.png", contourLayer);
            WriteFloatingToolWindowCapture(outputPath);
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureShellHostBlobTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostBlob");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(16);
            AssertActiveToolTextsVisible("Blob verification guide initial", "Blob 검증", "미리보기 전", "면적 ", "다음:");
            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Blob input layer combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Blob input layer combo");
            AssertComboBoxSelectionCanChange(inputLayerCombo, "Blob input layer combo");
            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertVisionToolComboTemplate(outputLayerCombo, "Blob output layer combo");
            AssertComboBoxPopupLayout(outputLayerCombo, "Blob output layer combo");
            AssertFloatingPropertyGridRowsRendered("Blob property grid");
            AssertFloatingPropertyGridMinimumSize("Blob property grid", 600D, 380D);
            AssertPropertyGridRangeEditorLayout(GetActiveFloatingPropertyGrid("Blob property grid"), "Blob property grid range editor");
            ComboBox? propertyGridCombo = FindFloatingComboBoxes()
                .FirstOrDefault(combo => combo.Items.Cast<object>().Any(item =>
                    string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), "GaussianC", StringComparison.Ordinal)
                    || string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), "Binary", StringComparison.Ordinal)));
            if (propertyGridCombo != null)
            {
                AssertPropertyGridBridgeComboTemplate(propertyGridCombo, "Blob property grid combo");
                AssertComboBoxPopupLayout(propertyGridCombo, "Blob property grid combo");
                string selectedText = Convert.ToString(propertyGridCombo.SelectedItem, CultureInfo.InvariantCulture) ?? propertyGridCombo.Text;
                if (!string.IsNullOrWhiteSpace(selectedText))
                {
                    AssertComboBoxSelectionTextIsSingle(propertyGridCombo, selectedText.Trim(), "Blob property grid combo");
                }
            }

            int beforeThresholdToggleRuns = shellHost.NativePreviewRunCount;
            AssertFloatingPropertyBrowsable("Blob threshold value before toggle", "THRESHOLD", true);
            SetFloatingPropertyGridPropertyValue("Blob disable threshold toggle", "USE_THRESHOLD", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob threshold value after disable", "THRESHOLD", false);
            if (shellHost.NativePreviewRunCount != beforeThresholdToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_THRESHOLD toggle should update PropertyGrid visibility without running auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Blob enable threshold toggle", "USE_THRESHOLD", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob threshold value after enable", "THRESHOLD", true);
            if (shellHost.NativePreviewRunCount != beforeThresholdToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_THRESHOLD enable should not immediately threshold the output image.");
            }

            int beforeAdaptiveToggleRuns = shellHost.NativePreviewRunCount;
            AssertFloatingPropertyBrowsable("Blob adaptive threshold before toggle", "ADAPTIVE_THRESHOLD", false);
            AssertFloatingPropertyBrowsable("Blob adaptive type before toggle", "ADAPTIVE_THRESHOLD_TYPES", false);
            SetFloatingPropertyGridPropertyValue("Blob enable adaptive threshold toggle", "USE_ADAPTIVE_THRESHOLD", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob adaptive threshold after enable", "ADAPTIVE_THRESHOLD", true);
            AssertFloatingPropertyBrowsable("Blob adaptive type after enable", "ADAPTIVE_THRESHOLD_TYPES", true);
            AssertFloatingPropertyBrowsable("Blob adaptive block size after enable", "BlockSize", true);
            AssertFloatingPropertyBrowsable("Blob adaptive weight after enable", "Weight", true);
            if (shellHost.NativePreviewRunCount != beforeAdaptiveToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_ADAPTIVE_THRESHOLD should reveal adaptive fields without running auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Blob disable adaptive threshold toggle", "USE_ADAPTIVE_THRESHOLD", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob adaptive threshold after disable", "ADAPTIVE_THRESHOLD", false);
            AssertFloatingPropertyBrowsable("Blob adaptive type after disable", "ADAPTIVE_THRESHOLD_TYPES", false);
            if (shellHost.NativePreviewRunCount != beforeAdaptiveToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_ADAPTIVE_THRESHOLD disable should not run auto-preview.");
            }

            int beforeMaskingToggleRuns = shellHost.NativePreviewRunCount;
            AssertFloatingPropertyBrowsable("Blob masking editor before toggle", "CvMASKS", false);
            SetFloatingPropertyGridPropertyValue("Blob enable masking toggle", "USE_MASKING", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob masking editor after enable", "CvMASKS", true);
            if (shellHost.NativePreviewRunCount != beforeMaskingToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_MASKING should reveal the masking editor without running auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Blob disable masking toggle", "USE_MASKING", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Blob masking editor after disable", "CvMASKS", false);
            if (shellHost.NativePreviewRunCount != beforeMaskingToggleRuns)
            {
                throw new InvalidOperationException("Blob USE_MASKING disable should not run auto-preview.");
            }

            int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
            SetFloatingPropertyGridThresholdSliderValue("Blob threshold auto preview slider", 84D);
            Thread.Sleep(180);
            Pump(30);

            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF Blob tool did not auto-preview after threshold slider changes.");
            }

            if (shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns)
            {
                throw new InvalidOperationException("Native WPF Blob threshold slider did not request an auto-preview.");
            }

            AssertResultReviewRawVisible("Blob threshold teaching preview", "결과 대기");
            AssertResultReviewRawDoesNotContain("Blob threshold teaching preview", "Blob /", "검출 0", "중심", "박스 512x384");

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Blob did not open the WPF Blob tool view.");
            }

            AssertActiveFloatingInlinePreviewSlotCount("Blob auto-preview input/output", 2);
            AssertActiveFloatingInlinePreviewZoomAnchors("Blob inline preview zoom anchor");
            ClickActiveFloatingPreviewFrameByAutomationId("VisionToolOutputPreviewSlot", "Blob output preview");
            if (!string.Equals(shellHost.ActiveHostLayerTitle, "Blob_Preview", StringComparison.OrdinalIgnoreCase)
                || !shellHost.HasWorkspaceLayerPreview
                || !string.Equals(shellHost.WorkspaceLayerTitle, "Blob_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Blob output preview click did not activate the output layer in the main workspace. "
                    + $"Active={shellHost.ActiveHostLayerTitle}, Workspace={shellHost.WorkspaceLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
            }

            using (Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main"))
            using (Bitmap blobLayer = shellHost.GetLayerImageCloneForTest("Blob_Preview"))
            {
                AssertBitmapPresent(blobLayer, "Blob_Preview layer after threshold slider auto-preview");
                AssertBitmapVisiblyDifferent(mainLayer, blobLayer, "Blob output should show threshold teaching image instead of raw Main");
                AssertBitmapBinaryLike(blobLayer, "Blob_Preview threshold teaching output");
                AssertBitmapMostlyGrayscale(blobLayer, "Blob threshold auto-preview should not show detection markers before Run");
                SaveDiagnosticBitmap(outputPath, "blob-tool-main.png", mainLayer);
                SaveDiagnosticBitmap(outputPath, "blob-tool-threshold-preview.png", blobLayer);
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            AssertResultReviewVisible("Blob", "Blob /", "검출", "최대 면적", "중심", "박스");
            AssertResultReviewVisible("Blob result guidance", "미리보기 OK", "합격 기준:", "최대 면적", "박스", "다음:");
            AssertActiveToolTextsVisible("Blob verification guide result", "Blob 검증", "미리보기 OK", "면적 ", "다음:");

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "Blob", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Blob_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.ContainsKey("MIN_AREA")
                || !step.Parameters.ContainsKey("MAX_AREA")
                || !step.Parameters.ContainsKey("THRESHOLD"))
            {
                throw new InvalidOperationException("Blob WPF tool did not create a valid pipeline step.");
            }

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Blob floating tool did not accept the dock-to-right request.");
            }

            Pump(24);
            if (!shellHost.IsDockedToolInspectorVisibleForTest
                || !shellHost.ActiveWpfToolWindowTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal)
                || !shellHost.IsNativeDocumentActive
                || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException(
                    "Blob dock-to-right did not preserve the hosted tool/document state. "
                    + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                    + $"NativeActive={shellHost.IsNativeDocumentActive}, HasPreview={shellHost.HasNativePreviewResult}");
            }

            if (CountVisibleFloatingToolWindows() != 0)
            {
                throw new InvalidOperationException("Docked Blob should not leave a visible floating tool window behind.");
            }

            AssertDockedSingleInputToolLayout("Docked Blob tool layout");
            AssertResultReviewVisible("Docked Blob result guidance", "미리보기 OK", "합격 기준:", "최대 면적", "박스", "다음:");
            AssertActiveToolTextsVisible("Docked Blob verification guide", "Blob 검증", "미리보기 OK", "면적 ", "다음:");
            AssertDirectResultOkBanner(shellHost, "Docked Blob after Preview");

            shellHost.SetDockedToolInspectorWidthForTest(680D);
            Pump(8);
            if (Math.Abs(shellHost.DockedToolInspectorWidthForTest - 680D) > 2D)
            {
                throw new InvalidOperationException(
                    "Docked Blob inspector did not accept an operator width adjustment. "
                    + $"Width={shellHost.DockedToolInspectorWidthForTest:0.0}");
            }

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(24);
            if (!shellHost.IsDockedToolInspectorVisibleForTest
                || !shellHost.ActiveWpfToolWindowTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal)
                || CountVisibleFloatingToolWindows() != 0)
            {
                throw new InvalidOperationException(
                    "Selecting Blob again while docked should reuse the docked inspector instead of opening a duplicate floating tool. "
                    + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                    + $"FloatingWindows={CountVisibleFloatingToolWindows()}");
            }

            if (Math.Abs(shellHost.DockedToolInspectorWidthForTest - 680D) > 2D)
            {
                throw new InvalidOperationException(
                    "Selecting Blob again while docked should preserve the adjusted inspector width. "
                    + $"Width={shellHost.DockedToolInspectorWidthForTest:0.0}");
            }

            AssertDockedSingleInputToolLayout("Docked Blob tool layout after reselect");
            AssertActiveToolTextsVisible("Docked Blob verification guide after reselect", "Blob 검증", "미리보기 OK", "면적 ", "다음:");
            AssertDirectResultOkBanner(shellHost, "Docked Blob after same-tool reselect");

            shellHost.SelectToolForTest(VISION_MENU.Contour);
            Pump(24);
            if (!shellHost.IsDockedToolInspectorVisibleForTest
                || !shellHost.ActiveWpfToolWindowTypeName.Contains("ContourToolWpfView", StringComparison.Ordinal)
                || CountVisibleFloatingToolWindows() != 0)
            {
                throw new InvalidOperationException(
                    "Selecting Contour from docked Blob should reuse the docked inspector. "
                    + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                    + $"FloatingWindows={CountVisibleFloatingToolWindows()}");
            }

            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(24);
            if (!shellHost.IsDockedToolInspectorVisibleForTest
                || !shellHost.ActiveWpfToolWindowTypeName.Contains("BlobToolWpfView", StringComparison.Ordinal)
                || !shellHost.HasNativePreviewResult
                || CountVisibleFloatingToolWindows() != 0)
            {
                throw new InvalidOperationException(
                    "Reselecting cached Blob from another docked tool should restore the Blob result document in-place. "
                    + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                    + $"HasPreview={shellHost.HasNativePreviewResult}, FloatingWindows={CountVisibleFloatingToolWindows()}");
            }

            AssertDockedSingleInputToolLayout("Docked Blob tool layout after cross-tool reselect");
            AssertActiveToolTextsVisible("Docked Blob verification guide after cross-tool reselect", "Blob 검증", "미리보기 OK", "면적 ", "다음:");
            AssertDirectResultOkBanner(shellHost, "Docked Blob after cross-tool reselect");
        });
    }

    private static CaptureResult CaptureShellHostContourTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostContour");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Contour);
            Pump(16);
            AssertActiveToolTextsVisible("Contour verification guide initial", "Contour 검증", "미리보기 전", "면적 ", "다음:");
            AssertFloatingPropertyGridRowsRendered("Contour property grid");
            System.Windows.Controls.WpfPropertyGrid.PropertyGrid contourGrid = GetActiveFloatingPropertyGrid("Contour property grid");
            List<string> contourPropertyEvents = new();
            contourGrid.PropertyValueChanged += (_, e) =>
            {
                contourPropertyEvents.Add(e?.PropertyName ?? "<null>");
            };

            AssertPropertyGridRangeEditorLayout(contourGrid, "Contour property grid range editor");
            Thread.Sleep(700);
            Pump(80);
            int beforeDrawToggleRuns = shellHost.NativePreviewRunCount;
            SetFloatingPropertyGridPropertyValue("Contour reset approx poly", "USE_APPROXPOLYDP", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Contour approx epsilon before approx poly", "EPSILON", false);
            if (shellHost.NativePreviewRunCount != beforeDrawToggleRuns)
            {
                throw new InvalidOperationException(
                    "Contour USE_APPROXPOLYDP reset should only update approximation rows, not run auto-preview. Events="
                    + string.Join(",", contourPropertyEvents));
            }
            SetFloatingPropertyGridPropertyValue("Contour enable approx poly", "USE_APPROXPOLYDP", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Contour approx epsilon after enable", "EPSILON", true);
            if (shellHost.NativePreviewRunCount != beforeDrawToggleRuns)
            {
                throw new InvalidOperationException(
                    "Contour USE_APPROXPOLYDP should reveal approximation options without running auto-preview. Events="
                    + string.Join(",", contourPropertyEvents));
            }

            SetFloatingPropertyGridPropertyValue("Contour disable approx poly", "USE_APPROXPOLYDP", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Contour approx epsilon after disable", "EPSILON", false);
            if (shellHost.NativePreviewRunCount != beforeDrawToggleRuns)
            {
                throw new InvalidOperationException(
                    "Contour USE_APPROXPOLYDP disable should not run auto-preview. Events="
                    + string.Join(",", contourPropertyEvents));
            }

            AssertFloatingPropertyBrowsable("Contour legacy draw result hidden", "USE_DRAW_IMAGE", false);
            AssertFloatingPropertyBrowsable("Contour draw mode visible", "DrawMode", true);
            AssertFloatingPropertyBrowsable("Contour draw color visible", "DrawColor", true);
            AssertFloatingPropertyBrowsable("Contour draw thickness visible", "DrawThickness", true);
            if (shellHost.NativePreviewRunCount != beforeDrawToggleRuns)
            {
                throw new InvalidOperationException(
                    "Contour drawing options should be visible without running auto-preview. Events="
                    + string.Join(",", contourPropertyEvents));
            }

            SetFloatingPropertyGridPropertyValue("Contour draw color red", "DrawColor", DrawingColor.Red);
            SetFloatingPropertyGridPropertyValue("Contour draw thickness", "DrawThickness", 5);
            Thread.Sleep(180);
            Pump(30);

            int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
            SetFloatingPropertyGridThresholdSliderValue("Contour threshold auto preview slider", 84D);
            Thread.Sleep(180);
            Pump(30);
            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF Contour tool did not auto-preview after threshold slider changes.");
            }

            if (shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns)
            {
                throw new InvalidOperationException("Native WPF Contour threshold slider did not request an auto-preview.");
            }

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("ContourToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Contour did not open the WPF Contour tool view.");
            }

            AssertActiveFloatingInlinePreviewSlotCount("Contour auto-preview input/output", 2);
            AssertActiveFloatingInlinePreviewZoomAnchors("Contour inline preview zoom/pan");
            AssertPreviewClickActivatesWorkspaceLayer(
                shellHost,
                "VisionToolOutputPreviewSlot",
                "Contour_Preview",
                "Contour output preview click");
            AssertPreviewClickActivatesWorkspaceLayer(
                shellHost,
                "VisionToolInputPreviewSlot",
                "Main",
                "Contour input preview click");
            using (Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main"))
            using (Bitmap contourLayer = shellHost.GetLayerImageCloneForTest("Contour_Preview"))
            {
                AssertBitmapPresent(contourLayer, "Contour_Preview layer after threshold slider auto-preview");
                AssertBitmapVisiblyDifferent(mainLayer, contourLayer, "Contour output should show threshold teaching image instead of raw Main");
                AssertBitmapBinaryLike(contourLayer, "Contour_Preview threshold teaching output");
                AssertBitmapMostlyGrayscale(contourLayer, "Contour threshold auto-preview should not show detection markers before Run");
                SaveDiagnosticBitmap(outputPath, "contour-tool-main.png", mainLayer);
                SaveDiagnosticBitmap(outputPath, "contour-tool-threshold-preview.png", contourLayer);
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            AssertResultReviewVisible("Contour", "Contour /", "검출", "최대 면적", "중심", "박스");
            AssertResultReviewVisible("Contour result guidance", "미리보기 OK", "합격 기준:", "최대 면적", "박스", "다음:");
            AssertActiveToolTextsVisible("Contour verification guide result", "Contour 검증", "미리보기 OK", "면적 ", "다음:");
            using (Bitmap contourDrawLayer = shellHost.GetLayerImageCloneForTest("Contour_Preview"))
            {
                AssertBitmapContainsColorNear(contourDrawLayer, DrawingColor.Red, 30, "Contour draw result red overlay");
                SaveDiagnosticBitmap(outputPath, "contour-tool-draw-result.png", contourDrawLayer);
            }

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "Contour", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Contour_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.ContainsKey("MIN_AREA")
                || !step.Parameters.ContainsKey("MAX_AREA")
                || !step.Parameters.ContainsKey("DetectMode"))
            {
                throw new InvalidOperationException("Contour WPF tool did not create a valid pipeline step.");
            }

            AssertDockActiveNativeTool(shellHost, "ContourToolWpfView", "Docked Contour tool layout");
            AssertResultReviewVisible("Docked Contour result guidance", "미리보기 OK", "합격 기준:", "최대 면적", "박스", "다음:");
            AssertActiveToolTextsVisible("Docked Contour verification guide", "Contour 검증", "미리보기 OK", "면적 ", "다음:");
        });
    }

    private static CaptureResult CaptureShellHostAreaToolPresets(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostAreaToolPresets");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(16);
            AssertActiveToolTextsVisible(
                "Blob preset controls",
                OpenVisionLanguageService.T("VisionTool.Preset.Title"),
                OpenVisionLanguageService.T("VisionTool.Preset.Basic"),
                OpenVisionLanguageService.T("VisionTool.Preset.Fast"),
                OpenVisionLanguageService.T("VisionTool.Preset.Precise"));
            AssertFloatingPropertyGridRowsRendered("Blob preset property grid");

            int beforeBlobPresetRuns = shellHost.NativePreviewRunCount;
            ClickFloatingButtonByName("btnPresetFast", "Blob fast preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Blob fast preset threshold", "USE_THRESHOLD", true);
            AssertFloatingSelectedObjectBooleanProperty("Blob fast preset adaptive", "USE_ADAPTIVE_THRESHOLD", false);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob fast preset threshold value", "THRESHOLD", 120D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob fast preset min area", "MIN_AREA", 500D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob fast preset max area", "MAX_AREA", 1000000D, 0.001D);
            AssertFloatingPropertyBrowsable("Blob fast preset adaptive threshold hidden", "ADAPTIVE_THRESHOLD", false);
            if (shellHost.NativePreviewRunCount != beforeBlobPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Blob fast preset must update PropertyGrid only, not run preview.");
            }

            ClickFloatingButtonByName("btnPresetPrecise", "Blob precise preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Blob precise preset threshold", "USE_THRESHOLD", false);
            AssertFloatingSelectedObjectBooleanProperty("Blob precise preset adaptive", "USE_ADAPTIVE_THRESHOLD", true);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob precise preset min area", "MIN_AREA", 80D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob precise preset block size", "BlockSize", 25D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Blob precise preset weight", "Weight", 5D, 0.001D);
            AssertFloatingPropertyBrowsable("Blob precise preset adaptive threshold visible", "ADAPTIVE_THRESHOLD", true);
            if (shellHost.NativePreviewRunCount != beforeBlobPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Blob precise preset must update PropertyGrid only, not run preview.");
            }

            AssertDockActiveNativeTool(shellHost, "BlobToolWpfView", "Docked Blob preset menu");
            AssertActiveToolButtonVisible("btnPresetMenu", "Docked Blob preset menu button", true);
            int beforeDockedBlobPresetRuns = shellHost.NativePreviewRunCount;
            ClickActiveToolPresetMenuItem("fast", "Docked Blob fast preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectNumericPropertyWithin("Docked Blob fast preset min area", "MIN_AREA", 500D, 0.001D);
            AssertFloatingSelectedObjectBooleanProperty("Docked Blob fast preset adaptive", "USE_ADAPTIVE_THRESHOLD", false);
            if (shellHost.NativePreviewRunCount != beforeDockedBlobPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Docked Blob preset menu must update PropertyGrid only, not run preview.");
            }

            shellHost.SelectToolForTest(VISION_MENU.Contour);
            Pump(16);
            AssertActiveToolTextsVisible(
                "Contour preset controls",
                OpenVisionLanguageService.T("VisionTool.Preset.Title"),
                OpenVisionLanguageService.T("VisionTool.Preset.Basic"),
                OpenVisionLanguageService.T("VisionTool.Preset.Fast"),
                OpenVisionLanguageService.T("VisionTool.Preset.Precise"));
            AssertFloatingPropertyGridRowsRendered("Contour preset property grid");

            int beforeContourPresetRuns = shellHost.NativePreviewRunCount;
            ClickFloatingButtonByName("btnPresetPrecise", "Contour precise preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Contour precise preset approx poly", "USE_APPROXPOLYDP", true);
            AssertFloatingSelectedObjectBooleanProperty("Contour precise preset adaptive", "USE_ADAPTIVE_THRESHOLD", false);
            AssertFloatingSelectedObjectNumericPropertyWithin("Contour precise preset epsilon", "EPSILON", 0.005D, 0.0001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Contour precise preset min area", "MIN_AREA", 80D, 0.001D);
            AssertFloatingPropertyBrowsable("Contour precise preset epsilon visible", "EPSILON", true);
            if (shellHost.NativePreviewRunCount != beforeContourPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Contour precise preset must update PropertyGrid only, not run preview.");
            }

            ClickFloatingButtonByName("btnPresetBasic", "Contour basic preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Contour basic preset approx poly", "USE_APPROXPOLYDP", false);
            AssertFloatingSelectedObjectNumericPropertyWithin("Contour basic preset min area", "MIN_AREA", 200D, 0.001D);
            AssertFloatingPropertyBrowsable("Contour basic preset epsilon hidden", "EPSILON", false);
            if (shellHost.NativePreviewRunCount != beforeContourPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Contour basic preset must update PropertyGrid only, not run preview.");
            }
        });
    }

    private static CaptureResult CaptureShellHostPendingTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        using OpenVisionPendingToolViewModel viewModel = new("VisionMenu.FeatureMatching", "Feature Matching", MahApps.Metro.IconPacks.PackIconMaterialKind.ImageSearch);
        OpenVisionPendingToolView view = new(viewModel);
        return CaptureElement(view, outputPath, 820, 560);
    }

    private static CaptureResult CaptureToolWindowReopenSameTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfToolWindowReopenSameTool");
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(16);

            if (!shellHost.IsActiveWpfToolWindowVisibleForTest || !shellHost.IsNativeDocumentActive)
            {
                throw new InvalidOperationException("Matching tool window did not open before the reopen check.");
            }

            if (!shellHost.CloseActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Matching tool window could not be closed for the reopen check.");
            }

            Pump(16);
            if (shellHost.IsActiveWpfToolWindowVisibleForTest || shellHost.IsNativeDocumentActive)
            {
                throw new InvalidOperationException("Closing the matching tool window did not detach the active document.");
            }

            // A second click on the already-selected menu must reopen the floating
            // tool window; otherwise the UI appears dead after the user closes it.
            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(16);

            if (!shellHost.IsActiveWpfToolWindowVisibleForTest
                || !shellHost.IsNativeDocumentActive
                || !shellHost.ActiveNativeDocumentTypeName.Contains("MatchingToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Re-clicking the selected Matching menu did not reopen the WPF tool window.");
            }

            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Reopened matching input layer combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Reopened matching input layer combo");
        });
    }

    private static CaptureResult CaptureToolWindowDockFloatCycle(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfToolWindowDockFloatCycle");
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        shellHost.SetMainLayerImageForTest(mainBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(16);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", false, 1, "Blob initial floating");

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Blob floating tool did not accept the dock-to-right request.");
            }

            Pump(24);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after dock");
            AssertDockedSingleInputToolLayout("Blob after dock");

            shellHost.SetDockedToolInspectorWidthForTest(680D);
            Pump(8);
            AssertDockedToolInspectorWidth(shellHost, 680D, "Blob adjusted dock width");

            if (!shellHost.FloatDockedWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Docked Blob tool did not accept the float request.");
            }

            Pump(24);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", false, 1, "Blob after float");

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Floated Blob tool did not accept the second dock request.");
            }

            Pump(24);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after re-dock");
            AssertDockedToolInspectorWidth(shellHost, 680D, "Blob re-dock width");
            AssertDockedSingleInputToolLayout("Blob after re-dock");

            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(24);
            AssertActiveToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after switching from docked Blob");
            AssertDockedSingleInputToolLayout("Matching after docked switch");

            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(16);
            AssertActiveToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after repeated docked selection");
            AssertDockedToolInspectorWidth(shellHost, 680D, "Matching repeated selection width");
            AssertDockedSingleInputToolLayout("Matching after repeated docked selection");

            double savedOffset = ScrollActivePropertyGridForNavigationRestore("Matching docked navigation restore setup", 160D);
            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(24);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after leaving scrolled Matching");
            AssertDockedSingleInputToolLayout("Blob after leaving scrolled Matching");

            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(36);
            AssertActiveToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after navigation restore");
            AssertDockedToolInspectorWidth(shellHost, 680D, "Matching navigation restore width");
            AssertDockedSingleInputToolLayout("Matching after navigation restore");
            AssertActivePropertyGridVerticalOffsetAtLeast(
                "Matching docked navigation restore",
                Math.Min(60D, savedOffset * 0.5D));

            SetActivePropertyGridSearchText("Matching docked search restore setup", "angle");
            AssertActivePropertyGridSearchText("Matching docked search before switch", "angle");
            shellHost.SelectToolForTest(VISION_MENU.Blob);
            Pump(24);
            AssertActiveToolWindowState(shellHost, "BlobToolWpfView", true, 0, "Blob after leaving searched Matching");
            AssertDockedSingleInputToolLayout("Blob after leaving searched Matching");

            shellHost.SelectToolForTest(VISION_MENU.Matching);
            Pump(36);
            AssertActiveToolWindowState(shellHost, "MatchingToolWpfView", true, 0, "Matching after search restore");
            AssertDockedSingleInputToolLayout("Matching after search restore");
            AssertActivePropertyGridSearchText("Matching docked search restore", "angle");
            SetActivePropertyGridSearchText("Matching docked empty search setup", "__no_property_match__");
            AssertActivePropertyGridSearchEmptyMessageVisible("Matching docked empty search", true);
            SetActivePropertyGridSearchText("Matching docked empty search clear", string.Empty);
            AssertActivePropertyGridSearchEmptyMessageVisible("Matching docked empty search cleared", false);
        });
    }

    private static CaptureResult CaptureShellHostMatchingTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostMatching");
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(matchingBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(16);
                ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                AssertVisionToolComboTemplate(inputLayerCombo, "Matching input layer combo");
                AssertComboBoxPopupLayout(inputLayerCombo, "Matching input layer combo");
                AssertComboBoxSelectionCanChange(inputLayerCombo, "Matching input layer combo");
                ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                AssertVisionToolComboTemplate(outputLayerCombo, "Matching output layer combo");
                AssertComboBoxPopupLayout(outputLayerCombo, "Matching output layer combo");
                AssertFloatingPropertyGridRowsRendered("Matching property grid");
                AssertFloatingPropertyGridMinimumSize("Matching property grid", 600D, 380D);
                SetFloatingPropertyGridPropertyValue("Matching auto preview off by default", "AUTO_PREVIEW", false);
                SetFloatingPropertyGridPropertyValue("Matching reset angle search", "USE_FIND_ANGLE", true);
                SetFloatingPropertyGridPropertyValue("Matching reset canny", "USE_CANNY", false);
                Thread.Sleep(180);
                Pump(30);
                int beforeVisibilityToggleRuns = shellHost.NativePreviewRunCount;
                AssertFloatingPropertyBrowsable("Matching angle step before disable", "FIND_ANGLE", true);
                AssertFloatingPropertyBrowsable("Matching min angle before disable", "FIND_ANGLE_MIN", true);
                AssertFloatingPropertyBrowsable("Matching max angle before disable", "FIND_ANGLE_MAX", true);
                AssertFloatingRangeEditorEndpointAllowsTransientText(
                    "Matching angle min range editor transient clear",
                    "FIND_ANGLE_MIN",
                    endpointIndex: 0,
                    transientText: string.Empty,
                    restoreValue: -10D);
                double maxAngleSliderTarget = Math.Abs(GetFloatingSelectedObjectNumericProperty("FIND_ANGLE_MAX") - 90D) < 0.5D ? 120D : 90D;
                SetFloatingRangeEditorEndpointSliderValue(
                    "Matching angle max range editor",
                    "FIND_ANGLE_MIN",
                    endpointIndex: 1,
                    maxAngleSliderTarget);
                AssertFloatingSelectedObjectNumericProperty("Matching angle max range editor", "FIND_ANGLE_MAX", maxAngleSliderTarget);
                SetFloatingPropertyGridPropertyValue("Matching disable angle search", "USE_FIND_ANGLE", false);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching angle step after disable", "FIND_ANGLE", false);
                AssertFloatingPropertyBrowsable("Matching min angle after disable", "FIND_ANGLE_MIN", false);
                AssertFloatingPropertyBrowsable("Matching max angle after disable", "FIND_ANGLE_MAX", false);
                if (shellHost.NativePreviewRunCount != beforeVisibilityToggleRuns)
                {
                    throw new InvalidOperationException("Matching USE_FIND_ANGLE should only update angle rows, not run auto-preview.");
                }

                SetFloatingPropertyGridPropertyValue("Matching enable angle search", "USE_FIND_ANGLE", true);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching angle step after enable", "FIND_ANGLE", true);
                AssertFloatingPropertyBrowsable("Matching min angle after enable", "FIND_ANGLE_MIN", true);
                AssertFloatingPropertyBrowsable("Matching max angle after enable", "FIND_ANGLE_MAX", true);
                if (shellHost.NativePreviewRunCount != beforeVisibilityToggleRuns)
                {
                    throw new InvalidOperationException("Matching USE_FIND_ANGLE enable should not run auto-preview.");
                }

                AssertFloatingPropertyBrowsable("Matching canny low before enable", "CANNY_LOW", false);
                AssertFloatingPropertyBrowsable("Matching canny high before enable", "CANNY_HIGH", false);
                SetFloatingPropertyGridPropertyValue("Matching enable canny", "USE_CANNY", true);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching canny low after enable", "CANNY_LOW", true);
                AssertFloatingPropertyBrowsable("Matching canny high after enable", "CANNY_HIGH", true);
                if (shellHost.NativePreviewRunCount != beforeVisibilityToggleRuns)
                {
                    throw new InvalidOperationException("Matching USE_CANNY should only update canny rows, not run auto-preview.");
                }

                SetFloatingPropertyGridPropertyValue("Matching disable canny", "USE_CANNY", false);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching canny low after disable", "CANNY_LOW", false);
                AssertFloatingPropertyBrowsable("Matching canny high after disable", "CANNY_HIGH", false);
                if (shellHost.NativePreviewRunCount != beforeVisibilityToggleRuns)
                {
                    throw new InvalidOperationException("Matching USE_CANNY disable should not run auto-preview.");
                }

                int beforeManualParameterRuns = shellHost.NativePreviewRunCount;
                SetFloatingPropertyGridPropertyValue("Matching angle step manual mode", "FIND_ANGLE", 1D);
                Thread.Sleep(180);
                Pump(30);
                if (shellHost.NativePreviewRunCount != beforeManualParameterRuns)
                {
                    throw new InvalidOperationException("Matching angle edits must not auto-preview while AUTO_PREVIEW is false.");
                }

                int beforeTemplateRegistrationRuns = shellHost.NativePreviewRunCount;
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyGridDialogButtonsReady("Matching property grid dialog button");
                if (shellHost.NativePreviewRunCount != beforeTemplateRegistrationRuns)
                {
                    throw new InvalidOperationException("Matching template registration must not auto-preview while AUTO_PREVIEW is false.");
                }

                SetFloatingPropertyGridTextBoxTextWithoutCommit(
                    "Matching min score command commit",
                    "SCORE_MIN",
                    "0.57");
                shellHost.RunActiveNativePreviewForTest();
                Pump(30);
                AssertFloatingSelectedObjectNumericProperty("Matching min score command commit", "SCORE_MIN", 0.57D);
                if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Native WPF Matching tool did not preview after explicit run: " + shellHost.ActiveNativeStatusText);
                }

                if (!shellHost.ActiveNativeDocumentTypeName.Contains("MatchingToolWpfView", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Matching did not open the WPF Matching tool view.");
                }

                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Matching_Preview", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.WorkspaceLayerTitle, "Matching_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Matching template registration changed the route or active input layer. "
                        + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, "
                        + $"Active={shellHost.ActiveHostLayerTitle}, Workspace={shellHost.WorkspaceLayerTitle}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertNoAutoDockedLayers(shellHost, "Matching explicit preview");
                AssertResultReviewVisible("Matching", "Template Match /", "검출", "점수", "중심", "박스", "처리");
                AssertResultReviewVisible("Matching result guidance", "미리보기 OK", "합격 기준:", "최고 점수", ">=", "검출", "다음:");
                AssertActiveToolTextsVisible("Matching verification guide", "검증 흐름", "미리보기 OK", "합격 기준", "다음:");
                AssertActiveToolTextsVisible("Matching teaching summary", "템플릿 준비", "점수 >=", "원본", "전체 이미지");
                AssertActiveFloatingInlinePreviewSlotCount("Matching explicit preview input/output", 2);
                AssertActiveFloatingInlinePreviewZoomAnchors("Matching inline preview zoom/pan");

                int beforeOptInAutoPreviewRuns = shellHost.NativePreviewRunCount;
                SetFloatingPropertyGridPropertyValue("Matching auto preview on", "AUTO_PREVIEW", true);
                Thread.Sleep(180);
                Pump(30);
                if (shellHost.NativePreviewRunCount != beforeOptInAutoPreviewRuns)
                {
                    throw new InvalidOperationException("Enabling Matching AUTO_PREVIEW must not run preview by itself.");
                }

                SetFloatingPropertyGridPropertyValue("Matching min score auto-preview", "SCORE_MIN", 0.56D);
                Thread.Sleep(220);
                Pump(40);
                if (shellHost.NativePreviewRunCount <= beforeOptInAutoPreviewRuns)
                {
                    throw new InvalidOperationException("Matching parameter edit did not auto-preview after AUTO_PREVIEW was enabled.");
                }

                AssertPreviewClickActivatesWorkspaceLayer(
                    shellHost,
                    "VisionToolOutputPreviewSlot",
                    "Matching_Preview",
                    "Matching output preview click");
                AssertPreviewClickActivatesWorkspaceLayer(
                    shellHost,
                    "VisionToolInputPreviewSlot",
                    "Main",
                    "Matching input preview click");

                VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
                if (step == null
                    || !string.Equals(step.ToolType, "Matching", StringComparison.Ordinal)
                    || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                    || !string.Equals(step.OutputLayer, "Matching_Preview", StringComparison.Ordinal)
                    || step.Parameters == null
                    || !step.Parameters.ContainsKey("PATTERN_PATH")
                    || !step.Parameters.ContainsKey("SCORE_MIN")
                    || !step.Parameters.ContainsKey("MATCH_MODE"))
                {
                    throw new InvalidOperationException("Matching WPF tool did not create a valid pipeline step.");
                }

                AssertDockActiveNativeTool(shellHost, "MatchingToolWpfView", "Docked Matching tool layout");
                AssertFloatingPropertyGridMinimumSize("Docked Matching property grid", 600D, 380D);
                AssertResultReviewVisible("Docked Matching", "Template Match /", "검출", "점수", "처리");
                AssertResultReviewVisible("Docked Matching result guidance", "미리보기 OK", "합격 기준:", "최고 점수", ">=", "검출", "다음:");
                AssertActiveToolTextsVisible("Docked Matching verification guide", "검증 흐름", "미리보기 OK", "합격 기준", "다음:");
                AssertActiveToolTextsVisible("Docked Matching teaching summary", "템플릿 준비", "점수 >=", "원본", "전체 이미지");
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static CaptureResult CaptureShellHostMatchingPresets(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostMatchingPresets");
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(matchingBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(16);
                AssertFloatingPropertyGridRowsRendered("Matching preset property grid");
                AssertFloatingPropertyGridMinimumSize("Matching preset property grid", 600D, 380D);
                AssertActiveToolTextsVisible("Matching preset controls", "추천 프리셋", "기본", "빠른", "정밀");

                SetFloatingPropertyGridPropertyValue("Matching preset auto preview off", "AUTO_PREVIEW", false);
                int beforeTemplateRuns = shellHost.NativePreviewRunCount;
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Thread.Sleep(180);
                Pump(30);
                if (shellHost.NativePreviewRunCount != beforeTemplateRuns)
                {
                    throw new InvalidOperationException("Matching preset setup template registration must not auto-preview while AUTO_PREVIEW is false.");
                }

                SetFloatingPropertyGridPropertyValue("Matching preset auto preview on", "AUTO_PREVIEW", true);
                Thread.Sleep(180);
                Pump(30);
                int beforePresetRuns = shellHost.NativePreviewRunCount;
                ClickFloatingButtonByName("btnPresetFast", "Matching fast preset");
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingSelectedObjectBooleanProperty("Matching fast preset auto preview", "AUTO_PREVIEW", false);
                AssertFloatingSelectedObjectBooleanProperty("Matching fast preset angle search", "USE_FIND_ANGLE", false);
                AssertFloatingSelectedObjectBooleanProperty("Matching fast preset scale search", "USE_FIND_SCALE", false);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching fast preset min score", "SCORE_MIN", 0.7D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching fast preset count", "NUM_MATCH", 1D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching fast preset magnification", "MAGNIFIATION", 0.5D, 0.001D);
                AssertFloatingPropertyBrowsable("Matching fast preset angle step hidden", "FIND_ANGLE", false);
                if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Matching fast preset must update PropertyGrid only, not run preview.");
                }

                ClickFloatingButtonByName("btnPresetPrecise", "Matching precise preset");
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingSelectedObjectBooleanProperty("Matching precise preset auto preview", "AUTO_PREVIEW", false);
                AssertFloatingSelectedObjectBooleanProperty("Matching precise preset angle search", "USE_FIND_ANGLE", true);
                AssertFloatingSelectedObjectBooleanProperty("Matching precise preset scale search", "USE_FIND_SCALE", false);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset min score", "SCORE_MIN", 0.6D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset count", "NUM_MATCH", 3D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset magnification", "MAGNIFIATION", 1.0D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset angle min", "FIND_ANGLE_MIN", -10D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset angle max", "FIND_ANGLE_MAX", 10D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching precise preset angle step", "FIND_ANGLE", 0.5D, 0.001D);
                AssertFloatingPropertyBrowsable("Matching precise preset angle step visible", "FIND_ANGLE", true);
                if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Matching precise preset must update PropertyGrid only, not run preview.");
                }

                ClickFloatingButtonByName("btnPresetBasic", "Matching basic preset");
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingSelectedObjectBooleanProperty("Matching basic preset auto preview", "AUTO_PREVIEW", false);
                AssertFloatingSelectedObjectBooleanProperty("Matching basic preset angle search", "USE_FIND_ANGLE", false);
                AssertFloatingSelectedObjectBooleanProperty("Matching basic preset scale search", "USE_FIND_SCALE", false);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching basic preset min score", "SCORE_MIN", 0.6D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching basic preset count", "NUM_MATCH", 1D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Matching basic preset magnification", "MAGNIFIATION", 1.0D, 0.001D);
                AssertFloatingPropertyBrowsable("Matching basic preset angle step hidden", "FIND_ANGLE", false);
                AssertActiveToolTextsVisible("Matching preset applied detail", "기본 적용됨", "미리보기로 검증");
                if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Matching basic preset must update PropertyGrid only, not run preview.");
                }

                AssertDockActiveNativeTool(shellHost, "MatchingToolWpfView", "Docked Matching preset menu");
                AssertActiveToolButtonVisible("btnPresetMenu", "Docked Matching preset menu button", true);
                AssertFloatingPropertyGridMinimumSize("Docked Matching preset property grid", 600D, 380D);

                int beforeDockedPresetRuns = shellHost.NativePreviewRunCount;
                ClickActiveToolPresetMenuItem("fast", "Docked Matching fast preset");
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingSelectedObjectBooleanProperty("Docked Matching fast preset auto preview", "AUTO_PREVIEW", false);
                AssertFloatingSelectedObjectBooleanProperty("Docked Matching fast preset angle search", "USE_FIND_ANGLE", false);
                AssertFloatingSelectedObjectNumericPropertyWithin("Docked Matching fast preset min score", "SCORE_MIN", 0.7D, 0.001D);
                AssertFloatingSelectedObjectNumericPropertyWithin("Docked Matching fast preset magnification", "MAGNIFIATION", 0.5D, 0.001D);
                AssertFloatingPropertyBrowsable("Docked Matching fast preset angle step hidden", "FIND_ANGLE", false);
                AssertFloatingPropertyGridMinimumSize("Docked Matching preset property grid after menu", 600D, 380D);
                if (shellHost.NativePreviewRunCount != beforeDockedPresetRuns || shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Docked Matching fast preset menu must update PropertyGrid only, not run preview.");
                }
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static CaptureResult CaptureShellHostMatchingPyramidPropertyGrid(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostMatchingPyramidPropertyGrid");
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(matchingBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(16);
                AssertFloatingPropertyGridRowsRendered("Matching pyramid property grid");
                AssertFloatingPropertyGridMinimumSize("Matching pyramid property grid", 600D, 380D);

                SetFloatingPropertyGridPropertyValue("Matching pyramid auto preview off", "AUTO_PREVIEW", false);
                SetFloatingPropertyGridPropertyValue("Matching pyramid disable angle search", "USE_FIND_ANGLE", false);
                SetFloatingPropertyGridPropertyValue("Matching pyramid enable scale search", "USE_FIND_SCALE", true);
                SetFloatingPropertyGridPropertyValue("Matching pyramid proposal reset", "USE_PYRAMID_POSITION_PROPOSAL", false);
                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Thread.Sleep(180);
                Pump(30);

                AssertFloatingPropertyBrowsable("Matching pyramid parent visible", "USE_PYRAMID_POSITION_PROPOSAL", true);
                AssertFloatingPropertyBrowsable("Matching pyramid top N hidden before enable", "PYRAMID_POSITION_TOP_N", false);
                AssertFloatingPropertyBrowsable("Matching pyramid min score hidden before enable", "PYRAMID_POSITION_MIN_SCORE", false);

                int beforeManualToggles = shellHost.NativePreviewRunCount;
                SetFloatingPropertyGridPropertyValue("Matching pyramid proposal enable", "USE_PYRAMID_POSITION_PROPOSAL", true);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching pyramid top N visible after enable", "PYRAMID_POSITION_TOP_N", true);
                AssertFloatingPropertyBrowsable("Matching pyramid min score visible after enable", "PYRAMID_POSITION_MIN_SCORE", true);
                if (shellHost.NativePreviewRunCount != beforeManualToggles)
                {
                    throw new InvalidOperationException("Matching Pyramid proposal enable must only update child rows while AUTO_PREVIEW is false.");
                }

                SetFloatingPropertyGridPropertyValue("Matching pyramid top N set", "PYRAMID_POSITION_TOP_N", 6);
                SetFloatingPropertyGridPropertyValue("Matching pyramid min score set", "PYRAMID_POSITION_MIN_SCORE", 0.72D);
                AssertFloatingSelectedObjectNumericProperty("Matching pyramid top N value", "PYRAMID_POSITION_TOP_N", 6D);
                AssertFloatingSelectedObjectNumericProperty("Matching pyramid min score value", "PYRAMID_POSITION_MIN_SCORE", 0.72D);

                int beforeAutoPreviewToggle = shellHost.NativePreviewRunCount;
                SetFloatingPropertyGridPropertyValue("Matching pyramid auto preview on", "AUTO_PREVIEW", true);
                Thread.Sleep(180);
                Pump(30);
                if (shellHost.NativePreviewRunCount != beforeAutoPreviewToggle)
                {
                    throw new InvalidOperationException("Matching AUTO_PREVIEW enable must not run preview by itself in pyramid smoke.");
                }

                SetFloatingPropertyGridPropertyValue("Matching pyramid proposal disable", "USE_PYRAMID_POSITION_PROPOSAL", false);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching pyramid top N hidden after disable", "PYRAMID_POSITION_TOP_N", false);
                AssertFloatingPropertyBrowsable("Matching pyramid min score hidden after disable", "PYRAMID_POSITION_MIN_SCORE", false);
                if (shellHost.NativePreviewRunCount != beforeAutoPreviewToggle)
                {
                    throw new InvalidOperationException("Matching Pyramid proposal disable must not auto-preview even when AUTO_PREVIEW is true.");
                }

                SetFloatingPropertyGridPropertyValue("Matching pyramid proposal re-enable", "USE_PYRAMID_POSITION_PROPOSAL", true);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyBrowsable("Matching pyramid top N visible after re-enable", "PYRAMID_POSITION_TOP_N", true);
                AssertFloatingPropertyBrowsable("Matching pyramid min score visible after re-enable", "PYRAMID_POSITION_MIN_SCORE", true);
                AssertFloatingSelectedObjectNumericProperty("Matching pyramid top N preserved", "PYRAMID_POSITION_TOP_N", 6D);
                AssertFloatingSelectedObjectNumericProperty("Matching pyramid min score preserved", "PYRAMID_POSITION_MIN_SCORE", 0.72D);
                if (shellHost.NativePreviewRunCount != beforeAutoPreviewToggle)
                {
                    throw new InvalidOperationException("Matching Pyramid proposal re-enable must not auto-preview even when AUTO_PREVIEW is true.");
                }
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static CaptureResult CaptureLayerSelectionMainOutputCreation(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionMainOutputCreation", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        shellHost.SetMainLayerImageForTest(mainBitmap);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);

            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Main output creation input combo");
            AssertVisionToolComboTemplate(outputLayerCombo, "Main output creation output combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Main output creation input combo");
            AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Main output creation input combo");
            if (!ComboBoxContainsText(inputLayerCombo, "Main")
                || ComboBoxContainsText(inputLayerCombo, "Threshold_Preview")
                || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold did not start with Main as the fixed input route. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
            }

            shellHost.CreateActiveNativeOutputLayerForTest();
            Pump(18);

            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertComboBoxPopupLayout(inputLayerCombo, "Main output creation input combo after output creation");
            AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Main output creation input combo after output creation");
            AssertComboBoxPopupLayout(outputLayerCombo, "Main output creation output combo after output creation");
            if (!ComboBoxContainsText(inputLayerCombo, "Main")
                || ComboBoxContainsText(inputLayerCombo, "Threshold_Preview")
                || !ComboBoxContainsText(outputLayerCombo, "Threshold_Preview")
                || !shellHost.HasLayerForTest("Threshold_Preview")
                || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Output layer creation moved the default Main input route. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, HasThreshold={shellHost.HasLayerForTest("Threshold_Preview")}");
            }

            shellHost.CreateActiveNativeOutputLayerForTest();
            Pump(18);

            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertComboBoxPopupLayout(inputLayerCombo, "Main output creation input combo after second output creation");
            AssertComboBoxPopupLayout(outputLayerCombo, "Main output creation output combo after second output creation");
            if (!ComboBoxContainsText(inputLayerCombo, "Main")
                || !ComboBoxContainsText(inputLayerCombo, "Threshold_Preview")
                || ComboBoxContainsText(inputLayerCombo, "Threshold_Preview_001")
                || !ComboBoxContainsText(outputLayerCombo, "Threshold_Preview")
                || !ComboBoxContainsText(outputLayerCombo, "Threshold_Preview_001")
                || !shellHost.HasLayerForTest("Threshold_Preview_001")
                || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), "Threshold_Preview_001", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Threshold_Preview_001", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Second output creation did not branch to a new result layer while preserving Main as input. "
                    + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, HasThreshold={shellHost.HasLayerForTest("Threshold_Preview")}, HasThreshold001={shellHost.HasLayerForTest("Threshold_Preview_001")}");
            }
        });
    }
    private static CaptureResult CaptureLayerSelectionThresholdTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionThreshold", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap auxBitmap = CreateDockingPanelSmokeBitmap(3);
        shellHost.SetMainLayerImageForTest(mainBitmap);
        if (!shellHost.AddLayerImageForTest("Aux_Threshold_Input", auxBitmap))
        {
            throw new InvalidOperationException("Aux threshold input layer could not be created.");
        }

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(16);

            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Layer selection threshold input combo");
            AssertVisionToolComboTemplate(outputLayerCombo, "Layer selection threshold output combo");
            AssertComboBoxPopupLayout(inputLayerCombo, "Layer selection threshold input combo");
            AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Layer selection threshold input combo");
            AssertComboBoxPopupLayout(outputLayerCombo, "Layer selection threshold output combo");
            if (ComboBoxContainsText(inputLayerCombo, "Threshold_Preview"))
            {
                throw new InvalidOperationException("Threshold input combo included its own preview output layer before creation.");
            }

            SelectComboBoxItemText(inputLayerCombo, "Aux_Threshold_Input", "Layer selection threshold input combo");
            Pump(12);
            if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold input route did not follow the selected Aux layer. "
                    + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
            }

            shellHost.CreateActiveNativeOutputLayerForTest();
            Pump(16);
            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertComboBoxPopupLayout(inputLayerCombo, "Layer selection threshold input combo after output creation");
            AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Layer selection threshold input combo after output creation");
            AssertComboBoxPopupLayout(outputLayerCombo, "Layer selection threshold output combo after output creation");
            if (!ComboBoxContainsText(outputLayerCombo, "Threshold_Preview"))
            {
                throw new InvalidOperationException("Threshold output combo lost its current output layer item after creation.");
            }

            if (ComboBoxContainsText(inputLayerCombo, "Threshold_Preview")
                || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold output layer creation changed or exposed the input layer route. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            if (!shellHost.HasNativePreviewResult
                || ComboBoxContainsText(inputLayerCombo, "Threshold_Preview")
                || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Threshold_Input", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Threshold_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Threshold preview did not preserve the selected input route. "
                    + $"Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }
        });
    }

    private static CaptureResult CaptureLayerSelectionExistingOutputWrite(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionExistingOutputWrite", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap existingOutputBitmap = CreateDockingPanelSmokeBitmap(9);
        shellHost.SetMainLayerImageForTest(mainBitmap);
        if (!shellHost.AddLayerImageForTest("Operator_Output", existingOutputBitmap))
        {
            throw new InvalidOperationException("Existing operator output layer could not be created.");
        }

        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException("Existing output write smoke could not reactivate Main.");
        }

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Threshold);
            Pump(20);

            ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertVisionToolComboTemplate(inputLayerCombo, "Existing output write input combo");
            AssertVisionToolComboTemplate(outputLayerCombo, "Existing output write output combo");
            AssertComboBoxPopupLayout(outputLayerCombo, "Existing output write output combo");

            if (!ComboBoxContainsText(outputLayerCombo, "Operator_Output"))
            {
                throw new InvalidOperationException("Output combo did not expose the existing operator output layer.");
            }

            SelectComboBoxItemText(outputLayerCombo, "Operator_Output", "Existing output write output combo");
            Pump(20);
            if (!string.Equals(GetComboBoxCurrentText(outputLayerCombo), "Operator_Output", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Operator_Output", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Selecting an existing output layer did not preserve the explicit input/output route. "
                    + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}");
            }

            using Bitmap beforeOutput = shellHost.GetLayerImageCloneForTest("Operator_Output");
            int beforeRuns = shellHost.NativePreviewRunCount;
            shellHost.RunActiveNativePreviewForTest();
            Pump(40);

            inputLayerCombo = FindFloatingComboBox("cbInputLayer");
            outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            if (!shellHost.HasNativePreviewResult
                || shellHost.NativePreviewRunCount <= beforeRuns
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Operator_Output", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), "Operator_Output", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
                || shellHost.HasLayerForTest("Threshold_Preview"))
            {
                throw new InvalidOperationException(
                    "Preview did not write into the selected existing output layer without route side effects. "
                    + $"RunsBefore={beforeRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                    + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, "
                    + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                    + $"HasDefaultOutput={shellHost.HasLayerForTest("Threshold_Preview")}, Status={shellHost.ActiveNativeStatusText}");
            }

            using Bitmap afterOutput = shellHost.GetLayerImageCloneForTest("Operator_Output");
            using Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main");
            AssertBitmapPresent(afterOutput, "Operator_Output after selected-output preview");
            AssertBitmapVisiblyDifferent(beforeOutput, afterOutput, "Existing output layer should be overwritten by preview");
            AssertBitmapVisiblyDifferent(mainLayer, afterOutput, "Existing output preview should differ from Main");
            AssertBitmapBinaryLike(afterOutput, "Operator_Output threshold result");
            SaveDiagnosticBitmap(outputPath, "existing-output-before.png", beforeOutput);
            SaveDiagnosticBitmap(outputPath, "existing-output-after.png", afterOutput);
        });
    }

    private static CaptureResult CaptureLayerSelectionAlgorithmExistingOutputWrite(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionAlgorithmExistingOutputWrite", seedMainLayer: false);
        using Bitmap workspaceBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        using Bitmap featureBitmap = CreateFeatureMatchingSmokeBitmap();
        string matchingTemplatePath = CreateMatchingTemplateFile(matchingBitmap);
        string featureTemplatePath = CreateFeatureMatchingTemplateFile(featureBitmap);

        try
        {
            shellHost.SetMainLayerImageForTest(workspaceBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                VerifyAlgorithmExistingOutputWrite(
                    shellHost,
                    outputPath,
                    VISION_MENU.Blob,
                    "BlobToolWpfView",
                    "Blob_Preview",
                    "Blob_Operator_Output",
                    workspaceBitmap,
                    null);
                VerifyAlgorithmExistingOutputWrite(
                    shellHost,
                    outputPath,
                    VISION_MENU.Contour,
                    "ContourToolWpfView",
                    "Contour_Preview",
                    "Contour_Operator_Output",
                    workspaceBitmap,
                    null);
                VerifyAlgorithmExistingOutputWrite(
                    shellHost,
                    outputPath,
                    VISION_MENU.Matching,
                    "MatchingToolWpfView",
                    "Matching_Preview",
                    "Matching_Operator_Output",
                    matchingBitmap,
                    host => host.SetActiveMatchingTemplatePathForTest(matchingTemplatePath));
                VerifyAlgorithmExistingOutputWrite(
                    shellHost,
                    outputPath,
                    VISION_MENU.EdgeBasedMatching,
                    "EdgeBasedMatchingToolWpfView",
                    "EdgeBasedMatching_Preview",
                    "Edge_Operator_Output",
                    matchingBitmap,
                    host => host.SetActiveEdgeBasedMatchingTemplatePathForTest(matchingTemplatePath));
                VerifyAlgorithmExistingOutputWrite(
                    shellHost,
                    outputPath,
                    VISION_MENU.FeatureMatching,
                    "FeatureMatchingToolWpfView",
                    "FeatureMatching_Preview",
                    "Feature_Operator_Output",
                    featureBitmap,
                    host => host.SetActiveFeatureMatchingTemplatePathForTest(featureTemplatePath));
            });
        }
        finally
        {
            TryDeleteFile(matchingTemplatePath);
            TryDeleteFile(featureTemplatePath);
        }
    }

    private static CaptureResult CaptureLayerSelectionPreprocessExistingOutputWrite(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionPreprocessExistingOutputWrite", seedMainLayer: false);
        using Bitmap workspaceBitmap = CreateWorkspaceSeedSmokeBitmap();

        shellHost.SetMainLayerImageForTest(workspaceBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            foreach ((VISION_MENU Menu, string DocumentType, string OutputLayer) scenario in GetPreprocessOutputPreviewScenarios())
            {
                VerifyPreprocessExistingOutputWrite(
                    shellHost,
                    outputPath,
                    scenario.Menu,
                    scenario.DocumentType,
                    scenario.OutputLayer,
                    scenario.Menu + "_Operator_Output",
                    workspaceBitmap);
            }
        });
    }

    private static CaptureResult CaptureLayerSelectionMatchingTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionMatching", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(mainBitmap);
            if (!shellHost.AddLayerImageForTest("Aux_Matching_Input", matchingBitmap))
            {
                throw new InvalidOperationException("Aux matching input layer could not be created.");
            }

            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.Matching);
                Pump(16);

                ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                AssertVisionToolComboTemplate(inputLayerCombo, "Layer selection matching input combo");
                AssertVisionToolComboTemplate(outputLayerCombo, "Layer selection matching output combo");
                AssertComboBoxPopupLayout(inputLayerCombo, "Layer selection matching input combo");
                AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, "Layer selection matching input combo");
                AssertComboBoxPopupLayout(outputLayerCombo, "Layer selection matching output combo");

                if (!outputLayerCombo.IsEditable)
                {
                    throw new InvalidOperationException("Matching output layer combo must be editable for new output layer names.");
                }

                if (inputLayerCombo.Items.Cast<object>().Any(item =>
                    string.Equals(Convert.ToString(item), "Matching_Preview", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Matching input combo included a non-existing preview output layer before it was created.");
                }

                SelectComboBoxItemText(inputLayerCombo, "Aux_Matching_Input", "Layer selection matching input combo");
                Pump(12);
                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Matching_Input", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Matching input route did not follow the selected Aux layer. Actual="
                        + shellHost.ActiveNativeRouteInputLayerNameForTest);
                }

                if (!string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Matching_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Matching output route lost the default preview layer. Actual="
                        + shellHost.ActiveNativeRouteOutputLayerNameForTest);
                }

                shellHost.CreateActiveNativeOutputLayerForTest();
                Pump(12);
                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Matching_Input", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Matching_Input", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Matching output layer creation changed the active input layer. "
                        + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
                }

                shellHost.SetActiveMatchingTemplatePathForTest(templatePath);
                Pump(8);
                shellHost.RunActiveNativePreviewForTest();
                Pump(24);

                if (!shellHost.HasNativePreviewResult
                    || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Matching_Input", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Matching_Input", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Matching_Preview", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Matching preview did not preserve the selected input/output layer route. "
                        + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
                }
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static void VerifyPreprocessExistingOutputWrite(
        OpenVisionShellHostView shellHost,
        string outputPath,
        VISION_MENU menu,
        string expectedDocumentType,
        string defaultOutputLayer,
        string existingOutputLayer,
        Bitmap sourceBitmap)
    {
        shellHost.SetMainLayerImageForTest(sourceBitmap);
        Pump(10);

        using Bitmap seedOutput = CreateDockingPanelSmokeBitmap(existingOutputLayer.Length % 13 + 1);
        if (!shellHost.HasLayerForTest(existingOutputLayer))
        {
            if (!shellHost.AddLayerImageForTest(existingOutputLayer, seedOutput))
            {
                throw new InvalidOperationException($"{menu} existing output layer could not be created.");
            }
        }
        else if (!shellHost.SetLayerImageForTest(existingOutputLayer, seedOutput))
        {
            throw new InvalidOperationException($"{menu} existing output layer could not be reset.");
        }

        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException($"{menu} could not reactivate Main before selected-output preview.");
        }

        Pump(8);
        shellHost.SelectToolForTest(menu);
        Pump(18);

        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        AssertVisionToolComboTemplate(inputLayerCombo, menu + " preprocess existing output input combo");
        AssertVisionToolComboTemplate(outputLayerCombo, menu + " preprocess existing output output combo");
        AssertComboBoxPopupLayout(outputLayerCombo, menu + " preprocess existing output output combo");
        if (!ComboBoxContainsText(outputLayerCombo, existingOutputLayer))
        {
            throw new InvalidOperationException($"{menu} output combo does not expose existing output layer {existingOutputLayer}.");
        }

        SelectComboBoxItemText(outputLayerCombo, existingOutputLayer, menu + " preprocess existing output output combo");
        Pump(14);
        if (!string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " did not preserve Main input while selecting an existing output layer. "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
        }

        if (!shellHost.SetLayerImageForTest(existingOutputLayer, seedOutput))
        {
            throw new InvalidOperationException($"{menu} existing output layer could not be reset before explicit preview.");
        }

        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException($"{menu} could not reactivate Main after resetting selected output layer.");
        }

        Pump(8);
        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " did not preserve selected routes after output reset. "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
        }

        using Bitmap beforeOutput = shellHost.GetLayerImageCloneForTest(existingOutputLayer);
        int beforeRuns = shellHost.NativePreviewRunCount;
        shellHost.RunActiveNativePreviewForTest();
        Pump(40);

        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!shellHost.IsNativeDocumentActive
            || !shellHost.HasNativePreviewResult
            || shellHost.NativePreviewRunCount <= beforeRuns
            || !shellHost.ActiveNativeDocumentTypeName.Contains(expectedDocumentType, StringComparison.Ordinal)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
            || shellHost.HasLayerForTest(defaultOutputLayer))
        {
            throw new InvalidOperationException(
                menu + " preview did not write into the selected existing output layer without route side effects. "
                + $"RunsBefore={beforeRuns}, RunsAfter={shellHost.NativePreviewRunCount}, Document={shellHost.ActiveNativeDocumentTypeName}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Active={shellHost.ActiveHostLayerTitle}, HasDefaultOutput={shellHost.HasLayerForTest(defaultOutputLayer)}, "
                + $"Status={shellHost.ActiveNativeStatusText}");
        }

        using Bitmap afterOutput = shellHost.GetLayerImageCloneForTest(existingOutputLayer);
        SaveDiagnosticBitmap(outputPath, menu + "-preprocess-existing-output-before.png", beforeOutput);
        SaveDiagnosticBitmap(outputPath, menu + "-preprocess-existing-output-after.png", afterOutput);
        AssertBitmapPresent(afterOutput, menu + " preprocess existing output after selected-output preview");
        AssertBitmapVisiblyDifferent(beforeOutput, afterOutput, menu + " preprocess existing output layer should be overwritten by preview");
    }

    private static CaptureResult CaptureLayerSelectionArithmeticTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionArithmetic", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap inputABitmap = CreateDockingPanelSmokeBitmap(1);
        using Bitmap inputBBitmap = CreateDockingPanelSmokeBitmap(2);
        string inputBLoadPath = CreateWorkspaceLoadSmokeImageFile();
        shellHost.SetMainLayerImageForTest(mainBitmap);
        if (!shellHost.AddLayerImageForTest("Aux_Arithmetic_A", inputABitmap)
            || !shellHost.AddLayerImageForTest("Aux_Arithmetic_B", inputBBitmap))
        {
            throw new InvalidOperationException("Aux arithmetic input layers could not be created.");
        }

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Arithmetic);
            Pump(16);

            ClickFloatingRadioButtonByName("rdoModeOperation", "Layer selection arithmetic operation mode radio");
            Pump(12);

            ComboBox inputACombo = FindFloatingComboBox("cbInputA");
            ComboBox inputBCombo = FindFloatingComboBox("cbInputB");
            ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
            AssertVisionToolComboTemplate(inputACombo, "Layer selection arithmetic input A combo");
            AssertVisionToolComboTemplate(inputBCombo, "Layer selection arithmetic input B combo");
            AssertVisionToolComboTemplate(outputLayerCombo, "Layer selection arithmetic output combo");
            AssertComboBoxPopupLayout(inputACombo, "Layer selection arithmetic input A combo");
            AssertComboBoxOpensFromPreviewMouseClick(inputACombo, "Layer selection arithmetic input A combo");
            AssertComboBoxPopupLayout(inputBCombo, "Layer selection arithmetic input B combo");
            AssertComboBoxOpensFromPreviewMouseClick(inputBCombo, "Layer selection arithmetic input B combo");
            AssertComboBoxPopupLayout(outputLayerCombo, "Layer selection arithmetic output combo");

            if (inputACombo.Items.Count > 1
                && inputBCombo.Items.Count > 1
                && string.Equals(GetComboBoxCurrentText(inputACombo), GetComboBoxCurrentText(inputBCombo), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic input B defaulted to the same layer as input A even though another input layer exists. "
                    + $"A={GetComboBoxCurrentText(inputACombo)}, B={GetComboBoxCurrentText(inputBCombo)}");
            }

            if (!outputLayerCombo.IsEditable)
            {
                throw new InvalidOperationException("Arithmetic output layer combo must be editable for new output layer names.");
            }

            if (inputACombo.Items.Cast<object>().Any(item =>
                    string.Equals(Convert.ToString(item), "Arithmetic_Preview", StringComparison.OrdinalIgnoreCase))
                || inputBCombo.Items.Cast<object>().Any(item =>
                    string.Equals(Convert.ToString(item), "Arithmetic_Preview", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Arithmetic input combo included a non-existing preview output layer before it was created.");
            }

            SelectComboBoxItemText(inputACombo, "Aux_Arithmetic_A", "Layer selection arithmetic input A combo");
            Pump(8);
            SelectComboBoxItemText(inputBCombo, "Aux_Arithmetic_B", "Layer selection arithmetic input B combo");
            Pump(12);

            if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerBNameForTest, "Aux_Arithmetic_B", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "Arithmetic_Preview", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic route did not follow selected A/B input layers. "
                    + $"A={shellHost.ActiveNativeRouteInputLayerNameForTest}, B={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
            }

            SelectComboBoxItemText(inputBCombo, "Aux_Arithmetic_A", "Layer selection arithmetic input B combo");
            Pump(8);
            if (!shellHost.LoadActiveNativePreviewImageFromFileForTest(inputBLoadPath, VisionToolPreviewImageRole.InputB))
            {
                throw new InvalidOperationException("Arithmetic input B image load failed. Status=" + shellHost.ActiveNativeStatusText);
            }

            Pump(12);
            if (!shellHost.HasLayerForTest("Arithmetic_InputB")
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerBNameForTest, "Arithmetic_InputB", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic input B load did not prepare an independent B layer. "
                    + $"A={shellHost.ActiveNativeRouteInputLayerNameForTest}, B={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Status={shellHost.ActiveNativeStatusText}");
            }

            SelectComboBoxItemText(inputBCombo, "Aux_Arithmetic_B", "Layer selection arithmetic input B combo");
            Pump(12);

            shellHost.CreateActiveNativeOutputLayerForTest();
            Pump(12);
            string createdArithmeticOutputLayer = shellHost.ActiveNativeRouteOutputLayerNameForTest;
            if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerBNameForTest, "Aux_Arithmetic_B", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic output layer creation changed the active input layer. "
                    + $"A={shellHost.ActiveNativeRouteInputLayerNameForTest}, B={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
            }

            ComboBox arithmeticTypeCombo = FindFloatingComboBox("cbArithmeticType");
            int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
            string selectedOperation = SelectDifferentComboBoxItemText(
                arithmeticTypeCombo,
                "Layer selection arithmetic type combo",
                IsArithmeticOperationWithInputB);
            Thread.Sleep(180);
            Pump(30);

            if (!shellHost.HasNativePreviewResult
                || shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerBNameForTest, "Aux_Arithmetic_B", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, createdArithmeticOutputLayer, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic parameter change did not auto-preview while preserving the selected input/output layer route. "
                    + $"Operation={selectedOperation}, RunsBefore={beforeAutoPreviewRuns}, RunsAfter={shellHost.NativePreviewRunCount}, A={shellHost.ActiveNativeRouteInputLayerNameForTest}, B={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertActiveFloatingInlinePreviewSlotCount("Arithmetic auto-preview A/B/output", 3);
            AssertNoAutoDockedLayers(shellHost, "Arithmetic operation auto-preview");

            int beforeOffsetPreviewRuns = shellHost.NativePreviewRunCount;
            ClickFloatingRadioButtonByName("rdoModeOffset", "Arithmetic offset mode radio");
            SetFloatingTextBoxTextByName("txtOffsetX", "12", "Arithmetic offset X text");
            Thread.Sleep(180);
            Pump(30);

            if (!shellHost.HasNativePreviewResult
                || shellHost.NativePreviewRunCount <= beforeOffsetPreviewRuns
                || !shellHost.ActiveNativeStatusText.Contains("Offset OK", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_Arithmetic_A", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, createdArithmeticOutputLayer, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Arithmetic offset mode did not auto-preview through the offset execution path. "
                    + $"RunsBefore={beforeOffsetPreviewRuns}, RunsAfter={shellHost.NativePreviewRunCount}, A={shellHost.ActiveNativeRouteInputLayerNameForTest}, B={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
            }

            AssertActiveFloatingInlinePreviewSlotCount("Arithmetic offset auto-preview input/output", 2);
            AssertNoAutoDockedLayers(shellHost, "Arithmetic offset auto-preview");

            inputACombo.IsDropDownOpen = false;
            inputBCombo.IsDropDownOpen = false;
            outputLayerCombo.IsDropDownOpen = false;
            arithmeticTypeCombo.IsDropDownOpen = false;
            Pump(8);

            if (!shellHost.DockActiveWpfToolWindowForTest())
            {
                throw new InvalidOperationException("Arithmetic floating tool did not accept the dock-to-right request.");
            }

            Pump(24);
            if (!shellHost.IsDockedToolInspectorVisibleForTest
                || !shellHost.ActiveWpfToolWindowTypeName.Contains("ArithmeticToolWpfView", StringComparison.Ordinal)
                || CountVisibleFloatingToolWindows() != 0)
            {
                throw new InvalidOperationException(
                    "Docked Arithmetic should reuse the inspector and close the floating host. "
                    + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                    + $"FloatingWindows={CountVisibleFloatingToolWindows()}");
            }

            AssertDockedDoubleInputToolLayout("Docked Arithmetic tool layout");
        });
    }

    private static CaptureResult CaptureLayerSelectionAllNativeTools(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfLayerSelectionAllNativeTools", seedMainLayer: false);
        using Bitmap mainBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap auxBitmap = CreateDockingPanelSmokeBitmap(3);
        using Bitmap auxBBitmap = CreateDockingPanelSmokeBitmap(4);
        shellHost.SetMainLayerImageForTest(mainBitmap);
        if (!shellHost.AddLayerImageForTest("Aux_AllTool_Input", auxBitmap)
            || !shellHost.AddLayerImageForTest("Aux_AllTool_InputB", auxBBitmap))
        {
            throw new InvalidOperationException("Aux all-tool input layers could not be created.");
        }

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            Pump(20);
            foreach (VISION_MENU menu in GetToolOpenPerfMenus())
            {
                shellHost.SelectToolForTest(menu);
                Pump(16);

                if (menu == VISION_MENU.Arithmetic)
                {
                    VerifyArithmeticLayerComboRoute(shellHost, menu);
                }
                else
                {
                    VerifySingleInputLayerComboRoute(shellHost, menu);
                }
            }
        });
    }

    private static void VerifySingleInputLayerComboRoute(OpenVisionShellHostView shellHost, VISION_MENU menu)
    {
        string name = menu + " input layer combo";
        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        AssertVisionToolComboTemplate(inputLayerCombo, name);
        AssertVisionToolComboTemplate(outputLayerCombo, menu + " output layer combo");
        AssertComboBoxPopupLayout(inputLayerCombo, name);
        AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, name);
        AssertComboBoxPopupLayout(outputLayerCombo, menu + " output layer combo");

        SelectComboBoxItemText(inputLayerCombo, "Aux_AllTool_Input", name);
        Pump(12);
        string outputLayer = shellHost.ActiveNativeRouteOutputLayerNameForTest;
        if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(menu + " did not switch input route to Aux_AllTool_Input. "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
        }

        shellHost.CreateActiveNativeOutputLayerForTest();
        Pump(14);
        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        AssertComboBoxPopupLayout(inputLayerCombo, name + " after output creation");
        AssertComboBoxOpensFromPreviewMouseClick(inputLayerCombo, name + " after output creation");
        AssertComboBoxPopupLayout(outputLayerCombo, menu + " output layer combo after output creation");

        // A tool may consume other tools' outputs, but not its own destination layer.
        if (ComboBoxContainsText(inputLayerCombo, outputLayer)
            || !ComboBoxContainsText(outputLayerCombo, outputLayer)
            || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(menu + " output creation corrupted the input layer route. "
                + $"Output={outputLayer}, Combo={GetComboBoxCurrentText(inputLayerCombo)}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
        }
    }

    private static void VerifyArithmeticLayerComboRoute(OpenVisionShellHostView shellHost, VISION_MENU menu)
    {
        ComboBox inputACombo = FindFloatingComboBox("cbInputA");
        ComboBox inputBCombo = FindFloatingComboBox("cbInputB");
        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        AssertVisionToolComboTemplate(inputACombo, "Arithmetic input A combo");
        AssertVisionToolComboTemplate(inputBCombo, "Arithmetic input B combo");
        AssertVisionToolComboTemplate(outputLayerCombo, "Arithmetic output layer combo");
        AssertComboBoxPopupLayout(inputACombo, "Arithmetic input A combo");
        AssertComboBoxOpensFromPreviewMouseClick(inputACombo, "Arithmetic input A combo");
        AssertComboBoxPopupLayout(inputBCombo, "Arithmetic input B combo");
        AssertComboBoxOpensFromPreviewMouseClick(inputBCombo, "Arithmetic input B combo");
        AssertComboBoxPopupLayout(outputLayerCombo, "Arithmetic output layer combo");

        SelectComboBoxItemText(inputACombo, "Aux_AllTool_Input", "Arithmetic input A combo");
        Pump(8);
        SelectComboBoxItemText(inputBCombo, "Aux_AllTool_InputB", "Arithmetic input B combo");
        Pump(12);
        string outputLayer = shellHost.ActiveNativeRouteOutputLayerNameForTest;
        shellHost.CreateActiveNativeOutputLayerForTest();
        Pump(14);

        inputACombo = FindFloatingComboBox("cbInputA");
        inputBCombo = FindFloatingComboBox("cbInputB");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (ComboBoxContainsText(inputACombo, outputLayer)
            || ComboBoxContainsText(inputBCombo, outputLayer)
            || !ComboBoxContainsText(outputLayerCombo, outputLayer)
            || !string.Equals(GetComboBoxCurrentText(inputACombo), "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(inputBCombo), "Aux_AllTool_InputB", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerBNameForTest, "Aux_AllTool_InputB", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Aux_AllTool_Input", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Arithmetic output creation corrupted the input layer route. "
                + $"Output={outputLayer}, A={GetComboBoxCurrentText(inputACombo)}, B={GetComboBoxCurrentText(inputBCombo)}, RouteA={shellHost.ActiveNativeRouteInputLayerNameForTest}, RouteB={shellHost.ActiveNativeRouteInputLayerBNameForTest}, Active={shellHost.ActiveHostLayerTitle}");
        }
    }

    private static CaptureResult CaptureAlgorithmOutputPreviewFlow(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfAlgorithmOutputPreviewFlow", seedMainLayer: false);
        using Bitmap workspaceBitmap = CreateWorkspaceSeedSmokeBitmap();
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        using Bitmap featureBitmap = CreateFeatureMatchingSmokeBitmap();
        string matchingTemplatePath = CreateMatchingTemplateFile(matchingBitmap);
        string featureTemplatePath = CreateFeatureMatchingTemplateFile(featureBitmap);

        try
        {
            shellHost.SetMainLayerImageForTest(workspaceBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                VerifyAlgorithmOutputPreview(
                    shellHost,
                    VISION_MENU.Blob,
                    "BlobToolWpfView",
                    "Blob_Preview",
                    workspaceBitmap,
                    null,
                    "Blob /",
                    "검출",
                    "중심",
                    "박스");
                VerifyAlgorithmOutputPreview(
                    shellHost,
                    VISION_MENU.Contour,
                    "ContourToolWpfView",
                    "Contour_Preview",
                    workspaceBitmap,
                    null,
                    "Contour /",
                    "검출",
                    "중심",
                    "박스");
                VerifyAlgorithmOutputPreview(
                    shellHost,
                    VISION_MENU.Matching,
                    "MatchingToolWpfView",
                    "Matching_Preview",
                    matchingBitmap,
                    host => host.SetActiveMatchingTemplatePathForTest(matchingTemplatePath),
                    "Template Match /",
                    "검출",
                    "점수",
                    "중심",
                    "박스");
                VerifyAlgorithmOutputPreview(
                    shellHost,
                    VISION_MENU.EdgeBasedMatching,
                    "EdgeBasedMatchingToolWpfView",
                    "EdgeBasedMatching_Preview",
                    matchingBitmap,
                    host => host.SetActiveEdgeBasedMatchingTemplatePathForTest(matchingTemplatePath),
                    "Edge Match /",
                    "검출",
                    "점수");
                VerifyAlgorithmOutputPreview(
                    shellHost,
                    VISION_MENU.FeatureMatching,
                    "FeatureMatchingToolWpfView",
                    "FeatureMatching_Preview",
                    featureBitmap,
                    host => host.SetActiveFeatureMatchingTemplatePathForTest(featureTemplatePath),
                    "Feature Match /",
                    "검출",
                    "점수",
                    "중심",
                    "박스");
            });
        }
        finally
        {
            TryDeleteFile(matchingTemplatePath);
            TryDeleteFile(featureTemplatePath);
        }
    }

    private static void VerifyAlgorithmOutputPreview(
        OpenVisionShellHostView shellHost,
        VISION_MENU menu,
        string expectedDocumentType,
        string expectedOutputLayer,
        Bitmap sourceBitmap,
        Action<OpenVisionShellHostView>? configure,
        params string[] resultReviewTokens)
    {
        shellHost.SetMainLayerImageForTest(sourceBitmap);
        Pump(10);
        shellHost.ActivateHostLayerForTest("Main");
        Pump(8);
        shellHost.SelectToolForTest(menu);
        Pump(18);
        AssertActiveFloatingInlinePreviewSlotCount(menu + " input", 1);

        configure?.Invoke(shellHost);
        Pump(10);
        shellHost.RunActiveNativePreviewForTest();
        Pump(30);

        if (!shellHost.IsNativeDocumentActive
            || !shellHost.HasNativePreviewResult
            || !shellHost.ActiveNativeDocumentTypeName.Contains(expectedDocumentType, StringComparison.Ordinal)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, expectedOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " did not produce a routed algorithm preview. "
                + $"Document={shellHost.ActiveNativeDocumentTypeName}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Status={shellHost.ActiveNativeStatusText}");
        }

        AssertActiveFloatingInlinePreviewSlotCount(menu + " input/output", 2);
        AssertResultReviewVisible(menu.ToString(), resultReviewTokens);
        AssertResultReviewDoesNotContain(menu.ToString(), "검출 0");
    }

    private static void VerifyAlgorithmExistingOutputWrite(
        OpenVisionShellHostView shellHost,
        string outputPath,
        VISION_MENU menu,
        string expectedDocumentType,
        string defaultOutputLayer,
        string existingOutputLayer,
        Bitmap sourceBitmap,
        Action<OpenVisionShellHostView>? configure)
    {
        shellHost.SetMainLayerImageForTest(sourceBitmap);
        Pump(10);

        using Bitmap seedOutput = CreateDockingPanelSmokeBitmap(existingOutputLayer.Length % 11 + 1);
        if (!shellHost.AddLayerImageForTest(existingOutputLayer, seedOutput))
        {
            throw new InvalidOperationException($"{menu} existing output layer could not be created.");
        }

        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException($"{menu} could not reactivate Main before selected-output preview.");
        }

        Pump(8);
        shellHost.SelectToolForTest(menu);
        Pump(18);

        ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        AssertVisionToolComboTemplate(inputLayerCombo, menu + " existing output input combo");
        AssertVisionToolComboTemplate(outputLayerCombo, menu + " existing output output combo");
        AssertComboBoxPopupLayout(outputLayerCombo, menu + " existing output output combo");
        if (!ComboBoxContainsText(outputLayerCombo, existingOutputLayer))
        {
            throw new InvalidOperationException($"{menu} output combo does not expose existing output layer {existingOutputLayer}.");
        }

        SelectComboBoxItemText(outputLayerCombo, existingOutputLayer, menu + " existing output output combo");
        Pump(14);
        if (!string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " did not preserve Main input while selecting an existing output layer. "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
        }

        configure?.Invoke(shellHost);
        Pump(10);

        if (!shellHost.SetLayerImageForTest(existingOutputLayer, seedOutput))
        {
            throw new InvalidOperationException($"{menu} existing output layer could not be reset before explicit preview.");
        }

        if (!shellHost.ActivateHostLayerForTest("Main"))
        {
            throw new InvalidOperationException($"{menu} could not reactivate Main after resetting selected output layer.");
        }

        Pump(8);
        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                menu + " did not preserve selected routes after resetting the existing output layer. "
                + $"InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
        }

        using Bitmap beforeOutput = shellHost.GetLayerImageCloneForTest(existingOutputLayer);
        int beforeRuns = shellHost.NativePreviewRunCount;
        shellHost.RunActiveNativePreviewForTest();
        Pump(40);

        inputLayerCombo = FindFloatingComboBox("cbInputLayer");
        outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
        if (!shellHost.IsNativeDocumentActive
            || !shellHost.HasNativePreviewResult
            || shellHost.NativePreviewRunCount <= beforeRuns
            || !shellHost.ActiveNativeDocumentTypeName.Contains(expectedDocumentType, StringComparison.Ordinal)
            || !string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(inputLayerCombo), "Main", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(GetComboBoxCurrentText(outputLayerCombo), existingOutputLayer, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase)
            || shellHost.HasLayerForTest(defaultOutputLayer))
        {
            throw new InvalidOperationException(
                menu + " preview did not write into the selected existing output layer without route side effects. "
                + $"RunsBefore={beforeRuns}, RunsAfter={shellHost.NativePreviewRunCount}, "
                + $"Document={shellHost.ActiveNativeDocumentTypeName}, Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, "
                + $"Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, InputCombo={GetComboBoxCurrentText(inputLayerCombo)}, "
                + $"OutputCombo={GetComboBoxCurrentText(outputLayerCombo)}, Active={shellHost.ActiveHostLayerTitle}, "
                + $"HasDefaultOutput={shellHost.HasLayerForTest(defaultOutputLayer)}, Status={shellHost.ActiveNativeStatusText}");
        }

        using Bitmap afterOutput = shellHost.GetLayerImageCloneForTest(existingOutputLayer);
        using Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main");
        SaveDiagnosticBitmap(outputPath, menu + "-existing-output-before.png", beforeOutput);
        SaveDiagnosticBitmap(outputPath, menu + "-existing-output-after.png", afterOutput);
        AssertBitmapPresent(afterOutput, menu + " existing output after selected-output preview");
        AssertBitmapVisiblyDifferent(beforeOutput, afterOutput, menu + " existing output layer should be overwritten by preview");
        AssertBitmapPreviewOverlayDifferentFromMain(mainLayer, afterOutput, menu + " existing output preview should differ from Main");
    }

    private static CaptureResult CaptureMatchingAngleEasyMatch(string outputPath)
    {
        DateTime started = DateTime.UtcNow;
        string easyMatchDirectory = FindEasyMatchDirectory();
        string[] sourceNames = { "Die Pad 1.bmp", "Die Pad 2.bmp", "Die Pad 3.bmp" };
        string[] templateNames = { "Die Pad Model 1.bmp", "Die Pad Model 2.bmp" };

        foreach (string sourceName in sourceNames)
        {
            string sourcePath = Path.Combine(easyMatchDirectory, sourceName);
            if (!File.Exists(sourcePath)) { continue; }

            foreach (string templateName in templateNames)
            {
                string templatePath = Path.Combine(easyMatchDirectory, templateName);
                if (!File.Exists(templatePath)) { continue; }

                using OpenCvSharp.Mat source = OpenCvSharp.Cv2.ImRead(sourcePath);
                using OpenCvSharp.Mat template = OpenCvSharp.Cv2.ImRead(templatePath);
                if (source.Empty() || template.Empty()) { continue; }

                MatchingProperty property = new("EasyMatchAngleSmoke")
                {
                    MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
                    SCORE_MIN = 0.55D,
                    NUM_MATCH = 3,
                    MAGNIFIATION = 1D,
                    USE_FIND_ANGLE = true,
                    FIND_ANGLE_MIN = -10,
                    FIND_ANGLE_MAX = 180,
                    FIND_ANGLE = 1D,
                    PATTERN_PATH = templatePath,
                    ImageTemplate = template.Clone()
                };

                Lib.OpenCV.Tool.MatchingTool tool = new();
                tool.SetProperty(property);
                tool.SetTemplateImage(template);
                Lib.OpenCV.Tool.VisionToolResult result = tool.Execute(source);
                List<Lib.OpenCV.Result.MatchingResult> matches = tool.results?.ToList() ?? new List<Lib.OpenCV.Result.MatchingResult>();
                Lib.OpenCV.Result.MatchingResult? best = matches.OrderByDescending(item => item.Score).FirstOrDefault();
                if (result?.Success != true || best == null || Math.Abs(best.Angle) < 0.5D)
                {
                    result?.ResultImage?.Dispose();
                    property.ImageTemplate?.Dispose();
                    continue;
                }

                OpenCvSharp.Mat overlayBase = source.Clone();
                OpenCvSharp.Mat overlay = OpenVisionNativeToolPreviewOverlayRenderer.CreateMatchingOverlayImage(overlayBase, source, matches);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
                    OpenCvSharp.Cv2.ImWrite(outputPath, overlay);
                    AssertMatchingAngleOverlayPixels(outputPath, best);
                    int outputWidth = overlay.Width;
                    int outputHeight = overlay.Height;
                    return new CaptureResult(outputWidth, outputHeight, (DateTime.UtcNow - started).TotalMilliseconds);
                }
                finally
                {
                    if (!ReferenceEquals(overlay, overlayBase))
                    {
                        overlay.Dispose();
                    }

                    overlayBase.Dispose();
                    result.ResultImage?.Dispose();
                    property.ImageTemplate?.Dispose();
                }
            }
        }

        throw new InvalidOperationException("EasyMatch angle smoke could not produce a non-zero angle match from Die Pad samples.");
    }

    private static string FindEasyMatchDirectory()
    {
        string current = Directory.GetCurrentDirectory();
        for (int i = 0; i < 6 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidate = Path.Combine(current, "bin", "Debug", "EasyMatch");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            DirectoryInfo? parent = Directory.GetParent(current);
            current = parent?.FullName ?? string.Empty;
        }

        throw new DirectoryNotFoundException("Could not find bin\\Debug\\EasyMatch from " + Directory.GetCurrentDirectory());
    }

    private static void AssertMatchingAngleOverlayPixels(string outputPath, Lib.OpenCV.Result.MatchingResult match)
    {
        using Bitmap bitmap = new(outputPath);
        DrawingColor expected = DrawingColor.FromArgb(255, 230, 0);
        int yellowPixels = 0;
        int offAxisYellowPixels = 0;
        DrawingRectangleF axisBox = match.Bounding;

        for (int y = 0; y < bitmap.Height; y += 1)
        {
            for (int x = 0; x < bitmap.Width; x += 1)
            {
                DrawingColor color = bitmap.GetPixel(x, y);
                if (Math.Abs(color.R - expected.R) <= 20
                    && Math.Abs(color.G - expected.G) <= 25
                    && color.B <= 45)
                {
                    yellowPixels++;
                    bool nearAxisEdge =
                        Math.Abs(x - axisBox.Left) <= 2
                        || Math.Abs(x - axisBox.Right) <= 2
                        || Math.Abs(y - axisBox.Top) <= 2
                        || Math.Abs(y - axisBox.Bottom) <= 2;
                    if (!nearAxisEdge)
                    {
                        offAxisYellowPixels++;
                    }
                }
            }
        }

        if (yellowPixels < 40 || offAxisYellowPixels < 12)
        {
            throw new InvalidOperationException(
                "EasyMatch angle overlay did not contain enough rotated match-box pixels. "
                + $"Yellow={yellowPixels}, OffAxis={offAxisYellowPixels}, Angle={match.Angle:0.###}, Path={outputPath}");
        }
    }

    private static CaptureResult CaptureShellHostEdgeBasedMatchingTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostEdgeBasedMatching");
        using Bitmap matchingBitmap = CreateMatchingSmokeBitmap();
        string templatePath = CreateMatchingTemplateFile(matchingBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(matchingBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.EdgeBasedMatching);
                Pump(16);
                ComboBox inputLayerCombo = FindFloatingComboBox("cbInputLayer");
                AssertVisionToolComboTemplate(inputLayerCombo, "EdgeBasedMatching input layer combo");
                AssertComboBoxPopupLayout(inputLayerCombo, "EdgeBasedMatching input layer combo");
                AssertComboBoxSelectionCanChange(inputLayerCombo, "EdgeBasedMatching input layer combo");
                ComboBox outputLayerCombo = FindFloatingComboBox("cbOutputLayer");
                AssertVisionToolComboTemplate(outputLayerCombo, "EdgeBasedMatching output layer combo");
                AssertComboBoxPopupLayout(outputLayerCombo, "EdgeBasedMatching output layer combo");
                int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
                shellHost.SetActiveEdgeBasedMatchingTemplatePathForTest(templatePath);
                Thread.Sleep(180);
                Pump(30);
                AssertFloatingPropertyGridDialogButtonsReady("EdgeBasedMatching property grid dialog button");
                if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Native WPF EdgeBasedMatching tool did not auto-preview after template registration: " + shellHost.ActiveNativeStatusText);
                }

                if (shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns)
                {
                    throw new InvalidOperationException("Native WPF EdgeBasedMatching template registration did not request an auto-preview.");
                }

                if (!shellHost.ActiveNativeDocumentTypeName.Contains("EdgeBasedMatchingToolWpfView", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("EdgeBasedMatching did not open the WPF EdgeBasedMatching tool view.");
                }

                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "EdgeBasedMatching_Preview", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "EdgeBasedMatching template registration changed the route or active input layer. "
                        + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertNoAutoDockedLayers(shellHost, "EdgeBasedMatching template auto-preview");
                AssertResultReviewVisible("EdgeBasedMatching", "Edge Match /", "검출", "점수");
                AssertActiveToolTextsVisible("EdgeBasedMatching verification guide", "엣지", "Canny", "포인트", "미리보기 OK");

                VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
                if (step == null
                    || !string.Equals(step.ToolType, "EdgeBasedMatching", StringComparison.Ordinal)
                    || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                    || !string.Equals(step.OutputLayer, "EdgeBasedMatching_Preview", StringComparison.Ordinal)
                    || step.Parameters == null
                    || !step.Parameters.ContainsKey("PATTERN_PATH")
                    || !step.Parameters.ContainsKey("SCORE_MIN")
                    || !step.Parameters.ContainsKey("CANNY_LOW"))
                {
                    throw new InvalidOperationException("EdgeBasedMatching WPF tool did not create a valid pipeline step.");
                }

                AssertDockActiveNativeTool(shellHost, "EdgeBasedMatchingToolWpfView", "Docked EdgeBasedMatching tool layout");
                AssertFloatingPropertyGridMinimumSize("Docked EdgeBasedMatching property grid", 600D, 380D);
                AssertResultReviewVisible("Docked EdgeBasedMatching", "Edge Match /", "검출", "점수");
                AssertActiveToolTextsVisible("Docked EdgeBasedMatching verification guide", "엣지", "Canny", "포인트", "미리보기 OK");
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static CaptureResult CaptureShellHostFeatureMatchingTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostFeatureMatching");
        using Bitmap featureBitmap = CreateFeatureMatchingSmokeBitmap();
        string templatePath = CreateFeatureMatchingTemplateFile(featureBitmap);
        try
        {
            shellHost.SetMainLayerImageForTest(featureBitmap);
            return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
            {
                shellHost.SelectToolForTest(VISION_MENU.FeatureMatching);
                Pump(16);
                int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
                shellHost.SetActiveFeatureMatchingTemplatePathForTest(templatePath);
                Thread.Sleep(180);
                Pump(30);
                if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
                {
                    throw new InvalidOperationException("Native WPF FeatureMatching tool did not auto-preview after template registration: " + shellHost.ActiveNativeStatusText);
                }

                if (shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns)
                {
                    throw new InvalidOperationException("Native WPF FeatureMatching template registration did not request an auto-preview.");
                }

                if (!shellHost.ActiveNativeDocumentTypeName.Contains("FeatureMatchingToolWpfView", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("FeatureMatching did not open the WPF FeatureMatching tool view.");
                }

                if (!string.Equals(shellHost.ActiveNativeRouteInputLayerNameForTest, "Main", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveNativeRouteOutputLayerNameForTest, "FeatureMatching_Preview", StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(shellHost.ActiveHostLayerTitle, "Main", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "FeatureMatching template registration changed the route or active input layer. "
                        + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}, Active={shellHost.ActiveHostLayerTitle}, Status={shellHost.ActiveNativeStatusText}");
                }

                AssertNoAutoDockedLayers(shellHost, "FeatureMatching template auto-preview");
                AssertResultReviewVisible("FeatureMatching", "Feature Match /", "검출", "점수", "중심", "박스");
                AssertResultReviewVisible("FeatureMatching result guidance", "미리보기 OK", "합격 기준:", "Ratio", "RANSAC", "다음:");
                AssertActiveToolTextsVisible("FeatureMatching verification guide", "특징 매칭 검증", "미리보기 OK", "Ratio", "RANSAC");
                AssertActiveToolTextsVisible("FeatureMatching teaching summary", "템플릿 준비", "Ratio", "RANSAC", "원본", "전체 이미지");

                VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
                if (step == null
                    || !string.Equals(step.ToolType, "FeatureMatching", StringComparison.Ordinal)
                    || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                    || !string.Equals(step.OutputLayer, "FeatureMatching_Preview", StringComparison.Ordinal)
                    || step.Parameters == null
                    || !step.Parameters.ContainsKey("PATTERN_PATH")
                    || !step.Parameters.ContainsKey("SCORE_MIN")
                    || !step.Parameters.ContainsKey("RANSAC_REPROJ_THRESHOLD"))
                {
                    throw new InvalidOperationException("FeatureMatching WPF tool did not create a valid pipeline step.");
                }

                AssertDockActiveNativeTool(shellHost, "FeatureMatchingToolWpfView", "Docked FeatureMatching tool layout");
                AssertFloatingPropertyGridMinimumSize("Docked FeatureMatching property grid", 600D, 380D);
                AssertResultReviewVisible("Docked FeatureMatching", "Feature Match /", "검출", "점수");
                AssertResultReviewVisible("Docked FeatureMatching result guidance", "미리보기 OK", "합격 기준:", "Ratio", "RANSAC", "다음:");
                AssertActiveToolTextsVisible("Docked FeatureMatching verification guide", "특징 매칭 검증", "미리보기 OK", "Ratio", "RANSAC");
            });
        }
        finally
        {
            TryDeleteFile(templatePath);
        }
    }

    private static CaptureResult CaptureShellHostLineTool(string outputPath)
    {
        return CaptureShellHostLineMeasureTool(outputPath);
    }

    private static CaptureResult CaptureShellHostLinePinsMeasureTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLinePinsMeasure", seedMainLayer: false);
        string pinsPath = Path.Combine(Environment.CurrentDirectory, "Sample", "EasyGauge", "Pins.bmp");
        if (!File.Exists(pinsPath))
        {
            throw new FileNotFoundException("Pin measurement sample image was not found.", pinsPath);
        }

        using Bitmap pinsBitmap = new(pinsPath);
        shellHost.SetMainLayerImageForTest(pinsBitmap);

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Line);
            Pump(16);
            AssertFloatingPropertyGridRowsRendered("Pins Line property grid");
            AssertFloatingPropertyGridMinimumSize("Pins Line property grid", 600D, 380D);

            ConfigurePinsVerticalDistanceLine(shellHost, "Line A", "X_LTOR");
            ConfigurePinsVerticalDistanceLine(shellHost, "Line B", "X_RTOL");
            shellHost.SetActiveLineSettingForTest("Line A");
            shellHost.SetActiveLinePurposeForTest("Measure");
            Pump(12);

            if (shellHost.ActiveLineInputRoiOverlayCount < 2)
            {
                throw new InvalidOperationException("Pins Line input preview did not publish both Line A/B ROI overlays.");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);

            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Pins Line measure preview did not complete: " + shellHost.ActiveNativeStatusText);
            }

            AssertResultReviewVisible("Pins Line Measure", "측정 /", "거리", "검출");
            AssertResultReviewDoesNotContain("Pins Line Measure", "거리 없음", "검출 0");
            AssertResultReviewContainsNumberInRange("Pins Line Measure", "mm", 0.30, 0.45);

            using (Bitmap outputLayer = shellHost.GetLayerImageCloneForTest("Line_Preview"))
            {
                AssertBitmapPresent(outputLayer, "Line_Preview after pins measure");
                SaveDiagnosticBitmap(outputPath, "line-pins-measure-preview.png", outputLayer);
            }

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "LineDistance", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Line_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.TryGetValue("LeftCvROI", out string? leftRoi)
                || !string.Equals(leftRoi, "430,170,125,145", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("RightCvROI", out string? rightRoi)
                || !string.Equals(rightRoi, "430,170,125,145", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("LeftPRJ_DIR", out string? leftDirection)
                || !string.Equals(leftDirection, "X_LTOR", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("RightPRJ_DIR", out string? rightDirection)
                || !string.Equals(rightDirection, "X_RTOL", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("LeftMANUAL_ANGLE_VALUE", out string? manualAngle)
                || !manualAngle.Contains("89", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Pins Line measure did not create the expected LineDistance pipeline step.");
            }
        });
    }

    private static void ConfigurePinsVerticalDistanceLine(OpenVisionShellHostView shellHost, string setting, string projectionDirection)
    {
        shellHost.SetActiveLineSettingForTest(setting);
        Pump(4);
        shellHost.SetActiveSelectedLineRoiForTest(430, 170, 125, 145);
        Pump(4);
        shellHost.ConfigureActiveSelectedLineForTest(projectionDirection, "WTOB", "X_RTOL");
        Pump(4);

        SetFloatingPropertyGridPropertyValue(setting + " threshold off", "USE_THRESHOLD", false);
        SetFloatingPropertyGridPropertyValue(setting + " adaptive threshold off", "USE_ADAPTIVE_THRESHOLD", false);
        SetFloatingPropertyGridPropertyValue(setting + " contrast", "CONTRAST", 18D);
        SetFloatingPropertyGridPropertyValue(setting + " thickness", "THICKNESS", 2D);
        SetFloatingPropertyGridPropertyValue(setting + " sampling", "SAMPLING_STEP", 6D);
        SetFloatingPropertyGridPropertyValue(setting + " point range", "POINT_RANGE", 8);
        SetFloatingPropertyGridPropertyValue(setting + " manual angle on", "USE_MANUAL_ANGLE", true);
        Thread.Sleep(180);
        Pump(30);
        SetFloatingPropertyGridPropertyValue(setting + " manual angle", "MANUAL_ANGLE_VALUE", 89D);
        SetFloatingPropertyGridPropertyValue(setting + " show edge", "SHOW_EDGE", true);
        SetFloatingPropertyGridPropertyValue(setting + " show vertical line", "SHOW_VERTICAL_LINE", true);
        SetFloatingPropertyGridPropertyValue(setting + " show fit line", "SHOW_FITLINE", true);
        Pump(8);
    }

    private static CaptureResult CaptureToolOpenPerf(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfToolOpenPerf");
        List<string> lines = new()
        {
            "Pass|Tool|SelectMs|ReadyMs|Document|CacheCount|PrewarmCount|PrewarmCompleted|PrewarmElapsedMs|ManagedMemoryMB|WorkingSetMB"
        };

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            Pump(24);
            WaitForNativeToolPrewarm(shellHost, lines);

            // Measure menu switching only. Preview execution is intentionally excluded
            // so tool-window and property-grid startup regressions are visible.
            shellHost.SelectToolForTest(VISION_MENU.Pipeline);
            Pump(12);

            foreach (VISION_MENU menu in GetToolOpenPerfMenus())
            {
                MeasureToolOpen(shellHost, menu, "Cold", lines);
            }

            foreach (VISION_MENU menu in GetToolOpenPerfMenus())
            {
                MeasureToolOpen(shellHost, menu, "Warm", lines);
            }

            WriteToolOpenPerfReport(outputPath, lines);
        });
    }

    private static CaptureResult CaptureToolOpenFastClickPerf(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfToolOpenFastClickPerf");
        List<string> lines = new()
        {
            "Pass|Tool|SelectMs|ReadyMs|Document|CacheCount|PrewarmCount|PrewarmCompleted|PrewarmElapsedMs|ManagedMemoryMB|WorkingSetMB"
        };

        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            for (int i = 0; i < 5 && !shellHost.IsShellLoadedForTest; i++)
            {
                Pump(1);
            }

            lines.Add(string.Join(
                "|",
                "Initial",
                "NativeTools",
                "Completed=" + shellHost.IsNativeToolPrewarmCompletedForTest.ToString(CultureInfo.InvariantCulture),
                "CacheCount=" + shellHost.NativeToolDocumentCacheCountForTest.ToString(CultureInfo.InvariantCulture),
                "CreatedCount=" + shellHost.NativeToolPrewarmCreatedCountForTest.ToString(CultureInfo.InvariantCulture),
                "ElapsedMs=" + shellHost.NativeToolPrewarmElapsedMillisecondsForTest.ToString(CultureInfo.InvariantCulture),
                "ManagedMemoryMB=" + FormatMegabytes(GC.GetTotalMemory(false)),
                "WorkingSetMB=" + FormatMegabytes(GetWorkingSetBytes())));

            MeasureToolOpen(shellHost, VISION_MENU.Blob, "FastClick", lines);
            MeasureToolOpen(shellHost, VISION_MENU.Matching, "FastClick", lines);
            MeasureToolOpen(shellHost, VISION_MENU.EdgeBasedMatching, "FastClick", lines);
            MeasureToolOpen(shellHost, VISION_MENU.FeatureMatching, "FastClick", lines);

            WriteToolOpenPerfReport(outputPath, lines);
        }, initialPumpCount: 1);
    }

    private static CaptureResult CaptureToolOpenFirstHeavyPerf(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        List<string> lines = new()
        {
            "Pass|Tool|SelectMs|ReadyMs|Document|CacheCount|PrewarmCount|PrewarmCompleted|PrewarmElapsedMs|ManagedMemoryMB|WorkingSetMB"
        };

        CaptureResult result = default;
        string directory = Path.GetDirectoryName(outputPath) ?? ".";
        string baseName = Path.GetFileNameWithoutExtension(outputPath);
        string lastCapturePath = outputPath;
        foreach (VISION_MENU menu in GetFirstHeavyToolOpenPerfMenus())
        {
            OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfToolOpenFirst" + menu);
            string capturePath = Path.Combine(directory, baseName + "." + menu + ".png");
            lastCapturePath = capturePath;
            result = CaptureWindowWithContent(shellHost, capturePath, 1600, 900, () =>
            {
                for (int i = 0; i < 5 && !shellHost.IsShellLoadedForTest; i++)
                {
                    Pump(1);
                }

                lines.Add(string.Join(
                    "|",
                    "Initial",
                    menu.ToString(),
                    "Completed=" + shellHost.IsNativeToolPrewarmCompletedForTest.ToString(CultureInfo.InvariantCulture),
                    "CacheCount=" + shellHost.NativeToolDocumentCacheCountForTest.ToString(CultureInfo.InvariantCulture),
                    "CreatedCount=" + shellHost.NativeToolPrewarmCreatedCountForTest.ToString(CultureInfo.InvariantCulture),
                    "ElapsedMs=" + shellHost.NativeToolPrewarmElapsedMillisecondsForTest.ToString(CultureInfo.InvariantCulture),
                    "ManagedMemoryMB=" + FormatMegabytes(GC.GetTotalMemory(false)),
                    "WorkingSetMB=" + FormatMegabytes(GetWorkingSetBytes())));

                MeasureToolOpen(shellHost, menu, "FirstClick", lines);
            }, initialPumpCount: 1);
        }

        if (!string.Equals(lastCapturePath, outputPath, StringComparison.OrdinalIgnoreCase)
            && File.Exists(lastCapturePath))
        {
            File.Copy(lastCapturePath, outputPath, overwrite: true);
        }

        WriteToolOpenPerfReport(outputPath, lines);
        return result;
    }

    private static void WaitForNativeToolPrewarm(OpenVisionShellHostView shellHost, List<string> lines)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 180 && !shellHost.IsNativeToolPrewarmCompletedForTest; i++)
        {
            Pump(1);
        }

        lines.Add(string.Join(
            "|",
            "Prewarm",
            "NativeTools",
            "WaitMs=" + stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture),
            "Completed=" + shellHost.IsNativeToolPrewarmCompletedForTest.ToString(CultureInfo.InvariantCulture),
            "CacheCount=" + shellHost.NativeToolDocumentCacheCountForTest.ToString(CultureInfo.InvariantCulture),
            "CreatedCount=" + shellHost.NativeToolPrewarmCreatedCountForTest.ToString(CultureInfo.InvariantCulture),
            "ElapsedMs=" + shellHost.NativeToolPrewarmElapsedMillisecondsForTest.ToString(CultureInfo.InvariantCulture),
            "ManagedMemoryMB=" + FormatMegabytes(GC.GetTotalMemory(false)),
            "WorkingSetMB=" + FormatMegabytes(GetWorkingSetBytes())));
    }

    private static void MeasureToolOpen(OpenVisionShellHostView shellHost, VISION_MENU menu, string passName, List<string> lines)
    {
        bool cachedBefore = shellHost.HasNativeToolDocumentCachedForTest(menu);
        Stopwatch stopwatch = Stopwatch.StartNew();
        shellHost.SelectToolForTest(menu);
        long selectMs = stopwatch.ElapsedMilliseconds;
        Pump(12);
        long readyMs = stopwatch.ElapsedMilliseconds;
        bool cachedAfter = shellHost.HasNativeToolDocumentCachedForTest(menu);

        List<string> parts = new()
        {
            passName,
            menu.ToString(),
            "SelectMs=" + selectMs.ToString(CultureInfo.InvariantCulture),
            "ReadyMs=" + readyMs.ToString(CultureInfo.InvariantCulture),
            "Document=" + shellHost.ActiveNativeDocumentTypeName,
            "CachedBefore=" + cachedBefore.ToString(CultureInfo.InvariantCulture),
            "CachedAfter=" + cachedAfter.ToString(CultureInfo.InvariantCulture),
            "CacheCount=" + shellHost.NativeToolDocumentCacheCountForTest.ToString(CultureInfo.InvariantCulture),
            "PrewarmCount=" + shellHost.NativeToolPrewarmCreatedCountForTest.ToString(CultureInfo.InvariantCulture),
            "PrewarmCompleted=" + shellHost.IsNativeToolPrewarmCompletedForTest.ToString(CultureInfo.InvariantCulture),
            "PrewarmElapsedMs=" + shellHost.NativeToolPrewarmElapsedMillisecondsForTest.ToString(CultureInfo.InvariantCulture),
            "ManagedMemoryMB=" + FormatMegabytes(GC.GetTotalMemory(false)),
            "WorkingSetMB=" + FormatMegabytes(GetWorkingSetBytes())
        };

        string internalTimingText = shellHost.LastToolOpenTimingTextForTest;
        if (!string.IsNullOrWhiteSpace(internalTimingText))
        {
            parts.AddRange(internalTimingText.Split('|'));
        }

        lines.Add(string.Join("|", parts));
    }

    private static IEnumerable<VISION_MENU> GetToolOpenPerfMenus()
    {
        return new[]
        {
            VISION_MENU.Threshold,
            VISION_MENU.Filter,
            VISION_MENU.Morphology,
            VISION_MENU.EdgeDetection,
            VISION_MENU.RotateAndScale,
            VISION_MENU.Arithmetic,
            VISION_MENU.HSV,
            VISION_MENU.Mean,
            VISION_MENU.Histogram,
            VISION_MENU.Blob,
            VISION_MENU.Contour,
            VISION_MENU.Line,
            VISION_MENU.Matching,
            VISION_MENU.EdgeBasedMatching,
            VISION_MENU.FeatureMatching
        };
    }

    private static IEnumerable<VISION_MENU> GetFirstHeavyToolOpenPerfMenus()
    {
        return new[]
        {
            VISION_MENU.Blob,
            VISION_MENU.Matching,
            VISION_MENU.EdgeBasedMatching,
            VISION_MENU.FeatureMatching
        };
    }

    private static CaptureResult CaptureShellHostLinePresets(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLinePresets");
        using Bitmap lineBitmap = CreateLineMeasureSmokeBitmap();
        shellHost.SetMainLayerImageForTest(lineBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Line);
            Pump(16);
            AssertActiveToolTextsVisible(
                "Line preset controls",
                OpenVisionLanguageService.T("VisionTool.Preset.Title"),
                OpenVisionLanguageService.T("VisionTool.Preset.Basic"),
                OpenVisionLanguageService.T("VisionTool.Preset.Fast"),
                OpenVisionLanguageService.T("VisionTool.Preset.Precise"));
            AssertActiveToolTextsVisible("Line purpose controls", "Line A", "Line B");
            AssertFloatingPropertyGridRowsRendered("Line preset property grid");

            int beforePresetRuns = shellHost.NativePreviewRunCount;
            shellHost.SetActiveLineSettingForTest("Line A");
            Pump(8);
            ClickFloatingButtonByName("btnPresetFast", "Line A fast preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast threshold", "USE_THRESHOLD", false);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast adaptive", "USE_ADAPTIVE_THRESHOLD", false);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast manual angle", "USE_MANUAL_ANGLE", false);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast extend fit", "USE_EXTEND_FIT_LINE", false);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast average filter", "USE_AVERAGE_FILTER", false);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast contrast", "CONTRAST", 45D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast thickness", "THICKNESS", 4D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast sampling", "SAMPLING_STEP", 16D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast scan interval", "POINT_RANGE", 16D, 0.001D);
            AssertFloatingPropertyBrowsable("Line A fast manual angle hidden", "MANUAL_ANGLE_VALUE", false);
            AssertFloatingPropertyBrowsable("Line A fast extend length hidden", "EXTEND_FIT_LINE_VALUE", false);
            AssertFloatingPropertyBrowsable("Line A fast average diff hidden", "AVERAGE_Diff", false);
            if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Line A fast preset must update selected PropertyGrid values only, not run preview.");
            }

            shellHost.SetActiveLineSettingForTest("Line B");
            Pump(8);
            ClickFloatingButtonByName("btnPresetPrecise", "Line B precise preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectBooleanProperty("Line B precise threshold", "USE_THRESHOLD", false);
            AssertFloatingSelectedObjectBooleanProperty("Line B precise adaptive", "USE_ADAPTIVE_THRESHOLD", false);
            AssertFloatingSelectedObjectBooleanProperty("Line B precise manual angle", "USE_MANUAL_ANGLE", false);
            AssertFloatingSelectedObjectBooleanProperty("Line B precise extend fit", "USE_EXTEND_FIT_LINE", true);
            AssertFloatingSelectedObjectBooleanProperty("Line B precise average filter", "USE_AVERAGE_FILTER", true);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise contrast", "CONTRAST", 20D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise thickness", "THICKNESS", 3D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise sampling", "SAMPLING_STEP", 4D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise scan interval", "POINT_RANGE", 4D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise extend length", "EXTEND_FIT_LINE_VALUE", 150D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line B precise average diff", "AVERAGE_Diff", 80D, 0.001D);
            AssertFloatingPropertyBrowsable("Line B precise manual angle hidden", "MANUAL_ANGLE_VALUE", false);
            AssertFloatingPropertyBrowsable("Line B precise extend length visible", "EXTEND_FIT_LINE_VALUE", true);
            AssertFloatingPropertyBrowsable("Line B precise average diff visible", "AVERAGE_Diff", true);
            if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Line B precise preset must update selected PropertyGrid values only, not run preview.");
            }

            shellHost.SetActiveLineSettingForTest("Line A");
            Pump(8);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast preserved contrast", "CONTRAST", 45D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Line A fast preserved sampling", "SAMPLING_STEP", 16D, 0.001D);
            AssertFloatingSelectedObjectBooleanProperty("Line A fast preserved average filter", "USE_AVERAGE_FILTER", false);
            if (shellHost.NativePreviewRunCount != beforePresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Line A/B selection after presets must not run preview.");
            }

            AssertDockActiveNativeTool(shellHost, "LineToolWpfView", "Docked Line preset menu");
            AssertActiveToolButtonVisible("btnPresetMenu", "Docked Line preset menu button", true);
            int beforeDockedPresetRuns = shellHost.NativePreviewRunCount;
            ClickActiveToolPresetMenuItem("basic", "Docked Line basic preset");
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingSelectedObjectNumericPropertyWithin("Docked Line basic contrast", "CONTRAST", 30D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Docked Line basic sampling", "SAMPLING_STEP", 10D, 0.001D);
            AssertFloatingSelectedObjectNumericPropertyWithin("Docked Line basic scan interval", "POINT_RANGE", 10D, 0.001D);
            AssertFloatingSelectedObjectBooleanProperty("Docked Line basic extend fit", "USE_EXTEND_FIT_LINE", false);
            AssertFloatingSelectedObjectBooleanProperty("Docked Line basic average filter", "USE_AVERAGE_FILTER", false);
            if (shellHost.NativePreviewRunCount != beforeDockedPresetRuns || shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Docked Line preset menu must update PropertyGrid only, not run preview.");
            }
        });
    }

    private static CaptureResult CaptureShellHostLineMeasureTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLineMeasure");
        using Bitmap lineBitmap = CreateLineMeasureSmokeBitmap();
        shellHost.SetMainLayerImageForTest(lineBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Line);
            Pump(16);
            AssertActiveToolTextsVisible("Line verification guide initial", "Line 검증", "미리보기 전", "대비", "다음:");
            AssertActiveToolTextsVisible("Line purpose controls", "목적", "라인", "엣지", "측정", "교차");
            AssertFloatingPropertyGridRowsRendered("Line property grid");
            AssertFloatingPropertyGridMinimumSize("Line property grid", 600D, 380D);
            int beforeLineVisibilityToggleRuns = shellHost.NativePreviewRunCount;
            AssertFloatingPropertyBrowsable("Line point range before manual angle", "POINT_RANGE", true);
            AssertFloatingPropertyBrowsable("Line manual angle before enable", "MANUAL_ANGLE_VALUE", false);
            SetFloatingPropertyGridPropertyValue("Line enable manual angle", "USE_MANUAL_ANGLE", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line point range after manual angle", "POINT_RANGE", false);
            AssertFloatingPropertyBrowsable("Line manual angle after enable", "MANUAL_ANGLE_VALUE", true);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_MANUAL_ANGLE should only update angle rows, not run auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Line disable manual angle", "USE_MANUAL_ANGLE", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line point range after manual angle disable", "POINT_RANGE", true);
            AssertFloatingPropertyBrowsable("Line manual angle after disable", "MANUAL_ANGLE_VALUE", false);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_MANUAL_ANGLE disable should not run auto-preview.");
            }

            AssertFloatingPropertyBrowsable("Line extend length before enable", "EXTEND_FIT_LINE_VALUE", false);
            SetFloatingPropertyGridPropertyValue("Line enable extend fit line", "USE_EXTEND_FIT_LINE", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line extend length after enable", "EXTEND_FIT_LINE_VALUE", true);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_EXTEND_FIT_LINE should only update fit-line rows, not run auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Line disable extend fit line", "USE_EXTEND_FIT_LINE", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line extend length after disable", "EXTEND_FIT_LINE_VALUE", false);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_EXTEND_FIT_LINE disable should not run auto-preview.");
            }

            AssertFloatingPropertyBrowsable("Line average diff before enable", "AVERAGE_Diff", false);
            AssertFloatingPropertyBrowsable("Line average filter type before enable", "AVERAGE_FILTER_TYPE", false);
            SetFloatingPropertyGridPropertyValue("Line enable average filter", "USE_AVERAGE_FILTER", true);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line average diff after enable", "AVERAGE_Diff", true);
            AssertFloatingPropertyBrowsable("Line average filter type after enable", "AVERAGE_FILTER_TYPE", true);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_AVERAGE_FILTER should only update filter rows, not run auto-preview.");
            }

            SetFloatingPropertyGridPropertyValue("Line disable average filter", "USE_AVERAGE_FILTER", false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingPropertyBrowsable("Line average diff after disable", "AVERAGE_Diff", false);
            AssertFloatingPropertyBrowsable("Line average filter type after disable", "AVERAGE_FILTER_TYPE", false);
            if (shellHost.NativePreviewRunCount != beforeLineVisibilityToggleRuns)
            {
                throw new InvalidOperationException("Line USE_AVERAGE_FILTER disable should not run auto-preview.");
            }

            AssertFloatingInlinePreviewSlotCount("Line input", 1);
            shellHost.SetActiveLineRoiForTest(92, 64, 120, 272);
            Pump(8);
            shellHost.SetActiveLineSettingForTest("Line B");
            Pump(4);
            shellHost.SetActiveSelectedLineRoiForTest(300, 64, 122, 272);
            Pump(4);
            shellHost.SetActiveLineSettingForTest("Line A");
            Pump(4);
            if (shellHost.ActiveLineInputRoiOverlayCount < 2)
            {
                throw new InvalidOperationException("Line OpenGL input preview did not publish both Line A/B ROI overlays.");
            }

            int beforeAutoPreviewRuns = shellHost.NativePreviewRunCount;
            shellHost.ConfigureActiveSelectedLineThresholdForTest(120, false);
            Thread.Sleep(180);
            Pump(30);
            AssertFloatingInlinePreviewSlotCount("Line input/output", 2);
            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF Line tool did not auto-preview after PropertyGrid threshold changes: " + shellHost.ActiveNativeStatusText);
            }

            if (shellHost.NativePreviewRunCount <= beforeAutoPreviewRuns)
            {
                throw new InvalidOperationException("Native WPF Line PropertyGrid threshold change did not request an auto-preview.");
            }

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("LineToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Line did not open the WPF Line tool view.");
            }

            using (Bitmap mainLayer = shellHost.GetLayerImageCloneForTest("Main"))
            using (Bitmap lineThresholdLayer = shellHost.GetLayerImageCloneForTest("Line_Preview"))
            {
                AssertBitmapPresent(lineThresholdLayer, "Line_Preview layer after threshold auto-preview");
                AssertBitmapVisiblyDifferent(mainLayer, lineThresholdLayer, "Line auto-preview should show threshold teaching image instead of raw Main");
                AssertBitmapBinaryLike(lineThresholdLayer, "Line_Preview threshold teaching output");
                SaveDiagnosticBitmap(outputPath, "line-tool-main.png", mainLayer);
                SaveDiagnosticBitmap(outputPath, "line-tool-threshold-preview.png", lineThresholdLayer);
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(16);
            AssertActiveFloatingInlinePreviewZoomAnchors("Line inline preview zoom/pan");
            AssertPreviewClickActivatesWorkspaceLayer(
                shellHost,
                "VisionToolOutputPreviewSlot",
                "Line_Preview",
                "Line output preview click");
            AssertPreviewClickActivatesWorkspaceLayer(
                shellHost,
                "VisionToolInputPreviewSlot",
                "Main",
                "Line input preview click");
            AssertResultReviewVisible("Line Edge result guidance", "미리보기 OK", "합격 기준:", "엣지점", "피팅 길이", "다음:");
            AssertResultReviewVisible("Line Edge", "엣지 /", "라인", "엣지", "길이");
            AssertActiveToolTextsVisible("Line verification guide edge", "Line 검증", "미리보기 OK", "대비", "다음:");
            shellHost.SetActiveLinePurposeForTest("Measure");
            Pump(8);
            shellHost.RunActiveNativePreviewForTest();
            Pump(16);
            AssertResultReviewVisible("Line Measure", "측정 /", "거리", "검출");
            AssertResultReviewDoesNotContain("Line Measure", "거리 없음", "검출 0");
            AssertActiveToolTextsVisible("Line verification guide measure", "Line 검증", "미리보기 OK", "대비", "다음:");
            AssertResultReviewVisible("Line Measure result guidance", "미리보기 OK", "합격 기준:", "거리", "Line A/B", "다음:");
            shellHost.SetActiveLinePurposeForTest("Intersection");
            Pump(8);
            shellHost.RunActiveNativePreviewForTest();
            Pump(16);
            AssertResultReviewVisible("Line Intersection", "교차 /", "교차", "엣지");
            AssertResultReviewVisible("Line Intersection result guidance", "미리보기 NG", "합격 기준:", "교차점 없음", "원인 후보", "Line A/B", "다음:");
            shellHost.SetActiveLinePurposeForTest("Measure");
            Pump(8);
            shellHost.RunActiveNativePreviewForTest();
            Pump(16);
            AssertResultReviewVisible("Line Measure", "측정 /", "거리", "검출");
            AssertResultReviewDoesNotContain("Line Measure", "거리 없음", "검출 0");

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "LineDistance", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Line_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.ContainsKey("LeftPRJ_DIR")
                || !step.Parameters.ContainsKey("RightPRJ_DIR")
                || !step.Parameters.ContainsKey("LeftCONTRAST")
                || !step.Parameters.ContainsKey("RightCONTRAST")
                || !step.Parameters.TryGetValue("RightCvROI", out string? rightRoi)
                || !rightRoi.Contains("300", StringComparison.Ordinal)
                || !step.Parameters.TryGetValue("LinePurpose", out string? purpose)
                || !string.Equals(purpose, "Measure", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Line WPF tool did not create a valid LineDistance pipeline step.");
            }

            AssertDockActiveNativeTool(shellHost, "LineToolWpfView", "Docked Line tool layout");
            AssertFloatingPropertyGridMinimumSize("Docked Line property grid", 600D, 380D);
            AssertActiveToolTextsVisible("Docked Line verification guide", "Line 검증", "미리보기 OK", "대비", "다음:");
            AssertActiveToolTextsVisible("Docked Line purpose controls", "목적", "라인", "엣지", "측정", "교차");
            AssertActiveToolNamedElementsDoNotOverlap(
                "Docked Line purpose selector layout",
                "rdoPurposeEdge",
                "rdoPurposeMeasure",
                "rdoPurposeIntersection");
            AssertActiveToolNamedElementsVisibleWithinAncestors(
                "Docked Line purpose controls",
                "rdoPurposeEdge",
                "rdoPurposeMeasure",
                "rdoPurposeIntersection",
                "rdoLineA",
                "rdoLineB",
                "btnEditSelectedRoi");
        });
    }

    private static CaptureResult CaptureShellHostLineIntersectionTool(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        OpenVisionShellHostView shellHost = CreateShellHost("Smoke_WpfShellHostLineIntersection");
        using Bitmap lineBitmap = CreateLineIntersectionSmokeBitmap();
        shellHost.SetMainLayerImageForTest(lineBitmap);
        return CaptureWindowWithContent(shellHost, outputPath, 1600, 900, () =>
        {
            shellHost.SelectToolForTest(VISION_MENU.Line);
            Pump(16);
            AssertFloatingInlinePreviewSlotCount("Line intersection input", 1);

            shellHost.SetActiveLineSettingForTest("Line A");
            Pump(4);
            shellHost.SetActiveSelectedLineRoiForTest(42, 268, 190, 74);
            Pump(4);
            shellHost.ConfigureActiveSelectedLineForTest("Y_TTOB", "ALL", "Y_BTOT");
            shellHost.ConfigureActiveSelectedLineThresholdForTest(170, false);
            shellHost.ConfigureActiveSelectedLineDrawForTest(false, false, false, true);
            Pump(4);

            shellHost.SetActiveLineSettingForTest("Line B");
            Pump(4);
            shellHost.SetActiveSelectedLineRoiForTest(306, 44, 86, 142);
            Pump(4);
            shellHost.ConfigureActiveSelectedLineForTest("X_LTOR", "ALL", "X_LTOR");
            shellHost.ConfigureActiveSelectedLineThresholdForTest(170, false);
            shellHost.ConfigureActiveSelectedLineDrawForTest(false, false, false, true);
            Pump(4);

            shellHost.SetActiveLineSettingForTest("Line A");
            Pump(4);
            shellHost.SetActiveLinePurposeForTest("Intersection");
            Pump(8);
            if (shellHost.ActiveLineInputRoiOverlayCount < 2)
            {
                throw new InvalidOperationException("Line intersection OpenGL input preview did not publish both Line A/B ROI overlays.");
            }

            shellHost.RunActiveNativePreviewForTest();
            Pump(24);
            AssertFloatingInlinePreviewSlotCount("Line intersection input/output", 2);

            if (!shellHost.IsNativeDocumentActive || !shellHost.HasNativePreviewResult)
            {
                throw new InvalidOperationException("Native WPF Line intersection tool did not produce a preview result: " + shellHost.ActiveNativeStatusText);
            }

            if (!shellHost.ActiveNativeDocumentTypeName.Contains("LineToolWpfView", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Line intersection did not open the WPF Line tool view.");
            }

            AssertResultReviewVisible("Line Intersection", "교차 /", "교차 예", "점");
            AssertResultReviewDoesNotContain("Line Intersection", "교차 아니오");

            VisionPipelineStep step = shellHost.AddActiveNativePipelineStepForTest();
            if (step == null
                || !string.Equals(step.ToolType, "LineIntersection", StringComparison.Ordinal)
                || !string.Equals(step.InputLayer, "Main", StringComparison.Ordinal)
                || !string.Equals(step.OutputLayer, "Line_Preview", StringComparison.Ordinal)
                || step.Parameters == null
                || !step.Parameters.ContainsKey("LeftPRJ_DIR")
                || !step.Parameters.ContainsKey("RightPRJ_DIR")
                || !step.Parameters.TryGetValue("LinePurpose", out string? purpose)
                || !string.Equals(purpose, "Intersection", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Line WPF tool did not create a valid LineIntersection pipeline step.");
            }
        });
    }

    private static Bitmap CreateLineMeasureSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(232, 238, 241));

        using System.Drawing.SolidBrush railBrush = new(DrawingColor.Black);
        using System.Drawing.SolidBrush shadowBrush = new(DrawingColor.FromArgb(186, 199, 205));
        using System.Drawing.SolidBrush laneBrush = new(DrawingColor.FromArgb(248, 251, 252));
        using System.Drawing.Pen guidePen = new(DrawingColor.FromArgb(150, 169, 177), 1);
        using System.Drawing.Pen roiGuidePen = new(DrawingColor.FromArgb(96, 130, 142), 1);

        graphics.FillRectangle(laneBrush, 86, 54, 342, 292);
        graphics.FillRectangle(shadowBrush, 90, 60, 126, 280);
        graphics.FillRectangle(shadowBrush, 296, 60, 132, 280);
        graphics.FillRectangle(railBrush, 96, 70, 74, 252);
        graphics.FillRectangle(railBrush, 340, 70, 74, 252);

        for (int y = 80; y <= 314; y += 18)
        {
            graphics.DrawLine(guidePen, 170, y, 340, y);
        }

        graphics.DrawRectangle(roiGuidePen, 92, 64, 120, 272);
        graphics.DrawRectangle(roiGuidePen, 300, 64, 122, 272);
        return bitmap;
    }

    private static Bitmap CreateLineIntersectionSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        Random random = new(5317);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                int baseValue = 102 + random.Next(-24, 25);
                if (((x / 9) + (y / 7)) % 2 == 0)
                {
                    baseValue += 8;
                }

                int value = Math.Clamp(baseValue, 62, 146);
                bitmap.SetPixel(x, y, DrawingColor.FromArgb(value, value, value));
            }
        }

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using System.Drawing.Pen darkShadowPen = new(DrawingColor.FromArgb(72, 76, 76), 14)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        using System.Drawing.Pen softShadowPen = new(DrawingColor.FromArgb(130, 136, 136), 24)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        using System.Drawing.SolidBrush objectBrush = new(DrawingColor.FromArgb(252, 252, 250));
        using System.Drawing.Pen objectEdgePen = new(DrawingColor.FromArgb(236, 238, 236), 2);
        using System.Drawing.Pen roiGuidePen = new(DrawingColor.FromArgb(82, 116, 128), 1);

        System.Drawing.Point[] objectShape =
        {
            new(34, 34),
            new(344, 34),
            new(344, 188),
            new(226, 306),
            new(34, 306)
        };

        graphics.DrawLine(softShadowPen, 352, 44, 352, 190);
        graphics.DrawLine(softShadowPen, 232, 314, 352, 194);
        graphics.DrawLine(softShadowPen, 42, 314, 230, 314);
        graphics.DrawLine(darkShadowPen, 352, 44, 352, 190);
        graphics.DrawLine(darkShadowPen, 232, 314, 352, 194);
        graphics.DrawLine(darkShadowPen, 42, 314, 230, 314);

        graphics.FillPolygon(objectBrush, objectShape);
        graphics.DrawLines(objectEdgePen, new[]
        {
            new System.Drawing.Point(344, 34),
            new System.Drawing.Point(344, 188),
            new System.Drawing.Point(226, 306),
            new System.Drawing.Point(34, 306)
        });

        graphics.DrawRectangle(roiGuidePen, 42, 268, 190, 74);
        graphics.DrawRectangle(roiGuidePen, 306, 44, 86, 142);
        return bitmap;
    }

    private static Bitmap CreateMatchingSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(226, 234, 239));

        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(28, 38, 52));
        using System.Drawing.SolidBrush midBrush = new(DrawingColor.FromArgb(76, 126, 170));
        using System.Drawing.SolidBrush lightBrush = new(DrawingColor.FromArgb(234, 241, 247));
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(255, 255, 255), 3);
        using System.Drawing.Pen outlinePen = new(DrawingColor.FromArgb(20, 30, 44), 2);

        graphics.FillRectangle(darkBrush, 150, 100, 120, 96);
        graphics.FillEllipse(midBrush, 172, 122, 36, 36);
        graphics.DrawLine(accentPen, 158, 184, 262, 108);
        graphics.DrawRectangle(outlinePen, 150, 100, 120, 96);
        graphics.FillRectangle(lightBrush, 230, 152, 26, 28);

        using System.Drawing.Font font = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Matching", font, textBrush, 34, 16);
        return bitmap;
    }

    private static Bitmap CreateLargeSmokeBitmap(int width, int height)
    {
        Bitmap bitmap = new(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        ColorPalette palette = bitmap.Palette;
        for (int i = 0; i < palette.Entries.Length; i++)
        {
            palette.Entries[i] = DrawingColor.FromArgb(i, i, i);
        }

        bitmap.Palette = palette;

        DrawingRectangle bounds = new(0, 0, width, height);
        BitmapData data = bitmap.LockBits(bounds, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        try
        {
            int stride = data.Stride;
            byte[] row = new byte[stride];
            int periodX = Math.Max(1, width / 32);
            int periodY = Math.Max(1, height / 32);
            int fixtureLeft = width / 5;
            int fixtureRight = width * 4 / 5;
            int fixtureTop = height / 3;
            int fixtureBottom = height * 2 / 3;

            for (int y = 0; y < height; y++)
            {
                int yRamp = y * 96 / Math.Max(1, height - 1);
                bool gridY = y % periodY < 5;
                for (int x = 0; x < width; x++)
                {
                    int xRamp = x * 128 / Math.Max(1, width - 1);
                    int value = 56 + ((xRamp + yRamp) / 2);
                    bool gridX = x % periodX < 5;
                    bool inFixture = x >= fixtureLeft && x <= fixtureRight && y >= fixtureTop && y <= fixtureBottom;
                    bool stripe = ((x - fixtureLeft) / Math.Max(1, width / 96)) % 2 == 0;
                    if (inFixture)
                    {
                        value = stripe ? 210 : 126;
                    }

                    if (gridX || gridY)
                    {
                        value = Math.Min(245, value + 42);
                    }

                    row[x] = (byte)Math.Clamp(value, 0, 255);
                }

                for (int x = width; x < stride; x++)
                {
                    row[x] = 0;
                }

                Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, y * stride), stride);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return bitmap;
    }

    private static string CreateMatchingTemplateFile(Bitmap source)
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_matching_smoke_template_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap template = source.Clone(new DrawingRectangle(150, 100, 120, 96), source.PixelFormat);
        template.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    private static string CreateWorkspaceLoadSmokeImageFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_workspace_load_smoke_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap bitmap = new(640, 360);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(236, 242, 244));

        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(33, 48, 58));
        using System.Drawing.SolidBrush accentBrush = new(DrawingColor.FromArgb(21, 124, 134));
        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(166, 188, 196), 1);
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(21, 124, 134), 4);
        for (int x = 40; x < 600; x += 40)
        {
            graphics.DrawLine(gridPen, x, 40, x, 320);
        }

        for (int y = 40; y < 320; y += 40)
        {
            graphics.DrawLine(gridPen, 40, y, 600, y);
        }

        graphics.FillRectangle(darkBrush, 180, 128, 180, 86);
        graphics.FillEllipse(accentBrush, 92, 126, 74, 74);
        graphics.DrawEllipse(accentPen, 438, 116, 104, 104);

        using System.Drawing.Font font = new("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Loaded Main Image", font, textBrush, 44, 20);
        bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    private static Bitmap CreateWorkspaceSeedSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(226, 234, 239));

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(132, 160, 170), 1);
        for (int x = 32; x < bitmap.Width; x += 32)
        {
            graphics.DrawLine(gridPen, x, 32, x, bitmap.Height - 32);
        }

        for (int y = 32; y < bitmap.Height; y += 32)
        {
            graphics.DrawLine(gridPen, 32, y, bitmap.Width - 32, y);
        }

        using System.Drawing.SolidBrush shapeBrush = new(DrawingColor.FromArgb(49, 65, 72));
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(0, 167, 179), 4);
        graphics.FillEllipse(shapeBrush, 84, 138, 58, 58);
        graphics.FillRectangle(shapeBrush, 198, 132, 140, 72);
        graphics.DrawEllipse(accentPen, 382, 130, 70, 70);

        using System.Drawing.Font font = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("OpenVisionLab", font, textBrush, 34, 12);
        return bitmap;
    }

    private static Bitmap CreateDockingPanelSmokeBitmap(int index)
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        DrawingColor background = index switch
        {
            1 => DrawingColor.FromArgb(235, 242, 245),
            2 => DrawingColor.FromArgb(231, 239, 232),
            3 => DrawingColor.FromArgb(243, 236, 230),
            _ => DrawingColor.FromArgb(235, 236, 244)
        };
        graphics.Clear(background);

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(130, 152, 160), 1);
        for (int x = 28; x < bitmap.Width; x += 36)
        {
            graphics.DrawLine(gridPen, x, 26, x, bitmap.Height - 28);
        }

        for (int y = 28; y < bitmap.Height; y += 36)
        {
            graphics.DrawLine(gridPen, 28, y, bitmap.Width - 28, y);
        }

        DrawingColor accent = index switch
        {
            1 => DrawingColor.FromArgb(16, 133, 142),
            2 => DrawingColor.FromArgb(70, 130, 80),
            3 => DrawingColor.FromArgb(174, 103, 52),
            _ => DrawingColor.FromArgb(88, 96, 172)
        };

        using System.Drawing.SolidBrush accentBrush = new(accent);
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(34, 45, 55));
        using System.Drawing.Pen accentPen = new(accent, 5);
        graphics.FillRectangle(darkBrush, 76, 116, 130, 92);
        graphics.DrawEllipse(accentPen, 270, 92, 112, 112);
        graphics.FillEllipse(accentBrush, 330, 236, 52, 52);
        graphics.DrawLine(accentPen, 82, 282, 428, 116);

        using System.Drawing.Font titleFont = new("Segoe UI", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.Font captionFont = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        graphics.DrawString("Dock " + index.ToString(CultureInfo.InvariantCulture), titleFont, darkBrush, 34, 20);
        graphics.DrawString("Panel split smoke", captionFont, darkBrush, 34, 338);
        return bitmap;
    }

    private static Bitmap CreateFeatureMatchingSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(232, 238, 242));

        DrawingRectangle fixture = new(132, 82, 190, 142);
        using System.Drawing.SolidBrush panelBrush = new(DrawingColor.FromArgb(248, 250, 252));
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(24, 35, 49));
        using System.Drawing.SolidBrush blueBrush = new(DrawingColor.FromArgb(48, 116, 170));
        using System.Drawing.SolidBrush tealBrush = new(DrawingColor.FromArgb(20, 134, 142));
        using System.Drawing.Pen darkPen = new(DrawingColor.FromArgb(24, 35, 49), 2);
        using System.Drawing.Pen bluePen = new(DrawingColor.FromArgb(48, 116, 170), 2);
        using System.Drawing.Font titleFont = new("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.Font smallFont = new("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

        graphics.FillRectangle(panelBrush, fixture);
        graphics.DrawRectangle(darkPen, fixture);
        graphics.DrawString("F7", titleFont, darkBrush, fixture.X + 14, fixture.Y + 12);
        graphics.DrawString("SIFT", smallFont, blueBrush, fixture.X + 118, fixture.Y + 18);
        graphics.DrawLine(bluePen, fixture.X + 16, fixture.Y + 98, fixture.X + 170, fixture.Y + 30);
        graphics.DrawEllipse(darkPen, fixture.X + 24, fixture.Y + 74, 34, 34);
        graphics.FillEllipse(tealBrush, fixture.X + 68, fixture.Y + 70, 18, 18);
        graphics.FillRectangle(darkBrush, fixture.X + 128, fixture.Y + 78, 32, 26);

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(94, 108, 122), 1);
        for (int x = fixture.X + 8; x < fixture.Right - 8; x += 18)
        {
            graphics.DrawLine(gridPen, x, fixture.Y + 116, x + 8, fixture.Y + 132);
        }

        Random random = new(17);
        for (int i = 0; i < 38; i++)
        {
            int x = random.Next(fixture.X + 8, fixture.Right - 10);
            int y = random.Next(fixture.Y + 8, fixture.Bottom - 10);
            using System.Drawing.SolidBrush dotBrush = new(i % 3 == 0 ? DrawingColor.FromArgb(24, 35, 49) : DrawingColor.FromArgb(48, 116, 170));
            graphics.FillEllipse(dotBrush, x, y, 3 + (i % 3), 3 + (i % 3));
        }

        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Feature Matching", smallFont, textBrush, 34, 16);
        return bitmap;
    }

    private static string CreateFeatureMatchingTemplateFile(Bitmap source)
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_feature_matching_smoke_template_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap template = source.Clone(new DrawingRectangle(132, 82, 190, 142), source.PixelFormat);
        template.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
        }
    }

    private static CaptureResult WithDockingStateFileBackup(Func<CaptureResult> capture)
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
            return capture();
        }
        finally
        {
            foreach (string path in paths)
            {
                try
                {
                    if (backups.TryGetValue(path, out byte[]? bytes) && bytes != null)
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

    private static CaptureResult CaptureLogPanel(string outputPath)
    {
        LogPanelView view = new();
        return CaptureElement(view, outputPath, 900, 360);
    }

    private static CaptureResult CaptureRoiEditor(string outputPath)
    {
        using Bitmap bitmap = CreateLargeSmokeBitmap(640, 480);
        RoiEditorWindow window = new(bitmap, DrawingRectangle.Empty, "ROI")
        {
            Width = 1040,
            Height = 700,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = -32000,
            Top = -32000
        };

        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            if (window.SelectedRegion.Width != 0 || window.SelectedRegion.Height != 0)
            {
                throw new InvalidOperationException("ROI editor should open with no selected region when the current ROI is empty.");
            }
        });
    }

    private static CaptureResult CaptureMatchingPropertyGridCombo(string outputPath)
    {
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = new()
        {
            SelectedObject = new MatchingProperty()
        };
        grid.ApplyDisplayOptions(OpenVisionLab.PropertyGrid.PropertyGridDisplayOptions.ToolForm);

        return CaptureWindowWithContent(grid, outputPath, 760, 520, () =>
        {
            ComboBox? matchModeCombo = FindVisualChildren<ComboBox>(grid)
                .FirstOrDefault(combo => combo.Items.Cast<object>().Any(item =>
                    string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), "CCoeffNormed", StringComparison.Ordinal)));
            if (matchModeCombo == null)
            {
                throw new InvalidOperationException("Matching property grid combo was not found.");
            }

            matchModeCombo.BringIntoView();
            Pump(16);
            matchModeCombo.UpdateLayout();

            AssertComboBoxPopupLayout(matchModeCombo, "Matching property grid combo");
            AssertPropertyGridBridgeComboTemplate(matchModeCombo, "Matching property grid combo");
            AssertComboBoxSelectionTextIsSingle(matchModeCombo, "CCoeffNormed", "Matching property grid combo");
            AssertPropertyGridRangeEditorLayout(grid, "Matching property grid range editor");
        }, captureFloatingToolWindow: false);
    }

    private static CaptureResult CaptureOpenGlTemplateEditor(string outputPath)
    {
        using Bitmap bitmap = CreateRoiSmokeBitmap();
        OpenGlTemplateEditorWindow window = new(bitmap, new DrawingRectangle(116, 82, 210, 142), "TRAIN")
        {
            Width = 1040,
            Height = 700,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = -32000,
            Top = -32000
        };

        return CaptureStandaloneWindow(window, outputPath, 1040, 700, () =>
        {
            var selected = window.SelectedRegion;
            if (selected.Width <= 0 || selected.Height <= 0)
            {
                throw new InvalidOperationException("OpenGL template editor did not keep the initial selected region.");
            }

            if (Math.Abs(selected.X - 116) > 2
                || Math.Abs(selected.Y - 82) > 2
                || Math.Abs(selected.Width - 210) > 2
                || Math.Abs(selected.Height - 142) > 2)
            {
                throw new InvalidOperationException($"OpenGL template editor returned unexpected ROI {selected}.");
            }

            bool hasPatternPreview = FindVisualChildren<Image>(window)
                .Any(item => item.IsVisible && item.Source != null && item.ActualWidth > 0D && item.ActualHeight > 0D);
            if (!hasPatternPreview)
            {
                throw new InvalidOperationException("OpenGL template editor did not render a pattern preview image.");
            }

            bool hasMainSourceImage = FindVisualChildren<Image>(window)
                .Any(item => string.Equals(item.Name, "wpfSourceImage", StringComparison.Ordinal)
                    && item.IsVisible
                    && item.Source != null
                    && item.ActualWidth > 0D
                    && item.ActualHeight > 0D);
            if (!hasMainSourceImage)
            {
                throw new InvalidOperationException("Template editor did not render the main source image.");
            }

            if (window.RoiHandleVisualCountForTest < 8)
            {
                throw new InvalidOperationException("Template editor did not render ROI resize handles.");
            }

            if (!window.MoveSelectedRegionForTest(12, -7))
            {
                throw new InvalidOperationException("Template editor could not move the selected ROI.");
            }

            selected = window.SelectedRegion;
            if (Math.Abs(selected.X - 128) > 2 || Math.Abs(selected.Y - 75) > 2)
            {
                throw new InvalidOperationException($"Template editor ROI move returned unexpected ROI {selected}.");
            }

            if (!window.ResizeSelectedRegionForTest(18, 11))
            {
                throw new InvalidOperationException("Template editor could not resize the selected ROI.");
            }

            selected = window.SelectedRegion;
            if (Math.Abs(selected.Width - 228) > 2 || Math.Abs(selected.Height - 153) > 2)
            {
                throw new InvalidOperationException($"Template editor ROI resize returned unexpected ROI {selected}.");
            }

            if (!window.SetTemplateRotationDegreesForTest(17.5))
            {
                throw new InvalidOperationException("Template editor could not set ROI rotation.");
            }

            if (Math.Abs(window.TemplateRotationDegreesForTest - 17.5) > 0.001)
            {
                throw new InvalidOperationException($"Template editor returned unexpected ROI rotation {window.TemplateRotationDegreesForTest:0.###}.");
            }

            if (window.RoiHandleVisualCountForTest < 10)
            {
                throw new InvalidOperationException("Template editor did not render ROI rotation handle.");
            }

            using OpenCvSharp.Mat sourceMat = Lib.Common.BitmapImageConverter.ToMat(bitmap);
            using OpenCvSharp.Mat extractedTemplate = TemplateImageExtraction.Extract(
                sourceMat,
                window.SelectedRegion,
                window.TemplateRotationDegreesForTest);
            if (extractedTemplate.Empty()
                || extractedTemplate.Width != window.SelectedRegion.Width
                || extractedTemplate.Height != window.SelectedRegion.Height)
            {
                throw new InvalidOperationException("Template editor rotation did not produce a valid zero-degree extracted template.");
            }
        });
    }

    private static CaptureResult CaptureImageCompare(string outputPath)
    {
        string sampleDirectory = Path.Combine(Path.GetDirectoryName(outputPath) ?? ".", "image_compare_samples");
        Directory.CreateDirectory(sampleDirectory);

        string leftPath = Path.Combine(sampleDirectory, "compare_left.png");
        string rightPath = Path.Combine(sampleDirectory, "compare_right.png");
        using (Bitmap left = CreateRoiSmokeBitmap())
        using (Bitmap right = CreateRoiSmokeBitmap())
        {
            using (Graphics graphics = Graphics.FromImage(right))
            using (System.Drawing.SolidBrush brush = new(DrawingColor.FromArgb(180, 62, 116, 208)))
            {
                graphics.FillRectangle(brush, 248, 122, 86, 44);
            }

            left.Save(leftPath, System.Drawing.Imaging.ImageFormat.Png);
            right.Save(rightPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        ImageCompareWindow window = new()
        {
            Width = 1280,
            Height = 760,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = -32000,
            Top = -32000
        };
        window.LoadImages(leftPath, rightPath);

        return CaptureStandaloneWindow(window, outputPath, 1280, 760, () =>
        {
            if (window.DataContext is not ImageCompareViewModel viewModel ||
                viewModel.Slots.Count(slot => slot.IsLoaded) < 2)
            {
                throw new InvalidOperationException("Image Compare did not load the smoke images.");
            }

            ImageCompareSlotViewModel selectedSlot = viewModel.Slots.First(slot => slot.IsLoaded);
            viewModel.ApplyZoom(selectedSlot, 120);
            if (viewModel.Slots.Where(slot => slot.IsLoaded).Any(slot => slot.Zoom <= 1.0))
            {
                throw new InvalidOperationException("Image Compare synchronized zoom did not update loaded slots.");
            }

            viewModel.FitAll();
            viewModel.UpdatePixelStatus(selectedSlot, 260, 140);
            if (!viewModel.ColorText.Contains("RGB[", StringComparison.Ordinal) ||
                !viewModel.XyText.Contains("260,140", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Image Compare did not publish pixel status.");
            }
        });
    }

    private static CaptureResult CaptureLocalizationCatalog(string outputPath)
    {
        string[] required =
        {
            "Shell.PendingTool.Status",
            "Shell.PendingTool.NavStatus",
            "Common.Close",
            "VisionMenu.Blob"
        };

        foreach (string key in required)
        {
            string value = OpenVisionLanguageService.T(key);
            if (string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Missing localization key: " + key);
            }
        }

        Border report = new()
        {
            Background = Brushes.White,
            Padding = new Thickness(24),
            Child = new TextBlock
            {
                Text = "Localization Catalog OK" + Environment.NewLine + string.Join(Environment.NewLine, required),
                FontSize = 16,
                Foreground = Brushes.DarkSlateGray
            }
        };
        return CaptureElement(report, outputPath, 760, 420);
    }

    private static OpenVisionShellHostView CreateShellHost(string recipeName, bool seedMainLayer = true)
    {
        DisplayManagerService displayManager = new();
        GlobalState global = new();
        global.Recipe.Name = recipeName;
        PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
        PropertyGridEditorFactory.SetRecipeNameContext(() => global.Recipe.Name);
        ApplicationRuntimeContext runtimeContext = new(global, displayManager);
        OpenVisionShellHostView shellHost = new(runtimeContext);
        if (seedMainLayer)
        {
            using Bitmap seedImage = CreateWorkspaceSeedSmokeBitmap();
            shellHost.SetMainLayerImageForTest(seedImage);
        }

        return shellHost;
    }

    private static CaptureResult CaptureWindowWithContent(
        FrameworkElement content,
        string outputPath,
        int width,
        int height,
        Action verify,
        bool captureFloatingToolWindow = true,
        Action<string>? verifyCapture = null,
        int initialPumpCount = 20,
        bool captureScreen = false)
    {
        Window window = new()
        {
            Content = content,
            Width = width,
            Height = height,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Topmost = true
        };

        DateTime started = DateTime.UtcNow;
        window.Show();
        window.Activate();
        try
        {
            Pump(initialPumpCount);
            verify();
            Pump(12);
            Window captureWindow = captureFloatingToolWindow
                ? Application.Current.Windows
                    .OfType<Window>()
                    .LastOrDefault(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
                    ?? window
                : window;
            if (captureScreen)
            {
                WriteScreenPng(captureWindow, outputPath);
            }
            else
            {
                WriteElementPng(captureWindow, outputPath, (int)captureWindow.ActualWidth, (int)captureWindow.ActualHeight);
            }

            WriteOpenGlDiagnostics(outputPath, captureWindow, content);
            verifyCapture?.Invoke(outputPath);
            return new CaptureResult(
                Math.Max(1, (int)Math.Round(captureWindow.ActualWidth)),
                Math.Max(1, (int)Math.Round(captureWindow.ActualHeight)),
                (DateTime.UtcNow - started).TotalMilliseconds);
        }
        finally
        {
            foreach (Window owned in Application.Current.Windows.OfType<Window>().Where(item => !ReferenceEquals(item, window)).ToArray())
            {
                owned.Close();
            }

            window.Close();
        }
    }

    private static void AssertWorkspaceLoadImageVisibleInCapture(string outputPath)
    {
        using Bitmap capture = new(outputPath);
        DrawingRectangle probe = CreateWorkspaceImageProbeRegion(capture.Width, capture.Height);
        int sampled = 0;
        int lightPixels = 0;
        int grayImagePixels = 0;

        for (int y = probe.Top; y < probe.Bottom; y += 3)
        {
            for (int x = probe.Left; x < probe.Right; x += 3)
            {
                DrawingColor color = capture.GetPixel(x, y);
                sampled++;
                if (IsLoadedWorkspaceImageLightPixel(color))
                {
                    lightPixels++;
                }

                if (IsLoadedWorkspaceImageGrayPixel(color))
                {
                    grayImagePixels++;
                }
            }
        }

        double lightRatio = sampled <= 0 ? 0D : lightPixels / (double)sampled;
        double grayImageRatio = sampled <= 0 ? 0D : grayImagePixels / (double)sampled;
        bool hasBrightImage = lightPixels >= 1800 && lightRatio >= 0.08D;
        bool hasGrayImage = grayImagePixels >= 4200 && grayImageRatio >= 0.07D;
        if (!hasBrightImage && !hasGrayImage)
        {
            throw new InvalidOperationException(
                "Workspace image load captured a visually blank main workspace. "
                + $"LightPixels={lightPixels}, LightRatio={lightRatio:0.000}, "
                + $"GrayPixels={grayImagePixels}, GrayRatio={grayImageRatio:0.000}, "
                + $"Sampled={sampled}, Path={outputPath}");
        }
    }

    private static DrawingRectangle CreateWorkspaceImageProbeRegion(int width, int height)
    {
        int left = Math.Max(1, (int)Math.Round(width * 0.16D));
        int top = Math.Max(1, (int)Math.Round(height * 0.18D));
        int right = Math.Min(width - 1, (int)Math.Round(width * 0.79D));
        int bottom = Math.Min(height - 1, (int)Math.Round(height * 0.74D));
        return DrawingRectangle.FromLTRB(left, top, right, bottom);
    }

    private static bool IsLoadedWorkspaceImageLightPixel(DrawingColor color)
    {
        return color.R >= 210 && color.G >= 220 && color.B >= 220;
    }

    private static bool IsLoadedWorkspaceImageGrayPixel(DrawingColor color)
    {
        int maximum = Math.Max(color.R, Math.Max(color.G, color.B));
        int minimum = Math.Min(color.R, Math.Min(color.G, color.B));
        int luminance = (color.R + color.G + color.B) / 3;
        return luminance >= 45 && maximum - minimum <= 32;
    }

    private static void AssertBitmapPresent(Bitmap bitmap, string name)
    {
        if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0)
        {
            throw new InvalidOperationException(name + " was not available.");
        }
    }

    private static void SaveDiagnosticBitmap(string outputPath, string fileName, Bitmap bitmap)
    {
        AssertBitmapPresent(bitmap, fileName);
        string? parentDirectory = Path.GetDirectoryName(outputPath);
        string outputName = Path.GetFileNameWithoutExtension(outputPath);
        string diagnosticsDirectory = Path.Combine(
            string.IsNullOrWhiteSpace(parentDirectory) ? "." : parentDirectory,
            string.IsNullOrWhiteSpace(outputName) ? "diagnostics" : outputName + ".diagnostics");
        Directory.CreateDirectory(diagnosticsDirectory);
        bitmap.Save(Path.Combine(diagnosticsDirectory, fileName), System.Drawing.Imaging.ImageFormat.Png);
    }

    private static void AssertBitmapVisiblyDifferent(Bitmap expectedSource, Bitmap actualOutput, string name)
    {
        AssertBitmapPresent(expectedSource, name + " source");
        AssertBitmapPresent(actualOutput, name + " output");
        int width = Math.Min(expectedSource.Width, actualOutput.Width);
        int height = Math.Min(expectedSource.Height, actualOutput.Height);
        int sampled = 0;
        int changed = 0;
        long totalDelta = 0;

        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                DrawingColor left = expectedSource.GetPixel(x, y);
                DrawingColor right = actualOutput.GetPixel(x, y);
                int delta = Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B);
                sampled++;
                totalDelta += delta;
                if (delta >= 60)
                {
                    changed++;
                }
            }
        }

        double changedRatio = sampled <= 0 ? 0D : changed / (double)sampled;
        double averageDelta = sampled <= 0 ? 0D : totalDelta / (double)sampled;
        if (changedRatio < 0.04D || averageDelta < 18D)
        {
            throw new InvalidOperationException(
                name + " did not change enough to be teachable. "
                + $"ChangedRatio={changedRatio:0.000}, AverageDelta={averageDelta:0.0}, Sampled={sampled}");
        }
    }

    private static void AssertBitmapPreviewOverlayDifferentFromMain(Bitmap expectedSource, Bitmap actualOutput, string name)
    {
        AssertBitmapPresent(expectedSource, name + " source");
        AssertBitmapPresent(actualOutput, name + " output");
        int width = Math.Min(expectedSource.Width, actualOutput.Width);
        int height = Math.Min(expectedSource.Height, actualOutput.Height);
        int sampled = 0;
        int changed = 0;
        long totalDelta = 0;

        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                DrawingColor left = expectedSource.GetPixel(x, y);
                DrawingColor right = actualOutput.GetPixel(x, y);
                int delta = Math.Abs(left.R - right.R) + Math.Abs(left.G - right.G) + Math.Abs(left.B - right.B);
                sampled++;
                totalDelta += delta;
                if (delta >= 60)
                {
                    changed++;
                }
            }
        }

        double changedRatio = sampled <= 0 ? 0D : changed / (double)sampled;
        double averageDelta = sampled <= 0 ? 0D : totalDelta / (double)sampled;
        if (changedRatio < 0.008D || averageDelta < 6D)
        {
            throw new InvalidOperationException(
                name + " did not show enough preview overlay/change. "
                + $"ChangedRatio={changedRatio:0.000}, AverageDelta={averageDelta:0.0}, Sampled={sampled}");
        }
    }

    private static void AssertBitmapBinaryLike(Bitmap bitmap, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int dark = 0;
        int light = 0;
        int middle = 0;

        for (int y = 0; y < bitmap.Height; y += 4)
        {
            for (int x = 0; x < bitmap.Width; x += 4)
            {
                DrawingColor color = bitmap.GetPixel(x, y);
                int value = (color.R + color.G + color.B) / 3;
                sampled++;
                if (value <= 35)
                {
                    dark++;
                }
                else if (value >= 220)
                {
                    light++;
                }
                else
                {
                    middle++;
                }
            }
        }

        double binaryRatio = sampled <= 0 ? 0D : (dark + light) / (double)sampled;
        double darkRatio = sampled <= 0 ? 0D : dark / (double)sampled;
        double lightRatio = sampled <= 0 ? 0D : light / (double)sampled;
        if (binaryRatio < 0.82D || darkRatio < 0.03D || lightRatio < 0.03D)
        {
            throw new InvalidOperationException(
                name + " is not binary-like enough for Blob teaching. "
                + $"BinaryRatio={binaryRatio:0.000}, DarkRatio={darkRatio:0.000}, LightRatio={lightRatio:0.000}, Middle={middle}, Sampled={sampled}");
        }
    }

    private static void AssertBitmapMostlyGrayscale(Bitmap bitmap, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int colored = 0;
        for (int y = 0; y < bitmap.Height; y += 2)
        {
            for (int x = 0; x < bitmap.Width; x += 2)
            {
                DrawingColor color = bitmap.GetPixel(x, y);
                int max = Math.Max(color.R, Math.Max(color.G, color.B));
                int min = Math.Min(color.R, Math.Min(color.G, color.B));
                sampled++;
                if (max - min > 20)
                {
                    colored++;
                }
            }
        }

        double coloredRatio = sampled <= 0 ? 0D : colored / (double)sampled;
        if (coloredRatio > 0.001D)
        {
            throw new InvalidOperationException(
                name + " contains colored overlay pixels before Run. "
                + $"ColoredRatio={coloredRatio:0.0000}, Colored={colored}, Sampled={sampled}");
        }
    }

    private static void AssertBitmapContainsColorNear(Bitmap bitmap, DrawingColor expected, int tolerance, string name)
    {
        AssertBitmapPresent(bitmap, name);
        int sampled = 0;
        int matched = 0;
        int step = Math.Max(1, Math.Min(bitmap.Width, bitmap.Height) / 240);

        for (int y = 0; y < bitmap.Height; y += step)
        {
            for (int x = 0; x < bitmap.Width; x += step)
            {
                DrawingColor color = bitmap.GetPixel(x, y);
                sampled++;
                if (Math.Abs(color.R - expected.R) <= tolerance
                    && Math.Abs(color.G - expected.G) <= tolerance
                    && Math.Abs(color.B - expected.B) <= tolerance)
                {
                    matched++;
                }
            }
        }

        double matchedRatio = sampled <= 0 ? 0D : matched / (double)sampled;
        if (matched < 8 || matchedRatio < 0.00008D)
        {
            throw new InvalidOperationException(
                name + " did not contain enough expected draw-color pixels. "
                + $"Matched={matched}, Ratio={matchedRatio:0.000000}, Sampled={sampled}, "
                + $"Expected=R{expected.R} G{expected.G} B{expected.B}");
        }
    }

    private static CaptureResult CaptureStandaloneWindow(Window window, string outputPath, int width, int height, Action verify)
    {
        DateTime started = DateTime.UtcNow;
        window.Width = width;
        window.Height = height;
        window.Show();
        try
        {
            Pump(20);
            verify();
            Pump(12);
            WriteElementPng(window, outputPath, Math.Max(1, (int)Math.Round(window.ActualWidth)), Math.Max(1, (int)Math.Round(window.ActualHeight)));
            WriteOpenGlDiagnostics(outputPath, window, window.Content as DependencyObject);
            return new CaptureResult(
                Math.Max(1, (int)Math.Round(window.ActualWidth)),
                Math.Max(1, (int)Math.Round(window.ActualHeight)),
                (DateTime.UtcNow - started).TotalMilliseconds);
        }
        finally
        {
            window.Close();
        }
    }

    private static CaptureResult CaptureElement(FrameworkElement element, string outputPath, int width, int height)
    {
        DateTime started = DateTime.UtcNow;
        WriteElementPng(element, outputPath, width, height);
        return new CaptureResult(width, height, (DateTime.UtcNow - started).TotalMilliseconds);
    }

    private static void WriteScreenPng(Window window, string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        window.UpdateLayout();
        Point topLeft = window.PointToScreen(new Point(0D, 0D));
        int width = Math.Max(1, (int)Math.Round(window.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(window.ActualHeight));
        using Bitmap bitmap = new(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(
            Math.Max(0, (int)Math.Round(topLeft.X)),
            Math.Max(0, (int)Math.Round(topLeft.Y)),
            0,
            0,
            new DrawingSize(width, height));
        bitmap.Save(outputPath, ImageFormat.Png);
    }

    private static void WriteElementPng(FrameworkElement element, string outputPath, int width, int height)
    {
        width = Math.Max(1, width);
        height = Math.Max(1, height);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        element.Measure(new Size(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        element.UpdateLayout();

        RenderTargetBitmap bitmap = new(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(outputPath);
        encoder.Save(stream);
    }

    private static void WriteOpenGlDiagnostics(string outputPath, params DependencyObject?[] roots)
    {
        List<DependencyObject> validRoots = roots
            .Where(root => root != null)
            .Cast<DependencyObject>()
            .Distinct()
            .ToList();
        if (validRoots.Count == 0)
        {
            return;
        }

        List<OpenVisionShellHostView> shellHosts = validRoots
            .SelectMany(FindVisualChildrenIncludingSelf<OpenVisionShellHostView>)
            .Distinct()
            .ToList();
        List<OpenVisionLayerViewerView> layerViewers = validRoots
            .SelectMany(FindVisualChildrenIncludingSelf<OpenVisionLayerViewerView>)
            .Distinct()
            .ToList();
        List<VisionToolInlinePreviewSlot> previewSlots = validRoots
            .SelectMany(FindVisualChildrenIncludingSelf<VisionToolInlinePreviewSlot>)
            .Distinct()
            .ToList();

        if (shellHosts.Count == 0 && layerViewers.Count == 0 && previewSlots.Count == 0)
        {
            return;
        }

        List<string> lines = new()
        {
            "CaptureNote=RenderTargetBitmap can miss hosted OpenGL pixels; use these runtime diagnostics when PNG viewer regions look dark.",
            "ShellHostCount=" + shellHosts.Count.ToString(CultureInfo.InvariantCulture),
            "LayerViewerCount=" + layerViewers.Count.ToString(CultureInfo.InvariantCulture),
            "ToolPreviewSlotCount=" + previewSlots.Count.ToString(CultureInfo.InvariantCulture)
        };

        for (int i = 0; i < shellHosts.Count; i++)
        {
            OpenVisionShellHostView shell = shellHosts[i];
            lines.Add(
                $"ShellHost[{i}]=WorkspaceLayer={shell.WorkspaceLayerTitle}|WorkspaceTiles={shell.WorkspaceTextureTileCount}|DockedLayers={shell.DockedLayerCount}|DockedPanes={shell.DockedLayerPaneCount}|DockedTiles={shell.DockedLayerTextureTileCount}|DockedHeaders={shell.DockedLayerTabHeaderCount}|DockHeadersReady={shell.AreDockedLayerTabHeadersGestureReadyForTest}|Titles={shell.DockedLayerTitles}");
        }

        for (int i = 0; i < layerViewers.Count; i++)
        {
            OpenVisionLayerViewerView viewer = layerViewers[i];
            lines.Add(
                $"LayerViewer[{i}]=Title={viewer.LayerTitle}|HasImage={viewer.HasImage}|Size={viewer.ImagePixelWidth}x{viewer.ImagePixelHeight}|Tiles={viewer.TextureTileCount}|Compact={viewer.IsCompactChrome}");
        }

        for (int i = 0; i < previewSlots.Count; i++)
        {
            VisionToolInlinePreviewSlot slot = previewSlots[i];
            lines.Add(
                $"ToolPreviewSlot[{i}]=HasImage={slot.HasImage}|Size={slot.ImagePixelWidth}x{slot.ImagePixelHeight}|Tiles={slot.TextureTileCount}|RoiOverlays={slot.RoiOverlayCount}");
        }

        string diagnosticsPath = Path.ChangeExtension(outputPath, ".opengl.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(diagnosticsPath) ?? ".");
        File.WriteAllLines(diagnosticsPath, lines);
    }

    private static void WriteLargeImagePerfReport(string outputPath, List<string> lines, long elapsedMs, long memoryStart)
    {
        try
        {
            string path = Path.ChangeExtension(outputPath, ".perf.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            List<string> report = new(lines ?? Enumerable.Empty<string>())
            {
                "TotalElapsedMs=" + elapsedMs.ToString(CultureInfo.InvariantCulture),
                "WorkingSetStartMB=" + FormatMegabytes(memoryStart),
                "WorkingSetEndMB=" + FormatMegabytes(GetWorkingSetBytes()),
                "ManagedMemoryMB=" + FormatMegabytes(GC.GetTotalMemory(false))
            };
            File.WriteAllLines(path, report);
        }
        catch
        {
        }
    }

    private static void WriteToolOpenPerfReport(string outputPath, IEnumerable<string> lines)
    {
        try
        {
            string path = Path.ChangeExtension(outputPath, ".perf.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            File.WriteAllLines(path, lines ?? Enumerable.Empty<string>());
        }
        catch
        {
        }
    }

    private static long GetWorkingSetBytes()
    {
        try
        {
            using Process process = Process.GetCurrentProcess();
            return process.WorkingSet64;
        }
        catch
        {
            return Environment.WorkingSet;
        }
    }

    private static string FormatMegabytes(long bytes)
    {
        return (bytes / 1024D / 1024D).ToString("0.0", CultureInfo.InvariantCulture);
    }

    private static Bitmap CreateRoiSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(28, 31, 34));

        using System.Drawing.Pen railPen = new(DrawingColor.FromArgb(190, 210, 214), 4);
        using System.Drawing.Pen tracePen = new(DrawingColor.FromArgb(130, 177, 188), 2);
        using System.Drawing.SolidBrush padBrush = new(DrawingColor.FromArgb(225, 230, 230));
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(50, 62, 68));
        using System.Drawing.SolidBrush brightBrush = new(DrawingColor.FromArgb(245, 248, 248));

        for (int i = 0; i < 7; i++)
        {
            int x = 72 + i * 52;
            graphics.FillRectangle(padBrush, x, 58, 24, 44);
            graphics.FillRectangle(darkBrush, x + 5, 68, 14, 24);
        }

        graphics.DrawLine(railPen, 42, 180, 470, 260);
        graphics.DrawLine(railPen, 32, 224, 456, 312);
        graphics.DrawLine(tracePen, 72, 206, 418, 274);
        graphics.DrawLine(tracePen, 92, 244, 426, 304);
        graphics.FillEllipse(brightBrush, 318, 190, 24, 24);
        graphics.FillEllipse(brightBrush, 372, 214, 18, 18);
        return bitmap;
    }

    private static void EnsureApplication()
    {
        if (Application.Current == null)
        {
            new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }
    }

    private static void Pump(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(
                    new Action(() => { }),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1400)
            {
                // WPF PopupControlService can process a stale popup HWND while smoke tests close or dock tool windows.
                // Behavioral assertions still fail through the verification action; this only stabilizes dispatcher cleanup.
            }
        }
    }

    private static void AssertResultReviewVisible(string toolName, params string[] requiredTokens)
    {
        string[] tokens = requiredTokens?.Length > 0
            ? requiredTokens
            : new[] { "Center ", "Box " };
        string? reviewText = FindResultReviewText(tokens);

        if (string.IsNullOrWhiteSpace(reviewText))
        {
            throw new InvalidOperationException(
                toolName
                + " result review was not updated after preview. Text='"
                + reviewText
                + "', expected tokens="
                + string.Join(", ", tokens)
                + ", active text='"
                + string.Join(" | ", FindResultReviewTexts())
                + "'");
        }
    }

    private static void AssertResultReviewDoesNotContain(string toolName, params string[] blockedTokens)
    {
        List<string> reviewTexts = FindResultReviewTexts().ToList();
        if (reviewTexts.Count == 0)
        {
            throw new InvalidOperationException(toolName + " result review was not found.");
        }

        string? blocked = blockedTokens.FirstOrDefault(token => reviewTexts.Any(text => text.Contains(token, StringComparison.OrdinalIgnoreCase)));
        if (!string.IsNullOrWhiteSpace(blocked))
        {
            throw new InvalidOperationException(toolName + " result review contains blocked text '" + blocked + "'. Text='" + string.Join(" | ", reviewTexts) + "'");
        }
    }

    private static void AssertResultReviewContainsNumberInRange(string toolName, string unit, double minimum, double maximum)
    {
        List<string> reviewTexts = FindResultReviewTexts().ToList();
        Regex pattern = new Regex(
            @"([-+]?\d+(?:\.\d+)?)\s*" + Regex.Escape(unit ?? string.Empty),
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        foreach (string text in reviewTexts)
        {
            foreach (Match match in pattern.Matches(text))
            {
                if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    continue;
                }

                if (value >= minimum && value <= maximum)
                {
                    return;
                }
            }
        }

        throw new InvalidOperationException(
            toolName
            + " result review did not contain "
            + unit
            + " value in range "
            + minimum.ToString(CultureInfo.InvariantCulture)
            + ".."
            + maximum.ToString(CultureInfo.InvariantCulture)
            + ". Text='"
            + string.Join(" | ", reviewTexts)
            + "'");
    }

    private static void AssertResultReviewRawVisible(string toolName, params string[] requiredTokens)
    {
        string[] tokens = requiredTokens ?? Array.Empty<string>();
        string? reviewText = FindRawResultReviewTexts()
            .FirstOrDefault(text => tokens.All(token => text.Contains(token, StringComparison.Ordinal)));
        if (string.IsNullOrWhiteSpace(reviewText))
        {
            throw new InvalidOperationException(toolName + " raw result review was not visible. Expected tokens=" + string.Join(", ", tokens));
        }
    }

    private static void AssertResultReviewRawDoesNotContain(string toolName, params string[] blockedTokens)
    {
        List<string> reviewTexts = FindRawResultReviewTexts().ToList();
        string? blocked = blockedTokens.FirstOrDefault(token => reviewTexts.Any(text => text.Contains(token, StringComparison.OrdinalIgnoreCase)));
        if (!string.IsNullOrWhiteSpace(blocked))
        {
            throw new InvalidOperationException(toolName + " raw result review contains blocked text '" + blocked + "'. Text='" + string.Join(" | ", reviewTexts) + "'");
        }
    }

    private static string? FindResultReviewText(params string[] requiredTokens)
    {
        string[] tokens = requiredTokens ?? Array.Empty<string>();
        return FindResultReviewTexts()
            .FirstOrDefault(text => tokens.All(token => text.Contains(token, StringComparison.Ordinal)));
    }

    private static IEnumerable<string> FindResultReviewTexts()
    {
        return FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<TextBlock>)
            .Select(item => item.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text)
                && !text.Contains("not run", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> FindRawResultReviewTexts()
    {
        return FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<TextBlock>)
            .Select(item => item.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text));
    }

    private static void AssertActiveToolTextsVisible(string name, params string[] requiredTokens)
    {
        string text = string.Join(
            " | ",
            FindActiveToolVisualRoots()
                .SelectMany(FindVisualChildren<TextBlock>)
                .Select(item => item.Text)
                .Where(item => !string.IsNullOrWhiteSpace(item)));
        string? missing = requiredTokens.FirstOrDefault(token => !text.Contains(token, StringComparison.Ordinal));
        if (!string.IsNullOrWhiteSpace(missing))
        {
            throw new InvalidOperationException(
                name + " did not show expected text '" + missing + "'. Text='" + text + "'");
        }
    }

    private static void AssertRecipeContext(OpenVisionShellHostView shellHost, string expectedRecipeName, string expectedPipelineName)
    {
        if (!string.Equals(shellHost.ActiveRecipeContextNameForTest, expectedRecipeName, StringComparison.Ordinal)
            || !string.Equals(shellHost.ActiveRecipeContextPipelineNameForTest, expectedPipelineName, StringComparison.Ordinal)
            || !shellHost.ActiveRecipeContextDisplayTextForTest.Contains(expectedRecipeName, StringComparison.Ordinal)
            || !shellHost.ActiveRecipeContextDisplayTextForTest.Contains(expectedPipelineName, StringComparison.Ordinal)
            || !shellHost.ActiveRecipeContextSourcePathForTest.EndsWith(expectedPipelineName + ".xml", StringComparison.OrdinalIgnoreCase)
            || !File.Exists(shellHost.ActiveRecipeContextSourcePathForTest))
        {
            throw new InvalidOperationException(
                "Recipe context did not resolve the expected recipe/pipeline. "
                + $"Expected={expectedRecipeName}/{expectedPipelineName}, "
                + $"Actual={shellHost.ActiveRecipeContextNameForTest}/{shellHost.ActiveRecipeContextPipelineNameForTest}, "
                + $"Display='{shellHost.ActiveRecipeContextDisplayTextForTest}', "
                + $"Source='{shellHost.ActiveRecipeContextSourcePathForTest}'");
        }
    }

    private static void AssertStepParameter(VisionPipelineStep step, string key, object expectedValue, string name)
    {
        string expectedText = Convert.ToString(expectedValue, CultureInfo.InvariantCulture) ?? string.Empty;
        if (step?.Parameters == null
            || !step.Parameters.TryGetValue(key, out string? actualText)
            || !string.Equals(actualText, expectedText, StringComparison.Ordinal))
        {
            string actual = step?.Parameters == null
                ? "<missing parameters>"
                : step.Parameters.TryGetValue(key, out string? value) ? value ?? "<null>" : "<missing>";
            throw new InvalidOperationException(
                name + " did not keep the expected recipe-scoped parameter. "
                + $"Key={key}, Expected={expectedText}, Actual={actual}, "
                + $"Step={(step == null ? "<null>" : step.Name)}");
        }
    }

    private static void AssertVisibleAutomationIds(DependencyObject root, string name, params string[] requiredIds)
    {
        HashSet<string> visibleIds = FindVisualChildren<FrameworkElement>(root)
            .Where(item => item.IsVisible)
            .Select(System.Windows.Automation.AutomationProperties.GetAutomationId)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.Ordinal);

        string? missing = requiredIds.FirstOrDefault(id => !visibleIds.Contains(id));
        if (!string.IsNullOrWhiteSpace(missing))
        {
            throw new InvalidOperationException(
                name + " did not show expected AutomationId '" + missing + "'. "
                + "VisibleIds='" + string.Join(", ", visibleIds.OrderBy(item => item, StringComparer.Ordinal)) + "'");
        }
    }

    private static void AssertTopLayerIconButtonLayout(DependencyObject root)
    {
        string[] ids =
        {
            "HostTopCreateLayerButton",
            "HostTopLoadImageIntoLayerButton",
            "HostTopDeleteLayerButton"
        };
        double previousLeft = double.NegativeInfinity;
        foreach (string id in ids)
        {
            Button button = FindVisualChildren<Button>(root)
                .Where(item => item.IsVisible)
                .FirstOrDefault(item => string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    id,
                    StringComparison.Ordinal))
                ?? throw new InvalidOperationException("Top layer icon button was not visible: " + id);
            Point origin = button.TranslatePoint(new Point(0D, 0D), (UIElement)root);
            if (button.ActualWidth < 27D
                || button.ActualWidth > 29D
                || button.ActualHeight < 25D
                || button.ActualHeight > 27D
                || origin.X <= previousLeft)
            {
                throw new InvalidOperationException(
                    "Top layer icon button layout is unstable. "
                    + $"Id={id}, Size={button.ActualWidth:0.0}x{button.ActualHeight:0.0}, X={origin.X:0.0}, PreviousX={previousLeft:0.0}");
            }

            previousLeft = origin.X;
        }
    }

    private static void SaveVisibleAutomationElementPng(
        DependencyObject root,
        string automationId,
        string outputPath,
        string fileName)
    {
        FrameworkElement? element = FindVisualChildren<FrameworkElement>(root)
            .Where(item => item.IsVisible)
            .FirstOrDefault(item => string.Equals(
                System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                automationId,
                StringComparison.Ordinal));
        if (element == null || element.ActualWidth <= 1D || element.ActualHeight <= 1D)
        {
            throw new InvalidOperationException(
                "Visible AutomationId '" + automationId + "' was not available for PNG capture.");
        }

        string? parentDirectory = Path.GetDirectoryName(outputPath);
        string outputName = Path.GetFileNameWithoutExtension(outputPath);
        string diagnosticsDirectory = Path.Combine(
            string.IsNullOrWhiteSpace(parentDirectory) ? "." : parentDirectory,
            string.IsNullOrWhiteSpace(outputName) ? "diagnostics" : outputName + ".diagnostics");
        Directory.CreateDirectory(diagnosticsDirectory);
        string capturePath = Path.Combine(diagnosticsDirectory, fileName);
        WriteVisibleElementPng(element, capturePath);
    }

    private static void WriteVisibleElementPng(FrameworkElement element, string outputPath)
    {
        int width = Math.Max(1, (int)Math.Round(element.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(element.ActualHeight));
        RenderTargetBitmap bitmap = new(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(outputPath);
        encoder.Save(stream);
    }

    private static void AssertHiddenAutomationIds(DependencyObject root, string name, params string[] hiddenIds)
    {
        HashSet<string> visibleIds = FindVisualChildren<FrameworkElement>(root)
            .Where(item => item.IsVisible)
            .Select(System.Windows.Automation.AutomationProperties.GetAutomationId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);
        foreach (string visible in hiddenIds.Where(id => visibleIds.Contains(id)))
        {
            throw new InvalidOperationException(
                name + " still shows hidden AutomationId '" + visible + "'. "
                + "VisibleIds='" + string.Join(", ", visibleIds.OrderBy(item => item, StringComparer.Ordinal)) + "'");
        }
    }

    private static bool ContainsAny(string text, params string[] expectedFragments)
    {
        return expectedFragments.Any(fragment =>
            !string.IsNullOrWhiteSpace(fragment)
            && (text ?? string.Empty).IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static void AssertVisibleTextContains(DependencyObject root, string name, params string[] requiredTexts)
    {
        string text = string.Join(
            " | ",
            FindVisualChildren<TextBlock>(root)
                .Where(item => item.IsVisible)
                .Select(item => item.Text)
                .Where(item => !string.IsNullOrWhiteSpace(item)));
        string? missing = (requiredTexts ?? Array.Empty<string>())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .FirstOrDefault(item => !text.Contains(item, StringComparison.Ordinal));
        if (!string.IsNullOrWhiteSpace(missing))
        {
            throw new InvalidOperationException(name + " did not show expected text '" + missing + "'. Text='" + text + "'");
        }
    }

    private static void AssertVisibleTextDoesNotContain(DependencyObject root, string name, params string[] blockedTexts)
    {
        string text = string.Join(
            " | ",
            FindVisualChildren<TextBlock>(root)
                .Where(item => item.IsVisible)
                .Select(item => item.Text)
                .Where(item => !string.IsNullOrWhiteSpace(item)));
        string? blocked = (blockedTexts ?? Array.Empty<string>())
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .FirstOrDefault(item => text.Contains(item, StringComparison.Ordinal));
        if (!string.IsNullOrWhiteSpace(blocked))
        {
            throw new InvalidOperationException(name + " showed blocked text '" + blocked + "'. Text='" + text + "'");
        }
    }

    private static bool HasFloatingToolPreviewImageSource()
    {
        IEnumerable<DependencyObject> toolRoots = FindActiveToolVisualRoots();

        return toolRoots
            .SelectMany(FindVisualChildren<Image>)
            .Any(item => item.Source != null)
            || toolRoots
                .SelectMany(FindVisualChildren<VisionToolInlinePreviewSlot>)
                .Any(item => item.HasImage && item.TextureTileCount > 0);
    }

    private static void AssertFloatingInlinePreviewSlotCount(string name, int expectedMinimum)
    {
        int count = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<VisionToolInlinePreviewSlot>)
            .Count(item => item.HasImage && item.TextureTileCount > 0);
        if (count < expectedMinimum)
        {
            throw new InvalidOperationException(
                name + " inline preview slot count was too small. "
                + "Expected>=" + expectedMinimum.ToString(CultureInfo.InvariantCulture)
                + ", Actual=" + count.ToString(CultureInfo.InvariantCulture));
        }
    }

    private static void AssertNoAutoDockedLayers(OpenVisionShellHostView shellHost, string scenario)
    {
        if (shellHost.DockedLayerPaneCount > 1)
        {
            throw new InvalidOperationException(
                scenario + " must not auto-create comparison panes. Live layers may mirror as same-pane AvalonDock tabs only. "
                + $"DockedCount={shellHost.DockedLayerCount}, DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, "
                + $"SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, Panes={shellHost.DockedLayerPaneCount}, Titles={shellHost.DockedLayerTitles}");
        }

        if (shellHost.DockedLayerCount > 0 && !shellHost.IsDockedWorkspaceVisibleForTest)
        {
            throw new InvalidOperationException(
                scenario + " mirrored live layers exist but the docked workspace is not visible. "
                + $"DockedCount={shellHost.DockedLayerCount}, DockedVisible={shellHost.IsDockedWorkspaceVisibleForTest}, "
                + $"SingleVisible={shellHost.IsSingleWorkspaceVisibleForTest}, Panes={shellHost.DockedLayerPaneCount}, Titles={shellHost.DockedLayerTitles}");
        }
    }

    private static void AssertDirectResultOkBanner(OpenVisionShellHostView shellHost, string scenario)
    {
        string expectedOutputLayer = shellHost.ActiveNativeRouteOutputLayerNameForTest;
        if (!string.Equals(shellHost.DirectResultBadgeText, OpenVisionLanguageService.T("Shell.DirectBadgeOk"), StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(expectedOutputLayer)
            || !shellHost.DirectResultRouteText.Contains(expectedOutputLayer, StringComparison.OrdinalIgnoreCase)
            || string.Equals(shellHost.DirectResultStatusText, OpenVisionLanguageService.T("Shell.DirectStatusReadyDetail"), StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                scenario + " did not keep the Shell direct-result banner synchronized with the active preview result. "
                + $"Badge='{shellHost.DirectResultBadgeText}', Title='{shellHost.DirectResultTitleText}', "
                + $"Route='{shellHost.DirectResultRouteText}', Status='{shellHost.DirectResultStatusText}', "
                + $"ExpectedOutput='{expectedOutputLayer}'");
        }
    }

    private static void AssertActiveFloatingInlinePreviewSlotCount(string name, int expectedMinimum)
    {
        DependencyObject toolRoot = GetActiveToolVisualRoot(name);
        int count = FindVisualChildren<VisionToolInlinePreviewSlot>(toolRoot)
            .Count(item => item.HasImage && item.TextureTileCount > 0);
        if (count < expectedMinimum)
        {
            throw new InvalidOperationException(
                name + " active inline preview slot count was too small. "
                + "Expected>=" + expectedMinimum.ToString(CultureInfo.InvariantCulture)
                + ", Actual=" + count.ToString(CultureInfo.InvariantCulture));
        }
    }

    private static void AssertActiveFloatingInlinePreviewZoomAnchors(string name)
    {
        DependencyObject toolRoot = GetActiveToolVisualRoot(name);
        List<VisionToolInlinePreviewSlot> slots = FindVisualChildren<VisionToolInlinePreviewSlot>(toolRoot)
            .Where(item => item.HasImage && item.TextureTileCount > 0)
            .ToList();
        if (slots.Count == 0)
        {
            throw new InvalidOperationException(name + " did not find an image preview slot.");
        }

        foreach (VisionToolInlinePreviewSlot slot in slots)
        {
            slot.ApplyTemplate();
            slot.UpdateLayout();
            if (!slot.TryGetContentPointForTest(0.62D, 0.54D, out Point before))
            {
                throw new InvalidOperationException(name + " could not read the pre-zoom anchor point.");
            }

            slot.ZoomAtForTest(0.62D, 0.54D, 1.25D);
            Pump(4);

            if (!slot.TryGetContentPointForTest(0.62D, 0.54D, out Point after))
            {
                throw new InvalidOperationException(name + " could not read the post-zoom anchor point.");
            }

            double dx = Math.Abs(before.X - after.X);
            double dy = Math.Abs(before.Y - after.Y);
            if (dx > 0.5D || dy > 0.5D)
            {
                throw new InvalidOperationException(
                    name + " drifted away from the mouse anchor. "
                    + $"Before={before.X:0.00},{before.Y:0.00}, After={after.X:0.00},{after.Y:0.00}");
            }

            if (!slot.TryGetContentPointForTest(0.50D, 0.50D, out Point beforePan))
            {
                throw new InvalidOperationException(name + " could not read the pre-pan center point.");
            }

            slot.PanByForTest(24D, -18D);
            Pump(4);

            if (!slot.TryGetContentPointForTest(0.50D, 0.50D, out Point afterPan))
            {
                throw new InvalidOperationException(name + " could not read the post-pan center point.");
            }

            if (Math.Abs(beforePan.X - afterPan.X) < 0.5D && Math.Abs(beforePan.Y - afterPan.Y) < 0.5D)
            {
                throw new InvalidOperationException(
                    name + " did not pan the inline preview image. "
                    + $"Before={beforePan.X:0.00},{beforePan.Y:0.00}, After={afterPan.X:0.00},{afterPan.Y:0.00}");
            }

            slot.FitImageToView();
        }

        Pump(4);
    }

    private static void AssertPreviewClickActivatesWorkspaceLayer(
        OpenVisionShellHostView shellHost,
        string automationId,
        string expectedLayer,
        string name)
    {
        ClickActiveFloatingPreviewFrameByAutomationId(automationId, name);
        if (!string.Equals(shellHost.ActiveHostLayerTitle, expectedLayer, StringComparison.OrdinalIgnoreCase)
            || !shellHost.HasWorkspaceLayerPreview
            || !string.Equals(shellHost.WorkspaceLayerTitle, expectedLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                name
                + " did not activate the expected layer in the main workspace. "
                + $"Expected={expectedLayer}, Active={shellHost.ActiveHostLayerTitle}, Workspace={shellHost.WorkspaceLayerTitle}, "
                + $"Input={shellHost.ActiveNativeRouteInputLayerNameForTest}, Output={shellHost.ActiveNativeRouteOutputLayerNameForTest}");
        }
    }

    private static IEnumerable<ComboBox> FindFloatingComboBoxes()
    {
        return FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<ComboBox>);
    }

    private static ComboBox FindFloatingComboBox(string name)
    {
        ComboBox? comboBox = FindFloatingComboBoxes()
            .FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.Ordinal));
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " combo box was not found.");
        }

        return comboBox;
    }

    private static void ClickFloatingButtonByName(string buttonName, string name)
    {
        Button? button = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Button>)
            .FirstOrDefault(item => string.Equals(item.Name, buttonName, StringComparison.Ordinal));
        if (button == null)
        {
            throw new InvalidOperationException(name + " was not found: " + buttonName);
        }

        button.ApplyTemplate();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, button));
        Pump(8);
    }

    private static Button GetActiveToolButtonByName(string buttonName, string name)
    {
        Button? button = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Button>)
            .FirstOrDefault(item => string.Equals(item.Name, buttonName, StringComparison.Ordinal));
        if (button == null)
        {
            throw new InvalidOperationException(name + " was not found: " + buttonName);
        }

        button.ApplyTemplate();
        button.UpdateLayout();
        return button;
    }

    private static void AssertActiveToolButtonVisible(string buttonName, string name, bool expectedVisible)
    {
        Button button = GetActiveToolButtonByName(buttonName, name);
        bool actualVisible = button.IsVisible && button.Visibility == Visibility.Visible;
        if (actualVisible != expectedVisible)
        {
            throw new InvalidOperationException(
                name + " visibility mismatch for " + buttonName
                + $". Expected={expectedVisible}, Actual={actualVisible}, Visibility={button.Visibility}");
        }
    }

    private static void ClickActiveToolPresetMenuItem(string presetId, string name)
    {
        Button menuButton = GetActiveToolButtonByName("btnPresetMenu", name + " menu button");
        if (!menuButton.IsVisible || menuButton.Visibility != Visibility.Visible)
        {
            throw new InvalidOperationException(name + " preset menu button is not visible.");
        }

        menuButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, menuButton));
        Pump(8);

        ContextMenu? menu = menuButton.ContextMenu;
        if (menu == null || !menu.IsOpen)
        {
            throw new InvalidOperationException(name + " preset menu did not open.");
        }

        string automationId = "VisionToolPresetMenuItem_" + presetId;
        MenuItem? item = menu.Items
            .OfType<MenuItem>()
            .FirstOrDefault(candidate => string.Equals(
                AutomationProperties.GetAutomationId(candidate),
                automationId,
                StringComparison.Ordinal));
        if (item == null)
        {
            throw new InvalidOperationException(
                name + " preset menu item was not found: " + automationId
                + ". Items=" + string.Join(", ", menu.Items.OfType<MenuItem>().Select(candidate => AutomationProperties.GetAutomationId(candidate))));
        }

        item.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent, item));
        menu.IsOpen = false;
        Pump(8);
    }

    private static void ClickVisibleButtonByName(string buttonName, string name)
    {
        Button? button = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible)
            .SelectMany(FindVisualChildren<Button>)
            .FirstOrDefault(item => string.Equals(item.Name, buttonName, StringComparison.Ordinal));
        if (button == null)
        {
            throw new InvalidOperationException(name + " was not found: " + buttonName);
        }

        if (!button.IsEnabled)
        {
            throw new InvalidOperationException(name + " is disabled: " + buttonName);
        }

        button.ApplyTemplate();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, button));
        Pump(8);
    }

    private static List<DependencyObject> FindActiveToolVisualRoots()
    {
        List<DependencyObject> roots = new();
        List<Window> visibleWindows = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible)
            .ToList();

        roots.AddRange(visibleWindows.Where(IsFloatingToolWindow));
        foreach (Window window in visibleWindows.Where(item => !IsFloatingToolWindow(item)))
        {
            roots.AddRange(FindVisualChildren<ContentControl>(window)
                .Where(item => item.IsVisible
                    && string.Equals(item.Name, "dockedToolContentHost", StringComparison.Ordinal)
                    && item.Content != null));
        }

        return roots;
    }

    private static DependencyObject GetActiveToolVisualRoot(string scope)
    {
        DependencyObject? root = FindActiveToolVisualRoots().LastOrDefault();
        if (root == null)
        {
            throw new InvalidOperationException(scope + " did not open an active tool host.");
        }

        if (root is FrameworkElement frameworkElement)
        {
            frameworkElement.ApplyTemplate();
            frameworkElement.UpdateLayout();
        }

        return root;
    }

    private static bool IsFloatingToolWindow(Window window)
    {
        return window != null && window.GetType().Name == "OpenVisionFloatingToolWindow";
    }

    private static int CountVisibleFloatingToolWindows()
    {
        return Application.Current.Windows
            .OfType<Window>()
            .Count(item => item.IsVisible && IsFloatingToolWindow(item));
    }

    private static void AssertActiveToolWindowState(
        OpenVisionShellHostView shellHost,
        string expectedViewTypeName,
        bool docked,
        int floatingWindowCount,
        string scenario)
    {
        int actualFloatingWindowCount = CountVisibleFloatingToolWindows();
        bool actualDocked = shellHost.IsDockedToolInspectorVisibleForTest;
        string actualTypeName = docked
            ? shellHost.ActiveWpfToolWindowTypeName
            : shellHost.ActiveNativeDocumentTypeName;
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

        if (docked)
        {
            AssertDockedToolHeader(shellHost, scenario);
        }
    }

    private static void AssertDockedToolHeader(OpenVisionShellHostView shellHost, string scenario)
    {
        if (string.IsNullOrWhiteSpace(shellHost.DockedToolTitleForTest)
            || !shellHost.IsDockedToolFloatButtonVisibleForTest
            || !shellHost.IsDockedToolCloseButtonVisibleForTest
            || shellHost.DockedToolFloatButtonWidthForTest < 32D
            || shellHost.DockedToolCloseButtonWidthForTest < 32D
            || string.IsNullOrWhiteSpace(shellHost.DockedToolFloatButtonToolTipForTest)
            || string.IsNullOrWhiteSpace(shellHost.DockedToolCloseButtonToolTipForTest)
            || string.Equals(shellHost.DockedToolFloatButtonToolTipForTest, "Shell.FloatDockedTool", StringComparison.Ordinal)
            || string.Equals(shellHost.DockedToolCloseButtonToolTipForTest, "Shell.CloseDockedTool", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                scenario + " did not expose a usable docked tool header. "
                + $"Title='{shellHost.DockedToolTitleForTest}', "
                + $"FloatVisible={shellHost.IsDockedToolFloatButtonVisibleForTest}, "
                + $"CloseVisible={shellHost.IsDockedToolCloseButtonVisibleForTest}, "
                + $"FloatWidth={shellHost.DockedToolFloatButtonWidthForTest:0.0}, "
                + $"CloseWidth={shellHost.DockedToolCloseButtonWidthForTest:0.0}, "
                + $"FloatToolTip='{shellHost.DockedToolFloatButtonToolTipForTest}', "
                + $"CloseToolTip='{shellHost.DockedToolCloseButtonToolTipForTest}'");
        }
    }

    private static void AssertDockedToolInspectorWidth(OpenVisionShellHostView shellHost, double expectedWidth, string scenario)
    {
        double actualWidth = shellHost.DockedToolInspectorWidthForTest;
        if (Math.Abs(actualWidth - expectedWidth) > 2D)
        {
            throw new InvalidOperationException(
                scenario + " did not preserve the operator-adjusted dock width. "
                + $"Expected={expectedWidth:0.0}, Actual={actualWidth:0.0}");
        }
    }

    private static void AssertDockActiveNativeTool(OpenVisionShellHostView shellHost, string expectedViewTypeName, string name)
    {
        if (!shellHost.DockActiveWpfToolWindowForTest())
        {
            throw new InvalidOperationException(name + " did not accept the dock-to-right request.");
        }

        Pump(24);
        if (!shellHost.IsDockedToolInspectorVisibleForTest
            || !shellHost.ActiveWpfToolWindowTypeName.Contains(expectedViewTypeName, StringComparison.Ordinal)
            || CountVisibleFloatingToolWindows() != 0)
        {
            throw new InvalidOperationException(
                name + " did not preserve the native tool in the docked inspector. "
                + $"Docked={shellHost.IsDockedToolInspectorVisibleForTest}, ActiveTool={shellHost.ActiveWpfToolWindowTypeName}, "
                + $"FloatingWindows={CountVisibleFloatingToolWindows()}");
        }

        AssertDockedSingleInputToolLayout(name);
    }

    private static void AssertDockedSingleInputToolLayout(string name)
    {
        DependencyObject root = GetActiveToolVisualRoot(name);
        VisionToolSingleInputPropertyToolShell? shell = FindVisualChildren<VisionToolSingleInputPropertyToolShell>(root)
            .FirstOrDefault(item => item.IsVisible);
        if (shell == null)
        {
            throw new InvalidOperationException(name + " did not contain a single-input tool shell.");
        }

        shell.ApplyTemplate();
        shell.UpdateLayout();
        if (!shell.IsDockedInspectorMode)
        {
            throw new InvalidOperationException(name + " did not enter docked inspector layout mode.");
        }

        System.Windows.Point inputOrigin = shell.InputLayerGroup.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        System.Windows.Point outputOrigin = shell.OutputLayerGroup.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        if (Math.Abs(inputOrigin.Y - outputOrigin.Y) > 10D || outputOrigin.X <= inputOrigin.X)
        {
            throw new InvalidOperationException(
                name + " should place input/output previews side by side in the docked inspector. "
                + $"Input={inputOrigin.X:0.0},{inputOrigin.Y:0.0}, Output={outputOrigin.X:0.0},{outputOrigin.Y:0.0}");
        }

        AssertDockedCompactPreviewCard(name + " input preview", shell.InputLayerGroup, shell.InputPreviewFrame);
        AssertDockedCompactPreviewCard(name + " output preview", shell.OutputLayerGroup, shell.OutputPreviewFrame);

        if (shell.PropertyGridHost.IsVisible && shell.PropertyGridHost.ActualWidth < 480D)
        {
            throw new InvalidOperationException(
                name + " PropertyGrid host is too narrow after docking. "
                + $"ActualWidth={shell.PropertyGridHost.ActualWidth:0.0}");
        }

        AssertDockedPropertyGridUsableViewport(name, shell);

        AssertElementWithinAncestor(
            name + " Add Pipeline button",
            shell.AddPipelineButton,
            shell);
        AssertElementWithinAncestor(
            name + " Run Preview button",
            shell.RunPreviewButton,
            shell);

        AssertDockedActionRowDensity(
            name + " Run Preview button",
            shell.RunPreviewButton,
            shell);
    }

    private static void AssertDockedCompactPreviewCard(string name, FrameworkElement group, FrameworkElement previewFrame)
    {
        if (group.ActualHeight > 160D || previewFrame.ActualHeight < 60D || previewFrame.ActualHeight > 84D)
        {
            throw new InvalidOperationException(
                name + " is not using the compact docked preview density. "
                + $"GroupHeight={group.ActualHeight:0.0}, PreviewHeight={previewFrame.ActualHeight:0.0}");
        }

        TextBlock? emptyDetail = FindVisualChildren<TextBlock>(previewFrame)
            .FirstOrDefault(item => string.Equals(item.Name, "txtPreviewEmptyDetail", StringComparison.Ordinal));
        Button? emptyLoadButton = FindVisualChildren<Button>(previewFrame)
            .FirstOrDefault(item => string.Equals(item.Name, "btnPreviewLoadImage", StringComparison.Ordinal));
        if ((emptyDetail?.IsVisible == true || emptyLoadButton?.IsVisible == true) && previewFrame.ActualHeight < 90D)
        {
            throw new InvalidOperationException(
                name + " compact empty-state overlay should not show long detail text or load button. "
                + $"PreviewHeight={previewFrame.ActualHeight:0.0}, DetailVisible={emptyDetail?.IsVisible}, "
                + $"LoadButtonVisible={emptyLoadButton?.IsVisible}");
        }

        AssertDockedPreviewRouteHint(name, previewFrame);
    }

    private static void AssertDockedPreviewRouteHint(string name, FrameworkElement previewFrame)
    {
        previewFrame.ApplyTemplate();
        previewFrame.UpdateLayout();
        VisionToolInlinePreviewSlot? previewSlot = FindVisualChildren<VisionToolInlinePreviewSlot>(previewFrame)
            .FirstOrDefault();
        Border? routeHint = FindVisualChildren<Border>(previewFrame)
            .FirstOrDefault(item =>
                string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "VisionToolInputRoutePreviewHint",
                    StringComparison.Ordinal)
                || string.Equals(
                    System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                    "VisionToolOutputRoutePreviewHint",
                    StringComparison.Ordinal));

        if (previewSlot?.HasImage == true)
        {
            if (routeHint == null || routeHint.Visibility != Visibility.Visible || previewFrame.Cursor != Cursors.Hand)
            {
                throw new InvalidOperationException(
                    name + " image thumbnail did not expose the central-workspace route hint. "
                    + $"HintFound={routeHint != null}, HintVisibility={routeHint?.Visibility}, Cursor={previewFrame.Cursor}");
            }

            return;
        }

        if (routeHint?.Visibility == Visibility.Visible)
        {
            throw new InvalidOperationException(name + " empty thumbnail should not show a route hint.");
        }
    }

    private static void AssertDockedPropertyGridUsableViewport(string name, VisionToolSingleInputPropertyToolShell shell)
    {
        if (!shell.PropertyGridHost.IsVisible)
        {
            return;
        }

        shell.PropertyGridHost.ApplyTemplate();
        shell.PropertyGridHost.UpdateLayout();
        shell.SummaryHost.ApplyTemplate();
        shell.SummaryHost.UpdateLayout();
        shell.StatusHost.ApplyTemplate();
        shell.StatusHost.UpdateLayout();
        shell.RunPreviewButton.ApplyTemplate();
        shell.RunPreviewButton.UpdateLayout();

        System.Windows.Controls.WpfPropertyGrid.PropertyGrid? propertyGrid = FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(shell.PropertyGridHost)
            .FirstOrDefault(item => item.IsVisible);
        if (propertyGrid == null || !propertyGrid.IsCompactDensity)
        {
            throw new InvalidOperationException(
                name + " docked PropertyGrid did not enable compact density. "
                + $"Found={propertyGrid != null}, Compact={propertyGrid?.IsCompactDensity}");
        }

        AssertDockedResultReviewDensity(name, shell);
        AssertDockedSummaryStatusStrip(name, shell.SummaryHost, shell.SummaryText, shell.StatusHost, shell.StatusText);

        System.Windows.Point propertyGridOrigin = shell.PropertyGridHost.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        System.Windows.Point summaryOrigin = shell.SummaryHost.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        System.Windows.Point runOrigin = shell.RunPreviewButton.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        double bottomLimit = Math.Min(summaryOrigin.Y, runOrigin.Y) - 4D;
        double visibleHeight = bottomLimit - propertyGridOrigin.Y;
        double propertyGridBottom = propertyGridOrigin.Y + shell.PropertyGridHost.ActualHeight;

        if (visibleHeight < 160D)
        {
            throw new InvalidOperationException(
                name + " leaves too little visible PropertyGrid editor space after docking. "
                + $"VisibleHeight={visibleHeight:0.0}, PropertyGridY={propertyGridOrigin.Y:0.0}, "
                + $"SummaryY={summaryOrigin.Y:0.0}, RunY={runOrigin.Y:0.0}, ShellHeight={shell.ActualHeight:0.0}");
        }

        if (propertyGridBottom > bottomLimit + 1D)
        {
            throw new InvalidOperationException(
                name + " PropertyGrid host overlaps the result/summary area after docking. "
                + $"PropertyGridBottom={propertyGridBottom:0.0}, BottomLimit={bottomLimit:0.0}, "
                + $"PropertyGridHeight={shell.PropertyGridHost.ActualHeight:0.0}, ShellHeight={shell.ActualHeight:0.0}");
        }
    }

    private static void AssertDockedResultReviewDensity(string name, VisionToolSingleInputPropertyToolShell shell)
    {
        if (!shell.ResultReviewHost.IsVisible)
        {
            return;
        }

        shell.ResultReviewHost.ApplyTemplate();
        shell.ResultReviewHost.UpdateLayout();
        shell.ResultReviewScrollViewer.ApplyTemplate();
        shell.ResultReviewScrollViewer.UpdateLayout();

        if (shell.ResultReviewHost.ActualHeight > 150D
            || (shell.ResultReviewScrollViewer.Visibility == Visibility.Visible
                && shell.ResultReviewScrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Auto))
        {
            throw new InvalidOperationException(
                name + " docked result review should stay compact and let chips scroll internally. "
                + $"ResultHeight={shell.ResultReviewHost.ActualHeight:0.0}, "
                + $"ChipVisibility={shell.ResultReviewScrollViewer.Visibility}, "
                + $"ChipScroll={shell.ResultReviewScrollViewer.VerticalScrollBarVisibility}");
        }
    }

    private static void AssertDockedDoubleInputToolLayout(string name)
    {
        DependencyObject root = GetActiveToolVisualRoot(name);
        VisionToolDoubleInputCustomToolShell? shell = FindVisualChildren<VisionToolDoubleInputCustomToolShell>(root)
            .FirstOrDefault(item => item.IsVisible);
        if (shell == null)
        {
            throw new InvalidOperationException(name + " did not contain a double-input tool shell.");
        }

        shell.ApplyTemplate();
        shell.UpdateLayout();
        if (!shell.IsDockedInspectorMode)
        {
            throw new InvalidOperationException(name + " did not enter docked inspector layout mode.");
        }

        if (shell.InputAGroup.ActualWidth > 230D || shell.OutputLayerGroup.ActualWidth > 230D)
        {
            throw new InvalidOperationException(
                name + " preview column is still using the large floating layout after docking. "
                + $"InputAWidth={shell.InputAGroup.ActualWidth:0.0}, OutputWidth={shell.OutputLayerGroup.ActualWidth:0.0}");
        }

        if (shell.InputAPreviewFrame.ActualHeight < 50D || shell.OutputPreviewFrame.ActualHeight < 50D)
        {
            throw new InvalidOperationException(
                name + " compact docked previews collapsed too far. "
                + $"InputHeight={shell.InputAPreviewFrame.ActualHeight:0.0}, OutputHeight={shell.OutputPreviewFrame.ActualHeight:0.0}");
        }

        AssertDockedPreviewRouteHint(name + " input A preview", shell.InputAPreviewFrame);
        AssertDockedPreviewRouteHint(name + " output preview", shell.OutputPreviewFrame);
        if (shell.InputBGroup.IsVisible)
        {
            AssertDockedPreviewRouteHint(name + " input B preview", shell.InputBPreviewFrame);
        }

        AssertDockedSummaryStatusStrip(name, shell.SummaryHost, shell.SummaryText, shell.StatusHost, shell.StatusText);

        AssertElementWithinAncestor(
            name + " Add Pipeline button",
            shell.AddPipelineButton,
            shell);

        bool hasVisibleRunAction = false;
        FrameworkElement? visibleRunAction = null;
        if (shell.RunPreviewButton.IsVisible && shell.RunPreviewButton.Visibility == Visibility.Visible)
        {
            AssertElementWithinAncestor(
                name + " Run Preview button",
                shell.RunPreviewButton,
                shell);
            visibleRunAction = shell.RunPreviewButton;
            hasVisibleRunAction = true;
        }

        if (shell.RunOffsetButton.IsVisible && shell.RunOffsetButton.Visibility == Visibility.Visible)
        {
            AssertElementWithinAncestor(
                name + " Run Offset button",
                shell.RunOffsetButton,
                shell);
            visibleRunAction = shell.RunOffsetButton;
            hasVisibleRunAction = true;
        }

        if (!hasVisibleRunAction || visibleRunAction == null)
        {
            throw new InvalidOperationException(name + " did not expose a visible run action after docking.");
        }

        AssertDockedActionRowDensity(name + " active run action", visibleRunAction, shell);
    }

    private static void AssertDockedSummaryStatusStrip(
        string name,
        Border summaryHost,
        TextBlock summaryText,
        Border statusHost,
        TextBlock statusText)
    {
        summaryHost.ApplyTemplate();
        summaryHost.UpdateLayout();
        statusHost.ApplyTemplate();
        statusHost.UpdateLayout();

        if (summaryHost.ActualHeight < 24D
            || summaryHost.ActualHeight > 46D
            || summaryText.TextTrimming != TextTrimming.CharacterEllipsis
            || statusText.TextTrimming != TextTrimming.CharacterEllipsis)
        {
            throw new InvalidOperationException(
                name + " docked summary/status strip is not using the compact inspector rule. "
                + $"SummaryHeight={summaryHost.ActualHeight:0.0}, StatusHeight={statusHost.ActualHeight:0.0}, "
                + $"SummaryTrim={summaryText.TextTrimming}, StatusTrim={statusText.TextTrimming}");
        }

        if (string.IsNullOrWhiteSpace(statusText.Text))
        {
            if (statusHost.Visibility != Visibility.Collapsed)
            {
                throw new InvalidOperationException(
                    name + " docked status strip should collapse when there is no status text. "
                    + $"Visibility={statusHost.Visibility}");
            }

            return;
        }

        if (statusHost.Visibility != Visibility.Visible
            || statusHost.ActualHeight < 22D
            || statusHost.ActualHeight > 44D)
        {
            throw new InvalidOperationException(
                name + " docked status strip should be compact and visible when status text exists. "
                + $"Visibility={statusHost.Visibility}, StatusHeight={statusHost.ActualHeight:0.0}, Text='{statusText.Text}'");
        }

        if (statusHost.Margin.Top > 6D)
        {
            throw new InvalidOperationException(
                name + " docked status strip leaves too much gap under the summary. "
                + $"StatusMarginTop={statusHost.Margin.Top:0.0}");
        }
    }

    private static void AssertDockedActionRowDensity(string name, FrameworkElement actionElement, FrameworkElement shell)
    {
        System.Windows.Point actionOrigin = actionElement.TranslatePoint(new System.Windows.Point(0D, 0D), shell);
        double trailingBlank = shell.ActualHeight - (actionOrigin.Y + actionElement.ActualHeight);
        if (trailingBlank > 42D)
        {
            throw new InvalidOperationException(
                name + " leaves too much empty space below the docked action row. "
                + $"TrailingBlank={trailingBlank:0.0}, ShellHeight={shell.ActualHeight:0.0}, "
                + $"ActionY={actionOrigin.Y:0.0}, ActionHeight={actionElement.ActualHeight:0.0}");
        }
    }

    private static void AssertElementWithinAncestor(string name, FrameworkElement element, FrameworkElement ancestor)
    {
        if (element == null || ancestor == null)
        {
            throw new InvalidOperationException(name + " could not be checked because an element was null.");
        }

        element.ApplyTemplate();
        element.UpdateLayout();
        ancestor.ApplyTemplate();
        ancestor.UpdateLayout();

        if (!element.IsVisible || element.ActualWidth <= 1D || element.ActualHeight <= 1D)
        {
            throw new InvalidOperationException(
                name + " is not visible or has collapsed size. "
                + $"Visible={element.IsVisible}, Size={element.ActualWidth:0.0}x{element.ActualHeight:0.0}");
        }

        System.Windows.Point origin = element.TranslatePoint(new System.Windows.Point(0D, 0D), ancestor);
        if (origin.X < -1D
            || origin.Y < -1D
            || origin.X + element.ActualWidth > ancestor.ActualWidth + 1D
            || origin.Y + element.ActualHeight > ancestor.ActualHeight + 1D)
        {
            throw new InvalidOperationException(
                name + " is outside the docked tool viewport. "
                + $"Origin={origin.X:0.0},{origin.Y:0.0}, Size={element.ActualWidth:0.0}x{element.ActualHeight:0.0}, "
                + $"Ancestor={ancestor.ActualWidth:0.0}x{ancestor.ActualHeight:0.0}");
        }
    }

    private static void ClickFloatingRadioButtonByName(string radioButtonName, string name)
    {
        RadioButton? radioButton = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
            .SelectMany(FindVisualChildren<RadioButton>)
            .FirstOrDefault(item => string.Equals(item.Name, radioButtonName, StringComparison.Ordinal));
        if (radioButton == null)
        {
            throw new InvalidOperationException(name + " was not found: " + radioButtonName);
        }

        radioButton.ApplyTemplate();
        radioButton.IsChecked = true;
        radioButton.RaiseEvent(new RoutedEventArgs(RadioButton.CheckedEvent, radioButton));
        Pump(8);
    }

    private static void ClickActiveFloatingPreviewFrameByAutomationId(string automationId, string name)
    {
        DependencyObject toolRoot = GetActiveToolVisualRoot(name);
        FrameworkElement? frame = FindVisualChildrenIncludingSelf<FrameworkElement>(toolRoot)
            .FirstOrDefault(item => string.Equals(
                System.Windows.Automation.AutomationProperties.GetAutomationId(item),
                automationId,
                StringComparison.Ordinal));
        if (frame == null)
        {
            throw new InvalidOperationException(name + " was not found: " + automationId);
        }

        frame.ApplyTemplate();
        frame.UpdateLayout();
        MouseButtonEventArgs downArgs = new(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseDownEvent,
            Source = frame
        };
        frame.RaiseEvent(downArgs);
        MouseButtonEventArgs upArgs = new(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = UIElement.MouseUpEvent,
            Source = frame
        };
        frame.RaiseEvent(upArgs);
        Pump(12);
    }

    private static void SetFloatingTextBoxTextByName(string textBoxName, string text, string name)
    {
        TextBox? textBox = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
            .SelectMany(FindVisualChildren<TextBox>)
            .FirstOrDefault(item => string.Equals(item.Name, textBoxName, StringComparison.Ordinal));
        if (textBox == null)
        {
            throw new InvalidOperationException(name + " was not found: " + textBoxName);
        }

        textBox.ApplyTemplate();
        textBox.Text = text ?? string.Empty;
        textBox.UpdateLayout();
        Pump(8);
    }

    private static void AssertFloatingNamedElementsDoNotOverlap(string scope, params string[] elementNames)
    {
        Window toolWindow = GetActiveFloatingToolWindow(scope);
        AssertNamedElementsDoNotOverlap(toolWindow, scope, elementNames);
    }

    private static void AssertActiveToolNamedElementsDoNotOverlap(string scope, params string[] elementNames)
    {
        FrameworkElement root = GetActiveToolFrameworkRoot(scope);
        AssertNamedElementsDoNotOverlap(root, scope, elementNames);
    }

    private static void AssertNamedElementsDoNotOverlap(FrameworkElement root, string scope, params string[] elementNames)
    {
        List<(string Name, FrameworkElement Element, Rect Bounds)> visibleElements = new();
        List<string> elementStates = new();

        foreach (string elementName in elementNames)
        {
            FrameworkElement? element = FindVisualChildrenIncludingSelf<FrameworkElement>(root)
                .FirstOrDefault(item => string.Equals(item.Name, elementName, StringComparison.Ordinal));
            if (element == null)
            {
                throw new InvalidOperationException(scope + " could not find layout element: " + elementName);
            }

            element.ApplyTemplate();
            element.UpdateLayout();
            elementStates.Add(string.Format(
                CultureInfo.InvariantCulture,
                "{0}: Visibility={1}, IsVisible={2}, Actual={3:0.0}x{4:0.0}",
                elementName,
                element.Visibility,
                element.IsVisible,
                element.ActualWidth,
                element.ActualHeight));
            if (!element.IsVisible || element.Visibility != Visibility.Visible || element.ActualWidth <= 1D || element.ActualHeight <= 1D)
            {
                continue;
            }

            visibleElements.Add((elementName, element, GetElementBounds(root, element)));
        }

        if (visibleElements.Count < 2)
        {
            throw new InvalidOperationException(
                scope + " did not expose enough visible layout elements to verify. "
                + string.Join("; ", elementStates));
        }

        for (int i = 0; i < visibleElements.Count; i++)
        {
            for (int j = i + 1; j < visibleElements.Count; j++)
            {
                Rect overlap = Rect.Intersect(visibleElements[i].Bounds, visibleElements[j].Bounds);
                if (overlap.Width > 1D && overlap.Height > 1D)
                {
                    throw new InvalidOperationException(
                        scope + " has overlapping controls: "
                        + $"{visibleElements[i].Name} {FormatRect(visibleElements[i].Bounds)} vs "
                        + $"{visibleElements[j].Name} {FormatRect(visibleElements[j].Bounds)}.");
                }
            }
        }
    }

    private static void AssertFloatingNamedElementsVisibleWithinAncestors(string scope, params string[] elementNames)
    {
        Window toolWindow = GetActiveFloatingToolWindow(scope);
        AssertNamedElementsVisibleWithinAncestors(toolWindow, scope, elementNames);
    }

    private static void AssertActiveToolNamedElementsVisibleWithinAncestors(string scope, params string[] elementNames)
    {
        FrameworkElement root = GetActiveToolFrameworkRoot(scope);
        AssertNamedElementsVisibleWithinAncestors(root, scope, elementNames);
    }

    private static void AssertNamedElementsVisibleWithinAncestors(FrameworkElement root, string scope, params string[] elementNames)
    {
        foreach (string elementName in elementNames)
        {
            FrameworkElement? element = FindVisualChildrenIncludingSelf<FrameworkElement>(root)
                .FirstOrDefault(item => string.Equals(item.Name, elementName, StringComparison.Ordinal));
            if (element == null)
            {
                throw new InvalidOperationException(scope + " could not find layout element: " + elementName);
            }

            element.ApplyTemplate();
            element.UpdateLayout();
            if (!element.IsVisible || element.Visibility != Visibility.Visible || element.ActualWidth <= 1D || element.ActualHeight <= 1D)
            {
                throw new InvalidOperationException(scope + " is not visible: " + elementName);
            }

            string? clippedBy = GetFirstAncestorClippingElement(element, root);
            if (!string.IsNullOrWhiteSpace(clippedBy))
            {
                throw new InvalidOperationException(scope + " is clipped by " + clippedBy + ": " + elementName);
            }
        }
    }

    private static string? GetFirstAncestorClippingElement(FrameworkElement element, FrameworkElement boundary)
    {
        Point center = new(element.ActualWidth / 2D, element.ActualHeight / 2D);
        DependencyObject? current = VisualTreeHelper.GetParent(element);
        while (current != null)
        {
            if (current is FrameworkElement ancestor)
            {
                Point point;
                try
                {
                    point = element.TransformToAncestor(ancestor).Transform(center);
                }
                catch (InvalidOperationException)
                {
                    return ancestor.Name ?? ancestor.GetType().Name;
                }

                if (point.X < -1D
                    || point.Y < -1D
                    || point.X > ancestor.ActualWidth + 1D
                    || point.Y > ancestor.ActualHeight + 1D)
                {
                    string ancestorName = string.IsNullOrWhiteSpace(ancestor.Name)
                        ? ancestor.GetType().Name
                        : ancestor.Name;
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} point={1:0.0},{2:0.0} ancestor={3:0.0}x{4:0.0} element={5:0.0}x{6:0.0}",
                        ancestorName,
                        point.X,
                        point.Y,
                        ancestor.ActualWidth,
                        ancestor.ActualHeight,
                        element.ActualWidth,
                        element.ActualHeight);
                }

                if (ReferenceEquals(ancestor, boundary))
                {
                    return null;
                }
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static FrameworkElement GetActiveToolFrameworkRoot(string scope)
    {
        DependencyObject root = GetActiveToolVisualRoot(scope);
        if (root is FrameworkElement frameworkElement)
        {
            frameworkElement.ApplyTemplate();
            frameworkElement.UpdateLayout();
            return frameworkElement;
        }

        throw new InvalidOperationException(scope + " active tool root is not a FrameworkElement.");
    }

    private static Window GetActiveFloatingToolWindow(string scope)
    {
        Window? window = Application.Current.Windows
            .OfType<Window>()
            .LastOrDefault(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow");
        if (window == null)
        {
            throw new InvalidOperationException(scope + " did not open a floating tool window.");
        }

        window.ApplyTemplate();
        window.UpdateLayout();
        return window;
    }

    private static Rect GetElementBounds(FrameworkElement ancestor, FrameworkElement element)
    {
        return element.TransformToAncestor(ancestor)
            .TransformBounds(new Rect(0D, 0D, element.ActualWidth, element.ActualHeight));
    }

    private static string FormatRect(Rect rect)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "[{0:0.0},{1:0.0},{2:0.0},{3:0.0}]",
            rect.X,
            rect.Y,
            rect.Width,
            rect.Height);
    }

    private static void AssertFloatingPropertyGridDialogButtonsReady(string name)
    {
        List<Window> windows = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
            .ToList();

        foreach (Window window in windows)
        {
            if (TryFindHitTestReadyDialogButton(window, name))
            {
                return;
            }

            List<ScrollViewer> scrollViewers = FindVisualChildren<ScrollViewer>(window)
                .Where(item => item.IsVisible && item.ScrollableHeight > 0D)
                .ToList();

            foreach (ScrollViewer scrollViewer in scrollViewers)
            {
                foreach (double offset in GetDialogButtonProbeOffsets(scrollViewer))
                {
                    scrollViewer.ScrollToVerticalOffset(offset);
                    scrollViewer.UpdateLayout();
                    Pump(8);

                    if (TryFindHitTestReadyDialogButton(window, name))
                    {
                        return;
                    }
                }
            }
        }

        throw new InvalidOperationException(name + " was not found. " + DescribeFloatingDialogButtonCandidates());
    }

    private static string DescribeFloatingDialogButtonCandidates()
    {
        List<Window> windows = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
            .ToList();
        if (windows.Count == 0)
        {
            return "FloatingWindows=0";
        }

        List<string> descriptions = new();
        foreach (Window window in windows)
        {
            List<Button> buttons = FindVisualChildren<Button>(window)
                .Where(item => item.IsVisible)
                .Take(20)
                .ToList();
            descriptions.Add(
                "Window="
                + window.ActualWidth.ToString("0", CultureInfo.InvariantCulture)
                + "x"
                + window.ActualHeight.ToString("0", CultureInfo.InvariantCulture)
                + ", Buttons="
                + string.Join(
                    " | ",
                    buttons.Select(button =>
                        "Text='"
                        + ExtractElementText(button.Content)
                        + "', Enabled="
                        + button.IsEnabled.ToString(CultureInfo.InvariantCulture)
                        + ", DC="
                        + (button.DataContext?.GetType().Name ?? "<null>")
                        + ", CP="
                        + (button.CommandParameter?.GetType().Name ?? "<null>")
                        + ", Local="
                        + DescribeLocalValueNames(button))));
        }

        return string.Join("; ", descriptions);
    }

    private static string DescribeLocalValueNames(DependencyObject element)
    {
        if (element == null)
        {
            return string.Empty;
        }

        List<string> names = new();
        LocalValueEnumerator values = element.GetLocalValueEnumerator();
        while (values.MoveNext())
        {
            if (!string.IsNullOrWhiteSpace(values.Current.Property?.Name))
            {
                names.Add(values.Current.Property.Name);
            }
        }

        return string.Join(",", names);
    }

    private static void AssertFloatingPropertyGridVisible(string name)
    {
        bool hasVisiblePropertyGrid = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>)
            .Any(item => item.IsVisible
                && item.ActualWidth >= 240D
                && item.ActualHeight >= 120D
                && item.SelectedObject != null);
        if (!hasVisiblePropertyGrid)
        {
            throw new InvalidOperationException(name + " was not visible or had no selected object.");
        }
    }

    private static void AssertHostedPropertyGridRowsRendered(DependencyObject root, string name)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid? grid = FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>(root)
            .FirstOrDefault(item => item.IsVisible && item.ActualWidth >= 200D && item.ActualHeight >= 100D);
        if (grid == null)
        {
            throw new InvalidOperationException(name + " was not visible.");
        }

        object? selectedObject = grid.SelectedObject;
        int descriptorCount = selectedObject == null
            ? 0
            : System.ComponentModel.TypeDescriptor.GetProperties(selectedObject).Count;
        int renderedTextCount = FindVisualChildren<TextBlock>(grid)
            .Count(item => item.IsVisible
                && !string.IsNullOrWhiteSpace(item.Text)
                && !string.Equals(item.Text, "Search", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(item.Text, "검색", StringComparison.OrdinalIgnoreCase));

        if (selectedObject == null || descriptorCount <= 0 || renderedTextCount < 2)
        {
            throw new InvalidOperationException(
                name + " did not render property rows. "
                + $"Selected={selectedObject?.GetType().Name ?? "<null>"}, Descriptors={descriptorCount}, "
                + $"RenderedTextBlocks={renderedTextCount}");
        }
    }

    private static void AssertFloatingPropertyGridRowsRendered(string name)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid? grid = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>)
            .FirstOrDefault(item => item.IsVisible && item.ActualWidth >= 240D && item.ActualHeight >= 120D);
        if (grid == null)
        {
            throw new InvalidOperationException(name + " was not visible.");
        }

        object? selectedObject = grid.SelectedObject;
        int descriptorCount = selectedObject == null
            ? 0
            : System.ComponentModel.TypeDescriptor.GetProperties(selectedObject).Count;
        int innerPropertyCount = CountInnerPropertyGridItems(grid);
        int renderedTextCount = FindVisualChildren<TextBlock>(grid)
            .Count(item => item.IsVisible
                && !string.IsNullOrWhiteSpace(item.Text)
                && !string.Equals(item.Text, "Search", StringComparison.OrdinalIgnoreCase));

        if (selectedObject == null || descriptorCount <= 0 || renderedTextCount < 2)
        {
            throw new InvalidOperationException(
                name + " did not render property rows. "
                + $"Selected={selectedObject?.GetType().Name ?? "<null>"}, Descriptors={descriptorCount}, "
                + $"InnerItems={innerPropertyCount}, RenderedTextBlocks={renderedTextCount}");
        }
    }

    private static System.Windows.Controls.WpfPropertyGrid.PropertyGrid GetActiveFloatingPropertyGrid(string name)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid? grid = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<System.Windows.Controls.WpfPropertyGrid.PropertyGrid>)
            .FirstOrDefault(item => item.IsVisible && item.SelectedObject != null);
        if (grid == null)
        {
            throw new InvalidOperationException(name + " property grid was not found.");
        }

        grid.ApplyTemplate();
        grid.UpdateLayout();
        return grid;
    }

    private static void AssertFloatingPropertyGridMinimumSize(string name, double minWidth, double minHeight)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        bool docked = IsInsideDockedToolInspector(grid);
        double requiredWidth = docked ? Math.Min(minWidth, 320D) : minWidth;
        double requiredHeight = docked ? Math.Min(minHeight, 280D) : minHeight;
        if (grid.ActualWidth < requiredWidth || grid.ActualHeight < requiredHeight)
        {
            throw new InvalidOperationException(
                name + " is too small for comfortable parameter editing. "
                + $"Actual={grid.ActualWidth:0.0}x{grid.ActualHeight:0.0}, Minimum={requiredWidth:0.0}x{requiredHeight:0.0}, Docked={docked}");
        }
    }

    private static double ScrollActivePropertyGridForNavigationRestore(string name, double requestedOffset)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        ScrollViewer? scrollViewer = FindPrimaryPropertyGridScrollViewer(grid);
        if (scrollViewer == null || scrollViewer.ScrollableHeight < 40D)
        {
            throw new InvalidOperationException(
                name + " did not expose a scrollable PropertyGrid viewport. "
                + $"ScrollableHeight={scrollViewer?.ScrollableHeight ?? 0D:0.0}");
        }

        double targetOffset = Math.Min(requestedOffset, scrollViewer.ScrollableHeight);
        scrollViewer.ScrollToVerticalOffset(targetOffset);
        grid.UpdateLayout();
        Pump(8);
        double actualOffset = grid.VerticalScrollOffsetForTest;
        if (actualOffset < Math.Min(20D, targetOffset * 0.5D))
        {
            throw new InvalidOperationException(
                name + " could not scroll the active PropertyGrid far enough for a restore check. "
                + $"Target={targetOffset:0.0}, Actual={actualOffset:0.0}, Scrollable={scrollViewer.ScrollableHeight:0.0}");
        }

        return actualOffset;
    }

    private static void AssertActivePropertyGridVerticalOffsetAtLeast(string name, double minimumOffset)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        grid.UpdateLayout();
        Pump(8);
        double actualOffset = grid.VerticalScrollOffsetForTest;
        if (actualOffset + 1D < minimumOffset)
        {
            throw new InvalidOperationException(
                name + " did not restore the PropertyGrid scroll position after tool switching. "
                + $"ExpectedAtLeast={minimumOffset:0.0}, Actual={actualOffset:0.0}");
        }
    }

    private static void SetActivePropertyGridSearchText(string name, string searchText)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        TextBox searchTextBox = GetActivePropertyGridSearchTextBox(grid, name);
        searchTextBox.Focus();
        searchTextBox.SelectAll();
        searchTextBox.Text = searchText ?? string.Empty;
        searchTextBox.CaretIndex = searchTextBox.Text.Length;
        searchTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        grid.UpdateLayout();
        Pump(24);
        AssertActivePropertyGridSearchText(name, searchText ?? string.Empty);
    }

    private static void AssertActivePropertyGridSearchText(string name, string expectedText)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        TextBox searchTextBox = GetActivePropertyGridSearchTextBox(grid, name);
        string actualText = searchTextBox.Text ?? string.Empty;
        string bridgeText = grid.SearchTextForTest ?? string.Empty;
        if (!string.Equals(actualText, expectedText, StringComparison.Ordinal)
            || !string.Equals(bridgeText, expectedText, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                name + " PropertyGrid search text mismatch. "
                + $"Expected='{expectedText}', Actual='{actualText}', Bridge='{bridgeText}'");
        }
    }

    private static void AssertActivePropertyGridSearchEmptyMessageVisible(string name, bool expectedVisible)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        grid.UpdateLayout();
        Pump(24);
        bool actualVisible = grid.IsSearchEmptyMessageVisibleForTest;
        if (actualVisible != expectedVisible)
        {
            throw new InvalidOperationException(
                name + " PropertyGrid empty-search message visibility mismatch. "
                + $"Expected={expectedVisible}, Actual={actualVisible}");
        }
    }

    private static TextBox GetActivePropertyGridSearchTextBox(DependencyObject root, string name)
    {
        TextBox? searchTextBox = FindVisualChildren<TextBox>(root)
            .FirstOrDefault(IsPropertyGridSearchTextBox);
        if (searchTextBox == null)
        {
            throw new InvalidOperationException(name + " PropertyGrid search box was not found.");
        }

        return searchTextBox;
    }

    private static bool IsPropertyGridSearchTextBox(DependencyObject element)
    {
        string typeName = element?.GetType().FullName ?? string.Empty;
        return typeName.EndsWith(".Controls.SearchTextBox", StringComparison.Ordinal);
    }

    private static ScrollViewer? FindPrimaryPropertyGridScrollViewer(DependencyObject root)
    {
        return FindVisualChildren<ScrollViewer>(root)
            .Where(item => item.IsVisible)
            .OrderByDescending(item => item.ScrollableHeight)
            .ThenByDescending(item => item.ViewportHeight)
            .FirstOrDefault();
    }

    private static bool IsInsideDockedToolInspector(DependencyObject element)
    {
        DependencyObject? current = element;
        while (current != null)
        {
            if (current is FrameworkElement frameworkElement
                && string.Equals(frameworkElement.Name, "dockedToolContentHost", StringComparison.Ordinal))
            {
                return true;
            }

            DependencyObject? visualParent = null;
            try
            {
                visualParent = VisualTreeHelper.GetParent(current);
            }
            catch
            {
            }

            current = visualParent ?? LogicalTreeHelper.GetParent(current);
        }

        return false;
    }

    private static void SetFloatingPropertyGridPropertyValue(string name, string propertyName, object value)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        OpenVisionLab.PropertyGrid.IPropertyGridProperty? property = grid.Properties?[propertyName];
        if (property == null)
        {
            throw new InvalidOperationException(name + " property was not found: " + propertyName);
        }

        property.SetValue(value);
        grid.UpdateLayout();
        Pump(8);
    }

    private static void SetFloatingPropertyGridTextBoxTextWithoutCommit(string name, string propertyName, string text)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        TextBox? textBox = FindVisualChildren<TextBox>(grid)
            .Where(item => item.IsVisible && item.IsEnabled)
            .FirstOrDefault(item => string.Equals(
                ResolvePropertyGridEditorPropertyName(item),
                propertyName,
                StringComparison.OrdinalIgnoreCase));
        if (textBox == null)
        {
            throw new InvalidOperationException(
                name
                + " text editor was not found for property: "
                + propertyName
                + ". Editors="
                + DescribeFloatingPropertyGridTextEditors(grid));
        }

        textBox.Focus();
        textBox.SelectAll();
        textBox.Text = text ?? string.Empty;
        textBox.CaretIndex = textBox.Text.Length;
        grid.UpdateLayout();
        Pump(16);
    }

    private static string ResolvePropertyGridEditorPropertyName(DependencyObject editor)
    {
        DependencyObject? current = editor;
        while (current != null)
        {
            if (current is FrameworkElement element)
            {
                string resolvedName = ResolvePropertyGridPropertyName(element.DataContext);
                if (!string.IsNullOrWhiteSpace(resolvedName))
                {
                    return resolvedName;
                }
            }

            DependencyObject? visualParent = null;
            try
            {
                visualParent = VisualTreeHelper.GetParent(current);
            }
            catch
            {
            }

            current = visualParent ?? LogicalTreeHelper.GetParent(current);
        }

        return string.Empty;
    }

    private static string ResolvePropertyGridPropertyName(object? propertyObject)
    {
        object? current = propertyObject;
        for (int i = 0; i < 3 && current != null; i++)
        {
            if (ReadPublicProperty(current, "PropertyDescriptor") is PropertyDescriptor descriptor
                && !string.IsNullOrWhiteSpace(descriptor.Name))
            {
                return descriptor.Name;
            }

            if (ReadPublicProperty(current, "Name") is string name && !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            current = ReadPublicProperty(current, "ParentProperty");
        }

        return string.Empty;
    }

    private static string DescribeFloatingPropertyGridTextEditors(DependencyObject root)
    {
        return string.Join(
            ", ",
            FindVisualChildren<TextBox>(root)
                .Where(item => item.IsVisible)
                .Select(item => ResolvePropertyGridEditorPropertyName(item) + "='" + item.Text + "'")
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(20));
    }

    private static void AssertFloatingPropertyBrowsable(string name, string propertyName, bool expected)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(name);
        OpenVisionLab.PropertyGrid.IPropertyGridProperty? property = grid.Properties?[propertyName];
        if (property == null)
        {
            throw new InvalidOperationException(name + " property was not found: " + propertyName);
        }

        if (property.IsBrowsable != expected)
        {
            throw new InvalidOperationException(
                name + " property visibility mismatch for "
                + propertyName
                + ". Expected="
                + expected.ToString(CultureInfo.InvariantCulture)
                + ", Actual="
                + property.IsBrowsable.ToString(CultureInfo.InvariantCulture));
        }
    }

    private static int CountInnerPropertyGridItems(System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid)
    {
        System.Reflection.FieldInfo? field = grid.GetType().GetField(
            "innerPropertyGrid",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        object? innerGrid = field?.GetValue(grid);
        object? properties = innerGrid?.GetType().GetProperty("Properties")?.GetValue(innerGrid, null);
        if (properties is not System.Collections.IEnumerable enumerable)
        {
            return -1;
        }

        int count = 0;
        foreach (object _ in enumerable)
        {
            count++;
        }

        return count;
    }

    private static void WriteFloatingToolWindowCapture(string shellCapturePath)
    {
        Window? toolWindow = Application.Current.Windows
            .OfType<Window>()
            .LastOrDefault(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow");
        if (toolWindow == null)
        {
            throw new InvalidOperationException("Floating tool window was not visible for the supplemental capture.");
        }

        string toolCapturePath = Path.Combine(
            Path.GetDirectoryName(shellCapturePath) ?? ".",
            Path.GetFileNameWithoutExtension(shellCapturePath) + ".tool.png");
        int width = Math.Max(1, (int)Math.Round(toolWindow.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(toolWindow.ActualHeight));
        WriteElementPng(toolWindow, toolCapturePath, width, height);
        WriteOpenGlDiagnostics(toolCapturePath, toolWindow);
    }

    private static bool TryFindHitTestReadyDialogButton(Window window, string name)
    {
        List<Button> buttons = FindVisualChildren<Button>(window)
            .Where(IsVisiblePropertyGridDialogButton)
            .ToList();
        if (buttons.Count == 0)
        {
            return false;
        }

        foreach (Button button in buttons)
        {
            if (IsButtonHitTestReady(window, button))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<double> GetDialogButtonProbeOffsets(ScrollViewer scrollViewer)
    {
        yield return 0D;
        yield return scrollViewer.ScrollableHeight / 2D;
        yield return scrollViewer.ScrollableHeight;
    }

    private static bool IsVisiblePropertyGridDialogButton(Button button)
    {
        if (button == null || !button.IsVisible || !button.IsEnabled || button.ActualWidth <= 0D || button.ActualHeight <= 0D)
        {
            return false;
        }

        return HasDialogButtonText(button)
            || HasBridgeDialogPropertyValue(button)
            || IsPropertyGridPropertyValue(button.DataContext)
            || IsPropertyGridPropertyValue(button.CommandParameter);
    }

    private static bool HasBridgeDialogPropertyValue(Button button)
    {
        if (button == null)
        {
            return false;
        }

        LocalValueEnumerator values = button.GetLocalValueEnumerator();
        while (values.MoveNext())
        {
            LocalValueEntry entry = values.Current;
            if (entry.Property == null
                || !string.Equals(entry.Property.Name, "DialogPropertyValue", StringComparison.Ordinal))
            {
                continue;
            }

            if (IsPropertyGridPropertyValue(entry.Value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasDialogButtonText(Button button)
    {
        string text = ExtractElementText(button.Content);
        return string.Equals(text, "...", StringComparison.Ordinal)
            || (text.Length == 1 && text[0] == '\u2026');
    }

    private static bool IsPropertyGridPropertyValue(object value)
    {
        string typeName = value?.GetType().FullName ?? string.Empty;
        return typeName.Contains("WpfPropertyGrid", StringComparison.Ordinal)
            && (typeName.EndsWith(".PropertyItemValue", StringComparison.Ordinal)
                || typeName.EndsWith(".PropertyItem", StringComparison.Ordinal));
    }

    private static string ExtractElementText(object content)
    {
        if (content == null)
        {
            return string.Empty;
        }

        if (content is string text)
        {
            return text.Trim();
        }

        if (content is TextBlock textBlock)
        {
            return (textBlock.Text ?? string.Empty).Trim();
        }

        if (content is AccessText accessText)
        {
            return (accessText.Text ?? string.Empty).Trim();
        }

        if (content is ContentControl contentControl)
        {
            return ExtractElementText(contentControl.Content);
        }

        if (content is Panel panel)
        {
            foreach (UIElement child in panel.Children)
            {
                string childText = ExtractElementText(child);
                if (!string.IsNullOrWhiteSpace(childText))
                {
                    return childText.Trim();
                }
            }
        }

        if (content is DependencyObject dependencyObject)
        {
            foreach (TextBlock childTextBlock in FindVisualChildrenIncludingSelf<TextBlock>(dependencyObject))
            {
                if (!string.IsNullOrWhiteSpace(childTextBlock.Text))
                {
                    return childTextBlock.Text.Trim();
                }
            }

            foreach (AccessText childAccessText in FindVisualChildrenIncludingSelf<AccessText>(dependencyObject))
            {
                if (!string.IsNullOrWhiteSpace(childAccessText.Text))
                {
                    return childAccessText.Text.Trim();
                }
            }
        }

        return Convert.ToString(content)?.Trim() ?? string.Empty;
    }

    private static bool IsButtonHitTestReady(Window window, Button button)
    {
        button.UpdateLayout();
        Point center = new(button.ActualWidth / 2D, button.ActualHeight / 2D);
        Point windowPoint = button.TranslatePoint(center, window);
        HitTestResult hit = VisualTreeHelper.HitTest(window, windowPoint);
        if (hit == null || !IsVisualAncestorOrSelf(button, hit.VisualHit))
        {
            return false;
        }

        Point screenPoint = button.PointToScreen(center);
        IntPtr hwnd = WindowFromPoint(new NativePoint((int)Math.Round(screenPoint.X), (int)Math.Round(screenPoint.Y)));
        IntPtr windowHwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        return hwnd != IntPtr.Zero && hwnd == windowHwnd;
    }

    private static void AssertComboBoxHitTestReady(ComboBox comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        Window window = Window.GetWindow(comboBox);
        if (window == null)
        {
            throw new InvalidOperationException(name + " is not attached to a window.");
        }

        Point center = new(comboBox.ActualWidth / 2D, comboBox.ActualHeight / 2D);
        Point arrow = new(Math.Max(2D, comboBox.ActualWidth - 15D), comboBox.ActualHeight / 2D);
        string? centerFailure = DescribeComboBoxProbeHitTestFailure(window, comboBox, center);
        string? arrowFailure = DescribeComboBoxProbeHitTestFailure(window, comboBox, arrow);
        if (centerFailure != null)
        {
            throw new InvalidOperationException(
                name + " is visually present but not hit-test ready. "
                + "Center=" + (centerFailure ?? "OK")
                + "; Arrow=" + (arrowFailure ?? "OK"));
        }
    }

    private static bool IsComboBoxProbeHitTestReady(Window window, ComboBox comboBox, Point comboPoint)
    {
        return DescribeComboBoxProbeHitTestFailure(window, comboBox, comboPoint) == null;
    }

    private static string? DescribeComboBoxProbeHitTestFailure(Window window, ComboBox comboBox, Point comboPoint)
    {
        if (comboBox.ActualWidth <= 0D || comboBox.ActualHeight <= 0D)
        {
            return $"InvalidSize {comboBox.ActualWidth:0.0}x{comboBox.ActualHeight:0.0}";
        }

        Point windowPoint = comboBox.TranslatePoint(comboPoint, window);
        bool hasLocalComboHit = false;
        HitTestResult? localHit = VisualTreeHelper.HitTest(comboBox, comboPoint);
        if (localHit != null && IsVisualAncestorOrSelf(comboBox, localHit.VisualHit))
        {
            hasLocalComboHit = true;
        }

        HitTestResult? hit = VisualTreeHelper.HitTest(window, windowPoint);
        if (hit == null)
        {
            return $"NoVisualHit at {windowPoint.X:0.0},{windowPoint.Y:0.0}";
        }

        if (!hasLocalComboHit && !IsVisualAncestorOrSelf(comboBox, hit.VisualHit))
        {
            return "VisualHit=" + hit.VisualHit.GetType().FullName;
        }

        if (!WindowContainsNativeHost(window))
        {
            return null;
        }

        Point screenPoint = comboBox.PointToScreen(comboPoint);
        IntPtr hwnd = WindowFromPoint(new NativePoint((int)Math.Round(screenPoint.X), (int)Math.Round(screenPoint.Y)));
        IntPtr windowHwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            return $"NoNativeHwnd at {screenPoint.X:0.0},{screenPoint.Y:0.0}";
        }

        if (hwnd != windowHwnd)
        {
            return "NativeHwnd=" + hwnd.ToString("X") + ", WindowHwnd=" + windowHwnd.ToString("X");
        }

        return null;
    }

    private static void AssertComboBoxOpensFromPreviewMouseClick(ComboBox? comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        comboBox.IsDropDownOpen = false;
        Pump(8);

        MouseButtonEventArgs args = new(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
        {
            RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
            Source = comboBox
        };
        comboBox.RaiseEvent(args);
        Pump(12);

        if (!comboBox.IsDropDownOpen)
        {
            throw new InvalidOperationException(name + " did not open from a user click path.");
        }

        comboBox.IsDropDownOpen = false;
        Pump(8);
    }

    private static bool IsVisualAncestorOrSelf(DependencyObject expectedAncestor, DependencyObject visual)
    {
        DependencyObject? current = visual;
        while (current != null)
        {
            if (ReferenceEquals(current, expectedAncestor))
            {
                return true;
            }

            DependencyObject? parent = null;
            try
            {
                parent = VisualTreeHelper.GetParent(current);
            }
            catch
            {
            }

            parent ??= LogicalTreeHelper.GetParent(current);
            if (parent == null && current is FrameworkElement frameworkElement)
            {
                parent = frameworkElement.TemplatedParent as DependencyObject;
            }

            current = parent;
        }

        return false;
    }

    private static bool WindowContainsNativeHost(Window window)
    {
        // WindowFromPoint is only a reliable overlay guard when an HWND host can actually cover WPF controls.
        return window != null && FindVisualChildrenIncludingSelf<System.Windows.Interop.HwndHost>(window).Any();
    }

    private static void AssertHostComboBoxInteraction(OpenVisionShellHostView shellHost, string comboBoxName, string name)
    {
        ComboBox? comboBox = FindNamedVisualChild<ComboBox>(shellHost, comboBoxName);
        AssertComboBoxPopupLayout(comboBox, name, minimumItemHeight: 24D);
        AssertComboBoxOpensFromPreviewMouseClick(comboBox, name);
        AssertComboBoxSelectionTextVisible(comboBox, name);
    }

    private static T? FindNamedVisualChild<T>(DependencyObject root, string name) where T : FrameworkElement
    {
        return FindVisualChildren<T>(root)
            .FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.Ordinal));
    }

    private static void AssertComboBoxPopupLayout(ComboBox? comboBox, string name, double minimumItemHeight = 30D)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        AssertComboBoxHitTestReady(comboBox, name);
        AssertComboBoxPopupAnchor(comboBox, name);

        comboBox.IsDropDownOpen = true;
        comboBox.UpdateLayout();
        Pump(16);

        List<ComboBoxItem> items = Enumerable.Range(0, comboBox.Items.Count)
            .Select(index => comboBox.ItemContainerGenerator.ContainerFromIndex(index))
            .OfType<ComboBoxItem>()
            .ToList();
        if (items.Count == 0)
        {
            comboBox.IsDropDownOpen = false;
            throw new InvalidOperationException(name + " did not generate popup items.");
        }

        double minHeight = items.Min(item => item.ActualHeight);
        comboBox.IsDropDownOpen = false;
        if (minHeight < minimumItemHeight)
        {
            throw new InvalidOperationException(name + " popup item height is too small: " + minHeight.ToString("0.0", CultureInfo.InvariantCulture));
        }
    }

    private static void AssertComboBoxPopupAnchor(ComboBox comboBox, string name)
    {
        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        if (comboBox.Template?.FindName("PART_Popup", comboBox) is Popup popup
            && !ReferenceEquals(popup.PlacementTarget, comboBox))
        {
            string targetName = popup.PlacementTarget?.GetType().Name ?? "null";
            throw new InvalidOperationException(name + " popup is not anchored to its ComboBox. PlacementTarget=" + targetName);
        }
    }

    private static void AssertComboBoxSelectionTextVisible(ComboBox? comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        string selectedText = GetComboBoxCurrentText(comboBox);
        if (string.IsNullOrWhiteSpace(selectedText))
        {
            throw new InvalidOperationException(name + " selected text is empty.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        TextBlock? selectedTextBlock = FindVisualChildren<TextBlock>(comboBox)
            .FirstOrDefault(textBlock => string.Equals((textBlock.Text ?? string.Empty).Trim(), selectedText, StringComparison.Ordinal));
        ContentPresenter? selectedPresenter = FindVisualChildren<ContentPresenter>(comboBox)
            .FirstOrDefault(presenter => string.Equals(Convert.ToString(presenter.Content)?.Trim(), selectedText, StringComparison.Ordinal));
        if (selectedTextBlock == null && selectedPresenter == null)
        {
            throw new InvalidOperationException(name + " selected text is not rendered: " + selectedText);
        }

        Brush foreground = selectedTextBlock?.Foreground
            ?? (selectedPresenter != null ? System.Windows.Documents.TextElement.GetForeground(selectedPresenter) : comboBox.Foreground);
        AssertBrushContrast(foreground, comboBox.Background, name + " selected text");
    }

    private static void AssertBrushContrast(Brush foreground, Brush background, string name)
    {
        if (foreground is not SolidColorBrush foregroundBrush || background is not SolidColorBrush backgroundBrush)
        {
            return;
        }

        double contrast = GetContrastRatio(foregroundBrush.Color, backgroundBrush.Color);
        if (contrast < 3.0D)
        {
            throw new InvalidOperationException(name + " contrast is too low: " + contrast.ToString("0.00", CultureInfo.InvariantCulture));
        }
    }

    private static double GetContrastRatio(System.Windows.Media.Color foreground, System.Windows.Media.Color background)
    {
        double foregroundLuminance = GetRelativeLuminance(foreground);
        double backgroundLuminance = GetRelativeLuminance(background);
        double lighter = Math.Max(foregroundLuminance, backgroundLuminance);
        double darker = Math.Min(foregroundLuminance, backgroundLuminance);
        return (lighter + 0.05D) / (darker + 0.05D);
    }

    private static double GetRelativeLuminance(System.Windows.Media.Color color)
    {
        static double Linearize(byte channel)
        {
            double value = channel / 255D;
            return value <= 0.03928D
                ? value / 12.92D
                : Math.Pow((value + 0.055D) / 1.055D, 2.4D);
        }

        return 0.2126D * Linearize(color.R)
            + 0.7152D * Linearize(color.G)
            + 0.0722D * Linearize(color.B);
    }

    private static void AssertVisionToolComboTemplate(ComboBox comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        if (comboBox.Template?.FindName("toggleButton", comboBox) is not ToggleButton)
        {
            throw new InvalidOperationException(name + " is not using the vision tool ComboBox template.");
        }

        if (comboBox.Template.FindName("ComboChrome", comboBox) != null)
        {
            throw new InvalidOperationException(name + " is using the property grid bridge ComboBox template.");
        }
    }

    private static void AssertPropertyGridBridgeComboTemplate(ComboBox comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        if (comboBox.Template?.FindName("ComboChrome", comboBox) == null)
        {
            throw new InvalidOperationException(name + " is not using the property grid bridge ComboBox template.");
        }
    }

    private static void AssertComboBoxSelectionCanChange(ComboBox comboBox, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        if (comboBox.Items.Count < 2)
        {
            return;
        }

        int originalIndex = comboBox.SelectedIndex;
        object originalItem = comboBox.SelectedItem;
        string originalText = comboBox.Text;
        int targetIndex = originalIndex == 0 ? 1 : 0;
        int selectionChangedCount = 0;
        SelectionChangedEventHandler handler = (_, _) => selectionChangedCount++;
        comboBox.SelectionChanged += handler;
        try
        {
            comboBox.SelectedIndex = targetIndex;
            comboBox.UpdateLayout();
            Pump(8);

            if (comboBox.SelectedIndex != targetIndex
                || !Equals(comboBox.SelectedItem, comboBox.Items[targetIndex])
                || selectionChangedCount == 0)
            {
                throw new InvalidOperationException(name + " selection did not change.");
            }
        }
        finally
        {
            comboBox.SelectionChanged -= handler;
            if (originalIndex >= 0 && originalIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = originalIndex;
            }
            else
            {
                comboBox.SelectedItem = originalItem;
                if (comboBox.IsEditable)
                {
                    comboBox.Text = originalText ?? string.Empty;
                }
            }

            comboBox.UpdateLayout();
            Pump(8);
        }
    }

    private static bool ComboBoxContainsText(ComboBox comboBox, string text)
    {
        return comboBox?.Items.Cast<object>()
            .Any(item => string.Equals(Convert.ToString(item), text, StringComparison.OrdinalIgnoreCase)) == true;
    }

    private static string GetComboBoxCurrentText(ComboBox comboBox)
    {
        return Convert.ToString(comboBox?.SelectedItem) ?? comboBox?.Text ?? string.Empty;
    }
    private static void SelectComboBoxItemText(ComboBox comboBox, string text, string name)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        object? match = comboBox.Items.Cast<object>()
            .FirstOrDefault(item => string.Equals(Convert.ToString(item), text, StringComparison.OrdinalIgnoreCase));
        if (match == null)
        {
            throw new InvalidOperationException(name + " did not contain item: " + text);
        }

        bool wasAlreadySelected = Equals(comboBox.SelectedItem, match)
            || string.Equals(Convert.ToString(comboBox.SelectedItem), text, StringComparison.OrdinalIgnoreCase);
        int selectionChangedCount = 0;
        SelectionChangedEventHandler handler = (_, _) => selectionChangedCount++;
        comboBox.SelectionChanged += handler;
        try
        {
            comboBox.SelectedItem = match;
            comboBox.UpdateLayout();
            Pump(8);
        }
        finally
        {
            comboBox.SelectionChanged -= handler;
        }

        bool isSelected = Equals(comboBox.SelectedItem, match)
            || string.Equals(Convert.ToString(comboBox.SelectedItem), text, StringComparison.OrdinalIgnoreCase);
        if (!isSelected)
        {
            throw new InvalidOperationException(name + " did not select item: " + text);
        }

        if (!wasAlreadySelected && selectionChangedCount == 0)
        {
            throw new InvalidOperationException(name + " selection did not raise SelectionChanged: " + text);
        }
    }

    private static string SelectDifferentComboBoxItemText(ComboBox comboBox, string name, Func<string, bool>? itemFilter = null)
    {
        if (comboBox == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        string currentText = GetComboBoxCurrentText(comboBox);
        List<object> candidates = comboBox.Items
            .Cast<object>()
            .Where(item =>
            {
                string text = Convert.ToString(item) ?? string.Empty;
                return !string.IsNullOrWhiteSpace(text)
                    && !string.Equals(text, currentText, StringComparison.OrdinalIgnoreCase)
                    && (itemFilter == null || itemFilter(text));
            })
            .ToList();
        if (candidates.Count == 0)
        {
            candidates = comboBox.Items
                .Cast<object>()
                .Where(item =>
                {
                    string text = Convert.ToString(item) ?? string.Empty;
                    return !string.IsNullOrWhiteSpace(text)
                        && !string.Equals(text, currentText, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();
        }

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException(
                name + " did not contain a different selectable item. Current="
                + currentText
                + ", Items="
                + string.Join(",", comboBox.Items.Cast<object>().Select(item => Convert.ToString(item) ?? string.Empty)));
        }

        object match = candidates[0];
        string selectedText = Convert.ToString(match) ?? string.Empty;
        int selectionChangedCount = 0;
        SelectionChangedEventHandler handler = (_, _) => selectionChangedCount++;
        comboBox.SelectionChanged += handler;
        try
        {
            comboBox.SelectedItem = match;
            comboBox.UpdateLayout();
            Pump(8);
        }
        finally
        {
            comboBox.SelectionChanged -= handler;
        }

        if (!Equals(comboBox.SelectedItem, match) || selectionChangedCount == 0)
        {
            throw new InvalidOperationException(
                name
                + " did not select a different item. Previous="
                + currentText
                + ", Target="
                + selectedText
                + ", Actual="
                + GetComboBoxCurrentText(comboBox));
        }

        return selectedText;
    }

    private static bool IsArithmeticOperationWithInputB(string operationName)
    {
        return !string.Equals(operationName, "Bitwise_NOT", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(operationName, "ABS", StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertComboBoxSelectionTextIsSingle(ComboBox comboBox, string expectedText, string name)
    {
        comboBox.ApplyTemplate();
        comboBox.UpdateLayout();
        Pump(8);

        List<TextBlock> matchingTextBlocks = FindVisualChildren<TextBlock>(comboBox)
            .Where(item => item.IsVisible
                && string.Equals((item.Text ?? string.Empty).Trim(), expectedText, StringComparison.Ordinal))
            .ToList();

        if (matchingTextBlocks.Count > 1)
        {
            throw new InvalidOperationException(name + " rendered duplicate selected text: " + expectedText);
        }
    }

    private static void AssertPropertyGridRangeEditorLayout(DependencyObject root, string name)
    {
        List<Grid> rangeEditors = FindVisualChildren<Grid>(root)
            .Where(item => string.Equals(
                item.GetType().FullName,
                "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase",
                StringComparison.Ordinal))
            .ToList();

        if (rangeEditors.Count == 0)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        foreach (Grid rangeEditor in rangeEditors)
        {
            rangeEditor.ApplyTemplate();
            rangeEditor.UpdateLayout();

            if (rangeEditor.MinWidth < 380D || rangeEditor.ColumnDefinitions.Count < 5)
            {
                throw new InvalidOperationException(name + " does not reserve enough shared range-editor width.");
            }

            if (rangeEditor.ColumnDefinitions[2].MinWidth < 120D
                || rangeEditor.ColumnDefinitions[3].Width.Value < 80D
                || rangeEditor.ColumnDefinitions[4].Width.Value < 80D)
            {
                throw new InvalidOperationException(name + " column widths are too narrow for Min/Max numeric text.");
            }

            bool hasVisibleInvert = FindVisualChildren<CheckBox>(rangeEditor)
                .Any(item => item.IsVisible
                    && item.IsHitTestVisible
                    && ExtractElementText(item).IndexOf("Invert", StringComparison.OrdinalIgnoreCase) >= 0);
            if (hasVisibleInvert)
            {
                throw new InvalidOperationException(name + " should not expose Invert on Min/Max range editors.");
            }
        }
    }

    private static void SetFloatingRangeEditorEndpointTextValue(string name, string rangePropertyName, int endpointIndex, double value)
    {
        Grid rangeEditor = GetFloatingRangeEditorForProperty(name, rangePropertyName);
        List<TextBox> textBoxes = FindRangeEditorValueTextBoxes(rangeEditor);
        if (endpointIndex < 0 || endpointIndex >= textBoxes.Count)
        {
            throw new InvalidOperationException(name + " endpoint text box was not found. Index=" + endpointIndex.ToString(CultureInfo.InvariantCulture));
        }

        TextBox textBox = textBoxes[endpointIndex];
        textBox.Focus();
        textBox.Text = value.ToString("0.###", CultureInfo.CurrentCulture);
        textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        PresentationSource? source = PresentationSource.FromVisual(textBox);
        KeyEventArgs keyEventArgs = new(
            Keyboard.PrimaryDevice,
            source,
            Environment.TickCount,
            Key.Enter)
        {
            RoutedEvent = Keyboard.KeyDownEvent
        };
        textBox.RaiseEvent(keyEventArgs);
        textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        rangeEditor.UpdateLayout();
        Pump(32);
    }

    private static void AssertFloatingRangeEditorEndpointAllowsTransientText(
        string name,
        string rangePropertyName,
        int endpointIndex,
        string transientText,
        double restoreValue)
    {
        Grid rangeEditor = GetFloatingRangeEditorForProperty(name, rangePropertyName);
        List<TextBox> textBoxes = FindRangeEditorValueTextBoxes(rangeEditor);
        if (endpointIndex < 0 || endpointIndex >= textBoxes.Count)
        {
            throw new InvalidOperationException(name + " endpoint text box was not found. Index=" + endpointIndex.ToString(CultureInfo.InvariantCulture));
        }

        TextBox textBox = textBoxes[endpointIndex];
        textBox.Focus();
        textBox.SelectAll();
        textBox.Text = transientText;
        textBox.CaretIndex = textBox.Text.Length;
        rangeEditor.UpdateLayout();
        Pump(16);

        if (!string.Equals(textBox.Text, transientText, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                name
                + " should allow transient edit text before commit. Expected='"
                + transientText
                + "', Actual='"
                + textBox.Text
                + "'");
        }

        SetFloatingRangeEditorEndpointTextValue(name + " restore", rangePropertyName, endpointIndex, restoreValue);
    }

    private static List<TextBox> FindRangeEditorValueTextBoxes(Grid rangeEditor)
    {
        TextBox? minBox = ReadPrivateField<TextBox>(rangeEditor, "_minBox");
        TextBox? maxBox = ReadPrivateField<TextBox>(rangeEditor, "_maxBox");
        if (minBox != null && maxBox != null)
        {
            return new List<TextBox> { minBox, maxBox };
        }

        List<TextBox> allTextBoxes = FindVisualChildren<TextBox>(rangeEditor)
            .Where(item => item.IsVisible)
            .OrderBy(Grid.GetRow)
            .ThenBy(Grid.GetColumn)
            .ToList();
        if (rangeEditor.ColumnDefinitions.Count == 0)
        {
            return allTextBoxes;
        }

        int valueColumn = rangeEditor.ColumnDefinitions.Count - 1;
        List<TextBox> valueTextBoxes = allTextBoxes
            .Where(item => Grid.GetColumn(item) + Math.Max(1, Grid.GetColumnSpan(item)) - 1 >= valueColumn)
            .ToList();
        return valueTextBoxes.Count >= 2 ? valueTextBoxes : allTextBoxes;
    }

    private static void SetFloatingRangeEditorEndpointSliderValue(string name, string rangePropertyName, int endpointIndex, double value)
    {
        Grid rangeEditor = GetFloatingRangeEditorForProperty(name, rangePropertyName);
        List<Slider> sliders = FindRangeEditorValueSliders(rangeEditor);
        if (endpointIndex < 0 || endpointIndex >= sliders.Count)
        {
            throw new InvalidOperationException(name + " endpoint slider was not found. Index=" + endpointIndex.ToString(CultureInfo.InvariantCulture));
        }

        Slider slider = sliders[endpointIndex];
        slider.Focus();
        slider.SetCurrentValue(RangeBase.ValueProperty, Math.Min(slider.Maximum, Math.Max(slider.Minimum, value)));
        slider.GetBindingExpression(RangeBase.ValueProperty)?.UpdateSource();
        rangeEditor.UpdateLayout();
        Pump(32);
    }

    private static List<Slider> FindRangeEditorValueSliders(Grid rangeEditor)
    {
        Slider? minSlider = ReadPrivateField<Slider>(rangeEditor, "_minSlider");
        Slider? maxSlider = ReadPrivateField<Slider>(rangeEditor, "_maxSlider");
        if (minSlider != null && maxSlider != null)
        {
            return new List<Slider> { minSlider, maxSlider };
        }

        List<Slider> allSliders = FindVisualChildren<Slider>(rangeEditor)
            .Where(item => item.IsVisible)
            .OrderBy(Grid.GetRow)
            .ThenBy(Grid.GetColumn)
            .ToList();
        List<Slider> valueSliders = allSliders
            .Where(item => Grid.GetColumn(item) <= 2 && Grid.GetColumn(item) + Math.Max(1, Grid.GetColumnSpan(item)) - 1 >= 2)
            .ToList();
        return valueSliders.Count >= 2 ? valueSliders : allSliders;
    }

    private static T? ReadPrivateField<T>(object instance, string fieldName)
        where T : class
    {
        if (instance == null || string.IsNullOrWhiteSpace(fieldName))
        {
            return null;
        }

        return instance.GetType()
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(instance) as T;
    }

    private static Grid GetFloatingRangeEditorForProperty(string name, string propertyName)
    {
        IEnumerable<Window> floatingWindows = Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow")
            .ToList();

        foreach (Window window in floatingWindows)
        {
            foreach (Grid rangeEditor in FindVisualChildren<Grid>(window)
                .Where(item => string.Equals(
                    item.GetType().FullName,
                    "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase",
                    StringComparison.Ordinal)))
            {
                object? propertyValue = ReadPublicProperty(rangeEditor, "PropertyValue");
                object? parentProperty = ReadPublicProperty(propertyValue, "ParentProperty");
                PropertyDescriptor? descriptor = ReadPublicProperty(parentProperty, "PropertyDescriptor") as PropertyDescriptor;
                string resolvedName = descriptor?.Name
                    ?? ReadPublicProperty(parentProperty, "Name") as string
                    ?? string.Empty;
                if (string.Equals(resolvedName, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    rangeEditor.ApplyTemplate();
                    rangeEditor.UpdateLayout();
                    return rangeEditor;
                }
            }
        }

        throw new InvalidOperationException(name + " range editor was not found for property: " + propertyName);
    }

    private static object? ReadPublicProperty(object? instance, string propertyName)
    {
        if (instance == null || string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        return instance.GetType().GetProperty(propertyName)?.GetValue(instance, null);
    }

    private static object? GetFloatingSelectedObjectPropertyValue(string propertyName)
    {
        System.Windows.Controls.WpfPropertyGrid.PropertyGrid grid = GetActiveFloatingPropertyGrid(propertyName);
        object? selectedObject = grid.SelectedObject;
        return selectedObject?.GetType().GetProperty(propertyName)?.GetValue(selectedObject, null);
    }

    private static double GetFloatingSelectedObjectNumericProperty(string propertyName)
    {
        object? value = GetFloatingSelectedObjectPropertyValue(propertyName);
        return value == null ? 0D : Convert.ToDouble(value, CultureInfo.CurrentCulture);
    }

    private static void AssertFloatingSelectedObjectNumericProperty(string name, string propertyName, double expected)
    {
        double actual = GetFloatingSelectedObjectNumericProperty(propertyName);
        if (Math.Abs(actual - expected) > 0.5D)
        {
            throw new InvalidOperationException(
                name + " property value mismatch for " + propertyName
                + ". Expected=" + expected.ToString(CultureInfo.InvariantCulture)
                + ", Actual=" + actual.ToString(CultureInfo.InvariantCulture)
                + ". RangeEditors=" + DescribeFloatingRangeEditors());
        }
    }

    private static void AssertFloatingSelectedObjectNumericPropertyWithin(string name, string propertyName, double expected, double tolerance)
    {
        double actual = GetFloatingSelectedObjectNumericProperty(propertyName);
        if (Math.Abs(actual - expected) > tolerance)
        {
            throw new InvalidOperationException(
                name + " property value mismatch for " + propertyName
                + ". Expected=" + expected.ToString(CultureInfo.InvariantCulture)
                + ", Actual=" + actual.ToString(CultureInfo.InvariantCulture)
                + ", Tolerance=" + tolerance.ToString(CultureInfo.InvariantCulture)
                + ". RangeEditors=" + DescribeFloatingRangeEditors());
        }
    }

    private static void AssertFloatingSelectedObjectBooleanProperty(string name, string propertyName, bool expected)
    {
        object? value = GetFloatingSelectedObjectPropertyValue(propertyName);
        bool actual = value is bool booleanValue
            ? booleanValue
            : value != null && Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        if (actual != expected)
        {
            throw new InvalidOperationException(
                name + " property value mismatch for " + propertyName
                + ". Expected=" + expected
                + ", Actual=" + actual);
        }
    }

    private static string DescribeFloatingRangeEditors()
    {
        List<string> descriptions = new();
        foreach (Window window in Application.Current.Windows
            .OfType<Window>()
            .Where(item => item.IsVisible && item.GetType().Name == "OpenVisionFloatingToolWindow"))
        {
            foreach (Grid rangeEditor in FindVisualChildren<Grid>(window)
                .Where(item => string.Equals(
                    item.GetType().FullName,
                    "System.Windows.Controls.WpfPropertyGrid.Controls.RangeEditorBase",
                    StringComparison.Ordinal)))
            {
                object? propertyValue = ReadPublicProperty(rangeEditor, "PropertyValue");
                object? parentProperty = ReadPublicProperty(propertyValue, "ParentProperty");
                PropertyDescriptor? descriptor = ReadPublicProperty(parentProperty, "PropertyDescriptor") as PropertyDescriptor;
                string resolvedName = descriptor?.Name
                    ?? ReadPublicProperty(parentProperty, "Name") as string
                    ?? "<unknown>";
                string textBoxes = string.Join(
                    ",",
                    FindVisualChildren<TextBox>(rangeEditor)
                        .Where(item => item.IsVisible)
                        .OrderBy(Grid.GetRow)
                        .ThenBy(Grid.GetColumn)
                        .Select(item => string.Format(
                            CultureInfo.InvariantCulture,
                            "TB(r{0} c{1} s{2} '{3}')",
                            Grid.GetRow(item),
                            Grid.GetColumn(item),
                            Grid.GetColumnSpan(item),
                            item.Text)));
                string sliders = string.Join(
                    ",",
                    FindVisualChildren<Slider>(rangeEditor)
                        .Where(item => item.IsVisible)
                        .OrderBy(Grid.GetRow)
                        .ThenBy(Grid.GetColumn)
                        .Select(item => string.Format(
                            CultureInfo.InvariantCulture,
                            "SL(r{0} c{1} s{2} {3:0.###})",
                            Grid.GetRow(item),
                            Grid.GetColumn(item),
                            Grid.GetColumnSpan(item),
                            item.Value)));
                descriptions.Add(
                    resolvedName
                    + "["
                    + textBoxes
                    + "|"
                    + sliders
                    + "|fields="
                    + DescribeRangeEditorEndpointFields(rangeEditor)
                    + "]");
            }
        }

        return string.Join(" ; ", descriptions);
    }

    private static string DescribeRangeEditorEndpointFields(Grid rangeEditor)
    {
        return string.Join(
            ",",
            DescribeRangeEditorEndpointField(rangeEditor, "_minSlider", RangeBase.ValueProperty),
            DescribeRangeEditorEndpointField(rangeEditor, "_maxSlider", RangeBase.ValueProperty),
            DescribeRangeEditorEndpointField(rangeEditor, "_minBox", TextBox.TextProperty),
            DescribeRangeEditorEndpointField(rangeEditor, "_maxBox", TextBox.TextProperty));
    }

    private static string DescribeRangeEditorEndpointField(Grid rangeEditor, string fieldName, DependencyProperty property)
    {
        FrameworkElement? element = ReadPrivateField<FrameworkElement>(rangeEditor, fieldName);
        if (element == null)
        {
            return fieldName + "=null";
        }

        BindingExpression? binding = BindingOperations.GetBindingExpression(element, property);
        string path = binding?.ParentBinding?.Path?.Path ?? "<no-binding>";
        object? value = element.GetValue(property);
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}:{1} value={2} path={3}",
            fieldName,
            element.GetType().Name,
            value,
            path);
    }

    private static void SetFloatingPropertyGridThresholdSliderValue(string name, double value)
    {
        Slider? slider = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Slider>)
            .Where(item => item.IsVisible
                && item.Minimum <= 0D
                && item.Maximum >= 250D
                && item.Maximum <= 255D)
            .OrderByDescending(item => item.ActualWidth)
            .FirstOrDefault();

        if (slider == null)
        {
            throw new InvalidOperationException(name + " was not found.");
        }

        slider.ApplyTemplate();
        double targetValue = Math.Min(slider.Maximum, Math.Max(slider.Minimum, value));
        if (Math.Abs(slider.Value - targetValue) < 0.5D)
        {
            // Recipe XML persistence means a repeated smoke can reopen with the previous test value.
            // Move to a nearby valid value so the PropertyGrid change event and auto-preview path are exercised.
            double alternate = targetValue + 37D <= slider.Maximum ? targetValue + 37D : targetValue - 37D;
            targetValue = Math.Min(slider.Maximum, Math.Max(slider.Minimum, alternate));
        }

        slider.Value = targetValue;
        slider.UpdateLayout();
        Pump(8);
    }

    private static void SetFloatingSliderValueByName(string name, string sliderName, double value)
    {
        Slider? slider = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Slider>)
            .FirstOrDefault(item => string.Equals(item.Name, sliderName, StringComparison.Ordinal));

        if (slider == null)
        {
            throw new InvalidOperationException(name + " was not found: " + sliderName);
        }

        slider.ApplyTemplate();
        slider.Value = Math.Min(slider.Maximum, Math.Max(slider.Minimum, value));
        slider.UpdateLayout();
        Pump(8);
    }

    private static void SetDifferentFloatingSliderValueByName(string name, string sliderName, double preferredValue, double fallbackValue)
    {
        Slider? slider = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Slider>)
            .FirstOrDefault(item => string.Equals(item.Name, sliderName, StringComparison.Ordinal));
        if (slider == null)
        {
            throw new InvalidOperationException(name + " slider was not found: " + sliderName);
        }

        double targetValue = Math.Abs(slider.Value - preferredValue) < 0.001D ? fallbackValue : preferredValue;
        slider.Value = targetValue;
        slider.UpdateLayout();
        Pump(8);
    }

    private static void AssertFloatingSlidersHaveBreathingRoom(string name)
    {
        List<Slider> sliders = FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<Slider>)
            .Where(item => item.IsVisible)
            .ToList();

        if (sliders.Count == 0)
        {
            throw new InvalidOperationException(name + " did not render any visible sliders.");
        }

        foreach (Slider slider in sliders)
        {
            slider.ApplyTemplate();
            slider.UpdateLayout();
            List<Thumb> thumbs = FindVisualChildren<Thumb>(slider).ToList();
            List<Track> tracks = FindVisualChildren<Track>(slider).ToList();
            if (slider.ClipToBounds
                || slider.ActualHeight < 40D
                || thumbs.Count == 0
                || thumbs.Any(thumb => thumb.ActualHeight <= 0D || thumb.ActualHeight > slider.ActualHeight - 4D)
                || tracks.Count == 0
                || tracks.Any(track => track.ActualHeight <= 0D))
            {
                throw new InvalidOperationException(
                    name + " slider chrome does not have enough breathing room. "
                    + $"Slider={slider.Name}, Height={slider.ActualHeight:0.0}, Clip={slider.ClipToBounds}, Thumbs={thumbs.Count}, Tracks={tracks.Count}");
            }
        }
    }

    private static bool FloatingToolTextContains(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return FindActiveToolVisualRoots()
            .SelectMany(FindVisualChildren<TextBlock>)
            .Any(item => (item.Text ?? string.Empty).Contains(text, StringComparison.Ordinal));
    }

    private static bool FloatingToolComboHasItem(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return FindFloatingComboBoxes()
            .Any(combo => combo.Items.Cast<object>().Any(item => string.Equals(Convert.ToString(item), text, StringComparison.Ordinal)));
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        if (parent == null)
        {
            yield break;
        }

        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (T descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<T> FindVisualChildrenIncludingSelf<T>(DependencyObject parent)
        where T : DependencyObject
    {
        if (parent is T match)
        {
            yield return match;
        }

        foreach (T child in FindVisualChildren<T>(parent))
        {
            yield return child;
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(NativePoint point);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public NativePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;
    }

    private readonly record struct CaptureResult(int Width, int Height, double ElapsedMs);
}
