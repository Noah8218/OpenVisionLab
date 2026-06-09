using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Lib.Common;
using RJCodeUI_M1.RJForms;
using RJCodeUI_M1.Settings;

namespace OpenVisionLab
{
    public partial class FormMainFrame
    {

        private void btnScreenCapture_Click(object sender, EventArgs e)
        {
                        Rectangle bounds = Screen.PrimaryScreen.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

                string savePath = AppPathService.GetCaptureFilePath(Text, DateTime.Now);
                bitmap.Save(savePath);

            }
        
        }

        private void FormMainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnScreenCapture_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            Control control = (Control)sender;
            ddmCapture.ItemClicked -= CaptureClicked;
            ddmCapture.ItemClicked += CaptureClicked;
            ddmCapture.Show(control, 0, control.Height);
        }

        private void CaptureClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text != "Show Folder") return;

            Process.Start(new ProcessStartInfo
            {
                FileName = AppPathService.CaptureDirectory,
                UseShellExecute = true
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Global.System.Mode == SystemState.RunMode.AUTO)
            {
                Global.System.Notice = "Can't Close the Program, because Current Mode is Auto";
                return;
            }

            if (!AppCommon.ShowdialogMessageBox("EXIT", "DO YOU WANT TO EXIT?", FormMessageBox.MESSAGEBOX_TYPE.Stop))
            {
                return;
            }

            Global.Data.SaveConfig(Global.Recipe.Name);
            Global.Close();
            Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            FitToCurrentScreen();
        }

        private void timerConnection_Tick(object sender, EventArgs e)
        {
            double drivePercentC = AppUtil.DrivePercent("C:\\", out double driveTotalSizeC, out double driveUsedSizeC);
            double drivePercentD = AppUtil.DrivePercent("D:\\", out double driveTotalSizeD, out double driveUsedSizeD);

            lbDriveC.Text = $"Drive (C:) : {drivePercentC:F1}%  ({driveUsedSizeC:F1}/ {driveTotalSizeC:F1} GB)";
            lbDriveD.Text = $"Drive (D:) : {drivePercentD:F1}%  ({driveUsedSizeD:F1}/ {driveTotalSizeD:F1} GB)";

            pgbDriveC.Value = (int)drivePercentC;
            pgbDriveD.Value = (int)drivePercentD;
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            RJSettingsForm settingsForm = new RJSettingsForm();
            settingsForm.Show();
        }

        private void miImageCompare_Click(object sender, EventArgs e)
        {
                        FormImageCompare formCompare = new FormImageCompare
            {
                TopLevel = true,
                TopMost = false,
                StartPosition = FormStartPosition.CenterParent
            };

            if (!AppUtil.OpenCheckForm(formCompare)) return;
            formCompare.Show(this);
        
        }

        private void miLogViewer_Click(object sender, EventArgs e)
        {
            if (FrmVision == null)
            {
                return;
            }

            Global.System.Menu = SystemState.MenuKind.VISION;
            FrmVision.ShowDockedLogViewer();
        }
    }
}
