using System;
using System.Windows.Forms;

namespace OpenVisionLab
{
    internal static class VisionPipelineDialogService
    {
        public static DialogResult ShowDialog(Form dialog, IWin32Window owner)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            Form ownerForm = ResolveOwnerForm(owner);
            VisionPipelineDialogStyle.Apply(dialog);
            dialog.StartPosition = ownerForm == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
            dialog.ShowInTaskbar = false;
            bool restoreOwnerTopMost = ownerForm?.TopMost == true;
            bool restoreDialogTopMost = dialog.TopMost;

            dialog.Shown += OnDialogShown;
            try
            {
                if (restoreOwnerTopMost)
                {
                    ownerForm.TopMost = false;
                }

                dialog.TopMost = false;
                return ownerForm == null
                    ? dialog.ShowDialog(owner)
                    : dialog.ShowDialog(ownerForm);
            }
            finally
            {
                dialog.Shown -= OnDialogShown;
                dialog.TopMost = restoreDialogTopMost;
                if (restoreOwnerTopMost && ownerForm != null && !ownerForm.IsDisposed)
                {
                    ownerForm.TopMost = true;
                }
            }
        }

        private static void OnDialogShown(object sender, EventArgs e)
        {
            if (sender is Form form)
            {
                form.BringToFront();
                form.Activate();
            }
        }

        private static Form ResolveOwnerForm(IWin32Window owner)
        {
            if (owner is Form form)
            {
                return form;
            }

            if (owner is Control control)
            {
                return control.FindForm();
            }

            return Form.ActiveForm;
        }
    }
}
