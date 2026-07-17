using System;
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
                .FirstOrDefault(item => string.Equals(Convert.ToString(item), text, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                comboBox.SelectedItem = match;
                return;
            }

            comboBox.SelectedItem = null;
            if (allowFreeText)
            {
                comboBox.Text = text ?? string.Empty;
            }
            else if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
            else
            {
                comboBox.Text = text ?? string.Empty;
            }
        }
    }
}