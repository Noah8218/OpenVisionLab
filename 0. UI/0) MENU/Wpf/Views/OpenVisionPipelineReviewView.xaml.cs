using OpenVisionLab.Pipeline.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class OpenVisionPipelineReviewView : UserControl
    {
        public OpenVisionPipelineReviewView()
        {
            InitializeComponent();
            ViewModel = new OpenVisionPipelineReviewViewModel();
            DataContext = ViewModel;
            pipelineFlowView.StepSelected += OnPipelineFlowStepSelected;
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

        public OpenVisionPipelineReviewViewModel ViewModel { get; }
        public string SelectedStepText => ViewModel.SelectedStepText;
        public string SelectedToolText => ViewModel.SelectedToolText;
        public string SelectedStatusText => ViewModel.SelectedStatusText;
        public string ReviewProgressText => ViewModel.ReviewProgressText;
        public string FlowSummaryText => ViewModel.FlowSummaryText;
        public string ParameterSummaryText => ViewModel.ParameterSummaryText;
        public string ValidationStatusText => ViewModel.ValidationStatusText;
        public string ValidationDetailText => ViewModel.ValidationDetailText;
        public string ResultSummaryText => ViewModel.ResultSummaryText;
        public string ResultDetailText => ViewModel.ResultDetailText;
        public string RunLogText => ViewModel.RunLogText;
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
        public int SelectedFlowIndex => pipelineFlowView.SelectedIndex;

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ApplyLocalization();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            Unloaded -= OnUnloaded;
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
            btnPreviousStep.ToolTip = T("PipelineReview.PreviousStepToolTip", "Select the previous pipeline step");
            btnNextStep.ToolTip = T("PipelineReview.NextStepToolTip", "Select the next pipeline step");
            btnFirstIssueStep.ToolTip = T("PipelineReview.FirstIssueStepToolTip", "Select the first NG pipeline step");
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        public void SetPipelineHeader(string pipelineName, int stepCount)
        {
            ViewModel.SetPipelineHeader(pipelineName, stepCount);
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

        public void SetValidation(string status, string details)
        {
            ViewModel.SetValidation(status, details);
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

        public void SetResultSummary(string summary, string details)
        {
            ViewModel.SetResultSummary(summary, details);
        }

        public void SetRunReviewBusy(bool isBusy)
        {
            ViewModel.SetRunReviewBusy(isBusy);
        }

        public void SetEmptyState(string pipelineName)
        {
            SetSteps(Array.Empty<PipelineFlowStepItem>());
            ViewModel.SetEmptyState(pipelineName);
        }

        private void BtnRunReview_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RunReviewRequested(this, EventArgs.Empty);
        }

        private void BtnPreviousStep_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PreviousStepRequested(this, EventArgs.Empty);
        }

        private void BtnNextStep_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NextStepRequested(this, EventArgs.Empty);
        }

        private void BtnFirstIssueStep_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FirstIssueStepRequested(this, EventArgs.Empty);
        }

        private void BtnOpenPairSample_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenPairSampleRequested(this, EventArgs.Empty);
        }

        private void OnPipelineFlowStepSelected(object sender, PipelineFlowStepSelectedEventArgs e)
        {
            StepSelected(this, e);
        }
    }
}
