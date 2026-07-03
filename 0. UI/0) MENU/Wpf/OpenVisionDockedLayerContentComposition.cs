using OpenVisionLab.Docking.Controls;
using System;

namespace OpenVisionLab
{
    internal sealed class OpenVisionDockedLayerContentComposition
    {
        private OpenVisionDockedLayerContentComposition(
            IOpenVisionDockDocumentState documentState,
            OpenVisionDockedLayerWorkspaceViewModel viewModel,
            IOpenVisionDockDocumentContentSource contentSource,
            Predicate<object> documentContentPredicate)
        {
            DocumentState = documentState ?? throw new ArgumentNullException(nameof(documentState));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            ContentSource = contentSource ?? throw new ArgumentNullException(nameof(contentSource));
            DocumentContentPredicate = documentContentPredicate ?? throw new ArgumentNullException(nameof(documentContentPredicate));
        }

        public IOpenVisionDockDocumentState DocumentState { get; }

        public OpenVisionDockedLayerWorkspaceViewModel ViewModel { get; }

        public IOpenVisionDockDocumentContentSource ContentSource { get; }

        public Predicate<object> DocumentContentPredicate { get; }

        public static OpenVisionDockedLayerContentComposition Create(OpenVisionDockedLayerWorkspaceRuntimeOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IOpenVisionDockDocumentState documentState = OpenVisionDockedLayerDocumentStateFactory.Create();
            OpenVisionDockedLayerWorkspaceViewModel viewModel = new OpenVisionDockedLayerWorkspaceViewModel(documentState);
            IOpenVisionDockDocumentContentSource contentSource = new OpenVisionDockedLayerContentSource(
                options.DisplayManager,
                options.LayerTitleProvider,
                options.SelectedLayerTitleProvider,
                options.StatusTextProvider,
                new OpenVisionDockedLayerViewerFactory());

            return new OpenVisionDockedLayerContentComposition(
                documentState,
                viewModel,
                contentSource,
                IsDockedLayerViewerContent);
        }

        private static bool IsDockedLayerViewerContent(object content)
        {
            return content is IOpenVisionDockedLayerViewer;
        }
    }
}
