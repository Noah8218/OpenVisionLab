using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class ThresholdToolTextPresenter
    {
        private readonly GroupBox parameterGroup;
        private readonly GroupBox modeGroup;
        private readonly TextBlock basicTitleTextBlock;
        private readonly TextBlock basicHintTextBlock;
        private readonly TextBlock rangeTitleTextBlock;
        private readonly TextBlock rangeHintTextBlock;
        private readonly TextBlock adaptiveTitleTextBlock;
        private readonly TextBlock adaptiveHintTextBlock;
        private readonly TextBlock basicTypeLabel;
        private readonly RadioButton basicBinaryRadioButton;
        private readonly RadioButton basicInvertRadioButton;
        private readonly TextBlock basicMaxValueLabel;
        private readonly TextBlock basicThresholdLabel;
        private readonly TextBlock rangeTitleLabel;
        private readonly TextBlock rangeMinLabel;
        private readonly TextBlock rangeMaxLabel;
        private readonly CheckBox rangeInvertCheckBox;
        private readonly TextBlock adaptiveMethodLabel;
        private readonly RadioButton adaptiveMeanRadioButton;
        private readonly RadioButton adaptiveGaussianRadioButton;
        private readonly TextBlock adaptiveTypeLabel;
        private readonly RadioButton adaptiveBinaryRadioButton;
        private readonly RadioButton adaptiveInvertRadioButton;
        private readonly TextBlock adaptiveMaxValueLabel;
        private readonly TextBlock adaptiveWeightLabel;
        private readonly TextBlock blockSizeLabel;

        public ThresholdToolTextPresenter(
            GroupBox parameterGroup,
            GroupBox modeGroup,
            TextBlock basicTitleTextBlock,
            TextBlock basicHintTextBlock,
            TextBlock rangeTitleTextBlock,
            TextBlock rangeHintTextBlock,
            TextBlock adaptiveTitleTextBlock,
            TextBlock adaptiveHintTextBlock,
            TextBlock basicTypeLabel,
            RadioButton basicBinaryRadioButton,
            RadioButton basicInvertRadioButton,
            TextBlock basicMaxValueLabel,
            TextBlock basicThresholdLabel,
            TextBlock rangeTitleLabel,
            TextBlock rangeMinLabel,
            TextBlock rangeMaxLabel,
            CheckBox rangeInvertCheckBox,
            TextBlock adaptiveMethodLabel,
            RadioButton adaptiveMeanRadioButton,
            RadioButton adaptiveGaussianRadioButton,
            TextBlock adaptiveTypeLabel,
            RadioButton adaptiveBinaryRadioButton,
            RadioButton adaptiveInvertRadioButton,
            TextBlock adaptiveMaxValueLabel,
            TextBlock adaptiveWeightLabel,
            TextBlock blockSizeLabel)
        {
            this.parameterGroup = parameterGroup ?? throw new ArgumentNullException(nameof(parameterGroup));
            this.modeGroup = modeGroup ?? throw new ArgumentNullException(nameof(modeGroup));
            this.basicTitleTextBlock = basicTitleTextBlock ?? throw new ArgumentNullException(nameof(basicTitleTextBlock));
            this.basicHintTextBlock = basicHintTextBlock ?? throw new ArgumentNullException(nameof(basicHintTextBlock));
            this.rangeTitleTextBlock = rangeTitleTextBlock ?? throw new ArgumentNullException(nameof(rangeTitleTextBlock));
            this.rangeHintTextBlock = rangeHintTextBlock ?? throw new ArgumentNullException(nameof(rangeHintTextBlock));
            this.adaptiveTitleTextBlock = adaptiveTitleTextBlock ?? throw new ArgumentNullException(nameof(adaptiveTitleTextBlock));
            this.adaptiveHintTextBlock = adaptiveHintTextBlock ?? throw new ArgumentNullException(nameof(adaptiveHintTextBlock));
            this.basicTypeLabel = basicTypeLabel ?? throw new ArgumentNullException(nameof(basicTypeLabel));
            this.basicBinaryRadioButton = basicBinaryRadioButton ?? throw new ArgumentNullException(nameof(basicBinaryRadioButton));
            this.basicInvertRadioButton = basicInvertRadioButton ?? throw new ArgumentNullException(nameof(basicInvertRadioButton));
            this.basicMaxValueLabel = basicMaxValueLabel ?? throw new ArgumentNullException(nameof(basicMaxValueLabel));
            this.basicThresholdLabel = basicThresholdLabel ?? throw new ArgumentNullException(nameof(basicThresholdLabel));
            this.rangeTitleLabel = rangeTitleLabel ?? throw new ArgumentNullException(nameof(rangeTitleLabel));
            this.rangeMinLabel = rangeMinLabel ?? throw new ArgumentNullException(nameof(rangeMinLabel));
            this.rangeMaxLabel = rangeMaxLabel ?? throw new ArgumentNullException(nameof(rangeMaxLabel));
            this.rangeInvertCheckBox = rangeInvertCheckBox ?? throw new ArgumentNullException(nameof(rangeInvertCheckBox));
            this.adaptiveMethodLabel = adaptiveMethodLabel ?? throw new ArgumentNullException(nameof(adaptiveMethodLabel));
            this.adaptiveMeanRadioButton = adaptiveMeanRadioButton ?? throw new ArgumentNullException(nameof(adaptiveMeanRadioButton));
            this.adaptiveGaussianRadioButton = adaptiveGaussianRadioButton ?? throw new ArgumentNullException(nameof(adaptiveGaussianRadioButton));
            this.adaptiveTypeLabel = adaptiveTypeLabel ?? throw new ArgumentNullException(nameof(adaptiveTypeLabel));
            this.adaptiveBinaryRadioButton = adaptiveBinaryRadioButton ?? throw new ArgumentNullException(nameof(adaptiveBinaryRadioButton));
            this.adaptiveInvertRadioButton = adaptiveInvertRadioButton ?? throw new ArgumentNullException(nameof(adaptiveInvertRadioButton));
            this.adaptiveMaxValueLabel = adaptiveMaxValueLabel ?? throw new ArgumentNullException(nameof(adaptiveMaxValueLabel));
            this.adaptiveWeightLabel = adaptiveWeightLabel ?? throw new ArgumentNullException(nameof(adaptiveWeightLabel));
            this.blockSizeLabel = blockSizeLabel ?? throw new ArgumentNullException(nameof(blockSizeLabel));
        }

        public void ApplyLocalization()
        {
            parameterGroup.Header = ResolveText("Pipeline.ResultRow.Parameters", "Parameters");
            modeGroup.Header = ResolveText("Threshold.Mode", "Mode");

            basicTitleTextBlock.Text = ResolveText("Threshold.ModeBasic", "Basic");
            basicHintTextBlock.Text = ResolveText("Threshold.BasicHint", "Single threshold value.");
            rangeTitleTextBlock.Text = ResolveText("Threshold.ModeRange", "Range");
            rangeHintTextBlock.Text = ResolveText("Threshold.RangeHint", "Threshold between min and max.");
            adaptiveTitleTextBlock.Text = ResolveText("Threshold.ModeAdaptive", "Adaptive");
            adaptiveHintTextBlock.Text = ResolveText("Threshold.AdaptiveHint", "Adaptive local threshold.");

            basicTypeLabel.Text = ResolveText("Threshold.ResultType", "Result type");
            basicBinaryRadioButton.Content = ResolveText("Threshold.Binary", "Binary");
            basicInvertRadioButton.Content = ResolveText("Threshold.BinaryInv", "Binary Inv");
            basicMaxValueLabel.Text = ResolveText("Threshold.MaxValue", "Max value");
            basicThresholdLabel.Text = ResolveText("PropertyGrid.Property.Threshold.DisplayName", "Threshold");
            rangeTitleLabel.Text = ResolveText("Threshold.RangeTitle", "Range");
            rangeMinLabel.Text = ResolveText("Threshold.RangeMin", "Min");
            rangeMaxLabel.Text = ResolveText("Threshold.RangeMax", "Max");
            rangeInvertCheckBox.Content = ResolveText("Threshold.Invert", "Invert");
            adaptiveMethodLabel.Text = ResolveText("Threshold.Method", "Method");
            adaptiveMeanRadioButton.Content = ResolveText("Threshold.MeanC", "Mean C");
            adaptiveGaussianRadioButton.Content = ResolveText("Threshold.GaussianC", "Gaussian C");
            adaptiveTypeLabel.Text = ResolveText("Threshold.ResultType", "Result type");
            adaptiveBinaryRadioButton.Content = ResolveText("Threshold.Binary", "Binary");
            adaptiveInvertRadioButton.Content = ResolveText("Threshold.BinaryInv", "Binary Inv");
            adaptiveMaxValueLabel.Text = ResolveText("Threshold.MaxValue", "Max value");
            adaptiveWeightLabel.Text = ResolveText("Threshold.Weight", "Weight");
            blockSizeLabel.Text = ResolveText("Threshold.BlockSize", "Block size");
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
