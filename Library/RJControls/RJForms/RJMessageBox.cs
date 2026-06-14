using OpenVisionLab.MessageDialogs;
using System.Windows.Forms;

namespace RJCodeUI_M1
{
    public abstract class RJMessageBox
    {
        public static DialogResult Show(string text)
        {
            return Show(null, text, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(
            string text,
            string caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            return Show(null, text, caption, buttons, icon, defaultButton);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return Show(owner, text, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(
            IWin32Window owner,
            string text,
            string caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            return VisionMessageBox.Show(owner, new VisionMessageOptions
            {
                Title = string.IsNullOrWhiteSpace(caption) ? "Message" : caption,
                Message = text ?? string.Empty,
                Kind = MapKind(icon),
                Buttons = buttons,
                DefaultButton = defaultButton
            });
        }

        private static VisionMessageKind MapKind(MessageBoxIcon icon)
        {
            return icon switch
            {
                MessageBoxIcon.Information => VisionMessageKind.Info,
                MessageBoxIcon.Question => VisionMessageKind.Question,
                MessageBoxIcon.Warning or MessageBoxIcon.Exclamation => VisionMessageKind.Warning,
                MessageBoxIcon.Error or MessageBoxIcon.Hand or MessageBoxIcon.Stop => VisionMessageKind.Error,
                _ => VisionMessageKind.Normal
            };
        }
    }
}
