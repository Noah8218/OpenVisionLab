namespace OpenVisionLab
{
	partial class FormImageCompare
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
		/// Required method for Designer support.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageCompare));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.btnLoad = new System.Windows.Forms.Button();
			this.lbGV = new System.Windows.Forms.Label();
			this.lbXY = new System.Windows.Forms.Label();
			this.lbRGB = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlClientArea
			//
			this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
			this.pnlClientArea.Location = new System.Drawing.Point(1, 1);
			this.pnlClientArea.Size = new System.Drawing.Size(958, 759);
			//
			// splitContainer1
			//
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(1, 1);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1MinSize = 0;
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(32)))), ((int)(((byte)(38)))));
			this.splitContainer1.Panel2.Controls.Add(this.btnLoad);
			this.splitContainer1.Panel2.Controls.Add(this.lbGV);
			this.splitContainer1.Panel2.Controls.Add(this.lbXY);
			this.splitContainer1.Panel2.Controls.Add(this.lbRGB);
			this.splitContainer1.Panel2MinSize = 0;
			this.splitContainer1.Size = new System.Drawing.Size(958, 759);
			this.splitContainer1.SplitterDistance = 720;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 0;
			//
			// splitContainer2
			//
			this.splitContainer2.BackColor = System.Drawing.Color.Black;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.BackColor = System.Drawing.Color.Black;
			this.splitContainer2.Panel2.BackColor = System.Drawing.Color.Black;
			this.splitContainer2.Size = new System.Drawing.Size(958, 720);
			this.splitContainer2.SplitterDistance = 477;
			this.splitContainer2.TabIndex = 1948;
			//
			// btnLoad
			//
			this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(94)))), ((int)(((byte)(220)))));
			this.btnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnLoad.FlatAppearance.BorderSize = 0;
			this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnLoad.ForeColor = System.Drawing.Color.White;
			this.btnLoad.Location = new System.Drawing.Point(618, 0);
			this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(340, 38);
			this.btnLoad.TabIndex = 2005;
			this.btnLoad.TabStop = false;
			this.btnLoad.Text = "Load";
			this.btnLoad.UseVisualStyleBackColor = false;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			//
			// lbGV
			//
			this.lbGV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(30)))));
			this.lbGV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbGV.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbGV.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbGV.ForeColor = System.Drawing.Color.White;
			this.lbGV.Location = new System.Drawing.Point(412, 0);
			this.lbGV.Name = "lbGV";
			this.lbGV.Size = new System.Drawing.Size(206, 38);
			this.lbGV.TabIndex = 900;
			this.lbGV.Text = "GV[0]";
			this.lbGV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// lbXY
			//
			this.lbXY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(30)))));
			this.lbXY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbXY.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbXY.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbXY.ForeColor = System.Drawing.Color.White;
			this.lbXY.Location = new System.Drawing.Point(206, 0);
			this.lbXY.Name = "lbXY";
			this.lbXY.Size = new System.Drawing.Size(206, 38);
			this.lbXY.TabIndex = 899;
			this.lbXY.Text = "X,Y[0,0]";
			this.lbXY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// lbRGB
			//
			this.lbRGB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(24)))), ((int)(((byte)(30)))));
			this.lbRGB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbRGB.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbRGB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbRGB.ForeColor = System.Drawing.Color.White;
			this.lbRGB.Location = new System.Drawing.Point(0, 0);
			this.lbRGB.Name = "lbRGB";
			this.lbRGB.Size = new System.Drawing.Size(206, 38);
			this.lbRGB.TabIndex = 896;
			this.lbRGB.Text = "R,G,B[0,0,0]";
			this.lbRGB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// FormImageCompare
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
			this.BorderSize = 1;
			this.Caption = "Image Compare";
			this.ClientSize = new System.Drawing.Size(960, 761);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "FormImageCompare";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
			this.Text = "Image Compare";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LayerDisplay_FormClosed);
			this.Controls.SetChildIndex(this.pnlClientArea, 0);
			this.Controls.SetChildIndex(this.splitContainer1, 0);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Label lbGV;
		private System.Windows.Forms.Label lbXY;
		private System.Windows.Forms.Label lbRGB;
	}
}
