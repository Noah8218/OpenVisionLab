using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void ImportPipelineXml()
        {
            string path = selectImportPipelineXmlPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("XML 가져오기가 취소되었습니다.", "Import canceled.");
                return;
            }

            ImportPipelineXmlFromPath(path);
        }

        public bool ImportPipelineXmlFromPath(string path)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            if (IsReviewBundlePath(path))
            {
                return LoadReviewBundleForDryRun(path);
            }

            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                Path.GetFileNameWithoutExtension(path)))
            {
                return false;
            }

            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.Import(recipeName, path);
            if (!result.Succeeded)
            {
                StatusText = result.Detail;
                UpdateSelectedRecipeSummary();
                return false;
            }

            RefreshPipelineOptions(result.PipelineName);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("XML 가져오기 완료: {0}", "Imported XML: {0}"),
                result.PipelineName);
            RefreshOptions();
            refreshAfterSwitch();
            return true;
        }

        private void ExportActivePipelineXml()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string suggestedFileName = SanitizePathSegment(activePipelineName) + ".xml";
            string path = selectExportPipelineXmlPath(suggestedFileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("XML 내보내기가 취소되었습니다.", "Export canceled.");
                return;
            }

            ExportActivePipelineXmlToPath(path);
        }

        public bool ExportActivePipelineXmlToPath(string path)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.Export(
                recipeName,
                activePipelineName,
                path);
            if (!result.Succeeded)
            {
                StatusText = result.Detail;
                UpdateSelectedRecipeSummary();
                return false;
            }

            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("XML 내보내기 완료: {0}", "Exported XML: {0}"),
                Path.GetFileName(path));
            UpdateSelectedRecipeSummary();
            return true;
        }

        private void ExportActivePipelineReviewBundle()
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            string suggestedFileName = SanitizePathSegment(activePipelineName) + ".review.zip";
            string path = selectExportReviewBundlePath(suggestedFileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("검토 묶음 내보내기가 취소되었습니다.", "Review bundle export canceled.");
                return;
            }

            ExportActivePipelineReviewBundleToPath(path);
        }

        public bool ExportActivePipelineReviewBundleToPath(string path)
        {
            if (!CanUseSelectedRecipe())
            {
                StatusText = LocalText("선택된 레시피가 없습니다.", "No recipe selected.");
                return false;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string activePipelineName = VisionPipelineStorage.LoadActivePipelineName(
                recipeName,
                VisionPipelineAppendService.DefaultPipelineName);
            OpenVisionRecipePipelineExchangeResult result = pipelineExchangeUseCase.ExportReviewBundle(
                recipeName,
                activePipelineName,
                path,
                BuildRecipeReviewReferences());
            string message = result.Detail;
            if (!result.Succeeded)
            {
                StatusText = LocalText("검토 묶음 내보내기 실패: ", "Review bundle export failed: ") + message;
                return false;
            }

            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("검토 묶음 내보내기 완료: {0}", "Exported review bundle: {0}"),
                Path.GetFileName(result.Detail));
            return true;
        }

        private IReadOnlyList<OpenVisionRecipeReviewReference> BuildRecipeReviewReferences()
        {
            List<OpenVisionRecipeReviewReference> references = new List<OpenVisionRecipeReviewReference>();
            VisionPipelineSampleCatalogItem sample = SelectedSampleOption?.Sample;
            if (sample != null)
            {
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSampleImage",
                    sample.SampleName,
                    sample.ImageFullPath,
                    sample.CatalogSourceId));
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSamplePipeline",
                    sample.SampleName,
                    sample.PipelineFullPath,
                    sample.CatalogSourceId));
                references.Add(new OpenVisionRecipeReviewReference(
                    "SelectedSampleReferenceImage",
                    sample.SampleName,
                    sample.ReferenceImageFullPath,
                    sample.CatalogSourceId));
            }

            if (!string.IsNullOrWhiteSpace(LlmReferenceImagePath))
            {
                references.Add(new OpenVisionRecipeReviewReference(
                    "LlmReferenceImage",
                    Path.GetFileName(LlmReferenceImagePath),
                    LlmReferenceImagePath,
                    "OperatorSelected"));
            }

            return references
                .Where(reference => !string.IsNullOrWhiteSpace(reference.Path))
                .ToList();
        }
    }
}
