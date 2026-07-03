using System;
using System.Globalization;
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

        public VisionToolKernelSizeController(
            VisionToolParameterChangeController parameterChangeController,
            TextBox widthTextBox,
            CheckBox lockSizeCheckBox,
            Action<int> applyPreset,
            Action syncLockedHeightToWidth,
            Action<bool> setSuppressed)
        {
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.widthTextBox = widthTextBox ?? throw new ArgumentNullException(nameof(widthTextBox));
            this.lockSizeCheckBox = lockSizeCheckBox ?? throw new ArgumentNullException(nameof(lockSizeCheckBox));
            this.applyPreset = applyPreset ?? throw new ArgumentNullException(nameof(applyPreset));
            this.syncLockedHeightToWidth = syncLockedHeightToWidth ?? throw new ArgumentNullException(nameof(syncLockedHeightToWidth));
            this.setSuppressed = setSuppressed ?? throw new ArgumentNullException(nameof(setSuppressed));
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

        private bool IsLocked => lockSizeCheckBox.IsChecked == true;
    }
}
