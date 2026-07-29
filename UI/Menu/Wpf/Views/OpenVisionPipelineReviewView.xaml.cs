using OpenVisionLab.Pipeline.Controls;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Result;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public partial class OpenVisionPipelineReviewView : UserControl
    {
        private IReadOnlyList<VisionPipelineObjectResult> objectResults = Array.Empty<VisionPipelineObjectResult>();
        private Bitmap objectResultBaseImage;
        private Bitmap objectMetricSourceImage;
        private VisionPipelineStep objectMetricStep;
        private VisionPipelineObjectMetricKind objectMetricKind = VisionPipelineObjectMetricKind.Area;
        private VisionPipelineObjectMetricDistribution objectMetricDistribution;
        private bool suppressObjectSelection;
        private IReadOnlyList<VisionPipelineInstanceResult> instanceResults =
            Array.Empty<VisionPipelineInstanceResult>();
        private bool suppressInstanceSelection;
        private IReadOnlyList<VisionPipelineGeometryFeatureResult> geometryResults = Array.Empty<VisionPipelineGeometryFeatureResult>();
        private bool suppressGeometrySelection;
        private VisionPipelineCircleEvidence circleEvidence;
        private IReadOnlyList<VisionPipelineCircleSampleEvidence> circleSamples =
            Array.Empty<VisionPipelineCircleSampleEvidence>();
        private Bitmap circleSourceImage;
        private VisionToolSignalEvidence circleResidualSignal;
        private bool suppressCircleSelection;
        private bool showCircleProfile;
        private OpenVisionPipelineReviewMatcherDiagnosticState matcherDiagnosticState;
        private IReadOnlyList<VisionPipelineGeometryFeatureResult> scaleCalibrationPoints = Array.Empty<VisionPipelineGeometryFeatureResult>();
        private Bitmap scaleCalibrationBaseImage;
        private bool suppressScaleCalibrationSelection;
        private bool suppressFixtureConsumerSelection;
        private bool hasScaleCalibrationRecord;

        public OpenVisionPipelineReviewView()
        {
            InitializeComponent();
            ViewModel = new OpenVisionPipelineReviewViewModel();
            DataContext = ViewModel;
            pipelineFlowView.StepSelected += OnPipelineFlowStepSelected;
            objectMetricPlot.SampleSelectionRequested += ObjectMetricPlot_SampleSelectionRequested;
            circleEvidencePlot.SampleSelectionRequested += CircleEvidencePlot_SampleSelectionRequested;
            cmbScaleUnit.ItemsSource = new[]
            {
                new VisionScaleCalibrationUnitOption(VisionScaleCalibrationUnit.Millimeter, "mm"),
                new VisionScaleCalibrationUnitOption(VisionScaleCalibrationUnit.Micrometer, "µm"),
                new VisionScaleCalibrationUnitOption(VisionScaleCalibrationUnit.Inch, "inch")
            };
            cmbScaleUnit.DisplayMemberPath = nameof(VisionScaleCalibrationUnitOption.DisplayText);
            cmbScaleUnit.SelectedIndex = 0;
            ApplyLocalization();
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            Unloaded += OnUnloaded;
        }

        public event EventHandler<PipelineFlowStepSelectedEventArgs> StepSelected = delegate { };
        public event EventHandler RunReviewRequested = delegate { };
        public event EventHandler PreviousStepRequested = delegate { };
        public event EventHandler NextStepRequested = delegate { };
        public event EventHandler FirstIssueStepRequested = delegate { };
        public event EventHandler OpenPairSampleRequested = delegate { };
        public event EventHandler UseSelectedMatchingPoseRequested = delegate { };
        public event EventHandler ReturnToRecipeRequested = delegate { };
        public event EventHandler OpenSelectedToolLearnRequested = delegate { };
        public event EventHandler EditSelectedStepRequested = delegate { };
        public event EventHandler EditFixtureProducerRequested = delegate { };
        public event EventHandler EditFixtureMeasurementRequested = delegate { };
        public event EventHandler<OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs> FixtureConsumerSelected = delegate { };
        public event EventHandler<VisionScaleCalibrationRequestedEventArgs> ScaleCalibrationRequested = delegate { };
        public event EventHandler<VisionScaleCalibrationApplyRequestedEventArgs> ScaleCalibrationApplyRequested = delegate { };

        public OpenVisionPipelineReviewViewModel ViewModel { get; }
        public string SelectedStepText => ViewModel.SelectedStepText;
        public string SelectedToolText => ViewModel.SelectedToolText;
        public string SelectedStatusText => ViewModel.SelectedStatusText;
        public string RecipeContextText => ViewModel.RecipeContextText;
        public string ReviewProgressText => ViewModel.ReviewProgressText;
        public string FlowSummaryText => ViewModel.FlowSummaryText;
        public string ParameterSummaryText => ViewModel.ParameterSummaryText;
        public string ValidationStatusText => ViewModel.ValidationStatusText;
        public string ValidationDetailText => ViewModel.ValidationDetailText;
        public string ResultSummaryText => ViewModel.ResultSummaryText;
        public string ResultDetailText => ViewModel.ResultDetailText;
        public string RunLogText => ViewModel.RunLogText;
        public string ReadinessSummaryText => ViewModel.ReadinessSummaryText;
        public string ReviewGuideStageText => ViewModel.ReviewGuideStageText;
        public string ReviewGuideCurrentStepText => ViewModel.ReviewGuideCurrentStepText;
        public string ReviewGuideNextActionText => ViewModel.ReviewGuideNextActionText;
        public string ReviewGuideResultDecisionText => ViewModel.ReviewGuideResultDecisionText;
        public string ReviewGuideDetailText => ViewModel.ReviewGuideDetailText;
        public string ReviewGuidePairText => ViewModel.ReviewGuidePairText;
        public string ReviewGuidePairActionText => ViewModel.ReviewGuidePairActionText;
        public string ReviewGuidePairMetricText => ViewModel.ReviewGuidePairMetricText;
        public string ReviewGuideChecklistText => ViewModel.ReviewGuideChecklistText;
        public string ReviewGuideParameterFocusText => ViewModel.ReviewGuideParameterFocusText;
        public string ReviewGuideTriageFailureText => ViewModel.ReviewGuideTriageFailureText;
        public string ReviewGuideTriageAdjustmentText => ViewModel.ReviewGuideTriageAdjustmentText;
        public string ReviewGuideTriageRerunText => ViewModel.ReviewGuideTriageRerunText;
        public bool CanOpenReviewGuidePairAction => ViewModel.CanOpenReviewGuidePairAction;
        public bool CanSelectPreviousStep => ViewModel.CanSelectPreviousStep;
        public bool CanSelectNextStep => ViewModel.CanSelectNextStep;
        public bool CanSelectFirstIssueStep => ViewModel.CanSelectFirstIssueStep;
        public bool HasInputPreview => ViewModel.HasInputPreview;
        public bool HasOutputPreview => ViewModel.HasOutputPreview;
        public int ObjectResultCount => objectResults.Count;
        public int SelectedObjectResultNumber => objectResultsGrid.SelectedItem is VisionPipelineObjectResult item ? item.Number : 0;
        internal int InstanceResultCountForTest => instanceResults.Count;
        internal string SelectedInstanceIdForTest =>
            instanceResultsGrid.SelectedItem is VisionPipelineInstanceResult item
                ? item.InstanceId
                : string.Empty;
        internal void SelectInstanceForTest(int index)
        {
            if (index < 0 || index >= instanceResults.Count)
            {
                return;
            }

            VisionPipelineInstanceResult item = instanceResults[index];
            instanceResultsGrid.SelectedItem = item;
            instanceResultsGrid.ScrollIntoView(item);
        }
        public bool HasObjectHighlight { get; private set; }
        public int SelectedFlowIndex => pipelineFlowView.SelectedIndex;
        public bool IsFixtureDesignerVisible => ViewModel.IsFixtureDesignerVisible;
        public string FixtureRelationshipText => ViewModel.FixtureRelationshipText;
        public string ScaleCalibrationStatusText => txtScaleCalibrationStatus.Text;
        public string ScaleCalibrationResultText => txtScaleCalibrationResult.Text;

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            Unloaded -= OnUnloaded;
            objectResultBaseImage?.Dispose();
            objectResultBaseImage = null;
            objectMetricSourceImage?.Dispose();
            objectMetricSourceImage = null;
            circleSourceImage?.Dispose();
            circleSourceImage = null;
            matcherDiagnosticState?.Dispose();
            matcherDiagnosticState = null;
            scaleCalibrationBaseImage?.Dispose();
            scaleCalibrationBaseImage = null;
        }

        private void ApplyLocalization()
        {
            lblStepFlow.Text = T("PipelineReview.StepFlow", "Step Flow");
            lblStep.Text = T("PipelineReview.Step", "Step");
            lblRoute.Text = T("PipelineReview.Route", "Route");
            lblState.Text = T("PipelineReview.Validation", "Validation");
            lblTopResult.Text = T("PipelineReview.Result", "Result");
            lblInput.Text = T("PipelineReview.Input", "Input");
            lblOutput.Text = T("PipelineReview.Output", "Output");
            lblFlow.Text = T("PipelineReview.Flow", "Flow");
            lblParameters.Text = T("PipelineReview.Parameters", "Parameters");
            lblValidation.Text = T("PipelineReview.Validation", "Validation");
            lblResult.Text = T("PipelineReview.Result", "Result");
            lblRunLog.Text = T("PipelineReview.RunLog", "Run Log");
            objectInspectorTab.Header = T("PipelineReview.ObjectInspector.Title", "Object Results");
            instanceInspectorTab.Header = T("PipelineReview.InstanceInspector.Title", "Instance Results");
            geometryReviewTab.Header = T("PipelineReview.GeometryReview.Title", "Geometry Review");
            circleEvidenceTab.Header = T("PipelineReview.CircleEvidence.Title", "Circle Evidence");
            matcherDiagnosticTab.Header = T("PipelineReview.MatcherDiagnostics.Title", "Matcher Diagnostics");
            btnCircleResidualPlot.Content = T("PipelineReview.CircleEvidence.Residuals", "Residuals");
            btnCircleProfilePlot.Content = T("PipelineReview.CircleEvidence.Profile", "Selected scan profile");
            scaleCalibrationTab.Header = T("PipelineReview.ScaleCalibration.Title", "Scale Calibration");
            stepDetailsTab.Header = T("PipelineReview.StepDetails.Title", "Step Details");
            fixtureDesignerTab.Header = T("PipelineReview.FixtureDesigner.Title", "Fixture / Relative ROI");
            lblObjectHighlight.Text = T("PipelineReview.ObjectInspector.Highlight", "Selected object highlight");
            lblInstanceHighlight.Text = T("PipelineReview.InstanceInspector.Highlight", "Selected instance relative ROI");
            lblGeometryHighlight.Text = T("PipelineReview.GeometryReview.Highlight", "Selected geometry highlight");
            lblScaleCalibrationPreview.Text = T("PipelineReview.ScaleCalibration.Preview", "Point A/B evidence");
            lblScaleCalibrationInputs.Text = T("PipelineReview.ScaleCalibration.Inputs", "Same-run point evidence");
            lblScaleKnownDistance.Text = T("PipelineReview.ScaleCalibration.KnownDistance", "Real");
            lblScaleCalibrationResult.Text = T("PipelineReview.ScaleCalibration.Result", "Saved scale (mm/px)");
            txtCalculateScaleCalibrationButton.Text = T("PipelineReview.ScaleCalibration.Calculate", "Calculate + save");
            txtApplyScaleCalibrationButton.Text = T("PipelineReview.ScaleCalibration.Apply", "Apply to Step");
            lblReadiness.Text = T("PipelineReview.Readiness.Title", "Inspection readiness");
            lblReviewGuideStage.Text = T("PipelineReview.Guide.Stage", "Review");
            lblReviewGuideCurrent.Text = T("PipelineReview.Guide.Current", "Current Step");
            lblReviewGuideNext.Text = T("PipelineReview.Guide.Next", "Next Check");
            lblReviewGuideDecision.Text = T("PipelineReview.Guide.Decision", "Decision");
            lblReviewGuidePair.Text = T("PipelineReview.Guide.Pair", "Good/Bad Pair");
            lblReviewGuidePairMetric.Text = T("PipelineReview.Guide.PairMetric", "Metric Check");
            lblReviewGuideChecklist.Text = T("PipelineReview.Guide.Checklist", "Review Habit");
            lblReviewGuideTriageFailure.Text = T("PipelineReview.Guide.TriageFailure", "Cause");
            lblReviewGuideTriageAdjustment.Text = T("PipelineReview.Guide.TriageAdjustment", "Adjust");
            lblReviewGuideTriageRerun.Text = T("PipelineReview.Guide.TriageRerun", "Rerun");
            txtPreviousStepButton.Text = T("PipelineReview.PreviousStep", "Previous");
            txtNextStepButton.Text = T("PipelineReview.NextStep", "Next");
            txtFirstIssueStepButton.Text = T("PipelineReview.FirstIssueStep", "NG Step");
            txtOpenSelectedToolLearnButton.Text = T("PipelineReview.OpenSelectedToolLearnButton", "Learn Tool");
            txtEditSelectedStepButton.Text = T("PipelineReview.EditSelectedStepButton", "Edit Step");
            btnPreviousStep.ToolTip = T("PipelineReview.PreviousStepToolTip", "Select the previous pipeline step");
            btnNextStep.ToolTip = T("PipelineReview.NextStepToolTip", "Select the next pipeline step");
            btnFirstIssueStep.ToolTip = T("PipelineReview.FirstIssueStepToolTip", "Select the first NG pipeline step");
            btnOpenSelectedToolLearn.ToolTip = T(
                "PipelineReview.OpenSelectedToolLearnToolTip",
                "Open the Learn topic for the selected tool");
            btnEditSelectedStep.ToolTip = T(
                "PipelineReview.EditSelectedStepToolTip",
                "Open the selected step in the Recipe Manager parameter editor");
            txtReturnToRecipeButton.Text = T("PipelineReview.ReturnToRecipe", "Return to Recipe");
            btnReturnToRecipe.ToolTip = T(
                "PipelineReview.ReturnToRecipeToolTip",
                "Close Pipeline Review and return to the selected recipe summary.");
            txtUseSelectedMatchingPoseButton.Text = T("PipelineReview.FixtureTeach.Button", "Save as reference");
            txtLegacyUseSelectedMatchingPoseButton.Text = T("PipelineReview.FixtureTeach.Button", "Save as reference");
            btnUseSelectedMatchingPose.ToolTip = T(
                "PipelineReview.FixtureTeach.ButtonToolTip",
                "Save the reviewed Matching pose as the fixture reference without running the pipeline");
            btnLegacyUseSelectedMatchingPose.ToolTip = btnUseSelectedMatchingPose.ToolTip;
            lblFixtureRelationship.Text = T("PipelineReview.FixtureDesigner.Relationship", "Fixture relationship");
            lblFixtureSource.Text = T("PipelineReview.FixtureDesigner.Source", "Source + transformed ROI");
            lblFixtureNormalized.Text = T("PipelineReview.FixtureDesigner.Normalized", "Normalized + reference ROI");
            lblFixtureState.Text = T("PipelineReview.FixtureDesigner.State", "Reference and current state");
            if (fixtureConsumerGrid.Columns.Count >= 7)
            {
                fixtureConsumerGrid.Columns[1].Header = T("PipelineReview.FixtureDesigner.Consumer", "ROI consumer");
                fixtureConsumerGrid.Columns[2].Header = T("PipelineReview.FixtureDesigner.Tool", "Tool");
                fixtureConsumerGrid.Columns[3].Header = T("PipelineReview.FixtureDesigner.ReferenceRoi", "Reference ROI");
                fixtureConsumerGrid.Columns[4].Header = T("PipelineReview.FixtureDesigner.Route", "Route");
                fixtureConsumerGrid.Columns[5].Header = T("PipelineReview.FixtureDesigner.ConsumerState", "State");
                fixtureConsumerGrid.Columns[6].Header = T("PipelineReview.FixtureDesigner.Evidence", "Evidence");
            }
            txtFixtureProducerEditButton.Text = T("PipelineReview.FixtureDesigner.EditProducer", "Edit template / search ROI");
            txtFixtureMeasurementEditButton.Text = T("PipelineReview.FixtureDesigner.EditMeasurement", "Edit measurement ROI");
            txtFixtureRunButton.Text = T("PipelineReview.RunReview", "Run Review");
            btnFixtureProducerEdit.ToolTip = T("PipelineReview.FixtureDesigner.EditProducerToolTip", "Open the fixture Matching Step in the authoritative Recipe Manager PropertyGrid");
            btnFixtureMeasurementEdit.ToolTip = T("PipelineReview.FixtureDesigner.EditMeasurementToolTip", "Open the downstream reference-coordinate ROI Step in the authoritative Recipe Manager PropertyGrid");
            btnFixtureRun.ToolTip = T("PipelineReview.FixtureDesigner.RunToolTip", "Run the pipeline explicitly and refresh fixture evidence");
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackText, params object[] args)
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                string.IsNullOrWhiteSpace(fallbackText) ? T(key, string.Empty) : T(key, fallbackText),
                args ?? Array.Empty<object>());
        }

        public void SetPipelineHeader(string pipelineName, int stepCount)
        {
            ViewModel.SetPipelineHeader(pipelineName, stepCount);
        }

        public void SetRecipeContext(string recipeName)
        {
            ViewModel.SetRecipeContext(recipeName);
        }

        public void SetReviewProgress(string progressText)
        {
            ViewModel.SetReviewProgress(progressText);
        }

        public void SetSteps(IEnumerable<PipelineFlowStepItem> steps)
        {
            pipelineFlowView.SetSteps(steps ?? Enumerable.Empty<PipelineFlowStepItem>());
        }

        public void SelectStep(int index, PipelineFlowPreviewMode mode)
        {
            pipelineFlowView.SelectStep(index, mode);
        }

        public void SetSelectedStep(
            string name,
            string toolType,
            string status,
            string inputLayer,
            Bitmap inputImage,
            string outputLayer,
            Bitmap outputImage,
            string flowSummary,
            string parameterSummary,
            string runLog)
        {
            ReplaceObjectResultBaseImage(outputImage);
            ViewModel.SetSelectedStep(
                name,
                toolType,
                status,
                inputLayer,
                inputImage,
                outputLayer,
                outputImage,
                flowSummary,
                parameterSummary,
                runLog);
        }

        public void SetObjectResults(bool isSupportedTool, IEnumerable<VisionPipelineObjectResult> results)
        {
            SetObjectResults(isSupportedTool, null, results, null, null);
        }

        internal void SetObjectResults(
            bool isSupportedTool,
            VisionPipelineStep step,
            IEnumerable<VisionPipelineObjectResult> results,
            Bitmap sourceImage,
            Bitmap resultImage)
        {
            suppressObjectSelection = true;
            objectResults = (results ?? Enumerable.Empty<VisionPipelineObjectResult>()).ToList();
            objectMetricStep = step;
            objectMetricKind = VisionPipelineObjectMetricKind.Area;
            objectMetricSourceImage?.Dispose();
            objectMetricSourceImage = sourceImage == null ? null : new Bitmap(sourceImage);
            objectResultsGrid.ItemsSource = objectResults;
            objectInspectorTab.Visibility = isSupportedTool
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
            objectResultCountText.Text = TF(
                "PipelineReview.ObjectInspector.CountFormat",
                "Objects {0} / accepted {1} / rejected {2}",
                objectResults.Count,
                objectResults.Count(item => item.Accepted),
                objectResults.Count(item => !item.Accepted));
            objectResultsGrid.SelectedItem = null;
            suppressObjectSelection = false;
            HasObjectHighlight = false;
            RefreshObjectMetricDistribution(resultImage);
            UpdateReviewDetailRowHeight();

            if (isSupportedTool)
            {
                reviewDetailTabs.SelectedItem = objectInspectorTab;
                VisionPipelineObjectResult first = objectResults.FirstOrDefault(item => item.Accepted)
                    ?? objectResults.FirstOrDefault();
                if (first != null)
                {
                    objectResultsGrid.SelectedItem = first;
                    objectResultsGrid.ScrollIntoView(first);
                }
                else
                {
                    RestoreObjectResultPreview();
                }
            }
            else
            {
                reviewDetailTabs.SelectedItem = stepDetailsTab;
                RestoreObjectResultPreview();
            }
        }

        public void SetGeometryResults(bool isSupportedTool, IEnumerable<VisionPipelineGeometryFeatureResult> results)
        {
            suppressGeometrySelection = true;
            geometryResults = (results ?? Enumerable.Empty<VisionPipelineGeometryFeatureResult>()).ToList();
            geometryResultsGrid.ItemsSource = geometryResults;
            geometryReviewTab.Visibility = isSupportedTool
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
            geometryResultCountText.Text = TF(
                "PipelineReview.GeometryReview.CountFormat",
                "Typed features {0} / same-run, pixel-only",
                geometryResults.Count);
            geometryResultsGrid.SelectedItem = null;
            suppressGeometrySelection = false;

            if (isSupportedTool)
            {
                reviewDetailTabs.SelectedItem = geometryReviewTab;
                VisionPipelineGeometryFeatureResult first = geometryResults.FirstOrDefault();
                if (first != null)
                {
                    geometryResultsGrid.SelectedItem = first;
                    geometryResultsGrid.ScrollIntoView(first);
                }
                else
                {
                    RestoreObjectResultPreview();
                }
            }
        }

        public void SetInstanceResults(
            bool isSupportedTool,
            IEnumerable<VisionPipelineInstanceResult> results)
        {
            suppressInstanceSelection = true;
            instanceResults = (results ?? Enumerable.Empty<VisionPipelineInstanceResult>())
                .Select(item => item?.Clone())
                .Where(item => item != null)
                .OrderBy(item => item.Number)
                .ToList();
            instanceResultsGrid.ItemsSource = instanceResults;
            instanceInspectorTab.Visibility = isSupportedTool
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
            instanceResultCountText.Text = TF(
                "PipelineReview.InstanceInspector.CountFormat",
                "Instances {0} / OK {1} / NG {2}",
                instanceResults.Count,
                instanceResults.Count(item => item.Accepted),
                instanceResults.Count(item => !item.Accepted));
            instanceResultsGrid.SelectedItem = null;
            suppressInstanceSelection = false;
            UpdateReviewDetailRowHeight();

            if (isSupportedTool)
            {
                reviewDetailTabs.SelectedItem = instanceInspectorTab;
                VisionPipelineInstanceResult first =
                    instanceResults.FirstOrDefault(item => !item.Accepted)
                    ?? instanceResults.FirstOrDefault();
                if (first != null)
                {
                    instanceResultsGrid.SelectedItem = first;
                    instanceResultsGrid.ScrollIntoView(first);
                }
                else
                {
                    RestoreObjectResultPreview();
                }
            }
        }

        internal void SetCircleEvidence(
            bool isCircleGauge,
            VisionPipelineCircleEvidence evidence,
            Bitmap sourceImage,
            Bitmap resultImage)
        {
            suppressCircleSelection = true;
            circleEvidence = evidence?.Clone();
            circleSamples = circleEvidence?.Samples?.ToList()
                ?? (IReadOnlyList<VisionPipelineCircleSampleEvidence>)Array.Empty<VisionPipelineCircleSampleEvidence>();
            circleSamplesGrid.ItemsSource = circleSamples;
            circleEvidenceTab.Visibility = isCircleGauge
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
            UpdateReviewDetailRowHeight();
            circleSourceImage?.Dispose();
            circleSourceImage = sourceImage == null ? null : new Bitmap(sourceImage);
            circleResidualSignal = circleEvidence == null
                ? null
                : OpenVisionPipelineReviewCircleEvidencePresenter.CreateResidualEvidence(
                    circleEvidence,
                    circleSourceImage,
                    resultImage);
            showCircleProfile = false;
            circleEvidenceSummaryText.Text = circleEvidence?.SummaryText
                ?? T(
                    "PipelineReview.CircleEvidence.NoEvidence",
                    "Run Review explicitly to retain radial sample evidence.");
            circleEvidenceSummaryText.ToolTip = circleEvidenceSummaryText.Text;
            circleSamplesGrid.SelectedItem = null;
            suppressCircleSelection = false;

            if (!isCircleGauge)
            {
                circleEvidencePlot.SetEvidence(null);
                return;
            }

            reviewDetailTabs.SelectedItem = circleEvidenceTab;
            VisionPipelineCircleSampleEvidence first =
                circleSamples.FirstOrDefault(item => item.FitInlier)
                ?? circleSamples.FirstOrDefault(item => item.ContrastAccepted)
                ?? circleSamples.FirstOrDefault();
            if (first != null)
            {
                SelectCircleSampleInternal(first);
            }
            else
            {
                RefreshCircleEvidencePlot();
                RestoreObjectResultPreview();
            }
        }

        internal void SetMatcherDiagnostics(
            bool isEdgeBasedMatching,
            EdgeBasedMatchingDiagnosticEvidence evidence,
            IReadOnlyDictionary<string, double> metrics,
            Bitmap sourceImage)
        {
            matcherDiagnosticState?.Dispose();
            matcherDiagnosticState = OpenVisionPipelineReviewMatcherDiagnosticPresenter.Create(
                evidence,
                metrics,
                sourceImage);
            matcherDiagnosticTab.Visibility = isEdgeBasedMatching
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
            matcherDiagnosticGrid.ItemsSource =
                matcherDiagnosticState?.Rows
                ?? (IReadOnlyList<OpenVisionPipelineReviewMatcherDiagnosticRow>)
                    Array.Empty<OpenVisionPipelineReviewMatcherDiagnosticRow>();
            ViewModel.SetMatcherDiagnosticPreviews(
                matcherDiagnosticState?.ModelPreview,
                matcherDiagnosticState?.CandidatePreview);
            matcherDiagnosticSummaryText.Text = matcherDiagnosticState?.SummaryText
                ?? T(
                    "PipelineReview.MatcherDiagnostics.NoEvidence",
                    "Run Review explicitly to retain model, pyramid, candidate, and decision diagnostics.");
            matcherDiagnosticSummaryText.ToolTip = matcherDiagnosticSummaryText.Text;
            UpdateReviewDetailRowHeight();

            if (isEdgeBasedMatching)
            {
                reviewDetailTabs.SelectedItem = matcherDiagnosticTab;
            }
        }

        private void RefreshObjectMetricDistribution(Bitmap resultImage = null)
        {
            objectMetricDistribution = OpenVisionPipelineReviewObjectDistributionPresenter.Create(
                objectMetricStep,
                objectResults,
                objectMetricKind,
                objectMetricSourceImage,
                resultImage ?? objectResultBaseImage);
            objectMetricPlot.SetEvidence(objectMetricDistribution?.Evidence);
            objectMetricSummaryText.Text = objectMetricDistribution?.SummaryText
                ?? T(
                    "PipelineReview.ObjectInspector.NoDistribution",
                    "Run Review explicitly to retain object metric distribution evidence.");
            objectMetricSummaryText.ToolTip = objectMetricSummaryText.Text;
            btnObjectMetricArea.FontWeight = objectMetricKind == VisionPipelineObjectMetricKind.Area
                ? System.Windows.FontWeights.SemiBold
                : System.Windows.FontWeights.Normal;
            btnObjectMetricWidth.FontWeight = objectMetricKind == VisionPipelineObjectMetricKind.BoundsWidth
                ? System.Windows.FontWeights.SemiBold
                : System.Windows.FontWeights.Normal;
            btnObjectMetricHeight.FontWeight = objectMetricKind == VisionPipelineObjectMetricKind.BoundsHeight
                ? System.Windows.FontWeights.SemiBold
                : System.Windows.FontWeights.Normal;
            UpdateObjectMetricSelection();
        }

        private void SelectObjectMetricKindInternal(VisionPipelineObjectMetricKind kind)
        {
            objectMetricKind = kind;
            RefreshObjectMetricDistribution();
        }

        private void UpdateObjectMetricSelection()
        {
            VisionPipelineObjectResult selected =
                objectResultsGrid?.SelectedItem as VisionPipelineObjectResult;
            objectMetricPlot.SetSelectionX(
                selected == null || objectMetricDistribution == null
                    ? (double?)null
                    : objectMetricDistribution.GetValue(selected));
            if (objectMetricDistribution != null)
            {
                string maximumText = objectMetricDistribution.MaximumIsUnbounded
                    ? "unbounded"
                    : objectMetricDistribution.MaximumValue.ToString("0.###", System.Globalization.CultureInfo.CurrentCulture);
                string selectionText = selected == null
                    ? "No object selected"
                    : selected.Accepted
                        ? $"Selected #{selected.Number} OK — accepted by current object gates"
                        : $"Selected #{selected.Number} REJECT — {selected.RejectReason}";
                objectMetricSummaryText.Text =
                    $"{objectMetricDistribution.MetricName} | "
                    + $"{objectMetricDistribution.MinimumKey} {objectMetricDistribution.MinimumValue:0.###} .. "
                    + $"{objectMetricDistribution.MaximumKey} {maximumText} | {selectionText}";
                objectMetricSummaryText.ToolTip = objectMetricSummaryText.Text;
            }
        }

        private void UpdateReviewDetailRowHeight()
        {
            double height = matcherDiagnosticTab.Visibility == System.Windows.Visibility.Visible
                ? 300D
                : objectInspectorTab.Visibility == System.Windows.Visibility.Visible
                    ? 220D
                : circleEvidenceTab.Visibility == System.Windows.Visibility.Visible
                    ? 280D
                    : 160D;
            reviewDetailRow.Height = new System.Windows.GridLength(height);
        }

        internal bool MatcherDiagnosticTabVisibleForTest =>
            matcherDiagnosticTab.Visibility == System.Windows.Visibility.Visible;

        internal string MatcherDiagnosticStateForTest =>
            matcherDiagnosticState?.State ?? string.Empty;

        internal string MatcherDiagnosticEvidenceIdForTest =>
            matcherDiagnosticState?.EvidenceId ?? string.Empty;

        internal int MatcherDiagnosticRowCountForTest =>
            matcherDiagnosticState?.Rows?.Count ?? 0;

        internal int MatcherDiagnosticModelPointCountForTest =>
            matcherDiagnosticState?.ModelPointCount ?? 0;

        internal bool MatcherDiagnosticHasSelectedCandidateForTest =>
            matcherDiagnosticState?.HasSelectedCandidate == true;

        internal bool MatcherDiagnosticHasAlternativeForTest =>
            matcherDiagnosticState?.HasStrongestSpatialAlternative == true;

        internal int CircleEvidenceSampleCountForTest => circleSamples.Count;

        internal int SelectedCircleSampleNumberForTest =>
            (circleSamplesGrid.SelectedItem as VisionPipelineCircleSampleEvidence)?.Number ?? 0;

        internal int CircleEvidencePlotSeriesCountForTest => circleEvidencePlot.SeriesCount;

        internal bool CircleEvidenceTabVisibleForTest =>
            circleEvidenceTab.Visibility == System.Windows.Visibility.Visible;

        internal bool CircleEvidenceShowsProfileForTest => showCircleProfile;

        internal void ShowCircleResidualPlotForTest()
        {
            showCircleProfile = false;
            RefreshCircleEvidencePlot();
        }

        internal void ShowCircleProfilePlotForTest()
        {
            showCircleProfile = true;
            RefreshCircleEvidencePlot();
        }

        internal void SelectCircleSampleFromPlotForTest(double scanNumber)
        {
            circleEvidencePlot.SelectSampleForTest(scanNumber);
        }

        internal bool SelectCircleSampleAtImagePointForTest(double x, double y)
        {
            VisionPipelineCircleSampleEvidence selected = circleSamples
                .Where(item => item?.HasEdgePoint == true)
                .Select(item =>
                {
                    double dx = item.EdgeX - x;
                    double dy = item.EdgeY - y;
                    return (item, distance: Math.Sqrt(dx * dx + dy * dy));
                })
                .OrderBy(entry => entry.distance)
                .FirstOrDefault()
                .item;
            if (selected == null)
            {
                return false;
            }

            SelectCircleSampleInternal(selected);
            return true;
        }

        private void SelectCircleSampleInternal(VisionPipelineCircleSampleEvidence sample)
        {
            if (sample == null || circleSamplesGrid == null)
            {
                return;
            }

            suppressCircleSelection = true;
            circleSamplesGrid.SelectedItem = sample;
            circleSamplesGrid.ScrollIntoView(sample);
            circleSamplesGrid.UpdateLayout();
            circleSamplesGrid.ScrollIntoView(sample);
            ShowCircleSampleHighlight(sample);
            suppressCircleSelection = false;
            RefreshCircleEvidencePlot();
        }

        private void RefreshCircleEvidencePlot()
        {
            VisionPipelineCircleSampleEvidence selected =
                circleSamplesGrid.SelectedItem as VisionPipelineCircleSampleEvidence;
            VisionToolSignalEvidence signal = showCircleProfile
                ? OpenVisionPipelineReviewCircleEvidencePresenter.CreateProfileEvidence(
                    circleEvidence,
                    selected,
                    circleSourceImage,
                    objectResultBaseImage,
                    circleResidualSignal?.SourceSha256,
                    circleResidualSignal?.ResultSha256)
                : circleResidualSignal;
            circleEvidencePlot.SetEvidence(signal);
            circleEvidencePlot.SetSelectionX(
                !showCircleProfile && selected != null ? selected.Number : null);
            circlePlotCaptionText.Text = showCircleProfile
                ? selected == null
                    ? "Select one radial scan."
                    : $"Scan {selected.Number} / {selected.AngleDeg:0.###} deg / intensity + signed response"
                : selected == null
                    ? "Absolute fitted-radius residual by scan. Click to select."
                    : $"Scan {selected.Number} / {selected.StateText} / {selected.ResidualText} px"
                        + (string.IsNullOrWhiteSpace(selected.RejectReason)
                            ? string.Empty
                            : " / " + selected.RejectReason);
            circlePlotCaptionText.ToolTip = circlePlotCaptionText.Text;
            btnCircleProfilePlot.IsEnabled = selected != null;
        }

        internal void SetScaleCalibrationState(
            IEnumerable<VisionPipelineGeometryFeatureResult> points,
            IEnumerable<VisionPipelineScaleTargetOption> targets,
            VisionPipelineScaleCalibrationRecord record,
            Bitmap coordinateImage,
            string statusText)
        {
            suppressScaleCalibrationSelection = true;
            scaleCalibrationPoints = (points ?? Enumerable.Empty<VisionPipelineGeometryFeatureResult>())
                .Where(item => item?.Kind == VisionPipelineGeometryKind.Point)
                .Select(item => item.Clone())
                .ToList();
            cmbScalePointA.ItemsSource = scaleCalibrationPoints;
            cmbScalePointB.ItemsSource = scaleCalibrationPoints;

            VisionPipelineGeometryFeatureResult pointA = scaleCalibrationPoints.FirstOrDefault(item =>
                string.Equals(item.Identity, record?.PointAIdentity, StringComparison.OrdinalIgnoreCase))
                ?? scaleCalibrationPoints.FirstOrDefault();
            VisionPipelineGeometryFeatureResult pointB = scaleCalibrationPoints.FirstOrDefault(item =>
                string.Equals(item.Identity, record?.PointBIdentity, StringComparison.OrdinalIgnoreCase))
                ?? scaleCalibrationPoints.FirstOrDefault(item => !ReferenceEquals(item, pointA));
            cmbScalePointA.SelectedItem = pointA;
            cmbScalePointB.SelectedItem = pointB;

            List<VisionPipelineScaleTargetOption> targetItems = (targets ?? Enumerable.Empty<VisionPipelineScaleTargetOption>()).ToList();
            cmbScaleTargetStep.ItemsSource = targetItems;
            cmbScaleTargetStep.SelectedItem = targetItems.FirstOrDefault(item =>
                record?.AppliedStepNames?.Contains(item.StepName, StringComparer.OrdinalIgnoreCase) == true)
                ?? targetItems.FirstOrDefault();

            hasScaleCalibrationRecord = record != null;
            if (record != null)
            {
                txtScaleKnownDistance.Text = record.KnownDistance.ToString("0.###############", System.Globalization.CultureInfo.CurrentCulture);
                cmbScaleUnit.SelectedItem = cmbScaleUnit.Items
                    .Cast<VisionScaleCalibrationUnitOption>()
                    .FirstOrDefault(item => item.Unit == record.KnownDistanceUnit);
                string shortenedHash = record.SourceImageSha256.Length > 20
                    ? $"{record.SourceImageSha256.Substring(0, 12)}...{record.SourceImageSha256.Substring(record.SourceImageSha256.Length - 8)}"
                    : record.SourceImageSha256;
                txtScaleCalibrationResult.Text = string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    "{0:0.######} px = {1:0.######} {2} µm {3:0.############} mm/px\n{4} / SHA-256 {5}",
                    record.PixelDistance,
                    record.KnownDistance,
                    OpenVisionPipelineReviewViewRenderService.ResolveUnitText(record.KnownDistanceUnit),
                    record.MillimetersPerPixel,
                    record.CoordinateLayer,
                    shortenedHash);
                txtScaleCalibrationResult.ToolTip = "SHA-256 " + record.SourceImageSha256;
            }
            else
            {
                txtScaleCalibrationResult.Text = T("PipelineReview.ScaleCalibration.NotTaught", "Not taught");
            }

            scaleCalibrationBaseImage?.Dispose();
            scaleCalibrationBaseImage = coordinateImage == null ? null : new Bitmap(coordinateImage);
            suppressScaleCalibrationSelection = false;
            SetScaleCalibrationStatus(statusText);
            RefreshScaleCalibrationPreview();
        }

        public void SetScaleCalibrationStatus(string statusText)
        {
            txtScaleCalibrationStatus.Text = string.IsNullOrWhiteSpace(statusText)
                ? T("PipelineReview.ScaleCalibration.Waiting", "Run Review to obtain two typed points.")
                : statusText.Trim();
            btnCalculateScaleCalibration.IsEnabled = scaleCalibrationPoints.Count >= 2 && scaleCalibrationBaseImage != null;
            btnApplyScaleCalibration.IsEnabled = hasScaleCalibrationRecord && cmbScaleTargetStep.SelectedItem != null;
        }

        internal void SelectScaleCalibrationTabForTest()
        {
            reviewDetailTabs.SelectedItem = scaleCalibrationTab;
        }

        internal bool RequestScaleCalibrationForTest(
            string pointAIdentity,
            string pointBIdentity,
            double knownDistance,
            VisionScaleCalibrationUnit unit)
        {
            cmbScalePointA.SelectedItem = scaleCalibrationPoints.FirstOrDefault(item =>
                string.Equals(item.Identity, pointAIdentity, StringComparison.OrdinalIgnoreCase));
            cmbScalePointB.SelectedItem = scaleCalibrationPoints.FirstOrDefault(item =>
                string.Equals(item.Identity, pointBIdentity, StringComparison.OrdinalIgnoreCase));
            cmbScaleUnit.SelectedItem = cmbScaleUnit.Items
                .Cast<VisionScaleCalibrationUnitOption>()
                .FirstOrDefault(item => item.Unit == unit);
            txtScaleKnownDistance.Text = knownDistance.ToString("0.###############", System.Globalization.CultureInfo.InvariantCulture);
            if (cmbScalePointA.SelectedItem == null || cmbScalePointB.SelectedItem == null)
            {
                return false;
            }

            BtnCalculateScaleCalibration_Click(this, new System.Windows.RoutedEventArgs());
            return true;
        }

        internal bool RequestScaleCalibrationApplyForTest(int stepIndex)
        {
            VisionPipelineScaleTargetOption target = cmbScaleTargetStep.Items
                .Cast<VisionPipelineScaleTargetOption>()
                .FirstOrDefault(item => item.StepIndex == stepIndex);
            if (target == null)
            {
                return false;
            }

            cmbScaleTargetStep.SelectedItem = target;
            BtnApplyScaleCalibration_Click(this, new System.Windows.RoutedEventArgs());
            return true;
        }

        public void SelectObjectResultForTest(int index)
        {
            if (index < 0 || index >= objectResults.Count)
            {
                return;
            }

            objectResultsGrid.SelectedItem = objectResults[index];
            objectResultsGrid.ScrollIntoView(objectResults[index]);
        }

        public void SelectObjectResultFromImageForTest(int index)
        {
            if (index < 0 || index >= objectResults.Count)
            {
                return;
            }

            VisionPipelineObjectResult item = objectResults[index];
            SelectObjectAt(item.CenterX, item.CenterY);
        }

        internal int ObjectMetricDistributionSeriesCountForTest => objectMetricPlot.SeriesCount;

        internal int ObjectMetricDistributionMarkerCountForTest =>
            objectMetricDistribution?.Evidence?.Markers?.Count ?? 0;

        internal string ObjectMetricDistributionEvidenceIdForTest =>
            objectMetricDistribution?.Evidence?.EvidenceId ?? string.Empty;

        internal string ObjectMetricDistributionSummaryForTest =>
            objectMetricDistribution?.SummaryText ?? string.Empty;

        internal string ObjectMetricDistributionMetricForTest =>
            objectMetricDistribution?.MetricName ?? string.Empty;

        internal double? ObjectMetricDistributionSelectionForTest => objectMetricPlot.SelectionX;

        internal VisionToolSignalEvidence ObjectMetricDistributionEvidenceForTest =>
            objectMetricDistribution?.Evidence;

        internal void SelectObjectMetricForTest(VisionPipelineObjectMetricKind kind)
        {
            SelectObjectMetricKindInternal(kind);
        }

        internal void SelectObjectMetricFromPlotForTest(double value)
        {
            objectMetricPlot.SelectSampleForTest(value);
        }

        public void SetValidation(string status, string details)
        {
            ViewModel.SetValidation(status, details);
        }

        public void SetReadiness(OpenVisionPipelineReviewReadinessState state)
        {
            ViewModel.SetReadiness(state);
        }

        public void SetReviewGuide(OpenVisionPipelineReviewGuideState state)
        {
            ViewModel.SetReviewGuide(state);
        }

        public void SetReviewGuidePairAction(string actionText, bool canOpen)
        {
            ViewModel.SetReviewGuidePairAction(actionText, canOpen);
        }

        public void SetReviewGuidePairMetric(string metricText)
        {
            ViewModel.SetReviewGuidePairMetric(metricText);
        }

        public void SetNavigationState(int selectedIndex, int stepCount)
        {
            ViewModel.SetNavigationState(selectedIndex, stepCount);
        }

        public void SetIssueNavigationState(bool canSelectFirstIssueStep)
        {
            ViewModel.SetIssueNavigationState(canSelectFirstIssueStep);
        }

        public void SetSelectedToolLearnState(bool canOpen)
        {
            ViewModel.SetSelectedToolLearnState(canOpen);
        }

        public void SetResultSummary(string summary, string details)
        {
            ViewModel.SetResultSummary(summary, details);
        }

        public void SetRunReviewBusy(bool isBusy)
        {
            ViewModel.SetRunReviewBusy(isBusy);
        }

        public void SetFixtureTeachState(bool isVisible, bool poseAvailable, string statusText)
        {
            ViewModel.SetFixtureTeachState(isVisible, poseAvailable, statusText);
        }

        public void SetFixtureDesignerState(
            bool isVisible,
            string relationshipText,
            string templateText,
            string referenceText,
            string currentText,
            string qualityText,
            string sourceText,
            Bitmap sourcePreview,
            string normalizedText,
            Bitmap normalizedPreview,
            Bitmap templatePreview,
            bool canTeachReference,
            bool canEditProducer,
            bool canEditMeasurement,
            IReadOnlyList<OpenVisionPipelineReviewFixtureConsumerRow> consumers,
            int selectedMeasurementIndex)
        {
            ViewModel.SetFixtureDesignerState(
                isVisible,
                relationshipText,
                templateText,
                referenceText,
                currentText,
                qualityText,
                sourceText,
                sourcePreview,
                normalizedText,
                normalizedPreview,
                templatePreview,
                canTeachReference,
                canEditProducer,
                canEditMeasurement,
                consumers);
            suppressFixtureConsumerSelection = true;
            try
            {
                fixtureConsumerGrid.SelectedItem = ViewModel.FixtureConsumers
                    .FirstOrDefault(item => item.StepIndex == selectedMeasurementIndex);
            }
            finally
            {
                suppressFixtureConsumerSelection = false;
            }
        }

        public void SetEmptyState(string pipelineName)
        {
            SetSteps(Array.Empty<PipelineFlowStepItem>());
            ViewModel.SetEmptyState(pipelineName);
        }

        private void ReplaceObjectResultBaseImage(Bitmap image)
        {
            objectResultBaseImage?.Dispose();
            objectResultBaseImage = image == null ? null : new Bitmap(image);
            HasObjectHighlight = false;
        }

    }

    internal sealed class VisionScaleCalibrationUnitOption
    {
        public VisionScaleCalibrationUnitOption(VisionScaleCalibrationUnit unit, string displayText)
        {
            Unit = unit;
            DisplayText = displayText ?? string.Empty;
        }

        public VisionScaleCalibrationUnit Unit { get; }
        public string DisplayText { get; }
    }

    public sealed class OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs : EventArgs
    {
        public OpenVisionPipelineReviewFixtureConsumerSelectedEventArgs(int stepIndex)
        {
            StepIndex = stepIndex;
        }

        public int StepIndex { get; }
    }

    public sealed class VisionScaleCalibrationRequestedEventArgs : EventArgs
    {
        public VisionScaleCalibrationRequestedEventArgs(
            string pointAIdentity,
            string pointBIdentity,
            double knownDistance,
            VisionScaleCalibrationUnit unit)
        {
            PointAIdentity = pointAIdentity ?? string.Empty;
            PointBIdentity = pointBIdentity ?? string.Empty;
            KnownDistance = knownDistance;
            Unit = unit;
        }

        public string PointAIdentity { get; }
        public string PointBIdentity { get; }
        public double KnownDistance { get; }
        public VisionScaleCalibrationUnit Unit { get; }
    }

    public sealed class VisionScaleCalibrationApplyRequestedEventArgs : EventArgs
    {
        public VisionScaleCalibrationApplyRequestedEventArgs(int stepIndex)
        {
            StepIndex = stepIndex;
        }

        public int StepIndex { get; }
    }
}
