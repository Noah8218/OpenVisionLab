using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenVisionLab
{
    internal enum QualifiedRecipeSnapshotScope
    {
        InspectionJudgment,
        LocatorStability
    }

    internal enum QualifiedRecipeSnapshotLifecycleAction
    {
        Superseded,
        Revoked
    }

    internal sealed class QualifiedRecipeSnapshotCreateRequest
    {
        public QualifiedRecipeSnapshotScope Scope { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string QualificationNote { get; set; } = string.Empty;
        public string SourceRecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string PipelineFilePath { get; set; } = string.Empty;
        public string BatchSummaryFilePath { get; set; } = string.Empty;
        public string PredecessorSnapshotId { get; set; } = string.Empty;
        public string ChangeReason { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public QualifiedRecipeValidationSetSnapshot ValidationSet { get; set; } =
            new QualifiedRecipeValidationSetSnapshot();
        public List<QualifiedRecipeRuntimeFileSource> RuntimeFiles { get; set; } =
            new List<QualifiedRecipeRuntimeFileSource>();
    }

    [XmlRoot("QualifiedRecipeValidationSet")]
    public sealed class QualifiedRecipeValidationSetSnapshot
    {
        [XmlAttribute]
        public int SchemaVersion { get; set; } = 1;

        [XmlAttribute]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineName { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineDefinitionSha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string ImageSetSha256 { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [XmlArray("Dependencies")]
        [XmlArrayItem("Dependency")]
        public List<QualifiedRecipeDependencySource> Dependencies { get; set; } =
            new List<QualifiedRecipeDependencySource>();

        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public List<QualifiedRecipeValidationImageSource> Images { get; set; } =
            new List<QualifiedRecipeValidationImageSource>();
    }

    public sealed class QualifiedRecipeDependencySource
    {
        [XmlAttribute]
        public string LogicalPath { get; set; } = string.Empty;

        [XmlAttribute]
        public string SourcePath { get; set; } = string.Empty;

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;
    }

    public sealed class QualifiedRecipeValidationImageSource
    {
        [XmlAttribute]
        public string ExpectedOutcome { get; set; } = string.Empty;

        [XmlAttribute]
        public string SourcePath { get; set; } = string.Empty;

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }

    internal sealed class QualifiedRecipeRuntimeFileSource
    {
        public string Label { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
    }

    [XmlRoot("QualifiedRecipeSnapshot")]
    public sealed class QualifiedRecipeSnapshotManifest
    {
        [XmlAttribute]
        public int SchemaVersion { get; set; } = 1;

        [XmlAttribute]
        public string SnapshotId { get; set; } = string.Empty;

        [XmlAttribute]
        public string Scope { get; set; } = string.Empty;

        [XmlAttribute]
        public string CreatedAtUtc { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
        public string QualificationNote { get; set; } = string.Empty;
        public string SourceRecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string PipelineFile { get; set; } = "pipeline.xml";
        public string PipelineSha256 { get; set; } = string.Empty;
        public string ValidationSetFile { get; set; } = "validation-set.xml";
        public string ValidationSetSha256 { get; set; } = string.Empty;
        public string BatchSummaryFile { get; set; } = "evidence/summary.xml";
        public string BatchSummarySha256 { get; set; } = string.Empty;
        public string ReviewQueuePolicy { get; set; } = string.Empty;
        public string ReviewQueueSha256 { get; set; } = string.Empty;
        public string InventorySha256 { get; set; } = string.Empty;
        public string IdempotencyKeySha256 { get; set; } = string.Empty;
        public string PredecessorSnapshotId { get; set; } = string.Empty;
        public string ChangeReason { get; set; } = string.Empty;
        public QualifiedRecipeQualificationCounts Counts { get; set; } =
            new QualifiedRecipeQualificationCounts();

        [XmlArray("Dependencies")]
        [XmlArrayItem("Dependency")]
        public List<QualifiedRecipeArchivedFile> Dependencies { get; set; } =
            new List<QualifiedRecipeArchivedFile>();

        [XmlArray("RuntimeFingerprint")]
        [XmlArrayItem("File")]
        public List<QualifiedRecipeRuntimeFingerprint> RuntimeFingerprint { get; set; } =
            new List<QualifiedRecipeRuntimeFingerprint>();

        [XmlArray("EvidenceRows")]
        [XmlArrayItem("Row")]
        public List<QualifiedRecipeEvidenceRow> EvidenceRows { get; set; } =
            new List<QualifiedRecipeEvidenceRow>();
    }

    public sealed class QualifiedRecipeQualificationCounts
    {
        public int Total { get; set; }
        public int ExpectedOk { get; set; }
        public int ExpectedNg { get; set; }
        public int CorrectAccept { get; set; }
        public int CorrectReject { get; set; }
        public int FalseAccept { get; set; }
        public int FalseReject { get; set; }
        public int ExecutionError { get; set; }
        public int EvidenceGap { get; set; }
    }

    public sealed class QualifiedRecipeArchivedFile
    {
        [XmlAttribute]
        public string LogicalPath { get; set; } = string.Empty;

        [XmlAttribute]
        public string ArchivePath { get; set; } = string.Empty;

        [XmlAttribute]
        public long Size { get; set; }

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;
    }

    public sealed class QualifiedRecipeRuntimeFingerprint
    {
        [XmlAttribute]
        public string Label { get; set; } = string.Empty;

        [XmlAttribute]
        public string SourcePath { get; set; } = string.Empty;

        [XmlAttribute]
        public string FileVersion { get; set; } = string.Empty;

        [XmlAttribute]
        public long Size { get; set; }

        [XmlAttribute]
        public string Sha256 { get; set; } = string.Empty;
    }

    public sealed class QualifiedRecipeEvidenceRow
    {
        [XmlAttribute]
        public int Index { get; set; }

        [XmlAttribute]
        public string SampleName { get; set; } = string.Empty;

        [XmlAttribute]
        public string ExpectedOutcome { get; set; } = string.Empty;

        [XmlAttribute]
        public string ActualOutcome { get; set; } = string.Empty;

        [XmlAttribute]
        public bool JudgmentCorrect { get; set; }

        [XmlAttribute]
        public string SourceSha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string ReportFile { get; set; } = string.Empty;

        [XmlAttribute]
        public string ReportSha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineFile { get; set; } = string.Empty;

        [XmlAttribute]
        public string PipelineSha256 { get; set; } = string.Empty;

        [XmlAttribute]
        public string SourceFile { get; set; } = string.Empty;
    }

    internal sealed class QualifiedRecipeSnapshotPreflightResult
    {
        public bool Success => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();
        public VisionPipelineBatchRunSummary Summary { get; set; }
        public string PipelineSha256 { get; set; } = string.Empty;
        public string PipelineDefinitionSha256 { get; set; } = string.Empty;
        public string ReviewQueueSha256 { get; set; } = string.Empty;
        public QualifiedRecipeQualificationCounts Counts { get; set; } =
            new QualifiedRecipeQualificationCounts();
        public List<QualifiedRecipePreparedEvidenceRow> Rows { get; } =
            new List<QualifiedRecipePreparedEvidenceRow>();
        public List<QualifiedRecipeRuntimeFingerprint> RuntimeFingerprint { get; } =
            new List<QualifiedRecipeRuntimeFingerprint>();
    }

    internal sealed class QualifiedRecipePreparedEvidenceRow
    {
        public int Index { get; set; }
        public VisionPipelineBatchSampleRunResult Result { get; set; }
        public VisionPipelineRunReport Report { get; set; }
        public QualifiedRecipeValidationImageSource ValidationImage { get; set; }
        public string ReportDirectory { get; set; } = string.Empty;
        public string PipelinePath { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
    }

    internal sealed class QualifiedRecipeSnapshotCreateResult
    {
        public bool Success { get; set; }
        public bool ReusedExisting { get; set; }
        public string SnapshotId { get; set; } = string.Empty;
        public string SnapshotDirectory { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    internal sealed class QualifiedRecipeSnapshotVerificationResult
    {
        public bool Success => Errors.Count == 0;
        public bool PayloadIntegrityValid { get; set; }
        public bool RuntimeFingerprintMatches { get; set; }
        public string SnapshotId { get; set; } = string.Empty;
        public QualifiedRecipeSnapshotManifest Manifest { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }

    [XmlRoot("QualifiedRecipeSnapshotLifecycleEvent")]
    public sealed class QualifiedRecipeSnapshotLifecycleEvent
    {
        [XmlAttribute]
        public int SchemaVersion { get; set; } = 1;

        [XmlAttribute]
        public int Sequence { get; set; }

        [XmlAttribute]
        public string SnapshotId { get; set; } = string.Empty;

        [XmlAttribute]
        public string Action { get; set; } = string.Empty;

        [XmlAttribute]
        public string OccurredAtUtc { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
        public string RelatedSnapshotId { get; set; } = string.Empty;
        public string PreviousEventSha256 { get; set; } = string.Empty;
        public string EventSha256 { get; set; } = string.Empty;
    }

    internal sealed class QualifiedRecipeSnapshotLifecycleState
    {
        public bool Success => Errors.Count == 0;
        public string State { get; set; } = "Qualified";
        public List<QualifiedRecipeSnapshotLifecycleEvent> Events { get; } =
            new List<QualifiedRecipeSnapshotLifecycleEvent>();
        public List<string> Errors { get; } = new List<string>();
    }

    internal sealed class QualifiedRecipeWorkingCopyResult
    {
        public bool Success { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public string PipelineName { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
