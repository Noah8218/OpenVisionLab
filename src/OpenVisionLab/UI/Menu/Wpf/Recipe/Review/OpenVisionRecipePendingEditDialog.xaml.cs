using System;
using System.Windows;
using System.Windows.Input;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionRecipePendingEditDialog : Window
    {
        private readonly OpenVisionRecipePendingEditDialogViewModel viewModel;

        public OpenVisionRecipePendingEditDialog(OpenVisionRecipePendingEditRequest request)
        {
            InitializeComponent();
            viewModel = OpenVisionRecipePendingEditDialogViewModel.Create(
                request ?? new OpenVisionRecipePendingEditRequest());
            DataContext = viewModel;
            viewModel.DecisionRequested += CloseWithDecision;
        }

        public OpenVisionRecipePendingEditDecision Decision { get; private set; } =
            OpenVisionRecipePendingEditDecision.Cancel;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (viewModel.IsRecipeDelete) { pendingCancelButton.Focus(); }
        }

        protected override void OnClosed(EventArgs e)
        {
            viewModel.DecisionRequested -= CloseWithDecision;
            base.OnClosed(e);
        }

        private void CloseWithDecision(OpenVisionRecipePendingEditDecision decision)
        {
            Decision = decision;
            DialogResult = decision != OpenVisionRecipePendingEditDecision.Cancel;
        }
    }
}
