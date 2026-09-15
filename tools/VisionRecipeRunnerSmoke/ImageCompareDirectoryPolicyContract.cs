using OpenVisionLab;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

internal static class ImageCompareDirectoryPolicyContract
{
    public static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "image_compare_directory_policy_contract_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        string dataRoot = Path.Combine(evidenceDirectory, "data-root");
        string imageDirectory = Path.Combine(evidenceDirectory, "images");
        string imagePath = Path.Combine(imageDirectory, "remembered.png");
        string missingPath = Path.Combine(imageDirectory, "missing.png");
        string? previousDataRoot = Environment.GetEnvironmentVariable("OPENVISIONLAB_DATA_ROOT");

        try
        {
            Directory.CreateDirectory(imageDirectory);
            Environment.SetEnvironmentVariable("OPENVISIONLAB_DATA_ROOT", dataRoot);
            CreateImage(imagePath);

            ImageCompareDirectoryPolicy policy = new ImageCompareDirectoryPolicy();
            policy.RememberImageDirectory(new[] { imagePath });
            string remembered = policy.ResolveInitialDirectory();
            Require(PathsEqual(remembered, imageDirectory), "Policy did not retain the selected image directory.");

            string persistedPath = Path.Combine(dataRoot, "CONFIG", "image_compare_last_directory.txt");
            Require(File.Exists(persistedPath), "Policy did not create the persisted directory file.");
            Require(PathsEqual(File.ReadAllText(persistedPath).Trim(), imageDirectory), "Persisted directory content changed.");

            ImageCompareDirectoryPolicy reopenedPolicy = new ImageCompareDirectoryPolicy();
            Require(PathsEqual(reopenedPolicy.ResolveInitialDirectory(), imageDirectory), "A new policy instance did not restore the persisted directory.");
            observations.Add("policy save/reopen: selected directory persisted and restored");

            ImageCompareViewModel viewModel = new ImageCompareViewModel();
            try
            {
                viewModel.LoadImages(imagePath);
                Require(PathsEqual(viewModel.InitialImageDirectory, imageDirectory), "ViewModel did not expose the policy-owned initial directory.");
                viewModel.LoadImages(missingPath);
                Require(PathsEqual(viewModel.InitialImageDirectory, imageDirectory), "Invalid image input changed the remembered directory.");
                observations.Add("ViewModel call path: LoadImages delegates directory memory without Window file I/O");
            }
            finally
            {
                viewModel.Dispose();
            }
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENVISIONLAB_DATA_ROOT", previousDataRoot);
        }

        string reportPath = Path.Combine(evidenceDirectory, "image_compare_directory_policy_contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: Image Compare directory policy ownership, persistence, and ViewModel call path",
                "EvidenceDirectory: " + evidenceDirectory,
                "PolicyOwner: ImageCompareDirectoryPolicy",
                "WindowRole: file dialog and pointer/window UI only"
            }
            .Concat(observations)
            .Concat(failures.Select(item => "Failure: " + item)));

        if (failures.Count == 0)
        {
            Console.WriteLine("Image Compare directory policy contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Image Compare directory policy contract failed.");
        foreach (string failure in failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void CreateImage(string path)
    {
        using Bitmap bitmap = new Bitmap(4, 3, PixelFormat.Format24bppRgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.DarkCyan);
        bitmap.Save(path, ImageFormat.Png);
    }

    private static bool PathsEqual(string first, string second)
    {
        return string.Equals(
            Path.GetFullPath(first ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            Path.GetFullPath(second ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
