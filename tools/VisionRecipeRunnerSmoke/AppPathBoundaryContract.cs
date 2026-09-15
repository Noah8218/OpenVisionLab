using System;
using System.IO;

namespace OpenVisionLab.Smoke;

internal static class AppPathBoundaryContract
{
    public static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(
            requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "apppath-boundary-contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        string dataRoot = Path.Combine(evidenceDirectory, "data");
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        int passed = 0;
        int failed = 0;
        try
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                dataRoot);

            string safePath = AppPathService.Combine("CONFIG", "UI");
            Check(
                IsContained(dataRoot, safePath),
                "Valid runtime path remains under DataRoot",
                ref passed,
                ref failed);

            string safeInstallationPath = AppPathService.CombineInstallation("assets", "tool.dll");
            Check(
                IsContained(AppPathService.InstallationRootDirectory, safeInstallationPath),
                "Valid installation path remains under InstallationRoot",
                ref passed,
                ref failed);

            CheckThrows(
                () => AppPathService.Combine("..", "escaped"),
                "Traversal runtime path is rejected",
                ref passed,
                ref failed);
            CheckThrows(
                () => AppPathService.Combine("CONFIG", "..", "escaped"),
                "Nested traversal runtime path is rejected",
                ref passed,
                ref failed);
            CheckThrows(
                () => AppPathService.Combine(Path.Combine(evidenceDirectory, "outside")),
                "External rooted runtime path is rejected",
                ref passed,
                ref failed);
            CheckThrows(
                () => AppPathService.EnsureDirectory("..", "escaped-directory"),
                "Traversal directory creation is rejected",
                ref passed,
                ref failed);
            CheckThrows(
                () => AppPathService.GetCaptureFilePath("..\\escaped", DateTime.UtcNow),
                "Traversal capture title is rejected",
                ref passed,
                ref failed);
            CheckThrows(
                () => AppPathService.GetTestConfigPath("..\\escaped"),
                "Traversal test config name is rejected",
                ref passed,
                ref failed);

            string reportPath = Path.Combine(
                evidenceDirectory,
                "app-path-boundary-contract.txt");
            File.WriteAllLines(
                reportPath,
                new[]
                {
                    "APP_PATH_BOUNDARY_CONTRACT=" + (failed == 0 ? "PASS" : "FAIL"),
                    "Passed=" + passed,
                    "Failed=" + failed,
                    "DataRoot=" + dataRoot,
                    "SafePath=" + safePath,
                    "SafeInstallationPath=" + safeInstallationPath
                });
            Console.WriteLine(
                "APP_PATH_BOUNDARY_CONTRACT="
                + (failed == 0 ? "PASS" : "FAIL")
                + " Passed=" + passed
                + " Failed=" + failed);
            return failed == 0 ? 0 : 1;
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }
    }

    private static void Check(
        bool condition,
        string name,
        ref int passed,
        ref int failed)
    {
        if (condition)
        {
            passed++;
            return;
        }

        failed++;
        Console.Error.WriteLine("FAIL: " + name);
    }

    private static void CheckThrows(
        Action action,
        string name,
        ref int passed,
        ref int failed)
    {
        try
        {
            action();
            Check(false, name, ref passed, ref failed);
        }
        catch (InvalidOperationException)
        {
            Check(true, name, ref passed, ref failed);
        }
    }

    private static bool IsContained(string root, string path)
    {
        string fullRoot = Path.GetFullPath(root).TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);
        string fullPath = Path.GetFullPath(path).TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);
        return string.Equals(fullRoot, fullPath, StringComparison.OrdinalIgnoreCase)
            || fullPath.StartsWith(
                fullRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase);
    }
}
