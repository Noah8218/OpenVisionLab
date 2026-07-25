using Lib.OpenCV.Pipeline;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class VisionPipelineTransformPropertyAdapter
    {
        public static bool TryCreateProperty(
            VisionPipelineStep step,
            string name,
            VisionPipelinePropertyContext context,
            out object property)
        {
            property = null;
            if (step == null)
            {
                return false;
            }

            switch (NormalizeToolType(step.ToolType))
            {
                case "rotatescale":
                case "rotateandscale":
                    property = AttachStepMetadata(
                        new VisionPipelineStepPropertyMapper.PipelineRotateScaleToolProperty
                        {
                            Angle = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.Angle), 0d),
                            ScaleXPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleXPercent), 100d),
                            ScaleYPercent = GetDouble(step.Parameters, nameof(RotateScaleToolProperty.ScaleYPercent), 100d),
                            Interpolation = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.Interpolation), InterpolationFlags.Linear),
                            BorderType = GetEnum(step.Parameters, nameof(RotateScaleToolProperty.BorderType), BorderTypes.Constant),
                            USE_FIXTURE_FRAME = GetBool(step.Parameters, VisionPipelineFixtureFrameService.ConsumeParameter, false),
                            FIXTURE_FRAME_NAME = GetString(step.Parameters, VisionPipelineFixtureFrameService.FrameNameParameter, string.Empty),
                            FIXTURE_APPLY_MODE = GetEnum(
                                step.Parameters,
                                VisionPipelineFixtureFrameService.ApplyModeParameter,
                                VisionPipelineFixtureApplyMode.TranslationRoi),
                            FIXTURE_MIN_VALID_PIXEL_RATIO = GetDouble(
                                step.Parameters,
                                VisionPipelineFixtureFrameService.MinimumValidPixelRatioParameter,
                                VisionPipelineFixtureFrameService.DefaultMinimumValidPixelRatio),
                            ALLOW_BRANCH_INPUT = GetBool(
                                step.Parameters,
                                VisionPipelineNormalizer.AllowBranchInputParameter,
                                false)
                        },
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                case "affine":
                case "affinematrix":
                case "affinetransform":
                    property = AttachStepMetadata(
                        new VisionPipelineStepPropertyMapper.PipelineAffineTransformToolProperty
                        {
                            Context = context ?? VisionPipelinePropertyContext.Empty,
                            UseDetectedSourcePoints = GetBool(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter,
                                false),
                            SourcePoint1Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter,
                                string.Empty),
                            SourcePoint2Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter,
                                string.Empty),
                            SourcePoint3Feature = GetString(
                                step.Parameters,
                                VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter,
                                string.Empty),
                            SourcePoint1X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint1X), 0d),
                            SourcePoint1Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint1Y), 0d),
                            SourcePoint2X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint2X), 100d),
                            SourcePoint2Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint2Y), 0d),
                            SourcePoint3X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint3X), 0d),
                            SourcePoint3Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.SourcePoint3Y), 100d),
                            DestinationPoint1X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint1X), 0d),
                            DestinationPoint1Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint1Y), 0d),
                            DestinationPoint2X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint2X), 100d),
                            DestinationPoint2Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint2Y), 0d),
                            DestinationPoint3X = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint3X), 0d),
                            DestinationPoint3Y = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.DestinationPoint3Y), 100d),
                            OutputWidth = GetInt(step.Parameters, nameof(AffineTransformToolProperty.OutputWidth), 0),
                            OutputHeight = GetInt(step.Parameters, nameof(AffineTransformToolProperty.OutputHeight), 0),
                            Interpolation = GetEnum(step.Parameters, nameof(AffineTransformToolProperty.Interpolation), InterpolationFlags.Linear),
                            BorderType = GetEnum(step.Parameters, nameof(AffineTransformToolProperty.BorderType), BorderTypes.Constant),
                            BorderValue = GetDouble(step.Parameters, nameof(AffineTransformToolProperty.BorderValue), 0d),
                            MinimumSourceTriangleArea = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumSourceTriangleArea),
                                1d),
                            MinimumDestinationTriangleArea = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumDestinationTriangleArea),
                                1d),
                            MinimumValidPixelRatio = GetDouble(
                                step.Parameters,
                                nameof(AffineTransformToolProperty.MinimumValidPixelRatio),
                                0d)
                        },
                        name,
                        step.InputLayer,
                        step.OutputLayer);
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryCreateStep(
            object property,
            string fallbackName,
            string inputLayer,
            string outputLayer,
            out VisionPipelineStep step)
        {
            step = null;
            if (property is RotateScaleToolProperty rotateScale)
            {
                step = VisionPipelineStepBuilder.FromRotateScaleProperty(
                    rotateScale,
                    GetPropertyName(property, fallbackName),
                    inputLayer,
                    outputLayer);
                if (property is VisionPipelineStepPropertyMapper.PipelineRotateScaleToolProperty normalizeFixture)
                {
                    normalizeFixture.ApplyFixtureParameters(step.Parameters);
                }

                return true;
            }

            if (property is not AffineTransformToolProperty affineTransform)
            {
                return false;
            }

            step = VisionPipelineStepBuilder.FromAffineTransformProperty(
                affineTransform,
                GetPropertyName(property, fallbackName),
                inputLayer,
                outputLayer);
            if (property is VisionPipelineStepPropertyMapper.PipelineAffineTransformToolProperty affinePointBinding)
            {
                AddParameter(
                    step.Parameters,
                    VisionPipelineAffinePointBindingService.UseDetectedSourcePointsParameter,
                    affinePointBinding.UseDetectedSourcePoints);
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint1FeatureParameter] =
                    affinePointBinding.SourcePoint1Feature?.Trim() ?? string.Empty;
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint2FeatureParameter] =
                    affinePointBinding.SourcePoint2Feature?.Trim() ?? string.Empty;
                step.Parameters[VisionPipelineAffinePointBindingService.SourcePoint3FeatureParameter] =
                    affinePointBinding.SourcePoint3Feature?.Trim() ?? string.Empty;
            }

            return true;
        }

        private static T AttachStepMetadata<T>(
            T property,
            string name,
            string inputLayer,
            string outputLayer)
            where T : VisionPipelineStepPropertyMapper.IPipelineStepMetadata
        {
            property.PipelineStepName = string.IsNullOrWhiteSpace(name)
                ? property.PipelineStepName
                : name;
            property.InputLayer = string.IsNullOrWhiteSpace(inputLayer) ? "Main" : inputLayer;
            property.OutputLayer = string.IsNullOrWhiteSpace(outputLayer)
                ? "Pipeline_Output"
                : outputLayer;
            return property;
        }

        private static string GetPropertyName(object property, string fallback)
        {
            return property is VisionPipelineStepPropertyMapper.IPipelineStepMetadata metadata
                && !string.IsNullOrWhiteSpace(metadata.PipelineStepName)
                    ? metadata.PipelineStepName
                    : fallback;
        }

        private static string NormalizeToolType(string toolType)
        {
            string value = (toolType ?? string.Empty).Trim();
            if (value.EndsWith("Tool", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 4);
            }

            return value.Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();
        }

        private static string GetString(
            IDictionary<string, string> parameters,
            string key,
            string defaultValue)
        {
            string value = GetValue(parameters, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int GetInt(
            IDictionary<string, string> parameters,
            string key,
            int defaultValue)
        {
            return int.TryParse(
                GetValue(parameters, key),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result)
                    ? result
                    : defaultValue;
        }

        private static double GetDouble(
            IDictionary<string, string> parameters,
            string key,
            double defaultValue)
        {
            return double.TryParse(
                GetValue(parameters, key),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result)
                    ? result
                    : defaultValue;
        }

        private static bool GetBool(
            IDictionary<string, string> parameters,
            string key,
            bool defaultValue)
        {
            return bool.TryParse(GetValue(parameters, key), out bool result)
                ? result
                : defaultValue;
        }

        private static TEnum GetEnum<TEnum>(
            IDictionary<string, string> parameters,
            string key,
            TEnum defaultValue)
            where TEnum : struct
        {
            return Enum.TryParse(GetValue(parameters, key), true, out TEnum result)
                ? result
                : defaultValue;
        }

        private static string GetValue(IDictionary<string, string> parameters, string key)
        {
            if (parameters == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (KeyValuePair<string, string> item in parameters)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }

            return null;
        }

        private static void AddParameter(
            IDictionary<string, string> parameters,
            string key,
            object value)
        {
            parameters[key] =
                Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
