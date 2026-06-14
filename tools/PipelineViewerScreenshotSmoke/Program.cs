using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using OpenVisionLab;
using OpenVisionLab._1._Core;
using OpenVisionLab.ImageSpace.Core;
using OpenVisionLab.MessageDialogs;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

internal static class Program
{
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

    private static List<(string Name, Func<string, CaptureDiagnostics> Capture)> CreateTargets()
    {
        return new List<(string Name, Func<string, CaptureDiagnostics> Capture)>
        {
            ("pipeline_viewer", CapturePipelineViewer),
            ("ai_recipe_form", CaptureAiRecipeForm),
            ("pipeline_form", CapturePipelineForm),
            ("pipeline_form_branch", CapturePipelineFormBranch),
            ("pipeline_form_branch_check", CapturePipelineFormBranchCheck),
            ("pipeline_form_run_preview", CapturePipelineFormRunPreview),
            ("pipeline_designable_forms", CapturePipelineDesignableForms),
            ("pipeline_samples_form", CapturePipelineSamplesForm),
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
            .First(item => item.GetParameters().Length == 7);

        using Form viewer = (Form)constructor.Invoke(new object?[]
        {
            "Pipeline Preview Smoke",
            source,
            overlays,
            null,
            labelMode,
            300,
            1
        });

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
        return CaptureForm(form, outputPath, new Size(1280, 760), 16, useScreenCapture: true);
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
        return CaptureForm(form, outputPath, new Size(1280, 760), 16, shownForm => SelectPipelineStep(shownForm, formType, 2), useScreenCapture: true);
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
            new[] { "Threshold", "Morphology", "Contour", "LineGauge" },
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
        return CaptureForm(form, outputPath, new Size(760, 520), 14);
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
            Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point),
            Text = string.Join(Environment.NewLine, lines)
        };
        form.Controls.Add(textBox);
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
            new[] { "Threshold", "Morphology", "Contour", "LineGauge" },
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

    private static void RunAiRecipePreview(Form form)
    {
        Type formType = form.Tag as Type ?? form.GetType();
        MethodInfo? sampleMethod = formType.GetMethod("OnSampleClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo? runPreviewMethod = formType.GetMethod("OnRunPreviewClicked", BindingFlags.Instance | BindingFlags.NonPublic);
        sampleMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        PumpUi(4);
        runPreviewMethod?.Invoke(form, new object?[] { null, EventArgs.Empty });
        WaitForAiRecipePreview(form, formType, 80);
    }

    private static void WaitForAiRecipePreview(Form form, Type formType, int maxIterations)
    {
        FieldInfo? logField = formType.GetField("tbLog", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo? runButtonField = formType.GetField("btnRunPreview", BindingFlags.Instance | BindingFlags.NonPublic);
        for (int i = 0; i < maxIterations; i++)
        {
            Application.DoEvents();
            string logText = logField?.GetValue(form) is TextBoxBase logBox ? logBox.Text : string.Empty;
            bool runFinished = logText.Contains("RUN OK", StringComparison.OrdinalIgnoreCase)
                || logText.Contains("RUN NG", StringComparison.OrdinalIgnoreCase);
            bool buttonReady = runButtonField?.GetValue(form) is not Control runButton || runButton.Enabled;
            if (runFinished && buttonReady)
            {
                PumpUi(3);
                return;
            }

            Thread.Sleep(80);
        }
    }

    private static CaptureDiagnostics CaptureThresholdForm(string outputPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        Assembly appAssembly = ResolveOpenVisionLabAssembly();
        Type formType = appAssembly.GetType("OpenVisionLab.FormThreshold", throwOnError: true)
            ?? throw new InvalidOperationException("FormThreshold type was not found.");
        using Form form = (Form)(Activator.CreateInstance(formType)
            ?? throw new InvalidOperationException("FormThreshold could not be created."));
        return CaptureForm(form, outputPath, new Size(720, 720), 16);
    }

    private static CaptureDiagnostics CaptureMatchingToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Matching", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureFeatureMatchingToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_FeatureMatching", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureContourToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Contour", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureBlobToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Blob", new Size(920, 660));
    }

    private static CaptureDiagnostics CaptureLineToolForm(string outputPath)
    {
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_Line", new Size(920, 660));
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
        return CaptureVisionToolForm(outputPath, "OpenVisionLab.FormVision_EdgeDetection", new Size(920, 660));
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

        return CaptureForm(form, outputPath, size, 14, afterShow, useScreenCapture: true);
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
        return CaptureForm(form, outputPath, new Size(620, 270), 12, expandDetails ? ExpandMessageDetails : null);
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
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(40, 40);
        form.Size = size;
        form.ShowInTaskbar = false;
        form.TopMost = useScreenCapture;
        try
        {
            form.Show();
            form.BringToFront();
            form.Activate();
            PumpUi(pumpIterations);
            afterShow?.Invoke(form);
            if (afterShow != null)
            {
                form.BringToFront();
                form.Activate();
                PumpUi(Math.Max(8, pumpIterations / 2));
            }

            CaptureDiagnostics diagnostics = AnalyzeControlLayout(form);
            using Bitmap capture = new Bitmap(Math.Max(1, form.Width), Math.Max(1, form.Height));
            if (useScreenCapture)
            {
                using Graphics graphics = Graphics.FromImage(capture);
                graphics.CopyFromScreen(form.PointToScreen(Point.Empty), Point.Empty, form.Size);
                if (LooksLikeExternalScreenCapture(capture))
                {
                    form.DrawToBitmap(capture, new Rectangle(Point.Empty, form.Size));
                }
            }
            else
            {
                form.DrawToBitmap(capture, new Rectangle(Point.Empty, form.Size));
            }

            capture.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            return diagnostics;
        }
        finally
        {
            if (!form.IsDisposed)
            {
                form.Close();
            }
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
        displayManager.SelectedItem = "Main";
        displayManager.ImageSpace.SetActiveImage(sample);
        displayManager.ActivateLayer("Main");
        displayManager.ZoomLayerToFit("Main");
        displayManager.NotifyParameterChanged();
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
