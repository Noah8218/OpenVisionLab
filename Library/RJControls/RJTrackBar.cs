using RJCodeUI_M1.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    public class RJTrackBar : TrackBar
    {
        /// <summary>
        /// This class inherits from the <see cref = "TrackBar" /> class.
        /// The OnPaint event is overridden to draw the channel and track bar slider.
        /// Based on Hans Passant's example: https://stackoverflow.com/a/2714457/12778930
        /// </summary>

        #region -> Fields

        private bool customizable; // Gets or sets if the control color is customizable or the colors are set by the appearance settings
        private bool showValue; // Gets or sets if the current value of the tracker bar is shown
        private Color channelColor; // Gets or sets the color of the track bar channel
        private Color sliderColor; // Gets or sets the color of the track bar slider
        private Color textColor; // Gets or sets the color of the tracker bar text.

        private int trackerValue;
        private Font textFont;

        // Window messages
        private const int WM_USER = 0x0400; // Used to define private messages for use by private window classes, generally of the form WM_USER + X, like the following.
        // TrackBar messages
        private const int TBM_GETCHANNELRECT = WM_USER + 26; // This message retrieves the size and position of the bounding rectangle of the slider on a track bar.
        private const int TBM_GETTHUMBRECT = WM_USER + 25; // This message retrieves the size and position of the bounding rectangle of the slider on a track bar.
        // RECT structure
        private struct RECT { public int Left, Top, Right, Bottom; } // The RECT structure defines the coordinates of the upper left and lower right corners of a rectangle.

        #endregion

        #region -> Constructor

        public RJTrackBar()
        {
            SetStyle(ControlStyles.UserPaint, true);//Set the UserPaint control style to true so that the control paints itself instead of the operating system.
            //Initialize objects
            textFont = new Font("Verdana", 8F, FontStyle.Regular);
        }
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        public bool Customizable
        {// Gets or sets whether the control color is customizable or the colors are set by the appearance settings
            get { return customizable; }
            set { customizable = value; }
        }

        [Category("RJ Code Advance")]
        public Color ChannelColor
        {// Gets or sets the channel color of the track bar
            get { return channelColor; }
            set
            {
                channelColor = value;
                if (this.DesignMode) // Preview changes in design mode
                    this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public Color SliderColor
        {// Gets or sets the color of the track bar slider
            get { return sliderColor; }
            set
            {
                sliderColor = value;
                if (this.DesignMode) // Preview changes in design mode
                    this.Update();
            }
        }

        [Category("RJ Code Advance")]
        public Color TextColor
        {// Gets or sets the text color of the value label.
            get { return textColor; }
            set
            {
                textColor = value;
                if (this.DesignMode) // Preview changes in design mode
                    this.Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        public bool ShowValue
        {// Gets or sets whether the value label is displayed.
            get { return showValue; }
            set
            {
                showValue = value;
                if (this.DesignMode) // Preview changes in design mode
                    this.Invalidate();
            }
        }
        #endregion

        #region -> External methods
        ///<summary>
        /// The <SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam)/> method is an external function of the user.32 
        /// library, this method Sends the specified message to a window or windows.
        /// The SendMessage function calls the window procedure for the specified window and does not return until the window 
        /// procedure has processed the message.
        /// <param name="hWnd">A handle to the window whose window procedure will receive the message</param>
        /// <param name="wMsg">The message to be sent</param>
        /// <param name="wParam">Additional message-specific information: Must be zero </param>
        /// <param name="lParam">Additional message-specific information: ref RECT> Pointer to a RECT structure.
        /// The message fills this structure with the bounding rectangle of the trackbar's slider in client coordinates
        /// of the trackbar's window </param>
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessageRect(IntPtr hWnd, int msg, IntPtr wParam, ref RECT lParam);
        #endregion

        #region -> Private methods

        private Rectangle GetSlider()
        {// Get the size and position of the slider
            RECT rect = new RECT();
            SendMessageRect(this.Handle, TBM_GETTHUMBRECT, IntPtr.Zero, ref rect);
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

        }
        private Rectangle GetChannel()
        {// Get the size and position of the channel

            RECT rect = new RECT();
            SendMessageRect(this.Handle, TBM_GETCHANNELRECT, IntPtr.Zero, ref rect);
            if (this.Orientation == Orientation.Horizontal) // Horizontal Orientation
                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top + 3);
            else // Vertical Orientation
                return new Rectangle(rect.Left, rect.Top, rect.Bottom - rect.Top + 3, rect.Right - rect.Left);

        }
        private void ApplyAppearanceSettings()
        {
            // Load appearance settings
            if (customizable == false)
            {
                if (UIAppearance.Theme == UITheme.Light)
                {
                    channelColor = Color.LightGray;
                    sliderColor = UIAppearance.StyleColor;
                    textColor = Utils.ColorEditor.Lighten(UIAppearance.TextColor, 25);
                }
                else
                {
                    channelColor = RJColors.DefaultFormBorderColor;
                    sliderColor = UIAppearance.StyleColor;
                    textColor = Utils.ColorEditor.Darken(UIAppearance.TextColor, 7);
                }
            }
        }
        #endregion

        #region -> Overridden methods

        protected override void OnHandleCreated(EventArgs e)
        {// This event is equivalent to the load event
            base.OnHandleCreated(e);

            trackerValue = this.Value; // Set current value
            ApplyAppearanceSettings(); // Load appearance settings
        }
        protected override void OnValueChanged(EventArgs e)
        {// This event occurs when the value of the track bar changes
            base.OnValueChanged(e);

            trackerValue = this.Value; // Set current value
            this.Invalidate(false); // Redraw control (invoke the Paint event)
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var channel = GetChannel(); // Get the size and position of the channel
            var slider = GetSlider(); // Get the size and position of the slider
            using (Graphics graph = e.Graphics)
            using (SolidBrush brushChannel = new SolidBrush(channelColor))
            using (SolidBrush brushSlider = new SolidBrush(sliderColor))
            using (SolidBrush brushText = new SolidBrush(textColor))
            {
                graph.FillRectangle(brushChannel, channel); // Draw the channel of the tracking bar with the specified color and the size and position obtained.
                graph.FillRectangle(brushSlider, slider); // Draw the slider of the track bar with the specified color and the size and position obtained.

                if (showValue) // Draw the text with the current value of the track bar
                {
                    if (this.Orientation == Orientation.Horizontal) // Horizontal Orientation
                    {
                        if (trackerValue >= 100)
                            graph.DrawString(trackerValue.ToString(), textFont, brushText, slider.Left - 18, 30);
                        else
                            graph.DrawString(trackerValue.ToString(), textFont, brushText, slider.Left - 12, 30);
                    }
                    else // Vertical Orientation
                    {
                        graph.DrawString(trackerValue.ToString(), textFont, brushText, 21, slider.Top);
                        //this.Value.ToString () will not work in this scenario, therefore the trackerValue field is created.
                    }
                }
            }
            /* Note: If you don't want to draw the text, it is not necessary to call the Invalidate () method in the ValueChanged event */
        }
        #endregion

    }
}

