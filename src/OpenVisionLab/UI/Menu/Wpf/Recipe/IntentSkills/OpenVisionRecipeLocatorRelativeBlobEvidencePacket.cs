using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenVisionLab
{
    // Immutable, file-backed evidence contract for locator-relative-blob-v1.
    // A packet is only usable when every referenced byte hash and selected candidate
    // remains valid; this is deliberately independent from pipeline execution.
    internal sealed class OpenVisionRecipeLocatorRelativeBlobEvidencePacket
    {
        internal const string CurrentSchemaVersion = "locator-relative-blob-evidence-v1.1";

        public string SchemaVersion { get; set; } = CurrentSchemaVersion;
        public string PlanSchemaVersion { get; set; } = OpenVisionRecipeLocatorRelativeBlobIntentSkill.PlanSchemaVersion;
        public string SkillId { get; set; } = OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId;
        public string SkillVersion { get; set; } = "1.1.0";
        public string SourceImagePath { get; set; } = string.Empty;
        public string SourceImageSha256 { get; set; } = string.Empty;
        public string LocatorTemplatePath { get; set; } = string.Empty;
        public string LocatorTemplateSha256 { get; set; } = string.Empty;
        public string PreviewOverlayPath { get; set; } = string.Empty;
        public string PreviewOverlaySha256 { get; set; } = string.Empty;
        public int SourceImageWidth { get; set; }
        public int SourceImageHeight { get; set; }
        public string CoordinateFrame { get; set; } = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame;
        public string Producer { get; set; } = string.Empty;
        public string ProducerVersion { get; set; } = string.Empty;
        public string SelectedCandidateId { get; set; } = string.Empty;
        public string CreatedUtc { get; set; } = string.Empty;
        public List<OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate> Candidates { get; set; } = new List<OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate>();

        internal bool TryValidate(out string message)
        {
            message = string.Empty;
            if (!string.Equals(SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal)
                || !string.Equals(PlanSchemaVersion, OpenVisionRecipeLocatorRelativeBlobIntentSkill.PlanSchemaVersion, StringComparison.Ordinal)
                || !string.Equals(SkillId, OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId, StringComparison.Ordinal)
                || string.IsNullOrWhiteSpace(SkillVersion)
                || string.IsNullOrWhiteSpace(Producer)
                || string.IsNullOrWhiteSpace(ProducerVersion)
                || string.IsNullOrWhiteSpace(CreatedUtc)
                || SourceImageWidth <= 0
                || SourceImageHeight <= 0
                || !string.Equals(CoordinateFrame, OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame, StringComparison.Ordinal))
            {
                message = "Evidence packet schema, producer, dimensions, skill, or coordinate frame is invalid.";
                return false;
            }

            if (!ValidateHashedFile(SourceImagePath, SourceImageSha256, "source image", out message)
                || !ValidateHashedFile(LocatorTemplatePath, LocatorTemplateSha256, "locator template", out message)
                || !ValidateHashedFile(PreviewOverlayPath, PreviewOverlaySha256, "preview overlay", out message))
            {
                return false;
            }

            if (Candidates == null || Candidates.Count == 0 || string.IsNullOrWhiteSpace(SelectedCandidateId))
            {
                message = "At least one locator candidate and a selected candidate ID are required.";
                return false;
            }

            if (Candidates.Count < 2)
            {
                message = "At least two retained locator candidates are required; the second candidate is missing, so a single candidate cannot substantiate ScoreMargin.";
                return false;
            }

            HashSet<string> candidateIds = new HashSet<string>(StringComparer.Ordinal);
            HashSet<int> nativeIndexes = new HashSet<int>();
            int acceptedCount = 0;
            OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate selected = null;
            foreach (OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate candidate in Candidates)
            {
                if (candidate == null
                    || string.IsNullOrWhiteSpace(candidate.CandidateId)
                    || !candidateIds.Add(candidate.CandidateId)
                    || candidate.NativeIndex < 0
                    || !nativeIndexes.Add(candidate.NativeIndex)
                    || !string.Equals(candidate.CoordinateFrame, OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame, StringComparison.Ordinal)
                    || !IsFinite(candidate.CenterX)
                    || !IsFinite(candidate.CenterY)
                    || !IsFinite(candidate.Angle)
                    || !IsFinite(candidate.Scale)
                    || candidate.Scale <= 0D
                    || !IsFinite(candidate.Score)
                    || candidate.Score < 0D
                    || candidate.Score > 1D
                    || !IsFinite(candidate.ScoreMargin)
                    || candidate.ScoreMargin < 0D
                    || candidate.BoundsWidth <= 0
                    || candidate.BoundsHeight <= 0)
                {
                    message = "Candidate IDs, native indexes, frame, geometry, score, or bounds are invalid or duplicated.";
                    return false;
                }

                if (candidate.Accepted)
                {
                    acceptedCount++;
                }

                if (string.Equals(candidate.CandidateId, SelectedCandidateId, StringComparison.Ordinal))
                {
                    selected = candidate;
                }
            }

            if (selected == null || !selected.Accepted)
            {
                message = "SelectedCandidateId must identify an accepted candidate from this packet; unknown IDs fail closed.";
                return false;
            }

            if (acceptedCount != 1)
            {
                message = "Exactly one accepted locator candidate is required; ambiguous evidence must be reviewed before compilation.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(selected.OverlayPath)
                && !ValidateHashedFile(selected.OverlayPath, selected.OverlaySha256, "selected candidate overlay", out message))
            {
                return false;
            }

            return true;
        }

        internal bool TryGetSelectedCandidate(
            out OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate candidate,
            out string message)
        {
            candidate = null;
            if (!TryValidate(out message))
            {
                return false;
            }

            candidate = Candidates.FirstOrDefault(item =>
                item != null && string.Equals(item.CandidateId, SelectedCandidateId, StringComparison.Ordinal));
            if (candidate == null)
            {
                message = "Selected candidate ID is not present in the validated packet.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        internal static bool TryLoad(
            string path,
            out OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet,
            out string message)
        {
            packet = null;
            message = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    message = "Evidence packet file is missing.";
                    return false;
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                packet = JsonSerializer.Deserialize<OpenVisionRecipeLocatorRelativeBlobEvidencePacket>(File.ReadAllText(path), options);
                if (packet == null || !packet.TryValidate(out message))
                {
                    packet = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                packet = null;
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        internal bool TrySave(string path, out string message)
        {
            message = string.Empty;
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
                File.WriteAllText(path, JsonSerializer.Serialize(this, options));
                message = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                message = ex.GetBaseException().Message;
                return false;
            }
        }

        internal static string ComputeSha256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToHexString(sha256.ComputeHash(stream));
            }
        }

        private static bool ValidateHashedFile(string path, string expectedHash, string label, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(path)
                || !File.Exists(path)
                || !IsSha256(expectedHash))
            {
                message = "Evidence " + label + " path or SHA-256 is missing.";
                return false;
            }

            string actualHash;
            try
            {
                actualHash = ComputeSha256(path);
            }
            catch (Exception ex)
            {
                message = "Evidence " + label + " hash could not be read: " + ex.GetBaseException().Message;
                return false;
            }

            if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                message = "Evidence " + label + " SHA-256 mismatch; the packet is stale or altered.";
                return false;
            }

            return true;
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

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }

    internal sealed class OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
    {
        public string CandidateId { get; set; } = string.Empty;
        public int NativeIndex { get; set; }
        public bool Accepted { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; } = 1D;
        public int BoundsX { get; set; }
        public int BoundsY { get; set; }
        public int BoundsWidth { get; set; }
        public int BoundsHeight { get; set; }
        public double Score { get; set; }
        public double ScoreMargin { get; set; }
        public string CoordinateFrame { get; set; } = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame;
        public string OverlayPath { get; set; } = string.Empty;
        public string OverlaySha256 { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
