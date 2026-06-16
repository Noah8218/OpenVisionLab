using Lib.OpenCV.Pipeline;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using Lib.Common;
using OpenVisionLab;
using OpenVisionLab._1._Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.Logging;
using OpenVisionLab.Logging.Controls.Model;
using OpenVisionLab.Logging.Controls.View;
using OpenVisionLab.Logging.Controls.ViewModel;
using OpenVisionLab.MessageDialogs;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using static Lib.Common.FormulaUtil;

internal static class Program
{
    private static bool quietCaptureMode;

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            return Run(args);
        }
        finally
        {
            Application.Exit();
            Application.ExitThread();
        }
    }

    private static int Run(string[] args)
    {
        args = ParseCaptureModeArguments(args);

        if (args.Length >= 2 && string.Equals(args[0], "--all", StringComparison.OrdinalIgnoreCase))
        {
            return CaptureTargets(args[1], CreateTargets());
        }

        if (args.Length >= 1 && string.Equals(args[0], "--list", StringComparison.OrdinalIgnoreCase))
        {
            foreach ((string name, _) in CreateTargets())
            {
                Console.WriteLine(name);
            }

            return 0;
        }

        if (args.Length >= 3
            && (string.Equals(args[0], "--target", StringComparison.OrdinalIgnoreCase)
                || string.Equals(args[0], "--only", StringComparison.OrdinalIgnoreCase)))
        {
            return CaptureSelectedTargets(args[2], args[1]);
        }

        string outputPath = args.Length > 0
            ? args[0]
            : Path.Combine(Path.GetTempPath(), "pipeline_viewer_smoke.png");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        CapturePipelineViewer(outputPath);
        Console.WriteLine(outputPath);
        return File.Exists(outputPath) ? 0 : 1;
    }

    private static string[] ParseCaptureModeArguments(string[] args)
    {
        List<string> normalized = new();
        foreach (string arg in args ?? Array.Empty<string>())
        {
            if (string.Equals(arg, "--quiet", StringComparison.OrdinalIgnoreCase)
                || string.Equals(arg, "--offscreen", StringComparison.OrdinalIgnoreCase))
            {
                quietCaptureMode = true;
                continue;
            }

            if (string.Equals(arg, "--visible-capture", StringComparison.OrdinalIgnoreCase)
                || string.Equals(arg, "--visible", StringComparison.OrdinalIgnoreCase))
            {
                quietCaptureMode = false;
                continue;
            }

            normalized.Add(arg);
        }

        return normalized.ToArray();
    }

    private static List<(string Name, Func<string, CaptureDiagnostics> Capture)> CreateTargets()
    {
        return new List<(string Name, Func<string, CaptureDiagnostics> Capture)>
        {
            ("pipeline_viewer", CapturePipelineViewer),
            ("ai_recipe_form", CaptureAiRecipeForm),
            ("ai_recipe_prompt_contract_check", CaptureAiRecipePromptContractCheck),
            ("ai_recipe_feedback_check", CaptureAiRecipeFeedbackCheck),
            ("ai_recipe_failed_step_focus_check", CaptureAiRecipeFailedStepFocusCheck),
            ("pipeline_form", CapturePipelineForm),
            ("pipeline_form_branch", CapturePipelineFormBranch),
            ("pipeline_form_branch_check", CapturePipelineFormBranchCheck),
            ("pipeline_sample_open_preview", CapturePipelineSampleOpenPreview),
            ("pipeline_sample_llm_open_preview", CapturePipelineSampleLlmOpenPreview),
            ("pipeline_matching_review_check", CapturePipelineMatchingReviewCheck),
            ("pipeline_feature_matching_review_check", CapturePipelineFeatureMatchingReviewCheck),
            ("pipeline_preprocess_parameter_check", CapturePipelinePreprocessParameterCheck),
            ("pipeline_validation_focus_check", CapturePipelineValidationFocusCheck),
            ("pipeline_chain_auto_fix_check", CapturePipelineChainAutoFixCheck),
            ("pipeline_error_flow_check", CapturePipelineErrorFlowCheck),
            ("pipeline_property_grid_contract_check", CapturePipelinePropertyGridContractCheck),
            ("log_panel_contract_check", CaptureLogPanelContractCheck),
            ("tool_result_status_contract_check", CaptureToolResultStatusContractCheck),
            ("pipeline_sample_catalog_check", CapturePipelineSampleCatalogCheck),
            ("sample_inventory_contract_check", CaptureSampleInventoryContractCheck),
            ("pipeline_sample_catalog_run_check", CapturePipelineSampleCatalogRunCheck),
            ("algorithm_sample_contract_check", CaptureAlgorithmSampleContractCheck),
            ("pipeline_tool_result_contract_check", CapturePipelineToolResultContractCheck),
            ("vision_recipe_runner_api_contract_check", CaptureVisionRecipeRunnerApiContractCheck),
            ("pipeline_form_run_preview", CapturePipelineFormRunPreview),
            ("pipeline_designable_forms", CapturePipelineDesignableForms),
            ("pipeline_samples_form", CapturePipelineSamplesForm),
            ("pipeline_samples_check_action", CapturePipelineSamplesCheckAction),
            ("pipeline_samples_pins_line_check_action", CapturePipelineSamplesPinsLineCheckAction),
            ("pipeline_add_step_form", CapturePipelineAddStepForm),
            ("pipeline_add_step_branch_form", CapturePipelineAddStepBranchForm),
            ("pipeline_text_prompt", CapturePipelineTextPromptForm),
            ("message_box_form", CaptureMessageBoxForm),
            ("message_box_info", CaptureMessageBoxInfoForm),
            ("message_box_warning", CaptureMessageBoxWarningForm),
            ("message_box_error", CaptureMessageBoxErrorForm),
            ("message_box_error_details", CaptureMessageBoxErrorDetailsForm),
            ("message_box_confirm", CaptureMessageBoxConfirmForm),
            ("threshold_form", CaptureThresholdForm),
            ("tool_matching_form", CaptureMatchingToolForm),
            ("tool_feature_matching_form", CaptureFeatureMatchingToolForm),
            ("tool_contour_form", CaptureContourToolForm),
            ("tool_blob_form", CaptureBlobToolForm),
            ("tool_line_form", CaptureLineToolForm),
            ("tool_morphology_form", CaptureMorphologyToolForm),
            ("tool_filter_form", CaptureFilterToolForm),
            ("tool_arithmetic_form", CaptureArithmeticToolForm),
            ("tool_edge_detection_form", CaptureEdgeDetectionToolForm),
            ("tool_rotate_scale_form", CaptureRotateAndScaleToolForm),
            ("tool_histogram_form", CaptureHistogramToolForm),
            ("tool_mean_form", CaptureMeanToolForm),
            ("tool_hsv_form", CaptureHsvToolForm),
            ("main_workspace", CaptureMainWorkspace)
        };
    }

    private static int CaptureSelectedTargets(string outputDirectory, string targetText)
    {
        Dictionary<string, Func<string, CaptureDiagnostics>> targets = CreateTargets()
            .ToDictionary(item => item.Name, item => item.Capture, StringComparer.OrdinalIgnoreCase);
        string[] names = targetText
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        List<(string Name, Func<string, CaptureDiagnostics> Capture)> selected = new();
        foreach (string name in names)
        {
            if (!targets.TryGetValue(name, out Func<string, CaptureDiagnostics>? capture))
            {
                Console.WriteLine($"Unknown target: {name}");
                Console.WriteLine("Available targets: " + string.Join(", ", targets.Keys.OrderBy(item => item)));
                return 1;
            }

            selected.Add((name, capture));
        }

        if (selected.Count == 0)
        {
            Console.WriteLine("No target selected.");
            Console.WriteLine("Available targets: " + string.Join(", ", targets.Keys.OrderBy(item => item)));
            return 1;
        }

        return CaptureTargets(outputDirectory, selected);
    }

    private static int CaptureTargets(string outputDirectory, IEnumerable<(string Name, Func<string, CaptureDiagnostics> Capture)> targets)
    {
        Directory.CreateDirectory(outputDirectory);

        int failureCount = 0;
        foreach ((string name, Func<string, CaptureDiagnostics> capture) in targets)
        {
            string path = Path.Combine(outputDirectory, name + ".png");
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                CaptureDiagnostics diagnostics = capture(path);
                stopwatch.Stop();
                ScreenshotAnalysis analysis = AnalyzeScreenshot(path);
                string state = analysis.IsUsable ? "OK" : "NG";
                if (analysis.IsUsable
                    && (!analysis.HasHealthyColorSpread
                        || analysis.HasLargeFlatRegion
                        || diagnostics.HasWarnings))
                {
                    state = "WARN";
                }

                if (!analysis.IsUsable)
                {
                    failureCount++;
                }

                WriteIssueFile(path, diagnostics);
                Console.WriteLine(
                    $"{name}=OK|check={state}|elapsed={stopwatch.ElapsedMilliseconds}ms|colors={analysis.SampledColorCount}|flat={analysis.FlatTilePercent:0}%|layout={diagnostics.OverflowIssueCount}|text={diagnostics.TextClipIssueCount}|internal={diagnostics.InternalTextIssueCount}|size={analysis.Width}x{analysis.Height}|{path}");
            }
            catch (Exception ex)
            {
                failureCount++;
                Console.WriteLine($"{name}=NG|{ex.GetType().Name}|{ex.Message}");
            }
        }

        return failureCount == 0 ? 0 : 1;
    }

    private static CaptureDiagnostics CapturePipelineViewer(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        using Bitmap source = CreateSampleImage();
        List<VisionToolOverlay> overlays = CreateSampleOverlays();
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type viewerType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineImageViewer", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineImageViewer type was not found.");
        Type labelModeType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline+OverlayLabelMode", throwOnError: true)
            ?? throw new InvalidOperationException("OverlayLabelMode type was not found.");
        object labelMode = Enum.Parse(labelModeType, "Details");
        ConstructorInfo constructor = viewerType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length >= 7);

        object?[] constructorArgs = constructor.GetParameters().Length >= 8
            ? new object?[]
            {
                "Pipeline Preview Smoke",
                source,
                overlays,
                null,
                labelMode,
                300,
                -1,
                new[] { new RectangleF(72, 64, 260, 150) }
            }
            : new object?[]
            {
                "Pipeline Preview Smoke",
                source,
                overlays,
                null,
                labelMode,
                300,
                -1
            };

        using Form viewer = (Form)constructor.Invoke(constructorArgs);

        viewer.StartPosition = FormStartPosition.Manual;
        viewer.Location = new Point(40, 40);
        viewer.ShowInTaskbar = false;
        return CaptureForm(viewer, outputPath, new Size(1180, 760), 12);
    }

    private static CaptureDiagnostics CapturePipelineForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVision_Pipeline could not be created."));
        SeedPipelineFormWithSample(form, formType);
        return CaptureForm(form, outputPath, new Size(1280, 760), 16, shownForm => AssertPipelineWorkflowHint(shownForm, formType), useScreenCapture: true);
    }

    private static CaptureDiagnostics CapturePipelineFormBranch(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVision_Pipeline could not be created."));
        SeedPipelineFormWithSample(form, formType, branchContourInput: true);
        return CaptureForm(
            form,
            outputPath,
            new Size(1280, 760),
            16,
            shownForm =>
            {
                SelectPipelineStep(shownForm, formType, 2);
                AssertPipelineBranchSelectionState(shownForm, formType);
                AssertPipelineFlowActionHint(shownForm);
            },
            useScreenCapture: true);
    }

    private static void AssertPipelineBranchSelectionState(Form form, Type formType)
    {
        FieldInfo? captionField = formType.GetField("propertiesCaption", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? statusField = formType.GetField("stepIoStatusLabel", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? chainButtonField = formType.GetField("btnStepChainInput", BindingFlags.Instance | BindingFlags.NonPublic);

        string captionText = captionField?.GetValue(form) is Label caption ? caption.Text : string.Empty;
        if (!captionText.Contains("03 Text Symbol Contour", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline branch smoke expected Properties to follow the selected branch step.");
        }

        string statusText = statusField?.GetValue(form) is Label status ? status.Text : string.Empty;
        if (!statusText.Contains("Review branch", StringComparison.OrdinalIgnoreCase)
            || !statusText.Contains("TextSymbol_Clean", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline branch smoke expected selected Step I/O to explain the review branch input.");
        }

        string chainButtonText = chainButtonField?.GetValue(form) is Button button ? button.Text : string.Empty;
        if (!chainButtonText.Contains("Link Prev", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline branch smoke expected Link Prev action for review branch input.");
        }
    }

    private static CaptureDiagnostics CapturePipelineFormBranchCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        VisionPipeline pipeline = CreatePipelineFormSample(branchContourInput: true);
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type validatorType = appAssembly.GetType("OpenVisionLab.VisionPipelineValidator", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineValidator type was not found.");
        MethodInfo validateMethod = validatorType.GetMethod("Validate", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineValidator.Validate was not found.");
        object validation = validateMethod.Invoke(null, new object[] { pipeline, new[] { "Main" } })
            ?? throw new InvalidOperationException("Pipeline validation did not return a result.");
        bool success = validation.GetType().GetProperty("Success")?.GetValue(validation) is bool value && value;
        string errors = InvokeValidationFormatter(validation, "FormatErrors");
        string warnings = InvokeValidationFormatter(validation, "FormatWarnings");
        int warningCount = GetValidationCount(validation, "Warnings");

        if (!success)
        {
            throw new InvalidOperationException("Branch pipeline check should pass with review items, not fail. " + errors);
        }

        if (!warnings.Contains("Review branch input", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Branch check smoke expected a review branch message. Warnings: " + warnings);
        }

        VisionPipeline reviewPipeline = CreatePipelineFormSample();
        reviewPipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "04 Extra Branch Contour",
            ToolType = "Contour",
            InputLayer = "Main",
            OutputLayer = "Extra_Contour"
        });
        object reviewValidation = validateMethod.Invoke(null, new object[] { reviewPipeline, new[] { "Main" } })
            ?? throw new InvalidOperationException("Pipeline review validation did not return a result.");
        string reviewWarnings = InvokeValidationFormatter(reviewValidation, "FormatWarnings");
        if (!reviewWarnings.Contains("no OverlayMerge", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline review check expected an OverlayMerge review warning. Warnings: " + reviewWarnings);
        }

        string[] lines =
        {
            "Pipeline Branch Check",
            "Status: CHECK OK",
            $"Review items: {warningCount}",
            "",
            warnings
        };
        using Form report = CreateSmokeReportForm("Pipeline Branch Check", lines);
        return CaptureForm(report, outputPath, new Size(760, 360), 8);
    }

    private static CaptureDiagnostics CapturePipelinePreprocessParameterCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        VisionPipeline pipeline = CreatePreprocessParameterCheckPipeline();

        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type validatorType = appAssembly.GetType("OpenVisionLab.VisionPipelineValidator", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineValidator type was not found.");
        MethodInfo validateMethod = validatorType.GetMethod("Validate", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineValidator.Validate was not found.");
        object validation = validateMethod.Invoke(null, new object[] { pipeline, new[] { "Main" } })
            ?? throw new InvalidOperationException("Pipeline validation did not return a result.");

        bool success = validation.GetType().GetProperty("Success")?.GetValue(validation) is bool value && value;
        string errors = InvokeValidationFormatter(validation, "FormatErrors");
        string warnings = InvokeValidationFormatter(validation, "FormatWarnings");
        int errorCount = GetValidationCount(validation, "Errors");
        int warningCount = GetValidationCount(validation, "Warnings");

        if (success)
        {
            throw new InvalidOperationException("Preprocess parameter check expected validation errors.");
        }

        AssertContains(errors, "RangeMin is greater than RangeMax", "range min/max validation");
        AssertContains(errors, "SobelDegreeX and SobelDegreeY cannot both be 0", "Sobel derivative validation");
        AssertContains(warnings, "RangeMin is usually expected", "gray range warning");
        AssertContains(warnings, "BlockSize should usually be odd", "adaptive block warning");
        AssertContains(warnings, "CannyApertureSize should usually be 3, 5, or 7", "Canny aperture warning");
        AssertContains(warnings, "SobelKernelSize should usually be between 1 and 31", "Sobel kernel range warning");

        string[] lines =
        {
            "Pipeline Preprocess Parameter Check",
            "Status: CHECK OK",
            $"Errors: {errorCount}",
            $"Warnings: {warningCount}",
            "",
            "Errors",
            errors,
            "",
            "Warnings",
            warnings
        };
        using Form report = CreateSmokeReportForm("Pipeline Preprocess Parameter Check", lines);
        return CaptureForm(report, outputPath, new Size(980, 560), 8);
    }

    private static CaptureDiagnostics CapturePipelineValidationFocusCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVision_Pipeline could not be created."));

        VisionPipeline pipeline = CreatePreprocessParameterCheckPipeline();
        pipeline.Steps[0].Parameters["RangeMax"] = "300";
        FieldInfo? pipelineField = formType.GetField("pipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? bindPipelineMethod = formType.GetMethod("BindPipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? focusErrorMethod = formType.GetMethod("FocusFirstValidationError", BindingFlags.Instance | BindingFlags.NonPublic);
        PropertyInfo? selectedIndexProperty = formType.GetProperty("SelectedStepIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runLogField = formType.GetField("tbRunLog", BindingFlags.Instance | BindingFlags.NonPublic);

        if (pipelineField == null || bindPipelineMethod == null || focusErrorMethod == null || selectedIndexProperty == null)
        {
            throw new InvalidOperationException("Pipeline validation focus members were not found.");
        }

        pipelineField.SetValue(form, pipeline);
        bindPipelineMethod.Invoke(form, null);

        Type validatorType = appAssembly.GetType("OpenVisionLab.VisionPipelineValidator", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineValidator type was not found.");
        MethodInfo validateMethod = validatorType.GetMethod("Validate", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineValidator.Validate was not found.");
        object validation = validateMethod.Invoke(null, new object[] { pipeline, new[] { "Main" } })
            ?? throw new InvalidOperationException("Pipeline validation did not return a result.");

        focusErrorMethod.Invoke(form, new[] { validation });
        int selectedIndex = selectedIndexProperty.GetValue(form) is int index ? index : -1;
        string runLog = runLogField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;

        if (selectedIndex != 1)
        {
            throw new InvalidOperationException($"Expected validation focus to select Step 2, but selected index was {selectedIndex}.");
        }

        if (!runLog.Contains("FOCUS | Step 2 selected", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Validation focus log was not written.");
        }

        string[] lines =
        {
            "Pipeline Validation Focus Check",
            "Status: CHECK OK",
            "",
            "Validation error: Step 2 Sobel derivative pair",
            $"Selected step index: {selectedIndex}",
            "Run log:",
            runLog
        };
        using Form report = CreateSmokeReportForm("Pipeline Validation Focus Check", lines);
        return CaptureForm(report, outputPath, new Size(860, 420), 8);
    }

    private static VisionPipeline CreatePreprocessParameterCheckPipeline()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_PreprocessParameterCheck"
        };

        VisionPipelineStep threshold = new()
        {
            Name = "01 Threshold Review",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "Threshold_Out"
        };
        threshold.Parameters["Mode"] = "Range";
        threshold.Parameters["RangeMin"] = "260";
        threshold.Parameters["RangeMax"] = "10";
        threshold.Parameters["BlockSize"] = "10";
        pipeline.Steps.Add(threshold);

        VisionPipelineStep edge = new()
        {
            Name = "02 Edge Error",
            ToolType = "EdgeDetection",
            InputLayer = "Threshold_Out",
            OutputLayer = "Edge_Out"
        };
        edge.Parameters["EdgeType"] = "Sobel";
        edge.Parameters["SobelDegreeX"] = "0";
        edge.Parameters["SobelDegreeY"] = "0";
        edge.Parameters["SobelKernelSize"] = "34";
        edge.Parameters["CannyApertureSize"] = "4";
        pipeline.Steps.Add(edge);

        return pipeline;
    }

    private static void AssertContains(string text, string expected, string label)
    {
        if (text?.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return;
        }

        throw new InvalidOperationException($"Expected {label}: {expected}");
    }

    private static void AssertAiRecipePromptContract(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new InvalidOperationException("AI Recipe prompt was empty.");
        }

        string supportedBlock = ExtractPromptBlock(prompt, "Supported ToolType values:", "Unsupported pipeline ToolType guard:");
        string[] supportedToolTypes =
        {
            "Threshold",
            "Morphology",
            "Filter",
            "EdgeDetection",
            "Blob",
            "Contour",
            "LineGauge",
            "RotateScale",
            "Matching",
            "Mean",
            "FeatureMatching",
            "OverlayMerge"
        };

        foreach (string toolType in supportedToolTypes)
        {
            AssertContains(supportedBlock, "- " + toolType, "AI Recipe supported ToolType " + toolType);
        }

        string[] forbiddenSupportedToolTypes =
        {
            "- HSV",
            "- Histogram",
            "- Arithmetic",
            "- Color",
            "- Barcode",
            "- QR",
            "- OCR",
            "- EasyBarCode",
            "- EasyQRCode",
            "- EasyOcr"
        };

        foreach (string forbidden in forbiddenSupportedToolTypes)
        {
            if (supportedBlock.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new InvalidOperationException("AI Recipe supported ToolType block contains form-only ToolType: " + forbidden);
            }
        }

        AssertContains(prompt, "Unsupported pipeline ToolType guard:", "AI Recipe unsupported ToolType guard");
        AssertContains(prompt, "Do not output HSV, Histogram, Arithmetic, Color, Barcode, QR, OCR", "AI Recipe form-only ToolType warning");
        AssertContains(prompt, "Use their chains as starting patterns, but do not claim semantic decoding/OCR", "AI Recipe semantic decoder guard");
        AssertContains(prompt, "Prefer sample-backed metric gates", "AI Recipe sample-backed metric guidance");
        AssertContains(prompt, "The final OverlayMerge review layer must contain all branch detections in one image", "AI Recipe final review layer contract");
        AssertContains(prompt, "Users should not need to inspect several separate branch images", "AI Recipe branch review UX contract");
        AssertContains(prompt, "[Required]", "AI Recipe required sample catalog prompt entry");
        AssertContains(prompt, "Expected gate:", "AI Recipe sample expected gate prompt entry");
    }

    private static string ExtractPromptBlock(string prompt, string startMarker, string endMarker)
    {
        int start = prompt.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            throw new InvalidOperationException("Prompt block start was not found: " + startMarker);
        }

        int end = prompt.IndexOf(endMarker, start, StringComparison.OrdinalIgnoreCase);
        if (end < 0)
        {
            throw new InvalidOperationException("Prompt block end was not found: " + endMarker);
        }

        return prompt.Substring(start, end - start);
    }

    private static string TruncatePromptPreview(string prompt, int maxLength)
    {
        if (string.IsNullOrEmpty(prompt) || prompt.Length <= maxLength)
        {
            return prompt ?? string.Empty;
        }

        return prompt.Substring(0, maxLength) + Environment.NewLine + "...";
    }

    private static CaptureDiagnostics CapturePipelinePropertyGridContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        VisionPipelineStep thresholdStep = new()
        {
            Name = "01 Threshold Range",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "Threshold_Out"
        };
        thresholdStep.Parameters["Mode"] = "Range";
        thresholdStep.Parameters["Threshold"] = "127";
        thresholdStep.Parameters["ThresholdType"] = "Binary";
        thresholdStep.Parameters["MaxValue"] = "255";
        thresholdStep.Parameters["RangeMin"] = "30";
        thresholdStep.Parameters["RangeMax"] = "220";
        thresholdStep.Parameters["Invert"] = "True";
        thresholdStep.Parameters["AdaptiveType"] = "MeanC";
        thresholdStep.Parameters["AdaptiveThresholdType"] = "Binary";
        thresholdStep.Parameters["BlockSize"] = "25";
        thresholdStep.Parameters["Weight"] = "5";

        VisionPipelineStepPropertyMapper.SetLayerNameContext(() => new[] { "Main", "Threshold_Out", "Contour_Out", "Main" });
        try
        {
            object propertyObject = CreatePipelineStepProperty(thresholdStep);
            Type propertyType = propertyObject.GetType();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyObject);
            AssertPropertyEditorContract(properties, "Threshold", "WpgThresholdEditor", "ThresholdEditorAttribute");
            AssertPropertyEditorContract(properties, "RangeMin", "WpgRangeEditor", "RangeEditorAttribute");
            AssertPropertyEditorContract(properties, "AcceptanceMetricMinimum", "WpgMetricRangeEditor", "MetricRangeEditorAttribute");
            AssertPropertyBrowsable(properties, "RangeMax", false);
            AssertPropertyBrowsable(properties, "Invert", false);
            AssertPropertyCategory(properties, "InputLayer", "Step");
            AssertPropertyCategory(properties, "OutputLayer", "Step");
            AssertPropertyDescription(properties, "InputLayer", "previous step output");
            AssertPropertyDescription(properties, "OutputLayer", "reviewed later");
            AssertPropertyDescription(properties, "Threshold", "Single threshold value");
            AssertPropertyDescription(properties, "RangeMin", "Combined range threshold");
            AssertPropertyDescription(properties, "AdaptiveType", "Adaptive threshold algorithm");
            AssertPropertyEditorContract(properties, "BlockSize", "WpgSliderEditor", "NumberRangeAttribute");
            AssertPropertyNumberRange(properties, "BlockSize", 3, 255);
            AssertPropertyEditorContract(properties, "Weight", "WpgSliderEditor", "NumberRangeAttribute");
            AssertPropertyNumberRange(properties, "Weight", -50, 50);
            AssertLayerNameConverterContract(properties, "InputLayer", "Main", "Threshold_Out", "Contour_Out");
            AssertLayerNameConverterContract(properties, "OutputLayer", "Main", "Threshold_Out", "Contour_Out");
            AssertLineGaugePropertyGridEditorContract();
            AssertEdgeDetectionPropertyGridEditorContract();
            AssertMorphologyPropertyGridEditorContract();
            AssertFilterPropertyGridEditorContract();
            AssertRotateScalePropertyGridEditorContract();

            Type gridType = Type.GetType("System.Windows.Controls.WpfPropertyGrid.PropertyGrid, WpfPropertyGridBridge", throwOnError: true)
                ?? throw new InvalidOperationException("WpfPropertyGridBridge.PropertyGrid type was not found.");
            object grid = Activator.CreateInstance(gridType)
                ?? throw new InvalidOperationException("WPF property grid could not be created.");

            gridType.GetProperty("SelectedObject")?.SetValue(grid, propertyObject);
            InvokePropertyGridVisibilityBinder(grid);
            AssertGridPropertyBrowsable(grid, "RangeMin", true);
            AssertGridPropertyBrowsable(grid, "RangeMax", false);
            AssertGridPropertyBrowsable(grid, "Invert", false);
            AssertGridPropertyBrowsable(grid, "Threshold", false);

            using Form form = new()
            {
                Text = "Pipeline PropertyGrid Contract Check",
                Width = 900,
                Height = 620,
                StartPosition = FormStartPosition.CenterScreen
            };
            using ElementHost host = new()
            {
                Dock = DockStyle.Fill,
                Child = grid as System.Windows.UIElement
            };

            if (host.Child == null)
            {
                throw new InvalidOperationException("WPF property grid did not expose a UIElement.");
            }

            form.Controls.Add(host);
            form.Tag = propertyType.FullName;
            return CaptureForm(form, outputPath, new Size(900, 620), 16, shownForm =>
            {
                if (shownForm.Controls.Count == 0)
                {
                    throw new InvalidOperationException("PropertyGrid host was not attached.");
                }

                string text = CollectControlText(shownForm);
                if (text.Contains("Range max", StringComparison.OrdinalIgnoreCase)
                    || text.Contains("Use Acceptance\r\nInvert", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Range helper properties were rendered as duplicate rows.");
                }
            }, useScreenCapture: true);
        }
        finally
        {
            VisionPipelineStepPropertyMapper.SetLayerNameContext(null);
        }
    }

    private static object CreatePipelineStepProperty(VisionPipelineStep step)
    {
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type mapperType = appAssembly.GetType("OpenVisionLab.VisionPipelineStepPropertyMapper", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineStepPropertyMapper type was not found.");
        MethodInfo createMethod = mapperType.GetMethod("CreateProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineStepPropertyMapper.CreateProperty was not found.");

        return createMethod.Invoke(null, new object[] { step })
            ?? throw new InvalidOperationException("Pipeline step property mapper returned null.");
    }

    private static CaptureDiagnostics CaptureLogPanelContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        using LogPanelViewModel viewModel = new();
        viewModel.ResetCommand.Execute(null);
        AssertLogPanelFilterContract(viewModel);

        LogPanelView view = new();
        if (view.DataContext is not LogPanelViewModel uiViewModel)
        {
            throw new InvalidOperationException("LogPanelView did not expose LogPanelViewModel as DataContext.");
        }

        uiViewModel.ResetCommand.Execute(null);
        SeedLogPanelViewModel(uiViewModel);
        uiViewModel.IsDetailedMode = true;
        uiViewModel.ShowEntireStream = false;
        uiViewModel.SelectedType = nameof(LogCategory.Pipeline);
        uiViewModel.SelectedLevel = nameof(LogLevel.Warning);
        uiViewModel.SearchText = "recipe";

        using Form form = new()
        {
            Text = "Log Panel Contract Check",
            Width = 900,
            Height = 360,
            StartPosition = FormStartPosition.CenterScreen
        };
        using ElementHost host = new()
        {
            Dock = DockStyle.Fill,
            Child = view
        };

        form.Controls.Add(host);
        return CaptureForm(form, outputPath, new Size(900, 360), 16, shownForm =>
        {
            string text = CollectControlText(shownForm);
            AssertContains(text, "All Logs", "log all-stream toggle");
            AssertContains(text, "Level", "log level label");
            AssertContains(text, "Area", "log area label");
            AssertContains(text, "Auto Scroll", "log auto-scroll label");
            AssertContains(text, "Warning", "visible warning level");
            AssertContains(text, "Pipeline", "visible pipeline category");

            if (text.Contains("Debug", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Debug must not be exposed in the normal log-level filter.");
            }
        }, useScreenCapture: true);
    }

    private static void AssertLogPanelFilterContract(LogPanelViewModel viewModel)
    {
        string[] expectedLevels =
        {
            "Any",
            nameof(LogLevel.Info),
            nameof(LogLevel.Warning),
            nameof(LogLevel.Error)
        };

        string[] actualLevels = viewModel.Levels.ToArray();
        if (!actualLevels.SequenceEqual(expectedLevels, StringComparer.Ordinal))
        {
            throw new InvalidOperationException(
                "Unexpected log levels: " + string.Join(", ", actualLevels));
        }

        if (viewModel.Levels.Any(level => string.Equals(level, nameof(LogLevel.Debug), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Debug must stay hidden from the normal log-level filter.");
        }

        if (!viewModel.ShowEntireStream || viewModel.IsFilterControlsEnabled)
        {
            throw new InvalidOperationException("All Logs should be enabled by default and disable area/level filters.");
        }

        SeedLogPanelViewModel(viewModel);

        viewModel.ShowEntireStream = false;
        viewModel.SelectedType = nameof(LogCategory.Pipeline);
        viewModel.SelectedLevel = nameof(LogLevel.Warning);
        viewModel.SearchText = "recipe";

        if (!viewModel.IsFilterControlsEnabled)
        {
            throw new InvalidOperationException("Area/level filters should be enabled when All Logs is off.");
        }

        if (viewModel.FilteredLogs.Count != 1
            || !viewModel.FilteredLogs[0].Message.Contains("recipe", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Filtered log result should contain exactly one pipeline warning recipe log.");
        }

        AssertContains(viewModel.ActiveFilterText, nameof(LogCategory.Pipeline), "active log area filter text");
        AssertContains(viewModel.ActiveFilterText, nameof(LogLevel.Warning), "active log level filter text");
        AssertContains(viewModel.ActiveFilterText, "Filtered view", "active log filtered-view marker");
        AssertContains(viewModel.ActiveFilterText, "Area:", "active log area label");
        AssertContains(viewModel.ActiveFilterText, "Level:", "active log level label");

        viewModel.SearchText = string.Empty;
        viewModel.ShowEntireStream = true;

        if (viewModel.IsFilterControlsEnabled)
        {
            throw new InvalidOperationException("Area/level filters should be disabled when All Logs is on.");
        }

        if (viewModel.FilteredLogs.Count != 3)
        {
            throw new InvalidOperationException($"All Logs should show all seeded logs. Actual={viewModel.FilteredLogs.Count}");
        }

        if (!viewModel.ActiveFilterText.Contains("All Logs", StringComparison.Ordinal)
            || !viewModel.ActiveFilterText.Contains("Filters off", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"All Logs active filter text should explain disabled filters: {viewModel.ActiveFilterText}");
        }
    }

    private static void SeedLogPanelViewModel(LogPanelViewModel viewModel)
    {
        viewModel.Logs.Clear();
        viewModel.FilteredLogs.Clear();
        viewModel.Logs.AddRange(CreateLogPanelSmokeLines());
        viewModel.ShowEntireStream = false;
        viewModel.ShowEntireStream = true;
    }

    private static List<LogLine> CreateLogPanelSmokeLines()
    {
        DateTime now = DateTime.Now;
        return new List<LogLine>
        {
            CreateLogLine(now.AddMilliseconds(-30), LogCategory.Main, LogLevel.Info, "Main workspace ready."),
            CreateLogLine(now.AddMilliseconds(-20), LogCategory.Pipeline, LogLevel.Warning, "Pipeline recipe needs review."),
            CreateLogLine(now.AddMilliseconds(-10), LogCategory.Pipeline, LogLevel.Error, "Pipeline preview failed.")
        };
    }

    private static LogLine CreateLogLine(DateTime timestamp, LogCategory category, LogLevel level, string message)
    {
        string rawText = string.Format(
            CultureInfo.InvariantCulture,
            "[{0:yyyy-MM-dd HH:mm:ss.fff}][{1}][{2}][Smoke] {3}",
            timestamp,
            category,
            level,
            message);
        return LogLine.Parse(rawText);
    }

    private static void InvokePropertyGridVisibilityBinder(object propertyGrid)
    {
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type binderType = appAssembly.GetType("OpenVisionLab._2._Common.PropertyGridEventBinder", throwOnError: true)
            ?? throw new InvalidOperationException("PropertyGridEventBinder type was not found.");
        object binder = Activator.CreateInstance(binderType, new object?[] { null })
            ?? throw new InvalidOperationException("PropertyGridEventBinder could not be created.");
        MethodInfo applyMethod = binderType.GetMethod("ApplyVisibilityRules", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("PropertyGridEventBinder.ApplyVisibilityRules was not found.");

        applyMethod.Invoke(binder, new[] { propertyGrid });
    }

    private static void AssertGridPropertyBrowsable(object propertyGrid, string propertyName, bool expectedBrowsable)
    {
        object? properties = propertyGrid.GetType().GetProperty("Properties")?.GetValue(propertyGrid);
        object? propertyItem = properties?.GetType()
            .GetMethod("get_Item", new[] { typeof(string) })
            ?.Invoke(properties, new object[] { propertyName });

        if (propertyItem == null)
        {
            if (!expectedBrowsable)
            {
                return;
            }

            throw new InvalidOperationException($"Grid property '{propertyName}' was not found.");
        }

        bool actualBrowsable = propertyItem.GetType().GetProperty("IsBrowsable")?.GetValue(propertyItem) is bool value && value;
        if (actualBrowsable != expectedBrowsable)
        {
            throw new InvalidOperationException($"{propertyName}: expected grid Browsable={expectedBrowsable}, actual {actualBrowsable}.");
        }
    }

    private static void AssertEdgeDetectionPropertyGridEditorContract()
    {
        VisionPipelineStep edgeStep = new()
        {
            Name = "02 Edge Detection",
            ToolType = "EdgeDetection",
            InputLayer = "Filtered",
            OutputLayer = "Edge_Out"
        };
        edgeStep.Parameters["EdgeType"] = "Canny";
        edgeStep.Parameters["CannyThresholdLow"] = "100";
        edgeStep.Parameters["CannyThresholdHigh"] = "200";
        edgeStep.Parameters["CannyApertureSize"] = "3";
        edgeStep.Parameters["SobelDegreeX"] = "1";
        edgeStep.Parameters["SobelDegreeY"] = "0";
        edgeStep.Parameters["SobelKernelSize"] = "3";
        edgeStep.Parameters["ScharrDegreeX"] = "1";
        edgeStep.Parameters["ScharrDegreeY"] = "0";
        edgeStep.Parameters["LaplacianKernelSize"] = "3";

        object propertyObject = CreatePipelineStepProperty(edgeStep);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyObject);
        AssertPropertyEditorContract(properties, "CannyThresholdLow", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "CannyThresholdLow", 0, 255);
        AssertPropertyEditorContract(properties, "CannyThresholdHigh", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "CannyThresholdHigh", 0, 255);
        AssertPropertyEditorContract(properties, "CannyApertureSize", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "CannyApertureSize", 3, 7);
        AssertPropertyEditorContract(properties, "SobelDegreeX", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SobelDegreeX", 0, 2);
        AssertPropertyEditorContract(properties, "SobelDegreeY", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SobelDegreeY", 0, 2);
        AssertPropertyEditorContract(properties, "SobelKernelSize", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SobelKernelSize", 1, 31);
        AssertPropertyEditorContract(properties, "ScharrDegreeX", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "ScharrDegreeX", 0, 1);
        AssertPropertyEditorContract(properties, "ScharrDegreeY", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "ScharrDegreeY", 0, 1);
        AssertPropertyEditorContract(properties, "LaplacianKernelSize", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "LaplacianKernelSize", 1, 31);
    }

    private static void AssertMorphologyPropertyGridEditorContract()
    {
        VisionPipelineStep morphologyStep = new()
        {
            Name = "03 Morphology",
            ToolType = "Morphology",
            InputLayer = "Threshold_Out",
            OutputLayer = "Morphology_Out"
        };
        morphologyStep.Parameters["Shape"] = "Rect";
        morphologyStep.Parameters["Operator"] = "Close";
        morphologyStep.Parameters["KernelWidth"] = "5";
        morphologyStep.Parameters["KernelHeight"] = "7";
        morphologyStep.Parameters["Iterations"] = "2";

        object propertyObject = CreatePipelineStepProperty(morphologyStep);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyObject);
        AssertPropertyEditorContract(properties, "KernelWidth", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "KernelWidth", 1, 99);
        AssertPropertyEditorContract(properties, "KernelHeight", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "KernelHeight", 1, 99);
        AssertPropertyEditorContract(properties, "Iterations", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "Iterations", 1, 20);
    }

    private static void AssertFilterPropertyGridEditorContract()
    {
        VisionPipelineStep filterStep = new()
        {
            Name = "04 Filter",
            ToolType = "Filter",
            InputLayer = "Main",
            OutputLayer = "Filter_Out"
        };
        filterStep.Parameters["FilterType"] = "Bilateral";
        filterStep.Parameters["KernelWidth"] = "3";
        filterStep.Parameters["KernelHeight"] = "3";
        filterStep.Parameters["MedianKernelSize"] = "5";
        filterStep.Parameters["Diameter"] = "7";
        filterStep.Parameters["SigmaColor"] = "40";
        filterStep.Parameters["SigmaSpace"] = "40";

        object propertyObject = CreatePipelineStepProperty(filterStep);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyObject);
        AssertPropertyEditorContract(properties, "KernelWidth", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "KernelWidth", 1, 99);
        AssertPropertyEditorContract(properties, "KernelHeight", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "KernelHeight", 1, 99);
        AssertPropertyEditorContract(properties, "MedianKernelSize", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "MedianKernelSize", 3, 99);
        AssertPropertyEditorContract(properties, "Diameter", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "Diameter", 1, 99);
        AssertPropertyEditorContract(properties, "SigmaColor", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SigmaColor", 0, 255);
        AssertPropertyEditorContract(properties, "SigmaSpace", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SigmaSpace", 0, 255);
    }

    private static void AssertRotateScalePropertyGridEditorContract()
    {
        VisionPipelineStep rotateScaleStep = new()
        {
            Name = "05 RotateScale",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "RotateScale_Out"
        };
        rotateScaleStep.Parameters["Angle"] = "5";
        rotateScaleStep.Parameters["ScaleXPercent"] = "120";
        rotateScaleStep.Parameters["ScaleYPercent"] = "80";

        object propertyObject = CreatePipelineStepProperty(rotateScaleStep);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(propertyObject);
        AssertPropertyEditorContract(properties, "Angle", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "Angle", -180, 180);
        AssertPropertyEditorContract(properties, "ScaleXPercent", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "ScaleXPercent", 1, 300);
        AssertPropertyEditorContract(properties, "ScaleYPercent", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "ScaleYPercent", 1, 300);
    }

    private static void AssertLineGaugePropertyGridEditorContract()
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(LineGaugeProperty));
        AssertPropertyEditorContract(properties, "CONTRAST", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "CONTRAST", 0, 255);
        AssertPropertyEditorContract(properties, "THICKNESS", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "THICKNESS", 1, 50);
        AssertPropertyEditorContract(properties, "SAMPLING_STEP", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "SAMPLING_STEP", 1, 100);
        AssertPropertyEditorContract(properties, "POINT_RANGE", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "POINT_RANGE", 1, 100);
        AssertPropertyEditorContract(properties, "MANUAL_ANGLE_VALUE", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "MANUAL_ANGLE_VALUE", -180, 180);
        AssertPropertyEditorContract(properties, "EXTEND_FIT_LINE_VALUE", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "EXTEND_FIT_LINE_VALUE", 0, 1000);
        AssertPropertyEditorContract(properties, "AVERAGE_Diff", "WpgSliderEditor", "NumberRangeAttribute");
        AssertPropertyNumberRange(properties, "AVERAGE_Diff", 0, 255);
    }

    private static void AssertPropertyEditorContract(
        PropertyDescriptorCollection properties,
        string propertyName,
        string expectedEditorName,
        string expectedAttributeName)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");

        Attribute editorAttribute = descriptor.Attributes
            .Cast<Attribute>()
            .FirstOrDefault(item => item.GetType().Name == "PropertyEditorAttribute")
            ?? throw new InvalidOperationException($"{propertyName}: PropertyEditorAttribute is missing.");
        object editorTypeValue = editorAttribute.GetType().GetProperty("EditorType")?.GetValue(editorAttribute)
            ?? throw new InvalidOperationException($"{propertyName}: editor type is missing.");
        string actualEditorName = editorTypeValue is Type editorType ? editorType.Name : editorTypeValue.ToString() ?? string.Empty;
        if (!string.Equals(actualEditorName, expectedEditorName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{propertyName}: expected editor {expectedEditorName}, actual {actualEditorName}.");
        }

        bool hasExpectedAttribute = descriptor.Attributes
            .Cast<Attribute>()
            .Any(item => string.Equals(item.GetType().Name, expectedAttributeName, StringComparison.Ordinal));
        if (!hasExpectedAttribute)
        {
            throw new InvalidOperationException($"{propertyName}: {expectedAttributeName} is missing.");
        }
    }

    private static void AssertPropertyNumberRange(PropertyDescriptorCollection properties, string propertyName, double expectedMinimum, double expectedMaximum)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        Attribute numberRangeAttribute = descriptor.Attributes
            .Cast<Attribute>()
            .FirstOrDefault(item => item.GetType().Name == "NumberRangeAttribute")
            ?? throw new InvalidOperationException($"{propertyName}: NumberRangeAttribute is missing.");

        double actualMinimum = Convert.ToDouble(numberRangeAttribute.GetType().GetProperty("Minimum")?.GetValue(numberRangeAttribute), CultureInfo.InvariantCulture);
        double actualMaximum = Convert.ToDouble(numberRangeAttribute.GetType().GetProperty("Maximum")?.GetValue(numberRangeAttribute), CultureInfo.InvariantCulture);

        if (Math.Abs(actualMinimum - expectedMinimum) > 0.0001 || Math.Abs(actualMaximum - expectedMaximum) > 0.0001)
        {
            throw new InvalidOperationException(
                $"{propertyName}: expected NumberRange {expectedMinimum:0.###}-{expectedMaximum:0.###}, actual {actualMinimum:0.###}-{actualMaximum:0.###}.");
        }
    }

    private static void AssertPropertyBrowsable(PropertyDescriptorCollection properties, string propertyName, bool expectedBrowsable)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        if (descriptor.IsBrowsable != expectedBrowsable)
        {
            throw new InvalidOperationException($"{propertyName}: expected Browsable={expectedBrowsable}, actual {descriptor.IsBrowsable}.");
        }
    }

    private static void AssertPropertyCategory(PropertyDescriptorCollection properties, string propertyName, string expectedCategory)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        if (!string.Equals(descriptor.Category, expectedCategory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{propertyName}: expected category {expectedCategory}, actual {descriptor.Category}.");
        }
    }

    private static void AssertPropertyDescription(PropertyDescriptorCollection properties, string propertyName, string expectedSnippet)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        if (string.IsNullOrWhiteSpace(descriptor.Description)
            || !descriptor.Description.Contains(expectedSnippet, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{propertyName}: expected description containing '{expectedSnippet}'. Actual={descriptor.Description}");
        }
    }

    private static void AssertLayerNameConverterContract(PropertyDescriptorCollection properties, string propertyName, params string[] expectedValues)
    {
        PropertyDescriptor descriptor = properties[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        if (descriptor.Converter == null || !descriptor.Converter.GetStandardValuesSupported())
        {
            throw new InvalidOperationException($"{propertyName}: layer converter should expose standard values.");
        }

        List<string> actualValues = descriptor.Converter
            .GetStandardValues()
            .Cast<object>()
            .Select(value => value?.ToString() ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();

        foreach (string expected in expectedValues)
        {
            if (!actualValues.Any(value => string.Equals(value, expected, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"{propertyName}: expected layer '{expected}' was not exposed. Actual={string.Join(", ", actualValues)}");
            }
        }

        if (actualValues.Count != actualValues.Distinct(StringComparer.OrdinalIgnoreCase).Count())
        {
            throw new InvalidOperationException($"{propertyName}: duplicate layer names were exposed. Actual={string.Join(", ", actualValues)}");
        }
    }

    private static string InvokeValidationFormatter(object validation, string methodName)
    {
        return validation.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.Invoke(validation, null)
            ?.ToString() ?? string.Empty;
    }

    private static int GetValidationCount(object validation, string propertyName)
    {
        object? value = validation.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(validation);
        return value is System.Collections.ICollection collection ? collection.Count : 0;
    }

    private static CaptureDiagnostics CapturePipelineFormRunPreview(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(string);
            });

        using MemoryDisplayManager displayManager = new();
        using Bitmap mainImage = CreatePipelinePreviewRunImage();
        displayManager.SetLayer("Main", mainImage);

        using Form form = (Form)constructor.Invoke(new object?[] { displayManager, "Smoke" });
        form.Tag = formType;
        SeedPipelineFormWithSample(form, formType);
        return CaptureForm(form, outputPath, new Size(1280, 760), 18, RunPipelinePreviewSmoke, useScreenCapture: true);
    }

    private static CaptureDiagnostics CapturePipelineSampleOpenPreview(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(string);
            });

        using MemoryDisplayManager displayManager = new();
        using Form form = (Form)constructor.Invoke(new object?[] { displayManager, "Smoke" });
        form.Tag = formType;
        return CaptureForm(form, outputPath, new Size(1280, 760), 18, RunPipelineSampleOpenPreviewSmoke, useScreenCapture: true);
    }

    private static CaptureDiagnostics CapturePipelineSampleLlmOpenPreview(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(string);
            });

        using MemoryDisplayManager displayManager = new();
        using Form form = (Form)constructor.Invoke(new object?[] { displayManager, "Smoke" });
        form.Tag = formType;
        return CaptureForm(
            form,
            outputPath,
            new Size(1280, 760),
            18,
            shownForm => RunPipelineSampleOpenPreviewSmoke(
                shownForm,
                "Contour_AllSymbolsAndFaint_LLM",
                "MergeOverlayCount=55",
                "AllSymbols_Overlay",
                "Threshold > Morphology > Contour > OverlayMerge"),
            useScreenCapture: true);
    }

    private static CaptureDiagnostics CapturePipelineMatchingReviewCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(string);
            });

        using MemoryDisplayManager displayManager = new();
        using Form form = (Form)constructor.Invoke(new object?[] { displayManager, "Smoke" });
        form.Tag = formType;
        return CaptureForm(
            form,
            outputPath,
            new Size(1280, 760),
            18,
            RunPipelineMatchingReviewSmoke,
            useScreenCapture: true);
    }

    private static CaptureDiagnostics CapturePipelineFeatureMatchingReviewCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVision_Pipeline", throwOnError: true)
            ?? throw new InvalidOperationException("FormVision_Pipeline type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(string);
            });

        using MemoryDisplayManager displayManager = new();
        string outputDirectory = Path.GetDirectoryName(outputPath) ?? ".";
        string templatePath = Path.Combine(outputDirectory, "feature_matching_template.png");
        using (OpenCvSharp.Mat source = CreateFeatureMatchingSourceImage(out OpenCvSharp.Rect templateRect, out _))
        using (OpenCvSharp.Mat template = source.SubMat(templateRect).Clone())
        using (Bitmap sourceBitmap = BitmapImageConverter.ToBitmap(source))
        {
            OpenCvSharp.Cv2.ImWrite(templatePath, template);
            displayManager.SetLayer("Main", sourceBitmap);
        }

        using Form form = (Form)constructor.Invoke(new object?[] { displayManager, "Smoke" });
        form.Tag = formType;
        return CaptureForm(
            form,
            outputPath,
            new Size(1280, 760),
            18,
            shownForm => RunPipelineFeatureMatchingReviewSmoke(shownForm, templatePath),
            useScreenCapture: true);
    }

    private static void RunPipelineMatchingReviewSmoke(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        RunPipelineSampleOpenPreviewSmoke(
            form,
            "Contour_TemplateMatching",
            "ScoreMax=100",
            "Matching_Result",
            "Matching");

        ComboBox previewMode = FindControl<ComboBox>(form, "cbPreviewImageMode")
            ?? throw new InvalidOperationException("Pipeline preview mode combo was not found.");
        previewMode.SelectedItem = "Overlay";
        if (!string.Equals(previewMode.SelectedItem?.ToString(), "Overlay", StringComparison.OrdinalIgnoreCase))
        {
            previewMode.SelectedIndex = Math.Min(3, previewMode.Items.Count - 1);
        }
        PumpUi(6);

        previewMode.SelectedItem = "Overlay";
        if (!string.Equals(previewMode.SelectedItem?.ToString(), "Overlay", StringComparison.OrdinalIgnoreCase))
        {
            previewMode.SelectedIndex = Math.Min(3, previewMode.Items.Count - 1);
        }
        PumpUi(6);

        SelectPipelineStep(form, formType, 0);
        PumpUi(8);

        Panel reviewPanel = FindControl<Panel>(form, "matchingReviewPanel")
            ?? throw new InvalidOperationException("Pipeline Matching Review panel was not found.");
        PictureBox templateBox = FindControl<PictureBox>(form, "matchingTemplateBox")
            ?? throw new InvalidOperationException("Pipeline Matching template preview was not found.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "matchingDetectedBox")
            ?? throw new InvalidOperationException("Pipeline Matching detected crop preview was not found.");
        Label summaryLabel = FindControl<Label>(form, "matchingReviewSummary")
            ?? throw new InvalidOperationException("Pipeline Matching review summary was not found.");
        DataGridView resultGrid = FindControl<DataGridView>(form, "resultGrid")
            ?? throw new InvalidOperationException("Pipeline result grid was not found.");

        if (!reviewPanel.Visible)
        {
            throw new InvalidOperationException("Pipeline Matching Review panel should be visible when a Matching step is selected.");
        }

        if (templateBox.Image == null)
        {
            throw new InvalidOperationException("Pipeline Matching Review should show the template image.");
        }

        if (detectedBox.Image == null)
        {
            throw new InvalidOperationException("Pipeline Matching Review should show the detected crop image.");
        }

        if (templateBox.Cursor != Cursors.Hand || detectedBox.Cursor != Cursors.Hand)
        {
            throw new InvalidOperationException("Pipeline Matching Review images should expose a hand cursor for zoomable inspection.");
        }

        AssertContains(summaryLabel.Text, "Score:", "pipeline matching review score");
        AssertContains(summaryLabel.Text, "Center:", "pipeline matching review center");
        AssertContains(summaryLabel.Text, "Size:", "pipeline matching review size");

        string resultGridText = BuildGridText(resultGrid);
        AssertContains(resultGridText, "Template", "pipeline matching result grid template row");
        AssertContains(resultGridText, "Detected crop", "pipeline matching result grid crop row");
        AssertContains(resultGridText, "Match center", "pipeline matching result grid center row");
    }

    private static void RunPipelineFeatureMatchingReviewSmoke(Form form, string templatePath)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        VisionPipeline pipeline = CreateFeatureMatchingReviewPipeline(templatePath);
        FieldInfo? pipelineField = formType.GetField("pipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? bindPipelineMethod = formType.GetMethod("BindPipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? runMethod = formType.GetMethod("OnRunClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runLogField = formType.GetField("tbRunLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runButtonField = formType.GetField("btnRun", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runningField = formType.GetField("isRunningPipeline", BindingFlags.Instance | BindingFlags.NonPublic);

        if (pipelineField == null || bindPipelineMethod == null || runMethod == null)
        {
            throw new InvalidOperationException("Pipeline FeatureMatching smoke could not access required Pipeline form members.");
        }

        pipelineField.SetValue(form, pipeline);
        bindPipelineMethod.Invoke(form, null);
        if (formType.GetField("tbPipelineName", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(form) is TextBox pipelineNameTextBox)
        {
            pipelineNameTextBox.Text = pipeline.Name;
        }

        if (runLogField?.GetValue(form) is TextBox runLog)
        {
            runLog.Clear();
        }

        ComboBox previewMode = FindControl<ComboBox>(form, "cbPreviewImageMode")
            ?? throw new InvalidOperationException("Pipeline preview mode combo was not found.");
        previewMode.SelectedItem = "Overlay";
        if (!string.Equals(previewMode.SelectedItem?.ToString(), "Overlay", StringComparison.OrdinalIgnoreCase))
        {
            previewMode.SelectedIndex = Math.Min(3, previewMode.Items.Count - 1);
        }

        SelectPipelineStep(form, formType, 0);
        runMethod.Invoke(form, new object?[] { form, EventArgs.Empty });

        bool completed = false;
        bool previewOk = false;
        string lastLogText = string.Empty;
        for (int i = 0; i < 220; i++)
        {
            Application.DoEvents();
            string logText = runLogField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;
            lastLogText = logText;
            bool running = runningField?.GetValue(form) is bool isRunning && isRunning;
            bool runButtonReady = runButtonField?.GetValue(form) is not Control runButton || runButton.Enabled;
            previewOk = logText.Contains("PREVIEW OK", StringComparison.OrdinalIgnoreCase);
            completed = previewOk
                || logText.Contains("PREVIEW NG", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("PREVIEW CANCELED", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("Pipeline failed", StringComparison.OrdinalIgnoreCase);
            if (completed && !running && runButtonReady)
            {
                break;
            }

            Thread.Sleep(80);
        }

        if (!completed)
        {
            throw new TimeoutException("FeatureMatching Pipeline preview did not finish in the smoke capture.");
        }

        if (!previewOk)
        {
            throw new InvalidOperationException("FeatureMatching Pipeline preview expected PREVIEW OK. Log: " + Truncate(lastLogText, 300));
        }

        previewMode.SelectedItem = "Overlay";
        if (!string.Equals(previewMode.SelectedItem?.ToString(), "Overlay", StringComparison.OrdinalIgnoreCase))
        {
            previewMode.SelectedIndex = Math.Min(3, previewMode.Items.Count - 1);
        }
        PumpUi(6);

        SelectPipelineStep(form, formType, 0);
        PumpUi(8);

        Panel reviewPanel = FindControl<Panel>(form, "matchingReviewPanel")
            ?? throw new InvalidOperationException("Pipeline FeatureMatching Review panel was not found.");
        PictureBox templateBox = FindControl<PictureBox>(form, "matchingTemplateBox")
            ?? throw new InvalidOperationException("Pipeline FeatureMatching template preview was not found.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "matchingDetectedBox")
            ?? throw new InvalidOperationException("Pipeline FeatureMatching detected crop preview was not found.");
        Label summaryLabel = FindControl<Label>(form, "matchingReviewSummary")
            ?? throw new InvalidOperationException("Pipeline FeatureMatching review summary was not found.");
        DataGridView resultGrid = FindControl<DataGridView>(form, "resultGrid")
            ?? throw new InvalidOperationException("Pipeline result grid was not found.");

        if (!reviewPanel.Visible)
        {
            throw new InvalidOperationException("Pipeline FeatureMatching Review panel should be visible when a FeatureMatching step is selected.");
        }

        if (templateBox.Image == null || detectedBox.Image == null)
        {
            throw new InvalidOperationException("Pipeline FeatureMatching Review should show both template and detected crop images.");
        }

        if (templateBox.Cursor != Cursors.Hand || detectedBox.Cursor != Cursors.Hand)
        {
            throw new InvalidOperationException("Pipeline FeatureMatching Review images should expose a hand cursor for zoomable inspection.");
        }

        AssertContains(summaryLabel.Text, "Score:", "pipeline feature matching review score");
        AssertContains(summaryLabel.Text, "Center:", "pipeline feature matching review center");
        AssertContains(summaryLabel.Text, "Size:", "pipeline feature matching review size");

        string resultGridText = BuildGridText(resultGrid);
        AssertContains(resultGridText, "Template", "pipeline feature matching result grid template row");
        AssertContains(resultGridText, "Detected crop", "pipeline feature matching result grid crop row");
        AssertContains(resultGridText, "Match center", "pipeline feature matching result grid center row");
    }

    private static VisionPipeline CreateFeatureMatchingReviewPipeline(string templatePath)
    {
        VisionPipeline pipeline = new()
        {
            Name = "Feature_Template_Review"
        };

        VisionPipelineStep step = new()
        {
            Name = "01 Feature Template",
            ToolType = "FeatureMatching",
            InputLayer = "Main",
            OutputLayer = "Feature_Result",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1
        };
        step.Parameters["Name"] = "Feature_Template";
        step.Parameters["TemplatePath"] = templatePath;
        step.Parameters["PATTERN_PATH"] = templatePath;
        step.Parameters["SCORE_MIN"] = "0.95";
        step.Parameters["RANSAC_REPROJ_THRESHOLD"] = "5";
        pipeline.Steps.Add(step);
        return pipeline;
    }

    private static CaptureDiagnostics CapturePipelineErrorFlowCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        List<string> lines = new()
        {
            "Pipeline Error Flow Check",
            "Status: CHECK OK",
            ""
        };

        lines.Add(RunErrorFlowCase(
            "Missing input layer",
            new VisionPipelineStep
            {
                Name = "MissingInput",
                ToolType = "Threshold",
                InputLayer = "MissingLayer",
                OutputLayer = "MissingInput_Output"
            },
            VisionToolErrorCode.InputLayerMissing));

        VisionPipelineStep invalidThresholdRange = new()
        {
            Name = "InvalidThresholdRange",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "InvalidThresholdRange_Output"
        };
        invalidThresholdRange.Parameters["Mode"] = "Range";
        invalidThresholdRange.Parameters["RangeMin"] = "240";
        invalidThresholdRange.Parameters["RangeMax"] = "30";
        lines.Add(RunErrorFlowCase(
            "Invalid threshold range",
            invalidThresholdRange,
            VisionToolErrorCode.ThresholdInvalidRange));

        VisionPipelineStep invalidMorphologyKernel = new()
        {
            Name = "InvalidMorphologyKernel",
            ToolType = "Morphology",
            InputLayer = "Main",
            OutputLayer = "InvalidMorphologyKernel_Output"
        };
        invalidMorphologyKernel.Parameters["KernelWidth"] = "0";
        invalidMorphologyKernel.Parameters["KernelHeight"] = "3";
        lines.Add(RunErrorFlowCase(
            "Invalid morphology kernel",
            invalidMorphologyKernel,
            VisionToolErrorCode.MorphologyInvalidKernel));

        VisionPipelineStep invalidFilterKernel = new()
        {
            Name = "InvalidFilterKernel",
            ToolType = "Filter",
            InputLayer = "Main",
            OutputLayer = "InvalidFilterKernel_Output"
        };
        invalidFilterKernel.Parameters["FilterType"] = "MedianBlur";
        invalidFilterKernel.Parameters["MedianKernelSize"] = "2";
        lines.Add(RunErrorFlowCase(
            "Invalid filter kernel",
            invalidFilterKernel,
            VisionToolErrorCode.FilterInvalidKernel));

        VisionPipelineStep invalidEdgeThreshold = new()
        {
            Name = "InvalidEdgeThreshold",
            ToolType = "EdgeDetection",
            InputLayer = "Main",
            OutputLayer = "InvalidEdgeThreshold_Output"
        };
        invalidEdgeThreshold.Parameters["EdgeType"] = "Canny";
        invalidEdgeThreshold.Parameters["CannyThresholdLow"] = "200";
        invalidEdgeThreshold.Parameters["CannyThresholdHigh"] = "100";
        lines.Add(RunErrorFlowCase(
            "Invalid edge threshold",
            invalidEdgeThreshold,
            VisionToolErrorCode.EdgeDetectionInvalidThreshold));

        VisionPipelineStep invalidContourArea = new()
        {
            Name = "InvalidContourArea",
            ToolType = "Contour",
            InputLayer = "Main",
            OutputLayer = "InvalidContourArea_Output"
        };
        invalidContourArea.Parameters["MIN_AREA"] = "500";
        invalidContourArea.Parameters["MAX_AREA"] = "10";
        lines.Add(RunErrorFlowCase(
            "Invalid contour area",
            invalidContourArea,
            VisionToolErrorCode.ContourInvalidAreaRange));

        VisionPipelineStep contourNoResult = new()
        {
            Name = "ContourNoResult",
            ToolType = "Contour",
            InputLayer = "Main",
            OutputLayer = "ContourNoResult_Output"
        };
        contourNoResult.Parameters["MIN_AREA"] = "1000000";
        contourNoResult.Parameters["MAX_AREA"] = "2000000";
        lines.Add(RunErrorFlowCase(
            "Contour no result",
            contourNoResult,
            VisionToolErrorCode.ContourNoResult));

        VisionPipelineStep invalidBlobArea = new()
        {
            Name = "InvalidBlobArea",
            ToolType = "Blob",
            InputLayer = "Main",
            OutputLayer = "InvalidBlobArea_Output"
        };
        invalidBlobArea.Parameters["MIN_AREA"] = "500";
        invalidBlobArea.Parameters["MAX_AREA"] = "10";
        lines.Add(RunErrorFlowCase(
            "Invalid blob area",
            invalidBlobArea,
            VisionToolErrorCode.BlobInvalidAreaRange));

        VisionPipelineStep blobNoResult = new()
        {
            Name = "BlobNoResult",
            ToolType = "Blob",
            InputLayer = "Main",
            OutputLayer = "BlobNoResult_Output"
        };
        blobNoResult.Parameters["MIN_AREA"] = "1000000";
        blobNoResult.Parameters["MAX_AREA"] = "2000000";
        lines.Add(RunErrorFlowCase(
            "Blob no result",
            blobNoResult,
            VisionToolErrorCode.BlobNoResult));

        VisionPipelineStep invalidLineSampling = new()
        {
            Name = "InvalidLineSampling",
            ToolType = "LineGauge",
            InputLayer = "Main",
            OutputLayer = "InvalidLineSampling_Output"
        };
        invalidLineSampling.Parameters["SAMPLING_STEP"] = "0";
        invalidLineSampling.Parameters["THICKNESS"] = "5";
        lines.Add(RunErrorFlowCase(
            "Invalid line sampling",
            invalidLineSampling,
            VisionToolErrorCode.LineGaugeInvalidSampling));

        VisionPipelineStep lineNoEdge = new()
        {
            Name = "LineNoEdge",
            ToolType = "LineGauge",
            InputLayer = "Main",
            OutputLayer = "LineNoEdge_Output"
        };
        lineNoEdge.Parameters["CvROI"] = "0,0,20,20";
        lineNoEdge.Parameters["USE_ROI"] = "true";
        lineNoEdge.Parameters["CONTRAST"] = "255";
        lineNoEdge.Parameters["SAMPLING_STEP"] = "2";
        lineNoEdge.Parameters["THICKNESS"] = "3";
        lines.Add(RunErrorFlowCase(
            "LineGauge no edge",
            lineNoEdge,
            VisionToolErrorCode.LineGaugeEdgeNotFound));

        VisionPipelineStep invalidRotateScale = new()
        {
            Name = "InvalidRotateScale",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "InvalidRotateScale_Output"
        };
        invalidRotateScale.Parameters["ScaleXPercent"] = "0";
        invalidRotateScale.Parameters["ScaleYPercent"] = "100";
        lines.Add(RunErrorFlowCase(
            "Invalid rotate scale",
            invalidRotateScale,
            VisionToolErrorCode.RotateScaleInvalidScale));

        VisionPipelineStep invalidMeanAdaptiveBlock = new()
        {
            Name = "InvalidMeanAdaptiveBlock",
            ToolType = "Mean",
            InputLayer = "Main",
            OutputLayer = "InvalidMeanAdaptiveBlock_Output"
        };
        invalidMeanAdaptiveBlock.Parameters["USE_ADAPTIVE_THRESHOLD"] = "true";
        invalidMeanAdaptiveBlock.Parameters["BlockSize"] = "2";
        lines.Add(RunErrorFlowCase(
            "Invalid mean adaptive block",
            invalidMeanAdaptiveBlock,
            VisionToolErrorCode.MeanInvalidAdaptiveBlockSize));

        lines.Add(RunErrorFlowCase(
            "Missing matching template",
            new VisionPipelineStep
            {
                Name = "MissingMatchingTemplate",
                ToolType = "Matching",
                InputLayer = "Main",
                OutputLayer = "MissingMatchingTemplate_Output"
            },
            VisionToolErrorCode.MatchingTemplateMissing));

        lines.Add(RunErrorFlowCase(
            "Missing feature template",
            new VisionPipelineStep
            {
                Name = "MissingFeatureTemplate",
                ToolType = "FeatureMatching",
                InputLayer = "Main",
                OutputLayer = "MissingFeatureTemplate_Output"
            },
            VisionToolErrorCode.FeatureTemplateMissing));

        lines.Add(RunErrorFlowCase(
            "Missing output layer",
            new VisionPipelineStep
            {
                Name = "MissingOutput",
                ToolType = "Threshold",
                InputLayer = "Main",
                OutputLayer = string.Empty
            },
            VisionToolErrorCode.InvalidParameter));

        lines.Add(RunErrorFlowCase(
            "Unsupported tool type",
            new VisionPipelineStep
            {
                Name = "UnsupportedTool",
                ToolType = "NotARealTool",
                InputLayer = "Main",
                OutputLayer = "Unsupported_Output"
            },
            VisionToolErrorCode.ToolFactoryFailed));

        using Form report = CreateSmokeReportForm("Pipeline Error Flow Check", lines);
        return CaptureForm(report, outputPath, new Size(840, 420), 8);
    }

    private static CaptureDiagnostics CapturePipelineChainAutoFixCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        VisionPipeline pipeline = new()
        {
            Name = "Smoke_ChainAutoFix"
        };

        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "01 Binary",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "Binary"
        });
        pipeline.Steps[0].Parameters["Threshold"] = "128";
        pipeline.Steps[0].Parameters["ThresholdType"] = "BinaryInv";

        pipeline.Steps.Add(new VisionPipelineStep
        {
            Name = "02 Clean",
            ToolType = "Morphology",
            InputLayer = "Binary",
            OutputLayer = "Clean"
        });
        pipeline.Steps[1].Parameters["Operator"] = "Close";
        pipeline.Steps[1].Parameters["KernelWidth"] = "3";
        pipeline.Steps[1].Parameters["KernelHeight"] = "3";

        VisionPipelineStep brokenContour = new()
        {
            Name = "03 Contour",
            ToolType = "Contour",
            InputLayer = "Main",
            OutputLayer = "ContourOut"
        };
        brokenContour.Parameters["Name"] = "Contour_AutoFix";
        brokenContour.Parameters["USE_THRESHOLD"] = "true";
        brokenContour.Parameters["MIN_AREA"] = "15";
        brokenContour.Parameters["MAX_AREA"] = "2500";
        pipeline.Steps.Add(brokenContour);

        using Bitmap bitmap = CreatePipelinePreviewRunImage();
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", 1000, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!string.Equals(brokenContour.InputLayer, "Clean", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Contour input was not auto-fixed. Input={brokenContour.InputLayer}");
        }

        if (!brokenContour.Parameters.TryGetValue("USE_THRESHOLD", out string? useThreshold)
            || !string.Equals(useThreshold, "false", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Contour internal threshold was not disabled after chain auto-fix.");
        }

        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        if (result.NormalizationMessages.Count < 2
            || !result.NormalizationMessages.Any(message => message.Contains("CHAIN LINK", StringComparison.OrdinalIgnoreCase))
            || !result.NormalizationMessages.Any(message => message.Contains("CHAIN AUTO", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Chain auto-fix messages were not reported by VisionRecipeRunner.");
        }

        string[] lines =
        {
            "Pipeline Chain Auto Fix Check",
            "Status: CHECK OK",
            "",
            "Broken flow: Threshold -> Morphology -> Contour(input Main)",
            "Fixed flow: Contour input Main -> Clean",
            "Internal preprocessing: USE_THRESHOLD true -> false",
            $"Messages: {result.NormalizationMessages.Count}",
            $"Runtime: {result.TotalMilliseconds:0.0} ms"
        };

        using Form report = CreateSmokeReportForm("Pipeline Chain Auto Fix Check", lines);
        return CaptureForm(report, outputPath, new Size(860, 440), 8);
    }

    private static string RunErrorFlowCase(string title, VisionPipelineStep step, VisionToolErrorCode expectedError)
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_ErrorFlow"
        };
        pipeline.Steps.Add(step);

        using Bitmap bitmap = CreatePipelinePreviewRunImage();
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", 1000, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        VisionRecipeStepRunSummary summary = result.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException($"{title}: no step summary was produced.");

        if (result.Success)
        {
            throw new InvalidOperationException($"{title}: expected failure, but recipe returned success.");
        }

        if (summary.ErrorCode != (int)expectedError)
        {
            throw new InvalidOperationException(
                $"{title}: expected ErrorCode {(int)expectedError}:{expectedError}, but got {summary.ErrorCode}:{summary.ErrorName}. Message={summary.Message}");
        }

        if (!string.Equals(summary.Status, "ERROR", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{title}: expected ERROR status, but got '{summary.Status}'.");
        }

        VisionToolResultStatus expectedStatus = ResolveExpectedResultStatus(expectedError);
        if (!string.Equals(summary.ResultStatus, expectedStatus.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"{title}: expected ResultStatus {expectedStatus}, but got '{summary.ResultStatus}'. Error={summary.ErrorCode}:{summary.ErrorName}.");
        }

        return $"{title}: ERROR {summary.ErrorCode}:{summary.ErrorName} | {summary.ResultStatus} | {summary.Message}";
    }

    private static VisionToolResultStatus ResolveExpectedResultStatus(VisionToolErrorCode errorCode)
    {
        return errorCode switch
        {
            VisionToolErrorCode.None => VisionToolResultStatus.Passed,
            VisionToolErrorCode.Unknown => VisionToolResultStatus.Failed,
            VisionToolErrorCode.InputImageInvalid => VisionToolResultStatus.InvalidInput,
            VisionToolErrorCode.InputLayerMissing => VisionToolResultStatus.InvalidInput,
            VisionToolErrorCode.InvalidParameter => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.ThresholdInvalidRange => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.ThresholdInvalidMaxValue => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.ThresholdInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MorphologyInvalidKernel => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MorphologyInvalidIterations => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.FilterInvalidKernel => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.FilterInvalidSigma => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.EdgeDetectionInvalidThreshold => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.EdgeDetectionInvalidKernel => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.EdgeDetectionInvalidDerivative => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.ContourInvalidAreaRange => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.ContourInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.BlobInvalidAreaRange => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.BlobInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.LineGaugeInvalidSampling => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.LineGaugeInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MeanInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MatchingInvalidScale => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MatchingInvalidAngleStep => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.MatchingInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.FeatureInvalidAdaptiveBlockSize => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.RotateScaleInvalidScale => VisionToolResultStatus.InvalidParameter,
            VisionToolErrorCode.InvalidRoi => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.ContourRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.BlobRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.MatchingRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.LineGaugeRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.MeanRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.FeatureRoiInvalid => VisionToolResultStatus.InvalidRoi,
            VisionToolErrorCode.ToolPropertyMissing => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.TemplateImageMissing => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.TemplateImageInvalid => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.ToolFactoryFailed => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.MatchingTemplateMissing => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.MatchingTemplateInvalid => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.FeatureTemplateMissing => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.FeatureTemplateInvalid => VisionToolResultStatus.ConfigurationError,
            VisionToolErrorCode.StepTimeout => VisionToolResultStatus.Timeout,
            VisionToolErrorCode.StepCanceled => VisionToolResultStatus.Canceled,
            VisionToolErrorCode.ToolExecutionException => VisionToolResultStatus.Exception,
            VisionToolErrorCode.OpenCvExecutionFailed => VisionToolResultStatus.Exception,
            VisionToolErrorCode.BlobLabelingFailed => VisionToolResultStatus.Exception,
            _ => VisionToolResultStatus.Failed
        };
    }

    private static CaptureDiagnostics CaptureToolResultStatusContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        List<string> lines = new()
        {
            "Tool Result Status Contract Check",
            "Status: CHECK OK",
            ""
        };

        foreach (VisionToolErrorCode errorCode in Enum.GetValues(typeof(VisionToolErrorCode)))
        {
            VisionToolResultStatus expectedStatus = ResolveExpectedResultStatus(errorCode);
            VisionToolResultStatus resolvedStatus = VisionToolResult.ResolveStatus(errorCode);
            if (resolvedStatus != expectedStatus)
            {
                throw new InvalidOperationException(
                    $"{errorCode}: expected ResolveStatus={expectedStatus}, but got {resolvedStatus}.");
            }

            if (errorCode != VisionToolErrorCode.None)
            {
                VisionToolResult failed = VisionToolResult.Failed(errorCode, "status contract", TimeSpan.Zero);
                if (failed.ErrorCode != errorCode)
                {
                    throw new InvalidOperationException(
                        $"{errorCode}: failed result changed ErrorCode to {failed.ErrorCode}.");
                }

                if (failed.ResultStatus != expectedStatus)
                {
                    throw new InvalidOperationException(
                        $"{errorCode}: expected Failed ResultStatus={expectedStatus}, but got {failed.ResultStatus}.");
                }

                if (failed.Success)
                {
                    throw new InvalidOperationException($"{errorCode}: failed result returned Success=true.");
                }
            }

            lines.Add($"{(int)errorCode,4} | {errorCode,-42} -> {expectedStatus}");
        }

        VisionToolResult noneFailure = VisionToolResult.Failed(VisionToolErrorCode.None, "none should normalize", TimeSpan.Zero);
        if (noneFailure.ErrorCode != VisionToolErrorCode.Unknown
            || noneFailure.ResultStatus != VisionToolResultStatus.Failed
            || noneFailure.Success)
        {
            throw new InvalidOperationException(
                $"Failed(None) must normalize to Unknown/Failed, but got {noneFailure.ErrorCode}/{noneFailure.ResultStatus}/Success={noneFailure.Success}.");
        }

        lines.Add("");
        lines.Add("Failed(None) normalization: OK");
        lines.Add("");
        lines.AddRange(ValidateDiagnosticCoverage());

        using Form report = CreateSmokeReportForm("Tool Result Status Contract Check", lines);
        return CaptureForm(report, outputPath, new Size(980, 680), 8);
    }

    private static IEnumerable<string> ValidateDiagnosticCoverage()
    {
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type diagnosticType = appAssembly.GetType("OpenVisionLab.VisionPipelineStepDiagnosticService", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineStepDiagnosticService type was not found.");
        MethodInfo resolveHintMethod = diagnosticType.GetMethod("ResolveDiagnosticHint", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ResolveDiagnosticHint was not found.");
        MethodInfo resolveFixMethod = diagnosticType.GetMethod("ResolveSuggestedFix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ResolveSuggestedFix was not found.");

        int checkedCount = 0;
        foreach (VisionToolErrorCode errorCode in Enum.GetValues(typeof(VisionToolErrorCode)))
        {
            if (errorCode == VisionToolErrorCode.None)
            {
                continue;
            }

            VisionPipelineStepResult stepResult = CreateDiagnosticCoverageStepResult(errorCode);
            string resolvedMessage = stepResult.ToolResult?.Message ?? string.Empty;
            string hint = resolveHintMethod.Invoke(null, new object[] { stepResult, resolvedMessage })?.ToString() ?? string.Empty;
            string fix = resolveFixMethod.Invoke(null, new object[] { stepResult, resolvedMessage })?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hint))
            {
                throw new InvalidOperationException($"{errorCode}: diagnostic hint is empty.");
            }

            if (string.IsNullOrWhiteSpace(fix))
            {
                throw new InvalidOperationException($"{errorCode}: suggested fix is empty.");
            }

            checkedCount++;
        }

        return new[]
        {
            $"Diagnostic coverage: OK | ErrorCodes={checkedCount}",
            "Every non-None ErrorCode returns non-empty Hint/Fix text."
        };
    }

    private static VisionPipelineStepResult CreateDiagnosticCoverageStepResult(VisionToolErrorCode errorCode)
    {
        VisionPipelineStep step = new()
        {
            Name = $"Diagnostic {errorCode}",
            ToolType = ResolveDiagnosticToolType(errorCode),
            InputLayer = "Main",
            OutputLayer = "DiagnosticOut"
        };
        VisionToolResult toolResult = VisionToolResult.Failed(errorCode, $"Synthetic failure for {errorCode}.", TimeSpan.Zero);
        return new VisionPipelineStepResult
        {
            Step = step,
            ToolResult = toolResult,
            AcceptancePassed = false
        };
    }

    private static string ResolveDiagnosticToolType(VisionToolErrorCode errorCode)
    {
        string name = errorCode.ToString();
        if (name.StartsWith("Threshold", StringComparison.OrdinalIgnoreCase)) { return "Threshold"; }
        if (name.StartsWith("Morphology", StringComparison.OrdinalIgnoreCase)) { return "Morphology"; }
        if (name.StartsWith("Filter", StringComparison.OrdinalIgnoreCase)) { return "Filter"; }
        if (name.StartsWith("EdgeDetection", StringComparison.OrdinalIgnoreCase)) { return "EdgeDetection"; }
        if (name.StartsWith("Contour", StringComparison.OrdinalIgnoreCase)) { return "Contour"; }
        if (name.StartsWith("Blob", StringComparison.OrdinalIgnoreCase)) { return "Blob"; }
        if (name.StartsWith("Matching", StringComparison.OrdinalIgnoreCase)) { return "Matching"; }
        if (name.StartsWith("LineGauge", StringComparison.OrdinalIgnoreCase)) { return "LineGauge"; }
        if (name.StartsWith("Mean", StringComparison.OrdinalIgnoreCase)) { return "Mean"; }
        if (name.StartsWith("Feature", StringComparison.OrdinalIgnoreCase)) { return "FeatureMatching"; }
        if (name.StartsWith("RotateScale", StringComparison.OrdinalIgnoreCase)) { return "RotateScale"; }
        if (name.Contains("Template", StringComparison.OrdinalIgnoreCase)) { return "Matching"; }
        return "Contour";
    }

    private static CaptureDiagnostics CaptureVisionRecipeRunnerApiContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        object sample = LoadRunnableCatalogSamples()
            .FirstOrDefault(item => string.Equals(GetStringProperty(item, "SampleName"), "Contour_TextSymbols", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Contour_TextSymbols sample was not found.");

        using VisionRecipeRunResult result = RunCatalogSample(sample);
        if (!result.Success || !string.Equals(result.OutcomeText, "OK", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Runner API expected OK. Success={result.Success}, Outcome={result.OutcomeText}, Message={result.Message}");
        }

        if (result.FinalStepSummary == null)
        {
            throw new InvalidOperationException("Runner API did not expose FinalStepSummary.");
        }

        if (!result.HasFinalResultImage || result.ResultImageWidth <= 0 || result.ResultImageHeight <= 0)
        {
            throw new InvalidOperationException("Runner API did not expose a final result image summary.");
        }

        if (result.FinalMetricCount <= 0 || !result.FinalMetricsText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Runner API final metric summary is missing ResultCount.");
        }

        if (result.FinalOverlayCount <= 0)
        {
            throw new InvalidOperationException("Runner API final overlay count should be greater than zero.");
        }

        if (result.HasFailedStep
            || result.FirstFailedStepIndex != 0
            || result.FirstFailedErrorCode != 0
            || !string.IsNullOrWhiteSpace(result.FirstFailedErrorName))
        {
            throw new InvalidOperationException("Runner API reported a failed step for a successful recipe.");
        }

        if (!result.SummaryText.Contains("OK", StringComparison.OrdinalIgnoreCase)
            || !result.SummaryText.Contains(result.FinalLayer, StringComparison.OrdinalIgnoreCase)
            || !result.SummaryText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Runner API summary text does not include outcome, final layer, and metric context.");
        }

        if (!result.ActionSummaryText.Contains("Preview OK", StringComparison.OrdinalIgnoreCase)
            || !result.ActionSummaryText.Contains(result.FinalLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Runner API action summary should direct successful callers to the final layer.");
        }

        if (!result.StepSummaryText.Contains("01 OK", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("Main->", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("TextSymbol_Contour", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Runner API step summary should include ordered status and layer flow.");
        }

        if (!string.Equals(result.FirstFailedSummaryText, "None", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Runner API success should expose FirstFailedSummaryText=None, actual={result.FirstFailedSummaryText}");
        }

        string boundsAcceptanceText = RunBoundsMetricAcceptanceContractCase();
        using VisionRecipeRunResult failedResult = RunRunnerFailureSummaryCase();
        string[] lines =
        {
            "VisionRecipeRunner API Contract Check",
            "Status: CHECK OK",
            "",
            $"Outcome: {result.OutcomeText}",
            $"Summary: {result.SummaryText}",
            $"Action: {result.ActionSummaryText}",
            $"Step summary: {result.StepSummaryText}",
            $"Pipeline: {result.PipelineName}",
            $"Final layer: {result.FinalLayer}",
            $"Final step: {result.FinalStepSummary.Index:00} {result.FinalStepSummary.Name} [{result.FinalStepSummary.ToolType}]",
            $"Result image: {result.ResultImageSizeText}",
            $"Final metrics: {result.FinalMetricsText}",
            $"Final overlays: {result.FinalOverlayCount}",
            $"Steps: {result.PassedStepCount}/{result.StepCount}",
            boundsAcceptanceText,
            "",
            $"Failure outcome: {failedResult.OutcomeText}",
            $"Failure summary: {failedResult.FirstFailedSummaryText}",
            $"Failure action: {failedResult.ActionSummaryText}"
        };

        using Form report = CreateSmokeReportForm("VisionRecipeRunner API Contract Check", lines);
        return CaptureForm(report, outputPath, new Size(940, 500), 8);
    }

    private static string RunBoundsMetricAcceptanceContractCase()
    {
        VisionRecipeRunResult goodResult = null;
        VisionRecipeRunResult badResult = null;
        VisionRecipeRunResult mismatchResult = null;
        try
        {
            goodResult = RunBentPinShaftWithBoundsAcceptance("BentPin_GoodShaft", 0, 18);
            badResult = RunBentPinShaftWithBoundsAcceptance("BentPin_BadShaft", 24, 40);
            mismatchResult = RunBentPinShaftWithBoundsAcceptance("BentPin_BadShaft", 0, 18, requireSuccess: false);

            VisionRecipeStepRunSummary goodStep = FindRequiredStep(goodResult, "03 Pin Shaft Contour");
            VisionRecipeStepRunSummary badStep = FindRequiredStep(badResult, "03 Pin Shaft Contour");
            ValidateMetricRange(goodStep, "BoundsWidthMax", 0, 18);
            ValidateMetricRange(goodStep, "BoundsWidthMmMax", 0, 0.108);
            ValidateMetricRange(badStep, "BoundsWidthMax", 24, 40);
            ValidateMetricRange(badStep, "BoundsWidthMmMax", 0.144, 0.24);

            if (mismatchResult.Success || !mismatchResult.HasFailedStep)
            {
                throw new InvalidOperationException("Bounds metric mismatch case should fail acceptance.");
            }

            if (!mismatchResult.FirstFailedSummaryText.Contains("BoundsWidthMax", StringComparison.OrdinalIgnoreCase)
                && !mismatchResult.FirstFailedSummaryText.Contains("Bounds Width Max", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Bounds metric mismatch did not report the failing metric.");
            }

            return $"Bounds acceptance: OK | GoodMax={goodStep.Metrics["BoundsWidthMax"]:0.#}px/{goodStep.Metrics["BoundsWidthMmMax"]:0.###}mm, BadMax={badStep.Metrics["BoundsWidthMax"]:0.#}px/{badStep.Metrics["BoundsWidthMmMax"]:0.###}mm, Mismatch=NG";
        }
        finally
        {
            goodResult?.Dispose();
            badResult?.Dispose();
            mismatchResult?.Dispose();
        }
    }

    private static VisionRecipeRunResult RunBentPinShaftWithBoundsAcceptance(
        string sampleName,
        double minimum,
        double maximum,
        bool requireSuccess = true)
    {
        object sample = LoadRunnableCatalogSamples()
            .FirstOrDefault(item => string.Equals(GetStringProperty(item, "SampleName"), sampleName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"{sampleName} sample was not found.");

        string pipelinePath = GetStringProperty(sample, "PipelineFullPath");
        if (!SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline) || pipeline == null)
        {
            throw new InvalidOperationException($"{sampleName} pipeline XML could not be loaded.");
        }

        VisionPipelineStep step = pipeline.Steps
            .FirstOrDefault(item => string.Equals(item?.Name, "03 Pin Shaft Contour", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"{sampleName} final contour step was not found.");
        step.UseAcceptance = true;
        step.ExpectedSuccess = true;
        step.AcceptanceMetricName = "BoundsWidthMax";
        step.UseAcceptanceMetricMinimum = true;
        step.AcceptanceMetricMinimum = minimum;
        step.UseAcceptanceMetricMaximum = true;
        step.AcceptanceMetricMaximum = maximum;

        string imagePath = GetStringProperty(sample, "ImageFullPath");
        using Bitmap bitmap = new Bitmap(imagePath);
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (requireSuccess && !result.Success)
        {
            string message = result.Message;
            result.Dispose();
            throw new InvalidOperationException($"{sampleName} BoundsWidthMax acceptance expected OK. {message}");
        }

        return result;
    }

    private static VisionRecipeRunResult RunRunnerFailureSummaryCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_Runner_FailureSummary"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 Invalid Range",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "InvalidRange"
        };
        step.Parameters["Mode"] = "Range";
        step.Parameters["RangeMin"] = "220";
        step.Parameters["RangeMax"] = "30";
        step.Parameters["MaxValue"] = "255";
        step.Parameters["ThresholdType"] = "Binary";
        pipeline.Steps.Add(step);

        using Bitmap bitmap = CreatePipelinePreviewRunImage();
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", 1000, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (result.Success || !string.Equals(result.OutcomeText, "NG", StringComparison.Ordinal))
        {
            result.Dispose();
            throw new InvalidOperationException($"Runner API failure case expected NG. Success={result.Success}, Outcome={result.OutcomeText}");
        }

        if (!result.HasFailedStep || result.FirstFailedStepIndex != 1)
        {
            result.Dispose();
            throw new InvalidOperationException("Runner API failure case did not expose the first failed step.");
        }

        if (result.FirstFailedErrorCode != (int)VisionToolErrorCode.ThresholdInvalidRange
            || !string.Equals(result.FirstFailedResultStatus, VisionToolResultStatus.InvalidParameter.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            result.Dispose();
            throw new InvalidOperationException(
                $"Runner API failure case returned unexpected failure. {result.FirstFailedSummaryText}");
        }

        if (!result.FirstFailedSummaryText.Contains("ThresholdInvalidRange", StringComparison.OrdinalIgnoreCase)
            || !result.FirstFailedSummaryText.Contains("InvalidParameter", StringComparison.OrdinalIgnoreCase)
            || !result.SummaryText.Contains("Failed=1", StringComparison.OrdinalIgnoreCase))
        {
            result.Dispose();
            throw new InvalidOperationException("Runner API failure summary text is not actionable enough.");
        }

        if (!result.FirstFailedDiagnosticHint.Contains("range", StringComparison.OrdinalIgnoreCase)
            || !result.FirstFailedSuggestedFix.Contains("RangeMin", StringComparison.OrdinalIgnoreCase)
            || !result.FirstFailedSuggestedFix.Contains("RangeMax", StringComparison.OrdinalIgnoreCase))
        {
            result.Dispose();
            throw new InvalidOperationException(
                "Runner API failure diagnostic should explain the invalid threshold range and direct the user to RangeMin/RangeMax.");
        }

        if (!result.ActionSummaryText.Contains("Fix step 01", StringComparison.OrdinalIgnoreCase)
            || !result.ActionSummaryText.Contains("RangeMin", StringComparison.OrdinalIgnoreCase)
            || !result.ActionSummaryText.Contains("RangeMax", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("01 ERROR", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("ThresholdInvalidRange", StringComparison.OrdinalIgnoreCase))
        {
            result.Dispose();
            throw new InvalidOperationException("Runner API failure action/step summaries should be directly actionable.");
        }

        return result;
    }

    private static CaptureDiagnostics CapturePipelineSampleCatalogCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type catalogItemType = appAssembly.GetType("OpenVisionLab.VisionPipelineSampleCatalogItem", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem type was not found.");
        MethodInfo loadMethod = catalogItemType.GetMethod("LoadRunnable", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem.LoadRunnable was not found.");
        object? loaded = loadMethod.Invoke(null, null);
        System.Collections.IEnumerable samples = loaded as System.Collections.IEnumerable
            ?? throw new InvalidOperationException("Sample catalog did not return a list.");

        List<string> errors = new();
        List<string> lines = new()
        {
            "Pipeline Sample Catalog Check",
            "Status: CHECK OK"
        };

        int sampleCount = 0;
        int warningCount = 0;
        foreach (object sample in samples)
        {
            sampleCount++;
            string name = GetStringProperty(sample, "SampleName");
            string imagePath = GetStringProperty(sample, "ImageFullPath");
            string pipelinePath = GetStringProperty(sample, "PipelineFullPath");
            string referenceImagePath = GetStringProperty(sample, "ReferenceImageFullPath");
            string referenceImageRelativePath = GetStringProperty(sample, "ReferenceImagePath");
            int expectedWidth = GetIntProperty(sample, "Width");
            int expectedHeight = GetIntProperty(sample, "Height");

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Catalog row has no sample name.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                errors.Add($"{name}: image missing. {imagePath}");
                continue;
            }

            if (expectedWidth > 0 && expectedHeight > 0)
            {
                using Image image = Image.FromFile(imagePath);
                if (image.Width != expectedWidth || image.Height != expectedHeight)
                {
                    errors.Add($"{name}: image size {image.Width}x{image.Height}, catalog expects {expectedWidth}x{expectedHeight}.");
                }
            }

            if (!string.IsNullOrWhiteSpace(referenceImageRelativePath))
            {
                if (string.IsNullOrWhiteSpace(referenceImagePath) || !File.Exists(referenceImagePath))
                {
                    errors.Add($"{name}: reference image missing. {referenceImageRelativePath}");
                    continue;
                }

                using Image referenceImage = Image.FromFile(referenceImagePath);
                if (referenceImage.Width <= 0 || referenceImage.Height <= 0)
                {
                    errors.Add($"{name}: reference image is invalid. {referenceImageRelativePath}");
                }
            }

            if (string.IsNullOrWhiteSpace(pipelinePath) || !File.Exists(pipelinePath))
            {
                errors.Add($"{name}: pipeline missing. {pipelinePath}");
                continue;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline) || pipeline == null)
            {
                errors.Add($"{name}: pipeline XML could not be loaded. {pipelinePath}");
                continue;
            }

            if (!ValidatePipelineForSmoke(appAssembly, pipeline, out string validationErrors, out int validationWarningCount))
            {
                errors.Add($"{name}: pipeline validation failed. {validationErrors}");
                continue;
            }

            warningCount += validationWarningCount;
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Sample catalog check failed. " + string.Join(" | ", errors.Take(5)));
        }

        lines.Add($"Runnable samples: {sampleCount}");
        lines.Add($"Validation warnings: {warningCount}");
        lines.Add("");
        lines.Add("Images: exists and size matches catalog.");
        lines.Add("Reference images: optional, but existing paths must load.");
        lines.Add("Pipelines: XML loads and validator has no errors.");

        using Form report = CreateSmokeReportForm("Pipeline Sample Catalog Check", lines);
        return CaptureForm(report, outputPath, new Size(840, 420), 8);
    }

    private static CaptureDiagnostics CaptureSampleInventoryContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        string repoRoot = ResolveRepoRootForSmoke();
        string sampleRoot = Path.Combine(repoRoot, "Sample");
        if (!Directory.Exists(sampleRoot))
        {
            throw new InvalidOperationException("Sample directory was not found: " + sampleRoot);
        }

        string[] imageExtensions =
        {
            ".bmp",
            ".jpg",
            ".jpeg",
            ".png",
            ".tif",
            ".tiff"
        };

        List<string> imageFiles = Directory
            .EnumerateFiles(sampleRoot, "*.*", SearchOption.AllDirectories)
            .Where(file => imageExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            .Select(Path.GetFullPath)
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        List<object> runnableSamples = LoadRunnableCatalogSamples().ToList();
        List<string> catalogRelativeImages = runnableSamples
            .Select(sample => GetRepoRelativePath(repoRoot, GetStringProperty(sample, "ImageFullPath")))
            .Where(path => path.StartsWith("Sample\\", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
        Dictionary<string, int> catalogRowsByFolder = catalogRelativeImages
            .Select(GetSampleTopFolderFromCatalogPath)
            .Where(folder => !string.IsNullOrWhiteSpace(folder))
            .GroupBy(folder => folder, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        string docsSampleRoot = Path.Combine(repoRoot, "docs", "samples");
        List<string> recipeFiles = Directory
            .EnumerateFiles(docsSampleRoot, "*.pipeline.xml", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        HashSet<string> catalogRecipeFiles = runnableSamples
            .Select(sample => Path.GetFileName(GetStringProperty(sample, "PipelineFullPath")))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        HashSet<string> allowedUncatalogedRecipes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Filter_Edge_Line.pipeline.xml"
        };

        List<string> errors = new();
        if (imageFiles.Count < 100)
        {
            errors.Add($"Recursive sample inventory is unexpectedly small. Images={imageFiles.Count}.");
        }

        string[] requiredFolders =
        {
            "EasyImage",
            "EasyObject",
            "EasyGauge",
            "EasyMatch",
            "EasyColor",
            "EasyFind",
            "EasyBarCode",
            "EasyOcr",
            "EasyQRCode"
        };

        HashSet<string> topFolders = imageFiles
            .Select(file => GetSampleTopFolder(sampleRoot, file))
            .Where(folder => !string.IsNullOrWhiteSpace(folder))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string folder in requiredFolders)
        {
            if (!topFolders.Contains(folder))
            {
                errors.Add($"Representative sample folder is missing: Sample\\{folder}");
            }
        }

        string[] requiredRepresentativeSamples =
        {
            "EasyImage_Text_Contour",
            "EasyGauge_Pins_LineGauge",
            "EasyGauge_BentPin_Large",
            "EasyMatch_DiePad1_Surface",
            "EasyObject_Rice_Particle",
            "EasyObject_SurfaceDefect1_Edge",
            "EasyObject_SurfaceDefect2_Edge",
            "EasyColor_ColorDots_Contour",
            "EasyFind_Fiducial_Contour",
            "EasyBarCode_Code39_Contour",
            "EasyQRCode_QR1_Contour",
            "EasyOcr_Characters_Contour"
        };

        foreach (string sampleName in requiredRepresentativeSamples)
        {
            if (!runnableSamples.Any(sample => string.Equals(GetStringProperty(sample, "SampleName"), sampleName, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Recursive representative catalog sample is missing: {sampleName}");
            }
        }

        int subfolderCatalogCount = catalogRelativeImages.Count(path =>
        {
            string relativeFromSample = path.Substring("Sample\\".Length);
            return relativeFromSample.Contains("\\", StringComparison.Ordinal);
        });
        if (subfolderCatalogCount < requiredRepresentativeSamples.Length)
        {
            errors.Add($"Catalog should include recursive representative samples. Subfolder catalog images={subfolderCatalogCount}.");
        }

        foreach (string recipeFile in recipeFiles)
        {
            if (!catalogRecipeFiles.Contains(recipeFile)
                && !allowedUncatalogedRecipes.Contains(recipeFile))
            {
                errors.Add($"Recipe XML is not covered by the sample catalog: docs\\samples\\{recipeFile}");
            }
        }

        foreach (string allowedRecipe in allowedUncatalogedRecipes)
        {
            if (!recipeFiles.Contains(allowedRecipe, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Allowed uncataloged recipe is missing: docs\\samples\\{allowedRecipe}");
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Sample inventory contract failed. " + string.Join(" | ", errors.Take(5)));
        }

        List<string> lines = new()
        {
            "Sample Inventory Contract Check",
            "Status: CHECK OK",
            "",
            $"Recursive image files: {imageFiles.Count}",
            $"Runnable catalog rows: {runnableSamples.Count}",
            $"Catalog rows using Sample subfolders: {subfolderCatalogCount}",
            $"Recipe XML files: {recipeFiles.Count}",
            $"Catalog recipe files: {catalogRecipeFiles.Count}",
            "",
            "Representative folders:"
        };

        foreach (IGrouping<string, string> group in imageFiles
            .GroupBy(file => GetSampleTopFolder(sampleRoot, file), StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            catalogRowsByFolder.TryGetValue(group.Key, out int catalogRefs);
            string coverageStatus = catalogRefs > 0 ? "covered" : "backlog";
            lines.Add($"- {group.Key}: {group.Count()} image(s), catalog refs={catalogRefs} [{coverageStatus}]");
        }

        List<string> uncoveredFolders = imageFiles
            .GroupBy(file => GetSampleTopFolder(sampleRoot, file), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Key)
            .Where(folder => !string.Equals(folder, ".", StringComparison.OrdinalIgnoreCase)
                && (!catalogRowsByFolder.TryGetValue(folder, out int catalogRefs) || catalogRefs == 0))
            .OrderBy(folder => folder, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (uncoveredFolders.Count > 0)
        {
            lines.Add("");
            lines.Add("Sample folder coverage backlog:");
            foreach (string folder in uncoveredFolders)
            {
                lines.Add($"- {folder}: no runnable catalog representative yet");
            }
        }

        lines.Add("");
        lines.Add("Recursive representative catalog samples:");
        foreach (string sampleName in requiredRepresentativeSamples)
        {
            object sample = runnableSamples.First(item => string.Equals(GetStringProperty(item, "SampleName"), sampleName, StringComparison.OrdinalIgnoreCase));
            lines.Add($"- {sampleName}: {GetRepoRelativePath(repoRoot, GetStringProperty(sample, "ImageFullPath"))}");
        }

        lines.Add("");
        lines.Add("Uncataloged recipe exceptions:");
        foreach (string allowedRecipe in allowedUncatalogedRecipes.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            lines.Add($"- {allowedRecipe}: template/example recipe, not a benchmark row");
        }

        using Form report = CreateSmokeReportForm("Sample Inventory Contract Check", lines);
        return CaptureForm(report, outputPath, new Size(980, 640), 8);
    }

    private static CaptureDiagnostics CapturePipelineSampleCatalogRunCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        List<object> samples = LoadRunnableCatalogSamples().ToList();
        List<string> errors = new();
        List<string> lines = new()
        {
            "Pipeline Sample Runtime Check",
            "Status: CHECK OK"
        };

        Stopwatch totalStopwatch = Stopwatch.StartNew();
        foreach (object sample in samples)
        {
            try
            {
                lines.Add(RunSampleCatalogCase(sample));
            }
            catch (Exception ex)
            {
                string name = GetStringProperty(sample, "SampleName");
                errors.Add($"{name}: {ex.GetBaseException().Message}");
            }
        }

        totalStopwatch.Stop();
        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Sample runtime check failed. " + string.Join(" | ", errors.Take(5)));
        }

        lines.Insert(2, $"Runnable samples: {samples.Count}");
        lines.Insert(3, $"Total time: {totalStopwatch.Elapsed.TotalMilliseconds:0.0} ms");
        lines.Insert(4, "");

        using Form report = CreateSmokeReportForm("Pipeline Sample Runtime Check", lines);
        return CaptureForm(report, outputPath, new Size(980, 560), 8);
    }

    private static CaptureDiagnostics CaptureAlgorithmSampleContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        List<object> samples = LoadRunnableCatalogSamples().ToList();
        List<string> errors = new();
        List<string> lines = new()
        {
            "Algorithm Sample Contract Check",
            "Status: CHECK OK",
            ""
        };

        AddAlgorithmContractLine(samples, lines, errors, "Contour_TextSymbols", ValidateContourTextSymbolsSample);
        AddAlgorithmContractLine(samples, lines, errors, "Contour_MeanBrightness", ValidateMeanBrightnessSample);
        AddAlgorithmContractLine(samples, lines, errors, "Contour_RotateScale_Resize", ValidateRotateScaleResizeSample);
        AddAlgorithmContractLine(samples, lines, errors, "Rice_Particle", ValidateRiceParticleSample);
        AddAlgorithmContractLine(samples, lines, errors, "Rice_Particle_Blob", ValidateRiceParticleBlobSample);
        AddAlgorithmContractLine(samples, lines, errors, "Pins_Feature", ValidatePinFeatureSample);
        AddAlgorithmContractLine(samples, lines, errors, "BentPin_Large", ValidateBentPinLargeSample);
        AddAlgorithmContractLine(samples, lines, errors, "BentPin_TopBottom_Overlay", ValidateBentPinTopBottomOverlaySample);
        AddAlgorithmContractLine(samples, lines, errors, "BentPin_GoodShaft", ValidateBentPinGoodShaftSample);
        AddAlgorithmContractLine(samples, lines, errors, "BentPin_BadShaft", ValidateBentPinBadShaftSample);
        AddAlgorithmContractLine(samples, lines, errors, "DiePad1_Surface", ValidateDiePadSurfaceSample);
        AddAlgorithmContractLine(samples, lines, errors, "DiePad2_Surface", ValidateDiePadSurfaceSample);
        AddAlgorithmContractLine(samples, lines, errors, "DiePad3_Surface", ValidateDiePadSurfaceSample);
        AddAlgorithmContractLine(samples, lines, errors, "DiePad4_Surface", ValidateDiePadSurfaceSample);
        AddAlgorithmContractLine(samples, lines, errors, "Pins_LineGauge", ValidatePinsLineGaugeSample);
        AddAlgorithmContractLine(samples, lines, errors, "Contour_TemplateMatching", ValidateContourTemplateMatchingSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyImage_Text_Contour", ValidateEasyImageTextContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyObject_SurfaceDefect1_Edge", ValidateSurfaceDefectEdgeSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyObject_SurfaceDefect2_Edge", ValidateSurfaceDefectEdgeSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyColor_ColorDots_Contour", ValidateGenericThresholdContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyFind_Fiducial_Contour", ValidateGenericThresholdContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyBarCode_Code39_Contour", ValidateGenericThresholdContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyQRCode_QR1_Contour", ValidateGenericThresholdContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "EasyOcr_Characters_Contour", ValidateGenericThresholdContourSample);
        AddAlgorithmContractLine(samples, lines, errors, "Contour_AllSymbolsAndFaint_LLM", ValidateLlmContourRecipeSample);
        AddLegacyCvSummary(lines);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Algorithm sample contract check failed. " + string.Join(" | ", errors.Take(5)));
        }

        using Form report = CreateSmokeReportForm("Algorithm Sample Contract Check", lines);
        return CaptureForm(report, outputPath, new Size(1080, 620), 8);
    }

    private static void AddAlgorithmContractLine(
        List<object> samples,
        List<string> lines,
        List<string> errors,
        string sampleName,
        Func<object, string> validate)
    {
        object? sample = samples.FirstOrDefault(item =>
            string.Equals(GetStringProperty(item, "SampleName"), sampleName, StringComparison.OrdinalIgnoreCase));
        if (sample == null)
        {
            errors.Add($"{sampleName}: sample catalog row was not found.");
            return;
        }

        try
        {
            lines.Add(validate(sample));
        }
        catch (Exception ex)
        {
            errors.Add($"{sampleName}: {ex.GetBaseException().Message}");
        }
    }

    private static string ValidateContourTextSymbolsSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary contourStep = FindRequiredStep(result, "03 Text Symbol Contour");
        ValidateMetricRange(contourStep, "ResultCount", 35, 80);
        ValidateResultStepImage(contourStep, 768, 576);
        ValidateInspectionOverlays(contourStep, "Contour_TextSymbols");
        ValidateFinalLayer(result, "TextSymbol_Contour");
        ValidateStepFlow(result, "01 Text Symbol Binary", "Main", "TextSymbol_Binary");
        ValidateStepFlow(result, "02 Text Symbol Clean", "TextSymbol_Binary", "TextSymbol_Clean");
        ValidateStepFlow(result, "03 Text Symbol Contour", "TextSymbol_Clean", "TextSymbol_Contour");
        double count = contourStep.Metrics["ResultCount"];
        return $"Contour_TextSymbols: OK | ResultCount={count:0}, Overlay={contourStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static string ValidateMeanBrightnessSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary meanStep = FindRequiredStep(result, "01 Mean Brightness");
        ValidateMetricRange(meanStep, "MeanValueAvg", 250, 256);
        ValidateResultStepImage(meanStep, 768, 576);
        ValidateFinalLayer(result, "Mean_Result");
        ValidateStepFlow(result, "01 Mean Brightness", "Main", "Mean_Result");
        ValidateSuccessfulStepDiagnostics(meanStep);

        return $"Contour_MeanBrightness: OK | MeanValueAvg={meanStep.Metrics["MeanValueAvg"]:0.#}, Final={result.FinalLayer}";
    }

    private static string ValidateRotateScaleResizeSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary resizeStep = FindRequiredStep(result, "01 Resize Half");
        ValidateMetricRange(resizeStep, "ResultImageWidth", 384, 384);
        ValidateMetricRange(resizeStep, "ResultImageHeight", 288, 288);
        ValidateResultStepImage(resizeStep, 384, 288);
        ValidateFinalLayer(result, "ResizeHalf_Result");
        ValidateStepFlow(result, "01 Resize Half", "Main", "ResizeHalf_Result");
        ValidateSuccessfulStepDiagnostics(resizeStep);

        return $"Contour_RotateScale_Resize: OK | Result={resizeStep.ResultImageSizeText}, Final={result.FinalLayer}";
    }

    private static string ValidateRiceParticleSample(object sample)
    {
        return ValidateSimpleContourSample(
            sample,
            "03 Rice Particle Contour",
            "Rice_Contour",
            100,
            170,
            new[]
            {
                "01 Rice Particle Binary|Main|Rice_Binary",
                "02 Rice Particle Clean|Rice_Binary|Rice_Clean",
                "03 Rice Particle Contour|Rice_Clean|Rice_Contour"
            });
    }

    private static string ValidateRiceParticleBlobSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary blobStep = FindRequiredStep(result, "03 Rice Particle Blob");
        ValidateMetricRange(blobStep, "ResultCount", 120, 170);
        ValidateMetricRange(blobStep, "AreaAvg", 250, 400);
        ValidateMetricRange(blobStep, "BoundsWidthAvg", 15, 35);
        ValidateCatalogImageSize(sample, blobStep);
        ValidateInspectionOverlays(blobStep, "Rice_Particle_Blob");
        ValidateOverlayCountTracksResult(blobStep, "Rice_Particle_Blob", 0.95);
        ValidateSuccessfulStepDiagnostics(blobStep);
        ValidateFinalLayer(result, "Rice_Blob");
        ValidateStepFlow(result, "01 Rice Particle Binary", "Main", "Rice_Binary");
        ValidateStepFlow(result, "02 Rice Particle Clean", "Rice_Binary", "Rice_Clean");
        ValidateStepFlow(result, "03 Rice Particle Blob", "Rice_Clean", "Rice_Blob");

        return $"Rice_Particle_Blob: OK | ResultCount={blobStep.Metrics["ResultCount"]:0}, AreaAvg={blobStep.Metrics["AreaAvg"]:0.#}, BoundsWidthAvg={blobStep.Metrics["BoundsWidthAvg"]:0.#}, Final={result.FinalLayer}";
    }

    private static string ValidatePinFeatureSample(object sample)
    {
        return ValidateSimpleContourSample(
            sample,
            "03 Pin Feature Contour",
            "Pin_Contour",
            40,
            70,
            new[]
            {
                "01 Pin Feature Binary|Main|Pin_Binary",
                "02 Pin Feature Close|Pin_Binary|Pin_Clean",
                "03 Pin Feature Contour|Pin_Clean|Pin_Contour"
            });
    }

    private static string ValidateBentPinLargeSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary finalStep = FindRequiredStep(result, "03 Bent Pin Large Contour");
        ValidateMetricRange(finalStep, "ResultCount", 1, 5);
        ValidateMetricRange(finalStep, "AreaMax", 100000, 300000);
        ValidateMetricRange(finalStep, "AreaAvg", 90000, 260000);
        ValidateCatalogImageSize(sample, finalStep);
        ValidateInspectionOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateOverlayCountTracksResult(finalStep, GetStringProperty(sample, "SampleName"), 0.8);
        ValidateLargeRectangleOverlay(finalStep, GetStringProperty(sample, "SampleName"), 120, 120);
        ValidateBentPinLargeGeometry(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateSuccessfulStepDiagnostics(finalStep);
        ValidateFinalLayer(result, "BentPin_Contour");
        ValidateStepFlow(result, "01 Bent Pin Binary", "Main", "BentPin_Binary");
        ValidateStepFlow(result, "02 Bent Pin Close", "BentPin_Binary", "BentPin_Clean");
        ValidateStepFlow(result, "03 Bent Pin Large Contour", "BentPin_Clean", "BentPin_Contour");

        return $"{GetStringProperty(sample, "SampleName")}: OK | ResultCount={finalStep.Metrics["ResultCount"]:0}, AreaMax={finalStep.Metrics["AreaMax"]:0.#}, AreaAvg={finalStep.Metrics["AreaAvg"]:0.#}, Geometry=SplitLarge, Overlay={finalStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static string ValidateBentPinTopBottomOverlaySample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary topStep = FindRequiredStep(result, "03 Bent Pin Top Region");
        VisionRecipeStepRunSummary bottomStep = FindRequiredStep(result, "04 Bent Pin Bottom Region");
        VisionRecipeStepRunSummary mergeStep = FindRequiredStep(result, "05 Merge Bent Pin Review");

        ValidateMetricRange(topStep, "ResultCount", 1, 1);
        ValidateMetricRange(bottomStep, "ResultCount", 1, 1);
        ValidateMetricExact(mergeStep, "MergeOverlayCount", 2);
        ValidateMetricExact(mergeStep, "MergeSourceCount", 2);
        ValidateCatalogImageSize(sample, topStep);
        ValidateCatalogImageSize(sample, bottomStep);
        ValidateCatalogImageSize(sample, mergeStep);
        ValidateInspectionOverlays(topStep, "BentPin_TopBottom_Overlay top");
        ValidateInspectionOverlays(bottomStep, "BentPin_TopBottom_Overlay bottom");
        ValidateInspectionOverlays(mergeStep, "BentPin_TopBottom_Overlay merge");
        ValidateBentPinSingleRegionGeometry(topStep, "top", upperRegion: true);
        ValidateBentPinSingleRegionGeometry(bottomStep, "bottom", upperRegion: false);
        ValidateBentPinLargeGeometry(mergeStep, "BentPin_TopBottom_Overlay merge");
        ValidateMergedOverlayIdentity(mergeStep, new[] { topStep, bottomStep }, 768, 576);
        ValidateSuccessfulStepDiagnostics(topStep);
        ValidateSuccessfulStepDiagnostics(bottomStep);
        ValidateSuccessfulStepDiagnostics(mergeStep);
        ValidateFinalLayer(result, "BentPin_Review");
        ValidateStepFlow(result, "01 Bent Pin Binary", "Main", "BentPin_Binary");
        ValidateStepFlow(result, "02 Bent Pin Close", "BentPin_Binary", "BentPin_Clean");
        ValidateStepFlow(result, "03 Bent Pin Top Region", "BentPin_Clean", "BentPin_TopContour");
        ValidateStepFlow(result, "04 Bent Pin Bottom Region", "BentPin_Clean", "BentPin_BottomContour");
        ValidateStepFlow(result, "05 Merge Bent Pin Review", "Main", "BentPin_Review");

        return $"BentPin_TopBottom_Overlay: OK | Top={topStep.Metrics["ResultCount"]:0}, Bottom={bottomStep.Metrics["ResultCount"]:0}, Merged={mergeStep.Metrics["MergeOverlayCount"]:0}, Final={result.FinalLayer}";
    }

    private static string ValidateBentPinGoodShaftSample(object sample)
    {
        return ValidateBentPinShaftSample(sample, expectedBent: false);
    }

    private static string ValidateBentPinBadShaftSample(object sample)
    {
        return ValidateBentPinShaftSample(sample, expectedBent: true);
    }

    private static string ValidateBentPinShaftSample(object sample, bool expectedBent)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary finalStep = FindRequiredStep(result, "03 Pin Shaft Contour");
        ValidateMetricRange(finalStep, "ResultCount", 13, 13);
        ValidateMetricRange(finalStep, "BoundsWidthMax", expectedBent ? 24 : 0, expectedBent ? 40 : 18);
        ValidateCatalogImageSize(sample, finalStep);
        ValidateInspectionOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateOverlayCountTracksResult(finalStep, GetStringProperty(sample, "SampleName"), 1.0);
        ValidateBentPinShaftGeometry(finalStep, GetStringProperty(sample, "SampleName"), expectedBent);
        ValidateSuccessfulStepDiagnostics(finalStep);
        ValidateFinalLayer(result, "PinShaft_Contour");
        ValidateStepFlow(result, "01 Pin Shaft Binary", "Main", "PinShaft_Binary");
        ValidateStepFlow(result, "02 Pin Shaft Clean", "PinShaft_Binary", "PinShaft_Clean");
        ValidateStepFlow(result, "03 Pin Shaft Contour", "PinShaft_Clean", "PinShaft_Contour");

        double maxWidth = finalStep.Overlays
            .Where(overlay => string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .Select(overlay => (double)overlay.BoundsWidth)
            .DefaultIfEmpty(0)
            .Max();
        return $"{GetStringProperty(sample, "SampleName")}: OK | ResultCount={finalStep.Metrics["ResultCount"]:0}, MaxWidth={maxWidth:0.#}, Final={result.FinalLayer}";
    }

    private static string ValidateDiePadSurfaceSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary finalStep = FindRequiredStep(result, "03 Die Pad Surface Contour");
        ValidateMetricRange(finalStep, "ResultCount", 8, 25);
        ValidateMetricRange(finalStep, "AreaMax", 45000, 90000);
        ValidateMetricRange(finalStep, "AreaAvg", 2500, 12000);
        ValidateCatalogImageSize(sample, finalStep);
        ValidateInspectionOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateOverlayCountTracksResult(finalStep, GetStringProperty(sample, "SampleName"), 0.8);
        ValidateDiePadSurfaceGeometry(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateSuccessfulStepDiagnostics(finalStep);
        ValidateFinalLayer(result, "DiePad_Contour");
        ValidateStepFlow(result, "01 Die Pad Binary", "Main", "DiePad_Binary");
        ValidateStepFlow(result, "02 Die Pad Close", "DiePad_Binary", "DiePad_Clean");
        ValidateStepFlow(result, "03 Die Pad Surface Contour", "DiePad_Clean", "DiePad_Contour");

        return $"{GetStringProperty(sample, "SampleName")}: OK | ResultCount={finalStep.Metrics["ResultCount"]:0}, AreaMax={finalStep.Metrics["AreaMax"]:0.#}, AreaAvg={finalStep.Metrics["AreaAvg"]:0.#}, Geometry=SurfaceFeatures, Overlay={finalStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static string ValidateSimpleContourSample(
        object sample,
        string finalStepName,
        string expectedFinalLayer,
        double minimumCount,
        double maximumCount,
        IEnumerable<string> expectedFlows)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary finalStep = FindRequiredStep(result, finalStepName);
        ValidateMetricRange(finalStep, "ResultCount", minimumCount, maximumCount);
        ValidateCatalogImageSize(sample, finalStep);
        ValidateInspectionOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateSuccessfulStepDiagnostics(finalStep);
        ValidateFinalLayer(result, expectedFinalLayer);

        foreach (string flow in expectedFlows ?? Enumerable.Empty<string>())
        {
            string[] parts = flow.Split('|');
            if (parts.Length == 3)
            {
                ValidateStepFlow(result, parts[0], parts[1], parts[2]);
            }
        }

        double count = finalStep.Metrics["ResultCount"];
        return $"{GetStringProperty(sample, "SampleName")}: OK | ResultCount={count:0}, Overlay={finalStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static string ValidatePinsLineGaugeSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary lineStep = FindRequiredStep(result, "03 Rail Line Gauge");
        ValidateMetricRange(lineStep, "EdgeCount", 30, 70);
        ValidateMetricRange(lineStep, "EdgeCountMin", 30, 70);
        ValidateMetricRange(lineStep, "EdgeCountMax", 30, 70);
        ValidateMetricRange(lineStep, "EdgePointCount", 30, 70);
        ValidateMetricRange(lineStep, "EdgePointCountMin", 30, 70);
        ValidateMetricRange(lineStep, "EdgePointCountMax", 30, 70);
        ValidateMetricRange(lineStep, "LineLengthMax", 500, 900);
        ValidateMetricRange(lineStep, "LineLengthMmMax", 3, 6);
        ValidateMetricRange(lineStep, "LineAngleAvg", -20, 20);
        ValidateResultStepImage(lineStep, 768, 576);
        ValidateInspectionOverlays(lineStep, "Pins_LineGauge");
        ValidateSuccessfulStepDiagnostics(lineStep);
        VisionRecipeOverlaySummary? fittedLine = lineStep.Overlays.FirstOrDefault(overlay =>
            string.Equals(overlay.Kind, VisionToolOverlayKind.Line.ToString(), StringComparison.OrdinalIgnoreCase));
        if (fittedLine == null)
        {
            throw new InvalidOperationException("Pins_LineGauge did not produce a fitted line overlay.");
        }

        VisionRecipeOverlaySummary? points = lineStep.Overlays.FirstOrDefault(overlay =>
            string.Equals(overlay.Kind, VisionToolOverlayKind.Points.ToString(), StringComparison.OrdinalIgnoreCase));
        if (points == null || points.PointCount < 30)
        {
            throw new InvalidOperationException($"Pins_LineGauge points overlay is too small. Points={points?.PointCount ?? 0}.");
        }

        ValidatePinsLineGaugeGeometry(fittedLine);
        ValidateFinalLayer(result, "Pins_LineGauge");
        ValidateStepFlow(result, "01 Smooth Pins", "Main", "Pins_Filtered");
        ValidateStepFlow(result, "02 Pin Rail Edge", "Pins_Filtered", "Pins_Edge");
        ValidateStepFlow(result, "03 Rail Line Gauge", "Pins_Edge", "Pins_LineGauge");
        double edgeCount = lineStep.Metrics["EdgeCount"];
        double lineLength = lineStep.Metrics["LineLengthMax"];
        double lineLengthMm = lineStep.Metrics["LineLengthMmMax"];
        double lineAngle = lineStep.Metrics["LineAngleAvg"];
        double lineY = (fittedLine.StartY + fittedLine.EndY) / 2d;
        return $"Pins_LineGauge: OK | EdgeCount={edgeCount:0}, Points={points.PointCount}, Length={lineLength:0.0}px/{lineLengthMm:0.###}mm, Angle={lineAngle:0.0}, LineY={lineY:0.0}, Final={result.FinalLayer}";
    }

    private static void ValidatePinsLineGaugeGeometry(VisionRecipeOverlaySummary line)
    {
        double minX = Math.Min(line.StartX, line.EndX);
        double maxX = Math.Max(line.StartX, line.EndX);
        double minY = Math.Min(line.StartY, line.EndY);
        double maxY = Math.Max(line.StartY, line.EndY);
        double spanX = maxX - minX;
        double driftY = maxY - minY;
        double centerY = (line.StartY + line.EndY) / 2d;

        if (spanX < 500)
        {
            throw new InvalidOperationException($"Pins_LineGauge fitted line is too short. SpanX={spanX:0.0}.");
        }

        if (centerY < 250 || centerY > 390)
        {
            throw new InvalidOperationException($"Pins_LineGauge fitted line is outside the rail ROI. CenterY={centerY:0.0}.");
        }

        if (driftY > 120)
        {
            throw new InvalidOperationException($"Pins_LineGauge fitted line is not stable enough horizontally. DriftY={driftY:0.0}.");
        }
    }

    private static string ValidateContourTemplateMatchingSample(object sample)
    {
        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary matchingStep = FindRequiredStep(result, "01 Match 7PQRS Button");
        ValidateMetricRange(matchingStep, "ResultCount", 1, 3);
        ValidateMetricRange(matchingStep, "ScoreMax", 90, 100);
        ValidateResultStepImage(matchingStep, 768, 576);
        ValidateInspectionOverlays(matchingStep, "Contour_TemplateMatching");
        VisionRecipeOverlaySummary matchingOverlay = ValidateTemplateMatchingOverlayGeometry(matchingStep);
        ValidateSuccessfulStepDiagnostics(matchingStep);
        ValidateFinalLayer(result, "Matching_Result");
        double score = matchingStep.Metrics["ScoreMax"];
        return $"Contour_TemplateMatching: OK | ScoreMax={score:0.#}, Center=({matchingOverlay.CenterX:0},{matchingOverlay.CenterY:0}), Overlay={matchingStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static VisionRecipeOverlaySummary ValidateTemplateMatchingOverlayGeometry(VisionRecipeStepRunSummary step)
    {
        VisionRecipeOverlaySummary overlay = step.Overlays
            .FirstOrDefault(item => string.Equals(item.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Contour_TemplateMatching did not produce a rectangle overlay.");

        if (overlay.CenterX < 170
            || overlay.CenterX > 192
            || overlay.CenterY < 113
            || overlay.CenterY > 140)
        {
            throw new InvalidOperationException(
                $"Contour_TemplateMatching detected the wrong target position. Center=({overlay.CenterX:0.#},{overlay.CenterY:0.#}). Expected the 7PQRS button near (182,126).");
        }

        if (overlay.BoundsX < 142
            || overlay.BoundsX > 154
            || overlay.BoundsY < 58
            || overlay.BoundsY > 70
            || overlay.BoundsWidth < 62
            || overlay.BoundsWidth > 74
            || overlay.BoundsHeight < 118
            || overlay.BoundsHeight > 130)
        {
            throw new InvalidOperationException(
                $"Contour_TemplateMatching overlay bounds are outside the 7PQRS button. Bounds=({overlay.BoundsX:0.#},{overlay.BoundsY:0.#},{overlay.BoundsWidth:0.#},{overlay.BoundsHeight:0.#}).");
        }

        return overlay;
    }

    private static string ValidateEasyImageTextContourSample(object sample)
    {
        return ValidateSimpleContourSample(
            sample,
            "03 Text Symbol Contour",
            "TextSymbol_Contour",
            20,
            60,
            new[]
            {
                "01 Text Symbol Binary|Main|TextSymbol_Binary",
                "02 Text Symbol Clean|TextSymbol_Binary|TextSymbol_Clean",
                "03 Text Symbol Contour|TextSymbol_Clean|TextSymbol_Contour"
            });
    }

    private static string ValidateGenericThresholdContourSample(object sample)
    {
        if (!TryParseDouble(GetStringProperty(sample, "ExpectedMetricMinimum"), out double minimum))
        {
            minimum = 1;
        }

        if (!TryParseDouble(GetStringProperty(sample, "ExpectedMetricMaximum"), out double maximum))
        {
            maximum = double.MaxValue;
        }

        return ValidateSimpleContourSample(
            sample,
            "03 Contour Inspect",
            "Contour_Result",
            minimum,
            maximum,
            new[]
            {
                "01 Binary Threshold|Main|Binary",
                "02 Noise Close|Binary|Closed",
                "03 Contour Inspect|Closed|Contour_Result"
            });
    }

    private static string ValidateSurfaceDefectEdgeSample(object sample)
    {
        if (!TryParseDouble(GetStringProperty(sample, "ExpectedMetricMinimum"), out double minimum))
        {
            minimum = 1;
        }

        if (!TryParseDouble(GetStringProperty(sample, "ExpectedMetricMaximum"), out double maximum))
        {
            maximum = 60;
        }

        using VisionRecipeRunResult result = RunCatalogSample(sample);
        VisionRecipeStepRunSummary finalStep = FindRequiredStep(result, "04 Surface Defect Contour");
        ValidateMetricRange(finalStep, "ResultCount", minimum, maximum);
        ValidateCatalogImageSize(sample, finalStep);
        ValidateInspectionOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateOverlayCountTracksResult(finalStep, GetStringProperty(sample, "SampleName"), 0.8);
        ValidateSmallSurfaceDefectOverlays(finalStep, GetStringProperty(sample, "SampleName"));
        ValidateSuccessfulStepDiagnostics(finalStep);
        ValidateFinalLayer(result, "SurfaceDefect_Contour");
        ValidateStepFlow(result, "01 Surface Smooth", "Main", "Surface_Smooth");
        ValidateStepFlow(result, "02 Surface Edge", "Surface_Smooth", "Surface_Edge");
        ValidateStepFlow(result, "03 Surface Edge Join", "Surface_Edge", "Surface_EdgeJoin");
        ValidateStepFlow(result, "04 Surface Defect Contour", "Surface_EdgeJoin", "SurfaceDefect_Contour");

        return $"{GetStringProperty(sample, "SampleName")}: OK | ResultCount={finalStep.Metrics["ResultCount"]:0}, Overlay={finalStep.OverlayCount}, Final={result.FinalLayer}";
    }

    private static string ValidateLlmContourRecipeSample(object sample)
    {
        string pipelinePath = GetStringProperty(sample, "PipelineFullPath");
        if (!SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline pipeline) || pipeline == null)
        {
            throw new InvalidOperationException($"LLM recipe could not be loaded. Path={pipelinePath}");
        }

        if (!ValidatePipelineForSmoke(ResolveOpenVisionLabAssembly(), pipeline, out string validationErrors, out int validationWarningCount))
        {
            throw new InvalidOperationException("LLM recipe validation failed. " + validationErrors.Replace(Environment.NewLine, " | "));
        }

        if (validationWarningCount < 2)
        {
            throw new InvalidOperationException($"LLM recipe should expose branch warnings for Main-based faint branches. Warnings={validationWarningCount}.");
        }

        using VisionRecipeRunResult result = RunCatalogSample(sample);
        if (result.Steps.Count != 10)
        {
            throw new InvalidOperationException($"LLM recipe should run 10 steps. Actual={result.Steps.Count}.");
        }

        List<VisionRecipeStepRunSummary> contourSteps = result.Steps
            .Where(step => string.Equals(step.ToolType, "Contour", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (contourSteps.Count < 3)
        {
            throw new InvalidOperationException($"LLM recipe should include at least 3 contour branches. Actual={contourSteps.Count}.");
        }

        foreach (VisionRecipeStepRunSummary step in contourSteps)
        {
            ValidateInspectionOverlays(step, step.Name);
            ValidateSuccessfulStepDiagnostics(step);
        }

        VisionRecipeStepRunSummary mergeStep = FindRequiredStep(result, "10 Merge All Detections");
        ValidateMetricRange(mergeStep, "MergeOverlayCount", 37, 100);
        ValidateMetricRange(mergeStep, "MergeSourceCount", 3, 3);
        ValidateResultStepImage(mergeStep, 768, 576);
        ValidateInspectionOverlays(mergeStep, "Contour_AllSymbolsAndFaint_LLM merge");
        ValidateMergedOverlayContract(mergeStep, contourSteps, 768, 576);
        ValidateSuccessfulStepDiagnostics(mergeStep);
        ValidateFinalLayer(result, "AllSymbols_Overlay");
        ValidateStepFlow(result, "10 Merge All Detections", "Main", "AllSymbols_Overlay");

        return $"Contour_AllSymbolsAndFaint_LLM: OK | Steps={result.Steps.Count}, Contours={contourSteps.Count}, Merged={mergeStep.Metrics["MergeOverlayCount"]:0}, Final={result.FinalLayer}";
    }

    private static VisionRecipeRunResult RunCatalogSample(object sample)
    {
        string imagePath = GetStringProperty(sample, "ImageFullPath");
        string pipelinePath = GetStringProperty(sample, "PipelineFullPath");
        using Bitmap bitmap = new Bitmap(imagePath);
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipelinePath, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!result.Success)
        {
            result.Dispose();
            throw new InvalidOperationException(result.Message);
        }

        return result;
    }

    private static VisionRecipeStepRunSummary FindRequiredStep(VisionRecipeRunResult result, string stepName)
    {
        return result.Steps.FirstOrDefault(step => string.Equals(step.Name, stepName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Step '{stepName}' was not produced.");
    }

    private static void ValidateMetricRange(VisionRecipeStepRunSummary step, string metricName, double minimum, double maximum)
    {
        if (!step.Metrics.TryGetValue(metricName, out double value))
        {
            throw new InvalidOperationException($"{step.Name}: metric '{metricName}' was not produced.");
        }

        if (value < minimum || value > maximum)
        {
            throw new InvalidOperationException($"{step.Name}: {metricName}={value:0.###} outside {minimum:0.###}..{maximum:0.###}.");
        }
    }

    private static void ValidateResultStepImage(VisionRecipeStepRunSummary step, int expectedWidth, int expectedHeight)
    {
        if (!step.HasResultImage)
        {
            throw new InvalidOperationException($"{step.Name}: result image was not produced.");
        }

        if (step.ResultImageWidth != expectedWidth || step.ResultImageHeight != expectedHeight)
        {
            throw new InvalidOperationException($"{step.Name}: result image size {step.ResultImageWidth}x{step.ResultImageHeight}, expected {expectedWidth}x{expectedHeight}.");
        }
    }

    private static void ValidateCatalogImageSize(object sample, VisionRecipeStepRunSummary step)
    {
        int expectedWidth = GetIntProperty(sample, "Width");
        int expectedHeight = GetIntProperty(sample, "Height");
        if (expectedWidth > 0 && expectedHeight > 0)
        {
            ValidateResultStepImage(step, expectedWidth, expectedHeight);
        }
    }

    private static void ValidateInspectionOverlays(VisionRecipeStepRunSummary step, string context)
    {
        if (step.OverlayCount <= 0 || step.Overlays.Count == 0)
        {
            throw new InvalidOperationException($"{context}: no overlay was produced.");
        }
    }

    private static void ValidateOverlayCountTracksResult(VisionRecipeStepRunSummary step, string context, double minimumRatio)
    {
        if (!step.Metrics.TryGetValue("ResultCount", out double resultCount))
        {
            return;
        }

        int expectedMinimum = Math.Max(1, (int)Math.Floor(resultCount * minimumRatio));
        if (step.OverlayCount < expectedMinimum || step.Overlays.Count < expectedMinimum)
        {
            throw new InvalidOperationException($"{context}: overlay count {step.OverlayCount}/{step.Overlays.Count} is too small for ResultCount={resultCount:0}.");
        }
    }

    private static void ValidateLargeRectangleOverlay(VisionRecipeStepRunSummary step, string context, double minimumWidth, double minimumHeight)
    {
        bool hasLargeRectangle = step.Overlays.Any(overlay =>
            string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
            && overlay.BoundsWidth >= minimumWidth
            && overlay.BoundsHeight >= minimumHeight);

        if (!hasLargeRectangle)
        {
            throw new InvalidOperationException($"{context}: no large rectangle overlay was produced.");
        }
    }

    private static void ValidateBentPinLargeGeometry(VisionRecipeStepRunSummary step, string context)
    {
        double imageWidth = Math.Max(1, step.ResultImageWidth);
        double imageHeight = Math.Max(1, step.ResultImageHeight);
        List<VisionRecipeOverlaySummary> largeRectangles = step.Overlays
            .Where(overlay => string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .Where(overlay => overlay.BoundsWidth >= imageWidth * 0.85 && overlay.BoundsHeight >= imageHeight * 0.35)
            .OrderBy(overlay => overlay.CenterY)
            .ToList();

        if (largeRectangles.Count < 2)
        {
            throw new InvalidOperationException($"{context}: expected upper/lower large defect regions, but found {largeRectangles.Count}.");
        }

        VisionRecipeOverlaySummary upper = largeRectangles.First();
        VisionRecipeOverlaySummary lower = largeRectangles.Last();
        if (upper.CenterY > imageHeight * 0.40)
        {
            throw new InvalidOperationException($"{context}: upper large defect region is too low. CenterY={upper.CenterY:0.#}.");
        }

        if (lower.CenterY < imageHeight * 0.60)
        {
            throw new InvalidOperationException($"{context}: lower large defect region is too high. CenterY={lower.CenterY:0.#}.");
        }

        if (lower.CenterY - upper.CenterY < imageHeight * 0.35)
        {
            throw new InvalidOperationException($"{context}: large defect regions are not vertically separated enough. Upper={upper.CenterY:0.#}, Lower={lower.CenterY:0.#}.");
        }
    }

    private static void ValidateBentPinSingleRegionGeometry(
        VisionRecipeStepRunSummary step,
        string regionName,
        bool upperRegion)
    {
        double imageWidth = Math.Max(1, step.ResultImageWidth);
        double imageHeight = Math.Max(1, step.ResultImageHeight);
        VisionRecipeOverlaySummary rectangle = step.Overlays.FirstOrDefault(overlay =>
            string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
            && overlay.BoundsWidth >= imageWidth * 0.85
            && overlay.BoundsHeight >= imageHeight * 0.35);

        if (rectangle == null)
        {
            throw new InvalidOperationException($"BentPin {regionName}: large ROI rectangle was not produced.");
        }

        if (upperRegion && rectangle.CenterY > imageHeight * 0.40)
        {
            throw new InvalidOperationException($"BentPin {regionName}: expected upper defect region, CenterY={rectangle.CenterY:0.#}.");
        }

        if (!upperRegion && rectangle.CenterY < imageHeight * 0.60)
        {
            throw new InvalidOperationException($"BentPin {regionName}: expected lower defect region, CenterY={rectangle.CenterY:0.#}.");
        }
    }

    private static void ValidateBentPinShaftGeometry(
        VisionRecipeStepRunSummary step,
        string context,
        bool expectedBent)
    {
        List<VisionRecipeOverlaySummary> rectangles = step.Overlays
            .Where(overlay => string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .OrderBy(overlay => overlay.CenterX)
            .ToList();

        if (rectangles.Count != 13)
        {
            throw new InvalidOperationException($"{context}: expected 13 upper pin shafts, actual {rectangles.Count}.");
        }

        double spanX = rectangles.Last().CenterX - rectangles.First().CenterX;
        if (spanX < 620)
        {
            throw new InvalidOperationException($"{context}: pin shaft row span is too short. SpanX={spanX:0.#}.");
        }

        double maxWidth = rectangles.Max(rectangle => rectangle.BoundsWidth);
        double minHeight = rectangles.Min(rectangle => rectangle.BoundsHeight);
        double maxHeight = rectangles.Max(rectangle => rectangle.BoundsHeight);
        if (minHeight < 110 || maxHeight > 170)
        {
            throw new InvalidOperationException($"{context}: pin shaft height is outside expected range. Min={minHeight:0.#}, Max={maxHeight:0.#}.");
        }

        if (expectedBent)
        {
            if (maxWidth < 24)
            {
                throw new InvalidOperationException($"{context}: bent pin was not separated from normal shafts. MaxWidth={maxWidth:0.#}.");
            }
        }
        else if (maxWidth > 18)
        {
            throw new InvalidOperationException($"{context}: normal pin shaft is too wide. MaxWidth={maxWidth:0.#}.");
        }
    }

    private static void ValidateDiePadSurfaceGeometry(VisionRecipeStepRunSummary step, string context)
    {
        double imageWidth = Math.Max(1, step.ResultImageWidth);
        double imageHeight = Math.Max(1, step.ResultImageHeight);
        List<VisionRecipeOverlaySummary> rectangles = step.Overlays
            .Where(overlay => string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (rectangles.Count < 8)
        {
            throw new InvalidOperationException($"{context}: die-pad contour should expose several surface features. Rectangles={rectangles.Count}.");
        }

        VisionRecipeOverlaySummary largest = rectangles
            .OrderByDescending(overlay => overlay.BoundsWidth * overlay.BoundsHeight)
            .First();

        double largestWidthRatio = largest.BoundsWidth / imageWidth;
        double largestHeightRatio = largest.BoundsHeight / imageHeight;
        if (largest.BoundsX > imageWidth * 0.12
            || largest.BoundsY > imageHeight * 0.08
            || largestWidthRatio < 0.45
            || largestWidthRatio > 0.72
            || largestHeightRatio < 0.52
            || largestHeightRatio > 0.75)
        {
            throw new InvalidOperationException(
                $"{context}: largest surface region is outside expected die-pad location/size. Bounds=({largest.BoundsX:0},{largest.BoundsY:0},{largest.BoundsWidth:0},{largest.BoundsHeight:0}).");
        }

        int tallRightStructures = rectangles.Count(overlay =>
            overlay.BoundsX >= imageWidth * 0.40
            && overlay.BoundsHeight >= imageHeight * 0.35
            && overlay.BoundsWidth <= imageWidth * 0.25);
        if (tallRightStructures < 2)
        {
            throw new InvalidOperationException($"{context}: expected right-side tall die-pad structures. Count={tallRightStructures}.");
        }

        int smallFeatures = rectangles.Count(overlay =>
            overlay.BoundsWidth * overlay.BoundsHeight >= 50
            && overlay.BoundsWidth * overlay.BoundsHeight <= 1500);
        if (smallFeatures < 6)
        {
            throw new InvalidOperationException($"{context}: expected small die-pad feature contours. Count={smallFeatures}.");
        }
    }

    private static void ValidateSmallSurfaceDefectOverlays(VisionRecipeStepRunSummary step, string context)
    {
        List<VisionRecipeOverlaySummary> rectangles = step.Overlays
            .Where(overlay => string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (rectangles.Count == 0)
        {
            throw new InvalidOperationException($"{context}: no surface-defect rectangle overlays were produced.");
        }

        double imageArea = Math.Max(1, step.ResultImageWidth * step.ResultImageHeight);
        foreach (VisionRecipeOverlaySummary rectangle in rectangles)
        {
            double area = rectangle.BoundsWidth * rectangle.BoundsHeight;
            double ratio = area / imageArea;
            if (rectangle.BoundsWidth > 80
                || rectangle.BoundsHeight > 80
                || ratio > 0.01)
            {
                throw new InvalidOperationException(
                    $"{context}: surface defect overlay is too broad. Bounds=({rectangle.BoundsX:0},{rectangle.BoundsY:0},{rectangle.BoundsWidth:0},{rectangle.BoundsHeight:0}).");
            }
        }

        int candidatesInsideInspectionBand = rectangles.Count(rectangle =>
            rectangle.CenterX >= 120
            && rectangle.CenterX <= Math.Max(120, step.ResultImageWidth - 120)
            && rectangle.CenterY >= 8
            && rectangle.CenterY <= Math.Max(8, step.ResultImageHeight - 8));

        if (candidatesInsideInspectionBand < rectangles.Count)
        {
            throw new InvalidOperationException($"{context}: surface defect overlay escaped the inspection band.");
        }
    }

    private static void ValidateMergedOverlayContract(
        VisionRecipeStepRunSummary mergeStep,
        IReadOnlyList<VisionRecipeStepRunSummary> sourceSteps,
        int imageWidth,
        int imageHeight)
    {
        if (sourceSteps == null || sourceSteps.Count == 0)
        {
            throw new InvalidOperationException($"{mergeStep.Name}: no source contour steps were supplied for merge validation.");
        }

        int expectedOverlayCount = sourceSteps.Sum(step => step.OverlayCount);
        int expectedSourceCount = sourceSteps.Count(step => step.OverlayCount > 0);
        ValidateMetricExact(mergeStep, "MergeOverlayCount", expectedOverlayCount);
        ValidateMetricExact(mergeStep, "MergeSourceCount", expectedSourceCount);

        if (mergeStep.OverlayCount != expectedOverlayCount || mergeStep.Overlays.Count != expectedOverlayCount)
        {
            throw new InvalidOperationException($"{mergeStep.Name}: merged overlay count {mergeStep.OverlayCount}/{mergeStep.Overlays.Count}, expected {expectedOverlayCount}.");
        }

        foreach (VisionRecipeStepRunSummary sourceStep in sourceSteps)
        {
            foreach (VisionRecipeOverlaySummary sourceOverlay in sourceStep.Overlays)
            {
                if (!mergeStep.Overlays.Any(mergeOverlay => IsSameOverlay(sourceOverlay, mergeOverlay)))
                {
                    throw new InvalidOperationException($"{mergeStep.Name}: missing overlay from source step '{sourceStep.Name}'.");
                }
            }
        }

        foreach (VisionRecipeOverlaySummary overlay in mergeStep.Overlays)
        {
            ValidateMergedOverlayBounds(mergeStep.Name, overlay, imageWidth, imageHeight);
        }

        ValidateMergedRegion(mergeStep, "text and symbol candidates", 40, 55, 610, 470, 35);
        ValidateMergedRegion(mergeStep, "faint top mark", 330, 45, 150, 110, 2);
        ValidateMergedRegion(mergeStep, "faint phone mark", 690, 315, 65, 125, 2);
    }

    private static void ValidateMergedOverlayIdentity(
        VisionRecipeStepRunSummary mergeStep,
        IReadOnlyList<VisionRecipeStepRunSummary> sourceSteps,
        int imageWidth,
        int imageHeight)
    {
        if (sourceSteps == null || sourceSteps.Count == 0)
        {
            throw new InvalidOperationException($"{mergeStep.Name}: no source steps were supplied for merge validation.");
        }

        int expectedOverlayCount = sourceSteps.Sum(step => step.OverlayCount);
        int expectedSourceCount = sourceSteps.Count(step => step.OverlayCount > 0);
        ValidateMetricExact(mergeStep, "MergeOverlayCount", expectedOverlayCount);
        ValidateMetricExact(mergeStep, "MergeSourceCount", expectedSourceCount);

        if (mergeStep.OverlayCount != expectedOverlayCount || mergeStep.Overlays.Count != expectedOverlayCount)
        {
            throw new InvalidOperationException($"{mergeStep.Name}: merged overlay count {mergeStep.OverlayCount}/{mergeStep.Overlays.Count}, expected {expectedOverlayCount}.");
        }

        foreach (VisionRecipeStepRunSummary sourceStep in sourceSteps)
        {
            foreach (VisionRecipeOverlaySummary sourceOverlay in sourceStep.Overlays)
            {
                if (!mergeStep.Overlays.Any(mergeOverlay => IsSameOverlay(sourceOverlay, mergeOverlay)))
                {
                    throw new InvalidOperationException($"{mergeStep.Name}: missing overlay from source step '{sourceStep.Name}'.");
                }
            }
        }

        foreach (VisionRecipeOverlaySummary overlay in mergeStep.Overlays)
        {
            ValidateOverlayInsideImageBounds(mergeStep.Name, overlay, imageWidth, imageHeight);
        }
    }

    private static void ValidateMetricExact(VisionRecipeStepRunSummary step, string metricName, int expected)
    {
        if (!step.Metrics.TryGetValue(metricName, out double value))
        {
            throw new InvalidOperationException($"{step.Name}: metric '{metricName}' was not produced.");
        }

        int actual = (int)Math.Round(value);
        if (actual != expected)
        {
            throw new InvalidOperationException($"{step.Name}: {metricName}={actual}, expected {expected}.");
        }
    }

    private static bool IsSameOverlay(VisionRecipeOverlaySummary left, VisionRecipeOverlaySummary right)
    {
        return string.Equals(left.Kind, right.Kind, StringComparison.OrdinalIgnoreCase)
            && IsNear(left.BoundsX, right.BoundsX)
            && IsNear(left.BoundsY, right.BoundsY)
            && IsNear(left.BoundsWidth, right.BoundsWidth)
            && IsNear(left.BoundsHeight, right.BoundsHeight)
            && IsNear(left.CenterX, right.CenterX)
            && IsNear(left.CenterY, right.CenterY);
    }

    private static bool IsNear(float left, float right)
    {
        return Math.Abs(left - right) <= 0.25F;
    }

    private static void ValidateOverlayInsideImageBounds(
        string context,
        VisionRecipeOverlaySummary overlay,
        int imageWidth,
        int imageHeight)
    {
        if (!string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (overlay.BoundsWidth <= 0 || overlay.BoundsHeight <= 0)
        {
            throw new InvalidOperationException($"{context}: merged rectangle has invalid bounds.");
        }

        if (overlay.BoundsX < -1
            || overlay.BoundsY < -1
            || overlay.BoundsX + overlay.BoundsWidth > imageWidth + 1
            || overlay.BoundsY + overlay.BoundsHeight > imageHeight + 1)
        {
            throw new InvalidOperationException($"{context}: merged rectangle is outside image bounds. Bounds={overlay.BoundsX:0.#},{overlay.BoundsY:0.#},{overlay.BoundsWidth:0.#},{overlay.BoundsHeight:0.#}, Image={imageWidth}x{imageHeight}.");
        }
    }

    private static void ValidateMergedOverlayBounds(
        string context,
        VisionRecipeOverlaySummary overlay,
        int imageWidth,
        int imageHeight)
    {
        if (!string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (overlay.BoundsWidth <= 0 || overlay.BoundsHeight <= 0)
        {
            throw new InvalidOperationException($"{context}: merged rectangle has invalid bounds.");
        }

        if (overlay.BoundsX < -1
            || overlay.BoundsY < -1
            || overlay.BoundsX + overlay.BoundsWidth > imageWidth + 1
            || overlay.BoundsY + overlay.BoundsHeight > imageHeight + 1)
        {
            throw new InvalidOperationException($"{context}: merged rectangle is outside image bounds.");
        }

        double areaRatio = (overlay.BoundsWidth * overlay.BoundsHeight) / Math.Max(1.0, imageWidth * imageHeight);
        if (overlay.BoundsWidth > 120
            || overlay.BoundsHeight > 120
            || areaRatio > 0.03)
        {
            throw new InvalidOperationException($"{context}: merged rectangle is too broad for object-level review. Bounds={overlay.BoundsX:0.#},{overlay.BoundsY:0.#},{overlay.BoundsWidth:0.#},{overlay.BoundsHeight:0.#}.");
        }
    }

    private static void ValidateMergedRegion(
        VisionRecipeStepRunSummary mergeStep,
        string regionName,
        float x,
        float y,
        float width,
        float height,
        int minimumCount)
    {
        int count = mergeStep.Overlays.Count(overlay =>
            string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
            && overlay.CenterX >= x
            && overlay.CenterX <= x + width
            && overlay.CenterY >= y
            && overlay.CenterY <= y + height);

        if (count < minimumCount)
        {
            throw new InvalidOperationException($"{mergeStep.Name}: merged result should contain at least {minimumCount} overlays in '{regionName}', actual {count}.");
        }
    }

    private static void ValidateSuccessfulStepDiagnostics(VisionRecipeStepRunSummary step)
    {
        if (!step.Success)
        {
            throw new InvalidOperationException($"{step.Name}: expected successful step, but status was {step.Status}.");
        }

        if (!step.AcceptancePassed)
        {
            throw new InvalidOperationException($"{step.Name}: successful step must have AcceptancePassed=true.");
        }

        if (!string.Equals(step.Status, "OK", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{step.Name}: successful step status should be OK, but was '{step.Status}'.");
        }

        if (step.ErrorCode != 0
            || !string.Equals(step.ErrorName, VisionToolErrorCode.None.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{step.Name}: successful step should expose ErrorCode=0/None, but was {step.ErrorCode}:{step.ErrorName}.");
        }

        if (!string.Equals(step.ResultStatus, VisionToolResultStatus.Passed.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{step.Name}: successful step should expose ResultStatus=Passed, but was '{step.ResultStatus}'.");
        }

        if (!string.IsNullOrWhiteSpace(step.DiagnosticHint)
            || !string.IsNullOrWhiteSpace(step.SuggestedFix))
        {
            throw new InvalidOperationException($"{step.Name}: successful step should not expose failure diagnostics.");
        }
    }

    private static void ValidateFinalLayer(VisionRecipeRunResult result, string expectedFinalLayer)
    {
        if (!string.Equals(result.FinalLayer, expectedFinalLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Final layer should be '{expectedFinalLayer}', but was '{result.FinalLayer}'.");
        }
    }

    private static void ValidateStepFlow(VisionRecipeRunResult result, string stepName, string expectedInput, string expectedOutput)
    {
        VisionRecipeStepRunSummary step = FindRequiredStep(result, stepName);
        if (!string.Equals(step.InputLayer, expectedInput, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(step.OutputLayer, expectedOutput, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{stepName}: flow {step.InputLayer} -> {step.OutputLayer}, expected {expectedInput} -> {expectedOutput}.");
        }
    }

    private static void AddLegacyCvSummary(List<string> lines)
    {
        string noahToolDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Library-Noah", "Lib.OpenCV", "OpenCV", "Tool"));
        if (!Directory.Exists(noahToolDirectory))
        {
            noahToolDirectory = @"C:\Git\Library-Noah\Lib.OpenCV\OpenCV\Tool";
        }

        string[] legacyTypes =
        {
            "CVContour",
            "CVMatching",
            "CVSIFT",
            "CVLineGuage",
            "CVMean"
        };

        List<string> summaries = new();
        foreach (string typeName in legacyTypes)
        {
            int hitCount = Directory.Exists(noahToolDirectory)
                ? Directory.EnumerateFiles(noahToolDirectory, "*.cs", SearchOption.TopDirectoryOnly)
                    .Sum(file => File.ReadAllText(file).Split(new[] { typeName }, StringSplitOptions.None).Length - 1)
                : 0;
            summaries.Add($"{typeName}:{hitCount}");
        }

        lines.Add("Legacy CV* scan: " + string.Join(", ", summaries));
    }

    private static CaptureDiagnostics CapturePipelineToolResultContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

        List<object> samples = LoadRunnableCatalogSamples().ToList();
        List<string> errors = new();
        List<string> lines = new()
        {
            "Pipeline Tool Result Contract Check",
            "Status: CHECK OK"
        };

        int checkedStepCount = 0;
        int checkedStatusStepCount = 0;
        foreach (object sample in samples)
        {
            string name = GetStringProperty(sample, "SampleName");
            string imagePath = GetStringProperty(sample, "ImageFullPath");
            string pipelinePath = GetStringProperty(sample, "PipelineFullPath");

            try
            {
                using Bitmap bitmap = new Bitmap(imagePath);
                using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
                using VisionRecipeRunResult result = new VisionRecipeRunner()
                    .RunAsync(pipelinePath, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

                if (!result.Success)
                {
                    errors.Add($"{name}: run failed. {result.Message}");
                    continue;
                }

                int sampleCheckedCount = 0;
                foreach (VisionRecipeStepRunSummary step in result.Steps)
                {
                    if (!step.Skipped)
                    {
                        ValidateSuccessfulStepDiagnostics(step);
                        checkedStatusStepCount++;
                    }

                    if (!step.Success || step.Skipped || !IsInspectionToolForContract(step.ToolType))
                    {
                        continue;
                    }

                    sampleCheckedCount++;
                    checkedStepCount++;
                    if (step.MetricCount <= 0)
                    {
                        errors.Add($"{name} / {step.Name}: inspection step produced no metrics.");
                    }

                    if (IsScoreToolForContract(step.ToolType)
                        && step.Metrics.TryGetValue("ResultCount", out double resultCount)
                        && resultCount > 0
                        && !step.Metrics.ContainsKey("ScoreMax"))
                    {
                        errors.Add($"{name} / {step.Name}: detected results did not expose ScoreMax.");
                    }

                    if (RequiresOverlayForContract(step.ToolType) && step.OverlayCount <= 0)
                    {
                        errors.Add($"{name} / {step.Name}: inspection step produced no overlays.");
                    }
                }

                lines.Add($"{name}: {sampleCheckedCount} inspection step(s) checked");
            }
            catch (Exception ex)
            {
                errors.Add($"{name}: {ex.GetBaseException().Message}");
            }
        }

        try
        {
            lines.Add(RunTransformImageMetricContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Transform image metric contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunRotateScaleContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("RotateScale contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMeanContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Mean contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunSourceImageIsolationContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Source image isolation contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunInvalidStepConfigurationContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Invalid step configuration contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingPreprocessModeContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching preprocess contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingMultiRoiContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching multi ROI contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingMultipleDetectionContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching multiple detection contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingSqDiffNormedContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching SqDiffNormed contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingAngleScaleContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching angle/scale contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingRotatedFixtureContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching rotated fixture contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunMatchingNoResultContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Matching no-result contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunFeatureMatchingContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("FeatureMatching contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunFeatureMatchingNoFalsePositiveContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("FeatureMatching no false positive contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunRectangleOverlayGeometryContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Rectangle overlay geometry contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunContourOptionContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Contour option contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunContourSquareContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Contour SquareRun contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunBlobOptionContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Blob option contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunLineGaugeDirectionContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("LineGauge direction contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunLineGaugePolarityContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("LineGauge polarity contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunAcceptanceReasonContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Acceptance reason contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunAcceptancePresetContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Acceptance preset contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunLegacyApiContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Legacy API contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunToolRunNotificationContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Tool run notification contract: " + ex.GetBaseException().Message);
        }

        try
        {
            lines.Add(RunDirectToolFormContractCase());
            checkedStepCount++;
        }
        catch (Exception ex)
        {
            errors.Add("Direct Tool Form contract: " + ex.GetBaseException().Message);
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException("Tool result contract check failed. " + string.Join(" | ", errors.Take(5)));
        }

        lines.Insert(2, $"Runnable samples: {samples.Count}");
        lines.Insert(3, $"Checked successful step statuses: {checkedStatusStepCount}");
        lines.Insert(4, $"Checked inspection steps: {checkedStepCount}");
        lines.Insert(5, "");
        lines.Add("");
        lines.Add("Contract: successful steps must expose OK/Passed status, ErrorCode=0/None, AcceptancePassed=true, and no failure diagnostics.");
        lines.Add("Contract: successful inspection steps must expose metrics.");
        lines.Add("Contract: visual inspection steps must expose overlays.");
        lines.Add("Contract: standalone Tool Form notifications must carry ResultStatus, ErrorCode, metrics, and overlays.");
        lines.Add("Contract: legacy direct Tool Forms must record a VisionToolResult before publishing output.");

        using Form report = CreateSmokeReportForm("Pipeline Tool Result Contract Check", lines);
        return CaptureForm(report, outputPath, new Size(980, 560), 8);
    }

    private static string RunLegacyApiContractCase()
    {
        Assembly openCvAssembly = typeof(MatchingTool).Assembly;
        Assembly blobAssembly = typeof(BlobTool).Assembly;

        List<Type> legacyTypes = openCvAssembly.GetExportedTypes()
            .Concat(blobAssembly.GetExportedTypes())
            .Where(IsLegacyCompatibilityType)
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToList();

        if (legacyTypes.Count == 0)
        {
            throw new InvalidOperationException("No legacy compatibility types were found.");
        }

        foreach (Type type in legacyTypes)
        {
            ValidateLegacyObsoleteType(type);
        }

        return $"Legacy API contract: {legacyTypes.Count} CV*/CResult* compatibility types are marked obsolete";
    }

    private static string RunToolRunNotificationContractCase()
    {
        VisionToolRunEventArgs ok = new VisionToolRunEventArgs
        {
            Status = VisionToolRunStatus.Completed,
            ToolName = "Contour",
            SourceLayer = "Main",
            OutputLayer = "Contour_Result",
            ResultWidth = 768,
            ResultHeight = 576,
            OverlayCount = 12,
            MetricCount = 6,
            ErrorCode = 0,
            ErrorName = VisionToolErrorCode.None.ToString(),
            ResultStatus = VisionToolResultStatus.Passed.ToString(),
            Message = "OK"
        };

        if (!ok.Success || ok.HasToolError || ok.OverlayCount <= 0 || ok.MetricCount <= 0)
        {
            throw new InvalidOperationException("Successful notification did not preserve success metrics.");
        }

        VisionToolRunEventArgs ng = new VisionToolRunEventArgs
        {
            Status = VisionToolRunStatus.Failed,
            ToolName = "Contour",
            SourceLayer = "Main",
            ErrorCode = (int)VisionToolErrorCode.ContourNoResult,
            ErrorName = VisionToolErrorCode.ContourNoResult.ToString(),
            ResultStatus = "NoResult",
            Message = "No contour found."
        };

        if (ng.Success || !ng.HasToolError || ng.ErrorCode <= 0 || string.IsNullOrWhiteSpace(ng.ResultStatus))
        {
            throw new InvalidOperationException("Failed notification did not preserve error/status fields.");
        }

        return "ToolRunNotification contract: OK/NG event fields preserved";
    }

    private static string RunDirectToolFormContractCase()
    {
        string repoRoot = ResolveRepoRootForSmoke();
        string formDirectory = Path.Combine(repoRoot, "0. UI", "6) Vision Test");
        string[] requiredFiles =
        {
            "FormVision_Arithmetic.cs",
            "FormVision_Histogram.cs",
            "FormVision_Line.cs",
            "FormVision_RotateAndScale.cs"
        };

        List<string> missing = new();
        foreach (string fileName in requiredFiles)
        {
            string path = Path.Combine(formDirectory, fileName);
            if (!File.Exists(path))
            {
                missing.Add(fileName + " (missing)");
                continue;
            }

            string source = File.ReadAllText(path);
            if (!source.Contains("RecordDirectVisionToolPassed(", StringComparison.Ordinal))
            {
                missing.Add(fileName);
            }
        }

        if (missing.Count > 0)
        {
            throw new InvalidOperationException("Direct Tool Forms without result recording: " + string.Join(", ", missing));
        }

        return $"Direct Tool Form contract: {requiredFiles.Length} legacy direct forms record VisionToolResult";
    }

    private static bool IsLegacyCompatibilityType(Type type)
    {
        if (type == null || string.IsNullOrWhiteSpace(type.Namespace))
        {
            return false;
        }

        if (!type.Namespace.StartsWith("Lib.OpenCV", StringComparison.Ordinal))
        {
            return false;
        }

        return type.Name.StartsWith("CV", StringComparison.Ordinal)
            || type.Name.StartsWith("CResult", StringComparison.Ordinal);
    }

    private static void ValidateLegacyObsoleteType(Type type)
    {
        ObsoleteAttribute? obsolete = type.GetCustomAttribute<ObsoleteAttribute>(inherit: false);
        if (obsolete == null)
        {
            throw new InvalidOperationException($"{type.FullName} is not marked as a legacy API.");
        }

        if (string.IsNullOrWhiteSpace(obsolete.Message)
            || !obsolete.Message.Contains("Legacy compatibility", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{type.FullName} obsolete message does not describe legacy compatibility.");
        }
    }

    private static string RunRotateScaleContractCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_RotateScaleContract"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 RotateScale Contract",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "RotateScale_Output"
        };
        step.Parameters["Angle"] = "0";
        step.Parameters["ScaleXPercent"] = "150";
        step.Parameters["ScaleYPercent"] = "50";
        pipeline.Steps.Add(step);

        using Bitmap bitmap = CreatePipelinePreviewRunImage();
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        VisionRecipeStepRunSummary summary = result.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException("RotateScale step summary was not produced.");
        int expectedWidth = Math.Max(1, (int)Math.Round(bitmap.Width * 1.5d));
        int expectedHeight = Math.Max(1, (int)Math.Round(bitmap.Height * 0.5d));
        if (summary.ResultImageWidth != expectedWidth || summary.ResultImageHeight != expectedHeight)
        {
            throw new InvalidOperationException(
                $"Expected image {expectedWidth}x{expectedHeight}, but got {summary.ResultImageWidth}x{summary.ResultImageHeight}.");
        }

        ValidateStepImageMetrics("RotateScale", summary, expectedWidth, expectedHeight, summary.ResultImageChannelCount);

        using OpenCvSharp.Mat directSource = new OpenCvSharp.Mat(
            30,
            50,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(10, 80, 160));
        using OpenCvSharp.Mat scaled = RotateScaleTool.Transform(
            directSource,
            0,
            200,
            50,
            OpenCvSharp.InterpolationFlags.Nearest,
            OpenCvSharp.BorderTypes.Constant);
        ValidateRotateScaleTransform("RotateScale direct scale", scaled, 100, 15, 3);

        using OpenCvSharp.Mat rotated = RotateScaleTool.Transform(
            directSource,
            35,
            100,
            100,
            OpenCvSharp.InterpolationFlags.Linear,
            OpenCvSharp.BorderTypes.Constant);
        ValidateRotateScaleTransform("RotateScale direct rotate", rotated, 50, 30, 3);

        return $"RotateScale: OK | Output={summary.ResultImageSizeText}, Direct={scaled.Width}x{scaled.Height}/{rotated.Width}x{rotated.Height}";
    }

    private static string RunTransformImageMetricContractCase()
    {
        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            24,
            32,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(120));

        ThresholdTool thresholdTool = new();
        thresholdTool.SetProperty(new ThresholdToolProperty
        {
            Mode = ThresholdToolMode.Threshold,
            Threshold = 100,
            MaxValue = 255,
            ThresholdType = OpenCvSharp.ThresholdTypes.Binary
        });

        VisionToolResult thresholdResult = thresholdTool.Execute(source);
        ValidateResultImageMetrics("Threshold", thresholdResult, 32, 24, 1);

        using OpenCvSharp.Mat thresholdSource4Ch = new OpenCvSharp.Mat(
            24,
            32,
            OpenCvSharp.MatType.CV_8UC4,
            new OpenCvSharp.Scalar(120, 120, 120, 255));
        VisionToolResult threshold4ChResult = thresholdTool.Execute(thresholdSource4Ch);
        ValidateResultImageMetrics("Threshold4Ch", threshold4ChResult, 32, 24, 1);

        using OpenCvSharp.Mat rangeSource = new OpenCvSharp.Mat(
            24,
            32,
            OpenCvSharp.MatType.CV_8UC4,
            new OpenCvSharp.Scalar(180, 180, 180, 255));
        ThresholdTool rangeTool = new();
        rangeTool.SetProperty(new ThresholdToolProperty
        {
            Mode = ThresholdToolMode.Range,
            RangeMin = 170,
            RangeMax = 238,
            MaxValue = 255
        });

        VisionToolResult rangeResult = rangeTool.Execute(rangeSource);
        ValidateResultImageMetrics("ThresholdRange4Ch", rangeResult, 32, 24, 1);
        double rangeMean = OpenCvSharp.Cv2.Mean(rangeResult.ResultImage).Val0;
        if (rangeMean < 250)
        {
            throw new InvalidOperationException($"ThresholdRange4Ch should ignore alpha channel after grayscale normalization. Mean={rangeMean:0.###}.");
        }

        MorphologyTool morphologyTool = new();
        morphologyTool.SetProperty(new MorphologyToolProperty
        {
            Shape = OpenCvSharp.MorphShapes.Rect,
            Operator = OpenCvSharp.MorphTypes.Dilate,
            KernelWidth = 3,
            KernelHeight = 3,
            Iterations = 1
        });

        VisionToolResult morphologyResult = morphologyTool.Execute(thresholdResult.ResultImage);
        ValidateResultImageMetrics("Morphology", morphologyResult, 32, 24, 1);

        return "Transform image metrics: OK | Threshold=32x24x1, Range4Ch=32x24x1, Morphology=32x24x1";
    }

    private static void ValidateRotateScaleTransform(
        string name,
        OpenCvSharp.Mat image,
        int expectedWidth,
        int expectedHeight,
        int expectedChannels)
    {
        if (image.Empty())
        {
            throw new InvalidOperationException($"{name} produced an empty image.");
        }

        if (image.Width != expectedWidth || image.Height != expectedHeight)
        {
            throw new InvalidOperationException(
                $"{name} expected {expectedWidth}x{expectedHeight}, but got {image.Width}x{image.Height}.");
        }

        if (image.Channels() != expectedChannels)
        {
            throw new InvalidOperationException(
                $"{name} changed channels. Expected={expectedChannels}, Actual={image.Channels()}.");
        }
    }

    private static string RunMeanContractCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_MeanContract"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 Mean Contract",
            ToolType = "Mean",
            InputLayer = "Main",
            OutputLayer = "Mean_Output"
        };
        step.Parameters["MEAN_TYPES"] = "Mean";
        step.Parameters["USE_THRESHOLD"] = "true";
        step.Parameters["THRESHOLD"] = "50";
        step.Parameters["THRESHOLD_TYPES"] = "Binary";
        pipeline.Steps.Add(step);

        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            20,
            30,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(120));
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        VisionRecipeStepRunSummary summary = result.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException("Mean step summary was not produced.");
        if (!summary.Metrics.TryGetValue("ResultCount", out double resultCount)
            || Math.Abs(resultCount - 1d) > 0.001d)
        {
            throw new InvalidOperationException($"Expected ResultCount=1, but got {resultCount:0.###}.");
        }

        if (!summary.Metrics.TryGetValue("MeanValueAvg", out double meanValue)
            || Math.Abs(meanValue - 255d) > 0.001d)
        {
            throw new InvalidOperationException($"Expected thresholded MeanValueAvg=255, but got {meanValue:0.###}.");
        }

        MeanProperty directProperty = new("Smoke_MeanNoMutation")
        {
            USE_ROI = false,
            USE_MULTI_ROI = false,
            CvROI = new OpenCvSharp.Rect(0, 0, 0, 0),
            MEAN_TYPES = MeanType.Mean
        };
        MeanTool directTool = new();
        directTool.SetProperty(directProperty);
        VisionToolResult directResult = directTool.Execute(source);
        if (!directResult.Success)
        {
            throw new InvalidOperationException($"Mean direct run failed. {directResult.ErrorName}: {directResult.Message}");
        }

        if (directProperty.CvROI.Width != 0 || directProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Mean fallback ROI mutated property. ROI={directProperty.CvROI}.");
        }

        return $"Mean: OK | ResultCount={resultCount:0} | MeanValueAvg={meanValue:0.###}";
    }

    private static string RunSourceImageIsolationContractCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_SourceImageIsolationContract"
        };

        VisionPipelineStep blob = new()
        {
            Name = "01 Blob Source Isolation",
            ToolType = "Blob",
            InputLayer = "Main",
            OutputLayer = "Blob_Output"
        };
        blob.Parameters["USE_THRESHOLD"] = "true";
        blob.Parameters["THRESHOLD"] = "80";
        blob.Parameters["THRESHOLD_TYPES"] = "Binary";
        blob.Parameters["MIN_AREA"] = "10";
        blob.Parameters["MAX_AREA"] = "10000";
        pipeline.Steps.Add(blob);

        VisionPipelineStep lineGauge = new()
        {
            Name = "02 LineGauge ROI Isolation",
            ToolType = "LineGauge",
            InputLayer = "Main",
            OutputLayer = "Line_Output"
        };
        lineGauge.Parameters["USE_ROI"] = "true";
        lineGauge.Parameters["CvROI"] = "4,4,20,20";
        lineGauge.Parameters["USE_THRESHOLD"] = "true";
        lineGauge.Parameters["THRESHOLD"] = "80";
        lineGauge.Parameters["THRESHOLD_TYPES"] = "Binary";
        lineGauge.Parameters["SAMPLING_STEP"] = "2";
        lineGauge.Parameters["THICKNESS"] = "1";
        lineGauge.Parameters["CONTRAST"] = "5";
        pipeline.Steps.Add(lineGauge);

        using OpenCvSharp.Mat source = CreateSourceIsolationImage();
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        VisionRecipeStepRunSummary blobSummary = result.Steps.FirstOrDefault(step => step.Name == "01 Blob Source Isolation")
            ?? throw new InvalidOperationException("Blob isolation summary was not produced.");
        VisionRecipeStepRunSummary lineSummary = result.Steps.FirstOrDefault(step => step.Name == "02 LineGauge ROI Isolation")
            ?? throw new InvalidOperationException("LineGauge isolation summary was not produced.");

        if (blobSummary.ResultImageChannelCount != source.Channels())
        {
            throw new InvalidOperationException(
                $"Blob result image changed source channels. Expected={source.Channels()}, Actual={blobSummary.ResultImageChannelCount}.");
        }

        if (lineSummary.ResultImageChannelCount != source.Channels())
        {
            throw new InvalidOperationException(
                $"LineGauge result image changed source channels. Expected={source.Channels()}, Actual={lineSummary.ResultImageChannelCount}.");
        }

        using OpenCvSharp.Mat matchingSource = CreateSourceIsolationImage();
        using OpenCvSharp.Mat matchingTemplate = matchingSource.SubMat(new OpenCvSharp.Rect(6, 6, 18, 18)).Clone();
        MatchingTool matchingTool = CreateMatchingToolForSourceIsolation(matchingTemplate);
        VisionToolResult matchingResult = matchingTool.Execute(matchingSource);
        if (!matchingResult.Success)
        {
            throw new InvalidOperationException($"Matching source isolation failed. {matchingResult.ErrorName}: {matchingResult.Message}");
        }

        int matchingChannels = matchingResult.ResultImage != null && !matchingResult.ResultImage.Empty()
            ? matchingResult.ResultImage.Channels()
            : 0;
        if (matchingChannels != matchingSource.Channels())
        {
            throw new InvalidOperationException(
                $"Matching result image changed source channels. Expected={matchingSource.Channels()}, Actual={matchingChannels}.");
        }

        string matchingSize = matchingResult.ResultImage != null && !matchingResult.ResultImage.Empty()
            ? $"{matchingResult.ResultImage.Width}x{matchingResult.ResultImage.Height}"
            : string.Empty;
        return $"Source image isolation: OK | Blob={blobSummary.ResultImageSizeText}, LineGauge={lineSummary.ResultImageSizeText}, Matching={matchingSize}";
    }

    private static string RunInvalidStepConfigurationContractCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_InvalidStepConfigurationContract"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 Missing Tool Type",
            ToolType = string.Empty,
            InputLayer = "Main",
            OutputLayer = "MissingTool_Output"
        };
        pipeline.Steps.Add(step);

        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            16,
            16,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(120));
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (result.Success || !result.HasFailedStep || result.FirstFailedStepIndex != 1)
        {
            throw new InvalidOperationException("Missing ToolType should fail on step 1 without completing the recipe.");
        }

        if (result.FirstFailedErrorCode != (int)VisionToolErrorCode.ToolFactoryFailed
            || !string.Equals(result.FirstFailedErrorName, VisionToolErrorCode.ToolFactoryFailed.ToString(), StringComparison.OrdinalIgnoreCase)
            || !string.Equals(result.FirstFailedResultStatus, VisionToolResultStatus.ConfigurationError.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Missing ToolType should expose ToolFactoryFailed/ConfigurationError, but got {result.FirstFailedErrorCode}:{result.FirstFailedErrorName}/{result.FirstFailedResultStatus}.");
        }

        if (!result.FirstFailedDiagnosticHint.Contains("configuration", StringComparison.OrdinalIgnoreCase)
            && !result.FirstFailedDiagnosticHint.Contains("tool", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Missing ToolType diagnostic is not actionable: {result.FirstFailedDiagnosticHint}");
        }

        if (!result.FirstFailedSuggestedFix.Contains("ToolType", StringComparison.OrdinalIgnoreCase)
            && !result.FirstFailedSuggestedFix.Contains("supported", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Missing ToolType fix is not actionable: {result.FirstFailedSuggestedFix}");
        }

        if (!result.ActionSummaryText.Contains("Fix step 01", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("01 ERROR", StringComparison.OrdinalIgnoreCase)
            || !result.StepSummaryText.Contains("ToolFactoryFailed", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Missing ToolType action/step summary should name the failed step and error.");
        }

        string reportStatus = RunReportDiagnosticContractCase(pipeline);
        return $"Invalid step configuration: OK | Missing ToolType -> ToolFactoryFailed/ConfigurationError | {reportStatus}";
    }

    private static string RunReportDiagnosticContractCase(VisionPipeline pipeline)
    {
        using VisionPipelineContext context = new();
        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            16,
            16,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(120));
        context.SetLayer("Main", source);

        DateTime startedAt = DateTime.Now;
        VisionPipelineRunResult runResult = VisionPipelineExecutionService
            .RunAsync(pipeline, context, VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
        DateTime finishedAt = DateTime.Now;

        string reportPath = VisionPipelineRunReportStorage.Save(
            "Smoke",
            pipeline,
            runResult,
            startedAt,
            finishedAt,
            false,
            "invalid_step_diagnostics");
        VisionPipelineRunReport report = VisionPipelineRunReportStorage.Load(reportPath);
        VisionPipelineStepRunReport reportStep = report?.Steps?.FirstOrDefault();

        if (reportStep == null)
        {
            throw new InvalidOperationException("Run report did not persist the failed step.");
        }

        if (!string.Equals(reportStep.ErrorName, VisionToolErrorCode.ToolFactoryFailed.ToString(), StringComparison.OrdinalIgnoreCase)
            || !string.Equals(reportStep.ResultStatus, VisionToolResultStatus.ConfigurationError.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Run report saved wrong error/status: {reportStep.ErrorName}/{reportStep.ResultStatus}.");
        }

        if (string.IsNullOrWhiteSpace(reportStep.DiagnosticHint)
            || string.IsNullOrWhiteSpace(reportStep.SuggestedFix))
        {
            throw new InvalidOperationException("Run report did not persist DiagnosticHint/SuggestedFix.");
        }

        return "RunReport diagnostics persisted";
    }

    private static MatchingTool CreateMatchingToolForSourceIsolation(OpenCvSharp.Mat template)
    {
        MatchingProperty property = new("Smoke_MatchingIsolation")
        {
            USE_THRESHOLD = true,
            THRESHOLD = 80,
            THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.Binary,
            SCORE_MIN = 0.1,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = false
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);
        return tool;
    }

    private static string RunMatchingPreprocessModeContractCase()
    {
        string[] modes = { "Raw", "Threshold", "Canny" };
        List<string> results = new();

        foreach (string mode in modes)
        {
            using OpenCvSharp.Mat source = CreateMatchingPreprocessSourceImage();
            using OpenCvSharp.Mat template = source.SubMat(new OpenCvSharp.Rect(18, 18, 24, 24)).Clone();
            MatchingTool tool = CreateMatchingToolForPreprocessMode(mode, template);

            VisionToolResult result = tool.Execute(source);
            if (!result.Success)
            {
                throw new InvalidOperationException($"{mode} failed. {result.ErrorName}: {result.Message}");
            }

            if (!result.Metrics.TryGetValue("ResultCount", out double resultCount) || resultCount < 1)
            {
                throw new InvalidOperationException($"{mode} did not produce ResultCount. Metrics={string.Join(", ", result.Metrics.Keys)}");
            }

            if (!result.Metrics.TryGetValue("ScoreMax", out double scoreMax) || scoreMax <= 0)
            {
                throw new InvalidOperationException($"{mode} did not produce ScoreMax. Metrics={string.Join(", ", result.Metrics.Keys)}");
            }

            int channels = result.ResultImage != null && !result.ResultImage.Empty()
                ? result.ResultImage.Channels()
                : 0;
            if (channels != source.Channels())
            {
                throw new InvalidOperationException($"{mode} changed source channels. Expected={source.Channels()}, Actual={channels}.");
            }

            results.Add($"{mode}=Count:{resultCount:0},Score:{scoreMax:0.#}");
        }

        return $"Matching preprocess modes: OK | {string.Join(", ", results)}";
    }

    private static string RunMatchingMultiRoiContractCase()
    {
        OpenCvSharp.Rect roi = new OpenCvSharp.Rect(50, 46, 40, 40);
        using OpenCvSharp.Mat source = CreateMatchingPreprocessSourceImage();
        using OpenCvSharp.Mat template = source.SubMat(new OpenCvSharp.Rect(56, 52, 24, 24)).Clone();

        MatchingProperty property = new("Smoke_MatchingMultiRoi")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
            SCORE_MIN = 0.01,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = false,
            USE_MULTI_ROI = true,
            CvROIS = new List<OpenCvSharp.Rect> { roi }
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Multi ROI failed. {result.ErrorName}: {result.Message}");
        }

        MatchingResult match = tool.results.FirstOrDefault()
            ?? throw new InvalidOperationException("Multi ROI did not produce any matching result.");
        if (match.Bounding.X < roi.X || match.Bounding.Y < roi.Y)
        {
            throw new InvalidOperationException(
                $"Multi ROI offset was not applied. Bounding={match.Bounding}, ROI={roi}.");
        }

        return $"Matching multi ROI: OK | Bounding=({match.Bounding.X:0},{match.Bounding.Y:0},{match.Bounding.Width:0},{match.Bounding.Height:0})";
    }

    private static string RunMatchingMultipleDetectionContractCase()
    {
        using OpenCvSharp.Mat source = CreateMatchingPreprocessSourceImage();
        using OpenCvSharp.Mat template = source.SubMat(new OpenCvSharp.Rect(18, 18, 24, 24)).Clone();

        MatchingProperty property = new("Smoke_MatchingMultiple")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCorrNormed,
            SCORE_MIN = 0.8,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 2,
            USE_FIND_ANGLE = false
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Multiple matching failed. {result.ErrorName}: {result.Message}");
        }

        if (tool.results.Count < 2)
        {
            string centers = string.Join(
                ", ",
                tool.results.Select(r => $"({r.Center.X:0.#},{r.Center.Y:0.#}) Score={r.Score:0.#}"));
            throw new InvalidOperationException($"Multiple matching should detect at least two instances. Count={tool.results.Count}. Results={centers}.");
        }

        double dx = tool.results[0].Center.X - tool.results[1].Center.X;
        double dy = tool.results[0].Center.Y - tool.results[1].Center.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        if (distance < 20)
        {
            throw new InvalidOperationException(
                $"Multiple matching repeated the same location. Distance={distance:0.#}, C1=({tool.results[0].Center.X:0.#},{tool.results[0].Center.Y:0.#}), C2=({tool.results[1].Center.X:0.#},{tool.results[1].Center.Y:0.#}).");
        }

        if (!result.Metrics.TryGetValue("ResultCount", out double resultCount) || resultCount < 2)
        {
            throw new InvalidOperationException($"Multiple matching did not expose ResultCount>=2. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        return $"Matching multiple: OK | Count={tool.results.Count}, Distance={distance:0.#}";
    }

    private static string RunMatchingSqDiffNormedContractCase()
    {
        OpenCvSharp.Rect templateRect = new OpenCvSharp.Rect(28, 34, 24, 24);
        using OpenCvSharp.Mat template = CreateMatchingRotatedTemplate();
        using OpenCvSharp.Mat source = CreateMatchingSqDiffSourceImage(templateRect, template);

        MatchingProperty property = new("Smoke_MatchingSqDiffNormed")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.SqDiffNormed,
            SCORE_MIN = 0.95,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = false
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (!result.Success)
        {
            throw new InvalidOperationException($"SqDiffNormed matching failed. {result.ErrorName}: {result.Message}");
        }

        MatchingResult match = tool.results.FirstOrDefault()
            ?? throw new InvalidOperationException("SqDiffNormed matching did not produce a result.");
        double expectedCenterX = templateRect.X + templateRect.Width / 2.0;
        double expectedCenterY = templateRect.Y + templateRect.Height / 2.0;
        double centerDx = match.Center.X - expectedCenterX;
        double centerDy = match.Center.Y - expectedCenterY;
        double centerDistance = Math.Sqrt(centerDx * centerDx + centerDy * centerDy);
        if (centerDistance > 2)
        {
            throw new InvalidOperationException($"SqDiffNormed selected wrong location. Center=({match.Center.X:0.#},{match.Center.Y:0.#}), Distance={centerDistance:0.#}.");
        }

        if (!result.Metrics.TryGetValue("ScoreMax", out double scoreMax) || scoreMax < 95)
        {
            throw new InvalidOperationException($"SqDiffNormed quality score should be high. ScoreMax={scoreMax:0.#}.");
        }

        return $"Matching SqDiffNormed: OK | Score={match.Score:0.#}, Center=({match.Center.X:0.#},{match.Center.Y:0.#})";
    }

    private static string RunMatchingAngleScaleContractCase()
    {
        using OpenCvSharp.Mat source = CreateMatchingPreprocessSourceImage();
        using OpenCvSharp.Mat template = source.SubMat(new OpenCvSharp.Rect(18, 18, 24, 24)).Clone();

        MatchingProperty property = new("Smoke_MatchingAngleScale")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
            SCORE_MIN = 0.01,
            MAGNIFIATION = 2.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = true,
            FIND_ANGLE = 5,
            FIND_ANGLE_MIN = -10,
            FIND_ANGLE_MAX = 10,
            USE_PADDING_COLOR_WHITE = false
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Angle/scale matching failed. {result.ErrorName}: {result.Message}");
        }

        MatchingResult match = tool.results.FirstOrDefault()
            ?? throw new InvalidOperationException("Angle/scale matching did not produce any result.");

        if (match.Score <= 0 || match.Bounding.Width <= 0 || match.Bounding.Height <= 0)
        {
            throw new InvalidOperationException($"Angle/scale matching produced invalid geometry. Score={match.Score:0.#}, Bounding={match.Bounding}.");
        }

        if (match.Angle < property.FIND_ANGLE_MIN || match.Angle > property.FIND_ANGLE_MAX)
        {
            throw new InvalidOperationException(
                $"Angle/scale matching result angle is outside configured range. Angle={match.Angle}, Range={property.FIND_ANGLE_MIN}..{property.FIND_ANGLE_MAX}.");
        }

        int channels = result.ResultImage != null && !result.ResultImage.Empty()
            ? result.ResultImage.Channels()
            : 0;
        if (channels != source.Channels())
        {
            throw new InvalidOperationException($"Angle/scale matching changed source channels. Expected={source.Channels()}, Actual={channels}.");
        }

        return $"Matching angle/scale: OK | Score={match.Score:0.#}, Angle={match.Angle:0.#}, Bounding=({match.Bounding.X:0},{match.Bounding.Y:0},{match.Bounding.Width:0},{match.Bounding.Height:0})";
    }

    private static string RunMatchingRotatedFixtureContractCase()
    {
        const double fixtureAngle = 15d;
        using OpenCvSharp.Mat template = CreateMatchingRotatedTemplate();
        using OpenCvSharp.Mat source = CreateMatchingRotatedFixture(template, fixtureAngle, out OpenCvSharp.Rect expectedBounds);

        MatchingProperty property = new("Smoke_MatchingRotatedFixture")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
            SCORE_MIN = -1,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = true,
            FIND_ANGLE = 5,
            FIND_ANGLE_MIN = -15,
            FIND_ANGLE_MAX = 15,
            USE_PADDING_COLOR_WHITE = false,
            USE_ROI = true,
            CvROI = new OpenCvSharp.Rect(48, 34, 76, 82)
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Rotated fixture matching failed. {result.ErrorName}: {result.Message}");
        }

        MatchingResult match = tool.results.FirstOrDefault()
            ?? throw new InvalidOperationException("Rotated fixture matching did not produce any result.");

        if (match.Score < 10)
        {
            throw new InvalidOperationException($"Rotated fixture matching score is too low. Score={match.Score:0.#}.");
        }

        if (Math.Abs(match.Angle) < 5)
        {
            throw new InvalidOperationException($"Rotated fixture matching did not use angle search. Angle={match.Angle:0.#}.");
        }

        if (!IsPointNearRect(match.Center.X, match.Center.Y, expectedBounds, 24))
        {
            throw new InvalidOperationException(
                $"Rotated fixture matching center is outside expected target. Center=({match.Center.X:0.#},{match.Center.Y:0.#}), Expected={expectedBounds}.");
        }

        if (result.Overlays.Count == 0)
        {
            throw new InvalidOperationException("Rotated fixture matching produced no overlay.");
        }

        return $"Matching rotated fixture: OK | Score={match.Score:0.#}, Angle={match.Angle:0.#}, Center=({match.Center.X:0},{match.Center.Y:0})";
    }

    private static string RunMatchingNoResultContractCase()
    {
        using OpenCvSharp.Mat source = CreateMatchingPreprocessSourceImage();
        using OpenCvSharp.Mat template = source.SubMat(new OpenCvSharp.Rect(18, 18, 24, 24)).Clone();

        MatchingProperty property = new("Smoke_MatchingNoResult")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
            SCORE_MIN = 1.1,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = false
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);

        VisionToolResult result = tool.Execute(source);
        if (result.Success)
        {
            throw new InvalidOperationException("Matching should fail when no candidate satisfies ScoreMin.");
        }

        if (result.ErrorCode != VisionToolErrorCode.MatchingNoResult)
        {
            throw new InvalidOperationException($"Matching no-result returned wrong error. Error={result.ErrorCode}:{result.ErrorName}, Message={result.Message}");
        }

        if (tool.results.Count != 0 || result.Overlays.Count != 0)
        {
            throw new InvalidOperationException($"Matching no-result produced unexpected results. Results={tool.results.Count}, Overlays={result.Overlays.Count}.");
        }

        ValidateFailureDebugDetails("Matching no-result", result, source.Width, source.Height, source.Channels());
        return "Matching no result: OK | Error=MatchingNoResult";
    }

    private static string RunFeatureMatchingContractCase()
    {
        using OpenCvSharp.Mat source = CreateFeatureMatchingSourceImage(
            out OpenCvSharp.Rect templateRect,
            out OpenCvSharp.Rect roi);
        using OpenCvSharp.Mat template = source.SubMat(templateRect).Clone();

        OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty singleProperty =
            CreateFeatureMatchingProperty("Smoke_FeatureSingle", false, false, new OpenCvSharp.Rect());
        SiftTool singleTool = CreateFeatureMatchingTool(singleProperty, template);
        VisionToolResult singleResult = singleTool.Execute(source);
        ValidateFeatureMatchingResult("Single", singleResult, singleTool, source.Channels(), templateRect);

        if (singleProperty.CvROI.Width != 0 || singleProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Single fallback ROI mutated property. ROI={singleProperty.CvROI}.");
        }

        OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty multiProperty =
            CreateFeatureMatchingProperty("Smoke_FeatureMulti", false, true, roi);
        SiftTool multiTool = CreateFeatureMatchingTool(multiProperty, template);
        VisionToolResult multiResult = multiTool.Execute(source);
        ValidateFeatureMatchingResult("Multi", multiResult, multiTool, source.Channels(), templateRect);

        MatchingResult multiMatch = multiTool.results.First();
        if (multiMatch.Center.X < roi.X || multiMatch.Center.Y < roi.Y)
        {
            throw new InvalidOperationException(
                $"Multi ROI offset was not applied. Center=({multiMatch.Center.X:0.#},{multiMatch.Center.Y:0.#}), ROI={roi}.");
        }

        return $"FeatureMatching: OK | SingleScore={singleResult.Metrics["ScoreMax"]:0.#}, MultiCenter=({multiMatch.Center.X:0},{multiMatch.Center.Y:0})";
    }

    private static string RunFeatureMatchingNoFalsePositiveContractCase()
    {
        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            96,
            96,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(120, 120, 120));
        using OpenCvSharp.Mat template = new OpenCvSharp.Mat(
            32,
            32,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(120, 120, 120));

        OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty property =
            CreateFeatureMatchingProperty("Smoke_FeatureNoFalsePositive", false, false, new OpenCvSharp.Rect());
        SiftTool tool = CreateFeatureMatchingTool(property, template);
        VisionToolResult result = tool.Execute(source);
        if (result.Success)
        {
            throw new InvalidOperationException("FeatureMatching blank template should fail with a no-detection error.");
        }

        if (result.ErrorCode != VisionToolErrorCode.FeatureNoKeypoints)
        {
            throw new InvalidOperationException($"FeatureMatching blank template returned wrong error. Error={result.ErrorCode}:{result.ErrorName}, Message={result.Message}");
        }

        if (tool.results.Count != 0 || result.Overlays.Any(overlay => overlay.Kind == VisionToolOverlayKind.Rectangle))
        {
            throw new InvalidOperationException($"FeatureMatching blank template produced false positives. Results={tool.results.Count}, Overlays={result.Overlays.Count}.");
        }

        ValidateFailureDebugDetails("FeatureMatching no-keypoints", result, source.Width, source.Height, source.Channels());

        string templatePath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_FeatureBlankTemplate.png");
        OpenCvSharp.Cv2.ImWrite(templatePath, template);
        VisionPipeline pipeline = new()
        {
            Name = "FeatureNoDetectionMessage"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 Feature No Detection",
            ToolType = "FeatureMatching",
            InputLayer = "Main",
            OutputLayer = "Feature_NoDetection",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1
        };
        step.Parameters["TemplatePath"] = templatePath;
        step.Parameters["PATTERN_PATH"] = templatePath;
        step.Parameters["SCORE_MIN"] = "0.95";
        step.Parameters["RANSAC_REPROJ_THRESHOLD"] = "5";
        pipeline.Steps.Add(step);

        using VisionRecipeRunResult recipeResult = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
        VisionRecipeStepRunSummary summary = recipeResult.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException("FeatureMatching no-detection recipe produced no step summary.");
        if (recipeResult.Success || summary.ErrorCode != (int)VisionToolErrorCode.FeatureNoKeypoints)
        {
            throw new InvalidOperationException($"FeatureMatching no-detection error was not actionable. Success={recipeResult.Success}, Error={summary.ErrorCode}:{summary.ErrorName}, Message='{summary.Message}'.");
        }

        return "FeatureMatching no false positive: OK | Error=FeatureNoKeypoints";
    }

    private static void ValidateFailureDebugDetails(
        string name,
        VisionToolResult result,
        int expectedWidth,
        int expectedHeight,
        int expectedChannels)
    {
        if (result.Success)
        {
            throw new InvalidOperationException($"{name} expected a failed result.");
        }

        ValidateResultImageMetrics(name, result, expectedWidth, expectedHeight, expectedChannels);
        if (!result.Metrics.TryGetValue("ResultCount", out double resultCount) || Math.Abs(resultCount) > 0.000001)
        {
            throw new InvalidOperationException($"{name} should expose ResultCount=0 for debugging. Value={resultCount:0.###}.");
        }
    }

    private static OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty CreateFeatureMatchingProperty(
        string name,
        bool useRoi,
        bool useMultiRoi,
        OpenCvSharp.Rect roi)
    {
        return new OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty(name)
        {
            SCORE_MIN = 0.95,
            RANSAC_REPROJ_THRESHOLD = 5,
            USE_ROI = useRoi,
            USE_MULTI_ROI = useMultiRoi,
            CvROI = roi,
            CvROIS = new List<OpenCvSharp.Rect> { roi }
        };
    }

    private static SiftTool CreateFeatureMatchingTool(
        OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty property,
        OpenCvSharp.Mat template)
    {
        SiftTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);
        return tool;
    }

    private static void ValidateFeatureMatchingResult(
        string mode,
        VisionToolResult result,
        SiftTool tool,
        int expectedChannels,
        OpenCvSharp.Rect expectedBounds)
    {
        if (!result.Success)
        {
            throw new InvalidOperationException($"{mode} failed. {result.ErrorName}: {result.Message}");
        }

        if (!result.Metrics.TryGetValue("ResultCount", out double resultCount) || resultCount < 1)
        {
            throw new InvalidOperationException($"{mode} did not produce ResultCount. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        if (!result.Metrics.TryGetValue("ScoreMax", out double scoreMax) || scoreMax <= 0)
        {
            throw new InvalidOperationException($"{mode} did not produce ScoreMax. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        MatchingResult match = tool.results.FirstOrDefault()
            ?? throw new InvalidOperationException($"{mode} did not produce any feature result.");
        if (match.Index != 1)
        {
            throw new InvalidOperationException($"{mode} result index should start at 1. Actual={match.Index}.");
        }

        ValidateFeatureBounding(mode, match, expectedBounds);

        int channels = result.ResultImage != null && !result.ResultImage.Empty()
            ? result.ResultImage.Channels()
            : 0;
        if (channels != expectedChannels)
        {
            throw new InvalidOperationException($"{mode} changed source channels. Expected={expectedChannels}, Actual={channels}.");
        }
    }

    private static void ValidateFeatureBounding(string mode, MatchingResult match, OpenCvSharp.Rect expectedBounds)
    {
        const float tolerance = 28f;
        RectangleF bounds = match.Bounding;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new InvalidOperationException($"{mode} produced invalid bounding size. Bounding={bounds}.");
        }

        if (!bounds.Contains(match.Center.X, match.Center.Y))
        {
            throw new InvalidOperationException(
                $"{mode} bounding does not contain center. Bounding={bounds}, Center=({match.Center.X:0.#},{match.Center.Y:0.#}).");
        }

        if (Math.Abs(bounds.X - expectedBounds.X) > tolerance
            || Math.Abs(bounds.Y - expectedBounds.Y) > tolerance
            || Math.Abs(bounds.Width - expectedBounds.Width) > tolerance
            || Math.Abs(bounds.Height - expectedBounds.Height) > tolerance)
        {
            throw new InvalidOperationException(
                $"{mode} bounding is outside expected tolerance. Expected={expectedBounds}, Actual={bounds}.");
        }
    }

    private static bool IsPointNearRect(float x, float y, OpenCvSharp.Rect bounds, float tolerance)
    {
        return x >= bounds.X - tolerance
            && x <= bounds.X + bounds.Width + tolerance
            && y >= bounds.Y - tolerance
            && y <= bounds.Y + bounds.Height + tolerance;
    }

    private static string RunRectangleOverlayGeometryContractCase()
    {
        List<string> summaries = new();

        using OpenCvSharp.Mat contourBlobSource = CreateRectangleGeometrySourceImage();
        ContourProperty contourProperty = new("Smoke_GeometryContour")
        {
            MIN_AREA = 100,
            MAX_AREA = 50000,
            DetectMode = OpenCvSharp.RetrievalModes.External
        };
        ContourTool contourTool = new();
        contourTool.SetProperty(contourProperty);
        VisionToolResult contourResult = contourTool.Execute(contourBlobSource);
        summaries.Add(ValidateRectangleOverlayGeometry("Contour", contourResult, contourBlobSource.Channels()));
        if (contourProperty.CvROI.Width != 0 || contourProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Contour fallback ROI mutated property. ROI={contourProperty.CvROI}.");
        }

        BlobProperty blobProperty = new("Smoke_GeometryBlob")
        {
            MIN_AREA = 100,
            MAX_AREA = 50000
        };
        BlobTool blobTool = new();
        blobTool.SetProperty(blobProperty);
        VisionToolResult blobResult = blobTool.Execute(contourBlobSource);
        summaries.Add(ValidateRectangleOverlayGeometry("Blob", blobResult, contourBlobSource.Channels()));
        if (blobProperty.CvROI.Width != 0 || blobProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Blob fallback ROI mutated property. ROI={blobProperty.CvROI}.");
        }

        using OpenCvSharp.Mat matchingSource = CreateMatchingPreprocessSourceImage();
        using OpenCvSharp.Mat matchingTemplate = matchingSource.SubMat(new OpenCvSharp.Rect(18, 18, 24, 24)).Clone();
        MatchingTool matchingTool = CreateMatchingToolForPreprocessMode("Raw", matchingTemplate);
        VisionToolResult matchingResult = matchingTool.Execute(matchingSource);
        summaries.Add(ValidateRectangleOverlayGeometry("Matching", matchingResult, matchingSource.Channels()));

        using OpenCvSharp.Mat featureSource = CreateFeatureMatchingSourceImage(
            out OpenCvSharp.Rect featureTemplateRect,
            out OpenCvSharp.Rect featureRoi);
        using OpenCvSharp.Mat featureTemplate = featureSource.SubMat(featureTemplateRect).Clone();
        SiftTool featureTool = CreateFeatureMatchingTool(
            CreateFeatureMatchingProperty("Smoke_GeometryFeature", false, true, featureRoi),
            featureTemplate);
        VisionToolResult featureResult = featureTool.Execute(featureSource);
        summaries.Add(ValidateRectangleOverlayGeometry("Feature", featureResult, featureSource.Channels()));

        return $"Rectangle overlay geometry: OK | {string.Join(", ", summaries)}";
    }

    private static string RunContourOptionContractCase()
    {
        using OpenCvSharp.Mat source = CreateContourOptionSourceImage();
        List<string> summaries = new();

        ContourProperty singleProperty = CreateContourContractProperty("Smoke_ContourSingle");
        ContourTool singleTool = new();
        singleTool.SetProperty(singleProperty);
        VisionToolResult singleResult = singleTool.Execute(source);
        summaries.Add(ValidateRectangleOverlayGeometry("ContourSingle", singleResult, source.Channels()));
        ValidateAreaMetrics("ContourSingle", singleResult, singleProperty.MIN_AREA, singleProperty.MAX_AREA);
        if (singleProperty.CvROI.Width != 0 || singleProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Contour single fallback ROI mutated property. ROI={singleProperty.CvROI}.");
        }

        ContourProperty approxProperty = CreateContourContractProperty("Smoke_ContourApprox");
        approxProperty.USE_APPROXPOLYDP = true;
        approxProperty.EPSILON = 0.02;
        ContourTool approxTool = new();
        approxTool.SetProperty(approxProperty);
        VisionToolResult approxResult = approxTool.Execute(source);
        summaries.Add(ValidateRectangleOverlayGeometry("ContourApprox", approxResult, source.Channels()));
        ValidateAreaMetrics("ContourApprox", approxResult, approxProperty.MIN_AREA, approxProperty.MAX_AREA);

        ContourProperty drawProperty = CreateContourContractProperty("Smoke_ContourDraw");
        drawProperty.USE_DRAW_IMAGE = true;
        drawProperty.DrawThickness = 2;
        ContourTool drawTool = new();
        drawTool.SetProperty(drawProperty);
        VisionToolResult drawResult = drawTool.Execute(source);
        summaries.Add(ValidateRectangleOverlayGeometry("ContourDraw", drawResult, 3));
        ValidateAreaMetrics("ContourDraw", drawResult, drawProperty.MIN_AREA, drawProperty.MAX_AREA);

        ContourProperty multiProperty = CreateContourContractProperty("Smoke_ContourMulti");
        multiProperty.USE_MULTI_ROI = true;
        multiProperty.USE_ROI = false;
        multiProperty.CvROIS = new List<OpenCvSharp.Rect>
        {
            new OpenCvSharp.Rect(0, 0, 0, 0),
            new OpenCvSharp.Rect(66, 40, 66, 52)
        };
        ContourTool multiTool = new();
        multiTool.SetProperty(multiProperty);
        VisionToolResult multiResult = multiTool.Execute(source);
        summaries.Add(ValidateRectangleOverlayGeometry("ContourMulti", multiResult, source.Channels()));
        ValidateAreaMetrics("ContourMulti", multiResult, multiProperty.MIN_AREA, multiProperty.MAX_AREA);
        if (multiProperty.CvROIS[0].Width != 0 || multiProperty.CvROIS[0].Height != 0)
        {
            throw new InvalidOperationException($"Contour multi fallback ROI mutated property. ROI={multiProperty.CvROIS[0]}.");
        }

        return $"Contour options: OK | {string.Join(", ", summaries)}";
    }

    private static string RunContourSquareContractCase()
    {
        using OpenCvSharp.Mat source = CreateContourSquareSourceImage();
        ContourProperty property = CreateContourContractProperty("Smoke_ContourSquare");
        property.MIN_AREA = 200;
        property.MAX_AREA = 5000;
        property.EPSILON = 0.03;
        property.USE_THRESHOLD = true;
        property.THRESHOLD = 120;
        property.THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.Binary;

        ContourTool tool = new();
        tool.SetProperty(property);
        tool.SetSourceImage(source);

        if (!tool.SquareRun())
        {
            throw new InvalidOperationException("Contour SquareRun returned false.");
        }

        if (tool.results.Count != 2)
        {
            throw new InvalidOperationException($"Contour SquareRun expected 2 square results, but got {tool.results.Count}.");
        }

        for (int i = 0; i < tool.results.Count; i++)
        {
            if (tool.results[i].Index != i + 1)
            {
                throw new InvalidOperationException($"Contour SquareRun result index should be sequential from 1. Position={i}, Index={tool.results[i].Index}.");
            }

            if (tool.results[i].Bounding.Width <= 0 || tool.results[i].Bounding.Height <= 0)
            {
                throw new InvalidOperationException($"Contour SquareRun produced invalid bounds. Index={tool.results[i].Index}, Bounds={tool.results[i].Bounding}.");
            }
        }

        if (tool.imageResult.Empty() || tool.imageResult.Channels() != 3)
        {
            throw new InvalidOperationException($"Contour SquareRun should produce a 3-channel draw image. Channels={tool.imageResult.Channels()}.");
        }

        if (property.CvROI.Width != 0 || property.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Contour SquareRun fallback ROI mutated property. ROI={property.CvROI}.");
        }

        return $"Contour SquareRun: OK | Results={tool.results.Count}, Image={tool.imageResult.Width}x{tool.imageResult.Height}x{tool.imageResult.Channels()}";
    }

    private static string RunBlobOptionContractCase()
    {
        using OpenCvSharp.Mat source = CreateBlobOptionSourceImage();
        List<string> summaries = new();

        BlobProperty singleProperty = CreateBlobContractProperty("Smoke_BlobSingle");
        BlobTool singleTool = new();
        singleTool.SetProperty(singleProperty);
        VisionToolResult singleResult = singleTool.Execute(source);
        summaries.Add(ValidateBlobContract("BlobSingle", singleResult, singleTool, source.Channels(), 3));
        if (singleProperty.CvROI.Width != 0 || singleProperty.CvROI.Height != 0)
        {
            throw new InvalidOperationException($"Blob single fallback ROI mutated property. ROI={singleProperty.CvROI}.");
        }

        BlobProperty maskProperty = CreateBlobContractProperty("Smoke_BlobMask");
        maskProperty.CvMASKS = new List<OpenCvSharp.Rect>
        {
            new OpenCvSharp.Rect(60, 16, 44, 34)
        };
        BlobTool maskTool = new();
        maskTool.SetProperty(maskProperty);
        VisionToolResult maskResult = maskTool.Execute(source);
        summaries.Add(ValidateBlobContract("BlobMask", maskResult, maskTool, source.Channels(), 2));

        BlobProperty multiProperty = CreateBlobContractProperty("Smoke_BlobMulti");
        multiProperty.USE_ROI = false;
        multiProperty.USE_MULTI_ROI = true;
        multiProperty.CvROIS = new List<OpenCvSharp.Rect>
        {
            new OpenCvSharp.Rect(0, 0, 100, 60),
            new OpenCvSharp.Rect(100, 52, 48, 48)
        };
        BlobTool multiTool = new();
        multiTool.SetProperty(multiProperty);
        VisionToolResult multiResult = multiTool.Execute(source);
        summaries.Add(ValidateBlobContract("BlobMulti", multiResult, multiTool, source.Channels(), 3));
        if (!multiTool.results.Any(result => result.Bounding.X >= 100 && result.Center.X >= 100))
        {
            throw new InvalidOperationException("Blob multi ROI result did not preserve full-image coordinates.");
        }

        using OpenCvSharp.Mat thresholdSource = CreateBlobThresholdSourceImage();
        BlobProperty thresholdProperty = CreateBlobContractProperty("Smoke_BlobThreshold");
        thresholdProperty.USE_THRESHOLD = true;
        thresholdProperty.THRESHOLD = 100;
        thresholdProperty.THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.BinaryInv;
        BlobTool thresholdTool = new();
        thresholdTool.SetProperty(thresholdProperty);
        VisionToolResult thresholdResult = thresholdTool.Execute(thresholdSource);
        summaries.Add(ValidateBlobContract("BlobThreshold", thresholdResult, thresholdTool, thresholdSource.Channels(), 2));

        return $"Blob options: OK | {string.Join(", ", summaries)}";
    }

    private static ContourProperty CreateContourContractProperty(string name)
    {
        return new ContourProperty(name)
        {
            MIN_AREA = 100,
            MAX_AREA = 50000,
            DetectMode = OpenCvSharp.RetrievalModes.External,
            ApproximationModes = OpenCvSharp.ContourApproximationModes.ApproxSimple
        };
    }

    private static BlobProperty CreateBlobContractProperty(string name)
    {
        return new BlobProperty(name)
        {
            USE_THRESHOLD = false,
            USE_ADAPTIVE_THRESHOLD = false,
            USE_BITWISENOT = false,
            USE_ROI = false,
            USE_MULTI_ROI = false,
            MIN_AREA = 250,
            MAX_AREA = 900
        };
    }

    private static string ValidateBlobContract(
        string name,
        VisionToolResult result,
        BlobTool tool,
        int expectedChannels,
        int expectedCount)
    {
        string geometrySummary = ValidateRectangleOverlayGeometry(name, result, expectedChannels);
        if (tool.results.Count != expectedCount)
        {
            throw new InvalidOperationException($"{name} expected {expectedCount} blobs, but got {tool.results.Count}.");
        }

        for (int i = 0; i < tool.results.Count; i++)
        {
            if (tool.results[i].Index != i + 1)
            {
                throw new InvalidOperationException($"{name} blob index should be sequential from 1. Position={i}, Index={tool.results[i].Index}.");
            }
        }

        ValidateAreaMetrics(name, result, 250, 900);

        return $"{geometrySummary}/Expected={expectedCount}";
    }

    private static void ValidateAreaMetrics(string name, VisionToolResult result, double minArea, double maxArea)
    {
        if (!result.Metrics.TryGetValue("AreaMin", out double areaMin)
            || !result.Metrics.TryGetValue("AreaMax", out double areaMax)
            || !result.Metrics.TryGetValue("AreaAvg", out double areaAvg))
        {
            throw new InvalidOperationException($"{name} did not expose AreaMin/AreaMax/AreaAvg. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        if (areaMin < minArea || areaMax > maxArea || areaAvg < minArea || areaAvg > maxArea)
        {
            throw new InvalidOperationException($"{name} area metrics are outside the filter range. Min={areaMin}, Max={areaMax}, Avg={areaAvg}, Range={minArea}..{maxArea}.");
        }
    }

    private static string ValidateRectangleOverlayGeometry(string name, VisionToolResult result, int expectedChannels)
    {
        if (!result.Success)
        {
            throw new InvalidOperationException($"{name} failed. {result.ErrorName}: {result.Message}");
        }

        if (!result.Metrics.TryGetValue("ResultCount", out double resultCount) || resultCount < 1)
        {
            throw new InvalidOperationException($"{name} did not produce ResultCount. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        OpenCvSharp.Mat resultImage = result.ResultImage
            ?? throw new InvalidOperationException($"{name} did not produce a result image.");
        if (resultImage.Empty())
        {
            throw new InvalidOperationException($"{name} produced an empty result image.");
        }

        int channels = resultImage.Channels();
        if (channels != expectedChannels)
        {
            throw new InvalidOperationException($"{name} changed source channels. Expected={expectedChannels}, Actual={channels}.");
        }

        ValidateResultImageMetrics(name, result, resultImage.Width, resultImage.Height, channels);

        List<VisionToolOverlay> overlays = result.Overlays
            .Where(overlay => overlay.Kind == VisionToolOverlayKind.Rectangle)
            .ToList();
        if (overlays.Count < (int)resultCount)
        {
            throw new InvalidOperationException($"{name} rectangle overlays are missing. ResultCount={resultCount:0}, Overlays={overlays.Count}.");
        }

        VisionToolOverlay first = overlays[0];
        if (!first.Label.StartsWith("#1", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{name} first overlay label should start with #1. Label={first.Label}.");
        }

        foreach (VisionToolOverlay overlay in overlays)
        {
            if (overlay.Bounds.Width <= 0 || overlay.Bounds.Height <= 0)
            {
                throw new InvalidOperationException($"{name} overlay has invalid bounds. Label={overlay.Label}, Bounds={overlay.Bounds}.");
            }

            if (!overlay.Bounds.Contains(overlay.Center))
            {
                throw new InvalidOperationException(
                    $"{name} overlay center is outside bounds. Label={overlay.Label}, Bounds={overlay.Bounds}, Center={overlay.Center}.");
            }
        }

        return $"{name}:{resultCount:0}";
    }

    private static void ValidateStepImageMetrics(
        string name,
        VisionRecipeStepRunSummary summary,
        int expectedWidth,
        int expectedHeight,
        int expectedChannels)
    {
        if (!summary.Metrics.TryGetValue("ResultImageWidth", out double width)
            || Math.Abs(width - expectedWidth) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageWidth metric mismatch. Expected={expectedWidth}, Actual={width:0.###}.");
        }

        if (!summary.Metrics.TryGetValue("ResultImageHeight", out double height)
            || Math.Abs(height - expectedHeight) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageHeight metric mismatch. Expected={expectedHeight}, Actual={height:0.###}.");
        }

        if (!summary.Metrics.TryGetValue("ResultImageChannels", out double channels)
            || Math.Abs(channels - expectedChannels) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageChannels metric mismatch. Expected={expectedChannels}, Actual={channels:0.###}.");
        }
    }

    private static void ValidateResultImageMetrics(
        string name,
        VisionToolResult result,
        int expectedWidth,
        int expectedHeight,
        int expectedChannels)
    {
        if (!result.Metrics.TryGetValue("ResultImageWidth", out double width)
            || Math.Abs(width - expectedWidth) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageWidth metric mismatch. Expected={expectedWidth}, Actual={width:0.###}.");
        }

        if (!result.Metrics.TryGetValue("ResultImageHeight", out double height)
            || Math.Abs(height - expectedHeight) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageHeight metric mismatch. Expected={expectedHeight}, Actual={height:0.###}.");
        }

        if (!result.Metrics.TryGetValue("ResultImageChannels", out double channels)
            || Math.Abs(channels - expectedChannels) > 0.001d)
        {
            throw new InvalidOperationException($"{name} ResultImageChannels metric mismatch. Expected={expectedChannels}, Actual={channels:0.###}.");
        }
    }

    private static OpenCvSharp.Mat CreateRectangleGeometrySourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            120,
            140,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(0));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(18, 22, 28, 34), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(74, 48, 42, 30), OpenCvSharp.Scalar.All(255), -1);
        return image;
    }

    private static OpenCvSharp.Mat CreateContourOptionSourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            120,
            140,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(0));

        OpenCvSharp.Point[] polygon =
        {
            new OpenCvSharp.Point(18, 18),
            new OpenCvSharp.Point(46, 20),
            new OpenCvSharp.Point(55, 45),
            new OpenCvSharp.Point(39, 67),
            new OpenCvSharp.Point(14, 56)
        };
        OpenCvSharp.Cv2.FillPoly(image, new[] { polygon }, OpenCvSharp.Scalar.All(255));
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(82, 50, 34, 28), OpenCvSharp.Scalar.All(255), -1);
        return image;
    }

    private static OpenCvSharp.Mat CreateContourSquareSourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            120,
            160,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(0));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(18, 18, 32, 32), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(92, 28, 30, 30), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Point[] triangle =
        {
            new OpenCvSharp.Point(36, 86),
            new OpenCvSharp.Point(72, 86),
            new OpenCvSharp.Point(54, 108)
        };
        OpenCvSharp.Cv2.FillPoly(image, new[] { triangle }, OpenCvSharp.Scalar.All(255));
        return image;
    }

    private static OpenCvSharp.Mat CreateBlobOptionSourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            120,
            160,
            OpenCvSharp.MatType.CV_8UC3,
            OpenCvSharp.Scalar.All(0));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(12, 16, 22, 18), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(66, 20, 30, 24), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(112, 62, 24, 28), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(20, 82, 8, 8), OpenCvSharp.Scalar.All(255), -1);
        return image;
    }

    private static OpenCvSharp.Mat CreateBlobThresholdSourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            96,
            128,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(220));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(14, 18, 24, 20), OpenCvSharp.Scalar.All(30), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(74, 42, 28, 22), OpenCvSharp.Scalar.All(30), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(8, 78, 8, 8), OpenCvSharp.Scalar.All(30), -1);
        return image;
    }

    private static MatchingTool CreateMatchingToolForPreprocessMode(string mode, OpenCvSharp.Mat template)
    {
        MatchingProperty property = new($"Smoke_Matching_{mode}")
        {
            MATCH_MODE = OpenCvSharp.TemplateMatchModes.CCoeffNormed,
            SCORE_MIN = 0.01,
            MAGNIFIATION = 1.0,
            NUM_MATCH = 1,
            USE_FIND_ANGLE = false,
            USE_THRESHOLD = string.Equals(mode, "Threshold", StringComparison.OrdinalIgnoreCase),
            THRESHOLD = 90,
            THRESHOLD_TYPES = OpenCvSharp.ThresholdTypes.Binary,
            USE_CANNY = string.Equals(mode, "Canny", StringComparison.OrdinalIgnoreCase),
            CANNY_LOW = 30,
            CANNY_HIGH = 120
        };

        MatchingTool tool = new();
        tool.SetProperty(property);
        tool.SetTemplateImage(template);
        return tool;
    }

    private static OpenCvSharp.Mat CreateMatchingPreprocessSourceImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            96,
            96,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(30, 35, 40));

        DrawMatchingPattern(image, 18, 18);
        DrawMatchingPattern(image, 56, 52);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(4, 70, 18, 10), new OpenCvSharp.Scalar(80, 120, 180), -1);
        OpenCvSharp.Cv2.Circle(image, new OpenCvSharp.Point(74, 20), 8, new OpenCvSharp.Scalar(160, 80, 60), -1);
        return image;
    }

    private static OpenCvSharp.Mat CreateMatchingSqDiffSourceImage(OpenCvSharp.Rect templateRect, OpenCvSharp.Mat template)
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            96,
            96,
            OpenCvSharp.MatType.CV_8UC3,
            OpenCvSharp.Scalar.Black);

        template.CopyTo(image.SubMat(templateRect));
        OpenCvSharp.Cv2.Circle(image, new OpenCvSharp.Point(72, 18), 7, new OpenCvSharp.Scalar(90, 120, 160), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(8, 70, 16, 12), new OpenCvSharp.Scalar(150, 90, 60), -1);
        return image;
    }

    private static OpenCvSharp.Mat CreateMatchingRotatedTemplate()
    {
        OpenCvSharp.Mat template = new OpenCvSharp.Mat(
            24,
            24,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(30, 35, 40));

        OpenCvSharp.Cv2.Rectangle(template, new OpenCvSharp.Rect(2, 2, 17, 5), new OpenCvSharp.Scalar(235, 235, 235), -1);
        OpenCvSharp.Cv2.Rectangle(template, new OpenCvSharp.Rect(2, 2, 5, 17), new OpenCvSharp.Scalar(235, 235, 235), -1);
        OpenCvSharp.Cv2.Line(template, new OpenCvSharp.Point(7, 19), new OpenCvSharp.Point(21, 6), new OpenCvSharp.Scalar(205, 205, 205), 2);
        OpenCvSharp.Cv2.Rectangle(template, new OpenCvSharp.Rect(12, 10, 8, 4), new OpenCvSharp.Scalar(85, 85, 85), -1);
        OpenCvSharp.Cv2.Circle(template, new OpenCvSharp.Point(18, 18), 3, new OpenCvSharp.Scalar(10, 10, 10), -1);
        return template;
    }

    private static OpenCvSharp.Mat CreateMatchingRotatedFixture(
        OpenCvSharp.Mat template,
        double angle,
        out OpenCvSharp.Rect expectedBounds)
    {
        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            150,
            170,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(30, 35, 40));

        expectedBounds = new OpenCvSharp.Rect(
            source.Width / 2 - template.Width / 2,
            source.Height / 2 - template.Height / 2,
            template.Width,
            template.Height);
        template.CopyTo(source.SubMat(expectedBounds));
        OpenCvSharp.Cv2.Circle(source, new OpenCvSharp.Point(142, 24), 9, new OpenCvSharp.Scalar(80, 110, 170), -1);
        using OpenCvSharp.Mat rotated = RotateFixturePatch(source, angle);
        return rotated.Clone();
    }

    private static OpenCvSharp.Mat RotateFixturePatch(OpenCvSharp.Mat image, double angle)
    {
        OpenCvSharp.Mat rotated = new OpenCvSharp.Mat(image.Size(), image.Type());
        OpenCvSharp.Point2f center = new OpenCvSharp.Point2f(image.Width / 2f, image.Height / 2f);
        using OpenCvSharp.Mat matrix = OpenCvSharp.Cv2.GetRotationMatrix2D(center, angle, 1.0);
        OpenCvSharp.Cv2.WarpAffine(
            image,
            rotated,
            matrix,
            image.Size(),
            OpenCvSharp.InterpolationFlags.Linear,
            OpenCvSharp.BorderTypes.Constant,
            new OpenCvSharp.Scalar(30, 35, 40));
        return rotated;
    }

    private static void DrawMatchingPattern(OpenCvSharp.Mat image, int x, int y)
    {
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(x, y, 24, 24), new OpenCvSharp.Scalar(230, 230, 230), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(x + 3, y + 3, 18, 18), new OpenCvSharp.Scalar(45, 45, 45), 2);
        OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(x + 5, y + 18), new OpenCvSharp.Point(x + 18, y + 5), new OpenCvSharp.Scalar(250, 250, 250), 2);
        OpenCvSharp.Cv2.Circle(image, new OpenCvSharp.Point(x + 16, y + 16), 3, new OpenCvSharp.Scalar(20, 20, 20), -1);
    }

    private static OpenCvSharp.Mat CreateFeatureMatchingSourceImage(
        out OpenCvSharp.Rect templateRect,
        out OpenCvSharp.Rect roi)
    {
        templateRect = new OpenCvSharp.Rect(72, 48, 128, 128);
        roi = new OpenCvSharp.Rect(56, 34, 156, 146);

        OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            220,
            270,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(32, 36, 40));

        using OpenCvSharp.Mat template = CreateFeatureMatchingTemplatePattern();
        using OpenCvSharp.Mat templateColor = new OpenCvSharp.Mat();
        OpenCvSharp.Cv2.CvtColor(template, templateColor, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
        templateColor.CopyTo(source.SubMat(templateRect));

        OpenCvSharp.Cv2.PutText(
            source,
            "NOISE",
            new OpenCvSharp.Point(8, 28),
            OpenCvSharp.HersheyFonts.HersheySimplex,
            0.7,
            new OpenCvSharp.Scalar(90, 90, 90),
            1);
        OpenCvSharp.Cv2.Line(source, new OpenCvSharp.Point(15, 165), new OpenCvSharp.Point(210, 170), new OpenCvSharp.Scalar(70, 120, 160), 2);
        OpenCvSharp.Cv2.Circle(source, new OpenCvSharp.Point(208, 38), 12, new OpenCvSharp.Scalar(120, 80, 60), -1);
        return source;
    }

    private static OpenCvSharp.Mat CreateFeatureMatchingTemplatePattern()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            128,
            128,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(80));

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int value = (x * 37 + y * 53 + (x * y) % 97) % 256;
                image.Set(y, x, (byte)value);
            }
        }

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(5, 5, 118, 118), OpenCvSharp.Scalar.All(215), 2);
        OpenCvSharp.Cv2.PutText(image, "OVL", new OpenCvSharp.Point(16, 44), OpenCvSharp.HersheyFonts.HersheySimplex, 1.1, OpenCvSharp.Scalar.All(245), 2);
        OpenCvSharp.Cv2.PutText(image, "SIFT", new OpenCvSharp.Point(18, 92), OpenCvSharp.HersheyFonts.HersheySimplex, 0.9, OpenCvSharp.Scalar.All(30), 2);
        OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(10, 110), new OpenCvSharp.Point(114, 18), OpenCvSharp.Scalar.All(250), 2);
        OpenCvSharp.Cv2.Circle(image, new OpenCvSharp.Point(94, 88), 13, OpenCvSharp.Scalar.All(245), 2);
        OpenCvSharp.Cv2.Circle(image, new OpenCvSharp.Point(34, 74), 8, OpenCvSharp.Scalar.All(15), -1);
        OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(14, 24), new OpenCvSharp.Point(112, 30), OpenCvSharp.Scalar.All(250), 1);
        OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(26, 12), new OpenCvSharp.Point(28, 116), OpenCvSharp.Scalar.All(25), 1);

        OpenCvSharp.Point[] points =
        {
            new OpenCvSharp.Point(92, 12),
            new OpenCvSharp.Point(116, 45),
            new OpenCvSharp.Point(78, 60),
            new OpenCvSharp.Point(54, 16)
        };
        OpenCvSharp.Cv2.Polylines(image, new[] { points }, true, OpenCvSharp.Scalar.All(80), 2);
        return image;
    }

    private static OpenCvSharp.Mat CreateSourceIsolationImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            32,
            32,
            OpenCvSharp.MatType.CV_8UC3,
            new OpenCvSharp.Scalar(20, 40, 60));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(8, 8, 12, 12), new OpenCvSharp.Scalar(220, 220, 220), -1);
        OpenCvSharp.Cv2.Line(image, new OpenCvSharp.Point(2, 24), new OpenCvSharp.Point(29, 24), new OpenCvSharp.Scalar(250, 250, 250), 2);
        return image;
    }

    private static string RunLineGaugeDirectionContractCase()
    {
        using OpenCvSharp.Mat source = CreateLineGaugeDirectionImage();
        PROJECTION_DIR[] directions =
        {
            PROJECTION_DIR.X_LTOR,
            PROJECTION_DIR.X_RTOL,
            PROJECTION_DIR.Y_TTOB,
            PROJECTION_DIR.Y_BTOT
        };

        List<string> results = new();
        foreach (PROJECTION_DIR direction in directions)
        {
            LineGaugeTool tool = CreateLineGaugeToolForContract(direction, false);
            VisionToolResult result = tool.Execute(source);
            if (!result.Success)
            {
                throw new InvalidOperationException($"{direction} failed. {result.ErrorName}: {result.Message}");
            }

            if (!result.Metrics.TryGetValue("EdgeCount", out double edgeCount) || edgeCount <= 0)
            {
                throw new InvalidOperationException($"{direction} did not produce EdgeCount. Metrics={string.Join(", ", result.Metrics.Keys)}");
            }

            ValidateLineGaugeStandardMetrics(direction.ToString(), result);
            ValidateLineGaugeFitLine(direction.ToString(), tool);
            results.Add($"{direction}={edgeCount:0}");
        }

        LineGaugeTool multiTool = CreateLineGaugeToolForContract(PROJECTION_DIR.X_LTOR, true);
        VisionToolResult multiResult = multiTool.Execute(source);
        if (!multiResult.Success)
        {
            throw new InvalidOperationException($"Multi ROI failed. {multiResult.ErrorName}: {multiResult.Message}");
        }

        List<OpenCvSharp.Point> multiEdges = multiTool.resultList
            .SelectMany(item => item.Results_List)
            .Select(edge => edge.MeasPos)
            .ToList();
        if (multiEdges.Count == 0)
        {
            throw new InvalidOperationException("Multi ROI did not produce any edge point.");
        }

        int minX = multiEdges.Min(point => point.X);
        if (minX < 20)
        {
            throw new InvalidOperationException($"Multi ROI edge offset was not applied. MinX={minX}");
        }

        ValidateLineGaugeOverlayGeometry(multiResult);
        ValidateLineGaugeStandardMetrics("Multi ROI", multiResult);
        ValidateLineGaugeFitLine("Multi ROI", multiTool);

        return $"LineGauge directions: OK | {string.Join(", ", results)} | MultiMinX={minX}";
    }

    private static string RunLineGaugePolarityContractCase()
    {
        using OpenCvSharp.Mat source = CreateLineGaugeDirectionImage();

        LineGaugeTool blackToWhiteTool = CreateLineGaugeToolForPolarity(PROJECTION_POLARITY.BTOW);
        VisionToolResult blackToWhiteResult = blackToWhiteTool.Execute(source);
        ValidateLineGaugeSuccess("LineGauge BTOW", blackToWhiteResult);

        LineGaugeTool whiteToBlackTool = CreateLineGaugeToolForPolarity(PROJECTION_POLARITY.WTOB);
        VisionToolResult whiteToBlackResult = whiteToBlackTool.Execute(source);
        ValidateLineGaugeSuccess("LineGauge WTOB", whiteToBlackResult);

        LineGaugeTool allTool = CreateLineGaugeToolForPolarity(PROJECTION_POLARITY.ALL);
        VisionToolResult allResult = allTool.Execute(source);
        ValidateLineGaugeSuccess("LineGauge ALL", allResult);

        double blackToWhiteX = AverageLineGaugeEdgeX(blackToWhiteTool, "BTOW");
        double whiteToBlackX = AverageLineGaugeEdgeX(whiteToBlackTool, "WTOB");
        double allX = AverageLineGaugeEdgeX(allTool, "ALL");

        if (whiteToBlackX - blackToWhiteX < 3)
        {
            throw new InvalidOperationException(
                $"LineGauge polarity did not separate leading/trailing edge. BTOW={blackToWhiteX:0.0}, WTOB={whiteToBlackX:0.0}.");
        }

        if (Math.Abs(allX - blackToWhiteX) > 1.5)
        {
            throw new InvalidOperationException(
                $"LineGauge ALL polarity should follow the first scanned edge. ALL={allX:0.0}, BTOW={blackToWhiteX:0.0}.");
        }

        return $"LineGauge polarity: OK | BTOW={blackToWhiteX:0.0}, WTOB={whiteToBlackX:0.0}, ALL={allX:0.0}";
    }

    private static void ValidateLineGaugeSuccess(string name, VisionToolResult result)
    {
        if (!result.Success)
        {
            throw new InvalidOperationException($"{name} failed. {result.ErrorName}: {result.Message}");
        }

        if (!result.Metrics.TryGetValue("EdgeCount", out double edgeCount) || edgeCount <= 0)
        {
            throw new InvalidOperationException($"{name} did not produce EdgeCount. Metrics={string.Join(", ", result.Metrics.Keys)}");
        }

        ValidateLineGaugeStandardMetrics(name, result);
    }

    private static void ValidateLineGaugeStandardMetrics(string name, VisionToolResult result)
    {
        string[] metricNames =
        {
            "ResultCount",
            "EdgeCount",
            "EdgeCountMin",
            "EdgeCountMax",
            "EdgeCountAvg",
            "EdgePointCount",
            "EdgePointCountMin",
            "EdgePointCountMax",
            "EdgePointCountAvg"
        };

        foreach (string metricName in metricNames)
        {
            if (!result.Metrics.TryGetValue(metricName, out double metricValue) || metricValue <= 0)
            {
                throw new InvalidOperationException($"{name} did not expose standard metric {metricName}. Metrics={string.Join(", ", result.Metrics.Keys)}");
            }
        }
    }

    private static void ValidateLineGaugeFitLine(string name, LineGaugeTool tool)
    {
        LineGaugeResult result = tool.resultList.FirstOrDefault()
            ?? throw new InvalidOperationException($"{name} did not produce a LineGaugeResult.");
        double distance = result.FitLine?.Distance() ?? 0d;
        if (double.IsNaN(distance) || double.IsInfinity(distance) || distance <= 0)
        {
            throw new InvalidOperationException($"{name} produced invalid fit line. Distance={distance:0.###}.");
        }
    }

    private static double AverageLineGaugeEdgeX(LineGaugeTool tool, string name)
    {
        List<OpenCvSharp.Point> points = tool.resultList
            .SelectMany(result => result.Results_List)
            .Select(edge => edge.MeasPos)
            .ToList();
        if (points.Count == 0)
        {
            throw new InvalidOperationException($"LineGauge {name} did not produce edge points.");
        }

        return points.Average(point => point.X);
    }

    private static void ValidateLineGaugeOverlayGeometry(VisionToolResult result)
    {
        VisionToolOverlay pointOverlay = result.Overlays.FirstOrDefault(overlay => overlay.Kind == VisionToolOverlayKind.Points)
            ?? throw new InvalidOperationException("LineGauge did not produce point overlay.");
        if (!pointOverlay.Label.StartsWith("#1", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"LineGauge point overlay label should start with #1. Label={pointOverlay.Label}.");
        }

        if (!result.Metrics.TryGetValue("EdgeCount", out double edgeCount) || pointOverlay.Points.Count < (int)edgeCount)
        {
            throw new InvalidOperationException($"LineGauge point overlay count is smaller than EdgeCount. EdgeCount={edgeCount:0}, Points={pointOverlay.Points.Count}.");
        }

        VisionToolOverlay lineOverlay = result.Overlays.FirstOrDefault(overlay => overlay.Kind == VisionToolOverlayKind.Line)
            ?? throw new InvalidOperationException("LineGauge did not produce line overlay.");
        if (lineOverlay.Start == lineOverlay.End)
        {
            throw new InvalidOperationException("LineGauge line overlay has identical start/end points.");
        }
    }

    private static LineGaugeTool CreateLineGaugeToolForContract(PROJECTION_DIR direction, bool useMultiRoi)
    {
        LineGaugeProperty property = new("Smoke_LineGauge")
        {
            PRJ_DIR = direction,
            PRJ_PORALITY = PROJECTION_POLARITY.BTOW,
            CONTRAST = 80,
            SAMPLING_STEP = 5,
            THICKNESS = 1,
            USE_ROI = !useMultiRoi,
            USE_MULTI_ROI = useMultiRoi,
            CvROI = new OpenCvSharp.Rect(0, 0, 80, 80),
            CvROIS = new List<OpenCvSharp.Rect> { new OpenCvSharp.Rect(20, 0, 50, 80) }
        };

        LineGaugeTool tool = new();
        tool.SetProperty(property);
        return tool;
    }

    private static LineGaugeTool CreateLineGaugeToolForPolarity(PROJECTION_POLARITY polarity)
    {
        LineGaugeProperty property = new("Smoke_LineGaugePolarity")
        {
            PRJ_DIR = PROJECTION_DIR.X_LTOR,
            PRJ_PORALITY = polarity,
            CONTRAST = 80,
            SAMPLING_STEP = 5,
            THICKNESS = 1,
            USE_ROI = true,
            USE_MULTI_ROI = false,
            CvROI = new OpenCvSharp.Rect(0, 0, 80, 80)
        };

        LineGaugeTool tool = new();
        tool.SetProperty(property);
        return tool;
    }

    private static OpenCvSharp.Mat CreateLineGaugeDirectionImage()
    {
        OpenCvSharp.Mat image = new OpenCvSharp.Mat(
            80,
            80,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(0));

        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(36, 4, 6, 72), OpenCvSharp.Scalar.All(255), -1);
        OpenCvSharp.Cv2.Rectangle(image, new OpenCvSharp.Rect(4, 44, 72, 6), OpenCvSharp.Scalar.All(255), -1);
        return image;
    }

    private static string RunAcceptanceReasonContractCase()
    {
        VisionPipeline pipeline = new()
        {
            Name = "Smoke_AcceptanceReasonContract"
        };
        VisionPipelineStep step = new()
        {
            Name = "01 Acceptance Reason Contract",
            ToolType = "Mean",
            InputLayer = "Main",
            OutputLayer = "Mean_Output",
            UseAcceptance = true,
            AcceptanceMetricName = "MeanValueAvg",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 300
        };
        step.Parameters["MEAN_TYPES"] = "Mean";
        pipeline.Steps.Add(step);

        using OpenCvSharp.Mat source = new OpenCvSharp.Mat(
            16,
            16,
            OpenCvSharp.MatType.CV_8UC1,
            OpenCvSharp.Scalar.All(120));
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipeline, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        VisionRecipeStepRunSummary summary = result.Steps.FirstOrDefault()
            ?? throw new InvalidOperationException("Acceptance step summary was not produced.");

        if (result.Success || summary.Success || summary.AcceptancePassed)
        {
            throw new InvalidOperationException("Expected acceptance NG, but the step passed.");
        }

        if (string.IsNullOrWhiteSpace(summary.Message)
            || !summary.Message.Contains("Mean Avg", StringComparison.OrdinalIgnoreCase)
            || !summary.Message.Contains("below target 300", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unexpected acceptance reason: {summary.Message}");
        }

        return $"Acceptance reason: OK | {summary.Message}";
    }

    private static string RunAcceptancePresetContractCase()
    {
        Type metricsType = typeof(VisionRecipeRunner).Assembly.GetType("OpenVisionLab.VisionPipelineKnownMetrics")
            ?? throw new InvalidOperationException("VisionPipelineKnownMetrics type was not found.");
        MethodInfo getPresetsForTool = metricsType.GetMethod(
            "GetPresetsForTool",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetPresetsForTool method was not found.");

        List<string> matchingPresets = GetPresetNames(getPresetsForTool, "Matching");
        List<string> meanPresets = GetPresetNames(getPresetsForTool, "Mean");
        List<string> featurePresets = GetPresetNames(getPresetsForTool, "FeatureMatching");
        List<string> lineGaugePresets = GetPresetNames(getPresetsForTool, "LineGauge");
        List<string> contourPresets = GetPresetNames(getPresetsForTool, "Contour");

        if (!matchingPresets.Any(name => name.Contains("Best Score >= 80", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Matching preset list did not include Best Score >= 80.");
        }

        if (matchingPresets.Any(name => name.Contains("Mean <=", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Matching preset list included Mean preset.");
        }

        if (!featurePresets.Any(name => name.Contains("Best Score >= 60", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("FeatureMatching preset list did not include relaxed feature score preset.");
        }

        if (!meanPresets.Any(name => name.Contains("Mean <= 180", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Mean preset list did not include Mean <= 180.");
        }

        if (meanPresets.Any(name => name.Contains("Best Score", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Mean preset list included score preset.");
        }

        if (!lineGaugePresets.Any(name => name.Contains("Line Edge Count >= 1", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("LineGauge preset list did not include Line Edge Count >= 1.");
        }

        if (!lineGaugePresets.Any(name => name.Contains("Fitted Line Length >= 100 px", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("LineGauge preset list did not include Fitted Line Length >= 100 px.");
        }

        if (lineGaugePresets.Any(name => name.Contains("Best Score", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("LineGauge preset list included score preset.");
        }

        if (!contourPresets.Any(name => name.Contains("Max Bounds Height <= 20 px", StringComparison.OrdinalIgnoreCase))
            || !contourPresets.Any(name => name.Contains("Max Bounds Height <= 0.12 mm", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Contour preset list did not include bounds-height pixel/mm measurement presets.");
        }

        return $"Acceptance presets: OK | Matching={matchingPresets.Count}, Feature={featurePresets.Count}, Mean={meanPresets.Count}, LineGauge={lineGaugePresets.Count}, Contour={contourPresets.Count}";
    }

    private static List<string> GetPresetNames(MethodInfo getPresetsForTool, string toolType)
    {
        object? presets = getPresetsForTool.Invoke(null, new object[] { toolType });
        IEnumerable enumerable = presets as IEnumerable
            ?? throw new InvalidOperationException($"Preset result for {toolType} was not enumerable.");
        List<string> names = new();
        foreach (object preset in enumerable)
        {
            string name = preset.GetType().GetProperty("Name")?.GetValue(preset)?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                names.Add(name);
            }
        }

        return names;
    }

    private static bool IsInspectionToolForContract(string toolType)
    {
        switch (NormalizeToolTypeForSmoke(toolType))
        {
            case "blob":
            case "contour":
            case "line":
            case "linegauge":
            case "matching":
            case "templatematching":
            case "feature":
            case "featurematching":
            case "sift":
            case "mean":
            case "overlaymerge":
            case "resultmerge":
            case "mergeresult":
                return true;
            default:
                return false;
        }
    }

    private static bool IsScoreToolForContract(string toolType)
    {
        switch (NormalizeToolTypeForSmoke(toolType))
        {
            case "matching":
            case "templatematching":
            case "feature":
            case "featurematching":
            case "sift":
                return true;
            default:
                return false;
        }
    }

    private static bool RequiresOverlayForContract(string toolType)
    {
        switch (NormalizeToolTypeForSmoke(toolType))
        {
            case "blob":
            case "contour":
            case "line":
            case "linegauge":
            case "matching":
            case "templatematching":
            case "feature":
            case "featurematching":
            case "sift":
            case "overlaymerge":
            case "resultmerge":
            case "mergeresult":
                return true;
            default:
                return false;
        }
    }

    private static string NormalizeToolTypeForSmoke(string toolType)
    {
        string value = (toolType ?? string.Empty).Trim();
        if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring(0, value.Length - 4);
        }

        return value.Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
    }

    private static IEnumerable<object> LoadRunnableCatalogSamples()
    {
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type catalogItemType = appAssembly.GetType("OpenVisionLab.VisionPipelineSampleCatalogItem", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem type was not found.");
        MethodInfo loadMethod = catalogItemType.GetMethod("LoadRunnable", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem.LoadRunnable was not found.");
        object? loaded = loadMethod.Invoke(null, null);
        System.Collections.IEnumerable samples = loaded as System.Collections.IEnumerable
            ?? throw new InvalidOperationException("Sample catalog did not return a list.");
        return samples.Cast<object>();
    }

    private static string ResolveRepoRootForSmoke()
    {
        foreach (string root in EnumerateRepoSearchRootsForSmoke())
        {
            if (Directory.Exists(Path.Combine(root, "Sample"))
                && Directory.Exists(Path.Combine(root, "docs", "samples")))
            {
                return Path.GetFullPath(root);
            }
        }

        throw new InvalidOperationException("OpenVisionLab repo root could not be resolved from the smoke process.");
    }

    private static IEnumerable<string> EnumerateRepoSearchRootsForSmoke()
    {
        HashSet<string> seeds = new(StringComparer.OrdinalIgnoreCase)
        {
            Directory.GetCurrentDirectory(),
            AppDomain.CurrentDomain.BaseDirectory
        };

        foreach (string seed in seeds)
        {
            DirectoryInfo? directory = new(seed);
            for (int i = 0; i < 10 && directory != null; i++)
            {
                yield return directory.FullName;
                directory = directory.Parent;
            }
        }
    }

    private static string GetRepoRelativePath(string repoRoot, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string fullPath = Path.GetFullPath(path);
        string relativePath = Path.GetRelativePath(repoRoot, fullPath);
        return relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }

    private static string GetSampleTopFolder(string sampleRoot, string imagePath)
    {
        string relativePath = Path.GetRelativePath(sampleRoot, imagePath)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        int separatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);
        return separatorIndex < 0 ? "." : relativePath.Substring(0, separatorIndex);
    }

    private static string GetSampleTopFolderFromCatalogPath(string catalogPath)
    {
        if (string.IsNullOrWhiteSpace(catalogPath)
            || !catalogPath.StartsWith("Sample\\", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        string relativePath = catalogPath.Substring("Sample\\".Length)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        int separatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);
        return separatorIndex < 0 ? "." : relativePath.Substring(0, separatorIndex);
    }

    private static string RunSampleCatalogCase(object sample)
    {
        string name = GetStringProperty(sample, "SampleName");
        string imagePath = GetStringProperty(sample, "ImageFullPath");
        string pipelinePath = GetStringProperty(sample, "PipelineFullPath");
        List<ExpectedMetricCheck> expectedMetrics = GetExpectedMetricChecks(sample);

        using Bitmap bitmap = new Bitmap(imagePath);
        using OpenCvSharp.Mat source = BitmapImageConverter.ToMat(bitmap);
        using VisionRecipeRunResult result = new VisionRecipeRunner()
            .RunAsync(pipelinePath, source, "Main", VisionRecipeRunner.DefaultStepTimeoutMilliseconds, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (!result.Success)
        {
            throw new InvalidOperationException(result.Message);
        }

        string metricText = "no metric gate";
        if (expectedMetrics.Count > 0)
        {
            List<string> metricParts = new();
            foreach (ExpectedMetricCheck expectedMetric in expectedMetrics)
            {
                if (!TryFindMetric(result, expectedMetric.Name, out double metricValue))
                {
                    throw new InvalidOperationException($"Expected metric '{expectedMetric.Name}' was not produced.");
                }

                if (TryParseDouble(expectedMetric.Minimum, out double minimum) && metricValue < minimum)
                {
                    throw new InvalidOperationException($"{expectedMetric.Name} {metricValue:0.###} < {minimum:0.###}.");
                }

                if (TryParseDouble(expectedMetric.Maximum, out double maximum) && metricValue > maximum)
                {
                    throw new InvalidOperationException($"{expectedMetric.Name} {metricValue:0.###} > {maximum:0.###}.");
                }

                metricParts.Add($"{expectedMetric.Name}: expected {BuildExpectedMetricRangeText(expectedMetric)}, actual {metricValue:0.###}, judgment OK");
            }

            metricText = string.Join("; ", metricParts);
        }

        return $"{name}: OK | {metricText} | {result.TotalMilliseconds:0.0} ms";
    }

    private static List<ExpectedMetricCheck> GetExpectedMetricChecks(object sample)
    {
        object? value = sample.GetType()
            .GetProperty("ExpectedMetrics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(sample);
        if (value is System.Collections.IEnumerable expectedMetrics)
        {
            List<ExpectedMetricCheck> checks = new();
            foreach (object metric in expectedMetrics)
            {
                string name = GetStringProperty(metric, "Name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                checks.Add(new ExpectedMetricCheck(
                    name,
                    GetStringProperty(metric, "Minimum"),
                    GetStringProperty(metric, "Maximum")));
            }

            return checks;
        }

        string expectedMetric = GetStringProperty(sample, "ExpectedMetricName");
        if (string.IsNullOrWhiteSpace(expectedMetric))
        {
            return new List<ExpectedMetricCheck>();
        }

        return new List<ExpectedMetricCheck>
        {
            new ExpectedMetricCheck(
                expectedMetric,
                GetStringProperty(sample, "ExpectedMetricMinimum"),
                GetStringProperty(sample, "ExpectedMetricMaximum"))
        };
    }

    private sealed record ExpectedMetricCheck(string Name, string Minimum, string Maximum);

    private static string BuildExpectedMetricRangeText(ExpectedMetricCheck expectedMetric)
    {
        string minimum = expectedMetric?.Minimum?.Trim() ?? string.Empty;
        string maximum = expectedMetric?.Maximum?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(minimum) && !string.IsNullOrWhiteSpace(maximum))
        {
            return string.Equals(minimum, maximum, StringComparison.OrdinalIgnoreCase)
                ? minimum
                : $"{minimum}..{maximum}";
        }

        if (!string.IsNullOrWhiteSpace(minimum))
        {
            return $">= {minimum}";
        }

        if (!string.IsNullOrWhiteSpace(maximum))
        {
            return $"<= {maximum}";
        }

        return "-";
    }

    private static bool TryFindMetric(VisionRecipeRunResult result, string metricName, out double value)
    {
        value = 0;
        foreach (VisionRecipeStepRunSummary step in result.Steps.AsEnumerable().Reverse())
        {
            if (step.Metrics != null && step.Metrics.TryGetValue(metricName, out value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryParseDouble(string text, out double value)
    {
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static bool ValidatePipelineForSmoke(
        Assembly appAssembly,
        VisionPipeline pipeline,
        out string errors,
        out int warningCount)
    {
        Type validatorType = appAssembly.GetType("OpenVisionLab.VisionPipelineValidator", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineValidator type was not found.");
        MethodInfo validateMethod = validatorType.GetMethod("Validate", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineValidator.Validate was not found.");
        object validation = validateMethod.Invoke(null, new object[] { pipeline, new[] { "Main" } })
            ?? throw new InvalidOperationException("Pipeline validation did not return a result.");
        bool success = validation.GetType().GetProperty("Success")?.GetValue(validation) is bool value && value;
        errors = InvokeValidationFormatter(validation, "FormatErrors");
        warningCount = GetValidationCount(validation, "Warnings");
        return success;
    }

    private static string GetStringProperty(object source, string propertyName)
    {
        return source.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(source)
            ?.ToString() ?? string.Empty;
    }

    private static int GetIntProperty(object source, string propertyName)
    {
        object? value = source.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(source);
        return value is int intValue ? intValue : 0;
    }

    private static string BuildGridText(DataGridView grid)
    {
        if (grid == null)
        {
            return string.Empty;
        }

        List<string> parts = new();
        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row == null || row.IsNewRow)
            {
                continue;
            }

            foreach (DataGridViewCell cell in row.Cells)
            {
                string value = cell?.FormattedValue?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    parts.Add(value.Trim());
                }
            }
        }

        return string.Join(" | ", parts);
    }

    private static CaptureDiagnostics CapturePipelineAddStepForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineAddStep", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineAddStep type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 10);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Threshold", "Morphology", "Contour", "LineGauge", "RotateScale" },
            new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" },
            "Contour",
            "TextSymbol_Clean",
            "TextSymbol_Clean_Contour",
            "03 Text Symbol Contour",
            new[] { "01 Text Symbol Binary", "02 Text Symbol Clean" },
            new Func<IEnumerable<string>>(() => new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" }),
            "Add after 02 Text Symbol Clean",
            "TextSymbol_Clean"
        });

        return CaptureForm(form, outputPath, new Size(720, 470), 12);
    }

    private static CaptureDiagnostics CapturePipelineDesignableForms(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        string[] typeNames =
        {
            "OpenVisionLab.FormVisionPipelineAddStep",
            "OpenVisionLab.FormVisionPipelineTextPrompt",
            "OpenVisionLab.FormVisionPipelineSamples",
            "OpenVisionLab.FormVisionPipelineBatch",
            "OpenVisionLab.FormVisionPipelineHistory",
            "OpenVisionLab.FormVisionPipelineBatchHistory",
            "OpenVisionLab.FormVisionPipelinePromptPreview",
            "OpenVisionLab.FormVisionPipelineLlmRecipe",
            "OpenVisionLab.FormVisionPipelineImageViewer"
        };

        List<string> lines = new();
        foreach (string typeName in typeNames)
        {
            Type type = appAssembly.GetType(typeName, throwOnError: true)
                ?? throw new InvalidOperationException($"{typeName} type was not found.");
            using Form form = (Form)(Activator.CreateInstance(type)
                ?? throw new InvalidOperationException($"{typeName} could not be created with a default constructor."));
            lines.Add($"OK  {type.Name}");
        }

        using Form report = CreateSmokeReportForm("Pipeline Designer Constructors", lines);
        return CaptureForm(report, outputPath, new Size(620, 380), 8);
    }

    private static CaptureDiagnostics CapturePipelineSamplesForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineSamples", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples could not be created."));
        return CaptureForm(form, outputPath, new Size(940, 620), 14);
    }

    private static CaptureDiagnostics CapturePipelineSamplesCheckAction(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineSamples", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples could not be created."));
        return CaptureForm(form, outputPath, new Size(940, 620), 14, RunPipelineSampleCheckAction);
    }

    private static CaptureDiagnostics CapturePipelineSamplesPinsLineCheckAction(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineSamples", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormVisionPipelineSamples could not be created."));
        return CaptureForm(form, outputPath, new Size(940, 620), 14, shownForm => RunPipelineSampleCheckAction(shownForm, "Pins_LineGauge"));
    }

    private static void RunPipelineSampleCheckAction(Form form)
    {
        RunPipelineSampleCheckAction(form, string.Empty);
    }

    private static void RunPipelineSampleCheckAction(Form form, string sampleName)
    {
        Button checkButton = FindControl<Button>(form, "btnCheckCatalog")
            ?? throw new InvalidOperationException("Check Sample button was not found.");
        Button openButton = FindControl<Button>(form, "btnOpenCatalog")
            ?? throw new InvalidOperationException("Open Sample button was not found.");
        TextBox detailsText = FindControl<TextBox>(form, "catalogDetailsText")
            ?? throw new InvalidOperationException("Catalog details text box was not found.");
        Label statusLabel = FindControl<Label>(form, "catalogStatusLabel")
            ?? throw new InvalidOperationException("Catalog status label was not found.");
        Label titleLabel = FindControl<Label>(form, "catalogTitleLabel")
            ?? throw new InvalidOperationException("Catalog title label was not found.");
        Label learningLabel = FindControl<Label>(form, "catalogLearningLabel")
            ?? throw new InvalidOperationException("Catalog learning label was not found.");
        PictureBox referenceBox = FindControl<PictureBox>(form, "catalogReferenceBox")
            ?? throw new InvalidOperationException("Catalog reference preview box was not found.");
        Label referenceEmptyLabel = FindControl<Label>(form, "catalogReferenceEmptyLabel")
            ?? throw new InvalidOperationException("Catalog reference empty label was not found.");
        ListBox catalogList = FindControl<ListBox>(form, "catalogList")
            ?? throw new InvalidOperationException("Catalog list was not found.");

        string firstCatalogText = catalogList.Items.Count == 0 ? string.Empty : catalogList.Items[0]?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(firstCatalogText)
            || !firstCatalogText.Contains("Contour_TextSymbols", StringComparison.OrdinalIgnoreCase)
            || !firstCatalogText.Contains("Ready", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Catalog list should expose compact sample/readiness text.");
        }

        if (firstCatalogText.Contains("ResultCount", StringComparison.OrdinalIgnoreCase)
            || firstCatalogText.Contains("min ", StringComparison.OrdinalIgnoreCase)
            || firstCatalogText.Contains("max ", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Catalog list should keep metric details in the details panel, not in each list item.");
        }

        if (!openButton.Text.Contains("Preview", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Open catalog button should make the preview action explicit. Text='{openButton.Text}'");
        }

        if (!learningLabel.Text.Contains("Learn:", StringComparison.OrdinalIgnoreCase)
            || !learningLabel.Text.Contains("Flow:", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Catalog learning label did not show learning focus and flow. Text='{learningLabel.Text}'");
        }

        if (!string.IsNullOrWhiteSpace(sampleName))
        {
            SelectCatalogSample(form, sampleName);
        }

        if (string.IsNullOrWhiteSpace(sampleName)
            && referenceBox.Image == null)
        {
            throw new InvalidOperationException("Expected first catalog sample to show a reference result image.");
        }

        if (string.IsNullOrWhiteSpace(sampleName)
            && referenceEmptyLabel.Visible)
        {
            throw new InvalidOperationException("Reference empty label should be hidden when the first catalog sample has a reference image.");
        }

        if (!string.IsNullOrWhiteSpace(sampleName)
            && string.Equals(sampleName, "Pins_LineGauge", StringComparison.OrdinalIgnoreCase)
            && (!referenceEmptyLabel.Visible
                || !referenceEmptyLabel.Text.Contains("No expected result", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Expected Pins_LineGauge to show an explicit empty reference state. Text='{referenceEmptyLabel.Text}', Visible={referenceEmptyLabel.Visible}");
        }

        checkButton.PerformClick();
        bool completed = false;
        for (int i = 0; i < 120; i++)
        {
            Application.DoEvents();
            completed = detailsText.Text.Contains("Last check: OK", StringComparison.OrdinalIgnoreCase)
                || detailsText.Text.Contains("Last check: NG", StringComparison.OrdinalIgnoreCase)
                || detailsText.Text.Contains("Last check: ERROR", StringComparison.OrdinalIgnoreCase);
            if (completed)
            {
                break;
            }

            Thread.Sleep(80);
        }

        if (!completed)
        {
            throw new InvalidOperationException("Check Sample action did not finish.");
        }

        for (int i = 0; i < 40 && checkButton.Text.Contains("Checking", StringComparison.OrdinalIgnoreCase); i++)
        {
            Application.DoEvents();
            Thread.Sleep(50);
        }

        if (!detailsText.Text.Contains("Last check: OK", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Expected first catalog sample check to pass. Details: " + detailsText.Text);
        }

        if (!detailsText.Text.Contains("Expected metric:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Recipe guide:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Check:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Metric review:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("expected", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("actual", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("judgment", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Final:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Overlays:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Action:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Step flow:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Catalog coverage:", StringComparison.OrdinalIgnoreCase)
            || !detailsText.Text.Contains("Backlog:", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Catalog check details should include recipe guide, check guide, expected/actual metric review, final layer, overlay count, action summary, step flow, and catalog coverage. Details: " + detailsText.Text);
        }

        if (!detailsText.Text.Contains("Backlog: none", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Catalog coverage should report no uncovered sample folders. Details: " + detailsText.Text);
        }

        if (!string.Equals(statusLabel.Text, "OK", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Expected catalog status label to be OK, but it was '{statusLabel.Text}'.");
        }

        if (!string.IsNullOrWhiteSpace(sampleName)
            && !titleLabel.Text.Contains(sampleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Expected sample title to contain '{sampleName}', but it was '{titleLabel.Text}'.");
        }

        if (!string.IsNullOrWhiteSpace(sampleName)
            && string.Equals(sampleName, "Pins_LineGauge", StringComparison.OrdinalIgnoreCase)
            && !learningLabel.Text.Contains("LineGauge", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Expected Pins_LineGauge learning flow to mention LineGauge. Text='{learningLabel.Text}'");
        }

        if (!string.IsNullOrWhiteSpace(sampleName)
            && string.Equals(sampleName, "Pins_LineGauge", StringComparison.OrdinalIgnoreCase)
            && !detailsText.Text.Contains("fitted line length/angle", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Expected Pins_LineGauge check guide to mention fitted line length/angle. Details: " + detailsText.Text);
        }
    }

    private static void SelectCatalogSample(Form form, string sampleName)
    {
        ListBox catalogList = FindControl<ListBox>(form, "catalogList")
            ?? throw new InvalidOperationException("Catalog list was not found.");
        for (int i = 0; i < catalogList.Items.Count; i++)
        {
            string currentName = GetStringProperty(catalogList.Items[i], "SampleName");
            if (string.Equals(currentName, sampleName, StringComparison.OrdinalIgnoreCase))
            {
                catalogList.SelectedIndex = i;
                PumpUi(6);
                return;
            }
        }

        throw new InvalidOperationException($"Catalog sample '{sampleName}' was not found.");
    }

    private static Form CreateSmokeReportForm(string title, IEnumerable<string> lines)
    {
        Form form = new Form
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            BackColor = Color.FromArgb(238, 242, 246)
        };
        TextBox textBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(250, 252, 253),
            HideSelection = true,
            Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point),
            ScrollBars = ScrollBars.Both,
            TabStop = false,
            Text = string.Join(Environment.NewLine, lines),
            WordWrap = false
        };
        form.Controls.Add(textBox);
        form.Shown += (_, _) =>
        {
            textBox.SelectionStart = 0;
            textBox.SelectionLength = 0;
            form.ActiveControl = null;
        };
        return form;
    }

    private static CaptureDiagnostics CapturePipelineAddStepBranchForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineAddStep", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineAddStep type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 10);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Threshold", "Morphology", "Contour", "LineGauge", "RotateScale" },
            new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" },
            "Contour",
            "Main",
            "Main_Contour",
            "03 Text Symbol Contour",
            new[] { "01 Text Symbol Binary", "02 Text Symbol Clean" },
            new Func<IEnumerable<string>>(() => new[] { "Main", "TextSymbol_Binary", "TextSymbol_Clean" }),
            "Add after 02 Text Symbol Clean",
            "TextSymbol_Clean"
        });

        return CaptureForm(form, outputPath, new Size(720, 470), 12, shownForm => AssertBranchConfirmationState(shownForm, formType));
    }

    private static void AssertBranchConfirmationState(Form form, Type formType)
    {
        FieldInfo? addButtonField = formType.GetField("btnAdd", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? branchCheckField = formType.GetField("chkAllowBranch", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? validationField = formType.GetField("validationLabel", BindingFlags.Instance | BindingFlags.NonPublic);

        if (addButtonField?.GetValue(form) is not Button addButton || addButton.Enabled)
        {
            throw new InvalidOperationException("Branch add-step smoke expected Add Step to be disabled until branch is allowed.");
        }

        if (branchCheckField?.GetValue(form) is not CheckBox branchCheck || !branchCheck.Visible || branchCheck.Checked)
        {
            throw new InvalidOperationException("Branch add-step smoke expected visible unchecked branch confirmation.");
        }

        string validationText = validationField?.GetValue(form) is Label label ? label.Text : string.Empty;
        if (!validationText.Contains("Branch input selected", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Branch add-step smoke expected branch validation message.");
        }

        branchCheck.Checked = true;
        Application.DoEvents();
        if (!addButton.Enabled)
        {
            throw new InvalidOperationException("Branch add-step smoke expected Add Step to become enabled after allowing branch input.");
        }

        branchCheck.Checked = false;
        Application.DoEvents();
    }

    private static CaptureDiagnostics CapturePipelineTextPromptForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineTextPrompt", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineTextPrompt type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 3);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            "Save Sample",
            "Sample Name",
            "Contour_TextSymbols_20260614"
        });

        return CaptureForm(form, outputPath, new Size(480, 250), 12);
    }

    private static void SeedPipelineFormWithSample(Form form, Type formType, bool branchContourInput = false)
    {
        FieldInfo? pipelineField = formType.GetField("pipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? bindPipelineMethod = formType.GetMethod("BindPipeline", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? appendActiveLogMethod = formType.GetMethod("AppendActivePipelineLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runLogField = formType.GetField("tbRunLog", BindingFlags.Instance | BindingFlags.NonPublic);
        if (pipelineField == null || bindPipelineMethod == null)
        {
            return;
        }

        VisionPipeline samplePipeline = CreatePipelineFormSample(branchContourInput);
        pipelineField.SetValue(form, samplePipeline);
        bindPipelineMethod.Invoke(form, null);
        if (formType.GetField("tbPipelineName", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(form) is TextBox pipelineNameTextBox)
        {
            pipelineNameTextBox.Text = samplePipeline.Name;
        }

        if (runLogField?.GetValue(form) is TextBox runLog)
        {
            runLog.Clear();
        }

        appendActiveLogMethod?.Invoke(form, new object[] { "OPEN" });
    }

    private static VisionPipeline CreatePipelineFormSample(bool branchContourInput = false)
    {
        VisionPipeline pipeline = new()
        {
            Name = "Contour_TextSymbols"
        };

        VisionPipelineStep threshold = new()
        {
            Name = "01 Text Symbol Binary",
            ToolType = "Threshold",
            InputLayer = "Main",
            OutputLayer = "TextSymbol_Binary"
        };
        threshold.Parameters["Mode"] = "Threshold";
        threshold.Parameters["Threshold"] = "170";
        threshold.Parameters["MaxValue"] = "255";
        threshold.Parameters["ThresholdType"] = "BinaryInv";

        VisionPipelineStep morphology = new()
        {
            Name = "02 Text Symbol Clean",
            ToolType = "Morphology",
            InputLayer = "TextSymbol_Binary",
            OutputLayer = "TextSymbol_Clean"
        };
        morphology.Parameters["Operator"] = "Open";
        morphology.Parameters["KernelWidth"] = "3";
        morphology.Parameters["KernelHeight"] = "3";
        morphology.Parameters["Iterations"] = "1";

        VisionPipelineStep contour = new()
        {
            Name = "03 Text Symbol Contour",
            ToolType = "Contour",
            InputLayer = branchContourInput ? "Main" : "TextSymbol_Clean",
            OutputLayer = "TextSymbol_Contour",
            UseAcceptance = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 35,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 80
        };
        contour.Parameters["Name"] = "Contour_TextSymbol";
        contour.Parameters["MIN_AREA"] = "15";
        contour.Parameters["MAX_AREA"] = "2500";

        pipeline.Steps.Add(threshold);
        pipeline.Steps.Add(morphology);
        pipeline.Steps.Add(contour);
        return pipeline;
    }

    private static void SelectPipelineStep(Form form, Type formType, int index)
    {
        MethodInfo? selectStepMethod = formType.GetMethod("SelectStepAt", BindingFlags.Instance | BindingFlags.NonPublic);
        selectStepMethod?.Invoke(form, new object[] { index });
        PumpUi(8);
    }

    private static void RunPipelinePreviewSmoke(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        AssertPipelineWorkflowHint(form, formType);
        MethodInfo? runMethod = formType.GetMethod("OnRunClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? selectStepMethod = formType.GetMethod("SelectStepAt", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runLogField = formType.GetField("tbRunLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runButtonField = formType.GetField("btnRun", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runningField = formType.GetField("isRunningPipeline", BindingFlags.Instance | BindingFlags.NonPublic);

        if (runMethod == null)
        {
            throw new InvalidOperationException("Pipeline run handler was not found.");
        }

        runMethod.Invoke(form, new object?[] { form, EventArgs.Empty });
        bool completed = false;
        bool previewOk = false;
        string lastLogText = string.Empty;
        for (int i = 0; i < 160; i++)
        {
            Application.DoEvents();
            string logText = runLogField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;
            lastLogText = logText;
            bool running = runningField?.GetValue(form) is bool isRunning && isRunning;
            bool runButtonReady = runButtonField?.GetValue(form) is not Control runButton || runButton.Enabled;
            previewOk = logText.Contains("PREVIEW OK", StringComparison.OrdinalIgnoreCase);
            completed = previewOk
                || logText.Contains("PREVIEW NG", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("PREVIEW CANCELED", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("Pipeline failed", StringComparison.OrdinalIgnoreCase);
            if (completed && !running && runButtonReady)
            {
                break;
            }

            Thread.Sleep(80);
        }

        if (!completed)
        {
            throw new TimeoutException("Pipeline preview did not finish in the smoke capture.");
        }

        if (!previewOk)
        {
            throw new InvalidOperationException("Pipeline preview smoke expected PREVIEW OK. Log: " + Truncate(lastLogText, 240));
        }

        selectStepMethod?.Invoke(form, new object[] { 2 });
        PumpUi(8);
        AssertPipelineWorkflowHint(form, formType);
    }

    private static void RunPipelineSampleOpenPreviewSmoke(Form form)
    {
        RunPipelineSampleOpenPreviewSmoke(
            form,
            "Contour_TextSymbols",
            "ResultCount=51",
            "TextSymbol_Contour",
            "Threshold > Morphology > Contour");
    }

    private static void RunPipelineSampleOpenPreviewSmoke(
        Form form,
        string sampleName,
        string expectedMetricText,
        string expectedFinalLayer,
        string expectedWorkflowText)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        AssertPipelineWorkflowHint(form, formType);

        object sample = LoadCatalogSample(formType.Assembly, sampleName);
        MethodInfo? applySampleMethod = formType.GetMethod("ApplyCatalogSample", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runLogField = formType.GetField("tbRunLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runButtonField = formType.GetField("btnRun", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runningField = formType.GetField("isRunningPipeline", BindingFlags.Instance | BindingFlags.NonPublic);

        if (applySampleMethod == null)
        {
            throw new InvalidOperationException("ApplyCatalogSample was not found.");
        }

        applySampleMethod.Invoke(form, new[] { sample });
        bool completed = false;
        bool previewOk = false;
        bool sampleOpened = false;
        string lastLogText = string.Empty;
        int maxIterations = sampleName.Contains("AllSymbols", StringComparison.OrdinalIgnoreCase) ? 600 : 180;
        for (int i = 0; i < maxIterations; i++)
        {
            Application.DoEvents();
            string logText = runLogField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;
            lastLogText = logText;
            bool running = runningField?.GetValue(form) is bool isRunning && isRunning;
            bool runButtonReady = runButtonField?.GetValue(form) is not Control runButton || runButton.Enabled;
            sampleOpened = logText.Contains("SAMPLE OPEN", StringComparison.OrdinalIgnoreCase);
            previewOk = logText.Contains("PREVIEW OK", StringComparison.OrdinalIgnoreCase);
            completed = sampleOpened && previewOk;
            if (completed && !running && runButtonReady)
            {
                break;
            }

            Thread.Sleep(80);
        }

        if (!sampleOpened)
        {
            throw new InvalidOperationException("Sample open did not write SAMPLE OPEN to the Pipeline log. Log: " + Truncate(lastLogText, 240));
        }

        if (!previewOk)
        {
            throw new InvalidOperationException(
                "Sample open should automatically run preview and finish with PREVIEW OK. Log head: "
                + Truncate(lastLogText, 1000)
                + " | Log tail: "
                + Tail(lastLogText, 1200));
        }

        if (!lastLogText.Contains("SAMPLE RESULT OK", StringComparison.OrdinalIgnoreCase)
            || !lastLogText.Contains(expectedMetricText, StringComparison.OrdinalIgnoreCase)
            || !lastLogText.Contains($"Final={expectedFinalLayer}", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Sample preview log should include sample result metric and final layer. Log: " + Truncate(lastLogText, 240));
        }

        if (!lastLogText.Contains("SAMPLE GUIDE", StringComparison.OrdinalIgnoreCase)
            || !lastLogText.Contains(expectedWorkflowText.Replace(" > ", " -> "), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Sample preview log should include the sample recipe guide. Log: " + Truncate(lastLogText, 240));
        }

        DataGridView resultGrid = FindControl<DataGridView>(form, "resultGrid")
            ?? throw new InvalidOperationException("Pipeline result grid was not found.");
        string resultGridText = BuildGridText(resultGrid);
        if (!resultGridText.Contains("Sample", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains("Goal", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains("Recipe flow", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains(expectedWorkflowText.Replace(" > ", " -> "), StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains("Expected", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains("Actual", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains(expectedMetricText, StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains("Final layer", StringComparison.OrdinalIgnoreCase)
            || !resultGridText.Contains(expectedFinalLayer, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline result grid should include sample expected/actual/final layer details. Grid: " + Truncate(resultGridText, 240));
        }

        AssertSampleContextHint(form, formType, sampleName, expectedMetricText, expectedFinalLayer);
        AssertPipelineWorkflowHint(form, formType);
        AssertPipelineWorkflowHintContains(form, formType, expectedWorkflowText);
    }

    private static object LoadCatalogSample(Assembly appAssembly, string sampleName)
    {
        Type sampleType = appAssembly.GetType("OpenVisionLab.VisionPipelineSampleCatalogItem", throwOnError: true)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem type was not found.");
        MethodInfo loadMethod = sampleType.GetMethod("LoadRunnable", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("VisionPipelineSampleCatalogItem.LoadRunnable was not found.");
        if (loadMethod.Invoke(null, null) is not IEnumerable samples)
        {
            throw new InvalidOperationException("Catalog samples could not be loaded.");
        }

        foreach (object sample in samples)
        {
            string currentName = GetStringProperty(sample, "SampleName");
            if (string.Equals(currentName, sampleName, StringComparison.OrdinalIgnoreCase))
            {
                return sample;
            }
        }

        throw new InvalidOperationException($"Catalog sample '{sampleName}' was not found.");
    }

    private static void AssertPipelineWorkflowHint(Form form, Type formType)
    {
        FieldInfo? hintField = formType.GetField("workflowHintLabel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (hintField?.GetValue(form) is not Label hintLabel)
        {
            throw new InvalidOperationException("Pipeline workflow hint label was not found.");
        }

        if (!hintLabel.Visible)
        {
            throw new InvalidOperationException("Pipeline workflow hint label should be visible at the smoke viewport.");
        }

        if (!hintLabel.Text.Contains("Preview", StringComparison.OrdinalIgnoreCase)
            || !hintLabel.Text.Contains("Publish", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Pipeline workflow hint should mention Preview and Publish. Text='{hintLabel.Text}'");
        }

        AssertPipelineFlowActionHint(form);
    }

    private static void AssertPipelineFlowActionHint(Form form)
    {
        string allText = CollectControlText(form);
        bool hasImageAction = allText.Contains("View input image", StringComparison.OrdinalIgnoreCase)
            || allText.Contains("View output image", StringComparison.OrdinalIgnoreCase);
        bool hasPreviewRequired = allText.Contains("Run Preview required", StringComparison.OrdinalIgnoreCase);
        if (!hasImageAction && !hasPreviewRequired)
        {
            throw new InvalidOperationException("Pipeline flow should explain whether a step image can be opened or needs Run Preview first.");
        }
    }

    private static void AssertPipelineWorkflowHintContains(Form form, Type formType, string expectedText)
    {
        FieldInfo? hintField = formType.GetField("workflowHintLabel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (hintField?.GetValue(form) is not Label hintLabel)
        {
            throw new InvalidOperationException("Pipeline workflow hint label was not found.");
        }

        if (!hintLabel.Text.Contains(expectedText, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Pipeline workflow hint should include '{expectedText}'. Text='{hintLabel.Text}'");
        }
    }

    private static void AssertSampleContextHint(
        Form form,
        Type formType,
        string expectedSampleName,
        string expectedMetricText,
        string expectedFinalLayer)
    {
        FieldInfo? hintField = formType.GetField("sampleContextLabel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (hintField?.GetValue(form) is not Label hintLabel)
        {
            throw new InvalidOperationException("Pipeline sample context label was not found.");
        }

        if (!hintLabel.Visible)
        {
            throw new InvalidOperationException("Pipeline sample context label should be visible after opening a catalog sample.");
        }

        if (!hintLabel.Text.Contains(expectedSampleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Pipeline sample context label did not include the opened sample name.");
        }

        if (!hintLabel.Text.Contains(expectedMetricText, StringComparison.OrdinalIgnoreCase)
            || !hintLabel.Text.Contains($"Final {expectedFinalLayer}", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Pipeline sample context label should include actual metric and final layer. Text='{hintLabel.Text}'");
        }
    }

    private static CaptureDiagnostics CaptureAiRecipeForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineLlmRecipe", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineLlmRecipe type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 2);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Main" },
            new Func<VisionPipelineContext>(() => new VisionPipelineContext())
        });

        form.Tag = formType;

        return CaptureForm(form, outputPath, new Size(1180, 760), 16, RunAiRecipePreview);
    }

    private static CaptureDiagnostics CaptureAiRecipePromptContractCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineLlmRecipe", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineLlmRecipe type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 2);
        MethodInfo buildPromptMethod = formType.GetMethod("BuildLlmPrompt", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("FormVisionPipelineLlmRecipe.BuildLlmPrompt was not found.");

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Main", "Threshold", "TextSymbol_Contour" },
            new Func<VisionPipelineContext>(() => new VisionPipelineContext())
        });

        string prompt = buildPromptMethod.Invoke(form, new object?[] { "Detect barcode, OCR text, faint marks, and contour symbols" }) as string
            ?? string.Empty;

        AssertAiRecipePromptContract(prompt);

        using Form reportForm = new()
        {
            Text = "AI Recipe Prompt Contract Check",
            Width = 980,
            Height = 560,
            StartPosition = FormStartPosition.CenterScreen,
            BackColor = Color.FromArgb(236, 242, 248)
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(18)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        Label titleLabel = new()
        {
            Text = "AI Recipe Prompt Contract",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            ForeColor = Color.FromArgb(16, 82, 124),
            TextAlign = ContentAlignment.MiddleLeft
        };

        Label summaryLabel = new()
        {
            Text = "OK - supported ToolTypes, form-only guard, sample-backed metrics, and final OverlayMerge review rule are present.",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(218, 238, 229),
            ForeColor = Color.FromArgb(16, 84, 46),
            Padding = new Padding(12, 8, 12, 8),
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };

        TextBox previewTextBox = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            TabStop = false,
            ScrollBars = ScrollBars.Vertical,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Consolas", 9f),
            Text = TruncatePromptPreview(prompt, 6500)
        };

        Label footerLabel = new()
        {
            Text = $"Prompt length: {prompt.Length:N0} chars",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(64, 80, 96),
            TextAlign = ContentAlignment.MiddleRight
        };

        layout.Controls.Add(titleLabel, 0, 0);
        layout.Controls.Add(summaryLabel, 0, 1);
        layout.Controls.Add(previewTextBox, 0, 2);
        layout.Controls.Add(footerLabel, 0, 3);
        reportForm.Controls.Add(layout);

        return CaptureForm(reportForm, outputPath, new Size(980, 560), 16, _ =>
        {
            previewTextBox.SelectionStart = 0;
            previewTextBox.SelectionLength = 0;
            titleLabel.Focus();
        });
    }

    private static CaptureDiagnostics CaptureAiRecipeFeedbackCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineLlmRecipe", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineLlmRecipe type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 2);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Main" },
            new Func<VisionPipelineContext>(() => new VisionPipelineContext())
        });

        form.Tag = formType;

        return CaptureForm(form, outputPath, new Size(1180, 760), 16, RunAiRecipeFeedbackCheck);
    }

    private static CaptureDiagnostics CaptureAiRecipeFailedStepFocusCheck(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormVisionPipelineLlmRecipe", throwOnError: true)
            ?? throw new InvalidOperationException("FormVisionPipelineLlmRecipe type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item => item.GetParameters().Length == 2);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            new[] { "Main" },
            new Func<VisionPipelineContext>(() => new VisionPipelineContext())
        });

        form.Tag = formType;

        return CaptureForm(form, outputPath, new Size(1180, 760), 16, RunAiRecipeFailedStepFocusCheck);
    }

    private static void RunAiRecipePreview(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        MethodInfo? sampleMethod = formType.GetMethod("OnSampleClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? runPreviewMethod = formType.GetMethod("OnRunPreviewClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        sampleMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        PumpUi(4);
        runPreviewMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        WaitForAiRecipePreview(form, formType, 160);
    }

    private static void RunAiRecipeFeedbackCheck(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        RunAiRecipePreview(form);

        FieldInfo? feedbackButtonField = formType.GetField("btnCopyFeedback", BindingFlags.Instance | BindingFlags.NonPublic);
        if (feedbackButtonField?.GetValue(form) is not Button feedbackButton)
        {
            throw new InvalidOperationException("Copy Feedback button was not found.");
        }

        if (!WaitForControlEnabled(feedbackButton, 80))
        {
            TextBox logText = FindControl<TextBox>(form, "tbLog")
                ?? throw new InvalidOperationException("AI Recipe log text box was not found.");
            TextBox feedbackPreviewText = FindControl<TextBox>(form, "tbFeedback")
                ?? throw new InvalidOperationException("AI Feedback text box was not found.");
            throw new InvalidOperationException(
                "Copy Feedback button was not enabled after Run Preview. "
                + $"ButtonEnabled={feedbackButton.Enabled}, Log={Truncate(logText.Text, 180)}, Feedback={Truncate(feedbackPreviewText.Text, 180)}");
        }

        feedbackButton.PerformClick();
        PumpUi(4);

        string feedback = ReadPrivateField<string>(form, formType, "latestFeedbackText") ?? string.Empty;
        if (!feedback.Contains("OpenVisionLab AI Recipe Feedback", StringComparison.Ordinal)
            || !feedback.Contains("Run Step Results:", StringComparison.Ordinal)
            || !feedback.Contains("Final Review Contract:", StringComparison.Ordinal)
            || !feedback.Contains("Suggested Next LLM Request:", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Copied LLM feedback did not contain the expected sections.");
        }

        TextBox overviewText = FindControl<TextBox>(form, "tbOverview")
            ?? throw new InvalidOperationException("Recipe Overview text box was not found.");
        TextBox flowText = FindControl<TextBox>(form, "tbFlow")
            ?? throw new InvalidOperationException("Step Flow text box was not found.");
        TextBox feedbackText = FindControl<TextBox>(form, "tbFeedback")
            ?? throw new InvalidOperationException("AI Feedback text box was not found.");

        if (!overviewText.Text.Contains("Run Preview OK", StringComparison.OrdinalIgnoreCase)
            || !flowText.Text.Contains("TextSymbol_Binary", StringComparison.Ordinal)
            || !flowText.Text.Contains("OK 01 Text", StringComparison.OrdinalIgnoreCase)
            || !feedbackText.Text.Contains("Ready for LLM retry", StringComparison.OrdinalIgnoreCase)
            || !feedbackText.Text.Contains("Final Review Contract", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("AI Recipe guide panels did not reflect the latest preview result.");
        }

        Button promptButton = FindControl<Button>(form, "btnPrompt")
            ?? throw new InvalidOperationException("Prompt button was not found.");
        if (!string.Equals(promptButton.Text, "Retry Prompt", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Expected prompt button to switch to Retry Prompt, but it was '{promptButton.Text}'.");
        }

        MethodInfo? promptMethod = formType.GetMethod("BuildLlmPrompt", BindingFlags.Instance | BindingFlags.NonPublic);
        string prompt = promptMethod?.Invoke(form, new object?[] { "Detect all keypad symbols and faint shapes." }) as string ?? string.Empty;
        if (!prompt.Contains("Previous OpenVisionLab Run Preview feedback:", StringComparison.Ordinal)
            || !prompt.Contains("OpenVisionLab AI Recipe Feedback", StringComparison.Ordinal)
            || !prompt.Contains("Revision request:", StringComparison.Ordinal)
            || !prompt.Contains("OverlayMerge", StringComparison.Ordinal)
            || !prompt.Contains("SourceLayers", StringComparison.Ordinal)
            || !prompt.Contains("MergeOverlayCount", StringComparison.Ordinal)
            || !prompt.Contains("GateStatus=OK", StringComparison.Ordinal)
            || !prompt.Contains("ArtifactIssueCount=0", StringComparison.Ordinal)
            || !prompt.Contains("MetadataIssueCount=0", StringComparison.Ordinal)
            || !prompt.Contains("Required examples are stable validation contracts", StringComparison.Ordinal)
            || !prompt.Contains("Expected gate:", StringComparison.Ordinal)
            || !prompt.Contains("result image, overlay image, and raw log", StringComparison.OrdinalIgnoreCase)
            || !prompt.Contains("first failed step, error code, diagnostic hint, suggested fix", StringComparison.OrdinalIgnoreCase)
            || !prompt.Contains("Final Review Contract NG", StringComparison.Ordinal)
            || !prompt.Contains("Change only the first failed step and directly dependent steps", StringComparison.Ordinal)
            || !prompt.Contains("Preserve every successful step", StringComparison.Ordinal)
            || !prompt.Contains("one final OverlayMerge review layer", StringComparison.Ordinal)
            || !prompt.Contains("object-level boxes", StringComparison.Ordinal)
            || !prompt.Contains("Contour_AllSymbolsAndFaint_LLM", StringComparison.Ordinal)
            || !prompt.Contains("Rice_Particle_Blob", StringComparison.Ordinal)
            || !prompt.Contains("Pins_LineGauge", StringComparison.Ordinal)
            || !prompt.Contains("Contour_TemplateMatching", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("AI prompt did not include the latest Run Preview feedback.");
        }
    }

    private static void RunAiRecipeFailedStepFocusCheck(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        MethodInfo? sampleMethod = formType.GetMethod("OnSampleClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? runPreviewMethod = formType.GetMethod("OnRunPreviewClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        sampleMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        PumpUi(4);

        TextBox xmlText = FindControl<TextBox>(form, "tbXml")
            ?? throw new InvalidOperationException("LLM XML text box was not found.");
        xmlText.Text = xmlText.Text.Replace(
            "<AcceptanceMetricMinimum>35</AcceptanceMetricMinimum>",
            "<AcceptanceMetricMinimum>999</AcceptanceMetricMinimum>",
            StringComparison.Ordinal).Replace(
            "<AcceptanceMetricMaximum>80</AcceptanceMetricMaximum>",
            "<AcceptanceMetricMaximum>1000</AcceptanceMetricMaximum>",
            StringComparison.Ordinal);
        PumpUi(4);

        runPreviewMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        WaitForAiRecipePreview(form, formType, 180);

        TextBox overviewText = FindControl<TextBox>(form, "tbOverview")
            ?? throw new InvalidOperationException("Recipe Overview text box was not found.");
        TextBox flowText = FindControl<TextBox>(form, "tbFlow")
            ?? throw new InvalidOperationException("Step Flow text box was not found.");
        TextBox feedbackText = FindControl<TextBox>(form, "tbFeedback")
            ?? throw new InvalidOperationException("AI Feedback text box was not found.");
        TextBox patchText = FindControl<TextBox>(form, "tbPatch")
            ?? throw new InvalidOperationException("XML Patch Request text box was not found.");
        TextBox logText = FindControl<TextBox>(form, "tbLog")
            ?? throw new InvalidOperationException("AI Recipe log text box was not found.");
        DataGridView stepGrid = FindControl<DataGridView>(form, "stepGrid")
            ?? throw new InvalidOperationException("Run Result grid was not found.");
        Button feedbackButton = FindControl<Button>(form, "btnCopyFeedback")
            ?? throw new InvalidOperationException("Copy Feedback button was not found.");
        Button patchButton = FindControl<Button>(form, "btnCopyPatch")
            ?? throw new InvalidOperationException("Copy Patch Request button was not found.");

        if (!overviewText.Text.Contains("Run Preview NG", StringComparison.OrdinalIgnoreCase)
            || !flowText.Text.Contains("NG 03", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "AI Recipe failed-step guide did not show the expected NG state. Overview="
                + Truncate(overviewText.Text, 180)
                + " | Flow="
                + Truncate(flowText.Text, 220));
        }

        string selectedStatus = stepGrid.CurrentRow == null
            ? string.Empty
            : Convert.ToString(stepGrid.CurrentRow.Cells[2].Value, CultureInfo.InvariantCulture) ?? string.Empty;
        if (stepGrid.CurrentRow == null
            || stepGrid.CurrentRow.Index != 2
            || !selectedStatus.Contains("NG", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Expected failed contour step row to be selected after Run Preview NG.");
        }

        if (!feedbackText.Text.Contains("Suggested Fix:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("Change Scope:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("Direct Dependents:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("Patch Proposal:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("XML fields:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("Metric context:", StringComparison.Ordinal)
            || !feedbackText.Text.Contains("Fix step 03", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "AI Feedback preview did not surface the failed-step fix scope. Actual="
                + Truncate(feedbackText.Text, 420));
        }

        if (!patchText.Text.Contains("XML Patch Request target:", StringComparison.Ordinal)
            || !patchText.Text.Contains("03. Text Symbol Contour [Contour]", StringComparison.OrdinalIgnoreCase)
            || !patchText.Text.Contains("Patch:", StringComparison.Ordinal)
            || !patchText.Text.Contains("full VisionPipeline XML", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "AI Recipe patch preview did not focus the selected failed step. Actual="
                + Truncate(patchText.Text, 260));
        }

        if (!logText.Text.Contains("FOCUS | First failed step 03 selected", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("AI Recipe log did not explain the selected failed step.");
        }

        if (!WaitForControlEnabled(feedbackButton, 30))
        {
            bool isBusy = ReadPrivateField<bool>(form, formType, "isBusy");
            string latestFeedback = ReadPrivateField<string>(form, formType, "latestFeedbackText") ?? string.Empty;
            Button? runPreviewButton = FindControl<Button>(form, "btnRunPreview");
            throw new InvalidOperationException(
                "Copy Feedback button was not enabled after failed Run Preview. "
                + $"ButtonEnabled={feedbackButton.Enabled}, RunButtonEnabled={runPreviewButton?.Enabled}, "
                + $"IsBusy={isBusy}, FeedbackLength={latestFeedback.Length}, "
                + $"FeedbackPreview={Truncate(feedbackText.Text, 180)}, Log={Truncate(logText.Text, 180)}");
        }

        if (!WaitForControlEnabled(patchButton, 30))
        {
            throw new InvalidOperationException(
                "Copy Patch Request button was not enabled after failed Run Preview. "
                + $"PatchPreview={Truncate(patchText.Text, 220)}");
        }

        patchButton.PerformClick();
        PumpUi(4);
        object? patchSummary = formType.GetMethod("GetPatchTargetSummary", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(form, null);
        string patchRequest = formType.GetMethod("BuildPatchRequestText", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(form, new[] { patchSummary }) as string ?? string.Empty;
        if (!patchRequest.Contains("OpenVisionLab AI Recipe XML Patch Request", StringComparison.Ordinal)
            || !patchRequest.Contains("Target Step: 03.", StringComparison.Ordinal)
            || !patchRequest.Contains("Current Step XML Reference:", StringComparison.Ordinal)
            || !patchRequest.Contains("<VisionPipelineStep>", StringComparison.Ordinal)
            || !patchRequest.Contains("Return the full <VisionPipeline> XML", StringComparison.Ordinal)
            || !patchRequest.Contains("Do not return only the step fragment", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Copied XML patch request did not contain the expected actionable contract.");
        }

        feedbackButton.PerformClick();
        PumpUi(4);
        string feedback = ReadPrivateField<string>(form, formType, "latestFeedbackText") ?? string.Empty;
        if (!feedback.Contains("Diagnostic:", StringComparison.Ordinal)
            || !feedback.Contains("Suggested Fix:", StringComparison.Ordinal)
            || !feedback.Contains("Fix step 03", StringComparison.Ordinal)
            || !feedback.Contains("Direct dependents:", StringComparison.Ordinal)
            || !feedback.Contains("Patch proposal:", StringComparison.Ordinal)
            || !feedback.Contains("XML fields:", StringComparison.Ordinal)
            || !feedback.Contains("Metric context:", StringComparison.Ordinal)
            || !feedback.Contains("Acceptance hint:", StringComparison.Ordinal)
            || !feedback.Contains("Change scope:", StringComparison.Ordinal)
            || !feedback.Contains("directly dependent steps only", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Failed-step LLM feedback did not include actionable diagnostic and fix text.");
        }
    }

    private static void WaitForAiRecipePreview(Form form, Type formType, int maxIterations)
    {
        FieldInfo? logField = formType.GetField("tbLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runButtonField = formType.GetField("btnRunPreview", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? feedbackButtonField = formType.GetField("btnCopyFeedback", BindingFlags.Instance | BindingFlags.NonPublic);
        for (int i = 0; i < maxIterations; i++)
        {
            Application.DoEvents();
            string logText = logField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;
            bool runFinished = logText.Contains("RUN OK", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("RUN NG", StringComparison.OrdinalIgnoreCase);
            bool buttonReady = runButtonField?.GetValue(form) is not Control runButton || runButton.Enabled;
            bool feedbackReady = feedbackButtonField?.GetValue(form) is not Control feedbackButton || feedbackButton.Enabled;
            if (runFinished && buttonReady && feedbackReady)
            {
                PumpUi(3);
                return;
            }

            Thread.Sleep(80);
        }
    }

    private static bool WaitForControlEnabled(Control control, int maxIterations)
    {
        for (int i = 0; i < maxIterations; i++)
        {
            Application.DoEvents();
            if (control.Enabled)
            {
                return true;
            }

            Thread.Sleep(80);
        }

        return control.Enabled;
    }

    private static T? ReadPrivateField<T>(object instance, Type type, string fieldName)
    {
        FieldInfo? field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        object? value = field?.GetValue(instance);
        if (value is T typed)
        {
            return typed;
        }

        return default;
    }

    private static CaptureDiagnostics CaptureThresholdForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormThreshold", throwOnError: true)
            ?? throw new InvalidOperationException("FormThreshold type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormThreshold could not be created."));
        return CaptureForm(form, outputPath, new Size(720, 720), 16, AssertThresholdFormContract);
    }

    private static void AssertThresholdFormContract(Form form)
    {
        Label previewTitle = FindControl<Label>(form, "lblPreviewTitle")
            ?? throw new InvalidOperationException("Threshold preview title label was not found.");
        Label previewDescription = FindControl<Label>(form, "lblPreviewDescription")
            ?? throw new InvalidOperationException("Threshold preview description label was not found.");
        Button addButton = FindControl<Button>(form, "btnAddToPipeline")
            ?? throw new InvalidOperationException("Threshold Add Pipeline button was not found.");

        AssertContains(previewTitle.Text, "Preview - Basic Threshold", "threshold preview mode title");
        AssertContains(previewDescription.Text, "Input Main -> Output Threshold", "threshold preview layer flow");
        AssertContains(previewDescription.Text, "one cutoff", "threshold basic mode purpose");
        AssertContains(addButton.Text, "Add Basic Threshold Step", "threshold add-step mode text");

        string allText = CollectControlText(form);
        AssertContains(allText, "Threshold (Preview)", "threshold output layer label");
        AssertContains(allText, "Basic Threshold", "threshold basic section");
        AssertContains(allText, "Range Threshold", "threshold range section");
        AssertContains(allText, "Adaptive Threshold", "threshold adaptive section");
    }

    private static CaptureDiagnostics CaptureMatchingToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Matching", new Size(920, 660), RunMatchingToolReviewSmoke);
    }

    private static void AssertMatchingToolFormContract(Form form)
    {
        GroupBox reviewGroup = FindControl<GroupBox>(form, "groupBoxMatchReview")
            ?? throw new InvalidOperationException("Matching Form should expose the Match Review panel.");
        PictureBox templateBox = FindControl<PictureBox>(form, "pbTemplate")
            ?? throw new InvalidOperationException("Matching Form should expose the template preview box.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "pbDetectedCrop")
            ?? throw new InvalidOperationException("Matching Form should expose the detected crop preview box.");
        Label summaryLabel = FindControl<Label>(form, "lblMatchSummary")
            ?? throw new InvalidOperationException("Matching Form should expose the match summary label.");

        if (reviewGroup.Width < 440 || reviewGroup.Height < 170)
        {
            throw new InvalidOperationException($"Matching Form Match Review panel is too small. Size={reviewGroup.Width}x{reviewGroup.Height}.");
        }

        if (templateBox.Width < 100 || detectedBox.Width < 100)
        {
            throw new InvalidOperationException("Matching Form template/detected crop previews are too narrow.");
        }

        string allText = CollectControlText(form);
        AssertContains(allText, "Match Review", "matching review group");
        AssertContains(allText, "Template to Match", "matching template preview title");
        AssertContains(allText, "Detected Crop", "matching detected crop preview title");
        AssertContains(allText, "Match Result", "matching result summary title");
        AssertContains(summaryLabel.Text, "Overlay", "matching output overlay summary");
    }

    private static void RunMatchingToolReviewSmoke(Form form)
    {
        AssertMatchingToolFormContract(form);

        string repoRoot = ResolveRepoRootForSmoke();
        string samplePath = Path.Combine(repoRoot, "Sample", "Contour.jpg");
        string templatePath = Path.Combine(repoRoot, "docs", "samples", "templates", "Contour_7PQRS_Template.png");
        if (!File.Exists(samplePath))
        {
            throw new InvalidOperationException("Matching smoke sample image was not found: " + samplePath);
        }

        if (!File.Exists(templatePath))
        {
            throw new InvalidOperationException("Matching smoke template image was not found: " + templatePath);
        }

        FieldInfo? propertyField = form.GetType().GetField("Property_Matching", BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyField?.GetValue(form) is not MatchingProperty property)
        {
            throw new InvalidOperationException("Matching Form property object was not found.");
        }

        property.PATTERN_PATH = templatePath;
        property.SCORE_MIN = 0.85D;
        property.NUM_MATCH = 1;
        property.USE_FIND_ANGLE = false;
        property.USE_THRESHOLD = false;
        property.USE_ADAPTIVE_THRESHOLD = false;

        VisionTestImageCanvas sourceViewer = FindControl<VisionTestImageCanvas>(form, "ibSource")
            ?? throw new InvalidOperationException("Matching Form source image viewer was not found.");
        using (Bitmap sampleImage = new Bitmap(samplePath))
        {
            sourceViewer.DisplayImage = new Bitmap(sampleImage);
        }

        Button runButton = FindControl<Button>(form, "btnRun")
            ?? throw new InvalidOperationException("Matching Form Run button was not found.");
        runButton.PerformClick();
        PumpUi(18);

        PictureBox templateBox = FindControl<PictureBox>(form, "pbTemplate")
            ?? throw new InvalidOperationException("Matching Form template preview box was not found after run.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "pbDetectedCrop")
            ?? throw new InvalidOperationException("Matching Form detected crop preview box was not found after run.");
        Label summaryLabel = FindControl<Label>(form, "lblMatchSummary")
            ?? throw new InvalidOperationException("Matching Form match summary label was not found after run.");

        if (templateBox.Image == null)
        {
            throw new InvalidOperationException("Matching Form template preview should be populated after run.");
        }

        if (detectedBox.Image == null)
        {
            throw new InvalidOperationException("Matching Form detected crop preview should be populated after run.");
        }

        AssertContains(summaryLabel.Text, "Score:", "matching review score");
        AssertContains(summaryLabel.Text, "Center:", "matching review center");
        AssertContains(summaryLabel.Text, "Overlay: Output", "matching review overlay target");
    }

    private static CaptureDiagnostics CaptureFeatureMatchingToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_FeatureMatching", new Size(920, 660), RunFeatureMatchingToolReviewSmoke);
    }

    private static void AssertFeatureMatchingToolFormContract(Form form)
    {
        GroupBox reviewGroup = FindControl<GroupBox>(form, "groupBoxFeatureReview")
            ?? throw new InvalidOperationException("FeatureMatching Form should expose the Feature Review panel.");
        PictureBox templateBox = FindControl<PictureBox>(form, "pbTemplate")
            ?? throw new InvalidOperationException("FeatureMatching Form should expose the template preview box.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "pbDetectedCrop")
            ?? throw new InvalidOperationException("FeatureMatching Form should expose the detected crop preview box.");
        Label summaryLabel = FindControl<Label>(form, "lblFeatureSummary")
            ?? throw new InvalidOperationException("FeatureMatching Form should expose the feature summary label.");

        if (reviewGroup.Width < 440 || reviewGroup.Height < 170)
        {
            throw new InvalidOperationException($"FeatureMatching Form Feature Review panel is too small. Size={reviewGroup.Width}x{reviewGroup.Height}.");
        }

        if (templateBox.Width < 100 || detectedBox.Width < 100)
        {
            throw new InvalidOperationException("FeatureMatching Form template/detected crop previews are too narrow.");
        }

        string allText = CollectControlText(form);
        AssertContains(allText, "Feature Review", "feature matching review group");
        AssertContains(allText, "Feature Template", "feature matching template preview title");
        AssertContains(allText, "Detected Crop", "feature matching detected crop preview title");
        AssertContains(allText, "Feature Result", "feature matching result summary title");
        AssertContains(summaryLabel.Text, "Overlay", "feature matching output overlay summary");
    }

    private static void RunFeatureMatchingToolReviewSmoke(Form form)
    {
        AssertFeatureMatchingToolFormContract(form);

        FieldInfo? propertyField = form.GetType().GetField("Property_FeatureMatching", BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyField?.GetValue(form) is not OpenVisionLab.Vision._1._Tools.OpenCV.FeatureMatchingProperty property)
        {
            throw new InvalidOperationException("FeatureMatching Form property object was not found.");
        }

        string templatePath = Path.Combine(Path.GetTempPath(), "OpenVisionLab_FeatureToolTemplate.png");
        using (OpenCvSharp.Mat source = CreateFeatureMatchingSourceImage(out OpenCvSharp.Rect templateRect, out _))
        using (OpenCvSharp.Mat template = source.SubMat(templateRect).Clone())
        using (Bitmap sourceBitmap = BitmapImageConverter.ToBitmap(source))
        {
            OpenCvSharp.Cv2.ImWrite(templatePath, template);
            VisionTestImageCanvas sourceViewer = FindControl<VisionTestImageCanvas>(form, "ibSource")
                ?? throw new InvalidOperationException("FeatureMatching Form source image viewer was not found.");
            sourceViewer.DisplayImage = new Bitmap(sourceBitmap);
        }

        property.PATTERN_PATH = templatePath;
        property.SCORE_MIN = 0.95D;
        property.RANSAC_REPROJ_THRESHOLD = 5D;
        property.USE_ROI = false;
        property.USE_MULTI_ROI = false;
        property.USE_THRESHOLD = false;
        property.USE_ADAPTIVE_THRESHOLD = false;

        Button runButton = FindControl<Button>(form, "btnRun")
            ?? throw new InvalidOperationException("FeatureMatching Form Run button was not found.");
        runButton.PerformClick();
        PumpUi(24);

        PictureBox templateBox = FindControl<PictureBox>(form, "pbTemplate")
            ?? throw new InvalidOperationException("FeatureMatching Form template preview box was not found after run.");
        PictureBox detectedBox = FindControl<PictureBox>(form, "pbDetectedCrop")
            ?? throw new InvalidOperationException("FeatureMatching Form detected crop preview box was not found after run.");
        Label summaryLabel = FindControl<Label>(form, "lblFeatureSummary")
            ?? throw new InvalidOperationException("FeatureMatching Form feature summary label was not found after run.");

        if (templateBox.Image == null)
        {
            throw new InvalidOperationException("FeatureMatching Form template preview should be populated after run.");
        }

        if (detectedBox.Image == null)
        {
            throw new InvalidOperationException("FeatureMatching Form detected crop preview should be populated after run.");
        }

        AssertContains(summaryLabel.Text, "Score:", "feature matching review score");
        AssertContains(summaryLabel.Text, "Center:", "feature matching review center");
        AssertContains(summaryLabel.Text, "Angle:", "feature matching review angle");
        AssertContains(summaryLabel.Text, "Overlay: Output", "feature matching review overlay target");
    }

    private static CaptureDiagnostics CaptureContourToolForm(string outputPath)
    {
        return CaptureVisionToolForm(
            outputPath,
            "OpenVisionLab.FormVision_Contour",
            new Size(920, 660),
            form => InjectToolFormPreviewImages(
                form,
                Path.Combine("Sample", "Contour.jpg"),
                Path.Combine("docs", "assets", "tutorial", "tool_contour_result.png")));
    }

    private static CaptureDiagnostics CaptureBlobToolForm(string outputPath)
    {
        return CaptureVisionToolForm(
            outputPath,
            "OpenVisionLab.FormVision_Blob",
            new Size(920, 660),
            form => InjectToolFormPreviewImages(
                form,
                Path.Combine("Sample", "Rice.jpg"),
                Path.Combine("docs", "assets", "tutorial", "tool_blob_result.png")));
    }

    private static CaptureDiagnostics CaptureLineToolForm(string outputPath)
    {
        return CaptureVisionToolForm(
            outputPath,
            "OpenVisionLab.FormVision_Line",
            new Size(920, 660),
            form => InjectToolFormPreviewImages(
                form,
                Path.Combine("Sample", "Pins.bmp"),
                Path.Combine("docs", "assets", "tutorial", "tool_line_result.png")));
    }

    private static CaptureDiagnostics CaptureMorphologyToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Morphology", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureFilterToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Filter", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureArithmeticToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Arithmetic", new Size(980, 720));
    }

    private static CaptureDiagnostics CaptureEdgeDetectionToolForm(string outputPath)
    {
        return CaptureVisionToolForm(
            outputPath,
            "OpenVisionLab.FormVision_EdgeDetection",
            new Size(920, 660),
            form => InjectToolFormPreviewImages(
                form,
                Path.Combine("Sample", "Pins.bmp"),
                Path.Combine("docs", "assets", "tutorial", "tool_edge_result.png")));
    }

    private static CaptureDiagnostics CaptureRotateAndScaleToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_RotateAndScale", new Size(920, 660), RunRotateScalePreviewSmoke);
    }

    private static CaptureDiagnostics CaptureHistogramToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Histogram", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureMeanToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Mean", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureHsvToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_HSV", new Size(920, 660), RunHsvPreviewSmoke);
    }

    private static CaptureDiagnostics CaptureVisionToolForm(
        string outputPath,
        string typeName,
        Size size,
        Action<Form>? afterShow = null)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType(typeName, throwOnError: true)
            ?? throw new InvalidOperationException($"{typeName} type was not found.");
        ConstructorInfo constructor = formType
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(item =>
            {
                ParameterInfo[] parameters = item.GetParameters();
                return parameters.Length == 2
                    && typeof(IDisplayManager).IsAssignableFrom(parameters[0].ParameterType)
                    && parameters[1].ParameterType == typeof(EventHandler<DockDisplayEventArgs>);
            });

        using MemoryDisplayManager displayManager = new();
        using Bitmap mainImage = CreatePipelinePreviewRunImage();
        displayManager.SetLayer("Main", mainImage);
        displayManager.SetLayer("NewPanel_Output", mainImage);

        using Form form = (Form)constructor.Invoke(new object?[]
        {
            displayManager,
            new EventHandler<DockDisplayEventArgs>((_, _) => { })
        });

        return CaptureForm(
            form,
            outputPath,
            size,
            14,
            shownForm =>
            {
                NormalizeVisionToolFormLayout(shownForm);
                afterShow?.Invoke(shownForm);
                NormalizeVisionToolFormLayout(shownForm);
            },
            useScreenCapture: true);
    }

    private static void NormalizeVisionToolFormLayout(Form form)
    {
        Control? clientArea = FindControl<Control>(form, "pnlClientArea");
        clientArea?.SendToBack();

        foreach (string name in new[]
        {
            "groupBox3",
            "groupBox4",
            "groupBox1",
            "Tab",
            "btnResult",
            "btnRun",
            "btnFilterRun",
            "rjButton1",
            "btnVerLineRun"
        })
        {
            Control? control = FindControl<Control>(form, name);
            if (control == null)
            {
                continue;
            }

            if (clientArea != null && control.Parent != clientArea)
            {
                Point screenLocation = control.PointToScreen(Point.Empty);
                Point clientLocation = clientArea.PointToScreen(Point.Empty);
                control.Parent?.Controls.Remove(control);
                clientArea.Controls.Add(control);
                control.Location = new Point(
                    screenLocation.X - clientLocation.X,
                    screenLocation.Y - clientLocation.Y);
            }

            control.BringToFront();
        }
    }

    private static void InjectToolFormPreviewImages(Form form, string sourceRelativePath, string resultRelativePath)
    {
        string repoRoot = ResolveRepoRootForSmoke();
        SetToolFormCanvasImage(form, "ibSource", Path.Combine(repoRoot, sourceRelativePath));
        SetToolFormCanvasImage(form, "ibDestination", Path.Combine(repoRoot, resultRelativePath));
    }

    private static void SetToolFormCanvasImage(Form form, string controlName, string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new InvalidOperationException($"Tool form tutorial image was not found: {imagePath}");
        }

        VisionTestImageCanvas canvas = FindControl<VisionTestImageCanvas>(form, controlName)
            ?? throw new InvalidOperationException($"Tool form image canvas was not found: {controlName}");
        using Bitmap image = new(imagePath);
        canvas.DisplayImage = new Bitmap(image);
    }

    private static void RunRotateScalePreviewSmoke(Form form)
    {
        Type formType = form.GetType();
        SetTrackBarValue(form, "trbRotate", 35);
        SetTrackBarValue(form, "trbScaleX", 120);
        SetTrackBarValue(form, "trbScaleY", 90);

        FieldInfo? scaleField = formType.GetField("trbScaleY", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? scrollMethod = formType.GetMethod("trbScale_Scroll", BindingFlags.Instance | BindingFlags.NonPublic);
        if (scaleField?.GetValue(form) is TrackBar trackBar && scrollMethod != null)
        {
            scrollMethod.Invoke(form, new object?[] { trackBar, EventArgs.Empty });
            PumpUi(4);
        }
    }

    private static void RunHsvPreviewSmoke(Form form)
    {
        Type formType = form.GetType();
        SetTrackBarValue(form, "trbHueMin", 0);
        SetTrackBarValue(form, "trbHueMax", 179);
        SetTrackBarValue(form, "trbSatMin", 0);
        SetTrackBarValue(form, "trbSatMax", 255);
        SetTrackBarValue(form, "trbValMin", 0);
        SetTrackBarValue(form, "trbValMax", 255);

        FieldInfo? valueField = formType.GetField("trbValMax", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? scrollMethod = formType.GetMethod("trbHsv_Scroll", BindingFlags.Instance | BindingFlags.NonPublic);
        if (valueField?.GetValue(form) is TrackBar trackBar && scrollMethod != null)
        {
            scrollMethod.Invoke(form, new object?[] { trackBar, EventArgs.Empty });
            PumpUi(4);
        }
    }

    private static void SetTrackBarValue(Form form, string fieldName, int value)
    {
        FieldInfo? field = form.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field?.GetValue(form) is not TrackBar trackBar)
        {
            return;
        }

        trackBar.Value = Math.Min(trackBar.Maximum, Math.Max(trackBar.Minimum, value));
    }

    private static CaptureDiagnostics CaptureMessageBoxForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline Warning",
            Message = "Contour step uses a branch input. Check the input/output layer before publishing.",
            Kind = VisionMessageKind.Warning,
            Buttons = MessageBoxButtons.OKCancel,
            PrimaryText = "Review",
            SecondaryText = "Close",
            PrimaryResult = DialogResult.OK,
            SecondaryResult = DialogResult.Cancel
        });
    }

    private static CaptureDiagnostics CaptureMessageBoxInfoForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline",
            Message = "Pipeline project saved.",
            Kind = VisionMessageKind.Info,
            Buttons = MessageBoxButtons.OK
        });
    }

    private static CaptureDiagnostics CaptureMessageBoxWarningForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline Check",
            Message = "The selected step has no cached output layer. Run preview before publishing.",
            Kind = VisionMessageKind.Warning,
            Buttons = MessageBoxButtons.OK
        });
    }

    private static CaptureDiagnostics CaptureMessageBoxErrorForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline Error",
            Message = "Pipeline execution failed. Check the technical details for the original exception.",
            Details = "System.InvalidOperationException: Sample detail text for smoke capture.\r\n   at OpenVisionLab.Pipeline.Step.Run()",
            Kind = VisionMessageKind.Error,
            Buttons = MessageBoxButtons.OK
        });
    }

    private static CaptureDiagnostics CaptureMessageBoxErrorDetailsForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline Error",
            Message = "Pipeline execution failed. Check the technical details for the original exception.",
            Details = "System.InvalidOperationException: Sample detail text for smoke capture.\r\n   at OpenVisionLab.Pipeline.Step.Run()",
            Kind = VisionMessageKind.Error,
            Buttons = MessageBoxButtons.OK
        }, expandDetails: true);
    }

    private static CaptureDiagnostics CaptureMessageBoxConfirmForm(string outputPath)
    {
        return CaptureVisionMessageBox(outputPath, new VisionMessageOptions
        {
            Title = "Pipeline Samples",
            Message = "Delete sample 'Contour_TextSymbols'?",
            Kind = VisionMessageKind.Question,
            Buttons = MessageBoxButtons.YesNo,
            PrimaryText = "Yes",
            SecondaryText = "No"
        });
    }

    private static CaptureDiagnostics CaptureVisionMessageBox(
        string outputPath,
        VisionMessageOptions options,
        bool expandDetails = false)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        using VisionMessageBoxForm form = new(options);
        return CaptureForm(form, outputPath, new Size(620, 270), 12, shownForm =>
        {
            if (expandDetails)
            {
                ExpandMessageDetails(shownForm);
            }

            AssertVisionMessageBoxContract(shownForm, options, expandDetails);
        });
    }

    private static void AssertVisionMessageBoxContract(Form form, VisionMessageOptions options, bool detailsExpanded)
    {
        Label titleLabel = FindControl<Label>(form, "titleLabel") ?? ReadPrivateField<Label>(form, form.GetType(), "titleLabel")
            ?? throw new InvalidOperationException("Message box title label was not found.");
        Label messageLabel = FindControl<Label>(form, "messageLabel") ?? ReadPrivateField<Label>(form, form.GetType(), "messageLabel")
            ?? throw new InvalidOperationException("Message box message label was not found.");

        AssertContains(titleLabel.Text, string.IsNullOrWhiteSpace(options.Title) ? "Message" : options.Title, "message box title");
        AssertContains(messageLabel.Text, string.IsNullOrWhiteSpace(options.Message) ? "-" : options.Message.Split('\r', '\n').FirstOrDefault() ?? "-", "message box message");

        Button? detailsButton = FindControl<Button>(form, "detailsButton") ?? ReadPrivateField<Button>(form, form.GetType(), "detailsButton");
        if (string.IsNullOrWhiteSpace(options.Details))
        {
            if (detailsButton != null && detailsButton.Visible)
            {
                throw new InvalidOperationException("Message box details button should be hidden when details are empty.");
            }

            return;
        }

        if (detailsButton == null || !detailsButton.Visible)
        {
            throw new InvalidOperationException("Message box details button should be visible when details exist.");
        }

        AssertContains(detailsButton.Text, detailsExpanded ? "Hide Details" : "Technical Details", "message box details action");

        if (detailsExpanded)
        {
            Button copyButton = FindControl<Button>(form, "copyDetailsButton") ?? ReadPrivateField<Button>(form, form.GetType(), "copyDetailsButton")
                ?? throw new InvalidOperationException("Message box copy details button was not found.");
            TextBox detailsText = FindControl<TextBox>(form, "detailsTextBox") ?? ReadPrivateField<TextBox>(form, form.GetType(), "detailsTextBox")
                ?? throw new InvalidOperationException("Message box details text box was not found.");

            AssertContains(copyButton.Text, "Copy Details", "message box copy details action");
            AssertContains(detailsText.Text, options.Details.Split('\r', '\n').FirstOrDefault() ?? string.Empty, "message box detail content");
        }
    }

    private static void ExpandMessageDetails(Form form)
    {
        if (FindControl<Button>(form, "detailsButton") is Button detailsButton && detailsButton.Visible)
        {
            detailsButton.PerformClick();
            PumpUi(3);
        }
    }

    private static T? FindControl<T>(Control root, string name)
        where T : Control
    {
        foreach (Control child in root.Controls)
        {
            if (child is T typed && string.Equals(child.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return typed;
            }

            T? nested = FindControl<T>(child, name);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private static string CollectControlText(Control root)
    {
        List<string> values = new();
        CollectControlText(root, values);
        return string.Join("\r\n", values);
    }

    private static void CollectControlText(Control control, List<string> values)
    {
        if (!string.IsNullOrWhiteSpace(control.Text))
        {
            values.Add(control.Text);
        }

        if (control is ToolStrip toolStrip)
        {
            CollectToolStripText(toolStrip.Items, values);
        }

        if (control is ElementHost { Child: not null } host)
        {
            CollectWpfText(host.Child, values);
        }

        foreach (Control child in control.Controls)
        {
            CollectControlText(child, values);
        }
    }

    private static void CollectToolStripText(ToolStripItemCollection items, List<string> values)
    {
        foreach (ToolStripItem item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                values.Add(item.Text);
            }

            if (item is ToolStripDropDownItem dropDownItem)
            {
                CollectToolStripText(dropDownItem.DropDownItems, values);
            }
        }
    }

    private static void CollectWpfText(System.Windows.DependencyObject visual, List<string> values)
    {
        switch (visual)
        {
            case System.Windows.Controls.TextBlock textBlock when !string.IsNullOrWhiteSpace(textBlock.Text):
                values.Add(textBlock.Text);
                break;
            case System.Windows.Controls.TextBox textBox when !string.IsNullOrWhiteSpace(textBox.Text):
                values.Add(textBox.Text);
                break;
            case System.Windows.Controls.ComboBox comboBox when !string.IsNullOrWhiteSpace(comboBox.Text):
                values.Add(comboBox.Text);
                break;
            case System.Windows.Controls.ContentControl contentControl when contentControl.Content != null:
                string contentText = contentControl.Content.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(contentText) && !ContainsInternalTypeText(contentText))
                {
                    values.Add(contentText);
                }
                break;
        }

        int childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(visual);
        for (int i = 0; i < childCount; i++)
        {
            CollectWpfText(System.Windows.Media.VisualTreeHelper.GetChild(visual, i), values);
        }
    }

    private static CaptureDiagnostics CaptureMainWorkspace(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormTeachingVision", throwOnError: true)
            ?? throw new InvalidOperationException("FormTeachingVision type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormTeachingVision could not be created."));
        return CaptureForm(form, outputPath, new Size(1600, 900), 28, InjectMainWorkspaceImage, useScreenCapture: true);
    }

    private static CaptureDiagnostics CaptureForm(
        Form form,
        string outputPath,
        Size size,
        int pumpIterations,
        Action<Form>? afterShow = null,
        bool useScreenCapture = false)
    {
        bool visibleScreenCapture = useScreenCapture && !quietCaptureMode;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = quietCaptureMode ? new Point(-32000, -32000) : new Point(40, 40);
        form.Size = size;
        form.ShowInTaskbar = false;
        form.TopMost = visibleScreenCapture;
        Form? captureBackdrop = null;
        try
        {
            if (visibleScreenCapture)
            {
                captureBackdrop = new Form
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = form.Location,
                    Size = form.Size,
                    ShowInTaskbar = false,
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = Color.FromArgb(238, 242, 246),
                    TopMost = false
                };
                captureBackdrop.Show();
                captureBackdrop.BringToFront();
                PumpUi(2);
            }

            form.Show();
            if (!quietCaptureMode)
            {
                form.BringToFront();
                form.Activate();
            }

            PumpUi(pumpIterations);
            afterShow?.Invoke(form);
            if (afterShow != null)
            {
                if (!quietCaptureMode)
                {
                    form.BringToFront();
                    form.Activate();
                }

                PumpUi(Math.Max(8, pumpIterations / 2));
            }

            if (visibleScreenCapture)
            {
                Cursor.Position = form.PointToScreen(new Point(form.Width + 24, form.Height + 24));
                PumpUi(4);
            }

            CaptureDiagnostics diagnostics = AnalyzeControlLayout(form);
            using Bitmap capture = new Bitmap(Math.Max(1, form.Width), Math.Max(1, form.Height));
            if (visibleScreenCapture)
            {
                using Graphics graphics = Graphics.FromImage(capture);
                bool copiedFromScreen = false;
                try
                {
                    graphics.CopyFromScreen(form.PointToScreen(Point.Empty), Point.Empty, form.Size);
                    copiedFromScreen = true;
                }
                catch (Win32Exception)
                {
                    copiedFromScreen = false;
                }

                if (!copiedFromScreen || LooksLikeExternalScreenCapture(capture))
                {
                    form.DrawToBitmap(capture, new Rectangle(Point.Empty, form.Size));
                }
            }
            else
            {
                form.DrawToBitmap(capture, new Rectangle(Point.Empty, form.Size));
            }

            CompositeVisionTestImageCanvases(form, capture);
            CompositeElementHosts(form, capture);
            capture.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            return diagnostics;
        }
        finally
        {
            if (!form.IsDisposed)
            {
                form.Close();
            }

            if (captureBackdrop != null && !captureBackdrop.IsDisposed)
            {
                captureBackdrop.Close();
            }
        }
    }

    private static void CompositeVisionTestImageCanvases(Control root, Bitmap capture)
    {
        foreach (VisionTestImageCanvas canvas in EnumerateControls(root).OfType<VisionTestImageCanvas>())
        {
            Bitmap? displayBitmap = canvas.DisplayBitmap;
            if (displayBitmap == null || displayBitmap.Width <= 0 || displayBitmap.Height <= 0)
            {
                continue;
            }

            Rectangle bounds = GetControlBoundsRelativeTo(root, canvas);
            if (bounds.Width <= 4 || bounds.Height <= 4)
            {
                continue;
            }

            using Graphics graphics = Graphics.FromImage(capture);
            graphics.FillRectangle(Brushes.Black, bounds);
            Rectangle imageBounds = FitImageBounds(displayBitmap.Size, bounds);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(displayBitmap, imageBounds);
        }
    }

    private static Rectangle FitImageBounds(Size imageSize, Rectangle bounds)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return Rectangle.Empty;
        }

        double scale = Math.Min(
            bounds.Width / (double)imageSize.Width,
            bounds.Height / (double)imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale));
        int x = bounds.X + (bounds.Width - width) / 2;
        int y = bounds.Y + (bounds.Height - height) / 2;
        return new Rectangle(x, y, width, height);
    }

    private static Rectangle GetControlBoundsRelativeTo(Control root, Control control)
    {
        Point location = Point.Empty;
        Control? current = control;
        while (current != null && current != root)
        {
            location.Offset(current.Left, current.Top);
            current = current.Parent;
        }

        return new Rectangle(location, control.Size);
    }

    private static void CompositeElementHosts(Control root, Bitmap capture)
    {
        List<ElementHost> hosts = EnumerateControls(root).OfType<ElementHost>().ToList();
        if (hosts.Count == 0)
        {
            return;
        }

        Point formScreen = root.PointToScreen(Point.Empty);
        using Graphics graphics = Graphics.FromImage(capture);
        foreach (ElementHost host in hosts)
        {
            if (!host.Visible || host.Width <= 0 || host.Height <= 0 || host.Child == null)
            {
                continue;
            }

            Bitmap? rendered = RenderElementHost(host);
            if (rendered == null)
            {
                continue;
            }

            Point hostScreen = host.PointToScreen(Point.Empty);
            Point target = new Point(hostScreen.X - formScreen.X, hostScreen.Y - formScreen.Y);
            using (rendered)
            {
                graphics.DrawImageUnscaled(rendered, target);
            }
        }
    }

    private static IEnumerable<Control> EnumerateControls(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (Control descendant in EnumerateControls(child))
            {
                yield return descendant;
            }
        }
    }

    private static Bitmap? RenderElementHost(ElementHost host)
    {
        int width = Math.Max(1, host.Width);
        int height = Math.Max(1, host.Height);
        System.Windows.UIElement visual = host.Child;

        if (visual.Dispatcher != null && !visual.Dispatcher.CheckAccess())
        {
            return null;
        }

        try
        {
            visual.Measure(new System.Windows.Size(width, height));
            visual.Arrange(new System.Windows.Rect(0, 0, width, height));
            visual.UpdateLayout();

            System.Windows.Media.Imaging.RenderTargetBitmap renderTarget =
                new System.Windows.Media.Imaging.RenderTargetBitmap(
                    width,
                    height,
                    96,
                    96,
                    System.Windows.Media.PixelFormats.Pbgra32);
            renderTarget.Render(visual);

            int stride = width * 4;
            byte[] pixels = new byte[stride * height];
            renderTarget.CopyPixels(pixels, stride, 0);

            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            try
            {
                Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    private static bool LooksLikeExternalScreenCapture(Bitmap capture)
    {
        if (capture.Width < 12 || capture.Height < 12)
        {
            return false;
        }

        Color chrome = capture.GetPixel(5, 5);
        bool blueWindowChrome =
            chrome.B > chrome.R + 20
            && chrome.B > chrome.G + 20
            && chrome.R < 140;
        bool darkWindowChrome =
            chrome.R < 100
            && chrome.G < 110
            && chrome.B < 150;
        if (blueWindowChrome || darkWindowChrome)
        {
            return false;
        }

        bool imageLikeTopLeft =
            chrome.R > 160
            && chrome.G > 90
            && chrome.B > 70;
        return imageLikeTopLeft;
    }

    private static void InjectMainWorkspaceImage(Form form)
    {
        FieldInfo? field = form.GetType().GetField("displayManager", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field?.GetValue(form) is not IDisplayManager displayManager)
        {
            return;
        }

        Bitmap sample = CreateMainWorkspaceImage();
        form.Disposed += (sender, e) => sample.Dispose();
        displayManager.CreateLayerDisplay(ImageSpaceFrame.FromBitmap(sample), "Main", false);
        int mainLayerIndex = displayManager.FindIndex("Main");
        if (mainLayerIndex >= 0)
        {
            displayManager.SetLayerImage(mainLayerIndex, sample);
            displayManager.RefreshLayer(mainLayerIndex);
        }

        displayManager.SelectedItem = "Main";
        displayManager.ImageSpace.SetActiveImage(sample);
        displayManager.ActivateLayer("Main");
        displayManager.ZoomLayerToFit("Main");
        displayManager.NotifyParameterChanged();
        InvokeFormMethod(form, "RefreshLayerResultPanel");
        InvokeFormMethod(form, "RefreshToolbarStatus", new object?[] { null });
        AssertMainWorkspaceLayerResultState(form, displayManager, mainLayerIndex);
    }

    private static void AssertMainWorkspaceLayerResultState(Form form, IDisplayManager displayManager, int mainLayerIndex)
    {
        if (mainLayerIndex < 0)
        {
            throw new InvalidOperationException("Main layer was not created.");
        }

        Bitmap mainImage = displayManager.GetLayerImage(mainLayerIndex);
        if (DisplayManagerImageExtensions.IsPlaceholderBitmap(mainImage))
        {
            throw new InvalidOperationException("Main layer image was not stored in the display manager.");
        }

        FieldInfo? listField = form.GetType().GetField("lstLayerResults", BindingFlags.Instance | BindingFlags.NonPublic);
        if (listField?.GetValue(form) is not ListBox layerList || layerList.Items.Count == 0)
        {
            throw new InvalidOperationException("Main layer result list was not populated.");
        }

        string layerText = string.Join("\r\n", layerList.Items.Cast<object>().Select(item => item?.ToString() ?? string.Empty));
        if (!layerText.Contains("Main", StringComparison.OrdinalIgnoreCase)
            || !layerText.Contains("768x576", StringComparison.OrdinalIgnoreCase)
            || !layerText.Contains("기준", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Main layer result list should show the source layer role and stored image size. Actual=" + layerText);
        }

        Label activeLayerLabel = ReadPrivateField<Label>(form, form.GetType(), "lblToolbarActiveLayer")
            ?? throw new InvalidOperationException("Main toolbar active layer label was not found.");
        Label sourceModeLabel = ReadPrivateField<Label>(form, form.GetType(), "lblToolbarSourceMode")
            ?? throw new InvalidOperationException("Main toolbar source mode label was not found.");
        Label flowLabel = ReadPrivateField<Label>(form, form.GetType(), "lblToolbarFlow")
            ?? throw new InvalidOperationException("Main toolbar flow label was not found.");

        AssertContains(activeLayerLabel.Text, "Main", "main toolbar active layer");
        AssertContains(activeLayerLabel.Text, "1", "main toolbar layer count");
        AssertContains(sourceModeLabel.Text, "Main", "main toolbar source mode");
        AssertContains(flowLabel.Text, "Main", "main toolbar flow input");

        string allText = CollectControlText(form);
        AssertContains(allText, "Guide", "main guide menu");

        MethodInfo? resolveDocumentationPath = form.GetType().GetMethod("ResolveDocumentationPath", BindingFlags.Static | BindingFlags.NonPublic);
        if (resolveDocumentationPath == null)
        {
            throw new InvalidOperationException("Main guide documentation resolver was not found.");
        }

        string? guidePath = resolveDocumentationPath.Invoke(null, new object?[] { "OPENVISIONLAB_TUTORIAL.html" }) as string;
        if (string.IsNullOrWhiteSpace(guidePath) || !File.Exists(guidePath))
        {
            throw new InvalidOperationException("Main guide tutorial document could not be resolved. Path=" + (guidePath ?? string.Empty));
        }

        string html = File.ReadAllText(guidePath);
        foreach (string requiredText in new[]
        {
            "검사 폼 기반 Tool 티칭 가이드",
            "Contour",
            "Blob",
            "Pattern Matching",
            "FeatureMatching",
            "EdgeDetection",
            "LineGauge",
            "거리 / 치수 측정",
            "Pixel/mm",
            "Input Layer",
            "Output Layer",
            "Input / Output Flow Checklist",
            "BRANCH IN",
            "Link Prev",
            "Good / Bad Sample Pair Check",
            "BoundsWidthMax",
            "Tool Form",
            "전체 Form",
            "Pipeline Matching Review",
            "Detected Crop",
            "Feature_TemplateReview",
            "Text_Binary",
            "Pin_Line"
        })
        {
            if (!html.Contains(requiredText, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Main guide tutorial should include tool test guide text: " + requiredText);
            }
        }

        foreach (string imagePath in new[]
        {
            "assets/tutorial/main_workspace.png",
            "assets/tutorial/threshold_form.png",
            "assets/tutorial/pipeline_form.png",
            "assets/tutorial/sample_catalog_preview.png",
            "assets/tutorial/ai_recipe_form.png",
            "assets/tutorial/tool_contour_form.png",
            "assets/tutorial/tool_contour_result.png",
            "assets/tutorial/tool_blob_form.png",
            "assets/tutorial/tool_blob_result.png",
            "assets/tutorial/tool_matching_form.png",
            "assets/tutorial/tool_matching_template.png",
            "assets/tutorial/tool_matching_detected_crop.png",
            "assets/tutorial/tool_matching_result.png",
            "assets/tutorial/pipeline_matching_review.png",
            "assets/tutorial/tool_feature_matching_form.png",
            "assets/tutorial/pipeline_feature_matching_review.png",
            "assets/tutorial/feature_template_source.png",
            "assets/tutorial/feature_template_template.png",
            "assets/tutorial/feature_template_result.png",
            "assets/tutorial/tool_edge_detection_form.png",
            "assets/tutorial/tool_edge_result.png",
            "assets/tutorial/tool_line_form.png",
            "assets/tutorial/tool_line_result.png",
            "assets/tutorial/tool_vertical_measurement_result.png"
        })
        {
            if (!html.Contains(imagePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Main guide tutorial should reference image asset: " + imagePath);
            }

            string absoluteImagePath = Path.Combine(Path.GetDirectoryName(guidePath) ?? string.Empty, imagePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absoluteImagePath))
            {
                throw new InvalidOperationException("Main guide tutorial image asset was not found: " + absoluteImagePath);
            }
        }
    }

    private static void InvokeFormMethod(Form form, string methodName, object?[]? arguments = null)
    {
        MethodInfo? method = form.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(form, arguments);
    }

    private static ScreenshotAnalysis AnalyzeScreenshot(string path)
    {
        if (!File.Exists(path))
        {
            return ScreenshotAnalysis.Missing;
        }

        using Bitmap bitmap = new Bitmap(path);
        HashSet<int> colors = new();
        long brightnessTotal = 0;
        int sampled = 0;
        int stepX = Math.Max(1, bitmap.Width / 80);
        int stepY = Math.Max(1, bitmap.Height / 60);

        for (int y = 0; y < bitmap.Height; y += stepY)
        {
            for (int x = 0; x < bitmap.Width; x += stepX)
            {
                Color color = bitmap.GetPixel(x, y);
                colors.Add(color.ToArgb());
                brightnessTotal += color.R + color.G + color.B;
                sampled++;
            }
        }

        double averageBrightness = sampled == 0 ? 0 : brightnessTotal / (sampled * 3.0);
        double flatTilePercent = CalculateFlatTilePercent(bitmap);
        return new ScreenshotAnalysis(
            bitmap.Width,
            bitmap.Height,
            colors.Count,
            averageBrightness,
            flatTilePercent);
    }

    private static double CalculateFlatTilePercent(Bitmap bitmap)
    {
        const int columns = 8;
        const int rows = 5;
        int flatTiles = 0;
        int totalTiles = columns * rows;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int left = column * bitmap.Width / columns;
                int right = (column + 1) * bitmap.Width / columns;
                int top = row * bitmap.Height / rows;
                int bottom = (row + 1) * bitmap.Height / rows;
                if (IsFlatTile(bitmap, left, top, Math.Max(1, right - left), Math.Max(1, bottom - top)))
                {
                    flatTiles++;
                }
            }
        }

        return totalTiles == 0 ? 0 : flatTiles * 100.0 / totalTiles;
    }

    private static bool IsFlatTile(Bitmap bitmap, int left, int top, int width, int height)
    {
        HashSet<int> colors = new();
        int stepX = Math.Max(1, width / 10);
        int stepY = Math.Max(1, height / 8);
        for (int y = top; y < top + height && y < bitmap.Height; y += stepY)
        {
            for (int x = left; x < left + width && x < bitmap.Width; x += stepX)
            {
                colors.Add(bitmap.GetPixel(x, y).ToArgb());
                if (colors.Count > 4)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static CaptureDiagnostics AnalyzeControlLayout(Control root)
    {
        List<string> issues = new();
        int overflow = 0;
        int textClip = 0;
        int internalText = 0;
        VisitControl(root, root.NameOrType(), issues, ref overflow, ref textClip, ref internalText);
        return new CaptureDiagnostics(overflow, textClip, internalText, issues.Take(20).ToArray());
    }

    private static void VisitControl(Control control, string path, List<string> issues, ref int overflow, ref int textClip, ref int internalText)
    {
        foreach (Control child in control.Controls)
        {
            string childPath = path + "/" + child.NameOrType();
            if (!child.Visible)
            {
                continue;
            }

            if (IsOutsideParent(child))
            {
                overflow++;
                AddIssue(issues, $"OVERFLOW|{childPath}|bounds={child.Bounds}|parent={child.Parent?.ClientSize}");
            }

            if (IsTextLikelyClipped(child))
            {
                textClip++;
                AddIssue(issues, $"TEXT|{childPath}|text=\"{Truncate(child.Text, 60)}\"|size={child.Size}");
            }

            if (ContainsInternalTypeText(child.Text))
            {
                internalText++;
                AddIssue(issues, $"INTERNAL_TEXT|{childPath}|text=\"{Truncate(child.Text, 80)}\"");
            }

            if (child is ElementHost elementHost)
            {
                VisitWpfVisual(elementHost.Child, childPath + "/WPF", issues, ref internalText);
            }

            VisitControl(child, childPath, issues, ref overflow, ref textClip, ref internalText);
        }
    }

    private static void VisitWpfVisual(System.Windows.DependencyObject? visual, string path, List<string> issues, ref int internalText)
    {
        if (visual == null)
        {
            return;
        }

        if (visual is System.Windows.Controls.TextBlock textBlock
            && ContainsInternalTypeText(textBlock.Text))
        {
            internalText++;
            AddIssue(issues, $"INTERNAL_TEXT|{path}/{visual.GetType().Name}|text=\"{Truncate(textBlock.Text, 80)}\"");
        }

        int childCount;
        try
        {
            childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(visual);
        }
        catch
        {
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            VisitWpfVisual(System.Windows.Media.VisualTreeHelper.GetChild(visual, i), path, issues, ref internalText);
        }
    }

    private static bool IsOutsideParent(Control control)
    {
        Control? parent = control.Parent;
        if (parent == null || parent.ClientSize.Width <= 0 || parent.ClientSize.Height <= 0)
        {
            return false;
        }

        if (parent is ScrollableControl { AutoScroll: true })
        {
            return false;
        }

        Rectangle bounds = control.Bounds;
        const int tolerance = 3;
        return bounds.Left < -tolerance
            || bounds.Top < -tolerance
            || bounds.Right > parent.ClientSize.Width + tolerance
            || bounds.Bottom > parent.ClientSize.Height + tolerance;
    }

    private static bool IsTextLikelyClipped(Control control)
    {
        if (control.AutoSize || string.IsNullOrWhiteSpace(control.Text) || control.Width <= 12)
        {
            return false;
        }

        if (control is not ButtonBase && control is not Label && control is not ComboBox)
        {
            return false;
        }

        if (control is CheckBox or RadioButton)
        {
            return false;
        }

        int availableWidth = control is Label
            ? Math.Max(0, control.ClientSize.Width)
            : Math.Max(0, control.ClientSize.Width - 14);
        if (control is ComboBox)
        {
            availableWidth = Math.Max(0, control.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 14);
        }

        if (control is Label)
        {
            Size wrapped = TextRenderer.MeasureText(
                control.Text,
                control.Font,
                new Size(Math.Max(1, availableWidth), int.MaxValue),
                TextFormatFlags.WordBreak);
            return wrapped.Height > control.ClientSize.Height + 8;
        }

        Size measured = TextRenderer.MeasureText(control.Text, control.Font, Size.Empty, TextFormatFlags.SingleLine);
        int tolerance = control is Label ? 18 : 8;
        return measured.Width > availableWidth + tolerance;
    }

    private static void AddIssue(List<string> issues, string issue)
    {
        if (issues.Count < 20)
        {
            issues.Add(issue);
        }
    }

    private static bool ContainsInternalTypeText(string? text)
    {
        return !string.IsNullOrWhiteSpace(text)
            && (text.Contains("System.Windows.Controls.WpfPropertyGrid", StringComparison.Ordinal)
                || text.Contains("WpfPropertyGrid.CategoryItem", StringComparison.Ordinal)
                || text.Contains("CategoryItem", StringComparison.Ordinal));
    }

    private static void WriteIssueFile(string imagePath, CaptureDiagnostics diagnostics)
    {
        string issuePath = Path.ChangeExtension(imagePath, ".issues.txt");
        if (!diagnostics.HasWarnings)
        {
            if (File.Exists(issuePath))
            {
                File.Delete(issuePath);
            }

            return;
        }

        File.WriteAllLines(issuePath, diagnostics.IssueSamples);
    }

    private static string NameOrType(this Control control)
    {
        return string.IsNullOrWhiteSpace(control.Name)
            ? control.GetType().Name
            : control.Name;
    }

    private static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text ?? string.Empty;
        }

        return text.Substring(0, maxLength - 3) + "...";
    }

    private static string Tail(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text ?? string.Empty;
        }

        return "..." + text.Substring(text.Length - maxLength + 3);
    }

    private static Assembly ResolveOpenVisionLabAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(item => string.Equals(item.GetName().Name, "OpenVisionLab", StringComparison.OrdinalIgnoreCase))
            ?? Assembly.Load("OpenVisionLab");
    }

    private static Bitmap CreateSampleImage()
    {
        Bitmap image = new Bitmap(768, 576);
        using Graphics graphics = Graphics.FromImage(image);
        using Brush shapeBrush = new SolidBrush(Color.FromArgb(220, 225, 230));
        using Font titleFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
        graphics.Clear(Color.FromArgb(242, 245, 247));
        graphics.FillRectangle(shapeBrush, 70, 70, 160, 100);
        graphics.FillRectangle(shapeBrush, 320, 180, 210, 150);
        graphics.FillEllipse(shapeBrush, 560, 80, 90, 90);
        graphics.DrawString("UI Smoke", titleFont, Brushes.DarkSlateGray, 88, 105);
        return image;
    }

    private static Bitmap CreatePipelinePreviewRunImage()
    {
        Bitmap image = new Bitmap(768, 576);
        using Graphics graphics = Graphics.FromImage(image);
        using Brush markBrush = new SolidBrush(Color.FromArgb(24, 28, 32));
        using Pen guidePen = new Pen(Color.FromArgb(218, 224, 230), 1F);

        graphics.Clear(Color.FromArgb(245, 247, 249));
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

        const int columns = 7;
        const int rows = 7;
        const int startX = 116;
        const int startY = 74;
        const int gapX = 82;
        const int gapY = 66;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int left = startX + x * gapX;
                int top = startY + y * gapY;
                graphics.DrawRectangle(guidePen, left - 10, top - 10, 48, 48);
                if ((x + y) % 3 == 0)
                {
                    graphics.FillEllipse(markBrush, left, top, 24, 24);
                }
                else if ((x + y) % 3 == 1)
                {
                    graphics.FillRectangle(markBrush, left + 2, top + 2, 24, 22);
                }
                else
                {
                    Point[] triangle =
                    {
                        new(left + 14, top),
                        new(left + 28, top + 24),
                        new(left, top + 24)
                    };
                    graphics.FillPolygon(markBrush, triangle);
                }
            }
        }

        return image;
    }

    private static Bitmap CreateMainWorkspaceImage()
    {
        Bitmap image = new Bitmap(768, 576);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen linePen = new Pen(Color.FromArgb(80, 108, 135), 2F);
        using Pen accentPen = new Pen(Color.FromArgb(0, 168, 210), 4F);
        using Brush panelBrush = new SolidBrush(Color.FromArgb(220, 226, 232));
        using Brush darkBrush = new SolidBrush(Color.FromArgb(55, 70, 86));
        using Font titleFont = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point);
        using Font smallFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        graphics.Clear(Color.FromArgb(244, 246, 248));
        graphics.FillRectangle(panelBrush, 94, 78, 580, 420);
        graphics.DrawRectangle(linePen, 94, 78, 580, 420);
        graphics.DrawLine(accentPen, 94, 138, 674, 138);

        for (int x = 150; x <= 620; x += 78)
        {
            graphics.DrawLine(linePen, x, 160, x, 452);
        }

        for (int y = 190; y <= 440; y += 62)
        {
            graphics.DrawLine(linePen, 128, y, 640, y);
        }

        graphics.FillEllipse(darkBrush, 162, 220, 80, 80);
        graphics.FillRectangle(darkBrush, 318, 214, 150, 92);
        graphics.DrawEllipse(accentPen, 530, 214, 84, 84);
        graphics.DrawString("OpenVisionLab Smoke Image", titleFont, darkBrush, 124, 92);
        graphics.DrawString("Main layer sample for screenshot validation", smallFont, darkBrush, 126, 462);
        return image;
    }

    private static List<VisionToolOverlay> CreateSampleOverlays()
    {
        return new List<VisionToolOverlay>
        {
            CreateRectangleOverlay("01 Area=16000 Cx=150 Cy=120", 70, 70, 160, 100),
            CreateRectangleOverlay("02 Area=31500 Cx=425 Cy=255", 320, 180, 210, 150),
            CreateRectangleOverlay("03 Area=8100 Cx=605 Cy=125", 560, 80, 90, 90)
        };
    }

    private static VisionToolOverlay CreateRectangleOverlay(string label, float x, float y, float width, float height)
    {
        return new VisionToolOverlay
        {
            Kind = VisionToolOverlayKind.Rectangle,
            Label = label,
            Bounds = new RectangleF(x, y, width, height),
            Center = new PointF(x + width / 2F, y + height / 2F)
        };
    }

    private static void PumpUi(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            Application.DoEvents();
            Thread.Sleep(80);
        }
    }

    private sealed class MemoryDisplayManager : IDisplayManager, IDisposable
    {
        private readonly List<string> layerTitles = new();
        private readonly List<Bitmap> ownedImages = new();
        private readonly VisionRuntimeState state = new();

        public event EventHandler<EventArgs>? UpdateParameter;
        public event EventHandler<EventArgs>? UpdateResult;
        public event EventHandler<EventArgs>? UpdateCam;

        public MemoryDisplayManager()
        {
            ImageSpace = new ImageSpaceService();
        }

        public VisionRuntimeState State => state;
        public IImageSpace ImageSpace { get; }
        public int LayerCount => layerTitles.Count;

        public string SelectedItem
        {
            get => state.SelectedItem;
            set => state.SelectedItem = value;
        }

        public string FocusItem
        {
            get => state.FocusItem;
            set => state.FocusItem = value;
        }

        public int CameraIndex
        {
            get => state.CameraIndex;
            set => state.CameraIndex = value;
        }

        public string TackTime
        {
            get => state.TackTime;
            set => state.TackTime = value;
        }

        public void SetLayer(string title, Bitmap image)
        {
            if (string.IsNullOrWhiteSpace(title) || image == null)
            {
                return;
            }

            int index = FindIndex(title);
            Bitmap copy = new(image);
            if (index < 0)
            {
                index = layerTitles.Count;
                layerTitles.Add(title);
            }
            else if (ownedImages.Count > index)
            {
                ownedImages[index]?.Dispose();
                ownedImages[index] = copy;
            }

            while (ownedImages.Count <= index)
            {
                ownedImages.Add(null!);
            }

            ownedImages[index] = copy;
            ImageSpace.SetImage(index, title, copy);
            ImageSpace.SetActiveImage(copy);
            SelectedItem = title;
            FocusItem = title;
        }

        public IReadOnlyList<DisplayLayerInfo> GetLayerInfos()
        {
            return layerTitles
                .Select((title, index) => new DisplayLayerInfo(index, title))
                .ToArray();
        }

        public string GetLayerTitle(int index)
        {
            return index >= 0 && index < layerTitles.Count ? layerTitles[index] : string.Empty;
        }

        public void CreatePanel(ImageSpaceFrame? frame = null)
        {
            if (frame?.Image != null)
            {
                SetLayer($"NewPanel_{layerTitles.Count + 1}", frame.Image);
            }
        }

        public int FindIndex(string title)
        {
            return layerTitles.FindIndex(item => string.Equals(item, title, StringComparison.OrdinalIgnoreCase));
        }

        public int FindIndex()
        {
            return FindIndex(SelectedItem);
        }

        public void CreateLayerDisplay(ImageSpaceFrame frame, string title, bool useClose = true)
        {
            if (frame?.Image != null)
            {
                SetLayer(title, frame.Image);
            }
        }

        public void RefreshLayer(int index)
        {
        }

        public void ActivateLayer(string title)
        {
            SelectedItem = title;
            FocusItem = title;
        }

        public void ActivateLayer(int index)
        {
            string title = GetLayerTitle(index);
            if (!string.IsNullOrWhiteSpace(title))
            {
                ActivateLayer(title);
            }
        }

        public void ZoomLayerToFit(string title)
        {
        }

        public void ZoomLayerToFit(int index)
        {
        }

        public void SetCameraIndex(int cameraIndex)
        {
            CameraIndex = cameraIndex;
            UpdateCam?.Invoke(this, EventArgs.Empty);
        }

        public void SetTackTime(string tackTime)
        {
            TackTime = tackTime;
            UpdateResult?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyParameterChanged()
        {
            UpdateParameter?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            foreach (Bitmap image in ownedImages)
            {
                image?.Dispose();
            }

            ownedImages.Clear();
            layerTitles.Clear();
        }
    }

    private readonly record struct ScreenshotAnalysis(
        int Width,
        int Height,
        int SampledColorCount,
        double AverageBrightness,
        double FlatTilePercent)
    {
        public static ScreenshotAnalysis Missing => new(0, 0, 0, 0, 100);

        public bool IsUsable =>
            Width >= 320
            && Height >= 240
            && SampledColorCount >= 8
            && AverageBrightness > 4
            && AverageBrightness < 251;

        public bool HasHealthyColorSpread => SampledColorCount >= 24;

        public bool HasLargeFlatRegion => FlatTilePercent >= 88;
    }

    private readonly record struct CaptureDiagnostics(
        int OverflowIssueCount,
        int TextClipIssueCount,
        int InternalTextIssueCount,
        IReadOnlyList<string> IssueSamples)
    {
        public bool HasWarnings => OverflowIssueCount > 0 || TextClipIssueCount > 0 || InternalTextIssueCount > 0;
    }
}
