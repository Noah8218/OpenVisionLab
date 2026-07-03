using Lib.Common;
using OpenVisionLab.MessageDialogs;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace OpenVisionLab
{
    public class CCommon
    {
        private static readonly object SyncRoot = new object();

        public static bool ShowdialogMessageBox(
            string strHead,
            string strMessage,
            VisionMessageKind type = VisionMessageKind.Normal)
        {
            try
            {
                CLOG.NORMAL($"[{strHead}] ==> {strMessage}");
                return VisionMessageBox.Show(strHead, strMessage, type) == MessageBoxResult.OK;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {ex.Message}");
                return false;
            }
        }

        public static bool ShowMessageBox(
            string strHead,
            string strMessage,
            VisionMessageKind type = VisionMessageKind.Normal)
        {
            try
            {
                CLOG.NORMAL($"[{strHead}] ==> {strMessage}");
                VisionMessageBox.Show(strHead, strMessage, type);
                return true;
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {ex.Message}");
                return false;
            }
        }

        public static string SaveLotIDPath()
        {
            return EnsureDatedPath("LOT");
        }

        public static string GetPathOK()
        {
            return EnsureDatedPath("Image", "OK");
        }

        public static string GetPathOK_Ori()
        {
            return EnsureDatedPath("Image", "OK", "Ori");
        }

        public static string GetPath_Crop()
        {
            return EnsureDatedPath("Image", "Crop");
        }

        public static string GetPath_Screen()
        {
            return EnsureDatedPath("Image", "Screen");
        }

        public static string GetPathOK_Insp()
        {
            return EnsureDatedPath("Image", "OK", "Insp");
        }

        public static string GetPathNG()
        {
            return EnsureDatedPath("Image", "NG");
        }

        public static string GetPathNG_Ori()
        {
            return EnsureDatedPath("Image", "NG", "Ori");
        }

        public static string GetPathNG_Insp()
        {
            return EnsureDatedPath("Image", "NG", "Insp");
        }

        private static string EnsureDatedPath(params string[] rootAndLeafParts)
        {
            lock (SyncRoot)
            {
                DateTime now = DateTime.Now;
                string[] parts = new string[rootAndLeafParts.Length + 3];
                parts[0] = rootAndLeafParts[0];
                parts[1] = now.ToString("yyyy");
                parts[2] = now.ToString("MM");
                parts[3] = now.ToString("dd");

                for (int i = 1; i < rootAndLeafParts.Length; i++)
                {
                    parts[i + 3] = rootAndLeafParts[i];
                }

                string path = AppPathService.EnsureDirectory(parts);
                return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                    ? path
                    : path + Path.DirectorySeparatorChar;
            }
        }
    }
}
