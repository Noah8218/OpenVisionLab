using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;

namespace RJCodeUI_M1.RJControls
{
    public class RJLabel : Label
    {
        #region -> Fields

        private LabelStyle style;//Gets or Sets Label style     
        private bool linkLabel;//Gets or set if it is a link label (Open a browser or form)
        private Color textColor;
        #endregion

        #region -> Constructor
        public RJLabel()
        {
            this.ForeColor = UIAppearance.TextColor;//Set Theme text color
            this.Font = new Font("Verdana", UIAppearance.TextSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));//Set default font
            style = LabelStyle.Normal;//Set label style
            this.MouseEnter += new EventHandler(Label_MouseEnter);
            this.MouseLeave += new EventHandler(Label_MouseLeave);
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        public LabelStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                ApplyAppearanceSettings();//Apply theme when set label style
            }
        }

        [Category("RJ Code Advance")]
        public bool LinkLabel
        {//Gets or sets if the label is a link 
            //(If true the label changes pointer and text color when the mouse passes over it, see event methods)
            get { return linkLabel; }
            set
            {
                linkLabel = value;
                if (linkLabel == true)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Arrow;
            }
        }

        #endregion

        #region -> Private methods

        private void ApplyAppearanceSettings()
        {
            switch (style)
            {
                case LabelStyle.Normal: //If the style is normal set the color and size of the text set in the theme                    
                    textColor = UIAppearance.TextColor;
                    this.ForeColor = textColor;
                    this.Font = new Font(this.Font.Name, UIAppearance.TextSize, this.Font.Style, this.Font.Unit);
                    break;

                case LabelStyle.Description: //If the style is description
                    if (UIAppearance.Theme == Settings.UITheme.Dark) //If the theme is dark style lighten the text color
                        textColor = ColorEditor.Darken(UIAppearance.TextColor, 10);
                    else //If the theme is light style darken the text color
                        textColor = ColorEditor.Lighten(UIAppearance.TextColor, 15);

                    this.ForeColor = textColor;
                    this.Font = new Font(this.Font.Name, 8F, this.Font.Style, this.Font.Unit);
                    break;

                case LabelStyle.Subtitle://If the style is subtitle set the color set in the theme and a size of 12.5F
                    if (UIAppearance.Theme == Settings.UITheme.Dark) //If the theme is dark style lighten the text color
                        textColor = ColorEditor.Lighten(UIAppearance.TextColor, 45);
                    else //If the theme is light style darken the text color
                        textColor = ColorEditor.Darken(UIAppearance.TextColor, 20);

                    this.ForeColor = textColor;
                    this.Font = new Font(this.Font.Name, 12F, this.Font.Style, this.Font.Unit);
                    break;

                case LabelStyle.Title://If the style is title set the style color as text color or a size of 16F.
                    textColor = UIAppearance.StyleColor;
                    this.ForeColor = textColor;
                    this.Font = new Font(this.Font.Name, 14F, this.Font.Style, this.Font.Unit);
                    break;

                case LabelStyle.Title2:
                    if (UIAppearance.Theme == Settings.UITheme.Dark) //If the theme is dark style lighten the text color
                        textColor = ColorEditor.Lighten(UIAppearance.TextColor, 50);
                    else //If the theme is light style darken the text color
                        textColor = ColorEditor.Darken(UIAppearance.TextColor, 25);
                    this.ForeColor = textColor;
                    this.Font = new Font(this.Font.Name, 14F, this.Font.Style, this.Font.Unit);
                    break;

                case LabelStyle.BarCaption://If the style is a caption of the title bar of the main form.
                    this.Font = new Font("Verdana", 12.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    ColorTitleBar();
                    break;

                case LabelStyle.BarText://If the style is a text of the title bar of the main form.
                    this.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    ColorTitleBar();
                    break;

                case LabelStyle.Custom://Custom size and font, however text color is set by theme
                    //textColor = UIAppearance.TextColor;
                    //this.ForeColor = textColor;
                    break;
            }
        }
        private void ColorTitleBar()
        {
            if (UIAppearance.Theme == UITheme.Light && UIAppearance.Style == UIStyle.Supernova)//If the theme is light and the style is supernova, darken the text color.
                this.ForeColor = ColorEditor.Darken(UIAppearance.TextColor, 25);
            else if (UIAppearance.Theme == UITheme.Dark && UIAppearance.Style == UIStyle.Supernova) //If the theme is dark and the style is supernova, lighten the text color.
                this.ForeColor = ColorEditor.Lighten(UIAppearance.TextColor, 65);
            else //If the style is any other set white as the text color
                this.ForeColor = Color.WhiteSmoke;
        }
        #endregion 

        #region -> Event Methods

        private void Label_MouseEnter(object sender, EventArgs e)
        {//If the label is of type link, change the text color when the mouse passes over it.
            if (linkLabel == true)
                this.ForeColor = ColorEditor.Lighten(UIAppearance.StyleColor, 20);
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {//If the label is of type link, return the text color when the mouse leaves the control.
            if (linkLabel == true)
                this.ForeColor = textColor;
        }
        #endregion
    }
}