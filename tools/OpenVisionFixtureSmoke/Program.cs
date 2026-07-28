using Lib.Common;
using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Tool;
using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bitmap = System.Drawing.Bitmap;

internal static class Program
{
    private const int ShiftX = 70;
    private const int ShiftY = 40;
    private const string SavedRoi = "170,80,50,50";

    private static async Task<int> Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "--cvr09-line-fixture", StringComparison.OrdinalIgnoreCase))
        {
            Assert(args.Length == 2, "Usage: --cvr09-line-fixture <new-output-directory>");
            return await RunCvr09LineFixture(
                Path.GetFullPath(args[1]));
        }

        if (args.Length > 0 && string.Equals(args[0], "--c9-gap-corpus", StringComparison.OrdinalIgnoreCase))
        {
            Assert(args.Length == 4, "Usage: --c9-gap-corpus <p175-evidence-root> <device-top-left-images-root> <new-output-directory>");
            return await RunC9FullCorpusGapBatch(
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]),
                Path.GetFullPath(args[3]));
        }

        if (args.Length > 0 && string.Equals(args[0], "--c9-gap", StringComparison.OrdinalIgnoreCase))
        {
            Assert(args.Length == 3, "Usage: --c9-gap <p175-evidence-root> <new-output-directory>");
            return await RunC9NormalizedGapBatch(
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]));
        }

        if (args.Length > 0 && string.Equals(args[0], "--c9-candidate-audit", StringComparison.OrdinalIgnoreCase))
        {
            Assert(args.Length == 3, "Usage: --c9-candidate-audit <p175-evidence-root> <new-output-directory>");
            return await RunC9CandidateAudit(
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]));
        }

        if (args.Length > 0 && string.Equals(args[0], "--c9-gate", StringComparison.OrdinalIgnoreCase))
        {
            Assert(args.Length == 3, "Usage: --c9-gate <p175-evidence-root> <new-output-directory>");
            return await RunC9FailClosedGate(
                Path.GetFullPath(args[1]),
                Path.GetFullPath(args[2]));
        }

        string outputDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "artifacts",
            "p181_matching_similarity_normalize_image_20260721",
            "runtime");
        Directory.CreateDirectory(outputDirectory);

        string templatePath = Path.Combine(outputDirectory, "fixture_template.png");
        using Mat referenceImage = CreateImage(0, 0);
        using Mat shiftedImage = CreateImage(ShiftX, ShiftY);
        using (Mat template = new Mat(referenceImage, new Rect(40, 50, 40, 40)).Clone())
        {
            Cv2.ImWrite(templatePath, template);
        }

        Cv2.ImWrite(Path.Combine(outputDirectory, "reference.png"), referenceImage);
        Cv2.ImWrite(Path.Combine(outputDirectory, "shifted.png"), shiftedImage);

        VisionPipeline referencePipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipeline shiftedPipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipeline unfixturedPipeline = CreatePipeline(templatePath, useFixture: false);
        VisionPipeline missingFramePipeline = CreatePipeline(templatePath, useFixture: true);
        missingFramePipeline.Steps[1].Parameters["FIXTURE_FRAME_NAME"] = "MissingFrame";
        VisionPipeline multiRoiPipeline = CreatePipeline(templatePath, useFixture: true);
        multiRoiPipeline.Steps[1].Parameters["USE_MULTI_ROI"] = "true";
        VisionPipeline duplicateFramePipeline = CreateDuplicateFramePipeline(templatePath);
        string fixturePipelinePath = Path.Combine(outputDirectory, "fixture_translation.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(fixturePipelinePath, shiftedPipeline), "Fixture pipeline XML save must succeed.");
        VisionRecipeRunner runner = new VisionRecipeRunner();

        using VisionRecipeRunResult referenceResult = await runner.RunAsync(referencePipeline, referenceImage);
        using VisionRecipeRunResult shiftedResult = await runner.RunAsync(fixturePipelinePath, shiftedImage);
        using VisionRecipeRunResult unfixturedResult = await runner.RunAsync(unfixturedPipeline, shiftedImage);
        using VisionRecipeRunResult missingFrameResult = await runner.RunAsync(missingFramePipeline, referenceImage);
        using VisionRecipeRunResult multiRoiResult = await runner.RunAsync(multiRoiPipeline, referenceImage);
        using VisionRecipeRunResult duplicateFrameResult = await runner.RunAsync(duplicateFramePipeline, referenceImage);

        string publicPipelinePath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Public_Matching_FixturePad.pipeline.xml"));
        string publicGoodPath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Fixture_Pad_Synthetic_Shifted_OK.png"));
        string publicBadPath = Path.GetFullPath(Path.Combine(
            "docs",
            "samples",
            "public",
            "Fixture_Pad_Synthetic_Shifted_Missing_NG.png"));
        Assert(File.Exists(publicPipelinePath), "Public Fixture pipeline must exist.");
        Assert(File.Exists(publicGoodPath), "Public Fixture Good image must exist.");
        Assert(File.Exists(publicBadPath), "Public Fixture Bad image must exist.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(publicPipelinePath, out VisionPipeline publicNoFixturePipeline)
                && publicNoFixturePipeline != null,
            "Public Fixture pipeline XML load must succeed.");
        publicNoFixturePipeline.Steps[0].Parameters["USE_AS_FIXTURE_FRAME"] = "false";
        publicNoFixturePipeline.Steps[1].Parameters["USE_FIXTURE_FRAME"] = "false";
        using Mat publicGoodImage = Cv2.ImRead(publicGoodPath, ImreadModes.Unchanged);
        using Mat publicBadImage = Cv2.ImRead(publicBadPath, ImreadModes.Unchanged);
        using VisionRecipeRunResult publicGoodResult = await runner.RunAsync(publicPipelinePath, publicGoodImage);
        using VisionRecipeRunResult publicBadResult = await runner.RunAsync(publicPipelinePath, publicBadImage);
        using VisionRecipeRunResult publicNoFixtureResult = await runner.RunAsync(publicNoFixturePipeline, publicGoodImage);

        Assert(referenceResult.Success, "Reference fixture run must pass.");
        Assert(shiftedResult.Success, "Shifted fixture run must pass.");
        Assert(!unfixturedResult.Success, "Shifted run without fixture must fail the downstream ROI inspection.");
        AssertFixtureConfigurationFailure(missingFrameResult, "Missing fixture frame must fail closed.");
        AssertFixtureConfigurationFailure(multiRoiResult, "Fixture multi-ROI must fail closed.");
        AssertFixtureConfigurationFailure(duplicateFrameResult, "Duplicate fixture frame must fail closed.");
        Assert(publicGoodResult.Success, "Public shifted Fixture Good sample must pass.");
        Assert(!publicBadResult.Success, "Public shifted Fixture Bad sample must fail at the pad inspection.");
        Assert(
            string.Equals(publicBadResult.FirstFailedErrorName, "BlobNoResult", StringComparison.Ordinal),
            "Public Fixture Bad sample must keep Matching successful and fail at BlobNoResult.");
        Assert(!publicNoFixtureResult.Success, "Public shifted Good sample without Fixture correction must fail.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(fixturePipelinePath, out VisionPipeline persistedPipeline)
                && persistedPipeline != null,
            "Fixture pipeline XML reload must succeed.");
        Assert(
            string.Equals(persistedPipeline.Steps[1].Parameters["CvROI"], SavedRoi, StringComparison.Ordinal),
            "Runtime fixture application must not rewrite the saved CvROI.");

        VisionRecipeStepRunSummary fixtureProducer = shiftedResult.Steps[0];
        VisionRecipeStepRunSummary fixtureConsumer = shiftedResult.Steps[1];
        AssertMetric(fixtureProducer, "FixtureOffsetX", ShiftX, 1.1);
        AssertMetric(fixtureProducer, "FixtureOffsetY", ShiftY, 1.1);
        AssertMetric(fixtureConsumer, "FixtureEffectiveRoiX", 170 + ShiftX, 0.1);
        AssertMetric(fixtureConsumer, "FixtureEffectiveRoiY", 80 + ShiftY, 0.1);
        AssertMetric(publicGoodResult.Steps[0], "FixtureOffsetX", 80, 0.1);
        AssertMetric(publicGoodResult.Steps[0], "FixtureOffsetY", 55, 0.1);
        AssertMetric(publicGoodResult.Steps[1], "FixtureEffectiveRoiX", 400, 0.1);
        AssertMetric(publicGoodResult.Steps[1], "FixtureEffectiveRoiY", 235, 0.1);
        Assert(
            string.Equals(publicNoFixturePipeline.Steps[1].Parameters["CvROI"], "320,180,60,50", StringComparison.Ordinal),
            "Public Fixture execution must not rewrite the saved CvROI.");

        IReadOnlyList<string> similarityReport = await RunSimilarityNormalizationSmoke(outputDirectory, runner);

        if (shiftedResult.ResultImage != null && !shiftedResult.ResultImage.Empty())
        {
            Cv2.ImWrite(Path.Combine(outputDirectory, "shifted_fixture_result.png"), shiftedResult.ResultImage);
        }

        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: Matching fixture translation",
            "ReferenceRun: " + referenceResult.SummaryText,
            "ShiftedFixtureRun: " + shiftedResult.SummaryText,
            "ShiftedWithoutFixtureRun: " + unfixturedResult.SummaryText,
            "MissingFrameRun: " + missingFrameResult.SummaryText,
            "MultiRoiRun: " + multiRoiResult.SummaryText,
            "DuplicateFrameRun: " + duplicateFrameResult.SummaryText,
            "PublicGoodRun: " + publicGoodResult.SummaryText,
            "PublicBadRun: " + publicBadResult.SummaryText,
            "PublicGoodWithoutFixtureRun: " + publicNoFixtureResult.SummaryText,
            "SavedCvROI: " + persistedPipeline.Steps[1].Parameters["CvROI"],
            "PublicSavedCvROI: " + publicNoFixturePipeline.Steps[1].Parameters["CvROI"],
            "FixtureOffsetX: " + GetMetric(fixtureProducer, "FixtureOffsetX").ToString("0.###", CultureInfo.InvariantCulture),
            "FixtureOffsetY: " + GetMetric(fixtureProducer, "FixtureOffsetY").ToString("0.###", CultureInfo.InvariantCulture),
            "EffectiveRoiX: " + GetMetric(fixtureConsumer, "FixtureEffectiveRoiX").ToString("0.###", CultureInfo.InvariantCulture),
            "EffectiveRoiY: " + GetMetric(fixtureConsumer, "FixtureEffectiveRoiY").ToString("0.###", CultureInfo.InvariantCulture)
        }.Concat(similarityReport));
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report);
        Console.WriteLine(report);
        return 0;
    }

    private static async Task<int> RunC9CandidateAudit(string evidenceRoot, string outputDirectory)
    {
        Assert(Directory.Exists(evidenceRoot), "P175 evidence root must exist: " + evidenceRoot);
        Assert(
            !Directory.Exists(outputDirectory) || !Directory.EnumerateFileSystemEntries(outputDirectory).Any(),
            "P183 candidate-audit output must be new or empty: " + outputDirectory);

        string rowsPath = Path.Combine(evidenceRoot, "native_rows.csv");
        string sourceTemplatePath = Path.Combine(evidenceRoot, "reference", "locator_template.png");
        Assert(File.Exists(rowsPath), "P175 native_rows.csv is required.");
        Assert(File.Exists(sourceTemplatePath), "P175 C9 template is required.");
        List<Dictionary<string, string>> sourceRows = ReadCsv(rowsPath);
        Assert(sourceRows.Count == 24, "P183 candidate audit requires the exact P175 24 rows.");

        Directory.CreateDirectory(outputDirectory);
        string casesDirectory = Path.Combine(outputDirectory, "cases");
        Directory.CreateDirectory(casesDirectory);
        string templatePath = Path.Combine(outputDirectory, "locator_template.png");
        File.Copy(sourceTemplatePath, templatePath, true);

        VisionPipeline sourcePipeline = CreateC9NormalizedGapPipeline(templatePath, includeMeasurement: false);
        VisionPipeline pipeline = new VisionPipeline { Name = "C9_Two_Candidate_Audit" };
        VisionPipelineStep matching = sourcePipeline.Steps[0];
        matching.Name = "01 Audit C9 Candidates";
        matching.OutputLayer = "CandidateMatches";
        matching.Parameters["USE_AS_FIXTURE_FRAME"] = "false";
        matching.Parameters["NUM_MATCH"] = "2";
        matching.Parameters["SCORE_MIN"] = "0";
        pipeline.Steps.Add(matching);
        Assert(VisionPipelineValidator.Validate(pipeline, new[] { "Main" }).Success, "P183 candidate-audit pipeline must validate.");
        string pipelinePath = Path.Combine(outputDirectory, "c9_two_candidate_audit.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(pipelinePath, pipeline), "P183 candidate-audit XML save must pass.");

        List<C9CandidateAuditRow> rows = new List<C9CandidateAuditRow>();
        VisionRecipeRunner runner = new VisionRecipeRunner();
        foreach (Dictionary<string, string> sourceRow in sourceRows)
        {
            string sourcePath = Path.GetFullPath(Path.Combine(
                evidenceRoot,
                sourceRow["SourceCopyPath"].Replace('/', Path.DirectorySeparatorChar)));
            await AddC9CandidateAuditRow(
                runner,
                pipeline,
                sourcePath,
                sourceRow["RowId"],
                "observed",
                outputDirectory,
                casesDirectory,
                rows);
        }

        using Mat template = Cv2.ImRead(templatePath, ImreadModes.Unchanged);
        Assert(!template.Empty(), "P183 C9 template image must load.");
        using Mat noTarget = new Mat(new Size(640, 480), template.Type(), Scalar.All(255));
        string noTargetPath = Path.Combine(casesDirectory, "no_target.png");
        Cv2.ImWrite(noTargetPath, noTarget);
        await AddC9CandidateAuditRow(runner, pipeline, noTargetPath, "synthetic_no_target", "no-target", outputDirectory, casesDirectory, rows);

        using Mat ambiguous = noTarget.Clone();
        using (Mat firstTarget = new Mat(ambiguous, new Rect(100, 100, template.Width, template.Height)))
        using (Mat secondTarget = new Mat(ambiguous, new Rect(430, 300, template.Width, template.Height)))
        {
            template.CopyTo(firstTarget);
            template.CopyTo(secondTarget);
        }
        string ambiguousPath = Path.Combine(casesDirectory, "ambiguous_two_exact_targets.png");
        Cv2.ImWrite(ambiguousPath, ambiguous);
        await AddC9CandidateAuditRow(runner, pipeline, ambiguousPath, "synthetic_ambiguous_two_exact", "ambiguous", outputDirectory, casesDirectory, rows);

        using StreamWriter writer = new StreamWriter(Path.Combine(outputDirectory, "rows.csv"), false, new UTF8Encoding(false));
        writer.WriteLine("RowId,Kind,Success,ResultCount,ScoreMax,ScoreSecond,ScoreMargin,SourceSha256,SourcePath,OverlayPath");
        foreach (C9CandidateAuditRow row in rows)
        {
            writer.WriteLine(string.Join(",", new[]
            {
                Csv(row.RowId), Csv(row.Kind), row.Success.ToString(), Number(row.ResultCount), Number(row.ScoreMax),
                Number(row.ScoreSecond), Number(row.ScoreMargin), Csv(row.SourceSha256), Csv(row.SourcePath), Csv(row.OverlayPath)
            }));
        }

        List<C9CandidateAuditRow> observed = rows.Where(row => string.Equals(row.Kind, "observed", StringComparison.Ordinal)).ToList();
        C9CandidateAuditRow noTargetRow = rows.Single(row => string.Equals(row.Kind, "no-target", StringComparison.Ordinal));
        C9CandidateAuditRow ambiguousRow = rows.Single(row => string.Equals(row.Kind, "ambiguous", StringComparison.Ordinal));
        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: C9 two-candidate score audit before operating-policy selection",
            "ObservedRows: " + observed.Count.ToString(CultureInfo.InvariantCulture),
            "ObservedScoreMarginMinMax: " + observed.Min(row => row.ScoreMargin).ToString("0.###", CultureInfo.InvariantCulture) + " / " + observed.Max(row => row.ScoreMargin).ToString("0.###", CultureInfo.InvariantCulture),
            "NoTarget: success=" + noTargetRow.Success + ", count=" + Number(noTargetRow.ResultCount) + ", best=" + Number(noTargetRow.ScoreMax),
            "Ambiguous: success=" + ambiguousRow.Success + ", count=" + Number(ambiguousRow.ResultCount) + ", best=" + Number(ambiguousRow.ScoreMax) + ", second=" + Number(ambiguousRow.ScoreSecond) + ", margin=" + Number(ambiguousRow.ScoreMargin),
            "PipelineSha256: " + ComputeSha256(pipelinePath),
            "Boundary: diagnostic evidence only; no operating margin is selected by this command."
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, new UTF8Encoding(false));
        Console.WriteLine(report);
        return 0;
    }

    private static async Task AddC9CandidateAuditRow(
        VisionRecipeRunner runner,
        VisionPipeline pipeline,
        string sourcePath,
        string rowId,
        string kind,
        string outputDirectory,
        string casesDirectory,
        ICollection<C9CandidateAuditRow> rows)
    {
        Assert(File.Exists(sourcePath), "P183 candidate-audit source is missing: " + sourcePath);
        using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Unchanged);
        using VisionRecipeRunResult result = await runner.RunAsync(pipeline, source);
        VisionRecipeStepRunSummary summary = result.Steps.FirstOrDefault();
        double count = GetOptionalMetric(summary, "ResultCount");
        double best = GetOptionalMetric(summary, "ScoreMax");
        double second = !double.IsNaN(count) && count >= 2 ? GetOptionalMetric(summary, "ScoreMin") : 0d;
        double margin = GetOptionalMetric(summary, VisionPipelineKnownMetrics.ScoreMargin);
        Assert(
            double.IsNaN(best) || Math.Abs(margin - (best - second)) <= 0.000001d,
            "P183 ScoreMargin enrichment disagrees with the two-candidate result: " + rowId);
        string savedSourcePath = Path.Combine(casesDirectory, rowId + Path.GetExtension(sourcePath));
        if (!string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(savedSourcePath), StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(sourcePath, savedSourcePath, true);
        }
        string overlayPath = RenderEvidence(
            savedSourcePath,
            summary,
            pipeline.Steps[0],
            Path.Combine(casesDirectory, rowId + "_overlay.png"));
        rows.Add(new C9CandidateAuditRow
        {
            RowId = rowId,
            Kind = kind,
            Success = result.Success,
            ResultCount = count,
            ScoreMax = best,
            ScoreSecond = second,
            ScoreMargin = margin,
            SourceSha256 = ComputeSha256(savedSourcePath),
            SourcePath = RelativeTo(outputDirectory, savedSourcePath),
            OverlayPath = RelativeTo(outputDirectory, overlayPath)
        });
    }

    private static async Task<int> RunC9FailClosedGate(string evidenceRoot, string outputDirectory)
    {
        Assert(Directory.Exists(evidenceRoot), "P175 evidence root must exist: " + evidenceRoot);
        Assert(
            !Directory.Exists(outputDirectory) || !Directory.EnumerateFileSystemEntries(outputDirectory).Any(),
            "P183 gate output must be new or empty: " + outputDirectory);

        string rowsPath = Path.Combine(evidenceRoot, "native_rows.csv");
        string sourceTemplatePath = Path.Combine(evidenceRoot, "reference", "locator_template.png");
        string sourceReferencePath = Path.Combine(evidenceRoot, "reference", "operator_reference_ok0001.jpg");
        Assert(File.Exists(rowsPath), "P175 native_rows.csv is required.");
        Assert(File.Exists(sourceTemplatePath), "P175 C9 template is required.");
        Assert(File.Exists(sourceReferencePath), "P175 reviewed reference image is required.");
        List<Dictionary<string, string>> sourceRows = ReadCsv(rowsPath);
        Assert(sourceRows.Count == 24, "P183 gate requires the exact P175 24 rows.");

        string referenceDirectory = Path.Combine(outputDirectory, "reference");
        string casesDirectory = Path.Combine(outputDirectory, "cases");
        Directory.CreateDirectory(referenceDirectory);
        Directory.CreateDirectory(casesDirectory);
        string templatePath = Path.Combine(referenceDirectory, "locator_template.png");
        string referencePath = Path.Combine(referenceDirectory, "operator_reference_ok0001.jpg");
        File.Copy(sourceTemplatePath, templatePath, true);
        File.Copy(sourceReferencePath, referencePath, true);
        File.Copy(rowsPath, Path.Combine(referenceDirectory, "p175_native_rows.csv"), true);

        VisionPipeline operatingPipeline = CreateC9FailClosedPipeline(templatePath, 5d);
        VisionPipeline angleExercisePipeline = CreateC9FailClosedPipeline(templatePath, 10d);
        VisionPipeline coverageExercisePipeline = CreateC9FailClosedPipeline(templatePath, 5d);
        coverageExercisePipeline.Name = "C9_Valid_Pixel_Gate_Exercise";
        coverageExercisePipeline.Steps[0].Parameters["FIND_SCALE_MAX"] = "2.1";
        coverageExercisePipeline.Steps[1].Parameters["FIND_SCALE_MAX"] = "2.1";
        coverageExercisePipeline.Steps[1].Parameters["FIXTURE_MAX_SCALE_RATIO"] = "2.1";
        Assert(VisionPipelineValidator.Validate(operatingPipeline, new[] { "Main" }).Success, "P183 operating gate pipeline must validate.");
        Assert(VisionPipelineValidator.Validate(angleExercisePipeline, new[] { "Main" }).Success, "P183 angle exercise pipeline must validate.");
        Assert(VisionPipelineValidator.Validate(coverageExercisePipeline, new[] { "Main" }).Success, "P183 coverage exercise pipeline must validate.");
        string operatingPipelinePath = Path.Combine(outputDirectory, "c9_fail_closed_pre_measurement.pipeline.xml");
        string anglePipelinePath = Path.Combine(outputDirectory, "c9_angle_gate_exercise.pipeline.xml");
        string coveragePipelinePath = Path.Combine(outputDirectory, "c9_valid_pixel_gate_exercise.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(operatingPipelinePath, operatingPipeline), "P183 operating gate XML save must pass.");
        Assert(SerializeHelper.SaveXmlFile(anglePipelinePath, angleExercisePipeline), "P183 angle exercise XML save must pass.");
        Assert(SerializeHelper.SaveXmlFile(coveragePipelinePath, coverageExercisePipeline), "P183 coverage exercise XML save must pass.");
        string operatingPipelineSha = ComputeSha256(operatingPipelinePath);
        string anglePipelineSha = ComputeSha256(anglePipelinePath);
        string coveragePipelineSha = ComputeSha256(coveragePipelinePath);

        VisionRecipeRunner runner = new VisionRecipeRunner();
        List<C9GateEvidenceRow> rows = new List<C9GateEvidenceRow>();
        foreach (Dictionary<string, string> sourceRow in sourceRows)
        {
            string rowId = sourceRow["RowId"];
            string sourcePath = Path.GetFullPath(Path.Combine(
                evidenceRoot,
                sourceRow["SourceCopyPath"].Replace('/', Path.DirectorySeparatorChar)));
            Assert(File.Exists(sourcePath), "P183 normal source is missing: " + sourcePath);
            await AddC9GateEvidenceRow(
                runner,
                operatingPipeline,
                operatingPipelineSha,
                sourcePath,
                rowId,
                "observed-normal",
                true,
                0,
                outputDirectory,
                casesDirectory,
                rows);
        }

        using Mat template = Cv2.ImRead(templatePath, ImreadModes.Unchanged);
        using Mat reference = Cv2.ImRead(referencePath, ImreadModes.Unchanged);
        Assert(!template.Empty() && !reference.Empty(), "P183 reference images must load.");
        using Mat noTarget = new Mat(reference.Size(), reference.Type(), Scalar.All(255));
        string noTargetPath = Path.Combine(casesDirectory, "no_target.png");
        Cv2.ImWrite(noTargetPath, noTarget);
        await AddC9GateEvidenceRow(runner, operatingPipeline, operatingPipelineSha, noTargetPath, "synthetic_no_target", "no-target", false, 1, outputDirectory, casesDirectory, rows);

        using Mat ambiguous = noTarget.Clone();
        using (Mat firstTarget = new Mat(ambiguous, new Rect(100, 100, template.Width, template.Height)))
        using (Mat secondTarget = new Mat(ambiguous, new Rect(430, 300, template.Width, template.Height)))
        {
            template.CopyTo(firstTarget);
            template.CopyTo(secondTarget);
        }
        string ambiguousPath = Path.Combine(casesDirectory, "ambiguous_two_exact_targets.png");
        Cv2.ImWrite(ambiguousPath, ambiguous);
        await AddC9GateEvidenceRow(runner, operatingPipeline, operatingPipelineSha, ambiguousPath, "synthetic_ambiguous_two_exact", "ambiguous", false, 1, outputDirectory, casesDirectory, rows);

        using Mat angleOut = CreateSimilarityCurrent(reference, new SimilarityCase("angle_out", 8d, 1d, 272d, 300d), 272d, 300d);
        string angleOutPath = Path.Combine(casesDirectory, "angle_out_8deg.png");
        Cv2.ImWrite(angleOutPath, angleOut);
        await AddC9GateEvidenceRow(runner, angleExercisePipeline, anglePipelineSha, angleOutPath, "synthetic_angle_out_8deg", "angle-out", false, 2, outputDirectory, casesDirectory, rows);

        using Mat scaleOut = CreateSimilarityCurrent(reference, new SimilarityCase("scale_out", 0d, 1.9d, 272d, 300d), 272d, 300d);
        string scaleOutPath = Path.Combine(casesDirectory, "scale_out_1_9x.png");
        Cv2.ImWrite(scaleOutPath, scaleOut);
        await AddC9GateEvidenceRow(runner, operatingPipeline, operatingPipelineSha, scaleOutPath, "synthetic_scale_out_1_9x", "scale-out", false, 2, outputDirectory, casesDirectory, rows);

        using Mat coverageOut = CreateSimilarityCurrent(reference, new SimilarityCase("coverage_out", 0d, 2.1d, 272d, 300d), 272d, 300d);
        string coverageOutPath = Path.Combine(casesDirectory, "coverage_out_2_1x.png");
        Cv2.ImWrite(coverageOutPath, coverageOut);
        await AddC9GateEvidenceRow(runner, coverageExercisePipeline, coveragePipelineSha, coverageOutPath, "synthetic_coverage_out", "coverage-out", false, 3, outputDirectory, casesDirectory, rows);

        using StreamWriter writer = new StreamWriter(Path.Combine(outputDirectory, "rows.csv"), false, new UTF8Encoding(false));
        writer.WriteLine("RowId,Kind,ExpectedSuccess,ActualSuccess,ExpectedFailedStep,ActualFailedStep,ErrorName,Message,ScoreMax,ScoreMargin,AngleDelta,ScaleRatio,ValidPixelRatio,SourceSha256,PipelineSha256,SourcePath,OverlayPath");
        foreach (C9GateEvidenceRow row in rows)
        {
            writer.WriteLine(string.Join(",", new[]
            {
                Csv(row.RowId), Csv(row.Kind), row.ExpectedSuccess.ToString(), row.ActualSuccess.ToString(),
                row.ExpectedFailedStep.ToString(CultureInfo.InvariantCulture), row.ActualFailedStep.ToString(CultureInfo.InvariantCulture),
                Csv(row.ErrorName), Csv(row.Message), Number(row.ScoreMax), Number(row.ScoreMargin), Number(row.AngleDelta),
                Number(row.ScaleRatio), Number(row.ValidPixelRatio), Csv(row.SourceSha256), Csv(row.PipelineSha256),
                Csv(row.SourcePath), Csv(row.OverlayPath)
            }));
        }

        List<C9GateEvidenceRow> normals = rows.Where(row => string.Equals(row.Kind, "observed-normal", StringComparison.Ordinal)).ToList();
        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: C9 fail-closed pre-measurement gate",
            "NormalRowsPassed: " + normals.Count(row => row.ActualSuccess).ToString(CultureInfo.InvariantCulture) + "/" + normals.Count.ToString(CultureInfo.InvariantCulture),
            "NormalScoreMin: " + normals.Min(row => row.ScoreMax).ToString("0.###", CultureInfo.InvariantCulture),
            "NormalScoreMarginMin: " + normals.Min(row => row.ScoreMargin).ToString("0.###", CultureInfo.InvariantCulture),
            "NoTarget: blocked at Step 1",
            "AmbiguousDuplicate: blocked at Step 1",
            "Angle8Deg: blocked at Step 2 with widened diagnostic search",
            "Scale1.9x: blocked at Step 2",
            "InsufficientValidPixels: blocked at Step 3",
            "OperatingPolicy: ScoreMax >= 80; ScoreMargin >= 10 percentage points; abs angle delta <= 5.25 deg; scale ratio 0.8..1.8; valid pixel ratio >= 0.25",
            "OperatingPipelineSha256: " + operatingPipelineSha,
            "AngleExercisePipelineSha256: " + anglePipelineSha,
            "CoverageExercisePipelineSha256: " + coveragePipelineSha,
            "Boundary: thresholds are bounded to the reviewed C9/P175 24-image evidence and synthetic gate exercises; this is not Gap OK/NG, calibrated measurement, or field qualification."
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, new UTF8Encoding(false));
        Console.WriteLine(report);
        return 0;
    }

    private static async Task AddC9GateEvidenceRow(
        VisionRecipeRunner runner,
        VisionPipeline pipeline,
        string pipelineSha,
        string sourcePath,
        string rowId,
        string kind,
        bool expectedSuccess,
        int expectedFailedStep,
        string outputRoot,
        string casesDirectory,
        ICollection<C9GateEvidenceRow> rows)
    {
        using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Unchanged);
        using VisionRecipeRunResult result = await runner.RunAsync(pipeline, source);
        Assert(result.Success == expectedSuccess, "P183 gate outcome mismatch: " + rowId + " | " + result.SummaryText + " | " + result.Message);
        Assert(result.FirstFailedStepIndex == expectedFailedStep, "P183 failed-step mismatch: " + rowId + " | " + result.FirstFailedSummaryText + " | " + result.Message);
        VisionRecipeStepRunSummary evidenceStep = expectedSuccess ? result.Steps[0] : result.FirstFailedStep;
        if (!expectedSuccess && evidenceStep.Overlays.Count == 0)
        {
            VisionRecipeStepRunSummary priorPose = result.Steps
                .Where(step => step.Index < evidenceStep.Index && step.Overlays.Count > 0)
                .OrderByDescending(step => step.Index)
                .FirstOrDefault();
            if (priorPose != null)
            {
                evidenceStep.Overlays.AddRange(priorPose.Overlays);
                evidenceStep.OverlayCount = evidenceStep.Overlays.Count;
            }
        }
        string savedSourcePath = Path.Combine(casesDirectory, rowId + Path.GetExtension(sourcePath));
        if (!string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(savedSourcePath), StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(sourcePath, savedSourcePath, true);
        }
        string overlayPath = RenderEvidence(savedSourcePath, evidenceStep, pipeline.Steps[Math.Max(0, evidenceStep.Index - 1)], Path.Combine(casesDirectory, rowId + "_overlay.png"));
        VisionRecipeStepRunSummary audit = result.Steps.FirstOrDefault(step => step.Index == 1);
        VisionRecipeStepRunSummary pose = result.Steps.FirstOrDefault(step => step.Index == 2);
        VisionRecipeStepRunSummary normalize = result.Steps.FirstOrDefault(step => step.Index == 3);
        rows.Add(new C9GateEvidenceRow
        {
            RowId = rowId,
            Kind = kind,
            ExpectedSuccess = expectedSuccess,
            ActualSuccess = result.Success,
            ExpectedFailedStep = expectedFailedStep,
            ActualFailedStep = result.FirstFailedStepIndex,
            ErrorName = result.FirstFailedErrorName,
            Message = expectedSuccess ? result.SummaryText : result.FirstFailedStep?.Message ?? result.Message,
            ScoreMax = GetOptionalMetric(audit, VisionPipelineKnownMetrics.ScoreMax),
            ScoreMargin = GetOptionalMetric(audit, VisionPipelineKnownMetrics.ScoreMargin),
            AngleDelta = GetOptionalMetric(pose, VisionPipelineKnownMetrics.FixtureAngleDelta),
            ScaleRatio = GetOptionalMetric(pose, VisionPipelineKnownMetrics.FixtureScaleRatio),
            ValidPixelRatio = GetOptionalMetric(normalize, VisionPipelineKnownMetrics.FixtureValidPixelRatio),
            SourceSha256 = ComputeSha256(savedSourcePath),
            PipelineSha256 = pipelineSha,
            SourcePath = RelativeTo(outputRoot, savedSourcePath),
            OverlayPath = RelativeTo(outputRoot, overlayPath)
        });
    }

    private static async Task<int> RunC9FullCorpusGapBatch(
        string evidenceRoot,
        string imagesRoot,
        string outputDirectory)
    {
        Assert(Directory.Exists(evidenceRoot), "P175 evidence root must exist: " + evidenceRoot);
        Assert(Directory.Exists(imagesRoot), "Device top-left images root must exist: " + imagesRoot);
        Assert(
            !Directory.Exists(outputDirectory) || !Directory.EnumerateFileSystemEntries(outputDirectory).Any(),
            "P184 corpus output must be new or empty: " + outputDirectory);

        string sourceTemplatePath = Path.Combine(evidenceRoot, "reference", "locator_template.png");
        string sourceReferencePath = Path.Combine(evidenceRoot, "reference", "operator_reference_ok0001.jpg");
        Assert(File.Exists(sourceTemplatePath), "P175 C9 template is required.");
        Assert(File.Exists(sourceReferencePath), "P175 reviewed reference image is required.");
        Assert(
            string.Equals(
                ComputeSha256(sourceTemplatePath),
                "BA09B78D79D3A2936504B04FE70DDA066021754395772E49465B9F2BA192D9D2",
                StringComparison.OrdinalIgnoreCase),
            "P175 C9 template hash changed.");

        List<C9CorpusInput> inputs = new List<C9CorpusInput>();
        foreach (string role in new[] { "OK", "NG" })
        {
            string roleDirectory = Path.Combine(imagesRoot, role);
            Assert(Directory.Exists(roleDirectory), "Corpus role directory is missing: " + roleDirectory);
            List<string> files = Directory.EnumerateFiles(roleDirectory)
                .Where(path =>
                {
                    string extension = Path.GetExtension(path);
                    return string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase);
                })
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
                .ToList();
            Assert(files.Count == 250, "P184 requires exactly 250 " + role + " images. Actual=" + files.Count);
            foreach (string path in files)
            {
                inputs.Add(new C9CorpusInput
                {
                    RowId = role + "_" + Path.GetFileNameWithoutExtension(path),
                    RoleLabelOnly = role,
                    SourcePath = path,
                    SourceRelativePath = RelativeTo(imagesRoot, path),
                    SourceSha256 = ComputeSha256(path)
                });
            }
        }

        Assert(inputs.Count == 500, "P184 requires the exact 500-image device_top_left corpus.");
        Assert(inputs.Select(input => input.SourceSha256).Distinct(StringComparer.OrdinalIgnoreCase).Count() == 500, "P184 corpus contains duplicate image content.");
        C9CorpusInput firstOk = inputs.First(input =>
            string.Equals(Path.GetFileName(input.SourcePath), "device_top_left_OK_0001.jpg", StringComparison.OrdinalIgnoreCase));
        Assert(
            string.Equals(firstOk.SourceSha256, ComputeSha256(sourceReferencePath), StringComparison.OrdinalIgnoreCase),
            "P175 reference image does not match device_top_left_OK_0001.jpg.");

        string referenceDirectory = Path.Combine(outputDirectory, "reference");
        string runsDirectory = Path.Combine(outputDirectory, "runs");
        string contactsDirectory = Path.Combine(outputDirectory, "contacts");
        Directory.CreateDirectory(referenceDirectory);
        Directory.CreateDirectory(runsDirectory);
        Directory.CreateDirectory(contactsDirectory);
        string templatePath = Path.Combine(referenceDirectory, "locator_template.png");
        string referencePath = Path.Combine(referenceDirectory, "operator_reference_ok0001.jpg");
        File.Copy(sourceTemplatePath, templatePath, true);
        File.Copy(sourceReferencePath, referencePath, true);

        string manifestPath = Path.Combine(outputDirectory, "source_manifest.csv");
        using (StreamWriter manifest = new StreamWriter(manifestPath, false, new UTF8Encoding(false)))
        {
            manifest.WriteLine("RowId,RoleLabelOnly,SourceRelativePath,SourceSha256");
            foreach (C9CorpusInput input in inputs)
            {
                manifest.WriteLine(string.Join(",", new[]
                {
                    Csv(input.RowId), Csv(input.RoleLabelOnly), Csv(input.SourceRelativePath), Csv(input.SourceSha256)
                }));
            }
        }
        string manifestSha = ComputeSha256(manifestPath);

        VisionPipeline pipeline = CreateC9FullCorpusGapPipeline(templatePath);
        Assert(VisionPipelineValidator.Validate(pipeline, new[] { "Main" }).Success, "P184 full-corpus pipeline definition must validate.");
        string pipelinePath = Path.Combine(outputDirectory, "c9_fail_closed_normalize_gap.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(pipelinePath, pipeline), "P184 pipeline XML save must pass.");
        string pipelineSha = ComputeSha256(pipelinePath);

        VisionRecipeRunner runner = new VisionRecipeRunner();
        List<C9CorpusEvidenceRow> rows = new List<C9CorpusEvidenceRow>();
        for (int inputIndex = 0; inputIndex < inputs.Count; inputIndex++)
        {
            C9CorpusInput input = inputs[inputIndex];
            string rowDirectory = Path.Combine(runsDirectory, input.RowId);
            Directory.CreateDirectory(rowDirectory);
            string sourceCopyPath = Path.Combine(rowDirectory, "source" + Path.GetExtension(input.SourcePath).ToLowerInvariant());
            File.Copy(input.SourcePath, sourceCopyPath, true);
            Assert(string.Equals(ComputeSha256(sourceCopyPath), input.SourceSha256, StringComparison.OrdinalIgnoreCase), "P184 copied source hash changed: " + input.RowId);

            using Mat source = Cv2.ImRead(sourceCopyPath, ImreadModes.Unchanged);
            Assert(!source.Empty() && source.Width == 640 && source.Height == 480, "P184 source must be 640x480: " + input.RowId);
            using VisionRecipeRunResult result = await runner.RunAsync(pipelinePath, source);

            string resultImagePath = string.Empty;
            if (result.HasFinalResultImage)
            {
                resultImagePath = Path.Combine(rowDirectory, "runtime_result.png");
                Cv2.ImWrite(resultImagePath, result.ResultImage);
            }

            List<string> overlayPaths = new List<string>();
            foreach (VisionRecipeStepRunSummary step in result.Steps.Where(step => !step.Skipped))
            {
                if (!step.Success && step.Overlays.Count == 0)
                {
                    VisionRecipeStepRunSummary prior = result.Steps
                        .Where(candidate => candidate.Index < step.Index && candidate.Overlays.Count > 0)
                        .OrderByDescending(candidate => candidate.Index)
                        .FirstOrDefault();
                    if (prior != null)
                    {
                        step.Overlays.AddRange(prior.Overlays);
                        step.OverlayCount = step.Overlays.Count;
                    }
                }

                string baseImagePath = step.Index <= 2 || string.IsNullOrWhiteSpace(resultImagePath)
                    ? sourceCopyPath
                    : resultImagePath;
                string overlayPath = RenderEvidence(
                    baseImagePath,
                    step,
                    pipeline.Steps[step.Index - 1],
                    Path.Combine(rowDirectory, step.Index.ToString("00", CultureInfo.InvariantCulture) + "_overlay.png"));
                overlayPaths.Add(overlayPath);
            }

            VisionRecipeStepRunSummary audit = result.Steps.FirstOrDefault(step => step.Index == 1);
            VisionRecipeStepRunSummary pose = result.Steps.FirstOrDefault(step => step.Index == 2);
            VisionRecipeStepRunSummary normalize = result.Steps.FirstOrDefault(step => step.Index == 3);
            VisionRecipeStepRunSummary measurement = result.Steps.FirstOrDefault(step => step.Index == 4);
            List<VisionRecipeOverlaySummary> measurementLines = measurement?.Overlays
                .Where(overlay =>
                    string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                    && (overlay.Label ?? string.Empty).StartsWith("D", StringComparison.Ordinal))
                .ToList() ?? new List<VisionRecipeOverlaySummary>();
            bool linesInsideRoi = result.Success
                && measurementLines.Count > 0
                && measurementLines.All(IsInsideReviewedGapRoi);
            if (result.Success)
            {
                Assert(linesInsideRoi, "P184 successful measurement escaped the reviewed ROI: " + input.RowId);
                Assert(measurement.Overlays.Any(overlay =>
                    string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(overlay.Label, "Measurement ROI", StringComparison.Ordinal)),
                    "P184 measurement ROI overlay is missing: " + input.RowId);
            }

            string outcomeCategory = ClassifyC9CorpusOutcome(result);
            C9CorpusEvidenceRow row = new C9CorpusEvidenceRow
            {
                RowId = input.RowId,
                RoleLabelOnly = input.RoleLabelOnly,
                SourceRelativePath = input.SourceRelativePath,
                SourceSha256 = input.SourceSha256,
                Success = result.Success,
                OutcomeCategory = outcomeCategory,
                FailedStep = result.FirstFailedStepIndex,
                ErrorName = result.FirstFailedErrorName,
                Message = result.Success ? result.SummaryText : result.FirstFailedStep?.Message ?? result.Message,
                ScoreMax = GetOptionalMetric(audit, VisionPipelineKnownMetrics.ScoreMax),
                ScoreMargin = GetOptionalMetric(audit, VisionPipelineKnownMetrics.ScoreMargin),
                PoseScoreMax = GetOptionalMetric(pose, VisionPipelineKnownMetrics.ScoreMax),
                AngleDelta = GetOptionalMetric(pose, VisionPipelineKnownMetrics.FixtureAngleDelta),
                ScaleRatio = GetOptionalMetric(pose, VisionPipelineKnownMetrics.FixtureScaleRatio),
                ValidPixelRatio = GetOptionalMetric(normalize, VisionPipelineKnownMetrics.FixtureValidPixelRatio),
                DistanceCount = GetOptionalMetric(measurement, "DistanceCount"),
                DistancePxMin = GetOptionalMetric(measurement, "DistancePxMin"),
                DistancePxMax = GetOptionalMetric(measurement, "DistancePxMax"),
                DistancePxAverage = GetOptionalMetric(measurement, "DistancePxAvg"),
                DistancePxRange = GetOptionalMetric(measurement, "DistancePxRange"),
                MeasurementLineCount = measurementLines.Count,
                MeasurementLinesInsideRoi = linesInsideRoi,
                ElapsedMilliseconds = result.TotalMilliseconds,
                SourceCopyPath = RelativeTo(outputDirectory, sourceCopyPath),
                RuntimeResultPath = string.IsNullOrWhiteSpace(resultImagePath) ? string.Empty : RelativeTo(outputDirectory, resultImagePath),
                Step1OverlayPath = overlayPaths.Count > 0 ? RelativeTo(outputDirectory, overlayPaths[0]) : string.Empty,
                Step2OverlayPath = overlayPaths.Count > 1 ? RelativeTo(outputDirectory, overlayPaths[1]) : string.Empty,
                Step3OverlayPath = overlayPaths.Count > 2 ? RelativeTo(outputDirectory, overlayPaths[2]) : string.Empty,
                Step4OverlayPath = overlayPaths.Count > 3 ? RelativeTo(outputDirectory, overlayPaths[3]) : string.Empty,
                PrimaryOverlayPath = overlayPaths.Count > 0 ? RelativeTo(outputDirectory, overlayPaths.Last()) : string.Empty
            };
            rows.Add(row);
            if ((inputIndex + 1) % 25 == 0 || inputIndex + 1 == inputs.Count)
            {
                Console.WriteLine("P184 progress: " + (inputIndex + 1).ToString(CultureInfo.InvariantCulture) + "/" + inputs.Count.ToString(CultureInfo.InvariantCulture));
            }
        }

        Assert(rows.Count == 500, "P184 did not retain all 500 corpus rows.");
        Assert(rows.All(row => !string.IsNullOrWhiteSpace(row.PrimaryOverlayPath)), "P184 requires a current-run overlay for every row.");
        Assert(rows.All(row => !string.Equals(row.OutcomeCategory, "Unclassified", StringComparison.Ordinal)), "P184 contains an unclassified runtime outcome.");
        Assert(rows.Where(row => row.Success).All(row => row.MeasurementLinesInsideRoi), "P184 contains a successful measurement outside the reviewed ROI.");

        WriteC9CorpusRows(Path.Combine(outputDirectory, "rows.csv"), rows, pipelineSha);
        SaveC9CorpusContactSheets(rows, contactsDirectory, outputDirectory);
        List<C9CorpusRepresentative> representatives = SelectC9CorpusRepresentatives(rows);
        WriteC9CorpusRepresentatives(Path.Combine(outputDirectory, "representatives.csv"), representatives);
        SaveC9CorpusContactSheet(
            representatives.Select(item => item.Row).Distinct().ToList(),
            Path.Combine(contactsDirectory, "representative_extremes.png"),
            outputDirectory);

        List<C9CorpusEvidenceRow> measured = rows.Where(row => row.Success).ToList();
        List<C9CorpusEvidenceRow> rejected = rows.Where(row => !row.Success).ToList();
        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: frozen P183 C9 gate -> NormalizeImage -> frozen P182 pixel Gap on full device_top_left corpus",
            "CorpusRows: " + rows.Count.ToString(CultureInfo.InvariantCulture) + " (OK label-only 250; NG label-only 250)",
            "MeasuredRows: " + measured.Count.ToString(CultureInfo.InvariantCulture),
            "GateRejectedRows: " + rejected.Count.ToString(CultureInfo.InvariantCulture),
            "OutcomeCategories: " + string.Join("; ", rows.GroupBy(row => row.OutcomeCategory).OrderBy(group => group.Key).Select(group => group.Key + "=" + group.Count().ToString(CultureInfo.InvariantCulture))),
            "MeasuredByRoleLabelOnly: " + string.Join("; ", measured.GroupBy(row => row.RoleLabelOnly).OrderBy(group => group.Key).Select(group => group.Key + "=" + group.Count().ToString(CultureInfo.InvariantCulture))),
            "RejectedByRoleLabelOnly: " + string.Join("; ", rejected.GroupBy(row => row.RoleLabelOnly).OrderBy(group => group.Key).Select(group => group.Key + "=" + group.Count().ToString(CultureInfo.InvariantCulture))),
            "DistancePxAvgMinMedianMax: " + DescribeMinMedianMax(measured.Select(row => row.DistancePxAverage)),
            "DistancePxRangeMinMedianMax: " + DescribeMinMedianMax(measured.Select(row => row.DistancePxRange)),
            "ScoreMaxMinMedianMax: " + DescribeMinMedianMax(rows.Select(row => row.ScoreMax)),
            "ScoreMarginMinMedianMax: " + DescribeMinMedianMax(rows.Select(row => row.ScoreMargin)),
            "AngleDeltaMinMedianMax: " + DescribeMinMedianMax(measured.Select(row => row.AngleDelta)),
            "ScaleRatioMinMedianMax: " + DescribeMinMedianMax(measured.Select(row => row.ScaleRatio)),
            "ValidPixelRatioMinMedianMax: " + DescribeMinMedianMax(measured.Select(row => row.ValidPixelRatio)),
            "SourceManifestSha256: " + manifestSha,
            "PipelineSha256: " + pipelineSha,
            "TemplateSha256: " + ComputeSha256(templatePath),
            "ReferenceSha256: " + ComputeSha256(referencePath),
            "Boundary: OK/NG directory names are retained only as source labels, not independent Gap truth. Results are pixel-only mechanical measurements or fail-closed gate outcomes; no defect accuracy, tolerance, mm calibration, all-direction, unseen, or field qualification claim is made."
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, new UTF8Encoding(false));
        Console.WriteLine(report);
        return 0;
    }

    private static VisionPipeline CreateC9FullCorpusGapPipeline(string templatePath)
    {
        VisionPipeline pipeline = CreateC9FailClosedPipeline(templatePath, 5d);
        pipeline.Name = "C9_Fail_Closed_Normalize_Gap_Full_Corpus";
        VisionPipelineStep measurement = CreateC9GapMeasurementStep("DeviceAligned", "GapMeasured");
        measurement.Name = "04 Measure Reviewed Black Strip Gap Px";
        pipeline.Steps.Add(measurement);
        return pipeline;
    }

    private static string ClassifyC9CorpusOutcome(VisionRecipeRunResult result)
    {
        if (result.Success)
        {
            return "Measured";
        }

        string message = result.FirstFailedStep?.Message ?? result.Message ?? string.Empty;
        switch (result.FirstFailedStepIndex)
        {
            case 1:
                return string.Equals(result.FirstFailedErrorName, "MatchingNoResult", StringComparison.OrdinalIgnoreCase)
                    ? "Gate1_NoTargetOrLowScore"
                    : message.IndexOf("Margin", StringComparison.OrdinalIgnoreCase) >= 0
                        ? "Gate1_AmbiguousMargin"
                        : "Gate1_Other";
            case 2:
                return message.IndexOf("angle", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "Gate2_Angle"
                    : message.IndexOf("scale", StringComparison.OrdinalIgnoreCase) >= 0
                        ? "Gate2_Scale"
                        : "Gate2_OtherPose";
            case 3:
                return message.IndexOf("valid-pixel", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "Gate3_Coverage"
                    : "Gate3_OtherNormalize";
            case 4:
                return "MeasurementFailed";
            default:
                return "Unclassified";
        }
    }

    private static void WriteC9CorpusRows(string path, IReadOnlyCollection<C9CorpusEvidenceRow> rows, string pipelineSha)
    {
        using StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("RowId,RoleLabelOnly,SourceRelativePath,SourceSha256,PipelineSha256,Success,OutcomeCategory,FailedStep,ErrorName,Message,ScoreMax,ScoreMargin,PoseScoreMax,AngleDelta,ScaleRatio,ValidPixelRatio,DistanceCount,DistancePxMin,DistancePxMax,DistancePxAverage,DistancePxRange,MeasurementLineCount,MeasurementLinesInsideRoi,ElapsedMilliseconds,SourceCopyPath,RuntimeResultPath,Step1OverlayPath,Step2OverlayPath,Step3OverlayPath,Step4OverlayPath,PrimaryOverlayPath");
        foreach (C9CorpusEvidenceRow row in rows)
        {
            writer.WriteLine(string.Join(",", new[]
            {
                Csv(row.RowId), Csv(row.RoleLabelOnly), Csv(row.SourceRelativePath), Csv(row.SourceSha256), Csv(pipelineSha),
                row.Success.ToString(), Csv(row.OutcomeCategory), row.FailedStep.ToString(CultureInfo.InvariantCulture), Csv(row.ErrorName), Csv(row.Message),
                Number(row.ScoreMax), Number(row.ScoreMargin), Number(row.PoseScoreMax), Number(row.AngleDelta), Number(row.ScaleRatio),
                Number(row.ValidPixelRatio), Number(row.DistanceCount), Number(row.DistancePxMin), Number(row.DistancePxMax),
                Number(row.DistancePxAverage), Number(row.DistancePxRange), row.MeasurementLineCount.ToString(CultureInfo.InvariantCulture),
                row.MeasurementLinesInsideRoi.ToString(), Number(row.ElapsedMilliseconds), Csv(row.SourceCopyPath), Csv(row.RuntimeResultPath),
                Csv(row.Step1OverlayPath), Csv(row.Step2OverlayPath), Csv(row.Step3OverlayPath), Csv(row.Step4OverlayPath), Csv(row.PrimaryOverlayPath)
            }));
        }
    }

    private static void SaveC9CorpusContactSheets(
        IReadOnlyList<C9CorpusEvidenceRow> rows,
        string contactsDirectory,
        string outputRoot)
    {
        const int rowsPerPage = 25;
        int pageCount = (int)Math.Ceiling(rows.Count / (double)rowsPerPage);
        for (int page = 0; page < pageCount; page++)
        {
            List<C9CorpusEvidenceRow> pageRows = rows.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
            SaveC9CorpusContactSheet(
                pageRows,
                Path.Combine(contactsDirectory, "all_" + (page + 1).ToString("00", CultureInfo.InvariantCulture) + ".png"),
                outputRoot);
        }

        List<C9CorpusEvidenceRow> failures = rows.Where(row => !row.Success).ToList();
        for (int page = 0; page < (int)Math.Ceiling(failures.Count / (double)rowsPerPage); page++)
        {
            SaveC9CorpusContactSheet(
                failures.Skip(page * rowsPerPage).Take(rowsPerPage).ToList(),
                Path.Combine(contactsDirectory, "gate_rejections_" + (page + 1).ToString("00", CultureInfo.InvariantCulture) + ".png"),
                outputRoot);
        }
    }

    private static void SaveC9CorpusContactSheet(
        IReadOnlyList<C9CorpusEvidenceRow> rows,
        string outputPath,
        string outputRoot)
    {
        if (rows.Count == 0)
        {
            return;
        }

        const int tileWidth = 320;
        const int tileHeight = 260;
        const int columns = 5;
        int rowCount = (int)Math.Ceiling(rows.Count / (double)columns);
        using Bitmap sheet = new Bitmap(columns * tileWidth, rowCount * tileHeight);
        using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(sheet);
        graphics.Clear(System.Drawing.Color.FromArgb(24, 24, 24));
        using System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
        using System.Drawing.Brush text = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        for (int index = 0; index < rows.Count; index++)
        {
            C9CorpusEvidenceRow row = rows[index];
            int x = (index % columns) * tileWidth;
            int y = (index / columns) * tileHeight;
            string imagePath = Path.Combine(outputRoot, row.PrimaryOverlayPath.Replace('/', Path.DirectorySeparatorChar));
            using Bitmap image = new Bitmap(imagePath);
            graphics.DrawImage(image, new System.Drawing.Rectangle(x, y, tileWidth, 238));
            string metric = row.Success ? "gap " + Number(row.DistancePxAverage) : row.OutcomeCategory;
            graphics.DrawString(row.RowId + " | " + metric, font, text, x + 3, y + 240);
        }
        sheet.Save(outputPath);
    }

    private static List<C9CorpusRepresentative> SelectC9CorpusRepresentatives(IReadOnlyCollection<C9CorpusEvidenceRow> rows)
    {
        List<C9CorpusRepresentative> selected = new List<C9CorpusRepresentative>();
        List<C9CorpusEvidenceRow> measured = rows.Where(row => row.Success).ToList();
        AddRepresentative(selected, "ordinary_first_measured", measured.FirstOrDefault());
        AddRepresentative(selected, "minimum_score", rows.Where(row => !double.IsNaN(row.ScoreMax)).OrderBy(row => row.ScoreMax).FirstOrDefault());
        AddRepresentative(selected, "minimum_margin", rows.Where(row => !double.IsNaN(row.ScoreMargin)).OrderBy(row => row.ScoreMargin).FirstOrDefault());
        AddRepresentative(selected, "maximum_abs_angle", measured.Where(row => !double.IsNaN(row.AngleDelta)).OrderByDescending(row => Math.Abs(row.AngleDelta)).FirstOrDefault());
        AddRepresentative(selected, "minimum_scale", measured.Where(row => !double.IsNaN(row.ScaleRatio)).OrderBy(row => row.ScaleRatio).FirstOrDefault());
        AddRepresentative(selected, "maximum_scale", measured.Where(row => !double.IsNaN(row.ScaleRatio)).OrderByDescending(row => row.ScaleRatio).FirstOrDefault());
        AddRepresentative(selected, "minimum_coverage", measured.Where(row => !double.IsNaN(row.ValidPixelRatio)).OrderBy(row => row.ValidPixelRatio).FirstOrDefault());
        AddRepresentative(selected, "minimum_gap_average", measured.Where(row => !double.IsNaN(row.DistancePxAverage)).OrderBy(row => row.DistancePxAverage).FirstOrDefault());
        AddRepresentative(selected, "maximum_gap_average", measured.Where(row => !double.IsNaN(row.DistancePxAverage)).OrderByDescending(row => row.DistancePxAverage).FirstOrDefault());
        AddRepresentative(selected, "maximum_gap_range", measured.Where(row => !double.IsNaN(row.DistancePxRange)).OrderByDescending(row => row.DistancePxRange).FirstOrDefault());
        foreach (IGrouping<string, C9CorpusEvidenceRow> group in rows.Where(row => !row.Success).GroupBy(row => row.OutcomeCategory).OrderBy(group => group.Key))
        {
            AddRepresentative(selected, "failure_" + group.Key, group.First());
        }
        return selected;
    }

    private static void AddRepresentative(
        ICollection<C9CorpusRepresentative> selected,
        string reason,
        C9CorpusEvidenceRow row)
    {
        if (row != null)
        {
            selected.Add(new C9CorpusRepresentative { Reason = reason, Row = row });
        }
    }

    private static void WriteC9CorpusRepresentatives(string path, IReadOnlyCollection<C9CorpusRepresentative> representatives)
    {
        using StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("Reason,RowId,RoleLabelOnly,OutcomeCategory,ScoreMax,ScoreMargin,AngleDelta,ScaleRatio,ValidPixelRatio,DistancePxAverage,DistancePxRange,PrimaryOverlayPath");
        foreach (C9CorpusRepresentative representative in representatives)
        {
            C9CorpusEvidenceRow row = representative.Row;
            writer.WriteLine(string.Join(",", new[]
            {
                Csv(representative.Reason), Csv(row.RowId), Csv(row.RoleLabelOnly), Csv(row.OutcomeCategory), Number(row.ScoreMax),
                Number(row.ScoreMargin), Number(row.AngleDelta), Number(row.ScaleRatio), Number(row.ValidPixelRatio),
                Number(row.DistancePxAverage), Number(row.DistancePxRange), Csv(row.PrimaryOverlayPath)
            }));
        }
    }

    private static string DescribeMinMedianMax(IEnumerable<double> values)
    {
        List<double> ordered = values.Where(value => !double.IsNaN(value) && !double.IsInfinity(value)).OrderBy(value => value).ToList();
        if (ordered.Count == 0)
        {
            return "n/a";
        }

        double median = ordered.Count % 2 == 1
            ? ordered[ordered.Count / 2]
            : (ordered[(ordered.Count / 2) - 1] + ordered[ordered.Count / 2]) / 2d;
        return ordered.First().ToString("0.###", CultureInfo.InvariantCulture)
            + " / " + median.ToString("0.###", CultureInfo.InvariantCulture)
            + " / " + ordered.Last().ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static async Task<int> RunC9NormalizedGapBatch(string evidenceRoot, string outputDirectory)
    {
        Assert(Directory.Exists(evidenceRoot), "P175 evidence root must exist: " + evidenceRoot);
        Assert(
            !Directory.Exists(outputDirectory) || !Directory.EnumerateFileSystemEntries(outputDirectory).Any(),
            "P182 output must be new or empty: " + outputDirectory);

        string rowsPath = Path.Combine(evidenceRoot, "native_rows.csv");
        string sourceTemplatePath = Path.Combine(evidenceRoot, "reference", "locator_template.png");
        string referenceSourcePath = Path.Combine(evidenceRoot, "reference", "operator_reference_ok0001.jpg");
        Assert(File.Exists(rowsPath), "P175 native_rows.csv is required.");
        Assert(File.Exists(sourceTemplatePath), "P175 C9 template is required.");
        Assert(File.Exists(referenceSourcePath), "P175 reviewed reference image is required.");

        List<Dictionary<string, string>> sourceRows = ReadCsv(rowsPath);
        Assert(sourceRows.Count == 24, "P182 requires the exact P175 24 rows. Actual=" + sourceRows.Count);
        Assert(
            string.Equals(
                ComputeSha256(sourceTemplatePath),
                "BA09B78D79D3A2936504B04FE70DDA066021754395772E49465B9F2BA192D9D2",
                StringComparison.OrdinalIgnoreCase),
            "P175 C9 template hash changed.");

        string referenceDirectory = Path.Combine(outputDirectory, "reference");
        string runsDirectory = Path.Combine(outputDirectory, "runs");
        string contactsDirectory = Path.Combine(outputDirectory, "contacts");
        Directory.CreateDirectory(referenceDirectory);
        Directory.CreateDirectory(runsDirectory);
        Directory.CreateDirectory(contactsDirectory);
        string templatePath = Path.Combine(referenceDirectory, "locator_template.png");
        string referencePath = Path.Combine(referenceDirectory, "operator_reference_ok0001.jpg");
        File.Copy(sourceTemplatePath, templatePath, true);
        File.Copy(referenceSourcePath, referencePath, true);
        File.Copy(rowsPath, Path.Combine(referenceDirectory, "p175_native_rows.csv"), true);

        VisionPipeline normalizedPipeline = CreateC9NormalizedGapPipeline(templatePath, includeMeasurement: true);
        VisionPipeline normalizationPrefix = CreateC9NormalizedGapPipeline(templatePath, includeMeasurement: false);
        VisionPipeline controlPipeline = CreateC9RawGapControlPipeline();
        Assert(
            VisionPipelineValidator.Validate(normalizedPipeline, new[] { "Main" }).Success,
            "P182 normalized pipeline definition must validate.");
        Assert(
            VisionPipelineValidator.Validate(controlPipeline, new[] { "Main" }).Success,
            "P182 raw control pipeline definition must validate.");
        AssertSameMeasurementParameters(normalizedPipeline.Steps[2], controlPipeline.Steps[0]);

        string normalizedPipelinePath = Path.Combine(outputDirectory, "c9_matching_normalize_linedistance.pipeline.xml");
        string controlPipelinePath = Path.Combine(outputDirectory, "c9_raw_linedistance_control.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(normalizedPipelinePath, normalizedPipeline), "P182 normalized XML save must pass.");
        Assert(SerializeHelper.SaveXmlFile(controlPipelinePath, controlPipeline), "P182 control XML save must pass.");
        string normalizedPipelineSha = ComputeSha256(normalizedPipelinePath);
        string controlPipelineSha = ComputeSha256(controlPipelinePath);

        VisionRecipeRunner runner = new VisionRecipeRunner();
        List<C9GapEvidenceRow> results = new List<C9GapEvidenceRow>();
        foreach (Dictionary<string, string> sourceRow in sourceRows)
        {
            string rowId = sourceRow["RowId"];
            string sourcePath = Path.GetFullPath(Path.Combine(
                evidenceRoot,
                sourceRow["SourceCopyPath"].Replace('/', Path.DirectorySeparatorChar)));
            Assert(File.Exists(sourcePath), "P175 copied source is missing: " + sourcePath);
            Assert(
                string.Equals(ComputeSha256(sourcePath), sourceRow["SourceSha256"], StringComparison.OrdinalIgnoreCase),
                "P175 source hash changed: " + rowId);

            string rowDirectory = Path.Combine(runsDirectory, rowId);
            Directory.CreateDirectory(rowDirectory);
            string sourceCopyPath = Path.Combine(rowDirectory, "source.jpg");
            File.Copy(sourcePath, sourceCopyPath, true);
            using Mat source = Cv2.ImRead(sourcePath, ImreadModes.Unchanged);
            Assert(!source.Empty() && source.Width == 640 && source.Height == 480, "P182 source must be 640x480: " + rowId);

            using VisionRecipeRunResult prefixResult = await runner.RunAsync(normalizationPrefix, source);
            using VisionRecipeRunResult normalizedResult = await runner.RunAsync(normalizedPipeline, source);
            using VisionRecipeRunResult controlResult = await runner.RunAsync(controlPipeline, source);
            Assert(prefixResult.Success && prefixResult.Steps.Count == 2, "Normalization prefix failed: " + rowId + " | " + prefixResult.Message);
            Assert(normalizedResult.Success && normalizedResult.Steps.Count == 3, "Normalized Gap run failed: " + rowId + " | " + normalizedResult.Message);
            AssertPoseParity(prefixResult.Steps[0], normalizedResult.Steps[0], rowId);

            VisionRecipeStepRunSummary matching = normalizedResult.Steps[0];
            VisionRecipeStepRunSummary normalize = normalizedResult.Steps[1];
            VisionRecipeStepRunSummary measurement = normalizedResult.Steps[2];
            List<VisionRecipeOverlaySummary> measurementLines = measurement.Overlays
                .Where(overlay =>
                    string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                    && (overlay.Label ?? string.Empty).StartsWith("D", StringComparison.Ordinal))
                .ToList();
            bool linesInsideRoi = measurementLines.Count > 0
                && measurementLines.All(IsInsideReviewedGapRoi);
            Assert(
                linesInsideRoi,
                "Normalized measurement lines escaped the reviewed ROI: " + rowId + " | "
                    + string.Join(
                        "; ",
                        measurementLines
                            .Where(line => !IsInsideReviewedGapRoi(line))
                            .Take(6)
                            .Select(line => string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}=({1},{2})->({3},{4})",
                                line.Label,
                                line.StartX,
                                line.StartY,
                                line.EndX,
                                line.EndY))));
            Assert(
                measurement.Overlays.Any(overlay =>
                    string.Equals(overlay.Kind, "Rectangle", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(overlay.Label, "Measurement ROI", StringComparison.Ordinal)),
                "LineDistance runtime overlay must retain the measurement ROI: " + rowId);
            Assert(
                measurement.Overlays.Count(overlay =>
                    string.Equals(overlay.Kind, "Line", StringComparison.OrdinalIgnoreCase)
                    && (string.Equals(overlay.Label, "Line A fitted edge", StringComparison.Ordinal)
                        || string.Equals(overlay.Label, "Line B fitted edge", StringComparison.Ordinal))) == 2,
                "LineDistance fitted-edge mode must retain both fitted boundaries: " + rowId);

            string normalizedImagePath = Path.Combine(rowDirectory, "device_aligned.png");
            Cv2.ImWrite(normalizedImagePath, prefixResult.ResultImage);
            string measuredResultPath = Path.Combine(rowDirectory, "normalized_measurement_result.png");
            Cv2.ImWrite(measuredResultPath, normalizedResult.ResultImage);
            string matchingOverlayPath = RenderEvidence(
                sourceCopyPath,
                matching,
                normalizedPipeline.Steps[0],
                Path.Combine(rowDirectory, "01_matching_overlay.png"));
            string normalizedOverlayPath = RenderEvidence(
                normalizedImagePath,
                normalize,
                normalizedPipeline.Steps[1],
                Path.Combine(rowDirectory, "02_normalized_overlay.png"));
            string measurementOverlayPath = RenderEvidence(
                normalizedImagePath,
                measurement,
                normalizedPipeline.Steps[2],
                Path.Combine(rowDirectory, "03_measurement_overlay.png"));
            string controlOverlayPath = RenderEvidence(
                sourceCopyPath,
                controlResult.Steps.FirstOrDefault(),
                controlPipeline.Steps[0],
                Path.Combine(rowDirectory, "04_raw_control_overlay.png"));

            results.Add(new C9GapEvidenceRow
            {
                RowId = rowId,
                Split = sourceRow["Split"],
                RoleLabelOnly = sourceRow["RoleLabelOnly"],
                SourceImage = sourceRow["Image"],
                SourceSha256 = sourceRow["SourceSha256"],
                MatchingScore = GetMetric(matching, "ScoreMax"),
                CenterX = GetMetric(matching, "FixtureCenterX"),
                CenterY = GetMetric(matching, "FixtureCenterY"),
                Angle = GetMetric(matching, "FixtureAngle"),
                Scale = GetMetric(matching, "FixtureScale"),
                ValidPixelRatio = GetMetric(normalize, "FixtureValidPixelRatio"),
                NormalizedSuccess = normalizedResult.Success,
                NormalizedCount = GetMetric(measurement, "DistanceCount"),
                NormalizedMin = GetMetric(measurement, "DistancePxMin"),
                NormalizedMax = GetMetric(measurement, "DistancePxMax"),
                NormalizedAverage = GetMetric(measurement, "DistancePxAvg"),
                NormalizedRange = GetMetric(measurement, "DistancePxRange"),
                NormalizedLineCount = measurementLines.Count,
                NormalizedLinesInsideRoi = linesInsideRoi,
                ControlSuccess = controlResult.Success,
                ControlCount = GetOptionalMetric(controlResult.Steps.FirstOrDefault(), "DistanceCount"),
                ControlMin = GetOptionalMetric(controlResult.Steps.FirstOrDefault(), "DistancePxMin"),
                ControlMax = GetOptionalMetric(controlResult.Steps.FirstOrDefault(), "DistancePxMax"),
                ControlAverage = GetOptionalMetric(controlResult.Steps.FirstOrDefault(), "DistancePxAvg"),
                ControlRange = GetOptionalMetric(controlResult.Steps.FirstOrDefault(), "DistancePxRange"),
                SourceCopyPath = RelativeTo(outputDirectory, sourceCopyPath),
                MatchingOverlayPath = RelativeTo(outputDirectory, matchingOverlayPath),
                NormalizedImagePath = RelativeTo(outputDirectory, normalizedImagePath),
                NormalizedOverlayPath = RelativeTo(outputDirectory, normalizedOverlayPath),
                MeasurementOverlayPath = RelativeTo(outputDirectory, measurementOverlayPath),
                ControlOverlayPath = RelativeTo(outputDirectory, controlOverlayPath)
            });
        }

        WriteC9GapRows(Path.Combine(outputDirectory, "rows.csv"), results, normalizedPipelineSha, controlPipelineSha);
        foreach (IGrouping<string, C9GapEvidenceRow> split in results.GroupBy(row => row.Split))
        {
            SaveContactSheet(
                split.ToList(),
                Path.Combine(contactsDirectory, split.Key + "_normalized_measurements.png"),
                outputDirectory);
        }

        SaveContactSheet(results, Path.Combine(contactsDirectory, "all_normalized_measurements.png"), outputDirectory);
        int normalizedPassCount = results.Count(row => row.NormalizedSuccess && row.NormalizedLinesInsideRoi);
        int controlPassCount = results.Count(row => row.ControlSuccess);
        string report = string.Join(Environment.NewLine, new[]
        {
            "Result: PASS",
            "Scenario: C9 Matching -> NormalizeImage -> LineDistance versus raw control",
            "Rows: " + results.Count.ToString(CultureInfo.InvariantCulture),
            "NormalizedMechanicalAndRoiPass: " + normalizedPassCount.ToString(CultureInfo.InvariantCulture) + "/" + results.Count.ToString(CultureInfo.InvariantCulture),
            "RawControlMechanicalPass: " + controlPassCount.ToString(CultureInfo.InvariantCulture) + "/" + results.Count.ToString(CultureInfo.InvariantCulture),
            "NormalizedDistancePxAvgMinMax: " + results.Min(row => row.NormalizedAverage).ToString("0.###", CultureInfo.InvariantCulture) + " / " + results.Max(row => row.NormalizedAverage).ToString("0.###", CultureInfo.InvariantCulture),
            "NormalizedDistancePxRangeMax: " + results.Max(row => row.NormalizedRange).ToString("0.###", CultureInfo.InvariantCulture),
            "MatchingScoreMin: " + results.Min(row => row.MatchingScore).ToString("0.###", CultureInfo.InvariantCulture),
            "ValidPixelRatioMin: " + results.Min(row => row.ValidPixelRatio).ToString("0.###", CultureInfo.InvariantCulture),
            "NormalizedPipelineSha256: " + normalizedPipelineSha,
            "ControlPipelineSha256: " + controlPipelineSha,
            "Boundary: mechanical and coordinate/drawing evidence only; role labels are not Gap truth and no OK/NG or mm claim is made."
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, new UTF8Encoding(false));
        Console.WriteLine(report);
        return 0;
    }

    private static VisionPipeline CreateC9NormalizedGapPipeline(string templatePath, bool includeMeasurement)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = includeMeasurement ? "C9_Normalized_Gap" : "C9_Normalization_Prefix" };
        VisionPipelineStep matching = new VisionPipelineStep
        {
            Name = "01 Locate C9 Device Pose",
            ToolType = "Matching",
            InputLayer = "Main",
            OutputLayer = "FixtureMatch"
        };
        Add(matching, "Name", "C9DevicePose");
        Add(matching, "TemplatePath", templatePath);
        Add(matching, "PATTERN_PATH", templatePath);
        Add(matching, "MATCH_MODE", "CCoeffNormed");
        Add(matching, "SCORE_MIN", "0");
        Add(matching, "MAGNIFIATION", "1");
        Add(matching, "NUM_MATCH", "1");
        Add(matching, "USE_FIND_ANGLE", "true");
        Add(matching, "FIND_ANGLE_MIN", "-5");
        Add(matching, "FIND_ANGLE_MAX", "5");
        Add(matching, "FIND_ANGLE", "1");
        Add(matching, "USE_COARSE_TO_FINE_ANGLE_SEARCH", "false");
        Add(matching, "USE_FIND_SCALE", "true");
        Add(matching, "FIND_SCALE_MIN", "0.8");
        Add(matching, "FIND_SCALE_MAX", "1.9");
        Add(matching, "FIND_SCALE_STEP", "0.1");
        Add(matching, "USE_PYRAMID_POSITION_PROPOSAL", "false");
        Add(matching, "USE_CANNY", "false");
        Add(matching, "USE_THRESHOLD", "false");
        Add(matching, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(matching, "USE_ROI", "false");
        Add(matching, "USE_MULTI_ROI", "false");
        Add(matching, "USE_PADDING_COLOR_WHITE", "true");
        Add(matching, "USE_AS_FIXTURE_FRAME", "true");
        Add(matching, "FIXTURE_FRAME_NAME", "DeviceFrame");
        Add(matching, "FIXTURE_REFERENCE_X", "272");
        Add(matching, "FIXTURE_REFERENCE_Y", "300");
        Add(matching, "FIXTURE_REFERENCE_ANGLE", "0");
        Add(matching, "FIXTURE_REFERENCE_SCALE", "1");
        Add(matching, "FIXTURE_MAX_ANGLE_DELTA", "5.25");
        Add(matching, "FIXTURE_REFERENCE_IMAGE_WIDTH", "640");
        Add(matching, "FIXTURE_REFERENCE_IMAGE_HEIGHT", "480");
        pipeline.Steps.Add(matching);

        VisionPipelineStep normalize = new VisionPipelineStep
        {
            Name = "02 Normalize Device Image",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "DeviceAligned"
        };
        Add(normalize, "Name", "C9DeviceNormalizeImage");
        Add(normalize, "Angle", "0");
        Add(normalize, "ScaleXPercent", "100");
        Add(normalize, "ScaleYPercent", "100");
        Add(normalize, "Interpolation", "Linear");
        Add(normalize, "BorderType", "Constant");
        Add(normalize, "USE_FIXTURE_FRAME", "true");
        Add(normalize, "FIXTURE_FRAME_NAME", "DeviceFrame");
        Add(normalize, "FIXTURE_APPLY_MODE", "NormalizeImage");
        Add(normalize, "FIXTURE_MIN_VALID_PIXEL_RATIO", "0.25");
        Add(normalize, "ALLOW_BRANCH_INPUT", "true");
        pipeline.Steps.Add(normalize);
        if (includeMeasurement)
        {
            pipeline.Steps.Add(CreateC9GapMeasurementStep("DeviceAligned", "GapMeasured"));
        }

        return pipeline;
    }

    private static VisionPipeline CreateC9FailClosedPipeline(string templatePath, double angleSearchLimit)
    {
        VisionPipeline firstSource = CreateC9NormalizedGapPipeline(templatePath, includeMeasurement: false);
        VisionPipeline secondSource = CreateC9NormalizedGapPipeline(templatePath, includeMeasurement: false);
        VisionPipeline pipeline = new VisionPipeline { Name = "C9_Fail_Closed_Pre_Measurement" };

        VisionPipelineStep candidateAudit = firstSource.Steps[0];
        candidateAudit.Name = "01 Reject Missing Or Ambiguous C9";
        candidateAudit.OutputLayer = "CandidateAudit";
        candidateAudit.Parameters["USE_AS_FIXTURE_FRAME"] = "false";
        candidateAudit.Parameters["NUM_MATCH"] = "2";
        candidateAudit.Parameters["SCORE_MIN"] = "0.8";
        candidateAudit.Parameters["FIND_ANGLE_MIN"] = (-angleSearchLimit).ToString("0.###", CultureInfo.InvariantCulture);
        candidateAudit.Parameters["FIND_ANGLE_MAX"] = angleSearchLimit.ToString("0.###", CultureInfo.InvariantCulture);
        candidateAudit.UseAcceptance = true;
        candidateAudit.ExpectedSuccess = true;
        candidateAudit.AcceptanceMetricName = VisionPipelineKnownMetrics.ScoreMargin;
        candidateAudit.UseAcceptanceMetricMinimum = true;
        candidateAudit.AcceptanceMetricMinimum = 10d;
        pipeline.Steps.Add(candidateAudit);

        VisionPipelineStep pose = secondSource.Steps[0];
        pose.Name = "02 Publish Bounded C9 Pose";
        pose.Parameters["SCORE_MIN"] = "0.8";
        pose.Parameters["FIND_ANGLE_MIN"] = (-angleSearchLimit).ToString("0.###", CultureInfo.InvariantCulture);
        pose.Parameters["FIND_ANGLE_MAX"] = angleSearchLimit.ToString("0.###", CultureInfo.InvariantCulture);
        pose.Parameters["FIXTURE_MIN_SCALE_RATIO"] = "0.8";
        pose.Parameters["FIXTURE_MAX_SCALE_RATIO"] = "1.8";
        pose.Parameters[VisionPipelineNormalizer.AllowBranchInputParameter] = "true";
        pipeline.Steps.Add(pose);

        VisionPipelineStep normalize = secondSource.Steps[1];
        normalize.Name = "03 Reject Low-Coverage Normalization";
        pipeline.Steps.Add(normalize);
        return pipeline;
    }

    private static VisionPipeline CreateC9RawGapControlPipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "C9_Raw_Gap_Control" };
        pipeline.Steps.Add(CreateC9GapMeasurementStep("Main", "RawGapMeasured"));
        return pipeline;
    }

    private static VisionPipelineStep CreateC9GapMeasurementStep(string inputLayer, string outputLayer)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = "03 Measure Reviewed Black Strip Gap Px",
            ToolType = "LineDistance",
            InputLayer = inputLayer,
            OutputLayer = outputLayer
        };
        Add(step, "Name", "ReviewedBlackStripGap");
        Add(step, "PIXELPERMM", "0");
        Add(step, "USE_THRESHOLD", "false");
        Add(step, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(step, "USE_BITWISENOT", "false");
        Add(step, "USE_ROI", "true");
        Add(step, "CvROI", "20,200,510,60");
        Add(step, "LeftPRJ_DIR", "Y_TTOB");
        Add(step, "RightPRJ_DIR", "Y_BTOT");
        Add(step, "PRJ_PORALITY", "WTOB");
        Add(step, "CONTRAST", "18");
        Add(step, "THICKNESS", "2");
        Add(step, "SAMPLING_STEP", "8");
        Add(step, "POINT_RANGE", "12");
        Add(step, "VER_PRJ_DIR", "Y_BTOT");
        Add(step, "USE_MANUAL_ANGLE", "true");
        Add(step, "MANUAL_ANGLE_VALUE", "0");
        Add(step, "USE_EXTEND_FIT_LINE", "true");
        Add(step, "EXTEND_FIT_LINE_VALUE", "100");
        Add(step, "SHOW_VERTICAL_LINE", "true");
        Add(step, "SHOW_EDGE", "true");
        Add(step, "SHOW_FITLINE", "true");
        return step;
    }

    private static void AssertSameMeasurementParameters(VisionPipelineStep normalized, VisionPipelineStep control)
    {
        string left = string.Join("\n", normalized.Parameters.OrderBy(item => item.Key).Select(item => item.Key + "=" + item.Value));
        string right = string.Join("\n", control.Parameters.OrderBy(item => item.Key).Select(item => item.Key + "=" + item.Value));
        Assert(string.Equals(left, right, StringComparison.Ordinal), "Normalized and raw-control measurement parameters must be identical.");
    }

    private static void AssertPoseParity(VisionRecipeStepRunSummary expected, VisionRecipeStepRunSummary actual, string rowId)
    {
        foreach (string metric in new[] { "FixtureCenterX", "FixtureCenterY", "FixtureAngle", "FixtureScale" })
        {
            Assert(
                Math.Abs(GetMetric(expected, metric) - GetMetric(actual, metric)) <= 0.000001d,
                "Normalization-prefix/full-pipeline pose mismatch for " + rowId + ": " + metric);
        }
    }

    private static bool IsInsideReviewedGapRoi(VisionRecipeOverlaySummary overlay)
    {
        const float minimumX = 20f;
        const float maximumX = 530f;
        const float minimumY = 200f;
        const float maximumY = 260f;
        return overlay.StartX >= minimumX && overlay.StartX <= maximumX
            && overlay.EndX >= minimumX && overlay.EndX <= maximumX
            && overlay.StartY >= minimumY && overlay.StartY <= maximumY
            && overlay.EndY >= minimumY && overlay.EndY <= maximumY;
    }

    private static string RenderEvidence(
        string baseImagePath,
        VisionRecipeStepRunSummary summary,
        VisionPipelineStep step,
        string outputPath)
    {
        using Bitmap source = new Bitmap(baseImagePath);
        using Bitmap image = new Bitmap(
            source.Width,
            source.Height,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image))
        {
            graphics.DrawImageUnscaled(source, 0, 0);
        }
        if (summary != null)
        {
            VisionPipelineRunReportImageRenderer.RenderInPlace(image, summary, step);
        }
        image.Save(outputPath);
        return outputPath;
    }

    private static void SaveContactSheet(IReadOnlyCollection<C9GapEvidenceRow> rows, string outputPath, string outputRoot)
    {
        const int tileWidth = 320;
        const int tileHeight = 260;
        int columns = rows.Count > 12 ? 4 : 2;
        int rowCount = (int)Math.Ceiling(rows.Count / (double)columns);
        using Bitmap sheet = new Bitmap(columns * tileWidth, rowCount * tileHeight);
        using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(sheet);
        graphics.Clear(System.Drawing.Color.FromArgb(24, 24, 24));
        using System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
        using System.Drawing.Brush text = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        int index = 0;
        foreach (C9GapEvidenceRow row in rows)
        {
            int x = (index % columns) * tileWidth;
            int y = (index / columns) * tileHeight;
            string imagePath = Path.Combine(outputRoot, row.MeasurementOverlayPath.Replace('/', Path.DirectorySeparatorChar));
            using Bitmap image = new Bitmap(imagePath);
            graphics.DrawImage(image, new System.Drawing.Rectangle(x, y, tileWidth, 240));
            string label = row.RowId + " | avg " + row.NormalizedAverage.ToString("0.##", CultureInfo.InvariantCulture)
                + " | raw " + (row.ControlSuccess ? row.ControlAverage.ToString("0.##", CultureInfo.InvariantCulture) : "FAIL");
            graphics.DrawString(label, font, text, x + 3, y + 241);
            index++;
        }
        sheet.Save(outputPath);
    }

    private static void WriteC9GapRows(
        string path,
        IReadOnlyCollection<C9GapEvidenceRow> rows,
        string normalizedPipelineSha,
        string controlPipelineSha)
    {
        using StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("RowId,Split,RoleLabelOnly,SourceImage,SourceSha256,NormalizedPipelineSha256,ControlPipelineSha256,MatchingScore,CenterX,CenterY,Angle,Scale,ValidPixelRatio,NormalizedSuccess,NormalizedCount,NormalizedMin,NormalizedMax,NormalizedAverage,NormalizedRange,NormalizedLineCount,NormalizedLinesInsideRoi,ControlSuccess,ControlCount,ControlMin,ControlMax,ControlAverage,ControlRange,SourceCopyPath,MatchingOverlayPath,NormalizedImagePath,NormalizedOverlayPath,MeasurementOverlayPath,ControlOverlayPath");
        foreach (C9GapEvidenceRow row in rows)
        {
            writer.WriteLine(string.Join(",", new[]
            {
                Csv(row.RowId), Csv(row.Split), Csv(row.RoleLabelOnly), Csv(row.SourceImage), Csv(row.SourceSha256),
                Csv(normalizedPipelineSha), Csv(controlPipelineSha), Number(row.MatchingScore), Number(row.CenterX), Number(row.CenterY),
                Number(row.Angle), Number(row.Scale), Number(row.ValidPixelRatio), row.NormalizedSuccess.ToString(), Number(row.NormalizedCount),
                Number(row.NormalizedMin), Number(row.NormalizedMax), Number(row.NormalizedAverage), Number(row.NormalizedRange),
                row.NormalizedLineCount.ToString(CultureInfo.InvariantCulture), row.NormalizedLinesInsideRoi.ToString(), row.ControlSuccess.ToString(),
                Number(row.ControlCount), Number(row.ControlMin), Number(row.ControlMax), Number(row.ControlAverage), Number(row.ControlRange),
                Csv(row.SourceCopyPath), Csv(row.MatchingOverlayPath), Csv(row.NormalizedImagePath), Csv(row.NormalizedOverlayPath),
                Csv(row.MeasurementOverlayPath), Csv(row.ControlOverlayPath)
            }));
        }
    }

    private static List<Dictionary<string, string>> ReadCsv(string path)
    {
        using TextFieldParser parser = new TextFieldParser(path, Encoding.UTF8);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        string[] headers = parser.ReadFields() ?? Array.Empty<string>();
        List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
        while (!parser.EndOfData)
        {
            string[] values = parser.ReadFields() ?? Array.Empty<string>();
            Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < headers.Length; index++)
            {
                row[headers[index]] = index < values.Length ? values[index] : string.Empty;
            }
            rows.Add(row);
        }
        return rows;
    }

    private static string ComputeSha256(string path)
    {
        using SHA256 sha = SHA256.Create();
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(sha.ComputeHash(stream));
    }

    private static double GetOptionalMetric(VisionRecipeStepRunSummary step, string name)
    {
        return step?.Metrics != null && step.Metrics.TryGetValue(name, out double value) ? value : double.NaN;
    }

    private static string RelativeTo(string root, string path)
    {
        return Path.GetRelativePath(root, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    private static string Number(double value)
    {
        return double.IsNaN(value) ? string.Empty : value.ToString("0.######", CultureInfo.InvariantCulture);
    }

    private static string Csv(string value)
    {
        string text = value ?? string.Empty;
        return '"' + text.Replace("\"", "\"\"") + '"';
    }

    private sealed class C9GapEvidenceRow
    {
        public string RowId { get; set; } = string.Empty;
        public string Split { get; set; } = string.Empty;
        public string RoleLabelOnly { get; set; } = string.Empty;
        public string SourceImage { get; set; } = string.Empty;
        public string SourceSha256 { get; set; } = string.Empty;
        public double MatchingScore { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Angle { get; set; }
        public double Scale { get; set; }
        public double ValidPixelRatio { get; set; }
        public bool NormalizedSuccess { get; set; }
        public double NormalizedCount { get; set; }
        public double NormalizedMin { get; set; }
        public double NormalizedMax { get; set; }
        public double NormalizedAverage { get; set; }
        public double NormalizedRange { get; set; }
        public int NormalizedLineCount { get; set; }
        public bool NormalizedLinesInsideRoi { get; set; }
        public bool ControlSuccess { get; set; }
        public double ControlCount { get; set; }
        public double ControlMin { get; set; }
        public double ControlMax { get; set; }
        public double ControlAverage { get; set; }
        public double ControlRange { get; set; }
        public string SourceCopyPath { get; set; } = string.Empty;
        public string MatchingOverlayPath { get; set; } = string.Empty;
        public string NormalizedImagePath { get; set; } = string.Empty;
        public string NormalizedOverlayPath { get; set; } = string.Empty;
        public string MeasurementOverlayPath { get; set; } = string.Empty;
        public string ControlOverlayPath { get; set; } = string.Empty;
    }

    private sealed class C9CorpusInput
    {
        public string RowId { get; set; } = string.Empty;
        public string RoleLabelOnly { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string SourceRelativePath { get; set; } = string.Empty;
        public string SourceSha256 { get; set; } = string.Empty;
    }

    private sealed class C9CorpusEvidenceRow
    {
        public string RowId { get; set; } = string.Empty;
        public string RoleLabelOnly { get; set; } = string.Empty;
        public string SourceRelativePath { get; set; } = string.Empty;
        public string SourceSha256 { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string OutcomeCategory { get; set; } = string.Empty;
        public int FailedStep { get; set; }
        public string ErrorName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public double ScoreMax { get; set; } = double.NaN;
        public double ScoreMargin { get; set; } = double.NaN;
        public double PoseScoreMax { get; set; } = double.NaN;
        public double AngleDelta { get; set; } = double.NaN;
        public double ScaleRatio { get; set; } = double.NaN;
        public double ValidPixelRatio { get; set; } = double.NaN;
        public double DistanceCount { get; set; } = double.NaN;
        public double DistancePxMin { get; set; } = double.NaN;
        public double DistancePxMax { get; set; } = double.NaN;
        public double DistancePxAverage { get; set; } = double.NaN;
        public double DistancePxRange { get; set; } = double.NaN;
        public int MeasurementLineCount { get; set; }
        public bool MeasurementLinesInsideRoi { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public string SourceCopyPath { get; set; } = string.Empty;
        public string RuntimeResultPath { get; set; } = string.Empty;
        public string Step1OverlayPath { get; set; } = string.Empty;
        public string Step2OverlayPath { get; set; } = string.Empty;
        public string Step3OverlayPath { get; set; } = string.Empty;
        public string Step4OverlayPath { get; set; } = string.Empty;
        public string PrimaryOverlayPath { get; set; } = string.Empty;
    }

    private sealed class C9CorpusRepresentative
    {
        public string Reason { get; set; } = string.Empty;
        public C9CorpusEvidenceRow Row { get; set; }
    }

    private sealed class C9CandidateAuditRow
    {
        public string RowId { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public bool Success { get; set; }
        public double ResultCount { get; set; }
        public double ScoreMax { get; set; }
        public double ScoreSecond { get; set; }
        public double ScoreMargin { get; set; }
        public string SourceSha256 { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string OverlayPath { get; set; } = string.Empty;
    }

    private sealed class C9GateEvidenceRow
    {
        public string RowId { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public bool ExpectedSuccess { get; set; }
        public bool ActualSuccess { get; set; }
        public int ExpectedFailedStep { get; set; }
        public int ActualFailedStep { get; set; }
        public string ErrorName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public double ScoreMax { get; set; }
        public double ScoreMargin { get; set; }
        public double AngleDelta { get; set; }
        public double ScaleRatio { get; set; }
        public double ValidPixelRatio { get; set; }
        public string SourceSha256 { get; set; } = string.Empty;
        public string PipelineSha256 { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string OverlayPath { get; set; } = string.Empty;
    }

    private static Mat CreateImage(int offsetX, int offsetY)
    {
        Mat image = new Mat(new Size(320, 220), MatType.CV_8UC1, Scalar.Black);
        Point markerCenter = new Point(60 + offsetX, 70 + offsetY);
        Cv2.Rectangle(
            image,
            new Rect(markerCenter.X - 12, markerCenter.Y - 12, 24, 24),
            Scalar.White,
            2);
        Cv2.Line(
            image,
            new Point(markerCenter.X - 9, markerCenter.Y),
            new Point(markerCenter.X + 9, markerCenter.Y),
            Scalar.White,
            2);
        Cv2.Line(
            image,
            new Point(markerCenter.X, markerCenter.Y - 9),
            new Point(markerCenter.X, markerCenter.Y + 9),
            Scalar.White,
            2);
        Cv2.Circle(image, markerCenter, 3, Scalar.Black, -1);

        Cv2.Rectangle(
            image,
            new Rect(182 + offsetX, 92 + offsetY, 24, 24),
            Scalar.White,
            -1);
        return image;
    }

    private static VisionPipeline CreatePipeline(string templatePath, bool useFixture)
    {
        VisionPipeline pipeline = new VisionPipeline
        {
            Name = useFixture ? "FixtureTranslation" : "NoFixtureTranslation"
        };

        VisionPipelineStep matching = new VisionPipelineStep
        {
            Name = "01 Locate Reference",
            ToolType = "Matching",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "FixtureMatch",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 1,
            MaxElapsedMilliseconds = 2000
        };
        Add(matching, "Name", "FixtureTranslationMatching");
        Add(matching, "TemplatePath", templatePath);
        Add(matching, "PATTERN_PATH", templatePath);
        Add(matching, "MATCH_MODE", "CCoeffNormed");
        Add(matching, "SCORE_MIN", "0.80");
        Add(matching, "MAGNIFIATION", "1");
        Add(matching, "NUM_MATCH", "1");
        Add(matching, "USE_FIND_ANGLE", "false");
        Add(matching, "USE_THRESHOLD", "false");
        Add(matching, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(matching, "USE_ROI", "false");
        Add(matching, "USE_AS_FIXTURE_FRAME", useFixture ? "true" : "false");
        Add(matching, "FIXTURE_FRAME_NAME", "PartFrame");
        Add(matching, "FIXTURE_REFERENCE_X", "60");
        Add(matching, "FIXTURE_REFERENCE_Y", "70");
        Add(matching, "FIXTURE_REFERENCE_ANGLE", "0");
        Add(matching, "FIXTURE_MAX_ANGLE_DELTA", "1");
        pipeline.Steps.Add(matching);

        VisionPipelineStep blob = new VisionPipelineStep
        {
            Name = "02 Inspect Shifted Pad",
            ToolType = "Blob",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "PadBlob",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "ResultCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 1,
            UseAcceptanceMetricMaximum = true,
            AcceptanceMetricMaximum = 1,
            MaxElapsedMilliseconds = 1000
        };
        Add(blob, "Name", "FixtureTranslationBlob");
        Add(blob, "MIN_AREA", "300");
        Add(blob, "MAX_AREA", "900");
        Add(blob, "USE_THRESHOLD", "true");
        Add(blob, "THRESHOLD_TYPES", "Binary");
        Add(blob, "THRESHOLD", "128");
        Add(blob, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(blob, "USE_BITWISENOT", "false");
        Add(blob, "USE_ROI", "true");
        Add(blob, "USE_MULTI_ROI", "false");
        Add(blob, "USE_MASKING", "false");
        Add(blob, "CvROI", SavedRoi);
        Add(blob, "ALLOW_BRANCH_INPUT", "true");
        Add(blob, "USE_FIXTURE_FRAME", useFixture ? "true" : "false");
        Add(blob, "FIXTURE_FRAME_NAME", "PartFrame");
        pipeline.Steps.Add(blob);

        VisionPipelineStep merge = new VisionPipelineStep
        {
            Name = "03 Merge Fixture Review",
            ToolType = "OverlayMerge",
            Enabled = true,
            InputLayer = "Main",
            OutputLayer = "FixtureReview",
            UseAcceptance = true,
            ExpectedSuccess = true,
            AcceptanceMetricName = "MergeOverlayCount",
            UseAcceptanceMetricMinimum = true,
            AcceptanceMetricMinimum = 2,
            MaxElapsedMilliseconds = 1000
        };
        Add(merge, "SourceLayers", "FixtureMatch;PadBlob");
        Add(merge, "BurnIn", "true");
        Add(merge, "DrawLabels", "true");
        Add(merge, "AllowEmpty", "false");
        pipeline.Steps.Add(merge);

        return pipeline;
    }

    private static VisionPipeline CreateDuplicateFramePipeline(string templatePath)
    {
        VisionPipeline pipeline = CreatePipeline(templatePath, useFixture: true);
        VisionPipelineStep duplicate = CreatePipeline(templatePath, useFixture: true).Steps[0];
        duplicate.Name = "02 Duplicate Reference";
        duplicate.OutputLayer = "DuplicateFixtureMatch";
        pipeline.Steps.Insert(1, duplicate);
        pipeline.Steps[2].Name = "03 Inspect Shifted Pad";
        pipeline.Steps[3].Name = "04 Merge Fixture Review";
        return pipeline;
    }

    private static async Task<IReadOnlyList<string>> RunSimilarityNormalizationSmoke(
        string outputDirectory,
        VisionRecipeRunner runner)
    {
        const int width = 320;
        const int height = 240;
        const double referenceX = 110d;
        const double referenceY = 90d;
        string similarityDirectory = Path.Combine(outputDirectory, "similarity_normalization");
        Directory.CreateDirectory(similarityDirectory);

        string templatePath = Path.Combine(similarityDirectory, "reference_template.png");
        using Mat reference = CreateSimilarityReference(width, height);
        using (Mat template = new Mat(reference, new Rect(70, 60, 80, 60)).Clone())
        {
            Cv2.ImWrite(templatePath, template);
        }

        string referencePath = Path.Combine(similarityDirectory, "reference.png");
        Cv2.ImWrite(referencePath, reference);
        VisionPipeline pipeline = CreateSimilarityNormalizationPipeline(
            templatePath,
            width,
            height,
            referenceX,
            referenceY);
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(pipeline, new[] { "Main" });
        Assert(validation.Success, "NormalizeImage pipeline validation must pass. " + validation.FormatErrors());
        string pipelinePath = Path.Combine(similarityDirectory, "matching_normalize_image.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(pipelinePath, pipeline), "NormalizeImage pipeline XML save must succeed.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline persistedPipeline)
                && persistedPipeline?.Steps?.Count == 2
                && string.Equals(persistedPipeline.Steps[1].Parameters["FIXTURE_APPLY_MODE"], "NormalizeImage", StringComparison.Ordinal),
            "NormalizeImage pipeline XML round trip must preserve the fixture mode.");

        SimilarityCase[] cases =
        {
            new SimilarityCase("identity", 0d, 1d, referenceX, referenceY),
            new SimilarityCase("angle_min", -5d, 1d, 130d, 105d),
            new SimilarityCase("angle_max", 5d, 1d, 145d, 115d),
            new SimilarityCase("scale_min", 0d, 0.8d, 135d, 100d),
            new SimilarityCase("scale_max", 0d, 1.2d, 140d, 110d)
        };
        List<string> report = new List<string>
        {
            "SimilarityNormalize: PASS",
            "SimilarityPipelineXml: " + pipelinePath
        };

        foreach (SimilarityCase testCase in cases)
        {
            using Mat current = CreateSimilarityCurrent(reference, testCase, referenceX, referenceY);
            string currentPath = Path.Combine(similarityDirectory, testCase.Name + "_source.png");
            Cv2.ImWrite(currentPath, current);
            using VisionRecipeRunResult result = await runner.RunAsync(pipelinePath, current);
            Assert(result.Success, "NormalizeImage case must pass: " + testCase.Name + ". " + result.Message);
            Assert(result.Steps.Count == 2, "NormalizeImage case must execute Matching and RotateScale: " + testCase.Name);
            AssertMetric(result.Steps[0], "FixtureAngleDelta", testCase.Angle, 0.55);
            AssertMetric(result.Steps[0], "FixtureScaleRatio", testCase.Scale, 0.051);
            AssertMetric(result.Steps[1], "FixtureNormalizedImageWidth", width, 0.1);
            AssertMetric(result.Steps[1], "FixtureNormalizedImageHeight", height, 0.1);
            Assert(GetMetric(result.Steps[1], "FixtureValidPixelRatio") >= 0.25d, "NormalizeImage valid-pixel ratio must pass: " + testCase.Name);
            VisionRecipeOverlaySummary validPixelBounds = result.Steps[1].Overlays.FirstOrDefault(overlay =>
                string.Equals(overlay.Kind, VisionToolOverlayKind.Rectangle.ToString(), StringComparison.Ordinal)
                && (overlay.Label?.StartsWith("Valid normalized pixels", StringComparison.Ordinal) ?? false));
            Assert(
                validPixelBounds != null
                    && validPixelBounds.BoundsWidth > 0d
                    && validPixelBounds.BoundsHeight > 0d
                    && validPixelBounds.BoundsX >= 0d
                    && validPixelBounds.BoundsY >= 0d
                    && validPixelBounds.BoundsX + validPixelBounds.BoundsWidth <= width
                    && validPixelBounds.BoundsY + validPixelBounds.BoundsHeight <= height,
                "NormalizeImage must publish a bounded valid-pixel rectangle without contour extraction: " + testCase.Name);

            string normalizedPath = Path.Combine(similarityDirectory, testCase.Name + "_normalized.png");
            Cv2.ImWrite(normalizedPath, result.ResultImage);
            double meanDifference = CalculateMeanAbsoluteDifference(
                reference,
                result.ResultImage,
                new Rect(50, 40, 210, 150));

            string matchingOverlayPath = Path.Combine(similarityDirectory, testCase.Name + "_matching_overlay.png");
            using (Bitmap matchingOverlay = new Bitmap(currentPath))
            {
                VisionPipelineRunReportImageRenderer.RenderInPlace(matchingOverlay, result.Steps[0], pipeline.Steps[0]);
                matchingOverlay.Save(matchingOverlayPath);
            }

            string normalizedOverlayPath = Path.Combine(similarityDirectory, testCase.Name + "_normalized_overlay.png");
            using (Bitmap normalizedOverlay = new Bitmap(normalizedPath))
            {
                VisionPipelineRunReportImageRenderer.RenderInPlace(normalizedOverlay, result.Steps[1], pipeline.Steps[1]);
                normalizedOverlay.Save(normalizedOverlayPath);
            }

            Assert(meanDifference <= 18d, $"NormalizeImage reviewed-region mean absolute difference is too large for {testCase.Name}: {meanDifference:0.###}.");

            report.Add(
                $"SimilarityCase {testCase.Name}: Angle={GetMetric(result.Steps[0], "FixtureAngleDelta"):0.###}, "
                + $"Scale={GetMetric(result.Steps[0], "FixtureScaleRatio"):0.###}, "
                + $"Valid={GetMetric(result.Steps[1], "FixtureValidPixelRatio"):0.###}, "
                + $"MeanAbsDiff={meanDifference:0.###}");
        }

        VisionPipeline missingDimensions = CreateSimilarityNormalizationPipeline(
            templatePath,
            width,
            height,
            referenceX,
            referenceY);
        missingDimensions.Steps[0].Parameters.Remove("FIXTURE_REFERENCE_IMAGE_WIDTH");
        missingDimensions.Steps[0].Parameters.Remove("FIXTURE_REFERENCE_IMAGE_HEIGHT");
        Assert(
            !VisionPipelineValidator.Validate(missingDimensions, new[] { "Main" }).Success,
            "NormalizeImage pipeline validation must reject missing taught dimensions.");
        using VisionRecipeRunResult missingDimensionsResult = await runner.RunAsync(missingDimensions, reference);
        AssertFixtureConfigurationFailure(missingDimensionsResult, "NormalizeImage without taught dimensions must fail closed.");

        VisionPipeline wrongDimensions = CreateSimilarityNormalizationPipeline(
            templatePath,
            width - 1,
            height,
            referenceX,
            referenceY);
        using VisionRecipeRunResult wrongDimensionsResult = await runner.RunAsync(wrongDimensions, reference);
        AssertFixtureConfigurationFailure(wrongDimensionsResult, "NormalizeImage with a source/reference size mismatch must fail closed.");

        VisionPipeline invalidCoverage = CreateSimilarityNormalizationPipeline(
            templatePath,
            width,
            height,
            referenceX,
            referenceY);
        invalidCoverage.Steps[1].Parameters["FIXTURE_MIN_VALID_PIXEL_RATIO"] = "1.1";
        Assert(
            !VisionPipelineValidator.Validate(invalidCoverage, new[] { "Main" }).Success,
            "NormalizeImage pipeline validation must reject an invalid valid-pixel gate.");
        using VisionRecipeRunResult invalidCoverageResult = await runner.RunAsync(invalidCoverage, reference);
        AssertFixtureConfigurationFailure(invalidCoverageResult, "NormalizeImage with an invalid valid-pixel gate must fail closed.");

        VisionPipeline fixedRotateScale = new VisionPipeline { Name = "FixedRotateScaleCompatibility" };
        VisionPipelineStep fixedStep = new VisionPipelineStep
        {
            Name = "01 Fixed RotateScale",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "FixedResult"
        };
        Add(fixedStep, "Angle", "0");
        Add(fixedStep, "ScaleXPercent", "100");
        Add(fixedStep, "ScaleYPercent", "100");
        Add(fixedStep, "Interpolation", "Linear");
        Add(fixedStep, "BorderType", "Constant");
        fixedRotateScale.Steps.Add(fixedStep);
        using VisionRecipeRunResult fixedResult = await runner.RunAsync(fixedRotateScale, reference);
        Assert(fixedResult.Success && fixedResult.ResultImageWidth == width && fixedResult.ResultImageHeight == height,
            "Existing fixed RotateScale behavior must remain available when fixture mode is off.");

        report.Add("FailClosedMissingDimensions: PASS");
        report.Add("FailClosedMismatchedDimensions: PASS");
        report.Add("FailClosedInvalidCoverageGate: PASS");
        report.Add("FixedRotateScaleCompatibility: PASS");
        return report;
    }

    private static Mat CreateSimilarityReference(int width, int height)
    {
        Mat image = new Mat(new Size(width, height), MatType.CV_8UC3, Scalar.White);
        Cv2.Rectangle(image, new Rect(70, 60, 80, 60), new Scalar(215, 215, 215), -1);
        Cv2.Rectangle(image, new Rect(74, 64, 72, 52), Scalar.Black, 3);
        Cv2.Line(image, new Point(82, 105), new Point(137, 70), Scalar.Black, 5);
        Cv2.Circle(image, new Point(91, 80), 8, new Scalar(30, 30, 30), -1);
        Cv2.Rectangle(image, new Rect(119, 90, 18, 17), new Scalar(80, 80, 80), -1);
        Cv2.PutText(image, "P", new Point(99, 108), HersheyFonts.HersheySimplex, 0.7, Scalar.Black, 2);
        Cv2.Rectangle(image, new Rect(205, 150, 45, 28), new Scalar(90, 90, 90), -1);
        Cv2.Line(image, new Point(30, 200), new Point(280, 200), new Scalar(175, 175, 175), 2);
        return image;
    }

    private static Mat CreateSimilarityCurrent(
        Mat reference,
        SimilarityCase testCase,
        double referenceX,
        double referenceY)
    {
        using Mat matrix = Cv2.GetRotationMatrix2D(
            new Point2f((float)referenceX, (float)referenceY),
            testCase.Angle,
            testCase.Scale);
        matrix.Set(0, 2, matrix.At<double>(0, 2) + testCase.CenterX - referenceX);
        matrix.Set(1, 2, matrix.At<double>(1, 2) + testCase.CenterY - referenceY);
        Mat current = new Mat();
        Cv2.WarpAffine(
            reference,
            current,
            matrix,
            reference.Size(),
            InterpolationFlags.Linear,
            BorderTypes.Constant,
            Scalar.White);
        return current;
    }

    private static VisionPipeline CreateSimilarityNormalizationPipeline(
        string templatePath,
        int referenceWidth,
        int referenceHeight,
        double referenceX,
        double referenceY)
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "MatchingSimilarityNormalizeImage" };
        VisionPipelineStep matching = new VisionPipelineStep
        {
            Name = "01 Locate Device Pose",
            ToolType = "Matching",
            InputLayer = "Main",
            OutputLayer = "PoseMatch"
        };
        Add(matching, "Name", "SimilarityPoseMatching");
        Add(matching, "TemplatePath", templatePath);
        Add(matching, "PATTERN_PATH", templatePath);
        Add(matching, "MATCH_MODE", "CCoeffNormed");
        Add(matching, "SCORE_MIN", "0.55");
        Add(matching, "MAGNIFIATION", "1");
        Add(matching, "NUM_MATCH", "1");
        Add(matching, "USE_FIND_ANGLE", "true");
        Add(matching, "FIND_ANGLE_MIN", "-5");
        Add(matching, "FIND_ANGLE_MAX", "5");
        Add(matching, "FIND_ANGLE", "0.5");
        Add(matching, "USE_FIND_SCALE", "true");
        Add(matching, "FIND_SCALE_MIN", "0.8");
        Add(matching, "FIND_SCALE_MAX", "1.2");
        Add(matching, "FIND_SCALE_STEP", "0.1");
        Add(matching, "USE_THRESHOLD", "false");
        Add(matching, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(matching, "USE_ROI", "false");
        Add(matching, "USE_AS_FIXTURE_FRAME", "true");
        Add(matching, "FIXTURE_FRAME_NAME", "DeviceFrame");
        Add(matching, "FIXTURE_REFERENCE_X", referenceX.ToString(CultureInfo.InvariantCulture));
        Add(matching, "FIXTURE_REFERENCE_Y", referenceY.ToString(CultureInfo.InvariantCulture));
        Add(matching, "FIXTURE_REFERENCE_ANGLE", "0");
        Add(matching, "FIXTURE_REFERENCE_SCALE", "1");
        Add(matching, "FIXTURE_MAX_ANGLE_DELTA", "5.25");
        Add(matching, "FIXTURE_REFERENCE_IMAGE_WIDTH", referenceWidth.ToString(CultureInfo.InvariantCulture));
        Add(matching, "FIXTURE_REFERENCE_IMAGE_HEIGHT", referenceHeight.ToString(CultureInfo.InvariantCulture));
        pipeline.Steps.Add(matching);

        VisionPipelineStep normalize = new VisionPipelineStep
        {
            Name = "02 Normalize Device Image",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "DeviceAligned"
        };
        Add(normalize, "Name", "DeviceNormalizeImage");
        Add(normalize, "Angle", "0");
        Add(normalize, "ScaleXPercent", "100");
        Add(normalize, "ScaleYPercent", "100");
        Add(normalize, "Interpolation", "Linear");
        Add(normalize, "BorderType", "Constant");
        Add(normalize, "USE_FIXTURE_FRAME", "true");
        Add(normalize, "FIXTURE_FRAME_NAME", "DeviceFrame");
        Add(normalize, "FIXTURE_APPLY_MODE", "NormalizeImage");
        Add(normalize, "FIXTURE_MIN_VALID_PIXEL_RATIO", "0.25");
        Add(normalize, "ALLOW_BRANCH_INPUT", "true");
        pipeline.Steps.Add(normalize);
        return pipeline;
    }

    private static async Task<int> RunCvr09LineFixture(string outputDirectory)
    {
        Assert(
            !Directory.Exists(outputDirectory) || !Directory.EnumerateFileSystemEntries(outputDirectory).Any(),
            "CVR-09 output must be new or empty: " + outputDirectory);
        Directory.CreateDirectory(outputDirectory);
        string casesDirectory = Path.Combine(outputDirectory, "cases");
        Directory.CreateDirectory(casesDirectory);

        VisionPipeline pipeline = CreateCvr09LineFixturePipeline();
        VisionPipelineValidationResult validation = VisionPipelineValidator.Validate(
            pipeline,
            new[] { "Main" });
        Assert(validation.Success, "CVR-09 pipeline must validate: " + validation.FormatErrors());

        string pipelinePath = Path.Combine(outputDirectory, "cvr09_line_fixture.pipeline.xml");
        Assert(SerializeHelper.SaveXmlFile(pipelinePath, pipeline), "CVR-09 pipeline XML save must succeed.");
        Assert(
            SerializeHelper.TryLoadFromXmlFile(pipelinePath, out VisionPipeline reloaded)
                && reloaded != null
                && reloaded.Steps.Count == pipeline.Steps.Count,
            "CVR-09 pipeline XML must round-trip.");

        object property = VisionPipelineStepPropertyMapper.CreateProperty(
            reloaded.Steps[2],
            new VisionPipelinePropertyContext(reloaded, 2));
        Assert(property != null, "LineFixture PropertyGrid mapping must exist.");
        VisionPipelineStep mapped = new VisionPipelineStep
        {
            Name = reloaded.Steps[2].Name,
            ToolType = reloaded.Steps[2].ToolType,
            InputLayer = reloaded.Steps[2].InputLayer,
            OutputLayer = reloaded.Steps[2].OutputLayer
        };
        Assert(
            VisionPipelineStepPropertyMapper.ApplyProperty(mapped, property)
                && string.Equals(mapped.ToolType, "LineFixture", StringComparison.Ordinal)
                && string.Equals(mapped.Parameters.GetValueOrDefault("SourceStepA"), "01 Top Datum", StringComparison.Ordinal)
                && string.Equals(mapped.Parameters.GetValueOrDefault("SourceStepB"), "02 Left Datum", StringComparison.Ordinal)
                && string.Equals(mapped.Parameters.GetValueOrDefault("USE_AS_FIXTURE_FRAME"), "True", StringComparison.OrdinalIgnoreCase),
            "LineFixture PropertyGrid must preserve typed sources and fixture publication.");

        (string Name, double Dx, double Dy, double Angle)[] cases =
        {
            ("reference", 0D, 0D, 0D),
            ("shift_right_down", 24D, 12D, 0D),
            ("shift_left_down", -18D, 15D, 0D),
            ("shift_right_up", 12D, -14D, 0D),
            ("rotate_positive", 8D, 8D, 3D),
            ("rotate_negative", -6D, 10D, -3D),
            ("rail_distractors_positive", 18D, -5D, 2D),
            ("rail_distractors_negative", -14D, -8D, -2D)
        };

        VisionRecipeRunner runner = new VisionRecipeRunner();
        List<string> rows = new List<string>
        {
            "Case\tSuccess\tOriginX\tOriginY\tAngleDeg\tIncludedDeg\tSupportA\tSupportB\tResidualA\tResidualB\tValidPixelRatio\tPadMean"
        };
        foreach ((string name, double dx, double dy, double angle) in cases)
        {
            using Mat image = CreateCvr09FixtureImage(dx, dy, angle, includeMarker: true);
            string sourcePath = Path.Combine(casesDirectory, name + "_source.png");
            Cv2.ImWrite(sourcePath, image);
            using VisionRecipeRunResult result = await runner.RunAsync(reloaded, image);
            string resultPath = Path.Combine(casesDirectory, name + "_result.png");
            if (result.ResultImage != null && !result.ResultImage.Empty())
            {
                Cv2.ImWrite(resultPath, result.ResultImage);
            }
            string diagnostic = result.Steps.Count >= 5
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    " | origin=({0:0.###},{1:0.###}) angle={2:0.###} mean={3:0.###}",
                    GetMetric(result.Steps[2], "FixtureCenterX"),
                    GetMetric(result.Steps[2], "FixtureCenterY"),
                    GetMetric(result.Steps[2], "FixtureAngleDelta"),
                    GetMetric(result.Steps[4], "MeanValueAvg"))
                : string.Empty;
            Assert(result.Success, "CVR-09 run must pass: " + name + " | " + result.SummaryText + diagnostic);
            Assert(result.Steps.Count == 5, "CVR-09 run must retain all five Steps.");
            RenderEvidence(
                sourcePath,
                result.Steps[0],
                reloaded.Steps[0],
                Path.Combine(casesDirectory, name + "_datum_a_overlay.png"));
            RenderEvidence(
                sourcePath,
                result.Steps[1],
                reloaded.Steps[1],
                Path.Combine(casesDirectory, name + "_datum_b_overlay.png"));
            RenderEvidence(
                sourcePath,
                result.Steps[2],
                reloaded.Steps[2],
                Path.Combine(casesDirectory, name + "_fixture_overlay.png"));

            VisionRecipeStepRunSummary fixture = result.Steps[2];
            VisionRecipeStepRunSummary normalize = result.Steps[3];
            VisionRecipeStepRunSummary padMean = result.Steps[4];
            AssertMetric(fixture, "FixtureCenterX", 90D + dx, 5D);
            AssertMetric(fixture, "FixtureCenterY", 70D + dy, 5D);
            AssertMetric(fixture, "FixtureAngleDelta", angle, 1.2D);
            Assert(GetMetric(fixture, "FixtureIncludedAngleDeg") >= 86D, "CVR-09 included angle must remain physical: " + name);
            Assert(GetMetric(fixture, "FixtureLineASupportCount") >= 20D, "CVR-09 datum A support must be retained: " + name);
            Assert(GetMetric(fixture, "FixtureLineBSupportCount") >= 20D, "CVR-09 datum B support must be retained: " + name);
            Assert(GetMetric(normalize, "FixtureValidPixelRatio") >= 0.70D, "CVR-09 normalization coverage must pass: " + name);
            Assert(GetMetric(padMean, "MeanValueAvg") >= 170D, "CVR-09 fixed relative pad ROI must remain on the bright marker: " + name);

            rows.Add(string.Join(
                "\t",
                name,
                result.Success,
                GetMetric(fixture, "FixtureCenterX").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureCenterY").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureAngleDelta").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureIncludedAngleDeg").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureLineASupportCount").ToString("0", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureLineBSupportCount").ToString("0", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureLineAFitResidualPx").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(fixture, "FixtureLineBFitResidualPx").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(normalize, "FixtureValidPixelRatio").ToString("0.###", CultureInfo.InvariantCulture),
                GetMetric(padMean, "MeanValueAvg").ToString("0.###", CultureInfo.InvariantCulture)));
        }

        VisionPipeline angleRejectPipeline = CreateCvr09LineFixturePipeline();
        angleRejectPipeline.Steps[2].Parameters["MIN_INCLUDED_ANGLE_DEG"] = "60";
        angleRejectPipeline.Steps[2].Parameters["MAX_INCLUDED_ANGLE_DEG"] = "80";
        using Mat rejectImage = CreateCvr09FixtureImage(0D, 0D, 0D, includeMarker: true);
        using VisionRecipeRunResult angleReject = await runner.RunAsync(angleRejectPipeline, rejectImage);
        Assert(!angleReject.Success, "CVR-09 impossible included-angle gate must fail closed.");
        Assert(
            angleReject.Steps.Count >= 3
                && (angleReject.Steps[2].Message?.Contains("included angle", StringComparison.OrdinalIgnoreCase) ?? false),
            "CVR-09 included-angle failure must preserve the exact reason.");

        VisionPipeline duplicateSourcePipeline = CreateCvr09LineFixturePipeline();
        duplicateSourcePipeline.Steps[2].Parameters["SourceStepB"] = "01 Top Datum";
        VisionPipelineValidationResult duplicateValidation = VisionPipelineValidator.Validate(
            duplicateSourcePipeline,
            new[] { "Main" });
        Assert(
            !duplicateValidation.Success
                && duplicateValidation.Errors.Any(error => error.Contains("distinct Segment", StringComparison.OrdinalIgnoreCase)),
            "CVR-09 duplicate typed source must fail definition validation.");

        File.WriteAllLines(Path.Combine(outputDirectory, "runtime_matrix.tsv"), rows, Encoding.UTF8);
        string report = string.Join(Environment.NewLine, new[]
        {
            "Status: Complete",
            "Scope: CVR-09 bounded synthetic Line -> LineFixture -> NormalizeImage -> fixed-ROI Mean workflow",
            "Cases: " + cases.Length,
            "RuntimePass: " + cases.Length + "/" + cases.Length,
            "FailClosedIncludedAngle: PASS",
            "FailClosedDuplicateSource: PASS",
            "PropertyGridRoundTrip: PASS",
            "PipelineXmlRoundTrip: PASS",
            "Boundary: synthetic two-datum pixel-space evidence; no scale, perspective, calibration, unseen-data, or field-qualification claim"
        });
        File.WriteAllText(Path.Combine(outputDirectory, "report.txt"), report, Encoding.UTF8);
        Console.WriteLine(report);
        return 0;
    }

    private static VisionPipeline CreateCvr09LineFixturePipeline()
    {
        VisionPipeline pipeline = new VisionPipeline { Name = "CVR09_LineFixture_Normalize_RelativeRoi" };
        pipeline.Steps.Add(CreateCvr09LineStep(
            "01 Top Datum",
            "TopDatumDrawing",
            "40,25,400,105",
            "Y_TTOB",
            "X_LTOR"));
        pipeline.Steps.Add(CreateCvr09LineStep(
            "02 Left Datum",
            "LeftDatumDrawing",
            "30,30,125,300",
            "X_LTOR",
            "Y_TTOB"));

        VisionPipelineStep fixture = new VisionPipelineStep
        {
            Name = "03 Publish Dual-Edge Fixture",
            ToolType = "LineFixture",
            InputLayer = "Main",
            OutputLayer = "FixtureReview"
        };
        Add(fixture, "SourceStepA", "01 Top Datum");
        Add(fixture, "SourceFeatureA", "Segment");
        Add(fixture, "SourceStepB", "02 Left Datum");
        Add(fixture, "SourceFeatureB", "Segment");
        Add(fixture, "MIN_SUPPORT_A", "20");
        Add(fixture, "MIN_SUPPORT_B", "20");
        Add(fixture, "MAX_FIT_RESIDUAL_A_PX", "2");
        Add(fixture, "MAX_FIT_RESIDUAL_B_PX", "2");
        Add(fixture, "MIN_INCLUDED_ANGLE_DEG", "85");
        Add(fixture, "MAX_INCLUDED_ANGLE_DEG", "90");
        Add(fixture, "MAX_EXTENSION_A_PX", "80");
        Add(fixture, "MAX_EXTENSION_B_PX", "80");
        Add(fixture, "USE_AS_FIXTURE_FRAME", "true");
        Add(fixture, "FIXTURE_FRAME_NAME", "OuterDatumFrame");
        Add(fixture, "FIXTURE_REFERENCE_X", "90");
        Add(fixture, "FIXTURE_REFERENCE_Y", "70");
        Add(fixture, "FIXTURE_REFERENCE_ANGLE", "0");
        Add(fixture, "FIXTURE_REFERENCE_SCALE", "1");
        Add(fixture, "FIXTURE_MAX_ANGLE_DELTA", "5");
        Add(fixture, "FIXTURE_MIN_SCALE_RATIO", "1");
        Add(fixture, "FIXTURE_MAX_SCALE_RATIO", "1");
        Add(fixture, "FIXTURE_REFERENCE_IMAGE_WIDTH", "480");
        Add(fixture, "FIXTURE_REFERENCE_IMAGE_HEIGHT", "360");
        Add(fixture, "ALLOW_BRANCH_INPUT", "true");
        fixture.UseAcceptance = true;
        fixture.AcceptanceMetricName = "ResultCount";
        fixture.UseAcceptanceMetricMinimum = true;
        fixture.AcceptanceMetricMinimum = 1D;
        fixture.UseAcceptanceMetricMaximum = true;
        fixture.AcceptanceMetricMaximum = 1D;
        pipeline.Steps.Add(fixture);

        VisionPipelineStep normalize = new VisionPipelineStep
        {
            Name = "04 Normalize From Dual-Edge Fixture",
            ToolType = "RotateScale",
            InputLayer = "Main",
            OutputLayer = "DatumAligned"
        };
        Add(normalize, "Angle", "0");
        Add(normalize, "ScaleXPercent", "100");
        Add(normalize, "ScaleYPercent", "100");
        Add(normalize, "Interpolation", "Linear");
        Add(normalize, "BorderType", "Constant");
        Add(normalize, "USE_FIXTURE_FRAME", "true");
        Add(normalize, "FIXTURE_FRAME_NAME", "OuterDatumFrame");
        Add(normalize, "FIXTURE_APPLY_MODE", "NormalizeImage");
        Add(normalize, "FIXTURE_MIN_VALID_PIXEL_RATIO", "0.70");
        Add(normalize, "ALLOW_BRANCH_INPUT", "true");
        pipeline.Steps.Add(normalize);

        VisionPipelineStep blob = new VisionPipelineStep
        {
            Name = "05 Inspect Fixed Relative Pad ROI",
            ToolType = "Mean",
            InputLayer = "DatumAligned",
            OutputLayer = "PadMean"
        };
        Add(blob, "MEAN_TYPES", "Mean");
        Add(blob, "MEAN_MIN", "170");
        Add(blob, "MEAN_MAX", "255");
        Add(blob, "USE_ROI", "true");
        Add(blob, "USE_MULTI_ROI", "false");
        Add(blob, "USE_MASKING", "false");
        Add(blob, "CvROI", "258,188,34,29");
        blob.UseAcceptance = true;
        blob.AcceptanceMetricName = "MeanValueAvg";
        blob.UseAcceptanceMetricMinimum = true;
        blob.AcceptanceMetricMinimum = 170D;
        blob.UseAcceptanceMetricMaximum = true;
        blob.AcceptanceMetricMaximum = 255D;
        pipeline.Steps.Add(blob);
        return pipeline;
    }

    private static VisionPipelineStep CreateCvr09LineStep(
        string name,
        string outputLayer,
        string roi,
        string projectionDirection,
        string verificationDirection)
    {
        VisionPipelineStep step = new VisionPipelineStep
        {
            Name = name,
            ToolType = "LineGauge",
            InputLayer = "Main",
            OutputLayer = outputLayer
        };
        Add(step, "USE_THRESHOLD", "false");
        Add(step, "USE_ADAPTIVE_THRESHOLD", "false");
        Add(step, "USE_BITWISENOT", "false");
        Add(step, "USE_ROI", "true");
        Add(step, "CvROI", roi);
        Add(step, "PRJ_PORALITY", "WTOB");
        Add(step, "PRJ_DIR", projectionDirection);
        Add(step, "VER_PRJ_DIR", verificationDirection);
        Add(step, "CONTRAST", "30");
        Add(step, "THICKNESS", "2");
        Add(step, "SAMPLING_STEP", "6");
        Add(step, "POINT_RANGE", "12");
        Add(step, "USE_MANUAL_ANGLE", "false");
        Add(step, "USE_EXTEND_FIT_LINE", "true");
        Add(step, "EXTEND_FIT_LINE_VALUE", "30");
        Add(step, "SHOW_VERTICAL_LINE", "true");
        Add(step, "SHOW_EDGE", "true");
        Add(step, "SHOW_CONTOUR", "true");
        Add(step, "SHOW_FITLINE", "true");
        Add(step, "ALLOW_BRANCH_INPUT", "true");
        step.UseAcceptance = true;
        step.AcceptanceMetricName = "ResultCount";
        step.UseAcceptanceMetricMinimum = true;
        step.AcceptanceMetricMinimum = 1D;
        step.UseAcceptanceMetricMaximum = true;
        step.AcceptanceMetricMaximum = 1D;
        return step;
    }

    private static Mat CreateCvr09FixtureImage(
        double dx,
        double dy,
        double angle,
        bool includeMarker)
    {
        const int width = 480;
        const int height = 360;
        using Mat reference = new Mat(height, width, MatType.CV_8UC3, new Scalar(238, 238, 238));
        Cv2.Rectangle(reference, new Rect(90, 70, 300, 220), new Scalar(72, 72, 72), -1);
        Cv2.Line(reference, new Point(125, 130), new Point(365, 130), new Scalar(150, 150, 150), 4);
        Cv2.Line(reference, new Point(110, 172), new Point(370, 172), new Scalar(185, 185, 185), 3);
        Cv2.Line(reference, new Point(170, 92), new Point(170, 265), new Scalar(145, 145, 145), 4);
        Cv2.Line(reference, new Point(215, 88), new Point(215, 270), new Scalar(115, 115, 115), 2);
        if (includeMarker)
        {
            Cv2.Rectangle(reference, new Rect(260, 190, 30, 25), new Scalar(230, 230, 230), -1);
        }

        using Mat transform = Cv2.GetRotationMatrix2D(new Point2f(90F, 70F), angle, 1D);
        transform.Set(0, 2, transform.At<double>(0, 2) + dx);
        transform.Set(1, 2, transform.At<double>(1, 2) + dy);
        Mat current = new Mat();
        Cv2.WarpAffine(
            reference,
            current,
            transform,
            reference.Size(),
            InterpolationFlags.Linear,
            BorderTypes.Constant,
            new Scalar(238, 238, 238));
        return current;
    }

    private static double CalculateMeanAbsoluteDifference(Mat expected, Mat actual, Rect reviewedRegion)
    {
        Assert(actual != null && !actual.Empty(), "NormalizeImage result image is required for comparison.");
        Assert(expected.Size() == actual.Size() && expected.Type() == actual.Type(), "NormalizeImage result type/size must match the reference.");
        Assert(
            reviewedRegion.X >= 0
                && reviewedRegion.Y >= 0
                && reviewedRegion.Right <= expected.Width
                && reviewedRegion.Bottom <= expected.Height,
            "NormalizeImage reviewed comparison region must be inside the reference image.");
        using Mat expectedRegion = new Mat(expected, reviewedRegion);
        using Mat actualRegion = new Mat(actual, reviewedRegion);
        using Mat difference = new Mat();
        Cv2.Absdiff(expectedRegion, actualRegion, difference);
        Scalar mean = Cv2.Mean(difference);
        return (mean.Val0 + mean.Val1 + mean.Val2) / 3d;
    }

    private sealed class SimilarityCase
    {
        public SimilarityCase(string name, double angle, double scale, double centerX, double centerY)
        {
            Name = name;
            Angle = angle;
            Scale = scale;
            CenterX = centerX;
            CenterY = centerY;
        }

        public string Name { get; }
        public double Angle { get; }
        public double Scale { get; }
        public double CenterX { get; }
        public double CenterY { get; }
    }

    private static void Add(VisionPipelineStep step, string key, string value)
    {
        step.Parameters[key] = value;
    }

    private static void AssertMetric(
        VisionRecipeStepRunSummary step,
        string metricName,
        double expected,
        double tolerance)
    {
        double actual = GetMetric(step, metricName);
        Assert(
            Math.Abs(actual - expected) <= tolerance,
            $"{metricName} expected {expected:0.###} +/- {tolerance:0.###}, actual {actual:0.###}.");
    }

    private static double GetMetric(VisionRecipeStepRunSummary step, string metricName)
    {
        if (step?.Metrics == null || !step.Metrics.TryGetValue(metricName, out double value))
        {
            string available = step?.Metrics == null
                ? "none"
                : string.Join(", ", step.Metrics.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase));
            throw new InvalidOperationException($"Metric '{metricName}' is missing. Available: {available}");
        }

        return value;
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void AssertFixtureConfigurationFailure(VisionRecipeRunResult result, string message)
    {
        Assert(!result.Success, message);
        Assert(
            string.Equals(result.FirstFailedErrorName, "InvalidParameter", StringComparison.Ordinal)
                || string.Equals(result.FirstFailedErrorName, "InvalidRoi", StringComparison.Ordinal),
            message + " Error=" + result.FirstFailedErrorName);
    }
}
