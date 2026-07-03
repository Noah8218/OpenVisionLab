using System;

namespace OpenVisionLab.Docking.Controls
{
    public interface IOpenVisionDockLifecycle
    {
        void Track(Action attach, Action detach);
    }
}
