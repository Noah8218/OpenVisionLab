namespace OpenVisionLab
{
    partial class FormMetroFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        { 
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMetroFrame));
            this.timerAlarm = new System.Windows.Forms.Timer(this.components);
            this.pnMDI = new System.Windows.Forms.Panel();
            this.pnFormMain = new MetroFramework.Controls.MetroPanel();
            this.OperatorPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.timerConnection = new System.Windows.Forms.Timer(this.components);
            this.pnStatusBar = new RJCodeUI_M1.RJControls.RJPanel();
            this.pgbDriveD = new MetroFramework.Controls.MetroProgressBar();
            this.lbDriveD = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbVersion = new RJCodeUI_M1.RJControls.RJLabel();
            this.pgbDriveC = new MetroFramework.Controls.MetroProgressBar();
            this.lbDriveC = new RJCodeUI_M1.RJControls.RJLabel();
            this.pnlTitleBar = new RJCodeUI_M1.RJControls.RJPanel();
            this.biUserOptions = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.dmUserOptions = new RJCodeUI_M1.RJControls.RJDropdownMenu(this.components);
            this.miMyProfile = new FontAwesome.Sharp.IconMenuItem();
            this.miSettings = new FontAwesome.Sharp.IconMenuItem();
            this.miTermsCond = new FontAwesome.Sharp.IconMenuItem();
            this.miHelp = new FontAwesome.Sharp.IconMenuItem();
            this.miLogout = new FontAwesome.Sharp.IconMenuItem();
            this.miExit = new FontAwesome.Sharp.IconMenuItem();
            this.btnAuthoriztionName = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton2 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnAuthoriztion = new RJCodeUI_M1.RJControls.RJButton();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.ddmDevice = new RJCodeUI_M1.RJControls.RJDropdownMenu(this.components);
            this.iconMenuItem2 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem3 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem4 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem5 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem6 = new FontAwesome.Sharp.IconMenuItem();
            this.ddmCapture = new RJCodeUI_M1.RJControls.RJDropdownMenu(this.components);
            this.iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
            this.pnMDI.SuspendLayout();
            this.pnFormMain.SuspendLayout();
            this.pnStatusBar.SuspendLayout();
            this.pnlTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.biUserOptions)).BeginInit();
            this.dmUserOptions.SuspendLayout();
            this.ddmDevice.SuspendLayout();
            this.ddmCapture.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerAlarm
            // 
            this.timerAlarm.Enabled = true;
            this.timerAlarm.Interval = 500;
            // 
            // pnMDI
            // 
            this.pnMDI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnMDI.Controls.Add(this.pnFormMain);
            this.pnMDI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMDI.Location = new System.Drawing.Point(0, 50);
            this.pnMDI.Margin = new System.Windows.Forms.Padding(0);
            this.pnMDI.Name = "pnMDI";
            this.pnMDI.Padding = new System.Windows.Forms.Padding(0, 0, 0, 33);
            this.pnMDI.Size = new System.Drawing.Size(1924, 996);
            this.pnMDI.TabIndex = 1258;
            // 
            // pnFormMain
            // 
            this.pnFormMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.pnFormMain.Controls.Add(this.OperatorPanel);
            this.pnFormMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnFormMain.HorizontalScrollbarBarColor = true;
            this.pnFormMain.HorizontalScrollbarHighlightOnWheel = false;
            this.pnFormMain.HorizontalScrollbarSize = 10;
            this.pnFormMain.Location = new System.Drawing.Point(0, 0);
            this.pnFormMain.Margin = new System.Windows.Forms.Padding(0);
            this.pnFormMain.Name = "pnFormMain";
            this.pnFormMain.Size = new System.Drawing.Size(1924, 963);
            this.pnFormMain.TabIndex = 895;
            this.pnFormMain.VerticalScrollbarBarColor = true;
            this.pnFormMain.VerticalScrollbarHighlightOnWheel = false;
            this.pnFormMain.VerticalScrollbarSize = 10;
            // 
            // OperatorPanel
            // 
            this.OperatorPanel.BackColor = System.Drawing.Color.Black;
            this.OperatorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OperatorPanel.Location = new System.Drawing.Point(0, 0);
            this.OperatorPanel.Name = "OperatorPanel";
            this.OperatorPanel.Size = new System.Drawing.Size(1924, 963);
            this.OperatorPanel.TabIndex = 2138;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1910, 47);
            this.panel3.TabIndex = 1949;
            // 
            // timerConnection
            // 
            this.timerConnection.Enabled = true;
            this.timerConnection.Tick += new System.EventHandler(this.timerConnection_Tick);
            // 
            // pnStatusBar
            // 
            this.pnStatusBar.BackColor = System.Drawing.Color.Black;
            this.pnStatusBar.BorderRadius = 0;
            this.pnStatusBar.Controls.Add(this.pgbDriveD);
            this.pnStatusBar.Controls.Add(this.lbDriveD);
            this.pnStatusBar.Controls.Add(this.lbVersion);
            this.pnStatusBar.Controls.Add(this.pgbDriveC);
            this.pnStatusBar.Controls.Add(this.lbDriveC);
            this.pnStatusBar.Customizable = true;
            this.pnStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnStatusBar.Location = new System.Drawing.Point(0, 1013);
            this.pnStatusBar.Name = "pnStatusBar";
            this.pnStatusBar.Size = new System.Drawing.Size(1924, 33);
            this.pnStatusBar.TabIndex = 0;
            // 
            // pgbDriveD
            // 
            this.pgbDriveD.Location = new System.Drawing.Point(268, 21);
            this.pgbDriveD.Name = "pgbDriveD";
            this.pgbDriveD.Size = new System.Drawing.Size(218, 10);
            this.pgbDriveD.Style = MetroFramework.MetroColorStyle.Lime;
            this.pgbDriveD.TabIndex = 2109;
            this.pgbDriveD.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lbDriveD
            // 
            this.lbDriveD.AutoSize = true;
            this.lbDriveD.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbDriveD.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriveD.ForeColor = System.Drawing.Color.White;
            this.lbDriveD.LinkLabel = false;
            this.lbDriveD.Location = new System.Drawing.Point(265, 6);
            this.lbDriveD.Name = "lbDriveD";
            this.lbDriveD.Size = new System.Drawing.Size(164, 13);
            this.lbDriveD.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbDriveD.TabIndex = 2111;
            this.lbDriveD.Text = "(D:) : 00%    (000/000 GB)";
            // 
            // lbVersion
            // 
            this.lbVersion.AutoSize = true;
            this.lbVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbVersion.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbVersion.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbVersion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.ForeColor = System.Drawing.Color.White;
            this.lbVersion.LinkLabel = false;
            this.lbVersion.Location = new System.Drawing.Point(1745, 0);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(179, 18);
            this.lbVersion.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbVersion.TabIndex = 2122;
            this.lbVersion.Text = "Version 2.5 - 211007";
            // 
            // pgbDriveC
            // 
            this.pgbDriveC.Location = new System.Drawing.Point(10, 20);
            this.pgbDriveC.Name = "pgbDriveC";
            this.pgbDriveC.Size = new System.Drawing.Size(218, 10);
            this.pgbDriveC.Style = MetroFramework.MetroColorStyle.Lime;
            this.pgbDriveC.TabIndex = 2108;
            this.pgbDriveC.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lbDriveC
            // 
            this.lbDriveC.AutoSize = true;
            this.lbDriveC.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbDriveC.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriveC.ForeColor = System.Drawing.Color.White;
            this.lbDriveC.LinkLabel = false;
            this.lbDriveC.Location = new System.Drawing.Point(7, 6);
            this.lbDriveC.Name = "lbDriveC";
            this.lbDriveC.Size = new System.Drawing.Size(164, 13);
            this.lbDriveC.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbDriveC.TabIndex = 2110;
            this.lbDriveC.Text = "(C:) : 00%    (000/000 GB)";
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.Color.Black;
            this.pnlTitleBar.BorderRadius = 0;
            this.pnlTitleBar.Controls.Add(this.biUserOptions);
            this.pnlTitleBar.Controls.Add(this.btnAuthoriztionName);
            this.pnlTitleBar.Controls.Add(this.rjButton2);
            this.pnlTitleBar.Controls.Add(this.btnCerrar);
            this.pnlTitleBar.Controls.Add(this.btnAuthoriztion);
            this.pnlTitleBar.Controls.Add(this.btnMinimizar);
            this.pnlTitleBar.Controls.Add(this.rjLabel1);
            this.pnlTitleBar.Customizable = true;
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(1924, 50);
            this.pnlTitleBar.TabIndex = 1260;
            // 
            // biUserOptions
            // 
            this.biUserOptions.BackColor = System.Drawing.Color.Transparent;
            this.biUserOptions.BackIcon = false;
            this.biUserOptions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.biUserOptions.Customizable = false;
            this.biUserOptions.DropdownMenu = this.dmUserOptions;
            this.biUserOptions.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.biUserOptions.IconChar = FontAwesome.Sharp.IconChar.Cog;
            this.biUserOptions.IconColor = System.Drawing.Color.WhiteSmoke;
            this.biUserOptions.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.biUserOptions.IconSize = 40;
            this.biUserOptions.Location = new System.Drawing.Point(1746, 4);
            this.biUserOptions.Name = "biUserOptions";
            this.biUserOptions.Size = new System.Drawing.Size(40, 40);
            this.biUserOptions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.biUserOptions.TabIndex = 1267;
            this.biUserOptions.TabStop = false;
            this.biUserOptions.Click += new System.EventHandler(this.biUserOptions_Click_1);
            // 
            // dmUserOptions
            // 
            this.dmUserOptions.ActiveMenuItem = false;
            this.dmUserOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dmUserOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMyProfile,
            this.miSettings,
            this.miTermsCond,
            this.miHelp,
            this.miLogout,
            this.miExit});
            this.dmUserOptions.Name = "dmUserOptions";
            this.dmUserOptions.OwnerIsMenuButton = false;
            this.dmUserOptions.Size = new System.Drawing.Size(182, 136);
            // 
            // miMyProfile
            // 
            this.miMyProfile.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
            this.miMyProfile.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(85)))), ((int)(((byte)(230)))));
            this.miMyProfile.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miMyProfile.IconSize = 21;
            this.miMyProfile.Name = "miMyProfile";
            this.miMyProfile.Size = new System.Drawing.Size(181, 22);
            this.miMyProfile.Text = "My Profile";
            // 
            // miSettings
            // 
            this.miSettings.IconChar = FontAwesome.Sharp.IconChar.Tools;
            this.miSettings.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(168)))), ((int)(((byte)(210)))));
            this.miSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miSettings.IconSize = 21;
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(181, 22);
            this.miSettings.Text = "Settings";
            this.miSettings.Click += new System.EventHandler(this.miSettings_Click);
            // 
            // miTermsCond
            // 
            this.miTermsCond.IconChar = FontAwesome.Sharp.IconChar.ShieldAlt;
            this.miTermsCond.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(132)))), ((int)(((byte)(235)))));
            this.miTermsCond.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miTermsCond.IconSize = 21;
            this.miTermsCond.Name = "miTermsCond";
            this.miTermsCond.Size = new System.Drawing.Size(181, 22);
            this.miTermsCond.Text = "Terms and Cond";
            // 
            // miHelp
            // 
            this.miHelp.IconChar = FontAwesome.Sharp.IconChar.QuestionCircle;
            this.miHelp.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(96)))), ((int)(((byte)(112)))));
            this.miHelp.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miHelp.IconSize = 21;
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(181, 22);
            this.miHelp.Text = "Help";
            // 
            // miLogout
            // 
            this.miLogout.IconChar = FontAwesome.Sharp.IconChar.SignOutAlt;
            this.miLogout.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(84)))), ((int)(((byte)(228)))));
            this.miLogout.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miLogout.IconSize = 21;
            this.miLogout.Name = "miLogout";
            this.miLogout.Size = new System.Drawing.Size(181, 22);
            this.miLogout.Text = "Logout";
            // 
            // miExit
            // 
            this.miExit.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.miExit.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(37)))), ((int)(((byte)(118)))));
            this.miExit.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.miExit.IconSize = 21;
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(181, 22);
            this.miExit.Text = "Exit";
            // 
            // btnAuthoriztionName
            // 
            this.btnAuthoriztionName.BackColor = System.Drawing.Color.Black;
            this.btnAuthoriztionName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnAuthoriztionName.BorderRadius = 10;
            this.btnAuthoriztionName.BorderSize = 1;
            this.btnAuthoriztionName.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnAuthoriztionName.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnAuthoriztionName.FlatAppearance.BorderSize = 0;
            this.btnAuthoriztionName.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAuthoriztionName.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAuthoriztionName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuthoriztionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuthoriztionName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnAuthoriztionName.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnAuthoriztionName.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnAuthoriztionName.IconFont = FontAwesome.Sharp.IconFont.Regular;
            this.btnAuthoriztionName.IconSize = 24;
            this.btnAuthoriztionName.Location = new System.Drawing.Point(74, 10);
            this.btnAuthoriztionName.Name = "btnAuthoriztionName";
            this.btnAuthoriztionName.Size = new System.Drawing.Size(108, 32);
            this.btnAuthoriztionName.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnAuthoriztionName.TabIndex = 2647;
            this.btnAuthoriztionName.Text = "Model";
            this.btnAuthoriztionName.UseVisualStyleBackColor = false;
            // 
            // rjButton2
            // 
            this.rjButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.rjButton2.BorderRadius = 0;
            this.rjButton2.BorderSize = 1;
            this.rjButton2.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
            this.rjButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton2.FlatAppearance.BorderSize = 0;
            this.rjButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.rjButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton2.ForeColor = System.Drawing.Color.White;
            this.rjButton2.IconChar = FontAwesome.Sharp.IconChar.FileExport;
            this.rjButton2.IconColor = System.Drawing.Color.White;
            this.rjButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton2.IconSize = 40;
            this.rjButton2.Location = new System.Drawing.Point(1792, 2);
            this.rjButton2.Name = "rjButton2";
            this.rjButton2.Size = new System.Drawing.Size(63, 45);
            this.rjButton2.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.rjButton2.TabIndex = 8;
            this.rjButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rjButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rjButton2.UseVisualStyleBackColor = false;
            this.rjButton2.Click += new System.EventHandler(this.btnScreenCapture_Click);
            this.rjButton2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnScreenCapture_MouseUp);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Image = ((System.Drawing.Image)(resources.GetObject("btnCerrar.Image")));
            this.btnCerrar.Location = new System.Drawing.Point(1888, 8);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(30, 30);
            this.btnCerrar.TabIndex = 2638;
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnAuthoriztion
            // 
            this.btnAuthoriztion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnAuthoriztion.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnAuthoriztion.BorderRadius = 0;
            this.btnAuthoriztion.BorderSize = 1;
            this.btnAuthoriztion.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
            this.btnAuthoriztion.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnAuthoriztion.FlatAppearance.BorderSize = 0;
            this.btnAuthoriztion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.btnAuthoriztion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.btnAuthoriztion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuthoriztion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuthoriztion.ForeColor = System.Drawing.Color.White;
            this.btnAuthoriztion.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
            this.btnAuthoriztion.IconColor = System.Drawing.Color.White;
            this.btnAuthoriztion.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAuthoriztion.IconSize = 40;
            this.btnAuthoriztion.Location = new System.Drawing.Point(5, 4);
            this.btnAuthoriztion.Name = "btnAuthoriztion";
            this.btnAuthoriztion.Size = new System.Drawing.Size(63, 43);
            this.btnAuthoriztion.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnAuthoriztion.TabIndex = 9;
            this.btnAuthoriztion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAuthoriztion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAuthoriztion.UseVisualStyleBackColor = false;
            this.btnAuthoriztion.Click += new System.EventHandler(this.btnAuthoriztion_Click);
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimizar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnMinimizar.FlatAppearance.BorderSize = 0;
            this.btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizar.ForeColor = System.Drawing.Color.White;
            this.btnMinimizar.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimizar.Image")));
            this.btnMinimizar.Location = new System.Drawing.Point(1859, 8);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(30, 30);
            this.btnMinimizar.TabIndex = 2639;
            this.btnMinimizar.UseVisualStyleBackColor = true;
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.BackColor = System.Drawing.Color.Transparent;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel1.ForeColor = System.Drawing.Color.White;
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(188, 5);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(503, 40);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel1.TabIndex = 2133;
            this.rjLabel1.Text = "비전 테스트 프로그램(룰베이스 기반)";
            // 
            // ddmDevice
            // 
            this.ddmDevice.ActiveMenuItem = false;
            this.ddmDevice.BackColor = System.Drawing.Color.White;
            this.ddmDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddmDevice.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconMenuItem2,
            this.iconMenuItem3,
            this.iconMenuItem4,
            this.iconMenuItem5,
            this.iconMenuItem6});
            this.ddmDevice.Name = "ddmDevice";
            this.ddmDevice.OwnerIsMenuButton = false;
            this.ddmDevice.Size = new System.Drawing.Size(134, 114);
            // 
            // iconMenuItem2
            // 
            this.iconMenuItem2.IconChar = FontAwesome.Sharp.IconChar.Camera;
            this.iconMenuItem2.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem2.Name = "iconMenuItem2";
            this.iconMenuItem2.Size = new System.Drawing.Size(133, 22);
            this.iconMenuItem2.Text = "CAMERA";
            // 
            // iconMenuItem3
            // 
            this.iconMenuItem3.IconChar = FontAwesome.Sharp.IconChar.Lightbulb;
            this.iconMenuItem3.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem3.Name = "iconMenuItem3";
            this.iconMenuItem3.Size = new System.Drawing.Size(133, 22);
            this.iconMenuItem3.Text = "LIGHT";
            // 
            // iconMenuItem4
            // 
            this.iconMenuItem4.IconChar = FontAwesome.Sharp.IconChar.None;
            this.iconMenuItem4.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem4.Name = "iconMenuItem4";
            this.iconMenuItem4.Size = new System.Drawing.Size(133, 22);
            this.iconMenuItem4.Text = "PLC";
            this.iconMenuItem4.Visible = false;
            // 
            // iconMenuItem5
            // 
            this.iconMenuItem5.IconChar = FontAwesome.Sharp.IconChar.None;
            this.iconMenuItem5.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem5.Name = "iconMenuItem5";
            this.iconMenuItem5.Size = new System.Drawing.Size(133, 22);
            this.iconMenuItem5.Text = "I/O";
            this.iconMenuItem5.Visible = false;
            // 
            // iconMenuItem6
            // 
            this.iconMenuItem6.IconChar = FontAwesome.Sharp.IconChar.Cog;
            this.iconMenuItem6.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem6.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem6.Name = "iconMenuItem6";
            this.iconMenuItem6.Size = new System.Drawing.Size(133, 22);
            this.iconMenuItem6.Text = "UTIL";
            // 
            // ddmCapture
            // 
            this.ddmCapture.ActiveMenuItem = false;
            this.ddmCapture.BackColor = System.Drawing.Color.White;
            this.ddmCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddmCapture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconMenuItem1});
            this.ddmCapture.Name = "ddmCapture";
            this.ddmCapture.OwnerIsMenuButton = false;
            this.ddmCapture.Size = new System.Drawing.Size(155, 26);
            // 
            // iconMenuItem1
            // 
            this.iconMenuItem1.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
            this.iconMenuItem1.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem1.Name = "iconMenuItem1";
            this.iconMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.iconMenuItem1.Text = "Show Folder";
            // 
            // FormMetroFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1924, 1046);
            this.Controls.Add(this.pnStatusBar);
            this.Controls.Add(this.pnMDI);
            this.Controls.Add(this.pnlTitleBar);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "FormMetroFrame";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMetroFrame_FormClosing);
            this.Load += new System.EventHandler(this.FormMetroFrame_Load);
            this.Shown += new System.EventHandler(this.FormMetroFrame_Shown);
            this.pnMDI.ResumeLayout(false);
            this.pnFormMain.ResumeLayout(false);
            this.pnStatusBar.ResumeLayout(false);
            this.pnStatusBar.PerformLayout();
            this.pnlTitleBar.ResumeLayout(false);
            this.pnlTitleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.biUserOptions)).EndInit();
            this.dmUserOptions.ResumeLayout(false);
            this.ddmDevice.ResumeLayout(false);
            this.ddmCapture.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerAlarm;
        private System.Windows.Forms.Panel pnMDI;
        private System.Windows.Forms.Panel panel3;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmDevice;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmCapture;
        private MetroFramework.Controls.MetroPanel pnFormMain;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJPanel pnlTitleBar;
        private RJCodeUI_M1.RJControls.RJButton rjButton2;
        private System.Windows.Forms.Panel OperatorPanel;
        private RJCodeUI_M1.RJControls.RJButton btnAuthoriztion;
        private RJCodeUI_M1.RJControls.RJLabel lbVersion;
        private RJCodeUI_M1.RJControls.RJPanel pnStatusBar;
        private System.Windows.Forms.Timer timerConnection;
        private RJCodeUI_M1.RJControls.RJDropdownMenu dmUserOptions;
        private FontAwesome.Sharp.IconMenuItem miMyProfile;
        private FontAwesome.Sharp.IconMenuItem miSettings;
        private FontAwesome.Sharp.IconMenuItem miTermsCond;
        private FontAwesome.Sharp.IconMenuItem miHelp;
        private FontAwesome.Sharp.IconMenuItem miLogout;
        private FontAwesome.Sharp.IconMenuItem miExit;
        private RJCodeUI_M1.RJControls.RJMenuIcon biUserOptions;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnMinimizar;
        private MetroFramework.Controls.MetroProgressBar pgbDriveD;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveD;
        private MetroFramework.Controls.MetroProgressBar pgbDriveC;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveC;
        private RJCodeUI_M1.RJControls.RJButton btnAuthoriztionName;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem1;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem2;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem3;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem4;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem5;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem6;
    }
}