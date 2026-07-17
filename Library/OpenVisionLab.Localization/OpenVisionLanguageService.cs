using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenVisionLab
{
    public enum OpenVisionLanguage
    {
        Korean,
        English
    }

    public static class OpenVisionLanguageService
    {
        private const string ConfigDirectoryName = "CONFIG";
        private const string CatalogFileName = "localization_catalog.tsv";
        private const string LanguageFileName = "language.txt";

        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<string, OpenVisionLocalizationEntry> Entries = new Dictionary<string, OpenVisionLocalizationEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DefaultCatalogMigration> DefaultCatalogMigrations = new Dictionary<string, DefaultCatalogMigration>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "VisionTest.OutputLayer",
                new DefaultCatalogMigration(
                    "출력 레이어",
                    "Output Layer")
            },
            {
                "VisionTest.CreateOutputLayer",
                new DefaultCatalogMigration(
                    "새 출력 레이어를 생성합니다.",
                    "Create Output Layer")
            },
            {
                "Pipeline.WorkflowHint",
                new DefaultCatalogMigration(
                    "Preview는 여기에서만 확인하고, Publish Result가 메인 작업영역을 업데이트합니다.",
                    "Run Preview stays here; Publish Result updates the main workspace.")
            },
            {
                "Pipeline.NewStepTool",
                new DefaultCatalogMigration(
                    "새 Step Tool",
                    "New Step Tool")
            },
            {
                "Menu.Settings",
                new DefaultCatalogMigration(
                    "다국어 편집",
                    "Localization Editor")
            },
            {
                "PipelineSamples.btnOpenCatalog.Text",
                new DefaultCatalogMigration(
                    "열기 + Preview",
                    "Open + Preview")
            },
            {
                "PipelineSamples.btnOpenCatalog.ToolTip",
                new DefaultCatalogMigration(
                    "선택한 샘플 이미지와 파이프라인을 열고 Preview를 실행합니다.",
                    "Open the selected sample image and pipeline, then run Preview.")
            },
            {
                "PipelineSamples.OpenPreviewAction",
                new DefaultCatalogMigration(
                    "동작: 열기 + Preview는 이 이미지와 레시피를 불러온 뒤 Pipeline에서 Preview를 실행합니다.",
                    "Action: Open + Preview loads this image and recipe, then runs preview in Pipeline.")
            },
            {
                "Shell.WorkspaceEmptyGuideButton",
                new DefaultCatalogMigration(
                    "가이드 보기",
                    "View Guide")
            },
            {
                "Shell.WorkspaceEmptyDetail",
                new DefaultCatalogMigration(
                    "이미지를 로드하거나 샘플을 열어 테스트를 시작하십시오.",
                    "Load an image or open a sample to start testing.")
            },
            {
                "Shell.WorkspaceEmptyStepLoadTitle",
                new DefaultCatalogMigration(
                    "1. 이미지 로드",
                    "1. Load image")
            },
            {
                "Shell.WorkspaceEmptyStepLoadDetail",
                new DefaultCatalogMigration(
                    "Main 레이어에 검사할 이미지를 엽니다.",
                    "Open the image to inspect into the Main layer.")
            },
            {
                "Shell.WorkspaceEmptyStepSelectTitle",
                new DefaultCatalogMigration(
                    "2. 도구 선택",
                    "2. Select tool")
            },
            {
                "Shell.WorkspaceEmptyStepSelectDetail",
                new DefaultCatalogMigration(
                    "왼쪽 목록에서 스레시홀드, 매칭, 라인 같은 도구를 선택합니다.",
                    "Choose Threshold, Matching, Line, or another tool from the left list.")
            },
            {
                "Shell.WorkspaceEmptyStepPreviewTitle",
                new DefaultCatalogMigration(
                    "3. 미리보기 확인",
                    "3. Check preview")
            },
            {
                "Shell.WorkspaceEmptyStepPreviewDetail",
                new DefaultCatalogMigration(
                    "결과를 확인한 뒤 검증된 Step을 파이프라인에 추가합니다.",
                    "Check the result, then add the verified step to the pipeline.")
            },
            {
                "Shell.WorkspaceEmptyLogHint",
                new DefaultCatalogMigration(
                    "이미지를 불러오면 이 영역에서 확대, 이동, 픽셀 값을 확인하고 하단 실행 로그에서 상태를 추적합니다.",
                    "After loading an image, use this area for zoom, pan, pixel values, and the Run Log for state tracking.")
            },
            {
                "Shell.WorkspaceStatus.SampleRoute",
                new DefaultCatalogMigration(
                    "Pipeline Review \uB610\uB294 \uCCAB Step \uC5F4\uAE30",
                    "Open Pipeline Review or the first step")
            },
            // Legacy CONFIG values with implementation terms are kept only for migration to neutral Shell.Ready* defaults.
            {
                "Shell.ReadyFoundation",
                new DefaultCatalogMigration(
                    "준비 | WPF 셸 기반",
                    "Ready | WPF shell foundation")
            },
            {
                "Shell.ReadySelectedFormat",
                new DefaultCatalogMigration(
                    "준비 | WPF 셸 기반 | 선택: {0}",
                    "Ready | WPF shell foundation | Selected: {0}")
            },
            {
                "Shell.RouteEmpty",
                new DefaultCatalogMigration(
                    "경로: Main -> -",
                    "Route: Main -> -")
            },
            {
                "Shell.RouteFormat",
                new DefaultCatalogMigration(
                    "경로: Main -> {0}",
                    "Route: Main -> {0}")
            },
            {
                "Shell.PendingTool.NavStatus",
                new DefaultCatalogMigration(
                    "\uC804\uD658",
                    "WPF")
            },
            {
                "Shell.PendingTool.NavDescription",
                new DefaultCatalogMigration(
                    "WPF \uD654\uBA74 \uC804\uD658 \uB300\uC0C1\uC785\uB2C8\uB2E4.",
                    "WPF view migration target.")
            },
            {
                "Shell.PendingTool.Status",
                new DefaultCatalogMigration(
                    "WPF \uC804\uD658 \uB300\uC0C1",
                    "WPF migration target")
            },
            {
                "Shell.PendingTool.Message",
                new DefaultCatalogMigration(
                    "\uC774 \uB3C4\uAD6C\uB294 WPF \uC804\uC6A9 \uD654\uBA74\uC73C\uB85C \uC804\uD658 \uC911\uC785\uB2C8\uB2E4. \uC804\uD658 \uC644\uB8CC \uD6C4 \uC774 \uCC3D\uC5D0\uC11C \uBC14\uB85C \uC2E4\uD589\uB429\uB2C8\uB2E4.",
                    "This tool is being moved to a WPF-only view. It will run from this window after migration.")
            },
            {
                "PipelineReview.Guide.OkNext",
                new DefaultCatalogMigration(
                    "\uCD9C\uB825 \uC774\uBBF8\uC9C0 \uD655\uC778 \uD6C4 \uB2E4\uC74C Step\uC73C\uB85C \uC9C4\uD589",
                    "Check the output image, then continue to the next step")
            },
            {
                "PipelineReview.Guide.OkFinalNext",
                new DefaultCatalogMigration(
                    "\uCD9C\uB825 \uC774\uBBF8\uC9C0, \uC9C0\uD45C, Good/Bad \uC30D\uC744 \uBE44\uAD50\uD55C \uB4A4 Pipeline \uC2B9\uC778",
                    "Compare output, metrics, and the Good/Bad pair before accepting the pipeline")
            },
            {
                "PipelineReview.Guide.NgNext",
                new DefaultCatalogMigration(
                    "Tool \uD30C\uB77C\uBBF8\uD130 \uB610\uB294 \uB77C\uC6B0\uD2B8 \uC870\uC815 \uD6C4 \uB2E4\uC2DC \uB9AC\uBDF0",
                    "Adjust the tool parameters or route, then run review again")
            },
            {
                "PipelineReview.Metric.MeanValueAvg",
                new DefaultCatalogMigration(
                    "\uD3C9\uADE0 \uBC1D\uAE30",
                    "Mean Avg")
            },
            {
                "PipelineReview.Metric.DistanceMmAvg",
                new DefaultCatalogMigration(
                    "\uD3C9\uADE0 \uAC70\uB9AC(mm)",
                    "Distance Avg (mm)")
            },
            {
                "PropertyGrid.Type.FeatureMatchingProperty.SCORE_MIN.DisplayName",
                new DefaultCatalogMigration(
                    "\uCD5C\uC18C \uB9E4\uCE6D \uC810\uC218",
                    "Min match score")
            },
            {
                "PropertyGrid.Type.FeatureMatchingProperty.SCORE_MIN.Description",
                new DefaultCatalogMigration(
                    "\uD2B9\uC9D5 \uB9E4\uCE6D \uACB0\uACFC\uB85C \uC778\uC815\uD560 \uCD5C\uC18C \uC810\uC218\uC785\uB2C8\uB2E4. \uB192\uC744\uC218\uB85D \uC57D\uD55C \uD2B9\uC9D5\uC810 \uB9E4\uCE6D\uC744 \uB354 \uC5C4\uACA9\uD558\uAC8C \uC81C\uC678\uD569\uB2C8\uB2E4.",
                    "Minimum score accepted as a feature match. Higher values reject weak feature matches more strictly.")
            },
            {
                "VisionTool.Preset.FeatureMatching.Fast.Description",
                new DefaultCatalogMigration(
                    "\uBE60\uB978 \uD2B9\uC9D5 \uD655\uC778: \uB354 \uC5C4\uACA9\uD55C Ratio\uC640 \uB113\uC740 \uAE30\uD558 \uD5C8\uC6A9 \uC624\uCC28",
                    "Stricter feature ratio with wider geometry tolerance for quick checks.")
            },
            {
                "VisionTool.Preset.FeatureMatching.Precise.Description",
                new DefaultCatalogMigration(
                    "\uC815\uBC00 \uD2B9\uC9D5 \uD655\uC778: \uB354 \uB9CE\uC740 \uD2B9\uC9D5 \uD6C4\uBCF4\uC640 \uC5C4\uACA9\uD55C \uAE30\uD558 \uAC80\uC99D",
                    "Looser feature ratio with tighter geometry validation for final tuning.")
            },
            {
                "PipelineReview.FixtureTeach.Waiting",
                new DefaultCatalogMigration(
                    "\uAE30\uC900 \uC774\uBBF8\uC9C0\uC5D0\uC11C \uB9AC\uBDF0 \uC2E4\uD589 \uD6C4 Matching 1\uAC1C\uB97C \uD655\uC778\uD558\uC2ED\uC2DC\uC624. \uC18C\uBE44 Step ROI\uB294 \uBCC0\uACBD\uB418\uC9C0 \uC54A\uC2B5\uB2C8\uB2E4.",
                    "Run Review on the reference image and verify one match. Consumer ROI stays unchanged.")
            },
            {
                "PipelineReview.FixtureTeach.ReadyFormat",
                new DefaultCatalogMigration(
                    "\uAC80\uD1A0 \uC790\uC138 \uC900\uBE44: X {0}, Y {1}, \uAC01\uB3C4 {2}\uB3C4. \uAE30\uC900 \uC774\uBBF8\uC9C0\uC5D0\uC11C\uB9CC \uC800\uC7A5\uD558\uC2ED\uC2DC\uC624.",
                    "Reviewed pose ready: X {0}, Y {1}, angle {2} deg. Save only on the reference image.")
            },
            {
                "PipelineReview.FixtureTeach.SavedFormat",
                new DefaultCatalogMigration(
                    "\uCC38\uC870 \uC800\uC7A5: X {0}, Y {1}, \uAC01\uB3C4 {2}\uB3C4. \uC18C\uBE44 Step ROI\uB294 \uBCC0\uACBD\uD558\uC9C0 \uC54A\uC558\uC2B5\uB2C8\uB2E4. \uB2E4\uC2DC \uB9AC\uBDF0 \uC2E4\uD589\uD558\uC2ED\uC2DC\uC624.",
                    "Reference saved: X {0}, Y {1}, angle {2} deg. Consumer ROI was not changed. Run Review again.")
            },
            {
                "PipelineReview.FixtureTeach.RunRequired",
                new DefaultCatalogMigration(
                    "\uCC38\uC870 \uC800\uC7A5\uB428 / \uB9AC\uBDF0 \uC7AC\uC2E4\uD589 \uD544\uC694",
                    "Reference saved / run review required")
            }
        };
        private static bool loaded;

        public static event EventHandler LanguageChanged;

        public static OpenVisionLanguage CurrentLanguage { get; private set; } = OpenVisionLanguage.Korean;

        public static string CatalogPath => Path.Combine(GetConfigDirectory(), CatalogFileName);

        public static void Load()
        {
            EnsureCatalogFile();
            LoadCatalog();
            LoadLanguage();
        }

        public static void ReloadCatalog(bool notify = true)
        {
            EnsureCatalogFile();
            LoadCatalog();
            if (notify)
            {
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void SetLanguage(OpenVisionLanguage language, bool save = true)
        {
            if (CurrentLanguage == language)
            {
                return;
            }

            CurrentLanguage = language;
            if (save)
            {
                SaveLanguage(language);
            }

            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static string T(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            EnsureLoaded();
            lock (SyncRoot)
            {
                if (!Entries.TryGetValue(key, out OpenVisionLocalizationEntry entry))
                {
                    return key;
                }

                string text = CurrentLanguage == OpenVisionLanguage.English ? entry.English : entry.Korean;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }

                string fallback = CurrentLanguage == OpenVisionLanguage.English ? entry.Korean : entry.English;
                return string.IsNullOrWhiteSpace(fallback) ? key : fallback;
            }
        }

        public static bool TryT(string key, out string text)
        {
            text = string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            EnsureLoaded();
            lock (SyncRoot)
            {
                if (!Entries.TryGetValue(key, out OpenVisionLocalizationEntry entry))
                {
                    return false;
                }

                text = CurrentLanguage == OpenVisionLanguage.English ? entry.English : entry.Korean;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return true;
                }

                text = CurrentLanguage == OpenVisionLanguage.English ? entry.Korean : entry.English;
                return !string.IsNullOrWhiteSpace(text);
            }
        }

        public static IReadOnlyList<OpenVisionLocalizationEntry> GetEntries()
        {
            EnsureLoaded();
            lock (SyncRoot)
            {
                return Entries.Values
                    .OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(entry => new OpenVisionLocalizationEntry
                    {
                        Key = entry.Key,
                        Korean = entry.Korean,
                        English = entry.English
                    })
                    .ToList();
            }
        }

        public static void SaveEntries(IEnumerable<OpenVisionLocalizationEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            List<OpenVisionLocalizationEntry> normalized = entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry?.Key))
                .Select(entry => new OpenVisionLocalizationEntry
                {
                    Key = entry.Key.Trim(),
                    Korean = entry.Korean ?? string.Empty,
                    English = entry.English ?? string.Empty
                })
                .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.Last())
                .OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Directory.CreateDirectory(GetConfigDirectory());
            File.WriteAllText(CatalogPath, BuildCatalogText(normalized), Encoding.UTF8);
            LoadCatalog();
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static IReadOnlyList<OpenVisionLanguageOption> GetLanguageOptions()
        {
            return new[]
            {
                new OpenVisionLanguageOption(OpenVisionLanguage.Korean, "\uD55C\uAD6D\uC5B4"),
                new OpenVisionLanguageOption(OpenVisionLanguage.English, "English")
            };
        }

        private static void EnsureCatalogFile()
        {
            string path = CatalogPath;
            Directory.CreateDirectory(GetConfigDirectory());
            string defaultCatalog = ReadEmbeddedCatalog();
            if (!File.Exists(path))
            {
                File.WriteAllText(path, defaultCatalog, Encoding.UTF8);
                return;
            }

            Dictionary<string, OpenVisionLocalizationEntry> currentEntries = ParseCatalogText(File.ReadAllText(path, Encoding.UTF8));
            Dictionary<string, OpenVisionLocalizationEntry> defaultEntries = ParseCatalogText(defaultCatalog);
            bool changed = false;

            foreach (KeyValuePair<string, OpenVisionLocalizationEntry> pair in defaultEntries)
            {
                if (currentEntries.TryGetValue(pair.Key, out OpenVisionLocalizationEntry currentEntry))
                {
                    if (TryMigrateDefaultCatalogValue(pair.Key, currentEntry, pair.Value))
                    {
                        changed = true;
                    }

                    continue;
                }

                currentEntries[pair.Key] = pair.Value;
                changed = true;
            }

            if (changed)
            {
                File.WriteAllText(path, BuildCatalogText(currentEntries.Values.OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)), Encoding.UTF8);
            }
        }

        private static bool TryMigrateDefaultCatalogValue(string key, OpenVisionLocalizationEntry currentEntry, OpenVisionLocalizationEntry defaultEntry)
        {
            if (currentEntry == null || defaultEntry == null)
            {
                return false;
            }

            if (!DefaultCatalogMigrations.TryGetValue(key, out DefaultCatalogMigration migration))
            {
                return false;
            }

            if (!string.Equals(currentEntry.Korean, migration.OldKorean, StringComparison.Ordinal)
                || !string.Equals(currentEntry.English, migration.OldEnglish, StringComparison.Ordinal))
            {
                return false;
            }

            currentEntry.Korean = defaultEntry.Korean;
            currentEntry.English = defaultEntry.English;
            return true;
        }

        private static void LoadCatalog()
        {
            Dictionary<string, OpenVisionLocalizationEntry> loaded = File.Exists(CatalogPath)
                ? ParseCatalogText(File.ReadAllText(CatalogPath, Encoding.UTF8))
                : ParseCatalogText(ReadEmbeddedCatalog());

            lock (SyncRoot)
            {
                Entries.Clear();
                foreach (KeyValuePair<string, OpenVisionLocalizationEntry> pair in loaded)
                {
                    Entries[pair.Key] = pair.Value;
                }

                OpenVisionLanguageService.loaded = true;
            }
        }

        private static void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }

            Load();
        }

        private static string ReadEmbeddedCatalog()
        {
            Assembly assembly = typeof(OpenVisionLanguageService).Assembly;
            string resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith("LocalizationCatalog.tsv", StringComparison.OrdinalIgnoreCase));
            if (resourceName == null)
            {
                return "Key\tKorean\tEnglish\r\n";
            }

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static Dictionary<string, OpenVisionLocalizationEntry> ParseCatalogText(string catalogText)
        {
            Dictionary<string, OpenVisionLocalizationEntry> loaded = new Dictionary<string, OpenVisionLocalizationEntry>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(catalogText))
            {
                return loaded;
            }

            string[] lines = catalogText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split('\t');
                if (parts.Length < 1 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                string key = Unescape(parts[0]).Trim();
                loaded[key] = new OpenVisionLocalizationEntry
                {
                    Key = key,
                    Korean = parts.Length > 1 ? Unescape(parts[1]) : string.Empty,
                    English = parts.Length > 2 ? Unescape(parts[2]) : string.Empty
                };
            }

            return loaded;
        }

        private static string BuildCatalogText(IEnumerable<OpenVisionLocalizationEntry> entries)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Key\tKorean\tEnglish");
            foreach (OpenVisionLocalizationEntry entry in entries)
            {
                builder
                    .Append(Escape(entry.Key))
                    .Append('\t')
                    .Append(Escape(entry.Korean))
                    .Append('\t')
                    .Append(Escape(entry.English))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static void LoadLanguage()
        {
            try
            {
                string path = GetLanguagePath();
                if (!File.Exists(path))
                {
                    return;
                }

                if (TryParseLanguage(File.ReadAllText(path).Trim(), out OpenVisionLanguage language))
                {
                    CurrentLanguage = language;
                }
            }
            catch
            {
                CurrentLanguage = OpenVisionLanguage.Korean;
            }
        }

        private static void SaveLanguage(OpenVisionLanguage language)
        {
            try
            {
                Directory.CreateDirectory(GetConfigDirectory());
                File.WriteAllText(GetLanguagePath(), language == OpenVisionLanguage.English ? "en" : "ko", Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static bool TryParseLanguage(string text, out OpenVisionLanguage language)
        {
            language = OpenVisionLanguage.Korean;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (text.Equals("ko", StringComparison.OrdinalIgnoreCase)
                || text.Equals("kor", StringComparison.OrdinalIgnoreCase)
                || text.Equals("korean", StringComparison.OrdinalIgnoreCase)
                || text.Equals("한국어", StringComparison.OrdinalIgnoreCase))
            {
                language = OpenVisionLanguage.Korean;
                return true;
            }

            if (text.Equals("en", StringComparison.OrdinalIgnoreCase)
                || text.Equals("eng", StringComparison.OrdinalIgnoreCase)
                || text.Equals("english", StringComparison.OrdinalIgnoreCase))
            {
                language = OpenVisionLanguage.English;
                return true;
            }

            return Enum.TryParse(text, true, out language);
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n")
                .Replace("\t", " ");
        }

        private static string Unescape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\n", Environment.NewLine)
                .Replace("\\\\", "\\");
        }

        private static string GetConfigDirectory()
        {
            return Path.Combine(
                AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                ConfigDirectoryName);
        }

        private static string GetLanguagePath()
        {
            return Path.Combine(GetConfigDirectory(), LanguageFileName);
        }
    }

    public sealed class OpenVisionLanguageOption
    {
        public OpenVisionLanguageOption(OpenVisionLanguage language, string displayName)
        {
            Language = language;
            DisplayName = displayName ?? string.Empty;
        }

        public OpenVisionLanguage Language { get; }

        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    internal sealed class DefaultCatalogMigration
    {
        public DefaultCatalogMigration(string oldKorean, string oldEnglish)
        {
            OldKorean = oldKorean ?? string.Empty;
            OldEnglish = oldEnglish ?? string.Empty;
        }

        public string OldKorean { get; }

        public string OldEnglish { get; }
    }
}
