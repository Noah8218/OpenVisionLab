using OpenVisionLab.MessageDialogs;
using System;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public partial class FormMessageBox : VisionMessageBoxForm
    {
        public enum MESSAGEBOX_TYPE
        {
            Normal,
            Info,
            Quit,
            Stop,
            Waring
        }

        public FormMessageBox(string strHead, string strMessage, MESSAGEBOX_TYPE type)
            : base(CreateOptions(strHead, strMessage, type))
        {
        }

        public void OnShowProgress(object sender = null, EventArgs e = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Show));
                return;
            }

            Show();
        }

        private static VisionMessageOptions CreateOptions(string title, string message, MESSAGEBOX_TYPE type)
        {
            return new VisionMessageOptions
            {
                Title = title,
                Message = message,
                Kind = MapKind(type),
                Buttons = MessageBoxButtons.OKCancel,
                PrimaryText = "Yes",
                SecondaryText = "No",
                PrimaryResult = DialogResult.OK,
                SecondaryResult = DialogResult.Cancel,
                TopMost = false
            };
        }

        private static VisionMessageKind MapKind(MESSAGEBOX_TYPE type)
        {
            return type switch
            {
                MESSAGEBOX_TYPE.Info => VisionMessageKind.Info,
                MESSAGEBOX_TYPE.Quit => VisionMessageKind.Question,
                MESSAGEBOX_TYPE.Stop => VisionMessageKind.Stop,
                MESSAGEBOX_TYPE.Waring => VisionMessageKind.Warning,
                _ => VisionMessageKind.Normal
            };
        }
    }
}
