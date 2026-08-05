using OpenVisionLab.Vision2D.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputMatchingToolRuntime<TProperty> : IDisposable
    {
        private readonly VisionToolSingleInputPropertyToolShell shell;
        private readonly string titleLocalizationKey;
        private readonly VisionToolSingleInputViewRuntime inputRuntime;
        private readonly VisionToolMatchingPropertyRuntime<TProperty> matchingRuntime;
        private readonly VisionToolPresetButtonPresenter<TProperty> presetPresenter;

        private VisionToolSingleInputMatchingToolRuntime(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            shell = owner.FindName("toolShell") as VisionToolSingleInputPropertyToolShell
                ?? throw new InvalidOperationException(owner.GetType().Name + " must define a VisionToolSingleInputPropertyToolShell named 'toolShell'.");
            this.titleLocalizationKey = titleLocalizationKey ?? string.Empty;
            VisionToolVerificationGuideView verificationGuideView = CreateVerificationGuideView();
            verificationGuideView.IsCompactMode = true;
            shell.ToolContent = verificationGuideView;
            shell.ToolContentVisibility = Visibility.Visible;

            Stopwatch phaseStopwatch = Stopwatch.StartNew();
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
                clearResultReview: ClearResultReview);
            OpenVisionToolOpenProfiler.Record("AttachMatchingInputRuntime", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            matchingRuntime = VisionToolMatchingPropertyRuntime<TProperty>.Attach(
                owner,
                shell.PropertyGridHost,
                presenter,
                shell.SummaryText,
                shell.TemplateStatusText,
                shell.TemplateStatusIcon,
                verificationGuideView,
                shell.ResultReviewText,
                shell.ResultGuidanceText,
                shell.ResultReviewChips,
                inputRuntime.RequestRunPreview,
                RefreshInputRoiOverlay);
            OpenVisionToolOpenProfiler.Record("AttachMatchingPropertyRuntime", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            presetPresenter = VisionToolPresetButtonPresenter<TProperty>.Attach(
                shell,
                VisionToolPresetCatalog.GetMatchingPresets<TProperty>(),
                preset => ApplyPreset(preset));
            OpenVisionToolOpenProfiler.Record("AttachMatchingPresetPresenter", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            ApplyLocalization();
            OpenVisionToolOpenProfiler.Record("ApplyMatchingLocalization", phaseStopwatch.ElapsedMilliseconds);
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

        public static VisionToolSingleInputMatchingToolRuntime<TProperty> Attach(
            FrameworkElement owner,
            VisionToolPropertyGridPresenter<TProperty> presenter,
            string titleLocalizationKey,
            Action sourceLayerChanged,
            Action destinationLayerChanged,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action<VisionToolPreviewImageRole> loadPreviewImageRequested,
            Action<VisionToolPreviewImageRole> savePreviewImageRequested)
        {
            return new VisionToolSingleInputMatchingToolRuntime<TProperty>(
                owner,
                presenter,
                titleLocalizationKey,
                sourceLayerChanged,
                destinationLayerChanged,
                inputPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                loadPreviewImageRequested,
                savePreviewImageRequested);
        }

        public TProperty CreateProperty()
        {
            return matchingRuntime.CreateProperty();
        }

        public void SetTemplatePathForTest(string path)
        {
            matchingRuntime.SetTemplatePathForTest(path);
        }

        public void ConfigurePropertyForTest(Action<TProperty> configure)
        {
            matchingRuntime.ConfigurePropertyForTest(configure);
        }

        public bool ApplyPresetForTest(string presetId)
        {
            VisionToolPreset<TProperty> preset = VisionToolPresetCatalog.GetMatchingPresets<TProperty>()
                .FirstOrDefault(item => string.Equals(item.Id, presetId, StringComparison.OrdinalIgnoreCase));
            return ApplyPreset(preset);
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            inputRuntime.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            inputRuntime.SetInputPreview(image, RefreshInputRoiOverlay);
        }

        public void SetOutputPreview(Bitmap image)
        {
            inputRuntime.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            VisionToolWpfStatusPresenter.Apply(shell.StatusText, status);
        }

        public void SetResultReview(string title, IEnumerable<MatchingResult> results, TimeSpan? tactTime = null)
        {
            matchingRuntime.SetResultReview(title, results, tactTime);
        }

        public void ApplyLocalization()
        {
            // Matching tools use the shared shell but keep template status in the matching-specific runtime.
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
            presetPresenter?.ApplyLocalization();
        }

        private static VisionToolVerificationGuideView CreateVerificationGuideView()
        {
            return new VisionToolVerificationGuideView
            {
                Margin = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        public void RefreshSelectedObject()
        {
            matchingRuntime.RefreshSelectedObject();
        }

        public void UpdateSummary()
        {
            matchingRuntime.UpdateSummary();
        }

        public void ClearResultReview()
        {
            matchingRuntime?.ClearResultReview();
        }

        private void RefreshInputRoiOverlay()
        {
            matchingRuntime?.RefreshInputRoiOverlay(shell.InputPreview);
        }

        private bool ApplyPreset(VisionToolPreset<TProperty> preset)
        {
            return matchingRuntime?.ApplyPreset(preset) ?? false;
        }

        public void Dispose()
        {
            presetPresenter.Dispose();
            inputRuntime.Dispose();
            matchingRuntime.Dispose();
        }
    }
}
