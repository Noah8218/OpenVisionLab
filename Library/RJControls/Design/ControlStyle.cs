using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.RJControls
{
    public enum ControlStyle
    {
        Glass, //The background color of the control is transparent and with a colored border, in this style you can change the border size.
        Solid, //The background color of the control is a solid color without border, 
                 //this style allows you to apply rounded corners to the control, 
                //for this it uses the RoundedCorner class of the utilities.
    }
}
