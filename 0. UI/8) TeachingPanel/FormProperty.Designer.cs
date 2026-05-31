using Cyotek.Windows.Forms;

namespace OpenVisionLab
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
            this.rjRadioButton1 = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoVisionPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoBlobPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoContourPara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnParameter = new RJCodeUI_M1.RJControls.RJPanel();
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
            this.splitContainer1.Panel1.Controls.Add(this.rjRadioButton1);
            this.splitContainer1.Panel1.Controls.Add(this.rdoVisionPara);
            this.splitContainer1.Panel1.Controls.Add(this.rdoBlobPara);
            this.splitContainer1.Panel1.Controls.Add(this.rdoContourPara);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(612, 464);
            this.splitContainer1.SplitterDistance = 34;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2148;
            // 
            // rjRadioButton1
            // 
            this.rjRadioButton1.AutoSize = true;
            this.rjRadioButton1.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjRadioButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjRadioButton1.Customizable = true;
            this.rjRadioButton1.Dock = System.Windows.Forms.DockStyle.Left;
            this.rjRadioButton1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjRadioButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjRadioButton1.Location = new System.Drawing.Point(224, 0);
            this.rjRadioButton1.Margin = new System.Windows.Forms.Padding(0);
            this.rjRadioButton1.MinimumSize = new System.Drawing.Size(0, 15);
            this.rjRadioButton1.Name = "rjRadioButton1";
            this.rjRadioButton1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rjRadioButton1.Size = new System.Drawing.Size(84, 34);
            this.rjRadioButton1.TabIndex = 2149;
            this.rjRadioButton1.Text = "Feature";
            this.rjRadioButton1.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rjRadioButton1.UseVisualStyleBackColor = true;
            this.rjRadioButton1.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoVisionPara
            // 
            this.rdoVisionPara.AutoSize = true;
            this.rdoVisionPara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoVisionPara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoVisionPara.Customizable = true;
            this.rdoVisionPara.Dock = System.Windows.Forms.DockStyle.Left;
            this.rdoVisionPara.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoVisionPara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoVisionPara.Location = new System.Drawing.Point(148, 0);
            this.rdoVisionPara.Margin = new System.Windows.Forms.Padding(0);
            this.rdoVisionPara.MinimumSize = new System.Drawing.Size(0, 15);
            this.rdoVisionPara.Name = "rdoVisionPara";
            this.rdoVisionPara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoVisionPara.Size = new System.Drawing.Size(76, 34);
            this.rdoVisionPara.TabIndex = 2148;
            this.rdoVisionPara.Text = "Vision ";
            this.rdoVisionPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoVisionPara.UseVisualStyleBackColor = true;
            this.rdoVisionPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
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
            this.rdoBlobPara.Size = new System.Drawing.Size(62, 34);
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
            this.rdoContourPara.Size = new System.Drawing.Size(86, 34);
            this.rdoContourPara.TabIndex = 2137;
            this.rdoContourPara.Text = "Contour";
            this.rdoContourPara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoContourPara.UseVisualStyleBackColor = true;
            this.rdoContourPara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
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
            this.splitContainer2.Panel1.Controls.Add(this.pnParameter);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.splitContainer2.Panel2.Controls.Add(this.btnSaveVisionParam);
            this.splitContainer2.Size = new System.Drawing.Size(612, 429);
            this.splitContainer2.SplitterDistance = 328;
            this.splitContainer2.TabIndex = 2105;
            // 
            // pnParameter
            // 
            this.pnParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.pnParameter.BorderRadius = 5;
            this.pnParameter.Customizable = false;
            this.pnParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnParameter.Location = new System.Drawing.Point(0, 0);
            this.pnParameter.Name = "pnParameter";
            this.pnParameter.Size = new System.Drawing.Size(612, 328);
            this.pnParameter.TabIndex = 2168;
            // 
            // btnSaveVisionParam
            // 
            this.btnSaveVisionParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveVisionParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnSaveVisionParam.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.btnSaveVisionParam.BorderRadius = 15;
            this.btnSaveVisionParam.BorderSize = 3;
            this.btnSaveVisionParam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveVisionParam.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnSaveVisionParam.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnSaveVisionParam.FlatAppearance.BorderSize = 0;
            this.btnSaveVisionParam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(137)))), ((int)(((byte)(231)))));
            this.btnSaveVisionParam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(128)))), ((int)(((byte)(216)))));
            this.btnSaveVisionParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveVisionParam.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.btnSaveVisionParam.ForeColor = System.Drawing.Color.White;
            this.btnSaveVisionParam.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveVisionParam.IconColor = System.Drawing.Color.White;
            this.btnSaveVisionParam.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveVisionParam.IconSize = 65;
            this.btnSaveVisionParam.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveVisionParam.Location = new System.Drawing.Point(484, 7);
            this.btnSaveVisionParam.Name = "btnSaveVisionParam";
            this.btnSaveVisionParam.Size = new System.Drawing.Size(125, 87);
            this.btnSaveVisionParam.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnSaveVisionParam.TabIndex = 2148;
            this.btnSaveVisionParam.Text = "Save";
            this.btnSaveVisionParam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveVisionParam.UseVisualStyleBackColor = false;
            this.btnSaveVisionParam.Click += new System.EventHandler(this.btnSaveVisionParam_Click);
            // 
            // FormProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(612, 464);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormProperty";
            this.Text = " Property";
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
        private RJCodeUI_M1.RJControls.RJRadioButton rdoBlobPara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoContourPara;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private RJCodeUI_M1.RJControls.RJPanel pnParameter;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoVisionPara;
        private RJCodeUI_M1.RJControls.RJRadioButton rjRadioButton1;
        private RJCodeUI_M1.RJControls.RJButton btnSaveVisionParam;
    }
}