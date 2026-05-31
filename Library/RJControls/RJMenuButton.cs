using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using FontAwesome.Sharp;
using RJCodeUI_M1.RJForms;

namespace RJCodeUI_M1.RJControls
{
    public class RJMenuButton : IconButton
    {
        /// <summary>
        ///------This class inherits from the <see cref="IconButton"/> class of the <see cref="FontAwesome.Sharp"/> library
        ///      Autor: mkoertgen
        ///      GitHub: https://github.com/awesome-inc/FontAwesome.Sharp
        ///      Nuget Package: https://www.nuget.org/packages/FontAwesome.Sharp
        ///      
        /// This is a special control that is only designed to be used in the side menu of the main form.
        /// This control can work as a normal menu button or as a drop-down menu button, for this you need 
        /// to add a drop-down menu (<see cref="RJDropdownMenu"/>) from the properties(<see cref="DropdownMenu"/>)  
        /// of this control and the click event will be created automatically to show the drop-down menu.
        /// It has 2 essential appearance methods:
        /// -> As a normal menu button; allows associating a form and the button is activated/highlighted 
        /// until the form is closed<see cref="Activate(RJChildForm)"/>
        /// -> As a drop-down menu button; allows associating many forms, the button and menu item is 
        /// activated/highlighted until all forms are closed <see cref="Activate(RJChildForm, ToolStripMenuItem)"/>
        /// These methods help the user to identify which forms are open, it works in a similar way as tabs.
        /// </summary>
        /// 

        #region -> Fields

        //Fields
        private string textLeftPadding;//Sets a padding to the left before the text to keep considerable space between the icon and text
        private string tempText;//Sets or gets temporary text in case side menu collapses (text is set empty)
        private bool isDropdownMenu;//Sets or gets if the button is a dropdown menu
        private int guestFormsCount;//Stores the number of associated forms that the menu button has (if the button is not a drop-down menu it will only have an associated form and if the button is a drop-down menu it will have 2 or more associated forms)
        private Color tempMenuItemTextColor;//Sets or gets the temporary text color of the menu item in case it is activated /highlighted
        private Color supernovaColor = UIAppearance.Style == UIStyle.Supernova ? RJColors.GetSupernovaColor() : Color.CornflowerBlue;//Sets and Gets supernova color if the style is supernova

        //Control Fields
        private RJDropdownMenu dropdownMenu;//Sets or Gets dropdown menu
        private IconPictureBox pbDropdownArrowIcon;//Sets or gets dropdown angle icon in case button displays dropdown menu        

        //Static Fields
        private static readonly Color deactivateItemsColor = Color.FromArgb(122, 119, 170);//Sets or Gets Icon and text color when the menu button is deactivated
        private static readonly Color activateForeColor = Color.FromArgb(206, 203, 226);//Sets or gets Text Color when menu button is activated
        private static readonly Color activateBackColor = Color.FromArgb(58, 50, 97);//Sets or gets back Color when menu button is activated and dropdown menu is displayed

        #endregion

        #region -> Constructor
        public RJMenuButton()
        {
            pbDropdownArrowIcon = new IconPictureBox();
            //initialize properties and events
            this.Dock = DockStyle.Top;
            this.BackColor = RJColors.SideMenuColor;
            this.FlatAppearance.BorderSize= 0;
            this.FlatAppearance.MouseDownBackColor = RJColors.DarkItemBackground;
            this.FlatStyle = FlatStyle.Flat;
            this.Flip = FlipOrientation.Normal;
            this.Font = new Font("Verdana", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = deactivateItemsColor;
            this.IconChar =FontAwesome.Sharp. IconChar.DiceD6;
            this.IconColor = deactivateItemsColor;
            this.IconSize = 28;
            this.Rotation = 0D;
            this.Size = new Size(220, 55);
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.Padding = new Padding(12, 0, 0, 0);
            this.SizeChanged += new EventHandler(WidthChanged);
            this.HandleCreated += new EventHandler(MB_HandleCreated);
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (value != null)
                {
                    textLeftPadding = "   ";//Set the number of whitespace to simulate a padding to the left before the text
                    base.Text = textLeftPadding + value.Trim();//Set padding and concatenate text by trimming leading and trailing empty characters
                }
            }
        }

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

                if (dropdownMenu != null)//if the value is not null (Its an object), Set as dropdown menu button
                {
                    isDropdownMenu = true;//Set menu button as dropdown menu
                    dropdownMenu.VisibleChanged += new EventHandler(DropdownMenu_VisibleChanged);//Subscribe RJDropdownMenu.VisibleChanged event
                    DrawDropdownArrowIcon(false);//Draw a dropdown angle icon
                    this.Click += new EventHandler(DropdownMenuButton_Click);//Subscribe click event (show dropdown menu)
                }
                else //if the value is null (Is none), Set normal menu button
                {
                    isDropdownMenu = false;// Set menu button as no dropdown menu
                    this.BackgroundImage = null;//Draw nothing
                }
            }
        }
        #endregion

        #region -> Private methods
        public void Activate(RJChildForm guestForm)
        {//-> Normal Menu button
            ///<summary>
            ///This method allows activating / highlighting the menu button from the outside, at the same time 
            ///associating a guest form (the button will remain activated when the form is open or instantiated 
            ///and once the form is closed the button will be deactivated) Use this method when you open a form 
            ///directly from the menu button (the button is not a drop-down menu and it will only have an associated form).
            /// </summary>
            ///
            ActivateButton();//Activate / highlight the menu button
            guestForm.FormClosed += new FormClosedEventHandler(GuestForm_Closed);//Subscribe to a closed form event, to deactivate the menu button
            guestFormsCount = 1;//set associated form to one, this is optional since by default it is 0, therefore once the form is closed the button will be deactivated. 
        }
        public void Activate(RJChildForm guestForm, ToolStripMenuItem menuItem)
        {//-> Dropdown Menu Button
            ///<summary>
            ///This method allows activating / highlighting the menu button and the sender menu item from the outside, 
            ///at the same time associating a guest form (the menu button will remain activated when the form is open or 
            ///instantiated and once the menu button has no associated forms, the button will be deactivated) , 
            ///Use this method when the button is a dropdown menu and you open a form from the menu item (the menu 
            ///button is a drop down menu and can have many associated forms).
            /// </summary>
            ///
            ActivateButton();//Activate / highlight the menu button         
            ActivateMenuItem(guestForm, menuItem);//Activate / highlight the menu item
            if (this.Width > 100)//if width is greater than 100, i.e. side menu is expanded,
                DrawDropdownArrowIcon(false);// Draw the collapsed arrow icon, i.e the dropdown menu is not displayed.

            guestForm.FormClosed += new FormClosedEventHandler((sender, e) => GuestForm_Closed(sender, e, menuItem));
            //Subscribe to a closed form event, to deactivate the menu item (pass additional parameter to the event using delegate via lambda expressions)
            //More information: https://stackoverflow.com/a/34622752/12778930 

            guestFormsCount++;//Increment the counter of associated forms by one
        }
        private void ActivateButton()
        {//Activate / highlight menu button

            if (guestFormsCount == 0)
            {//If the button does not have an associated form yet (This condition to not reactivate the button  
             //already has an associated form, therefore the button has already been activated previously)

                this.ForeColor = activateForeColor;//set menu button text color 
                if (UIAppearance.Style == UIStyle.Supernova)
                    this.IconColor = supernovaColor;//If the style is supernova, set the icon color to a supernova color
                else this.IconColor = UIAppearance.StyleColor;//If it is any other theme, set the style color as the icon color
            }
        }
        private void DeactivateButton()
        {
            if (guestFormsCount == 0)//Deactivate the menu button as long as it doesn't have an associated form
            {
                this.IconColor = deactivateItemsColor;
                this.ForeColor = deactivateItemsColor;
                if (this.Width > 100 && isDropdownMenu)//if width is greater than 100 (i.e. side menu is expanded) and menu button is dropdown menu,  
                    DrawDropdownArrowIcon(false);//Draw the collapsed arrow icon, i.e the dropdown menu is not displayed.

            }
        }
        private void ShowDropdownMenu()
        {
            try //Show the button drop down menu
            {
                dropdownMenu.OwnerIsMenuButton = true; //Indicate to the drop-down menu that its owner is a menu button
                if (this.Width > 100)//If the side menu is expanded, show the dropdown menu at the bottom right of the menu button
                    dropdownMenu.Show(this, DropdownMenuPosition.BottomRight);
                else //If the side menu is collapsed, show the dropdown menu at the top right of the menu button
                    dropdownMenu.Show(this, DropdownMenuPosition.TopRight);
            }
            catch (Exception Desc)
            {
                //RJMessageBox.Show("An error has occurred\n" + ex.ToString());
            }
        }
        private void DrawDropdownArrowIcon(bool expandedMenu)
        {//this method draws the arrow icon of the drop-down menu button

            int iconSize = 25;//Define an set icon size
            Color iconColor = this.IconColor;//Define and set icon color
            Bitmap arrowIcon;//Define the bitmap for the arrow icon

            pbDropdownArrowIcon.IconSize = iconSize;//Set icon size
            pbDropdownArrowIcon.IconColor = iconColor;//Set icon color
            if (expandedMenu) pbDropdownArrowIcon.IconChar =FontAwesome.Sharp. IconChar.AngleDown;//If the parameter is an expanded menu, set the down arrow as the drop-down menu icon
            else pbDropdownArrowIcon.IconChar = FontAwesome.Sharp.IconChar.AngleRight;//If the parameter is not an expanded menu, set the right arrow as the drop-down menu icon           

            arrowIcon = new Bitmap(this.Width, this.Height);//initialize bitmap to the same size as the menu button
            using (Graphics drawIcon = Graphics.FromImage(arrowIcon))//Create a graphic object from the bitmap
            {
                Point point = new Point((this.Width - iconSize) - 15, ((this.Height - iconSize) / 2) + 3);//Set arrow icon location (docked right and center) 
                var rectangle = new Rectangle(point, new Size(iconSize, iconSize));//Create a rectangle with the specified location and size equal to the size of the icon

                drawIcon.SmoothingMode = SmoothingMode.AntiAlias;//Set Smoothing Mode
                drawIcon.DrawImage(pbDropdownArrowIcon.Image, rectangle);//Draws the arrow icon at the specified location on the bitmap
                this.BackgroundImage = arrowIcon; //set arrow icon as button menu background
            }
        }
        private void ActivateMenuItem(RJChildForm guestForm, ToolStripMenuItem menuItem)
        { //Activate / highlight MenuItem

            Color iconColor;//Define icon color
            tempMenuItemTextColor = menuItem.ForeColor;//Save text color
            menuItem.ForeColor = activateForeColor;//Set active text color
            dropdownMenu.ActiveMenuItem = true;//Indicate the drop-down menu that you have an active menu

            //Set Menu Item Icon Color
            if (UIAppearance.Style == UIStyle.Supernova)
                iconColor = RJColors.GetSupernovaColor();
            else iconColor = UIAppearance.StyleColor;

            if (UIAppearance.FormIconActiveMenuItem)//If the setting is set to show the form icon in the active menu item
            {                
				//Get and Set form icon to menu item.
                Bitmap formIcon = guestForm.FormIcon.ToBitmap(iconColor, 25);
                menuItem.Image = formIcon; 
            }
            else //otherwise set simple shape as active menu item icon
            {
                Bitmap iconShape = new Bitmap(14, 14); //Create a bitmap object with size 14

                using (Graphics graphics = Graphics.FromImage(iconShape))//Create a graphic object from the bitmap
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;//Set Smoothing Mode
                    using (Brush brush = new SolidBrush(iconColor))
                    {
                        graphics.FillEllipse(brush, 0, 0, 13, 13);//Draw a perfect circular fill
                    }
                }
                menuItem.Image = iconShape;//Set the bitmap as the icon of the active menu item
            }
        }
        private void DeactivateMenuItem(ToolStripMenuItem menuItem)
        {
            if (menuItem != null)//Deactivate menu item
            {
                menuItem.Image = null;//Remove icon   
                menuItem.ForeColor = tempMenuItemTextColor;//Restore original text color
                if (guestFormsCount == 0)//if the dropdown button no longer has associated forms, 
                    dropdownMenu.ActiveMenuItem = false;//indicate the dropdown that no longer has an active menu
            }
        }
        #endregion

        #region -> Event Methods 
        private void MB_HandleCreated(object sender, EventArgs e)
        {
            if (!this.DesignMode)//Run this when not in design mode
                tempText = this.Text;//Save text menu button
        }
        private void GuestForm_Closed(object sender, FormClosedEventArgs e)
        {
            ///see predecessor method<see cref="Activate(RJChildForm guestForm)"/>

            guestFormsCount = 0;//Set associated form to 0
            DeactivateButton();//Deactivate menu button
        }
        private void GuestForm_Closed(object sender, FormClosedEventArgs e, ToolStripMenuItem menuItem)
        {
            ///see predecessor method<see cref="Activate(RJChildForm guestForm, ToolStripMenuItem menuItem))"/>

            guestFormsCount--; //When the form is closed, decrement the counter of associated forms by one
            DeactivateButton();//Deactivate menu button
            DeactivateMenuItem(menuItem);//Deactivate menu item
        }
        private void DropdownMenuButton_Click(object sender, EventArgs e)
        {
            ShowDropdownMenu();
        }
        private void DropdownMenu_VisibleChanged(object sender, EventArgs e)
        {
            //This event happens when the dropdown menu is shown or hidden

            if (!DesignMode)//Run this when not in design mode
            {
                if (dropdownMenu.Visible)//If the dropdown menu was displayed
                {
                    ActivateButton();//Activate menu button
                    this.BackColor = activateBackColor;//Set active back color
                    if (this.Width > 100)//if width is greater than 100, i.e. side menu is expanded,
                        DrawDropdownArrowIcon(true);// Draw expanded arrow icon
                }
                else {//If the dropdown menu has been hidden (Close)               

                    DeactivateButton();//deactivate menu button (It is also responsible for drawing the arrow icon when the button does not have associated forms)
                    this.BackColor = RJColors.SideMenuColor;//Restore back color
                    if (this.Width > 100 && guestFormsCount != 0)//This condition is to draw the arrow icon when the button has associated forms
                        DrawDropdownArrowIcon(false);// Draw collapsed arrow icon
                }
            }
        }
        private void WidthChanged(object sender, EventArgs e)
        {
            //this event will happen when the side menu is collapsed or expanded (the button has the dock property set to top, don't change that)

            if (!this.DesignMode)//Run this when not in design mode
            {
                if (this.Size.Width < 100) //If the side menu is collapsed
                {
                    this.Text = "";//Set empty text
                    if (isDropdownMenu)//If the button is drop-down menu remove the arrow icon
                        this.BackgroundImage = null;
                }
                else //If the side menu is expanded
                {
                    this.Text = tempText;//Restore text menu button
                    if (isDropdownMenu)//If the button is drop-down menu draw collapsed arrow icon
                        DrawDropdownArrowIcon(false);
                }
            }
        }
        #endregion

    }
}