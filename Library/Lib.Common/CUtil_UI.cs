using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using OpenCvSharp;
using System.Drawing;

namespace Lib.Common
{
    public class CUtil_UI
    {
        public static void txtInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링                         
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)
               || e.KeyChar == Convert.ToChar('.')))    //숫자와 백스페이스를 제외한 나머지를 바로 처리             
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                e.Handled = true;
            }
        }

        public static void TypingOnlyNumber(object sender, KeyPressEventArgs e, bool includePoint, bool includeMinus)
        {
            bool isValidInput = false;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if (includePoint == true) { if (e.KeyChar == '.') isValidInput = true; }
                if (includeMinus == true) { if (e.KeyChar == '-') isValidInput = true; }

                if (isValidInput == false) e.Handled = true;
            }

            if (includePoint == true)
            {
                if (e.KeyChar == '.' && (string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('.') > -1)) e.Handled = true;
            }
            if (includeMinus == true)
            {
                if (e.KeyChar == '-' && (!string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('-') > -1)) e.Handled = true;
            }
        }

        public static void UpdateLabelSignal(Label lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelSignal), lb, bOn);
            }
            else
            {
                if (bOn)
                {
                    lb.BackColor = Color.Aquamarine;
                    lb.ForeColor = Color.Black;
                    lb.Text = "ON";
                }
                else
                {
                    lb.BackColor = Color.DimGray;
                    lb.ForeColor = Color.White;
                    lb.Text = "OFF";
                }
            }
        }

        public static void UpdateLabelResult(Label lb, bool bOK)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOK);
            }
            else
            {
                if (bOK)
                {
                    lb.BackColor = Color.Aquamarine;
                    lb.ForeColor = Color.Black;
                    lb.Text = "OK";
                }
                else
                {
                    lb.BackColor = Color.Red;
                    lb.ForeColor = Color.Yellow;
                    lb.Text = "NG";
                }
            }
        }

        public static void UpdateLabelOnOff(Label lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOn);
            }
            else
            {
                System.Drawing.Color COLOR_TEAL = System.Drawing.Color.FromArgb(0, 170, 173);
                if (bOn)
                {
                    lb.BackColor = COLOR_TEAL;
                    lb.ForeColor = Color.White;
                }
                else
                {
                    lb.BackColor = Color.DimGray;
                    lb.ForeColor = COLOR_TEAL;
                }
            }
        }

        public static void UpdateButtonOnOff(Button lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Button, bool>(UpdateButtonOnOff), lb, bOn);
            }
            else
            {
                if (bOn)
                {
                    lb.BackColor = Color.LimeGreen;
                }
                else
                {
                    lb.BackColor = Color.Red;
                }
            }
        }
    }
}
