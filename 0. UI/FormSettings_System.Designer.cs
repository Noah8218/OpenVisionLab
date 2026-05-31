namespace KtemVisionSystem
{
    partial class FormSettings_System
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
            this.btnOptionDryRun = new MetroFramework.Controls.MetroTile();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // btnOptionDryRun
            // 
            this.btnOptionDryRun.ActiveControl = null;
            this.btnOptionDryRun.BackColor = System.Drawing.Color.DimGray;
            this.btnOptionDryRun.Enabled = false;
            this.btnOptionDryRun.Location = new System.Drawing.Point(506, 373);
            this.btnOptionDryRun.Name = "btnOptionDryRun";
            this.btnOptionDryRun.Size = new System.Drawing.Size(182, 40);
            this.btnOptionDryRun.Style = MetroFramework.MetroColorStyle.Silver;
            this.btnOptionDryRun.TabIndex = 1643;
            this.btnOptionDryRun.Text = "USE DRY RUN";
            this.btnOptionDryRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnOptionDryRun.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnOptionDryRun.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnOptionDryRun.UseCustomBackColor = true;
            this.btnOptionDryRun.UseSelectable = true;
            this.btnOptionDryRun.UseTileImage = true;
            this.btnOptionDryRun.Visible = false;
            this.btnOptionDryRun.Click += new System.EventHandler(this.OnClickOption);
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(11, 63);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(365, 91);
            this.metroTile4.Style = MetroFramework.MetroColorStyle.Yellow;
            this.metroTile4.TabIndex = 1648;
            this.metroTile4.Text = "DEVICE";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.metroTile4.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile4.UseSelectable = true;
            this.metroTile4.UseTileImage = true;
            this.metroTile4.Click += new System.EventHandler(this.btnDevice_Click);
            // 
            // FormSettings_System
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 595);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.btnOptionDryRun);
            this.Name = "FormSettings_System";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "SETTINGS";
            this.Load += new System.EventHandler(this.FormTeachingSelect_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroTile btnOptionDryRun;
        private MetroFramework.Controls.MetroTile metroTile4;
    }
}