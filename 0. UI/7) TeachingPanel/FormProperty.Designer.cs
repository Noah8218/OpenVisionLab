using Cyotek.Windows.Forms;

namespace KtemVisionSystem
{
    partial class FormProperty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProperty));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid_Parameter = new System.Windows.Forms.PropertyGrid();
            this.rdoRightEdgePara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoLeftEdgePara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoBlobPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoContourPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.btnSaveVisionParam = new RJCodeUI_M1.RJControls.RJButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.splitContainer1.Panel1.Controls.Add(this.rdoRightEdgePara);
            this.splitContainer1.Panel1.Controls.Add(this.rdoLeftEdgePara);
            this.splitContainer1.Panel1.Controls.Add(this.rdoBlobPara);
            this.splitContainer1.Panel1.Controls.Add(this.rdoContourPara);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(612, 531);
            this.splitContainer1.SplitterDistance = 39;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2148;
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
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid_Parameter);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.splitContainer2.Panel2.Controls.Add(this.btnSaveVisionParam);
            this.splitContainer2.Size = new System.Drawing.Size(612, 491);
            this.splitContainer2.SplitterDistance = 350;
            this.splitContainer2.TabIndex = 2105;
            // 
            // propertyGrid_Parameter
            // 
            this.propertyGrid_Parameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.propertyGrid_Parameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_Parameter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGrid_Parameter.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid_Parameter.Name = "propertyGrid_Parameter";
            this.propertyGrid_Parameter.Size = new System.Drawing.Size(612, 350);
            this.propertyGrid_Parameter.TabIndex = 2104;
            // 
            // rdoRightEdgePara
            // 
            this.rdoRightEdgePara.AutoSize = true;
            this.rdoRightEdgePara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoRightEdgePara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoRightEdgePara.Customizable = true;
            this.rdoRightEdgePara.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoRightEdgePara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoRightEdgePara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoRightEdgePara.Location = new System.Drawing.Point(232, 0);
            this.rdoRightEdgePara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoRightEdgePara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoRightEdgePara.Name = "rdoRightEdgePara";
            this.rdoRightEdgePara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoRightEdgePara.Size = new System.Drawing.Size(85, 39);
            this.rdoRightEdgePara.TabIndex = 2136;
            this.rdoRightEdgePara.Text = "Edge(R)";
            this.rdoRightEdgePara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoRightEdgePara.UseVisualStyleBackColor = true;
            this.rdoRightEdgePara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoLeftEdgePara
            // 
            this.rdoLeftEdgePara.AutoSize = true;
            this.rdoLeftEdgePara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoLeftEdgePara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoLeftEdgePara.Customizable = true;
            this.rdoLeftEdgePara.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoLeftEdgePara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoLeftEdgePara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoLeftEdgePara.Location = new System.Drawing.Point(148, 0);
            this.rdoLeftEdgePara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoLeftEdgePara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoLeftEdgePara.Name = "rdoLeftEdgePara";
            this.rdoLeftEdgePara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoLeftEdgePara.Size = new System.Drawing.Size(84, 39);
            this.rdoLeftEdgePara.TabIndex = 2135;
            this.rdoLeftEdgePara.Text = "Edge(L)";
            this.rdoLeftEdgePara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoLeftEdgePara.UseVisualStyleBackColor = true;
            this.rdoLeftEdgePara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoBlobPara
            // 
            this.rdoBlobPara.AutoSize = true;
            this.rdoBlobPara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoBlobPara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBlobPara.Customizable = true;
            this.rdoBlobPara.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoBlobPara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBlobPara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoBlobPara.Location = new System.Drawing.Point(86, 0);
            this.rdoBlobPara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoBlobPara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoBlobPara.Name = "rdoBlobPara";
            this.rdoBlobPara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoBlobPara.Size = new System.Drawing.Size(62, 39);
            this.rdoBlobPara.TabIndex = 2146;
            this.rdoBlobPara.Text = "Blob";
            this.rdoBlobPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoBlobPara.UseVisualStyleBackColor = true;
            this.rdoBlobPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoContourPara
            // 
            this.rdoContourPara.AutoSize = true;
            this.rdoContourPara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoContourPara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoContourPara.Customizable = true;
            this.rdoContourPara.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoContourPara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoContourPara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoContourPara.Location = new System.Drawing.Point(0, 0);
            this.rdoContourPara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoContourPara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoContourPara.Name = "rdoContourPara";
            this.rdoContourPara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoContourPara.Size = new System.Drawing.Size(86, 39);
            this.rdoContourPara.TabIndex = 2137;
            this.rdoContourPara.Text = "Contour";
            this.rdoContourPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoContourPara.UseVisualStyleBackColor = true;
            this.rdoContourPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // btnSaveVisionParam
            // 
            this.btnSaveVisionParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveVisionParam.BackColor = System.Drawing.Color.White;
            this.btnSaveVisionParam.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.BorderRadius = 15;
            this.btnSaveVisionParam.BorderSize = 3;
            this.btnSaveVisionParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveVisionParam.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveVisionParam.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveVisionParam.FlatAppearance.BorderSize = 3;
            this.btnSaveVisionParam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveVisionParam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveVisionParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveVisionParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveVisionParam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveVisionParam.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveVisionParam.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveVisionParam.IconSize = 65;
            this.btnSaveVisionParam.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveVisionParam.Location = new System.Drawing.Point(484, -1);
            this.btnSaveVisionParam.Name = "btnSaveVisionParam";
            this.btnSaveVisionParam.Size = new System.Drawing.Size(125, 87);
            this.btnSaveVisionParam.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveVisionParam.TabIndex = 2136;
            this.btnSaveVisionParam.Text = "SAVE";
            this.btnSaveVisionParam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveVisionParam.UseVisualStyleBackColor = false;
            this.btnSaveVisionParam.Click += new System.EventHandler(this.btnSaveVisionParam_Click);
            // 
            // FormProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 531);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormProperty";
            this.Text = "Parameter";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoRightEdgePara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoLeftEdgePara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoBlobPara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoContourPara;
        private System.Windows.Forms.PropertyGrid propertyGrid_Parameter;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private RJCodeUI_M1.RJControls.RJButton btnSaveVisionParam;
    }
}