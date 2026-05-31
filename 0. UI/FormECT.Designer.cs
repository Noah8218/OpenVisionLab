namespace KtemVisionSystem
{
    partial class FormECT
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
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.btnConnect = new MetroFramework.Controls.MetroButton();
            this.nbDeleteRemainDay = new System.Windows.Forms.NumericUpDown();
            this.cbDrivePath = new System.Windows.Forms.ComboBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.nbDiskVolume = new System.Windows.Forms.NumericUpDown();
            this.btnImageDeleteSetting = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.nbDeleteRemainDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbDiskVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // metroLabel2
            // 
            this.metroLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.ForeColor = System.Drawing.Color.White;
            this.metroLabel2.Location = new System.Drawing.Point(35, 140);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(147, 50);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel2.TabIndex = 1063;
            this.metroLabel2.Text = "Image Preserve Day";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.UseCustomBackColor = true;
            this.metroLabel2.UseCustomForeColor = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(35, 191);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(147, 54);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1064;
            this.metroLabel1.Text = "DISK :";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnConnect.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnConnect.Highlight = true;
            this.btnConnect.Location = new System.Drawing.Point(35, 104);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(289, 35);
            this.btnConnect.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnConnect.TabIndex = 1065;
            this.btnConnect.Text = "ImageDelete";
            this.btnConnect.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnConnect.UseSelectable = true;
            // 
            // nbDeleteRemainDay
            // 
            this.nbDeleteRemainDay.BackColor = System.Drawing.Color.Black;
            this.nbDeleteRemainDay.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nbDeleteRemainDay.ForeColor = System.Drawing.Color.Aquamarine;
            this.nbDeleteRemainDay.Location = new System.Drawing.Point(183, 140);
            this.nbDeleteRemainDay.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.nbDeleteRemainDay.Name = "nbDeleteRemainDay";
            this.nbDeleteRemainDay.Size = new System.Drawing.Size(141, 50);
            this.nbDeleteRemainDay.TabIndex = 1066;
            this.nbDeleteRemainDay.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // cbDrivePath
            // 
            this.cbDrivePath.BackColor = System.Drawing.Color.Black;
            this.cbDrivePath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrivePath.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbDrivePath.ForeColor = System.Drawing.Color.Aquamarine;
            this.cbDrivePath.FormattingEnabled = true;
            this.cbDrivePath.Location = new System.Drawing.Point(183, 194);
            this.cbDrivePath.Name = "cbDrivePath";
            this.cbDrivePath.Size = new System.Drawing.Size(143, 50);
            this.cbDrivePath.TabIndex = 1067;
            // 
            // metroLabel3
            // 
            this.metroLabel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel3.ForeColor = System.Drawing.Color.White;
            this.metroLabel3.Location = new System.Drawing.Point(35, 246);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(147, 54);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel3.TabIndex = 1064;
            this.metroLabel3.Text = "Disk Volume(%)";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel3.UseCustomBackColor = true;
            this.metroLabel3.UseCustomForeColor = true;
            // 
            // nbDiskVolume
            // 
            this.nbDiskVolume.BackColor = System.Drawing.Color.Black;
            this.nbDiskVolume.Font = new System.Drawing.Font("Arial", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nbDiskVolume.ForeColor = System.Drawing.Color.Aquamarine;
            this.nbDiskVolume.Location = new System.Drawing.Point(183, 250);
            this.nbDiskVolume.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.nbDiskVolume.Name = "nbDiskVolume";
            this.nbDiskVolume.Size = new System.Drawing.Size(141, 50);
            this.nbDiskVolume.TabIndex = 1068;
            this.nbDiskVolume.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnImageDeleteSetting
            // 
            this.btnImageDeleteSetting.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnImageDeleteSetting.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImageDeleteSetting.Highlight = true;
            this.btnImageDeleteSetting.Location = new System.Drawing.Point(35, 301);
            this.btnImageDeleteSetting.Name = "btnImageDeleteSetting";
            this.btnImageDeleteSetting.Size = new System.Drawing.Size(289, 66);
            this.btnImageDeleteSetting.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnImageDeleteSetting.TabIndex = 1090;
            this.btnImageDeleteSetting.Text = "SAVE";
            this.btnImageDeleteSetting.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnImageDeleteSetting.UseSelectable = true;
            this.btnImageDeleteSetting.Click += new System.EventHandler(this.btnImageDeleteSetting_Click);
            // 
            // FormECT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 400);
            this.Controls.Add(this.btnImageDeleteSetting);
            this.Controls.Add(this.nbDiskVolume);
            this.Controls.Add(this.cbDrivePath);
            this.Controls.Add(this.nbDeleteRemainDay);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.metroLabel2);
            this.Name = "FormECT";
            this.Text = "FormECT";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormECT_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nbDeleteRemainDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nbDiskVolume)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnConnect;
        private System.Windows.Forms.NumericUpDown nbDeleteRemainDay;
        private System.Windows.Forms.ComboBox cbDrivePath;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private System.Windows.Forms.NumericUpDown nbDiskVolume;
        private MetroFramework.Controls.MetroButton btnImageDeleteSetting;
    }
}