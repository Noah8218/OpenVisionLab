using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KtemVisionSystem
{
    public static class CDrawInspection
    {
        public static void DrawBlob(Graphics g, Rect CVROI, List<CResultBlob> result, bool findBlackBlob)
        {
            int count = 1;
            foreach (var item in result)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 1), item.Bounding);
                if (findBlackBlob) { g.DrawString("B-" + (count).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10); }
                else { g.DrawString("W-" + (count).ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Lime), (int)item.Bounding.X - 10, (int)item.Bounding.Y - 10); }

                g.DrawString(item.Area.ToString(), new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Blue), (int)item.Center.X, (int)item.Center.Y);
                count++;
            }

            if (result.Count == 0)
            {
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 5), CConverter.CVRectToRect(CVROI));
                g.DrawString("1", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.OrangeRed), (int)CVROI.X - 20, (int)CVROI.Y - 20);
            }
        }

        public static void DrawPoint(Graphics graphics, PointF point)
        {
            RectangleF rectangle = new RectangleF(point.X - 3, point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        public static void DrawPoint(Graphics graphics, Point2d point)
        {
            RectangleF rectangle = new RectangleF((float)point.X - 3, (float)point.Y - 3, 6, 6);

            graphics.FillEllipse(Brushes.White, rectangle);

            graphics.DrawEllipse(Pens.Black, rectangle);
        }
    }
}
