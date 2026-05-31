using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RJCodeUI_M1.Settings;

namespace RJCodeUI_M1.RJControls
{
    public class RJPictureBox : PictureBox
    {
        #region -> Fields
        private Color borderColor = Color.RoyalBlue; // Gets or sets the border color.
        private int borderSize = 0; // Gets or sets the border size.
        private int borderRadius = 0; // Gets or sets the border radius size.
        private bool customizable = true; // Gets or sets if the border color is customizable, otherwise it takes the color from the theme settings.
        #endregion

        #region -> Properties
        [Category("RJ Code Advance")]
        public bool Customizable
        {
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; this.Invalidate(); }
        }

        [Category("RJ Code Advance")]
        public int BorderSize
        {
            get { return borderSize; }
            set { borderSize = value; this.Invalidate(); }
        }

        [Category("RJ Code Advance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; this.Invalidate(); }
        }
        #endregion

        #region -> Private methods
        private void ApplyAppearanceSettings()
        {
            if (customizable == false)
            {
                borderColor = UIAppearance.StyleColor;
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAppearanceSettings();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            this.BackColor = this.Parent.BackColor;
            Utils.RoundedControl.RegionAndBorder(this, borderRadius, pe.Graphics, borderColor, borderSize);
        }
        #endregion
    }
}
