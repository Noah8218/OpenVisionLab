using System;

namespace OpenVisionLab
{
    internal enum RecipeDataPersistenceStateKind
    {
        InvalidFileSubstituted,
        LoadFailed,
        SaveFailed,
        SaveRecovered
    }

    internal sealed class RecipeDataPersistenceState
    {
        public RecipeDataPersistenceState(
            RecipeDataPersistenceStateKind kind,
            string recipeName,
            string sourcePath,
            string backupPath,
            string errorMessage)
        {
            Kind = kind;
            RecipeName = recipeName ?? string.Empty;
            SourcePath = sourcePath ?? string.Empty;
            BackupPath = backupPath ?? string.Empty;
            ErrorMessage = errorMessage ?? string.Empty;
            OccurredAtUtc = DateTime.UtcNow;
        }

        public RecipeDataPersistenceStateKind Kind { get; }

        public string RecipeName { get; }

        public string SourcePath { get; }

        public string BackupPath { get; }

        public string ErrorMessage { get; }

        public DateTime OccurredAtUtc { get; }

        public bool IsFailure =>
            Kind != RecipeDataPersistenceStateKind.SaveRecovered;
    }
}
