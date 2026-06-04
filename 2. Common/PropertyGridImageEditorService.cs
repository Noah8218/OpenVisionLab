using Lib.Common;
using OpenCvSharp;
using OpenVisionLab._1._Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab
{
    internal sealed class PropertyGridImageEditorService : IPropertyGridImageEditorService
    {
        private Func<IDisplayManager> runtimeContextAccessor;
        private Func<string> recipeNameAccessor = () => string.Empty;

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

        public Mat GetSourceImage()
        {
            return RuntimeContext.GetImageSrc();
        }

        public FormImageEditView CreateImageEditView(Mat sourceImage, Rectangle roi, string mode)
        {
            return new FormImageEditView(CreateSourceBitmap(sourceImage), roi, mode);
        }

        public FormImageEditView CreateImageEditView(Mat sourceImage, List<Rect> roi, string mode)
        {
            return new FormImageEditView(CreateSourceBitmap(sourceImage), roi ?? new List<Rect>(), mode);
        }

        public string SaveTemplateImage(Mat sourceImage, Rect selectedRegion)
        {
            string path = $@"{RecipeWorkspaceService.GetPatternDirectory(recipeNameAccessor())}\{DateTime.Now:yyyyMMdd_HHmmss}.bmp";

            using (Mat imageTemplate = sourceImage.SubMat(selectedRegion).Clone())
            {
                Cv2.ImWrite(path, imageTemplate);
            }

            return path;
        }

        private static Bitmap CreateSourceBitmap(Mat sourceImage)
        {
            if (sourceImage == null || sourceImage.Empty())
            {
                return new Bitmap(10, 10);
            }

            return CImageConverter.ToBitmap(sourceImage);
        }
    }
}
