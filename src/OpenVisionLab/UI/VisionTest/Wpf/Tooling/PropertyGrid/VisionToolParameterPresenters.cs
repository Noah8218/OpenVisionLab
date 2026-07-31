using Lib.OpenCV;
using Lib.OpenCV.Property;
using Lib.OpenCV.Tool;
using OpenCvSharp;
using OpenVisionLab.Contracts;
using OpenVisionLab.Mvvm;
using OpenVisionLab.Services;

namespace OpenVisionLab
{
    internal sealed class ThresholdToolPresenter : ObservableObject
    {
        private readonly IThresholdToolViewModel viewModel;

        public ThresholdToolPresenter(IThresholdToolViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public ThresholdToolMode Mode
        {
            get => viewModel.Mode;
            set => SetAndRefresh(() => viewModel.Mode = value, nameof(Mode));
        }

        public double Threshold
        {
            get => viewModel.Threshold;
            set => SetAndRefresh(() => viewModel.Threshold = value, nameof(Threshold));
        }

        public double MaxValue
        {
            get => viewModel.MaxValue;
            set => SetAndRefresh(() => viewModel.MaxValue = value, nameof(MaxValue));
        }

        public bool BasicInvert
        {
            get => viewModel.BasicInvert;
            set => SetAndRefresh(() => viewModel.BasicInvert = value, nameof(BasicInvert));
        }

        public int RangeMin
        {
            get => viewModel.RangeMin;
            set => SetAndRefresh(() => viewModel.RangeMin = value, nameof(RangeMin));
        }

        public int RangeMax
        {
            get => viewModel.RangeMax;
            set => SetAndRefresh(() => viewModel.RangeMax = value, nameof(RangeMax));
        }

        public bool RangeInvert
        {
            get => viewModel.RangeInvert;
            set => SetAndRefresh(() => viewModel.RangeInvert = value, nameof(RangeInvert));
        }

        public bool AdaptiveGaussian
        {
            get => viewModel.AdaptiveGaussian;
            set => SetAndRefresh(() => viewModel.AdaptiveGaussian = value, nameof(AdaptiveGaussian));
        }

        public bool AdaptiveInvert
        {
            get => viewModel.AdaptiveInvert;
            set => SetAndRefresh(() => viewModel.AdaptiveInvert = value, nameof(AdaptiveInvert));
        }

        public double AdaptiveMaxValue
        {
            get => viewModel.AdaptiveMaxValue;
            set => SetAndRefresh(() => viewModel.AdaptiveMaxValue = value, nameof(AdaptiveMaxValue));
        }

        public int BlockSize
        {
            get => viewModel.BlockSize;
            set => SetAndRefresh(() => viewModel.BlockSize = value, nameof(BlockSize));
        }

        public int Weight
        {
            get => viewModel.Weight;
            set => SetAndRefresh(() => viewModel.Weight = value, nameof(Weight));
        }

        public string Summary => viewModel.Summary;

        public ThresholdToolProperty CreateProperty()
        {
            return viewModel.CreateProperty();
        }

        public void KeepRangeOrdered(bool preferMinimum)
        {
            if (RangeMin <= RangeMax)
            {
                return;
            }

            if (preferMinimum)
            {
                RangeMax = RangeMin;
            }
            else
            {
                RangeMin = RangeMax;
            }
        }

        public void NormalizeBlockSize()
        {
            int normalized = VisionToolControlValueReader.NormalizeThresholdBlockSize(BlockSize);
            if (BlockSize != normalized)
            {
                BlockSize = normalized;
            }
        }

        public void NotifyAllParametersChanged()
        {
            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(Summary));
        }

        private void SetAndRefresh(System.Action apply, string propertyName)
        {
            apply();
            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Summary));
        }
    }

    internal sealed class FilterToolPresenter : ObservableObject
    {
        private readonly IFilterToolViewModel viewModel;

        public FilterToolPresenter(IFilterToolViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public FilterToolType FilterType
        {
            get => viewModel.FilterType;
            set
            {
                viewModel.FilterType = value;
                OnPropertyChanged(nameof(FilterType));
                OnPropertyChanged(nameof(UsesKernelSize));
                OnPropertyChanged(nameof(UsesMedianKernel));
                OnPropertyChanged(nameof(UsesBilateral));
                OnPropertyChanged(nameof(Summary));
            }
        }

        public int KernelWidth
        {
            get => viewModel.KernelWidth;
            set => SetAndRefresh(() => viewModel.KernelWidth = value, nameof(KernelWidth));
        }

        public int KernelHeight
        {
            get => viewModel.KernelHeight;
            set => SetAndRefresh(() => viewModel.KernelHeight = value, nameof(KernelHeight));
        }

        public int MedianKernelSize
        {
            get => viewModel.MedianKernelSize;
            set => SetAndRefresh(() => viewModel.MedianKernelSize = value, nameof(MedianKernelSize));
        }

        public int Diameter
        {
            get => viewModel.Diameter;
            set => SetAndRefresh(() => viewModel.Diameter = value, nameof(Diameter));
        }

        public int SigmaColor
        {
            get => viewModel.SigmaColor;
            set => SetAndRefresh(() => viewModel.SigmaColor = value, nameof(SigmaColor));
        }

        public int SigmaSpace
        {
            get => viewModel.SigmaSpace;
            set => SetAndRefresh(() => viewModel.SigmaSpace = value, nameof(SigmaSpace));
        }

        public BorderTypes BorderType
        {
            get => viewModel.BorderType;
            set => SetAndRefresh(() => viewModel.BorderType = value, nameof(BorderType));
        }

        public bool UsesKernelSize => viewModel.UsesKernelSize;

        public bool UsesMedianKernel => viewModel.UsesMedianKernel;

        public bool UsesBilateral => viewModel.UsesBilateral;

        public string Summary => viewModel.Summary;

        public FilterToolProperty CreateProperty()
        {
            return viewModel.CreateProperty();
        }

        public void SetKernelPreset(int size)
        {
            KernelWidth = size;
            KernelHeight = size;
            MedianKernelSize = size;
        }

        public void SyncKernelHeightToWidth()
        {
            KernelHeight = KernelWidth;
        }

        public void NotifyAllParametersChanged()
        {
            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(Summary));
        }

        private void SetAndRefresh(System.Action apply, string propertyName)
        {
            apply();
            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Summary));
        }
    }

    internal sealed class MorphologyToolPresenter : ObservableObject
    {
        private readonly IMorphologyToolViewModel viewModel;

        public MorphologyToolPresenter(IMorphologyToolViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        }

        public MorphTypes Operator
        {
            get => viewModel.Operator;
            set => SetAndRefresh(() => viewModel.Operator = value, nameof(Operator));
        }

        public MorphShapes Shape
        {
            get => viewModel.Shape;
            set => SetAndRefresh(() => viewModel.Shape = value, nameof(Shape));
        }

        public int KernelWidth
        {
            get => viewModel.KernelWidth;
            set => SetAndRefresh(() => viewModel.KernelWidth = value, nameof(KernelWidth));
        }

        public int KernelHeight
        {
            get => viewModel.KernelHeight;
            set => SetAndRefresh(() => viewModel.KernelHeight = value, nameof(KernelHeight));
        }

        public int Iterations
        {
            get => viewModel.Iterations;
            set => SetAndRefresh(() => viewModel.Iterations = value, nameof(Iterations));
        }

        public string Summary => viewModel.Summary;

        public MorphologyToolProperty CreateProperty()
        {
            return viewModel.CreateProperty();
        }

        public void SetKernelPreset(int size)
        {
            KernelWidth = size;
            KernelHeight = size;
        }

        public void SyncKernelHeightToWidth()
        {
            KernelHeight = KernelWidth;
        }

        public void NotifyAllParametersChanged()
        {
            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(Summary));
        }

        private void SetAndRefresh(System.Action apply, string propertyName)
        {
            apply();
            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Summary));
        }
    }
}
