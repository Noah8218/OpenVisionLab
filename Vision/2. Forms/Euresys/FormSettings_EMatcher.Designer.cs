#if EURESYS
namespace IntelligentFactory
{
    partial class FormSettings_EMatcher
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

#region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.btnRst1ToolLoad = new System.Windows.Forms.ToolStripButton();
            this.btnRst1ToolSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFit = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.pblImage = new System.Windows.Forms.PictureBox();
            this.lbUseTheshold = new MetroFramework.Controls.MetroTile();
            this.btnImageLoad = new MetroFramework.Controls.MetroButton();
            this.btnImageSave = new MetroFramework.Controls.MetroButton();
            this.btnGrab = new MetroFramework.Controls.MetroButton();
            this.btnApply = new MetroFramework.Controls.MetroButton();
            this.lbThreshold = new MetroFramework.Controls.MetroTile();
            this.trbThreshold = new MetroFramework.Controls.MetroTrackBar();
            this.metroTile2 = new MetroFramework.Controls.MetroTile();
            this.tbScoreMin = new MetroFramework.Controls.MetroTextBox();
            this.btnRun = new MetroFramework.Controls.MetroButton();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.cbIndex = new MetroFramework.Controls.MetroComboBox();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.pbTemplate = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::IntelligentFactory.Properties.Resources.ButtonBackGround;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(3);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton3,
            this.toolStripButton4,
            this.btnRst1ToolLoad,
            this.btnRst1ToolSave,
            this.toolStripSeparator3,
            this.btnFit,
            this.btnZoomIn,
            this.btnZoomOut,
            this.toolStripButton7,
            this.toolStripButton9});
            this.toolStrip1.Location = new System.Drawing.Point(20, 719);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(974, 28);
            this.toolStrip1.TabIndex = 823;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.Checked = true;
            this.toolStripButton2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(125, 50);
            this.toolStripButton2.Text = "Cursor";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoToolTip = false;
            this.toolStripButton3.Checked = true;
            this.toolStripButton3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton3.ForeColor = System.Drawing.Color.White;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(46, 26);
            this.toolStripButton3.Text = "LIVE";
            this.toolStripButton3.ToolTipText = "Expansion";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton4.ForeColor = System.Drawing.Color.White;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(57, 25);
            this.toolStripButton4.Text = "GRAB";
            // 
            // btnRst1ToolLoad
            // 
            this.btnRst1ToolLoad.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolLoad.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolLoad.Name = "btnRst1ToolLoad";
            this.btnRst1ToolLoad.Size = new System.Drawing.Size(56, 25);
            this.btnRst1ToolLoad.Text = "LOAD";
            // 
            // btnRst1ToolSave
            // 
            this.btnRst1ToolSave.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolSave.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolSave.Name = "btnRst1ToolSave";
            this.btnRst1ToolSave.Size = new System.Drawing.Size(55, 25);
            this.btnRst1ToolSave.Text = "SAVE";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // btnFit
            // 
            this.btnFit.Font = new System.Drawing.Font("Arial", 12F);
            this.btnFit.ForeColor = System.Drawing.Color.White;
            this.btnFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(30, 25);
            this.btnFit.Text = "Fit";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(67, 25);
            this.btnZoomIn.Text = "Zoom In";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(80, 25);
            this.btnZoomOut.Text = "Zoom Out";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton7.ForeColor = System.Drawing.Color.White;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(69, 25);
            this.toolStripButton7.Text = "CROSS";
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButton9.ForeColor = System.Drawing.Color.White;
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(38, 25);
            this.toolStripButton9.Text = "ROI";
            // 
            // pblImage
            // 
            this.pblImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pblImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pblImage.Location = new System.Drawing.Point(20, 60);
            this.pblImage.Name = "pblImage";
            this.pblImage.Size = new System.Drawing.Size(984, 720);
            this.pblImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pblImage.TabIndex = 999;
            this.pblImage.TabStop = false;
            this.pblImage.DoubleClick += new System.EventHandler(this.pblImage_DoubleClick);
            this.pblImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseDown);
            this.pblImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseMove);
            this.pblImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pblImage_MouseUp);
            // 
            // lbUseTheshold
            // 
            this.lbUseTheshold.ActiveControl = null;
            this.lbUseTheshold.BackColor = System.Drawing.Color.Transparent;
            this.lbUseTheshold.Location = new System.Drawing.Point(703, 737);
            this.lbUseTheshold.Name = "lbUseTheshold";
            this.lbUseTheshold.Size = new System.Drawing.Size(301, 43);
            this.lbUseTheshold.Style = MetroFramework.MetroColorStyle.Silver;
            this.lbUseTheshold.TabIndex = 1052;
            this.lbUseTheshold.Text = "이진화 이미지 사용";
            this.lbUseTheshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbUseTheshold.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbUseTheshold.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.lbUseTheshold.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.lbUseTheshold.UseSelectable = true;
            this.lbUseTheshold.Click += new System.EventHandler(this.lbUseTheshold_Click);
            // 
            // btnImageLoad
            // 
            this.btnImageLoad.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnImageLoad.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImageLoad.Highlight = true;
            this.btnImageLoad.Location = new System.Drawing.Point(703, 817);
            this.btnImageLoad.Name = "btnImageLoad";
            this.btnImageLoad.Size = new System.Drawing.Size(151, 35);
            this.btnImageLoad.TabIndex = 1051;
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
            this.btnImageSave.TabIndex = 1050;
            this.btnImageSave.Text = "이미지 저장";
            this.btnImageSave.UseSelectable = true;
            this.btnImageSave.Click += new System.EventHandler(this.btnImageSave_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnGrab.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnGrab.Highlight = true;
            this.btnGrab.Location = new System.Drawing.Point(703, 781);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(151, 35);
            this.btnGrab.TabIndex = 1044;
            this.btnGrab.Text = "그랩";
            this.btnGrab.UseSelectable = true;
            // 
            // btnApply
            // 
            this.btnApply.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnApply.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnApply.Highlight = true;
            this.btnApply.Location = new System.Drawing.Point(703, 853);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(151, 35);
            this.btnApply.TabIndex = 1043;
            this.btnApply.Text = "적용";
            this.btnApply.UseSelectable = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lbThreshold
            // 
            this.lbThreshold.ActiveControl = null;
            this.lbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.lbThreshold.Location = new System.Drawing.Point(642, 737);
            this.lbThreshold.Name = "lbThreshold";
            this.lbThreshold.Size = new System.Drawing.Size(59, 43);
            this.lbThreshold.TabIndex = 1042;
            this.lbThreshold.Text = "100";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbThreshold.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbThreshold.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.lbThreshold.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.lbThreshold.UseSelectable = true;
            // 
            // trbThreshold
            // 
            this.trbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.trbThreshold.Location = new System.Drawing.Point(20, 737);
            this.trbThreshold.Maximum = 254;
            this.trbThreshold.Minimum = 1;
            this.trbThreshold.Name = "trbThreshold";
            this.trbThreshold.Size = new System.Drawing.Size(621, 43);
            this.trbThreshold.TabIndex = 1041;
            this.trbThreshold.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.trbThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trbThreshold_Scroll);
            // 
            // metroTile2
            // 
            this.metroTile2.ActiveControl = null;
            this.metroTile2.BackColor = System.Drawing.Color.Transparent;
            this.metroTile2.Location = new System.Drawing.Point(20, 817);
            this.metroTile2.Name = "metroTile2";
            this.metroTile2.Size = new System.Drawing.Size(139, 35);
            this.metroTile2.TabIndex = 1038;
            this.metroTile2.Text = "Score (Min)";
            this.metroTile2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile2.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile2.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile2.UseSelectable = true;
            // 
            // tbScoreMin
            // 
            // 
            // 
            // 
            this.tbScoreMin.CustomButton.Image = null;
            this.tbScoreMin.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbScoreMin.CustomButton.Name = "";
            this.tbScoreMin.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbScoreMin.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbScoreMin.CustomButton.TabIndex = 1;
            this.tbScoreMin.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbScoreMin.CustomButton.UseSelectable = true;
            this.tbScoreMin.DisplayIcon = true;
            this.tbScoreMin.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbScoreMin.Lines = new string[] {
        "0.8"};
            this.tbScoreMin.Location = new System.Drawing.Point(160, 817);
            this.tbScoreMin.MaxLength = 32767;
            this.tbScoreMin.Name = "tbScoreMin";
            this.tbScoreMin.PasswordChar = '\0';
            this.tbScoreMin.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbScoreMin.SelectedText = "";
            this.tbScoreMin.SelectionLength = 0;
            this.tbScoreMin.SelectionStart = 0;
            this.tbScoreMin.ShortcutsEnabled = true;
            this.tbScoreMin.ShowButton = true;
            this.tbScoreMin.ShowClearButton = true;
            this.tbScoreMin.Size = new System.Drawing.Size(200, 35);
            this.tbScoreMin.TabIndex = 1037;
            this.tbScoreMin.Text = "0.8";
            this.tbScoreMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbScoreMin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbScoreMin.UseSelectable = true;
            this.tbScoreMin.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbScoreMin.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnRun
            // 
            this.btnRun.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRun.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRun.Highlight = true;
            this.btnRun.Location = new System.Drawing.Point(853, 781);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(151, 35);
            this.btnRun.TabIndex = 1036;
            this.btnRun.Text = "검사";
            this.btnRun.UseSelectable = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(20, 781);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(139, 35);
            this.metroTile4.TabIndex = 1033;
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
            this.cbIndex.TabIndex = 1032;
            this.cbIndex.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbIndex.UseSelectable = true;
            // 
            // btnSave
            // 
            this.btnSave.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSave.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnSave.Highlight = true;
            this.btnSave.Location = new System.Drawing.Point(853, 853);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(151, 35);
            this.btnSave.TabIndex = 1031;
            this.btnSave.Text = "저장";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Transparent;
            this.metroTile1.Location = new System.Drawing.Point(362, 781);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(139, 107);
            this.metroTile1.TabIndex = 1053;
            this.metroTile1.Text = "Template";
            this.metroTile1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile1.UseSelectable = true;
            // 
            // pbTemplate
            // 
            this.pbTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pbTemplate.Location = new System.Drawing.Point(502, 781);
            this.pbTemplate.Name = "pbTemplate";
            this.pbTemplate.Size = new System.Drawing.Size(199, 107);
            this.pbTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbTemplate.TabIndex = 1054;
            this.pbTemplate.TabStop = false;
            // 
            // FormSettings_EMatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 900);
            this.Controls.Add(this.pbTemplate);
            this.Controls.Add(this.metroTile1);
            this.Controls.Add(this.lbUseTheshold);
            this.Controls.Add(this.btnImageLoad);
            this.Controls.Add(this.btnImageSave);
            this.Controls.Add(this.btnGrab);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.trbThreshold);
            this.Controls.Add(this.metroTile2);
            this.Controls.Add(this.tbScoreMin);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.cbIndex);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pblImage);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormSettings_EMatcher";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Template Matching";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pblImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplate)).EndInit();
            this.ResumeLayout(false);

        }

#endregion
        internal System.Windows.Forms.ToolStrip toolStrip1;
        internal System.Windows.Forms.ToolStripButton toolStripButton2;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton btnRst1ToolLoad;
        private System.Windows.Forms.ToolStripButton btnRst1ToolSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripButton btnFit;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.PictureBox pblImage;
        private MetroFramework.Controls.MetroTile lbUseTheshold;
        private MetroFramework.Controls.MetroButton btnImageLoad;
        private MetroFramework.Controls.MetroButton btnImageSave;
        private MetroFramework.Controls.MetroButton btnGrab;
        private MetroFramework.Controls.MetroButton btnApply;
        private MetroFramework.Controls.MetroTile lbThreshold;
        private MetroFramework.Controls.MetroTrackBar trbThreshold;
        private MetroFramework.Controls.MetroTile metroTile2;
        private MetroFramework.Controls.MetroTextBox tbScoreMin;
        private MetroFramework.Controls.MetroButton btnRun;
        private MetroFramework.Controls.MetroTile metroTile4;
        private MetroFramework.Controls.MetroComboBox cbIndex;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroTile metroTile1;
        private System.Windows.Forms.PictureBox pbTemplate;
    }
}
#endif