// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The base service implements <see cref="IMixedRealityService"/> and provides default properties for all services.
    /// </summary>
    public abstract class BaseService : IMixedRealityService
    {
        public const uint DefaultPriority = 10;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public virtual string Name { get; protected set; }

        /// <inheritdoc />
        public virtual uint Priority { get; protected set; } = DefaultPriority;

        /// <inheritdoc />
        public virtual BaseMixedRealityProfile ConfigurationProfile { get; protected set; } = null;

        /// <inheritdoc />
        public virtual void Initialize() { }

        /// <inheritdoc />
        public virtual void Reset() { }

        /// <inheritdoc />
        public virtual void Enable() { }

        /// <inheritdoc />
        public virtual void Update() { }

        /// <inheritdoc />
        public virtual void LateUpdate() { }

        /// <inheritdoc />
        public virtual void Disable() { }

        /// <inheritdoc />
        public virtual void Destroy() { }

        #endregion IMixedRealityService Implementation

        #region IDisposable Implementation

        /// <summary>
        /// Value indicating if the object has completed disposal.
        /// </summary>
        /// <remarks>
        /// Set by derived classes to indicate that disposal has been completed.
        /// </remarks>
        protected bool disposed = false;

        /// <summary>
        /// Finalizer
        /// </summary>
        ~BaseService()
        {
            Dispose();
        }

        /// <summary>
        /// Cleanup resources used by this object.
        /// </summary>
        public void Dispose()
        {
            // Clean up our resources (managed and unmanaged resources)
            Dispose(true);

            // Suppress finalization as the finalizer also calls our cleanup code.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleanup resources used by the object
        /// </summary>
        /// <param name="disposing">Are we fully disposing the object? 
        /// True will release all managed resources, unmanaged resources are always released.
        /// </param>
        protected virtual void Dispose(bool disposing) { }
    
        #endregion IDisposable Implementation
    }
}
