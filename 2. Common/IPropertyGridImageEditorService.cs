using OpenCvSharp;
using OpenVisionLab._1._Core;
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

        Mat GetSourceImage();

        FormImageEditView CreateImageEditView(Mat sourceImage, Rectangle roi, string mode);

        FormImageEditView CreateImageEditView(Mat sourceImage, List<Rect> roi, string mode);

        Rect LoadTemplateRoi(Mat sourceImage, string templatePath);

        string SaveTemplateImage(Mat sourceImage, Rect selectedRegion);
    }
}
