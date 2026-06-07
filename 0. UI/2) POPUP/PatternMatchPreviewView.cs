using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    public sealed class PatternMatchPreviewView : UserControl
    {
        private readonly Label titleLabel = new Label();
        private readonly PictureBox previewBox = new PictureBox();

        public PatternMatchPreviewView()
        {
            BackColor = Color.FromArgb(238, 243, 247);
            Padding = new Padding(10);

            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 28;
            titleLabel.Text = "Pattern Preview";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            titleLabel.ForeColor = Color.FromArgb(55, 66, 82);

            previewBox.Dock = DockStyle.Fill;
            previewBox.BackColor = Color.FromArgb(18, 20, 24);
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;

            Controls.Add(previewBox);
            Controls.Add(titleLabel);
        }

        public void SetPreview(Image image)
        {
            Image previousImage = previewBox.Image;
            previewBox.Image = image;
            if (previousImage != null && !ReferenceEquals(previousImage, image))
            {
                previousImage.Dispose();
            }
        }

        public void ClearPreview()
        {
            Image previousImage = previewBox.Image;
            previewBox.Image = null;
            previousImage?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPreview();
                titleLabel.Dispose();
                previewBox.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
