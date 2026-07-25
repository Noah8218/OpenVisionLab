using OpenVisionLab.Mvvm;
using System;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostLayerCommandSurface : ObservableObject
    {
        private readonly OpenVisionShellHostLayerSelectionController selectionController;
        private readonly OpenVisionShellHostLayerViewerController viewerController;
        private readonly OpenVisionShellHostLayerManagementController managementController;
        private readonly IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace;
        private readonly Func<string> workspaceLayerTitleProvider;

        internal OpenVisionShellHostLayerCommandSurface(
            OpenVisionShellHostLayerSelectionController selectionController,
            OpenVisionShellHostLayerViewerController viewerController,
            OpenVisionShellHostLayerManagementController managementController,
            IOpenVisionDockedLayerWorkspaceCommands dockedLayerWorkspace,
            Func<string> workspaceLayerTitleProvider)
        {
            this.selectionController = selectionController ?? throw new ArgumentNullException(nameof(selectionController));
            this.viewerController = viewerController ?? throw new ArgumentNullException(nameof(viewerController));
            this.managementController = managementController ?? throw new ArgumentNullException(nameof(managementController));
            this.dockedLayerWorkspace = dockedLayerWorkspace ?? throw new ArgumentNullException(nameof(dockedLayerWorkspace));
            this.workspaceLayerTitleProvider = workspaceLayerTitleProvider ?? throw new ArgumentNullException(nameof(workspaceLayerTitleProvider));

            HandleSelectedLayerChangedCommand = new RelayCommand<object>(_ => HandleSelectedLayerChanged());
            CreateLayerCommand = new RelayCommand(CreateLayer);
            LoadImageIntoSelectedLayerCommand = new RelayCommand(LoadImageIntoSelectedLayer, CanLoadImageIntoSelectedLayer);
            LoadImageIntoCurrentLayerCommand = new RelayCommand(LoadImageIntoCurrentLayer, CanLoadImageIntoCurrentLayer);
            RenameSelectedLayerCommand = new RelayCommand<object>(RenameSelectedLayer, _ => CanRenameSelectedLayer());
            RenameCurrentLayerCommand = new RelayCommand<object>(RenameCurrentLayer, _ => CanRenameCurrentLayer());
            DeleteSelectedLayerCommand = new RelayCommand(DeleteSelectedLayer, CanDeleteSelectedLayer);
            DeleteCurrentLayerCommand = new RelayCommand(DeleteCurrentLayer, CanDeleteCurrentLayer);
            OpenSelectedLayerWindowCommand = new RelayCommand(OpenSelectedLayerWindow, CanOpenSelectedLayerWindow);
            OpenCurrentLayerWindowCommand = new RelayCommand(OpenCurrentLayerWindow, CanOpenCurrentLayerWindow);
            DockSelectedLayerCommand = new RelayCommand(DockSelectedLayer, CanDockSelectedLayer);
            DockCurrentLayerCommand = new RelayCommand(DockCurrentLayer, CanDockCurrentLayer);
            ClearDockedLayersCommand = new RelayCommand(ClearDockedLayers, () => dockedLayerWorkspace.HasLayers);

            dockedLayerWorkspace.WorkspaceStateChanged += OnWorkspaceStateChanged;
        }

        public ICommand HandleSelectedLayerChangedCommand { get; }

        public ICommand CreateLayerCommand { get; }

        public ICommand LoadImageIntoSelectedLayerCommand { get; }

        public ICommand LoadImageIntoCurrentLayerCommand { get; }

        public ICommand RenameSelectedLayerCommand { get; }

        public ICommand RenameCurrentLayerCommand { get; }

        public ICommand DeleteSelectedLayerCommand { get; }

        public ICommand DeleteCurrentLayerCommand { get; }

        public ICommand OpenSelectedLayerWindowCommand { get; }

        public ICommand OpenCurrentLayerWindowCommand { get; }

        public ICommand DockSelectedLayerCommand { get; }

        public ICommand DockCurrentLayerCommand { get; }

        public ICommand ClearDockedLayersCommand { get; }

        public string OpenLayerWindowText => OpenVisionLanguageService.T("Shell.OpenLayerWindow");

        public string CreateLayerText => OpenVisionLanguageService.T("Shell.CreateLayer");

        public string CreateLayerShortText => OpenVisionLanguageService.T("Shell.CreateLayerShort");

        public string LoadImageIntoLayerText => OpenVisionLanguageService.T("Shell.LoadImageIntoLayer");

        public string LoadImageIntoLayerShortText => OpenVisionLanguageService.T("Shell.LoadImageIntoLayerShort");

        public string RenameLayerText => OpenVisionLanguageService.T("Shell.RenameLayer");

        public string RenameLayerShortText => OpenVisionLanguageService.T("Shell.RenameLayerShort");

        public string DeleteLayerText => OpenVisionLanguageService.T("Shell.DeleteLayer");

        public string DeleteLayerShortText => OpenVisionLanguageService.T("Shell.DeleteLayerShort");

        public string DockLayerText => OpenVisionLanguageService.T("Shell.DockLayer");

        public string ClearDockedLayersText => OpenVisionLanguageService.T("Shell.ClearDockedLayers");

        public void RefreshLocalization()
        {
            OnPropertyChanged(nameof(OpenLayerWindowText));
            OnPropertyChanged(nameof(CreateLayerText));
            OnPropertyChanged(nameof(CreateLayerShortText));
            OnPropertyChanged(nameof(LoadImageIntoLayerText));
            OnPropertyChanged(nameof(LoadImageIntoLayerShortText));
            OnPropertyChanged(nameof(RenameLayerText));
            OnPropertyChanged(nameof(RenameLayerShortText));
            OnPropertyChanged(nameof(DeleteLayerText));
            OnPropertyChanged(nameof(DeleteLayerShortText));
            OnPropertyChanged(nameof(DockLayerText));
            OnPropertyChanged(nameof(ClearDockedLayersText));
        }

        public void RefreshCanExecute()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool DockLayerDocument(string layerTitle)
        {
            return DockLayer(layerTitle);
        }

        public bool CanDockLayerDocument(string layerTitle)
        {
            return CanDockLayer(layerTitle);
        }

        private void HandleSelectedLayerChanged()
        {
            selectionController.HandleSelectionChanged();
            RefreshCanExecute();
        }

        private void CreateLayer()
        {
            managementController.CreateLayer();
            RefreshCanExecute();
        }

        private void LoadImageIntoSelectedLayer()
        {
            managementController.PromptAndLoadImageIntoLayer(selectionController.GetSelectedLayerTitle());
            RefreshCanExecute();
        }

        private bool CanLoadImageIntoSelectedLayer()
        {
            return managementController.CanLoadImageIntoLayer(selectionController.GetSelectedLayerTitle());
        }

        private void LoadImageIntoCurrentLayer()
        {
            managementController.PromptAndLoadImageIntoLayer(workspaceLayerTitleProvider());
            RefreshCanExecute();
        }

        private bool CanLoadImageIntoCurrentLayer()
        {
            return managementController.CanLoadImageIntoLayer(workspaceLayerTitleProvider());
        }

        private void RenameSelectedLayer(object newLayerTitle)
        {
            managementController.RenameLayer(
                selectionController.GetSelectedLayerTitle(),
                Convert.ToString(newLayerTitle, System.Globalization.CultureInfo.CurrentCulture));
            RefreshCanExecute();
        }

        private bool CanRenameSelectedLayer()
        {
            string selectedLayer = selectionController.GetSelectedLayerTitle();
            return managementController.CanDeleteLayer(selectedLayer);
        }

        private void RenameCurrentLayer(object newLayerTitle)
        {
            managementController.RenameLayer(
                workspaceLayerTitleProvider(),
                Convert.ToString(newLayerTitle, System.Globalization.CultureInfo.CurrentCulture));
            RefreshCanExecute();
        }

        private bool CanRenameCurrentLayer()
        {
            return managementController.CanDeleteLayer(workspaceLayerTitleProvider());
        }

        private void DeleteSelectedLayer()
        {
            managementController.DeleteLayer(selectionController.GetSelectedLayerTitle());
            RefreshCanExecute();
        }

        private bool CanDeleteSelectedLayer()
        {
            return managementController.CanDeleteLayer(selectionController.GetSelectedLayerTitle());
        }

        private void DeleteCurrentLayer()
        {
            managementController.DeleteLayer(workspaceLayerTitleProvider());
            RefreshCanExecute();
        }

        private bool CanDeleteCurrentLayer()
        {
            return managementController.CanDeleteLayer(workspaceLayerTitleProvider());
        }

        private void OpenSelectedLayerWindow()
        {
            OpenLayerWindow(selectionController.GetSelectedLayerTitle());
        }

        private bool CanOpenSelectedLayerWindow()
        {
            return CanOpenLayerWindow(selectionController.GetSelectedLayerTitle());
        }

        private void OpenCurrentLayerWindow()
        {
            OpenLayerWindow(workspaceLayerTitleProvider());
        }

        private bool CanOpenCurrentLayerWindow()
        {
            return CanOpenLayerWindow(workspaceLayerTitleProvider());
        }

        private void DockSelectedLayer()
        {
            DockLayer(selectionController.GetSelectedLayerTitle());
        }

        private bool CanDockSelectedLayer()
        {
            return CanDockLayer(selectionController.GetSelectedLayerTitle());
        }

        private void DockCurrentLayer()
        {
            DockLayer(workspaceLayerTitleProvider());
        }

        private bool CanDockCurrentLayer()
        {
            return CanDockLayer(workspaceLayerTitleProvider());
        }

        private void ClearDockedLayers()
        {
            dockedLayerWorkspace.ClearDockedLayerDocuments();
            RefreshCanExecute();
        }

        private bool OpenLayerWindow(string layerTitle)
        {
            return viewerController.Open(layerTitle);
        }

        private bool CanOpenLayerWindow(string layerTitle)
        {
            return viewerController.CanOpen(layerTitle);
        }

        private bool DockLayer(string layerTitle)
        {
            bool docked = dockedLayerWorkspace.DockLayerDocument(layerTitle);
            RefreshCanExecute();
            return docked;
        }

        private bool CanDockLayer(string layerTitle)
        {
            return CanOpenLayerWindow(layerTitle)
                && !dockedLayerWorkspace.LayerTitles.Any(title => string.Equals(title, layerTitle, StringComparison.OrdinalIgnoreCase));
        }

        private void OnWorkspaceStateChanged(object sender, EventArgs e)
        {
            RefreshCanExecute();
        }
    }
}
