using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVisionLab
{
    public partial class ArithmeticToolWpfView : UserControl, IArithmeticVisionToolWpfView, IVisionToolPreviewImageCommands, IVisionToolViewLifetime
    {
        private readonly VisionToolDoubleInputCustomToolController toolController;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly ArithmeticToolTextPresenter textPresenter;
        private ArithmeticToolInteractionController interactionController;
        private bool suppressEvents;

        public ArithmeticToolWpfView()
        {
            InitializeComponent();
            toolController = VisionToolDoubleInputCustomToolController.Attach(
                this,
                "VisionMenu.Arithmetic",
                parameterContentHost,
                () => UseOffsetMode,
                applyToolLocalization: ApplyLocalization);
            previewScheduler = new VisionToolDebouncedPreviewScheduler(this, RequestPreviewForCurrentMode, 120);
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
                toolController.SetRunOffsetText,
                toolController.SetSummaryText);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                textPresenter.RefreshSummary,
                () => ParameterChanged(this, EventArgs.Empty),
                ScheduleAutoPreview);
            interactionController = new ArithmeticToolInteractionController(
                parameterChangeController,
                value => suppressEvents = value,
                toolController.SetInputBPreviewVisible,
                toolController.SetOffsetActionsVisible,
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

        public event EventHandler InputALayerChanged
        {
            add { toolController.InputALayerChanged += value; }
            remove { toolController.InputALayerChanged -= value; }
        }

        public event EventHandler InputBLayerChanged
        {
            add { toolController.InputBLayerChanged += value; }
            remove { toolController.InputBLayerChanged -= value; }
        }

        public event EventHandler OutputLayerChanged
        {
            add { toolController.OutputLayerChanged += value; }
            remove { toolController.OutputLayerChanged -= value; }
        }

        public event EventHandler InputAPreviewClicked
        {
            add { toolController.InputAPreviewClicked += value; }
            remove { toolController.InputAPreviewClicked -= value; }
        }

        public event EventHandler InputBPreviewClicked
        {
            add { toolController.InputBPreviewClicked += value; }
            remove { toolController.InputBPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { toolController.OutputPreviewClicked += value; }
            remove { toolController.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { toolController.CreateOutputLayerRequested += value; }
            remove { toolController.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { toolController.RunPreviewRequested += value; }
            remove { toolController.RunPreviewRequested -= value; }
        }

        public event EventHandler RunOffsetRequested
        {
            add { toolController.RunOffsetRequested += value; }
            remove { toolController.RunOffsetRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { toolController.AddPipelineRequested += value; }
            remove { toolController.AddPipelineRequested -= value; }
        }

        public event EventHandler ParameterChanged = delegate { };

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { toolController.LoadPreviewImageRequested += value; }
            remove { toolController.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { toolController.SavePreviewImageRequested += value; }
            remove { toolController.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayerA => toolController.SelectedInputLayerA;
        public string SelectedInputLayerB => toolController.SelectedInputLayerB;
        public string SelectedOutputLayer => toolController.SelectedOutputLayer;
        public string SelectedArithmeticType => interactionController?.SelectedArithmeticType ?? string.Empty;
        public bool UseConstantInput => interactionController?.UseConstantInput == true;
        public bool UseColorConstant => interactionController?.UseColorConstant == true;
        public bool UseOffsetMode => interactionController?.UseOffsetMode == true;

        public void DisposeView()
        {
            previewScheduler.Dispose();
            toolController.Dispose();
        }

        private void ApplyLocalization()
        {
            toolController.ApplyLocalization();
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
            bool previousSuppressEvents = suppressEvents;
            suppressEvents = true;
            try
            {
                interactionController?.ApplySettings(settings);
            }
            finally
            {
                suppressEvents = previousSuppressEvents;
            }
        }

        public void SetLayerList(IEnumerable<string> layerNames, string selectedInputA, string selectedInputB, string selectedOutput)
        {
            toolController.SetLayerList(layerNames, selectedInputA, selectedInputB, selectedOutput);
        }

        public void SetInputAPreview(Bitmap image)
        {
            toolController.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            toolController.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolController.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolController.SetStatus(status);
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

        private void ArithmeticType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            interactionController?.HandleArithmeticTypeChanged();
        }

        private void Mode_Changed(object sender, RoutedEventArgs e)
        {
            interactionController?.HandleModeChanged();
        }

        private void Parameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            interactionController?.HandleParameterTextChanged();
        }

        private void ScheduleAutoPreview()
        {
            previewScheduler.Schedule();
        }

        private void RequestPreviewForCurrentMode()
        {
            // Offset mode has a separate execution path and status; auto-preview must respect the visible mode.
            if (UseOffsetMode)
            {
                toolController.RequestRunOffset();
                return;
            }

            toolController.RequestRunPreview();
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            interactionController?.HandleNumberTextInput(e);
        }

        private void SignedNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            interactionController?.HandleSignedNumberTextInput(e);
        }
    }
}
