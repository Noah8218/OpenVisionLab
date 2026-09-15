using Microsoft.Win32;
using OpenVisionLab.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class RecipeDialogAdapter
    {
        private readonly Func<Window> ownerWindowProvider;
        private readonly Queue<OpenVisionRecipePendingEditDecision> pendingRecipeEditDecisionsForTest =
            new Queue<OpenVisionRecipePendingEditDecision>();

        internal RecipeDialogAdapter(Func<Window> ownerWindowProvider)
        {
            this.ownerWindowProvider = ownerWindowProvider ?? throw new ArgumentNullException(nameof(ownerWindowProvider));
        }

        internal Func<string, string, string, bool> QualifiedSnapshotLifecycleConfirmationForTest { get; set; }

        internal Func<string, bool> QualifiedSnapshotEvidenceOpenerForTest { get; set; }

        internal void QueuePendingRecipeEditDecisionForTest(OpenVisionRecipePendingEditDecision decision)
        {
            pendingRecipeEditDecisionsForTest.Enqueue(decision);
        }

        internal OpenVisionRecipePendingEditDecision DecidePendingEdit(
            OpenVisionRecipePendingEditRequest request)
        {
            if (pendingRecipeEditDecisionsForTest.Count > 0)
            {
                return pendingRecipeEditDecisionsForTest.Dequeue();
            }

            OpenVisionRecipePendingEditDialog dialog = new OpenVisionRecipePendingEditDialog(request)
            {
                Owner = ownerWindowProvider()
            };
            dialog.ShowDialog();
            return dialog.Decision;
        }

        internal bool ConfirmDeleteRecipe(string recipeName)
        {
            string message = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "레시피 '{0}'을(를) 삭제하시겠습니까?"
                    : "Delete recipe '{0}'?",
                recipeName);
            MessageBoxResult result = MessageBox.Show(
                ownerWindowProvider(),
                message,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "레시피 삭제" : "Delete recipe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        internal bool ConfirmQualifiedSnapshotLifecycle(
            string snapshotId,
            string action,
            string reason)
        {
            if (QualifiedSnapshotLifecycleConfirmationForTest != null)
            {
                return QualifiedSnapshotLifecycleConfirmationForTest(
                    snapshotId,
                    action,
                    reason);
            }

            string normalizedAction = string.Equals(
                action,
                "Revoked",
                StringComparison.Ordinal)
                ? "Revoke"
                : "Supersede";
            string message = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                OpenVisionLanguageService.CurrentLanguage
                    == OpenVisionLanguage.Korean
                    ? "불변 적격 Snapshot에 '{0}' 생명주기 기록을 추가합니다.\n\nSnapshot: {1}\n사유: {2}\n\n증거 payload는 보존되며 이 기록은 삭제하지 않습니다. 계속하시겠습니까?"
                    : "Append the '{0}' lifecycle record to this immutable qualified Snapshot.\n\nSnapshot: {1}\nReason: {2}\n\nThe evidence payload remains and this record is not deleted. Continue?",
                normalizedAction,
                snapshotId,
                reason);
            MessageBoxResult result = MessageBox.Show(
                ownerWindowProvider(),
                message,
                OpenVisionLanguageService.CurrentLanguage
                    == OpenVisionLanguage.Korean
                    ? "Qualified Snapshot 생명주기 확인"
                    : "Confirm qualified Snapshot lifecycle",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        internal bool OpenQualifiedSnapshotEvidence(string directory)
        {
            if (QualifiedSnapshotEvidenceOpenerForTest != null)
            {
                return QualifiedSnapshotEvidenceOpenerForTest(directory);
            }

            if (string.IsNullOrWhiteSpace(directory)
                || !Directory.Exists(directory))
            {
                return false;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = directory,
                    UseShellExecute = true
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool ConfirmDeletePipeline(string recipeName, string pipelineName)
        {
            string message = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "레시피 '{0}'에서 파이프라인 '{1}'을(를) 삭제하시겠습니까?"
                    : "Delete pipeline '{1}' from recipe '{0}'?",
                recipeName,
                pipelineName);
            MessageBoxResult result = MessageBox.Show(
                ownerWindowProvider(),
                message,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "파이프라인 삭제" : "Delete pipeline",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        internal string SelectImportPipelineXmlPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "파이프라인 XML 또는 검토 번들 열기"
                    : "Open pipeline XML or review bundle",
                Filter = "OpenVision XML / Review bundle (*.xml;*.review.zip;*.zip)|*.xml;*.review.zip;*.zip|OpenVision Pipeline XML (*.xml)|*.xml|Review bundle (*.review.zip;*.zip)|*.review.zip;*.zip|All files (*.*)|*.*",
                Multiselect = false
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true ? dialog.FileName : string.Empty;
        }

        internal string SelectLocatorEvidencePacketPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "Locator Evidence Packet 열기"
                    : "Open locator Evidence Packet",
                Filter = "Locator Evidence Packet (*.packet.json;*.json)|*.packet.json;*.json|All files (*.*)|*.*",
                Multiselect = false
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true ? dialog.FileName : string.Empty;
        }

        internal string SelectLocatorEvidenceReviewDecisionPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "Locator review decision 열기"
                    : "Open locator review decision",
                Filter = "Locator review decision (*.json)|*.json|All files (*.*)|*.*",
                Multiselect = false
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true ? dialog.FileName : string.Empty;
        }

        internal string SelectExportPipelineXmlPath(string suggestedFileName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "파이프라인 XML 내보내기"
                    : "Export pipeline XML",
                Filter = "OpenVision Pipeline XML (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "Pipeline.xml" : suggestedFileName,
                AddExtension = true,
                DefaultExt = ".xml",
                OverwritePrompt = true
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true ? dialog.FileName : string.Empty;
        }

        internal string SelectExportRecipeReviewBundlePath(string suggestedFileName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "레시피 검토 번들 내보내기"
                    : "Export recipe review bundle",
                Filter = "OpenVision review bundle (*.review.zip)|*.review.zip|Zip archive (*.zip)|*.zip",
                FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "Pipeline.review.zip" : suggestedFileName,
                AddExtension = true,
                DefaultExt = ".zip",
                OverwritePrompt = true
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true ? dialog.FileName : string.Empty;
        }

        internal IReadOnlyList<string> SelectValidationSetImagePaths(string expected)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "로컬 검증 세트에 " + expected + "개 이미지 추가"
                    : "Add " + expected + " images to local validation set",
                Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true
                ? dialog.FileNames
                : Array.Empty<string>();
        }

        internal string SelectValidationSetFolderPath(string expected)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "로컬 검증 세트에 " + expected + "개 이미지 폴더 추가"
                    : "Add " + expected + " folder to local validation set",
                Multiselect = false
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true
                ? dialog.FolderName
                : string.Empty;
        }

        internal string SelectValidationSetReplacementImagePath(string missingPath)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "누락된 검증 이미지 교체: " + Path.GetFileName(missingPath)
                    : "Replace missing validation image: " + Path.GetFileName(missingPath),
                Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true
            };

            return dialog.ShowDialog(ownerWindowProvider()) == true
                ? dialog.FileName
                : string.Empty;
        }

        internal bool ConfirmDeleteValidationSet(string setName)
        {
            MessageBoxResult result = MessageBox.Show(
                ownerWindowProvider(),
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "로컬 검증 세트 '" + setName + "'을(를) 삭제하시겠습니까? 원본 이미지는 삭제되지 않습니다."
                    : "Delete local validation set '" + setName + "'? Source images will not be deleted.",
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "검증 세트 삭제"
                    : "Delete validation set",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

    }
}
