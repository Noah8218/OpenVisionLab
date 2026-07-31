// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 Noah-Choi

using System;
using System.Threading;

namespace OpenVisionLab
{
    internal sealed class OpenVisionLabSingleInstanceGuard : IDisposable
    {
        private readonly Mutex _mutex;
        private bool _disposed;

        private OpenVisionLabSingleInstanceGuard(Mutex mutex, bool isOwner)
        {
            _mutex = mutex;
            IsOwner = isOwner;
        }

        public bool IsOwner { get; }

        public static OpenVisionLabSingleInstanceGuard TryCreate(string mutexName)
        {
            var mutex = new Mutex(true, mutexName, out bool isOwner);
            return new OpenVisionLabSingleInstanceGuard(mutex, isOwner);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (IsOwner)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch
                {
                    // Ignore expected failures when shutdown races with mutex ownership.
                }
            }

            _mutex.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
