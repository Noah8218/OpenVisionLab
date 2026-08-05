// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 Noah-Choi

using OpenVisionLab.Logging;
using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionLabUnhandledExceptionPolicy : IDisposable
    {
        private readonly Application _application;

        private OpenVisionLabUnhandledExceptionPolicy(Application application)
        {
            _application = application;
            _application.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public static IDisposable Attach(Application application)
        {
            return new OpenVisionLabUnhandledExceptionPolicy(application);
        }

        public void Dispose()
        {
            _application.DispatcherUnhandledException -= Application_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            GC.SuppressFinalize(this);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OVLog.Write(
                LogCategory.System,
                LogLevel.Error,
                e.ExceptionObject?.ToString() ?? "Unhandled domain exception.");
        }

        private static void Application_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            OVLog.Write(
                LogCategory.System,
                LogLevel.Error,
                e.Exception?.ToString() ?? "Unhandled UI thread exception.");
            e.Handled = IsRecoverableDispatcherException(e.Exception);
        }

        internal static bool IsRecoverableDispatcherException(Exception exception)
        {
            return exception is OperationCanceledException;
        }
    }
}
