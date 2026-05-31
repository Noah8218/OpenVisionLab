using System;
using System.Drawing;

namespace RJCodeUI_M1.Utils
{
    public static class ColorEditor
    {
        /// <summary>
        /// With this static class you can lighten or darken a specific color
        /// <see cref="https://www.pvladov.com/2012/09/make-color-lighter-or-darker.html">Author: Pavel Vladov</see>/>
        /// </summary>
        /// <param name="color">"color to lighten or darken"</param>
        /// <param name="percentage">"The level to lighten or darken the color, maximum value is 100%"</param>
        /// <returns>Color</returns>
        /// 
        public static Color Lighten(Color color, ushort percentage)
        {//Lighten color

            if (percentage <= 100) //maximum value is 100 %
            {
                float correctionFactor = percentage / (float)100;//Convert percent to decimals

                float red = color.R;//Get the red component
                float green = color.G;//Get the green component
                float blue = color.B;//Get the blue component

                red = (255 - red) * correctionFactor + red;//Set new red component
                green = (255 - green) * correctionFactor + green; //Set new green component
                blue = (255 - blue) * correctionFactor + blue;//Set new blue component

                return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);//Return lightened color
            }
            else
                return color;
        }
        public static Color Darken(Color color, ushort percentage)
        {//darken color

            if (percentage <= 100)
            {
                float correctionFactor = (percentage / (float)100) * -1;//Convert the percentage to negative decimals to produce dark colors
                correctionFactor = 1 + correctionFactor;//Add 1, since sometimes it happens that the color parameter is negative and throws an exception

                float red = color.R;
                float green = color.G;
                float blue = color.B;

                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;

                return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);//Return darkened color 
            }
            else
                return color;
        }
    }
}
