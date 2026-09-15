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
        }

        public OpenVisionRecipePendingEditDecision Decision { get; private set; } =
            OpenVisionRecipePendingEditDecision.Cancel;

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            CloseWithDecision(OpenVisionRecipePendingEditDecision.ApplyAndContinue);
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            CloseWithDecision(OpenVisionRecipePendingEditDecision.Discard);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWithDecision(OpenVisionRecipePendingEditDecision.Cancel);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                return;
            }

            CloseWithDecision(OpenVisionRecipePendingEditDecision.Cancel);
            e.Handled = true;
        }

        private void CloseWithDecision(OpenVisionRecipePendingEditDecision decision)
        {
            Decision = decision;
            DialogResult = decision != OpenVisionRecipePendingEditDecision.Cancel;
        }
    }
}
