using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;


namespace RJCodeUI_M1.RJControls
{
    public class RJMenuIcon : IconPictureBox
    {
        /// <summary>
        /// This class inherits from the <see cref="IconPictureBox"/> class of the <see cref="FontAwesome.Sharp"/> library
        ///      Autor: mkoertgen
        ///      GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        ///      Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp
        ///This is a special control that is designed primarily to be used on the title bar of the main form,
        ///however you can disable that by setting the BackIcon property to true (Icon color equals text color)
        ///This control can work as a normal button or as a drop down menu button, for this you need to add 
        ///a drop down menu(<see cref = "RJDropdownMenu" />) of the properties of this control (<see cref="DropdownMenu"/>)
        ///and the click event will be automatically created to display the drop-down menu.
        ///You can change the appearance color, but you must set the customizable property to TRUE.
        /// </summary>

        #region -> Fields
        //Fields
        private Color supernovaColor;//Sets a color for the supernova style
        private Color tempColor;//Sets a temporary color for the icon        
        private int tempSize;//Sets a temporary size for the icon        
        private bool dropdownMenuIsDisplayed;//Sets if the control displays a dropdown menu
        private bool customizable;//Sets if the control can be customized regardless of the style set
        private bool backIcon;//Gets or sets whether the control is an icon that can be used in the client area of any form.

        //Control Fields
        private RJDropdownMenu dropdownMenu;//Sets and Gets dropdown menu
        #endregion

        #region -> Constructor

        public RJMenuIcon()
        {
            this.IconChar = FontAwesome.Sharp.IconChar.Github;
            this.IconColor = Color.Crimson;
            this.IconSize = 25;
            this.BackColor = Color.Transparent;
            this.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Cursor = Cursors.Hand;
            this.HandleCreated += new EventHandler(BarIcon_Load);//Apply the settings when the HandleCreated event is activated
            this.MouseEnter += new EventHandler(BarIcon_MouseEnter);//When mouse hovers over control
            this.MouseLeave += new EventHandler(BarIcon_MouseLeave);//When the mouse leaves control
        }
        #endregion

        #region -> Properties

        [Browsable(false)]//Hide ContextMenuStrip property
        [ReadOnly(true)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { }
        }
        [Category("RJ Code Advance")]///create a new property to associate a <see cref="RJDropdownMenu"/>
        [Description("Gets or sets the RJDropdownMenu associated with this control")]
        public RJDropdownMenu DropdownMenu
        {
            get
            {
                return dropdownMenu;//Return RJDropdownMenu
            }
            set
            {
                dropdownMenu = value;//Set value
                if (dropdownMenu != null)
                {
                    dropdownMenu.VisibleChanged += new EventHandler(DropdownMenu_VisibleChanged);//Subscribe RJDropdownMenu.VisibleChanged event
                    this.Click += new EventHandler(Show_DropdownMenu);//Subscribe click event (show dropdown menu)                 
                }
            }
        }
        [Category("RJ Code Advance")]
        public bool BackIcon
        {//Sets or gets if the control is a menu icon in the title bar of the main form (False),
            //or is a menu icon in any client area of the form (True).
            get { return backIcon; }
            set
            {
                backIcon = value;
                if (this.DesignMode)
                    ApplyAppearanceSettings();
            }
        }

        [Category("RJ Code Advance")]
        public bool Customizable
        {//Sets if the control can be customized regardless of the style set
            get { return customizable; }
            set { customizable = value; }
        }
        #endregion

        #region -> Private methods
        private void ShowDropdownMenu()
        {
            try //Show drop down menu
            {
                dropdownMenu.Show(this, DropdownMenuPosition.BottomRight);
            }
            catch (Exception Desc)
            {
                //RJMessageBox.Show("An error has occurred\n" + ex.ToString());
            }
        }
        private void ApplyAppearanceSettings()
        {
            
            if (customizable == false)//Apply the setting as long as it is not customizable
            {              
                    //this.BackColor = Color.Transparent;
                if (backIcon == true)//Icon on any background of a form.
                {
                    this.IconColor = UIAppearance.TextColor;
                }
                else //Main form title bar icon
                {
                    if (UIAppearance.Theme == UITheme.Dark && UIAppearance.Style == UIStyle.Supernova)//if the theme is dark  and supernova skin, the icon color will be the same lightened color of the normal paragraph text
                        this.IconColor = ColorEditor.Lighten(UIAppearance.TextColor, 65);
                    else if (UIAppearance.Theme == UITheme.Light && UIAppearance.Style == UIStyle.Supernova)//if the theme is light  and supernova skin, the icon color will be the same lightened color as the normal paragraph text
                        this.IconColor = ColorEditor.Darken(UIAppearance.TextColor, 25);
                    else//if the style is another, the icon color will be white
                        this.IconColor = Color.WhiteSmoke;
                }
            }
        }
        #endregion

        #region -> Event Methods

        private void BarIcon_Load(object sender, EventArgs e)
        {//Apply the setting when the HandleCreated event is activated

            ApplyAppearanceSettings();
            if (UIAppearance.Style == UIStyle.Supernova)//if the style is supernova get a color
                supernovaColor = RJColors.GetSupernovaColor();
        }
        private void BarIcon_MouseEnter(object sender, EventArgs e)
        {//When mouse hovers over control, enlarge the icon size and if the style is supernova change the icon color

            tempSize = this.IconSize;//Save original size
            this.IconSize = this.IconSize + 2;//Set new icon size
            if (UIAppearance.Style == UIStyle.Supernova)
            {
                tempColor = this.IconColor;//Save original color
                this.IconColor = supernovaColor;//Set supernova color
            }
        }
        private void BarIcon_MouseLeave(object sender, EventArgs e)
        {//When the mouse leaves control

            if (!dropdownMenuIsDisplayed)//Reset original values as long as the dropdown menu is not open (showned)
            {
                this.IconSize = tempSize;
                if (UIAppearance.Style == UIStyle.Supernova)
                    this.IconColor = tempColor;
                ///<Note>this is to keep the bar icon activated(highlighted) when the dropdown menu is displayed</Note>
            }
        }
        private void Show_DropdownMenu(object sender, EventArgs e)
        {
            ShowDropdownMenu();
        }
        private void DropdownMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.DesignMode)//Run this when not in design mode
            {
                if (dropdownMenu.Visible)//If the dropdown menu was displayed
                    dropdownMenuIsDisplayed = true;//Set true

                else//If the dropdown menu has been hidden (Closed)
                {
                    dropdownMenuIsDisplayed = false;//Set false
                    //Reset original values                  
                    this.IconSize = tempSize;
                    if (UIAppearance.Style == UIStyle.Supernova)
                        this.IconColor = tempColor;
                }
            }
        }
        #endregion
    }
}
