using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    internal interface IPropertyGridImageEditView : IDisposable
    {
        bool ShowDialog();

        Rect SelectedRegion { get; }

        List<Rect> SelectedRegions { get; }

        void LoadPatternPreviewImage(string imagePath);
    }

    internal interface IPropertyGridTemplateImageEditView : IPropertyGridImageEditView
    {
        double TemplateRotationDegrees { get; set; }
    }
}
