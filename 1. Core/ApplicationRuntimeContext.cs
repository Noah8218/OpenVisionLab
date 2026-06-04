using System;

namespace OpenVisionLab._1._Core
{
    public sealed class ApplicationRuntimeContext
    {
        private static readonly Lazy<ApplicationRuntimeContext> defaultContext =
            new Lazy<ApplicationRuntimeContext>(CreateDefaultContext);

        public ApplicationRuntimeContext(CGlobal global, IDisplayManager displayManager, IDisplayHostBinder displayHostBinder)
        {
            Global = global ?? throw new ArgumentNullException(nameof(global));
            DisplayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            DisplayHostBinder = displayHostBinder ?? throw new ArgumentNullException(nameof(displayHostBinder));
        }

        public CGlobal Global { get; }
        public IDisplayManager DisplayManager { get; }
        public IDisplayHostBinder DisplayHostBinder { get; }

        public static ApplicationRuntimeContext CreateDefault()
        {
            return defaultContext.Value;
        }

        private static ApplicationRuntimeContext CreateDefaultContext()
        {
            DisplayManagerService displayManager = DisplayManagerService.Default;
            return new ApplicationRuntimeContext(new CGlobal(), displayManager, displayManager);
        }
    }
}
