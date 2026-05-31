
#if EURESYS
namespace IntelligentFactory
{
    partial class FormSettings_ECircleGuage
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
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.pblImage = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.metroTile6 = new MetroFramework.Controls.MetroTile();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.cbIndex = new MetroFramework.Controls.MetroComboBox();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.btnRun = new MetroFramework.Controls.MetroButton();
            this.metroTile2 = new MetroFramework.Controls.MetroTile();
            this.tbThickness = new MetroFramework.Controls.MetroTextBox();
            this.metroTile3 = new MetroFramework.Controls.MetroTile();
            this.tbSmoothing = new MetroFramework.Controls.MetroTextBox();
            this.trbThreshold = new MetroFramework.Controls.MetroTrackBar();
            this.lbThreshold = new MetroFramework.Controls.MetroTile();
            this.btnApply = new MetroFramework.Controls.MetroButton();
            this.btnGrab = new MetroFramework.Controls.MetroButton();
            this.tbInvalidSpec = new MetroFramework.Controls.MetroTextBox();
            this.cbDirection = new MetroFramework.Controls.MetroComboBox();
            this.metroTile5 = new MetroFramework.Controls.MetroTile();
            this.lbTaktTimems = new MetroFramework.Controls.MetroLabel();
            this.metroTile13 = new MetroFramework.Controls.MetroTile();
            this.btnImageLoad = new MetroFramework.Controls.MetroButton();
            this.btnImageSave = new MetroFramework.Controls.MetroButton();
            this.lbUseInsp = new MetroFramework.Controls.MetroTile();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // btnSave
            // 
            this.btnSave.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSave.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnSave.Highlight = true;
            this.btnSave.Location = new System.Drawing.Point(853, 853);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(151, 35);
            this.btnSave.TabIndex = 1004;
            this.btnSave.Text = "저장";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pblImage
            // 
            this.pblImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pblImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pblImage.InitialImage = global::IntelligentFactory.Properties.Resources.Disconnected;
            this.pblImage.Location = new System.Drawing.Point(20, 60);
            this.pblImage.Margin = new System.Windows.Forms.Padding(10);
            this.pblImage.Name = "pblImage";
            this.pblImage.Padding = new System.Windows.Forms.Padding(1);
            this.pblImage.Size = new System.Drawing.Size(984, 720);
            this.pblImage.TabIndex = 1005;
            this.pblImage.TabStop = false;
            this.pblImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseClick);
            this.pblImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseDown);
            this.pblImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseMove);
            this.pblImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseUp);
            // 
            // metroTile6
            // 
            this.metroTile6.ActiveControl = null;
            this.metroTile6.BackColor = System.Drawing.Color.Transparent;
            this.metroTile6.Location = new System.Drawing.Point(361, 781);
            this.metroTile6.Name = "metroTile6";
            this.metroTile6.Size = new System.Drawing.Size(139, 35);
            this.metroTile6.TabIndex = 1009;
            this.metroTile6.Text = "검사 여부";
            this.metroTile6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile6.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile6.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile6.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile6.UseSelectable = true;
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(20, 781);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(139, 35);
            this.metroTile4.TabIndex = 1007;
            this.metroTile4.Text = "카메라";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile4.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile4.UseSelectable = true;
            // 
            // cbIndex
            // 
            this.cbIndex.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbIndex.FormattingEnabled = true;
            this.cbIndex.ItemHeight = 29;
            this.cbIndex.Items.AddRange(new object[] {
            "Cam#1_X",
            "Cam#2_X",
            "Cam#3_LX",
            "Cam#3_LY",
            "Cam#3_RX",
            "Cam#3_RY",
            "Cam#4_LX",
            "Cam#4_LY",
            "Cam#4_RX",
            "Cam#4_RY"});
            this.cbIndex.Location = new System.Drawing.Point(160, 781);
            this.cbIndex.Name = "cbIndex";
            this.cbIndex.Size = new System.Drawing.Size(200, 35);
            this.cbIndex.TabIndex = 1006;
            this.cbIndex.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbIndex.UseSelectable = true;
            this.cbIndex.SelectedIndexChanged += new System.EventHandler(this.cbIndex_SelectedIndexChanged);
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Transparent;
            this.metroTile1.Location = new System.Drawing.Point(361, 853);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(139, 35);
            this.metroTile1.TabIndex = 1011;
            this.metroTile1.Text = "검사 스펙 (픽셀)";
            this.metroTile1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile1.UseSelectable = true;
            // 
            // btnRun
            // 
            this.btnRun.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRun.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRun.Highlight = true;
            this.btnRun.Location = new System.Drawing.Point(853, 781);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(151, 35);
            this.btnRun.TabIndex = 1012;
            this.btnRun.Text = "검사";
            this.btnRun.UseSelectable = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // metroTile2
            // 
            this.metroTile2.ActiveControl = null;
            this.metroTile2.BackColor = System.Drawing.Color.Transparent;
            this.metroTile2.Location = new System.Drawing.Point(20, 817);
            this.metroTile2.Name = "metroTile2";
            this.metroTile2.Size = new System.Drawing.Size(139, 35);
            this.metroTile2.TabIndex = 1014;
            this.metroTile2.Text = "두께";
            this.metroTile2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile2.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile2.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile2.UseSelectable = true;
            // 
            // tbThickness
            // 
            // 
            // 
            // 
            this.tbThickness.CustomButton.Image = null;
            this.tbThickness.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbThickness.CustomButton.Name = "";
            this.tbThickness.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbThickness.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbThickness.CustomButton.TabIndex = 1;
            this.tbThickness.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbThickness.CustomButton.UseSelectable = true;
            this.tbThickness.DisplayIcon = true;
            this.tbThickness.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbThickness.Lines = new string[] {
        "2"};
            this.tbThickness.Location = new System.Drawing.Point(160, 817);
            this.tbThickness.MaxLength = 32767;
            this.tbThickness.Name = "tbThickness";
            this.tbThickness.PasswordChar = '\0';
            this.tbThickness.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbThickness.SelectedText = "";
            this.tbThickness.SelectionLength = 0;
            this.tbThickness.SelectionStart = 0;
            this.tbThickness.ShortcutsEnabled = true;
            this.tbThickness.ShowButton = true;
            this.tbThickness.ShowClearButton = true;
            this.tbThickness.Size = new System.Drawing.Size(200, 35);
            this.tbThickness.TabIndex = 1013;
            this.tbThickness.Text = "2";
            this.tbThickness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbThickness.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbThickness.UseSelectable = true;
            this.tbThickness.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbThickness.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroTile3
            // 
            this.metroTile3.ActiveControl = null;
            this.metroTile3.BackColor = System.Drawing.Color.Transparent;
            this.metroTile3.Location = new System.Drawing.Point(361, 817);
            this.metroTile3.Name = "metroTile3";
            this.metroTile3.Size = new System.Drawing.Size(139, 35);
            this.metroTile3.TabIndex = 1016;
            this.metroTile3.Text = "필터";
            this.metroTile3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile3.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile3.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile3.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile3.UseSelectable = true;
            // 
            // tbSmoothing
            // 
            // 
            // 
            // 
            this.tbSmoothing.CustomButton.Image = null;
            this.tbSmoothing.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbSmoothing.CustomButton.Name = "";
            this.tbSmoothing.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbSmoothing.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbSmoothing.CustomButton.TabIndex = 1;
            this.tbSmoothing.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbSmoothing.CustomButton.UseSelectable = true;
            this.tbSmoothing.DisplayIcon = true;
            this.tbSmoothing.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbSmoothing.Lines = new string[] {
        "5"};
            this.tbSmoothing.Location = new System.Drawing.Point(501, 817);
            this.tbSmoothing.MaxLength = 32767;
            this.tbSmoothing.Name = "tbSmoothing";
            this.tbSmoothing.PasswordChar = '\0';
            this.tbSmoothing.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSmoothing.SelectedText = "";
            this.tbSmoothing.SelectionLength = 0;
            this.tbSmoothing.SelectionStart = 0;
            this.tbSmoothing.ShortcutsEnabled = true;
            this.tbSmoothing.ShowButton = true;
            this.tbSmoothing.ShowClearButton = true;
            this.tbSmoothing.Size = new System.Drawing.Size(200, 35);
            this.tbSmoothing.TabIndex = 1015;
            this.tbSmoothing.Text = "5";
            this.tbSmoothing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSmoothing.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbSmoothing.UseSelectable = true;
            this.tbSmoothing.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbSmoothing.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // trbThreshold
            // 
            this.trbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.trbThreshold.Location = new System.Drawing.Point(20, 737);
            this.trbThreshold.Maximum = 254;
            this.trbThreshold.Minimum = 1;
            this.trbThreshold.Name = "trbThreshold";
            this.trbThreshold.Size = new System.Drawing.Size(621, 43);
            this.trbThreshold.TabIndex = 1017;
            this.trbThreshold.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.trbThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trbThreshold_Scroll);
            // 
            // lbThreshold
            // 
            this.lbThreshold.ActiveControl = null;
            this.lbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.lbThreshold.Location = new System.Drawing.Point(642, 737);
            this.lbThreshold.Name = "lbThreshold";
            this.lbThreshold.Size = new System.Drawing.Size(59, 43);
            this.lbThreshold.TabIndex = 1018;
            this.lbThreshold.Text = "100";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbThreshold.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbThreshold.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.lbThreshold.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.lbThreshold.UseSelectable = true;
            // 
            // btnApply
            // 
            this.btnApply.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnApply.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnApply.Highlight = true;
            this.btnApply.Location = new System.Drawing.Point(703, 853);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(151, 35);
            this.btnApply.TabIndex = 1019;
            this.btnApply.Text = "적용";
            this.btnApply.UseSelectable = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnGrab.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnGrab.Highlight = true;
            this.btnGrab.Location = new System.Drawing.Point(703, 781);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(151, 35);
            this.btnGrab.TabIndex = 1021;
            this.btnGrab.Text = "그랩";
            this.btnGrab.UseSelectable = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // tbInvalidSpec
            // 
            // 
            // 
            // 
            this.tbInvalidSpec.CustomButton.Image = null;
            this.tbInvalidSpec.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbInvalidSpec.CustomButton.Name = "";
            this.tbInvalidSpec.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbInvalidSpec.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbInvalidSpec.CustomButton.TabIndex = 1;
            this.tbInvalidSpec.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbInvalidSpec.CustomButton.UseSelectable = true;
            this.tbInvalidSpec.DisplayIcon = true;
            this.tbInvalidSpec.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbInvalidSpec.Lines = new string[] {
        "1.0"};
            this.tbInvalidSpec.Location = new System.Drawing.Point(501, 853);
            this.tbInvalidSpec.MaxLength = 32767;
            this.tbInvalidSpec.Name = "tbInvalidSpec";
            this.tbInvalidSpec.PasswordChar = '\0';
            this.tbInvalidSpec.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbInvalidSpec.SelectedText = "";
            this.tbInvalidSpec.SelectionLength = 0;
            this.tbInvalidSpec.SelectionStart = 0;
            this.tbInvalidSpec.ShortcutsEnabled = true;
            this.tbInvalidSpec.ShowButton = true;
            this.tbInvalidSpec.ShowClearButton = true;
            this.tbInvalidSpec.Size = new System.Drawing.Size(200, 35);
            this.tbInvalidSpec.TabIndex = 1022;
            this.tbInvalidSpec.Text = "1.0";
            this.tbInvalidSpec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbInvalidSpec.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbInvalidSpec.UseSelectable = true;
            this.tbInvalidSpec.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbInvalidSpec.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // cbDirection
            // 
            this.cbDirection.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbDirection.FormattingEnabled = true;
            this.cbDirection.ItemHeight = 29;
            this.cbDirection.Items.AddRange(new object[] {
            "백->흑",
            "흑->백"});
            this.cbDirection.Location = new System.Drawing.Point(160, 853);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(200, 35);
            this.cbDirection.TabIndex = 1024;
            this.cbDirection.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbDirection.UseSelectable = true;
            this.cbDirection.SelectedIndexChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
            // 
            // metroTile5
            // 
            this.metroTile5.ActiveControl = null;
            this.metroTile5.BackColor = System.Drawing.Color.Transparent;
            this.metroTile5.Location = new System.Drawing.Point(20, 853);
            this.metroTile5.Name = "metroTile5";
            this.metroTile5.Size = new System.Drawing.Size(139, 35);
            this.metroTile5.TabIndex = 1025;
            this.metroTile5.Text = "방향";
            this.metroTile5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile5.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile5.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile5.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile5.UseSelectable = true;
            // 
            // lbTaktTimems
            // 
            this.lbTaktTimems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTaktTimems.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbTaktTimems.Location = new System.Drawing.Point(855, 737);
            this.lbTaktTimems.Name = "lbTaktTimems";
            this.lbTaktTimems.Size = new System.Drawing.Size(149, 41);
            this.lbTaktTimems.TabIndex = 1027;
            this.lbTaktTimems.Text = "-";
            this.lbTaktTimems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTaktTimems.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbTaktTimems.UseStyleColors = true;
            // 
            // metroTile13
            // 
            this.metroTile13.ActiveControl = null;
            this.metroTile13.BackColor = System.Drawing.Color.Transparent;
            this.metroTile13.Location = new System.Drawing.Point(703, 737);
            this.metroTile13.Name = "metroTile13";
            this.metroTile13.Size = new System.Drawing.Size(151, 41);
            this.metroTile13.TabIndex = 1026;
            this.metroTile13.Text = "검사시간";
            this.metroTile13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile13.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile13.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile13.UseSelectable = true;
            this.metroTile13.UseTileImage = true;
            // 
            // btnImageLoad
            // 
            this.btnImageLoad.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnImageLoad.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImageLoad.Highlight = true;
            this.btnImageLoad.Location = new System.Drawing.Point(703, 817);
            this.btnImageLoad.Name = "btnImageLoad";
            this.btnImageLoad.Size = new System.Drawing.Size(151, 35);
            this.btnImageLoad.TabIndex = 1029;
            this.btnImageLoad.Text = "이미지 불러오기";
            this.btnImageLoad.UseSelectable = true;
            this.btnImageLoad.Click += new System.EventHandler(this.btnImageLoad_Click);
            // 
            // btnImageSave
            // 
            this.btnImageSave.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnImageSave.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImageSave.Highlight = true;
            this.btnImageSave.Location = new System.Drawing.Point(853, 817);
            this.btnImageSave.Name = "btnImageSave";
            this.btnImageSave.Size = new System.Drawing.Size(151, 35);
            this.btnImageSave.TabIndex = 1028;
            this.btnImageSave.Text = "이미지 저장";
            this.btnImageSave.UseSelectable = true;
            this.btnImageSave.Click += new System.EventHandler(this.btnImageSave_Click);
            // 
            // lbUseInsp
            // 
            this.lbUseInsp.ActiveControl = null;
            this.lbUseInsp.BackColor = System.Drawing.Color.Transparent;
            this.lbUseInsp.Location = new System.Drawing.Point(502, 781);
            this.lbUseInsp.Name = "lbUseInsp";
            this.lbUseInsp.Size = new System.Drawing.Size(199, 35);
            this.lbUseInsp.Style = MetroFramework.MetroColorStyle.Lime;
            this.lbUseInsp.TabIndex = 1030;
            this.lbUseInsp.Text = "검사 사용";
            this.lbUseInsp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbUseInsp.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbUseInsp.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.lbUseInsp.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.lbUseInsp.UseSelectable = true;
            this.lbUseInsp.Click += new System.EventHandler(this.lbUseInsp_Click);
            // 
            // FormSettings_ECircleGuage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 900);
            this.Controls.Add(this.lbUseInsp);
            this.Controls.Add(this.btnImageLoad);
            this.Controls.Add(this.btnImageSave);
            this.Controls.Add(this.lbTaktTimems);
            this.Controls.Add(this.metroTile13);
            this.Controls.Add(this.metroTile5);
            this.Controls.Add(this.cbDirection);
            this.Controls.Add(this.tbInvalidSpec);
            this.Controls.Add(this.btnGrab);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.trbThreshold);
            this.Controls.Add(this.metroTile3);
            this.Controls.Add(this.tbSmoothing);
            this.Controls.Add(this.metroTile2);
            this.Controls.Add(this.tbThickness);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.metroTile1);
            this.Controls.Add(this.metroTile6);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.cbIndex);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pblImage);
            this.Name = "FormSettings_ECircleGuage";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "ECircleGuage";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormSettings_ECircleGuage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).EndInit();
            this.ResumeLayout(false);

        }

#endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroButton btnSave;
        private OpenCvSharp.UserInterface.PictureBoxIpl pblImage;
        private MetroFramework.Controls.MetroTile metroTile3;
        private MetroFramework.Controls.MetroTextBox tbSmoothing;
        private MetroFramework.Controls.MetroTile metroTile2;
        private MetroFramework.Controls.MetroTextBox tbThickness;
        private MetroFramework.Controls.MetroButton btnRun;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroTile metroTile6;
        private MetroFramework.Controls.MetroTile metroTile4;
        private MetroFramework.Controls.MetroComboBox cbIndex;
        private MetroFramework.Controls.MetroTile lbThreshold;
        private MetroFramework.Controls.MetroTrackBar trbThreshold;
        private MetroFramework.Controls.MetroButton btnApply;
        private MetroFramework.Controls.MetroButton btnGrab;
        private MetroFramework.Controls.MetroTextBox tbInvalidSpec;
        private MetroFramework.Controls.MetroTile metroTile5;
        private MetroFramework.Controls.MetroComboBox cbDirection;
        private MetroFramework.Controls.MetroLabel lbTaktTimems;
        private MetroFramework.Controls.MetroTile metroTile13;
        private MetroFramework.Controls.MetroButton btnImageLoad;
        private MetroFramework.Controls.MetroButton btnImageSave;
        private MetroFramework.Controls.MetroTile lbUseInsp;
    }
}
#endif