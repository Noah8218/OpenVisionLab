using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputCustomToolRuntime : IDisposable
    {
        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly FrameworkElement owner;
        private readonly string titleLocalizationKey;
        private readonly VisionToolSingleInputViewRuntime inputRuntime;

        private VisionToolSingleInputCustomToolRuntime(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            shell = owner.FindName("toolShell") as VisionToolSingleInputPropertyToolShell
                ?? throw new InvalidOperationException(owner.GetType().Name + " must define a VisionToolSingleInputPropertyToolShell named 'toolShell'.");
            this.owner = owner;
            this.titleLocalizationKey = titleLocalizationKey ?? string.Empty;
            MoveParameterContent(parameterContent);

            // The shared shell owns the layer selectors and preview slots; custom tools keep only their parameter controls.
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
                refreshViewState,
                clearResultReview);

            ApplyLocalization();
        }

        public string SelectedInputLayer => inputRuntime.SelectedInputLayer;

        public string SelectedOutputLayer => inputRuntime.SelectedOutputLayer;

        public string ResultReviewText
        {
            get
            {
                string summary = shell.ResultReviewText?.Text ?? string.Empty;
                string guidance = shell.ResultGuidanceText?.Text ?? string.Empty;
                return string.IsNullOrWhiteSpace(guidance)
                    ? summary
                    : summary + " / " + guidance;
            }
        }

        public static VisionToolSingleInputCustomToolRuntime Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            return new VisionToolSingleInputCustomToolRuntime(
                owner,
                titleLocalizationKey,
                parameterContent,
                sourceLayerChanged,
                destinationLayerChanged,
                inputPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                refreshViewState,
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

        public void SetOutputPreview(Bitmap image)
        {
            inputRuntime.SetOutputPreview(image);
        }

        public void RequestRunPreview()
        {
            inputRuntime.RequestRunPreview();
        }

        public void SetStatus(string status)
        {
            VisionToolWpfStatusPresenter.Apply(shell.StatusText, status);
        }

        public void SetTitleText(string text)
        {
            shell.TitleText.Text = text ?? string.Empty;
        }

        public void SetTitleIconKind(PackIconMaterialKind iconKind)
        {
            shell.TitleIconKind = iconKind;
        }

        public void SetAddPipelineVisible(bool visible)
        {
            shell.AddPipelineButton.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void BindSummary(BindingBase binding)
        {
            if (binding == null)
            {
                BindingOperations.ClearBinding(shell.SummaryText, TextBlock.TextProperty);
                return;
            }

            BindingOperations.SetBinding(shell.SummaryText, TextBlock.TextProperty, binding);
        }

        public void RefreshSummaryBinding()
        {
            shell.SummaryText.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        public void SetSummaryText(string text)
        {
            BindingOperations.ClearBinding(shell.SummaryText, TextBlock.TextProperty);
            shell.SummaryText.Text = text ?? string.Empty;
        }

        public void ShowResultReview(
            string summary,
            bool isSuccess,
            IEnumerable<VisionToolResultReviewItem> items,
            string guidance)
        {
            VisionToolResultReviewPresenter.Show(owner, shell.ResultReviewText, shell.ResultReviewChips, summary, isSuccess, items);
            ApplyResultGuidance(isSuccess, guidance);
        }

        public void ClearResultReview()
        {
            VisionToolResultReviewPresenter.Clear(owner, shell.ResultReviewText, shell.ResultReviewChips);
            ApplyResultGuidance(false, VisionToolVerificationText.PreviewNotRunCurrentParameters);
        }

        public void ApplyLocalization()
        {
            VisionToolChromePresenter.ApplySingleInputTool(
                shell.InputLayerGroup,
                shell.OutputLayerGroup,
                shell.ParameterGroup,
                string.IsNullOrWhiteSpace(titleLocalizationKey) ? null : shell.TitleText,
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

        public void Dispose()
        {
            inputRuntime.Dispose();
        }

        private void MoveParameterContent(FrameworkElement parameterContent)
        {
            if (parameterContent == null)
            {
                return;
            }

            if (parameterContent.Parent is Panel panel)
            {
                panel.Children.Remove(parameterContent);
            }
            else if (parameterContent.Parent is ContentControl contentControl && ReferenceEquals(contentControl.Content, parameterContent))
            {
                contentControl.Content = null;
            }
            else if (parameterContent.Parent is Decorator decorator && ReferenceEquals(decorator.Child, parameterContent))
            {
                decorator.Child = null;
            }

            parameterContent.Visibility = Visibility.Visible;
            shell.PropertyGridHostVisibility = Visibility.Collapsed;
            shell.ParameterContentVisibility = Visibility.Visible;
            shell.ParameterContent = parameterContent;
        }

        private void ApplyResultGuidance(bool isSuccess, string guidance)
        {
            if (shell.ResultGuidanceText == null)
            {
                return;
            }

            shell.ResultGuidanceText.Text = string.IsNullOrWhiteSpace(guidance) ? "-" : guidance.Trim();
            shell.ResultGuidanceText.ToolTip = shell.ResultGuidanceText.Text;
            shell.ResultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(owner, isSuccess);
        }
    }
}
