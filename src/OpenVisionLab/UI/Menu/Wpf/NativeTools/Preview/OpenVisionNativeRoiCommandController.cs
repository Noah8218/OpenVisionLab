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

            Action<string> editAction = PropertyGridEditorFactory.GetEditActionCallback(ownerElement);
            using IDisposable imageEditScope = PropertyGridEditorFactory.BeginImageEditWriteScope(ownerElement);
            string inputLayer = resolveInputLayer();
            Bitmap source = displayManager.GetLayerImage(inputLayer);
            if (source == null)
            {
                setStatus("ROI edit NG / input image missing");
                return;
            }

            string selectedLine = lineView.SelectedLineName;
            LineGaugeProperty selected = lineView.CreateProperty();
            Rectangle initialRoi = selected.CvROI.Width > 0 && selected.CvROI.Height > 0
                ? new Rectangle(
                    selected.CvROI.X,
                    selected.CvROI.Y,
                    selected.CvROI.Width,
                    selected.CvROI.Height)
                : new Rectangle(0, 0, source.Width, source.Height);

            try
            {
                using RoiEditorWindow editor = new RoiEditorWindow(source, initialRoi, "ROI");
                editor.Owner = Window.GetWindow(ownerElement);
                editAction?.Invoke("Workbench.Action.RoiEditing");
                if (PropertyGridEditorFactory.ShowImageEdit(editor, ownerElement) && lineView.SelectedLineName == selectedLine)
                {
                    // Keep ROI editing scoped to the selected line; layer routing stays owned by the document.
                    lineView.ApplySelectedLineRoi(editor.SelectedRegion);
                    setStatus("ROI updated / " + lineView.SelectedLineName);
                    editAction?.Invoke("Workbench.Action.RoiApplied");
                }
                else { editAction?.Invoke("Workbench.Action.RoiEditCanceled"); }
            }
            catch { editAction?.Invoke("Workbench.Status.Error"); throw; }
        }
    }
}
