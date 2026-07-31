using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;

namespace OpenVisionLab.Logging.Model
{
    public sealed class RuntimeLogStream : IDisposable
    {
        private readonly RuntimeLogSink _appender;
        private readonly Hierarchy _logRepository;
        private bool _disposed;

        public RuntimeLogStream()
        {
            _appender = new RuntimeLogSink();
            _logRepository = (Hierarchy)LogManager.GetRepository();
            _logRepository.Root.AddAppender(_appender);
            _logRepository.Root.Level = Level.All;
            _logRepository.Configured = true;
            _logRepository.RaiseConfigurationChanged(EventArgs.Empty);
        }

        public string GetLog() => _disposed ? string.Empty : _appender.ReadBuffer();

        public string[] GetLogs() => GetLog().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _logRepository.Root.RemoveAppender(_appender);
            _logRepository.RaiseConfigurationChanged(EventArgs.Empty);
            _appender.Close();
            _disposed = true;
        }
    }
}
