using Lib.OpenCV.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OpenVisionLab
{
    internal enum OpenVisionRecipeReviewBundlePathState
    {
        Found,
        RelocationCandidate,
        Missing,
        ContentMismatch
    }

    internal sealed class OpenVisionRecipeReviewBundlePathReview
    {
        internal OpenVisionRecipeReviewBundlePathReview(
            bool isDependency,
            string name,
            string stepName,
            string parameterName,
            string sourcePath,
            string reviewedPath,
            OpenVisionRecipeReviewBundlePathState state,
            string detail)
        {
            IsDependency = isDependency;
            Name = name ?? string.Empty;
            StepName = stepName ?? string.Empty;
            ParameterName = parameterName ?? string.Empty;
            SourcePath = sourcePath ?? string.Empty;
            ReviewedPath = reviewedPath ?? string.Empty;
            State = state;
            Detail = detail ?? string.Empty;
        }

        internal bool IsDependency { get; }

        internal string Name { get; }

        internal string StepName { get; }

        internal string ParameterName { get; }

        internal string SourcePath { get; }

        internal string ReviewedPath { get; }

        internal OpenVisionRecipeReviewBundlePathState State { get; }

        internal string Detail { get; }

        internal bool IsContentMismatch => State == OpenVisionRecipeReviewBundlePathState.ContentMismatch;
    }

    internal sealed class OpenVisionRecipeReviewBundleInspection
    {
        internal OpenVisionRecipeReviewBundleInspection(
            string packagePath,
            string pipelineXml,
            string integrityReport,
            string pathReport,
            IReadOnlyList<OpenVisionRecipeReviewBundlePathReview> pathReviews)
        {
            PackagePath = packagePath ?? string.Empty;
            PipelineXml = pipelineXml ?? string.Empty;
            IntegrityReport = integrityReport ?? string.Empty;
            PathReport = pathReport ?? string.Empty;
            PathReviews = pathReviews ?? Array.Empty<OpenVisionRecipeReviewBundlePathReview>();
        }

        internal string PackagePath { get; }

        internal string PipelineXml { get; }

        internal string IntegrityReport { get; }

        internal string PathReport { get; }

        internal IReadOnlyList<OpenVisionRecipeReviewBundlePathReview> PathReviews { get; }

        internal OpenVisionRecipeReviewBundlePathReview FindDependency(string stepName, string parameterName, string sourcePath)
        {
            return PathReviews.FirstOrDefault(item =>
                item.IsDependency
                && string.Equals(item.StepName, stepName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.ParameterName, parameterName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                && string.Equals(NormalizePath(item.SourcePath), NormalizePath(sourcePath), StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizePath(string value)
        {
            return (value ?? string.Empty).Trim().Trim('"');
        }
    }

    internal static class OpenVisionRecipeReviewBundleInspector
    {
        internal const string FormatName = "OpenVisionLab.RecipeReviewBundle";
        internal const int SupportedSchemaVersion = 1;

        private const long MaxPipelineEntryBytes = 5L * 1024L * 1024L;
        private const long MaxManifestEntryBytes = 2L * 1024L * 1024L;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        internal static bool TryInspect(string packagePath, out OpenVisionRecipeReviewBundleInspection inspection)
        {
            inspection = new OpenVisionRecipeReviewBundleInspection(
                packagePath,
                string.Empty,
                Local("검토 번들 dry-run: NG", "Review bundle dry-run: NG"),
                Local("경로 검토를 실행하지 못했습니다.", "Path review was not executed."),
                Array.Empty<OpenVisionRecipeReviewBundlePathReview>());

            if (string.IsNullOrWhiteSpace(packagePath) || !File.Exists(packagePath))
            {
                inspection = Failure(packagePath, Local("검토 번들 파일을 찾을 수 없습니다.", "Review bundle file was not found."));
                return false;
            }

            try
            {
                string fullPath = Path.GetFullPath(packagePath);
                using FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
                if (archive.Entries.Count != 2
                    || archive.Entries.GroupBy(entry => entry.FullName, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() != 1))
                {
                    inspection = Failure(fullPath, Local(
                        "schema v1 번들은 pipeline.xml과 review-manifest.json 두 엔트리만 포함해야 합니다.",
                        "A schema v1 bundle must contain only pipeline.xml and review-manifest.json."));
                    return false;
                }

                ZipArchiveEntry pipelineEntry = archive.Entries.SingleOrDefault(entry =>
                    string.Equals(entry.FullName, OpenVisionRecipeReviewBundleExporter.PipelineEntryName, StringComparison.Ordinal));
                ZipArchiveEntry manifestEntry = archive.Entries.SingleOrDefault(entry =>
                    string.Equals(entry.FullName, OpenVisionRecipeReviewBundleExporter.ManifestEntryName, StringComparison.Ordinal));
                if (pipelineEntry == null || manifestEntry == null)
                {
                    inspection = Failure(fullPath, Local("필수 번들 엔트리가 없습니다.", "Required bundle entries are missing."));
                    return false;
                }

                byte[] pipelineBytes = ReadEntryBytes(pipelineEntry, MaxPipelineEntryBytes);
                byte[] manifestBytes = ReadEntryBytes(manifestEntry, MaxManifestEntryBytes);
                ReviewManifest manifest = JsonSerializer.Deserialize<ReviewManifest>(manifestBytes, JsonOptions);
                if (!TryValidateManifest(manifest, pipelineBytes, out string manifestError))
                {
                    inspection = Failure(fullPath, manifestError);
                    return false;
                }

                string pipelineXml = new UTF8Encoding(false, true).GetString(pipelineBytes);
                if (!SerializeHelper.TryLoadFromXmlText(pipelineXml, out VisionPipeline pipeline, out string loadError) || pipeline == null)
                {
                    inspection = Failure(fullPath, Local(
                        "pipeline.xml을 VisionPipeline으로 읽을 수 없습니다: ",
                        "pipeline.xml could not be read as VisionPipeline: ") + loadError);
                    return false;
                }

                if (!DependencyManifestMatchesPipeline(manifest.Dependencies, pipeline))
                {
                    inspection = Failure(fullPath, Local(
                        "manifest 의존성 목록이 pipeline.xml의 Step 파라미터와 일치하지 않습니다.",
                        "Manifest dependencies do not match pipeline.xml step parameters."));
                    return false;
                }

                int stepCount = pipeline.Steps?.Count(step => step != null) ?? 0;
                if (manifest.Summary == null
                    || manifest.Summary.StepCount != stepCount
                    || manifest.Summary.DependencyCount != (manifest.Dependencies?.Count ?? 0))
                {
                    inspection = Failure(fullPath, Local(
                        "manifest 요약 수치가 pipeline.xml 또는 의존성 목록과 일치하지 않습니다.",
                        "Manifest summary counts do not match pipeline.xml or its dependency list."));
                    return false;
                }

                string packageDirectory = Path.GetDirectoryName(fullPath) ?? string.Empty;
                List<OpenVisionRecipeReviewBundlePathReview> reviews = new List<OpenVisionRecipeReviewBundlePathReview>();
                reviews.AddRange((manifest.Dependencies ?? new List<ReviewFileRecord>())
                    .Select(item => BuildPathReview(item, packageDirectory, isDependency: true)));
                reviews.AddRange((manifest.References ?? new List<ReviewFileRecord>())
                    .Select(item => BuildPathReview(item, packageDirectory, isDependency: false)));

                string integrityReport = string.Join(Environment.NewLine, new[]
                {
                    Local("검토 번들 dry-run: OK", "Review bundle dry-run: OK"),
                    Local("파일: ", "File: ") + fullPath,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Local("형식/스키마: {0} / v{1}", "Format/schema: {0} / v{1}"),
                        manifest.Format,
                        manifest.SchemaVersion),
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Local("엔트리/무결성: 2개 / pipeline.xml {0:N0} bytes / SHA-256 OK", "Entries/integrity: 2 / pipeline.xml {0:N0} bytes / SHA-256 OK"),
                        pipelineBytes.LongLength),
                    Local("내보낸 앱 버전: ", "Exporting app version: ") + FormatValue(manifest.ApplicationVersion),
                    Local("내보낼 때 XML 검증: ", "XML validation at export: ") + FormatValue(manifest.Validation?.Status),
                    Local(
                        "정책: 참조 파일 미포함 / 가져오기 미실행 / Preview·Run 미실행",
                        "Policy: references not embedded / import not executed / Preview and Run not executed")
                });

                inspection = new OpenVisionRecipeReviewBundleInspection(
                    fullPath,
                    pipelineXml,
                    integrityReport,
                    BuildPathReport(reviews),
                    reviews);
                return true;
            }
            catch (Exception ex)
            {
                inspection = Failure(packagePath, ex.GetBaseException().Message);
                return false;
            }
        }

        private static bool TryValidateManifest(ReviewManifest manifest, byte[] pipelineBytes, out string error)
        {
            if (manifest == null)
            {
                error = Local("review-manifest.json을 읽을 수 없습니다.", "review-manifest.json could not be read.");
                return false;
            }

            if (!string.Equals(manifest.Format, FormatName, StringComparison.Ordinal)
                || manifest.SchemaVersion != SupportedSchemaVersion
                || !string.Equals(manifest.PipelineXmlSchema, "VisionPipeline", StringComparison.Ordinal)
                || manifest.PipelineXml == null
                || !string.Equals(manifest.PipelineXml.Entry, OpenVisionRecipeReviewBundleExporter.PipelineEntryName, StringComparison.Ordinal))
            {
                error = Local("지원하지 않는 검토 번들 형식 또는 스키마입니다.", "The review bundle format or schema is not supported.");
                return false;
            }

            if (manifest.PackagePolicy == null
                || manifest.PackagePolicy.ReferencedFilesCopied
                || manifest.PackagePolicy.PrivateLocalAssetsCopied
                || manifest.PackagePolicy.ImportExecuted
                || manifest.PackagePolicy.PreviewOrRunExecuted)
            {
                error = Local("schema v1 참조 전용 패키지 정책과 일치하지 않습니다.", "The bundle does not match the schema v1 reference-only package policy.");
                return false;
            }

            string actualHash = ComputeSha256(pipelineBytes);
            if (manifest.PipelineXml.SizeBytes != pipelineBytes.LongLength
                || !string.Equals(manifest.PipelineXml.Sha256, actualHash, StringComparison.OrdinalIgnoreCase))
            {
                error = Local("pipeline.xml 크기 또는 SHA-256 무결성 검사가 실패했습니다.", "pipeline.xml size or SHA-256 integrity check failed.");
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool DependencyManifestMatchesPipeline(IReadOnlyList<ReviewFileRecord> manifestDependencies, VisionPipeline pipeline)
        {
            List<string> expected = new List<string>();
            foreach (VisionPipelineStep step in pipeline.Steps ?? new List<VisionPipelineStep>())
            {
                if (step?.Parameters == null)
                {
                    continue;
                }

                foreach (KeyValuePair<string, string> parameter in step.Parameters)
                {
                    if (OpenVisionRecipeDependencyReviewService.LooksLikeDependencyPath(parameter.Key, parameter.Value))
                    {
                        expected.Add(BuildDependencyKey(step.Name, parameter.Key, parameter.Value));
                    }
                }
            }

            List<string> actual = (manifestDependencies ?? Array.Empty<ReviewFileRecord>())
                .Select(item => BuildDependencyKey(item.StepName, item.ParameterName, item.SourcePath))
                .ToList();
            expected.Sort(StringComparer.OrdinalIgnoreCase);
            actual.Sort(StringComparer.OrdinalIgnoreCase);
            return expected.SequenceEqual(actual, StringComparer.OrdinalIgnoreCase);
        }

        private static string BuildDependencyKey(string stepName, string parameterName, string sourcePath)
        {
            return (stepName ?? string.Empty).Trim()
                + "\n" + (parameterName ?? string.Empty).Trim()
                + "\n" + NormalizePath(sourcePath);
        }

        private static OpenVisionRecipeReviewBundlePathReview BuildPathReview(
            ReviewFileRecord record,
            string packageDirectory,
            bool isDependency)
        {
            record ??= new ReviewFileRecord();
            string sourcePath = NormalizePath(record.SourcePath);
            string currentPath = ResolveCurrentPath(sourcePath);
            if (!string.IsNullOrWhiteSpace(currentPath) && File.Exists(currentPath))
            {
                return EvaluateExistingPath(record, isDependency, currentPath, isRelocationCandidate: false);
            }

            string relocationCandidate = ResolveRelocationCandidate(packageDirectory, sourcePath);
            if (!string.IsNullOrWhiteSpace(relocationCandidate) && File.Exists(relocationCandidate))
            {
                return EvaluateExistingPath(record, isDependency, relocationCandidate, isRelocationCandidate: true);
            }

            return new OpenVisionRecipeReviewBundlePathReview(
                isDependency,
                record.Name,
                record.StepName,
                record.ParameterName,
                sourcePath,
                sourcePath,
                OpenVisionRecipeReviewBundlePathState.Missing,
                Local("원본 경로와 번들 인접 후보 모두 없음", "Neither the source path nor an adjacent bundle candidate exists"));
        }

        private static OpenVisionRecipeReviewBundlePathReview EvaluateExistingPath(
            ReviewFileRecord record,
            bool isDependency,
            string path,
            bool isRelocationCandidate)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                bool sizeMatches = !record.SizeBytes.HasValue || record.SizeBytes.Value == file.Length;
                bool hashMatches = string.IsNullOrWhiteSpace(record.Sha256)
                    || string.Equals(record.Sha256, ComputeFileSha256(path), StringComparison.OrdinalIgnoreCase);
                if (!sizeMatches || !hashMatches)
                {
                    return new OpenVisionRecipeReviewBundlePathReview(
                        isDependency,
                        record.Name,
                        record.StepName,
                        record.ParameterName,
                        NormalizePath(record.SourcePath),
                        path,
                        OpenVisionRecipeReviewBundlePathState.ContentMismatch,
                        Local("내보낼 때 기록한 크기/SHA-256과 다름", "Size/SHA-256 differs from the export record"));
                }

                return new OpenVisionRecipeReviewBundlePathReview(
                    isDependency,
                    record.Name,
                    record.StepName,
                    record.ParameterName,
                    NormalizePath(record.SourcePath),
                    path,
                    isRelocationCandidate
                        ? OpenVisionRecipeReviewBundlePathState.RelocationCandidate
                        : OpenVisionRecipeReviewBundlePathState.Found,
                    isRelocationCandidate
                        ? Local("SHA-256이 일치하는 번들 인접 후보", "Adjacent bundle candidate with matching SHA-256")
                        : Local("현재 경로와 내보내기 증거가 일치", "Current path matches the export evidence"));
            }
            catch (Exception ex)
            {
                return new OpenVisionRecipeReviewBundlePathReview(
                    isDependency,
                    record.Name,
                    record.StepName,
                    record.ParameterName,
                    NormalizePath(record.SourcePath),
                    path,
                    OpenVisionRecipeReviewBundlePathState.ContentMismatch,
                    Local("파일 증거를 읽을 수 없음: ", "File evidence could not be read: ") + ex.GetBaseException().Message);
            }
        }

        private static string ResolveCurrentPath(string sourcePath)
        {
            try
            {
                return OpenVisionRecipeDependencyReviewService.ResolveDependencySourcePath(sourcePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ResolveRelocationCandidate(string packageDirectory, string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(packageDirectory) || string.IsNullOrWhiteSpace(sourcePath))
            {
                return string.Empty;
            }

            try
            {
                string relativeCandidate = Path.IsPathRooted(sourcePath)
                    ? Path.GetFileName(sourcePath)
                    : sourcePath;
                string candidate = Path.GetFullPath(Path.Combine(packageDirectory, relativeCandidate));
                string relativeToPackage = Path.GetRelativePath(packageDirectory, candidate);
                if (Path.IsPathRooted(relativeToPackage)
                    || relativeToPackage.Equals("..", StringComparison.Ordinal)
                    || relativeToPackage.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                    || relativeToPackage.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
                {
                    return string.Empty;
                }

                return candidate;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string BuildPathReport(IReadOnlyList<OpenVisionRecipeReviewBundlePathReview> reviews)
        {
            int dependencyCount = reviews.Count(item => item.IsDependency);
            int referenceCount = reviews.Count(item => !item.IsDependency);
            int found = reviews.Count(item => item.State == OpenVisionRecipeReviewBundlePathState.Found);
            int relocation = reviews.Count(item => item.State == OpenVisionRecipeReviewBundlePathState.RelocationCandidate);
            int missing = reviews.Count(item => item.State == OpenVisionRecipeReviewBundlePathState.Missing);
            int changed = reviews.Count(item => item.State == OpenVisionRecipeReviewBundlePathState.ContentMismatch);
            return string.Join(Environment.NewLine, new[]
            {
                Local("검토 번들 경로 dry-run", "Review bundle path dry-run"),
                string.Format(
                    CultureInfo.CurrentCulture,
                    Local("대상: 파이프라인 의존성={0}, 검토 참조={1}", "Targets: pipeline dependencies={0}, review references={1}"),
                    dependencyCount,
                    referenceCount),
                string.Format(
                    CultureInfo.CurrentCulture,
                    Local("결과: 현재 경로={0}, 재배치 후보={1}, 누락={2}, 내용 변경={3}", "Result: current path={0}, relocation candidate={1}, missing={2}, content changed={3}"),
                    found,
                    relocation,
                    missing,
                    changed),
                Local(
                    "정책: 후보를 표시할 뿐 XML 경로 변경, 파일 복사, 가져오기, Preview/Run은 실행하지 않습니다.",
                    "Policy: candidates are reported only; XML paths, file copies, import, Preview, and Run are not changed or executed."),
                relocation > 0
                    ? Local("다음: 후보 증거를 확인한 뒤 XML 경로를 명시적으로 수정하고 다시 검증하세요.", "Next: verify candidate evidence, explicitly update the XML path, then validate again.")
                    : Local("다음: 누락/변경 항목을 해결한 뒤 명시적으로 검증·가져오기를 실행하세요.", "Next: resolve missing/changed items, then explicitly validate and import.")
            });
        }

        private static byte[] ReadEntryBytes(ZipArchiveEntry entry, long maximumBytes)
        {
            if (entry.Length < 0 || entry.Length > maximumBytes)
            {
                throw new InvalidDataException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Bundle entry '{0}' exceeds the {1} byte limit.",
                    entry.FullName,
                    maximumBytes));
            }

            using Stream stream = entry.Open();
            using MemoryStream buffer = new MemoryStream((int)Math.Min(entry.Length, int.MaxValue));
            byte[] chunk = new byte[81920];
            long total = 0;
            int read;
            while ((read = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                total += read;
                if (total > maximumBytes)
                {
                    throw new InvalidDataException("Bundle entry expanded beyond the allowed size.");
                }

                buffer.Write(chunk, 0, read);
            }

            return buffer.ToArray();
        }

        private static OpenVisionRecipeReviewBundleInspection Failure(string packagePath, string detail)
        {
            return new OpenVisionRecipeReviewBundleInspection(
                packagePath,
                string.Empty,
                Local("검토 번들 dry-run: NG", "Review bundle dry-run: NG") + Environment.NewLine + detail,
                Local("무결성 오류를 해결한 뒤 번들을 다시 선택하세요.", "Fix the integrity issue, then select the bundle again."),
                Array.Empty<OpenVisionRecipeReviewBundlePathReview>());
        }

        private static string ComputeSha256(byte[] content)
        {
            return Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
        }

        private static string ComputeFileSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using SHA256 sha256 = SHA256.Create();
            return Convert.ToHexString(sha256.ComputeHash(stream)).ToLowerInvariant();
        }

        private static string NormalizePath(string value)
        {
            return (value ?? string.Empty).Trim().Trim('"');
        }

        private static string FormatValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string Local(string korean, string english)
        {
            return OpenVisionRecipeText.Local(korean, english);
        }

        private sealed class ReviewManifest
        {
            public ReviewManifest()
            {
            }

            public string Format { get; set; }

            public int SchemaVersion { get; set; }

            public string ApplicationVersion { get; set; }

            public string PipelineXmlSchema { get; set; }

            public ReviewPackagePolicy PackagePolicy { get; set; }

            public ReviewPipelineXmlRecord PipelineXml { get; set; }

            public ReviewValidationRecord Validation { get; set; }

            public ReviewSummaryRecord Summary { get; set; }

            public List<ReviewFileRecord> Dependencies { get; set; }

            public List<ReviewFileRecord> References { get; set; }
        }

        private sealed class ReviewPackagePolicy
        {
            public ReviewPackagePolicy()
            {
            }

            public bool ReferencedFilesCopied { get; set; }

            public bool PrivateLocalAssetsCopied { get; set; }

            public bool ImportExecuted { get; set; }

            public bool PreviewOrRunExecuted { get; set; }
        }

        private sealed class ReviewPipelineXmlRecord
        {
            public ReviewPipelineXmlRecord()
            {
            }

            public string Entry { get; set; }

            public long SizeBytes { get; set; }

            public string Sha256 { get; set; }
        }

        private sealed class ReviewValidationRecord
        {
            public ReviewValidationRecord()
            {
            }

            public string Status { get; set; }
        }

        private sealed class ReviewSummaryRecord
        {
            public ReviewSummaryRecord()
            {
            }

            public int StepCount { get; set; }

            public int DependencyCount { get; set; }
        }

        private sealed class ReviewFileRecord
        {
            public ReviewFileRecord()
            {
            }

            public string Name { get; set; }

            public string StepName { get; set; }

            public string ParameterName { get; set; }

            public string SourcePath { get; set; }

            public long? SizeBytes { get; set; }

            public string Sha256 { get; set; }
        }
    }
}
