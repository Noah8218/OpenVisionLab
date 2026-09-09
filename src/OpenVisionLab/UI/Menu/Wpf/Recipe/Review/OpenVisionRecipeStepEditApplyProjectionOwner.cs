namespace OpenVisionLab
{
    // Owns Step Edit apply-result text projection without owning mutable UI state.
    internal sealed class OpenVisionRecipeStepEditApplyProjectionOwner
    {
        internal OpenVisionRecipeStepEditApplyProjection ProjectFailure(
            OpenVisionRecipeStepEditApplyResult applyResult)
        {
            string shellStatus = string.Empty;
            if (applyResult.IsRoundTripValidationFailure)
            {
                shellStatus = applyResult.RestoreSucceeded
                    ? OpenVisionRecipeText.Local(
                        "Step XML 적용 실패 — 기존 저장 상태 복원",
                        "Step XML apply failed — previous saved state restored")
                    : OpenVisionRecipeText.Local(
                        "Step XML 적용 실패 — 복원 오류 확인 필요",
                        "Step XML apply failed — review the restore error");
            }

            return OpenVisionRecipeStepEditApplyProjection.Failure(
                applyResult.Message,
                shellStatus);
        }

        internal OpenVisionRecipeStepEditApplyProjection ProjectSuccess(
            string pipelineName,
            int selectedIndex,
            OpenVisionRecipePipelineStepPreview selectedStep,
            string validationMessage,
            bool rerunValidationSet)
        {
            string status = OpenVisionRecipeText.Local("XML 반영 완료: ", "Applied to XML: ")
                + pipelineName
                + " / Step "
                + selectedIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " / "
                + validationMessage;
            string correctedOutputReview =
                OpenVisionRecipePipelineStepReviewPresenter.BuildCorrectedOutputAppliedText(
                    selectedStep,
                    pipelineName,
                    selectedIndex,
                    validationMessage,
                    rerunValidationSet);
            return OpenVisionRecipeStepEditApplyProjection.Success(
                status,
                OpenVisionRecipeText.Local("Step XML 반영 완료", "Step XML apply complete"),
                correctedOutputReview);
        }
    }

    internal sealed class OpenVisionRecipeStepEditApplyProjection
    {
        private OpenVisionRecipeStepEditApplyProjection(
            bool succeeded,
            string selectedStepEditStatusText,
            string shellStatusText,
            string correctedOutputReviewText)
        {
            Succeeded = succeeded;
            SelectedStepEditStatusText = selectedStepEditStatusText ?? string.Empty;
            ShellStatusText = shellStatusText ?? string.Empty;
            CorrectedOutputReviewText = correctedOutputReviewText ?? string.Empty;
        }

        internal bool Succeeded { get; }

        internal string SelectedStepEditStatusText { get; }

        internal string ShellStatusText { get; }

        internal string CorrectedOutputReviewText { get; }

        internal static OpenVisionRecipeStepEditApplyProjection Failure(
            string selectedStepEditStatusText,
            string shellStatusText)
        {
            return new OpenVisionRecipeStepEditApplyProjection(
                false,
                selectedStepEditStatusText,
                shellStatusText,
                string.Empty);
        }

        internal static OpenVisionRecipeStepEditApplyProjection Success(
            string selectedStepEditStatusText,
            string shellStatusText,
            string correctedOutputReviewText)
        {
            return new OpenVisionRecipeStepEditApplyProjection(
                true,
                selectedStepEditStatusText,
                shellStatusText,
                correctedOutputReviewText);
        }
    }
}
