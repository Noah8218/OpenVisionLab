using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OpenVisionLab
{
    internal enum VisionPipelineLifecycleFailureStage
    {
        AfterJournalPrepared,
        AfterBackupCreated,
        AfterTargetCreated,
        AfterActivePointerUpdated,
        AfterSourceRemoved,
        AfterBackupRemoved
    }

    internal static class VisionPipelineStorage
    {
        private const string ActivePipelineFileName = "pipeline.active";
        private const string LifecycleJournalFileName = "pipeline.lifecycle.json";
        private const int LifecycleJournalSchemaVersion = 1;
        private static readonly JsonSerializerOptions lifecycleJsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        private static readonly object lifecycleSync = new object();
        private static readonly object persistenceStateSync = new object();
        private static readonly Dictionary<string, VisionPipelinePersistenceState>
            persistenceStates =
                new Dictionary<string, VisionPipelinePersistenceState>(
                    StringComparer.OrdinalIgnoreCase);
        private static VisionPipelineLifecycleFailureStage?
            lifecycleFailureStageForTest;

        internal static IDisposable BeginLifecycleFailureInjectionForTest(
            VisionPipelineLifecycleFailureStage stage)
        {
            lock (lifecycleSync)
            {
                VisionPipelineLifecycleFailureStage? previous =
                    lifecycleFailureStageForTest;
                lifecycleFailureStageForTest = stage;
                return new LifecycleFailureInjectionScope(previous);
            }
        }

        internal static void ResetRuntimePersistenceStateForTest()
        {
            lock (persistenceStateSync)
            {
                persistenceStates.Clear();
            }
        }

        public static VisionPipeline Load(string recipeName, string pipelineName)
        {
            VisionPipeline defaultPipeline = new VisionPipeline
            {
                Name = string.IsNullOrWhiteSpace(pipelineName) ? "Pipeline" : pipelineName
            };

            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, defaultPipeline.Name);
            try
            {
                if (!TryRecoverPendingLifecycleTransaction(
                        recipeName,
                        out string recoveryMessage))
                {
                    SetLifecycleRecoveryState(
                        recipeName,
                        defaultPipeline.Name,
                        VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                        path,
                        recoveryMessage);
                    return defaultPipeline;
                }

                if (SerializeHelper.TryLoadFromXmlFile(
                        path,
                        out VisionPipeline pipeline,
                        out Exception loadException)
                    && pipeline != null)
                {
                    return pipeline;
                }

                if (!File.Exists(path))
                {
                    ClearPersistenceState(path);
                    SerializeHelper.SaveXmlFile(
                        path,
                        defaultPipeline);
                    return defaultPipeline;
                }

                VisionPipelinePersistenceState previousState =
                    GetPersistenceState(path);
                string backupPath =
                    previousState?.Kind
                            == VisionPipelinePersistenceStateKind
                                .InvalidFileSubstituted
                        && File.Exists(previousState.BackupPath)
                            ? previousState.BackupPath
                            : SerializeHelper.BackupInvalidXmlFile(
                                path);
                SetPersistenceState(
                    path,
                    new VisionPipelinePersistenceState(
                        VisionPipelinePersistenceStateKind
                            .InvalidFileSubstituted,
                        recipeName,
                        defaultPipeline.Name,
                        path,
                        backupPath,
                        loadException?.Message));
                return defaultPipeline;
            }
            catch (Exception ex)
            {
                SetPersistenceState(
                    path,
                    new VisionPipelinePersistenceState(
                        VisionPipelinePersistenceStateKind.LoadFailed,
                        recipeName,
                        defaultPipeline.Name,
                        path,
                        string.Empty,
                        ex.GetBaseException().Message));
                return defaultPipeline;
            }
        }

        public static void Save(string recipeName, VisionPipeline pipeline)
        {
            if (!TryRecoverPendingLifecycleTransaction(
                    recipeName,
                    out string recoveryMessage))
            {
                throw new InvalidOperationException(recoveryMessage);
            }

            VisionPipeline target = pipeline ?? new VisionPipeline { Name = "Pipeline" };
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, target.Name);
            VisionPipelinePersistenceState previousState =
                GetPersistenceState(path);
            try
            {
                SerializeHelper.SaveXmlFile(path, target);
                if (previousState?.IsFailure == true)
                {
                    SetPersistenceState(
                        path,
                        new VisionPipelinePersistenceState(
                            VisionPipelinePersistenceStateKind.SaveRecovered,
                            recipeName,
                            target.Name,
                            path,
                            previousState.BackupPath,
                            string.Empty));
                }
                else
                {
                    ClearPersistenceState(path);
                }
            }
            catch (Exception ex)
            {
                SetPersistenceState(
                    path,
                    new VisionPipelinePersistenceState(
                        VisionPipelinePersistenceStateKind.SaveFailed,
                        recipeName,
                        target.Name,
                        path,
                        previousState?.BackupPath,
                        ex.GetBaseException().Message));
                throw;
            }
        }

        internal static bool TryGetPersistenceState(
            string recipeName,
            string pipelineName,
            out VisionPipelinePersistenceState state)
        {
            if (!TryRecoverPendingLifecycleTransaction(
                    recipeName,
                    out string recoveryMessage))
            {
                string recoveryName = string.IsNullOrWhiteSpace(pipelineName)
                    ? "Pipeline"
                    : pipelineName.Trim();
                string recoveryPath = RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    recoveryName);
                SetLifecycleRecoveryState(
                    recipeName,
                    recoveryName,
                    VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                    recoveryPath,
                    recoveryMessage);
            }

            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    pipelineName,
                    "Pipeline",
                    "Pipeline name",
                    out string name,
                    out _))
            {
                state = null;
                return false;
            }

            string path =
                RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    name);
            state = GetPersistenceState(path);
            return state != null;
        }

        public static bool TryLoadFromFile(string path, out VisionPipeline pipeline, out string message)
        {
            pipeline = null;
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                message = "Pipeline XML file was not found.";
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlFile(path, out pipeline) || pipeline == null)
            {
                message = "Pipeline XML could not be loaded as an OpenVision pipeline.";
                return false;
            }

            message = $"Loaded pipeline '{pipeline.Name}' with {pipeline.Steps.Count} step(s).";
            return true;
        }

        public static bool TrySaveToFile(string path, VisionPipeline pipeline, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                message = "Export path is empty.";
                return false;
            }

            if (pipeline == null)
            {
                message = "Pipeline is null.";
                return false;
            }

            try
            {
                SerializeHelper.SaveXmlFile(path, pipeline);
                message = $"Exported pipeline '{pipeline.Name}' to {path}.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        public static bool TryDuplicatePipeline(
            string recipeName,
            string sourcePipelineName,
            string targetPipelineName,
            out string message)
        {
            message = string.Empty;
            if (!TryRecoverPendingLifecycleTransaction(recipeName, out message))
            {
                return false;
            }

            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    sourcePipelineName,
                    "Pipeline",
                    "Source pipeline name",
                    out string sourceName,
                    out message)
                || !RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    targetPipelineName,
                    "Pipeline",
                    "Target pipeline name",
                    out string targetName,
                    out message))
            {
                return false;
            }

            if (string.Equals(sourceName, targetName, StringComparison.OrdinalIgnoreCase))
            {
                message = "Source and target pipeline names are the same.";
                return false;
            }

            string sourcePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, sourceName);
            string targetPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, targetName);
            if (!File.Exists(sourcePath))
            {
                message = "Source pipeline XML was not found.";
                return false;
            }

            if (File.Exists(targetPath))
            {
                message = "Target pipeline already exists.";
                return false;
            }

            if (!TryLoadFromFile(sourcePath, out VisionPipeline pipeline, out message))
            {
                return false;
            }

            pipeline.Name = targetName;
            Save(recipeName, pipeline);
            message = $"Duplicated pipeline '{sourceName}' to '{targetName}'.";
            return true;
        }

        public static bool TryRenamePipeline(
            string recipeName,
            string oldPipelineName,
            string newPipelineName,
            out string message)
        {
            message = string.Empty;
            lock (lifecycleSync)
            {
                if (!TryRecoverPendingLifecycleTransactionCore(recipeName, out message))
                {
                    return false;
                }

                if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                        oldPipelineName,
                        "Pipeline",
                        "Existing pipeline name",
                        out string oldName,
                        out message)
                    || !RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                        newPipelineName,
                        "Pipeline",
                        "New pipeline name",
                        out string newName,
                        out message))
                {
                    return false;
                }

                if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    message = "Pipeline name did not change.";
                    return false;
                }

                string oldPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, oldName);
                string newPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, newName);
                if (!File.Exists(oldPath))
                {
                    message = "Pipeline XML was not found.";
                    return false;
                }

                if (File.Exists(newPath))
                {
                    message = "Target pipeline already exists.";
                    return false;
                }

                if (!TryLoadFromFile(oldPath, out VisionPipeline pipeline, out message))
                {
                    return false;
                }

                VisionPipelineLifecycleJournal journal = CreateLifecycleJournal(
                    recipeName,
                    LifecycleOperation.Rename,
                    oldName,
                    newName,
                    string.Empty);
                string journalPath = GetLifecycleJournalPath(recipeName);
                try
                {
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterJournalPrepared);

                    string backupPath = GetLifecycleBackupPath(oldPath, journal);
                    File.Copy(oldPath, backupPath, overwrite: false);
                    journal.Stage = LifecycleStage.BackupCreated;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterBackupCreated);

                    pipeline.Name = newName;
                    SavePipelineFile(newPath, pipeline);
                    journal.Stage = LifecycleStage.TargetCreated;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterTargetCreated);

                    if (journal.PointerChangeRequired)
                    {
                        WriteActivePipelinePointerAtomically(
                            GetActivePipelineNamePath(recipeName),
                            journal.ExpectedActivePointerText);
                    }
                    journal.Stage = LifecycleStage.ActivePointerUpdated;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterActivePointerUpdated);

                    File.Delete(oldPath);
                    journal.Stage = LifecycleStage.SourceRemoved;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterSourceRemoved);

                    journal.Stage = LifecycleStage.BackupRemoved;
                    SaveLifecycleJournal(journalPath, journal);
                    File.Delete(backupPath);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterBackupRemoved);

                    File.Delete(journalPath);
                    ClearPersistenceState(oldPath);
                    ClearPersistenceState(newPath);
                    message = $"Renamed pipeline '{oldName}' to '{newName}'.";
                    return true;
                }
                catch (Exception ex)
                {
                    SetLifecycleRecoveryState(
                        recipeName,
                        oldName,
                        VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                        oldPath,
                        "Pipeline rename was interrupted. Recovery will run on the next storage open. "
                            + ex.GetBaseException().Message,
                        journal.BackupFileName);
                    message =
                        "Pipeline rename was not completed; the prior state is retained for recovery. "
                        + ex.GetBaseException().Message;
                    return false;
                }
            }
        }

        public static bool TryDeletePipeline(
            string recipeName,
            string pipelineName,
            out string fallbackPipelineName,
            out string message)
        {
            fallbackPipelineName = string.Empty;
            message = string.Empty;
            lock (lifecycleSync)
            {
                if (!TryRecoverPendingLifecycleTransactionCore(recipeName, out message))
                {
                    return false;
                }

                if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                        pipelineName,
                        "Pipeline",
                        "Pipeline name",
                        out string name,
                        out message))
                {
                    return false;
                }

                string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
                if (pipelineNames.Length <= 1)
                {
                    message = "Cannot delete the last pipeline in a recipe.";
                    return false;
                }

                string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, name);
                if (!File.Exists(path))
                {
                    message = "Pipeline XML was not found.";
                    return false;
                }

                fallbackPipelineName = pipelineNames
                    .FirstOrDefault(candidate => !string.Equals(candidate, name, StringComparison.OrdinalIgnoreCase))
                    ?? "Pipeline";

                VisionPipelineLifecycleJournal journal = CreateLifecycleJournal(
                    recipeName,
                    LifecycleOperation.Delete,
                    name,
                    string.Empty,
                    fallbackPipelineName);
                string journalPath = GetLifecycleJournalPath(recipeName);
                try
                {
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterJournalPrepared);

                    string backupPath = GetLifecycleBackupPath(path, journal);
                    File.Copy(path, backupPath, overwrite: false);
                    journal.Stage = LifecycleStage.BackupCreated;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterBackupCreated);

                    if (journal.PointerChangeRequired)
                    {
                        WriteActivePipelinePointerAtomically(
                            GetActivePipelineNamePath(recipeName),
                            journal.ExpectedActivePointerText);
                    }
                    journal.Stage = LifecycleStage.ActivePointerUpdated;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterActivePointerUpdated);

                    File.Delete(path);
                    journal.Stage = LifecycleStage.SourceRemoved;
                    SaveLifecycleJournal(journalPath, journal);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterSourceRemoved);

                    journal.Stage = LifecycleStage.BackupRemoved;
                    SaveLifecycleJournal(journalPath, journal);
                    File.Delete(backupPath);
                    InjectLifecycleFailure(VisionPipelineLifecycleFailureStage.AfterBackupRemoved);

                    File.Delete(journalPath);
                    ClearPersistenceState(path);
                    message = $"Deleted pipeline '{name}'.";
                    return true;
                }
                catch (Exception ex)
                {
                    SetLifecycleRecoveryState(
                        recipeName,
                        name,
                        VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                        path,
                        "Pipeline deletion was interrupted. Recovery will run on the next storage open. "
                            + ex.GetBaseException().Message,
                        journal.BackupFileName);
                    message =
                        "Pipeline deletion was not completed; the prior state is retained for recovery. "
                        + ex.GetBaseException().Message;
                    return false;
                }
            }
        }

        public static string LoadActivePipelineName(string recipeName, string fallbackName)
        {
            string fallback = RecipeWorkspaceService.NormalizeStoragePathSegment(
                fallbackName,
                "Pipeline",
                "Fallback pipeline name");
            if (!TryRecoverPendingLifecycleTransaction(recipeName, out _))
            {
                return fallback;
            }

            string path = GetActivePipelineNamePath(recipeName);
            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            if (!System.IO.File.Exists(path))
            {
                return ResolveExistingPipelineName(pipelineNames, fallback);
            }

            string name = System.IO.File.ReadAllText(path, Encoding.UTF8)?.Trim();
            if (RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                name,
                fallback,
                "Active pipeline name",
                out string normalized,
                out _))
            {
                return pipelineNames.Any(candidate =>
                        string.Equals(candidate, normalized, StringComparison.OrdinalIgnoreCase))
                    ? normalized
                    : ResolveExistingPipelineName(pipelineNames, fallback);
            }

            return ResolveExistingPipelineName(pipelineNames, fallback);
        }

        public static void SaveActivePipelineName(string recipeName, string pipelineName)
        {
            string name = RecipeWorkspaceService.NormalizeStoragePathSegment(
                pipelineName,
                "Pipeline",
                "Pipeline name");
            if (!TryRecoverPendingLifecycleTransaction(
                    recipeName,
                    out string recoveryMessage))
            {
                throw new InvalidOperationException(recoveryMessage);
            }

            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            if (!pipelineNames.Any(candidate =>
                    string.Equals(candidate, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"Active Pipeline '{name}' does not exist in the current Pipeline inventory.");
            }

            string path = GetActivePipelineNamePath(recipeName);
            WriteActivePipelinePointerAtomically(path, name);
        }

        private static string GetActivePipelineNamePath(string recipeName)
        {
            return RecipeWorkspaceService.GetVisionConfigPath(recipeName, ActivePipelineFileName);
        }

        private static bool TryRecoverPendingLifecycleTransaction(
            string recipeName,
            out string message)
        {
            lock (lifecycleSync)
            {
                return TryRecoverPendingLifecycleTransactionCore(
                    recipeName,
                    out message);
            }
        }

        private static bool TryRecoverPendingLifecycleTransactionCore(
            string recipeName,
            out string message)
        {
            message = string.Empty;
            string journalPath = GetLifecycleJournalPath(recipeName);
            if (!File.Exists(journalPath))
            {
                return true;
            }

            VisionPipelineLifecycleJournal journal;
            try
            {
                journal = JsonSerializer.Deserialize<VisionPipelineLifecycleJournal>(
                    File.ReadAllText(journalPath, Encoding.UTF8),
                    lifecycleJsonOptions);
                ValidateLifecycleJournal(journal, recipeName);
            }
            catch (Exception ex)
            {
                message =
                    "An incomplete Pipeline lifecycle journal could not be read; "
                    + "operator files were not changed. "
                    + ex.GetBaseException().Message;
                SetLifecycleRecoveryState(
                    recipeName,
                    "Pipeline",
                    VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                    journalPath,
                    message);
                return false;
            }

            string oldPath = RecipeWorkspaceService.GetVisionPipelinePath(
                recipeName,
                journal.OldPipelineName);
            string newPath = string.IsNullOrWhiteSpace(journal.NewPipelineName)
                ? string.Empty
                : RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    journal.NewPipelineName);
            string backupPath = GetLifecycleBackupPath(oldPath, journal);

            try
            {
                if (!File.Exists(backupPath)
                    && StageAtLeast(journal.Stage, LifecycleStage.BackupRemoved)
                    && IsLifecycleCommitted(
                        recipeName,
                        journal,
                        oldPath,
                        newPath))
                {
                    File.Delete(journalPath);
                    SetLifecycleRecoveryState(
                        recipeName,
                        string.Equals(journal.Operation, LifecycleOperation.Rename, StringComparison.Ordinal)
                            ? journal.NewPipelineName
                            : journal.FallbackPipelineName,
                        VisionPipelinePersistenceStateKind.LifecycleRecovered,
                        string.IsNullOrWhiteSpace(newPath) ? oldPath : newPath,
                        "The interrupted Pipeline lifecycle was already complete; its journal was cleared.");
                    message = "Recovered an already completed Pipeline lifecycle.";
                    return true;
                }

                RollbackLifecycleJournal(
                    recipeName,
                    journal,
                    journalPath,
                    oldPath,
                    newPath,
                    backupPath);
                SetLifecycleRecoveryState(
                    recipeName,
                    journal.OldPipelineName,
                    VisionPipelinePersistenceStateKind.LifecycleRecovered,
                    oldPath,
                    "The incomplete Pipeline lifecycle was rolled back to its prior state.");
                message = "Recovered the incomplete Pipeline lifecycle to its prior state.";
                return true;
            }
            catch (Exception ex)
            {
                message =
                    "An incomplete Pipeline lifecycle needs operator review; "
                    + "no uncertain file was deleted. "
                    + ex.GetBaseException().Message;
                SetLifecycleRecoveryState(
                    recipeName,
                    journal.OldPipelineName,
                    VisionPipelinePersistenceStateKind.LifecycleRecoveryRequired,
                    oldPath,
                    message,
                    journal.BackupFileName);
                return false;
            }
        }

        private static void ValidateLifecycleJournal(
            VisionPipelineLifecycleJournal journal,
            string recipeName)
        {
            if (journal == null
                || journal.SchemaVersion != LifecycleJournalSchemaVersion)
            {
                throw new InvalidDataException("Unsupported Pipeline lifecycle journal schema.");
            }

            if (!string.Equals(
                    journal.RecipeName ?? string.Empty,
                    recipeName ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Pipeline lifecycle journal Recipe does not match its storage root.");
            }

            if (!string.Equals(journal.Operation, LifecycleOperation.Rename, StringComparison.Ordinal)
                && !string.Equals(journal.Operation, LifecycleOperation.Delete, StringComparison.Ordinal))
            {
                throw new InvalidDataException("Unknown Pipeline lifecycle operation.");
            }

            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    journal.OldPipelineName,
                    "Pipeline",
                    "Journal source pipeline name",
                    out string normalizedOldName,
                    out string oldNameError))
            {
                throw new InvalidDataException(oldNameError);
            }

            if (!string.Equals(normalizedOldName, journal.OldPipelineName, StringComparison.Ordinal))
            {
                throw new InvalidDataException("Pipeline lifecycle journal source name is not canonical.");
            }

            if (string.Equals(journal.Operation, LifecycleOperation.Rename, StringComparison.Ordinal))
            {
                if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                        journal.NewPipelineName,
                        "Pipeline",
                        "Journal target pipeline name",
                        out string normalizedNewName,
                        out string newNameError)
                    || !string.Equals(normalizedNewName, journal.NewPipelineName, StringComparison.Ordinal))
                {
                    throw new InvalidDataException(newNameError ?? "Pipeline lifecycle target name is not canonical.");
                }
            }

            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    journal.FallbackPipelineName,
                    "Pipeline",
                    "Journal fallback pipeline name",
                    out _,
                    out string fallbackError)
                && string.Equals(journal.Operation, LifecycleOperation.Delete, StringComparison.Ordinal))
            {
                throw new InvalidDataException(fallbackError);
            }

            if (string.IsNullOrWhiteSpace(journal.BackupFileName)
                || journal.BackupFileName.IndexOfAny(
                    new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) >= 0)
            {
                throw new InvalidDataException("Pipeline lifecycle journal backup name is invalid.");
            }

            if (!IsKnownLifecycleStage(journal.Stage))
            {
                throw new InvalidDataException("Unknown Pipeline lifecycle journal stage.");
            }
        }

        private static void RollbackLifecycleJournal(
            string recipeName,
            VisionPipelineLifecycleJournal journal,
            string journalPath,
            string oldPath,
            string newPath,
            string backupPath)
        {
            bool backupExists = File.Exists(backupPath);
            bool sourceExists = File.Exists(oldPath);
            bool targetExists = !string.IsNullOrWhiteSpace(newPath)
                && File.Exists(newPath);

            if (string.Equals(journal.Operation, LifecycleOperation.Rename, StringComparison.Ordinal)
                && targetExists)
            {
                if (!StageAtLeast(journal.Stage, LifecycleStage.TargetCreated)
                    || !PipelineFileHasName(newPath, journal.NewPipelineName))
                {
                    throw new IOException("The Pipeline rename target was not proven to be transaction-owned.");
                }

                File.Delete(newPath);
                targetExists = false;
            }

            if (!sourceExists)
            {
                if (!backupExists)
                {
                    throw new IOException("The prior Pipeline file is missing and no recovery backup remains.");
                }

                if (!PipelineFileHasName(backupPath, journal.OldPipelineName))
                {
                    throw new IOException("The recovery backup is not a valid copy of the prior Pipeline.");
                }

                CopyFileAtomically(backupPath, oldPath);
                sourceExists = true;
            }
            else if (backupExists && !FilesHaveSameBytes(oldPath, backupPath))
            {
                throw new IOException("The prior Pipeline file changed while lifecycle recovery was pending.");
            }

            RestoreActivePointer(recipeName, journal);

            if (backupExists)
            {
                File.Delete(backupPath);
            }

            File.Delete(journalPath);
        }

        private static bool IsLifecycleCommitted(
            string recipeName,
            VisionPipelineLifecycleJournal journal,
            string oldPath,
            string newPath)
        {
            if (File.Exists(oldPath))
            {
                return false;
            }

            if (string.Equals(journal.Operation, LifecycleOperation.Rename, StringComparison.Ordinal)
                && (!File.Exists(newPath)
                    || !PipelineFileHasName(newPath, journal.NewPipelineName)))
            {
                return false;
            }

            if (string.Equals(journal.Operation, LifecycleOperation.Delete, StringComparison.Ordinal)
                && !RecipeWorkspaceService.GetVisionPipelineNames(recipeName).Any(candidate =>
                    string.Equals(
                        candidate,
                        journal.FallbackPipelineName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return IsPointerInExpectedState(recipeName, journal);
        }

        private static void RestoreActivePointer(
            string recipeName,
            VisionPipelineLifecycleJournal journal)
        {
            if (!journal.PointerChangeRequired)
            {
                return;
            }

            string path = GetActivePipelineNamePath(recipeName);
            PointerSnapshot current = ReadActivePointer(path);
            if (!current.Exists)
            {
                if (!journal.OldActivePointerExisted)
                {
                    return;
                }

                throw new IOException("The prior active Pipeline pointer is missing during recovery.");
            }

            if (string.Equals(
                    current.Text,
                    journal.OldActivePointerText ?? string.Empty,
                    StringComparison.Ordinal))
            {
                return;
            }

            if (!string.Equals(
                    current.Text.Trim(),
                    journal.ExpectedActivePointerText,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("The active Pipeline pointer changed outside the lifecycle transaction.");
            }

            if (journal.OldActivePointerExisted)
            {
                WriteActivePipelinePointerAtomically(
                    path,
                    journal.OldActivePointerText ?? string.Empty);
            }
            else
            {
                File.Delete(path);
            }
        }

        private static bool IsPointerInExpectedState(
            string recipeName,
            VisionPipelineLifecycleJournal journal)
        {
            PointerSnapshot current = ReadActivePointer(
                GetActivePipelineNamePath(recipeName));
            if (!journal.PointerChangeRequired)
            {
                return current.Exists == journal.OldActivePointerExisted
                    && (!current.Exists
                        || string.Equals(
                            current.Text,
                            journal.OldActivePointerText ?? string.Empty,
                            StringComparison.Ordinal));
            }

            return current.Exists
                && string.Equals(
                    current.Text.Trim(),
                    journal.ExpectedActivePointerText,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static VisionPipelineLifecycleJournal CreateLifecycleJournal(
            string recipeName,
            string operation,
            string oldPipelineName,
            string newPipelineName,
            string fallbackPipelineName)
        {
            string[] pipelineNames = RecipeWorkspaceService.GetVisionPipelineNames(recipeName);
            PointerSnapshot pointer = ReadActivePointer(
                GetActivePipelineNamePath(recipeName));
            string activeName = ResolveActiveNameFromSnapshot(
                pointer,
                pipelineNames,
                "Pipeline");
            bool pointerChangeRequired = string.Equals(
                activeName,
                oldPipelineName,
                StringComparison.OrdinalIgnoreCase);

            return new VisionPipelineLifecycleJournal
            {
                SchemaVersion = LifecycleJournalSchemaVersion,
                Operation = operation,
                RecipeName = recipeName ?? string.Empty,
                OldPipelineName = oldPipelineName,
                NewPipelineName = newPipelineName ?? string.Empty,
                FallbackPipelineName = fallbackPipelineName ?? string.Empty,
                BackupFileName = "."
                    + oldPipelineName
                    + ".lifecycle-"
                    + Guid.NewGuid().ToString("N")
                    + ".bak",
                Stage = LifecycleStage.Prepared,
                OldActivePointerExisted = pointer.Exists,
                OldActivePointerText = pointer.Text,
                PointerChangeRequired = pointerChangeRequired,
                ExpectedActivePointerText = string.Equals(
                        operation,
                        LifecycleOperation.Rename,
                        StringComparison.Ordinal)
                    ? newPipelineName
                    : fallbackPipelineName
            };
        }

        private static string ResolveActiveNameFromSnapshot(
            PointerSnapshot pointer,
            string[] pipelineNames,
            string fallbackName)
        {
            if (pointer.Exists
                && RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    pointer.Text,
                    fallbackName,
                    "Active pipeline name",
                    out string normalized,
                    out _)
                && pipelineNames.Any(candidate =>
                    string.Equals(candidate, normalized, StringComparison.OrdinalIgnoreCase)))
            {
                return normalized;
            }

            return ResolveExistingPipelineName(pipelineNames, fallbackName);
        }

        private static string ResolveExistingPipelineName(
            IEnumerable<string> pipelineNames,
            string fallbackName)
        {
            string[] names = (pipelineNames ?? Array.Empty<string>()).ToArray();
            return names.FirstOrDefault(candidate =>
                       string.Equals(candidate, fallbackName, StringComparison.OrdinalIgnoreCase))
                ?? names.FirstOrDefault()
                ?? fallbackName;
        }

        private static string GetLifecycleJournalPath(string recipeName)
        {
            return RecipeWorkspaceService.GetVisionConfigPath(
                recipeName,
                LifecycleJournalFileName);
        }

        private static string GetLifecycleBackupPath(
            string sourcePath,
            VisionPipelineLifecycleJournal journal)
        {
            string directory = Path.GetDirectoryName(sourcePath)
                ?? throw new InvalidOperationException("Pipeline storage directory could not be resolved.");
            return RecipeWorkspaceService.GetContainedStoragePath(
                directory,
                journal.BackupFileName,
                "Pipeline lifecycle backup path");
        }

        private static void SaveLifecycleJournal(
            string journalPath,
            VisionPipelineLifecycleJournal journal)
        {
            WriteTextFileAtomically(
                journalPath,
                JsonSerializer.Serialize(journal, lifecycleJsonOptions));
        }

        private static void SavePipelineFile(
            string path,
            VisionPipeline pipeline)
        {
            SerializeHelper.SaveXmlFile(path, pipeline);
        }

        private static void WriteActivePipelinePointerAtomically(
            string path,
            string value)
        {
            WriteTextFileAtomically(path, value ?? string.Empty);
        }

        private static void WriteTextFileAtomically(
            string path,
            string contents)
        {
            string directory = Path.GetDirectoryName(path)
                ?? throw new InvalidOperationException("Atomic text-file directory could not be resolved.");
            Directory.CreateDirectory(directory);
            string temporaryPath = Path.Combine(
                directory,
                "." + Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                byte[] bytes = new UTF8Encoding(false).GetBytes(contents ?? string.Empty);
                using (FileStream stream = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.WriteThrough))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }

                ReplaceFileAtomically(temporaryPath, path);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static void ReplaceFileAtomically(
            string temporaryPath,
            string path)
        {
            if (File.Exists(path))
            {
                File.Replace(temporaryPath, path, null);
            }
            else
            {
                File.Move(temporaryPath, path);
            }
        }

        private static void CopyFileAtomically(
            string sourcePath,
            string destinationPath)
        {
            string directory = Path.GetDirectoryName(destinationPath)
                ?? throw new InvalidOperationException("Pipeline restore directory could not be resolved.");
            string temporaryPath = Path.Combine(
                directory,
                "." + Path.GetFileName(destinationPath) + "." + Guid.NewGuid().ToString("N") + ".restore.tmp");
            try
            {
                File.Copy(sourcePath, temporaryPath, overwrite: false);
                ReplaceFileAtomically(temporaryPath, destinationPath);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static PointerSnapshot ReadActivePointer(string path)
        {
            return File.Exists(path)
                ? new PointerSnapshot(true, File.ReadAllText(path, Encoding.UTF8))
                : new PointerSnapshot(false, string.Empty);
        }

        private static bool PipelineFileHasName(string path, string expectedName)
        {
            return SerializeHelper.TryLoadFromXmlFile(
                    path,
                    out VisionPipeline pipeline)
                && pipeline != null
                && string.Equals(
                    pipeline.Name,
                    expectedName,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool FilesHaveSameBytes(string leftPath, string rightPath)
        {
            FileInfo left = new FileInfo(leftPath);
            FileInfo right = new FileInfo(rightPath);
            if (left.Length != right.Length)
            {
                return false;
            }

            using FileStream leftStream = new FileStream(
                leftPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            using FileStream rightStream = new FileStream(
                rightPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            byte[] leftBuffer = new byte[81920];
            byte[] rightBuffer = new byte[81920];
            while (true)
            {
                int leftRead = leftStream.Read(leftBuffer, 0, leftBuffer.Length);
                int rightRead = rightStream.Read(rightBuffer, 0, rightBuffer.Length);
                if (leftRead != rightRead)
                {
                    return false;
                }

                if (leftRead == 0)
                {
                    return true;
                }

                for (int i = 0; i < leftRead; i++)
                {
                    if (leftBuffer[i] != rightBuffer[i])
                    {
                        return false;
                    }
                }
            }
        }

        private static bool StageAtLeast(string stage, string minimumStage)
        {
            return LifecycleStageRank(stage) >= LifecycleStageRank(minimumStage);
        }

        private static int LifecycleStageRank(string stage)
        {
            return stage switch
            {
                LifecycleStage.Prepared => 0,
                LifecycleStage.BackupCreated => 1,
                LifecycleStage.TargetCreated => 2,
                LifecycleStage.ActivePointerUpdated => 3,
                LifecycleStage.SourceRemoved => 4,
                LifecycleStage.BackupRemoved => 5,
                _ => -1
            };
        }

        private static bool IsKnownLifecycleStage(string stage)
        {
            return LifecycleStageRank(stage) >= 0;
        }

        private static void InjectLifecycleFailure(
            VisionPipelineLifecycleFailureStage stage)
        {
            if (lifecycleFailureStageForTest == stage)
            {
                throw new IOException("Injected Pipeline lifecycle failure at " + stage + ".");
            }
        }

        private static void SetLifecycleRecoveryState(
            string recipeName,
            string pipelineName,
            VisionPipelinePersistenceStateKind kind,
            string sourcePath,
            string message,
            string backupFileName = "")
        {
            if (!RecipeWorkspaceService.TryNormalizeStoragePathSegment(
                    pipelineName,
                    "Pipeline",
                    "Pipeline name",
                    out string normalizedName,
                    out _))
            {
                normalizedName = "Pipeline";
            }

            string backupPath = string.Empty;
            if (!string.IsNullOrWhiteSpace(backupFileName))
            {
                try
                {
                    string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                        recipeName,
                        normalizedName);
                    backupPath = GetLifecycleBackupPath(
                        pipelinePath,
                        new VisionPipelineLifecycleJournal
                        {
                            BackupFileName = backupFileName
                        });
                }
                catch
                {
                    backupPath = string.Empty;
                }
            }

            SetPersistenceState(
                RecipeWorkspaceService.GetVisionPipelinePath(recipeName, normalizedName),
                new VisionPipelinePersistenceState(
                    kind,
                    recipeName,
                    normalizedName,
                    sourcePath,
                    backupPath,
                    message));
        }

        private sealed class LifecycleFailureInjectionScope : IDisposable
        {
            private readonly VisionPipelineLifecycleFailureStage? previous;
            private bool disposed;

            public LifecycleFailureInjectionScope(
                VisionPipelineLifecycleFailureStage? previous)
            {
                this.previous = previous;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                lock (lifecycleSync)
                {
                    lifecycleFailureStageForTest = previous;
                    disposed = true;
                }
            }
        }

        private readonly struct PointerSnapshot
        {
            public PointerSnapshot(bool exists, string text)
            {
                Exists = exists;
                Text = text ?? string.Empty;
            }

            public bool Exists { get; }

            public string Text { get; }
        }

        private static class LifecycleOperation
        {
            public const string Rename = "rename";
            public const string Delete = "delete";
        }

        private static class LifecycleStage
        {
            public const string Prepared = "prepared";
            public const string BackupCreated = "backup-created";
            public const string TargetCreated = "target-created";
            public const string ActivePointerUpdated = "active-pointer-updated";
            public const string SourceRemoved = "source-removed";
            public const string BackupRemoved = "backup-removed";
        }

        private sealed class VisionPipelineLifecycleJournal
        {
            public int SchemaVersion { get; set; }

            public string Operation { get; set; }

            public string RecipeName { get; set; }

            public string OldPipelineName { get; set; }

            public string NewPipelineName { get; set; }

            public string FallbackPipelineName { get; set; }

            public string BackupFileName { get; set; }

            public string Stage { get; set; }

            public bool OldActivePointerExisted { get; set; }

            public string OldActivePointerText { get; set; }

            public bool PointerChangeRequired { get; set; }

            public string ExpectedActivePointerText { get; set; }
        }

        private static VisionPipelinePersistenceState GetPersistenceState(
            string path)
        {
            string key = NormalizePersistencePath(path);
            lock (persistenceStateSync)
            {
                persistenceStates.TryGetValue(
                    key,
                    out VisionPipelinePersistenceState state);
                return state;
            }
        }

        private static void SetPersistenceState(
            string path,
            VisionPipelinePersistenceState state)
        {
            string key = NormalizePersistencePath(path);
            lock (persistenceStateSync)
            {
                persistenceStates[key] = state;
            }
        }

        private static void ClearPersistenceState(string path)
        {
            string key = NormalizePersistencePath(path);
            lock (persistenceStateSync)
            {
                persistenceStates.Remove(key);
            }
        }

        private static string NormalizePersistencePath(string path)
        {
            return Path.GetFullPath(path ?? string.Empty);
        }

        public static bool TryValidateRoundTrip(string recipeName, VisionPipeline pipeline, out string message)
        {
            message = string.Empty;
            if (pipeline == null)
            {
                message = "Pipeline is null.";
                return false;
            }

            try
            {
                VisionPipeline loaded = Load(recipeName, pipeline.Name);
                if (loaded == null)
                {
                    message = "Saved pipeline could not be loaded.";
                    return false;
                }

                if (!SameText(pipeline.Name, loaded.Name))
                {
                    message = $"Pipeline name mismatch. saved='{pipeline.Name}', loaded='{loaded.Name}'.";
                    return false;
                }

                if (pipeline.Steps.Count != loaded.Steps.Count)
                {
                    message = $"Step count mismatch. saved={pipeline.Steps.Count}, loaded={loaded.Steps.Count}.";
                    return false;
                }

                for (int i = 0; i < pipeline.Steps.Count; i++)
                {
                    if (!CompareStep(pipeline.Steps[i], loaded.Steps[i], i, out message))
                    {
                        return false;
                    }
                }

                message = $"Round-trip validation passed. Steps={pipeline.Steps.Count}.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        private static bool CompareStep(VisionPipelineStep expected, VisionPipelineStep actual, int index, out string message)
        {
            message = string.Empty;
            if (expected == null || actual == null)
            {
                message = $"Step {index + 1} is null after load.";
                return false;
            }

            if (!SameText(expected.Name, actual.Name)
                || !SameText(expected.ToolType, actual.ToolType)
                || expected.Enabled != actual.Enabled
                || !SameText(expected.InputLayer, actual.InputLayer)
                || !SameText(expected.OutputLayer, actual.OutputLayer)
                || expected.UseAcceptance != actual.UseAcceptance
                || expected.ExpectedSuccess != actual.ExpectedSuccess
                || !SameDouble(expected.MaxElapsedMilliseconds, actual.MaxElapsedMilliseconds)
                || !SameText(expected.RequiredMessageText, actual.RequiredMessageText)
                || !SameText(expected.AcceptanceMetricName, actual.AcceptanceMetricName)
                || expected.UseAcceptanceMetricMinimum != actual.UseAcceptanceMetricMinimum
                || !SameDouble(expected.AcceptanceMetricMinimum, actual.AcceptanceMetricMinimum)
                || expected.UseAcceptanceMetricMaximum != actual.UseAcceptanceMetricMaximum
                || !SameDouble(expected.AcceptanceMetricMaximum, actual.AcceptanceMetricMaximum))
            {
                message = $"Step {index + 1} metadata mismatch. '{expected.Name}'";
                return false;
            }

            if (!CompareParameters(expected.Parameters, actual.Parameters, index, out message))
            {
                return false;
            }

            return true;
        }

        private static bool CompareParameters(
            IDictionary<string, string> expected,
            IDictionary<string, string> actual,
            int stepIndex,
            out string message)
        {
            message = string.Empty;
            Dictionary<string, string> expectedMap = new Dictionary<string, string>(expected ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> actualMap = new Dictionary<string, string>(actual ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);

            string missingKey = expectedMap.Keys.FirstOrDefault(key => !actualMap.ContainsKey(key));
            if (!string.IsNullOrWhiteSpace(missingKey))
            {
                message = $"Step {stepIndex + 1} parameter missing after load: {missingKey}.";
                return false;
            }

            string extraKey = actualMap.Keys.FirstOrDefault(key => !expectedMap.ContainsKey(key));
            if (!string.IsNullOrWhiteSpace(extraKey))
            {
                message = $"Step {stepIndex + 1} unexpected parameter after load: {extraKey}.";
                return false;
            }

            foreach (KeyValuePair<string, string> parameter in expectedMap)
            {
                if (!SameText(parameter.Value, actualMap[parameter.Key]))
                {
                    message = $"Step {stepIndex + 1} parameter mismatch: {parameter.Key}.";
                    return false;
                }
            }

            return true;
        }

        private static bool SameText(string left, string right)
        {
            return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
        }

        private static bool SameDouble(double left, double right)
        {
            return Math.Abs(left - right) < 0.0000001;
        }
    }
}
