namespace OpenVisionLab
{
    partial class FormLayerDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLayerDisplay));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbZOOM = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbGV = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbXY = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbRGB = new RJCodeUI_M1.RJControls.RJLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
            this.timePixelData.Tick += new System.EventHandler(this.timePixelData_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1255, 573);
            this.panel1.TabIndex = 1952;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1255, 554);
            this.panel3.TabIndex = 1954;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbZOOM);
            this.panel2.Controls.Add(this.lbGV);
            this.panel2.Controls.Add(this.lbXY);
            this.panel2.Controls.Add(this.lbRGB);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 554);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1255, 19);
            this.panel2.TabIndex = 1953;
            // 
            // lbZOOM
            // 
            this.lbZOOM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbZOOM.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbZOOM.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbZOOM.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lbZOOM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbZOOM.LinkLabel = false;
            this.lbZOOM.Location = new System.Drawing.Point(504, 0);
            this.lbZOOM.Name = "lbZOOM";
            this.lbZOOM.Size = new System.Drawing.Size(168, 19);
            this.lbZOOM.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lbZOOM.TabIndex = 1954;
            this.lbZOOM.Text = "ZOOM[0]";
            // 
            // lbGV
            // 
            this.lbGV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbGV.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbGV.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lbGV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbGV.LinkLabel = false;
            this.lbGV.Location = new System.Drawing.Point(336, 0);
            this.lbGV.Name = "lbGV";
            this.lbGV.Size = new System.Drawing.Size(168, 19);
            this.lbGV.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lbGV.TabIndex = 1953;
            this.lbGV.Text = "GV[0]";
            // 
            // lbXY
            // 
            this.lbXY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbXY.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbXY.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbXY.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lbXY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbXY.LinkLabel = false;
            this.lbXY.Location = new System.Drawing.Point(168, 0);
            this.lbXY.Name = "lbXY";
            this.lbXY.Size = new System.Drawing.Size(168, 19);
            this.lbXY.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lbXY.TabIndex = 1949;
            this.lbXY.Text = "X,Y[0,0]";
            // 
            // lbRGB
            // 
            this.lbRGB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRGB.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbRGB.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbRGB.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lbRGB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbRGB.LinkLabel = false;
            this.lbRGB.Location = new System.Drawing.Point(0, 0);
            this.lbRGB.Name = "lbRGB";
            this.lbRGB.Size = new System.Drawing.Size(168, 19);
            this.lbRGB.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lbRGB.TabIndex = 1948;
            this.lbRGB.Text = "R,G,B[0,0,0]";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormLayerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1255, 573);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormLayerDisplay";
            this.Text = "LayerDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LayerDisplay_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.FormLayerDisplay_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private RJCodeUI_M1.RJControls.RJLabel lbRGB;
        private RJCodeUI_M1.RJControls.RJLabel lbXY;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private RJCodeUI_M1.RJControls.RJLabel lbZOOM;
        private RJCodeUI_M1.RJControls.RJLabel lbGV;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer timer1;
    }
}