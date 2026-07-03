using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace OpenVisionLab
{
    internal sealed class VisionToolDoubleInputCustomToolRuntime : IDisposable
    {
        private readonly VisionToolDoubleInputCustomToolShell shell;
        private readonly string titleLocalizationKey;
        private readonly VisionToolDoubleInputViewRuntime inputRuntime;

        private VisionToolDoubleInputCustomToolRuntime(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Func<bool> useOffsetMode,
            Action inputALayerChanged,
            Action inputBLayerChanged,
            Action outputLayerChanged,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action runOffsetRequested,
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

            shell = owner.FindName("toolShell") as VisionToolDoubleInputCustomToolShell
                ?? throw new InvalidOperationException(owner.GetType().Name + " must define a VisionToolDoubleInputCustomToolShell named 'toolShell'.");
            this.titleLocalizationKey = titleLocalizationKey ?? string.Empty;
            MoveParameterContent(parameterContent);

            inputRuntime = VisionToolDoubleInputViewRuntime.Attach(
                shell.InputAComboBox,
                shell.InputBComboBox,
                shell.OutputLayerComboBox,
                shell.LoadInputAImageButton,
                shell.LoadInputBImageButton,
                shell.InputAPreviewFrame,
                shell.InputAPreview,
                shell.InputBPreviewFrame,
                shell.InputBPreview,
                shell.OutputPreviewFrame,
                shell.OutputPreview,
                shell.CreateOutputLayerButton,
                shell.RunPreviewButton,
                shell.RunOffsetButton,
                shell.AddPipelineButton,
                inputALayerChanged,
                inputBLayerChanged,
                outputLayerChanged,
                inputAPreviewClicked,
                inputBPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                runOffsetRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                useOffsetMode,
                refreshViewState,
                clearResultReview);

            ApplyLocalization();
        }

        public string SelectedInputLayerA => inputRuntime.SelectedInputLayerA;

        public string SelectedInputLayerB => inputRuntime.SelectedInputLayerB;

        public string SelectedOutputLayer => inputRuntime.SelectedOutputLayer;

        public static VisionToolDoubleInputCustomToolRuntime Attach(
            FrameworkElement owner,
            string titleLocalizationKey,
            FrameworkElement parameterContent,
            Func<bool> useOffsetMode,
            Action inputALayerChanged,
            Action inputBLayerChanged,
            Action outputLayerChanged,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action runOffsetRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action refreshViewState = null,
            Action clearResultReview = null)
        {
            return new VisionToolDoubleInputCustomToolRuntime(
                owner,
                titleLocalizationKey,
                parameterContent,
                useOffsetMode,
                inputALayerChanged,
                inputBLayerChanged,
                outputLayerChanged,
                inputAPreviewClicked,
                inputBPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                runOffsetRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested,
                refreshViewState,
                clearResultReview);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayerA, string selectedInputLayerB, string selectedOutputLayer)
        {
            inputRuntime.SetLayerList(layerNames, selectedInputLayerA, selectedInputLayerB, selectedOutputLayer);
        }

        public void SetInputAPreview(Bitmap image)
        {
            inputRuntime.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            inputRuntime.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            inputRuntime.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            VisionToolWpfStatusPresenter.Apply(shell.StatusText, status);
        }

        public void SetSummaryText(string text)
        {
            shell.SummaryText.Text = text ?? string.Empty;
        }

        public void SetRunOffsetText(string text)
        {
            shell.RunOffsetText.Text = text ?? string.Empty;
        }

        public void SetTitleIconKind(PackIconMaterialKind iconKind)
        {
            shell.TitleIconKind = iconKind;
        }

        public void SetInputBPreviewVisible(bool visible)
        {
            shell.SetInputBPreviewVisible(visible);
        }

        public void SetOffsetActionsVisible(bool useOffsetMode)
        {
            shell.SetOffsetActionsVisible(useOffsetMode);
        }

        public void ApplyLocalization()
        {
            VisionToolChromePresenter.ApplyDoubleInputTool(
                shell.InputAGroup,
                shell.InputBGroup,
                shell.OutputLayerGroup,
                shell.TitleText,
                titleLocalizationKey,
                shell.AddPipelineText,
                shell.RunPreviewText,
                shell.InputAPreviewFrame,
                shell.InputAPreview,
                shell.InputBPreviewFrame,
                shell.InputBPreview,
                shell.OutputLayerComboBox,
                shell.OutputPreviewFrame,
                shell.OutputPreview,
                shell.LoadInputAImageButton,
                shell.LoadInputBImageButton,
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
            shell.ParameterContent = parameterContent;
        }
    }
}
