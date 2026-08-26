using System;

namespace OpenVisionLab
{
    internal enum VisionPipelinePersistenceStateKind
    {
        InvalidFileSubstituted,
        LoadFailed,
        SaveFailed,
        SaveRecovered,
        LifecycleRecoveryRequired,
        LifecycleRecovered
    }

    internal sealed class VisionPipelinePersistenceState
    {
        public VisionPipelinePersistenceState(
            VisionPipelinePersistenceStateKind kind,
            string recipeName,
            string pipelineName,
            string sourcePath,
            string backupPath,
            string errorMessage)
        {
            Kind = kind;
            RecipeName = recipeName ?? string.Empty;
            PipelineName = pipelineName ?? string.Empty;
            SourcePath = sourcePath ?? string.Empty;
            BackupPath = backupPath ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            OccurredAtUtc = DateTime.UtcNow;
        }

        public VisionPipelinePersistenceStateKind Kind { get; }

        public string RecipeName { get; }

        public string PipelineName { get; }

        public string SourcePath { get; }

        public string BackupPath { get; }

        public string ErrorMessage { get; }

        public DateTime OccurredAtUtc { get; }

        public bool IsFailure =>
            Kind != VisionPipelinePersistenceStateKind.SaveRecovered
                && Kind != VisionPipelinePersistenceStateKind.LifecycleRecovered;
    }
}
