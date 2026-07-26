using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void CreateValidationSet()
        {
            if (!CanCreateValidationSet())
            {
                return;
            }

            string name = NewValidationSetName.Trim();
            validationSetDocument.Sets.Add(new OpenVisionRecipeValidationSet { Name = name });
            if (!TrySaveValidationSetDocument(LocalText("검증 세트 만들기", "Create validation set")))
            {
                return;
            }

            RefreshValidationSetOptions(name);
            NewValidationSetName = CreateUniqueValidationSetName();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 만들었습니다: ", "Created local validation set: ") + name;
        }

        private bool CanCreateValidationSet()
        {
            string name = NewValidationSetName?.Trim() ?? string.Empty;
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && OpenVisionRecipeValidationSetStorage.IsValidSetName(name)
                && !validationSetDocument.Sets.Any(set =>
                    string.Equals(set?.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private void DeleteValidationSet()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanDeleteValidationSet() || option == null || !confirmDeleteValidationSet(option.Name))
            {
                return;
            }

            validationSetDocument.Sets.RemoveAll(set =>
                string.Equals(set?.Name, option.Name, StringComparison.OrdinalIgnoreCase));
            if (!TrySaveValidationSetDocument(LocalText("검증 세트 삭제", "Delete validation set")))
            {
                return;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 삭제했습니다: ", "Deleted local validation set: ") + option.Name;
        }

        private bool CanDeleteValidationSet()
        {
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption != null;
        }

        private void AddValidationSetImages(string expected)
        {
            if (!CanAddValidationSetImages())
            {
                return;
            }

            try
            {
                IReadOnlyList<string> paths = selectValidationSetImagePaths(expected) ?? Array.Empty<string>();
                AddValidationSetImages(expected, paths, ValidationSetPendingNotes);
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("이미지 선택 ERROR: ", "Image selection ERROR: ") + ex.GetBaseException().Message;
            }
        }

        private void AddValidationSetFolder(string expected)
        {
            if (!CanAddValidationSetImages())
            {
                return;
            }

            try
            {
                string folderPath = selectValidationSetFolderPath(expected) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    AddValidationSetFolder(expected, folderPath, ValidationSetPendingNotes);
                }
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("폴더 선택 ERROR: ", "Folder selection ERROR: ")
                    + ex.GetBaseException().Message;
            }
        }

        internal bool AddValidationSetFolderForTest(string expected, string folderPath, string notes = "")
        {
            return AddValidationSetFolder(expected, folderPath, notes);
        }

        private bool AddValidationSetFolder(string expected, string folderPath, string notes)
        {
            if (!CanAddValidationSetImages())
            {
                return false;
            }

            if (!OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths(
                    folderPath,
                    out IReadOnlyList<string> paths,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("폴더 이미지 등록 ERROR: ", "Folder image registration ERROR: ") + error;
                return false;
            }

            if (paths.Count == 0)
            {
                ValidationSuiteStatusText = LocalText(
                    "선택한 폴더의 바로 아래에서 지원 이미지 파일을 찾지 못했습니다.",
                    "No supported images were found directly in the selected folder.");
                return false;
            }

            return AddValidationSetImages(expected, paths, notes);
        }

        internal bool AddValidationSetImagesForTest(string expected, IEnumerable<string> paths, string notes = "")
        {
            return AddValidationSetImages(expected, paths, notes);
        }

        private bool AddValidationSetImages(string expected, IEnumerable<string> paths, string notes)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanAddValidationSetImages() || option?.Set == null)
            {
                return false;
            }

            int added = OpenVisionRecipeValidationSetStorage.AddOrUpdateImages(
                option.Set,
                paths,
                expected,
                notes,
                out int updated,
                out int skipped);
            if (added == 0 && updated == 0)
            {
                if (skipped > 0)
                {
                    ValidationSuiteStatusText = LocalText("지원되는 기존 이미지가 선택되지 않았습니다.", "No supported existing images were selected.");
                }

                return false;
            }

            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 추가", "Add validation images")))
            {
                return false;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("{0} 이미지: 추가 {1}, 갱신 {2}, 건너뜀 {3}", "{0} images: added {1}, updated {2}, skipped {3}"),
                expected,
                added,
                updated,
                skipped);
            return true;
        }

        private bool CanAddValidationSetImages()
        {
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked;
        }

        private void RepairValidationSetImagePath()
        {
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRepairValidationSetImagePath() || row == null)
            {
                return;
            }

            try
            {
                string replacementPath = selectValidationSetReplacementImagePath(row.Path) ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(replacementPath))
                {
                    RepairValidationSetImagePath(replacementPath);
                }
            }
            catch (Exception ex)
            {
                ValidationSuiteStatusText = LocalText("경로 복구 ERROR: ", "Path repair ERROR: ")
                    + ex.GetBaseException().Message;
            }
        }

        internal bool RepairValidationSetImagePathForTest(string replacementPath)
        {
            return RepairValidationSetImagePath(replacementPath);
        }

        private bool RepairValidationSetImagePath(string replacementPath)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRepairValidationSetImagePath() || option?.Set == null || row?.Image == null)
            {
                return false;
            }

            string missingFileName = row.FileName;
            if (!OpenVisionRecipeValidationSetStorage.TryRepairMissingImagePath(
                    option.Set,
                    row.Image,
                    replacementPath,
                    out string repairedPath,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("경로 복구 ERROR: ", "Path repair ERROR: ") + error;
                return false;
            }

            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 경로 복구", "Repair validation image path")))
            {
                return false;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                LocalText("누락 이미지 경로를 복구했습니다: {0} -> {1}", "Repaired missing image path: {0} -> {1}"),
                missingFileName,
                Path.GetFileName(repairedPath));
            return true;
        }

        private bool CanRepairValidationSetImagePath()
        {
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null
                && SelectedValidationSetImageRow.IsMissing;
        }

        private void RemoveValidationSetImage()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanRemoveValidationSetImage() || option?.Set == null || row?.Image == null)
            {
                return;
            }

            option.Set.Images.RemoveAll(image => ReferenceEquals(image, row.Image)
                || string.Equals(image?.Path, row.Path, StringComparison.OrdinalIgnoreCase));
            string setName = option.Name;
            if (!TrySaveValidationSetDocument(LocalText("검증 이미지 제거", "Remove validation image")))
            {
                return;
            }

            RefreshValidationSetOptions(setName);
            ValidationSuiteStatusText = LocalText("검증 세트에서 이미지를 제거했습니다: ", "Removed image from validation set: ") + row.FileName;
        }

        private bool CanRemoveValidationSetImage()
        {
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool TrySaveValidationSetDocument(string operation)
        {
            if (OpenVisionRecipeValidationSetStorage.TrySave(
                NormalizeRecipeName(selectedRecipeName),
                validationSetDocument,
                out string error))
            {
                return true;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = operation + " ERROR: " + error;
            return false;
        }

        private void RefreshValidationSetOptions(string preferredSetName = null)
        {
            string recipeName = NormalizeRecipeName(selectedRecipeName);
            string previousName = preferredSetName
                ?? SelectedValidationSetOption?.Name
                ?? string.Empty;
            string previousTrainName = PinArrayGapTrainValidationSetOption?.Name ?? string.Empty;
            string previousValidationName = PinArrayGapValidationValidationSetOption?.Name ?? string.Empty;
            string previousTestName = PinArrayGapTestValidationSetOption?.Name ?? string.Empty;
            validationSetStorageReady = OpenVisionRecipeValidationSetStorage.TryLoad(
                recipeName,
                out validationSetDocument,
                out string error);

            if (!validationSetStorageReady)
            {
                ValidationSetOptions = Array.Empty<OpenVisionRecipeValidationSetOption>();
                SelectedValidationSetOption = null;
                PinArrayGapTrainValidationSetOption = null;
                PinArrayGapValidationValidationSetOption = null;
                PinArrayGapTestValidationSetOption = null;
                RefreshValidationSetImageRows();
                ValidationSuiteStatusText = LocalText("로컬 검증 세트 로드 ERROR: ", "Local validation set load ERROR: ") + error;
                RefreshCommandState();
                return;
            }

            if (string.IsNullOrWhiteSpace(previousTrainName)
                && string.IsNullOrWhiteSpace(previousValidationName)
                && string.IsNullOrWhiteSpace(previousTestName)
                && OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad(
                    recipeName,
                    out OpenVisionRecipePinArrayGapValidationRecord frozenRecord,
                    out _))
            {
                previousTrainName = frozenRecord.Train?.SetName ?? string.Empty;
                previousValidationName = frozenRecord.Validation?.SetName ?? string.Empty;
                previousTestName = frozenRecord.Test?.SetName ?? string.Empty;
            }

            OpenVisionRecipeValidationSetOptionSelection selection = OpenVisionRecipeValidationSetPresenter.BuildOptionSelection(
                validationSetDocument,
                previousName,
                previousTrainName,
                previousValidationName,
                previousTestName);
            ValidationSetOptions = selection.Options;
            SelectedValidationSetOption = selection.Selected;
            PinArrayGapTrainValidationSetOption = selection.Train;
            PinArrayGapValidationValidationSetOption = selection.Validation;
            PinArrayGapTestValidationSetOption = selection.Test;
            if (selection.Selected == null)
            {
                RefreshValidationSetImageRows();
            }

            OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
            OnPropertyChanged(nameof(ValidationSuiteSummaryText));
            RefreshCommandState();
        }

        private void RefreshValidationSetImageRows()
        {
            string previousPath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            OpenVisionRecipeValidationSetImageSelection selection = OpenVisionRecipeValidationSetPresenter.BuildImageSelection(
                SelectedValidationSetOption,
                previousPath);
            ValidationSetImageRows = selection.Rows;
            SelectedValidationSetImageRow = selection.Selected;
            NotifyValidationSetEvidenceChanged();
        }

        private string CreateUniqueValidationSetName()
        {
            const string baseName = "Local_Validation_Set";
            HashSet<string> names = validationSetDocument.Sets
                .Where(set => set != null && !string.IsNullOrWhiteSpace(set.Name))
                .Select(set => set.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!names.Contains(baseName))
            {
                return baseName;
            }

            int suffix = 2;
            while (names.Contains(baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture)))
            {
                suffix++;
            }

            return baseName + "_" + suffix.ToString(CultureInfo.InvariantCulture);
        }
    }
}
