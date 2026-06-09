using Lib.Common;
using OpenVisionLab._1._Core;
using OpenVisionLab.Logging;
using RJCodeUI_M1;
using RJCodeUI_M1.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenVisionLab
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(true, "OpenVisionLab", out bool bNew);
            if (bNew)
            {
                if (null == System.Windows.Application.Current)
                {
                    new System.Windows.Application().ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                }

                Application.ThreadException += Application_ThreadException;
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //UIAppearance.FormBorderSize = 5;

                SettingsManager.LoadApperanceSettings();//Load current appearance settings.

                ApplicationRuntimeContext runtimeContext = ApplicationRuntimeContext.CreateDefault();
                runtimeContext.Global.System.ApplyLogConfig();
                CLog.Write(LogCategory.Info, LogLevel.Main, $"Application ready. Version {AppVersion.VERSION}");

                StartupSplashScreen splashScreen = StartupSplashScreen.Start(
                    $"VERSION : {AppVersion.VERSION} - {AppVersion.DATETIME_UPDATED} ({AppVersion.MANAGER})",
                    null);
#if Release

#endif
                Application.Run(new FormMainFrame(splashScreen.Form, runtimeContext));
                CLog.Write(LogCategory.Info, LogLevel.Main, "Application shutdown.");
                splashScreen.Dispose();

                mutex.ReleaseMutex();
            }
            else
            {
                AppCommon.ShowdialogMessageBox("Program Already Running", "Check Job Process");

                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CLog.Write(LogCategory.Fatal, LogLevel.Abnormal, e.ExceptionObject?.ToString() ?? "Unhandled domain exception.");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            CLog.Write(LogCategory.Fatal, LogLevel.Abnormal, e.Exception?.ToString() ?? "Unhandled UI thread exception.");
        }

        private sealed class StartupSplashScreen : IDisposable
        {
            private readonly ManualResetEventSlim formReady = new ManualResetEventSlim(false);
            private readonly Thread thread;
            private readonly string versionText;
            private readonly Action<string> versionLogAction;
            private FormInit form;

            private StartupSplashScreen(string versionText, Action<string> versionLogAction)
            {
                this.versionText = versionText;
                this.versionLogAction = versionLogAction;
                thread = new Thread(Run);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Name = "OpenVisionLab.Init";
            }

            public FormInit Form
            {
                get
                {
                    formReady.Wait();
                    return form;
                }
            }

            public static StartupSplashScreen Start(string versionText, Action<string> versionLogAction)
            {
                StartupSplashScreen splashScreen = new StartupSplashScreen(versionText, versionLogAction);
                splashScreen.thread.Start();
                splashScreen.formReady.Wait();
                return splashScreen;
            }

            public void Dispose()
            {
                Close();
                formReady.Dispose();
            }

            private void Run()
            {
                try
                {
                    form = new FormInit
                    {
                        VersionText = versionText,
                        VersionLogAction = versionLogAction
                    };
                    form.Shown += (sender, e) => formReady.Set();
                    form.FormClosed += (sender, e) => Application.ExitThread();
                    Application.Run(form);
                }
                catch
                {
                    formReady.Set();
                }
            }

            private void Close()
            {
                formReady.Wait();
                if (form == null || form.IsDisposed)
                {
                    return;
                }

                try
                {
                    form.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
                    {
                        if (!form.IsDisposed)
                        {
                            form.Close();
                        }
                    }));
                }
                catch
                {
                }
            }
        }
    }
}
