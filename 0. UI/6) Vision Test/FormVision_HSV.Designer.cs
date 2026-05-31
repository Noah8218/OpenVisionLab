using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVision_HSV
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
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new Cyotek.Windows.Forms.ImageBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.trbSatMin = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbSatMax = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbHueMax = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbHueMin = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbValMax = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.trbValMin = new RJCodeUI_M1.RJControls.RJTrackBar();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbSatMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbSatMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHueMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHueMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbValMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbValMin)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.rjLabel5);
            this.pnlClientArea.Controls.Add(this.trbValMax);
            this.pnlClientArea.Controls.Add(this.rjLabel6);
            this.pnlClientArea.Controls.Add(this.trbValMin);
            this.pnlClientArea.Controls.Add(this.rjLabel2);
            this.pnlClientArea.Controls.Add(this.trbHueMax);
            this.pnlClientArea.Controls.Add(this.rjLabel3);
            this.pnlClientArea.Controls.Add(this.trbHueMin);
            this.pnlClientArea.Controls.Add(this.rjLabel1);
            this.pnlClientArea.Controls.Add(this.trbSatMax);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(584, 708);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.groupBox3.Location = new System.Drawing.Point(6, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(284, 268);
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
            this.groupBox4.Location = new System.Drawing.Point(296, 44);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(284, 268);
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
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // trbSatMin
            // 
            this.trbSatMin.AutoSize = false;
            this.trbSatMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbSatMin.ChannelColor = System.Drawing.Color.LightGray;
            this.trbSatMin.Customizable = true;
            this.trbSatMin.LargeChange = 1;
            this.trbSatMin.Location = new System.Drawing.Point(6, 327);
            this.trbSatMin.Maximum = 255;
            this.trbSatMin.Minimum = 1;
            this.trbSatMin.Name = "trbSatMin";
            this.trbSatMin.ShowValue = true;
            this.trbSatMin.Size = new System.Drawing.Size(380, 46);
            this.trbSatMin.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbSatMin.TabIndex = 2156;
            this.trbSatMin.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbSatMin.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbSatMin.Value = 1;
            this.trbSatMin.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(3, 308);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(57, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 2157;
            this.rjLabel4.Text = "Sat Min";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(2, 334);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(61, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2159;
            this.rjLabel1.Text = "Sat Max";
            // 
            // trbSatMax
            // 
            this.trbSatMax.AutoSize = false;
            this.trbSatMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbSatMax.ChannelColor = System.Drawing.Color.LightGray;
            this.trbSatMax.Customizable = true;
            this.trbSatMax.LargeChange = 1;
            this.trbSatMax.Location = new System.Drawing.Point(5, 353);
            this.trbSatMax.Maximum = 255;
            this.trbSatMax.Minimum = 1;
            this.trbSatMax.Name = "trbSatMax";
            this.trbSatMax.ShowValue = true;
            this.trbSatMax.Size = new System.Drawing.Size(380, 46);
            this.trbSatMax.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbSatMax.TabIndex = 2158;
            this.trbSatMax.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbSatMax.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbSatMax.Value = 1;
            this.trbSatMax.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // rjLabel2
            // 
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(2, 462);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(63, 16);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel2.TabIndex = 2163;
            this.rjLabel2.Text = "Hue Max";
            // 
            // trbHueMax
            // 
            this.trbHueMax.AutoSize = false;
            this.trbHueMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbHueMax.ChannelColor = System.Drawing.Color.LightGray;
            this.trbHueMax.Customizable = true;
            this.trbHueMax.LargeChange = 1;
            this.trbHueMax.Location = new System.Drawing.Point(5, 481);
            this.trbHueMax.Maximum = 255;
            this.trbHueMax.Minimum = 1;
            this.trbHueMax.Name = "trbHueMax";
            this.trbHueMax.ShowValue = true;
            this.trbHueMax.Size = new System.Drawing.Size(380, 46);
            this.trbHueMax.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbHueMax.TabIndex = 2162;
            this.trbHueMax.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbHueMax.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbHueMax.Value = 1;
            this.trbHueMax.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(3, 401);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(59, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 2161;
            this.rjLabel3.Text = "Hue Min";
            // 
            // trbHueMin
            // 
            this.trbHueMin.AutoSize = false;
            this.trbHueMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbHueMin.ChannelColor = System.Drawing.Color.LightGray;
            this.trbHueMin.Customizable = true;
            this.trbHueMin.LargeChange = 1;
            this.trbHueMin.Location = new System.Drawing.Point(6, 420);
            this.trbHueMin.Maximum = 255;
            this.trbHueMin.Minimum = 1;
            this.trbHueMin.Name = "trbHueMin";
            this.trbHueMin.ShowValue = true;
            this.trbHueMin.Size = new System.Drawing.Size(380, 46);
            this.trbHueMin.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbHueMin.TabIndex = 2160;
            this.trbHueMin.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbHueMin.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbHueMin.Value = 1;
            this.trbHueMin.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // rjLabel5
            // 
            this.rjLabel5.AutoSize = true;
            this.rjLabel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel5.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel5.LinkLabel = false;
            this.rjLabel5.Location = new System.Drawing.Point(3, 590);
            this.rjLabel5.Name = "rjLabel5";
            this.rjLabel5.Size = new System.Drawing.Size(57, 16);
            this.rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel5.TabIndex = 2167;
            this.rjLabel5.Text = "Val Max";
            // 
            // trbValMax
            // 
            this.trbValMax.AutoSize = false;
            this.trbValMax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbValMax.ChannelColor = System.Drawing.Color.LightGray;
            this.trbValMax.Customizable = true;
            this.trbValMax.LargeChange = 1;
            this.trbValMax.Location = new System.Drawing.Point(6, 609);
            this.trbValMax.Maximum = 255;
            this.trbValMax.Minimum = 1;
            this.trbValMax.Name = "trbValMax";
            this.trbValMax.ShowValue = true;
            this.trbValMax.Size = new System.Drawing.Size(380, 46);
            this.trbValMax.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbValMax.TabIndex = 2166;
            this.trbValMax.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbValMax.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbValMax.Value = 1;
            this.trbValMax.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(4, 529);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(53, 16);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel6.TabIndex = 2165;
            this.rjLabel6.Text = "Val Min";
            // 
            // trbValMin
            // 
            this.trbValMin.AutoSize = false;
            this.trbValMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.trbValMin.ChannelColor = System.Drawing.Color.LightGray;
            this.trbValMin.Customizable = true;
            this.trbValMin.LargeChange = 1;
            this.trbValMin.Location = new System.Drawing.Point(7, 548);
            this.trbValMin.Maximum = 255;
            this.trbValMin.Minimum = 1;
            this.trbValMin.Name = "trbValMin";
            this.trbValMin.ShowValue = true;
            this.trbValMin.Size = new System.Drawing.Size(380, 46);
            this.trbValMin.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.trbValMin.TabIndex = 2164;
            this.trbValMin.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(160)))), ((int)(((byte)(162)))));
            this.trbValMin.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trbValMin.Value = 1;
            this.trbValMin.Scroll += new System.EventHandler(this.trbHsv_Scroll);
            // 
            // FormVision_HSV
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "HSV";
            this.ClientSize = new System.Drawing.Size(586, 750);
            this.Controls.Add(this.rjLabel4);
            this.Controls.Add(this.trbSatMin);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_HSV";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Resizable = false;
            this.Text = "HSV";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.trbSatMin, 0);
            this.Controls.SetChildIndex(this.rjLabel4, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbSatMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbSatMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHueMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHueMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbValMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbValMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private ImageBox ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private ImageBox ibDestination;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJTrackBar trbSatMin;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJTrackBar trbSatMax;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJTrackBar trbHueMax;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJTrackBar trbHueMin;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        private RJCodeUI_M1.RJControls.RJTrackBar trbValMax;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJTrackBar trbValMin;
    }
}