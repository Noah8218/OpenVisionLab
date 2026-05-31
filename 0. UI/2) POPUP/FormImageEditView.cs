using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using OpenCvSharp;
using Cyotek.Windows.Forms;
using RJCodeUI_M1.RJForms;
using System.Drawing.Imaging;
using OpenVisionLab._1._Core;
using Vila.Win32;
using static OpenVisionLab.DrawObject.CEnum;
using static OpenVisionLab._2._Common.CParameterManager;
using OpenVisionLab.DrawObject;
using Lib.Common;

namespace OpenVisionLab
{
    public partial class FormImageEditView : RJChildForm
    {
        private CPropertyImageView PropertyImageView = new CPropertyImageView("IMAGE_VIEW");

        private Mat ImageSource = new Mat();        
        public Mat ImageProcess = new Mat();
        private Bitmap ImageGrey = new Bitmap(10, 10);        
        public Rect SelectedRegion = new Rect();
        public List<Rect> SelectedRegions = new List<Rect>();
        private CViewer KtemViewer = new CViewer();
        private CViewer Train = new CViewer();

        private string Mode = "";

        public FormImageEditView(Bitmap image, string strMode = "Drag")
        {
            InitializeComponent();

            try
            {
                KtemViewer.LoadImageBox(ibSource);
                Train.LoadImageBox(ibTrainImage);

                KtemViewer.SetModeDrag();
                Mode = strMode;

                ImageGrey = (Bitmap)image;
                ImageSource = Lib.Common.CImageConverter.ToMat(image);
                ibSource.Image = image;
                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;

                propertygrid_Parameter.SelectedObject = PropertyImageView;
                ibSource.ZoomToFit();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                this.Close();
            }
        }

        public FormImageEditView(Bitmap image, Rectangle ROI,  string strMode = "")
        {
            InitializeComponent();

            try
            {                                
                KtemViewer.LoadImageBox(ibSource);
                Train.LoadImageBox(ibTrainImage);

                switch(strMode.ToUpper())
                {
                    case "ROI":
                        KtemViewer.Roi = ROI;                        
                        KtemViewer.SetModeRoi();
                        break;
                    case "TRAIN":
                        KtemViewer.Roi = ROI;
                        KtemViewer.SetModeTrainRoi();
                        break;
                    case "MULTI_ROI":
                        KtemViewer.SetModeMultiRoi();
                        break;
                }
                Mode = strMode;

                ImageGrey = ( Bitmap ) image;
                ImageSource = Lib.Common.CImageConverter.ToMat(image);
                ibSource.Image = image;
                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;
                

                propertygrid_Parameter.SelectedObject = PropertyImageView;

                Rectangle rt = ibSource.RectangleToScreen(ROI);
                ibSource.ZoomToRegion(rt.X, rt.Y, rt.Width, rt.Height);
                ibSource.ScrollTo(ROI.X - (ROI.Width / 2), ROI.Y - (ROI.Height / 2), 1, 1);          
            }
            catch ( Exception Desc )
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Execption ==> {Desc.Message}");
                this.Close( );
            }            
        }

        public FormImageEditView(Bitmap image, List<Rect> ROI, string strMode = "")
        {
            InitializeComponent();

            try
            {
                KtemViewer.LoadImageBox(ibSource);
                KtemViewer.SetModeMultiRoi();

                for(int i = 0; i < ROI.Count; i++)
                {                    
                    CRectangleObject cRectangleOb = new CRectangleObject();
                    cRectangleOb.Roi = new Rectangle(ROI[i].X, ROI[i].Y, ROI[i].Width, ROI[i].Height);
                    KtemViewer._RoisOb.Add(cRectangleOb);
                }

                Mode = strMode;
                ImageGrey = (Bitmap)image.Clone();

                ibSource.Image = image;
                this.KeyPreview = true;
                this.TopLevel = true;
                this.TopMost = true;
                ImageSource = Lib.Common.CImageConverter.ToMat(image);

                propertygrid_Parameter.SelectedObject = PropertyImageView;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name} Exception ==> {Desc.Message}");
                this.Close();
            }
        }


        #region Display
        private void ZoomInImage() { ibSource.ZoomIn(); }
        private void ZoomOutImage() { ibSource.ZoomOut(); }
        private void ZoomFitImage() { ibSource.ZoomToFit(); }
        private void FormImageView_FormClosing(object sender, FormClosingEventArgs e) { ImageSource.Dispose(); }
        private void btnZoomOut_Click(object sender, EventArgs e) { ZoomOutImage(); }
        private void btnZoomIn_Click(object sender, EventArgs e) { ZoomInImage(); }
        private void btnFit_Click(object sender, EventArgs e) { ZoomFitImage(); }       
        #endregion

        private System.Drawing.Point GreyPoint = new System.Drawing.Point();
        private void ibSource_MouseMove(object sender, MouseEventArgs e)
        {            
            
            GreyPoint = UpdateCursorPosition(e.Location);
            lbPosition.Text = $"{GreyPoint.X},{GreyPoint.Y}";

            int nGreyValue = 0;

            if (GreyPoint.X + 10 < ImageGrey.Width && GreyPoint.Y + 10 < ImageGrey.Height)
            {
                if (GreyPoint.X > 0 && GreyPoint.Y > 0)
                {
                    System.Drawing.Color color = ImageGrey.GetPixel(GreyPoint.X, GreyPoint.Y);
                    nGreyValue = (color.R + color.G + color.B) / 3;
                    lbGV.Text = string.Format("{0}", nGreyValue.ToString());
                }
            }
            if (!KtemViewer.TrainROI.IsEmpty)
            {
                ibSource.Invalidate();
                Rectangle r = KtemViewer.TrainROI;
                if(r.Width > 5 || r.Height > 5)
                {
                    Bitmap ImageTemplate = Lib.Common.CBitmapProcessing.CropAtRect((Bitmap)ibSource.Image, r).Result;
                    ibTrainImage.Image = ImageTemplate;
                }                
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch((Keys)e.KeyValue)
            {
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                case Keys.Enter:
                    btnCut_Click(null, null);
                    break;
            }
        }

        private System.Drawing.Point UpdateCursorPosition(System.Drawing.Point location)
        {
            return ibSource.PointToImage(location);
        }

        private void btnSelectionMode_Click(object sender, EventArgs e)
        {            
            ibSource.SelectionMode = ImageBoxSelectionMode.Rectangle;
            ibSource.SelectionRegion = new RectangleF(ibSource.Image.Width / 2, ibSource.Image.Height / 2, ibSource.Image.Width / 8, ibSource.Image.Height / 8);
            ibSource.SelectionMode = ImageBoxSelectionMode.None;
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            using(Mat ImageSrc = ImageSource.Clone())
            {
                switch (Mode)
                {
                    case "ROI":
                        SelectedRegion = CConverter.RectToCVRect(KtemViewer.Roi);
                        if (SelectedRegion.X < 0)
                        {
                            SelectedRegion.X = 0;
                            SelectedRegion.Width = ImageSrc.Width;
                        }
                        if (SelectedRegion.Y < 0)
                        {
                            SelectedRegion.Y = 0;
                            SelectedRegion.Height = ImageSrc.Height;
                        }
                        break;
                    case "TRAIN":
                        SelectedRegion = CConverter.RectToCVRect(KtemViewer.TrainROI);
                        if (SelectedRegion.X < 0)
                        {
                            SelectedRegion.X = 0;
                            SelectedRegion.Width = ImageSrc.Width;
                        }
                        if (SelectedRegion.Y < 0)
                        {
                            SelectedRegion.Y = 0;
                            SelectedRegion.Height = ImageSrc.Height;
                        }
                        break;                                           
                    case "MULTI_ROI":
                        foreach(var ROI in KtemViewer._RoisOb)
                        {
                            Rect r = CConverter.RectangleToRect(ROI.Roi);
                            if (r.X < 0)
                            {
                                r.Width = r.Width - r.X;
                                r.X = 0;                                
                            }
                            if (r.Y < 0)
                            {
                                r.Height = r.Height - r.Y;
                                r.Y = 0;                                
                            }
                            SelectedRegions.Add(r);
                        }
                        
                        break;
                }

                

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ibSource.SelectionMode = ImageBoxSelectionMode.None;
        }

        private void btnMean_Click(object sender, EventArgs e)
        {
            using (Mat ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
            {
                Rect rtSubmat = new Rect(
                    (int)ibSource.SelectionRegion.X,
                    (int)ibSource.SelectionRegion.Y,
                    (int)ibSource.SelectionRegion.Width,
                    (int)ibSource.SelectionRegion.Height);

                double dMean = Cv2.Mean(ImageSrc.SubMat(rtSubmat)).Val0;
                btnMean.Text = $"Mean : {dMean.ToString("F2")}";
                ibSource.SelectionMode = ImageBoxSelectionMode.None;
            }
        }

        private void btnMatrixView_Click(object sender, EventArgs e)
        {
            Rect rtSubmatOrg = new Rect(
                    (int)ibSource.SelectionRegion.X,
                    (int)ibSource.SelectionRegion.Y,
                    (int)ibSource.SelectionRegion.Width,
                    (int)ibSource.SelectionRegion.Height);

            using (Mat ImageSrc = Lib.Common.CImageConverter.ToMat((Bitmap)ibSource.Image).Clone())
            using (Mat ImageSub = ImageSrc.SubMat(rtSubmatOrg).Clone())
            {
                Bitmap imageDisplay = Lib.Common.CImageConverter.ToBitmap(ImageSub);

                double dMean = Cv2.Mean(ImageSub).Val0-10;

                using (Graphics g = Graphics.FromImage(imageDisplay))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    int nRow = 12;
                    int nCol = 12;

                    int nW = imageDisplay.Width;
                    int nH = imageDisplay.Height;

                    int nSW = nW / nCol;
                    int nSH = nH / nRow;

                    for (int nx = 0; nx < nCol; nx++)
                    {
                        for (int ny = 0; ny < nRow; ny++)
                        {
                            Rect rtSubmat = new Rect((nSW * nx), (nSH * ny), nSW, nSH);

                            double dPartialMean = Cv2.Mean(ImageSub.SubMat(rtSubmat)).Val0;


                            Rectangle rtDrawing = new Rectangle((nSW * nx), (nSH * ny), nSW, nSH);
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Silver, 1), rtDrawing);

                            if (dPartialMean < dMean)
                            {
                                g.DrawString(((int)(dPartialMean)).ToString(), new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(System.Drawing.Color.Red), new PointF((nSW * nx), (nSH * ny)));
                            }
                            else
                            {
                                g.DrawString(((int)(dPartialMean)).ToString(), new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(System.Drawing.Color.Lime), new PointF((nSW * nx), (nSH * ny)));
                            }

                        }
                    }
                    ibSource.SelectionMode = ImageBoxSelectionMode.None;
                }

                ibSource.Image = imageDisplay;
            }

            ibSource.SelectionMode = ImageBoxSelectionMode.None;
            ibSource.SelectionRegion = new RectangleF();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(KtemViewer._Mode)
            {
                case RoiMode.Drag:
                    ibSource.Text = "DRAG MODE";
                    break;
                case RoiMode.ROI:
                    ibSource.Text = "ROI MODE";
                    break;
                case RoiMode.MultiROI:
                    ibSource.Text = "MULTI ROI MODE";
                    break;
                case RoiMode.Train:
                    ibSource.Text = "TRAIN ROI MODE";
                    break;
            }
            //if (!KtemViewer.TrainROI.IsEmpty)
            //{
            //    ibSource.Invalidate();                
            //    Rectangle r = KtemViewer.TrainROI;                
            //    Bitmap ImageTemplate = Lib.Common.CBitmapProcessing.CropAtRect((Bitmap)ibSource.Image, r).Result;                
            //    ibTrainImage.Image = ImageTemplate;
            //}
        }

        private void Onbtn_Click(object sender, EventArgs e)
        {
            string strIndex = ((RJCodeUI_M1.RJControls.RJButton)sender).Name;

            switch(strIndex)
            {
                case "btnDrag":
                    KtemViewer.SetModeDrag();
                    break;
                case "btnROI":
                    KtemViewer.SetModeRoi();
                    break;
                case "btnMultiRoi":
                    KtemViewer.SetModeMultiRoi();
                    break;
                case "btnTrainROI":
                    KtemViewer.SetModeTrainRoi();
                    break;                    
            }
        }
    }
}
