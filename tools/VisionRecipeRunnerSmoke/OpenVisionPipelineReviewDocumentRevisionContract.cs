using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class OpenVisionPipelineReviewDocumentRevisionContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "refactor-ovl07-pipeline-review-document-revision-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> observations = new List<string>();
        List<string> failures = new List<string>();

        Check("Recipe invalidation rejects a captured run", () =>
        {
            using OpenVisionPipelineReviewDocumentRevisionGate gate = new OpenVisionPipelineReviewDocumentRevisionGate();
            gate.InvalidateRecipe();
            OpenVisionPipelineReviewDocumentRevision run = gate.BeginRun();
            Require(gate.IsCurrent(run), "A newly begun run was not current.");
            gate.InvalidateRecipe();
            Require(!gate.IsCurrent(run), "Recipe invalidation left an older run current.");
            observations.Add("recipe invalidation: captured run rejected");
        });

        Check("Input invalidation rejects a captured run while keeping the new revision current", () =>
        {
            using OpenVisionPipelineReviewDocumentRevisionGate gate = new OpenVisionPipelineReviewDocumentRevisionGate();
            gate.InvalidateRecipe();
            OpenVisionPipelineReviewDocumentRevision run = gate.BeginRun();
            OpenVisionPipelineReviewDocumentRevision current = gate.InvalidateInput();
            Require(!gate.IsCurrent(run), "Input invalidation left an older run current.");
            Require(gate.IsCurrentRevision(current.InputRevision, current.RecipeRevision), "The invalidated input revision was not current.");
            observations.Add("input invalidation: old run rejected and current document revision retained");
        });

        Check("A newer run generation rejects an older run with unchanged input and recipe revisions", () =>
        {
            using OpenVisionPipelineReviewDocumentRevisionGate gate = new OpenVisionPipelineReviewDocumentRevisionGate();
            gate.InvalidateRecipe();
            OpenVisionPipelineReviewDocumentRevision firstRun = gate.BeginRun();
            OpenVisionPipelineReviewDocumentRevision secondRun = gate.BeginRun();
            Require(!gate.IsCurrent(firstRun), "A previous run generation remained current.");
            Require(gate.IsCurrent(secondRun), "The latest run generation was not current.");
            observations.Add("run generation: previous completion cannot overwrite the latest run");
        });

        Check("Dispose invalidates all captured document revisions", () =>
        {
            OpenVisionPipelineReviewDocumentRevisionGate gate = new OpenVisionPipelineReviewDocumentRevisionGate();
            gate.InvalidateRecipe();
            OpenVisionPipelineReviewDocumentRevision run = gate.BeginRun();
            gate.Dispose();
            gate.Dispose();
            Require(!gate.IsCurrent(run), "Dispose left a captured run current.");
            Require(!gate.IsCurrentRevision(run.InputRevision, run.RecipeRevision), "Dispose left a document revision current.");
            observations.Add("dispose: captured run and document revision rejected; disposal is idempotent");
        });

        Check("Document and controller call paths use the revision boundary", () =>
        {
            string documentSource = ReadRepositoryFile(Path.Combine(
                "src",
                "OpenVisionLab",
                "UI",
                "Menu",
                "Wpf",
                "Documents",
                "OpenVisionPipelineReviewDocument.cs"));
            string eventSource = ReadRepositoryFile(Path.Combine(
                "src",
                "OpenVisionLab",
                "UI",
                "Menu",
                "Wpf",
                "PipelineReview",
                "Execution",
                "OpenVisionPipelineReviewExecutionResult.cs"));
            string controllerSource = ReadRepositoryFile(Path.Combine(
                "src",
                "OpenVisionLab",
                "UI",
                "Menu",
                "Wpf",
                "PipelineReview",
                "Execution",
                "OpenVisionPipelineReviewExecutionController.cs"));
            Require(documentSource.Contains("revisionGate.InvalidateRecipe()", StringComparison.Ordinal), "Recipe refresh does not use the document revision gate.");
            Require(documentSource.Contains("revisionGate.InvalidateInput()", StringComparison.Ordinal), "Input refresh does not use the document revision gate.");
            Require(documentSource.Contains("revisionGate.BeginRun()", StringComparison.Ordinal), "Run start does not capture a document revision.");
            Require(documentSource.Contains("ApplyReviewRunResult(runRevision, runResult)", StringComparison.Ordinal), "Completion does not carry the captured document revision.");
            Require(documentSource.Contains("revisionGate.IsCurrentRevision(e.InputRevision, e.RecipeRevision)", StringComparison.Ordinal), "StepUpdated projection does not validate event revisions.");
            Require(eventSource.Contains("public long InputRevision", StringComparison.Ordinal)
                && eventSource.Contains("public long RecipeRevision", StringComparison.Ordinal), "StepUpdated event arguments do not carry document revisions.");
            Require(controllerSource.Contains("stamp.InputRevision", StringComparison.Ordinal)
                && controllerSource.Contains("stamp.RecipeRevision", StringComparison.Ordinal), "Controller does not stamp StepUpdated events.");
            observations.Add("source path: document, event args, and controller use the revision gate contract");
        });

        string reportPath = Path.Combine(evidenceDirectory, "pipeline-review-document-revision-contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: OVL-07 Pipeline Review document result projection revision guard",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Pipeline Review document revision contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Pipeline Review document revision contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }
        Console.Error.WriteLine(reportPath);
        return 1;

        void Check(string name, Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                failures.Add(name + ": " + exception.GetBaseException().Message);
            }
        }
    }

    private static string ReadRepositoryFile(string relativePath)
    {
        foreach (string seed in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            DirectoryInfo? directory = new DirectoryInfo(Path.GetFullPath(seed));
            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return File.ReadAllText(candidate);
                }

                directory = directory.Parent;
            }
        }

        throw new FileNotFoundException("Repository source was not found for structural contract.", relativePath);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
