using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Linq;
using System.Threading;

namespace OpenVisionLab.Logging.Model
{
    public sealed class RuntimeLogStream : IDisposable
    {
        private readonly RuntimeLogSink _appender;
        private readonly Hierarchy _logRepository;
        private int _disposed;

        public RuntimeLogStream()
        {
            _appender = new RuntimeLogSink();
            _logRepository = (Hierarchy)LogManager.GetRepository();
            _logRepository.Root.AddAppender(_appender);
            _logRepository.Root.Level = Level.All;
            _logRepository.Configured = true;
            _logRepository.RaiseConfigurationChanged(EventArgs.Empty);
        }

        public string GetLog() => Volatile.Read(ref _disposed) != 0 ? string.Empty : _appender.ReadBuffer();

        public string[] GetLogs() => Volatile.Read(ref _disposed) != 0
            ? Array.Empty<string>()
            : _appender.ReadEntries().ToArray();

        public long DroppedLogCount => _appender.DroppedLogCount;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _logRepository.Root.RemoveAppender(_appender);
            _logRepository.RaiseConfigurationChanged(EventArgs.Empty);
            _appender.Close();
        }
    }
}
