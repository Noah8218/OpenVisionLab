using Lib.Common;
using Lib.Line;
using Lib.OpenCV;
using Lib.OpenCV.Blob;
using Lib.OpenCV.Result;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace OpenVisionLab
{
    public static class CInspectionAlgorithm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ImageCVSource"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="USE_MULTI_INSPECTION"></param>
        /// <param name="UseSpec"></param>
        /// <returns></returns>
        public static (CVLineGuage, CVLineGuage, OpenCvSharp.Point) RunIntersectionFittingLines(Mat ImageCVSource, CPropertyLineGuage line1, CPropertyLineGuage line2)
        {
            CVLineGuage edgeToolL = new CVLineGuage();
            CVLineGuage edgeToolR = new CVLineGuage();
            edgeToolL.SetProperty((CPropertyLineGuage)line1.DeepCopy());
            edgeToolR.SetProperty((CPropertyLineGuage)line2.DeepCopy());
            edgeToolL.SetSourceImage(ImageCVSource);
            edgeToolR.SetSourceImage(ImageCVSource);
            var lineTask = Task.Run(() =>
            {
                edgeToolL.Run();
            });

            var lineTask2 = Task.Run(() =>
            {
                edgeToolR.Run();
            });

            Task.WaitAll(lineTask, lineTask2);

            return (edgeToolL, edgeToolR, CLineVertical.GetIntersectionLines(edgeToolL.resultList[0].FitLine, edgeToolR.resultList[0].FitLine));
        }

        public static (CVLineGuage, CVLineGuage, List<CVLineGuage_VerticalLines>) RunIntersectionLines(Mat ImageCVSource, CPropertyLineGuage line1, CPropertyLineGuage line2)
        {
            CVLineGuage edgeToolL = new CVLineGuage();
            CVLineGuage edgeToolR = new CVLineGuage();
            edgeToolL.SetProperty((CPropertyLineGuage)line1.DeepCopy());
            edgeToolR.SetProperty((CPropertyLineGuage)line2.DeepCopy());
            edgeToolL.SetSourceImage(ImageCVSource);
            edgeToolR.SetSourceImage(ImageCVSource);
            var lineTask = Task.Run(() =>
            {
                edgeToolL.Run();
            });

            var lineTask2 = Task.Run(() =>
            {
                edgeToolR.Run();
            });

            Task.WaitAll(lineTask, lineTask2);

            List<CVLineGuage_VerticalLines> intersectionLines = new List<CVLineGuage_VerticalLines>();
            for (int i = 0; i < edgeToolL.resultList.Count; i++)
            {
                List<CLine> verticalLines = edgeToolL.property.USE_MANUAL_ANGLE ? CLineVertical.GetVerticalLinesManual(edgeToolL.resultList[i].edgeList, edgeToolL.size.Width, edgeToolL.size.Height, edgeToolL.property.MANUAL_ANGLE_VALUE, edgeToolL.property.VER_PRJ_DIR) : CLineVertical.GetVerticalLines(edgeToolL.resultList[i].edgeList, edgeToolL.size.Width, edgeToolL.size.Height, edgeToolL.property.POINT_RANGE, edgeToolL.property.VER_PRJ_DIR);
                List<CLine> intersectionLine = CLineVertical.GetIntersectionLines(verticalLines, edgeToolR.resultList[i].edgeList);
                List<double> intersectionLengths = new List<double>();

                for (int h = 0; h < intersectionLine.Count; h++)
                {
                    var line = intersectionLine[h];
                    OpenCvSharp.Point ptS = new OpenCvSharp.Point(line.Start.X, line.Start.Y);
                    OpenCvSharp.Point ptE = new OpenCvSharp.Point(line.End.X, line.End.Y);

                    double Distance = ptS.DistanceTo(ptE) * edgeToolL.property.PIXELPERMM;
                    intersectionLengths.Add(Distance);
                }

                intersectionLines.Add(new CVLineGuage_VerticalLines()
                {
                    index = i,
                    intersectionLengths = intersectionLengths,
                    cLines = intersectionLine
                });
            }

            return (edgeToolL, edgeToolR, intersectionLines);
        }

    public static List<CVLineGuage_Result> RunEdge(Graphics g, Mat ImageCVSource, CPropertyLineGuage line1, System.Drawing.Size size, bool USE_MULTI_INSPECTION = false)
    {
        CVLineGuage edgeTool = new CVLineGuage();
        edgeTool.SetProperty((CPropertyLineGuage)line1.DeepCopy());
        edgeTool.SetSourceImage(ImageCVSource);

        if (!USE_MULTI_INSPECTION)
        {
            edgeTool.Run();
            //edgeTool.Draw(g, edgeTool.resultList[0].edgeList, size);
        }
        else
        {
            for (int j = 0; j < edgeTool.property.CvROIS.Count; j++)
            {
                edgeTool.property.CvROI = edgeTool.property.CvROIS[j];
                edgeTool.Run();
                //edgeTool.Draw(g, edgeTool.resultList[j].edgeList, size);
            }
        }

        return edgeTool.resultList;
    }

    //public static (List<CResultBlob>, List<CResultBlob>, List<CResultBlob>) RunSlitterBlob(Graphics g, Mat ImageCVSource, CPropertyBlob CPropertyBlob, Rectangle filterBound)
    //{
    //    List<CResultBlob> totalResults = new List<CResultBlob>();
    //    List<CResultBlob> black_Result = new List<CResultBlob>();
    //    List<CResultBlob> white_Result = new List<CResultBlob>();
    //    Rect black_rt = new Rect();
    //    Rect white_rt = new Rect();
    //    bool USE_MULTI_INSPECTION = true;
    //    bool find_Black = true;
    //    var blobTaskB = Task.Run(() =>
    //    {
    //        (black_Result, black_rt) = RunBlob(g, ImageCVSource, CPropertyBlob, filterBound, ref totalResults, find_Black, USE_MULTI_INSPECTION);
    //    });
    //    var blobTaskW = Task.Run(() =>
    //    {
    //        (white_Result, white_rt) = RunBlob(g, ImageCVSource, CPropertyBlob, filterBound, ref totalResults, !find_Black, USE_MULTI_INSPECTION);
    //    });

    //    Task.WaitAll(blobTaskB);
    //    Task.WaitAll(blobTaskW);

    //    totalResults.RemoveAll(s => s == null);
    //    black_Result.RemoveAll(s => s == null);
    //    white_Result.RemoveAll(s => s == null);

    //    return (totalResults, black_Result, white_Result);
    //}

    public static void DrawBlobResulte(Graphics g, CPropertyBlob CPropertyBlob, List<CResultBlob> Results, bool findBlackBlob = true, bool USE_MULTI_INSPECTION = false)
    {
        if (!USE_MULTI_INSPECTION)
        {
            CDrawBitmap.DrawBlob(g, Results, findBlackBlob);
        }
        else
        {
            for (int j = 0; j < CPropertyBlob.CvROIS.Count; j++)
            {
                CDrawBitmap.DrawBlob(g, Results, findBlackBlob);
            }
        }
        for (int i = 0; i < CPropertyBlob.CvMASKS.Count; i++)
        { g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Green, 1), CConverter.CVRectToRect(CPropertyBlob.CvMASKS[i])); }
    }

    //public static (List<CResultBlob>, Rect) RunBlob(Graphics g, Mat oriImage, CPropertyBlob property, Rectangle filterBound, ref List<CResultBlob> cResultBlobs, bool findBlackBlob = true, bool USE_MULTI_INSPECTION = false)
    //{
    //    List<CResultBlob> blobs = new List<CResultBlob>();
    //    CVBlob cIVT_CVBlob = new CVBlob();
    //    cIVT_CVBlob.SetProperty(property.DeepCopy());
    //    if (!findBlackBlob) { cIVT_CVBlob.property.USE_BITWISENOT = !cIVT_CVBlob.property.USE_BITWISENOT; }
    //    cIVT_CVBlob.SetSourceImage(oriImage);

    //    if (!USE_MULTI_INSPECTION)
    //    {
    //        //cIVT_CVBlob.Run(filterBound, findBlackBlob);
    //    }
    //    else
    //    {
    //        for (int nReelIndex = 0; nReelIndex < cIVT_CVBlob.property.CvROIS.Count; nReelIndex++)
    //        {
    //            //var blobTask = Task.Run(() =>
    //            //{
    //            //    cIVT_CVBlob.Property.CvROI = cIVT_CVBlob.Property.CvROIS[nReelIndex];
    //            //    cIVT_CVBlob.Run(filterBound, findBlackBlob);
    //            //});

    //            //Task.WaitAll(blobTask);

    //            //for (int i = 0; i < cIVT_CVBlob.Results.Count; i++)
    //            //{
    //            //    cIVT_CVBlob.Results[i].ReelIndex = nReelIndex;
    //            //    cResultBlobs.Add(cIVT_CVBlob.Results[i]);
    //            //    blobs.Add(cIVT_CVBlob.Results[i]);
    //            //}
    //        }
    //    }

    //    return (blobs, cIVT_CVBlob.property.CvROI);
    //}
}
}
