using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class VisionToolKernelSizeController
    {
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly TextBox widthTextBox;
        private readonly CheckBox lockSizeCheckBox;
        private readonly Action<int> applyPreset;
        private readonly Action syncLockedHeightToWidth;
        private readonly Action<bool> setSuppressed;
        private readonly IReadOnlyList<TextBox> parameterTextBoxes;
        private readonly IReadOnlyList<Button> presetButtons;

        public VisionToolKernelSizeController(
            VisionToolParameterChangeController parameterChangeController,
            TextBox widthTextBox,
            CheckBox lockSizeCheckBox,
            Action<int> applyPreset,
            Action syncLockedHeightToWidth,
            Action<bool> setSuppressed,
            IReadOnlyList<TextBox> parameterTextBoxes = null,
            IReadOnlyList<Button> presetButtons = null)
        {
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.widthTextBox = widthTextBox ?? throw new ArgumentNullException(nameof(widthTextBox));
            this.lockSizeCheckBox = lockSizeCheckBox ?? throw new ArgumentNullException(nameof(lockSizeCheckBox));
            this.applyPreset = applyPreset ?? throw new ArgumentNullException(nameof(applyPreset));
            this.syncLockedHeightToWidth = syncLockedHeightToWidth ?? throw new ArgumentNullException(nameof(syncLockedHeightToWidth));
            this.setSuppressed = setSuppressed ?? throw new ArgumentNullException(nameof(setSuppressed));
            this.parameterTextBoxes = parameterTextBoxes ?? Array.Empty<TextBox>();
            this.presetButtons = presetButtons ?? Array.Empty<Button>();
            AttachControls();
        }

        public void Detach()
        {
            foreach (TextBox textBox in parameterTextBoxes)
            {
                if (textBox != null)
                {
                    textBox.TextChanged -= ParameterTextBox_TextChanged;
                }
            }

            lockSizeCheckBox.Checked -= LockSizeCheckBox_Changed;
            lockSizeCheckBox.Unchecked -= LockSizeCheckBox_Changed;

            foreach (Button button in presetButtons)
            {
                if (button != null)
                {
                    button.Click -= PresetButton_Click;
                }
            }
        }

        public void HandleTextChanged(object sender)
        {
            parameterChangeController.TryHandle(() =>
            {
                VisionToolControlBinding.UpdateTextSource(sender as TextBox);
                if (ReferenceEquals(sender, widthTextBox) && IsLocked)
                {
                    syncLockedHeightToWidth();
                }
            }, schedulePreview: true);
        }

        public void HandleLockChanged()
        {
            parameterChangeController.TryHandle(() =>
            {
                if (IsLocked)
                {
                    VisionToolControlBinding.UpdateTextSource(widthTextBox);
                    syncLockedHeightToWidth();
                }
            }, schedulePreview: true);
        }

        public void HandlePresetClick(object sender)
        {
            if (sender is not Button { Tag: string tag } ||
                !int.TryParse(tag, NumberStyles.Integer, CultureInfo.InvariantCulture, out int size))
            {
                return;
            }

            ApplyPreset(size);
        }

        public void ApplyPreset(int size)
        {
            // Preset changes update several bound properties; suppress event echo and settle once through the shared change controller.
            setSuppressed(true);
            try
            {
                applyPreset(size);
            }
            finally
            {
                setSuppressed(false);
            }

            parameterChangeController.RefreshProgrammatic(schedulePreview: true);
        }

        public void FlushParameterBindings()
        {
            foreach (TextBox textBox in parameterTextBoxes)
            {
                VisionToolControlBinding.UpdateTextSource(textBox);
            }
        }

        private bool IsLocked => lockSizeCheckBox.IsChecked == true;

        private void AttachControls()
        {
            foreach (TextBox textBox in parameterTextBoxes)
            {
                if (textBox != null)
                {
                    textBox.TextChanged += ParameterTextBox_TextChanged;
                }
            }

            lockSizeCheckBox.Checked += LockSizeCheckBox_Changed;
            lockSizeCheckBox.Unchecked += LockSizeCheckBox_Changed;

            foreach (Button button in presetButtons)
            {
                if (button != null)
                {
                    button.Click += PresetButton_Click;
                }
            }
        }

        private void ParameterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextChanged(sender);
        }

        private void LockSizeCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            HandleLockChanged();
        }

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            HandlePresetClick(sender);
        }
    }
}
