using OpenVisionLab.Mvvm;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVisionLab
{
    internal sealed class VisionToolActionBehavior
    {
        private readonly FrameworkElement inputAPreviewFrame;
        private readonly FrameworkElement inputAPreviewContent;
        private readonly FrameworkElement inputBPreviewFrame;
        private readonly FrameworkElement inputBPreviewContent;
        private readonly FrameworkElement outputPreviewFrame;
        private readonly FrameworkElement outputPreviewContent;
        private readonly Button createOutputLayerButton;
        private readonly Button runPreviewButton;
        private readonly Button addPipelineButton;
        private readonly Button runOffsetButton;
        private readonly Action inputAPreviewClicked;
        private readonly Action inputBPreviewClicked;
        private readonly Action outputPreviewClicked;
        private int lastInputAPreviewClickTimestamp = int.MinValue;
        private int lastInputBPreviewClickTimestamp = int.MinValue;
        private int lastOutputPreviewClickTimestamp = int.MinValue;
        private bool disposed;

        private VisionToolActionBehavior(
            FrameworkElement inputAPreviewFrame,
            FrameworkElement inputAPreviewContent,
            FrameworkElement inputBPreviewFrame,
            FrameworkElement inputBPreviewContent,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewContent,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            Button runOffsetButton,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action runOffsetRequested,
            Func<bool> useOffsetMode)
        {
            this.inputAPreviewFrame = inputAPreviewFrame;
            this.inputAPreviewContent = inputAPreviewContent;
            this.inputBPreviewFrame = inputBPreviewFrame;
            this.inputBPreviewContent = inputBPreviewContent;
            this.outputPreviewFrame = outputPreviewFrame;
            this.outputPreviewContent = outputPreviewContent;
            this.createOutputLayerButton = createOutputLayerButton;
            this.runPreviewButton = runPreviewButton;
            this.addPipelineButton = addPipelineButton;
            this.runOffsetButton = runOffsetButton;
            this.inputAPreviewClicked = inputAPreviewClicked;
            this.inputBPreviewClicked = inputBPreviewClicked;
            this.outputPreviewClicked = outputPreviewClicked;

            AttachPreview(inputAPreviewFrame, InputAPreview_MouseUp);
            AttachPreview(inputAPreviewContent, InputAPreview_MouseUp);
            AttachPreview(inputBPreviewFrame, InputBPreview_MouseUp);
            AttachPreview(inputBPreviewContent, InputBPreview_MouseUp);
            AttachPreview(outputPreviewFrame, OutputPreview_MouseUp);
            AttachPreview(outputPreviewContent, OutputPreview_MouseUp);
            BindButton(createOutputLayerButton, createOutputLayerRequested);
            BindButton(runPreviewButton, () => RunPreview(runPreviewRequested, runOffsetRequested, useOffsetMode));
            BindButton(addPipelineButton, addPipelineRequested);
            BindButton(runOffsetButton, runOffsetRequested);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DetachPreview(inputAPreviewFrame, InputAPreview_MouseUp);
            DetachPreview(inputAPreviewContent, InputAPreview_MouseUp);
            DetachPreview(inputBPreviewFrame, InputBPreview_MouseUp);
            DetachPreview(inputBPreviewContent, InputBPreview_MouseUp);
            DetachPreview(outputPreviewFrame, OutputPreview_MouseUp);
            DetachPreview(outputPreviewContent, OutputPreview_MouseUp);
            ClearButton(createOutputLayerButton);
            ClearButton(runPreviewButton);
            ClearButton(addPipelineButton);
            ClearButton(runOffsetButton);
        }

        public static VisionToolActionBehavior AttachSingle(
            FrameworkElement inputPreviewFrame,
            FrameworkElement inputPreviewContent,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewContent,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            VisionToolActionRequestController actionRequests)
        {
            if (actionRequests == null)
            {
                throw new ArgumentNullException(nameof(actionRequests));
            }

            return AttachSingle(
                inputPreviewFrame,
                inputPreviewContent,
                outputPreviewFrame,
                outputPreviewContent,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                actionRequests.RequestInputPreviewClick,
                actionRequests.RequestOutputPreviewClick,
                actionRequests.RequestCreateOutputLayer,
                actionRequests.RequestRunPreview,
                actionRequests.RequestAddPipeline);
        }

        public static VisionToolActionBehavior AttachSingle(
            FrameworkElement inputPreviewFrame,
            FrameworkElement inputPreviewContent,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewContent,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            Action inputPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested)
        {
            return new VisionToolActionBehavior(
                inputPreviewFrame,
                inputPreviewContent,
                null,
                null,
                outputPreviewFrame,
                outputPreviewContent,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                null,
                inputPreviewClicked,
                null,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                null,
                null);
        }

        public static VisionToolActionBehavior AttachArithmetic(
            FrameworkElement inputAPreviewFrame,
            FrameworkElement inputAPreviewContent,
            FrameworkElement inputBPreviewFrame,
            FrameworkElement inputBPreviewContent,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewContent,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            Button runOffsetButton,
            VisionToolActionRequestController actionRequests,
            Func<bool> useOffsetMode)
        {
            if (actionRequests == null)
            {
                throw new ArgumentNullException(nameof(actionRequests));
            }

            return AttachArithmetic(
                inputAPreviewFrame,
                inputAPreviewContent,
                inputBPreviewFrame,
                inputBPreviewContent,
                outputPreviewFrame,
                outputPreviewContent,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                runOffsetButton,
                actionRequests.RequestInputPreviewClick,
                actionRequests.RequestInputBPreviewClick,
                actionRequests.RequestOutputPreviewClick,
                actionRequests.RequestCreateOutputLayer,
                actionRequests.RequestRunPreview,
                actionRequests.RequestAddPipeline,
                actionRequests.RequestRunOffset,
                useOffsetMode);
        }

        public static VisionToolActionBehavior AttachArithmetic(
            FrameworkElement inputAPreviewFrame,
            FrameworkElement inputAPreviewContent,
            FrameworkElement inputBPreviewFrame,
            FrameworkElement inputBPreviewContent,
            FrameworkElement outputPreviewFrame,
            FrameworkElement outputPreviewContent,
            Button createOutputLayerButton,
            Button runPreviewButton,
            Button addPipelineButton,
            Button runOffsetButton,
            Action inputAPreviewClicked,
            Action inputBPreviewClicked,
            Action outputPreviewClicked,
            Action createOutputLayerRequested,
            Action runPreviewRequested,
            Action addPipelineRequested,
            Action runOffsetRequested,
            Func<bool> useOffsetMode)
        {
            return new VisionToolActionBehavior(
                inputAPreviewFrame,
                inputAPreviewContent,
                inputBPreviewFrame,
                inputBPreviewContent,
                outputPreviewFrame,
                outputPreviewContent,
                createOutputLayerButton,
                runPreviewButton,
                addPipelineButton,
                runOffsetButton,
                inputAPreviewClicked,
                inputBPreviewClicked,
                outputPreviewClicked,
                createOutputLayerRequested,
                runPreviewRequested,
                addPipelineRequested,
                runOffsetRequested,
                useOffsetMode);
        }

        private static void AttachPreview(FrameworkElement element, MouseButtonEventHandler handler)
        {
            if (element == null)
            {
                return;
            }

            element.RemoveHandler(UIElement.MouseUpEvent, handler);
            element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, handler);
            // Inline viewers own zoom/pan gestures and may mark mouse events handled.
            // Layer activation is a higher-level tool action, so receive handled clicks too.
            element.AddHandler(UIElement.MouseUpEvent, handler, true);
            element.AddHandler(UIElement.MouseLeftButtonUpEvent, handler, true);
        }

        private static void DetachPreview(FrameworkElement element, MouseButtonEventHandler handler)
        {
            if (element == null)
            {
                return;
            }

            element.RemoveHandler(UIElement.MouseUpEvent, handler);
            element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, handler);
        }

        private static void BindButton(Button button, Action action)
        {
            if (button == null || action == null)
            {
                return;
            }

            // Use ICommand for common tool actions so button wiring follows the MVVM command path.
            button.Command = new RelayCommand(action);
        }

        private static void ClearButton(Button button)
        {
            if (button != null)
            {
                button.Command = null;
            }
        }

        private static void RunPreview(Action runPreviewRequested, Action runOffsetRequested, Func<bool> useOffsetMode)
        {
            if (useOffsetMode?.Invoke() == true)
            {
                runOffsetRequested?.Invoke();
                return;
            }

            runPreviewRequested?.Invoke();
        }

        private void InputAPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            InvokePreviewClick(sender, e, inputAPreviewClicked, ref lastInputAPreviewClickTimestamp);
        }

        private void InputBPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            InvokePreviewClick(sender, e, inputBPreviewClicked, ref lastInputBPreviewClickTimestamp);
        }

        private void OutputPreview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            InvokePreviewClick(sender, e, outputPreviewClicked, ref lastOutputPreviewClickTimestamp);
        }

        private static void InvokePreviewClick(
            object sender,
            MouseButtonEventArgs e,
            Action action,
            ref int lastClickTimestamp)
        {
            if (e.ChangedButton != MouseButton.Left || IsHandledInlinePreviewPanGesture(sender, e))
            {
                return;
            }

            e.Handled = true;
            // MouseUp and MouseLeftButtonUp are both observed because inline viewers own mouse
            // capture. They can describe the same operator click, so only dispatch it once.
            if (lastClickTimestamp == e.Timestamp)
            {
                return;
            }

            lastClickTimestamp = e.Timestamp;
            action?.Invoke();
        }

        private static bool IsHandledInlinePreviewPanGesture(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                return false;
            }

            return TryFindInlinePreviewSlot(sender as DependencyObject, out VisionToolInlinePreviewSlot senderSlot)
                ? senderSlot.LastMouseUpWasPanGesture
                : TryFindInlinePreviewSlot(e.OriginalSource as DependencyObject, out VisionToolInlinePreviewSlot sourceSlot)
                    && sourceSlot.LastMouseUpWasPanGesture;
        }

        private static bool TryFindInlinePreviewSlot(DependencyObject start, out VisionToolInlinePreviewSlot slot)
        {
            DependencyObject current = start;
            while (current != null)
            {
                if (current is VisionToolInlinePreviewSlot previewSlot)
                {
                    slot = previewSlot;
                    return true;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            slot = null;
            return false;
        }
    }
}
