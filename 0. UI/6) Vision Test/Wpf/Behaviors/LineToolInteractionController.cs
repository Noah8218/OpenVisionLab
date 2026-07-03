using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using System;
using System.Windows.Controls;

namespace OpenVisionLab
{
    internal sealed class LineToolInteractionController
    {
        private readonly LineToolPresenter presenter;
        private readonly VisionToolPropertyGridHost propertyGridController;
        private readonly VisionToolPropertyChangeController propertyChangeController;
        private readonly RadioButton purposeEdgeRadioButton;
        private readonly RadioButton purposeMeasureRadioButton;
        private readonly RadioButton purposeIntersectionRadioButton;
        private readonly RadioButton lineARadioButton;
        private readonly RadioButton lineBRadioButton;
        private readonly Button editSelectedRoiButton;
        private readonly Action refreshSummary;
        private readonly Action clearResultReview;
        private readonly Action editSelectedRoiRequested;
        private bool disposed;

        public LineToolInteractionController(
            LineToolPresenter presenter,
            VisionToolPropertyGridHost propertyGridController,
            VisionToolPropertyChangeController propertyChangeController,
            RadioButton purposeEdgeRadioButton,
            RadioButton purposeMeasureRadioButton,
            RadioButton purposeIntersectionRadioButton,
            RadioButton lineARadioButton,
            RadioButton lineBRadioButton,
            Button editSelectedRoiButton,
            Action refreshSummary,
            Action clearResultReview,
            Action editSelectedRoiRequested)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.propertyGridController = propertyGridController ?? throw new ArgumentNullException(nameof(propertyGridController));
            this.propertyChangeController = propertyChangeController ?? throw new ArgumentNullException(nameof(propertyChangeController));
            this.purposeEdgeRadioButton = purposeEdgeRadioButton ?? throw new ArgumentNullException(nameof(purposeEdgeRadioButton));
            this.purposeMeasureRadioButton = purposeMeasureRadioButton ?? throw new ArgumentNullException(nameof(purposeMeasureRadioButton));
            this.purposeIntersectionRadioButton = purposeIntersectionRadioButton ?? throw new ArgumentNullException(nameof(purposeIntersectionRadioButton));
            this.lineARadioButton = lineARadioButton ?? throw new ArgumentNullException(nameof(lineARadioButton));
            this.lineBRadioButton = lineBRadioButton ?? throw new ArgumentNullException(nameof(lineBRadioButton));
            this.editSelectedRoiButton = editSelectedRoiButton ?? throw new ArgumentNullException(nameof(editSelectedRoiButton));
            this.refreshSummary = refreshSummary ?? throw new ArgumentNullException(nameof(refreshSummary));
            this.clearResultReview = clearResultReview ?? throw new ArgumentNullException(nameof(clearResultReview));
            this.editSelectedRoiRequested = editSelectedRoiRequested ?? throw new ArgumentNullException(nameof(editSelectedRoiRequested));

            AttachCommands();
        }

        public LineToolPurpose SelectedPurpose
        {
            get
            {
                if (purposeMeasureRadioButton.IsChecked == true)
                {
                    return LineToolPurpose.Measure;
                }

                if (purposeIntersectionRadioButton.IsChecked == true)
                {
                    return LineToolPurpose.Intersection;
                }

                return LineToolPurpose.Edge;
            }
        }

        public string SelectedLineName => IsLineBSelected ? "Line B" : "Line A";

        public bool IsLineBSelected => lineBRadioButton.IsChecked == true;

        public LineGaugeProperty GetSelectedLineProperty()
        {
            return presenter.GetSelectedLineProperty(IsLineBSelected);
        }

        public LineGaugeProperty CreateSelectedLineProperty()
        {
            return presenter.CreateSelectedLineProperty(IsLineBSelected);
        }

        public void SetPurposeForTest(string purpose)
        {
            if (string.Equals(purpose, nameof(LineToolPurpose.Measure), StringComparison.OrdinalIgnoreCase))
            {
                purposeMeasureRadioButton.IsChecked = true;
            }
            else if (string.Equals(purpose, nameof(LineToolPurpose.Intersection), StringComparison.OrdinalIgnoreCase))
            {
                purposeIntersectionRadioButton.IsChecked = true;
            }
            else
            {
                purposeEdgeRadioButton.IsChecked = true;
            }

            HandlePurposeChanged();
        }

        public void SetLineSettingForTest(string setting)
        {
            if (string.Equals(setting, "Line B", StringComparison.OrdinalIgnoreCase)
                || string.Equals(setting, "B", StringComparison.OrdinalIgnoreCase)
                || string.Equals(setting, "Right", StringComparison.OrdinalIgnoreCase))
            {
                lineBRadioButton.IsChecked = true;
            }
            else
            {
                lineARadioButton.IsChecked = true;
            }

            HandleLineSelectionChanged();
        }

        public void Detach()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            purposeEdgeRadioButton.Command = null;
            purposeMeasureRadioButton.Command = null;
            purposeIntersectionRadioButton.Command = null;
            lineARadioButton.Command = null;
            lineBRadioButton.Command = null;
            editSelectedRoiButton.Command = null;
        }

        private void AttachCommands()
        {
            // Selection commands keep radio/button behavior on the MVVM command path instead of View event handlers.
            purposeEdgeRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            purposeMeasureRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            purposeIntersectionRadioButton.Command = new RelayCommand(HandlePurposeChanged);
            lineARadioButton.Command = new RelayCommand(HandleLineSelectionChanged);
            lineBRadioButton.Command = new RelayCommand(HandleLineSelectionChanged);
            editSelectedRoiButton.Command = new RelayCommand(editSelectedRoiRequested);
        }

        private void HandlePurposeChanged()
        {
            refreshSummary();
            clearResultReview();
        }

        private void HandleLineSelectionChanged()
        {
            propertyGridController.SelectObject(GetSelectedLineProperty());
            propertyChangeController.RefreshViewState();
        }
    }
}
