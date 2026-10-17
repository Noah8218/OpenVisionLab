using OpenVisionLab.Mvvm;
using System;
using System.Windows.Input;

namespace OpenVisionLab
{
    internal sealed class InlineImageEditViewModel : ObservableObject
    {
        private string status = string.Empty;
        private readonly Func<int> regionCount;

        internal InlineImageEditViewModel(bool multi, bool template, Func<int> regionCount)
        {
            IsMulti = multi; IsTemplate = template; this.regionCount = regionCount;
            ApplyCommand = new RelayCommand(() => Requested?.Invoke("apply"));
            CancelCommand = new RelayCommand(() => Requested?.Invoke("cancel"));
            WholeCommand = new RelayCommand(() => Requested?.Invoke("whole"));
            AddCommand = new RelayCommand(() => Requested?.Invoke("add"));
            DeleteCommand = new RelayCommand(() => Requested?.Invoke("delete"));
            LargeCommand = new RelayCommand(() => Requested?.Invoke("large"));
        }

        internal event Action<string> Requested;
        public bool IsMulti { get; }
        public bool IsTemplate { get; }
        public string Title => IsTemplate ? Text("템플릿 영역", "Template region") : IsMulti ? string.Format(Text("영역 {0}개", "Regions: {0}"), regionCount()) : "ROI";
        internal void RefreshTitle() => OnPropertyChanged(nameof(Title));
        public string Notice => Text("드래그하거나 좌표를 수정하세요. Enter 적용 · Esc 취소", "Drag or edit coordinates. Enter applies · Esc cancels");
        public string ApplyText => IsTemplate ? Text("템플릿 등록", "Register template") : Text("적용", "Apply");
        public string CancelText => Text("취소", "Cancel");
        public string WholeText => Text("전체 영역", "Whole image");
        public string AddText => Text("추가", "Add");
        public string DeleteText => Text("선택 삭제", "Delete selected");
        public string LargeText => Text("크게 편집", "Edit larger");
        public string Status { get => status; internal set => SetProperty(ref status, value); }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand WholeCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LargeCommand { get; }

        private static string Text(string ko, string en) => OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English ? en : ko;
    }
}
