using OpenVisionLab.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private readonly IReadOnlyList<OpenVisionRecipeQualificationScopeOption>
            qualificationScopeOptions =
                OpenVisionRecipeQualificationScopeOption.CreateDefaults();
        private OpenVisionRecipeQualificationScopeOption
            selectedQualificationScopeOption;
        private IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption>
            qualifiedSnapshotOptions =
                Array.Empty<OpenVisionRecipeQualifiedSnapshotOption>();
        private OpenVisionRecipeQualifiedSnapshotOption
            selectedQualifiedSnapshotOption;
        private string qualificationNote = string.Empty;
        private string qualifiedSnapshotLifecycleReason = string.Empty;
        private string qualifiedSnapshotWorkingCopyName = "Qualified_Working_Copy";
        private string qualifiedSnapshotStatusText = string.Empty;

        public IReadOnlyList<OpenVisionRecipeQualificationScopeOption>
            QualificationScopeOptions => qualificationScopeOptions;

        public OpenVisionRecipeQualificationScopeOption
            SelectedQualificationScopeOption
        {
            get => selectedQualificationScopeOption;
            set
            {
                if (SetProperty(
                        ref selectedQualificationScopeOption,
                        value ?? qualificationScopeOptions.FirstOrDefault()))
                {
                    OnPropertyChanged(nameof(QualificationScopeClaimText));
                    NotifyQualifiedSnapshotContextChanged();
                }
            }
        }

        public IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption>
            QualifiedSnapshotOptions
        {
            get => qualifiedSnapshotOptions;
            private set => SetProperty(
                ref qualifiedSnapshotOptions,
                value ?? Array.Empty<OpenVisionRecipeQualifiedSnapshotOption>());
        }

        public OpenVisionRecipeQualifiedSnapshotOption
            SelectedQualifiedSnapshotOption
        {
            get => selectedQualifiedSnapshotOption;
            set
            {
                if (SetProperty(ref selectedQualifiedSnapshotOption, value))
                {
                    OnPropertyChanged(
                        nameof(SelectedQualifiedSnapshotDetailText));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualificationNote
        {
            get => qualificationNote;
            set
            {
                if (SetProperty(ref qualificationNote, value ?? string.Empty))
                {
                    NotifyQualifiedSnapshotContextChanged();
                }
            }
        }

        public string QualifiedSnapshotLifecycleReason
        {
            get => qualifiedSnapshotLifecycleReason;
            set
            {
                if (SetProperty(
                        ref qualifiedSnapshotLifecycleReason,
                        value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualifiedSnapshotWorkingCopyName
        {
            get => qualifiedSnapshotWorkingCopyName;
            set
            {
                if (SetProperty(
                        ref qualifiedSnapshotWorkingCopyName,
                        value ?? string.Empty))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string QualifiedSnapshotStatusText
        {
            get => string.IsNullOrWhiteSpace(qualifiedSnapshotStatusText)
                ? LocalText(
                    "선택 실행의 불변 증거를 확인한 뒤 명시적으로 Snapshot을 생성하세요.",
                    "Review the selected run evidence, then explicitly create a Snapshot.")
                : qualifiedSnapshotStatusText;
            private set => SetProperty(
                ref qualifiedSnapshotStatusText,
                value ?? string.Empty);
        }

        public string QualificationScopeClaimText =>
            SelectedQualificationScopeOption?.ClaimText
            ?? LocalText(
                "자격 범위를 선택하세요.",
                "Select a qualification scope.");

        public string QualifiedSnapshotPreflightText
        {
            get
            {
                if (IsSelectedStepEditDirty)
                {
                    return LocalText(
                        "차단: 저장하지 않은 Step 편집을 먼저 적용하거나 취소하세요.",
                        "Blocked: apply or discard the pending Step edit first.");
                }

                VisionPipelineBatchRunSummary summary =
                    SelectedRecentBatchRunOption?.RunSummary;
                if (summary == null
                    || string.IsNullOrWhiteSpace(
                        SelectedRecentBatchRunOption?.SummaryPath))
                {
                    return LocalText(
                        "차단: 저장된 Local Validation Set 실행을 선택하세요.",
                        "Blocked: select a saved Local Validation Set run.");
                }

                if (!string.Equals(
                        summary.SuiteKind,
                        "LocalValidationSet",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return LocalText(
                        "차단: 부분/일반 실행은 자격 증거가 아닙니다.",
                        "Blocked: partial or generic runs are not qualification evidence.");
                }

                if (SelectedValidationSetOption?.Set == null
                    || !string.Equals(
                        SelectedValidationSetOption.Set.Name,
                        summary.SuiteName,
                        StringComparison.Ordinal)
                    || SelectedValidationSetOption.Set.IsIdentityLocked
                        && !string.Equals(
                        SelectedValidationSetOption.Set.PipelineName,
                        SelectedPipelineOption?.PipelineName,
                        StringComparison.Ordinal))
                {
                    return LocalText(
                        "차단: 실행과 동일한 Validation Set/Pipeline을 선택하세요.",
                        "Blocked: select the Validation Set/Pipeline used by the run.");
                }

                if (string.IsNullOrWhiteSpace(QualificationNote))
                {
                    return LocalText(
                        "차단: 운영자 자격 메모를 입력하세요.",
                        "Blocked: enter an operator qualification note.");
                }

                string counts = summary.TotalCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " rows | correct "
                    + summary.JudgmentCorrectCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | FA "
                    + summary.FalseAcceptCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | FR "
                    + summary.FalseRejectCount.ToString(
                        CultureInfo.InvariantCulture)
                    + " | errors "
                    + summary.ExecutionErrorCount.ToString(
                        CultureInfo.InvariantCulture);
                return LocalText(
                    "준비: 생성 시 모든 해시·보고서·도면·검토 큐를 정확히 재검증합니다. ",
                    "Ready: Create will exactly reverify all hashes, reports, drawings, and the review queue. ")
                    + counts;
            }
        }

        public string SelectedQualifiedSnapshotDetailText =>
            SelectedQualifiedSnapshotOption == null
                ? LocalText(
                    "Snapshot을 선택하면 무결성, runtime, 수명주기 상태가 표시됩니다.",
                    "Select a Snapshot to show integrity, runtime, and lifecycle state.")
                : SelectedQualifiedSnapshotOption.DetailText
                    + " | ID "
                    + SelectedQualifiedSnapshotOption.SnapshotId;

        public string QualifiedRecipeSnapshotText =>
            LocalText("자격 Recipe Snapshot", "Qualified Recipe Snapshot");

        public string QualificationScopeText =>
            LocalText("자격 범위", "Qualification scope");

        public string QualificationNoteText =>
            LocalText("운영자 메모", "Operator note");

        public string CreateQualifiedSnapshotText =>
            LocalText("Snapshot 생성", "Create Snapshot");

        public string VerifyQualifiedSnapshotText =>
            LocalText("무결성 확인", "Verify integrity");

        public string OpenQualifiedSnapshotEvidenceText =>
            LocalText("증거 열기", "Open evidence");

        public string CreateQualifiedSnapshotWorkingCopyText =>
            LocalText("작업 복사본", "Working copy");

        public string SupersedeQualifiedSnapshotText =>
            LocalText("대체 생성", "Supersede");

        public string RevokeQualifiedSnapshotText =>
            LocalText("폐기 기록", "Revoke");

        public string RefreshQualifiedSnapshotsText =>
            LocalText("새로고침", "Refresh");

        public string QualifiedSnapshotLifecycleReasonText =>
            LocalText("대체/폐기 사유", "Supersede/revoke reason");

        public string QualifiedSnapshotWorkingCopyNameText =>
            LocalText("작업 Recipe 이름", "Working Recipe name");

        public ICommand CreateQualifiedSnapshotCommand { get; private set; }
        public ICommand VerifyQualifiedSnapshotCommand { get; private set; }
        public ICommand OpenQualifiedSnapshotEvidenceCommand { get; private set; }
        public ICommand CreateQualifiedSnapshotWorkingCopyCommand { get; private set; }
        public ICommand SupersedeQualifiedSnapshotCommand { get; private set; }
        public ICommand RevokeQualifiedSnapshotCommand { get; private set; }
        public ICommand RefreshQualifiedSnapshotsCommand { get; private set; }

        private void InitializeQualifiedSnapshotCommands()
        {
            selectedQualificationScopeOption =
                qualificationScopeOptions.FirstOrDefault();
            CreateQualifiedSnapshotCommand = new RelayCommand(
                CreateQualifiedSnapshot,
                CanCreateQualifiedSnapshot);
            VerifyQualifiedSnapshotCommand = new RelayCommand(
                VerifyQualifiedSnapshot,
                CanUseSelectedQualifiedSnapshot);
            OpenQualifiedSnapshotEvidenceCommand = new RelayCommand(
                OpenQualifiedSnapshotEvidence,
                CanOpenQualifiedSnapshotEvidence);
            CreateQualifiedSnapshotWorkingCopyCommand = new RelayCommand(
                CreateQualifiedSnapshotWorkingCopy,
                CanCreateQualifiedSnapshotWorkingCopy);
            SupersedeQualifiedSnapshotCommand = new RelayCommand(
                SupersedeQualifiedSnapshot,
                CanSupersedeQualifiedSnapshot);
            RevokeQualifiedSnapshotCommand = new RelayCommand(
                RevokeQualifiedSnapshot,
                CanRevokeQualifiedSnapshot);
            RefreshQualifiedSnapshotsCommand = new RelayCommand(
                () => RefreshQualifiedSnapshotOptions());
        }

        private void CreateQualifiedSnapshot()
        {
            OpenVisionRecipeQualificationEvaluation evaluation =
                EvaluateQualifiedSnapshot();
            if (!evaluation.Success)
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    string.Join(" | ", evaluation.Errors));
                return;
            }

            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Create(evaluation);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(result.SnapshotId);
        }

        private void SupersedeQualifiedSnapshot()
        {
            string predecessor =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            string reason = QualifiedSnapshotLifecycleReason.Trim();
            if (!confirmQualifiedSnapshotLifecycle(
                    predecessor,
                    "Superseded",
                    reason))
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    LocalText(
                        "대체 작업을 취소했습니다.",
                        "Supersede was cancelled."));
                return;
            }

            OpenVisionRecipeQualificationEvaluation evaluation =
                qualifiedSnapshotController.Evaluate(
                    NormalizeRecipeName(selectedRecipeName),
                    SelectedPipelineOption?.PipelineName ?? string.Empty,
                    SelectedRecentBatchRunOption,
                    SelectedValidationSetOption,
                    SelectedQualificationScopeOption,
                    QualificationNote,
                    IsSelectedStepEditDirty,
                    predecessor,
                    reason);
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Supersede(
                    evaluation,
                    predecessor,
                    reason);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(
                result.Success ? result.SnapshotId : predecessor);
        }

        private void RevokeQualifiedSnapshot()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            string reason = QualifiedSnapshotLifecycleReason.Trim();
            if (!confirmQualifiedSnapshotLifecycle(
                    snapshotId,
                    "Revoked",
                    reason))
            {
                SetQualifiedSnapshotActionStatus(
                    false,
                    LocalText(
                        "폐기 기록을 취소했습니다.",
                        "Revoke was cancelled."));
                return;
            }

            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Revoke(snapshotId, reason);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(snapshotId);
        }

        private void VerifyQualifiedSnapshot()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.Verify(snapshotId);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            RefreshQualifiedSnapshotOptions(snapshotId);
        }

        private void OpenQualifiedSnapshotEvidence()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            if (!qualifiedSnapshotController.TryGetEvidenceDirectory(
                    snapshotId,
                    out string directory,
                    out string error))
            {
                SetQualifiedSnapshotActionStatus(false, error);
                return;
            }

            bool opened = openQualifiedSnapshotEvidence(directory);
            SetQualifiedSnapshotActionStatus(
                opened,
                opened
                    ? LocalText(
                        "Snapshot 증거 폴더를 열었습니다: ",
                        "Opened Snapshot evidence folder: ")
                        + directory
                    : LocalText(
                        "Snapshot 증거 폴더를 열지 못했습니다: ",
                        "Could not open Snapshot evidence folder: ")
                        + directory);
        }

        private void CreateQualifiedSnapshotWorkingCopy()
        {
            string snapshotId =
                SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty;
            OpenVisionRecipeQualifiedSnapshotActionResult result =
                qualifiedSnapshotController.CreateWorkingCopy(
                    snapshotId,
                    QualifiedSnapshotWorkingCopyName);
            SetQualifiedSnapshotActionStatus(result.Success, result.Message);
            if (result.Success)
            {
                RefreshOptions();
            }
        }

        private OpenVisionRecipeQualificationEvaluation
            EvaluateQualifiedSnapshot()
        {
            return qualifiedSnapshotController.Evaluate(
                NormalizeRecipeName(selectedRecipeName),
                SelectedPipelineOption?.PipelineName ?? string.Empty,
                SelectedRecentBatchRunOption,
                SelectedValidationSetOption,
                SelectedQualificationScopeOption,
                QualificationNote,
                IsSelectedStepEditDirty);
        }

        private bool CanCreateQualifiedSnapshot()
        {
            VisionPipelineBatchRunSummary summary =
                SelectedRecentBatchRunOption?.RunSummary;
            return !IsSelectedStepEditDirty
                && !executionSession.IsValidationSuiteRunning
                && summary != null
                && string.Equals(
                    summary.SuiteKind,
                    "LocalValidationSet",
                    StringComparison.OrdinalIgnoreCase)
                && SelectedValidationSetOption?.Set != null
                && string.Equals(
                    SelectedValidationSetOption.Set.Name,
                    summary.SuiteName,
                    StringComparison.Ordinal)
                && (!SelectedValidationSetOption.Set.IsIdentityLocked
                    || string.Equals(
                        SelectedValidationSetOption.Set.PipelineName,
                        SelectedPipelineOption?.PipelineName,
                        StringComparison.Ordinal))
                && !string.IsNullOrWhiteSpace(QualificationNote)
                && File.Exists(SelectedRecentBatchRunOption.SummaryPath);
        }

        private bool CanUseSelectedQualifiedSnapshot()
        {
            return SelectedQualifiedSnapshotOption != null
                && !string.IsNullOrWhiteSpace(
                    SelectedQualifiedSnapshotOption.SnapshotId);
        }

        private bool CanOpenQualifiedSnapshotEvidence()
        {
            return CanUseSelectedQualifiedSnapshot()
                && SelectedQualifiedSnapshotOption.PayloadIntegrityValid;
        }

        private bool CanCreateQualifiedSnapshotWorkingCopy()
        {
            return CanOpenQualifiedSnapshotEvidence()
                && RecipeWorkspaceService.IsValidRecipeName(
                    QualifiedSnapshotWorkingCopyName)
                && !RecipeWorkspaceService.GetRecipeNames().Contains(
                    QualifiedSnapshotWorkingCopyName.Trim(),
                    StringComparer.OrdinalIgnoreCase);
        }

        private bool CanSupersedeQualifiedSnapshot()
        {
            return CanUseSelectedQualifiedSnapshot()
                && string.Equals(
                    SelectedQualifiedSnapshotOption.LifecycleState,
                    "Qualified",
                    StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(
                    QualifiedSnapshotLifecycleReason)
                && CanCreateQualifiedSnapshot();
        }

        private bool CanRevokeQualifiedSnapshot()
        {
            return CanUseSelectedQualifiedSnapshot()
                && string.Equals(
                    SelectedQualifiedSnapshotOption.LifecycleState,
                    "Qualified",
                    StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(
                    QualifiedSnapshotLifecycleReason);
        }

        private void RefreshQualifiedSnapshotOptions(
            string preferredSnapshotId = "")
        {
            string preferred = string.IsNullOrWhiteSpace(preferredSnapshotId)
                ? SelectedQualifiedSnapshotOption?.SnapshotId ?? string.Empty
                : preferredSnapshotId;
            IReadOnlyList<OpenVisionRecipeQualifiedSnapshotOption> options =
                qualifiedSnapshotController.List();
            QualifiedSnapshotOptions = options;
            SelectedQualifiedSnapshotOption = options.FirstOrDefault(option =>
                    string.Equals(
                        option.SnapshotId,
                        preferred,
                        StringComparison.OrdinalIgnoreCase))
                ?? options.FirstOrDefault();
            OnPropertyChanged(nameof(SelectedQualifiedSnapshotDetailText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void NotifyQualifiedSnapshotContextChanged()
        {
            OnPropertyChanged(nameof(QualifiedSnapshotPreflightText));
            OnPropertyChanged(nameof(QualificationScopeClaimText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetQualifiedSnapshotActionStatus(
            bool success,
            string message)
        {
            QualifiedSnapshotStatusText =
                (success ? "OK | " : "BLOCKED | ")
                + (message ?? string.Empty);
            StatusText = QualifiedSnapshotStatusText;
        }

        internal void DiscardSelectedStepEditForQualificationTest()
        {
            ClearSelectedStepEdit();
            NotifyQualifiedSnapshotContextChanged();
            RefreshCommandState();
        }

        internal void MarkSelectedStepEditDirtyForQualificationTest()
        {
            selectedStepEditSession.MarkDirty(
                "Qualification test pending Step edit.");
            NotifyQualifiedSnapshotContextChanged();
            RefreshCommandState();
        }
    }
}
