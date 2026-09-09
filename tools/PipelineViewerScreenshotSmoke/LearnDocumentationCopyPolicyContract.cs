using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class LearnDocumentationCopyPolicyContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "ovl23-learn-document-copy-policy-contract-" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        if (!string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Learn documentation contract evidence must be written under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        string documentDirectory = Path.Combine(evidenceDirectory, "documents");
        Directory.CreateDirectory(documentDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string programPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "Program.cs");
        string policyPath = Path.Combine(repositoryRoot, "tools", "PipelineViewerScreenshotSmoke", "LearnDocumentationCopyPolicy.cs");
        string program = File.ReadAllText(programPath);
        string policy = File.ReadAllText(policyPath);

        Check(
            "Program delegates Learn document and visible copy checks to the policy owner",
            program.Contains("LearnDocumentationCopyPolicy.AssertDocumentHasNoInternalContractCopy(", StringComparison.Ordinal)
                && program.Contains("LearnDocumentationCopyPolicy.AssertVisibleCopyHasNoInternalContractCopy(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Program no longer owns the forbidden phrase list or assertion methods",
            !program.Contains("InternalLearnContractPhrases", StringComparison.Ordinal)
                && !program.Contains("private static void AssertLearnDocumentHasNoInternalContractCopy(", StringComparison.Ordinal)
                && !program.Contains("private static void AssertNoInternalLearnContractCopy(", StringComparison.Ordinal),
            passed,
            failed);
        Check(
            "Policy owner keeps the complete phrase matching rule",
            policy.Contains("private static readonly string[] ForbiddenPhrases", StringComparison.Ordinal)
                && policy.Contains("StringComparison.OrdinalIgnoreCase", StringComparison.Ordinal)
                && policy.Contains("File.ReadAllText(documentPath)", StringComparison.Ordinal),
            passed,
            failed);

        string cleanDocumentPath = Path.Combine(documentDirectory, "clean.md");
        string forbiddenDocumentPath = Path.Combine(documentDirectory, "forbidden.md");
        File.WriteAllText(cleanDocumentPath, "Threshold separates bright and dark pixels.");
        File.WriteAllText(forbiddenDocumentPath, "This guide must not expose an internal execution detail.");
        bool cleanAccepted = true;
        try
        {
            LearnDocumentationCopyPolicy.AssertDocumentHasNoInternalContractCopy(cleanDocumentPath, "clean.md");
        }
        catch
        {
            cleanAccepted = false;
        }

        bool forbiddenRejected = false;
        try
        {
            LearnDocumentationCopyPolicy.AssertDocumentHasNoInternalContractCopy(forbiddenDocumentPath, "forbidden.md");
        }
        catch (InvalidOperationException ex)
        {
            forbiddenRejected = ex.Message.Contains("must not", StringComparison.OrdinalIgnoreCase)
                && ex.Message.Contains("forbidden.md", StringComparison.Ordinal);
        }

        Check("clean document copy remains accepted", cleanAccepted, passed, failed);
        Check("internal document copy is rejected with context", forbiddenRejected, passed, failed);

        bool visibleCopyRejected = false;
        try
        {
            LearnDocumentationCopyPolicy.AssertVisibleCopyHasNoInternalContractCopy(
                new[] { "Learner-facing explanation", "Execution evidence" },
                "visible curriculum");
        }
        catch (InvalidOperationException ex)
        {
            visibleCopyRejected = ex.Message.Contains("execution evidence", StringComparison.OrdinalIgnoreCase)
                && ex.Message.Contains("visible curriculum", StringComparison.Ordinal);
        }

        LearnDocumentationCopyPolicy.AssertVisibleCopyHasNoInternalContractCopy(
            new[] { "Learner-facing explanation", "" },
            "clean visible curriculum");
        Check("visible copy policy rejects internal evidence language", visibleCopyRejected, passed, failed);
        Check(
            "policy matching is case insensitive and returns the first configured phrase",
            string.Equals(
                LearnDocumentationCopyPolicy.FindForbiddenPhrase("A RUNTIME CONTRACT belongs in tests."),
                "runtime contract",
                StringComparison.Ordinal)
                && LearnDocumentationCopyPolicy.FindForbiddenPhrase("operator explanation") == null,
            passed,
            failed);

        List<string> report = new List<string>
        {
            "Status: " + (failed.Count == 0 ? "PASS" : "FAIL"),
            "ChecksPassed: " + passed.Count,
            "ChecksFailed: " + failed.Count
        };
        report.AddRange(passed.Select(item => "PASS: " + item));
        report.AddRange(failed.Select(item => "FAIL: " + item));
        string reportPath = Path.Combine(evidenceDirectory, "learn-document-copy-policy-contract.txt");
        File.WriteAllLines(reportPath, report);
        if (failed.Count != 0)
        {
            Console.Error.WriteLine("LEARN_DOCUMENT_COPY_POLICY_CONTRACT=FAIL|report=" + reportPath);
            return 1;
        }

        Console.WriteLine("LEARN_DOCUMENT_COPY_POLICY_CONTRACT=PASS|checks=" + passed.Count + "|report=" + reportPath);
        return 0;
    }

    private static void Check(string name, bool condition, ICollection<string> passed, ICollection<string> failed)
    {
        if (condition)
        {
            passed.Add(name);
        }
        else
        {
            failed.Add(name);
        }
    }

    private static string ResolveRepositoryRoot()
    {
        foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? current = new DirectoryInfo(start);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
