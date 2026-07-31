using System;
using System.Windows;

namespace OpenVisionLab.MessageDialogs
{
    public enum VisionMessageKind
    {
        Normal,
        Info,
        Success,
        Warning,
        Error,
        Stop,
        Question
    }

    public static class VisionMessageBox
    {
        public static MessageBoxResult Show(string message)
        {
            return Show(null, message, VisionMessageKind.Normal);
        }

        public static MessageBoxResult Show(string title, string message, VisionMessageKind kind = VisionMessageKind.Normal)
        {
            return Show(title, message, kind, MessageBoxButton.OK);
        }

        public static MessageBoxResult Info(string title, string message)
        {
            return Show(title, message, VisionMessageKind.Info);
        }

        public static MessageBoxResult Error(string title, string message, string details = "")
        {
            string body = string.IsNullOrWhiteSpace(details)
                ? message
                : string.Concat(message, Environment.NewLine, Environment.NewLine, details);
            return Show(title, body, VisionMessageKind.Error);
        }

        public static MessageBoxResult Warning(string title, string message)
        {
            return Show(title, message, VisionMessageKind.Warning);
        }

        public static MessageBoxResult Confirm(string title, string message)
        {
            return Show(title, message, VisionMessageKind.Question, MessageBoxButton.YesNo);
        }

        public static MessageBoxResult Show(
            string title,
            string message,
            VisionMessageKind kind,
            MessageBoxButton buttons)
        {
            if (string.Equals(Environment.GetEnvironmentVariable("OPENVISIONLAB_SUPPRESS_MESSAGEBOX"), "1", StringComparison.Ordinal))
            {
                return buttons == MessageBoxButton.YesNo ? MessageBoxResult.Yes : MessageBoxResult.OK;
            }

            MessageBoxImage image = kind switch
            {
                VisionMessageKind.Info or VisionMessageKind.Success => MessageBoxImage.Information,
                VisionMessageKind.Warning => MessageBoxImage.Warning,
                VisionMessageKind.Error or VisionMessageKind.Stop => MessageBoxImage.Error,
                VisionMessageKind.Question => MessageBoxImage.Question,
                _ => MessageBoxImage.None
            };

            string caption = string.IsNullOrWhiteSpace(title)
                ? OpenVisionLanguageService.T("MessageBox.Message")
                : title;
            string text = message ?? string.Empty;

            if (Application.Current?.Dispatcher?.CheckAccess() == false)
            {
                return Application.Current.Dispatcher.Invoke(() => MessageBox.Show(text, caption, buttons, image));
            }

            return MessageBox.Show(text, caption, buttons, image);
        }
    }
}
