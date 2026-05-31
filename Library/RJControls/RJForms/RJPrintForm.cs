using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJForms
{
    public partial class RJPrintForm : RJChildForm
    {
		/// <summary>
        /// This class inherits from the <see cref="RJChildForm"/>  class
        /// </summary>
        /// 
		
        #region Fields Definition

        private Image screenshot;//screenshot source
        private Size sizeA4;//A4 size
        private Image imgDocument;//Document in image format (this is what is printed)

        #endregion

        #region Constructor

        public RJPrintForm(Image _screenshot, string docTitle)
        {
            InitializeComponent();//The designer was used to make this form.

            this.DisableFormOptions = true;//Disable form options
            toolTip1.SetToolTip(btnPrint, "Print Document");
            toolTip1.SetToolTip(btnPortrait, "Portrait Orientation");
            toolTip1.SetToolTip(btnLandscape, "Landscape Orientation");
            toolTip1.SetToolTip(btnCancel, "Cancel Print");

            sizeA4 = new Size(794, 1123);//96 DPI A4 paper size in pixels           
            lblDate.Text = DateTime.Now.ToLongDateString();//Get current date
            lblTitle.Text = docTitle;//Set document title
            pbContent.Image = _screenshot;//set  screenshot to picture box content
            screenshot = _screenshot;//Set screenshot field

            if (Settings.UIAppearance.Theme == Settings.UITheme.Dark) //Set print menu backcolor 
                this.pnlPrintMenu.BackColor = Settings.RJColors.DarkActiveBackground;
            else
                this.pnlPrintMenu.BackColor = Settings.RJColors.LightItemBackground;

            PortraitOrientation();//Start the preview in portrait mode
        }
        #endregion

        #region Methods Definition

        private void PortraitOrientation()
        {//print layout in portrait mode

            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;

            printDoc.DefaultPageSettings.Landscape = false;//Deactivate LandScape mode (activate portrait orientation).

            this.Size = new Size(960, Screen.FromHandle(this.Handle).WorkingArea.Height);//set as size 960 and the entire height of the desktop area.
            this.CenterToScreen(); //Center the form on the current screeen.

            pnlDocument.Size = sizeA4;//Set the size of the document panel.
            pnlDocument.Location = new Point(70, 50);//Set the location of the document panel.
            pbContent.Size = new Size(pnlHeader.Width, 1000);//Set the size of the content box

            //Adjust the height of the picture box equal to the height of the scaled image within the content box
            double widhtFactor = (double)screenshot.Width / pbContent.Width;//Get widht factor.
            double heightFactor = (double)screenshot.Height / pbContent.Height;//Get height factor
            double resizeFactor = Math.Max(widhtFactor, heightFactor);//Returns the largest number that indicates what is the resizing factor.

            pbContent.Height = (int)(screenshot.Height / resizeFactor);//Set the NEW SIZE of the content box (this way the image will be displayed at the top and center).
        }
        private void LandscapeOrientation()
        {//print layout in LandScape mode

            if (this.WindowState == FormWindowState.Normal)//Maximize window.
                this.MaximizeWindow();

            printDoc.DefaultPageSettings.Landscape = true;//Set LandScape Mode.

            pnlDocument.Size = new Size(sizeA4.Height - 50, sizeA4.Width);//flip a4 size: Widht=height-50(we subtract 50, somehow not the same size) and Height=Width).
            int xCenteredLocation = (this.Width - pnlDocument.Width) / 2;//Get X coordinate to center document panel.
            pnlDocument.Location = new Point(xCenteredLocation, 50);//Set documento panel location.
            pbContent.Size = new Size(pnlHeader.Width, 680);// Set the size of the picture box(Content).

            //Adjust the height of the picture box equal to the height of the scaled image within the content box
            double widhtFactor = (double)screenshot.Width / pbContent.Width;
            double heightfactor = (double)screenshot.Height / pbContent.Height;
            double resizeFactor = Math.Max(widhtFactor, heightfactor);

            pbContent.Height = (int)(screenshot.Height / resizeFactor);//Set the NEW SIZE of the content box(this way the image will be displayed at the top and center)
        }
        private void PrintDocument()
        {
            var bmpScreenshot = new Bitmap(pnlDocument.Width, pnlDocument.Height);//Set the bitmap object to the size of the desktop panel
            pnlDocument.DrawToBitmap(bmpScreenshot, new Rectangle(0, 0, bmpScreenshot.Width, bmpScreenshot.Height));//Draw the document panel on the bitmap
            imgDocument = (Image)bmpScreenshot;//Set the screenshot to the document to print
            try
            {
                printDoc.Print();//Start document printing
                //This method calls the PrintPage event ( private void printDoc_PrintPage(object sender, PrintPageEventArgs e))
            }
            catch (Exception ex)
            {
                RJMessageBox.Show("There was a problem trying to print, please try again.\nDetails\n" + ex);
            }
        }
        #endregion

        #region Event Methods Definition 
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(imgDocument, 0, 0);//Draw imgDocumento in printDocument and print
        }
        private void btnLandscape_Click(object sender, EventArgs e)
        {
            LandscapeOrientation();
        }
        private void btnPortrait_Click(object sender, EventArgs e)
        {
            PortraitOrientation();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument();
        }
        #endregion

    }
}
