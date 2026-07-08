using System;
using System.Diagnostics;
using System.IO;

namespace OpenVisionLab
{
    internal static class OpenVisionWorkspaceLearnDocumentService
    {
        public static bool TryResolveDocumentPath(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionWorkspaceSampleLearnPathOption learnPath,
            out string documentPath)
        {
            string fileName = ResolveDocumentFileName(sample, learnPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                documentPath = string.Empty;
                return false;
            }

            return TryResolveLearnDocumentFile(fileName, out documentPath);
        }

        public static bool TryResolveLearnDocumentFile(string fileName, out string documentPath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal))
            {
                documentPath = string.Empty;
                return false;
            }

            documentPath = Path.Combine(ResolveRepositoryRoot(), "docs", "learn", fileName);
            return File.Exists(documentPath);
        }

        public static string ResolveDocumentTitle(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionWorkspaceSampleLearnPathOption learnPath)
        {
            string fileName = ResolveDocumentFileName(sample, learnPath);
            return fileName switch
            {
                "LEARN_PRODUCT_SAMPLES.md" => LocalText("제품군 샘플 가이드", "Product Sample Guide"),
                "LEARN_MATCHING.md" => LocalText("Matching 배우기", "Learn Matching"),
                "LEARN_BLOB.md" => LocalText("Blob으로 얼룩/입자 찾기", "Learn Blob"),
                "LEARN_CONTOUR.md" => LocalText("Contour로 형상/개수 검증", "Learn Contour"),
                "LEARN_THRESHOLD.md" => LocalText("Threshold로 밝기 구간 검증", "Learn Threshold"),
                "LEARN_ARITHMETIC.md" => LocalText("Arithmetic로 산술/논리 연산 배우기", "Learn Arithmetic"),
                "LEARN_GEOMETRY_TRANSFORM.md" => LocalText("RotateScale로 기하 변환 배우기", "Learn Geometry Transform"),
                "LEARN_FILTER.md" => LocalText("Filter로 노이즈/경계 준비", "Learn Filter"),
                "LEARN_MORPHOLOGY.md" => LocalText("Morphology로 binary 정리", "Learn Morphology"),
                "LEARN_COLOR_HSV.md" => LocalText("Color/HSV로 색상 처리 배우기", "Learn Color / HSV"),
                "LEARN_MEAN.md" => LocalText("Mean으로 밝기 변화 검증", "Learn Mean"),
                "LEARN_EDGE_DETECTION.md" => LocalText("EdgeDetection으로 경계 검출 배우기", "Learn Edge Detection"),
                "LEARN_FEATURE_MATCHING.md" => LocalText("Feature Matching 배우기", "Learn Feature Matching"),
                "LEARN_EDGE_BASED_MATCHING.md" => LocalText("Edge Based Matching 배우기", "Learn Edge Based Matching"),
                "LEARN_LINE.md" => LocalText("Line으로 거리/각도 측정", "Learn Line"),
                _ => LocalText("Learn 문서 모음", "Learn Guide Index")
            };
        }

        public static void OpenDocument(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionWorkspaceSampleLearnPathOption learnPath)
        {
            if (!TryResolveDocumentPath(sample, learnPath, out string documentPath))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = documentPath,
                UseShellExecute = true
            });
        }

        public static void OpenLearnDocumentFile(string fileName)
        {
            if (!TryResolveLearnDocumentFile(fileName, out string documentPath))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = documentPath,
                UseShellExecute = true
            });
        }

        private static string ResolveDocumentFileName(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionWorkspaceSampleLearnPathOption learnPath)
        {
            string sampleText = string.Join(
                " ",
                sample?.ToolFlowText,
                sample?.Category,
                sample?.SampleName,
                sample?.Goal);

            if (sample != null
                && sample.CatalogSourceKind == VisionPipelineSampleCatalogSourceKind.Product)
            {
                return "LEARN_PRODUCT_SAMPLES.md";
            }

            if (Contains(sampleText, "EdgeBased"))
            {
                return "LEARN_EDGE_BASED_MATCHING.md";
            }

            if (Contains(sampleText, "Feature"))
            {
                return "LEARN_FEATURE_MATCHING.md";
            }

            if (Contains(sampleText, "EdgeDetection"))
            {
                return "LEARN_EDGE_DETECTION.md";
            }

            if (Contains(sampleText, "Filter"))
            {
                return "LEARN_FILTER.md";
            }

            if (Contains(sampleText, "Morphology"))
            {
                return "LEARN_MORPHOLOGY.md";
            }

            if (Contains(sampleText, "Threshold"))
            {
                return "LEARN_THRESHOLD.md";
            }

            if (Contains(sampleText, "Arithmetic")
                || Contains(sampleText, "AbsDiff")
                || Contains(sampleText, "Bitwise"))
            {
                return "LEARN_ARITHMETIC.md";
            }

            if (Contains(sampleText, "RotateScale")
                || Contains(sampleText, "RotateAndScale")
                || Contains(sampleText, "Transform"))
            {
                return "LEARN_GEOMETRY_TRANSFORM.md";
            }

            if (Contains(sampleText, "Matching"))
            {
                return "LEARN_MATCHING.md";
            }

            if (Contains(sampleText, "Blob"))
            {
                return "LEARN_BLOB.md";
            }

            if (Contains(sampleText, "Contour"))
            {
                return "LEARN_CONTOUR.md";
            }

            if (Contains(sampleText, "LineDistance")
                || Contains(sampleText, "LineGauge")
                || Contains(sampleText, "Distance"))
            {
                return "LEARN_LINE.md";
            }

            if (Contains(sampleText, "HSV")
                || Contains(sampleText, "Histogram")
                || Contains(sampleText, "Color range"))
            {
                return "LEARN_COLOR_HSV.md";
            }

            if (Contains(sampleText, "Mean"))
            {
                return "LEARN_MEAN.md";
            }

            string id = learnPath?.Id?.Trim() ?? string.Empty;
            if (string.Equals(id, "matching", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_MATCHING.md";
            }

            if (string.Equals(id, "template-matching", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_MATCHING.md";
            }

            if (string.Equals(id, "edge-matching", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_EDGE_BASED_MATCHING.md";
            }

            if (string.Equals(id, "feature-matching", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_FEATURE_MATCHING.md";
            }

            if (string.Equals(id, "blob", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_BLOB.md";
            }

            if (string.Equals(id, "contour", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_CONTOUR.md";
            }

            if (string.Equals(id, "preprocess", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_THRESHOLD.md";
            }

            if (string.Equals(id, "geometry", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_GEOMETRY_TRANSFORM.md";
            }

            if (string.Equals(id, "line", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_LINE.md";
            }

            if (string.Equals(id, "color-hsv", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_COLOR_HSV.md";
            }

            if (string.Equals(id, "mean", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_MEAN.md";
            }

            return "README.md";
        }

        private static string ResolveRepositoryRoot()
        {
            string[] candidates =
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory()
            };

            foreach (string candidate in candidates)
            {
                string current = candidate;
                while (!string.IsNullOrWhiteSpace(current))
                {
                    if (File.Exists(Path.Combine(current, "OpenVisionLab.sln")))
                    {
                        return current;
                    }

                    current = Directory.GetParent(current)?.FullName;
                }
            }

            return Directory.GetCurrentDirectory();
        }

        private static bool Contains(string text, string token)
        {
            return !string.IsNullOrWhiteSpace(text)
                && !string.IsNullOrWhiteSpace(token)
                && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
