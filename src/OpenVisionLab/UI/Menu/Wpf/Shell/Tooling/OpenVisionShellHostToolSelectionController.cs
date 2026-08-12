using System;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostToolSelectionController
    {
        private readonly OpenVisionShellPreviewViewModel viewModel;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;
        private readonly OpenVisionShellHostToolPrewarmController toolPrewarmController;
        private readonly OpenVisionShellHostBusyPresenter busyPresenter;
        private readonly TextBlock timingDiagnosticsText;
        private readonly Func<bool> canShowSelectedTool;

        public OpenVisionShellHostToolSelectionController(
            OpenVisionShellPreviewViewModel viewModel,
            OpenVisionShellHostToolWindowController toolWindowController,
            OpenVisionShellHostToolPrewarmController toolPrewarmController,
            OpenVisionShellHostBusyPresenter busyPresenter,
            TextBlock timingDiagnosticsText,
            Func<bool> canShowSelectedTool)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.toolWindowController = toolWindowController ?? throw new ArgumentNullException(nameof(toolWindowController));
            this.toolPrewarmController = toolPrewarmController ?? throw new ArgumentNullException(nameof(toolPrewarmController));
            this.busyPresenter = busyPresenter ?? throw new ArgumentNullException(nameof(busyPresenter));
            this.timingDiagnosticsText = timingDiagnosticsText;
            this.canShowSelectedTool = canShowSelectedTool ?? throw new ArgumentNullException(nameof(canShowSelectedTool));
        }

        public void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OpenVisionShellPreviewViewModel.SelectedItem))
            {
                ShowSelectedTool();
            }
        }

        public void ShowSelectedTool()
        {
            OpenVisionShellNavItem selectedItem = viewModel.SelectedItem;
            if (!canShowSelectedTool() || selectedItem == null)
            {
                return;
            }

            bool shouldResumePrewarm = toolPrewarmController.PauseForOperatorSelection();
            bool showPipelineLoading = selectedItem.Menu == DEFINE.VISION_MENU.Pipeline
                && !toolWindowController.HasPreparedPipelineReview;
            if (showPipelineLoading)
            {
                busyPresenter.ShowPipelineLoading();
            }

            try
            {
                if (toolWindowController.ShowSelectedTool(selectedItem))
                {
                    toolPrewarmController.RecordSelection(selectedItem.Menu);
                }

                UpdateToolOpenTimingDiagnostics();
            }
            finally
            {
                if (showPipelineLoading)
                {
                    busyPresenter.Hide();
                }

                toolPrewarmController.ResumeAfterOperatorSelection(shouldResumePrewarm);
            }
        }

        private void UpdateToolOpenTimingDiagnostics()
        {
            if (timingDiagnosticsText == null)
            {
                return;
            }

            string timingText = toolWindowController.LastTiming?.ToPerfText() ?? string.Empty;
            timingDiagnosticsText.Text = timingText;
            AutomationProperties.SetName(timingDiagnosticsText, timingText);
        }
    }
}
