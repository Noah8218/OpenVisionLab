using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static OpenVisionLab.DEFINE;

namespace OpenVisionLab
{
    internal static class OpenVisionNativeToolPrewarmPolicy
    {
        public static IEnumerable<VISION_MENU> GetDefaultMenus()
        {
            VISION_MENU? recentMenu = OpenVisionNativeToolRecentSelectionStore.TryRead(out VISION_MENU menu)
                && OpenVisionNativeToolRegistry.IsRegistered(menu)
                ? menu
                : (VISION_MENU?)null;
            return GetMenus(recentMenu);
        }

        public static IEnumerable<VISION_MENU> GetMenus(VISION_MENU? preferredMenu)
        {
            IEnumerable<VISION_MENU> menus = OpenVisionNativeToolRegistry.GetPrewarmMenus();
            if (!preferredMenu.HasValue || !OpenVisionNativeToolRegistry.IsRegistered(preferredMenu.Value))
            {
                return menus;
            }

            return new[] { preferredMenu.Value }.Concat(menus.Where(menu => menu != preferredMenu.Value));
        }

        public static void RecordSelection(VISION_MENU menu)
        {
            if (OpenVisionNativeToolRegistry.IsRegistered(menu))
            {
                OpenVisionNativeToolRecentSelectionStore.Save(menu);
            }
        }

        public static Size GetPreferredWindowSize(VISION_MENU menu)
        {
            return OpenVisionNativeToolRegistry.GetPreferredWindowSize(menu);
        }

        public static Size GetLayoutWarmSize()
        {
            return OpenVisionNativeToolRegistry.GetLayoutWarmSize();
        }

        public static bool ShouldWarmHostedLayout(OpenVisionNativeToolDocument document)
        {
            string viewTypeName = document?.ActiveViewTypeName ?? string.Empty;
            return OpenVisionNativeToolRegistry.ShouldWarmHostedLayout(viewTypeName);
        }
    }
}
