using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal static class VisionPipelineDialogStyle
    {
        private static readonly Color SurfaceColor = Color.FromArgb(238, 242, 246);
        private static readonly Color FieldColor = Color.FromArgb(250, 252, 253);
        private static readonly Color AccentColor = Color.FromArgb(35, 85, 132);
        private static readonly Color BorderColor = Color.FromArgb(47, 111, 171);
        private static readonly Color HoverColor = Color.FromArgb(232, 241, 250);
        private static readonly Color DownColor = Color.FromArgb(216, 232, 247);
        private static readonly Font DialogFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        public static void Apply(Form dialog)
        {
            if (dialog == null || dialog.IsDisposed)
            {
                return;
            }

            dialog.Font = DialogFont;
            if (dialog.BackColor == SystemColors.Control || dialog.BackColor == Color.Empty)
            {
                dialog.BackColor = SurfaceColor;
            }

            dialog.ShowInTaskbar = false;
            ApplyToChildren(dialog);
        }

        private static void ApplyToChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                ApplyControl(child);
                if (child.HasChildren)
                {
                    ApplyToChildren(child);
                }
            }
        }

        private static void ApplyControl(Control control)
        {
            switch (control)
            {
                case Button button:
                    StyleButton(button);
                    break;
                case TextBox textBox:
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    if (!textBox.ReadOnly)
                    {
                        textBox.BackColor = Color.White;
                    }
                    break;
                case ComboBox comboBox:
                    comboBox.FlatStyle = FlatStyle.Popup;
                    break;
                case ListBox listBox:
                    listBox.BackColor = Color.White;
                    listBox.ForeColor = Color.FromArgb(20, 38, 58);
                    break;
                case Label label when label.ForeColor == SystemColors.ControlText:
                    label.ForeColor = Color.FromArgb(35, 85, 132);
                    break;
            }
        }

        public static void StyleButton(Button button, bool primary = false)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button.Cursor = Cursors.Hand;
            button.BackColor = primary ? AccentColor : FieldColor;
            button.ForeColor = primary ? Color.White : AccentColor;
            button.FlatAppearance.BorderColor = primary ? AccentColor : BorderColor;
            button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(47, 111, 171) : HoverColor;
            button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(31, 73, 116) : DownColor;
        }
    }
}
