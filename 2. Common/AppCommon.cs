using Lib.Common;
using OpenVisionLab.MessageDialogs;
using RJCodeUI_M1.RJControls;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public class AppCommon
    {
        public static bool ShowdialogMessageBox(string strHead, string strMessage, FormMessageBox.MESSAGEBOX_TYPE type = FormMessageBox.MESSAGEBOX_TYPE.Normal)
        {
                        FormMessageBox FrmMessageBox = new FormMessageBox(strHead, strMessage, type);
            Form owner = GetMessageBoxOwner();


            if (owner != null)
            {
                return FrmMessageBox.ShowDialog(owner) == DialogResult.OK;
            }

            if (FrmMessageBox.ShowDialog() == DialogResult.OK) { return true; }
            else { return false; }
        
        }

        public static bool ShowMessageBox(string strHead, string strMessage, FormMessageBox.MESSAGEBOX_TYPE type = FormMessageBox.MESSAGEBOX_TYPE.Normal)
        {
                        FormMessageBox FrmMessageBox = new FormMessageBox(strHead, strMessage, type);
            Form owner = GetMessageBoxOwner();


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

        private static Form GetMessageBoxOwner()
        {
            return Application.OpenForms
                .OfType<FormMainFrame>()
                .FirstOrDefault(form => form.Visible && !form.IsDisposed)
                ?? Form.ActiveForm
                ?? Application.OpenForms
                    .Cast<Form>()
                    .FirstOrDefault(form => form.Visible && !form.IsDisposed && !(form is FormMessageBox) && !(form is VisionMessageBoxForm));
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

    }
}
