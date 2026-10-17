using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OpenVisionLab
{
    internal sealed class VisionToolLayerSelectionBehavior : IDisposable
    {
        private readonly ComboBox inputAComboBox;
        private readonly ComboBox inputBComboBox;
        private readonly ComboBox outputComboBox;
        private readonly Action inputAChanged;
        private readonly Action inputBChanged;
        private readonly Action outputChanged;
        private string lastNotifiedOutputLayer = string.Empty;
        private bool isApplyingLayerList;
        private bool disposed;

        private VisionToolLayerSelectionBehavior(
            ComboBox inputAComboBox,
            ComboBox inputBComboBox,
            ComboBox outputComboBox,
            Action inputAChanged,
            Action inputBChanged,
            Action outputChanged)
        {
            this.inputAComboBox = inputAComboBox;
            this.inputBComboBox = inputBComboBox;
            this.outputComboBox = outputComboBox;
            this.inputAChanged = inputAChanged;
            this.inputBChanged = inputBChanged;
            this.outputChanged = outputChanged;

            Attach(inputAComboBox);
            Attach(inputBComboBox);
            Attach(outputComboBox);
            outputComboBox?.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(OutputComboBox_TextChanged));
        }

        public string SelectedInputLayer => VisionToolLayerComboHelper.GetLayerText(inputAComboBox);
        public string SelectedInputLayerB => VisionToolLayerComboHelper.GetLayerText(inputBComboBox);
        public string SelectedOutputLayer => VisionToolLayerComboHelper.GetLayerText(outputComboBox);

        public static VisionToolLayerSelectionBehavior AttachSingle(
            ComboBox inputComboBox,
            ComboBox outputComboBox,
            Action inputChanged,
            Action outputChanged)
        {
            return new VisionToolLayerSelectionBehavior(
                inputComboBox,
                null,
                outputComboBox,
                inputChanged,
                null,
                outputChanged);
        }

        public static VisionToolLayerSelectionBehavior AttachDual(
            ComboBox inputAComboBox,
            ComboBox inputBComboBox,
            ComboBox outputComboBox,
            Action inputAChanged,
            Action inputBChanged,
            Action outputChanged)
        {
            return new VisionToolLayerSelectionBehavior(
                inputAComboBox,
                inputBComboBox,
                outputComboBox,
                inputAChanged,
                inputBChanged,
                outputChanged);
        }

        public void ApplySingle(
            IEnumerable<string> layerNames,
            string selectedInputLayer,
            string selectedOutputLayer)
        {
            isApplyingLayerList = true;
            try
            {
                VisionToolLayerSelectionPresenter.ApplySingle(
                    inputAComboBox,
                    outputComboBox,
                    layerNames,
                    selectedInputLayer,
                    selectedOutputLayer);
            }
            finally
            {
                isApplyingLayerList = false;
                lastNotifiedOutputLayer = SelectedOutputLayer;
            }
        }

        public void ApplyDual(
            IEnumerable<string> layerNames,
            string selectedInputLayerA,
            string selectedInputLayerB,
            string selectedOutputLayer)
        {
            isApplyingLayerList = true;
            try
            {
                VisionToolLayerSelectionPresenter.ApplyDual(
                    inputAComboBox,
                    inputBComboBox,
                    outputComboBox,
                    layerNames,
                    selectedInputLayerA,
                    selectedInputLayerB,
                    selectedOutputLayer);
            }
            finally
            {
                isApplyingLayerList = false;
                lastNotifiedOutputLayer = SelectedOutputLayer;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Detach(inputAComboBox);
            Detach(inputBComboBox);
            Detach(outputComboBox);
            outputComboBox?.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(OutputComboBox_TextChanged));
        }

        private void Attach(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.SelectionChanged -= ComboBox_SelectionChanged;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void Detach(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.SelectionChanged -= ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isApplyingLayerList)
            {
                return;
            }

            if (ReferenceEquals(sender, inputAComboBox))
            {
                inputAChanged?.Invoke();
            }
            else if (ReferenceEquals(sender, inputBComboBox))
            {
                inputBChanged?.Invoke();
            }
            else if (ReferenceEquals(sender, outputComboBox))
            {
                NotifyOutputChanged();
            }
        }

        private void OutputComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isApplyingLayerList && outputComboBox.IsEditable)
            {
                NotifyOutputChanged();
            }
        }

        private void NotifyOutputChanged()
        {
            string selectedOutputLayer = SelectedOutputLayer;
            if (string.Equals(lastNotifiedOutputLayer, selectedOutputLayer, StringComparison.Ordinal))
            {
                return;
            }

            lastNotifiedOutputLayer = selectedOutputLayer;
            outputChanged?.Invoke();
        }
    }
}
