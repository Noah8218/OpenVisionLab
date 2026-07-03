using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal sealed class ArithmeticToolInteractionController
    {
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly Action<bool> setSuppressed;
        private readonly Action<bool> setInputBPreviewVisible;
        private readonly Action<bool> setOffsetActionsVisible;
        private readonly ComboBox arithmeticTypeComboBox;
        private readonly RadioButton operationModeRadioButton;
        private readonly RadioButton sourceImageRadioButton;
        private readonly RadioButton constantInputRadioButton;
        private readonly RadioButton colorConstantRadioButton;
        private readonly RadioButton offsetModeRadioButton;
        private readonly StackPanel constantModePanel;
        private readonly GroupBox constantGroup;
        private readonly StackPanel arithmeticTypePanel;
        private readonly GroupBox inputBSourceGroup;
        private readonly GroupBox offsetGroup;
        private readonly RowDefinition inputBSourceRow;
        private readonly RowDefinition inputBSourceGapRow;
        private readonly RowDefinition constantRow;
        private readonly RowDefinition offsetRow;
        private readonly RowDefinition offsetGapRow;
        private readonly TextBox grayTextBox;
        private readonly TextBox blueTextBox;
        private readonly TextBox greenTextBox;
        private readonly TextBox redTextBox;
        private readonly TextBox offsetXTextBox;
        private readonly TextBox offsetYTextBox;

        public ArithmeticToolInteractionController(
            VisionToolParameterChangeController parameterChangeController,
            Action<bool> setSuppressed,
            Action<bool> setInputBPreviewVisible,
            Action<bool> setOffsetActionsVisible,
            ComboBox arithmeticTypeComboBox,
            RadioButton operationModeRadioButton,
            RadioButton sourceImageRadioButton,
            RadioButton constantInputRadioButton,
            RadioButton colorConstantRadioButton,
            RadioButton offsetModeRadioButton,
            StackPanel constantModePanel,
            GroupBox constantGroup,
            StackPanel arithmeticTypePanel,
            GroupBox inputBSourceGroup,
            GroupBox offsetGroup,
            RowDefinition inputBSourceRow,
            RowDefinition inputBSourceGapRow,
            RowDefinition constantRow,
            RowDefinition offsetRow,
            RowDefinition offsetGapRow,
            TextBox grayTextBox,
            TextBox blueTextBox,
            TextBox greenTextBox,
            TextBox redTextBox,
            TextBox offsetXTextBox,
            TextBox offsetYTextBox)
        {
            this.parameterChangeController = parameterChangeController ?? throw new ArgumentNullException(nameof(parameterChangeController));
            this.setSuppressed = setSuppressed ?? throw new ArgumentNullException(nameof(setSuppressed));
            this.setInputBPreviewVisible = setInputBPreviewVisible ?? throw new ArgumentNullException(nameof(setInputBPreviewVisible));
            this.setOffsetActionsVisible = setOffsetActionsVisible ?? throw new ArgumentNullException(nameof(setOffsetActionsVisible));
            this.arithmeticTypeComboBox = arithmeticTypeComboBox ?? throw new ArgumentNullException(nameof(arithmeticTypeComboBox));
            this.operationModeRadioButton = operationModeRadioButton ?? throw new ArgumentNullException(nameof(operationModeRadioButton));
            this.sourceImageRadioButton = sourceImageRadioButton ?? throw new ArgumentNullException(nameof(sourceImageRadioButton));
            this.constantInputRadioButton = constantInputRadioButton ?? throw new ArgumentNullException(nameof(constantInputRadioButton));
            this.colorConstantRadioButton = colorConstantRadioButton ?? throw new ArgumentNullException(nameof(colorConstantRadioButton));
            this.offsetModeRadioButton = offsetModeRadioButton ?? throw new ArgumentNullException(nameof(offsetModeRadioButton));
            this.constantModePanel = constantModePanel ?? throw new ArgumentNullException(nameof(constantModePanel));
            this.constantGroup = constantGroup ?? throw new ArgumentNullException(nameof(constantGroup));
            this.arithmeticTypePanel = arithmeticTypePanel ?? throw new ArgumentNullException(nameof(arithmeticTypePanel));
            this.inputBSourceGroup = inputBSourceGroup ?? throw new ArgumentNullException(nameof(inputBSourceGroup));
            this.offsetGroup = offsetGroup ?? throw new ArgumentNullException(nameof(offsetGroup));
            this.inputBSourceRow = inputBSourceRow ?? throw new ArgumentNullException(nameof(inputBSourceRow));
            this.inputBSourceGapRow = inputBSourceGapRow ?? throw new ArgumentNullException(nameof(inputBSourceGapRow));
            this.constantRow = constantRow ?? throw new ArgumentNullException(nameof(constantRow));
            this.offsetRow = offsetRow ?? throw new ArgumentNullException(nameof(offsetRow));
            this.offsetGapRow = offsetGapRow ?? throw new ArgumentNullException(nameof(offsetGapRow));
            this.grayTextBox = grayTextBox ?? throw new ArgumentNullException(nameof(grayTextBox));
            this.blueTextBox = blueTextBox ?? throw new ArgumentNullException(nameof(blueTextBox));
            this.greenTextBox = greenTextBox ?? throw new ArgumentNullException(nameof(greenTextBox));
            this.redTextBox = redTextBox ?? throw new ArgumentNullException(nameof(redTextBox));
            this.offsetXTextBox = offsetXTextBox ?? throw new ArgumentNullException(nameof(offsetXTextBox));
            this.offsetYTextBox = offsetYTextBox ?? throw new ArgumentNullException(nameof(offsetYTextBox));
        }

        public string SelectedArithmeticType => GetComboText(arithmeticTypeComboBox);

        public bool UseConstantInput => constantInputRadioButton.IsChecked == true;

        public bool UseColorConstant => colorConstantRadioButton.IsChecked == true;

        public bool UseOffsetMode => offsetModeRadioButton.IsChecked == true;

        public int GetGrayValue(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(grayTextBox, fallback);
        }

        public int GetBValue(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(blueTextBox, fallback);
        }

        public int GetGValue(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(greenTextBox, fallback);
        }

        public int GetRValue(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(redTextBox, fallback);
        }

        public int GetOffsetX(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(offsetXTextBox, fallback);
        }

        public int GetOffsetY(int fallback)
        {
            return VisionToolControlValueReader.ReadInvariantInt(offsetYTextBox, fallback);
        }

        public void SetOperationList(IEnumerable<string> operationNames, string selectedOperation)
        {
            List<string> operations = operationNames?.Where(item => !string.IsNullOrWhiteSpace(item)).ToList()
                ?? new List<string>();

            setSuppressed(true);
            try
            {
                arithmeticTypeComboBox.ItemsSource = operations;
                SelectComboText(arithmeticTypeComboBox, selectedOperation);
                if (arithmeticTypeComboBox.SelectedItem == null && operations.Count > 0)
                {
                    arithmeticTypeComboBox.SelectedIndex = 0;
                }
            }
            finally
            {
                setSuppressed(false);
            }

            parameterChangeController.RefreshProgrammatic(RefreshMode);
        }

        public ArithmeticToolSettings CaptureSettings()
        {
            return new ArithmeticToolSettings
            {
                SelectedOperation = SelectedArithmeticType,
                UseConstantInput = UseConstantInput,
                UseColorConstant = UseColorConstant,
                UseOffsetMode = UseOffsetMode,
                Gray = GetGrayValue(1),
                B = GetBValue(1),
                G = GetGValue(1),
                R = GetRValue(1),
                OffsetX = GetOffsetX(1),
                OffsetY = GetOffsetY(1)
            };
        }

        public void ApplySettings(ArithmeticToolSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            setSuppressed(true);
            try
            {
                SelectComboText(arithmeticTypeComboBox, settings.SelectedOperation);
                operationModeRadioButton.IsChecked = !settings.UseOffsetMode;
                offsetModeRadioButton.IsChecked = settings.UseOffsetMode;
                sourceImageRadioButton.IsChecked = !settings.UseConstantInput;
                constantInputRadioButton.IsChecked = settings.UseConstantInput;
                colorConstantRadioButton.IsChecked = settings.UseColorConstant;
                grayTextBox.Text = settings.Gray.ToString(CultureInfo.InvariantCulture);
                blueTextBox.Text = settings.B.ToString(CultureInfo.InvariantCulture);
                greenTextBox.Text = settings.G.ToString(CultureInfo.InvariantCulture);
                redTextBox.Text = settings.R.ToString(CultureInfo.InvariantCulture);
                offsetXTextBox.Text = settings.OffsetX.ToString(CultureInfo.InvariantCulture);
                offsetYTextBox.Text = settings.OffsetY.ToString(CultureInfo.InvariantCulture);
            }
            finally
            {
                setSuppressed(false);
            }

            parameterChangeController.RefreshProgrammatic(RefreshMode);
        }

        public void HandleArithmeticTypeChanged()
        {
            parameterChangeController.TryHandle(RefreshMode, notifyChanged: true, schedulePreview: true);
        }

        public void HandleModeChanged()
        {
            parameterChangeController.TryHandle(RefreshMode, notifyChanged: true, schedulePreview: true);
        }

        public void HandleParameterTextChanged()
        {
            parameterChangeController.TryHandle(notifyChanged: true, schedulePreview: true);
        }

        public void HandleNumberTextInput(TextCompositionEventArgs e)
        {
            VisionToolControlBinding.AllowUnsignedIntegerInput(e);
        }

        public void HandleSignedNumberTextInput(TextCompositionEventArgs e)
        {
            VisionToolControlBinding.AllowSignedIntegerInput(e);
        }

        public ArithmeticToolTextState CreateTextState()
        {
            string operationName = SelectedArithmeticType;
            return new ArithmeticToolTextState(
                operationName,
                RequiresInputB(operationName),
                UseConstantInput,
                UseColorConstant,
                UseOffsetMode,
                GetGrayValue(1),
                GetBValue(1),
                GetGValue(1),
                GetRValue(1),
                GetOffsetX(1),
                GetOffsetY(1));
        }

        public void RefreshMode()
        {
            bool useOffsetMode = UseOffsetMode;
            bool requiresInputB = RequiresInputB(SelectedArithmeticType);
            bool showSourcePanel = !useOffsetMode && requiresInputB;
            bool showConstantPanel = showSourcePanel && UseConstantInput;
            bool showInputBPreview = showSourcePanel && !UseConstantInput;

            arithmeticTypePanel.Visibility = useOffsetMode ? Visibility.Collapsed : Visibility.Visible;
            arithmeticTypeComboBox.IsEnabled = !useOffsetMode;

            inputBSourceGroup.Visibility = showSourcePanel ? Visibility.Visible : Visibility.Collapsed;
            inputBSourceGroup.IsEnabled = showSourcePanel;

            // Shell-owned preview/action chrome is updated through callbacks so Arithmetic keeps only parameter layout policy here.
            setInputBPreviewVisible(showInputBPreview);
            setOffsetActionsVisible(useOffsetMode);

            constantModePanel.IsEnabled = showConstantPanel;
            constantGroup.Visibility = showConstantPanel ? Visibility.Visible : Visibility.Collapsed;
            constantGroup.IsEnabled = showConstantPanel;

            offsetGroup.Visibility = useOffsetMode ? Visibility.Visible : Visibility.Collapsed;
            Grid.SetRow(offsetGroup, useOffsetMode ? 2 : 6);
            inputBSourceRow.Height = showSourcePanel || useOffsetMode ? GridLength.Auto : new GridLength(0);
            inputBSourceGapRow.Height = showConstantPanel ? new GridLength(12) : new GridLength(0);
            constantRow.Height = showConstantPanel ? GridLength.Auto : new GridLength(0);
            offsetRow.Height = new GridLength(0);
            offsetGapRow.Height = new GridLength(0);

            grayTextBox.IsEnabled = showConstantPanel && !UseColorConstant;
            blueTextBox.IsEnabled = showConstantPanel && UseColorConstant;
            greenTextBox.IsEnabled = showConstantPanel && UseColorConstant;
            redTextBox.IsEnabled = showConstantPanel && UseColorConstant;
        }

        private static bool RequiresInputB(string operationName)
        {
            return !string.Equals(operationName, "Bitwise_NOT", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(operationName, "ABS", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetComboText(ComboBox comboBox)
        {
            return Convert.ToString(comboBox.SelectedItem, CultureInfo.InvariantCulture) ?? comboBox.Text ?? string.Empty;
        }

        private static void SelectComboText(ComboBox comboBox, string text)
        {
            if (comboBox == null)
            {
                return;
            }

            object match = comboBox.Items.Cast<object>()
                .FirstOrDefault(item => string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), text, StringComparison.OrdinalIgnoreCase));
            comboBox.SelectedItem = match;
            if (match == null)
            {
                comboBox.Text = text ?? string.Empty;
            }
        }
    }
}
