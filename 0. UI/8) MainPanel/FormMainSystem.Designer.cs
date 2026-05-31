using Cyotek.Windows.Forms;

namespace KtemVisionSystem
{
    partial class FormMainSystem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainSystem));
            this.timePixelData = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rjLabel11 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel10 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel9 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel8 = new RJCodeUI_M1.RJControls.RJLabel();
            this.btnStatusCAM2 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnStatusCAM1 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnStatusIO_PC1 = new RJCodeUI_M1.RJControls.RJButton();
            this.btnStatusPLC_PC1 = new RJCodeUI_M1.RJControls.RJButton();
            this.pgbDriveD = new MetroFramework.Controls.MetroProgressBar();
            this.pgbDriveC = new MetroFramework.Controls.MetroProgressBar();
            this.lbDriveD = new RJCodeUI_M1.RJControls.RJLabel();
            this.lbDriveC = new RJCodeUI_M1.RJControls.RJLabel();
            this.timerDateTime = new System.Windows.Forms.Timer(this.components);
            this.timerConnection = new System.Windows.Forms.Timer(this.components);
            this.rjPanel1 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lbMenu = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel4 = new RJCodeUI_M1.RJControls.RJPanel();
            this.rjLabel2 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel5 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel25 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel7 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel1 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel6 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel3 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lblProfit = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel14 = new RJCodeUI_M1.RJControls.RJLabel();
            this.btnReset = new RJCodeUI_M1.RJControls.RJButton();
            this.rjPanel6 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lblNumberSales = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel16 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjPanel2 = new RJCodeUI_M1.RJControls.RJPanel();
            this.lblRevenue = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel15 = new RJCodeUI_M1.RJControls.RJLabel();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.rjPanel1.SuspendLayout();
            this.rjPanel4.SuspendLayout();
            this.rjPanel3.SuspendLayout();
            this.rjPanel6.SuspendLayout();
            this.rjPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePixelData
            // 
            this.timePixelData.Enabled = true;
            this.timePixelData.Interval = 10;
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
            this.timer1.Enabled = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.pgbDriveD);
            this.panel1.Controls.Add(this.pgbDriveC);
            this.panel1.Controls.Add(this.lbDriveD);
            this.panel1.Controls.Add(this.lbDriveC);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 441);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 126);
            this.panel1.TabIndex = 2141;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.groupBox1.Controls.Add(this.rjLabel11);
            this.groupBox1.Controls.Add(this.rjLabel10);
            this.groupBox1.Controls.Add(this.rjLabel9);
            this.groupBox1.Controls.Add(this.rjLabel8);
            this.groupBox1.Controls.Add(this.btnStatusCAM2);
            this.groupBox1.Controls.Add(this.btnStatusCAM1);
            this.groupBox1.Controls.Add(this.btnStatusIO_PC1);
            this.groupBox1.Controls.Add(this.btnStatusPLC_PC1);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(276, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 121);
            this.groupBox1.TabIndex = 2096;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // rjLabel11
            // 
            this.rjLabel11.AutoSize = true;
            this.rjLabel11.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel11.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel11.LinkLabel = false;
            this.rjLabel11.Location = new System.Drawing.Point(98, 76);
            this.rjLabel11.Name = "rjLabel11";
            this.rjLabel11.Size = new System.Drawing.Size(31, 16);
            this.rjLabel11.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel11.TabIndex = 2110;
            this.rjLabel11.Text = "PLC";
            // 
            // rjLabel10
            // 
            this.rjLabel10.AutoSize = true;
            this.rjLabel10.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel10.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel10.LinkLabel = false;
            this.rjLabel10.Location = new System.Drawing.Point(6, 76);
            this.rjLabel10.Name = "rjLabel10";
            this.rjLabel10.Size = new System.Drawing.Size(28, 16);
            this.rjLabel10.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel10.TabIndex = 2109;
            this.rjLabel10.Text = "I/O";
            // 
            // rjLabel9
            // 
            this.rjLabel9.AutoSize = true;
            this.rjLabel9.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel9.LinkLabel = false;
            this.rjLabel9.Location = new System.Drawing.Point(95, 32);
            this.rjLabel9.Name = "rjLabel9";
            this.rjLabel9.Size = new System.Drawing.Size(44, 16);
            this.rjLabel9.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel9.TabIndex = 2108;
            this.rjLabel9.Text = "CAM2";
            // 
            // rjLabel8
            // 
            this.rjLabel8.AutoSize = true;
            this.rjLabel8.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel8.LinkLabel = false;
            this.rjLabel8.Location = new System.Drawing.Point(6, 32);
            this.rjLabel8.Name = "rjLabel8";
            this.rjLabel8.Size = new System.Drawing.Size(44, 16);
            this.rjLabel8.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel8.TabIndex = 2107;
            this.rjLabel8.Text = "CAM1";
            // 
            // btnStatusCAM2
            // 
            this.btnStatusCAM2.BackColor = System.Drawing.Color.Red;
            this.btnStatusCAM2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM2.BorderRadius = 17;
            this.btnStatusCAM2.BorderSize = 1;
            this.btnStatusCAM2.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnStatusCAM2.FlatAppearance.BorderSize = 0;
            this.btnStatusCAM2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusCAM2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusCAM2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatusCAM2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatusCAM2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM2.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnStatusCAM2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnStatusCAM2.IconSize = 24;
            this.btnStatusCAM2.Location = new System.Drawing.Point(141, 20);
            this.btnStatusCAM2.Name = "btnStatusCAM2";
            this.btnStatusCAM2.Size = new System.Drawing.Size(36, 35);
            this.btnStatusCAM2.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnStatusCAM2.TabIndex = 2002;
            this.btnStatusCAM2.UseVisualStyleBackColor = false;
            // 
            // btnStatusCAM1
            // 
            this.btnStatusCAM1.BackColor = System.Drawing.Color.Red;
            this.btnStatusCAM1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM1.BorderRadius = 17;
            this.btnStatusCAM1.BorderSize = 1;
            this.btnStatusCAM1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnStatusCAM1.FlatAppearance.BorderSize = 0;
            this.btnStatusCAM1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusCAM1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusCAM1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatusCAM1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatusCAM1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnStatusCAM1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusCAM1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnStatusCAM1.IconSize = 24;
            this.btnStatusCAM1.Location = new System.Drawing.Point(53, 19);
            this.btnStatusCAM1.Name = "btnStatusCAM1";
            this.btnStatusCAM1.Size = new System.Drawing.Size(36, 35);
            this.btnStatusCAM1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnStatusCAM1.TabIndex = 1965;
            this.btnStatusCAM1.UseVisualStyleBackColor = false;
            // 
            // btnStatusIO_PC1
            // 
            this.btnStatusIO_PC1.BackColor = System.Drawing.Color.Red;
            this.btnStatusIO_PC1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusIO_PC1.BorderRadius = 17;
            this.btnStatusIO_PC1.BorderSize = 1;
            this.btnStatusIO_PC1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnStatusIO_PC1.FlatAppearance.BorderSize = 0;
            this.btnStatusIO_PC1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusIO_PC1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusIO_PC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatusIO_PC1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatusIO_PC1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusIO_PC1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnStatusIO_PC1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusIO_PC1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnStatusIO_PC1.IconSize = 24;
            this.btnStatusIO_PC1.Location = new System.Drawing.Point(53, 64);
            this.btnStatusIO_PC1.Name = "btnStatusIO_PC1";
            this.btnStatusIO_PC1.Size = new System.Drawing.Size(36, 35);
            this.btnStatusIO_PC1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnStatusIO_PC1.TabIndex = 2007;
            this.btnStatusIO_PC1.UseVisualStyleBackColor = false;
            // 
            // btnStatusPLC_PC1
            // 
            this.btnStatusPLC_PC1.BackColor = System.Drawing.Color.Red;
            this.btnStatusPLC_PC1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusPLC_PC1.BorderRadius = 17;
            this.btnStatusPLC_PC1.BorderSize = 1;
            this.btnStatusPLC_PC1.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnStatusPLC_PC1.FlatAppearance.BorderSize = 0;
            this.btnStatusPLC_PC1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusPLC_PC1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStatusPLC_PC1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatusPLC_PC1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatusPLC_PC1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusPLC_PC1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnStatusPLC_PC1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnStatusPLC_PC1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnStatusPLC_PC1.IconSize = 24;
            this.btnStatusPLC_PC1.Location = new System.Drawing.Point(141, 64);
            this.btnStatusPLC_PC1.Name = "btnStatusPLC_PC1";
            this.btnStatusPLC_PC1.Size = new System.Drawing.Size(36, 35);
            this.btnStatusPLC_PC1.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnStatusPLC_PC1.TabIndex = 2004;
            this.btnStatusPLC_PC1.UseVisualStyleBackColor = false;
            // 
            // pgbDriveD
            // 
            this.pgbDriveD.Location = new System.Drawing.Point(2, 81);
            this.pgbDriveD.Name = "pgbDriveD";
            this.pgbDriveD.Size = new System.Drawing.Size(266, 41);
            this.pgbDriveD.Style = MetroFramework.MetroColorStyle.Lime;
            this.pgbDriveD.TabIndex = 2080;
            this.pgbDriveD.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // pgbDriveC
            // 
            this.pgbDriveC.Location = new System.Drawing.Point(2, 19);
            this.pgbDriveC.Name = "pgbDriveC";
            this.pgbDriveC.Size = new System.Drawing.Size(266, 42);
            this.pgbDriveC.Style = MetroFramework.MetroColorStyle.Lime;
            this.pgbDriveC.TabIndex = 2079;
            this.pgbDriveC.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lbDriveD
            // 
            this.lbDriveD.AutoSize = true;
            this.lbDriveD.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbDriveD.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriveD.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbDriveD.LinkLabel = false;
            this.lbDriveD.Location = new System.Drawing.Point(2, 64);
            this.lbDriveD.Name = "lbDriveD";
            this.lbDriveD.Size = new System.Drawing.Size(187, 16);
            this.lbDriveD.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbDriveD.TabIndex = 2107;
            this.lbDriveD.Text = "(D:) : 00%    (000/000 GB)";
            // 
            // lbDriveC
            // 
            this.lbDriveC.AutoSize = true;
            this.lbDriveC.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbDriveC.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDriveC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbDriveC.LinkLabel = false;
            this.lbDriveC.Location = new System.Drawing.Point(2, 0);
            this.lbDriveC.Name = "lbDriveC";
            this.lbDriveC.Size = new System.Drawing.Size(187, 16);
            this.lbDriveC.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbDriveC.TabIndex = 2106;
            this.lbDriveC.Text = "(C:) : 00%    (000/000 GB)";
            // 
            // timerDateTime
            // 
            this.timerDateTime.Enabled = true;
            this.timerDateTime.Interval = 1000;
            this.timerDateTime.Tick += new System.EventHandler(this.timerDateTime_Tick);
            // 
            // timerConnection
            // 
            this.timerConnection.Enabled = true;
            this.timerConnection.Tick += new System.EventHandler(this.timerConnection_Tick);
            // 
            // rjPanel1
            // 
            this.rjPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel1.BorderRadius = 5;
            this.rjPanel1.Controls.Add(this.lbMenu);
            this.rjPanel1.Customizable = false;
            this.rjPanel1.Location = new System.Drawing.Point(0, 2);
            this.rjPanel1.Name = "rjPanel1";
            this.rjPanel1.Size = new System.Drawing.Size(480, 92);
            this.rjPanel1.TabIndex = 2143;
            // 
            // lbMenu
            // 
            this.lbMenu.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lbMenu.Font = new System.Drawing.Font("Consolas", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lbMenu.LinkLabel = false;
            this.lbMenu.Location = new System.Drawing.Point(12, 7);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(441, 79);
            this.lbMenu.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.lbMenu.TabIndex = 2126;
            this.lbMenu.Text = "Mode";
            this.lbMenu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjPanel4
            // 
            this.rjPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel4.BorderRadius = 5;
            this.rjPanel4.Controls.Add(this.rjLabel2);
            this.rjPanel4.Controls.Add(this.rjLabel3);
            this.rjPanel4.Controls.Add(this.rjLabel5);
            this.rjPanel4.Controls.Add(this.rjLabel25);
            this.rjPanel4.Controls.Add(this.rjLabel4);
            this.rjPanel4.Controls.Add(this.rjLabel7);
            this.rjPanel4.Controls.Add(this.rjLabel1);
            this.rjPanel4.Controls.Add(this.rjLabel6);
            this.rjPanel4.Customizable = false;
            this.rjPanel4.Location = new System.Drawing.Point(1, 91);
            this.rjPanel4.Name = "rjPanel4";
            this.rjPanel4.Size = new System.Drawing.Size(482, 119);
            this.rjPanel4.TabIndex = 2136;
            // 
            // rjLabel2
            // 
            this.rjLabel2.AutoSize = true;
            this.rjLabel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel2.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel2.LinkLabel = false;
            this.rjLabel2.Location = new System.Drawing.Point(123, 6);
            this.rjLabel2.Name = "rjLabel2";
            this.rjLabel2.Size = new System.Drawing.Size(63, 34);
            this.rjLabel2.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel2.TabIndex = 2120;
            this.rjLabel2.Text = "000";
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(259, 6);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(63, 34);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel3.TabIndex = 2122;
            this.rjLabel3.Text = "000";
            // 
            // rjLabel5
            // 
            this.rjLabel5.AutoSize = true;
            this.rjLabel5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel5.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel5.ForeColor = System.Drawing.Color.Red;
            this.rjLabel5.LinkLabel = false;
            this.rjLabel5.Location = new System.Drawing.Point(405, 6);
            this.rjLabel5.Name = "rjLabel5";
            this.rjLabel5.Size = new System.Drawing.Size(63, 34);
            this.rjLabel5.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel5.TabIndex = 2124;
            this.rjLabel5.Text = "000";
            // 
            // rjLabel25
            // 
            this.rjLabel25.AutoSize = true;
            this.rjLabel25.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel25.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel25.LinkLabel = false;
            this.rjLabel25.Location = new System.Drawing.Point(3, 6);
            this.rjLabel25.Name = "rjLabel25";
            this.rjLabel25.Size = new System.Drawing.Size(127, 34);
            this.rjLabel25.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel25.TabIndex = 2119;
            this.rjLabel25.Text = "TOTAL :";
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(188, 6);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(79, 34);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel4.TabIndex = 2121;
            this.rjLabel4.Text = "OK :";
            // 
            // rjLabel7
            // 
            this.rjLabel7.AutoSize = true;
            this.rjLabel7.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel7.Font = new System.Drawing.Font("Consolas", 35.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(113)))));
            this.rjLabel7.LinkLabel = false;
            this.rjLabel7.Location = new System.Drawing.Point(235, 54);
            this.rjLabel7.Name = "rjLabel7";
            this.rjLabel7.Size = new System.Drawing.Size(76, 55);
            this.rjLabel7.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel7.TabIndex = 2126;
            this.rjLabel7.Text = "OK";
            // 
            // rjLabel1
            // 
            this.rjLabel1.AutoSize = true;
            this.rjLabel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel1.Font = new System.Drawing.Font("Consolas", 35.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel1.LinkLabel = false;
            this.rjLabel1.Location = new System.Drawing.Point(10, 53);
            this.rjLabel1.Name = "rjLabel1";
            this.rjLabel1.Size = new System.Drawing.Size(232, 55);
            this.rjLabel1.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel1.TabIndex = 2125;
            this.rjLabel1.Text = "VISION :";
            // 
            // rjLabel6
            // 
            this.rjLabel6.AutoSize = true;
            this.rjLabel6.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel6.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rjLabel6.ForeColor = System.Drawing.Color.Red;
            this.rjLabel6.LinkLabel = false;
            this.rjLabel6.Location = new System.Drawing.Point(331, 6);
            this.rjLabel6.Name = "rjLabel6";
            this.rjLabel6.Size = new System.Drawing.Size(79, 34);
            this.rjLabel6.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel6.TabIndex = 2123;
            this.rjLabel6.Text = "NG :";
            // 
            // rjPanel3
            // 
            this.rjPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel3.BorderRadius = 5;
            this.rjPanel3.Controls.Add(this.lblProfit);
            this.rjPanel3.Controls.Add(this.rjLabel14);
            this.rjPanel3.Customizable = false;
            this.rjPanel3.Location = new System.Drawing.Point(1, 279);
            this.rjPanel3.Name = "rjPanel3";
            this.rjPanel3.Size = new System.Drawing.Size(361, 57);
            this.rjPanel3.TabIndex = 2138;
            // 
            // lblProfit
            // 
            this.lblProfit.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblProfit.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblProfit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.lblProfit.LinkLabel = false;
            this.lblProfit.Location = new System.Drawing.Point(87, 27);
            this.lblProfit.Name = "lblProfit";
            this.lblProfit.Size = new System.Drawing.Size(195, 23);
            this.lblProfit.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lblProfit.TabIndex = 1;
            this.lblProfit.Text = "-";
            this.lblProfit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel14
            // 
            this.rjLabel14.AutoSize = true;
            this.rjLabel14.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel14.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel14.LinkLabel = false;
            this.rjLabel14.Location = new System.Drawing.Point(136, 9);
            this.rjLabel14.Name = "rjLabel14";
            this.rjLabel14.Size = new System.Drawing.Size(96, 18);
            this.rjLabel14.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel14.TabIndex = 0;
            this.rjLabel14.Text = "검사 시작 시간";
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.BorderRadius = 15;
            this.btnReset.BorderSize = 3;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnReset.FlatAppearance.BorderSize = 3;
            this.btnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.IconChar = FontAwesome.Sharp.IconChar.Redo;
            this.btnReset.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnReset.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnReset.IconSize = 80;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReset.Location = new System.Drawing.Point(366, 216);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(117, 120);
            this.btnReset.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnReset.TabIndex = 2140;
            this.btnReset.Text = "Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // rjPanel6
            // 
            this.rjPanel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel6.BorderRadius = 5;
            this.rjPanel6.Controls.Add(this.lblNumberSales);
            this.rjPanel6.Controls.Add(this.rjLabel16);
            this.rjPanel6.Customizable = false;
            this.rjPanel6.Location = new System.Drawing.Point(1, 216);
            this.rjPanel6.Name = "rjPanel6";
            this.rjPanel6.Size = new System.Drawing.Size(181, 57);
            this.rjPanel6.TabIndex = 2137;
            // 
            // lblNumberSales
            // 
            this.lblNumberSales.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblNumberSales.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblNumberSales.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.lblNumberSales.LinkLabel = false;
            this.lblNumberSales.Location = new System.Drawing.Point(3, 26);
            this.lblNumberSales.Name = "lblNumberSales";
            this.lblNumberSales.Size = new System.Drawing.Size(175, 23);
            this.lblNumberSales.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lblNumberSales.TabIndex = 1;
            this.lblNumberSales.Text = "-";
            this.lblNumberSales.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel16
            // 
            this.rjLabel16.AutoSize = true;
            this.rjLabel16.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel16.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel16.LinkLabel = false;
            this.rjLabel16.Location = new System.Drawing.Point(54, 6);
            this.rjLabel16.Name = "rjLabel16";
            this.rjLabel16.Size = new System.Drawing.Size(68, 18);
            this.rjLabel16.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel16.TabIndex = 0;
            this.rjLabel16.Text = "LOT 번호";
            // 
            // rjPanel2
            // 
            this.rjPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.rjPanel2.BorderRadius = 5;
            this.rjPanel2.Controls.Add(this.lblRevenue);
            this.rjPanel2.Controls.Add(this.rjLabel15);
            this.rjPanel2.Customizable = false;
            this.rjPanel2.Location = new System.Drawing.Point(184, 216);
            this.rjPanel2.Name = "rjPanel2";
            this.rjPanel2.Size = new System.Drawing.Size(178, 57);
            this.rjPanel2.TabIndex = 2139;
            // 
            // lblRevenue
            // 
            this.lblRevenue.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblRevenue.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblRevenue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.lblRevenue.LinkLabel = false;
            this.lblRevenue.Location = new System.Drawing.Point(3, 26);
            this.lblRevenue.Name = "lblRevenue";
            this.lblRevenue.Size = new System.Drawing.Size(175, 23);
            this.lblRevenue.Style = RJCodeUI_M1.RJControls.LabelStyle.Title;
            this.lblRevenue.TabIndex = 1;
            this.lblRevenue.Text = "-";
            this.lblRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rjLabel15
            // 
            this.rjLabel15.AutoSize = true;
            this.rjLabel15.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel15.Font = new System.Drawing.Font("Verdana", 11F);
            this.rjLabel15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel15.LinkLabel = false;
            this.rjLabel15.Location = new System.Drawing.Point(61, 6);
            this.rjLabel15.Name = "rjLabel15";
            this.rjLabel15.Size = new System.Drawing.Size(63, 18);
            this.rjLabel15.Style = RJCodeUI_M1.RJControls.LabelStyle.Custom;
            this.rjLabel15.TabIndex = 0;
            this.rjLabel15.Text = "길이(m)";
            // 
            // FormMainSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(489, 567);
            this.Controls.Add(this.rjPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rjPanel4);
            this.Controls.Add(this.rjPanel3);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.rjPanel6);
            this.Controls.Add(this.rjPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMainSystem";
            this.Text = "System";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.Form_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.rjPanel1.ResumeLayout(false);
            this.rjPanel4.ResumeLayout(false);
            this.rjPanel4.PerformLayout();
            this.rjPanel3.ResumeLayout(false);
            this.rjPanel3.PerformLayout();
            this.rjPanel6.ResumeLayout(false);
            this.rjPanel6.PerformLayout();
            this.rjPanel2.ResumeLayout(false);
            this.rjPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timePixelData;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel4;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel2;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel3;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel5;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel25;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel4;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel7;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel6;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel3;
        private RJCodeUI_M1.RJControls.RJLabel lblProfit;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel14;
        private RJCodeUI_M1.RJControls.RJButton btnReset;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel6;
        private RJCodeUI_M1.RJControls.RJLabel lblNumberSales;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel16;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel2;
        private RJCodeUI_M1.RJControls.RJLabel lblRevenue;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel11;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel10;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel9;
        private RJCodeUI_M1.RJControls.RJLabel rjLabel8;
        private RJCodeUI_M1.RJControls.RJButton btnStatusCAM2;
        private RJCodeUI_M1.RJControls.RJButton btnStatusCAM1;
        private RJCodeUI_M1.RJControls.RJButton btnStatusIO_PC1;
        private RJCodeUI_M1.RJControls.RJButton btnStatusPLC_PC1;
        private MetroFramework.Controls.MetroProgressBar pgbDriveD;
        private MetroFramework.Controls.MetroProgressBar pgbDriveC;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveD;
        private RJCodeUI_M1.RJControls.RJLabel lbDriveC;
        private System.Windows.Forms.Timer timerDateTime;
        private System.Windows.Forms.Timer timerConnection;
        private RJCodeUI_M1.RJControls.RJPanel rjPanel1;
        private RJCodeUI_M1.RJControls.RJLabel lbMenu;
    }
}