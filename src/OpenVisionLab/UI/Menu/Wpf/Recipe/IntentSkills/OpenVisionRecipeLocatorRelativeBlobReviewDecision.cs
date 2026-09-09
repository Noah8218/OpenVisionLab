using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenVisionLab
{
    // File-backed operator decision for a hash-verified locator evidence packet.
    // This record never approves a candidate by itself: PENDING is the only state
    // created by the diagnostic flow, and APPROVED requires an explicit review.
    internal sealed class OpenVisionRecipeLocatorRelativeBlobReviewDecision
    {
        internal const string CurrentSchemaVersion = "locator-relative-blob-review-decision-v1";
        internal const string Pending = "PENDING";
        internal const string Approved = "APPROVED";
        internal const string Rejected = "REJECTED";
        internal const string ReplacementRequested = "REPLACEMENT_REQUESTED";
        internal const string NotReviewed = "NOT_REVIEWED";
        internal const string Pass = "PASS";
        internal const string Fail = "FAIL";

        public string SchemaVersion { get; set; } = CurrentSchemaVersion;
        public string Decision { get; set; } = Pending;
        public string VisualCorrespondence { get; set; } = NotReviewed;
        public string EvidencePacketPath { get; set; } = string.Empty;
        public string EvidencePacketSha256 { get; set; } = string.Empty;
        public string SourceImageSha256 { get; set; } = string.Empty;
        public string LocatorTemplateSha256 { get; set; } = string.Empty;
        public string PreviewOverlaySha256 { get; set; } = string.Empty;
        public string ReviewedCandidateId { get; set; } = string.Empty;
        public string PlanFingerprint { get; set; } = string.Empty;
        public string InspectionRoi { get; set; } = string.Empty;
        public int Threshold { get; set; }
        public int MinimumArea { get; set; }
        public int MaximumArea { get; set; }
        public int? ExpectedResultCount { get; set; }
        public string Reviewer { get; set; } = string.Empty;
        public string ReviewedUtc { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        internal static bool TryCreatePending(
            string evidencePacketPath,
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out OpenVisionRecipeLocatorRelativeBlobReviewDecision decision,
            out string message)
        {
            decision = null;
            message = string.Empty;

            string normalizedPacketPath;
            try
            {
                normalizedPacketPath = Path.GetFullPath(evidencePacketPath ?? string.Empty);
            }
            catch (Exception exception)
            {
                message = "Evidence packet path is invalid: " + exception.GetBaseException().Message;
                return false;
            }

            if (!File.Exists(normalizedPacketPath))
            {
                message = "Evidence packet file is missing.";
                return false;
            }

            if (packet == null || !packet.TryValidate(out message) || plan == null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "A validated evidence packet and reviewed plan are required.";
                }

                return false;
            }

            decision = new OpenVisionRecipeLocatorRelativeBlobReviewDecision
            {
                Decision = Pending,
                VisualCorrespondence = NotReviewed,
                EvidencePacketPath = normalizedPacketPath,
                EvidencePacketSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(normalizedPacketPath),
                SourceImageSha256 = packet.SourceImageSha256,
                LocatorTemplateSha256 = packet.LocatorTemplateSha256,
                PreviewOverlaySha256 = packet.PreviewOverlaySha256,
                ReviewedCandidateId = packet.SelectedCandidateId,
                PlanFingerprint = ComputePlanFingerprint(plan),
                InspectionRoi = plan.InspectionRoi.ToText(),
                Threshold = plan.Threshold,
                MinimumArea = plan.MinimumArea,
                MaximumArea = plan.MaximumArea,
                ExpectedResultCount = plan.ExpectedCount,
                Notes = "Operator visual correspondence review and downstream ROI/tolerance confirmation are required."
            };

            if (!decision.TryValidateAgainst(normalizedPacketPath, packet, plan, out message))
            {
                decision = null;
                return false;
            }

            message = string.Empty;
            return true;
        }

        internal bool TryValidate(out string message)
        {
            message = string.Empty;
            if (!string.Equals(SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal)
                || !IsDecision(Decision)
                || !IsVisualCorrespondence(VisualCorrespondence)
                || !IsSha256(EvidencePacketSha256)
                || !IsSha256(SourceImageSha256)
                || !IsSha256(LocatorTemplateSha256)
                || !IsSha256(PreviewOverlaySha256)
                || string.IsNullOrWhiteSpace(EvidencePacketPath)
                || string.IsNullOrWhiteSpace(ReviewedCandidateId)
                || !IsSha256(PlanFingerprint)
                || string.IsNullOrWhiteSpace(InspectionRoi)
                || Threshold < 0
                || Threshold > 255
                || MinimumArea <= 0
                || MaximumArea < MinimumArea
                || (ExpectedResultCount.HasValue && ExpectedResultCount.Value < 0))
            {
                message = "Review decision schema, identity, ROI, threshold, area, or count data is invalid.";
                return false;
            }

            if (string.Equals(Decision, Pending, StringComparison.Ordinal))
            {
                if (!string.Equals(VisualCorrespondence, NotReviewed, StringComparison.Ordinal)
                    || !string.IsNullOrWhiteSpace(Reviewer)
                    || !string.IsNullOrWhiteSpace(ReviewedUtc))
                {
                    message = "A pending review decision must remain NOT_REVIEWED and must not claim a reviewer or review time.";
                    return false;
                }

                return true;
            }

            if (string.IsNullOrWhiteSpace(Reviewer)
                || string.IsNullOrWhiteSpace(ReviewedUtc)
                || string.IsNullOrWhiteSpace(Notes)
                || !IsUtcTimestamp(ReviewedUtc))
            {
                message = "A completed review decision requires reviewer, review time, and notes.";
                return false;
            }

            if (string.Equals(Decision, Approved, StringComparison.Ordinal)
                && !string.Equals(VisualCorrespondence, Pass, StringComparison.Ordinal))
            {
                message = "An approved locator requires PASS visual correspondence.";
                return false;
            }

            return true;
        }

        internal bool TryValidateAgainst(
            string evidencePacketPath,
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string message)
        {
            if (!TryValidate(out message))
            {
                return false;
            }

            if (packet == null || !packet.TryValidate(out message) || plan == null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "A validated evidence packet and reviewed plan are required.";
                }

                return false;
            }

            string normalizedPacketPath;
            try
            {
                normalizedPacketPath = Path.GetFullPath(evidencePacketPath ?? string.Empty);
            }
            catch (Exception exception)
            {
                message = "Evidence packet path is invalid: " + exception.GetBaseException().Message;
                return false;
            }

            if (!AreSamePath(EvidencePacketPath, normalizedPacketPath))
            {
                message = "Review decision does not identify the supplied evidence packet path.";
                return false;
            }

            string currentPacketSha256;
            try
            {
                currentPacketSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(normalizedPacketPath);
            }
            catch (Exception exception)
            {
                message = "Evidence packet SHA-256 could not be read: " + exception.GetBaseException().Message;
                return false;
            }

            if (!string.Equals(currentPacketSha256, EvidencePacketSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(packet.SourceImageSha256, SourceImageSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(packet.LocatorTemplateSha256, LocatorTemplateSha256, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(packet.PreviewOverlaySha256, PreviewOverlaySha256, StringComparison.OrdinalIgnoreCase))
            {
                message = "Review decision evidence hashes are stale or do not match the packet.";
                return false;
            }

            if (!string.Equals(packet.SelectedCandidateId, ReviewedCandidateId, StringComparison.Ordinal)
                || !packet.Candidates.Exists(candidate =>
                    candidate != null
                    && string.Equals(candidate.CandidateId, ReviewedCandidateId, StringComparison.Ordinal)))
            {
                message = "Review decision CandidateId does not match a retained packet candidate.";
                return false;
            }

            if (string.Equals(Decision, Approved, StringComparison.Ordinal))
            {
                OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate selected = packet.Candidates.Find(candidate =>
                    candidate != null
                    && string.Equals(candidate.CandidateId, ReviewedCandidateId, StringComparison.Ordinal));
                if (selected == null || !selected.Accepted)
                {
                    message = "Only the packet's numerically accepted candidate can be approved; replacement requires a new packet.";
                    return false;
                }
            }

            if (!string.Equals(PlanFingerprint, ComputePlanFingerprint(plan), StringComparison.OrdinalIgnoreCase)
                || !string.Equals(InspectionRoi, plan.InspectionRoi.ToText(), StringComparison.Ordinal)
                || Threshold != plan.Threshold
                || MinimumArea != plan.MinimumArea
                || MaximumArea != plan.MaximumArea
                || ExpectedResultCount != plan.ExpectedCount)
            {
                message = "Review decision is stale against the current reviewed locator plan or downstream inspection definition.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        internal bool TryApplyOperatorDecision(
            string decision,
            string visualCorrespondence,
            string reviewer,
            string notes,
            DateTimeOffset reviewedUtc,
            out string message)
        {
            string previousDecision = Decision;
            string previousVisualCorrespondence = VisualCorrespondence;
            string previousReviewer = Reviewer;
            string previousReviewedUtc = ReviewedUtc;
            string previousNotes = Notes;

            Decision = decision ?? string.Empty;
            VisualCorrespondence = visualCorrespondence ?? string.Empty;
            Reviewer = reviewer ?? string.Empty;
            ReviewedUtc = reviewedUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
            Notes = notes ?? string.Empty;
            if (TryValidate(out message))
            {
                return true;
            }

            Decision = previousDecision;
            VisualCorrespondence = previousVisualCorrespondence;
            Reviewer = previousReviewer;
            ReviewedUtc = previousReviewedUtc;
            Notes = previousNotes;
            return false;
        }

        internal bool TrySave(string path, out string message)
        {
            if (!TryValidate(out message))
            {
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(Path.GetFullPath(path));
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never
                };
                File.WriteAllText(path, JsonSerializer.Serialize(this, options), Encoding.UTF8);
                message = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                message = exception.GetBaseException().Message;
                return false;
            }
        }

        internal static bool TryLoad(
            string path,
            out OpenVisionRecipeLocatorRelativeBlobReviewDecision decision,
            out string message)
        {
            decision = null;
            message = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    message = "Review decision file is missing.";
                    return false;
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                decision = JsonSerializer.Deserialize<OpenVisionRecipeLocatorRelativeBlobReviewDecision>(
                    File.ReadAllText(path),
                    options);
                if (decision == null || !decision.TryValidate(out message))
                {
                    decision = null;
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                decision = null;
                message = exception.GetBaseException().Message;
                return false;
            }
        }

        internal static string ComputePlanFingerprint(OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            StringBuilder canonical = new StringBuilder();
            Append(canonical, "planSchema", OpenVisionRecipeLocatorRelativeBlobIntentSkill.PlanSchemaVersion);
            Append(canonical, "skillId", OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId);
            Append(canonical, "templatePath", NormalizePath(plan.LocatorTemplatePath));
            Append(canonical, "searchRoi", plan.SearchRoi.ToText());
            Append(canonical, "inspectionRoi", plan.InspectionRoi.ToText());
            Append(canonical, "referenceX", FormatDouble(plan.ReferencePose.X));
            Append(canonical, "referenceY", FormatDouble(plan.ReferencePose.Y));
            Append(canonical, "referenceAngle", FormatDouble(plan.ReferencePose.Angle));
            Append(canonical, "referenceScale", FormatDouble(plan.ReferencePose.Scale));
            Append(canonical, "referenceWidth", plan.ReferencePose.ImageWidth.ToString(CultureInfo.InvariantCulture));
            Append(canonical, "referenceHeight", plan.ReferencePose.ImageHeight.ToString(CultureInfo.InvariantCulture));
            Append(canonical, "scoreMinimum", FormatDouble(plan.ScoreMinimum));
            Append(canonical, "scoreMargin", FormatDouble(plan.ScoreMargin));
            Append(canonical, "angleMinimum", FormatDouble(plan.AngleMinimum));
            Append(canonical, "angleMaximum", FormatDouble(plan.AngleMaximum));
            Append(canonical, "scaleRatioMinimum", FormatDouble(plan.ScaleRatioMinimum));
            Append(canonical, "scaleRatioMaximum", FormatDouble(plan.ScaleRatioMaximum));
            Append(canonical, "minimumValidPixelRatio", FormatDouble(plan.MinimumValidPixelRatio));
            Append(canonical, "threshold", plan.Threshold.ToString(CultureInfo.InvariantCulture));
            Append(canonical, "minimumArea", plan.MinimumArea.ToString(CultureInfo.InvariantCulture));
            Append(canonical, "maximumArea", plan.MaximumArea.ToString(CultureInfo.InvariantCulture));
            Append(canonical, "expectedCount", plan.ExpectedCount.HasValue
                ? plan.ExpectedCount.Value.ToString(CultureInfo.InvariantCulture)
                : string.Empty);

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())));
        }

        private static bool IsDecision(string value)
        {
            return string.Equals(value, Pending, StringComparison.Ordinal)
                || string.Equals(value, Approved, StringComparison.Ordinal)
                || string.Equals(value, Rejected, StringComparison.Ordinal)
                || string.Equals(value, ReplacementRequested, StringComparison.Ordinal);
        }

        private static bool IsVisualCorrespondence(string value)
        {
            return string.Equals(value, NotReviewed, StringComparison.Ordinal)
                || string.Equals(value, Pass, StringComparison.Ordinal)
                || string.Equals(value, Fail, StringComparison.Ordinal);
        }

        private static bool IsUtcTimestamp(string value)
        {
            return DateTimeOffset.TryParse(
                       value,
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.RoundtripKind,
                       out DateTimeOffset timestamp)
                && timestamp.Offset == TimeSpan.Zero;
        }

        private static bool AreSamePath(string left, string right)
        {
            try
            {
                return string.Equals(
                    NormalizePath(left),
                    NormalizePath(right),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path ?? string.Empty)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture);
        }

        private static void Append(StringBuilder builder, string key, string value)
        {
            builder.Append(key).Append('=').Append(value ?? string.Empty).Append('\n');
        }

        private static bool IsSha256(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 64)
            {
                return false;
            }

            foreach (char character in value)
            {
                if (!Uri.IsHexDigit(character))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
