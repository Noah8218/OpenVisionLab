using System;
using System.Collections.Generic;
using OpenVisionLab.Common;
using OpenVisionLab;
using OpenVisionLab.Property;
using OpenVisionLab.Vision2D.Pipeline;
using OpenCvSharp;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--locator-relative-blob-runtime-pilot", StringComparison.OrdinalIgnoreCase))
{
    return await RunLocatorRelativeBlobRuntimePilotAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 3
    && string.Equals(args[0], "--locator-relative-blob-corpus-pilot", StringComparison.OrdinalIgnoreCase))
{
    return await RunLocatorRelativeBlobCorpusPilotAsync(args[1], args[2]);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--locator-relative-blob-dual-candidate-probe", StringComparison.OrdinalIgnoreCase))
{
    return await RunLocatorRelativeBlobDualCandidateProbeAsync(args.Length == 2 ? args[1] : null);
}

if ((args.Length == 1 || args.Length == 2)
    && string.Equals(args[0], "--locator-relative-blob-external-die-array-provenance", StringComparison.OrdinalIgnoreCase))
{
    return await RunLocatorRelativeBlobExternalDieArrayProvenanceAsync(args.Length == 2 ? args[1] : null);
}

if (args.Length == 6
    && string.Equals(args[0], "--locator-relative-blob-external-native-candidate", StringComparison.OrdinalIgnoreCase))
{
    return await RunLocatorRelativeBlobExternalNativeCandidateAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        args[5]);
}

string artifactRoot = Path.Combine(
    "D:\\OpenVisionLab-TestData",
    "OpenVisionLab_Dev",
    "locator-relative-blob-v1.1-contract-20260829");
Directory.CreateDirectory(artifactRoot);
string sourcePath = Path.Combine(artifactRoot, "source.bin");
string templatePath = Path.Combine(artifactRoot, "locator-template.bin");
string overlayPath = Path.Combine(artifactRoot, "locator-overlay.bin");
string packetPath = Path.Combine(artifactRoot, "evidence.packet.json");
File.WriteAllBytes(sourcePath, new byte[] { 1, 2, 3, 4, 5 });
File.WriteAllBytes(templatePath, new byte[] { 9, 8, 7, 6 });
File.WriteAllBytes(overlayPath, new byte[] { 4, 3, 2, 1 });

Assert(
    OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
        templatePath,
        "0,0,572,420",
        "320,180,60,50",
        "120,100,0,1,572,420",
        "0.8",
        "10",
        "-5",
        "5",
        "0.8",
        "1.8",
        "0.25",
        "170",
        "700",
        "1300",
        string.Empty,
        out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
        out string planMessage),
    "plan should validate: " + planMessage);

VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
Assert(
    OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
    "compiled pipeline should match the locked contract: " + pipelineMessage);
Assert(pipeline.Steps.Count == 5, "compiled pipeline must have five steps");
Assert(pipeline.Steps[0].ToolType == "Matching" && pipeline.Steps[1].ToolType == "Matching", "locator prefix must use two Matching steps");
Assert(pipeline.Steps[2].ToolType == "RotateScale" && pipeline.Steps[3].ToolType == "Threshold" && pipeline.Steps[4].ToolType == "Blob", "step order must be Matching, Matching, RotateScale, Threshold, Blob");
Assert(pipeline.Steps[4].UseAcceptance == false, "blank expected count must remain measurement-only");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket packet = new OpenVisionRecipeLocatorRelativeBlobEvidencePacket
{
    SourceImagePath = sourcePath,
    SourceImageSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(sourcePath),
    LocatorTemplatePath = templatePath,
    LocatorTemplateSha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(templatePath),
    PreviewOverlayPath = overlayPath,
    PreviewOverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath),
    SourceImageWidth = 572,
    SourceImageHeight = 420,
    CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
    Producer = "LocatorRelativeBlobSkillSmoke",
    ProducerVersion = "20260829",
    CreatedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
    SelectedCandidateId = "locator-0",
    Candidates = new List<OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate>
    {
        new OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
        {
            CandidateId = "locator-0",
            NativeIndex = 0,
            Accepted = true,
            CenterX = 120,
            CenterY = 100,
            Angle = 0,
            Scale = 1,
            BoundsX = 100,
            BoundsY = 80,
            BoundsWidth = 40,
            BoundsHeight = 40,
            Score = 0.95,
            ScoreMargin = 12,
            CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
            OverlayPath = overlayPath,
            OverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath),
            Reason = "reviewed top locator candidate"
        },
        new OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
        {
            CandidateId = "locator-1",
            NativeIndex = 1,
            Accepted = false,
            CenterX = 300,
            CenterY = 200,
            Angle = 0,
            Scale = 1,
            BoundsX = 280,
            BoundsY = 180,
            BoundsWidth = 40,
            BoundsHeight = 40,
            Score = 0.83,
            ScoreMargin = 12,
            CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
            OverlayPath = overlayPath,
            OverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath),
            Reason = "lower-ranked candidate retained for margin review"
        }
    }
};
Assert(packet.TrySave(packetPath, out string saveMessage), "packet should save: " + saveMessage);
Assert(OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(packetPath, out OpenVisionRecipeLocatorRelativeBlobEvidencePacket loaded, out string loadMessage), "packet should reload: " + loadMessage);
Assert(OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(loaded, plan, out VisionPipeline compiled, out string compileMessage), "evidence packet should compile: " + compileMessage);
Assert(compiled.Steps.Count == 5, "evidence compilation must preserve the five-step deterministic plan");

string reviewDecisionPath = Path.Combine(artifactRoot, "review-decision.template.json");
Assert(
    OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryCreatePending(
        packetPath,
        loaded,
        plan,
        out OpenVisionRecipeLocatorRelativeBlobReviewDecision pendingReview,
        out string pendingReviewMessage),
    "pending review decision should be created: " + pendingReviewMessage);
Assert(pendingReview.Decision == OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pending, "new review decision must remain pending");
Assert(pendingReview.VisualCorrespondence == OpenVisionRecipeLocatorRelativeBlobReviewDecision.NotReviewed, "new review decision must remain not reviewed");
Assert(pendingReview.TrySave(reviewDecisionPath, out string reviewSaveMessage), "pending review decision should save: " + reviewSaveMessage);
Assert(
    OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
        reviewDecisionPath,
        out OpenVisionRecipeLocatorRelativeBlobReviewDecision loadedReview,
        out string reviewLoadMessage),
    "review decision should reload: " + reviewLoadMessage);
Assert(
    loadedReview.TryValidateAgainst(packetPath, loaded, plan, out string reviewValidationMessage),
    "review decision should match the packet and plan: " + reviewValidationMessage);

OpenVisionRecipeLocatorRelativeBlobReviewDecision syntheticApprovedReview = CloneReviewDecision(loadedReview);
Assert(
    syntheticApprovedReview.TryApplyOperatorDecision(
        OpenVisionRecipeLocatorRelativeBlobReviewDecision.Approved,
        OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pass,
        "contract-smoke",
        "Synthetic contract fixture only; not physical locator approval.",
        DateTimeOffset.UtcNow,
        out string approvedReviewMessage),
    "a fully explicit synthetic approval should apply: " + approvedReviewMessage);
Assert(
    syntheticApprovedReview.TryValidateAgainst(packetPath, loaded, plan, out approvedReviewMessage),
    "a fully explicit synthetic approval should validate: " + approvedReviewMessage);
string approvedReviewPath = Path.Combine(artifactRoot, "review-decision.approved.json");
Assert(syntheticApprovedReview.TrySave(approvedReviewPath, out approvedReviewMessage), "approved review decision should save: " + approvedReviewMessage);
Assert(
    OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryLoad(
        approvedReviewPath,
        out OpenVisionRecipeLocatorRelativeBlobReviewDecision reloadedApprovedReview,
        out approvedReviewMessage)
        && reloadedApprovedReview.TryValidateAgainst(packetPath, loaded, plan, out approvedReviewMessage),
    "approved review decision should reload and remain current: " + approvedReviewMessage);

OpenVisionRecipeLocatorRelativeBlobReviewDecision staleReview = CloneReviewDecision(loadedReview);
staleReview.PlanFingerprint = new string('0', 64);
Assert(!staleReview.TryValidateAgainst(packetPath, loaded, plan, out _), "stale plan fingerprint must fail closed");

OpenVisionRecipeLocatorRelativeBlobReviewDecision stalePacketReview = CloneReviewDecision(loadedReview);
stalePacketReview.EvidencePacketSha256 = new string('0', 64);
Assert(!stalePacketReview.TryValidateAgainst(packetPath, loaded, plan, out _), "stale packet hash must fail closed");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket missingSecondCandidate = Clone(loaded);
missingSecondCandidate.Candidates.RemoveAt(1);
Assert(!missingSecondCandidate.TryValidate(out string missingSecondMessage)
    && missingSecondMessage.Contains("second candidate", StringComparison.OrdinalIgnoreCase),
    "a single retained candidate must not substantiate ScoreMargin");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket legacySchema = Clone(loaded);
legacySchema.SchemaVersion = "locator-relative-blob-evidence-v1";
Assert(!legacySchema.TryValidate(out _), "legacy v1 evidence schema must fail closed without explicit migration");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket unknownId = Clone(loaded);
unknownId.SelectedCandidateId = "unknown-candidate";
Assert(!unknownId.TryValidate(out _), "unknown candidate ID must fail closed");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket hashMismatch = Clone(loaded);
hashMismatch.SourceImageSha256 = new string('0', 64);
Assert(!hashMismatch.TryValidate(out _), "source hash mismatch must fail closed");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket wrongFrame = Clone(loaded);
wrongFrame.Candidates[0].CoordinateFrame = "Main";
Assert(!wrongFrame.TryValidate(out _), "wrong coordinate frame must fail closed");

OpenVisionRecipeLocatorRelativeBlobEvidencePacket ambiguous = Clone(loaded);
ambiguous.Candidates.Add(new OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
{
    CandidateId = "locator-2",
    NativeIndex = 2,
    Accepted = true,
    CenterX = 300,
    CenterY = 200,
    Angle = 0,
    Scale = 1,
    BoundsX = 280,
    BoundsY = 180,
    BoundsWidth = 40,
    BoundsHeight = 40,
    Score = 0.94,
    ScoreMargin = 1,
    CoordinateFrame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
    OverlayPath = overlayPath,
    OverlaySha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath)
});
Assert(!ambiguous.TryValidate(out _), "multiple accepted candidates must fail as ambiguous");

VisionPipeline tampered = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
tampered.Steps[4].Parameters["CvROI"] = "0,0,572,420";
Assert(!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(tampered, plan, out _), "tampered fixed ROI must fail the locked pipeline contract");
Assert(!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(null, plan, out _, out _), "compile without evidence must fail closed");

Console.WriteLine("LocatorRelativeBlobSkillSmoke: PASS");
Console.WriteLine("ArtifactRoot: " + artifactRoot);
Console.WriteLine("Packet: " + packetPath);
return 0;

static async Task<int> RunLocatorRelativeBlobRuntimePilotAsync(string? requestedEvidenceDirectory)
{
    string evidenceRoot = string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
        ? Path.Combine(
            "D:\\OpenVisionLab-TestData",
            "OpenVisionLab_Dev",
            "locator-relative-blob-runtime-pilot-20260827")
        : Path.GetFullPath(requestedEvidenceDirectory);
    Directory.CreateDirectory(evidenceRoot);

    string templatePath = Path.GetFullPath(Path.Combine(
        "docs",
        "samples",
        "public",
        "templates",
        "Fixture_Locator_Synthetic_Template.png"));
    string[] samplePaths =
    {
        Path.GetFullPath(Path.Combine("docs", "samples", "public", "Fixture_Pad_Synthetic_Shifted_OK.png")),
        Path.GetFullPath(Path.Combine("docs", "samples", "public", "Fixture_Pad_Synthetic_Shifted_Missing_NG.png"))
    };
    string[] sampleRoles = { "Good", "Missing-Bad" };
    Assert(File.Exists(templatePath), "public locator template must exist");
    string evidenceTemplatePath = Path.Combine(evidenceRoot, "locator-template.png");
    File.Copy(templatePath, evidenceTemplatePath, true);

    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
            evidenceTemplatePath,
            "0,0,572,420",
            "320,180,60,50",
            "120,100,0,1,572,420",
            "0.8",
            "10",
            "-5",
            "5",
            "0.8",
            "1.8",
            "0.25",
            "170",
            "700",
            "1300",
            string.Empty,
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string planMessage),
        "runtime pilot plan should validate: " + planMessage);

    VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
        "runtime pilot pipeline should match the locked contract: " + pipelineMessage);

    List<string> freezeRows = new List<string>
    {
        "PilotOnly=true",
        "Qualification=false",
        "SkillId=" + OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId,
        "TemplatePath=" + templatePath,
        "EvidenceTemplatePath=" + evidenceTemplatePath,
        "TemplateSha256=" + OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(templatePath),
        "Role\tPath\tSha256\tWidth\tHeight"
    };
    List<string> observations = new List<string>();

    for (int sampleIndex = 0; sampleIndex < samplePaths.Length; sampleIndex++)
    {
        string samplePath = samplePaths[sampleIndex];
        string role = sampleRoles[sampleIndex];
        Assert(File.Exists(samplePath), "public pilot sample must exist: " + samplePath);
        string sampleDirectory = Path.Combine(evidenceRoot, role.ToLowerInvariant());
        Directory.CreateDirectory(sampleDirectory);
        string evidenceSourcePath = Path.Combine(sampleDirectory, "source.png");
        string evidencePipelinePath = Path.Combine(sampleDirectory, "pipeline.xml");
        File.Copy(samplePath, evidenceSourcePath, true);
        File.WriteAllBytes(evidencePipelinePath, VisionPipelineExecutionPlan.SerializePipeline(pipeline));

        using Mat source = Cv2.ImRead(samplePath, ImreadModes.Color);
        Assert(!source.Empty(), "public pilot sample could not be decoded: " + samplePath);
        freezeRows.Add(string.Join(
            "\t",
            role,
            samplePath,
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(samplePath),
            source.Width.ToString(CultureInfo.InvariantCulture),
            source.Height.ToString(CultureInfo.InvariantCulture)));

        VisionPipelineRunResult run;
        using (VisionPipelineContext context = new VisionPipelineContext())
        {
            context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
            run = await VisionPipelineExecutionService.RunAsync(
                pipeline,
                context,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                CancellationToken.None);
        }

        try
        {
            Assert(run.StepResults.Count > 0, role + " runtime result must contain the locator step");
            VisionPipelineStepResult locatorResult = run.StepResults[0];
            Assert(locatorResult.ToolResult?.Success == true, role + " locator step must succeed");
            Assert(locatorResult.AcceptancePassed, role + " locator ambiguity gate must pass");
            Assert((locatorResult.ToolResult?.Overlays?.Count ?? 0) > 0, role + " locator overlay must be retained");

            string packetPath;
            string overlayPath;
            Assert(
                OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
                    plan,
                    pipeline,
                    run,
                    source,
                    evidenceSourcePath,
                    sampleDirectory,
                    "20260827-runtime-pilot",
                    out OpenVisionRecipeLocatorRelativeBlobEvidencePacket exported,
                    out packetPath,
                    out overlayPath,
                    out string exportMessage),
                role + " runtime evidence export failed: " + exportMessage);
            Assert(File.Exists(packetPath), role + " evidence packet must be written");
            Assert(File.Exists(overlayPath), role + " current locator overlay must be written");
            Assert(
                OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                    packetPath,
                    out OpenVisionRecipeLocatorRelativeBlobEvidencePacket loaded,
                    out string loadMessage),
                role + " evidence packet must reload: " + loadMessage);
            Assert(
                OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                    loaded,
                    plan,
                    out VisionPipeline compiled,
                    out string compileMessage),
                role + " evidence packet must compile: " + compileMessage);

            OpenVisionRecipeLocatorRelativeBlobEvidencePacket unknown = Clone(loaded);
            unknown.SelectedCandidateId = "unknown-candidate";
            Assert(!unknown.TryValidate(out _), role + " unknown candidate ID must fail closed");

            VisionPipelineRunResult compiledRun;
            using (VisionPipelineContext compiledContext = new VisionPipelineContext())
            {
                compiledContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
                compiledRun = await VisionPipelineExecutionService.RunAsync(
                    compiled,
                    compiledContext,
                    VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                    CancellationToken.None);
            }

            try
            {
                Assert(compiledRun.Success == run.Success, role + " compiled runtime outcome must match the source run");
            }
            finally
            {
                DisposeRunImages(compiledRun);
            }

            VisionPipelineStepResult? blobResult = run.StepResults.LastOrDefault(step =>
                string.Equals(step?.Step?.ToolType, "Blob", StringComparison.OrdinalIgnoreCase));
            int blobIndex = run.StepResults.FindIndex(step =>
                string.Equals(step?.Step?.ToolType, "Blob", StringComparison.OrdinalIgnoreCase));
            Assert(blobResult != null && blobIndex >= 0, role + " runtime result must contain the Blob step");
            string resultCountText = blobResult?.ToolResult?.Metrics != null
                && blobResult.ToolResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double resultCount)
                    ? resultCount.ToString("0.###", CultureInfo.InvariantCulture)
                    : "missing";
            if (sampleIndex == 0)
            {
                Assert(run.Success, "Good pilot run must complete successfully");
                Assert(resultCountText == "1", "Good pilot Blob ResultCount must be 1");
                Assert((blobResult?.ToolResult?.Overlays?.Count ?? 0) > 0, "Good pilot Blob overlay must be retained");
            }
            else
            {
                Assert(!run.Success, "Missing-Bad pilot run must fail at the explicit Blob check");
                Assert(resultCountText == "0", "Missing-Bad pilot Blob ResultCount must be 0");
            }

            string blobOverlayPath = Path.Combine(sampleDirectory, "blob-runtime-overlay.png");
            if (run.StepResults.LastOrDefault()?.ToolResult?.ResultImage is Mat resultImage
                && !resultImage.Empty())
            {
                Cv2.ImWrite(Path.Combine(sampleDirectory, "result.png"), resultImage);
                Assert(
                    SaveStepOverlay(resultImage, blobResult!, blobIndex + 1, plan.InspectionRoi, blobOverlayPath, out string blobOverlayMessage),
                    role + " Blob overlay could not be rendered: " + blobOverlayMessage);
            }
            else
            {
                throw new InvalidOperationException(role + " runtime result image is missing.");
            }

            List<string> summary = new List<string>
            {
                "Role=" + role,
                "Source=" + samplePath,
                "EvidenceSource=" + evidenceSourcePath,
                "EvidenceTemplate=" + evidenceTemplatePath,
                "EvidencePipeline=" + evidencePipelinePath,
                "SourceSha256=" + OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(samplePath),
                "RuntimeSuccess=" + run.Success,
                "CompiledRuntimeSuccess=" + compiledRun.Success,
                "LocatorCandidateCount=" + VisionPipelineMatchResultStore.Get(locatorResult.ToolResult).Count.ToString(CultureInfo.InvariantCulture),
                "LocatorOverlayCount=" + (locatorResult.ToolResult?.Overlays?.Count ?? 0).ToString(CultureInfo.InvariantCulture),
                "BlobResultCount=" + resultCountText,
                "BlobOverlayCount=" + (blobResult?.ToolResult?.Overlays?.Count ?? 0).ToString(CultureInfo.InvariantCulture),
                "EvidencePacket=" + packetPath,
                "LocatorOverlay=" + overlayPath,
                "BlobOverlay=" + blobOverlayPath,
                "CompileMessage=" + compileMessage
            };
            foreach (VisionPipelineStepResult stepResult in run.StepResults)
            {
                summary.Add(
                    "Step=" + (stepResult.Step?.Name ?? string.Empty)
                    + "|ToolSuccess=" + (stepResult.ToolResult?.Success == true)
                    + "|Acceptance=" + stepResult.AcceptancePassed
                    + "|Overlays=" + (stepResult.ToolResult?.Overlays?.Count ?? 0)
                    + "|Metrics=" + VisionPipelineKnownMetrics.FormatMetrics(stepResult.ToolResult?.Metrics));
            }
            File.WriteAllLines(Path.Combine(sampleDirectory, "runtime-summary.txt"), summary);
            observations.Add(role + ": runtime=" + run.Success + ", BlobResultCount=" + resultCountText + ", packet=" + packetPath);
        }
        finally
        {
            DisposeRunImages(run);
        }
    }

    File.WriteAllLines(Path.Combine(evidenceRoot, "pilot-freeze-manifest.tsv"), freezeRows);
    File.WriteAllLines(Path.Combine(evidenceRoot, "observations.txt"), observations);
    File.WriteAllLines(
        Path.Combine(evidenceRoot, "completion.txt"),
        new[]
        {
            "Status=Pilot evidence complete; qualification incomplete",
            "Scope=locator-relative-blob-v1 public Good/missing-Bad runtime bridge",
            "Acceptance=Matching runtime candidate -> overlay -> hash-verified packet -> CandidateId-only compile -> explicit compiled replay",
            "Boundary=Two samples are not held-out qualification and do not prove field correctness.",
            "EvidenceRoot=" + evidenceRoot
        });

    Console.WriteLine("LocatorRelativeBlobRuntimePilot: PASS");
    Console.WriteLine("EvidenceRoot: " + evidenceRoot);
    Console.WriteLine("FreezeManifest: " + Path.Combine(evidenceRoot, "pilot-freeze-manifest.tsv"));
    return 0;
}

static async Task<int> RunLocatorRelativeBlobDualCandidateProbeAsync(string? requestedEvidenceDirectory)
{
    string evidenceRoot = string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
        ? Path.Combine(
            "D:\\OpenVisionLab-TestData",
            "OpenVisionLab_Dev",
            "locator-relative-blob-dual-candidate-probe-20260829")
        : Path.GetFullPath(requestedEvidenceDirectory);
    if (Directory.Exists(evidenceRoot)
        && Directory.EnumerateFileSystemEntries(evidenceRoot).Any())
    {
        throw new InvalidOperationException(
            "Dual-candidate probe output must be a new or empty directory: " + evidenceRoot);
    }

    Directory.CreateDirectory(evidenceRoot);
    string templatePath = Path.GetFullPath(Path.Combine(
        "docs",
        "samples",
        "public",
        "templates",
        "Fixture_Locator_Synthetic_Template.png"));
    string samplePath = Path.GetFullPath(Path.Combine(
        "docs",
        "samples",
        "public",
        "Fixture_Pad_Synthetic_Shifted_OK.png"));
    Assert(File.Exists(templatePath), "dual-candidate probe template must exist");
    Assert(File.Exists(samplePath), "dual-candidate probe source must exist");

    string evidenceTemplatePath = Path.Combine(evidenceRoot, "locator-template.png");
    string evidenceSourcePath = Path.Combine(evidenceRoot, "dual-candidate-source.png");
    File.Copy(templatePath, evidenceTemplatePath);

    using Mat source = Cv2.ImRead(samplePath, ImreadModes.Color);
    using Mat template = Cv2.ImRead(templatePath, ImreadModes.Color);
    Assert(!source.Empty(), "dual-candidate probe source could not be decoded");
    Assert(!template.Empty(), "dual-candidate probe template could not be decoded");
    Assert(template.Width <= source.Width && template.Height <= source.Height, "dual-candidate probe template must fit the source");

    Vec3b templateBackground = template.At<Vec3b>(0, 0);
    Scalar templateBackgroundColor = new Scalar(
        templateBackground.Item0,
        templateBackground.Item1,
        templateBackground.Item2);
    using Mat degradedTemplate = template.Clone();
    Cv2.Rectangle(
        degradedTemplate,
        new Rect(0, 0, Math.Max(1, template.Width / 2), Math.Max(1, template.Height / 2)),
        templateBackgroundColor,
        -1);
    Cv2.GaussianBlur(degradedTemplate, degradedTemplate, new OpenCvSharp.Size(3, 3), 0);

    Rect secondCandidateRect = new Rect(300, 100, template.Width, template.Height);
    using (Mat destination = new Mat(source, secondCandidateRect))
    {
        degradedTemplate.CopyTo(destination);
    }
    Assert(Cv2.ImWrite(evidenceSourcePath, source), "dual-candidate probe source could not be written");

    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
            evidenceTemplatePath,
            "0,0,572,420",
            "320,180,60,50",
            "120,100,0,1,572,420",
            "0.8",
            "10",
            "-5",
            "5",
            "0.8",
            "1.8",
            "0.25",
            "170",
            "700",
            "1300",
            string.Empty,
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string planMessage),
        "dual-candidate probe plan should validate: " + planMessage);

    VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
        "dual-candidate probe pipeline should validate: " + pipelineMessage);

    VisionPipelineRunResult run;
    using (VisionPipelineContext context = new VisionPipelineContext())
    {
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
        run = await VisionPipelineExecutionService.RunAsync(
            pipeline,
            context,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None);
    }

    try
    {
        Assert(run.StepResults.Count > 0, "dual-candidate probe runtime result must contain the locator step");
        VisionPipelineStepResult locatorResult = run.StepResults[0];
        IReadOnlyList<VisionPipelineMatchResultEvidence> matches =
            VisionPipelineMatchResultStore.Get(locatorResult.ToolResult);
        double observedScoreMargin = locatorResult.ToolResult?.Metrics != null
            && locatorResult.ToolResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMargin, out double scoreMargin)
                ? scoreMargin
                : double.NaN;
        Console.WriteLine(
            "DualCandidateProbe: candidates=" + matches.Count.ToString(CultureInfo.InvariantCulture)
            + ", scoreMargin=" + (double.IsNaN(observedScoreMargin) ? "missing" : observedScoreMargin.ToString("0.###", CultureInfo.InvariantCulture))
            + ", locatorSuccess=" + (locatorResult.ToolResult?.Success == true)
            + ", acceptance=" + locatorResult.AcceptancePassed);
        foreach (VisionPipelineMatchResultEvidence match in matches)
        {
            Console.WriteLine(
                "DualCandidateProbeCandidate: nativeIndex=" + match.NativeIndex.ToString(CultureInfo.InvariantCulture)
                + ", score=" + match.Score.ToString("0.###", CultureInfo.InvariantCulture));
        }
        Assert(locatorResult.ToolResult?.Success == true, "dual-candidate probe locator step must succeed");
        Assert(locatorResult.AcceptancePassed, "dual-candidate probe ambiguity gate must pass");
        Assert(matches.Count >= 2, "dual-candidate probe must retain at least two native candidates");

        Assert(
            OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
                plan,
                pipeline,
                run,
                source,
                evidenceSourcePath,
                evidenceRoot,
                "20260829-dual-candidate-probe",
                out OpenVisionRecipeLocatorRelativeBlobEvidencePacket exported,
                out string packetPath,
                out string overlayPath,
                out string exportMessage),
            "dual-candidate probe export should pass: " + exportMessage);
        Assert(exported.Candidates.Count >= 2, "dual-candidate packet must retain at least two candidates");
        Assert(File.Exists(packetPath) && File.Exists(overlayPath), "dual-candidate packet and overlay must exist");
        Assert(
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                packetPath,
                out OpenVisionRecipeLocatorRelativeBlobEvidencePacket loaded,
                out string loadMessage),
            "dual-candidate packet should reload: " + loadMessage);
        Assert(
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                loaded,
                plan,
                out VisionPipeline compiled,
                out string compileMessage),
            "dual-candidate packet should compile: " + compileMessage);

        VisionPipelineRunResult compiledRun;
        using (VisionPipelineContext compiledContext = new VisionPipelineContext())
        {
            compiledContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
            compiledRun = await VisionPipelineExecutionService.RunAsync(
                compiled,
                compiledContext,
                VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                CancellationToken.None);
        }

        try
        {
            Assert(compiledRun.Success == run.Success, "dual-candidate compiled replay outcome must match the source run");
            File.WriteAllLines(
                Path.Combine(evidenceRoot, "probe-summary.txt"),
                new[]
                {
                    "Status=Complete",
                    "CandidateCount=" + matches.Count.ToString(CultureInfo.InvariantCulture),
                    "PacketSchema=locator-relative-blob-evidence-v1.1",
                    "Packet=" + packetPath,
                    "LocatorOverlay=" + overlayPath,
                    "SourceRunSuccess=" + run.Success,
                    "CompiledReplaySuccess=" + compiledRun.Success,
                    "ReplayOutcomeMatch=true"
                });
        }
        finally
        {
            DisposeRunImages(compiledRun);
        }

        Console.WriteLine("LocatorRelativeBlobDualCandidateProbe: PASS");
        Console.WriteLine("EvidenceRoot: " + evidenceRoot);
        Console.WriteLine("Packet: " + packetPath);
        return 0;
    }
    finally
    {
        DisposeRunImages(run);
    }
}

static void DisposeRunImages(VisionPipelineRunResult run)
{
    foreach (VisionPipelineStepResult stepResult in run?.StepResults ?? new List<VisionPipelineStepResult>())
    {
        stepResult?.ToolResult?.ResultImage?.Dispose();
    }
}

static async Task<int> RunLocatorRelativeBlobExternalDieArrayProvenanceAsync(string? requestedEvidenceDirectory)
{
    string evidenceRoot = string.IsNullOrWhiteSpace(requestedEvidenceDirectory)
        ? Path.Combine(
            "D:\\OpenVisionLab-TestData",
            "OpenVisionLab_Dev",
            "locator-relative-blob-external-die-array-provenance-20260829")
        : Path.GetFullPath(requestedEvidenceDirectory);
    if (Directory.Exists(evidenceRoot)
        && Directory.EnumerateFileSystemEntries(evidenceRoot).Any())
    {
        throw new InvalidOperationException(
            "External Die Array provenance output must be a new or empty directory: " + evidenceRoot);
    }

    string sourcePath = Path.GetFullPath(
        "E:\\라벨테스트\\EasyMatch_Die_Array_500(1)\\EasyMatch_Die_Array_500\\all_images\\OK\\die_array_003_ok.jpg");
    string templatePath = Path.GetFullPath(Path.Combine(
        "artifacts",
        "p227_auto_mpoint_six_corpus_pilot_20260724_r5_html",
        "templates",
        "die_array__Die1_rank_01.png"));
    Assert(File.Exists(sourcePath), "external Die Array source must exist");
    Assert(File.Exists(templatePath), "historical Die Array template evidence must exist");

    Directory.CreateDirectory(evidenceRoot);
    string evidenceSourcePath = Path.Combine(evidenceRoot, "source.jpg");
    string evidenceTemplatePath = Path.Combine(evidenceRoot, "locator-template.png");
    File.Copy(sourcePath, evidenceSourcePath);
    File.Copy(templatePath, evidenceTemplatePath);

    using Mat source = Cv2.ImRead(evidenceSourcePath, ImreadModes.Color);
    using Mat template = Cv2.ImRead(evidenceTemplatePath, ImreadModes.Color);
    Assert(!source.Empty() && source.Width == 512 && source.Height == 512, "external Die Array source must decode as 512x512");
    Assert(!template.Empty() && template.Width == 96 && template.Height == 96, "historical Die Array template must decode as 96x96");

    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
            evidenceTemplatePath,
            "0,0,512,512",
            "144,128,96,96",
            "192,176,0,1,512,512",
            "0.75",
            "10",
            "-8",
            "8",
            "0.9",
            "1.1",
            "0.25",
            "170",
            "700",
            "1300",
            string.Empty,
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string planMessage),
        "external Die Array provenance plan should validate: " + planMessage);

    VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
        "external Die Array provenance pipeline should match the locked contract: " + pipelineMessage);

    VisionPipelineRunResult run;
    using (VisionPipelineContext context = new VisionPipelineContext())
    {
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
        run = await VisionPipelineExecutionService.RunAsync(
            pipeline,
            context,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None);
    }

    try
    {
        Assert(run.StepResults.Count > 0, "external Die Array provenance run must contain the locator step");
        VisionPipelineStepResult locatorResult = run.StepResults[0];
        IReadOnlyList<VisionPipelineMatchResultEvidence> matches =
            VisionPipelineMatchResultStore.Get(locatorResult.ToolResult);
        double scoreMargin = locatorResult.ToolResult?.Metrics != null
            && locatorResult.ToolResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ScoreMargin, out double metricMargin)
                ? metricMargin
                : double.NaN;
        string overlayPath = Path.Combine(evidenceRoot, "locator-runtime-overlay.png");
        Assert(
            SaveStepOverlay(
                source,
                locatorResult,
                1,
                plan.InspectionRoi,
                overlayPath,
                out string overlayMessage),
            "external Die Array locator overlay should be written: " + overlayMessage);

        bool exporterAccepted = OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
            plan,
            pipeline,
            run,
            source,
            evidenceSourcePath,
            evidenceRoot,
            "20260829-external-die-array-provenance",
            out _,
            out string exporterPacketPath,
            out string exporterOverlayPath,
            out string exporterMessage);
        bool packetExistsAfterRefusal = File.Exists(Path.Combine(evidenceRoot, "evidence.packet.json"));
        Assert(!exporterAccepted, "external Die Array ambiguity probe must be refused by the evidence exporter");
        Assert(
            string.IsNullOrWhiteSpace(exporterPacketPath),
            "refused external Die Array export must not return a packet path");
        Assert(
            string.IsNullOrWhiteSpace(exporterOverlayPath),
            "refused external Die Array export must not return an exporter overlay path");
        Assert(
            exporterMessage.Contains("ambiguity gates", StringComparison.OrdinalIgnoreCase),
            "refused external Die Array export must report the ambiguity gate: " + exporterMessage);
        Assert(
            !packetExistsAfterRefusal,
            "refused external Die Array export must not create evidence.packet.json");

        string candidateState = matches.Count >= 2
            ? "ObservedSecondCandidate"
            : matches.Count == 1
                ? "MissingSecondCandidate"
                : "Unavailable";
        var provenance = new
        {
            schema_version = "locator-relative-blob-candidate-provenance-v1",
            status = candidateState,
            source_path = evidenceSourcePath,
            source_sha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(evidenceSourcePath),
            template_path = evidenceTemplatePath,
            template_sha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(evidenceTemplatePath),
            source_width = source.Width,
            source_height = source.Height,
            coordinate_frame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
            search_roi = "0,0,512,512",
            inspection_roi = "144,128,96,96",
            reference_pose = "192,176,0,1,512,512",
            runtime_success = run.Success,
            locator_tool_success = locatorResult.ToolResult?.Success == true,
            locator_acceptance_passed = locatorResult.AcceptancePassed,
            candidate_count = matches.Count,
            score_margin = FormatDouble(scoreMargin),
            overlay_path = overlayPath,
            packet_exported = exporterAccepted,
            exporter_attempted = true,
            exporter_refused = !exporterAccepted,
            exporter_message = exporterMessage,
            exporter_packet_path = exporterPacketPath,
            exporter_overlay_path = exporterOverlayPath,
            packet_exists_after_refusal = packetExistsAfterRefusal,
            visual_correspondence = "NOT_REVIEWED",
            qualification = false,
            reason = matches.Count >= 2
                ? "Two native candidates were retained, but the evidence exporter refused packet creation because the locator ambiguity gate failed; this historical repeated-grid candidate is not an approved locator."
                : "The native locator did not retain two candidates; no packet or ambiguity claim was created.",
            candidates = matches.Select((match, order) => new
            {
                order,
                native_index = match.NativeIndex,
                score = match.Score,
                center_x = match.CenterX,
                center_y = match.CenterY,
                bounds_x = match.BoundsX,
                bounds_y = match.BoundsY,
                bounds_width = match.BoundsWidth,
                bounds_height = match.BoundsHeight,
                angle = match.Angle,
                scale = match.Scale
            }).ToArray()
        };
        File.WriteAllText(
            Path.Combine(evidenceRoot, "candidate-provenance.json"),
            JsonSerializer.Serialize(provenance, new JsonSerializerOptions { WriteIndented = true }),
            Encoding.UTF8);
        File.WriteAllLines(
            Path.Combine(evidenceRoot, "probe-summary.txt"),
            new[]
            {
                "Status=Complete",
                "CandidateState=" + candidateState,
                "CandidateCount=" + matches.Count.ToString(CultureInfo.InvariantCulture),
                "ScoreMargin=" + FormatDouble(scoreMargin),
                "LocatorToolSuccess=" + (locatorResult.ToolResult?.Success == true),
                "LocatorAcceptancePassed=" + locatorResult.AcceptancePassed,
                "ExporterAttempted=true",
                "ExporterRefused=" + !exporterAccepted,
                "ExporterMessage=" + exporterMessage,
                "ExporterPacketPath=" + exporterPacketPath,
                "ExporterOverlayPath=" + exporterOverlayPath,
                "PacketExistsAfterRefusal=" + packetExistsAfterRefusal,
                "PacketExported=" + exporterAccepted,
                "VisualCorrespondence=NOT_REVIEWED",
                "Qualification=false",
                "Source=" + evidenceSourcePath,
                "Template=" + evidenceTemplatePath,
                "Overlay=" + overlayPath
            },
            Encoding.UTF8);

        Console.WriteLine(
            "ExternalDieArrayProvenance: candidates=" + matches.Count.ToString(CultureInfo.InvariantCulture)
            + ", scoreMargin=" + (double.IsNaN(scoreMargin) ? "missing" : scoreMargin.ToString("0.###", CultureInfo.InvariantCulture))
            + ", locatorSuccess=" + (locatorResult.ToolResult?.Success == true)
            + ", acceptance=" + locatorResult.AcceptancePassed);
        foreach (VisionPipelineMatchResultEvidence match in matches)
        {
            Console.WriteLine(
                "ExternalDieArrayCandidate: nativeIndex=" + match.NativeIndex.ToString(CultureInfo.InvariantCulture)
                + ", score=" + match.Score.ToString("0.###", CultureInfo.InvariantCulture)
                + ", center=" + match.CenterX.ToString("0.###", CultureInfo.InvariantCulture)
                + "," + match.CenterY.ToString("0.###", CultureInfo.InvariantCulture));
        }

        Assert(matches.Count >= 2, "external Die Array native run must retain a second candidate");
        Assert(!double.IsNaN(scoreMargin) && !double.IsInfinity(scoreMargin), "external Die Array score margin must be finite");
        Console.WriteLine("LocatorRelativeBlobExternalDieArrayProvenance: PASS");
        Console.WriteLine("EvidenceRoot: " + evidenceRoot);
        Console.WriteLine("Provenance: " + Path.Combine(evidenceRoot, "candidate-provenance.json"));
        return 0;
    }
    finally
    {
        DisposeRunImages(run);
    }
}

static async Task<int> RunLocatorRelativeBlobExternalNativeCandidateAsync(
    string requestedSourcePath,
    string requestedTemplatePath,
    string inspectionRoiText,
    string referencePoseText,
    string requestedEvidenceDirectory)
{
    string sourcePath = Path.GetFullPath(requestedSourcePath ?? string.Empty);
    string templatePath = Path.GetFullPath(requestedTemplatePath ?? string.Empty);
    string evidenceRoot = Path.GetFullPath(requestedEvidenceDirectory ?? string.Empty);
    if (!File.Exists(sourcePath))
    {
        throw new FileNotFoundException("The requested native candidate source image was not found.", sourcePath);
    }

    if (!File.Exists(templatePath))
    {
        throw new FileNotFoundException("The requested native candidate template was not found.", templatePath);
    }

    if (Directory.Exists(evidenceRoot)
        && Directory.EnumerateFileSystemEntries(evidenceRoot).Any())
    {
        throw new InvalidOperationException(
            "External native candidate output must be a new or empty directory: " + evidenceRoot);
    }

    Directory.CreateDirectory(evidenceRoot);
    string sourceExtension = Path.GetExtension(sourcePath);
    string templateExtension = Path.GetExtension(templatePath);
    string evidenceSourcePath = Path.Combine(
        evidenceRoot,
        "source" + (string.IsNullOrWhiteSpace(sourceExtension) ? ".img" : sourceExtension));
    string evidenceTemplatePath = Path.Combine(
        evidenceRoot,
        "locator-template" + (string.IsNullOrWhiteSpace(templateExtension) ? ".img" : templateExtension));
    File.Copy(sourcePath, evidenceSourcePath, false);
    File.Copy(templatePath, evidenceTemplatePath, false);

    using Mat source = Cv2.ImRead(evidenceSourcePath, ImreadModes.Color);
    using Mat template = Cv2.ImRead(evidenceTemplatePath, ImreadModes.Color);
    Assert(!source.Empty(), "the requested native candidate source could not be decoded");
    Assert(!template.Empty(), "the requested native candidate template could not be decoded");
    Assert(template.Width <= source.Width && template.Height <= source.Height, "the native template must fit the source image");

    string searchRoiText = string.Join(
        ",",
        "0",
        "0",
        source.Width.ToString(CultureInfo.InvariantCulture),
        source.Height.ToString(CultureInfo.InvariantCulture));
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
            evidenceTemplatePath,
            searchRoiText,
            inspectionRoiText,
            referencePoseText,
            "0.8",
            "10",
            "-5",
            "5",
            "0.8",
            "1.8",
            "0.25",
            "170",
            "700",
            "1300",
            string.Empty,
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string planMessage),
        "external native candidate plan should validate: " + planMessage);
    Assert(
        plan.ReferencePose.ImageWidth == source.Width && plan.ReferencePose.ImageHeight == source.Height,
        "the supplied reference pose dimensions must match the native source image");

    VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
        "external native candidate pipeline should match the locked contract: " + pipelineMessage);

    VisionPipelineRunResult run;
    using (VisionPipelineContext context = new VisionPipelineContext())
    {
        context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
        run = await VisionPipelineExecutionService.RunAsync(
            pipeline,
            context,
            VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
            CancellationToken.None);
    }

    VisionPipelineRunResult compiledRun = null;
    try
    {
        Assert(run.StepResults.Count > 0, "external native candidate run must contain the locator step");
        VisionPipelineStepResult locatorResult = run.StepResults[0];
        IReadOnlyList<VisionPipelineMatchResultEvidence> matches =
            VisionPipelineMatchResultStore.Get(locatorResult.ToolResult);
        double scoreMargin = matches.Count >= 2
            && locatorResult.ToolResult?.Metrics != null
            && locatorResult.ToolResult.Metrics.TryGetValue(
                VisionPipelineKnownMetrics.ScoreMargin,
                out double metricMargin)
                    ? metricMargin
                    : double.NaN;
        bool ambiguityGatePassed = locatorResult.ToolResult?.Success == true
            && locatorResult.AcceptancePassed == true
            && matches.Count >= 2
            && !double.IsNaN(scoreMargin)
            && !double.IsInfinity(scoreMargin)
            && scoreMargin >= plan.ScoreMargin;
        string candidateState = matches.Count >= 2
            ? "ObservedSecondCandidate"
            : matches.Count == 1
                ? "MissingSecondCandidate"
                : "Unavailable";
        string gateState = ambiguityGatePassed
            ? "AMBIGUITY_GATE_PASS"
            : "AMBIGUITY_GATE_FAIL";
        bool exporterAccepted = OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
            plan,
            pipeline,
            run,
            source,
            evidenceSourcePath,
            evidenceRoot,
            "20260829-external-native-candidate",
            out OpenVisionRecipeLocatorRelativeBlobEvidencePacket exportedPacket,
            out string exporterPacketPath,
            out string exporterOverlayPath,
            out string exporterMessage);
        string overlayPath = exporterAccepted && !string.IsNullOrWhiteSpace(exporterOverlayPath)
            ? exporterOverlayPath
            : Path.Combine(evidenceRoot, "locator-runtime-overlay.png");
        if (!File.Exists(overlayPath))
        {
            Assert(
                SaveStepOverlay(
                    source,
                    locatorResult,
                    1,
                    plan.InspectionRoi,
                    overlayPath,
                    out string overlayMessage),
                "external native candidate locator overlay should be written: " + overlayMessage);
        }

        string candidateReviewRoot = Path.Combine(evidenceRoot, "candidate-review");
        Directory.CreateDirectory(candidateReviewRoot);
        List<object> candidateRecords = new List<object>();
        bool reviewArtifactsComplete = true;
        string reviewArtifactFailure = string.Empty;
        foreach (VisionPipelineMatchResultEvidence match in matches.OrderByDescending(item => item.Score).ThenBy(item => item.NativeIndex))
        {
            string candidateLeaf = "candidate-" + match.NativeIndex.ToString(CultureInfo.InvariantCulture);
            string candidateDirectory = Path.Combine(candidateReviewRoot, candidateLeaf);
            Directory.CreateDirectory(candidateDirectory);
            bool artifactsSaved = SaveCandidateReviewArtifacts(
                source,
                template,
                match,
                candidateDirectory,
                out string patchPath,
                out string comparisonPath,
                out string blendPath,
                out string artifactMessage);
            if (!artifactsSaved)
            {
                reviewArtifactsComplete = false;
                reviewArtifactFailure = string.IsNullOrWhiteSpace(reviewArtifactFailure)
                    ? artifactMessage
                    : reviewArtifactFailure + " | " + artifactMessage;
            }

            candidateRecords.Add(new
            {
                native_index = match.NativeIndex,
                score = match.Score,
                center_x = match.CenterX,
                center_y = match.CenterY,
                bounds_x = match.BoundsX,
                bounds_y = match.BoundsY,
                bounds_width = match.BoundsWidth,
                bounds_height = match.BoundsHeight,
                angle = match.Angle,
                scale = match.Scale,
                visual_correspondence = "NOT_REVIEWED",
                patch_path = patchPath,
                patch_sha256 = File.Exists(patchPath)
                    ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(patchPath)
                    : string.Empty,
                comparison_path = comparisonPath,
                comparison_sha256 = File.Exists(comparisonPath)
                    ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(comparisonPath)
                    : string.Empty,
                blend_path = blendPath,
                blend_sha256 = File.Exists(blendPath)
                    ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(blendPath)
                    : string.Empty,
                artifact_message = artifactMessage
            });
        }

        bool packetReloaded = false;
        bool compiledReplaySuccess = false;
        bool replayOutcomeMatch = false;
        string replayMessage = string.Empty;
        string compiledPipelinePath = string.Empty;
        object[] stepRecords = run.StepResults
            .Select((step, index) => new
            {
                index = index + 1,
                name = step?.Step?.Name ?? string.Empty,
                tool_type = step?.Step?.ToolType ?? string.Empty,
                tool_success = step?.ToolResult?.Success == true,
                acceptance_passed = step?.AcceptancePassed == true,
                tool_message = step?.ToolResult?.Message ?? string.Empty,
                acceptance_message = step?.AcceptanceMessage ?? string.Empty,
                metrics = step?.ToolResult?.Metrics ?? new Dictionary<string, double>()
            })
            .Cast<object>()
            .ToArray();
        if (exporterAccepted && !string.IsNullOrWhiteSpace(exporterPacketPath))
        {
            if (!OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                    exporterPacketPath,
                    out OpenVisionRecipeLocatorRelativeBlobEvidencePacket loadedPacket,
                    out string loadMessage))
            {
                replayMessage = "packet reload failed: " + loadMessage;
            }
            else if (!OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                         loadedPacket,
                         plan,
                         out VisionPipeline compiledPipeline,
                         out string compileMessage))
            {
                replayMessage = "packet compile failed: " + compileMessage;
            }
            else
            {
                packetReloaded = true;
                Assert(
                    OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(
                        compiledPipeline,
                        plan,
                        out string compiledPipelineMessage),
                    "reloaded external native candidate pipeline should validate: " + compiledPipelineMessage);
                compiledPipelinePath = Path.Combine(evidenceRoot, "compiled.pipeline.xml");
                File.WriteAllBytes(
                    compiledPipelinePath,
                    VisionPipelineExecutionPlan.SerializePipeline(compiledPipeline));
                using (VisionPipelineContext compiledContext = new VisionPipelineContext())
                {
                    compiledContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
                    compiledRun = await VisionPipelineExecutionService.RunAsync(
                        compiledPipeline,
                        compiledContext,
                        VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                        CancellationToken.None);
                }

                compiledReplaySuccess = compiledRun.Success;
                replayOutcomeMatch = compiledRun.Success == run.Success;
                replayMessage = replayOutcomeMatch
                    ? "packet reload, compile, and replay preserved the runtime outcome"
                    : "compiled replay outcome differed from the source runtime";
            }
        }

        string reviewDecisionTemplatePath = string.Empty;
        string reviewDecisionTemplateMessage = string.Empty;
        bool reviewDecisionTemplateWritten = !exporterAccepted;
        if (exporterAccepted)
        {
            if (exportedPacket == null || string.IsNullOrWhiteSpace(exporterPacketPath))
            {
                reviewDecisionTemplateMessage = "The exported evidence packet was not available for review-decision preparation.";
            }
            else if (!OpenVisionRecipeLocatorRelativeBlobReviewDecision.TryCreatePending(
                         exporterPacketPath,
                         exportedPacket,
                         plan,
                         out OpenVisionRecipeLocatorRelativeBlobReviewDecision pendingReview,
                         out reviewDecisionTemplateMessage))
            {
                // Keep the runtime evidence, but fail the bounded diagnostic because its
                // explicit operator-review handoff could not be materialized.
            }
            else
            {
                reviewDecisionTemplatePath = Path.Combine(evidenceRoot, "review-decision.template.json");
                reviewDecisionTemplateWritten = pendingReview.TrySave(
                    reviewDecisionTemplatePath,
                    out reviewDecisionTemplateMessage);
            }
        }

        string reason = !ambiguityGatePassed
            ? matches.Count < 2
                ? "the native locator did not retain two candidates; ScoreMargin is not an observed competition result"
                : "two native candidates were retained, but the fixed ambiguity gate failed; visual correspondence remains NOT_REVIEWED"
            : "the fixed native ambiguity gate passed; REVIEW_ONLY still requires operator visual correspondence review before any Recipe approval";
        var provenance = new
        {
            schema_version = "locator-relative-blob-external-native-candidate-v1",
            status = ambiguityGatePassed ? "READY_FOR_OPERATOR_REVIEW" : "AUTO_REJECTED_AMBIGUOUS",
            selection_mode = "REVIEW_ONLY",
            operator_approval = "NOT_APPROVED",
            source_path = evidenceSourcePath,
            source_sha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(evidenceSourcePath),
            template_path = evidenceTemplatePath,
            template_sha256 = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(evidenceTemplatePath),
            source_width = source.Width,
            source_height = source.Height,
            coordinate_frame = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CoordinateFrame,
            search_roi = searchRoiText,
            inspection_roi = plan.InspectionRoi.ToText(),
            reference_pose = referencePoseText,
            score_minimum = plan.ScoreMinimum,
            score_margin_minimum = plan.ScoreMargin,
            angle_minimum = plan.AngleMinimum,
            angle_maximum = plan.AngleMaximum,
            scale_minimum = plan.ScaleRatioMinimum,
            scale_maximum = plan.ScaleRatioMaximum,
            candidate_state = candidateState,
            runtime_success = run.Success,
            locator_tool_success = locatorResult.ToolResult?.Success == true,
            locator_acceptance_passed = locatorResult.AcceptancePassed,
            candidate_count = matches.Count,
            score_margin = FormatDouble(scoreMargin),
            score_margin_state = candidateState,
            ambiguity_gate = gateState,
            overlay_path = overlayPath,
            overlay_sha256 = File.Exists(overlayPath)
                ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(overlayPath)
                : string.Empty,
            packet_exported = exporterAccepted,
            exporter_packet_path = exporterPacketPath,
            exporter_message = exporterMessage,
            packet_reloaded = packetReloaded,
            compiled_pipeline_path = compiledPipelinePath,
            compiled_replay_success = compiledReplaySuccess,
            replay_outcome_match = replayOutcomeMatch,
            replay_message = replayMessage,
            review_decision = exporterAccepted
                ? OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pending
                : string.Empty,
            review_decision_template_path = reviewDecisionTemplatePath,
            review_decision_template_written = reviewDecisionTemplateWritten,
            review_decision_template_message = reviewDecisionTemplateMessage,
            step_results = stepRecords,
            visual_correspondence = "NOT_REVIEWED",
            review_artifacts_complete = reviewArtifactsComplete,
            review_artifact_failure = reviewArtifactFailure,
            qualification = false,
            reason,
            candidates = candidateRecords
        };
        File.WriteAllText(
            Path.Combine(evidenceRoot, "candidate-provenance.json"),
            JsonSerializer.Serialize(provenance, new JsonSerializerOptions { WriteIndented = true }),
            Encoding.UTF8);
        File.WriteAllLines(
            Path.Combine(evidenceRoot, "probe-summary.txt"),
            new[]
            {
                "Status=Complete",
                "ReviewState=" + (ambiguityGatePassed ? "READY_FOR_OPERATOR_REVIEW" : "AUTO_REJECTED_AMBIGUOUS"),
                "SelectionMode=REVIEW_ONLY",
                "OperatorApproval=NOT_APPROVED",
                "CandidateState=" + candidateState,
                "CandidateCount=" + matches.Count.ToString(CultureInfo.InvariantCulture),
                "ScoreMargin=" + FormatDouble(scoreMargin),
                "AmbiguityGate=" + gateState,
                "LocatorToolSuccess=" + (locatorResult.ToolResult?.Success == true),
                "LocatorAcceptancePassed=" + locatorResult.AcceptancePassed,
                "ExporterAttempted=true",
                "ExporterAccepted=" + exporterAccepted,
                "ExporterMessage=" + exporterMessage,
                "Packet=" + exporterPacketPath,
                "PacketReloaded=" + packetReloaded,
                "CompiledReplaySuccess=" + compiledReplaySuccess,
                "ReplayOutcomeMatch=" + replayOutcomeMatch,
                "ReviewDecision=" + (exporterAccepted
                    ? OpenVisionRecipeLocatorRelativeBlobReviewDecision.Pending
                    : "NOT_CREATED"),
                "ReviewDecisionTemplate=" + reviewDecisionTemplatePath,
                "ReviewDecisionTemplateWritten=" + reviewDecisionTemplateWritten,
                "ReviewDecisionTemplateMessage=" + reviewDecisionTemplateMessage,
                "VisualCorrespondence=NOT_REVIEWED",
                "ReviewArtifactsComplete=" + reviewArtifactsComplete,
                "Qualification=false",
                "Source=" + evidenceSourcePath,
                "Template=" + evidenceTemplatePath,
                "Overlay=" + overlayPath,
                "CandidateReviewRoot=" + candidateReviewRoot,
                "Reason=" + reason
            },
            Encoding.UTF8);

        Console.WriteLine(
            "ExternalNativeCandidate: candidates=" + matches.Count.ToString(CultureInfo.InvariantCulture)
            + ", scoreMargin=" + (double.IsNaN(scoreMargin) ? "missing" : scoreMargin.ToString("0.###", CultureInfo.InvariantCulture))
            + ", locatorSuccess=" + (locatorResult.ToolResult?.Success == true)
            + ", acceptance=" + locatorResult.AcceptancePassed
            + ", gate=" + gateState
            + ", exporter=" + exporterAccepted);
        foreach (VisionPipelineMatchResultEvidence match in matches)
        {
            Console.WriteLine(
                "ExternalNativeCandidateMatch: nativeIndex=" + match.NativeIndex.ToString(CultureInfo.InvariantCulture)
                + ", score=" + match.Score.ToString("0.###", CultureInfo.InvariantCulture)
                + ", center=" + match.CenterX.ToString("0.###", CultureInfo.InvariantCulture)
                + "," + match.CenterY.ToString("0.###", CultureInfo.InvariantCulture));
        }
        Console.WriteLine("LocatorRelativeBlobExternalNativeCandidate: PASS");
        Console.WriteLine("EvidenceRoot: " + evidenceRoot);
        Console.WriteLine("Provenance: " + Path.Combine(evidenceRoot, "candidate-provenance.json"));
        return reviewArtifactsComplete
            && reviewDecisionTemplateWritten
            && (!exporterAccepted || packetReloaded && replayOutcomeMatch)
                ? 0
                : 1;
    }
    finally
    {
        DisposeRunImages(compiledRun);
        DisposeRunImages(run);
    }
}

static async Task<int> RunLocatorRelativeBlobCorpusPilotAsync(
    string requestedBatchRoot,
    string requestedOutputRoot)
{
    string batchRoot = Path.GetFullPath(requestedBatchRoot ?? string.Empty);
    string evidenceRoot = Path.GetFullPath(requestedOutputRoot ?? string.Empty);
    if (!Directory.Exists(batchRoot))
    {
        throw new DirectoryNotFoundException("The existing native batch evidence directory was not found: " + batchRoot);
    }

    if (Directory.Exists(evidenceRoot)
        && Directory.EnumerateFileSystemEntries(evidenceRoot).Any())
    {
        throw new InvalidOperationException(
            "Locator-relative Blob corpus pilot output must be a new or empty directory: " + evidenceRoot);
    }

    string summaryPath = Path.Combine(batchRoot, "summary.json");
    string nativeRowsPath = Path.Combine(batchRoot, "native_rows.csv");
    string batchTemplatePath = Path.Combine(batchRoot, "reference", "die_pad_1_template.png");
    foreach (string requiredPath in new[] { summaryPath, nativeRowsPath, batchTemplatePath })
    {
        if (!File.Exists(requiredPath))
        {
            throw new FileNotFoundException("Locator-relative Blob corpus pilot input was not found.", requiredPath);
        }
    }

    using JsonDocument summaryDocument = JsonDocument.Parse(File.ReadAllText(summaryPath, Encoding.UTF8));
    JsonElement summary = summaryDocument.RootElement;
    RequireJsonString(summary, "scenario", "matching-die-pad-batch");
    RequireJsonString(summary, "profile", "object-only-zero-reference");
    int expectedRows = RequireExpectedJsonInt(summary, "rows", 122);
    int passedRows = RequireExpectedJsonInt(summary, "passed", expectedRows);
    if (passedRows != expectedRows)
    {
        throw new InvalidOperationException(
            "The native batch must be completely passed before it can feed the locator skill. Rows="
            + expectedRows.ToString(CultureInfo.InvariantCulture)
            + ", Passed="
            + passedRows.ToString(CultureInfo.InvariantCulture));
    }

    JsonElement explicitRunContract = summary.GetProperty("explicit_run_contract");
    if (RequireExpectedJsonInt(explicitRunContract, "loads_without_preview", expectedRows) != expectedRows
        || RequireExpectedJsonInt(explicitRunContract, "explicit_preview_increment_one", expectedRows) != expectedRows
        || RequireExpectedJsonInt(explicitRunContract, "native_results", expectedRows) != expectedRows
        || RequireExpectedJsonInt(explicitRunContract, "native_preview_files", expectedRows) != expectedRows
        || !RequireJsonBool(explicitRunContract, "all_pass"))
    {
        throw new InvalidOperationException("The native batch explicit Preview contract is incomplete.");
    }

    List<Dictionary<string, string>> nativeRows = ReadCsv(nativeRowsPath);
    if (nativeRows.Count != expectedRows)
    {
        throw new InvalidOperationException(
            "Native batch row count changed. Expected="
            + expectedRows.ToString(CultureInfo.InvariantCulture)
            + ", Actual="
            + nativeRows.Count.ToString(CultureInfo.InvariantCulture));
    }

    JsonElement templateRoi = summary.GetProperty("template").GetProperty("roi");
    int templateX = RequireJsonInt(templateRoi, "X");
    int templateY = RequireJsonInt(templateRoi, "Y");
    int templateWidth = RequireJsonInt(templateRoi, "Width");
    int templateHeight = RequireJsonInt(templateRoi, "Height");
    string templateHash = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(batchTemplatePath);
    string expectedTemplateHash = RequireJsonString(summary.GetProperty("template"), "sha256");
    if (!string.Equals(templateHash, expectedTemplateHash, StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "The native batch template hash changed. Expected="
            + expectedTemplateHash
            + ", Actual="
            + templateHash);
    }

    JsonElement settings = summary.GetProperty("settings");
    double angleMinimum = RequireJsonDouble(settings, "angle_min");
    double angleMaximum = RequireJsonDouble(settings, "angle_max");
    double scaleMinimum = RequireJsonDouble(settings, "scale_min");
    double scaleMaximum = RequireJsonDouble(settings, "scale_max");
    string firstNativeSourcePath = RequireCsv(nativeRows[0], "SourcePath");
    string sourceDataset = Path.GetFullPath(
        Path.Combine(
            Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(firstNativeSourcePath) ?? firstNativeSourcePath) ?? firstNativeSourcePath)
                ?? firstNativeSourcePath));
    string copiedTemplateDirectory = Path.Combine(evidenceRoot, "template");
    string copiedTemplatePath = Path.Combine(copiedTemplateDirectory, "die_pad_1_template.png");
    Directory.CreateDirectory(copiedTemplateDirectory);
    File.Copy(batchTemplatePath, copiedTemplatePath, false);
    if (!string.Equals(
            OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(copiedTemplatePath),
            expectedTemplateHash,
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException("The copied locator template hash did not survive intake.");
    }

    string searchRoiText = "0,0,512,512";
    string inspectionRoiText = string.Join(
        ",",
        templateX.ToString(CultureInfo.InvariantCulture),
        templateY.ToString(CultureInfo.InvariantCulture),
        templateWidth.ToString(CultureInfo.InvariantCulture),
        templateHeight.ToString(CultureInfo.InvariantCulture));
    string referencePoseText = string.Join(
        ",",
        (templateX + (templateWidth / 2D)).ToString(CultureInfo.InvariantCulture),
        (templateY + (templateHeight / 2D)).ToString(CultureInfo.InvariantCulture),
        "0",
        "1",
        "512",
        "512");
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCreatePlan(
            copiedTemplatePath,
            searchRoiText,
            inspectionRoiText,
            referencePoseText,
            "0.8",
            "10",
            angleMinimum.ToString(CultureInfo.InvariantCulture),
            angleMaximum.ToString(CultureInfo.InvariantCulture),
            scaleMinimum.ToString(CultureInfo.InvariantCulture),
            scaleMaximum.ToString(CultureInfo.InvariantCulture),
            "0.25",
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultThreshold.ToString(CultureInfo.InvariantCulture),
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultMinimumArea.ToString(CultureInfo.InvariantCulture),
            OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultMaximumArea.ToString(CultureInfo.InvariantCulture),
            string.Empty,
            out OpenVisionRecipeLocatorRelativeBlobIntentSkill.Plan plan,
            out string planMessage),
        "corpus pilot plan should validate: " + planMessage);
    VisionPipeline pipeline = OpenVisionRecipeLocatorRelativeBlobIntentSkill.CreateMeasurementPipeline(plan);
    Assert(
        OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(pipeline, plan, out string pipelineMessage),
        "corpus pilot pipeline should match the locked contract: " + pipelineMessage);

    string rowsDirectory = Path.Combine(evidenceRoot, "rows");
    Directory.CreateDirectory(rowsDirectory);
    List<string> intakeRows = new List<string>
    {
        "RowId,Split,RoleLabelOnly,SourceSha256,NativeOverlaySha256,PacketSha256,LocatorCandidateCount,LocatorScore,LocatorScoreMargin,LocatorScoreMarginState,BlobResultCount,SourceRunSuccess,CompiledReplaySuccess,ReplayOutcomeMatch,Status,Reason,SourcePath,PacketPath,LocatorOverlayPath,BlobOverlayPath"
    };
    List<string> freezeRows = new List<string>
    {
        "PilotOnly=true",
        "Qualification=false",
        "SkillId=" + OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId,
        "SourceDataset=" + sourceDataset,
        "NativeBatchEvidence=" + batchRoot,
        "TemplateSha256=" + expectedTemplateHash,
        "Plan=search:" + searchRoiText + "|inspection:" + inspectionRoiText + "|reference:" + referencePoseText,
        "RowId\tSplit\tRoleLabelOnly\tSourceSha256\tPacketSha256\tStatus\tReason"
    };
    int packetCount = 0;
    int replayCount = 0;
    int replayMatchCount = 0;
    int locatorRejectedCount = 0;
    int observedSecondCandidateCount = 0;
    int missingSecondCandidateCount = 0;
    int unavailableCandidateCount = 0;
    int errorCount = 0;

    foreach (Dictionary<string, string> nativeRow in nativeRows
        .OrderBy(row => row.GetValueOrDefault("Split"), StringComparer.OrdinalIgnoreCase)
        .ThenBy(row => row.GetValueOrDefault("RoleLabelOnly"), StringComparer.OrdinalIgnoreCase)
        .ThenBy(row => row.GetValueOrDefault("RowId"), StringComparer.Ordinal))
    {
        string rowId = RequireCsv(nativeRow, "RowId");
        string split = RequireCsv(nativeRow, "Split");
        string roleLabelOnly = RequireCsv(nativeRow, "RoleLabelOnly");
        string sourcePath = Path.GetFullPath(RequireCsv(nativeRow, "SourcePath"));
        string expectedSourceSha = RequireCsv(nativeRow, "SourceSha256");
        string nativeOverlayPath = RequireCsv(nativeRow, "EvidenceOverlayPath");
        string rowDirectory = Path.Combine(rowsDirectory, SafeLeaf(rowId));
        Directory.CreateDirectory(rowDirectory);
        string copiedSourcePath = Path.Combine(rowDirectory, "source.jpg");
        string copiedNativeOverlayPath = Path.Combine(rowDirectory, "native-batch-overlay.png");
        string packetPath = string.Empty;
        string locatorOverlayPath = string.Empty;
        string blobOverlayPath = string.Empty;
        string reason = string.Empty;
        string status = "Rejected";
        int candidateCount = 0;
        double locatorScore = double.NaN;
        double locatorScoreMargin = double.NaN;
        string locatorScoreMarginState = "Unavailable";
        string blobResultCount = "missing";
        bool sourceRunSuccess = false;
        bool compiledReplaySuccess = false;
        bool replayOutcomeMatch = false;
        VisionPipelineRunResult run = null;
        VisionPipelineRunResult compiledRun = null;
        try
        {
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("Corpus source image is missing.", sourcePath);
            }

            string actualSourceSha = OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(sourcePath);
            if (!string.Equals(actualSourceSha, expectedSourceSha, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Corpus source hash mismatch. Expected=" + expectedSourceSha + ", Actual=" + actualSourceSha);
            }

            File.Copy(sourcePath, copiedSourcePath, false);
            if (File.Exists(nativeOverlayPath))
            {
                File.Copy(nativeOverlayPath, copiedNativeOverlayPath, false);
            }

            using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Color);
            if (source.Empty() || source.Width != 512 || source.Height != 512)
            {
                throw new InvalidOperationException(
                    "Corpus source must decode as a 512x512 image: " + sourcePath);
            }

            using (VisionPipelineContext context = new VisionPipelineContext())
            {
                context.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
                run = await VisionPipelineExecutionService.RunAsync(
                    pipeline,
                    context,
                    VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                    CancellationToken.None);
            }
            sourceRunSuccess = run.Success;

            VisionPipelineStepResult locatorResult = run.StepResults?.FirstOrDefault();
            if (locatorResult?.ToolResult != null)
            {
                IReadOnlyList<VisionPipelineMatchResultEvidence> candidates =
                    VisionPipelineMatchResultStore.Get(locatorResult.ToolResult);
                candidateCount = candidates?.Count ?? 0;
                locatorScoreMarginState = candidateCount >= 2
                    ? "ObservedSecondCandidate"
                    : candidateCount == 1
                        ? "MissingSecondCandidate"
                        : "Unavailable";
                if (candidateCount >= 2
                    && locatorResult.ToolResult.Metrics.TryGetValue(
                        VisionPipelineKnownMetrics.ScoreMargin,
                        out double margin))
                {
                    locatorScoreMargin = margin;
                }

                if (candidates != null && candidates.Count > 0)
                {
                    locatorScore = candidates.Max(candidate => candidate.Score);
                }
            }

            if (string.Equals(locatorScoreMarginState, "ObservedSecondCandidate", StringComparison.Ordinal))
            {
                observedSecondCandidateCount++;
            }
            else if (string.Equals(locatorScoreMarginState, "MissingSecondCandidate", StringComparison.Ordinal))
            {
                missingSecondCandidateCount++;
            }
            else
            {
                unavailableCandidateCount++;
            }

            if (locatorResult?.ToolResult?.Success != true || locatorResult.AcceptancePassed != true)
            {
                locatorRejectedCount++;
                reason = "locator ambiguity/success gate rejected the native runtime result";
            }
            else if (candidateCount < 2)
            {
                locatorRejectedCount++;
                reason = "locator second-candidate evidence was not retained; ScoreMargin is not observed competition evidence";
            }
            else
            {
                if (!OpenVisionRecipeLocatorRelativeBlobEvidenceExporter.TryExport(
                        plan,
                        pipeline,
                        run,
                        source,
                        copiedSourcePath,
                        rowDirectory,
                        "20260827-corpus-pilot",
                        out OpenVisionRecipeLocatorRelativeBlobEvidencePacket exported,
                        out packetPath,
                        out locatorOverlayPath,
                        out string exportMessage))
                {
                    throw new InvalidOperationException("Evidence packet export failed: " + exportMessage);
                }

                if (!OpenVisionRecipeLocatorRelativeBlobEvidencePacket.TryLoad(
                        packetPath,
                        out OpenVisionRecipeLocatorRelativeBlobEvidencePacket loaded,
                        out string loadMessage))
                {
                    throw new InvalidOperationException("Evidence packet reload failed: " + loadMessage);
                }

                Assert(
                    OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryCompile(
                        loaded,
                        plan,
                        out VisionPipeline compiled,
                        out string compileMessage),
                    "compiled corpus packet should validate: " + compileMessage);
                Assert(
                    OpenVisionRecipeLocatorRelativeBlobIntentSkill.TryValidatePipeline(
                        compiled,
                        plan,
                        out string compiledPipelineMessage),
                    "compiled corpus packet pipeline should validate: " + compiledPipelineMessage);
                File.WriteAllBytes(
                    Path.Combine(rowDirectory, "compiled.pipeline.xml"),
                    VisionPipelineExecutionPlan.SerializePipeline(compiled));

                using (VisionPipelineContext compiledContext = new VisionPipelineContext())
                {
                    compiledContext.SetLayer(VisionRecipeRunner.DefaultInputLayer, source.Clone());
                    compiledRun = await VisionPipelineExecutionService.RunAsync(
                        compiled,
                        compiledContext,
                        VisionRecipeRunner.DefaultStepTimeoutMilliseconds,
                        CancellationToken.None);
                }
                compiledReplaySuccess = compiledRun.Success;
                replayCount++;
                replayOutcomeMatch = compiledRun.Success == run.Success;
                if (replayOutcomeMatch)
                {
                    replayMatchCount++;
                }

                VisionPipelineStepResult blobResult = run.StepResults?.LastOrDefault(step =>
                    string.Equals(step?.Step?.ToolType, "Blob", StringComparison.OrdinalIgnoreCase));
                if (blobResult?.ToolResult?.Metrics != null
                    && blobResult.ToolResult.Metrics.TryGetValue(VisionPipelineKnownMetrics.ResultCount, out double resultCount))
                {
                    blobResultCount = resultCount.ToString("0.###", CultureInfo.InvariantCulture);
                }

                int blobIndex = run.StepResults?.FindIndex(step =>
                    string.Equals(step?.Step?.ToolType, "Blob", StringComparison.OrdinalIgnoreCase)) ?? -1;
                if (blobResult?.ToolResult?.ResultImage is Mat resultImage
                    && !resultImage.Empty()
                    && blobIndex >= 0)
                {
                    SaveStepOverlay(
                        resultImage,
                        blobResult,
                        blobIndex + 1,
                        plan.InspectionRoi,
                        Path.Combine(rowDirectory, "blob-runtime-overlay.png"),
                        out _);
                    blobOverlayPath = Path.Combine(rowDirectory, "blob-runtime-overlay.png");
                }

                packetCount++;
                status = replayOutcomeMatch ? "Accepted" : "ReplayMismatch";
                reason = replayOutcomeMatch
                    ? "hash-verified locator candidate compiled and replayed with the same outcome"
                    : "compiled replay outcome differed from the source runtime";
            }
        }
        catch (Exception exception)
        {
            errorCount++;
            reason = exception.GetBaseException().Message;
            status = "Error";
        }
        finally
        {
            DisposeRunImages(compiledRun);
            DisposeRunImages(run);
        }

        string packetSha = !string.IsNullOrWhiteSpace(packetPath) && File.Exists(packetPath)
            ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(packetPath)
            : string.Empty;
        string nativeOverlaySha = File.Exists(copiedNativeOverlayPath)
            ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(copiedNativeOverlayPath)
            : string.Empty;
        string sourceShaForCsv = File.Exists(copiedSourcePath)
            ? OpenVisionRecipeLocatorRelativeBlobEvidencePacket.ComputeSha256(copiedSourcePath)
            : expectedSourceSha;
        intakeRows.Add(string.Join(
            ",",
            Csv(rowId),
            Csv(split),
            Csv(roleLabelOnly),
            Csv(sourceShaForCsv),
            Csv(nativeOverlaySha),
            Csv(packetSha),
            candidateCount.ToString(CultureInfo.InvariantCulture),
            FormatDouble(locatorScore),
            FormatDouble(locatorScoreMargin),
            Csv(locatorScoreMarginState),
            Csv(blobResultCount),
            sourceRunSuccess.ToString(),
            compiledReplaySuccess.ToString(),
            replayOutcomeMatch.ToString(),
            Csv(status),
            Csv(reason),
            Csv(sourcePath),
            Csv(packetPath),
            Csv(locatorOverlayPath),
            Csv(blobOverlayPath)));
        freezeRows.Add(string.Join(
            "\t",
            rowId,
            split,
            roleLabelOnly,
            expectedSourceSha,
            packetSha,
            status,
            reason));
    }

    File.WriteAllLines(Path.Combine(evidenceRoot, "intake.csv"), intakeRows, Encoding.UTF8);
    File.WriteAllLines(Path.Combine(evidenceRoot, "corpus-freeze-manifest.tsv"), freezeRows, Encoding.UTF8);

    var resultSummary = new
    {
        scenario = "locator-relative-blob-corpus-pilot",
        pilot_only = true,
        qualification = false,
        skill_id = OpenVisionRecipeLocatorRelativeBlobIntentSkill.SkillId,
        source_dataset = sourceDataset,
        native_batch_evidence = batchRoot,
        rows = expectedRows,
        packets = packetCount,
        replayed_packets = replayCount,
        replay_outcome_matches = replayMatchCount,
        locator_rejected = locatorRejectedCount,
        second_candidate_observed = observedSecondCandidateCount,
        missing_second_candidate = missingSecondCandidateCount,
        candidate_evidence_unavailable = unavailableCandidateCount,
        errors = errorCount,
        all_packets_replayed_with_same_outcome = packetCount == expectedRows
            && replayCount == expectedRows
            && replayMatchCount == expectedRows
            && locatorRejectedCount == 0
            && errorCount == 0,
        template_sha256 = expectedTemplateHash,
        plan = new
        {
            search_roi = searchRoiText,
            inspection_roi = inspectionRoiText,
            reference_pose = referencePoseText,
            score_minimum = 0.8D,
            score_margin_minimum = 10D,
            angle_minimum = angleMinimum,
            angle_maximum = angleMaximum,
            scale_minimum = scaleMinimum,
            scale_maximum = scaleMaximum,
            threshold = OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultThreshold,
            minimum_area = OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultMinimumArea,
            maximum_area = OpenVisionRecipeLocatorRelativeBlobIntentSkill.DefaultMaximumArea,
            expected_count = (int?)null
        },
        boundary = "ScoreMargin is exported only when two retained locator candidates are present; RoleLabelOnly is corpus metadata, not defect truth; this is locator evidence and deterministic replay only. Qualification still requires reviewed Train/Validation/Held-out splits, deduplication, and approved visual evidence."
    };
    File.WriteAllText(
        Path.Combine(evidenceRoot, "summary.json"),
        JsonSerializer.Serialize(resultSummary, new JsonSerializerOptions { WriteIndented = true }),
        Encoding.UTF8);
    string report = string.Join(
        Environment.NewLine,
        new[]
        {
            "Result: " + (resultSummary.all_packets_replayed_with_same_outcome ? "PASS" : "PARTIAL"),
            "Scenario: locator-relative-blob-corpus-pilot",
            "Rows: " + expectedRows.ToString(CultureInfo.InvariantCulture),
            "Packets: " + packetCount.ToString(CultureInfo.InvariantCulture),
            "Compiled replay outcome matches: " + replayMatchCount.ToString(CultureInfo.InvariantCulture) + "/" + replayCount.ToString(CultureInfo.InvariantCulture),
            "Locator rejected: " + locatorRejectedCount.ToString(CultureInfo.InvariantCulture),
            "Second candidate observed: " + observedSecondCandidateCount.ToString(CultureInfo.InvariantCulture),
            "Missing second candidate: " + missingSecondCandidateCount.ToString(CultureInfo.InvariantCulture),
            "Candidate evidence unavailable: " + unavailableCandidateCount.ToString(CultureInfo.InvariantCulture),
            "Errors: " + errorCount.ToString(CultureInfo.InvariantCulture),
            "TemplateSha256: " + expectedTemplateHash,
            "Plan: search " + searchRoiText + " / inspection " + inspectionRoiText + " / reference " + referencePoseText,
            "PilotOnly: true",
            "Qualification: false",
            "Boundary: ScoreMargin is exported only when two retained locator candidates are present; RoleLabelOnly is corpus metadata, not defect truth; qualification requires reviewed frozen splits and held-out evidence."
        });
    File.WriteAllText(Path.Combine(evidenceRoot, "report.txt"), report, Encoding.UTF8);

    Console.WriteLine("LocatorRelativeBlobCorpusPilot: " + (resultSummary.all_packets_replayed_with_same_outcome ? "PASS" : "PARTIAL"));
    Console.WriteLine("EvidenceRoot: " + evidenceRoot);
    Console.WriteLine("Summary: " + Path.Combine(evidenceRoot, "summary.json"));
    Console.WriteLine("Intake: " + Path.Combine(evidenceRoot, "intake.csv"));
    return resultSummary.all_packets_replayed_with_same_outcome ? 0 : 1;
}

static List<Dictionary<string, string>> ReadCsv(string path)
{
    List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
    using (Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(path, Encoding.UTF8))
    {
        parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        string[] headers = parser.ReadFields() ?? Array.Empty<string>();
        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields() ?? Array.Empty<string>();
            Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < headers.Length; index++)
            {
                row[headers[index]] = index < fields.Length ? fields[index] : string.Empty;
            }

            rows.Add(row);
        }
    }

    return rows;
}

static string RequireCsv(IReadOnlyDictionary<string, string> row, string key)
{
    if (row == null || !row.TryGetValue(key, out string value) || string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException("Native batch CSV is missing a required value: " + key);
    }

    return value.Trim();
}

static string Csv(string value)
{
    value ??= string.Empty;
    return value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0
        ? value
        : "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
}

static string SafeLeaf(string value)
{
    string leaf = string.IsNullOrWhiteSpace(value) ? "row" : value.Trim();
    foreach (char invalid in Path.GetInvalidFileNameChars())
    {
        leaf = leaf.Replace(invalid, '_');
    }

    return leaf;
}

static string FormatDouble(double value)
{
    return double.IsNaN(value) || double.IsInfinity(value)
        ? string.Empty
        : value.ToString("0.###", CultureInfo.InvariantCulture);
}

static string RequireJsonString(JsonElement element, string propertyName, string expected = null)
{
    if (!element.TryGetProperty(propertyName, out JsonElement value)
        || value.ValueKind != JsonValueKind.String)
    {
        throw new InvalidOperationException("JSON summary is missing a string property: " + propertyName);
    }

    string text = value.GetString() ?? string.Empty;
    if (expected != null && !string.Equals(text, expected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            "JSON summary property " + propertyName + " changed. Expected=" + expected + ", Actual=" + text);
    }

    return text;
}

static int RequireJsonInt(JsonElement element, string propertyName)
{
    if (!element.TryGetProperty(propertyName, out JsonElement value)
        || value.ValueKind != JsonValueKind.Number
        || !value.TryGetInt32(out int actual)
        || actual < 0)
    {
        throw new InvalidOperationException(
            "JSON summary is missing a non-negative integer property: " + propertyName);
    }

    return actual;
}

static int RequireExpectedJsonInt(JsonElement element, string propertyName, int expected)
{
    int actual = RequireJsonInt(element, propertyName);
    if (actual != expected)
    {
        throw new InvalidOperationException(
            "JSON summary property " + propertyName + " changed. Expected="
            + expected.ToString(CultureInfo.InvariantCulture)
            + ", Actual="
            + actual.ToString(CultureInfo.InvariantCulture));
    }

    return actual;
}

static double RequireJsonDouble(JsonElement element, string propertyName)
{
    if (!element.TryGetProperty(propertyName, out JsonElement value)
        || value.ValueKind != JsonValueKind.Number
        || !value.TryGetDouble(out double actual)
        || double.IsNaN(actual)
        || double.IsInfinity(actual))
    {
        throw new InvalidOperationException("JSON summary is missing a finite number: " + propertyName);
    }

    return actual;
}

static bool RequireJsonBool(JsonElement element, string propertyName)
{
    if (!element.TryGetProperty(propertyName, out JsonElement value)
        || value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False)
    {
        throw new InvalidOperationException("JSON summary is missing a boolean property: " + propertyName);
    }

    return value.GetBoolean();
}

static bool SaveStepOverlay(
    Mat sourceImage,
    VisionPipelineStepResult stepResult,
    int stepIndex,
    OpenVisionRecipePinGapIntentSkill.RoiSample inspectionRoi,
    string outputPath,
    out string message)
{
    message = string.Empty;
    try
    {
        using Bitmap source = BitmapImageConverter.ToBitmap(sourceImage);
        using Bitmap rendered = VisionPipelineRunReportImageRenderer.Render(source, stepResult, stepIndex);
        if (rendered == null)
        {
            message = "The step overlay renderer returned no image.";
            return false;
        }

        using (Graphics graphics = Graphics.FromImage(rendered))
        using (Pen roiPen = new Pen(Color.Gold, 2F))
        {
            graphics.DrawRectangle(roiPen, inspectionRoi.X, inspectionRoi.Y, inspectionRoi.Width, inspectionRoi.Height);
        }
        rendered.Save(outputPath, ImageFormat.Png);
        return File.Exists(outputPath) && new FileInfo(outputPath).Length > 0;
    }
    catch (Exception exception)
    {
        message = exception.GetBaseException().Message;
        return false;
    }
}

static bool SaveCandidateReviewArtifacts(
    Mat sourceImage,
    Mat templateImage,
    VisionPipelineMatchResultEvidence match,
    string outputDirectory,
    out string patchPath,
    out string comparisonPath,
    out string blendPath,
    out string message)
{
    patchPath = Path.Combine(outputDirectory, "candidate-patch.png");
    comparisonPath = Path.Combine(outputDirectory, "template-vs-candidate.png");
    blendPath = Path.Combine(outputDirectory, "template-candidate-blend.png");
    message = string.Empty;
    try
    {
        using Mat patch = ExtractPoseNormalizedCandidatePatch(sourceImage, templateImage, match);
        if (patch.Empty())
        {
            message = "The reported candidate bounds could not produce a pose-normalized source patch.";
            return false;
        }

        if (!Cv2.ImWrite(patchPath, patch))
        {
            message = "The pose-normalized source patch could not be written.";
            return false;
        }

        int reviewHeight = 256;
        int templateReviewWidth = Math.Max(1, (int)Math.Round(templateImage.Width * reviewHeight / (double)templateImage.Height));
        int patchReviewWidth = Math.Max(1, (int)Math.Round(patch.Width * reviewHeight / (double)patch.Height));
        using Mat templatePanel = new Mat();
        using Mat patchPanel = new Mat();
        Cv2.Resize(templateImage, templatePanel, new OpenCvSharp.Size(templateReviewWidth, reviewHeight), 0D, 0D, InterpolationFlags.Nearest);
        Cv2.Resize(patch, patchPanel, new OpenCvSharp.Size(patchReviewWidth, reviewHeight), 0D, 0D, InterpolationFlags.Nearest);
        Cv2.PutText(
            templatePanel,
            "TEMPLATE",
            new OpenCvSharp.Point(8, 24),
            HersheyFonts.HersheySimplex,
            0.6D,
            Scalar.Lime,
            2);
        Cv2.PutText(
            patchPanel,
            "CANDIDATE " + match.NativeIndex.ToString(CultureInfo.InvariantCulture),
            new OpenCvSharp.Point(8, 24),
            HersheyFonts.HersheySimplex,
            0.6D,
            Scalar.Lime,
            2);
        using Mat comparison = new Mat();
        Cv2.HConcat(new[] { templatePanel, patchPanel }, comparison);
        if (!Cv2.ImWrite(comparisonPath, comparison))
        {
            message = "The template-versus-candidate comparison could not be written.";
            return false;
        }

        using Mat blend = new Mat();
        Cv2.AddWeighted(templateImage, 0.5D, patch, 0.5D, 0D, blend);
        if (!Cv2.ImWrite(blendPath, blend))
        {
            message = "The template-candidate blend could not be written.";
            return false;
        }

        message = "Pose-normalized patch, side-by-side comparison, and blend were written; visual review remains NOT_REVIEWED.";
        return true;
    }
    catch (Exception exception)
    {
        message = exception.GetBaseException().Message;
        return false;
    }
}

static Mat ExtractPoseNormalizedCandidatePatch(
    Mat sourceImage,
    Mat templateImage,
    VisionPipelineMatchResultEvidence match)
{
    int x = (int)Math.Round(match.BoundsX, MidpointRounding.AwayFromZero);
    int y = (int)Math.Round(match.BoundsY, MidpointRounding.AwayFromZero);
    int width = Math.Max(1, (int)Math.Round(match.BoundsWidth, MidpointRounding.AwayFromZero));
    int height = Math.Max(1, (int)Math.Round(match.BoundsHeight, MidpointRounding.AwayFromZero));
    if (x < 0 || y < 0 || x + width > sourceImage.Width || y + height > sourceImage.Height)
    {
        return new Mat();
    }

    using Mat extracted = TemplateImageExtraction.Extract(
        sourceImage,
        new Rect(x, y, width, height),
        match.Angle);
    if (extracted.Empty())
    {
        return new Mat();
    }

    if (extracted.Width == templateImage.Width && extracted.Height == templateImage.Height)
    {
        return extracted.Clone();
    }

    Mat normalized = new Mat();
    Cv2.Resize(
        extracted,
        normalized,
        new OpenCvSharp.Size(templateImage.Width, templateImage.Height),
        0D,
        0D,
        InterpolationFlags.Linear);
    return normalized;
}

static OpenVisionRecipeLocatorRelativeBlobEvidencePacket Clone(OpenVisionRecipeLocatorRelativeBlobEvidencePacket source)
{
    return new OpenVisionRecipeLocatorRelativeBlobEvidencePacket
    {
        SchemaVersion = source.SchemaVersion,
        PlanSchemaVersion = source.PlanSchemaVersion,
        SkillId = source.SkillId,
        SkillVersion = source.SkillVersion,
        SourceImagePath = source.SourceImagePath,
        SourceImageSha256 = source.SourceImageSha256,
        LocatorTemplatePath = source.LocatorTemplatePath,
        LocatorTemplateSha256 = source.LocatorTemplateSha256,
        PreviewOverlayPath = source.PreviewOverlayPath,
        PreviewOverlaySha256 = source.PreviewOverlaySha256,
        SourceImageWidth = source.SourceImageWidth,
        SourceImageHeight = source.SourceImageHeight,
        CoordinateFrame = source.CoordinateFrame,
        Producer = source.Producer,
        ProducerVersion = source.ProducerVersion,
        SelectedCandidateId = source.SelectedCandidateId,
        CreatedUtc = source.CreatedUtc,
        Candidates = source.Candidates.Select(candidate => new OpenVisionRecipeLocatorRelativeBlobEvidenceCandidate
        {
            CandidateId = candidate.CandidateId,
            NativeIndex = candidate.NativeIndex,
            Accepted = candidate.Accepted,
            CenterX = candidate.CenterX,
            CenterY = candidate.CenterY,
            Angle = candidate.Angle,
            Scale = candidate.Scale,
            BoundsX = candidate.BoundsX,
            BoundsY = candidate.BoundsY,
            BoundsWidth = candidate.BoundsWidth,
            BoundsHeight = candidate.BoundsHeight,
            Score = candidate.Score,
            ScoreMargin = candidate.ScoreMargin,
            CoordinateFrame = candidate.CoordinateFrame,
            OverlayPath = candidate.OverlayPath,
            OverlaySha256 = candidate.OverlaySha256,
            Reason = candidate.Reason
        }).ToList()
    };
}

static OpenVisionRecipeLocatorRelativeBlobReviewDecision CloneReviewDecision(
    OpenVisionRecipeLocatorRelativeBlobReviewDecision source)
{
    return new OpenVisionRecipeLocatorRelativeBlobReviewDecision
    {
        SchemaVersion = source.SchemaVersion,
        Decision = source.Decision,
        VisualCorrespondence = source.VisualCorrespondence,
        EvidencePacketPath = source.EvidencePacketPath,
        EvidencePacketSha256 = source.EvidencePacketSha256,
        SourceImageSha256 = source.SourceImageSha256,
        LocatorTemplateSha256 = source.LocatorTemplateSha256,
        PreviewOverlaySha256 = source.PreviewOverlaySha256,
        ReviewedCandidateId = source.ReviewedCandidateId,
        PlanFingerprint = source.PlanFingerprint,
        InspectionRoi = source.InspectionRoi,
        Threshold = source.Threshold,
        MinimumArea = source.MinimumArea,
        MaximumArea = source.MaximumArea,
        ExpectedResultCount = source.ExpectedResultCount,
        Reviewer = source.Reviewer,
        ReviewedUtc = source.ReviewedUtc,
        Notes = source.Notes
    };
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException("ASSERT FAILED: " + message);
    }
}
