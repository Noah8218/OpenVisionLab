namespace KtemVisionSystem
{
    partial class FormAuthorization_DoubleCheck
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
            this.btnOK = new MetroFramework.Controls.MetroButton();
            this.btnLoginMaster1 = new MetroFramework.Controls.MetroTile();
            this.cbMaster1 = new System.Windows.Forms.ComboBox();
            this.lbMaster1 = new System.Windows.Forms.Label();
            this.btnLoginMaster2 = new MetroFramework.Controls.MetroTile();
            this.cbMaster2 = new System.Windows.Forms.ComboBox();
            this.lbMaster2 = new System.Windows.Forms.Label();
            this.btnCanel = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnOK.Highlight = true;
            this.btnOK.Location = new System.Drawing.Point(23, 186);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(308, 58);
            this.btnOK.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnOK.TabIndex = 1083;
            this.btnOK.Text = "변경";
            this.btnOK.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnOK.UseSelectable = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnLoginMaster1
            // 
            this.btnLoginMaster1.ActiveControl = null;
            this.btnLoginMaster1.BackColor = System.Drawing.Color.Transparent;
            this.btnLoginMaster1.Location = new System.Drawing.Point(508, 69);
            this.btnLoginMaster1.Name = "btnLoginMaster1";
            this.btnLoginMaster1.Size = new System.Drawing.Size(133, 57);
            this.btnLoginMaster1.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnLoginMaster1.TabIndex = 1665;
            this.btnLoginMaster1.Text = "로그인";
            this.btnLoginMaster1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoginMaster1.TileImage = global::KtemVisionSystem.Properties.Resources.icons8_window_search_50;
            this.btnLoginMaster1.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnLoginMaster1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnLoginMaster1.UseSelectable = true;
            this.btnLoginMaster1.UseTileImage = true;
            this.btnLoginMaster1.Click += new System.EventHandler(this.btnLoginMaster1_Click);
            // 
            // cbMaster1
            // 
            this.cbMaster1.Font = new System.Drawing.Font("Arial", 32F);
            this.cbMaster1.FormattingEnabled = true;
            this.cbMaster1.Location = new System.Drawing.Point(212, 69);
            this.cbMaster1.Name = "cbMaster1";
            this.cbMaster1.Size = new System.Drawing.Size(295, 57);
            this.cbMaster1.TabIndex = 1664;
            this.cbMaster1.Text = "ALL";
            // 
            // lbMaster1
            // 
            this.lbMaster1.BackColor = System.Drawing.Color.Transparent;
            this.lbMaster1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMaster1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbMaster1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lbMaster1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbMaster1.Location = new System.Drawing.Point(23, 69);
            this.lbMaster1.Name = "lbMaster1";
            this.lbMaster1.Size = new System.Drawing.Size(188, 57);
            this.lbMaster1.TabIndex = 1663;
            this.lbMaster1.Text = "첫 번째 마스터";
            this.lbMaster1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLoginMaster2
            // 
            this.btnLoginMaster2.ActiveControl = null;
            this.btnLoginMaster2.BackColor = System.Drawing.Color.Transparent;
            this.btnLoginMaster2.Location = new System.Drawing.Point(508, 127);
            this.btnLoginMaster2.Name = "btnLoginMaster2";
            this.btnLoginMaster2.Size = new System.Drawing.Size(133, 57);
            this.btnLoginMaster2.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnLoginMaster2.TabIndex = 1668;
            this.btnLoginMaster2.Text = "로그인";
            this.btnLoginMaster2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoginMaster2.TileImage = global::KtemVisionSystem.Properties.Resources.icons8_window_search_50;
            this.btnLoginMaster2.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnLoginMaster2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnLoginMaster2.UseSelectable = true;
            this.btnLoginMaster2.UseTileImage = true;
            this.btnLoginMaster2.Click += new System.EventHandler(this.btnLoginMaster2_Click);
            // 
            // cbMaster2
            // 
            this.cbMaster2.Font = new System.Drawing.Font("Arial", 32F);
            this.cbMaster2.FormattingEnabled = true;
            this.cbMaster2.Location = new System.Drawing.Point(212, 127);
            this.cbMaster2.Name = "cbMaster2";
            this.cbMaster2.Size = new System.Drawing.Size(295, 57);
            this.cbMaster2.TabIndex = 1667;
            this.cbMaster2.Text = "ALL";
            // 
            // lbMaster2
            // 
            this.lbMaster2.BackColor = System.Drawing.Color.Transparent;
            this.lbMaster2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbMaster2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbMaster2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.lbMaster2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbMaster2.Location = new System.Drawing.Point(23, 127);
            this.lbMaster2.Name = "lbMaster2";
            this.lbMaster2.Size = new System.Drawing.Size(188, 57);
            this.lbMaster2.TabIndex = 1666;
            this.lbMaster2.Text = "두 번째 마스터";
            this.lbMaster2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCanel
            // 
            this.btnCanel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnCanel.Highlight = true;
            this.btnCanel.Location = new System.Drawing.Point(332, 186);
            this.btnCanel.Name = "btnCanel";
            this.btnCanel.Size = new System.Drawing.Size(309, 58);
            this.btnCanel.Style = MetroFramework.MetroColorStyle.Red;
            this.btnCanel.TabIndex = 1669;
            this.btnCanel.Text = "취소";
            this.btnCanel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnCanel.UseSelectable = true;
            this.btnCanel.Click += new System.EventHandler(this.btnCanel_Click);
            // 
            // FormAuthorization_DoubleCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(668, 260);
            this.Controls.Add(this.btnCanel);
            this.Controls.Add(this.btnLoginMaster2);
            this.Controls.Add(this.cbMaster2);
            this.Controls.Add(this.lbMaster2);
            this.Controls.Add(this.btnLoginMaster1);
            this.Controls.Add(this.cbMaster1);
            this.Controls.Add(this.lbMaster1);
            this.Controls.Add(this.btnOK);
            this.Name = "FormAuthorization_DoubleCheck";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "모델 변경 체크";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMessageBox_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton btnOK;
        private MetroFramework.Controls.MetroTile btnLoginMaster1;
        private System.Windows.Forms.ComboBox cbMaster1;
        private System.Windows.Forms.Label lbMaster1;
        private MetroFramework.Controls.MetroTile btnLoginMaster2;
        private System.Windows.Forms.ComboBox cbMaster2;
        private System.Windows.Forms.Label lbMaster2;
        private MetroFramework.Controls.MetroButton btnCanel;
    }
}