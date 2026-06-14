using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenVisionLab;
using OpenVisionLab.MessageDialogs;
using RJCodeUI_M1.RJForms;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;

namespace RJCodeUI_M1.RJForms
{
    public partial class RJSettingsForm : RJChildForm
    {
        /// <summary>
        /// This class inherits from the <see cref="RJChildForm"/>  class
        /// </summary>
        /// 

        #region -> Constructor

        public RJSettingsForm()
        {
            //This form was built by the designer.
            InitializeComponent();
        }
        #endregion

        #region -> Event Methods

        private void RJSettingsForm_Load(object sender, EventArgs e)
        {
            LoadAppearanceSettings(); //Load and display the current appearance settings on the form.
        }

        private void btnApplyChanges_Click(object sender, EventArgs e)
        {
            SaveAppearanceSettings(); //Save appearance settings
        }

        private void lblRestartApp_Click(object sender, EventArgs e)
        {//Restart application

            Application.Restart();
            Environment.Exit(0);
            /* Note: When executing the application from visual studio, the configuration file is saved 
              * in the folder RJCodeUI_M1.vshost.exe. And when restarting the application the configuration
              * file is obtained from the C:\Users\YourUsername\AppData\Local\RJCodeUI_M1\RJCodeUI_M1.exe folder, 
			  * since after restarting the application it
              * runs independently of visual studio, so it will not load the settings you established on the 
              * first restart since it will take the file of 
			  * C:\Users\YourUsername\AppData\Local\RJCodeUI_M1\RJCodeUI_M1.exe configuration. If you want to 
              * test and apply the settings established when you are developing the application, I recommend 
              * you close the application (or stop debugging) and rerun from visual studio or compile the project
              * and run the application directly from the project's bin folder.*/
        }
        #endregion

        #region -> Private methods

        private void LoadAppearanceSettings()
        {//Display the current appearance settings on the form.

            //Theme
            if (UIAppearance.Theme == UITheme.Dark)
                rbDarkTheme.Checked = true;
            else
                rbLightTheme.Checked = true;

            //Style
            cbStyles.DataSource = Enum.GetValues(typeof(UIStyle));
            cbStyles.SelectedIndex = (int)UIAppearance.Style;

            //Form Border Size
            tbmFormBorderSize.Value = UIAppearance.FormBorderSize;

            //Is Color Form Border
            tbColorFormBorder.Checked = UIAppearance.FormBorderColor == RJColors.DefaultFormBorderColor ? false : true;

            //Child Form Marker
            tbChildFormMarker.Checked = UIAppearance.ChildFormMarker;

            //Form Icon in Activated Menu Item
            tbIconMenuItem.Checked = UIAppearance.FormIconActiveMenuItem;

            //MDI Desktop Panel
            tbMultiChildForms.Checked = UIAppearance.MultiChildForms;

            //Preview
            panelBorde.Padding = new Padding(UIAppearance.FormBorderSize);
            panelBorde.BackColor = UIAppearance.FormBorderColor;
            panelBackground.BackColor = UIAppearance.BackgroundColor;
            if (UIAppearance.Style == UIStyle.Supernova)
                panelTitleBar.BackColor = ColorEditor.Darken(UIAppearance.BackgroundColor, 9);
            else panelTitleBar.BackColor = UIAppearance.PrimaryStyleColor;

        }
        private void SaveAppearanceSettings()
        {
            //Save appearance settings
            SettingsManager.SaveAppearanceSettings(rbDarkTheme.Checked ? (int)UITheme.Dark : (int)UITheme.Light,/*Theme*/
                                                            (int)cbStyles.SelectedValue,/*Style*/
                                                            tbmFormBorderSize.Value,/*Form border size*/
                                                            tbColorFormBorder.Checked,/*Color form border*/
                                                            tbChildFormMarker.Checked,/*Child form marker*/
                                                            tbIconMenuItem.Checked,/*Form icon in activated menu item*/
                                                            tbMultiChildForms.Checked);/*Multiple child forms*/
            //Show restart message
            var result = VisionMessageBox.Confirm(
                this,
                "Message",
                "Please restart the application to view changes.\r\nRestart now?");

            if (result == DialogResult.Yes)//Restart application
            {
                Application.Restart();
                Environment.Exit(0);
                 /* Note: When executing the application from visual studio, the configuration file is saved 
              * in the folder RJCodeUI_M1.vshost.exe. And when restarting the application the configuration
              * file is obtained from the C:\Users\YourUsername\AppData\Local\RJCodeUI_M1\RJCodeUI_M1.exe folder, 
			  * since after restarting the application it
              * runs independently of visual studio, so it will not load the settings you established on the 
              * first restart since it will take the file of 
			  * C:\Users\YourUsername\AppData\Local\RJCodeUI_M1\RJCodeUI_M1.exe configuration. If you want to 
              * test and apply the settings established when you are developing the application, I recommend 
              * you close the application (or stop debugging) and rerun from visual studio or compile the project
              * and run the application directly from the project's bin folder.*/
            }
            else //Show restart message label in case you did not reboot from the message box.
            {
                lblRestartApp.Visible = true;
            }

        }
        #endregion

        #region -> Preview

        private void rbLightTheme_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLightTheme.Checked)
                panelBackground.BackColor = RJColors.LightBackground;
            else panelBackground.BackColor = RJColors.DarkBackground;

            if (cbStyles.SelectedIndex == (int)UIStyle.Supernova)
                panelTitleBar.BackColor = ColorEditor.Darken(panelBackground.BackColor, 9);
        }

        private void cbStyles_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            UIStyle style = (UIStyle)cbStyles.SelectedIndex;
            switch (style)
            {
                case UIStyle.Axolotl:
                    panelTitleBar.BackColor = RJColors.Axolotl;
                    break;
                case UIStyle.Navi:
                    panelTitleBar.BackColor = RJColors.Navi;
                    break;
                case UIStyle.FireOpal:
                    panelTitleBar.BackColor = RJColors.FireOpal;
                    break;
                case UIStyle.Forest:
                    panelTitleBar.BackColor = RJColors.Forest;
                    break;
                case UIStyle.Lisianthus:
                    panelTitleBar.BackColor = RJColors.Lisianthus;
                    break;
                case UIStyle.Neptune:
                    panelTitleBar.BackColor = RJColors.Neptune;
                    break;
                case UIStyle.Petunia:
                    panelTitleBar.BackColor = RJColors.Petunia;
                    break;
                case UIStyle.Ruby:
                    panelTitleBar.BackColor = RJColors.Ruby;
                    break;
                case UIStyle.Sky:
                    panelTitleBar.BackColor = RJColors.Sky;
                    break;
                case UIStyle.Spinel:
                    panelTitleBar.BackColor = RJColors.Spinel;
                    break;
                case UIStyle.Supernova:
                    panelTitleBar.BackColor = ColorEditor.Darken(panelBackground.BackColor, 9);
                    break;
            }
            if (tbColorFormBorder.Checked)
            {
                if (cbStyles.SelectedIndex == (int)UIStyle.Supernova)
                    panelBorde.BackColor = RJColors.FantasyColorScheme1;
                else panelBorde.BackColor = panelTitleBar.BackColor;
            }
            else panelBorde.BackColor = RJColors.DefaultFormBorderColor;
        }

        private void tbmFormBorderSize_Scroll(object sender, EventArgs e)
        {
            panelBorde.Padding = new Padding(tbmFormBorderSize.Value);
        }

        private void tbColorFormBorder_CheckedChanged(object sender, EventArgs e)
        {
            if (tbColorFormBorder.Checked)
            {
                if (cbStyles.SelectedIndex == (int)UIStyle.Supernova)
                    panelBorde.BackColor = RJColors.FantasyColorScheme1;
                else panelBorde.BackColor = panelTitleBar.BackColor;
            }
            else panelBorde.BackColor = RJColors.DefaultFormBorderColor;
        }
        #endregion

    }
}
