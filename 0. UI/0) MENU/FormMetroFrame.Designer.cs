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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMetroFrame));
			timerAlarm = new System.Windows.Forms.Timer(components);
			mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			pnlTitleBar = new RJCodeUI_M1.RJControls.RJPanel();
			biUserOptions = new RJCodeUI_M1.RJControls.RJMenuIcon();
			dmUserOptions = new RJCodeUI_M1.RJControls.RJDropdownMenu(components);
			miMyProfile = new FontAwesome.Sharp.IconMenuItem();
			miSettings = new FontAwesome.Sharp.IconMenuItem();
			miTermsCond = new FontAwesome.Sharp.IconMenuItem();
			miHelp = new FontAwesome.Sharp.IconMenuItem();
			miLogout = new FontAwesome.Sharp.IconMenuItem();
			miExit = new FontAwesome.Sharp.IconMenuItem();
			btnAuthoriztionName = new RJCodeUI_M1.RJControls.RJButton();
			rjButton2 = new RJCodeUI_M1.RJControls.RJButton();
			btnCerrar = new System.Windows.Forms.Button();
			btnAuthoriztion = new RJCodeUI_M1.RJControls.RJButton();
			btnMinimizar = new System.Windows.Forms.Button();
			rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
			pnMDI = new System.Windows.Forms.Panel();
			pnStatusBar = new RJCodeUI_M1.RJControls.RJPanel();
			pgbDriveD = new MetroFramework.Controls.MetroProgressBar();
			lbDriveD = new RJCodeUI_M1.RJControls.RJLabel();
			lbVersion = new RJCodeUI_M1.RJControls.RJLabel();
			pgbDriveC = new MetroFramework.Controls.MetroProgressBar();
			lbDriveC = new RJCodeUI_M1.RJControls.RJLabel();
			panel3 = new System.Windows.Forms.Panel();
			timerConnection = new System.Windows.Forms.Timer(components);
			ddmDevice = new RJCodeUI_M1.RJControls.RJDropdownMenu(components);
			iconMenuItem2 = new FontAwesome.Sharp.IconMenuItem();
			iconMenuItem3 = new FontAwesome.Sharp.IconMenuItem();
			iconMenuItem4 = new FontAwesome.Sharp.IconMenuItem();
			iconMenuItem5 = new FontAwesome.Sharp.IconMenuItem();
			iconMenuItem6 = new FontAwesome.Sharp.IconMenuItem();
			ddmCapture = new RJCodeUI_M1.RJControls.RJDropdownMenu(components);
			iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
			mainLayoutPanel.SuspendLayout();
			pnlTitleBar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)biUserOptions).BeginInit();
			dmUserOptions.SuspendLayout();
			pnStatusBar.SuspendLayout();
			ddmDevice.SuspendLayout();
			ddmCapture.SuspendLayout();
			SuspendLayout();
			// 
			// timerAlarm
			// 
			timerAlarm.Enabled = true;
			timerAlarm.Interval = 500;
			// 
			// mainLayoutPanel
			// 
			mainLayoutPanel.ColumnCount = 1;
			mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			mainLayoutPanel.Controls.Add(pnlTitleBar, 0, 0);
			mainLayoutPanel.Controls.Add(pnMDI, 0, 1);
			mainLayoutPanel.Controls.Add(pnStatusBar, 0, 2);
			mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
			mainLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			mainLayoutPanel.Name = "mainLayoutPanel";
			mainLayoutPanel.RowCount = 3;
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
			mainLayoutPanel.Size = new System.Drawing.Size(1924, 1061);
			mainLayoutPanel.TabIndex = 1261;
			// 
			// pnlTitleBar
			// 
			pnlTitleBar.BackColor = System.Drawing.Color.Black;
			pnlTitleBar.BorderRadius = 0;
			pnlTitleBar.Controls.Add(biUserOptions);
			pnlTitleBar.Controls.Add(btnAuthoriztionName);
			pnlTitleBar.Controls.Add(rjButton2);
			pnlTitleBar.Controls.Add(btnCerrar);
			pnlTitleBar.Controls.Add(btnAuthoriztion);
			pnlTitleBar.Controls.Add(btnMinimizar);
			pnlTitleBar.Controls.Add(rjLabel1);
			pnlTitleBar.Customizable = true;
			pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Fill;
			pnlTitleBar.Location = new System.Drawing.Point(0, 0);
			pnlTitleBar.Margin = new System.Windows.Forms.Padding(0);
			pnlTitleBar.Name = "pnlTitleBar";
			pnlTitleBar.Size = new System.Drawing.Size(1924, 50);
			pnlTitleBar.TabIndex = 1260;
			// 
			// biUserOptions
			// 
			biUserOptions.BackColor = System.Drawing.Color.Transparent;
			biUserOptions.BackIcon = false;
			biUserOptions.Cursor = System.Windows.Forms.Cursors.Hand;
			biUserOptions.Customizable = false;
			biUserOptions.DropdownMenu = dmUserOptions;
			biUserOptions.ForeColor = System.Drawing.Color.WhiteSmoke;
			biUserOptions.IconChar = FontAwesome.Sharp.IconChar.Cog;
			biUserOptions.IconColor = System.Drawing.Color.WhiteSmoke;
			biUserOptions.IconFont = FontAwesome.Sharp.IconFont.Auto;
			biUserOptions.IconSize = 40;
			biUserOptions.Location = new System.Drawing.Point(1746, 4);
			biUserOptions.Name = "biUserOptions";
			biUserOptions.Size = new System.Drawing.Size(40, 40);
			biUserOptions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			biUserOptions.TabIndex = 1267;
			biUserOptions.TabStop = false;
			biUserOptions.Click += biUserOptions_Click_1;
			// 
			// dmUserOptions
			// 
			dmUserOptions.ActiveMenuItem = false;
			dmUserOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			dmUserOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miMyProfile, miSettings, miTermsCond, miHelp, miLogout, miExit });
			dmUserOptions.Name = "dmUserOptions";
			dmUserOptions.OwnerIsMenuButton = false;
			dmUserOptions.Size = new System.Drawing.Size(182, 136);
			// 
			// miMyProfile
			// 
			miMyProfile.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
			miMyProfile.IconColor = System.Drawing.Color.FromArgb(104, 85, 230);
			miMyProfile.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miMyProfile.IconSize = 21;
			miMyProfile.Name = "miMyProfile";
			miMyProfile.Size = new System.Drawing.Size(181, 22);
			miMyProfile.Text = "My Profile";
			// 
			// miSettings
			// 
			miSettings.IconChar = FontAwesome.Sharp.IconChar.Tools;
			miSettings.IconColor = System.Drawing.Color.FromArgb(47, 168, 210);
			miSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miSettings.IconSize = 21;
			miSettings.Name = "miSettings";
			miSettings.Size = new System.Drawing.Size(181, 22);
			miSettings.Text = "Settings";
			miSettings.Click += miSettings_Click;
			// 
			// miTermsCond
			// 
			miTermsCond.IconChar = FontAwesome.Sharp.IconChar.ShieldAlt;
			miTermsCond.IconColor = System.Drawing.Color.FromArgb(70, 132, 235);
			miTermsCond.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miTermsCond.IconSize = 21;
			miTermsCond.Name = "miTermsCond";
			miTermsCond.Size = new System.Drawing.Size(181, 22);
			miTermsCond.Text = "Terms and Cond";
			// 
			// miHelp
			// 
			miHelp.IconChar = FontAwesome.Sharp.IconChar.QuestionCircle;
			miHelp.IconColor = System.Drawing.Color.FromArgb(238, 96, 112);
			miHelp.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miHelp.IconSize = 21;
			miHelp.Name = "miHelp";
			miHelp.Size = new System.Drawing.Size(181, 22);
			miHelp.Text = "Help";
			// 
			// miLogout
			// 
			miLogout.IconChar = FontAwesome.Sharp.IconChar.SignOutAlt;
			miLogout.IconColor = System.Drawing.Color.FromArgb(73, 84, 228);
			miLogout.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miLogout.IconSize = 21;
			miLogout.Name = "miLogout";
			miLogout.Size = new System.Drawing.Size(181, 22);
			miLogout.Text = "Logout";
			// 
			// miExit
			// 
			miExit.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
			miExit.IconColor = System.Drawing.Color.FromArgb(220, 37, 118);
			miExit.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miExit.IconSize = 21;
			miExit.Name = "miExit";
			miExit.Size = new System.Drawing.Size(181, 22);
			miExit.Text = "Exit";
			// 
			// btnAuthoriztionName
			// 
			btnAuthoriztionName.BackColor = System.Drawing.Color.Black;
			btnAuthoriztionName.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthoriztionName.BorderRadius = 10;
			btnAuthoriztionName.BorderSize = 1;
			btnAuthoriztionName.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
			btnAuthoriztionName.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			btnAuthoriztionName.FlatAppearance.BorderSize = 0;
			btnAuthoriztionName.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 0);
			btnAuthoriztionName.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 0, 0);
			btnAuthoriztionName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnAuthoriztionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnAuthoriztionName.ForeColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthoriztionName.IconChar = FontAwesome.Sharp.IconChar.None;
			btnAuthoriztionName.IconColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthoriztionName.IconFont = FontAwesome.Sharp.IconFont.Regular;
			btnAuthoriztionName.IconSize = 24;
			btnAuthoriztionName.Location = new System.Drawing.Point(74, 10);
			btnAuthoriztionName.Name = "btnAuthoriztionName";
			btnAuthoriztionName.Size = new System.Drawing.Size(108, 32);
			btnAuthoriztionName.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			btnAuthoriztionName.TabIndex = 2647;
			btnAuthoriztionName.Text = "Model";
			btnAuthoriztionName.UseVisualStyleBackColor = false;
			// 
			// rjButton2
			// 
			rjButton2.BackColor = System.Drawing.Color.FromArgb(90, 146, 246);
			rjButton2.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			rjButton2.BorderRadius = 0;
			rjButton2.BorderSize = 1;
			rjButton2.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
			rjButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			rjButton2.FlatAppearance.BorderSize = 0;
			rjButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(84, 137, 231);
			rjButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(79, 128, 216);
			rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			rjButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			rjButton2.ForeColor = System.Drawing.Color.White;
			rjButton2.IconChar = FontAwesome.Sharp.IconChar.FileExport;
			rjButton2.IconColor = System.Drawing.Color.White;
			rjButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
			rjButton2.IconSize = 40;
			rjButton2.Location = new System.Drawing.Point(1792, 2);
			rjButton2.Name = "rjButton2";
			rjButton2.Size = new System.Drawing.Size(63, 45);
			rjButton2.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
			rjButton2.TabIndex = 8;
			rjButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			rjButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			rjButton2.UseVisualStyleBackColor = false;
			rjButton2.Click += btnScreenCapture_Click;
			rjButton2.MouseUp += btnScreenCapture_MouseUp;
			// 
			// btnCerrar
			// 
			btnCerrar.Cursor = System.Windows.Forms.Cursors.Hand;
			btnCerrar.FlatAppearance.BorderColor = System.Drawing.Color.White;
			btnCerrar.FlatAppearance.BorderSize = 0;
			btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnCerrar.ForeColor = System.Drawing.Color.White;
			btnCerrar.Image = (System.Drawing.Image)resources.GetObject("btnCerrar.Image");
			btnCerrar.Location = new System.Drawing.Point(1888, 8);
			btnCerrar.Name = "btnCerrar";
			btnCerrar.Size = new System.Drawing.Size(30, 30);
			btnCerrar.TabIndex = 2638;
			btnCerrar.UseVisualStyleBackColor = true;
			btnCerrar.Click += btnCerrar_Click;
			// 
			// btnAuthoriztion
			// 
			btnAuthoriztion.BackColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthoriztion.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthoriztion.BorderRadius = 0;
			btnAuthoriztion.BorderSize = 1;
			btnAuthoriztion.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
			btnAuthoriztion.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			btnAuthoriztion.FlatAppearance.BorderSize = 0;
			btnAuthoriztion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(84, 137, 231);
			btnAuthoriztion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(79, 128, 216);
			btnAuthoriztion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnAuthoriztion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnAuthoriztion.ForeColor = System.Drawing.Color.White;
			btnAuthoriztion.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
			btnAuthoriztion.IconColor = System.Drawing.Color.White;
			btnAuthoriztion.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnAuthoriztion.IconSize = 40;
			btnAuthoriztion.Location = new System.Drawing.Point(5, 4);
			btnAuthoriztion.Name = "btnAuthoriztion";
			btnAuthoriztion.Size = new System.Drawing.Size(63, 43);
			btnAuthoriztion.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
			btnAuthoriztion.TabIndex = 9;
			btnAuthoriztion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnAuthoriztion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			btnAuthoriztion.UseVisualStyleBackColor = false;
			// 
			// btnMinimizar
			// 
			btnMinimizar.Cursor = System.Windows.Forms.Cursors.Hand;
			btnMinimizar.FlatAppearance.BorderColor = System.Drawing.Color.White;
			btnMinimizar.FlatAppearance.BorderSize = 0;
			btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnMinimizar.ForeColor = System.Drawing.Color.White;
			btnMinimizar.Image = (System.Drawing.Image)resources.GetObject("btnMinimizar.Image");
			btnMinimizar.Location = new System.Drawing.Point(1859, 8);
			btnMinimizar.Name = "btnMinimizar";
			btnMinimizar.Size = new System.Drawing.Size(30, 30);
			btnMinimizar.TabIndex = 2639;
			btnMinimizar.UseVisualStyleBackColor = true;
			btnMinimizar.Click += btnMinimizar_Click;
			// 
			// rjLabel1
			// 
			rjLabel1.AutoSize = true;
			rjLabel1.BackColor = System.Drawing.Color.Transparent;
			rjLabel1.Font = new System.Drawing.Font("Verdana", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			rjLabel1.ForeColor = System.Drawing.Color.White;
			rjLabel1.LinkLabel = false;
			rjLabel1.Location = new System.Drawing.Point(188, 5);
			rjLabel1.Name = "rjLabel1";
			rjLabel1.Size = new System.Drawing.Size(503, 40);
			rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
			rjLabel1.TabIndex = 2133;
			rjLabel1.Text = "비전 테스트 프로그램(룰베이스 기반)";
			// 
			// pnMDI
			// 
			pnMDI.BackColor = System.Drawing.Color.FromArgb(240, 245, 249);
			pnMDI.Dock = System.Windows.Forms.DockStyle.Fill;
			pnMDI.Location = new System.Drawing.Point(0, 50);
			pnMDI.Margin = new System.Windows.Forms.Padding(0);
			pnMDI.Name = "pnMDI";
			pnMDI.Size = new System.Drawing.Size(1924, 978);
			pnMDI.TabIndex = 1258;
			// 
			// pnStatusBar
			// 
			pnStatusBar.BackColor = System.Drawing.Color.Black;
			pnStatusBar.BorderRadius = 0;
			pnStatusBar.Controls.Add(pgbDriveD);
			pnStatusBar.Controls.Add(lbDriveD);
			pnStatusBar.Controls.Add(lbVersion);
			pnStatusBar.Controls.Add(pgbDriveC);
			pnStatusBar.Controls.Add(lbDriveC);
			pnStatusBar.Customizable = true;
			pnStatusBar.Dock = System.Windows.Forms.DockStyle.Fill;
			pnStatusBar.Location = new System.Drawing.Point(0, 1028);
			pnStatusBar.Margin = new System.Windows.Forms.Padding(0);
			pnStatusBar.Name = "pnStatusBar";
			pnStatusBar.Size = new System.Drawing.Size(1924, 33);
			pnStatusBar.TabIndex = 0;
			// 
			// pgbDriveD
			// 
			pgbDriveD.Location = new System.Drawing.Point(268, 21);
			pgbDriveD.Name = "pgbDriveD";
			pgbDriveD.Size = new System.Drawing.Size(218, 10);
			pgbDriveD.Style = MetroFramework.MetroColorStyle.Lime;
			pgbDriveD.TabIndex = 2109;
			pgbDriveD.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// lbDriveD
			// 
			lbDriveD.AutoSize = true;
			lbDriveD.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			lbDriveD.ForeColor = System.Drawing.Color.White;
			lbDriveD.LinkLabel = false;
			lbDriveD.Location = new System.Drawing.Point(265, 6);
			lbDriveD.Name = "lbDriveD";
			lbDriveD.Size = new System.Drawing.Size(164, 13);
			lbDriveD.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
			lbDriveD.TabIndex = 2111;
			lbDriveD.Text = "(D:) : 00%    (000/000 GB)";
			// 
			// lbVersion
			// 
			lbVersion.AutoSize = true;
			lbVersion.BackColor = System.Drawing.Color.Transparent;
			lbVersion.Dock = System.Windows.Forms.DockStyle.Right;
			lbVersion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			lbVersion.ForeColor = System.Drawing.Color.White;
			lbVersion.LinkLabel = false;
			lbVersion.Location = new System.Drawing.Point(1745, 0);
			lbVersion.Name = "lbVersion";
			lbVersion.Size = new System.Drawing.Size(179, 18);
			lbVersion.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
			lbVersion.TabIndex = 2122;
			lbVersion.Text = "Version 2.5 - 211007";
			// 
			// pgbDriveC
			// 
			pgbDriveC.Location = new System.Drawing.Point(10, 20);
			pgbDriveC.Name = "pgbDriveC";
			pgbDriveC.Size = new System.Drawing.Size(218, 10);
			pgbDriveC.Style = MetroFramework.MetroColorStyle.Lime;
			pgbDriveC.TabIndex = 2108;
			pgbDriveC.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// lbDriveC
			// 
			lbDriveC.AutoSize = true;
			lbDriveC.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			lbDriveC.ForeColor = System.Drawing.Color.White;
			lbDriveC.LinkLabel = false;
			lbDriveC.Location = new System.Drawing.Point(7, 6);
			lbDriveC.Name = "lbDriveC";
			lbDriveC.Size = new System.Drawing.Size(164, 13);
			lbDriveC.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
			lbDriveC.TabIndex = 2110;
			lbDriveC.Text = "(C:) : 00%    (000/000 GB)";
			// 
			// panel3
			// 
			panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			panel3.Location = new System.Drawing.Point(0, 0);
			panel3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(1910, 47);
			panel3.TabIndex = 1949;
			// 
			// timerConnection
			// 
			timerConnection.Enabled = true;
			timerConnection.Tick += timerConnection_Tick;
			// 
			// ddmDevice
			// 
			ddmDevice.ActiveMenuItem = false;
			ddmDevice.BackColor = System.Drawing.Color.White;
			ddmDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			ddmDevice.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { iconMenuItem2, iconMenuItem3, iconMenuItem4, iconMenuItem5, iconMenuItem6 });
			ddmDevice.Name = "ddmDevice";
			ddmDevice.OwnerIsMenuButton = false;
			ddmDevice.Size = new System.Drawing.Size(134, 114);
			// 
			// iconMenuItem2
			// 
			iconMenuItem2.IconChar = FontAwesome.Sharp.IconChar.Camera;
			iconMenuItem2.IconColor = System.Drawing.Color.Black;
			iconMenuItem2.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem2.Name = "iconMenuItem2";
			iconMenuItem2.Size = new System.Drawing.Size(133, 22);
			iconMenuItem2.Text = "CAMERA";
			// 
			// iconMenuItem3
			// 
			iconMenuItem3.IconChar = FontAwesome.Sharp.IconChar.Lightbulb;
			iconMenuItem3.IconColor = System.Drawing.Color.Black;
			iconMenuItem3.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem3.Name = "iconMenuItem3";
			iconMenuItem3.Size = new System.Drawing.Size(133, 22);
			iconMenuItem3.Text = "LIGHT";
			// 
			// iconMenuItem4
			// 
			iconMenuItem4.IconChar = FontAwesome.Sharp.IconChar.None;
			iconMenuItem4.IconColor = System.Drawing.Color.Black;
			iconMenuItem4.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem4.Name = "iconMenuItem4";
			iconMenuItem4.Size = new System.Drawing.Size(133, 22);
			iconMenuItem4.Text = "PLC";
			iconMenuItem4.Visible = false;
			// 
			// iconMenuItem5
			// 
			iconMenuItem5.IconChar = FontAwesome.Sharp.IconChar.None;
			iconMenuItem5.IconColor = System.Drawing.Color.Black;
			iconMenuItem5.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem5.Name = "iconMenuItem5";
			iconMenuItem5.Size = new System.Drawing.Size(133, 22);
			iconMenuItem5.Text = "I/O";
			iconMenuItem5.Visible = false;
			// 
			// iconMenuItem6
			// 
			iconMenuItem6.IconChar = FontAwesome.Sharp.IconChar.Cog;
			iconMenuItem6.IconColor = System.Drawing.Color.Black;
			iconMenuItem6.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem6.Name = "iconMenuItem6";
			iconMenuItem6.Size = new System.Drawing.Size(133, 22);
			iconMenuItem6.Text = "UTIL";
			// 
			// ddmCapture
			// 
			ddmCapture.ActiveMenuItem = false;
			ddmCapture.BackColor = System.Drawing.Color.White;
			ddmCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			ddmCapture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { iconMenuItem1 });
			ddmCapture.Name = "ddmCapture";
			ddmCapture.OwnerIsMenuButton = false;
			ddmCapture.Size = new System.Drawing.Size(155, 26);
			// 
			// iconMenuItem1
			// 
			iconMenuItem1.IconChar = FontAwesome.Sharp.IconChar.FolderOpen;
			iconMenuItem1.IconColor = System.Drawing.Color.Black;
			iconMenuItem1.IconFont = FontAwesome.Sharp.IconFont.Auto;
			iconMenuItem1.Name = "iconMenuItem1";
			iconMenuItem1.Size = new System.Drawing.Size(154, 22);
			iconMenuItem1.Text = "Show Folder";
			// 
			// FormMetroFrame
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.Black;
			BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			ClientSize = new System.Drawing.Size(1924, 1061);
			Controls.Add(mainLayoutPanel);
			Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			Name = "FormMetroFrame";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			FormClosing += FormMetroFrame_FormClosing;
			Load += FormMetroFrame_Load;
			Shown += FormMetroFrame_Shown;
			mainLayoutPanel.ResumeLayout(false);
			pnlTitleBar.ResumeLayout(false);
			pnlTitleBar.PerformLayout();
			((System.ComponentModel.ISupportInitialize)biUserOptions).EndInit();
			dmUserOptions.ResumeLayout(false);
			pnStatusBar.ResumeLayout(false);
			pnStatusBar.PerformLayout();
			ddmDevice.ResumeLayout(false);
			ddmCapture.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.Timer timerAlarm;
		private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.Panel pnMDI;
        private System.Windows.Forms.Panel panel3;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmDevice;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmCapture;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJPanel pnlTitleBar;
        private RJCodeUI_M1.RJControls.RJButton rjButton2;
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
