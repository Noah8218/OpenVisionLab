#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenVisionLab.Core.Integration;

namespace OpenVisionLab;

internal sealed class OpenVisionTcpIntegrationExeSmokeReport
{
    public string Schema { get; init; } = "1.0";
    public DateTimeOffset CapturedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string Role { get; init; } = "consumer";
    public string? IntegrationProfilePath { get; init; }
    public string? TransactionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ScreenshotPath { get; init; }
    public string? SourceSha256 { get; init; }
    public int SourceImageWidth { get; init; }
    public int SourceImageHeight { get; init; }
    public string? RecipeSha256 { get; init; }
    public required IReadOnlyDictionary<string, bool> Checks { get; init; }
    public required IReadOnlyList<string> Failures { get; init; }
    public bool IsValid => Failures.Count == 0 && Checks.Values.All(value => value);

    public void Save(string path)
    {
        var fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(
            fullPath,
            JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }));
    }
}

internal static class OpenVisionTcpIntegrationExeSmoke
{
    public static bool IsRequested(IReadOnlyList<string> args) =>
        string.Equals(
            GetArgumentValue(args, "--smoke-integration-exe-role"),
            "consumer",
            StringComparison.OrdinalIgnoreCase);

    public static async Task<bool> RunAsync(
        OpenVisionShellHostWindow shellWindow,
        IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(shellWindow);
        ArgumentNullException.ThrowIfNull(args);

        var checks = new Dictionary<string, bool>(StringComparer.Ordinal);
        var failures = new List<string>();
        var transactionId = (Guid?)null;
        var status = string.Empty;
        var reportPath = RequireArgument(args, "--smoke-integration-exe-report");
        var integrationProfilePath = GetArgumentValue(args, "--smoke-integration-profile");
        var screenshotPath = GetArgumentValue(args, "--smoke-integration-exe-screenshot");
        var holdMilliseconds = ParseIntArgument(
            GetArgumentValue(args, "--smoke-integration-exe-hold-ms"),
            0,
            0,
            30000);

        void Check(string name, bool passed)
        {
            checks[name] = passed;
            if (!passed)
            {
                failures.Add(name);
            }
        }

        OpenVisionTcpIntegrationController? controller = null;
        string? sourceSha256 = null;
        string? recipeSha256 = null;
        var sourceImageWidth = 0;
        var sourceImageHeight = 0;
        try
        {
            var root = RequireArgument(args, "--smoke-integration-exchange-root");
            var listenPort = ParseIntArgument(
                GetArgumentValue(args, "--smoke-integration-listen-port"),
                45102,
                1,
                65535);
            var peerPort = ParseIntArgument(
                GetArgumentValue(args, "--smoke-integration-peer-port"),
                45101,
                1,
                65535);
            Directory.CreateDirectory(root);

            if (!string.IsNullOrWhiteSpace(integrationProfilePath))
            {
                Check("integrationProfilePresent", File.Exists(Path.GetFullPath(integrationProfilePath)));
                if (File.Exists(Path.GetFullPath(integrationProfilePath)))
                {
                    try
                    {
                        using var profile = JsonDocument.Parse(File.ReadAllText(integrationProfilePath));
                        var profileRoot = profile.RootElement;
                        Check("integrationProfileSchema", profileRoot.GetProperty("schemaVersion").GetInt32() == 1);
                        Check("integrationProfileExchangeRoot",
                            string.Equals(
                                profileRoot.GetProperty("twoDExchangeRoot").GetString(),
                                Path.GetFullPath(root),
                                StringComparison.OrdinalIgnoreCase));
                        Check("integrationProfileTwoDListenPort",
                            profileRoot.GetProperty("twoD").GetProperty("listenPort").GetInt32() == listenPort);
                    }
                    catch (Exception exception) when (exception is IOException or JsonException or KeyNotFoundException)
                    {
                        Check("integrationProfileSchema", false);
                    }
                }
            }

            controller = new OpenVisionTcpIntegrationController(
                shellWindow.Dispatcher);
            controller.LocalExchangeRoot = root;
            controller.ListenAddress = "127.0.0.1";
            controller.ListenPortText = listenPort.ToString();
            controller.PeerHost = "127.0.0.1";
            controller.PeerPortText = peerPort.ToString();
            controller.SetSessionSharedKey(
                Environment.GetEnvironmentVariable(
                    OpenVisionTcpIntegrationController.SharedKeyEnvironmentVariable)
                ?? string.Empty);
            controller.Show(shellWindow);
            await shellWindow.Dispatcher.InvokeAsync(
                () => { },
                DispatcherPriority.ApplicationIdle);

            Check("consumerSettingsValid", controller.CanStart);
            await controller.StartAsync();
            Check("consumerListening", controller.IsListening);

            await controller.PingAsync();
            Check(
                "machinePingAccepted",
                controller.LastTransferText.StartsWith(
                    "Ping",
                    StringComparison.OrdinalIgnoreCase));

            await WaitForAsync(
                () =>
                {
                    controller.RefreshTransactions();
                    return controller.Transactions.Count > 0;
                },
                TimeSpan.FromSeconds(30),
                "2D consumer did not discover the pushed Machine transaction.");
            Check("handoffDiscovered", controller.Transactions.Count > 0);

            var selected = controller.SelectedTransaction
                ?? throw new InvalidOperationException(
                    "The 2D consumer did not select its discovered transaction.");
            transactionId = selected.TransactionId;
            Check("handoffTargetsTwoD", selected.IsTwoDTarget);
            Check("handoffStartsWithoutAckOrResult", !selected.HasAcknowledgement && !selected.HasResult);
            var transactionRoot = Path.Combine(
                root,
                "transactions",
                selected.TransactionId.ToString("D"));
            var handoffPath = Path.Combine(transactionRoot, "handoff.json");
            if (File.Exists(handoffPath))
            {
                using var handoffDocument = JsonDocument.Parse(File.ReadAllText(handoffPath));
                var context = handoffDocument.RootElement.GetProperty("context");
                sourceSha256 = context.GetProperty("inputSha256").GetString();
                recipeSha256 = context.GetProperty("recipeSha256").GetString();
                Check("handoffSourceHashPresent", !string.IsNullOrWhiteSpace(sourceSha256));
                Check("handoffRecipeHashPresent", !string.IsNullOrWhiteSpace(recipeSha256));
            }

            await controller.AcknowledgeAsync();
            controller.RefreshTransactions();
            selected = controller.SelectedTransaction
                ?? throw new InvalidOperationException("The 2D transaction disappeared after ACK.");
            Check("acknowledgementCreated", selected.HasAcknowledgement);
            Check("acknowledgementAccepted", controller.AcknowledgementStatusText == "Accepted");

            await controller.RunInspectionAsync();
            controller.RefreshTransactions();
            selected = controller.SelectedTransaction
                ?? throw new InvalidOperationException("The 2D transaction disappeared after Run.");
            Check("resultCreated", selected.HasResult);
            Check("resultCompleted", controller.ResultStatusText == "Completed");
            Check("resultHasRunId", !string.IsNullOrWhiteSpace(controller.ResultRunIdText)
                && controller.ResultRunIdText != "-");

            var runRecordPath = Path.Combine(
                transactionRoot,
                "artifacts",
                "2d-run-record.json");
            if (File.Exists(runRecordPath))
            {
                using var runRecord = JsonDocument.Parse(File.ReadAllText(runRecordPath));
                var runRecordRoot = runRecord.RootElement;
                sourceImageWidth = runRecordRoot.GetProperty("sourceImageWidth").GetInt32();
                sourceImageHeight = runRecordRoot.GetProperty("sourceImageHeight").GetInt32();
                Check("sourceDimensionsPresent", sourceImageWidth > 0 && sourceImageHeight > 0);
            }

            if (!string.IsNullOrWhiteSpace(screenshotPath)
                && controller.WindowForSmoke is { } integrationWindow)
            {
                CaptureWindow(integrationWindow, screenshotPath);
                Check("integrationScreenshotCaptured", File.Exists(Path.GetFullPath(screenshotPath)));
            }

            await controller.PushAsync();
            Check(
                "resultPushedToMachine",
                controller.LastTransferText.StartsWith(
                    "Push",
                    StringComparison.OrdinalIgnoreCase));
            status = controller.OperationStatusText;
        }
        catch (Exception exception)
        {
            failures.Add(exception.GetBaseException().Message);
            status = exception.GetBaseException().ToString();
        }
        finally
        {
            var report = new OpenVisionTcpIntegrationExeSmokeReport
            {
                IntegrationProfilePath = integrationProfilePath,
                TransactionId = transactionId?.ToString("D"),
                Status = status,
                ScreenshotPath = screenshotPath,
                SourceSha256 = sourceSha256,
                SourceImageWidth = sourceImageWidth,
                SourceImageHeight = sourceImageHeight,
                RecipeSha256 = recipeSha256,
                Checks = checks,
                Failures = failures
            };
            report.Save(reportPath);
            if (holdMilliseconds > 0)
            {
                await Task.Delay(holdMilliseconds);
            }

            if (controller is not null)
            {
                await controller.StopAsync();
                await controller.DisposeAsync();
            }
        }

        return failures.Count == 0 && checks.Values.All(value => value);
    }

    private static async Task WaitForAsync(
        Func<bool> condition,
        TimeSpan timeout,
        string failureMessage)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException(failureMessage);
    }

    private static string RequireArgument(
        IReadOnlyList<string> args,
        string name) =>
        GetArgumentValue(args, name) is { Length: > 0 } value
            ? Path.GetFullPath(value)
            : throw new ArgumentException($"Missing required argument '{name}'.");

    private static int ParseIntArgument(
        string? value,
        int defaultValue,
        int minimum,
        int maximum)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (!int.TryParse(value, out var parsed) || parsed < minimum || parsed > maximum)
        {
            throw new ArgumentException(
                $"Integration smoke port/hold value must be between {minimum} and {maximum}.");
        }

        return parsed;
    }

    private static string? GetArgumentValue(
        IReadOnlyList<string> args,
        string name)
    {
        for (var index = 0; index < args.Count - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private static void CaptureWindow(Window window, string path)
    {
        window.UpdateLayout();
        var width = Math.Max(1, (int)Math.Ceiling(window.ActualWidth));
        var height = Math.Max(1, (int)Math.Ceiling(window.ActualHeight));
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(window);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        var fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        using var stream = File.Create(fullPath);
        encoder.Save(stream);
    }
}
