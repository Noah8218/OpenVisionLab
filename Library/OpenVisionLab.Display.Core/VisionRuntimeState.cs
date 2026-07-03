namespace OpenVisionLab._1._Core
{
    public sealed class VisionRuntimeState
    {
        public string SelectedItem { get; set; } = "Main";
        public string FocusItem { get; set; } = "";
        public int CameraIndex { get; set; }
        public string TackTime { get; set; }
    }
}
