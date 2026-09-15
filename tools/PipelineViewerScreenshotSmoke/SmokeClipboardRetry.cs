#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Threading;

internal static class SmokeClipboardRetry
{
    internal static T Run<T>(Func<T> action, Action pump)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (pump == null)
        {
            throw new ArgumentNullException(nameof(pump));
        }

        COMException? lastException = null;
        for (int attempt = 0; attempt < 40; attempt++)
        {
            try
            {
                return action();
            }
            catch (COMException ex) when ((uint)ex.ErrorCode == 0x800401D0)
            {
                lastException = ex;
                pump();
                Thread.Sleep(Math.Min(250, 50 + attempt * 10));
            }
        }

        throw lastException ?? new COMException("Clipboard operation failed.");
    }
}
