using MahApps.Metro.IconPacks;
using System;

namespace OpenVisionLab
{
    public partial class SimplePreprocessToolWpfView : VisionToolSingleInputCustomToolViewBase
    {
        private readonly SimplePreprocessParameterController parameterController;
        private readonly SimplePreprocessTextPresenter textPresenter;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private bool suppressEvents;

        public SimplePreprocessToolWpfView()
        {
            InitializeComponent();
            AttachToolController(
                string.Empty,
                parameterContentHost,
                refreshViewState: null,
                clearResultReview: ClearResultReview,
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
            ClearResultReview();
        }

        public event EventHandler ParameterChanged = delegate { };

        internal SimplePreprocessParameterController Parameters => parameterController;

        protected override void DisposeToolResources()
        {
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter.ApplyLocalization();
            parameterController.RefreshLabels();
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

        internal void ShowResultReview(SimplePreprocessResultReview review)
        {
            ShowToolResultReview(review.Summary, review.IsSuccess, review.Items, review.Guidance);
        }

        private void RefreshSummaryAndClearResultReview()
        {
            textPresenter.RefreshSummary();
            ClearResultReview();
        }
    }
}
