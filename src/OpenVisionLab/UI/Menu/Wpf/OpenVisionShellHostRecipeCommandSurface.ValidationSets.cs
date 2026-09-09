using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostRecipeCommandSurface
    {
        private void CreateValidationSetFromSelectedPair()
        {
            CreateValidationSetFromSelectedPairCore();
        }

        internal bool CreateValidationSetFromSelectedPairForTest()
        {
            return CreateValidationSetFromSelectedPairCore();
        }

        private bool CreateValidationSetFromSelectedPairCore()
        {
            if (!CanCreateValidationSetFromSelectedPair())
            {
                return false;
            }

            OpenVisionRecipeCatalogPairValidationSetImportResult result =
                validationSetDocumentOwner.ImportCatalogPair(
                    SelectedSampleOption.Sample,
                    SampleOptions.Select(option => option?.Sample),
                    SelectedPipelineOption?.PipelineName);
            if (!result.Success)
            {
                ValidationSuiteStatusText = LocalText(
                    "카탈로그 쌍 가져오기 ERROR: ",
                    "Catalog pair import ERROR: ")
                    + result.Error;
                return false;
            }

            if (!TrySaveValidationSetDocument(LocalText(
                    "카탈로그 쌍을 검증 세트로 저장",
                    "Save catalog pair as validation set")))
            {
                return false;
            }

            SelectedValidationSuiteScopeOption = ValidationSuiteScopeOptions.FirstOrDefault(option =>
                string.Equals(
                    option.Key,
                    OpenVisionRecipeValidationSuiteScopeOption.LocalValidationSetKey,
                    StringComparison.OrdinalIgnoreCase));
            RefreshValidationSetOptions(result.SetName);
            NewValidationSetName = CreateUniqueValidationSetName();
            ValidationSuiteStatusText = string.Format(
                CultureInfo.CurrentCulture,
                result.Updated
                    ? LocalText(
                        "카탈로그 쌍 검증 세트를 갱신했습니다: {0} | OK {1} / NG {2} | 실행 안 함",
                        "Updated catalog pair validation set: {0} | OK {1} / NG {2} | not run")
                    : LocalText(
                        "카탈로그 쌍 검증 세트를 만들었습니다: {0} | OK {1} / NG {2} | 실행 안 함",
                        "Created catalog pair validation set: {0} | OK {1} / NG {2} | not run"),
                result.SetName,
                result.OkCount,
                result.NgCount);
            StatusText = ValidationSuiteStatusText;
            return true;
        }

        private bool CanCreateValidationSetFromSelectedPair()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && !executionSession.IsSampleCheckRunning
                && !executionSession.IsPairCheckRunning
                && !executionSession.IsCatalogBenchmarkRunning
                && OpenVisionRecipeCatalogPairValidationSetService.CanImport(
                    SelectedSampleOption?.Sample,
                    SampleOptions.Select(option => option?.Sample));
        }

        private void CreateValidationSet()
        {
            if (!CanCreateValidationSet())
            {
                return;
            }

            string name = NewValidationSetName.Trim();
            if (!validationSetDocumentOwner.TryCreateSet(name))
            {
                return;
            }

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
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && OpenVisionRecipeValidationSetStorage.IsValidSetName(name)
                && !validationSetDocumentOwner.ContainsSet(name);
        }

        private void DeleteValidationSet()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanDeleteValidationSet() || option == null || !confirmDeleteValidationSet(option.Name))
            {
                return;
            }

            if (!validationSetDocumentOwner.TryDeleteSet(option.Name))
            {
                return;
            }

            if (!TrySaveValidationSetDocument(LocalText("검증 세트 삭제", "Delete validation set")))
            {
                return;
            }

            RefreshValidationSetOptions();
            ValidationSuiteStatusText = LocalText("로컬 검증 세트를 삭제했습니다: ", "Deleted local validation set: ") + option.Name;
        }

        private bool CanDeleteValidationSet()
        {
            return validationSetDocumentOwner.StorageReady
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
                AddValidationSetImages(
                    expected,
                    paths,
                    ValidationSetPendingNotes,
                    ValidationSetPendingVariantId,
                    ValidationSetPendingMetricName,
                    ValidationSetPendingMetricMinimum,
                    ValidationSetPendingMetricMaximum);
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
                    AddValidationSetFolder(
                        expected,
                        folderPath,
                        ValidationSetPendingNotes,
                        ValidationSetPendingVariantId,
                        ValidationSetPendingMetricName,
                        ValidationSetPendingMetricMinimum,
                        ValidationSetPendingMetricMaximum);
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
            return AddValidationSetFolder(expected, folderPath, notes, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        internal bool AddValidationSetFolderForTest(
            string expected,
            string folderPath,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            return AddValidationSetFolder(
                expected,
                folderPath,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        private bool AddValidationSetFolder(
            string expected,
            string folderPath,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
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

            return AddValidationSetImages(
                expected,
                paths,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        internal bool AddValidationSetImagesForTest(string expected, IEnumerable<string> paths, string notes = "")
        {
            return AddValidationSetImages(expected, paths, notes, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        internal bool AddValidationSetImagesForTest(
            string expected,
            IEnumerable<string> paths,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            return AddValidationSetImages(
                expected,
                paths,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum);
        }

        private bool AddValidationSetImages(
            string expected,
            IEnumerable<string> paths,
            string notes,
            string variantId,
            string metricName,
            string metricMinimum,
            string metricMaximum)
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            if (!CanAddValidationSetImages() || option?.Set == null)
            {
                return false;
            }

            if (!validationSetDocumentOwner.TryAddImages(
                    option.Name,
                    paths,
                    expected,
                    notes,
                    variantId,
                    metricName,
                    metricMinimum,
                    metricMaximum,
                    out int added,
                    out int updated,
                    out int skipped,
                    out string contractError))
            {
                if (!string.IsNullOrWhiteSpace(contractError))
                {
                    ValidationSuiteStatusText = LocalText("Variant 계약 ERROR: ", "Variant contract ERROR: ")
                        + contractError;
                    return false;
                }

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

        private void LoadSelectedValidationVariantContract()
        {
            OpenVisionRecipeValidationSetImage image = SelectedValidationSetImageRow?.Image;
            validationSetPendingVariantId = image?.VariantId ?? string.Empty;
            validationSetPendingMetricName = image?.ExpectedMetricName ?? string.Empty;
            validationSetPendingMetricMinimum = image?.ExpectedMetricMinimum ?? string.Empty;
            validationSetPendingMetricMaximum = image?.ExpectedMetricMaximum ?? string.Empty;
            OnPropertyChanged(nameof(ValidationSetPendingVariantId));
            OnPropertyChanged(nameof(ValidationSetPendingMetricName));
            OnPropertyChanged(nameof(ValidationSetPendingMetricMinimum));
            OnPropertyChanged(nameof(ValidationSetPendingMetricMaximum));
        }

        private void ApplyValidationSetVariantContract()
        {
            OpenVisionRecipeValidationSetOption option = SelectedValidationSetOption;
            OpenVisionRecipeValidationSetImageRow row = SelectedValidationSetImageRow;
            if (!CanApplyValidationSetVariantContract()
                || option?.Set == null
                || row?.Image == null)
            {
                return;
            }

            if (!validationSetDocumentOwner.TryApplyVariantContract(
                    option.Name,
                    row.Path,
                    ValidationSetPendingVariantId,
                    ValidationSetPendingMetricName,
                    ValidationSetPendingMetricMinimum,
                    ValidationSetPendingMetricMaximum,
                    out string error))
            {
                ValidationSuiteStatusText = LocalText("Variant 계약 ERROR: ", "Variant contract ERROR: ") + error;
                return;
            }

            string setName = option.Name;
            string imagePath = row.Path;
            if (!TrySaveValidationSetDocument(LocalText("Validation Variant 적용", "Apply validation Variant")))
            {
                return;
            }

            RefreshValidationSetOptions(setName);
            SelectedValidationSetImageRow = ValidationSetImageRows.FirstOrDefault(item =>
                string.Equals(item.Path, imagePath, StringComparison.OrdinalIgnoreCase));
            ValidationSuiteStatusText = LocalText(
                "선택 이미지의 Variant 계약을 저장했습니다. Preview/Run은 실행되지 않았습니다.",
                "Saved the selected image Variant contract. Preview/Run was not executed.");
        }

        private void ResetValidationSetVariantContract()
        {
            ValidationSetPendingVariantId = string.Empty;
            ValidationSetPendingMetricName = string.Empty;
            ValidationSetPendingMetricMinimum = string.Empty;
            ValidationSetPendingMetricMaximum = string.Empty;
            ApplyValidationSetVariantContract();
        }

        private bool CanApplyValidationSetVariantContract()
        {
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool CanAddValidationSetImages()
        {
            return validationSetDocumentOwner.StorageReady
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
            if (!validationSetDocumentOwner.TryRepairMissingImagePath(
                    option.Name,
                    row.Path,
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
            return validationSetDocumentOwner.StorageReady
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

            if (!validationSetDocumentOwner.TryRemoveImage(option.Name, row.Path))
            {
                return;
            }

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
            return validationSetDocumentOwner.StorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
        }

        private bool TrySaveValidationSetDocument(string operation)
        {
            if (validationSetDocumentOwner.TrySave(
                NormalizeRecipeName(selectedRecipeName),
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
            string previousImagePath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            bool storageReady = validationSetDocumentOwner.TryLoad(recipeName, out string error);

            if (!storageReady)
            {
                validationSetSelectionOwner.Clear();
                OnPropertyChanged(nameof(ValidationSetOptions));
                OnPropertyChanged(nameof(SelectedValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
                OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
                OnPropertyChanged(nameof(ValidationSetImageRows));
                OnPropertyChanged(nameof(SelectedValidationSetImageRow));
                LoadSelectedValidationVariantContract();
                ValidationSuiteStatusText = LocalText("로컬 검증 세트 로드 ERROR: ", "Local validation set load ERROR: ") + error;
                RefreshPinArrayGapValidationIdentityState();
                NotifyValidationSetEvidenceChanged();
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

            validationSetSelectionOwner.Refresh(
                previousName,
                previousTrainName,
                previousValidationName,
                previousTestName,
                previousImagePath);
            OnPropertyChanged(nameof(ValidationSetOptions));
            OnPropertyChanged(nameof(SelectedValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapTrainValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapValidationValidationSetOption));
            OnPropertyChanged(nameof(PinArrayGapTestValidationSetOption));
            OnPropertyChanged(nameof(ValidationSetImageRows));
            OnPropertyChanged(nameof(SelectedValidationSetImageRow));
            LoadSelectedValidationVariantContract();
            RefreshPinArrayGapValidationIdentityState();
            NotifyValidationSetEvidenceChanged();

            OnPropertyChanged(nameof(ValidationSetSelectionSummaryText));
            OnPropertyChanged(nameof(ValidationSuiteSummaryText));
            RefreshCommandState();
        }

        private void RefreshValidationSetImageRows()
        {
            string previousPath = SelectedValidationSetImageRow?.Path ?? string.Empty;
            validationSetSelectionOwner.RefreshImageRows(previousPath);
            OnPropertyChanged(nameof(ValidationSetImageRows));
            OnPropertyChanged(nameof(SelectedValidationSetImageRow));
            LoadSelectedValidationVariantContract();
            NotifyValidationSetEvidenceChanged();
        }

        private string CreateUniqueValidationSetName()
        {
            return validationSetDocumentOwner.CreateUniqueSetName();
        }
    }
}
