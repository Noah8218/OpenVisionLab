using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal static class VisionToolChromePresenter
    {
        public static void ApplySingleInputTool(
            HeaderedContentControl inputLayerGroup,
            HeaderedContentControl outputLayerGroup,
            HeaderedContentControl parameterGroup,
            TextBlock titleText,
            string titleLocalizationKey,
            TextBlock addPipelineText,
            TextBlock runPreviewText,
            FrameworkElement inputPreviewFrame,
            FrameworkElement inputPreviewImage,
            FrameworkElement outputLayerSelector,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewImage,
            FrameworkElement createOutputLayerButton)
        {
            SetHeader(inputLayerGroup, "VisionTest.InputLayer");
            SetHeader(outputLayerGroup, "VisionTest.OutputLayer");
            SetHeader(parameterGroup, "Pipeline.ResultRow.Parameters");
            SetText(titleText, titleLocalizationKey);
            SetText(addPipelineText, "ToolView.AddPipeline");
            SetText(runPreviewText, "Pipeline.RunPreview");

            string inputTooltip = OpenVisionLanguageService.T("PipelineFlow.ViewInputImage");
            string outputTooltip = OpenVisionLanguageService.T("PipelineFlow.ViewOutputImage");
            ApplyTooltip(inputPreviewFrame, inputTooltip);
            ApplyTooltip(inputPreviewImage, inputTooltip);
            ApplyTooltip(outputPreviewFrame, outputTooltip);
            ApplyTooltip(outputPreviewImage, outputTooltip);
            ApplyTooltip(outputLayerSelector, OpenVisionLanguageService.T("VisionTest.OutputLayerWriteTargetTooltip"));

            string createOutputLayerText = OpenVisionLanguageService.T("VisionTest.CreateOutputLayer");
            ApplyTooltip(createOutputLayerButton, createOutputLayerText);
        }


        public static void ApplyDoubleInputTool(
            HeaderedContentControl inputAGroup,
            HeaderedContentControl inputBGroup,
            HeaderedContentControl outputLayerGroup,
            TextBlock titleText,
            string titleLocalizationKey,
            TextBlock addPipelineText,
            TextBlock runPreviewText,
            FrameworkElement inputAPreviewFrame,
            FrameworkElement inputAPreviewImage,
            FrameworkElement inputBPreviewFrame,
            FrameworkElement inputBPreviewImage,
            FrameworkElement outputLayerSelector,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewImage,
            FrameworkElement loadInputAImageButton,
            FrameworkElement loadInputBImageButton,
            FrameworkElement createOutputLayerButton)
        {
            string inputText = OpenVisionLanguageService.T("VisionTest.InputLayer");
            if (inputAGroup != null)
            {
                inputAGroup.Header = inputText + " A";
            }

            if (inputBGroup != null)
            {
                inputBGroup.Header = inputText + " B";
            }

            SetHeader(outputLayerGroup, "VisionTest.OutputLayer");
            SetText(titleText, titleLocalizationKey);
            SetText(addPipelineText, "ToolView.AddPipeline");
            SetText(runPreviewText, "Pipeline.RunPreview");

            string inputTooltip = OpenVisionLanguageService.T("PipelineFlow.ViewInputImage");
            string inputAText = inputTooltip + " A";
            string inputBText = inputTooltip + " B";
            string outputTooltip = OpenVisionLanguageService.T("PipelineFlow.ViewOutputImage");
            ApplyTooltip(inputAPreviewFrame, inputAText);
            ApplyTooltip(inputAPreviewImage, inputAText);
            ApplyTooltip(inputBPreviewFrame, inputBText);
            ApplyTooltip(inputBPreviewImage, inputBText);
            ApplyTooltip(outputPreviewFrame, outputTooltip);
            ApplyTooltip(outputPreviewImage, outputTooltip);
            ApplyTooltip(outputLayerSelector, OpenVisionLanguageService.T("VisionTest.OutputLayerWriteTargetTooltip"));

            string loadImageText = OpenVisionLanguageService.T("ToolView.LoadImage");
            ApplyTooltip(loadInputAImageButton, loadImageText + " A");
            ApplyTooltip(loadInputBImageButton, loadImageText + " B");

            string createOutputLayerText = OpenVisionLanguageService.T("VisionTest.CreateOutputLayer");
            ApplyTooltip(createOutputLayerButton, createOutputLayerText);
        }
        public static void ApplyTooltip(FrameworkElement element, string text)
        {
            if (element == null)
            {
                return;
            }

            string resolvedText = text ?? string.Empty;
            element.ToolTip = resolvedText;
            AutomationProperties.SetName(element, resolvedText);
        }

        private static void SetHeader(HeaderedContentControl control, string localizationKey)
        {
            if (control != null)
            {
                string text = OpenVisionLanguageService.T(localizationKey);
                if (TrySetHeaderText(control.Header as DependencyObject, text))
                {
                    return;
                }

                control.Header = text;
            }
        }

        private static void SetText(TextBlock textBlock, string localizationKey)
        {
            if (textBlock != null)
            {
                textBlock.Text = OpenVisionLanguageService.T(localizationKey);
            }
        }

        private static bool TrySetHeaderText(DependencyObject header, string text)
        {
            if (header == null)
            {
                return false;
            }

            if (header is TextBlock textBlock
                && string.Equals(textBlock.Name, "txtParameterHeader", System.StringComparison.Ordinal))
            {
                textBlock.Text = text;
                return true;
            }

            foreach (object child in LogicalTreeHelper.GetChildren(header))
            {
                if (child is DependencyObject dependencyObject && TrySetHeaderText(dependencyObject, text))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
