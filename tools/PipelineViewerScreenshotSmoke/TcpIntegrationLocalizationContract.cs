using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using OpenVisionLab;

internal static class TcpIntegrationLocalizationContract
{
    internal static int Run(string? requestedEvidenceDirectory)
    {
        const string testRoot = @"D:\OpenVisionLab-TestData\";
        string configuredRoot = Environment.GetEnvironmentVariable(AppPathService.DataRootEnvironmentVariable) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(configuredRoot) || !Path.GetFullPath(configuredRoot).StartsWith(testRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Run in a fresh process with OPENVISIONLAB_DATA_ROOT under D:\\OpenVisionLab-TestData.");
        }

        string evidenceDirectory = Path.GetFullPath(requestedEvidenceDirectory ?? Path.Combine(configuredRoot, "tcp-localization-contract"));
        if (!evidenceDirectory.StartsWith(testRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("TCP localization evidence must remain under D:\\OpenVisionLab-TestData.");
        }

        // Bootstrap may create/migrate isolated runtime directories; the assertion below covers TCP settings only.
        string dataRoot = AppPathService.DataRootDirectory;
        if (!string.Equals(Path.GetFullPath(configuredRoot).TrimEnd('\\'), dataRoot.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The runtime data root was already initialized elsewhere; use a fresh process.");
        }

        string settingsPath = Path.Combine(dataRoot, "CONFIG", "UI", "TcpIntegration2D.json");
        if (File.Exists(settingsPath))
        {
            throw new InvalidOperationException("Use a fresh isolated data root without saved TCP settings for the idle fixture.");
        }

        Directory.CreateDirectory(evidenceDirectory);
        List<string> report = new List<string>();
        int failures = 0;
        void Check(string name, bool passed)
        {
            report.Add((passed ? "PASS " : "FAIL ") + name);
            if (!passed) failures++;
        }

        OpenVisionLanguage originalLanguage = OpenVisionLanguageService.CurrentLanguage;
        Type controllerType = typeof(OpenVisionShellHostView).Assembly.GetType("OpenVisionLab.OpenVisionTcpIntegrationController", throwOnError: true)!;
        object? controller = null;
        try
        {
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            controller = Activator.CreateInstance(controllerType, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { Dispatcher.CurrentDispatcher }, null)!;
            object? Read(string name) => controllerType.GetProperty(name)!.GetValue(controller);
            void Write(string name, string value) => controllerType.GetProperty(name)!.SetValue(controller, value);
            object? ReadField(string name) => controllerType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(controller);
            StackPanel host = new StackPanel { DataContext = controller };
            TextBlock Bind(string name)
            {
                TextBlock text = new TextBlock();
                text.SetBinding(TextBlock.TextProperty, new Binding(name));
                host.Children.Add(text);
                return text;
            }

            TextBlock session = Bind("SessionStatusText");
            TextBlock operation = Bind("OperationStatusText");
            TextBlock validation = Bind("ValidationMessageText");
            void RefreshLayout()
            {
                host.Measure(new Size(800, 600));
                host.Arrange(new Rect(0, 0, 800, 600));
                host.UpdateLayout();
                Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.DataBind);
            }

            RefreshLayout();
            Check("KO idle binding captions", session.Text == "중지됨" && operation.Text == "설정을 복원했습니다. 수신은 시작되지 않았습니다.");
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            RefreshLayout();
            Check("EN idle binding captions", session.Text == "Stopped" && operation.Text == "Settings restored. Listening was not started.");
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            RefreshLayout();
            Check("KO idle round trip", session.Text == "중지됨" && operation.Text == "설정을 복원했습니다. 수신은 시작되지 않았습니다.");

            string originalAddress = (string)Read("ListenAddress")!;
            Write("ListenAddress", "invalid-listen-address");
            RefreshLayout();
            Check("KO invalid input binding", validation.Text == "수신 IP 주소가 올바르지 않습니다.");
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            RefreshLayout();
            Check("EN invalid input binding", validation.Text == "Listen IP address is invalid.");
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            RefreshLayout();
            Check("KO invalid input round trip", validation.Text == "수신 IP 주소가 올바르지 않습니다.");
            object?[] validationArguments = { null, null };
            controllerType.GetMethod("TryValidateSettings", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(controller, validationArguments);
            controllerType.GetMethod("SetOperationStatus", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(controller, new[] { validationArguments[1] });
            Write("ListenAddress", originalAddress);
            RefreshLayout();
            Check("Valid input recovery", validation.Text == string.Empty);
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.English, save: false);
            RefreshLayout();
            Check("Recorded validation outcome is not replaced by current valid settings", operation.Text == "Listen IP address is invalid." && validation.Text == string.Empty);
            OpenVisionLanguageService.SetLanguage(OpenVisionLanguage.Korean, save: false);
            RefreshLayout();
            Check("Recorded outcome returns to KO without changing meaning", operation.Text == "수신 IP 주소가 올바르지 않습니다." && validation.Text == string.Empty);

            const string rawError = "OriginalError 원문 · transaction-A · D:\\fixture\\image.png";
            const string runId = "run-observed-A";
            Write("OperationStatusText", rawError);
            Write("ResultErrorText", rawError);
            Write("ResultRunIdText", runId);
            Write("LastTransferText", "transaction-observed-A · 123 bytes");
            Write("ListenAddress", "127.0.0.2");
            foreach (OpenVisionLanguage language in new[] { OpenVisionLanguage.English, OpenVisionLanguage.Korean })
            {
                OpenVisionLanguageService.SetLanguage(language, save: false);
                RefreshLayout();
                Check(language + " raw error and observed result snapshots unchanged",
                    operation.Text == rawError && (string)Read("ResultErrorText")! == rawError
                    && (string)Read("ResultRunIdText")! == runId
                    && (string)Read("LastTransferText")! == "transaction-observed-A · 123 bytes");
            }

            Check("No listener or operation was started", !(bool)Read("IsListening")! && !(bool)Read("IsBusy")!
                && ReadField("exchange") == null && ReadField("activeOperationTask") == null);
            Check("No transaction or image was selected", ((ICollection)Read("Transactions")!).Count == 0
                && Read("SelectedTransaction") == null && (string)Read("SelectedSourceImagePath")! == string.Empty);
            Check("TCP settings were not saved", !File.Exists(settingsPath));
        }
        catch (Exception exception)
        {
            Check("Contract exception: " + exception.GetBaseException().Message, false);
        }
        finally
        {
            (controller as IDisposable)?.Dispose();
            OpenVisionLanguageService.SetLanguage(originalLanguage, save: false);
        }

        report.Insert(0, "Build: " + typeof(OpenVisionShellHostView).Assembly.ManifestModule.ModuleVersionId);
        report.Add("Boundary: in-process controller and WPF TextBlock bindings only; no Window.Show, TCP commands, sockets, inspection, or settings-save command.");
        report.Add("Unverified: actual window/empty canvas rendering, active listening endpoint, in-flight operation, themes, DPI, pointer and keyboard paths.");
        string reportPath = Path.Combine(evidenceDirectory, "tcp-localization-contract.txt");
        File.WriteAllLines(reportPath, report);
        Console.WriteLine($"TCP localization contract: {(failures == 0 ? "PASS" : "FAIL")} ({reportPath})");
        return failures == 0 ? 0 : 1;
    }
}
