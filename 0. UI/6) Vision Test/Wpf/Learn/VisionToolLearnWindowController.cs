using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class VisionToolLearnWindowController
    {
        private readonly Func<Window> ownerProvider;
        private OpenVisionLearnWindow learnWindow;
        private int learnTopicIndex = -1;

        public VisionToolLearnWindowController(Func<Window> ownerProvider)
        {
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
        }

        public void Open(int topicIndex)
        {
            if (learnWindow != null && learnTopicIndex == topicIndex)
            {
                learnWindow.Activate();
                return;
            }

            CloseCurrentWindow();
            learnTopicIndex = topicIndex;
            learnWindow = new OpenVisionLearnWindow(127, 255, false, topicIndex)
            {
                Owner = ownerProvider()
            };
            learnWindow.Closed += LearnWindow_Closed;
            learnWindow.Show();
        }

        private void LearnWindow_Closed(object sender, EventArgs e)
        {
            if (sender is not OpenVisionLearnWindow window)
            {
                return;
            }

            window.Closed -= LearnWindow_Closed;
            if (ReferenceEquals(learnWindow, window))
            {
                learnWindow = null;
                learnTopicIndex = -1;
            }
        }

        private void CloseCurrentWindow()
        {
            OpenVisionLearnWindow window = learnWindow;
            if (window == null)
            {
                return;
            }

            window.Closed -= LearnWindow_Closed;
            learnWindow = null;
            learnTopicIndex = -1;
            window.Close();
        }
    }
}
