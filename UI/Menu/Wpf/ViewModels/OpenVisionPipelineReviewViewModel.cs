using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    public sealed class OpenVisionPipelineReviewFixtureConsumerRow
    {
        public int StepIndex { get; init; }
        public int StepNumber { get; init; }
        public string StepName { get; init; } = string.Empty;
        public string ToolType { get; init; } = string.Empty;
        public string RoiText { get; init; } = string.Empty;
        public string RouteText { get; init; } = string.Empty;
        public string StatusText { get; init; } = string.Empty;
        public string EvidenceId { get; init; } = string.Empty;
        public string EvidenceShortId { get; init; } = string.Empty;
    }

    public sealed class OpenVisionPipelineReviewViewModel : INotifyPropertyChanged
    {
        private string pipelineTitle = T("PipelineReview.Title", "Pipeline Review");
        private string pipelineMeta = TF("PipelineReview.MetaFormat", "{0} / {1} steps", T("Pipeline.Title", "Pipeline"), 0);
        private string recipeContextText = TF("PipelineReview.RecipeContextFormat", "Recipe: {0}", "-");
        private string reviewProgressText = T("PipelineReview.Progress.NotRun", "Not run");
        private string selectedStepText = "-";
        private string selectedToolText = "-";
        private string selectedStatusText = "-";
        private string routeText = "-";
        private string inputLayerText = "-";
        private string inputMetaText = "-";
        private BitmapImage inputPreviewImage;
        private string outputLayerText = "-";
        private string outputMetaText = "-";
        private BitmapImage outputPreviewImage;
        private string flowSummaryText = "-";
        private string parameterSummaryText = "-";
        private string validationStatusText = T("PipelineReview.ValidationNotRun", "Validation not run");
        private string validationDetailText = "-";
        private string resultSummaryText = T("PipelineReview.RunRequired", "Run review required");
        private string resultDetailText = "-";
        private string runLogText = "-";
        private string runReviewButtonText = T("PipelineReview.RunReview", "Run Review");
        private bool canRunReview = true;
        private string statusText = T("PipelineReview.Ready", "Pipeline review ready");
        private IReadOnlyList<OpenVisionPipelineReviewReadinessItem> readinessItems = Array.Empty<OpenVisionPipelineReviewReadinessItem>();
        private string readinessSummaryText = T("PipelineReview.Readiness.SummaryReady", "Ready to run review");
        private string reviewGuideStageText = T("PipelineReview.Guide.EmptyStage", "No steps");
        private string reviewGuideCurrentStepText = "-";
        private string reviewGuideNextActionText = T("PipelineReview.Guide.EmptyNext", "Add a tool result to the pipeline");
        private string reviewGuideResultDecisionText = T("PipelineReview.Guide.EmptyDecision", "No result to judge");
        private string reviewGuideDetailText = T("PipelineReview.Guide.EmptyDetail", "Add a tool result from a Tool View, then run review.");
        private string reviewGuidePairText = string.Empty;
        private string reviewGuidePairActionText = string.Empty;
        private string reviewGuidePairMetricText = string.Empty;
        private string reviewGuideChecklistText = T("PipelineReview.Guide.ChecklistText", "Review habit: run Good first -> run Bad in the same PairGroup with the same pipeline -> compare output image, overlay, metrics, and log.");
        private string reviewGuideParameterFocusText = string.Empty;
        private string reviewGuideTriageFailureText = string.Empty;
        private string reviewGuideTriageAdjustmentText = string.Empty;
        private string reviewGuideTriageRerunText = string.Empty;
        private bool hasReviewGuidePairText;
        private bool hasReviewGuidePairMetricText;
        private bool hasReviewGuideParameterFocusText;
        private bool hasReviewGuideTriage;
        private bool canOpenReviewGuidePairAction;
        private bool canSelectPreviousStep;
        private bool canSelectNextStep;
        private bool canSelectFirstIssueStep;
        private bool canOpenSelectedToolLearn;
        private bool isFixtureTeachVisible;
        private bool fixturePoseAvailable;
        private bool canUseSelectedMatchingPose;
        private string fixtureTeachStatusText = string.Empty;
        private bool isFixtureDesignerVisible;
        private string fixtureRelationshipText = string.Empty;
        private string fixtureTemplateText = string.Empty;
        private string fixtureReferenceText = string.Empty;
        private string fixtureCurrentText = string.Empty;
        private string fixtureQualityText = string.Empty;
        private string fixtureSourceText = string.Empty;
        private string fixtureNormalizedText = string.Empty;
        private BitmapImage fixtureTemplatePreviewImage;
        private BitmapImage fixtureSourcePreviewImage;
        private BitmapImage fixtureNormalizedPreviewImage;
        private bool canEditFixtureProducer;
        private bool canEditFixtureMeasurement;
        private bool fixtureProducerEditAvailable;
        private bool fixtureMeasurementEditAvailable;
        private BitmapImage scaleCalibrationPreviewImage;
        private BitmapImage matcherModelPreviewImage;
        private BitmapImage matcherCandidatePreviewImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string PipelineTitle { get => pipelineTitle; private set => SetField(ref pipelineTitle, value); }
        public string PipelineMeta { get => pipelineMeta; private set => SetField(ref pipelineMeta, value); }
        public string RecipeContextText { get => recipeContextText; private set => SetField(ref recipeContextText, value); }
        public string ReviewProgressText { get => reviewProgressText; private set => SetField(ref reviewProgressText, value); }
        public string SelectedStepText { get => selectedStepText; private set => SetField(ref selectedStepText, value); }
        public string SelectedToolText { get => selectedToolText; private set => SetField(ref selectedToolText, value); }
        public string SelectedStatusText { get => selectedStatusText; private set => SetField(ref selectedStatusText, value); }
        public string RouteText { get => routeText; private set => SetField(ref routeText, value); }
        public string InputLayerText { get => inputLayerText; private set => SetField(ref inputLayerText, value); }
        public string InputMetaText { get => inputMetaText; private set => SetField(ref inputMetaText, value); }
        public BitmapImage InputPreviewImage { get => inputPreviewImage; private set => SetField(ref inputPreviewImage, value); }
        public string OutputLayerText { get => outputLayerText; private set => SetField(ref outputLayerText, value); }
        public string OutputMetaText { get => outputMetaText; private set => SetField(ref outputMetaText, value); }
        public BitmapImage OutputPreviewImage { get => outputPreviewImage; private set => SetField(ref outputPreviewImage, value); }
        public string FlowSummaryText { get => flowSummaryText; private set => SetField(ref flowSummaryText, value); }
        public string ParameterSummaryText { get => parameterSummaryText; private set => SetField(ref parameterSummaryText, value); }
        public string ValidationStatusText { get => validationStatusText; private set => SetField(ref validationStatusText, value); }
        public string ValidationDetailText { get => validationDetailText; private set => SetField(ref validationDetailText, value); }
        public string ResultSummaryText { get => resultSummaryText; private set => SetField(ref resultSummaryText, value); }
        public string ResultDetailText { get => resultDetailText; private set => SetField(ref resultDetailText, value); }
        public string RunLogText { get => runLogText; private set => SetField(ref runLogText, value); }
        public string RunReviewButtonText { get => runReviewButtonText; private set => SetField(ref runReviewButtonText, value); }
        public bool CanRunReview { get => canRunReview; private set => SetField(ref canRunReview, value); }
        public string StatusText { get => statusText; private set => SetField(ref statusText, value); }
        public IReadOnlyList<OpenVisionPipelineReviewReadinessItem> ReadinessItems { get => readinessItems; private set => SetField(ref readinessItems, value); }
        public string ReadinessSummaryText { get => readinessSummaryText; private set => SetField(ref readinessSummaryText, value); }
        public string ReviewGuideStageText { get => reviewGuideStageText; private set => SetField(ref reviewGuideStageText, value); }
        public string ReviewGuideCurrentStepText { get => reviewGuideCurrentStepText; private set => SetField(ref reviewGuideCurrentStepText, value); }
        public string ReviewGuideNextActionText { get => reviewGuideNextActionText; private set => SetField(ref reviewGuideNextActionText, value); }
        public string ReviewGuideResultDecisionText { get => reviewGuideResultDecisionText; private set => SetField(ref reviewGuideResultDecisionText, value); }
        public string ReviewGuideDetailText { get => reviewGuideDetailText; private set => SetField(ref reviewGuideDetailText, value); }
        public string ReviewGuidePairText { get => reviewGuidePairText; private set => SetField(ref reviewGuidePairText, value); }
        public string ReviewGuidePairActionText { get => reviewGuidePairActionText; private set => SetField(ref reviewGuidePairActionText, value); }
        public string ReviewGuidePairMetricText { get => reviewGuidePairMetricText; private set => SetField(ref reviewGuidePairMetricText, value); }
        public string ReviewGuideChecklistText { get => reviewGuideChecklistText; private set => SetField(ref reviewGuideChecklistText, value); }
        public string ReviewGuideParameterFocusText { get => reviewGuideParameterFocusText; private set => SetField(ref reviewGuideParameterFocusText, value); }
        public string ReviewGuideTriageFailureText { get => reviewGuideTriageFailureText; private set => SetField(ref reviewGuideTriageFailureText, value); }
        public string ReviewGuideTriageAdjustmentText { get => reviewGuideTriageAdjustmentText; private set => SetField(ref reviewGuideTriageAdjustmentText, value); }
        public string ReviewGuideTriageRerunText { get => reviewGuideTriageRerunText; private set => SetField(ref reviewGuideTriageRerunText, value); }
        public bool HasReviewGuidePairText { get => hasReviewGuidePairText; private set => SetField(ref hasReviewGuidePairText, value); }
        public bool HasReviewGuidePairMetricText { get => hasReviewGuidePairMetricText; private set => SetField(ref hasReviewGuidePairMetricText, value); }
        public bool HasReviewGuideParameterFocusText { get => hasReviewGuideParameterFocusText; private set => SetField(ref hasReviewGuideParameterFocusText, value); }
        public bool HasReviewGuideTriage { get => hasReviewGuideTriage; private set => SetField(ref hasReviewGuideTriage, value); }
        public bool CanOpenReviewGuidePairAction { get => canOpenReviewGuidePairAction; private set => SetField(ref canOpenReviewGuidePairAction, value); }
        public bool CanSelectPreviousStep { get => canSelectPreviousStep; private set => SetField(ref canSelectPreviousStep, value); }
        public bool CanSelectNextStep { get => canSelectNextStep; private set => SetField(ref canSelectNextStep, value); }
        public bool CanSelectFirstIssueStep { get => canSelectFirstIssueStep; private set => SetField(ref canSelectFirstIssueStep, value); }
        public bool CanOpenSelectedToolLearn { get => canOpenSelectedToolLearn; private set => SetField(ref canOpenSelectedToolLearn, value); }
        public bool IsFixtureTeachVisible { get => isFixtureTeachVisible; private set => SetField(ref isFixtureTeachVisible, value); }
        public bool IsLegacyFixtureTeachVisible => IsFixtureTeachVisible && !IsFixtureDesignerVisible;
        public bool CanUseSelectedMatchingPose { get => canUseSelectedMatchingPose; private set => SetField(ref canUseSelectedMatchingPose, value); }
        public string FixtureTeachStatusText { get => fixtureTeachStatusText; private set => SetField(ref fixtureTeachStatusText, value); }
        public bool IsFixtureDesignerVisible { get => isFixtureDesignerVisible; private set => SetField(ref isFixtureDesignerVisible, value); }
        public string FixtureRelationshipText { get => fixtureRelationshipText; private set => SetField(ref fixtureRelationshipText, value); }
        public string FixtureTemplateText { get => fixtureTemplateText; private set => SetField(ref fixtureTemplateText, value); }
        public string FixtureReferenceText { get => fixtureReferenceText; private set => SetField(ref fixtureReferenceText, value); }
        public string FixtureCurrentText { get => fixtureCurrentText; private set => SetField(ref fixtureCurrentText, value); }
        public string FixtureQualityText { get => fixtureQualityText; private set => SetField(ref fixtureQualityText, value); }
        public string FixtureSourceText { get => fixtureSourceText; private set => SetField(ref fixtureSourceText, value); }
        public string FixtureNormalizedText { get => fixtureNormalizedText; private set => SetField(ref fixtureNormalizedText, value); }
        public BitmapImage FixtureTemplatePreviewImage { get => fixtureTemplatePreviewImage; private set => SetField(ref fixtureTemplatePreviewImage, value); }
        public BitmapImage FixtureSourcePreviewImage { get => fixtureSourcePreviewImage; private set => SetField(ref fixtureSourcePreviewImage, value); }
        public BitmapImage FixtureNormalizedPreviewImage { get => fixtureNormalizedPreviewImage; private set => SetField(ref fixtureNormalizedPreviewImage, value); }
        public bool CanEditFixtureProducer { get => canEditFixtureProducer; private set => SetField(ref canEditFixtureProducer, value); }
        public bool CanEditFixtureMeasurement { get => canEditFixtureMeasurement; private set => SetField(ref canEditFixtureMeasurement, value); }
        public ObservableCollection<OpenVisionPipelineReviewFixtureConsumerRow> FixtureConsumers { get; } =
            new ObservableCollection<OpenVisionPipelineReviewFixtureConsumerRow>();
        public BitmapImage ScaleCalibrationPreviewImage { get => scaleCalibrationPreviewImage; private set => SetField(ref scaleCalibrationPreviewImage, value); }
        public BitmapImage MatcherModelPreviewImage { get => matcherModelPreviewImage; private set => SetField(ref matcherModelPreviewImage, value); }
        public BitmapImage MatcherCandidatePreviewImage { get => matcherCandidatePreviewImage; private set => SetField(ref matcherCandidatePreviewImage, value); }
        public bool HasInputPreview => InputPreviewImage != null;
        public bool HasOutputPreview => OutputPreviewImage != null;

        public void SetPipelineHeader(string pipelineName, int stepCount)
        {
            string name = string.IsNullOrWhiteSpace(pipelineName) ? T("Pipeline.Title", "Pipeline") : pipelineName.Trim();
            PipelineTitle = T("PipelineReview.Title", "Pipeline Review");
            PipelineMeta = TF("PipelineReview.MetaFormat", "{0} / {1} steps", name, stepCount);
        }

        public void SetRecipeContext(string recipeName)
        {
            RecipeContextText = TF(
                "PipelineReview.RecipeContextFormat",
                "Recipe: {0}",
                SafeText(recipeName));
        }

        public void SetReviewProgress(string progressText)
        {
            ReviewProgressText = string.IsNullOrWhiteSpace(progressText)
                ? T("PipelineReview.Progress.NotRun", "Not run")
                : progressText.Trim();
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
            SelectedStepText = SafeText(name);
            SelectedToolText = SafeText(toolType);
            SelectedStatusText = SafeText(status);
            SetInputPreview(inputLayer, inputImage);
            SetOutputPreview(outputLayer, outputImage);
            RouteText = TF("PipelineReview.RouteFormat", "{0} -> {1}", SafeText(inputLayer), SafeText(outputLayer));
            FlowSummaryText = SafeText(flowSummary);
            ParameterSummaryText = SafeText(parameterSummary);
            RunLogText = SafeText(runLog);
            StatusText = TF("PipelineReview.SelectedFormat", "Selected: {0}", SafeText(name));
        }

        public void SetResultSummary(string summary, string details)
        {
            ResultSummaryText = SafeText(summary);
            ResultDetailText = SafeText(details);
        }

        public void SetHighlightedOutputPreview(Bitmap bitmap)
        {
            OutputPreviewImage = CreateBitmapImage(bitmap);
            OnPropertyChanged(nameof(HasOutputPreview));
        }

        public void SetScaleCalibrationPreview(Bitmap bitmap)
        {
            ScaleCalibrationPreviewImage = CreateBitmapImage(bitmap);
        }

        public void SetMatcherDiagnosticPreviews(Bitmap modelPreview, Bitmap candidatePreview)
        {
            MatcherModelPreviewImage = CreateBitmapImage(modelPreview);
            MatcherCandidatePreviewImage = CreateBitmapImage(candidatePreview);
        }

        public void SetRunReviewBusy(bool isBusy)
        {
            CanRunReview = !isBusy;
            CanUseSelectedMatchingPose = fixturePoseAvailable && !isBusy;
            CanEditFixtureProducer = fixtureProducerEditAvailable && !isBusy;
            CanEditFixtureMeasurement = fixtureMeasurementEditAvailable && !isBusy;
            RunReviewButtonText = isBusy ? T("PipelineReview.RunningButton", "Running...") : T("PipelineReview.RunReview", "Run Review");
            StatusText = isBusy ? T("PipelineReview.Running", "Pipeline review is running.") : StatusText;
        }

        public void SetFixtureTeachState(bool isVisible, bool poseAvailable, string statusText)
        {
            IsFixtureTeachVisible = isVisible;
            fixturePoseAvailable = isVisible && poseAvailable;
            CanUseSelectedMatchingPose = fixturePoseAvailable && CanRunReview;
            FixtureTeachStatusText = isVisible && !string.IsNullOrWhiteSpace(statusText)
                ? statusText.Trim()
                : string.Empty;
            OnPropertyChanged(nameof(IsLegacyFixtureTeachVisible));
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
            IReadOnlyList<OpenVisionPipelineReviewFixtureConsumerRow> consumers)
        {
            IsFixtureDesignerVisible = isVisible;
            OnPropertyChanged(nameof(IsLegacyFixtureTeachVisible));
            FixtureRelationshipText = isVisible ? SafeText(relationshipText) : string.Empty;
            FixtureTemplateText = isVisible ? SafeText(templateText) : string.Empty;
            FixtureReferenceText = isVisible ? SafeText(referenceText) : string.Empty;
            FixtureCurrentText = isVisible ? SafeText(currentText) : string.Empty;
            FixtureQualityText = isVisible ? SafeText(qualityText) : string.Empty;
            FixtureSourceText = isVisible ? SafeText(sourceText) : string.Empty;
            FixtureNormalizedText = isVisible ? SafeText(normalizedText) : string.Empty;
            FixtureSourcePreviewImage = isVisible ? CreateBitmapImage(sourcePreview) : null;
            FixtureNormalizedPreviewImage = isVisible ? CreateBitmapImage(normalizedPreview) : null;
            FixtureTemplatePreviewImage = isVisible ? CreateBitmapImage(templatePreview) : null;
            FixtureConsumers.Clear();
            if (isVisible && consumers != null)
            {
                foreach (OpenVisionPipelineReviewFixtureConsumerRow consumer in consumers)
                {
                    FixtureConsumers.Add(consumer);
                }
            }
            if (isVisible)
            {
                fixturePoseAvailable = canTeachReference;
                CanUseSelectedMatchingPose = fixturePoseAvailable && CanRunReview;
            }
            fixtureProducerEditAvailable = isVisible && canEditProducer;
            fixtureMeasurementEditAvailable = isVisible && canEditMeasurement;
            CanEditFixtureProducer = fixtureProducerEditAvailable && CanRunReview;
            CanEditFixtureMeasurement = fixtureMeasurementEditAvailable && CanRunReview;
        }

        public void SetValidation(string status, string details)
        {
            ValidationStatusText = SafeText(status);
            ValidationDetailText = SafeText(details);
        }

        public void SetReadiness(OpenVisionPipelineReviewReadinessState state)
        {
            ReadinessItems = state?.Items ?? Array.Empty<OpenVisionPipelineReviewReadinessItem>();
            ReadinessSummaryText = string.IsNullOrWhiteSpace(state?.SummaryText)
                ? T("PipelineReview.Readiness.SummaryReady", "Ready to run review")
                : state.SummaryText.Trim();
        }

        public void SetReviewGuide(OpenVisionPipelineReviewGuideState state)
        {
            ReviewGuideStageText = SafeText(state?.StageText);
            ReviewGuideCurrentStepText = SafeText(state?.CurrentStepText);
            ReviewGuideNextActionText = SafeText(state?.NextActionText);
            ReviewGuideResultDecisionText = SafeText(state?.ResultDecisionText);
            ReviewGuideDetailText = SafeText(state?.DetailText);
            ReviewGuidePairText = string.IsNullOrWhiteSpace(state?.PairReviewText) ? string.Empty : state.PairReviewText.Trim();
            HasReviewGuidePairText = !string.IsNullOrWhiteSpace(ReviewGuidePairText);
            SetReviewGuidePairAction(string.Empty, false);
            SetReviewGuidePairMetric(string.Empty);
            ReviewGuideChecklistText = SafeText(state?.ChecklistText);
            ReviewGuideParameterFocusText = string.IsNullOrWhiteSpace(state?.ParameterFocusText) ? string.Empty : state.ParameterFocusText.Trim();
            HasReviewGuideParameterFocusText = !string.IsNullOrWhiteSpace(ReviewGuideParameterFocusText);
            ReviewGuideTriageFailureText = string.IsNullOrWhiteSpace(state?.TriageFailureText) ? string.Empty : state.TriageFailureText.Trim();
            ReviewGuideTriageAdjustmentText = string.IsNullOrWhiteSpace(state?.TriageAdjustmentText) ? string.Empty : state.TriageAdjustmentText.Trim();
            ReviewGuideTriageRerunText = string.IsNullOrWhiteSpace(state?.TriageRerunText) ? string.Empty : state.TriageRerunText.Trim();
            HasReviewGuideTriage = !string.IsNullOrWhiteSpace(ReviewGuideTriageFailureText)
                || !string.IsNullOrWhiteSpace(ReviewGuideTriageAdjustmentText)
                || !string.IsNullOrWhiteSpace(ReviewGuideTriageRerunText);
        }

        public void SetReviewGuidePairAction(string actionText, bool canOpen)
        {
            ReviewGuidePairActionText = string.IsNullOrWhiteSpace(actionText) ? string.Empty : actionText.Trim();
            CanOpenReviewGuidePairAction = canOpen && !string.IsNullOrWhiteSpace(ReviewGuidePairActionText);
        }

        public void SetReviewGuidePairMetric(string metricText)
        {
            ReviewGuidePairMetricText = string.IsNullOrWhiteSpace(metricText) ? string.Empty : metricText.Trim();
            HasReviewGuidePairMetricText = !string.IsNullOrWhiteSpace(ReviewGuidePairMetricText);
        }

        public void SetNavigationState(int selectedIndex, int stepCount)
        {
            CanSelectPreviousStep = selectedIndex > 0 && stepCount > 1;
            CanSelectNextStep = selectedIndex >= 0 && selectedIndex < stepCount - 1;
        }

        public void SetIssueNavigationState(bool canSelectFirstIssueStep)
        {
            CanSelectFirstIssueStep = canSelectFirstIssueStep;
        }

        public void SetSelectedToolLearnState(bool canOpen)
        {
            CanOpenSelectedToolLearn = canOpen;
        }

        public void SetEmptyState(string pipelineName)
        {
            CanOpenSelectedToolLearn = false;
            SetPipelineHeader(pipelineName, 0);
            SetSelectedStep(
                "-",
                "-",
                "EMPTY",
                "-",
                null,
                "-",
                null,
                T("PipelineReview.EmptyFlow", "No pipeline steps."),
                "-",
                T("PipelineReview.EmptyHint", "Add steps from a tool view or the Pipeline editor."));
            SetReviewGuide(OpenVisionPipelineReviewGuidePresenter.CreateEmpty(pipelineName));
            SetNavigationState(-1, 0);
            SetIssueNavigationState(false);
            SetFixtureTeachState(false, false, string.Empty);
            SetFixtureDesignerState(
                false,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                string.Empty,
                null,
                null,
                false,
                false,
                false,
                Array.Empty<OpenVisionPipelineReviewFixtureConsumerRow>());
            SetResultSummary(T("PipelineReview.NoRunResult", "No run result"), "-");
            SetReviewProgress(T("PipelineReview.Progress.NoSteps", "No steps"));
            StatusText = T("PipelineReview.NoStepsStatus", "Pipeline has no steps.");
        }

        private void SetInputPreview(string layerName, Bitmap bitmap)
        {
            InputLayerText = SafeText(layerName);
            InputPreviewImage = CreateBitmapImage(bitmap);
            InputMetaText = bitmap == null ? T("PipelineReview.NoImage", "No image") : FormatImageSize(bitmap);
            OnPropertyChanged(nameof(HasInputPreview));
        }

        private void SetOutputPreview(string layerName, Bitmap bitmap)
        {
            OutputLayerText = SafeText(layerName);
            OutputPreviewImage = CreateBitmapImage(bitmap);
            OutputMetaText = bitmap == null ? T("PipelineReview.NoImage", "No image") : FormatImageSize(bitmap);
            OnPropertyChanged(nameof(HasOutputPreview));
        }

        private static string FormatImageSize(Bitmap bitmap)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}x{1}", bitmap.Width, bitmap.Height);
        }

        private static BitmapImage CreateBitmapImage(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }

            try
            {
                using MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }

        private static string SafeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string TF(string key, string fallbackFormat, params object[] args)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, T(key, fallbackFormat), args);
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
