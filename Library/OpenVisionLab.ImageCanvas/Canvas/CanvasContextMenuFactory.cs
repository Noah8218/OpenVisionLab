using OpenVisionLab.ImageCanvas.SharedViewModels;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace OpenVisionLab.ImageCanvas.Canvas
{
	[SupportedOSPlatform("windows")]
	public static class CanvasContextMenuFactory
	{
		private static readonly Color IconColor = Color.Black;

		public static ContextMenuStrip CreateImageMenu(EventHandler loadImageClicked, EventHandler saveImageClicked)
		{
			ContextMenuStrip menu = new ContextMenuStrip
			{
				ShowImageMargin = true,
				Renderer = new ToolStripProfessionalRenderer(new CanvasContextMenuColorTable())
			};

			menu.Items.Add(CreateItem(MenuItemUtil.GetDescription(EnumImageCanvasItems.LoadImage), CreateImageIcon(), loadImageClicked));
			menu.Items.Add(CreateItem(MenuItemUtil.GetDescription(EnumImageCanvasItems.SaveImage), CreateSaveIcon(), saveImageClicked));

			return menu;
		}

		private static ToolStripMenuItem CreateItem(string text, Image image, EventHandler clicked)
		{
			ToolStripMenuItem item = new ToolStripMenuItem(text, image);
			item.Click += clicked;
			return item;
		}

		private static Bitmap CreateImageIcon()
		{
			Bitmap bitmap = CreateIconBitmap();

			using (Graphics graphics = Graphics.FromImage(bitmap))
			using (Pen pen = new Pen(IconColor, 1.8f))
			using (SolidBrush brush = new SolidBrush(IconColor))
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.DrawRectangle(pen, 3, 3, 18, 18);
				graphics.FillEllipse(brush, 6, 6, 3, 3);

				PointF[] mountain =
				{
					new PointF(4.5f, 19f),
					new PointF(10f, 13f),
					new PointF(13f, 16f),
					new PointF(16f, 11f),
					new PointF(20f, 19f)
				};
				graphics.DrawLines(pen, mountain);
			}

			return bitmap;
		}

		private static Bitmap CreateSaveIcon()
		{
			Bitmap bitmap = CreateIconBitmap();

			using (Graphics graphics = Graphics.FromImage(bitmap))
			using (Pen pen = new Pen(IconColor, 1.8f))
			using (SolidBrush brush = new SolidBrush(IconColor))
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;

				GraphicsPath body = new GraphicsPath();
				body.AddLines(new[]
				{
					new PointF(5, 3),
					new PointF(17, 3),
					new PointF(21, 7),
					new PointF(21, 21),
					new PointF(5, 21)
				});
				body.CloseFigure();
				graphics.DrawPath(pen, body);

				graphics.DrawRectangle(pen, 7, 5, 9, 5);
				graphics.FillEllipse(brush, 10, 14, 6, 6);
			}

			return bitmap;
		}

		private static Bitmap CreateIconBitmap()
		{
			return new Bitmap(24, 24, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		}

		private sealed class CanvasContextMenuColorTable : ProfessionalColorTable
		{
			public override Color MenuItemSelected => Color.FromArgb(236, 241, 247);
			public override Color MenuItemBorder => Color.FromArgb(206, 213, 224);
			public override Color ImageMarginGradientBegin => Color.White;
			public override Color ImageMarginGradientMiddle => Color.White;
			public override Color ImageMarginGradientEnd => Color.White;
		}
	}
}
