using System.Drawing;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal sealed partial class FormVisionPipelinePromptPreview : Form
    {
        private TextBox textBox;

        public string PromptText => textBox.Text;

        public FormVisionPipelinePromptPreview()
            : this("Describe the image inspection goal and expected output layers here.")
        {
        }

        public FormVisionPipelinePromptPreview(string promptText)
        {
            InitializeComponent();
            textBox.Text = promptText ?? string.Empty;
            VisionPipelineDialogStyle.Apply(this);
            VisionPipelineDialogStyle.StyleButton(copyButton, primary: true);
        }
    }
}
