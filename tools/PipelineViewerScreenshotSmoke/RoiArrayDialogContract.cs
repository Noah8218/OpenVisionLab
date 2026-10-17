using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using OpenVisionLab;
using OpenVisionLab.ImageCanvas;
using OpenVisionLab.ImageCanvas.ViewModels;
using OpenVisionLab.ImageCanvas.Views;

internal static class RoiArrayDialogContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory ?? @"D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-array-contract");
        if (!evidenceDirectory.StartsWith(@"D:\OpenVisionLab-TestData\", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("ROI array evidence must remain under D:\\OpenVisionLab-TestData.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> report = new List<string>();
        int failures = 0;
        void Check(string name, bool passed)
        {
            report.Add((passed ? "PASS " : "FAIL ") + name);
            if (!passed) failures++;
        }

        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        OpenVisionLanguage originalLanguage = OpenVisionLanguageService.CurrentLanguage;
        AddRoiArrayView? view = null;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            AddRoiArrayViewModel vm = new AddRoiArrayViewModel();
            List<bool> requests = new List<bool>();
            vm.RequestClose += requests.Add;
            AddRoiArrayData? Accepted() => (AddRoiArrayData?)typeof(AddRoiArrayViewModel).GetProperty("AcceptedData", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm);
            int SubscriberCount() => ((Delegate?)typeof(AddRoiArrayViewModel).GetField("RequestClose", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm))?.GetInvocationList().Length ?? 0;
            foreach ((string rows, string columns, string rowGap, string columnGap) in new[]
            {
                ("", "2", "1", "1"), ("text", "2", "1", "1"), ("0", "2", "1", "1"),
                ("2", "-1", "1", "1"), ("2", "1.5", "1", "1"), ("2147483648", "2", "1", "1"),
                ("2", "2", "", "1"), ("2", "2", "NaN", "1"), ("2", "2", "1", "Infinity"),
                ("2", "2", "-Infinity", "1"), ("2", "2", "1e99", "1")
            })
            {
                vm.Rows = rows;
                vm.Columns = columns;
                vm.RowSpacing = rowGap;
                vm.ColumnSpacing = columnGap;
                vm.ApplyCommand.Execute(null);
                Check($"Invalid [{rows}, {columns}, {rowGap}, {columnGap}] stays open with explanation",
                    requests.Count == 0 && Accepted() == null && !string.IsNullOrWhiteSpace(vm.ValidationMessage));
            }

            vm.Rows = "2";
            vm.Columns = "3";
            vm.RowSpacing = "-1.5";
            vm.ColumnSpacing = "0";
            vm.ApplyCommand.Execute(null);
            Check("Valid Apply creates exact accepted snapshot; signed and zero gaps remain supported", requests.SequenceEqual(new[] { true })
                && Accepted() is { Rows: 2, Columns: 3, RowSpacing: -1.5f, ColumnSpacing: 0f } && vm.ValidationMessage == string.Empty);
            vm.Rows = "9";
            Check("Accepted data does not re-read a subsequently edited draft", Accepted()?.Rows == 2);
            vm.CancelCommand.Execute(null);
            Check("Cancel reports false and clears acceptance without changing input values", requests.SequenceEqual(new[] { true, false }) && Accepted() == null && vm.Rows == "9");

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            vm.RowSpacing = "1,5";
            vm.ColumnSpacing = "-2,5";
            vm.ApplyCommand.Execute(null);
            Check("Current-culture decimal parsing preserved", Accepted() is { RowSpacing: 1.5f, ColumnSpacing: -2.5f });

            view = new AddRoiArrayView { DataContext = vm };
            Grid grid = (Grid)view.Content;
            TextBox rowsEditor = grid.Children.OfType<TextBox>().Single(item => AutomationProperties.GetAutomationId(item) == "RoiArrayRows");
            grid.Measure(new Size(396, double.PositiveInfinity));
            grid.Arrange(new Rect(0, 0, 396, grid.DesiredSize.Height));
            grid.UpdateLayout();
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
            Check("Initial draft reaches the unshown editor before user input", rowsEditor.Text == vm.Rows);
            rowsEditor.SetCurrentValue(TextBox.TextProperty, "invalid");
            rowsEditor.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
            vm.ApplyCommand.Execute(null);
            grid.Measure(new Size(396, double.PositiveInfinity));
            grid.Arrange(new Rect(0, 0, 396, grid.DesiredSize.Height));
            grid.UpdateLayout();
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
            TextBlock message = grid.Children.OfType<TextBlock>().Single(item => AutomationProperties.GetAutomationId(item) == "RoiArrayValidationMessage");
            Button apply = grid.Children.OfType<Button>().Single(item => AutomationProperties.GetAutomationId(item) == "RoiArrayApplyButton");
            Button cancel = grid.Children.OfType<Button>().Single(item => AutomationProperties.GetAutomationId(item) == "RoiArrayCancelButton");
            Check($"Editor binding reaches the VM and error reaches the View [rows={vm.Rows}; message={message.Text}]",
                vm.Rows == "invalid" && message.Text == "Rows must be a positive whole number.");
            Check("Validation row does not overlap command row", message.TranslatePoint(new Point(0, message.ActualHeight), grid).Y <= apply.TranslatePoint(new Point(), grid).Y);
            Check("Default/cancel command bindings are preserved", apply.IsDefault && cancel.IsCancel && ReferenceEquals(apply.Command, vm.ApplyCommand) && ReferenceEquals(cancel.Command, vm.CancelCommand));

            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            Check("Repeated Loaded does not duplicate close callback", SubscriberCount() == 2);
            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
            Check("Unloaded detaches close callback", SubscriberCount() == 1);
            view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            view.Close();
            Check("Closed detaches callback from reused VM", SubscriberCount() == 1);
            view = null;
            vm.CancelCommand.Execute(null);
            Check("Reused VM can cancel after View close without stale Window callback", requests.Last() == false);
        }
        catch (Exception exception)
        {
            Check("Contract exception: " + exception.GetBaseException().Message, false);
        }
        finally
        {
            view?.Close();
            CultureInfo.CurrentCulture = originalCulture;
            OpenVisionLanguageService.SetLanguage(originalLanguage, save: false);
        }

        report.Insert(0, "Build: " + typeof(AddRoiArrayView).Assembly.ManifestModule.ModuleVersionId);
        report.Add("Boundary: in-process VM, bindings, layout and explicitly raised Loaded/Unloaded lifecycle; no Show/ShowDialog or native ImageCanvas.");
        report.Add("Unverified: actual public modal Cancel/X/Apply overlay flow, rendering, pointer/keyboard, theme/DPI, window placement and calibration/large arrays.");
        string reportPath = Path.Combine(evidenceDirectory, "roi-array-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine($"ROI array contract: {(failures == 0 ? "PASS" : "FAIL")} ({reportPath})");
        return failures == 0 ? 0 : 1;
    }
}
