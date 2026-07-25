using Lib.OpenCV.Pipeline;
using System;
using System.Globalization;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void ActivateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = LocalText("선택된 파이프라인이 없습니다.", "No pipeline selected.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Activate(recipeName, option.PipelineName);
            StatusText = string.Format(CultureInfo.CurrentCulture, LocalText("활성 파이프라인: {0}", "Active pipeline: {0}"), option.PipelineName);
            RefreshPipelineOptions(result.PipelineName);
            refreshAfterSwitch();
        }

        private void DuplicateSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanUseSelectedPipeline())
            {
                StatusText = LocalText("선택된 파이프라인이 없습니다.", "No pipeline selected.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string requestedName = NormalizePipelineName(PipelineEditName);
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Duplicate(
                recipeName,
                option.PipelineName,
                requestedName);
            if (!result.Succeeded)
            {
                StatusText = result.Detail;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = result.Detail;
            RefreshPipelineOptions(result.PipelineName);
            refreshAfterSwitch();
        }

        private void RenameSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanRenameSelectedPipeline())
            {
                StatusText = LocalText("이 파이프라인 이름은 변경할 수 없습니다.", "Cannot rename this pipeline.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string targetName = NormalizePipelineName(PipelineEditName);
            bool wasActive = option.IsActive;
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Rename(
                recipeName,
                option.PipelineName,
                targetName);
            if (!result.Succeeded)
            {
                StatusText = result.Detail;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = result.Detail;
            RefreshPipelineOptions(result.PipelineName);
            if (wasActive)
            {
                refreshAfterSwitch();
            }
        }

        private void DeleteSelectedPipeline()
        {
            OpenVisionRecipePipelineOption option = SelectedPipelineOption;
            if (!CanDeleteSelectedPipeline())
            {
                StatusText = LocalText("이 파이프라인은 삭제할 수 없습니다.", "Cannot delete this pipeline.");
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!confirmDeletePipeline(recipeName, option.PipelineName))
            {
                StatusText = LocalText("파이프라인 삭제가 취소되었습니다.", "Pipeline delete canceled.");
                return;
            }

            bool wasActive = option.IsActive;
            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.Delete(recipeName, option.PipelineName);
            if (!result.Succeeded)
            {
                StatusText = result.Detail;
                RefreshPipelineOptions(option.PipelineName);
                return;
            }

            StatusText = result.Detail;
            RefreshPipelineOptions(result.PipelineName);
            if (wasActive)
            {
                refreshAfterSwitch();
            }
        }

        private void DuplicatePipelineFromSample()
        {
            if (SelectedSampleOption == null)
            {
                StatusText = LocalText("먼저 샘플 파이프라인을 선택하세요.", "Select a sample pipeline first.");
                return;
            }

            DuplicatePipelineFromSampleOption(SelectedSampleOption);
        }

        public bool DuplicatePipelineFromSampleOption(OpenVisionRecipeSampleOption sampleOption)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            if (sampleOption == null || string.IsNullOrWhiteSpace(sampleOption.PipelinePath))
            {
                StatusText = LocalText("샘플 파이프라인을 사용할 수 없습니다.", "Sample pipeline is not available.");
                return false;
            }

            OpenVisionRecipePipelineLifecycleResult result = pipelineLifecycleUseCase.DuplicateFromSample(
                recipeName,
                sampleOption.PipelinePath,
                sampleOption.SampleName);
            string message = result.Detail;
            if (!result.Succeeded)
            {
                StatusText = LocalText("샘플 파이프라인 로드 실패: ", "Sample pipeline load failed: ") + message;
                UpdateSelectedRecipeSummary();
                return false;
            }

            RefreshPipelineOptions(result.PipelineName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("샘플 파이프라인 복제됨: {0}", "Duplicated sample pipeline: {0}"),
                result.PipelineName);
            RefreshOptions();
            refreshAfterSwitch();
            return true;
        }
    }
}
