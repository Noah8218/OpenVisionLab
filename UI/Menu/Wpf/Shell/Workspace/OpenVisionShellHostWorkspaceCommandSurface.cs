using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    public sealed class OpenVisionShellHostWorkspaceCommandSurface
    {
        private readonly OpenVisionShellHostCommandController commandController;
        private readonly OpenVisionShellHostWorkspacePreviewController workspacePreviewController;
        private readonly Action<VISION_MENU> selectTool;
        private readonly Func<VISION_MENU?> sampleFirstStepMenuProvider;
        private readonly Func<string> sampleCounterpartNameProvider;
        private readonly Action applySampleFirstStepParameters;

        internal OpenVisionShellHostWorkspaceCommandSurface(
            OpenVisionShellHostCommandController commandController,
            OpenVisionShellHostWorkspacePreviewController workspacePreviewController,
            Action<VISION_MENU> selectTool = null,
            Func<VISION_MENU?> sampleFirstStepMenuProvider = null,
            Func<string> sampleCounterpartNameProvider = null,
            Action applySampleFirstStepParameters = null)
        {
            this.commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            this.workspacePreviewController = workspacePreviewController ?? throw new ArgumentNullException(nameof(workspacePreviewController));
            this.selectTool = selectTool;
            this.sampleFirstStepMenuProvider = sampleFirstStepMenuProvider;
            this.sampleCounterpartNameProvider = sampleCounterpartNameProvider;
            this.applySampleFirstStepParameters = applySampleFirstStepParameters;

            LoadImageCommand = new RelayCommand(commandController.PromptAndLoadWorkspaceImage);
            OpenSampleCommand = new RelayCommand(commandController.PromptAndOpenRunnableSample, commandController.HasRunnableSample);
            FitImageCommand = new RelayCommand(commandController.FitWorkspaceImage, HasWorkspaceImage);
            SaveImageCommand = new RelayCommand(commandController.PromptAndSaveWorkspaceImage, HasWorkspaceImage);
            OpenThresholdToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Threshold), CanOpenToolAfterImageReady);
            OpenMatchingToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Matching), CanOpenToolAfterImageReady);
            OpenLineToolCommand = new RelayCommand(() => OpenTool(VISION_MENU.Line), CanOpenToolAfterImageReady);
            OpenSamplePipelineCommand = new RelayCommand(OpenPipelineReview, CanOpenSampleNavigation);
            OpenSampleFirstStepCommand = new RelayCommand(OpenSampleFirstStepTool, CanOpenSampleFirstStepTool);
            OpenSampleCounterpartCommand = new RelayCommand(OpenSampleCounterpart, CanOpenSampleCounterpart);
        }

        public ICommand LoadImageCommand { get; }

        public ICommand OpenSampleCommand { get; }

        public ICommand FitImageCommand { get; }

        public ICommand SaveImageCommand { get; }

        public ICommand OpenThresholdToolCommand { get; }

        public ICommand OpenMatchingToolCommand { get; }

        public ICommand OpenLineToolCommand { get; }

        public ICommand OpenSamplePipelineCommand { get; }

        public ICommand OpenSampleFirstStepCommand { get; }

        public ICommand OpenSampleCounterpartCommand { get; }

        public void RefreshCanExecute()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private bool HasWorkspaceImage()
        {
            return workspacePreviewController.HasImage;
        }

        private bool CanOpenSampleNavigation()
        {
            return selectTool != null;
        }

        private bool CanOpenToolAfterImageReady()
        {
            return selectTool != null && HasWorkspaceImage();
        }

        private void OpenTool(VISION_MENU menu)
        {
            selectTool?.Invoke(menu);
        }

        private bool CanOpenSampleFirstStepTool()
        {
            return selectTool != null && sampleFirstStepMenuProvider?.Invoke().HasValue == true;
        }

        private bool CanOpenSampleCounterpart()
        {
            return !string.IsNullOrWhiteSpace(sampleCounterpartNameProvider?.Invoke());
        }

        private void OpenPipelineReview()
        {
            selectTool?.Invoke(VISION_MENU.Pipeline);
        }

        private void OpenSampleFirstStepTool()
        {
            VISION_MENU? menu = sampleFirstStepMenuProvider?.Invoke();
            if (menu.HasValue)
            {
                selectTool?.Invoke(menu.Value);
                applySampleFirstStepParameters?.Invoke();
            }
        }

        private void OpenSampleCounterpart()
        {
            string sampleName = sampleCounterpartNameProvider?.Invoke();
            if (!string.IsNullOrWhiteSpace(sampleName))
            {
                commandController.OpenRunnableSampleByName(sampleName);
            }
        }
    }
}
