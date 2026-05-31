namespace IntelligentFactory
{
    partial class FormSetting_TowerLamp
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
            this.btnCancel = new MetroFramework.Controls.MetroTile();
            this.btnSave = new MetroFramework.Controls.MetroTile();
            this.label31 = new System.Windows.Forms.Label();
            this.lbStatusRed = new System.Windows.Forms.Label();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.lbStatusYellow = new System.Windows.Forms.Label();
            this.lbStatusGreen = new System.Windows.Forms.Label();
            this.lbStatusBuzzer = new System.Windows.Forms.Label();
            this.btnRedOn = new MetroFramework.Controls.MetroTile();
            this.btnRedOff = new MetroFramework.Controls.MetroTile();
            this.btnRedBlink200ms = new MetroFramework.Controls.MetroTile();
            this.btnRedBlink500ms = new MetroFramework.Controls.MetroTile();
            this.btnYellowBlink500ms = new MetroFramework.Controls.MetroTile();
            this.btnYellowBlink200ms = new MetroFramework.Controls.MetroTile();
            this.btnYellowOff = new MetroFramework.Controls.MetroTile();
            this.btnYellowOn = new MetroFramework.Controls.MetroTile();
            this.btnBuzzerBlink500ms = new MetroFramework.Controls.MetroTile();
            this.btnBuzzerBlink200ms = new MetroFramework.Controls.MetroTile();
            this.btnBuzzerOff = new MetroFramework.Controls.MetroTile();
            this.btnBuzzerOn = new MetroFramework.Controls.MetroTile();
            this.btnGreenBlink500ms = new MetroFramework.Controls.MetroTile();
            this.btnGreenBlink200ms = new MetroFramework.Controls.MetroTile();
            this.btnGreenOff = new MetroFramework.Controls.MetroTile();
            this.btnGreenOn = new MetroFramework.Controls.MetroTile();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.timerR = new System.Windows.Forms.Timer(this.components);
            this.timerG = new System.Windows.Forms.Timer(this.components);
            this.timerY = new System.Windows.Forms.Timer(this.components);
            this.timerB = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.SuspendLayout();
            // 
            // timerView
            // 
            this.timerView.Enabled = true;
            this.timerView.Interval = 1000;
            this.timerView.Tick += new System.EventHandler(this.timerView_Tick);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Lime;
            // 
            // btnCancel
            // 
            this.btnCancel.ActiveControl = null;
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Location = new System.Drawing.Point(455, 407);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(196, 58);
            this.btnCancel.Style = MetroFramework.MetroColorStyle.Red;
            this.btnCancel.TabIndex = 1750;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.TileImage = global::IntelligentFactory.Properties.Resources.delete_50;
            this.btnCancel.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnCancel.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.UseTileImage = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.ActiveControl = null;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.Location = new System.Drawing.Point(259, 407);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(195, 58);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1749;
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
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label31.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label31.Location = new System.Drawing.Point(22, 96);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(125, 30);
            this.label31.TabIndex = 1655;
            this.label31.Text = "INITIALIZE";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label31.Click += new System.EventHandler(this.OnClickSituation);
            // 
            // lbStatusRed
            // 
            this.lbStatusRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbStatusRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusRed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusRed.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lbStatusRed.ForeColor = System.Drawing.Color.White;
            this.lbStatusRed.Location = new System.Drawing.Point(22, 163);
            this.lbStatusRed.Name = "lbStatusRed";
            this.lbStatusRed.Size = new System.Drawing.Size(125, 60);
            this.lbStatusRed.TabIndex = 1642;
            this.lbStatusRed.Text = "RED";
            this.lbStatusRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(23, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(628, 35);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1658;
            this.metroLabel1.Text = "SITUATION";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // lbStatusYellow
            // 
            this.lbStatusYellow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lbStatusYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusYellow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusYellow.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lbStatusYellow.ForeColor = System.Drawing.Color.White;
            this.lbStatusYellow.Location = new System.Drawing.Point(22, 224);
            this.lbStatusYellow.Name = "lbStatusYellow";
            this.lbStatusYellow.Size = new System.Drawing.Size(125, 60);
            this.lbStatusYellow.TabIndex = 1667;
            this.lbStatusYellow.Text = "YELLOW";
            this.lbStatusYellow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusGreen
            // 
            this.lbStatusGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lbStatusGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusGreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusGreen.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lbStatusGreen.ForeColor = System.Drawing.Color.White;
            this.lbStatusGreen.Location = new System.Drawing.Point(22, 285);
            this.lbStatusGreen.Name = "lbStatusGreen";
            this.lbStatusGreen.Size = new System.Drawing.Size(125, 60);
            this.lbStatusGreen.TabIndex = 1668;
            this.lbStatusGreen.Text = "GREEN";
            this.lbStatusGreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatusBuzzer
            // 
            this.lbStatusBuzzer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbStatusBuzzer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatusBuzzer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatusBuzzer.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lbStatusBuzzer.ForeColor = System.Drawing.Color.White;
            this.lbStatusBuzzer.Location = new System.Drawing.Point(22, 346);
            this.lbStatusBuzzer.Name = "lbStatusBuzzer";
            this.lbStatusBuzzer.Size = new System.Drawing.Size(125, 60);
            this.lbStatusBuzzer.TabIndex = 1669;
            this.lbStatusBuzzer.Text = "BUZZER";
            this.lbStatusBuzzer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRedOn
            // 
            this.btnRedOn.ActiveControl = null;
            this.btnRedOn.BackColor = System.Drawing.Color.DimGray;
            this.btnRedOn.Location = new System.Drawing.Point(148, 163);
            this.btnRedOn.Name = "btnRedOn";
            this.btnRedOn.Size = new System.Drawing.Size(125, 60);
            this.btnRedOn.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRedOn.TabIndex = 1751;
            this.btnRedOn.Text = "ON";
            this.btnRedOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRedOn.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRedOn.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRedOn.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnRedOn.UseCustomBackColor = true;
            this.btnRedOn.UseSelectable = true;
            this.btnRedOn.UseTileImage = true;
            this.btnRedOn.Click += new System.EventHandler(this.OnClickRed);
            // 
            // btnRedOff
            // 
            this.btnRedOff.ActiveControl = null;
            this.btnRedOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnRedOff.Location = new System.Drawing.Point(274, 163);
            this.btnRedOff.Name = "btnRedOff";
            this.btnRedOff.Size = new System.Drawing.Size(125, 60);
            this.btnRedOff.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRedOff.TabIndex = 1752;
            this.btnRedOff.Text = "OFF";
            this.btnRedOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRedOff.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRedOff.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRedOff.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnRedOff.UseCustomBackColor = true;
            this.btnRedOff.UseSelectable = true;
            this.btnRedOff.UseTileImage = true;
            this.btnRedOff.Click += new System.EventHandler(this.OnClickRed);
            // 
            // btnRedBlink200ms
            // 
            this.btnRedBlink200ms.ActiveControl = null;
            this.btnRedBlink200ms.BackColor = System.Drawing.Color.DimGray;
            this.btnRedBlink200ms.Location = new System.Drawing.Point(400, 163);
            this.btnRedBlink200ms.Name = "btnRedBlink200ms";
            this.btnRedBlink200ms.Size = new System.Drawing.Size(125, 60);
            this.btnRedBlink200ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRedBlink200ms.TabIndex = 1753;
            this.btnRedBlink200ms.Text = "200 ms";
            this.btnRedBlink200ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRedBlink200ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRedBlink200ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRedBlink200ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnRedBlink200ms.UseCustomBackColor = true;
            this.btnRedBlink200ms.UseSelectable = true;
            this.btnRedBlink200ms.UseTileImage = true;
            this.btnRedBlink200ms.Click += new System.EventHandler(this.OnClickRed);
            // 
            // btnRedBlink500ms
            // 
            this.btnRedBlink500ms.ActiveControl = null;
            this.btnRedBlink500ms.BackColor = System.Drawing.Color.DimGray;
            this.btnRedBlink500ms.Location = new System.Drawing.Point(526, 163);
            this.btnRedBlink500ms.Name = "btnRedBlink500ms";
            this.btnRedBlink500ms.Size = new System.Drawing.Size(125, 60);
            this.btnRedBlink500ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnRedBlink500ms.TabIndex = 1754;
            this.btnRedBlink500ms.Text = "500 ms";
            this.btnRedBlink500ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRedBlink500ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRedBlink500ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnRedBlink500ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnRedBlink500ms.UseCustomBackColor = true;
            this.btnRedBlink500ms.UseSelectable = true;
            this.btnRedBlink500ms.UseTileImage = true;
            this.btnRedBlink500ms.Click += new System.EventHandler(this.OnClickRed);
            // 
            // btnYellowBlink500ms
            // 
            this.btnYellowBlink500ms.ActiveControl = null;
            this.btnYellowBlink500ms.BackColor = System.Drawing.Color.DimGray;
            this.btnYellowBlink500ms.Location = new System.Drawing.Point(526, 224);
            this.btnYellowBlink500ms.Name = "btnYellowBlink500ms";
            this.btnYellowBlink500ms.Size = new System.Drawing.Size(125, 60);
            this.btnYellowBlink500ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnYellowBlink500ms.TabIndex = 1758;
            this.btnYellowBlink500ms.Text = "500 ms";
            this.btnYellowBlink500ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnYellowBlink500ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYellowBlink500ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnYellowBlink500ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnYellowBlink500ms.UseCustomBackColor = true;
            this.btnYellowBlink500ms.UseSelectable = true;
            this.btnYellowBlink500ms.UseTileImage = true;
            this.btnYellowBlink500ms.Click += new System.EventHandler(this.OnClickYellow);
            // 
            // btnYellowBlink200ms
            // 
            this.btnYellowBlink200ms.ActiveControl = null;
            this.btnYellowBlink200ms.BackColor = System.Drawing.Color.DimGray;
            this.btnYellowBlink200ms.Location = new System.Drawing.Point(400, 224);
            this.btnYellowBlink200ms.Name = "btnYellowBlink200ms";
            this.btnYellowBlink200ms.Size = new System.Drawing.Size(125, 60);
            this.btnYellowBlink200ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnYellowBlink200ms.TabIndex = 1757;
            this.btnYellowBlink200ms.Text = "200 ms";
            this.btnYellowBlink200ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnYellowBlink200ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYellowBlink200ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnYellowBlink200ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnYellowBlink200ms.UseCustomBackColor = true;
            this.btnYellowBlink200ms.UseSelectable = true;
            this.btnYellowBlink200ms.UseTileImage = true;
            this.btnYellowBlink200ms.Click += new System.EventHandler(this.OnClickYellow);
            // 
            // btnYellowOff
            // 
            this.btnYellowOff.ActiveControl = null;
            this.btnYellowOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnYellowOff.Location = new System.Drawing.Point(274, 224);
            this.btnYellowOff.Name = "btnYellowOff";
            this.btnYellowOff.Size = new System.Drawing.Size(125, 60);
            this.btnYellowOff.Style = MetroFramework.MetroColorStyle.Red;
            this.btnYellowOff.TabIndex = 1756;
            this.btnYellowOff.Text = "OFF";
            this.btnYellowOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnYellowOff.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYellowOff.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnYellowOff.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnYellowOff.UseCustomBackColor = true;
            this.btnYellowOff.UseSelectable = true;
            this.btnYellowOff.UseTileImage = true;
            this.btnYellowOff.Click += new System.EventHandler(this.OnClickYellow);
            // 
            // btnYellowOn
            // 
            this.btnYellowOn.ActiveControl = null;
            this.btnYellowOn.BackColor = System.Drawing.Color.DimGray;
            this.btnYellowOn.Location = new System.Drawing.Point(148, 224);
            this.btnYellowOn.Name = "btnYellowOn";
            this.btnYellowOn.Size = new System.Drawing.Size(125, 60);
            this.btnYellowOn.Style = MetroFramework.MetroColorStyle.Red;
            this.btnYellowOn.TabIndex = 1755;
            this.btnYellowOn.Text = "ON";
            this.btnYellowOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnYellowOn.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYellowOn.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnYellowOn.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnYellowOn.UseCustomBackColor = true;
            this.btnYellowOn.UseSelectable = true;
            this.btnYellowOn.UseTileImage = true;
            this.btnYellowOn.Click += new System.EventHandler(this.OnClickYellow);
            // 
            // btnBuzzerBlink500ms
            // 
            this.btnBuzzerBlink500ms.ActiveControl = null;
            this.btnBuzzerBlink500ms.BackColor = System.Drawing.Color.DimGray;
            this.btnBuzzerBlink500ms.Location = new System.Drawing.Point(526, 346);
            this.btnBuzzerBlink500ms.Name = "btnBuzzerBlink500ms";
            this.btnBuzzerBlink500ms.Size = new System.Drawing.Size(125, 60);
            this.btnBuzzerBlink500ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnBuzzerBlink500ms.TabIndex = 1766;
            this.btnBuzzerBlink500ms.Text = "500 ms";
            this.btnBuzzerBlink500ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnBuzzerBlink500ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuzzerBlink500ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnBuzzerBlink500ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnBuzzerBlink500ms.UseCustomBackColor = true;
            this.btnBuzzerBlink500ms.UseSelectable = true;
            this.btnBuzzerBlink500ms.UseTileImage = true;
            this.btnBuzzerBlink500ms.Click += new System.EventHandler(this.OnClickBuzzer);
            // 
            // btnBuzzerBlink200ms
            // 
            this.btnBuzzerBlink200ms.ActiveControl = null;
            this.btnBuzzerBlink200ms.BackColor = System.Drawing.Color.DimGray;
            this.btnBuzzerBlink200ms.Location = new System.Drawing.Point(400, 346);
            this.btnBuzzerBlink200ms.Name = "btnBuzzerBlink200ms";
            this.btnBuzzerBlink200ms.Size = new System.Drawing.Size(125, 60);
            this.btnBuzzerBlink200ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnBuzzerBlink200ms.TabIndex = 1765;
            this.btnBuzzerBlink200ms.Text = "200 ms";
            this.btnBuzzerBlink200ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnBuzzerBlink200ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuzzerBlink200ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnBuzzerBlink200ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnBuzzerBlink200ms.UseCustomBackColor = true;
            this.btnBuzzerBlink200ms.UseSelectable = true;
            this.btnBuzzerBlink200ms.UseTileImage = true;
            this.btnBuzzerBlink200ms.Click += new System.EventHandler(this.OnClickBuzzer);
            // 
            // btnBuzzerOff
            // 
            this.btnBuzzerOff.ActiveControl = null;
            this.btnBuzzerOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnBuzzerOff.Location = new System.Drawing.Point(274, 346);
            this.btnBuzzerOff.Name = "btnBuzzerOff";
            this.btnBuzzerOff.Size = new System.Drawing.Size(125, 60);
            this.btnBuzzerOff.Style = MetroFramework.MetroColorStyle.Red;
            this.btnBuzzerOff.TabIndex = 1764;
            this.btnBuzzerOff.Text = "OFF";
            this.btnBuzzerOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnBuzzerOff.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuzzerOff.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnBuzzerOff.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnBuzzerOff.UseCustomBackColor = true;
            this.btnBuzzerOff.UseSelectable = true;
            this.btnBuzzerOff.UseTileImage = true;
            this.btnBuzzerOff.Click += new System.EventHandler(this.OnClickBuzzer);
            // 
            // btnBuzzerOn
            // 
            this.btnBuzzerOn.ActiveControl = null;
            this.btnBuzzerOn.BackColor = System.Drawing.Color.DimGray;
            this.btnBuzzerOn.Location = new System.Drawing.Point(148, 346);
            this.btnBuzzerOn.Name = "btnBuzzerOn";
            this.btnBuzzerOn.Size = new System.Drawing.Size(125, 60);
            this.btnBuzzerOn.Style = MetroFramework.MetroColorStyle.Red;
            this.btnBuzzerOn.TabIndex = 1763;
            this.btnBuzzerOn.Text = "ON";
            this.btnBuzzerOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnBuzzerOn.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuzzerOn.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnBuzzerOn.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnBuzzerOn.UseCustomBackColor = true;
            this.btnBuzzerOn.UseSelectable = true;
            this.btnBuzzerOn.UseTileImage = true;
            this.btnBuzzerOn.Click += new System.EventHandler(this.OnClickBuzzer);
            // 
            // btnGreenBlink500ms
            // 
            this.btnGreenBlink500ms.ActiveControl = null;
            this.btnGreenBlink500ms.BackColor = System.Drawing.Color.DimGray;
            this.btnGreenBlink500ms.Location = new System.Drawing.Point(526, 285);
            this.btnGreenBlink500ms.Name = "btnGreenBlink500ms";
            this.btnGreenBlink500ms.Size = new System.Drawing.Size(125, 60);
            this.btnGreenBlink500ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnGreenBlink500ms.TabIndex = 1762;
            this.btnGreenBlink500ms.Text = "500 ms";
            this.btnGreenBlink500ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnGreenBlink500ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGreenBlink500ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnGreenBlink500ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnGreenBlink500ms.UseCustomBackColor = true;
            this.btnGreenBlink500ms.UseSelectable = true;
            this.btnGreenBlink500ms.UseTileImage = true;
            this.btnGreenBlink500ms.Click += new System.EventHandler(this.OnClickGreen);
            // 
            // btnGreenBlink200ms
            // 
            this.btnGreenBlink200ms.ActiveControl = null;
            this.btnGreenBlink200ms.BackColor = System.Drawing.Color.DimGray;
            this.btnGreenBlink200ms.Location = new System.Drawing.Point(400, 285);
            this.btnGreenBlink200ms.Name = "btnGreenBlink200ms";
            this.btnGreenBlink200ms.Size = new System.Drawing.Size(125, 60);
            this.btnGreenBlink200ms.Style = MetroFramework.MetroColorStyle.Red;
            this.btnGreenBlink200ms.TabIndex = 1761;
            this.btnGreenBlink200ms.Text = "200 ms";
            this.btnGreenBlink200ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnGreenBlink200ms.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGreenBlink200ms.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnGreenBlink200ms.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnGreenBlink200ms.UseCustomBackColor = true;
            this.btnGreenBlink200ms.UseSelectable = true;
            this.btnGreenBlink200ms.UseTileImage = true;
            this.btnGreenBlink200ms.Click += new System.EventHandler(this.OnClickGreen);
            // 
            // btnGreenOff
            // 
            this.btnGreenOff.ActiveControl = null;
            this.btnGreenOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.btnGreenOff.Location = new System.Drawing.Point(274, 285);
            this.btnGreenOff.Name = "btnGreenOff";
            this.btnGreenOff.Size = new System.Drawing.Size(125, 60);
            this.btnGreenOff.Style = MetroFramework.MetroColorStyle.Red;
            this.btnGreenOff.TabIndex = 1760;
            this.btnGreenOff.Text = "OFF";
            this.btnGreenOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnGreenOff.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGreenOff.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnGreenOff.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnGreenOff.UseCustomBackColor = true;
            this.btnGreenOff.UseSelectable = true;
            this.btnGreenOff.UseTileImage = true;
            this.btnGreenOff.Click += new System.EventHandler(this.OnClickGreen);
            // 
            // btnGreenOn
            // 
            this.btnGreenOn.ActiveControl = null;
            this.btnGreenOn.BackColor = System.Drawing.Color.DimGray;
            this.btnGreenOn.Location = new System.Drawing.Point(148, 285);
            this.btnGreenOn.Name = "btnGreenOn";
            this.btnGreenOn.Size = new System.Drawing.Size(125, 60);
            this.btnGreenOn.Style = MetroFramework.MetroColorStyle.Red;
            this.btnGreenOn.TabIndex = 1759;
            this.btnGreenOn.Text = "ON";
            this.btnGreenOn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnGreenOn.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGreenOn.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnGreenOn.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.btnGreenOn.UseCustomBackColor = true;
            this.btnGreenOn.UseSelectable = true;
            this.btnGreenOn.UseTileImage = true;
            this.btnGreenOn.Click += new System.EventHandler(this.OnClickGreen);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label1.Location = new System.Drawing.Point(148, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 30);
            this.label1.TabIndex = 1767;
            this.label1.Text = "LOT END";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.OnClickSituation);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label2.Location = new System.Drawing.Point(274, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 30);
            this.label2.TabIndex = 1768;
            this.label2.Text = "AUTO";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.OnClickSituation);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label6.Location = new System.Drawing.Point(400, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 30);
            this.label6.TabIndex = 1769;
            this.label6.Text = "STOP";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Click += new System.EventHandler(this.OnClickSituation);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label7.Location = new System.Drawing.Point(526, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(125, 30);
            this.label7.TabIndex = 1770;
            this.label7.Text = "ALARM";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.Click += new System.EventHandler(this.OnClickSituation);
            // 
            // metroLabel2
            // 
            this.metroLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel2.ForeColor = System.Drawing.Color.White;
            this.metroLabel2.Location = new System.Drawing.Point(22, 127);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(628, 35);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel2.TabIndex = 1772;
            this.metroLabel2.Text = "SETTING";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.UseCustomBackColor = true;
            this.metroLabel2.UseCustomForeColor = true;
            // 
            // timerR
            // 
            this.timerR.Enabled = true;
            this.timerR.Tick += new System.EventHandler(this.timerR_Tick);
            // 
            // timerG
            // 
            this.timerG.Enabled = true;
            this.timerG.Tick += new System.EventHandler(this.timerG_Tick);
            // 
            // timerY
            // 
            this.timerY.Enabled = true;
            this.timerY.Tick += new System.EventHandler(this.timerY_Tick);
            // 
            // timerB
            // 
            this.timerB.Enabled = true;
            // 
            // FormSetting_TowerLamp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 493);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBuzzerBlink500ms);
            this.Controls.Add(this.btnBuzzerBlink200ms);
            this.Controls.Add(this.btnBuzzerOff);
            this.Controls.Add(this.btnBuzzerOn);
            this.Controls.Add(this.btnGreenBlink500ms);
            this.Controls.Add(this.btnGreenBlink200ms);
            this.Controls.Add(this.btnGreenOff);
            this.Controls.Add(this.btnGreenOn);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnYellowBlink500ms);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnYellowBlink200ms);
            this.Controls.Add(this.btnYellowOff);
            this.Controls.Add(this.btnYellowOn);
            this.Controls.Add(this.btnRedBlink500ms);
            this.Controls.Add(this.btnRedBlink200ms);
            this.Controls.Add(this.btnRedOff);
            this.Controls.Add(this.btnRedOn);
            this.Controls.Add(this.lbStatusBuzzer);
            this.Controls.Add(this.lbStatusGreen);
            this.Controls.Add(this.lbStatusYellow);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.lbStatusRed);
            this.Name = "FormSetting_TowerLamp";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "TOWER LAMP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetting_Delay_FormClosing);
            this.Load += new System.EventHandler(this.FormSetting_TowerLamp_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerView;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lbStatusRed;
        private MetroFramework.Controls.MetroTile btnCancel;
        private MetroFramework.Controls.MetroTile btnSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private MetroFramework.Controls.MetroTile btnBuzzerBlink500ms;
        private MetroFramework.Controls.MetroTile btnBuzzerBlink200ms;
        private MetroFramework.Controls.MetroTile btnBuzzerOff;
        private MetroFramework.Controls.MetroTile btnBuzzerOn;
        private MetroFramework.Controls.MetroTile btnGreenBlink500ms;
        private MetroFramework.Controls.MetroTile btnGreenBlink200ms;
        private MetroFramework.Controls.MetroTile btnGreenOff;
        private MetroFramework.Controls.MetroTile btnGreenOn;
        private MetroFramework.Controls.MetroTile btnYellowBlink500ms;
        private MetroFramework.Controls.MetroTile btnYellowBlink200ms;
        private MetroFramework.Controls.MetroTile btnYellowOff;
        private MetroFramework.Controls.MetroTile btnYellowOn;
        private MetroFramework.Controls.MetroTile btnRedBlink500ms;
        private MetroFramework.Controls.MetroTile btnRedBlink200ms;
        private MetroFramework.Controls.MetroTile btnRedOff;
        private MetroFramework.Controls.MetroTile btnRedOn;
        private System.Windows.Forms.Label lbStatusBuzzer;
        private System.Windows.Forms.Label lbStatusGreen;
        private System.Windows.Forms.Label lbStatusYellow;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.Timer timerR;
        private System.Windows.Forms.Timer timerG;
        private System.Windows.Forms.Timer timerY;
        private System.Windows.Forms.Timer timerB;
    }
}