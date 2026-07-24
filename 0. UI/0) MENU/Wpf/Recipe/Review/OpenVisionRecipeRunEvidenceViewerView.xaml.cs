using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionRecipeRunEvidenceViewerView : UserControl, IDisposable
    {
        private bool hasSourceImage;
        private bool hasDrawingImage;
        private bool disposed;
        private OpenVisionRecipeRunEvidence evidence;
        private OpenVisionRecipeRunEvidenceDrawing selectedDrawing;
        private bool selectingDrawing;

        public OpenVisionRecipeRunEvidenceViewerView()
        {
            InitializeComponent();
            txtDrawingSelectorLabel.Text = OpenVisionRecipeText.Local("저장된 Step 드로잉", "Stored Step drawing");
        }

        public bool HasSourceImage => hasSourceImage;

        public bool HasDrawingImage => hasDrawingImage;

        internal int DrawingCount => evidence?.Drawings?.Count ?? 0;

        internal string SelectedStepText => selectedDrawing?.StepText ?? string.Empty;

        internal string LoadError { get; private set; } = string.Empty;

        internal bool TrySetEvidence(OpenVisionRecipeRunEvidence evidence)
        {
            if (evidence == null)
            {
                LoadError = "Evidence is missing.";
                return false;
            }

            try
            {
                LoadError = string.Empty;
                using Bitmap source = LoadBitmap(evidence.OriginalImagePath, "source");
                sourceViewer.SetLayer(
                    OpenVisionRecipeText.Local("원본 | ", "Source | ") + evidence.SampleName,
                    source,
                    evidence.OriginalImagePath);
                hasSourceImage = true;
                this.evidence = evidence;
                selectingDrawing = true;
                cmbStoredDrawing.ItemsSource = evidence.Drawings;
                cmbStoredDrawing.SelectedItem = evidence.DefaultDrawing;
                selectingDrawing = false;
                cmbStoredDrawing.Visibility = evidence.Drawings.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                return TrySetDrawing(evidence.DefaultDrawing);
            }
            catch (Exception exception)
            {
                hasSourceImage = false;
                hasDrawingImage = false;
                LoadError = exception.GetType().Name + ": " + exception.Message;
                return false;
            }
        }

        private void StoredDrawing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectingDrawing || disposed)
            {
                return;
            }

            TrySetDrawing(cmbStoredDrawing.SelectedItem as OpenVisionRecipeRunEvidenceDrawing);
        }

        private bool TrySetDrawing(OpenVisionRecipeRunEvidenceDrawing drawing)
        {
            if (evidence == null || drawing == null)
            {
                hasDrawingImage = false;
                LoadError = "Stored drawing evidence is missing.";
                return false;
            }

            try
            {
                using Bitmap image = LoadBitmap(drawing.DrawingImagePath, "drawing");
                drawingViewer.SetLayer(
                    OpenVisionRecipeText.Local("검출 드로잉 | ", "Detection drawing | ") + drawing.StepText,
                    image,
                    drawing.DrawingImagePath);
                selectedDrawing = drawing;
                hasDrawingImage = true;
                LoadError = string.Empty;
                txtEvidenceStatus.Text = evidence.BuildStatusText(drawing);
                return true;
            }
            catch (Exception exception)
            {
                hasDrawingImage = false;
                LoadError = exception.GetType().Name + ": " + exception.Message;
                txtEvidenceStatus.Text = LoadError;
                return false;
            }
        }

        private static Bitmap LoadBitmap(string path, string role)
        {
            try
            {
                using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using System.Drawing.Image decoded = System.Drawing.Image.FromStream(stream, useEmbeddedColorManagement: false, validateImageData: true);
                return new Bitmap(decoded);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(role + " image could not be loaded: " + path, exception);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            cmbStoredDrawing.ItemsSource = null;
            evidence = null;
            selectedDrawing = null;
            sourceViewer?.Dispose();
            drawingViewer?.Dispose();
        }
    }
}
