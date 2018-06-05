using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using KitchenSink.Extensions;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// Collects and atomically applies a set of updates to Refs.
    /// </summary>
    public class Tran : IDisposable
    {
        public enum TranStatus
        {
            Running, Committed, RolledBack
        }

        private readonly Mutex mutex;
        private readonly ISet<Ref> refs = new HashSet<Ref>();
        private IDisposable pop;

        /// <summary>
        /// Creates a new Tran with given ambiance.
        /// </summary>
        internal Tran(Mutex mutex, bool ambient)
        {
            this.mutex = mutex;
            IsAmbient = ambient;

            if (ambient)
            {
                pop = Scope.Push(this);
            }
        }

        public TranStatus Status { get; private set; } = TranStatus.Running;

        /// <summary>
        /// Ambient transactions are referenced in Ref
        /// updates without being explicitly passed.
        /// </summary>
        public bool IsAmbient { get; }

        internal bool Join(Ref r) => refs.Add(r);

        /// <summary>
        /// Attempts to apply prepared changes in order.
        /// If any fail, all locks will be released and
        /// all Refs will retain previous value.
        /// </summary>
        public void Commit()
        {
            refs.ForEach(r => r.Commit());
            Status = TranStatus.Committed;
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Releases lock on all Refs in Tran and
        /// discards tentative values.
        /// </summary>
        public void Rollback()
        {
            refs.ForEach(r => r.Rollback());
            Status = TranStatus.RolledBack;
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Attempts Commit if no exception was raised
        /// or rolls back if there was an exception.
        /// </summary>
        public void Dispose()
        {
            pop?.Dispose();
            pop = null;

            if (Status == TranStatus.Running)
            {
                if (IsErrorState)
                {
                    Rollback();
                }
                else
                {
                    Commit();
                }
            }
        }

        private static bool IsErrorState =>
            Marshal.GetExceptionPointers() != IntPtr.Zero
            || Marshal.GetExceptionCode() != 0;
    }
}
