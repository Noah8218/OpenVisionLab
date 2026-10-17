using OpenVisionLab.Composition;
using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal static class VisionToolLayerSelectionPresenter
    {
        private const string WorkbenchInputChoicesKey = "VisionTool.WorkbenchInputChoices";

        internal static void SetWorkbenchInputChoices(ComboBox comboBox, IReadOnlyList<KeyValuePair<string, string>> choices)
        {
            if (comboBox != null) { comboBox.Resources[WorkbenchInputChoicesKey] = choices; }
        }

        public static void ApplySingle(
            ComboBox inputComboBox,
            ComboBox outputComboBox,
            IEnumerable<string> layerNames,
            string selectedInputLayer,
            string selectedOutputLayer)
        {
            IVisionToolLayerSelectionViewModel layerSelection = VisionToolCompositionService.CreateSingleLayerSelection(
                layerNames,
                selectedInputLayer,
                selectedOutputLayer);

            ConfigureExistingLayerCombo(inputComboBox);
            ConfigureOutputLayerCombo(outputComboBox);
            ApplyInputLayerSelection(inputComboBox, layerSelection.InputLayers, HasWorkbenchInputChoices(inputComboBox) ? selectedInputLayer : layerSelection.SelectedInputLayer);
            ApplyOutputLayerSelection(outputComboBox, layerSelection.OutputLayers, layerSelection.SelectedOutputLayer);
        }

        public static void ApplyDual(
            ComboBox inputAComboBox,
            ComboBox inputBComboBox,
            ComboBox outputComboBox,
            IEnumerable<string> layerNames,
            string selectedInputA,
            string selectedInputB,
            string selectedOutputLayer)
        {
            IVisionToolLayerSelectionViewModel layerSelection = VisionToolCompositionService.CreateDualLayerSelection(
                layerNames,
                selectedInputA,
                selectedInputB,
                selectedOutputLayer);

            ConfigureExistingLayerCombo(inputAComboBox);
            ConfigureExistingLayerCombo(inputBComboBox);
            ConfigureOutputLayerCombo(outputComboBox);
            ApplyInputLayerSelection(inputAComboBox, layerSelection.InputLayers, HasWorkbenchInputChoices(inputAComboBox) ? selectedInputA : layerSelection.SelectedInputLayer);
            ApplyInputLayerSelection(inputBComboBox, layerSelection.InputLayers, HasWorkbenchInputChoices(inputBComboBox) ? selectedInputB : layerSelection.SelectedInputLayerB);
            ApplyOutputLayerSelection(outputComboBox, layerSelection.OutputLayers, layerSelection.SelectedOutputLayer);
        }

        private static void ApplyInputLayerSelection(
            ComboBox comboBox,
            IEnumerable<string> layerNames,
            string selectedLayer)
        {
            if (comboBox == null)
            {
                return;
            }

            if (comboBox.Resources[WorkbenchInputChoicesKey] is IReadOnlyList<KeyValuePair<string, string>> choices)
            {
                comboBox.ItemsSource = choices;
                comboBox.SetResourceReference(ItemsControl.ItemTemplateProperty, "VisionTool.WorkbenchInputLayerTemplate");
                TextSearch.SetTextPath(comboBox, "Value");
            }
            else
            {
                // Independent tool inputs retain the existing workspace-only contract.
                comboBox.ItemsSource = layerNames?.ToList() ?? new List<string>();
            }
            VisionToolLayerComboHelper.SelectLayerText(comboBox, selectedLayer, allowFreeText: false);
        }

        private static bool HasWorkbenchInputChoices(ComboBox comboBox) => comboBox?.Resources[WorkbenchInputChoicesKey] is IReadOnlyList<KeyValuePair<string, string>>;

        private static void ApplyOutputLayerSelection(
            ComboBox comboBox,
            IEnumerable<string> layerNames,
            string selectedLayer)
        {
            if (comboBox == null)
            {
                return;
            }

            // Output selectors can target an existing layer or keep a pending new layer name.
            comboBox.ItemsSource = layerNames?.ToList() ?? new List<string>();
            VisionToolLayerComboHelper.SelectLayerText(comboBox, selectedLayer, allowFreeText: true);
        }

        private static void ConfigureExistingLayerCombo(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.IsEditable = false;
            comboBox.IsTextSearchEnabled = true;
            ComboBoxInteractionBehavior.SetOpenOnFieldClick(comboBox, true);
        }

        private static void ConfigureOutputLayerCombo(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.IsEditable = true;
            comboBox.IsReadOnly = false;
            comboBox.IsTextSearchEnabled = true;
            comboBox.StaysOpenOnEdit = true;
            ComboBoxInteractionBehavior.SetOpenOnFieldClick(comboBox, false);
        }
    }
}
