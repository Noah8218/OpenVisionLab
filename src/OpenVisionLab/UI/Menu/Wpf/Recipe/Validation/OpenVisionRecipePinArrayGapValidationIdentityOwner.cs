using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.IO;

namespace OpenVisionLab
{
    // Owns the selected Pipeline XML read and PinArrayGap identity workflow without WPF state.
    internal sealed class OpenVisionRecipePinArrayGapValidationIdentityOwner
    {
        internal OpenVisionRecipePinArrayGapValidationIdentityResult Freeze(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeValidationSetOption train,
            OpenVisionRecipeValidationSetOption validation,
            OpenVisionRecipeValidationSetOption test)
        {
            if (!TryReadPipelineXml(recipeName, pipelineName, out string pipelineXmlText, out string error)
                || !OpenVisionRecipePinArrayGapValidationRecordStorage.TrySave(
                    recipeName,
                    pipelineXmlText,
                    train,
                    validation,
                    test,
                    out OpenVisionRecipePinArrayGapValidationRecord record,
                    out error))
            {
                return OpenVisionRecipePinArrayGapValidationIdentityResult.Failure(error);
            }

            return OpenVisionRecipePinArrayGapValidationIdentityResult.Success(record, matches: true);
        }

        internal OpenVisionRecipePinArrayGapValidationIdentityResult Evaluate(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeValidationSetOption train,
            OpenVisionRecipeValidationSetOption validation,
            OpenVisionRecipeValidationSetOption test)
        {
            if (!TryReadPipelineXml(recipeName, pipelineName, out string pipelineXmlText, out string error)
                || !OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad(
                    recipeName,
                    out OpenVisionRecipePinArrayGapValidationRecord record,
                    out error))
            {
                return OpenVisionRecipePinArrayGapValidationIdentityResult.Failure(error);
            }

            if (!OpenVisionRecipePinArrayGapValidationRecordStorage.TryMatchesCurrent(
                    recipeName,
                    pipelineXmlText,
                    train,
                    validation,
                    test,
                    record,
                    out bool matches,
                    out error))
            {
                return OpenVisionRecipePinArrayGapValidationIdentityResult.Failure(error);
            }

            return OpenVisionRecipePinArrayGapValidationIdentityResult.Success(record, matches);
        }

        internal bool TryGetFrozenSelectionNames(
            string recipeName,
            out string trainName,
            out string validationName,
            out string testName)
        {
            trainName = string.Empty;
            validationName = string.Empty;
            testName = string.Empty;
            if (!OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad(
                    recipeName,
                    out OpenVisionRecipePinArrayGapValidationRecord record,
                    out _))
            {
                return false;
            }

            trainName = record.Train?.SetName ?? string.Empty;
            validationName = record.Validation?.SetName ?? string.Empty;
            testName = record.Test?.SetName ?? string.Empty;
            return true;
        }

        private static bool TryReadPipelineXml(
            string recipeName,
            string pipelineName,
            out string pipelineXmlText,
            out string error)
        {
            pipelineXmlText = string.Empty;
            if (string.IsNullOrWhiteSpace(pipelineName))
            {
                error = "Select the imported PinArrayGap pipeline.";
                return false;
            }

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            if (!File.Exists(path))
            {
                error = "Selected pipeline XML was not found: " + path;
                return false;
            }

            try
            {
                pipelineXmlText = File.ReadAllText(path);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.GetBaseException().Message;
                return false;
            }
        }
    }

    internal sealed class OpenVisionRecipePinArrayGapValidationIdentityResult
    {
        private OpenVisionRecipePinArrayGapValidationIdentityResult(
            bool succeeded,
            bool matches,
            OpenVisionRecipePinArrayGapValidationRecord record,
            string error)
        {
            Succeeded = succeeded;
            Matches = matches;
            Record = record;
            Error = error ?? string.Empty;
        }

        internal bool Succeeded { get; }

        internal bool Matches { get; }

        internal OpenVisionRecipePinArrayGapValidationRecord Record { get; }

        internal string Error { get; }

        internal static OpenVisionRecipePinArrayGapValidationIdentityResult Success(
            OpenVisionRecipePinArrayGapValidationRecord record,
            bool matches)
        {
            return new OpenVisionRecipePinArrayGapValidationIdentityResult(
                true,
                matches,
                record,
                string.Empty);
        }

        internal static OpenVisionRecipePinArrayGapValidationIdentityResult Failure(string error)
        {
            return new OpenVisionRecipePinArrayGapValidationIdentityResult(
                false,
                false,
                null,
                error);
        }
    }
}
