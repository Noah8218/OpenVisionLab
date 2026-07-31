using OpenVisionLab.Core;
using System;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeToolRegistration
    {
        public OpenVisionNativeToolRegistration(
            VISION_MENU menu,
            Func<IDisplayManager, OpenVisionNativeToolDocument> createDocument,
            Size preferredWindowSize,
            string viewTypeName,
            bool warmHostedLayout)
        {
            Menu = menu;
            CreateDocument = createDocument ?? throw new ArgumentNullException(nameof(createDocument));
            PreferredWindowSize = preferredWindowSize;
            ViewTypeName = string.IsNullOrWhiteSpace(viewTypeName) ? string.Empty : viewTypeName;
            WarmHostedLayout = warmHostedLayout;
        }

        public VISION_MENU Menu { get; }

        public Func<IDisplayManager, OpenVisionNativeToolDocument> CreateDocument { get; }

        public Size PreferredWindowSize { get; }

        public string ViewTypeName { get; }

        public bool WarmHostedLayout { get; }
    }
}
