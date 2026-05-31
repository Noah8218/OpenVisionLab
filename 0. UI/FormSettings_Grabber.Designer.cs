namespace KtemVisionSystem
{
    partial class FormSettings_Grabber
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
            this.cbPortName = new MetroFramework.Controls.MetroComboBox();
            this.cbBaudrate = new MetroFramework.Controls.MetroComboBox();
            this.btnOFF = new MetroFramework.Controls.MetroButton();
            this.btnON = new MetroFramework.Controls.MetroButton();
            this.lbValue = new MetroFramework.Controls.MetroLabel();
            this.trbValue = new MetroFramework.Controls.MetroTrackBar();
            this.cbChannel = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.btnConnect = new MetroFramework.Controls.MetroButton();
            this.lbConnection = new MetroFramework.Controls.MetroTile();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            // 
            // cbPortName
            // 
            this.cbPortName.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbPortName.FormattingEnabled = true;
            this.cbPortName.ItemHeight = 29;
            this.cbPortName.Location = new System.Drawing.Point(158, 65);
            this.cbPortName.Name = "cbPortName";
            this.cbPortName.Size = new System.Drawing.Size(151, 35);
            this.cbPortName.TabIndex = 949;
            this.cbPortName.UseSelectable = true;
            // 
            // cbBaudrate
            // 
            this.cbBaudrate.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbBaudrate.FormattingEnabled = true;
            this.cbBaudrate.ItemHeight = 29;
            this.cbBaudrate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "51200",
            "115400"});
            this.cbBaudrate.Location = new System.Drawing.Point(447, 65);
            this.cbBaudrate.Name = "cbBaudrate";
            this.cbBaudrate.Size = new System.Drawing.Size(151, 35);
            this.cbBaudrate.TabIndex = 1005;
            this.cbBaudrate.UseSelectable = true;
            // 
            // btnOFF
            // 
            this.btnOFF.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnOFF.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnOFF.Highlight = true;
            this.btnOFF.Location = new System.Drawing.Point(845, 101);
            this.btnOFF.Name = "btnOFF";
            this.btnOFF.Size = new System.Drawing.Size(74, 35);
            this.btnOFF.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnOFF.TabIndex = 1060;
            this.btnOFF.Text = "OFF";
            this.btnOFF.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnOFF.UseSelectable = true;
            this.btnOFF.Click += new System.EventHandler(this.btnOFF_Click);
            // 
            // btnON
            // 
            this.btnON.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnON.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnON.Highlight = true;
            this.btnON.Location = new System.Drawing.Point(768, 101);
            this.btnON.Name = "btnON";
            this.btnON.Size = new System.Drawing.Size(76, 35);
            this.btnON.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnON.TabIndex = 1059;
            this.btnON.Text = "ON";
            this.btnON.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnON.UseSelectable = true;
            this.btnON.Click += new System.EventHandler(this.btnON_Click);
            // 
            // lbValue
            // 
            this.lbValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbValue.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbValue.ForeColor = System.Drawing.Color.White;
            this.lbValue.Location = new System.Drawing.Point(687, 101);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(80, 35);
            this.lbValue.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbValue.TabIndex = 1058;
            this.lbValue.Text = "000";
            this.lbValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbValue.UseCustomBackColor = true;
            this.lbValue.UseCustomForeColor = true;
            // 
            // trbValue
            // 
            this.trbValue.BackColor = System.Drawing.Color.Transparent;
            this.trbValue.Location = new System.Drawing.Point(310, 101);
            this.trbValue.Maximum = 255;
            this.trbValue.Name = "trbValue";
            this.trbValue.Size = new System.Drawing.Size(376, 35);
            this.trbValue.TabIndex = 1057;
            this.trbValue.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // cbChannel
            // 
            this.cbChannel.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.ItemHeight = 29;
            this.cbChannel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cbChannel.Location = new System.Drawing.Point(158, 101);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(151, 35);
            this.cbChannel.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbChannel.TabIndex = 1056;
            this.cbChannel.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbChannel.UseSelectable = true;
            // 
            // metroLabel5
            // 
            this.metroLabel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel5.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel5.ForeColor = System.Drawing.Color.White;
            this.metroLabel5.Location = new System.Drawing.Point(21, 101);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(135, 35);
            this.metroLabel5.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel5.TabIndex = 1055;
            this.metroLabel5.Text = "CHANNEL";
            this.metroLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel5.UseCustomBackColor = true;
            this.metroLabel5.UseCustomForeColor = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(21, 65);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(135, 35);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1061;
            this.metroLabel1.Text = "COMPORT";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // metroLabel2
            // 
            this.metroLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.ForeColor = System.Drawing.Color.White;
            this.metroLabel2.Location = new System.Drawing.Point(310, 65);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(135, 35);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel2.TabIndex = 1062;
            this.metroLabel2.Text = "BAUDRATE";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.UseCustomBackColor = true;
            this.metroLabel2.UseCustomForeColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnConnect.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnConnect.Highlight = true;
            this.btnConnect.Location = new System.Drawing.Point(600, 65);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(167, 35);
            this.btnConnect.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnConnect.TabIndex = 1063;
            this.btnConnect.Text = "CONNECT";
            this.btnConnect.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnConnect.UseSelectable = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lbConnection
            // 
            this.lbConnection.ActiveControl = null;
            this.lbConnection.BackColor = System.Drawing.Color.Transparent;
            this.lbConnection.Location = new System.Drawing.Point(769, 65);
            this.lbConnection.Name = "lbConnection";
            this.lbConnection.Size = new System.Drawing.Size(150, 35);
            this.lbConnection.Style = MetroFramework.MetroColorStyle.Silver;
            this.lbConnection.TabIndex = 1088;
            this.lbConnection.Text = "DISCONNECT";
            this.lbConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbConnection.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbConnection.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.lbConnection.UseSelectable = true;
            this.lbConnection.UseTileImage = true;
            // 
            // btnSave
            // 
            this.btnSave.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSave.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnSave.Highlight = true;
            this.btnSave.Location = new System.Drawing.Point(21, 142);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(898, 70);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1089;
            this.btnSave.Text = "SAVE";
            this.btnSave.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnSave.UseSelectable = true;            
            // 
            // FormSettings_Grabber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 234);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lbConnection);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.btnOFF);
            this.Controls.Add(this.btnON);
            this.Controls.Add(this.lbValue);
            this.Controls.Add(this.trbValue);
            this.Controls.Add(this.cbChannel);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.cbBaudrate);
            this.Controls.Add(this.cbPortName);
            this.Name = "FormSettings_Grabber";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "GRABBER BOARD";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormSettings_Illumination_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroComboBox cbPortName;
        private MetroFramework.Controls.MetroComboBox cbBaudrate;
        private MetroFramework.Controls.MetroButton btnOFF;
        private MetroFramework.Controls.MetroButton btnON;
        private MetroFramework.Controls.MetroLabel lbValue;
        private MetroFramework.Controls.MetroTrackBar trbValue;
        private MetroFramework.Controls.MetroComboBox cbChannel;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnConnect;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTile lbConnection;
        private MetroFramework.Controls.MetroButton btnSave;
    }
}