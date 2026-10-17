using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal static class VisionToolLayerComboHelper
    {
        public static string GetLayerText(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                return string.Empty;
            }

            if (comboBox.SelectedItem is KeyValuePair<string, string> choice) { return choice.Key; }
            string selectedText = Convert.ToString(comboBox.SelectedItem);
            return !string.IsNullOrWhiteSpace(selectedText)
                ? selectedText
                : comboBox.Text ?? string.Empty;
        }

        public static void SelectLayerText(ComboBox comboBox, string text, bool allowFreeText)
        {
            if (comboBox == null)
            {
                return;
            }

            object match = comboBox.Items.Cast<object>()
                .FirstOrDefault(item => string.Equals(GetItemLayerText(item), text, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                comboBox.SelectedItem = match;
                if (allowFreeText) { comboBox.Text = GetItemLayerText(match); }
                return;
            }

            comboBox.SelectedItem = null;
            if (allowFreeText)
            {
                comboBox.Text = text ?? string.Empty;
            }
            else if (comboBox.Items.Count > 0 && !(comboBox.Items[0] is KeyValuePair<string, string>))
            {
                comboBox.SelectedIndex = 0;
            }
            else
            {
                comboBox.Text = text ?? string.Empty;
            }
        }

        internal static string GetItemLayerText(object item) => item is KeyValuePair<string, string> choice ? choice.Key : Convert.ToString(item);
    }
}
