using System.Globalization;

namespace OpenVisionLab
{
    internal enum OpenVisionRecipePipelineLifecycleOperation
    {
        Activate,
        Duplicate,
        Rename,
        Delete,
        DuplicateFromSample
    }

    // Owns Pipeline lifecycle result-to-status projection without owning UI state.
    internal sealed class OpenVisionRecipePipelineLifecycleProjectionOwner
    {
        internal OpenVisionRecipePipelineLifecycleProjection Project(
            OpenVisionRecipePipelineLifecycleOperation operation,
            OpenVisionRecipePipelineLifecycleResult result)
        {
            if (!result.Succeeded)
            {
                return OpenVisionRecipePipelineLifecycleProjection.Failure(
                    BuildFailureStatus(operation, result.Detail));
            }

            return OpenVisionRecipePipelineLifecycleProjection.Success(
                result.PipelineName,
                BuildSuccessStatus(operation, result));
        }

        private static string BuildFailureStatus(
            OpenVisionRecipePipelineLifecycleOperation operation,
            string detail)
        {
            return operation == OpenVisionRecipePipelineLifecycleOperation.DuplicateFromSample
                ? OpenVisionRecipeText.Local(
                    "샘플 파이프라인 로드 실패: ",
                    "Sample pipeline load failed: ") + detail
                : detail;
        }

        private static string BuildSuccessStatus(
            OpenVisionRecipePipelineLifecycleOperation operation,
            OpenVisionRecipePipelineLifecycleResult result)
        {
            string format = operation switch
            {
                OpenVisionRecipePipelineLifecycleOperation.Activate =>
                    OpenVisionRecipeText.Local("활성 파이프라인: {0}", "Active pipeline: {0}"),
                OpenVisionRecipePipelineLifecycleOperation.DuplicateFromSample =>
                    OpenVisionRecipeText.Local("샘플 파이프라인 복제됨: {0}", "Duplicated sample pipeline: {0}"),
                _ => string.Empty
            };
            return string.IsNullOrEmpty(format)
                ? result.Detail
                : string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    result.PipelineName);
        }
    }

    internal sealed class OpenVisionRecipePipelineLifecycleProjection
    {
        private OpenVisionRecipePipelineLifecycleProjection(
            bool succeeded,
            string pipelineName,
            string statusText)
        {
            Succeeded = succeeded;
            PipelineName = pipelineName ?? string.Empty;
            StatusText = statusText ?? string.Empty;
        }

        internal bool Succeeded { get; }
        internal string PipelineName { get; }
        internal string StatusText { get; }

        internal static OpenVisionRecipePipelineLifecycleProjection Success(
            string pipelineName,
            string statusText)
        {
            return new OpenVisionRecipePipelineLifecycleProjection(
                true,
                pipelineName,
                statusText);
        }

        internal static OpenVisionRecipePipelineLifecycleProjection Failure(
            string statusText)
        {
            return new OpenVisionRecipePipelineLifecycleProjection(
                false,
                string.Empty,
                statusText);
        }
    }
}
