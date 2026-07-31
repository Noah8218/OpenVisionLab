using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipePendingEditTransitionController
    {
        private readonly Func<OpenVisionRecipePendingEditRequest, OpenVisionRecipePendingEditDecision> decide;
        private readonly Func<bool> apply;
        private readonly Action discard;

        internal OpenVisionRecipePendingEditTransitionController(
            Func<OpenVisionRecipePendingEditRequest, OpenVisionRecipePendingEditDecision> decide,
            Func<bool> apply,
            Action discard)
        {
            this.decide = decide ?? throw new ArgumentNullException(nameof(decide));
            this.apply = apply ?? throw new ArgumentNullException(nameof(apply));
            this.discard = discard ?? throw new ArgumentNullException(nameof(discard));
        }

        internal bool TryLeave(
            bool isDirty,
            OpenVisionRecipePendingEditRequest request)
        {
            if (!isDirty)
            {
                return true;
            }

            switch (decide(request ?? new OpenVisionRecipePendingEditRequest()))
            {
                case OpenVisionRecipePendingEditDecision.ApplyAndContinue:
                    return apply();
                case OpenVisionRecipePendingEditDecision.Discard:
                    discard();
                    return true;
                default:
                    return false;
            }
        }
    }

    public sealed class OpenVisionRecipePendingEditRequest
    {
        public OpenVisionRecipePendingEditTransitionKind Kind { get; set; }

        public string RecipeName { get; set; } = string.Empty;

        public string PipelineName { get; set; } = string.Empty;

        public string StepName { get; set; } = string.Empty;

        public string TargetName { get; set; } = string.Empty;
    }

    public enum OpenVisionRecipePendingEditTransitionKind
    {
        Step,
        Pipeline,
        Recipe,
        RecipeManagerClose
    }

    public enum OpenVisionRecipePendingEditDecision
    {
        Cancel,
        ApplyAndContinue,
        Discard
    }
}
