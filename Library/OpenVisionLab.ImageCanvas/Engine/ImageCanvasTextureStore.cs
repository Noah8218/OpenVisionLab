using OpenVisionLab.ImageCanvas.OpenGLRendering;
using SharpGL;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab.ImageCanvas.Rendering
{
	internal static class ImageCanvasTextureStore
	{
		public static List<uint> CollectTextureIds(IEnumerable<OpenGlTextureDrawingParam> textureParams)
		{
			if (textureParams == null)
			{
				return new List<uint>();
			}

			return textureParams
				.SelectMany(param => new[]
				{
					param.OriTextureId,
					param.OriBackgroundTextureId,
					param.MaskTextureId,
					param.MaskBackgroundMaskTextureId,
					param.TransparentBackgroundTextureId,
					param.ThresholdTextureId
				})
				.Where(textureId => textureId != 0)
				.Distinct()
				.ToList();
		}

		public static List<uint> CollectTextureIds(IEnumerable<IEnumerable<OpenGlTextureDrawingParam>> textureGroups)
		{
			if (textureGroups == null)
			{
				return new List<uint>();
			}

			return CollectTextureIds(textureGroups.SelectMany(textureParams => textureParams));
		}

		public static void DeleteTextures(OpenGL gl, IEnumerable<uint> textureIds)
		{
			if (gl == null || textureIds == null)
			{
				return;
			}

			uint[] ids = textureIds.Where(textureId => textureId != 0).Distinct().ToArray();
			if (ids.Length == 0)
			{
				return;
			}

			gl.DeleteTextures(ids.Length, ids);
		}
	}
}
