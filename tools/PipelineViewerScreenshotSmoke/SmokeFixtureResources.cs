using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Bitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;

internal static class SmokeFixtureResources
{
    internal static Bitmap CreateLineMeasureSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(232, 238, 241));

        using System.Drawing.SolidBrush railBrush = new(DrawingColor.Black);
        using System.Drawing.SolidBrush shadowBrush = new(DrawingColor.FromArgb(186, 199, 205));
        using System.Drawing.SolidBrush laneBrush = new(DrawingColor.FromArgb(248, 251, 252));
        using System.Drawing.Pen guidePen = new(DrawingColor.FromArgb(150, 169, 177), 1);
        using System.Drawing.Pen roiGuidePen = new(DrawingColor.FromArgb(96, 130, 142), 1);

        graphics.FillRectangle(laneBrush, 86, 54, 342, 292);
        graphics.FillRectangle(shadowBrush, 90, 60, 126, 280);
        graphics.FillRectangle(shadowBrush, 296, 60, 132, 280);
        graphics.FillRectangle(railBrush, 96, 70, 74, 252);
        graphics.FillRectangle(railBrush, 340, 70, 74, 252);

        for (int y = 80; y <= 314; y += 18)
        {
            graphics.DrawLine(guidePen, 170, y, 340, y);
        }

        graphics.DrawRectangle(roiGuidePen, 92, 64, 120, 272);
        graphics.DrawRectangle(roiGuidePen, 300, 64, 122, 272);
        return bitmap;
    }

    internal static Bitmap CreateLineIntersectionSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        Random random = new(5317);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                int baseValue = 102 + random.Next(-24, 25);
                if (((x / 9) + (y / 7)) % 2 == 0)
                {
                    baseValue += 8;
                }

                int value = Math.Clamp(baseValue, 62, 146);
                bitmap.SetPixel(x, y, DrawingColor.FromArgb(value, value, value));
            }
        }

        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using System.Drawing.Pen darkShadowPen = new(DrawingColor.FromArgb(72, 76, 76), 14)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        using System.Drawing.Pen softShadowPen = new(DrawingColor.FromArgb(130, 136, 136), 24)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        using System.Drawing.SolidBrush objectBrush = new(DrawingColor.FromArgb(252, 252, 250));
        using System.Drawing.Pen objectEdgePen = new(DrawingColor.FromArgb(236, 238, 236), 2);
        using System.Drawing.Pen roiGuidePen = new(DrawingColor.FromArgb(82, 116, 128), 1);

        System.Drawing.Point[] objectShape =
        {
            new(34, 34),
            new(344, 34),
            new(344, 188),
            new(226, 306),
            new(34, 306)
        };

        graphics.DrawLine(softShadowPen, 352, 44, 352, 190);
        graphics.DrawLine(softShadowPen, 232, 314, 352, 194);
        graphics.DrawLine(softShadowPen, 42, 314, 230, 314);
        graphics.DrawLine(darkShadowPen, 352, 44, 352, 190);
        graphics.DrawLine(darkShadowPen, 232, 314, 352, 194);
        graphics.DrawLine(darkShadowPen, 42, 314, 230, 314);

        graphics.FillPolygon(objectBrush, objectShape);
        graphics.DrawLines(objectEdgePen, new[]
        {
            new System.Drawing.Point(344, 34),
            new System.Drawing.Point(344, 188),
            new System.Drawing.Point(226, 306),
            new System.Drawing.Point(34, 306)
        });

        graphics.DrawRectangle(roiGuidePen, 42, 268, 190, 74);
        graphics.DrawRectangle(roiGuidePen, 306, 44, 86, 142);
        return bitmap;
    }

    internal static Bitmap CreateMatchingSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(226, 234, 239));

        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(28, 38, 52));
        using System.Drawing.SolidBrush midBrush = new(DrawingColor.FromArgb(76, 126, 170));
        using System.Drawing.SolidBrush lightBrush = new(DrawingColor.FromArgb(234, 241, 247));
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(255, 255, 255), 3);
        using System.Drawing.Pen outlinePen = new(DrawingColor.FromArgb(20, 30, 44), 2);

        graphics.FillRectangle(darkBrush, 150, 100, 120, 96);
        graphics.FillEllipse(midBrush, 172, 122, 36, 36);
        graphics.DrawLine(accentPen, 158, 184, 262, 108);
        graphics.DrawRectangle(outlinePen, 150, 100, 120, 96);
        graphics.FillRectangle(lightBrush, 230, 152, 26, 28);

        using System.Drawing.Font font = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Matching", font, textBrush, 34, 16);
        return bitmap;
    }

    internal static Bitmap CreateLargeSmokeBitmap(int width, int height, byte variation = 0)
    {
        Bitmap bitmap = new(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        ColorPalette palette = bitmap.Palette;
        for (int i = 0; i < palette.Entries.Length; i++)
        {
            palette.Entries[i] = DrawingColor.FromArgb(i, i, i);
        }

        bitmap.Palette = palette;

        DrawingRectangle bounds = new(0, 0, width, height);
        BitmapData data = bitmap.LockBits(bounds, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        try
        {
            int stride = data.Stride;
            byte[] row = new byte[stride];
            int periodX = Math.Max(1, width / 32);
            int periodY = Math.Max(1, height / 32);
            int fixtureLeft = width / 5;
            int fixtureRight = width * 4 / 5;
            int fixtureTop = height / 3;
            int fixtureBottom = height * 2 / 3;

            for (int y = 0; y < height; y++)
            {
                int yRamp = y * 96 / Math.Max(1, height - 1);
                bool gridY = y % periodY < 5;
                for (int x = 0; x < width; x++)
                {
                    int xRamp = x * 128 / Math.Max(1, width - 1);
                    int value = 56 + ((xRamp + yRamp) / 2);
                    bool gridX = x % periodX < 5;
                    bool inFixture = x >= fixtureLeft && x <= fixtureRight && y >= fixtureTop && y <= fixtureBottom;
                    bool stripe = ((x - fixtureLeft) / Math.Max(1, width / 96)) % 2 == 0;
                    if (inFixture)
                    {
                        value = stripe ? 210 : 126;
                    }

                    if (gridX || gridY)
                    {
                        value = Math.Min(245, value + 42);
                    }

                    row[x] = (byte)Math.Clamp(value + variation, 0, 255);
                }

                for (int x = width; x < stride; x++)
                {
                    row[x] = 0;
                }

                Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, y * stride), stride);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return bitmap;
    }

    internal static string ComputeStreamingBitmapSha256(Bitmap bitmap)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] metadata = System.Text.Encoding.UTF8.GetBytes(
            $"{bitmap.Width}x{bitmap.Height}:32bppArgb:");
        sha256.TransformBlock(metadata, 0, metadata.Length, metadata, 0);

        DrawingRectangle bounds = new(0, 0, bitmap.Width, bitmap.Height);
        BitmapData data = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);
        try
        {
            int stride = Math.Abs(data.Stride);
            byte[] sourceRow = new byte[stride];
            byte[] normalizedRow = new byte[bitmap.Width * 4];
            ColorPalette palette = bitmap.Palette;
            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), sourceRow, 0, stride);
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        DrawingColor color = palette.Entries[sourceRow[x]];
                        int offset = x * 4;
                        normalizedRow[offset] = color.B;
                        normalizedRow[offset + 1] = color.G;
                        normalizedRow[offset + 2] = color.R;
                        normalizedRow[offset + 3] = color.A;
                    }
                }
                else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int sourceOffset = x * 3;
                        int targetOffset = x * 4;
                        normalizedRow[targetOffset] = sourceRow[sourceOffset];
                        normalizedRow[targetOffset + 1] = sourceRow[sourceOffset + 1];
                        normalizedRow[targetOffset + 2] = sourceRow[sourceOffset + 2];
                        normalizedRow[targetOffset + 3] = byte.MaxValue;
                    }
                }
                else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    Buffer.BlockCopy(sourceRow, 0, normalizedRow, 0, normalizedRow.Length);
                }
                else
                {
                    throw new NotSupportedException(
                        "Streaming smoke hash does not support " + bitmap.PixelFormat + ".");
                }

                sha256.TransformBlock(normalizedRow, 0, normalizedRow.Length, normalizedRow, 0);
            }

            sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return Convert.ToHexString(sha256.Hash!);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

    internal static string CreateMatchingTemplateFile(Bitmap source)
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_matching_smoke_template_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap template = source.Clone(new DrawingRectangle(150, 100, 120, 96), source.PixelFormat);
        template.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    internal static List<string> CreateAutoMPointRepresentativeFiles(Bitmap source, int count)
    {
        List<string> paths = new List<string>();
        for (int index = 0; index < count; index++)
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                "OpenVisionLab_auto_mpoint_representative_"
                + index.ToString(CultureInfo.InvariantCulture)
                + "_"
                + Guid.NewGuid().ToString("N")
                + ".png");
            source.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            paths.Add(path);
        }

        return paths;
    }

    internal static string CreateWorkspaceLoadSmokeImageFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_workspace_load_smoke_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap bitmap = new(640, 360);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(236, 242, 244));

        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(33, 48, 58));
        using System.Drawing.SolidBrush accentBrush = new(DrawingColor.FromArgb(21, 124, 134));
        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(166, 188, 196), 1);
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(21, 124, 134), 4);
        for (int x = 40; x < 600; x += 40)
        {
            graphics.DrawLine(gridPen, x, 40, x, 320);
        }

        for (int y = 40; y < 320; y += 40)
        {
            graphics.DrawLine(gridPen, 40, y, 600, y);
        }

        graphics.FillRectangle(darkBrush, 180, 128, 180, 86);
        graphics.FillEllipse(accentBrush, 92, 126, 74, 74);
        graphics.DrawEllipse(accentPen, 438, 116, 104, 104);

        using System.Drawing.Font font = new("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Loaded Main Image", font, textBrush, 44, 20);
        bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    internal static Bitmap CreateWorkspaceSeedSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(226, 234, 239));

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(132, 160, 170), 1);
        for (int x = 32; x < bitmap.Width; x += 32)
        {
            graphics.DrawLine(gridPen, x, 32, x, bitmap.Height - 32);
        }

        for (int y = 32; y < bitmap.Height; y += 32)
        {
            graphics.DrawLine(gridPen, 32, y, bitmap.Width - 32, y);
        }

        using System.Drawing.SolidBrush shapeBrush = new(DrawingColor.FromArgb(49, 65, 72));
        using System.Drawing.Pen accentPen = new(DrawingColor.FromArgb(0, 167, 179), 4);
        graphics.FillEllipse(shapeBrush, 84, 138, 58, 58);
        graphics.FillRectangle(shapeBrush, 198, 132, 140, 72);
        graphics.DrawEllipse(accentPen, 382, 130, 70, 70);

        using System.Drawing.Font font = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("OpenVisionLab", font, textBrush, 34, 12);
        return bitmap;
    }

    internal static Bitmap CreateDockingPanelSmokeBitmap(int index)
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        DrawingColor background = index switch
        {
            1 => DrawingColor.FromArgb(235, 242, 245),
            2 => DrawingColor.FromArgb(231, 239, 232),
            3 => DrawingColor.FromArgb(243, 236, 230),
            _ => DrawingColor.FromArgb(235, 236, 244)
        };
        graphics.Clear(background);

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(130, 152, 160), 1);
        for (int x = 28; x < bitmap.Width; x += 36)
        {
            graphics.DrawLine(gridPen, x, 26, x, bitmap.Height - 28);
        }

        for (int y = 28; y < bitmap.Height; y += 36)
        {
            graphics.DrawLine(gridPen, 28, y, bitmap.Width - 28, y);
        }

        DrawingColor accent = index switch
        {
            1 => DrawingColor.FromArgb(16, 133, 142),
            2 => DrawingColor.FromArgb(70, 130, 80),
            3 => DrawingColor.FromArgb(174, 103, 52),
            _ => DrawingColor.FromArgb(88, 96, 172)
        };

        using System.Drawing.SolidBrush accentBrush = new(accent);
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(34, 45, 55));
        using System.Drawing.Pen accentPen = new(accent, 5);
        graphics.FillRectangle(darkBrush, 76, 116, 130, 92);
        graphics.DrawEllipse(accentPen, 270, 92, 112, 112);
        graphics.FillEllipse(accentBrush, 330, 236, 52, 52);
        graphics.DrawLine(accentPen, 82, 282, 428, 116);

        using System.Drawing.Font titleFont = new("Segoe UI", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.Font captionFont = new("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        graphics.DrawString("Dock " + index.ToString(CultureInfo.InvariantCulture), titleFont, darkBrush, 34, 20);
        graphics.DrawString("Panel split smoke", captionFont, darkBrush, 34, 338);
        return bitmap;
    }

    internal static Bitmap CreateFeatureMatchingSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(232, 238, 242));

        DrawingRectangle fixture = new(132, 82, 190, 142);
        using System.Drawing.SolidBrush panelBrush = new(DrawingColor.FromArgb(248, 250, 252));
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(24, 35, 49));
        using System.Drawing.SolidBrush blueBrush = new(DrawingColor.FromArgb(48, 116, 170));
        using System.Drawing.SolidBrush tealBrush = new(DrawingColor.FromArgb(20, 134, 142));
        using System.Drawing.Pen darkPen = new(DrawingColor.FromArgb(24, 35, 49), 2);
        using System.Drawing.Pen bluePen = new(DrawingColor.FromArgb(48, 116, 170), 2);
        using System.Drawing.Font titleFont = new("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        using System.Drawing.Font smallFont = new("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

        graphics.FillRectangle(panelBrush, fixture);
        graphics.DrawRectangle(darkPen, fixture);
        graphics.DrawString("F7", titleFont, darkBrush, fixture.X + 14, fixture.Y + 12);
        graphics.DrawString("SIFT", smallFont, blueBrush, fixture.X + 118, fixture.Y + 18);
        graphics.DrawLine(bluePen, fixture.X + 16, fixture.Y + 98, fixture.X + 170, fixture.Y + 30);
        graphics.DrawEllipse(darkPen, fixture.X + 24, fixture.Y + 74, 34, 34);
        graphics.FillEllipse(tealBrush, fixture.X + 68, fixture.Y + 70, 18, 18);
        graphics.FillRectangle(darkBrush, fixture.X + 128, fixture.Y + 78, 32, 26);

        using System.Drawing.Pen gridPen = new(DrawingColor.FromArgb(94, 108, 122), 1);
        for (int x = fixture.X + 8; x < fixture.Right - 8; x += 18)
        {
            graphics.DrawLine(gridPen, x, fixture.Y + 116, x + 8, fixture.Y + 132);
        }

        Random random = new(17);
        for (int i = 0; i < 38; i++)
        {
            int x = random.Next(fixture.X + 8, fixture.Right - 10);
            int y = random.Next(fixture.Y + 8, fixture.Bottom - 10);
            using System.Drawing.SolidBrush dotBrush = new(i % 3 == 0 ? DrawingColor.FromArgb(24, 35, 49) : DrawingColor.FromArgb(48, 116, 170));
            graphics.FillEllipse(dotBrush, x, y, 3 + (i % 3), 3 + (i % 3));
        }

        using System.Drawing.SolidBrush textBrush = new(DrawingColor.FromArgb(36, 48, 64));
        graphics.DrawString("Feature Matching", smallFont, textBrush, 34, 16);
        return bitmap;
    }

    internal static string CreateFeatureMatchingTemplateFile(Bitmap source)
    {
        string path = Path.Combine(Path.GetTempPath(), "OpenVisionLab_feature_matching_smoke_template_" + Guid.NewGuid().ToString("N") + ".png");
        using Bitmap template = source.Clone(new DrawingRectangle(132, 82, 190, 142), source.PixelFormat);
        template.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }

    internal static void TryDeleteFile(string path)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
        }
    }

    internal static Bitmap CreateRoiSmokeBitmap()
    {
        Bitmap bitmap = new(512, 384);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(DrawingColor.FromArgb(28, 31, 34));

        using System.Drawing.Pen railPen = new(DrawingColor.FromArgb(190, 210, 214), 4);
        using System.Drawing.Pen tracePen = new(DrawingColor.FromArgb(130, 177, 188), 2);
        using System.Drawing.SolidBrush padBrush = new(DrawingColor.FromArgb(225, 230, 230));
        using System.Drawing.SolidBrush darkBrush = new(DrawingColor.FromArgb(50, 62, 68));
        using System.Drawing.SolidBrush brightBrush = new(DrawingColor.FromArgb(245, 248, 248));

        for (int i = 0; i < 7; i++)
        {
            int x = 72 + i * 52;
            graphics.FillRectangle(padBrush, x, 58, 24, 44);
            graphics.FillRectangle(darkBrush, x + 5, 68, 14, 24);
        }

        graphics.DrawLine(railPen, 42, 180, 470, 260);
        graphics.DrawLine(railPen, 32, 224, 456, 312);
        graphics.DrawLine(tracePen, 72, 206, 418, 274);
        graphics.DrawLine(tracePen, 92, 244, 426, 304);
        graphics.FillEllipse(brightBrush, 318, 190, 24, 24);
        graphics.FillEllipse(brightBrush, 372, 214, 18, 18);
        return bitmap;
    }

}
