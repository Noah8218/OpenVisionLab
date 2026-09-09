using OpenVisionLab.Vision2D.Pipeline;
using System;

namespace OpenVisionLab
{
    // Owns the persisted Step Edit transaction without owning WPF state or Shell presentation.
    internal sealed class OpenVisionRecipeStepEditApplyOwner
    {
        private readonly Action<string, VisionPipeline> savePipeline;
        private readonly Func<string, VisionPipeline, OpenVisionRecipeRoundTripValidationResult> validateRoundTrip;
        private bool failNextSaveForTest;
        private bool failNextRoundTripValidationForTest;

        internal OpenVisionRecipeStepEditApplyOwner(
            Func<string, VisionPipeline, OpenVisionRecipeRoundTripValidationResult> validateRoundTrip = null,
            Action<string, VisionPipeline> savePipeline = null)
        {
            this.savePipeline = savePipeline ?? VisionPipelineStorage.Save;
            this.validateRoundTrip = validateRoundTrip ?? ValidateRoundTrip;
        }

        internal OpenVisionRecipeStepEditApplyResult Apply(
            string recipeName,
            string pipelineName,
            VisionPipeline pipeline,
            VisionPipelineStep step,
            object editObject)
        {
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!VisionPipelineStorage.TryLoadFromFile(
                pipelinePath,
                out VisionPipeline originalPipeline,
                out string originalLoadMessage))
            {
                return OpenVisionRecipeStepEditApplyResult.Failure(
                    OpenVisionRecipeText.Local(
                        "기존 XML 백업을 읽지 못해 적용을 중단했습니다: ",
                        "Apply was stopped because the existing XML backup could not be read: ")
                    + originalLoadMessage);
            }

            if (!VisionPipelineStepPropertyMapper.ApplyProperty(step, editObject))
            {
                return OpenVisionRecipeStepEditApplyResult.Failure(
                    OpenVisionRecipeText.Local(
                        "이 Step 파라미터는 XML로 반영할 수 없습니다.",
                        "This step property set cannot be applied to XML."));
            }

            try
            {
                pipeline.Name = pipelineName;
                if (failNextSaveForTest)
                {
                    failNextSaveForTest = false;
                    throw new InvalidOperationException(
                        "Forced XML save failure for current-build smoke.");
                }

                savePipeline(recipeName, pipeline);
            }
            catch (Exception ex)
            {
                string saveFailure = OpenVisionRecipeText.Local("XML 저장 실패: ", "XML save failed: ")
                    + ex.GetBaseException().Message;
                TryRestorePipelineAfterFailedApply(
                    recipeName,
                    originalPipeline,
                    out string restoreMessage);
                return OpenVisionRecipeStepEditApplyResult.Failure(
                    saveFailure + Environment.NewLine + restoreMessage);
            }

            OpenVisionRecipeRoundTripValidationResult validation = failNextRoundTripValidationForTest
                ? ConsumeForcedRoundTripValidationFailure()
                : validateRoundTrip(recipeName, pipeline)
                    ?? new OpenVisionRecipeRoundTripValidationResult
                    {
                        Succeeded = false,
                        Message = OpenVisionRecipeText.Local(
                            "왕복 검증 결과가 없습니다.",
                            "No round-trip validation result was returned.")
                    };
            string validationMessage = validation.Message ?? string.Empty;
            if (validation.Succeeded)
            {
                return OpenVisionRecipeStepEditApplyResult.Success(validationMessage);
            }

            bool restored = TryRestorePipelineAfterFailedApply(
                recipeName,
                originalPipeline,
                out string validationRestoreMessage);
            return OpenVisionRecipeStepEditApplyResult.RoundTripFailure(
                OpenVisionRecipeText.Local(
                    "XML 왕복 검증에 실패하여 전환을 중단했습니다: ",
                    "Transition was stopped because XML round-trip validation failed: ")
                + validationMessage
                + Environment.NewLine
                + validationRestoreMessage,
                restored);
        }

        internal void FailNextSaveForTest()
        {
            failNextSaveForTest = true;
        }

        internal void FailNextRoundTripValidationForTest()
        {
            failNextRoundTripValidationForTest = true;
        }

        private OpenVisionRecipeRoundTripValidationResult ConsumeForcedRoundTripValidationFailure()
        {
            failNextRoundTripValidationForTest = false;
            return new OpenVisionRecipeRoundTripValidationResult
            {
                Succeeded = false,
                Message = "Forced round-trip validation failure for current-build smoke."
            };
        }

        private static OpenVisionRecipeRoundTripValidationResult ValidateRoundTrip(
            string recipeName,
            VisionPipeline pipeline)
        {
            bool succeeded = VisionPipelineStorage.TryValidateRoundTrip(
                recipeName,
                pipeline,
                out string message);
            return new OpenVisionRecipeRoundTripValidationResult
            {
                Succeeded = succeeded,
                Message = message
            };
        }

        private static bool TryRestorePipelineAfterFailedApply(
            string recipeName,
            VisionPipeline originalPipeline,
            out string message)
        {
            try
            {
                VisionPipelineStorage.Save(recipeName, originalPipeline);
                if (VisionPipelineStorage.TryValidateRoundTrip(
                    recipeName,
                    originalPipeline,
                    out string validationMessage))
                {
                    message = OpenVisionRecipeText.Local(
                        "기존 저장 상태를 복원했습니다. ",
                        "The previous saved state was restored. ")
                        + validationMessage;
                    return true;
                }

                message = OpenVisionRecipeText.Local(
                    "기존 XML을 다시 저장했지만 복원 검증에 실패했습니다: ",
                    "The previous XML was saved again, but restore validation failed: ")
                    + validationMessage;
                return false;
            }
            catch (Exception ex)
            {
                message = OpenVisionRecipeText.Local(
                    "기존 저장 상태 복원 실패: ",
                    "Failed to restore the previous saved state: ")
                    + ex.GetBaseException().Message;
                return false;
            }
        }
    }

    internal sealed class OpenVisionRecipeStepEditApplyResult
    {
        private OpenVisionRecipeStepEditApplyResult(
            bool succeeded,
            string message,
            string validationMessage,
            bool isRoundTripValidationFailure,
            bool restoreSucceeded)
        {
            Succeeded = succeeded;
            Message = message ?? string.Empty;
            ValidationMessage = validationMessage ?? string.Empty;
            IsRoundTripValidationFailure = isRoundTripValidationFailure;
            RestoreSucceeded = restoreSucceeded;
        }

        internal bool Succeeded { get; }

        internal string Message { get; }

        internal string ValidationMessage { get; }

        internal bool IsRoundTripValidationFailure { get; }

        internal bool RestoreSucceeded { get; }

        internal static OpenVisionRecipeStepEditApplyResult Success(string validationMessage)
        {
            return new OpenVisionRecipeStepEditApplyResult(
                true,
                string.Empty,
                validationMessage,
                false,
                false);
        }

        internal static OpenVisionRecipeStepEditApplyResult Failure(string message)
        {
            return new OpenVisionRecipeStepEditApplyResult(
                false,
                message,
                string.Empty,
                false,
                false);
        }

        internal static OpenVisionRecipeStepEditApplyResult RoundTripFailure(
            string message,
            bool restoreSucceeded)
        {
            return new OpenVisionRecipeStepEditApplyResult(
                false,
                message,
                string.Empty,
                true,
                restoreSucceeded);
        }
    }
}
