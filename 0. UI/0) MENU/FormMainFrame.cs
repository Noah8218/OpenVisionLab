using System;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.RJControls;
using OpenVisionLab._1._Core;
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
            pnlTitleBar.BackColor = Color.FromArgb(64, 73, 108);
            pnStatusBar.BackColor = Color.FromArgb(64, 73, 108);
            //pnSideUtil.BackColor = Color.FromArgb(49, 42, 81);
            //icoBanner.OverlayColor = UIAppearance.StyleColor;
            this.BackColor = Color.FromArgb(64, 73, 108);
            btnAuthorizationIcon.BackColor = Color.FromArgb(64, 73, 108);
            btnScreenCapture.BackColor = Color.FromArgb(64, 73, 108);

            InitUI();
            InitMenu();

            Global.System.Menu = SystemState.MenuKind.VISION;

        }

        private void FormMainFrame_Shown(object sender, EventArgs e)
        {
            formInit?.OnInitEnd();
            BeginInvoke(new System.Windows.Forms.MethodInvoker(EnsureStartupWindowVisible));
        }

        #region INIT

        private bool InitUI()
        {
            try
            {
                //ddmCapture.OwnerIsMenuButton = true;

                FitToCurrentScreen();
                this.IsMdiContainer = true;

                lbVersion.Text = $"VERSION : {AppVersion.VERSION} - {AppVersion.DATETIME_UPDATED} ({AppVersion.MANAGER})";
                LayoutStatusBar();
                if (FrmVision != null && !FrmVision.Visible) FrmVision.Show();

            }
            catch (Exception Desc)
            {
                System.Windows.Forms.MessageBox.Show(Desc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
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
