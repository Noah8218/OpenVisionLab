using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public partial class SimplePreprocessToolWpfView : UserControl, ISingleInputVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly SimplePreprocessParameterController parameterController;
        private readonly SimplePreprocessTextPresenter textPresenter;
        private readonly VisionToolSingleInputCustomToolController toolController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private bool suppressEvents;

        public SimplePreprocessToolWpfView()
        {
            InitializeComponent();
            toolController = VisionToolSingleInputCustomToolController.Attach(
                this,
                string.Empty,
                parameterContentHost,
                refreshViewState: null,
                clearResultReview: ClearResultReview,
                applyToolLocalization: ApplyLocalization);
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, () => toolController?.RequestRunPreview());
            textPresenter = new SimplePreprocessTextPresenter(
                toolController.SetTitleText,
                toolController.SetTitleIconKind,
                toolController.SetSummaryText);
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

        public event EventHandler SourceLayerChanged
        {
            add { toolController.SourceLayerChanged += value; }
            remove { toolController.SourceLayerChanged -= value; }
        }

        public event EventHandler DestinationLayerChanged
        {
            add { toolController.DestinationLayerChanged += value; }
            remove { toolController.DestinationLayerChanged -= value; }
        }

        public event EventHandler InputPreviewClicked
        {
            add { toolController.InputPreviewClicked += value; }
            remove { toolController.InputPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { toolController.OutputPreviewClicked += value; }
            remove { toolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { toolController.CreateOutputLayerRequested += value; }
            remove { toolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { toolController.RunPreviewRequested += value; }
            remove { toolController.RunPreviewRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { toolController.AddPipelineRequested += value; }
            remove { toolController.AddPipelineRequested -= value; }
        }

        public event EventHandler ParameterChanged = delegate { };

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { toolController.LoadPreviewImageRequested += value; }
            remove { toolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { toolController.SavePreviewImageRequested += value; }
            remove { toolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayer => toolController.SelectedInputLayer;
        public string SelectedOutputLayer => toolController.SelectedOutputLayer;
        public string ResultReviewTextForTest => toolController.ResultReviewText;

        public void DisposeView()
        {
            toolController.Dispose();
            previewScheduler.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
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

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputLayer, string selectedOutputLayer)
        {
            toolController.SetLayerList(layerNames, selectedInputLayer, selectedOutputLayer);
        }

        public void SetInputPreview(Bitmap image)
        {
            toolController.SetInputPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolController.SetOutputPreview(image);
        }

        public void SetSummary(string summary)
        {
            textPresenter.SetSummary(summary);
            parameterChangeController.RefreshProgrammatic();
        }

        public void SetStatus(string status)
        {
            toolController.SetStatus(status);
        }

        internal void ShowResultReview(SimplePreprocessResultReview review)
        {
            toolController.ShowResultReview(review.Summary, review.IsSuccess, review.Items, review.Guidance);
        }

        public void ClearResultReview()
        {
            toolController?.ClearResultReview();
        }

        public void SetAddPipelineVisible(bool visible)
        {
            toolController.SetAddPipelineVisible(visible);
        }

        public void ClearParameters()
        {
            parameterController.Clear();
        }

        public SimplePreprocessToolSettings CaptureSettings()
        {
            return parameterController.CaptureSettings();
        }

        public void ApplyPersistedSettings(SimplePreprocessToolSettings settings)
        {
            bool previousSuppressEvents = suppressEvents;
            suppressEvents = true;
            try
            {
                parameterController.ApplySettings(settings);
            }
            finally
            {
                suppressEvents = previousSuppressEvents;
            }

            // Tool-specific configurators own summary/visibility rules, so replay their lightweight change path after restore.
            ParameterChanged(this, EventArgs.Empty);
        }

        public ComboBox AddChoice(string key, string label, IEnumerable<object> values, object selectedValue, string labelLocalizationKey = null)
        {
            return parameterController.AddChoice(key, label, values, selectedValue, labelLocalizationKey);
        }

        public TextBox AddNumber(string key, string label, double value, double minimum, double maximum, bool allowDecimal, bool allowNegative, string labelLocalizationKey = null)
        {
            return parameterController.AddNumber(key, label, value, minimum, maximum, allowDecimal, allowNegative, labelLocalizationKey);
        }

        public void AddSlider(string key, string label, double minimum, double maximum, double value, double tickFrequency, string labelLocalizationKey = null)
        {
            parameterController.AddSlider(key, label, minimum, maximum, value, tickFrequency, labelLocalizationKey);
        }

        public void AddRangeSliderPair(
            string groupKey,
            string groupLabel,
            string minKey,
            string minLabel,
            string maxKey,
            string maxLabel,
            double minimum,
            double maximum,
            double minValue,
            double maxValue,
            double tickFrequency,
            string groupLocalizationKey = null,
            string minLabelLocalizationKey = null,
            string maxLabelLocalizationKey = null)
        {
            parameterController.AddRangeSliderPair(
                groupKey,
                groupLabel,
                minKey,
                minLabel,
                maxKey,
                maxLabel,
                minimum,
                maximum,
                minValue,
                maxValue,
                tickFrequency,
                groupLocalizationKey,
                minLabelLocalizationKey,
                maxLabelLocalizationKey);
        }

        public CheckBox AddCheck(string key, string label, bool isChecked, string labelLocalizationKey = null)
        {
            return parameterController.AddCheck(key, label, isChecked, labelLocalizationKey);
        }

        public void SetParameterVisible(string key, bool visible)
        {
            parameterController.SetParameterVisible(key, visible);
        }

        public void SetParametersVisible(IEnumerable<string> keys, bool visible)
        {
            parameterController.SetParametersVisible(keys, visible);
        }

        public T GetEnum<T>(string key, T fallback)
            where T : struct
        {
            return parameterController.GetEnum(key, fallback);
        }

        public string GetChoiceText(string key, string fallback)
        {
            return parameterController.GetChoiceText(key, fallback);
        }

        public int GetInt(string key, int fallback)
        {
            return parameterController.GetInt(key, fallback);
        }

        public double GetDouble(string key, double fallback)
        {
            return parameterController.GetDouble(key, fallback);
        }

        public bool GetBool(string key, bool fallback)
        {
            return parameterController.GetBool(key, fallback);
        }

        private void RefreshSummaryAndClearResultReview()
        {
            textPresenter.RefreshSummary();
            ClearResultReview();
        }
    }
}
