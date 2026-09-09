using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void LoadLlmXmlDraft()
        {
            string path = selectImportPipelineXmlPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText = LocalText("LLM XML 초안 로드가 취소되었습니다.", "LLM XML draft load canceled.");
                return;
            }

            LoadLlmXmlDraftFromPath(path);
        }

        private void LoadLocatorEvidencePacket()
        {
            string path = selectLocatorEvidencePacketPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 로드가 취소되었습니다.",
                    "Evidence Packet load canceled.");
                return;
            }

            LoadLocatorEvidencePacketFromPath(path);
        }

        public bool LoadLocatorEvidencePacketFromPath(string path)
        {
            string normalizedPath;
            try
            {
                normalizedPath = string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : Path.GetFullPath(path.Trim());
            }
            catch (Exception exception)
            {
                LocatorEvidencePacketPath = path ?? string.Empty;
                loadedLocatorEvidencePacket = null;
                loadedLocatorEvidencePacketPath = string.Empty;
                locatorEvidenceCompilationReady = false;
                LocatorEvidenceOverlayImage = null;
                ClearLoadedLocatorEvidenceReviewDecision();
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 경로가 잘못되었습니다: ",
                    "The Evidence Packet path is invalid: ") + exception.GetBaseException().Message;
                LocatorEvidenceReviewText = LocalText(
                    "유효한 .packet.json 경로를 선택하세요.",
                    "Select a valid .packet.json path.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }
            LocatorEvidencePacketPath = normalizedPath;
            loadedLocatorEvidencePacket = null;
            loadedLocatorEvidencePacketPath = string.Empty;
            locatorEvidenceCompilationReady = false;
            LocatorEvidenceOverlayImage = null;
            ClearLoadedLocatorEvidenceReviewDecision();

            if (!OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                    normalizedPath,
                    out OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
                    out string message))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 검증 NG: ",
                    "Evidence Packet validation failed: ") + message;
                LocatorEvidenceReviewText = LocalText(
                    "해시·Candidate·좌표계 검증에 실패했습니다. Packet을 가져오지 않았고 실행하지 않았습니다.",
                    "Hash, candidate, or coordinate-frame validation failed. The packet was not applied and nothing was executed.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            try
            {
                BitmapImage overlay = LoadLocatorEvidenceOverlay(packet.PreviewOverlayPath);
                loadedLocatorEvidencePacket = packet;
                loadedLocatorEvidencePacketPath = normalizedPath;
                LocatorEvidenceOverlayImage = overlay;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(packet);
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet 검증 OK. Compile을 눌러 현재 설정과 대조하세요. Preview/Run은 실행하지 않았습니다.",
                    "Evidence Packet verified. Select Compile to compare it with the current settings. Preview/Run was not executed.");
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Packet 검증 OK. review decision 파일을 로드해 후보를 명시적으로 검토하세요.",
                    "Packet verified. Load its review decision file for explicit candidate review.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return true;
            }
            catch (Exception exception)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet overlay를 표시할 수 없습니다: ",
                    "The Evidence Packet overlay could not be displayed: ") + exception.GetBaseException().Message;
                LocatorEvidenceReviewText = LocalText(
                    "검토용 overlay 디코딩에 실패했습니다. Packet을 적용하지 않았고 실행하지 않았습니다.",
                    "The review overlay could not be decoded. The packet was not applied and nothing was executed.");
                OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }
        }

        private void LoadLocatorEvidenceReviewDecision()
        {
            string path = selectLocatorEvidenceReviewDecisionPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 로드가 취소되었습니다.",
                    "Review decision load canceled.");
                return;
            }

            LoadLocatorEvidenceReviewDecisionFromPath(path);
        }

        public bool LoadLocatorEvidenceReviewDecisionFromPath(string path)
        {
            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "먼저 검증된 Evidence Packet을 로드하세요.",
                    "Load a verified Evidence Packet first.");
                return false;
            }

            string normalizedPath;
            try
            {
                normalizedPath = string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : Path.GetFullPath(path.Trim());
            }
            catch (Exception exception)
            {
                ClearLoadedLocatorEvidenceReviewDecision();
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 경로가 잘못되었습니다: ",
                    "The review decision path is invalid: ") + exception.GetBaseException().Message;
                RefreshCommandState();
                return false;
            }

            ClearLoadedLocatorEvidenceReviewDecision();
            LocatorEvidenceReviewDecisionPath = normalizedPath;
            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    normalizedPath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision decision,
                    out string message))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 검증 NG: ",
                    "Review decision validation failed: ") + message;
                RefreshCommandState();
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "현재 Guided Setup 설정과 review decision을 대조할 수 없습니다: ",
                    "The review decision cannot be compared with the current Guided Setup settings: ") + planMessage;
                RefreshCommandState();
                return false;
            }

            if (!decision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string currentMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision이 현재 Packet 또는 설정과 일치하지 않습니다: ",
                    "The review decision does not match the current packet or settings: ") + currentMessage;
                RefreshCommandState();
                return false;
            }

            loadedLocatorEvidenceReviewDecision = decision;
            loadedLocatorEvidenceReviewDecisionPath = normalizedPath;
            LocatorEvidenceVisualCorrespondence = decision.VisualCorrespondence;
            LocatorEvidenceReviewer = decision.Reviewer;
            LocatorEvidenceReviewNotes = string.Equals(
                decision.Decision,
                OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pending,
                StringComparison.Ordinal)
                ? string.Empty
                : decision.Notes;
            LocatorEvidenceReviewDecisionStatusText = BuildLocatorEvidenceReviewDecisionStatus(decision);
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
            RefreshCommandState();
            return true;
        }

        private void ApproveLocatorEvidenceReviewDecision()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved);
        }

        private void RejectLocatorEvidenceReviewDecision()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.Rejected);
        }

        private void RequestLocatorEvidenceReplacement()
        {
            RecordLocatorEvidenceReviewDecision(OpenVisionRecipeLocatorRelativeBlobReviewDecision.ReplacementRequested);
        }

        public bool RecordLocatorEvidenceReviewDecision(string decision)
        {
            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath)
                || loadedLocatorEvidenceReviewDecision == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "검토할 Packet과 review decision을 먼저 로드하세요.",
                    "Load the packet and review decision before recording a review.");
                return false;
            }

            if (string.Equals(
                    LocatorEvidenceVisualCorrespondence,
                    OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed,
                    StringComparison.Ordinal))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Visual correspondence에서 PASS 또는 FAIL을 선택하세요.",
                    "Select PASS or FAIL for visual correspondence.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LocatorEvidenceReviewer)
                || string.IsNullOrWhiteSpace(LocatorEvidenceReviewNotes))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Reviewer와 검토 Notes를 입력하세요.",
                    "Enter a reviewer and review notes.");
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "현재 Guided Setup 설정이 검토 결정과 일치하지 않습니다: ",
                    "The current Guided Setup settings cannot be matched to the review decision: ") + planMessage;
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string beforeApplyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "기존 review decision이 stale 상태입니다. 최신 Packet/결정을 다시 로드하세요: ",
                    "The existing review decision is stale. Reload the current packet/decision: ") + beforeApplyMessage;
                RefreshCommandState();
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryApplyOperatorDecision(
                    decision,
                    LocatorEvidenceVisualCorrespondence,
                    LocatorEvidenceReviewer.Trim(),
                    LocatorEvidenceReviewNotes.Trim(),
                    DateTimeOffset.UtcNow,
                    out string applyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision을 기록할 수 없습니다: ",
                    "The review decision could not be recorded: ")
                    + applyMessage;
                RefreshCommandState();
                return false;
            }

            if (!loadedLocatorEvidenceReviewDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string afterApplyMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision을 기록할 수 없습니다: ",
                    "The review decision could not be recorded: ")
                    + afterApplyMessage;
                RefreshCommandState();
                return false;
            }

            string savePath = ResolveLocatorEvidenceReviewDecisionSavePath(loadedLocatorEvidenceReviewDecisionPath);
            if (!loadedLocatorEvidenceReviewDecision.TrySave(savePath, out string saveMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "review decision 저장 NG. Recipe 승격은 계속 차단됩니다: ",
                    "Review decision save failed. Recipe promotion remains blocked: ") + saveMessage;
                RefreshCommandState();
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    savePath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision persistedDecision,
                    out string reloadMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "저장된 review decision 재검증 NG. Recipe 승격은 차단됩니다: ",
                    "The saved review decision failed reload validation. Recipe promotion remains blocked: ")
                    + reloadMessage;
                RefreshCommandState();
                return false;
            }

            if (!persistedDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string persistedValidationMessage))
            {
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "저장된 review decision 재검증 NG. Recipe 승격은 차단됩니다: ",
                    "The saved review decision failed reload validation. Recipe promotion remains blocked: ")
                    + persistedValidationMessage;
                RefreshCommandState();
                return false;
            }

            loadedLocatorEvidenceReviewDecision = persistedDecision;
            loadedLocatorEvidenceReviewDecisionPath = savePath;
            LocatorEvidenceReviewDecisionPath = savePath;
            LocatorEvidenceVisualCorrespondence = persistedDecision.VisualCorrespondence;
            LocatorEvidenceReviewer = persistedDecision.Reviewer;
            LocatorEvidenceReviewNotes = persistedDecision.Notes;
            LocatorEvidenceReviewDecisionStatusText = BuildLocatorEvidenceReviewDecisionStatus(persistedDecision)
                + LocalText(" 저장됨. Import는 현재 APPROVED일 때만 활성화됩니다.", " Saved. Import is enabled only for a current APPROVED decision.");
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
            RefreshCommandState();
            return true;
        }

        private bool CanRecordLocatorEvidenceReviewDecision()
        {
            return CanUseSelectedRecipe()
                && loadedLocatorEvidencePacket != null
                && loadedLocatorEvidenceReviewDecision != null;
        }

        private bool TryValidateLocatorEvidenceReviewDecisionForPromotion(out string message)
        {
            message = string.Empty;
            if (!OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate))
            {
                return true;
            }

            if (loadedLocatorEvidencePacket == null
                || string.IsNullOrWhiteSpace(loadedLocatorEvidencePacketPath))
            {
                message = LocalText(
                    "locator Recipe 승격에는 검증된 Evidence Packet이 필요합니다.",
                    "Locator Recipe promotion requires a verified Evidence Packet.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath))
            {
                message = LocalText(
                    "locator Recipe 승격에는 현재 APPROVED review decision이 필요합니다.",
                    "Locator Recipe promotion requires a current APPROVED review decision.");
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
                    loadedLocatorEvidenceReviewDecisionPath,
                    out OpenVisionRecipeLocatorRelativeBlobReviewDecision persistedDecision,
                    out string loadMessage))
            {
                message = loadMessage;
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                message = planMessage;
                return false;
            }

            if (!persistedDecision.TryValidateAgainst(
                    loadedLocatorEvidencePacketPath,
                    loadedLocatorEvidencePacket,
                    plan,
                    out string validationMessage))
            {
                message = validationMessage;
                return false;
            }

            if (!string.Equals(
                    persistedDecision.Decision,
                    OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved,
                    StringComparison.Ordinal))
            {
                message = LocalText(
                    "현재 review decision이 APPROVED가 아니므로 Recipe를 승격하지 않습니다.",
                    "The current review decision is not APPROVED, so the Recipe will not be promoted.");
                return false;
            }

            return true;
        }

        private void ClearLoadedLocatorEvidenceReviewDecision()
        {
            loadedLocatorEvidenceReviewDecision = null;
            loadedLocatorEvidenceReviewDecisionPath = string.Empty;
            LocatorEvidenceReviewDecisionPath = string.Empty;
            LocatorEvidenceVisualCorrespondence = OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed;
            LocatorEvidenceReviewer = string.Empty;
            LocatorEvidenceReviewNotes = string.Empty;
            LocatorEvidenceReviewDecisionStatusText = LocalText(
                "대기 중: Evidence Packet을 로드한 뒤 review decision 파일을 선택하세요.",
                "Waiting: load an Evidence Packet, then select its review decision file.");
            OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
        }

        private string BuildLocatorEvidenceReviewDecisionStatus(
            OpenVisionRecipeLocatorRelativeBlobReviewDecision decision)
        {
            return LocalText("Review decision: ", "Review decision: ")
                + decision.Decision
                + " / visual="
                + decision.VisualCorrespondence
                + " / CandidateId="
                + decision.ReviewedCandidateId;
        }

        private static string ResolveLocatorEvidenceReviewDecisionSavePath(string path)
        {
            const string templateSuffix = ".template.json";
            if (!string.IsNullOrWhiteSpace(path)
                && path.EndsWith(templateSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return path.Substring(0, path.Length - templateSuffix.Length) + ".json";
            }

            return path;
        }

        private void CompileLocatorEvidencePacket()
        {
            CompileLocatorEvidencePacketFromCurrentSettings();
        }

        private bool TryCreateCurrentLocatorRelativeBlobPlan(
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string message)
        {
            return OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
                LlmReferenceImagePath,
                MatchingIntentSearchRoiText,
                HybridRelativeRoiText,
                HybridReferencePoseText,
                MatchingIntentScoreMinText,
                HybridScoreMarginText,
                HybridAngleMinimumText,
                HybridAngleMaximumText,
                HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText,
                BlobCountIntentThresholdText,
                BlobCountIntentMinAreaText,
                BlobCountIntentMaxAreaText,
                ResolveLocatorRelativeBlobExpectedCountText(),
                out plan,
                out message);
        }

        public bool CompileLocatorEvidencePacketFromCurrentSettings()
        {
            locatorEvidenceCompilationReady = false;
            if (loadedLocatorEvidencePacket == null)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "먼저 검증된 Evidence Packet을 로드하세요.",
                    "Load a verified Evidence Packet first.");
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                return false;
            }

            if (!OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Locator-relative Blob 의도를 선택한 뒤 Compile하세요.",
                    "Select the Locator-relative Blob intent before compiling.");
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                return false;
            }

            if (!TryCreateCurrentLocatorRelativeBlobPlan(
                    out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
                    out string planMessage))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "현재 Guided Setup 설정이 Compile 불가합니다: ",
                    "The current Guided Setup settings cannot be compiled: ") + planMessage;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText("현재 설정 대조: NG - ", "Current settings comparison: NG - ")
                    + planMessage;
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                    loadedLocatorEvidencePacket,
                    plan,
                    out VisionPipeline pipeline,
                    out string compileMessage))
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Evidence Packet Compile NG: ",
                    "Evidence Packet compile failed: ") + compileMessage;
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText("Evidence compile: NG - ", "Evidence compile: NG - ")
                    + compileMessage;
                OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                RefreshCommandState();
                return false;
            }

            LlmPromptText = BuildLlmPromptText();
            LlmXmlDraftText = SerializePipelineToXmlText(pipeline);
            bool xmlReady = ValidateLlmXmlDraftText(false);
            locatorEvidenceCompilationReady = xmlReady;
            LocatorEvidencePacketStatusText = xmlReady
                ? LocalText(
                    "Evidence Packet Compile OK. XML 초안만 준비했으며 Import와 명시적 Run은 별도입니다.",
                    "Evidence Packet compile OK. Only the XML draft was prepared; Import and explicit Run remain separate.")
                : LocalText(
                    "Evidence Packet은 OK지만 XML 검증이 NG입니다. Import/Preview/Run은 실행하지 않았습니다.",
                    "The Evidence Packet is valid, but XML validation failed. Import/Preview/Run were not executed.");
            LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                + Environment.NewLine
                + Environment.NewLine
                + LocalText(
                    "현재 설정 대조: OK - CandidateId만 사용했으며 LLM 좌표는 적용하지 않았습니다.",
                    "Current settings comparison: OK - only CandidateId was used; no LLM-supplied coordinates were applied.")
                + Environment.NewLine
                + LocalText(
                    "다음 단계: XML 검증/가져오기 후 기존 명시적 Run 명령을 별도로 실행하세요.",
                    "Next: validate/import the XML, then execute the existing explicit Run command separately.");
            OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
            RefreshCommandState();
            return xmlReady;
        }

        private bool CanCompileLocatorEvidencePacket()
        {
            return CanUseSelectedRecipe()
                && loadedLocatorEvidencePacket != null
                && OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate);
        }

        private void InvalidateLocatorEvidenceCompilation()
        {
            if (!locatorEvidenceCompilationReady)
            {
                return;
            }

            locatorEvidenceCompilationReady = false;
            if (loadedLocatorEvidencePacket != null)
            {
                LocatorEvidencePacketStatusText = LocalText(
                    "Guided Setup 또는 XML이 변경되었습니다. Evidence Packet을 다시 Compile하세요.",
                    "Guided Setup or XML changed. Compile the Evidence Packet again.");
                LocatorEvidenceReviewText = BuildLocatorEvidenceReviewText(loadedLocatorEvidencePacket)
                    + Environment.NewLine
                    + Environment.NewLine
                    + LocalText(
                        "상태: STALE - 현재 설정과의 Compile을 다시 수행해야 합니다.",
                        "State: STALE - compile it again against the current settings.");
            }

            OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
        }

        public string LocatorEvidencePacketPath
        {
            get => locatorEvidencePacketPath;
            set
            {
                string next = value ?? string.Empty;
                if (SetProperty(ref locatorEvidencePacketPath, next))
                {
                    if ((loadedLocatorEvidencePacket != null
                            || loadedLocatorEvidenceReviewDecision != null)
                        && !AreSamePath(next, loadedLocatorEvidencePacketPath))
                    {
                        loadedLocatorEvidencePacket = null;
                        loadedLocatorEvidencePacketPath = string.Empty;
                        locatorEvidenceCompilationReady = false;
                        LocatorEvidenceOverlayImage = null;
                        ClearLoadedLocatorEvidenceReviewDecision();
                        LocatorEvidencePacketStatusText = LocalText(
                            "Packet 경로가 변경되었습니다. 새 Packet을 로드하세요.",
                            "The packet path changed. Load the new packet.");
                        LocatorEvidenceReviewText = LocalText(
                            "대기 중: 새 Evidence Packet을 로드하세요.",
                            "Waiting: load the new Evidence Packet.");
                        OnPropertyChanged(nameof(IsLocatorEvidencePacketLoaded));
                        OnPropertyChanged(nameof(IsLocatorEvidenceCompilationReady));
                    }

                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidencePacketStatusText
        {
            get => locatorEvidencePacketStatusText;
            private set => SetProperty(ref locatorEvidencePacketStatusText, value ?? string.Empty);
        }

        public string LocatorEvidenceReviewDecisionPath
        {
            get => locatorEvidenceReviewDecisionPath;
            set
            {
                string next = value ?? string.Empty;
                if (!SetProperty(ref locatorEvidenceReviewDecisionPath, next))
                {
                    return;
                }

                if (loadedLocatorEvidenceReviewDecision != null
                    && !AreSamePath(next, loadedLocatorEvidenceReviewDecisionPath))
                {
                    loadedLocatorEvidenceReviewDecision = null;
                    loadedLocatorEvidenceReviewDecisionPath = string.Empty;
                    LocatorEvidenceVisualCorrespondence = OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed;
                    LocatorEvidenceReviewer = string.Empty;
                    LocatorEvidenceReviewNotes = string.Empty;
                    LocatorEvidenceReviewDecisionStatusText = LocalText(
                        "review decision 경로가 변경되었습니다. 새 결정을 로드하세요.",
                        "The review decision path changed. Load the new decision.");
                    OnPropertyChanged(nameof(IsLocatorEvidenceReviewDecisionLoaded));
                }

                RefreshCommandState();
            }
        }

        public string LocatorEvidenceReviewDecisionStatusText
        {
            get => locatorEvidenceReviewDecisionStatusText;
            private set => SetProperty(ref locatorEvidenceReviewDecisionStatusText, value ?? string.Empty);
        }

        public string LocatorEvidenceVisualCorrespondence
        {
            get => locatorEvidenceVisualCorrespondence;
            set
            {
                if (SetProperty(ref locatorEvidenceVisualCorrespondence, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewer
        {
            get => locatorEvidenceReviewer;
            set
            {
                if (SetProperty(ref locatorEvidenceReviewer, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewNotes
        {
            get => locatorEvidenceReviewNotes;
            set
            {
                if (SetProperty(ref locatorEvidenceReviewNotes, value ?? string.Empty))
                {
                    RefreshCommandState();
                }
            }
        }

        public string LocatorEvidenceReviewText
        {
            get => locatorEvidenceReviewText;
            private set => SetProperty(ref locatorEvidenceReviewText, value ?? string.Empty);
        }

        public BitmapSource LocatorEvidenceOverlayImage
        {
            get => locatorEvidenceOverlayImage;
            private set => SetProperty(ref locatorEvidenceOverlayImage, value);
        }

        public bool IsLocatorEvidencePacketLoaded => loadedLocatorEvidencePacket != null;

        public bool IsLocatorEvidenceReviewDecisionLoaded => loadedLocatorEvidenceReviewDecision != null;

        public bool IsLocatorEvidenceCompilationReady => locatorEvidenceCompilationReady;

        public string LocatorEvidencePacketLabelText => LocalText("Evidence Packet", "Evidence Packet");

        public string LocatorEvidenceLoadText => LocalText("Packet 로드", "Load packet");

        public string LocatorEvidenceCompileText => LocalText("검증·Compile", "Validate / compile");

        public string LocatorEvidenceReviewDecisionLabelText => LocalText("검토 결정", "Review decision");

        public string LocatorEvidenceReviewDecisionLoadText => LocalText("결정 로드", "Load decision");

        public string LocatorEvidenceVisualCorrespondenceLabelText => LocalText("시각 대응", "Visual correspondence");

        public string LocatorEvidenceReviewerLabelText => LocalText("검토자", "Reviewer");

        public string LocatorEvidenceReviewNotesLabelText => LocalText("검토 Notes", "Review notes");

        public string LocatorEvidenceApproveText => LocalText("후보 승인", "Approve candidate");

        public string LocatorEvidenceRejectText => LocalText("후보 거부", "Reject candidate");

        public string LocatorEvidenceReplacementText => LocalText("교체 요청", "Request replacement");

        public string LocatorEvidenceReviewLabelText => LocalText("Candidate / 무결성 검토", "Candidate / integrity review");

        public string LocatorEvidenceOverlayLabelText => LocalText("현재 실행 overlay", "Current-run overlay");

        public string LocatorEvidenceBoundaryText => LocalText(
            "Load는 Packet 검토만 수행합니다. Compile은 현재 설정과 대조해 XML 초안만 준비합니다. 현재 Packet·Plan과 일치하는 APPROVED 검토 결정 전에는 Recipe Import가 차단되며, 승인도 qualification·Preview·Run·레이어·라우팅을 실행하지 않습니다.",
            "Load only reviews the packet. Compile compares the current settings and prepares XML only. Recipe Import remains blocked until a current Packet/Plan-matched APPROVED decision; approval does not qualify, Preview, Run, or mutate layers or routing.");

        private static BitmapImage LoadLocatorEvidenceOverlay(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new FileNotFoundException("The packet overlay file is missing.", path);
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            return image;
        }

        private string BuildLocatorEvidenceReviewText(OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet)
        {
            StringBuilder review = new StringBuilder();
            review.AppendLine("Schema: " + packet.SchemaVersion);
            review.AppendLine("Skill: " + packet.SkillId + " / " + packet.SkillVersion);
            review.AppendLine("Source: " + packet.SourceImagePath);
            review.AppendLine("Source SHA-256: " + packet.SourceImageSha256);
            review.AppendLine("Template: " + packet.LocatorTemplatePath);
            review.AppendLine("Template SHA-256: " + packet.LocatorTemplateSha256);
            review.AppendLine("Overlay: " + packet.PreviewOverlayPath);
            review.AppendLine("Overlay SHA-256: " + packet.PreviewOverlaySha256);
            review.AppendLine("Frame: " + packet.CoordinateFrame + " / "
                + packet.SourceImageWidth.ToString(CultureInfo.InvariantCulture) + "x"
                + packet.SourceImageHeight.ToString(CultureInfo.InvariantCulture));
            review.AppendLine("Selected CandidateId: " + packet.SelectedCandidateId);
            foreach (OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate candidate in packet.Candidates)
            {
                review.AppendLine(
                    candidate.CandidateId
                    + " / native=" + candidate.NativeIndex.ToString(CultureInfo.InvariantCulture)
                    + " / accepted=" + candidate.Accepted.ToString(CultureInfo.InvariantCulture)
                    + " / center=(" + candidate.CenterX.ToString("0.###", CultureInfo.InvariantCulture)
                    + "," + candidate.CenterY.ToString("0.###", CultureInfo.InvariantCulture) + ")"
                    + " / score=" + candidate.Score.ToString("0.###", CultureInfo.InvariantCulture)
                    + " / margin=" + candidate.ScoreMargin.ToString("0.###", CultureInfo.InvariantCulture));
            }

            return review.ToString().TrimEnd();
        }

        private static bool AreSamePath(string left, string right)
        {
            try
            {
                return string.Equals(
                    Path.GetFullPath(left ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    Path.GetFullPath(right ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public bool LoadLlmXmlDraftFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                StatusText = LocalText("LLM XML 초안 파일을 찾을 수 없습니다.", "LLM XML draft file was not found.");
                return false;
            }

            if (IsReviewBundlePath(path))
            {
                return LoadReviewBundleForDryRun(path);
            }

            LlmXmlDraftText = File.ReadAllText(path);
            StatusText = LocalText("LLM XML 초안 로드됨: ", "Loaded LLM XML draft: ") + Path.GetFileName(path);
            return ValidateLlmXmlDraftText(false);
        }

        private bool LoadReviewBundleForDryRun(string path)
        {
            if (!OpenVisionRecipeReviewBundleInspector.TryInspect(path, out OpenVisionRecipeReviewBundleInspection inspection))
            {
                ClearLoadedReviewBundleContext();
                LlmXmlDraftValidationReport = inspection.IntegrityReport;
                LlmXmlDraftDependencyReport = inspection.PathReport;
                SetLlmXmlDraftDependencyPlaceholder(LocalText(
                    "번들 무결성 오류를 해결한 뒤 다시 선택하세요.",
                    "Fix the bundle integrity issue, then select it again."));
                OpenVisionRecipeReviewBundleDryRunProjection failureProjection = reviewBundleDryRunProjectionOwner.Project(
                    inspectionSucceeded: false,
                    xmlReady: false);
                StatusText = failureProjection.StatusText;
                openLlmXmlReview();
                return failureProjection.Succeeded;
            }

            LlmXmlDraftText = inspection.PipelineXml;
            loadedReviewBundleInspection = inspection;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            bool xmlReady = ValidateLlmXmlDraftText(false);
            OpenVisionRecipeReviewBundleDryRunProjection reviewProjection = reviewBundleDryRunProjectionOwner.Project(
                inspectionSucceeded: true,
                xmlReady: xmlReady);
            StatusText = reviewProjection.StatusText;
            openLlmXmlReview();
            return reviewProjection.Succeeded;
        }

        private static bool IsReviewBundlePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && (path.EndsWith(".review.zip", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(Path.GetExtension(path), ".zip", StringComparison.OrdinalIgnoreCase));
        }

        private void ClearLoadedReviewBundleContext()
        {
            if (loadedReviewBundleInspection == null)
            {
                return;
            }

            loadedReviewBundleInspection = null;
            OnPropertyChanged(nameof(LlmDraftValidationText));
            OnPropertyChanged(nameof(LlmDependencyReportText));
            LlmXmlDraftValidationReport = LocalText(
                "검토 번들에서 로드한 XML이 변경되었습니다. 다시 검증하세요.",
                "XML loaded from the review bundle changed. Validate it again.");
            LlmXmlDraftDependencyReport = LocalText(
                "번들 경로 증거 연결이 해제되었습니다.",
                "Bundle path evidence was detached.");
            SetLlmXmlDraftDependencyPlaceholder(LocalText(
                "변경된 XML을 다시 검증하세요.",
                "Validate the changed XML again."));
        }

        private void ValidateLlmXmlDraft()
        {
            ValidateLlmXmlDraftText(false);
        }

        public bool ValidateLlmXmlDraftTextForTest()
        {
            return ValidateLlmXmlDraftText(false);
        }

        private void ImportLlmXmlDraft()
        {
            if (!TryValidateLocatorEvidenceReviewDecisionForPromotion(out string reviewDecisionMessage))
            {
                llmXmlDraftImportReady = false;
                LocatorEvidenceReviewDecisionStatusText = LocalText(
                    "Recipe 승격 차단: ",
                    "Recipe promotion blocked: ") + reviewDecisionMessage;
                StatusText = LocalText(
                    "현재 APPROVED review decision 없이는 locator Recipe를 가져올 수 없습니다.",
                    "A locator Recipe cannot be imported without a current APPROVED review decision.");
                RefreshCommandState();
                return;
            }

            if (!TryBuildLlmDraftPipeline(copyDependencies: true, out VisionPipeline pipeline, out string validationReport, out string dependencyReport))
            {
                llmXmlDraftImportReady = false;
                LlmXmlDraftValidationReport = validationReport;
                LlmXmlDraftDependencyReport = dependencyReport;
                LlmXmlDraftReviewReport = LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
                LlmXmlDraftDiffReport = LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
                StatusText = LocalText("LLM XML 초안을 가져올 수 없습니다.", "LLM XML draft is not importable.");
                RefreshCommandState();
                return;
            }

            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string basePipelineName = string.IsNullOrWhiteSpace(pipeline.Name)
                ? "LLM_Draft_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                : pipeline.Name.Trim();
            pipeline.Name = CreateUniquePipelineName(recipeName, basePipelineName);
            if (!TryLeaveSelectedStepEdit(
                OpenVisionRecipePendingEditTransitionKind.Pipeline,
                pipeline.Name))
            {
                return;
            }

            if (OpenVisionRecipeDependencyReviewService.TryCopyReferenceImageToRecipe(
                recipeName,
                pipeline.Name,
                LlmReferenceImagePath,
                out string copiedReferenceImagePath))
            {
                dependencyReport = string.IsNullOrWhiteSpace(dependencyReport)
                    ? LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath
                    : dependencyReport + Environment.NewLine + LocalText("참조 이미지 복사됨: ", "Reference image copied: ") + copiedReferenceImagePath;
            }
            LlmXmlDraftReviewReport = BuildLlmDraftReviewReport(pipeline);
            LlmXmlDraftDiffReport = BuildLlmDraftDiffReport(pipeline);
            VisionPipelineStorage.Save(recipeName, pipeline);
            VisionPipelineStorage.SaveActivePipelineName(recipeName, pipeline.Name);
            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            StatusText = LocalText("LLM XML 초안 가져오기 완료: ", "Imported LLM XML draft: ") + pipeline.Name;
            RefreshPipelineOptions(pipeline.Name);
            RefreshOptions();
            refreshAfterSwitch();
        }

        private void UseSelectedSampleReference()
        {
            if (SelectedSampleOption?.Sample == null || string.IsNullOrWhiteSpace(SelectedSampleOption.Sample.ImageFullPath))
            {
                StatusText = LocalText("선택된 샘플 이미지를 사용할 수 없습니다.", "No selected sample image is available.");
                return;
            }

            LlmReferenceImagePath = SelectedSampleOption.Sample.ImageFullPath;
            StatusText = LocalText("참조 이미지가 샘플에서 설정됨: ", "Reference image set from sample: ") + SelectedSampleOption.Sample.SampleName;
        }

        private bool ValidateLlmXmlDraftText(bool copyDependencies)
        {
            bool ok = TryBuildLlmDraftPipeline(copyDependencies, out VisionPipeline pipeline, out string validationReport, out string dependencyReport);
            if (loadedReviewBundleInspection != null)
            {
                validationReport = loadedReviewBundleInspection.IntegrityReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + validationReport;
                dependencyReport = loadedReviewBundleInspection.PathReport
                    + Environment.NewLine
                    + Environment.NewLine
                    + dependencyReport;
            }

            LlmXmlDraftValidationReport = validationReport;
            LlmXmlDraftDependencyReport = dependencyReport;
            LlmXmlDraftReviewReport = ok ? BuildLlmDraftReviewReport(pipeline) : LocalText("초안 검토 건너뜀: 검증 실패.", "Draft review skipped: validation failed.");
            LlmXmlDraftDiffReport = ok ? BuildLlmDraftDiffReport(pipeline) : LocalText("변경점 검토 건너뜀: 검증 실패.", "Diff review skipped: validation failed.");
            llmXmlDraftImportReady = ok;
            StatusText = ok ? LocalText("LLM XML 초안 검증 OK.", "LLM XML draft validation OK.") : LocalText("LLM XML 초안 검증 NG.", "LLM XML draft validation NG.");
            RefreshCommandState();
            return ok;
        }

        private bool TryBuildLlmDraftPipeline(
            bool copyDependencies,
            out VisionPipeline pipeline,
            out string validationReport,
            out string dependencyReport)
        {
            OpenVisionRecipeLlmDraftValidationResult result =
                OpenVisionRecipeLlmDraftValidationService.Validate(
                    new OpenVisionRecipeLlmDraftValidationRequest(
                        LlmXmlDraftText,
                        NormalizeRecipeName(selectedRecipeName),
                        SelectedLlmToolTemplate,
                        LlmReferenceImagePath,
                        loadedReviewBundleInspection == null,
                        loadedReviewBundleInspection,
                        copyDependencies,
                        OpenVisionRecipeLlmIntent.IsPinArrayGapTemplate(SelectedLlmToolTemplate)
                            ? CreatePinArrayGapIntentValidationContext()
                            : null,
                        OpenVisionRecipeLlmIntent.IsDarkBandGapTemplate(SelectedLlmToolTemplate)
                            ? new OpenVisionRecipeDarkBandGapIntentValidationContext(DarkBandGapIntentRoiText)
                            : null,
                        OpenVisionRecipeLlmIntent.IsHybridRelativeRoiGapTemplate(SelectedLlmToolTemplate)
                            ? CreateHybridRelativeRoiIntentValidationContext()
                            : null,
                        OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate)
                            ? CreateLocatorRelativeBlobIntentValidationContext()
                            : null));

            pipeline = result.Pipeline;
            validationReport = result.ValidationReport;
            dependencyReport = result.DependencyReport;
            LlmXmlDraftDependencyRows = result.DependencyRows;
            return result.Success;
        }

        private OpenVisionRecipePinArrayGapIntentValidationContext CreatePinArrayGapIntentValidationContext()
        {
            int sourceWidth = 0;
            int sourceHeight = 0;
            string sourceImagePath = ResolvePinGapRoiSuggestionImagePath();
            if (!string.IsNullOrWhiteSpace(sourceImagePath))
            {
                try
                {
                    BitmapFrame frame = BitmapFrame.Create(
                        new Uri(sourceImagePath, UriKind.Absolute),
                        BitmapCreateOptions.DelayCreation,
                        BitmapCacheOption.OnLoad);
                    sourceWidth = frame.PixelWidth;
                    sourceHeight = frame.PixelHeight;
                }
                catch
                {
                    sourceWidth = 0;
                    sourceHeight = 0;
                }
            }

            return new OpenVisionRecipePinArrayGapIntentValidationContext(
                PinArrayGapRoiText,
                PinArrayGapPolarityText,
                PinArrayGapMeasurementText,
                PinArrayGapRangeMaxText,
                PinArrayGapDarkThresholdText,
                PinArrayGapMinDarkCoverageRatioText,
                PinArrayGapMinPinWidthText,
                PinArrayGapMaxPinBreakWidthText,
                PinArrayGapMinGapWidthText,
                sourceWidth,
                sourceHeight);
        }

        private OpenVisionRecipeHybridRelativeRoiIntentValidationContext CreateHybridRelativeRoiIntentValidationContext()
        {
            return new OpenVisionRecipeHybridRelativeRoiIntentValidationContext(
                LlmReferenceImagePath,
                MatchingIntentSearchRoiText,
                HybridRelativeRoiText,
                HybridReferencePoseText,
                MatchingIntentScoreMinText,
                HybridScoreMarginText,
                HybridAngleMinimumText,
                HybridAngleMaximumText,
                HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText);
        }

        private OpenVisionRecipeLocatorRelativeBlobIntentValidationContext CreateLocatorRelativeBlobIntentValidationContext()
        {
            return new OpenVisionRecipeLocatorRelativeBlobIntentValidationContext(
                LlmReferenceImagePath,
                MatchingIntentSearchRoiText,
                HybridRelativeRoiText,
                HybridReferencePoseText,
                MatchingIntentScoreMinText,
                HybridScoreMarginText,
                HybridAngleMinimumText,
                HybridAngleMaximumText,
                HybridScaleRatioMinimumText,
                HybridScaleRatioMaximumText,
                HybridMinimumValidPixelRatioText,
                BlobCountIntentThresholdText,
                BlobCountIntentMinAreaText,
                BlobCountIntentMaxAreaText,
                ResolveLocatorRelativeBlobExpectedCountText());
        }

        private void SetLlmXmlDraftDependencyPlaceholder(string action)
        {
            LlmXmlDraftDependencyRows = new[]
            {
                new OpenVisionRecipeDependencyReviewRow(
                    LocalText("대기", "Waiting"),
                    "-",
                    "-",
                    "-",
                    action)
            };
        }

        private bool CanUseLlmXmlDraft()
        {
            return CanUseSelectedRecipe()
                && !string.IsNullOrWhiteSpace(LlmXmlDraftText);
        }

        private bool CanImportLlmXmlDraft()
        {
            if (!CanUseLlmXmlDraft() || !llmXmlDraftImportReady)
            {
                return false;
            }

            return !OpenVisionRecipeLlmIntent.IsLocatorRelativeBlobTemplate(SelectedLlmToolTemplate)
                || (loadedLocatorEvidencePacket != null
                    && loadedLocatorEvidenceReviewDecision != null
                    && string.Equals(
                        loadedLocatorEvidenceReviewDecision.Decision,
                        OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved,
                        StringComparison.Ordinal)
                    && !string.IsNullOrWhiteSpace(loadedLocatorEvidenceReviewDecisionPath));
        }
    }
}
