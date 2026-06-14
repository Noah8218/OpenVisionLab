namespace OpenVisionLab
{
    partial class FormTeachingVision
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
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			TeachingPanel = new System.Windows.Forms.Panel();
			panel1 = new System.Windows.Forms.Panel();
			lbTackTime = new System.Windows.Forms.Label();
			chkUseLayerImage = new RJCodeUI_M1.RJControls.RJCheckBox();
			rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
			btnNewPanel = new RJCodeUI_M1.RJControls.RJMenuIcon();
			cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
			cbCamera = new RJCodeUI_M1.RJControls.RJComboBox();
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			imageProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			morphologyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			arithmeticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			edgeDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			rotateAndScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			histogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			hSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			algorithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			blobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			contourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			matchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			featureMatchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			meanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pipelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			timer1 = new System.Windows.Forms.Timer(components);
			toolTip1 = new System.Windows.Forms.ToolTip(components);
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)btnNewPanel).BeginInit();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			splitContainer1.IsSplitterFixed = true;
			splitContainer1.Location = new System.Drawing.Point(0, 0);
			splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(panel1);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(TeachingPanel);
			splitContainer1.Panel1MinSize = 34;
			splitContainer1.Size = new System.Drawing.Size(1924, 1000);
			splitContainer1.SplitterDistance = 34;
			splitContainer1.SplitterWidth = 1;
			splitContainer1.TabIndex = 2178;
			// 
			// TeachingPanel
			// 
			TeachingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			TeachingPanel.Location = new System.Drawing.Point(0, 0);
			TeachingPanel.Margin = new System.Windows.Forms.Padding(0);
			TeachingPanel.Name = "TeachingPanel";
			TeachingPanel.Size = new System.Drawing.Size(1924, 965);
			TeachingPanel.TabIndex = 1949;
			// 
			// panel1
			// 
			panel1.BackColor = System.Drawing.Color.FromArgb(68, 79, 116);
			panel1.Controls.Add(lbTackTime);
			panel1.Controls.Add(chkUseLayerImage);
			panel1.Controls.Add(rjLabel3);
			panel1.Controls.Add(btnNewPanel);
			panel1.Controls.Add(cbLayerList);
			panel1.Controls.Add(cbCamera);
			panel1.Controls.Add(menuStrip1);
			panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			panel1.Location = new System.Drawing.Point(0, 0);
			panel1.Margin = new System.Windows.Forms.Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(1924, 34);
			panel1.TabIndex = 0;
			// 
			// lbTackTime
			// 
			lbTackTime.BackColor = System.Drawing.Color.FromArgb(53, 62, 92);
			lbTackTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
			lbTackTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			lbTackTime.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			lbTackTime.ForeColor = System.Drawing.Color.WhiteSmoke;
			lbTackTime.Location = new System.Drawing.Point(885, 3);
			lbTackTime.Name = "lbTackTime";
			lbTackTime.Size = new System.Drawing.Size(162, 28);
			lbTackTime.TabIndex = 2168;
			lbTackTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// chkUseLayerImage
			// 
			chkUseLayerImage.Appearance = System.Windows.Forms.Appearance.Button;
			chkUseLayerImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			chkUseLayerImage.BorderColor = System.Drawing.Color.FromArgb(120, 132, 190);
			chkUseLayerImage.BorderSize = 1;
			chkUseLayerImage.Check = true;
			chkUseLayerImage.Checked = true;
			chkUseLayerImage.CheckState = System.Windows.Forms.CheckState.Checked;
			chkUseLayerImage.Cursor = System.Windows.Forms.Cursors.Hand;
			chkUseLayerImage.Customizable = true;
			chkUseLayerImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(120, 132, 190);
			chkUseLayerImage.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(83, 97, 212);
			chkUseLayerImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(108, 120, 218);
			chkUseLayerImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(70, 82, 180);
			chkUseLayerImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			chkUseLayerImage.Font = new System.Drawing.Font("Verdana", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			chkUseLayerImage.ForeColor = System.Drawing.Color.White;
			chkUseLayerImage.IconColor = System.Drawing.Color.White;
			chkUseLayerImage.Location = new System.Drawing.Point(597, 3);
			chkUseLayerImage.MinimumSize = new System.Drawing.Size(0, 21);
			chkUseLayerImage.Name = "chkUseLayerImage";
			chkUseLayerImage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			chkUseLayerImage.Size = new System.Drawing.Size(167, 28);
			chkUseLayerImage.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
			chkUseLayerImage.TabIndex = 2175;
			chkUseLayerImage.Text = "Panel Source Image";
			chkUseLayerImage.UseVisualStyleBackColor = false;
			chkUseLayerImage.CheckedChanged += chkUseLayerImage_CheckedChanged;
			// 
			// rjLabel3
			// 
			rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
			rjLabel3.ForeColor = System.Drawing.Color.White;
			rjLabel3.LinkLabel = false;
			rjLabel3.Location = new System.Drawing.Point(806, 3);
			rjLabel3.Name = "rjLabel3";
			rjLabel3.Size = new System.Drawing.Size(73, 28);
			rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel3.TabIndex = 2150;
			rjLabel3.Text = "Tack Time";
			rjLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnNewPanel
			// 
			btnNewPanel.BackColor = System.Drawing.Color.Transparent;
			btnNewPanel.BackIcon = true;
			btnNewPanel.Cursor = System.Windows.Forms.Cursors.Hand;
			btnNewPanel.Customizable = true;
			btnNewPanel.DropdownMenu = null;
			btnNewPanel.ForeColor = System.Drawing.Color.WhiteSmoke;
			btnNewPanel.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
			btnNewPanel.IconColor = System.Drawing.Color.WhiteSmoke;
			btnNewPanel.IconFont = FontAwesome.Sharp.IconFont.Auto;
			btnNewPanel.IconSize = 30;
			btnNewPanel.Location = new System.Drawing.Point(770, 3);
			btnNewPanel.Name = "btnNewPanel";
			btnNewPanel.Size = new System.Drawing.Size(30, 30);
			btnNewPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			btnNewPanel.TabIndex = 2174;
			btnNewPanel.TabStop = false;
			btnNewPanel.Click += btnNewPanel_Click;
			// 
			// cbLayerList
			// 
			cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbLayerList.BackColor = System.Drawing.Color.FromArgb(53, 62, 92);
			cbLayerList.BorderColor = System.Drawing.Color.FromArgb(120, 132, 190);
			cbLayerList.BorderRadius = 0;
			cbLayerList.BorderSize = 2;
			cbLayerList.Customizable = true;
			cbLayerList.DataSource = null;
			cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(53, 62, 92);
			cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbLayerList.DropDownTextColor = System.Drawing.Color.WhiteSmoke;
			cbLayerList.Font = new System.Drawing.Font("Verdana", 12F);
			cbLayerList.ForeColor = System.Drawing.Color.WhiteSmoke;
			cbLayerList.IconColor = System.Drawing.Color.WhiteSmoke;
			cbLayerList.Location = new System.Drawing.Point(413, 3);
			cbLayerList.MinimumSize = new System.Drawing.Size(100, 20);
			cbLayerList.Name = "cbLayerList";
			cbLayerList.Padding = new System.Windows.Forms.Padding(2);
			cbLayerList.SelectedIndex = -1;
			cbLayerList.Size = new System.Drawing.Size(174, 28);
			cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbLayerList.TabIndex = 2176;
			cbLayerList.Texts = "";
			cbLayerList.OnSelectedIndexChanged += cbLayerList_SelectedIndexChanged;
			// 
			// cbCamera
			// 
			cbCamera.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbCamera.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbCamera.BackColor = System.Drawing.Color.FromArgb(53, 62, 92);
			cbCamera.BorderColor = System.Drawing.Color.FromArgb(120, 132, 190);
			cbCamera.BorderRadius = 0;
			cbCamera.BorderSize = 2;
			cbCamera.Customizable = true;
			cbCamera.DataSource = null;
			cbCamera.DropDownBackColor = System.Drawing.Color.FromArgb(53, 62, 92);
			cbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbCamera.DropDownTextColor = System.Drawing.Color.WhiteSmoke;
			cbCamera.Font = new System.Drawing.Font("Verdana", 12F);
			cbCamera.ForeColor = System.Drawing.Color.WhiteSmoke;
			cbCamera.IconColor = System.Drawing.Color.WhiteSmoke;
			cbCamera.Location = new System.Drawing.Point(250, 3);
			cbCamera.MinimumSize = new System.Drawing.Size(100, 20);
			cbCamera.Name = "cbCamera";
			cbCamera.Padding = new System.Windows.Forms.Padding(2);
			cbCamera.SelectedIndex = -1;
			cbCamera.Size = new System.Drawing.Size(159, 28);
			cbCamera.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbCamera.TabIndex = 2177;
			cbCamera.Texts = "";
			cbCamera.OnSelectedIndexChanged += cbCamera_SelectedIndexChanged;
			// 
			// menuStrip1
			// 
			menuStrip1.AutoSize = false;
			menuStrip1.BackColor = System.Drawing.Color.FromArgb(68, 79, 116);
			menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
			menuStrip1.ForeColor = System.Drawing.Color.WhiteSmoke;
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { imageProcessingToolStripMenuItem, algorithmToolStripMenuItem });
			menuStrip1.Location = new System.Drawing.Point(0, 3);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size(242, 28);
			menuStrip1.TabIndex = 2165;
			menuStrip1.Text = "menuStrip1";
			// 
			// imageProcessingToolStripMenuItem
			// 
			imageProcessingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { morphologyToolStripMenuItem, filterToolStripMenuItem, arithmeticToolStripMenuItem, edgeDetectionToolStripMenuItem, rotateAndScaleToolStripMenuItem, histogramToolStripMenuItem, hSVToolStripMenuItem });
			imageProcessingToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(68, 79, 116);
			imageProcessingToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9.5F);
			imageProcessingToolStripMenuItem.ForeColor = System.Drawing.Color.WhiteSmoke;
			imageProcessingToolStripMenuItem.Name = "imageProcessingToolStripMenuItem";
			imageProcessingToolStripMenuItem.Size = new System.Drawing.Size(134, 24);
			imageProcessingToolStripMenuItem.Text = "Image Processing";
			imageProcessingToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// morphologyToolStripMenuItem
			// 
			morphologyToolStripMenuItem.Name = "morphologyToolStripMenuItem";
			morphologyToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			morphologyToolStripMenuItem.Text = "Morphology";
			morphologyToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// filterToolStripMenuItem
			// 
			filterToolStripMenuItem.Name = "filterToolStripMenuItem";
			filterToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			filterToolStripMenuItem.Text = "Filter";
			filterToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// arithmeticToolStripMenuItem
			// 
			arithmeticToolStripMenuItem.Name = "arithmeticToolStripMenuItem";
			arithmeticToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			arithmeticToolStripMenuItem.Text = "Arithmetic";
			arithmeticToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// edgeDetectionToolStripMenuItem
			// 
			edgeDetectionToolStripMenuItem.Name = "edgeDetectionToolStripMenuItem";
			edgeDetectionToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			edgeDetectionToolStripMenuItem.Text = "EdgeDetection";
			edgeDetectionToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// rotateAndScaleToolStripMenuItem
			// 
			rotateAndScaleToolStripMenuItem.Name = "rotateAndScaleToolStripMenuItem";
			rotateAndScaleToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			rotateAndScaleToolStripMenuItem.Text = "RotateAndScale";
			rotateAndScaleToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// histogramToolStripMenuItem
			// 
			histogramToolStripMenuItem.Name = "histogramToolStripMenuItem";
			histogramToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			histogramToolStripMenuItem.Text = "Histogram";
			histogramToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// hSVToolStripMenuItem
			// 
			hSVToolStripMenuItem.Name = "hSVToolStripMenuItem";
			hSVToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			hSVToolStripMenuItem.Text = "HSV";
			hSVToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// algorithmToolStripMenuItem
			// 
			algorithmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { blobToolStripMenuItem, contourToolStripMenuItem, matchingToolStripMenuItem, featureMatchingToolStripMenuItem, lineToolStripMenuItem, meanToolStripMenuItem, pipelineToolStripMenuItem });
			algorithmToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(68, 79, 116);
			algorithmToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9.5F);
			algorithmToolStripMenuItem.ForeColor = System.Drawing.Color.WhiteSmoke;
			algorithmToolStripMenuItem.Name = "algorithmToolStripMenuItem";
			algorithmToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
			algorithmToolStripMenuItem.Text = "Algorithm";
			// 
			// blobToolStripMenuItem
			// 
			blobToolStripMenuItem.Name = "blobToolStripMenuItem";
			blobToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			blobToolStripMenuItem.Text = "Blob";
			blobToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// contourToolStripMenuItem
			// 
			contourToolStripMenuItem.Name = "contourToolStripMenuItem";
			contourToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			contourToolStripMenuItem.Text = "Contour";
			contourToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// matchingToolStripMenuItem
			// 
			matchingToolStripMenuItem.Name = "matchingToolStripMenuItem";
			matchingToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			matchingToolStripMenuItem.Text = "Matching";
			matchingToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// featureMatchingToolStripMenuItem
			// 
			featureMatchingToolStripMenuItem.Name = "featureMatchingToolStripMenuItem";
			featureMatchingToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			featureMatchingToolStripMenuItem.Text = "FeatureMatching";
			featureMatchingToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// lineToolStripMenuItem
			// 
			lineToolStripMenuItem.Name = "lineToolStripMenuItem";
			lineToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			lineToolStripMenuItem.Text = "Line";
			lineToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// meanToolStripMenuItem
			// 
			meanToolStripMenuItem.Name = "meanToolStripMenuItem";
			meanToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			meanToolStripMenuItem.Text = "Mean";
			meanToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// pipelineToolStripMenuItem
			// 
			pipelineToolStripMenuItem.Name = "pipelineToolStripMenuItem";
			pipelineToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			pipelineToolStripMenuItem.Text = "Pipeline";
			pipelineToolStripMenuItem.Click += OnToolStripMenuItem_Click;
			// 
			// timer1
			// 
			timer1.Enabled = true;
			timer1.Tick += timer1_Tick;
			// 
			// toolTip1
			// 
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 100;
			toolTip1.IsBalloon = true;
			toolTip1.ReshowDelay = 100;
			// 
			// FormTeachingVision
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			ClientSize = new System.Drawing.Size(1924, 1000);
			ControlBox = false;
			Controls.Add(splitContainer1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "FormTeachingVision";
			ShowInTaskbar = false;
			Load += FormTeachingVision_Load;
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)btnNewPanel).EndInit();
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.Panel TeachingPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private System.Windows.Forms.Label lbTackTime;
        private RJCodeUI_M1.RJControls.RJCheckBox chkUseLayerImage;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem imageProcessingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem morphologyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem arithmeticToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem edgeDetectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateAndScaleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem histogramToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hSVToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem algorithmToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blobToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contourToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem matchingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem featureMatchingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem meanToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pipelineToolStripMenuItem;
		private RJCodeUI_M1.RJControls.RJComboBox cbCamera;
		private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
		public System.Windows.Forms.Panel panel1;
	}
}
