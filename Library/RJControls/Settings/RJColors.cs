using System;
using System.Collections.Generic;
using System.Drawing;

namespace RJCodeUI_M1.Settings
{
    public static class RJColors
    {
        // Fields
        public static int colorIndex;

        //-> RJColors
        public static readonly Color SideMenuColor = Color.FromArgb(49, 42, 81);
        public static readonly Color DefaultFormBorderColor = Color.FromArgb(111, 106, 143);

        // Style Colors 
        public static readonly Color Axolotl = Color.FromArgb(248, 112, 171);
        public static readonly Color FireOpal = Color.FromArgb(245, 124, 37);
        public static readonly Color Forest = Color.FromArgb(20, 138, 75);
        public static readonly Color Lisianthus = Color.FromArgb(123, 104, 238);
        public static readonly Color Neptune = Color.FromArgb(83, 97, 212);
        public static readonly Color Navi = Color.FromArgb(69, 83, 100);
        public static readonly Color Petunia = Color.FromArgb(171, 48, 243);
        public static readonly Color Ruby = Color.FromArgb(224, 40, 67);
        public static readonly Color Sky = Color.FromArgb(90, 146, 246);
        public static readonly Color Spinel = Color.FromArgb(251, 111, 126);
        // Fantasy Color Scheme   
        public static readonly Color FantasyColorScheme1 = Color.FromArgb(104, 85, 230);
        public static readonly Color FantasyColorScheme2 = Color.FromArgb(47, 168, 210);
        public static readonly Color FantasyColorScheme3 = Color.FromArgb(70, 132, 235);
        public static readonly Color FantasyColorScheme4 = Color.FromArgb(238, 96, 112);
        public static readonly Color FantasyColorScheme5 = Color.FromArgb(73, 84, 228);
        public static readonly Color FantasyColorScheme6 = Color.FromArgb(230, 52, 114);
        public static readonly Color FantasyColorScheme7 = Color.FromArgb(208, 101, 243);
        public static readonly Color FantasyColorScheme8 = Color.FromArgb(238, 134, 139);
        public static readonly Color FantasyColorScheme9 = Color.FromArgb(250, 173, 112);
        public static readonly Color FantasyColorScheme10 = Color.FromArgb(133, 108, 238);
        public static readonly Color FantasyColorScheme11 = Color.FromArgb(224, 195, 252);
        public static readonly Color FantasyColorScheme12 = Color.FromArgb(125, 179, 250);       

        // Colors for DARK theme 
        public static readonly Color DarkBackground = Color.FromArgb(58, 52, 95);
        public static readonly Color DarkItemBackground = Color.FromArgb(66, 60, 109);
        public static readonly Color DarkActiveBackground = Color.FromArgb(77, 70, 130);
        public static readonly Color DarkTextColor = Color.FromArgb(134, 131, 177);

        // Colors for LIGHT theme
        public static readonly Color LightBackground = Color.FromArgb(240, 245, 249);
        public static readonly Color LightItemBackground = Color.FromArgb(250, 252, 253);
        public static readonly Color LightActiveBackground = Color.FromArgb(231, 238, 246);
        public static readonly Color LightTextColor = Color.FromArgb(132, 129, 132);

        // Action Colors
        public static readonly Color Delete = Color.FromArgb(234, 79, 82);
        public static readonly Color Confirm = Color.FromArgb(55, 159, 113);
        public static readonly Color Cancel = Color.FromArgb(104, 110, 134);
        public static readonly Color Deactive = Color.FromArgb(111, 127, 148);
        public static readonly Color DeactiveDark = Color.FromArgb(53, 64, 81);


        //Supernova color list
        private static readonly List<Color> SupernovaColors = new List<Color>() {            
         FantasyColorScheme1 ,
         FantasyColorScheme2 ,
         FantasyColorScheme3 ,
         FantasyColorScheme4 ,
         FantasyColorScheme5 ,
         FantasyColorScheme6 ,
         FantasyColorScheme7 ,
         FantasyColorScheme8 ,
         FantasyColorScheme9 ,
         FantasyColorScheme10
        };

        //Get a supernova color
        public static Color GetSupernovaColor()
        {
            Color color = SupernovaColors[colorIndex];
            colorIndex++;
            if (colorIndex >= SupernovaColors.Count)
                colorIndex = 0;
            return color;
        }
    }
}
