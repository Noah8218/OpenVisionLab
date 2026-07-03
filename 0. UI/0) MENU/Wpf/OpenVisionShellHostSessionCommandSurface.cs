using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostSessionCommandSurface
    {
        private readonly OpenVisionShellHostSessionController sessionController;
        private readonly Func<Dispatcher> dispatcherProvider;

        internal OpenVisionShellHostSessionCommandSurface(
            OpenVisionShellHostSessionController sessionController,
            Action disposeSession,
            Func<Dispatcher> dispatcherProvider)
        {
            this.sessionController = sessionController ?? throw new ArgumentNullException(nameof(sessionController));
            this.dispatcherProvider = dispatcherProvider ?? throw new ArgumentNullException(nameof(dispatcherProvider));

            if (disposeSession == null)
            {
                throw new ArgumentNullException(nameof(disposeSession));
            }

            LoadedCommand = new RelayCommand(this.sessionController.OnLoaded);
            UnloadedCommand = new RelayCommand(disposeSession);
            WorkspaceCanvasLoadedCommand = new RelayCommand(OnWorkspaceCanvasLoaded);
        }

        public ICommand LoadedCommand { get; }

        public ICommand UnloadedCommand { get; }

        public ICommand WorkspaceCanvasLoadedCommand { get; }

        private void OnWorkspaceCanvasLoaded()
        {
            sessionController.OnWorkspaceCanvasLoaded(dispatcherProvider());
        }
    }
}
