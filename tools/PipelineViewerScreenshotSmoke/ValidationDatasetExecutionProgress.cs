using System;
using System.Globalization;
using System.IO;

internal static class ValidationDatasetExecutionProgress
{
    internal static void Run(
        string progressPath,
        int registeredOkCount,
        int registeredNgCount,
        Func<bool> canExecute,
        Action execute,
        Func<string> statusText,
        Func<bool> hasSavedRun,
        Action<int> pump,
        Func<DateTime> utcNow,
        Func<DateTime> localNow,
        TimeSpan timeout,
        TimeSpan progressInterval)
    {
        File.WriteAllText(
            progressPath,
            $"Registered OK {registeredOkCount} / NG {registeredNgCount}{Environment.NewLine}");
        execute();
        DateTime deadline = utcNow().Add(timeout);
        DateTime nextProgressWrite = DateTime.MinValue;
        while (utcNow() < deadline)
        {
            pump(20);
            if (utcNow() >= nextProgressWrite)
            {
                File.AppendAllText(
                    progressPath,
                    localNow().ToString("HH:mm:ss", CultureInfo.InvariantCulture)
                    + " | "
                    + statusText()
                    + Environment.NewLine);
                nextProgressWrite = utcNow().Add(progressInterval);
            }

            if (canExecute() && hasSavedRun())
            {
                break;
            }
        }
    }
}
