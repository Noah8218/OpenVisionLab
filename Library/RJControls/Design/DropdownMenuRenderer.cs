using FontAwesome.Sharp;
using RJCodeUI_M1.Settings;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    class DropdownMenuRenderer : ToolStripProfessionalRenderer
    {
        /// <summary>
        /// this class inherits the<see cref="ToolStripProfessionalRenderer"/> class that provides
        /// the painting functionality for <see cref="ToolStrip"/> objects, applying a custom palette and a 
        /// streamlined style based on a replaceable color table called <see cref="ProfessionalColorTable"/>.
        /// Remember that the ToolStripProfessionalRenderer class calls an instance of the<see cref="ProfessionalColorTable"/>  class,
        /// In this case the <see cref="DropdownMenuColors"/> class the inherits ProfessionalColorTable class.
        /// 
        /// The <see cref="bool menuButton"/> parameter simply sets if the dropdown menu is displayed from the menu button of the main form
        /// </summary>
        /// 

        //Field Definition
        private Bitmap dropdownItemIcon;//Sets the icon of a dropdown item

        //Constructor 
        public DropdownMenuRenderer(bool menuButton)
            : base(new DropdownMenuColors(menuButton))///Send menubutton value and Send an instance of the custom DropdownMenuColors class(<"ProfessionalColorTable"/>)
                                                      ///to the <ToolStripProfessionalRenderer/> base class.
        {
           dropdownItemIcon = IconChar.AngleRight.ToBitmap(UIAppearance.StyleColor, 25);//Set Icon 
        }

        //Override the RenderArrow event to draw a custom icon for a dropdown item
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)///Occurs when an arrow on a<see cref="ToolStripItem"/>  is rendered.
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;//Sets SmoothingMode

            Point point = new Point(e.ArrowRectangle.Location.X - 10, e.Item.Height / 2 - 7);//Set the location of the icon to draw
            var rectangle = new Rectangle(point, new Size(25, 25)); // Create ractangle
                     
            e.Graphics.DrawImage(dropdownItemIcon, rectangle);//Draw icon in dropdown item
        }

    }


}

