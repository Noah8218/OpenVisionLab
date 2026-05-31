using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.Utils
{
    public static class RoundedControl
    {
        /// <summary>
        /// This static class is in charge of making controls with rounded corners.
        /// It has 3 ways to do it:
        /// -Apply the radius border only to the Region of the control.
        /// -Apply the control's Region radius border + Contour Smoothing.
        /// -Apply the Control's Region Radius Border + Contour Smoothing + Border Drawing.
        /// </summary>
        ///

        #region -> Public methods
        public static GraphicsPath GetRoundedGPath(Rectangle rect, float radius)
        {//This method is responsible for creating the rounded path of a specified rectangle and radius.
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ROUNDED REGION
        public static void RegionOnly(Control control, int radius)
        {// This method takes care of applying a rounded region to a specified control and radius.
            // The rounded corners are not smooth.

            if (radius >= 1) // Apply border radius (ROUNDED CONTROL)
            {
                using (GraphicsPath roundPath = GetRoundedGPath(control.ClientRectangle, radius)) // Get shape path with rounded corners.
                {
                    control.Region = new Region(roundPath); // Apply rounded path as new region.
                }
                control.Resize += (s, e) => // If the control resizes, update the rounded region.
               {
                   using (GraphicsPath newRoundPath = GetRoundedGPath(control.ClientRectangle, radius))
                   {
                       control.Region = new Region(newRoundPath);
                   }
               };
            }
            else // Don't apply border radius (RECTANGULAR CONTROL)
            {
                // Reset the region
                control.Region = new Region(control.ClientRectangle);
                control.Resize += (s, e) =>
                {
                    control.Region = new Region(control.ClientRectangle);
                };
            }

        }

        // ROUNDED REGION + CONTOUR SMOOTHING
        public static void RegionAndSmoothed(Control control, int radius, Graphics graph)
        {// This method is responsible for applying a rounded region and smoothing the contour of the control
            // to get high quality rounded corners with smooth borders (Not pixelated).
            // Use this method from the Paint event of the control.

            if (radius > 1) // Apply border radius (ROUNDED CONTROL)
            {
                using (GraphicsPath roundPath = GetRoundedGPath(control.ClientRectangle, radius)) // Get shape path with rounded corners.
                using (Pen penSmoothing = new Pen(control.Parent.BackColor, 1)) // Pen for contour smoothing, the color must be equal to the background color of its container.
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    control.Region = new Region(roundPath); // Apply rounded path as new region.
                    graph.DrawPath(penSmoothing, roundPath); // Draw the rounded path to smooth the outline of the control.
                }
            }
            else // Don't apply border radius (RECTANGULAR CONTROL)
            {
                control.Region = new Region(control.ClientRectangle); // Reset the region.
            }
        }

        // ROUNDED REGION + CONTOUR SMOOTHING + border DRAWING
        public static void RegionAndBorder(Control control, int radius, Graphics graph, Color borderColor, float borderSize)
        {// This method is responsible for applying a rounded region and smoothing the contour of the control
            // to get high quality rounded corners with smooth borders (Not pixelated), in addition to drawing the border of the control.
            // Use this method from the Paint event of the control.

            if (radius> 1) // Apply border radius (ROUNDED CONTROL)
            {
                using (GraphicsPath roundPath = GetRoundedGPath (control.ClientRectangle, radius)) // Get shape path with rounded corners.
                using (Pen penSmoothing = new Pen (control.Parent.BackColor, borderSize + 1)) // Pen for contour smoothing, the color must be equal to the background color of its container.
                using (Pen penBorder = new Pen (borderColor, borderSize)) // Pen to draw the border of the control
                using (Matrix transform = new Matrix ()) // Matrix to scale the graph object and draw the border.
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    control.Region = new Region (roundPath); // Apply rounded path as new region.
                    graph.DrawPath (penSmoothing, roundPath); // Draw the rounded path to smooth the outline of the control.

                    // Draw the rounded border of the control.
                    // For this purpose it is necessary to scale the dimensions of the path within the previous smoothing drawing.
                    if (borderSize >= 1)
                    {
                        Rectangle rect = control.ClientRectangle;
                        float scaleX = 1.0F - ((borderSize + 1) / rect.Width);
                        float scaleY = 1.0F - ((borderSize + 1) / rect.Height);

                        transform.Scale (scaleX, scaleY);
                        transform.Translate (borderSize / 1.6F, borderSize / 1.6F);

                        graph.Transform = transform; // Apply the scaling to the graphics object.
                        graph.DrawPath (penBorder, roundPath); // Draw the border of the control.
                    }
                }
            }
            else // Don't apply border radius (RECTANGULAR CONTROL)
            {
                control.Region = new Region (control.ClientRectangle); // Reset the region.
                if (borderSize >= 1) // Draw rectangular border
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        graph.DrawRectangle(penBorder, 0, 0, control.Width - 0.5F, control.Height - 0.5F);
                    }
                }
            }
        }
        #endregion

    }
}
