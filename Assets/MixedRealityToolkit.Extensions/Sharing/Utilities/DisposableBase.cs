// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// This is a base class for common IDisposable implementation.
    /// </summary>
    /// <remarks>Follows https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose </remarks>
    public class DisposableBase : IDisposable
    {
        private string objectName;
        private ThreadLocal<bool> insideDisposeFunction = new ThreadLocal<bool>(() => false);

        /// <summary>
        /// Is the current object disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The name of the current object.
        /// </summary>
        protected virtual string ObjectName => objectName ?? (objectName = GetType().Name);

        ~DisposableBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the current object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            // If the finalizer is running, don't access the insideDisposeFunction, as it will
            // also be finalizing.
            if (isDisposing)
            {
                insideDisposeFunction = null;
            }
            else
            {
                insideDisposeFunction.Value = true;
            }

            try
            {
                if (isDisposing)
                {
                    OnManagedDispose();
                }

                OnUnmanagedDispose();
            }
            finally
            {
                if (insideDisposeFunction != null)
                {
                    insideDisposeFunction.Value = false;
                }
            }
        }

        /// <summary>
        /// Override this method to dispose of managed objects.
        /// </summary>
        protected virtual void OnManagedDispose() { }

        /// <summary>
        /// Override this method to dispose of unmanaged objects.
        /// </summary>
        protected virtual void OnUnmanagedDispose() { }

        /// <summary>
        /// A helper method to throw if the current object is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (insideDisposeFunction == null || (!insideDisposeFunction.Value && IsDisposed))
            {
                throw new ObjectDisposedException(ObjectName);
            }
        }
    }
}
