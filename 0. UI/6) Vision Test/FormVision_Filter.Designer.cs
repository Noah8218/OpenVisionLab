using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormVision_Filter
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
            this.rjLabel7 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbFilterBorderType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.cbFilterType = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbSigmaSpace = new System.Windows.Forms.Label();
            this.tbSigmaSpace = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lbSigmaColor = new System.Windows.Forms.Label();
            this.tbSigmaColor = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lbDiameter = new System.Windows.Forms.Label();
            this.tbDiameter = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lbKernel = new System.Windows.Forms.Label();
            this.lbHeight = new System.Windows.Forms.Label();
            this.tbKernalFilter = new RJCodeUI_M1.RJControls.RJTextBox();
            this.tbFilterH = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lbWidth = new System.Windows.Forms.Label();
            this.tbFilterW = new RJCodeUI_M1.RJControls.RJTextBox();
            this.ibSource = new Cyotek.Windows.Forms.ImageBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLayerList = new RJCodeUI_M1.RJControls.RJComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbLayerList2 = new RJCodeUI_M1.RJControls.RJComboBox();
            this.btnNewPanel_Desty = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.ibDestination = new Cyotek.Windows.Forms.ImageBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnFilterRun = new RJCodeUI_M1.RJControls.RJButton();
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
            this.groupBox1.Controls.Add(this.rjLabel7);
            this.groupBox1.Controls.Add(this.cbFilterBorderType);
            this.groupBox1.Controls.Add(this.rjLabel1);
            this.groupBox1.Controls.Add(this.cbFilterType);
            this.groupBox1.Location = new System.Drawing.Point(592, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 73);
            this.groupBox1.TabIndex = 2147;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operations";
            // 
            // rjLabel7
            // 
            this.rjLabel7.AutoSize = true;
            this.rjLabel7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel7.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel7.LinkLabel = false;
            this.rjLabel7.Location = new System.Drawing.Point(148, 16);
            this.rjLabel7.Name = "rjLabel7";
            this.rjLabel7.Size = new System.Drawing.Size(124, 16);
            this.rjLabel7.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel7.TabIndex = 2161;
            this.rjLabel7.Text = "Filter Border Type";
            // 
            // cbFilterBorderType
            // 
            this.cbFilterBorderType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbFilterBorderType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbFilterBorderType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbFilterBorderType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbFilterBorderType.BorderRadius = 0;
            this.cbFilterBorderType.BorderSize = 2;
            this.cbFilterBorderType.Customizable = false;
            this.cbFilterBorderType.DataSource = null;
            this.cbFilterBorderType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbFilterBorderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterBorderType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbFilterBorderType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbFilterBorderType.ForeColor = System.Drawing.Color.DimGray;
            this.cbFilterBorderType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbFilterBorderType.Location = new System.Drawing.Point(151, 34);
            this.cbFilterBorderType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbFilterBorderType.Name = "cbFilterBorderType";
            this.cbFilterBorderType.Padding = new System.Windows.Forms.Padding(2);
            this.cbFilterBorderType.SelectedIndex = -1;
            this.cbFilterBorderType.Size = new System.Drawing.Size(139, 30);
            this.cbFilterBorderType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbFilterBorderType.TabIndex = 2160;
            this.cbFilterBorderType.Texts = "";
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
            this.rjLabel1.Size = new System.Drawing.Size(77, 16);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel1.TabIndex = 2159;
            this.rjLabel1.Text = "Filter Type";
            // 
            // cbFilterType
            // 
            this.cbFilterType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.cbFilterType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.cbFilterType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cbFilterType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbFilterType.BorderRadius = 0;
            this.cbFilterType.BorderSize = 2;
            this.cbFilterType.Customizable = false;
            this.cbFilterType.DataSource = null;
            this.cbFilterType.DropDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.cbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterType.DropDownTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.cbFilterType.Font = new System.Drawing.Font("Verdana", 15F);
            this.cbFilterType.ForeColor = System.Drawing.Color.DimGray;
            this.cbFilterType.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.cbFilterType.Location = new System.Drawing.Point(6, 34);
            this.cbFilterType.MinimumSize = new System.Drawing.Size(100, 30);
            this.cbFilterType.Name = "cbFilterType";
            this.cbFilterType.Padding = new System.Windows.Forms.Padding(2);
            this.cbFilterType.SelectedIndex = -1;
            this.cbFilterType.Size = new System.Drawing.Size(139, 30);
            this.cbFilterType.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.cbFilterType.TabIndex = 2158;
            this.cbFilterType.Texts = "";
            this.cbFilterType.OnSelectedIndexChanged += new System.EventHandler(this.cbFilterType_OnSelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox2.Controls.Add(this.lbSigmaSpace);
            this.groupBox2.Controls.Add(this.tbSigmaSpace);
            this.groupBox2.Controls.Add(this.lbSigmaColor);
            this.groupBox2.Controls.Add(this.tbSigmaColor);
            this.groupBox2.Controls.Add(this.lbDiameter);
            this.groupBox2.Controls.Add(this.tbDiameter);
            this.groupBox2.Controls.Add(this.lbKernel);
            this.groupBox2.Controls.Add(this.lbHeight);
            this.groupBox2.Controls.Add(this.tbKernalFilter);
            this.groupBox2.Controls.Add(this.tbFilterH);
            this.groupBox2.Controls.Add(this.lbWidth);
            this.groupBox2.Controls.Add(this.tbFilterW);
            this.groupBox2.Location = new System.Drawing.Point(592, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 143);
            this.groupBox2.TabIndex = 2148;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Kernel";
            // 
            // lbSigmaSpace
            // 
            this.lbSigmaSpace.AutoSize = true;
            this.lbSigmaSpace.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSigmaSpace.ForeColor = System.Drawing.Color.Black;
            this.lbSigmaSpace.Location = new System.Drawing.Point(3, 117);
            this.lbSigmaSpace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSigmaSpace.Name = "lbSigmaSpace";
            this.lbSigmaSpace.Size = new System.Drawing.Size(112, 18);
            this.lbSigmaSpace.TabIndex = 2172;
            this.lbSigmaSpace.Text = "Sigma Space";
            // 
            // tbSigmaSpace
            // 
            this.tbSigmaSpace._Customizable = true;
            this.tbSigmaSpace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbSigmaSpace.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSigmaSpace.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbSigmaSpace.BorderRadius = 0;
            this.tbSigmaSpace.BorderSize = 1;
            this.tbSigmaSpace.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbSigmaSpace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSigmaSpace.Location = new System.Drawing.Point(122, 110);
            this.tbSigmaSpace.MultiLine = false;
            this.tbSigmaSpace.Name = "tbSigmaSpace";
            this.tbSigmaSpace.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbSigmaSpace.PasswordChar = false;
            this.tbSigmaSpace.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbSigmaSpace.PlaceHolderText = "3";
            this.tbSigmaSpace.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSigmaSpace.Size = new System.Drawing.Size(45, 31);
            this.tbSigmaSpace.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbSigmaSpace.TabIndex = 2171;
            // 
            // lbSigmaColor
            // 
            this.lbSigmaColor.AutoSize = true;
            this.lbSigmaColor.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSigmaColor.ForeColor = System.Drawing.Color.Black;
            this.lbSigmaColor.Location = new System.Drawing.Point(3, 84);
            this.lbSigmaColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSigmaColor.Name = "lbSigmaColor";
            this.lbSigmaColor.Size = new System.Drawing.Size(106, 18);
            this.lbSigmaColor.TabIndex = 2170;
            this.lbSigmaColor.Text = "Sigma Color";
            // 
            // tbSigmaColor
            // 
            this.tbSigmaColor._Customizable = true;
            this.tbSigmaColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbSigmaColor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSigmaColor.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbSigmaColor.BorderRadius = 0;
            this.tbSigmaColor.BorderSize = 1;
            this.tbSigmaColor.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbSigmaColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbSigmaColor.Location = new System.Drawing.Point(122, 77);
            this.tbSigmaColor.MultiLine = false;
            this.tbSigmaColor.Name = "tbSigmaColor";
            this.tbSigmaColor.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbSigmaColor.PasswordChar = false;
            this.tbSigmaColor.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbSigmaColor.PlaceHolderText = "3";
            this.tbSigmaColor.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSigmaColor.Size = new System.Drawing.Size(45, 31);
            this.tbSigmaColor.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbSigmaColor.TabIndex = 2169;
            // 
            // lbDiameter
            // 
            this.lbDiameter.AutoSize = true;
            this.lbDiameter.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiameter.ForeColor = System.Drawing.Color.Black;
            this.lbDiameter.Location = new System.Drawing.Point(119, 54);
            this.lbDiameter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbDiameter.Name = "lbDiameter";
            this.lbDiameter.Size = new System.Drawing.Size(83, 18);
            this.lbDiameter.TabIndex = 2168;
            this.lbDiameter.Text = "Diameter";
            // 
            // tbDiameter
            // 
            this.tbDiameter._Customizable = true;
            this.tbDiameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbDiameter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbDiameter.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbDiameter.BorderRadius = 0;
            this.tbDiameter.BorderSize = 1;
            this.tbDiameter.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbDiameter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbDiameter.Location = new System.Drawing.Point(209, 48);
            this.tbDiameter.MultiLine = false;
            this.tbDiameter.Name = "tbDiameter";
            this.tbDiameter.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbDiameter.PasswordChar = false;
            this.tbDiameter.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbDiameter.PlaceHolderText = "3";
            this.tbDiameter.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbDiameter.Size = new System.Drawing.Size(45, 31);
            this.tbDiameter.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbDiameter.TabIndex = 2167;
            // 
            // lbKernel
            // 
            this.lbKernel.AutoSize = true;
            this.lbKernel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbKernel.ForeColor = System.Drawing.Color.Black;
            this.lbKernel.Location = new System.Drawing.Point(3, 54);
            this.lbKernel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbKernel.Name = "lbKernel";
            this.lbKernel.Size = new System.Drawing.Size(59, 18);
            this.lbKernel.TabIndex = 2166;
            this.lbKernel.Text = "Kernel";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeight.ForeColor = System.Drawing.Color.Black;
            this.lbHeight.Location = new System.Drawing.Point(119, 17);
            this.lbHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(62, 18);
            this.lbHeight.TabIndex = 2153;
            this.lbHeight.Text = "Height";
            // 
            // tbKernalFilter
            // 
            this.tbKernalFilter._Customizable = true;
            this.tbKernalFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbKernalFilter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbKernalFilter.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbKernalFilter.BorderRadius = 0;
            this.tbKernalFilter.BorderSize = 1;
            this.tbKernalFilter.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbKernalFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbKernalFilter.Location = new System.Drawing.Point(67, 48);
            this.tbKernalFilter.MultiLine = false;
            this.tbKernalFilter.Name = "tbKernalFilter";
            this.tbKernalFilter.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbKernalFilter.PasswordChar = false;
            this.tbKernalFilter.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbKernalFilter.PlaceHolderText = "3";
            this.tbKernalFilter.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbKernalFilter.Size = new System.Drawing.Size(45, 31);
            this.tbKernalFilter.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbKernalFilter.TabIndex = 2165;
            // 
            // tbFilterH
            // 
            this.tbFilterH._Customizable = true;
            this.tbFilterH.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbFilterH.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbFilterH.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbFilterH.BorderRadius = 0;
            this.tbFilterH.BorderSize = 1;
            this.tbFilterH.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbFilterH.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbFilterH.Location = new System.Drawing.Point(209, 11);
            this.tbFilterH.MultiLine = false;
            this.tbFilterH.Name = "tbFilterH";
            this.tbFilterH.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbFilterH.PasswordChar = false;
            this.tbFilterH.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbFilterH.PlaceHolderText = "3";
            this.tbFilterH.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbFilterH.Size = new System.Drawing.Size(45, 31);
            this.tbFilterH.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbFilterH.TabIndex = 2152;
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWidth.ForeColor = System.Drawing.Color.Black;
            this.lbWidth.Location = new System.Drawing.Point(3, 17);
            this.lbWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(57, 18);
            this.lbWidth.TabIndex = 2149;
            this.lbWidth.Text = "Width";
            // 
            // tbFilterW
            // 
            this.tbFilterW._Customizable = true;
            this.tbFilterW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.tbFilterW.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbFilterW.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbFilterW.BorderRadius = 0;
            this.tbFilterW.BorderSize = 1;
            this.tbFilterW.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbFilterW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbFilterW.Location = new System.Drawing.Point(67, 11);
            this.tbFilterW.MultiLine = false;
            this.tbFilterW.Name = "tbFilterW";
            this.tbFilterW.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbFilterW.PasswordChar = false;
            this.tbFilterW.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbFilterW.PlaceHolderText = "3";
            this.tbFilterW.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbFilterW.Size = new System.Drawing.Size(45, 31);
            this.tbFilterW.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.tbFilterW.TabIndex = 2150;
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
            // btnFilterRun
            // 
            this.btnFilterRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnFilterRun.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnFilterRun.BorderRadius = 15;
            this.btnFilterRun.BorderSize = 3;
            this.btnFilterRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterRun.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnFilterRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnFilterRun.FlatAppearance.BorderSize = 3;
            this.btnFilterRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnFilterRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnFilterRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnFilterRun.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.btnFilterRun.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.btnFilterRun.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFilterRun.IconSize = 24;
            this.btnFilterRun.Location = new System.Drawing.Point(774, 263);
            this.btnFilterRun.Name = "btnFilterRun";
            this.btnFilterRun.Size = new System.Drawing.Size(117, 50);
            this.btnFilterRun.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnFilterRun.TabIndex = 2163;
            this.btnFilterRun.Text = "EXCUTE";
            this.btnFilterRun.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFilterRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFilterRun.UseVisualStyleBackColor = false;
            this.btnFilterRun.Click += new System.EventHandler(this.btnFilterRun_Click);
            // 
            // FormVision_Filter
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Filter";
            this.ClientSize = new System.Drawing.Size(898, 318);
            this.Controls.Add(this.btnFilterRun);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.Name = "FormVision_Filter";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Filter";
            this.Load += new System.EventHandler(this.FormSettings_Camera_Load);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox4, 0);
            this.Controls.SetChildIndex(this.btnFilterRun, 0);
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
        private RJCodeUI_M1.RJControls.RJTextBox tbFilterH;
        private RJCodeUI_M1.RJControls.RJTextBox tbFilterW;
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
        private RJCodeUI_M1.RJControls.RJButton btnFilterRun;
        private System.Windows.Forms.Label lbKernel;
        private RJCodeUI_M1.RJControls.RJTextBox tbKernalFilter;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel7;
        private RJCodeUI_M1.RJControls.RJComboBox cbFilterBorderType;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJComboBox cbFilterType;
        private System.Windows.Forms.Label lbSigmaSpace;
        private RJCodeUI_M1.RJControls.RJTextBox tbSigmaSpace;
        private System.Windows.Forms.Label lbSigmaColor;
        private RJCodeUI_M1.RJControls.RJTextBox tbSigmaColor;
        private System.Windows.Forms.Label lbDiameter;
        private RJCodeUI_M1.RJControls.RJTextBox tbDiameter;
    }
}
