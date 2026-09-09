using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    // Owns preview-list Step matching, adjacent navigation, and identity checks for Shell Step Edit.
    internal sealed class OpenVisionRecipeStepPreviewNavigationOwner
    {
        internal OpenVisionRecipePipelineStepPreview Find(
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps,
            string stepReference)
        {
            if (steps == null || steps.Count == 0 || string.IsNullOrWhiteSpace(stepReference))
            {
                return null;
            }

            string needle = Normalize(stepReference);
            if (string.IsNullOrWhiteSpace(needle))
            {
                return null;
            }

            return steps.FirstOrDefault(step => Matches(step, needle));
        }

        internal OpenVisionRecipePipelineStepPreview GetByOffset(
            IReadOnlyList<OpenVisionRecipePipelineStepPreview> steps,
            OpenVisionRecipePipelineStepPreview selected,
            int offset)
        {
            if (steps == null || steps.Count == 0 || selected == null)
            {
                return null;
            }

            int selectedPosition = -1;
            for (int i = 0; i < steps.Count; i++)
            {
                OpenVisionRecipePipelineStepPreview candidate = steps[i];
                if (candidate != null
                    && (ReferenceEquals(candidate, selected) || candidate.Index == selected.Index))
                {
                    selectedPosition = i;
                    break;
                }
            }

            if (selectedPosition < 0)
            {
                return null;
            }

            int targetPosition = selectedPosition + offset;
            return targetPosition >= 0 && targetPosition < steps.Count
                ? steps[targetPosition]
                : null;
        }

        internal bool AreSame(
            OpenVisionRecipePipelineStepPreview left,
            OpenVisionRecipePipelineStepPreview right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            return left != null
                && right != null
                && left.Index == right.Index
                && string.Equals(left.Name, right.Name, StringComparison.Ordinal)
                && string.Equals(left.ToolType, right.ToolType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.InputLayer, right.InputLayer, StringComparison.OrdinalIgnoreCase)
                && string.Equals(left.OutputLayer, right.OutputLayer, StringComparison.OrdinalIgnoreCase);
        }

        private static bool Matches(OpenVisionRecipePipelineStepPreview step, string needle)
        {
            if (step == null)
            {
                return false;
            }

            if (TryExtractStepIndex(needle, out int stepIndex)
                && step.Index == stepIndex)
            {
                return true;
            }

            string[] candidates =
            {
                step.Name,
                step.ToolType,
                step.OutputLayer,
                step.DisplayText,
                step.DetailText,
                step.FullDetailText
            };

            return candidates
                .Select(Normalize)
                .Any(candidate => !string.IsNullOrWhiteSpace(candidate)
                    && (candidate.Contains(needle) || needle.Contains(candidate)));
        }

        private static bool TryExtractStepIndex(string value, out int stepIndex)
        {
            stepIndex = 0;
            string digits = new string((value ?? string.Empty)
                .SkipWhile(ch => !char.IsDigit(ch))
                .TakeWhile(char.IsDigit)
                .ToArray());
            return !string.IsNullOrWhiteSpace(digits)
                && int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out stepIndex);
        }

        private static string Normalize(string value)
        {
            return new string((value ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(ch => !char.IsWhiteSpace(ch))
                .ToArray());
        }
    }
}
