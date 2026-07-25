using System;

namespace OpenVisionLab.Core
{
    public sealed class ApplicationRuntimeContext
    {
        private static readonly Lazy<ApplicationRuntimeContext> defaultContext =
            new Lazy<ApplicationRuntimeContext>(CreateDefaultContext);

        public ApplicationRuntimeContext(GlobalState global, IDisplayManager displayManager)
        {
            Global = global ?? throw new ArgumentNullException(nameof(global));
            DisplayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
        }

        public GlobalState Global { get; }
        public IDisplayManager DisplayManager { get; }

        public static ApplicationRuntimeContext CreateDefault()
        {
            return defaultContext.Value;
        }

        private static ApplicationRuntimeContext CreateDefaultContext()
        {
            DisplayManagerService displayManager = DisplayManagerService.Default;
            return new ApplicationRuntimeContext(new GlobalState(), displayManager);
        }
    }
}
