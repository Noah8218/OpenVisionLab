using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeQualifiedSnapshotController
    {
        private readonly QualifiedRecipeSnapshotStore store;
        private readonly QualifiedRecipeSnapshotLifecycleStore lifecycle;
        private readonly QualifiedRecipeSnapshotWorkingCopyService workingCopy;

        internal OpenVisionRecipeQualifiedSnapshotController(
            string qualifiedRecipeRoot = null)
        {
            string root = string.IsNullOrWhiteSpace(qualifiedRecipeRoot)
                ? AppPathService.QualifiedRecipeRootDirectory
                : Path.GetFullPath(qualifiedRecipeRoot);
            store = new QualifiedRecipeSnapshotStore(root);
            lifecycle = new QualifiedRecipeSnapshotLifecycleStore(root, store);
            workingCopy = new QualifiedRecipeSnapshotWorkingCopyService(store);
        }

        internal OpenVisionRecipeQualificationEvaluation Evaluate(
            string recipeName,
            string pipelineName,
            OpenVisionRecipeBatchRunOption selectedRun,
            OpenVisionRecipeValidationSetOption selectedValidationSet,
            OpenVisionRecipeQualificationScopeOption scope,
            string qualificationNote,
            bool hasPendingEdit,
            string predecessorSnapshotId = "",
            string changeReason = "")
        {
            OpenVisionRecipeQualificationEvaluation evaluation =
                new OpenVisionRecipeQualificationEvaluation();
            if (hasPendingEdit)
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "저장하지 않은 Step 편집이 있습니다. 먼저 적용하거나 취소하세요.",
                        "There is a pending Step edit. Apply or discard it first."));
                return evaluation;
            }

            VisionPipelineBatchRunSummary summary = selectedRun?.RunSummary;
            OpenVisionRecipeValidationSet set = selectedValidationSet?.Set;
            if (summary == null
                || string.IsNullOrWhiteSpace(selectedRun?.SummaryPath))
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "저장된 검증 실행을 선택하세요.",
                        "Select a saved validation run."));
                return evaluation;
            }

            if (!string.Equals(
                    summary.SuiteKind,
                    "LocalValidationSet",
                    StringComparison.OrdinalIgnoreCase))
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "완료된 Local Validation Set 실행만 자격화할 수 있습니다.",
                        "Only a completed Local Validation Set run can be qualified."));
            }

            if (set == null
                || !string.Equals(
                    set.Name,
                    summary.SuiteName,
                    StringComparison.Ordinal)
                || set.IsIdentityLocked
                    && !string.Equals(
                    set.PipelineName,
                    pipelineName,
                    StringComparison.Ordinal))
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "선택한 Validation Set이 실행의 Set/Pipeline과 일치하지 않습니다.",
                        "The selected Validation Set does not match the run Set/Pipeline."));
            }

            if (scope == null)
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "자격 범위를 선택하세요.",
                        "Select a qualification scope."));
            }

            if (string.IsNullOrWhiteSpace(qualificationNote))
            {
                evaluation.Errors.Add(
                    OpenVisionRecipeText.Local(
                        "운영자가 확인한 자격 메모를 입력하세요.",
                        "Enter the operator qualification note."));
            }

            if (evaluation.Errors.Count > 0)
            {
                return evaluation;
            }

            string pipelinePath =
                RecipeWorkspaceService.GetVisionPipelinePath(
                    recipeName,
                    pipelineName);
            QualifiedRecipeSnapshotCreateRequest request =
                new QualifiedRecipeSnapshotCreateRequest
                {
                    Scope = scope.Scope,
                    DisplayName = recipeName + " / " + pipelineName,
                    QualificationNote = qualificationNote.Trim(),
                    SourceRecipeName = recipeName,
                    PipelineName = pipelineName,
                    PipelineFilePath = pipelinePath,
                    BatchSummaryFilePath = selectedRun.SummaryPath,
                    PredecessorSnapshotId = predecessorSnapshotId ?? string.Empty,
                    ChangeReason = changeReason ?? string.Empty,
                    CreatedAtUtc = DateTime.UtcNow,
                    ValidationSet = CreateValidationSetSnapshot(
                        set,
                        pipelineName,
                        pipelinePath),
                    RuntimeFiles = CreateRuntimeFingerprintSources()
                };
            QualifiedRecipeSnapshotPreflightResult preflight =
                QualifiedRecipeSnapshotPreflight.Evaluate(request);
            evaluation.Request = request;
            evaluation.Counts = preflight.Counts;
            evaluation.Errors.AddRange(preflight.Errors);
            return evaluation;
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult Create(
            OpenVisionRecipeQualificationEvaluation evaluation)
        {
            if (evaluation?.Success != true || evaluation.Request == null)
            {
                return Failure(
                    evaluation == null
                        ? "Qualification preflight was not evaluated."
                        : string.Join(Environment.NewLine, evaluation.Errors));
            }

            QualifiedRecipeSnapshotCreateResult created =
                store.Create(evaluation.Request);
            if (!created.Success)
            {
                return Failure(created.Error);
            }

            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = true,
                SnapshotId = created.SnapshotId,
                Message = created.ReusedExisting
                    ? OpenVisionRecipeText.Local(
                        "동일한 불변 자격 Snapshot을 재사용했습니다: ",
                        "Reused the identical immutable qualified Snapshot: ")
                        + ShortId(created.SnapshotId)
                    : OpenVisionRecipeText.Local(
                        "자격 Snapshot을 생성하고 재검증했습니다: ",
                        "Created and reverified qualified Snapshot: ")
                        + ShortId(created.SnapshotId)
            };
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult Supersede(
            OpenVisionRecipeQualificationEvaluation evaluation,
            string predecessorSnapshotId,
            string reason)
        {
            if (evaluation?.Success != true || evaluation.Request == null)
            {
                return Failure(
                    evaluation == null
                        ? "Qualification preflight was not evaluated."
                        : string.Join(Environment.NewLine, evaluation.Errors));
            }

            evaluation.Request.PredecessorSnapshotId =
                predecessorSnapshotId ?? string.Empty;
            evaluation.Request.ChangeReason = reason?.Trim() ?? string.Empty;
            QualifiedRecipeSnapshotCreateResult successor =
                store.Create(evaluation.Request);
            if (!successor.Success)
            {
                return Failure(successor.Error);
            }

            if (string.Equals(
                    successor.SnapshotId,
                    predecessorSnapshotId,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Failure(
                    "Supersede requires a changed immutable qualification identity.");
            }

            if (!lifecycle.TryAppend(
                    predecessorSnapshotId,
                    QualifiedRecipeSnapshotLifecycleAction.Superseded,
                    reason,
                    successor.SnapshotId,
                    DateTime.UtcNow,
                    out _,
                    out string lifecycleError))
            {
                return Failure(
                    "Successor Snapshot exists, but supersede relation failed: "
                    + lifecycleError);
            }

            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = true,
                SnapshotId = successor.SnapshotId,
                Message = OpenVisionRecipeText.Local(
                    "새 Snapshot을 만든 뒤 기존 Snapshot을 대체 상태로 기록했습니다: ",
                    "Created the successor, then recorded the prior Snapshot as superseded: ")
                    + ShortId(successor.SnapshotId)
            };
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult Revoke(
            string snapshotId,
            string reason)
        {
            if (!lifecycle.TryAppend(
                    snapshotId,
                    QualifiedRecipeSnapshotLifecycleAction.Revoked,
                    reason,
                    string.Empty,
                    DateTime.UtcNow,
                    out _,
                    out string error))
            {
                return Failure(error);
            }

            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = true,
                SnapshotId = snapshotId ?? string.Empty,
                Message = OpenVisionRecipeText.Local(
                    "Snapshot을 폐기 상태로 기록했습니다. 증거 payload는 유지됩니다.",
                    "Recorded the Snapshot as revoked. Its evidence payload remains.")
            };
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult Verify(
            string snapshotId)
        {
            return Verify(
                snapshotId,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null);
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult Verify(
            string snapshotId,
            string currentRecipeName,
            string currentPipelineName,
            string currentPipelinePath,
            OpenVisionRecipeValidationSet currentValidationSet,
            IReadOnlyList<QualifiedRecipeRuntimeFileSource> currentRuntimeFiles)
        {
            QualifiedRecipeSnapshotVerificationResult verification =
                store.Verify(snapshotId);
            QualifiedRecipeSnapshotLifecycleState lifecycleState =
                lifecycle.Load(snapshotId);
            if (!verification.PayloadIntegrityValid || !lifecycleState.Success)
            {
                return Failure(
                    string.Join(
                        " | ",
                        verification.Errors.Concat(lifecycleState.Errors)));
            }

            OpenVisionRecipeQualifiedSnapshotCurrentIdentity current =
                EvaluateCurrentIdentity(
                    snapshotId,
                    currentRecipeName,
                    currentPipelineName,
                    currentPipelinePath,
                    currentValidationSet,
                    currentRuntimeFiles);
            if (!current.Success)
            {
                return Failure(
                    OpenVisionRecipeText.Local(
                        "기존 자격 결과가 stale입니다. 현재 Recipe/입력/runtime을 다시 평가하세요: ",
                        "The existing qualification is stale. Re-evaluate the current Recipe/input/runtime: ")
                    + string.Join(" | ", current.Errors));
            }

            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = true,
                SnapshotId = snapshotId ?? string.Empty,
                Message = lifecycleState.State
                    + " | "
                    + OpenVisionRecipeText.Local(
                        "현재 검증 구성 일치",
                        "current verification identity matches")
            };
        }

        internal OpenVisionRecipeQualifiedSnapshotCurrentIdentity
            EvaluateCurrentIdentity(
                string snapshotId,
                string currentRecipeName,
                string currentPipelineName,
                string currentPipelinePath,
                OpenVisionRecipeValidationSet currentValidationSet,
                IReadOnlyList<QualifiedRecipeRuntimeFileSource> currentRuntimeFiles)
        {
            OpenVisionRecipeQualifiedSnapshotCurrentIdentity result =
                new OpenVisionRecipeQualifiedSnapshotCurrentIdentity();
            QualifiedRecipeSnapshotVerificationResult verification =
                store.Verify(snapshotId);
            if (!verification.PayloadIntegrityValid || verification.Manifest == null)
            {
                result.Errors.Add(
                    "Snapshot payload is not intact: "
                    + string.Join(" | ", verification.Errors));
                return result;
            }

            result.RuntimeMatches = CompareCurrentRuntime(
                verification.Manifest,
                currentRuntimeFiles ?? CreateRuntimeFingerprintSources(),
                result);

            bool hasCurrentRecipeContext = !string.IsNullOrWhiteSpace(currentRecipeName)
                || !string.IsNullOrWhiteSpace(currentPipelineName)
                || !string.IsNullOrWhiteSpace(currentPipelinePath)
                || currentValidationSet != null;
            if (!hasCurrentRecipeContext)
            {
                result.RecipeMatches = true;
                result.InputMatches = true;
                return result;
            }

            QualifiedRecipeSnapshotManifest manifest = verification.Manifest;
            result.RecipeMatches = true;
            if (!string.Equals(
                    manifest.SourceRecipeName,
                    currentRecipeName?.Trim() ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase))
            {
                result.RecipeMatches = false;
                result.Errors.Add("Current Recipe identity differs from the qualified Snapshot.");
            }

            if (!string.Equals(
                    manifest.PipelineName,
                    currentPipelineName?.Trim() ?? string.Empty,
                    StringComparison.Ordinal))
            {
                result.RecipeMatches = false;
                result.Errors.Add("Current Pipeline identity differs from the qualified Snapshot.");
            }

            if (string.IsNullOrWhiteSpace(currentPipelinePath)
                || !File.Exists(currentPipelinePath))
            {
                result.RecipeMatches = false;
                result.Errors.Add("Current Pipeline file is missing.");
            }
            else
            {
                string currentPipelineSha =
                    QualifiedRecipeSnapshotPreflight.ComputeFileSha256(currentPipelinePath);
                if (!string.Equals(
                        currentPipelineSha,
                        manifest.PipelineSha256,
                        StringComparison.OrdinalIgnoreCase))
                {
                    result.RecipeMatches = false;
                    result.Errors.Add("Current Pipeline content differs from the qualified Snapshot.");
                }
            }

            result.InputMatches = CompareCurrentValidationSet(
                snapshotId,
                manifest,
                currentPipelinePath,
                currentValidationSet,
                result);
            return result;
        }

        internal OpenVisionRecipeQualifiedSnapshotActionResult CreateWorkingCopy(
            string snapshotId,
            string targetRecipeName)
        {
            QualifiedRecipeWorkingCopyResult result =
                workingCopy.Create(snapshotId, targetRecipeName);
            if (!result.Success)
            {
                return Failure(result.Error);
            }

            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = true,
                SnapshotId = snapshotId ?? string.Empty,
                RecipeName = result.RecipeName,
                Message = OpenVisionRecipeText.Local(
                    "자격 상태를 상속하지 않는 작업 Recipe를 만들었습니다: ",
                    "Created an editable working Recipe without inherited qualification: ")
                    + result.RecipeName
                    + " / "
                    + result.PipelineName
            };
        }

        internal bool TryGetEvidenceDirectory(
            string snapshotId,
            out string directory,
            out string error)
        {
            QualifiedRecipeSnapshotVerificationResult verification =
                store.Verify(snapshotId);
            if (!verification.PayloadIntegrityValid)
            {
                directory = string.Empty;
                error = string.Join(" | ", verification.Errors);
                return false;
            }

            directory = store.GetSnapshotDirectory(snapshotId);
            error = string.Empty;
            return Directory.Exists(directory);
        }

        internal IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption> List()
        {
            return store.ListSnapshotIds()
                .Select(CreateOption)
                .OrderByDescending(option => option.CreatedAtUtc)
                .ToList();
        }

        internal IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption> List(
            string currentRecipeName,
            string currentPipelineName,
            string currentPipelinePath,
            OpenVisionRecipeValidationSet currentValidationSet)
        {
            return store.ListSnapshotIds()
                .Select(snapshotId => CreateOption(
                    snapshotId,
                    currentRecipeName,
                    currentPipelineName,
                    currentPipelinePath,
                    currentValidationSet))
                .OrderByDescending(option => option.CreatedAtUtc)
                .ToList();
        }

        private OpenVisionRecipeQualifiedSnapshotOption CreateOption(
            string snapshotId)
        {
            return CreateOption(
                snapshotId,
                string.Empty,
                string.Empty,
                string.Empty,
                null);
        }

        private OpenVisionRecipeQualifiedSnapshotOption CreateOption(
            string snapshotId,
            string currentRecipeName,
            string currentPipelineName,
            string currentPipelinePath,
            OpenVisionRecipeValidationSet currentValidationSet)
        {
            QualifiedRecipeSnapshotVerificationResult verification =
                store.Verify(snapshotId);
            QualifiedRecipeSnapshotLifecycleState lifecycleState =
                lifecycle.Load(snapshotId);
            OpenVisionRecipeQualifiedSnapshotCurrentIdentity current =
                EvaluateCurrentIdentity(
                    snapshotId,
                    currentRecipeName,
                    currentPipelineName,
                    currentPipelinePath,
                    currentValidationSet,
                    null);
            QualifiedRecipeSnapshotManifest manifest = verification.Manifest;
            DateTime.TryParse(
                manifest?.CreatedAtUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime createdAt);
            string lifecycleText = lifecycleState.Success
                ? lifecycleState.State
                : "Lifecycle tampered";
            string integrity = !verification.PayloadIntegrityValid
                ? "Tampered or incomplete"
                : current.Success && lifecycleState.Success
                    ? "Payload OK / Current verification"
                    : "Payload OK / Qualification stale";
            return new OpenVisionRecipeQualifiedSnapshotOption
            {
                SnapshotId = snapshotId,
                CreatedAtUtc = createdAt,
                DisplayText =
                    (createdAt == default
                        ? "-"
                        : createdAt.ToLocalTime().ToString(
                            "MM-dd HH:mm:ss",
                            CultureInfo.CurrentCulture))
                    + " | " + lifecycleText
                    + " | " + ShortId(snapshotId),
                DetailText = (manifest?.Scope ?? "-")
                    + " | "
                    + (manifest?.Counts?.Total ?? 0).ToString(
                        CultureInfo.InvariantCulture)
                    + " rows | " + integrity,
                LifecycleState = lifecycleText,
                IntegrityState = integrity,
                PayloadIntegrityValid = verification.PayloadIntegrityValid,
                RuntimeFingerprintMatches =
                    current.RuntimeMatches,
                CurrentIdentityEvaluated = true,
                CurrentIdentityMatches = current.Success
            };
        }

        private bool CompareCurrentValidationSet(
            string snapshotId,
            QualifiedRecipeSnapshotManifest manifest,
            string currentPipelinePath,
            OpenVisionRecipeValidationSet currentValidationSet,
            OpenVisionRecipeQualifiedSnapshotCurrentIdentity result)
        {
            if (currentValidationSet == null)
            {
                result.Errors.Add("Current Validation Set is not selected.");
                return false;
            }

            string snapshotDirectory = store.GetSnapshotDirectory(snapshotId);
            if (!TryResolveSnapshotFile(
                    snapshotDirectory,
                    manifest.ValidationSetFile,
                    out string archivedValidationSetPath)
                || !SerializeHelper.TryLoadFromXmlFile(
                    archivedValidationSetPath,
                    out QualifiedRecipeValidationSetSnapshot archivedValidationSet)
                || archivedValidationSet == null)
            {
                result.Errors.Add("Qualified Validation Set identity could not be loaded.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(currentPipelinePath)
                || !File.Exists(currentPipelinePath))
            {
                result.Errors.Add("Current Validation Set cannot be compared without the Pipeline file.");
                return false;
            }

            QualifiedRecipeValidationSetSnapshot currentSnapshot;
            try
            {
                currentSnapshot = CreateValidationSetSnapshot(
                    currentValidationSet,
                    manifest.PipelineName,
                    currentPipelinePath);
                foreach (QualifiedRecipeValidationImageSource image in
                    currentSnapshot.Images ?? new List<QualifiedRecipeValidationImageSource>())
                {
                    image.Sha256 = File.Exists(image.SourcePath)
                        ? OpenVisionRecipeValidationSetStorage.ComputeFileSha256(image.SourcePath)
                        : string.Empty;
                }

                currentSnapshot.ImageSetSha256 =
                    QualifiedRecipeSnapshotPreflight.ComputeImageSetSha256(
                        currentSnapshot.Images);
                foreach (QualifiedRecipeDependencySource dependency in
                    currentSnapshot.Dependencies ?? new List<QualifiedRecipeDependencySource>())
                {
                    dependency.Sha256 = File.Exists(dependency.SourcePath)
                        ? OpenVisionRecipeValidationSetStorage.ComputeFileSha256(
                            dependency.SourcePath)
                        : string.Empty;
                }
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is IOException
                || exception is InvalidOperationException)
            {
                result.Errors.Add(
                    "Current Validation Set identity could not be computed: "
                    + exception.GetBaseException().Message);
                return false;
            }

            if (!SameValidationSetIdentity(archivedValidationSet, currentSnapshot))
            {
                result.Errors.Add("Current input bytes or Validation Set identity differs from the qualified Snapshot.");
                return false;
            }

            return true;
        }

        private static bool SameValidationSetIdentity(
            QualifiedRecipeValidationSetSnapshot expected,
            QualifiedRecipeValidationSetSnapshot actual)
        {
            List<QualifiedRecipeValidationImageSource> expectedImages =
                expected?.Images ?? new List<QualifiedRecipeValidationImageSource>();
            List<QualifiedRecipeValidationImageSource> actualImages =
                actual?.Images ?? new List<QualifiedRecipeValidationImageSource>();
            List<QualifiedRecipeDependencySource> expectedDependencies =
                expected?.Dependencies ?? new List<QualifiedRecipeDependencySource>();
            List<QualifiedRecipeDependencySource> actualDependencies =
                actual?.Dependencies ?? new List<QualifiedRecipeDependencySource>();
            if (expected == null || actual == null
                || !string.Equals(expected.Name, actual.Name, StringComparison.Ordinal)
                || !string.Equals(expected.PipelineName, actual.PipelineName, StringComparison.Ordinal)
                || !string.Equals(expected.PipelineDefinitionSha256, actual.PipelineDefinitionSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(expected.ImageSetSha256, actual.ImageSetSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(expected.Notes, actual.Notes, StringComparison.Ordinal)
                || expectedImages.Count != actualImages.Count
                || expectedDependencies.Count != actualDependencies.Count)
            {
                return false;
            }

            for (int index = 0; index < expectedImages.Count; index++)
            {
                QualifiedRecipeValidationImageSource left = expectedImages[index];
                QualifiedRecipeValidationImageSource right = actualImages[index];
                if (!string.Equals(left.ExpectedOutcome, right.ExpectedOutcome, StringComparison.Ordinal)
                    || !SameFullPath(left.SourcePath, right.SourcePath)
                    || !string.Equals(left.Sha256, right.Sha256, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(left.VariantId, right.VariantId, StringComparison.Ordinal)
                    || !string.Equals(left.ExpectedMetricName, right.ExpectedMetricName, StringComparison.Ordinal)
                    || !string.Equals(left.ExpectedMetricMinimum, right.ExpectedMetricMinimum, StringComparison.Ordinal)
                    || !string.Equals(left.ExpectedMetricMaximum, right.ExpectedMetricMaximum, StringComparison.Ordinal)
                    || !string.Equals(left.Notes, right.Notes, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            for (int index = 0; index < expectedDependencies.Count; index++)
            {
                QualifiedRecipeDependencySource left = expectedDependencies[index];
                QualifiedRecipeDependencySource right = actualDependencies[index];
                if (!SameFullPath(left.SourcePath, right.SourcePath)
                    || !string.Equals(left.Sha256, right.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CompareCurrentRuntime(
            QualifiedRecipeSnapshotManifest manifest,
            IReadOnlyList<QualifiedRecipeRuntimeFileSource> currentSources,
            OpenVisionRecipeQualifiedSnapshotCurrentIdentity result)
        {
            List<QualifiedRecipeRuntimeFingerprint> current =
                new List<QualifiedRecipeRuntimeFingerprint>();
            HashSet<string> labels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (QualifiedRecipeRuntimeFileSource source in
                currentSources ?? Array.Empty<QualifiedRecipeRuntimeFileSource>())
            {
                if (source == null || !labels.Add(source.Label ?? string.Empty))
                {
                    result.Errors.Add("Current runtime fingerprint labels are invalid or duplicated.");
                    continue;
                }

                if (!File.Exists(source.SourcePath))
                {
                    result.Errors.Add("Current runtime fingerprint file is missing: " + source.Label);
                    continue;
                }

                FileInfo info = new FileInfo(source.SourcePath);
                current.Add(new QualifiedRecipeRuntimeFingerprint
                {
                    Label = source.Label.Trim(),
                    SourcePath = info.FullName,
                    FileVersion = FileVersionInfo.GetVersionInfo(info.FullName).FileVersion
                        ?? string.Empty,
                    Size = info.Length,
                    Sha256 = QualifiedRecipeSnapshotPreflight.ComputeFileSha256(info.FullName)
                });
            }

            List<QualifiedRecipeRuntimeFingerprint> expected =
                manifest.RuntimeFingerprint ?? new List<QualifiedRecipeRuntimeFingerprint>();
            bool matches = expected.Count == current.Count;
            foreach (QualifiedRecipeRuntimeFingerprint expectedRuntime in expected)
            {
                QualifiedRecipeRuntimeFingerprint actual = current.FirstOrDefault(item =>
                    string.Equals(item.Label, expectedRuntime.Label, StringComparison.OrdinalIgnoreCase));
                if (actual == null
                    || !SameFullPath(actual.SourcePath, expectedRuntime.SourcePath)
                    || !string.Equals(actual.FileVersion, expectedRuntime.FileVersion, StringComparison.Ordinal)
                    || actual.Size != expectedRuntime.Size
                    || !string.Equals(actual.Sha256, expectedRuntime.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    matches = false;
                    result.Errors.Add("Current SDK/runtime identity differs: " + expectedRuntime.Label);
                }
            }

            if (!matches && !result.Errors.Any(error =>
                    error.StartsWith("Current SDK/runtime identity differs", StringComparison.Ordinal)))
            {
                result.Errors.Add("Current SDK/runtime identity set differs from the qualified Snapshot.");
            }

            return matches;
        }

        private static bool TryResolveSnapshotFile(
            string snapshotDirectory,
            string archivePath,
            out string path)
        {
            path = string.Empty;
            if (string.IsNullOrWhiteSpace(snapshotDirectory)
                || string.IsNullOrWhiteSpace(archivePath)
                || Path.IsPathRooted(archivePath))
            {
                return false;
            }

            try
            {
                string root = Path.GetFullPath(snapshotDirectory)
                    .TrimEnd(Path.DirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                string candidate = Path.GetFullPath(
                    Path.Combine(
                        root,
                        archivePath.Replace('/', Path.DirectorySeparatorChar)));
                if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                    || !File.Exists(candidate))
                {
                    return false;
                }

                path = candidate;
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is IOException
                || exception is NotSupportedException)
            {
                return false;
            }
        }

        private static bool SameFullPath(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
            {
                return string.IsNullOrWhiteSpace(left) && string.IsNullOrWhiteSpace(right);
            }

            try
            {
                return string.Equals(
                    Path.GetFullPath(left),
                    Path.GetFullPath(right),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is IOException
                || exception is NotSupportedException)
            {
                return false;
            }
        }

        private static QualifiedRecipeValidationSetSnapshot
            CreateValidationSetSnapshot(
                OpenVisionRecipeValidationSet set,
                string pipelineName,
                string pipelinePath)
        {
            string pipelineXml = File.ReadAllText(pipelinePath);
            List<QualifiedRecipeValidationImageSource> images =
                (set.Images
                    ?? new List<OpenVisionRecipeValidationSetImage>())
                    .Where(item => item != null)
                    .Select(item =>
                        new QualifiedRecipeValidationImageSource
                        {
                            ExpectedOutcome =
                                item.Expected ?? string.Empty,
                            SourcePath = item.Path ?? string.Empty,
                            Sha256 = !string.IsNullOrWhiteSpace(item.Sha256)
                                ? item.Sha256
                                : File.Exists(item.Path)
                                    ? OpenVisionRecipeValidationSetStorage
                                        .ComputeFileSha256(item.Path)
                                    : string.Empty,
                            Notes = item.Notes ?? string.Empty,
                            VariantId = item.VariantId ?? string.Empty,
                            ExpectedMetricName = item.ExpectedMetricName ?? string.Empty,
                            ExpectedMetricMinimum = item.ExpectedMetricMinimum ?? string.Empty,
                            ExpectedMetricMaximum = item.ExpectedMetricMaximum ?? string.Empty
                        })
                    .ToList();
            string imageSetSha256 =
                OpenVisionRecipeValidationSetStorage.ComputeImageSetSha256(
                    images.Select(item =>
                        new OpenVisionRecipeValidationSetImage
                        {
                            Path = item.SourcePath,
                            Sha256 = item.Sha256,
                            VariantId = item.VariantId,
                            ExpectedMetricName = item.ExpectedMetricName,
                            ExpectedMetricMinimum = item.ExpectedMetricMinimum,
                            ExpectedMetricMaximum = item.ExpectedMetricMaximum
                        }));
            return new QualifiedRecipeValidationSetSnapshot
            {
                Name = set.Name ?? string.Empty,
                PipelineName = pipelineName ?? string.Empty,
                PipelineDefinitionSha256 =
                    OpenVisionRecipeValidationSetStorage.ComputeTextSha256(
                        pipelineXml),
                ImageSetSha256 =
                    imageSetSha256,
                Notes = set.Notes ?? string.Empty,
                Dependencies = (set.Dependencies
                    ?? new List<OpenVisionRecipeValidationSetDependency>())
                    .Where(item => item != null)
                    .Select(item => new QualifiedRecipeDependencySource
                    {
                        LogicalPath = item.Path ?? string.Empty,
                        SourcePath = item.Path ?? string.Empty,
                        Sha256 = !string.IsNullOrWhiteSpace(item.Sha256)
                            ? item.Sha256
                            : File.Exists(item.Path)
                                ? OpenVisionRecipeValidationSetStorage
                                    .ComputeFileSha256(item.Path)
                                : string.Empty
                    })
                    .ToList(),
                Images = images
            };
        }

        private static List<QualifiedRecipeRuntimeFileSource>
            CreateRuntimeFingerprintSources()
        {
            List<QualifiedRecipeRuntimeFileSource> files =
                new List<QualifiedRecipeRuntimeFileSource>
                {
                    new QualifiedRecipeRuntimeFileSource
                    {
                        Label = "OpenVisionLab",
                        SourcePath =
                            typeof(OpenVisionShellHostRecipeCommandSurface)
                            .Assembly.Location
                    },
                    new QualifiedRecipeRuntimeFileSource
                    {
                        Label = "OpenVisionLab.Vision2D.dll",
                        SourcePath = typeof(VisionPipeline).Assembly.Location
                    },
                    new QualifiedRecipeRuntimeFileSource
                    {
                        Label = "OpenCvSharp.dll",
                        SourcePath = typeof(Mat).Assembly.Location
                    }
                };
            string externPath = Path.Combine(
                AppPathService.StartupPath,
                "OpenCvSharpExtern.dll");
            if (File.Exists(externPath))
            {
                files.Add(new QualifiedRecipeRuntimeFileSource
                {
                    Label = "OpenCvSharpExtern.dll",
                    SourcePath = externPath
                });
            }

            return files;
        }

        private static OpenVisionRecipeQualifiedSnapshotActionResult Failure(
            string error)
        {
            return new OpenVisionRecipeQualifiedSnapshotActionResult
            {
                Success = false,
                Message = error ?? string.Empty
            };
        }

        private static string ShortId(string value)
        {
            string normalized = value?.Trim() ?? string.Empty;
            return normalized.Length <= 12
                ? normalized
                : normalized.Substring(0, 12);
        }
    }

    internal sealed class OpenVisionRecipeQualificationEvaluation
    {
        internal bool Success => Errors.Count == 0 && Request != null;
        internal QualifiedRecipeSnapshotCreateRequest Request { get; set; }
        internal QualifiedRecipeQualificationCounts Counts { get; set; } =
            new QualifiedRecipeQualificationCounts();
        internal List<string> Errors { get; } = new List<string>();
    }

    internal sealed class OpenVisionRecipeQualifiedSnapshotActionResult
    {
        internal bool Success { get; set; }
        internal string SnapshotId { get; set; } = string.Empty;
        internal string RecipeName { get; set; } = string.Empty;
        internal string Message { get; set; } = string.Empty;
    }

    internal sealed class OpenVisionRecipeQualifiedSnapshotCurrentIdentity
    {
        internal bool Success =>
            Errors.Count == 0
            && RecipeMatches
            && InputMatches
            && RuntimeMatches;

        internal bool RecipeMatches { get; set; }
        internal bool InputMatches { get; set; }
        internal bool RuntimeMatches { get; set; }
        internal List<string> Errors { get; } = new List<string>();
    }

    public sealed class OpenVisionRecipeQualificationScopeOption
    {
        internal OpenVisionRecipeQualificationScopeOption(
            QualifiedRecipeSnapshotScope scope,
            string displayText,
            string claimText)
        {
            Scope = scope;
            DisplayText = displayText ?? string.Empty;
            ClaimText = claimText ?? string.Empty;
        }

        internal QualifiedRecipeSnapshotScope Scope { get; }
        public string DisplayText { get; }
        public string ClaimText { get; }

        internal static IReadOnlyList<OpenVisionRecipeQualificationScopeOption>
            CreateDefaults()
        {
            return new[]
            {
                new OpenVisionRecipeQualificationScopeOption(
                    QualifiedRecipeSnapshotScope.InspectionJudgment,
                    OpenVisionRecipeText.Local(
                        "검사 판정",
                        "Inspection judgment"),
                    OpenVisionRecipeText.Local(
                        "이 고정 OK/NG Validation Set에 대해서만 자격화합니다.",
                        "Qualified only for this frozen OK/NG Validation Set.")),
                new OpenVisionRecipeQualificationScopeOption(
                    QualifiedRecipeSnapshotScope.LocatorStability,
                    OpenVisionRecipeText.Local(
                        "Locator 안정성",
                        "Locator stability"),
                    OpenVisionRecipeText.Local(
                        "이 고정 Expected-OK Set의 Locator 실행에 대해서만 자격화합니다.",
                        "Qualified only for locator execution on this frozen Expected-OK Set."))
            };
        }
    }

    public sealed class OpenVisionRecipeQualifiedSnapshotOption
    {
        public string SnapshotId { get; internal set; } = string.Empty;
        public DateTime CreatedAtUtc { get; internal set; }
        public string DisplayText { get; internal set; } = string.Empty;
        public string DetailText { get; internal set; } = string.Empty;
        public string LifecycleState { get; internal set; } = string.Empty;
        public string IntegrityState { get; internal set; } = string.Empty;
        public bool PayloadIntegrityValid { get; internal set; }
        public bool RuntimeFingerprintMatches { get; internal set; }
        public bool CurrentIdentityEvaluated { get; internal set; }
        public bool CurrentIdentityMatches { get; internal set; }
    }
}
