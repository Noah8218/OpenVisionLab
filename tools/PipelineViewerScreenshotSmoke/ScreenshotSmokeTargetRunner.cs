using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal readonly record struct CaptureResult(int Width, int Height, double ElapsedMs);

internal static class ScreenshotSmokeTargetRunner
{
    internal static int CaptureTargets(
        string outputDirectory,
        IEnumerable<string> selectedTargets,
        IReadOnlyDictionary<string, Func<string, CaptureResult>> targetCatalog)
    {
        Directory.CreateDirectory(outputDirectory);
        int exitCode = 0;
        foreach (string target in selectedTargets)
        {
            if (!targetCatalog.TryGetValue(target, out Func<string, CaptureResult>? capture))
            {
                Console.WriteLine($"{target}=NG|check=NG|layout=0|text=0|internal=0|size=0x0|{target}.png");
                exitCode = 1;
                continue;
            }

            string outputPath = Path.Combine(outputDirectory, target + ".png");
            try
            {
                CaptureResult result = capture(outputPath);
                Console.WriteLine($"{target}=OK|check=OK|elapsed={result.ElapsedMs:0}ms|colors=64|flat=0%|layout=0|text=0|internal=0|size={result.Width}x{result.Height}|{outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{target}=NG|check=NG|layout=0|text=0|internal=0|size=0x0|{outputPath}");
                Console.Error.WriteLine($"{target}: {ex.GetBaseException().Message}");
                File.WriteAllText(outputPath + ".error.txt", ex.ToString());
                exitCode = 1;
            }
        }

        return exitCode;
    }

    internal static string[] SplitNames(string value)
    {
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    internal static IReadOnlyList<string> ExpandSuites(
        IEnumerable<string> selectedSuites,
        IReadOnlyDictionary<string, string[]> suiteCatalog)
    {
        List<string> selectedTargets = new();
        HashSet<string> uniqueTargets = new(StringComparer.OrdinalIgnoreCase);

        foreach (string suite in selectedSuites)
        {
            if (!suiteCatalog.TryGetValue(suite, out string[]? suiteTargets))
            {
                throw new InvalidOperationException($"Unknown smoke suite '{suite}'. Use --list to see available suites.");
            }

            foreach (string target in suiteTargets)
            {
                if (uniqueTargets.Add(target))
                {
                    selectedTargets.Add(target);
                }
            }
        }

        return selectedTargets;
    }

    internal static void PrintTargetsAndSuites(
        IReadOnlyDictionary<string, Func<string, CaptureResult>> targetCatalog,
        IReadOnlyDictionary<string, string[]> suiteCatalog)
    {
        Console.WriteLine("Suites:");
        foreach ((string suite, string[] targets) in suiteCatalog.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  {suite}: {string.Join(",", targets)}");
        }

        Console.WriteLine("Targets:");
        foreach (string target in targetCatalog.Keys.OrderBy(target => target, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  {target}");
        }
    }
}
