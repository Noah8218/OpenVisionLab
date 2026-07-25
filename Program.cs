// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 최노아(Noah-Choi)

using System;

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
