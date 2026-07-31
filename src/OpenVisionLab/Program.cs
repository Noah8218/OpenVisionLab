// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 최노아(Noah-Choi)

using System;
using System.Windows;

namespace OpenVisionLab
{
    public static class Program
    {
        /// <summary>
        /// 애플리케이션 진입점입니다.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            try
            {
                AppPathService.Initialize();
                OpenVisionLanguageService.ConfigureDataDirectory(
                    AppPathService.ConfigRootDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "OpenVisionLab could not initialize its writable data folder."
                    + Environment.NewLine
                    + "OpenVisionLab이 쓰기 데이터 폴더를 초기화하지 못했습니다."
                    + Environment.NewLine
                    + Environment.NewLine
                    + ex.Message,
                    "OpenVisionLab data folder",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return 2;
            }

            #if OPENVISIONLAB_EMBEDDED_SMOKE
            if (OpenVisionLabDirectSmokeRunner.TryRun(args))
            {
                return 0;
            }
            #endif

            _ = typeof(OpenVisionShellHostWindow);
            return new OpenVisionLabApplication().Run(args);
        }
    }
}
