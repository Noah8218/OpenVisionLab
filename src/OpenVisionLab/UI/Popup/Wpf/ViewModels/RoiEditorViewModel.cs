using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OpenVisionLab
{
    internal sealed class RoiEditorViewModel : INotifyPropertyChanged
    {
        private BitmapSource sourceImage;
        private BitmapSource patternPreviewImage;
        private RoiEditorRegionViewModel selectedRegion;
        private string statusText = "Ready";
        private string coordinateText = "No ROI selected";
        private int selectedX;
        private int selectedY;
        private int selectedWidth;
        private int selectedHeight;

        public RoiEditorViewModel(string mode, double imageWidth, double imageHeight)
        {
            Mode = string.IsNullOrWhiteSpace(mode) ? "ROI" : mode;
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            IsMultiRoiMode = string.Equals(Mode, "MULTI_ROI", StringComparison.OrdinalIgnoreCase);
            IsTrainingMode = string.Equals(Mode, "TRAIN", StringComparison.OrdinalIgnoreCase);
            Title = IsTrainingMode
                ? "Pattern ROI"
                : IsMultiRoiMode
                    ? "Multi ROI"
                    : "ROI";
            PrimaryActionText = IsTrainingMode ? "Use Pattern" : "OK";
            HelpText = IsMultiRoiMode
                ? OpenVisionLanguageService.T("RoiEditor.Help.Multi")
                : OpenVisionLanguageService.T("RoiEditor.Help.Single");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Mode { get; }

        public string Title { get; }

        public string PrimaryActionText { get; }

        public string HelpText { get; }

        public string ZoomOutText => OpenVisionLanguageService.T("RoiEditor.ZoomOut");

        public string ZoomInText => OpenVisionLanguageService.T("RoiEditor.ZoomIn");

        public string FitViewText => OpenVisionLanguageService.T("RoiEditor.FitView");

        public bool IsMultiRoiMode { get; }

        public bool IsTrainingMode { get; }

        public Visibility MultiRoiVisibility => IsMultiRoiMode ? Visibility.Visible : Visibility.Collapsed;

        public Visibility TrainingPreviewVisibility => IsTrainingMode ? Visibility.Visible : Visibility.Collapsed;

        public double ImageWidth { get; }

        public double ImageHeight { get; }

        public ObservableCollection<RoiEditorRegionViewModel> Regions { get; } = new ObservableCollection<RoiEditorRegionViewModel>();

        public BitmapSource SourceImage
        {
            get => sourceImage;
            set => SetField(ref sourceImage, value);
        }

        public BitmapSource PatternPreviewImage
        {
            get => patternPreviewImage;
            set => SetField(ref patternPreviewImage, value);
        }

        public RoiEditorRegionViewModel SelectedRegion
        {
            get => selectedRegion;
            set
            {
                if (ReferenceEquals(selectedRegion, value)) { return; }
                if (selectedRegion != null) { selectedRegion.IsSelected = false; }
                selectedRegion = value;
                if (selectedRegion != null) { selectedRegion.IsSelected = true; }
                OnPropertyChanged();
                UpdateSelectionSummary();
            }
        }

        public string StatusText
        {
            get => statusText;
            set => SetField(ref statusText, value);
        }

        public string CoordinateText
        {
            get => coordinateText;
            private set => SetField(ref coordinateText, value);
        }

        public int SelectedX
        {
            get => selectedX;
            set => SetField(ref selectedX, value);
        }

        public int SelectedY
        {
            get => selectedY;
            set => SetField(ref selectedY, value);
        }

        public int SelectedWidth
        {
            get => selectedWidth;
            set => SetField(ref selectedWidth, value);
        }

        public int SelectedHeight
        {
            get => selectedHeight;
            set => SetField(ref selectedHeight, value);
        }

        public RoiEditorRegionViewModel AddRegion(Rect imageRect)
        {
            RoiEditorRegionViewModel region = new RoiEditorRegionViewModel(imageRect);
            Regions.Add(region);
            SelectedRegion = region;
            UpdateSelectionSummary();
            return region;
        }

        public void SetSingleRegion(Rect imageRect)
        {
            Regions.Clear();
            if (!imageRect.IsEmpty && imageRect.Width > 0 && imageRect.Height > 0)
            {
                AddRegion(imageRect);
                return;
            }

            SelectedRegion = null;
        }

        public void SetRegions(IEnumerable<Rect> imageRects)
        {
            Regions.Clear();
            if (imageRects != null)
            {
                foreach (Rect rect in imageRects)
                {
                    if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) { continue; }
                    Regions.Add(new RoiEditorRegionViewModel(rect));
                }
            }

            SelectedRegion = Regions.Count > 0 ? Regions[0] : null;
            UpdateSelectionSummary();
        }

        public void ReplaceRegion(int index, Rect imageRect)
        {
            if (index < 0 || index >= Regions.Count) { return; }
            Regions[index].ImageRect = imageRect;
            if (ReferenceEquals(SelectedRegion, Regions[index]))
            {
                UpdateSelectionSummary();
            }
        }

        public void RemoveSelectedRegion()
        {
            if (SelectedRegion == null) { return; }

            int index = Regions.IndexOf(SelectedRegion);
            Regions.Remove(SelectedRegion);
            if (Regions.Count == 0)
            {
                SelectedRegion = null;
                return;
            }

            SelectedRegion = Regions[Math.Max(0, Math.Min(index, Regions.Count - 1))];
        }

        public void ClearRegions()
        {
            Regions.Clear();
            SelectedRegion = null;
            UpdateSelectionSummary();
        }

        public int IndexOf(RoiEditorRegionViewModel region)
        {
            return region == null ? -1 : Regions.IndexOf(region);
        }

        public void UpdateSelectionSummary()
        {
            if (SelectedRegion == null || SelectedRegion.ImageRect.IsEmpty)
            {
                CoordinateText = "No ROI selected";
                SelectedX = 0;
                SelectedY = 0;
                SelectedWidth = 0;
                SelectedHeight = 0;
                return;
            }

            Rect rect = SelectedRegion.ImageRect;
            SelectedX = Math.Max(0, (int)Math.Round(rect.X));
            SelectedY = Math.Max(0, (int)Math.Round(rect.Y));
            SelectedWidth = Math.Max(0, (int)Math.Round(rect.Width));
            SelectedHeight = Math.Max(0, (int)Math.Round(rect.Height));
            CoordinateText = $"X {SelectedX} / Y {SelectedY} / W {SelectedWidth} / H {SelectedHeight}";
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal sealed class RoiEditorRegionViewModel : INotifyPropertyChanged
    {
        private Rect imageRect;
        private bool isSelected;

        public RoiEditorRegionViewModel(Rect imageRect)
        {
            Id = Guid.NewGuid();
            this.imageRect = imageRect;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id { get; }

        public Rect ImageRect
        {
            get => imageRect;
            set
            {
                if (imageRect == value) { return; }
                imageRect = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value) { return; }
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public string DisplayText
        {
            get
            {
                if (ImageRect.IsEmpty) { return "Empty"; }
                return $"{Math.Round(ImageRect.X):0}, {Math.Round(ImageRect.Y):0}, {Math.Round(ImageRect.Width):0} x {Math.Round(ImageRect.Height):0}";
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
