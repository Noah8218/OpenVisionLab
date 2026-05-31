using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using System.Diagnostics;
using MetroFramework.Forms;
using OpenVisionLab;

namespace RJCodeUI_M1
{
    public partial class LoginForm : RJForms.RJBaseForm
    {        

        #region -> Constructor

        public LoginForm()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            AddControlBox();
            ApplyAppearanceSettings();
        }
        #endregion

        #region -> Private methods

        private void AddControlBox()
        {//Add control buttons (Maximize, close and minimize)
		
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMinimize);
            this.btnClose.Height = 20;
            this.btnClose.Location = new Point(this.Width - btnClose.Width, 0);

            this.btnMinimize.Height = 20;
            this.btnMinimize.Location = new Point(this.btnClose.Location.X - btnMinimize.Width, 0);
        }
        private void ApplyAppearanceSettings()
        {// Apply the appearance properties of the configuration.

             this.PrimaryForm = true; // Set as primary form.
             this.Resizable = false; // Set that the form cannot be resized from the border.
             this.BorderSize = 0; // Remove the border.
             this.BackColor = UIAppearance.BackgroundColor; // Set the back color.

            if (UIAppearance.Theme == UITheme.Light)//if the theme is LIGHT, set the maximize, minimize and close buttons to black.
            {
                //this.btnClose.Image = Properties.Resources.CloseDark;
                //this.btnMaximize.Image = Properties.Resources.MaximizeDark;
                //this.btnMinimize.Image = Properties.Resources.MinimizeDark;
            }

        }
        private void Login()
        {
            //Validate fields
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                lblMessage.Text = "*Please enter your username";
                lblMessage.Visible = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.Text = "*Please enter your password";
                lblMessage.Visible = true;
                return;
            }

            //Login
            //var user = new Models.User().Login(txtUser.Text, txtPassword.Text);

            //if (user != null)
            //{
            //    var mainForm = new FormMetroFrame(user);               
            //    mainForm.Show();
            //    this.Hide();

            //    //Redisplay the login form and clear fields if the main form is closed
            //    mainForm.FormClosed += new FormClosedEventHandler(MainForm_Logout);
            //}
            //else
            //{
            //    lblMessage.Text = "*Incorrect username or password";
            //    lblMessage.Visible = true;
            //}
        }
        private void Logout()
        {
            txtPassword.Clear();
            txtUser.Clear();           
            lblMessage.Visible = false;
            lblCaption.Select();
            this.Close();
        }
        #endregion

        #region -> Overrides

        protected override void CloseWindow()
        {//Override the method (To remove the message box if you want to exit the app) and simply exit the application, This is optional.
            Application.Exit();
        }
        #endregion

        #region -> Event Methods

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Login();
        }

        private void MainForm_Logout(object sender, FormClosedEventArgs e)
        {
            Logout();//Log out
        }
        #endregion

        private void lblForgotPass_Click(object sender, EventArgs e)
        {

        }
    }
}
