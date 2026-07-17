using System;
using System.Globalization;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal readonly struct ArithmeticToolTextState
    {
        public static readonly ArithmeticToolTextState Empty = new ArithmeticToolTextState(
            string.Empty,
            requiresInputB: false,
            useConstantInput: false,
            useColorConstant: false,
            useOffsetMode: false,
            grayValue: 1,
            blueValue: 1,
            greenValue: 1,
            redValue: 1,
            offsetX: 1,
            offsetY: 1);

        public ArithmeticToolTextState(
            string selectedArithmeticType,
            bool requiresInputB,
            bool useConstantInput,
            bool useColorConstant,
            bool useOffsetMode,
            int grayValue,
            int blueValue,
            int greenValue,
            int redValue,
            int offsetX,
            int offsetY)
        {
            SelectedArithmeticType = selectedArithmeticType ?? string.Empty;
            RequiresInputB = requiresInputB;
            UseConstantInput = useConstantInput;
            UseColorConstant = useColorConstant;
            UseOffsetMode = useOffsetMode;
            GrayValue = grayValue;
            BlueValue = blueValue;
            GreenValue = greenValue;
            RedValue = redValue;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public string SelectedArithmeticType { get; }
        public bool RequiresInputB { get; }
        public bool UseConstantInput { get; }
        public bool UseColorConstant { get; }
        public bool UseOffsetMode { get; }
        public int GrayValue { get; }
        public int BlueValue { get; }
        public int GreenValue { get; }
        public int RedValue { get; }
        public int OffsetX { get; }
        public int OffsetY { get; }
    }

    internal sealed class ArithmeticToolTextPresenter
    {
        private readonly Func<ArithmeticToolTextState> getState;
        private readonly GroupBox operationGroup;
        private readonly TextBlock arithmeticModeLabel;
        private readonly RadioButton operationModeRadioButton;
        private readonly RadioButton offsetModeRadioButton;
        private readonly TextBlock arithmeticTypeLabel;
        private readonly GroupBox inputBSourceGroup;
        private readonly RadioButton sourceImageRadioButton;
        private readonly RadioButton constantInputRadioButton;
        private readonly RadioButton grayRadioButton;
        private readonly RadioButton colorRadioButton;
        private readonly TextBlock constantGrayLabel;
        private readonly GroupBox constantGroup;
        private readonly GroupBox offsetGroup;
        private readonly TextBlock copyOffsetTextBlock;
        private readonly Action<string> setRunOffsetText;
        private readonly Action<string> setSummaryText;

        public ArithmeticToolTextPresenter(
            Func<ArithmeticToolTextState> getState,
            GroupBox operationGroup,
            TextBlock arithmeticModeLabel,
            RadioButton operationModeRadioButton,
            RadioButton offsetModeRadioButton,
            TextBlock arithmeticTypeLabel,
            GroupBox inputBSourceGroup,
            RadioButton sourceImageRadioButton,
            RadioButton constantInputRadioButton,
            RadioButton grayRadioButton,
            RadioButton colorRadioButton,
            TextBlock constantGrayLabel,
            GroupBox constantGroup,
            GroupBox offsetGroup,
            TextBlock copyOffsetTextBlock,
            Action<string> setRunOffsetText,
            Action<string> setSummaryText)
        {
            this.getState = getState ?? throw new ArgumentNullException(nameof(getState));
            this.operationGroup = operationGroup ?? throw new ArgumentNullException(nameof(operationGroup));
            this.arithmeticModeLabel = arithmeticModeLabel ?? throw new ArgumentNullException(nameof(arithmeticModeLabel));
            this.operationModeRadioButton = operationModeRadioButton ?? throw new ArgumentNullException(nameof(operationModeRadioButton));
            this.offsetModeRadioButton = offsetModeRadioButton ?? throw new ArgumentNullException(nameof(offsetModeRadioButton));
            this.arithmeticTypeLabel = arithmeticTypeLabel ?? throw new ArgumentNullException(nameof(arithmeticTypeLabel));
            this.inputBSourceGroup = inputBSourceGroup ?? throw new ArgumentNullException(nameof(inputBSourceGroup));
            this.sourceImageRadioButton = sourceImageRadioButton ?? throw new ArgumentNullException(nameof(sourceImageRadioButton));
            this.constantInputRadioButton = constantInputRadioButton ?? throw new ArgumentNullException(nameof(constantInputRadioButton));
            this.grayRadioButton = grayRadioButton ?? throw new ArgumentNullException(nameof(grayRadioButton));
            this.colorRadioButton = colorRadioButton ?? throw new ArgumentNullException(nameof(colorRadioButton));
            this.constantGrayLabel = constantGrayLabel ?? throw new ArgumentNullException(nameof(constantGrayLabel));
            this.constantGroup = constantGroup ?? throw new ArgumentNullException(nameof(constantGroup));
            this.offsetGroup = offsetGroup ?? throw new ArgumentNullException(nameof(offsetGroup));
            this.copyOffsetTextBlock = copyOffsetTextBlock ?? throw new ArgumentNullException(nameof(copyOffsetTextBlock));
            this.setRunOffsetText = setRunOffsetText ?? throw new ArgumentNullException(nameof(setRunOffsetText));
            this.setSummaryText = setSummaryText ?? throw new ArgumentNullException(nameof(setSummaryText));
        }

        public void ApplyLocalization()
        {
            operationGroup.Header = ResolveText("Arithmetic.Operation", "Operation");
            arithmeticModeLabel.Text = ResolveText("Arithmetic.Mode", "Mode");
            operationModeRadioButton.Content = ResolveText("Arithmetic.OperationMode", "Operation");
            offsetModeRadioButton.Content = ResolveText("Arithmetic.OffsetMode", "Offset");
            arithmeticTypeLabel.Text = ResolveText("Arithmetic.Type", "Arithmetic Type");
            inputBSourceGroup.Header = ResolveText("Arithmetic.InputBSource", "Input B Source");
            sourceImageRadioButton.Content = ResolveText("Arithmetic.InputB", "Input B");
            constantInputRadioButton.Content = ResolveText("Arithmetic.Constant", "Constant");
            grayRadioButton.Content = ResolveText("Arithmetic.Gray", "Gray");
            colorRadioButton.Content = ResolveText("Arithmetic.Color", "Color");
            constantGrayLabel.Text = ResolveText("Arithmetic.Gray", "Gray");
            constantGroup.Header = ResolveText("Arithmetic.ConstantValue", "Constant Value");
            offsetGroup.Header = ResolveText("Arithmetic.Offset", "Offset");
            copyOffsetTextBlock.Text = ResolveText("Arithmetic.CopyAByOffset", "Copy A by offset");
            setRunOffsetText(ResolveText("Arithmetic.RunOffset", "Run Offset"));
            RefreshSummary();
        }

        public void RefreshSummary()
        {
            // Arithmetic summary text is derived from mode and input state, so formatting belongs in the presenter layer.
            ArithmeticToolTextState state = getState();
            if (state.UseOffsetMode)
            {
                setSummaryText(string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} / X {1} / Y {2}",
                    ResolveText("Arithmetic.OffsetMode", "Offset"),
                    state.OffsetX,
                    state.OffsetY));
                return;
            }

            string sourceText = !state.RequiresInputB
                ? ResolveText("Arithmetic.InputAOnly", "Input A only")
                : state.UseConstantInput
                    ? state.UseColorConstant
                        ? ResolveFormat("Arithmetic.ColorBgrFormat", "Color BGR({0},{1},{2})", state.BlueValue, state.GreenValue, state.RedValue)
                        : ResolveFormat("Arithmetic.GrayFormat", "Gray {0}", state.GrayValue)
                    : ResolveText("Arithmetic.InputB", "Input B");
            setSummaryText(string.Format(CultureInfo.CurrentCulture, "{0} / {1}", state.SelectedArithmeticType, sourceText));
        }

        private static string ResolveFormat(string localizationKey, string fallbackText, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, ResolveText(localizationKey, fallbackText), args);
        }

        private static string ResolveText(string localizationKey, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(localizationKey);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, localizationKey, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }
    }
}