using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenVisionLab.ImageCanvas.External
{
	/// <summary>
	/// The small read-only DTO needed by an ImageCanvas consumer to display the
	/// 3D-to-2D side of the cross-modal projection contract.
	/// </summary>
	public sealed class ImageCanvasExternalProjectedPoint
	{
		public string Direction { get; set; }
		public string Id { get; set; }
		public string Kind { get; set; }
		public string Label { get; set; }
		public double ImageX { get; set; }
		public double ImageY { get; set; }
		public double GridX { get; set; }
		public double GridY { get; set; }
		public double? SampledHeight { get; set; }
		public string SampleStatus { get; set; }
		public string InspectionStatus { get; set; }
	}

	public sealed class ImageCanvasExternalProjectionResult
	{
		public string SchemaVersion { get; set; }
		public string ProjectionId { get; set; }
		public string TwoDTransactionId { get; set; }
		public string ThreeDTransactionId { get; set; }
		public string Outcome { get; set; }
		public string TwoDRunId { get; set; }
		public string ThreeDRunId { get; set; }
		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }
		public int GridWidth { get; set; }
		public int GridHeight { get; set; }
		public List<ImageCanvasExternalProjectedPoint> TwoDToThreeD { get; set; }
		public List<ImageCanvasExternalProjectedPoint> ThreeDToTwoD { get; set; }
		public DateTimeOffset RecordedAtUtc { get; set; }
	}

	/// <summary>
	/// Reads only the stable projection evidence. It intentionally does not
	/// reference the OpenVisionLab application or the 3D project.
	/// </summary>
	public static class ImageCanvasExternalProjectionResultReader
	{
		private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};

		public static ImageCanvasExternalProjectionResult Read(
			string path,
			string expectedTwoDTransactionId,
			int expectedImageWidth,
			int expectedImageHeight)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(path);
			ArgumentException.ThrowIfNullOrWhiteSpace(expectedTwoDTransactionId);
			if (expectedImageWidth <= 1 || expectedImageHeight <= 1)
			{
				throw new ArgumentOutOfRangeException(nameof(expectedImageWidth), "Expected image dimensions must be greater than one.");
			}
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Projection result evidence was not found.", path);
			}

			ImageCanvasExternalProjectionResult result = JsonSerializer.Deserialize<ImageCanvasExternalProjectionResult>(
				File.ReadAllText(path),
				JsonOptions);
			Validate(result, expectedTwoDTransactionId, expectedImageWidth, expectedImageHeight);
			return result;
		}

		public static IReadOnlyList<ImageCanvasExternalProjectedPoint> GetVisibleThreeDToTwoD(
			ImageCanvasExternalProjectionResult result)
		{
			if (result == null)
			{
				return Array.Empty<ImageCanvasExternalProjectedPoint>();
			}

			return (result.ThreeDToTwoD ?? new List<ImageCanvasExternalProjectedPoint>())
				.Where(point => point != null
					&& string.Equals(point.SampleStatus, "Valid", StringComparison.OrdinalIgnoreCase)
					&& point.ImageX >= 0
					&& point.ImageX < result.ImageWidth
					&& point.ImageY >= 0
					&& point.ImageY < result.ImageHeight)
				.ToArray();
		}

		private static void Validate(
			ImageCanvasExternalProjectionResult result,
			string expectedTwoDTransactionId,
			int expectedImageWidth,
			int expectedImageHeight)
		{
			if (result == null)
			{
				throw new InvalidDataException("Projection result evidence is empty.");
			}
			if (!string.Equals(result.SchemaVersion, "1.0", StringComparison.Ordinal)
				|| string.IsNullOrWhiteSpace(result.ProjectionId)
				|| !string.Equals(result.TwoDTransactionId, expectedTwoDTransactionId, StringComparison.OrdinalIgnoreCase)
				|| result.ImageWidth != expectedImageWidth
				|| result.ImageHeight != expectedImageHeight
				|| result.GridWidth <= 1
				|| result.GridHeight <= 1)
			{
				throw new InvalidDataException("Projection result identity or dimensions do not match the current ImageCanvas image.");
			}

			ValidatePoints(result.TwoDToThreeD, "2D-to-3D");
			ValidatePoints(result.ThreeDToTwoD, "3D-to-2D");
		}

		private static void ValidatePoints(
			IEnumerable<ImageCanvasExternalProjectedPoint> points,
			string direction)
		{
			if (points == null)
			{
				throw new InvalidDataException($"Projection result is missing the {direction} point list.");
			}

			foreach (ImageCanvasExternalProjectedPoint point in points)
			{
				if (point == null
					|| string.IsNullOrWhiteSpace(point.Id)
					|| !IsFinite(point.ImageX)
					|| !IsFinite(point.ImageY)
					|| !IsFinite(point.GridX)
					|| !IsFinite(point.GridY)
					|| (point.SampledHeight.HasValue && !IsFinite(point.SampledHeight.Value)))
				{
					throw new InvalidDataException($"Projection result contains an invalid {direction} point.");
				}
			}
		}

		private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
	}
}
