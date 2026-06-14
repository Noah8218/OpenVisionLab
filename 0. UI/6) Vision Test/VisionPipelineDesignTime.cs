using System.ComponentModel;
using System.Diagnostics;

namespace OpenVisionLab
{
    internal static class VisionPipelineDesignTime
    {
        public static bool IsActive =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv")
            || Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("designtoolsserver");
    }
}
