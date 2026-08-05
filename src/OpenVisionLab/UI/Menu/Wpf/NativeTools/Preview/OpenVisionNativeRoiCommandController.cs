using OpenVisionLab.Vision2D.Property;
using OpenVisionLab.Core;
using OpenVisionLab.Contracts;
using System;
using System.Drawing;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionNativeRoiCommandController
    {
        private readonly IDisplayManager displayManager;
        private readonly FrameworkElement ownerElement;
        private readonly Func<string> resolveInputLayer;
        private readonly Action<string> setStatus;

        public OpenVisionNativeRoiCommandController(
            IDisplayManager displayManager,
            FrameworkElement ownerElement,
            Func<string> resolveInputLayer,
            Action<string> setStatus)
        {
            this.displayManager = displayManager ?? throw new ArgumentNullException(nameof(displayManager));
            this.ownerElement = ownerElement ?? throw new ArgumentNullException(nameof(ownerElement));
            this.resolveInputLayer = resolveInputLayer ?? throw new ArgumentNullException(nameof(resolveInputLayer));
            this.setStatus = setStatus ?? throw new ArgumentNullException(nameof(setStatus));
        }

        public void EditSelectedLineRoi(LineToolWpfView lineView)
        {
            if (lineView == null)
            {
                return;
            }

            string inputLayer = resolveInputLayer();
            Bitmap source = displayManager.GetLayerImage(inputLayer);
            if (source == null)
            {
                setStatus("ROI edit NG / input image missing");
                return;
            }

            lineView.EnsureDefaultRoi(source.Width, source.Height);
            LineGaugeProperty selected = lineView.CreateProperty();
            Rectangle initialRoi = new Rectangle(
                selected.CvROI.X,
                selected.CvROI.Y,
                selected.CvROI.Width,
                selected.CvROI.Height);

            using RoiEditorWindow editor = new RoiEditorWindow(source, initialRoi, "ROI");
            editor.Owner = Window.GetWindow(ownerElement);
            if (((IPropertyGridImageEditView)editor).ShowDialog())
            {
                // Keep ROI editing scoped to the selected line; layer routing stays owned by the document.
                lineView.ApplySelectedLineRoi(editor.SelectedRegion);
                setStatus("ROI updated / " + lineView.SelectedLineName);
            }
        }
    }
}
