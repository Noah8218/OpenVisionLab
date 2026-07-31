using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.Common;

namespace OpenVisionLab
{
    public sealed class BackgroundLoopWorker : IDisposable
    {
        private readonly object sync = new object();
        private readonly string name;
        private CancellationTokenSource cancellation;
        private Task workerTask;

        public BackgroundLoopWorker(string name)
        {
            this.name = name;
        }

        public bool IsRunning
        {
            get
            {
                lock (sync)
                {
                    return workerTask != null && !workerTask.IsCompleted;
                }
            }
        }

        public bool Start(Func<CancellationToken, Task> work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            lock (sync)
            {
                if (workerTask != null && !workerTask.IsCompleted)
                {
                    return false;
                }

                cancellation?.Dispose();
                cancellation = new CancellationTokenSource();
                CancellationToken token = cancellation.Token;

                workerTask = Task.Run(async () =>
                {

                    try
                    {
                        await work(token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                    }
                }, token);

                return true;
            }
        }

        public bool Stop(int timeoutMilliseconds = 1000)
        {
            Task taskToWait = null;

            lock (sync)
            {
                if (workerTask == null || workerTask.IsCompleted)
                {
                    return true;
                }

                cancellation?.Cancel();
                taskToWait = workerTask;
            }

            try
            {
                return taskToWait.Wait(timeoutMilliseconds);
            }
            catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
            {
                return true;
            }
        }

        public void Dispose()
        {
            Stop();
            cancellation?.Dispose();
        }
    }
}
