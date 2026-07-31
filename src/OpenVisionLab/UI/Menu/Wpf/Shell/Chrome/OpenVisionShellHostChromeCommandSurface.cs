using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostChromeCommandSurface
    {
        private readonly Action toggleToolRail;
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController;
        private readonly OpenVisionShellHostToolWindowController toolWindowController;
        private readonly Action<VISION_MENU> openGuidedSetupForTool;

        internal OpenVisionShellHostChromeCommandSurface(
            Action toggleToolRail,
            OpenVisionShellHostCommandController commandController,
            OpenVisionShellHostToolWindowLifecycleController toolWindowLifecycleController,
            OpenVisionShellHostToolWindowController toolWindowController,
            Action<VISION_MENU> openGuidedSetupForTool)
        {
            this.toggleToolRail = toggleToolRail ?? throw new ArgumentNullException(nameof(toggleToolRail));
            this.commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            this.toolWindowLifecycleController = toolWindowLifecycleController ?? throw new ArgumentNullException(nameof(toolWindowLifecycleController));
            this.toolWindowController = toolWindowController ?? throw new ArgumentNullException(nameof(toolWindowController));
            this.openGuidedSetupForTool = openGuidedSetupForTool ?? throw new ArgumentNullException(nameof(openGuidedSetupForTool));

            ToggleToolRailCommand = new RelayCommand(this.toggleToolRail);
            OpenLearnCommand = new RelayCommand(this.commandController.OpenLearn);
            OpenToolLearnCommand = new RelayCommand<object>(parameter =>
            {
                if (parameter is OpenVisionShellNavItem item)
                {
                    this.commandController.OpenLearnForTool(item.Menu);
                }
            });
            OpenToolSamplesCommand = new RelayCommand<object>(parameter =>
            {
                if (parameter is OpenVisionShellNavItem item)
                {
                    this.commandController.OpenSamplesForTool(item.Menu);
                }
            });
            OpenToolGuidedSetupCommand = new RelayCommand<object>(parameter =>
            {
                if (parameter is OpenVisionShellNavItem item && item.HasGuidedSetup)
                {
                    this.openGuidedSetupForTool(item.Menu);
                }
            });
            OpenTutorialCommand = new RelayCommand(this.commandController.OpenTutorial);
            FloatDockedToolCommand = new RelayCommand(FloatDockedTool);
            CloseDockedToolCommand = new RelayCommand(CloseDockedTool);
        }

        public ICommand ToggleToolRailCommand { get; }

        public ICommand OpenLearnCommand { get; }

        public ICommand OpenToolLearnCommand { get; }

        public ICommand OpenToolSamplesCommand { get; }

        public ICommand OpenToolGuidedSetupCommand { get; }

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
