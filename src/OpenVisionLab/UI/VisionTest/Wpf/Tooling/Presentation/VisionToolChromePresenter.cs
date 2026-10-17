using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenVisionLab
{
    internal static class VisionToolChromePresenter
    {
        internal static void ApplyWorkbenchEditor(TextBlock title, TextBlock addText, TextBlock runText, Button addButton,
            Button runButton, FrameworkElement stepActions, Button defaultsButton, OpenVisionWorkbenchToolViewModel editor, Button nImageButton = null,
            TextBlock nImageText = null, FrameworkElement runShortcut = null, TextBlock alternateRunText = null, FrameworkElement alternateRunShortcut = null, Button alternateRunButton = null,
            ContentControl acceptanceHost = null)
        {
            title.SetBinding(TextBlock.TextProperty, new Binding(nameof(editor.Title)) { Source = editor });
            addText.SetBinding(TextBlock.TextProperty, new Binding("[Workbench.Tool.AddToPipeline]") { Source = editor });
            runText.SetBinding(TextBlock.TextProperty, new Binding(nameof(editor.RunText)) { Source = editor });
            alternateRunText?.SetBinding(TextBlock.TextProperty, new Binding(nameof(editor.RunText)) { Source = editor });
            runButton.Command = editor.RunCommand;
            if (alternateRunButton != null) { alternateRunButton.Command = editor.RunCommand; }
            runButton.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(editor.CanUseToolActions)) { Source = editor });
            alternateRunButton?.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(editor.CanUseToolActions)) { Source = editor });
            runButton.SetResourceReference(FrameworkElement.StyleProperty, "OpenVision.Workbench.PrimaryButtonStyle");
            alternateRunButton?.SetResourceReference(FrameworkElement.StyleProperty, "OpenVision.Workbench.PrimaryButtonStyle");
            addButton.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(editor.IsNewTest)) { Source = editor, Converter = new BooleanToVisibilityConverter() });
            addButton.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(editor.CanAddToPipeline)) { Source = editor });
            addButton.SetBinding(FrameworkElement.ToolTipProperty, new Binding(nameof(editor.AddTooltip)) { Source = editor });
            addButton.ContextMenu = (ContextMenu)addButton.FindResource("WorkbenchToolAddMenu");
            addButton.ContextMenu.DataContext = editor;
            defaultsButton.DataContext = editor;
            defaultsButton.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(editor.IsNewTest)) { Source = editor, Converter = new BooleanToVisibilityConverter() });
            stepActions.DataContext = editor;
            stepActions.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(editor.IsStepEditing)) { Source = editor, Converter = new BooleanToVisibilityConverter() });
            if (acceptanceHost != null)
            {
                acceptanceHost.Content = editor;
                acceptanceHost.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(editor.HasAcceptance)) { Source = editor, Converter = new BooleanToVisibilityConverter() });
            }
            if (nImageButton?.Visibility == Visibility.Visible)
            {
                nImageButton.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(editor.IsNewTest)) { Source = editor, Converter = new BooleanToVisibilityConverter() });
                nImageButton.SetBinding(UIElement.IsEnabledProperty, new Binding(nameof(editor.CanUseToolActions)) { Source = editor });
                nImageText?.SetBinding(TextBlock.TextProperty, new Binding("[Workbench.Tool.NTest]") { Source = editor });
                nImageButton.SetResourceReference(FrameworkElement.StyleProperty, "OpenVision.Workbench.ButtonStyle");
                nImageButton.Height = 34;
                nImageButton.FontSize = 12;
                nImageButton.Padding = new Thickness(8, 2, 8, 2);
            }
            addButton.SetResourceReference(FrameworkElement.StyleProperty, "OpenVision.Workbench.ButtonStyle");
            addButton.Height = 34;
            addButton.Padding = new Thickness(8, 2, 8, 2);
            if (runShortcut != null) { runShortcut.Visibility = Visibility.Visible; }
            if (alternateRunShortcut != null) { alternateRunShortcut.Visibility = Visibility.Visible; }
        }

        internal static void ApplyWorkbenchRoute(HeaderedContentControl input, HeaderedContentControl output, OpenVisionWorkbenchToolViewModel editor, HeaderedContentControl inputB = null)
        {
            input.SetBinding(HeaderedContentControl.HeaderProperty, new Binding(inputB == null ? "[Workbench.Tool.Input]" : "[Workbench.Tool.InputA]") { Source = editor });
            output.SetBinding(HeaderedContentControl.HeaderProperty, new Binding("[Workbench.Tool.Output]") { Source = editor });
            inputB?.SetBinding(HeaderedContentControl.HeaderProperty, new Binding("[Workbench.Tool.InputB]") { Source = editor });
            foreach (HeaderedContentControl group in new[] { input, output, inputB })
            {
                if (group != null) { group.Resources["VisionTool.WorkbenchOriginalText"] = editor["Workbench.Pane.Original"]; }
            }
        }

        internal static void ApplyWorkbenchTemplate(VisionToolSingleInputPropertyToolShell shell, OpenVisionWorkbenchToolViewModel editor)
        {
            if (!editor.HasTemplateCard) { return; }
            shell.WorkbenchTemplateHost.Content = editor;
            shell.WorkbenchTemplateHost.Visibility = Visibility.Visible;
            if (shell.FindName("legacyTemplateStatus") is FrameworkElement legacy) { legacy.Visibility = Visibility.Collapsed; }
            if (shell.FindName("bdTemplateStatus") is Border border)
            {
                border.SetResourceReference(Border.BackgroundProperty, "OpenVision.Workbench.PanelAltBrush");
                border.SetResourceReference(Border.BorderBrushProperty, "OpenVision.Workbench.LineAltBrush");
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(10, 4, 10, 4);
            }
            if (shell.ToolContent is Grid content)
            {
                foreach (UIElement child in content.Children)
                {
                    if (child is VisionToolVerificationGuideView guide) { guide.Visibility = Visibility.Collapsed; }
                    else if (child is AutoMPointTeachingPanel panel)
                    {
                        panel.TitleText.SetBinding(TextBlock.TextProperty, new Binding("[Workbench.Tool.AutoMPoint]") { Source = editor });
                        if (panel.SuggestedText.Parent is FrameworkElement badge) { badge.Visibility = Visibility.Collapsed; }
                    }
                }
            }
        }

        internal static void ApplyWorkbenchResult(FrameworkElement shell, OpenVisionWorkbenchToolViewModel editor)
        {
            if (shell.FindName("workbenchResultHost") is not ContentControl resultHost) { return; }
            resultHost.Content = editor;
            resultHost.Visibility = Visibility.Visible;
            if (shell is VisionToolSingleInputPropertyToolShell single && single.ToolContent is VisionToolVerificationGuideView
                && (editor.HasTemplateCard || editor.CurrentStep?.ToolType is "Blob" or "Contour")) { single.ToolContentVisibility = Visibility.Collapsed; }
            foreach (string name in new[] { "bdSummary", "bdStatus", "bdResultReview" })
            {
                if (shell.FindName(name) is FrameworkElement legacy) { legacy.Visibility = Visibility.Collapsed; }
            }
        }

        internal static void RevealWorkbenchResult(FrameworkElement shell)
        {
            if (shell?.FindName("workbenchResultHost") is FrameworkElement result)
            {
                shell.UpdateLayout();
                result.BringIntoView();
            }
        }

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
            SetHeader(inputAGroup, "VisionTest.InputLayer", " A");
            SetHeader(inputBGroup, "VisionTest.InputLayer", " B");

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

        private static void SetHeader(HeaderedContentControl control, string localizationKey, string suffix = "")
        {
            if (control != null)
            {
                if (control.GetBindingExpression(HeaderedContentControl.HeaderProperty) != null)
                {
                    control.Resources["VisionTool.WorkbenchOriginalText"] = OpenVisionLanguageService.T("Workbench.Pane.Original");
                    return;
                }
                string text = OpenVisionLanguageService.T(localizationKey) + suffix;
                if (TrySetHeaderText(control.Header as DependencyObject, text))
                {
                    return;
                }

                control.Header = text;
            }
        }

        private static void SetText(TextBlock textBlock, string localizationKey)
        {
            if (textBlock != null && textBlock.GetBindingExpression(TextBlock.TextProperty) == null)
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
