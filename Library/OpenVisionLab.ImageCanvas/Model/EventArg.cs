using OpenVisionLab.ImageCanvas.CanvasShapes;
using OpenVisionLab.ImageCanvas.Overlays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab.ImageCanvas.Model
{
	public class RoiChangedEventArgs
	{
		private bool _handled = false;
		private IEnumerable<PointF> _canvasPoints = new List<PointF>();
		private IEnumerable<Point> _pixelPos = new List<Point>();
		private float _zoomScale = 0;

		public RoiChangedEventArgs()
		{

		}


		public RoiChangedEventArgs(IEnumerable<PointF> canvasPoints, IEnumerable<Point> pixelPos)
		{
			_canvasPoints = canvasPoints;
			_pixelPos = pixelPos;
		}

		public bool Handled => _handled;

		public float ZoomScale
		{
			get { return _zoomScale; }
			set { _zoomScale = value; }
		}

		public IEnumerable<PointF> CanvasPoints
		{
			get
			{
				return _canvasPoints;
			}
			set
			{
				_canvasPoints = value;
			}
		}
		public IEnumerable<Point> PixelPos
		{
			get
			{
				return _pixelPos;
			}
			set
			{
				_pixelPos = value;

			}
		}
		private CanvasRect<float> _roiRect = new CanvasRect<float>();
		public CanvasRect<float> RoiRect
		{
			get => _roiRect;
			set => _roiRect = value;
		}

		private CanvasOverlayItem _group = new CanvasOverlayItem();
		public CanvasOverlayItem Group
		{
			get => _group;
			set => _group = value;
		}
		public override string ToString()
		{
			return String.Format($"Canvas : {String.Join(",", CanvasPoints.Select(x => String.Format($"({x.X},{x.Y})")))}, PixelPos : {String.Join(",", PixelPos.Select(x => String.Format($"({x.X},{x.Y})")))}");
		}
	}

	public class MouseEventArgsEx : System.Windows.Forms.MouseEventArgs
	{
		private bool _handled = false;
		private PointF _canvasPos;
		private PointF _pxelPos;
		private Color _pixelColor;
		public MouseEventArgsEx(MouseButtons button, int clicks, int x, int y, int delta, float canvasX, float canvasY, int pixelX, int pixelY)
			: base(button, clicks, x, y, delta)
		{
			_canvasPos = new PointF(canvasX, canvasY);
			_pxelPos = new Point(pixelX, pixelY);
		}

		public bool Handled
		{
			get
			{
				return _handled;
			}
			set
			{
				_handled = value;
			}
		}

		public PointF CanvasPos
		{
			get
			{
				return _canvasPos;
			}
		}

		public PointF PixelPos
		{
			get
			{
				return _pxelPos;
			}
			set
			{
				_pxelPos = value;
			}
		}

		public Color PixelColor
		{
			get
			{
				return _pixelColor;
			}
			set
			{
				_pixelColor = value;
			}
		}
	}
}
