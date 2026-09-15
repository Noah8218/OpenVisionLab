using OpenVisionLab;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class PinArrayGapValidationIdentityOwnerContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "ovl46-pinarraygap-validation-identity-owner-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        string? previousDataRoot = Environment.GetEnvironmentVariable(
            AppPathService.DataRootEnvironmentVariable);
        string recipeName = "Smoke_PinArrayGapIdentity_" + Guid.NewGuid().ToString("N")[..10];
        const string pipelineName = "Pin_Row_EdgeGap_Consistency";
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "PinArrayGap validation identity evidence must be on D:.");

            string dataRoot = Path.Combine(evidenceDirectory, "data");
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                dataRoot);

            string fixtureDirectory = Path.Combine(evidenceDirectory, "fixtures");
            Directory.CreateDirectory(fixtureDirectory);
            string trainImage = WriteFixture(fixtureDirectory, "train.bmp", "train-v1");
            string validationImage = WriteFixture(fixtureDirectory, "validation.bmp", "validation-v1");
            string testImage = WriteFixture(fixtureDirectory, "test.bmp", "test-v1");
            string validationDuplicateImage = WriteFixture(fixtureDirectory, "validation-duplicate-name.bmp", "train-v1");
            VisionPipeline pipeline = OpenVisionRecipePinArrayGapIntentSkill.CreateJudgedPipeline(
                new[] { new OpenVisionRecipePinGapIntentSkill.RoiSample(10, 10, 40, 40) },
                OpenVisionRecipePinArrayGapIntentSkill.DefaultDarkThreshold,
                OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumDarkCoverageRatio,
                OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumPinWidth,
                OpenVisionRecipePinArrayGapIntentSkill.DefaultMaximumPinBreakWidth,
                OpenVisionRecipePinArrayGapIntentSkill.DefaultMinimumGapWidth,
                maximumDistancePxRange: 6D);
            VisionPipelineStorage.Save(recipeName, pipeline);
            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeName, pipelineName);
            byte[] pipelineBefore = File.ReadAllBytes(pipelinePath);
            OpenVisionRecipeValidationSetOption train = CreateSet("Train", trainImage, "Train");
            OpenVisionRecipeValidationSetOption validation = CreateSet("Validation", validationImage, "Validation");
            OpenVisionRecipeValidationSetOption test = CreateSet("Test", testImage, "Test");
            OpenVisionRecipeValidationSetOption validationDuplicate = CreateSet(
                "ValidationDuplicate",
                validationDuplicateImage,
                "Validation duplicate bytes");
            OpenVisionRecipePinArrayGapValidationIdentityOwner owner =
                new OpenVisionRecipePinArrayGapValidationIdentityOwner();

            RunCase(
                "freeze rejects same image bytes under a different filename across split roles",
                () =>
                {
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Freeze(
                        recipeName,
                        pipelineName,
                        train,
                        validationDuplicate,
                        test);
                    Require(!result.Succeeded
                        && result.Error.Contains(
                            "Validation image content must be pairwise disjoint by SHA-256",
                            StringComparison.Ordinal),
                        "The owner accepted duplicate image content across split roles.");
                },
                passed,
                failed);

            RunCase(
                "freeze reads the selected Pipeline XML and persists all three split identities",
                () =>
                {
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Freeze(
                        recipeName,
                        pipelineName,
                        train,
                        validation,
                        test);
                    Require(result.Succeeded,
                        "The identity owner did not persist a valid PinArrayGap record.");
                    OpenVisionRecipePinArrayGapValidationRecord record = result.Record
                        ?? throw new InvalidOperationException(
                            "The identity owner returned no persisted PinArrayGap record.");
                    Require(string.Equals(record.PipelineName, pipelineName, StringComparison.Ordinal)
                        && record.Train.ImageCount == 1
                        && record.Validation.ImageCount == 1
                        && record.Test.ImageCount == 1
                        && Math.Abs(record.DistancePxRangeMaximum - 6D) <= 0.000001D,
                        "The persisted identity did not retain the selected Pipeline and split contract.");
                },
                passed,
                failed);

            RunCase(
                "evaluate reports the saved identity as current",
                () =>
                {
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Evaluate(
                        recipeName,
                        pipelineName,
                        train,
                        validation,
                        test);
                    Require(result.Succeeded && result.Matches,
                        "The owner did not match the unchanged Pipeline and split identity.");
                },
                passed,
                failed);

            RunCase(
                "selection restoration reads the frozen split names through the identity owner",
                () =>
                {
                    Require(owner.TryGetFrozenSelectionNames(
                            recipeName,
                            out string trainName,
                            out string validationName,
                            out string testName)
                        && string.Equals(trainName, "Train", StringComparison.Ordinal)
                        && string.Equals(validationName, "Validation", StringComparison.Ordinal)
                        && string.Equals(testName, "Test", StringComparison.Ordinal),
                        "The identity owner did not project the persisted split names for selection restoration.");
                },
                passed,
                failed);

            RunCase(
                "a split-role set-name change becomes stale without rewriting the frozen record",
                () =>
                {
                    OpenVisionRecipeValidationSetOption renamedValidation = CreateSet(
                        "ValidationRenamed",
                        validationImage,
                        "Validation renamed");
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Evaluate(
                        recipeName,
                        pipelineName,
                        train,
                        renamedValidation,
                        test);
                    Require(result.Succeeded && !result.Matches,
                        "The owner did not mark a validation-role set-name change stale.");
                    Require(string.Equals(
                            result.Record?.Validation?.Role,
                            "Validation",
                            StringComparison.Ordinal)
                        && string.Equals(
                            result.Record?.Validation?.SetName,
                            "Validation",
                            StringComparison.Ordinal),
                        "The stale role change rewrote or lost the frozen Validation identity.");
                },
                passed,
                failed);

            RunCase(
                "evaluate reports a changed image as stale without changing the record",
                () =>
                {
                    File.WriteAllText(trainImage, "train-v2");
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Evaluate(
                        recipeName,
                        pipelineName,
                        train,
                        validation,
                        test);
                    Require(result.Succeeded && !result.Matches,
                        "The owner did not detect a changed validation image.");
                    Require(result.Record != null && result.Record.Train.ImageCount == 1,
                        "The stale evaluation lost the persisted record projection.");
                    File.WriteAllText(trainImage, "train-v1");
                },
                passed,
                failed);

            RunCase(
                "missing Pipeline XML returns a read-only failure and leaves Recipe XML unchanged",
                () =>
                {
                    OpenVisionRecipePinArrayGapValidationIdentityResult result = owner.Evaluate(
                        recipeName,
                        "Missing_PinArrayGap",
                        train,
                        validation,
                        test);
                    Require(!result.Succeeded
                        && result.Error.Contains("Selected pipeline XML was not found", StringComparison.Ordinal),
                        "The missing Pipeline path did not return the existing failure contract.");
                    Require(File.ReadAllBytes(pipelinePath).SequenceEqual(pipelineBefore),
                        "Identity evaluation changed the saved Recipe XML.");
                },
                passed,
                failed);
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                AppPathService.DataRootEnvironmentVariable,
                previousDataRoot);
        }

        string outputPath = Path.Combine(
            evidenceDirectory,
            "pinarraygap-validation-identity-owner-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-46 PinArrayGap validation identity owner",
                "EvidenceDirectory: " + evidenceDirectory,
                "RecipeName: " + recipeName
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
            "CONTRACT|pinarraygap-validation-identity-owner|passed="
            + passed.Count.ToString(CultureInfo.InvariantCulture)
            + "|failed="
            + failed.Count.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(outputPath);
        return failed.Count == 0 ? 0 : 1;
    }

    private static OpenVisionRecipeValidationSetOption CreateSet(
        string name,
        string imagePath,
        string notes)
    {
        return new OpenVisionRecipeValidationSetOption(new OpenVisionRecipeValidationSet
        {
            Name = name,
            Images = new List<OpenVisionRecipeValidationSetImage>
            {
                new OpenVisionRecipeValidationSetImage
                {
                    Path = imagePath,
                    Expected = OpenVisionRecipeValidationSetImage.ExpectedOk,
                    Notes = notes
                }
            }
        });
    }

    private static string WriteFixture(string directory, string name, string contents)
    {
        string path = Path.Combine(directory, name);
        File.WriteAllText(path, contents);
        return path;
    }

    private static void RunCase(
        string name,
        Action action,
        ICollection<string> passed,
        ICollection<string> failed)
    {
        try
        {
            action();
            passed.Add(name);
        }
        catch (Exception exception)
        {
            failed.Add(name + ": " + exception.GetBaseException().Message);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
