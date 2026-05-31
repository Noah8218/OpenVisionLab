#if MATROX
namespace IntelligentFactory
{
    partial class FormSettings_MROI
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
            this.lbROI = new MetroFramework.Controls.MetroLabel();
            this.metroTile2 = new MetroFramework.Controls.MetroTile();
            this.pnDisplay = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnOK = new MetroFramework.Controls.MetroButton();
            this.toolStrip1.SuspendLayout();
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
            this.toolStrip1.Location = new System.Drawing.Point(17, 779);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(835, 30);
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
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
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
            this.toolStripButton3.Size = new System.Drawing.Size(46, 28);
            this.toolStripButton3.Text = "LIVE";
            this.toolStripButton3.ToolTipText = "Expansion";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton4.ForeColor = System.Drawing.Color.White;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(57, 27);
            this.toolStripButton4.Text = "GRAB";
            // 
            // btnRst1ToolLoad
            // 
            this.btnRst1ToolLoad.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolLoad.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolLoad.Name = "btnRst1ToolLoad";
            this.btnRst1ToolLoad.Size = new System.Drawing.Size(56, 27);
            this.btnRst1ToolLoad.Text = "LOAD";
            // 
            // btnRst1ToolSave
            // 
            this.btnRst1ToolSave.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolSave.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolSave.Name = "btnRst1ToolSave";
            this.btnRst1ToolSave.Size = new System.Drawing.Size(55, 27);
            this.btnRst1ToolSave.Text = "SAVE";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 30);
            // 
            // btnFit
            // 
            this.btnFit.Font = new System.Drawing.Font("Arial", 12F);
            this.btnFit.ForeColor = System.Drawing.Color.White;
            this.btnFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(30, 27);
            this.btnFit.Text = "Fit";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(67, 27);
            this.btnZoomIn.Text = "Zoom In";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(80, 27);
            this.btnZoomOut.Text = "Zoom Out";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton7.ForeColor = System.Drawing.Color.White;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(69, 27);
            this.toolStripButton7.Text = "CROSS";
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButton9.ForeColor = System.Drawing.Color.White;
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(38, 27);
            this.toolStripButton9.Text = "ROI";
            // 
            // lbROI
            // 
            this.lbROI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbROI.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lbROI.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lbROI.Location = new System.Drawing.Point(137, 914);
            this.lbROI.Name = "lbROI";
            this.lbROI.Size = new System.Drawing.Size(460, 39);
            this.lbROI.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbROI.TabIndex = 1053;
            this.lbROI.Text = "000000";
            this.lbROI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbROI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbROI.UseStyleColors = true;
            // 
            // metroTile2
            // 
            this.metroTile2.ActiveControl = null;
            this.metroTile2.BackColor = System.Drawing.Color.Transparent;
            this.metroTile2.Location = new System.Drawing.Point(17, 914);
            this.metroTile2.Name = "metroTile2";
            this.metroTile2.Size = new System.Drawing.Size(119, 39);
            this.metroTile2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTile2.TabIndex = 1052;
            this.metroTile2.Text = "ROI";
            this.metroTile2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile2.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile2.UseSelectable = true;
            this.metroTile2.UseTileImage = true;
            // 
            // pnDisplay
            // 
            this.pnDisplay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnDisplay.Location = new System.Drawing.Point(18, 68);
            this.pnDisplay.Name = "pnDisplay";
            this.pnDisplay.Size = new System.Drawing.Size(840, 844);
            this.pnDisplay.TabIndex = 1054;
            this.pnDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnDisplay_MouseMove);
            // 
            // btnCancel
            // 
            this.btnCancel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnCancel.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnCancel.Highlight = true;
            this.btnCancel.Location = new System.Drawing.Point(729, 914);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(129, 38);
            this.btnCancel.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnCancel.TabIndex = 1056;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnOK.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnOK.Highlight = true;
            this.btnOK.Location = new System.Drawing.Point(598, 914);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(130, 38);
            this.btnOK.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnOK.TabIndex = 1055;
            this.btnOK.Text = "OK";
            this.btnOK.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnOK.UseSelectable = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormSettings_MROI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 975);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnDisplay);
            this.Controls.Add(this.lbROI);
            this.Controls.Add(this.metroTile2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormSettings_MROI";
            this.Padding = new System.Windows.Forms.Padding(17, 65, 17, 22);
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "ROI";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private MetroFramework.Controls.MetroLabel lbROI;
        private MetroFramework.Controls.MetroTile metroTile2;
        private System.Windows.Forms.Panel pnDisplay;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnOK;
    }
}

#endif