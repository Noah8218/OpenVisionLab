#if EURESYS
namespace IntelligentFactory
{
    partial class FormSettings_EFilter
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.pblImage = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.cbIndex = new MetroFramework.Controls.MetroComboBox();
            this.btnRun = new MetroFramework.Controls.MetroButton();
            this.metroTile3 = new MetroFramework.Controls.MetroTile();
            this.tbKernelSize = new MetroFramework.Controls.MetroTextBox();
            this.trbThreshold = new MetroFramework.Controls.MetroTrackBar();
            this.lbThreshold = new MetroFramework.Controls.MetroTile();
            this.btnApply = new MetroFramework.Controls.MetroButton();
            this.btnGrab = new MetroFramework.Controls.MetroButton();
            this.cbFilter = new MetroFramework.Controls.MetroComboBox();
            this.metroTile5 = new MetroFramework.Controls.MetroTile();
            this.lbTaktTimems = new MetroFramework.Controls.MetroLabel();
            this.metroTile13 = new MetroFramework.Controls.MetroTile();
            this.btnImageLoad = new MetroFramework.Controls.MetroButton();
            this.btnImageSave = new MetroFramework.Controls.MetroButton();
            this.gridProcess = new MetroFramework.Controls.MetroGrid();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnFilterAdd = new MetroFramework.Controls.MetroButton();
            this.btnFilterDelete = new MetroFramework.Controls.MetroButton();
            this.btnFilterMoveUp = new MetroFramework.Controls.MetroButton();
            this.btnFilterMoveDown = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).BeginInit();
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
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1004;
            this.btnSave.Text = "저장";
            this.btnSave.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.pblImage.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseDoubleClick);
            this.pblImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseDown);
            this.pblImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseMove);
            this.pblImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseUp);
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(20, 781);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(139, 35);
            this.metroTile4.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile4.TabIndex = 1007;
            this.metroTile4.Text = "카메라";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile4.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            "Cam#1_X1",
            "Cam#1_X2",
            "Cam#2_X1",
            "Cam#2_X2",
            "Cam#3_LX1",
            "Cam#3_LX2",
            "Cam#3_LY1",
            "Cam#3_LY2",
            "Cam#3_RX1",
            "Cam#3_RX2",
            "Cam#3_RY1",
            "Cam#3_RY2",
            "Cam#4_LX1",
            "Cam#4_LX2",
            "Cam#4_LY1",
            "Cam#4_LY2",
            "Cam#4_RX1",
            "Cam#4_RX2",
            "Cam#4_RY1",
            "Cam#4_RY2"});
            this.cbIndex.Location = new System.Drawing.Point(160, 781);
            this.cbIndex.Name = "cbIndex";
            this.cbIndex.Size = new System.Drawing.Size(200, 35);
            this.cbIndex.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbIndex.TabIndex = 1006;
            this.cbIndex.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbIndex.UseSelectable = true;
            this.cbIndex.SelectedIndexChanged += new System.EventHandler(this.cbIndex_SelectedIndexChanged);
            // 
            // btnRun
            // 
            this.btnRun.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRun.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRun.Highlight = true;
            this.btnRun.Location = new System.Drawing.Point(853, 781);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(151, 35);
            this.btnRun.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnRun.TabIndex = 1012;
            this.btnRun.Text = "검사";
            this.btnRun.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnRun.UseSelectable = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // metroTile3
            // 
            this.metroTile3.ActiveControl = null;
            this.metroTile3.BackColor = System.Drawing.Color.Transparent;
            this.metroTile3.Location = new System.Drawing.Point(20, 818);
            this.metroTile3.Name = "metroTile3";
            this.metroTile3.Size = new System.Drawing.Size(139, 35);
            this.metroTile3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile3.TabIndex = 1016;
            this.metroTile3.Text = "사이즈";
            this.metroTile3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTile3.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile3.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile3.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile3.UseSelectable = true;
            // 
            // tbKernelSize
            // 
            // 
            // 
            // 
            this.tbKernelSize.CustomButton.Image = null;
            this.tbKernelSize.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbKernelSize.CustomButton.Name = "";
            this.tbKernelSize.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbKernelSize.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbKernelSize.CustomButton.TabIndex = 1;
            this.tbKernelSize.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbKernelSize.CustomButton.UseSelectable = true;
            this.tbKernelSize.DisplayIcon = true;
            this.tbKernelSize.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbKernelSize.Lines = new string[] {
        "5"};
            this.tbKernelSize.Location = new System.Drawing.Point(160, 818);
            this.tbKernelSize.MaxLength = 32767;
            this.tbKernelSize.Name = "tbKernelSize";
            this.tbKernelSize.PasswordChar = '\0';
            this.tbKernelSize.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbKernelSize.SelectedText = "";
            this.tbKernelSize.SelectionLength = 0;
            this.tbKernelSize.SelectionStart = 0;
            this.tbKernelSize.ShortcutsEnabled = true;
            this.tbKernelSize.ShowButton = true;
            this.tbKernelSize.ShowClearButton = true;
            this.tbKernelSize.Size = new System.Drawing.Size(200, 35);
            this.tbKernelSize.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbKernelSize.TabIndex = 1015;
            this.tbKernelSize.Text = "5";
            this.tbKernelSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbKernelSize.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbKernelSize.UseSelectable = true;
            this.tbKernelSize.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbKernelSize.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
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
            this.lbThreshold.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbThreshold.TabIndex = 1018;
            this.lbThreshold.Text = "100";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbThreshold.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.btnApply.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnApply.TabIndex = 1019;
            this.btnApply.Text = "적용";
            this.btnApply.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.btnGrab.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnGrab.TabIndex = 1021;
            this.btnGrab.Text = "그랩";
            this.btnGrab.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnGrab.UseSelectable = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // cbFilter
            // 
            this.cbFilter.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbFilter.FormattingEnabled = true;
            this.cbFilter.ItemHeight = 29;
            this.cbFilter.Items.AddRange(new object[] {
            "Threshold",
            "Threshold_Inv",
            "Threshold_Adaptive",
            "Morp_Open",
            "Morp_Close",
            "Morp_Erode",
            "Morp_Dilate",
            "Uniform",
            "Gaussian",
            "LowPass",
            "HighPass",
            "Gradient",
            "GradientX",
            "GradientY",
            "Sobel",
            "SobelX",
            "SobelY",
            "Laplacian",
            "LaplacianX",
            "LaplacianY"});
            this.cbFilter.Location = new System.Drawing.Point(502, 781);
            this.cbFilter.Name = "cbFilter";
            this.cbFilter.Size = new System.Drawing.Size(200, 35);
            this.cbFilter.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbFilter.TabIndex = 1024;
            this.cbFilter.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbFilter.UseSelectable = true;
            this.cbFilter.SelectedIndexChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
            // 
            // metroTile5
            // 
            this.metroTile5.ActiveControl = null;
            this.metroTile5.BackColor = System.Drawing.Color.Transparent;
            this.metroTile5.Location = new System.Drawing.Point(362, 781);
            this.metroTile5.Name = "metroTile5";
            this.metroTile5.Size = new System.Drawing.Size(139, 35);
            this.metroTile5.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile5.TabIndex = 1025;
            this.metroTile5.Text = "필터";
            this.metroTile5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile5.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.lbTaktTimems.Style = MetroFramework.MetroColorStyle.Teal;
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
            this.metroTile13.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile13.TabIndex = 1026;
            this.metroTile13.Text = "검사시간";
            this.metroTile13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile13.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.btnImageLoad.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnImageLoad.TabIndex = 1029;
            this.btnImageLoad.Text = "이미지 불러오기";
            this.btnImageLoad.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.btnImageSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnImageSave.TabIndex = 1028;
            this.btnImageSave.Text = "이미지 저장";
            this.btnImageSave.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnImageSave.UseSelectable = true;
            this.btnImageSave.Click += new System.EventHandler(this.btnImageSave_Click);
            // 
            // gridProcess
            // 
            this.gridProcess.AllowUserToResizeRows = false;
            this.gridProcess.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.gridProcess.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridProcess.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridProcess.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridProcess.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProcess.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridProcess.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridProcess.EnableHeadersVisualStyles = false;
            this.gridProcess.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gridProcess.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.gridProcess.Location = new System.Drawing.Point(362, 853);
            this.gridProcess.Name = "gridProcess";
            this.gridProcess.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridProcess.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridProcess.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridProcess.RowTemplate.Height = 23;
            this.gridProcess.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridProcess.Size = new System.Drawing.Size(339, 165);
            this.gridProcess.Style = MetroFramework.MetroColorStyle.Teal;
            this.gridProcess.TabIndex = 1030;
            this.gridProcess.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "순서";
            this.Column1.Name = "Column1";
            this.Column1.Width = 95;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "필터";
            this.Column2.Name = "Column2";
            this.Column2.Width = 203;
            // 
            // btnFilterAdd
            // 
            this.btnFilterAdd.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnFilterAdd.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnFilterAdd.Highlight = true;
            this.btnFilterAdd.Location = new System.Drawing.Point(362, 817);
            this.btnFilterAdd.Name = "btnFilterAdd";
            this.btnFilterAdd.Size = new System.Drawing.Size(110, 35);
            this.btnFilterAdd.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnFilterAdd.TabIndex = 1031;
            this.btnFilterAdd.Text = "추가";
            this.btnFilterAdd.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnFilterAdd.UseSelectable = true;
            // 
            // btnFilterDelete
            // 
            this.btnFilterDelete.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnFilterDelete.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnFilterDelete.Highlight = true;
            this.btnFilterDelete.Location = new System.Drawing.Point(473, 817);
            this.btnFilterDelete.Name = "btnFilterDelete";
            this.btnFilterDelete.Size = new System.Drawing.Size(110, 35);
            this.btnFilterDelete.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnFilterDelete.TabIndex = 1032;
            this.btnFilterDelete.Text = "삭제";
            this.btnFilterDelete.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnFilterDelete.UseSelectable = true;
            // 
            // btnFilterMoveUp
            // 
            this.btnFilterMoveUp.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnFilterMoveUp.Highlight = true;
            this.btnFilterMoveUp.Location = new System.Drawing.Point(584, 817);
            this.btnFilterMoveUp.Name = "btnFilterMoveUp";
            this.btnFilterMoveUp.Size = new System.Drawing.Size(58, 35);
            this.btnFilterMoveUp.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnFilterMoveUp.TabIndex = 1033;
            this.btnFilterMoveUp.Text = "↑";
            this.btnFilterMoveUp.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnFilterMoveUp.UseSelectable = true;
            // 
            // btnFilterMoveDown
            // 
            this.btnFilterMoveDown.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnFilterMoveDown.Highlight = true;
            this.btnFilterMoveDown.Location = new System.Drawing.Point(644, 817);
            this.btnFilterMoveDown.Name = "btnFilterMoveDown";
            this.btnFilterMoveDown.Size = new System.Drawing.Size(58, 35);
            this.btnFilterMoveDown.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnFilterMoveDown.TabIndex = 1034;
            this.btnFilterMoveDown.Text = "↓";
            this.btnFilterMoveDown.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnFilterMoveDown.UseSelectable = true;
            // 
            // FormSettings_EFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 1024);
            this.Controls.Add(this.btnFilterMoveDown);
            this.Controls.Add(this.btnFilterMoveUp);
            this.Controls.Add(this.btnFilterDelete);
            this.Controls.Add(this.btnFilterAdd);
            this.Controls.Add(this.gridProcess);
            this.Controls.Add(this.btnImageLoad);
            this.Controls.Add(this.btnImageSave);
            this.Controls.Add(this.lbTaktTimems);
            this.Controls.Add(this.metroTile13);
            this.Controls.Add(this.metroTile5);
            this.Controls.Add(this.cbFilter);
            this.Controls.Add(this.btnGrab);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.trbThreshold);
            this.Controls.Add(this.metroTile3);
            this.Controls.Add(this.tbKernelSize);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.cbIndex);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pblImage);
            this.Name = "FormSettings_EFilter";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "EFilter";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormSettings_ELineGuage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).EndInit();
            this.ResumeLayout(false);

        }

#endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroButton btnSave;
        private OpenCvSharp.UserInterface.PictureBoxIpl pblImage;
        private MetroFramework.Controls.MetroTile metroTile3;
        private MetroFramework.Controls.MetroTextBox tbKernelSize;
        private MetroFramework.Controls.MetroButton btnRun;
        private MetroFramework.Controls.MetroTile metroTile4;
        private MetroFramework.Controls.MetroComboBox cbIndex;
        private MetroFramework.Controls.MetroTile lbThreshold;
        private MetroFramework.Controls.MetroTrackBar trbThreshold;
        private MetroFramework.Controls.MetroButton btnApply;
        private MetroFramework.Controls.MetroButton btnGrab;
        private MetroFramework.Controls.MetroTile metroTile5;
        private MetroFramework.Controls.MetroComboBox cbFilter;
        private MetroFramework.Controls.MetroLabel lbTaktTimems;
        private MetroFramework.Controls.MetroTile metroTile13;
        private MetroFramework.Controls.MetroButton btnImageLoad;
        private MetroFramework.Controls.MetroButton btnImageSave;
        private MetroFramework.Controls.MetroGrid gridProcess;
        private MetroFramework.Controls.MetroButton btnFilterMoveDown;
        private MetroFramework.Controls.MetroButton btnFilterMoveUp;
        private MetroFramework.Controls.MetroButton btnFilterDelete;
        private MetroFramework.Controls.MetroButton btnFilterAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}
#endif