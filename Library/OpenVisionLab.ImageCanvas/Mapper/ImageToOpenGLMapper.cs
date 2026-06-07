namespace OpenVisionLab.ImageCanvas.Mapping
{
	public static class ImageToOpenGLMapper
	{
		/// <summary>
		/// image 기반 좌표계를 Opengl 좌표계로 변환합니다.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="imageHeight"></param>
		/// <returns></returns>
		public static System.Drawing.PointF ConvertTopLeftToBottomLeft(System.Drawing.PointF point, int imageHeight)
		{
			return new System.Drawing.PointF(point.X, imageHeight - point.Y);
		}

		/// <summary>
		/// OpenGL 좌표계를 image 기반 좌표계로 변환합니다.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="imageHeight"></param>
		/// <returns></returns>
		public static System.Drawing.Rectangle ConvertOpenGLToImageRectangle(System.Drawing.Rectangle rect, int imageHeight)
		{
			// 새 Y 좌표는 이미지 높이에서 기존 Rectangle의 Y 좌표와 높이를 뺀 값으로 설정
			int newY = imageHeight - rect.Y;

			// 새로운 Rectangle 반환
			return new System.Drawing.Rectangle(rect.X, newY, rect.Width, rect.Height);
		}

		/// <summary>
		/// OpenGL 좌표계를 image 기반 좌표계로 변환합니다.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="imageHeight"></param>
		/// <returns></returns>
		public static System.Drawing.RectangleF ConvertOpenGLToImageRectangle(System.Drawing.RectangleF rect, int imageHeight)
		{
			// 새 Y 좌표는 이미지 높이에서 기존 Rectangle의 Y 좌표와 높이를 뺀 값으로 설정
			float newY = imageHeight - rect.Y;

			// 새로운 Rectangle 반환
			return new System.Drawing.RectangleF(rect.X, newY, rect.Width, rect.Height);
		}

		/// <summary>
		/// image 기반 좌표계를 Opengl 좌표계로 변환합니다.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="imageHeight"></param>
		/// <returns></returns>
		public static System.Drawing.Rectangle ConvertImageToOpenGLRectangle(System.Drawing.Rectangle rect, int imageHeight)
		{
			int newY = imageHeight - rect.Y - rect.Height;

			// 새로운 Rectangle 반환
			return new System.Drawing.Rectangle(rect.X, newY, rect.Width, rect.Height);
		}

		/// <summary>
		/// image 기반 좌표계를 Opengl 좌표계로 변환합니다.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="imageHeight"></param>
		/// <returns></returns>
		public static System.Drawing.RectangleF ConvertImageToOpenGLRectangle(System.Drawing.RectangleF rect, int imageHeight)
		{
			float newY = imageHeight - rect.Y - rect.Height;

			// 새로운 Rectangle 반환
			return new System.Drawing.RectangleF(rect.X, newY, rect.Width, rect.Height);
		}
	}
}
