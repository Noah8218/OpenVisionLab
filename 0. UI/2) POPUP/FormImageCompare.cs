using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Cyotek.Windows.Forms;
using Lib.Common;
using System.Windows.Media;

namespace OpenVisionLab
{
    public partial class FormImageCompare : RJCodeUI_M1.RJForms.RJChildForm
    {
        private int m_nWidth = 0;
        public int Width
        {
            get => m_nWidth;
            set => m_nWidth = value;
        }

        private int m_nHeight = 0;
        public int Height
        {
            get => m_nHeight;
            set => m_nHeight = value;
        }
        private float m_fImageScale { get; set; } = 5;

        public CViewer ImageView = new CViewer();
        public CViewer ImageViewCopy = new CViewer();

        private enum CompareMode
        {
            Image1,
            Image2
        }

        private CompareMode SelecteCompareMode = CompareMode.Image1;

        public FormImageCompare()
        {           
            InitializeComponent();

            //ImageView.LoadImageBox2(ibSource);
            //ImageViewCopy.LoadImageBox2(ibSourceCopy);

            ibSource.MouseWheel += new MouseEventHandler(MouseWheelEvent);
            ibSourceCopy.MouseWheel += new MouseEventHandler(MouseWheelEvent);

            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);

            System.Drawing.Color color = System.Drawing.Color.FromArgb(20, 20, 20);

            ibSource.GridColor = color;
            ibSource.GridColorAlternate = color;

            ibSourceCopy.GridColor = color;
            ibSourceCopy.GridColorAlternate = color;

            ibSource.MouseDoubleClick += IbSource_MouseDoubleClick;
            ibSourceCopy.MouseDoubleClick += IbSource_MouseDoubleClick;

            ibSource.ShowPixelGrid = true;
            ibSourceCopy.ShowPixelGrid = true;
        }

        private void IbSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                ibSource.ZoomOut();
            }

            ibSource.ZoomToFit();

            for (int i = 0; i < 20; i++)
            {
                ibSourceCopy.ZoomOut();
            }

            ibSourceCopy.ZoomToFit();
        }

        private System.Drawing.Point UpdateCursorPosition(ImageBox imageBoxEx, System.Drawing.Point location)
        {
            System.Drawing.Point point;

            if (imageBoxEx.IsPointInImage(location))
            {
                return point = location;
            }

            return point = imageBoxEx.PointToImage(location);
        }

        System.Drawing.Point PT = new System.Drawing.Point();
        private void ibSource1_MouseMove(object sender, MouseEventArgs e)
        {
            PT = UpdateCursorPosition(ibSource, e.Location);
            SelecteCompareMode = CompareMode.Image1;

            if (ImageCompare1.Width == 10 || ImageCompare1.Height == 10) return;

            System.Drawing.Point point = UpdateCursorPosition2(ibSource, e.Location);

            if (ImageCompare1.Width == 10 || ImageCompare1.Height == 10) return;

            if (point.X > ImageCompare1.Width || point.Y > ImageCompare1.Height) return;

            try
            {
                System.Drawing.Color color = ImageCompare1.GetPixel(point.X, point.Y);
                int nBright = (color.R + color.G + color.B) / 3;

                lbRGB.Text = string.Format("R,G,B[{0},{1},{2}]", color.R, color.G, color.B);
                lbXY.Text = string.Format("X,Y[{0},{1}]", point.X, point.Y);
                lbGV.Text = string.Format("GV[{0}]", nBright);
            }
            catch
            {
            }

        }

        private void ibSource2_MouseMove(object sender, MouseEventArgs e)
        {
            PT = UpdateCursorPosition(ibSourceCopy, e.Location);
            SelecteCompareMode = CompareMode.Image2;

            if (ImageCompare2.Width == 10 || ImageCompare2.Height == 10) return;

            System.Drawing.Point point = UpdateCursorPosition2(ibSourceCopy, e.Location);

            if (ImageCompare2.Width == 10 || ImageCompare2.Height == 10) return;

            if (point.X > ImageCompare2.Width || point.Y > ImageCompare2.Height) return;

            try
            {
                System.Drawing.Color color = ImageCompare2.GetPixel(point.X, point.Y);
                int nBright = (color.R + color.G + color.B) / 3;

                lbRGB.Text = string.Format("R,G,B[{0},{1},{2}]", color.R, color.G, color.B);
                lbXY.Text = string.Format("X,Y[{0},{1}]", point.X, point.Y);
                lbGV.Text = string.Format("GV[{0}]", nBright);
            }
            catch
            {

            }

        }

        private void ibSource1_Scroll(object sender, ScrollEventArgs e)
        {
            //if (SelecteCompareMode == CompareMode.Image1)
            //{
            //    ibSourceCopy.AutoScrollPosition = new System.Drawing.Point();
            //    ibSourceCopy.ScrollTo(ibSourceCopy.AutoScrollPosition.X, ibSourceCopy.AutoScrollPosition.Y, ibSource.AutoScrollPosition.X, ibSource.AutoScrollPosition.Y);
            //}
            //else
            //{
            //    ibSource.AutoScrollPosition = new System.Drawing.Point();
            //    ibSource.ScrollTo(ibSource.AutoScrollPosition.X, ibSource.AutoScrollPosition.Y, ibSourceCopy.AutoScrollPosition.X, ibSourceCopy.AutoScrollPosition.Y);
            //}

        }

        private void ibSource1_MouseUp(object sender, MouseEventArgs e)
        {
            if (SelecteCompareMode == CompareMode.Image1)
            {
                ibSourceCopy.AutoScrollPosition = new System.Drawing.Point();
                ibSourceCopy.ScrollTo(ibSourceCopy.AutoScrollPosition.X, ibSourceCopy.AutoScrollPosition.Y, ibSource.AutoScrollPosition.X, ibSource.AutoScrollPosition.Y);
            }
            else
            {
                ibSource.AutoScrollPosition = new System.Drawing.Point();
                ibSource.ScrollTo(ibSource.AutoScrollPosition.X, ibSource.AutoScrollPosition.Y, ibSourceCopy.AutoScrollPosition.X, ibSourceCopy.AutoScrollPosition.Y);
                ibSource.Refresh();
            }
        }
        bool bZoom = false;

        private void ZoomInImage(ImageBox ImageBox)
        {
            ImageBox.ZoomIn();
        }

        private void ZoomOutImage(ImageBox ImageBox)
        {
            ImageBox.ZoomOut();
        }

        private object ob = new object();

        private void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            lock(ob)
            {
                ImageBox ImageBox = sender as ImageBox;

                if ((e.Delta / 120) > 0)
                {
                    // up
                    if (m_fImageScale > 1)
                        m_fImageScale--;

                    ZoomInImage(ImageBox);

                    if (SelecteCompareMode == CompareMode.Image1)
                    {                        
                        ZoomInImage(ibSourceCopy);
                        ZoomInImage(ibSourceCopy);
                    }
                    else
                    {
                        ZoomInImage(ibSource);
                        ZoomInImage(ibSource);
                    }
                }
                else
                {
                    // down
                    m_fImageScale++;

                    ZoomOutImage(ImageBox);

                    if (SelecteCompareMode == CompareMode.Image1)
                    {
                        ZoomOutImage(ibSourceCopy);
                        ZoomOutImage(ibSourceCopy);
                    }
                    else
                    {
                        ZoomOutImage(ibSource);
                        ZoomOutImage(ibSource);
                    }
                }
            }           
        }

        private void ibSource1_MouseDown(object sender, MouseEventArgs e)
        {
            SelecteCompareMode = CompareMode.Image1;
        }

        private void ibSource2_MouseDown(object sender, MouseEventArgs e)
        {
            SelecteCompareMode = CompareMode.Image2;
        }

        private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private System.Drawing.Point UpdateCursorPosition2(ImageBox imageBoxEx, System.Drawing.Point location)
        {
            System.Drawing.Point point;

            return point = imageBoxEx.PointToImage(location);
        }


        private void timePixelData_Tick(object sender, EventArgs e)
        {
            if (SelecteCompareMode == CompareMode.Image1)
            {
                var g = ibSourceCopy.CreateGraphics();

                float fRectSize = 10;

                float fTLX = Math.Abs(fRectSize / 2 - PT.X);
                float fTLY = Math.Abs(fRectSize / 2 - PT.Y);

                float fWidth = Math.Abs(fRectSize / 2 + PT.X);
                float fHeight = Math.Abs(fRectSize / 2 + PT.Y);

                g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Yellow, 30), fTLX, fTLY, fRectSize, fRectSize);
                ibSourceCopy.Invalidate(true);

                //g.Dispose();
            }
            else
            {
                var g = ibSource.CreateGraphics();

                float fRectSize = 10;

                float fTLX = Math.Abs(fRectSize / 2 - PT.X);
                float fTLY = Math.Abs(fRectSize / 2 - PT.Y);

                float fWidth = Math.Abs(fRectSize / 2 + PT.X);
                float fHeight = Math.Abs(fRectSize / 2 + PT.Y);

                g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Yellow, 30), fTLX, fTLY, fRectSize, fRectSize);
                ibSource.Invalidate(true);
            }
        }
        private Bitmap ImageCompare1 = new Bitmap(10, 10);
        private Bitmap ImageCompare2 = new Bitmap(10, 10);
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string[] strImagePath = CUtil.LoadImagesFilePath();

                if (strImagePath == null) { return; }

                ibSource.Image = new Bitmap(strImagePath[0]);
                ibSourceCopy.Image = new Bitmap(strImagePath[1]);

                ImageCompare1 = new Bitmap(strImagePath[0]);
                ImageCompare2 = new Bitmap(strImagePath[1]);
                ibSource.ZoomToFit();
                ibSourceCopy.ZoomToFit();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
    }
}
