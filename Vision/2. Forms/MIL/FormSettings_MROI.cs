#if MATROX
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Matrox.MatroxImagingLibrary;

using MetroFramework;
using MetroFramework.Forms;

namespace IntelligentFactory
{
    public partial class FormSettings_MROI : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;
        public event EventHandler<RectEventArgs> EventRoiEnd;

        private int m_nMinY = 1;
        private int m_nMaxY = 540;
        private int m_nMinX = 1;
        private int m_nMaxX = 720;
        
        private double dX = 0;
        private double dY = 0;
        private double dW = 0;
        private double dH = 0;

        private Point m_ptStart = new Point();
        private Point m_ptEnd = new Point();

        private MIL_ID ImageSource;
        private MIL_ID Display;

        public Rectangle ROI = new Rectangle();

        private Bitmap ImageDisplay = null;

        private const int IMAGE_THRESHOLD_VALUE = 110;

        // Initial region parameters.
        private const int RECTANGLE_POSITION_X = 160;
        private const int RECTANGLE_POSITION_Y = 310;
        private const int RECTANGLE_WIDTH = 200;
        private const int RECTANGLE_HEIGHT = 175;
        private const int RECTANGLE_ANGLE = 0;

        private string Mode = "";

        public FormSettings_MROI(MIL_ID image, MIL_ID disp, string strMode)
        {
            InitializeComponent();

            try
            {
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.ResizeRedraw, true);

                this.DoubleBuffered = true;
                UpdateStyles();

                m_nMinY = 1;
                m_nMaxY = pnDisplay.Height;
                m_nMinX = 1;
                m_nMaxX = pnDisplay.Width;

                ImageSource = image;
                Display = disp;

                Mode = strMode;

                //MIL.MdispSelect(Display, image);

                MIL.MdispSelectWindow(Display, image, pnDisplay.Handle);

                InitInteractivity();
                UpdateDisplay();
            }
            catch (Exception Desc)
            {
            }
        }

        private MIL_ID MilImage = MIL.M_NULL;
        private MIL_ID MilGraphicsList = MIL.M_NULL;                    // Graphics list identifier.
        private MIL_ID MilGraphicsContext = MIL.M_NULL;                 // Graphics context identifier.
        private MIL_ID MilBinImage = MIL.M_NULL;                        // Binary image buffer identifier.
        private MIL_ID MilBlobContext = MIL.M_NULL;                     // Context identifier.
        private MIL_ID MilBlobResult = MIL.M_NULL;                      // Blob result buffer identifier.
        
        private MIL_INT SizeX = 0;                                      // Size X of the source buffer.
        private MIL_INT SizeY = 0;                                      // Size Y of the source buffer.
        private MIL_INT RegionLabel = 0;                                // Label value of the region.

        private void InitInteractivity()
        {

        }

        private void Close()
        {
            //MIL.MgraHookFunction(MilGraphicsList, MIL.M_GRAPHIC_MODIFIED + MIL.M_UNHOOK, HookHandlerDelegate, GCHandle.ToIntPtr(DataStructureHandle));
            //DataStructureHandle.Free();

            //// Free all allocations.
            //MIL.MblobFree(MilBlobResult);
            //MIL.MblobFree(MilBlobContext);
            //MIL.MbufFree(MilBinImage);
            //MIL.MgraFree(MilGraphicsContext);
            //MIL.MgraFree(MilGraphicsList);
            //MIL.MbufFree(MilImage);
        }

        private static MIL_INT HookHandler(MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr)
        {
            // this is how to check if the user data is null, the IntPtr class
            // contains a member, Zero, which exists solely for this purpose
            if (!IntPtr.Zero.Equals(UserDataPtr))
            {
                // get the handle to the DigHookUserData object back from the IntPtr
                GCHandle hUserData = GCHandle.FromIntPtr(UserDataPtr);

                // get a reference to the DigHookUserData object
                STestParameters DataStructure = hUserData.Target as STestParameters;

                // Check that the modified graphic is the rectangular region.
                MIL_INT ModifiedGraphicLabel = 0;
                MIL.MgraGetHookInfo(EventId, MIL.M_GRAPHIC_LABEL_VALUE, ref ModifiedGraphicLabel);

                if (ModifiedGraphicLabel == DataStructure.RegionLabel)
                {

                    //// Count objects and draw the corresponding annotations.
                    //CountObjects(DataStructure.MilDisplay,
                    //             DataStructure.MilGraphicsList,
                    //             DataStructure.MilGraphicsContext,
                    //             DataStructure.MilBinImage,
                    //             DataStructure.MilBlobContext,
                    //             DataStructure.MilBlobResult);
                }
            }

            return MIL.M_NULL;
        }

        private static void CountObjects(MIL_ID MilDisplay, MIL_ID MilGraphicsList, MIL_ID MilGraphicsContext, MIL_ID MilBinImage, MIL_ID MilBlobContext, MIL_ID MilBlobResult)
        {
            MIL_INT NumberOfBlobs = 0;
            MIL_INT NumberOfPrimitives = 0;
            MIL_INT Index;

            string TextLabel;

            // Disable the display update for better performance.
            MIL.MdispControl(MilDisplay, MIL.M_UPDATE, MIL.M_DISABLE);

            // Remove all elements from the graphics list, except the rectangle
            // region primitive at index 0.
            MIL.MgraInquireList(MilGraphicsList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_NUMBER_OF_GRAPHICS, ref NumberOfPrimitives);
            for (Index = NumberOfPrimitives - 1; Index > 0; Index--)
            {
                MIL.MgraControlList(MilGraphicsList, MIL.M_GRAPHIC_INDEX(Index), MIL.M_DEFAULT, MIL.M_DELETE, MIL.M_DEFAULT);
            }

            // Set the input region. The blob analysis will be done
            // from the (filled) interactive rectangle.           
            MIL.MbufSetRegion(MilBinImage, MilGraphicsList, MIL.M_DEFAULT, MIL.M_RASTERIZE + MIL.M_FILL_REGION, MIL.M_DEFAULT);

            // Calculate the blobs and their features.
            MIL.MblobCalculate(MilBlobContext, MilBinImage, MIL.M_NULL, MilBlobResult);

            // Get the total number of blobs.
            MIL.MblobGetResult(MilBlobResult, MIL.M_GENERAL, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref NumberOfBlobs);

            // Set the input units to display unit for the count annotations.
            MIL.MgraControl(MilGraphicsContext, MIL.M_INPUT_UNITS, MIL.M_DISPLAY);
            TextLabel = string.Format(" Number of blobs found: {0,2} ", NumberOfBlobs);

            MIL.MgraColor(MilGraphicsContext, MIL.M_COLOR_WHITE);
            MIL.MgraText(MilGraphicsContext, MilGraphicsList, 10, 10, TextLabel);

            // Restore the input units to pixel units for result annotations.
            MIL.MgraControl(MilGraphicsContext, MIL.M_INPUT_UNITS, MIL.M_PIXEL);

            // Draw blob center of gravity annotation.
            MIL.MgraColor(MilGraphicsContext, MIL.M_COLOR_RED);
            MIL.MblobDraw(MilGraphicsContext, MilBlobResult, MilGraphicsList, MIL.M_DRAW_CENTER_OF_GRAVITY, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);

            // Draw blob bounding box annotations.
            MIL.MgraColor(MilGraphicsContext, MIL.M_COLOR_GREEN);
            MIL.MblobDraw(MilGraphicsContext, MilBlobResult, MilGraphicsList, MIL.M_DRAW_BOX, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);

            // Enable the display to update the drawings.
            MIL.MdispControl(MilDisplay, MIL.M_UPDATE, MIL.M_ENABLE);
        }

        private void OnClickOperation(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is Button)) return;

                string strIndex = (sender as Button).Text;

                switch (strIndex)
                {
                    case "OK":
                        
                        break;
                    case "CANCEL":
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        break;
                    case "LOAD":
                        
                        break;
                }

            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void UpdateDisplay(bool bDrawROI = true)
        {
            try
            {
                //if (ImageSource == null) return;
                //if (ImageSource.IsVoid) return;

                //ImageDisplay = new Bitmap(pblImage.Width, pblImage.Height);

                //using (Graphics g = Graphics.FromImage(ImageDisplay))
                //{
                //    m_fScaleFactorResizeX = (float)pblImage.Width / (float)ImageSource.Width;
                //    m_fScaleFactorResizeY = (float)pblImage.Height / (float)ImageSource.Height;

                //    ImageSource.Draw(g, m_fScaleFactorResizeX, m_fScaleFactorResizeY);

                //    ROI.Attach(ImageSource);                    
                //    if (bDrawROI) ROI.DrawFrame(g, true, m_fScaleFactorResizeX, m_fScaleFactorResizeY);


                //    pblImage.Image = ImageDisplay;
                //}

            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                }

                if (e.Button == MouseButtons.Left && ImageSource != null)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (e.Y < m_nMinY) { nY = m_nMinY; }
                    if (e.Y > m_nMaxY) { nY = m_nMaxY; }
                    if (e.X < m_nMinX) { nX = m_nMinX; }
                    if (e.X > m_nMaxX) { nX = m_nMaxX; }

                    //m_fScaleFactorResizeX = (float)(pblImage.Width) / (float)(ImageSource.Width);
                    //m_fScaleFactorResizeY = (float)(pblImage.Height) / (float)(ImageSource.Height);

                    m_ptStart = new Point(nX, nY);
                    m_ptEnd = new Point(nX, nY);


                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && ImageSource != null)
                {
                    int nX = e.X;
                    int nY = e.Y;

                    if (e.Y < m_nMinY) { nY = m_nMinY; }
                    if (e.Y > m_nMaxY) { nY = m_nMaxY; }
                    if (e.X < m_nMinX) { nX = m_nMinX; }
                    if (e.X > m_nMaxX) { nX = m_nMaxX; }

                    //if (m_DragHandle != EDragHandle.NoHandle)
                    //{
                    //    ROI.Drag(m_DragHandle, nX, nY, m_fScaleFactorResizeX, m_fScaleFactorResizeY);
                    //    UpdateDisplay();

                    //    lbROI.Text = $"X : {ROI.OrgX} Y : {ROI.OrgY} W : {ROI.Width} H : {ROI.Height}";
                    //}

                    m_ptEnd = new Point(nX, nY);

                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
            }
        }

        private void pblImage_MouseUp(object sender, MouseEventArgs e)
        {
            //m_DragHandle = EDragHandle.NoHandle;
        }


        public Rectangle ScreenRectToLogicalRect(Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X / fScaleFactorX);
            rt.Y = (int)(rt.Y / fScaleFactorY);
            rt.Width = (int)(rt.Width / fScaleFactorX);
            rt.Height = (int)(rt.Height / fScaleFactorY);

            return rt;
        }

        public Rectangle LogicalRectToScreenRect(Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X * fScaleFactorX);
            rt.Y = (int)(rt.Y * fScaleFactorY);
            rt.Width = (int)(rt.Width * fScaleFactorX);
            rt.Height = (int)(rt.Height * fScaleFactorY);

            return rt;
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            //ROI.Save(Application.StartupPath + "\\Template.bmp", EImageFileType.Bmp);
            //Global.iSystem.Recipe.ImageTemplate.SetSize(ROI);
            //EasyImage.Oper(EArithmeticLogicOperation.Copy, ROI, Global.iSystem.Recipe.ImageTemplate);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CUtil.ShowMessageBox("SAVE", "Do you want to save the image?") == true)
            {
                //ROI.Save(Application.StartupPath + "\\Template.bmp", EImageFileType.Bmp);
                //Global.iSystem.Recipe.ImageTemplate.SetSize(ROI);
                //EasyImage.Oper(EArithmeticLogicOperation.Copy, ROI, Global.iSystem.Recipe.ImageTemplate);
            }
        }

        private void btnImageLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "이미지 로드";
            ofd.Filter = "All Files(*.*)|*.*|Bitmap File(*.bmp)|*.bmp|JPEG File(*.jpg)|*.jpg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strPath = ofd.FileName;



                //ImageDisplay = new Bitmap(pblImage.Width, pblImage.Height);

                //using (Graphics g = Graphics.FromImage(ImageDisplay))
                //{
                //    m_fScaleFactorResizeX = (float)pblImage.Width / (float)ImageSource.Width;
                //    m_fScaleFactorResizeY = (float)pblImage.Height / (float)ImageSource.Height;

                //    ImageSource.Draw(g, m_fScaleFactorResizeX, m_fScaleFactorResizeY);

                //    ROI.Attach(ImageSource);
                //    ROI.SetPlacement(ImageSource.Width / 2, ImageSource.Height / 2, 100, 100);
                //    ROI.DrawFrame(g, true, m_fScaleFactorResizeX, m_fScaleFactorResizeY);


                //    pblImage.Image = ImageDisplay;
                //}
            }
        }

        private void btnImageSave_Click(object sender, EventArgs e)
        {
            try
            {
                //if (ImageSource.IsVoid) return;

                //IUtil.InitDirectory(DEFINE.CAPTURE);
                //string strImagePath = Application.StartupPath + "\\" + DEFINE.CAPTURE + "\\Image_" + DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".jpg";

                //int[] sizes = new int[2] { (int)ImageSource.Height, (int)ImageSource.Width };
                //IntPtr intPtr = ImageSource.GetImagePtr();
                //OpenCvSharp.Mat MatGrab = new OpenCvSharp.Mat(sizes, OpenCvSharp.MatType.CV_8UC1, intPtr);
                //OpenCvSharp.Cv2.ImShow("TEST", MatGrab);
                //MatGrab = MatGrab.Resize(new OpenCvSharp.Size(pblMain.Width, pblMain.Height));
                //pblMain.ImageIpl = MatGrab;

                //if (strImagePath != "") ImageSource.Save(strImagePath);
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, )
            }
        }

        private void pnDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    MIL.MgraInquireList(MilGraphicsList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_LAST_LABEL, ref RegionLabel);

                   

                    MIL.MgraInquireList(MilGraphicsList, MIL.M_GRAPHIC_INDEX(0), MIL.M_DEFAULT, MIL.M_POSITION_X, ref dX);
                    MIL.MgraInquireList(MilGraphicsList, MIL.M_GRAPHIC_INDEX(0), MIL.M_DEFAULT, MIL.M_POSITION_Y, ref dY);
                    MIL.MgraInquireList(MilGraphicsList, MIL.M_GRAPHIC_INDEX(0), MIL.M_DEFAULT, MIL.M_RECTANGLE_WIDTH, ref dW);
                    MIL.MgraInquireList(MilGraphicsList, MIL.M_GRAPHIC_INDEX(0), MIL.M_DEFAULT, MIL.M_RECTANGLE_HEIGHT, ref dH);


                    ROI = new Rectangle((int)dX, (int)dY, (int)dW, (int)dH);
                    lbROI.Text = ROI.ToString();
                }
            }
            catch(Exception Desc)
            {

            }            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(EventRoiEnd != null)
                {
                    EventRoiEnd(this, new RectEventArgs(ROI, Mode));
                }
                this.DialogResult = DialogResult.OK;
            }
            catch(Exception Desc)
            {

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception Desc)
            {

            }
        }
    }
    public class STestParameters
    {
        public MIL_ID MilDisplay;
        public MIL_ID MilGraphicsList;
        public MIL_ID MilGraphicsContext;
        public MIL_ID MilBinImage;
        public MIL_ID MilBlobContext;
        public MIL_ID MilBlobResult;
        public MIL_INT RegionLabel;
    }
}

#endif