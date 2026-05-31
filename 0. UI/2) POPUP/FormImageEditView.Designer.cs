
using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormImageEditView
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
            this.components = new System.ComponentModel.Container();
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
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.lbGV = new MetroFramework.Controls.MetroLabel();
            this.metroTile11 = new MetroFramework.Controls.MetroTile();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.lbPosition = new MetroFramework.Controls.MetroLabel();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.btnMean = new MetroFramework.Controls.MetroButton();
            this.btnMatrixView = new MetroFramework.Controls.MetroButton();
            this.metroButton2 = new MetroFramework.Controls.MetroButton();
            this.metroButton3 = new MetroFramework.Controls.MetroButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnTrainROI = new RJCodeUI_M1.RJControls.RJButton();
            this.btnDrag = new RJCodeUI_M1.RJControls.RJButton();
            this.btnMultiRoi = new RJCodeUI_M1.RJControls.RJButton();
            this.btnROI = new RJCodeUI_M1.RJControls.RJButton();
            this.btnSaveParameter = new RJCodeUI_M1.RJControls.RJButton();
            this.propertygrid_Parameter = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ibTrainImage = new Cyotek.Windows.Forms.ImageBox();
            this.pnlClientArea.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.splitContainer1);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(1267, 843);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 739);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1014, 28);
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
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(67, 25);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(80, 25);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
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
            // ibSource
            // 
            this.ibSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibSource.Font = new System.Drawing.Font("굴림", 35.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ibSource.ForeColor = System.Drawing.Color.White;
            this.ibSource.Location = new System.Drawing.Point(0, 0);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(909, 775);
            this.ibSource.TabIndex = 824;
            this.ibSource.Text = "ROI Mode";
            this.ibSource.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.ibSource.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ibSource_MouseMove);
            // 
            // lbGV
            // 
            this.lbGV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbGV.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbGV.Location = new System.Drawing.Point(138, 42);
            this.lbGV.Name = "lbGV";
            this.lbGV.Size = new System.Drawing.Size(205, 35);
            this.lbGV.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbGV.TabIndex = 893;
            this.lbGV.Text = "-";
            this.lbGV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbGV.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbGV.UseStyleColors = true;
            // 
            // metroTile11
            // 
            this.metroTile11.ActiveControl = null;
            this.metroTile11.BackColor = System.Drawing.Color.Transparent;
            this.metroTile11.Location = new System.Drawing.Point(6, 42);
            this.metroTile11.Name = "metroTile11";
            this.metroTile11.Size = new System.Drawing.Size(131, 35);
            this.metroTile11.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile11.TabIndex = 892;
            this.metroTile11.Text = "GV";
            this.metroTile11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile11.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile11.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile11.UseSelectable = true;
            this.metroTile11.UseTileImage = true;
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Transparent;
            this.metroTile1.Location = new System.Drawing.Point(6, 6);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(131, 35);
            this.metroTile1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile1.TabIndex = 894;
            this.metroTile1.Text = "Position";
            this.metroTile1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile1.UseSelectable = true;
            this.metroTile1.UseTileImage = true;
            // 
            // lbPosition
            // 
            this.lbPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPosition.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbPosition.Location = new System.Drawing.Point(138, 6);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(205, 35);
            this.lbPosition.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbPosition.TabIndex = 895;
            this.lbPosition.Text = "-";
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbPosition.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbPosition.UseStyleColors = true;
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(222, 768);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(120, 36);
            this.metroButton1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroButton1.TabIndex = 898;
            this.metroButton1.Text = "ROI";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Visible = false;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // btnMean
            // 
            this.btnMean.Location = new System.Drawing.Point(101, 731);
            this.btnMean.Name = "btnMean";
            this.btnMean.Size = new System.Drawing.Size(120, 36);
            this.btnMean.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnMean.TabIndex = 899;
            this.btnMean.Text = "MEAN";
            this.btnMean.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnMean.UseSelectable = true;
            this.btnMean.Visible = false;
            this.btnMean.Click += new System.EventHandler(this.btnMean_Click);
            // 
            // btnMatrixView
            // 
            this.btnMatrixView.Location = new System.Drawing.Point(222, 731);
            this.btnMatrixView.Name = "btnMatrixView";
            this.btnMatrixView.Size = new System.Drawing.Size(120, 36);
            this.btnMatrixView.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnMatrixView.TabIndex = 900;
            this.btnMatrixView.Text = "MATRIX VIEW";
            this.btnMatrixView.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnMatrixView.UseSelectable = true;
            this.btnMatrixView.Visible = false;
            this.btnMatrixView.Click += new System.EventHandler(this.btnMatrixView_Click);
            // 
            // metroButton2
            // 
            this.metroButton2.Location = new System.Drawing.Point(222, 694);
            this.metroButton2.Name = "metroButton2";
            this.metroButton2.Size = new System.Drawing.Size(120, 36);
            this.metroButton2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroButton2.TabIndex = 902;
            this.metroButton2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton2.UseSelectable = true;
            this.metroButton2.Visible = false;
            // 
            // metroButton3
            // 
            this.metroButton3.Location = new System.Drawing.Point(101, 694);
            this.metroButton3.Name = "metroButton3";
            this.metroButton3.Size = new System.Drawing.Size(120, 36);
            this.metroButton3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroButton3.TabIndex = 901;
            this.metroButton3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton3.UseSelectable = true;
            this.metroButton3.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnTrainROI
            // 
            this.btnTrainROI.BackColor = System.Drawing.Color.Transparent;
            this.btnTrainROI.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTrainROI.BorderRadius = 15;
            this.btnTrainROI.BorderSize = 3;
            this.btnTrainROI.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTrainROI.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnTrainROI.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnTrainROI.FlatAppearance.BorderSize = 3;
            this.btnTrainROI.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnTrainROI.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTrainROI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTrainROI.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrainROI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTrainROI.IconChar = FontAwesome.Sharp.IconChar.FileImage;
            this.btnTrainROI.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnTrainROI.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTrainROI.IconSize = 40;
            this.btnTrainROI.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTrainROI.Location = new System.Drawing.Point(727, 6);
            this.btnTrainROI.Name = "btnTrainROI";
            this.btnTrainROI.Size = new System.Drawing.Size(52, 52);
            this.btnTrainROI.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnTrainROI.TabIndex = 2146;
            this.btnTrainROI.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTrainROI.UseVisualStyleBackColor = false;
            this.btnTrainROI.Click += new System.EventHandler(this.Onbtn_Click);
            // 
            // btnDrag
            // 
            this.btnDrag.BackColor = System.Drawing.Color.Transparent;
            this.btnDrag.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDrag.BorderRadius = 15;
            this.btnDrag.BorderSize = 3;
            this.btnDrag.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDrag.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnDrag.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnDrag.FlatAppearance.BorderSize = 3;
            this.btnDrag.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnDrag.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDrag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDrag.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDrag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDrag.IconChar = FontAwesome.Sharp.IconChar.MousePointer;
            this.btnDrag.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDrag.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDrag.IconSize = 40;
            this.btnDrag.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDrag.Location = new System.Drawing.Point(611, 7);
            this.btnDrag.Name = "btnDrag";
            this.btnDrag.Size = new System.Drawing.Size(52, 52);
            this.btnDrag.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnDrag.TabIndex = 2145;
            this.btnDrag.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDrag.UseVisualStyleBackColor = false;
            this.btnDrag.Click += new System.EventHandler(this.Onbtn_Click);
            // 
            // btnMultiRoi
            // 
            this.btnMultiRoi.BackColor = System.Drawing.Color.Transparent;
            this.btnMultiRoi.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnMultiRoi.BorderRadius = 15;
            this.btnMultiRoi.BorderSize = 3;
            this.btnMultiRoi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMultiRoi.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnMultiRoi.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnMultiRoi.FlatAppearance.BorderSize = 3;
            this.btnMultiRoi.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnMultiRoi.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnMultiRoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMultiRoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMultiRoi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnMultiRoi.IconChar = FontAwesome.Sharp.IconChar.ObjectUngroup;
            this.btnMultiRoi.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnMultiRoi.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMultiRoi.IconSize = 40;
            this.btnMultiRoi.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMultiRoi.Location = new System.Drawing.Point(785, 7);
            this.btnMultiRoi.Name = "btnMultiRoi";
            this.btnMultiRoi.Size = new System.Drawing.Size(52, 52);
            this.btnMultiRoi.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnMultiRoi.TabIndex = 2144;
            this.btnMultiRoi.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMultiRoi.UseVisualStyleBackColor = false;
            this.btnMultiRoi.Click += new System.EventHandler(this.Onbtn_Click);
            // 
            // btnROI
            // 
            this.btnROI.BackColor = System.Drawing.Color.Transparent;
            this.btnROI.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnROI.BorderRadius = 15;
            this.btnROI.BorderSize = 3;
            this.btnROI.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnROI.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnROI.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnROI.FlatAppearance.BorderSize = 3;
            this.btnROI.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnROI.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnROI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnROI.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnROI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnROI.IconChar = FontAwesome.Sharp.IconChar.VectorSquare;
            this.btnROI.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnROI.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnROI.IconSize = 40;
            this.btnROI.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnROI.Location = new System.Drawing.Point(669, 6);
            this.btnROI.Name = "btnROI";
            this.btnROI.Size = new System.Drawing.Size(52, 52);
            this.btnROI.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnROI.TabIndex = 2143;
            this.btnROI.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnROI.UseVisualStyleBackColor = false;
            this.btnROI.Click += new System.EventHandler(this.Onbtn_Click);
            // 
            // btnSaveParameter
            // 
            this.btnSaveParameter.BackColor = System.Drawing.Color.White;
            this.btnSaveParameter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.BorderRadius = 15;
            this.btnSaveParameter.BorderSize = 3;
            this.btnSaveParameter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveParameter.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveParameter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveParameter.FlatAppearance.BorderSize = 3;
            this.btnSaveParameter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveParameter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveParameter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveParameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveParameter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveParameter.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveParameter.IconSize = 40;
            this.btnSaveParameter.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveParameter.Location = new System.Drawing.Point(843, 7);
            this.btnSaveParameter.Name = "btnSaveParameter";
            this.btnSaveParameter.Size = new System.Drawing.Size(52, 52);
            this.btnSaveParameter.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveParameter.TabIndex = 2145;
            this.btnSaveParameter.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveParameter.UseVisualStyleBackColor = false;
            this.btnSaveParameter.Click += new System.EventHandler(this.btnCut_Click);
            // 
            // propertygrid_Parameter
            // 
            this.propertygrid_Parameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.propertygrid_Parameter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertygrid_Parameter.Location = new System.Drawing.Point(6, 80);
            this.propertygrid_Parameter.Name = "propertygrid_Parameter";
            this.propertygrid_Parameter.Size = new System.Drawing.Size(336, 328);
            this.propertygrid_Parameter.TabIndex = 2163;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.lbGV);
            this.splitContainer1.Panel2.Controls.Add(this.propertygrid_Parameter);
            this.splitContainer1.Panel2.Controls.Add(this.metroTile11);
            this.splitContainer1.Panel2.Controls.Add(this.metroTile1);
            this.splitContainer1.Panel2.Controls.Add(this.lbPosition);
            this.splitContainer1.Panel2.Controls.Add(this.metroButton2);
            this.splitContainer1.Panel2.Controls.Add(this.metroButton3);
            this.splitContainer1.Panel2.Controls.Add(this.btnMatrixView);
            this.splitContainer1.Panel2.Controls.Add(this.btnMean);
            this.splitContainer1.Panel2.Controls.Add(this.metroButton1);
            this.splitContainer1.Size = new System.Drawing.Size(1267, 843);
            this.splitContainer1.SplitterDistance = 909;
            this.splitContainer1.TabIndex = 2164;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnSaveParameter);
            this.splitContainer2.Panel1.Controls.Add(this.btnMultiRoi);
            this.splitContainer2.Panel1.Controls.Add(this.btnDrag);
            this.splitContainer2.Panel1.Controls.Add(this.btnTrainROI);
            this.splitContainer2.Panel1.Controls.Add(this.btnROI);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ibSource);
            this.splitContainer2.Size = new System.Drawing.Size(909, 843);
            this.splitContainer2.SplitterDistance = 64;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.ibTrainImage);
            this.groupBox1.Location = new System.Drawing.Point(6, 414);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 298);
            this.groupBox1.TabIndex = 2164;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Train Image";
            // 
            // ibTrainImage
            // 
            this.ibTrainImage.Location = new System.Drawing.Point(6, 20);
            this.ibTrainImage.Name = "ibTrainImage";
            this.ibTrainImage.Size = new System.Drawing.Size(324, 272);
            this.ibTrainImage.TabIndex = 2149;
            // 
            // FormImageEditView
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Viewer";
            this.ClientSize = new System.Drawing.Size(1269, 885);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormImageEditView";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormImageView_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
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
        private ImageBox ibSource;
        private MetroFramework.Controls.MetroLabel lbGV;
        private MetroFramework.Controls.MetroTile metroTile11;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroLabel lbPosition;
        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroButton btnMean;
        private MetroFramework.Controls.MetroButton btnMatrixView;
        private MetroFramework.Controls.MetroButton metroButton2;
        private MetroFramework.Controls.MetroButton metroButton3;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJButton btnROI;
        private RJCodeUI_M1.RJControls.RJButton btnMultiRoi;
        private RJCodeUI_M1.RJControls.RJButton btnDrag;
        private RJCodeUI_M1.RJControls.RJButton btnTrainROI;
        private RJCodeUI_M1.RJControls.RJButton btnSaveParameter;
        private System.Windows.Forms.PropertyGrid propertygrid_Parameter;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private ImageBox ibTrainImage;
    }
}
