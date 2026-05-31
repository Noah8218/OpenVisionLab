using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;

using MetroFramework;
using MetroFramework.Forms;

namespace KtemVisionSystem
{
    public partial class FormImageView : MetroForm
    {
        Mat ImageSource = new Mat();
        Mat ImageDisplay     = new Mat();

        private Rectangle m_ROI;

        private System.Drawing.Point m_ptStart = new System.Drawing.Point(0, 0);
        private System.Drawing.Point m_ptEnd = new System.Drawing.Point(0, 0);

        bool m_bIsMouseJob = false;
        bool m_bIsToUpDown = true;

        public float m_fImageScale  { get; set; } = 1;
        public float m_fImgW        { get; set; } = 0;
        public float m_fImgH        { get; set; } = 0;

        public float m_dPenX        { get; set; } = 0;
        public float m_dPenY        { get; set; } = 0;

        public float m_fMinX { get; private set; } = 1;
        public float m_fMinY { get; private set; } = 1;

        public float m_fMaxX { get; private set; } = 0;
        public float m_fMaxY { get; private set; } = 0;

        public FormImageView(Mat image)
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(MouseWheelEvent);

            ibSource.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap( image );

            m_fMaxX = m_fImgW = ImageSource.Width;
            m_fMaxY = m_fImgH = ImageSource.Height;
        }

        public FormImageView(Bitmap image)
        {
            InitializeComponent();

            this.MouseWheel += new MouseEventHandler(MouseWheelEvent);

            ibSource.Image = image;

            m_fMaxX = m_fImgW = ImageSource.Width;
            m_fMaxY = m_fImgH = ImageSource.Height;
        }

        /// <summary>
        /// EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            if ( (e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;
                
                ZoomInImage();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImage();
            }
        }

        private void pblSource_DragDrop(object sender, DragEventArgs e)
        {
            //m_OriginalImg = 

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        }

        private void pblSource_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;

        }

        #region Display
        private void ZoomInImage()
        {
            ibSource.ZoomIn();
        }

        private void ZoomOutImage()
        {
            ibSource.ZoomOut( );
        }

        private void ZoomFitImage()
        {
            ibSource.ZoomToFit();
        }

        private void FormImageView_FormClosing(object sender, FormClosingEventArgs e)
        {
            ImageSource.Dispose();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOutImage();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomInImage();
        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            ZoomFitImage();
        }
        #endregion

        private void metroLabel14_Click(object sender, EventArgs e)
        {

        }

        private void metroLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
