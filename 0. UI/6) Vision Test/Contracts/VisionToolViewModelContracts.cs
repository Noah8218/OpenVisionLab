using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System.Collections.Generic;

namespace OpenVisionLab.Contracts
{
    internal enum LineToolPurpose
    {
        Edge,
        Measure,
        Intersection
    }


    internal sealed class VisionToolTemplateStatus
    {
        public VisionToolTemplateStatus(string text, bool isReady)
        {
            Text = text ?? string.Empty;
            IsReady = isReady;
        }

        public string Text { get; }

        public bool IsReady { get; }
    }

    internal interface IVisionToolLayerSelectionViewModel
    {
        IReadOnlyList<string> InputLayers { get; }

        IReadOnlyList<string> OutputLayers { get; }

        string SelectedInputLayer { get; }

        string SelectedInputLayerB { get; }

        string SelectedOutputLayer { get; }
    }

    internal interface IThresholdToolViewModel
    {
        ThresholdToolMode Mode { get; set; }

        double Threshold { get; set; }

        double MaxValue { get; set; }

        bool BasicInvert { get; set; }

        int RangeMin { get; set; }

        int RangeMax { get; set; }

        bool RangeInvert { get; set; }

        bool AdaptiveGaussian { get; set; }

        bool AdaptiveInvert { get; set; }

        double AdaptiveMaxValue { get; set; }

        int BlockSize { get; set; }

        int Weight { get; set; }

        string Summary { get; }

        ThresholdToolProperty CreateProperty();
    }

    internal interface IFilterToolViewModel
    {
        FilterToolType FilterType { get; set; }

        int KernelWidth { get; set; }

        int KernelHeight { get; set; }

        int MedianKernelSize { get; set; }

        int Diameter { get; set; }

        int SigmaColor { get; set; }

        int SigmaSpace { get; set; }

        BorderTypes BorderType { get; set; }

        bool UsesKernelSize { get; }

        bool UsesMedianKernel { get; }

        bool UsesBilateral { get; }

        string Summary { get; }

        FilterToolProperty CreateProperty();
    }

    internal interface IMorphologyToolViewModel
    {
        MorphTypes Operator { get; set; }

        MorphShapes Shape { get; set; }

        int KernelWidth { get; set; }

        int KernelHeight { get; set; }

        int Iterations { get; set; }

        string Summary { get; }

        MorphologyToolProperty CreateProperty();
    }

    internal interface IPropertyGridToolViewModel<TProperty>
    {
        string Summary { get; }

        TProperty CreateProperty();
    }

    internal interface ITemplateBackedPropertyGridToolViewModel<TProperty>
        : IPropertyGridToolViewModel<TProperty>
    {
        VisionToolTemplateStatus TemplateStatus { get; }

        void ApplyTemplatePathForTest(string path);

        void ReloadTemplateIfPatternChanged(string propertyName);
    }

    internal interface IBlobToolViewModel : IPropertyGridToolViewModel<BlobProperty>
    {
    }

    internal interface IContourToolViewModel : IPropertyGridToolViewModel<ContourProperty>
    {
    }

    internal interface IMatchingToolViewModel : ITemplateBackedPropertyGridToolViewModel<MatchingProperty>
    {
    }

    internal interface IFeatureMatchingToolViewModel : ITemplateBackedPropertyGridToolViewModel<FeatureMatchingProperty>
    {
    }

    internal interface IEdgeBasedMatchingToolViewModel : ITemplateBackedPropertyGridToolViewModel<EdgeBasedMatchingProperty>
    {
    }

    internal interface ILineToolViewModel
    {
        LineGaugeProperty LineAProperty { get; }

        LineGaugeProperty LineBProperty { get; }

        LineGaugeProperty GetSelectedLineProperty(bool isLineBSelected);

        LineGaugeProperty CreateSelectedLineProperty(bool isLineBSelected);

        LineGaugeProperty CreateLineAProperty();

        LineGaugeProperty CreateLineBProperty();

        string CreateSummary(LineToolPurpose purpose, bool isLineBSelected, string purposeText, string lineText);

        string CreatePurposeHint(LineToolPurpose purpose);
    }
}
