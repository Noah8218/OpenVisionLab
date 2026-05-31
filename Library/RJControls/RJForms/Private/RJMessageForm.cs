using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJCodeUI_M1.RJControls;
using RJCodeUI_M1.Utils;
using RJCodeUI_M1.Settings;

namespace RJCodeUI_M1.RJForms.Private
{
    public partial class RJMessageForm : Form
    {
        /// <summary>
        /// More information about the DialogResult property.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.button.dialogresult?view=net-5.0
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.form.dialogresult?view=net-5.0
        /// </summary>
        /// 

        #region Fields Definition

        private Button button1;//Button number 1
        private Button button2;//Button number 2
        private Button button3;//Button number 3
        #endregion

        #region Constructor

        public RJMessageForm(string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)//Parameters required for a message box
        {
            InitializeComponent();//This form was made with the form designer

            this.FormBorderStyle = FormBorderStyle.None; //Remove form border
            this.lblCaption.Text = caption;//Set caption
            this.lblMessageText.Text = text;//Set message text 

            CreateIcon(icon);//Create message box icon
            CreateButtons(buttons, defaultButton);//Create message box icon

            ApplyAppearanceSettings();//Apply settings

        }
        #endregion

        #region Methods Definition

        private void ApplyAppearanceSettings()
        {//load theme settings

            this.pnlTittleBar.BackColor = UIAppearance.PrimaryStyleColor;//Set title bar backcolor
            this.BackColor = RJColors.DefaultFormBorderColor;//Set form border color
            

            if (UIAppearance.Theme == UITheme.Light)
            {
                this.pnlMessageText.BackColor = RJColors.LightBackground;//Set BackgroundColor
                this.pnlIcon.BackColor = RJColors.LightBackground;
                this.pnlButtons.BackColor = RJColors.LightActiveBackground;//Set buttons panel backcolor
                this.lblMessageText.ForeColor = UIAppearance.TextColor;
            }
            else
            {
                this.pnlMessageText.BackColor = RJColors.DarkBackground;//Set BackgroundColor
                this.pnlIcon.BackColor = RJColors.DarkBackground;
                this.pnlButtons.BackColor =Utils.ColorEditor.Darken( RJColors.DarkBackground,3);//Set buttons panel backcolor
                this.lblMessageText.ForeColor =Utils.ColorEditor.Lighten(UIAppearance.TextColor,12);
            }
            this.lblMessageText.MaximumSize = new Size(600, 0);//Set 600 pixels as maximum width of the text message label
            this.Size = new Size(//Set message box size
               width: this.lblMessageText.Width + this.pnlIcon.Width + 50,//Message box widht
               height: this.lblMessageText.Height + this.pnlTittleBar.Height + this.pnlButtons.Height + 5);//Message box eight

        }
        private void CreateIcon(MessageBoxIcon icon)
        {//set message box icon

            switch (icon)
            {
                case MessageBoxIcon.Error: //Error
                    this.pbIcon.IconChar = IconChar.TimesCircle;
                    this.pbIcon.IconColor = Color.FromArgb(241, 98, 96);
                    break;
                case MessageBoxIcon.Information: //Information
                    this.pbIcon.IconChar = IconChar.InfoCircle;
                    this.pbIcon.IconColor = Color.FromArgb(20, 161, 228);
                    break;
                case MessageBoxIcon.Question://Question
                    this.pbIcon.IconChar = IconChar.QuestionCircle;
                    this.pbIcon.IconColor = Color.CornflowerBlue;
                    break;
                case MessageBoxIcon.Exclamation://Exclamation
                    this.pbIcon.IconChar = IconChar.ExclamationTriangle;
                    this.pbIcon.IconColor = Color.FromArgb(255, 177, 17);
                    break;
                case MessageBoxIcon.None: //None
                    this.pbIcon.IconChar = IconChar.CommentDots;
                    this.pbIcon.IconColor = Color.FromArgb(236, 93, 123);
                    break;

            }
        }
        private void CreateButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {//Set message box buttons and indicate  the default button for the message box

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    button1 = OKButton(143, 13);
                    break;
                case MessageBoxButtons.OKCancel:
                    button1 = OKButton(79, 13);
                    button2 = CancelButton(205, 13);
                    break;
                case MessageBoxButtons.YesNo:
                    button1 = YesButton(79, 13);
                    button2 = NoButton(205, 13);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    button1 = YesButton(20, 13);
                    button2 = NoButton(146, 13);
                    button3 = CancelButton(272, 13);
                    break;
                case MessageBoxButtons.RetryCancel:
                    button1 = RetryButton(79, 13);
                    button2 = CancelButton(205, 13);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    button1 = AbortButton(20, 13);
                    button2 = RetryButton(146, 13);
                    button3 = IgnoreButton(272, 13);
                    break;
            }
            ActivateButton(defaultButton);//Specify the default button for the message box.
        }
        private void ActivateButton(MessageBoxDefaultButton defaultButton)
        {//Focus the default button and user can directly press Enter key to perform button action
            switch (defaultButton)
            {
                case MessageBoxDefaultButton.Button1://Focus button 1
                    button1.Select();
                    button1.ForeColor = Color.White;
                    button1.Text = "> " + button1.Text;
                    break;
                case MessageBoxDefaultButton.Button2://Focus button 2
                    if (button2 != null)
                    {
                        button2.Select();
                        button2.Text = "> " + button2.Text;
                        button2.ForeColor = Color.White;

                    }
                    else //If the button 2 does not exist, Focus button 1
                    {
                        button1.Select();
                        button1.ForeColor = Color.White;
                        button1.Text = "> " + button1.Text;
                    }
                    break;
                case MessageBoxDefaultButton.Button3://Focus button 3
                    if (button3 != null)
                    {
                        button3.Select();
                        button3.ForeColor = Color.White;
                        button3.Text = "> " + button3.Text;
                    }
                    else //If the button 3 does not exist, Focus button 1
                    {
                        button1.Select();
                        button1.ForeColor = Color.White;
                        button1.Text = "> " + button1.Text;
                    }
                    break;
            }
        }
        //Message box buttons
        private Button OKButton(int locationX, int locationY)
        {//Create Ok Button

            Button btnOk;
            btnOk = new Button();
            btnOk.Anchor = AnchorStyles.None;
            btnOk.BackColor = UIAppearance.StyleColor;
            btnOk.FlatAppearance.BorderSize= 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnOk.ForeColor = Color.WhiteSmoke;
            btnOk.Location = new Point(locationX, locationY);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(110, 35);
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = false;

            btnOk.DialogResult = DialogResult.OK;//Set DialogResult

            pnlButtons.Controls.Add(btnOk);//Add button

            return btnOk;

        }
        private new Button CancelButton(int locationX, int locationY)
        {//Create Cancel Button

            Button btnCancel;
            btnCancel = new Button();
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.BackColor = RJColors.Cancel;
            btnCancel.FlatAppearance.BorderSize= 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnCancel.ForeColor = Color.WhiteSmoke;
            btnCancel.Location = new Point(locationX, locationY);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 35);
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;

            btnCancel.DialogResult = DialogResult.Cancel;//Set DialogResult

            pnlButtons.Controls.Add(btnCancel);//Add button

            return btnCancel;
        }
        private Button YesButton(int locationX, int locationY)
        {//Create Yes Button

            Button btnYes;
            btnYes = new Button();
            btnYes.Anchor = AnchorStyles.None;
            btnYes.BackColor = RJColors.Confirm;
            btnYes.FlatAppearance.BorderSize= 0;
            btnYes.FlatStyle = FlatStyle.Flat;
            btnYes.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnYes.ForeColor = Color.WhiteSmoke;
            btnYes.Location = new Point(locationX, locationY);
            btnYes.Name = "btnYes";
            btnYes.Size = new Size(110, 35);
            btnYes.Text = "Yes";
            btnYes.UseVisualStyleBackColor = false;

            btnYes.DialogResult = DialogResult.Yes;//Set DialogResult

            pnlButtons.Controls.Add(btnYes);//Add button

            return btnYes;
        }
        private Button NoButton(int locationX, int locationY)
        {//Create No Button

            Button btnNo;
            btnNo = new Button();
            btnNo.Anchor = AnchorStyles.None;
            btnNo.BackColor = RJColors.Delete;
            btnNo.FlatAppearance.BorderSize= 0;
            btnNo.FlatStyle = FlatStyle.Flat;
            btnNo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnNo.ForeColor = Color.WhiteSmoke;
            btnNo.Location = new Point(locationX, locationY);
            btnNo.Name = "btnNo";
            btnNo.Size = new Size(110, 35);
            btnNo.Text = "No";
            btnNo.UseVisualStyleBackColor = false;

            btnNo.DialogResult = DialogResult.No;//Set DialogResult

            pnlButtons.Controls.Add(btnNo);//Add button

            return btnNo;
        }
        private Button RetryButton(int locationX, int locationY)
        {//Create RetryButton

            Button btnRetry;
            btnRetry = new Button();
            btnRetry.Anchor = AnchorStyles.None;
            btnRetry.BackColor = UIAppearance.StyleColor;
            btnRetry.FlatAppearance.BorderSize= 0;
            btnRetry.FlatStyle = FlatStyle.Flat;
            btnRetry.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnRetry.ForeColor = Color.WhiteSmoke;
            btnRetry.Location = new Point(locationX, locationY);
            btnRetry.Name = "btnRetry";
            btnRetry.Size = new Size(110, 35);
            btnRetry.Text = "Retry";
            btnRetry.UseVisualStyleBackColor = false;

            btnRetry.DialogResult = DialogResult.Retry;//Set DialogResult

            pnlButtons.Controls.Add(btnRetry);//Add button

            return btnRetry;
        }
        private Button AbortButton(int locationX, int locationY)
        {//Create Abort Button
            Button btnAbort;
            btnAbort = new Button();
            btnAbort.Anchor = AnchorStyles.None;
            btnAbort.BackColor = RJColors.Delete;
            btnAbort.FlatAppearance.BorderSize= 0;
            btnAbort.FlatStyle = FlatStyle.Flat;
            btnAbort.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnAbort.ForeColor = Color.WhiteSmoke;
            btnAbort.Location = new Point(locationX, locationY);
            btnAbort.Name = "btnAbort";
            btnAbort.Size = new Size(110, 35);
            btnAbort.Text = "Abort";
            btnAbort.UseVisualStyleBackColor = false;

            btnAbort.DialogResult = DialogResult.Abort;//Set DialogResult

            pnlButtons.Controls.Add(btnAbort);//Add button

            return btnAbort;
        }
        private Button IgnoreButton(int locationX, int locationY)
        {//Create Ignore Button

            Button btnIgnore;
            btnIgnore = new Button();
            btnIgnore.Anchor = AnchorStyles.None;
            btnIgnore.BackColor = RJColors.Cancel;
            btnIgnore.FlatAppearance.BorderSize= 0;
            btnIgnore.FlatStyle = FlatStyle.Flat;
            btnIgnore.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            btnIgnore.ForeColor = Color.WhiteSmoke;
            btnIgnore.Location = new Point(locationX, locationY);
            btnIgnore.Name = "btnIgnore";
            btnIgnore.Size = new Size(110, 35);
            btnIgnore.Text = "Ignore";
            btnIgnore.UseVisualStyleBackColor = false;

            btnIgnore.DialogResult = DialogResult.Ignore;//Set DialogResult

            pnlButtons.Controls.Add(btnIgnore);//Add Button

            return btnIgnore;
        }
        #endregion

        #region Event Methods Definition 

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();//Message Box Close            
        }
        #endregion

        private void RJMessageForm_Load(object sender, EventArgs e)
        {

        }
    }

}
