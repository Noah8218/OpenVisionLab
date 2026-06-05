using Lib.Common;
using RJCodeUI_M1.RJControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public class CCommon
    {
        public static bool ShowdialogMessageBox(string strHead, string strMessage, FormMessageBox.MESSAGEBOX_TYPE type = FormMessageBox.MESSAGEBOX_TYPE.Normal)
        {
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(strHead, strMessage, type);
                Form owner = GetMessageBoxOwner();

                CLOG.NORMAL($"[{strHead}] ==> {strMessage}");

                if (owner != null)
                {
                    return FrmMessageBox.ShowDialog(owner) == DialogResult.OK;
                }

                if (FrmMessageBox.ShowDialog() == DialogResult.OK) { return true; }
                else { return false; }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        public static bool ShowMessageBox(string strHead, string strMessage, FormMessageBox.MESSAGEBOX_TYPE type = FormMessageBox.MESSAGEBOX_TYPE.Normal)
        {
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(strHead, strMessage, type);
                Form owner = GetMessageBoxOwner();

                CLOG.NORMAL($"[{strHead}] ==> {strMessage}");

                FrmMessageBox.UIThreadInvoke(() =>
                {
                    if (owner != null)
                    {
                        FrmMessageBox.Show(owner);
                    }
                    else
                    {
                        FrmMessageBox.Show();//You GUI code here
                    }
                });

                return true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }
        }

        private static Form GetMessageBoxOwner()
        {
            return Application.OpenForms
                .OfType<FormMetroFrame>()
                .FirstOrDefault(form => form.Visible && !form.IsDisposed)
                ?? Form.ActiveForm
                ?? Application.OpenForms
                    .Cast<Form>()
                    .FirstOrDefault(form => form.Visible && !form.IsDisposed && !(form is FormMessageBox));
        }

        public static void SetButtonBlue(RJButton rJButton)
        {
            rJButton.ForeColor = DEFINE.ButtonColorBlue;
            rJButton.IconColor = DEFINE.ButtonColorBlue;
            rJButton.BorderColor = DEFINE.ButtonColorBlue;
        }

        public static void SetButtonRed(RJButton rJButton)
        {
            rJButton.ForeColor = DEFINE.ButtonColorRed;
            rJButton.IconColor = DEFINE.ButtonColorRed;
            rJButton.BorderColor = DEFINE.ButtonColorRed;
        }

        static object m_ob = new object();

        public static string SaveLotIDPath()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "LOT\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
                CUtil.InitDirectory("LOT\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("LOT\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("LOT\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));

                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "LOT\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";
            }
        }

        public static string GetPathOK()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK");

                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\";
            }
        }

        public static string GetPathOK_Ori()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Ori";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK");
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Ori");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Ori" + "\\";
            }
        }

        public static string GetPath_Crop()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\Crop";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "Crop");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\Crop" + "\\";
            }
        }

        public static string GetPath_Screen()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\Screen";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "Screen");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\Screen" + "\\";
            }
        }

        public static string GetPathOK_Insp()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Insp";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK");
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Insp");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "OK" + "\\Insp" + "\\";
            }
        }

        public static string GetPathNG()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\";
            }

        }

        public static string GetPathNG_Ori()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Ori";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG");
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Ori");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Ori" + "\\";
            }
        }

        public static string GetPathNG_Insp()
        {
            lock (m_ob)
            {
                string strLogPath = Application.StartupPath;
                string strPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Insp";
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd"));
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG");
                CUtil.InitDirectory("Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Insp");
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists == false) dir.Create();

                return strLogPath = Application.StartupPath + "\\" + "Image\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\" + "NG" + "\\Insp" + "\\";
            }
        }
    }
}
