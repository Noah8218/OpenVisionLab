using KtemVisionSystem._1._Core;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KtemVisionSystem
{
    public class CSeqVision
    {
        public EventHandler<InspResultArgs> EventInspResult;

        #region Thread
        public CThreadStatus ThreadStatusVision = new CThreadStatus();

        public void StartThreadVision()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadVision));
            t.Start(ThreadStatusVision);
        }

        public void ResetThreadVision()
        {
            ThreadStatusVision.End();
        }

        public void StopThreadVision()
        {
            if (!ThreadStatusVision.IsExit())
            {
                ThreadStatusVision.Stop(100);
            }

            ResetThreadVision();
        }

        /// <summary>
        /// 프로그램이 종료가 될때까지 실행되며, 큐에 있는 데이터를 비전검사합니다.
        /// </summary>
        /// <param name="ob"></param>
        private void ThreadVision(object ob)
        {
            CThreadStatus ThreadStatus = (CThreadStatus)ob;

            ThreadStatus.Start("Vision Inspection");
            CLOG.NORMAL( "Vision Inspection");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    if (CGlobal.Inst.Data.GrabQueue.Count > 0)
                    {
                        int START_TIME = Environment.TickCount;

                        if (CGlobal.Inst.Data.GrabQueue.TryDequeue(out CGrabBuffer GrabBuffer))
                        {
                            RunInspection(GrabBuffer);
                        }
                    }
                    Thread.Sleep(5);
                }
                ThreadStatus.End();
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ThreadStatus.End();
            }
        }

        public async void RunInspection(CGrabBuffer GrabBuffer)
        {
            var taskInspection = Task.Run(() => Inspection(GrabBuffer));
            await taskInspection;
        }

        object m_ob = new object();

        public void Inspection(CGrabBuffer GrabBuffer)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (Mat ImageCVSource = GrabBuffer.ImageGrab)
                {
                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = CConverter.ToBitmap(ImageCVSource);
                    Bitmap ImageDisplay = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();
                    DEFINE.RESULT Result = DEFINE.RESULT.OK;

                    Graphics g = Graphics.FromImage(ImageDisplay);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    int W = ImageCVSource.Width;
                    int H = ImageCVSource.Height;

                    CIVT_CVBlob cIVT_CVBlob = new CIVT_CVBlob("BLOB");
                    cIVT_CVBlob.SetProperty(CVisionTools.Blobs[CDisplayManager.CameraIndex]);
                    cIVT_CVBlob.SetSourceImage(ImageCVSource);
                    cIVT_CVBlob.Run();

                    List<Line> ver_Line_L = new List<Line>();
                    List<Line> ver_Line_R = new List<Line>();
                    List<Line> fit_Lines_L = new List<Line>();
                    List<Line> fit_Lines_R = new List<Line>();

                    List<OpenCvSharp.Point> Edges_L = new List<OpenCvSharp.Point>();
                    List<OpenCvSharp.Point> Edges_R = new List<OpenCvSharp.Point>();
                    for (int i = 0; i < 2; i++)
                    {
                        CIVT_LineGuage LineGuage_EDGE = new CIVT_LineGuage();

                        if (i == 0) { LineGuage_EDGE.SetProperty(CVisionTools.Lines_L[CDisplayManager.CameraIndex]); }
                        else { LineGuage_EDGE.SetProperty(CVisionTools.Lines_R[CDisplayManager.CameraIndex]); }
                        LineGuage_EDGE.SetSourceImage(ImageCVSource);
                        LineGuage_EDGE.Run();

                        if (i == 0)
                        {
                            LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_L, true);
                            LineGuage_EDGE.DrawEdge(g, out Edges_L);
                            LineGuage_EDGE.DrawVerticalLine(Edges_L, W, H, g, out ver_Line_L);

                            for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                            {
                                LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                            }
                        }
                        else
                        {
                            LineGuage_EDGE.DrawFitLine(g, out double T, out fit_Lines_R, true);
                            LineGuage_EDGE.DrawEdge(g, out Edges_R);
                            LineGuage_EDGE.DrawVerticalLine(Edges_R, W, H, g, out ver_Line_R);

                            for (int j = 0; j < LineGuage_EDGE.Results.Count; j++)
                            {
                                LineGuage_EDGE.Results[j].MeasPos = new System.Drawing.Point(LineGuage_EDGE.Results[j].MeasPos.X + LineGuage_EDGE.Property.CVROI.X, LineGuage_EDGE.Results[j].MeasPos.Y + LineGuage_EDGE.Property.CVROI.Y);
                            }
                        }
                    }


                    List<PointF> Points_R = new List<PointF>();
                    for (int i = 0; i < Edges_R.Count; i++)
                    {
                        PointF pointF = new PointF();
                        Points_R.Add(pointF = new PointF(Edges_R[i].X, Edges_R[i].Y));
                    }

                    for (int i = 0; i < ver_Line_L.Count; i++)
                    {
                        Line verLine = ver_Line_L[i];

                        bool draw = true;

                        PointF start = new PointF(ver_Line_L[i].Start.X, ver_Line_L[i].Start.Y);
                        PointF end = new PointF(ver_Line_L[i].End.X, ver_Line_L[i].End.Y);

                        if (Points_R.Count == 0) { break; }

                        PointF[] intersectionPointArray = CVision.GetClippingPointArray
                        (
                            out draw,
                            start,
                            end,
                            Points_R
                        );

                        using (Pen pen = new Pen(Color.Green, 5))
                        {
                            for (int h = 0; h < intersectionPointArray.Length - 1; h++)
                            {
                                OpenCvSharp.Point ptS = new OpenCvSharp.Point(intersectionPointArray[h].X, intersectionPointArray[h].Y);
                                OpenCvSharp.Point ptE = new OpenCvSharp.Point(intersectionPointArray[h + 1].X, intersectionPointArray[h + 1].Y);

                                double Distance = ptS.DistanceTo(ptE) * DEFINE.PIXEL_RESOLUTION_MM;
                                g.DrawString(Distance.ToString("F1") + "mm", new Font("Arial", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), intersectionPointArray[h + 1].X + 5, intersectionPointArray[h + 1].Y);

                                //g.DrawLine(pen, intersectionPointArray[h], intersectionPointArray[h + 1]);
                            }

                            foreach (PointF point in intersectionPointArray)
                            {
                                DrawPoint(g, point);
                            }
                        }
                    }

                    int Count = 0;

                    List<CResultBlob> TotalResults = new List<CResultBlob>();

                    foreach (var item in cIVT_CVBlob.Results)
                    {
                        bool Masking = false;
                        for (int i = 0; i < cIVT_CVBlob.Property.CV_MASKS.Count; i++)
                        {
                            if (cIVT_CVBlob.Property.CV_MASKS[i].IntersectsWith(CConverter.RectToCVRect(item.Bounding)))
                            {
                                Masking = true;
                                break;
                            }
                        }
                        if (!Masking)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                            g.DrawString((Count + 1).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10);
                            g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                            Count++;
                            TotalResults.Add(item);
                        }
                    }

                    if (cIVT_CVBlob.Results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVBlob.Property.CVROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVBlob.Property.CVROI.X - 20, (int)cIVT_CVBlob.Property.CVROI.Y - 20);
                    }

                    string saveImagePathOri = "";
                    string saveImagePathInsp = "";

                    Rectangle rtFullScreen = new Rectangle(0, 0, ImageDisplay.Width, ImageDisplay.Height);
                    
                    if (Result == DEFINE.RESULT.OK)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Aquamarine, 30), rtFullScreen);
                        g.DrawString(string.Format("OK"), new Font("Arial", 250, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Aquamarine), ImageDisplay.Width - 700, ImageDisplay.Height - 500);

                        saveImagePathOri = CUtil.GetPathOK_Ori() + $"Ori_Cam{(GrabBuffer.Index + 1)}_{DateTime.Now.ToString("yyMMdd_HHmmss")}_OK.jpeg";
                        saveImagePathInsp = CUtil.GetPathOK_Insp() + $"Insp_Cam{(GrabBuffer.Index + 1)}_{DateTime.Now.ToString("yyMMdd_HHmmss")}_OK.jpeg";
                        CImageManager.SaveImages.Queues.Enqueue(new Images(CConverter.ToBitmap(GrabBuffer.ImageGrab.Clone()), saveImagePathOri, ImageFormat.Jpeg));
                        CImageManager.SaveImages.Queues.Enqueue(new Images((Bitmap)ImageDisplay.Clone(), saveImagePathInsp, ImageFormat.Jpeg));
                    }
                    else
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 30), rtFullScreen);
                        g.DrawString(string.Format("NG"), new Font("Arial", 250, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Red), ImageDisplay.Width - 700, ImageDisplay.Height - 500);

                        saveImagePathOri = CUtil.GetPathNG_Ori() + $"Ori_Cam{(GrabBuffer.Index + 1)}_{DateTime.Now.ToString("yyMMdd_HHmmss")}_NG.jpeg";
                        saveImagePathInsp = CUtil.GetPathNG_Insp() + $"Insp_Cam{(GrabBuffer.Index + 1)}_{DateTime.Now.ToString("yyMMdd_HHmmss")}_NG.jpeg";
                        CImageManager.SaveImages.Queues.Enqueue(new Images(CConverter.ToBitmap(GrabBuffer.ImageGrab.Clone()), saveImagePathOri, ImageFormat.Jpeg));
                        CImageManager.SaveImages.Queues.Enqueue(new Images((Bitmap)ImageDisplay.Clone(), saveImagePathInsp, ImageFormat.Jpeg));
                    }

                    if (CGlobal.Inst.SeqVision.EventInspResult != null)
                    {
                        CGlobal.Inst.SeqVision.EventInspResult(null, new InspResultArgs(ImageDisplay, GrabBuffer.Index, stopwatch.ElapsedMilliseconds, Result));
                    }
                }
            }
            catch (Exception ex)
            {
                CLOG.ABNORMAL( "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void DrawPoint(Graphics graphics, PointF point)
        {
            RectangleF rectangle = new RectangleF(point.X - 3, point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        private DEFINE.RESULT InspectionJudge(double Dist, double Thickness, bool FindLength)
        {
            DEFINE.RESULT rESULT = DEFINE.RESULT.OK;

            // 길이를 못찾으면 두께만 판정
            if (!FindLength)
            {
                if (CGlobal.Inst.Data.SPEC.THICKNESS_MIN_MM > Thickness || CGlobal.Inst.Data.SPEC.THICKNESS_MAX_MM < Thickness || Thickness == 0) { rESULT = DEFINE.RESULT.T_NG;}
                else { rESULT = DEFINE.RESULT.OK; }
            }
            else if (CGlobal.Inst.Data.SPEC.DIST_MIN_MM > Dist || CGlobal.Inst.Data.SPEC.DIST_MAX_MM < Dist || Dist == 0)
            {
                rESULT = DEFINE.RESULT.L_NG;
            }
            else if (CGlobal.Inst.Data.SPEC.THICKNESS_MIN_MM > Thickness || CGlobal.Inst.Data.SPEC.THICKNESS_MAX_MM < Thickness || Thickness == 0)
            {
                rESULT = DEFINE.RESULT.T_NG;
            } 
            else { rESULT = DEFINE.RESULT.OK; }

            return rESULT;
        }

        #endregion
    }
}
