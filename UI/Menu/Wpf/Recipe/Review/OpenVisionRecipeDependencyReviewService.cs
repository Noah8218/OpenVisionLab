using Lib.OpenCV;
using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionRecipeDependencyReviewResult
    {
        internal OpenVisionRecipeDependencyReviewResult(
            string report,
            IReadOnlyList<OpenVisionRecipeDependencyReviewRow> rows,
            int blockingDependencyCount)
        {
            Report = report ?? string.Empty;
            Rows = rows ?? Array.Empty<OpenVisionRecipeDependencyReviewRow>();
            BlockingDependencyCount = Math.Max(0, blockingDependencyCount);
        }

        internal string Report { get; }

        internal IReadOnlyList<OpenVisionRecipeDependencyReviewRow> Rows { get; }

        internal int BlockingDependencyCount { get; }
    }

    internal static class OpenVisionRecipeDependencyReviewService
    {
        internal static OpenVisionRecipeDependencyReviewResult Review(
            VisionPipeline pipeline,
            string recipeName,
            bool copyDependencies,
            OpenVisionRecipeReviewBundleInspection reviewBundleInspection)
        {
            if (pipeline?.Steps == null || pipeline.Steps.Count == 0)
            {
                string action = OpenVisionRecipeText.Local(
                    "Step이 없어 검사할 의존 경로가 없습니다.",
                    "No steps are available for dependency path review.");
                return new OpenVisionRecipeDependencyReviewResult(
                    OpenVisionRecipeText.Local(
                        "의존 파일 스캔 건너뜀: 파이프라인 단계가 없습니다.",
                        "Dependency scan skipped: pipeline has no steps."),
                    new[]
                    {
                        new OpenVisionRecipeDependencyReviewRow(
                            OpenVisionRecipeText.Local("대기", "Waiting"),
                            "-",
                            "-",
                            "-",
                            action)
                    },
                    0);
            }

            List<OpenVisionRecipeDependencyReviewRow> rows = new List<OpenVisionRecipeDependencyReviewRow>();
            List<string> lines = new List<string>
            {
                copyDependencies
                    ? OpenVisionRecipeText.Local("의존 파일 복사 보고서", "Dependency copy report")
                    : OpenVisionRecipeText.Local("의존 파일 스캔 보고서", "Dependency scan report")
            };
            int detected = 0;
            int copied = 0;
            int missing = 0;
            int changed = 0;
            foreach (VisionPipelineStep step in pipeline.Steps)
            {
                if (step?.Parameters == null)
                {
                    continue;
                }

                foreach (string key in step.Parameters.Keys.ToList())
                {
                    string value = step.Parameters[key];
                    if (!LooksLikeDependencyPath(key, value))
                    {
                        continue;
                    }

                    detected++;
                    OpenVisionRecipeReviewBundlePathReview bundleReview = reviewBundleInspection?.FindDependency(step.Name, key, value);
                    string sourcePath;
                    try
                    {
                        sourcePath = ResolveDependencySourcePath(value);
                    }
                    catch
                    {
                        sourcePath = string.Empty;
                    }

                    if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    {
                        if (bundleReview?.State == OpenVisionRecipeReviewBundlePathState.RelocationCandidate)
                        {
                            missing++;
                            rows.Add(new OpenVisionRecipeDependencyReviewRow(
                                OpenVisionRecipeText.Local("재배치 후보", "Relocation"),
                                step.Name,
                                key,
                                bundleReview.ReviewedPath,
                                OpenVisionRecipeText.Local("SHA 일치 후보. XML 경로를 명시적으로 수정", "SHA-matched candidate. Explicitly update the XML path")));
                            lines.Add(OpenVisionRecipeText.Local("재배치 후보: ", "Relocation candidate: ") + bundleReview.ReviewedPath);
                            lines.Add(OpenVisionRecipeText.Local("원본 XML 경로: ", "Original XML path: ") + value);
                        }
                        else if (bundleReview?.IsContentMismatch == true)
                        {
                            changed++;
                            rows.Add(new OpenVisionRecipeDependencyReviewRow(
                                OpenVisionRecipeText.Local("내용 변경", "Changed"),
                                step.Name,
                                key,
                                bundleReview.ReviewedPath,
                                bundleReview.Detail));
                            lines.Add(OpenVisionRecipeText.Local("내용 변경 후보: ", "Changed candidate: ") + bundleReview.ReviewedPath);
                        }
                        else
                        {
                            missing++;
                            rows.Add(new OpenVisionRecipeDependencyReviewRow(
                                OpenVisionRecipeText.Local("누락", "Missing"),
                                step.Name,
                                key,
                                value,
                                OpenVisionRecipeText.Local("파일 연결 또는 XML 경로 수정", "Attach file or fix XML path")));
                            lines.Add(string.Format(
                                CultureInfo.CurrentCulture,
                                OpenVisionRecipeText.Local("누락: {0}.{1} -> {2}", "Missing: {0}.{1} -> {2}"),
                                step.Name,
                                key,
                                value));
                        }

                        continue;
                    }

                    if (bundleReview?.IsContentMismatch == true)
                    {
                        changed++;
                        rows.Add(new OpenVisionRecipeDependencyReviewRow(
                            OpenVisionRecipeText.Local("내용 변경", "Changed"),
                            step.Name,
                            key,
                            sourcePath,
                            bundleReview.Detail));
                        lines.Add(OpenVisionRecipeText.Local("내용 변경: ", "Content changed: ") + sourcePath);
                        lines.Add(OpenVisionRecipeText.Local(
                            "조치: 내보내기 증거와 다른 파일은 복사하지 않습니다.",
                            "Action: a file that differs from export evidence is not copied."));
                        continue;
                    }

                    if (!copyDependencies)
                    {
                        rows.Add(new OpenVisionRecipeDependencyReviewRow(
                            OpenVisionRecipeText.Local("확인", "Found"),
                            step.Name,
                            key,
                            sourcePath,
                            OpenVisionRecipeText.Local("가져오기 시 레시피로 복사", "Copy into recipe on import")));
                        lines.Add(string.Format(
                            CultureInfo.CurrentCulture,
                            OpenVisionRecipeText.Local("찾음: {0}.{1} -> {2}", "Found: {0}.{1} -> {2}"),
                            step.Name,
                            key,
                            sourcePath));
                        continue;
                    }

                    string copiedPath = CopyDependencyToRecipe(recipeName, sourcePath);
                    step.Parameters[key] = copiedPath;
                    copied++;
                    rows.Add(new OpenVisionRecipeDependencyReviewRow(
                        OpenVisionRecipeText.Local("복사됨", "Copied"),
                        step.Name,
                        key,
                        copiedPath,
                        OpenVisionRecipeText.Local("XML 경로를 복사본으로 갱신", "XML path updated to copied file")));
                    lines.Add(string.Format(
                        CultureInfo.CurrentCulture,
                        OpenVisionRecipeText.Local("복사됨: {0}.{1} -> {2}", "Copied: {0}.{1} -> {2}"),
                        step.Name,
                        key,
                        copiedPath));
                    lines.Add(OpenVisionRecipeText.Local("원본: ", "Source: ") + sourcePath);
                }
            }

            if (reviewBundleInspection != null)
            {
                foreach (OpenVisionRecipeReviewBundlePathReview reference in reviewBundleInspection.PathReviews.Where(item => !item.IsDependency))
                {
                    rows.Add(BuildReviewBundleReferenceRow(reference));
                }
            }

            if (detected == 0)
            {
                if (rows.Count == 0)
                {
                    rows.Add(new OpenVisionRecipeDependencyReviewRow(
                        "None",
                        "-",
                        "-",
                        "-",
                        "No external dependency paths"));
                }

                lines.Add(OpenVisionRecipeText.Local("외부 이미지/템플릿 의존 파일이 없습니다.", "No external image/template dependencies detected."));
            }

            lines.Add(string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionRecipeText.Local("요약: 감지={0}, 복사={1}, 누락/재배치={2}, 내용 변경={3}", "Summary: detected={0}, copied={1}, missing/relocation={2}, changed={3}"),
                detected,
                copied,
                missing,
                changed));
            return new OpenVisionRecipeDependencyReviewResult(
                string.Join(Environment.NewLine, lines),
                rows,
                missing + changed);
        }

        internal static bool TryCopyReferenceImageToRecipe(
            string recipeName,
            string pipelineName,
            string referenceImagePath,
            out string copiedPath)
        {
            copiedPath = string.Empty;
            if (string.IsNullOrWhiteSpace(referenceImagePath) || !File.Exists(referenceImagePath))
            {
                return false;
            }

            string imageDirectory = RecipeWorkspaceService.GetVisionPipelineImageDirectory(recipeName, pipelineName);
            copiedPath = CreateUniqueFilePath(imageDirectory, "Reference_" + Path.GetFileName(referenceImagePath));
            File.Copy(referenceImagePath, copiedPath, overwrite: false);
            return true;
        }

        internal static bool LooksLikeDependencyPath(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalizedKey = key ?? string.Empty;
            bool keyLooksPath = normalizedKey.IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0
                || normalizedKey.IndexOf("template", StringComparison.OrdinalIgnoreCase) >= 0
                || normalizedKey.IndexOf("pattern", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!keyLooksPath)
            {
                return false;
            }

            return IsSupportedDependencyExtension(Path.GetExtension(value.Trim()));
        }

        internal static string ResolveDependencySourcePath(string value)
        {
            string candidate = (value ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(candidate))
            {
                return Path.GetFullPath(candidate);
            }

            return Path.GetFullPath(Path.Combine(AppPathService.StartupPath, candidate));
        }

        private static OpenVisionRecipeDependencyReviewRow BuildReviewBundleReferenceRow(OpenVisionRecipeReviewBundlePathReview review)
        {
            string status;
            string action;
            switch (review.State)
            {
                case OpenVisionRecipeReviewBundlePathState.Found:
                    status = OpenVisionRecipeText.Local("참조 확인", "Reference");
                    action = OpenVisionRecipeText.Local("검토 전용. 자동 복사/가져오기 없음", "Review only. No automatic copy/import");
                    break;
                case OpenVisionRecipeReviewBundlePathState.RelocationCandidate:
                    status = OpenVisionRecipeText.Local("참조 후보", "Ref candidate");
                    action = OpenVisionRecipeText.Local("SHA 일치 후보. 검토 전용", "SHA-matched candidate. Review only");
                    break;
                case OpenVisionRecipeReviewBundlePathState.ContentMismatch:
                    status = OpenVisionRecipeText.Local("참조 변경", "Ref changed");
                    action = review.Detail;
                    break;
                default:
                    status = OpenVisionRecipeText.Local("참조 누락", "Ref missing");
                    action = OpenVisionRecipeText.Local("선택 검토 자료 누락. 파이프라인 경로는 변경 안 함", "Optional review evidence missing. Pipeline path unchanged");
                    break;
            }

            return new OpenVisionRecipeDependencyReviewRow(
                status,
                OpenVisionRecipeText.Local("검토 참조", "Review reference"),
                review.Name,
                review.ReviewedPath,
                action);
        }

        private static bool IsSupportedDependencyExtension(string extension)
        {
            return string.Equals(extension, ".bmp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tif", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".tiff", StringComparison.OrdinalIgnoreCase);
        }

        private static string CopyDependencyToRecipe(string recipeName, string sourcePath)
        {
            string templateDirectory = RecipeWorkspaceService.GetTemplateDirectory(recipeName);
            string targetPath = CreateUniqueFilePath(templateDirectory, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, targetPath, overwrite: false);
            return Path.GetRelativePath(AppPathService.StartupPath, targetPath);
        }

        private static string CreateUniqueFilePath(string directory, string fileName)
        {
            Directory.CreateDirectory(directory);
            string safeName = string.IsNullOrWhiteSpace(fileName) ? "Dependency.bin" : fileName;
            string candidate = Path.Combine(directory, safeName);
            if (!File.Exists(candidate))
            {
                return candidate;
            }

            string name = Path.GetFileNameWithoutExtension(safeName);
            string extension = Path.GetExtension(safeName);
            for (int index = 2; ; index++)
            {
                candidate = Path.Combine(directory, name + "_" + index.ToString(CultureInfo.InvariantCulture) + extension);
                if (!File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }
    }
}
