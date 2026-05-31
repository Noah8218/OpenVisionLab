using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVision_Line
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
            this.hostedComponent2 = new System.Windows.Controls.WpfPropertyGrid.PropertyGrid();
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.metroTabControl2 = new MetroFramework.Controls.MetroTabControl();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new Cyotek.Windows.Forms.ImageBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnResult = new RJCodeUI_M1.RJControls.RJButton();
            this.btnRun = new RJCodeUI_M1.RJControls.RJButton();
            this.rdoLeftEdgePara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rdoRightEdgePara = new RJCodeUI_M1.RJControls.RJRadioButton();
            this.rjButton1 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnVerLineRun = new RJCodeUI_M1.RJControls.RJButton();
            this.pnParameter = new RJCodeUI_M1.RJControls.RJPanel();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).BeginInit();
            this.SuspendLayout();            
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.pnParameter);
            this.pnlClientArea.Controls.Add(this.btnVerLineRun);
            this.pnlClientArea.Controls.Add(this.rjButton1);
            this.pnlClientArea.Controls.Add(this.btnResult);
            this.pnlClientArea.Controls.Add(this.rdoLeftEdgePara);
            this.pnlClientArea.Controls.Add(this.btnRun);
            this.pnlClientArea.Controls.Add(this.rdoRightEdgePara);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(601, 570);
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
            this.ibSource.Size = new System.Drawing.Size(270, 243);
            this.ibSource.TabIndex = 2149;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox3.Controls.Add(this.metroTabControl2);
            this.groupBox3.Controls.Add(this.cbLayerList);
            this.groupBox3.Controls.Add(this.ibSource);
            this.groupBox3.Location = new System.Drawing.Point(5, 45);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(284, 298);
            this.groupBox3.TabIndex = 2154;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Source Image";
            // 
            // metroTabControl2
            // 
            this.metroTabControl2.Location = new System.Drawing.Point(70, 133);
            this.metroTabControl2.Name = "metroTabControl2";
            this.metroTabControl2.Size = new System.Drawing.Size(8, 8);
            this.metroTabControl2.TabIndex = 2159;
            this.metroTabControl2.UseSelectable = true;
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
            this.cbLayerList.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList.Location = new System.Drawing.Point(6, 265);
            this.cbLayerList.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList.Name = "cbLayerList";
            this.cbLayerList.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList.SelectedIndex = -1;
            this.cbLayerList.Size = new System.Drawing.Size(270, 30);
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
            this.groupBox4.Location = new System.Drawing.Point(295, 45);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(284, 298);
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
            this.cbLayerList2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(146)))), ((int)(((byte)(246)))));
            this.cbLayerList2.Location = new System.Drawing.Point(6, 265);
            this.cbLayerList2.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbLayerList2.Name = "cbLayerList2";
            this.cbLayerList2.Padding = new System.Windows.Forms.Padding(2);
            this.cbLayerList2.SelectedIndex = -1;
            this.cbLayerList2.Size = new System.Drawing.Size(221, 30);
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
            this.btnNewPanel_Desty.Location = new System.Drawing.Point(233, 265);
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
            this.ibDestination.Size = new System.Drawing.Size(270, 243);
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
            // btnResult
            // 
            this.btnResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnResult.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnResult.BorderRadius = 15;
            this.btnResult.BorderSize = 3;
            this.btnResult.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResult.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnResult.FlatAppearance.BorderSize = 3;
            this.btnResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnResult.IconChar = FontAwesome.Sharp.IconChar.ClipboardList;
            this.btnResult.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnResult.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnResult.IconSize = 30;
            this.btnResult.Location = new System.Drawing.Point(441, 306);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(138, 62);
            this.btnResult.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnResult.TabIndex = 2156;
            this.btnResult.Text = "RESULT";
            this.btnResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnResult.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnResult.UseVisualStyleBackColor = false;
            this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
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
            this.btnRun.Location = new System.Drawing.Point(441, 431);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(137, 62);
            this.btnRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnRun.TabIndex = 2153;
            this.btnRun.Text = "Intersection\r\n(Fit Line)\r\n";
            this.btnRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // rdoLeftEdgePara
            // 
            this.rdoLeftEdgePara.AutoSize = true;
            this.rdoLeftEdgePara.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rdoLeftEdgePara.Checked = true;
            this.rdoLeftEdgePara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoLeftEdgePara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoLeftEdgePara.Customizable = true;
            this.rdoLeftEdgePara.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoLeftEdgePara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoLeftEdgePara.Location = new System.Drawing.Point(11, 308);
            this.rdoLeftEdgePara.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoLeftEdgePara.Name = "rdoLeftEdgePara";
            this.rdoLeftEdgePara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoLeftEdgePara.Size = new System.Drawing.Size(86, 21);
            this.rdoLeftEdgePara.TabIndex = 2158;
            this.rdoLeftEdgePara.TabStop = true;
            this.rdoLeftEdgePara.Text = "Edge(L)";
            this.rdoLeftEdgePara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoLeftEdgePara.UseVisualStyleBackColor = false;
            this.rdoLeftEdgePara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
            // 
            // rdoRightEdgePara
            // 
            this.rdoRightEdgePara.AutoSize = true;
            this.rdoRightEdgePara.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rdoRightEdgePara.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.rdoRightEdgePara.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoRightEdgePara.Customizable = true;
            this.rdoRightEdgePara.Font = new System.Drawing.Font("Verdana", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoRightEdgePara.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rdoRightEdgePara.Location = new System.Drawing.Point(103, 308);
            this.rdoRightEdgePara.MinimumSize = new System.Drawing.Size(0, 21);
            this.rdoRightEdgePara.Name = "rdoRightEdgePara";
            this.rdoRightEdgePara.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rdoRightEdgePara.Size = new System.Drawing.Size(87, 21);
            this.rdoRightEdgePara.TabIndex = 2159;
            this.rdoRightEdgePara.Text = "Edge(R)";
            this.rdoRightEdgePara.UnCheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(110)))), ((int)(((byte)(134)))));
            this.rdoRightEdgePara.UseVisualStyleBackColor = false;
            this.rdoRightEdgePara.CheckedChanged += new System.EventHandler(this.OnPara_CheckedChanged);
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
            this.rjButton1.Location = new System.Drawing.Point(441, 369);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(137, 62);
            this.rjButton1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.rjButton1.TabIndex = 2163;
            this.rjButton1.Text = "Edge";
            this.rjButton1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rjButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rjButton1.UseVisualStyleBackColor = false;
            this.rjButton1.Click += new System.EventHandler(this.btnEdgeRun_Click);
            // 
            // btnVerLineRun
            // 
            this.btnVerLineRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnVerLineRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnVerLineRun.BorderRadius = 15;
            this.btnVerLineRun.BorderSize = 3;
            this.btnVerLineRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerLineRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnVerLineRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnVerLineRun.FlatAppearance.BorderSize = 3;
            this.btnVerLineRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnVerLineRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnVerLineRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerLineRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerLineRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnVerLineRun.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.btnVerLineRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnVerLineRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnVerLineRun.IconSize = 24;
            this.btnVerLineRun.Location = new System.Drawing.Point(441, 493);
            this.btnVerLineRun.Name = "btnVerLineRun";
            this.btnVerLineRun.Size = new System.Drawing.Size(137, 62);
            this.btnVerLineRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnVerLineRun.TabIndex = 2164;
            this.btnVerLineRun.Text = "Intersection\r\n(ver Line)\r\n";
            this.btnVerLineRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVerLineRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnVerLineRun.UseVisualStyleBackColor = false;
            this.btnVerLineRun.Click += new System.EventHandler(this.btnVerLineRun_Click);
            // 
            // pnParameter
            // 
            this.pnParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.pnParameter.BorderRadius = 5;
            this.pnParameter.Customizable = false;
            this.pnParameter.Location = new System.Drawing.Point(11, 335);
            this.pnParameter.Name = "pnParameter";
            this.pnParameter.Size = new System.Drawing.Size(424, 220);
            this.pnParameter.TabIndex = 2165;
            // 
            // FormVision_Line
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Line";
            this.ClientSize = new System.Drawing.Size(603, 612);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_Line";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Resizable = false;
            this.Text = "Line";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPanel_Desty)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private ImageBox ibSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private ImageBox ibDestination;
        private RJCodeUI_M1.RJControls.RJButton btnRun;
        private System.Windows.Forms.GroupBox groupBox3;
        private RJCodeUI_M1.RJControls.RJMenuIcon btnNewPanel_Desty;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList2;
        private RJCodeUI_M1.RJControls.RJComboBox cbLayerList;
        private RJCodeUI_M1.RJControls.RJButton btnResult;
        private MetroFramework.Controls.MetroTabControl metroTabControl2;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoLeftEdgePara;
        private RJCodeUI_M1.RJControls.RJRadioButton rdoRightEdgePara;
        private RJCodeUI_M1.RJControls.RJButton rjButton1;
        private RJCodeUI_M1.RJControls.RJButton btnVerLineRun;
        public System.Windows.Controls.WpfPropertyGrid.PropertyGrid hostedComponent1;
        private RJCodeUI_M1.RJControls.RJPanel pnParameter;
        public System.Windows.Controls.WpfPropertyGrid.PropertyGrid hostedComponent2;
    }
}