namespace OpenVisionLab.ImageCanvas.OpenGLRendering
{
	public class OpenGlFontRenderOptions
	{
		// RGB 색상 값
		public float R { get; set; }
		public float G { get; set; }
		public float B { get; set; }

		// 폰트 이름
		public string FontName { get; set; } = "Arial";

		// 폰트 크기
		public float FontSize { get; set; } = 12.0f;

		// 표시할 텍스트 문자열
		public string Text { get; set; } = "";

		public System.Drawing.Color Color
		{
			get => ToSystemDrawingColor();
		}

		// 기본 생성자
		public OpenGlFontRenderOptions()
		{

		}

		// 모든 프로퍼티를 매개변수로 받는 생성자
		public OpenGlFontRenderOptions(float r, float g, float b, string fontName, float fontSize, string text)
		{
			R = r;
			G = g;
			B = b;
			FontName = fontName;
			FontSize = fontSize;
			Text = text;
		}

		public OpenGlFontRenderOptions(System.Drawing.Color color, string fontName, float fontSize, string text)
		{
			float r, g, b, a = 1.0f; // 빨간색				
			(r, g, b, a) = OpenGlDrawing.ConvertColorToOpenGLRGB(color);

			R = r;
			G = g;
			B = b;
			FontName = fontName;
			FontSize = fontSize;
			Text = text;
		}

		public System.Drawing.Color ToSystemDrawingColor()
		{
			// R, G, B 값을 0에서 255 사이의 정수로 변환
			int r = (int)(R * 255);
			int g = (int)(G * 255);
			int b = (int)(B * 255);

			// 변환된 값으로 Color 객체 생성
			return System.Drawing.Color.FromArgb(r, g, b);
		}
	}
}
