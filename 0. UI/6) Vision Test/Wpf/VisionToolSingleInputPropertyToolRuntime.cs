using OpenVisionLab.PropertyGrid;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolSingleInputPropertyToolRuntime<TProperty> : IDisposable
    {
        private readonly FrameworkElement owner;
        private readonly VisionToolPropertyGridPresenter<TProperty> presenter;
        private readonly string titleLocalizationKey;
        private readonly HeaderedContentControl inputLayerGroup;
        private readonly HeaderedContentControl outputLayerGroup;
        private readonly HeaderedContentControl parameterGroup;
        private readonly TextBlock titleText;
        private readonly TextBlock addPipelineText;
        private readonly TextBlock runPreviewText;
        private readonly TextBlock statusText;
        private readonly TextBlock summaryText;
        private readonly TextBlock resultReviewText;
        private readonly TextBlock resultGuidanceText;
        private readonly Panel resultReviewChips;
        private readonly Border inputPreviewFrame;
        private readonly VisionToolInlinePreviewSlot inputPreview;
        private readonly Border outputPreviewFrame;
        private readonly VisionToolInlinePreviewSlot outputPreview;
        private readonly ComboBox outputLayerComboBox;
        private readonly Button createOutputLayerButton;
        private readonly VisionToolSingleInputViewRuntime inputRuntime;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolDebouncedPreviewScheduler autoPreviewScheduler;
        private readonly Action<TProperty> refreshVerificationGuide;
        private readonly VisionToolPresetButtonPresenter<TProperty> presetPresenter;

        private VisionToolSingleInputPropertyToolRuntime(
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
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action<PropertyGridPropertyValueChangedEventArgs> beforePropertyRefresh = null,
            Action refreshOverlay = null,
            Action beforeAutoPreview = null,
            bool autoPreviewOnPropertyChanged = false,
            Action<TProperty> refreshVerificationGuide = null,
            IReadOnlyList<VisionToolPreset<TProperty>> presets = null)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.titleLocalizationKey = titleLocalizationKey ?? string.Empty;
            Stopwatch phaseStopwatch = Stopwatch.StartNew();
            VisionToolSingleInputPropertyToolShell shell = owner.FindName("toolShell") as VisionToolSingleInputPropertyToolShell;

            inputLayerGroup = Resolve(shell, item => item.InputLayerGroup, "gbInputLayer");
            outputLayerGroup = Resolve(shell, item => item.OutputLayerGroup, "gbOutputLayer");
            parameterGroup = Resolve(shell, item => item.ParameterGroup, "gbParameters");
            titleText = Resolve(shell, item => item.TitleText, "txtTitle");
            addPipelineText = Resolve(shell, item => item.AddPipelineText, "txtAddPipelineText");
            runPreviewText = Resolve(shell, item => item.RunPreviewText, "txtRunPreviewText");
            statusText = Resolve(shell, item => item.StatusText, "txtStatus");
            summaryText = Resolve(shell, item => item.SummaryText, "txtSummary");
            resultReviewText = Resolve(shell, item => item.ResultReviewText, "txtResultReview");
            resultGuidanceText = Resolve(shell, item => item.ResultGuidanceText, "txtResultGuidance");
            resultReviewChips = Resolve(shell, item => item.ResultReviewChips, "resultReviewChips");
            inputPreviewFrame = Resolve(shell, item => item.InputPreviewFrame, "bdInputPreview");
            inputPreview = Resolve(shell, item => item.InputPreview, "imgInputPreview");
            outputPreviewFrame = Resolve(shell, item => item.OutputPreviewFrame, "bdOutputPreview");
            outputPreview = Resolve(shell, item => item.OutputPreview, "imgOutputPreview");
            outputLayerComboBox = Resolve(shell, item => item.OutputLayerComboBox, "cbOutputLayer");
            createOutputLayerButton = Resolve(shell, item => item.CreateOutputLayerButton, "btnCreateOutputLayer");
            this.refreshVerificationGuide = refreshVerificationGuide;
            OpenVisionToolOpenProfiler.Record("ResolveSingleInputPropertyControls", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            inputRuntime = VisionToolSingleInputViewRuntime.Attach(
                Resolve(shell, item => item.InputLayerComboBox, "cbInputLayer"),
                Resolve(shell, item => item.OutputLayerComboBox, "cbOutputLayer"),
                inputPreviewFrame,
                inputPreview,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton,
                Resolve(shell, item => item.RunPreviewButton, "btnRunPreview"),
                Resolve(shell, item => item.AddPipelineButton, "btnAddPipeline"),
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
            OpenVisionToolOpenProfiler.Record("AttachSingleInputViewRuntime", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            propertyChangeController = new VisionToolPropertyChangeController(
                UpdateSummary,
                ClearResultReview,
                e =>
                {
                    beforePropertyRefresh?.Invoke(e);
                    presenter.PersistSelectedObject();
                },
                () =>
                {
                    refreshOverlay?.Invoke();
                    RefreshInputRoiOverlay();
                },
                autoPreviewOnPropertyChanged ? ScheduleAutoPreview : null,
                autoPreviewOnPropertyChanged ? CancelAutoPreview : null,
                autoPreviewOnPropertyChanged ? VisionToolPropertyPreviewPolicy.ShouldScheduleAutoPreview : null);
            OpenVisionToolOpenProfiler.Record("CreatePropertyChangeController", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            propertyGridController = VisionToolPropertyGridHost.Attach(
                Resolve(shell, item => item.PropertyGridHost, "propertyGridHost"),
                presenter.SelectedObject,
                propertyChangeController.OnPropertyValueChanged);
            OpenVisionToolOpenProfiler.Record("AttachPropertyGridHost", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            if (shell != null)
            {
                presetPresenter = VisionToolPresetButtonPresenter<TProperty>.Attach(
                    shell,
                    presets ?? Array.Empty<VisionToolPreset<TProperty>>(),
                    preset => ApplyPreset(preset));
            }

            OpenVisionToolOpenProfiler.Record("AttachPropertyPresetPresenter", phaseStopwatch.ElapsedMilliseconds);

            phaseStopwatch.Restart();
            ApplyLocalization();
            OpenVisionToolOpenProfiler.Record("ApplyPropertyToolLocalization", phaseStopwatch.ElapsedMilliseconds);
            phaseStopwatch.Restart();
            UpdateSummary();
            OpenVisionToolOpenProfiler.Record("UpdatePropertyToolSummary", phaseStopwatch.ElapsedMilliseconds);
            phaseStopwatch.Restart();
            ClearResultReview();
            OpenVisionToolOpenProfiler.Record("ClearPropertyToolResultReview", phaseStopwatch.ElapsedMilliseconds);

            if (autoPreviewOnPropertyChanged)
            {
                // PropertyGrid tools share one debounced preview path instead of adding tool-specific timers.
                autoPreviewScheduler = new VisionToolDebouncedPreviewScheduler(owner, () =>
                {
                    beforeAutoPreview?.Invoke();
                    inputRuntime.RequestRunPreview();
                }, 120);
            }
        }

        public string SelectedInputLayer => inputRuntime.SelectedInputLayer;

        public string SelectedOutputLayer => inputRuntime.SelectedOutputLayer;

        public static VisionToolSingleInputPropertyToolRuntime<TProperty> Attach(
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
            Action<VisionToolPreviewImageRole> savePreviewImageRequested,
            Action<PropertyGridPropertyValueChangedEventArgs> beforePropertyRefresh = null,
            Action refreshOverlay = null,
            Action beforeAutoPreview = null,
            bool autoPreviewOnPropertyChanged = false,
            Action<TProperty> refreshVerificationGuide = null,
            IReadOnlyList<VisionToolPreset<TProperty>> presets = null)
        {
            return new VisionToolSingleInputPropertyToolRuntime<TProperty>(
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
                savePreviewImageRequested,
                beforePropertyRefresh,
                refreshOverlay,
                beforeAutoPreview,
                autoPreviewOnPropertyChanged,
                refreshVerificationGuide,
                presets);
        }

        public TProperty CreateProperty()
        {
            if (propertyGridController.CommitPendingEdit())
            {
                presenter.PersistSelectedObject();
                UpdateSummary();
            }

            return presenter.CreateProperty();
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
            VisionToolWpfStatusPresenter.Apply(statusText, status);
        }

        public void ApplyLocalization()
        {
            // A shared shell lets new single-input PropertyGrid tools avoid repeating the same XAML and chrome wiring.
            VisionToolChromePresenter.ApplySingleInputTool(
                inputLayerGroup,
                outputLayerGroup,
                parameterGroup,
                titleText,
                titleLocalizationKey,
                addPipelineText,
                runPreviewText,
                inputPreviewFrame,
                inputPreview,
                outputLayerComboBox,
                outputPreviewFrame,
                outputPreview,
                createOutputLayerButton);
            presetPresenter?.ApplyLocalization();
        }

        public void RefreshSelectedObject()
        {
            propertyGridController.RefreshSelectedObject();
        }

        public void UpdateSummary()
        {
            summaryText.Text = presenter.Summary;
            RefreshVerificationGuide();
        }

        public void ShowResultReview(string summary, bool isSuccess, IEnumerable<VisionToolResultReviewItem> items)
        {
            VisionToolResultReviewPresenter.Show(owner, resultReviewText, resultReviewChips, summary, isSuccess, items);
        }

        public void ClearResultReview()
        {
            VisionToolResultReviewPresenter.Clear(owner, resultReviewText, resultReviewChips);
            resultGuidanceText.Text = VisionToolVerificationText.PreviewNotRunCurrentParameters;
            resultGuidanceText.ToolTip = resultGuidanceText.Text;
            resultGuidanceText.Foreground = VisionToolResultReviewPresenter.ResolveStatusBrush(owner, false);
        }

        public void Dispose()
        {
            presetPresenter?.Dispose();
            autoPreviewScheduler?.Dispose();
            inputRuntime.Dispose();
            propertyGridController.Dispose();
        }

        private bool ApplyPreset(VisionToolPreset<TProperty> preset)
        {
            if (preset == null)
            {
                return false;
            }

            if (presenter.SelectedObject is not TProperty property)
            {
                return false;
            }

            preset.ApplyTo(property);
            presenter.PersistSelectedObject();
            propertyGridController.RefreshAndApplyVisibilityRules();
            UpdateSummary();
            RefreshInputRoiOverlay();
            ClearResultReview();
            return true;
        }

        private void ScheduleAutoPreview()
        {
            autoPreviewScheduler?.Schedule();
        }

        private void CancelAutoPreview()
        {
            autoPreviewScheduler?.Cancel();
        }

        private void RefreshInputRoiOverlay()
        {
            if (presenter.SelectedObject is OpenCvPropertyBase property)
            {
                inputPreview.SetOpenCvRoiOverlays(property);
            }
        }

        private void RefreshVerificationGuide()
        {
            if (refreshVerificationGuide != null && presenter.SelectedObject is TProperty property)
            {
                refreshVerificationGuide(property);
            }
        }

        private TControl FindRequired<TControl>(string name)
            where TControl : class
        {
            TControl control = owner.FindName(name) as TControl;
            if (control == null)
            {
                throw new InvalidOperationException(
                    owner.GetType().Name + " must define a " + typeof(TControl).Name + " named '" + name + "'.");
            }

            return control;
        }

        private TControl Resolve<TControl>(
            VisionToolSingleInputPropertyToolShell shell,
            Func<VisionToolSingleInputPropertyToolShell, TControl> fromShell,
            string fallbackName)
            where TControl : class
        {
            TControl control = shell == null ? null : fromShell(shell);
            return control ?? FindRequired<TControl>(fallbackName);
        }
    }
}
