using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolRegistry
    {
        private static readonly Size LargeHostedToolSize = new Size(1180, 890);
        private static readonly Size StandardHostedToolSize = new Size(920, 660);

        // Native tool composition root: one row per menu keeps creation, sizing, and prewarm policy in sync.
        // New tools should pick one extension lane first; see docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md.
        // PropertyGrid inspection tools stay model-driven: edit the property model and let the grid bridge create the UI.
        private static readonly IReadOnlyList<OpenVisionNativeToolRegistration> Registrations =
            new[]
            {
                Tool(VISION_MENU.Threshold, OpenVisionNativeCustomToolFactory.CreateThreshold, nameof(ThresholdToolWpfView)),
                Tool(VISION_MENU.Filter, OpenVisionNativeCustomToolFactory.CreateFilter, nameof(FilterToolWpfView)),
                Tool(VISION_MENU.Morphology, OpenVisionNativeCustomToolFactory.CreateMorphology, nameof(MorphologyToolWpfView)),
                Tool(VISION_MENU.Arithmetic, OpenVisionNativeArithmeticDocumentFactory.Create, nameof(ArithmeticToolWpfView)),
                LargeTool(VISION_MENU.Blob, OpenVisionNativePropertyGridToolFactory.CreateBlob, nameof(BlobToolWpfView), warmHostedLayout: true),
                LargeTool(VISION_MENU.Contour, OpenVisionNativePropertyGridToolFactory.CreateContour, nameof(ContourToolWpfView), warmHostedLayout: true),
                LargeTool(VISION_MENU.Line, OpenVisionNativeCustomToolFactory.CreateLine, nameof(LineToolWpfView), warmHostedLayout: true),
                LargeTool(VISION_MENU.Matching, OpenVisionNativePropertyGridToolFactory.CreateMatching, nameof(MatchingToolWpfView), warmHostedLayout: true),
                LargeTool(VISION_MENU.EdgeBasedMatching, OpenVisionNativePropertyGridToolFactory.CreateEdgeBasedMatching, nameof(EdgeBasedMatchingToolWpfView), warmHostedLayout: true),
                LargeTool(VISION_MENU.FeatureMatching, OpenVisionNativePropertyGridToolFactory.CreateFeatureMatching, nameof(FeatureMatchingToolWpfView), warmHostedLayout: true),
                Tool(VISION_MENU.EdgeDetection, OpenVisionNativeSimplePreprocessDocumentFactory.CreateEdgeDetectionDocument, nameof(SimplePreprocessToolWpfView)),
                Tool(VISION_MENU.RotateAndScale, OpenVisionNativeSimplePreprocessDocumentFactory.CreateRotateScaleDocument, nameof(SimplePreprocessToolWpfView)),
                LargeTool(VISION_MENU.AffineTransform, OpenVisionNativePropertyGridToolFactory.CreateAffineTransform, nameof(AffineTransformToolWpfView), warmHostedLayout: true),
                Tool(VISION_MENU.HSV, OpenVisionNativeSimplePreprocessDocumentFactory.CreateHsvDocument, nameof(SimplePreprocessToolWpfView)),
                Tool(VISION_MENU.Mean, OpenVisionNativeSimplePreprocessDocumentFactory.CreateMeanDocument, nameof(SimplePreprocessToolWpfView)),
                Tool(VISION_MENU.Histogram, OpenVisionNativeSimplePreprocessDocumentFactory.CreateHistogramDocument, nameof(SimplePreprocessToolWpfView))
            };

        private static readonly IReadOnlyDictionary<VISION_MENU, OpenVisionNativeToolRegistration> RegistrationsByMenu =
            Registrations.ToDictionary(registration => registration.Menu);

        public static IEnumerable<VISION_MENU> GetPrewarmMenus()
        {
            // Prewarm cost is not uniform. Create the heavy PropertyGrid/inspection tools first so
            // operator clicks on algorithm tools are more likely to hit the cache during startup.
            yield return VISION_MENU.Blob;
            yield return VISION_MENU.Matching;
            yield return VISION_MENU.EdgeBasedMatching;
            yield return VISION_MENU.FeatureMatching;
            yield return VISION_MENU.Line;
            yield return VISION_MENU.Contour;

            foreach (OpenVisionNativeToolRegistration registration in Registrations)
            {
                VISION_MENU menu = registration.Menu;
                if (menu == VISION_MENU.Blob
                    || menu == VISION_MENU.Matching
                    || menu == VISION_MENU.EdgeBasedMatching
                    || menu == VISION_MENU.FeatureMatching
                    || menu == VISION_MENU.Line
                    || menu == VISION_MENU.Contour)
                {
                    continue;
                }

                yield return menu;
            }
        }

        public static bool IsRegistered(VISION_MENU menu)
        {
            return RegistrationsByMenu.ContainsKey(menu);
        }

        public static Size GetPreferredWindowSize(VISION_MENU menu)
        {
            return RegistrationsByMenu.TryGetValue(menu, out OpenVisionNativeToolRegistration registration)
                ? registration.PreferredWindowSize
                : StandardHostedToolSize;
        }

        public static Size GetLayoutWarmSize()
        {
            return LargeHostedToolSize;
        }

        public static bool ShouldWarmHostedLayout(string viewTypeName)
        {
            if (string.IsNullOrWhiteSpace(viewTypeName))
            {
                return false;
            }

            return Registrations.Any(registration =>
                registration.WarmHostedLayout
                && string.Equals(registration.ViewTypeName, viewTypeName, StringComparison.Ordinal));
        }

        public static bool TryCreateDocument(
            VISION_MENU menu,
            IDisplayManager displayManager,
            out OpenVisionNativeToolDocument document)
        {
            if (!RegistrationsByMenu.TryGetValue(menu, out OpenVisionNativeToolRegistration registration))
            {
                document = null;
                return false;
            }

            document = registration.CreateDocument(displayManager);
            string settingsConfigName =
                OpenVisionNativeToolSettingsStore.CreateConfigName(
                    document.ToolName);
            if (OpenVisionNativeToolSettingsStore.TryGetLoadFailure(
                    settingsConfigName,
                    out OpenVisionNativeToolSettingsLoadFailure loadFailure))
            {
                document.SetPropertyPersistenceStatus(
                    OpenVisionNativeToolPersistenceStatusText.CreateLoadFailure(
                        loadFailure.ToolName,
                        loadFailure.RecipeName,
                        loadFailure.ErrorMessage,
                        loadFailure.BackupPath));
            }

            return true;
        }

        private static OpenVisionNativeToolRegistration Tool(
            VISION_MENU menu,
            Func<IDisplayManager, OpenVisionNativeToolDocument> createDocument,
            string viewTypeName)
        {
            return new OpenVisionNativeToolRegistration(menu, createDocument, StandardHostedToolSize, viewTypeName, warmHostedLayout: false);
        }

        private static OpenVisionNativeToolRegistration LargeTool(
            VISION_MENU menu,
            Func<IDisplayManager, OpenVisionNativeToolDocument> createDocument,
            string viewTypeName,
            bool warmHostedLayout)
        {
            return new OpenVisionNativeToolRegistration(menu, createDocument, LargeHostedToolSize, viewTypeName, warmHostedLayout);
        }
    }
}
