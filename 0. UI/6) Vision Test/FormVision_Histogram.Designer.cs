using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVision_Histogram
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
            this.cbType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbHeight = new System.Windows.Forms.Label();
            this.tbTilesGridSize = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lbWidth = new System.Windows.Forms.Label();
            this.tbClipLimit = new RJCodeUI_M1.RJControls.RJTextBox();
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new Cyotek.Windows.Forms.ImageBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnRun = new RJCodeUI_M1.RJControls.RJButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbAlpha = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbBeta = new RJCodeUI_M1.RJControls.RJTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(896, 276);
            // 
            // 
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.rjLabel1);
            this.groupBox1.Controls.Add(this.cbType);
            this.groupBox1.Location = new System.Drawing.Point(592, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 73);
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
            this.rjLabel1.Location = new System.Drawing.Point(6, 16);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(39, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2159;
            this.rjLabel1.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbType.BorderRadius = 0;
            this.cbType.BorderSize = 2;
            this.cbType.Customizable = false;
            this.cbType.DataSource = null;
            this.cbType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbType.ForeColor = System.Drawing.Color.DimGray;
            this.cbType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbType.Location = new System.Drawing.Point(6, 34);
            this.cbType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbType.Name = "cbType";
            this.cbType.Padding = new System.Windows.Forms.Padding(2);
            this.cbType.SelectedIndex = -1;
            this.cbType.Size = new System.Drawing.Size(287, 30);
            this.cbType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbType.TabIndex = 2158;
            this.cbType.Texts = "";
            this.cbType.OnSelectedIndexChanged += new System.EventHandler(this.cbFilterType_OnSelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox2.Controls.Add(this.tbBeta);
            this.groupBox2.Controls.Add(this.tbAlpha);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lbHeight);
            this.groupBox2.Controls.Add(this.tbTilesGridSize);
            this.groupBox2.Controls.Add(this.lbWidth);
            this.groupBox2.Controls.Add(this.tbClipLimit);
            this.groupBox2.Location = new System.Drawing.Point(592, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 143);
            this.groupBox2.TabIndex = 2148;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Kernel";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeight.ForeColor = System.Drawing.Color.Black;
            this.lbHeight.Location = new System.Drawing.Point(6, 58);
            this.lbHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(114, 18);
            this.lbHeight.TabIndex = 2153;
            this.lbHeight.Text = "TilesGridSize";
            // 
            // tbTilesGridSize
            // 
            this.tbTilesGridSize._Customizable = true;
            this.tbTilesGridSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbTilesGridSize.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbTilesGridSize.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbTilesGridSize.BorderRadius = 0;
            this.tbTilesGridSize.BorderSize = 1;
            this.tbTilesGridSize.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbTilesGridSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbTilesGridSize.Location = new System.Drawing.Point(159, 45);
            this.tbTilesGridSize.MultiLine = false;
            this.tbTilesGridSize.Name = "tbTilesGridSize";
            this.tbTilesGridSize.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbTilesGridSize.PasswordChar = false;
            this.tbTilesGridSize.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbTilesGridSize.PlaceHolderText = "3";
            this.tbTilesGridSize.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbTilesGridSize.Size = new System.Drawing.Size(45, 31);
            this.tbTilesGridSize.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbTilesGridSize.TabIndex = 2152;
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.ForeColor = System.Drawing.Color.Black;
            this.lbWidth.Location = new System.Drawing.Point(7, 18);
            this.lbWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(80, 18);
            this.lbWidth.TabIndex = 2149;
            this.lbWidth.Text = "ClipLimit";
            // 
            // tbClipLimit
            // 
            this.tbClipLimit._Customizable = true;
            this.tbClipLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbClipLimit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbClipLimit.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbClipLimit.BorderRadius = 0;
            this.tbClipLimit.BorderSize = 1;
            this.tbClipLimit.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbClipLimit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbClipLimit.Location = new System.Drawing.Point(159, 8);
            this.tbClipLimit.MultiLine = false;
            this.tbClipLimit.Name = "tbClipLimit";
            this.tbClipLimit.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbClipLimit.PasswordChar = false;
            this.tbClipLimit.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbClipLimit.PlaceHolderText = "3";
            this.tbClipLimit.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbClipLimit.Size = new System.Drawing.Size(45, 31);
            this.tbClipLimit.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbClipLimit.TabIndex = 2150;
            // 
            // ibSource
            // 
            this.ibSource.Location = new System.Drawing.Point(6, 20);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(270, 200);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(12, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(284, 269);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source Image";
            // 
            // cbLayerList
            // 
            this.cbLayerList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbLayerList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbLayerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbLayerList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.BorderRadius = 0;
            this.cbLayerList.BorderSize = 2;
            this.cbLayerList.Customizable = false;
            this.cbLayerList.DataSource = null;
            this.cbLayerList.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbLayerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerList.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbLayerList.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbLayerList.ForeColor = System.Drawing.Color.DimGray;
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbLayerList.Location = new System.Drawing.Point(6, 226);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(270, 35);
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
            this.groupBox4.Location = new System.Drawing.Point(302, 44);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(284, 269);
            this.groupBox4.TabIndex = 2155;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Destination Image";
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
            this.cbLayerList2.Location = new System.Drawing.Point(6, 226);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(221, 35);
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
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(232, 226);
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
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnRun.BorderRadius = 15;
            this.btnRun.BorderSize = 3;
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnRun.FlatAppearance.BorderSize = 3;
            this.btnRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnRun.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.btnRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRun.IconSize = 24;
            this.btnRun.Location = new System.Drawing.Point(774, 263);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(117, 50);
            this.btnRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRun.TabIndex = 2163;
            this.btnRun.Text = "EXCUTE";
            this.btnRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnFilterRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 95);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 18);
            this.label1.TabIndex = 2154;
            this.label1.Text = "Alpha";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(139, 95);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 18);
            this.label2.TabIndex = 2155;
            this.label2.Text = "Beta";
            // 
            // tbAlpha
            // 
            this.tbAlpha._Customizable = true;
            this.tbAlpha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbAlpha.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbAlpha.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbAlpha.BorderRadius = 0;
            this.tbAlpha.BorderSize = 1;
            this.tbAlpha.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbAlpha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbAlpha.Location = new System.Drawing.Point(75, 82);
            this.tbAlpha.MultiLine = false;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbAlpha.PasswordChar = false;
            this.tbAlpha.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbAlpha.PlaceHolderText = "0";
            this.tbAlpha.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAlpha.Size = new System.Drawing.Size(45, 31);
            this.tbAlpha.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbAlpha.TabIndex = 2156;
            // 
            // tbBeta
            // 
            this.tbBeta._Customizable = true;
            this.tbBeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbBeta.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbBeta.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbBeta.BorderRadius = 0;
            this.tbBeta.BorderSize = 1;
            this.tbBeta.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbBeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbBeta.Location = new System.Drawing.Point(208, 82);
            this.tbBeta.MultiLine = false;
            this.tbBeta.Name = "tbBeta";
            this.tbBeta.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbBeta.PasswordChar = false;
            this.tbBeta.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbBeta.PlaceHolderText = "100";
            this.tbBeta.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbBeta.Size = new System.Drawing.Size(45, 31);
            this.tbBeta.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbBeta.TabIndex = 2157;
            // 
            // FormVision_Histogram
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Histogram";
            this.ClientSize = new System.Drawing.Size(898, 318);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_Histogram";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Histogram";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.btnRun, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbHeight;
        private RJCodeUI_M1.RJControls.RJTextBox tbTilesGridSize;
        private RJCodeUI_M1.RJControls.RJTextBox tbClipLimit;
        private System.Windows.Forms.Label lbWidth;
        private ImageBox ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private ImageBox ibDestination;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJButton btnRun;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJComboBox cbType;
        private RJCodeUI_M1.RJControls.RJTextBox tbBeta;
        private RJCodeUI_M1.RJControls.RJTextBox tbAlpha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
