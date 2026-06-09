namespace OpenVisionLab
{
    partial class FormVision_EdgeDetection
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
                        this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbEdgeType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.ibSource = new OpenVisionLab.VisionTestImageCanvas();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new OpenVisionLab.VisionTestImageCanvas();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Tab = new System.Windows.Forms.TabControl();
            this.metroTabPage1 = new System.Windows.Forms.TabPage();
            this.rjPanel3 = new RJCodeUI_M1.RJControls.RJPanel();
            this.chkUseL2 = new RJCodeUI_M1.RJControls.RJCheckBox();
            this.tbThresholdLow = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.tbThresholdHight = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbSobelMask = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.metroTabPage2 = new System.Windows.Forms.TabPage();
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.nudKernel = new System.Windows.Forms.NumericUpDown();
            this.nudDegreeY = new System.Windows.Forms.NumericUpDown();
            this.nudDegreeX = new System.Windows.Forms.NumericUpDown();
            this.rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel7 = new RJCodeUI_M1.RJControls.RJLabel();
            this.metroTabPage3 = new System.Windows.Forms.TabPage();
            this.rjPanel2 = new RJCodeUI_M1.RJControls.RJPanel();
            this.nudScharrDegreeY = new System.Windows.Forms.NumericUpDown();
            this.nudScharrDegreeX = new System.Windows.Forms.NumericUpDown();
            this.rjLabel9 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel10 = new RJCodeUI_M1.RJControls.RJLabel();
            this.metroTabPage4 = new System.Windows.Forms.TabPage();
            this.rjPanel4 = new RJCodeUI_M1.RJControls.RJPanel();
            this.nudLaplacianKernel = new System.Windows.Forms.NumericUpDown();
            this.rjLabel8 = new RJCodeUI_M1.RJControls.RJLabel();
            this.btnFilterRun = new RJCodeUI_M1.RJControls.RJButton();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.Tab.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.rjPanel3.SuspendLayout();
            this.metroTabPage2.SuspendLayout();
            this.rjPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKernel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDegreeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDegreeX)).BeginInit();
            this.metroTabPage3.SuspendLayout();
            this.rjPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScharrDegreeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScharrDegreeX)).BeginInit();
            this.metroTabPage4.SuspendLayout();
            this.rjPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaplacianKernel)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(918, 613);
            // 
            // 
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.rjLabel1);
            this.groupBox1.Controls.Add(this.cbEdgeType);
            this.groupBox1.Location = new System.Drawing.Point(424, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 73);
            this.groupBox1.TabIndex = 2147;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operations";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(6, 15);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(139, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2159;
            this.rjLabel1.Text = "Edge Detector Type";
            // 
            // cbEdgeType
            // 
            this.cbEdgeType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbEdgeType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbEdgeType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbEdgeType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbEdgeType.BorderRadius = 3;
            this.cbEdgeType.BorderSize = 2;
            this.cbEdgeType.Customizable = false;
            this.cbEdgeType.DataSource = null;
            this.cbEdgeType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbEdgeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEdgeType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbEdgeType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbEdgeType.ForeColor = System.Drawing.Color.DimGray;
            this.cbEdgeType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbEdgeType.Location = new System.Drawing.Point(6, 34);
            this.cbEdgeType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbEdgeType.Name = "cbEdgeType";
            this.cbEdgeType.Padding = new System.Windows.Forms.Padding(2);
            this.cbEdgeType.SelectedIndex = -1;
            this.cbEdgeType.Size = new System.Drawing.Size(472, 30);
            this.cbEdgeType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbEdgeType.TabIndex = 2158;
            this.cbEdgeType.Texts = "";
            this.cbEdgeType.OnSelectedIndexChanged += new System.EventHandler(this.cbFilterType_OnSelectedIndexChanged);
            // 
            // ibSource
            // 
            this.ibSource.Location = new System.Drawing.Point(6, 20);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(374, 220);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(16, 58);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(390, 285);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Input Layer";
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 3;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(374, 32);
            this.cbLayerList.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList.TabIndex = 2158;
            this.cbLayerList.Texts = "";
            this.cbLayerList.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox4.Controls.Add(this.cbLayerList2);
            this.groupBox4.Controls.Add(this.btnNewPanel_Desty);
            this.groupBox4.Controls.Add(this.ibDestination);
            this.groupBox4.Location = new System.Drawing.Point(16, 360);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(390, 285);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output Layer";
            // 
            // cbLayerList2
            // 
            this.cbLayerList2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.BorderRadius = 3;
            this.cbLayerList2.BorderSize = 2;
            this.cbLayerList2.Customizable = false;
            this.cbLayerList2.DataSource = null;
            this.cbLayerList2.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList2.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList2.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList2.Location = new System.Drawing.Point(8, 248);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(338, 32);
            this.cbLayerList2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList2.TabIndex = 2159;
            this.cbLayerList2.Texts = "";
            this.cbLayerList2.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList2_SelectedIndexChanged);
            // 
            // btnNewPanel_Desty
            // 
            this.btnNewPanel_Desty.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPanel_Desty.BackIcon = true;
            this.btnNewPanel_Desty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPanel_Desty.Customizable = true;
            this.btnNewPanel_Desty.DropdownMenu = null;
            this.btnNewPanel_Desty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            this.btnNewPanel_Desty.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.btnNewPanel_Desty.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewPanel_Desty.IconSize = 30;
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(354, 250);
            this.btnNewPanel_Desty.Name = "btnNewPanel_Desty";
            this.btnNewPanel_Desty.Size = new System.Drawing.Size(28, 28);
            this.btnNewPanel_Desty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnNewPanel_Desty.TabIndex = 2157;
            this.btnNewPanel_Desty.TabStop = false;
            this.btnNewPanel_Desty.Click += new System.EventHandler(this.btnNewPanel_Desty_Click);
            // 
            // ibDestination
            // 
            this.ibDestination.Location = new System.Drawing.Point(6, 20);
            this.ibDestination.Name = "ibDestination";
            this.ibDestination.Size = new System.Drawing.Size(374, 220);
            this.ibDestination.TabIndex = 2149;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // 
            // Tab
            // 
            this.Tab.Controls.Add(this.metroTabPage1);
            this.Tab.Controls.Add(this.metroTabPage2);
            this.Tab.Controls.Add(this.metroTabPage3);
            this.Tab.Controls.Add(this.metroTabPage4);
            this.Tab.Location = new System.Drawing.Point(424, 146);
            this.Tab.Margin = new System.Windows.Forms.Padding(0);
            this.Tab.Name = "Tab";
            this.Tab.SelectedIndex = 0;
            this.Tab.Size = new System.Drawing.Size(488, 250);
            this.Tab.TabIndex = 2164;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.metroTabPage1.Controls.Add(this.rjPanel3);
            this.metroTabPage1.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(480, 208);
            this.metroTabPage1.TabIndex = 8;
            this.metroTabPage1.Text = "Canny";
            // 
            // rjPanel3
            // 
            this.rjPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel3.BorderRadius = 5;
            this.rjPanel3.Controls.Add(this.chkUseL2);
            this.rjPanel3.Controls.Add(this.tbThresholdLow);
            this.rjPanel3.Controls.Add(this.rjLabel3);
            this.rjPanel3.Controls.Add(this.tbThresholdHight);
            this.rjPanel3.Controls.Add(this.tbSobelMask);
            this.rjPanel3.Controls.Add(this.rjLabel4);
            this.rjPanel3.Controls.Add(this.rjLabel2);
            this.rjPanel3.Customizable = false;
            this.rjPanel3.Location = new System.Drawing.Point(0, 3);
            this.rjPanel3.Name = "rjPanel3";
            this.rjPanel3.Size = new System.Drawing.Size(480, 208);
            this.rjPanel3.TabIndex = 2131;
            // 
            // chkUseL2
            // 
            this.chkUseL2.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUseL2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkUseL2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseL2.BorderSize = 1;
            this.chkUseL2.Check = true;
            this.chkUseL2.Checked = true;
            this.chkUseL2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseL2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkUseL2.Customizable = false;
            this.chkUseL2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseL2.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.chkUseL2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.chkUseL2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(82)))), ((int)(((byte)(180)))));
            this.chkUseL2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkUseL2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseL2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.chkUseL2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.chkUseL2.Location = new System.Drawing.Point(114, 83);
            this.chkUseL2.MinimumSize = new System.Drawing.Size(0, 21);
            this.chkUseL2.Name = "chkUseL2";
            this.chkUseL2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chkUseL2.Size = new System.Drawing.Size(146, 26);
            this.chkUseL2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.chkUseL2.TabIndex = 2161;
            this.chkUseL2.Text = "L2 그레이디언트";
            this.chkUseL2.UseVisualStyleBackColor = false;
            // 
            // tbThresholdLow
            // 
            this.tbThresholdLow._Customizable = false;
            this.tbThresholdLow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbThresholdLow.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbThresholdLow.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbThresholdLow.BorderRadius = 10;
            this.tbThresholdLow.BorderSize = 1;
            this.tbThresholdLow.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbThresholdLow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbThresholdLow.Location = new System.Drawing.Point(3, 83);
            this.tbThresholdLow.MultiLine = false;
            this.tbThresholdLow.Name = "tbThresholdLow";
            this.tbThresholdLow.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbThresholdLow.PasswordChar = false;
            this.tbThresholdLow.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbThresholdLow.PlaceHolderText = "200";
            this.tbThresholdLow.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbThresholdLow.Size = new System.Drawing.Size(105, 31);
            this.tbThresholdLow.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbThresholdLow.TabIndex = 2156;
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(117, 7);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(81, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 2160;
            this.rjLabel3.Text = "Sobel Mask";
            // 
            // tbThresholdHight
            // 
            this.tbThresholdHight._Customizable = false;
            this.tbThresholdHight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbThresholdHight.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbThresholdHight.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbThresholdHight.BorderRadius = 10;
            this.tbThresholdHight.BorderSize = 1;
            this.tbThresholdHight.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbThresholdHight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbThresholdHight.Location = new System.Drawing.Point(3, 26);
            this.tbThresholdHight.MultiLine = false;
            this.tbThresholdHight.Name = "tbThresholdHight";
            this.tbThresholdHight.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbThresholdHight.PasswordChar = false;
            this.tbThresholdHight.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbThresholdHight.PlaceHolderText = "100";
            this.tbThresholdHight.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbThresholdHight.Size = new System.Drawing.Size(105, 31);
            this.tbThresholdHight.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbThresholdHight.TabIndex = 2155;
            // 
            // tbSobelMask
            // 
            this.tbSobelMask._Customizable = false;
            this.tbSobelMask.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbSobelMask.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSobelMask.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbSobelMask.BorderRadius = 10;
            this.tbSobelMask.BorderSize = 1;
            this.tbSobelMask.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbSobelMask.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSobelMask.Location = new System.Drawing.Point(114, 26);
            this.tbSobelMask.MultiLine = false;
            this.tbSobelMask.Name = "tbSobelMask";
            this.tbSobelMask.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbSobelMask.PasswordChar = false;
            this.tbSobelMask.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbSobelMask.PlaceHolderText = "3";
            this.tbSobelMask.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSobelMask.Size = new System.Drawing.Size(105, 31);
            this.tbSobelMask.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteBorder;
            this.tbSobelMask.TabIndex = 2159;
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(6, 7);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(35, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 2157;
            this.rjLabel4.Text = "High";
            // 
            // rjLabel2
            // 
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(6, 64);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(33, 16);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel2.TabIndex = 2158;
            this.rjLabel2.Text = "Low";
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.Controls.Add(this.rjPanel1);
            this.metroTabPage2.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(480, 208);
            this.metroTabPage2.TabIndex = 9;
            this.metroTabPage2.Text = "Sobel";
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.nudKernel);
            this.rjPanel1.Controls.Add(this.nudDegreeY);
            this.rjPanel1.Controls.Add(this.nudDegreeX);
            this.rjPanel1.Controls.Add(this.rjLabel5);
            this.rjPanel1.Controls.Add(this.rjLabel6);
            this.rjPanel1.Controls.Add(this.rjLabel7);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Location = new System.Drawing.Point(0, 1);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(480, 208);
            this.rjPanel1.TabIndex = 2132;
            // 
            // nudKernel
            // 
            this.nudKernel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudKernel.Location = new System.Drawing.Point(120, 30);
            this.nudKernel.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudKernel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudKernel.Name = "nudKernel";
            this.nudKernel.Size = new System.Drawing.Size(90, 27);
            this.nudKernel.TabIndex = 2164;
            this.nudKernel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudDegreeY
            // 
            this.nudDegreeY.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDegreeY.Location = new System.Drawing.Point(9, 84);
            this.nudDegreeY.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudDegreeY.Name = "nudDegreeY";
            this.nudDegreeY.Size = new System.Drawing.Size(90, 27);
            this.nudDegreeY.TabIndex = 2163;
            // 
            // nudDegreeX
            // 
            this.nudDegreeX.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDegreeX.Location = new System.Drawing.Point(9, 30);
            this.nudDegreeX.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudDegreeX.Name = "nudDegreeX";
            this.nudDegreeX.Size = new System.Drawing.Size(90, 27);
            this.nudDegreeX.TabIndex = 2162;
            // 
            // rjLabel5
            // 
            this.rjLabel5.AutoSize = true;
            this.rjLabel5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel5.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel5.LinkLabel = false;
            this.rjLabel5.Location = new System.Drawing.Point(117, 7);
            this.rjLabel5.Name = "rjLabel5";
            this.rjLabel5.Size = new System.Drawing.Size(47, 16);
            this.rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel5.TabIndex = 2160;
            this.rjLabel5.Text = "Kernel";
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(6, 7);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(67, 16);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel6.TabIndex = 2157;
            this.rjLabel6.Text = "X Degree";
            // 
            // rjLabel7
            // 
            this.rjLabel7.AutoSize = true;
            this.rjLabel7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel7.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel7.LinkLabel = false;
            this.rjLabel7.Location = new System.Drawing.Point(6, 64);
            this.rjLabel7.Name = "rjLabel7";
            this.rjLabel7.Size = new System.Drawing.Size(67, 16);
            this.rjLabel7.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel7.TabIndex = 2158;
            this.rjLabel7.Text = "Y Degree";
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.Controls.Add(this.rjPanel2);
            this.metroTabPage3.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(480, 208);
            this.metroTabPage3.TabIndex = 10;
            this.metroTabPage3.Text = "Scharr";
            // 
            // rjPanel2
            // 
            this.rjPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel2.BorderRadius = 5;
            this.rjPanel2.Controls.Add(this.nudScharrDegreeY);
            this.rjPanel2.Controls.Add(this.nudScharrDegreeX);
            this.rjPanel2.Controls.Add(this.rjLabel9);
            this.rjPanel2.Controls.Add(this.rjLabel10);
            this.rjPanel2.Customizable = false;
            this.rjPanel2.Location = new System.Drawing.Point(0, 1);
            this.rjPanel2.Name = "rjPanel2";
            this.rjPanel2.Size = new System.Drawing.Size(480, 208);
            this.rjPanel2.TabIndex = 2133;
            // 
            // nudScharrDegreeY
            // 
            this.nudScharrDegreeY.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudScharrDegreeY.Location = new System.Drawing.Point(113, 30);
            this.nudScharrDegreeY.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudScharrDegreeY.Name = "nudScharrDegreeY";
            this.nudScharrDegreeY.Size = new System.Drawing.Size(90, 27);
            this.nudScharrDegreeY.TabIndex = 2163;
            // 
            // nudScharrDegreeX
            // 
            this.nudScharrDegreeX.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudScharrDegreeX.Location = new System.Drawing.Point(9, 30);
            this.nudScharrDegreeX.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudScharrDegreeX.Name = "nudScharrDegreeX";
            this.nudScharrDegreeX.Size = new System.Drawing.Size(90, 27);
            this.nudScharrDegreeX.TabIndex = 2162;
            // 
            // rjLabel9
            // 
            this.rjLabel9.AutoSize = true;
            this.rjLabel9.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel9.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel9.LinkLabel = false;
            this.rjLabel9.Location = new System.Drawing.Point(6, 7);
            this.rjLabel9.Name = "rjLabel9";
            this.rjLabel9.Size = new System.Drawing.Size(67, 16);
            this.rjLabel9.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel9.TabIndex = 2157;
            this.rjLabel9.Text = "X Degree";
            // 
            // rjLabel10
            // 
            this.rjLabel10.AutoSize = true;
            this.rjLabel10.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel10.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel10.LinkLabel = false;
            this.rjLabel10.Location = new System.Drawing.Point(110, 8);
            this.rjLabel10.Name = "rjLabel10";
            this.rjLabel10.Size = new System.Drawing.Size(67, 16);
            this.rjLabel10.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel10.TabIndex = 2158;
            this.rjLabel10.Text = "Y Degree";
            // 
            // metroTabPage4
            // 
            this.metroTabPage4.Controls.Add(this.rjPanel4);
            this.metroTabPage4.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage4.Name = "metroTabPage4";
            this.metroTabPage4.Size = new System.Drawing.Size(480, 208);
            this.metroTabPage4.TabIndex = 11;
            this.metroTabPage4.Text = "Laplacian";
            // 
            // rjPanel4
            // 
            this.rjPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel4.BorderRadius = 5;
            this.rjPanel4.Controls.Add(this.nudLaplacianKernel);
            this.rjPanel4.Controls.Add(this.rjLabel8);
            this.rjPanel4.Customizable = false;
            this.rjPanel4.Location = new System.Drawing.Point(0, 1);
            this.rjPanel4.Name = "rjPanel4";
            this.rjPanel4.Size = new System.Drawing.Size(480, 208);
            this.rjPanel4.TabIndex = 2134;
            // 
            // nudLaplacianKernel
            // 
            this.nudLaplacianKernel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLaplacianKernel.Location = new System.Drawing.Point(4, 28);
            this.nudLaplacianKernel.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudLaplacianKernel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLaplacianKernel.Name = "nudLaplacianKernel";
            this.nudLaplacianKernel.Size = new System.Drawing.Size(90, 27);
            this.nudLaplacianKernel.TabIndex = 2166;
            this.nudLaplacianKernel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rjLabel8
            // 
            this.rjLabel8.AutoSize = true;
            this.rjLabel8.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel8.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel8.LinkLabel = false;
            this.rjLabel8.Location = new System.Drawing.Point(6, 9);
            this.rjLabel8.Name = "rjLabel8";
            this.rjLabel8.Size = new System.Drawing.Size(47, 16);
            this.rjLabel8.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel8.TabIndex = 2165;
            this.rjLabel8.Text = "Kernel";
            // 
            // btnFilterRun
            // 
            this.btnFilterRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.btnFilterRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnFilterRun.BorderRadius = 3;
            this.btnFilterRun.BorderSize = 1;
            this.btnFilterRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnFilterRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(111)))), ((int)(((byte)(171)))));
            this.btnFilterRun.FlatAppearance.BorderSize = 1;
            this.btnFilterRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(241)))), ((int)(((byte)(247)))));
            this.btnFilterRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.btnFilterRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFilterRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnFilterRun.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnFilterRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(85)))), ((int)(((byte)(132)))));
            this.btnFilterRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFilterRun.IconSize = 1;
            this.btnFilterRun.Location = new System.Drawing.Point(424, 604);
            this.btnFilterRun.Name = "btnFilterRun";
            this.btnFilterRun.Size = new System.Drawing.Size(488, 40);
            this.btnFilterRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnFilterRun.TabIndex = 2163;
            this.btnFilterRun.Text = "Run";
            this.btnFilterRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnFilterRun.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnFilterRun.UseVisualStyleBackColor = false;
            this.btnFilterRun.Click += new System.EventHandler(this.btnFilterRun_Click);
            // 
            // FormVision_EdgeDetection
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Edge Detection";
            this.ClientSize = new System.Drawing.Size(920, 655);
            this.Controls.Add(this.Tab);
            this.Controls.Add(this.btnFilterRun);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_EdgeDetection";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Edge Detection";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.btnFilterRun, 0);
            this.Controls.SetChildIndex(this.Tab, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.Tab.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.rjPanel3.ResumeLayout(false);
            this.rjPanel3.PerformLayout();
            this.metroTabPage2.ResumeLayout(false);
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKernel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDegreeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDegreeX)).EndInit();
            this.metroTabPage3.ResumeLayout(false);
            this.rjPanel2.ResumeLayout(false);
            this.rjPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScharrDegreeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScharrDegreeX)).EndInit();
            this.metroTabPage4.ResumeLayout(false);
            this.rjPanel4.ResumeLayout(false);
            this.rjPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaplacianKernel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private VisionTestImageCanvas ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private VisionTestImageCanvas ibDestination;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJButton btnFilterRun;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJComboBox cbEdgeType;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJTextBox tbSobelMask;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJTextBox tbThresholdLow;
        private RJCodeUI_M1.RJControls.RJTextBox tbThresholdHight;
        private RJCodeUI_M1.RJControls.RJCheckBox chkUseL2;
        private System.Windows.Forms.TabControl Tab;
        private System.Windows.Forms.TabPage metroTabPage1;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel3;
        private System.Windows.Forms.TabPage metroTabPage2;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel7;
        private System.Windows.Forms.NumericUpDown nudKernel;
        private System.Windows.Forms.NumericUpDown nudDegreeY;
        private System.Windows.Forms.NumericUpDown nudDegreeX;
        private System.Windows.Forms.TabPage metroTabPage3;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel2;
        private System.Windows.Forms.NumericUpDown nudScharrDegreeY;
        private System.Windows.Forms.NumericUpDown nudScharrDegreeX;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel9;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel10;
        private System.Windows.Forms.TabPage metroTabPage4;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel4;
        private System.Windows.Forms.NumericUpDown nudLaplacianKernel;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel8;
    }
}



