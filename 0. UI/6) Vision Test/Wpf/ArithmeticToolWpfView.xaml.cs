using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    public partial class ArithmeticToolWpfView : VisionToolDoubleInputCustomToolViewBase, IArithmeticVisionToolWpfView
    {
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly ArithmeticToolPreviewController previewController;
        private readonly ArithmeticToolTextPresenter textPresenter;
        private ArithmeticToolInteractionController interactionController;
        private bool suppressEvents;

        public ArithmeticToolWpfView()
        {
            InitializeComponent();
            AttachToolController(
                "VisionMenu.Arithmetic",
                parameterContentHost,
                () => UseOffsetMode,
                applyToolLocalization: ApplyLocalization);
            previewController = new ArithmeticToolPreviewController(
                this,
                ToolController,
                () => interactionController?.UseOffsetMode == true);
            textPresenter = new ArithmeticToolTextPresenter(
                () => interactionController?.CreateTextState() ?? ArithmeticToolTextState.Empty,
                gbOperation,
                lblArithmeticMode,
                rdoModeOperation,
                rdoModeOffset,
                lblArithmeticType,
                gbInputBSource,
                rdoSourceImage,
                rdoContrast,
                rdoGray,
                rdoColor,
                lblConstantGray,
                groupConstant,
                gbOffset,
                txtCopyOffset,
                ToolController.SetRunOffsetText,
                ToolController.SetSummaryText);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                textPresenter.RefreshSummary,
                () => ParameterChanged(this, EventArgs.Empty),
                previewController.ScheduleAutoPreview);
            interactionController = new ArithmeticToolInteractionController(
                parameterChangeController,
                () => suppressEvents,
                value => suppressEvents = value,
                ToolController.SetInputBPreviewVisible,
                ToolController.SetOffsetActionsVisible,
                cbArithmeticType,
                rdoModeOperation,
                rdoSourceImage,
                rdoContrast,
                rdoColor,
                rdoModeOffset,
                panelConstantMode,
                groupConstant,
                panelArithmeticType,
                gbInputBSource,
                gbOffset,
                rowInputBSource,
                rowInputBSourceGap,
                rowConstant,
                rowOffset,
                rowOffsetGap,
                txtGray,
                txtB,
                txtG,
                txtR,
                txtOffsetX,
                txtOffsetY);
            ApplyLocalization();
            parameterChangeController.RefreshProgrammatic(interactionController.RefreshMode);
        }

        public event EventHandler ParameterChanged = delegate { };

        public string SelectedArithmeticType => interactionController?.SelectedArithmeticType ?? string.Empty;
        public bool UseConstantInput => interactionController?.UseConstantInput == true;
        public bool UseColorConstant => interactionController?.UseColorConstant == true;
        public bool UseOffsetMode => interactionController?.UseOffsetMode == true;

        protected override void DisposeToolResources()
        {
            interactionController?.Detach();
            previewController?.Dispose();
        }

        private void ApplyLocalization()
        {
            ToolController.ApplyLocalization();
            textPresenter?.ApplyLocalization();
        }

        public void SetOperationList(IEnumerable<string> operationNames, string selectedOperation)
        {
            interactionController.SetOperationList(operationNames, selectedOperation);
        }

        public ArithmeticToolSettings CaptureSettings()
        {
            return interactionController?.CaptureSettings() ?? new ArithmeticToolSettings();
        }

        public void ApplyPersistedSettings(ArithmeticToolSettings settings)
        {
            interactionController?.ApplySettings(settings);
        }

        public int GetGrayValue(int fallback)
        {
            return interactionController?.GetGrayValue(fallback) ?? fallback;
        }

        public int GetBValue(int fallback)
        {
            return interactionController?.GetBValue(fallback) ?? fallback;
        }

        public int GetGValue(int fallback)
        {
            return interactionController?.GetGValue(fallback) ?? fallback;
        }

        public int GetRValue(int fallback)
        {
            return interactionController?.GetRValue(fallback) ?? fallback;
        }

        public int GetOffsetX(int fallback)
        {
            return interactionController?.GetOffsetX(fallback) ?? fallback;
        }

        public int GetOffsetY(int fallback)
        {
            return interactionController?.GetOffsetY(fallback) ?? fallback;
        }

    }
}
