using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Reflection;

namespace KtemVisionSystem
{
    public partial class FormLayerDisplay : WeifenLuo.WinFormsUI.Docking.DockContent
    {

        public int nIndex = 0;

        public KtemViewer ImageView = new KtemViewer();

        public bool ChangeSize { get; set; } = false;        

        public FormLayerDisplay(Bitmap ImageSource, int nIndex, List<FormLayerDisplay> LayerDisplayList, bool UseCloseButton = true, string strTitle = "TEST")
        {           
            InitializeComponent();

            //this.MinimumSize = new Size(1000, 1000);
            //this.Resize += new EventHandler(ResizeEvent);

            
            this.Activated += FormLayerDisplay_Activated;
            ImageView.LoadImageBox(ibSource);

            ibSource.MouseMove += IbSource_MouseMove;

            if (strTitle != "TEST")
            {
                this.Text = strTitle;
            }
            else
            {
                this.Text = "TEST";
            }

            if (UseCloseButton)
            {
                CloseButton = true;
                CloseButtonVisible = true;
            }
            else
            {
                CloseButton = false;
                CloseButtonVisible = false;
            }

            if (ImageSource != null)
            {
                ibSource.Image = ImageSource;
            }

            this.nIndex = nIndex;
            DisplayList = LayerDisplayList;            
        }

        private void IbSource_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void FormLayerDisplay_Activated(object sender, EventArgs e)
        {
            //if(!ChangeSize)
            //{
            //    DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(0, 0, 800, 800);
            //    this.Refresh();
            //    ChangeSize = true;
            //}            
        }

        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        public List<FormLayerDisplay> DisplayList = new List<FormLayerDisplay>();

        private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisplayList.RemoveAt(GetDisplayIndex(this.Text));
        }

        private int GetDisplayIndex(string strTitle)
        {
            for (int i = 0; i < DisplayList.Count; i++)
            {
                if (DisplayList[i].Text == strTitle) { return i; }
            }

            return 0;
        }

        private void timePixelData_Tick(object sender, EventArgs e)
        {
            try
            {
                lbRGB.Text = string.Format("R,G,B[{0},{1},{2}]", ImageView.RGB.R, ImageView.RGB.G, ImageView.RGB.B);
                lbXY.Text = string.Format("X,Y[{0},{1}]", ImageView.POSITION.X, ImageView.POSITION.Y);
                lbGV.Text = string.Format("GV[{0}]", ImageView.GrayValue);
                lbZOOM.Text = string.Format("ZOOM[{0}%]", ImageView.ib.Zoom.ToString("F1"));
                //lbRGB.Text = string.Format("R,G,B[{0},{1},{2}]", ImageView.RGB.R, ImageView.RGB.G, ImageView.RGB.B);
                //lbXY.Text = string.Format("X,Y[{0},{1}]", ImageView.POSITION.X, ImageView.POSITION.Y);
                //lbGV.Text = string.Format("GV[{0}]", ImageView.GrayValue);
                //lbZOOM.Text = string.Format("ZOOM[{0}%]", ImageView.ibSource.Zoom.ToString("F1"));

                //CLog.Info( "{0}", Global.Device.CAMERAS[0].FPS);
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void FormLayerDisplay_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if(DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
                ImageView.ib.ZoomToFit();
            }
        }
    }
}
