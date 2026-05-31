using OpenVisionLab._1._Core;
using Lib.Common;
using OpenCvSharp;
using RJCodeUI_M1.RJControls;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace OpenVisionLab
{
    public partial class FormMainSystem : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        public FormMainSystem()
        {           
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }
     
        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Global.Thread.CSeqVision.EventSeqComplete += OnInspResult;            
            dgvAttachLabel.DataSource = new CAttatchLabelling().GetAttachLabelList(Global.Data.Already_AttachedList);
            dgvLabelDefct.DataSource = new CAttatchLabelling().GetAttachLabelList(Global.Data.Attaches);

            // 다음날 오후 8시가 되면 모든 카운터를 초기화를 한다.
            Task dailyResetTask = new Task(async () =>
            {
                DateTime now = DateTime.Now;
                DateTime today = now.Date;
                // 다음날 오후 8시로 초기화
                DateTime targetTime = today.AddHours(20);
                DateTime targetTimeMax = today.AddHours(20).AddMinutes(3);

                while (true)
                {
                    now = DateTime.Now;

                    if (now >= targetTime && now <= targetTimeMax)
                    {
                        this.BeginInvoke(new Action(delegate ()
                        {
                            now = DateTime.Now;
                            today = now.Date;
                            // 다음날 오후 8시로 초기화
                            targetTime = today.AddDays(1).AddHours(20);
                            targetTimeMax = today.AddDays(1).AddHours(20).AddMinutes(3);
                        }));

                        //break;
                    }

                    await Task.Delay(1);                    
                }
            });
            dailyResetTask.Start();         
        }

        private void OnInspResult(object sender, EventArgs e)
        {
            if (!(e is InspResultArgs args)) { return; }
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.MAIN)
                {
                    dgvLabelDefct.DataSource = new CAttatchLabelling().GetAttachLabelList(args.Attaches);
                    dgvAttachLabel.DataSource = new CAttatchLabelling().GetAttachLabelList(args.Already_AttachedList);
                    if (dgvLabelDefct.FirstDisplayedScrollingRowIndex == -1) { return; }
                    if (dgvAttachLabel.FirstDisplayedScrollingRowIndex == -1) { return; }
                    dgvLabelDefct.FirstDisplayedScrollingRowIndex = 0;
                    dgvAttachLabel.FirstDisplayedScrollingRowIndex = dgvAttachLabel.Rows.Count - 1;
                }
            });
        }

        private void OnLabelResult(object sender, LabelResultArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                if (Global.System.Menu == CSystem.MENU.MAIN)
                {
                    dgvLabelDefct.DataSource = new CAttatchLabelling().GetAttachLabelList(e.Attaches);
                    dgvAttachLabel.DataSource = new CAttatchLabelling().GetAttachLabelList(e.Already_AttachedList);
                    if (dgvLabelDefct.FirstDisplayedScrollingRowIndex == -1) { return; }
                    if (dgvAttachLabel.FirstDisplayedScrollingRowIndex == -1) { return; }
                    dgvLabelDefct.FirstDisplayedScrollingRowIndex = 0;
                    dgvAttachLabel.FirstDisplayedScrollingRowIndex = dgvAttachLabel.Rows.Count - 1;
                }
            });
        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
        }

        private void timerConnection_Tick(object sender, EventArgs e)
        {
            lbMenu.Text = string.Format("STATUS : {0}", Global.System.Mode.ToString());
            if (Global.System.Mode == CSystem.MODE.AUTO) { lbMenu.ForeColor = System.Drawing.Color.FromArgb(55, 159, 113); }
            else { lbMenu.ForeColor = System.Drawing.Color.FromArgb(132, 129, 132); }
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private void timerEncoder_Tick(object sender, EventArgs e)
        {
            double plcMotionSpeed = Global.Device.DIO_PLC.DI_PLC_DRIVE_SPEED.Current / 100;
            lbPlcMotionSpeed.Text = plcMotionSpeed.ToString();

            // 수정해야함
            double getEncoder = Global.Device.ENC600.GetEncoderValue;
            //int getEncoder = Global.Data.TestEncoder;
            Global.Data.Total_Encoder = (getEncoder + Global.Data.RealTimeEncoder);
            lbEncoder.Text = Global.Data.Total_Encoder.ToString();
            Global.Data.Total_Dist_PerMM = (Global.Data.Total_Encoder * Global.Data.SETTING.EncoderPermm);
            lbTotalDis.Text = Global.Data.Total_Dist_PerMM.ToString("F2") + "mm";
            lbTotalDisM.Text = (Global.Data.Total_Dist_PerMM / 1000).ToString("F2") + "m";
            lbLotNO.Text = Global.Data.LotName;     
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach(var attach in Global.Data.Attaches)
            {
                attach.Clear();
            }
            foreach (var attach in Global.Data.AttachesTemp)
            {
                attach.Clear();
            }
            Global.Data.Already_AttachedList.Clear();

            Global.Data.TestEncoder = 0;
            Global.Data.RealTimeEncoder = 0;
            Global.Data.AcqusitionFrameCount = 0;            
            dgvLabelDefct.DataSource = new CAttatchLabelling().GetAttachLabelList(Global.Data.Attaches);
            dgvAttachLabel.DataSource = new CAttatchLabelling().GetAttachLabelList(Global.Data.Already_AttachedList);
            Global.Data.SaveConfig(Global.Recipe.Name);
            Global.Device.ENC600.Reset();
            //Global.Device.CAMERAS[DEFINE.CAM_1].ClearEncoder();
            //Global.Device.CAMERAS[DEFINE.CAM_2].ClearEncoder();
        }
        private void rjButton1_Click_1(object sender, EventArgs e)
        {
            Global.Data.IsSaveImage = !Global.Data.IsSaveImage;

            //if (Global.Data.IsSaveImage) { rjLabel19.Text = "세이브"; }
            //else { rjLabel19.Text = "논세이브"; }
        }

        //int count = 0;

        private void timerCountUp_Tick(object sender, EventArgs e)
        {
            //count += 100;

            //if (count % 3000 == 0)
            //{
            //    if (Global.Data.Total_Encoder == 0) { return; }
            //    this.UIThreadBeginInvoke(() =>
            //    {
            //        if (Global.Data.Total_Encoder == 0) { return; }
            //        int count = 0;
            //        string[] strExtensions = { "jpg", "png", "jpeg", "bmp" };

            //        string strPath1 = Application.StartupPath + "\\Aging\\Cam1\\";
            //        string strPath2 = Application.StartupPath + "\\Aging\\Cam2\\";

            //        string[] strImages1 = Directory.GetFiles(strPath1, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();
            //        string[] strImages2 = Directory.GetFiles(strPath2, "*.*").Where(f => strExtensions.Contains(f.Split('.').Last().ToLower())).ToArray();

            //        Random random = new Random();

            //        Mat ImageSource = Cv2.ImRead(strImages1[random.Next(0, strImages1.Length - 1)]);

            //        if (ImageSource.Channels() != 1)
            //        {
            //            Cv2.CvtColor(ImageSource, ImageSource, ColorConversionCodes.RGBA2GRAY);
            //        }

            //        Mat ImageSource2 = Cv2.ImRead(strImages2[random.Next(0, strImages2.Length - 1)]);

            //        if (ImageSource2.Channels() != 1)
            //        {
            //            Cv2.CvtColor(ImageSource2, ImageSource2, ColorConversionCodes.RGBA2GRAY);
            //        }

            //        if(Global.Device.CAMERA_COUNT == 1)
            //        {
            //            Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0));
            //        }
            //        else
            //        {
            //            Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource, 0));
            //            Global.Data.GrabQueue.Enqueue(new CGrabBuffer(ImageSource2, 1));
            //        }

            //        count++;
            //    });
            //}
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            timerCountUp.Enabled = !timerCountUp.Enabled;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void grid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            Bitmap image = new CAttatchLabelling().GetAttachLabelList(Global.Data.Attaches)[rowIndex].CropImage;

            FormImageEditView FrmImageView = new FormImageEditView(image);
            FrmImageView.Show();
        }

        private void dgvAttachLabel_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            Bitmap image = new CAttatchLabelling().GetAttachLabelList(Global.Data.Already_AttachedList)[rowIndex].CropImage;

            FormImageEditView FrmImageView = new FormImageEditView(image);
            FrmImageView.Show();
        }

        private int SelectAttachLabelRowIndex = 0;
        private int SelectLabelDefctRowIndex = 0;

        private void dgvAttachLabel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }
                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;
                switch(dgv.Name)
                {
                    case "dgvLabelDefct":
                        SelectLabelDefctRowIndex = rowIndex;
                        break;
                    case "dgvAttachLabel":
                        SelectAttachLabelRowIndex = rowIndex;
                        break;
                }


            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
