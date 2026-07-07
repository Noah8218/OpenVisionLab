using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostChromeCommandSurface
    {
        private readonly Action toggleToolRail;
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;

        internal OpenVisionShellHostChromeCommandSurface(
            Action toggleToolRail,
            OpenVisionShellHostCommandController commandController,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            OpenVisionShellHostToolWindowController toolWindowController)
        {
            this.toggleToolRail = toggleToolRail ?? throw new ArgumentNullException(nameof(toggleToolRail));
            this.commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.toolWindowController = toolWindowController ?? throw new ArgumentNullException(nameof(toolWindowController));

            ToggleToolRailCommand = new RelayCommand(this.toggleToolRail);
            OpenLearnCommand = new RelayCommand(this.commandController.OpenLearn);
            OpenTutorialCommand = new RelayCommand(this.commandController.OpenTutorial);
            FloatDockedToolCommand = new RelayCommand(FloatDockedTool);
            CloseDockedToolCommand = new RelayCommand(CloseDockedTool);
        }

        public ICommand ToggleToolRailCommand { get; }

        public ICommand OpenLearnCommand { get; }

        public ICommand OpenTutorialCommand { get; }

        public ICommand FloatDockedToolCommand { get; }

        public ICommand CloseDockedToolCommand { get; }

        public void RefreshCanExecute()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void FloatDockedTool()
        {
            toolWindowLifecycleController.FloatDockedTool(toolWindowController.ShowWpfToolWindow);
            RefreshCanExecute();
        }

        private void CloseDockedTool()
        {
            toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
            RefreshCanExecute();
        }
    }
}
