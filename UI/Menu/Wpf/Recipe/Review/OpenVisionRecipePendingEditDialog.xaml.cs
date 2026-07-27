using System;
using System.Globalization;
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

    public sealed class OpenVisionRecipePendingEditDialogViewModel
    {
        private OpenVisionRecipePendingEditDialogViewModel()
        {
        }

        public string TitleText { get; private set; } = string.Empty;

        public string HeadingText { get; private set; } = string.Empty;

        public string DetailText { get; private set; } = string.Empty;

        public string CurrentContextText { get; private set; } = string.Empty;

        public string TargetContextText { get; private set; } = string.Empty;

        public string ApplyButtonText { get; private set; } = string.Empty;

        public string DiscardButtonText { get; private set; } = string.Empty;

        public string CancelButtonText { get; private set; } = string.Empty;

        internal static OpenVisionRecipePendingEditDialogViewModel Create(
            OpenVisionRecipePendingEditRequest request)
        {
            string transitionText = ResolveTransitionText(request.Kind);
            string target = string.IsNullOrWhiteSpace(request.TargetName)
                ? OpenVisionRecipeText.Local("현재 화면 닫기", "Close the current view")
                : request.TargetName.Trim();
            return new OpenVisionRecipePendingEditDialogViewModel
            {
                TitleText = OpenVisionRecipeText.Local("적용하지 않은 Recipe 변경", "Unapplied recipe changes"),
                HeadingText = OpenVisionRecipeText.Local(
                    "Step 편집 내용을 어떻게 처리할까요?",
                    "How should the Step edit be handled?"),
                DetailText = string.Format(
                    CultureInfo.CurrentCulture,
                    OpenVisionRecipeText.Local(
                        "{0} 전에 적용, 폐기 또는 취소를 선택하세요. 적용에 실패하면 현재 편집 화면에 남습니다.",
                        "Choose apply, discard, or cancel before {0}. If apply fails, the current editor stays open."),
                    transitionText),
                CurrentContextText = OpenVisionRecipeText.Local("현재: ", "Current: ")
                    + BuildCurrentContext(request),
                TargetContextText = OpenVisionRecipeText.Local("이동 대상: ", "Continue to: ")
                    + target,
                ApplyButtonText = OpenVisionRecipeText.Local("적용 후 계속", "Apply and continue"),
                DiscardButtonText = OpenVisionRecipeText.Local("변경 폐기", "Discard changes"),
                CancelButtonText = OpenVisionRecipeText.Local("취소", "Cancel")
            };
        }

        private static string BuildCurrentContext(OpenVisionRecipePendingEditRequest request)
        {
            string[] parts =
            {
                request.RecipeName?.Trim() ?? string.Empty,
                request.PipelineName?.Trim() ?? string.Empty,
                request.StepName?.Trim() ?? string.Empty
            };
            string value = string.Join(" / ", Array.FindAll(parts, part => !string.IsNullOrWhiteSpace(part)));
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static string ResolveTransitionText(OpenVisionRecipePendingEditTransitionKind kind)
        {
            return kind switch
            {
                OpenVisionRecipePendingEditTransitionKind.Step =>
                    OpenVisionRecipeText.Local("다른 Step으로 이동하기", "moving to another Step"),
                OpenVisionRecipePendingEditTransitionKind.Pipeline =>
                    OpenVisionRecipeText.Local("다른 Pipeline으로 이동하기", "moving to another Pipeline"),
                OpenVisionRecipePendingEditTransitionKind.Recipe =>
                    OpenVisionRecipeText.Local("다른 Recipe로 이동하기", "moving to another Recipe"),
                _ => OpenVisionRecipeText.Local("Recipe Manager를 닫기", "closing Recipe Manager")
            };
        }
    }
}
