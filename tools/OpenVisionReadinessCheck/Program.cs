using System.Globalization;
using System.Text;

namespace OpenVisionReadinessCheck;

internal static class Program
{
    private static readonly List<string> Failures = new List<string>();
    private static readonly List<string> Passed = new List<string>();

    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string repoRoot = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
            ? Path.GetFullPath(args[0])
            : FindRepoRoot(AppContext.BaseDirectory);

        if (string.IsNullOrWhiteSpace(repoRoot) || !Directory.Exists(repoRoot))
        {
            Console.Error.WriteLine("OpenVisionReadinessCheck NG | Repository root was not found.");
            return 1;
        }

        CheckWpfShellMigration(repoRoot);
        CheckToolSamplesAndDiagnostics(repoRoot);
        CheckPipelineInputOutputUx(repoRoot);
        CheckToolViewControllerOwnership(repoRoot);
        CheckWpgEditorContracts(repoRoot);
        CheckLocalizationExpansion(repoRoot);
        CheckTutorialAndLearningDocs(repoRoot);
        CheckReleaseAndExternalPolicy(repoRoot);
        CheckCompletedItemHygiene(repoRoot);

        foreach (string item in Passed)
        {
            Console.WriteLine($"OK | {item}");
        }

        if (Failures.Count == 0)
        {
            Console.WriteLine("OpenVisionLab readiness contract passed.");
            return 0;
        }

        Console.Error.WriteLine("OpenVisionLab readiness contract failed.");
        foreach (string failure in Failures)
        {
            Console.Error.WriteLine($"NG | {failure}");
        }

        return 1;
    }

    private static void CheckWpfShellMigration(string repoRoot)
    {
        string program = Read(repoRoot, "Program.cs");
        RequireContains(program, "OpenVisionShellHostWindow", "Application starts through the WPF shell.");

        string shellHost = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionShellHostView.xaml.cs");
        RequireContains(shellHost, "SetDirectRunPending", "Pending state is surfaced by the WPF shell direct-run pending state.");

        string documentController = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionShellHostDocumentController.cs");
        RequireContains(documentController, "ActivatePendingTool", "Pending tool state is represented by a WPF ViewModel.");
        RequireContains(documentController, "OpenVisionPendingToolViewModel", "Pending tool state is represented by a WPF ViewModel.");

        string statePresenter = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs");
        RequireContains(statePresenter, "ActivePendingToolTitle", "Pending tool state is projected through shell host state presenter.");

        string commandCatalog = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionShellCommandCatalog.cs");
        RequireContains(commandCatalog, "PendingAlgorithmTool", "Algorithm tools without completed views are marked as pending work.");

        string uiPrecheck = Read(repoRoot, @"tools\RunUiPrecheck.ps1");
        RequireContains(uiPrecheck, "wpf_shell_host_workspace", "UI precheck covers the WPF image workspace.");
        RequireContains(uiPrecheck, "wpf_shell_host_workspace_output", "UI precheck covers output-layer switching in the WPF image workspace.");
        RequireContains(uiPrecheck, "wpf_shell_host_pending_tool", "UI precheck covers the WPF pending tool surface.");

        string smoke = Read(repoRoot, @"tools\PipelineViewerScreenshotSmoke\Program.cs");
        RequireContains(smoke, "CaptureShellHostWorkspaceOutput", "Screenshot smoke verifies WPF workspace output-layer switching.");
        RequireContains(smoke, "wpf_shell_host_native_tool", "Screenshot smoke covers native WPF tool windows.");
        RequireContains(smoke, "wpf_shell_host_pending_tool", "Screenshot smoke covers pending WPF tool windows.");

        string catalog = Read(repoRoot, @"Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv");
        RequireContains(catalog, "Shell.PendingTool.Message", "Localization catalog includes pending tool copy.");
        Pass("WPF shell migration contract");
    }

    private static void CheckToolSamplesAndDiagnostics(string repoRoot)
    {
        string diagnostic = Read(repoRoot, @"1. Core\VisionPipelineStepDiagnosticService.cs");
        RequireContains(diagnostic, "ResultCount", "Tool diagnostics include result count.");
        RequireContains(diagnostic, "Score", "Tool diagnostics include matching/feature score context.");
        RequireContains(diagnostic, "EdgeCount", "Tool diagnostics include edge/line count context.");
        RequireContains(diagnostic, "LineLength", "Tool diagnostics include line length context.");

        string catalog = Read(repoRoot, @"docs\samples\OpenVisionLab.SampleCatalog.csv");
        RequireContains(catalog, "Contour_TextSymbols", "Sample catalog includes contour text/symbol benchmark.");
        RequireContains(catalog, "Rice_Particle_Blob", "Sample catalog includes blob benchmark.");
        RequireContains(catalog, "Pins_LineGauge", "Sample catalog includes line gauge benchmark.");
        RequireContains(catalog, "Contour_TemplateMatching", "Sample catalog includes matching benchmark.");
        RequireContains(catalog, "Good", "Sample catalog includes Good sample pairs.");
        RequireContains(catalog, "Bad", "Sample catalog includes Bad sample pairs.");
        RequireContains(catalog, "ExpectedFailure", "Sample catalog includes expected-failure negative samples.");
        RequireContains(catalog, "Blob_ParticleDensity", "Sample catalog includes Blob Good/Bad pair.");
        RequireContains(catalog, "LineGauge_Angle", "Sample catalog includes LineGauge Good/Bad pair.");
        RequireContains(catalog, "Mean_BrightnessDrift", "Sample catalog includes Mean Good/Bad pair.");
        RequireContains(catalog, "Template_TargetPresence", "Sample catalog includes Matching target/no-target pair.");
        RequireContains(catalog, "Feature_ScoreDiscrimination", "Sample catalog includes FeatureMatching score-discrimination pair.");
        RequireContains(catalog, "Fiducial_Solder", "Sample catalog includes fiducial solder Good/Bad pair.");
        ValidateGoodBadPairCatalog(catalog);
        ValidatePublicSampleCatalog(repoRoot);
        ValidateProductSampleCatalog(repoRoot);

        string sampleCatalogUi = Read(repoRoot, @"0. UI\6) Vision Test\VisionPipelineSampleCatalog.cs");
        RequireContains(sampleCatalogUi, "VisionPipelineSampleCatalogSourceKind.Public", "Sample catalog loader exposes public catalog source.");
        RequireContains(sampleCatalogUi, "VisionPipelineSampleCatalogSourceKind.Product", "Sample catalog loader exposes product-domain catalog source.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.PublicSampleCatalog.csv", "Sample catalog loader reads the public catalog.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.ProductSampleCatalog.csv", "Sample catalog loader reads the product-domain catalog.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.SampleCatalog.csv", "Sample catalog loader keeps the local legacy catalog.");
        RequireContains(sampleCatalogUi, "FixGuideText", "Sample catalog UX exposes fix guidance.");
        RequireContains(sampleCatalogUi, "ExpectedReasonText", "Sample catalog UX explains why expected metrics matter.");
        RequireContains(sampleCatalogUi, "Expected outcome: no result / controlled NG", "Sample catalog UX explains expected-failure samples.");

        string samplePickerViewModel = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs");
        RequireContains(samplePickerViewModel, "CatalogSourceOptions", "Workspace sample picker exposes catalog source options.");
        RequireContains(samplePickerViewModel, "ActiveRouteSummaryText", "Workspace sample picker exposes active route summary text.");
        RequireContains(samplePickerViewModel, "SelectedCatalogSourceOption", "Workspace sample picker can switch between catalog sources.");
        RequireContains(samplePickerViewModel, "RebuildLearnPathOptions", "Workspace sample picker rebuilds learn paths per selected catalog source.");
        RequireContains(samplePickerViewModel, "OpenLearnDocumentCommand", "Workspace sample picker exposes a non-executing Learn document command.");
        RequireContains(samplePickerViewModel, "CanOpenLearnAndSample", "Workspace sample picker exposes an explicit guide-plus-sample action state.");

        string learnDocumentService = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionWorkspaceLearnDocumentService.cs");
        RequireContains(learnDocumentService, "docs", "Workspace sample picker resolves Learn documents from the repository docs folder.");
        RequireContains(learnDocumentService, "LEARN_PRODUCT_SAMPLES.md", "Workspace Learn document resolver links product-domain samples.");
        RequireContains(learnDocumentService, "LEARN_MATCHING.md", "Workspace Learn document resolver links Matching samples.");
        RequireContains(learnDocumentService, "LEARN_EDGE_BASED_MATCHING.md", "Workspace Learn document resolver links EdgeBasedMatching samples.");
        RequireContains(learnDocumentService, "LEARN_GEOMETRY_TRANSFORM.md", "Workspace Learn document resolver links Geometry samples.");
        string sampleLearnPathOption = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionWorkspaceSampleLearnPathOption.cs");
        RequireContains(sampleLearnPathOption, "\"geometry\"", "Workspace sample picker exposes the Geometry Learn path.");
        RequireContains(sampleLearnPathOption, "RotateScale", "Workspace sample picker classifies RotateScale samples as Geometry.");

        string samplePickerView = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml");
        RequireContains(samplePickerView, "WorkspaceSamplePickerCatalogSourceList", "Workspace sample picker renders a catalog source selector.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerRouteSummary", "Workspace sample picker renders the active catalog/focus/Learn route summary.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerCatalogSourceSummary", "Workspace sample picker renders selected catalog source guidance.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerOpenLearnDocumentButton", "Workspace sample picker renders a Learn document open button.");

        string samplePickerWindow = Read(repoRoot, @"0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerWindow.xaml");
        RequireContains(samplePickerWindow, "WorkspaceSamplePickerOpenGuideAndSampleButton", "Workspace sample picker renders an explicit guide-plus-sample button.");

        string sampleSmoke = Read(repoRoot, @"tools\PipelineViewerScreenshotSmoke\Program.cs");
        RequireContains(sampleSmoke, "wpf_shell_host_workspace_product_sample_review", "Screenshot smoke covers the Product sample catalog WPF review flow.");
        RequireContains(sampleSmoke, "wpf_shell_host_workspace_product_sample_review_ng", "Screenshot smoke covers the Product sample catalog WPF controlled-NG review flow.");
        RequireContains(sampleSmoke, "Product_Display_Particle_Good", "Product sample smoke opens a concrete Product catalog Good sample.");
        RequireContains(sampleSmoke, "Product_Display_Particle_Many_Bad", "Product sample smoke opens a concrete Product catalog controlled-NG sample.");
        RequireContains(sampleSmoke, "Public_Blob_Particles_Good", "Generic sample smoke opens a public-safe Blob Good sample.");
        RequireContains(sampleSmoke, "Public_Blob_Particles_Sparse_Bad", "Generic sample smoke opens a public-safe Blob controlled-NG sample.");
        RequireContains(sampleSmoke, "Public_Mean_Brightness_Dark_Bad", "Generic sample smoke opens a public-safe Mean controlled-NG sample.");
        RequireNotContains(sampleSmoke, "Blob_RiceParticle_Good", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "Blob_Bacteria_SparseBad", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "Mean_Brightness_DimBad", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "Feature_TemplateReview_LowScoreSwitch", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "LineGauge_PinsTilted_Bad", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "BentPin_BadShaft", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireNotContains(sampleSmoke, "EasyObject_FilmBad_DarkSpot", "Generic sample smoke must not require the local legacy root Sample catalog.");
        RequireContains(sampleSmoke, "LEARN_PRODUCT_SAMPLES.md", "Product sample smoke verifies the Product Learn document route.");

        string localization = Read(repoRoot, @"Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv");
        RequireContains(localization, "PipelineSamples.FixGuide", "Sample selection details show recommended fix points.");
        RequireContains(localization, "PipelineSamples.ExpectedReason", "Sample selection details show expected metric reason.");
        RequireContains(localization, "PipelineSamples.ExpectedPipeline", "Sample selection header shows expected result and recommended pipeline.");
        RequireContains(localization, "PipelineSamples.CatalogSource", "Sample selection exposes localized catalog source text.");
        Pass("Tool sample validation and diagnostic contract");
    }

    private static void ValidatePublicSampleCatalog(string repoRoot)
    {
        string publicCatalog = Read(repoRoot, @"docs\samples\OpenVisionLab.PublicSampleCatalog.csv");
        RequireContains(publicCatalog, "Public_Matching_DiePad_Good", "Public sample catalog includes Matching synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Matching_DiePad_NoTarget_Bad", "Public sample catalog includes Matching no-target Bad benchmark.");
        RequireContains(publicCatalog, "Public_Blob_Particles_Good", "Public sample catalog includes Blob synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Blob_Particles_Sparse_Bad", "Public sample catalog includes Blob sparse Bad benchmark.");
        RequireContains(publicCatalog, "Public_Contour_Shapes_Good", "Public sample catalog includes Contour synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Contour_Shapes_Missing_Bad", "Public sample catalog includes Contour missing-shape Bad benchmark.");
        RequireContains(publicCatalog, "Public_Threshold_BandPads_Good", "Public sample catalog includes Threshold synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Threshold_BandPads_Missing_Bad", "Public sample catalog includes Threshold Bad benchmark.");
        RequireContains(publicCatalog, "Public_Filter_Denoise_Good", "Public sample catalog includes Filter synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Filter_Denoise_Missing_Bad", "Public sample catalog includes Filter Bad benchmark.");
        RequireContains(publicCatalog, "Public_EdgeDetection_Shapes_Good", "Public sample catalog includes EdgeDetection synthetic benchmark.");
        RequireContains(publicCatalog, "Public_EdgeDetection_Shapes_Missing_Bad", "Public sample catalog includes EdgeDetection Bad benchmark.");
        RequireContains(publicCatalog, "Public_Morphology_Cleanup_Good", "Public sample catalog includes Morphology synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Morphology_Cleanup_Missing_Bad", "Public sample catalog includes Morphology Bad benchmark.");
        RequireContains(publicCatalog, "Public_Mean_Brightness_Good", "Public sample catalog includes Mean brightness synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Mean_Brightness_Dark_Bad", "Public sample catalog includes Mean brightness Bad benchmark.");
        RequireContains(publicCatalog, "Public_Arithmetic_Invert_Good", "Public sample catalog includes Arithmetic inversion synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Arithmetic_Invert_Bright_Bad", "Public sample catalog includes Arithmetic inversion Bad benchmark.");
        RequireContains(publicCatalog, "Public_HSV_ColorPatch_Good", "Public sample catalog includes HSV color-mask synthetic benchmark.");
        RequireContains(publicCatalog, "Public_HSV_ColorPatch_Missing_Bad", "Public sample catalog includes HSV color-mask Bad benchmark.");
        RequireContains(publicCatalog, "Public_Feature_Card_Good", "Public sample catalog includes FeatureMatching synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Feature_Card_Wrong_Bad", "Public sample catalog includes FeatureMatching Bad benchmark.");
        RequireContains(publicCatalog, "Public_Edge_Fiducial_Good", "Public sample catalog includes EdgeBasedMatching synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Edge_Fiducial_Wrong_Bad", "Public sample catalog includes EdgeBasedMatching Bad benchmark.");
        RequireContains(publicCatalog, "Public_Line_Pins_Good", "Public sample catalog includes LineDistance synthetic benchmark.");
        RequireContains(publicCatalog, "Public_Line_Pins_WidePin_Bad", "Public sample catalog includes LineDistance Bad benchmark.");
        RequireContains(publicCatalog, "Public_Geometry_RotateScale_Good", "Public sample catalog includes RotateScale geometry benchmark.");
        RequireContains(publicCatalog, "Public_Geometry_RotateScale_Wide_Bad", "Public sample catalog includes RotateScale geometry Bad benchmark.");
        RequireContains(publicCatalog, "ResultImageWidth;ResultImageHeight", "Public RotateScale benchmark checks output width and height.");
        RequireContains(publicCatalog, "ExpectedFailure", "Public sample catalog includes controlled NG rows.");
        RequireContains(publicCatalog, "Public_Matching_DiePad,Bad", "Public sample catalog groups Matching Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Blob_Particles,Bad", "Public sample catalog groups Blob Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Contour_Shapes,Bad", "Public sample catalog groups Contour Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Threshold_BandPads,Bad", "Public sample catalog groups Threshold Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Filter_Denoise,Bad", "Public sample catalog groups Filter Good/Bad pair.");
        RequireContains(publicCatalog, "Public_EdgeDetection_Shapes,Bad", "Public sample catalog groups EdgeDetection Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Morphology_Cleanup,Bad", "Public sample catalog groups Morphology Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Mean_BrightnessDrift,Bad", "Public sample catalog groups Mean Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Arithmetic_Invert,Bad", "Public sample catalog groups Arithmetic Good/Bad pair.");
        RequireContains(publicCatalog, "Public_HSV_ColorPatch,Bad", "Public sample catalog groups HSV Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Feature_Card,Bad", "Public sample catalog groups FeatureMatching Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Edge_Fiducial,Bad", "Public sample catalog groups EdgeBasedMatching Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Line_Pins,Bad", "Public sample catalog groups Line Good/Bad pair.");
        RequireContains(publicCatalog, "Public_Geometry_RotateScale,Bad", "Public sample catalog groups RotateScale Good/Bad pair.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Matching_DiePad.pipeline.xml", "Public sample catalog points Matching to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Blob_Particles.pipeline.xml", "Public sample catalog points Blob to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Contour_Shapes.pipeline.xml", "Public sample catalog points Contour to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Threshold_BandPads.pipeline.xml", "Public sample catalog points Threshold to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Filter_Denoise.pipeline.xml", "Public sample catalog points Filter to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_EdgeDetection_Shapes.pipeline.xml", "Public sample catalog points EdgeDetection to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Morphology_Cleanup.pipeline.xml", "Public sample catalog points Morphology to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Mean_BrightnessDrift.pipeline.xml", "Public sample catalog points Mean to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Arithmetic_Invert.pipeline.xml", "Public sample catalog points Arithmetic to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_HSV_ColorPatch.pipeline.xml", "Public sample catalog points HSV to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Feature_Card.pipeline.xml", "Public sample catalog points FeatureMatching to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Edge_Fiducial.pipeline.xml", "Public sample catalog points EdgeBasedMatching to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Line_Pins_Distance.pipeline.xml", "Public sample catalog points LineDistance to a public pipeline.");
        RequireContains(publicCatalog, @"docs\samples\public\Public_Geometry_RotateScale.pipeline.xml", "Public sample catalog points RotateScale to a public pipeline.");
        RequireNotContains(publicCatalog, @"Sample\", "Public sample catalog must not use legacy SDK Sample paths.");
        RequireNotContains(publicCatalog, @"Sample/", "Public sample catalog must not use legacy SDK Sample paths.");
        RequireNotContains(publicCatalog, @"bin\Debug\EasyMatch", "Public sample catalog must not use local EasyMatch output paths.");
        RequireNotContains(publicCatalog, "Euresys", "Public sample catalog must not depend on Euresys sample assets.");
        RequireNotContains(publicCatalog, "MVTec", "Public sample catalog must not depend on MVTec non-commercial assets.");

        string publicManifest = Read(repoRoot, @"docs\samples\public\OpenVisionLab.PublicSampleManifest.csv");
        RequireContains(publicManifest, "Workspace_Inspection_Synthetic_OK.png", "Public sample manifest includes workspace synthetic image.");
        RequireContains(publicManifest, "Matching_DiePad_Synthetic_OK.png", "Public sample manifest includes matching synthetic image.");
        RequireContains(publicManifest, "Matching_DiePad_Synthetic_NoTarget_NG.png", "Public sample manifest includes matching no-target synthetic image.");
        RequireContains(publicManifest, "Matching_DiePad_Synthetic_Template.png", "Public sample manifest includes matching synthetic template.");
        RequireContains(publicManifest, "Blob_Particles_Synthetic_OK.png", "Public sample manifest includes blob synthetic image.");
        RequireContains(publicManifest, "Blob_Particles_Synthetic_Sparse_NG.png", "Public sample manifest includes blob sparse synthetic image.");
        RequireContains(publicManifest, "Contour_Shapes_Synthetic_OK.png", "Public sample manifest includes contour synthetic image.");
        RequireContains(publicManifest, "Contour_Shapes_Synthetic_Missing_NG.png", "Public sample manifest includes contour Bad synthetic image.");
        RequireContains(publicManifest, "Threshold_BandPads_Synthetic_OK.png", "Public sample manifest includes threshold synthetic image.");
        RequireContains(publicManifest, "Threshold_BandPads_Synthetic_Missing_NG.png", "Public sample manifest includes threshold Bad synthetic image.");
        RequireContains(publicManifest, "Filter_Denoise_Synthetic_OK.png", "Public sample manifest includes filter synthetic image.");
        RequireContains(publicManifest, "Filter_Denoise_Synthetic_Missing_NG.png", "Public sample manifest includes filter Bad synthetic image.");
        RequireContains(publicManifest, "EdgeDetection_Shapes_Synthetic_OK.png", "Public sample manifest includes edge detection synthetic image.");
        RequireContains(publicManifest, "EdgeDetection_Shapes_Synthetic_Missing_NG.png", "Public sample manifest includes edge detection Bad synthetic image.");
        RequireContains(publicManifest, "Morphology_Cleanup_Synthetic_OK.png", "Public sample manifest includes morphology synthetic image.");
        RequireContains(publicManifest, "Morphology_Cleanup_Synthetic_Missing_NG.png", "Public sample manifest includes morphology Bad synthetic image.");
        RequireContains(publicManifest, "Mean_Brightness_Synthetic_OK.png", "Public sample manifest includes mean synthetic image.");
        RequireContains(publicManifest, "Mean_Brightness_Synthetic_Dark_NG.png", "Public sample manifest includes mean Bad synthetic image.");
        RequireContains(publicManifest, "Arithmetic_Invert_Synthetic_OK.png", "Public sample manifest includes arithmetic synthetic image.");
        RequireContains(publicManifest, "Arithmetic_Invert_Synthetic_Bright_NG.png", "Public sample manifest includes arithmetic Bad synthetic image.");
        RequireContains(publicManifest, "HSV_ColorPatch_Synthetic_OK.png", "Public sample manifest includes HSV synthetic image.");
        RequireContains(publicManifest, "HSV_ColorPatch_Synthetic_Missing_NG.png", "Public sample manifest includes HSV Bad synthetic image.");
        RequireContains(publicManifest, "Feature_Card_Synthetic_OK.png", "Public sample manifest includes feature matching synthetic image.");
        RequireContains(publicManifest, "Feature_Card_Synthetic_Wrong_NG.png", "Public sample manifest includes feature matching Bad synthetic image.");
        RequireContains(publicManifest, "Feature_Card_Synthetic_Template.png", "Public sample manifest includes feature matching synthetic template.");
        RequireContains(publicManifest, "Edge_Fiducial_Synthetic_OK.png", "Public sample manifest includes edge-based matching synthetic image.");
        RequireContains(publicManifest, "Edge_Fiducial_Synthetic_Wrong_NG.png", "Public sample manifest includes edge-based matching Bad synthetic image.");
        RequireContains(publicManifest, "Edge_Fiducial_Synthetic_Template.png", "Public sample manifest includes edge-based matching synthetic template.");
        RequireContains(publicManifest, "Line_Pins_Synthetic_OK.png", "Public sample manifest includes line synthetic image.");
        RequireContains(publicManifest, "Line_Pins_Synthetic_WidePin_NG.png", "Public sample manifest includes line Bad synthetic image.");
        RequireContains(publicManifest, "Geometry_RotateScale_Synthetic_OK.png", "Public sample manifest includes geometry transform synthetic image.");
        RequireContains(publicManifest, "Geometry_RotateScale_Synthetic_Wide_NG.png", "Public sample manifest includes geometry transform Bad synthetic image.");
        RequireNotContains(publicManifest, @"Sample\", "Public sample manifest must not use legacy SDK Sample paths.");
        RequireNotContains(publicManifest, @"Sample/", "Public sample manifest must not use legacy SDK Sample paths.");
        RequireNotContains(publicManifest, "Euresys", "Public sample manifest must not depend on Euresys sample assets.");
        RequireNotContains(publicManifest, "MVTec", "Public sample manifest must not depend on MVTec non-commercial assets.");

        foreach (string relativePath in new[]
        {
            @"docs\samples\public\Public_Matching_DiePad.pipeline.xml",
            @"docs\samples\public\Public_Blob_Particles.pipeline.xml",
            @"docs\samples\public\Public_Contour_Shapes.pipeline.xml",
            @"docs\samples\public\Public_Threshold_BandPads.pipeline.xml",
            @"docs\samples\public\Public_Filter_Denoise.pipeline.xml",
            @"docs\samples\public\Public_EdgeDetection_Shapes.pipeline.xml",
            @"docs\samples\public\Public_Morphology_Cleanup.pipeline.xml",
            @"docs\samples\public\Public_Mean_BrightnessDrift.pipeline.xml",
            @"docs\samples\public\Public_Arithmetic_Invert.pipeline.xml",
            @"docs\samples\public\Public_HSV_ColorPatch.pipeline.xml",
            @"docs\samples\public\Public_Feature_Card.pipeline.xml",
            @"docs\samples\public\Public_Edge_Fiducial.pipeline.xml",
            @"docs\samples\public\Public_Line_Pins_Distance.pipeline.xml",
            @"docs\samples\public\Public_Geometry_RotateScale.pipeline.xml",
        })
        {
            string pipeline = Read(repoRoot, relativePath);
            RequireContains(pipeline, "Main", $"Public pipeline {relativePath} reads the Main input layer.");
            RequireNotContains(pipeline, @"Sample\", $"Public pipeline {relativePath} must not use legacy SDK Sample paths.");
            RequireNotContains(pipeline, @"Sample/", $"Public pipeline {relativePath} must not use legacy SDK Sample paths.");
            RequireNotContains(pipeline, @"bin\Debug\EasyMatch", $"Public pipeline {relativePath} must not use local EasyMatch output paths.");
            RequireNotContains(pipeline, "Euresys", $"Public pipeline {relativePath} must not depend on Euresys sample assets.");
            RequireNotContains(pipeline, "MVTec", $"Public pipeline {relativePath} must not depend on MVTec non-commercial assets.");
        }

        Pass("Public sample asset contract");
    }

    private static void CheckPipelineInputOutputUx(string repoRoot)
    {
        string flowView = Read(repoRoot, @"Library\OpenVisionLab.Pipeline.Controls\PipelineFlowView.cs");
        RequireContains(flowView, "PipelineFlowPreviewMode.Input", "Pipeline flow can select input preview mode.");
        RequireContains(flowView, "PipelineFlowPreviewMode.Output", "Pipeline flow can select output preview mode.");
        RequireContains(flowView, "PipelineFlowPreviewMode.Overlay", "Pipeline flow can select overlay preview mode.");
        RequireContains(flowView, "ResolveInputPillLabel", "Pipeline flow resolves source/previous/branch input labels.");
        RequireContains(flowView, "PipelineFlow.ViewInputImage", "Input layer pill tells the user it opens input image.");
        RequireContains(flowView, "PipelineFlow.ViewOutputImage", "Output layer pill tells the user it opens output image.");
        RequireContains(flowView, "PipelineFlow.BranchInputTooltip", "Pipeline flow explains branch input deviations.");
        Pass("Pipeline input/output UX contract");
    }

    private static void CheckToolViewControllerOwnership(string repoRoot)
    {
        string toolViewDirectory = Path.Combine(repoRoot, @"0. UI\6) Vision Test\Wpf");
        if (!Directory.Exists(toolViewDirectory))
        {
            Failures.Add($"Tool View directory was not found: {toolViewDirectory}");
            return;
        }

        string[] toolViewFiles = Directory
            .EnumerateFiles(toolViewDirectory, "*ToolWpfView.xaml.cs", SearchOption.TopDirectoryOnly)
            .ToArray();
        if (toolViewFiles.Length == 0)
        {
            Failures.Add("Tool View controller ownership check did not find any *ToolWpfView.xaml.cs files.");
            return;
        }

        string[] forbiddenTokens =
        {
            "new VisionToolSingleInputToolEventHub(",
            "new VisionToolDoubleInputToolEventHub(",
            "VisionToolSingleInputCustomToolRuntime.Attach(",
            "VisionToolSingleInputPropertyToolRuntime<",
            "VisionToolSingleInputMatchingToolRuntime<",
            "VisionToolSingleInputSpecialPropertyToolRuntime.Attach(",
            "VisionToolDoubleInputCustomToolRuntime.Attach(",
            "VisionToolLanguageChangeController.Attach("
        };

        foreach (string toolViewFile in toolViewFiles)
        {
            string text = File.ReadAllText(toolViewFile, Encoding.UTF8);
            foreach (string token in forbiddenTokens)
            {
                if (text.IndexOf(token, StringComparison.Ordinal) >= 0)
                {
                    Failures.Add($"{Path.GetFileName(toolViewFile)} should delegate shell runtime/event/language wiring to a VisionTool controller. Forbidden token: {token}");
                }
            }
        }

        string lineView = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs");
        RequireContains(lineView, "VisionToolSingleInputSpecialPropertyToolController.Attach", "Line Tool delegates special PropertyGrid shell wiring to the shared controller.");

        string arithmeticView = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs");
        RequireContains(arithmeticView, "VisionToolDoubleInputCustomToolViewBase", "Arithmetic Tool delegates double-input shell forwarding to the shared view base.");
        string doubleInputCustomToolViewBase = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolViewBase.cs");
        RequireContains(doubleInputCustomToolViewBase, "VisionToolDoubleInputCustomToolController.Attach", "Double-input custom Tool View base delegates shell wiring to the shared controller.");

        string singleInputSpecialController = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\VisionToolSingleInputSpecialPropertyToolController.cs");
        RequireContains(singleInputSpecialController, "VisionToolSingleInputSpecialPropertyToolRuntime.Attach", "Special PropertyGrid controller owns the special single-input runtime wiring.");

        string doubleInputController = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolController.cs");
        RequireContains(doubleInputController, "VisionToolDoubleInputCustomToolRuntime.Attach", "Double-input controller owns the custom double-input runtime wiring.");

        Pass("Tool View controller ownership contract");
    }

    private static void ValidateGoodBadPairCatalog(string catalog)
    {
        string[] lines = catalog.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
        if (lines.Length < 2)
        {
            Failures.Add("Sample catalog should contain data rows.");
            return;
        }

        List<string> headers = SplitCsvLine(lines[0]);
        int pairGroupIndex = headers.FindIndex(header => string.Equals(header, "PairGroup", StringComparison.OrdinalIgnoreCase));
        int pairRoleIndex = headers.FindIndex(header => string.Equals(header, "PairRole", StringComparison.OrdinalIgnoreCase));
        int pipelineIndex = headers.FindIndex(header => string.Equals(header, "BaselinePipeline", StringComparison.OrdinalIgnoreCase));
        int validationModeIndex = headers.FindIndex(header => string.Equals(header, "ValidationMode", StringComparison.OrdinalIgnoreCase));
        int expectedMetricIndex = headers.FindIndex(header => string.Equals(header, "ExpectedMetricName", StringComparison.OrdinalIgnoreCase));
        int expectedMinIndex = headers.FindIndex(header => string.Equals(header, "ExpectedMetricMinimum", StringComparison.OrdinalIgnoreCase));
        int expectedMaxIndex = headers.FindIndex(header => string.Equals(header, "ExpectedMetricMaximum", StringComparison.OrdinalIgnoreCase));
        if (pairGroupIndex < 0 || pairRoleIndex < 0 || pipelineIndex < 0 || validationModeIndex < 0 || expectedMetricIndex < 0 || expectedMinIndex < 0 || expectedMaxIndex < 0)
        {
            Failures.Add("Sample catalog should expose PairGroup, PairRole, BaselinePipeline, ValidationMode, and expected metric columns.");
            return;
        }

        int pairRows = 0;
        bool hasExpectedFailurePair = false;
        Dictionary<string, HashSet<string>> rolesByGroup = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> pipelinesByGroup = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i < lines.Length; i++)
        {
            List<string> cells = SplitCsvLine(lines[i]);
            if (cells.Count <= Math.Max(Math.Max(pairGroupIndex, pairRoleIndex), Math.Max(expectedMinIndex, expectedMaxIndex)))
            {
                continue;
            }

            string pairGroup = cells[pairGroupIndex].Trim();
            if (string.IsNullOrWhiteSpace(pairGroup))
            {
                continue;
            }

            pairRows++;
            string pairRole = cells[pairRoleIndex].Trim();
            string pipeline = cells[pipelineIndex].Trim();
            string validationMode = cells[validationModeIndex].Trim();
            string metric = cells[expectedMetricIndex].Trim();
            string minimum = cells[expectedMinIndex].Trim();
            string maximum = cells[expectedMaxIndex].Trim();
            if (string.IsNullOrWhiteSpace(metric) || string.IsNullOrWhiteSpace(minimum) || string.IsNullOrWhiteSpace(maximum))
            {
                Failures.Add($"Sample catalog pair row is missing expected metric bounds. Group={pairGroup}, Role={pairRole}");
            }

            if (!string.Equals(pairRole, "Good", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(pairRole, "Bad", StringComparison.OrdinalIgnoreCase))
            {
                Failures.Add($"Sample catalog pair row should use PairRole Good or Bad. Group={pairGroup}, Role={pairRole}");
            }

            if (string.Equals(validationMode, "ExpectedFailure", StringComparison.OrdinalIgnoreCase))
            {
                hasExpectedFailurePair = true;
                if (!string.Equals(pairRole, "Bad", StringComparison.OrdinalIgnoreCase))
                {
                    Failures.Add($"ExpectedFailure pair row should be a Bad reference. Group={pairGroup}, Role={pairRole}");
                }
            }

            if (!rolesByGroup.TryGetValue(pairGroup, out HashSet<string> roles))
            {
                roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                rolesByGroup[pairGroup] = roles;
            }

            roles.Add(pairRole);
            if (!pipelinesByGroup.TryGetValue(pairGroup, out HashSet<string> pipelines))
            {
                pipelines = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                pipelinesByGroup[pairGroup] = pipelines;
            }

            pipelines.Add(pipeline);
        }

        if (pairRows < 27)
        {
            Failures.Add($"Sample catalog should have at least 27 Good/Bad pair rows. Rows={pairRows}");
        }

        if (rolesByGroup.Count < 12)
        {
            Failures.Add($"Sample catalog should have at least 12 named Good/Bad pair groups. Groups={rolesByGroup.Count}");
        }

        if (!hasExpectedFailurePair)
        {
            Failures.Add("Sample catalog should include at least one ExpectedFailure Good/Bad pair row for no-target validation.");
        }

        foreach (KeyValuePair<string, HashSet<string>> pair in rolesByGroup)
        {
            if (!pair.Value.Contains("Good") || !pair.Value.Contains("Bad"))
            {
                Failures.Add($"Sample catalog Good/Bad pair group must include both roles. Group={pair.Key}");
            }

            if (pipelinesByGroup.TryGetValue(pair.Key, out HashSet<string> pipelines) && pipelines.Count != 1)
            {
                Failures.Add($"Sample catalog Good/Bad pair group should use one shared baseline pipeline. Group={pair.Key}");
            }
        }
    }

    private static List<string> SplitCsvLine(string line)
    {
        List<string> cells = new List<string>();
        StringBuilder cell = new StringBuilder();
        bool quoted = false;
        for (int i = 0; i < (line?.Length ?? 0); i++)
        {
            char ch = line[i];
            if (ch == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cell.Append('"');
                    i++;
                    continue;
                }

                quoted = !quoted;
                continue;
            }

            if (ch == ',' && !quoted)
            {
                cells.Add(cell.ToString());
                cell.Clear();
                continue;
            }

            cell.Append(ch);
        }

        cells.Add(cell.ToString());
        return cells;
    }

    private static void CheckWpgEditorContracts(string repoRoot)
    {
        string uiContract = Read(repoRoot, @"tools\VisionUiContractCheck\Program.cs");
        RequireContains(uiContract, "AssertPipelineLayerSelectorContract", "WPG contract checks pipeline layer selector.");
        RequireContains(uiContract, "AssertPipelineMetricRangeContract", "WPG contract checks acceptance metric range editor.");
        RequireContains(uiContract, "AssertNativeToolPrewarmContract", "WPF contract checks native tool registry/prewarm parity.");
        RequireContains(uiContract, "AssertRangeEditorContract", "WPG contract checks common range editor.");
        RequireContains(uiContract, "PipelineLayerNameConverter", "WPG reflection contract expects pipeline layer converter.");
        RequireContains(uiContract, "WpgMetricRangeEditor", "WPG reflection contract expects metric range editor.");
        RequireContains(uiContract, "WpgRangeEditor", "WPG reflection contract expects common range editor.");
        Pass("WPG editor commonization contract");
    }

    private static void CheckLocalizationExpansion(string repoRoot)
    {
        string catalog = Read(repoRoot, @"Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv");
        RequireCatalogKey(catalog, "AiRecipe.Title");
        RequireCatalogKey(catalog, "PipelineFlow.ViewInputImage");
        RequireCatalogKey(catalog, "PipelineFlow.ViewOutputImage");
        RequireCatalogKey(catalog, "PropertyGrid.Property.InputLayer.DisplayName");
        RequireCatalogKey(catalog, "PropertyGrid.Category.Threshold");
        RequireCatalogKey(catalog, "Localization.Title");

        string check = Read(repoRoot, @"tools\LocalizationCatalogCheck\Program.cs");
        RequireContains(check, "DirectKeyPattern", "Localization check scans direct T(...) keys.");
        RequireContains(check, "DirectFormatKeyPattern", "Localization check scans formatted TF(...) keys.");
        Pass("Localization expansion contract");
    }

    private static void CheckTutorialAndLearningDocs(string repoRoot)
    {
        string tutorial = Read(repoRoot, @"docs\OPENVISIONLAB_TUTORIAL.html");
        RequireContains(tutorial, "Contour", "Tutorial includes contour workflow.");
        RequireContains(tutorial, "Blob", "Tutorial includes blob workflow.");
        RequireContains(tutorial, "Pattern Matching", "Tutorial includes pattern matching workflow.");
        RequireContains(tutorial, "EdgeDetection", "Tutorial includes edge detection workflow.");
        RequireContains(tutorial, "LineGauge", "Tutorial includes line gauge workflow.");
        RequireContains(tutorial, "Layer", "Tutorial includes layer workflow.");
        RequireContains(tutorial, "Recipe", "Tutorial includes recipe workflow.");
        RequireContains(tutorial, "Good/Bad", "Tutorial includes Good/Bad sample workflow.");
        RequireContains(tutorial, "run_log_collapsed_callouts.png", "Tutorial includes collapsed Run Log walkthrough.");
        RequireContains(tutorial, "run_log_open_callouts.png", "Tutorial includes open Run Log walkthrough.");
        RequireContains(tutorial, "assets/tutorial/", "Source tutorial uses asset references.");
        RequirePathExists(repoRoot, @"docs\assets\tutorial\annotated\run_log_collapsed_callouts.png", "Collapsed Run Log tutorial asset exists.");
        RequirePathExists(repoRoot, @"docs\assets\tutorial\annotated\run_log_open_callouts.png", "Open Run Log tutorial asset exists.");
        RequireContains(tutorial, "LEARN_CONTOUR.md", "Tutorial links Contour learn page.");
        RequireContains(tutorial, "LEARN_THRESHOLD.md", "Tutorial links Threshold learn page.");
        RequireContains(tutorial, "LEARN_MEAN.md", "Tutorial links Mean learn page.");
        RequireContains(tutorial, "LEARN_FEATURE_MATCHING.md", "Tutorial links FeatureMatching learn page.");
        RequireContains(tutorial, "LEARN_EDGE_BASED_MATCHING.md", "Tutorial links EdgeBasedMatching learn page.");

        foreach (string relativePath in new[]
        {
            @"docs\learn\README.md",
            @"docs\learn\LEARN_PRODUCT_SAMPLES.md",
            @"docs\learn\LEARN_MATCHING.md",
            @"docs\learn\LEARN_BLOB.md",
            @"docs\learn\LEARN_CONTOUR.md",
            @"docs\learn\LEARN_THRESHOLD.md",
            @"docs\learn\LEARN_MEAN.md",
            @"docs\learn\LEARN_FEATURE_MATCHING.md",
            @"docs\learn\LEARN_EDGE_BASED_MATCHING.md",
            @"docs\learn\LEARN_LINE.md",
        })
        {
            string learn = Read(repoRoot, relativePath);
            RequireContains(learn, "public sample", $"Learn document {relativePath} is grounded in public samples.");
            RequireContains(learn, "Good", $"Learn document {relativePath} explains Good sample behavior.");
            RequireContains(learn, "Bad", $"Learn document {relativePath} explains Bad sample behavior.");
        }

        string learnIndex = Read(repoRoot, @"docs\learn\README.md");
        RequireContains(learnIndex, "## Learn Window Topic Map", "Learn index documents the Learn window topic map.");
        foreach ((int Topic, string Document, string PathId) learnTopic in new[]
        {
            (0, "OPENVISIONLAB_LEARN_CURRICULUM.md", "all"),
            (1, "LEARN_MEAN.md", "mean"),
            (2, "LEARN_THRESHOLD.md", "preprocess"),
            (3, "LEARN_FILTER.md", "preprocess"),
            (4, "LEARN_MORPHOLOGY.md", "preprocess"),
            (5, "LEARN_BLOB.md", "blob"),
            (6, "LEARN_CONTOUR.md", "contour"),
            (7, "LEARN_EDGE_DETECTION.md", "preprocess"),
            (8, "LEARN_LINE.md", "line"),
            (9, "LEARN_MATCHING.md", "template-matching"),
            (10, "LEARN_FEATURE_MATCHING.md", "feature-matching"),
            (11, "LEARN_PIPELINE_LAYER_ROUTING.md", "all"),
            (12, "LEARN_EDGE_BASED_MATCHING.md", "edge-matching"),
            (13, "LEARN_METRICS_ACCEPTANCE.md", "all"),
            (14, "LEARN_ARITHMETIC.md", "preprocess"),
            (15, "LEARN_GEOMETRY_TRANSFORM.md", "geometry"),
            (16, "LEARN_COLOR_HSV.md", "mean"),
        })
        {
            RequireContains(learnIndex, $"| {learnTopic.Topic} |", $"Learn index documents topic {learnTopic.Topic}.");
            RequireContains(learnIndex, $"`{learnTopic.Document}`", $"Learn index documents topic {learnTopic.Topic} document {learnTopic.Document}.");
            RequireContains(learnIndex, $"`{learnTopic.PathId}`", $"Learn index documents topic {learnTopic.Topic} practice path {learnTopic.PathId}.");
        }

        foreach (string relativePath in new[]
        {
            @"docs\learn\OPENVISIONLAB_LEARN_CURRICULUM.md",
            @"docs\learn\LEARN_OPENCVSHARP_FOUNDATIONS.md",
            @"docs\learn\LEARN_PRODUCT_SAMPLES.md",
            @"docs\learn\LEARN_MATCHING.md",
            @"docs\learn\LEARN_BLOB.md",
            @"docs\learn\LEARN_CONTOUR.md",
            @"docs\learn\LEARN_THRESHOLD.md",
            @"docs\learn\LEARN_ARITHMETIC.md",
            @"docs\learn\LEARN_GEOMETRY_TRANSFORM.md",
            @"docs\learn\LEARN_FILTER.md",
            @"docs\learn\LEARN_MORPHOLOGY.md",
            @"docs\learn\LEARN_MEAN.md",
            @"docs\learn\LEARN_COLOR_HSV.md",
            @"docs\learn\LEARN_EDGE_DETECTION.md",
            @"docs\learn\LEARN_FEATURE_MATCHING.md",
            @"docs\learn\LEARN_EDGE_BASED_MATCHING.md",
            @"docs\learn\LEARN_LINE.md",
            @"docs\learn\LEARN_PIPELINE_LAYER_ROUTING.md",
            @"docs\learn\LEARN_METRICS_ACCEPTANCE.md",
        })
        {
            RequirePathExists(repoRoot, relativePath, $"Learn document exists: {relativePath}.");
            RequireContains(learnIndex, Path.GetFileName(relativePath), $"Learn index links {relativePath}.");
            string learn = Read(repoRoot, relativePath);
            RequireContainsAny(
                learn,
                $"Learn document {relativePath} keeps execution explicit.",
                "Preview/Run",
                "Preview or Run",
                "Preview and Run",
                "Run Preview",
                "Preview를");
            RequireContainsAny(
                learn,
                $"Learn document {relativePath} states guides/settings do not auto-run.",
                "must not run",
                "does not run",
                "not run Preview",
                "automatically",
                "자동");
        }

        foreach (string relativePath in new[]
        {
            @"docs\learn\OPENVISIONLAB_LEARN_CURRICULUM.md",
            @"docs\learn\LEARN_OPENCVSHARP_FOUNDATIONS.md",
            @"docs\learn\LEARN_PRODUCT_SAMPLES.md",
            @"docs\learn\LEARN_MATCHING.md",
            @"docs\learn\LEARN_BLOB.md",
            @"docs\learn\LEARN_CONTOUR.md",
            @"docs\learn\LEARN_THRESHOLD.md",
            @"docs\learn\LEARN_ARITHMETIC.md",
            @"docs\learn\LEARN_GEOMETRY_TRANSFORM.md",
            @"docs\learn\LEARN_FILTER.md",
            @"docs\learn\LEARN_MORPHOLOGY.md",
            @"docs\learn\LEARN_MEAN.md",
            @"docs\learn\LEARN_COLOR_HSV.md",
            @"docs\learn\LEARN_EDGE_DETECTION.md",
            @"docs\learn\LEARN_FEATURE_MATCHING.md",
            @"docs\learn\LEARN_EDGE_BASED_MATCHING.md",
            @"docs\learn\LEARN_LINE.md",
            @"docs\learn\LEARN_PIPELINE_LAYER_ROUTING.md",
            @"docs\learn\LEARN_METRICS_ACCEPTANCE.md",
        })
        {
            string learn = Read(repoRoot, relativePath);
            RequireNotContains(learn, @"Sample\", $"Learn document {relativePath} must not depend on local legacy root Sample paths.");
            RequireNotContains(learn, "Sample/", $"Learn document {relativePath} must not depend on local legacy root Sample paths.");
            RequireNotContains(learn, @"bin\Debug", $"Learn document {relativePath} must not depend on local build output sample paths.");
            RequireNotContains(learn, "Euresys", $"Learn document {relativePath} must not depend on vendor sample assets.");
            RequireNotContains(learn, "MVTec", $"Learn document {relativePath} must not depend on non-commercial dataset assets.");
        }

        string learnWindowXaml = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\OpenVisionLearnWindow.xaml");
        string learnWindow = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\OpenVisionLearnWindow.xaml.cs");
        string toolShell = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.xaml.cs");
        string foundationGuide = Read(repoRoot, @"docs\learn\LEARN_OPENCVSHARP_FOUNDATIONS.md");
        RequireContains(learnIndex, "LEARN_OPENCVSHARP_FOUNDATIONS.md", "Learn index links the OpenCvSharp foundations guide.");
        RequireContains(foundationGuide, "`Point`", "OpenCvSharp foundations guide explains Point.");
        RequireContains(foundationGuide, "`Size`", "OpenCvSharp foundations guide explains Size.");
        RequireContains(foundationGuide, "`Rect`", "OpenCvSharp foundations guide explains Rect.");
        RequireContains(foundationGuide, "`RotatedRect`", "OpenCvSharp foundations guide explains RotatedRect.");
        RequireContains(foundationGuide, "`Scalar`", "OpenCvSharp foundations guide explains Scalar.");
        RequireContains(foundationGuide, "`Mat`", "OpenCvSharp foundations guide explains Mat.");
        RequireContains(foundationGuide, "Mat[Row=Y, Column=X, Channel]", "OpenCvSharp foundations guide explains matrix-style image access.");
        RequireContains(foundationGuide, "`InputLayer` is the source layer", "OpenCvSharp foundations guide explains input layer routing.");
        RequireContains(foundationGuide, "`OutputLayer` is the result created by the step", "OpenCvSharp foundations guide explains output layer routing.");
        RequireContains(foundationGuide, "must not run Preview/Run", "OpenCvSharp foundations guide keeps execution explicit.");
        RequireContains(foundationGuide, "Use public samples only", "OpenCvSharp foundations guide keeps sample practice public-safe.");
        RequireContains(foundationGuide, "Good/Bad samples", "OpenCvSharp foundations guide connects concepts to Good/Bad validation.");
        RequireContains(learnWindowXaml, "OpenVisionLearnOpenFoundationDocsButton", "OpenVision Learn exposes a Foundation Docs button.");
        RequireContains(learnWindowXaml, "Foundation Docs", "OpenVision Learn labels the Foundation Docs button.");
        RequireContains(learnWindow, "OpenFoundationDocsButton_Click", "OpenVision Learn handles the Foundation Docs button.");
        RequireContains(learnWindow, "LEARN_OPENCVSHARP_FOUNDATIONS.md", "OpenVision Learn Foundation Docs button opens the foundations guide.");
        RequireContains(learnWindowXaml, "OpenVisionLearnFoundationTypeCards", "OpenVision Learn foundation topic exposes Point/Rect/Size/Mat cards.");
        RequireContains(learnWindowXaml, "OpenVisionLearnBeginnerPathPanel", "OpenVision Learn foundation topic exposes a beginner path panel.");
        RequireContains(learnWindowXaml, "Beginner path: Foundation -> Brightness/GV -> Threshold -> Filter/Morphology -> Blob/Contour/LineDistance", "OpenVision Learn foundation topic shows the beginner tool path.");
        RequireContains(learnWindowXaml, "Do not skip the metric or Good/Bad check.", "OpenVision Learn beginner path requires metric and Good/Bad checks.");
        RequireContains(learnWindowXaml, "Mat = rows x cols x channels", "OpenVision Learn foundation topic explains Mat as an image matrix.");
        string meanGuide = Read(repoRoot, @"docs\learn\LEARN_MEAN.md");
        string thresholdGuide = Read(repoRoot, @"docs\learn\LEARN_THRESHOLD.md");
        string filterGuide = Read(repoRoot, @"docs\learn\LEARN_FILTER.md");
        string edgeDetectionGuide = Read(repoRoot, @"docs\learn\LEARN_EDGE_DETECTION.md");
        string morphologyGuide = Read(repoRoot, @"docs\learn\LEARN_MORPHOLOGY.md");
        RequireContains(meanGuide, "## Beginner path handoff", "Mean Learn guide includes beginner handoff.");
        RequireContains(meanGuide, "Practice Samples path: `mean`", "Mean Learn guide names the mean practice path.");
        RequireContains(meanGuide, "Public_Mean_Brightness_Good", "Mean beginner handoff names the public Good sample.");
        RequireContains(meanGuide, "Public_Mean_Brightness_Dark_Bad", "Mean beginner handoff names the public Bad sample.");
        RequireContains(thresholdGuide, "## Beginner path handoff", "Threshold Learn guide includes beginner handoff.");
        RequireContains(thresholdGuide, "Practice Samples path: `preprocess`", "Threshold Learn guide names the preprocess practice path.");
        RequireContains(thresholdGuide, "Public_Threshold_BandPads_Good", "Threshold beginner handoff names the public Good sample.");
        RequireContains(thresholdGuide, "downstream `ResultCount`", "Threshold beginner handoff requires downstream metric evidence.");
        RequireContains(filterGuide, "## Beginner path handoff", "Filter Learn guide includes beginner handoff.");
        RequireContains(filterGuide, "Public_Filter_Denoise_Good", "Filter Learn guide names the public Good sample.");
        RequireContains(filterGuide, "Public_Filter_Denoise_Missing_Bad", "Filter Learn guide names the public Bad sample.");
        RequireContains(filterGuide, "Public sample pair", "Filter Learn guide marks the public sample pair.");
        RequireContains(filterGuide, "Filter alone is not the final OK/NG decision", "Filter Learn guide avoids treating preprocessing as final inspection.");
        RequireContains(edgeDetectionGuide, "## Beginner path handoff", "EdgeDetection Learn guide includes beginner handoff.");
        RequireContains(edgeDetectionGuide, "Public_EdgeDetection_Shapes_Good", "EdgeDetection Learn guide names the public Good sample.");
        RequireContains(edgeDetectionGuide, "Public_EdgeDetection_Shapes_Missing_Bad", "EdgeDetection Learn guide names the public Bad sample.");
        RequireContains(edgeDetectionGuide, "Public sample pair", "EdgeDetection Learn guide marks the public sample pair.");
        RequireContains(edgeDetectionGuide, "downstream Contour metric", "EdgeDetection Learn guide keeps final evidence downstream.");
        RequireContains(morphologyGuide, "## Beginner path handoff", "Morphology Learn guide includes beginner handoff.");
        RequireContains(morphologyGuide, "Public_Morphology_Cleanup_Good", "Morphology Learn guide names the public Good sample.");
        RequireContains(morphologyGuide, "Public_Morphology_Cleanup_Missing_Bad", "Morphology Learn guide names the public Bad sample.");
        RequireContains(morphologyGuide, "Public sample pair", "Morphology Learn guide marks the public sample pair.");
        RequireContains(morphologyGuide, "before/after output-layer comparison", "Morphology Learn guide requires output-layer comparison.");
        RequireContains(toolShell, "new OpenVisionLearnWindow(127, 255, false, LearnTopicIndex)", "PropertyGrid tool Learn buttons open the configured Learn topic.");
        foreach ((string Xaml, string Text, int TopicIndex, string Document) toolLearn in new[]
        {
            (@"0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml", "Learn Filter", 3, "LEARN_FILTER.md"),
            (@"0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml", "Learn Morph", 4, "LEARN_MORPHOLOGY.md"),
            (@"0. UI\6) Vision Test\Wpf\BlobToolWpfView.xaml", "Learn Blob", 5, "LEARN_BLOB.md"),
            (@"0. UI\6) Vision Test\Wpf\ContourToolWpfView.xaml", "Learn Contour", 6, "LEARN_CONTOUR.md"),
            (@"0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml", "Learn Line", 8, "LEARN_LINE.md"),
            (@"0. UI\6) Vision Test\Wpf\EdgeBasedMatchingToolWpfView.xaml", "Learn Edge Match", 12, "LEARN_EDGE_BASED_MATCHING.md"),
            (@"0. UI\6) Vision Test\Wpf\MatchingToolWpfView.xaml", "Learn Matching", 9, "LEARN_MATCHING.md"),
            (@"0. UI\6) Vision Test\Wpf\FeatureMatchingToolWpfView.xaml", "Learn Feature", 10, "LEARN_FEATURE_MATCHING.md"),
        })
        {
            string toolXaml = Read(repoRoot, toolLearn.Xaml);
            string topicIndex = toolLearn.TopicIndex.ToString(CultureInfo.InvariantCulture);
            RequireContains(toolXaml, "LearnButtonVisibility=\"Visible\"", $"Tool Learn button is visible: {toolLearn.Xaml}.");
            RequireContains(toolXaml, $"LearnButtonText=\"{toolLearn.Text}\"", $"Tool Learn button text is mapped: {toolLearn.Xaml}.");
            RequireContains(toolXaml, $"LearnTopicIndex=\"{topicIndex}\"", $"Tool Learn topic index is mapped: {toolLearn.Xaml}.");
            RequireContains(learnWindow, $"{topicIndex} => \"{toolLearn.Document}\"", $"Learn topic {topicIndex} resolves {toolLearn.Document}.");
            RequirePathExists(repoRoot, Path.Combine("docs", "learn", toolLearn.Document), $"Tool Learn document exists for {toolLearn.Xaml}.");
        }

        foreach ((string Document, string ToolName) matchingGuide in new[]
        {
            (@"docs\learn\LEARN_MATCHING.md", "Matching"),
            (@"docs\learn\LEARN_EDGE_BASED_MATCHING.md", "EdgeBasedMatching"),
            (@"docs\learn\LEARN_FEATURE_MATCHING.md", "FeatureMatching"),
        })
        {
            string learn = Read(repoRoot, matchingGuide.Document);
            RequireContains(learn, "## Matching Family Selection", $"{matchingGuide.ToolName} Learn document compares Matching-family tool selection.");
            RequireContains(learn, "Stable brightness/template appearance", $"{matchingGuide.ToolName} Learn document explains when to use Matching.");
            RequireContains(learn, "Shape survives lighting but edge geometry is stable", $"{matchingGuide.ToolName} Learn document explains when to use EdgeBasedMatching.");
            RequireContains(learn, "Target changes scale/rotation/view but local features remain", $"{matchingGuide.ToolName} Learn document explains when to use FeatureMatching.");
            RequireContains(learn, "ScoreMax, ResultCount", $"{matchingGuide.ToolName} Learn document anchors Matching-family metrics.");
            RequireContains(learn, "Use explicit Preview/Run only.", $"{matchingGuide.ToolName} Learn document keeps execution explicit.");
        }

        string metricsAcceptanceGuide = Read(repoRoot, @"docs\learn\LEARN_METRICS_ACCEPTANCE.md");
        string publicSampleCatalog = Read(repoRoot, @"docs\samples\OpenVisionLab.PublicSampleCatalog.csv");
        foreach (string sampleName in new[]
        {
            "Public_Matching_DiePad_Good",
            "Public_Matching_DiePad_NoTarget_Bad",
            "Public_Blob_Particles_Good",
            "Public_Blob_Particles_Sparse_Bad",
            "Public_Filter_Denoise_Good",
            "Public_Filter_Denoise_Missing_Bad",
            "Public_EdgeDetection_Shapes_Good",
            "Public_EdgeDetection_Shapes_Missing_Bad",
            "Public_Morphology_Cleanup_Good",
            "Public_Morphology_Cleanup_Missing_Bad",
            "Public_Mean_Brightness_Good",
            "Public_Mean_Brightness_Dark_Bad",
            "Public_HSV_ColorPatch_Good",
            "Public_HSV_ColorPatch_Missing_Bad",
            "Public_Line_Pins_Good",
            "Public_Line_Pins_WidePin_Bad",
            "Public_Edge_Fiducial_Good",
            "Public_Edge_Fiducial_Wrong_Bad",
            "Public_Geometry_RotateScale_Good",
        })
        {
            RequireContains(metricsAcceptanceGuide, sampleName, $"Metrics/Acceptance Learn document names public sample {sampleName}.");
            RequireContains(publicSampleCatalog, sampleName, $"Public sample catalog contains {sampleName}.");
        }

        RequireContains(metricsAcceptanceGuide, "DistanceMmAvg=0.20..0.25", "Metrics/Acceptance Learn document teaches the LineDistance Good gate.");
        RequireContains(metricsAcceptanceGuide, "add range/max gates for outliers", "Metrics/Acceptance Learn document teaches distance outlier gates.");
        RequireContains(metricsAcceptanceGuide, "ResultImageWidth=286", "Metrics/Acceptance Learn document teaches the RotateScale output width gate.");
        RequireContains(metricsAcceptanceGuide, "ResultImageHeight=210", "Metrics/Acceptance Learn document teaches the RotateScale output height gate.");
        RequireContains(metricsAcceptanceGuide, "Transform samples may be Good-only", "Metrics/Acceptance Learn document explains Good-only transform benchmarks.");
        RequireContains(metricsAcceptanceGuide, "Good/Bad comparison is useful only when the Bad row fails for the intended metric.", "Metrics/Acceptance Learn document distinguishes intended metric failure from setup failure.");
        RequireNotContains(metricsAcceptanceGuide, "Public_Template_Circle", "Metrics/Acceptance Learn document must not reference non-catalog sample names.");
        RequireContains(learnWindow, "13. Metrics / Acceptance", "OpenVision Learn topic list exposes Metrics/Acceptance.");
        RequireContains(learnWindow, "13 => \"LEARN_METRICS_ACCEPTANCE.md\"", "OpenVision Learn topic 13 resolves Metrics/Acceptance document.");
        RequireContains(learnWindow, "Metrics/Acceptance gates", "OpenVision Learn topic 13 exposes Metrics/Acceptance practice guidance.");
        RequireContains(learnWindow, "metricsAcceptanceTopicPanel", "OpenVision Learn has a visible Metrics/Acceptance topic panel.");

        string pipelineLayerRoutingGuide = Read(repoRoot, @"docs\learn\LEARN_PIPELINE_LAYER_ROUTING.md");
        RequireContains(pipelineLayerRoutingGuide, "## Route Safety Checklist", "Pipeline/Layer Learn document has a route safety checklist.");
        RequireContains(pipelineLayerRoutingGuide, "`InputLayer` is the source image", "Pipeline/Layer Learn document explains InputLayer.");
        RequireContains(pipelineLayerRoutingGuide, "`OutputLayer` is the produced result", "Pipeline/Layer Learn document explains OutputLayer.");
        RequireContains(pipelineLayerRoutingGuide, "must not select, rewrite, or silently replace `InputLayer`", "Pipeline/Layer Learn document protects output/input isolation.");
        RequireContains(pipelineLayerRoutingGuide, "Layer create/delete/load-image actions and visibility toggles must not run Preview/Run.", "Pipeline/Layer Learn document protects explicit execution.");
        RequireContains(learnWindowXaml, "OpenVisionLearnLayerRoutingSafetyPanel", "OpenVision Learn exposes the route safety checklist panel.");
        RequireContains(learnWindowXaml, "Routing safety checklist", "OpenVision Learn topic 11 shows route safety guidance.");

        string learnSmokeScript = Read(repoRoot, @"tools\RunLearnModeUiSmokes.ps1");
        foreach (string learnSmokeTarget in new[]
        {
            "wpf_openvision_learn_curriculum",
            "wpf_openvision_learn_brightness",
            "wpf_openvision_learn_threshold",
            "wpf_openvision_learn_threshold_animation",
            "wpf_openvision_learn_threshold_apply",
            "wpf_openvision_learn_filtering",
            "wpf_openvision_learn_morphology",
            "wpf_openvision_learn_blob",
            "wpf_openvision_learn_contour",
            "wpf_openvision_learn_edge_line",
            "wpf_openvision_learn_line_distance",
            "wpf_openvision_learn_matching",
            "wpf_openvision_learn_feature_matching",
            "wpf_openvision_learn_layer_recipe",
            "wpf_openvision_learn_edge_based_matching",
            "wpf_openvision_learn_metrics_acceptance",
            "wpf_openvision_learn_arithmetic",
            "wpf_openvision_learn_geometry",
            "wpf_openvision_learn_color_hsv",
        })
        {
            RequireContains(learnSmokeScript, learnSmokeTarget, $"Learn Mode UI smoke runner covers {learnSmokeTarget}.");
        }

        RequireContains(learnSmokeScript, "foreach ($target in $normalizedTargets)", "Learn Mode UI smoke runner executes targets sequentially.");
        RequireNotContains(learnSmokeScript, "Start-Job", "Learn Mode UI smoke runner must not parallelize WPF smoke targets.");

        string learnScreenshotSmoke = Read(repoRoot, @"tools\PipelineViewerScreenshotSmoke\Program.cs");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_edge_based_matching", "Learn screenshot smoke exposes the EdgeBasedMatching topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnEdgeBasedMatching", "Learn screenshot smoke verifies the EdgeBasedMatching topic.");
        RequireContains(learnWindow, "EdgeDetection: pixels", "OpenVision Learn Edge / Line topic distinguishes edge, line, and measurement tool roles.");
        RequireContains(learnScreenshotSmoke, "\"LineGauge\", \"LineDistance\"", "Learn screenshot smoke verifies Edge / Line role-map guidance.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_arithmetic", "Learn screenshot smoke exposes the Arithmetic topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnArithmetic", "Learn screenshot smoke verifies the Arithmetic topic.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_geometry", "Learn screenshot smoke exposes the Geometry topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnGeometry", "Learn screenshot smoke verifies the Geometry topic.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_color_hsv", "Learn screenshot smoke exposes the Color / HSV topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnColorHsv", "Learn screenshot smoke verifies the Color / HSV topic.");
        RequireContains(learnWindow, "14. Arithmetic / Logic", "OpenVision Learn topic list exposes Arithmetic / Logic.");
        RequireContains(learnWindow, "14 => \"LEARN_ARITHMETIC.md\"", "OpenVision Learn topic 14 resolves Arithmetic document.");
        RequireContains(learnWindow, "15. Geometry Transform", "OpenVision Learn topic list exposes Geometry Transform.");
        RequireContains(learnWindow, "15 => \"LEARN_GEOMETRY_TRANSFORM.md\"", "OpenVision Learn topic 15 resolves Geometry document.");
        RequireContains(learnWindow, "16. Color / HSV", "OpenVision Learn topic list exposes Color / HSV.");
        RequireContains(learnWindow, "16 => \"LEARN_COLOR_HSV.md\"", "OpenVision Learn topic 16 resolves Color / HSV document.");
        RequireContains(learnWindow, "temporary brightness bridge, not HSV sample evidence", "OpenVision Learn Color / HSV practice text explains that the mean path is a bridge, not HSV evidence.");
        RequireContains(learnWindowXaml, "OpenVisionLearnColorSampleBridge", "OpenVision Learn Color / HSV topic explains the current Mean Good/Bad sample bridge.");
        RequireContains(learnWindowXaml, "public HSV color-classification pair is not stable yet", "OpenVision Learn Color / HSV topic states the current HSV public sample gap.");
        RequireContains(learnWindowXaml, "MaskPixelRatio only after runner support exists", "OpenVision Learn Color / HSV topic does not present MaskPixelRatio as a current runner metric.");
        RequireContains(learnScreenshotSmoke, "OpenVisionLearnColorSampleBridge", "Learn screenshot smoke verifies the Color / HSV sample bridge.");
        RequireContains(learnScreenshotSmoke, "future metric", "Learn screenshot smoke verifies MaskPixelRatio is presented as future runner support.");
        string colorHsvGuide = Read(repoRoot, @"docs\learn\LEARN_COLOR_HSV.md");
        RequireContains(colorHsvGuide, "Do not add public HSV sample rows until", "Color / HSV Learn guide blocks public HSV samples until sample smoke evidence exists.");
        RequireContains(colorHsvGuide, "initial `HSV` mask ToolType", "Color / HSV Learn guide acknowledges initial HSV pipeline runner support.");
        RequireContains(colorHsvGuide, "`MaskPixelRatio` is available only for the `HSV` pipeline runner path", "Color / HSV Learn guide scopes MaskPixelRatio availability to the runner path.");
        RequireContains(colorHsvGuide, "## HSV Pipeline Runtime Contract", "Color / HSV Learn guide defines the runtime contract before public HSV samples.");
        RequireContains(colorHsvGuide, "`HueMin`, `HueMax`, `SaturationMin`, `SaturationMax`, `ValueMin`, `ValueMax`", "Color / HSV Learn guide defines required HSV range parameters.");
        RequireContains(colorHsvGuide, "`InputLayer` and `OutputLayer`", "Color / HSV Learn guide keeps HSV output mask routing explicit.");
        RequireContains(colorHsvGuide, "`MaskPixelCount`", "Color / HSV Learn guide requires count metrics before sample promotion.");
        RequireContains(colorHsvGuide, "bounded `MaskPixelRatio` range", "Color / HSV Learn guide requires bounded mask-ratio acceptance.");
        RequirePathExists(repoRoot, @"1. Core\VisionPipelineHsvMaskTool.cs", "HSV pipeline runner tool exists.");
        string hsvPipelineTool = Read(repoRoot, @"1. Core\VisionPipelineHsvMaskTool.cs");
        RequireContains(hsvPipelineTool, "VisionPipelineHsvMaskTool", "HSV pipeline runner tool is implemented.");
        RequireContains(hsvPipelineTool, "ColorConversionCodes.BGR2HSV", "HSV pipeline runner converts BGR input to HSV.");
        RequireContains(hsvPipelineTool, "MaskPixelRatio", "HSV pipeline runner reports mask ratio.");
        string pipelineKnownMetrics = Read(repoRoot, @"1. Core\VisionPipelineKnownMetrics.cs");
        RequireContains(pipelineKnownMetrics, "public const string MaskPixelCount", "Known metrics include MaskPixelCount.");
        RequireContains(pipelineKnownMetrics, "public const string MaskPixelRatio", "Known metrics include MaskPixelRatio.");
        RequireContains(pipelineKnownMetrics, "[\"hsv\"]", "Known metrics map HSV ToolType.");
        string pipelineValidator = Read(repoRoot, @"1. Core\VisionPipelineValidation.cs");
        RequireContains(pipelineValidator, "\"hsv\"", "Pipeline validator supports HSV ToolType.");
        string geometryGuide = Read(repoRoot, @"docs\learn\LEARN_GEOMETRY_TRANSFORM.md");
        RequireContains(geometryGuide, "Public_Geometry_RotateScale_Good", "Geometry Learn guide uses the public RotateScale sample.");
        RequireContains(geometryGuide, "Public_Geometry_RotateScale_Wide_Bad", "Geometry Learn guide uses the public RotateScale Bad sample.");
        RequireContains(geometryGuide, "ResultImageWidth=320", "Geometry Learn guide explains the Bad sample output-size drift.");
        RequireContains(geometryGuide, @"docs\samples\public\Geometry_RotateScale_Synthetic_OK.png", "Geometry Learn guide points to a public-safe synthetic image.");
        RequireContains(geometryGuide, @"docs\samples\public\Geometry_RotateScale_Synthetic_Wide_NG.png", "Geometry Learn guide points to the public-safe wide negative image.");
        RequireNotContains(geometryGuide, @"Sample\Contour.jpg", "Geometry Learn guide must not depend on the local legacy root Sample image.");

        string thresholdToolXaml = Read(repoRoot, @"0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml");
        RequireContains(thresholdToolXaml, "ThresholdToolLearnButton", "Threshold Tool exposes its compact Learn entry.");
        RequireContains(thresholdToolXaml, "Learn Threshold", "Threshold Tool Learn entry is labelled for Threshold.");
        RequireContains(learnWindow, ": this(threshold, maxValue, invert, 2)", "Threshold Tool Learn entry opens the Threshold topic.");
        RequireContains(learnWindow, "2 => \"LEARN_THRESHOLD.md\"", "Threshold Learn topic resolves LEARN_THRESHOLD.md.");
        RequirePathExists(repoRoot, @"docs\learn\LEARN_THRESHOLD.md", "Threshold Tool Learn document exists.");

        foreach ((string Document, string Link, string Asset) evidence in new[]
        {
            (
                @"docs\learn\LEARN_PRODUCT_SAMPLES.md",
                "../assets/tutorial/annotated/product_sample_source_result_sheet.png",
                @"docs\assets\tutorial\annotated\product_sample_source_result_sheet.png"
            ),
            (
                @"docs\learn\LEARN_MATCHING.md",
                "../assets/tutorial/annotated/public_matching_diepad_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_matching_diepad_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_BLOB.md",
                "../assets/tutorial/annotated/public_blob_particles_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_blob_particles_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_CONTOUR.md",
                "../assets/tutorial/annotated/public_contour_shapes_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_contour_shapes_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_THRESHOLD.md",
                "../assets/tutorial/annotated/public_threshold_bandpads_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_threshold_bandpads_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_MEAN.md",
                "../assets/tutorial/annotated/public_mean_brightness_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_mean_brightness_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_FEATURE_MATCHING.md",
                "../assets/tutorial/annotated/public_feature_card_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_feature_card_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_EDGE_BASED_MATCHING.md",
                "../assets/tutorial/annotated/public_edge_fiducial_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_edge_fiducial_good_callouts.png"
            ),
            (
                @"docs\learn\LEARN_LINE.md",
                "../assets/tutorial/annotated/public_line_pins_good_callouts.png",
                @"docs\assets\tutorial\annotated\public_line_pins_good_callouts.png"
            ),
        })
        {
            string learn = Read(repoRoot, evidence.Document);
            RequireContains(learn, evidence.Link, $"Learn document {evidence.Document} links current result evidence image.");
            RequirePathExists(repoRoot, evidence.Asset, $"Learn result evidence asset exists for {evidence.Document}.");
        }

        string portable = Read(repoRoot, @"docs\OPENVISIONLAB_TUTORIAL_PORTABLE.html");
        RequireContains(portable, "data:image/", "Portable tutorial embeds images.");
        Pass("Tutorial and learning document contract");
    }

    private static void CheckReleaseAndExternalPolicy(string repoRoot)
    {
        string external = Read(repoRoot, @"docs\OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md");
        RequireContains(external, "dll\\Library-Noah", "DLL reference policy covers vendored Library-Noah DLLs.");
        RequireContains(external, "dll\\OpenCVSharp", "DLL reference policy covers shared OpenCVSharp native runtime.");
        RequireContains(external, "System.Windows.Controls.WpfPropertyGrid.dll", "DLL reference policy covers WPG runtime DLL.");

        string release = Read(repoRoot, @"docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md");
        RequireContains(release, "dll\\Library-Noah", "Release policy covers Library-Noah DLL versioning.");
        RequireContains(release, "dll\\OpenCVSharp", "Release policy covers shared OpenCVSharp native runtime.");
        RequireContains(release, "System.Windows.Controls.WpfPropertyGrid.dll", "Release policy covers WPG runtime versioning.");
        RequireContains(release, "OpenVisionLab", "Release policy covers OpenVisionLab release evidence.");

        RequirePathExists(repoRoot, @"dll\OpenCVSharp\OpenCvSharpExtern.dll", "Shared OpenCVSharp native runtime exists.");
        RequirePathMissing(repoRoot, @"dll\Library-Noah\OpenCvSharpExtern.dll", "Legacy Library-Noah native runtime remains removed.");

        string aiPolicy = Read(repoRoot, @"docs\OPENVISIONLAB_AI_RECIPE_AUTOMATION_POLICY.md");
        RequireContains(aiPolicy, "acceptance", "AI Recipe automation policy covers acceptance changes.");
        RequireContains(aiPolicy, "ROI", "AI Recipe automation policy covers ROI changes.");
        Pass("Release and external library policy contract");
    }

    private static void CheckCompletedItemHygiene(string repoRoot)
    {
        RequirePathMissing(repoRoot, "Resources", "Root Resources folder remains removed.");
        RequirePathMissing(repoRoot, @"Properties\Resources.resx", "Stale Properties.Resources.resx remains removed.");
        RequirePathMissing(repoRoot, @"Properties\Resources.Designer.cs", "Stale Properties.Resources.Designer.cs remains removed.");

        string completed = Read(repoRoot, @"docs\OPENVISIONLAB_COMPLETED_TRACKER.md");
        RequireContains(completed, "Image Compare source format display", "Completed tracker records Image Compare source format completion.");
        RequireContains(completed, "Image Compare last-open directory", "Completed tracker records Image Compare last-open directory completion.");
        RequireContains(completed, "Priority 1-7 non-UI platform precheck", "Completed tracker records Priority 1-7 precheck completion.");

        string progress = Read(repoRoot, @"docs\OPENVISIONLAB_PROGRESS_TRACKER.md");
        RequireContains(progress, "Current Snapshot After Priority 1-7 Pass", "Progress tracker separates current 1-7 snapshot.");
        RequireContains(progress, "Removed From Active Work", "Progress tracker has a removed-from-active section.");
        Pass("Completed/progress tracker hygiene contract");
    }

    private static void RequireCatalogKey(string catalogText, string key)
    {
        string token = key + "\t";
        RequireContains(catalogText, token, $"Localization catalog contains key {key}.");
    }

    private static void RequireContains(string text, string token, string description)
    {
        if (string.IsNullOrEmpty(text) || text.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
        {
            Failures.Add($"{description} Missing token: {token}");
        }
    }

    private static void RequireContainsAny(string text, string description, params string[] tokens)
    {
        if (!string.IsNullOrEmpty(text)
            && tokens.Any(token => !string.IsNullOrWhiteSpace(token)
                && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0))
        {
            return;
        }

        Failures.Add($"{description} Missing one of: {string.Join(", ", tokens)}");
    }

    private static void ValidateProductSampleCatalog(string repoRoot)
    {
        string productCatalog = Read(repoRoot, @"docs\samples\OpenVisionLab.ProductSampleCatalog.csv");
        RequireContains(productCatalog, "Product_Battery_TabGap_Good", "Product sample catalog includes secondary-battery tab gap Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabGap_Narrow_Bad", "Product sample catalog includes secondary-battery tab gap Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_WeldSpatter_Good", "Product sample catalog includes secondary-battery weld blob Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_WeldSpatter_Heavy_Bad", "Product sample catalog includes secondary-battery weld blob Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_WeldOverburn_Good", "Product sample catalog includes secondary-battery weld overburn Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_WeldOverburn_Many_Bad", "Product sample catalog includes secondary-battery weld overburn Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabTear_Good", "Product sample catalog includes secondary-battery tab-tear Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabTear_Many_Bad", "Product sample catalog includes secondary-battery tab-tear Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabPlatingPeel_Good", "Product sample catalog includes secondary-battery tab-plating peel Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabPlatingPeel_Many_Bad", "Product sample catalog includes secondary-battery tab-plating peel Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_ElectrolyteStain_Good", "Product sample catalog includes secondary-battery electrolyte-stain Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_ElectrolyteStain_Heavy_Bad", "Product sample catalog includes secondary-battery electrolyte-stain Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorWrinkle_Good", "Product sample catalog includes secondary-battery separator-wrinkle Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorWrinkle_Many_Bad", "Product sample catalog includes secondary-battery separator-wrinkle Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorPinhole_Good", "Product sample catalog includes secondary-battery separator-pinhole Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorPinhole_Many_Bad", "Product sample catalog includes secondary-battery separator-pinhole Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_CoatingGap_Good", "Product sample catalog includes secondary-battery coating gap Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_CoatingGap_Narrow_Bad", "Product sample catalog includes secondary-battery coating gap Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_ForeignObject_Good", "Product sample catalog includes secondary-battery foreign-object Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_ForeignObject_Many_Bad", "Product sample catalog includes secondary-battery foreign-object Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_EdgeBurr_Good", "Product sample catalog includes secondary-battery edge-burr Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_EdgeBurr_Many_Bad", "Product sample catalog includes secondary-battery edge-burr Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabOffset_Good", "Product sample catalog includes secondary-battery tab-offset Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabOffset_Shifted_Bad", "Product sample catalog includes secondary-battery tab-offset Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealWidth_Good", "Product sample catalog includes secondary-battery seal-width Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealWidth_Narrow_Bad", "Product sample catalog includes secondary-battery seal-width Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabWeldVoid_Good", "Product sample catalog includes secondary-battery tab-weld void Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabWeldVoid_Many_Bad", "Product sample catalog includes secondary-battery tab-weld void Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchEdgeFold_Good", "Product sample catalog includes secondary-battery pouch-edge fold Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchEdgeFold_Many_Bad", "Product sample catalog includes secondary-battery pouch-edge fold Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchSealBurn_Good", "Product sample catalog includes secondary-battery pouch-seal burn Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchSealBurn_Many_Bad", "Product sample catalog includes secondary-battery pouch-seal burn Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchSealBubble_Good", "Product sample catalog includes secondary-battery pouch-seal bubble Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchSealBubble_Many_Bad", "Product sample catalog includes secondary-battery pouch-seal bubble Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealEdgeDelamination_Good", "Product sample catalog includes secondary-battery seal-edge delamination Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealEdgeDelamination_Many_Bad", "Product sample catalog includes secondary-battery seal-edge delamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabOxidation_Good", "Product sample catalog includes secondary-battery tab oxidation Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabOxidation_Many_Bad", "Product sample catalog includes secondary-battery tab oxidation Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabDiscoloration_Good", "Product sample catalog includes secondary-battery tab discoloration Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabDiscoloration_Dark_Bad", "Product sample catalog includes secondary-battery tab discoloration Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealContamination_Good", "Product sample catalog includes secondary-battery seal-contamination Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SealContamination_Many_Bad", "Product sample catalog includes secondary-battery seal-contamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_LaserMark_Good", "Product sample catalog includes secondary-battery laser-mark Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_LaserMark_Missing_Bad", "Product sample catalog includes secondary-battery laser-mark Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabDateCode_Good", "Product sample catalog includes secondary-battery tab date-code Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_TabDateCode_Wrong_Bad", "Product sample catalog includes secondary-battery tab date-code Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_ElectrolyteFillLine_Good", "Product sample catalog includes secondary-battery electrolyte fill-line Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_ElectrolyteFillLine_Low_Bad", "Product sample catalog includes secondary-battery electrolyte fill-line Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_CellVentAlignment_Good", "Product sample catalog includes secondary-battery cell vent alignment Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_CellVentAlignment_Shifted_Bad", "Product sample catalog includes secondary-battery cell vent alignment Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchTabSkew_Good", "Product sample catalog includes secondary-battery pouch tab skew Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_PouchTabSkew_Shifted_Bad", "Product sample catalog includes secondary-battery pouch tab skew Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_CurrentCollectorBurr_Good", "Product sample catalog includes secondary-battery current collector burr Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_CurrentCollectorBurr_Many_Bad", "Product sample catalog includes secondary-battery current collector burr Bad benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorEdgeTear_Good", "Product sample catalog includes secondary-battery separator edge-tear Good benchmark.");
        RequireContains(productCatalog, "Product_Battery_SeparatorEdgeTear_Many_Bad", "Product sample catalog includes secondary-battery separator edge-tear Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PixelDefect_Good", "Product sample catalog includes display contour Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PixelDefect_Many_Bad", "Product sample catalog includes display contour Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_Alignment_Good", "Product sample catalog includes display image-matching Good benchmark.");
        RequireContains(productCatalog, "Product_Display_Alignment_Wrong_Bad", "Product sample catalog includes display image-matching Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_Scratch_Good", "Product sample catalog includes display scratch contour Good benchmark.");
        RequireContains(productCatalog, "Product_Display_Scratch_Many_Bad", "Product sample catalog includes display scratch contour Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_BrightnessBand_Good", "Product sample catalog includes display brightness-band Good benchmark.");
        RequireContains(productCatalog, "Product_Display_BrightnessBand_Bright_Bad", "Product sample catalog includes display brightness-band Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_Particle_Good", "Product sample catalog includes display particle Good benchmark.");
        RequireContains(productCatalog, "Product_Display_Particle_Many_Bad", "Product sample catalog includes display particle Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraVariation_Good", "Product sample catalog includes display mura-variation Good benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraVariation_Uneven_Bad", "Product sample catalog includes display mura-variation Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_EdgeChip_Good", "Product sample catalog includes display edge-chip Good benchmark.");
        RequireContains(productCatalog, "Product_Display_EdgeChip_Many_Bad", "Product sample catalog includes display edge-chip Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_BezelChip_Good", "Product sample catalog includes display bezel-chip Good benchmark.");
        RequireContains(productCatalog, "Product_Display_BezelChip_Many_Bad", "Product sample catalog includes display bezel-chip Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_CornerCrack_Good", "Product sample catalog includes display corner-crack Good benchmark.");
        RequireContains(productCatalog, "Product_Display_CornerCrack_Many_Bad", "Product sample catalog includes display corner-crack Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_CornerLightLeak_Good", "Product sample catalog includes display corner-light-leak Good benchmark.");
        RequireContains(productCatalog, "Product_Display_CornerLightLeak_Bright_Bad", "Product sample catalog includes display corner-light-leak Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_BlackMatrixScratch_Good", "Product sample catalog includes display black-matrix scratch Good benchmark.");
        RequireContains(productCatalog, "Product_Display_BlackMatrixScratch_Many_Bad", "Product sample catalog includes display black-matrix scratch Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_LineDropout_Good", "Product sample catalog includes display line-dropout Good benchmark.");
        RequireContains(productCatalog, "Product_Display_LineDropout_Many_Bad", "Product sample catalog includes display line-dropout Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraSpotCluster_Good", "Product sample catalog includes display mura-spot-cluster Good benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraSpotCluster_Many_Bad", "Product sample catalog includes display mura-spot-cluster Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraRing_Good", "Product sample catalog includes display mura-ring Good benchmark.");
        RequireContains(productCatalog, "Product_Display_MuraRing_Many_Bad", "Product sample catalog includes display mura-ring Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerScratch_Good", "Product sample catalog includes display polarizer-scratch Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerScratch_Many_Bad", "Product sample catalog includes display polarizer-scratch Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerCrease_Good", "Product sample catalog includes display polarizer-crease Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerCrease_Many_Bad", "Product sample catalog includes display polarizer-crease Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_SealWidth_Good", "Product sample catalog includes display seal-width Good benchmark.");
        RequireContains(productCatalog, "Product_Display_SealWidth_Narrow_Bad", "Product sample catalog includes display seal-width Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_AlignmentOffset_Good", "Product sample catalog includes display alignment-offset Good benchmark.");
        RequireContains(productCatalog, "Product_Display_AlignmentOffset_Shifted_Bad", "Product sample catalog includes display alignment-offset Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_ColorFilterShift_Good", "Product sample catalog includes display color-filter shift Good benchmark.");
        RequireContains(productCatalog, "Product_Display_ColorFilterShift_Shifted_Bad", "Product sample catalog includes display color-filter shift Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_LineStain_Good", "Product sample catalog includes display line-stain Good benchmark.");
        RequireContains(productCatalog, "Product_Display_LineStain_Many_Bad", "Product sample catalog includes display line-stain Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_SubpixelBridge_Good", "Product sample catalog includes display subpixel-bridge Good benchmark.");
        RequireContains(productCatalog, "Product_Display_SubpixelBridge_Many_Bad", "Product sample catalog includes display subpixel-bridge Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PadBridge_Good", "Product sample catalog includes display pad-bridge Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PadBridge_Many_Bad", "Product sample catalog includes display pad-bridge Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerBubble_Good", "Product sample catalog includes display polarizer-bubble Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerBubble_Many_Bad", "Product sample catalog includes display polarizer-bubble Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_SealContamination_Good", "Product sample catalog includes display seal-contamination Good benchmark.");
        RequireContains(productCatalog, "Product_Display_SealContamination_Many_Bad", "Product sample catalog includes display seal-contamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_SealCornerContamination_Good", "Product sample catalog includes display seal-corner contamination Good benchmark.");
        RequireContains(productCatalog, "Product_Display_SealCornerContamination_Many_Bad", "Product sample catalog includes display seal-corner contamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerEdgeLift_Good", "Product sample catalog includes display polarizer edge-lift Good benchmark.");
        RequireContains(productCatalog, "Product_Display_PolarizerEdgeLift_Many_Bad", "Product sample catalog includes display polarizer edge-lift Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_CofBondParticle_Good", "Product sample catalog includes display COF bond particle Good benchmark.");
        RequireContains(productCatalog, "Product_Display_CofBondParticle_Many_Bad", "Product sample catalog includes display COF bond particle Bad benchmark.");
        RequireContains(productCatalog, "Product_Display_FpcAlignmentMark_Good", "Product sample catalog includes display FPC alignment mark Good benchmark.");
        RequireContains(productCatalog, "Product_Display_FpcAlignmentMark_Wrong_Bad", "Product sample catalog includes display FPC alignment mark Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_Fiducial_Good", "Product sample catalog includes semiconductor edge-based matching Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_Fiducial_Wrong_Bad", "Product sample catalog includes semiconductor edge-based matching Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondMark_Good", "Product sample catalog includes semiconductor feature/shape matching Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondMark_Wrong_Bad", "Product sample catalog includes semiconductor feature/shape matching Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadContamination_Good", "Product sample catalog includes semiconductor pad contamination Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadContamination_Heavy_Bad", "Product sample catalog includes semiconductor pad contamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadPitch_Good", "Product sample catalog includes semiconductor pad pitch Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadPitch_Narrow_Bad", "Product sample catalog includes semiconductor pad pitch Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_RotationMark_Good", "Product sample catalog includes semiconductor rotation-mark Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_RotationMark_Rotated_Bad", "Product sample catalog includes semiconductor rotation-mark Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadAlignment_Good", "Product sample catalog includes semiconductor lead-alignment Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadAlignment_Shifted_Bad", "Product sample catalog includes semiconductor lead-alignment Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadWidth_Good", "Product sample catalog includes semiconductor lead-width Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadWidth_Narrow_Bad", "Product sample catalog includes semiconductor lead-width Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadCoplanarity_Good", "Product sample catalog includes semiconductor lead-coplanarity Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadCoplanarity_Shifted_Bad", "Product sample catalog includes semiconductor lead-coplanarity Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_ProbeMark_Good", "Product sample catalog includes semiconductor probe-mark Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_ProbeMark_Many_Bad", "Product sample catalog includes semiconductor probe-mark Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_DieEdgeChip_Good", "Product sample catalog includes semiconductor die-edge chip Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_DieEdgeChip_Many_Bad", "Product sample catalog includes semiconductor die-edge chip Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_MoldingFlash_Good", "Product sample catalog includes semiconductor molding-flash Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_MoldingFlash_Many_Bad", "Product sample catalog includes semiconductor molding-flash Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageCrack_Good", "Product sample catalog includes semiconductor package-crack Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageCrack_Many_Bad", "Product sample catalog includes semiconductor package-crack Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageCornerChip_Good", "Product sample catalog includes semiconductor package corner-chip Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageCornerChip_Many_Bad", "Product sample catalog includes semiconductor package corner-chip Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadBurr_Good", "Product sample catalog includes semiconductor lead-burr Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadBurr_Many_Bad", "Product sample catalog includes semiconductor lead-burr Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadCrack_Good", "Product sample catalog includes semiconductor lead-crack Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadCrack_Many_Bad", "Product sample catalog includes semiconductor lead-crack Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadOxidation_Good", "Product sample catalog includes semiconductor lead-oxidation Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_LeadOxidation_Many_Bad", "Product sample catalog includes semiconductor lead-oxidation Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackagePolarity_Good", "Product sample catalog includes semiconductor package-polarity Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackagePolarity_Missing_Bad", "Product sample catalog includes semiconductor package-polarity Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageLaserText_Good", "Product sample catalog includes semiconductor package laser-text Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageLaserText_Missing_Bad", "Product sample catalog includes semiconductor package laser-text Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WaferDieMark_Good", "Product sample catalog includes semiconductor wafer die mark Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WaferDieMark_Wrong_Bad", "Product sample catalog includes semiconductor wafer die mark Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_DieContamination_Good", "Product sample catalog includes semiconductor die contamination Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_DieContamination_Heavy_Bad", "Product sample catalog includes semiconductor die contamination Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_UnderfillVoid_Good", "Product sample catalog includes semiconductor underfill void Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_UnderfillVoid_Many_Bad", "Product sample catalog includes semiconductor underfill void Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageVoid_Good", "Product sample catalog includes semiconductor package void Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PackageVoid_Many_Bad", "Product sample catalog includes semiconductor package void Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_SolderBridge_Good", "Product sample catalog includes semiconductor solder bridge Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_SolderBridge_Many_Bad", "Product sample catalog includes semiconductor solder bridge Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadScratch_Good", "Product sample catalog includes semiconductor pad scratch Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_PadScratch_Many_Bad", "Product sample catalog includes semiconductor pad scratch Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondPadNick_Good", "Product sample catalog includes semiconductor bond-pad nick Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondPadNick_Many_Bad", "Product sample catalog includes semiconductor bond-pad nick Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WireBondLift_Good", "Product sample catalog includes semiconductor wire-bond lift Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WireBondLift_Many_Bad", "Product sample catalog includes semiconductor wire-bond lift Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WireSweepAlignment_Good", "Product sample catalog includes semiconductor wire sweep alignment Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_WireSweepAlignment_Shifted_Bad", "Product sample catalog includes semiconductor wire sweep alignment Bad benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondPadCorrosion_Good", "Product sample catalog includes semiconductor bond-pad corrosion Good benchmark.");
        RequireContains(productCatalog, "Product_Semiconductor_BondPadCorrosion_Many_Bad", "Product sample catalog includes semiconductor bond-pad corrosion Bad benchmark.");
        RequireContains(productCatalog, "ExpectedFailure", "Product sample catalog includes controlled NG rows.");
        RequireContains(productCatalog, "Battery_TabGap,Bad", "Product sample catalog groups battery tab gap Good/Bad pair.");
        RequireContains(productCatalog, "Battery_WeldSpatter,Bad", "Product sample catalog groups battery weld Good/Bad pair.");
        RequireContains(productCatalog, "Battery_WeldOverburn,Bad", "Product sample catalog groups battery weld overburn Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabTear,Bad", "Product sample catalog groups battery tab-tear Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabPlatingPeel,Bad", "Product sample catalog groups battery tab-plating peel Good/Bad pair.");
        RequireContains(productCatalog, "Battery_ElectrolyteStain,Bad", "Product sample catalog groups battery electrolyte-stain Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SeparatorWrinkle,Bad", "Product sample catalog groups battery separator-wrinkle Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SeparatorPinhole,Bad", "Product sample catalog groups battery separator-pinhole Good/Bad pair.");
        RequireContains(productCatalog, "Battery_CoatingGap,Bad", "Product sample catalog groups battery coating gap Good/Bad pair.");
        RequireContains(productCatalog, "Battery_ForeignObject,Bad", "Product sample catalog groups battery foreign-object Good/Bad pair.");
        RequireContains(productCatalog, "Battery_EdgeBurr,Bad", "Product sample catalog groups battery edge-burr Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabOffset,Bad", "Product sample catalog groups battery tab-offset Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SealWidth,Bad", "Product sample catalog groups battery seal-width Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabWeldVoid,Bad", "Product sample catalog groups battery tab-weld void Good/Bad pair.");
        RequireContains(productCatalog, "Battery_PouchEdgeFold,Bad", "Product sample catalog groups battery pouch-edge fold Good/Bad pair.");
        RequireContains(productCatalog, "Battery_PouchSealBurn,Bad", "Product sample catalog groups battery pouch-seal burn Good/Bad pair.");
        RequireContains(productCatalog, "Battery_PouchSealBubble,Bad", "Product sample catalog groups battery pouch-seal bubble Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SealEdgeDelamination,Bad", "Product sample catalog groups battery seal-edge delamination Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabOxidation,Bad", "Product sample catalog groups battery tab oxidation Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabDiscoloration,Bad", "Product sample catalog groups battery tab discoloration Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SealContamination,Bad", "Product sample catalog groups battery seal-contamination Good/Bad pair.");
        RequireContains(productCatalog, "Battery_LaserMark,Bad", "Product sample catalog groups battery laser-mark Good/Bad pair.");
        RequireContains(productCatalog, "Battery_TabDateCode,Bad", "Product sample catalog groups battery tab date-code Good/Bad pair.");
        RequireContains(productCatalog, "Battery_ElectrolyteFillLine,Bad", "Product sample catalog groups battery electrolyte fill-line Good/Bad pair.");
        RequireContains(productCatalog, "Battery_CellVentAlignment,Bad", "Product sample catalog groups battery cell vent alignment Good/Bad pair.");
        RequireContains(productCatalog, "Battery_PouchTabSkew,Bad", "Product sample catalog groups battery pouch tab skew Good/Bad pair.");
        RequireContains(productCatalog, "Battery_CurrentCollectorBurr,Bad", "Product sample catalog groups battery current collector burr Good/Bad pair.");
        RequireContains(productCatalog, "Battery_SeparatorEdgeTear,Bad", "Product sample catalog groups battery separator edge-tear Good/Bad pair.");
        RequireContains(productCatalog, "Display_PixelDefect,Bad", "Product sample catalog groups display pixel-defect Good/Bad pair.");
        RequireContains(productCatalog, "Display_Alignment,Bad", "Product sample catalog groups display alignment Good/Bad pair.");
        RequireContains(productCatalog, "Display_Scratch,Bad", "Product sample catalog groups display scratch Good/Bad pair.");
        RequireContains(productCatalog, "Display_BrightnessBand,Bad", "Product sample catalog groups display brightness-band Good/Bad pair.");
        RequireContains(productCatalog, "Display_Particle,Bad", "Product sample catalog groups display particle Good/Bad pair.");
        RequireContains(productCatalog, "Display_MuraVariation,Bad", "Product sample catalog groups display mura-variation Good/Bad pair.");
        RequireContains(productCatalog, "Display_EdgeChip,Bad", "Product sample catalog groups display edge-chip Good/Bad pair.");
        RequireContains(productCatalog, "Display_BezelChip,Bad", "Product sample catalog groups display bezel-chip Good/Bad pair.");
        RequireContains(productCatalog, "Display_CornerCrack,Bad", "Product sample catalog groups display corner-crack Good/Bad pair.");
        RequireContains(productCatalog, "Display_CornerLightLeak,Bad", "Product sample catalog groups display corner-light-leak Good/Bad pair.");
        RequireContains(productCatalog, "Display_BlackMatrixScratch,Bad", "Product sample catalog groups display black-matrix scratch Good/Bad pair.");
        RequireContains(productCatalog, "Display_LineDropout,Bad", "Product sample catalog groups display line-dropout Good/Bad pair.");
        RequireContains(productCatalog, "Display_MuraSpotCluster,Bad", "Product sample catalog groups display mura-spot-cluster Good/Bad pair.");
        RequireContains(productCatalog, "Display_MuraRing,Bad", "Product sample catalog groups display mura-ring Good/Bad pair.");
        RequireContains(productCatalog, "Display_PolarizerScratch,Bad", "Product sample catalog groups display polarizer-scratch Good/Bad pair.");
        RequireContains(productCatalog, "Display_PolarizerCrease,Bad", "Product sample catalog groups display polarizer-crease Good/Bad pair.");
        RequireContains(productCatalog, "Display_SealWidth,Bad", "Product sample catalog groups display seal-width Good/Bad pair.");
        RequireContains(productCatalog, "Display_AlignmentOffset,Bad", "Product sample catalog groups display alignment-offset Good/Bad pair.");
        RequireContains(productCatalog, "Display_ColorFilterShift,Bad", "Product sample catalog groups display color-filter shift Good/Bad pair.");
        RequireContains(productCatalog, "Display_LineStain,Bad", "Product sample catalog groups display line-stain Good/Bad pair.");
        RequireContains(productCatalog, "Display_SubpixelBridge,Bad", "Product sample catalog groups display subpixel-bridge Good/Bad pair.");
        RequireContains(productCatalog, "Display_PadBridge,Bad", "Product sample catalog groups display pad-bridge Good/Bad pair.");
        RequireContains(productCatalog, "Display_PolarizerBubble,Bad", "Product sample catalog groups display polarizer-bubble Good/Bad pair.");
        RequireContains(productCatalog, "Display_SealContamination,Bad", "Product sample catalog groups display seal-contamination Good/Bad pair.");
        RequireContains(productCatalog, "Display_SealCornerContamination,Bad", "Product sample catalog groups display seal-corner contamination Good/Bad pair.");
        RequireContains(productCatalog, "Display_PolarizerEdgeLift,Bad", "Product sample catalog groups display polarizer edge-lift Good/Bad pair.");
        RequireContains(productCatalog, "Display_CofBondParticle,Bad", "Product sample catalog groups display COF bond particle Good/Bad pair.");
        RequireContains(productCatalog, "Display_FpcAlignmentMark,Bad", "Product sample catalog groups display FPC alignment mark Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_Fiducial,Bad", "Product sample catalog groups semiconductor fiducial Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_BondMark,Bad", "Product sample catalog groups semiconductor bond mark Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PadContamination,Bad", "Product sample catalog groups semiconductor pad contamination Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PadPitch,Bad", "Product sample catalog groups semiconductor pad pitch Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_RotationMark,Bad", "Product sample catalog groups semiconductor rotation-mark Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadAlignment,Bad", "Product sample catalog groups semiconductor lead-alignment Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadWidth,Bad", "Product sample catalog groups semiconductor lead-width Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadCoplanarity,Bad", "Product sample catalog groups semiconductor lead-coplanarity Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_ProbeMark,Bad", "Product sample catalog groups semiconductor probe-mark Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_DieEdgeChip,Bad", "Product sample catalog groups semiconductor die-edge chip Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_MoldingFlash,Bad", "Product sample catalog groups semiconductor molding-flash Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PackageCrack,Bad", "Product sample catalog groups semiconductor package-crack Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PackageCornerChip,Bad", "Product sample catalog groups semiconductor package corner-chip Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadBurr,Bad", "Product sample catalog groups semiconductor lead-burr Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadCrack,Bad", "Product sample catalog groups semiconductor lead-crack Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_LeadOxidation,Bad", "Product sample catalog groups semiconductor lead-oxidation Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PackagePolarity,Bad", "Product sample catalog groups semiconductor package-polarity Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PackageLaserText,Bad", "Product sample catalog groups semiconductor package laser-text Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_WaferDieMark,Bad", "Product sample catalog groups semiconductor wafer die mark Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_DieContamination,Bad", "Product sample catalog groups semiconductor die contamination Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_UnderfillVoid,Bad", "Product sample catalog groups semiconductor underfill void Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PackageVoid,Bad", "Product sample catalog groups semiconductor package void Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_SolderBridge,Bad", "Product sample catalog groups semiconductor solder bridge Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_PadScratch,Bad", "Product sample catalog groups semiconductor pad scratch Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_BondPadNick,Bad", "Product sample catalog groups semiconductor bond-pad nick Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_WireBondLift,Bad", "Product sample catalog groups semiconductor wire-bond lift Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_WireSweepAlignment,Bad", "Product sample catalog groups semiconductor wire sweep alignment Good/Bad pair.");
        RequireContains(productCatalog, "Semiconductor_BondPadCorrosion,Bad", "Product sample catalog groups semiconductor bond-pad corrosion Good/Bad pair.");
        RequireNotContains(productCatalog, @"Sample\", "Product sample catalog must not use legacy SDK Sample paths.");
        RequireNotContains(productCatalog, @"Sample/", "Product sample catalog must not use legacy SDK Sample paths.");
        RequireNotContains(productCatalog, "Euresys", "Product sample catalog must not depend on Euresys sample assets.");
        RequireNotContains(productCatalog, "MVTec", "Product sample catalog must not depend on MVTec non-commercial assets.");

        string productManifest = Read(repoRoot, @"docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv");
        RequireContains(productManifest, "Battery_TabGap_OK.png", "Product sample manifest includes battery tab gap OK image.");
        RequireContains(productManifest, "Battery_WeldSpatter_OK.png", "Product sample manifest includes battery weld OK image.");
        RequireContains(productManifest, "Battery_WeldOverburn_OK.png", "Product sample manifest includes battery weld overburn OK image.");
        RequireContains(productManifest, "Battery_TabTear_OK.png", "Product sample manifest includes battery tab-tear OK image.");
        RequireContains(productManifest, "Battery_TabPlatingPeel_OK.png", "Product sample manifest includes battery tab-plating peel OK image.");
        RequireContains(productManifest, "Battery_ElectrolyteStain_OK.png", "Product sample manifest includes battery electrolyte-stain OK image.");
        RequireContains(productManifest, "Battery_SeparatorWrinkle_OK.png", "Product sample manifest includes battery separator-wrinkle OK image.");
        RequireContains(productManifest, "Battery_SeparatorPinhole_OK.png", "Product sample manifest includes battery separator-pinhole OK image.");
        RequireContains(productManifest, "Battery_CoatingGap_OK.png", "Product sample manifest includes battery coating gap OK image.");
        RequireContains(productManifest, "Battery_ForeignObject_OK.png", "Product sample manifest includes battery foreign-object OK image.");
        RequireContains(productManifest, "Battery_EdgeBurr_OK.png", "Product sample manifest includes battery edge-burr OK image.");
        RequireContains(productManifest, "Battery_TabOffset_OK.png", "Product sample manifest includes battery tab-offset OK image.");
        RequireContains(productManifest, "Battery_SealWidth_OK.png", "Product sample manifest includes battery seal-width OK image.");
        RequireContains(productManifest, "Battery_TabWeldVoid_OK.png", "Product sample manifest includes battery tab-weld void OK image.");
        RequireContains(productManifest, "Battery_PouchEdgeFold_OK.png", "Product sample manifest includes battery pouch-edge fold OK image.");
        RequireContains(productManifest, "Battery_PouchSealBurn_OK.png", "Product sample manifest includes battery pouch-seal burn OK image.");
        RequireContains(productManifest, "Battery_PouchSealBubble_OK.png", "Product sample manifest includes battery pouch-seal bubble OK image.");
        RequireContains(productManifest, "Battery_SealEdgeDelamination_OK.png", "Product sample manifest includes battery seal-edge delamination OK image.");
        RequireContains(productManifest, "Battery_TabOxidation_OK.png", "Product sample manifest includes battery tab oxidation OK image.");
        RequireContains(productManifest, "Battery_TabDiscoloration_OK.png", "Product sample manifest includes battery tab discoloration OK image.");
        RequireContains(productManifest, "Battery_SealContamination_OK.png", "Product sample manifest includes battery seal-contamination OK image.");
        RequireContains(productManifest, "Battery_LaserMark_Template.png", "Product sample manifest includes battery laser-mark template.");
        RequireContains(productManifest, "Battery_TabDateCode_Template.png", "Product sample manifest includes battery tab date-code template.");
        RequireContains(productManifest, "Battery_ElectrolyteFillLine_OK.png", "Product sample manifest includes battery electrolyte fill-line OK image.");
        RequireContains(productManifest, "Battery_CellVentAlignment_OK.png", "Product sample manifest includes battery cell vent alignment OK image.");
        RequireContains(productManifest, "Battery_PouchTabSkew_OK.png", "Product sample manifest includes battery pouch tab skew OK image.");
        RequireContains(productManifest, "Battery_CurrentCollectorBurr_OK.png", "Product sample manifest includes battery current collector burr OK image.");
        RequireContains(productManifest, "Battery_SeparatorEdgeTear_OK.png", "Product sample manifest includes battery separator edge-tear OK image.");
        RequireContains(productManifest, "Display_PixelDefect_OK.png", "Product sample manifest includes display pixel defect OK image.");
        RequireContains(productManifest, "Display_Alignment_Template.png", "Product sample manifest includes display alignment template.");
        RequireContains(productManifest, "Display_Scratch_OK.png", "Product sample manifest includes display scratch OK image.");
        RequireContains(productManifest, "Display_BrightnessBand_OK.png", "Product sample manifest includes display brightness-band OK image.");
        RequireContains(productManifest, "Display_Particle_OK.png", "Product sample manifest includes display particle OK image.");
        RequireContains(productManifest, "Display_MuraVariation_OK.png", "Product sample manifest includes display mura-variation OK image.");
        RequireContains(productManifest, "Display_EdgeChip_OK.png", "Product sample manifest includes display edge-chip OK image.");
        RequireContains(productManifest, "Display_BezelChip_OK.png", "Product sample manifest includes display bezel-chip OK image.");
        RequireContains(productManifest, "Display_CornerCrack_OK.png", "Product sample manifest includes display corner-crack OK image.");
        RequireContains(productManifest, "Display_CornerLightLeak_OK.png", "Product sample manifest includes display corner-light-leak OK image.");
        RequireContains(productManifest, "Display_BlackMatrixScratch_OK.png", "Product sample manifest includes display black-matrix scratch OK image.");
        RequireContains(productManifest, "Display_LineDropout_OK.png", "Product sample manifest includes display line-dropout OK image.");
        RequireContains(productManifest, "Display_MuraSpotCluster_OK.png", "Product sample manifest includes display mura-spot-cluster OK image.");
        RequireContains(productManifest, "Display_MuraRing_OK.png", "Product sample manifest includes display mura-ring OK image.");
        RequireContains(productManifest, "Display_PolarizerScratch_OK.png", "Product sample manifest includes display polarizer-scratch OK image.");
        RequireContains(productManifest, "Display_PolarizerCrease_OK.png", "Product sample manifest includes display polarizer-crease OK image.");
        RequireContains(productManifest, "Display_SealWidth_OK.png", "Product sample manifest includes display seal-width OK image.");
        RequireContains(productManifest, "Display_AlignmentOffset_OK.png", "Product sample manifest includes display alignment-offset OK image.");
        RequireContains(productManifest, "Display_ColorFilterShift_OK.png", "Product sample manifest includes display color-filter shift OK image.");
        RequireContains(productManifest, "Display_LineStain_OK.png", "Product sample manifest includes display line-stain OK image.");
        RequireContains(productManifest, "Display_SubpixelBridge_OK.png", "Product sample manifest includes display subpixel-bridge OK image.");
        RequireContains(productManifest, "Display_PadBridge_OK.png", "Product sample manifest includes display pad-bridge OK image.");
        RequireContains(productManifest, "Display_PolarizerBubble_OK.png", "Product sample manifest includes display polarizer-bubble OK image.");
        RequireContains(productManifest, "Display_SealContamination_OK.png", "Product sample manifest includes display seal-contamination OK image.");
        RequireContains(productManifest, "Display_SealCornerContamination_OK.png", "Product sample manifest includes display seal-corner contamination OK image.");
        RequireContains(productManifest, "Display_PolarizerEdgeLift_OK.png", "Product sample manifest includes display polarizer edge-lift OK image.");
        RequireContains(productManifest, "Display_CofBondParticle_OK.png", "Product sample manifest includes display COF bond particle OK image.");
        RequireContains(productManifest, "Display_FpcAlignmentMark_Template.png", "Product sample manifest includes display FPC alignment mark template.");
        RequireContains(productManifest, "Semiconductor_Fiducial_Template.png", "Product sample manifest includes semiconductor fiducial template.");
        RequireContains(productManifest, "Semiconductor_BondMark_Template.png", "Product sample manifest includes semiconductor bond mark template.");
        RequireContains(productManifest, "Semiconductor_PadContamination_OK.png", "Product sample manifest includes semiconductor pad contamination OK image.");
        RequireContains(productManifest, "Semiconductor_PadPitch_OK.png", "Product sample manifest includes semiconductor pad pitch OK image.");
        RequireContains(productManifest, "Semiconductor_RotationMark_Template.png", "Product sample manifest includes semiconductor rotation mark template.");
        RequireContains(productManifest, "Semiconductor_LeadAlignment_OK.png", "Product sample manifest includes semiconductor lead-alignment OK image.");
        RequireContains(productManifest, "Semiconductor_LeadWidth_OK.png", "Product sample manifest includes semiconductor lead-width OK image.");
        RequireContains(productManifest, "Semiconductor_LeadCoplanarity_OK.png", "Product sample manifest includes semiconductor lead-coplanarity OK image.");
        RequireContains(productManifest, "Semiconductor_ProbeMark_OK.png", "Product sample manifest includes semiconductor probe-mark OK image.");
        RequireContains(productManifest, "Semiconductor_DieEdgeChip_OK.png", "Product sample manifest includes semiconductor die-edge chip OK image.");
        RequireContains(productManifest, "Semiconductor_MoldingFlash_OK.png", "Product sample manifest includes semiconductor molding-flash OK image.");
        RequireContains(productManifest, "Semiconductor_PackageCrack_OK.png", "Product sample manifest includes semiconductor package-crack OK image.");
        RequireContains(productManifest, "Semiconductor_PackageCornerChip_OK.png", "Product sample manifest includes semiconductor package corner-chip OK image.");
        RequireContains(productManifest, "Semiconductor_LeadBurr_OK.png", "Product sample manifest includes semiconductor lead-burr OK image.");
        RequireContains(productManifest, "Semiconductor_LeadCrack_OK.png", "Product sample manifest includes semiconductor lead-crack OK image.");
        RequireContains(productManifest, "Semiconductor_LeadOxidation_OK.png", "Product sample manifest includes semiconductor lead-oxidation OK image.");
        RequireContains(productManifest, "Semiconductor_PackagePolarity_Template.png", "Product sample manifest includes semiconductor package-polarity template.");
        RequireContains(productManifest, "Semiconductor_PackageLaserText_Template.png", "Product sample manifest includes semiconductor package laser-text template.");
        RequireContains(productManifest, "Semiconductor_WaferDieMark_Template.png", "Product sample manifest includes semiconductor wafer die mark template.");
        RequireContains(productManifest, "Semiconductor_DieContamination_OK.png", "Product sample manifest includes semiconductor die contamination OK image.");
        RequireContains(productManifest, "Semiconductor_UnderfillVoid_OK.png", "Product sample manifest includes semiconductor underfill void OK image.");
        RequireContains(productManifest, "Semiconductor_PackageVoid_OK.png", "Product sample manifest includes semiconductor package void OK image.");
        RequireContains(productManifest, "Semiconductor_SolderBridge_OK.png", "Product sample manifest includes semiconductor solder bridge OK image.");
        RequireContains(productManifest, "Semiconductor_PadScratch_OK.png", "Product sample manifest includes semiconductor pad scratch OK image.");
        RequireContains(productManifest, "Semiconductor_BondPadNick_OK.png", "Product sample manifest includes semiconductor bond-pad nick OK image.");
        RequireContains(productManifest, "Semiconductor_WireBondLift_OK.png", "Product sample manifest includes semiconductor wire-bond lift OK image.");
        RequireContains(productManifest, "Semiconductor_WireSweepAlignment_OK.png", "Product sample manifest includes semiconductor wire sweep alignment OK image.");
        RequireContains(productManifest, "Semiconductor_BondPadCorrosion_OK.png", "Product sample manifest includes semiconductor bond-pad corrosion OK image.");
        RequireNotContains(productManifest, @"Sample\", "Product sample manifest must not use legacy SDK Sample paths.");
        RequireNotContains(productManifest, @"Sample/", "Product sample manifest must not use legacy SDK Sample paths.");
        RequireNotContains(productManifest, "Euresys", "Product sample manifest must not depend on Euresys sample assets.");
        RequireNotContains(productManifest, "MVTec", "Product sample manifest must not depend on MVTec non-commercial assets.");

        foreach (string relativePath in new[]
        {
            @"docs\samples\public\product\Product_Battery_TabGap_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_WeldSpatter_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_WeldOverburn_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabTear_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabPlatingPeel_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_ElectrolyteStain_Mean.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SeparatorWrinkle_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SeparatorPinhole_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_CoatingGap_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_ForeignObject_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_EdgeBurr_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabOffset_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SealWidth_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabWeldVoid_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_PouchEdgeFold_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_PouchSealBurn_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_PouchSealBubble_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SealEdgeDelamination_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabOxidation_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabDiscoloration_Mean.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SealContamination_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_LaserMark_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_TabDateCode_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_ElectrolyteFillLine_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_CellVentAlignment_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_PouchTabSkew_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_CurrentCollectorBurr_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Battery_SeparatorEdgeTear_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PixelDefect_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_Alignment_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Display_Scratch_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_BrightnessBand_Mean.pipeline.xml",
            @"docs\samples\public\product\Product_Display_Particle_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_MuraVariation_Mean.pipeline.xml",
            @"docs\samples\public\product\Product_Display_EdgeChip_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_BezelChip_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_CornerCrack_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_CornerLightLeak_Mean.pipeline.xml",
            @"docs\samples\public\product\Product_Display_BlackMatrixScratch_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_LineDropout_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_MuraSpotCluster_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_MuraRing_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PolarizerScratch_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PolarizerCrease_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_SealWidth_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Display_AlignmentOffset_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Display_ColorFilterShift_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Display_LineStain_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_SubpixelBridge_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PadBridge_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PolarizerBubble_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_SealContamination_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_SealCornerContamination_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_PolarizerEdgeLift_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Display_CofBondParticle_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Display_FpcAlignmentMark_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_Fiducial_Edge.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_BondMark_Feature.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PadContamination_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PadPitch_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_RotationMark_Edge.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadAlignment_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadWidth_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadCoplanarity_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_ProbeMark_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_DieEdgeChip_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_MoldingFlash_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PackageCrack_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PackageCornerChip_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadBurr_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadCrack_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_LeadOxidation_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PackagePolarity_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PackageLaserText_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_WaferDieMark_Matching.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_DieContamination_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_UnderfillVoid_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PackageVoid_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_SolderBridge_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_PadScratch_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_BondPadNick_Contour.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_WireBondLift_Blob.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_WireSweepAlignment_Distance.pipeline.xml",
            @"docs\samples\public\product\Product_Semiconductor_BondPadCorrosion_Blob.pipeline.xml",
        })
        {
            string pipeline = Read(repoRoot, relativePath);
            RequireContains(pipeline, "Main", $"Product pipeline {relativePath} reads the Main input layer.");
            RequireNotContains(pipeline, @"Sample\", $"Product pipeline {relativePath} must not use legacy SDK Sample paths.");
            RequireNotContains(pipeline, @"Sample/", $"Product pipeline {relativePath} must not use legacy SDK Sample paths.");
            RequireNotContains(pipeline, "Euresys", $"Product pipeline {relativePath} must not depend on Euresys sample assets.");
            RequireNotContains(pipeline, "MVTec", $"Product pipeline {relativePath} must not depend on MVTec non-commercial assets.");
        }
    }

    private static void RequireNotContains(string text, string token, string description)
    {
        if (!string.IsNullOrEmpty(text) && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            Failures.Add($"{description} Forbidden token: {token}");
        }
    }

    private static void RequirePathExists(string repoRoot, string relativePath, string description)
    {
        string path = Path.Combine(repoRoot, relativePath);
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            Failures.Add($"{description} Path was not found: {path}");
        }
    }

    private static void RequirePathMissing(string repoRoot, string relativePath, string description)
    {
        string path = Path.Combine(repoRoot, relativePath);
        if (File.Exists(path) || Directory.Exists(path))
        {
            Failures.Add($"{description} Path still exists: {path}");
        }
    }

    private static string Read(string repoRoot, string relativePath)
    {
        string path = Path.Combine(repoRoot, relativePath);
        if (!File.Exists(path))
        {
            Failures.Add(string.Create(CultureInfo.InvariantCulture, $"Required file not found: {path}"));
            return string.Empty;
        }

        return File.ReadAllText(path, Encoding.UTF8);
    }

    private static void Pass(string item)
    {
        Passed.Add(item);
    }

    private static string FindRepoRoot(string startPath)
    {
        DirectoryInfo directory = new DirectoryInfo(startPath);
        while (directory != null)
        {
            string solution = Path.Combine(directory.FullName, "OpenVisionLab.sln");
            if (File.Exists(solution))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return string.Empty;
    }
}
