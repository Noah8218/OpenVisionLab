// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 최노아(Noah-Choi)

using OpenVisionLab._1._Core;
using OpenVisionLab.Logging;
using System;
using System.Threading;

namespace OpenVisionLab
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (OpenVisionLabDirectSmokeRunner.TryRun(args))
            {
                return;
            }

            Mutex mutex = new Mutex(true, "OpenVisionLab", out bool isNewInstance);
            if (!isNewInstance)
            {
                System.Windows.MessageBox.Show(
                    "Program Already Running",
                    "Check Job Process",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
                return;
            }

            try
            {
                System.Windows.Application app = System.Windows.Application.Current ?? new System.Windows.Application();
                app.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
                app.DispatcherUnhandledException += Application_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                OpenVisionLanguageService.Load();
                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.System.ApplyLogConfig();
                OVLog.Write(LogCategory.System, LogLevel.Info, $"Application ready. Version {AppVersion.VERSION}");

                app.Run(new OpenVisionShellHostWindow(runtimeContext));

                OVLog.Write(LogCategory.System, LogLevel.Info, "Application shutdown.");
            }
            finally
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OVLog.Write(LogCategory.System, LogLevel.Error, e.ExceptionObject?.ToString() ?? "Unhandled domain exception.");
        }

        private static void Application_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            OVLog.Write(LogCategory.System, LogLevel.Error, e.Exception?.ToString() ?? "Unhandled UI thread exception.");
            e.Handled = true;
        }
    }
}
