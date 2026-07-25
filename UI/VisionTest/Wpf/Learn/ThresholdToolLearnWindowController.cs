using Lib.OpenCV.Property;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class ThresholdToolLearnWindowController : IDisposable
    {
        private readonly ThresholdToolPresenter presenter;
        private readonly VisionToolThresholdInteractionController interactionController;
        private readonly Func<Window> ownerProvider;
        private OpenVisionLearnWindow learnWindow;

        public ThresholdToolLearnWindowController(
            ThresholdToolPresenter presenter,
            VisionToolThresholdInteractionController interactionController,
            Func<Window> ownerProvider)
        {
            this.presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            this.interactionController = interactionController ?? throw new ArgumentNullException(nameof(interactionController));
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
        }

        public void Open()
        {
            interactionController.FlushParameterBindings();
            ThresholdToolProperty property = presenter.CreateProperty();
            if (learnWindow != null)
            {
                learnWindow.Activate();
                return;
            }

            learnWindow = new OpenVisionLearnWindow(
                property.Threshold,
                property.MaxValue,
                property.ThresholdType == OpenCvSharp.ThresholdTypes.BinaryInv)
            {
                Owner = ownerProvider()
            };
            learnWindow.ApplyThresholdRequested += LearnWindow_ApplyThresholdRequested;
            learnWindow.Closed += LearnWindow_Closed;
            learnWindow.Show();
        }

        public void Dispose()
        {
            OpenVisionLearnWindow window = learnWindow;
            if (window == null)
            {
                return;
            }

            Detach(window);
            learnWindow = null;
            window.Close();
        }

        private void LearnWindow_ApplyThresholdRequested(object sender, OpenVisionLearnThresholdApplyEventArgs e)
        {
            interactionController.ApplyBasicThresholdFromGuide(e.Threshold, e.Invert);
        }

        private void LearnWindow_Closed(object sender, EventArgs e)
        {
            OpenVisionLearnWindow window = sender as OpenVisionLearnWindow;
            if (window == null)
            {
                return;
            }

            Detach(window);
            if (ReferenceEquals(learnWindow, window))
            {
                learnWindow = null;
            }
        }

        private void Detach(OpenVisionLearnWindow window)
        {
            window.ApplyThresholdRequested -= LearnWindow_ApplyThresholdRequested;
            window.Closed -= LearnWindow_Closed;
        }
    }
}
