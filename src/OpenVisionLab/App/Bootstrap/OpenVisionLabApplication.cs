// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 Noah-Choi

using OpenVisionLab.Core;
using OpenVisionLab.Logging;
using System;
using System.Windows;

namespace OpenVisionLab
{
    internal sealed class OpenVisionLabApplication
    {
        private const string SingleInstanceMutexName = "OpenVisionLab";

        public int Run(string[] args)
        {
            using OpenVisionLabSingleInstanceGuard singleInstance =
                OpenVisionLabSingleInstanceGuard.TryCreate(SingleInstanceMutexName);

            if (!singleInstance.IsOwner)
            {
                MessageBox.Show(
                    "Program Already Running",
                    "Check Job Process",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return 1;
            }

            var application = Application.Current ?? new Application();
            ShutdownMode previousShutdownMode = application.ShutdownMode;
            OpenVisionStartupLoadingWindow startupLoadingWindow = null;
            ApplicationRuntimeContext runtimeContext = null;

            try
            {
                application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                using var exceptionPolicy = OpenVisionLabUnhandledExceptionPolicy.Attach(application);

                startupLoadingWindow = new OpenVisionStartupLoadingWindow();
                startupLoadingWindow.ShowReady();

                runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.RestoreLastRecipe();
                runtimeContext.Global.System.ApplyLogConfig();
                OVLog.Write(
                    LogCategory.System,
                    LogLevel.Info,
                    $"Runtime data root: {AppPathService.DataRootDirectory}");
                if (!string.IsNullOrWhiteSpace(AppPathService.MigrationNotice))
                {
                    OVLog.Write(
                        LogCategory.System,
                        LogLevel.Warning,
                        AppPathService.MigrationNotice);
                }
                OVLog.Write(LogCategory.System, LogLevel.Info, $"Application ready. Version {AppVersion.VERSION}");

                var shellWindow = new OpenVisionShellHostWindow(runtimeContext);
                application.MainWindow = shellWindow;
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                EventHandler startupContentRendered = null;
                startupContentRendered = async (_, _) =>
                {
                    shellWindow.ContentRendered -= startupContentRendered;
                    try
                    {
                        await shellWindow.StartupPreparationTask;
                    }
                    catch (Exception ex)
                    {
                        OVLog.Write(
                            LogCategory.System,
                            LogLevel.Warning,
                            "Pipeline Review startup preparation was skipped: " + ex.Message);
                    }
                    finally
                    {
                        startupLoadingWindow?.Complete();
                        startupLoadingWindow = null;
                        shellWindow.Activate();
                    }

                    if (OpenVisionTcpIntegrationExeSmoke.IsRequested(args))
                    {
                        var smokePassed = await OpenVisionTcpIntegrationExeSmoke.RunAsync(
                            shellWindow,
                            args);
                        Environment.ExitCode = smokePassed ? 0 : 1;
                        application.Shutdown(Environment.ExitCode);
                    }
                };
                shellWindow.ContentRendered += startupContentRendered;

                int exitCode = application.Run(shellWindow);

                OVLog.Write(LogCategory.System, LogLevel.Info, "Application shutdown.");
                return exitCode;
            }
            finally
            {
                startupLoadingWindow?.Complete();
                if (!application.Dispatcher.HasShutdownStarted
                    && !application.Dispatcher.HasShutdownFinished)
                {
                    application.ShutdownMode = previousShutdownMode;
                }

                // The shell view releases only resources it owns. The default
                // DisplayManager is process-scoped and is finalized here by the
                // application bootstrap after the WPF dispatcher has stopped.
                (runtimeContext?.DisplayManager as IDisposable)?.Dispose();
            }
        }

    }
}
