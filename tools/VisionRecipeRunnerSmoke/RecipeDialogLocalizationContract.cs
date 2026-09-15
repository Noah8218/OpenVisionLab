using System;
using System.Collections.Generic;
using System.IO;

internal static class RecipeDialogLocalizationContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string repositoryRoot = ResolveRepositoryRoot();
        string sourcePath = Path.Combine(
            repositoryRoot,
            "src",
            "OpenVisionLab",
            "UI",
            "Menu",
            "Wpf",
            "Shell",
            "Recipe",
            "RecipeDialogAdapter.cs");
        string source = File.ReadAllText(sourcePath);
        List<string> results = new();

        Check(
            "Recipe and pipeline Korean confirmation text is readable",
            source.Contains("레시피 '{0}'을(를) 삭제하시겠습니까?", StringComparison.Ordinal)
                && source.Contains("레시피 '{0}'에서 파이프라인 '{1}'을(를) 삭제하시겠습니까?", StringComparison.Ordinal)
                && source.Contains("파이프라인 삭제", StringComparison.Ordinal),
            results);
        Check(
            "File dialog Korean text is readable",
            source.Contains("파이프라인 XML 또는 검토 번들 열기", StringComparison.Ordinal)
                && source.Contains("파이프라인 XML 내보내기", StringComparison.Ordinal)
                && source.Contains("레시피 검토 번들 내보내기", StringComparison.Ordinal),
            results);
        Check(
            "Validation set Korean text is readable",
            source.Contains("로컬 검증 세트에 ", StringComparison.Ordinal)
                && source.Contains("누락된 검증 이미지 교체: ", StringComparison.Ordinal)
                && source.Contains("원본 이미지는 삭제되지 않습니다.", StringComparison.Ordinal),
            results);
        Check(
            "Known mojibake markers are absent from the adapter",
            !source.Contains("?덉떆", StringComparison.Ordinal)
                && !source.Contains("?뚯씠", StringComparison.Ordinal)
                && !source.Contains("濡쒖뺄", StringComparison.Ordinal)
                && !source.Contains("寃利", StringComparison.Ordinal),
            results);
        Check(
            "Dialog delegate signatures remain unchanged",
            source.Contains("internal bool ConfirmDeleteRecipe(string recipeName)", StringComparison.Ordinal)
                && source.Contains("internal bool ConfirmDeletePipeline(string recipeName, string pipelineName)", StringComparison.Ordinal)
                && source.Contains("internal bool ConfirmDeleteValidationSet(string setName)", StringComparison.Ordinal),
            results);

        if (!string.IsNullOrWhiteSpace(requestedEvidenceDirectory))
        {
            string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory);
            Directory.CreateDirectory(evidenceDirectory);
            File.WriteAllLines(
                Path.Combine(evidenceDirectory, "recipe-dialog-localization-contract.txt"),
                results);
        }

        bool passed = results.TrueForAll(line => line.StartsWith("PASS: ", StringComparison.Ordinal));
        Console.WriteLine(
            "RECIPE_DIALOG_LOCALIZATION_CONTRACT="
            + (passed ? "PASS" : "FAIL")
            + "|checks="
            + results.Count);
        return passed ? 0 : 1;
    }

    private static void Check(string name, bool condition, ICollection<string> results)
    {
        results.Add((condition ? "PASS: " : "FAIL: ") + name);
    }

    private static string ResolveRepositoryRoot()
    {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "src", "OpenVisionLab", "OpenVisionLab.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("OpenVisionLab repository root was not found.");
    }
}
