using System.Drawing;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;

namespace RJCodeUI_M1.RJControls
{
    class DropdownMenuColors : ProfessionalColorTable
    {
        /// <summary>
        /// This class inherits from the <see cref="ProfessionalColorTable"/> Class that provides colors used for Microsoft Office display elements.
        /// Also note that the <see cref="ToolStripProfessionalRenderer"/> class calls an instance of this class
        /// in this case the derived class <see cref="DropdownMenuRenderer"/> that inherits this class
        /// </summary>

        //Fields Definition
        private Color dropdownMenuBackground;//Sets the background color of the dropdown menu
        private Color dropdownMenuBorder;//Sets the border color of the dropdown menu
        private Color leftColumnGradientBegin;//Sets the start color for the left column of the dropdown menu
        private Color leftColumnGradientMiddle;//Sets the middle color for the left column of the dropdown menu
        private Color leftColumnGradientEnd;//Sets the end color for the left column of the dropdown menu
        private Color menuItemSelected;//Sets the background color of the selected menu item or when the mouse is hovering over it
        private Color menuItemBorder;//Sets the border color of the menu item


        //Constructor
        public DropdownMenuColors(bool menuButtonOwner)
        {
            if (UIAppearance.Theme==UITheme.Dark || menuButtonOwner == true)//If the theme is dark or the dropdown menu is displayed from the menu button, set the following colors
            {
                dropdownMenuBackground = RJColors.DarkItemBackground;
                dropdownMenuBorder = ColorEditor.Darken(RJColors.DarkItemBackground, 10);
                menuItemBorder = RJColors.DarkActiveBackground;
                menuItemSelected = RJColors.DarkActiveBackground;
                leftColumnGradientBegin = ColorEditor.Darken(RJColors.DarkItemBackground, 4);
                leftColumnGradientMiddle = ColorEditor.Darken(RJColors.DarkItemBackground, 4);
                leftColumnGradientEnd = ColorEditor.Darken(RJColors.DarkItemBackground, 4);
            }
            else //If the theme is light, set the following colors
            {
                dropdownMenuBackground = RJColors.LightItemBackground;
                dropdownMenuBorder = ColorEditor.Darken(UIAppearance.BackgroundColor, 10);
                menuItemBorder = RJColors.LightActiveBackground;
                menuItemSelected = RJColors.LightActiveBackground;
                leftColumnGradientBegin = ColorEditor.Darken(RJColors.LightItemBackground, 4);
                leftColumnGradientMiddle = ColorEditor.Darken(RJColors.LightItemBackground, 4);
                leftColumnGradientEnd = ColorEditor.Darken(RJColors.LightItemBackground, 4);
            }
        }
        //Override main layout color methods from drop down menu and Return the new color properties
        public override Color ToolStripDropDownBackground { get { return dropdownMenuBackground; } }///Gets the solid background color of the <see cref="ToolStripDropDown"/> 
        public override Color MenuBorder { get { return dropdownMenuBorder; } }///Gets the color that is the border color to use on a <see cref="MenuStrip"/>
        public override Color MenuItemBorder { get { return menuItemBorder; } }///Gets the border color to use with a <see cref="ToolStripMenuItem"/> 
        public override Color MenuItemSelected { get { return menuItemSelected; } }///Gets the solid color to use when a ToolStripMenuItem other than the top-level <see cref="ToolStripMenuItem"/> is selected        
        public override Color ImageMarginGradientBegin { get { return leftColumnGradientBegin; } }///Gets the starting color of the gradient used in the image margin of a <see cref="ToolStripDropDown"/> 
        public override Color ImageMarginGradientMiddle { get { return leftColumnGradientMiddle; } }///Gets the middle color of the gradient used in the image margin of a <see cref="ToolStripDropDown"/> 
        public override Color ImageMarginGradientEnd { get { return leftColumnGradientEnd; } }///Gets the end color of the gradient used in the image margin of a <see cref="ToolStripDropDown"/> 

    }
}
