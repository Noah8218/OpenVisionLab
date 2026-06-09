namespace OpenVisionLab
{
    partial class FormMainFrame
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainFrame));
			mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			pnlTitleBar = new RJCodeUI_M1.RJControls.RJPanel();
			biUserOptions = new RJCodeUI_M1.RJControls.RJMenuIcon();
			dmUserOptions = new RJCodeUI_M1.RJControls.RJDropdownMenu(components);
			miTools = new FontAwesome.Sharp.IconMenuItem();
			miImageCompare = new FontAwesome.Sharp.IconMenuItem();
			miLogViewer = new FontAwesome.Sharp.IconMenuItem();
			miSettings = new FontAwesome.Sharp.IconMenuItem();
			btnAuthorizationName = new RJCodeUI_M1.RJControls.RJButton();
			btnScreenCapture = new RJCodeUI_M1.RJControls.RJButton();
			btnClose = new FontAwesome.Sharp.IconButton();
			btnFullScreen = new FontAwesome.Sharp.IconButton();
			btnAuthorizationIcon = new RJCodeUI_M1.RJControls.RJButton();
			btnMinimize = new FontAwesome.Sharp.IconButton();
			rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
			pnMDI = new System.Windows.Forms.Panel();
			pnStatusBar = new RJCodeUI_M1.RJControls.RJPanel();
			pgbDriveD = new System.Windows.Forms.ProgressBar();
			lbDriveD = new RJCodeUI_M1.RJControls.RJLabel();
			lbVersion = new RJCodeUI_M1.RJControls.RJLabel();
			pgbDriveC = new System.Windows.Forms.ProgressBar();
			lbDriveC = new RJCodeUI_M1.RJControls.RJLabel();
			timerConnection = new System.Windows.Forms.Timer(components);
			ddmCapture = new RJCodeUI_M1.RJControls.RJDropdownMenu(components);
			iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
			mainLayoutPanel.SuspendLayout();
			pnlTitleBar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)biUserOptions).BeginInit();
			dmUserOptions.SuspendLayout();
			pnStatusBar.SuspendLayout();
			ddmCapture.SuspendLayout();
			SuspendLayout();
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
			pnlTitleBar.Controls.Add(btnAuthorizationName);
			pnlTitleBar.Controls.Add(btnScreenCapture);
			pnlTitleBar.Controls.Add(btnClose);
			pnlTitleBar.Controls.Add(btnFullScreen);
			pnlTitleBar.Controls.Add(btnAuthorizationIcon);
			pnlTitleBar.Controls.Add(btnMinimize);
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
			biUserOptions.Location = new System.Drawing.Point(1707, 7);
			biUserOptions.Name = "biUserOptions";
			biUserOptions.Size = new System.Drawing.Size(40, 40);
			biUserOptions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			biUserOptions.TabIndex = 1267;
			biUserOptions.TabStop = false;
			// 
			// dmUserOptions
			// 
			dmUserOptions.ActiveMenuItem = false;
			dmUserOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			dmUserOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { miTools, miSettings });
			dmUserOptions.Name = "dmUserOptions";
			dmUserOptions.OwnerIsMenuButton = false;
			dmUserOptions.Size = new System.Drawing.Size(128, 48);
			// 
			// miTools
			// 
			miTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { miImageCompare, miLogViewer });
			miTools.IconChar = FontAwesome.Sharp.IconChar.Tools;
			miTools.IconColor = System.Drawing.Color.FromArgb(47, 168, 210);
			miTools.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miTools.IconSize = 21;
			miTools.Name = "miTools";
			miTools.Size = new System.Drawing.Size(127, 22);
			miTools.Text = "Tools";
			// 
			// miImageCompare
			// 
			miImageCompare.IconChar = FontAwesome.Sharp.IconChar.Images;
			miImageCompare.IconColor = System.Drawing.Color.FromArgb(79, 94, 220);
			miImageCompare.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miImageCompare.IconSize = 21;
			miImageCompare.Name = "miImageCompare";
			miImageCompare.Size = new System.Drawing.Size(175, 22);
			miImageCompare.Text = "Image Compare";
			miImageCompare.Click += miImageCompare_Click;
			// 
			// miLogViewer
			// 
			miLogViewer.IconChar = FontAwesome.Sharp.IconChar.ListAlt;
			miLogViewer.IconColor = System.Drawing.Color.FromArgb(47, 168, 210);
			miLogViewer.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miLogViewer.IconSize = 21;
			miLogViewer.Name = "miLogViewer";
			miLogViewer.Size = new System.Drawing.Size(175, 22);
			miLogViewer.Text = "Log Viewer";
			miLogViewer.Click += miLogViewer_Click;
			// 
			// miSettings
			// 
			miSettings.IconChar = FontAwesome.Sharp.IconChar.Cog;
			miSettings.IconColor = System.Drawing.Color.FromArgb(47, 168, 210);
			miSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
			miSettings.IconSize = 21;
			miSettings.Name = "miSettings";
			miSettings.Size = new System.Drawing.Size(127, 22);
			miSettings.Text = "Settings";
			miSettings.Click += miSettings_Click;
			// 
			// btnAuthorizationName
			// 
			btnAuthorizationName.BackColor = System.Drawing.Color.Black;
			btnAuthorizationName.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthorizationName.BorderRadius = 10;
			btnAuthorizationName.BorderSize = 1;
			btnAuthorizationName.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
			btnAuthorizationName.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			btnAuthorizationName.FlatAppearance.BorderSize = 0;
			btnAuthorizationName.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 0, 0);
			btnAuthorizationName.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 0, 0);
			btnAuthorizationName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnAuthorizationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnAuthorizationName.ForeColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthorizationName.IconChar = FontAwesome.Sharp.IconChar.None;
			btnAuthorizationName.IconColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthorizationName.IconFont = FontAwesome.Sharp.IconFont.Regular;
			btnAuthorizationName.IconSize = 24;
			btnAuthorizationName.Location = new System.Drawing.Point(74, 10);
			btnAuthorizationName.Name = "btnAuthorizationName";
			btnAuthorizationName.Size = new System.Drawing.Size(108, 32);
			btnAuthorizationName.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			btnAuthorizationName.TabIndex = 2647;
			btnAuthorizationName.Text = "Model";
			btnAuthorizationName.UseVisualStyleBackColor = false;
			// 
			// btnScreenCapture
			// 
			btnScreenCapture.BackColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnScreenCapture.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnScreenCapture.BorderRadius = 0;
			btnScreenCapture.BorderSize = 1;
			btnScreenCapture.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
			btnScreenCapture.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			btnScreenCapture.FlatAppearance.BorderSize = 0;
			btnScreenCapture.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(84, 137, 231);
			btnScreenCapture.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(79, 128, 216);
			btnScreenCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnScreenCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnScreenCapture.ForeColor = System.Drawing.Color.White;
			btnScreenCapture.IconChar = FontAwesome.Sharp.IconChar.FileExport;
			btnScreenCapture.IconColor = System.Drawing.Color.White;
			btnScreenCapture.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnScreenCapture.IconSize = 40;
			btnScreenCapture.Location = new System.Drawing.Point(1753, 5);
			btnScreenCapture.Name = "btnScreenCapture";
			btnScreenCapture.Size = new System.Drawing.Size(63, 45);
			btnScreenCapture.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
			btnScreenCapture.TabIndex = 8;
			btnScreenCapture.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnScreenCapture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			btnScreenCapture.UseVisualStyleBackColor = false;
			btnScreenCapture.Click += btnScreenCapture_Click;
			btnScreenCapture.MouseUp += btnScreenCapture_MouseUp;
			// 
			// btnClose
			// 
			btnClose.BackColor = System.Drawing.Color.Transparent;
			btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
			btnClose.FlatAppearance.BorderColor = System.Drawing.Color.White;
			btnClose.FlatAppearance.BorderSize = 0;
			btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnClose.ForeColor = System.Drawing.Color.White;
			btnClose.IconChar = FontAwesome.Sharp.IconChar.WindowClose;
			btnClose.IconColor = System.Drawing.Color.White;
			btnClose.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnClose.IconSize = 24;
			btnClose.Location = new System.Drawing.Point(1888, 8);
			btnClose.Margin = new System.Windows.Forms.Padding(0);
			btnClose.MaximumSize = new System.Drawing.Size(30, 30);
			btnClose.MinimumSize = new System.Drawing.Size(30, 30);
			btnClose.Name = "btnClose";
			btnClose.Size = new System.Drawing.Size(30, 30);
			btnClose.TabIndex = 2638;
			btnClose.TabStop = false;
			btnClose.UseVisualStyleBackColor = false;
			btnClose.Click += btnClose_Click;
			// 
			// btnFullScreen
			// 
			btnFullScreen.BackColor = System.Drawing.Color.Transparent;
			btnFullScreen.Cursor = System.Windows.Forms.Cursors.Hand;
			btnFullScreen.FlatAppearance.BorderColor = System.Drawing.Color.White;
			btnFullScreen.FlatAppearance.BorderSize = 0;
			btnFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnFullScreen.ForeColor = System.Drawing.Color.White;
			btnFullScreen.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
			btnFullScreen.IconColor = System.Drawing.Color.White;
			btnFullScreen.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnFullScreen.IconSize = 24;
			btnFullScreen.Location = new System.Drawing.Point(1853, 8);
			btnFullScreen.Margin = new System.Windows.Forms.Padding(0);
			btnFullScreen.MaximumSize = new System.Drawing.Size(30, 30);
			btnFullScreen.MinimumSize = new System.Drawing.Size(30, 30);
			btnFullScreen.Name = "btnFullScreen";
			btnFullScreen.Size = new System.Drawing.Size(30, 30);
			btnFullScreen.TabIndex = 2640;
			btnFullScreen.TabStop = false;
			btnFullScreen.UseVisualStyleBackColor = false;
			btnFullScreen.Click += btnFullScreen_Click;
			// 
			// btnAuthorizationIcon
			// 
			btnAuthorizationIcon.BackColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthorizationIcon.BorderColor = System.Drawing.Color.FromArgb(90, 146, 246);
			btnAuthorizationIcon.BorderRadius = 0;
			btnAuthorizationIcon.BorderSize = 1;
			btnAuthorizationIcon.Design = RJCodeUI_M1.RJControls.ButtonDesign.IconButton;
			btnAuthorizationIcon.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(83, 97, 212);
			btnAuthorizationIcon.FlatAppearance.BorderSize = 0;
			btnAuthorizationIcon.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(84, 137, 231);
			btnAuthorizationIcon.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(79, 128, 216);
			btnAuthorizationIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnAuthorizationIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnAuthorizationIcon.ForeColor = System.Drawing.Color.White;
			btnAuthorizationIcon.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
			btnAuthorizationIcon.IconColor = System.Drawing.Color.White;
			btnAuthorizationIcon.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnAuthorizationIcon.IconSize = 40;
			btnAuthorizationIcon.Location = new System.Drawing.Point(5, 4);
			btnAuthorizationIcon.Name = "btnAuthorizationIcon";
			btnAuthorizationIcon.Size = new System.Drawing.Size(63, 43);
			btnAuthorizationIcon.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
			btnAuthorizationIcon.TabIndex = 9;
			btnAuthorizationIcon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnAuthorizationIcon.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			btnAuthorizationIcon.UseVisualStyleBackColor = false;
			// 
			// btnMinimize
			// 
			btnMinimize.BackColor = System.Drawing.Color.Transparent;
			btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
			btnMinimize.FlatAppearance.BorderColor = System.Drawing.Color.White;
			btnMinimize.FlatAppearance.BorderSize = 0;
			btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnMinimize.ForeColor = System.Drawing.Color.White;
			btnMinimize.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
			btnMinimize.IconColor = System.Drawing.Color.White;
			btnMinimize.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnMinimize.IconSize = 24;
			btnMinimize.Location = new System.Drawing.Point(1822, 8);
			btnMinimize.Margin = new System.Windows.Forms.Padding(0);
			btnMinimize.MaximumSize = new System.Drawing.Size(30, 30);
			btnMinimize.MinimumSize = new System.Drawing.Size(30, 30);
			btnMinimize.Name = "btnMinimize";
			btnMinimize.Size = new System.Drawing.Size(30, 30);
			btnMinimize.TabIndex = 2639;
			btnMinimize.TabStop = false;
			btnMinimize.UseVisualStyleBackColor = false;
			btnMinimize.Click += btnMinimize_Click;
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
			pgbDriveD.TabIndex = 2109;
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
			pgbDriveC.TabIndex = 2108;
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
			// timerConnection
			// 
			timerConnection.Enabled = true;
			timerConnection.Tick += timerConnection_Tick;
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
			// FormMainFrame
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
			Name = "FormMainFrame";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			FormClosing += FormMainFrame_FormClosing;
			Load += FormMainFrame_Load;
			Shown += FormMainFrame_Shown;
			mainLayoutPanel.ResumeLayout(false);
			pnlTitleBar.ResumeLayout(false);
			pnlTitleBar.PerformLayout();
			((System.ComponentModel.ISupportInitialize)biUserOptions).EndInit();
			dmUserOptions.ResumeLayout(false);
			pnStatusBar.ResumeLayout(false);
			pnStatusBar.PerformLayout();
			ddmCapture.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.Panel pnMDI;
        private RJCodeUI_M1.RJControls.RJDropdownMenu ddmCapture;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJPanel pnlTitleBar;
        private RJCodeUI_M1.RJControls.RJButton btnScreenCapture;
        private RJCodeUI_M1.RJControls.RJButton btnAuthorizationIcon;
        private RJCodeUI_M1.RJControls.RJLabel lbVersion;
        private RJCodeUI_M1.RJControls.RJPanel pnStatusBar;
        private System.Windows.Forms.Timer timerConnection;
        private RJCodeUI_M1.RJControls.RJDropdownMenu dmUserOptions;
        private FontAwesome.Sharp.IconMenuItem miTools;
        private FontAwesome.Sharp.IconMenuItem miImageCompare;
        private FontAwesome.Sharp.IconMenuItem miLogViewer;
        private FontAwesome.Sharp.IconMenuItem miSettings;
        private RJCodeUI_M1.RJControls.RJMenuIcon biUserOptions;
        private FontAwesome.Sharp.IconButton btnClose;
        private FontAwesome.Sharp.IconButton btnFullScreen;
        private FontAwesome.Sharp.IconButton btnMinimize;
        private System.Windows.Forms.ProgressBar pgbDriveD;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveD;
        private System.Windows.Forms.ProgressBar pgbDriveC;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveC;
        private RJCodeUI_M1.RJControls.RJButton btnAuthorizationName;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem1;
    }
}
