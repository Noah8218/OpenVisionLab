using System;
using System.Globalization;
using System.Windows.Input;
using OpenVisionLab.Mvvm;

namespace OpenVisionLab
{
    public sealed class OpenVisionRecipePendingEditDialogViewModel : ObservableObject
    {
        private readonly OpenVisionRecipePendingEditRequest request;
        private string errorText = string.Empty;

        private OpenVisionRecipePendingEditDialogViewModel(OpenVisionRecipePendingEditRequest request)
        {
            this.request = request;
            SaveCommand = new RelayCommand(() => Choose(OpenVisionRecipePendingEditDecision.ApplyAndContinue));
            DiscardCommand = new RelayCommand(() => Choose(OpenVisionRecipePendingEditDecision.Discard));
            CancelCommand = new RelayCommand(() => Choose(OpenVisionRecipePendingEditDecision.Cancel));
        }

        public event Action<OpenVisionRecipePendingEditDecision> DecisionRequested = delegate { };

        public ICommand SaveCommand { get; }
        public ICommand DiscardCommand { get; }
        public ICommand CancelCommand { get; }
        public bool ShowLegacyContext => !request.IsWorkbenchCopy && !IsDeleteConfirmation;
        public bool ShowDiscard => !IsDeleteConfirmation;
        public bool IsStepDelete => request.IsStepDelete;
        public bool IsRecipeDelete => request.IsRecipeDelete;
        public bool IsDeleteConfirmation => request.IsStepDelete || request.IsRecipeDelete;
        public bool IsApplyDefault => !request.IsRecipeDelete;
        public bool IsCancelDefault => request.IsRecipeDelete;
        public string ErrorText => errorText;
        public bool HasError => !string.IsNullOrWhiteSpace(errorText);

        private void Choose(OpenVisionRecipePendingEditDecision decision)
        {
            if (decision == OpenVisionRecipePendingEditDecision.ApplyAndContinue && request.IsWorkbenchCopy && request.TrySave?.Invoke() != true)
            {
                errorText = request.SaveError?.Invoke() ?? OpenVisionLanguageService.T("Workbench.Save.Failed");
                OnPropertyChanged(nameof(ErrorText));
                OnPropertyChanged(nameof(HasError));
                return;
            }
            DecisionRequested(decision);
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
            if (request.IsWorkbenchCopy || request.IsStepDelete || request.IsRecipeDelete)
            {
                bool deleting = request.IsStepDelete || request.IsRecipeDelete;
                string titleKey = request.IsRecipeDelete ? "Workbench.Recipe.DeleteTitle" : deleting ? "Workbench.Menu.DeleteStep" : "Workbench.Pending.Title";
                string bodyKey = request.IsRecipeDelete ? "Workbench.Recipe.DeleteConfirmFormat" : deleting ? "Workbench.Step.DeleteConfirmFormat" : "Workbench.Pending.BodyFormat";
                return new OpenVisionRecipePendingEditDialogViewModel(request)
                {
                    TitleText = OpenVisionLanguageService.T(titleKey),
                    HeadingText = string.Format(CultureInfo.CurrentCulture,
                        OpenVisionLanguageService.T(bodyKey), request.IsStepDelete ? request.StepName : request.RecipeName),
                    DetailText = request.IsRecipeDelete ? OpenVisionLanguageService.T("Workbench.Recipe.DeleteDetail")
                        : deleting ? OpenVisionLanguageService.T("Workbench.Step.DeleteDetail")
                        : string.Format(CultureInfo.CurrentCulture, OpenVisionLanguageService.T("Workbench.Pending.CountFormat"), request.ChangeCount),
                    ApplyButtonText = OpenVisionLanguageService.T(deleting ? "Workbench.Menu.DeleteStep" : "Workbench.Pending.Save"),
                    DiscardButtonText = OpenVisionLanguageService.T("Workbench.Pending.DontSave"),
                    CancelButtonText = OpenVisionLanguageService.T("Workbench.Pending.Cancel")
                };
            }

            string transitionText = ResolveTransitionText(request.Kind);
            string target = string.IsNullOrWhiteSpace(request.TargetName)
                ? OpenVisionRecipeText.Local("현재 화면 닫기", "Close the current view")
                : request.TargetName.Trim();
            return new OpenVisionRecipePendingEditDialogViewModel(request)
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
