using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using OpenCvSharp;
using OpenVisionLab.Common;

internal static class MatViewOwnershipContract
{
    public static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "2d028-mat-view-ownership-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        RunCase("roi-and-padded-stride-pixels", CheckRoiAndPaddedStridePixels, observations, failures);
        RunCase("caller-and-output-ownership", CheckCallerAndOutputOwnership, observations, failures);
        RunCase("disposed-input-rejection", CheckDisposedInputRejection, observations, failures);

        string reportPath = Path.Combine(evidenceDirectory, "mat_view_ownership_contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-028 non-contiguous Mat/ROI, padded stride, disposed input, and ownership safety",
                "EvidenceDirectory: " + evidenceDirectory,
                "Owner: src/OpenVisionLab/Common/Imaging/BitmapImageConverter.cs",
                "Scope: focused contract only; no zero-copy or production converter rewrite"
            });
        File.AppendAllLines(reportPath, observations);
        File.AppendAllLines(reportPath, failures.ConvertAll(item => "Failure: " + item));

        if (failures.Count == 0)
        {
            Console.WriteLine("Mat view ownership contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Mat view ownership contract failed.");
        foreach (string failure in failures)
            Console.Error.WriteLine("- " + failure);
        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void CheckRoiAndPaddedStridePixels()
    {
        const int width = 5;
        const int height = 3;
        Rect roi = new Rect(1, 1, width, height);
        using Mat expected = CreatePatternMat(width, height);
        Bitmap? converted = null;

        try
        {
            using (Mat parent = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5)))
            {
                using Mat sourceView = new Mat(parent, roi);
                Require(sourceView.Step() > width * 3, "ROI source fixture did not have a padded row stride.");
                expected.CopyTo(sourceView);
                converted = BitmapImageConverter.ToBitmap(sourceView);
                AssertMatGuard(parent, roi, 0xA5);
            }

            using Mat restoredAfterSourceRelease = BitmapImageConverter.ToMat(converted);
            VerifyMatEqual(expected, restoredAfterSourceRelease, "Bitmap output after ROI source release");
        }
        finally
        {
            converted?.Dispose();
        }

        using Bitmap sourceBitmap = BitmapImageConverter.ToBitmap(expected);
        using Mat destinationParent = new Mat(height + 2, width + 2, MatType.CV_8UC3, Scalar.All(0xA5));
        using Mat destinationView = new Mat(destinationParent, roi);
        Require(destinationView.Step() > width * 3, "ROI destination fixture did not have a padded row stride.");
        BitmapImageConverter.ToMat(sourceBitmap, destinationView);
        VerifyMatEqual(expected, destinationView, "Mat destination ROI pixels");
        AssertMatGuard(destinationParent, roi, 0xA5);
    }

    private static void CheckCallerAndOutputOwnership()
    {
        const int width = 7;
        const int height = 3;
        using Mat source = CreatePatternMat(width, height);
        using Mat sourceBefore = source.Clone();
        using Bitmap bitmap = BitmapImageConverter.ToBitmap(source);
        VerifyMatEqual(sourceBefore, source, "ToBitmap mutated caller Mat");

        Mat? ownedMat = null;
        try
        {
            ownedMat = BitmapImageConverter.ToMat(bitmap);
            bitmap.Dispose();
            VerifyMatEqual(sourceBefore, ownedMat, "ToMat output changed after caller Bitmap release");
        }
        finally
        {
            ownedMat?.Dispose();
        }
    }

    private static void CheckDisposedInputRejection()
    {
        Bitmap disposedBitmap = new Bitmap(3, 2, PixelFormat.Format24bppRgb);
        disposedBitmap.Dispose();
        ExpectDisposedOrInvalid(
            () => BitmapImageConverter.ToMat(disposedBitmap),
            "disposed Bitmap input");

        Mat disposedMat = new Mat(2, 3, MatType.CV_8UC1, Scalar.All(1));
        disposedMat.Dispose();
        ExpectDisposedOrInvalid(
            () => BitmapImageConverter.ToBitmap(disposedMat),
            "disposed Mat input");
    }

    private static Mat CreatePatternMat(int width, int height)
    {
        Mat mat = new Mat(height, width, MatType.CV_8UC3);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                mat.Set(y, x, new Vec3b((byte)(x + 1), (byte)(y + 11), (byte)(x + y + 21)));
        }

        return mat;
    }

    private static void AssertMatGuard(Mat parent, Rect roi, byte expected)
    {
        for (int y = 0; y < parent.Height; y++)
        {
            for (int x = 0; x < parent.Width; x++)
            {
                if (x >= roi.X && x < roi.X + roi.Width && y >= roi.Y && y < roi.Y + roi.Height)
                    continue;

                Vec3b actual = parent.At<Vec3b>(y, x);
                Require(
                    actual == new Vec3b(expected, expected, expected),
                    $"ROI guard changed at ({x},{y}).");
            }
        }
    }

    private static void VerifyMatEqual(Mat expected, Mat actual, string description)
    {
        Require(
            expected.Size() == actual.Size() && expected.Type() == actual.Type(),
            description + " changed dimensions or channels.");
        for (int y = 0; y < expected.Height; y++)
        {
            for (int x = 0; x < expected.Width; x++)
            {
                Require(
                    expected.At<Vec3b>(y, x) == actual.At<Vec3b>(y, x),
                    $"{description} changed pixel at ({x},{y}).");
            }
        }
    }

    private static void ExpectDisposedOrInvalid(Action action, string description)
    {
        try
        {
            action();
        }
        catch (ObjectDisposedException)
        {
            return;
        }
        catch (ArgumentException)
        {
            return;
        }
        catch (NotSupportedException)
        {
            return;
        }

        throw new InvalidOperationException(description + " was accepted instead of being rejected.");
    }

    private static void RunCase(
        string name,
        Action check,
        ICollection<string> observations,
        ICollection<string> failures)
    {
        try
        {
            check();
            observations.Add(name + ": PASS");
        }
        catch (Exception exception)
        {
            observations.Add(name + ": FAIL " + exception.GetType().Name);
            failures.Add(name + ": " + exception.Message);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
