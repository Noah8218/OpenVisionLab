using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputSpecialPropertyToolRuntime : IDisposable
    {
        private readonly FrameworkElement owner;
        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly string titleLocalizationKey;
        private readonly VisionToolSingleInputViewRuntime inputRuntime;

        private VisionToolSingleInputSpecialPropertyToolRuntime(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement toolContent,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action clearResultReview = null)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            shell = owner.FindName("toolShell") as VisionToolSingleInputPropertyToolShell
                ?? throw new InvalidOperationException(owner.GetType().Name + " must define a VisionToolSingleInputPropertyToolShell named 'toolShell'.");
            this.titleLocalizationKey = titleLocalizationKey ?? string.Empty;
            MoveToolContent(toolContent);

            inputRuntime = VisionToolSingleInputViewRuntime.Attach(
                shell.InputLayerComboBox,
                shell.OutputLayerComboBox,
                shell.InputPreviewFrame,
                shell.InputPreview,
                shell.OutputPreviewFrame,
                shell.OutputPreview,
                shell.CreateOutputLayerButton,
                shell.RunPreviewButton,
                shell.AddPipelineButton,
                sourceLayerChanged,
                destinationLayerChanged,
                inputPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                refreshViewState: null,
                clearResultReview: clearResultReview ?? ClearResultReview);

            ApplyLocalization();
        }

        public string SelectedInputLayer => inputRuntime.SelectedInputLayer;

        public string SelectedOutputLayer => inputRuntime.SelectedOutputLayer;

        public Border PropertyGridHost => shell.PropertyGridHost;

        public TextBlock ResultReviewText => shell.ResultReviewText;

        public TextBlock ResultGuidanceText => shell.ResultGuidanceText;

        public TextBlock SummaryText => shell.SummaryText;

        public Panel ResultReviewChips => shell.ResultReviewChips;

        public VisionToolInlinePreviewSlot InputPreview => shell.InputPreview;

        public VisionToolPresetButtonPresenter<TProperty> AttachPresetPresenter<TProperty>(
            IReadOnlyList<VisionToolPreset<TProperty>> presets,
            Action<VisionToolPreset<TProperty>> applyPreset)
        {
            return VisionToolPresetButtonPresenter<TProperty>.Attach(shell, presets, applyPreset);
        }

        public static VisionToolSingleInputSpecialPropertyToolRuntime Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement toolContent,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action clearResultReview = null)
        {
            return new VisionToolSingleInputSpecialPropertyToolRuntime(
                owner,
                titleLocalizationKey,
                toolContent,
                sourceLayerChanged,
                destinationLayerChanged,
                inputPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                clearResultReview);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            inputRuntime.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            inputRuntime.SetInputPreview(image);
        }

        public void SetInputPreview(Bitmap image, Action afterRefresh)
        {
            inputRuntime.SetInputPreview(image, afterRefresh);
        }

        public void SetOutputPreview(Bitmap image)
        {
            inputRuntime.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            VisionToolWpfStatusPresenter.Apply(shell.StatusText, status);
        }

        public void SetSummaryText(string summary)
        {
            shell.SummaryText.Text = summary ?? string.Empty;
        }

        public void ApplyLocalization()
        {
            // Special PropertyGrid tools keep custom controls above the grid while sharing the same layer/preview chrome.
            VisionToolChromePresenter.ApplySingleInputTool(
                shell.InputLayerGroup,
                shell.OutputLayerGroup,
                shell.ParameterGroup,
                shell.TitleText,
                titleLocalizationKey,
                shell.AddPipelineText,
                shell.RunPreviewText,
                shell.InputPreviewFrame,
                shell.InputPreview,
                shell.OutputLayerComboBox,
                shell.OutputPreviewFrame,
                shell.OutputPreview,
                shell.CreateOutputLayerButton);
        }

        public void ShowResultReview(string summary, bool isSuccess, IEnumerable<VisionToolResultReviewItem> items)
        {
            VisionToolResultReviewPresenter.Show(owner, shell.ResultReviewText, shell.ResultReviewChips, summary, isSuccess, items);
        }

        public void ClearResultReview()
        {
            VisionToolResultReviewPresenter.Clear(owner, shell.ResultReviewText, shell.ResultReviewChips);
            shell.ResultGuidanceText.Text = VisionToolVerificationText.PreviewNotRunCurrentParameters;
            shell.ResultGuidanceText.ToolTip = shell.ResultGuidanceText.Text;
            shell.ResultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(owner, false);
        }

        public void Dispose()
        {
            inputRuntime.Dispose();
        }

        private void MoveToolContent(FrameworkElement toolContent)
        {
            if (toolContent == null)
            {
                return;
            }

            if (toolContent.Parent is Panel panel)
            {
                panel.Children.Remove(toolContent);
            }
            else if (toolContent.Parent is ContentControl contentControl && ReferenceEquals(contentControl.Content, toolContent))
            {
                contentControl.Content = null;
            }
            else if (toolContent.Parent is Decorator decorator && ReferenceEquals(decorator.Child, toolContent))
            {
                decorator.Child = null;
            }

            toolContent.Visibility = Visibility.Visible;
            shell.ToolContent = toolContent;
            shell.ToolContentVisibility = Visibility.Visible;
        }
    }
}
