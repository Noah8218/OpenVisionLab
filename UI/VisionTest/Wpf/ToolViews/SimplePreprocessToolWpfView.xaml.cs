using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Windows;

namespace OpenVisionLab
{
    public partial class SimplePreprocessToolWpfView : VisionToolSingleInputCustomToolViewBase
    {
        private readonly SimplePreprocessParameterController parameterController;
        private readonly SimplePreprocessTextPresenter textPresenter;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private VisionToolCustomParameterGuideBinder parameterGuideBinder;
        private bool suppressEvents;

        public SimplePreprocessToolWpfView()
        {
            InitializeComponent();
            AttachToolController(
                string.Empty,
                parameterContentHost,
                refreshViewState: null,
                clearResultReview: ClearResultReviewAndSignalEvidence,
                applyToolLocalization: ApplyLocalization);
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RequestRunPreview);
            textPresenter = new SimplePreprocessTextPresenter(
                ToolController.SetTitleText,
                ToolController.SetTitleIconKind,
                ToolController.SetSummaryText);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                RefreshSummaryAndClearResultReview,
                () => ParameterChanged(this, EventArgs.Empty),
                previewScheduler.Schedule);
            parameterController = new SimplePreprocessParameterController(
                parameterPanel,
                this,
                parameterChangeController,
                () => suppressEvents,
                value => suppressEvents = value);
            ApplyLocalization();
            ClearResultReviewAndSignalEvidence();
        }

        public event EventHandler ParameterChanged = delegate { };

        internal SimplePreprocessParameterController Parameters => parameterController;

        internal void AttachParameterGuide(
            Func<object> selectedObjectFactory,
            params string[] propertyNames)
        {
            parameterGuideBinder?.Dispose();
            var bindings = parameterController.CreateParameterGuideBindings(propertyNames);
            if (bindings.Count == 0)
            {
                throw new InvalidOperationException(
                    "Simple preprocess Parameter Guide requires at least one registered editor.");
            }

            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                selectedObjectFactory,
                bindings);
        }

        internal void AttachParameterGuide(
            Func<object> selectedObjectFactory,
            IReadOnlyDictionary<string, string> controlPropertyNames)
        {
            parameterGuideBinder?.Dispose();
            var bindings =
                parameterController.CreateParameterGuideBindings(controlPropertyNames);
            if (bindings.Count == 0)
            {
                throw new InvalidOperationException(
                    "Simple preprocess Parameter Guide requires at least one registered editor.");
            }

            parameterGuideBinder = VisionToolCustomParameterGuideBinder.Attach(
                toolShell,
                selectedObjectFactory,
                bindings);
        }

        internal bool SignalInspectorHasEvidenceForTest => signalInspector.HasEvidence;

        internal string SignalInspectorEvidenceIdForTest => signalInspector.EvidenceId;

        internal string SignalInspectorSourceSha256ForTest => signalInspector.SourceSha256;

        internal int SignalInspectorSeriesCountForTest => signalInspector.SeriesCount;

        protected override void DisposeToolResources()
        {
            parameterGuideBinder?.Dispose();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
            parameterController.RefreshLabels();
            signalInspector.ApplyLocalization();
        }

        public void SetHeader(string title, PackIconMaterialKind iconKind)
        {
            textPresenter.SetHeader(title, iconKind);
        }

        public void SetLocalizedHeader(string localizationKey, string fallbackTitle, PackIconMaterialKind iconKind)
        {
            textPresenter.SetLocalizedHeader(localizationKey, fallbackTitle, iconKind);
        }

        public void SetSummary(string summary)
        {
            textPresenter.SetSummary(summary);
            parameterChangeController.RefreshProgrammatic();
        }

        public void SetLearnTopic(OpenVisionLearnTopicIndex topicIndex, string buttonText = "Learn")
        {
            SetLearnTopic((int)topicIndex, buttonText);
        }

        public void SetLearnTopic(int topicIndex, string buttonText = "Learn")
        {
            toolShell.LearnButtonVisibility = Visibility.Visible;
            toolShell.LearnTopicIndex = topicIndex;
            toolShell.LearnButtonText = string.IsNullOrWhiteSpace(buttonText) ? "Learn" : buttonText;
        }

        internal void ShowResultReview(SimplePreprocessResultReview review)
        {
            ShowToolResultReview(review.Summary, review.IsSuccess, review.Items, review.Guidance);
        }

        internal void ShowSignalEvidence(VisionToolSignalEvidence evidence)
        {
            signalInspector.ShowEvidence(evidence);
        }

        internal void ClearSignalEvidence()
        {
            signalInspector.ClearEvidence();
        }

        internal void ResetSignalInspectorViewForTest()
        {
            signalInspector.ResetViewForTest();
        }

        internal bool ExerciseSignalInspectorNavigationForTest()
        {
            return signalInspector.ExerciseNavigationForTest();
        }

        internal void ExportSignalEvidenceForTest(string path)
        {
            signalInspector.ExportForTest(path);
        }

        private void RefreshSummaryAndClearResultReview()
        {
            textPresenter.RefreshSummary();
            ClearResultReviewAndSignalEvidence();
        }

        private void ClearResultReviewAndSignalEvidence()
        {
            ClearResultReview();
            ClearSignalEvidence();
        }
    }
}
