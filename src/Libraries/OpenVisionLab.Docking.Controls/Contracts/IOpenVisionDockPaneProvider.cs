using System.Collections.Generic;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionDockPaneProvider
    {
        IEnumerable<OpenVisionDockPaneHandle> EnumeratePaneHandles();

        OpenVisionDockPaneHandle GetPrimaryPaneHandle();
    }
}
