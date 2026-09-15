using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class RecipeCommandSurfaceClipboardBoundaryContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "recipe-command-surface-clipboard-boundary_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        string copiedText = string.Empty;
        string pastedText = "<OpenVisionRecipeDraft />";

        try
        {
            Require(
                string.Equals(Path.GetPathRoot(evidenceDirectory), @"D:\", StringComparison.OrdinalIgnoreCase),
                "Clipboard boundary evidence must be on D:.");

            OpenVisionShellHostRecipeCommandSurface surface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => "ClipboardBoundaryRecipe",
                    _ => { },
                    () => { },
                    copyTextToClipboard: text => copiedText = text ?? string.Empty,
                    clipboardContainsText: () => !string.IsNullOrWhiteSpace(pastedText),
                    readClipboardText: () => pastedText);

            surface.LlmPromptText = "Prompt from the Recipe command surface.";
            Require(
                surface.CopyLlmPromptCommand.CanExecute(null),
                "Copy LLM prompt command was not executable after a prompt was supplied.");
            surface.CopyLlmPromptCommand.Execute(null);
            Require(
                string.Equals(copiedText, surface.LlmPromptText, StringComparison.Ordinal),
                "Copy command did not route its text through the injected Clipboard callback.");
            Require(
                surface.LlmPromptCopyStatusText.Contains("copied", StringComparison.OrdinalIgnoreCase)
                    || surface.LlmPromptCopyStatusText.Contains("복사", StringComparison.Ordinal),
                "Copy command did not preserve the success status projection.");
            passed.Add("copy command routes text through the Shell callback and preserves status");

            Require(
                surface.PasteLlmXmlDraftFromClipboardCommand.CanExecute(null),
                "Paste XML command was not executable.");
            surface.PasteLlmXmlDraftFromClipboardCommand.Execute(null);
            Require(
                string.Equals(surface.LlmXmlDraftText, pastedText, StringComparison.Ordinal),
                "Paste command did not route text through the injected Clipboard callbacks.");
            Require(
                surface.LlmXmlDraftPasteStatusText.Contains("Pasted", StringComparison.OrdinalIgnoreCase)
                    || surface.LlmXmlDraftPasteStatusText.Contains("붙여넣", StringComparison.Ordinal),
                "Paste command did not preserve the success status projection.");
            passed.Add("paste command routes text through the Shell callbacks and preserves XML state");

            OpenVisionShellHostRecipeCommandSurface failureSurface =
                new OpenVisionShellHostRecipeCommandSurface(
                    () => "ClipboardBoundaryFailureRecipe",
                    _ => { },
                    () => { },
                    copyTextToClipboard: _ => throw new InvalidOperationException("forced Clipboard failure"));
            failureSurface.LlmPromptText = "Prompt that fails to copy.";
            failureSurface.CopyLlmPromptCommand.Execute(null);
            Require(
                failureSurface.LlmPromptCopyStatusText.Contains("failed", StringComparison.OrdinalIgnoreCase)
                    || failureSurface.LlmPromptCopyStatusText.Contains("실패", StringComparison.Ordinal),
                "Clipboard callback failure was not converted to the existing status projection.");
            passed.Add("Clipboard callback failure preserves the existing error status path");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }

        string outputPath = Path.Combine(
            evidenceDirectory,
            "recipe-command-surface-clipboard-boundary-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: PL-0024 RecipeCommandSurface Clipboard boundary",
                "EvidenceDirectory: " + evidenceDirectory
            }
            .Concat(passed.Select(item => "PASS: " + item))
            .Concat(failed.Select(item => "FAIL: " + item)));

        foreach (string item in passed)
        {
            Console.WriteLine("PASS|" + item);
        }

        foreach (string item in failed)
        {
            Console.WriteLine("FAIL|" + item);
        }

        Console.WriteLine(
            "CONTRACT|recipe-command-surface-clipboard-boundary|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
