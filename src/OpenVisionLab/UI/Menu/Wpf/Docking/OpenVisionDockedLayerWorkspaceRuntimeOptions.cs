using OpenVisionLab.Core;
using OpenVisionLab.Docking.Controls;
using System;
using System.Collections.Generic;
using DrawingBitmap = System.Drawing.Bitmap;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerWorkspaceRuntimeOptions
    {
        public OpenVisionDockedLayerWorkspaceRuntimeOptions(
            OpenVisionLayerDockWorkspaceView workspaceView,
            IDisplayManager displayManager,
            Func<List<string>> layerTitleProvider,
            Func<string> selectedLayerTitleProvider,
            Func<string, DrawingBitmap, string> statusTextProvider,
            Func<string, bool> canOpenLayer,
            Func<bool> isLoadedProvider,
            Action<OpenVisionDockDocumentRefreshResult> applyRefreshResult,
            Action refreshLayerActions,
            Action<string> activateLayer)
        {
            WorkspaceView = workspaceView ?? throw new ArgumentNullException(nameof(workspaceView));
            DisplayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            LayerTitleProvider = layerTitleProvider ?? throw new ArgumentNullException(nameof(layerTitleProvider));
            SelectedLayerTitleProvider = selectedLayerTitleProvider ?? throw new ArgumentNullException(nameof(selectedLayerTitleProvider));
            StatusTextProvider = statusTextProvider ?? throw new ArgumentNullException(nameof(statusTextProvider));
            CanOpenLayer = canOpenLayer ?? throw new ArgumentNullException(nameof(canOpenLayer));
            IsLoadedProvider = isLoadedProvider ?? throw new ArgumentNullException(nameof(isLoadedProvider));
            ApplyRefreshResult = applyRefreshResult ?? throw new ArgumentNullException(nameof(applyRefreshResult));
            RefreshLayerActions = refreshLayerActions ?? throw new ArgumentNullException(nameof(refreshLayerActions));
            ActivateLayer = activateLayer ?? throw new ArgumentNullException(nameof(activateLayer));
        }

        public OpenVisionLayerDockWorkspaceView WorkspaceView { get; }

        public IDisplayManager DisplayManager { get; }

        public Func<List<string>> LayerTitleProvider { get; }

        public Func<string> SelectedLayerTitleProvider { get; }

        public Func<string, DrawingBitmap, string> StatusTextProvider { get; }

        public Func<string, bool> CanOpenLayer { get; }

        public Func<bool> IsLoadedProvider { get; }

        public Action<OpenVisionDockDocumentRefreshResult> ApplyRefreshResult { get; }

        public Action RefreshLayerActions { get; }

        public Action<string> ActivateLayer { get; }
    }
}
