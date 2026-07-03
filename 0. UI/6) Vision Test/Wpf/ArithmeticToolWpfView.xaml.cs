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
        private readonly VisionToolDoubleInputToolEventHub eventHub;
        private readonly VisionToolLanguageChangeController languageChangeController;
        private readonly VisionToolDoubleInputCustomToolRuntime toolRuntime;
        private readonly VisionToolParameterChangeController parameterChangeController;
        private readonly VisionToolDebouncedPreviewScheduler previewScheduler;
        private readonly ArithmeticToolTextPresenter textPresenter;
        private ArithmeticToolInteractionController interactionController;
        private bool suppressEvents;

        public ArithmeticToolWpfView()
        {
            InitializeComponent();
            eventHub = new VisionToolDoubleInputToolEventHub(this);
            toolRuntime = VisionToolDoubleInputCustomToolRuntime.Attach(
                this,
                "VisionMenu.Arithmetic",
                parameterContentHost,
                () => UseOffsetMode,
                eventHub.RaiseInputALayerChanged,
                eventHub.RaiseInputBLayerChanged,
                eventHub.RaiseOutputLayerChanged,
                eventHub.RaiseInputAPreviewClicked,
                eventHub.RaiseInputBPreviewClicked,
                eventHub.RaiseOutputPreviewClicked,
                eventHub.RaiseCreateOutputLayerRequested,
                eventHub.RaiseRunPreviewRequested,
                eventHub.RaiseRunOffsetRequested,
                eventHub.RaiseAddPipelineRequested,
                eventHub.RaiseLoadPreviewImageRequested,
                eventHub.RaiseSavePreviewImageRequested);
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
                toolRuntime.SetRunOffsetText,
                toolRuntime.SetSummaryText);
            parameterChangeController = new VisionToolParameterChangeController(
                () => suppressEvents,
                textPresenter.RefreshSummary,
                () => ParameterChanged(this, EventArgs.Empty),
                ScheduleAutoPreview);
            interactionController = new ArithmeticToolInteractionController(
                parameterChangeController,
                value => suppressEvents = value,
                toolRuntime.SetInputBPreviewVisible,
                toolRuntime.SetOffsetActionsVisible,
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
            languageChangeController = VisionToolLanguageChangeController.Attach(ApplyLocalization);
            parameterChangeController.RefreshProgrammatic(interactionController.RefreshMode);
        }

        public event EventHandler InputALayerChanged
        {
            add { eventHub.InputALayerChanged += value; }
            remove { eventHub.InputALayerChanged -= value; }
        }

        public event EventHandler InputBLayerChanged
        {
            add { eventHub.InputBLayerChanged += value; }
            remove { eventHub.InputBLayerChanged -= value; }
        }

        public event EventHandler OutputLayerChanged
        {
            add { eventHub.OutputLayerChanged += value; }
            remove { eventHub.OutputLayerChanged -= value; }
        }

        public event EventHandler InputAPreviewClicked
        {
            add { eventHub.InputAPreviewClicked += value; }
            remove { eventHub.InputAPreviewClicked -= value; }
        }

        public event EventHandler InputBPreviewClicked
        {
            add { eventHub.InputBPreviewClicked += value; }
            remove { eventHub.InputBPreviewClicked -= value; }
        }

        public event EventHandler OutputPreviewClicked
        {
            add { eventHub.OutputPreviewClicked += value; }
            remove { eventHub.OutputPreviewClicked -= value; }
        }

        public event EventHandler CreateOutputLayerRequested
        {
            add { eventHub.CreateOutputLayerRequested += value; }
            remove { eventHub.CreateOutputLayerRequested -= value; }
        }

        public event EventHandler RunPreviewRequested
        {
            add { eventHub.RunPreviewRequested += value; }
            remove { eventHub.RunPreviewRequested -= value; }
        }

        public event EventHandler RunOffsetRequested
        {
            add { eventHub.RunOffsetRequested += value; }
            remove { eventHub.RunOffsetRequested -= value; }
        }

        public event EventHandler AddPipelineRequested
        {
            add { eventHub.AddPipelineRequested += value; }
            remove { eventHub.AddPipelineRequested -= value; }
        }

        public event EventHandler ParameterChanged = delegate { };

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> LoadPreviewImageRequested
        {
            add { eventHub.LoadPreviewImageRequested += value; }
            remove { eventHub.LoadPreviewImageRequested -= value; }
        }

        public event EventHandler<VisionToolPreviewImageCommandEventArgs> SavePreviewImageRequested
        {
            add { eventHub.SavePreviewImageRequested += value; }
            remove { eventHub.SavePreviewImageRequested -= value; }
        }

        public string SelectedInputLayerA => toolRuntime.SelectedInputLayerA;
        public string SelectedInputLayerB => toolRuntime.SelectedInputLayerB;
        public string SelectedOutputLayer => toolRuntime.SelectedOutputLayer;
        public string SelectedArithmeticType => interactionController?.SelectedArithmeticType ?? string.Empty;
        public bool UseConstantInput => interactionController?.UseConstantInput == true;
        public bool UseColorConstant => interactionController?.UseColorConstant == true;
        public bool UseOffsetMode => interactionController?.UseOffsetMode == true;

        public void DisposeView()
        {
            languageChangeController.Dispose();
            previewScheduler.Dispose();
            toolRuntime.Dispose();
        }

        private void ApplyLocalization()
        {
            toolRuntime.ApplyLocalization();
            textPresenter.ApplyLocalization();
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
            toolRuntime.SetLayerList(layerNames, selectedInputA, selectedInputB, selectedOutput);
        }

        public void SetInputAPreview(Bitmap image)
        {
            toolRuntime.SetInputAPreview(image);
        }

        public void SetInputBPreview(Bitmap image)
        {
            toolRuntime.SetInputBPreview(image);
        }

        public void SetOutputPreview(Bitmap image)
        {
            toolRuntime.SetOutputPreview(image);
        }

        public void SetStatus(string status)
        {
            toolRuntime.SetStatus(status);
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
                eventHub.RaiseRunOffsetRequested();
                return;
            }

            eventHub.RaiseRunPreviewRequested();
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
