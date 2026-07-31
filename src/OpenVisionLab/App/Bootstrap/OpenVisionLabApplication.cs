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
            if (TryRunEmbeddedSmoke(args))
            {
                return 0;
            }

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

            try
            {
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
                using var exceptionPolicy = OpenVisionLabUnhandledExceptionPolicy.Attach(application);

                var runtimeContext = ApplicationRuntimeContext.CreateDefault();
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

                application.Run(new OpenVisionShellHostWindow(runtimeContext));

                OVLog.Write(LogCategory.System, LogLevel.Info, "Application shutdown.");
                return 0;
            }
            finally
            {
                if (!application.Dispatcher.HasShutdownStarted
                    && !application.Dispatcher.HasShutdownFinished)
                {
                    application.ShutdownMode = previousShutdownMode;
                }
            }
        }

        private static bool TryRunEmbeddedSmoke(string[] args)
        {
#if OPENVISIONLAB_EMBEDDED_SMOKE
            return OpenVisionLabDirectSmokeRunner.TryRun(args);
#else
            return false;
#endif
        }
    }
}
