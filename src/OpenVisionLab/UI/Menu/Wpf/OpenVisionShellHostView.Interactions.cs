using Lib.OpenCV.Pipeline;
using OpenVisionLab.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static OpenVisionLab.DEFINE;
using DrawingBitmap = System.Drawing.Bitmap;

namespace OpenVisionLab
{
    public sealed partial class OpenVisionShellHostView
    {
        private void AttachRecipeStepPropertyGridHost()
        {
            if (recipeStepPropertyGridHost == null || RecipeCommands == null)
            {
                return;
            }

            recipeStepPropertyGridHostController = VisionToolPropertyGridHost.Attach(
                recipeStepPropertyGridHost,
                RecipeCommands.SelectedStepEditObject,
                (_, __) => RecipeCommands?.MarkSelectedStepEditDirty());
            recipeStepPropertyGridHostController.SetCompactDensity(true);
            recipeStepPropertyGridHostController.SetThemeVariant(
                System.Windows.Controls.WpfPropertyGrid.PropertyGridThemeVariant.Dark);
            lifecycle.Track(
                () => RecipeCommands.PropertyChanged += OnRecipeCommandsPropertyChanged,
                () => RecipeCommands.PropertyChanged -= OnRecipeCommandsPropertyChanged);
            lifecycle.Track(
                () => { },
                () =>
                {
                    recipeStepPropertyGridHostController?.Dispose();
                    recipeStepPropertyGridHostController = null;
                });
        }

        private void OnRecipeCommandsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null || e.PropertyName == nameof(OpenVisionShellHostRecipeCommandSurface.SelectedStepEditObject))
            {
                recipeStepPropertyGridHostController?.SelectObject(RecipeCommands?.SelectedStepEditObject);
            }
        }

        private bool CommitPendingRecipeStepEdit()
        {
            if (failNextRecipeStepEditCommitForTest)
            {
                failNextRecipeStepEditCommitForTest = false;
                return false;
            }

            return recipeStepPropertyGridHostController?.CommitPendingEdit() ?? true;
        }

        private OpenVisionRecipeRoundTripValidationResult ValidateRecipeStepRoundTrip(
            string recipeName,
            Lib.OpenCV.Pipeline.VisionPipeline pipeline)
        {
            if (failNextRecipeStepRoundTripValidationForTest)
            {
                failNextRecipeStepRoundTripValidationForTest = false;
                return new OpenVisionRecipeRoundTripValidationResult
                {
                    Succeeded = false,
                    Message = "Forced round-trip validation failure for current-build smoke."
                };
            }

            bool succeeded = VisionPipelineStorage.TryValidateRoundTrip(
                recipeName,
                pipeline,
                out string message);
            return new OpenVisionRecipeRoundTripValidationResult
            {
                Succeeded = succeeded,
                Message = message
            };
        }

        private void SaveRecipeStepPipeline(
            string recipeName,
            Lib.OpenCV.Pipeline.VisionPipeline pipeline)
        {
            if (failNextRecipeStepSaveForTest)
            {
                failNextRecipeStepSaveForTest = false;
                throw new InvalidOperationException(
                    "Forced XML save failure for current-build smoke.");
            }

            VisionPipelineStorage.Save(recipeName, pipeline);
        }

        private void HandleRecipeManagerTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (recipeManagerPanel?.Visibility != Visibility.Visible || rootShellHost == null || recipeManagerPanelTransform == null)
            {
                return;
            }

            isRecipeManagerPanelDragging = true;
            recipeManagerPanelDragStartPoint = e.GetPosition(rootShellHost);
            recipeManagerPanelDragStartX = recipeManagerPanelTransform.X;
            recipeManagerPanelDragStartY = recipeManagerPanelTransform.Y;
            Mouse.Capture(recipeManagerTitleBar);
            e.Handled = true;
        }

        private void HandleRecipeManagerTitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (!isRecipeManagerPanelDragging || e.LeftButton != MouseButtonState.Pressed || rootShellHost == null)
            {
                return;
            }

            Point current = e.GetPosition(rootShellHost);
            SetRecipeManagerPanelOffset(
                recipeManagerPanelDragStartX + current.X - recipeManagerPanelDragStartPoint.X,
                recipeManagerPanelDragStartY + current.Y - recipeManagerPanelDragStartPoint.Y);
            e.Handled = true;
        }

        private void HandleRecipeManagerTitleBarMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopRecipeManagerPanelDrag();
            e.Handled = true;
        }

        private void HandleRecipeManagerTitleBarLostMouseCapture(object sender, MouseEventArgs e)
        {
            isRecipeManagerPanelDragging = false;
        }

        private void HandleRecipeManagerCloseClick(object sender, RoutedEventArgs e)
        {
            btnHostRecipeManager.IsChecked = false;
            e.Handled = true;
        }

        private void HandleRecipeManagerOpenUnchecked(object sender, RoutedEventArgs e)
        {
            if (RecipeCommands?.TryCloseRecipeManager() != false)
            {
                return;
            }

            isRestoringRecipeManagerAfterCanceledClose = true;
            try
            {
                btnHostRecipeManager.IsChecked = true;
            }
            finally
            {
                isRestoringRecipeManagerAfterCanceledClose = false;
            }

            e.Handled = true;
        }

        private void HandleRecipeManagerOpenChecked(object sender, RoutedEventArgs e)
        {
            if (isRestoringRecipeManagerAfterCanceledClose)
            {
                return;
            }

            RecipeCommands?.RefreshOptions();

            if (recipeAdvancedReviewToggle != null)
            {
                recipeAdvancedReviewToggle.IsChecked = false;
            }

            if (tabRecipeOverview != null)
            {
                tabRecipeOverview.IsSelected = true;
            }
        }

        private void HandleRecipeAdvancedReviewChecked(object sender, RoutedEventArgs e)
        {
            if (tabRecipePipeline != null)
            {
                tabRecipePipeline.IsSelected = true;
            }
        }

        private void HandleRecipeAdvancedReviewUnchecked(object sender, RoutedEventArgs e)
        {
            if (tabRecipeOverview != null)
            {
                tabRecipeOverview.IsSelected = true;
            }
        }

        private void OpenGuidedSetupForTool(VISION_MENU menu)
        {
            if (!RecipeCommands.SelectGuidedSetupForTool(menu))
            {
                return;
            }

            OpenRecipeGuidedSetup();
        }

        private void HandleOpenRecipeGuidedSetup(object sender, RoutedEventArgs e)
        {
            OpenRecipeGuidedSetup();
        }

        private void OpenRecipeGuidedSetup()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeGuidedSetup.IsSelected = true;
            recipeGuidedSetupScrollViewer.ScrollToTop();
        }

        private void HandleOpenRecipeImageListValidation(object sender, RoutedEventArgs e)
        {
            OpenRecipeImageListValidation();
        }

        private void OpenRecipeImageListValidation()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipePipeline.IsSelected = true;
            tabRecipePipelineRunHistory.IsSelected = true;
            RecipeCommands?.SelectLocalValidationSetScope();
        }

        private void OpenRecipeLlmXmlReview()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeLlmXml.IsSelected = true;
        }

        private void HandleOpenRecipeLlmBrowserAssist(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
        }

        private void OpenRecipeLlmBrowserAssist()
        {
            btnHostRecipeManager.IsChecked = true;
            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipeLlmBrowserAssist.IsSelected = true;
        }

        private async void HandleOpenRecipeLlmBrowserAssistChatGpt(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
            recipeLlmBrowserAssistPlaceholder.Visibility = Visibility.Collapsed;
            recipeLlmBrowserAssistWebView.Visibility = Visibility.Visible;

            OpenVisionRecipeLlmBrowserAssistOpenResult result =
                await llmBrowserAssistController.OpenChatGptAsync(recipeLlmBrowserAssistWebView);
            RecipeCommands?.SetLlmBrowserAssistStatus(result);

            if (result != OpenVisionRecipeLlmBrowserAssistOpenResult.EmbeddedChatGptOpened)
            {
                recipeLlmBrowserAssistWebView.Visibility = Visibility.Collapsed;
                recipeLlmBrowserAssistPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void HandleOpenRecipeLlmBrowserAssistExternal(object sender, RoutedEventArgs e)
        {
            OpenRecipeLlmBrowserAssist();
            RecipeCommands?.SetLlmBrowserAssistStatus(llmBrowserAssistController.OpenChatGptInExternalBrowser());
        }

        private void OpenRecipePipelineReview()
        {
            if (btnHostRecipeManager.IsChecked == true
                && RecipeCommands?.TryCloseRecipeManager() == false)
            {
                return;
            }

            btnHostRecipeManager.IsChecked = false;
            SelectToolMenu(VISION_MENU.Pipeline);
        }

        private OpenVisionRecipePendingEditDecision DecidePendingRecipeEdit(
            OpenVisionRecipePendingEditRequest request)
        {
            if (pendingRecipeEditDecisionsForTest.Count > 0)
            {
                return pendingRecipeEditDecisionsForTest.Dequeue();
            }

            OpenVisionRecipePendingEditDialog dialog = new OpenVisionRecipePendingEditDialog(request)
            {
                Owner = Window.GetWindow(this)
            };
            dialog.ShowDialog();
            return dialog.Decision;
        }

        private void ReturnToRecipeManagerFromPipelineReview()
        {
            toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
            btnHostRecipeManager.IsChecked = true;
        }

        private void OpenPipelineStepEditorFromPipelineReview(string recipeName, string pipelineName, int stepNumber)
        {
            toolWindowLifecycleController.CloseActiveWpfToolWindowByUser();
            btnHostRecipeManager.IsChecked = true;

            if (RecipeCommands?.FocusPipelineStepForEdit(recipeName, pipelineName, stepNumber) != true)
            {
                return;
            }

            recipeAdvancedReviewToggle.IsChecked = true;
            tabRecipePipeline.IsSelected = true;
            tabRecipePipelineXmlSteps.IsSelected = true;

            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new Action(() =>
                {
                    recipePipelineTabScrollViewer.UpdateLayout();
                    recipePipelineTabScrollViewer.ScrollToEnd();
                    Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() =>
                        {
                            recipePipelineTabScrollViewer.UpdateLayout();
                            recipePipelineTabScrollViewer.ScrollToEnd();
                        }));
                }));
        }

        private void OpenLearnForPipelineReviewTool(string toolType)
        {
            commandController?.OpenLearnForToolType(toolType);
        }

        private void StopRecipeManagerPanelDrag()
        {
            isRecipeManagerPanelDragging = false;
            if (Mouse.Captured == recipeManagerTitleBar)
            {
                Mouse.Capture(null);
            }
        }

        private void SetRecipeManagerPanelOffset(double x, double y)
        {
            if (recipeManagerPanelTransform == null)
            {
                return;
            }

            if (rootShellHost == null || recipeManagerPanel == null || rootShellHost.ActualWidth <= 0D || rootShellHost.ActualHeight <= 0D)
            {
                recipeManagerPanelTransform.X = x;
                recipeManagerPanelTransform.Y = y;
                return;
            }

            double currentLeft = recipeManagerPanel.TranslatePoint(new Point(0D, 0D), rootShellHost).X;
            double currentTop = recipeManagerPanel.TranslatePoint(new Point(0D, 0D), rootShellHost).Y;
            double baseLeft = currentLeft - recipeManagerPanelTransform.X;
            double baseTop = currentTop - recipeManagerPanelTransform.Y;
            const double minimumVisibleWidth = 260D;
            const double minimumVisibleHeight = 92D;
            double minX = 8D - baseLeft;
            double maxX = Math.Max(minX, rootShellHost.ActualWidth - minimumVisibleWidth - baseLeft);
            double minY = 8D - baseTop;
            double maxY = Math.Max(minY, rootShellHost.ActualHeight - minimumVisibleHeight - baseTop);

            recipeManagerPanelTransform.X = Math.Min(Math.Max(x, minX), maxX);
            recipeManagerPanelTransform.Y = Math.Min(Math.Max(y, minY), maxY);
        }

        private bool ConfirmDeleteRecipe(string recipeName)
        {
            string message = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "?덉떆??'{0}'????젣?섏떆寃좎뒿?덇퉴?"
                    : "Delete recipe '{0}'?",
                recipeName);
            MessageBoxResult result = MessageBox.Show(
                Window.GetWindow(this),
                message,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "?덉떆????젣" : "Delete recipe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private bool ConfirmQualifiedSnapshotLifecycle(
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
                Window.GetWindow(this),
                message,
                OpenVisionLanguageService.CurrentLanguage
                    == OpenVisionLanguage.Korean
                    ? "Qualified Snapshot 생명주기 확인"
                    : "Confirm qualified Snapshot lifecycle",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private bool OpenQualifiedSnapshotEvidence(string directory)
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

        private bool ConfirmDeletePipeline(string recipeName, string pipelineName)
        {
            string message = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "Delete pipeline '{1}' from recipe '{0}'?"
                    : "Delete pipeline '{1}' from recipe '{0}'?",
                recipeName,
                pipelineName);
            MessageBoxResult result = MessageBox.Show(
                Window.GetWindow(this),
                message,
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean ? "Delete pipeline" : "Delete pipeline",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private string SelectImportPipelineXmlPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "?뚯씠?꾨씪??XML ?먮뒗 寃??踰덈뱾 ?닿린"
                    : "Open pipeline XML or review bundle",
                Filter = "OpenVision XML / Review bundle (*.xml;*.review.zip;*.zip)|*.xml;*.review.zip;*.zip|OpenVision Pipeline XML (*.xml)|*.xml|Review bundle (*.review.zip;*.zip)|*.review.zip;*.zip|All files (*.*)|*.*",
                Multiselect = false
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true ? dialog.FileName : string.Empty;
        }

        private string SelectExportPipelineXmlPath(string suggestedFileName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "?뚯씠?꾨씪??XML ?대낫?닿린"
                    : "Export pipeline XML",
                Filter = "OpenVision Pipeline XML (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "Pipeline.xml" : suggestedFileName,
                AddExtension = true,
                DefaultExt = ".xml",
                OverwritePrompt = true
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true ? dialog.FileName : string.Empty;
        }

        private string SelectExportRecipeReviewBundlePath(string suggestedFileName)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "?덉떆??寃??臾띠쓬 ?대낫?닿린"
                    : "Export recipe review bundle",
                Filter = "OpenVision review bundle (*.review.zip)|*.review.zip|Zip archive (*.zip)|*.zip",
                FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "Pipeline.review.zip" : suggestedFileName,
                AddExtension = true,
                DefaultExt = ".zip",
                OverwritePrompt = true
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true ? dialog.FileName : string.Empty;
        }

        private IReadOnlyList<string> SelectValidationSetImagePaths(string expected)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "濡쒖뺄 寃利??명듃??" + expected + " ?대?吏 異붽?"
                    : "Add " + expected + " images to local validation set",
                Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true
                ? dialog.FileNames
                : Array.Empty<string>();
        }

        private string SelectValidationSetFolderPath(string expected)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "濡쒖뺄 寃利??명듃??" + expected + " ?대뜑 異붽?"
                    : "Add " + expected + " folder to local validation set",
                Multiselect = false
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true
                ? dialog.FolderName
                : string.Empty;
        }

        private string SelectValidationSetReplacementImagePath(string missingPath)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "?꾨씫 寃利??대?吏 援먯껜: " + Path.GetFileName(missingPath)
                    : "Replace missing validation image: " + Path.GetFileName(missingPath),
                Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true
            };

            return dialog.ShowDialog(Window.GetWindow(this)) == true
                ? dialog.FileName
                : string.Empty;
        }

        private bool ConfirmDeleteValidationSet(string setName)
        {
            MessageBoxResult result = MessageBox.Show(
                Window.GetWindow(this),
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "濡쒖뺄 寃利??명듃 '" + setName + "'????젣?섏떆寃좎뒿?덇퉴? ?대?吏 ?먮낯? ??젣?섏? ?딆뒿?덈떎."
                    : "Delete local validation set '" + setName + "'? Source images will not be deleted.",
                OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.Korean
                    ? "寃利??명듃 ??젣"
                    : "Delete validation set",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            chromeController.ApplyLocalization(IsToolRailCompact);
            mainActionPresenter?.ApplyLocalization();
            sampleWorkflowPresenter?.ApplyLocalization();
            if (sampleWorkflowPresenter?.IsVisible == true)
            {
                sampleWorkflowPresenter.ShowForActiveSample();
            }
            recipeContextPresenter?.Refresh();
            RecipeCommands?.RefreshLocalization();
            LayerCommands?.RefreshLocalization();
            ApplyShellLogLocalization();
        }

        private void OnRecipeContextChanged(object sender, EventArgs e)
        {
            recipeContextPresenter?.Refresh();
        }

        private void OnRuntimeRecipeChanged(object sender, EventArgs e)
        {
            RefreshRecipeContext();
            recipeController.OnRecipeChanged(sender, e);
            RefreshToolReadiness();
            RecipeCommands?.RefreshOptions();
            WorkspaceCommands?.RefreshCanExecute();
        }

        private void OnNativeToolPropertySaved(
            object sender,
            OpenVisionNativeToolPropertySavedEventArgs e)
        {
            RefreshToolReadiness();
            OpenVisionNativeToolDocument document =
                documentController?.ActiveNativeDocument;
            if (document == null
                || e == null
                || !string.Equals(
                    document.ToolName,
                    e.ToolName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string recipeName = string.IsNullOrWhiteSpace(e.RecipeName)
                ? T(
                    "VisionTool.Persistence.DefaultRecipe",
                    "default Recipe")
                : e.RecipeName;
            if (!e.Succeeded)
            {
                string format = T(
                    "VisionTool.Persistence.SaveFailedFormat",
                    "Settings could not be saved for {0} / Recipe {1}. "
                    + "The current values remain in memory but may be lost after reopening. Cause: {2}");
                document.SetPropertyPersistenceStatus(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        format,
                        e.ToolName,
                        recipeName,
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? T(
                                "VisionTool.Persistence.UnknownError",
                                "unknown error")
                            : e.ErrorMessage));
            }
            else if (e.RecoveredFromFailure)
            {
                string format = T(
                    "VisionTool.Persistence.SaveRecoveredFormat",
                    "Settings save recovered for {0} / Recipe {1}. "
                    + "The current values are now persisted.");
                document.SetPropertyPersistenceStatus(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        format,
                        e.ToolName,
                        recipeName));
            }
        }

        private void OnNativeToolSettingsSaved(
            object sender,
            OpenVisionNativeToolSettingsSavedEventArgs e)
        {
            RefreshToolReadiness();
            OpenVisionNativeToolDocument document =
                documentController?.ActiveNativeDocument;
            if (document == null
                || e == null
                || !string.Equals(
                    document.ToolName,
                    e.ToolName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!e.Succeeded)
            {
                document.SetPropertyPersistenceStatus(
                    OpenVisionNativeToolPersistenceStatusText.CreateSaveFailure(
                        e.ToolName,
                        e.RecipeName,
                        e.ErrorMessage));
            }
            else if (e.RecoveredFromFailure)
            {
                document.SetPropertyPersistenceStatus(
                    OpenVisionNativeToolPersistenceStatusText.CreateSaveRecovered(
                        e.ToolName,
                        e.RecipeName));
            }
        }

        private void RefreshToolReadiness()
        {
            ArithmeticToolSettings arithmeticSettings = OpenVisionNativeToolSettingsStore.Load(
                OpenVisionNativeToolSettingsStore.CreateConfigName("Arithmetic"),
                new ArithmeticToolSettings());
            bool arithmeticInputLayerBRequired = VisionPipelineArithmeticStep.RequiresInputLayerB(
                arithmeticSettings.UseOffsetMode
                    ? VisionPipelineArithmeticStep.ModeOffset
                    : VisionPipelineArithmeticStep.ModeOperation,
                arithmeticSettings.SelectedOperation,
                arithmeticSettings.UseConstantInput);
            viewModel.SetToolReadiness(
                displayManager.GetLayerImage("Main") != null,
                arithmeticInputLayerBRequired,
                HasSecondaryWorkspaceImage(),
                runtimeContext.Global?.VisionTools);
        }

        private bool HasSecondaryWorkspaceImage()
        {
            int imageCount = 0;
            for (int index = 0; index < displayManager.LayerCount; index++)
            {
                DrawingBitmap image = displayManager.GetLayerImage(index);
                if (image == null || DisplayManagerImageExtensions.IsPlaceholderBitmap(image))
                {
                    continue;
                }

                imageCount++;
                if (imageCount >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        private static void OnToolRailCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OpenVisionShellHostView view)
            {
                view.chromeController?.ApplyToolRailCompactState(view.IsToolRailCompact);
            }
        }

        private string ResolveRecipeName()
        {
            return recipeContextStore?.CurrentRecipeName ?? ResolveRuntimeRecipeName();
        }

        private string ResolveRuntimeRecipeName()
        {
            string recipeName = runtimeContext.Global?.Recipe?.Name;
            if (!string.IsNullOrWhiteSpace(recipeName))
            {
                return recipeName;
            }

            recipeName = PropertyGridEditorFactory.GetRecipeName();
            return string.IsNullOrWhiteSpace(recipeName) ? "Default" : recipeName;
        }

        private void RefreshRecipeContext()
        {
            recipeContextStore.Refresh();
            recipeContextPresenter?.Refresh();
        }

        private void SelectToolMenu(VISION_MENU menu)
        {
            OpenVisionShellNavItem item = viewModel.NavigationGroups
                .SelectMany(group => group.Items)
                .FirstOrDefault(command => command.Menu == menu);
            if (item != null)
            {
                viewModel.SelectToolCommand.Execute(item);
            }
        }

        private void ApplyActiveSampleFirstStepParameters()
        {
            VisionPipelineStep step = sampleWorkflowPresenter?.FirstStep;
            if (step == null)
            {
                return;
            }

            documentController.ActiveNativeDocument?.ApplySampleStepParameters(step);
        }

        private bool OpenWorkspaceSampleByNameFromReview(string sampleName)
        {
            return commandController?.OpenRunnableSampleByName(sampleName) == true;
        }

        private OpenVisionRecipeLayerCard BuildRecipeLayerCard(string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName) || string.Equals(layerName, "-", StringComparison.Ordinal))
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            int layerIndex = displayManager.FindIndex(layerName);
            if (layerIndex < 0)
            {
                return OpenVisionRecipeLayerCard.CreateMissing(layerName);
            }

            DrawingBitmap image = displayManager.GetLayerImage(layerName);
            string status = image == null
                ? LocalText("?대?吏 ?놁쓬", "No image")
                : string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}x{1}", image.Width, image.Height);
            return new OpenVisionRecipeLayerCard(
                layerName,
                status,
                OpenVisionBitmapImagePreviewFactory.Create(image),
                true);
        }

        private void SetWorkspaceDropOverlay(bool visible, bool canDrop)
        {
            if (workspaceDropOverlay == null)
            {
                return;
            }

            workspaceDropOverlay.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            workspaceDropOverlay.BorderBrush = new SolidColorBrush(canDrop
                ? Color.FromRgb(0x8A, 0xD7, 0xDA)
                : Color.FromRgb(0x9A, 0x64, 0x00));
            workspaceDropOverlay.Background = new SolidColorBrush(canDrop
                ? Color.FromArgb(0x33, 0x15, 0x7C, 0x86)
                : Color.FromArgb(0x2A, 0x9A, 0x64, 0x00));
        }

        private void ShellLogToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetShellLogExpanded(true);
        }

        private void ShellLogToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SetShellLogExpanded(false);
        }

        private void SetShellLogExpanded(bool expanded)
        {
            if (logPanelRow != null)
            {
                logPanelRow.Height = new GridLength(expanded ? 184D : 68D);
            }

            if (shellLogPanel != null)
            {
                shellLogPanel.Visibility = expanded ? Visibility.Visible : Visibility.Collapsed;
            }

            if (txtShellLogToggle != null)
            {
                txtShellLogToggle.Text = expanded
                    ? T("Shell.LogPanel.Close", LocalText("濡쒓렇 ?リ린", "Close Log"))
                    : T("Shell.LogPanel.Open", LocalText("濡쒓렇 ?닿린", "Open Log"));
            }
        }

        private void ApplyShellLogLocalization()
        {
            if (txtShellLogTitle != null)
            {
                txtShellLogTitle.Text = T("Pipeline.RunLog", LocalText("?ㅽ뻾 濡쒓렇", "Run Log"));
            }

            SetShellLogExpanded(btnShellLogToggle?.IsChecked == true);
        }

        private static string T(string key, string fallbackText)
        {
            string value = OpenVisionLanguageService.T(key);
            return string.IsNullOrWhiteSpace(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallbackText ?? string.Empty
                : value;
        }

        private static string LocalText(string korean, string english)
        {
            return OpenVisionLanguageService.CurrentLanguage == OpenVisionLanguage.English
                ? english ?? korean ?? string.Empty
                : korean ?? english ?? string.Empty;
        }

        private void SetDockedToolInspectorWidthForTestCore(double width)
        {
            if (toolInspectorColumn == null || width <= 0D)
            {
                return;
            }

            toolInspectorColumn.Width = new System.Windows.GridLength(width);
            toolInspectorColumn.MinWidth = Math.Min(toolInspectorColumn.MinWidth, width);
            toolInspectorPanel?.UpdateLayout();
            UpdateLayout();
        }
    }
}
