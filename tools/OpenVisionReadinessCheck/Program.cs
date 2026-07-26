using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
        CheckSourceOwnership(repoRoot);
        CheckToolViewSourceOrganization(repoRoot);
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

        string shellHost = Read(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostView.xaml.cs");
        RequireContains(shellHost, "SetDirectRunPending", "Pending state is surfaced by the WPF shell direct-run pending state.");

        string documentController = Read(repoRoot, @"UI\Menu\Wpf\Shell\Documents\OpenVisionShellHostDocumentController.cs");
        RequireContains(documentController, "ActivatePendingTool", "Pending tool state is represented by a WPF ViewModel.");
        RequireContains(documentController, "OpenVisionPendingToolViewModel", "Pending tool state is represented by a WPF ViewModel.");

        string statePresenter = Read(repoRoot, @"UI\Menu\Wpf\Shell\State\OpenVisionShellHostStatePresenter.cs");
        RequireContains(statePresenter, "ActivePendingToolTitle", "Pending tool state is projected through shell host state presenter.");

        string commandCatalog = Read(repoRoot, @"UI\Menu\Wpf\Shell\Commands\OpenVisionShellCommandCatalog.cs");
        RequireContains(commandCatalog, "PendingAlgorithmTool", "Algorithm tools without completed views are marked as pending work.");

        string recipeCommandSurface = ReadSourceFamily(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.cs");
        RequireNotContains(recipeCommandSurface, "public sealed class OpenVisionRecipeValidationSuiteScopeOption", "Recipe command surface no longer owns validation/review model declarations.");
        RequireNotContains(recipeCommandSurface, "public sealed class OpenVisionRecipeSampleRunSummary", "Recipe command surface no longer owns sample/batch model declarations.");

        string recipeValidationReviewModels = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Models\OpenVisionRecipeValidationReviewModels.cs");
        RequireContains(recipeValidationReviewModels, "public sealed class OpenVisionRecipeValidationSuiteScopeOption", "Recipe validation/review models have an explicit owner.");
        RequireContains(recipeValidationReviewModels, "public sealed class OpenVisionRecipeOperatorResultChannelRow", "Recipe operator decision rows have an explicit owner.");

        string recipeSampleRunModels = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Models\OpenVisionRecipeSampleRunModels.cs");
        RequireContains(recipeSampleRunModels, "public sealed class OpenVisionRecipeSampleRunSummary", "Recipe sample-run models have an explicit owner.");
        RequireContains(recipeSampleRunModels, "public sealed class OpenVisionRecipeBatchRunComparisonRow", "Recipe batch-comparison rows have an explicit owner.");

        RequireNotContains(recipeCommandSurface, "private string BuildOperatorRunReviewText()", "Recipe command surface does not derive operator run-review text.");
        RequireNotContains(recipeCommandSurface, "private string BuildSelectedPairRoleRunReviewSuffix()", "Recipe command surface does not derive selected pair-role review text.");
        RequireNotContains(recipeCommandSurface, "private string BuildSelectedRecentBatchRunReviewText()", "Recipe command surface does not derive selected batch-run review text.");
        RequireNotContains(recipeCommandSurface, "private static string BuildOperatorRunReviewNextAction(", "Recipe command surface does not own run-review next-action policy.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionXmlCardText()", "Recipe command surface does not format the decision-board XML card.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionSampleCardText()", "Recipe command surface does not format the decision-board sample card.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionPairCardText()", "Recipe command surface does not format the decision-board Good/Bad card.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionNextActionText()", "Recipe command surface does not format the decision-board next action.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionSummaryStatusText()", "Recipe command surface does not format the decision-board status summary.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorDecisionEvidenceText()", "Recipe command surface does not derive decision-board metric evidence.");
        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeOperatorValidationRow> BuildOperatorValidationChecklistRows()", "Recipe command surface does not derive the decision-board validation rows.");
        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeOperatorResultChannelRow> BuildOperatorResultChannelRows()", "Recipe command surface does not derive the decision-board result channels.");
        RequireNotContains(recipeCommandSurface, "private string BuildOperatorHandoffReportText()", "Recipe command surface does not format the operator handoff report.");

        string recipeRunReviewPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeRunReviewPresenter.cs");
        RequireContains(recipeRunReviewPresenter, "internal static class OpenVisionRecipeRunReviewPresenter", "Recipe run-review presentation has an explicit owner.");
        RequireContains(recipeRunReviewPresenter, "BuildOperatorRunReviewText", "Recipe run-review presenter formats operator review output.");
        RequireContains(recipeRunReviewPresenter, "BuildSelectedBatchRunReviewText", "Recipe run-review presenter formats saved batch-run review output.");
        RequireContains(recipeRunReviewPresenter, "BuildNextAction", "Recipe run-review presenter owns the next-action policy.");

        string recipeOperatorDecisionPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeOperatorDecisionPresenter.cs");
        RequireContains(recipeOperatorDecisionPresenter, "internal sealed class OpenVisionRecipeOperatorDecisionRequest", "Recipe operator-decision presentation has an explicit request contract.");
        RequireContains(recipeOperatorDecisionPresenter, "internal sealed class OpenVisionRecipeOperatorDecisionPresentation", "Recipe operator-decision presentation has an explicit result contract.");
        RequireContains(recipeOperatorDecisionPresenter, "internal static class OpenVisionRecipeOperatorDecisionPresenter", "Recipe operator-decision presentation has an explicit owner.");
        RequireContains(recipeOperatorDecisionPresenter, "BuildValidationRows", "Recipe operator-decision presenter owns validation-row presentation.");
        RequireContains(recipeOperatorDecisionPresenter, "BuildResultChannels", "Recipe operator-decision presenter owns result-channel presentation.");
        RequireContains(recipeOperatorDecisionPresenter, "BuildHandoffReportText", "Recipe operator-decision presenter owns operator handoff formatting.");

        RequireNotContains(recipeCommandSurface, "private string BuildPipelineDiffReport(", "Recipe command surface does not format pipeline diff reports.");
        RequireNotContains(recipeCommandSurface, "private static void AddLimitedDiffLines(", "Recipe command surface does not format limited pipeline-diff rows.");
        RequireNotContains(recipeCommandSurface, "private static string FormatDetailedStepDiff(", "Recipe command surface does not format detailed step diffs.");
        RequireNotContains(recipeCommandSurface, "private static string FormatParameterDiff(", "Recipe command surface does not format parameter diffs.");
        RequireNotContains(recipeCommandSurface, "private static int CountDependencyParameters(", "Recipe command surface does not count comparison dependency paths.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineComparisonPresenter.BuildDraftImportReview", "Recipe command surface delegates LLM draft review presentation.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineComparisonPresenter.BuildDraftDiffReview", "Recipe command surface delegates LLM draft diff presentation.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineComparisonPresenter.BuildVariantComparison", "Recipe command surface delegates active/selected pipeline comparison presentation.");

        string recipePipelineComparisonPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipePipelineComparisonPresenter.cs");
        RequireContains(recipePipelineComparisonPresenter, "internal static class OpenVisionRecipePipelineComparisonPresenter", "Recipe pipeline-comparison presentation has an explicit owner.");
        RequireContains(recipePipelineComparisonPresenter, "BuildDraftImportReview", "Recipe pipeline-comparison presenter formats LLM draft import review.");
        RequireContains(recipePipelineComparisonPresenter, "BuildDraftDiffReview", "Recipe pipeline-comparison presenter formats LLM draft diff review.");
        RequireContains(recipePipelineComparisonPresenter, "BuildVariantComparison", "Recipe pipeline-comparison presenter formats active/selected pipeline comparison.");
        RequireContains(recipePipelineComparisonPresenter, "BuildPipelineDiffReport", "Recipe pipeline-comparison presenter owns step and parameter diff formatting.");

        RequireNotContains(recipeCommandSurface, "private string BuildPipelineSelectedStepOperatorContextText()", "Recipe command surface does not format selected-step operator context.");
        RequireNotContains(recipeCommandSurface, "private string BuildFailureReviewText()", "Recipe command surface does not format failed-step review guidance.");
        RequireNotContains(recipeCommandSurface, "private string BuildCorrectedOutputReviewText()", "Recipe command surface does not format corrected-output guidance.");
        RequireNotContains(recipeCommandSurface, "private string BuildCorrectedOutputAppliedText(", "Recipe command surface does not format corrected-output apply evidence.");
        RequireNotContains(recipeCommandSurface, "private string BuildPipelineStepFlowReviewText()", "Recipe command surface does not format step-flow review.");
        RequireNotContains(recipeCommandSurface, "private string BuildBranchOutputComparisonText()", "Recipe command surface does not format branch/output comparison summary.");
        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeBranchOutputComparisonRow> BuildBranchOutputComparisonRows()", "Recipe command surface does not build branch/output comparison rows.");
        RequireNotContains(recipeCommandSurface, "private static string BuildPipelineStepSlotText(", "Recipe command surface does not format step-slot text.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineStepReviewPresenter.BuildOperatorContext", "Recipe command surface delegates selected-step operator context.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineStepReviewPresenter.BuildFailureReviewText", "Recipe command surface delegates failed-step review guidance.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputReviewText", "Recipe command surface delegates corrected-output guidance.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipePipelineStepReviewPresenter.BuildBranchOutputComparisonRows", "Recipe command surface delegates branch/output comparison rows.");

        string recipePipelineStepReviewPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipePipelineStepReviewPresenter.cs");
        RequireContains(recipePipelineStepReviewPresenter, "internal static class OpenVisionRecipePipelineStepReviewPresenter", "Recipe pipeline-step review presentation has an explicit owner.");
        RequireContains(recipePipelineStepReviewPresenter, "BuildOperatorContext", "Recipe pipeline-step review presenter formats selected-step context.");
        RequireContains(recipePipelineStepReviewPresenter, "BuildFailureReviewText", "Recipe pipeline-step review presenter formats failure review.");
        RequireContains(recipePipelineStepReviewPresenter, "BuildCorrectedOutputAppliedText", "Recipe pipeline-step review presenter formats corrected-output apply evidence.");
        RequireContains(recipePipelineStepReviewPresenter, "BuildStepFlowReview", "Recipe pipeline-step review presenter formats step flow.");
        RequireContains(recipePipelineStepReviewPresenter, "BuildBranchOutputComparisonRows", "Recipe pipeline-step review presenter owns branch/output rows.");

        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeBatchSampleResultOption> BuildFilteredRecentBatchRunSampleResults(", "Recipe command surface does not filter Run History rows directly.");
        RequireNotContains(recipeCommandSurface, "private string BuildRecentBatchRunNgFilterSummaryText()", "Recipe command surface does not format the Run History NG summary.");
        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeBatchRunComparisonRow> BuildRecentBatchRunComparisonRows()", "Recipe command surface does not build Run History comparison rows.");
        RequireNotContains(recipeCommandSurface, "private OpenVisionRecipeBatchRunOption FindBaselineBatchRunOption(", "Recipe command surface does not resolve the Run History baseline policy.");
        RequireNotContains(recipeCommandSurface, "private OpenVisionRecipeBatchRunOption FindAutoBaselineBatchRunOption(", "Recipe command surface does not resolve automatic Run History baselines.");
        RequireNotContains(recipeCommandSurface, "private static Dictionary<string, VisionPipelineBatchSampleRunResult> BuildBatchResultMap(", "Recipe command surface does not map Run History sample results.");
        RequireNotContains(recipeCommandSurface, "private static OpenVisionRecipeBatchRunComparisonRow SelectDefaultBatchComparisonRow(", "Recipe command surface does not select Run History comparison presentation.");
        RequireNotContains(recipeCommandSurface, "private string BuildRecentBatchRunComparisonSummaryText()", "Recipe command surface does not format the Run History comparison summary.");
        RequireNotContains(recipeCommandSurface, "private string BuildRecentBatchRunPerformanceComparisonText()", "Recipe command surface does not format Run History timing comparison.");
        RequireNotContains(recipeCommandSurface, "private static bool HaveEquivalentBatchSampleSets(", "Recipe command surface does not compare Run History sample sets.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeRunHistoryPresenter.BuildFilteredSampleResults", "Recipe command surface delegates Run History filtering.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeRunHistoryPresenter.BuildComparisonRows", "Recipe command surface delegates Run History comparison rows.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeRunHistoryPresenter.BuildComparisonSummaryText", "Recipe command surface delegates Run History comparison summary.");

        string recipeRunHistoryPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeRunHistoryPresenter.cs");
        RequireContains(recipeRunHistoryPresenter, "internal static class OpenVisionRecipeRunHistoryPresenter", "Recipe Run History presentation has an explicit owner.");
        RequireContains(recipeRunHistoryPresenter, "BuildFilteredSampleResults", "Recipe Run History presenter filters saved-run samples.");
        RequireContains(recipeRunHistoryPresenter, "ResolveBaselineRunOption", "Recipe Run History presenter owns baseline selection policy.");
        RequireContains(recipeRunHistoryPresenter, "BuildComparisonRows", "Recipe Run History presenter derives comparison rows.");
        RequireContains(recipeRunHistoryPresenter, "BuildComparisonSummaryText", "Recipe Run History presenter formats comparison and performance evidence.");
        RequireNotContains(recipeRunHistoryPresenter, "VisionPipelineBatchRunSummaryStorage.Load(", "Recipe Run History presenter does not load persisted run summaries.");

        RequireNotContains(recipeCommandSurface, "private IReadOnlyList<OpenVisionRecipeSampleMatrixRow> BuildSampleMatrixRows()", "Recipe command surface does not build sample-matrix rows directly.");
        RequireNotContains(recipeCommandSurface, "private static OpenVisionRecipeSampleMatrixRow SelectDefaultSampleMatrixRow(", "Recipe command surface does not select sample-matrix presentation directly.");
        RequireNotContains(recipeCommandSurface, "private string BuildSampleMatrixSummaryText()", "Recipe command surface does not format the sample-matrix summary.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeSampleMatrixPresenter.BuildRows", "Recipe command surface delegates sample-matrix row construction.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeSampleMatrixPresenter.SelectDefaultRow", "Recipe command surface delegates sample-matrix selection priority.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeSampleMatrixPresenter.BuildSummaryText", "Recipe command surface delegates sample-matrix summary presentation.");

        string recipeSampleMatrixPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeSampleMatrixPresenter.cs");
        RequireContains(recipeSampleMatrixPresenter, "internal static class OpenVisionRecipeSampleMatrixPresenter", "Recipe sample-matrix presentation has an explicit owner.");
        RequireContains(recipeSampleMatrixPresenter, "BuildRows", "Recipe sample-matrix presenter derives Good/Bad rows.");
        RequireContains(recipeSampleMatrixPresenter, "SelectDefaultRow", "Recipe sample-matrix presenter owns selected-row priority.");
        RequireContains(recipeSampleMatrixPresenter, "BuildSummaryText", "Recipe sample-matrix presenter formats matrix summary.");
        RequireNotContains(recipeSampleMatrixPresenter, "VisionPipelineExecutionService.RunAsync", "Recipe sample-matrix presenter does not execute a pipeline.");

        RequireNotContains(recipeCommandSurface, "private string BuildValidationSetExpectedText()", "Recipe command surface does not format validation-set expected-role summary.");
        RequireNotContains(recipeCommandSurface, "private string BuildValidationSetNextActionText()", "Recipe command surface does not format validation-set next action.");
        RequireNotContains(recipeCommandSurface, "private string BuildValidationSuiteSummaryText()", "Recipe command surface does not format Validation Suite summary.");
        RequireNotContains(recipeCommandSurface, "private string BuildValidationSetSelectionSummaryText()", "Recipe command surface does not format validation-set selection summary.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeValidationSetPresenter.BuildExpectedText", "Recipe command surface delegates validation-set expected-role presentation.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeValidationSetPresenter.BuildNextActionText", "Recipe command surface delegates validation-set next action presentation.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeValidationSetPresenter.BuildSelectionSummaryText", "Recipe command surface delegates validation-set selection summary.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeValidationSetPresenter.BuildValidationSuiteSummaryText", "Recipe command surface delegates Validation Suite summary presentation.");

        string recipeValidationSetPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeValidationSetPresenter.cs");
        RequireContains(recipeValidationSetPresenter, "internal static class OpenVisionRecipeValidationSetPresenter", "Recipe validation-set presentation has an explicit owner.");
        RequireContains(recipeValidationSetPresenter, "BuildExpectedText", "Recipe validation-set presenter formats expected roles.");
        RequireContains(recipeValidationSetPresenter, "BuildNextActionText", "Recipe validation-set presenter formats next action.");
        RequireContains(recipeValidationSetPresenter, "BuildSelectionSummaryText", "Recipe validation-set presenter formats selected-set summary.");
        RequireContains(recipeValidationSetPresenter, "BuildValidationSuiteSummaryText", "Recipe validation-set presenter formats Validation Suite summary.");
        RequireNotContains(recipeValidationSetPresenter, "OpenVisionRecipeValidationSetStorage.", "Recipe validation-set presenter does not persist validation sets.");
        RequireNotContains(recipeValidationSetPresenter, "VisionPipelineStorage.TryLoadFromFile(", "Recipe validation-set presenter does not load pipeline XML.");
        RequireNotContains(recipeValidationSetPresenter, "VisionPipelineExecutionService.RunAsync", "Recipe validation-set presenter does not execute a pipeline.");

        RequireNotContains(recipeCommandSurface, "private string BuildPinGapIntentLatestRunText()", "Recipe command surface does not format Pin gap latest-run feedback.");
        RequireNotContains(recipeCommandSurface, "private string BuildPinGapIntentCalibrationReviewText()", "Recipe command surface does not format Pin gap calibration feedback.");
        RequireNotContains(recipeCommandSurface, "private string ResolvePinGapMetricAdvice(string metrics)", "Recipe command surface does not resolve Pin gap metric advice.");
        RequireNotContains(recipeCommandSurface, "private string BuildBlobCountIntentLatestRunText()", "Recipe command surface does not format Blob latest-run feedback.");
        RequireNotContains(recipeCommandSurface, "private string ResolveBlobCountMetricAdvice(double count)", "Recipe command surface does not resolve Blob metric advice.");
        RequireNotContains(recipeCommandSurface, "private string BuildContourCountIntentLatestRunText()", "Recipe command surface does not format Contour latest-run feedback.");
        RequireNotContains(recipeCommandSurface, "private string ResolveContourCountMetricAdvice(double count, bool hasCount, double areaMax, bool hasAreaMax)", "Recipe command surface does not resolve Contour metric advice.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeIntentFeedbackPresenter.BuildPinGapLatestRunText", "Recipe command surface delegates Pin gap latest-run feedback.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeIntentFeedbackPresenter.BuildPinGapCalibrationReviewText", "Recipe command surface delegates Pin gap calibration feedback.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeIntentFeedbackPresenter.BuildBlobCountLatestRunText", "Recipe command surface delegates Blob latest-run feedback.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeIntentFeedbackPresenter.BuildContourCountLatestRunText", "Recipe command surface delegates Contour latest-run feedback.");

        string recipeIntentFeedbackPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeIntentFeedbackPresenter.cs");
        RequireContains(recipeIntentFeedbackPresenter, "internal static class OpenVisionRecipeIntentFeedbackPresenter", "Recipe intent feedback presentation has an explicit owner.");
        RequireContains(recipeIntentFeedbackPresenter, "BuildPinGapLatestRunText", "Recipe intent feedback presenter formats Pin gap latest runs.");
        RequireContains(recipeIntentFeedbackPresenter, "BuildPinGapCalibrationReviewText", "Recipe intent feedback presenter formats Pin gap calibration review.");
        RequireContains(recipeIntentFeedbackPresenter, "BuildBlobCountLatestRunText", "Recipe intent feedback presenter formats Blob latest runs.");
        RequireContains(recipeIntentFeedbackPresenter, "BuildContourCountLatestRunText", "Recipe intent feedback presenter formats Contour latest runs.");
        RequireNotContains(recipeIntentFeedbackPresenter, "VisionPipelineSampleCheckService.Run", "Recipe intent feedback presenter does not run sample checks.");
        RequireNotContains(recipeIntentFeedbackPresenter, "VisionPipelineStorage.TryLoadFromFile(", "Recipe intent feedback presenter does not load pipeline XML.");
        RequireNotContains(recipeIntentFeedbackPresenter, "VisionPipelineBatchRunSummaryStorage.Save(", "Recipe intent feedback presenter does not persist run history.");

        RequireNotContains(recipeCommandSurface, "private string BuildGuidedSetupReadinessText()", "Recipe command surface does not format Guided Setup readiness text.");
        RequireNotContains(recipeCommandSurface, "private bool TryBuildGuidedSetupIntentInputStatus(out string status)", "Recipe command surface does not evaluate Guided Setup required inputs.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedSetupReadinessPresenter.BuildReadinessText", "Recipe command surface delegates Guided Setup readiness text.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedSetupReadinessPresenter.Evaluate", "Recipe command surface delegates Guided Setup required-input evaluation.");

        string recipeGuidedSetupReadinessPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeGuidedSetupReadinessPresenter.cs");
        RequireContains(recipeGuidedSetupReadinessPresenter, "internal static class OpenVisionRecipeGuidedSetupReadinessPresenter", "Guided Setup readiness presentation has an explicit IntentSkills owner.");
        RequireContains(recipeGuidedSetupReadinessPresenter, "BuildReadinessText", "Guided Setup readiness presenter formats required-input guidance.");
        RequireContains(recipeGuidedSetupReadinessPresenter, "Evaluate(OpenVisionRecipeGuidedSetupReadinessInput input)", "Guided Setup readiness presenter evaluates the current input DTO.");
        RequireContains(recipeGuidedSetupReadinessPresenter, "OpenVisionRecipeGuidedSetupReadinessInput", "Guided Setup readiness presenter has an explicit read-only input contract.");
        RequireNotContains(recipeGuidedSetupReadinessPresenter, "VisionPipelineSampleCheckService.Run", "Guided Setup readiness presenter does not run sample checks.");
        RequireNotContains(recipeGuidedSetupReadinessPresenter, "VisionPipelineStorage.TryLoadFromFile(", "Guided Setup readiness presenter does not load pipeline XML.");
        RequireNotContains(recipeGuidedSetupReadinessPresenter, "VisionPipelineBatchRunSummaryStorage.Save(", "Guided Setup readiness presenter does not persist run history.");
        RequireNotContains(recipeGuidedSetupReadinessPresenter, "CreatePipeline(", "Guided Setup readiness presenter does not create starter XML pipelines.");

        RequireNotContains(recipeCommandSurface, "private string BuildRecipeGuidedSetupText()", "Recipe command surface does not format the Guided workflow strip.");
        RequireNotContains(recipeCommandSurface, "private string BuildRecipeGuidedNextActionText()", "Recipe command surface does not duplicate Guided workflow action labels.");
        RequireNotContains(recipeCommandSurface, "private Action ResolveRecipeGuidedNextAction()", "Recipe command surface does not duplicate Guided workflow action selection.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedWorkflowPresenter.BuildSetupText", "Recipe command surface delegates Guided workflow strip presentation.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedWorkflowPresenter.BuildNextActionText", "Recipe command surface delegates Guided workflow action labels.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedWorkflowPresenter.ResolveNextAction", "Recipe command surface delegates Guided workflow action selection.");

        string recipeGuidedWorkflowPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeGuidedWorkflowPresenter.cs");
        RequireContains(recipeGuidedWorkflowPresenter, "internal static class OpenVisionRecipeGuidedWorkflowPresenter", "Guided workflow presentation has an explicit Review owner.");
        RequireContains(recipeGuidedWorkflowPresenter, "BuildSetupText", "Guided workflow presenter formats the setup strip.");
        RequireContains(recipeGuidedWorkflowPresenter, "ResolveNextAction", "Guided workflow presenter owns ordered next-action selection.");
        RequireContains(recipeGuidedWorkflowPresenter, "OpenVisionRecipeGuidedWorkflowActionRequest", "Guided workflow presenter has an explicit availability request contract.");
        RequireNotContains(recipeGuidedWorkflowPresenter, "VisionPipelineExecutionService.RunAsync", "Guided workflow presenter does not execute a pipeline.");
        RequireNotContains(recipeGuidedWorkflowPresenter, "VisionPipelineStorage.TryLoadFromFile(", "Guided workflow presenter does not load pipeline XML.");
        RequireNotContains(recipeGuidedWorkflowPresenter, "CreatePipeline(", "Guided workflow presenter does not create starter XML pipelines.");
        RequireNotContains(recipeGuidedWorkflowPresenter, "System.Windows.Clipboard", "Guided workflow presenter does not access WPF clipboard state.");

        RequireNotContains(recipeCommandSurface, "private string BuildRecipeEditValidationText()", "Recipe command surface does not format recipe lifecycle validation text.");
        RequireNotContains(recipeCommandSurface, "private string BuildPipelineEditValidationText()", "Recipe command surface does not format pipeline lifecycle validation text.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeLifecycleValidationPresenter.BuildRecipeEditValidationText", "Recipe command surface delegates recipe lifecycle validation text.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeLifecycleValidationPresenter.BuildPipelineEditValidationText", "Recipe command surface delegates pipeline lifecycle validation text.");

        string recipeLifecycleValidationPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeLifecycleValidationPresenter.cs");
        RequireContains(recipeLifecycleValidationPresenter, "internal static class OpenVisionRecipeLifecycleValidationPresenter", "Recipe lifecycle validation presentation has an explicit Review owner.");
        RequireContains(recipeLifecycleValidationPresenter, "OpenVisionRecipeEditValidationRequest", "Recipe lifecycle validation presenter has a recipe edit request contract.");
        RequireContains(recipeLifecycleValidationPresenter, "OpenVisionRecipePipelineEditValidationRequest", "Recipe lifecycle validation presenter has a pipeline edit request contract.");
        RequireNotContains(recipeLifecycleValidationPresenter, "RecipeWorkspaceService.EnsureVisionWorkspace(", "Recipe lifecycle validation presenter does not create workspaces.");
        RequireNotContains(recipeLifecycleValidationPresenter, "RecipeWorkspaceService.DuplicateVisionWorkspace(", "Recipe lifecycle validation presenter does not duplicate workspaces.");
        RequireNotContains(recipeLifecycleValidationPresenter, "RecipeWorkspaceService.RenameVisionWorkspace(", "Recipe lifecycle validation presenter does not rename workspaces.");
        RequireNotContains(recipeLifecycleValidationPresenter, "RecipeWorkspaceService.DeleteVisionWorkspace(", "Recipe lifecycle validation presenter does not delete workspaces.");
        RequireNotContains(recipeLifecycleValidationPresenter, "VisionPipelineStorage.", "Recipe lifecycle validation presenter does not access pipeline storage.");
        RequireNotContains(recipeLifecycleValidationPresenter, "VisionPipelineExecutionService.RunAsync", "Recipe lifecycle validation presenter does not execute a pipeline.");

        RequireNotContains(recipeCommandSurface, "private static string BuildLlmXmlValidationReport(", "Recipe command surface does not format stored-pipeline XML validation evidence.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeStoredPipelineValidationReportBuilder.Build", "Recipe command surface delegates stored-pipeline XML validation evidence.");

        string storedPipelineValidationReportBuilder = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Validation\OpenVisionRecipeStoredPipelineValidationReportBuilder.cs");
        RequireContains(storedPipelineValidationReportBuilder, "internal static class OpenVisionRecipeStoredPipelineValidationReportBuilder", "Stored-pipeline XML validation evidence has an explicit Validation owner.");
        RequireContains(storedPipelineValidationReportBuilder, "OpenVisionRecipeStoredPipelineValidationReportRequest", "Stored-pipeline XML validation builder has an explicit request contract.");
        RequireContains(storedPipelineValidationReportBuilder, "VisionPipelineValidator.Validate", "Stored-pipeline XML validation builder preserves schema/routing validation.");
        RequireNotContains(storedPipelineValidationReportBuilder, "VisionPipelineStorage.", "Stored-pipeline XML validation builder does not load or save pipeline storage.");
        RequireNotContains(storedPipelineValidationReportBuilder, "VisionPipelineExecutionService.RunAsync", "Stored-pipeline XML validation builder does not execute a pipeline.");
        RequireNotContains(storedPipelineValidationReportBuilder, "System.Windows.", "Stored-pipeline XML validation builder does not access WPF state.");
        RequireContains(recipeCommandSurface, "다음: 파이프라인 열기", "Recipe Manager labels the summary primary action as the next step.");
        RequireContains(
            Read(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostView.xaml"),
            "AutomationProperties.Name=\"{Binding RecipeCommands.OpenPipelineReviewText}\"",
            "Recipe Manager exposes the summary primary action through its localized accessible name.");

        string pipelineReviewReadinessPresenter = Read(repoRoot, @"UI\Menu\Wpf\PipelineReview\Presenters\OpenVisionPipelineReviewReadinessPresenter.cs");
        RequireContains(pipelineReviewReadinessPresenter, "OpenVisionPipelineReviewReadinessPresenter", "Pipeline Review readiness presentation has an explicit owner.");

        string pipelineReviewDocument = Read(repoRoot, @"UI\Menu\Wpf\Documents\OpenVisionPipelineReviewDocument.cs");
        RequireNotContains(pipelineReviewDocument, "VisionPipelineExecutionService.RunAsync", "Pipeline Review document does not execute the pipeline directly.");
        RequireNotContains(pipelineReviewDocument, "reviewLayerImages", "Pipeline Review document does not own review output-image caches.");
        RequireNotContains(pipelineReviewDocument, "stepResultSummaries", "Pipeline Review document does not own Step result caches.");
        RequireNotContains(pipelineReviewDocument, "CreateReviewContextFromDisplayLayers", "Pipeline Review document does not build the execution context directly.");

        string pipelineReviewExecutionController = Read(repoRoot, @"UI\Menu\Wpf\PipelineReview\Execution\OpenVisionPipelineReviewExecutionController.cs");
        RequireContains(pipelineReviewExecutionController, "internal sealed class OpenVisionPipelineReviewExecutionController", "Pipeline Review execution has an explicit controller owner.");
        RequireContains(pipelineReviewExecutionController, "VisionPipelineExecutionService.RunAsync", "Pipeline Review execution controller invokes the shared pipeline runner.");
        RequireContains(pipelineReviewExecutionController, "CreateReviewContextFromDisplayLayers", "Pipeline Review execution controller builds the display-layer execution context.");
        RequireContains(pipelineReviewExecutionController, "CacheReviewOutput", "Pipeline Review execution controller owns review output-image caches.");
        RequireContains(pipelineReviewExecutionController, "DisposeRunResultImages", "Pipeline Review execution controller owns result-image disposal.");

        string pipelineReviewExecutionResult = Read(repoRoot, @"UI\Menu\Wpf\PipelineReview\Execution\OpenVisionPipelineReviewExecutionResult.cs");
        RequireContains(pipelineReviewExecutionResult, "OpenVisionPipelineReviewExecutionResult", "Pipeline Review execution returns an explicit result contract.");
        RequireContains(pipelineReviewExecutionResult, "OpenVisionPipelineReviewStepUpdatedEventArgs", "Pipeline Review execution exposes Step update events through an explicit contract.");

        string edgeBasedMatchingIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeEdgeBasedMatchingIntentSkill.cs");
        RequireContains(edgeBasedMatchingIntentSkill, "ToolType = \"EdgeBasedMatching\"", "EdgeBasedMatching intent starter has an explicit owner.");

        string featureMatchingIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeFeatureMatchingIntentSkill.cs");
        RequireContains(featureMatchingIntentSkill, "ToolType = \"FeatureMatching\"", "FeatureMatching intent starter has an explicit owner.");

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
        string diagnostic = Read(repoRoot, @"Core\Pipeline\Validation\VisionPipelineStepDiagnosticService.cs");
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

        string sampleCatalogUi = Read(repoRoot, @"Core\Pipeline\Storage\VisionPipelineSampleCatalog.cs");
        RequireContains(sampleCatalogUi, "VisionPipelineSampleCatalogSourceKind.Public", "Sample catalog loader exposes public catalog source.");
        RequireContains(sampleCatalogUi, "VisionPipelineSampleCatalogSourceKind.Product", "Sample catalog loader exposes product-domain catalog source.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.PublicSampleCatalog.csv", "Sample catalog loader reads the public catalog.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.ProductSampleCatalog.csv", "Sample catalog loader reads the product-domain catalog.");
        RequireContains(sampleCatalogUi, "OpenVisionLab.SampleCatalog.csv", "Sample catalog loader keeps the local legacy catalog.");
        RequireContains(sampleCatalogUi, "FixGuideText", "Sample catalog UX exposes fix guidance.");
        RequireContains(sampleCatalogUi, "ExpectedReasonText", "Sample catalog UX explains why expected metrics matter.");
        RequireContains(sampleCatalogUi, "Expected outcome: no result / controlled NG", "Sample catalog UX explains expected-failure samples.");

        string samplePickerViewModel = Read(repoRoot, @"UI\Menu\Wpf\Workspace\Samples\OpenVisionWorkspaceSamplePickerViewModel.cs");
        RequireContains(samplePickerViewModel, "CatalogSourceOptions", "Workspace sample picker exposes catalog source options.");
        RequireContains(samplePickerViewModel, "ActiveRouteSummaryText", "Workspace sample picker exposes active route summary text.");
        RequireContains(samplePickerViewModel, "SelectedCatalogSourceOption", "Workspace sample picker can switch between catalog sources.");
        RequireContains(samplePickerViewModel, "RebuildLearnPathOptions", "Workspace sample picker rebuilds learn paths per selected catalog source.");
        RequireContains(samplePickerViewModel, "OpenLearnDocumentCommand", "Workspace sample picker exposes a non-executing Learn document command.");
        RequireContains(samplePickerViewModel, "CanOpenLearnAndSample", "Workspace sample picker exposes an explicit guide-plus-sample action state.");
        RequireContains(samplePickerViewModel, "PracticeWorkflowText", "Workspace sample picker exposes the Learn practice workflow text.");
        RequireContains(samplePickerViewModel, "Tool View 또는 Pipeline Review", "Workspace sample picker points the operator to Tool View or Pipeline Review after sample open.");
        RequireContains(samplePickerViewModel, "Preview/Run Review를 직접 클릭", "Workspace sample picker keeps Learn sample verification explicit.");
        RequireContains(samplePickerViewModel, "Selected: {0} / {1} - {2}", "Workspace sample picker Learn path summary names the selected path and sample count.");
        RequireContains(samplePickerViewModel, "SampleCountText", "Workspace sample picker Learn path summary includes the selected path sample count.");

        string learnDocumentService = Read(repoRoot, @"UI\Menu\Wpf\Workspace\Samples\OpenVisionWorkspaceLearnDocumentService.cs");
        RequireContains(learnDocumentService, "docs", "Workspace sample picker resolves Learn documents from the repository docs folder.");
        RequireContains(learnDocumentService, "LEARN_PRODUCT_SAMPLES.md", "Workspace Learn document resolver links product-domain samples.");
        RequireContains(learnDocumentService, "LEARN_MATCHING.md", "Workspace Learn document resolver links Matching samples.");
        RequireContains(learnDocumentService, "LEARN_EDGE_BASED_MATCHING.md", "Workspace Learn document resolver links EdgeBasedMatching samples.");
        RequireContains(learnDocumentService, "LEARN_GEOMETRY_TRANSFORM.md", "Workspace Learn document resolver links Geometry samples.");
        string sampleLearnPathOption = Read(repoRoot, @"UI\Menu\Wpf\Workspace\Samples\OpenVisionWorkspaceSampleLearnPathOption.cs");
        RequireContains(sampleLearnPathOption, "\"geometry\"", "Workspace sample picker exposes the Geometry Learn path.");
        RequireContains(sampleLearnPathOption, "RotateScale", "Workspace sample picker classifies RotateScale samples as Geometry.");
        RequireContains(sampleLearnPathOption, "\"color-hsv\"", "Workspace sample picker exposes the Color / HSV Learn path.");
        RequireContains(sampleLearnPathOption, "\"HSV\", \"Color range\"", "Workspace sample picker classifies HSV samples as Color / HSV.");

        string samplePickerView = Read(repoRoot, @"UI\Menu\Wpf\Workspace\Samples\OpenVisionWorkspaceSamplePickerView.xaml");
        RequireContains(samplePickerView, "WorkspaceSamplePickerCatalogSourceList", "Workspace sample picker renders a catalog source selector.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerRouteSummary", "Workspace sample picker renders the active catalog/focus/Learn route summary.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerCatalogSourceSummary", "Workspace sample picker renders selected catalog source guidance.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerOpenLearnDocumentButton", "Workspace sample picker renders a Learn document open button.");
        RequireContains(samplePickerView, "WorkspaceSamplePickerPracticeWorkflowStrip", "Workspace sample picker renders the Learn practice workflow strip.");

        string samplePickerWindow = Read(repoRoot, @"UI\Menu\Wpf\Workspace\Samples\OpenVisionWorkspaceSamplePickerWindow.xaml");
        RequireContains(samplePickerWindow, "WorkspaceSamplePickerOpenGuideAndSampleButton", "Workspace sample picker renders an explicit guide-plus-sample button.");
        string shellSampleWorkflowPresenter = Read(repoRoot, @"UI\Menu\Wpf\Shell\Workspace\OpenVisionShellHostSampleWorkflowPresenter.cs");
        RequireContains(shellSampleWorkflowPresenter, "Pipeline 보기 -> Run Review", "Shell sample workflow overlay points from Pipeline view to explicit Run Review.");
        RequireContains(shellSampleWorkflowPresenter, "기준 열고 Run Review", "Shell sample workflow pair comparison tells operators to run review explicitly.");
        string localizationCatalog = Read(repoRoot, @"Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv");
        RequireContains(localizationCatalog, "Shell.WorkspaceStatus.SampleRoute\tPipeline Review 열기 -> Run Review 또는 첫 Step 열기", "Shell top sample route includes explicit Run Review.");

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
        RequireContains(publicCatalog, "DistanceMmRange,0.08,0.11", "Public LineDistance Bad benchmark records the consistency-gate failure range.");
        RequireContains(publicCatalog, "Public_Geometry_RotateScale_Good", "Public sample catalog includes RotateScale geometry benchmark.");
        RequireContains(publicCatalog, "Public_Geometry_RotateScale_Wide_Bad", "Public sample catalog includes RotateScale geometry Bad benchmark.");
        RequireContains(publicCatalog, "ResultImageWidth;ResultImageHeight", "Public RotateScale benchmark checks output width and height.");
        RequireContains(publicCatalog, "Public_AffineTransform_Synthetic_Good", "Public sample catalog includes the AffineTransform synthetic benchmark.");
        RequireContains(publicCatalog, "AffineM11;AffineM12;AffineM13;AffineM21;AffineM22;AffineM23;AffineValidPixelRatio", "Public AffineTransform benchmark checks the matrix and retained-source coverage.");
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
        RequireContains(publicCatalog, @"docs\samples\public\Public_AffineTransform_Synthetic.pipeline.xml", "Public sample catalog points AffineTransform to a public pipeline.");
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
            @"docs\samples\public\Public_AffineTransform_Synthetic.pipeline.xml",
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

        string publicLinePipeline = Read(repoRoot, @"docs\samples\public\Public_Line_Pins_Distance.pipeline.xml");
        RequireContains(publicLinePipeline, "<AcceptanceMetricName>DistanceMmRange</AcceptanceMetricName>", "Public LineDistance pipeline gates sampling-line consistency.");
        RequireContains(publicLinePipeline, "<AcceptanceMetricMaximum>0.03</AcceptanceMetricMaximum>", "Public LineDistance pipeline sets the range gate to 0.03 mm.");
        RequireContains(publicLinePipeline, "<Key>ALLOW_BRANCH_INPUT</Key>", "Public LineDistance average step explicitly reads Main as a second branch.");

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

    private static void CheckSourceOwnership(string repoRoot)
    {
        RequireExactDirectSourceFiles(repoRoot, @"Core", "Core root must not retain unowned source files.");
        RequireExactDirectSourceFiles(repoRoot, @"UI\VisionTest", "Vision Test root must not retain unowned source files.");
        RequireExactDirectSourceFiles(repoRoot, @"UI\VisionTest\Wpf", "Vision Test WPF root must not retain unowned source files.");
        RequireExactDirectSourceFiles(
            repoRoot,
            @"UI\Menu\Wpf",
            "MENU WPF root retains only the explicit Shell composition boundary.",
            "OpenVisionShellHostRecipeCommandSurface.cs",
            "OpenVisionShellHostRecipeCommandSurface.Commands.cs",
            "OpenVisionShellHostRecipeCommandSurface.Handlers.cs",
            "OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs",
            "OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs",
            "OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs",
            "OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs",
            "OpenVisionShellHostRecipeCommandSurface.RunHistory.cs",
            "OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs",
            "OpenVisionShellHostView.xaml",
            "OpenVisionShellHostView.xaml.cs",
            "OpenVisionShellHostView.Interactions.cs");

        string recipeCommandSurface = ReadSourceFamily(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.cs");
        RequireNotContains(recipeCommandSurface, "internal static class OpenVisionGuidedSetupCatalog", "Recipe command surface does not own the guided-setup catalog declaration.");
        RequireNotContains(recipeCommandSurface, "internal static class OpenVisionRecipeText", "Recipe command surface does not own recipe text localization.");
        RequireNotContains(recipeCommandSurface, "private string BuildLlmIntentSpecificPromptPacketText", "Recipe command surface does not own LLM prompt packet construction.");
        RequireNotContains(recipeCommandSurface, "private static string BuildLlmIntentContractText", "Recipe command surface does not own LLM intent contract text.");
        RequireNotContains(recipeCommandSurface, "private static bool AppendLlmResultChannelValidation", "Recipe command surface does not own LLM result-channel validation rules.");
        RequireNotContains(recipeCommandSurface, "private bool AppendLlmIntentContractValidation", "Recipe command surface does not own LLM intent validation rules.");
        RequireNotContains(recipeCommandSurface, "private static bool TryValidateXmlSyntax", "Recipe command surface does not own XML syntax validation rules.");
        RequireNotContains(recipeCommandSurface, "private string BuildDependencyReport", "Recipe command surface does not own LLM dependency-review execution.");
        RequireNotContains(recipeCommandSurface, "internal static bool LooksLikeDependencyPath", "Recipe command surface does not own dependency path classification.");
        RequireNotContains(recipeCommandSurface, "internal static string ResolveDependencySourcePath", "Recipe command surface does not own dependency path resolution.");
        RequireNotContains(recipeCommandSurface, "private void CopyReferenceImageForDraftImport", "Recipe command surface does not own dependency file-copy execution.");
        RequireNotContains(recipeCommandSurface, "SerializeHelper.TryLoadFromXmlText", "Recipe command surface does not deserialize LLM XML drafts directly.");
        RequireNotContains(recipeCommandSurface, "OpenVisionLab LLM XML review bundle", "Recipe command surface does not own correction-packet text construction.");

        string guidedSetupCatalog = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeGuidedSetupCatalog.cs");
        RequireContains(guidedSetupCatalog, "internal static class OpenVisionGuidedSetupCatalog", "Guided setup catalog has an IntentSkills owner.");
        RequireContains(guidedSetupCatalog, "TryResolveTemplate", "Guided setup catalog keeps deterministic tool-to-template mapping.");
        RequireContains(guidedSetupCatalog, "internal const string PinArrayGapTemplate = \"Pin row edge-gap consistency (PinArrayGap)\"", "Guided setup catalog exposes the exact PinArrayGap pilot template.");

        string pinArrayGapIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipePinArrayGapIntentSkill.cs");
        RequireContains(pinArrayGapIntentSkill, "internal static class OpenVisionRecipePinArrayGapIntentSkill", "PinArrayGap intent skill has an explicit IntentSkills owner.");
        RequireContains(pinArrayGapIntentSkill, "SupportedMeasurementDefinition = \"Adjacent edge-to-edge clearance\"", "PinArrayGap intent skill locks the supported edge-gap measurement.");
        RequireContains(pinArrayGapIntentSkill, "SupportedPinPolarity = \"Dark\"", "PinArrayGap intent skill locks the v1 dark-pin polarity.");
        RequireContains(pinArrayGapIntentSkill, "SupportedUnitMode = \"px\"", "PinArrayGap intent skill locks the v1 pixel-only unit mode.");
        RequireContains(pinArrayGapIntentSkill, "CreateMeasurementPipeline", "PinArrayGap intent skill exposes a measurement-only starter path.");
        RequireContains(pinArrayGapIntentSkill, "CreateJudgedPipeline", "PinArrayGap intent skill exposes an explicitly judged starter path.");
        RequireContains(pinArrayGapIntentSkill, "ToolType = \"PinArrayGap\"", "PinArrayGap intent skill locks every generated row Step to PinArrayGap.");
        RequireContains(pinArrayGapIntentSkill, "AcceptanceMetricName = VisionPipelineKnownMetrics.DistancePxRange", "PinArrayGap judged starter uses the DistancePxRange consistency gate.");

        string recipeText = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Models\OpenVisionRecipeText.cs");
        RequireContains(recipeText, "internal static class OpenVisionRecipeText", "Recipe text localization has a Models owner.");
        RequireContains(recipeText, "OpenVisionLanguageService.CurrentLanguage", "Recipe text localization keeps the current-language decision.");

        string llmPromptBuilder = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeLlmPromptBuilder.cs");
        RequireContains(llmPromptBuilder, "internal static class OpenVisionRecipeLlmPromptBuilder", "LLM prompt construction has an IntentSkills owner.");
        RequireContains(llmPromptBuilder, "internal sealed class OpenVisionRecipeLlmPromptRequest", "LLM prompt construction receives an explicit host-state request.");
        RequireContains(llmPromptBuilder, "Do not run Preview/Run automatically.", "LLM prompt construction preserves the explicit Preview/Run contract.");
        RequireContains(llmPromptBuilder, "internal static class OpenVisionRecipeLlmIntent", "LLM tool-family contracts have an IntentSkills owner.");
        RequireContains(llmPromptBuilder, "Use only ToolType=PinArrayGap. Do not substitute LineDistance, Contour, Blob, matching, or bounding-box measurements.", "PinArrayGap prompt packet locks the tool family.");
        RequireContains(llmPromptBuilder, "Supported measurement: ", "PinArrayGap prompt packet states the locked edge-gap definition.");
        RequireContains(llmPromptBuilder, "Do not claim center-to-center pitch.", "PinArrayGap prompt packet blocks unsupported center-pitch claims.");
        RequireContains(llmPromptBuilder, "Do not generate a bright-pin recipe.", "PinArrayGap prompt packet blocks unsupported bright-pin claims.");
        RequireContains(llmPromptBuilder, "Do not add PIXELPERMM or claim physical units.", "PinArrayGap prompt packet keeps v1 output pixel-only.");

        string llmReviewBundleBuilder = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeLlmReviewBundleBuilder.cs");
        RequireContains(llmReviewBundleBuilder, "internal static class OpenVisionRecipeLlmReviewBundleBuilder", "LLM correction-packet construction has an IntentSkills owner.");
        RequireContains(llmReviewBundleBuilder, "internal sealed class OpenVisionRecipeLlmReviewBundleRequest", "LLM correction-packet construction receives an explicit request.");
        RequireContains(llmReviewBundleBuilder, "OpenVisionLab LLM XML review bundle", "LLM correction-packet construction preserves the correction bundle format.");
        RequireContains(llmReviewBundleBuilder, "OpenVisionRecipeLlmIntent.BuildLlmIntentContractText", "LLM correction-packet construction uses the shared intent contract.");

        string llmDraftValidationRules = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Validation\OpenVisionRecipeLlmDraftValidationRules.cs");
        RequireContains(llmDraftValidationRules, "internal static class OpenVisionRecipeLlmDraftValidationRules", "LLM draft validation rules have a Recipe Validation owner.");
        RequireContains(llmDraftValidationRules, "internal static bool AppendResultChannelValidation", "LLM draft validation owns result-channel checks.");
        RequireContains(llmDraftValidationRules, "internal static bool AppendIntentContractValidation", "LLM draft validation owns intent-contract checks.");
        RequireContains(llmDraftValidationRules, "internal static bool TryValidateXmlSyntax", "LLM draft validation owns XML syntax checks.");
        RequireContains(llmDraftValidationRules, "AppendPinArrayGapIntentContractValidation", "LLM draft validation has a dedicated PinArrayGap contract path.");
        RequireContains(llmDraftValidationRules, "PinArrayGap contract: MEASURE ONLY / NOT JUDGED - no acceptance gate is present.", "PinArrayGap validation labels an ungated draft as measurement-only.");
        RequireContains(llmDraftValidationRules, "VisionPipelineKnownMetrics.DistancePxRange", "PinArrayGap validation requires the locked consistency metric.");
        RequireContains(llmDraftValidationRules, "every row uses a positive DistancePxRange maximum acceptance gate", "PinArrayGap validation requires the exact judged gate on every row.");
        RequireContains(llmDraftValidationRules, "does not match the reviewed ROI count", "PinArrayGap validation compares the returned row count with the current reviewed ROI state.");
        RequireContains(llmDraftValidationRules, "valid ROI/detection parameters matching the reviewed state", "PinArrayGap validation compares returned ROI and detection values with the current Guided Setup state.");
        RequireContains(llmDraftValidationRules, "the current Guided Setup state requires a DistancePxRange maximum on every row", "PinArrayGap validation rejects a measurement-only response when the current skill state is judged.");
        RequireContains(llmDraftValidationRules, "source-bounded ROI", "PinArrayGap strict validation keeps every reviewed ROI inside the selected source image.");

        string dependencyReviewService = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeDependencyReviewService.cs");
        RequireContains(dependencyReviewService, "internal static class OpenVisionRecipeDependencyReviewService", "Dependency review execution has a Recipe Review owner.");
        RequireContains(dependencyReviewService, "internal sealed class OpenVisionRecipeDependencyReviewResult", "Dependency review execution returns an explicit result.");
        RequireContains(dependencyReviewService, "internal static OpenVisionRecipeDependencyReviewResult Review", "Dependency review owns the scan/copy decision.");
        RequireContains(dependencyReviewService, "internal static bool LooksLikeDependencyPath", "Dependency review owns path classification.");
        RequireContains(dependencyReviewService, "internal static string ResolveDependencySourcePath", "Dependency review owns path resolution.");
        RequireContains(dependencyReviewService, "TryCopyReferenceImageToRecipe", "Dependency review owns reference-image copying.");

        string llmDraftValidationService = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Validation\OpenVisionRecipeLlmDraftValidationService.cs");
        RequireContains(llmDraftValidationService, "internal static class OpenVisionRecipeLlmDraftValidationService", "LLM draft orchestration has a Recipe Validation owner.");
        RequireContains(llmDraftValidationService, "internal sealed class OpenVisionRecipeLlmDraftValidationRequest", "LLM draft orchestration receives an explicit request.");
        RequireContains(llmDraftValidationService, "internal sealed class OpenVisionRecipeLlmDraftValidationResult", "LLM draft orchestration returns an explicit result.");
        RequireContains(llmDraftValidationService, "SerializeHelper.TryLoadFromXmlText", "LLM draft orchestration owns XML deserialization.");
        RequireContains(llmDraftValidationService, "VisionPipelineValidator.Validate", "LLM draft orchestration owns schema and routing validation.");
        RequireContains(llmDraftValidationService, "OpenVisionRecipeDependencyReviewService.Review", "LLM draft orchestration receives dependency review results.");

        Pass("Source ownership contract");
    }

    private static void CheckToolViewSourceOrganization(string repoRoot)
    {
        const string toolViewRoot = @"UI\VisionTest\Wpf";
        string toolViewDirectory = Path.Combine(repoRoot, toolViewRoot);
        if (!Directory.Exists(toolViewDirectory))
        {
            Failures.Add($"Tool View source root was not found: {toolViewDirectory}");
            return;
        }

        string[] rootSourceFiles = Directory
            .EnumerateFiles(toolViewDirectory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(path => string.Equals(Path.GetExtension(path), ".cs", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".xaml", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .ToArray();
        if (rootSourceFiles.Length > 0)
        {
            Failures.Add("Tool View source root must not retain direct C# or XAML files: " + string.Join(", ", rootSourceFiles));
        }

        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Behaviors",
            "ArithmeticToolInteractionController.cs",
            "LineToolInteractionController.cs",
            "SimplePreprocessParameterController.cs",
            "VisionToolActionBehavior.cs",
            "VisionToolControlBinding.cs",
            "VisionToolControlValueReader.cs",
            "VisionToolFilterInteractionController.cs",
            "VisionToolKernelSizeController.cs",
            "VisionToolLayerSelectionBehavior.cs",
            "VisionToolMorphologyInteractionController.cs",
            "VisionToolTextInputBehavior.cs",
            "VisionToolThresholdInteractionController.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Contracts",
            "IArithmeticVisionToolWpfView.cs",
            "ISingleInputVisionToolWpfView.cs",
            "IVisionToolViewLifetime.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Preview",
            "ArithmeticToolPreviewController.cs",
            "LineToolPreviewController.cs",
            "VisionToolDebouncedPreviewScheduler.cs",
            "VisionToolInlinePreviewSlot.cs",
            "VisionToolOpenGlPreviewSlot.cs",
            "VisionToolPreviewImageCommands.cs",
            "VisionToolPreviewStatePresenter.cs",
            "VisionToolPropertyPreviewPolicy.cs",
            "VisionToolThresholdTeachingPreviewController.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\PropertyGrid",
            "VisionToolMatchingPropertyRuntime.cs",
            "VisionToolParameterChangeController.cs",
            "VisionToolParameterPresenters.cs",
            "VisionToolPropertyChangeController.cs",
            "VisionToolPropertyGridHost.cs",
            "VisionToolPropertyGridPresenter.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Presets",
            "VisionToolPreset.cs",
            "VisionToolPresetButtonPresenter.cs",
            "VisionToolPresetCatalog.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Layers",
            "VisionToolLayerChangeController.cs",
            "VisionToolLayerComboHelper.cs",
            "VisionToolLayerSelectionPresenter.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Interaction",
            "VisionToolActionRequestController.cs",
            "VisionToolLanguageChangeController.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Presentation",
            "ArithmeticToolTextPresenter.cs",
            "FilterToolTextPresenter.cs",
            "LineToolPresenter.cs",
            "LineToolTextPresenter.cs",
            "MorphologyToolTextPresenter.cs",
            "SimplePreprocessTextPresenter.cs",
            "ThresholdToolTextPresenter.cs",
            "VisionToolChromePresenter.cs",
            "VisionToolTemplateStatusPresenter.cs",
            "VisionToolWpfStatusPresenter.cs",
            "VisionToolWpfTheme.xaml");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\Review",
            "AffineTransformResultReviewPresenter.cs",
            "AutoMPointHtmlReportExporter.cs",
            "LineToolResultExplanation.cs",
            "LineToolResultReviewPresenter.cs",
            "LineToolReviewController.cs",
            "LineToolVerificationGuidePresenter.cs",
            "SimplePreprocessResultExplanation.cs",
            "VisionToolAreaResultExplanation.cs",
            "VisionToolAreaResultReviewPresenter.cs",
            "VisionToolAreaVerificationCriteriaText.cs",
            "VisionToolAreaVerificationGuidePresenter.cs",
            "VisionToolMatchingResultExplanation.cs",
            "VisionToolMatchingResultReviewPresenter.cs",
            "VisionToolMatchingVerificationGuidePresenter.cs",
            "VisionToolResultReviewPresenter.cs",
            "VisionToolVerificationGuideView.xaml",
            "VisionToolVerificationGuideView.xaml.cs",
            "VisionToolVerificationText.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\SingleInput",
            "VisionToolSingleInputCustomToolController.cs",
            "VisionToolSingleInputCustomToolRuntime.cs",
            "VisionToolSingleInputCustomToolViewBase.cs",
            "VisionToolSingleInputMatchingToolController.cs",
            "VisionToolSingleInputMatchingToolRuntime.cs",
            "VisionToolSingleInputPropertyToolController.cs",
            "VisionToolSingleInputPropertyToolRuntime.cs",
            "VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs",
            "VisionToolSingleInputPropertyToolShell.xaml",
            "VisionToolSingleInputPropertyToolShell.xaml.cs",
            "VisionToolSingleInputPropertyToolViewBase.cs",
            "VisionToolSingleInputSpecialPropertyToolController.cs",
            "VisionToolSingleInputSpecialPropertyToolRuntime.cs",
            "VisionToolSingleInputToolEventHub.cs",
            "VisionToolSingleInputViewBinder.cs",
            "VisionToolSingleInputViewModel.cs",
            "VisionToolSingleInputViewRuntime.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Tooling\DoubleInput",
            "VisionToolDoubleInputCustomToolController.cs",
            "VisionToolDoubleInputCustomToolRuntime.cs",
            "VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs",
            "VisionToolDoubleInputCustomToolShell.xaml",
            "VisionToolDoubleInputCustomToolShell.xaml.cs",
            "VisionToolDoubleInputCustomToolViewBase.cs",
            "VisionToolDoubleInputToolEventHub.cs",
            "VisionToolDoubleInputViewBinder.cs",
            "VisionToolDoubleInputViewModel.cs",
            "VisionToolDoubleInputViewRuntime.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\ToolViews",
            "AffineTransformToolWpfView.xaml",
            "AffineTransformToolWpfView.xaml.cs",
            "ArithmeticToolWpfView.xaml",
            "ArithmeticToolWpfView.xaml.cs",
            "AutoMPointTeachingPanel.xaml",
            "AutoMPointTeachingPanel.xaml.cs",
            "BlobToolWpfView.xaml",
            "BlobToolWpfView.xaml.cs",
            "ContourToolWpfView.xaml",
            "ContourToolWpfView.xaml.cs",
            "EdgeBasedMatchingToolWpfView.xaml",
            "EdgeBasedMatchingToolWpfView.xaml.cs",
            "FeatureMatchingToolWpfView.xaml",
            "FeatureMatchingToolWpfView.xaml.cs",
            "FilterToolWpfView.xaml",
            "FilterToolWpfView.xaml.cs",
            "LineToolWpfView.xaml",
            "LineToolWpfView.xaml.cs",
            "MatchingToolWpfView.xaml",
            "MatchingToolWpfView.xaml.cs",
            "MorphologyToolWpfView.xaml",
            "MorphologyToolWpfView.xaml.cs",
            "SimplePreprocessToolWpfView.xaml",
            "SimplePreprocessToolWpfView.xaml.cs",
            "ThresholdToolWpfView.xaml",
            "ThresholdToolWpfView.xaml.cs");
        RequireToolViewOwnerFiles(repoRoot, @"UI\VisionTest\Wpf\Learn",
            "OpenVisionLearnTopics.cs",
            "OpenVisionLearnWindow.xaml",
            "OpenVisionLearnWindow.xaml.cs",
            "ThresholdToolLearnWindowController.cs",
            "VisionToolLearnWindowController.cs");

        Pass("Tool View source organization contract");
    }

    private static void RequireExactDirectSourceFiles(
        string repoRoot,
        string ownerDirectory,
        string description,
        params string[] expectedFiles)
    {
        string directoryPath = Path.Combine(repoRoot, ownerDirectory);
        if (!Directory.Exists(directoryPath))
        {
            Failures.Add($"{description} Missing directory: {ownerDirectory}");
            return;
        }

        string[] actualFiles = Directory
            .EnumerateFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(path => string.Equals(Path.GetExtension(path), ".cs", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".xaml", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        string[] normalizedExpectedFiles = (expectedFiles ?? Array.Empty<string>())
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (!actualFiles.SequenceEqual(normalizedExpectedFiles, StringComparer.OrdinalIgnoreCase))
        {
            Failures.Add(
                $"{description} Expected [{string.Join(", ", normalizedExpectedFiles)}], "
                + $"actual [{string.Join(", ", actualFiles)}].");
        }
    }

    private static void RequireToolViewOwnerFiles(string repoRoot, string ownerDirectory, params string[] expectedFiles)
    {
        string fullDirectory = Path.Combine(repoRoot, ownerDirectory);
        if (!Directory.Exists(fullDirectory))
        {
            Failures.Add($"Tool View owner directory was not found: {ownerDirectory}");
            return;
        }

        string[] actualFiles = Directory
            .EnumerateFiles(fullDirectory, "*.*", SearchOption.TopDirectoryOnly)
            .Where(path => string.Equals(Path.GetExtension(path), ".cs", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".xaml", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .ToArray();
        string[] expected = expectedFiles
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .ToArray();
        if (!actualFiles.SequenceEqual(expected, StringComparer.Ordinal))
        {
            Failures.Add($"Tool View owner directory does not match the required source layout: {ownerDirectory}. Expected='{string.Join(", ", expected)}', Actual='{string.Join(", ", actualFiles)}'");
        }
    }

    private static void CheckToolViewControllerOwnership(string repoRoot)
    {
        string toolViewDirectory = Path.Combine(repoRoot, @"UI\VisionTest\Wpf\ToolViews");
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

        string lineView = Read(repoRoot, @"UI\VisionTest\Wpf\ToolViews\LineToolWpfView.xaml.cs");
        RequireContains(lineView, "VisionToolSingleInputSpecialPropertyToolController.Attach", "Line Tool delegates special PropertyGrid shell wiring to the shared controller.");

        string arithmeticView = Read(repoRoot, @"UI\VisionTest\Wpf\ToolViews\ArithmeticToolWpfView.xaml.cs");
        RequireContains(arithmeticView, "VisionToolDoubleInputCustomToolViewBase", "Arithmetic Tool delegates double-input shell forwarding to the shared view base.");
        string doubleInputCustomToolViewBase = Read(repoRoot, @"UI\VisionTest\Wpf\Tooling\DoubleInput\VisionToolDoubleInputCustomToolViewBase.cs");
        RequireContains(doubleInputCustomToolViewBase, "VisionToolDoubleInputCustomToolController.Attach", "Double-input custom Tool View base delegates shell wiring to the shared controller.");

        string singleInputSpecialController = Read(repoRoot, @"UI\VisionTest\Wpf\Tooling\SingleInput\VisionToolSingleInputSpecialPropertyToolController.cs");
        RequireContains(singleInputSpecialController, "VisionToolSingleInputSpecialPropertyToolRuntime.Attach", "Special PropertyGrid controller owns the special single-input runtime wiring.");

        string doubleInputController = Read(repoRoot, @"UI\VisionTest\Wpf\Tooling\DoubleInput\VisionToolDoubleInputCustomToolController.cs");
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
        string commercialGapReview = Read(repoRoot, @"docs\OPENVISIONLAB_COMMERCIAL_GAP_PRIORITY_REVIEW_20260710.md");
        string guidedSetupSpec = Read(repoRoot, @"docs\OPENVISIONLAB_GUIDED_INSPECTION_SETUP_SPEC.md");
        RequireContains(commercialGapReview, "Non-LLM Guided Inspection Setup", "Commercial gap review prioritizes non-LLM guided setup.");
        RequireContains(commercialGapReview, "Guided Inspection Setup", "Commercial gap review names the guided setup development track.");
        RequireContains(commercialGapReview, "Camera acquisition setup", "Commercial gap review keeps camera acquisition out of scope.");
        RequireContains(commercialGapReview, "Tool Palette Search And Readiness", "Commercial gap review tracks tool discovery/readiness as a later operating gap.");
        RequireContains(commercialGapReview, "report-first summary baseline", "Commercial gap review records the consolidated result-board baseline.");
        RequireContains(guidedSetupSpec, "Guided Inspection Setup is a recipe authoring helper, not a hardware wizard.", "Guided setup spec keeps the feature inside recipe authoring scope.");
        RequireContains(guidedSetupSpec, "It must not:", "Guided setup spec documents forbidden side effects.");
        RequireContains(guidedSetupSpec, "Run Preview automatically.", "Guided setup spec blocks automatic Preview.");
        RequireContains(guidedSetupSpec, "Run Review automatically.", "Guided setup spec blocks automatic Run Review.");
        RequireContains(guidedSetupSpec, "Replace PropertyGrid tool editing.", "Guided setup spec preserves PropertyGrid editing.");
        RequireContains(guidedSetupSpec, "Blob count / particle count", "Guided setup spec includes Blob count starter intent.");
        RequireContains(guidedSetupSpec, "Contour shape / outline", "Guided setup spec includes Contour starter intent.");
        RequireContains(guidedSetupSpec, "Pin gap / pitch measurement", "Guided setup spec includes LineDistance measurement starter intent.");
        RequireContains(guidedSetupSpec, "Template target presence", "Guided setup spec includes Matching starter intent.");
        RequireContains(guidedSetupSpec, "Brightness drift / mean value", "Guided setup spec includes Mean starter intent.");
        RequireContains(guidedSetupSpec, "Do not choose `Contour` just because pins are visible.", "Guided setup spec prevents pin-gap fallback to Contour.");
        RequireContains(guidedSetupSpec, "DistancePxRange", "Guided setup spec requires pin-gap consistency metrics.");
        RequireContains(guidedSetupSpec, "DistanceMmRange", "Guided setup spec requires pin-gap mm consistency metrics.");
        RequireContains(guidedSetupSpec, "Create Starter XML", "Guided setup spec defines an explicit starter XML action.");
        RequireContains(guidedSetupSpec, "Starter XML creation does not call Preview", "Guided setup spec requires no auto-run verification.");
        RequireContains(guidedSetupSpec, "DistanceMmAvg=0.224", "Guided setup spec records public Pin gap mm/px parity evidence.");
        string recipeCommandSurface = ReadSourceFamily(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.cs");
        string shellHostView = Read(repoRoot, @"UI\Menu\Wpf\OpenVisionShellHostView.xaml");
        string screenshotSmoke = Read(repoRoot, @"tools\PipelineViewerScreenshotSmoke\Program.cs");
        string directSmokeRunner = Read(repoRoot, @"tools\OpenVisionLab.DirectSmokeRunner\OpenVisionLabDirectSmokeRunner.cs");
        string pinGapIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipePinGapIntentSkill.cs");
        string pipelineValidation = Read(repoRoot, @"Core\Pipeline\Validation\VisionPipelineValidation.cs");
        string matchingIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeMatchingIntentSkill.cs");
        string meanIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeMeanIntentSkill.cs");
        string referenceDifferenceIntentSkill = Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeReferenceDifferenceIntentSkill.cs");
        string recipeOperatorDecisionPresenter = Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeOperatorDecisionPresenter.cs");
        RequireContains(recipeCommandSurface, "GuidedSetupSummaryText", "Recipe Manager exposes Guided setup summary text.");
        RequireContains(recipeCommandSurface, "GuidedSetupReadinessText", "Recipe Manager exposes Guided setup readiness text.");
        RequireContains(recipeCommandSurface, "OperatorDecisionEvidenceText", "Recipe Manager exposes consolidated result-board metric evidence text.");
        RequireContains(recipeCommandSurface, "BuildOperatorDecisionPresentation()", "Recipe Manager supplies selected state to the consolidated decision-board presenter.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeOperatorDecisionPresenter.Build(", "Recipe Manager delegates decision-board text composition to its presenter.");
        RequireContains(recipeCommandSurface, "OperatorDecisionSummaryStatusText", "Recipe Manager consolidates final status and failed step from existing result channels.");
        RequireContains(recipeOperatorDecisionPresenter, "Metric review: expected", "Decision-board presenter summarizes expected versus actual metric evidence.");
        RequireContains(recipeOperatorDecisionPresenter, "Metric evidence: ", "Decision-board handoff report includes metric evidence.");
        RequireContains(recipeCommandSurface, "ShowRecentBatchNgOnly", "Recipe Manager exposes run-history NG-only filter state.");
        RequireContains(recipeCommandSurface, "FilteredRecentBatchRunSampleResults", "Recipe Manager exposes filtered run-history sample results.");
        RequireContains(recipeCommandSurface, "RecentBatchRunNgFilterSummaryText", "Recipe Manager summarizes run-history NG causes.");
        RequireContains(recipeCommandSurface, "PinGapIntentCalibrationReviewText", "Recipe Manager exposes Pin gap calibration review text.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeIntentFeedbackPresenter.BuildPinGapCalibrationReviewText", "Recipe Manager delegates Pin gap calibration review text.");
        RequireContains(recipeCommandSurface, "PIXELPERMM", "Recipe Manager explains the Pin gap mm/px scale.");
        RequireContains(recipeCommandSurface, "PX-ONLY", "Recipe Manager exposes Pin gap pixel-only state.");
        RequireContains(recipeCommandSurface, "MM-READY", "Recipe Manager exposes calibrated Pin gap state.");
        RequireContains(recipeCommandSurface, "CreatePixelPipeline", "Recipe Manager routes px-only Pin gap setup to pixel metrics.");
        RequireContains(
            Read(repoRoot, @"UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeIntentFeedbackPresenter.cs"),
            "average-only measurement is not enough",
            "Recipe intent feedback warns against average-only distance measurements.");
        RequireContains(recipeCommandSurface, "Starter XML creation only updates the draft; it does not create layers, import a recipe, Preview, or Run.", "Guided setup VM states starter XML has no execution side effects.");
        RequireContains(recipeCommandSurface, "Created Guided setup draft XML. Preview/Run was not executed", "Guided setup generic draft creation reports no auto Preview/Run.");
        RequireContains(recipeCommandSurface, "RecipeGuidedSetupTabText", "Recipe Manager exposes the standalone Guided setup tab label.");
        RequireContains(recipeCommandSurface, "GuidedSetupNoLlmText", "Guided setup explains that LLM assistance is optional.");
        RequireContains(recipeCommandSurface, "GuidedSetupActionBoundaryText", "Guided setup exposes its no-auto-run boundary text.");
        RequireContains(recipeCommandSurface, "CreateGuidedSetupStarterXmlCommand", "Guided setup exposes one deterministic Starter XML command.");
        RequireContains(recipeCommandSurface, "OpenVisionRecipeGuidedSetupReadinessPresenter.Evaluate", "Guided setup validates intent-specific readiness before Starter XML creation.");
        RequireContains(recipeCommandSurface, "OpenVisionGuidedSetupCatalog.PinArrayGapTemplate", "Guided setup exposes the dedicated PinArrayGap pilot template.");
        RequireContains(recipeCommandSurface, "CreatePinArrayGapIntentXmlDraft();", "Guided setup routes the PinArrayGap pilot to its deterministic starter generator.");
        RequireContains(recipeCommandSurface, "PinArrayGapRoiText", "Guided setup captures one or more reviewed PinArrayGap row ROIs.");
        RequireContains(recipeCommandSurface, "PinArrayGapPolarityText", "Guided setup captures the PinArrayGap polarity contract.");
        RequireContains(recipeCommandSurface, "PinArrayGapMeasurementText", "Guided setup captures the PinArrayGap measurement definition.");
        RequireContains(recipeCommandSurface, "PinArrayGapRangeMaxText", "Guided setup captures the optional DistancePxRange judgement gate.");
        RequireContains(recipeCommandSurface, "PinArrayGapDarkThresholdText", "Guided setup captures the PinArrayGap dark-threshold parameter.");
        RequireContains(recipeCommandSurface, "PinArrayGapMinDarkCoverageRatioText", "Guided setup captures the PinArrayGap dark-coverage parameter.");
        RequireContains(recipeCommandSurface, "PinArrayGapMinPinWidthText", "Guided setup captures the PinArrayGap minimum pin width.");
        RequireContains(recipeCommandSurface, "PinArrayGapMaxPinBreakWidthText", "Guided setup captures the PinArrayGap maximum pin-break width.");
        RequireContains(recipeCommandSurface, "PinArrayGapMinGapWidthText", "Guided setup captures the PinArrayGap minimum gap width.");
        RequireContains(recipeCommandSurface, "CreatePinGapIntentXmlDraft();", "Guided setup routes distance intent to the existing Pin gap generator.");
        RequireContains(recipeCommandSurface, "CreateBlobCountIntentXmlDraft();", "Guided setup routes Blob intent to the existing Blob generator.");
        RequireContains(recipeCommandSurface, "CreateContourCountIntentXmlDraft();", "Guided setup routes Contour intent to the existing Contour generator.");
        RequireContains(recipeCommandSurface, "CreateMatchingIntentXmlDraft();", "Guided setup routes Matching intent to the deterministic Matching generator.");
        RequireContains(
            Read(repoRoot, @"UI\Menu\Wpf\Recipe\IntentSkills\OpenVisionRecipeGuidedSetupReadinessPresenter.cs"),
            "READY: template + Search ROI + SCORE_MIN + ResultCount gate",
            "Guided setup reports Matching readiness inputs.");
        RequireContains(recipeCommandSurface, "CreateMeanIntentXmlDraft();", "Guided setup routes Mean intent to the deterministic Mean generator.");
        RequireContains(recipeCommandSurface, "CreateReferenceDifferenceIntentXmlDraft();", "Guided setup routes Golden-reference defect intent to the deterministic ReferenceDifference generator.");
        RequireContains(recipeCommandSurface, "MeanValueAvg ", "Guided setup reports MeanValueAvg readiness inputs.");
        RequireContains(matchingIntentSkill, "ToolType = \"Matching\"", "Matching intent skill locks ToolType to Matching.");
        RequireContains(matchingIntentSkill, "step.Parameters[\"PATTERN_PATH\"]", "Matching intent skill preserves the template dependency path.");
        RequireContains(matchingIntentSkill, "step.Parameters[\"CvROI\"]", "Matching intent skill writes the search ROI.");
        RequireContains(matchingIntentSkill, "AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount", "Matching intent skill judges the expected match count.");
        RequireContains(meanIntentSkill, "ToolType = \"Mean\"", "Mean intent skill locks ToolType to Mean.");
        RequireContains(meanIntentSkill, "step.Parameters[\"MEAN_TYPES\"]", "Mean intent skill preserves the selected mean type.");
        RequireContains(meanIntentSkill, "step.Parameters[\"USE_ROI\"]", "Mean intent skill records full-image or ROI scope.");
        RequireContains(meanIntentSkill, "AcceptanceMetricName = VisionPipelineKnownMetrics.MeanValueAvg", "Mean intent skill judges MeanValueAvg.");
        RequireContains(referenceDifferenceIntentSkill, "ToolType = \"ReferenceDifference\"", "Golden-reference defect intent locks ToolType to ReferenceDifference.");
        RequireContains(referenceDifferenceIntentSkill, "ReferencePath\" + (index + 1)", "Golden-reference defect intent writes up to four explicit reference paths.");
        RequireContains(referenceDifferenceIntentSkill, "AcceptanceMetricName = VisionPipelineKnownMetrics.ResultCount", "Golden-reference defect intent judges ResultCount.");
        RequireContains(referenceDifferenceIntentSkill, "AcceptanceMetricMaximum = 0", "Golden-reference defect intent requires zero detected defect regions.");
        RequireContains(pinGapIntentSkill, "CreatePixelPipeline", "Pin gap intent skill exposes a px-only pipeline path.");
        RequireContains(pinGapIntentSkill, "VisionPipelineKnownMetrics.DistancePxAvg", "Pin gap px-only pipeline judges DistancePxAvg.");
        RequireContains(pinGapIntentSkill, "VisionPipelineKnownMetrics.DistancePxRange", "Pin gap px-only pipeline judges DistancePxRange.");
        RequireContains(pipelineValidation, "ValidateMetricCalibration", "Pipeline validation blocks mm gates without calibration.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupSummary", "Recipe Manager renders the Guided setup summary.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupReadiness", "Recipe Manager renders Guided setup readiness.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupTab", "Recipe Manager exposes a separate Guided setup tab.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupIntentSelector", "Guided setup exposes an intent selector without requiring an LLM.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupCreateStarterButton", "Guided setup exposes explicit Starter XML creation.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupActionBoundary", "Guided setup states its no-auto-run action boundary.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupDraftText", "Guided setup renders the generated starter draft.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapInputs", "Guided setup renders the dedicated PinArrayGap pilot inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapRoiText", "Guided setup renders the PinArrayGap row-ROI input.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapPolarity", "Guided setup renders the PinArrayGap polarity selector.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapMeasurement", "Guided setup renders the PinArrayGap measurement selector.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapRangeMaxText", "Guided setup renders the optional DistancePxRange maximum.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapDarkThresholdText", "Guided setup renders the PinArrayGap dark threshold.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapMinDarkCoverageRatioText", "Guided setup renders the PinArrayGap dark coverage ratio.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapMinPinWidthText", "Guided setup renders the PinArrayGap minimum pin width.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapMaxPinBreakWidthText", "Guided setup renders the PinArrayGap maximum pin-break width.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinArrayGapMinGapWidthText", "Guided setup renders the PinArrayGap minimum gap width.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinGapInputs", "Guided setup renders Pin gap intent inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupPinGapCalibrationReview", "Guided setup renders Pin gap calibration state and conversion review.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupBlobInputs", "Guided setup renders Blob intent inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupContourInputs", "Guided setup renders Contour intent inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMatchingInputs", "Guided setup renders Matching intent inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMatchingTemplatePathText", "Guided setup renders the Matching template path.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMatchingSearchRoiText", "Guided setup renders the Matching search ROI.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMatchingScoreMinText", "Guided setup renders the Matching score gate.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMatchingExpectedCountText", "Guided setup renders the Matching expected count.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMeanInputs", "Guided setup renders Mean intent inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMeanRoiText", "Guided setup renders the optional Mean ROI.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMeanTypeSelector", "Guided setup renders the Mean type selector.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMeanMinimumText", "Guided setup renders the Mean minimum GV gate.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupMeanMaximumText", "Guided setup renders the Mean maximum GV gate.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupReferenceDifferenceInputs", "Guided setup renders Golden-reference defect inputs.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupReferenceDifferencePath4Text", "Guided setup renders up to four explicit Good reference paths.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupReferenceDifferenceThresholdText", "Guided setup renders the ReferenceDifference threshold.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupReferenceDifferenceMaximumAreaText", "Guided setup renders the ReferenceDifference defect-area limits.");
        RequireContains(shellHostView, "HostRecipeGuidedSetupIntentInputStatus", "Guided setup renders READY/MISSING intent status.");
        RequireContains(shellHostView, "RecipeCommands.CreateGuidedSetupStarterXmlCommand", "Guided setup binds the deterministic Starter XML command.");
        RequireContains(shellHostView, "HostRecipeOperatorDecisionEvidence", "Recipe Manager renders consolidated result-board metric evidence.");
        RequireContains(shellHostView, "RecipeCommands.OperatorDecisionEvidenceText", "Recipe Manager binds consolidated result-board metric evidence.");
        RequireContains(shellHostView, "HostRecipeOperatorDecisionSummaryBand", "Recipe Manager renders the report-first operator decision summary band.");
        RequireContains(shellHostView, "HostRecipeOperatorDecisionSummaryStatus", "Operator decision summary exposes final Good/Bad status.");
        RequireContains(shellHostView, "HostRecipeOperatorDecisionSummaryMetric", "Operator decision summary exposes expected/actual metric evidence.");
        RequireContains(shellHostView, "HostRecipeOperatorDecisionSummaryNextAction", "Operator decision summary exposes the next action.");
        RequireContains(shellHostView, "HostRecipeRecentBatchRunNgOnlyToggle", "Recipe Manager renders the run-history NG-only toggle.");
        RequireContains(shellHostView, "HostRecipeRecentBatchRunNgFilterSummary", "Recipe Manager renders the run-history NG cause summary.");
        RequireContains(shellHostView, "RecipeCommands.FilteredRecentBatchRunSampleResults", "Recipe Manager binds the filtered run-history sample list.");
        RequireContains(shellHostView, "HostRecipePinGapIntentCalibrationReviewText", "Recipe Manager renders Pin gap calibration review.");
        RequireContains(shellHostView, "RecipeCommands.GuidedSetupReadinessText", "Recipe Manager binds readiness to the selected intent.");
        RequireContains(shellHostView, "RecipeCommands.PinGapIntentCalibrationReviewText", "Recipe Manager binds Pin gap calibration review.");
        RequireContains(screenshotSmoke, "wpf_shell_host_recipe_guided_setup", "Screenshot smoke includes a dedicated Guided setup capture target.");
        RequireContains(screenshotSmoke, "CaptureShellHostRecipeGuidedSetup", "Screenshot smoke captures the Guided setup tab directly.");
        RequireContains(screenshotSmoke, "wpf_shell_host_recipe_operator_decision_board", "Screenshot smoke includes a dedicated operator decision board capture target.");
        RequireContains(screenshotSmoke, "CaptureShellHostRecipeOperatorDecisionBoard", "Screenshot smoke captures the operator decision board directly.");
        RequireContains(screenshotSmoke, "HostRecipeOperatorDecisionEvidence", "Screenshot smoke verifies consolidated result-board metric evidence.");
        RequireContains(screenshotSmoke, "OperatorDecisionEvidenceText", "Screenshot smoke checks consolidated result-board metric evidence text.");
        RequireContains(screenshotSmoke, "HostRecipeOperatorDecisionSummaryBand", "Screenshot smoke captures the report-first operator decision summary.");
        RequireContains(screenshotSmoke, "HostRecipeRecentBatchRunNgOnlyToggle", "Screenshot smoke verifies the run-history NG-only toggle.");
        RequireContains(screenshotSmoke, "FilteredRecentBatchRunSampleResults", "Screenshot smoke verifies filtered run-history sample results.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupSummary", "Screenshot smoke verifies the Guided setup summary.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupReadiness", "Screenshot smoke verifies Guided setup readiness.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupIntentSelector", "Screenshot smoke verifies the standalone Guided setup selector.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupCreateStarterButton", "Screenshot smoke verifies explicit Guided setup Starter XML creation.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupDraftText", "Screenshot smoke verifies the Guided setup draft output.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupPinGapInputs", "Screenshot smoke verifies Pin gap input controls.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupBlobInputs", "Screenshot smoke verifies Blob input controls.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupContourInputs", "Screenshot smoke verifies Contour input controls.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupMatchingInputs", "Screenshot smoke verifies Matching input controls.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupMeanInputs", "Screenshot smoke verifies Mean input controls.");
        RequireContains(screenshotSmoke, "HostRecipeGuidedSetupReferenceDifferenceInputs", "Screenshot smoke verifies Golden-reference defect input controls.");
        RequireContains(screenshotSmoke, "did not block missing ReferenceDifference Good references", "Screenshot smoke verifies missing Good references are blocked.");
        RequireContains(screenshotSmoke, "did not reject a ReferenceDifference draft without the exact ResultCount=0 gate", "Screenshot smoke verifies the Golden-reference defect acceptance contract.");
        RequireContains(screenshotSmoke, "did not expose ready px-only Pin gap inputs", "Screenshot smoke verifies Pin gap PX-ONLY readiness.");
        RequireContains(screenshotSmoke, "did not block an invalid Pin gap scale input", "Screenshot smoke verifies invalid Pin gap scale MISSING state.");
        RequireContains(screenshotSmoke, "did not block an invalid Blob threshold", "Screenshot smoke verifies Blob MISSING state.");
        RequireContains(screenshotSmoke, "did not block an invalid Contour max area", "Screenshot smoke verifies Contour MISSING state.");
        RequireContains(screenshotSmoke, "did not block a missing Matching template path", "Screenshot smoke verifies Matching template readiness.");
        RequireContains(screenshotSmoke, "did not block Matching SCORE_MIN outside 0..1", "Screenshot smoke verifies Matching score validation.");
        RequireContains(screenshotSmoke, "did not block Matching expected count <= 0", "Screenshot smoke verifies Matching count validation.");
        RequireContains(screenshotSmoke, "did not block an invalid Matching search ROI", "Screenshot smoke verifies Matching ROI validation.");
        RequireContains(screenshotSmoke, "did not block an unsupported Mean type", "Screenshot smoke verifies Mean type validation.");
        RequireContains(screenshotSmoke, "did not block Mean GV outside 0..255", "Screenshot smoke verifies Mean GV range validation.");
        RequireContains(screenshotSmoke, "did not block Mean Min GV greater than Max GV", "Screenshot smoke verifies Mean min/max ordering.");
        RequireContains(screenshotSmoke, "did not block an invalid optional Mean ROI", "Screenshot smoke verifies Mean ROI validation.");
        RequireContains(directSmokeRunner, "GuidedSetupStandalone: Pin gap + Blob + Contour + Matching + Feature Matching + Edge Based Matching + Mean + ReferenceDifference Starter XML without Preview/Run", "Direct EXE smoke records all eight standalone Guided setup intents.");
        RequireContains(directSmokeRunner, "recipe-manager-reference-difference-guided-setup", "Direct EXE smoke exposes a deployable ReferenceDifference Guided setup scenario.");
        RequireContains(directSmokeRunner, "LayerAndRouteStateUnchanged: true", "ReferenceDifference Guided setup EXE smoke records the no-layer/no-route side-effect contract.");
        RequireContains(directSmokeRunner, "GuidedSetupPinGapUnits: MM-READY conversion review + PX-ONLY DistancePx gates + invalid scale blocked", "Direct EXE smoke records both Pin gap unit modes.");
        RequireContains(directSmokeRunner, "GuidedSetupPinGapPublicSample: ", "Direct EXE smoke records public Pin gap Good/Bad unit parity.");
        RequireContains(directSmokeRunner, "Line_Pins_Synthetic_WidePin_NG.png", "Direct EXE smoke runs the public Pin gap Bad sample.");
        RequireContains(directSmokeRunner, "OperatorDecisionSummaryBand: final Good/Bad status + expected/actual metric evidence + next action", "Direct EXE smoke records the consolidated operator decision summary.");
        RequireContains(directSmokeRunner, "OpenCvSharp.ImreadModes.Unchanged", "LLM XML image smoke preserves color channels for HSV and color recipes.");
        RequireContains(directSmokeRunner, "--expect-run-success", "LLM XML image smoke can verify expected Good and expected NG samples separately.");
        RequireContains(learnIndex, "## Learn Window Topic Map", "Learn index documents the Learn window topic map.");
        RequireContains(learnIndex, "## Practice Workflow", "Learn index documents the learner practice workflow.");
        RequireContains(learnIndex, "Choose a Good/Bad sample pair", "Learn index starts practice from a Good/Bad pair.");
        RequireContains(learnIndex, "related Tool View or Pipeline Review", "Learn index connects samples to the related review surface.");
        RequireContains(learnIndex, "Click Preview or Run Review", "Learn index gives the operator the next execution action.");
        RequireContains(learnIndex, "Compare overlay, result image, metric, and Good/Bad reason", "Learn index names the evidence to compare.");
        RequireContains(learnIndex, "`Practice Samples` opens the sample catalog", "Learn index explains the Practice Samples action.");
        RequireNotContains(learnIndex, "must not create layers, change input/output routing", "Learn index must not expose internal execution contracts as learner copy.");
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
            (16, "LEARN_COLOR_HSV.md", "color-hsv"),
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
            foreach (string internalContractPhrase in new[]
            {
                "must not run",
                "does not run Preview",
                "no Preview/Run",
                "execution evidence",
                "smoke evidence",
                "runtime contract",
            })
            {
                RequireNotContains(
                    learn,
                    internalContractPhrase,
                    $"Learn document {relativePath} must explain the learner workflow instead of internal engineering contracts.");
            }
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

        string learnWindowXaml = Read(repoRoot, @"UI\VisionTest\Wpf\Learn\OpenVisionLearnWindow.xaml");
        string learnWindow = Read(repoRoot, @"UI\VisionTest\Wpf\Learn\OpenVisionLearnWindow.xaml.cs");
        string learnTopicsCatalog = Read(repoRoot, @"UI\VisionTest\Wpf\Learn\OpenVisionLearnTopics.cs");
        Dictionary<int, string> learnTopicIndexToEnum = BuildLearnTopicIndexToEnumMap(learnTopicsCatalog);
        string toolShell = Read(repoRoot, @"UI\VisionTest\Wpf\Tooling\SingleInput\VisionToolSingleInputPropertyToolShell.xaml.cs");
        string doubleInputToolShell = Read(repoRoot, @"UI\VisionTest\Wpf\Tooling\DoubleInput\VisionToolDoubleInputCustomToolShell.xaml.cs");
        string toolLearnWindowController = Read(repoRoot, @"UI\VisionTest\Wpf\Learn\VisionToolLearnWindowController.cs");
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
        RequireContains(foundationGuide, "Name the image concept first, choose the matching tool family", "OpenCvSharp foundations guide connects concepts to tool choice.");
        RequireContains(foundationGuide, "Use the provided Good/Bad samples", "OpenCvSharp foundations guide starts practice from provided sample evidence.");
        RequireContains(foundationGuide, "Good/Bad samples", "OpenCvSharp foundations guide connects concepts to Good/Bad validation.");
        RequireContains(learnWindowXaml, "OpenVisionLearnOpenFoundationDocsButton", "OpenVision Learn exposes a Foundation Docs button.");
        RequireContains(learnWindowXaml, "기초 용어", "OpenVision Learn labels the foundations button for learners.");
        RequireContains(learnWindow, "OpenFoundationDocsButton_Click", "OpenVision Learn handles the Foundation Docs button.");
        RequireContains(learnWindow, "LEARN_OPENCVSHARP_FOUNDATIONS.md", "OpenVision Learn Foundation Docs button opens the foundations guide.");
        RequireContains(learnWindowXaml, "OpenVisionLearnPracticeWorkflowPanel", "OpenVision Learn exposes the common practice workflow panel.");
        RequireContains(learnWindowXaml, "실습 순서", "OpenVision Learn labels the common practice workflow panel.");
        RequireContains(learnWindowXaml, "Preview 또는 Pipeline Review 실행", "OpenVision Learn gives the operator the next execution action.");
        RequireContains(learnWindowXaml, "입력 이미지, 출력 이미지, 핵심 지표", "OpenVision Learn names the evidence the operator should compare.");
        RequireNotContains(learnWindowXaml, "바뀌면 안", "OpenVision Learn must not expose internal routing contracts as learner copy.");
        RequireContains(learnWindowXaml, "OpenVisionLearnFoundationTypeCards", "OpenVision Learn foundation topic exposes Point/Rect/Size/Mat cards.");
        RequireContains(learnWindowXaml, "OpenVisionLearnBeginnerPathPanel", "OpenVision Learn foundation topic exposes a beginner path panel.");
        RequireContains(learnWindowXaml, "추천 순서: 영상 기초", "OpenVision Learn foundation topic shows the beginner tool path.");
        RequireContains(learnWindowXaml, "Good/Bad 기준으로 비교", "OpenVision Learn beginner path connects concepts to Good/Bad checks.");
        RequireContains(learnWindowXaml, "Mat = 행 x 열 x 채널", "OpenVision Learn foundation topic explains Mat as an image matrix.");
        string meanGuide = Read(repoRoot, @"docs\learn\LEARN_MEAN.md");
        string thresholdGuide = Read(repoRoot, @"docs\learn\LEARN_THRESHOLD.md");
        string filterGuide = Read(repoRoot, @"docs\learn\LEARN_FILTER.md");
        string edgeDetectionGuide = Read(repoRoot, @"docs\learn\LEARN_EDGE_DETECTION.md");
        string morphologyGuide = Read(repoRoot, @"docs\learn\LEARN_MORPHOLOGY.md");
        string blobGuide = Read(repoRoot, @"docs\learn\LEARN_BLOB.md");
        string contourGuide = Read(repoRoot, @"docs\learn\LEARN_CONTOUR.md");
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
        RequireContains(filterGuide, "final OK/NG metric", "Filter Learn guide connects preprocessing to downstream inspection evidence.");
        RequireContains(edgeDetectionGuide, "## Beginner path handoff", "EdgeDetection Learn guide includes beginner handoff.");
        RequireContains(edgeDetectionGuide, "Public_EdgeDetection_Shapes_Good", "EdgeDetection Learn guide names the public Good sample.");
        RequireContains(edgeDetectionGuide, "Public_EdgeDetection_Shapes_Missing_Bad", "EdgeDetection Learn guide names the public Bad sample.");
        RequireContains(edgeDetectionGuide, "Public sample pair", "EdgeDetection Learn guide marks the public sample pair.");
        RequireContains(edgeDetectionGuide, "downstream Contour metric", "EdgeDetection Learn guide keeps final evidence downstream.");
        RequireContains(morphologyGuide, "## Beginner path handoff", "Morphology Learn guide includes beginner handoff.");
        RequireContains(morphologyGuide, "Public_Morphology_Cleanup_Good", "Morphology Learn guide names the public Good sample.");
        RequireContains(morphologyGuide, "Public_Morphology_Cleanup_Missing_Bad", "Morphology Learn guide names the public Bad sample.");
        RequireContains(morphologyGuide, "Public sample pair", "Morphology Learn guide marks the public sample pair.");
        RequireContains(morphologyGuide, "before/after output layers", "Morphology Learn guide requires output-layer comparison.");
        RequireContains(blobGuide, "## Blob과 Contour 구분", "Blob Learn guide distinguishes Blob from Contour.");
        RequireContains(blobGuide, "`ResultCount`만으로 끝내지 말고", "Blob Learn guide requires metric gates beyond count.");
        RequireContains(contourGuide, "## Blob과 Contour 구분", "Contour Learn guide distinguishes Contour from Blob.");
        RequireContains(contourGuide, "`DrawMode`, `RetrievalMode`, `MIN_AREA`, `MAX_AREA`", "Contour Learn guide names the first operator checks.");
        RequireContains(learnWindowXaml, "OpenVisionLearnBlobDecisionPanel", "OpenVision Learn Blob topic exposes the Blob decision panel.");
        RequireContains(learnWindowXaml, "판단 기준: Blob = 연결 영역", "OpenVision Learn Blob topic labels the Blob decision rule.");
        RequireContains(learnWindowXaml, "OpenVisionLearnContourDecisionPanel", "OpenVision Learn Contour topic exposes the Contour decision panel.");
        RequireContains(learnWindowXaml, "판단 기준: Contour = 외곽선/모양", "OpenVision Learn Contour topic labels the Contour decision rule.");
        RequireContains(learnWindowXaml, "OpenVisionLearnMatchingFamilyDecisionPanel", "OpenVision Learn Matching topic exposes the Matching family decision panel.");
        RequireContains(learnWindowXaml, "도구 선택: Matching / EdgeBasedMatching / FeatureMatching", "OpenVision Learn Matching-family topics label the tool selection rule.");
        RequireContains(learnWindowXaml, "OpenVisionLearnFeatureMatchingFamilyDecisionPanel", "OpenVision Learn FeatureMatching topic exposes the Matching family decision panel.");
        RequireContains(toolShell, "learnWindowController.Open(LearnTopicIndex)", "PropertyGrid tool Learn buttons delegate the configured Learn topic.");
        RequireContains(doubleInputToolShell, "learnWindowController.Open(LearnTopicIndex)", "Double-input tool Learn buttons delegate the configured Learn topic.");
        RequireContains(toolLearnWindowController, "new OpenVisionLearnWindow(127, 255, false, topicIndex)", "Common Tool Learn controller opens the configured Learn topic.");
        RequireContains(toolLearnWindowController, "learnWindow.Activate()", "Common Tool Learn controller reactivates the existing Learn window.");
        foreach ((string Xaml, string Text, int TopicIndex, string Document) toolLearn in new[]
        {
            (@"UI\VisionTest\Wpf\ToolViews\FilterToolWpfView.xaml", "Learn Filter", 3, "LEARN_FILTER.md"),
            (@"UI\VisionTest\Wpf\ToolViews\MorphologyToolWpfView.xaml", "Learn Morph", 4, "LEARN_MORPHOLOGY.md"),
            (@"UI\VisionTest\Wpf\ToolViews\BlobToolWpfView.xaml", "Learn Blob", 5, "LEARN_BLOB.md"),
            (@"UI\VisionTest\Wpf\ToolViews\ContourToolWpfView.xaml", "Learn Contour", 6, "LEARN_CONTOUR.md"),
            (@"UI\VisionTest\Wpf\ToolViews\LineToolWpfView.xaml", "Learn Line", 8, "LEARN_LINE.md"),
            (@"UI\VisionTest\Wpf\ToolViews\EdgeBasedMatchingToolWpfView.xaml", "Learn Edge Match", 12, "LEARN_EDGE_BASED_MATCHING.md"),
            (@"UI\VisionTest\Wpf\ToolViews\MatchingToolWpfView.xaml", "Learn Matching", 9, "LEARN_MATCHING.md"),
            (@"UI\VisionTest\Wpf\ToolViews\FeatureMatchingToolWpfView.xaml", "Learn Feature", 10, "LEARN_FEATURE_MATCHING.md"),
            (@"UI\VisionTest\Wpf\ToolViews\ArithmeticToolWpfView.xaml", "Learn Arithmetic", 14, "LEARN_ARITHMETIC.md"),
            (@"UI\VisionTest\Wpf\ToolViews\AffineTransformToolWpfView.xaml", "Learn Affine Transform", 15, "LEARN_GEOMETRY_TRANSFORM.md"),
        })
        {
            string toolXaml = Read(repoRoot, toolLearn.Xaml);
            string topicIndex = toolLearn.TopicIndex.ToString(CultureInfo.InvariantCulture);
            RequireContains(toolXaml, "LearnButtonVisibility=\"Visible\"", $"Tool Learn button is visible: {toolLearn.Xaml}.");
            RequireContains(toolXaml, $"LearnButtonText=\"{toolLearn.Text}\"", $"Tool Learn button text is mapped: {toolLearn.Xaml}.");
            RequireContains(toolXaml, $"LearnTopicIndex=\"{topicIndex}\"", $"Tool Learn topic index is mapped: {toolLearn.Xaml}.");
            string expectedEnumName = LearnTopicEnumNameByIndex(learnTopicIndexToEnum, toolLearn.TopicIndex);
            RequireContains(learnTopicsCatalog, $"OpenVisionLearnTopicIndex.{expectedEnumName}", $"Learn topic {topicIndex} maps to enum {expectedEnumName}.");
            RequireContains(learnTopicsCatalog, $"\"{toolLearn.Document}\"", $"Learn topic {topicIndex} resolves {toolLearn.Document}.");
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
            RequireContains(learn, "run Preview or Run Review and compare", $"{matchingGuide.ToolName} Learn document connects parameter changes to review evidence.");
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
        RequireContains(metricsAcceptanceGuide, "## Minimum Tool Gate Cheat Sheet", "Metrics/Acceptance Learn document has the tool gate cheat sheet.");
        RequireContains(metricsAcceptanceGuide, "FeatureMatching | `GoodMatches`, `ScoreMax`, RANSAC/overlay", "Metrics/Acceptance Learn document names the FeatureMatching minimum gates.");
        RequireContains(learnWindowXaml, "OpenVisionLearnMetricGateCheatSheetPanel", "OpenVision Learn Metrics/Acceptance topic exposes the metric gate cheat sheet panel.");
        RequireContains(learnWindowXaml, "Minimum Good/Bad gate cheat sheet", "OpenVision Learn Metrics/Acceptance topic labels the metric gate cheat sheet.");
        RequireContains(metricsAcceptanceGuide, "ResultImageWidth=286", "Metrics/Acceptance Learn document teaches the RotateScale output width gate.");
        RequireContains(metricsAcceptanceGuide, "ResultImageHeight=210", "Metrics/Acceptance Learn document teaches the RotateScale output height gate.");
        RequireContains(metricsAcceptanceGuide, "Transform samples may be Good-only", "Metrics/Acceptance Learn document explains Good-only transform benchmarks.");
        RequireContains(metricsAcceptanceGuide, "Bad sample fails for the intended metric", "Metrics/Acceptance Learn document distinguishes intended metric failure from setup failure.");
        RequireNotContains(metricsAcceptanceGuide, "Public_Template_Circle", "Metrics/Acceptance Learn document must not reference non-catalog sample names.");
        RequireContains(learnTopicsCatalog, "Metrics / Acceptance", "OpenVision Learn topic list exposes Metrics / Acceptance.");
        RequireContains(learnTopicsCatalog, "OpenVisionLearnTopicIndex.MetricsAcceptance", "OpenVision Learn topic 13 resolves Metrics/Acceptance via enum mapping.");
        RequireContains(learnTopicsCatalog, "\"LEARN_METRICS_ACCEPTANCE.md\"", "OpenVision Learn topic 13 resolves Metrics/Acceptance document.");
        RequireContains(learnTopicsCatalog, "Metrics/Acceptance 기준", "OpenVision Learn topic 13 exposes Metrics/Acceptance practice guidance.");
        RequireContains(learnWindow, "metricsAcceptanceTopicPanel", "OpenVision Learn has a visible Metrics/Acceptance topic panel.");

        string pipelineLayerRoutingGuide = Read(repoRoot, @"docs\learn\LEARN_PIPELINE_LAYER_ROUTING.md");
        RequireContains(pipelineLayerRoutingGuide, "## Building A Route", "Pipeline/Layer Learn document explains how to build a route.");
        RequireContains(pipelineLayerRoutingGuide, "`InputLayer` is the source image", "Pipeline/Layer Learn document explains InputLayer.");
        RequireContains(pipelineLayerRoutingGuide, "`OutputLayer` is the produced result", "Pipeline/Layer Learn document explains OutputLayer.");
        RequireContains(pipelineLayerRoutingGuide, "select that previous `OutputLayer` as its `InputLayer`", "Pipeline/Layer Learn document explains how consecutive steps are connected.");
        RequireContains(pipelineLayerRoutingGuide, "Click Preview or Run Review", "Pipeline/Layer Learn document gives the operator the next review action.");
        RequireContains(pipelineLayerRoutingGuide, "## Operator Route Review Loop", "Pipeline/Layer Learn document has the operator route review loop.");
        RequireContains(pipelineLayerRoutingGuide, "inspect the previous `OutputLayer` first", "Pipeline/Layer Learn document explains how to locate the first changed result.");
        RequireContains(learnWindowXaml, "OpenVisionLearnLayerRoutingSafetyPanel", "OpenVision Learn exposes the route safety checklist panel.");
        RequireContains(learnWindowXaml, "Layer 연결 원리", "OpenVision Learn topic 11 explains layer routing.");
        RequireContains(learnWindowXaml, "OpenVisionLearnLayerRouteReviewLoopPanel", "OpenVision Learn exposes the operator route review loop panel.");
        RequireContains(learnWindowXaml, "Pipeline 검토 순서", "OpenVision Learn topic 11 shows the operator route review loop.");

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
        RequireContains(learnWindowXaml, "EdgeDetection에서는 경계 픽셀", "OpenVision Learn Edge / Line topic distinguishes edge and line evidence.");
        RequireContains(learnScreenshotSmoke, "\"EdgeDetection\", \"LineDistance\"", "Learn screenshot smoke verifies Edge / Line role-map guidance.");
        RequireContains(learnScreenshotSmoke, "OpenVisionLearnEdgeDetectionPracticePanel", "Learn screenshot smoke verifies the EdgeDetection public practice panel.");
        RequireContains(learnScreenshotSmoke, "Public_EdgeDetection_Shapes_Good", "Learn screenshot smoke verifies the EdgeDetection public Good/Bad pair.");
        RequireContains(learnScreenshotSmoke, "OpenVisionLearnLineDistancePracticePanel", "Learn screenshot smoke verifies the LineDistance public practice panel.");
        RequireContains(learnScreenshotSmoke, "Public_Line_Pins_Good", "Learn screenshot smoke verifies the LineDistance public Good/Bad pair.");
        RequireContains(learnScreenshotSmoke, "DistanceMmMax", "Learn screenshot smoke verifies LineDistance max/outlier guidance.");
        RequireContains(learnWindowXaml, "판정 기준: DistanceMmAvg", "OpenVision Learn LineDistance topic explains average plus consistency gates.");
        string lineGuide = Read(repoRoot, @"docs\learn\LEARN_LINE.md");
        RequireContains(lineGuide, "LineDistance outlier gate", "Line Learn document explains the LineDistance outlier gate.");
        RequireContains(lineGuide, "`DistanceMmRange`, `DistancePxRange`, `DistanceMmMax`, or `DistancePxMax`", "Line Learn document requires consistency/outlier metrics.");
        RequireContains(lineGuide, "Practice Samples path: `line`", "Line Learn document names the Line practice path.");
        RequireContains(lineGuide, "Public_Line_Pins_Good", "Line Learn document names the public Good sample.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_arithmetic", "Learn screenshot smoke exposes the Arithmetic topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnArithmetic", "Learn screenshot smoke verifies the Arithmetic topic.");
        RequireContains(learnScreenshotSmoke, "wpf_arithmetic_tool_learn_button", "Learn screenshot smoke exposes the Arithmetic Tool View Learn button target.");
        RequireContains(learnScreenshotSmoke, "CaptureArithmeticToolLearnButton", "Learn screenshot smoke verifies the Arithmetic Tool View Learn button.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_geometry", "Learn screenshot smoke exposes the Geometry topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnGeometry", "Learn screenshot smoke verifies the Geometry topic.");
        RequireContains(learnScreenshotSmoke, "wpf_openvision_learn_color_hsv", "Learn screenshot smoke exposes the Color / HSV topic target.");
        RequireContains(learnScreenshotSmoke, "CaptureOpenVisionLearnColorHsv", "Learn screenshot smoke verifies the Color / HSV topic.");
        RequireContains(learnTopicsCatalog, "Arithmetic / Logic", "OpenVision Learn topic list exposes Arithmetic / Logic.");
        RequireContains(learnTopicsCatalog, "OpenVisionLearnTopicIndex.Arithmetic", "OpenVision Learn topic 14 resolves Arithmetic via enum mapping.");
        RequireContains(learnTopicsCatalog, "\"LEARN_ARITHMETIC.md\"", "OpenVision Learn topic 14 resolves Arithmetic document.");
        RequireContains(learnTopicsCatalog, "Geometry Transform", "OpenVision Learn topic list exposes Geometry Transform.");
        RequireContains(learnTopicsCatalog, "OpenVisionLearnTopicIndex.GeometryTransform", "OpenVision Learn topic 15 resolves Geometry via enum mapping.");
        RequireContains(learnTopicsCatalog, "\"LEARN_GEOMETRY_TRANSFORM.md\"", "OpenVision Learn topic 15 resolves Geometry document.");
        RequireContains(learnTopicsCatalog, "Color / HSV", "OpenVision Learn topic list exposes Color / HSV.");
        RequireContains(learnTopicsCatalog, "OpenVisionLearnTopicIndex.ColorHsv", "OpenVision Learn topic 16 resolves Color / HSV via enum mapping.");
        RequireContains(learnTopicsCatalog, "\"LEARN_COLOR_HSV.md\"", "OpenVision Learn topic 16 resolves Color / HSV document.");
        RequireContains(learnTopicsCatalog, "\"color-hsv\"", "OpenVision Learn topic 16 opens the Color / HSV practice path.");
        RequireContains(learnTopicsCatalog, "HSV 색상 샘플", "OpenVision Learn Color / HSV practice text describes the sample intent.");
        RequireContains(learnTopicsCatalog, "MaskPixelRatio", "OpenVision Learn Color / HSV practice text names the review metric.");
        RequireContains(learnWindowXaml, "OpenVisionLearnColorSampleEvidence", "OpenVision Learn Color / HSV topic explains the current public HSV Good/Bad sample evidence.");
        RequireContains(learnWindowXaml, "공개 HSV 실습 샘플", "OpenVision Learn Color / HSV topic labels the public HSV sample evidence.");
        RequireContains(learnWindowXaml, "Public_HSV_ColorPatch_Good", "OpenVision Learn Color / HSV topic names the public HSV Good sample.");
        RequireContains(learnWindowXaml, "MaskPixelRatio", "OpenVision Learn Color / HSV topic presents MaskPixelRatio as the current HSV metric.");
        RequireContains(learnScreenshotSmoke, "OpenVisionLearnColorSampleEvidence", "Learn screenshot smoke verifies the Color / HSV sample evidence panel.");
        RequireContains(learnScreenshotSmoke, "metric=", "Learn screenshot smoke verifies MaskPixelRatio is presented as a current metric.");
        string colorHsvGuide = Read(repoRoot, @"docs\learn\LEARN_COLOR_HSV.md");
        RequireContains(colorHsvGuide, "Practice Samples path: `color-hsv`", "Color / HSV Learn guide names the Color / HSV practice path.");
        RequireContains(colorHsvGuide, "Public_HSV_ColorPatch_Good", "Color / HSV Learn guide names the public HSV Good sample.");
        RequireContains(colorHsvGuide, "Public_HSV_ColorPatch_Missing_Bad", "Color / HSV Learn guide names the public HSV Bad sample.");
        RequireContains(colorHsvGuide, "color-mask coverage with `MaskPixelRatio`", "Color / HSV Learn guide connects MaskPixelRatio to the public sample evidence.");
        RequireContains(colorHsvGuide, "## HSV Parameters And Output", "Color / HSV Learn guide explains parameters and output.");
        RequireContains(colorHsvGuide, "`HueMin`, `HueMax`, `SaturationMin`, `SaturationMax`, `ValueMin`, `ValueMax`", "Color / HSV Learn guide defines required HSV range parameters.");
        RequireContains(colorHsvGuide, "`InputLayer` and `OutputLayer`", "Color / HSV Learn guide keeps HSV output mask routing explicit.");
        RequireContains(colorHsvGuide, "`MaskPixelCount`", "Color / HSV Learn guide requires count metrics before sample promotion.");
        RequireContains(colorHsvGuide, "bounded `MaskPixelRatio` range", "Color / HSV Learn guide requires bounded mask-ratio acceptance.");
        RequirePathExists(repoRoot, @"Core\Pipeline\Tools\VisionPipelineHsvMaskTool.cs", "HSV pipeline runner tool exists.");
        string hsvPipelineTool = Read(repoRoot, @"Core\Pipeline\Tools\VisionPipelineHsvMaskTool.cs");
        RequireContains(hsvPipelineTool, "VisionPipelineHsvMaskTool", "HSV pipeline runner tool is implemented.");
        RequireContains(hsvPipelineTool, "ColorConversionCodes.BGR2HSV", "HSV pipeline runner converts BGR input to HSV.");
        RequireContains(hsvPipelineTool, "MaskPixelRatio", "HSV pipeline runner reports mask ratio.");
        string pipelineKnownMetrics = Read(repoRoot, @"Core\Pipeline\Validation\VisionPipelineKnownMetrics.cs");
        RequireContains(pipelineKnownMetrics, "public const string MaskPixelCount", "Known metrics include MaskPixelCount.");
        RequireContains(pipelineKnownMetrics, "public const string MaskPixelRatio", "Known metrics include MaskPixelRatio.");
        RequireContains(pipelineKnownMetrics, "[\"hsv\"]", "Known metrics map HSV ToolType.");
        string pipelineValidator = Read(repoRoot, @"Core\Pipeline\Validation\VisionPipelineValidation.cs");
        RequireContains(pipelineValidator, "\"hsv\"", "Pipeline validator supports HSV ToolType.");
        RequireContains(pipelineValidator, "Hue is circular", "Pipeline validator documents circular HSV hue validation.");
        RequireNotContains(pipelineValidator, "ValidateMinMax(result, label, step, \"HueMin\", \"HueMax\")", "Pipeline validator allows circular HSV hue ranges.");
        string llmToolCatalog = Read(repoRoot, @"docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json");
        RequireContains(llmToolCatalog, "\"toolType\": \"HSV\"", "LLM tool catalog exposes the HSV ToolType.");
        RequireContains(llmToolCatalog, "\"MaskPixelRatio\"", "LLM tool catalog exposes the HSV mask-ratio metric.");
        RequireContains(llmToolCatalog, "HueMin greater than HueMax intentionally wraps", "LLM tool catalog documents the HSV hue-wrap contract.");
        string llmAuthoringGuide = Read(repoRoot, @"docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md");
        RequireContains(llmAuthoringGuide, "### HSV Color Mask", "LLM XML authoring guide includes the HSV color-mask pattern.");
        RequireContains(llmAuthoringGuide, "`HSV`, `HsvMask`, `ColorHSV`, `ColorMask`", "LLM XML authoring guide lists HSV aliases.");
        RequireContains(llmAuthoringGuide, "`HueMin > HueMax` intentionally wraps", "LLM XML authoring guide documents the HSV hue-wrap contract.");
        RequirePathExists(repoRoot, @"Core\Pipeline\Tools\VisionPipelineReferenceDifferenceTool.cs", "ReferenceDifference pipeline runner tool exists.");
        string referenceDifferenceTool = Read(repoRoot, @"Core\Pipeline\Tools\VisionPipelineReferenceDifferenceTool.cs");
        RequireContains(referenceDifferenceTool, "Cv2.FindHomography", "ReferenceDifference registers approved Good references geometrically.");
        RequireContains(referenceDifferenceTool, "DifferencePixelRatio", "ReferenceDifference reports localized difference evidence.");
        RequireContains(referenceDifferenceTool, "regions.Count == 0 ? 0", "ReferenceDifference treats zero detected regions as measurable evidence.");
        string appToolFactory = Read(repoRoot, @"Core\Pipeline\Definition\VisionPipelineAppToolFactory.cs");
        RequireContains(appToolFactory, "CreateReferenceDifferenceTool", "Pipeline factory creates ReferenceDifference with resolved dependencies.");
        RequireContains(appToolFactory, "ReferencePath\" + index", "ReferenceDifference resolves each imported reference path independently.");
        string stepPropertyMapper = Read(repoRoot, @"UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineStepPropertyMapper.cs");
        string referenceDifferencePropertyAdapter = Read(
            repoRoot,
            @"UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineReferenceDifferencePropertyAdapter.cs");
        RequireContains(
            stepPropertyMapper,
            "VisionPipelineReferenceDifferencePropertyAdapter.TryCreateProperty",
            "Pipeline Step PropertyGrid dispatches ReferenceDifference through its adapter.");
        RequireContains(
            referenceDifferencePropertyAdapter,
            "ReferenceDifferenceProperty",
            "Pipeline Step PropertyGrid supports ReferenceDifference.");
        RequireContains(
            referenceDifferencePropertyAdapter,
            "ReferencePath4",
            "ReferenceDifference PropertyGrid exposes up to four approved Good references.");
        string pinArrayGapPropertyAdapter = Read(
            repoRoot,
            @"UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelinePinArrayGapPropertyAdapter.cs");
        RequireContains(
            stepPropertyMapper,
            "VisionPipelinePinArrayGapPropertyAdapter.TryCreateProperty",
            "Pipeline Step PropertyGrid dispatches PinArrayGap through its adapter.");
        RequireNotContains(
            stepPropertyMapper,
            "private sealed class PipelinePinArrayGapProperty",
            "The root mapper no longer owns the PinArrayGap PropertyGrid model.");
        RequireContains(
            pinArrayGapPropertyAdapter,
            "\"adjacentpingap\"",
            "The PinArrayGap adapter preserves the AdjacentPinGap alias.");
        RequireContains(
            pinArrayGapPropertyAdapter,
            "baselineParameters",
            "The PinArrayGap adapter preserves unrepresented Step parameters.");
        RequireContains(
            pinArrayGapPropertyAdapter,
            "MinimumDarkCoverageRatio",
            "The PinArrayGap adapter exposes its reviewed detection fields.");
        string linePairPropertyAdapter = Read(
            repoRoot,
            @"UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineLinePairPropertyAdapter.cs");
        RequireContains(
            stepPropertyMapper,
            "VisionPipelineLinePairPropertyAdapter.TryCreateProperty",
            "Pipeline Step PropertyGrid dispatches Line Pair mapping through its adapter.");
        RequireNotContains(
            stepPropertyMapper,
            "case \"linedistance\"",
            "The root mapper no longer owns the LineDistance ToolType case.");
        RequireContains(
            linePairPropertyAdapter,
            "internal static class VisionPipelineLinePairPropertyAdapter",
            "Line Pair mapping has a standalone non-partial owner.");
        RequireContains(
            linePairPropertyAdapter,
            "TryCreateLineGaugePair",
            "The Line Pair adapter preserves the Tool View edit handoff.");
        RequireNotContains(
            linePairPropertyAdapter,
            "PipelineGeometryPropertyBase",
            "The Line Pair adapter no longer owns the Geometry property base.");
        string geometryPropertyAdapter = Read(
            repoRoot,
            @"UI\Menu\Wpf\Recipe\PropertyGrid\VisionPipelineGeometryPropertyAdapter.cs");
        RequireContains(
            stepPropertyMapper,
            "VisionPipelineGeometryPropertyAdapter.TryCreateProperty",
            "Pipeline Step PropertyGrid dispatches Geometry mapping through its adapter.");
        RequireNotContains(
            stepPropertyMapper,
            "private abstract class PipelineGeometryPropertyBase",
            "The root mapper no longer owns the Geometry property base.");
        RequireNotContains(
            stepPropertyMapper,
            "case \"geometrymeasure\"",
            "The root mapper no longer owns the GeometryMeasure ToolType case.");
        RequireContains(
            geometryPropertyAdapter,
            "internal static class VisionPipelineGeometryPropertyAdapter",
            "Geometry mapping has a standalone non-partial owner.");
        RequireContains(
            geometryPropertyAdapter,
            "private abstract class GeometryPropertyBase",
            "The Geometry adapter owns the shared Geometry property base.");
        RequireContains(
            geometryPropertyAdapter,
            "GeometryFeatureConverter",
            "The Geometry adapter owns typed feature selection.");
        RequireContains(
            geometryPropertyAdapter,
            "CircleGaugeProperty",
            "The Geometry adapter owns CircleGauge PropertyGrid mapping.");
        RequireContains(
            learnScreenshotSmoke,
            "wpf_shell_host_recipe_line_pair_properties",
            "Screenshot smoke covers Line Pair PropertyGrid round trip.");
        RequireContains(
            learnScreenshotSmoke,
            "p213_geometry_property_grid",
            "Screenshot smoke covers Geometry PropertyGrid mapping.");
        RequireContains(
            learnScreenshotSmoke,
            "p213_geometry_review",
            "Screenshot smoke covers GeometryMeasure and CircleGauge core behavior.");
        RequireContains(pipelineValidator, "ValidateReferenceDifferenceParameters", "Pipeline validator checks ReferenceDifference parameters.");
        RequireContains(pipelineKnownMetrics, "DifferencePixelRatio", "Known metrics include ReferenceDifference coverage.");
        RequireContains(pipelineKnownMetrics, "RegistrationInlierRatio", "Known metrics include ReferenceDifference registration quality.");
        RequireContains(llmToolCatalog, "\"toolType\": \"ReferenceDifference\"", "LLM tool catalog exposes ReferenceDifference.");
        RequireContains(llmAuthoringGuide, "`ReferenceDifference` requires an existing `ReferencePath1`", "LLM authoring guide documents ReferenceDifference dependency inputs.");
        RequireContains(learnScreenshotSmoke, "wpf_shell_host_recipe_reference_difference_properties", "Screenshot smoke covers the ReferenceDifference PropertyGrid.");
        string geometryGuide = Read(repoRoot, @"docs\learn\LEARN_GEOMETRY_TRANSFORM.md");
        RequireContains(geometryGuide, "Public_Geometry_RotateScale_Good", "Geometry Learn guide uses the public RotateScale sample.");
        RequireContains(geometryGuide, "Public_Geometry_RotateScale_Wide_Bad", "Geometry Learn guide uses the public RotateScale Bad sample.");
        RequireContains(geometryGuide, "ResultImageWidth=320", "Geometry Learn guide explains the Bad sample output-size drift.");
        RequireContains(geometryGuide, @"docs\samples\public\Geometry_RotateScale_Synthetic_OK.png", "Geometry Learn guide points to a public-safe synthetic image.");
        RequireContains(geometryGuide, @"docs\samples\public\Geometry_RotateScale_Synthetic_Wide_NG.png", "Geometry Learn guide points to the public-safe wide negative image.");
        RequireNotContains(geometryGuide, @"Sample\Contour.jpg", "Geometry Learn guide must not depend on the local legacy root Sample image.");
        RequireContains(geometryGuide, "AffineTransform", "Geometry Learn guide teaches the three-point AffineTransform path.");
        RequireContains(geometryGuide, "AffineValidPixelRatio", "Geometry Learn guide teaches the Affine valid-pixel gate.");
        RequireContains(pipelineKnownMetrics, "public const string AffineM11", "Known metrics include the Affine matrix.");
        RequireContains(pipelineKnownMetrics, "public const string AffineValidPixelRatio", "Known metrics include Affine retained-source coverage.");
        RequireContains(pipelineKnownMetrics, "public const string AffineDetectedSourcePointCount", "Known metrics include dynamic Affine source-point resolution.");
        RequireContains(pipelineValidator, "ValidateAffineParameters", "Pipeline validator checks Affine point geometry and gates.");
        RequireContains(stepPropertyMapper, "PipelineAffineTransformToolProperty", "Pipeline selected-Step PropertyGrid supports AffineTransform.");
        RequireContains(stepPropertyMapper, "UseDetectedSourcePoints", "AffineTransform PropertyGrid supports explicit typed Point source binding.");
        RequireContains(stepPropertyMapper, "SourcePoint3Feature", "AffineTransform PropertyGrid exposes all three ordered typed Point references.");
        RequireContains(llmToolCatalog, "\"toolType\": \"AffineTransform\"", "Tool catalog exposes AffineTransform and its aliases.");
        RequireContains(llmToolCatalog, "\"USE_DETECTED_SOURCE_POINTS\"", "Tool catalog exposes detected-source Point binding without changing fixed numeric defaults.");
        RequireContains(llmToolCatalog, "\"AffineDetectedSourcePointCount\"", "Tool catalog exposes dynamic Affine source-point evidence metrics.");
        RequireContains(llmAuthoringGuide, "### Three-Point Pixel Mapping With AffineTransform", "XML authoring guide includes the AffineTransform pattern.");
        RequireContains(llmAuthoringGuide, "<Key>SOURCE_POINT_3_FEATURE</Key>", "XML authoring guide includes the ordered detected Point binding pattern.");
        RequireContains(geometryGuide, "### Same-Run Detected Source Points", "Geometry Learn guide teaches detected Point to Affine normalization.");
        RequireContains(learnScreenshotSmoke, "wpf_shell_host_affine_transform_tool", "Screenshot smoke covers the AffineTransform PropertyGrid Tool View.");

        string thresholdToolXaml = Read(repoRoot, @"UI\VisionTest\Wpf\ToolViews\ThresholdToolWpfView.xaml");
        RequireContains(thresholdToolXaml, "ThresholdToolLearnButton", "Threshold Tool exposes its compact Learn entry.");
        RequireContains(thresholdToolXaml, "Learn Threshold", "Threshold Tool Learn entry is labelled for Threshold.");
        RequireContains(learnWindow, ": this(threshold, maxValue, invert, 2)", "Threshold Tool Learn entry opens the Threshold topic.");
        RequireContains(learnTopicsCatalog, "OpenVisionLearnTopicIndex.Threshold", "Threshold Learn topic resolves LEARN_THRESHOLD.md via enum mapping.");
        RequireContains(learnTopicsCatalog, "\"LEARN_THRESHOLD.md\"", "Threshold Learn topic resolves LEARN_THRESHOLD.md.");
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
        RequireContains(external, @"dll\Library-Noah", "DLL reference policy covers vendored Library-Noah DLLs.");
        RequireContains(external, @"dll\OpenCVSharp", "DLL reference policy covers shared OpenCVSharp native runtime.");
        RequireContains(external, "System.Windows.Controls.WpfPropertyGrid.dll", "DLL reference policy covers WPG runtime DLL.");

        string release = Read(repoRoot, @"docs\OPENVISIONLAB_RELEASE_VERSION_POLICY.md");
        RequireContains(release, @"dll\Library-Noah", "Release policy covers Library-Noah DLL versioning.");
        RequireContains(release, @"dll\OpenCVSharp", "Release policy covers shared OpenCVSharp native runtime.");
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

    private static Dictionary<int, string> BuildLearnTopicIndexToEnumMap(string learnTopicsCatalog)
    {
        if (string.IsNullOrWhiteSpace(learnTopicsCatalog))
        {
            return new Dictionary<int, string>();
        }

        Dictionary<string, int> enumToIndex = new();
        Dictionary<string, string> unresolvedAliases = new();
        Regex enumLineRegex = new(@"^\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>[^,]+)\s*,", RegexOptions.Multiline);

        foreach (Match match in enumLineRegex.Matches(learnTopicsCatalog))
        {
            string name = match.Groups["name"].Value;
            string value = match.Groups["value"].Value.Trim();
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                enumToIndex[name] = index;
            }
            else if (!string.IsNullOrWhiteSpace(value))
            {
                unresolvedAliases[name] = value;
            }
        }

        bool changed = true;
        while (changed && unresolvedAliases.Count > 0)
        {
            changed = false;
            foreach (KeyValuePair<string, string> alias in unresolvedAliases.ToList())
            {
                if (enumToIndex.TryGetValue(alias.Value, out int resolvedIndex))
                {
                    enumToIndex[alias.Key] = resolvedIndex;
                    unresolvedAliases.Remove(alias.Key);
                    changed = true;
                }
            }
        }

        Dictionary<int, string> indexToEnum = new();
        Regex metadataRegex = new(@"OpenVisionLearnTopicMetadata\(\s*OpenVisionLearnTopicIndex\.(?<enumName>[A-Za-z_][A-Za-z0-9_]*)\s*,", RegexOptions.Multiline);
        foreach (Match match in metadataRegex.Matches(learnTopicsCatalog))
        {
            string enumName = match.Groups["enumName"].Value;
            if (enumToIndex.TryGetValue(enumName, out int index) && !indexToEnum.ContainsKey(index))
            {
                indexToEnum[index] = enumName;
            }
        }

        return indexToEnum;
    }

    private static string LearnTopicEnumNameByIndex(Dictionary<int, string> learnTopicIndexToEnum, int topicIndex)
    {
        if (learnTopicIndexToEnum.TryGetValue(topicIndex, out string enumName))
        {
            return enumName;
        }

        return "Unknown";
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

        for (int redirectCount = 0; redirectCount < 4; redirectCount++)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            if (!TryResolveMovedDocumentPath(repoRoot, path, text, out string redirectedPath))
            {
                return text;
            }

            path = redirectedPath;
        }

        return File.ReadAllText(path, Encoding.UTF8);
    }

    private static string ReadSourceFamily(string repoRoot, string relativePath)
    {
        string path = Path.Combine(repoRoot, relativePath);
        string directory = Path.GetDirectoryName(path) ?? string.Empty;
        string fileName = Path.GetFileNameWithoutExtension(path);
        string[] paths = Directory.Exists(directory)
            ? Directory.GetFiles(directory, fileName + "*.cs").OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToArray()
            : Array.Empty<string>();
        if (paths.Length == 0)
        {
            Failures.Add(string.Create(CultureInfo.InvariantCulture, $"Required source family not found: {path}"));
            return string.Empty;
        }

        return string.Join(Environment.NewLine, paths.Select(item => File.ReadAllText(item, Encoding.UTF8)));
    }

    private static bool TryResolveMovedDocumentPath(string repoRoot, string currentPath, string text, out string redirectedPath)
    {
        redirectedPath = string.Empty;
        if (string.IsNullOrWhiteSpace(text)
            || (text.IndexOf("Moved to canonical location", StringComparison.OrdinalIgnoreCase) < 0
                && text.IndexOf("이동 안내", StringComparison.OrdinalIgnoreCase) < 0))
        {
            return false;
        }

        Match link = Regex.Matches(text, @"\]\((?<path>[^)]+)\)").Cast<Match>().LastOrDefault();
        if (link == null)
        {
            return false;
        }

        string target = link.Groups["path"].Value.Replace('/', Path.DirectorySeparatorChar);
        string candidate = target.StartsWith("docs" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(repoRoot, target)
            : Path.Combine(Path.GetDirectoryName(currentPath) ?? repoRoot, target);
        if (!File.Exists(candidate))
        {
            return false;
        }

        redirectedPath = candidate;
        return true;
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
