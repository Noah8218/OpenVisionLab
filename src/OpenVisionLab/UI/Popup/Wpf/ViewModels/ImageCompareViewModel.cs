using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using DrawingColor = System.Drawing.Color;
using MediaBrush = System.Windows.Media.Brush;
using MediaBrushes = System.Windows.Media.Brushes;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace OpenVisionLab
{
    public sealed class ImageCompareViewModel : INotifyPropertyChanged, IDisposable
    {
        public const string ImageFilter = "Image Files (*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff)|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff";
        public const int MinimumCompareImages = 2;
        public const int MaximumCompareImages = 16;

        private bool syncViewEnabled = true;
        private string statusText = string.Empty;
        private string xyText = "XY[-,-]";
        private string colorText = "RGB[-] GV[-]";
        private string deltaText = "Delta[-]";
        private MediaBrush swatchBrush = MediaBrushes.Black;
        private int gridColumns = 2;
        private readonly ImageCompareDirectoryPolicy directoryPolicy = new ImageCompareDirectoryPolicy();

        public ImageCompareViewModel()
        {
            OpenVisionLanguageService.LanguageChanged += OnLanguageChanged;
            ResetSlots(MinimumCompareImages);
            ResetStatus();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ImageCompareSlotViewModel> Slots { get; } = new ObservableCollection<ImageCompareSlotViewModel>();

        public string TitleText => OpenVisionLanguageService.T("ImageCompare.Title");
        public string LoadImagesText => OpenVisionLanguageService.T("ImageCompare.LoadImages");
        public string FitAllText => OpenVisionLanguageService.T("ImageCompare.FitAll");
        public string SyncText => syncViewEnabled
            ? OpenVisionLanguageService.T("ImageCompare.SyncOn")
            : OpenVisionLanguageService.T("ImageCompare.SyncOff");

        public bool SyncViewEnabled
        {
            get => syncViewEnabled;
            set
            {
                if (syncViewEnabled == value) { return; }
                syncViewEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SyncText));
            }
        }

        public string StatusText
        {
            get => statusText;
            private set => SetField(ref statusText, value);
        }

        public string XyText
        {
            get => xyText;
            private set => SetField(ref xyText, value);
        }

        public string ColorText
        {
            get => colorText;
            private set => SetField(ref colorText, value);
        }

        public string DeltaText
        {
            get => deltaText;
            private set => SetField(ref deltaText, value);
        }

        public MediaBrush SwatchBrush
        {
            get => swatchBrush;
            private set => SetField(ref swatchBrush, value);
        }

        public int GridColumns
        {
            get => gridColumns;
            private set => SetField(ref gridColumns, value);
        }

        internal string InitialImageDirectory => directoryPolicy.ResolveInitialDirectory();

        public void LoadImages(params string[] imagePaths)
        {
            string[] validPaths = NormalizeImagePaths(imagePaths);
            int slotCount = Math.Max(MinimumCompareImages, validPaths.Length);
            ResetSlots(slotCount);

            directoryPolicy.RememberImageDirectory(validPaths);

            for (int index = 0; index < validPaths.Length; index++)
            {
                Slots[index].Load(validPaths[index]);
            }

            RefreshMismatchState();
            ResetStatus();
        }

        public void SelectSlot(ImageCompareSlotViewModel slot)
        {
            if (slot == null) { return; }

            foreach (ImageCompareSlotViewModel item in Slots)
            {
                item.IsSelected = ReferenceEquals(item, slot);
            }
        }

        public void FitAll()
        {
            foreach (ImageCompareSlotViewModel slot in Slots)
            {
                slot.Zoom = 1.0;
            }

            ResetStatus();
        }

        public void ApplyZoom(ImageCompareSlotViewModel slot, int wheelDelta)
        {
            if (slot == null || !slot.IsLoaded || wheelDelta == 0) { return; }

            double factor = wheelDelta > 0 ? 1.12 : 1.0 / 1.12;
            double nextZoom = ClampZoom(slot.Zoom * factor);
            if (SyncViewEnabled)
            {
                foreach (ImageCompareSlotViewModel item in Slots.Where(item => item.IsLoaded))
                {
                    item.Zoom = nextZoom;
                }
            }
            else
            {
                slot.Zoom = nextZoom;
            }
        }

        public void UpdatePixelStatus(ImageCompareSlotViewModel slot, int x, int y)
        {
            if (slot?.Bitmap == null || x < 0 || y < 0 || x >= slot.Bitmap.Width || y >= slot.Bitmap.Height)
            {
                ResetStatus();
                return;
            }

            SelectSlot(slot);
            DrawingColor color = slot.Bitmap.GetPixel(x, y);
            int gv = (int)Math.Round((color.R + color.G + color.B) / 3.0);
            StatusText = $"{slot.Index + 1:00} {slot.DisplayName}";
            XyText = string.Format(CultureInfo.CurrentCulture, "XY[{0},{1}]", x, y);
            ColorText = string.Format(CultureInfo.CurrentCulture, "RGB[{0},{1},{2}] GV[{3}]", color.R, color.G, color.B, gv);
            DeltaText = ResolveDeltaText(slot, x, y, gv);
            SwatchBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SwatchBrush.Freeze();
        }

        internal static bool TryMapDisplayedPoint(
            int pixelWidth,
            int pixelHeight,
            double hostWidth,
            double hostHeight,
            double pointX,
            double pointY,
            out int x,
            out int y)
        {
            x = -1;
            y = -1;
            if (pixelWidth <= 0 || pixelHeight <= 0 || hostWidth <= 0 || hostHeight <= 0)
            {
                return false;
            }

            double scale = Math.Min(hostWidth / pixelWidth, hostHeight / pixelHeight);
            if (scale <= 0 || double.IsInfinity(scale) || double.IsNaN(scale))
            {
                return false;
            }

            double displayWidth = pixelWidth * scale;
            double displayHeight = pixelHeight * scale;
            double offsetX = (hostWidth - displayWidth) / 2.0;
            double offsetY = (hostHeight - displayHeight) / 2.0;
            double imageX = (pointX - offsetX) / scale;
            double imageY = (pointY - offsetY) / scale;

            if (imageX < 0 || imageY < 0 || imageX >= pixelWidth || imageY >= pixelHeight)
            {
                return false;
            }

            x = Math.Max(0, Math.Min(pixelWidth - 1, (int)Math.Floor(imageX)));
            y = Math.Max(0, Math.Min(pixelHeight - 1, (int)Math.Floor(imageY)));
            return true;
        }

        public void ResetStatus()
        {
            int loadedCount = Slots.Count(slot => slot.IsLoaded);
            StatusText = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("ImageCompare.ImagesCount"),
                loadedCount,
                Slots.Count);
            XyText = "XY[-,-]";
            ColorText = "RGB[-] GV[-]";
            DeltaText = HasSizeMismatch() ? OpenVisionLanguageService.T("ImageCompare.SizeMismatchDetected") : "Delta[-]";
            SwatchBrush = MediaBrushes.Black;
        }

        public void Dispose()
        {
            OpenVisionLanguageService.LanguageChanged -= OnLanguageChanged;
            foreach (ImageCompareSlotViewModel slot in Slots)
            {
                slot.Dispose();
            }
        }

        private void ResetSlots(int count)
        {
            count = Math.Max(MinimumCompareImages, Math.Min(MaximumCompareImages, count));
            foreach (ImageCompareSlotViewModel slot in Slots)
            {
                slot.Dispose();
            }

            Slots.Clear();
            for (int index = 0; index < count; index++)
            {
                Slots.Add(new ImageCompareSlotViewModel(index));
            }

            GridColumns = Math.Max(2, (int)Math.Ceiling(Math.Sqrt(count)));
        }

        private string ResolveDeltaText(ImageCompareSlotViewModel slot, int x, int y, int gv)
        {
            ImageCompareSlotViewModel reference = Slots.FirstOrDefault(item => item.IsLoaded);
            if (reference?.Bitmap == null || ReferenceEquals(reference, slot))
            {
                return "Delta[-]";
            }

            if (x >= reference.Bitmap.Width || y >= reference.Bitmap.Height)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionLanguageService.T("ImageCompare.DeltaOut"),
                    reference.Index + 1);
            }

            DrawingColor referenceColor = reference.Bitmap.GetPixel(x, y);
            int referenceGv = (int)Math.Round((referenceColor.R + referenceColor.G + referenceColor.B) / 3.0);
            string label = string.Format(
                CultureInfo.CurrentCulture,
                OpenVisionLanguageService.T("ImageCompare.DeltaVs"),
                reference.Index + 1);
            return string.Format(CultureInfo.CurrentCulture, "{0}: {1:+#;-#;0}", label, gv - referenceGv);
        }

        private void RefreshMismatchState()
        {
            ImageCompareSlotViewModel reference = Slots.FirstOrDefault(slot => slot.IsLoaded);
            foreach (ImageCompareSlotViewModel slot in Slots)
            {
                slot.IsSizeMismatch = reference != null
                    && slot.IsLoaded
                    && (slot.Width != reference.Width || slot.Height != reference.Height);
                slot.RefreshHeader();
            }
        }

        private bool HasSizeMismatch()
        {
            ImageCompareSlotViewModel reference = Slots.FirstOrDefault(slot => slot.IsLoaded);
            if (reference == null) { return false; }

            return Slots.Any(slot => slot.IsLoaded && (slot.Width != reference.Width || slot.Height != reference.Height));
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(LoadImagesText));
            OnPropertyChanged(nameof(FitAllText));
            OnPropertyChanged(nameof(SyncText));
            foreach (ImageCompareSlotViewModel slot in Slots)
            {
                slot.RefreshHeader();
            }

            ResetStatus();
        }

        private static string[] NormalizeImagePaths(string[] imagePaths)
        {
            return (imagePaths ?? Array.Empty<string>())
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => path.Trim())
                .Where(File.Exists)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(MaximumCompareImages)
                .ToArray();
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private static double ClampZoom(double zoom)
        {
            if (double.IsNaN(zoom) || double.IsInfinity(zoom)) { return 1.0; }
            return Math.Max(0.5, Math.Min(6.0, zoom));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class ImageCompareSlotViewModel : INotifyPropertyChanged, IDisposable
    {
        private string filePath = string.Empty;
        private string displayName = string.Empty;
        private string headerText = string.Empty;
        private string formatText = string.Empty;
        private ImageCompareImageResource imageResource;
        private bool isSelected;
        private bool isSizeMismatch;
        private double zoom = 1.0;

        public ImageCompareSlotViewModel(int index)
        {
            Index = index;
            RefreshHeader();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Index { get; }
        public int Width => imageResource?.Bitmap?.Width ?? 0;
        public int Height => imageResource?.Bitmap?.Height ?? 0;
        public bool IsLoaded => imageResource?.Bitmap != null && Width > 0 && Height > 0;

        public string DisplayName
        {
            get => displayName;
            private set => SetField(ref displayName, value);
        }

        public string HeaderText
        {
            get => headerText;
            private set => SetField(ref headerText, value);
        }

        public string EmptyText => OpenVisionLanguageService.T("ImageCompare.NoImage");

        public BitmapSource Source => imageResource?.Source;

        public Bitmap Bitmap => imageResource?.Bitmap;

        public bool IsSelected
        {
            get => isSelected;
            set => SetField(ref isSelected, value);
        }

        public bool IsSizeMismatch
        {
            get => isSizeMismatch;
            set => SetField(ref isSizeMismatch, value);
        }

        public double Zoom
        {
            get => zoom;
            set => SetField(ref zoom, Math.Max(0.5, Math.Min(6.0, value)));
        }

        public void Load(string path)
        {
            DisposeBitmap();
            filePath = path ?? string.Empty;
            DisplayName = string.IsNullOrWhiteSpace(filePath)
                ? string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("ImageCompare.ImageSlot"), Index + 1)
                : Path.GetFileName(filePath);

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                RefreshHeader();
                return;
            }

            imageResource = ImageCompareImageResource.Load(filePath);
            formatText = imageResource.FormatText;
            OnPropertyChanged(nameof(Bitmap));
            OnPropertyChanged(nameof(Source));
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(IsLoaded));
            RefreshHeader();
        }

        public void RefreshHeader()
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                DisplayName = string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("ImageCompare.ImageSlot"), Index + 1);
            }

            string sizeText = IsLoaded
                ? $"{Width}x{Height}"
                : OpenVisionLanguageService.T("ImageCompare.NoImage");
            string resolvedFormat = string.IsNullOrWhiteSpace(formatText)
                ? OpenVisionLanguageService.T("ImageCompare.Ready")
                : formatText;
            string mismatch = IsSizeMismatch ? "  " + OpenVisionLanguageService.T("ImageCompare.SizeMismatch") : string.Empty;
            HeaderText = $"{Index + 1:00}  {DisplayName}    {sizeText}    {resolvedFormat}{mismatch}";
            OnPropertyChanged(nameof(EmptyText));
        }

        public void Dispose()
        {
            DisposeBitmap();
        }

        private void DisposeBitmap()
        {
            imageResource?.Dispose();
            imageResource = null;
            formatText = string.Empty;
            OnPropertyChanged(nameof(Bitmap));
            OnPropertyChanged(nameof(Source));
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(IsLoaded));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
