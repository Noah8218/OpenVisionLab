using Accord.MachineLearning;
using Accord.Math;
using Accord.Math.Distances;
using OpenVisionLab._1._Core;
using OpenVisionLab._2._Common;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using Lib.Common;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using RJCodeUI_M1.RJControls;
using Sunny.UI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Windows.Controls.WpfPropertyGrid.KnownTypes;

namespace OpenVisionLab
{
    public partial class FormSystem : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private CGlobal Global = CGlobal.Inst;
        private int PanelCount = 0;
        public FormSystem()
        {
            InitializeComponent();

            CloseButton = false;
            CloseButtonVisible = false;
        }

        // If the content won't display nicely, hide it
        private void ResizeEvent(object sender, EventArgs e)
        {
            this.Visible = this.Width > this.MinimumSize.Width && this.Height > this.MinimumSize.Height;
        }

        private bool ChangeSize = false;

        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!ChangeSize)
            {
                if (DockHandler.FloatPane == null) { return; }
                DockHandler.FloatPane.FloatWindow.Bounds = new Rectangle(DockHandler.FloatPane.FloatWindow.Bounds.X, DockHandler.FloatPane.FloatWindow.Bounds.Y, 800, 400);
                this.Refresh();
                ChangeSize = true;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            //tbExposure.Text = Global.Device.CAMERAS[DEFINE.CAM_1].Property.EXPOSURETIME_US.ToString();
            //tbGain.Text = Global.Device.CAMERAS[DEFINE.CAM_1].Property.GAIN.ToString();
            CDisplayManager.EventUpdateCam += OnCamUpdate;

            dgvResult1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvResult1.ColumnHeadersVisible = false;
            dgvResult1.DataSource = new CDefectList_Result().GetBlobList(Results_bef);
            dgvResult1.ColumnHeadersVisible = true;

            dgvResult2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvResult2.ColumnHeadersVisible = false;
            dgvResult2.DataSource = new CDefectList_Result().GetBlobList(Results_afe);
            dgvResult2.ColumnHeadersVisible = true;
        }

        private void OnCamUpdate(object sender, EventArgs e)
        {
            this.UIThreadBeginInvoke(() =>
            {
                //tbExposure.Text = Global.Device.CAMERAS[CDisplayManager.CameraIndex].Property.EXPOSURETIME_US.ToString();
                //tbGain.Text = Global.Device.CAMERAS[CDisplayManager.CameraIndex].Property.GAIN.ToString();
            });
        }


        private void tbPress_KeyPress(object sender, KeyPressEventArgs e) => CUtil_UI.txtInterval_KeyPress(sender, e);

        private void tbGain_MouseLeave(object sender, EventArgs e)
        {

        }

        private void OnClickCameraOperation(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is RJCodeUI_M1.RJControls.RJButton)) return;

                string strIndex = (sender as RJCodeUI_M1.RJControls.RJButton).Text;

                switch (strIndex)
                {
                    case DEFINE.Grab:
                        if (!Global.Device.CAMERAS[CDisplayManager.CameraIndex].IsOpen) return;
                        Global.Device.CAMERAS[CDisplayManager.CameraIndex].Grab();
                        CDisplayManager.Displays[DEFINE.Main].Activate();
                        CDisplayManager.Displays[DEFINE.Main].viewer._Ib.ZoomToFit();
                        break;
                    case DEFINE.Live:
                        if (!Global.Device.CAMERAS[CDisplayManager.CameraIndex].IsOpen) return;
                        (sender as RJCodeUI_M1.RJControls.RJButton).Text = "LIVE STOP";
                        Global.Device.CAMERAS[CDisplayManager.CameraIndex].Live(true);
                        CCommon.SetButtonRed((sender as RJButton));
                        CDisplayManager.Displays[DEFINE.Main].Activate();
                        CDisplayManager.Displays[DEFINE.Main].viewer._Ib.ZoomToFit();
                        break;
                    case DEFINE.Live_Stop:
                        if (!Global.Device.CAMERAS[CDisplayManager.CameraIndex].IsOpen) return;
                        (sender as RJCodeUI_M1.RJControls.RJButton).Text = "LIVE";
                        Global.Device.CAMERAS[CDisplayManager.CameraIndex].Live(false);
                        CCommon.SetButtonBlue((sender as RJButton));
                        break;
                }

                CLOG.NORMAL($"[OK] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}");
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSaveVisionParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (CCommon.ShowdialogMessageBox("Save", "현재까지 설정된 값을 저장하시겠습니까?", FormMessageBox.MESSAGEBOX_TYPE.Quit))
                {
                    CGlobal.Inst.Data.SPEC.SaveConfig(Global.Recipe.Name);
                    for (int i = 0; i < Global.Device.CAMERA_COUNT; i++)
                    {
                        Global.Device.CAMERAS[i].Property.SaveConfig(Global.Recipe.Name);
                    }
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            try
            {

                //int Gain = int.Parse(tbGain.Text);
                //int Exposure = int.Parse(tbExposure.Text);
                //Global.Device.CAMERAS[CDisplayManager.CameraIndex].Property.GAIN = Gain;
                //Global.Device.CAMERAS[CDisplayManager.CameraIndex].Property.EXPOSURETIME_US = Exposure;
                Global.Device.CAMERAS[CDisplayManager.CameraIndex].Property.SaveConfig(Global.Recipe.Name);

                //Global.Device.CAMERAS[CDisplayManager.CameraIndex].SetGain(Gain);
                //Global.Device.CAMERAS[CDisplayManager.CameraIndex].SetExposure(Exposure);
            }
            catch (Exception Desc) { CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}"); }
        }

        public static System.Drawing.PointF[] ConvertPointToPoint2f(OpenCvSharp.Point[] points)
        {
            System.Drawing.PointF[] point2fs = new System.Drawing.PointF[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                point2fs[i] = new System.Drawing.PointF(points[i].X, points[i].Y);
            }

            return point2fs;
        }

        public static OpenCvSharp.Point[] ExpandContour(OpenCvSharp.Point[] contour, int offsetX, int offsetY)
        {
            OpenCvSharp.Point[] expandedContour = new OpenCvSharp.Point[contour.Length];

            for (int i = 0; i < contour.Length; i++)
            {
                expandedContour[i] = new OpenCvSharp.Point(contour[i].X + offsetX, contour[i].Y + offsetY);
            }

            return expandedContour;
        }

        public static OpenCvSharp.Point[] ExpandContourAroundCenter(OpenCvSharp.Point[] contour, OpenCvSharp.Point2d center, int offset)
        {
            OpenCvSharp.Point[] expandedContour = new OpenCvSharp.Point[contour.Length];

            for (int i = 0; i < contour.Length; i++)
            {
                double angle = Math.Atan2(contour[i].Y - center.Y, contour[i].X - center.X);
                int offsetX = (int)(Math.Cos(angle) * offset);
                int offsetY = (int)(Math.Sin(angle) * offset);
                expandedContour[i] = new OpenCvSharp.Point(contour[i].X + offsetX, contour[i].Y + offsetY);
            }

            return expandedContour;
        }

        public static OpenCvSharp.Point CalculateCenter(OpenCvSharp.Point[] points)
        {
            int sumX = 0;
            int sumY = 0;

            foreach (OpenCvSharp.Point point in points)
            {
                sumX += point.X;
                sumY += point.Y;
            }

            int centerX = sumX / points.Length;
            int centerY = sumY / points.Length;

            return new OpenCvSharp.Point(centerX, centerY);
        }

        public List<CResultBlob> GetIntersectingBlobs(OpenCvSharp.Point[] contour, List<CResultBlob> blobs)
        {
            ConcurrentBag<CResultBlob> intersectingBlobs = new ConcurrentBag<CResultBlob>();

            Parallel.ForEach(blobs, blob =>
            {
                for (int i = 0; i < contour.Length - 1; i++)
                {
                    if (LineIntersectsRect(contour[i], contour[i + 1], blob.Bounding))
                    {
                        intersectingBlobs.Add(blob);
                        break;
                    }
                }
            });

            return intersectingBlobs.ToList();
        }

        private bool LineIntersectsRect(OpenCvSharp.Point p1, OpenCvSharp.Point p2, Rectangle rect)
        {
            if (rect.Contains(Lib.Common.CConverter.CVPointToPoint(p1)) || rect.Contains(Lib.Common.CConverter.CVPointToPoint(p2)))
            {
                return true;
            }

            // Calculate min and max X and Y values for the rectangle
            int minX = rect.Left;
            int maxX = rect.Right;
            int minY = rect.Top;
            int maxY = rect.Bottom;

            // Find the slope and y-intercept of the line between p1 and p2
            double m = (double)(p2.Y - p1.Y) / (double)(p2.X - p1.X);
            double b = p1.Y - (m * p1.X);

            // Check for intersection with the left vertical line of the rectangle
            double yLeft = (m * minX) + b;
            if (yLeft >= minY && yLeft <= maxY)
            {
                return true;
            }

            // Check for intersection with the right vertical line of the rectangle
            double yRight = (m * maxX) + b;
            if (yRight >= minY && yRight <= maxY)
            {
                return true;
            }

            // Check for intersection with the top horizontal line of the rectangle
            double xTop = (minY - b) / m;
            if (xTop >= minX && xTop <= maxX)
            {
                return true;
            }

            // Check for intersection with the bottom horizontal line of the rectangle
            double xBottom = (maxY - b) / m;
            if (xBottom >= minX && xBottom <= maxX)
            {
                return true;
            }

            // If no intersection found, return false
            return false;
        }

        public static PointF[] ConvertToPointFArray(List<CResultBlob> blobList)
        {
            PointF[] points = new PointF[blobList.Count];

            for (int i = 0; i < blobList.Count; i++)
            {
                CResultBlob blob = blobList[i];
                points[i] = new PointF((float)blob.Center.X, (float)blob.Center.Y);
            }

            return points;
        }

        public static OpenCvSharp.Point[] ConvertToPointArray(List<CResultBlob> blobList)
        {
            OpenCvSharp.Point[] points = new OpenCvSharp.Point[blobList.Count];

            for (int i = 0; i < blobList.Count; i++)
            {
                CResultBlob blob = blobList[i];
                points[i] = new OpenCvSharp.Point((float)blob.Center.X, (float)blob.Center.Y);
            }

            return points;
        }

        public static PointF[] SortByAngle(PointF[] points)
        {
            PointF center = new PointF(points.Average(p => p.X), points.Average(p => p.Y));
            return points.OrderBy(p => Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI).ToArray();
        }

        public static PointF[] SortPoints(PointF[] points)
        {
            // 가장 왼쪽에 있는 점을 찾습니다.
            PointF startPoint = points[0];
            foreach (PointF point in points)
            {
                if (point.X < startPoint.X)
                {
                    startPoint = point;
                }
            }

            // 시작점을 중심으로 각도를 계산합니다.
            List<PointF> sortedPoints = new List<PointF>();
            sortedPoints.Add(startPoint);
            PointF previousPoint = startPoint;
            while (sortedPoints.Count < points.Length)
            {
                double minAngle = double.MaxValue;
                PointF nextPoint = new PointF();
                foreach (PointF point in points)
                {
                    if (point == previousPoint) continue;

                    double angle = Math.Atan2(point.Y - previousPoint.Y, point.X - previousPoint.X);
                    double angleDifference = angle - Math.Atan2(startPoint.Y - previousPoint.Y, startPoint.X - previousPoint.X);
                    if (angleDifference < 0)
                    {
                        angleDifference += 2 * Math.PI;
                    }
                    if (angleDifference < minAngle)
                    {
                        nextPoint = point;
                        minAngle = angleDifference;
                    }
                }
                sortedPoints.Add(nextPoint);
                previousPoint = nextPoint;
            }

            return sortedPoints.ToArray();
        }

        public static void DrawPoints(PointF[] points, Graphics g, System.Drawing.Pen pen)
        {
            PointF[] sortedPoints = SortByAngle(points);

            for (int i = 0; i < sortedPoints.Length; i++)
            {
                PointF startPoint = sortedPoints[i];
                PointF endPoint = sortedPoints[(i + 1) % sortedPoints.Length];
                g.DrawLine(pen, startPoint, endPoint);
            }
        }
        private OpenCvSharp.Point[] beforeContour = new OpenCvSharp.Point[10];
        private OpenCvSharp.Point[] breforContourConvexHull = new OpenCvSharp.Point[10];
        private OpenCvSharp.Point2d breforeCenter= new OpenCvSharp.Point2d();
        private void btnBlobRun_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string test = Lib.Common.CUtil.LoadImageFilePath();

                //using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                //using (Mat ImageCVSource = Cv2.ImRead(Application.StartupPath + "\\OriginImage\\Chip 제거 전 원본 이미지 1.png"))
                using (Mat ImageCVSource = Cv2.ImRead(test))
                {
                    CDisplayManager.ImageSrc = ImageCVSource.Clone();

                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();
                    Graphics g = Graphics.FromImage(Result);

                    CVBlob cIVT_CVBlob = new CVBlob();
                    cIVT_CVBlob.SetProperty(CVisionTools.Blobs[DEFINE.CAM_1]);
                    cIVT_CVBlob.SetSourceImage(ImageCVSource);
                    cIVT_CVBlob.Run();

                    Results_bef = cIVT_CVBlob.results.OrderBy(x => x.Bounding.X).ToList();
                    Mat morph = new Mat();
                    Cv2.MorphologyEx(ImageCVSource, morph, MorphTypes.Erode, Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(8, 8)), new OpenCvSharp.Point(-1, -1), 1);

                    CVContour cIVT_CVContour = new CVContour();
                    cIVT_CVContour.SetProperty(CVisionTools.Contours[DEFINE.CAM_1]);
                    cIVT_CVContour.SetSourceImage(morph);
                    cIVT_CVContour.Run();

                    foreach (var item in cIVT_CVContour.results)
                    {
                        breforContourConvexHull = Cv2.ConvexHull(item.Contours);
                        beforeContour = Cv2.ConvexHull(item.Contours);
                        beforeContour = ExpandContourAroundCenter(beforeContour, item.Center, -8);
                        breforeCenter = item.Center;
                        g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Yellow, 1), ConvertPointToPoint2f(beforeContour));
                        g.FillEllipse(new SolidBrush(System.Drawing.Color.Yellow), new Rectangle((int)item.Center.X, (int)item.Center.Y, 10, 10));
                    }

                    List<CResultBlob> removelist = new List<CResultBlob>();
                    var tdd = GetIntersectingBlobs(beforeContour, Results_bef);
                    RemoveOutContoursBlobs(ref tdd, breforContourConvexHull, ref removelist);
                    RemoveOutContoursBlobs(ref Results_bef, breforContourConvexHull, ref removelist);

                    //Results_bef = Results_bef.Except(removelist).ToList();

                    foreach (var item in Results_bef)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                    }

                    //DrawPoints(ConvertToPointFArray(tdd), g, new System.Drawing.Pen(System.Drawing.Color.Red, 1));

                    //foreach (var item in tdd)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 1), item.Bounding);
                    //}

                    //var tdd2 = GetIntersectingBlobs(ConvertToPointArray(tdd), Results_bef);
                    //RemoveOutContoursBlobs(ref tdd2, contourConvexHull, ref removelist);

                    //var tdd2 = GetIntersectingBlobs(ConvertToPointArray(tdd), Results_bef);

                    //DrawPoints(ConvertToPointFArray(tdd2), g, new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 1));

                    //foreach (var item in tdd2)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 1), item.Bounding);
                    //}

                    //var extractarray = ExtractBoundaryBlobs(Results_bef, 10);

                    //foreach (var item in extractarray)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), item.Bounding);
                    //}

                    //Results_bef = Results_bef.OrderBy(b => b.Center.X).ToList();

                    //var array = ConvertListTo2DArray(Results_bef, Rt.Width, Rt.Height, cIVT_CVBlob.Property.CvROI);

                    //DrawBlobs(array, g, Rt);

                    //var extractarray = ExtractBoundaryBlobs(Results_bef, 10, filterBound);

                    //foreach (var item in extractarray)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), item.Bounding);
                    //}

                    //Results_bef = Results_bef.OrderBy(b => b.Bounding.Y).ToList();

                    //var outline = FindEmptyBlobs(Results_bef, 10);

                    //foreach (var item in outline)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), item.Bounding);
                    //}

                    // blobList를 반으로 나눕니다.
                    //int midIndex = Results_bef.Count / 10;
                    //List<CResultBlob> upList = Results_bef.GetRange(0, midIndex);
                    //List<CResultBlob> middleList = Results_bef.GetRange(midIndex, midIndex);
                    //List<CResultBlob> middleList2 = Results_bef.GetRange(midIndex * 2, midIndex);
                    //List<CResultBlob> bottomList = Results_bef.GetRange(midIndex * 9, midIndex);

                    //bottomList = bottomList.OrderBy(b => b.Bounding.X).ToList();

                    //var outline = upList.Concat(bottomList).ToList();

                    //foreach (var item in outline)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), item.Bounding);
                    //}

                    //var outline = GetOutlinePoints(upList, 4);
                    //var outline1 = GetMaxYMinBlobs(upList, 1, false);
                    //var outline2 = GetMaxYMinBlobs(bottomList, 1, true);


                    //var outline2 = GetMaxXMinBlobs(Results_bef, 1);
                    //var outline = GetNearbyBlobs(Results_bef, Rt, 1500);
                    //var outline = GetOutermostBlobs(Results_bef);

                    //int offset = 13;

                    //int nRows = Rt.Width / offset;
                    //int nCols = Rt.Height / offset;

                    //int startx = Rt.X;
                    //int starty = Rt.Y;

                    //for (int nRowIndex = 0; nRowIndex < nRows; nRowIndex++)
                    //{
                    //    for (int nColIndex = 0; nColIndex < nCols; nColIndex++)
                    //    {
                    //        Rectangle rtBoundary = new Rectangle(startx +(int)(nRowIndex * offset), starty + (int)(nColIndex * offset), offset, offset);
                    //        g.DrawRectangle(new Pen(Color.Yellow, 1), rtBoundary);
                    //    }
                    //}

                    //foreach (var item in outline2)
                    //{
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Yellow, 2), item.Bounding);
                    //}

                    //foreach (var item in Results_bef)
                    //{
                    //    //int offset2 = 2;
                    //    // Rectangle rtBoundary = new Rectangle(item.Bounding.X - offset2, item.Bounding.Y - offset2, item.Bounding.Width + offset2, item.Bounding.Height + offset2);
                    //    g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                    //    //g.DrawRectangle(new Pen(Color.Yellow, 1), rtBoundary);
                    //}

                    if (cIVT_CVBlob.results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVBlob.property.CvROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVBlob.property.CvROI.X - 20, (int)cIVT_CVBlob.property.CvROI.Y - 20);
                    }

                    //cResultBlobs = cIVT_CVBlob.Results;
                    stopwatch1.Stop();
                    //CLog.Error( "[Time] Blob {0} ", stopwatch1.ElapsedMilliseconds);

                    //Results_bef = cIVT_CVBlob.Results.OrderBy(x => x.Bounding.X).ToList();

                    //g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Orange, 1), Rt);
                    //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 1), Rt);
                    dgvResult1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgvResult1.ColumnHeadersVisible = false;
                    dgvResult1.DataSource = new CDefectList_Result().GetBlobList(Results_bef);
                    dgvResult1.ColumnHeadersVisible = true;

                    CDisplayManager.CreateLayerDisplay(Result, "제거전", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public List<CResultBlob> GetBlobsWithinDistance(List<CResultBlob> blobList, OpenCvSharp.Point2d Center, double maxDistance)
        {
            List<CResultBlob> filteredBlobs = blobList.Where(blob => GetDistance(Center, blob.Center) <= maxDistance).ToList();

            return filteredBlobs;
        }

        private double GetDistance(OpenCvSharp.Point2d point1, OpenCvSharp.Point2d point2)
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public List<CResultBlob> ExtractBoundaryBlobs(List<CResultBlob> blobList, double distanceThreshold)
        {
            if (blobList.Count == 0)
            {
                return new List<CResultBlob>();
            }

            // 가장 큰 사각형 범위를 계산합니다.
            double minX = blobList.Min(blob => blob.Center.X);
            double maxX = blobList.Max(blob => blob.Center.X);
            double minY = blobList.Min(blob => blob.Center.Y);
            double maxY = blobList.Max(blob => blob.Center.Y);

            // 주변에 다른 데이터가 있는지 확인하는 함수
            bool HasNeighbor(CResultBlob target, List<CResultBlob> blobs, double threshold, OpenCvSharp.Point2d offset)
            {
                OpenCvSharp.Point2d targetPosition = new OpenCvSharp.Point2d(target.Center.X + offset.X, target.Center.Y + offset.Y);
                return blobs.Any(blob => blob != target && Distance(targetPosition, blob.Center) <= threshold);
            }

            // 상하좌우에 데이터가 연속적으로 없는 경우를 외곽 데이터로 간주
            ConcurrentBag<CResultBlob> boundaryBlobs = new ConcurrentBag<CResultBlob>();

            // CResultBlob 객체들의 좌표값을 이용하여, 다른 군집에 위치한 객체들을 찾습니다.
            Parallel.ForEach(blobList, blob =>
            {
                bool leftNeighbor = HasNeighbor(blob, blobList, distanceThreshold, new OpenCvSharp.Point2d(-distanceThreshold, 0));
                bool rightNeighbor = HasNeighbor(blob, blobList, distanceThreshold, new OpenCvSharp.Point2d(distanceThreshold, 0));
                bool topNeighbor = HasNeighbor(blob, blobList, distanceThreshold, new OpenCvSharp.Point2d(0, -distanceThreshold));
                bool bottomNeighbor = HasNeighbor(blob, blobList, distanceThreshold, new OpenCvSharp.Point2d(0, distanceThreshold));

                if (!(leftNeighbor && rightNeighbor && topNeighbor && bottomNeighbor))
                {
                    boundaryBlobs.Add(blob);
                }
            });
            return boundaryBlobs.ToList();
        }

        // 두 점 사이의 거리 계산
        public double Distance(OpenCvSharp.Point2d point1, OpenCvSharp.Point2d point2)
        {
            double dx = point1.X - point2.X;
            double dy = point1.Y - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        //public List<CResultBlob> ExtractBoundaryBlobs(List<CResultBlob> blobList)
        //{
        //    if (blobList.Count == 0)
        //    {
        //        return new List<CResultBlob>();
        //    }

        //    // Blob의 중심점을 배열로 변환
        //    double[][] points = blobList.Select(blob => new double[] { blob.Center.X, blob.Center.Y }).ToArray();

        //    // K-means 클러스터링 설정
        //    int numberOfClusters = 30; // 클러스터 수를 조정하여 계층 수 변경 가능
        //    KMeans kmeans = new KMeans(numberOfClusters);
        //    var clusters = kmeans.Learn(points);

        //    // 클러스터링 결과를 사용하여 외곽 데이터 추출
        //    int[] labels = clusters.Decide(points);
        //    List<CResultBlob>[] groupedBlobs = new List<CResultBlob>[numberOfClusters];

        //    for (int i = 0; i < numberOfClusters; i++)
        //    {
        //        groupedBlobs[i] = new List<CResultBlob>();
        //    }

        //    for (int i = 0; i < blobList.Count; i++)
        //    {
        //        int clusterIndex = labels[i];
        //        groupedBlobs[clusterIndex].Add(blobList[i]);
        //    }

        //    // 각 클러스터의 반경 추정
        //    double[] radii = new double[numberOfClusters];
        //    for (int i = 0; i < numberOfClusters; i++)
        //    {
        //        var clusterCentroid = clusters.Centroids[i];
        //        double maxDistance = groupedBlobs[i].Max(blob => Distance(clusterCentroid, blob.Center));
        //        radii[i] = maxDistance;
        //    }

        //    // 가장 바깥쪽 클러스터를 외곽 데이터로 간주
        //    int outerClusterIndex = Array.IndexOf(radii, radii.Max());
        //    List<CResultBlob> boundaryBlobs = groupedBlobs[outerClusterIndex];

        //    return boundaryBlobs;
        //}

        //public List<CResultBlob> ExtractBoundaryBlobs(List<CResultBlob> blobList)
        //{
        //    if (blobList.Count == 0)
        //    {
        //        return new List<CResultBlob>();
        //    }

        //    // 데이터의 중심점 찾기
        //    double centerX = blobList.Average(blob => blob.Center.X);
        //    double centerY = blobList.Average(blob => blob.Center.Y);
        //    OpenCvSharp.Point2d center = new OpenCvSharp.Point2d(centerX, centerY);

        //    // 중심점에서 가장 멀리 떨어진 데이터를 외곽 데이터로 간주
        //    List<CResultBlob> boundaryBlobs = new List<CResultBlob>();
        //    double maxDistance = blobList.Max(blob => Distance(center, blob.Center));

        //    foreach (CResultBlob blob in blobList)
        //    {
        //        double distance = Distance(center, blob.Center);
        //        // 오차 범위(예: 0.9)를 적용하여 외곽에 가까운 데이터를 선택
        //        if (distance >= maxDistance * 0.9)
        //        {
        //            boundaryBlobs.Add(blob);
        //        }
        //    }

        //    return boundaryBlobs;
        //}

        // 두 점 사이의 거리 계산
        public double Distance(double[] point1, OpenCvSharp.Point2d point2)
        {
            double dx = point1[0] - point2.X;
            double dy = point1[1] - point2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        //public List<CResultBlob> ExtractBoundaryBlobs(List<CResultBlob> blobList)
        //{
        //    if (blobList.Count == 0)
        //    {
        //        return new List<CResultBlob>();
        //    }

        //    int minX = int.MaxValue;
        //    int minY = int.MaxValue;
        //    int maxX = int.MinValue;
        //    int maxY = int.MinValue;

        //    // 모든 경계 사각형 찾기
        //    foreach (CResultBlob blob in blobList)
        //    {
        //        Rectangle bounding = blob.Bounding;
        //        minX = Math.Min(minX, bounding.Left);
        //        minY = Math.Min(minY, bounding.Top);
        //        maxX = Math.Max(maxX, bounding.Right);
        //        maxY = Math.Max(maxY, bounding.Bottom);
        //    }

        //    // 가장 외곽에 있는 사각형 계산
        //    Rectangle outerRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

        //    // 외곽 영역 내의 CResultBlob 객체만 추출
        //    List<CResultBlob> boundaryBlobs = blobList.Where(blob =>
        //    {
        //        Rectangle bounding = blob.Bounding;
        //        return bounding.Left == outerRect.Left || bounding.Top == outerRect.Top ||
        //               bounding.Right == outerRect.Right || bounding.Bottom == outerRect.Bottom;
        //    }).ToList();

        //    return boundaryBlobs;
        //}

        //public List<CResultBlob> ExtractBoundaryBlobs(CResultBlob[,] blobArray)
        //{
        //    HashSet<CResultBlob> boundaryBlobs = new HashSet<CResultBlob>();

        //    int arrayWidth = blobArray.GetLength(0);
        //    int arrayHeight = blobArray.GetLength(1);

        //    // 왼쪽 및 오른쪽 가장자리 데이터 추출
        //    for (int y = 0; y < arrayHeight; y++)
        //    {
        //        for (int x = 0; x < arrayWidth; x++)
        //        {
        //            if (blobArray[x, y] != null)
        //            {
        //                boundaryBlobs.Add(blobArray[x, y]);
        //                break;
        //            }
        //        }

        //        for (int x = arrayWidth - 1; x >= 0; x--)
        //        {
        //            if (blobArray[x, y] != null)
        //            {
        //                boundaryBlobs.Add(blobArray[x, y]);
        //                break;
        //            }
        //        }
        //    }

        //    // 상단 및 하단 가장자리 데이터 추출
        //    for (int x = 1; x < arrayWidth - 1; x++)
        //    {
        //        for (int y = 0; y < arrayHeight; y++)
        //        {
        //            if (blobArray[x, y] != null)
        //            {
        //                boundaryBlobs.Add(blobArray[x, y]);
        //                break;
        //            }
        //        }

        //        for (int y = arrayHeight - 1; y >= 0; y--)
        //        {
        //            if (blobArray[x, y] != null)
        //            {
        //                boundaryBlobs.Add(blobArray[x, y]);
        //                break;
        //            }
        //        }
        //    }

        //    return boundaryBlobs.ToList();
        //}

        //public List<CResultBlob> ExtractBoundaryBlobs(CResultBlob[,] blobArray)
        //{
        //    List<CResultBlob> boundaryBlobs = new List<CResultBlob>();

        //    int arrayWidth = blobArray.GetLength(0);
        //    int arrayHeight = blobArray.GetLength(1);

        //    for (int x = 0; x < arrayWidth; x++)
        //    {
        //        for (int y = 0; y < arrayHeight; y++)
        //        {
        //            if (blobArray[x, y] != null)
        //            {
        //                bool isBoundary = false;

        //                // 왼쪽, 오른쪽 검사
        //                if (x == 0 || x == arrayWidth - 1)
        //                {
        //                    isBoundary = true;
        //                }
        //                // 위쪽, 아래쪽 검사
        //                else if (y == 0 || y == arrayHeight - 1)
        //                {
        //                    isBoundary = true;
        //                }
        //                // 대각선 방향 검사
        //                else if (x == y || x == arrayHeight - 1 - y)
        //                {
        //                    isBoundary = true;
        //                }

        //                if (isBoundary)
        //                {
        //                    boundaryBlobs.Add(blobArray[x, y]);
        //                }
        //            }
        //        }
        //    }

        //    return boundaryBlobs;
        //}

        public void DrawBlobs(CResultBlob[,] blobArray, Graphics graphics, Rectangle rt)
        {
            int arrayWidth = blobArray.GetLength(0);
            int arrayHeight = blobArray.GetLength(1);

            for (int x = 0; x < arrayWidth; x++)
            {
                for (int y = 0; y < arrayHeight; y++)
                {

                    if (blobArray[x, y] != null)
                    {
                        //Rectangle rectangle = new Rectangle(x - rt.Width, y - rt.Height, rt .Width* 2, rt .Height* 2);                        
                        graphics.DrawRectangle(Pens.Blue, blobArray[x, y].Bounding);
                    }
                    //else
                    //{
                    //    if(rt.Contains(new  System.Drawing.Point(x,y)))
                    //    {
                    //        Rectangle rectangle = new Rectangle(x - rt.Width, y - rt.Height, rt.Width * 2, rt.Height * 2);
                    //        graphics.DrawRectangle(Pens.Yellow, rectangle);
                    //    }                        
                    //}
                }
            }
        }

        public CResultBlob[,] ConvertListTo2DArray(List<CResultBlob> blobList, int arrayWidth, int arrayHeight, OpenCvSharp.Rect roi)
        {
            CResultBlob[,] blobArray = new CResultBlob[roi.Width, roi.Height];

            foreach (CResultBlob blob in blobList)
            {
                int x = (int)blob.Center.X - roi.X;
                int y = (int)blob.Center.Y - roi.Y;

                if (x >= 0 && x < roi.Width && y >= 0 && y < roi.Height)
                {
                    blobArray[x, y] = blob;
                }
            }

            return blobArray;
        }

        public List<CResultBlob> FindEmptyBlobs(List<CResultBlob> blobs, int distance)
        {
            List<CResultBlob> emptyBlobs = new List<CResultBlob>();
            foreach (CResultBlob blob in blobs)
            {
                bool isEmpty = true;
                foreach (CResultBlob nearbyBlob in blobs)
                {
                    if (nearbyBlob.Center == blob.Center) continue; // 같은 객체는 비교하지 않음
                    int dx = Math.Abs((int)blob.Center.X - (int)nearbyBlob.Center.X);
                    int dy = Math.Abs((int)blob.Center.Y - (int)nearbyBlob.Center.Y);
                    if (dx <= distance && dy <= distance) // 주변에 객체가 존재하는 경우
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty)
                {
                    emptyBlobs.Add(blob);
                }
            }
            return emptyBlobs;
        }

        public static List<CResultBlob> ExtractIsolatedBlobs(List<CResultBlob> blobs)
        {
            // blob들을 x, y 좌표 기준으로 정렬
            List<CResultBlob> sortedBlobs = blobs.OrderBy(b => b.Bounding.X).ThenBy(b => b.Bounding.Y).ToList();

            List<CResultBlob> isolatedBlobs = new List<CResultBlob>();
            // blob들 간의 연속된 x, y 범위를 저장할 변수
            int prevXMax = sortedBlobs[0].Bounding.Right; // 이전 blob의 x 최대값 초기화
            int prevYMax = sortedBlobs[0].Bounding.Bottom; // 이전 blob의 y 최대값 초기화
                                                           // 첫 번째 blob은 이미 정렬된 상태이므로 i=1부터 시작
            for (int i = 1; i < sortedBlobs.Count; i++)
            {
                CResultBlob currBlob = sortedBlobs[i];
                // 이전 blob과 x, y 연속 여부 확인
                if (currBlob.Bounding.X > prevXMax + 1 || currBlob.Bounding.Y > prevYMax + 1)
                {
                    // 연속되지 않는 blob은 isolatedBlobs 리스트에 추가
                    isolatedBlobs.Add(currBlob);
                }
                // x, y 범위 갱신
                prevXMax = Math.Max(prevXMax, currBlob.Bounding.Right);
                prevYMax = Math.Max(prevYMax, currBlob.Bounding.Bottom);
            }

            return isolatedBlobs;
        }

        public static List<CResultBlob> ExtractOuterBlobs(List<CResultBlob> blobs)
        {
            // 모든 blobs의 rectangle을 포함할 수 있는 큰 Circle 생성
            Point2d[] centers = blobs.Select(b => b.Center).ToArray();
            double maxRadius = Math.Sqrt(Math.Pow(centers.Max(p => p.X) - centers.Min(p => p.X), 2) + Math.Pow(centers.Max(p => p.Y) - centers.Min(p => p.Y), 2));
            Point2d center = new Point2d((centers.Max(p => p.X) + centers.Min(p => p.X)) / 2, (centers.Max(p => p.Y) + centers.Min(p => p.Y)) / 2);

            List<CResultBlob> outerBlobs = new List<CResultBlob>();
            foreach (CResultBlob blob in blobs)
            {
                // blob의 Center 좌표가 Circle의 외곽 영역에 위치하는지 확인
                double distance = Math.Sqrt(Math.Pow(center.X - blob.Center.X, 2) + Math.Pow(center.Y - blob.Center.Y, 2));
                if (distance <= maxRadius)
                {
                    outerBlobs.Add(blob); // 외곽에 위치하는 blob이면 추출 리스트에 추가
                }
            }

            return outerBlobs;
        }

        public List<CResultBlob> GetOutlinePoints(List<CResultBlob> blobList, double threshold)
        {
            // 1. Contour(Point[]) 정보 추출
            List<System.Drawing.Point[]> contours = new List<System.Drawing.Point[]>();
            foreach (CResultBlob blob in blobList)
            {
                contours.Add(new System.Drawing.Point[] {
            new System.Drawing.Point(blob.Bounding.Left, blob.Bounding.Top),
            new System.Drawing.Point(blob.Bounding.Right, blob.Bounding.Top),
            new System.Drawing.Point(blob.Bounding.Right, blob.Bounding.Bottom),
            new System.Drawing.Point(blob.Bounding.Left, blob.Bounding.Bottom)
                });
            }

            // 2. Contour(Point[]) 병합
            System.Drawing.Point[] mergedContour = contours.SelectMany(x => x).ToArray();

            // 3. List<CResultBlob> 객체들의 Center Point와 Contour(Point[])의 거리 계산
            List<CResultBlob> outlinePoints = new List<CResultBlob>();
            foreach (CResultBlob blob in blobList)
            {
                double minDistance = double.MaxValue;

                ConcurrentBag<CResultBlob> isolatedBlobs = new ConcurrentBag<CResultBlob>();

                // CResultBlob 객체들의 좌표값을 이용하여, 다른 군집에 위치한 객체들을 찾습니다.
                Parallel.ForEach(mergedContour, contourPoint =>
                {
                    double distance = Math.Sqrt(Math.Pow(blob.Center.X - contourPoint.X, 2) + Math.Pow(blob.Center.Y - contourPoint.Y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                });

                if (minDistance <= threshold)
                {
                    outlinePoints.Add(blob);
                }
            }

            return outlinePoints;
        }

        public List<CResultBlob> GetMaxXMinBlobs(List<CResultBlob> blobList, int padding)
        {
            List<CResultBlob> maxYMinBlobs = new List<CResultBlob>();

            // Center값을 기준으로 정렬합니다.
            blobList = blobList.OrderBy(b => b.Center.Y).ToList();

            // X값이 차이가 5픽셀 안에 들어오는 객체들을 한 그룹으로 묶습니다.
            List<List<CResultBlob>> blobGroups = new List<List<CResultBlob>>();
            List<CResultBlob> currGroup = new List<CResultBlob>();
            double prevY = -1;
            foreach (CResultBlob blob in blobList)
            {
                if (prevY < 0)
                {
                    // 첫 번째 객체인 경우
                    prevY = blob.Center.Y;
                    currGroup.Add(blob);
                }
                else
                {
                    // 이전 객체와의 X값 차이를 계산합니다.
                    double diff = Math.Abs(blob.Center.Y - prevY);

                    if (diff <= padding)
                    {
                        currGroup.Add(blob);
                    }
                    else
                    {
                        blobGroups.Add(currGroup);
                        currGroup = new List<CResultBlob>();
                        currGroup.Add(blob);
                    }

                    prevY = blob.Center.Y;
                }
            }
            blobGroups.Add(currGroup);

            // 각 그룹에서 Y축의 최대/최소 객체를 구합니다.
            foreach (List<CResultBlob> group in blobGroups)
            {
                if (group.Count > 0)
                {
                    CResultBlob maxXBlob = group[0];
                    CResultBlob minXBlob = group[0];

                    foreach (CResultBlob blob in group)
                    {
                        if (blob.Center.X > maxXBlob.Center.X)
                        {
                            maxXBlob = blob;
                        }
                        if (blob.Center.X < minXBlob.Center.X)
                        {
                            minXBlob = blob;
                        }
                    }

                    maxYMinBlobs.Add(maxXBlob);
                    maxYMinBlobs.Add(minXBlob);
                }
            }

            return maxYMinBlobs;
        }

        //public List<CResultBlob> GetMaxYMinBlobs(List<CResultBlob> blobList, int padding)
        //{
        //    List<CResultBlob> maxYMinBlobs = new List<CResultBlob>();

        //    // Center값을 기준으로 정렬합니다.
        //    blobList = blobList.OrderBy(b => b.Center.X).ToList();

        //    // blobList를 반으로 나눕니다.
        //    int midIndex = blobList.Count / 2;
        //    List<CResultBlob> leftList = blobList.Take(midIndex).ToList();
        //    List<CResultBlob> rightList = blobList.Skip(midIndex).ToList();

        //    // 왼쪽 리스트에서 Y축의 최대/최소 객체를 구합니다.
        //    if (leftList.Count > 0)
        //    {
        //        CResultBlob maxYBlob = leftList[0];
        //        CResultBlob minYBlob = leftList[0];

        //        foreach (CResultBlob blob in leftList)
        //        {
        //            if (blob.Center.Y > maxYBlob.Center.Y)
        //            {
        //                maxYBlob = blob;
        //            }
        //            if (blob.Center.Y < minYBlob.Center.Y)
        //            {
        //                minYBlob = blob;
        //            }
        //        }

        //        maxYMinBlobs.Add(maxYBlob);
        //        maxYMinBlobs.Add(minYBlob);
        //    }

        //    // 오른쪽 리스트에서 Y축의 최대/최소 객체를 구합니다.
        //    if (rightList.Count > 0)
        //    {
        //        CResultBlob maxYBlob = rightList[0];
        //        CResultBlob minYBlob = rightList[0];

        //        foreach (CResultBlob blob in rightList)
        //        {
        //            if (blob.Center.Y > maxYBlob.Center.Y)
        //            {
        //                maxYBlob = blob;
        //            }
        //            if (blob.Center.Y < minYBlob.Center.Y)
        //            {
        //                minYBlob = blob;
        //            }
        //        }

        //        maxYMinBlobs.Add(maxYBlob);
        //        maxYMinBlobs.Add(minYBlob);
        //    }

        //    return maxYMinBlobs;
        //}

        public List<CResultBlob> GetMaxYMinBlobs(List<CResultBlob> blobList, double padding, bool UseMax)
        {
            List<CResultBlob> maxYMinBlobs = new List<CResultBlob>();

            // Center값을 기준으로 정렬합니다.
            blobList = blobList.OrderBy(b => b.Bounding.X).ThenBy(b => b.Bounding.Y).ToList();

            // X값이 차이가 5픽셀 안에 들어오는 객체들을 한 그룹으로 묶습니다.
            List<List<CResultBlob>> blobGroups = new List<List<CResultBlob>>();
            List<CResultBlob> currGroup = new List<CResultBlob>();
            double prevX = -1;
            foreach (CResultBlob blob in blobList)
            {
                if (blob.Bounding.X == 2167 && blob.Bounding.Y == 3826)
                {

                }

                if (blob.Center.X == 2170.94 && blob.Center.Y == 1332.89)
                {

                }

                if (prevX < 0)
                {
                    // 첫 번째 객체인 경우
                    prevX = blob.Bounding.X;
                    currGroup.Add(blob);
                }
                else
                {
                    // 이전 객체와의 X값 차이를 계산합니다.
                    double diff = Math.Abs(blob.Bounding.X - prevX);

                    if (diff <= padding)
                    {
                        currGroup.Add(blob);
                        prevX = blob.Bounding.X;
                    }
                    else
                    {
                        blobGroups.Add(currGroup);
                        currGroup = new List<CResultBlob>();
                        currGroup.Add(blob);
                        prevX = -1;
                    }
                }
            }
            if (currGroup.Count == 0)
            {

            }
            blobGroups.Add(currGroup);

            // 각 그룹에서 Y축의 최대/최소 객체를 구합니다.
            foreach (List<CResultBlob> group in blobGroups)
            {
                if (group.Count > 0)
                {
                    CResultBlob maxYBlob = group[0];
                    CResultBlob minYBlob = group[0];

                    foreach (CResultBlob blob in group)
                    {
                        if (blob.Bounding.Y > maxYBlob.Bounding.Y)
                        {
                            maxYBlob = blob;
                        }
                        if (blob.Bounding.Y < minYBlob.Bounding.Y)
                        {
                            minYBlob = blob;
                        }
                    }
                    maxYMinBlobs.Add(maxYBlob);
                    maxYMinBlobs.Add(minYBlob);
                    if (UseMax) { }
                    else
                    {

                    }
                }
                else
                {

                }
            }

            return maxYMinBlobs;
        }

        public List<CResultBlob> GetDisconnectedBlobs(List<CResultBlob> blobList, int padding)
        {
            List<CResultBlob> disconnectedBlobs = new List<CResultBlob>();

            // 객체의 Rectangle을 x 좌표로 정렬합니다.
            blobList = blobList.OrderBy(b => b.Bounding.X).ToList();

            // 연속적인 객체 간의 간격을 계산합니다.
            int continuousCount = 0;
            int prevRight = -1;
            foreach (CResultBlob blob in blobList)
            {
                if (prevRight < 0)
                {
                    // 첫 번째 객체인 경우
                    prevRight = blob.Bounding.Left;
                    continuousCount = 1;
                }
                else
                {
                    // 이전 객체와 연속적인 간격을 계산합니다.
                    int gap = blob.Bounding.X - prevRight;
                    if (gap <= padding)
                    {
                        continuousCount++;
                    }
                    else
                    {
                        // 연속적인 객체의 개수가 5개 이하인 경우, 해당 객체를 리스트에 추가합니다.
                        if (continuousCount <= 5)
                        {
                            disconnectedBlobs.AddRange(blobList.GetRange(blobList.IndexOf(blob) - continuousCount, continuousCount));
                        }

                        continuousCount = 1;
                    }

                    prevRight = blob.Bounding.Left;
                }
            }

            // 마지막 연속적인 객체들을 처리합니다.
            if (continuousCount > 0 && continuousCount <= 5)
            {
                disconnectedBlobs.AddRange(blobList.GetRange(blobList.Count - continuousCount, continuousCount));
            }

            return disconnectedBlobs;
        }

        //public List<CResultBlob> GetDisconnectedBlobs(List<CResultBlob> blobList, int padding)
        //{
        //    List<CResultBlob> disconnectedBlobs = new List<CResultBlob>();

        //    // 객체의 Rectangle을 x,y 좌표로 정렬합니다.
        //    blobList = blobList.OrderBy(b => b.Bounding.X).ThenBy(b => b.Bounding.Y).ToList();

        //    // 인접한 Rectangle들의 간격을 비교하여 연속적인 간격을 가진 Rectangle들을 추출합니다.
        //    for (int i = 0; i < blobList.Count - 1; i++)
        //    {
        //        CResultBlob currBlob = blobList[i];
        //        CResultBlob nextBlob = blobList[i + 1];

        //        int xGap = nextBlob.Bounding.X - (currBlob.Bounding.X + currBlob.Bounding.Width);
        //        int yGap = nextBlob.Bounding.Y - currBlob.Bounding.Y;

        //        if (xGap >= padding && yGap >= padding)
        //        {
        //            disconnectedBlobs.Add(currBlob);
        //        }
        //    }

        //    // 마지막 객체를 검사합니다.
        //    CResultBlob lastBlob = blobList[blobList.Count - 1];
        //    int yGapFromLast = lastBlob.Bounding.Y - blobList[blobList.Count - 2].Bounding.Y;
        //    if (yGapFromLast >= padding)
        //    {
        //        disconnectedBlobs.Add(lastBlob);
        //    }

        //    return disconnectedBlobs;
        //}

        public List<CResultBlob> GetNearbyBlobs(List<CResultBlob> blobList, Rectangle centerRect, double distanceThreshold)
        {
            List<CResultBlob> nearbyBlobs = new List<CResultBlob>();

            foreach (CResultBlob blob in blobList)
            {
                double distance = Math.Sqrt(Math.Pow(blob.Bounding.X - centerRect.X, 2) + Math.Pow(blob.Bounding.Y - centerRect.Y, 2));
                if (distance <= distanceThreshold)
                {
                    nearbyBlobs.Add(blob);
                }
            }

            return nearbyBlobs;
        }

        public List<CResultBlob> GetOutermostBlobs(List<CResultBlob> blobList)
        {
            // List<CResultBlob> 객체들을 Bounding Box 기준으로 오름차순으로 정렬합니다.
            List<CResultBlob> sortedBlobs = blobList.OrderBy(blob => blob.Bounding.X).ToList();

            List<CResultBlob> outermostBlobs = new List<CResultBlob>();
            Rectangle prevBounding = new Rectangle(-1, -1, 0, 0); // 이전 Bounding Box의 위치를 저장하는 변수입니다.
            foreach (CResultBlob blob in sortedBlobs)
            {
                // 현재 Bounding Box의 위치가 이전 Bounding Box들 중 가장 오른쪽에 위치한 Bounding Box와 겹치지 않는 경우 해당 객체를 결과 리스트에 추가합니다.
                if (blob.Bounding.X >= prevBounding.Right)
                {
                    outermostBlobs.Add(blob);
                    prevBounding = blob.Bounding;
                }
            }

            return outermostBlobs;
        }

        public Rectangle FindOutermostPoint(List<CResultBlob> blobList)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            // X, Y, WIDTH, HEIGHT 계산
            foreach (CResultBlob blob in blobList)
            {
                int x = blob.Bounding.X;
                int y = blob.Bounding.Y;
                int width = blob.Bounding.Width;
                int height = blob.Bounding.Height;

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x + width);
                maxY = Math.Max(maxY, y + height);
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private void dgvSeletecList_CellClick2(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }

                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;

                if (dgv.Rows[rowIndex].Cells[1].Value == null) { return; }
                var rect = new Rectangle(int.Parse(dgv.Rows[rowIndex].Cells[5].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[6].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[7].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[8].Value.ToString()));


                int Index = CDisplayManager.FindIndex("제거후");

                double Factor_W = 50;
                double Factor_H = 50;

                int W = (int)(CDisplayManager.Displays[Index].viewer._Ib.Width / Factor_W);
                int H = (int)(CDisplayManager.Displays[Index].viewer._Ib.Height / Factor_H);

                Rectangle rt = CDisplayManager.Displays[Index].viewer._Ib.RectangleToScreen(rect);
                CDisplayManager.Displays[Index].viewer._Ib.ZoomToRegion((int)(rect.X), (int)(rect.Y), (int)(W), (int)(H));
                //CDisplayManager.Displays[Index].viewer._Ib.ScrollTo(rt.X, rt.Y, rt.Width, rt.Height);
                CDisplayManager.Displays[Index].Activate();

                //trbThreshold_Scroll(null, null);

                //Rectangle rt2 = CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.RectangleToScreen(rect);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ZoomToRegion(rt.X, rt.Y, 500, 500);
                //CDisplayManager.Displays[DEFINE.nThreshold].ImageView.ib.ScrollTo(rt.X, rt.Y, 400, 400);
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        private void dgvSeletecList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.CurrentCell == null) { return; }

                int columnIndex = dgv.CurrentCell.ColumnIndex;
                int rowIndex = dgv.CurrentCell.RowIndex;

                if (dgv.Rows[rowIndex].Cells[1].Value == null) { return; }
                var rect = new Rectangle(int.Parse(dgv.Rows[rowIndex].Cells[5].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[6].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[7].Value.ToString()),
                                         int.Parse(dgv.Rows[rowIndex].Cells[8].Value.ToString()));


                int Index = CDisplayManager.FindIndex("제거전");

                double Factor_W = 50;
                double Factor_H = 50;

                int W = (int)(CDisplayManager.Displays[Index].viewer._Ib.Width / Factor_W);
                int H = (int)(CDisplayManager.Displays[Index].viewer._Ib.Height / Factor_H);

                Rectangle rt = CDisplayManager.Displays[Index].viewer._Ib.RectangleToScreen(rect);
                CDisplayManager.Displays[Index].viewer._Ib.ZoomToRegion((int)(rect.X), (int)(rect.Y), (int)(W), (int)(H));
                //CDisplayManager.Displays[Index].viewer._Ib.ScrollTo(rt.X, rt.Y, rt.Width, rt.Height);
                CDisplayManager.Displays[Index].Activate();
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                CCommon.ShowMessageBox("EXCEPTION", $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public List<CResultBlob> Results_bef = new List<CResultBlob>();
        public List<CResultBlob> Results_afe = new List<CResultBlob>();


        private OpenCvSharp.Point[] Aftcontour = new OpenCvSharp.Point[10];
        private OpenCvSharp.Point[] AftcontourConvexHull = new OpenCvSharp.Point[10];
        private OpenCvSharp.Point2d AfterCenter = new OpenCvSharp.Point2d();
        private void rjButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string test = Lib.Common.CUtil.LoadImageFilePath();
                //using (Mat ImageCVSource = Cv2.ImRead(Application.StartupPath + "\\OriginImage\\Chip 제거 후 원본 이미지.png"))
                using (Mat ImageCVSource = Cv2.ImRead(test))
                {
                    CDisplayManager.ImageSrc = ImageCVSource.Clone();

                    if (ImageCVSource.Channels() == 3) Cv2.CvtColor(ImageCVSource, ImageCVSource, ColorConversionCodes.RGB2GRAY);

                    Bitmap TempSrc = Lib.Common.CImageConverter.ToBitmap(ImageCVSource);
                    Bitmap Result = TempSrc.Clone(new Rectangle(0, 0, TempSrc.Width, TempSrc.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Stopwatch stopwatch1 = Stopwatch.StartNew();
                    Graphics g = Graphics.FromImage(Result);

                    CVBlob cIVT_CVBlob = new CVBlob();
                    cIVT_CVBlob.SetProperty(CVisionTools.Blobs[DEFINE.CAM_1]);
                    cIVT_CVBlob.SetSourceImage(ImageCVSource);
                    cIVT_CVBlob.Run();

                    Results_afe = cIVT_CVBlob.results.OrderBy(x => x.Bounding.X).ToList();
                    Mat morph = new Mat();
                    Cv2.MorphologyEx(ImageCVSource, morph, MorphTypes.Erode, Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(10, 10)), new OpenCvSharp.Point(-1, -1), 1);

                    CVContour cIVT_CVContour = new CVContour();
                    cIVT_CVContour.SetProperty(CVisionTools.Contours[DEFINE.CAM_1]);
                    cIVT_CVContour.SetSourceImage(morph);
                    cIVT_CVContour.Run();

                    foreach (var item in cIVT_CVContour.results)
                    {
                        AftcontourConvexHull = Cv2.ConvexHull(item.Contours);
                        Aftcontour = Cv2.ConvexHull(item.Contours);
                        Aftcontour = ExpandContourAroundCenter(Aftcontour, item.Center, -8);
                        AfterCenter = item.Center;
                        g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Yellow, 1), ConvertPointToPoint2f(Aftcontour));
                        g.FillEllipse(new SolidBrush(System.Drawing.Color.Yellow), new Rectangle((int)item.Center.X, (int)item.Center.Y, 10, 10));
                    }

                    List<CResultBlob> removelist = new List<CResultBlob>();
                    var tdd = GetIntersectingBlobs(Aftcontour, Results_afe);
                    RemoveOutContoursBlobs(ref tdd, AftcontourConvexHull, ref removelist);
                    RemoveOutContoursBlobs(ref Results_afe, AftcontourConvexHull, ref removelist);

                    //Results_afe = Results_afe.Except(removelist).ToList();

                    foreach (var item in Results_afe)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                    }

                    DrawPoints(ConvertToPointFArray(tdd), g, new System.Drawing.Pen(System.Drawing.Color.Red, 1));

                    foreach (var item in tdd)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 1), item.Bounding);
                    }

                    if (cIVT_CVBlob.results.Count == 0)
                    {
                        g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cIVT_CVBlob.property.CvROI));
                        g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cIVT_CVBlob.property.CvROI.X - 20, (int)cIVT_CVBlob.property.CvROI.Y - 20);
                    }
                    //cResultBlobs = cIVT_CVBlob.Results;
                    stopwatch1.Stop();
                    //CLog.Error( "[Time] Blob {0} ", stopwatch1.ElapsedMilliseconds);

                    //RemoveIsolatedBlobs(Results_afe);

                    //var Rt = FindOutermostPoint(Results_afe);
                    //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 1), Rt);

                    dgvResult2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    dgvResult2.ColumnHeadersVisible = false;
                    dgvResult2.DataSource = new CDefectList_Result().GetBlobList(Results_afe);
                    dgvResult2.ColumnHeadersVisible = true;

                    CDisplayManager.CreateLayerDisplay(Result, "제거후", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }
        public static Point2f ConvertPoint2dToPoint2f(Point2d point2d)
        {
            return new Point2f((float)point2d.X, (float)point2d.Y);
        }

        public static System.Drawing.PointF ConvertPoint2dToPointf(Point2d point2d)
        {
            return new System.Drawing.PointF((float)point2d.X, (float)point2d.Y);
        }

        private object ob = new object();
        public void RemoveOutContoursBlobs(ref List<CResultBlob> blobList, OpenCvSharp.Point[] Contours, ref List<CResultBlob> removelist)
        {
            ConcurrentBag<CResultBlob> isolatedBlobs = new ConcurrentBag<CResultBlob>();

            Parallel.ForEach(blobList, blob =>
            {
                // 다각형 내부에 점이 있는지 확인합니다.
                double result = Cv2.PointPolygonTest(Contours, ConvertPoint2dToPoint2f(blob.Center), true);

                if (result > 0)
                {
                    Console.WriteLine("The point is inside the contour.");
                }
                else if (result < -5)
                {
                    isolatedBlobs.Add(blob);
                    Console.WriteLine("The point is outside the contour.");
                }
                else
                {
                    Console.WriteLine("The point is on the contour.");
                }
            });
            removelist = isolatedBlobs.ToList().ConvertAll(s => s);
            blobList = blobList.Except(isolatedBlobs).ToList();
        }

        public void RemoveInContoursBlobs(ref List<CResultBlob> blobList, OpenCvSharp.Point[] Contours, ref List<CResultBlob> removelist)
        {
            ConcurrentBag<CResultBlob> isolatedBlobs = new ConcurrentBag<CResultBlob>();

            Parallel.ForEach(blobList, blob =>
            {
                // 다각형 내부에 점이 있는지 확인합니다.
                double result = Cv2.PointPolygonTest(Contours, ConvertPoint2dToPoint2f(blob.Center), true);

                if (result > 0)
                {
                    isolatedBlobs.Add(blob);
                    Console.WriteLine("The point is inside the contour.");
                }
                else if (result < -5)
                {

                    Console.WriteLine("The point is outside the contour.");
                }
                else
                {
                    Console.WriteLine("The point is on the contour.");
                }
            });
            removelist = isolatedBlobs.ToList().ConvertAll(s => s);
            blobList = blobList.Except(isolatedBlobs).ToList();
        }

        public List<CResultBlob> RemoveIsolatedBlobs(List<CResultBlob> blobList, ref List<CResultBlob> removelist)
        {
            ConcurrentBag<CResultBlob> isolatedBlobs = new ConcurrentBag<CResultBlob>();

            Parallel.ForEach(blobList, blob =>
            {
                bool[] judge = new bool[8] { false, false, false, false, false, false, false, false };

                // 주변 객체 검사를 위해 루프를 돌지 않고, 모든 객체를 한 번에 필터링합니다.
                // 이렇게 하면 중첩 루프를 제거하여 성능을 향상시킬 수 있습니다.
                List<CResultBlob> filteredBlobs = blobList.Where(nearbyBlob => Math.Abs(nearbyBlob.Center.X - blob.Center.X) <= 20 &&
                                                                               Math.Abs(nearbyBlob.Center.Y - blob.Center.Y) <= 20).ToList();

                foreach (CResultBlob nearbyBlob in filteredBlobs)
                {
                    if (blob.Center.X == nearbyBlob.Center.X && blob.Center.Y == nearbyBlob.Center.Y) { continue; }

                    int diffCenterX = (int)(blob.Center.X - nearbyBlob.Center.X);
                    int diffCenterY = (int)(blob.Center.Y - nearbyBlob.Center.Y);

                    if (!judge[0] && diffCenterX >= 3 && diffCenterY >= 3) // 좌측 상단
                    {
                        judge[0] = true;
                    }

                    if (!judge[1] && diffCenterY >= 3) // 중앙 상단
                    {
                        judge[1] = true;
                    }

                    if (!judge[2] && diffCenterX <= 3 && diffCenterY >= 3) // 우측 상단
                    {
                        judge[2] = true;
                    }

                    if (!judge[3] && diffCenterX >= 3) // 좌측 중앙
                    {
                        judge[3] = true;
                    }

                    if (!judge[4] && diffCenterX <= 3) // 우측 중앙
                    {
                        judge[4] = true;
                    }

                    if (!judge[5] && diffCenterX >= 3 && diffCenterY <= 3) // 좌측 하단
                    {
                        judge[5] = true;
                    }

                    if (!judge[6] && diffCenterY <= 3) // 중앙 하단
                    {
                        judge[6] = true;
                    }

                    if (!judge[7] && diffCenterX >= 3 && diffCenterY <= 3) // 우측 하단
                    {
                        judge[7] = true;
                    }
                }

                int count = judge.Where(c => c).Count();

                if (count < 3)
                {
                    isolatedBlobs.Add(blob);
                }
            });

            removelist = isolatedBlobs.ToList().ConvertAll(s => s);

            return blobList = blobList.Except(isolatedBlobs).ToList();
        }

        public List<CResultBlob> FindIsolatedBlobs(List<CResultBlob> blobList, double threshold)
        {
            // List<CResultBlob> isolatedBlobs = new List<CResultBlob>();

            ConcurrentBag<CResultBlob> isolatedBlobs = new ConcurrentBag<CResultBlob>();

            // CResultBlob 객체들의 좌표값을 이용하여, 다른 군집에 위치한 객체들을 찾습니다.
            Parallel.ForEach(blobList, blob =>
            {
                List<CResultBlob> nearbyBlobs = blobList.Where(b => b != blob && Math.Sqrt(Math.Pow(blob.Center.X - b.Center.X, 2) + Math.Pow(blob.Center.Y - b.Center.Y, 2)) <= threshold).ToList();

                if (nearbyBlobs.Count == 0)
                {
                    isolatedBlobs.Add(blob);
                }
            });

            return isolatedBlobs.ToList();
        }

        public static OpenCvSharp.Point[] TranslateContour(OpenCvSharp.Point[] contour, PointF translationVector)
        {
            // Contour의 중심 좌표를 계산합니다.
            OpenCvSharp.Point centroid = new OpenCvSharp.Point((int)contour.Average(p => p.X), (int)contour.Average(p => p.Y));

            // 두 Contour 중심 좌표의 거리를 계산합니다.
            float distance = (float)Math.Sqrt(Math.Pow(translationVector.X, 2) + Math.Pow(translationVector.Y, 2));

            // 이동할 벡터의 방향을 계산합니다.
            float angle = (float)Math.Atan2(translationVector.Y, translationVector.X);

            // Contour의 모든 점들을 이동합니다.
            OpenCvSharp.Point[] translatedContour = new OpenCvSharp.Point[contour.Length];
            for (int i = 0; i < contour.Length; i++)
            {
                PointF vectorToVertex = new PointF(contour[i].X - centroid.X, contour[i].Y - centroid.Y);
                float vertexDistance = (float)Math.Sqrt(Math.Pow(vectorToVertex.X, 2) + Math.Pow(vectorToVertex.Y, 2));
                float vertexAngle = (float)Math.Atan2(vectorToVertex.Y, vectorToVertex.X);
                PointF translatedVector = new PointF(
                    (float)Math.Cos(angle + vertexAngle) * vertexDistance * distance,
                    (float)Math.Sin(angle + vertexAngle) * vertexDistance * distance);
                translatedContour[i] = new OpenCvSharp.Point((int)Math.Round(centroid.X + translatedVector.X), (int)Math.Round(centroid.Y + translatedVector.Y));
            }

            return translatedContour;
        }


        private void rjButton2_Click(object sender, EventArgs e)
        {
            //List<CResultBlob> filteredBlobs = FilterBlobs(Results_bef, Results_afe, 5.0);
            var rectangle = FindOutermostPoint(Results_bef);
            //GenerateMappingData(Results_bef, Results_afe, 10, 10, out int[,] mappingData);

            Bitmap beforeImage = new Bitmap(Application.StartupPath + "\\OriginImage\\Chip 제거 전 원본 이미지 1.png");

            Results_bef = Results_bef.OrderBy(x => x.Center.X).ThenBy(x => x.Center.Y).ToList();
            Results_afe = Results_afe.OrderBy(x => x.Center.X).ThenBy(x => x.Center.Y).ToList();

            List<CResultBlob> matchingChips = GetNonMatchingChips(Results_bef, Results_afe, 10);
            List<CResultBlob> removelist = new List<CResultBlob>();

            PointF translationVector = new PointF((float)AfterCenter.X - (float)breforeCenter.X, (float)AfterCenter.Y - (float)breforeCenter.Y);

            Aftcontour = ExpandContourAroundCenter(Aftcontour, AfterCenter, -5);

            RemoveInContoursBlobs(ref matchingChips, Aftcontour, ref removelist);

            DrawMatchingChipsOnGrid(beforeImage, 1, 1, matchingChips);

            CDisplayManager.CreateLayerDisplay(beforeImage, "매핑", true);
        }

  
        public void DrawMatchingChipsOnGrid(Bitmap bitmap, int gridWidth, int gridHeight, List<CResultBlob> matchingChips)
        {
            using (var g = Graphics.FromImage(bitmap))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;

                // 수평 라인 그리기
                for (int y = 0; y <= height; y += gridHeight)
                {
                    g.DrawLine(System.Drawing.Pens.Yellow, new System.Drawing.Point(0, y), new System.Drawing.Point(width, y));
                }

                // 수직 라인 그리기
                for (int x = 0; x <= width; x += gridWidth)
                {
                    g.DrawLine(System.Drawing.Pens.Yellow, new System.Drawing.Point(x, 0), new System.Drawing.Point(x, height));
                }

                // 매핑된 칩 그리기
                var brush = new SolidBrush(System.Drawing.Color.Red);
                foreach (var chip in matchingChips)
                {
                    var rect = chip.Bounding;
                    g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
                }

                g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), ConvertPointToPoint2f(Aftcontour));
                g.DrawLines(new System.Drawing.Pen(System.Drawing.Color.Aquamarine, 1), ConvertPointToPoint2f(beforeContour));
            }
        }

        public void DrawGridLines(Bitmap bitmap, int gridWidth, int gridHeight, System.Drawing.Color color)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (System.Drawing.Pen pen = new System.Drawing.Pen(color))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;

                // 수평 라인 그리기
                for (int y = 0; y <= height; y += gridHeight)
                {
                    graphics.DrawLine(pen, new System.Drawing.Point(0, y), new System.Drawing.Point(width, y));
                }

                // 수직 라인 그리기
                for (int x = 0; x <= width; x += gridWidth)
                {
                    graphics.DrawLine(pen, new System.Drawing.Point(x, 0), new System.Drawing.Point(x, height));
                }
            }
        }

        public List<CResultBlob> GetNonMatchingChips(List<CResultBlob> beforeChips, List<CResultBlob> afterChips, int maxDistance)
        {
            var matchingChips = new List<CResultBlob>();

            foreach (var beforeChip in beforeChips)
            {
                // 가장 가까운 제거 후 칩 찾기
                double minDistance = double.MaxValue;
                CResultBlob nearestAfterChip = null;
                Parallel.ForEach(afterChips, afterChip2 =>
                {
                    double distance = GetDistance(beforeChip.Center, afterChip2.Center);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestAfterChip = afterChip2;
                    }
                });
                //foreach (var afterChip2 in afterChips)
                //{

                //}

                // 일정 거리 이하인 경우 매칭된 칩으로 인식
                if (minDistance <= maxDistance)
                {
                    //  matchingChips.Add(afterChip);
                    //matchingChips.Add(nearestAfterChip);

                    // 이미 매칭된 제거 후 칩은 제거
                    afterChips.Remove(nearestAfterChip);
                }
                else
                {
                    matchingChips.Add(beforeChip);
                }
            }

            return matchingChips;
        }

        public List<CResultBlob> GetMatchingChips(List<CResultBlob> beforeChips, List<CResultBlob> afterChips, int maxDistance)
        {
            var matchingChips = new List<CResultBlob>();

            foreach (var afterChip in afterChips)
            {
                // 가장 가까운 제거 후 칩 찾기
                double minDistance = double.MaxValue;
                CResultBlob nearestAfterChip = null;

                Parallel.ForEach(beforeChips, beforeChip2 =>
                {
                    double distance = GetDistance(afterChip.Center, beforeChip2.Center);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestAfterChip = afterChip;
                    }
                });

                // 일정 거리 이하인 경우 매칭된 칩으로 인식
                if (minDistance <= maxDistance)
                {
                    //  matchingChips.Add(afterChip);
                    matchingChips.Add(nearestAfterChip);

                    // 이미 매칭된 제거 후 칩은 제거
                    //afterChips.Remove(nearestAfterChip);
                }
                else
                {
                    matchingChips.Add(afterChip);
                }
            }

            return matchingChips;
        }

        private void rjPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                using (Mat ImageCVSource = CDisplayManager.ImageSrc.Clone())
                //using (Mat ImageCVSource = Cv2.ImRead(Application.StartupPath + "\\OriginImage\\Chip 제거 전 원본 이미지 1.png"))
                //using (Mat ImageCVSource = Cv2.ImRead(test))
                {
                    COpenCVHelper.SetImageChannel1(ImageCVSource);
                    Bitmap Result = CDrawBitmap.GetBitmapFormat24bppRgb(Lib.Common.CImageConverter.ToBitmap(ImageCVSource));                    
                    CVSIFT cVSIFT = new CVSIFT();
                    cVSIFT.SetSourceImage(ImageCVSource);
                    if(File.Exists(CVisionTools.Features[DEFINE.CAM_1].PATTERN_PATH))
                    {
                        CVisionTools.Features[DEFINE.CAM_1].ImageTemplate= Cv2.ImRead(CVisionTools.Features[DEFINE.CAM_1].PATTERN_PATH);
                    }
                    cVSIFT.SetTemplateImage(CVisionTools.Features[DEFINE.CAM_1].ImageTemplate);
                    cVSIFT.SetProperty(CVisionTools.Features[DEFINE.CAM_1]);
                    cVSIFT.Run();

                    using (Graphics g = Graphics.FromImage(Result))
                    {
                        foreach (var item in cVSIFT.results)
                        {
                            RotateDraw(g, item, (float)item.Angle, new System.Drawing.Pen(System.Drawing.Color.Red, 1));
                        }

                        if (cVSIFT.results.Count == 0)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(cVSIFT.property.CvROI));
                            g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)cVSIFT.property.CvROI.X - 20, (int)cVSIFT.property.CvROI.Y - 20);
                        }
                    }

                    CDisplayManager.TackTime = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
                    CDisplayManager.CreateLayerDisplay(Result, "SIFT", true);
                }
            }
            catch (Exception Desc)
            {
                CLOG.ABNORMAL($"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
            }
        }

        public void RotateDraw(Graphics g, CResultMatching item, float angle, System.Drawing.Pen pen)
        {
            using (System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix())
            {
                RectangleF r = item.Bounding;
                m.RotateAt(angle, new PointF(r.Left + (r.Width / 2), r.Top + (r.Height / 2)));
                g.Transform = m;
                g.DrawRectangles(pen, new[] { r });
                g.DrawLine(pen, r.X + r.Width / 2, r.Y, r.X + r.Width / 2, r.Y + r.Height);
                g.DrawLine(pen, r.X, r.Y + r.Height / 2, r.X + r.Width, r.Y + r.Height / 2);
                g.DrawString((item.Index + 1).ToString(), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 20, (int)item.Bounding.Y - 20);
                g.DrawString(item.Score.ToString("F3"), new Font("Arial", 15, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Center.X, (int)item.Center.Y);
                g.ResetTransform();
            }
        }
    }
}
