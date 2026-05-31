using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using Matrox.MatroxImagingLibrary;
using System.Drawing.Imaging;
using OpenVisionLab;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using OpenCvSharp;
using Lib.Common;

namespace OpenVisionLab
{
    public class CGrabMatroxStruct
    {
        private const int BUFFERING_SIZE_MAX = 10;

        public MIL_ID MilDigitizer;
        public MIL_ID[] MilImageGrab = new MIL_ID[BUFFERING_SIZE_MAX];
        public int ProcessedImageCount;
    }


    public class CGrabberMatrox
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

        // 콜벡 이벤트 객체
        private MIL_DIG_HOOK_FUNCTION_PTR _GrabCallbackFunc;

        public CPropertyCamera Property { get; set; } = new CPropertyCamera("CAMERA");
        public CViewer ImageManager { get; set; } = new CViewer();
        public ManualResetEvent IsGrabDone = new ManualResetEvent(false);
        public bool IsOpen { get; set; } = false;

        public MIL_ID MIL_System;
        public MIL_ID MIL_App;

        public MIL_ID MIL_Digitizer;
        public MIL_ID MIL_Display;
        public MIL_ID MIL_GrabBuffer;

        public MIL_INT MIL_DispAttribute = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC;
        public MIL_INT MIL_GrabAttribute = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC + MIL.M_GRAB;

        public MIL_INT MIL_Channel;
        public MIL_INT MIL_Width;
        public MIL_INT MIL_Height;

        public MIL_INT MIL_Type = 8 + MIL.M_UNSIGNED;

        public MIL_INT MIL_RowBuffer = 450;   //그랩할 로우
        public MIL_INT MIL_BufferCount = 25;  //로우 몇 세트 그랩하는지

        private bool IsAcqStart = false;

        public enum TriggerMode
        {
            Software,
            Hardware
        }

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
        public CGrabberMatrox(string RecipeName, int nIndex)
        {
            Property = new CPropertyCamera($"CAMERA_{nIndex}");
            Property.INDEX = nIndex;
            Property = Property.LoadConfig(RecipeName);

            MIL_Digitizer = MIL.M_NULL;
            MIL_Display = MIL.M_NULL;
            //MIL_GrabBuffer = MIL.M_NULL;            
        }

        public bool Init()
        {
            try
            {
                if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW)
                {
                    FILE_PATH_DCF = Application.StartupPath + "\\" + "소프트웨어.dcf";
                }
                else
                {
                    FILE_PATH_DCF = Application.StartupPath + "\\" + "하드웨어.dcf";
                }

                //ALLOC 설정
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref MIL_App);
                MIL.MappControl(MIL_App, MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

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
                            //MIL.MdigControl(MIL_Digitizer, MIL.M_CAMERALINK_CC3_SOURCE, MIL.M_GRAB_EXPOSURE);
                            //MIL.MappControl(MIL_Digitizer, MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

                            //MIL.MdigControl(MIL_Digitizer, MIL.M_GRAB_MODE, MIL.M_ASYNC);

                            MIL_Width = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_X, MIL.M_NULL);
                            MIL_Height = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_Y, MIL.M_NULL);
                            MIL_Channel = MIL.MdigInquire(MIL_Digitizer, MIL.M_SIZE_BAND, MIL.M_NULL);
                        }
                        devCount++;
                    }
                }

                if ((int)MIL_Width == 0 || (int)MIL_Height == 0)
                {
                    IsOpen = false;
                    return false;
                }

                Property.WIDTH = (int)MIL_Width;
                Property.HEIGHT = (int)MIL_Height;

                IsOpen = true;

                MIL.MdigControl(MIL_Digitizer, MIL.M_GRAB_TIMEOUT, 15000);

                if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW) { SoftwareAcqStart(); }
                else if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW) { HardwareAcqStart(); }
                //AcqStart();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                return false;
            }

            return true;
        }

        private MIL_ID[] _ImageBuffer;

        public void SoftwareAcqStart()
        {
            try
            {
                //이미지 버퍼 등록
                MIL.MbufAllocColor(MIL_System, MIL_Channel, (int)MIL_Width, (int)MIL_Height, 8, MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC, ref MIL_GrabBuffer);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void SoftwareAcqStop()
        {
            try
            {
                MIL.MdigControl(MIL_Digitizer, MIL.M_GRAB_ABORT, MIL.M_DEFAULT);
                MIL.MdigHalt(MIL_Digitizer);

                //이미지 버퍼 해제
                if (MIL_GrabBuffer != MIL.M_NULL)
                {
                    MIL.MbufFree(MIL_GrabBuffer);
                    MIL_GrabBuffer = MIL.M_NULL;
                }

            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void HardwareAcqStart()
        {
            try
            {
                //if (IsAcqStart) { return; }
                //이미지 버퍼 등록
                //MIL.MbufAllocColor(MIL_System, MIL_Channel, (int)MIL_Width, (int)MIL_Height, 8, MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC, ref MIL_GrabBuffer);

                // 배열에 버퍼 개수를 미리 할당하고 넣으면 분할해서 grab 가능
                //이미지 버퍼 배열 선언
                _ImageBuffer = new MIL_ID[1];

                //배열 수만큼 돎
                for (int i = 0; i < _ImageBuffer.Length; i++)
                {
                    MIL.MbufAllocColor(MIL_System, 1, 8190, 3000, 8, MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC, ref _ImageBuffer[i]);
                }

                _GrabCallbackFunc = OnGrab;
                
                MIL.MdigProcess(MIL_Digitizer, _ImageBuffer, _ImageBuffer.Length, MIL.M_START, MIL.M_ASYNCHRONOUS + MIL.M_TRIGGER_FOR_FIRST_GRAB, _GrabCallbackFunc, IntPtr.Zero);

                IsAcqStart = true;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void HardwareAcqStop()
        {
            try
            {
                //if (!IsAcqStart) { return; }
                MIL.MdigControl(MIL_Digitizer, MIL.M_GRAB_ABORT, MIL.M_DEFAULT);
                MIL.MdigHalt(MIL_Digitizer);

                //콜백 및 그랩 비동기 종료
                MIL.MdigProcess(MIL_Digitizer, _ImageBuffer, _ImageBuffer.Length, MIL.M_STOP, MIL.M_DEFAULT, _GrabCallbackFunc, IntPtr.Zero);

                //buffer 메모리 반환
                foreach (var buf in _ImageBuffer)
                {
                    MIL.MbufFree(buf);
                }
                _ImageBuffer = null;

                ////이미지 버퍼 해제
                //if (_ImageBuffer[0] != MIL.M_NULL)
                //{
                //    MIL.MbufFree(_ImageBuffer[0]);
                //    _ImageBuffer[0] = MIL.M_NULL;
                //}
                IsAcqStart = false;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        //비동기 그랩 콜백 메서드
        private MIL_INT OnGrab(MIL_INT eventProp, MIL_ID eventId, IntPtr userData)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
               
                //지금 콜백에 들어온 Image Buffer ID 가져오기
                MIL_ID currentBuf = MIL.M_NULL;
                MIL.MdigGetHookInfo(eventId, eventProp + MIL.M_BUFFER_ID, ref currentBuf);

                //버퍼 byte[] 로 복사
                byte[] rawImage = new byte[Property.WIDTH * Property.HEIGHT * 1];
                byte[] rawImage2 = new byte[Property.WIDTH * Property.HEIGHT * 1];
                MIL.MbufGet2d(currentBuf, 0, 0, Property.WIDTH, Property.HEIGHT, rawImage);

                

                //var img = BitmapSource.Create(Property.WIDTH, Property.HEIGHT, 96d, 96d, PixelFormats.Indexed8, BitmapPalettes.Gray256, rawImage, Property.WIDTH);
                string ss = stopwatch.ElapsedMilliseconds.ToString();
                stopwatch.Restart();
                //var image = BitmapFromSource(img);

                //Mat colorMat = Mat.FromImageData(rawImage, ImreadModes.Color);
                //Mat grayscaleMat = Mat.FromImageData(rawImage, ImreadModes.Grayscale);
                //Mat alt = Cv2.ImDecode(rawImage, ImreadModes.Grayscale);

                Mat ImageGrab = new Mat(new int[] { Property.HEIGHT, Property.WIDTH }, MatType.CV_8UC1, rawImage);

                string sss = stopwatch.ElapsedMilliseconds.ToString();
                stopwatch.Restart();
                //Mat ImageGrab = Lib.Common.CImageConverter.ToMat(image);                
                if (Property.USE_FLIP) { Cv2.Flip(ImageGrab, ImageGrab, Property.FLIP); }
                if (Property.USE_ROTATE) { Cv2.Rotate(ImageGrab, ImageGrab, Property.ROTATE); }
                
                    string ssss = stopwatch.ElapsedMilliseconds.ToString();

                return 0;
            }
            catch (Exception err)
            {
                return -1;
            }
        }

        public Mat Rotate(Mat src, double angle)
        {
            Mat rotate = new Mat(src.Size(), MatType.CV_8UC3);
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), angle, 1);
            Cv2.WarpAffine(src, rotate, matrix, src.Size(), InterpolationFlags.Linear);
            return rotate;
        }

        private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
         }

        public void Close()
        {
            try
            {
                Live(false);                
                if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_SW) { SoftwareAcqStop(); }
                else if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW) { HardwareAcqStop(); }

                if (MIL_Digitizer != MIL.M_NULL)
                {
                    MIL.MbufFree(MIL_Digitizer);
                    MIL_Digitizer = MIL.M_NULL;
                }

                if (MIL_System != MIL.M_NULL)
                {
                    MIL.MbufFree(MIL_System);
                    MIL_System = MIL.M_NULL;
                }

                IsOpen = false;
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }


        public void Grab()
        {
            try
            {
                if (Property.TRIGGER_MODE == CPropertyCamera.TRIGGER_MODE_TYPE.ON_HW) { return; }
                if (IsOpen)
                {                    
                    //Thread 로 구성해야될 필요가 있습니다.
                    IsGrabDone.Reset();
                    byte[] rawImage = new byte[(int)MIL_Width * (int)MIL_Height * 1];

                    MIL.MdigGrab(MIL_Digitizer, MIL_GrabBuffer);
                    MIL.MbufGet2d(MIL_GrabBuffer, (int)0, (int)0, (int)MIL_Width, (int)MIL_Height, rawImage);

                    var img = BitmapSource.Create(Property.WIDTH, Property.HEIGHT, 96d, 96d, PixelFormats.Indexed8, BitmapPalettes.Gray256, rawImage, Property.WIDTH);
                    
                    var image = BitmapFromSource(img);
                                        
                    IsGrabDone.Set();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public Bitmap ByteToBitmap(byte[] imgArr, int nW, int nH)
        {
            Bitmap bmp = new Bitmap(nW, nH, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            // IntPtr ptr = res.GetHbitmap();

            BitmapData data = bmp.LockBits(
                                    new Rectangle(0, 0, nW, nH),
                                    ImageLockMode.ReadWrite,
                                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            IntPtr ptr = data.Scan0;
            Marshal.Copy(imgArr, 0, ptr, imgArr.Length);
            bmp.UnlockBits(data);

            //모노이미지로 변환해준다 사용하지 않을경우 칼라이미지가 깨진채로 사용된다
            ColorPalette Gpal = bmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                Gpal.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            }
            bmp.Palette = Gpal;

            return bmp;
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

                    Grab();

                    IsGrabDone.WaitOne();
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }

        }
        #endregion
    }
}