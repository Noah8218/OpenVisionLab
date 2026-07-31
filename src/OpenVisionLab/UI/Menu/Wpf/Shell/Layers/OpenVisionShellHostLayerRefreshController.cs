using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionShellHostLayerRefreshController
    {
        private readonly Dispatcher dispatcher;
        private readonly ListBox hostLayerRowsList;
        private readonly ScrollViewer hostLayerRowsScrollViewer;
        private readonly OpenVisionShellHostLayerListPresenter layerListPresenter;
        private readonly OpenVisionShellHostLayerDetailPresenter layerDetailPresenter;
        private readonly OpenVisionShellHostLayerWorkspacePresenter layerWorkspacePresenter;
        private readonly Func<string> selectedLayerTitleProvider;
        private readonly IOpenVisionDockedLayerWorkspaceSynchronization dockedLayerWorkspace;
        private readonly Action<IReadOnlyList<string>, string> updateLayerOptions;

        public OpenVisionShellHostLayerRefreshController(
            Dispatcher dispatcher,
            ListBox hostLayerRowsList,
            ScrollViewer hostLayerRowsScrollViewer,
            OpenVisionShellHostLayerListPresenter layerListPresenter,
            OpenVisionShellHostLayerDetailPresenter layerDetailPresenter,
            OpenVisionShellHostLayerWorkspacePresenter layerWorkspacePresenter,
            Func<string> selectedLayerTitleProvider,
            IOpenVisionDockedLayerWorkspaceSynchronization dockedLayerWorkspace,
            Action<IReadOnlyList<string>, string> updateLayerOptions = null)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.hostLayerRowsList = hostLayerRowsList;
            this.hostLayerRowsScrollViewer = hostLayerRowsScrollViewer;
            this.layerListPresenter = layerListPresenter ?? throw new ArgumentNullException(nameof(layerListPresenter));
            this.layerDetailPresenter = layerDetailPresenter ?? throw new ArgumentNullException(nameof(layerDetailPresenter));
            this.layerWorkspacePresenter = layerWorkspacePresenter ?? throw new ArgumentNullException(nameof(layerWorkspacePresenter));
            this.selectedLayerTitleProvider = selectedLayerTitleProvider ?? throw new ArgumentNullException(nameof(selectedLayerTitleProvider));
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.updateLayerOptions = updateLayerOptions;
        }

        public void RefreshRows()
        {
            OpenVisionShellHostLayerListRefreshResult result = layerListPresenter.Refresh();
            updateLayerOptions?.Invoke(layerListPresenter.LayerTitles, result.SelectedLayerTitle);
            dockedLayerWorkspace.SyncLayerDocuments(CreateWorkspaceLayerTitleSnapshot());
            SetSelection(result.SelectedIndex);
            RefreshSelectedLayerDetail(result.SelectedLayerTitle);
            ScrollRowsTo(result.SelectedIndex);
            dockedLayerWorkspace.RefreshLayerViewers();
        }

        public List<string> CreateWorkspaceLayerTitleSnapshot()
        {
            return layerListPresenter.LayerTitles
                .Where(layerDetailPresenter.CanOpenLayerViewer)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public void RefreshSelectedLayerDetail(string layerTitle)
        {
            OpenVisionShellHostLayerDetailState detail = layerDetailPresenter.BuildDetail(layerTitle);
            layerWorkspacePresenter.ApplySelectedLayerDetail(detail);
            RefreshWorkspace(detail);
            RefreshActionButtons();
        }

        public void RefreshActionButtons()
        {
            OpenVisionShellHostLayerActionState actionState =
                layerDetailPresenter.BuildActionState(selectedLayerTitleProvider(), dockedLayerWorkspace.LayerTitles);

            layerWorkspacePresenter.ApplyActionState(actionState);
        }

        public void ApplyWorkspacePointerStatus(OpenVisionZoomableImageStatus status)
        {
            layerWorkspacePresenter.ApplyWorkspacePointerStatus(status);
        }

        private void RefreshWorkspace(OpenVisionShellHostLayerDetailState detail)
        {
            layerWorkspacePresenter.ApplyWorkspace(detail);
            dockedLayerWorkspace.RefreshLayerViewers();
        }

        private void SetSelection(int selectedIndex)
        {
            layerListPresenter.ApplySelection(selectedIndex, index =>
            {
                if (hostLayerRowsList != null)
                {
                    hostLayerRowsList.SelectedIndex = index;
                }
            });
        }

        private void ScrollRowsTo(int selectedIndex)
        {
            dispatcher.BeginInvoke((Action)(() =>
            {
                if (hostLayerRowsScrollViewer == null)
                {
                    return;
                }

                if (selectedIndex <= 1)
                {
                    hostLayerRowsScrollViewer.ScrollToLeftEnd();
                    return;
                }

                if (selectedIndex >= layerListPresenter.RowCount - 2)
                {
                    hostLayerRowsScrollViewer.ScrollToRightEnd();
                    return;
                }

                hostLayerRowsScrollViewer.ScrollToHorizontalOffset(Math.Max(0, (selectedIndex - 1) * 112));
            }));
        }
    }
}
