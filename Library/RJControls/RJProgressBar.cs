using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using RJCodeUI_M1.Settings;

namespace RJCodeUI_M1.RJControls
{
    public class RJProgressBar : ProgressBar
    {
        // Tutorial guide: https://youtu.be/Sj_b3yOUQDk

        #region -> Fields
        // -> Appearance
        private Color channelColor = Color.LightSteelBlue; // Gets or sets the color of the channel or tracker.
        private Color sliderColor = Color.RoyalBlue; // Gets or sets the color of the slider or indicator.
        private Color foreBackColor = Color.RoyalBlue; // Gets or sets the background color of the value text.
        private int channelHeight = 6; // Gets or sets the height of the channel.
        private int sliderHeight = 6; // Gets or sets the height of the slider.
        private TextPosition showValue = TextPosition.Right; // Gets or sets the position of the value text.
        private string symbolBefore = ""; // Gets or sets a symbol or text before the value text.
        private string symbolAfter = ""; // Gets or sets a symbol or text after the value text.
        private bool showMaximun = false; // Gets or sets if the maximum value is shown.

        // -> Others
        private bool paintedBack = false; // Get or sets if background color has been painted.
        private bool stopPainting = false; // Gets or sets if the paint should stop painting.
        private bool customizable = true; // Gets or sets if the colors are customizable, or set by the appearance settings.
        #endregion

        #region -> Constructor
        public RJProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.ForeColor = Color.White;
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        public bool Customizable
        {
            get { return customizable; }
            set
            {
                customizable = value;
            }
        }

        [Category("RJ Code Advance")]
        public Color ChannelColor
        {
            get { return channelColor; }
            set
            {
                channelColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color SliderColor
        {
            get { return sliderColor; }
            set
            {
                sliderColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color ForeBackColor
        {
            get { return foreBackColor; }
            set
            {
                foreBackColor = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public int ChannelHeight
        {
            get { return channelHeight; }
            set
            {
                channelHeight = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public int SliderHeight
        {
            get { return sliderHeight; }
            set
            {
                sliderHeight = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public TextPosition ShowValue
        {
            get { return showValue; }
            set
            {
                showValue = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public string SymbolBefore
        {
            get { return symbolBefore; }
            set
            {
                symbolBefore = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public string SymbolAfter
        {
            get { return symbolAfter; }
            set
            {
                symbolAfter = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public bool ShowMaximun
        {
            get { return showMaximun; }
            set
            {
                showMaximun = value;
                this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
            }
        }

        [Category("RJ Code Advance")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyAppearanceSettings();
            this.Parent.Paint += new PaintEventHandler(ParentContainer_RePaint);
        }

        private void ParentContainer_RePaint(object sender, PaintEventArgs e)
        {
            // If the container (Form or control) of the control is repainted,
            // For example, when the form is scrolled (Scroll), restart the painting of the control.
            stopPainting = false;
            paintedBack = false;
        }

        // -> Paint the background and channel of the control.
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (stopPainting == false)
            {
                if (paintedBack == false)
                {
                    // Fields
                    Graphics graph = pevent.Graphics;
                    Rectangle rectChannel = new Rectangle (0, 0, this.Width, ChannelHeight);
                    using (var brushChannel = new SolidBrush (channelColor))
                    {
                        if (channelHeight>= sliderHeight)
                            rectChannel.Y = this.Height - channelHeight;
                        else rectChannel.Y = this.Height - ((channelHeight + sliderHeight) / 2);

                        //Painting
                        graph.Clear (this.Parent.BackColor); // Paint the surface / background of the control
                        graph.FillRectangle (brushChannel, rectChannel); // paint the channel of the control

                        // Stop painting the bottom and channel, with this condition the bottom and channel is only painted once, that is to avoid flickering.
                        if (this.DesignMode == false)
                            paintedBack = true;
                    }
                }
                // Restart the painting of the bottom and channel, as long as the value reaches the maximum or minimum value.
                if (this.Value == this.Maximum || this.Value == this.Minimum)
                    paintedBack = false;
            }
        }
        // -> Paint the slider or indicator.
        protected override void OnPaint(PaintEventArgs e)
        {
            if (stopPainting == false)
            {
                // Fields
                Graphics graph = e.Graphics;
                double scaleFactor = (((double)this.Value - this.Minimum) / ((double)this.Maximum - this.Minimum));
                int sliderWidth = (int)(this.Width * scaleFactor);
                Rectangle rectSlider = new Rectangle(0, 0, sliderWidth, sliderHeight);
                using (var brushSlider = new SolidBrush(sliderColor))
                {
                    if (sliderHeight >= channelHeight)
                        rectSlider.Y = this.Height - sliderHeight;
                    else rectSlider.Y = this.Height - ((sliderHeight + channelHeight) / 2);

                    //Painting
                    if (sliderWidth > 1) // Paint the slider
                        graph.FillRectangle(brushSlider, rectSlider);
                    if (showValue != TextPosition.None) // Draw the text of the value.
                        DrawValueText(graph, sliderWidth, rectSlider);
                }
            }
            if (this.Value == this.Maximum) stopPainting = true; // Stop painting if the value reaches the maximum value
            else stopPainting = false; // Otherwise, continue painting.

        }

        #endregion

        #region -> Private methods

        // -> Paint the text of the value.
        private void DrawValueText(Graphics graph, int sliderWidth, Rectangle rectSlider)
        {
            // Fields
            string text = symbolBefore + this.Value.ToString() + symbolAfter;
            if (showMaximun) text = text + "/" + symbolBefore + this.Maximum.ToString() + symbolAfter;
            var textSize = TextRenderer.MeasureText(text, this.Font);
            var rectText = new Rectangle(0, 0, textSize.Width, textSize.Height + 2);
            using (var brushText = new SolidBrush(this.ForeColor))
            using (var brushTextBack = new SolidBrush(foreBackColor))
            using (var textFormat = new StringFormat())
            {
                switch (showValue)
                {
                    case TextPosition.Left:
                        rectText.X = 0;
                        textFormat.Alignment = StringAlignment.Near;
                        break;

                    case TextPosition.Right:
                        rectText.X = this.Width - textSize.Width;
                        textFormat.Alignment = StringAlignment.Far;
                        break;

                    case TextPosition.Center:
                        rectText.X = (this.Width - textSize.Width) / 2;
                        textFormat.Alignment = StringAlignment.Center;
                        break;

                    case TextPosition.Sliding:
                        rectText.X = sliderWidth - textSize.Width;
                        textFormat.Alignment = StringAlignment.Center;
                        // Clean the previous surface of the text.
                        using (var brushClear = new SolidBrush(this.Parent.BackColor))
                        {
                            var rect = rectSlider;
                            rect.Y = rectText.Y;
                            rect.Height = rectText.Height;
                            graph.FillRectangle(brushClear, rect);
                        }
                        break;
                }
                //Painting
                graph.FillRectangle(brushTextBack, rectText); // Paint the background of the text.
                graph.DrawString(text, this.Font, brushText, rectText, textFormat); // Draw the text.
            }
        }

        private void ApplyAppearanceSettings()
        {
            if (customizable == false) // If not customizable, apply appearance color settings.
            {
                foreBackColor = UIAppearance.StyleColor;
                sliderColor = UIAppearance.StyleColor;
                if (UIAppearance.Theme == UITheme.Dark)
                    channelColor = Utils.ColorEditor.Lighten(UIAppearance.ItemBackgroundColor, 5);
                else channelColor = Utils.ColorEditor.Darken(UIAppearance.BackgroundColor, 15);
            }
        }
        #endregion
    }
}
