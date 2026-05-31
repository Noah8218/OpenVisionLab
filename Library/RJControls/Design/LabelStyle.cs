using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.RJControls
{
    public enum LabelStyle
    {//Defines predefined label styles, which can apply a font color and size

        Normal,
        Description,
        Subtitle,
        Title,
        Title2,
        BarCaption, //used to display the caption of the current form in the title bar of the main form
        BarText,//Used to display text in the title bar of the main form (e.g. username)
        Custom //Custom size and font, however text color is set by style
    }
}
