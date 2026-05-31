namespace KtemVisionSystem
{
    partial class FormAuthorization
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
            this.lbNotice = new MetroFramework.Controls.MetroLabel();
            this.metroTile5 = new MetroFramework.Controls.MetroTile();
            this.btnLoginChange = new MetroFramework.Controls.MetroButton();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAuthoriztion = new MetroFramework.Controls.MetroComboBox();
            this.btnLogin = new MetroFramework.Controls.MetroTile();
            this.tbPassword = new RJCodeUI_M1.RJControls.RJTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // lbNotice
            // 
            this.lbNotice.AutoSize = true;
            this.lbNotice.BackColor = System.Drawing.Color.White;
            this.lbNotice.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lbNotice.ForeColor = System.Drawing.Color.Black;
            this.lbNotice.Location = new System.Drawing.Point(11, 316);
            this.lbNotice.Name = "lbNotice";
            this.lbNotice.Size = new System.Drawing.Size(183, 19);
            this.lbNotice.Style = MetroFramework.MetroColorStyle.Silver;
            this.lbNotice.TabIndex = 1020;
            this.lbNotice.Text = "Please Enter the Password";
            this.lbNotice.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbNotice.UseCustomBackColor = true;
            this.lbNotice.UseCustomForeColor = true;
            // 
            // metroTile5
            // 
            this.metroTile5.ActiveControl = null;
            this.metroTile5.BackColor = System.Drawing.Color.White;
            this.metroTile5.ForeColor = System.Drawing.Color.White;
            this.metroTile5.Location = new System.Drawing.Point(-1, 52);
            this.metroTile5.Name = "metroTile5";
            this.metroTile5.Size = new System.Drawing.Size(363, 155);
            this.metroTile5.TabIndex = 1866;
            this.metroTile5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.metroTile5.TileImage = global::KtemVisionSystem.Properties.Resources.user_128_;
            this.metroTile5.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile5.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.metroTile5.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile5.UseCustomBackColor = true;
            this.metroTile5.UseSelectable = true;
            this.metroTile5.UseTileImage = true;
            // 
            // btnLoginChange
            // 
            this.btnLoginChange.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnLoginChange.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnLoginChange.Highlight = true;
            this.btnLoginChange.Location = new System.Drawing.Point(183, 338);
            this.btnLoginChange.Name = "btnLoginChange";
            this.btnLoginChange.Size = new System.Drawing.Size(168, 58);
            this.btnLoginChange.TabIndex = 1024;
            this.btnLoginChange.Text = "비밀번호 변경";
            this.btnLoginChange.UseCustomBackColor = true;
            this.btnLoginChange.UseCustomForeColor = true;
            this.btnLoginChange.UseSelectable = true;
            this.btnLoginChange.UseStyleColors = true;
            this.btnLoginChange.Click += new System.EventHandler(this.btnLoginClose_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Location = new System.Drawing.Point(23, 213);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(121, 25);
            this.label11.TabIndex = 1955;
            this.label11.Text = "Permission";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(23, 264);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 25);
            this.label1.TabIndex = 1956;
            this.label1.Text = "Password";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cbAuthoriztion
            // 
            this.cbAuthoriztion.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbAuthoriztion.FormattingEnabled = true;
            this.cbAuthoriztion.ItemHeight = 29;
            this.cbAuthoriztion.Items.AddRange(new object[] {
            "OPERATOR",
            "ENGINEER",
            "MASTER"});
            this.cbAuthoriztion.Location = new System.Drawing.Point(151, 213);
            this.cbAuthoriztion.Name = "cbAuthoriztion";
            this.cbAuthoriztion.Size = new System.Drawing.Size(200, 35);
            this.cbAuthoriztion.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbAuthoriztion.TabIndex = 1959;
            this.cbAuthoriztion.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbAuthoriztion.UseSelectable = true;
            // 
            // btnLogin
            // 
            this.btnLogin.ActiveControl = null;
            this.btnLogin.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(11, 338);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(166, 58);
            this.btnLogin.TabIndex = 1960;
            this.btnLogin.Text = "Login";
            this.btnLogin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogin.TileImage = global::KtemVisionSystem.Properties.Resources.icons8_window_search_50;
            this.btnLogin.TileImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogin.TileTextFontSize = MetroFramework.MetroTileTextSize.Tall;
            this.btnLogin.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.btnLogin.UseCustomBackColor = true;
            this.btnLogin.UseSelectable = true;
            this.btnLogin.UseTileImage = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tbPassword
            // 
            this.tbPassword._Customizable = false;
            this.tbPassword.BackColor = System.Drawing.Color.White;
            this.tbPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.tbPassword.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.tbPassword.BorderRadius = 0;
            this.tbPassword.BorderSize = 2;
            this.tbPassword.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.tbPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.tbPassword.Location = new System.Drawing.Point(151, 264);
            this.tbPassword.MultiLine = false;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.tbPassword.PasswordChar = true;
            this.tbPassword.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.tbPassword.PlaceHolderText = "Password";
            this.tbPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbPassword.Size = new System.Drawing.Size(200, 31);
            this.tbPassword.Style = RJCodeUI_M1.RJControls.TextBoxStyle.FlaringLine;
            this.tbPassword.TabIndex = 1961;
            // 
            // FormAuthorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(364, 408);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.cbAuthoriztion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.metroTile5);
            this.Controls.Add(this.btnLoginChange);
            this.Controls.Add(this.lbNotice);
            this.MaximizeBox = false;
            this.Name = "FormAuthorization";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "LOGIN";
            this.Load += new System.EventHandler(this.FormTeachingSelect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroLabel lbNotice;
        private MetroFramework.Controls.MetroTile metroTile5;
        private MetroFramework.Controls.MetroButton btnLoginChange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private MetroFramework.Controls.MetroComboBox cbAuthoriztion;
        private MetroFramework.Controls.MetroTile btnLogin;
        private RJCodeUI_M1.RJControls.RJTextBox tbPassword;
    }
}