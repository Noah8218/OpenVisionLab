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
using Cognex.VisionPro;

namespace KtemVisionSystem
{
    public partial class FormLayerCogDisplay : WeifenLuo.WinFormsUI.Docking.DockContent
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

        public int nIndex = 0;

        public FormLayerCogDisplay(CogImage8Grey ImageSource, int nIndex, List<FormLayerCogDisplay> LayerDisplayList, bool UseCloseButton = true, string strTitle = "TEST")
        {           
            InitializeComponent();

            CogDisplayStatusBarV2 cogDisplayStatusBar_Source = new CogDisplayStatusBarV2();
            cogDisplayStatusBar_Source.Display = cogDisplay_Source;

            splitContainer1.Panel2.Controls.Add(cogDisplayStatusBar_Source);

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
                cogDisplay_Source.Image = ImageSource;
                cogDisplay_Source.Fit(false);
            }

            this.nIndex = nIndex;
            m_LayerDisplayList = LayerDisplayList;

            cogDisplay_Source.DoubleClick += CogDisplay_Source_DoubleClick;
        }

        private void CogDisplay_Source_DoubleClick(object sender, EventArgs e)
        {
            Cognex.VisionPro.Display.CogDisplay Display = (sender as Cognex.VisionPro.Display.CogDisplay);
            Display.Fit(false);
        }

        public void SetCogImage(CogImage8Grey ImageSource)
        {
            try
            {
                cogDisplay_Source.Image = ImageSource;
                //cogDisplay_Source.Fit(true);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public CogImage8Grey GetCogImage()
        {                        
            return (CogImage8Grey)cogDisplay_Source.Image;
        }

        public bool ExistsCogImage()
        {
            if (cogDisplay_Source.Image == null) { return false; }
            else { return true; }
        }

        public void AddGraphics(CogRectangle cogROI, string strTitle)
        {
            try
            {
                cogDisplay_Source.InteractiveGraphics.Add(cogROI, strTitle, false);
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void ClearGraphics()
        {
            try
            {
                cogDisplay_Source.InteractiveGraphics.Clear();
                cogDisplay_Source.StaticGraphics.Clear();

                if(cogDisplay_Source.Image != null)
                {

                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void GetImageSize(out int nWidth, out int nHeight)
        {
            nWidth = 0; nHeight = 0;

            try
            {
                if (cogDisplay_Source.Image == null) { return; }

                nWidth = cogDisplay_Source.Image.Width;
                nHeight = cogDisplay_Source.Image.Height;
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        private void LayerDisplay_Load(object sender, EventArgs e)
        {
            
        }

      
        private void ListClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                switch (e.ClickedItem.Text)
                {
                    case "Save Current Image":
                        //TaskUtil.Start(() =>
                        //{
                        //    CUtil.InitDirectory("CAPTURE");

                        //    string strSavePath = $"{Application.StartupPath}\\CAPTURE\\{this.Text}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bmp";
                        //    HOperatorSet.WriteImage(hImageSource, "bmp", 0, strSavePath);
                        //});
                        break;
                    case "Swap Train Image":

                        //if(EventUpdateChangeImage != null)
                        //{
                        //    EventUpdateChangeImage(null, new HDisplayLayerEventArgs(this));
                        //}

                        //CUtil.HImageSwap(ref CGlobal.Instance.iSystem.MainDisplay.hImageSource, ref this.hImageSource);
                        
                        //HOperatorSet.ClearWindow(CGlobal.Instance.iSystem.MainDisplay.hv_WindowHandle);                        
                        //HOperatorSet.ClearWindow(hv_WindowHandle);
                        //HOperatorSet.DispObj(CGlobal.Instance.iSystem.MainDisplay.hImageSource, CGlobal.Instance.iSystem.MainDisplay.hv_WindowHandle);
                        //HOperatorSet.DispObj(this.hImageSource, hv_WindowHandle);
                        //CGlobal.Instance.iSystem.MainDisplay.hwImage.SetFullImagePart();
                        //this.hwImage.SetFullImagePart();                        
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CLogger.Add(LOG.EXCEPTION, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public List<FormLayerCogDisplay> m_LayerDisplayList = new List<FormLayerCogDisplay>();

        private void LayerDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (cogDisplay_Source != null) 
                {
                    m_LayerDisplayList.RemoveAt(nIndex);
                    //CGlobal.Instance.LayerDisplayList.RemoveAt(nIndex);
                    cogDisplay_Source.Dispose(); 
                }                
            }
            catch
            {

            }
        }
    }
}
