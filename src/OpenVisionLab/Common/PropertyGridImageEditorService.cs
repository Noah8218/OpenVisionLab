using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace OpenVisionLab
{
    internal sealed class PropertyGridImageEditorService : IPropertyGridImageEditorService
    {
        private const string TemplateRoiFormat = "OpenVisionLab.PatternRoi";
        private const int TemplateRoiFormatVersion = 1;
        private const double MinimumTemplateMatchScore = 0.5;
        private Func<IDisplayManager> runtimeContextAccessor;
        private Func<string> recipeNameAccessor = () => string.Empty;
        private Func<string> sourceLayerNameAccessor = () => string.Empty;

        public PropertyGridImageEditorService(Func<IDisplayManager> contextAccessor)
        {
            SetRuntimeContext(contextAccessor);
        }

        public IDisplayManager RuntimeContext
        {
            get => runtimeContextAccessor() ?? DisplayManagerService.Default;
            set => SetRuntimeContext(() => value ?? DisplayManagerService.Default);
        }

        public void SetRuntimeContext(Func<IDisplayManager> contextAccessor)
        {
            runtimeContextAccessor = contextAccessor ?? (() => DisplayManagerService.Default);
        }

        public void SetRecipeNameContext(Func<string> recipeNameAccessor)
        {
            this.recipeNameAccessor = recipeNameAccessor ?? (() => string.Empty);
        }

        public void SetSourceLayerContext(Func<string> sourceLayerNameAccessor)
        {
            this.sourceLayerNameAccessor = sourceLayerNameAccessor ?? (() => string.Empty);
        }

        public Mat GetSourceImage()
        {
            Bitmap sourceBitmap = ResolveSourceBitmap(RuntimeContext, GetSourceLayerName());
            return ToMatSafely(sourceBitmap);
        }

        public IPropertyGridImageEditView CreateImageEditView(Mat sourceImage, Rectangle roi, string mode)
        {
            using Bitmap bitmap = CreateSourceBitmap(sourceImage);
            if (string.Equals(mode, "TRAIN", StringComparison.OrdinalIgnoreCase))
            {
                return new OpenGlTemplateEditorWindow(bitmap, roi, mode);
            }

            return new RoiEditorWindow(bitmap, roi, mode);
        }

        public IPropertyGridImageEditView CreateImageEditView(Mat sourceImage, List<Rect> roi, string mode)
        {
            using Bitmap bitmap = CreateSourceBitmap(sourceImage);
            return new RoiEditorWindow(bitmap, roi ?? new List<Rect>(), mode);
        }

        public double LoadTemplateRotationDegrees(string templatePath)
        {
            return LoadTemplateRotationMetadata(templatePath);
        }

        public string SaveTemplateImage(Mat sourceImage, Rect selectedRegion)
        {
            return SaveTemplateImage(sourceImage, selectedRegion, 0D);
        }

        public string SaveTemplateImage(Mat sourceImage, Rect selectedRegion, double rotationDegrees)
        {
            Rect templateRegion = ClampToMatBounds(sourceImage, selectedRegion);
            if (!IsValidRoi(templateRegion)) { return string.Empty; }

            string path = $@"{RecipeWorkspaceService.GetPatternDirectory(recipeNameAccessor())}\{DateTime.Now:yyyyMMdd_HHmmss}.bmp";

            using (Mat imageTemplate = TemplateImageExtraction.Extract(sourceImage, templateRegion, rotationDegrees))
            {
                Cv2.ImWrite(path, imageTemplate);
            }

            SaveTemplateRoiMetadata(path, templateRegion, sourceImage.Size(), rotationDegrees);
            return path;
        }

        public Rect LoadTemplateRoi(Mat sourceImage, string templatePath)
        {
            Rect metadataRoi = LoadTemplateRoiMetadata(templatePath);
            if (IsValidRoi(metadataRoi)) { return metadataRoi; }

            return FindTemplateRoi(sourceImage, templatePath);
        }

        private static void SaveTemplateRoiMetadata(
            string templatePath,
            Rect selectedRegion,
            OpenCvSharp.Size sourceSize,
            double rotationDegrees)
        {
            if (string.IsNullOrWhiteSpace(templatePath) || !IsValidRoi(selectedRegion)) { return; }

            string metadataPath = GetTemplateRoiMetadataPath(templatePath);
            double normalizedRotationDegrees = TemplateImageExtraction.NormalizeRotationDegrees(rotationDegrees);
            string content = string.Join(Environment.NewLine, new[]
            {
                $"Format={TemplateRoiFormat}",
                $"Version={TemplateRoiFormatVersion.ToString(CultureInfo.InvariantCulture)}",
                $"X={selectedRegion.X.ToString(CultureInfo.InvariantCulture)}",
                $"Y={selectedRegion.Y.ToString(CultureInfo.InvariantCulture)}",
                $"Width={selectedRegion.Width.ToString(CultureInfo.InvariantCulture)}",
                $"Height={selectedRegion.Height.ToString(CultureInfo.InvariantCulture)}",
                $"SourceWidth={sourceSize.Width.ToString(CultureInfo.InvariantCulture)}",
                $"SourceHeight={sourceSize.Height.ToString(CultureInfo.InvariantCulture)}",
                $"RotationDegrees={normalizedRotationDegrees.ToString("0.###", CultureInfo.InvariantCulture)}"
            });

            File.WriteAllText(metadataPath, content);
        }

        private static Rect LoadTemplateRoiMetadata(string templatePath)
        {
            if (string.IsNullOrWhiteSpace(templatePath)) { return new Rect(); }

            string metadataPath = GetTemplateRoiMetadataPath(templatePath);
            if (!File.Exists(metadataPath)) { return new Rect(); }

            Dictionary<string, int> values = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (string line in File.ReadAllLines(metadataPath))
            {
                string[] parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2) { continue; }
                if (int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
                {
                    values[parts[0].Trim()] = parsedValue;
                }
            }

            if (!values.TryGetValue("X", out int x)
                || !values.TryGetValue("Y", out int y)
                || !values.TryGetValue("Width", out int width)
                || !values.TryGetValue("Height", out int height))
            {
                return new Rect();
            }

            return width > 0 && height > 0 ? new Rect(x, y, width, height) : new Rect();
        }

        private static double LoadTemplateRotationMetadata(string templatePath)
        {
            if (string.IsNullOrWhiteSpace(templatePath)) { return 0D; }

            string metadataPath = GetTemplateRoiMetadataPath(templatePath);
            if (!File.Exists(metadataPath)) { return 0D; }

            foreach (string line in File.ReadAllLines(metadataPath))
            {
                string[] parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2) { continue; }
                if (!string.Equals(parts[0].Trim(), "RotationDegrees", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
                {
                    return TemplateImageExtraction.NormalizeRotationDegrees(parsedValue);
                }
            }

            return 0D;
        }

        private static Rect FindTemplateRoi(Mat sourceImage, string templatePath)
        {
            if (sourceImage == null || sourceImage.Empty()) { return new Rect(); }
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath)) { return new Rect(); }

            using (Mat templateImage = Cv2.ImRead(templatePath, ImreadModes.Unchanged))
            using (Mat sourceGray = ToGrayMat(sourceImage))
            using (Mat templateGray = ToGrayMat(templateImage))
            {
                if (templateGray.Empty() || sourceGray.Empty()) { return new Rect(); }
                if (templateGray.Width > sourceGray.Width || templateGray.Height > sourceGray.Height) { return new Rect(); }

                int resultWidth = sourceGray.Width - templateGray.Width + 1;
                int resultHeight = sourceGray.Height - templateGray.Height + 1;
                using (Mat result = new Mat(resultHeight, resultWidth, MatType.CV_32FC1))
                {
                    Cv2.MatchTemplate(sourceGray, templateGray, result, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(result, out _, out double maxValue, out _, out OpenCvSharp.Point maxLocation);
                    if (double.IsNaN(maxValue) || double.IsInfinity(maxValue)) { return new Rect(); }
                    if (maxValue < MinimumTemplateMatchScore) { return new Rect(); }

                    return new Rect(maxLocation.X, maxLocation.Y, templateGray.Width, templateGray.Height);
                }
            }
        }

        private static Mat ToGrayMat(Mat image)
        {
            if (image == null || image.Empty()) { return new Mat(); }

            Mat gray = new Mat();
            if (image.Channels() == 1)
            {
                image.CopyTo(gray);
                return gray;
            }

            ColorConversionCodes conversionCode = image.Channels() == 4
                ? ColorConversionCodes.BGRA2GRAY
                : ColorConversionCodes.BGR2GRAY;
            Cv2.CvtColor(image, gray, conversionCode);
            return gray;
        }

        private static string GetTemplateRoiMetadataPath(string templatePath)
        {
            return Path.ChangeExtension(templatePath, ".roi");
        }

        private static bool IsValidRoi(Rect roi)
        {
            return roi.Width > 0 && roi.Height > 0;
        }

        private static Rect ClampToMatBounds(Mat sourceImage, Rect roi)
        {
            if (sourceImage == null || sourceImage.Empty() || !IsValidRoi(roi)) { return new Rect(); }

            int x = Math.Max(0, roi.X);
            int y = Math.Max(0, roi.Y);
            int right = Math.Min(sourceImage.Width, roi.X + roi.Width);
            int bottom = Math.Min(sourceImage.Height, roi.Y + roi.Height);
            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);

            return width > 0 && height > 0 ? new Rect(x, y, width, height) : new Rect();
        }

        private static Bitmap CreateSourceBitmap(Mat sourceImage)
        {
            if (sourceImage == null || sourceImage.Empty())
            {
                return new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            }

            try
            {
                return BitmapImageConverter.ToBitmap(sourceImage);
            }
            catch
            {
                return new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            }
        }

        private string GetSourceLayerName()
        {
            try
            {
                return sourceLayerNameAccessor() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static Bitmap ResolveSourceBitmap(IDisplayManager displayManager, string sourceLayerName)
        {
            if (displayManager == null)
            {
                return null;
            }

            // PropertyGrid editors are generic, so prefer the active tool input layer before shell focus/selection.
            Bitmap image = ResolveLayerBitmap(displayManager, sourceLayerName);
            if (IsUsableBitmap(image)) { return image; }

            image = ResolveLayerBitmap(displayManager, displayManager.FocusItem);
            if (IsUsableBitmap(image)) { return image; }

            image = ResolveLayerBitmap(displayManager, displayManager.SelectedItem);
            if (IsUsableBitmap(image)) { return image; }

            image = ResolveLayerBitmap(displayManager, "Main");
            if (IsUsableBitmap(image)) { return image; }

            image = displayManager.ImageSpace?.GetActiveImage();
            if (IsUsableBitmap(image)) { return image; }

            IReadOnlyList<DisplayLayerInfo> layers = displayManager.GetLayerInfos();
            if (layers != null)
            {
                foreach (DisplayLayerInfo layer in layers)
                {
                    image = ResolveLayerBitmap(displayManager, layer?.Title);
                    if (IsUsableBitmap(image)) { return image; }
                }
            }

            return null;
        }

        private static Bitmap ResolveLayerBitmap(IDisplayManager displayManager, string layerName)
        {
            return string.IsNullOrWhiteSpace(layerName)
                ? null
                : displayManager.GetLayerImage(layerName);
        }

        private static Mat ToMatSafely(Bitmap sourceBitmap)
        {
            if (!IsUsableBitmap(sourceBitmap))
            {
                return new Mat();
            }

            try
            {
                using Bitmap compatibleBitmap = WpfBitmapSourceFactory.CloneCompatibleBitmap(sourceBitmap);
                try
                {
                    return ImageSpaceFrameAdapter.ToMat(compatibleBitmap);
                }
                catch
                {
                    return CreateMatFromPixels(compatibleBitmap);
                }
            }
            catch
            {
                return new Mat();
            }
        }

        private static Mat CreateMatFromPixels(Bitmap bitmap)
        {
            if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return new Mat();
            }

            Mat image = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC3);
            try
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        image.Set(y, x, new Vec3b(color.B, color.G, color.R));
                    }
                }

                return image;
            }
            catch
            {
                image.Dispose();
                return new Mat();
            }
        }

        private static bool IsUsableBitmap(Bitmap image)
        {
            return image != null
                && image.Width > 0
                && image.Height > 0
                && !DisplayManagerImageExtensions.IsPlaceholderBitmap(image);
        }
    }
}
