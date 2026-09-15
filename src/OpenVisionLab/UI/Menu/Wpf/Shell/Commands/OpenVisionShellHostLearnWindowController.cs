using System;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    /// <summary>
    /// Owns the Learn Window lifetime and the Shell entry points that select a Learn topic.
    /// Workspace sample execution remains in OpenVisionShellHostCommandController and is
    /// supplied as a callback so Learn does not own Recipe or workspace state.
    /// </summary>
    internal sealed class OpenVisionShellHostLearnWindowController
    {
        private readonly Func<Window> ownerProvider;
        private readonly Action<string> openPracticeSamples;
        private readonly Action<VISION_MENU> selectToolMenu;
        private OpenVisionLearnWindow learnWindow;

        public OpenVisionShellHostLearnWindowController(
            Func<Window> ownerProvider,
            Action<string> openPracticeSamples,
            Action<VISION_MENU> selectToolMenu)
        {
            this.ownerProvider = ownerProvider ?? throw new ArgumentNullException(nameof(ownerProvider));
            this.openPracticeSamples = openPracticeSamples ?? throw new ArgumentNullException(nameof(openPracticeSamples));
            this.selectToolMenu = selectToolMenu ?? throw new ArgumentNullException(nameof(selectToolMenu));
        }

        public void OpenLearn()
        {
            OpenLearn(null);
        }

        public void OpenLearnForTool(VISION_MENU menu)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForTool(menu, out OpenVisionLearnTopicIndex topicIndex))
            {
                OpenLearn(topicIndex);
            }
        }

        public void OpenLearnForToolType(string toolType)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForToolType(toolType, out OpenVisionLearnTopicIndex topicIndex))
            {
                OpenLearn(topicIndex);
            }
        }

        public void OpenSamplesForTool(VISION_MENU menu)
        {
            if (OpenVisionLearnTopicCatalog.TryResolveForTool(menu, out OpenVisionLearnTopicIndex topicIndex))
            {
                openPracticeSamples(OpenVisionLearnTopicCatalog.Resolve(topicIndex).PracticePathId);
            }
        }

        private void OpenLearn(OpenVisionLearnTopicIndex? topicIndex)
        {
            if (learnWindow != null)
            {
                if (topicIndex.HasValue)
                {
                    learnWindow.SelectTopic(topicIndex.Value);
                }

                learnWindow.Activate();
                return;
            }

            learnWindow = topicIndex.HasValue
                ? new OpenVisionLearnWindow(127, 255, false, (int)topicIndex.Value)
                : new OpenVisionLearnWindow();
            learnWindow.Owner = ownerProvider();
            learnWindow.SetOpenLearnDocumentAction(OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile);
            learnWindow.SetOpenPracticeSamplesAction(openPracticeSamples);
            learnWindow.SetOpenRelatedToolAction(selectToolMenu);
            learnWindow.Closed += LearnWindow_Closed;
            learnWindow.Show();
        }

        private void LearnWindow_Closed(object sender, EventArgs e)
        {
            if (learnWindow != null)
            {
                learnWindow.Closed -= LearnWindow_Closed;
                learnWindow = null;
            }
        }
    }
}
