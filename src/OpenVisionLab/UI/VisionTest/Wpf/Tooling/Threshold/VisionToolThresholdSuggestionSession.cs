using OpenCvSharp;
using OpenVisionLab.Vision2D;
using OpenVisionLab.Vision2D.Property;
using System;

namespace OpenVisionLab
{
    internal enum VisionToolThresholdSuggestionUseResult
    {
        Applied,
        AlreadyCurrent
    }

    /// <summary>
    /// Owns the mutable suggestion and Undo policy for Threshold Basic.
    /// It does not access WPF controls or apply parameter changes itself.
    /// </summary>
    internal sealed class VisionToolThresholdSuggestionSession
    {
        private ThresholdSuggestionUndoState undoState;

        public VisionToolThresholdSuggestion CurrentSuggestion { get; private set; }

        public bool HasUndo => undoState != null;

        public int AppliedThreshold => undoState?.AppliedThreshold ?? -1;

        public int PreviousThreshold => undoState?.PreviousThreshold ?? -1;

        public VisionToolThresholdSuggestion Analyze(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            if (evidence == null || property == null || property.Mode != ThresholdToolMode.Threshold)
            {
                CurrentSuggestion = null;
                return null;
            }

            CurrentSuggestion = VisionToolThresholdSuggestionAnalyzer.Analyze(
                evidence,
                property.ThresholdType != ThresholdTypes.BinaryInv);
            return CurrentSuggestion;
        }

        public VisionToolThresholdSuggestionUseResult Use(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            VisionToolThresholdSuggestion suggestion = CurrentSuggestion;
            VisionToolThresholdSuggestion currentAnalysis = AnalyzeCurrent(evidence, property);
            if (suggestion?.Accepted != true
                || evidence == null
                || currentAnalysis == null
                || !string.Equals(
                    suggestion.EvidenceId,
                    currentAnalysis.EvidenceId,
                    StringComparison.Ordinal)
                || property == null
                || property.Mode != ThresholdToolMode.Threshold)
            {
                throw new InvalidOperationException(
                    "The Threshold suggestion is stale or no longer matches the current Preview evidence.");
            }

            int previousThreshold = ClampThreshold(property.Threshold);
            if (previousThreshold == suggestion.Threshold)
            {
                return VisionToolThresholdSuggestionUseResult.AlreadyCurrent;
            }

            undoState = new ThresholdSuggestionUndoState
            {
                SourceSha256 = evidence.SourceSha256,
                PreviousThreshold = previousThreshold,
                AppliedThreshold = suggestion.Threshold
            };
            return VisionToolThresholdSuggestionUseResult.Applied;
        }

        public bool CanUndo(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            bool canUndo = IsUndoValid(evidence, property);
            if (!canUndo)
            {
                undoState = null;
            }

            return canUndo;
        }

        public int Undo(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            ThresholdSuggestionUndoState undo = undoState;
            if (!IsUndoValid(evidence, property))
            {
                throw new InvalidOperationException(
                    "The previous Threshold teaching value is stale and cannot be restored.");
            }

            undoState = null;
            return undo.PreviousThreshold;
        }

        public void ClearSuggestion()
        {
            CurrentSuggestion = null;
        }

        private static VisionToolThresholdSuggestion AnalyzeCurrent(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            if (evidence == null || property == null || property.Mode != ThresholdToolMode.Threshold)
            {
                return null;
            }

            return VisionToolThresholdSuggestionAnalyzer.Analyze(
                evidence,
                property.ThresholdType != ThresholdTypes.BinaryInv);
        }

        private bool IsUndoValid(
            VisionToolSignalEvidence evidence,
            ThresholdToolProperty property)
        {
            return undoState != null
                && evidence != null
                && property != null
                && property.Mode == ThresholdToolMode.Threshold
                && Math.Abs(property.Threshold - undoState.AppliedThreshold) <= 0.001D
                && string.Equals(
                    evidence.SourceSha256,
                    undoState.SourceSha256,
                    StringComparison.Ordinal);
        }

        private static int ClampThreshold(double value)
        {
            return Math.Clamp((int)Math.Round(value), 0, 255);
        }

        private sealed class ThresholdSuggestionUndoState
        {
            public string SourceSha256 { get; init; } = string.Empty;
            public int PreviousThreshold { get; init; }
            public int AppliedThreshold { get; init; }
        }
    }
}
