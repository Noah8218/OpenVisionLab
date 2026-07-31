using OpenCvSharp;
using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenVisionLab
{
    internal interface IPropertyGridImageEditorService
    {
        IDisplayManager RuntimeContext { get; set; }

        void SetRuntimeContext(Func<IDisplayManager> contextAccessor);
        void SetRecipeNameContext(Func<string> recipeNameAccessor);
        void SetSourceLayerContext(Func<string> sourceLayerNameAccessor);

        Mat GetSourceImage();

        IPropertyGridImageEditView CreateImageEditView(Mat sourceImage, Rectangle roi, string mode);

        IPropertyGridImageEditView CreateImageEditView(Mat sourceImage, List<Rect> roi, string mode);

        Rect LoadTemplateRoi(Mat sourceImage, string templatePath);

        double LoadTemplateRotationDegrees(string templatePath);

        string SaveTemplateImage(Mat sourceImage, Rect selectedRegion);

        string SaveTemplateImage(Mat sourceImage, Rect selectedRegion, double rotationDegrees);
    }
}
