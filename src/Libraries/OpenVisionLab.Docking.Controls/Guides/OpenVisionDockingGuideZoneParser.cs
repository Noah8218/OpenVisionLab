using System;

namespace OpenVisionLab.Docking.Controls
{
    public static class OpenVisionDockingGuideZoneParser
    {
        public static DockingGuideZone ParseOrCenter(string zoneName)
        {
            return TryParse(zoneName, out DockingGuideZone zone)
                ? zone
                : DockingGuideZone.Center;
        }

        public static bool TryParse(string zoneName, out DockingGuideZone zone)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
            {
                zone = DockingGuideZone.Center;
                return false;
            }

            if (Enum.TryParse(zoneName, ignoreCase: true, out zone))
            {
                return true;
            }

            zone = DockingGuideZone.Center;
            return false;
        }
    }
}
