using Cyotek.Windows.Forms;

namespace OpenVisionLab
{
    partial class FormPowerGraphConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPowerGraphConfig));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.nbLeftFontSize = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.nbThickness = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbAlaram = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbWarning = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbListMaxCount = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tbMinY = new System.Windows.Forms.TextBox();
            this.tbMaxY = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnGridColor = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pnAlarmLineColor = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.pnWarningLineColor = new System.Windows.Forms.Panel();
            this.pnTextColor = new System.Windows.Forms.Panel();
            this.pnCurrentDataColor = new System.Windows.Forms.Panel();
            this.pnBackgroundColor = new System.Windows.Forms.Panel();
            this.btnSaveParameter = new RJCodeUI_M1.RJControls.RJButton();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbDisplayAlaram = new System.Windows.Forms.CheckBox();
            this.cbDisplayWarning = new System.Windows.Forms.CheckBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbUnit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.rdoLine = new System.Windows.Forms.RadioButton();
            this.rdoDot = new System.Windows.Forms.RadioButton();
            this.pnlClientArea.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbLeftFontSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbThickness)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.groupBox1);
            this.pnlClientArea.Controls.Add(this.btnSaveParameter);
            this.pnlClientArea.Controls.Add(this.groupBox7);
            this.pnlClientArea.Controls.Add(this.groupBox4);
            this.pnlClientArea.Controls.Add(this.groupBox2);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(627, 468);
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
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(627, 468);
            this.panel1.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label23);
            this.groupBox7.Controls.Add(this.nbLeftFontSize);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.nbThickness);
            this.groupBox7.Location = new System.Drawing.Point(3, 203);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(210, 81);
            this.groupBox7.TabIndex = 21;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Size";
            this.groupBox7.Enter += new System.EventHandler(this.groupBox7_Enter);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label23.Font = new System.Drawing.Font("Arial", 10F);
            this.label23.ForeColor = System.Drawing.Color.Black;
            this.label23.Location = new System.Drawing.Point(8, 24);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(66, 16);
            this.label23.TabIndex = 11;
            this.label23.Text = "Font Size\r\n";
            // 
            // nbLeftFontSize
            // 
            this.nbLeftFontSize.Font = new System.Drawing.Font("Arial", 12F);
            this.nbLeftFontSize.Location = new System.Drawing.Point(125, 21);
            this.nbLeftFontSize.Name = "nbLeftFontSize";
            this.nbLeftFontSize.Size = new System.Drawing.Size(79, 26);
            this.nbLeftFontSize.TabIndex = 12;
            this.nbLeftFontSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label16.Font = new System.Drawing.Font("Arial", 12F);
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(6, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(78, 18);
            this.label16.TabIndex = 9;
            this.label16.Text = "Thickness";
            // 
            // nbThickness
            // 
            this.nbThickness.Font = new System.Drawing.Font("Arial", 12F);
            this.nbThickness.Location = new System.Drawing.Point(125, 49);
            this.nbThickness.Name = "nbThickness";
            this.nbThickness.Size = new System.Drawing.Size(79, 26);
            this.nbThickness.TabIndex = 10;
            this.nbThickness.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbAlaram);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.tbWarning);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.tbListMaxCount);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.tbMinY);
            this.groupBox4.Controls.Add(this.tbMaxY);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Location = new System.Drawing.Point(219, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(198, 193);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Option";
            // 
            // tbAlaram
            // 
            this.tbAlaram.Location = new System.Drawing.Point(103, 134);
            this.tbAlaram.Name = "tbAlaram";
            this.tbAlaram.Size = new System.Drawing.Size(87, 22);
            this.tbAlaram.TabIndex = 20;
            this.tbAlaram.Text = "0";
            this.tbAlaram.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Arial", 12F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(2, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 18);
            this.label2.TabIndex = 19;
            this.label2.Text = "Alarm Val";
            // 
            // tbWarning
            // 
            this.tbWarning.Location = new System.Drawing.Point(103, 106);
            this.tbWarning.Name = "tbWarning";
            this.tbWarning.Size = new System.Drawing.Size(87, 22);
            this.tbWarning.TabIndex = 18;
            this.tbWarning.Text = "0";
            this.tbWarning.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label17.Font = new System.Drawing.Font("Arial", 12F);
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(2, 107);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 18);
            this.label17.TabIndex = 17;
            this.label17.Text = "Warning Val";
            // 
            // tbListMaxCount
            // 
            this.tbListMaxCount.Location = new System.Drawing.Point(103, 20);
            this.tbListMaxCount.Name = "tbListMaxCount";
            this.tbListMaxCount.Size = new System.Drawing.Size(87, 22);
            this.tbListMaxCount.TabIndex = 16;
            this.tbListMaxCount.Text = "1000";
            this.tbListMaxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label20.Font = new System.Drawing.Font("Arial", 12F);
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(2, 22);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(82, 18);
            this.label20.TabIndex = 15;
            this.label20.Text = "Max Count";
            // 
            // tbMinY
            // 
            this.tbMinY.Location = new System.Drawing.Point(103, 76);
            this.tbMinY.Name = "tbMinY";
            this.tbMinY.Size = new System.Drawing.Size(87, 22);
            this.tbMinY.TabIndex = 14;
            this.tbMinY.Text = "0";
            this.tbMinY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbMaxY
            // 
            this.tbMaxY.Location = new System.Drawing.Point(103, 47);
            this.tbMaxY.Name = "tbMaxY";
            this.tbMaxY.Size = new System.Drawing.Size(87, 22);
            this.tbMaxY.TabIndex = 13;
            this.tbMaxY.Text = "1000";
            this.tbMaxY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label18.Font = new System.Drawing.Font("Arial", 12F);
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(2, 76);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 18);
            this.label18.TabIndex = 12;
            this.label18.Text = "Lower Val";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label19.Font = new System.Drawing.Font("Arial", 12F);
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(2, 48);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(77, 18);
            this.label19.TabIndex = 11;
            this.label19.Text = "Upper Val";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.pnGridColor);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.pnAlarmLineColor);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.pnWarningLineColor);
            this.groupBox2.Controls.Add(this.pnTextColor);
            this.groupBox2.Controls.Add(this.pnCurrentDataColor);
            this.groupBox2.Controls.Add(this.pnBackgroundColor);
            this.groupBox2.Location = new System.Drawing.Point(3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 197);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Color";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Location = new System.Drawing.Point(216, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(210, 253);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Color";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Font = new System.Drawing.Font("Arial", 12F);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(19, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 18);
            this.label8.TabIndex = 6;
            this.label8.Text = "Currnet Data";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Arial", 12F);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(19, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 18);
            this.label9.TabIndex = 5;
            this.label9.Text = "Background";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Arial", 12F);
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(19, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 18);
            this.label10.TabIndex = 4;
            this.label10.Text = "Alarm Line";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Arial", 12F);
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(19, 186);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 18);
            this.label11.TabIndex = 3;
            this.label11.Text = "Warning Line";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Arial", 12F);
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(19, 148);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 18);
            this.label12.TabIndex = 2;
            this.label12.Text = "Spec Out";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label13.Font = new System.Drawing.Font("Arial", 12F);
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(19, 114);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 18);
            this.label13.TabIndex = 1;
            this.label13.Text = "Spec In";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label14.Font = new System.Drawing.Font("Arial", 12F);
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(19, 33);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 18);
            this.label14.TabIndex = 0;
            this.label14.Text = "Text";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(25, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text";
            // 
            // pnGridColor
            // 
            this.pnGridColor.BackColor = System.Drawing.Color.Silver;
            this.pnGridColor.Location = new System.Drawing.Point(128, 107);
            this.pnGridColor.Name = "pnGridColor";
            this.pnGridColor.Size = new System.Drawing.Size(54, 18);
            this.pnGridColor.TabIndex = 16;
            this.pnGridColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Arial", 12F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(25, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Alarm Area";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Arial", 12F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(25, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "Warning Area";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label15.Font = new System.Drawing.Font("Arial", 12F);
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(25, 107);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(38, 18);
            this.label15.TabIndex = 15;
            this.label15.Text = "Grid";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Arial", 12F);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(25, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 18);
            this.label6.TabIndex = 5;
            this.label6.Text = "Background";
            // 
            // pnAlarmLineColor
            // 
            this.pnAlarmLineColor.BackColor = System.Drawing.Color.Red;
            this.pnAlarmLineColor.Location = new System.Drawing.Point(128, 159);
            this.pnAlarmLineColor.Name = "pnAlarmLineColor";
            this.pnAlarmLineColor.Size = new System.Drawing.Size(54, 18);
            this.pnAlarmLineColor.TabIndex = 14;
            this.pnAlarmLineColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Arial", 12F);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(25, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 18);
            this.label7.TabIndex = 6;
            this.label7.Text = "Current Data";
            // 
            // pnWarningLineColor
            // 
            this.pnWarningLineColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnWarningLineColor.Location = new System.Drawing.Point(128, 134);
            this.pnWarningLineColor.Name = "pnWarningLineColor";
            this.pnWarningLineColor.Size = new System.Drawing.Size(54, 18);
            this.pnWarningLineColor.TabIndex = 13;
            this.pnWarningLineColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // pnTextColor
            // 
            this.pnTextColor.BackColor = System.Drawing.Color.Black;
            this.pnTextColor.Location = new System.Drawing.Point(128, 32);
            this.pnTextColor.Name = "pnTextColor";
            this.pnTextColor.Size = new System.Drawing.Size(54, 18);
            this.pnTextColor.TabIndex = 8;
            this.pnTextColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // pnCurrentDataColor
            // 
            this.pnCurrentDataColor.BackColor = System.Drawing.Color.Blue;
            this.pnCurrentDataColor.Location = new System.Drawing.Point(128, 57);
            this.pnCurrentDataColor.Name = "pnCurrentDataColor";
            this.pnCurrentDataColor.Size = new System.Drawing.Size(54, 18);
            this.pnCurrentDataColor.TabIndex = 9;
            this.pnCurrentDataColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // pnBackgroundColor
            // 
            this.pnBackgroundColor.BackColor = System.Drawing.Color.White;
            this.pnBackgroundColor.Location = new System.Drawing.Point(128, 82);
            this.pnBackgroundColor.Name = "pnBackgroundColor";
            this.pnBackgroundColor.Size = new System.Drawing.Size(54, 18);
            this.pnBackgroundColor.TabIndex = 10;
            this.pnBackgroundColor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnChangeColor);
            // 
            // btnSaveParameter
            // 
            this.btnSaveParameter.BackColor = System.Drawing.Color.White;
            this.btnSaveParameter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.BorderRadius = 15;
            this.btnSaveParameter.BorderSize = 3;
            this.btnSaveParameter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveParameter.Design = RJCodeUI_M1.RJControls.ButtonDesign.Custom;
            this.btnSaveParameter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(79)))), ((int)(((byte)(82)))));
            this.btnSaveParameter.FlatAppearance.BorderSize = 3;
            this.btnSaveParameter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.btnSaveParameter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveParameter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveParameter.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveParameter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveParameter.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnSaveParameter.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveParameter.IconSize = 80;
            this.btnSaveParameter.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveParameter.Location = new System.Drawing.Point(436, 349);
            this.btnSaveParameter.Name = "btnSaveParameter";
            this.btnSaveParameter.Size = new System.Drawing.Size(180, 105);
            this.btnSaveParameter.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnSaveParameter.TabIndex = 2145;
            this.btnSaveParameter.Text = "SAVE";
            this.btnSaveParameter.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveParameter.UseVisualStyleBackColor = false;
            this.btnSaveParameter.Click += new System.EventHandler(this.btnSaveParameter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDot);
            this.groupBox1.Controls.Add(this.rdoLine);
            this.groupBox1.Controls.Add(this.cbDisplayAlaram);
            this.groupBox1.Controls.Add(this.cbDisplayWarning);
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Controls.Add(this.tbUnit);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Location = new System.Drawing.Point(423, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 200);
            this.groupBox1.TabIndex = 2146;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display";
            // 
            // cbDisplayAlaram
            // 
            this.cbDisplayAlaram.AutoSize = true;
            this.cbDisplayAlaram.Location = new System.Drawing.Point(13, 111);
            this.cbDisplayAlaram.Name = "cbDisplayAlaram";
            this.cbDisplayAlaram.Size = new System.Drawing.Size(97, 20);
            this.cbDisplayAlaram.TabIndex = 20;
            this.cbDisplayAlaram.Text = "Alaram Use";
            this.cbDisplayAlaram.UseVisualStyleBackColor = true;
            // 
            // cbDisplayWarning
            // 
            this.cbDisplayWarning.AutoSize = true;
            this.cbDisplayWarning.Location = new System.Drawing.Point(13, 82);
            this.cbDisplayWarning.Name = "cbDisplayWarning";
            this.cbDisplayWarning.Size = new System.Drawing.Size(104, 20);
            this.cbDisplayWarning.TabIndex = 19;
            this.cbDisplayWarning.Text = "Warning Use";
            this.cbDisplayWarning.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(57, 21);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(120, 22);
            this.tbName.TabIndex = 18;
            this.tbName.Text = "1000";
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbUnit
            // 
            this.tbUnit.Location = new System.Drawing.Point(57, 48);
            this.tbUnit.Name = "tbUnit";
            this.tbUnit.Size = new System.Drawing.Size(120, 22);
            this.tbUnit.TabIndex = 17;
            this.tbUnit.Text = "1000";
            this.tbUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Arial", 10F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(8, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Name";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label21.Font = new System.Drawing.Font("Arial", 12F);
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(6, 51);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 18);
            this.label21.TabIndex = 9;
            this.label21.Text = "Unit";
            // 
            // rdoLine
            // 
            this.rdoLine.AutoSize = true;
            this.rdoLine.Location = new System.Drawing.Point(13, 142);
            this.rdoLine.Name = "rdoLine";
            this.rdoLine.Size = new System.Drawing.Size(50, 20);
            this.rdoLine.TabIndex = 21;
            this.rdoLine.TabStop = true;
            this.rdoLine.Text = "Line";
            this.rdoLine.UseVisualStyleBackColor = true;
            // 
            // rdoDot
            // 
            this.rdoDot.AutoSize = true;
            this.rdoDot.Location = new System.Drawing.Point(13, 168);
            this.rdoDot.Name = "rdoDot";
            this.rdoDot.Size = new System.Drawing.Size(46, 20);
            this.rdoDot.TabIndex = 22;
            this.rdoDot.TabStop = true;
            this.rdoDot.Text = "Dot";
            this.rdoDot.UseVisualStyleBackColor = true;
            // 
            // FormPowerGraphConfig
            // 
            this._DesktopPanelSize = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "PowerGraphConfig";
            this.ClientSize = new System.Drawing.Size(629, 510);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormPowerGraphConfig";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "PowerGraphConfig";
            this.Load += new System.EventHandler(this.Form_Load);
            this.VisibleChanged += new System.EventHandler(this.FormLayerDisplay_VisibleChanged);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbLeftFontSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbThickness)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown nbLeftFontSize;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nbThickness;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbListMaxCount;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbMinY;
        private System.Windows.Forms.TextBox tbMaxY;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnGridColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnAlarmLineColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnWarningLineColor;
        private System.Windows.Forms.Panel pnTextColor;
        private System.Windows.Forms.Panel pnCurrentDataColor;
        private System.Windows.Forms.Panel pnBackgroundColor;
        private RJCodeUI_M1.RJControls.RJButton btnSaveParameter;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TextBox tbAlaram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbWarning;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox cbDisplayAlaram;
        private System.Windows.Forms.CheckBox cbDisplayWarning;
        private System.Windows.Forms.RadioButton rdoDot;
        private System.Windows.Forms.RadioButton rdoLine;
    }
}