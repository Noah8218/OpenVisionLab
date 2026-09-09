namespace OpenVisionLab
{
    // Owns Review bundle dry-run result-to-status projection without owning review state.
    internal sealed class OpenVisionRecipeReviewBundleDryRunProjectionOwner
    {
        internal OpenVisionRecipeReviewBundleDryRunProjection Project(
            bool inspectionSucceeded,
            bool xmlReady)
        {
            if (!inspectionSucceeded)
            {
                return OpenVisionRecipeReviewBundleDryRunProjection.Failure(
                    OpenVisionRecipeText.Local(
                        "검토 번들 dry-run NG. 가져오지 않았습니다.",
                        "Review bundle dry-run NG. Nothing was imported."));
            }

            return OpenVisionRecipeReviewBundleDryRunProjection.Success(
                xmlReady
                    ? OpenVisionRecipeText.Local(
                        "검토 번들 dry-run OK. XML은 검토 화면에만 로드했으며 가져오기/Preview/Run은 실행하지 않았습니다.",
                        "Review bundle dry-run OK. XML was loaded for review only; import, Preview, and Run were not executed.")
                    : OpenVisionRecipeText.Local(
                        "검토 번들 무결성은 OK지만 XML/의존성 검토는 NG입니다. 가져오기/Preview/Run은 실행하지 않았습니다.",
                        "Review bundle integrity is OK, but XML/dependency review is NG. Import, Preview, and Run were not executed."));
        }
    }

    internal sealed class OpenVisionRecipeReviewBundleDryRunProjection
    {
        private OpenVisionRecipeReviewBundleDryRunProjection(
            bool succeeded,
            string statusText)
        {
            Succeeded = succeeded;
            StatusText = statusText ?? string.Empty;
        }

        internal bool Succeeded { get; }

        internal string StatusText { get; }

        internal static OpenVisionRecipeReviewBundleDryRunProjection Success(
            string statusText)
        {
            return new OpenVisionRecipeReviewBundleDryRunProjection(
                true,
                statusText);
        }

        internal static OpenVisionRecipeReviewBundleDryRunProjection Failure(
            string statusText)
        {
            return new OpenVisionRecipeReviewBundleDryRunProjection(
                false,
                statusText);
        }
    }
}
