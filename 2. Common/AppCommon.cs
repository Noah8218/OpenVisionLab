using OpenVisionLab.MessageDialogs;
using System.Windows;

namespace OpenVisionLab
{
    public class AppCommon
    {
        public static bool ShowdialogMessageBox(
            string strHead,
            string strMessage,
            VisionMessageKind type = VisionMessageKind.Normal)
        {
            return VisionMessageBox.Show(strHead, strMessage, type) == MessageBoxResult.OK;
        }

        public static bool ShowMessageBox(
            string strHead,
            string strMessage,
            VisionMessageKind type = VisionMessageKind.Normal)
        {
            VisionMessageBox.Show(strHead, strMessage, type);
            return true;
        }
    }
}
