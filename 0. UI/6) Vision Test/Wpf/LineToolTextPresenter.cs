using System;
using System.Windows.Controls;
using OpenVisionLab.Contracts;

namespace OpenVisionLab
{
    internal sealed class LineToolTextPresenter
    {
        private readonly LineToolPresenter presenter;
        private readonly TextBlock purposeLabel;
        private readonly TextBlock lineSelectorLabel;
        private readonly RadioButton purposeEdgeRadioButton;
        private readonly RadioButton purposeMeasureRadioButton;
        private readonly RadioButton purposeIntersectionRadioButton;
        private readonly RadioButton lineARadioButton;
        private readonly RadioButton lineBRadioButton;
        private readonly TextBlock purposeHintTextBlock;
        private readonly Button editSelectedRoiButton;
        private readonly Action<string> setSummaryText;

        public LineToolTextPresenter(
            LineToolPresenter presenter,
            TextBlock purposeLabel,
            TextBlock lineSelectorLabel,
            RadioButton purposeEdgeRadioButton,
            RadioButton purposeMeasureRadioButton,
            RadioButton purposeIntersectionRadioButton,
            RadioButton lineARadioButton,
            RadioButton lineBRadioButton,
            TextBlock purposeHintTextBlock,
            Button editSelectedRoiButton,
            Action<string> setSummaryText)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.purposeLabel = purposeLabel ?? throw new ArgumentNullException(nameof(purposeLabel));
            this.lineSelectorLabel = lineSelectorLabel ?? throw new ArgumentNullException(nameof(lineSelectorLabel));
            this.purposeEdgeRadioButton = purposeEdgeRadioButton ?? throw new ArgumentNullException(nameof(purposeEdgeRadioButton));
            this.purposeMeasureRadioButton = purposeMeasureRadioButton ?? throw new ArgumentNullException(nameof(purposeMeasureRadioButton));
            this.purposeIntersectionRadioButton = purposeIntersectionRadioButton ?? throw new ArgumentNullException(nameof(purposeIntersectionRadioButton));
            this.lineARadioButton = lineARadioButton ?? throw new ArgumentNullException(nameof(lineARadioButton));
            this.lineBRadioButton = lineBRadioButton ?? throw new ArgumentNullException(nameof(lineBRadioButton));
            this.purposeHintTextBlock = purposeHintTextBlock ?? throw new ArgumentNullException(nameof(purposeHintTextBlock));
            this.editSelectedRoiButton = editSelectedRoiButton ?? throw new ArgumentNullException(nameof(editSelectedRoiButton));
            this.setSummaryText = setSummaryText ?? throw new ArgumentNullException(nameof(setSummaryText));
        }

        public void ApplyLocalization(LineToolPurpose purpose)
        {
            purposeLabel.Text = VisionToolVerificationText.LinePurposeLabel;
            lineSelectorLabel.Text = VisionToolVerificationText.LineSettingLabel;
            purposeEdgeRadioButton.Content = VisionToolVerificationText.LinePurposeEdge;
            purposeMeasureRadioButton.Content = VisionToolVerificationText.LinePurposeMeasure;
            purposeIntersectionRadioButton.Content = VisionToolVerificationText.LinePurposeIntersection;
            lineARadioButton.Content = VisionToolVerificationText.LineA;
            lineBRadioButton.Content = VisionToolVerificationText.LineB;
            RefreshPurposeHint(purpose);
            VisionToolChromePresenter.ApplyTooltip(editSelectedRoiButton, VisionToolVerificationText.EditSelectedLineRoiTooltip);
        }

        public void RefreshSummary(LineToolPurpose purpose, bool isLineBSelected, string selectedLineName)
        {
            string purposeText = VisionToolVerificationText.CreateLinePurposeText(purpose.ToString());
            setSummaryText(presenter.CreateSummary(purpose, isLineBSelected, purposeText, selectedLineName));
            RefreshPurposeHint(purpose);
        }

        private void RefreshPurposeHint(LineToolPurpose purpose)
        {
            purposeHintTextBlock.Text = VisionToolVerificationText.CreateLinePurposeHint(purpose.ToString());
        }
    }
}
