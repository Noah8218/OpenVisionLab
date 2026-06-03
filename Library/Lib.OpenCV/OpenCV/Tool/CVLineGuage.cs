using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Lib.Common;
using Lib.Line;
using Lib.OpenCV.Property;
using Lib.OpenCV.Result;
using OpenCvSharp;
using static Lib.Common.CFormula;
namespace Lib.OpenCV.Tool
{  
    public partial class CVLineGuage : COpenCVAlgorithmBase
    {
        public IOpenCVPropertyLineGuage property { get; set; }
        public List<CVLineGuage_Result> resultList { get; set; } = new List<CVLineGuage_Result>();
        public CVLineGuage() { }

        public void SetProperty(IOpenCVPropertyLineGuage property) => this.property = property;

        public override void Run()
        {
            if (property.USE_MULTI_ROI)
            {
                MultiRun();                
            }
            else
            {
                SingleRun();
            }
        }

        public unsafe bool SingleRun()
        {            
            List<CVLineGuage_Edge> edgeList  = new List<CVLineGuage_Edge>();            

            Mat imageSrc = new Mat();

            if (COpenCVHelper.IsImageEmpty(imageSource))
            {
                CLOG.ABNORMAL("Image is Empty");
                return false;
            }

            if (property.CvROI.Width == 0 || property.CvROI.Height == 0)
            {
                CLOG.ABNORMAL("ROI is Empty");
                return false;
            }

            imageSrc = property.USE_ROI ? imageSource.SubMat(property.CvROI) : imageSource.Clone();
            COpenCVHelper.SetImageChannel1(imageSrc);

            #region THRESHOLD
            if (property.USE_THRESHOLD) { Cv2.Threshold(imageSrc, imageSrc, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
            else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(imageSrc, imageSrc, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }
            #endregion

            Stopwatch sw_TaktTimems = Stopwatch.StartNew();

            ConcurrentBag<CVLineGuage_Edge> edgeS = new ConcurrentBag<CVLineGuage_Edge>();

            // 포인터에 직접 접근해서 값을 미리 한번에 읽어놓음
            byte[,] bytes = new byte[imageSrc.Rows, imageSrc.Cols];
            Parallel.For(0, imageSrc.Rows,
               i =>
               {
                   byte* ptr = (byte*)imageSrc.Ptr(i).ToPointer();
                   byte[] arr = new byte[imageSrc.Cols];
                   Marshal.Copy((IntPtr)ptr, arr, 0, imageSrc.Cols);

                   for (int j = 0; j < arr.Length; j++)
                   {
                       bytes[i, j] = arr[j];
                   }
               });

            int Step = (int)property.SAMPLING_STEP;
            switch (property.PRJ_DIR)
            {
                case PROJECTION_DIR.X_LTOR:
                case PROJECTION_DIR.X_RTOL:

                    Parallel.For(0, imageSrc.Rows,
                     ny =>
                     {
                         if (ny % Step == 0)
                         {
                             if (property.PRJ_DIR == PROJECTION_DIR.X_LTOR)
                             {
                                 for (int nx = 0; nx < imageSrc.Cols; nx++)
                                 {
                                     if (nx > 1)
                                     {
                                         int nGV_Curr = bytes[ny, nx];
                                         int nGV_Prev = bytes[ny, nx - 1];

                                         if (nx + property.THICKNESS < imageSrc.Cols)
                                         {
                                             bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                             if (bEdge)
                                             {
                                                 bool bThickness = true;

                                                 for (int k = 1; k <= property.THICKNESS; k++)
                                                 {
                                                     // L -> R 방향이므로 X + K
                                                     if (bThickness)
                                                     {
                                                         int nGV_T = bytes[ny, nx + (k + 1)];
                                                         bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                         if (!bEdge_T)
                                                         {
                                                             bThickness = false;
                                                             break;
                                                         }
                                                     }
                                                 }

                                                 if (bThickness)
                                                 {
                                                     // 검출된건 이전포인트로
                                                     if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx - 1) + property.CvROI.X, ny + property.CvROI.Y))); }
                                                     else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx - 1), ny))); }
                                                     break;
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                             else if (property.PRJ_DIR == PROJECTION_DIR.X_RTOL)
                             {
                                 for (int nx = imageSrc.Cols - 1; nx > 0; nx--)
                                 {
                                     if (nx < imageSrc.Cols - 1)
                                     {
                                         int nGV_Curr = bytes[ny, nx];
                                         int nGV_Prev = bytes[ny, nx + 1];

                                         if (nx - property.THICKNESS > 0)
                                         {
                                             bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);
                                             if (bEdge)
                                             {
                                                 bool bThickness = true;

                                                 for (int k = 1; k <= property.THICKNESS; k++)
                                                 {
                                                     //int nGV_T = ImageSrc.At<byte>(ny, nx - k);
                                                     int nGV_T = bytes[ny, nx - k];
                                                     bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);

                                                     if (!bEdge_T)
                                                     {
                                                         bThickness = false;
                                                         break;
                                                     }
                                                 }

                                                 if (bThickness)
                                                 {
                                                     if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx + 1) + property.CvROI.X, ny + property.CvROI.Y))); }
                                                     else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx + 1), ny))); }
                                                         
                                                     break;
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     });
                    
                    if (edgeS.Count > 2)
                    {
                        //Results.RemoveAll(s => s == null);
                        edgeList = edgeS.OrderBy(x => x.MeasPos.Y).ToList();
                    }
                    break;
                case PROJECTION_DIR.Y_TTOB:
                case PROJECTION_DIR.Y_BTOT:
                    Parallel.For(0, imageSrc.Cols,
                    nx =>
                    {
                        if (nx % Step == 0)
                        {
                            if (property.PRJ_DIR == PROJECTION_DIR.Y_TTOB)
                            {
                                for (int ny = 0, cy = imageSrc.Rows; ny < cy; ny++)
                                {
                                    if (ny > 1)
                                    {
                                        int nGV_Curr = bytes[ny, nx];
                                        int nGV_Prev = bytes[ny - 1, nx];

                                        if (ny + property.THICKNESS < imageSrc.Rows)
                                        {
                                            bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                            if (bEdge)
                                            {
                                                bool bThickness = true;

                                                Parallel.For(1, (int)property.THICKNESS,
                                                k =>
                                                {
                                                    if (bThickness)
                                                    {
                                                        int nGV_T = bytes[ny + k, nx];
                                                        bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                        if (!bEdge_T)
                                                        {
                                                            bThickness = false;
                                                            //break;
                                                        }
                                                    }
                                                });

                                                if (bThickness)
                                                {
                                                    if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx + property.CvROI.X, (ny - 1) + property.CvROI.Y))); }
                                                    else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx, (ny - 1)))); }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (property.PRJ_DIR == PROJECTION_DIR.Y_BTOT)
                            {
                                for (int ny = imageSrc.Rows - 1; ny > 0; ny--)
                                {
                                    if (ny < imageSrc.Rows - 1)
                                    {
                                        int nGV_Curr = bytes[ny, nx];
                                        int nGV_Prev = bytes[ny + 1, nx];

                                        if (ny - property.THICKNESS > 0)
                                        {
                                            bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                            if (bEdge)
                                            {
                                                bool bThickness = true;

                                                Parallel.For(1, (int)property.THICKNESS,
                                                k =>
                                                {
                                                    if (bThickness)
                                                    {
                                                        int nGV_T = bytes[ny - k, nx];
                                                        //int nGV_T = ImageSrc.At<byte>(ny - k, nx);
                                                        bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                        if (!bEdge_T)
                                                        {
                                                            bThickness = false;
                                                        }
                                                    }
                                                });

                                                if (bThickness)
                                                {
                                                    if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx + property.CvROI.X, (ny + 1) + property.CvROI.Y))); }
                                                    else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx, (ny + 1)))); }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    if (edgeS.Count > 2)
                    {
                        
                        //Results = edgeS.ToList().RemoveAll(s => s == null);
                        edgeList = edgeS.OrderBy(x => x.MeasPos.X).ToList();
                    }
                    break;
            }

            var fitLine = property.USE_EXTEND_FIT_LINE ? CLineFitting.GetFitLineExtend(edgeList.ConvertAll(s => s.MeasPos).ToList(), property.EXTEND_FIT_LINE_VALUE, property.PRJ_DIR) :  CLineFitting.GetFitLine(edgeList.ConvertAll(s => s.MeasPos).ToList(), property.PRJ_DIR);
            resultList.Add(new CVLineGuage_Result(edgeList.ConvertAll(s => s), fitLine));            
            return true;
        }

        public unsafe bool MultiRun()
        {
            List<CVLineGuage_Edge> edgeList = new List<CVLineGuage_Edge>();

            Mat imageSrc = new Mat();

            if (COpenCVHelper.IsImageEmpty(imageSource))
            {
                CLOG.ABNORMAL("Image is Empty");
                return false;
            }

            for(int index = 0; index < property.CvROIS.Count; index++)
            {
                if (property.CvROIS[index].Width == 0 || property.CvROIS[index].Height == 0)
                {
                    CLOG.ABNORMAL("ROI is Empty");
                    return false;
                }

                imageSrc = imageSource.SubMat(property.CvROIS[index]);

                COpenCVHelper.SetImageChannel1(imageSrc);

                #region THRESHOLD
                if (property.USE_THRESHOLD) { Cv2.Threshold(imageSrc, imageSrc, property.THRESHOLD, 255, property.THRESHOLD_TYPES); }
                else if (property.USE_ADAPTIVE_THRESHOLD) { Cv2.AdaptiveThreshold(imageSrc, imageSrc, property.ADAPTIVE_THRESHOLD, property.ADAPTIVE_THRESHOLD_ALGORITHM, property.ADAPTIVE_THRESHOLD_TYPES, property.BlockSize, property.Weight); }
                #endregion

                Stopwatch sw_TaktTimems = Stopwatch.StartNew();

                ConcurrentBag<CVLineGuage_Edge> edgeS = new ConcurrentBag<CVLineGuage_Edge>();

                // 포인터에 직접 접근해서 값을 미리 한번에 읽어놓음
                byte[,] bytes = new byte[imageSrc.Rows, imageSrc.Cols];
                Parallel.For(0, imageSrc.Rows,
                   i =>
                   {
                       byte* ptr = (byte*)imageSrc.Ptr(i).ToPointer();
                       byte[] arr = new byte[imageSrc.Cols];
                       Marshal.Copy((IntPtr)ptr, arr, 0, imageSrc.Cols);

                       for (int j = 0; j < arr.Length; j++)
                       {
                           bytes[i, j] = arr[j];
                       }
                   });

                int Step = (int)property.SAMPLING_STEP;
                switch (property.PRJ_DIR)
                {
                    case PROJECTION_DIR.X_LTOR:
                    case PROJECTION_DIR.X_RTOL:

                        Parallel.For(0, imageSrc.Rows,
                         ny =>
                         {
                             if (ny % Step == 0)
                             {
                                 if (property.PRJ_DIR == PROJECTION_DIR.X_LTOR)
                                 {
                                     for (int nx = 0; nx < imageSrc.Cols; nx++)
                                     {
                                         if (nx > 1)
                                         {
                                             int nGV_Curr = bytes[ny, nx];
                                             int nGV_Prev = bytes[ny, nx - 1];

                                             if (nx + property.THICKNESS < imageSrc.Cols)
                                             {
                                                 bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                                 if (bEdge)
                                                 {
                                                     bool bThickness = true;

                                                     for (int k = 1; k <= property.THICKNESS; k++)
                                                     {
                                                         // L -> R 방향이므로 X + K
                                                         if (bThickness)
                                                         {
                                                             int nGV_T = bytes[ny, nx + (k + 1)];
                                                             bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                             if (!bEdge_T)
                                                             {
                                                                 bThickness = false;
                                                                 break;
                                                             }
                                                         }
                                                     }

                                                     if (bThickness)
                                                     {
                                                         // 검출된건 이전포인트로
                                                         if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx - 1) + property.CvROIS[index].X, ny + property.CvROIS[index].Y))); }
                                                         else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx - 1), ny))); }
                                                         break;
                                                     }
                                                 }
                                             }
                                         }
                                     }
                                 }
                                 else if (property.PRJ_DIR == PROJECTION_DIR.X_RTOL)
                                 {
                                     for (int nx = imageSrc.Cols - 1; nx > 0; nx--)
                                     {
                                         if (nx < imageSrc.Cols - 1)
                                         {
                                             int nGV_Curr = bytes[ny, nx];
                                             int nGV_Prev = bytes[ny, nx + 1];

                                             if (nx - property.THICKNESS > 0)
                                             {
                                                 bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);
                                                 if (bEdge)
                                                 {
                                                     bool bThickness = true;

                                                     for (int k = 1; k <= property.THICKNESS; k++)
                                                     {
                                                         //int nGV_T = ImageSrc.At<byte>(ny, nx - k);
                                                         int nGV_T = bytes[ny, nx - k];
                                                         bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);

                                                         if (!bEdge_T)
                                                         {
                                                             bThickness = false;
                                                             break;
                                                         }
                                                     }

                                                     if (bThickness)
                                                     {
                                                         if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx + 1) + property.CvROIS[index].X, ny + property.CvROIS[index].Y))); }
                                                         else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point((nx + 1), ny))); }

                                                         break;
                                                     }
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                         });

                        if (edgeS.Count > 2)
                        {
                            //Results.RemoveAll(s => s == null);
                            edgeList = edgeS.OrderBy(x => x.MeasPos.Y).ToList();
                        }
                        break;
                    case PROJECTION_DIR.Y_TTOB:
                    case PROJECTION_DIR.Y_BTOT:
                        Parallel.For(0, imageSrc.Cols,
                        nx =>
                        {
                            if (nx % Step == 0)
                            {
                                if (property.PRJ_DIR == PROJECTION_DIR.Y_TTOB)
                                {
                                    for (int ny = 0, cy = imageSrc.Rows; ny < cy; ny++)
                                    {
                                        if (ny > 1)
                                        {
                                            int nGV_Curr = bytes[ny, nx];
                                            int nGV_Prev = bytes[ny - 1, nx];

                                            if (ny + property.THICKNESS < imageSrc.Rows)
                                            {
                                                bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                                if (bEdge)
                                                {
                                                    bool bThickness = true;

                                                    Parallel.For(1, (int)property.THICKNESS,
                                                    k =>
                                                    {
                                                        if (bThickness)
                                                        {
                                                            int nGV_T = bytes[ny + k, nx];
                                                            bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                            if (!bEdge_T)
                                                            {
                                                                bThickness = false;
                                                                //break;
                                                            }
                                                        }
                                                    });

                                                    if (bThickness)
                                                    {
                                                        if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx + property.CvROIS[index].X, (ny - 1) + property.CvROIS[index].Y))); }
                                                        else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx, (ny - 1)))); }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (property.PRJ_DIR == PROJECTION_DIR.Y_BTOT)
                                {
                                    for (int ny = imageSrc.Rows - 1; ny > 0; ny--)
                                    {
                                        if (ny < imageSrc.Rows - 1)
                                        {
                                            int nGV_Curr = bytes[ny, nx];
                                            int nGV_Prev = bytes[ny + 1, nx];

                                            if (ny - property.THICKNESS > 0)
                                            {
                                                bool bEdge = JudgeEdge(nGV_Prev, nGV_Curr);

                                                if (bEdge)
                                                {
                                                    bool bThickness = true;

                                                    Parallel.For(1, (int)property.THICKNESS,
                                                    k =>
                                                    {
                                                        if (bThickness)
                                                        {
                                                            int nGV_T = bytes[ny - k, nx];
                                                            //int nGV_T = ImageSrc.At<byte>(ny - k, nx);
                                                            bool bEdge_T = JudgeEdge(nGV_Prev, nGV_T);
                                                            if (!bEdge_T)
                                                            {
                                                                bThickness = false;
                                                            }
                                                        }
                                                    });

                                                    if (bThickness)
                                                    {
                                                        if (property.USE_ROI) { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx + property.CvROIS[index].X, (ny + 1) + property.CvROIS[index].Y))); }
                                                        else { edgeS.Add(new CVLineGuage_Edge(new OpenCvSharp.Point(nx, (ny + 1)))); }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });

                        if (edgeS.Count > 2)
                        {

                            //Results = edgeS.ToList().RemoveAll(s => s == null);
                            edgeList = edgeS.OrderBy(x => x.MeasPos.X).ToList();
                        }
                        break;
                }

                resultList.Add(new CVLineGuage_Result(edgeList.ConvertAll(s => s), CLineFitting.GetFitLine(edgeList.ConvertAll(s => s.MeasPos).ToList(), property.PRJ_DIR)));
            }            
            return true;
        }

        private bool JudgeEdge(double nGV_Prev, double nGV_Curr)
        {
            bool bEdge = false;
            if (property.PRJ_PORALITY == PROJECTION_POLARITY.ALL)
            {
                if (Math.Abs(nGV_Prev - nGV_Curr) > property.CONTRAST) { bEdge = true; }
            }
            else if (property.PRJ_PORALITY == PROJECTION_POLARITY.BTOW)
            {
                if (-(nGV_Prev - nGV_Curr) > property.CONTRAST) { bEdge = true; }
            }
            else if (property.PRJ_PORALITY == PROJECTION_POLARITY.WTOB)
            {
                if ((nGV_Prev - nGV_Curr) > property.CONTRAST) { bEdge = true; }
            }
            return bEdge;
        }
    }    
}
