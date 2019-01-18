// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base service implements <see cref="Interfaces.IMixedRealityService"/> and provides default properties for all services.
    /// </summary>
    public abstract class BaseService : Interfaces.IMixedRealityService
    {
        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public virtual string Name { get; protected set; }

        /// <inheritdoc />
        public virtual uint Priority { get; protected set; } = 5;

        /// <inheritdoc />
        public virtual void Initialize() { }

        /// <inheritdoc />
        public virtual void Reset() { }

        /// <inheritdoc />
        public virtual void Enable() { }

        /// <inheritdoc />
        public virtual void Update() { }

        /// <inheritdoc />
        public virtual void Disable() { }

        /// <inheritdoc />
        public virtual void Destroy() { }

        #endregion IMixedRealityService Implementation

        #region IDisposable Implementation

        private bool disposed;

        ~BaseService()
        {
            OnDispose(true);
        }

        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            GC.SuppressFinalize(this);
            OnDispose(false);
        }

        protected virtual void OnDispose(bool finalizing) { }

        #endregion IDisposable Implementation
    }
}
