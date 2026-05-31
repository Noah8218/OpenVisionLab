using RJCodeUI_M1.Settings;
using RJCodeUI_M1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public static class SettingsManager
    {
        //Save Appearance Settings
        public static void SaveAppearanceSettings(int theme,
                                                  int style,
                                                  int formBorderSize,
                                                  bool colorFormBorder,
                                                  bool childFormMarker,
                                                  bool formIconActiveMenuItem,
                                                  bool multiChildForms)
        {
            //Set values
            UIAppearanceSettings.Default.Theme = theme;
            UIAppearanceSettings.Default.Style = style;
            UIAppearanceSettings.Default.FormBorderSize = formBorderSize;
            UIAppearanceSettings.Default.ColorFormBorder = colorFormBorder;
            UIAppearanceSettings.Default.ChildFormMarker = childFormMarker;
            UIAppearanceSettings.Default.FormIconActiveMenuItem = formIconActiveMenuItem;
            UIAppearanceSettings.Default.MultiChildForms = multiChildForms;

            UIAppearanceSettings.Default.Save();//Save the current values to the application settings file.
        }
        //Load Apperance Settings
        public static void LoadApperanceSettings()
        {
            UIAppearance.Theme = (UITheme)UIAppearanceSettings.Default.Theme;//Set theme
            UIAppearance.Style = (UIStyle)UIAppearanceSettings.Default.Style;//Set style
            UIAppearance.FormBorderSize = UIAppearanceSettings.Default.FormBorderSize;//Set form border size            
            UIAppearance.ChildFormMarker = UIAppearanceSettings.Default.ChildFormMarker;//Set whether to show the marker of the current child form
            UIAppearance.FormIconActiveMenuItem = UIAppearanceSettings.Default.FormIconActiveMenuItem;//Set whether the active menu item icon is equal to the icon of its associated form.
            UIAppearance.MultiChildForms = UIAppearanceSettings.Default.MultiChildForms;//Set whether to open multiple child forms in the application

            //Set theme colors

            if (UIAppearance.Theme == UITheme.Light) //Light theme
            {
                UIAppearance.BackgroundColor = RJColors.LightBackground;
                UIAppearance.ItemBackgroundColor = RJColors.LightItemBackground;
                UIAppearance.ActiveBackgroundColor = RJColors.LightActiveBackground;
                UIAppearance.PrimaryTextColor = ColorEditor.Darken(RJColors.LightTextColor,15);
                UIAppearance.TextColor = RJColors.LightTextColor;
            }
            else //Dark theme
            {
                UIAppearance.BackgroundColor = RJColors.DarkBackground;
                UIAppearance.ItemBackgroundColor = RJColors.DarkItemBackground;
                UIAppearance.ActiveBackgroundColor = RJColors.DarkActiveBackground;
                UIAppearance.PrimaryTextColor = ColorEditor.Lighten(RJColors.DarkTextColor, 25);
                UIAppearance.TextColor = RJColors.DarkTextColor;
            }

            //Set style color
            switch (UIAppearance.Style)
            {
                case UIStyle.Axolotl:
                    UIAppearance.PrimaryStyleColor = RJColors.Axolotl;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Axolotl, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Axolotl, 15);
                    break;

                case UIStyle.FireOpal:
                    UIAppearance.PrimaryStyleColor = RJColors.FireOpal;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.FireOpal, 12);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.FireOpal, 15);
                    break;

                case UIStyle.Forest:
                    UIAppearance.PrimaryStyleColor = RJColors.Forest;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Forest, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Forest, 15);
                    break;

                case UIStyle.Lisianthus:
                    UIAppearance.PrimaryStyleColor = RJColors.Lisianthus;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Lisianthus, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Lisianthus, 15);
                    break;

                case UIStyle.Neptune:
                    UIAppearance.PrimaryStyleColor = RJColors.Neptune;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Neptune, 5);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Neptune, 15);
                    break;

                case UIStyle.Navi:
                    UIAppearance.PrimaryStyleColor = RJColors.Navi;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Navi, 5);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Navi, 15);
                    break;

                case UIStyle.Petunia:
                    UIAppearance.PrimaryStyleColor = RJColors.Petunia;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Petunia, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Petunia, 15);
                    break;

                case UIStyle.Ruby:
                    UIAppearance.PrimaryStyleColor = RJColors.Ruby;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Ruby, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Ruby, 15);
                    break;

                case UIStyle.Sky:
                    UIAppearance.PrimaryStyleColor = RJColors.Sky;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Sky, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Sky, 15);
                    break;

                case UIStyle.Spinel:
                    UIAppearance.PrimaryStyleColor = RJColors.Spinel;
                    if (UIAppearance.Theme == UITheme.Dark)
                        UIAppearance.StyleColor = ColorEditor.Darken(RJColors.Spinel, 10);
                    else UIAppearance.StyleColor = ColorEditor.Lighten(RJColors.Spinel, 15);
                    break;

                case UIStyle.Supernova:
                    UIAppearance.StyleColor = RJColors.Lisianthus;
                    UIAppearance.PrimaryStyleColor = RJColors.FantasyColorScheme1;
                    break;
            }

            //Set the border color of the form
            if (UIAppearanceSettings.Default.ColorFormBorder == true)
                UIAppearance.FormBorderColor = UIAppearance.PrimaryStyleColor;
            else UIAppearance.FormBorderColor = RJColors.DefaultFormBorderColor;

        }
    }
}
