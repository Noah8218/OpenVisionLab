using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Vision2D;
using System;

namespace OpenVisionLab
{
    /// <summary>
    /// Coordinates the Threshold teaching-suggestion workflow without owning WPF controls.
    /// The session owns suggestion/Undo policy; this controller only translates the workflow
    /// into view callbacks and the existing parameter interaction path.
    /// </summary>
    internal sealed class ThresholdToolSuggestionController
    {
        private readonly Func<VisionToolSignalEvidence> evidenceProvider;
        private readonly Func<ThresholdToolProperty> propertyProvider;
        private readonly VisionToolThresholdInteractionController interactionController;
        private readonly Action<bool> setPanelVisible;
        private readonly Action<bool> setUseEnabled;
        private readonly Action<bool> setUndoEnabled;
        private readonly Action<string> setStatus;
        private readonly Action<VisionToolSignalMarker> setAdvisoryMarker;
        private readonly VisionToolThresholdSuggestionSession session = new VisionToolThresholdSuggestionSession();

        public ThresholdToolSuggestionController(
            Func<VisionToolSignalEvidence> evidenceProvider,
            Func<ThresholdToolProperty> propertyProvider,
            VisionToolThresholdInteractionController interactionController,
            Action<bool> setPanelVisible,
            Action<bool> setUseEnabled,
            Action<bool> setUndoEnabled,
            Action<string> setStatus,
            Action<VisionToolSignalMarker> setAdvisoryMarker)
        {
            this.evidenceProvider = evidenceProvider ?? throw new ArgumentNullException(nameof(evidenceProvider));
            this.propertyProvider = propertyProvider ?? throw new ArgumentNullException(nameof(propertyProvider));
            this.interactionController = interactionController ?? throw new ArgumentNullException(nameof(interactionController));
            this.setPanelVisible = setPanelVisible ?? throw new ArgumentNullException(nameof(setPanelVisible));
            this.setUseEnabled = setUseEnabled ?? throw new ArgumentNullException(nameof(setUseEnabled));
            this.setUndoEnabled = setUndoEnabled ?? throw new ArgumentNullException(nameof(setUndoEnabled));
            this.setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
            this.setAdvisoryMarker = setAdvisoryMarker ?? throw new ArgumentNullException(nameof(setAdvisoryMarker));
        }

        public VisionToolThresholdSuggestion CurrentSuggestion => session.CurrentSuggestion;

        public bool HasAcceptedSuggestion => session.CurrentSuggestion?.Accepted == true;

        public int SuggestedThreshold => session.CurrentSuggestion?.Threshold ?? -1;

        public string SuggestionEvidenceId => session.CurrentSuggestion?.EvidenceId ?? string.Empty;

        public void Analyze()
        {
            VisionToolSignalEvidence evidence = evidenceProvider();
            ThresholdToolProperty property = propertyProvider();
            VisionToolThresholdSuggestion suggestion = session.Analyze(evidence, property);
            if (suggestion == null)
            {
                setAdvisoryMarker(null);
                setUseEnabled(false);
                setStatus("Rejected: Threshold Basic and one current Preview histogram are required.");
                return;
            }

            setStatus(
                suggestion.Reason
                + Environment.NewLine
                + "Suggestion evidence "
                + ShortId(suggestion.EvidenceId)
                + " / source "
                + ShortId(evidence.SourceSha256)
                + " / region "
                + evidence.RegionDescription);
            setUseEnabled(suggestion.Accepted);
            setAdvisoryMarker(
                suggestion.Accepted
                    ? new VisionToolSignalMarker(
                        "ThresholdSuggestion",
                        property.ThresholdType == OpenCvSharp.ThresholdTypes.BinaryInv
                            ? "Dark candidate"
                            : "Bright candidate",
                        suggestion.Threshold,
                        "#E67E22",
                        false)
                    : null);
        }

        public void Use()
        {
            VisionToolSignalEvidence evidence = evidenceProvider();
            ThresholdToolProperty property = propertyProvider();
            VisionToolThresholdSuggestion suggestion = session.CurrentSuggestion;
            VisionToolThresholdSuggestionUseResult result = session.Use(evidence, property);
            if (result == VisionToolThresholdSuggestionUseResult.AlreadyCurrent)
            {
                setStatus(
                    $"T={suggestion.Threshold} is already the current teaching value; no Preview was scheduled.");
                setUseEnabled(false);
                return;
            }

            interactionController.ApplySignalMarkerValue(
                OpenVisionNativeThresholdSignalEvidenceFactory.ThresholdMarkerId,
                suggestion.Threshold);
        }

        public void Undo()
        {
            ThresholdToolProperty property = propertyProvider();
            VisionToolSignalEvidence evidence = evidenceProvider();
            int previousThreshold = session.Undo(evidence, property);
            interactionController.ApplySignalMarkerValue(
                OpenVisionNativeThresholdSignalEvidenceFactory.ThresholdMarkerId,
                previousThreshold);
        }

        public void UpdateAvailability(VisionToolSignalEvidence evidence)
        {
            bool isBasic = evidence != null
                && string.Equals(
                    evidence.ToolIdentity,
                    "Threshold/" + ThresholdToolMode.Threshold,
                    StringComparison.Ordinal);
            setPanelVisible(isBasic);
            session.ClearSuggestion();
            setAdvisoryMarker(null);
            setUseEnabled(false);
            if (!isBasic)
            {
                setUndoEnabled(false);
                return;
            }

            ThresholdToolProperty property = propertyProvider();
            bool canUndo = session.CanUndo(evidence, property);
            setUndoEnabled(canUndo);
            setStatus(canUndo
                ? $"Applied suggested T={session.AppliedThreshold}. Previous T={session.PreviousThreshold} remains recoverable with Undo."
                : "Analyze the current Preview full-image histogram. No teaching value changes until Use.");
        }

        public void Clear()
        {
            session.ClearSuggestion();
            setPanelVisible(false);
            setUseEnabled(false);
        }

        private static string ShortId(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "-"
                : value.Substring(0, Math.Min(12, value.Length));
        }
    }
}
