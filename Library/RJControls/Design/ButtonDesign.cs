using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.RJControls
{
  public  enum ButtonDesign
    {
        Normal,//The button has a common and flat design, generally the appearance color is the same color as the application style color set by the appearance settings.
        IconButton,//The button has an icon and flat design, generally the appearance color is the same color as the application style color set by the appearance settings.
        Metro,//The button has a similar design to the Windows 8 start menu buttons, generally the appearance color is the same color as the application style color set by the appearance settings.
        Confirm,//The button has a flat and icon design, the appearance color is green, set in the RJColors color list.
        Cancel,//The button has a flat and icon design, the appearance color is dark gray, set in the RJColors color list.
        Delete,//The button has a flat and icon design, the appearance color is red, set in the RJColors color list.
        Custom//The button has an icon and flat or normal design, according to its previous design.
                //With this option you can customize both the design and colors of the button
                //(In this mode the application's appearance settings are not applied).
    }
}
