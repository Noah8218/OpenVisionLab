using Lib.Common;
using log4net;
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
        public static Form MainForm;//Gets or sets the primary form of the application        

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

                try
                {
                    //UIAppearance.FormBorderSize = 5;

                    SettingsManager.LoadApperanceSettings();//Load current appearance settings.

                    FormInit formInit = new FormInit();
#if Release

#endif
                    var Task = System.Threading.Tasks.Task.Run(() =>
                    {
                        Application.Run(formInit);
                    });

                    Application.Run(new FormMetroFrame(formInit));
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL( "Ex ==> {0}", Desc.Message);
                }

                mutex.ReleaseMutex();
            }
            else
            {
                CCommon.ShowdialogMessageBox("Program Already Running", "Check Job Process");

                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //CLog.Error( "Ex ==> {0}", e.ToString());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //CLog.Error( "Ex ==> {0}", e.ToString());
        }
    }
}
