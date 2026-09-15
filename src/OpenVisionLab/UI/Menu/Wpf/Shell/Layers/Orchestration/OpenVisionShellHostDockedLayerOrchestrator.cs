using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    // Owns the shell-facing docking command surface and keeps workspace state
    // persistence, gesture forwarding, and document operations on one lifetime.
    internal sealed class OpenVisionShellHostDockedLayerOrchestrator
    {
        #region Fields

        private readonly OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> composition;

        #endregion

        #region Constructors

        public OpenVisionShellHostDockedLayerOrchestrator(
            OpenVisionDockWorkspaceComposition<OpenVisionDockedLayerDocumentState, OpenVisionDockedLayerWorkspaceState> composition)
        {
            this.composition = composition ?? throw new ArgumentNullException(nameof(composition));
            this.composition.WorkspaceStateChanged += OnCompositionWorkspaceStateChanged;
            this.composition.AttachGestureController();
        }

        #endregion

        #region Events

        public event EventHandler WorkspaceStateChanged;

        #endregion

        #region Commands

        public bool DockLayer(string layerTitle)
        {
            return composition.DockDocument(layerTitle);
        }

        public bool ActivateLayer(string layerTitle)
        {
            return composition.SelectDocument(layerTitle);
        }

        public void SyncLayers(IReadOnlyList<string> layerTitles)
        {
            composition.SyncDocuments(layerTitles);
        }

        public void ClearLayers()
        {
            composition.ClearDocuments();
        }

        public void RefreshViews()
        {
            composition.RefreshDocuments();
        }

        public void ClearDocuments()
        {
            composition.ClearDocumentContents();
        }

        public bool SplitToNewPane(string layerTitle)
        {
            return composition.SplitToNewPane(layerTitle);
        }

        public bool MoveToPrimaryPane(string layerTitle)
        {
            return composition.MoveToPrimaryPane(layerTitle);
        }

        public bool DockLayerToGuideZone(string layerTitle, DockingGuideZone zone)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return composition.DockToPrimaryGuideZone(layerTitle, zone);
        }

        public bool DockLayerToGuideZone(string layerTitle, DockingGuideZone zone, OpenVisionDockPaneHandle targetPane)
        {
            if (string.IsNullOrWhiteSpace(layerTitle))
            {
                return false;
            }

            return composition.DockToGuideZone(layerTitle, zone, targetPane);
        }

        public bool ArrangePanes(Orientation orientation, params string[] layerTitles)
        {
            return composition.ArrangePanes(orientation, layerTitles);
        }

        public bool ArrangeGrid(params string[] layerTitles)
        {
            return composition.ArrangeGrid(layerTitles);
        }

        public void RefreshLayout()
        {
            composition.RefreshLayout();
        }

        #endregion

        #region Gesture

        public void AttachLifecycle(IOpenVisionDockLifecycle lifecycle)
        {
            if (lifecycle == null)
            {
                throw new ArgumentNullException(nameof(lifecycle));
            }

            composition.AttachLifecycle(lifecycle);
        }

        public void ShowGuideAt(Point point)
        {
            composition.ShowGuideAt(point);
        }

        public bool BeginTestDragGuide(DependencyObject source, Point point)
        {
            return composition.BeginTestDragGuide(source, point);
        }

        public bool IsGestureSource(DependencyObject source)
        {
            return composition.IsGestureSource(source);
        }

        public void ResetGuide()
        {
            composition.ResetGuide();
        }

        #endregion

        #region State

        public IOpenVisionDockedLayerViewer FindViewer(string layerTitle)
        {
            return composition.FindContent<IOpenVisionDockedLayerViewer>(layerTitle);
        }

        public void ResetLayoutToPrimaryPane()
        {
            composition.ResetLayoutToPrimaryPane();
        }

        public OpenVisionDockedLayerWorkspaceState GetWorkspaceState()
        {
            return composition.GetWorkspaceState();
        }

        public IEnumerable<FrameworkElement> EnumerateGestureHeaders()
        {
            return composition.EnumerateGestureHeaders();
        }

        public void EnsureStateLoaded()
        {
            composition.EnsureStateLoaded();
        }

        public bool ApplyPersistedLayers()
        {
            return composition.ApplyPersistedDocuments();
        }

        public bool RestoreWorkspaceState()
        {
            // Persisted docking is restored only through explicit commands/tests.
            // Loading or processing images must not create comparison panels behind the operator's back.
            ApplyPersistedLayers();
            RefreshViews();
            bool layoutRestored = composition.RestoreLayoutState();
            RefreshLayout();

            OpenVisionDockedLayerWorkspaceState state = GetWorkspaceState();
            bool restored = layoutRestored
                && composition.DocumentCount > 0
                && state.Documents.Sum(document => document.TextureTileCount) >= composition.DocumentCount;
            if (restored)
            {
                NotifyWorkspaceStateChanged();
            }

            return restored;
        }

        public void SaveWorkspaceState(bool preservePendingPersistedState = false)
        {
            composition.SaveWorkspaceState(preservePendingPersistedState);
        }

        public void StopPendingSave()
        {
            composition.StopPendingWorkspaceStateSave();
        }

        public List<string> GetWorkspaceLayerTitles()
        {
            return composition.GetDocumentIds();
        }

        #endregion

        #region Notifications

        private void NotifyWorkspaceStateChanged()
        {
            WorkspaceStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCompositionWorkspaceStateChanged(object sender, EventArgs e)
        {
            NotifyWorkspaceStateChanged();
        }

        #endregion
    }
}
