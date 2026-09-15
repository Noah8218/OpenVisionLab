using OpenVisionLab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

internal static class ValidationSetStatusPresenterContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory
            ?? Path.Combine(
                "D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev",
                "ovl47-validation-set-status-presenter-contract_"
                    + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)));
        Directory.CreateDirectory(evidenceDirectory);
        List<string> passed = new List<string>();
        List<string> failed = new List<string>();
        OpenVisionLanguage previousLanguage = OpenVisionLanguageService.CurrentLanguage;

        try
        {
            Require(string.Equals(
                    Path.GetPathRoot(evidenceDirectory),
                    @"D:\",
                    StringComparison.OrdinalIgnoreCase),
                "Validation Set status presenter evidence must be on D:.");

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildEmptyFolderImageRegistrationStatus(),
                    "선택한 폴더의 바로 아래에서 지원 이미지 파일을 찾지 못했습니다.",
                    StringComparison.Ordinal),
                "Korean empty-folder status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildFolderImageRegistrationError(
                        "Validation image folder is missing."),
                    "폴더 이미지 등록 ERROR: Validation image folder is missing.",
                    StringComparison.Ordinal),
                "Korean folder-error status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildImageRegistrationStatus("OK", 2, 1, 3),
                    "OK 이미지: 추가 2, 갱신 1, 건너뜀 3",
                    StringComparison.Ordinal),
                "Korean image-count status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus(
                        "검증 세트 만들기",
                        "Validation set document is missing."),
                    "검증 세트 만들기 ERROR: Validation set document is missing.",
                    StringComparison.Ordinal),
                "Korean save-error status changed.");
            passed.Add("Korean folder and image registration status projection");

            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildEmptyFolderImageRegistrationStatus(),
                    "No supported images were found directly in the selected folder.",
                    StringComparison.Ordinal),
                "English empty-folder status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildFolderImageRegistrationError(
                        "Validation image folder is missing."),
                    "Folder image registration ERROR: Validation image folder is missing.",
                    StringComparison.Ordinal),
                "English folder-error status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildImageRegistrationStatus("OK", 2, 1, 3),
                    "OK images: added 2, updated 1, skipped 3",
                    StringComparison.Ordinal),
                "English image-count status changed.");
            Require(
                string.Equals(
                    OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus(
                        "Create validation set",
                        "Validation set document is missing."),
                    "Create validation set ERROR: Validation set document is missing.",
                    StringComparison.Ordinal),
                "English save-error status changed.");
            passed.Add("English folder and image registration status projection");
        }
        catch (Exception exception)
        {
            failed.Add(exception.GetBaseException().Message);
        }
        finally
        {
            OpenVisionLanguageService.SetLanguage(previousLanguage, save: false);
        }

        string outputPath = Path.Combine(evidenceDirectory, "validation-set-status-presenter-contract.txt");
        File.WriteAllLines(
            outputPath,
            new[]
            {
                "Contract: OVL-47 Validation Set status presenter",
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
            "CONTRACT|validation-set-status-presenter|passed="
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
