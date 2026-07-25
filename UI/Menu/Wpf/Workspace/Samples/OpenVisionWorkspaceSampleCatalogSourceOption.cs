using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenVisionLab
{
    internal sealed class OpenVisionWorkspaceSampleCatalogSourceOption
    {
        private OpenVisionWorkspaceSampleCatalogSourceOption(
            VisionPipelineSampleCatalogSourceKind sourceKind,
            string displayName,
            string description,
            int sampleCount)
        {
            SourceKind = sourceKind;
            DisplayName = displayName ?? string.Empty;
            Description = description ?? string.Empty;
            SampleCount = sampleCount;
        }

        public VisionPipelineSampleCatalogSourceKind SourceKind { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public int SampleCount { get; }

        public string Id => SourceKind switch
        {
            VisionPipelineSampleCatalogSourceKind.Public => "public",
            VisionPipelineSampleCatalogSourceKind.LocalLegacy => "local-legacy",
            VisionPipelineSampleCatalogSourceKind.Product => "product",
            _ => "unknown"
        };

        public string SampleCountText => string.Format(
            CultureInfo.CurrentCulture,
            LocalText("{0}개 샘플", "{0} samples"),
            SampleCount);

        public bool Matches(VisionPipelineSampleCatalogItem sample)
        {
            return sample != null && sample.CatalogSourceKind == SourceKind;
        }

        public static IReadOnlyList<OpenVisionWorkspaceSampleCatalogSourceOption> Create(
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples)
        {
            IReadOnlyList<VisionPipelineSampleCatalogItem> source = samples ?? Array.Empty<VisionPipelineSampleCatalogItem>();
            List<OpenVisionWorkspaceSampleCatalogSourceOption> options = new List<OpenVisionWorkspaceSampleCatalogSourceOption>();
            AddIfNotEmpty(
                options,
                source,
                VisionPipelineSampleCatalogSourceKind.Public,
                LocalText("공개 샘플", "Public"),
                LocalText(
                    "튜토리얼, README, 포트폴리오에 사용할 수 있는 프로젝트 작성 synthetic 샘플입니다.",
                    "GitHub-safe synthetic samples for tutorials, README, and portfolio evidence."));
            AddIfNotEmpty(
                options,
                source,
                VisionPipelineSampleCatalogSourceKind.Product,
                LocalText("제품군 샘플", "Product"),
                LocalText(
                    "이차전지, 디스플레이, 반도체 Good/Bad 흐름을 툴별로 확인하는 synthetic 샘플입니다.",
                    "Product-domain synthetic samples for checking battery, display, and semiconductor Good/Bad flows."));
            AddIfNotEmpty(
                options,
                source,
                VisionPipelineSampleCatalogSourceKind.LocalLegacy,
                LocalText("로컬 Legacy", "Local Legacy"),
                LocalText(
                    "로컬 SDK/Sample 폴더 참조입니다. 사용 권리가 확인될 때까지 개인 개발용으로만 사용합니다.",
                    "Local-only SDK/sample-folder references. Use only for private development until asset rights are cleared."));
            return options;
        }

        private static void AddIfNotEmpty(
            ICollection<OpenVisionWorkspaceSampleCatalogSourceOption> options,
            IReadOnlyList<VisionPipelineSampleCatalogItem> samples,
            VisionPipelineSampleCatalogSourceKind sourceKind,
            string displayName,
            string description)
        {
            int count = samples.Count(sample => sample != null && sample.CatalogSourceKind == sourceKind);
            if (count <= 0)
            {
                return;
            }

            options.Add(new OpenVisionWorkspaceSampleCatalogSourceOption(sourceKind, displayName, description, count));
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }
    }
}
