using System.Windows.Forms;

namespace OpenVisionLab.MessageDialogs
{
    public static class VisionMessageBox
    {
        public static DialogResult Show(string message)
        {
            return Show(null, "Message", message, VisionMessageKind.Normal, MessageBoxButtons.OK);
        }

        public static DialogResult Show(string title, string message, VisionMessageKind kind = VisionMessageKind.Normal)
        {
            return Show(null, title, message, kind, MessageBoxButtons.OK);
        }

        public static DialogResult Info(IWin32Window owner, string title, string message)
        {
            return Show(owner, title, message, VisionMessageKind.Info, MessageBoxButtons.OK);
        }

        public static DialogResult Info(string title, string message)
        {
            return Info(null, title, message);
        }

        public static DialogResult Success(IWin32Window owner, string title, string message)
        {
            return Show(owner, title, message, VisionMessageKind.Success, MessageBoxButtons.OK);
        }

        public static DialogResult Warning(IWin32Window owner, string title, string message)
        {
            return Show(owner, title, message, VisionMessageKind.Warning, MessageBoxButtons.OK);
        }

        public static DialogResult Warning(string title, string message)
        {
            return Warning(null, title, message);
        }

        public static DialogResult Error(IWin32Window owner, string title, string message, string details = "")
        {
            return Show(owner, new VisionMessageOptions
            {
                Title = title,
                Message = message,
                Details = details,
                Kind = VisionMessageKind.Error,
                Buttons = MessageBoxButtons.OK
            });
        }

        public static DialogResult Error(string title, string message, string details = "")
        {
            return Error(null, title, message, details);
        }

        public static DialogResult Question(IWin32Window owner, string title, string message)
        {
            return Show(owner, title, message, VisionMessageKind.Question, MessageBoxButtons.YesNo);
        }

        public static DialogResult Confirm(IWin32Window owner, string title, string message)
        {
            return Show(owner, new VisionMessageOptions
            {
                Title = title,
                Message = message,
                Kind = VisionMessageKind.Question,
                Buttons = MessageBoxButtons.YesNo,
                PrimaryText = "Yes",
                SecondaryText = "No"
            });
        }

        public static DialogResult Show(
            string title,
            string message,
            VisionMessageKind kind,
            MessageBoxButtons buttons)
        {
            return Show(null, title, message, kind, buttons);
        }

        public static DialogResult Show(
            IWin32Window owner,
            string title,
            string message,
            VisionMessageKind kind = VisionMessageKind.Normal,
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return Show(owner, new VisionMessageOptions
            {
                Title = title,
                Message = message,
                Kind = kind,
                Buttons = buttons
            });
        }

        public static DialogResult Show(IWin32Window owner, VisionMessageOptions options)
        {
            using VisionMessageBoxForm form = new VisionMessageBoxForm(new VisionMessageOptions
            {
                Title = options.Title,
                Message = options.Message,
                Details = options.Details,
                Kind = options.Kind,
                Buttons = options.Buttons,
                DefaultButton = options.DefaultButton,
                PrimaryText = options.PrimaryText,
                SecondaryText = options.SecondaryText,
                TertiaryText = options.TertiaryText,
                PrimaryResult = options.PrimaryResult,
                SecondaryResult = options.SecondaryResult,
                TertiaryResult = options.TertiaryResult,
                TopMost = options.TopMost
            });

            return owner == null ? form.ShowDialog() : form.ShowDialog(owner);
        }
    }
}
