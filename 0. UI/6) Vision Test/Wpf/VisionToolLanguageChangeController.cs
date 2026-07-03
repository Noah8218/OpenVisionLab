using System;

namespace OpenVisionLab
{
    internal sealed class VisionToolLanguageChangeController : IDisposable
    {
        private readonly Action refreshLocalization;
        private bool disposed;

        private VisionToolLanguageChangeController(Action refreshLocalization)
        {
            this.refreshLocalization = refreshLocalization ?? throw new ArgumentNullException(nameof(refreshLocalization));
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
        }

        public static VisionToolLanguageChangeController Attach(Action refreshLocalization)
        {
            return new VisionToolLanguageChangeController(refreshLocalization);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            refreshLocalization();
        }
    }
}
