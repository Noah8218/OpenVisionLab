using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Matrox.MatroxImagingLibrary;
using System.Xml;
using System.Reflection;
using System.Drawing.Imaging;
using System.Diagnostics;
using KtemVisionSystem;
using log4net.Repository.Hierarchy;
using static KtemVisionSystem.CLOG;
using OpenCvSharp;
using CodeMeter;
using static PACNET.Sys;
using System.Windows.Media.Media3D;

namespace IntelligentFactory
{
    public class CGrabMatroxStruct
    {
        private const int BUFFERING_SIZE_MAX = 10;

        public MIL_ID MilDigitizer;
        public MIL_ID[] MilImageGrab = new MIL_ID[BUFFERING_SIZE_MAX];
        public int ProcessedImageCount;
    }


    public class CGrabberMatroxSolios
    {
        /// <summary>
        /// Matrox 보드 종류
        /// </summary>
        public enum EMatroxBoardType
        {
            M_SYSTEM_1394,
            M_SYSTEM_CRONOSPLUS,
            M_SYSTEM_DEFAULT,
            M_SYSTEM_GIGE_VISION,
            M_SYSTEM_GPU,
            M_SYSTEM_HOST,
            M_SYSTEM_IRIS_GT,
            M_SYSTEM_MORPHIS,
            M_SYSTEM_MORPHISQXT,
            M_SYSTEM_ORION_HD,
            M_SYSTEM_RADIENT,
            M_SYSTEM_RADIENTCLHS,
            M_SYSTEM_RADIENTCXP,
            M_SYSTEM_RADIENTEVCL,
            M_SYSTEM_RADIENTPRO,
            M_SYSTEM_SOLIOS,
            M_SYSTEM_USB3_VISION,
            M_SYSTEM_VIO,
            Other
        }

        private readonly object lockGrab;
        public int Index { get; set; } = 0;
        public int ImageHeight { get; set; } = 4000;
        public int ImageWidth { get; set; } = 4096;
        public int Threshold { get; set; } = 125;
        public double ExposureTime { get; set; } = 10000.0D;
        public double Gain { get; set; } = 1.0D;
        public double PixelPermm { get; set; } = 0.05D;
        public string Name { get; set; } = "";
        public bool ViewModeCrss { get; set; } = false;

        public ManualResetEvent IsGrabDone = new ManualResetEvent(false);

        public bool IsOpen { get; set; } = false;

        public  System.Drawing.Size GrabSize, DispSize = new System.Drawing.Size(100, 100);
        private float m_fScaleFactorX, m_fScaleFactorY = 1.0F;

        public MIL_ID MIL_System;
        public MIL_ID MIL_App;

        public MIL_ID MIL_Digitizer;
        public MIL_ID MIL_Display;
        public MIL_ID MIL_ImageBuffer;

        public MIL_ID MIL_GrabBuffer;
        public MIL_ID MIL_ProcBuffer;
        public MIL_ID MIL_DispBuffer;

        public MIL_ID MIL_LastBuffer;

        public MIL_INT MIL_DispAttribute = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC;
        public MIL_INT MIL_GrabAttribute = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC + MIL.M_GRAB;

        public MIL_INT MIL_Channel;
        public MIL_INT MIL_Width;
        public MIL_INT MIL_Height;

        public MIL_INT MIL_Type = 8 + MIL.M_UNSIGNED;

        public MIL_INT MIL_RowBuffer = 450;   //그랩할 로우
        public MIL_INT MIL_BufferCount = 25;  //로우 몇 세트 그랩하는지

        //public MIL_DIG_HOOK_FUNCTION_PTR GrabProcessDelegate_GrabEnd;

        public class HookDataObject // User's archive function hook data structure.
        {
            public MIL_ID MilSystem;
            public MIL_ID MilDisplay;
            public MIL_ID MilImageDisp;
            public MIL_ID MilCompressedImage;
            public int NbGrabbedFrames;
            public int NbArchivedFrames;
            public bool SaveSequenceToDisk;
        };

        public HookDataObject UserHookData = new HookDataObject();

        public IntPtr Handle;

        public int Channel = 0;
        public string FILE_PATH_DCF = "basler 4k_base(2tap)_Freerun_v10.dcf";

        public event EventHandler<GrabEventArgs> EventGrabEnd;
        public CGrabberMatroxSolios(int nIndex, string strName)
        {
            Index = nIndex;
            Name = strName;

            MIL_Digitizer = MIL.M_NULL;
            MIL_Display = MIL.M_NULL;

            MIL_ImageBuffer = MIL.M_NULL;
            MIL_GrabBuffer = MIL.M_NULL;
            MIL_ProcBuffer = MIL.M_NULL;
            MIL_DispBuffer = MIL.M_NULL;
        }

        public bool Init()
        {
            try
            {
                FILE_PATH_DCF = Application.StartupPath + "\\" + "소프트웨어.dcf";

                //ALLOC 설정
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref MIL_App);
                MIL.MappControl(MIL_App, MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

                //MIL.MappAlloc(MIL.M_DEFAULT, ref MIL_App);

                var insSysCount = MIL.MappInquire(MIL.M_INSTALLED_SYSTEM_COUNT);

                for (int i = 0; i < insSysCount; i++)
                {
                    MIL_ID tmpSystem = MIL.M_NULL;
                    StringBuilder sb = new StringBuilder();

                    //보드 종류 문자열로 뽑아내기
                    MIL.MappInquire(MIL.M_INSTALLED_SYSTEM_DESCRIPTOR + i, sb);

                    var devCount = 0;
                    //같은 종류의 보드 몇 개까지 존재하는지 확인
                    while (sb.ToString() != EMatroxBoardType.M_SYSTEM_HOST.ToString())
                    {
                        MIL_ID systemId = MIL.M_NULL;
                        try
                        {
                            //보드 alloc
                            MIL.MsysAlloc(sb.ToString(), devCount, MIL.M_DEFAULT, ref systemId);
                        }
                        catch
                        {
                            break;
                        }
                        MIL_System = systemId;
                        var digCount = MIL.MsysInquire(MIL_System, MIL.M_DIGITIZER_NUM);                        
                        //Digitizer 몇개 존재하는지 확인
                        for (int ii = 0; ii < digCount; ii++)
                        {                            
                            MIL.MdigAlloc(MIL_System, Channel, FILE_PATH_DCF, MIL.M_DEFAULT, ref MIL_Digitizer);
                            MIL.MdispAlloc(MIL_System, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MIL_Display);
                            MIL.MdigControl(MIL_Digitizer, MIL.M_CAMERALINK_CC3_SOURCE, MIL.M_GRAB_EXPOSURE);

                            //MIL.MdigControl(MIL_Digitizer, MIL.M_GRAB_MODE, MIL.M_ASYNC);

                            MIL_Width = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_X, MIL.M_NULL);
                            MIL_Height = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_Y, MIL.M_NULL);
                            MIL_Channel = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_BAND, MIL.M_NULL);
                        }
                        devCount++;
                    }
                }

                //MIL.MsysAlloc(MIL.m_sys, MIL.M_DEV0, MIL.M_DEFAULT, ref MIL_System);



                // 임시로 주석
#if !Dbug
                MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_Y, MIL.M_NULL);

                MIL.MbufAllocColor(MIL_System, MIL_Channel, MIL_Width, MIL_Height, 8 + MIL.M_UNSIGNED, MIL_GrabAttribute, ref MIL_GrabBuffer);
                MIL.MbufClear(MIL_GrabBuffer, 0);

                MIL.MbufAllocColor(MIL_System, MIL_Channel, MIL_Width, MIL_Height, 8 + MIL.M_UNSIGNED, MIL_GrabAttribute, ref MIL_LastBuffer);
                MIL.MbufClear(MIL_LastBuffer, 0);

                MIL.MbufAllocColor(MIL_System, MIL_Channel, MIL_Width, MIL_Height, 8 + MIL.M_UNSIGNED, MIL_GrabAttribute, ref MIL_ProcBuffer);
                MIL.MbufClear(MIL_ProcBuffer, 0);

                MIL.MbufAllocColor(MIL_System, MIL_Channel, MIL_Width, MIL_Height, 8 + MIL.M_UNSIGNED, MIL_GrabAttribute, ref MIL_DispBuffer);
                MIL.MbufClear(MIL_DispBuffer, 0);
#endif
                if ((int)MIL_Width == 0 || (int)MIL_Height == 0)
                {
                    IsOpen = false;
                    return false;
                }

                ImageWidth = (int)MIL_Width;
                ImageHeight = (int)MIL_Height;

                IsOpen = true;

                AcqStart();
            }
            catch (Exception Desc)
            {
               // Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        public void AcqStart()
        {
            
            int tmpChannel = 0;
            //픽셀 형식에 따라 채널 설정
            tmpChannel = 1;

            //이미지 버퍼 등록
            MIL.MbufAllocColor(MIL_System, MIL_Channel, (int)MIL_Width, (int)MIL_Height, 8, MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC, ref MIL_GrabBuffer);            
        }


        public void GrabStart()
        {
            try
            {
                if (IsOpen)
                {
                    //Thread 로 구성해야될 필요가 있습니다.
                    IsGrabDone.Reset();
                    byte[] rawImage = new byte[(int)MIL_Width * (int)MIL_Height* 1];

                    //이미지 버퍼에 복사                    
                    MIL.MdigGrab(MIL_Digitizer, MIL_GrabBuffer);
                    MIL.MbufGet2d(MIL_GrabBuffer, (int)0, (int)0, (int)MIL_Width, (int)MIL_Height, rawImage);

                    Bitmap image = ByteToBitmap(rawImage, (int)MIL_Width, (int)MIL_Height);

                    if (EventGrabEnd != null)
                    {
                        EventGrabEnd(null, new GrabEventArgs(CConverter.ToMat(image), 0));
                    }

                    IsGrabDone.Set();
                }
            }
            catch (Exception Desc)
            {
               // Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public Bitmap LoadImage(string strImagePath, ref MIL_ID image)
        {
            Bitmap bmp = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format24bppRgb);

            try
            {
                int nSTART = Environment.TickCount;
                byte[] buff = new byte[ImageWidth * ImageHeight * 3];

                if (image != MIL.M_NULL)
                    MIL.MbufFree(image);

                MIL.MbufRestore(strImagePath, MIL_System, ref image);
                MIL.MbufGet(image, buff); // MilImage-> MIL 이미지 ,  UserBuffer -> Array 버퍼                
                bmp = ByteToBitmap(buff, ImageWidth, ImageHeight);

                int nEnd = Environment.TickCount - nSTART;
            }
            catch (Exception Desc)
            {
               // Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return bmp;
            }

            return bmp;
        }

        public Bitmap ByteToBitmap(byte[] imgArr, int nW, int nH)
        {
            Bitmap bmp = new Bitmap(nW, nH, PixelFormat.Format8bppIndexed);
            // IntPtr ptr = res.GetHbitmap();

            BitmapData data = bmp.LockBits(
                                    new Rectangle(0, 0, nW, nH),
                                    ImageLockMode.ReadWrite,
                                        PixelFormat.Format8bppIndexed);
            IntPtr ptr = data.Scan0;
            Marshal.Copy(imgArr, 0, ptr, nW * nH);
            bmp.UnlockBits(data);

            //모노이미지로 변환해준다 사용하지 않을경우 칼라이미지가 깨진채로 사용된다
            ColorPalette Gpal = bmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                Gpal.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = Gpal;

            return bmp;
        }

        public void Close()
        {
            try
            {
                Live(false);
            }
            catch (Exception Desc)
            {
              //  Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public void Live(bool bEnable)
        {
            if (!IsOpen) return;

            if (bEnable)
            {
                StartThreadLive();
            }
            else
            {
                StopThreadLive();
                ResetThreadLive();
            }

        }

        #region Thread
        private CThreadStatus m_ThreadStatusLive = new CThreadStatus();
        public CThreadStatus ThreadStatusLive
        {
            get { return m_ThreadStatusLive; }
        }

        private void StartThreadLive()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadLive));
            t.Start(m_ThreadStatusLive);
        }

        public void StopThreadLive()
        {
            if (!ThreadStatusLive.IsExit())
            {
                ThreadStatusLive.Stop(100);
            }
        }

        private void ResetThreadLive()
        {
            m_ThreadStatusLive.End();
        }

        private void ThreadLive(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Live Thread");
            //Logger.WriteLog(LOG.Normal, "Live Thread");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(100);

                    GrabStart();

                    IsGrabDone.WaitOne();
                }
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }
        #endregion

        //#region File Manager              
        //private string m_XMLName = "CameraSetting";
        //public bool LoadConfig(string strRecipe)
        //{
        //    try
        //    {
        //        string strPath = Application.StartupPath + "\\" + DEFINE.RECIPE + "\\" + strRecipe + "\\" + m_XMLName + Index.ToString() + ".xml";

        //        if (File.Exists(strPath))
        //        {
        //            XmlTextReader xmlReader = new XmlTextReader(strPath);

        //            try
        //            {
        //                LoadConfigByXML(xmlReader);
        //            }
        //            catch (Exception e)
        //            {
        //                xmlReader.Close();
        //            }

        //            xmlReader.Close();
        //        }
        //        else
        //        {
        //            SaveConfig(strRecipe);
        //            return false;
        //        }
        //    }
        //    catch (Exception Desc)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SaveConfig(string strRecipe)
        //{
        //    string strPath = Application.StartupPath + "\\" + DEFINE.RECIPE + "\\" + strRecipe + "\\" + m_XMLName + Index.ToString() + ".xml";
        //    //IData.InitRecipeDirectory(strRecipe);

        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.NewLineOnAttributes = true;
        //    settings.IndentChars = "\t";
        //    settings.NewLineChars = "\r\n";
        //    XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
        //    try
        //    {
        //        xmlWriter.WriteStartDocument();

        //        SaveConfigByXML(xmlWriter);
        //        xmlWriter.WriteEndDocument();
        //    }
        //    catch (Exception Desc)
        //    {

        //    }
        //    finally
        //    {
        //        xmlWriter.Flush();
        //        xmlWriter.Close();
        //    }
        //    return true;
        //}

        //public bool LoadConfigByXML(XmlReader xmlReader)
        //{
        //    try
        //    {
        //        while (xmlReader.Read())
        //        {
        //            if (xmlReader.NodeType == XmlNodeType.Element)
        //            {
        //                switch (xmlReader.Name)
        //                {
        //                    case "ExposureTime":
        //                        if (!xmlReader.Read()) return false;
        //                        ExposureTime = double.Parse(xmlReader.Value);
        //                        break;
        //                    case "Gain":
        //                        if (!xmlReader.Read()) return false;
        //                        Gain = double.Parse(xmlReader.Value);
        //                        break;
        //                    case "Threshold":
        //                        if (!xmlReader.Read()) return false;
        //                        Threshold = int.Parse(xmlReader.Value);
        //                        break;
        //                    case "ROI":
        //                        if (!xmlReader.Read()) return false;
        //                        string[] strROI = xmlReader.Value.Split(',');

        //                        if (strROI.Length == 4)
        //                        {
        //                            int nX = int.Parse(strROI[0]);
        //                            int nY = int.Parse(strROI[1]);
        //                            int nWidth = int.Parse(strROI[2]);
        //                            int nHeight = int.Parse(strROI[3]);

        //                            //ROI = new Rect(nX, nY, nWidth, nHeight);
        //                        }
        //                        break;
        //                    case "PixelPermm":
        //                        if (!xmlReader.Read()) return false;
        //                        PixelPermm = double.Parse(xmlReader.Value);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                if (xmlReader.NodeType == XmlNodeType.EndElement)
        //                {
        //                    if (xmlReader.Name == m_XMLName) break;
        //                }
        //            }
        //        }

        //        //Logger.WriteLog(LOG.SYS, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
        //        return true;
        //    }
        //    catch (Exception Desc)
        //    {
        //        //Logger.WriteLog(LOG.ERR, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
        //        return false;
        //    }
        //}

        //public bool SaveConfigByXML(XmlWriter xmlWriter)
        //{
        //    xmlWriter.WriteStartElement("Parameter");
        //    xmlWriter.WriteElementString("ExposureTime", ExposureTime.ToString());
        //    xmlWriter.WriteElementString("Gain", Gain.ToString());
        //    xmlWriter.WriteElementString("Threshold", Threshold.ToString());
        //    //xmlWriter.WriteElementString("ROI", string.Format("{0},{1},{2},{3}", ROI.X, ROI.Y, ROI.Width, ROI.Height));
        //    xmlWriter.WriteElementString("PixelPermm", PixelPermm.ToString());
        //    xmlWriter.WriteEndElement();
        //    return true;
        //}
        //#endregion
    }
}