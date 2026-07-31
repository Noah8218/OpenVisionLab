using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    internal enum OpenVisionRecipeLlmBrowserAssistOpenResult
    {
        EmbeddedChatGptOpened,
        ExternalChatGptOpened,
        EmbeddedBrowserUnavailable,
        BrowserOpenFailed
    }

    internal sealed class OpenVisionRecipeLlmBrowserAssistController : IDisposable
    {
        private const string ChatGptUri = "https://chatgpt.com/";

        private Task<OpenVisionRecipeLlmBrowserAssistOpenResult> embeddedBrowserInitialization;
        private string transientUserDataFolder;
        private bool disposed;

        public async Task<OpenVisionRecipeLlmBrowserAssistOpenResult> OpenChatGptAsync(WebView2 browser)
        {
            if (browser == null || disposed)
            {
                return OpenVisionRecipeLlmBrowserAssistOpenResult.BrowserOpenFailed;
            }

            OpenVisionRecipeLlmBrowserAssistOpenResult initialization = await EnsureEmbeddedBrowserAsync(browser);
            if (initialization != OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened)
            {
                return initialization;
            }

            try
            {
                browser.CoreWebView2.Navigate(ChatGptUri);
                return OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened;
            }
            catch
            {
                return OpenVisionRecipeLlmBrowserAssistOpenResult.BrowserOpenFailed;
            }
        }

        public OpenVisionRecipeLlmBrowserAssistOpenResult OpenChatGptInExternalBrowser()
        {
            if (disposed)
            {
                return OpenVisionRecipeLlmBrowserAssistOpenResult.BrowserOpenFailed;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ChatGptUri,
                    UseShellExecute = true
                });
                return OpenVisionRecipeLlmBrowserAssistOpenResult.ExternalChatGptOpened;
            }
            catch
            {
                return OpenVisionRecipeLlmBrowserAssistOpenResult.BrowserOpenFailed;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            TryDeleteTransientUserDataFolder();
        }

        private Task<OpenVisionRecipeLlmBrowserAssistOpenResult> EnsureEmbeddedBrowserAsync(WebView2 browser)
        {
            return embeddedBrowserInitialization ??= InitializeEmbeddedBrowserAsync(browser);
        }

        private async Task<OpenVisionRecipeLlmBrowserAssistOpenResult> InitializeEmbeddedBrowserAsync(WebView2 browser)
        {
            try
            {
                if (browser.CoreWebView2 == null && browser.CreationProperties == null)
                {
                    transientUserDataFolder = Path.Combine(
                        Path.GetTempPath(),
                        "OpenVisionLab",
                        "BrowserAssist",
                        Guid.NewGuid().ToString("N"));
                    Directory.CreateDirectory(transientUserDataFolder);
                    browser.CreationProperties = new CoreWebView2CreationProperties
                    {
                        UserDataFolder = transientUserDataFolder
                    };
                }

                if (browser.CoreWebView2 == null)
                {
                    await browser.EnsureCoreWebView2Async();
                }

                return OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened;
            }
            catch
            {
                return OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedBrowserUnavailable;
            }
        }

        private void TryDeleteTransientUserDataFolder()
        {
            if (string.IsNullOrWhiteSpace(transientUserDataFolder))
            {
                return;
            }

            try
            {
                Directory.Delete(transientUserDataFolder, true);
            }
            catch
            {
                // ponytail: profile cleanup is best effort; the OS releases a locked WebView2 profile after process exit.
            }
        }
    }
}
