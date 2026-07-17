using Markdig;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenVisionLab
{
    internal static class OpenVisionWorkspaceLearnDocumentService
    {
        private const string HtmlCacheVersion = "v1";
        private static readonly MarkdownPipeline HtmlPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        private static readonly Regex MarkdownDocumentLinkRegex = new Regex(
            "href=\"(?<name>[A-Za-z0-9_.-]+)\\.md(?<fragment>#[^\"]*)?\"",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

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
            if (!TryCreateHtmlDocument(sample, learnPath, out string documentPath))
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
            if (!TryResolveLearnDocumentFile(fileName, out string markdownPath)
                || !TryCreateHtmlDocument(markdownPath, out string documentPath))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = documentPath,
                UseShellExecute = true
            });
        }

        public static bool TryCreateHtmlDocument(
            VisionPipelineSampleCatalogItem sample,
            OpenVisionWorkspaceSampleLearnPathOption learnPath,
            out string documentPath)
        {
            if (!TryResolveDocumentPath(sample, learnPath, out string markdownPath))
            {
                documentPath = string.Empty;
                return false;
            }

            return TryCreateHtmlDocument(markdownPath, out documentPath);
        }

        private static bool TryCreateHtmlDocument(string markdownPath, out string documentPath)
        {
            if (string.IsNullOrWhiteSpace(markdownPath) || !File.Exists(markdownPath))
            {
                documentPath = string.Empty;
                return false;
            }

            string learnDirectory = Path.GetDirectoryName(markdownPath) ?? string.Empty;
            string outputDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OpenVisionLab",
                "LearnHtml",
                HtmlCacheVersion);
            Directory.CreateDirectory(outputDirectory);

            foreach (string sourcePath in Directory.EnumerateFiles(learnDirectory, "*.md", SearchOption.TopDirectoryOnly))
            {
                string outputPath = Path.Combine(
                    outputDirectory,
                    Path.GetFileNameWithoutExtension(sourcePath) + ".html");
                if (!File.Exists(outputPath)
                    || File.GetLastWriteTimeUtc(outputPath) < File.GetLastWriteTimeUtc(sourcePath))
                {
                    File.WriteAllText(
                        outputPath,
                        BuildHtmlDocument(sourcePath, learnDirectory, outputDirectory),
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
            }

            documentPath = Path.Combine(
                outputDirectory,
                Path.GetFileNameWithoutExtension(markdownPath) + ".html");
            return File.Exists(documentPath);
        }

        private static string BuildHtmlDocument(string markdownPath, string learnDirectory, string outputDirectory)
        {
            string markdown = File.ReadAllText(markdownPath, Encoding.UTF8);
            string body = Markdown.ToHtml(markdown, HtmlPipeline);
            body = MarkdownDocumentLinkRegex.Replace(body, match =>
            {
                string targetPath = Path.Combine(outputDirectory, match.Groups["name"].Value + ".html");
                string fragment = match.Groups["fragment"].Value;
                return "href=\"" + new Uri(targetPath).AbsoluteUri + fragment + "\"";
            });

            string title = ResolveHtmlTitle(markdown, Path.GetFileNameWithoutExtension(markdownPath));
            string sourceBaseUri = new Uri(learnDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar).AbsoluteUri;
            return """
                <!doctype html>
                <html lang="ko">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <base href="{{BASE_URI}}">
                  <title>{{TITLE}}</title>
                  <style>
                    :root { color-scheme: light; font-family: "Segoe UI", "Noto Sans KR", Arial, sans-serif; }
                    * { box-sizing: border-box; }
                    body { margin: 0; background: #f3f6f7; color: #172126; font-size: 17px; line-height: 1.72; }
                    main { width: min(1080px, calc(100% - 32px)); margin: 24px auto; padding: 32px 44px 56px; background: #fff; border: 1px solid #cbd7da; }
                    h1, h2, h3 { color: #102a30; line-height: 1.3; }
                    h1 { margin-top: 0; padding-bottom: 14px; border-bottom: 3px solid #167a83; font-size: 2rem; }
                    h2 { margin-top: 2.2rem; padding-bottom: 8px; border-bottom: 1px solid #cbd7da; font-size: 1.5rem; }
                    h3 { margin-top: 1.7rem; font-size: 1.2rem; }
                    a { color: #006b75; font-weight: 600; text-decoration-thickness: 1px; }
                    a:hover { color: #9b4f00; }
                    code { padding: 0.12rem 0.35rem; background: #edf2f3; color: #183a42; border-radius: 3px; }
                    pre { overflow: auto; padding: 18px; background: #17292e; color: #eefafa; border-left: 4px solid #d38a26; }
                    pre code { padding: 0; background: transparent; color: inherit; }
                    blockquote { margin: 1.4rem 0; padding: 12px 18px; background: #eef7f7; border-left: 4px solid #167a83; }
                    table { width: 100%; border-collapse: collapse; margin: 1.4rem 0; font-size: 0.95rem; }
                    th, td { padding: 10px 12px; border: 1px solid #b9c9cd; text-align: left; vertical-align: top; }
                    th { background: #e8f0f1; color: #18343a; }
                    img { display: block; max-width: 100%; height: auto; margin: 18px auto; border: 1px solid #b9c9cd; }
                    li { margin: 0.35rem 0; }
                    hr { border: 0; border-top: 1px solid #cbd7da; margin: 2rem 0; }
                    @media (max-width: 720px) { body { font-size: 16px; } main { width: 100%; margin: 0; padding: 22px 18px 40px; border: 0; } }
                  </style>
                </head>
                <body><main>{{BODY}}</main></body>
                </html>
                """
                .Replace("{{BASE_URI}}", WebUtility.HtmlEncode(sourceBaseUri), StringComparison.Ordinal)
                .Replace("{{TITLE}}", WebUtility.HtmlEncode(title), StringComparison.Ordinal)
                .Replace("{{BODY}}", body, StringComparison.Ordinal);
        }

        private static string ResolveHtmlTitle(string markdown, string fallback)
        {
            using StringReader reader = new StringReader(markdown ?? string.Empty);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("# ", StringComparison.Ordinal))
                {
                    return line.Substring(2).Trim();
                }
            }

            return string.IsNullOrWhiteSpace(fallback) ? "OpenVisionLab Learn" : fallback;
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

            if (string.Equals(id, "edge-detection", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_EDGE_DETECTION.md";
            }

            if (string.Equals(id, "preprocess", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_THRESHOLD.md";
            }

            if (string.Equals(id, "threshold", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_THRESHOLD.md";
            }

            if (string.Equals(id, "filter", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_FILTER.md";
            }

            if (string.Equals(id, "morphology", StringComparison.OrdinalIgnoreCase))
            {
                return "LEARN_MORPHOLOGY.md";
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
