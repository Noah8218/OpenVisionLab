using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    internal enum OpenVisionRecipePipelineExchangeOperation
    {
        Import,
        Export,
        ExportReviewBundle
    }

    // Owns Pipeline exchange result-to-status projection without owning UI state.
    internal sealed class OpenVisionRecipePipelineExchangeProjectionOwner
    {
        internal OpenVisionRecipePipelineExchangeProjection Project(
            OpenVisionRecipePipelineExchangeOperation operation,
            OpenVisionRecipePipelineExchangeResult result)
        {
            if (!result.Succeeded)
            {
                return OpenVisionRecipePipelineExchangeProjection.Failure(
                    BuildFailureStatus(operation, result.Detail));
            }

            return OpenVisionRecipePipelineExchangeProjection.Success(
                result.PipelineName,
                BuildSuccessStatus(operation, result));
        }

        private static string BuildFailureStatus(
            OpenVisionRecipePipelineExchangeOperation operation,
            string detail)
        {
            return operation == OpenVisionRecipePipelineExchangeOperation.ExportReviewBundle
                ? OpenVisionRecipeText.Local(
                    "검토 묶음 내보내기 실패: ",
                    "Review bundle export failed: ") + detail
                : detail;
        }

        private static string BuildSuccessStatus(
            OpenVisionRecipePipelineExchangeOperation operation,
            OpenVisionRecipePipelineExchangeResult result)
        {
            string format = operation switch
            {
                OpenVisionRecipePipelineExchangeOperation.Import =>
                    OpenVisionRecipeText.Local("XML 가져오기 완료: {0}", "Imported XML: {0}"),
                OpenVisionRecipePipelineExchangeOperation.Export =>
                    OpenVisionRecipeText.Local("XML 내보내기 완료: {0}", "Exported XML: {0}"),
                OpenVisionRecipePipelineExchangeOperation.ExportReviewBundle =>
                    OpenVisionRecipeText.Local("검토 묶음 내보내기 완료: {0}", "Exported review bundle: {0}"),
                _ => string.Empty
            };
            string displayValue = operation == OpenVisionRecipePipelineExchangeOperation.Import
                ? result.PipelineName
                : Path.GetFileName(result.Detail);
            return string.IsNullOrEmpty(format)
                ? result.Detail
                : string.Format(
                    CultureInfo.CurrentCulture,
                    format,
                    displayValue);
        }
    }

    internal sealed class OpenVisionRecipePipelineExchangeProjection
    {
        private OpenVisionRecipePipelineExchangeProjection(
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

        internal static OpenVisionRecipePipelineExchangeProjection Success(
            string pipelineName,
            string statusText)
        {
            return new OpenVisionRecipePipelineExchangeProjection(
                true,
                pipelineName,
                statusText);
        }

        internal static OpenVisionRecipePipelineExchangeProjection Failure(
            string statusText)
        {
            return new OpenVisionRecipePipelineExchangeProjection(
                false,
                string.Empty,
                statusText);
        }
    }
}
