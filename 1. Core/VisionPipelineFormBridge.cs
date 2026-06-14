using Lib.OpenCV.Pipeline;
using OpenVisionLab.MessageDialogs;
using OpenVisionLab.Vision._1._Tools.OpenCV;
using RJCodeUI_M1.RJControls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal static class VisionPipelineFormBridge
    {
        public static void AttachAddButton(
            Control anchorButton,
            Func<OpenCvPropertyBase> propertyAccessor,
            RJComboBox inputComboBox,
            RJComboBox outputComboBox)
        {
            AttachAddButton(
                anchorButton,
                () =>
                {
                    OpenCvPropertyBase property = propertyAccessor();
                    return VisionPipelineStepBuilder.FromProperty(
                        property,
                        GetComboText(inputComboBox, "Main"),
                        GetComboText(outputComboBox, string.Format("{0}_Output", property?.NAME ?? "Tool")));
                });
        }

        public static void AttachAddButton(
            Control anchorButton,
            Func<VisionPipelineStep> stepAccessor)
        {
            if (anchorButton == null || anchorButton.Parent == null || stepAccessor == null)
            {
                return;
            }

            Control parent = anchorButton.Parent;
            if (parent.Controls.ContainsKey("btnAddToPipeline"))
            {
                return;
            }

            Control detailsButton = parent.Controls.Find("btnResult", false).FirstOrDefault();
            Rectangle bounds = GetButtonBounds(anchorButton, detailsButton);

            Button button = new Button
            {
                Name = "btnAddToPipeline",
                Text = "Add Pipeline",
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height,
                Anchor = detailsButton?.Anchor ?? anchorButton.Anchor,
                BackColor = Color.FromArgb(250, 252, 253),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(35, 85, 132),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 171);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 241, 247);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 246, 251);

            button.Click += (sender, e) =>
            {
                try
                {
                    VisionPipelineStep step = VisionPipelineAppendService.AddStep(stepAccessor());
                    VisionMessageBox.Info(null, "Pipeline", string.Format("Added to Pipeline.\r\nStep: {0}", step.Name));
                }
                catch (Exception ex)
                {
                    VisionMessageBox.Error(null, "Pipeline", ex.GetBaseException().Message, ex.ToString());
                }
            };

            parent.Controls.Add(button);
            button.BringToFront();
        }

        private static Rectangle GetButtonBounds(Control anchorButton, Control detailsButton)
        {
            if (detailsButton != null && detailsButton.Width > 120)
            {
                int gap = 8;
                int originalWidth = detailsButton.Width;
                int halfWidth = (originalWidth - gap) / 2;
                detailsButton.Width = halfWidth;

                return new Rectangle(
                    detailsButton.Left + halfWidth + gap,
                    detailsButton.Top,
                    originalWidth - halfWidth - gap,
                    detailsButton.Height);
            }

            return new Rectangle(
                anchorButton.Left,
                Math.Max(4, anchorButton.Top - anchorButton.Height - 8),
                anchorButton.Width,
                anchorButton.Height);
        }

        private static string GetComboText(RJComboBox comboBox, string fallback)
        {
            if (comboBox == null)
            {
                return fallback;
            }

            string selected = Convert.ToString(comboBox.SelectedItem);
            if (!string.IsNullOrWhiteSpace(selected))
            {
                return selected;
            }

            return string.IsNullOrWhiteSpace(comboBox.Texts) ? fallback : comboBox.Texts;
        }
    }
}
