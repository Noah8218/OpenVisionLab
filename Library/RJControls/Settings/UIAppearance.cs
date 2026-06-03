using System;
using System.Drawing;

namespace RJCodeUI_M1.Settings
{
    public struct UIAppearance
    {
        //UI Appearance Settings     (Default settings)
        public static UITheme Theme = UITheme.Light;//Sets or gets the theme, either dark or light.
        public static UIStyle Style = UIStyle.Neptune;////Sets or gets the style
        public static Color PrimaryStyleColor = RJColors.Sky; //Sets or gets the style color of the form title bar
        public static Color StyleColor = RJColors.Sky; //Sets or gets the style color of the buttons, comboboxs, datetime pickers, datagridviews, checkboxs, radio buttons and others (the color is a little more opaque than the PrimaryStyleColor field - See SettingsManagger Class)   
        public static Color BackgroundColor = RJColors.LightBackground;//Sets or gets the background color of forms
        public static Color ItemBackgroundColor = RJColors.LightItemBackground;
        public static Color ActiveBackgroundColor = RJColors.LightActiveBackground;
        public static Color PrimaryTextColor = RJColors.LightTextColor;//Sets or gets the color of the text of the text boxes, combo box, date time picker (The color is a little more highlighted than the TextColor field - See SettingsManagger Class)
        public static Color TextColor = RJColors.LightTextColor;//Sets or gets Text Color of the label, radio button and others, also applies to the text and BarIcon control in the title bar of the main form in the supernova style.
        public static Color FormBorderColor = RJColors.Sky;//Sets or gets the border color of the form        
        public static Color DeactiveFormColor = Color.FromArgb(76, 70, 116);//Sets or gets the title bar color when the form is deactivate.
        
        public static int FormBorderSize = 1;//Sets the width of the form border        
        public static bool ChildFormMarker = true;//Sets or gets if the child form marker will be displayed on the main form side menu menu button
        public static bool FormIconActiveMenuItem = false; //Sets or gets if the form icon will be displayed in an active menu item
        public static bool MultiChildForms = true;//Sets or gets if the user can open multiple child forms within the desktop panel or can simply open a single form (close the previous one and open the new form)       
        public static string TextFamilyName = "Verdana";
        public static float TextSize = 9.5F;//Sets a default font size, applies to most custom controls at design time, 
        //however it can be changed later from the control's properties
        //unless there is a restriction on a specific control       
    }
}
