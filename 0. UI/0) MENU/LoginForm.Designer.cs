namespace RJCodeUI_M1
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.dragControl1 = new RJCodeUI_M1.RJControls.RJDragControl(this.components);
            this.lblCaption = new RJCodeUI_M1.RJControls.RJLabel();
            this.txtUser = new RJCodeUI_M1.RJControls.RJTextBox();
            this.txtPassword = new RJCodeUI_M1.RJControls.RJTextBox();
            this.lblForgotPass = new RJCodeUI_M1.RJControls.RJLabel();
            this.dragControl2 = new RJCodeUI_M1.RJControls.RJDragControl(this.components);
            this.icoBanner = new RJCodeUI_M1.RJControls.RJImageColorOverlay();
            this.lblMessage = new System.Windows.Forms.Label();
            this.rjBarIcon2 = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.rjBarIcon1 = new RJCodeUI_M1.RJControls.RJMenuIcon();
            this.btnLogin = new RJCodeUI_M1.RJControls.RJButton();
            this.rjDragControl1 = new RJCodeUI_M1.RJControls.RJDragControl(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.rjBarIcon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rjBarIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // dragControl1
            // 
            this.dragControl1.DragControl = this;
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblCaption.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(96)))), ((int)(((byte)(99)))));
            this.lblCaption.LinkLabel = false;
            this.lblCaption.Location = new System.Drawing.Point(397, 43);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(63, 23);
            this.lblCaption.Style = RJCodeUI_M1.RJControls.LabelStyle.Title2;
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Login";
            // 
            // txtUser
            // 
            this.txtUser._Customizable = false;
            this.txtUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtUser.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.txtUser.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.txtUser.BorderRadius = 0;
            this.txtUser.BorderSize = 2;
            this.txtUser.Font = new System.Drawing.Font("Verdana", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.txtUser.Location = new System.Drawing.Point(429, 121);
            this.txtUser.MaxLength = 100;
            this.txtUser.MultiLine = false;
            this.txtUser.Name = "txtUser";
            this.txtUser.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.txtUser.PasswordChar = false;
            this.txtUser.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.txtUser.PlaceHolderText = "Username";
            this.txtUser.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtUser.Size = new System.Drawing.Size(254, 32);
            this.txtUser.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.txtUser.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword._Customizable = false;
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.txtPassword.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(120)))), ((int)(((byte)(218)))));
            this.txtPassword.BorderRadius = 0;
            this.txtPassword.BorderSize = 2;
            this.txtPassword.Font = new System.Drawing.Font("Verdana", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.txtPassword.Location = new System.Drawing.Point(429, 177);
            this.txtPassword.MaxLength = 100;
            this.txtPassword.MultiLine = false;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.txtPassword.PasswordChar = true;
            this.txtPassword.PlaceHolderColor = System.Drawing.Color.DarkGray;
            this.txtPassword.PlaceHolderText = "Password";
            this.txtPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtPassword.Size = new System.Drawing.Size(254, 32);
            this.txtPassword.Style = RJCodeUI_M1.RJControls.TextBoxStyle.MatteLine;
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            // 
            // lblForgotPass
            // 
            this.lblForgotPass.AutoSize = true;
            this.lblForgotPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblForgotPass.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lblForgotPass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lblForgotPass.LinkLabel = true;
            this.lblForgotPass.Location = new System.Drawing.Point(497, 313);
            this.lblForgotPass.Name = "lblForgotPass";
            this.lblForgotPass.Size = new System.Drawing.Size(124, 16);
            this.lblForgotPass.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lblForgotPass.TabIndex = 5;
            this.lblForgotPass.Text = "Forgot Password?";
            this.lblForgotPass.Click += new System.EventHandler(this.lblForgotPass_Click);
            // 
            // dragControl2
            // 
            this.dragControl2.DragControl = this.icoBanner;
            // 
            // icoBanner
            // 
            this.icoBanner.BackgroundImage = global::OpenVisionLab.Properties.Resources.ktem1;
            this.icoBanner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.icoBanner.BorderRadius = 0;
            this.icoBanner.Customizable = false;
            this.icoBanner.Dock = System.Windows.Forms.DockStyle.Left;
            this.icoBanner.Image = global::OpenVisionLab.Properties.Resources.ktem1;
            this.icoBanner.ImageMode = System.Windows.Forms.ImageLayout.Stretch;
            this.icoBanner.Location = new System.Drawing.Point(0, 0);
            this.icoBanner.Name = "icoBanner";
            this.icoBanner.Opacity = 80;
            this.icoBanner.OverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.icoBanner.Size = new System.Drawing.Size(380, 470);
            this.icoBanner.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.ForeColor = System.Drawing.Color.IndianRed;
            this.lblMessage.Location = new System.Drawing.Point(426, 225);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(64, 16);
            this.lblMessage.TabIndex = 9;
            this.lblMessage.Text = "Message";
            this.lblMessage.Visible = false;
            // 
            // rjBarIcon2
            // 
            this.rjBarIcon2.BackColor = System.Drawing.Color.Transparent;
            this.rjBarIcon2.BackIcon = true;
            this.rjBarIcon2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjBarIcon2.Customizable = false;
            this.rjBarIcon2.DropdownMenu = null;
            this.rjBarIcon2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjBarIcon2.IconChar = FontAwesome.Sharp.IconChar.Key;
            this.rjBarIcon2.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjBarIcon2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjBarIcon2.IconSize = 25;
            this.rjBarIcon2.Location = new System.Drawing.Point(401, 185);
            this.rjBarIcon2.Name = "rjBarIcon2";
            this.rjBarIcon2.Size = new System.Drawing.Size(25, 25);
            this.rjBarIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.rjBarIcon2.TabIndex = 11;
            this.rjBarIcon2.TabStop = false;
            // 
            // rjBarIcon1
            // 
            this.rjBarIcon1.BackColor = System.Drawing.Color.Transparent;
            this.rjBarIcon1.BackIcon = true;
            this.rjBarIcon1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rjBarIcon1.Customizable = false;
            this.rjBarIcon1.DropdownMenu = null;
            this.rjBarIcon1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjBarIcon1.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
            this.rjBarIcon1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjBarIcon1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.rjBarIcon1.IconSize = 25;
            this.rjBarIcon1.Location = new System.Drawing.Point(401, 129);
            this.rjBarIcon1.Name = "rjBarIcon1";
            this.rjBarIcon1.Size = new System.Drawing.Size(25, 25);
            this.rjBarIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.rjBarIcon1.TabIndex = 10;
            this.rjBarIcon1.TabStop = false;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.SystemColors.Control;
            this.btnLogin.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLogin.BorderRadius = 20;
            this.btnLogin.BorderSize = 2;
            this.btnLogin.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.btnLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(211)))), ((int)(((byte)(211)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLogin.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnLogin.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnLogin.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLogin.IconSize = 24;
            this.btnLogin.Location = new System.Drawing.Point(429, 257);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(254, 40);
            this.btnLogin.Style = RJCodeUI_M1.RJControls.ControlStyle.Glass;
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // rjDragControl1
            // 
            this.rjDragControl1.DragControl = null;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 470);
            this.ControlBox = false;
            this.Controls.Add(this.rjBarIcon2);
            this.Controls.Add(this.rjBarIcon1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblForgotPass);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.icoBanner);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)(this.rjBarIcon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rjBarIcon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private RJControls.RJDragControl dragControl1;
        private RJControls.RJTextBox txtPassword;
        private RJControls.RJTextBox txtUser;
        private RJControls.RJLabel lblCaption;
        private RJControls.RJButton btnLogin;
        private RJControls.RJLabel lblForgotPass;
        private RJControls.RJDragControl dragControl2;
        private System.Windows.Forms.Label lblMessage;
        private RJControls.RJMenuIcon rjBarIcon2;
        private RJControls.RJMenuIcon rjBarIcon1;
        private RJControls.RJDragControl rjDragControl1;
        private RJControls.RJImageColorOverlay icoBanner;
    }
}