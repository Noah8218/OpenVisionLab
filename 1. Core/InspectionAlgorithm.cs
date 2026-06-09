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
    public static class InspectionAlgorithm
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
        public static (LineGaugeTool, LineGaugeTool, OpenCvSharp.Point) RunIntersectionFittingLines(Mat ImageCVSource, LineGaugeProperty line1, LineGaugeProperty line2)
        {
            LineGaugeTool edgeToolL = new LineGaugeTool();
            LineGaugeTool edgeToolR = new LineGaugeTool();
            edgeToolL.SetProperty((LineGaugeProperty)line1.DeepCopy());
            edgeToolR.SetProperty((LineGaugeProperty)line2.DeepCopy());
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

            return (edgeToolL, edgeToolR, VerticalLineCalculator.GetIntersectionLines(edgeToolL.resultList[0].FitLine, edgeToolR.resultList[0].FitLine));
        }

        public static (LineGaugeTool, LineGaugeTool, List<LineGaugeVerticalLines>) RunIntersectionLines(Mat ImageCVSource, LineGaugeProperty line1, LineGaugeProperty line2)
        {
            LineGaugeTool edgeToolL = new LineGaugeTool();
            LineGaugeTool edgeToolR = new LineGaugeTool();
            edgeToolL.SetProperty((LineGaugeProperty)line1.DeepCopy());
            edgeToolR.SetProperty((LineGaugeProperty)line2.DeepCopy());
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

            List<LineGaugeVerticalLines> intersectionLines = new List<LineGaugeVerticalLines>();
            for (int i = 0; i < edgeToolL.resultList.Count; i++)
            {
                List<LineSegment2D> verticalLines = edgeToolL.property.USE_MANUAL_ANGLE ? VerticalLineCalculator.GetVerticalLinesManual(edgeToolL.resultList[i].edgeList, edgeToolL.size.Width, edgeToolL.size.Height, edgeToolL.property.MANUAL_ANGLE_VALUE, edgeToolL.property.VER_PRJ_DIR) : VerticalLineCalculator.GetVerticalLines(edgeToolL.resultList[i].edgeList, edgeToolL.size.Width, edgeToolL.size.Height, edgeToolL.property.POINT_RANGE, edgeToolL.property.VER_PRJ_DIR);
                List<LineSegment2D> intersectionLine = VerticalLineCalculator.GetIntersectionLines(verticalLines, edgeToolR.resultList[i].edgeList);
                List<double> intersectionLengths = new List<double>();

                for (int h = 0; h < intersectionLine.Count; h++)
                {
                    var line = intersectionLine[h];
                    OpenCvSharp.Point ptS = new OpenCvSharp.Point(line.Start.X, line.Start.Y);
                    OpenCvSharp.Point ptE = new OpenCvSharp.Point(line.End.X, line.End.Y);

                    double Distance = ptS.DistanceTo(ptE) * edgeToolL.property.PIXELPERMM;
                    intersectionLengths.Add(Distance);
                }

                intersectionLines.Add(new LineGaugeVerticalLines()
                {
                    index = i,
                    intersectionLengths = intersectionLengths,
                    cLines = intersectionLine
                });
            }

            return (edgeToolL, edgeToolR, intersectionLines);
        }

    public static List<LineGaugeResult> RunEdge(Graphics g, Mat ImageCVSource, LineGaugeProperty line1, System.Drawing.Size size, bool USE_MULTI_INSPECTION = false)
    {
        LineGaugeTool edgeTool = new LineGaugeTool();
        edgeTool.SetProperty((LineGaugeProperty)line1.DeepCopy());
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

    //public static (List<BlobResult>, List<BlobResult>, List<BlobResult>) RunSlitterBlob(Graphics g, Mat ImageCVSource, BlobProperty BlobProperty, Rectangle filterBound)
    //{
    //    List<BlobResult> totalResults = new List<BlobResult>();
    //    List<BlobResult> black_Result = new List<BlobResult>();
    //    List<BlobResult> white_Result = new List<BlobResult>();
    //    Rect black_rt = new Rect();
    //    Rect white_rt = new Rect();
    //    bool USE_MULTI_INSPECTION = true;
    //    bool find_Black = true;
    //    var blobTaskB = Task.Run(() =>
    //    {
    //        (black_Result, black_rt) = RunBlob(g, ImageCVSource, BlobProperty, filterBound, ref totalResults, find_Black, USE_MULTI_INSPECTION);
    //    });
    //    var blobTaskW = Task.Run(() =>
    //    {
    //        (white_Result, white_rt) = RunBlob(g, ImageCVSource, BlobProperty, filterBound, ref totalResults, !find_Black, USE_MULTI_INSPECTION);
    //    });

    //    Task.WaitAll(blobTaskB);
    //    Task.WaitAll(blobTaskW);

    //    totalResults.RemoveAll(s => s == null);
    //    black_Result.RemoveAll(s => s == null);
    //    white_Result.RemoveAll(s => s == null);

    //    return (totalResults, black_Result, white_Result);
    //}

    public static void DrawBlobResulte(Graphics g, BlobProperty BlobProperty, List<BlobResult> Results, bool findBlackBlob = true, bool USE_MULTI_INSPECTION = false)
    {
        if (!USE_MULTI_INSPECTION)
        {
            BitmapDrawing.DrawBlob(g, Results, findBlackBlob);
        }
        else
        {
            for (int j = 0; j < BlobProperty.CvROIS.Count; j++)
            {
                BitmapDrawing.DrawBlob(g, Results, findBlackBlob);
            }
        }
        for (int i = 0; i < BlobProperty.CvMASKS.Count; i++)
        { g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Green, 1), CommonConverter.CVRectToRect(BlobProperty.CvMASKS[i])); }
    }

    //public static (List<BlobResult>, Rect) RunBlob(Graphics g, Mat oriImage, BlobProperty property, Rectangle filterBound, ref List<BlobResult> cResultBlobs, bool findBlackBlob = true, bool USE_MULTI_INSPECTION = false)
    //{
    //    List<BlobResult> blobs = new List<BlobResult>();
    //    BlobTool cIVT_CVBlob = new BlobTool();
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
