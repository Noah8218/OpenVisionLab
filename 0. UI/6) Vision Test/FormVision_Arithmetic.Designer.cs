using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVision_Arithmetic
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
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.ibSource1 = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList1 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList_Dest = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new Cyotek.Windows.Forms.ImageBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gpSourceImage = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.ibSource2 = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tip = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbArithmeticType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.gpContrast = new System.Windows.Forms.GroupBox();
            this.tbB = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbG = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbR = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbGray = new RJCodeUI_M1.RJControls.RJTextBox();
            this.rdoColor = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoGray = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoContrast = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoSourceImage = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.btnArithmeticRun = new RJCodeUI_M1.RJControls.RJButton();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbY = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbX = new RJCodeUI_M1.RJControls.RJTextBox();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.gpSourceImage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gpContrast.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.groupBox1);
            this.pnlClientArea.Controls.Add(this.rdoSourceImage);
            this.pnlClientArea.Controls.Add(this.rdoContrast);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(871, 437);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // ibSource1
            // 
            this.ibSource1.Location = new System.Drawing.Point(6, 20);
            this.ibSource1.Name = "ibSource1";
            this.ibSource1.Size = new System.Drawing.Size(270, 200);
            this.ibSource1.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.cbLayerList1);
            this.groupBox3.Controls.Add(this.ibSource1);
            this.groupBox3.Location = new System.Drawing.Point(6, 67);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(284, 266);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source Image 1";
            // 
            // cbLayerList1
            // 
            this.cbLayerList1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList1.BorderRadius = 0;
            this.cbLayerList1.BorderSize = 2;
            this.cbLayerList1.Customizable = false;
            this.cbLayerList1.DataSource = null;
            this.cbLayerList1.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList1.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList1.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList1.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList1.Location = new System.Drawing.Point(8, 226);
            this.cbLayerList1.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList1.Name = "cbLayerList1";
            this.cbLayerList1.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList1.SelectedIndex = -1;
            this.cbLayerList1.Size = new System.Drawing.Size(270, 33);
            this.cbLayerList1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList1.TabIndex = 2158;
            this.cbLayerList1.Texts = "";
            this.cbLayerList1.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList1_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox4.Controls.Add(this.cbLayerList_Dest);
            this.groupBox4.Controls.Add(this.btnNewPanel_Desty);
            this.groupBox4.Controls.Add(this.ibDestination);
            this.groupBox4.Location = new System.Drawing.Point(586, 67);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(284, 266);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Destination Image";
            // 
            // cbLayerList_Dest
            // 
            this.cbLayerList_Dest.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList_Dest.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList_Dest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList_Dest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList_Dest.BorderRadius = 0;
            this.cbLayerList_Dest.BorderSize = 2;
            this.cbLayerList_Dest.Customizable = false;
            this.cbLayerList_Dest.DataSource = null;
            this.cbLayerList_Dest.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList_Dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList_Dest.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList_Dest.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList_Dest.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList_Dest.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList_Dest.Location = new System.Drawing.Point(6, 226);
            this.cbLayerList_Dest.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList_Dest.Name = "cbLayerList_Dest";
            this.cbLayerList_Dest.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList_Dest.SelectedIndex = -1;
            this.cbLayerList_Dest.Size = new System.Drawing.Size(221, 33);
            this.cbLayerList_Dest.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList_Dest.TabIndex = 2159;
            this.cbLayerList_Dest.Texts = "";
            this.cbLayerList_Dest.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerListDestination_SelectedIndexChanged);
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
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(237, 226);
            this.btnNewPanel_Desty.Name = "btnNewPanel_Desty";
            this.btnNewPanel_Desty.Size = new System.Drawing.Size(30, 30);
            this.btnNewPanel_Desty.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnNewPanel_Desty.TabIndex = 2157;
            this.btnNewPanel_Desty.TabStop = false;
            this.btnNewPanel_Desty.Click += new System.EventHandler(this.btnNewPanel_Desty_Click);
            // 
            // ibDestination
            // 
            this.ibDestination.Location = new System.Drawing.Point(6, 20);
            this.ibDestination.Name = "ibDestination";
            this.ibDestination.Size = new System.Drawing.Size(270, 200);
            this.ibDestination.TabIndex = 2149;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // gpSourceImage
            // 
            this.gpSourceImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.gpSourceImage.Controls.Add(this.cbLayerList2);
            this.gpSourceImage.Controls.Add(this.ibSource2);
            this.gpSourceImage.Location = new System.Drawing.Point(296, 67);
            this.gpSourceImage.Name = "gpSourceImage";
            this.gpSourceImage.Size = new System.Drawing.Size(284, 266);
            this.gpSourceImage.TabIndex = 2156;
            this.gpSourceImage.TabStop = false;
            // 
            // cbLayerList2
            // 
            this.cbLayerList2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.BorderRadius = 0;
            this.cbLayerList2.BorderSize = 2;
            this.cbLayerList2.Customizable = false;
            this.cbLayerList2.DataSource = null;
            this.cbLayerList2.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList2.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList2.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList2.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList2.Location = new System.Drawing.Point(8, 226);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(270, 33);
            this.cbLayerList2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbLayerList2.TabIndex = 2159;
            this.cbLayerList2.Texts = "";
            this.cbLayerList2.OnSelectedIndexChanged += new System.EventHandler(this.cbLayerList2_OnSelectedIndexChanged);
            // 
            // ibSource2
            // 
            this.ibSource2.Location = new System.Drawing.Point(6, 20);
            this.ibSource2.Name = "ibSource2";
            this.ibSource2.Size = new System.Drawing.Size(270, 200);
            this.ibSource2.TabIndex = 2149;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox2.Controls.Add(this.tip);
            this.groupBox2.Controls.Add(this.rjLabel1);
            this.groupBox2.Controls.Add(this.cbArithmeticType);
            this.groupBox2.Location = new System.Drawing.Point(586, 335);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 78);
            this.groupBox2.TabIndex = 2165;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operations";
            // 
            // tip
            // 
            this.tip.AutoSize = true;
            this.tip.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tip.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tip.LinkLabel = false;
            this.tip.Location = new System.Drawing.Point(83, 17);
            this.tip.Name = "tip";
            this.tip.Size = new System.Drawing.Size(27, 16);
            this.tip.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.tip.TabIndex = 2160;
            this.tip.Text = "Tip";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(6, 16);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(71, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2159;
            this.rjLabel1.Text = "Operation";
            // 
            // cbArithmeticType
            // 
            this.cbArithmeticType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbArithmeticType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbArithmeticType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbArithmeticType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbArithmeticType.BorderRadius = 0;
            this.cbArithmeticType.BorderSize = 2;
            this.cbArithmeticType.Customizable = false;
            this.cbArithmeticType.DataSource = null;
            this.cbArithmeticType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbArithmeticType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArithmeticType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbArithmeticType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbArithmeticType.ForeColor = System.Drawing.Color.DimGray;
            this.cbArithmeticType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbArithmeticType.Location = new System.Drawing.Point(6, 34);
            this.cbArithmeticType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbArithmeticType.Name = "cbArithmeticType";
            this.cbArithmeticType.Padding = new System.Windows.Forms.Padding(2);
            this.cbArithmeticType.SelectedIndex = -1;
            this.cbArithmeticType.Size = new System.Drawing.Size(272, 31);
            this.cbArithmeticType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbArithmeticType.TabIndex = 2158;
            this.cbArithmeticType.Texts = "";
            this.cbArithmeticType.OnSelectedIndexChanged += new System.EventHandler(this.cbArithmeticType_OnSelectedIndexChanged);
            // 
            // gpContrast
            // 
            this.gpContrast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.gpContrast.Controls.Add(this.tbB);
            this.gpContrast.Controls.Add(this.tbG);
            this.gpContrast.Controls.Add(this.tbR);
            this.gpContrast.Controls.Add(this.tbGray);
            this.gpContrast.Controls.Add(this.rdoColor);
            this.gpContrast.Controls.Add(this.rdoGray);
            this.gpContrast.Location = new System.Drawing.Point(296, 359);
            this.gpContrast.Name = "gpContrast";
            this.gpContrast.Size = new System.Drawing.Size(284, 107);
            this.gpContrast.TabIndex = 2166;
            this.gpContrast.TabStop = false;
            // 
            // tbB
            // 
            this.tbB._Customizable = true;
            this.tbB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbB.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbB.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbB.BorderRadius = 0;
            this.tbB.BorderSize = 1;
            this.tbB.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbB.Location = new System.Drawing.Point(118, 74);
            this.tbB.MultiLine = false;
            this.tbB.Name = "tbB";
            this.tbB.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbB.PasswordChar = false;
            this.tbB.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbB.PlaceHolderText = "1";
            this.tbB.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbB.Size = new System.Drawing.Size(45, 31);
            this.tbB.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbB.TabIndex = 2167;
            // 
            // tbG
            // 
            this.tbG._Customizable = true;
            this.tbG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbG.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbG.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbG.BorderRadius = 0;
            this.tbG.BorderSize = 1;
            this.tbG.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbG.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbG.Location = new System.Drawing.Point(67, 74);
            this.tbG.MultiLine = false;
            this.tbG.Name = "tbG";
            this.tbG.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbG.PasswordChar = false;
            this.tbG.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbG.PlaceHolderText = "1";
            this.tbG.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbG.Size = new System.Drawing.Size(45, 31);
            this.tbG.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbG.TabIndex = 2166;
            // 
            // tbR
            // 
            this.tbR._Customizable = true;
            this.tbR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbR.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbR.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbR.BorderRadius = 0;
            this.tbR.BorderSize = 1;
            this.tbR.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbR.Location = new System.Drawing.Point(16, 74);
            this.tbR.MultiLine = false;
            this.tbR.Name = "tbR";
            this.tbR.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbR.PasswordChar = false;
            this.tbR.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbR.PlaceHolderText = "1";
            this.tbR.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbR.Size = new System.Drawing.Size(45, 31);
            this.tbR.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbR.TabIndex = 2165;
            // 
            // tbGray
            // 
            this.tbGray._Customizable = true;
            this.tbGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbGray.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbGray.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbGray.BorderRadius = 0;
            this.tbGray.BorderSize = 1;
            this.tbGray.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbGray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbGray.Location = new System.Drawing.Point(118, 17);
            this.tbGray.MultiLine = false;
            this.tbGray.Name = "tbGray";
            this.tbGray.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbGray.PasswordChar = false;
            this.tbGray.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbGray.PlaceHolderText = "1";
            this.tbGray.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbGray.Size = new System.Drawing.Size(45, 31);
            this.tbGray.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbGray.TabIndex = 2164;
            // 
            // rdoColor
            // 
            this.rdoColor.AutoSize = true;
            this.rdoColor.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoColor.Customizable = true;
            this.rdoColor.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoColor.Location = new System.Drawing.Point(6, 47);
            this.rdoColor.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoColor.Name = "rdoColor";
            this.rdoColor.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoColor.Size = new System.Drawing.Size(68, 21);
            this.rdoColor.TabIndex = 2162;
            this.rdoColor.Text = "Color";
            this.rdoColor.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoColor.UseVisualStyleBackColor = true;
            this.rdoColor.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoGray
            // 
            this.rdoGray.AutoSize = true;
            this.rdoGray.Checked = true;
            this.rdoGray.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoGray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoGray.Customizable = true;
            this.rdoGray.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoGray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoGray.Location = new System.Drawing.Point(6, 17);
            this.rdoGray.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoGray.Name = "rdoGray";
            this.rdoGray.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoGray.Size = new System.Drawing.Size(106, 21);
            this.rdoGray.TabIndex = 2161;
            this.rdoGray.TabStop = true;
            this.rdoGray.Text = "Gray Scale";
            this.rdoGray.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoGray.UseVisualStyleBackColor = true;
            this.rdoGray.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoContrast
            // 
            this.rdoContrast.AutoSize = true;
            this.rdoContrast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rdoContrast.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoContrast.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoContrast.Customizable = true;
            this.rdoContrast.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoContrast.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoContrast.Location = new System.Drawing.Point(295, 294);
            this.rdoContrast.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoContrast.Name = "rdoContrast";
            this.rdoContrast.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoContrast.Size = new System.Drawing.Size(92, 21);
            this.rdoContrast.TabIndex = 2161;
            this.rdoContrast.Text = "Contrast";
            this.rdoContrast.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoContrast.UseVisualStyleBackColor = false;
            this.rdoContrast.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoSourceImage
            // 
            this.rdoSourceImage.AutoSize = true;
            this.rdoSourceImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rdoSourceImage.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoSourceImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSourceImage.Customizable = true;
            this.rdoSourceImage.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoSourceImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoSourceImage.Location = new System.Drawing.Point(295, 3);
            this.rdoSourceImage.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoSourceImage.Name = "rdoSourceImage";
            this.rdoSourceImage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoSourceImage.Size = new System.Drawing.Size(139, 21);
            this.rdoSourceImage.TabIndex = 2160;
            this.rdoSourceImage.Text = "Source Image 2";
            this.rdoSourceImage.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoSourceImage.UseVisualStyleBackColor = false;
            this.rdoSourceImage.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // btnArithmeticRun
            // 
            this.btnArithmeticRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnArithmeticRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnArithmeticRun.BorderRadius = 15;
            this.btnArithmeticRun.BorderSize = 3;
            this.btnArithmeticRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnArithmeticRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnArithmeticRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnArithmeticRun.FlatAppearance.BorderSize = 3;
            this.btnArithmeticRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnArithmeticRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnArithmeticRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArithmeticRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArithmeticRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnArithmeticRun.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.btnArithmeticRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnArithmeticRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnArithmeticRun.IconSize = 24;
            this.btnArithmeticRun.Location = new System.Drawing.Point(753, 412);
            this.btnArithmeticRun.Name = "btnArithmeticRun";
            this.btnArithmeticRun.Size = new System.Drawing.Size(117, 63);
            this.btnArithmeticRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnArithmeticRun.TabIndex = 2164;
            this.btnArithmeticRun.Text = "EXCUTE";
            this.btnArithmeticRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnArithmeticRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnArithmeticRun.UseVisualStyleBackColor = false;
            this.btnArithmeticRun.Click += new System.EventHandler(this.btnArithmeticRun_Click);
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton1.BorderRadius = 15;
            this.rjButton1.BorderSize = 3;
            this.rjButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjButton1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.rjButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.rjButton1.FlatAppearance.BorderSize = 3;
            this.rjButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.rjButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton1.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.rjButton1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjButton1.IconSize = 24;
            this.rjButton1.Location = new System.Drawing.Point(147, 42);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(117, 63);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 2165;
            this.rjButton1.Text = "Shift";
            this.rjButton1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rjButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rjButton1.UseVisualStyleBackColor = false;
            this.rjButton1.Click += new System.EventHandler(this.rjButton1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.tbY);
            this.groupBox1.Controls.Add(this.rjButton1);
            this.groupBox1.Controls.Add(this.tbX);
            this.groupBox1.Location = new System.Drawing.Point(11, 298);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 107);
            this.groupBox1.TabIndex = 2167;
            this.groupBox1.TabStop = false;
            // 
            // tbY
            // 
            this.tbY._Customizable = true;
            this.tbY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbY.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbY.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbY.BorderRadius = 0;
            this.tbY.BorderSize = 1;
            this.tbY.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbY.Location = new System.Drawing.Point(6, 54);
            this.tbY.MultiLine = false;
            this.tbY.Name = "tbY";
            this.tbY.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbY.PasswordChar = false;
            this.tbY.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbY.PlaceHolderText = "1";
            this.tbY.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbY.Size = new System.Drawing.Size(93, 31);
            this.tbY.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbY.TabIndex = 2166;
            // 
            // tbX
            // 
            this.tbX._Customizable = true;
            this.tbX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbX.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbX.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbX.BorderRadius = 0;
            this.tbX.BorderSize = 1;
            this.tbX.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbX.Location = new System.Drawing.Point(6, 17);
            this.tbX.MultiLine = false;
            this.tbX.Name = "tbX";
            this.tbX.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbX.PasswordChar = false;
            this.tbX.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbX.PlaceHolderText = "1";
            this.tbX.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbX.Size = new System.Drawing.Size(93, 31);
            this.tbX.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbX.TabIndex = 2165;
            // 
            // FormVision_Arithmetic
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Arithmetic";
            this.ClientSize = new System.Drawing.Size(873, 479);
            this.Controls.Add(this.gpContrast);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnArithmeticRun);
            this.Controls.Add(this.gpSourceImage);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_Arithmetic";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Arithmetic";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Click += new System.EventHandler(this.FormVision_Arithmetic_Click);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.gpSourceImage, 0);
            this.Controls.SetChildIndex(this.btnArithmeticRun, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.gpContrast, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.gpSourceImage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gpContrast.ResumeLayout(false);
            this.gpContrast.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private ImageBox ibSource1;
        private System.Windows.Forms.GroupBox groupBox4;
        private ImageBox ibDestination;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList_Dest;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList1;
        private System.Windows.Forms.GroupBox gpSourceImage;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private ImageBox ibSource2;
        private RJCodeUI_M1.RJControls.RJButton btnArithmeticRun;
        private System.Windows.Forms.GroupBox groupBox2;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJComboBox cbArithmeticType;
        private RJCodeUI_M1.RJControls.RJLabel tip;
        private System.Windows.Forms.GroupBox gpContrast;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoContrast;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoSourceImage;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoColor;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoGray;
        private RJCodeUI_M1.RJControls.RJTextBox tbB;
        private RJCodeUI_M1.RJControls.RJTextBox tbG;
        private RJCodeUI_M1.RJControls.RJTextBox tbR;
        private RJCodeUI_M1.RJControls.RJTextBox tbGray;
        private System.Windows.Forms.GroupBox groupBox1;
        private RJCodeUI_M1.RJControls.RJTextBox tbY;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
        private RJCodeUI_M1.RJControls.RJTextBox tbX;
    }
}