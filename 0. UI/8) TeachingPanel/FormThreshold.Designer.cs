namespace OpenVisionLab
{
    partial class FormThreshold
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormThreshold));
			timePixelData = new System.Windows.Forms.Timer(components);
			panel1 = new System.Windows.Forms.Panel();
			pnlPreviewSummary = new System.Windows.Forms.Panel();
			lblPreviewDescription = new System.Windows.Forms.Label();
			lblPreviewTitle = new System.Windows.Forms.Label();
			btnAddToPipeline = new System.Windows.Forms.Button();
			rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
			tblAdaptiveParams = new System.Windows.Forms.TableLayoutPanel();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			tbBlockSize = new RJCodeUI_M1.RJControls.RJTextBox();
			tbWeight = new RJCodeUI_M1.RJControls.RJTextBox();
			adaptiveValueBar = new ThresholdValueTrackBar();
			rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
			pnlAdaptiveOptions = new System.Windows.Forms.Panel();
			tblAdaptiveOptions = new System.Windows.Forms.TableLayoutPanel();
			rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
			rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
			cbAdaptiveThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
			cbAdaptiveType = new RJCodeUI_M1.RJControls.RJComboBox();
			pnlAdaptiveHeader = new System.Windows.Forms.Panel();
			lblAdaptiveHint = new System.Windows.Forms.Label();
			lblAdaptiveTitle = new System.Windows.Forms.Label();
			lblAdaptiveNo = new System.Windows.Forms.Label();
			rangeThresholdBar = new ThresholdRangeTrackBar();
			pnlRangeHeader = new System.Windows.Forms.Panel();
			lblRangeHint = new System.Windows.Forms.Label();
			lblRangeTitle = new System.Windows.Forms.Label();
			lblRangeNo = new System.Windows.Forms.Label();
			thresholdValueBar = new ThresholdValueTrackBar();
			cbThresholdMenu = new RJCodeUI_M1.RJControls.RJComboBox();
			rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
			pnlBasicHeader = new System.Windows.Forms.Panel();
			lblBasicHint = new System.Windows.Forms.Label();
			lblBasicTitle = new System.Windows.Forms.Label();
			lblBasicNo = new System.Windows.Forms.Label();
			pnlLayerFlow = new System.Windows.Forms.Panel();
			tblLayerFlow = new System.Windows.Forms.TableLayoutPanel();
			lblInputLayer = new System.Windows.Forms.Label();
			lblOutputLayer = new System.Windows.Forms.Label();
			cbInputLayer = new RJCodeUI_M1.RJControls.RJComboBox();
			lblOutputLayerValue = new System.Windows.Forms.Label();
			panel1.SuspendLayout();
			pnlPreviewSummary.SuspendLayout();
			rjPanel1.SuspendLayout();
			tblAdaptiveParams.SuspendLayout();
			pnlAdaptiveOptions.SuspendLayout();
			tblAdaptiveOptions.SuspendLayout();
			pnlAdaptiveHeader.SuspendLayout();
			pnlRangeHeader.SuspendLayout();
			pnlBasicHeader.SuspendLayout();
			pnlLayerFlow.SuspendLayout();
			tblLayerFlow.SuspendLayout();
			SuspendLayout();
			// 
			// timePixelData
			// 
			timePixelData.Enabled = true;
			timePixelData.Interval = 10;
			// 
			// panel1
			// 
			panel1.AutoScroll = true;
			panel1.BackColor = System.Drawing.Color.FromArgb(232, 238, 245);
			panel1.Controls.Add(pnlPreviewSummary);
			panel1.Controls.Add(btnAddToPipeline);
			panel1.Controls.Add(rjPanel1);
			panel1.Controls.Add(adaptiveValueBar);
			panel1.Controls.Add(rjLabel2);
			panel1.Controls.Add(pnlAdaptiveOptions);
			panel1.Controls.Add(pnlAdaptiveHeader);
			panel1.Controls.Add(rangeThresholdBar);
			panel1.Controls.Add(pnlRangeHeader);
			panel1.Controls.Add(thresholdValueBar);
			panel1.Controls.Add(cbThresholdMenu);
			panel1.Controls.Add(rjLabel4);
			panel1.Controls.Add(pnlBasicHeader);
			panel1.Controls.Add(pnlLayerFlow);
			panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			panel1.Location = new System.Drawing.Point(0, 0);
			panel1.Name = "panel1";
			panel1.Padding = new System.Windows.Forms.Padding(12, 10, 12, 12);
			panel1.Size = new System.Drawing.Size(640, 690);
			panel1.TabIndex = 1952;
			// 
			// pnlPreviewSummary
			// 
			pnlPreviewSummary.BackColor = System.Drawing.Color.FromArgb(28, 40, 58);
			pnlPreviewSummary.Controls.Add(lblPreviewDescription);
			pnlPreviewSummary.Controls.Add(lblPreviewTitle);
			pnlPreviewSummary.Dock = System.Windows.Forms.DockStyle.Top;
			pnlPreviewSummary.Location = new System.Drawing.Point(12, 580);
			pnlPreviewSummary.Name = "pnlPreviewSummary";
			pnlPreviewSummary.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
			pnlPreviewSummary.Size = new System.Drawing.Size(616, 52);
			pnlPreviewSummary.TabIndex = 2214;
			// 
			// lblPreviewDescription
			// 
			lblPreviewDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			lblPreviewDescription.Font = new System.Drawing.Font("Segoe UI", 8.5F);
			lblPreviewDescription.ForeColor = System.Drawing.Color.FromArgb(210, 222, 238);
			lblPreviewDescription.Location = new System.Drawing.Point(12, 26);
			lblPreviewDescription.Name = "lblPreviewDescription";
			lblPreviewDescription.Size = new System.Drawing.Size(592, 20);
			lblPreviewDescription.TabIndex = 1;
			lblPreviewDescription.Text = "Preview result writes to Threshold layer.";
			lblPreviewDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblPreviewTitle
			// 
			lblPreviewTitle.Dock = System.Windows.Forms.DockStyle.Top;
			lblPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			lblPreviewTitle.ForeColor = System.Drawing.Color.White;
			lblPreviewTitle.Location = new System.Drawing.Point(12, 6);
			lblPreviewTitle.Name = "lblPreviewTitle";
			lblPreviewTitle.Size = new System.Drawing.Size(592, 20);
			lblPreviewTitle.TabIndex = 0;
			lblPreviewTitle.Text = "Preview";
			lblPreviewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnAddToPipeline
			// 
			btnAddToPipeline.BackColor = System.Drawing.Color.FromArgb(39, 111, 156);
			btnAddToPipeline.Cursor = System.Windows.Forms.Cursors.Hand;
			btnAddToPipeline.Dock = System.Windows.Forms.DockStyle.Top;
			btnAddToPipeline.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(84, 151, 184);
			btnAddToPipeline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnAddToPipeline.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			btnAddToPipeline.ForeColor = System.Drawing.Color.White;
			btnAddToPipeline.Location = new System.Drawing.Point(12, 546);
			btnAddToPipeline.Name = "btnAddToPipeline";
			btnAddToPipeline.Size = new System.Drawing.Size(616, 34);
			btnAddToPipeline.TabIndex = 2213;
			btnAddToPipeline.Text = "+ Add Pipeline Step";
			btnAddToPipeline.UseVisualStyleBackColor = false;
			// 
			// rjPanel1
			// 
			rjPanel1.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			rjPanel1.BorderRadius = 0;
			rjPanel1.Controls.Add(tblAdaptiveParams);
			rjPanel1.Customizable = false;
			rjPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			rjPanel1.Location = new System.Drawing.Point(12, 482);
			rjPanel1.Name = "rjPanel1";
			rjPanel1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 6);
			rjPanel1.Size = new System.Drawing.Size(616, 64);
			rjPanel1.TabIndex = 2195;
			// 
			// tblAdaptiveParams
			// 
			tblAdaptiveParams.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			tblAdaptiveParams.ColumnCount = 4;
			tblAdaptiveParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
			tblAdaptiveParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblAdaptiveParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
			tblAdaptiveParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblAdaptiveParams.Controls.Add(label4, 0, 0);
			tblAdaptiveParams.Controls.Add(label5, 2, 0);
			tblAdaptiveParams.Controls.Add(tbBlockSize, 1, 0);
			tblAdaptiveParams.Controls.Add(tbWeight, 3, 0);
			tblAdaptiveParams.Dock = System.Windows.Forms.DockStyle.Fill;
			tblAdaptiveParams.Location = new System.Drawing.Point(0, 4);
			tblAdaptiveParams.Name = "tblAdaptiveParams";
			tblAdaptiveParams.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
			tblAdaptiveParams.RowCount = 1;
			tblAdaptiveParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tblAdaptiveParams.Size = new System.Drawing.Size(616, 54);
			tblAdaptiveParams.TabIndex = 0;
			// 
			// label4
			// 
			label4.Dock = System.Windows.Forms.DockStyle.Fill;
			label4.Font = new System.Drawing.Font("Segoe UI", 8.6F, System.Drawing.FontStyle.Bold);
			label4.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			label4.Location = new System.Drawing.Point(13, 6);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(78, 42);
			label4.TabIndex = 2136;
			label4.Text = "Block size";
			label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			label5.Dock = System.Windows.Forms.DockStyle.Fill;
			label5.Font = new System.Drawing.Font("Segoe UI", 8.6F, System.Drawing.FontStyle.Bold);
			label5.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			label5.Location = new System.Drawing.Point(317, 6);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(66, 42);
			label5.TabIndex = 2137;
			label5.Text = "Weight";
			label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbBlockSize
			// 
			tbBlockSize._Customizable = true;
			tbBlockSize.BackColor = System.Drawing.Color.FromArgb(252, 254, 255);
			tbBlockSize.BorderColor = System.Drawing.Color.FromArgb(154, 178, 204);
			tbBlockSize.BorderFocusColor = System.Drawing.Color.FromArgb(67, 154, 188);
			tbBlockSize.BorderRadius = 0;
			tbBlockSize.BorderSize = 1;
			tbBlockSize.Dock = System.Windows.Forms.DockStyle.Fill;
			tbBlockSize.Font = new System.Drawing.Font("Segoe UI", 9F);
			tbBlockSize.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			tbBlockSize.Location = new System.Drawing.Point(97, 9);
			tbBlockSize.MultiLine = false;
			tbBlockSize.Name = "tbBlockSize";
			tbBlockSize.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
			tbBlockSize.PasswordChar = false;
			tbBlockSize.PlaceHolderColor = System.Drawing.Color.DarkGray;
			tbBlockSize.PlaceHolderText = "25";
			tbBlockSize.ScrollBars = System.Windows.Forms.ScrollBars.None;
			tbBlockSize.Size = new System.Drawing.Size(214, 30);
			tbBlockSize.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
			tbBlockSize.TabIndex = 2140;
			// 
			// tbWeight
			// 
			tbWeight._Customizable = true;
			tbWeight.BackColor = System.Drawing.Color.FromArgb(252, 254, 255);
			tbWeight.BorderColor = System.Drawing.Color.FromArgb(154, 178, 204);
			tbWeight.BorderFocusColor = System.Drawing.Color.FromArgb(67, 154, 188);
			tbWeight.BorderRadius = 0;
			tbWeight.BorderSize = 1;
			tbWeight.Dock = System.Windows.Forms.DockStyle.Fill;
			tbWeight.Font = new System.Drawing.Font("Segoe UI", 9F);
			tbWeight.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			tbWeight.Location = new System.Drawing.Point(389, 9);
			tbWeight.MultiLine = false;
			tbWeight.Name = "tbWeight";
			tbWeight.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
			tbWeight.PasswordChar = false;
			tbWeight.PlaceHolderColor = System.Drawing.Color.DarkGray;
			tbWeight.PlaceHolderText = "5";
			tbWeight.ScrollBars = System.Windows.Forms.ScrollBars.None;
			tbWeight.Size = new System.Drawing.Size(214, 30);
			tbWeight.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
			tbWeight.TabIndex = 2141;
			// 
			// adaptiveValueBar
			// 
			adaptiveValueBar.BackColor = System.Drawing.Color.FromArgb(24, 33, 47);
			adaptiveValueBar.Caption = "Max:";
			adaptiveValueBar.Dock = System.Windows.Forms.DockStyle.Top;
			adaptiveValueBar.ForeColor = System.Drawing.Color.White;
			adaptiveValueBar.Location = new System.Drawing.Point(12, 434);
			adaptiveValueBar.Maximum = 255;
			adaptiveValueBar.Minimum = 0;
			adaptiveValueBar.MinimumSize = new System.Drawing.Size(360, 44);
			adaptiveValueBar.Name = "adaptiveValueBar";
			adaptiveValueBar.Size = new System.Drawing.Size(616, 48);
			adaptiveValueBar.TabIndex = 2212;
			adaptiveValueBar.Value = 1;
			// 
			// rjLabel2
			// 
			rjLabel2.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			rjLabel2.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			rjLabel2.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			rjLabel2.LinkLabel = false;
			rjLabel2.Location = new System.Drawing.Point(12, 410);
			rjLabel2.Name = "rjLabel2";
			rjLabel2.Padding = new System.Windows.Forms.Padding(10, 4, 8, 0);
			rjLabel2.Size = new System.Drawing.Size(616, 24);
			rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel2.TabIndex = 2193;
			rjLabel2.Text = "Max value";
			rjLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pnlAdaptiveOptions
			// 
			pnlAdaptiveOptions.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			pnlAdaptiveOptions.Controls.Add(tblAdaptiveOptions);
			pnlAdaptiveOptions.Dock = System.Windows.Forms.DockStyle.Top;
			pnlAdaptiveOptions.Location = new System.Drawing.Point(12, 338);
			pnlAdaptiveOptions.Name = "pnlAdaptiveOptions";
			pnlAdaptiveOptions.Padding = new System.Windows.Forms.Padding(10, 6, 10, 8);
			pnlAdaptiveOptions.Size = new System.Drawing.Size(616, 72);
			pnlAdaptiveOptions.TabIndex = 2211;
			// 
			// tblAdaptiveOptions
			// 
			tblAdaptiveOptions.ColumnCount = 2;
			tblAdaptiveOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblAdaptiveOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblAdaptiveOptions.Controls.Add(rjLabel3, 0, 0);
			tblAdaptiveOptions.Controls.Add(rjLabel6, 1, 0);
			tblAdaptiveOptions.Controls.Add(cbAdaptiveThresholdMenu, 0, 1);
			tblAdaptiveOptions.Controls.Add(cbAdaptiveType, 1, 1);
			tblAdaptiveOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			tblAdaptiveOptions.Location = new System.Drawing.Point(10, 6);
			tblAdaptiveOptions.Name = "tblAdaptiveOptions";
			tblAdaptiveOptions.RowCount = 2;
			tblAdaptiveOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			tblAdaptiveOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tblAdaptiveOptions.Size = new System.Drawing.Size(596, 58);
			tblAdaptiveOptions.TabIndex = 0;
			// 
			// rjLabel3
			// 
			rjLabel3.BackColor = System.Drawing.Color.Transparent;
			rjLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
			rjLabel3.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			rjLabel3.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			rjLabel3.LinkLabel = false;
			rjLabel3.Location = new System.Drawing.Point(3, 0);
			rjLabel3.Name = "rjLabel3";
			rjLabel3.Size = new System.Drawing.Size(292, 22);
			rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel3.TabIndex = 2189;
			rjLabel3.Text = "Result type";
			rjLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rjLabel6
			// 
			rjLabel6.BackColor = System.Drawing.Color.Transparent;
			rjLabel6.Dock = System.Windows.Forms.DockStyle.Fill;
			rjLabel6.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			rjLabel6.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			rjLabel6.LinkLabel = false;
			rjLabel6.Location = new System.Drawing.Point(301, 0);
			rjLabel6.Name = "rjLabel6";
			rjLabel6.Size = new System.Drawing.Size(292, 22);
			rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel6.TabIndex = 2191;
			rjLabel6.Text = "Method";
			rjLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbAdaptiveThresholdMenu
			// 
			cbAdaptiveThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbAdaptiveThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbAdaptiveThresholdMenu.BackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbAdaptiveThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(144, 166, 194);
			cbAdaptiveThresholdMenu.BorderRadius = 0;
			cbAdaptiveThresholdMenu.BorderSize = 1;
			cbAdaptiveThresholdMenu.Customizable = true;
			cbAdaptiveThresholdMenu.DataSource = null;
			cbAdaptiveThresholdMenu.Dock = System.Windows.Forms.DockStyle.Fill;
			cbAdaptiveThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbAdaptiveThresholdMenu.DropDownItemHeight = 26;
			cbAdaptiveThresholdMenu.DropDownSelectedBackColor = System.Drawing.Color.FromArgb(39, 111, 156);
			cbAdaptiveThresholdMenu.DropDownSelectedTextColor = System.Drawing.Color.White;
			cbAdaptiveThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbAdaptiveThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbAdaptiveThresholdMenu.Font = new System.Drawing.Font("Segoe UI", 9.5F);
			cbAdaptiveThresholdMenu.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbAdaptiveThresholdMenu.IconColor = System.Drawing.Color.FromArgb(47, 111, 171);
			cbAdaptiveThresholdMenu.Location = new System.Drawing.Point(3, 25);
			cbAdaptiveThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
			cbAdaptiveThresholdMenu.Name = "cbAdaptiveThresholdMenu";
			cbAdaptiveThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
			cbAdaptiveThresholdMenu.SelectedIndex = -1;
			cbAdaptiveThresholdMenu.Size = new System.Drawing.Size(292, 30);
			cbAdaptiveThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbAdaptiveThresholdMenu.TabIndex = 2190;
			cbAdaptiveThresholdMenu.Texts = "";
			// 
			// cbAdaptiveType
			// 
			cbAdaptiveType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbAdaptiveType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbAdaptiveType.BackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbAdaptiveType.BorderColor = System.Drawing.Color.FromArgb(144, 166, 194);
			cbAdaptiveType.BorderRadius = 0;
			cbAdaptiveType.BorderSize = 1;
			cbAdaptiveType.Customizable = true;
			cbAdaptiveType.DataSource = null;
			cbAdaptiveType.Dock = System.Windows.Forms.DockStyle.Fill;
			cbAdaptiveType.DropDownBackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbAdaptiveType.DropDownItemHeight = 26;
			cbAdaptiveType.DropDownSelectedBackColor = System.Drawing.Color.FromArgb(39, 111, 156);
			cbAdaptiveType.DropDownSelectedTextColor = System.Drawing.Color.White;
			cbAdaptiveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbAdaptiveType.DropDownTextColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbAdaptiveType.Font = new System.Drawing.Font("Segoe UI", 9.5F);
			cbAdaptiveType.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbAdaptiveType.IconColor = System.Drawing.Color.FromArgb(47, 111, 171);
			cbAdaptiveType.Location = new System.Drawing.Point(301, 25);
			cbAdaptiveType.MinimumSize = new System.Drawing.Size(100, 30);
			cbAdaptiveType.Name = "cbAdaptiveType";
			cbAdaptiveType.Padding = new System.Windows.Forms.Padding(2);
			cbAdaptiveType.SelectedIndex = -1;
			cbAdaptiveType.Size = new System.Drawing.Size(292, 30);
			cbAdaptiveType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbAdaptiveType.TabIndex = 2192;
			cbAdaptiveType.Texts = "";
			// 
			// pnlAdaptiveHeader
			// 
			pnlAdaptiveHeader.BackColor = System.Drawing.Color.FromArgb(31, 45, 66);
			pnlAdaptiveHeader.Controls.Add(lblAdaptiveHint);
			pnlAdaptiveHeader.Controls.Add(lblAdaptiveTitle);
			pnlAdaptiveHeader.Controls.Add(lblAdaptiveNo);
			pnlAdaptiveHeader.Dock = System.Windows.Forms.DockStyle.Top;
			pnlAdaptiveHeader.Location = new System.Drawing.Point(12, 304);
			pnlAdaptiveHeader.Name = "pnlAdaptiveHeader";
			pnlAdaptiveHeader.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			pnlAdaptiveHeader.Size = new System.Drawing.Size(616, 34);
			pnlAdaptiveHeader.TabIndex = 2210;
			// 
			// lblAdaptiveHint
			// 
			lblAdaptiveHint.Dock = System.Windows.Forms.DockStyle.Fill;
			lblAdaptiveHint.Font = new System.Drawing.Font("Segoe UI", 8.4F, System.Drawing.FontStyle.Bold);
			lblAdaptiveHint.ForeColor = System.Drawing.Color.FromArgb(163, 190, 218);
			lblAdaptiveHint.Location = new System.Drawing.Point(274, 0);
			lblAdaptiveHint.Name = "lblAdaptiveHint";
			lblAdaptiveHint.Size = new System.Drawing.Size(332, 34);
			lblAdaptiveHint.TabIndex = 2;
			lblAdaptiveHint.Text = "Uneven light";
			lblAdaptiveHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblAdaptiveTitle
			// 
			lblAdaptiveTitle.Dock = System.Windows.Forms.DockStyle.Left;
			lblAdaptiveTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			lblAdaptiveTitle.ForeColor = System.Drawing.Color.White;
			lblAdaptiveTitle.Location = new System.Drawing.Point(44, 0);
			lblAdaptiveTitle.Name = "lblAdaptiveTitle";
			lblAdaptiveTitle.Size = new System.Drawing.Size(230, 34);
			lblAdaptiveTitle.TabIndex = 1;
			lblAdaptiveTitle.Text = "Adaptive Threshold";
			lblAdaptiveTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblAdaptiveNo
			// 
			lblAdaptiveNo.Dock = System.Windows.Forms.DockStyle.Left;
			lblAdaptiveNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblAdaptiveNo.ForeColor = System.Drawing.Color.FromArgb(205, 239, 255);
			lblAdaptiveNo.Location = new System.Drawing.Point(10, 0);
			lblAdaptiveNo.Name = "lblAdaptiveNo";
			lblAdaptiveNo.Size = new System.Drawing.Size(34, 34);
			lblAdaptiveNo.TabIndex = 0;
			lblAdaptiveNo.Text = "03";
			lblAdaptiveNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rangeThresholdBar
			// 
			rangeThresholdBar.BackColor = System.Drawing.Color.FromArgb(24, 33, 47);
			rangeThresholdBar.Dock = System.Windows.Forms.DockStyle.Top;
			rangeThresholdBar.ForeColor = System.Drawing.Color.White;
			rangeThresholdBar.Invert = false;
			rangeThresholdBar.Location = new System.Drawing.Point(12, 256);
			rangeThresholdBar.Maximum = 255;
			rangeThresholdBar.Minimum = 0;
			rangeThresholdBar.MinimumSize = new System.Drawing.Size(420, 44);
			rangeThresholdBar.Name = "rangeThresholdBar";
			rangeThresholdBar.RangeMax = 255;
			rangeThresholdBar.RangeMin = 30;
			rangeThresholdBar.Size = new System.Drawing.Size(616, 48);
			rangeThresholdBar.TabIndex = 2209;
			// 
			// pnlRangeHeader
			// 
			pnlRangeHeader.BackColor = System.Drawing.Color.FromArgb(31, 45, 66);
			pnlRangeHeader.Controls.Add(lblRangeHint);
			pnlRangeHeader.Controls.Add(lblRangeTitle);
			pnlRangeHeader.Controls.Add(lblRangeNo);
			pnlRangeHeader.Dock = System.Windows.Forms.DockStyle.Top;
			pnlRangeHeader.Location = new System.Drawing.Point(12, 222);
			pnlRangeHeader.Name = "pnlRangeHeader";
			pnlRangeHeader.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			pnlRangeHeader.Size = new System.Drawing.Size(616, 34);
			pnlRangeHeader.TabIndex = 2208;
			// 
			// lblRangeHint
			// 
			lblRangeHint.Dock = System.Windows.Forms.DockStyle.Fill;
			lblRangeHint.Font = new System.Drawing.Font("Segoe UI", 8.4F, System.Drawing.FontStyle.Bold);
			lblRangeHint.ForeColor = System.Drawing.Color.FromArgb(163, 190, 218);
			lblRangeHint.Location = new System.Drawing.Point(274, 0);
			lblRangeHint.Name = "lblRangeHint";
			lblRangeHint.Size = new System.Drawing.Size(332, 34);
			lblRangeHint.TabIndex = 2;
			lblRangeHint.Text = "Keep Min-Max";
			lblRangeHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblRangeTitle
			// 
			lblRangeTitle.Dock = System.Windows.Forms.DockStyle.Left;
			lblRangeTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			lblRangeTitle.ForeColor = System.Drawing.Color.White;
			lblRangeTitle.Location = new System.Drawing.Point(44, 0);
			lblRangeTitle.Name = "lblRangeTitle";
			lblRangeTitle.Size = new System.Drawing.Size(230, 34);
			lblRangeTitle.TabIndex = 1;
			lblRangeTitle.Text = "Range Threshold";
			lblRangeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblRangeNo
			// 
			lblRangeNo.Dock = System.Windows.Forms.DockStyle.Left;
			lblRangeNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblRangeNo.ForeColor = System.Drawing.Color.FromArgb(120, 186, 222);
			lblRangeNo.Location = new System.Drawing.Point(10, 0);
			lblRangeNo.Name = "lblRangeNo";
			lblRangeNo.Size = new System.Drawing.Size(34, 34);
			lblRangeNo.TabIndex = 0;
			lblRangeNo.Text = "02";
			lblRangeNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// thresholdValueBar
			// 
			thresholdValueBar.BackColor = System.Drawing.Color.FromArgb(24, 33, 47);
			thresholdValueBar.Caption = "Value:";
			thresholdValueBar.Dock = System.Windows.Forms.DockStyle.Top;
			thresholdValueBar.ForeColor = System.Drawing.Color.White;
			thresholdValueBar.Location = new System.Drawing.Point(12, 174);
			thresholdValueBar.Maximum = 255;
			thresholdValueBar.Minimum = 0;
			thresholdValueBar.MinimumSize = new System.Drawing.Size(360, 44);
			thresholdValueBar.Name = "thresholdValueBar";
			thresholdValueBar.Size = new System.Drawing.Size(616, 48);
			thresholdValueBar.TabIndex = 2207;
			thresholdValueBar.Value = 1;
			// 
			// cbThresholdMenu
			// 
			cbThresholdMenu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbThresholdMenu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbThresholdMenu.BackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbThresholdMenu.BorderColor = System.Drawing.Color.FromArgb(144, 166, 194);
			cbThresholdMenu.BorderRadius = 0;
			cbThresholdMenu.BorderSize = 1;
			cbThresholdMenu.Customizable = true;
			cbThresholdMenu.DataSource = null;
			cbThresholdMenu.Dock = System.Windows.Forms.DockStyle.Top;
			cbThresholdMenu.DropDownBackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbThresholdMenu.DropDownItemHeight = 26;
			cbThresholdMenu.DropDownSelectedBackColor = System.Drawing.Color.FromArgb(39, 111, 156);
			cbThresholdMenu.DropDownSelectedTextColor = System.Drawing.Color.White;
			cbThresholdMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbThresholdMenu.DropDownTextColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbThresholdMenu.Font = new System.Drawing.Font("Segoe UI", 9.5F);
			cbThresholdMenu.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbThresholdMenu.IconColor = System.Drawing.Color.FromArgb(47, 111, 171);
			cbThresholdMenu.Location = new System.Drawing.Point(12, 140);
			cbThresholdMenu.MinimumSize = new System.Drawing.Size(100, 30);
			cbThresholdMenu.Name = "cbThresholdMenu";
			cbThresholdMenu.Padding = new System.Windows.Forms.Padding(2);
			cbThresholdMenu.SelectedIndex = -1;
			cbThresholdMenu.Size = new System.Drawing.Size(616, 34);
			cbThresholdMenu.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbThresholdMenu.TabIndex = 2181;
			cbThresholdMenu.Texts = "";
			// 
			// rjLabel4
			// 
			rjLabel4.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			rjLabel4.Dock = System.Windows.Forms.DockStyle.Top;
			rjLabel4.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			rjLabel4.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			rjLabel4.LinkLabel = false;
			rjLabel4.Location = new System.Drawing.Point(12, 116);
			rjLabel4.Name = "rjLabel4";
			rjLabel4.Padding = new System.Windows.Forms.Padding(10, 4, 8, 0);
			rjLabel4.Size = new System.Drawing.Size(616, 24);
			rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
			rjLabel4.TabIndex = 2180;
			rjLabel4.Text = "Result type";
			rjLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pnlBasicHeader
			// 
			pnlBasicHeader.BackColor = System.Drawing.Color.FromArgb(31, 45, 66);
			pnlBasicHeader.Controls.Add(lblBasicHint);
			pnlBasicHeader.Controls.Add(lblBasicTitle);
			pnlBasicHeader.Controls.Add(lblBasicNo);
			pnlBasicHeader.Dock = System.Windows.Forms.DockStyle.Top;
			pnlBasicHeader.Location = new System.Drawing.Point(12, 82);
			pnlBasicHeader.Name = "pnlBasicHeader";
			pnlBasicHeader.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			pnlBasicHeader.Size = new System.Drawing.Size(616, 34);
			pnlBasicHeader.TabIndex = 2206;
			// 
			// lblBasicHint
			// 
			lblBasicHint.Dock = System.Windows.Forms.DockStyle.Fill;
			lblBasicHint.Font = new System.Drawing.Font("Segoe UI", 8.4F, System.Drawing.FontStyle.Bold);
			lblBasicHint.ForeColor = System.Drawing.Color.FromArgb(163, 190, 218);
			lblBasicHint.Location = new System.Drawing.Point(274, 0);
			lblBasicHint.Name = "lblBasicHint";
			lblBasicHint.Size = new System.Drawing.Size(332, 34);
			lblBasicHint.TabIndex = 2;
			lblBasicHint.Text = "Single cutoff";
			lblBasicHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblBasicTitle
			// 
			lblBasicTitle.Dock = System.Windows.Forms.DockStyle.Left;
			lblBasicTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			lblBasicTitle.ForeColor = System.Drawing.Color.White;
			lblBasicTitle.Location = new System.Drawing.Point(44, 0);
			lblBasicTitle.Name = "lblBasicTitle";
			lblBasicTitle.Size = new System.Drawing.Size(230, 34);
			lblBasicTitle.TabIndex = 1;
			lblBasicTitle.Text = "Basic Threshold";
			lblBasicTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblBasicNo
			// 
			lblBasicNo.Dock = System.Windows.Forms.DockStyle.Left;
			lblBasicNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblBasicNo.ForeColor = System.Drawing.Color.FromArgb(120, 186, 222);
			lblBasicNo.Location = new System.Drawing.Point(10, 0);
			lblBasicNo.Name = "lblBasicNo";
			lblBasicNo.Size = new System.Drawing.Size(34, 34);
			lblBasicNo.TabIndex = 0;
			lblBasicNo.Text = "01";
			lblBasicNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pnlLayerFlow
			// 
			pnlLayerFlow.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
			pnlLayerFlow.Controls.Add(tblLayerFlow);
			pnlLayerFlow.Dock = System.Windows.Forms.DockStyle.Top;
			pnlLayerFlow.Location = new System.Drawing.Point(12, 10);
			pnlLayerFlow.Name = "pnlLayerFlow";
			pnlLayerFlow.Padding = new System.Windows.Forms.Padding(10, 6, 10, 8);
			pnlLayerFlow.Size = new System.Drawing.Size(616, 72);
			pnlLayerFlow.TabIndex = 2215;
			// 
			// tblLayerFlow
			// 
			tblLayerFlow.ColumnCount = 2;
			tblLayerFlow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblLayerFlow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tblLayerFlow.Controls.Add(lblInputLayer, 0, 0);
			tblLayerFlow.Controls.Add(lblOutputLayer, 1, 0);
			tblLayerFlow.Controls.Add(cbInputLayer, 0, 1);
			tblLayerFlow.Controls.Add(lblOutputLayerValue, 1, 1);
			tblLayerFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			tblLayerFlow.Location = new System.Drawing.Point(10, 6);
			tblLayerFlow.Name = "tblLayerFlow";
			tblLayerFlow.RowCount = 2;
			tblLayerFlow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			tblLayerFlow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tblLayerFlow.Size = new System.Drawing.Size(596, 58);
			tblLayerFlow.TabIndex = 0;
			// 
			// lblInputLayer
			// 
			lblInputLayer.Dock = System.Windows.Forms.DockStyle.Fill;
			lblInputLayer.Font = new System.Drawing.Font("Segoe UI", 8.6F, System.Drawing.FontStyle.Bold);
			lblInputLayer.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			lblInputLayer.Location = new System.Drawing.Point(3, 0);
			lblInputLayer.Name = "lblInputLayer";
			lblInputLayer.Size = new System.Drawing.Size(292, 22);
			lblInputLayer.TabIndex = 0;
			lblInputLayer.Text = "Input Source";
			lblInputLayer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblOutputLayer
			// 
			lblOutputLayer.Dock = System.Windows.Forms.DockStyle.Fill;
			lblOutputLayer.Font = new System.Drawing.Font("Segoe UI", 8.6F, System.Drawing.FontStyle.Bold);
			lblOutputLayer.ForeColor = System.Drawing.Color.FromArgb(30, 73, 116);
			lblOutputLayer.Location = new System.Drawing.Point(301, 0);
			lblOutputLayer.Name = "lblOutputLayer";
			lblOutputLayer.Size = new System.Drawing.Size(292, 22);
			lblOutputLayer.TabIndex = 1;
			lblOutputLayer.Text = "Output Result";
			lblOutputLayer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbInputLayer
			// 
			cbInputLayer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
			cbInputLayer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
			cbInputLayer.BackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbInputLayer.BorderColor = System.Drawing.Color.FromArgb(144, 166, 194);
			cbInputLayer.BorderRadius = 0;
			cbInputLayer.BorderSize = 1;
			cbInputLayer.Customizable = true;
			cbInputLayer.DataSource = null;
			cbInputLayer.Dock = System.Windows.Forms.DockStyle.Fill;
			cbInputLayer.DropDownBackColor = System.Drawing.Color.FromArgb(248, 251, 254);
			cbInputLayer.DropDownItemHeight = 26;
			cbInputLayer.DropDownSelectedBackColor = System.Drawing.Color.FromArgb(39, 111, 156);
			cbInputLayer.DropDownSelectedTextColor = System.Drawing.Color.White;
			cbInputLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbInputLayer.DropDownTextColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbInputLayer.Font = new System.Drawing.Font("Segoe UI", 9.5F);
			cbInputLayer.ForeColor = System.Drawing.Color.FromArgb(20, 38, 58);
			cbInputLayer.IconColor = System.Drawing.Color.FromArgb(47, 111, 171);
			cbInputLayer.Location = new System.Drawing.Point(3, 25);
			cbInputLayer.MinimumSize = new System.Drawing.Size(100, 30);
			cbInputLayer.Name = "cbInputLayer";
			cbInputLayer.Padding = new System.Windows.Forms.Padding(2);
			cbInputLayer.SelectedIndex = -1;
			cbInputLayer.Size = new System.Drawing.Size(292, 30);
			cbInputLayer.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
			cbInputLayer.TabIndex = 2;
			cbInputLayer.Texts = "";
			// 
			// lblOutputLayerValue
			// 
			lblOutputLayerValue.BackColor = System.Drawing.Color.FromArgb(229, 238, 250);
			lblOutputLayerValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			lblOutputLayerValue.Dock = System.Windows.Forms.DockStyle.Fill;
			lblOutputLayerValue.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
			lblOutputLayerValue.ForeColor = System.Drawing.Color.FromArgb(24, 77, 126);
			lblOutputLayerValue.Location = new System.Drawing.Point(301, 22);
			lblOutputLayerValue.Name = "lblOutputLayerValue";
			lblOutputLayerValue.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
			lblOutputLayerValue.Size = new System.Drawing.Size(292, 36);
			lblOutputLayerValue.TabIndex = 3;
			lblOutputLayerValue.Text = "Threshold";
			lblOutputLayerValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormThreshold
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			AutoScroll = true;
			ClientSize = new System.Drawing.Size(640, 690);
			Controls.Add(panel1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			Name = "FormThreshold";
			Text = "Threshold";
			Load += Form_Load;
			VisibleChanged += Form_VisibleChanged;
			panel1.ResumeLayout(false);
			pnlPreviewSummary.ResumeLayout(false);
			rjPanel1.ResumeLayout(false);
			tblAdaptiveParams.ResumeLayout(false);
			pnlAdaptiveOptions.ResumeLayout(false);
			tblAdaptiveOptions.ResumeLayout(false);
			pnlAdaptiveHeader.ResumeLayout(false);
			pnlRangeHeader.ResumeLayout(false);
			pnlBasicHeader.ResumeLayout(false);
			pnlLayerFlow.ResumeLayout(false);
			tblLayerFlow.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlLayerFlow;
        private System.Windows.Forms.TableLayoutPanel tblLayerFlow;
        private System.Windows.Forms.Label lblInputLayer;
        private System.Windows.Forms.Label lblOutputLayer;
        private RJCodeUI_M1.RJControls.RJComboBox cbInputLayer;
        private System.Windows.Forms.Label lblOutputLayerValue;
        private System.Windows.Forms.Panel pnlPreviewSummary;
        private System.Windows.Forms.Label lblPreviewDescription;
        private System.Windows.Forms.Label lblPreviewTitle;
        private System.Windows.Forms.Button btnAddToPipeline;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private System.Windows.Forms.TableLayoutPanel tblAdaptiveParams;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private RJCodeUI_M1.RJControls.RJTextBox tbBlockSize;
        private RJCodeUI_M1.RJControls.RJTextBox tbWeight;
        private OpenVisionLab.ThresholdValueTrackBar adaptiveValueBar;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private System.Windows.Forms.Panel pnlAdaptiveOptions;
        private System.Windows.Forms.TableLayoutPanel tblAdaptiveOptions;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveThresholdMenu;
        private RJCodeUI_M1.RJControls.RJComboBox cbAdaptiveType;
        private System.Windows.Forms.Panel pnlAdaptiveHeader;
        private System.Windows.Forms.Label lblAdaptiveHint;
        private System.Windows.Forms.Label lblAdaptiveTitle;
        private System.Windows.Forms.Label lblAdaptiveNo;
        private OpenVisionLab.ThresholdRangeTrackBar rangeThresholdBar;
        private System.Windows.Forms.Panel pnlRangeHeader;
        private System.Windows.Forms.Label lblRangeHint;
        private System.Windows.Forms.Label lblRangeTitle;
        private System.Windows.Forms.Label lblRangeNo;
        private OpenVisionLab.ThresholdValueTrackBar thresholdValueBar;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJComboBox cbThresholdMenu;
        private System.Windows.Forms.Panel pnlBasicHeader;
        private System.Windows.Forms.Label lblBasicHint;
        private System.Windows.Forms.Label lblBasicTitle;
        private System.Windows.Forms.Label lblBasicNo;
    }
}
