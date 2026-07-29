using System;
using System.Drawing;

namespace OpenVisionLab
{
    internal static class OpenVisionPipelineReviewInstanceRenderService
    {
        public static Bitmap CreateHighlight(
            Bitmap sourceImage,
            VisionPipelineInstanceResult item)
        {
            if (sourceImage == null || item == null)
            {
                return null;
            }

            var highlighted = new Bitmap(sourceImage);
            using Graphics graphics = Graphics.FromImage(highlighted);
            graphics.SmoothingMode =
                System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Color color = item.Accepted ? Color.LimeGreen : Color.OrangeRed;
            float thickness = Math.Max(
                2F,
                Math.Min(highlighted.Width, highlighted.Height) / 150F);
            using Pen pen = new Pen(color, thickness);
            System.Drawing.Drawing2D.GraphicsState state = graphics.Save();
            graphics.TranslateTransform(
                (float)item.RoiCenterX,
                (float)item.RoiCenterY);
            graphics.RotateTransform((float)-item.RoiAngle);
            graphics.DrawRectangle(
                pen,
                (float)(-item.RoiWidth / 2D),
                (float)(-item.RoiHeight / 2D),
                (float)Math.Max(1D, item.RoiWidth),
                (float)Math.Max(1D, item.RoiHeight));
            graphics.Restore(state);

            float cross = Math.Max(6F, thickness * 2F);
            graphics.DrawLine(
                pen,
                (float)item.RoiCenterX - cross,
                (float)item.RoiCenterY,
                (float)item.RoiCenterX + cross,
                (float)item.RoiCenterY);
            graphics.DrawLine(
                pen,
                (float)item.RoiCenterX,
                (float)item.RoiCenterY - cross,
                (float)item.RoiCenterX,
                (float)item.RoiCenterY + cross);
            using Font font = new Font(
                "Segoe UI",
                Math.Max(9F, cross * 1.4F),
                FontStyle.Bold,
                GraphicsUnit.Pixel);
            using Brush brush = new SolidBrush(color);
            graphics.DrawString(
                item.InstanceId ?? string.Empty,
                font,
                brush,
                (float)item.RoiCenterX + cross + 2F,
                (float)item.RoiCenterY - cross);
            return highlighted;
        }
    }
}
