using OpenVisionLab.Services;
using OpenVisionLab.Contracts;
using Lib.OpenCV;
using Lib.OpenCV.Property;
using OpenCvSharp;
using OpenVisionLab.Mvvm;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace OpenVisionLab.ViewModels
{
    internal sealed class ThresholdToolViewModel : ObservableObject, IThresholdToolViewModel
    {
        private readonly string settingsConfigName;
        private bool suppressSettingsSave;
        private ThresholdToolMode mode = ThresholdToolMode.Threshold;
        private double threshold = 127;
        private double maxValue = 255;
        private bool basicInvert;
        private int rangeMin = 1;
        private int rangeMax = 255;
        private bool rangeInvert;
        private bool adaptiveGaussian;
        private bool adaptiveInvert;
        private double adaptiveMaxValue = 255;
        private int blockSize = 25;
        private int weight = 5;

        public ThresholdToolViewModel()
            : this(OpenVisionNativeToolSettingsStore.CreateConfigName("Threshold"))
        {
        }

        public ThresholdToolViewModel(string settingsConfigName)
        {
            this.settingsConfigName = string.IsNullOrWhiteSpace(settingsConfigName)
                ? OpenVisionNativeToolSettingsStore.CreateConfigName("Threshold")
                : settingsConfigName;
        }

        public ThresholdToolMode Mode
        {
            get => mode;
            set => SetParameterProperty(ref mode, value);
        }

        public double Threshold
        {
            get => threshold;
            set => SetParameterProperty(ref threshold, value);
        }

        public double MaxValue
        {
            get => maxValue;
            set => SetParameterProperty(ref maxValue, value);
        }

        public bool BasicInvert
        {
            get => basicInvert;
            set => SetParameterProperty(ref basicInvert, value);
        }

        public int RangeMin
        {
            get => rangeMin;
            set => SetParameterProperty(ref rangeMin, value);
        }

        public int RangeMax
        {
            get => rangeMax;
            set => SetParameterProperty(ref rangeMax, value);
        }

        public bool RangeInvert
        {
            get => rangeInvert;
            set => SetParameterProperty(ref rangeInvert, value);
        }

        public bool AdaptiveGaussian
        {
            get => adaptiveGaussian;
            set => SetParameterProperty(ref adaptiveGaussian, value);
        }

        public bool AdaptiveInvert
        {
            get => adaptiveInvert;
            set => SetParameterProperty(ref adaptiveInvert, value);
        }

        public double AdaptiveMaxValue
        {
            get => adaptiveMaxValue;
            set => SetParameterProperty(ref adaptiveMaxValue, value);
        }

        public int BlockSize
        {
            get => blockSize;
            set => SetParameterProperty(ref blockSize, value);
        }

        public int Weight
        {
            get => weight;
            set => SetParameterProperty(ref weight, value);
        }

        public string Summary => CreateSummary(CreateProperty());

        public ThresholdToolProperty CreateProperty()
        {
            int normalizedRangeMin = Clamp(RangeMin, 0, 255);
            int normalizedRangeMax = Clamp(RangeMax, 0, 255);
            if (normalizedRangeMin > normalizedRangeMax)
            {
                (normalizedRangeMin, normalizedRangeMax) = (normalizedRangeMax, normalizedRangeMin);
            }

            // Keep OpenCV parameter normalization in the ViewModel so views only mirror control state.
            return new ThresholdToolProperty
            {
                Mode = Mode,
                Threshold = Clamp(Threshold, 0, 255),
                MaxValue = Mode == ThresholdToolMode.Adaptive
                    ? Clamp(AdaptiveMaxValue, 1, 255)
                    : Clamp(MaxValue, 1, 255),
                ThresholdType = BasicInvert
                    ? ThresholdTypes.BinaryInv
                    : ThresholdTypes.Binary,
                RangeMin = normalizedRangeMin,
                RangeMax = normalizedRangeMax,
                Invert = RangeInvert,
                AdaptiveType = AdaptiveGaussian
                    ? AdaptiveThresholdTypes.GaussianC
                    : AdaptiveThresholdTypes.MeanC,
                AdaptiveThresholdType = AdaptiveInvert
                    ? ThresholdTypes.BinaryInv
                    : ThresholdTypes.Binary,
                BlockSize = VisionToolParameterPolicy.NormalizeThresholdBlockSize(BlockSize),
                Weight = Clamp(Weight, -255, 255)
            };
        }

        public ThresholdToolSettings CaptureSettings()
        {
            return new ThresholdToolSettings
            {
                Mode = Mode,
                Threshold = Threshold,
                MaxValue = MaxValue,
                BasicInvert = BasicInvert,
                RangeMin = RangeMin,
                RangeMax = RangeMax,
                RangeInvert = RangeInvert,
                AdaptiveGaussian = AdaptiveGaussian,
                AdaptiveInvert = AdaptiveInvert,
                AdaptiveMaxValue = AdaptiveMaxValue,
                BlockSize = BlockSize,
                Weight = Weight
            };
        }

        public void ApplySettings(ThresholdToolSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            suppressSettingsSave = true;
            try
            {
                Mode = settings.Mode;
                Threshold = settings.Threshold;
                MaxValue = settings.MaxValue;
                BasicInvert = settings.BasicInvert;
                RangeMin = settings.RangeMin;
                RangeMax = settings.RangeMax;
                RangeInvert = settings.RangeInvert;
                AdaptiveGaussian = settings.AdaptiveGaussian;
                AdaptiveInvert = settings.AdaptiveInvert;
                AdaptiveMaxValue = settings.AdaptiveMaxValue;
                BlockSize = settings.BlockSize;
                Weight = settings.Weight;
            }
            finally
            {
                suppressSettingsSave = false;
            }

            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(Summary));
        }

        public static int NormalizeBlockSize(int value)
        {
            return VisionToolParameterPolicy.NormalizeThresholdBlockSize(value);
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

        private static string CreateSummary(ThresholdToolProperty property)
        {
            switch (property.Mode)
            {
                case ThresholdToolMode.Range:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "Range {0}-{1} / {2}",
                        property.RangeMin,
                        property.RangeMax,
                        property.Invert ? "Invert" : "Normal");
                case ThresholdToolMode.Adaptive:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "Adaptive / {0} / {1} / Block {2} / C {3}",
                        property.AdaptiveType,
                        property.AdaptiveThresholdType,
                        property.BlockSize,
                        property.Weight);
                default:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "Basic / T {0:0.#} / {1} / Max {2:0.#}",
                        property.Threshold,
                        property.ThresholdType,
                        property.MaxValue);
            }
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
