using System;

namespace OpenVisionLab._1._Core
{
    public interface IVisionRuntimeContext
    {
        event EventHandler<EventArgs> UpdateParameter;
        event EventHandler<EventArgs> UpdateResult;
        event EventHandler<EventArgs> UpdateCam;

        VisionRuntimeState State { get; }
        string SelectedItem { get; set; }
        string FocusItem { get; set; }
        int CameraIndex { get; set; }
        string TackTime { get; set; }

        void SetCameraIndex(int cameraIndex);
        void SetTackTime(string tackTime);
        void NotifyParameterChanged();
    }
}
