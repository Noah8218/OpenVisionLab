using Lib.OpenCV.Pipeline;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenVisionLab
{
    internal sealed class VisionToolNImageValidationPromotionResult
    {
        public string RecipeName { get; init; } = string.Empty;
        public string PipelineName { get; init; } = string.Empty;
        public string ValidationSetName { get; init; } = string.Empty;
        public string PipelineDefinitionSha256 { get; init; } = string.Empty;
        public string ImageSetSha256 { get; init; } = string.Empty;
        public string PipelinePath { get; init; } = string.Empty;
        public string ValidationSetPath { get; init; } = string.Empty;
        public int ImageCount { get; init; }
        public int DependencyCount { get; init; }
        public bool ReusedExistingIdentity { get; init; }
    }

    internal static class VisionToolNImageValidationPromotionService
    {
        public static bool TryPromoteLocatorExpectedSuccess(
            string recipeName,
            VisionToolNImageVerificationSession session,
            out VisionToolNImageValidationPromotionResult result,
            out string error)
        {
            result = null;
            error = string.Empty;
            string targetRecipeName = recipeName?.Trim() ?? string.Empty;
            if (!RecipeWorkspaceService.IsValidRecipeName(targetRecipeName))
            {
                error = "승격할 Recipe 이름이 올바르지 않습니다.";
                return false;
            }

            if (session == null
                || session.WasCancelled
                || session.Rows == null
                || session.Rows.Count == 0
                || session.Rows.Any(row => row == null || !row.Success))
            {
                error = "완료된 locator 성공 세션만 승격할 수 있습니다.";
                return false;
            }

            string definitionSha256 =
                OpenVisionRecipeValidationSetStorage.ComputeTextSha256(session.PipelineXml);
            if (!string.Equals(
                    definitionSha256,
                    session.StepDefinitionSha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                error = "보존된 Pipeline 정의 SHA-256이 현재 세션과 다릅니다.";
                return false;
            }

            if (!SerializeHelper.TryLoadFromXmlText(
                    session.PipelineXml,
                    out VisionPipeline pipeline,
                    out string parseError)
                || pipeline == null
                || pipeline.Steps.Count != 1)
            {
                error = string.IsNullOrWhiteSpace(parseError)
                    ? "승격 Pipeline은 정확히 한 Step이어야 합니다."
                    : parseError;
                return false;
            }

            VisionPipelineStep step = pipeline.Steps[0];
            if (!IsLocatorTool(step.ToolType))
            {
                error = "Matching, EdgeBasedMatching 또는 FeatureMatching locator만 이 경로로 승격할 수 있습니다.";
                return false;
            }

            if (!string.Equals(pipeline.Name, session.PipelineName, StringComparison.Ordinal))
            {
                error = "보존된 Pipeline 이름이 현재 세션과 다릅니다.";
                return false;
            }

            List<OpenVisionRecipeValidationSetImage> images = new List<OpenVisionRecipeValidationSetImage>();
            foreach (VisionToolNImageVerificationRow row in session.Rows)
            {
                string imageSha256 = string.IsNullOrWhiteSpace(row?.ImagePath) || !File.Exists(row.ImagePath)
                    ? string.Empty
                    : OpenVisionRecipeValidationSetStorage.ComputeFileSha256(row.ImagePath);
                if (string.IsNullOrWhiteSpace(row.ImagePath)
                    || !File.Exists(row.ImagePath)
                    || !VisionPipelineRunReportStorage.IsFileSha256Match(
                        row.SourceSnapshotPath,
                        row.SourceSha256)
                    || !AreDecodedImagesEqual(row.ImagePath, row.SourceSnapshotPath))
                {
                    error = "승격할 이미지가 실행 시점 source 증거와 픽셀 단위로 일치하지 않습니다: " + row.FileName;
                    return false;
                }

                images.Add(new OpenVisionRecipeValidationSetImage
                {
                    Expected = OpenVisionRecipeValidationSetImage.ExpectedOk,
                    Path = Path.GetFullPath(row.ImagePath),
                    Sha256 = imageSha256,
                    Notes = string.Format(
                        CultureInfo.InvariantCulture,
                        "Locator expected success; source N-image row #{0:0000}; retained snapshot SHA-256={1}.",
                        row.Index,
                        row.SourceSha256)
                });
            }

            string imageSetSha256 =
                OpenVisionRecipeValidationSetStorage.ComputeImageSetSha256(images);
            List<OpenVisionRecipeValidationSetDependency> dependencies =
                CollectDependencies(step, out error);
            if (dependencies == null)
            {
                return false;
            }

            string setName = CreateSetName(
                session.ToolName,
                definitionSha256,
                imageSetSha256);
            if (!OpenVisionRecipeValidationSetStorage.TryLoad(
                    targetRecipeName,
                    out OpenVisionRecipeValidationSetDocument document,
                    out error))
            {
                return false;
            }

            OpenVisionRecipeValidationSet existing = document.Sets.FirstOrDefault(set =>
                string.Equals(set?.Name, setName, StringComparison.OrdinalIgnoreCase));
            bool reused = existing != null;
            if (existing != null && !HasSameIdentity(
                    existing,
                    session.PipelineName,
                    definitionSha256,
                    imageSetSha256,
                    images,
                    dependencies))
            {
                error = "같은 이름의 Validation Set이 다른 동결 identity를 사용합니다: " + setName;
                return false;
            }

            string pipelinePath = RecipeWorkspaceService.GetVisionPipelinePath(
                targetRecipeName,
                session.PipelineName);
            bool createdPipeline = false;
            if (File.Exists(pipelinePath))
            {
                string existingXml = File.ReadAllText(pipelinePath);
                string existingSha256 =
                    OpenVisionRecipeValidationSetStorage.ComputeTextSha256(existingXml);
                if (!string.Equals(existingSha256, definitionSha256, StringComparison.OrdinalIgnoreCase))
                {
                    error = "같은 이름의 Recipe Manager Pipeline이 다른 정의를 사용합니다: " + session.PipelineName;
                    return false;
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pipelinePath));
                File.WriteAllText(pipelinePath, session.PipelineXml, Encoding.Unicode);
                createdPipeline = true;
            }

            if (!VisionPipelineStorage.TryLoadFromFile(
                    pipelinePath,
                    out VisionPipeline loadedPipeline,
                    out string loadMessage)
                || loadedPipeline == null
                || !string.Equals(loadedPipeline.Name, session.PipelineName, StringComparison.Ordinal))
            {
                if (createdPipeline)
                {
                    File.Delete(pipelinePath);
                }

                error = "저장된 Pipeline 재로드 실패: " + loadMessage;
                return false;
            }

            if (existing == null)
            {
                document.Sets.Add(new OpenVisionRecipeValidationSet
                {
                    Name = setName,
                    PipelineName = session.PipelineName,
                    PipelineDefinitionSha256 = definitionSha256,
                    ImageSetSha256 = imageSetSha256,
                    Notes =
                        "Locator expected-success validation. All rows are Expected OK for locator execution; "
                        + "source corpus OK/NG labels are not defect judgments. "
                        + "Promotion does not run Preview or Validation.",
                    Dependencies = dependencies,
                    Images = images
                });
            }

            if (!OpenVisionRecipeValidationSetStorage.TrySave(
                    targetRecipeName,
                    document,
                    out error))
            {
                if (createdPipeline)
                {
                    File.Delete(pipelinePath);
                }

                return false;
            }

            if (!OpenVisionRecipeValidationSetStorage.TryLoad(
                    targetRecipeName,
                    out OpenVisionRecipeValidationSetDocument reloaded,
                    out error))
            {
                return false;
            }

            OpenVisionRecipeValidationSet reloadedSet = reloaded.Sets.FirstOrDefault(set =>
                string.Equals(set?.Name, setName, StringComparison.OrdinalIgnoreCase));
            if (!HasSameIdentity(
                    reloadedSet,
                    session.PipelineName,
                    definitionSha256,
                    imageSetSha256,
                    images,
                    dependencies)
                || !OpenVisionRecipeValidationSetStorage.TryValidateFrozenIdentity(
                    reloadedSet,
                    session.PipelineName,
                    session.PipelineXml,
                    out error))
            {
                error = string.IsNullOrWhiteSpace(error)
                    ? "승격된 Validation Set 재로드 identity가 일치하지 않습니다."
                    : error;
                return false;
            }

            result = new VisionToolNImageValidationPromotionResult
            {
                RecipeName = targetRecipeName,
                PipelineName = session.PipelineName,
                ValidationSetName = setName,
                PipelineDefinitionSha256 = definitionSha256,
                ImageSetSha256 = imageSetSha256,
                PipelinePath = pipelinePath,
                ValidationSetPath = OpenVisionRecipeValidationSetStorage.GetPath(targetRecipeName),
                ImageCount = images.Count,
                DependencyCount = dependencies.Count,
                ReusedExistingIdentity = reused
            };
            return true;
        }

        private static List<OpenVisionRecipeValidationSetDependency> CollectDependencies(
            VisionPipelineStep step,
            out string error)
        {
            error = string.Empty;
            Dictionary<string, OpenVisionRecipeValidationSetDependency> dependencies =
                new Dictionary<string, OpenVisionRecipeValidationSetDependency>(
                    StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> parameter in
                step?.Parameters ?? new Dictionary<string, string>())
            {
                if (!OpenVisionRecipeDependencyReviewService.LooksLikeDependencyPath(
                        parameter.Key,
                        parameter.Value))
                {
                    continue;
                }

                string path;
                try
                {
                    path = OpenVisionRecipeDependencyReviewService.ResolveDependencySourcePath(
                        parameter.Value);
                }
                catch (Exception ex)
                {
                    error = ex.GetBaseException().Message;
                    return null;
                }

                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    error = "locator 의존 파일을 찾을 수 없습니다: " + parameter.Value;
                    return null;
                }

                if (!dependencies.ContainsKey(path))
                {
                    dependencies[path] = new OpenVisionRecipeValidationSetDependency
                    {
                        Path = path,
                        Sha256 = OpenVisionRecipeValidationSetStorage.ComputeFileSha256(path)
                    };
                }
            }

            return dependencies.Values
                .OrderBy(item => item.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool HasSameIdentity(
            OpenVisionRecipeValidationSet set,
            string pipelineName,
            string definitionSha256,
            string imageSetSha256,
            IReadOnlyList<OpenVisionRecipeValidationSetImage> images,
            IReadOnlyList<OpenVisionRecipeValidationSetDependency> dependencies)
        {
            if (set == null
                || !string.Equals(set.PipelineName, pipelineName, StringComparison.Ordinal)
                || !string.Equals(
                    set.PipelineDefinitionSha256,
                    definitionSha256,
                    StringComparison.OrdinalIgnoreCase)
                || !string.Equals(
                    set.ImageSetSha256,
                    imageSetSha256,
                    StringComparison.OrdinalIgnoreCase)
                || (set.Images?.Count ?? 0) != images.Count
                || (set.Dependencies?.Count ?? 0) != dependencies.Count)
            {
                return false;
            }

            return set.Images.Zip(images, (left, right) =>
                    string.Equals(left.Path, right.Path, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(left.Sha256, right.Sha256, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(
                        left.Expected,
                        OpenVisionRecipeValidationSetImage.ExpectedOk,
                        StringComparison.OrdinalIgnoreCase))
                .All(equal => equal)
                && set.Dependencies.All(left => dependencies.Any(right =>
                    string.Equals(left.Path, right.Path, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(left.Sha256, right.Sha256, StringComparison.OrdinalIgnoreCase)));
        }

        private static bool IsLocatorTool(string toolType)
        {
            string value = toolType?.Trim() ?? string.Empty;
            return string.Equals(value, "Matching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "TemplateMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "EdgeBasedMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "EdgeBasedTemplateMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "EdgeTemplateMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "FeatureMatching", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "Feature", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "Sift", StringComparison.OrdinalIgnoreCase);
        }

        private static string CreateSetName(
            string toolName,
            string definitionSha256,
            string imageSetSha256)
        {
            string safeToolName = new string((toolName ?? "Locator")
                .Where(character => !char.IsControl(character) && !char.IsWhiteSpace(character))
                .ToArray());
            if (string.IsNullOrWhiteSpace(safeToolName))
            {
                safeToolName = "Locator";
            }

            string name =
                "Locator_"
                + safeToolName
                + "_"
                + definitionSha256.Substring(0, 8)
                + "_"
                + imageSetSha256.Substring(0, 8);
            return name.Length <= 80 ? name : name.Substring(0, 80);
        }

        private static bool AreDecodedImagesEqual(string leftPath, string rightPath)
        {
            try
            {
                using System.Drawing.Bitmap leftBitmap = new System.Drawing.Bitmap(leftPath);
                using System.Drawing.Bitmap rightBitmap = new System.Drawing.Bitmap(rightPath);
                using Mat left = Lib.Common.BitmapImageConverter.ToMat(leftBitmap);
                using Mat right = Lib.Common.BitmapImageConverter.ToMat(rightBitmap);
                if (left.Empty()
                    || right.Empty()
                    || left.Rows != right.Rows
                    || left.Cols != right.Cols
                    || left.Type() != right.Type())
                {
                    return false;
                }

                return Cv2.Norm(left, right, NormTypes.L1) == 0D;
            }
            catch (Exception ex) when (
                ex is ArgumentException
                || ex is IOException
                || ex is OpenCvSharpException)
            {
                return false;
            }
        }
    }
}
