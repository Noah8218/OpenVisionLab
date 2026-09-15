#nullable enable

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

internal static class SmokeTaskWaiter
{
    internal static void Wait(
        Task task,
        string description,
        Action pump,
        TimeSpan timeout,
        TimeSpan iterationDelay,
        string timeoutMessageSuffix)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        if (pump == null)
        {
            throw new ArgumentNullException(nameof(pump));
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!task.IsCompleted && stopwatch.Elapsed < timeout)
        {
            pump();
            if (iterationDelay > TimeSpan.Zero)
            {
                Thread.Sleep(iterationDelay);
            }
        }

        if (!task.IsCompleted)
        {
            throw new TimeoutException(description + timeoutMessageSuffix);
        }

        task.GetAwaiter().GetResult();
    }
}
