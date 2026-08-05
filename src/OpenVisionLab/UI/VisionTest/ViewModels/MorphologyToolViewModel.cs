using OpenVisionLab.Services;
using OpenVisionLab.Contracts;
using OpenVisionLab.Vision2D.Property;
using OpenCvSharp;
using OpenVisionLab.Mvvm;
using System;
using System.Runtime.CompilerServices;

namespace OpenVisionLab.ViewModels
{
    internal sealed class MorphologyToolViewModel : ObservableObject, IMorphologyToolViewModel
    {
        private readonly string settingsConfigName;
        private bool suppressSettingsSave;
        private MorphTypes operation = MorphTypes.Erode;
        private MorphShapes shape = MorphShapes.Rect;
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private int iterations = 1;

        public MorphologyToolViewModel()
            : this(OpenVisionNativeToolSettingsStore.CreateConfigName("Morphology"))
        {
        }

        public MorphologyToolViewModel(string settingsConfigName)
        {
            this.settingsConfigName = string.IsNullOrWhiteSpace(settingsConfigName)
                ? OpenVisionNativeToolSettingsStore.CreateConfigName("Morphology")
                : settingsConfigName;
        }

        public MorphTypes Operator
        {
            get => operation;
            set => SetParameterProperty(ref operation, value);
        }

        public MorphShapes Shape
        {
            get => shape;
            set => SetParameterProperty(ref shape, value);
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

        public int Iterations
        {
            get => iterations;
            set => SetParameterProperty(ref iterations, value);
        }

        public string Summary => $"{Operator} / {Shape} / {VisionToolParameterPolicy.NormalizePositiveSize(KernelWidth)} x {VisionToolParameterPolicy.NormalizePositiveSize(KernelHeight)}";

        public MorphologyToolProperty CreateProperty()
        {
            // Morphology property creation is shared by preview and pipeline, so keep normalization outside the view.
            return new MorphologyToolProperty
            {
                Shape = Shape,
                Operator = Operator,
                KernelWidth = VisionToolParameterPolicy.NormalizePositiveSize(KernelWidth),
                KernelHeight = VisionToolParameterPolicy.NormalizePositiveSize(KernelHeight),
                Iterations = VisionToolParameterPolicy.NormalizePositiveSize(Iterations)
            };
        }

        public MorphologyToolSettings CaptureSettings()
        {
            return new MorphologyToolSettings
            {
                Operator = Operator,
                Shape = Shape,
                KernelWidth = KernelWidth,
                KernelHeight = KernelHeight,
                Iterations = Iterations
            };
        }

        public void ApplySettings(MorphologyToolSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            suppressSettingsSave = true;
            try
            {
                Operator = settings.Operator;
                Shape = settings.Shape;
                KernelWidth = settings.KernelWidth;
                KernelHeight = settings.KernelHeight;
                Iterations = settings.Iterations;
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

        public static MorphShapes ParseShape(string value)
        {
            return VisionToolParameterPolicy.ParseMorphShape(value);
        }

        public static MorphTypes ParseOperation(string value)
        {
            return VisionToolParameterPolicy.ParseMorphOperation(value);
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
    }
}
