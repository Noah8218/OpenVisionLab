using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using OpenCvSharp;
using OpenVisionLab.Common;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab;

internal static class InputFormatMeaningContract
{
    public static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev",
                "2d029-input-format-meaning-" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);

        List<string> observations = new List<string>();
        List<string> failures = new List<string>();
        RunCase("runner-8bit-gray-bgr-bgra-meaning", CheckRunnerEightBitMeaning, observations, failures);
        RunCase("runner-16bit-explicit-rejection", () => CheckRunner16BitRejection(evidenceDirectory), observations, failures);
        RunCase("runner-damaged-decode-rejection", () => CheckRunnerDamagedDecodeRejection(evidenceDirectory), observations, failures);
        RunCase("display-only-16bit-and-alpha-mapping", () => CheckDisplayOnlyMapping(evidenceDirectory), observations, failures);

        string reportPath = Path.Combine(evidenceDirectory, "input_format_meaning_contract.txt");
        File.WriteAllLines(
            reportPath,
            new[]
            {
                "Result: " + (failures.Count == 0 ? "PASS" : "FAIL"),
                "Contract: 2D-029 Gray/BGR/BGRA/16-bit UI display versus Runner inspection meaning",
                "EvidenceDirectory: " + evidenceDirectory,
                "RunnerOwner: src/OpenVisionLab/Common/Imaging/BitmapImageConverter.cs",
                "DisplayOwner: src/Libraries/OpenVisionLab.ImageCanvas/Util/CanvasImageLoader.cs",
                "Boundary: display-only downscale/tone mapping is not inspection input normalization"
            });
        File.AppendAllLines(reportPath, observations);
        File.AppendAllLines(reportPath, failures.ConvertAll(item => "Failure: " + item));

        if (failures.Count == 0)
        {
            Console.WriteLine("Input format meaning contract passed.");
            Console.WriteLine(reportPath);
            return 0;
        }

        Console.Error.WriteLine("Input format meaning contract failed.");
        foreach (string failure in failures)
            Console.Error.WriteLine("- " + failure);
        Console.Error.WriteLine(reportPath);
        return 1;
    }

    private static void CheckRunnerEightBitMeaning()
    {
        using Mat gray = CreateGrayMat();
        using Bitmap grayBitmap = BitmapImageConverter.ToBitmap(gray);
        using Mat grayRoundTrip = BitmapImageConverter.ToMat(grayBitmap);
        VerifyMatEqual(gray, grayRoundTrip, "Runner Gray input meaning");

        using Mat bgr = CreateBgrMat();
        using Bitmap bgrBitmap = BitmapImageConverter.ToBitmap(bgr);
        using Mat bgrRoundTrip = BitmapImageConverter.ToMat(bgrBitmap);
        VerifyMatEqual(bgr, bgrRoundTrip, "Runner BGR input meaning");

        using Mat bgra = CreateBgraMat();
        using Bitmap bgraBitmap = BitmapImageConverter.ToBitmap(bgra);
        using Mat bgraRoundTrip = BitmapImageConverter.ToMat(bgraBitmap);
        VerifyMatEqual(bgra, bgraRoundTrip, "Runner BGRA input meaning");
    }

    private static void CheckRunner16BitRejection(string evidenceDirectory)
    {
        string sourcePath = Path.Combine(evidenceDirectory, "runner-16bit-gray.png");
        using (Mat gray16 = CreateGray16Mat())
        {
            Require(Cv2.ImWrite(sourcePath, gray16), "Could not write the isolated 16-bit fixture.");
        }

        VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.RunSampleCheckSafe(
            new VisionPipelineSampleCatalogItem
            {
                SampleName = "PL-0087 16-bit input",
                ImageFullPath = sourcePath
            },
            "<preflight-only />");
        Require(result.Status == "ERROR", "Runner 16-bit input did not fail before calculation.");
        Require(
            result.Message.Contains("depth", StringComparison.OrdinalIgnoreCase)
                && result.Message.Contains("8-bit", StringComparison.OrdinalIgnoreCase),
            "Runner 16-bit rejection did not identify the unsupported depth and accepted 8-bit formats.");
    }

    private static void CheckDisplayOnlyMapping(string evidenceDirectory)
    {
        string gray16Path = Path.Combine(evidenceDirectory, "display-16bit-gray.png");
        using (Mat gray16 = CreateGray16Mat())
        {
            Require(Cv2.ImWrite(gray16Path, gray16), "Could not write the display 16-bit fixture.");
        }

        using Mat displayGray = CanvasImageLoader.LoadMatFromFile(gray16Path);
        Require(displayGray.Type() == MatType.CV_8UC1, "Display 16-bit mapping did not produce CV_8UC1.");
        ushort[] expected16 = { 0, 257, 514, 1028, 32768, 65535 };
        for (int index = 0; index < expected16.Length; index++)
        {
            byte expected = (byte)(expected16[index] >> 8);
            Require(
                displayGray.At<byte>(index / 3, index % 3) == expected,
                $"Display 16-bit mapping changed sample {index}.");
        }

        string bgraPath = Path.Combine(evidenceDirectory, "display-bgra.png");
        using (Mat bgra = CreateBgraMat())
        {
            Require(Cv2.ImWrite(bgraPath, bgra), "Could not write the display BGRA fixture.");
        }

        using Mat displayBgr = CanvasImageLoader.LoadMatFromFile(bgraPath);
        Require(displayBgr.Type() == MatType.CV_8UC3, "Display BGRA mapping did not produce CV_8UC3.");
        using Mat expectedBgr = CreateBgrMat();
        VerifyMatEqual(expectedBgr, displayBgr, "Display BGRA alpha-drop mapping");
    }

    private static void CheckRunnerDamagedDecodeRejection(string evidenceDirectory)
    {
        string sourcePath = Path.Combine(evidenceDirectory, "runner-damaged-input.bin");
        File.WriteAllBytes(sourcePath, new byte[] { 0x4F, 0x56, 0x4C, 0x2D, 0x00, 0xFF, 0x13 });

        VisionPipelineSampleCheckResult result = VisionPipelineSampleCheckService.RunSampleCheckSafe(
            new VisionPipelineSampleCatalogItem
            {
                SampleName = "PL-0087 damaged input",
                ImageFullPath = sourcePath
            },
            "<preflight-only />");
        Require(result.Status == "ERROR", "Damaged Runner input did not fail before calculation.");
        Require(
            result.Message.Contains("could not be decoded", StringComparison.OrdinalIgnoreCase),
            "Damaged Runner input did not return a decode failure message.");
    }

    private static Mat CreateGrayMat()
    {
        Mat mat = new Mat(2, 3, MatType.CV_8UC1);
        byte[] values = { 0, 17, 128, 200, 254, 255 };
        for (int index = 0; index < values.Length; index++)
            mat.Set(index / 3, index % 3, values[index]);
        return mat;
    }

    private static Mat CreateGray16Mat()
    {
        Mat mat = new Mat(2, 3, MatType.CV_16UC1);
        ushort[] values = { 0, 257, 514, 1028, 32768, 65535 };
        for (int index = 0; index < values.Length; index++)
            mat.Set(index / 3, index % 3, values[index]);
        return mat;
    }

    private static Mat CreateBgrMat()
    {
        Mat mat = new Mat(2, 3, MatType.CV_8UC3);
        for (int y = 0; y < mat.Height; y++)
        {
            for (int x = 0; x < mat.Width; x++)
                mat.Set(y, x, new Vec3b((byte)(x + 1), (byte)(y + 11), (byte)(x + y + 21)));
        }
        return mat;
    }

    private static Mat CreateBgraMat()
    {
        Mat mat = new Mat(2, 3, MatType.CV_8UC4);
        for (int y = 0; y < mat.Height; y++)
        {
            for (int x = 0; x < mat.Width; x++)
                mat.Set(y, x, new Vec4b((byte)(x + 1), (byte)(y + 11), (byte)(x + y + 21), (byte)(100 + x + y)));
        }
        return mat;
    }

    private static void VerifyMatEqual(Mat expected, Mat actual, string description)
    {
        Require(
            expected.Size() == actual.Size() && expected.Type() == actual.Type(),
            description + " changed dimensions, depth, or channels.");
        for (int y = 0; y < expected.Height; y++)
        {
            for (int x = 0; x < expected.Width; x++)
            {
                if (expected.Channels() == 1)
                {
                    Require(
                        expected.At<byte>(y, x) == actual.At<byte>(y, x),
                        $"{description} changed pixel at ({x},{y}).");
                }
                else if (expected.Channels() == 3)
                {
                    Require(
                        expected.At<Vec3b>(y, x) == actual.At<Vec3b>(y, x),
                        $"{description} changed pixel at ({x},{y}).");
                }
                else
                {
                    Require(
                        expected.At<Vec4b>(y, x) == actual.At<Vec4b>(y, x),
                        $"{description} changed pixel at ({x},{y}).");
                }
            }
        }
    }

    private static void ExpectNotSupported(Action action, string description)
    {
        try
        {
            action();
        }
        catch (NotSupportedException exception)
        {
            Require(!string.IsNullOrWhiteSpace(exception.Message), description + " returned an empty rejection message.");
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
