using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelineTextPrompt : Form
    {
        public FormVisionPipelineTextPrompt()
            : this("Save Sample", "Sample Name", "Contour_TextSymbols_20260614")
        {
        }

        private FormVisionPipelineTextPrompt(string title, string label, string defaultValue)
        {
            InitializeComponent();
            Text = title;
            titleLabel.Text = title;
            promptLabel.Text = label;
            textBox.Text = defaultValue ?? string.Empty;

            VisionPipelineDialogStyle.Apply(this);
            VisionPipelineDialogStyle.StyleButton(okButton, primary: true);
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            textBox.SelectAll();
            textBox.Focus();
        }

        public static string Show(IWin32Window owner, string title, string label, string defaultValue)
        {
            using (FormVisionPipelineTextPrompt prompt = new FormVisionPipelineTextPrompt(title, label, defaultValue))
            {
                return VisionPipelineDialogService.ShowDialog(prompt, owner) == DialogResult.OK
                    ? prompt.textBox.Text
                    : string.Empty;
            }
        }
    }
}
