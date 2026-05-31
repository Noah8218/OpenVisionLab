namespace KtemVisionSystem
{
    partial class FormSetting_Delay
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
            this.pnLotOpen = new System.Windows.Forms.Panel();
            this.btnCancel = new MetroFramework.Controls.MetroTile();
            this.btnSave = new MetroFramework.Controls.MetroTile();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.tbDelayBefore = new MetroFramework.Controls.MetroTextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbDelayAfter = new MetroFramework.Controls.MetroTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.pnLotOpen.SuspendLayout();
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
            // pnLotOpen
            // 
            this.pnLotOpen.Controls.Add(this.btnCancel);
            this.pnLotOpen.Controls.Add(this.btnSave);
            this.pnLotOpen.Controls.Add(this.label1);
            this.pnLotOpen.Controls.Add(this.label9);
            this.pnLotOpen.Controls.Add(this.label31);
            this.pnLotOpen.Controls.Add(this.tbDelayBefore);
            this.pnLotOpen.Controls.Add(this.label17);
            this.pnLotOpen.Controls.Add(this.tbDelayAfter);
            this.pnLotOpen.Location = new System.Drawing.Point(23, 63);
            this.pnLotOpen.Name = "pnLotOpen";
            this.pnLotOpen.Size = new System.Drawing.Size(395, 132);
            this.pnLotOpen.TabIndex = 1656;
            // 
            // btnCancel
            // 
            this.btnCancel.ActiveControl = null;
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Location = new System.Drawing.Point(197, 72);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(196, 58);
            this.btnCancel.Style = MetroFramework.MetroColorStyle.Red;
            this.btnCancel.TabIndex = 1750;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.TileImage = global::KtemVisionSystem.Properties.Resources.delete_50;
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
            this.btnSave.Location = new System.Drawing.Point(1, 72);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(195, 58);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1749;
            this.btnSave.Text = "SAVE";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.TileImage = global::KtemVisionSystem.Properties.Resources.save_50;
            this.btnSave.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnSave.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnSave.UseSelectable = true;
            this.btnSave.UseTileImage = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(336, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 35);
            this.label1.TabIndex = 1666;
            this.label1.Text = "ms";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(336, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 35);
            this.label9.TabIndex = 1665;
            this.label9.Text = "ms";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label31.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label31.Location = new System.Drawing.Point(1, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(156, 35);
            this.label31.TabIndex = 1655;
            this.label31.Text = "DELAY (BEFORE)";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbDelayBefore
            // 
            // 
            // 
            // 
            this.tbDelayBefore.CustomButton.Image = null;
            this.tbDelayBefore.CustomButton.Location = new System.Drawing.Point(144, 1);
            this.tbDelayBefore.CustomButton.Name = "";
            this.tbDelayBefore.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbDelayBefore.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbDelayBefore.CustomButton.TabIndex = 1;
            this.tbDelayBefore.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbDelayBefore.CustomButton.UseSelectable = true;
            this.tbDelayBefore.CustomButton.Visible = false;
            this.tbDelayBefore.DisplayIcon = true;
            this.tbDelayBefore.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbDelayBefore.Lines = new string[] {
        "0"};
            this.tbDelayBefore.Location = new System.Drawing.Point(158, 0);
            this.tbDelayBefore.MaxLength = 32767;
            this.tbDelayBefore.Name = "tbDelayBefore";
            this.tbDelayBefore.PasswordChar = '\0';
            this.tbDelayBefore.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbDelayBefore.SelectedText = "";
            this.tbDelayBefore.SelectionLength = 0;
            this.tbDelayBefore.SelectionStart = 0;
            this.tbDelayBefore.ShortcutsEnabled = true;
            this.tbDelayBefore.Size = new System.Drawing.Size(178, 35);
            this.tbDelayBefore.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbDelayBefore.TabIndex = 1656;
            this.tbDelayBefore.Text = "0";
            this.tbDelayBefore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDelayBefore.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbDelayBefore.UseSelectable = true;
            this.tbDelayBefore.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbDelayBefore.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Transparent;
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label17.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.label17.Location = new System.Drawing.Point(1, 36);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(156, 35);
            this.label17.TabIndex = 1642;
            this.label17.Text = "DELAY (AFTER)";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbDelayAfter
            // 
            // 
            // 
            // 
            this.tbDelayAfter.CustomButton.Image = null;
            this.tbDelayAfter.CustomButton.Location = new System.Drawing.Point(144, 1);
            this.tbDelayAfter.CustomButton.Name = "";
            this.tbDelayAfter.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbDelayAfter.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbDelayAfter.CustomButton.TabIndex = 1;
            this.tbDelayAfter.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbDelayAfter.CustomButton.UseSelectable = true;
            this.tbDelayAfter.CustomButton.Visible = false;
            this.tbDelayAfter.DisplayIcon = true;
            this.tbDelayAfter.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbDelayAfter.Lines = new string[] {
        "0"};
            this.tbDelayAfter.Location = new System.Drawing.Point(158, 36);
            this.tbDelayAfter.MaxLength = 32767;
            this.tbDelayAfter.Name = "tbDelayAfter";
            this.tbDelayAfter.PasswordChar = '\0';
            this.tbDelayAfter.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbDelayAfter.SelectedText = "";
            this.tbDelayAfter.SelectionLength = 0;
            this.tbDelayAfter.SelectionStart = 0;
            this.tbDelayAfter.ShortcutsEnabled = true;
            this.tbDelayAfter.Size = new System.Drawing.Size(178, 35);
            this.tbDelayAfter.Style = MetroFramework.MetroColorStyle.Teal;
            this.tbDelayAfter.TabIndex = 1643;
            this.tbDelayAfter.Text = "0";
            this.tbDelayAfter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbDelayAfter.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbDelayAfter.UseSelectable = true;
            this.tbDelayAfter.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbDelayAfter.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // FormSetting_Delay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 208);
            this.Controls.Add(this.pnLotOpen);
            this.Name = "FormSetting_Delay";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "LOT OPEN";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetting_Delay_FormClosing);
            this.Load += new System.EventHandler(this.FormSetting_Delay_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.pnLotOpen.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerView;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private System.Windows.Forms.Panel pnLotOpen;
        private System.Windows.Forms.Label label31;
        private MetroFramework.Controls.MetroTextBox tbDelayBefore;
        private System.Windows.Forms.Label label17;
        private MetroFramework.Controls.MetroTextBox tbDelayAfter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private MetroFramework.Controls.MetroTile btnCancel;
        private MetroFramework.Controls.MetroTile btnSave;
    }
}