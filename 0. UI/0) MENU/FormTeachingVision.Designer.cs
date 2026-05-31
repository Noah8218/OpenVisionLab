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
            this.components = new System.ComponentModel.Container();
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.TeachingPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timeragin = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbTackTime = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.cbCamera = new RJCodeUI_M1.RJControls.RJComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.imageProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.morphologyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arithmeticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateAndScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.algorithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featureMatchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.chkUseLayerImage = new RJCodeUI_M1.RJControls.RJCheckBox();
            this.btnNewPanel = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.rjButton3 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton2 = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // timerStatus
            // 
            this.timerStatus.Enabled = true;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(1943, 425);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 46);
            this.label2.TabIndex = 1927;
            this.label2.Text = "00.000 mm";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TeachingPanel
            // 
            this.TeachingPanel.Location = new System.Drawing.Point(0, 1);
            this.TeachingPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TeachingPanel.Name = "TeachingPanel";
            this.TeachingPanel.Size = new System.Drawing.Size(1922, 892);
            this.TeachingPanel.TabIndex = 1949;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timeragin
            // 
            this.timeragin.Interval = 300;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // lbTackTime
            // 
            this.lbTackTime.BackColor = System.Drawing.Color.Transparent;
            this.lbTackTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbTackTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbTackTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTackTime.ForeColor = System.Drawing.Color.Black;
            this.lbTackTime.Location = new System.Drawing.Point(857, 896);
            this.lbTackTime.Name = "lbTackTime";
            this.lbTackTime.Size = new System.Drawing.Size(162, 26);
            this.lbTackTime.TabIndex = 2168;
            this.lbTackTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.cbLayerList);
            this.panel1.Controls.Add(this.cbCamera);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Location = new System.Drawing.Point(0, -4);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(707, 926);
            this.panel1.TabIndex = 0;
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 0;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = true;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList.Location = new System.Drawing.Point(385, 900);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 20);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(174, 27);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2176;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // cbCamera
            // 
            this.cbCamera.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbCamera.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbCamera.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbCamera.BorderRadius = 0;
            this.cbCamera.BorderSize = 2;
            this.cbCamera.Customizable = true;
            this.cbCamera.DataSource = null;
            this.cbCamera.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCamera.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbCamera.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbCamera.ForeColor = System.Drawing.Color.DimGray;
            this.cbCamera.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbCamera.Location = new System.Drawing.Point(222, 900);
            this.cbCamera.MinimumSize = new System.Drawing.Size(100, 20);
            this.cbCamera.Name = "cbCamera";
            this.cbCamera.Padding = new System.Windows.Forms.Padding(2);
            this.cbCamera.SelectedIndex = -1;
            this.cbCamera.Size = new System.Drawing.Size(159, 27);
            this.cbCamera.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbCamera.TabIndex = 2177;
            this.cbCamera.Texts = "";
            this.cbCamera.OnSelectedIndexChanged += new System.EventHandler(this.cbCamera_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageProcessingToolStripMenuItem,
            this.algorithmToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 898);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(707, 28);
            this.menuStrip1.TabIndex = 2165;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // imageProcessingToolStripMenuItem
            // 
            this.imageProcessingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.morphologyToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.arithmeticToolStripMenuItem,
            this.edgeDetectionToolStripMenuItem,
            this.rotateAndScaleToolStripMenuItem,
            this.histogramToolStripMenuItem,
            this.hSVToolStripMenuItem});
            this.imageProcessingToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.imageProcessingToolStripMenuItem.Name = "imageProcessingToolStripMenuItem";
            this.imageProcessingToolStripMenuItem.Size = new System.Drawing.Size(134, 24);
            this.imageProcessingToolStripMenuItem.Text = "Image Processing";
            this.imageProcessingToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // morphologyToolStripMenuItem
            // 
            this.morphologyToolStripMenuItem.Name = "morphologyToolStripMenuItem";
            this.morphologyToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.morphologyToolStripMenuItem.Text = "Morphology";
            this.morphologyToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.filterToolStripMenuItem.Text = "Filter";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // arithmeticToolStripMenuItem
            // 
            this.arithmeticToolStripMenuItem.Name = "arithmeticToolStripMenuItem";
            this.arithmeticToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.arithmeticToolStripMenuItem.Text = "Arithmetic";
            this.arithmeticToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // edgeDetectionToolStripMenuItem
            // 
            this.edgeDetectionToolStripMenuItem.Name = "edgeDetectionToolStripMenuItem";
            this.edgeDetectionToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.edgeDetectionToolStripMenuItem.Text = "EdgeDetection";
            this.edgeDetectionToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // rotateAndScaleToolStripMenuItem
            // 
            this.rotateAndScaleToolStripMenuItem.Name = "rotateAndScaleToolStripMenuItem";
            this.rotateAndScaleToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.rotateAndScaleToolStripMenuItem.Text = "RotateAndScale";
            this.rotateAndScaleToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // histogramToolStripMenuItem
            // 
            this.histogramToolStripMenuItem.Name = "histogramToolStripMenuItem";
            this.histogramToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.histogramToolStripMenuItem.Text = "Histogram";
            this.histogramToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // hSVToolStripMenuItem
            // 
            this.hSVToolStripMenuItem.Name = "hSVToolStripMenuItem";
            this.hSVToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.hSVToolStripMenuItem.Text = "HSV";
            this.hSVToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // algorithmToolStripMenuItem
            // 
            this.algorithmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blobToolStripMenuItem,
            this.contourToolStripMenuItem,
            this.matchingToolStripMenuItem,
            this.featureMatchingToolStripMenuItem,
            this.lineToolStripMenuItem,
            this.meanToolStripMenuItem});
            this.algorithmToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.algorithmToolStripMenuItem.Name = "algorithmToolStripMenuItem";
            this.algorithmToolStripMenuItem.Size = new System.Drawing.Size(80, 24);
            this.algorithmToolStripMenuItem.Text = "Algorithm";
            // 
            // blobToolStripMenuItem
            // 
            this.blobToolStripMenuItem.Name = "blobToolStripMenuItem";
            this.blobToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.blobToolStripMenuItem.Text = "Blob";
            this.blobToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // contourToolStripMenuItem
            // 
            this.contourToolStripMenuItem.Name = "contourToolStripMenuItem";
            this.contourToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.contourToolStripMenuItem.Text = "Contour";
            this.contourToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // matchingToolStripMenuItem
            // 
            this.matchingToolStripMenuItem.Name = "matchingToolStripMenuItem";
            this.matchingToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.matchingToolStripMenuItem.Text = "Matching";
            this.matchingToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // featureMatchingToolStripMenuItem
            // 
            this.featureMatchingToolStripMenuItem.Name = "featureMatchingToolStripMenuItem";
            this.featureMatchingToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.featureMatchingToolStripMenuItem.Text = "FeatureMatching";
            this.featureMatchingToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            this.lineToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.lineToolStripMenuItem.Text = "Line";
            this.lineToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // meanToolStripMenuItem
            // 
            this.meanToolStripMenuItem.Name = "meanToolStripMenuItem";
            this.meanToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.meanToolStripMenuItem.Text = "Mean";
            this.meanToolStripMenuItem.Click += new System.EventHandler(this.OnToolStripMenuItem_Click);
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(778, 902);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(73, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 2150;
            this.rjLabel3.Text = "Tack Time";
            // 
            // chkUseLayerImage
            // 
            this.chkUseLayerImage.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUseLayerImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkUseLayerImage.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.chkUseLayerImage.BorderSize = 1;
            this.chkUseLayerImage.Check = true;
            this.chkUseLayerImage.Checked = true;
            this.chkUseLayerImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseLayerImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkUseLayerImage.Customizable = false;
            this.chkUseLayerImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseLayerImage.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseLayerImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.chkUseLayerImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(82)))), ((int)(((byte)(180)))));
            this.chkUseLayerImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUseLayerImage.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseLayerImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.chkUseLayerImage.IconColor = System.Drawing.Color.White;
            this.chkUseLayerImage.Location = new System.Drawing.Point(569, 897);
            this.chkUseLayerImage.MinimumSize = new System.Drawing.Size(0, 21);
            this.chkUseLayerImage.Name = "chkUseLayerImage";
            this.chkUseLayerImage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chkUseLayerImage.Size = new System.Drawing.Size(167, 26);
            this.chkUseLayerImage.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.chkUseLayerImage.TabIndex = 2175;
            this.chkUseLayerImage.Text = "Panel Source Image";
            this.chkUseLayerImage.UseVisualStyleBackColor = false;
            this.chkUseLayerImage.CheckedChanged += new System.EventHandler(this.chkUseLayerImage_CheckedChanged);
            // 
            // btnNewPanel
            // 
            this.btnNewPanel.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel.BackIcon = true;
            this.btnNewPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel.Customizable = true;
            this.btnNewPanel.DropdownMenu = null;
            this.btnNewPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel.IconSize = 30;
            this.btnNewPanel.Location = new System.Drawing.Point(742, 897);
            this.btnNewPanel.Name = "btnNewPanel";
            this.btnNewPanel.Size = new System.Drawing.Size(30, 30);
            this.btnNewPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnNewPanel.TabIndex = 2174;
            this.btnNewPanel.TabStop = false;
            this.btnNewPanel.Click += new System.EventHandler(this.btnNewPanel_Click);
            // 
            // rjButton3
            // 
            this.rjButton3.BackColor = System.Drawing.Color.White;
            this.rjButton3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton3.BorderRadius = 15;
            this.rjButton3.BorderSize = 3;
            this.rjButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton3.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton3.FlatAppearance.BorderSize = 3;
            this.rjButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton3.IconChar = FontAwesome.Sharp.IconChar.Youtube;
            this.rjButton3.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton3.IconSize = 80;
            this.rjButton3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton3.Location = new System.Drawing.Point(208, 121);
            this.rjButton3.Name = "rjButton3";
            this.rjButton3.Size = new System.Drawing.Size(150, 112);
            this.rjButton3.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton3.TabIndex = 2138;
            this.rjButton3.Text = "LIVE";
            this.rjButton3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton3.UseVisualStyleBackColor = false;
            // 
            // rjButton2
            // 
            this.rjButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.rjButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton2.BorderRadius = 15;
            this.rjButton2.BorderSize = 3;
            this.rjButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton2.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton2.FlatAppearance.BorderSize = 3;
            this.rjButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton2.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.rjButton2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton2.IconSize = 24;
            this.rjButton2.Location = new System.Drawing.Point(396, 234);
            this.rjButton2.Name = "rjButton2";
            this.rjButton2.Size = new System.Drawing.Size(117, 96);
            this.rjButton2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton2.TabIndex = 2139;
            this.rjButton2.Text = "EXCUTE";
            this.rjButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rjButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rjButton2.UseVisualStyleBackColor = false;
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.White;
            this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.BorderRadius = 15;
            this.rjButton1.BorderSize = 3;
            this.rjButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton1.FlatAppearance.BorderSize = 3;
            this.rjButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconChar = FontAwesome.Sharp.IconChar.Camera;
            this.rjButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rjButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton1.IconSize = 80;
            this.rjButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rjButton1.Location = new System.Drawing.Point(52, 121);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(150, 112);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 2137;
            this.rjButton1.Text = "GRAB";
            this.rjButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rjButton1.UseVisualStyleBackColor = false;
            // 
            // FormTeachingVision
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1924, 923);
            this.ControlBox = false;
            this.Controls.Add(this.lbTackTime);
            this.Controls.Add(this.rjLabel3);
            this.Controls.Add(this.btnNewPanel);
            this.Controls.Add(this.chkUseLayerImage);
            this.Controls.Add(this.TeachingPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Movable = false;
            this.Name = "FormTeachingVision";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.None;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.White;
            this.TransparencyKey = System.Drawing.Color.Empty;
            this.Load += new System.EventHandler(this.FormTeachingVision_Load);
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Panel TeachingPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timeragin;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJButton rjButton3;
        private RJCodeUI_M1.RJControls.RJButton rjButton2;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
        private System.Windows.Forms.Label lbTackTime;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem imageProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem morphologyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arithmeticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edgeDetectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateAndScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem algorithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blobToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineToolStripMenuItem;
        private RJCodeUI_M1.RJControls.RJCheckBox chkUseLayerImage;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel;
        private RJCodeUI_M1.RJControls.RJComboBox cbCamera;
        private System.Windows.Forms.ToolStripMenuItem histogramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem featureMatchingToolStripMenuItem;
    }
}
