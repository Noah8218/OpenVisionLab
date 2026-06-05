using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FontAwesome.Sharp;

namespace OpenVisionLab
{
    public partial class FormMessageBox : Form
    {
        public enum MESSAGEBOX_TYPE { Normal, Info, Quit, Stop, Waring };

        //public FormMessageBox(string strHead, string strMessage)
        //{
        //    InitializeComponent();

        //    lbHeader.Text = strHead;
        //    lbMessage.Text = strMessage;
        //    this.KeyPreview = true;

        //    this.TopLevel = true;
        //    this.TopMost = true;

        //    pnBackground.BackColor = System.Drawing.Color.LimeGreen;
        //    lbHeader.ForeColor = Color.White;
        //    lbMessage.ForeColor = Color.White;
        //}
        public FormMessageBox(string strHead, string strMessage, MESSAGEBOX_TYPE type)
        {
            InitializeComponent();

            lbHeader.Text = strHead;
            lbMessage.Text = strMessage;

            switch (type)
            {
                case MESSAGEBOX_TYPE.Normal:
                    pnBackground.BackColor = System.Drawing.Color.White;
                    lbHeader.ForeColor = Color.Black;
                    lbMessage.ForeColor = Color.Black;
                    break;
                case MESSAGEBOX_TYPE.Info:
                    pnBackground.BackColor = System.Drawing.Color.DeepSkyBlue;
                    lbHeader.ForeColor = Color.White;
                    lbMessage.ForeColor = Color.White;
                    break;
                case MESSAGEBOX_TYPE.Quit:
                    pnBackground.BackColor = System.Drawing.Color.LimeGreen;
                    lbHeader.ForeColor = Color.White;
                    lbMessage.ForeColor = Color.White;
                    break;
                case MESSAGEBOX_TYPE.Stop:
                    pnBackground.BackColor = System.Drawing.Color.Red;
                    lbHeader.ForeColor = Color.White;
                    lbMessage.ForeColor = Color.White;
                    break;
                case MESSAGEBOX_TYPE.Waring:
                    pnBackground.BackColor = System.Drawing.Color.Orange;
                    lbHeader.ForeColor = Color.White;
                    lbMessage.ForeColor = Color.White;
                    break;
            }
            btnOK.BackColor = Color.White;
            btnCancel.BackColor = Color.White;

            this.TopLevel = true;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.KeyPreview = true;

            CreateIcon(type);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CenterToOwnerOrScreen();
        }

        private void CenterToOwnerOrScreen()
        {
            Rectangle targetBounds = Owner != null && !Owner.IsDisposed
                ? Owner.Bounds
                : Screen.FromPoint(Cursor.Position).WorkingArea;

            Location = new Point(
                targetBounds.Left + Math.Max(0, (targetBounds.Width - Width) / 2),
                targetBounds.Top + Math.Max(0, (targetBounds.Height - Height) / 2));
        }
        private void CreateIcon(MESSAGEBOX_TYPE icon)
        {//set message box icon

            switch (icon)
            {
                case MESSAGEBOX_TYPE.Stop: //Error
                    this.pbIcon.IconChar = IconChar.TimesCircle;
                    this.pbIcon.BackColor = System.Drawing.Color.Red;
                    break;
                case MESSAGEBOX_TYPE.Info: //Information
                    this.pbIcon.IconChar = IconChar.InfoCircle;
                    this.pbIcon.BackColor = System.Drawing.Color.DeepSkyBlue;
                    break;
                case MESSAGEBOX_TYPE.Quit://Question
                    this.pbIcon.IconChar = IconChar.QuestionCircle;
                    this.pbIcon.BackColor = System.Drawing.Color.LimeGreen;
                    break;
                case MESSAGEBOX_TYPE.Waring://Exclamation
                    this.pbIcon.IconChar = IconChar.ExclamationTriangle;
                    this.pbIcon.BackColor = System.Drawing.Color.Orange;
                    break;
                case MESSAGEBOX_TYPE.Normal: //None
                    this.pbIcon.IconChar = IconChar.CommentDots;
                    this.pbIcon.BackColor = System.Drawing.Color.White;
                    break;

            }
            this.pbIcon.IconColor = Color.White;
        }

        public void OnShowProgress(object sender = null, EventArgs e = null)
        {
            this.UIThreadBeginInvoke(() =>
            {
                this.Show();
            });
        }

        //protected override void OnShown(EventArgs e)
        //{
        //    FormEffect(this);
        //    base.OnShown(e);            
        //}

        //private void FormEffect(Form fm)
        //{
        //    double[] opacity = new double[] { 0.1d, 0.3d, 0.7d, 0.8d, 0.9d, 1.0d };
        //    int cnt = 0;
        //    System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
        //    {
        //        fm.RightToLeftLayout = false;
        //        fm.Opacity = 0d;
        //        tm.Interval = 50;
        //        // 나타나는 속도를 조정함. ? ?
        //        tm.Tick += delegate (object obj, EventArgs e)
        //        {
        //            if ((cnt + 1 > opacity.Length) || fm == null)
        //            {
        //                tm.Stop();
        //                tm.Dispose();
        //                tm = null;
        //                return;
        //            }
        //            else
        //            {
        //                fm.Opacity = opacity[cnt++];
        //            }
        //        };
        //        tm.Start();
        //    }
        //}


        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private Point mousePoint;

        private bool isDragging = false;

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            mousePoint = new Point(e.X, e.Y);
        }

        private void form_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //if (mousePoint.X == 0 || mousePoint.Y == 0) { return; }
                if (!isDragging) { return; }

                Location = new Point(this.Left - (mousePoint.X - e.X),
                    this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void form_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            this.Update();
        }
    }
}
