using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RJCodeUI_M1.RJControls;
using System.Collections.Generic;
using Cyotek.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading.Tasks;
using static OpenVisionLab.DEFINE;
using Lib.Common;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.RJForms;
using System.Threading;
using Sunny.UI;
using Lib.OpenCV;

namespace OpenVisionLab
{
    public partial class FormMetroFrame : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private const double RestoredWindowRatio = 0.82;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private CGlobal Global = CGlobal.Inst;
        private DockPanel OperDockPanel;
        private Rectangle restoredWindowBounds = Rectangle.Empty;
        private bool isFullScreenLayout = false;
        private Button btnFullScreen;
        private bool titleBarMouseDown = false;
        private bool titleBarDragging = false;
        private bool suppressTitleBarDrag = false;
        private Point titleBarMouseDownPoint = Point.Empty;
        private DateTime lastTitleBarClickTime = DateTime.MinValue;
        private Point lastTitleBarClickScreenPoint = Point.Empty;

        private Dictionary<MAIN_DOCK_FORM, object> Forms = new Dictionary<MAIN_DOCK_FORM, object>();

        private List<FormLayerDisplay> Displays { get; set; } = new List<FormLayerDisplay>();

        #region MENU
        private FormTeachingVision FrmVision = null;
        private FormInit formInit = null;
        #endregion

        public FormMetroFrame(FormInit formInit)
        {
            this.formInit = formInit;
            InitializeComponent();
            InitWindowBehavior();
            InitEvent();
            InitConfig();
            // 프로그램 Load때 마지막에 사용한 레시피를 Load
            if (Global.System.LastRecipe != "" && Global.System.LastRecipe != "\r\n\t") { Global.Recipe.Name = Global.System.LastRecipe; }
            InitDevice();

            btnAuthoriztionName.Text = Global.System.Authorization.ToString();
        }

        private void FormMetroFrame_Load(object sender, EventArgs e)
        {
            pnlTitleBar.BackColor = Color.FromArgb(64, 73, 108);
            pnStatusBar.BackColor = Color.FromArgb(64, 73, 108);
            //pnSideUtil.BackColor = Color.FromArgb(49, 42, 81);
            //icoBanner.OverlayColor = UIAppearance.StyleColor;
            this.BackColor = Color.FromArgb(64, 73, 108);
            btnAuthoriztion.BackColor = Color.FromArgb(64, 73, 108);
            rjButton2.BackColor = Color.FromArgb(64, 73, 108);

            InitUI();
            InitMenu();

            Global.System.Menu = CSystem.MENU.VISION;

            CLOG.NORMAL("Progra");
        }

        private void FormMetroFrame_Shown(object sender, EventArgs e) { formInit.Close = true; }

        #region INIT

        private void InitWindowBehavior()
        {
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(1280, 720);

            biUserOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rjButton2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Resize += FormMetroFrame_Resize;
            pnStatusBar.Resize += pnStatusBar_Resize;

            InitFullScreenButton();
            InitStatusBarLayout();

            pnlTitleBar.MouseDown += TitleBar_MouseDown;
            pnlTitleBar.MouseMove += TitleBar_MouseMove;
            pnlTitleBar.MouseUp += TitleBar_MouseUp;
            rjLabel1.MouseDown += TitleBar_MouseDown;
            rjLabel1.MouseMove += TitleBar_MouseMove;
            rjLabel1.MouseUp += TitleBar_MouseUp;

            FitToScreen(GetInitialScreen());
        }

        private void InitStatusBarLayout()
        {
            lbVersion.Dock = DockStyle.None;
            lbVersion.AutoSize = false;
            lbVersion.TextAlign = ContentAlignment.MiddleRight;
            lbVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lbDriveC.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lbDriveD.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pgbDriveC.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            pgbDriveD.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            LayoutStatusBar();
        }

        private void InitFullScreenButton()
        {
            btnFullScreen = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Image = CreateFullScreenButtonImage(),
                Location = new Point(btnCerrar.Left - btnCerrar.Width, btnCerrar.Top),
                Size = btnCerrar.Size,
                TabStop = false,
                UseVisualStyleBackColor = true
            };

            btnFullScreen.FlatAppearance.BorderSize = 0;
            btnFullScreen.FlatAppearance.BorderColor = Color.White;
            btnFullScreen.Click += btnFullScreen_Click;

            pnlTitleBar.Controls.Add(btnFullScreen);
            btnFullScreen.BringToFront();
            btnCerrar.BringToFront();
            btnMinimizar.BringToFront();
            LayoutTitleBarButtons();
        }

        private void FormMetroFrame_Resize(object sender, EventArgs e)
        {
            LayoutTitleBarButtons();
            LayoutStatusBar();
        }

        private void pnStatusBar_Resize(object sender, EventArgs e)
        {
            LayoutStatusBar();
        }

        private void LayoutStatusBar()
        {
            if (pnStatusBar.ClientSize.Width <= 0) return;

            int margin = 8;
            int progressTop = Math.Max(0, pnStatusBar.ClientSize.Height - pgbDriveC.Height - 2);

            lbDriveC.Location = new Point(margin, 4);
            pgbDriveC.Location = new Point(margin + 2, progressTop);

            lbDriveD.Location = new Point(265, 4);
            pgbDriveD.Location = new Point(268, progressTop);

            int versionLeft = Math.Max(500, pgbDriveD.Right + 20);
            int versionWidth = Math.Max(220, pnStatusBar.ClientSize.Width - versionLeft - margin);
            lbVersion.SetBounds(versionLeft, 0, versionWidth, pnStatusBar.ClientSize.Height);
            FitLabelFontToWidth(lbVersion, 12f, 8f);
        }

        private void FitLabelFontToWidth(Label label, float maxSize, float minSize)
        {
            if (string.IsNullOrEmpty(label.Text)) return;

            float fontSize = maxSize;
            Font baseFont = label.Font;
            Size proposedSize = new Size(label.Width, label.Height);

            while (fontSize > minSize)
            {
                using (Font testFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style))
                {
                    Size textSize = TextRenderer.MeasureText(label.Text, testFont, proposedSize, TextFormatFlags.SingleLine);
                    if (textSize.Width <= label.Width && textSize.Height <= label.Height)
                    {
                        break;
                    }
                }

                fontSize -= 0.5f;
            }

            if (Math.Abs(label.Font.Size - fontSize) > 0.1f)
            {
                label.Font = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
            }
        }

        private void LayoutTitleBarButtons()
        {
            if (btnFullScreen == null) return;

            int margin = 6;
            int gap = 5;
            int top = 8;
            int right = pnlTitleBar.ClientSize.Width - margin;

            btnCerrar.Location = new Point(right - btnCerrar.Width, top);
            right = btnCerrar.Left - gap;

            btnFullScreen.Location = new Point(right - btnFullScreen.Width, top);
            right = btnFullScreen.Left - gap;

            btnMinimizar.Location = new Point(right - btnMinimizar.Width, top);
            right = btnMinimizar.Left - gap;

            rjButton2.Location = new Point(right - rjButton2.Width, 2);
            right = rjButton2.Left - gap;

            biUserOptions.Location = new Point(right - biUserOptions.Width, 4);
        }

        private Image CreateFullScreenButtonImage()
        {
            Bitmap image = new Bitmap(30, 30);
            using (Graphics g = Graphics.FromImage(image))
            using (Pen pen = new Pen(Color.White, 2))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(pen, 8, 8, 14, 14);
            }

            return image;
        }

        private void FitToCurrentScreen()
        {
            FitToScreen(GetCurrentScreen());
        }

        private Screen GetInitialScreen()
        {
            return Screen.FromPoint(Cursor.Position) ?? Screen.FromControl(this) ?? Screen.PrimaryScreen;
        }

        private void FitToScreen(Screen screen)
        {
            Rectangle workingArea = screen.WorkingArea;

            WindowState = FormWindowState.Normal;
            Bounds = workingArea;
            isFullScreenLayout = true;
        }

        private Screen GetCurrentScreen()
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                return Screen.FromRectangle(Bounds);
            }

            return Screen.FromControl(this) ?? Screen.PrimaryScreen;
        }

        private void ToggleWindowState()
        {
            if (isFullScreenLayout || WindowState == FormWindowState.Maximized)
            {
                RestoreToReducedSize();
            }
            else
            {
                FitToCurrentScreen();
            }
        }

        private void RestoreToReducedSize()
        {
            Screen screen = GetCurrentScreen();
            Rectangle workingArea = screen.WorkingArea;

            if (restoredWindowBounds == Rectangle.Empty || !workingArea.Contains(restoredWindowBounds))
            {
                int width = Math.Max(MinimumSize.Width, (int)(workingArea.Width * RestoredWindowRatio));
                int height = Math.Max(MinimumSize.Height, (int)(workingArea.Height * RestoredWindowRatio));
                int x = workingArea.Left + (workingArea.Width - width) / 2;
                int y = workingArea.Top + (workingArea.Height - height) / 2;

                restoredWindowBounds = new Rectangle(x, y, width, height);
            }

            WindowState = FormWindowState.Normal;
            Bounds = ClampBoundsToWorkingArea(restoredWindowBounds, workingArea);
            isFullScreenLayout = false;
        }

        private Rectangle ClampBoundsToWorkingArea(Rectangle bounds, Rectangle workingArea)
        {
            int width = Math.Min(bounds.Width, workingArea.Width);
            int height = Math.Min(bounds.Height, workingArea.Height);
            int x = Math.Max(workingArea.Left, Math.Min(bounds.X, workingArea.Right - width));
            int y = Math.Max(workingArea.Top, Math.Min(bounds.Y, workingArea.Bottom - height));

            return new Rectangle(x, y, width, height);
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Point screenPoint = ((Control)sender).PointToScreen(e.Location);
            bool isDoubleClick =
                (DateTime.Now - lastTitleBarClickTime).TotalMilliseconds <= SystemInformation.DoubleClickTime &&
                Math.Abs(screenPoint.X - lastTitleBarClickScreenPoint.X) <= SystemInformation.DoubleClickSize.Width &&
                Math.Abs(screenPoint.Y - lastTitleBarClickScreenPoint.Y) <= SystemInformation.DoubleClickSize.Height;

            if (isDoubleClick)
            {
                suppressTitleBarDrag = true;
                titleBarMouseDown = false;
                titleBarDragging = false;
                lastTitleBarClickTime = DateTime.MinValue;
                ToggleWindowState();
                return;
            }

            suppressTitleBarDrag = false;
            lastTitleBarClickTime = DateTime.Now;
            lastTitleBarClickScreenPoint = screenPoint;

            titleBarMouseDown = true;
            titleBarDragging = false;
            titleBarMouseDownPoint = e.Location;
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (suppressTitleBarDrag) return;
            if (!titleBarMouseDown || e.Button != MouseButtons.Left || titleBarDragging) return;

            int distanceX = Math.Abs(e.X - titleBarMouseDownPoint.X);
            int distanceY = Math.Abs(e.Y - titleBarMouseDownPoint.Y);
            if (distanceX < SystemInformation.DragSize.Width / 2 && distanceY < SystemInformation.DragSize.Height / 2) return;

            titleBarDragging = true;

            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
            else if (isFullScreenLayout)
            {
                RestoreToReducedSize();
            }

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            restoredWindowBounds = Bounds;
            isFullScreenLayout = false;
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            titleBarMouseDown = false;
            titleBarDragging = false;
            suppressTitleBarDrag = false;
        }

        private bool InitUI()
        {
            try
            {
                //ddmDevice.OwnerIsMenuButton = true;
                //ddmCapture.OwnerIsMenuButton = true;

                FitToCurrentScreen();
                this.IsMdiContainer = true;

                CLOG.NORMAL("Start S/W");
                Global.System.IF_Handle = this.Handle;
                lbVersion.Text = $"VERSION : {CVersion.VERSION} - {CVersion.DATETIME_UPDATED} ({CVersion.MANAGER})";
                LayoutStatusBar();
                if (FrmVision != null && !FrmVision.Visible) FrmVision.Show();

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                System.Windows.Forms.MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ShowMainForms()
        {
            WeifenLuo.WinFormsUI.Docking.DockContent fr;
            foreach (var form in Forms)
            {
                //  DockState.DockLeft : 도킹 방향
                // CAM_TOP.Pane, DockAlignment.Bottom, 0.5 : 
                // CAM_TOP 폼에 도킹 
                // DockAlignment.Bottom => 폼의 하단부분에 도킹
                // 0.5 => 폼에 사이즈 비율 1:1비율임
                fr = (form.Value as WeifenLuo.WinFormsUI.Docking.DockContent);
                switch (form.Key)
                {
                    case MAIN_DOCK_FORM.MainSystem:
                        fr.Show(this.OperDockPanel, DockState.DockLeft);
                        fr.AutoHidePortion = 500;
                        break;
                    case MAIN_DOCK_FORM.Defect:
                        fr.Show(this.OperDockPanel, DockState.DockRight);
                        fr.AutoHidePortion = 500;
                        break;
                    case MAIN_DOCK_FORM.Graph:
                        fr.Show(this.OperDockPanel, DockState.DockLeft);
                        fr.AutoHidePortion = 500;
                        break;
                    case MAIN_DOCK_FORM.IO:
                        fr.Show(this.OperDockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 1200;
                        break;
                    case MAIN_DOCK_FORM.PLC:
                        fr.Show(this.OperDockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 1200;
                        break;
                    case MAIN_DOCK_FORM.CSV:
                        fr.Show(this.OperDockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 700;
                        break;
                    case MAIN_DOCK_FORM.SEARCHDB:
                        fr.Show(this.OperDockPanel, DockState.DockLeftAutoHide);
                        fr.AutoHidePortion = 720;
                        break;
                    case MAIN_DOCK_FORM.LABEL:
                        fr.Show(this.OperDockPanel, DockState.DockRight);
                        fr.AutoHidePortion = 500;
                        break;
                    case MAIN_DOCK_FORM.LOG:
                        fr.Show(this.OperDockPanel, DockState.DockBottomAutoHide);
                        fr.DockState = DockState.DockBottom;
                        fr.AutoHidePortion = 400;
                        break;
                    case MAIN_DOCK_FORM.SUMMARY:
                        DockContent log = (Forms[MAIN_DOCK_FORM.LOG] as DockContent);
                        fr.Show(log.Pane, null);
                        break;
                    case MAIN_DOCK_FORM.BUTTON:
                        fr.Show(this.OperDockPanel, DockState.DockBottomAutoHide);
                        fr.DockState = DockState.DockBottom;
                        fr.AutoHidePortion = 180;
                        break;
                    case MAIN_DOCK_FORM.CAM_TOP:
                        fr.Show(this.OperDockPanel, DockState.Document);
                        break;
                    case MAIN_DOCK_FORM.CAM_BOTTOM:
                        DockContent CAM_TOP = (Forms[MAIN_DOCK_FORM.CAM_TOP] as DockContent);
                        fr.Show(CAM_TOP.Pane, DockAlignment.Bottom, 0.5);
                        break;
                }
            }
            // 텝 활성화
            fr = (Forms[MAIN_DOCK_FORM.MainSystem] as WeifenLuo.WinFormsUI.Docking.DockContent);
            fr.Activate();

            fr = (Forms[MAIN_DOCK_FORM.LOG] as WeifenLuo.WinFormsUI.Docking.DockContent);
            fr.Activate();
        }

        private bool InitEvent()
        {
            try
            {
                Global.System.EventChangedMenu += OnChangedMenu;
                Global.System.EventChangedAuthorization += OnChangedAuthorization;
                Global.Recipe.EventChagedRecipe += OnChangedRecipe;

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private bool InitConfig()
        {
            try
            {
                Global.System.Notice = "Initialize the Config";
                CUtil.InitDirectory("TEST");
                CUtil.InitDirectory("CAPTURE");
                CUtil.InitDirectory("CONFIG");
                CUtil.InitDirectory("CONFIG\\DEVICE");
                CUtil.InitDirectory("RECIPE");
                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private bool InitMenu()
        {
            try
            {
                if (FrmVision == null) FrmVision = new FormTeachingVision();

                FrmVision.FormBorderStyle = FormBorderStyle.None;
                FrmVision.ControlBox = false;
                FrmVision.Margin = Padding.Empty;
                FrmVision.Padding = Padding.Empty;
                FrmVision.TopLevel = false;
                FrmVision.Bounds = pnMDI.ClientRectangle;
                this.pnMDI.Controls.Add(FrmVision);
                FrmVision.Dock = DockStyle.Fill;
                FrmVision.Visible = false;
                FrmVision.Show();

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private bool InitDevice()
        {
            try
            {
                Global.Device = Global.Device.LoadConfig();
                Global.Device.Init();

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        #endregion

        #region CALL BACK   
        private void OnGrabEnd(object sender, GrabEventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                try
                {
                    if (Global.System.Menu == CSystem.MENU.MAIN)
                    {
                        if (Global.System.Mode == CSystem.MODE.AUTO)
                        {
                            // Grab 완료후 현재 측정한 엔코더값을                             
                            Global.Data.Total_Encoder = (e.getEncoder + Global.Data.RealTimeEncoder);
                            CGlobal.Inst.Data.GrabQueue.Enqueue(new CGrabBuffer(e.ImageGrab.Clone(), e.m_Index, Global.Data.Total_Encoder, false));
                        }
                        else
                        {
                            if (!COpenCVHelper.IsImageEmpty(e.ImageGrab)) { Global.Device.CAMERAS[e.m_Index].ImageManager._Ib.Image = Lib.Common.CImageConverter.ToBitmap(e.ImageGrab); }
                        }
                        //if (!CUtil.IsImageEmpty(e.ImageGrab)) { Global.Device.CAMERAS[e.m_Index].ImageManager._Ib.Image = Lib.Common.CImageConverter.ToBitmap(e.ImageGrab); }
                        GC.Collect();
                    }
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            });
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            this.UIThreadInvoke(() =>
            {
                try
                {
                    CUtil.InitDirectory("RECIPE");
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\VISION");
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\DEVICE");
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\MOTION");
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\GRAPH");
                    CUtil.InitDirectory($"RECIPE\\{Global.Recipe.Name}\\PATTERN");
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            });
        }

        private void HideForm()
        {
            if (FrmVision != null && FrmVision.Visible) FrmVision.Hide();
            //pnFormMain.Hide();
        }

        private void OnChangedMenu(object sender, EventArgs e)
        {
            this.UIThreadInvoke(() =>
            {
                switch (Global.System.Menu)
                {
                    case CSystem.MENU.MAIN:
                        HideForm();
                        //pnFormMain.Show();
                        //btnMenuMotion.BackColor = DEFINE.BACK_COLOR;
                        break;
                    case CSystem.MENU.VISION:
                        HideForm();
                        if (FrmVision != null && !FrmVision.Visible) FrmVision.Show();

                        //btnMenuMotion.BackColor = DEFINE.BACK_COLOR;
                        break;
                    case CSystem.MENU.MOTION:

                        //btnMenuMotion.BackColor = DEFINE.MOUSEHOVER_COLOR;
                        HideForm();
                        break;
                }
            });
        }

        private void OnClickMenu(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((RJButton)sender).Tag.ToString();
                Global.System.Menu = CUtil.ParseEnum<CSystem.MENU>(strIndex);

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void OnChangedAuthorization(object sender, EventArgs e)
        {
            btnAuthoriztionName.Text = Global.System.Authorization.ToString();
        }

        #endregion


        private void timerStatus_Tick(object sender, EventArgs e)
        {

        }

        private void btnScreenCapture_Click(object sender, EventArgs e)
        {
            try
            {
                int w = Screen.PrimaryScreen.Bounds.Width;
                int h = Screen.PrimaryScreen.Bounds.Height;

                System.Drawing.Size s = new System.Drawing.Size(w, h);
                Bitmap b = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(b);

                g.CopyFromScreen(0, 0, 0, 0, s);

                string strSavePath = $"{System.Windows.Forms.Application.StartupPath}\\CAPTURE\\{this.Text}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.jpeg";

                b.Save(strSavePath);

                CLOG.NORMAL($"저장 경로 : {strSavePath}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }


        private void FormMetroFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnScreenCapture_MouseUp(object sender, MouseEventArgs e)
        {           
            if (e.Button == MouseButtons.Right)
            {
                Control control = (Control)sender;
                ddmCapture.ItemClicked += new ToolStripItemClickedEventHandler(CaptureClicked);
                ddmCapture.Show(control, 0, control.Height);
            }
        }

        private void CaptureClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Show Folder":
                    Process.Start(System.Windows.Forms.Application.StartupPath + "\\CAPTURE");
                    break;
                default:
                    break;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (Global.System.Mode == CSystem.MODE.AUTO)
            {
                Global.System.Notice = "Can't Close the Program, because Current Mode is Auto";
                return;
            }
            else
            {
                if (CCommon.ShowdialogMessageBox("EXIT", "DO YOU WANT TO EXIT?", FormMessageBox.MESSAGEBOX_TYPE.Stop))
                {
                    Global.Data.RealTimeEncoder = Global.Data.Total_Encoder;
                    Global.Data.SaveConfig(Global.Recipe.Name);

                    Global.Close();
                    this.Close();
                }
            }
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            FitToCurrentScreen();
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            FitToCurrentScreen();
            //btnRestaurar.Visible = false;
            ///btnMaximizar.Visible = true;
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            ToggleWindowState();
            //  btnMaximizar.Visible = false;
            //  btnRestaurar.Visible = true;
        }
        private void timerConnection_Tick(object sender, EventArgs e)
        {
            double dDrivePercentC = CUtil.DrivePercent("C:\\", out double dCDriveTotalSize, out double dCDriveUsedSize);
            double dDrivePercentD = CUtil.DrivePercent("D:\\", out double dDDriveTotalSize, out double dDDriveUsedSize);

            lbDriveC.Text = $"Drive (C:) : {dDrivePercentC.ToString("F1")}%  ({dCDriveUsedSize.ToString("F1")}/ {dCDriveTotalSize.ToString("F1")} GB)";
            lbDriveD.Text = $"Drive (D:) : {dDrivePercentD.ToString("F1")}%  ({dDDriveUsedSize.ToString("F1")}/ {dDDriveTotalSize.ToString("F1")} GB)";

            pgbDriveC.Value = (int)dDrivePercentC;
            pgbDriveD.Value = (int)dDrivePercentD;
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            RJSettingsForm rJSettingsForm = new RJSettingsForm();
            rJSettingsForm.Show();
        }

        private void biUserOptions_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAuthoriztion_Click(object sender, EventArgs e)
        {

        }
    }
}
