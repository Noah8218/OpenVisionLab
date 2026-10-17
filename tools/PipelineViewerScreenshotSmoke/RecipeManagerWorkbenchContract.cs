using OpenVisionLab;
using OpenVisionLab.Common;
using OpenVisionLab.Core;
using OpenVisionLab.Vision2D.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

internal static class RecipeManagerWorkbenchContract
{
    internal static int Run(string evidenceDirectory)
    {
        Prepare(evidenceDirectory);
        List<string> passed = new();
        string prefix = "Smoke_S01_" + Guid.NewGuid().ToString("N")[..10];
        string recipeA = prefix + "_A";
        string recipeB = prefix + "_B";
        Seed(recipeA);
        Seed(recipeB);
        string runtimeRecipe = recipeA;
        int switches = 0;
        int decisions = 0;
        bool confirmDelete = false;
        bool failSave = true;
        OpenVisionRecipePendingEditDecision decision = OpenVisionRecipePendingEditDecision.Cancel;
        OpenVisionShellHostRecipeCommandSurface commands = new(
            () => runtimeRecipe,
            name => { runtimeRecipe = name; switches++; },
            () => { },
            confirmDeleteRecipe: _ => confirmDelete,
            confirmDeletePipeline: (_, _) => confirmDelete,
            decidePendingEdit: _ => { decisions++; return decision; },
            saveStepEditPipeline: (recipe, pipeline) =>
            {
                if (failSave) throw new IOException("S01 isolated save failure");
                VisionPipelineStorage.Save(recipe, pipeline);
            });
        void Check(string name, bool condition)
        {
            if (!condition) throw new InvalidOperationException(name + ": " + commands.StatusText);
            passed.Add(name);
        }
        try
        {
            commands.RecipeManagerSelectedRecipeName = recipeB;
            commands.RecipeManagerSelectedPipelineName = "Inspection_B";
            Check("browse preserves working recipe/pipeline and active pipeline", runtimeRecipe == recipeA
                && commands.SelectedPipelineOption?.PipelineName == "Inspection_A"
                && Active(recipeB) == "Inspection_A" && switches == 0);
            Check("open transfers a non-active pipeline without activation", commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && runtimeRecipe == recipeB && commands.SelectedPipelineOption?.PipelineName == "Inspection_B"
                && Active(recipeB) == "Inspection_A" && switches == 1);
            Check("selected step loads through the existing editor", commands.FocusPipelineStepForEdit(recipeB, "Inspection_B", 1)
                && commands.SelectedStepEditObject != null);
            object? editObject = commands.SelectedStepEditObject;
            commands.MarkSelectedStepEditDirtyForQualificationTest();
            Check("same work keeps pending editor and does not reload", commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && ReferenceEquals(editObject, commands.SelectedStepEditObject) && commands.IsSelectedStepEditDirty
                && decisions == 0 && switches == 1);
            commands.RecipeManagerSelectedPipelineName = "Inspection_A";
            Check("cancel keeps pending editor and selected working pipeline", !commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && commands.SelectedPipelineOption?.PipelineName == "Inspection_B" && commands.IsSelectedStepEditDirty
                && runtimeRecipe == recipeB && switches == 1);
            string stalePath = RecipeWorkspaceService.GetVisionPipelinePath(recipeB, "Inspection_A");
            byte[] staleValid = File.ReadAllBytes(stalePath);
            int decisionsBeforeDamage = decisions;
            decision = OpenVisionRecipePendingEditDecision.Discard;
            File.WriteAllText(stalePath, "<VisionPipeline>");
            Check("post-selection XML damage preserves pending editor before discard", !commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && commands.IsSelectedStepEditDirty && ReferenceEquals(editObject, commands.SelectedStepEditObject)
                && commands.SelectedPipelineOption?.PipelineName == "Inspection_B" && switches == 1
                && decisions == decisionsBeforeDamage && File.ReadAllText(stalePath) == "<VisionPipeline>");
            File.WriteAllBytes(stalePath, staleValid);
            commands.RecipeManagerSelectedPipelineName = "Inspection_B";
            commands.RecipeManagerSelectedPipelineName = "Inspection_A";
            decision = OpenVisionRecipePendingEditDecision.ApplyAndContinue;
            Check("save failure blocks handoff and keeps the draft", !commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && commands.IsSelectedStepEditDirty && ReferenceEquals(editObject, commands.SelectedStepEditObject)
                && commands.SelectedPipelineOption?.PipelineName == "Inspection_B" && switches == 1);
            failSave = false;
            Check("save retry transfers only the selected working pipeline", commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && !commands.IsSelectedStepEditDirty && commands.SelectedPipelineOption?.PipelineName == "Inspection_A"
                && Active(recipeB) == "Inspection_A" && switches == 1);

            commands.RecipeManagerSelectedRecipeName = recipeA;
            commands.RecipeManagerSelectedPipelineName = "Inspection_B";
            commands.RecipeManagerPipelineEditName = "Inspection_Copy";
            commands.ManageBrowsedPipelineCommand.Execute("Duplicate");
            Check("manager duplicate preserves main work and active designation", File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_Copy"))
                && Active(recipeA) == "Inspection_A" && runtimeRecipe == recipeB
                && commands.SelectedPipelineOption?.PipelineName == "Inspection_A");
            commands.RecipeManagerPipelineEditName = "Inspection_Renamed";
            commands.ManageBrowsedPipelineCommand.Execute("Rename");
            Check("manager rename changes only browsed storage identity", File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_Renamed"))
                && !File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_Copy"))
                && Active(recipeA) == "Inspection_A" && runtimeRecipe == recipeB);
            commands.ManageBrowsedPipelineCommand.Execute("Delete");
            Check("delete cancellation retains the selected inspection", File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_Renamed")));
            confirmDelete = true;
            commands.ManageBrowsedPipelineCommand.Execute("Delete");
            Check("confirmed delete uses existing fallback and keeps other work", !File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_Renamed"))
                && File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_A"))
                && runtimeRecipe == recipeB);
            commands.RecipeManagerSelectedPipelineName = "Inspection_B";
            commands.ManageBrowsedPipelineCommand.Execute("Activate");
            Check("explicit activation updates browsed recipe only", Active(recipeA) == "Inspection_B"
                && Active(recipeB) == "Inspection_A" && runtimeRecipe == recipeB && switches == 1);

            string lockedPath = RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_B");
            using (FileStream locked = new(lockedPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                commands.RecipeManagerPipelineEditName = "Inspection_B_Retry";
                commands.ManageBrowsedPipelineCommand.Execute("Rename");
                Check("rename storage failure keeps identity and editable name", commands.RecipeManagerSelectedPipelineName == "Inspection_B"
                    && commands.RecipeManagerPipelineEditName == "Inspection_B_Retry"
                    && !File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_B_Retry")));
            }
            commands.ManageBrowsedPipelineCommand.Execute("Rename");
            Check("rename retry recovers and updates active storage identity", commands.RecipeManagerSelectedPipelineName == "Inspection_B_Retry"
                && Active(recipeA) == "Inspection_B_Retry");
            commands.ManageBrowsedPipelineCommand.Execute("Delete");
            Check("last inspection delete is disabled", commands.RecipeManagerSelectedPipelineOptions.Count == 1
                && !commands.ManageBrowsedPipelineCommand.CanExecute("Delete"));

            string blockedRecipe = prefix + "_BlockedCreate";
            string blockedDirectory = RecipeWorkspaceService.GetRecipeDirectoryPath(blockedRecipe);
            File.WriteAllText(blockedDirectory, "owned storage-failure fixture");
            commands.EditRecipeName = blockedRecipe;
            commands.CreateNamedRecipeCommand.Execute(null);
            Check("create storage failure keeps editable name and main work", commands.EditRecipeName == blockedRecipe
                && commands.RecipeManagerSelectedRecipeName == recipeA && runtimeRecipe == recipeB
                && commands.StatusText.Contains("실패", StringComparison.Ordinal));
            File.Delete(blockedDirectory);
            commands.EditRecipeName = prefix + "_New";
            commands.CreateNamedRecipeCommand.Execute(null);
            string created = commands.RecipeManagerSelectedRecipeName;
            Check("create adds a browsed recipe without changing main work", created == prefix + "_New"
                && File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(created, VisionPipelineAppendService.DefaultPipelineName)) && runtimeRecipe == recipeB);
            commands.EditRecipeName = created + "_Renamed";
            commands.RenameRecipeCommand.Execute(null);
            Check("recipe rename persists without switching other main work", commands.RecipeManagerSelectedRecipeName == created + "_Renamed" && runtimeRecipe == recipeB);
            commands.EditRecipeName = "temporary name";
            commands.CancelRecipeNameEditCommand.Execute(null);
            Check("escape command restores the browsed name", commands.EditRecipeName == commands.RecipeManagerSelectedRecipeName);
            commands.DuplicateRecipeCommand.Execute(null);
            Check("recipe copy preserves main work", commands.RecipeManagerSelectedRecipeName.EndsWith("_Copy", StringComparison.Ordinal)
                && runtimeRecipe == recipeB);

            for (int i = 0; i < 50; i++) Seed(prefix + "_Catalog_" + i.ToString("D2"));
            commands.RefreshOptions();
            commands.RecipeFilterText = prefix + "_Catalog_";
            Check("50 recipes remain searchable", commands.FilteredRecipeOptions.Count == 50);
            commands.RecipeFilterText = prefix + "_Catalog_49";
            Check("filter narrows to the exact long-list target", commands.FilteredRecipeOptions.Count == 1);
            commands.RecipeFilterText = "no_s01_match";
            Check("empty filter results preserve browse and working identities", commands.FilteredRecipeOptions.Count == 0 && runtimeRecipe == recipeB);
            commands.RecipeFilterText = string.Empty;
            commands.RecipeManagerSelectedRecipeName = recipeA;
            commands.RecipeManagerSelectedPipelineName = "Inspection_A";
            string path = RecipeWorkspaceService.GetVisionPipelinePath(recipeA, "Inspection_A");
            byte[] valid = File.ReadAllBytes(path);
            File.WriteAllText(path, "<VisionPipeline>");
            commands.RefreshOptions();
            Check("damaged XML disables handoff without overwriting bytes", !commands.RecipeManagerApplyToMainEnabled
                && !commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult()
                && File.ReadAllText(path) == "<VisionPipeline>" && runtimeRecipe == recipeB);
            File.WriteAllBytes(path, valid);
            commands.RefreshOptions();
            Check("restored XML recovers handoff without execution", commands.RecipeManagerApplyToMainEnabled
                && commands.ApplyRecipeManagerSelectionToMainAsync().GetAwaiter().GetResult() && runtimeRecipe == recipeA);
            int closed = 0;
            int opened = 0;
            int pendingPrompts = 0;
            int entrySwitches = 0;
            OpenVisionShellHostRecipeCommandSurface? entry = null;
            entry = new(
                () => recipeB,
                name => { if (name != recipeB) throw new InvalidOperationException("Unexpected fixture Recipe switch."); entrySwitches++; },
                () => { },
                decidePendingEdit: _ => { pendingPrompts++; return OpenVisionRecipePendingEditDecision.Cancel; },
                closeRecipeManager: () =>
                {
                    closed++;
                    entry!.HandleRecipeManagerCloseCommand.Execute(true);
                    entry.ApplyRecipeManagerSelectionToMainCommand.Execute(null);
                },
                openMainRecipeWork: section => { if (section == "Pipeline") opened++; });
            Check("work entry fixture loads the existing editor", entry.FocusPipelineStepForEdit(recipeB, "Inspection_A", 1));
            int switchesBeforeEntry = entrySwitches;
            object? pendingObject = entry.SelectedStepEditObject;
            entry.MarkSelectedStepEditDirtyForQualificationTest();
            entry.RecipeManagerSelectedPipelineName = "Inspection_B";
            entry.ApplyRecipeManagerSelectionToMainCommand.Execute(null);
            Check("canceled work-open command keeps the manager and draft", closed == 0 && opened == 0 && pendingPrompts == 1
                && entry.IsSelectedStepEditDirty && ReferenceEquals(pendingObject, entry.SelectedStepEditObject));
            entry.RecipeManagerSelectedPipelineName = "Inspection_A";
            entry.ApplyRecipeManagerSelectionToMainCommand.Execute(null);
            Check("same-work command opens the main without prompting again or resetting draft", closed == 1 && opened == 0 && pendingPrompts == 1
                && !entry.IsRecipeManagerDetailContextActive && entry.IsSelectedStepEditDirty
                && ReferenceEquals(pendingObject, entry.SelectedStepEditObject) && entry.SelectedPipelineOption?.PipelineName == "Inspection_A"
                && entrySwitches == switchesBeforeEntry);
            Check("normal close restores its pending-edit guard after work-open", !entry.TryCloseRecipeManager()
                && pendingPrompts == 2 && entry.IsSelectedStepEditDirty && ReferenceEquals(pendingObject, entry.SelectedStepEditObject));
            OpenVisionRecipePipelineLifecycleResult legacyCopy = new OpenVisionRecipePipelineLifecycleUseCase().Duplicate(recipeA, "Inspection_A", "Legacy_Copy");
            Check("legacy duplicate API retains default activation", legacyCopy.Succeeded && Active(recipeA) == legacyCopy.PipelineName
                && File.Exists(RecipeWorkspaceService.GetVisionPipelinePath(recipeA, legacyCopy.PipelineName)));
            CheckRenderedPipelineBindings(evidenceDirectory, prefix, Check);
            File.WriteAllLines(Path.Combine(evidenceDirectory, "workbench-contract.txt"), passed.Select(item => "PASS: " + item));
            Console.WriteLine("S01 workbench contract: PASS " + passed.Count);
            return 0;
        }
        catch (Exception ex)
        {
            File.WriteAllLines(Path.Combine(evidenceDirectory, "workbench-contract.txt"), passed.Select(item => "PASS: " + item).Append("FAIL: " + ex));
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void CheckRenderedPipelineBindings(string evidenceDirectory, string prefix, Action<string, bool> check)
    {
        string main = prefix + "_Binding_Main";
        string browsed = prefix + "_Binding_Other";
        Seed(main);
        Seed(browsed);
        GlobalState global = new();
        global.Recipe.Name = main;
        DisplayManagerService display = new();
        PropertyGridEditorFactory.SetRuntimeContext(() => display);
        PropertyGridEditorFactory.SetRecipeNameContext(() => global.Recipe.Name);
        OpenVisionShellHostView shell = new(new ApplicationRuntimeContext(global, display));
        Window window = new() { Title = "S01 rendered pipeline binding contract", Width = 1600, Height = 900, Content = shell };
        ListBox? Find(DependencyObject source)
        {
            if (source is ListBox list && System.Windows.Automation.AutomationProperties.GetAutomationId(list) == "HostRecipeStoredInspectionSelector") return list;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(source); i++)
            {
                ListBox? found = Find(VisualTreeHelper.GetChild(source, i));
                if (found != null) return found;
            }
            return null;
        }
        try
        {
            window.Show();
            File.WriteAllText(Path.Combine(evidenceDirectory, "binding-monitor.txt"), SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor(window));
            shell.SetRecipeManagerOpenForTest(true);
            window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            ListBox list = Find(shell) ?? throw new InvalidOperationException("Saved inspection list is missing.");
            bool SelectionMatches(string pipeline)
            {
                window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                return (string?)list.SelectedValue == pipeline && shell.RecipeCommands.RecipeManagerSelectedPipelineName == pipeline
                    && list.SelectedItem != null && list.ItemContainerGenerator.ContainerFromItem(list.SelectedItem) is ListBoxItem item && item.IsSelected;
            }
            check("rendered initial selection matches its visible container", SelectionMatches("Inspection_A"));
            list.SelectedValue = "Inspection_B";
            check("rendered view selection updates browse identity only", SelectionMatches("Inspection_B")
                && shell.RecipeCommands.SelectedPipelineOption?.PipelineName == "Inspection_A" && shell.RecipeCommands.MainRecipeName == main);
            ScreenshotPngWriter.WriteDpiAwareWindowPng(window, Path.Combine(evidenceDirectory, "binding-view-selection.png"));
            window.Width = 1280;
            window.Height = 760;
            check("compact resize keeps the visible selection and working identity", SelectionMatches("Inspection_B")
                && shell.RecipeCommands.MainRecipeName == main && shell.RecipeCommands.SelectedPipelineOption?.PipelineName == "Inspection_A");
            ScreenshotPngWriter.WriteDpiAwareWindowPng(window, Path.Combine(evidenceDirectory, "binding-compact.png"));
            window.Width = 1600;
            window.Height = 900;
            shell.RecipeCommands.RecipeManagerSelectedPipelineName = "Inspection_A";
            check("rendered VM selection updates the selected container", SelectionMatches("Inspection_A"));
            shell.RecipeCommands.RecipeManagerSelectedRecipeName = browsed;
            check("rendered recipe browse restores selection without execution or layers", SelectionMatches("Inspection_A")
                && shell.RecipeCommands.MainRecipeName == main && shell.RecipeCommands.SelectedPipelineOption?.PipelineName == "Inspection_A"
                && shell.NativePreviewRunCount == 0 && shell.LayerDocumentCount == 0);
        }
        finally
        {
            window.Close();
            shell.Dispose();
            display.Dispose();
        }
    }

    internal static void ShowInteractive(string evidenceDirectory)
    {
        Prepare(evidenceDirectory);
        if (!File.Exists(Path.Combine(evidenceDirectory, "fixture-seeded.txt")))
        {
            for (int i = 0; i < 50; i++) Seed("S01_" + i.ToString("D2") + "_레시피_패드_개수_검사");
            Seed("S01_긴_레시피_이름_전자부품_연결부_상하좌우_패드_위치_정렬_검사_저장본");
            File.WriteAllText(Path.Combine(evidenceDirectory, "fixture-seeded.txt"), AppPathService.DataRootDirectory);
        }
        GlobalState global = new();
        global.Recipe.Name = "S01_00_레시피_패드_개수_검사";
        DisplayManagerService displayManager = new();
        PropertyGridEditorFactory.SetRuntimeContext(() => displayManager);
        PropertyGridEditorFactory.SetRecipeNameContext(() => global.Recipe.Name);
        ApplicationRuntimeContext runtime = new(global, displayManager);
        OpenVisionShellHostView shell = new(runtime);
        Window window = new() { Title = "S01 Recipe Manager verification", Content = shell, Width = 1600, Height = 900, MinWidth = 1000, MinHeight = 700 };
        HashSet<string> captured = new(StringComparer.Ordinal);
        FrameworkElement? previousMouseControl = null;
        FrameworkElement? FindControl(DependencyObject? source)
        {
            while (source != null)
            {
                if (source is FrameworkElement element && System.Windows.Automation.AutomationProperties.GetAutomationId(element).StartsWith("HostRecipe", StringComparison.Ordinal)) return element;
                source = source is Visual ? VisualTreeHelper.GetParent(source) : null;
            }
            return null;
        }
        void Record(FrameworkElement? control)
        {
            if (control == null || !control.IsVisible) return;
            string id = System.Windows.Automation.AutomationProperties.GetAutomationId(control);
            bool pressed = control is ButtonBase button && button.IsPressed;
            bool selected = control is ToggleButton toggle && toggle.IsChecked == true;
            string state = !control.IsEnabled ? "disabled" : pressed ? "pressed" : selected ? "checked" : "normal";
            if (control.IsKeyboardFocusWithin) state += "-focused";
            if (control.IsMouseOver) state += "-hover";
            string filename = id + "_" + state + ".png";
            if (captured.Add(id + "_" + state)) ScreenshotPngWriter.WriteDpiAwareWindowPng(window, Path.Combine(evidenceDirectory, filename));
            string record = JsonSerializer.Serialize(new
            {
                id, state, filename, control.IsEnabled, control.IsMouseOver, control.IsKeyboardFocusWithin, Pressed = pressed, Checked = selected,
                Text = control is TextBox textBox ? textBox.Text : control is ContentControl content ? content.Content?.ToString() : null,
                Selection = control is ListBox list ? list.SelectedValue?.ToString() : null,
                Main = shell.RecipeCommands.MainRecipeName, Work = shell.RecipeCommands.SelectedPipelineOption?.PipelineName,
                BrowseRecipe = shell.RecipeCommands.RecipeManagerSelectedRecipeName, BrowsePipeline = shell.RecipeCommands.RecipeManagerSelectedPipelineName,
                Active = Active(shell.RecipeCommands.RecipeManagerSelectedRecipeName),
                Runs = shell.NativePreviewRunCount, Layers = shell.LayerDocumentCount, Dpi = VisualTreeHelper.GetDpi(window).DpiScaleX
            });
            File.AppendAllText(Path.Combine(evidenceDirectory, "input-observations.jsonl"), record + Environment.NewLine);
        }
        void Observe(object sender, ProcessInputEventArgs args)
        {
            if (args.StagingItem.Input is MouseEventArgs)
            {
                FrameworkElement? current = FindControl(Mouse.DirectlyOver as DependencyObject);
                if (!ReferenceEquals(previousMouseControl, current)) Record(previousMouseControl);
                Record(current);
                previousMouseControl = current;
            }
            else if (args.StagingItem.Input is KeyEventArgs) Record(FindControl(Keyboard.FocusedElement as DependencyObject));
        }
        InputManager.Current.PostProcessInput += Observe;
        window.Closed += (_, _) => { InputManager.Current.PostProcessInput -= Observe; shell.Dispose(); displayManager.Dispose(); };
        window.Show();
        File.WriteAllText(Path.Combine(evidenceDirectory, "monitor.txt"), SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor(window));
        window.Activate();
        shell.SetRecipeManagerOpenForTest(true);
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        ScreenshotPngWriter.WriteDpiAwareWindowPng(window, Path.Combine(evidenceDirectory, "interactive-initial.png"));
        Application.Current.Run();
    }

    private static void Prepare(string evidenceDirectory)
    {
        if (!string.Equals(Path.GetPathRoot(Path.GetFullPath(evidenceDirectory)), @"D:\", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(Path.GetPathRoot(AppPathService.DataRootDirectory), @"D:\", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("S01 evidence and isolated runtime must stay on D:.");
        Directory.CreateDirectory(evidenceDirectory);
        _ = Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnLastWindowClose };
        OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, false);
    }

    private static void Seed(string recipe)
    {
        VisionPipelineStorage.Save(recipe, RecipeContextFixture.CreatePipeline("Inspection_A", 2));
        VisionPipelineStorage.Save(recipe, RecipeContextFixture.CreatePipeline("Inspection_B", 3));
        VisionPipelineStorage.SaveActivePipelineName(recipe, "Inspection_A");
    }

    private static string Active(string recipe) => VisionPipelineStorage.LoadActivePipelineName(recipe, "VisionPipeline");

}
