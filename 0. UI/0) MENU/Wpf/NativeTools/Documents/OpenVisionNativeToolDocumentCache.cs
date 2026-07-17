using OpenVisionLab._1._Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolDocumentCache
    {
        private readonly Dictionary<VISION_MENU, OpenVisionNativeToolDocument> documents =
            new Dictionary<VISION_MENU, OpenVisionNativeToolDocument>();

        public int Count => documents.Count;

        public bool Contains(VISION_MENU menu)
        {
            return documents.ContainsKey(menu);
        }

        public bool TryGetOrCreate(
            VISION_MENU menu,
            IDisplayManager displayManager,
            out OpenVisionNativeToolDocument document)
        {
            return TryGetOrCreate(menu, displayManager, null, out document);
        }

        public bool TryGetOrCreate(
            VISION_MENU menu,
            IDisplayManager displayManager,
            OpenVisionRecipeContext recipeContext,
            out OpenVisionNativeToolDocument document)
        {
            if (documents.TryGetValue(menu, out document))
            {
                document.ApplyRecipeContext(recipeContext);
                return true;
            }

            if (!OpenVisionNativeToolDocumentFactory.TryCreate(menu, displayManager, out document))
            {
                return false;
            }

            document.ApplyRecipeContext(recipeContext);
            // Cached native tool documents survive menu switches to avoid rebuilding heavy WPF/property-grid views.
            documents[menu] = document;
            return true;
        }

        public void DisposeAll(Action<OpenVisionNativeToolDocument> beforeDispose)
        {
            foreach (OpenVisionNativeToolDocument document in documents.Values.ToList())
            {
                beforeDispose?.Invoke(document);
                document.Dispose();
            }

            documents.Clear();
        }
    }
}
