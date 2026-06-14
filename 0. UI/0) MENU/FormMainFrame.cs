using System;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.RJControls;
using OpenVisionLab._1._Core;
using OpenVisionLab.MessageDialogs;
using static OpenVisionLab.DEFINE;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormMainFrame : Form
    {
        private readonly ApplicationRuntimeContext runtimeContext;
        private readonly GlobalState Global;
        #region MENU
        private FormTeachingVision FrmVision = null;
        private FormInit formInit = null;
        private Label lblWorkbenchSubtitle = null;
        private Panel statusBarTopLine = null;
        private Panel driveCTrack = null;
        private Panel driveCFill = null;
        private Panel driveDTrack = null;
        private Panel driveDFill = null;
        private ToolTip statusBarToolTip = null;
        private bool startupSplashClosed = false;
        #endregion

        public FormMainFrame(FormInit formInit)
            : this(formInit, ApplicationRuntimeContext.CreateDefault())
        {
        }

        public FormMainFrame(FormInit formInit, ApplicationRuntimeContext runtimeContext)
        {
            this.runtimeContext = runtimeContext ?? ApplicationRuntimeContext.CreateDefault();
            Global = this.runtimeContext.Global;
            this.formInit = formInit;
            InitializeComponent();
            InitWindowBehavior();
            InitEvent();
            InitConfig();
            // 프로그램 Load때 마지막에 사용한 레시피를 Load
            if (Global.System.LastRecipe != "" && Global.System.LastRecipe != "\r\n\t") { Global.Recipe.Name = Global.System.LastRecipe; }
            btnAuthorizationName.Text = Global.System.Authorization.ToString();
        }

        private void FormMainFrame_Load(object sender, EventArgs e)
        {
            try
            {
                ApplyMainFrameChrome();
                //pnSideUtil.BackColor = Color.FromArgb(49, 42, 81);
                //icoBanner.OverlayColor = UIAppearance.StyleColor;

                InitUI();
                InitMenu();

                Global.System.Menu = SystemState.MenuKind.VISION;
            }
            finally
            {
                CloseStartupSplash();
            }

        }

        private void FormMainFrame_Shown(object sender, EventArgs e)
        {
            CloseStartupSplash();
            BeginInvoke(new System.Windows.Forms.MethodInvoker(EnsureStartupWindowVisible));
        }

        #region INIT

        private void ApplyMainFrameChrome()
        {
            Color chromeColor = Color.FromArgb(56, 66, 101);
            Color chromeAccent = Color.FromArgb(132, 178, 245);

            Text = "OpenVisionLab";
            pnlTitleBar.BackColor = chromeColor;
            pnStatusBar.BackColor = Color.FromArgb(30, 40, 63);
            BackColor = chromeColor;
            btnAuthorizationIcon.BackColor = chromeColor;
            btnScreenCapture.BackColor = chromeColor;

            btnAuthorizationName.Text = Global.System.Authorization.ToString();
            btnAuthorizationName.BackColor = Color.FromArgb(64, 76, 116);
            btnAuthorizationName.ForeColor = Color.FromArgb(205, 222, 249);
            btnAuthorizationName.BorderColor = chromeAccent;

            rjLabel1.AutoSize = false;
            rjLabel1.Text = "OpenVisionLab";
            rjLabel1.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
            rjLabel1.ForeColor = Color.White;
            rjLabel1.TextAlign = ContentAlignment.MiddleLeft;
            rjLabel1.SetBounds(196, 5, 220, 40);

            if (lblWorkbenchSubtitle == null)
            {
                lblWorkbenchSubtitle = new Label
                {
                    AutoSize = false,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(198, 211, 237),
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                    Text = "Rule-based Vision Workbench",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                lblWorkbenchSubtitle.MouseDown += TitleBar_MouseDown;
                lblWorkbenchSubtitle.MouseMove += TitleBar_MouseMove;
                lblWorkbenchSubtitle.MouseUp += TitleBar_MouseUp;
                pnlTitleBar.Controls.Add(lblWorkbenchSubtitle);
            }

            lblWorkbenchSubtitle.SetBounds(rjLabel1.Right + 10, 18, 260, 20);
            lblWorkbenchSubtitle.BringToFront();
        }

        private bool InitUI()
        {
            try
            {
                //ddmCapture.OwnerIsMenuButton = true;

                FitToCurrentScreen();
                this.IsMdiContainer = true;

                lbVersion.Text = $"OpenVisionLab {AppVersion.VERSION} | Updated {AppVersion.DATETIME_UPDATED} | {AppVersion.MANAGER}";
                LayoutStatusBar();
                if (FrmVision != null && !FrmVision.Visible) FrmVision.Show();

            }
            catch (Exception Desc)
            {
                VisionMessageBox.Error(this, "Error", Desc.Message, Desc.ToString());
                return false;
            }

            return true;
        }

        private void CloseStartupSplash()
        {
            if (startupSplashClosed)
            {
                return;
            }

            startupSplashClosed = true;
            try
            {
                formInit?.OnInitEnd();
            }
            finally
            {
                formInit = null;
            }
        }

        private bool InitEvent()
        {
            Global.System.EventChangedMenu += OnChangedMenu;
            Global.System.EventChangedAuthorization += OnChangedAuthorization;
            Global.Recipe.EventChagedRecipe += OnChangedRecipe;

            return true;
        }

        private bool InitConfig()
        {
            Global.System.Notice = "Initialize the Config";
            AppUtil.InitDirectory("TEST");
            AppUtil.InitDirectory("CAPTURE");
            AppUtil.InitDirectory("CONFIG");
            RecipeWorkspaceService.EnsureRoot();

            return true;
        }

        private bool InitMenu()
        {
            if (FrmVision == null) FrmVision = new FormTeachingVision(runtimeContext);

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

            return true;
        }

        #endregion

        #region CALL BACK
        private void OnChangedRecipe(object sender, EventArgs e)
        {
            this.UIThreadInvoke(() =>
            {
                RecipeWorkspaceService.EnsureVisionWorkspace(Global.Recipe.Name);

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
                    case SystemState.MenuKind.MAIN:
                        HideForm();
                        //pnFormMain.Show();
                        //btnMenuMotion.BackColor = DEFINE.BACK_COLOR;
                        break;
                    case SystemState.MenuKind.VISION:
                        HideForm();
                        if (FrmVision != null && !FrmVision.Visible) FrmVision.Show();

                        //btnMenuMotion.BackColor = DEFINE.BACK_COLOR;
                        break;
                    case SystemState.MenuKind.MOTION:

                        //btnMenuMotion.BackColor = DEFINE.MOUSEHOVER_COLOR;
                        HideForm();
                        break;
                }
            });
        }

        private void OnClickMenu(object sender, EventArgs e)
        {
            string strIndex = ((RJButton)sender).Tag.ToString();
            Global.System.Menu = AppUtil.ParseEnum<SystemState.MenuKind>(strIndex);

        }

        private void OnChangedAuthorization(object sender, EventArgs e)
        {
            btnAuthorizationName.Text = Global.System.Authorization.ToString();
        }

        #endregion

    }
}
