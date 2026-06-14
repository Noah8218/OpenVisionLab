using System.Windows.Forms;

namespace OpenVisionLab.MessageDialogs
{
    public sealed class VisionMessageOptions
    {
        public string Title { get; set; } = "Message";

        public string Message { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public VisionMessageKind Kind { get; set; } = VisionMessageKind.Normal;

        public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.OK;

        public MessageBoxDefaultButton DefaultButton { get; set; } = MessageBoxDefaultButton.Button1;

        public string PrimaryText { get; set; }

        public string SecondaryText { get; set; }

        public string TertiaryText { get; set; }

        public DialogResult? PrimaryResult { get; set; }

        public DialogResult? SecondaryResult { get; set; }

        public DialogResult? TertiaryResult { get; set; }

        public bool TopMost { get; set; }
    }
}
