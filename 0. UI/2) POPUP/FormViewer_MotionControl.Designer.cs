namespace IntelligentFactory
{
    partial class FormViewer_MotionControl
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
            this.timerView = new System.Windows.Forms.Timer(this.components);
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnAxisStop = new MetroFramework.Controls.MetroTile();
            this.btnMotorServoOn = new MetroFramework.Controls.MetroTile();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.btnJogPlus = new MetroFramework.Controls.MetroTile();
            this.panel2 = new System.Windows.Forms.Panel();
            this.metroTile5 = new MetroFramework.Controls.MetroTile();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnJogMinus = new MetroFramework.Controls.MetroTile();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbAbsoluteSpeed = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.tbAbsoluteTarget = new MetroFramework.Controls.MetroTextBox();
            this.tbStepSpeed = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.tbStepmm = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.tbJogSpeed = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.lbStatusGPIO1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.btnStartHome = new MetroFramework.Controls.MetroTile();
            this.lbStatusServoAlarm = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbCommandPulse = new System.Windows.Forms.Label();
            this.lbStatusInPosition = new System.Windows.Forms.Label();
            this.lbStatusMotioning = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbStatusLimitMinus = new System.Windows.Forms.Label();
            this.lbStatusHomeComplete = new System.Windows.Forms.Label();
            this.lbActualPulse = new System.Windows.Forms.Label();
            this.lbStatusHome = new System.Windows.Forms.Label();
            this.lbStatusLimitPlus = new System.Windows.Forms.Label();
            this.lbStatusServoOn = new System.Windows.Forms.Label();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.btnSave = new MetroFramework.Controls.MetroTile();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.label10 = new System.Windows.Forms.Label();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.metroTextBox2 = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroTextBox3 = new MetroFramework.Controls.MetroTextBox();
            this.Move_Pos = new MetroFramework.Controls.MetroTile();
            this.tcIO = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage10 = new MetroFramework.Controls.MetroTabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbInWorkCountSensor = new System.Windows.Forms.Label();
            this.ucCylinder6 = new IntelligentFactory.ucCylinder();
            this.ucCylinder5 = new IntelligentFactory.ucCylinder();
            this.ucCylinder4 = new IntelligentFactory.ucCylinder();
            this.ucCylinder3 = new IntelligentFactory.ucCylinder();
            this.ucCylinder2 = new IntelligentFactory.ucCylinder();
            this.ucCylinder1 = new IntelligentFactory.ucCylinder();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.panel2.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.tcIO.SuspendLayout();
            this.metroTabPage10.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerView
            // 
            this.timerView.Enabled = true;
            this.timerView.Tick += new System.EventHandler(this.timerView_Tick);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Lime;
            // 
            // btnAxisStop
            // 
            this.btnAxisStop.ActiveControl = null;
            this.btnAxisStop.BackColor = System.Drawing.Color.Transparent;
            this.btnAxisStop.Location = new System.Drawing.Point(603, 246);
            this.btnAxisStop.Name = "btnAxisStop";
            this.btnAxisStop.Size = new System.Drawing.Size(155, 191);
            this.btnAxisStop.Style = MetroFramework.MetroColorStyle.Red;
            this.btnAxisStop.TabIndex = 1273;
            this.btnAxisStop.Text = "STOP";
            this.btnAxisStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAxisStop.TileImage = global::IntelligentFactory.Properties.Resources.delete_50;
            this.btnAxisStop.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAxisStop.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnAxisStop.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnAxisStop.UseSelectable = true;
            this.btnAxisStop.UseTileImage = true;
            this.btnAxisStop.Click += new System.EventHandler(this.btnAxisStop_Click);
            // 
            // btnMotorServoOn
            // 
            this.btnMotorServoOn.ActiveControl = null;
            this.btnMotorServoOn.BackColor = System.Drawing.Color.Transparent;
            this.btnMotorServoOn.Location = new System.Drawing.Point(604, 96);
            this.btnMotorServoOn.Name = "btnMotorServoOn";
            this.btnMotorServoOn.Size = new System.Drawing.Size(155, 113);
            this.btnMotorServoOn.Style = MetroFramework.MetroColorStyle.Purple;
            this.btnMotorServoOn.TabIndex = 1274;
            this.btnMotorServoOn.Text = "SERVO ON";
            this.btnMotorServoOn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnMotorServoOn.TileImage = global::IntelligentFactory.Properties.Resources.switches_50;
            this.btnMotorServoOn.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnMotorServoOn.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnMotorServoOn.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnMotorServoOn.UseSelectable = true;
            this.btnMotorServoOn.UseTileImage = true;
            this.btnMotorServoOn.Click += new System.EventHandler(this.btnMotorServoOn_Click);
            // 
            // metroLabel12
            // 
            this.metroLabel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel12.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel12.ForeColor = System.Drawing.Color.White;
            this.metroLabel12.Location = new System.Drawing.Point(448, 60);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(312, 35);
            this.metroLabel12.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel12.TabIndex = 1275;
            this.metroLabel12.Text = "OPERATION";
            this.metroLabel12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel12.UseCustomBackColor = true;
            this.metroLabel12.UseCustomForeColor = true;
            // 
            // btnJogPlus
            // 
            this.btnJogPlus.ActiveControl = null;
            this.btnJogPlus.BackColor = System.Drawing.Color.Transparent;
            this.btnJogPlus.Location = new System.Drawing.Point(447, 247);
            this.btnJogPlus.Name = "btnJogPlus";
            this.btnJogPlus.Size = new System.Drawing.Size(155, 62);
            this.btnJogPlus.Style = MetroFramework.MetroColorStyle.Yellow;
            this.btnJogPlus.TabIndex = 1272;
            this.btnJogPlus.Text = "JOG +";
            this.btnJogPlus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJogPlus.TileImage = global::IntelligentFactory.Properties.Resources.move_right_50;
            this.btnJogPlus.TileImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJogPlus.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnJogPlus.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnJogPlus.UseSelectable = true;
            this.btnJogPlus.UseTileImage = true;
            this.btnJogPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogPlus_MouseDown);
            this.btnJogPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogPlus_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.metroTile5);
            this.panel2.Controls.Add(this.metroTile4);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.btnJogMinus);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbAbsoluteSpeed);
            this.panel2.Controls.Add(this.metroLabel10);
            this.panel2.Controls.Add(this.tbAbsoluteTarget);
            this.panel2.Controls.Add(this.tbStepSpeed);
            this.panel2.Controls.Add(this.metroLabel3);
            this.panel2.Controls.Add(this.tbStepmm);
            this.panel2.Controls.Add(this.metroLabel2);
            this.panel2.Controls.Add(this.tbJogSpeed);
            this.panel2.Location = new System.Drawing.Point(22, 246);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(737, 193);
            this.panel2.TabIndex = 1270;
            // 
            // metroTile5
            // 
            this.metroTile5.ActiveControl = null;
            this.metroTile5.BackColor = System.Drawing.Color.Transparent;
            this.metroTile5.Location = new System.Drawing.Point(425, 128);
            this.metroTile5.Name = "metroTile5";
            this.metroTile5.Size = new System.Drawing.Size(155, 63);
            this.metroTile5.Style = MetroFramework.MetroColorStyle.Yellow;
            this.metroTile5.TabIndex = 1266;
            this.metroTile5.Text = "MOVE";
            this.metroTile5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.metroTile5.TileImage = global::IntelligentFactory.Properties.Resources.all_out_50;
            this.metroTile5.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile5.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile5.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile5.UseSelectable = true;
            this.metroTile5.UseTileImage = true;
            this.metroTile5.Click += new System.EventHandler(this.metroTile5_Click);
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(425, 64);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(155, 63);
            this.metroTile4.Style = MetroFramework.MetroColorStyle.Yellow;
            this.metroTile4.TabIndex = 1265;
            this.metroTile4.Text = "MOVE";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.metroTile4.TileImage = global::IntelligentFactory.Properties.Resources.all_out_50;
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile4.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile4.UseSelectable = true;
            this.metroTile4.UseTileImage = true;
            this.metroTile4.Click += new System.EventHandler(this.metroTile4_Click);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label9.Location = new System.Drawing.Point(269, 128);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(155, 27);
            this.label9.TabIndex = 1261;
            this.label9.Text = "TARGET (mm)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.Location = new System.Drawing.Point(269, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(155, 27);
            this.label8.TabIndex = 1260;
            this.label8.Text = "STEP (mm)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnJogMinus
            // 
            this.btnJogMinus.ActiveControl = null;
            this.btnJogMinus.BackColor = System.Drawing.Color.Transparent;
            this.btnJogMinus.Location = new System.Drawing.Point(269, 1);
            this.btnJogMinus.Name = "btnJogMinus";
            this.btnJogMinus.Size = new System.Drawing.Size(155, 62);
            this.btnJogMinus.Style = MetroFramework.MetroColorStyle.Yellow;
            this.btnJogMinus.TabIndex = 1265;
            this.btnJogMinus.Text = "JOG -";
            this.btnJogMinus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJogMinus.TileImage = global::IntelligentFactory.Properties.Resources.move_left_50;
            this.btnJogMinus.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJogMinus.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnJogMinus.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnJogMinus.UseSelectable = true;
            this.btnJogMinus.UseTileImage = true;
            this.btnJogMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogPlus_MouseDown);
            this.btnJogMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogPlus_MouseUp);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.Location = new System.Drawing.Point(113, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(155, 27);
            this.label7.TabIndex = 1259;
            this.label7.Text = "SPEED (um/s)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Location = new System.Drawing.Point(113, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 27);
            this.label5.TabIndex = 1258;
            this.label5.Text = "SPEED (um/s)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(113, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 26);
            this.label3.TabIndex = 1257;
            this.label3.Text = "SPEED (mm/s)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAbsoluteSpeed
            // 
            // 
            // 
            // 
            this.tbAbsoluteSpeed.CustomButton.Image = null;
            this.tbAbsoluteSpeed.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.tbAbsoluteSpeed.CustomButton.Name = "";
            this.tbAbsoluteSpeed.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbAbsoluteSpeed.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAbsoluteSpeed.CustomButton.TabIndex = 1;
            this.tbAbsoluteSpeed.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAbsoluteSpeed.CustomButton.UseSelectable = true;
            this.tbAbsoluteSpeed.CustomButton.Visible = false;
            this.tbAbsoluteSpeed.DisplayIcon = true;
            this.tbAbsoluteSpeed.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbAbsoluteSpeed.Lines = new string[] {
        "10"};
            this.tbAbsoluteSpeed.Location = new System.Drawing.Point(113, 156);
            this.tbAbsoluteSpeed.MaxLength = 32767;
            this.tbAbsoluteSpeed.Name = "tbAbsoluteSpeed";
            this.tbAbsoluteSpeed.PasswordChar = '\0';
            this.tbAbsoluteSpeed.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAbsoluteSpeed.SelectedText = "";
            this.tbAbsoluteSpeed.SelectionLength = 0;
            this.tbAbsoluteSpeed.SelectionStart = 0;
            this.tbAbsoluteSpeed.ShortcutsEnabled = true;
            this.tbAbsoluteSpeed.Size = new System.Drawing.Size(156, 35);
            this.tbAbsoluteSpeed.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbAbsoluteSpeed.TabIndex = 1089;
            this.tbAbsoluteSpeed.Text = "10";
            this.tbAbsoluteSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbAbsoluteSpeed.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAbsoluteSpeed.UseSelectable = true;
            this.tbAbsoluteSpeed.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAbsoluteSpeed.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel10
            // 
            this.metroLabel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel10.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel10.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel10.ForeColor = System.Drawing.Color.White;
            this.metroLabel10.Location = new System.Drawing.Point(0, 128);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(112, 63);
            this.metroLabel10.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel10.TabIndex = 1088;
            this.metroLabel10.Text = "ABSOLUTE";
            this.metroLabel10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel10.UseCustomBackColor = true;
            this.metroLabel10.UseCustomForeColor = true;
            // 
            // tbAbsoluteTarget
            // 
            // 
            // 
            // 
            this.tbAbsoluteTarget.CustomButton.Image = null;
            this.tbAbsoluteTarget.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.tbAbsoluteTarget.CustomButton.Name = "";
            this.tbAbsoluteTarget.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbAbsoluteTarget.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAbsoluteTarget.CustomButton.TabIndex = 1;
            this.tbAbsoluteTarget.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAbsoluteTarget.CustomButton.UseSelectable = true;
            this.tbAbsoluteTarget.CustomButton.Visible = false;
            this.tbAbsoluteTarget.DisplayIcon = true;
            this.tbAbsoluteTarget.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbAbsoluteTarget.Lines = new string[] {
        "5"};
            this.tbAbsoluteTarget.Location = new System.Drawing.Point(269, 156);
            this.tbAbsoluteTarget.MaxLength = 32767;
            this.tbAbsoluteTarget.Name = "tbAbsoluteTarget";
            this.tbAbsoluteTarget.PasswordChar = '\0';
            this.tbAbsoluteTarget.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAbsoluteTarget.SelectedText = "";
            this.tbAbsoluteTarget.SelectionLength = 0;
            this.tbAbsoluteTarget.SelectionStart = 0;
            this.tbAbsoluteTarget.ShortcutsEnabled = true;
            this.tbAbsoluteTarget.Size = new System.Drawing.Size(156, 35);
            this.tbAbsoluteTarget.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbAbsoluteTarget.TabIndex = 1086;
            this.tbAbsoluteTarget.Text = "5";
            this.tbAbsoluteTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbAbsoluteTarget.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAbsoluteTarget.UseSelectable = true;
            this.tbAbsoluteTarget.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAbsoluteTarget.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // tbStepSpeed
            // 
            // 
            // 
            // 
            this.tbStepSpeed.CustomButton.Image = null;
            this.tbStepSpeed.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.tbStepSpeed.CustomButton.Name = "";
            this.tbStepSpeed.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbStepSpeed.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbStepSpeed.CustomButton.TabIndex = 1;
            this.tbStepSpeed.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStepSpeed.CustomButton.UseSelectable = true;
            this.tbStepSpeed.CustomButton.Visible = false;
            this.tbStepSpeed.DisplayIcon = true;
            this.tbStepSpeed.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbStepSpeed.Lines = new string[] {
        "10"};
            this.tbStepSpeed.Location = new System.Drawing.Point(113, 92);
            this.tbStepSpeed.MaxLength = 32767;
            this.tbStepSpeed.Name = "tbStepSpeed";
            this.tbStepSpeed.PasswordChar = '\0';
            this.tbStepSpeed.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbStepSpeed.SelectedText = "";
            this.tbStepSpeed.SelectionLength = 0;
            this.tbStepSpeed.SelectionStart = 0;
            this.tbStepSpeed.ShortcutsEnabled = true;
            this.tbStepSpeed.Size = new System.Drawing.Size(156, 35);
            this.tbStepSpeed.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbStepSpeed.TabIndex = 1084;
            this.tbStepSpeed.Text = "10";
            this.tbStepSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbStepSpeed.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStepSpeed.UseSelectable = true;
            this.tbStepSpeed.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbStepSpeed.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel3
            // 
            this.metroLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel3.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel3.ForeColor = System.Drawing.Color.White;
            this.metroLabel3.Location = new System.Drawing.Point(0, 64);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(112, 63);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel3.TabIndex = 1081;
            this.metroLabel3.Text = "STEP";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel3.UseCustomBackColor = true;
            this.metroLabel3.UseCustomForeColor = true;
            // 
            // tbStepmm
            // 
            // 
            // 
            // 
            this.tbStepmm.CustomButton.Image = null;
            this.tbStepmm.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.tbStepmm.CustomButton.Name = "";
            this.tbStepmm.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbStepmm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbStepmm.CustomButton.TabIndex = 1;
            this.tbStepmm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStepmm.CustomButton.UseSelectable = true;
            this.tbStepmm.CustomButton.Visible = false;
            this.tbStepmm.DisplayIcon = true;
            this.tbStepmm.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbStepmm.Lines = new string[] {
        "5"};
            this.tbStepmm.Location = new System.Drawing.Point(269, 92);
            this.tbStepmm.MaxLength = 32767;
            this.tbStepmm.Name = "tbStepmm";
            this.tbStepmm.PasswordChar = '\0';
            this.tbStepmm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbStepmm.SelectedText = "";
            this.tbStepmm.SelectionLength = 0;
            this.tbStepmm.SelectionStart = 0;
            this.tbStepmm.ShortcutsEnabled = true;
            this.tbStepmm.Size = new System.Drawing.Size(156, 35);
            this.tbStepmm.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbStepmm.TabIndex = 1079;
            this.tbStepmm.Text = "5";
            this.tbStepmm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbStepmm.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStepmm.UseSelectable = true;
            this.tbStepmm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbStepmm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel2.ForeColor = System.Drawing.Color.White;
            this.metroLabel2.Location = new System.Drawing.Point(0, 0);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(112, 63);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel2.TabIndex = 1076;
            this.metroLabel2.Text = "JOG";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.UseCustomBackColor = true;
            this.metroLabel2.UseCustomForeColor = true;
            // 
            // tbJogSpeed
            // 
            // 
            // 
            // 
            this.tbJogSpeed.CustomButton.Image = null;
            this.tbJogSpeed.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.tbJogSpeed.CustomButton.Name = "";
            this.tbJogSpeed.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbJogSpeed.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbJogSpeed.CustomButton.TabIndex = 1;
            this.tbJogSpeed.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbJogSpeed.CustomButton.UseSelectable = true;
            this.tbJogSpeed.CustomButton.Visible = false;
            this.tbJogSpeed.DisplayIcon = true;
            this.tbJogSpeed.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbJogSpeed.Lines = new string[] {
        "10"};
            this.tbJogSpeed.Location = new System.Drawing.Point(113, 28);
            this.tbJogSpeed.MaxLength = 32767;
            this.tbJogSpeed.Name = "tbJogSpeed";
            this.tbJogSpeed.PasswordChar = '\0';
            this.tbJogSpeed.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbJogSpeed.SelectedText = "";
            this.tbJogSpeed.SelectionLength = 0;
            this.tbJogSpeed.SelectionStart = 0;
            this.tbJogSpeed.ShortcutsEnabled = true;
            this.tbJogSpeed.Size = new System.Drawing.Size(156, 35);
            this.tbJogSpeed.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbJogSpeed.TabIndex = 1015;
            this.tbJogSpeed.Text = "10";
            this.tbJogSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbJogSpeed.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbJogSpeed.UseSelectable = true;
            this.tbJogSpeed.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbJogSpeed.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel7
            // 
            this.metroLabel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel7.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel7.ForeColor = System.Drawing.Color.White;
            this.metroLabel7.Location = new System.Drawing.Point(22, 210);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(737, 35);
            this.metroLabel7.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel7.TabIndex = 1269;
            this.metroLabel7.Text = "CONTROL";
            this.metroLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel7.UseCustomBackColor = true;
            this.metroLabel7.UseCustomForeColor = true;
            // 
            // lbStatusGPIO1
            // 
            this.lbStatusGPIO1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusGPIO1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lbStatusGPIO1.ForeColor = System.Drawing.Color.White;
            this.lbStatusGPIO1.Location = new System.Drawing.Point(23, 60);
            this.lbStatusGPIO1.Name = "lbStatusGPIO1";
            this.lbStatusGPIO1.Size = new System.Drawing.Size(424, 35);
            this.lbStatusGPIO1.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbStatusGPIO1.TabIndex = 1268;
            this.lbStatusGPIO1.Text = "STATUS";
            this.lbStatusGPIO1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbStatusGPIO1.UseCustomBackColor = true;
            this.lbStatusGPIO1.UseCustomForeColor = true;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.btnStartHome);
            this.metroPanel1.Controls.Add(this.lbStatusServoAlarm);
            this.metroPanel1.Controls.Add(this.label6);
            this.metroPanel1.Controls.Add(this.lbCommandPulse);
            this.metroPanel1.Controls.Add(this.lbStatusInPosition);
            this.metroPanel1.Controls.Add(this.lbStatusMotioning);
            this.metroPanel1.Controls.Add(this.label1);
            this.metroPanel1.Controls.Add(this.lbStatusLimitMinus);
            this.metroPanel1.Controls.Add(this.lbStatusHomeComplete);
            this.metroPanel1.Controls.Add(this.lbActualPulse);
            this.metroPanel1.Controls.Add(this.lbStatusHome);
            this.metroPanel1.Controls.Add(this.lbStatusLimitPlus);
            this.metroPanel1.Controls.Add(this.lbStatusServoOn);
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(22, 95);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(738, 114);
            this.metroPanel1.TabIndex = 1271;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // btnStartHome
            // 
            this.btnStartHome.ActiveControl = null;
            this.btnStartHome.BackColor = System.Drawing.Color.Transparent;
            this.btnStartHome.Location = new System.Drawing.Point(426, 1);
            this.btnStartHome.Name = "btnStartHome";
            this.btnStartHome.Size = new System.Drawing.Size(155, 113);
            this.btnStartHome.Style = MetroFramework.MetroColorStyle.Yellow;
            this.btnStartHome.TabIndex = 1264;
            this.btnStartHome.Text = "HOME";
            this.btnStartHome.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.btnStartHome.TileImage = global::IntelligentFactory.Properties.Resources.icons8_home_50;
            this.btnStartHome.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnStartHome.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnStartHome.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnStartHome.UseSelectable = true;
            this.btnStartHome.UseTileImage = true;
            this.btnStartHome.Click += new System.EventHandler(this.btnStartHome_Click);
            // 
            // lbStatusServoAlarm
            // 
            this.lbStatusServoAlarm.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusServoAlarm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusServoAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusServoAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusServoAlarm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusServoAlarm.Location = new System.Drawing.Point(1, 39);
            this.lbStatusServoAlarm.Name = "lbStatusServoAlarm";
            this.lbStatusServoAlarm.Size = new System.Drawing.Size(105, 37);
            this.lbStatusServoAlarm.TabIndex = 1191;
            this.lbStatusServoAlarm.Text = "ALARM";
            this.lbStatusServoAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Location = new System.Drawing.Point(213, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 37);
            this.label6.TabIndex = 1260;
            this.label6.Text = "Command Position\r\n[Pulse]";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCommandPulse
            // 
            this.lbCommandPulse.BackColor = System.Drawing.Color.Black;
            this.lbCommandPulse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbCommandPulse.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.lbCommandPulse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbCommandPulse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbCommandPulse.Location = new System.Drawing.Point(326, 77);
            this.lbCommandPulse.Name = "lbCommandPulse";
            this.lbCommandPulse.Size = new System.Drawing.Size(99, 37);
            this.lbCommandPulse.TabIndex = 1259;
            this.lbCommandPulse.Text = "00000000";
            this.lbCommandPulse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusInPosition
            // 
            this.lbStatusInPosition.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusInPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusInPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusInPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusInPosition.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusInPosition.Location = new System.Drawing.Point(319, 39);
            this.lbStatusInPosition.Name = "lbStatusInPosition";
            this.lbStatusInPosition.Size = new System.Drawing.Size(106, 37);
            this.lbStatusInPosition.TabIndex = 1190;
            this.lbStatusInPosition.Text = "INPOSITION";
            this.lbStatusInPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusMotioning
            // 
            this.lbStatusMotioning.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusMotioning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusMotioning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusMotioning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusMotioning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusMotioning.Location = new System.Drawing.Point(213, 39);
            this.lbStatusMotioning.Name = "lbStatusMotioning";
            this.lbStatusMotioning.Size = new System.Drawing.Size(105, 37);
            this.lbStatusMotioning.TabIndex = 1189;
            this.lbStatusMotioning.Text = "MOTIONING";
            this.lbStatusMotioning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(1, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 37);
            this.label1.TabIndex = 1256;
            this.label1.Text = "Actual Position\r\n[Pulse]";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusLimitMinus
            // 
            this.lbStatusLimitMinus.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusLimitMinus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusLimitMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusLimitMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusLimitMinus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusLimitMinus.Location = new System.Drawing.Point(319, 1);
            this.lbStatusLimitMinus.Name = "lbStatusLimitMinus";
            this.lbStatusLimitMinus.Size = new System.Drawing.Size(106, 37);
            this.lbStatusLimitMinus.TabIndex = 1187;
            this.lbStatusLimitMinus.Text = "LIMIT -";
            this.lbStatusLimitMinus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusHomeComplete
            // 
            this.lbStatusHomeComplete.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusHomeComplete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusHomeComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusHomeComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusHomeComplete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusHomeComplete.Location = new System.Drawing.Point(107, 39);
            this.lbStatusHomeComplete.Name = "lbStatusHomeComplete";
            this.lbStatusHomeComplete.Size = new System.Drawing.Size(105, 37);
            this.lbStatusHomeComplete.TabIndex = 1188;
            this.lbStatusHomeComplete.Text = "HOME\r\nCOMPLETE";
            this.lbStatusHomeComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbActualPulse
            // 
            this.lbActualPulse.BackColor = System.Drawing.Color.Black;
            this.lbActualPulse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbActualPulse.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Bold);
            this.lbActualPulse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbActualPulse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbActualPulse.Location = new System.Drawing.Point(114, 77);
            this.lbActualPulse.Name = "lbActualPulse";
            this.lbActualPulse.Size = new System.Drawing.Size(98, 37);
            this.lbActualPulse.TabIndex = 1255;
            this.lbActualPulse.Text = "0000000";
            this.lbActualPulse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusHome
            // 
            this.lbStatusHome.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusHome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusHome.Location = new System.Drawing.Point(107, 1);
            this.lbStatusHome.Name = "lbStatusHome";
            this.lbStatusHome.Size = new System.Drawing.Size(105, 37);
            this.lbStatusHome.TabIndex = 1185;
            this.lbStatusHome.Text = "HOME";
            this.lbStatusHome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusLimitPlus
            // 
            this.lbStatusLimitPlus.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusLimitPlus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusLimitPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusLimitPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusLimitPlus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusLimitPlus.Location = new System.Drawing.Point(213, 1);
            this.lbStatusLimitPlus.Name = "lbStatusLimitPlus";
            this.lbStatusLimitPlus.Size = new System.Drawing.Size(105, 37);
            this.lbStatusLimitPlus.TabIndex = 1186;
            this.lbStatusLimitPlus.Text = "LIMIT +";
            this.lbStatusLimitPlus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusServoOn
            // 
            this.lbStatusServoOn.BackColor = System.Drawing.Color.DimGray;
            this.lbStatusServoOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusServoOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusServoOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusServoOn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbStatusServoOn.Location = new System.Drawing.Point(1, 1);
            this.lbStatusServoOn.Name = "lbStatusServoOn";
            this.lbStatusServoOn.Size = new System.Drawing.Size(105, 37);
            this.lbStatusServoOn.TabIndex = 1184;
            this.lbStatusServoOn.Text = "SERVO\r\nON";
            this.lbStatusServoOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Transparent;
            this.metroTile1.Location = new System.Drawing.Point(-508, 609);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(279, 90);
            this.metroTile1.Style = MetroFramework.MetroColorStyle.Red;
            this.metroTile1.TabIndex = 1741;
            this.metroTile1.Text = "STOP";
            this.metroTile1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.metroTile1.TileImage = global::IntelligentFactory.Properties.Resources.delete_50;
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroTile1.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile1.UseSelectable = true;
            this.metroTile1.UseTileImage = true;
            // 
            // btnSave
            // 
            this.btnSave.ActiveControl = null;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Location = new System.Drawing.Point(22, 562);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(1268, 58);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1742;
            this.btnSave.Text = "SAVE";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.TileImage = global::IntelligentFactory.Properties.Resources.save_50;
            this.btnSave.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnSave.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnSave.UseSelectable = true;
            this.btnSave.UseTileImage = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // metroLabel4
            // 
            this.metroLabel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel4.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel4.ForeColor = System.Drawing.Color.White;
            this.metroLabel4.Location = new System.Drawing.Point(22, 439);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(736, 35);
            this.metroLabel4.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel4.TabIndex = 1743;
            this.metroLabel4.Text = "PARAMETER";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel4.UseCustomBackColor = true;
            this.metroLabel4.UseCustomForeColor = true;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Location = new System.Drawing.Point(446, 475);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(155, 36);
            this.label10.TabIndex = 1746;
            this.label10.Text = "SLOW SPEED (mm/s)";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroLabel5
            // 
            this.metroLabel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel5.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel5.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel5.ForeColor = System.Drawing.Color.White;
            this.metroLabel5.Location = new System.Drawing.Point(333, 475);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(112, 72);
            this.metroLabel5.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel5.TabIndex = 1745;
            this.metroLabel5.Text = "SPEED";
            this.metroLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel5.UseCustomBackColor = true;
            this.metroLabel5.UseCustomForeColor = true;
            // 
            // metroTextBox1
            // 
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(122, 1);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.DisplayIcon = true;
            this.metroTextBox1.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.metroTextBox1.Lines = new string[] {
        "10"};
            this.metroTextBox1.Location = new System.Drawing.Point(446, 512);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.ShortcutsEnabled = true;
            this.metroTextBox1.Size = new System.Drawing.Size(156, 35);
            this.metroTextBox1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTextBox1.TabIndex = 1744;
            this.metroTextBox1.Text = "10";
            this.metroTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label11.Location = new System.Drawing.Point(602, 475);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(156, 36);
            this.label11.TabIndex = 1748;
            this.label11.Text = "FAST SPEED (mm/s)";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroTextBox2
            // 
            // 
            // 
            // 
            this.metroTextBox2.CustomButton.Image = null;
            this.metroTextBox2.CustomButton.Location = new System.Drawing.Point(123, 1);
            this.metroTextBox2.CustomButton.Name = "";
            this.metroTextBox2.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.metroTextBox2.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox2.CustomButton.TabIndex = 1;
            this.metroTextBox2.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox2.CustomButton.UseSelectable = true;
            this.metroTextBox2.CustomButton.Visible = false;
            this.metroTextBox2.DisplayIcon = true;
            this.metroTextBox2.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.metroTextBox2.Lines = new string[] {
        "10"};
            this.metroTextBox2.Location = new System.Drawing.Point(602, 512);
            this.metroTextBox2.MaxLength = 32767;
            this.metroTextBox2.Name = "metroTextBox2";
            this.metroTextBox2.PasswordChar = '\0';
            this.metroTextBox2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox2.SelectedText = "";
            this.metroTextBox2.SelectionLength = 0;
            this.metroTextBox2.SelectionStart = 0;
            this.metroTextBox2.ShortcutsEnabled = true;
            this.metroTextBox2.Size = new System.Drawing.Size(157, 35);
            this.metroTextBox2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTextBox2.TabIndex = 1747;
            this.metroTextBox2.Text = "10";
            this.metroTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.metroTextBox2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox2.UseSelectable = true;
            this.metroTextBox2.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox2.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel6
            // 
            this.metroLabel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel6.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel6.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel6.ForeColor = System.Drawing.Color.White;
            this.metroLabel6.Location = new System.Drawing.Point(22, 475);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(112, 72);
            this.metroLabel6.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel6.TabIndex = 1750;
            this.metroLabel6.Text = "POSITION";
            this.metroLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel6.UseCustomBackColor = true;
            this.metroLabel6.UseCustomForeColor = true;
            // 
            // metroTextBox3
            // 
            // 
            // 
            // 
            this.metroTextBox3.CustomButton.Image = null;
            this.metroTextBox3.CustomButton.Location = new System.Drawing.Point(164, 1);
            this.metroTextBox3.CustomButton.Name = "";
            this.metroTextBox3.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.metroTextBox3.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox3.CustomButton.TabIndex = 1;
            this.metroTextBox3.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox3.CustomButton.UseSelectable = true;
            this.metroTextBox3.CustomButton.Visible = false;
            this.metroTextBox3.DisplayIcon = true;
            this.metroTextBox3.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.metroTextBox3.Lines = new string[] {
        "10"};
            this.metroTextBox3.Location = new System.Drawing.Point(135, 475);
            this.metroTextBox3.MaxLength = 32767;
            this.metroTextBox3.Name = "metroTextBox3";
            this.metroTextBox3.PasswordChar = '\0';
            this.metroTextBox3.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox3.SelectedText = "";
            this.metroTextBox3.SelectionLength = 0;
            this.metroTextBox3.SelectionStart = 0;
            this.metroTextBox3.ShortcutsEnabled = true;
            this.metroTextBox3.Size = new System.Drawing.Size(198, 35);
            this.metroTextBox3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroTextBox3.TabIndex = 1749;
            this.metroTextBox3.Text = "10";
            this.metroTextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.metroTextBox3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox3.UseSelectable = true;
            this.metroTextBox3.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox3.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // Move_Pos
            // 
            this.Move_Pos.ActiveControl = null;
            this.Move_Pos.BackColor = System.Drawing.Color.Transparent;
            this.Move_Pos.Location = new System.Drawing.Point(135, 511);
            this.Move_Pos.Name = "Move_Pos";
            this.Move_Pos.Size = new System.Drawing.Size(197, 36);
            this.Move_Pos.Style = MetroFramework.MetroColorStyle.Yellow;
            this.Move_Pos.TabIndex = 1751;
            this.Move_Pos.Text = "MOVE";
            this.Move_Pos.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.Move_Pos.TileImage = global::IntelligentFactory.Properties.Resources.all_out_50;
            this.Move_Pos.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.Move_Pos.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.Move_Pos.UseSelectable = true;
            this.Move_Pos.UseTileImage = true;
            this.Move_Pos.Click += new System.EventHandler(this.bntMove_Pos);
            // 
            // tcIO
            // 
            this.tcIO.Controls.Add(this.metroTabPage10);
            this.tcIO.Controls.Add(this.metroTabPage1);
            this.tcIO.Location = new System.Drawing.Point(758, 86);
            this.tcIO.Name = "tcIO";
            this.tcIO.SelectedIndex = 0;
            this.tcIO.Size = new System.Drawing.Size(540, 481);
            this.tcIO.TabIndex = 1752;
            this.tcIO.UseSelectable = true;
            this.tcIO.Visible = false;
            // 
            // metroTabPage10
            // 
            this.metroTabPage10.Controls.Add(this.label13);
            this.metroTabPage10.Controls.Add(this.label12);
            this.metroTabPage10.Controls.Add(this.label4);
            this.metroTabPage10.Controls.Add(this.label2);
            this.metroTabPage10.Controls.Add(this.lbInWorkCountSensor);
            this.metroTabPage10.Controls.Add(this.ucCylinder6);
            this.metroTabPage10.Controls.Add(this.ucCylinder5);
            this.metroTabPage10.Controls.Add(this.ucCylinder4);
            this.metroTabPage10.Controls.Add(this.ucCylinder3);
            this.metroTabPage10.Controls.Add(this.ucCylinder2);
            this.metroTabPage10.Controls.Add(this.ucCylinder1);
            this.metroTabPage10.HorizontalScrollbarBarColor = true;
            this.metroTabPage10.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage10.HorizontalScrollbarSize = 10;
            this.metroTabPage10.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage10.Name = "metroTabPage10";
            this.metroTabPage10.Size = new System.Drawing.Size(532, 439);
            this.metroTabPage10.TabIndex = 1;
            this.metroTabPage10.Text = "LOT TRANSFER";
            this.metroTabPage10.VerticalScrollbarBarColor = true;
            this.metroTabPage10.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage10.VerticalScrollbarSize = 10;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.DimGray;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label13.Location = new System.Drawing.Point(-1, 397);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(529, 35);
            this.label13.TabIndex = 1344;
            this.label13.Text = "DI_00_XXXXXXXXX";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.DimGray;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label12.Location = new System.Drawing.Point(-1, 361);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(529, 35);
            this.label12.TabIndex = 1343;
            this.label12.Text = "DI_00_XXXXXXXXX";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.DimGray;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label4.Location = new System.Drawing.Point(-1, 325);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(529, 35);
            this.label4.TabIndex = 1342;
            this.label4.Text = "DI_00_XXXXXXXXX";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DimGray;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label2.Location = new System.Drawing.Point(-1, 289);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(529, 35);
            this.label2.TabIndex = 1341;
            this.label2.Text = "DI_00_XXXXXXXXX";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInWorkCountSensor
            // 
            this.lbInWorkCountSensor.BackColor = System.Drawing.Color.DimGray;
            this.lbInWorkCountSensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbInWorkCountSensor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbInWorkCountSensor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbInWorkCountSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbInWorkCountSensor.Location = new System.Drawing.Point(-1, 253);
            this.lbInWorkCountSensor.Name = "lbInWorkCountSensor";
            this.lbInWorkCountSensor.Size = new System.Drawing.Size(529, 35);
            this.lbInWorkCountSensor.TabIndex = 1191;
            this.lbInWorkCountSensor.Text = "00";
            this.lbInWorkCountSensor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucCylinder6
            // 
            this.ucCylinder6.BackColor = System.Drawing.Color.Black;
            this.ucCylinder6.Location = new System.Drawing.Point(353, 127);
            this.ucCylinder6.Name = "ucCylinder6";
            this.ucCylinder6.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder6.TabIndex = 1340;
            // 
            // ucCylinder5
            // 
            this.ucCylinder5.BackColor = System.Drawing.Color.Black;
            this.ucCylinder5.Location = new System.Drawing.Point(177, 127);
            this.ucCylinder5.Name = "ucCylinder5";
            this.ucCylinder5.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder5.TabIndex = 1339;
            // 
            // ucCylinder4
            // 
            this.ucCylinder4.BackColor = System.Drawing.Color.Black;
            this.ucCylinder4.Location = new System.Drawing.Point(1, 127);
            this.ucCylinder4.Name = "ucCylinder4";
            this.ucCylinder4.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder4.TabIndex = 1338;
            // 
            // ucCylinder3
            // 
            this.ucCylinder3.BackColor = System.Drawing.Color.Black;
            this.ucCylinder3.Location = new System.Drawing.Point(353, 1);
            this.ucCylinder3.Name = "ucCylinder3";
            this.ucCylinder3.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder3.TabIndex = 1335;
            // 
            // ucCylinder2
            // 
            this.ucCylinder2.BackColor = System.Drawing.Color.Black;
            this.ucCylinder2.Location = new System.Drawing.Point(177, 1);
            this.ucCylinder2.Name = "ucCylinder2";
            this.ucCylinder2.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder2.TabIndex = 1334;
            // 
            // ucCylinder1
            // 
            this.ucCylinder1.BackColor = System.Drawing.Color.Black;
            this.ucCylinder1.Location = new System.Drawing.Point(1, 1);
            this.ucCylinder1.Name = "ucCylinder1";
            this.ucCylinder1.Size = new System.Drawing.Size(175, 125);
            this.ucCylinder1.TabIndex = 1333;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(532, 439);
            this.metroTabPage1.TabIndex = 2;
            this.metroTabPage1.Text = "metroTabPage1";
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // metroLabel8
            // 
            this.metroLabel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel8.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel8.ForeColor = System.Drawing.Color.White;
            this.metroLabel8.Location = new System.Drawing.Point(761, 60);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(537, 35);
            this.metroLabel8.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel8.TabIndex = 1753;
            this.metroLabel8.Text = "I/O";
            this.metroLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel8.UseCustomBackColor = true;
            this.metroLabel8.UseCustomForeColor = true;
            // 
            // FormViewer_MotionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 644);
            this.Controls.Add(this.metroLabel8);
            this.Controls.Add(this.Move_Pos);
            this.Controls.Add(this.metroLabel6);
            this.Controls.Add(this.metroTextBox3);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.metroTextBox2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.metroTile1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAxisStop);
            this.Controls.Add(this.btnMotorServoOn);
            this.Controls.Add(this.metroLabel12);
            this.Controls.Add(this.btnJogPlus);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.lbStatusGPIO1);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.tcIO);
            this.Name = "FormViewer_MotionControl";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "LOT OPEN";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormViewer_LotOpen_FormClosing);
            this.Load += new System.EventHandler(this.FormViewer_MotionControl_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.panel2.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.tcIO.ResumeLayout(false);
            this.metroTabPage10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerView;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroTile btnAxisStop;
        private MetroFramework.Controls.MetroTile btnMotorServoOn;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        private MetroFramework.Controls.MetroTile btnJogPlus;
        private System.Windows.Forms.Panel panel2;
        private MetroFramework.Controls.MetroTile metroTile5;
        private MetroFramework.Controls.MetroTile metroTile4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private MetroFramework.Controls.MetroTile btnJogMinus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private MetroFramework.Controls.MetroTextBox tbAbsoluteSpeed;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroTextBox tbAbsoluteTarget;
        private MetroFramework.Controls.MetroTextBox tbStepSpeed;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroTextBox tbStepmm;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTextBox tbJogSpeed;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel lbStatusGPIO1;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTile btnStartHome;
        private System.Windows.Forms.Label lbStatusServoAlarm;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbCommandPulse;
        private System.Windows.Forms.Label lbStatusInPosition;
        private System.Windows.Forms.Label lbStatusMotioning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbStatusLimitMinus;
        private System.Windows.Forms.Label lbStatusHomeComplete;
        private System.Windows.Forms.Label lbActualPulse;
        private System.Windows.Forms.Label lbStatusHome;
        private System.Windows.Forms.Label lbStatusLimitPlus;
        private System.Windows.Forms.Label lbStatusServoOn;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroTile btnSave;
        private MetroFramework.Controls.MetroTile Move_Pos;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroTextBox metroTextBox3;
        private System.Windows.Forms.Label label11;
        private MetroFramework.Controls.MetroTextBox metroTextBox2;
        private System.Windows.Forms.Label label10;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroTextBox metroTextBox1;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroTabControl tcIO;
        private MetroFramework.Controls.MetroTabPage metroTabPage10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbInWorkCountSensor;
        private ucCylinder ucCylinder6;
        private ucCylinder ucCylinder5;
        private ucCylinder ucCylinder4;
        private ucCylinder ucCylinder3;
        private ucCylinder ucCylinder2;
        private ucCylinder ucCylinder1;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
    }
}