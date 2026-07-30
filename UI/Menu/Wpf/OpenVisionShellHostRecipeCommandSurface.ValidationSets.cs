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
                OpenVisionRecipeCatalogPairValidationSetService.Import(
                    validationSetDocument,
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
            return validationSetStorageReady
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

            OpenVisionRecipeValidationSetImage contract = new OpenVisionRecipeValidationSetImage
            {
                VariantId = variantId,
                ExpectedMetricName = metricName,
                ExpectedMetricMinimum = metricMinimum,
                ExpectedMetricMaximum = metricMaximum
            };
            if (!OpenVisionRecipeValidationSetStorage.TryValidateVariantContract(contract, out string contractError))
            {
                ValidationSuiteStatusText = LocalText("Variant 계약 ERROR: ", "Variant contract ERROR: ")
                    + contractError;
                return false;
            }

            int added = OpenVisionRecipeValidationSetStorage.AddOrUpdateImages(
                option.Set,
                paths,
                expected,
                notes,
                variantId,
                metricName,
                metricMinimum,
                metricMaximum,
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

            if (!OpenVisionRecipeValidationSetStorage.TryApplyVariantContract(
                    option.Set,
                    row.Image,
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
            return validationSetStorageReady
                && !executionSession.IsValidationSuiteRunning
                && SelectedValidationSetOption?.Set != null
                && !SelectedValidationSetOption.Set.IsIdentityLocked
                && SelectedValidationSetImageRow?.Image != null;
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
