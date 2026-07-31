using OpenVisionLab.Services;
using OpenVisionLab.Contracts;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Mvvm;
using System;
using System.Runtime.CompilerServices;

namespace OpenVisionLab.ViewModels
{
    internal sealed class FilterToolViewModel : ObservableObject, IFilterToolViewModel
    {
        private readonly string settingsConfigName;
        private bool suppressSettingsSave;
        private FilterToolType filterType = FilterToolType.Blur;
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private int medianKernelSize = 3;
        private int diameter = 3;
        private int sigmaColor = 3;
        private int sigmaSpace = 3;
        private BorderTypes borderType = BorderTypes.Reflect101;

        public FilterToolViewModel()
            : this(OpenVisionNativeToolSettingsStore.CreateConfigName("Filter"))
        {
        }

        public FilterToolViewModel(string settingsConfigName)
        {
            this.settingsConfigName = string.IsNullOrWhiteSpace(settingsConfigName)
                ? OpenVisionNativeToolSettingsStore.CreateConfigName("Filter")
                : settingsConfigName;
        }

        public FilterToolType FilterType
        {
            get => filterType;
            set
            {
                if (SetParameterProperty(ref filterType, value))
                {
                    OnPropertyChanged(nameof(UsesKernelSize));
                    OnPropertyChanged(nameof(UsesMedianKernel));
                    OnPropertyChanged(nameof(UsesBilateral));
                }
            }
        }

        public int KernelWidth
        {
            get => kernelWidth;
            set => SetParameterProperty(ref kernelWidth, value);
        }

        public int KernelHeight
        {
            get => kernelHeight;
            set => SetParameterProperty(ref kernelHeight, value);
        }

        public int MedianKernelSize
        {
            get => medianKernelSize;
            set => SetParameterProperty(ref medianKernelSize, value);
        }

        public int Diameter
        {
            get => diameter;
            set => SetParameterProperty(ref diameter, value);
        }

        public int SigmaColor
        {
            get => sigmaColor;
            set => SetParameterProperty(ref sigmaColor, value);
        }

        public int SigmaSpace
        {
            get => sigmaSpace;
            set => SetParameterProperty(ref sigmaSpace, value);
        }

        public BorderTypes BorderType
        {
            get => borderType;
            set => SetParameterProperty(ref borderType, value);
        }

        public bool UsesKernelSize => FilterType == FilterToolType.Blur
            || FilterType == FilterToolType.GaussianBlur
            || FilterType == FilterToolType.BoxFilter;

        public bool UsesMedianKernel => FilterType == FilterToolType.MedianBlur;

        public bool UsesBilateral => FilterType == FilterToolType.BilateralFilter;

        public string Summary => CreateSummary(CreateProperty());

        public FilterToolProperty CreateProperty()
        {
            // Normalize filter parameters in one place before they enter preview or pipeline execution.
            return new FilterToolProperty
            {
                FilterType = FilterType,
                KernelWidth = VisionToolParameterPolicy.NormalizePositiveSize(KernelWidth),
                KernelHeight = VisionToolParameterPolicy.NormalizePositiveSize(KernelHeight),
                MedianKernelSize = VisionToolParameterPolicy.NormalizeOddKernelSize(MedianKernelSize),
                Diameter = VisionToolParameterPolicy.NormalizePositiveSize(Diameter),
                SigmaColor = VisionToolParameterPolicy.NormalizePositiveSize(SigmaColor),
                SigmaSpace = VisionToolParameterPolicy.NormalizePositiveSize(SigmaSpace),
                BorderType = BorderType
            };
        }

        public FilterToolSettings CaptureSettings()
        {
            return new FilterToolSettings
            {
                FilterType = FilterType,
                KernelWidth = KernelWidth,
                KernelHeight = KernelHeight,
                MedianKernelSize = MedianKernelSize,
                Diameter = Diameter,
                SigmaColor = SigmaColor,
                SigmaSpace = SigmaSpace,
                BorderType = BorderType
            };
        }

        public void ApplySettings(FilterToolSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            suppressSettingsSave = true;
            try
            {
                FilterType = settings.FilterType;
                KernelWidth = settings.KernelWidth;
                KernelHeight = settings.KernelHeight;
                MedianKernelSize = settings.MedianKernelSize;
                Diameter = settings.Diameter;
                SigmaColor = settings.SigmaColor;
                SigmaSpace = settings.SigmaSpace;
                BorderType = settings.BorderType;
            }
            finally
            {
                suppressSettingsSave = false;
            }

            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(Summary));
        }

        public static int NormalizeSize(int value)
        {
            return VisionToolParameterPolicy.NormalizePositiveSize(value);
        }

        public static int NormalizeOddKernelSize(int value)
        {
            return VisionToolParameterPolicy.NormalizeOddKernelSize(value);
        }

        private bool SetParameterProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref field, value, propertyName))
            {
                return false;
            }

            OnPropertyChanged(nameof(Summary));
            SaveSettings();
            return true;
        }

        private void SaveSettings()
        {
            if (suppressSettingsSave)
            {
                return;
            }

            OpenVisionNativeToolSettingsStore.Save(settingsConfigName, CaptureSettings());
        }

        private static string CreateSummary(FilterToolProperty property)
        {
            string sizeText = property.FilterType == FilterToolType.MedianBlur
                ? $"Kernel {property.MedianKernelSize}"
                : property.FilterType == FilterToolType.BilateralFilter
                    ? $"D {property.Diameter} / Color {property.SigmaColor} / Space {property.SigmaSpace}"
                    : $"{property.KernelWidth} x {property.KernelHeight}";
            return $"{property.FilterType} / {sizeText} / {property.BorderType}";
        }
    }
}
